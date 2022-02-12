using System.Collections.Generic;
using System.Geometries.Mathematics;
using System.Linq;

namespace System.Geometries.Algorithm
{
    internal static class CGAlgorithms
    {
        /// <summary> 
        /// A value that indicates an orientation of clockwise, or a right turn.
        /// </summary>
        public const int Clockwise = -1;
        /// <summary> 
        /// A value that indicates an orientation of clockwise, or a right turn.
        /// </summary>
        public const int Right = Clockwise;

        /// <summary>
        /// A value that indicates an orientation of counterclockwise, or a left turn.
        /// </summary>
        public const int CounterClockwise = 1;
        /// <summary>
        /// A value that indicates an orientation of counterclockwise, or a left turn.
        /// </summary>
        public const int Left = CounterClockwise;

        /// <summary>
        /// A value that indicates an orientation of collinear, or no turn (straight).
        /// </summary>
        public const int Collinear = 0;
        /// <summary>
        /// A value that indicates an orientation of collinear, or no turn (straight).
        /// </summary>
        public const int Straight = Collinear;

        /// <summary> 
        /// Returns the index of the direction of the point <c>q</c>
        /// relative to a vector specified by <c>p1-p2</c>.
        /// </summary>
        /// <param name="p1">The origin point of the vector.</param>
        /// <param name="p2">The final point of the vector.</param>
        /// <param name="q">The point to compute the direction to.</param>
        /// <returns> 
        /// 1 if q is counter-clockwise (left) from p1-p2,
        /// -1 if q is clockwise (right) from p1-p2,
        /// 0 if q is collinear with p1-p2.
        /// </returns>
        public static int OrientationIndex(ICoordinate p1, ICoordinate p2, ICoordinate q)
        {
            return CGAlgorithmsDD.OrientationIndex(p1, p2, q);
        }

        /// <summary> 
        /// Tests whether a point lies inside or on a ring.
        /// </summary>
        /// <remarks>
        /// <para>The ring may be oriented in either direction.</para>
        /// <para>A point lying exactly on the ring boundary is considered to be inside the ring.</para>
        /// <para>This method does <i>not</i> first check the point against the envelope
        /// of the ring.</para>
        /// </remarks>
        /// <param name="p">Point to check for ring inclusion.</param>
        /// <param name="ring">An array of <see cref="Coordinate"/>s representing the ring (which must have first point identical to last point)</param>
        /// <returns>true if p is inside ring.</returns>
        /// <see cref="IPointInRing"/>
        public static bool IsPointInRing(ICoordinate p, IEnumerable<ICoordinate> ring)
        {
            return RayCrossingCounter.LocatePointInRing(p, ring) != Locations.Exterior;
        }

        ///<summary>
        /// Determines whether a point lies in the interior, on the boundary, or in the exterior of a ring.
        ///</summary>
        /// <remarks>
        /// <para>The ring may be oriented in either direction.</para>
        /// <para>This method does <i>not</i> first check the point against the envelope of the ring.</para>
        /// </remarks>
        /// <param name="p">Point to check for ring inclusion</param>
        /// <param name="ring">A sequence of coordinates representing the ring (which must have first point identical to last point)</param>
        /// <returns>The <see cref="Location"/> of p relative to the ring</returns>
        public static Locations LocatePointInRing(ICoordinate p, ICoordinateCollection ring)
        {
            return RayCrossingCounter.LocatePointInRing(p, ring);
        }

