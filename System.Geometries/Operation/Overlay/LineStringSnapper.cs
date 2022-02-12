namespace System.Geometries.Operation.Overlay
{
    internal class LineStringSnapper
    {
        /// <summary>
        /// Creates a new snapper using the points in the given <see cref="LineString"/>
        /// as target snap points.
        /// </summary>
        /// <param name="line">A LineString to snap (may be empty)</param>
        /// <param name="tolerance">the snap tolerance to use</param>
        public LineStringSnapper(LineString line, double tolerance) : this(line.Coordinates, tolerance) { }

        /// <summary>
        /// Creates a new snapper using the given points
        /// as source points to be snapped.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="snapTolerance"></param>
        public LineStringSnapper(ICoordinateCollection points, double snapTolerance)
        {
            Points = points;
            IsClosed = Points.IsClosed;
            Tolerance = snapTolerance;
        }

        readonly double Tolerance;
        readonly ICoordinateCollection Points;
        readonly LineSegment Segment = new LineSegment();
        readonly bool IsClosed;

        public bool AllowSnappingToSourceVertices;

        /// <summary>
        /// Snaps the vertices and segments of the source LineString
        /// to the given set of snap points.
        /// </summary>
        /// <param name="points">the vertices to snap to</param>
        /// <returns>list of the snapped points</returns>
        public ICoordinate[] SnapTo(ICoordinate[] points)
        {
            var coordList = new CoordinateList(Points.ToArray());

            SnapVertices(coordList, points);
            SnapSegments(coordList, points);

            return coordList.ToCoordinateArray();
        }

        /// <summary>
        /// Snap source vertices to vertices in the target.
        /// </summary>
        /// <param name="list">the points to snap</param>
        /// <param name="snaps">the points to snap to</param>
        void SnapVertices(CoordinateList list, ICoordinate[] snaps)
        {
            int end = IsClosed ? list.Count - 1 : list.Count;

            for (var i = 0; i < end; i++)
            {
                var r = FindSnapForVertex(list[i], snaps);

                if (r == null)
                {
                    continue;
                }

                list[i] = new Coordinate(r);

                if (i == 0 && IsClosed)
                {
                    list[list.Count - 1] = new Coordinate(r);
                }
            }
        }

        ICoordinate FindSnapForVertex(ICoordinate c, ICoordinate[] snaps)
        {
            foreach (Coordinate current in snaps)
            {
                if (c.IsEquivalent(current))
                {
                    return default;
                }

                if (c.Distance(current) < Tolerance)
                {
                    return current;
                }
            }

            return default;
        }

        /// <summary>
        /// Snap segments of the source to nearby snap vertices.
        /// <para/>Source segments are "cracked" at a snap vertex. A single input segment may be snapped several times to different snap vertices.<para/>
        /// For each distinct snap vertex, at most one source segment is snapped to.  
        /// This prevents "cracking" multiple segments at the same point, which would likely cause topology collapse when being used on polygonal linework.
        /// </summary>
        /// <param name="list">The coordinates of the source linestring to snap</param>
        /// <param name="snaps">The target snap vertices</param>
        void SnapSegments(CoordinateList list, ICoordinate[] snaps)
        {
            if (snaps.Length == 0)
            {
                return;
            }

            int count = snaps.Length;

            // Check for duplicate snap pts when they are sourced from a linear ring.
            // TODO: Need to do this better - need to check *all* points for dups (using a Set?)
            if (snaps[0].IsEquivalent(snaps[snaps.Length - 1]))
            {
                count = snaps.Length - 1;
            }

            for (var i = 0; i < count; i++)
            {
                ICoordinate snap = snaps[i];
                int index = FindSegmentIndexToSnap(snap, list);

                // If a segment to snap to was found, "crack" it at the snap pt.
                // The new pt is inserted immediately into the src segment list,
                // so that subsequent snapping will take place on the modified segments.
                // Duplicate points are not added.

                if (index >= 0)
                {
                    list.Add(index + 1, new Coordinate(snap), false);
                }
            }
        }

        /// <summary>
        /// Finds a src segment which snaps to (is close to) the given snap point
        /// <para/>Only a single segment is selected for snapping. 
        /// This prevents multiple segments snapping to the same snap vertex, which would almost certainly cause invalid geometry to be created.
        /// (The heuristic approach of snapping used here is really only appropriate when snap pts snap to a unique spot on the src geometry)<para/>
        /// Also, if the snap vertex occurs as a vertex in the src coordinate list, no snapping is performed.
        /// </summary>
        /// <param name="snapPt">The point to snap to</param>
        /// <param name="srcCoords">The source segment coordinates</param>
        /// <returns>The index of the snapped segment <br/>or -1 if no segment snaps to the snap point.</returns>
        int FindSegmentIndexToSnap(ICoordinate snapPt, CoordinateList srcCoords)
        {
            int index = -1;
            double min = double.MaxValue;

            for (var i = 0; i < srcCoords.Count - 1; i++)
            {
                Segment.P0 = srcCoords[i];
                Segment.P1 = srcCoords[i + 1];

                // Check if the snap pt is equal to one of the segment endpoints.
                // If the snap pt is already in the src list, don't snap at all.
                
                if (Segment.P0.IsEquivalent(snapPt) || Segment.P1.IsEquivalent(snapPt))
                {
                    if (AllowSnappingToSourceVertices)
                    {
                        continue;
                    }

                    return -1;
                }

                double d = Segment.Distance(snapPt);
                
                if (d < Tolerance && d < min)
                {
                    min = d;
                    index = i;
                }
            }

            return index;
        }
    }
}
