namespace System.Geometries.Noding
{
    /// <summary>
    ///  Allows comparing <see cref="Coordinate" /> arrays in an orientation-independent way.
    /// </summary>
    internal class OrientedCoordinateArray : IComparable
    {
        public OrientedCoordinateArray(ICoordinateCollection sequence)
        {
            Sequence = sequence;
            iOrientation = Orientation(sequence);
        }

        protected readonly ICoordinateCollection Sequence;
        protected readonly bool iOrientation;

        /// <summary>
        /// Computes the canonical orientation for a coordinate array.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns>
        /// <c>true</c> if the points are oriented forwards <br/>
        /// or <c>false</c>if the points are oriented in reverse.
        /// </returns>
        static bool Orientation(ICoordinateCollection sequence)
        {
            return CoordinateArrays.IncreasingDirection(sequence) == 1;
        }

        /// <summary>
        /// Compares two <see cref="OrientedCoordinateArray" />s for their relative order.
        /// </summary>
        /// <param name="o1"></param>
        /// <returns>
        /// -1 this one is smaller;<br/>
        ///  0 the two objects are equal;<br/>
        ///  1 this one is greater.
        /// </returns>
        public int CompareTo(object o1)
        {
            OrientedCoordinateArray oca = (OrientedCoordinateArray)o1;
            return CompareOriented(Sequence, iOrientation, oca.Sequence, oca.iOrientation);
        }

        static int CompareOriented(ICoordinateCollection sequence0, bool orientation0, ICoordinateCollection sequence1, bool orientation1)
        {
            int dir1 = orientation0 ? 1 : -1;
            int dir2 = orientation1 ? 1 : -1;

            int limit1 = orientation0 ? sequence0.Count : -1;
            int limit2 = orientation1 ? sequence1.Count : -1;

            int i1 = orientation0 ? 0 : sequence0.Count - 1;
            int i2 = orientation1 ? 0 : sequence1.Count - 1;

            while (true)
            {
                int compPt = sequence0.Get(i1).CompareTo(sequence1.Get(i2));

                if (compPt != 0)
                {
                    return compPt;
                }

                i1 += dir1;
                i2 += dir2;

                bool done1 = i1 == limit1;
                bool done2 = i2 == limit2;

                if (done1 && !done2)
                {
                    return -1;
                }

                if (!done1 && done2)
                {
                    return 1;
                }

                if (done1 && done2)
                {
                    return 0;
                }
            }
        }
    }
}
