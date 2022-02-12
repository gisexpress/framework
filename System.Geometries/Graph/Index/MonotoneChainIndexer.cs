using System.Collections.Generic;

namespace System.Geometries.Graph.Index
{
    internal class MonotoneChainIndexer
    {
        public static int[] GetChainStartIndices(ICoordinateCollection coordinates)
        {
            int index = 0;
            int numPoints = coordinates.Count - 1;

            ICoordinate p0, p1;

            var chainQuad = default(int?);
            var indexes = new List<int> { 0 };

            while (index < numPoints)
            {
                p0 = coordinates.Get(index);
                p1 = coordinates.Get(index + 1);

                if (QuadrantOp.TryGetQuadrant(p1.X - p0.X, p1.Y - p0.Y, out int n))
                {
                    if (chainQuad.HasValue)
                    {
                        if (chainQuad.Value != n)
                        {
                            chainQuad = n;
                            indexes.Add(index);
                        }
                    }
                    else
                    {
                        chainQuad = n;
                    }
                }

                index++;
            }

            indexes.Add(index);

            return indexes.ToArray();
        }
    }
}