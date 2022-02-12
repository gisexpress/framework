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

using System.ComponentModel;
using System.CoordinateSystems;
using System.Diagnostics;
using System.Geometries.Algorithm;
using System.Transformations;

namespace System.Geometries
{
    internal class LineSegment : ILineSegment
    {
        public LineSegment()
        {
        }

        public LineSegment(LineSegment segment)
            : this(segment.P0, segment.P1)
        {
        }

        public LineSegment(ICoordinate start, ICoordinate end)
        {
            P0 = start;
            P1 = end;
        }

        public LineSegment(double x1, double y1, double x2, double y2)
        {
            P0 = new Coordinate(x1, y1);
            P1 = new Coordinate(x2, y2);
        }

        public LineSegment(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            P0 = new Coordinate(x1, y1, z1);
            P1 = new Coordinate(x2, y2, z2);
        }

        public ICoordinate P0
        {
            get;
            set;
        }

        public ICoordinate P1
        {
            get;
            set;
        }

        public ICoordinate GetCoordinate(int i)
        {
            return (i == 0) ? P0 : P1;
        }

        public void SetCoordinates(ICoordinate p0, ICoordinate p1)
        {
            P0 = P0 ?? Coordinate.Empty;
            P1 = P1 ?? Coordinate.Empty;

            p0.CopyTo(P0);
            p1.CopyTo(P1);
        }

        //public IEnvelope GetBounds()
        //{
        //    return new Envelope(P0, P1);
        //}

        public double GetLength()
        {
            return P0.Distance(P1);
        }

        public void SetLength(double distance)
        {
            SetLength(P0, P1, distance);
        }

        public static void SetLength(ICoordinate p0, ICoordinate p1, double distance)
        {
            Extend(p0, p1, 0.0, distance - p0.Distance(p1));
        }

        public bool IsHorizontal
        {
            get { return P0.Y.IsEquivalent(P1.Y); }
        }

        public bool IsVertical
        {
            get { return P0.X.IsEquivalent(P1.X); }
        }

        public void Reverse()
        {
            ICoordinate c = P0;
            P0 = P1;
            P1 = c;
        }

        public int OrientationIndex(ICoordinate c)
        {
            return ComputeOrientation(P0, P1, c);
        }

        public int ReverseOrientationIndex(ICoordinate c)
        {
            return ComputeOrientation(P1, P0, c);
        }

        public static int ComputeOrientation(ICoordinate p0, ICoordinate p1, ICoordinate c)
        {
            return RobustDeterminant.OrientationIndex(p0, p1, c);
        }

        public int OrientationIndex(LineSegment seg)
        {
            int orient0 = OrientationIndex(seg.P0);
            int orient1 = OrientationIndex(seg.P1);

            if (orient0 >= 0 && orient1 >= 0) return Math.Max(orient0, orient1);
            if (orient0 <= 0 && orient1 <= 0) return Math.Max(orient0, orient1);

            return 0;
        }

        public void Normalize()
        {
            if (P1.CompareTo(P0) < 0) Reverse();
        }

        public double GetAngle()
        {
            return P0.Angle(P1);
        }

        public void SetAngle(double angle)
        {
            SetAngle(P0, P1, angle);
        }

        public static void SetAngle(ICoordinate p0, ICoordinate p1, double angle)
        {
            double length = p0.Distance(p1);

            if (!length.IsZero())
            {
                p1.SetValues(p0.X + length * Math.Cos(angle), p0.Y + length * Math.Sin(angle));
            }
        }

        public double Distance(ILineSegment other)
        {
            if (P0.IsEquivalent(P1)) return other.Distance(P0);
            if (other.P0.IsEquivalent(other.P1)) return Distance(other.P1);

            if (IsIntersects(other))
            {
                return 0.0;
            }

            return Math.Min(other.Distance(P0), Math.Min(other.Distance(P1), Math.Min(Distance(other.P0), Distance(other.P1))));
        }

        public double Distance(ICoordinate c)
        {
            return Distance(c.X, c.Y);
        }

