using System.ComponentModel;

namespace System.Geometries.Algorithm
{
    public class MinimumDiameter
    {
        public MinimumDiameter(IGeometry input) : this(input, false)
        {
        }

        public MinimumDiameter(IGeometry input, bool isConvex)
        {
            Input = input;
            IsConvex = isConvex;
        }

        public static IGeometry GetMinimumRectangle(IGeometry g)
        {
            return new MinimumDiameter(g).GetMinimumRectangle();
        }

        public static IGeometry GetMinimumDiameter(IGeometry g)
        {
            return new MinimumDiameter(g).Diameter;
        }

        readonly IGeometry Input;
        readonly bool IsConvex;

        ICoordinate[] ConvexPoints;
        LineSegment MinSegment = new LineSegment();
        ICoordinate MinWidthPt;

        int MinPtIndex;
        double MinWidth;

        /// <summary> 
        /// Gets the length of the minimum diameter of the input Geometry.
        /// </summary>
        /// <returns>The length of the minimum diameter.</returns>
        public double Length
        {
            get
            {
                ComputeMinimumDiameter();
                return MinWidth;
            }
        }

        /// <summary>
        /// Gets the <c>Coordinate</c> forming one end of the minimum diameter.
        /// </summary>
        /// <returns>A coordinate forming one end of the minimum diameter.</returns>
        public ICoordinate WidthCoordinate
        {
            get
            {
                ComputeMinimumDiameter();
                return MinWidthPt;
            }
        }

        /// <summary>
        /// Gets the segment forming the base of the minimum diameter.
        /// </summary>
        /// <returns>The segment forming the base of the minimum diameter.</returns>
        public ILineString SupportingSegment
        {
            get
            {
                ComputeMinimumDiameter();
                return Input.Factory.Create<ILineString>(MinSegment.P0, MinSegment.P1);
            }
        }

        /// <summary>
        /// Gets a <c>LineString</c> which is a minimum diameter.
        /// </summary>
        /// <returns>A <c>LineString</c> which is a minimum diameter.</returns>
        public ILineString Diameter
        {
            get
            {
                ComputeMinimumDiameter();

                // return empty linearRing if no minimum width calculated
                if (MinWidthPt == null)
                {
                    return default;
                }

                ICoordinate p0 = MinWidthPt.Clone();
                ICoordinate p1 = MinWidthPt.Clone();

                if (MinSegment.Project(p1))
                {
                    return Input.Factory.Create<ILineString>(p0, p1);
                }

                return default;
            }
        }

        void ComputeMinimumDiameter()
        {
            if (MinWidthPt == null)
            {
                if (IsConvex)
                {
                    ComputeWidthConvex(Input);
                }
                else
                {
                    ComputeWidthConvex(new ConvexHull(Input).GetConvexHull());
                }
            }
        }

        void ComputeWidthConvex(IGeometry g)
        {
            var poly = g as IPolygon;

            if (poly == null)
            {
                ConvexPoints = g.Coordinates.ToArray();
            }
            else
            {
                ConvexPoints = poly.ExteriorRing.Coordinates.ToArray();
            }

            // Special cases for lines or points or degenerate rings
            if (ConvexPoints.Length == 0)
            {
                MinWidth = 0.0;
                MinWidthPt = null;
                MinSegment = null;
            }
            else if (ConvexPoints.Length == 1)
            {
                MinWidth = 0.0;
                MinWidthPt = ConvexPoints[0];
                MinSegment.P0 = ConvexPoints[0];
                MinSegment.P1 = ConvexPoints[0];
            }
            else if (ConvexPoints.Length == 2 || ConvexPoints.Length == 3)
            {
                MinWidth = 0.0;
                MinWidthPt = ConvexPoints[0];
                MinSegment.P0 = ConvexPoints[0];
                MinSegment.P1 = ConvexPoints[1];
            }
            else
            {
                ComputeConvexRingMinDiameter(ConvexPoints);
            }
        }

        /// <summary> 
        /// Compute the width information for a ring of <c>Coordinate</c>s.
        /// Leaves the width information in the instance variables.
        /// </summary>
        void ComputeConvexRingMinDiameter(ICoordinate[] points)
        {
            // for each segment in the ring
            int currMaxIndex = 1;
            MinWidth = double.MaxValue;

            var seg = new LineSegment();
            // Compute the max distance for all segments in the ring, and pick the minimum
            for (int i = 0; i < points.Length - 1; i++)
            {
                seg.P0 = points[i];
                seg.P1 = points[i + 1];
                currMaxIndex = FindMaxPerpDistance(points, seg, currMaxIndex);
            }
        }

