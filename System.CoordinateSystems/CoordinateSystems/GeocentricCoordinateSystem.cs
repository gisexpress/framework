//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright � GISExpress 2015-2022. All Rights Reserved.
//  
//  GISExpress .NET API and Component Library
//  
//  The entire contents of this file is protected by local and International Copyright Laws.
//  Unauthorized reproduction, reverse-engineering, and distribution of all or any portion of
//  the code contained in this file is strictly prohibited and may result in severe civil and 
//  criminal penalties and will be prosecuted to the maximum extent possible under the law.
//  
//  RESTRICTIONS
//  
//  THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES ARE CONFIDENTIAL AND PROPRIETARY TRADE SECRETS OF GISExpress
//  THE REGISTERED DEVELOPER IS LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET COMPONENTS AS PART OF AN EXECUTABLE PROGRAM ONLY.
//  
//  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE
//  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT
//  AND PERMISSION FROM GISExpress
//  
//  CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.
//  
//  Warning: This content was generated by GISExpress tools.
//  Changes to this content may cause incorrect behavior and will be lost if the content is regenerated.
//
///////////////////////////////////////////////////////////////////////////////////////////////////

namespace System.CoordinateSystems
{
    internal class GeocentricCoordinateSystem : CoordinateSystem, IGeocentricCoordinateSystem
    {
        public GeocentricCoordinateSystem(string name, Authority authority, IHorizontalDatum datum, IPrimeMeridian primeMeridian)
            : this(name, authority, datum, primeMeridian, null)
        {
        }

        public GeocentricCoordinateSystem(string name, Authority authority, IHorizontalDatum datum, IPrimeMeridian primeMeridian, AxisInfoCollection axes)
            : base(name, authority, axes ?? new AxisInfoCollection { { "X", AxisOrientation.Other }, { "Y", AxisOrientation.East }, { "Z", AxisOrientation.North } })
        {
            HorizontalDatum = datum;
            PrimeMeridian = primeMeridian;
        }

        public IHorizontalDatum HorizontalDatum
        {
            get;
            protected set;
        }

        public IPrimeMeridian PrimeMeridian
        {
            get;
            protected set;
        }

        public override IUnit GetUnit()
        {
            return HorizontalDatum.Ellipsoid.AxisUnit;
        }

        protected override IMathTransform OnCreateTransform(ICoordinateSystem target)
        {
            IMathTransform transform;

            if (!ReferenceEquals(transform = CreateTransform(target as GeocentricCoordinateSystem), null)) return ((ConcatenatedTransform)transform).GetTransform();
            if (!ReferenceEquals(transform = CreateTransform(target as GeographicCoordinateSystemCls), null)) return transform;

            throw new NotImplementedException();
        }

        ConcatenatedTransform CreateTransform(GeocentricCoordinateSystem target)
        {
            if (target.HasValue())
            {
                var concatenated = new ConcatenatedTransform(Authority, target.Authority);

                // Does source has a datum different from WGS84 and is there a shift specified?
                if (!HorizontalDatum.BursaWolfParameters.IsEmpty())
                {
                    concatenated.Add(new DatumTransform(Authority, target.Authority, HorizontalDatum.BursaWolfParameters));
                }

                // Does target has a datum different from WGS84 and is there a shift specified?
                if (!target.HorizontalDatum.BursaWolfParameters.IsEmpty())
                {
                    var transform = new DatumTransform(Authority, target.Authority, target.HorizontalDatum.BursaWolfParameters);
                    transform.Invert();
                    concatenated.Add(transform);
                }

                return concatenated;
            }

            return null;
        }

        IMathTransform CreateTransform(GeographicCoordinateSystemCls target)
        {
            IMathTransform transform = target.CreateTransform(this);
            transform.Invert();
            return transform;
        }

        public override bool IsEquivalent(object obj)
        {
            var o = obj as IGeocentricCoordinateSystem;

            if (o.IsNull())
            {
                return false;
            }

            return o.HorizontalDatum.IsEquivalent(HorizontalDatum) && o.PrimeMeridian.IsEquivalent(PrimeMeridian);
        }

        public override string GetDefinition()
        {
            return string.Concat(
                    "Datum : ", HorizontalDatum.Name, Environment.NewLine,
                    "  Spheroid : ", HorizontalDatum.Ellipsoid.Name, Environment.NewLine,
                    "    Axis Unit : ", HorizontalDatum.Ellipsoid.AxisUnit.Name, Environment.NewLine,
                    "    Semimajor Axis : ", HorizontalDatum.Ellipsoid.SemiMajorAxis, Environment.NewLine,
                    "    Semiminor Axis : ", HorizontalDatum.Ellipsoid.SemiMinorAxis, Environment.NewLine,
                    "    Inverse Flattening : ", HorizontalDatum.Ellipsoid.InverseFlattening, Environment.NewLine);
        }

        public override string ToString()
        {
            var s = string.Concat(@"GEOCCS[""", Name, @""",", HorizontalDatum, ',', PrimeMeridian, ',', HorizontalDatum.Ellipsoid.AxisUnit);

            if (Axes.Count != 3 ||
                Axes[0].Name != "X" || Axes[0].Orientation != AxisOrientation.Other ||
                Axes[1].Name != "Y" || Axes[1].Orientation != AxisOrientation.East ||
                Axes[2].Name != "Z" || Axes[2].Orientation != AxisOrientation.North)
            {
                foreach (AxisInfo i in Axes)
                {
                    s = string.Concat(s, ",{0}", i);
                }
            }

            if (!Authority.IsEmpty())
            {
                s = string.Concat(s, ',', Authority);
            }

            return string.Concat(s, "]");
        }
    }
}
