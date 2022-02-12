using System.Collections.Generic;
using System.Geometries.Algorithm;
using System.IO;

namespace System.Geometries.Graph
{
    internal class PlanarGraph
    {
        public PlanarGraph()
            : this(new NodeFactory())
        {
        }

        public PlanarGraph(NodeFactory nodeFact)
        {
            iEdges = new List<Edge>();
            iEdgeEndList = new List<EdgeEnd>();
            iNodes = new NodeMap(nodeFact);
        }

        readonly NodeMap iNodes;
        readonly List<Edge> iEdges;
        readonly IList<EdgeEnd> iEdgeEndList;

        /// <summary> 
        /// For nodes in the Collection, link the DirectedEdges at the node that are in the result.
        /// This allows clients to link only a subset of nodes in the graph, for
        /// efficiency (because they know that only a subset is of interest).
        /// </summary>
        /// <param name="nodes"></param>
        public static bool LinkResultDirectedEdges(IList<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if (((DirectedEdgeStar)node.Edges).LinkResultDirectedEdges())
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public IEnumerator<Edge> GetEdgeEnumerator()
        {
            return iEdges.GetEnumerator();
        }

        public IList<EdgeEnd> EdgeEnds
        {
            get { return iEdgeEndList; }
        }

        protected internal IList<Edge> Edges
        {
            get { return iEdges; }
        }

        public bool IsBoundaryNode(int geomIndex, Coordinate coord)
        {
            Node node = iNodes.Find(coord);

            if (node == null)
            {
                return false;
            }

            Label label = node.Label;

            if (label == null)
            {
                return false;
            }

            if (label.GetLocation(geomIndex) == Locations.Boundary)
            {
                return true;
            }

            return false;
        }

        protected void InsertEdge(Edge e)
        {
            iEdges.Add(e);
        }

        public void Add(EdgeEnd e)
        {
            iNodes.Add(e);
            iEdgeEndList.Add(e);
        }

        public IEnumerator<Node> GetNodeEnumerator()
        {
            return iNodes.GetEnumerator();
        }

        public IList<Node> Nodes
        {
            get { return iNodes.Values; }
        }

        protected NodeMap NodeMap
        {
            get { return iNodes; }
        }

        public Node AddNode(Node node)
        {
            return iNodes.AddNode(node);
        }

        public Node AddNode(ICoordinate coord)
        {
            return iNodes.AddNode(coord);
        }

        /// <returns> 
        /// The node if found; null otherwise
        /// </returns>
        /// <param name="coord"></param>
        public Node Find(Coordinate coord)
        {
            return iNodes.Find(coord);
        }

        /// <summary> 
        /// Add a set of edges to the graph.  For each edge two DirectedEdges
        /// will be created.  DirectedEdges are NOT linked by this method.
        /// </summary>
        /// <param name="edgesToAdd"></param>
        public void AddEdges(IList<Edge> edgesToAdd)
        {
            // create all the nodes for the edges
            foreach (Edge e in edgesToAdd)
            {
                iEdges.Add(e);

                var de1 = new DirectedEdge(e, true);

                if (de1.IsValid)
                {
                    var de2 = new DirectedEdge(e, false);

                    if (de2.IsValid)
                    {
                        de1.Directed = de2;
                        de2.Directed = de1;

                        Add(de1);
                        Add(de2);
                    }
                }
            }
        }

        /// <summary> 
        /// Link the DirectedEdges at the nodes of the graph.
        /// This allows clients to link only a subset of nodes in the graph, for
        /// efficiency (because they know that only a subset is of interest).
        /// </summary>
        public bool LinkResultDirectedEdges()
        {
            foreach (Node node in Nodes)
            {
                if (((DirectedEdgeStar)node.Edges).LinkResultDirectedEdges())
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary> 
        /// Link the DirectedEdges at the nodes of the graph.
        /// This allows clients to link only a subset of nodes in the graph, for
        /// efficiency (because they know that only a subset is of interest).
        /// </summary>
        public void LinkAllDirectedEdges()
        {
            foreach (Node node in Nodes)
            {
                ((DirectedEdgeStar)node.Edges).LinkAllDirectedEdges();
            }
        }

        /// <summary> 
        /// Returns the EdgeEnd which has edge e as its base edge
        /// (MD 18 Feb 2002 - this should return a pair of edges).
        /// </summary>
        /// <param name="e"></param>
        /// <returns> The edge, if found <c>null</c> if the edge was not found.</returns>
        public EdgeEnd FindEdgeEnd(Edge e)
        {
            foreach (EdgeEnd ee in iEdgeEndList)
            {
                if (ee.Edge == e)
                {
                    return ee;
                }
            }

            return default(EdgeEnd);
        }

        /// <summary>
        /// Returns the edge whose first two coordinates are p0 and p1.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns> The edge, if found <c>null</c> if the edge was not found.</returns>
        public Edge FindEdge(Coordinate p0, Coordinate p1)
        {
            for (int i = 0; i < iEdges.Count; i++)
            {
                Edge e = iEdges[i];

                if (p0.IsEquivalent(e.Sequence.Get(0)) && p1.IsEquivalent(e.Sequence.Get(1)))
                {
                    return e;
                }
            }

            return default(Edge);
        }

        /// <summary>
        /// Returns the edge which starts at p0 and whose first segment is
        /// parallel to p1.
        /// </summary>
        /// <param name="p0"></param>
        ///<param name="p1"></param>
        /// <returns> The edge, if found <c>null</c> if the edge was not found.</returns>
        public Edge FindEdgeInSameDirection(Coordinate p0, Coordinate p1)
        {
            for (int i = 0; i < iEdges.Count; i++)
            {
                Edge e = iEdges[i];

                if (MatchInSameDirection(p0, p1, e.Sequence.Get(0), e.Sequence.Get(1)))
                {
                    return e;
                }

                if (MatchInSameDirection(p0, p1, e.Sequence.Get(e.NumPoints - 1), e.Sequence.Get(e.NumPoints - 2)))
                {
                    return e;
                }
            }

            return default(Edge);
        }

        /// <summary>
        /// The coordinate pairs match if they define line segments lying in the same direction.
        /// E.g. the segments are parallel and in the same quadrant
        /// (as opposed to parallel and opposite!).
        /// </summary>
        static bool MatchInSameDirection(ICoordinate p0, ICoordinate p1, ICoordinate ep0, ICoordinate ep1)
        {
            if (p0.IsEquivalent(ep0))
            {
                if (CGAlgorithms.ComputeOrientation(p0, p1, ep1) == CGAlgorithms.Collinear)
                {
                    int d0, d1;

                    double dx = p1.X - p0.X;
                    double dy = p1.Y - p0.Y;

                    if (QuadrantOp.TryGetQuadrant(dx, dy, out d0))
                    {
                        dx = ep1.X - ep0.X;
                        dy = ep1.Y - ep0.Y;

                        if (QuadrantOp.TryGetQuadrant(dx, dy, out d1))
                        {
                            return d0 == d1;
                        }
                    }
                }
            }

            return false;
        }
    }
}