        /// <summary> 
        /// Tests whether a point lies on the line segments defined by a list of coordinates.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sequence"></param>
        /// <returns>true if the point is a vertex of the line
        /// or lies in the interior of a line segment in the linestring
        /// </returns>
        public static bool IsOnLine(Coordinate p, ICoordinateCollection sequence)
        {
            var lineIntersector = new RobustLineIntersector();

            for (int i = 1; i < sequence.Count; i++)
            {
                ICoordinate p0 = sequence.Get(i - 1);
                ICoordinate p1 = sequence.Get(i);

                lineIntersector.ComputeIntersection(p, p0, p1);

                if (lineIntersector.HasIntersection)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Computes whether a ring defined by an array of <see cref="Coordinate" />s is oriented counter-clockwise.
        /// </summary>>
        /// <remarks>
        /// <list type="Bullet">
        /// <item>The list of points is assumed to have the first and last points equal.</item>
        /// <item>This will handle coordinate lists which contain repeated points.</item>
        /// </list>
        /// <para>This algorithm is only guaranteed to work with valid rings. If the ring is invalid (e.g. self-crosses or touches), the computed result may not be correct.</para>
        /// </remarks>
        /// <param name="ring">An array of <see cref="Coordinate"/>s froming a ring</param>
        /// <returns>true if the ring is oriented <see cref="Orientation.CounterClockwise"/></returns>
        /// <exception cref="ArgumentException">If there are too few points to determine orientation (&lt;4)</exception>
        public static bool IsCCW(ICoordinate[] ring)
        {
            // # of points without closing endpoint
            int nPts = ring.Length - 1;

            // sanity check
            if (nPts < 3)
            {
                return false;
            }

            // find highest point
            ICoordinate hiPt = ring[0];
            int hiIndex = 0;
            for (int i = 1; i <= nPts; i++)
            {
                ICoordinate p = ring[i];
                if (p.Y > hiPt.Y)
                {
                    hiPt = p;
                    hiIndex = i;
                }
            }

            // find distinct point before highest point
            int iPrev = hiIndex;
            do
            {
                iPrev = iPrev - 1;
                if (iPrev < 0) iPrev = nPts;
            }
            while (ring[iPrev].IsEquivalent(hiPt) && iPrev != hiIndex);

            // find distinct point after highest point
            int iNext = hiIndex;
            do
                iNext = (iNext + 1) % nPts;
            while (ring[iNext].IsEquivalent(hiPt) && iNext != hiIndex);

            ICoordinate prev = ring[iPrev];
            ICoordinate next = ring[iNext];

            // This check catches cases where the ring contains an A-B-A configuration of points.
            // This can happen if the ring does not contain 3 distinct points (including the case where the input array has fewer than 4 elements), or it contains coincident line segments.
            if (prev.IsEquivalent(hiPt) || next.IsEquivalent(hiPt) || prev.IsEquivalent(next))
            {
                return false;
            }

            int disc = ComputeOrientation(prev, hiPt, next);

            // If disc is exactly 0, lines are collinear.  There are two possible cases:
            // (1) the lines lie along the x axis in opposite directions
            // (2) the lines lie on top of one another
            //
            // (1) is handled by checking if next is left of prev ==> CCW
            // (2) will never happen if the ring is valid, so don't check for it
            // (Might want to assert this)

            bool isCCW = false;

            if (disc == 0)
            {
                // poly is CCW if prev x is right of next x
                isCCW = (prev.X > next.X);
            }
            else
            {
                // if area is positive, points are ordered CCW
                isCCW = (disc > 0);
            }

            return isCCW;
        }

        /// <summary>
        /// Computes whether a ring defined by a coordinate sequence is oriented counter-clockwise.
        /// </summary>>
        /// <remarks>
        /// <list type="Bullet">
        /// <item>The list of points is assumed to have the first and last points equal.</item>
        /// <item>This will handle coordinate lists which contain repeated points.</item>
        /// </list>
        /// <para>This algorithm is only guaranteed to work with valid rings. If the ring is invalid (e.g. self-crosses or touches), the computed result may not be correct.</para>
        /// </remarks>
        /// <param name="ring">A coordinate sequence froming a ring</param>
        /// <returns>true if the ring is oriented <see cref="Orientation.CounterClockwise"/></returns>
        /// <exception cref="ArgumentException">If there are too few points to determine orientation (&lt;4)</exception>
        public static bool IsCCW(ICoordinateCollection ring)
        {
            return IsCCW(ring.ToArray());
        }

        /// <summary>
        /// Computes the orientation of a point q to the directed line segment p1-p2.
        /// The orientation of a point relative to a directed line segment indicates which way you turn to get to q after travelling from p1 to p2.
        /// </summary>
        /// <param name="p1">The first vertex of the line segment</param>
        /// <param name="p2">The second vertex of the line segment</param>
        /// <param name="q">The point to compute the relative orientation of</param>
        /// <returns> 
        /// 1 if q is counter-clockwise from p1-p2,
        /// or -1 if q is clockwise from p1-p2,
        /// or 0 if q is collinear with p1-p2
        /// </returns>
        public static int ComputeOrientation(ICoordinate p1, ICoordinate p2, ICoordinate q)
        {
            return OrientationIndex(p1, p2, q);
        }

        /// <summary> 
        /// Computes the distance from a point p to a line segment AB.
        /// Note: NON-ROBUST!
        /// </summary>
        /// <param name="p">The point to compute the distance for.</param>
        /// <param name="A">One point of the line.</param>
        /// <param name="B">Another point of the line (must be different to A).</param>
        /// <returns> The distance from p to line segment AB.</returns>
        public static double DistancePointLine(ICoordinate p, ICoordinate A, ICoordinate B)
        {
            // if start = end, then just compute distance to one of the endpoints
            if (A.X == B.X && A.Y == B.Y)
            {
                return p.Distance(A);
            }

            // otherwise use comp.graphics.algorithms Frequently Asked Questions method
            /*(1)     	      AC dot AB
                        r =   ---------
                              ||AB||^2
             
		                r has the following meaning:
		                r=0 Point = A
		                r=1 Point = B
		                r<0 Point is on the backward extension of AB
		                r>1 Point is on the forward extension of AB
		                0<r<1 Point is interior to AB
	        */

            var len2 = ((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
            var r = ((p.X - A.X) * (B.X - A.X) + (p.Y - A.Y) * (B.Y - A.Y)) / len2;

            if (r <= 0.0)
                return p.Distance(A);
            if (r >= 1.0)
                return p.Distance(B);


            /*(2)
		                    (Ay-Cy)(Bx-Ax)-(Ax-Cx)(By-Ay)
		                s = -----------------------------
		             	                Curve^2

		                Then the distance from C to Point = |s|*Curve.
      
                        This is the same calculation as {@link #distancePointLinePerpendicular}.
                        Unrolled here for performance.
	        */

            var s = ((A.Y - p.Y) * (B.X - A.X) - (A.X - p.X) * (B.Y - A.Y)) / len2;

            return Math.Abs(s) * Math.Sqrt(len2);
        }

        /// <summary> 
        /// Computes the perpendicular distance from a point p
        /// to the (infinite) line containing the points AB
        /// </summary>
        /// <param name="p">The point to compute the distance for.</param>
        /// <param name="A">One point of the line.</param>
        /// <param name="B">Another point of the line (must be different to A).</param>
        /// <returns>The perpendicular distance from p to line AB.</returns>
        public static double DistancePointLinePerpendicular(Coordinate p, Coordinate A, Coordinate B)
        {
            // use comp.graphics.algorithms Frequently Asked Questions method
            /*(2)
                            (Ay-Cy)(Bx-Ax)-(Ax-Cx)(By-Ay)
                        s = -----------------------------
                                         Curve^2

                        Then the distance from C to Point = |s|*Curve.
            */
            var len2 = ((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
            var s = ((A.Y - p.Y) * (B.X - A.X) - (A.X - p.X) * (B.Y - A.Y)) / len2;


            return Math.Abs(s) * Math.Sqrt(len2);
        }

        /// <summary>
        /// Computes the distance from a point to a sequence of line segments.
        /// </summary>
        /// <param name="p">A point</param>
        /// <param name="line">A sequence of contiguous line segments defined by their vertices</param>
        /// <returns>The minimum distance between the point and the line segments</returns>
        /// <exception cref="ArgumentException">If there are too few points to make up a line (at least one?)</exception>
        public static double DistancePointLine(Coordinate p, Coordinate[] line)
        {
            double minDistance = double.MaxValue;

            for (int i = 0; i < line.Length - 1; i++)
            {
                double d = CGAlgorithms.DistancePointLine(p, line[i], line[i + 1]);

                if (d < minDistance)
                {
                    minDistance = d;
                }
            }

            return minDistance;
        }


        /// <summary> 
        /// Computes the distance from a line segment AB to a line segment CD.
        /// Note: NON-ROBUST!
        /// </summary>
        /// <param name="A">A point of one line.</param>
        /// <param name="B">The second point of the line (must be different to A).</param>
        /// <param name="C">One point of the line.</param>
        /// <param name="D">Another point of the line (must be different to A).</param>
        /// <returns>The distance from line segment AB to line segment CD.</returns>
        public static double DistanceLineLine(Coordinate A, Coordinate B, Coordinate C, Coordinate D)
        {
            // check for zero-length segments

            if (A.IsEquivalent(B))
            {
                return DistancePointLine(A, C, D);
            }

            if (C.IsEquivalent(D))
            {
                return DistancePointLine(D, A, B);
            }

            // AB and CD are line segments
            /* from comp.graphics.algo
             *
	         *  Solving the above for r and s yields
             * 
             *     (Ay-Cy)(Dx-Cx)-(Ax-Cx)(Dy-Cy) 
             * r = ----------------------------- (eqn 1) 
             *     (Bx-Ax)(Dy-Cy)-(By-Ay)(Dx-Cx)
             * 
             *     (Ay-Cy)(Bx-Ax)-(Ax-Cx)(By-Ay)  
             * s = ----------------------------- (eqn 2)
             *     (Bx-Ax)(Dy-Cy)-(By-Ay)(Dx-Cx) 
             *     
             * Let P be the position vector of the
             * intersection point, then 
             *   P=A+r(B-A) or 
             *   Px=Ax+r(Bx-Ax) 
             *   Py=Ay+r(By-Ay) 
             * By examining the values of r & s, you can also determine some other limiting
             * conditions: 
             *   If 0<=r<=1 & 0<=s<=1, intersection exists 
             *      r<0 or r>1 or s<0 or s>1 line segments do not intersect 
             *   If the denominator in eqn 1 is zero, AB & CD are parallel 
             *   If the numerator in eqn 1 is also zero, AB & CD are collinear.
	         */

            var noIntersection = false;

            if (Envelope.Intersects(A, B, C, D))
            {
                var denom = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

                if (denom == 0)
                {
                    noIntersection = true;
                }
                else
                {
                    var r_num = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
                    var s_num = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);

                    var s = s_num / denom;
                    var r = r_num / denom;

                    if ((r < 0) || (r > 1) || (s < 0) || (s > 1))
                    {
                        noIntersection = true;
                    }
                }
            }
            else
            {
                noIntersection = true;
            }

            if (noIntersection)
            {
                return MathUtil.Min(DistancePointLine(A, C, D), DistancePointLine(B, C, D), DistancePointLine(C, A, B), DistancePointLine(D, A, B));
            }

            return 0.0;
        }

        public static double Area(IEnumerable<ICoordinate> ring)
        {
            return SignedArea(ring).Abs() / 2.0;
        }

        /// <summary>
        /// Computes the signed area for a ring.
        /// <remarks>
        /// <para>
        /// The signed area is
        /// </para>  
        /// <list type="Table">
        /// <item>positive</item><description>if the ring is oriented CW</description>
        /// <item>negative</item><description>if the ring is oriented CCW</description>
        /// <item>zero</item><description>if the ring is degenerate or flat</description>
        /// </list>
        /// </remarks>
        /// </summary>
        /// <param name="ring">The coordinates of the ring</param>
        /// <returns>The signed area of the ring</returns>
        public static double SignedArea(IEnumerable<ICoordinate> ring)
        {
            bool f0 = true, f1 = true;

            double lat, lon;
            double offset = 0.0, lonc = 0.0, latc = 0.0, latp = 0.0, area = 0.0;

            foreach (ICoordinate c in ring)
            {
                lat = c.Y;
                lon = c.X;

                if (f0)
                {
                    latp = lat;
                    offset = lon;
                    f0 = false;
                    continue;
                }

                if (f1)
                {
                    lonc = lon;
                    latc = lat;
                    f1 = false;
                    continue;
                }

                area += (lonc - offset) * (latp - c.Y);

                latp = latc;
                lonc = lon;
                latc = lat;
            }

            return area;
        }

        /// <summary>
        /// Computes the length of a linestring specified by a sequence of points.
        /// </summary>
        /// <param name="sequence">The points specifying the linestring</param>
        /// <returns>The length of the linestring</returns>
        public static double Length(ICoordinateCollection sequence)
        {
            return Enumerable.Range(0, sequence.Count - 1).Sum(i => sequence.Get(i + 1).Distance(sequence.Get(i)));
        }

        //public static double Length3d(ICoordinateCollection sequence)
        //{
        //    return Enumerable.Range(0, sequence.Count - 1).Sum(i => sequence[i + 1].Distance3d(sequence[i]));
        //}
    }
}
