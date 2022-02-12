namespace System.Geometries
{
    /// <summary>
    /// The spatial functions supported by this class.
    /// These operations implement various bool combinations of the resultants of the overlay.
    /// </summary>
    public enum SpatialFunctions
    {
        /// <summary>
        /// The code for the Intersection overlay operation
        /// </summary>
        Intersection = 1,
        /// <summary>
        /// The code for the Union overlay operation
        /// </summary>
        Union = 2,
        /// <summary>
        /// The code for the Difference overlay operation
        /// </summary>
        Difference = 3,
        /// <summary>
        /// The code for the Symmetric Difference overlay operation
        /// </summary>
        SymDifference = 4,
    }
}
