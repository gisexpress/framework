using System.Collections.Generic;
using System.Diagnostics;
using System.Geometries.Algorithm;
using System.Geometries.Algorithm.Locate;
using System.Geometries.Graph.Index;

namespace System.Geometries.Graph
{
    /// <summary>
    /// A GeometryGraph is a graph that models a given Geometry.
    /// </summary>
    internal class GeometryGraph : PlanarGraph
    {
        public GeometryGraph(int argIndex, IGeometry parent)
            : this(argIndex, parent, BoundaryNodeRules.OgcSfsBoundaryRule)
        {
        }

        public GeometryGraph(int argIndex, IGeometry parent, IBoundaryNodeRule boundaryNodeRule)
        {
            ArgIndex = argIndex;
            NodeRule = boundaryNodeRule;
            Geometry = parent;

            if (parent == null)
            {
                return;
            }

            Add(parent);
        }

        public static Locations DetermineBoundary(IBoundaryNodeRule boundaryNodeRule, int boundaryCount)
        {
            return boundaryNodeRule.IsInBoundary(boundaryCount) ? Locations.Boundary : Locations.Interior;
        }

        protected static EdgeSetIntersector CreateEdgeSetIntersector()
        {
            // various options for computing intersections, from slowest to fastest
            return new SimpleMCSweepLineIntersector();
        }

        /// <summary>
        /// The lineEdgeMap is a map of the linestring components of the
        /// parentGeometry to the edges which are derived from them.
        /// This is used to efficiently perform findEdge queries
        /// </summary>
        protected readonly IDictionary<ILineString, Edge> LineEdgeMap = new Dictionary<ILineString, Edge>();

        protected readonly IBoundaryNodeRule NodeRule;

        /// <summary>
        /// If this flag is true, the Boundary Determination Rule will used when deciding whether nodes are in the boundary or not
        /// </summary>
        protected bool UseBoundaryDeterminationRule = true;

        protected readonly int ArgIndex;  // the index of this point as an argument to a spatial function (used for labelling)
        protected IList<Node> BoundaryNodeList;

        protected IPointOnGeometryLocator AreaLocator;
        protected readonly PointLocator Locator = new PointLocator();

        public bool HasTooFewPoints
        {
            get;
            protected set;
        }

        public ICoordinate InvalidPoint
        {
            get;
            protected set;
        }

        public IGeometry Geometry
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the <see cref="IBoundaryNodeRule"/> used with this geometry graph.
        /// </summary>
        public IBoundaryNodeRule BoundaryNodeRule
        {
            get { return NodeRule; }
        }

        public IList<Node> BoundaryNodes
        {
            get
            {
                if (BoundaryNodeList == null)
                    BoundaryNodeList = NodeMap.GetBoundaryNodes(ArgIndex);

                return BoundaryNodeList;
            }
        }

        public ICoordinate[] GetBoundaryPoints()
        {
            var coll = BoundaryNodes;
            ICoordinate[] pts = new Coordinate[coll.Count];
            int i = 0;
            foreach (Node node in coll)
            {
                pts[i++] = node.Coordinate.Clone();
            }
            return pts;
        }

        public Edge FindEdge(LineString line)
        {
            return LineEdgeMap[line];
        }

        public void ComputeSplitEdges(IList<Edge> edgelist)
        {
            foreach (Edge e in Edges)
            {
                e.EdgeIntersectionList.AddSplitEdges(edgelist);
            }
        }

        void Add(IGeometry g)
        {
            // Check if this Geometry should obey the Boundary Determination Rule all collections except MultiPolygons obey the rule
            if (g is MultiPolygon)
            {
                UseBoundaryDeterminationRule = false;
            }

            if (AddPolygon(g as Polygon))
            {
                return;
            }

            if (AddRing(g as LinearRing))
            {
                return;
            }

            if (AddLineString(g as LineString))
            {
                return;
            }

            if (AddPoint(g as Point))
            {
                return;
            }

            if (AddCollection(g as IGeometryCollection))
            {
                return;
            }

            throw new NotSupportedException(g.GetType().FullName);
        }

        bool AddCollection(IGeometryCollection collection)
        {
            if (collection == null)
            {
                return false;
            }

            for (int i = 0; i < collection.NumGeometries(); i++)
            {
                Add(collection.GetGeometryN(i));
            }

            return true;
        }

        bool AddPoint(Point p)
        {
            if (p == null)
            {
                return false;
            }

            InsertPoint(ArgIndex, p.Coordinate, Locations.Interior);
            return true;
        }

