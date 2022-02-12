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

using System.Diagnostics;
using System.Drawing;

namespace System.Transformations
{
    public struct Quaternion
    {
        static Quaternion()
        {
            Identity = new Quaternion(0, 0, 0, 1)
            {
                IsDistinguishedIdentity = true
            };
        }

        public Quaternion(double x, double y, double z, double w)
        {
            ValueX = x;
            ValueY = y;
            ValueZ = z;
            ValueW = w;
            IsDistinguishedIdentity = false;
        }

        /// <summary>
        /// Constructs a quaternion via specified axis of rotation and an angle.
        /// </summary>
        /// <param name="axisOfRotation">Vector representing axis of rotation.</param>
        /// <param name="angle">Angle to turn around the given axis (in degrees).</param>
        public Quaternion(Point3D axisOfRotation, double angle)
        {
            double angleInRadians = ((angle * 0.5) % 360.0).DegreeToRadian();
            Point3D c = (axisOfRotation / axisOfRotation.Length) * Math.Sin(angleInRadians);

            ValueX = c.X;
            ValueY = c.Y;
            ValueZ = c.Z;
            ValueW = Math.Cos(angleInRadians);

            IsDistinguishedIdentity = false;
        }

        internal double ValueX;
        internal double ValueY;
        internal double ValueZ;
        internal double ValueW;
        internal bool IsDistinguishedIdentity;

        /// <summary>
        /// Identity quaternion
        /// </summary>
        public static readonly Quaternion Identity;

        /// <summary>
        /// Retrieves quaternion's axis.
        /// </summary>
        public Point3D Axis
        {
            get
            {
                if (IsDistinguishedIdentity || (ValueX == 0 && ValueY == 0 && ValueZ == 0))
                {
                    return new Point3D(0, 1, 0);
                }
                else
                {
                    var point = new Point3D(ValueX, ValueY, ValueZ);
                    point.Normalize();
                    return point;
                }
            }
        }

        /// <summary>
        /// Retrieves quaternion's angle.
        /// </summary>
        public double Angle
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 0;
                }

                double msin = (ValueX.Pow() + ValueY.Pow() + ValueZ.Pow()).Sqrt();
                double mcos = ValueW;

                if (!(msin <= double.MaxValue))
                {
                    double max = Math.Max(ValueX.Abs(), Math.Max(ValueY.Abs(), ValueZ.Abs()));
                    double x = ValueX / max;
                    double y = ValueY / max;
                    double z = ValueZ / max;

                    msin = (x.Pow() + y.Pow() + z.Pow()).Sqrt();
                    mcos = ValueW / max;
                }