        public double Distance(double x, double y)
        {
            if (P0.IsEquivalent(P1))
            {
                return P0.Distance(x, y);
            }
            
            double n = (((x - P0.X) * (P1.X - P0.X)) + ((y - P0.Y) * (P1.Y - P0.Y))) / (((P1.X - P0.X) * (P1.X - P0.X)) + ((P1.Y - P0.Y) * (P1.Y - P0.Y)));

            if (n <= 0.0)
            {
                return P0.Distance(x, y);
            }

            if (n >= 1.0)
            {
                return P1.Distance(x, y);
            }

            return ((((P0.Y - y) * (P1.X - P0.X)) - ((P0.X - x) * (P1.Y - P0.Y))) / (((P1.X - P0.X) * (P1.X - P0.X)) + ((P1.Y - P0.Y) * (P1.Y - P0.Y)))).Abs() * (((P1.X - P0.X) * (P1.X - P0.X)) + ((P1.Y - P0.Y) * (P1.Y - P0.Y))).Sqrt();
        }

        public double DistancePerpendicular(ICoordinate c)
        {
            return SignedDistancePerpendicular(P0, P1, c).Abs();
        }

        public double SignedDistancePerpendicular(ICoordinate c)
        {
            return SignedDistancePerpendicular(P0, P1, c);
        }

        public static double SignedDistancePerpendicular(ICoordinate c1, ICoordinate c2, ICoordinate point)
        {
            return ((((c1.Y - point.Y) * (c2.X - c1.X)) - ((c1.X - point.X) * (c2.Y - c1.Y))) / (((c2.X - c1.X) * (c2.X - c1.X)) + ((c2.Y - c1.Y) * (c2.Y - c1.Y)))) * c1.Distance(c2);
        }

        public double EdgeDistance(ICoordinate c)
        {
            double d = -1.0;
            double dx = Math.Abs(P1.X - P0.X);
            double dy = Math.Abs(P1.Y - P0.Y);

            if (c.IsEquivalent(P0))
            {
                d = 0.0;
            }
            else if (c.IsEquivalent(P1))
            {
                if (dx > dy)
                {
                    d = dx;
                }
                else
                {
                    d = dy;
                }
            }
            else
            {
                double pdx = Math.Abs(c.X - P0.X);
                double pdy = Math.Abs(c.Y - P0.Y);

                if (dx > dy)
                {
                    d = pdx;
                }
                else
                {
                    d = pdy;
                }

                if (d.IsZero() && !c.IsEquivalent(P0))
                {
                    d = Math.Max(pdx, pdy);
                }
            }

            Debug.Assert(!(d.IsZero() && !c.IsEquivalent(P0)), "Bad distance calculation");

            return d;
        }

        public double ProjectionFactor(ICoordinate c)
        {
            if (c.IsEquivalent(P0)) return 0.0;
            if (c.IsEquivalent(P1)) return 1.0;

            double dx = P1.X - P0.X;
            double dy = P1.Y - P0.Y;
            double length = dx * dx + dy * dy;

            return ((c.X - P0.X) * dx + (c.Y - P0.Y) * dy) / length;
        }

        public bool Project(ICoordinate c)
        {
            if (c.IsEquivalent(P0))
            {
                return true;
            }

            if (c.IsEquivalent(P1))
            {
                return true;
            }

            double r = ProjectionFactor(c);

            if (!double.IsNaN(r))
            {
                c.SetValues(P0.X + r * (P1.X - P0.X), P0.Y + r * (P1.Y - P0.Y));
                return true;
            }

            return false;
        }

        public ICoordinate GetMidPoint()
        {
            return GetMidPoint(P0, P1);
        }

        public static ICoordinate GetMidPoint(ICoordinate c1, ICoordinate c2)
        {
            return Coordinate.Create((c1.X + c2.X) / 2.0, (c1.Y + c2.Y) / 2.0, (c1.Z + c2.Z) / 2.0);
        }

        public void Extend(double distance)
        {
            Extend(distance, distance);
        }

