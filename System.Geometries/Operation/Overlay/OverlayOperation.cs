using System.Collections.Generic;
using System.Diagnostics;
using System.Geometries.Algorithm;
using System.Geometries.Graph;
using System.Linq;

namespace System.Geometries.Operation.Overlay
{
    /// <summary>
    /// Computes the geometric overlay of two <see cref="Geometry"/>s.
    /// The overlay can be used to determine any bool combination of the geometries.
    /// </summary>
    internal class OverlayOperation : GeometryGraphOperation
    {
        public OverlayOperation(SpatialFunctions function):base()
        {
            Function = function;
        }

        public static IGeometry Overlay(SpatialFunctions function, params IGeometry[] args)
        {
            return Overlay(function, false, args);
        }

        public static IGeometry Overlay(SpatialFunctions function, bool optimized, params IGeometry[] args)
        {
            var o = new OverlayOperation(function);

            if (o.Compute(optimized, args))
            {
                return o.Result;
            }

            return default(Geometry);
        }

        /// <summary>
        /// Disable <see cref="EdgeNodingValidator"/> 
        /// when an intersection is made (<see cref="ComputeOverlay"/>), 
        /// so performances are dramatically improved but failures are not managed.
        /// </summary>
        /// <remarks>
        /// Use ay your own risk!
        /// </remarks>        
        public bool NodingValidatorDisabled = true;

        PlanarGraph PGraph;
        PointLocator Locator;
        EdgeList EdgeList;

        IList<IGeometry> ResultPolyList;
        IList<IGeometry> ResultLineList;
        IList<IGeometry> ResultPointList;

        protected IGeometry Result;
        protected readonly SpatialFunctions Function;

        /// <summary>
        /// Gets the graph constructed to compute the overlay.
        /// </summary>
        public PlanarGraph Graph
        {
            get { return PGraph; }
        }

        protected override void OnInit(IGeometry[] args)
        {
            EdgeList = new EdgeList();
            Locator = new PointLocator();
            PGraph = new PlanarGraph(new OverlayNodeFactory());

            base.OnInit(args);
        }

        protected override bool OnCompute(params IGeometry[] args)
        {
            // Copy points from input Geometries.
            // This ensures that any Point geometries in the input are considered for inclusion in the result set.

            CopyPoints(0);
            CopyPoints(1);

            // Node the input Geometries
            Graphs[0].ComputeSelfNodes(Intersector, false);
            Graphs[1].ComputeSelfNodes(Intersector, false);

            // Compute intersections between edges of the two input geometries
            Graphs[0].ComputeEdgeIntersections(Graphs[1], Intersector, true);

            IList<Edge> baseSplitEdges = new List<Edge>();
            Graphs[0].ComputeSplitEdges(baseSplitEdges);
            Graphs[1].ComputeSplitEdges(baseSplitEdges);

            // Add the noded edges to this result graph
            InsertUniqueEdges(baseSplitEdges);

            ComputeLabelsFromDepths();
            ReplaceCollapsedEdges();

            if (NodingValidatorDisabled == false)
            {
                // Check that the noding completed correctly.
                // 
                // This test is slow, but necessary in order to catch robustness failure situations.
                // If an exception is thrown because of a noding failure, then snapping will be performed, which will hopefully avoid the problem.
                // In the future hopefully a faster check can be developed.  

                new EdgeNodingValidator(EdgeList.Edges).CheckValid();
            }

            PGraph.AddEdges(EdgeList.Edges);

            if (ComputeLabelling())
            {
                LabelIncompleteNodes();

                // The ordering of building the result Geometries is important.
                // Areas must be built before lines, which must be built before points.
                // This is so that lines which are covered by areas are not included explicitly, and similarly for points.

                FindResultAreaEdges(Function);
                CancelDuplicateResultEdges();
                var polyBuilder = new PolygonBuilder();

                if (polyBuilder.Add(PGraph))
                {
                    ResultPolyList = polyBuilder.Polygons;

                    var lineBuilder = new LineBuilder(this, Locator);
                    ResultLineList = lineBuilder.Build(Function);

                    var pointBuilder = new PointBuilder(this);
                    ResultPointList = pointBuilder.Build(Function);

                    // Gather the results from all calculations into a single Geometry for the result set
                    Result = ComputeGeometry(ResultPointList.Concat(ResultLineList).Concat(ResultPolyList));

                    return true;
                }
            }

            return false;
        }

        void InsertUniqueEdges(IEnumerable<Edge> edges)
        {
            for (var i = edges.GetEnumerator(); i.MoveNext();)
            {
                var e = i.Current;
                InsertUniqueEdge(e);
            }
        }

