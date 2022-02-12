﻿namespace System.Geometries.Algorithm
{
    /// <summary>
    /// Computes whether a rectangle intersects line segments.
    /// </summary>
    /// <remarks>
    /// Rectangles contain a large amount of inherent symmetry
    /// (or to put it another way, although they contain four
    /// coordinates they only actually contain 4 ordinates
    /// worth of information).
    /// The algorithm used takes advantage of the symmetry of 
    /// the geometric situation 
    /// to optimize performance by minimizing the number
    /// of line intersection tests.
    /// </remarks>
    /// <author>Martin Davis</author>
    internal class RectangleLineIntersector
    {
        public RectangleLineIntersector(IEnvelope rect)
        {
            Rect = rect;
            Intersector = new RobustLineIntersector();
            // Up and Down are the diagonal orientations
            // relative to the Left side of the rectangle.
            // Index 0 is the left side, 1 is the right side.
            Up0 = new Coordinate(rect.Min.X, rect.Min.Y);
            Up1 = new Coordinate(rect.Max.X, rect.Max.Y);
            Down0 = new Coordinate(rect.Min.X, rect.Max.Y);
            Down1 = new Coordinate(rect.Max.X, rect.Min.Y);
        }

        readonly IEnvelope Rect;
        readonly LineIntersector Intersector;

        readonly ICoordinate Up0;
        readonly ICoordinate Up1;
        readonly ICoordinate Down0;
        readonly ICoordinate Down1;

        /// <summary>
        /// Tests whether the query rectangle intersects a given line segment.
        /// </summary>
        /// <param name="p0">The first endpoint of the segment</param>
        /// <param name="p1">The second endpoint of the segment</param>
        /// <returns><c>true</c> if the rectangle intersects the segment</returns>
        public bool Intersects(ICoordinate p0, ICoordinate p1)
        {
            // TODO: confirm that checking envelopes first is faster

            // If the segment envelope is disjoint from the rectangle envelope, there is no intersection
            if (Rect.Intersects(p0, p1))
            {
                // If either segment endpoint lies in the rectangle, there is an intersection.
                if (Rect.Intersects(p0)) return true;
                if (Rect.Intersects(p1)) return true;

                // Normalize segment.
                // This makes p0 less than p1, so that the segment runs to the right, or vertically upwards.
                if (p0.CompareTo(p1) > 0)
                {
                    var tmp = p0;
                    p0 = p1;
                    p1 = tmp;
                }

                // Compute angle of segment.
                // Since the segment is normalized to run left to right, it is sufficient to simply test the Y ordinate.
                // "Upwards" means relative to the left end of the segment.
                var isSegUpwards = p1.Y > p0.Y;

                // Since we now know that neither segment endpoint lies in the rectangle, there are two possible situations:
                // 1) the segment is disjoint to the rectangle
                // 2) the segment crosses the rectangle completely.
                // 
                // In the case of a crossing, the segment must intersect a diagonal of the rectangle.
                // 
                // To distinguish these two cases, it is sufficient to test intersection with a single diagonal of the rectangle,
                // namely the one with slope "opposite" to the slope of the segment.
                // (Note that if the segment is axis-parallel, it must intersect both diagonals, so this is still sufficient.)  
                
                if (isSegUpwards)
                {
                    Intersector.ComputeIntersection(p0, p1, Down0, Down1);
                }
                else
                {
                    Intersector.ComputeIntersection(p0, p1, Up0, Up1);
                }

                if (Intersector.HasIntersection)
                {
                    return true;
                }
            }

            return false;
        }
    }
}