using System.Collections;
using System.Diagnostics;
using System.Geometries.Algorithm;
using System.Geometries.Graph;

namespace System.Geometries.Operation.Buffer
{
    /// <summary>
    /// A RightmostEdgeFinder find the DirectedEdge in a list which has the highest coordinate,
    /// and which is oriented L to R at that point. (I.e. the right side is on the RHS of the edge.)
    /// </summary>
    internal class RightmostEdgeFinder
    {
        DirectedEdge _minDe;
        int _minIndex = -1;

        public DirectedEdge Edge;
        public ICoordinate Coordinate;

        void CheckForRightmostCoordinate(DirectedEdge de)
        {
            ICoordinateCollection sequence = de.Edge.Sequence;

            for (int i = 0; i < (sequence.Count - 1); i++)
            {
                if (Coordinate == null || (sequence.Get(i).X > Coordinate.X))
                {
                    _minDe = de;
                    _minIndex = i;
                    Coordinate = sequence.Get(i);
                }
            }
        }

        public void FindEdge(IList dirEdgeList)
        {
            IEnumerator e = dirEdgeList.GetEnumerator();

            while (e.MoveNext())
            {
                var current = (DirectedEdge)e.Current;

                if (current != null && current.IsForward)
                {
                    CheckForRightmostCoordinate(current);
                }
            }

            Debug.Assert((_minIndex != 0) || Coordinate.IsEquivalent(_minDe.Coordinate), "inconsistency in rightmost processing");

            if (_minIndex == 0)
            {
                FindRightmostEdgeAtNode();
            }
            else
            {
                FindRightmostEdgeAtVertex();
            }

            Edge = _minDe;

            if (GetRightmostSide(_minDe, _minIndex) == Positions.Left)
            {
                Edge = _minDe.Directed;
            }
        }

        void FindRightmostEdgeAtNode()
        {
            _minDe = ((DirectedEdgeStar)_minDe.Node.Edges).GetRightmostEdge();

            if (!_minDe.IsForward)
            {
                _minDe = _minDe.Directed;
                _minIndex = _minDe.Edge.NumPoints - 1;
            }
        }

        void FindRightmostEdgeAtVertex()
        {
            Debug.Assert((_minIndex > 0) && (_minIndex < _minDe.Edge.Sequence.Count), "rightmost point expected to be interior vertex of edge");

            ICoordinate q = _minDe.Edge.Sequence.Get(_minIndex - 1);
            ICoordinate coordinate2 = _minDe.Edge.Sequence.Get(_minIndex + 1);

            int num = CGAlgorithms.ComputeOrientation(Coordinate, coordinate2, q);
            bool flag = false;

            if (((q.Y < Coordinate.Y) && (coordinate2.Y < Coordinate.Y)) && (num == 1))
            {
                flag = true;
            }
            else if (((q.Y > Coordinate.Y) && (coordinate2.Y > Coordinate.Y)) && (num == -1))
            {
                flag = true;
            }

            if (flag)
            {
                _minIndex--;
            }
        }

        Positions GetRightmostSide(DirectedEdge de, int index)
        {
            Positions rightmostSideOfSegment = GetRightmostSideOfSegment(de, index);

            if (rightmostSideOfSegment < Positions.On)
            {
                rightmostSideOfSegment = GetRightmostSideOfSegment(de, index - 1);
            }

            if (rightmostSideOfSegment < Positions.On)
            {
                Coordinate = null;
                CheckForRightmostCoordinate(de);
            }

            return rightmostSideOfSegment;
        }

        static Positions GetRightmostSideOfSegment(DirectedEdge de, int i)
        {
            ICoordinateCollection sequence = de.Edge.Sequence;

            if ((i < 0) || ((i + 1) >= sequence.Count))
            {
                return Positions.Parallel;
            }

            if (Equals(sequence.Get(i).Y, sequence.Get(i + 1).Y))
            {
                return Positions.Parallel;
            }

            var left = Positions.Left;

            if (sequence.Get(i).Y < sequence.Get(i + 1).Y)
            {
                left = Positions.Right;
            }

            return left;
        }
    }
}
