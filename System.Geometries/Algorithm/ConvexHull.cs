using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Geometries.Algorithm
{
    /// <summary> 
    /// Computes the convex hull of a <see cref="Geometry" />.
    /// The convex hull is the smallest convex Geometry that contains all the
    /// points in the input Geometry.
    /// Uses the Graham Scan algorithm.
    /// </summary>
    public class ConvexHull
    {
        public ConvexHull(IGeometry g)
            : this(ExtractCoordinates(g))
        {
            Factory = g.Factory;
        }

        ConvexHull(ICoordinate[] points)
        {
            InputPoints = points;
        }

        protected readonly ICoordinate[] InputPoints;
        readonly IGeometryFactory Factory;

        static ICoordinate[] CleanRing(ICoordinate[] original)
        {
            var list = new List<ICoordinate>();

            ICoordinate c = null;

            for (int i = 0; i <= (original.Length - 2); i++)
            {
                ICoordinate c0 = original[i];
                ICoordinate c1 = original[i + 1];

                if (!c0.IsEquivalent(c1) && ((c == null) || !IsBetween(c, c0, c1)))
                {
                    list.Add(c0);
                    c = c0;
                }
            }

            list.Add(original[original.Length - 1]);
            return list.ToArray();
        }

        static ICoordinate[] ComputeOctPts(ICoordinate[] inputPts)
        {
            var coordinateArray = new ICoordinate[8];

            for (int i = 0; i < coordinateArray.Length; i++)
            {
                coordinateArray[i] = inputPts[0];
            }

            for (int j = 1; j < inputPts.Length; j++)
            {
                if (inputPts[j].X < coordinateArray[0].X)
                {
                    coordinateArray[0] = inputPts[j];
                }
                if ((inputPts[j].X - inputPts[j].Y) < (coordinateArray[1].X - coordinateArray[1].Y))
                {
                    coordinateArray[1] = inputPts[j];
                }
                if (inputPts[j].Y > coordinateArray[2].Y)
                {
                    coordinateArray[2] = inputPts[j];
                }
                if ((inputPts[j].X + inputPts[j].Y) > (coordinateArray[3].X + coordinateArray[3].Y))
                {
                    coordinateArray[3] = inputPts[j];
                }
                if (inputPts[j].X > coordinateArray[4].X)
                {
                    coordinateArray[4] = inputPts[j];
                }
                if ((inputPts[j].X - inputPts[j].Y) > (coordinateArray[5].X - coordinateArray[5].Y))
                {
                    coordinateArray[5] = inputPts[j];
                }
                if (inputPts[j].Y < coordinateArray[6].Y)
                {
                    coordinateArray[6] = inputPts[j];
                }
                if ((inputPts[j].X + inputPts[j].Y) < (coordinateArray[7].X + coordinateArray[7].Y))
                {
                    coordinateArray[7] = inputPts[j];
                }
            }

            return coordinateArray;
        }

        static ICoordinate[] ComputeOctRing(ICoordinate[] inputPts)
        {
            ICoordinate[] coord = ComputeOctPts(inputPts);
            var list = new CoordinateList(coord, false);

            if (list.Count < 3)
            {
                return null;
            }

            list.CloseRing();

            return list.ToCoordinateArray();
        }

        static ICoordinate[] ExtractCoordinates(IGeometry g)
        {
            return Enumerable.Distinct(g.Coordinates).ToArray();
        }

        public virtual IGeometry GetConvexHull()
        {
            if (InputPoints.Length == 0)
            {
                return Factory.Create<IGeometryCollection>();
            }

            if (InputPoints.Length == 1)
            {
                return Factory.Create<IPoint>(InputPoints[0]);
            }

            if (InputPoints.Length == 2)
            {
                return Factory.Create<ILineString>(InputPoints);
            }

            ICoordinate[] inputPts = InputPoints;

            if (InputPoints.Length > 50)
            {
                inputPts = Reduce();
            }

            ICoordinate[] c = PreSort(inputPts);
            ICoordinate[] coordinates = GrahamScan(c).ToArray();

            return LineOrPolygon(coordinates);
        }

        static Stack<ICoordinate> GrahamScan(ICoordinate[] c)
        {
            var stack = new Stack<ICoordinate>(c.Length);

            stack.Push(c[0]);
            stack.Push(c[1]);
            stack.Push(c[2]);

            for (int i = 3; i < c.Length; i++)
            {
                ICoordinate coordinate = stack.Pop();

                while (CGAlgorithms.ComputeOrientation(stack.Peek(), coordinate, c[i]) > 0)
                {
                    coordinate = stack.Pop();
                }

                stack.Push(coordinate);
                stack.Push(c[i]);
            }

            stack.Push(c[0]);

            return stack;
        }

        static bool IsBetween(ICoordinate c1, ICoordinate c2, ICoordinate c3)
        {
            if (CGAlgorithms.ComputeOrientation(c1, c2, c3) == 0)
            {
                if (!double.Equals(c1.X, c3.X))
                {
                    if ((c1.X <= c2.X) && (c2.X <= c3.X))
                    {
                        return true;
                    }
                    if ((c3.X <= c2.X) && (c2.X <= c1.X))
                    {
                        return true;
                    }
                }

                if (!double.Equals(c1.Y, c3.Y))
                {
                    if ((c1.Y <= c2.Y) && (c2.Y <= c3.Y))
                    {
                        return true;
                    }
                    if ((c3.Y <= c2.Y) && (c2.Y <= c1.Y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        IGeometry LineOrPolygon(ICoordinate[] coordinates)
        {
            coordinates = CleanRing(coordinates);

            if (coordinates.Length == 3)
            {
                var line = Factory.Create<ILineString>();
                line.Coordinates.Add(coordinates.Take(2));
                return line;
            }

            var poly = Factory.Create<IPolygon>();
            poly.Coordinates.Add(coordinates);
            return poly;
        }

        static ICoordinate[] PreSort(ICoordinate[] pts)
        {
            for (int i = 1; i < pts.Length; i++)
            {
                if ((pts[i].Y < pts[0].Y) || (double.Equals(pts[i].Y, pts[0].Y) && (pts[i].X < pts[0].X)))
                {
                    ICoordinate coordinate = pts[0];
                    pts[0] = pts[i];
                    pts[i] = coordinate;
                }
            }

            Array.Sort(pts, 1, pts.Length - 1, new RadialComparator(pts[0]));

            return pts;
        }

        ICoordinate[] Reduce()
        {
            ICoordinate[] ring = ComputeOctRing(InputPoints);

            if (ring == null)
            {
                return InputPoints;
            }

            var set = new SortedSet<ICoordinate>();

            for (int i = 0; i < ring.Length; i++)
            {
                set.Add(ring[i]);
            }

            for (int j = 0; j < InputPoints.Length; j++)
            {
                if (!CGAlgorithms.IsPointInRing(InputPoints[j], ring))
                {
                    set.Add(InputPoints[j]);
                }
            }

            var array = new Coordinate[set.Count];
            set.CopyTo(array, 0);

            return array;
        }

        class RadialComparator : IComparer<ICoordinate>
        {
            readonly ICoordinate _origin;

            public RadialComparator(ICoordinate origin)
            {
                _origin = origin;
            }

            public int Compare(ICoordinate p1, ICoordinate p2)
            {
                return PolarCompare(_origin, p1, p2);
            }

            static int PolarCompare(ICoordinate o, ICoordinate p, ICoordinate q)
            {
                double num = p.X - o.X;
                double num2 = p.Y - o.Y;
                double num3 = q.X - o.X;
                double num4 = q.Y - o.Y;

                switch (CGAlgorithms.ComputeOrientation(o, p, q))
                {
                    case 1:
                        return 1;

                    case -1:
                        return -1;
                }

                double num6 = (num * num) + (num2 * num2);
                double num7 = (num3 * num3) + (num4 * num4);

                if (num6 < num7)
                {
                    return -1;
                }

                if (num6 > num7)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
