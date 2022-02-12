using System.Collections.Generic;
using System.Geometries.Algorithm;
using System.Geometries.Graph;
using System.Geometries.Noding;
using System.Geometries.Operation.Overlay;
using System.Linq;

namespace System.Geometries.Operation.Buffer
{
    internal class BufferBuilder
    {
        ///<summary>
        ///Compute the change in depth as an edge is crossed from R to L
        ///</summary>
        static int DepthDelta(Label label)
        {
            Locations left = label.GetLocation(0, Positions.Left);
            Locations right = label.GetLocation(0, Positions.Right);

            if (left == Locations.Interior && right == Locations.Exterior)
            {
                return 1;
            }

            if (left == Locations.Exterior && right == Locations.Interior)
            {
                return -1;
            }

            return 0;
        }

        protected PlanarGraph Graph;
        protected INoder WorkingNoder;
        protected readonly IBufferParameters BufferOptions;
        protected readonly EdgeList EdgeList = new EdgeList();

        public BufferBuilder(IBufferParameters bufParams)
        {
            BufferOptions = bufParams;
        }

        ///<summary>
        /// Sets the <see cref="INoder"/> to use during noding.
        /// This allows choosing fast but non-robust noding, or slower
        /// but robust noding.
        ///</summary>
        public INoder Noder
        {
            get { return WorkingNoder; }
            set { WorkingNoder = value; }
        }

        public IGeometry Buffer(Geometry g, double distance)
        {
            var curveBuilder = new OffsetCurveBuilder(BufferOptions);
            var curveSetBuilder = new OffsetCurveSetBuilder(g, distance, curveBuilder);
            IList<ISegmentString> bufferSegStrList = curveSetBuilder.GetCurves();

            // short-circuit test
            if (bufferSegStrList.Count <= 0)
            {
                return default(Geometry);
            }

            ComputeNodedEdges(bufferSegStrList);

            Graph = new PlanarGraph(new OverlayNodeFactory());
            Graph.AddEdges(EdgeList.Edges);

            var polyBuilder = new PolygonBuilder();

            if (BuildSubgraphs(CreateSubgraphs(Graph), polyBuilder))
            {
                IList<IGeometry> resultPolyList = polyBuilder.Polygons;

                // just in case...
                if (resultPolyList.Count <= 0)
                {
                    return default(Geometry);
                }

                return g.Factory.BuildGeometry(resultPolyList);
            }

            return default(Geometry);
        }

        INoder GetNoder()
        {
            if (WorkingNoder == null)
            {
                // otherwise use a fast (but non-robust) noder
                return new MCIndexNoder(new IntersectionAdder(new RobustLineIntersector()));
            }

            return WorkingNoder;
        }

        void ComputeNodedEdges(IList<ISegmentString> bufferSegStrList)
        {
            var noder = GetNoder();

            noder.ComputeNodes(bufferSegStrList);

            var nodedSegStrings = noder.GetNodedSubstrings();

            foreach (ISegmentString item in nodedSegStrings)
            {
                // Discard edges which have zero length, since they carry no information and cause problems with topology building
                if (item.Sequence.Count == 2 && item.Sequence.Get(0).IsEquivalent(item.Sequence.Get(1)))
                {
                    continue;
                }

                var oldLabel = (Label)item.Context;
                var edge = new Edge(item.Sequence, new Label(oldLabel));

                InsertUniqueEdge(edge);
            }
        }


        /// <summary>
        /// Inserted edges are checked to see if an identical edge already exists.
        /// If so, the edge is not inserted, but its label is merged
        /// with the existing edge.
        /// </summary>
        protected void InsertUniqueEdge(Edge e)
        {
            //<FIX> MD 8 Oct 03  speed up identical edge lookup
            // fast lookup
            Edge existingEdge = EdgeList.FindEqualEdge(e);

            // If an identical edge already exists, simply update its label
            if (existingEdge != null)
            {
                Label existingLabel = existingEdge.Label;

                Label labelToMerge = e.Label;
                // check if new edge is in reverse direction to existing edge
                // if so, must flip the label before merging it
                if (!existingEdge.IsPointwiseEqual(e))
                {
                    labelToMerge = new Label(e.Label);
                    labelToMerge.Flip();
                }
                existingLabel.Merge(labelToMerge);

                // compute new depth delta of sum of edges
                int mergeDelta = DepthDelta(labelToMerge);
                int existingDelta = existingEdge.DepthDelta;
                int newDelta = existingDelta + mergeDelta;
                existingEdge.DepthDelta = newDelta;
            }
            else
            {   // no matching existing edge was found
                // add this new edge to the list of edges in this graph
                //e.setName(name + edges.size());
                EdgeList.Add(e);
                e.DepthDelta = DepthDelta(e.Label);
            }
        }

        static IEnumerable<BufferSubgraph> CreateSubgraphs(PlanarGraph graph)
        {
            var subgraphList = new List<BufferSubgraph>();

            foreach (Node node in graph.Nodes)
            {
                if (!node.IsVisited)
                {
                    var subgraph = new BufferSubgraph();
                    subgraph.Create(node);
                    subgraphList.Add(subgraph);
                }
            }

            subgraphList.Sort();
            subgraphList.Reverse();

            return subgraphList;
        }

        /// <summary>
        /// Completes the building of the input subgraphs by depth-labelling them,
        /// and adds them to the PolygonBuilder.
        /// </summary>
        /// <remarks>
        /// The subgraph list must be sorted in rightmost-coordinate order.
        /// </remarks>
        /// <param name="subgraphList"> the subgraphs to build</param>
        /// <param name="polyBuilder"> the PolygonBuilder which will build the final polygons</param>
        static bool BuildSubgraphs(IEnumerable<BufferSubgraph> subgraphList, PolygonBuilder polyBuilder)
        {
            var processedGraphs = new List<BufferSubgraph>();

            foreach (BufferSubgraph subgraph in subgraphList)
            {
                ICoordinate p = subgraph.RightMostCoordinate;
                var locater = new SubgraphDepthLocater(processedGraphs);
                int outsideDepth = locater.GetDepth(p);
                subgraph.ComputeDepth(outsideDepth);
                subgraph.FindResultEdges();
                processedGraphs.Add(subgraph);

                if (polyBuilder.Add(subgraph.DirectedEdges.Cast<EdgeEnd>().ToList(), subgraph.Nodes))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        //static IGeometry ConvertSegStrings(IEnumerator<ISegmentString> it)
        //{
        //    var lines = new List<IGeometry>();

        //    while (it.MoveNext())
        //    {
        //        ISegmentString ss = it.Current;
        //        ILineString line = Factory.CreateLineString(ss.Sequence.ToArray());
        //        lines.Add(line);
        //    }

        //    return Factory.BuildGeometry(lines);
        //}
    }
}
