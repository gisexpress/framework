using System.Geometries.Graph;
using System.Linq;

namespace System.Geometries.Operation.Buffer
{
    /// <summary>
    /// Computes the raw offset curve for a
    /// single <see cref="IGeometry"/> component (ring, line or point).
    /// A raw offset curve line is not noded -
    /// it may contain self-intersections (and usually will).
    /// The final buffer polygon is computed by forming a topological graph
    /// of all the noded raw curves and tracing outside contours.
    /// The points in the raw curve are rounded
    /// to a given <see cref="IPrecisionModel"/>.
    /// </summary>
    internal class OffsetCurveBuilder
    {
        private double _distance;
        private readonly IBufferParameters _bufParams;

        public OffsetCurveBuilder(IBufferParameters bufParams)
        {
            _bufParams = bufParams;
        }

        /// <summary>
        /// Gets the buffer parameters being used to generate the curve.
        /// </summary>
        public IBufferParameters BufferParameters
        {
            get { return _bufParams; }
        }

        /// <summary>
        /// This method handles single points as well as LineStrings.
        /// LineStrings are assumed <b>not</b> to be closed (the function will not
        /// fail for closed lines, but will generate superfluous line caps).
        /// </summary>
        /// <param name="inputPts">The vertices of the line to offset</param>
        /// <param name="distance">The offset distance</param>
        /// <returns>A Coordinate array representing the curve <br/>
        /// or <c>null</c> if the curve is empty
        /// </returns>
        public ICoordinate[] GetLineCurve(ICoordinate[] inputPts, double distance)
        {
            _distance = distance;

            if (distance.IsZero() || (distance < 0.0 && !_bufParams.SingleSide))
            {
                return null;
            }

            double posDistance = Math.Abs(distance);
            OffsetSegmentGenerator segGen = GetSegmentGenerator(posDistance);

            if (inputPts.Length <= 1)
            {
                ComputePointCurve(inputPts[0], segGen);
            }
            else
            {
                if (_bufParams.SingleSide)
                {
                    var isRightSide = distance < 0.0;
                    ComputeSingleSidedBufferCurve(inputPts, isRightSide, segGen);
                }
                else
                {
                    ComputeLineBufferCurve(inputPts, segGen);
                }
            }

            return segGen.GetCoordinates();
        }

        /// <summary>
        /// This method handles the degenerate cases of single points and lines,
        /// as well as rings.
        /// </summary>
        /// <returns>A Coordinate array representing the curve<br/>
        /// or <c>null</c> if the curve is empty</returns>
        public ICoordinate[] GetRingCurve(ICoordinate[] inputPts, Positions side, double distance)
        {
            _distance = distance;
            if (inputPts.Length <= 2)
                return GetLineCurve(inputPts, distance);

            // optimize creating ring for for zero distance
            if (distance == 0.0)
            {
                return CopyCoordinates(inputPts);
            }
            OffsetSegmentGenerator segGen = GetSegmentGenerator(distance);
            ComputeRingBufferCurve(inputPts, side, segGen);
            return segGen.GetCoordinates();
        }

        public ICoordinate[] GetOffsetCurve(Coordinate[] inputPts, double distance)
        {
            _distance = distance;

            // a zero width offset curve is empty
            if (distance == 0.0) return null;

            var isRightSide = distance < 0.0;
            double posDistance = Math.Abs(distance);
            OffsetSegmentGenerator segGen = GetSegmentGenerator(posDistance);
            if (inputPts.Length <= 1)
            {
                ComputePointCurve(inputPts[0], segGen);
            }
            else
            {
                ComputeOffsetCurve(inputPts, isRightSide, segGen);
            }
            
            ICoordinate[] curvePts = segGen.GetCoordinates();
            
            // for right side line is traversed in reverse direction, so have to reverse generated line
            if (isRightSide)
            {
                return curvePts.Reverse().ToArray();
            }

            return curvePts;
        }

        private static ICoordinate[] CopyCoordinates(ICoordinate[] pts)
        {
            var copy = new Coordinate[pts.Length];
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = new Coordinate(pts[i]);
            }
            return copy;
        }

