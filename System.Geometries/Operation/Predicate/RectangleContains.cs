namespace System.Geometries.Operation.Predicate
{
    /// <summary>
    /// Optimized implementation of spatial predicate "contains"
    /// for cases where the first <c>Geometry</c> is a rectangle.    
    /// As a further optimization,
    /// this class can be used directly to test many geometries against a single rectangle.
    /// </summary>
    public class RectangleContains
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Contains(IPolygon rectangle, IGeometry b)
        {
            RectangleContains rc = new RectangleContains(rectangle);
            return rc.Contains(b);
        }

        private IPolygon rectangle;
        private readonly IEnvelope rectEnv;

        /// <summary>
        /// Create a new contains computer for two geometries.
        /// </summary>
        /// <param name="rectangle">A rectangular geometry.</param>
        public RectangleContains(IPolygon rectangle)
        {
            this.rectangle = rectangle;
            rectEnv = rectangle.GetBounds();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public bool Contains(IGeometry geom)
        {
            if (!rectEnv.Contains(geom.GetBounds()))
                return false;
            // check that geom is not contained entirely in the rectangle boundary
            if (IsContainedInBoundary(geom))
                return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        private bool IsContainedInBoundary(IGeometry geom)
        {
            // polygons can never be wholely contained in the boundary
            if (geom is IPolygon) 
                return false;
            if (geom is IPoint) 
                return IsPointContainedInBoundary((IPoint) geom);
            if (geom is ILineString) 
                return IsLineStringContainedInBoundary((ILineString) geom);

            for (int i = 0; i < geom.NumGeometries(); i++) 
            {
                IGeometry comp = geom.GetGeometryN(i);
                if (!IsContainedInBoundary(comp))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool IsPointContainedInBoundary(IPoint point)
        {
            return IsPointContainedInBoundary(point.Coordinate);
        }

        /// <summary>
        /// Tests if a point is contained in the boundary of the target rectangle.
        /// </summary>
        /// <param name="pt">the point to test</param>
        /// <returns>true if the point is contained in the boundary</returns>
        private bool IsPointContainedInBoundary(ICoordinate pt)
        {

            /**
             * contains = false iff the point is properly contained in the rectangle.
             * 
             * This code assumes that the point lies in the rectangle envelope
             */
            return pt.X == rectEnv.Min.X
                    || pt.X == rectEnv.Max.X
                    || pt.Y == rectEnv.Min.Y
                    || pt.Y == rectEnv.Max.Y;
        }

        /// <summary>
        /// Tests if a linestring is completely contained in the boundary of the target rectangle.
        /// </summary>
        /// <param name="line">the linestring to test</param>
        /// <returns>true if the linestring is contained in the boundary</returns>
        private bool IsLineStringContainedInBoundary(ILineString line)
        {
            ICoordinate p0, p1;
            ICoordinateCollection seq = line.Coordinates;

            for (int i = 0; i < seq.Count - 1; i++)
            {
                p0 = seq.Get(i);
                p1 = seq.Get(i + 1);

                if (!IsLineSegmentContainedInBoundary(p0, p1))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if a line segment is contained in the boundary of the target rectangle.
        /// </summary>
        /// <param name="p0">an endpoint of the segment</param>
        /// <param name="p1">an endpoint of the segment</param>
        /// <returns>true if the line segment is contained in the boundary</returns>
        private bool IsLineSegmentContainedInBoundary(ICoordinate p0, ICoordinate p1)
        {
            if (p0.Equals(p1))
                return IsPointContainedInBoundary(p0);
            // we already know that the segment is contained in the rectangle envelope
            if (p0.X == p1.X)
            {
                if (p0.X == rectEnv.Min.X || 
                    p0.X == rectEnv.Max.X)
                        return true;
            }
            else if (p0.Y == p1.Y)
            {
                if (p0.Y == rectEnv.Min.Y || 
                    p0.Y == rectEnv.Max.Y)
                        return true;
            }
            /*
             * Either both x and y values are different
             * or one of x and y are the same, but the other ordinate is not the same as a boundary ordinate
             * In either case, the segment is not wholely in the boundary
             */
            return false;         
        }
    }
}
