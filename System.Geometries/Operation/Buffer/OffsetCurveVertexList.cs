using System.Collections.Generic;

namespace System.Geometries.Operation.Buffer
{
    ///<summary>
    /// A list of the vertices in a constructed offset curve.
    ///</summary>
    /// <remarks>Automatically removes close adjacent vertices.</remarks>
    /// <author>Martin Davis</author>
    public class OffsetCurveVertexList
    {
        private readonly List<Coordinate> _ptList;
        private double _minimimVertexDistance;

        public OffsetCurveVertexList()
        {
            _ptList = new List<Coordinate>();
        }

        /// <summary>
        /// The distance below which two adjacent points on the curve are considered to be coincident.
        /// </summary>
        /// <remarks>This is chosen to be a small fraction of the offset distance.</remarks>
        public double MinimumVertexDistance { get { return _minimimVertexDistance; } set { _minimimVertexDistance = value; } }

        /// <summary>
        /// Function to add a point
        /// </summary>
        /// <remarks>
        /// The point is only added if <see cref="IsDuplicate(Coordinate)"/> evaluates to false.
        /// </remarks>
        /// <param name="pt">The point to add.</param>
        public void AddPt(ICoordinate pt)
        {
            var bufPt = new Coordinate(pt);

            // don't add duplicate (or near-duplicate) points
            if (IsDuplicate(bufPt))
                return;
            _ptList.Add(bufPt);
            //System.out.println(bufPt);
        }

        ///<summary>
        /// Tests whether the given point duplicates the previous point in the list (up to tolerance)
        ///</summary>
        /// <param name="pt">The point to test</param>
        /// <returns>true if the point duplicates the previous point</returns>
        private bool IsDuplicate(Coordinate pt)
        {
            if (_ptList.Count < 1)
                return false;
            var lastPt = _ptList[_ptList.Count - 1];
            var ptDist = pt.Distance(lastPt);
            if (ptDist < _minimimVertexDistance)
                return true;
            return false;
        }

        /// <summary>
        /// Automatically closes the ring (if it not alread is).
        /// </summary>
        public void CloseRing()
        {
            if (_ptList.Count < 1) return;
            var startPt = new Coordinate(_ptList[0]);
            var lastPt = _ptList[_ptList.Count - 1];
            /*Coordinate last2Pt = null;
              if (ptList.Count >= 2)
                  last2Pt = (Coordinate)ptList[ptList.Count - 2];*/
            if (startPt.IsEquivalent(lastPt)) return;
            _ptList.Add(startPt);
        }

        /// <summary>
        /// Gets the Coordinates for the curve.
        /// </summary>
        public ICoordinate[] Coordinates
        {
            get
            {
                // check that points are a ring - add the startpoint again if they are not
                if (_ptList.Count > 1)
                {
                    var start = _ptList[0];
                    var end = _ptList[_ptList.Count - 1];
                    if (!start.IsEquivalent(end)) AddPt(start);
                }
                var coord = _ptList.ToArray();
                return coord;
            }
        }
    }
}