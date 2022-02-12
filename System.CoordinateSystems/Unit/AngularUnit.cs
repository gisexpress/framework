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

using System.IO;
using System.Xml;

namespace System.CoordinateSystems
{
    internal class AngularUnitCls : Info, IAngularUnit
    {
        public AngularUnitCls(string name, Authority authority, double radiansPerUnit)
            : base(name, authority)
        {
            RadiansPerUnit = radiansPerUnit;
            Unit2Meter = Math.Cos(39.0 * radiansPerUnit) * 6378137.0;
        }

        protected readonly double Unit2Meter;

        public static AngularUnitCls Degrees
        {
            get { return new AngularUnitCls("Degree", 9102, Math.PI / 180.0); }
        }

        public static AngularUnitCls Radian
        {
            get { return new AngularUnitCls("Radian", 9101, 1); }
        }

        public static AngularUnitCls Grad
        {
            get { return new AngularUnitCls("Grad", 9105, Math.PI / 200.0); }
        }

        public static AngularUnitCls Gon
        {
            get { return new AngularUnitCls("Gon", 9106, Math.PI / 200.0); }
        }

        public double RadiansPerUnit
        {
            get;
            protected set;
        }

        public double ToMeter(double value)
        {
            return (value * RadiansPerUnit) * Unit2Meter;
        }

        public override bool IsEquivalent(object obj)
        {
            var o = obj as AngularUnitCls;

            if (ReferenceEquals(o, null))
            {
                return false;
            }

            return Math.Round(o.RadiansPerUnit, 8).Equals(Math.Round(RadiansPerUnit, 8));
        }

        public static bool Read(ITokenEnumerator e, out IAngularUnit unit)
        {
            if (e.Current.Equals(','))
            {
                e.MoveNext();
            }

            if (e.Current.Equals("UNIT") && e.ReadNext('['))
            {
                if (ReadName(e, out string name) && e.Current.Equals(',') && e.ReadNumber(out NumberToken radiansPerUnit))
                {
                    if (e.MoveNext())
                    {
                        ReadAuthority(e, out Authority authority);

                        if (e.Current.Equals(']'))
                        {
                            e.MoveNext();
                            unit = new AngularUnitCls(name, authority, Convert.ToDouble(radiansPerUnit.Value));
                            return true;
                        }
                    }
                }
            }

            unit = null;
            return false;
        }

        public override string ToString()
        {
            var s = string.Concat(@"UNIT[""", Name, @""",", RadiansPerUnit.ToText());

            if (!Authority.IsEmpty())
            {
                s = string.Concat(s, ',', Authority);
            }

            return string.Concat(s, "]");
        }
    }
}
