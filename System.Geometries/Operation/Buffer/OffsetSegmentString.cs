using System.Collections.Generic;

namespace System.Geometries.Operation.Buffer
{
    /// <summary>
    /// A dynamic list of the vertices in a constructed offset curve.
    /// Automatically removes adjacent vertices
    /// which are closer than a given tolerance.
    /// </summary>
    /// <author>Martin Davis</author>
    internal class OffsetSegmentString
    {
        readonly List<ICoordinate> _ptList;

        /**
         * The distance below which two adjacent points on the curve
         * are considered to be coincident.
         * This is chosen to be a small fraction of the offset distance.
         */
        private double _minimimVertexDistance;

        public OffsetSegmentString()
        {
            _ptList = new List<ICoordinate>();
        }

        public double MinimumVertexDistance
        {
            get { return _minimimVertexDistance; }
            set { _minimimVertexDistance = value; }
        }

        public void AddPt(ICoordinate pt)
        {
            var bufPt = new Coordinate(pt);
            // don't add duplicate (or near-duplicate) points
            if (IsRedundant(bufPt))
                return;
            _ptList.Add(bufPt);
            //Console.WriteLine(bufPt);
        }

        public void AddPts(ICoordinate[] pt, bool isForward)
        {
            if (isForward)
            {
                for (int i = 0; i < pt.Length; i++)
                {
                    AddPt(pt[i]);
                }
            }
            else
            {
                for (int i = pt.Length - 1; i >= 0; i--)
                {
                    AddPt(pt[i]);
                }
            }
        }

        /// <summary>
        /// Tests whether the given point is redundant
        /// relative to the previous
        /// point in the list (up to tolerance).
        /// </summary>
        /// <param name="pt"></param>
        /// <returns>true if the point is redundant</returns>
        private bool IsRedundant(Coordinate pt)
        {
            if (_ptList.Count < 1)
                return false;
            var lastPt = _ptList[_ptList.Count - 1];
            double ptDist = pt.Distance(lastPt);
            if (ptDist < _minimimVertexDistance)
                return true;
            return false;
        }

        public void CloseRing()
        {
            if (_ptList.Count < 1)
                return;

            var startPt = new Coordinate(_ptList[0]);
            var lastPt = _ptList[_ptList.Count - 1];
            /*
            Coordinate last2Pt = null;
            if (ptList.Count >= 2)
                last2Pt = ptList[ptList.Count - 2];
            */
            if (startPt.IsEquivalent(lastPt)) return;
            _ptList.Add(startPt);
        }

        public void Reverse()
        {
        }

        public ICoordinate[] GetCoordinates()
        {
            return _ptList.ToArray();
        }
    }
}