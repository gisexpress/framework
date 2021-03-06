namespace System.Geometries
{
    public interface IExpandable<T>
    {
        /// <summary>
        /// Method to expand this object by <paramref name="other"/>
        /// </summary>
        /// <param name="other">The object to expand with</param>
        void ExpandToInclude(T other);

        /// <summary>
        /// Function to expand compute a new object that is this object by expanded by <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The object to expand with</param>
        /// <returns>The expanded object</returns>
        T ExpandedBy(T other);
    }
}
