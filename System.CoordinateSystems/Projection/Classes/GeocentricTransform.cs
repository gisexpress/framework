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

using System.Geometries;

namespace System.CoordinateSystems
{
    internal class GeocentricTransform : MathTransform
    {
        public GeocentricTransform(int sourceSrid, int targetSrid, IEllipsoid ellipsoid)
            : base(sourceSrid, targetSrid)
        {
            double d1 = (SemiMinorAxis = ellipsoid.SemiMinorAxis).Pow();
            double d2 = (SemiMajorAxis = ellipsoid.SemiMajorAxis).Pow();

            EccentricitySquared = 1.0 - (d1 / d2);
            SecondEccentricitySquared = (d2 - d1) / d1;
        }

        protected const double AdC = 1.0026000;
        protected const double Cos67P5 = 0.38268343236508977;

        protected readonly double SemiMajorAxis;
        protected readonly double SemiMinorAxis;
        protected readonly double EccentricitySquared;
        protected readonly double SecondEccentricitySquared;

        protected override bool OnTransform(ICoordinate c)
        {
            if (!IsInverse)
            {
                return DegreesToMeters(c);
            }

            return MetersToDegrees(c);
        }

        bool DegreesToMeters(ICoordinate c)
        {
            double lon = c.X.DegreeToRadian();
            double lat = c.Y.DegreeToRadian();
            double latSin = Math.Sin(lat);
            double latCos = Math.Cos(lat);
            double v = SemiMajorAxis / Math.Sqrt(1.0 - EccentricitySquared * latSin.Pow());
            double vh = v + c.Z;

            c.SetValues(vh * latCos * Math.Cos(lon), vh * latCos * Math.Sin(lon), ((1.0 - EccentricitySquared) * vh) * latSin);

            return true;
        }

        bool MetersToDegrees(ICoordinate c)
        {
            bool atPole = false;

            double lon;
            double lat = 0.0;
            double height;

            if (!c.X.IsZero())
            {
                lon = Math.Atan2(c.Y, c.X);
            }
            else
            {
                if (c.Y > 0.0)
                {
                    lon = AppConstants.HalfPI;
                }
                else if (c.Y < 0.0)
                {
                    lon = -AppConstants.HalfPI;
                }
                else
                {
                    atPole = true;
                    lon = 0.0;

                    if (c.Z > 0.0)
                    {
                        // north pole
                        lat = AppConstants.HalfPI;
                    }
                    else if (c.Z < 0.0)
                    {
                        // south pole
                        lat = -AppConstants.HalfPI;
                    }
                    else
                    {
                        // center of earth
                        c.SetValues(lon.RadianToDegree(), AppConstants.HalfPI.RadianToDegree(), -SemiMinorAxis);
                        return true;
                    }
                }
            }

            double w2 = c.X.Pow() + c.Y.Pow();
            double w = w2.Sqrt();
            double t0 = c.Z * AdC;
            double s0 = (t0.Pow() + w2).Sqrt();
            double t1 = c.Z + SemiMinorAxis * SecondEccentricitySquared * (t0 / s0).Pow(3);
            double sum = w - SemiMajorAxis * EccentricitySquared * (w / s0).Pow(3);
            double s1 = (t1.Pow() + sum.Pow()).Sqrt();
            double sinP1 = t1 / s1;
            double cosP1 = sum / s1;
            double rn = SemiMajorAxis / (1.0 - EccentricitySquared * sinP1.Pow()).Sqrt();

            if (cosP1 >= Cos67P5)
            {
                height = w / cosP1 - rn;
            }
            else if (cosP1 <= -Cos67P5)
            {
                height = w / -cosP1 - rn;
            }
            else
            {
                height = c.Z / sinP1 + rn * (EccentricitySquared - 1.0);
            }

            if (!atPole)
            {
                lat = Math.Atan(sinP1 / cosP1);
            }

            c.SetValues(lon.RadianToDegree(), lat.RadianToDegree(), height);

            return true;
        }
    }
}
