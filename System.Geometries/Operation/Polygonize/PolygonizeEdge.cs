using System.Geometries.Planargraph;

namespace System.Geometries.Operation.Polygonize
{
    /// <summary>
    /// An edge of a polygonization graph.
    /// </summary>
    internal class PolygonizeEdge : Edge
    {
        private readonly ILineString line;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        public PolygonizeEdge(ILineString line)
        {
            this.line = line;
        }

        /// <summary>
        /// 
        /// </summary>
        public ILineString Line
        {
            get
            {
                return line;
            }
        }
    }
}
