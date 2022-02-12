namespace System.Geometries
{
    public interface IIntersectable<T>
    {
        /// <summary>
        /// Predicate function to test if <paramref name="other"/> intersects with this object.
        /// </summary>
        /// <param name="other">The object to test</param>
        /// <returns><value>true</value> if this objects intersects with <paramref name="other"/></returns>
        bool Intersects(T other);
    }
}
