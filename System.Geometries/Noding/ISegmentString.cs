namespace System.Geometries.Noding
{
    ///<summary>
    /// An interface for classes which represent a sequence of contiguous line segments.
    /// SegmentStrings can carry a context object, which is useful
    /// for preserving topological or parentage information.
    ///</summary>
    public interface ISegmentString
    {
        ICoordinateCollection Sequence
        {
            get;
        }

        ILineSegment this[int index]
        {
            get;
            set;
        }

        ///<summary>
        /// Gets/Sets the user-defined data for this segment string.
        ///</summary>
        object Context
        {
            get;
            set;
        }
    }
}