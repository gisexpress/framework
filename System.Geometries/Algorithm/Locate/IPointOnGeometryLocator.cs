namespace System.Geometries.Algorithm.Locate
{
    ///<summary>
    /// An interface for classes which determine the <see cref="Locations"/> of
    /// points in areal geometries.
    /// </summary>
    /// <author>Martin Davis</author>
    public interface IPointOnGeometryLocator
    {
        ///<summary>
        /// Determines the <see cref="Locations"/> of a point in an areal <see cref="IGeometry"/>.
        ///</summary>
        ///<param name="p">The point to test</param>
        ///<returns>The location of the point in the geometry</returns>
        Locations Locate(ICoordinate p);
    }

    /// <summary>
    /// Static methods for <see cref="IPointOnGeometryLocator"/> classes
    /// </summary>
    internal static class PointOnGeometryLocatorExtensions
    {
        /// <summary> 
        /// Convenience method to test a point for intersection with a geometry
        /// <para/>
        /// The geometry is wrapped in a <see cref="IPointOnGeometryLocator"/> class.
        /// </summary>
        /// <param name="locator">The locator to use.</param>
        /// <param name="coordinate">The coordinate to test.</param>
        /// <returns><c>true</c> if the point is in the interior or boundary of the geometry.</returns>
        public static bool Intersects(IPointOnGeometryLocator locator, Coordinate coordinate)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (coordinate == null)
                throw new ArgumentNullException("coordinate");

            switch (locator.Locate(coordinate))
            {
                case Locations.Boundary:
                case Locations.Interior:
                    return true;

                case Locations.Exterior:
                    return false;

                default:
                    throw new InvalidOperationException("IPointOnGeometryLocator.Locate should never return anything other than Boundary, Interior, or Exterior.");
            }
        }
    }
}