        public void Extend(double start, double end)
        {
            Extend(P0, P1, start, end);
        }

        public static void Extend(ICoordinate p0, ICoordinate p1, double start, double end)
        {
            double length = p0.Distance(p1);

            if (!length.IsZero())
            {
                double x1 = p0.X + (p0.X - p1.X) / length * start;
                double y1 = p0.Y + (p0.Y - p1.Y) / length * start;
                double x2 = p1.X + (p1.X - p0.X) / length * end;
                double y2 = p1.Y + (p1.Y - p0.Y) / length * end;

                p0.SetValues(x1, y1);
                p1.SetValues(x2, y2);
            }
        }

        public bool Transform(IMathTransform transform)
        {
            return transform.Transform(P0) && transform.Transform(P1);
        }

        public bool IsIntersects(ILineSegment other)
        {
            double n = ((P0.Y - other.P0.Y) * (other.P1.X - other.P0.X)) - ((P0.X - other.P0.X) * (other.P1.Y - other.P0.Y));
            double n2 = ((P1.X - P0.X) * (other.P1.Y - other.P0.Y)) - ((P1.Y - P0.Y) * (other.P1.X - other.P0.X));
            double n3 = ((P0.Y - other.P0.Y) * (P1.X - P0.X)) - ((P0.X - other.P0.X) * (P1.Y - P0.Y));
            double n4 = ((P1.X - P0.X) * (other.P1.Y - other.P0.Y)) - ((P1.Y - P0.Y) * (other.P1.X - other.P0.X));

            if (!n2.IsZero() && !n4.IsZero())
            {
                double num5 = n3 / n4;
                double num6 = n / n2;

                if (((num6 >= 0.0) && (num6 <= 1.0)) && ((num5 >= 0.0) && (num5 <= 1.0)))
                {
                    return true;
                }
            }

            return false;
        }

        public ICoordinate Intersection(ILineSegment other)
        {
            var intersector = new RobustLineIntersector();

            intersector.ComputeIntersection(P0, P1, other.P0, other.P1);

            if (intersector.HasIntersection)
            {
                return intersector.GetIntersection(0);
            }

            return null;
        }

        public int CollinearIntersection(ILineSegment other, ref ICoordinate[] inputPoints)
        {
            bool r0 = Envelope.Intersects(P0, P1, other.P0);
            bool r1 = Envelope.Intersects(P0, P1, other.P1);
            bool r2 = Envelope.Intersects(other.P0, other.P1, P0);
            bool r3 = Envelope.Intersects(other.P0, other.P1, P1);

            if (r0 && r1)
            {
                inputPoints[0] = other.P0;
                inputPoints[1] = other.P1;
                return 2;
            }

            if (r2 && r3)
            {
                inputPoints[0] = P0;
                inputPoints[1] = P1;
                return 2;
            }

            if (r0 && r2)
            {
                inputPoints[0] = other.P0;
                inputPoints[1] = P0;

                if ((other.P0.IsEquivalent(P0)))
                {
                    return 1;
                }

                return 2;
            }

            if (r0 && r3)
            {
                inputPoints[0] = other.P0;
                inputPoints[1] = P1;

                if ((other.P0.IsEquivalent(P1)))
                {
                    return 1;
                }

                return 2;
            }

            if (r1 && r2)
            {
                inputPoints[0] = other.P1;
                inputPoints[1] = P0;

                if ((other.P1.IsEquivalent(P0)))
                {
                    return 1;
                }

                return 2;
            }

            if (!r1 || !r3)
            {
                return 0;
            }

            inputPoints[0] = other.P1;
            inputPoints[1] = P1;

            if (other.P1.IsEquivalent(P1))
            {
                return 1;
            }

            return 2;
        }

        public Coordinate PointAlongOffset(double segmentLengthFraction, double offsetDistance)
        {
            return PointAlongOffset(P0, P1, segmentLengthFraction, offsetDistance);
        }

        public static Coordinate PointMidOffset(ICoordinate c1, ICoordinate c2, double offsetDistance)
        {
            return PointAlongOffset(c1, c2, 0.5, offsetDistance);
        }

