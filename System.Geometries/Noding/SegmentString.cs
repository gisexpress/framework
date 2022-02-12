using System.Collections.Generic;
using System.Geometries.Algorithm;

namespace System.Geometries.Noding
{
    internal class NodedSegmentString : INodableSegmentString
    {
        /// <summary>
        /// Gets the <see cref="ISegmentString"/>s which result from splitting this string at node points.
        /// </summary>
        /// <param name="segStrings">A collection of NodedSegmentStrings</param>
        /// <returns>A collection of NodedSegmentStrings representing the substrings</returns>
        public static IList<ISegmentString> GetNodedSubstrings(IList<ISegmentString> segStrings)
        {
            IList<ISegmentString> resultEdgelist = new List<ISegmentString>();
            GetNodedSubstrings(segStrings, resultEdgelist);
            return resultEdgelist;
        }

        /// <summary>
        /// Adds the noded <see cref="ISegmentString"/>s which result from splitting this string at node points.
        /// </summary>
        /// <param name="segStrings">A collection of NodedSegmentStrings</param>
        /// <param name="resultEdgelist">A list which will collect the NodedSegmentStrings representing the substrings</param>
        public static void GetNodedSubstrings(IList<ISegmentString> segStrings, IList<ISegmentString> resultEdgelist)
        {
            foreach (var obj in segStrings)
            {
                var ss = (NodedSegmentString)obj;
                ss.NodeList.AddSplitEdges(resultEdgelist);
            }
        }

        /// <summary>
        /// Creates a new segment string from a list of vertices.
        /// </summary>
        /// <param name="pts">The vertices of the segment string.</param>
        /// <param name="data">The user-defined data of this segment string (may be null).</param>
        public NodedSegmentString(ICoordinateCollection sequence, Object data)
        {
            Context = data;
            Sequence = sequence;
            NodeList = new SegmentNodeList(this);
        }

        public readonly ICoordinateCollection Sequence;

        ICoordinateCollection ISegmentString.Sequence
        {
            get { return Sequence; }
        }

        /// <summary>
        /// Gets/Sets the user-defined data for this segment string.
        /// </summary>
        public object Context
        {
            get;
            set;
        }

        public SegmentNodeList NodeList
        {
            get;
            protected set;
        }

        /// <summary>
        ///  Gets the octant of the segment starting at vertex <c>index</c>.
        /// </summary>
        /// <param name="index">
        /// The index of the vertex starting the segment.  
        /// Must not be the last index in the vertex list
        /// </param>
        /// <returns>The octant of the segment at the vertex</returns>
        public Octants GetSegmentOctant(int index)
        {
            return index == Sequence.Count - 1 ? Octants.Null : SafeOctant(Sequence.Get(index), Sequence.Get(index + 1));
        }

        static Octants SafeOctant(ICoordinate p0, ICoordinate p1)
        {
            if (p0.IsEquivalent(p1))
            {
                return Octants.Zero;
            }

            return Octant.GetOctant(p0, p1);
        }


        /// <summary>
        /// Adds EdgeIntersections for one or both
        /// intersections found for a segment of an edge to the edge intersection list.   
        /// </summary>
        /// <param name="li"></param>
        /// <param name="segmentIndex"></param>
        /// <param name="geomIndex"></param>
        public void AddIntersections(LineIntersector li, int segmentIndex, int geomIndex)
        {
            for (var i = 0; i < li.IntersectionNum; i++)
            {
                AddIntersection(li, segmentIndex, geomIndex, i);
            }
        }

        /// <summary>
        /// Add an <see cref="SegmentNode" /> for intersection intIndex.
        /// An intersection that falls exactly on a vertex
        /// of the <see cref="NodedSegmentString" /> is normalized
        /// to use the higher of the two possible segmentIndexes.
        /// </summary>
        /// <param name="li"></param>
        /// <param name="segmentIndex"></param>
        /// <param name="geomIndex"></param>
        /// <param name="intIndex"></param>
        public void AddIntersection(LineIntersector li, int segmentIndex, int geomIndex, int intIndex)
        {
            Coordinate intersection = new Coordinate(li.GetIntersection(intIndex));
            AddIntersection(intersection, segmentIndex);
        }

        public void AddIntersection(ICoordinate intersection, int segmentIndex)
        {
            var normalizedSegmentIndex = segmentIndex;
            // normalize the intersection point location
            var nextSegIndex = normalizedSegmentIndex + 1;

            if (nextSegIndex < Sequence.Count)
            {
                ICoordinate next = Sequence.Get(nextSegIndex);

                // Normalize segment index if intPt falls on vertex
                // The check for point equality is 2D only - Z values are ignored
                if (intersection.IsEquivalent(next))
                    normalizedSegmentIndex = nextSegIndex;
            }

            // Add the intersection point to edge intersection list.
            /*var ei = */
            NodeList.Add(intersection, normalizedSegmentIndex);
        }

        public ILineSegment this[int index]
        {
            get
            {
                if (index < 0 || index >= Sequence.Count)
                {
                    throw new ArgumentOutOfRangeException("index", index, "Parameter must be greater than or equal to 0 and less than TotalItemCount.");
                }

                return new LineSegment(Sequence.Get(index), Sequence.Get(index + 1));
            }
            set
            {
                throw new NotSupportedException("Setting line segments in a ISegmentString not supported.");
            }
        }
    }
}
