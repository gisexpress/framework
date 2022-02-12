using System.Diagnostics;

namespace System.Geometries.Index.Quadtree
{
    /// <summary>
    /// Represents a node of a <c>Quadtree</c>.  
    /// Nodes contain items which have a spatial extent corresponding to the node's position in the quadtree.
    /// </summary>
    internal class Node<T> : NodeBase<T>
    {
        public static Node<T> CreateNode(IEnvelope e)
        {
            Key key = new Key(e);
            var node = new Node<T>(key.Bounds, key.Level);
            return node;
        }

        public static Node<T> CreateExpanded(Node<T> node, IEnvelope e)
        {
            IEnvelope expand = e.Clone();
            
            if (node != null) 
                expand.ExpandToInclude(node.Bounds);

            Node<T> largerNode = CreateNode(expand);

            if (node != null)
                largerNode.InsertNode(node);
            
            return largerNode;
        }

        public Node(IEnvelope e, int level)
        {
            Bounds = e;
            Level = level;
            CentreX = (e.Min.X + e.Max.X) / 2;
            CentreY = (e.Min.Y + e.Max.Y) / 2;
        }

        public readonly IEnvelope Bounds;
        readonly int Level;
        readonly double CentreX, CentreY;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchEnv"></param>
        /// <returns></returns>
        protected override bool IsSearchMatch(IEnvelope searchEnv)
        {
            return Bounds.Intersects(searchEnv);
        }

        /// <summary> 
        /// Returns the subquad containing the envelope <paramref name="searchEnv"/>.
        /// Creates the subquad if
        /// it does not already exist.
        /// </summary>
        /// <param name="searchEnv">The envelope to search for</param>
        /// <returns>The subquad containing the search envelope.</returns>
        public Node<T> GetNode(IEnvelope searchEnv)
        {
            int subnodeIndex = GetSubnodeIndex(searchEnv, CentreX, CentreY);            
            // if subquadIndex is -1 searchEnv is not contained in a subquad
            if (subnodeIndex != -1) 
            {
                // create the quad if it does not exist
                var node = GetSubnode(subnodeIndex);
                // recursively search the found/created quad
                return node.GetNode(searchEnv);
            }
            return this;
        }

        /// <summary>
        /// Returns the smallest <i>existing</i>
        /// node containing the envelope.
        /// </summary>
        /// <param name="searchEnv"></param>
        public NodeBase<T> Find(IEnvelope searchEnv)
        {
            int subnodeIndex = GetSubnodeIndex(searchEnv, CentreX, CentreY);
            if (subnodeIndex == -1)
                return this;
            if (Subnode[subnodeIndex] != null) 
            {
                // query lies in subquad, so search it
                var node = Subnode[subnodeIndex];
                return node.Find(searchEnv);
            }
            // no existing subquad, so return this one anyway
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void InsertNode(Node<T> node)
        {
            Debug.Assert(Bounds == null || Bounds.Contains(node.Bounds));
            int index = GetSubnodeIndex(node.Bounds, CentreX, CentreY);        
            if (node.Level == Level - 1)             
                Subnode[index] = node;                    
            else 
            {
                // the quad is not a direct child, so make a new child quad to contain it
                // and recursively insert the quad
                var childNode = CreateSubnode(index);
                childNode.InsertNode(node);
                Subnode[index] = childNode;
            }
        }

        /// <summary>
        /// Get the subquad for the index.
        /// If it doesn't exist, create it.
        /// </summary>
        /// <param name="index"></param>
        Node<T> GetSubnode(int index)
        {
            if (Subnode[index] == null) 
                Subnode[index] = CreateSubnode(index);            
            return Subnode[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Node<T> CreateSubnode(int index)
        {
            // create a new subquad in the appropriate quadrant
            double minx = 0.0;
            double maxx = 0.0;
            double miny = 0.0;
            double maxy = 0.0;

            switch (index) 
            {
                case 0:
                    minx = Bounds.Min.X;
                    maxx = CentreX;
                    miny = Bounds.Min.Y;
                    maxy = CentreY;
                    break;

                case 1:
                    minx = CentreX;
                    maxx = Bounds.Max.X;
                    miny = Bounds.Min.Y;
                    maxy = CentreY;
                    break;

                case 2:
                    minx = Bounds.Min.X;
                    maxx = CentreX;
                    miny = CentreY;
                    maxy = Bounds.Max.Y;
                    break;

                case 3:
                    minx = CentreX;
                    maxx = Bounds.Max.X;
                    miny = CentreY;
                    maxy = Bounds.Max.Y;
                    break;

	            default:
		            break;
            }
            
            return new Node<T>(new Envelope(minx, maxx, miny, maxy), Level - 1);
        }
    }
}