        int FindMaxPerpDistance(ICoordinate[] points, LineSegment segment, int startIndex)
        {
            double maxPerpDistance = segment.DistancePerpendicular(points[startIndex]);
            double nextPerpDistance = maxPerpDistance;

            int maxIndex = startIndex;
            int nextIndex = maxIndex;

            while (nextPerpDistance >= maxPerpDistance)
            {
                maxPerpDistance = nextPerpDistance;
                maxIndex = nextIndex;

                nextIndex = NextIndex(points, maxIndex);
                nextPerpDistance = segment.DistancePerpendicular(points[nextIndex]);
            }

            // Found maximum width for this segment - update global min dist if appropriate
            if (maxPerpDistance < MinWidth)
            {
                MinPtIndex = maxIndex;
                MinWidth = maxPerpDistance;
                MinWidthPt = points[MinPtIndex];
                MinSegment = new LineSegment(segment);
            }

            return maxIndex;
        }

        static int NextIndex(ICoordinate[] pts, int index)
        {
            index++;
            if (index >= pts.Length) index = 0;
            return index;
        }

        /// <summary>
        /// Gets the minimum rectangular <see cref="IPolygon"/> which encloses the input geometry.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The rectangle has width equal to the minimum diameter, and a longer length.
        /// If the convex hull of the input is degenerate (a line or point) a <see cref="ILineString"/> or <see cref="IPoint"/> is returned.
        /// </para>
        /// <para>
        /// The minimum rectangle can be used as an extremely generalized representation for the given geometry.
        /// </para>
        /// </remarks>
        /// <returns>The minimum rectangle enclosing the input (or a line or point if degenerate)</returns>
        public IGeometry GetMinimumRectangle()
        {
            ComputeMinimumDiameter();

            // check if minimum rectangle is degenerate (a point or line segment)
            if (MinWidth == 0.0)
            {
                if (MinSegment.P0.IsEquivalent(MinSegment.P1))
                {
                    return Input.Factory.Create<IPoint>(Input, MinSegment.P0);
                }

                return Input.Factory.Create<ILineString>(MinSegment.P0, MinSegment.P1);
            }

            // Deltas for the base segment of the minimum diameter
            double dx = MinSegment.P1.X - MinSegment.P0.X;
            double dy = MinSegment.P1.Y - MinSegment.P0.Y;

            double minPara = double.MaxValue;
            double maxPara = -double.MaxValue;
            double minPerp = double.MaxValue;
            double maxPerp = -double.MaxValue;

            // Compute maxima and minima of lines parallel and perpendicular to base segment
            for (int i = 0; i < ConvexPoints.Length; i++)
            {

                double paraC = ComputeC(dx, dy, ConvexPoints[i]);
                if (paraC > maxPara) maxPara = paraC;
                if (paraC < minPara) minPara = paraC;

                double perpC = ComputeC(-dy, dx, ConvexPoints[i]);
                if (perpC > maxPerp) maxPerp = perpC;
                if (perpC < minPerp) minPerp = perpC;
            }

            // compute lines along edges of minimum rectangle
            LineSegment maxPerpLine = ComputeSegmentForLine(-dx, -dy, maxPerp);
            LineSegment minPerpLine = ComputeSegmentForLine(-dx, -dy, minPerp);
            LineSegment maxParaLine = ComputeSegmentForLine(-dy, dx, maxPara);
            LineSegment minParaLine = ComputeSegmentForLine(-dy, dx, minPara);

            // compute vertices of rectangle (where the para/perp max & min lines intersect)
            ICoordinate p0 = maxParaLine.Intersection(maxPerpLine);
            ICoordinate p1 = minParaLine.Intersection(maxPerpLine);
            ICoordinate p2 = minParaLine.Intersection(minPerpLine);
            ICoordinate p3 = maxParaLine.Intersection(minPerpLine);

            return Input.Factory.Create<IPolygon>(Input, p0, p1, p2, p3, p0);

        }

        static double ComputeC(double a, double b, ICoordinate p)
        {
            return a * p.Y - b * p.X;
        }

        static LineSegment ComputeSegmentForLine(double a, double b, double c)
        {
            Coordinate p0, p1;
            
            // Line eqn is ax + by = c
            // Slope is a/b.
            // If slope is steep, use y values as the inputs
            
            if (Math.Abs(b) > Math.Abs(a))
            {
                p0 = new Coordinate(0.0, c / b);
                p1 = new Coordinate(1.0, c / b - a / b);
            }
            else
            {
                p0 = new Coordinate(c / a, 0.0);
                p1 = new Coordinate(c / a - b / a, 1.0);
            }
            
            return new LineSegment(p0, p1);
        }
    }
}
