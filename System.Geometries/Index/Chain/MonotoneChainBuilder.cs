using System.Collections.Generic;
using System.Geometries.Graph.Index;

namespace System.Geometries.Index.Chain
{
    internal class MonotoneChainBuilder
    {
        MonotoneChainBuilder()
        {
        }

        public static IList<MonotoneChain> GetChains(ICoordinateCollection sequence)
        {
            return GetChains(sequence, default(object));
        }

        public static IList<MonotoneChain> GetChains(ICoordinateCollection sequence, object context)
        {
            var list = new List<MonotoneChain>();
            int[] chainStartIndices = MonotoneChainIndexer.GetChainStartIndices(sequence);

            for (int i = 0; i < (chainStartIndices.Length - 1); i++)
            {
                MonotoneChain chain = new MonotoneChain(sequence, chainStartIndices[i], chainStartIndices[i + 1], context);
                list.Add(chain);
            }

            return list;
        }
    }
}
