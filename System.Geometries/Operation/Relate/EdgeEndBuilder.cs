using System.Collections.Generic;
using System.Geometries.Graph;

namespace System.Geometries.Operation.Relate
{
    internal class EdgeEndBuilder
    {
        public IList<EdgeEnd> ComputeEdgeEnds(IEnumerable<Edge> edges)
        {
            var list = new List<EdgeEnd>();

            foreach (Edge e in edges)
            {
                ComputeEdgeEnds(e, list);
            }

            return list;
        }

        /// <summary>
        /// Creates stub edges for all the intersections in this Edge (if any) and inserts them into the graph.
        /// </summary>
        public void ComputeEdgeEnds(Edge edge, IList<EdgeEnd> list)
        {
            EdgeIntersectionList eiList = edge.EdgeIntersectionList;

            // Ensure that the list has entries for the first and last point of the edge
            eiList.AddEndpoints();

            EdgeIntersection prev;
            EdgeIntersection current = null;
            IEnumerator<EdgeIntersection> e = eiList.GetEnumerator();

            // No intersections, so there is nothing to do
            if (e.MoveNext() == false)
            {
                return;
            }

            EdgeIntersection next = e.Current;

            while (true)
            {
                prev = current;
                current = next;
                next = null;

                if (e.MoveNext())
                {
                    next = e.Current;
                }

                if (current == null)
                {
                    break;
                }

                CreateEdgeEndForPrev(edge, list, current, prev);
                CreateEdgeEndForNext(edge, list, current, next);
            }
        }

        /// <summary>
        /// Create a EdgeStub for the edge before the intersection eiCurr.
        /// The previous intersection is provided
        /// in case it is the endpoint for the stub edge.
        /// Otherwise, the previous point from the parent edge will be the endpoint.
        /// eiCurr will always be an EdgeIntersection, but eiPrev may be null.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="l"></param>
        /// <param name="eiCurr"></param>
        /// <param name="eiPrev"></param>
        public void CreateEdgeEndForPrev(Edge edge, IList<EdgeEnd> l, EdgeIntersection eiCurr, EdgeIntersection eiPrev)
        {
            int iPrev = eiCurr.SegmentIndex;

            if (eiCurr.Distance == 0.0)
            {
                // if at the start of the edge there is no previous edge
                if (iPrev == 0)
                {
                    return;
                }

                iPrev--;
            }

            ICoordinate pPrev = edge.Sequence.Get(iPrev);

            // If prev intersection is past the previous vertex, use it instead
            if (eiPrev != null && eiPrev.SegmentIndex >= iPrev)
            {
                pPrev = eiPrev.Coordinate;
            }

            Label label = new Label(edge.Label);
            // since edgeStub is oriented opposite to it's parent edge, have to flip sides for edge label
            label.Flip();

            var e = new EdgeEnd(edge, label);

            if (e.Init(eiCurr.Coordinate, pPrev))
            {
                l.Add(e);
            }
        }

        /// <summary>
        /// Create a StubEdge for the edge after the intersection eiCurr.
        /// The next intersection is provided
        /// in case it is the endpoint for the stub edge.
        /// Otherwise, the next point from the parent edge will be the endpoint.
        /// eiCurr will always be an EdgeIntersection, but eiNext may be null.
        /// </summary>
        public void CreateEdgeEndForNext(Edge edge, IList<EdgeEnd> l, EdgeIntersection eiCurr, EdgeIntersection eiNext)
        {
            int iNext = eiCurr.SegmentIndex + 1;

            // if there is no next edge there is nothing to do            
            if (iNext >= edge.NumPoints && eiNext == null)
            {
                return;
            }

            ICoordinate pNext = edge.Sequence.Get(iNext);

            // if the next intersection is in the same segment as the current, use it as the endpoint
            if (eiNext != null && eiNext.SegmentIndex == eiCurr.SegmentIndex)
            {
                pNext = eiNext.Coordinate;
            }

            EdgeEnd e = new EdgeEnd(edge, new Label(edge.Label));

            if (e.Init(eiCurr.Coordinate, pNext))
            {
                l.Add(e);
            }
        }
    }
}