        private OffsetSegmentGenerator GetSegmentGenerator(double distance)
        {
            return new OffsetSegmentGenerator(_bufParams, distance);
        }

        /// <summary>
        /// Computes the distance tolerance to use during input
        /// line simplification.
        /// </summary>
        /// <param name="bufDistance">The buffer distance</param>
        /// <returns>The simplification tolerance</returns>
        private double SimplifyTolerance(double bufDistance)
        {
            return bufDistance * _bufParams.SimplifyFactor;
        }

        private void ComputePointCurve(ICoordinate pt, OffsetSegmentGenerator segGen)
        {
            switch (_bufParams.EndCapStyle)
            {
                case EndCapStyle.Round:
                    segGen.CreateCircle(pt);
                    break;
                case EndCapStyle.Square:
                    segGen.CreateSquare(pt);
                    break;
                    // otherwise curve is empty (e.g. for a butt cap);
            }
        }

        private void ComputeLineBufferCurve(ICoordinate[] inputPts, OffsetSegmentGenerator segGen)
        {
            var distTol = SimplifyTolerance(_distance);

            //--------- compute points for left side of line
            // Simplify the appropriate side of the line before generating
            var simp1 = BufferInputLineSimplifier.Simplify(inputPts, distTol);
            // MD - used for testing only (to eliminate simplification)
            //    Coordinate[] simp1 = inputPts;

            var n1 = simp1.Length - 1;
            segGen.InitSideSegments(simp1[0], simp1[1], Positions.Left);

            for (int i = 2; i <= n1; i++)
            {
                segGen.AddNextSegment(simp1[i], true);
            }

            segGen.AddLastSegment();
            // add line cap for end of line
            segGen.AddLineEndCap(simp1[n1 - 1], simp1[n1]);

            //---------- compute points for right side of line
            // Simplify the appropriate side of the line before generating
            var simp2 = BufferInputLineSimplifier.Simplify(inputPts, -distTol);
            // MD - used for testing only (to eliminate simplification)
            //    Coordinate[] simp2 = inputPts;
            var n2 = simp2.Length - 1;

            // since we are traversing line in opposite order, offset position is still LEFT
            segGen.InitSideSegments(simp2[n2], simp2[n2 - 1], Positions.Left);
            for (var i = n2 - 2; i >= 0; i--)
            {
                segGen.AddNextSegment(simp2[i], true);
            }
            segGen.AddLastSegment();
            // add line cap for start of line
            segGen.AddLineEndCap(simp2[1], simp2[0]);

            segGen.CloseRing();
        }

        /*
  private void OLDcomputeLineBufferCurve(Coordinate[] inputPts)
  {
    int n = inputPts.length - 1;

    // compute points for left side of line
    initSideSegments(inputPts[0], inputPts[1], Position.LEFT);
    for (int i = 2; i <= n; i++) {
      addNextSegment(inputPts[i], true);
    }
    addLastSegment();
    // add line cap for end of line
    addLineEndCap(inputPts[n - 1], inputPts[n]);

    // compute points for right side of line
    initSideSegments(inputPts[n], inputPts[n - 1], Position.LEFT);
    for (int i = n - 2; i >= 0; i--) {
      addNextSegment(inputPts[i], true);
    }
    addLastSegment();
    // add line cap for start of line
    addLineEndCap(inputPts[1], inputPts[0]);

    vertexList.closeRing();
  }
  */