        /// <summary>
        /// Adds a polygon ring to the graph. Empty rings are ignored.
        /// The left and right topological location arguments assume that the ring is oriented CW.
        /// If the ring is in the opposite orientation,
        /// the left and right locations must be interchanged.
        /// </summary>
        void AddPolygonRing(ILinearRing ring, Locations cwLeft, Locations cwRight)
        {
            if (ring.NumPoints() < 4)
            {
                HasTooFewPoints = true;
                InvalidPoint = ring.Coordinates.Get(0);
                return;
            }

            Locations left = cwLeft;
            Locations right = cwRight;

            if (CGAlgorithms.IsCCW(ring.Coordinates))
            {
                left = cwRight;
                right = cwLeft;
            }

            Edge e = new Edge(ring.Coordinates, new Label(ArgIndex, Locations.Boundary, left, right));

            LineEdgeMap[ring] = e;
            InsertEdge(e);

            // insert the endpoint as a node, to mark that it is on the boundary
            InsertPoint(ArgIndex, ring.Coordinates.StartPoint, Locations.Boundary);
        }

        bool AddPolygon(IPolygon poly)
        {
            if (poly == null)
            {
                return false;
            }

            AddPolygonRing(poly.ExteriorRing, Locations.Exterior, Locations.Interior);

            for (int i = 0; i < poly.InteriorRings.Count; i++)
            {
                AddPolygonRing(poly.InteriorRings.Get(i), Locations.Interior, Locations.Exterior);
            }

            return true;
        }

        bool AddRing(LinearRing ring)
        {
            if (ring == null)
            {
                return false;
            }

            AddPolygonRing(ring, Locations.Exterior, Locations.Interior);
            return true;
        }

        bool AddLineString(LineString line)
        {
            if (line == null)
            {
                return false;
            }

            if (line.NumPoints() < 2)
            {
                HasTooFewPoints = true;
                InvalidPoint = line.StartPoint;
                return true;
            }

            // add the edge for the LineString
            // line edges do not have locations for their left and right sides
            Edge e = new Edge(line.Coordinates, new Label(ArgIndex, Locations.Interior));

            LineEdgeMap[line] = e;
            InsertEdge(e);

            /*
            * Add the boundary points of the LineString, if any.
            * Even if the LineString is closed, add both points as if they were endpoints.
            * This allows for the case that the node already exists and is a boundary point.
            */
            Debug.Assert(line.NumPoints() >= 2, "found LineString with single point");

            InsertBoundaryPoint(ArgIndex, line.StartPoint);
            InsertBoundaryPoint(ArgIndex, line.EndPoint);

            return true;
        }

        /// <summary>
        /// Add an Edge computed externally.  The label on the Edge is assumed
        /// to be correct.
        /// </summary>
        /// <param name="e"></param>
        public void AddEdge(Edge e)
        {
            InsertEdge(e);
            // insert the endpoint as a node, to mark that it is on the boundary
            InsertPoint(ArgIndex, e.Sequence.StartPoint, Locations.Boundary);
            InsertPoint(ArgIndex, e.Sequence.EndPoint, Locations.Boundary);
        }

        /// <summary>
        /// Add a point computed externally.  The point is assumed to be a
        /// Point Geometry part, which has a location of INTERIOR.
        /// </summary>
        /// <param name="pt"></param>
        public void AddPoint(Coordinate pt)
        {
            InsertPoint(ArgIndex, pt, Locations.Interior);
        }

        /// <summary>
        /// Compute self-nodes, taking advantage of the Geometry type to
        /// minimize the number of intersection tests.  (E.g. rings are
        /// not tested for self-intersection, since they are assumed to be valid).
        /// </summary>
        /// <param name="li">The <c>LineIntersector</c> to use.</param>
        /// <param name="computeRingSelfNodes">If <c>false</c>, intersection checks are optimized to not test rings for self-intersection.</param>
        /// <returns>The computed SegmentIntersector, containing information about the intersections found.</returns>
        public SegmentIntersector ComputeSelfNodes(LineIntersector li, bool computeRingSelfNodes)
        {
            return ComputeSelfNodes(li, computeRingSelfNodes, false);
        }

