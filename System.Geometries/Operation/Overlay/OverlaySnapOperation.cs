using System.Geometries.Precision;

namespace System.Geometries.Operation.Overlay
{
    internal class OverlaySnapOperation : OverlayOperation
    {
        public OverlaySnapOperation(SpatialFunctions function)
            : base(function)
        {
        }

        double Tolerance;
        CommonBitsRemover Remover;

        public new static IGeometry Overlay(SpatialFunctions function, params IGeometry[] args)
        {
            return Overlay(function, false, args);
        }

        public new static IGeometry Overlay(SpatialFunctions function, bool optimized, params IGeometry[] args)
        {
            var o = new OverlaySnapOperation(function);

            if (o.Compute(optimized, args))
            {
                return o.Result;
            }

            return default(Geometry);
        }

        protected override void OnInit(IGeometry[] args)
        {
            Tolerance = GeometrySnapper.ComputeOverlaySnapTolerance(args[0], args[1]);
            base.OnInit(Snap(args));
        }

        protected override bool OnCompute(params IGeometry[] args)
        {
            if (base.OnCompute(args))
            {
                if (Result == null)
                {
                    return true;
                }

                Remover.AddCommonBits(Result);
                return true;
            }

            return false;
        }

        protected override bool OnComputeClean(params IGeometry[] args)
        {
            //TODO: clean
            //for (int i = 0; i < args.Length; i++)
            //{
            //    args[i] = args[i].Clean();
            //}

            OnInit(args);

            if (OnCompute(args))
            {
                return true;
            }

            return false;
        }

        protected override bool OnComputeFailed(params IGeometry[] args)
        {
            base.OnInit(args);

            if (base.OnCompute(args))
            {
                return true;
            }

            return false;
        }

        IGeometry[] Snap(IGeometry[] args)
        {
            IGeometry[] snaps = RemoveCommonBits(args);
            var r = new IGeometry[] { new GeometrySnapper(snaps[0]).SnapTo(snaps[1], Tolerance), default };

            if (r[0] == null)
            {
                return r;
            }

            // Snap the second geometry to the snapped first geometry
            // (this strategy minimizes the number of possible different points in the result)
            r[1] = new GeometrySnapper(snaps[1]).SnapTo(r[0], Tolerance);
            return r;
        }

        IGeometry[] RemoveCommonBits(IGeometry[] args)
        {
            var results = new IGeometry[args.Length];

            Remover = new CommonBitsRemover();

            for (int i = 0; i < results.Length; i++)
            {
                Remover.Add(results[i] = args[i].Clone());
            }

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = Remover.RemoveCommonBits(results[i]);
            }

            return results;
        }
    }
}
