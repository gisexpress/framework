namespace System.Geometries.Index.Strtree
{
    /// <summary>
    /// An <see cref="IItemDistance{Envelope, IGeometry}"/> function for
    /// items which are <see cref="IGeometry"/> using the <see cref="IGeometry.Distance(IGeometry)"/> method.
    /// </summary>
    /// <author>Martin Davis</author>
    internal class GeometryItemDistance : IItemDistance<IEnvelope, IGeometry>
    {
        /// <summary>
        /// Computes the distance between two <see cref="IGeometry"/> items, 
        /// using the <see cref="IGeometry.Distance(IGeometry)"/> method.
        /// </summary>
        /// <param name="item1">An item which is a geometry.</param>
        /// <param name="item2">An item which is a geometry.</param>
        /// <exception cref="InvalidCastException">if either item is not a Geometry</exception>
        /// <returns>The distance between the two items.</returns>
        public double Distance(IBoundable<IEnvelope, IGeometry> item1, IBoundable<IEnvelope, IGeometry> item2)
        {
            return item1.Item.GetDistance(item2.Item);
        }
    }
}