        /**
         * Compute self-nodes, taking advantage of the Geometry type to
         * minimize the number of intersection tests.  (E.g. rings are
         * not tested for self-intersection, since they are assumed to be valid).
         * 
         * @param li the LineIntersector to use
         * @param computeRingSelfNodes if <false>, intersection checks are optimized to not test rings for self-intersection
         * @param isDoneIfProperInt short-circuit the intersection computation if a proper intersection is found
         * @return the computed SegmentIntersector containing information about the intersections found
         */
        public SegmentIntersector ComputeSelfNodes(LineIntersector li, bool computeRingSelfNodes, bool isDoneIfProperInt)
        {
            SegmentIntersector si = new SegmentIntersector(li, true, false);
            si.IsDoneIfProperInt = isDoneIfProperInt;
            EdgeSetIntersector esi = CreateEdgeSetIntersector();
            // optimize intersection search for valid Polygons and LinearRings
            var isRings = Geometry is LinearRing || Geometry is Polygon || Geometry is MultiPolygon;
            var computeAllSegments = computeRingSelfNodes || !isRings;
            esi.ComputeIntersections(Edges, si, computeAllSegments);

            //System.out.println("SegmentIntersector # tests = " + si.numTests);
            AddSelfIntersectionNodes(ArgIndex);
            return si;
        }

        public SegmentIntersector ComputeEdgeIntersections(GeometryGraph g, LineIntersector li, bool includeProper)
        {
            SegmentIntersector si = new SegmentIntersector(li, includeProper, true);
            si.SetBoundaryNodes(BoundaryNodes, g.BoundaryNodes);
            EdgeSetIntersector esi = CreateEdgeSetIntersector();
            esi.ComputeIntersections(Edges, g.Edges, si);
            return si;
        }

        void InsertPoint(int argIndex, ICoordinate coord, Locations onLocation)
        {
            Node n = NodeMap.AddNode(coord);
            Label lbl = n.Label;
            if (lbl == null)
                n.Label = new Label(argIndex, onLocation);
            else lbl.SetLocation(argIndex, onLocation);
        }

        /// <summary>
        /// Adds candidate boundary points using the current <see cref="IBoundaryNodeRule"/>.
        /// This is used to add the boundary
        /// points of dim-1 geometries (Curves/MultiCurves).
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="coord"></param>
        void InsertBoundaryPoint(int argIndex, ICoordinate coord)
        {
            var n = NodeMap.AddNode(coord);
            // nodes always have labels
            Label lbl = n.Label;
            // the new point to insert is on a boundary
            int boundaryCount = 1;
            // determine the current location for the point (if any)
            //Location loc = Location.Null;
            var loc = lbl.GetLocation(argIndex, Positions.On);
            if (loc == Locations.Boundary)
                boundaryCount++;

            // determine the boundary status of the point according to the Boundary Determination Rule
            Locations newLoc = DetermineBoundary(NodeRule, boundaryCount);
            lbl.SetLocation(argIndex, newLoc);
        }

        void AddSelfIntersectionNodes(int argIndex)
        {
            foreach (Edge e in Edges)
            {
                Locations eLoc = e.Label.GetLocation(argIndex);
                foreach (EdgeIntersection ei in e.EdgeIntersectionList)
                {
                    AddSelfIntersectionNode(argIndex, ei.Coordinate, eLoc);
                }
            }
        }

        /// <summary>
        /// Add a node for a self-intersection.
        /// If the node is a potential boundary node (e.g. came from an edge which
        /// is a boundary) then insert it as a potential boundary node.
        /// Otherwise, just add it as a regular node.
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="coord"></param>
        /// <param name="loc"></param>
        private void AddSelfIntersectionNode(int argIndex, Coordinate coord, Locations loc)
        {
            // if this node is already a boundary node, don't change it
            if (IsBoundaryNode(argIndex, coord))
                return;
            if (loc == Locations.Boundary && UseBoundaryDeterminationRule)
                InsertBoundaryPoint(argIndex, coord);
            else InsertPoint(argIndex, coord, loc);
        }

        // MD - experimental for now
        ///<summary>
        /// Determines the <see cref="Location"/> of the given <see cref="Coordinate"/> in this geometry.
        ///</summary>
        /// <param name="pt">The point to test</param>
        /// <returns>
        /// The location of the point in the geometry
        /// </returns>
        public Locations Locate(ICoordinate pt)
        {
            if (Geometry is IPolygonal && Geometry.NumGeometries() > 50)
            {
                // lazily init point locator
                if (AreaLocator == null)
                {
                    AreaLocator = new IndexedPointInAreaLocator(Geometry);
                }
                return AreaLocator.Locate(pt);
            }

            return Geometry.Locate(pt);
        }
    }
}