                return Math.Atan2(msin, mcos) * (360.0 / Math.PI);
            }
        }

        /// <summary>
        /// Returns whether the quaternion is normalized (i.e. has a magnitude of 1).
        /// </summary>
        public bool IsNormalized
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }

                return (ValueX.Pow() + ValueY.Pow() + ValueZ.Pow() + ValueW.Pow()).IsOne();
            }
        }

        /// <summary>
        /// Tests whether or not a given quaternion is an identity quaternion.
        /// </summary>
        public bool IsIdentity
        {
            get { return IsDistinguishedIdentity || (ValueX.IsZero() && ValueY.IsZero() && ValueZ.IsZero() && ValueW.IsOne()); }
        }

        public void SetIdentity()
        {
            this = Identity;
        }

        /// <summary>
        /// Relaces quaternion with its conjugate
        /// </summary>
        public void Conjugate()
        {
            if (!IsDistinguishedIdentity)
            {
                ValueX = -ValueX;
                ValueY = -ValueY;
                ValueZ = -ValueZ;
            }
        }

        /// <summary>
        /// Replaces quaternion with its inverse
        /// </summary>
        public void Invert()
        {
            if (!IsDistinguishedIdentity)
            {
                Conjugate();

                double n = ValueX.Pow() + ValueY.Pow() + ValueZ.Pow() + ValueW.Pow();

                ValueX /= n;
                ValueY /= n;
                ValueZ /= n;
                ValueW /= n;
            }
        }

        /// <summary>
        /// Normalizes this quaternion.
        /// </summary>
        public void Normalize()
        {
            if (!IsDistinguishedIdentity)
            {
                double n = ValueX.Pow() + ValueY.Pow() + ValueZ.Pow() + ValueW.Pow();

                if (n > Double.MaxValue)
                {
                    double rmax = 1.0 / Max(ValueX.Abs(), ValueY.Abs(), ValueZ.Abs(), ValueW.Abs());

                    ValueX *= rmax;
                    ValueY *= rmax;
                    ValueZ *= rmax;
                    ValueW *= rmax;

                    n = ValueX.Pow() + ValueY.Pow() + ValueZ.Pow() + ValueW.Pow();
                }

                double inverse = 1.0 / n.Sqrt();

                ValueX *= inverse;
                ValueY *= inverse;
                ValueZ *= inverse;
                ValueW *= inverse;
            }
        }

        /// <summary>
        /// Quaternion addition.
        /// </summary>
        /// <param name="left">First quaternion being added.</param>
        /// <param name="right">Second quaternion being added.</param>
        /// <returns>Result of addition.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new Quaternion(0, 0, 0, 2);
                }
                else
                {
                    // We know left is not distinguished identity here.                    
                    left.ValueW += 1;
                    return left;
                }
            }
            else if (left.IsDistinguishedIdentity)
            {
                // We know right is not distinguished identity here.
                right.ValueW += 1;
                return right;
            }
            else
            {
                return new Quaternion(left.ValueX + right.ValueX, left.ValueY + right.ValueY, left.ValueZ + right.ValueZ, left.ValueW + right.ValueW);
            }
        }

        /// <summary>
        /// Quaternion addition.
        /// </summary>
        /// <param name="left">First quaternion being added.</param>
        /// <param name="right">Second quaternion being added.</param>
        /// <returns>Result of addition.</returns>
        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return (left + right);
        }

        /// <summary>
        /// Quaternion subtraction.
        /// </summary>
        /// <param name="left">Quaternion to subtract from.</param>
        /// <param name="right">Quaternion to subtract from the first quaternion.</param>
        /// <returns>Result of subtraction.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    // We know left is not distinguished identity here.
                    left.ValueW -= 1;
                    return left;
                }
            }
            else if (left.IsDistinguishedIdentity)
            {
                // We know right is not distinguished identity here.
                return new Quaternion(-right.ValueX, -right.ValueY, -right.ValueZ, 1 - right.ValueW);
            }
            else
            {
                return new Quaternion(left.ValueX - right.ValueX, left.ValueY - right.ValueY, left.ValueZ - right.ValueZ, left.ValueW - right.ValueW);
            }
        }

        /// <summary>
        /// Quaternion subtraction.
        /// </summary>
        /// <param name="left">Quaternion to subtract from.</param>
        /// <param name="right">Quaternion to subtract from the first quaternion.</param>
        /// <returns>Result of subtraction.</returns>
        public static Quaternion Subtract(Quaternion left, Quaternion right)
        {
            return (left - right);
        }

        /// <summary>
        /// Quaternion multiplication.
        /// </summary>
        /// <param name="left">First quaternion.</param>
        /// <param name="right">Second quaternion.</param>
        /// <returns>Result of multiplication.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            if (left.IsDistinguishedIdentity)
            {
                return right;
            }

            if (right.IsDistinguishedIdentity)
            {
                return left;
            }

            double x = left.ValueW * right.ValueX + left.ValueX * right.ValueW + left.ValueY * right.ValueZ - left.ValueZ * right.ValueY;
            double y = left.ValueW * right.ValueY + left.ValueY * right.ValueW + left.ValueZ * right.ValueX - left.ValueX * right.ValueZ;
            double z = left.ValueW * right.ValueZ + left.ValueZ * right.ValueW + left.ValueX * right.ValueY - left.ValueY * right.ValueX;
            double w = left.ValueW * right.ValueW - left.ValueX * right.ValueX - left.ValueY * right.ValueY - left.ValueZ * right.ValueZ;

            return new Quaternion(x, y, z, w);

        }

        /// <summary>
        /// Quaternion multiplication.
        /// </summary>
        /// <param name="left">First quaternion.</param>
        /// <param name="right">Second quaternion.</param>
        /// <returns>Result of multiplication.</returns>
        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            return left * right;
        }

        /// <summary>
        /// Scale this quaternion by a scalar.
        /// </summary>
        /// <param name="scale">Value to scale by.</param>            
        private void Scale(double scale)
        {
            if (IsDistinguishedIdentity)
            {
                ValueW = scale;
                IsDistinguishedIdentity = false;
            }
            else
            {
                ValueX *= scale;
                ValueY *= scale;
                ValueZ *= scale;
                ValueW *= scale;
            }
        }

        /// <summary>
        /// Return length of quaternion.
        /// </summary>
        public double Length()
        {
            if (IsDistinguishedIdentity)
            {
                return 1;
            }

            double n = ValueX.Pow() + ValueY.Pow() + ValueZ.Pow() + ValueW.Pow();

            if (!(n <= Double.MaxValue))
            {
                double max = Math.Max(Math.Max(ValueX.Abs(), ValueY.Abs()), Math.Max(ValueZ.Abs(), ValueW.Abs()));

                double x = ValueX / max;
                double y = ValueY / max;
                double z = ValueZ / max;
                double w = ValueW / max;

                // Return length of this smaller vector times the scale we applied originally.
                return (x.Pow() + y.Pow() + z.Pow() + w.Pow()).Sqrt() * max;
            }

            return n.Sqrt();
        }

        /// <summary>
        /// Smoothly interpolate between the two given quaternions using Spherical 
        /// Linear Interpolation (SLERP).
        /// </summary>
        /// <param name="from">First quaternion for interpolation.</param>
        /// <param name="to">Second quaternion for interpolation.</param>
        /// <param name="t">Interpolation coefficient.</param>
        /// <returns>SLERP-interpolated quaternion between the two given quaternions.</returns>
        public static Quaternion Slerp(Quaternion from, Quaternion to, double t)
        {
            return Slerp(from, to, t, true);
        }

        /// <summary>
        /// Smoothly interpolate between the two given quaternions using Spherical Linear Interpolation (SLERP).
        /// </summary>
        /// <param name="from">First quaternion for interpolation.</param>
        /// <param name="to">Second quaternion for interpolation.</param>
        /// <param name="t">Interpolation coefficient.</param>
        /// <param name="useShortestPath">If true, Slerp will automatically flip the sign of
        ///     the destination Quaternion to ensure the shortest path is taken.</param>
        /// <returns>SLERP-interpolated quaternion between the two given quaternions.</returns>
        public static Quaternion Slerp(Quaternion from, Quaternion to, double t, bool useShortestPath)
        {
            if (from.IsDistinguishedIdentity)
            {
                from.ValueW = 1;
            }
            if (to.IsDistinguishedIdentity)
            {
                to.ValueW = 1;
            }

            double cosOmega;
            double scaleFrom, scaleTo;

            // Normalize inputs and stash their lengths
            double lengthFrom = from.Length();
            double lengthTo = to.Length();
            from.Scale(1 / lengthFrom);
            to.Scale(1 / lengthTo);

            // Calculate cos of omega.
            cosOmega = from.ValueX * to.ValueX + from.ValueY * to.ValueY + from.ValueZ * to.ValueZ + from.ValueW * to.ValueW;

            if (useShortestPath)
            {
                // If we are taking the shortest path we flip the signs to ensure that
                // cosOmega will be positive.
                if (cosOmega < 0.0)
                {
                    cosOmega = -cosOmega;
                    to.ValueX = -to.ValueX;
                    to.ValueY = -to.ValueY;
                    to.ValueZ = -to.ValueZ;
                    to.ValueW = -to.ValueW;
                }
            }
            else
            {
                // If we are not taking the UseShortestPath we clamp cosOmega to
                // -1 to stay in the domain of Math.Acos below.
                if (cosOmega < -1.0)
                {
                    cosOmega = -1.0;
                }
            }

            // Clamp cosOmega to [-1,1] to stay in the domain of Math.Acos below.
            // The logic above has either flipped the sign of cosOmega to ensure it
            // is positive or clamped to -1 aready.  We only need to worry about the
            // upper limit here.
            if (cosOmega > 1.0)
            {
                cosOmega = 1.0;
            }

            Debug.Assert(!(cosOmega < -1.0) && !(cosOmega > 1.0), "cosOmega should be clamped to [-1,1]");

            // The mainline algorithm doesn't work for extreme
            // cosine values.  For large cosine we have a better
            // fallback hence the asymmetric limits.
            const double maxCosine = 1.0 - 1e-6;
            const double minCosine = 1e-10 - 1.0;

            // Calculate scaling coefficients.
            if (cosOmega > maxCosine)
            {
                // Quaternions are too close - use linear interpolation.
                scaleFrom = 1.0 - t;
                scaleTo = t;
            }
            else if (cosOmega < minCosine)
            {
                // Quaternions are nearly opposite, so we will pretend to 
                // is exactly -from.
                // First assign arbitrary perpendicular to "to".
                to = new Quaternion(-from.Y, from.X, -from.W, from.Z);

                double theta = t * Math.PI;

                scaleFrom = Math.Cos(theta);
                scaleTo = Math.Sin(theta);
            }
            else
            {
                // Standard case - use SLERP interpolation.
                double omega = Math.Acos(cosOmega);
                double sinOmega = Math.Sqrt(1.0 - cosOmega * cosOmega);
                scaleFrom = Math.Sin((1.0 - t) * omega) / sinOmega;
                scaleTo = Math.Sin(t * omega) / sinOmega;
            }

            // We want the magnitude of the output quaternion to be
            // multiplicatively interpolated between the input
            // magnitudes, i.e. lengthOut = lengthFrom * (lengthTo/lengthFrom)^t
            //                            = lengthFrom ^ (1-t) * lengthTo ^ t

            double lengthOut = lengthFrom * Math.Pow(lengthTo / lengthFrom, t);
            scaleFrom *= lengthOut;
            scaleTo *= lengthOut;

            return new Quaternion(scaleFrom * from.ValueX + scaleTo * to.ValueX,
                                  scaleFrom * from.ValueY + scaleTo * to.ValueY,
                                  scaleFrom * from.ValueZ + scaleTo * to.ValueZ,
                                  scaleFrom * from.ValueW + scaleTo * to.ValueW);
        }

        static private double Max(double a, double b, double c, double d)
        {
            if (b > a)
                a = b;
            if (c > a)
                a = c;
            if (d > a)
                a = d;
            return a;
        }

        /// <summary>
        /// X - Default value is 0.
        /// </summary>
        public double X
        {
            get
            {
                return ValueX;
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = Identity;
                    IsDistinguishedIdentity = false;
                }
                ValueX = value;
            }
        }

        /// <summary>
        /// Y - Default value is 0.
        /// </summary>
        public double Y
        {
            get
            {
                return ValueY;
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = Identity;
                    IsDistinguishedIdentity = false;
                }
                ValueY = value;
            }
        }

        /// <summary>
        /// Z - Default value is 0.
        /// </summary>
        public double Z
        {
            get
            {
                return ValueZ;
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = Identity;
                    IsDistinguishedIdentity = false;
                }
                ValueZ = value;
            }
        }

        /// <summary>
        /// W - Default value is 1.
        /// </summary>
        public double W
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1.0;
                }
                else
                {
                    return ValueW;
                }
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = Identity;
                    IsDistinguishedIdentity = false;
                }
                ValueW = value;
            }
        }
    }
}