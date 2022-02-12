using System.Collections;
using System.Collections.Generic;

namespace System.Geometries.Graph
{
    internal class NodeMapDictionary : IComparer<ICoordinate>, IEnumerable<Node>
    {
        public NodeMapDictionary(NodeFactory factory)
        {
            Factory = factory;
            Nodes = new SortedDictionary<ICoordinate, Node>(this);
        }

        public readonly NodeFactory Factory;

        protected readonly SortedDictionary<ICoordinate, Node> Nodes;

        public Node Add(ICoordinate key)
        {
            Node value;
            Add(key, value = Factory.CreateNode(key));
            return value;
        }

        public void Add(ICoordinate key, Node value)
        {
            Nodes.Add(key, value);
        }

        public bool TryGetValue(ICoordinate key, out Node value)
        {
            return Nodes.TryGetValue(key, out value);
        }

        public int Compare(ICoordinate x, ICoordinate y)
        {
            return x.CompareTo(y);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return Nodes.Values.GetEnumerator();
        }
    }
}
