//////////////////////////////////////////////////////////////////////////////////////////////////
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

using System.Collections;
using System.IO;

namespace System.CoordinateSystems
{
    internal abstract class ProjectionCls : Info, IProjection
    {
        protected ProjectionCls(string name, Authority authority, IEllipsoid ellipsoid, ILinearUnit unit, ProjectionParameterCollection parameters)
            : base(name, authority)
        {
            Ellipsoid = ellipsoid;
            Unit = unit;
            Parameters = parameters;
        }

        protected readonly ILinearUnit Unit;

        protected readonly IEllipsoid Ellipsoid;

        protected readonly ProjectionParameterCollection Parameters;

        protected double this[ProjectionParameterKind parameter]
        {
            get { return Parameters[parameter].Value; }
        }

        public abstract IMathTransform CreateTransform(int sourceSrid, int targetSrid);

        public override bool IsEquivalent(object obj)
        {
            var o = obj as ProjectionCls;

            if (ReferenceEquals(o, null))
            {
                return false;
            }

            if (Parameters.Count != o.Parameters.Count)
            {
                return false;
            }

            for (int i = 0; i < Parameters.Count; i++)
            {
                ProjectionParameter param = Parameters[o.Parameters[i].Kind, false];

                if (param == null)
                {
                    return false;
                }

                if (!((param.Value - o.Parameters[i].Value).Abs() < 1e-8))
                {
                    return false;
                }
            }

            return true;
        }

        protected const double TwoPi = (Math.PI * 2.0);
        protected const double PrjMaxlong = 2147483647;
        protected const double DoubleLong = 4.61168601e18;
        protected const double Epsilon = 1e-10;

        protected static double E0Fn(double x)
        {
            return (1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x)));
        }

        protected static double E1Fn(double x)
        {
            return (0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x)));
        }

        protected static double E2Fn(double x)
        {
            return (0.05859375 * x * x * (1.0 + 0.75 * x));
        }

        protected static double E3Fn(double x)
        {
            return (x * x * x * (35.0 / 3072.0));
        }

        protected double Mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return (e0 * phi - e1 * Math.Sin(phi * 2) + e2 * Math.Sin(phi * 4) - e3 * Math.Sin(phi * 6));
        }

        protected static double AdjustLon(double x)
        {
            int count = 0;

            for (; ; )
            {
                if (x.Abs() <= Math.PI)
                {
                    break;
                }

                if (((long)(x / Math.PI).Abs()) < 2)
                {
                    x = x - (x.Sign() * TwoPi);
                }
                else
                {
                    if (((long)(x / TwoPi).Abs()) < PrjMaxlong)
                    {
                        x = x - (((long)(x / TwoPi)) * TwoPi);
                    }
                    else
                        if (((long)(x / (PrjMaxlong * TwoPi)).Abs()) < PrjMaxlong)
                        {
                            x = x - (((long)(x / (PrjMaxlong * TwoPi))) * (TwoPi * PrjMaxlong));
                        }
                        else
                            if (((long)(x / (DoubleLong * TwoPi)).Abs()) < PrjMaxlong)
                            {
                                x = x - (((long)(x / (DoubleLong * TwoPi))) * (TwoPi * DoubleLong));
                            }
                            else
                                x = x - (x.Sign() * TwoPi);
                }

                count++;

                if (count > 4)
                {
                    break;
                }
            }

            return (x);
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return Parameters.Values.GetEnumerator();
        }

        public static bool Read(ITokenEnumerator e, IEllipsoid ellipsoid, ILinearUnit unit, out IProjection value)
        {
            value = null;

            if (e.Current.Equals(','))
            {
                e.MoveNext();
            }

            if (e.Current.Equals("PROJECTION") && e.MoveNext())
            {
                string name;
                Authority authority = null;
                ProjectionParameterCollection parameters;

                if (e.NextIs('['))
                {
                    e.MoveNext();
                }

                if (ReadName(e, out name))
                {
                    if (e.Current.Equals(',') && e.MoveNext())
                    {
                        ReadAuthority(e, out authority);
                    }

                    if (e.Current.Equals("PARAMETER") || (e.Current.Equals(']') && e.ReadNext(',') && e.MoveNext()))
                    {
                        ProjectionParameterCollection.Read(e, out parameters);
                        value = ProjectionFactory.Create(name, authority, ellipsoid, unit, parameters);
                    }
                }
            }

            return value.HasValue();
        }

        public override string ToString()
        {
            string s = string.Concat(@"PROJECTION[""", Name, @"""");

            if (!Authority.IsEmpty())
            {
                s = string.Concat(s, ',', Authority);
            }

            s = string.Concat(s, "]");

            foreach (ProjectionParameter parameter in this)
            {
                s = string.Concat(s, ',', parameter);
            }

            return s;
        }
    }
}