        void ComputeSingleSidedBufferCurve(ICoordinate[] inputPts, bool isRightSide, OffsetSegmentGenerator segGen)
        {
            var distTol = SimplifyTolerance(_distance);

            if (isRightSide)
            {
                // add original line
                segGen.AddSegments(inputPts, true);

                //---------- compute points for right side of line
                // Simplify the appropriate side of the line before generating
                var simp2 = BufferInputLineSimplifier.Simplify(inputPts, -distTol);
                // MD - used for testing only (to eliminate simplification)
                // Coordinate[] simp2 = inputPts;
                var n2 = simp2.Length - 1;

                // since we are traversing line in opposite order, offset position is still LEFT
                segGen.InitSideSegments(simp2[n2], simp2[n2 - 1], Positions.Left);
                segGen.AddFirstSegment();
                for (var i = n2 - 2; i >= 0; i--)
                {
                    segGen.AddNextSegment(simp2[i], true);
                }
            }
            else
            {
                // add original line
                segGen.AddSegments(inputPts, false);

                //--------- compute points for left side of line
                // Simplify the appropriate side of the line before generating
                var simp1 = BufferInputLineSimplifier.Simplify(inputPts, distTol);
                // MD - used for testing only (to eliminate simplification)
                //      Coordinate[] simp1 = inputPts;

                var n1 = simp1.Length - 1;
                segGen.InitSideSegments(simp1[0], simp1[1], Positions.Left);
                segGen.AddFirstSegment();
                for (var i = 2; i <= n1; i++)
                {
                    segGen.AddNextSegment(simp1[i], true);
                }
            }

            segGen.AddLastSegment();
            segGen.CloseRing();
        }

        void ComputeOffsetCurve(Coordinate[] inputPts, Boolean isRightSide, OffsetSegmentGenerator segGen)
        {
            var distTol = SimplifyTolerance(_distance);

            if (isRightSide)
            {
                //---------- compute points for right side of line
                // Simplify the appropriate side of the line before generating
                var simp2 = BufferInputLineSimplifier.Simplify(inputPts, -distTol);
                // MD - used for testing only (to eliminate simplification)
                // Coordinate[] simp2 = inputPts;
                var n2 = simp2.Length - 1;

                // since we are traversing line in opposite order, offset position is still LEFT
                segGen.InitSideSegments(simp2[n2], simp2[n2 - 1], Positions.Left);
                segGen.AddFirstSegment();
                for (var i = n2 - 2; i >= 0; i--)
                {
                    segGen.AddNextSegment(simp2[i], true);
                }
            }
            else
            {
                //--------- compute points for left side of line
                // Simplify the appropriate side of the line before generating
                var simp1 = BufferInputLineSimplifier.Simplify(inputPts, distTol);
                // MD - used for testing only (to eliminate simplification)
                // Coordinate[] simp1 = inputPts;

                var n1 = simp1.Length - 1;
                segGen.InitSideSegments(simp1[0], simp1[1], Positions.Left);
                segGen.AddFirstSegment();
                for (var i = 2; i <= n1; i++)
                {
                    segGen.AddNextSegment(simp1[i], true);
                }
            }
            segGen.AddLastSegment();
        }

        private void ComputeRingBufferCurve(ICoordinate[] inputPts, Positions side, OffsetSegmentGenerator segGen)
        {
            // simplify input line to improve performance
            var distTol = SimplifyTolerance(_distance);
            // ensure that correct side is simplified
            if (side == Positions.Right)
                distTol = -distTol;
            var simp = BufferInputLineSimplifier.Simplify(inputPts, distTol);
            // MD - used for testing only (to eliminate simplification)
            // Coordinate[] simp = inputPts;

            var n = simp.Length - 1;
            segGen.InitSideSegments(simp[n - 1], simp[0], side);
            for (var i = 1; i <= n; i++)
            {
                var addStartPoint = i != 1;
                segGen.AddNextSegment(simp[i], addStartPoint);
            }
            segGen.CloseRing();
        }

        public static double GetAngle(ICoordinate center, ICoordinate p0, ICoordinate p1, int direction, out double startAngle, out double endAngle)
        {
            return GetAngle(center, p0, p1, direction, 1.0, out startAngle, out endAngle);
        }

        public static double GetAngle(ICoordinate center, ICoordinate p0, ICoordinate p1, int direction, double ratio, out double startAngle, out double endAngle)
        {
            startAngle = Math.Atan2(ratio * (p0.Y - center.Y), p0.X - center.X);
            endAngle = Math.Atan2(ratio * (p1.Y - center.Y), p1.X - center.X);

            if (direction == -1)
            {
                if (startAngle <= endAngle)
                {
                    startAngle += AppConstants.TwoPI;
                }
            }
            else if (startAngle >= endAngle)
            {
                startAngle -= AppConstants.TwoPI;
            }

            return endAngle - startAngle;
        }
    }
}