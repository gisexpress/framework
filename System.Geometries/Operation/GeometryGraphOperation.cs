using System.Geometries.Algorithm;
using System.Geometries.Graph;
using System.Linq;

namespace System.Geometries.Operation
{
    /// <summary>
    /// The base class for operations that require <see cref="GeometryGraph"/>s.
    /// </summary>
    internal abstract class GeometryGraphOperation
    {
        protected LineIntersector Intersector;

        /// <summary>
        /// The operation args into an array so they can be accessed by index.
        /// </summary>
        protected GeometryGraph[] Graphs;

        public static bool HasInvalid(IGeometry[] args)
        {
            return args.Any(e => e.IsValid() == false);
        }

        public static IGeometry[] MakeValid(IGeometry[] args)
        {
            return args.Select(e => e.MakeValid()).ToArray();
        }

        public IGeometry GetArgGeometry(int index)
        {
            return Graphs[index].Geometry;
        }

        public bool Compute(bool optimized, params IGeometry[] args)
        {
            OnInit(args);

            if (OnCompute(args))
            {
                return true;
            }

            if (optimized)
            {
                return false;
            }

            if (HasInvalid(args))
            {
                OnInit(args = MakeValid(args));

                if (OnCompute(args))
                {
                    return true;
                }
            }

            if (OnComputeClean(args))
            {
                return true;
            }

            return OnComputeFailed(args);
        }

        protected virtual void OnInit(IGeometry[] args)
        {
            Graphs = new GeometryGraph[2];
            Intersector = new RobustLineIntersector();

            Graphs[0] = new GeometryGraph(0, args[0]);
            Graphs[1] = new GeometryGraph(1, args[1]);
        }

        protected abstract bool OnCompute(params IGeometry[] args);

        protected virtual bool OnComputeClean(params IGeometry[] args)
        {
            return false;
        }

        protected virtual bool OnComputeFailed(params IGeometry[] args)
        {
            return false;
        }
    }
}
