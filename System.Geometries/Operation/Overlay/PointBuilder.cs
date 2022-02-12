using System.Collections.Generic;
using System.Geometries.Graph;

namespace System.Geometries.Operation.Overlay
{
    /// <summary>
    /// Constructs <c>Point</c>s from the nodes of an overlay graph.
    /// </summary>
    internal class PointBuilder
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="operation">The operation</param>
        /// <param name="geometryFactory">The geometry factory</param>
        public PointBuilder(OverlayOperation operation)
        {
            Operation = operation;
            Points = new List<IGeometry>();
        }

        readonly OverlayOperation Operation;
        readonly List<IGeometry> Points;

        /// <summary>
        /// Computes the Point geometries which will appear in the result, given the specified overlay operation.
        /// </summary>
        public IList<IGeometry> Build(SpatialFunctions operation)
        {
            ExtractNonCoveredResultNodes(operation);
            return Points;
        }

        /// <summary>
        /// Determines nodes which are in the result, and creates <see cref="IPoint"/>s for them.
        /// </summary>
        /// <remarks>
        /// This method determines nodes which are candidates for the result via their labelling and their graph topology.
        /// </remarks>
        void ExtractNonCoveredResultNodes(SpatialFunctions operation)
        {
            foreach (Node n in Operation.Graph.Nodes)
            {
                // Filter out nodes which are known to be in the result
                // If an incident edge is in the result, then the node coordinate is included already
                if (n.IsInResult || n.IsIncidentEdgeInResult())
                {
                    continue;
                }

                if (n.Edges.Degree == 0 || operation == SpatialFunctions.Intersection)
                {
                    // For nodes on edges, only INTERSECTION can result in edge nodes being included even if none of their incident edges are included
                    if (OverlayOperation.IsResultOfOp(n.Label, operation))
                    {
                        FilterCoveredNodeToPoint(n);
                    }
                }
            }
        }

        /// <summary>
        /// Converts non-covered nodes to Point objects and adds them to the result.
        /// </summary>
        /// <remarks>
        /// A node is covered if it is contained in another element Geometry
        /// with higher dimension (e.g. a node point might be contained in a polygon,
        /// in which case the point can be eliminated from the result).
        /// </remarks>
        /// <param name="n">The node to test</param>
        void FilterCoveredNodeToPoint(Node n)
        {
            ICoordinate c = n.Coordinate;

            if (Operation.IsCoveredByLA(c))
            {
                return;
            }

            Points.Add(Operation.GetArgGeometry(0).Factory.Create<IPoint>(c.X, c.Y));
        }
    }
}