        public static Coordinate PointAlongOffset(ICoordinate c1, ICoordinate c2, double segmentLengthFraction, double offsetDistance)
        {
            double dx = c2.X - c1.X;
            double dy = c2.Y - c1.Y;
            double ux = 0.0;
            double uy = 0.0;

            double segx = c1.X + segmentLengthFraction * (c2.X - c1.X);
            double segy = c1.Y + segmentLengthFraction * (c2.Y - c1.Y);

            if (!offsetDistance.IsZero())
            {
                double len = (dx * dx + dy * dy).Sqrt();

                if (len <= 0.0)
                {
                    // Cannot compute offset from zero-length line segment
                    return null;
                }

                ux = offsetDistance * dx / len;
                uy = offsetDistance * dy / len;
            }

            double offsetx = segx - uy;
            double offsety = segy + ux;

            return new Coordinate(offsetx, offsety);
        }

        public void Rotate(double angle)
        {
            RotateAt(angle, GetMidPoint());
        }

        public void RotateAt(double angle, ICoordinate point)
        {
            RotateAt(angle, point.X, point.Y);
        }

        public void RotateAt(double angle, double x, double y)
        {
            var transform = new MatrixTransform();

            transform.RotateAt(angle, x, y);
            transform.Transform(P0);
            transform.Transform(P1);
        }

        /// <summary>
        /// Gets the minimum X ordinate
        /// </summary>
        public double MinX { get { return Math.Min(P0.X, P1.X); } }

        /// <summary>
        /// Gets the maximum X ordinate
        /// </summary>
        public double MaxX { get { return Math.Max(P0.X, P1.X); } }

        /// <summary>
        /// Gets the minimum Y ordinate
        /// </summary>
        public double MinY { get { return Math.Min(P0.Y, P1.Y); } }

        /// <summary>
        /// Gets the maximum Y ordinate
        /// </summary>
        public double MaxY { get { return Math.Max(P0.Y, P1.Y); } }

        public int GetHashCode(bool reverse)
        {
            return reverse ? GetHashCode(P1, P0) : GetHashCode(P0, P1);
        }

        public override int GetHashCode()
        {
            return GetHashCode(P0, P1);
        }

        public static int GetHashCode(ICoordinate p0, ICoordinate p1)
        {
            int n = Hash.Seed;

            unchecked
            {
                n = n * Hash.Step + p0.GetHashCode();
                n = n * Hash.Step + p1.GetHashCode();
            }

            return n;
        }

        public override bool Equals(object o)
        {
            var lineSegment = o as LineSegment;

            if (lineSegment == null)
            {
                return false;
            }

            return P0.IsEquivalent(lineSegment.P0) && P1.IsEquivalent(lineSegment.P1);
        }

        public static bool operator ==(LineSegment obj1, LineSegment obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(LineSegment obj1, LineSegment obj2)
        {
            return !(obj1 == obj2);
        }

        public int CompareTo(object o)
        {
            var other = (LineSegment)o;
            int comp0 = P0.CompareTo(other.P0);

            if (comp0 != 0)
            {
                return comp0;
            }

            return P1.CompareTo(other.P1);
        }

        public ILineSegment Subtract(ICoordinate c)
        {
            return new LineSegment(P0.Subtract(c), P1.Subtract(c));
        }

        //public ILineString ToLineString()
        //{
        //    return TypeFactory.Create<ILineString>(P0.Clone(), P1.Clone());
        //}

        public bool IsEquivalent(ILineSegment other)
        {
            return (P0.IsEquivalent(other.P0) && P1.IsEquivalent(other.P1)) || (P0.IsEquivalent(other.P1) && P1.IsEquivalent(other.P0));
        }

        public ILineSegment Clone()
        {
            var c = MemberwiseClone() as LineSegment;

            c.P0 = P0.Clone();
            c.P1 = P1.Clone();

            return c;
        }

        public override string ToString()
        {
            return string.Concat(P0, ", ", P1);
        }
    }
}