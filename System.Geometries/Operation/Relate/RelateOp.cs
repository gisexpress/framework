using System.Collections.Generic;
using System.Diagnostics;
using System.Geometries.Graph;
using System.Geometries.Graph.Index;

namespace System.Geometries.Operation.Relate
{
    internal class RelateOp : GeometryGraphOperation
    {
        NodeMap Nodes;
        List<Edge> IsolatedEdges;
        IntersectionMatrix Matrix;

        public static bool Relate(out IntersectionMatrix im, params IGeometry[] args)
        {
            var r = new RelateOp();

            if (r.Compute(false, args))
            {
                im = r.Matrix;
                return im != null;
            }

            im = default(IntersectionMatrix);
            return false;
        }

        protected override void OnInit(IGeometry[] args)
        {
            IsolatedEdges = new List<Edge>();
            Nodes = new NodeMap(new RelateNodeFactory());

            base.OnInit(args);
        }

        protected override bool OnCompute(params IGeometry[] args)
        {
            var im = new IntersectionMatrix();

            // Since Geometries are finite and embedded in a 2-D space, the EE element must always be 2
            im.Set(Locations.Exterior, Locations.Exterior, Dimensions.Surface);

            // if the Geometries don't overlap there is nothing to do
            if (Graphs[0].Geometry.GetBounds().Intersects(Graphs[1].Geometry.GetBounds()) == false)
            {
                ComputeDisjointIM(im);
                return true;
            }

            Graphs[0].ComputeSelfNodes(Intersector, false);
            Graphs[1].ComputeSelfNodes(Intersector, false);

            // Compute intersections between edges of the two input geometries
            SegmentIntersector intersector = Graphs[0].ComputeEdgeIntersections(Graphs[1], Intersector, false);
            ComputeIntersectionNodes(0);
            ComputeIntersectionNodes(1);

            // Copy the labelling for the nodes in the parent Geometries.  
            // These override any labels determined by intersections between the geometries.
            CopyNodesAndLabels(0);
            CopyNodesAndLabels(1);

            // Complete the labelling for any nodes which only have a label for a single point
            LabelIsolatedNodes();

            // If a proper intersection was found, we can set a lower bound on the IM.
            ComputeProperIntersectionIM(intersector, im);

            // Now process improper intersections (eg where one or other of the geometries has a vertex at the intersection point)
            // We need to compute the edge graph at all nodes to determine the IM.
            // Build EdgeEnds for all intersections
            var builder = new EdgeEndBuilder();
            IList<EdgeEnd> ee0 = builder.ComputeEdgeEnds(Graphs[0].Edges);
            InsertEdgeEnds(ee0);
            IList<EdgeEnd> ee1 = builder.ComputeEdgeEnds(Graphs[1].Edges);
            InsertEdgeEnds(ee1);

            if (LabelNodeEdges())
            {
                LabelIsolatedEdges(0, 1);
                LabelIsolatedEdges(1, 0);

                // Update the IM from all components
                UpdateIM(Matrix = im);
                return true;
            }

            return false;
        }

        void InsertEdgeEnds(IEnumerable<EdgeEnd> ee)
        {
            foreach (EdgeEnd e in ee)
            {
                Nodes.Add(e);
            }
        }