        /// <summary>
        /// Insert an edge from one of the noded input graphs.
        /// Checks edges that are inserted to see if an identical edge already exists.
        /// If so, the edge is not inserted, but its label is merged with the existing edge.
        /// </summary>
        /// <param name="e">The edge to insert</param>
        protected void InsertUniqueEdge(Edge e)
        {
            var existingEdge = EdgeList.FindEqualEdge(e);

            // If an identical edge already exists, simply update its label
            if (existingEdge == null)
            {
                // No matching existing edge was found add this new edge to the list of edges in this graph
                EdgeList.Add(e);
            }
            else
            {
                var existingLabel = existingEdge.Label;

                var labelToMerge = e.Label;
                // Check if new edge is in reverse direction to existing edge if so, must flip the label before merging it

                if (existingEdge.IsPointwiseEqual(e) == false)
                {
                    labelToMerge = new Label(e.Label);
                    labelToMerge.Flip();
                }

                var depth = existingEdge.Depth;

                // If this is the first duplicate found for this edge, initialize the depths

                if (depth.IsNull()) depth.Add(existingLabel);

                depth.Add(labelToMerge);
                existingLabel.Merge(labelToMerge);
            }
        }

        /// <summary>
        /// Update the labels for edges according to their depths.
        /// For each edge, the depths are first normalized.
        /// Then, if the depths for the edge are equal, this edge must have collapsed into a line edge.
        /// If the depths are not equal, update the label with the locations corresponding to the depths
        /// (i.e. a depth of 0 corresponds to a Locations of Exterior, a depth of 1 corresponds to Interior)
        /// </summary>
        void ComputeLabelsFromDepths()
        {
            for (var it = EdgeList.GetEnumerator(); it.MoveNext();)
            {
                var e = it.Current;
                var lbl = e.Label;
                var depth = e.Depth;
                /*
                * Only check edges for which there were duplicates,
                * since these are the only ones which might
                * be the result of dimensional collapses.
                */
                if (depth.IsNull())
                    continue;

                depth.Normalize();
                for (var i = 0; i < 2; i++)
                {
                    if (lbl.IsNull(i) || !lbl.IsArea() || depth.IsNull(i))
                        continue;
                    /*
                     * if the depths are equal, this edge is the result of
                     * the dimensional collapse of two or more edges.
                     * It has the same location on both sides of the edge,
                     * so it has collapsed to a line.
                     */
                    if (depth.GetDelta(i) == 0)
                        lbl.ToLine(i);
                    else
                    {
                        /*
                         * This edge may be the result of a dimensional collapse,
                         * but it still has different locations on both sides.  The
                         * label of the edge must be updated to reflect the resultant
                         * side locations indicated by the depth values.
                         */
                        Debug.Assert(!depth.IsNull(i, Positions.Left), "depth of Left side has not been initialized");
                        lbl.SetLocation(i, Positions.Left, depth.GetLocation(i, Positions.Left));
                        Debug.Assert(!depth.IsNull(i, Positions.Right), "depth of Right side has not been initialized");
                        lbl.SetLocation(i, Positions.Right, depth.GetLocation(i, Positions.Right));
                    }
                }
            }
        }

        /// <summary>
        /// If edges which have undergone dimensional collapse are found,
        /// replace them with a new edge which is a L edge
        /// </summary>
        void ReplaceCollapsedEdges()
        {
            var newEdges = new List<Edge>();
            var edgesToRemove = new List<Edge>();
            var it = EdgeList.GetEnumerator();
            while (it.MoveNext())
            {
                var e = it.Current;
                if (!e.IsCollapsed)
                    continue;
                // edgeList.Remove(it.Current as Edge); 
                // Diego Guidi says:
                // This instruction throws a "System.InvalidOperationException: Collection was modified; enumeration operation may not execute".
                // i try to not modify edgeList here, and remove all elements at the end of iteration.
                edgesToRemove.Add(it.Current);
                newEdges.Add(e.CollapsedEdge);
            }
            // Removing all collapsed edges at the end of iteration.
            foreach (Edge obj in edgesToRemove)
                EdgeList.Remove(obj);
            foreach (var obj in newEdges)
                EdgeList.Add(obj);
        }

