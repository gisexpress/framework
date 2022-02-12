namespace System.Geometries.Index.Chain
{
    internal class MonotoneChain
    {
        public MonotoneChain(ICoordinateCollection sequence, int start, int end, object context)
        {
            Sequence = sequence;
            Start = start;
            End = end;
            iContext = context;
        }

        protected int ID;
        protected readonly int Start;
        protected readonly int End;
        protected readonly object iContext;
        protected IEnvelope Bounds;
        protected readonly ICoordinateCollection Sequence;

        public virtual void ComputeOverlaps(MonotoneChain mc, MonotoneChainOverlapAction mco)
        {
            ComputeOverlaps(Start, End, mc, mc.Start, mc.End, mco);
        }

        void ComputeOverlaps(int start0, int end0, MonotoneChain mc, int start1, int end1, MonotoneChainOverlapAction mco)
        {
            ICoordinate c1 = Sequence.Get(start0);
            ICoordinate c2 = Sequence.Get(end0);
            ICoordinate c3 = mc.Sequence.Get(start1);
            ICoordinate c4 = mc.Sequence.Get(end1);

            if (((end0 - start0) == 1) && ((end1 - start1) == 1))
            {
                mco.Overlap(this, start0, mc, start1);
            }
            else
            {
                var e0 = Sequence.Factory.Create<IEnvelope>(c1, c2);
                var e1 = Sequence.Factory.Create<IEnvelope>(c3, c4);

                if (e0.Intersects(e1))
                {
                    int num = (start0 + end0) / 2;
                    int num2 = (start1 + end1) / 2;

                    if (start0 < num)
                    {
                        if (start1 < num2) ComputeOverlaps(start0, num, mc, start1, num2, mco);
                        if (num2 < end1) ComputeOverlaps(start0, num, mc, num2, end1, mco);
                    }
                    if (num < end0)
                    {
                        if (start1 < num2) ComputeOverlaps(num, end0, mc, start1, num2, mco);
                        if (num2 < end1) ComputeOverlaps(num, end0, mc, num2, end1, mco);
                    }
                }
            }
        }

        void ComputeSelect(IEnvelope searchEnv, int start0, int end0, MonotoneChainSelectAction mcs)
        {
            ICoordinate c1 = Sequence.Get(start0);
            ICoordinate c2 = Sequence.Get(end0);

            if ((end0 - start0) == 1)
            {
                mcs.Select(this, start0);
            }
            else if (searchEnv.Intersects(c1, c2))
            {
                int num = (start0 + end0) / 2;
                if (start0 < num) ComputeSelect(searchEnv, start0, num, mcs);
                if (num < end0) ComputeSelect(searchEnv, num, end0, mcs);
            }
        }

        public virtual void GetLineSegment(int index, ref LineSegment ls)
        {
            ls.P0 = Sequence.Get(index);
            ls.P1 = Sequence.Get(index + 1);
        }

        public virtual void Select(IEnvelope searchEnv, MonotoneChainSelectAction mcs)
        {
            ComputeSelect(searchEnv, Start, End, mcs);
        }

        public virtual object Context
        {
            get { return iContext; }
        }

        public virtual ICoordinate[] GetCoordinates()
        {
            int num = 0;
            ICoordinate[] coordinateArray = new ICoordinate[(End - Start) + 1];

            for (int i = Start; i <= End; i++)
            {
                coordinateArray[num++] = Sequence.Get(i);
            }

            return coordinateArray;
        }

        public virtual int EndIndex
        {
            get { return End; }
        }

        public virtual IEnvelope Envelope
        {
            get
            {
                if (Bounds == null)
                {
                    Bounds = Sequence.Factory.Create<IEnvelope>(Sequence.Get(Start), Sequence.Get(End));
                }

                return Bounds;
            }
        }

        public virtual int Id
        {
            get { return ID; }
            set { ID = value; }
        }

        public virtual int StartIndex
        {
            get { return this.Start; }
        }

        public override string ToString()
        {
            return string.Concat("Start ", StartIndex, ", End", EndIndex);
        }
    }
}
