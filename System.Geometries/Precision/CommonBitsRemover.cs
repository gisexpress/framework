namespace System.Geometries.Precision
{
    internal class CommonBitsRemover
    {
        public CommonBitsRemover()
        {
            Filter = new CommonCoordinateFilter();
        }

        ICoordinate Common;
        readonly CommonCoordinateFilter Filter;

        public virtual ICoordinate CommonCoordinate
        {
            get { return Common; }
        }

        public virtual void Add(IGeometry g)
        {
            g.GetCoordinates().ForEach(e => Filter.Apply(e));
            Common = Filter.CommonCoordinate;
        }

        public virtual void AddCommonBits(IGeometry g)
        {
            var translater = new Translater(Common);
            g.GetCoordinates().ForEach(e => translater.Apply(e));
        }

        public virtual IGeometry RemoveCommonBits(IGeometry g)
        {
            if (Common.X.IsZero() && Common.Y.IsZero())
            {
                return g;
            }

            var c = new Coordinate(Common);

            c.X = -c.X;
            c.Y = -c.Y;

            var translater = new Translater(c);
            g.GetCoordinates().ForEach(e => translater.Apply(e));

            return g;
        }

        public class CommonCoordinateFilter : ICoordinateFilter
        {
            public CommonCoordinateFilter()
            {
                X = new CommonBits();
                Y = new CommonBits();
            }

            readonly CommonBits X, Y;

            public virtual void Apply(ICoordinate coord)
            {
                X.Add(coord.X);
                Y.Add(coord.Y);
            }

            public virtual Coordinate CommonCoordinate
            {
                get { return new Coordinate(X.Common, Y.Common); }
            }
        }

        sealed class Translater : ICoordinateFilter
        {
            readonly ICoordinate Offset;

            public Translater(ICoordinate offset)
            {
                Offset = offset;
            }

            public void Apply(ICoordinate coord)
            {
                coord.X += Offset.X;
                coord.Y += Offset.Y;
            }
        }
    }
}
