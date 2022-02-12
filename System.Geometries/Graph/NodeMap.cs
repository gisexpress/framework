using System.Collections.Generic;
using System.IO;

namespace System.Geometries.Graph
{
    /// <summary> 
    /// A map of nodes, indexed by the coordinate of the node.
    /// </summary>
    internal class NodeMap
    {
        public NodeMap(NodeFactory factory)
        {
            Map = new NodeMapDictionary(factory);
        }

        protected readonly NodeMapDictionary Map;

        /// <summary> 
        /// This method expects that a node has a coordinate value.
        /// </summary>
        public Node AddNode(ICoordinate key)
        {
            Node node;

            if (Map.TryGetValue(key, out node))
            {
                return node;
            }

            return Map.Add(key);
        }

        public Node AddNode(Node value)
        {
            Node node = value;

            if (Map.TryGetValue(value.Coordinate, out node))
            {
                node.MergeLabel(value);
            }
            else
            {
                Map.Add(value.Coordinate, value);
            }

            return node;
        }

        /// <summary> 
        /// Adds a node for the start point of this EdgeEnd
        /// (if one does not already exist in this map).
        /// Adds the EdgeEnd to the (possibly new) node.
        /// </summary>
        /// <param name="e"></param>
        public void Add(EdgeEnd e)
        {
            ICoordinate p = e.Coordinate;
            Node n = AddNode(p);
            n.Add(e);
        }

        /// <returns> 
        /// The node if found; null otherwise.
        /// </returns>
        /// <param name="coord"></param>
        public Node Find(Coordinate coord)
        {
            Node res;
            if (!Map.TryGetValue(coord, out res))
                return null;
            return res;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return Map.GetEnumerator();
        }

        public IList<Node> Values
        {
            get { return new List<Node>(Map); }
        }

        public IList<Node> GetBoundaryNodes(int geomIndex)
        {
            IList<Node> bdyNodes = new List<Node>();
            foreach (Node node in Map)
            {
                if (node.Label.GetLocation(geomIndex) == Locations.Boundary)
                    bdyNodes.Add(node);
            }
            return bdyNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outstream"></param>
        public void Write(StreamWriter outstream)
        {
            foreach (Node node in Map)
            {
                node.Write(outstream);
            }
        }
    }
}
