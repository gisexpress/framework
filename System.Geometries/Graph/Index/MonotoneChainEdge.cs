namespace System.Geometries.Graph.Index
{
    /// <summary> 
    /// MonotoneChains are a way of partitioning the segments of an edge to
    /// allow for fast searching of intersections.
    /// They have the following properties:
    /// the segments within a monotone chain will never intersect each other, and 
    /// the envelope of any contiguous subset of the segments in a monotone chain
    /// is simply the envelope of the endpoints of the subset.
    /// Property 1 means that there is no need to test pairs of segments from within
    /// the same monotone chain for intersection.
    /// Property 2 allows
    /// binary search to be used to find the intersection points of two monotone chains.
    /// For many types of real-world data, these properties eliminate a large number of
    /// segment comparisons, producing substantial speed gains.
    /// </summary>
    internal class MonotoneChainEdge
    {
        readonly Edge Edge;
        readonly int[] Start;
        readonly IEnvelope Bounds0;
        readonly IEnvelope Bounds1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public MonotoneChainEdge(Edge e)
        {
            Edge = e;
            Sequence = e.Sequence;
            Bounds0 = Sequence.Factory.Create<IEnvelope>();
            Bounds1 = Sequence.Factory.Create<IEnvelope>();
            Start = MonotoneChainIndexer.GetChainStartIndices(Sequence);
        }

        public readonly ICoordinateCollection Sequence;

        /// <summary>
        /// 
        /// </summary>
        public int[] StartIndexes
        {
            get { return Start; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainIndex"></param>
        /// <returns></returns>
        public double GetMinX(int chainIndex)
        {
            double x1 = Sequence.Get(Start[chainIndex]).X;
            double x2 = Sequence.Get(Start[chainIndex + 1]).X;
            return x1 < x2 ? x1 : x2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainIndex"></param>
        /// <returns></returns>
        public double GetMaxX(int chainIndex)
        {
            double x1 = Sequence.Get(Start[chainIndex]).X;
            double x2 = Sequence.Get(Start[chainIndex + 1]).X;
            return x1 > x2 ? x1 : x2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mce"></param>
        /// <param name="si"></param>
        public void ComputeIntersects(MonotoneChainEdge mce, SegmentIntersector si)
        {
            for (int i = 0; i < Start.Length - 1; i++)
                for (int j = 0; j < mce.Start.Length - 1; j++)
                    ComputeIntersectsForChain(i, mce, j, si);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainIndex0"></param>
        /// <param name="mce"></param>
        /// <param name="chainIndex1"></param>
        /// <param name="si"></param>
        public void ComputeIntersectsForChain(int chainIndex0, MonotoneChainEdge mce, int chainIndex1, SegmentIntersector si)
        {
            ComputeIntersectsForChain(Start[chainIndex0], Start[chainIndex0 + 1], mce, mce.Start[chainIndex1], mce.Start[chainIndex1 + 1], si);
        }

        void ComputeIntersectsForChain(int start0, int end0, MonotoneChainEdge mce, int start1, int end1, SegmentIntersector ei)
        {
            ICoordinate p00 = Sequence.Get(start0);
            ICoordinate p01 = Sequence.Get(end0);
            ICoordinate p10 = mce.Sequence.Get(start1);
            ICoordinate p11 = mce.Sequence.Get(end1);

            // terminating condition for the recursion
            if (end0 - start0 == 1 && end1 - start1 == 1)
            {
                ei.AddIntersections(Edge, start0, mce.Edge, start1);
                return;
            }

            // nothing to do if the envelopes of these chains don't overlap
            Bounds0.Init(p00, p01);
            Bounds1.Init(p10, p11);

            if (!Bounds0.Intersects(Bounds1))
                return;

            // the chains overlap, so split each in half and iterate  (binary search)
            int mid0 = (start0 + end0) / 2;
            int mid1 = (start1 + end1) / 2;

            // check terminating conditions before recursing
            if (start0 < mid0)
            {
                if (start1 < mid1)
                    ComputeIntersectsForChain(start0, mid0, mce, start1, mid1, ei);
                if (mid1 < end1)
                    ComputeIntersectsForChain(start0, mid0, mce, mid1, end1, ei);
            }
            if (mid0 < end0)
            {
                if (start1 < mid1)
                    ComputeIntersectsForChain(mid0, end0, mce, start1, mid1, ei);
                if (mid1 < end1)
                    ComputeIntersectsForChain(mid0, end0, mce, mid1, end1, ei);
            }
        }
    }
}
