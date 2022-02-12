using System.Collections.Generic;
using System.Diagnostics;

namespace System.Geometries.Utilities
{
    public class GeometryTransformer
    {
        /// <summary>
        /// <c>true</c> if a homogenous collection result from a <c>GeometryCollection</c> should still be a general GeometryCollection.
        /// </summary>
        protected bool PreserveGeometryCollectionType = true;

        /// <summary>
        /// Makes the input geometry available
        /// </summary>
        public IGeometry InputGeometry
        {
            get;
            protected set;
        }

        public IGeometry Transform(IGeometry input)
        {
            InputGeometry = input;

            if (input is IPoint || input is IMultiPoint)
            {
                return input;
            }

            if (input is ILinearRing ring)
            {
                return TransformLineString(ring, default);
            }

            if (input is ILineString line)
            {
                return TransformLineString(line, default);
            }

            if (input is IPolygon poly)
            {
                return TransformPolygon(poly, default);
            }

            if (input is IGeometryCollection collection)
            {
                return TransformGeometryCollection(collection, default(Geometry));
            }

            Debug.Fail("Unknown Geometry subtype");
            return default;
        }

        protected virtual ICoordinateCollection TransformCoordinates(ICoordinateCollection coords, IGeometry parent)
        {
            return coords.Clone();
        }

        protected virtual IGeometry TransformLinearRing(ILinearRing input, IGeometry parent)
        {
            ICoordinateCollection coords = TransformCoordinates(input.Coordinates, input);

            if (coords == null)
            {
                return default;
            }

            var n = coords.Count;

            if (n > 0 && n < 4)
            {
                return input.Factory.Create<ILineString>(coords);
            }

            return input.Factory.Create<ILinearRing>(coords);
        }

        protected virtual IGeometry TransformLineString(ILineString input, IGeometry parent)
        {
            var g = input.Factory.Create<ILineString>();
            g.Coordinates = TransformCoordinates(input.Coordinates, input);
            return g;
        }

        protected virtual IGeometry TransformPolygon(IPolygon input, IGeometry parent)
        {
            bool validRings = true;
            IGeometry shell = TransformLinearRing(input.ExteriorRing, input);

            if (shell == null || !(shell is LinearRing))
            {
                validRings = false;
            }

            var holes = new List<ILineString>();

            for (int i = 0; i < input.InteriorRings.Count; i++)
            {
                IGeometry interior = TransformLinearRing(input.InteriorRings.Get(i), input);

                if (interior == null) continue;
                validRings &= interior is ILinearRing;

                holes.Add((LineString)interior);
            }

            if (validRings)
            {
                var rings = new ILinearRing[holes.Count];

                // in case your IDE whines about array covariance on this line,
                // don't worry, it's safe -- we proved above that it will work.
                holes.CopyTo(rings);
                return input.Factory.Create<IPolygon>((ILinearRing)shell, rings);
            }
            else
            {
                var components = new List<IGeometry>();

                if (shell != null)
                {
                    components.Add(shell);
                }

                foreach (Geometry hole in holes)
                {
                    components.Add(hole);
                }

                return input.Factory.BuildGeometry(components);
            }
        }

        protected virtual IGeometry TransformGeometryCollection(IGeometryCollection input, IGeometry parent)
        {
            var list = new List<IGeometry>();

            for (int i = 0; i < input.NumGeometries(); i++)
            {
                IGeometry g = Transform(input.GetGeometryN(i));
                if (g == null) continue;
                list.Add(g);
            }

            if (PreserveGeometryCollectionType)
            {
                return input.Factory.Create<IGeometryCollection>(list);
            }

            return input.Factory.BuildGeometry(list);
        }
    }
}
