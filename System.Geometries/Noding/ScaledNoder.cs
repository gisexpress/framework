//using System.Collections.Generic;

//namespace System.Geometries.Noding
//{

//    /// <summary>
//    /// Wraps a <see cref="INoder" /> and transforms its input into the integer domain.
//    /// This is intended for use with Snap-Rounding noders,
//    /// which typically are only intended to work in the integer domain.
//    /// Offsets can be provided to increase the number of digits of available precision.
//    /// <para>
//    /// Clients should be aware that rescaling can involve loss of precision,
//    /// which can cause zero-length line segments to be created.
//    /// These in turn can cause problems when used to build a planar graph.
//    /// This situation should be checked for and collapsed segments removed if necessary.
//    /// </para>
//    /// </summary>
//    internal class ScaledNoder : INoder
//    {
//        private readonly INoder _noder;
//        private readonly double _scaleFactor;
//        private readonly bool _isScaled;

//        public ScaledNoder(INoder noder)
//            : this(noder, 1.0)
//        {
//        }

//        ScaledNoder(INoder noder, double scaleFactor)
//        {
//            _noder = noder;
//            _scaleFactor = scaleFactor;
//            // no need to scale if input precision is already integral
//            _isScaled = !IsIntegerPrecision;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public bool IsIntegerPrecision
//        {
//            get
//            {
//                return _scaleFactor == 1.0;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public IList<ISegmentString> GetNodedSubstrings()
//        {
//            IList<ISegmentString> splitSS = _noder.GetNodedSubstrings();
//            if (_isScaled)
//                Rescale(splitSS);
//            return splitSS;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="inputSegStrings"></param>
//        public void ComputeNodes(IList<ISegmentString> inputSegStrings)
//        {
//            IList<ISegmentString> intSegStrings = inputSegStrings;
//            if (_isScaled)
//                intSegStrings = Scale(inputSegStrings);
//            _noder.ComputeNodes(intSegStrings);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="segStrings"></param>
//        /// <returns></returns>
//        private IList<ISegmentString> Scale(IList<ISegmentString> segStrings)
//        {
//            return CollectionUtil.Transform<ISegmentString, ISegmentString>(segStrings, ss => ((ISegmentString)new NodedSegmentString(Scale(ss.Sequence), ss.Context)));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="pts"></param>
//        /// <returns></returns>
//        private ICoordinate[] Scale(ICoordinateCollection pts)
//        {
//            Coordinate[] roundPts = new Coordinate[pts.Count];
            
//            for (int i = 0; i < pts.Count; i++)
//                roundPts[i] = new Coordinate(Math.Round((pts[i].X) * _scaleFactor), Math.Round((pts[i].Y) * _scaleFactor), pts[i].Z);
            
//            return CoordinateArrays.RemoveRepeatedPoints(roundPts);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="segStrings"></param>
//        private void Rescale(IList<ISegmentString> segStrings)
//        {
//            CollectionUtil.Apply(segStrings,
//                ss => { Rescale(ss.Coordinates); return null; });
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="pts"></param>
//        private void Rescale(ICoordinate[] pts)
//        {
//            Coordinate p0 = null;
//            Coordinate p1 = null;

//            if (pts.Length == 2)
//            {
//                p0 = new Coordinate(pts[0]);
//                p1 = new Coordinate(pts[1]);
//            }

//            for (int i = 0; i < pts.Length; i++)
//            {
//                pts[i].X = pts[i].X / _scaleFactor;
//                pts[i].Y = pts[i].Y / _scaleFactor;
//            }
//        }
//    }
//}