        void ComputeProperIntersectionIM(SegmentIntersector intersector, IntersectionMatrix im)
        {
            // If a proper intersection is found, we can set a lower bound on the IM.
            Dimensions dimA = Graphs[0].Geometry.GetDimension();
            Dimensions dimB = Graphs[1].Geometry.GetDimension();

            bool hasProper = intersector.HasProperIntersection;
            bool hasProperInterior = intersector.HasProperInteriorIntersection;

            // For Geometry's of dim 0 there can never be proper intersections.
            /*
             * If edge segments of Areas properly intersect, the areas must properly overlap.
             */
            if (dimA == Dimensions.Surface && dimB == Dimensions.Surface)
            {
                if (hasProper)
                {
                    im.SetAtLeast("212101212");
                }
            }

            /*
             * If an Line segment properly intersects an edge segment of an Area,
             * it follows that the Interior of the Line intersects the Boundary of the Area.
             * If the intersection is a proper <i>interior</i> intersection, then
             * there is an Interior-Interior intersection too.
             * Note that it does not follow that the Interior of the Line intersects the Exterior
             * of the Area, since there may be another Area component which contains the rest of the Line.
             */
            else if (dimA == Dimensions.Surface && dimB == Dimensions.Curve)
            {
                if (hasProper)
                    im.SetAtLeast("FFF0FFFF2");
                if (hasProperInterior)
                    im.SetAtLeast("1FFFFF1FF");
            }

            else if (dimA == Dimensions.Curve && dimB == Dimensions.Surface)
            {
                if (hasProper)
                    im.SetAtLeast("F0FFFFFF2");
                if (hasProperInterior)
                    im.SetAtLeast("1F1FFFFFF");
            }

            /* If edges of LineStrings properly intersect *in an interior point*, all
               we can deduce is that
               the interiors intersect.  (We can NOT deduce that the exteriors intersect,
               since some other segments in the geometries might cover the points in the
               neighbourhood of the intersection.)
               It is important that the point be known to be an interior point of
               both Geometries, since it is possible in a self-intersecting point to
               have a proper intersection on one segment that is also a boundary point of another segment.
            */
            else if (dimA == Dimensions.Curve && dimB == Dimensions.Curve)
            {
                if (hasProperInterior)
                    im.SetAtLeast("0FFFFFFFF");
            }
        }

        /// <summary>
        /// Copy all nodes from an arg point into this graph.
        /// The node label in the arg point overrides any previously computed label for that argIndex.
        /// (E.g. a node may be an intersection node with a computed label of Boundary,
        /// but in the original arg Geometry it is actually
        /// in the interior due to the Boundary Determination Rule)
        /// </summary>
        void CopyNodesAndLabels(int argIndex)
        {
            foreach (Node graphNode in Graphs[argIndex].Nodes)
            {
                Node newNode = Nodes.AddNode(graphNode.Coordinate);
                newNode.SetLabel(argIndex, graphNode.Label.GetLocation(argIndex));
            }
        }

        /// <summary>
        /// Insert nodes for all intersections on the edges of a Geometry.
        /// Label the created nodes the same as the edge label if they do not already have a label.
        /// This allows nodes created by either self-intersections or
        /// mutual intersections to be labelled.
        /// Endpoint nodes will already be labelled from when they were inserted.
        /// </summary>
        /// <param name="argIndex"></param>
        void ComputeIntersectionNodes(int argIndex)
        {
            foreach (Edge e in Graphs[argIndex].Edges)
            {
                Locations eLoc = e.Label.GetLocation(argIndex);
                foreach (EdgeIntersection ei in e.EdgeIntersectionList)
                {
                    RelateNode n = (RelateNode)Nodes.AddNode(ei.Coordinate);
                    if (eLoc == Locations.Boundary)
                        n.SetLabelBoundary(argIndex);
                    else
                    {
                        if (n.Label.IsNull(argIndex))
                            n.SetLabel(argIndex, Locations.Interior);
                    }
                }
            }
        }

        /// <summary>
        /// For all intersections on the edges of a Geometry,
        /// label the corresponding node IF it doesn't already have a label.
        /// This allows nodes created by either self-intersections or
        /// mutual intersections to be labelled.
        /// Endpoint nodes will already be labelled from when they were inserted.
        /// </summary>
        /// <param name="argIndex"></param>
        void LabelIntersectionNodes(int argIndex)
        {
            foreach (Edge e in Graphs[argIndex].Edges)
            {
                Locations eLoc = e.Label.GetLocation(argIndex);
                foreach (EdgeIntersection ei in e.EdgeIntersectionList)
                {
                    RelateNode n = (RelateNode)Nodes.Find(ei.Coordinate);
                    if (n.Label.IsNull(argIndex))
                    {
                        if (eLoc == Locations.Boundary)
                            n.SetLabelBoundary(argIndex);
                        else n.SetLabel(argIndex, Locations.Interior);
                    }
                }
            }
        }

