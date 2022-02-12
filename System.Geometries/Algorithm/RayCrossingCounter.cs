using System.Collections.Generic;

namespace System.Geometries.Algorithm
{
    internal class RayCrossingCounter
    {
        public RayCrossingCounter(ICoordinate p)
        {
            P = p;
        }

        protected readonly ICoordinate P;
        protected int CrossingCount;
        protected bool IsPointOnSegment;

        /// <summary>
        /// Determines the <see cref="Location"/> of a point in a ring.
        /// </summary>
        /// <param name="p">The point to test</param>
        /// <param name="ring">A coordinate sequence forming a ring</param>
        /// <returns>The location of the point in the ring</returns>
        public static Locations LocatePointInRing(ICoordinate p, IEnumerable<ICoordinate> ring)
        {
            ICoordinate p1, p2;

            var counter = new RayCrossingCounter(p);
            IEnumerator<ICoordinate> e = ring.GetEnumerator();

            if (e.MoveNext())
            {
                p1 = e.Current;

                while (e.MoveNext())
                {
                    p2 = e.Current;

                    counter.CountSegment(p1, p2);

                    if (counter.IsOnSegment)
                    {
                        return counter.Location;
                    }

                    p1 = p2;
                }
            }

            return counter.Location;
        }

        ///<summary>
        /// Counts a segment
        ///</summary>
        /// <param name="p1">An endpoint of the segment</param>
        /// <param name="p2">Another endpoint of the segment</param>
        public void CountSegment(ICoordinate p1, ICoordinate p2)
        {
            // For each segment, check if it crosses a horizontal ray running from the test point in the positive x direction.
            // Check if the segment is strictly to the left of the test point
            if (p1.X < P.X && p2.X < P.X)
            {
                return;
            }

            // Check if the point is equal to the current ring vertex
            if (P.IsEquivalent(p2))
            {
                IsPointOnSegment = true;
                return;
            }

            // For horizontal segments, check if the point is on the segment. Otherwise, horizontal segments are not counted.
            if (P.IsEquivalent(p1))
            {
                double minx = p1.X;
                double maxx = p2.X;

                if (minx > maxx)
                {
                    minx = p2.X;
                    maxx = p1.X;
                }

                if (P.X >= minx && P.X <= maxx)
                {
                    IsPointOnSegment = true;
                }

                return;
            }

            // Evaluate all non-horizontal segments which cross a horizontal ray to the right of the test pt. 
            // To avoid double-counting shared vertices, we use the convention that
            //  - an upward edge includes its starting endpoint, and excludes its final endpoint
            //  - a downward edge excludes its starting endpoint, and includes its final endpoint

            if (((P.Y < p1.Y) && (P.Y >= p2.Y)) || ((P.Y < p2.Y) && (P.Y >= p1.Y)))
            {
                // Translate the segment so that the test point lies on the origin
                double x1 = p1.X - P.X;
                double y1 = p1.Y - P.Y;
                double x2 = p2.X - P.X;
                double y2 = p2.Y - P.Y;

                // The translated segment straddles the x-axis. 
                // Compute the sign of the ordinate of intersection with the x-axis. (y2 != y1, so denominator will never be 0.0)
                double sign = RobustDeterminant.SignOfDet2x2(x1, y1, x2, y2);

                if (sign.IsZero())
                {
                    IsPointOnSegment = true;
                    return;
                }

                if (y2 < y1)
                {
                    sign = -sign;
                }

                // The segment crosses the ray if the sign is strictly positive.
                if (sign > 0.0)
                {
                    CrossingCount++;
                }
            }
        }

        ///<summary>
        /// Reports whether the point lies exactly on one of the supplied segments.
        /// <remarks>
        /// This method may be called at any time as segments are processed. If the result of this method is <c>true</c>, 
        /// no further segments need be supplied, since the result will never change again.
        /// </remarks>
        ///</summary>
        public bool IsOnSegment
        {
            get { return IsPointOnSegment; }

        }

        ///<summary>
        /// Gets the <see cref="GeoAPI.Geometries.Location"/> of the point relative to  the ring, polygon
        /// or multipolygon from which the processed segments were provided.
        ///</summary>
        /// <remarks>This property only determines the correct location if <b>all</b> relevant segments have been processed</remarks>
        public Locations Location
        {
            get
            {
                if (IsPointOnSegment)
                {
                    return Locations.Boundary;
                }

                // The point is in the interior of the ring if the number of X-crossings is odd.
                if ((CrossingCount % 2) == 1)
                {
                    return Locations.Interior;
                }

                return Locations.Exterior;
            }
        }

        ///<summary>
        /// Tests whether the point lies in or on the ring, polygon
        ///</summary>
        ///<remarks>
        /// This property only determines the correct location if <b>all</b> relevant segments have been processed</remarks>
        public bool IsPointInPolygon
        {
            get { return Location != Locations.Exterior; }
        }
    }
}
