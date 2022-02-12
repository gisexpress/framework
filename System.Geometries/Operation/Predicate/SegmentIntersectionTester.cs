//using System.Collections.Generic;
//using System.Geometries.Algorithm;

//namespace System.Geometries.Operation.Predicate
//{
//    /// <summary>
//    /// Tests if any line segments in two sets of <see cref="CoordinateSequences"/> intersect.
//    /// Optimized for use when at least one input is of small size.
//    /// Short-circuited to return as soon an intersection is found.
//    /// </summary>
//    public class SegmentIntersectionTester
//    {
//        // for purposes of intersection testing, don't need to set precision model
//        private readonly LineIntersector li = new RobustLineIntersector();

//        private bool _hasIntersection;
//        private ICoordinate pt00;
//        private ICoordinate pt01;
//        private ICoordinate pt10;
//        private ICoordinate pt11;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="seq"></param>
//        /// <param name="lines"></param>
//        /// <returns></returns>
//        public bool HasIntersectionWithLineStrings(ICoordinateCollection seq, ICollection<IGeometry> lines)
//        {
//            foreach (ILineString line in lines)
//            {
//                HasIntersection(seq, line.Coordinates);
//                if (_hasIntersection)
//                    break;
//            }
//            return _hasIntersection;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="seq0"></param>
//        /// <param name="seq1"></param>
//        /// <returns></returns>
//        public bool HasIntersection(ICoordinateCollection seq0, ICoordinateCollection seq1)
//        {
//            for (int i = 1; i < seq0.Count && !_hasIntersection; i++)
//            {
//                pt00 = seq0.Get(i - 1);
//                pt01 = seq0.Get(i);

//                for (int j = 1; j < seq1.Count && !_hasIntersection; j++)
//                {
//                    pt10 = seq1.Get(j - 1);
//                    pt11 = seq1.Get(j);

//                    li.ComputeIntersection(pt00, pt01, pt10, pt11);
//                    if (li.HasIntersection)
//                        _hasIntersection = true;
//                }
//            }
//            return _hasIntersection;
//        }
//    }
//}
