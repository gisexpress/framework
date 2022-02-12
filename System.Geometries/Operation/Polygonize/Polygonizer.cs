using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Geometries.Operation.Polygonize
{
    /// <summary>
    /// Polygonizes a set of <see cref="IGeometry"/>s which contain linework that
    /// represents the edges of a planar graph.
    /// </summary>
    /// <remarks>
    /// <para>All types of Geometry are accepted as input;
    /// the constituent linework is extracted as the edges to be polygonized.
    /// The processed edges must be correctly noded; that is, they must only meet
    /// at their endpoints. Polygonization will accept incorrectly noded input
    /// but will not form polygons from non-noded edges, 
    /// and reports them as errors.
    /// </para><para>
    /// The Polygonizer reports the follow kinds of errors:
    /// Dangles - edges which have one or both ends which are not incident on another edge endpoint
    /// Cut Edges - edges which are connected at both ends but which do not form part of polygon
    /// Invalid Ring Lines - edges which form rings which are invalid
    /// (e.g. the component lines contain a self-intersection).</para>
    /// <para>
    /// Polygonization supports extracting only polygons which form a valid polygonal geometry.
    /// The set of extracted polygons is guaranteed to be edge-disjoint.
    /// This is useful for situations where it is known that the input lines form a
    /// valid polygonal geometry.</para>
    /// </remarks>
    /// 
    public class Polygonizer : IPolygonizer
    {
        /// <summary>
        /// The default polygonizer output behavior
        /// </summary>
        public const bool AllPolys = false;

        public Polygonizer()
            : this(AllPolys)
        {
            LineAdder = new LineStringAdder(this);
        }

        /// <summary>
        /// Creates a polygonizer and allow specifyng if only polygons which form a valid polygonal geometry are to be extracted.
        /// </summary>
        /// <param name="onlyPolygonal"><value>true</value> if only polygons which form a valid polygonal geometry are to be extracted</param>
        public Polygonizer(bool onlyPolygonal)
        {
            OnlyPolygonal = onlyPolygonal;
            LineAdder = new LineStringAdder(this);
        }

        /// <summary>
        /// Default linestring adder.
        /// </summary>
        readonly LineStringAdder LineAdder;
        PolygonizeGraph Graph;

        // Initialized with empty collections, in case nothing is computed
        ICollection<ILineString> Dangles = new List<ILineString>();
        ICollection<ILineString> CutEdges = new List<ILineString>();
        IList<IGeometry> InvalidRingLines = new List<IGeometry>();
        List<EdgeRing> Holes;
        List<EdgeRing> Shells;
        ICollection<IPolygon> Polies;

        bool RingValidation = true;
        readonly bool OnlyPolygonal;

        /// <summary>
        /// Allows disabling the valid ring checking, 
        /// to optimize situations where invalid rings are not expected.
        /// </summary>
        /// <remarks>The default is <c>true</c></remarks>
        public bool IsCheckingRingsValid
        {
            get { return RingValidation; }
            set { RingValidation = value; }
        }

        /// <summary>
        /// Adds a collection of <see cref="IGeometry"/>s to be polygonized.
        /// May be called multiple times.
        /// Any dimension of Geometry may be added;
        /// the constituent linework will be extracted and used.
        /// </summary>
        /// <param name="geomList">A list of <c>Geometry</c>s with linework to be polygonized.</param>
        public void Add(ICollection<IGeometry> geomList)
        {
            foreach (var geometry in geomList)
                Add(geometry);
        }

        /// <summary>
        /// Adds a <see cref="IGeometry"/> to the linework to be polygonized.
        /// May be called multiple times.
        /// Any dimension of Geometry may be added;
        /// the constituent linework will be extracted and used
        /// </summary>
        /// <param name="g">A <c>Geometry</c> with linework to be polygonized.</param>
        public void Add(IGeometry g)
        {
            Add(g as ILineString);
        }

        /// <summary>
        /// Adds a  to the graph of polygon edges.
        /// </summary>
        /// <param name="line">The <see cref="ILineString"/> to add.</param>
        void Add(ILineString line)
        {
            if (line == null)
            {
                return;
            }

            // record the geometry factory for later use
            // create a new graph using the factory from the input Geometry
            if (Graph == null)
                Graph = new PolygonizeGraph(line.Factory);

            Graph.AddEdge(line);
        }

        /// <summary>
        /// Gets the list of polygons formed by the polygonization.
        /// </summary>        
        public ICollection<IPolygon> GetPolygons()
        {
            Polygonize();
            return Polies;
        }

        /// <summary>
        /// Gets a geometry representing the polygons formed by the polygonization.
        /// If a valid polygonal geometry was extracted the result is a <see cref="IPolygonal"/> geometry. 
        /// </summary>
        /// <returns>A geometry containing the polygons</returns>
        public IGeometry GetGeometry()
        {
            Polygonize();

            if (OnlyPolygonal)
            {
                return Polies.First().Factory.BuildGeometry(Polies);
            }

            // result may not be valid Polygonal, so return as a GeometryCollection
            return Polies.First().Factory.Create<IGeometryCollection>(Polies);
        }

        /// <summary> 
        /// Gets the list of dangling lines found during polygonization.
        /// </summary>
        public ICollection<ILineString> GetDangles()
        {
            Polygonize();
            return Dangles;
        }

        /// <summary>
        /// Gets the list of cut edges found during polygonization.
        /// </summary>
        public ICollection<ILineString> GetCutEdges()
        {
            Polygonize();
            return CutEdges;
        }

        /// <summary>
        /// Gets the list of lines forming invalid rings found during polygonization.
        /// </summary>
        public IList<IGeometry> GetInvalidRingLines()
        {
            Polygonize();
            return InvalidRingLines;
        }

        /// <summary>
        /// Performs the polygonization, if it has not already been carried out.
        /// </summary>
        private void Polygonize()
        {
            // check if already computed
            if (Polies != null)
                return;

            Polies = new List<IPolygon>();

            // if no geometries were supplied it's possible that graph is null
            if (Graph == null)
                return;

            Dangles = Graph.DeleteDangles();
            CutEdges = Graph.DeleteCutEdges();
            var edgeRingList = Graph.GetEdgeRings();

            var validEdgeRingList = new List<EdgeRing>();
            InvalidRingLines = new List<IGeometry>();
            if (IsCheckingRingsValid)
                FindValidRings(edgeRingList, validEdgeRingList, InvalidRingLines);
            else validEdgeRingList = (List<EdgeRing>)edgeRingList;

            FindShellsAndHoles(validEdgeRingList);
            AssignHolesToShells(Holes, Shells);
            // order the shells to make any subsequent processing deterministic
            Shells.Sort(new EdgeRing.EnvelopeComparator());

            var includeAll = true;
            if (OnlyPolygonal)
            {
                FindDisjointShells(Shells);
                includeAll = false;
            }
            Polies = ExtractPolygons(Shells, includeAll);

        }

        private static void FindValidRings(IEnumerable<EdgeRing> edgeRingList, ICollection<EdgeRing> validEdgeRingList, ICollection<IGeometry> invalidRingList)
        {
            foreach (var er in edgeRingList)
            {
                if (er.IsValid)
                    validEdgeRingList.Add(er);
                else invalidRingList.Add(er.LineString);
            }
        }

        private void FindShellsAndHoles(IEnumerable<EdgeRing> edgeRingList)
        {
            Holes = new List<EdgeRing>();
            Shells = new List<EdgeRing>();
            foreach (var er in edgeRingList)
            {
                er.ComputeHole();
                if (er.IsHole)
                    Holes.Add(er);
                else Shells.Add(er);

            }
        }

        private static void AssignHolesToShells(IEnumerable<EdgeRing> holeList, List<EdgeRing> shellList)
        {
            foreach (EdgeRing holeEdgeRing in holeList)
            {
                AssignHoleToShell(holeEdgeRing, shellList);
                /*
                if (!holeER.hasShell()) {
                    System.out.println("DEBUG: Outer hole: " + holeER);
                }
                */
            }
        }

        private static void AssignHoleToShell(EdgeRing holeEdgeRing, IList<EdgeRing> shellList)
        {
            var shell = EdgeRing.FindEdgeRingContaining(holeEdgeRing, shellList);
            if (shell != null)
            {
                shell.AddHole(holeEdgeRing);
            }
        }

        private static void FindDisjointShells(List<EdgeRing> shellList)
        {
            FindOuterShells(shellList);

            bool isMoreToScan;
            do
            {
                isMoreToScan = false;
                foreach (var er in shellList)
                {
                    if (er.IsIncludedSet)
                        continue;
                    er.UpdateIncluded();
                    if (!er.IsIncludedSet)
                    {
                        isMoreToScan = true;
                    }
                }
            } while (isMoreToScan);
        }


        /// <summary>
        /// For each outer hole finds and includes a single outer shell.
        /// This seeds the travesal algorithm for finding only polygonal shells.
        /// </summary>
        /// <param name="shellList">The list of shell EdgeRings</param>
        private static void FindOuterShells(List<EdgeRing> shellList)
        {

            foreach (var er in shellList)
            {
                var outerHoleER = er.OuterHole;
                if (outerHoleER != null && !outerHoleER.IsProcessed)
                {
                    er.IsIncluded = true;
                    outerHoleER.IsProcessed = true;
                }
            }
        }

        private static List<IPolygon> ExtractPolygons(List<EdgeRing> shellList, bool includeAll)
        {
            var polyList = new List<IPolygon>();

            foreach (EdgeRing er in shellList)
            {
                if (includeAll || er.IsIncluded)
                {
                    polyList.Add(er.Polygon);
                }
            }
            return polyList;
        }

        /// <summary>
        /// Adds every linear element in a <see cref="IGeometry"/> into the polygonizer graph.
        /// </summary>
        class LineStringAdder : IGeometryComponentFilter
        {
            private readonly Polygonizer _container;

            public LineStringAdder(Polygonizer container)
            {
                _container = container;
            }

            /// <summary>
            /// Filters all <see cref="ILineString"/> geometry instances
            /// </summary>
            /// <param name="g">The geometry instance</param>
            public void Filter(IGeometry g)
            {
                var lineString = g as ILineString;
                if (lineString != null)
                    _container.Add(lineString);
            }
        }
    }
}
