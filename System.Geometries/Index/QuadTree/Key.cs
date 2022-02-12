namespace System.Geometries.Index.Quadtree
{
    /// <summary> 
    /// A Key is a unique identifier for a node in a quadtree.
    /// It contains a lower-left point and a level number. 
    /// The level number is the power of two for the size of the node envelope.
    /// </summary>
    internal class Key
    {
        public static int ComputeQuadLevel(IEnvelope env)
        {
            double dx = env.GetWidth();
            double dy = env.GetHeight();
            double dMax = dx > dy ? dx : dy;
            return DoubleBits.GetExponent(dMax) + 1;
        }

        public Key(IEnvelope e)
        {
            ComputeKey(e);
        }

        public int Level;
        public IEnvelope Bounds;

        public Coordinate Centre
        {
            get { return new Coordinate((Bounds.Min.X + Bounds.Max.X) / 2, (Bounds.Min.Y + Bounds.Max.Y) / 2); }
        }

        /// <summary>
        /// Return a square envelope containing the argument envelope,
        /// whose extent is a power of two and which is based at a power of 2.
        /// </summary>
        public void ComputeKey(IEnvelope e)
        {
            Level = ComputeQuadLevel(e);
            Bounds = new Envelope();
            ComputeKey(Level, e);
            // MD - would be nice to have a non-iterative form of this algorithm
            while (!Bounds.Contains(e))
            {
                Level += 1;
                ComputeKey(Level, e);
            }
        }

        void ComputeKey(int level, IEnvelope itemEnv)
        {
            double quadSize = DoubleBits.PowerOf2(level);
            double x = Math.Floor(itemEnv.Min.X / quadSize) * quadSize;
            double y = Math.Floor(itemEnv.Min.Y / quadSize) * quadSize;
            
            Bounds.Init(x, x + quadSize, y, y + quadSize);
        }
    }
}
