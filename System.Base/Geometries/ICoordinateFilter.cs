namespace System.Geometries
{
    public interface ICoordinateFilter
    {
        /// <summary>
	    /// Performs an operation with or on <c>coord</c>.
    	/// </summary>
        /// <param name="c"><c>Coordinate</c> to which the filter is applied.</param>
    	void Apply(ICoordinate c);
    }

}