        /// <summary>
        /// If the Geometries are disjoint, we need to enter their dimension and
        /// boundary dimension in the Ext rows in the IM
        /// </summary>
        /// <param name="im"></param>
        void ComputeDisjointIM(IntersectionMatrix im)
        {
            IGeometry ga = Graphs[0].Geometry;

            im.Set(Locations.Interior, Locations.Exterior, ga.GetDimension());
            im.Set(Locations.Boundary, Locations.Exterior, ga.GetBoundaryDimension());

            IGeometry gb = Graphs[1].Geometry;

            im.Set(Locations.Exterior, Locations.Interior, gb.GetDimension());
            im.Set(Locations.Exterior, Locations.Boundary, gb.GetBoundaryDimension());
        }

        bool LabelNodeEdges()
        {
            foreach (RelateNode node in Nodes)
            {
                if (node.Edges.ComputeLabelling(Graphs))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the IM with the sum of the IMs for each component.
        /// </summary>
        void UpdateIM(IntersectionMatrix im)
        {
            foreach (Edge e in IsolatedEdges)
            {
                e.UpdateIM(im);
            }

            foreach (RelateNode node in Nodes)
            {
                node.UpdateIM(im);
                node.UpdateIMFromEdges(im);
            }
        }

        /// <summary> 
        /// Processes isolated edges by computing their labelling and adding them
        /// to the isolated edges list.
        /// Isolated edges are guaranteed not to touch the boundary of the target (since if they
        /// did, they would have caused an intersection to be computed and hence would
        /// not be isolated).
        /// </summary>
        /// <param name="thisIndex"></param>
        /// <param name="targetIndex"></param>
        void LabelIsolatedEdges(int thisIndex, int targetIndex)
        {
            foreach (var e in Graphs[thisIndex].Edges)
            {
                if (e.IsIsolated)
                {
                    LabelIsolatedEdge(e, targetIndex, Graphs[targetIndex].Geometry);
                    IsolatedEdges.Add(e);
                }
            }
        }

        /// <summary>
        /// Label an isolated edge of a graph with its relationship to the target point.
        /// If the target has dim 2 or 1, the edge can either be in the interior or the exterior.
        /// If the target has dim 0, the edge must be in the exterior.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="targetIndex"></param>
        /// <param name="target"></param>
        void LabelIsolatedEdge(Edge e, int targetIndex, IGeometry target)
        {
            // this won't work for GeometryCollections with both dim 2 and 1 geoms
            if (target.GetDimension() > 0)
            {
                // since edge is not in boundary, may not need the full generality of PointLocator?
                // Possibly should use ptInArea locator instead?  We probably know here
                // that the edge does not touch the bdy of the target Geometry
                Locations loc = target.Locate(e.Coordinate);
                e.Label.SetAllLocations(targetIndex, loc);
            }
            else e.Label.SetAllLocations(targetIndex, Locations.Exterior);
        }

        /// <summary>
        /// Isolated nodes are nodes whose labels are incomplete
        /// (e.g. the location for one Geometry is null).
        /// This is the case because nodes in one graph which don't intersect
        /// nodes in the other are not completely labelled by the initial process
        /// of adding nodes to the nodeList.
        /// To complete the labelling we need to check for nodes that lie in the
        /// interior of edges, and in the interior of areas.
        /// </summary>
        void LabelIsolatedNodes()
        {
            foreach (Node n in Nodes)
            {
                Label label = n.Label;
                // isolated nodes should always have at least one point in their label
                Debug.Assert(label.GeometryCount > 0, "node with empty label found");
                if (n.IsIsolated)
                {
                    if (label.IsNull(0))
                        LabelIsolatedNode(n, 0);
                    else LabelIsolatedNode(n, 1);
                }
            }
        }

        /// <summary>
        /// Label an isolated node with its relationship to the target point.
        /// </summary>
        void LabelIsolatedNode(Node n, int targetIndex)
        {
            Locations loc = Graphs[targetIndex].Geometry.Locate(n.Coordinate);
            n.Label.SetAllLocations(targetIndex, loc);
        }
    }

    public enum RelateOperations
    {
        Equals,
        Disjoint,
        Touches,
        Contains,
        Covers,
        Intersects,
        Within,
        CoveredBy,
        Crosses,
        Overlaps
    }
}