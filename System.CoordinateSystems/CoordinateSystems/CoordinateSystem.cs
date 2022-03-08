﻿//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright © GISExpress 2015-2022. All Rights Reserved.
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

using System.Collections.Generic;
using System.CoordinateSystems.Epsg;
using System.IO;
using System.Linq;

namespace System.CoordinateSystems
{
    internal abstract partial class CoordinateSystem : Info, ICoordinateSystem
    {
        protected CoordinateSystem(string name, Authority authority, IAxisInfoCollection axes)
            : base(name, authority)
        {
            Axes = axes ?? AxisInfoCollection.Default;
        }

        public IAxisInfoCollection Axes
        {
            get;
            protected set;
        }

        public abstract IUnit GetUnit();

        public IMathTransform CreateTransform(ICoordinateSystem target)
        {
            if (target.HasValue() && !IsEquivalent(target))
            {
                return OnCreateTransform(target);
            }

            return default;
        }

        protected abstract IMathTransform OnCreateTransform(ICoordinateSystem target);

        public double[] GetDefaultBounds()
        {
            return OnComputeBounds().ToArray();
        }

        protected virtual IEnumerable<double> OnComputeBounds()
        {
            if (!Authority.IsEmpty())
            {
                if (Authority == 3857)
                {
                    yield return -180.0;
                    yield return 180.0;
                    yield return -85.05113;
                    yield return 85.05113;
                }
                else
                {
                    var crs = EpsgCoordinateReferenceSystemTable.Current.Find(Authority);

                    if (crs.HasValue())
                    {
                        var area = EpsgAreaTable.Current.Find(crs.GetAreaCode());

                        if (area.HasValue())
                        {
                            yield return area.GetWestLongitude();
                            yield return area.GetEastLongitude();
                            yield return area.GetSouthLatitude();
                            yield return area.GetNorthLatitude();
                        }
                    }
                }
            }
        }

        public static ICoordinateSystem Read(string s)
        {
            return Read(Tokenizer.Parse(s));
        }

        protected static ICoordinateSystem Read(ITokenEnumerator e)
        {
            if (e.HasValue() & e.MoveNext())
            {
                ProjectedCoordinateSystem projected;
                IGeographicCoordinateSystem geographic;

                if (ProjectedCoordinateSystem.Read(e, out projected))
                {
                    return projected;
                }

                if (GeographicCoordinateSystemCls.Read(e, out geographic))
                {
                    return geographic;
                }
            }

            return default(ICoordinateSystem);
        }

        public abstract string GetDefinition();
    }
}