        /// <summary>
        /// Copy all nodes from an arg point into this graph.
        /// The node label in the arg point overrides any previously computed
        /// label for that argIndex.
        /// (E.g. a node may be an intersection node with
        /// a previously computed label of Boundary,
        /// but in the original arg Geometry it is actually
        /// in the interior due to the Boundary Determination Rule)
        /// </summary>
        /// <param name="argIndex"></param>
        void CopyPoints(int argIndex)
        {
            var i = Graphs[argIndex].GetNodeEnumerator();

            while (i.MoveNext())
            {
                var graphNode = i.Current;
                var newNode = PGraph.AddNode(graphNode.Coordinate);
                newNode.SetLabel(argIndex, graphNode.Label.GetLocation(argIndex));
            }
        }

        /// <summary> 
        /// Compute initial labelling for all DirectedEdges at each node.
        /// In this step, DirectedEdges will acquire a complete labelling
        /// (i.e. one with labels for both Geometries)
        /// only if they
        /// are incident on a node which has edges for both Geometries
        /// </summary>
        bool ComputeLabelling()
        {
            var nodeit = PGraph.Nodes.GetEnumerator();

            while (nodeit.MoveNext())
            {
                var node = nodeit.Current;

                if (node.Edges.ComputeLabelling(Graphs))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            MergeSymLabels();
            UpdateNodeLabelling();

            return true;
        }

        /// <summary> 
        /// For nodes which have edges from only one Geometry incident on them,
        /// the previous step will have left their dirEdges with no labelling for the other
        /// Geometry.  However, the sym dirEdge may have a labelling for the other
        /// Geometry, so merge the two labels.
        /// </summary>
        void MergeSymLabels()
        {
            var nodeit = PGraph.Nodes.GetEnumerator();
            while (nodeit.MoveNext())
            {
                var node = nodeit.Current;
                ((DirectedEdgeStar)node.Edges).MergeSymLabels();
            }
        }

        void UpdateNodeLabelling()
        {
            // update the labels for nodes
            // The label for a node is updated from the edges incident on it
            // (Note that a node may have already been labelled
            // because it is a point in one of the input geometries)
            var nodeit = PGraph.Nodes.GetEnumerator();
            while (nodeit.MoveNext())
            {
                var node = nodeit.Current;
                var lbl = ((DirectedEdgeStar)node.Edges).Label;
                node.Label.Merge(lbl);
            }
        }

        /// <summary>
        /// Incomplete nodes are nodes whose labels are incomplete.
        /// (e.g. the location for one Geometry is null).
        /// These are either isolated nodes,
        /// or nodes which have edges from only a single Geometry incident on them.
        /// Isolated nodes are found because nodes in one graph which don't intersect
        /// nodes in the other are not completely labelled by the initial process
        /// of adding nodes to the nodeList.
        /// To complete the labelling we need to check for nodes that lie in the
        /// interior of edges, and in the interior of areas.
        /// When each node labelling is completed, the labelling of the incident
        /// edges is updated, to complete their labelling as well.
        /// </summary>
        void LabelIncompleteNodes()
        {
            //int nodeCount = 0;
            var ni = PGraph.Nodes.GetEnumerator();
            while (ni.MoveNext())
            {
                var n = ni.Current;
                var label = n.Label;
                if (n.IsIsolated)
                {
                    //nodeCount++;
                    if (label.IsNull(0))
                        LabelIncompleteNode(n, 0);
                    else LabelIncompleteNode(n, 1);
                }
                // now update the labelling for the DirectedEdges incident on this node
                ((DirectedEdgeStar)n.Edges).UpdateLabelling(label);
            }
            /*
            int nPoly0 = arg[0].getGeometry().getNumGeometries();
            int nPoly1 = arg[1].getGeometry().getNumGeometries();
            Console.WriteLine("# isolated nodes= " + nodeCount 
                    + "   # poly[0] = " + nPoly0
                    + "   # poly[1] = " + nPoly1);
            */

        }

        /// <summary>
        /// Label an isolated node with its relationship to the target point.
        /// </summary>
        void LabelIncompleteNode(GraphComponent n, int targetIndex)
        {
            n.Label.SetLocation(targetIndex, Graphs[targetIndex].Geometry.Locate(n.Coordinate));
        }

        /// <summary>
        /// Find all edges whose label indicates that they are in the result area(s),
        /// according to the operation being performed.  Since we want polygon shells to be
        /// oriented CW, choose dirEdges with the interior of the result on the RHS.
        /// Mark them as being in the result.
        /// Interior Area edges are the result of dimensional collapses.
        /// They do not form part of the result area boundary.
        /// </summary>
        void FindResultAreaEdges(SpatialFunctions opCode)
        {
            var it = PGraph.EdgeEnds.GetEnumerator();
            while (it.MoveNext())
            {
                var de = (DirectedEdge)it.Current;
                // mark all dirEdges with the appropriate label
                var label = de.Label;
                if (label.IsArea() && !de.IsInteriorAreaEdge &&
                    IsResultOfOp(label.GetLocation(0, Positions.Right), label.GetLocation(1, Positions.Right), opCode))
                    de.InResult = true;
            }
        }

        /// <summary>
        /// If both a dirEdge and its sym are marked as being in the result, cancel
        /// them out.
        /// </summary>
        void CancelDuplicateResultEdges()
        {
            // remove any dirEdges whose sym is also included
            // (they "cancel each other out")
            var it = PGraph.EdgeEnds.GetEnumerator();
            while (it.MoveNext())
            {
                var de = (DirectedEdge)it.Current;
                var sym = de.Directed;
                if (!de.IsInResult || !sym.IsInResult)
                    continue;

                de.InResult = false;
                sym.InResult = false;
            }
        }

        /// <summary>
        /// Tests if a point node should be included in the result or not.
        /// </summary>
        /// <param name="coord">The point coordinate</param>
        /// <returns><c>true</c> if the coordinate point is covered by a result Line or Area geometry.</returns>
        public bool IsCoveredByLA(ICoordinate coord)
        {
            if (IsCovered(coord, ResultLineList))
                return true;
            return IsCovered(coord, ResultPolyList);
        }
        /// <summary>
        /// Tests if an L edge should be included in the result or not.
        /// </summary>
        /// <param name="coord">The point coordinate</param>
        /// <returns><c>true</c> if the coordinate point is covered by a result Area geometry.</returns>
        public bool IsCoveredByA(ICoordinate coord)
        {
            return IsCovered(coord, ResultPolyList);
        }

        /// <returns>
        /// <c>true</c> if the coord is located in the interior or boundary of
        /// a point in the list.
        /// </returns>
        bool IsCovered(ICoordinate coord, IEnumerable<IGeometry> geomList)
        {
            var it = geomList.GetEnumerator();
            while (it.MoveNext())
            {
                var geom = it.Current;
                var loc = geom.Locate(coord);
                if (loc != Locations.Exterior)
                    return true;
            }
            return false;
        }

        IGeometry ComputeGeometry(IEnumerable<IGeometry> geometries)
        {
            // Element geometries of the result are always in the order Point,Curve,A
            var list = new List<IGeometry>(geometries);

            if (list.Count == 0)
            {
                return default;
            }

            // Build the most specific point possible
            return GeometryFactory.Default.BuildGeometry(list);
        }

        /// <summary>
        /// Tests whether a point with a given topological <see cref="Label"/> relative to two geometries is contained in the result of overlaying the geometries using a given overlay operation.
        /// <para/>
        /// The method handles arguments of <see cref="Locations.Null"/> correctly
        /// </summary>
        /// <param name="label">The topological label of the point</param>
        /// <param name="overlayOpCode">The code for the overlay operation to test</param>
        /// <returns><c>true</c> if the label locations correspond to the overlayOpCode</returns>
        public static bool IsResultOfOp(Label label, SpatialFunctions overlayOpCode)
        {
            return IsResultOfOp(label.GetLocation(0), label.GetLocation(1), overlayOpCode);
        }

        /// <summary>
        /// Tests whether a point with given <see cref="Locations"/>s relative to two geometries is contained in the result of overlaying the geometries using a given overlay operation.
        /// <para/>
        /// The method handles arguments of <see cref="Locations.Null"/> correctly
        /// </summary>
        /// <param name="loc0">the code for the location in the first geometry </param>
        /// <param name="loc1">the code for the location in the second geometry</param>
        /// <param name="overlayOpCode">the code for the overlay operation to test</param>
        /// <returns><c>true</c> if the locations correspond to the overlayOpCode.</returns>
        public static bool IsResultOfOp(Locations loc0, Locations loc1, SpatialFunctions overlayOpCode)
        {
            if (loc0 == Locations.Boundary) loc0 = Locations.Interior;
            if (loc1 == Locations.Boundary) loc1 = Locations.Interior;

            switch (overlayOpCode)
            {
                case SpatialFunctions.Intersection:
                    return loc0 == Locations.Interior && loc1 == Locations.Interior;

                case SpatialFunctions.Union:
                    return loc0 == Locations.Interior || loc1 == Locations.Interior;

                case SpatialFunctions.Difference:
                    return loc0 == Locations.Interior && loc1 != Locations.Interior;

                case SpatialFunctions.SymDifference:
                    return (loc0 == Locations.Interior && loc1 != Locations.Interior) || (loc0 != Locations.Interior && loc1 == Locations.Interior);
            }

            return false;
        }
    }
}
