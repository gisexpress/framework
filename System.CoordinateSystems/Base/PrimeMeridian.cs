//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright ? GISExpress 2015-2022. All Rights Reserved.
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
using PredefinedAngularUnit = System.CoordinateSystems.AngularUnitCls;

namespace System.CoordinateSystems
{
    internal class PrimeMeridianCls : Info, IPrimeMeridian
    {
        internal PrimeMeridianCls(string name, Authority authority, double longitude, AngularUnitCls angularUnit)
            : base(name, authority)
        {
            Longitude = longitude;
            AngularUnit = angularUnit;
        }

        public static PrimeMeridianCls Greenwich
        {
            get { return new PrimeMeridianCls("Greenwich", 8901, 0, PredefinedAngularUnit.Degrees); }
        }

        public double Longitude
        {
            get;
            set;
        }

        public IAngularUnit AngularUnit
        {
            get;
            set;
        }

        public override bool IsEquivalent(object obj)
        {
            var o = obj as PrimeMeridianCls;

            if (o == null)
            {
                return false;
            }

            if (!AngularUnit.IsEquivalent(o.AngularUnit))
            {
                return false;
            }

            if (!Longitude.IsEquivalent(o.Longitude))
            {
                return false;
            }

            return true;
        }

        public static bool Read(ITokenEnumerator e, out IPrimeMeridian primeMeridian)
        {
            if (e.Current.Equals(','))
            {
                e.MoveNext();
            }

            if (e.Current.Equals("PRIMEM") && e.ReadNext('['))
            {
                string name;
                Authority authority;
                NumberToken longitude;

                if (ReadName(e, out name) && e.Current.Equals(',') && e.ReadNumber(out longitude))
                {
                    if (e.MoveNext())
                    {
                        ReadAuthority(e, out authority);

                        if (e.Current.Equals(']'))
                        {
                            e.MoveNext();
                            primeMeridian = new PrimeMeridianCls(name, authority, Convert.ToDouble(longitude.Value), PredefinedAngularUnit.Degrees);
                            return true;
                        }
                    }
                }
            }

            primeMeridian = null;
            return false;
        }

        public override string ToString()
        {
            var s = string.Concat(@"PRIMEM[""", Name, @""",", Longitude.ToText());

            if (!Authority.IsEmpty())
            {
                s = string.Concat(s, ',', Authority);
            }

            return string.Concat(s, "]");
        }
    }
}
