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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.CoordinateSystems;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Geometries.Operation.Buffer;
using System.Geometries.Operation.Overlay;
using System.Geometries.Operation.Relate;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Transformations;
using System.Xml;

namespace System.Geometries
{
    [DebuggerDisplay("{ToString()}")]
    internal abstract class Geometry : XmlGeometryElement, IGeometry
    {
        protected internal Geometry(string localName, XmlDocument doc) : base(string.Empty, localName, Constants.Xml.NamespaceURI, doc)
        {
        }

        IGeometryCollection Collection;
        IBufferParameters BufferParams;

        public IGeometryFactory Factory
        {
            get { return OwnerDocument.Factory; }
        }

        public int Srid
        {
            get;
            set;
        }

        public virtual new bool IsEmpty()
        {
            return Coordinates.IsEmpty();
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public virtual bool IsClosed()
        {
            return Coordinates.IsClosed;
        }

        public virtual bool IsRing()
        {
            return false;
        }

        public virtual bool IsRectangle()
        {
            return false;
        }

        public virtual ICoordinateCollection Coordinates
        {
            get { return (CoordinateCollection)GetOrCreate(Constants.Xml.Coordinates); }
            set
            {
                var e = value as XmlElementBase;

                e.Remove();
                AppendChild(e);
            }
        }

        public IBufferParameters BufferParameters
        {
            get { return BufferParams ?? (BufferParams = new BufferParameters()); }
            set { BufferParams = value; }
        }

        public virtual int NumPoints()
        {
            return Coordinates.Count;
        }

        public virtual int NumGeometries()
        {
            return 1;
        }

        public virtual IGeometry GetGeometryN(int i)
        {
            return this;
        }

        public virtual bool IsCollection()
        {
            return false;
        }

        public IGeometryCollection GetCollection()
        {
            return Collection;
        }

        public void SetCollection(IGeometryCollection collection)
        {
            Collection = collection;
        }

        public abstract string TypeName
        {
            get;
        }

        public virtual IEnumerable<IGeometry> GetGeometries()
        {
            yield return this;
        }

        public abstract Dimensions GetDimension();

        public abstract Dimensions GetBoundaryDimension();

        public abstract double GetDistance(ICoordinate c);

        public abstract double GetDistance(IGeometry other);

        public abstract bool Contains(IEnvelope e);

        public abstract bool Contains(ICoordinate c);

        public bool IsEquals(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Equals);
        }

        public bool IsDisjoint(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Disjoint);
        }

        public bool IsTouches(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Touches);
        }

        public bool IsContains(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Contains);
        }

        public bool IsCovers(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Covers);
        }

        public bool IsIntersects(string other)
        {
            return IsIntersects(Read(other));
        }

        public bool IsIntersects(IEnvelope other)
        {
            return IsIntersects(other.ToPolygon());
        }

        public bool IsIntersects(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Intersects);
        }

        public bool IsWithin(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Within);
        }

        public bool IsCoveredBy(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.CoveredBy);
        }

        public bool IsCoveredBy(IGeometry other, double distance)
        {
            return OnRelate(this, other.AsGeometry().Buffer(distance), RelateOperations.CoveredBy);
        }

        public bool IsCrosses(IEnvelope bounds)
        {
            return IsCrosses(bounds.ToPolygon());
        }

        public bool IsCrosses(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Crosses);
        }

        public bool IsOverlaps(IGeometry other)
        {
            return OnRelate(AsGeometry(), other.AsGeometry(), RelateOperations.Overlaps);
        }

        public bool IntersectsWith(IGeometry other)
        {
            if (IsEmpty())
            {
                return false;
            }

            if (GetBounds().Intersects(other.GetBounds()))
            {
                return true;
            }

            return false;
        }

        public bool IntersectsWith(IEnvelope bounds)
        {
            if (IsEmpty())
            {
                return false;
            }

            if (GetBounds().Intersects(bounds))
            {
                return true;
            }

            return false;
        }

        protected virtual bool OnRelate(IGeometry g0, IGeometry g1, RelateOperations operation)
        {
            if (g0 == null || g1 == null)
            {
                return false;
            }

            if (RelateOp.Relate(out IntersectionMatrix matrix, g0, g1))
            {
                switch (operation)
                {
                    case RelateOperations.Contains:
                        return matrix.IsContains();

                    case RelateOperations.CoveredBy:
                        return matrix.IsCoveredBy();

                    case RelateOperations.Covers:
                        return matrix.IsCovers();

                    case RelateOperations.Crosses:
                        return matrix.IsCrosses(g0.GetDimension(), g1.GetDimension());

                    case RelateOperations.Disjoint:
                        return matrix.IsDisjoint();

                    case RelateOperations.Equals:
                        return matrix.IsEquals(g0.GetDimension(), g1.GetDimension());

                    case RelateOperations.Intersects:
                        return matrix.IsIntersects();

                    case RelateOperations.Overlaps:
                        return matrix.IsOverlaps(g0.GetDimension(), g1.GetDimension());

                    case RelateOperations.Touches:
                        return matrix.IsTouches(g0.GetDimension(), g1.GetDimension());

                    case RelateOperations.Within:
                        return matrix.IsWithin();
                }
            }

            return false;
        }

        public IGeometry Buffer(int distance)
        {
            return new BufferBuilder(BufferParameters).Buffer(this, distance);
        }

        public IGeometry Buffer(double distance)
        {
            return new BufferBuilder(BufferParameters).Buffer(this, distance);
        }

        public IGeometry Buffer(double width, double height)
        {
            return new BufferBuilder(BufferParameters).Buffer(this, width);
        }

        public IGeometry Rotate(double angle)
        {
            var transform = new MatrixTransform();
            transform.RotateAt(angle, GetCentroid());
            return Transform(transform);
        }

        public IGeometry RotateAt(double angle, ICoordinate point)
        {
            var transform = new MatrixTransform();
            transform.RotateAt(angle, point);
            return Transform(transform);
        }

        public IGeometry Transform(IMathTransform transform)
        {
            return OnTransform(transform);
        }

        protected virtual IGeometry OnTransform(IMathTransform transform)
        {
            if (transform.HasValue())
            {
                IGeometry g = Clone();

                g.Srid = transform.GetSrid();
                g.Coordinates.Transform(transform);

                return g;
            }

            return this;
        }

        public IGeometry Union(IGeometry other)
        {
            return Overlay(other, SpatialFunctions.Union);
        }

        public IGeometry Intersection(IGeometry other)
        {
            return Overlay(other, SpatialFunctions.Intersection);
        }

        public IGeometry Difference(IGeometry other)
        {
            return Overlay(other, SpatialFunctions.Difference);
        }

        public IGeometry Overlay(IGeometry other, SpatialFunctions function)
        {
            IGeometry g0 = AsGeometry();
            IGeometry g1 = other.AsGeometry();
            IGeometry r = OverlayOperation.Overlay(function, g0, g1);

            if (r == null || r.IsCollection())
            {
                r = OverlaySnapOperation.Overlay(function, g0, g1) ?? r;
            }

            return r;
        }

        public IGeometry AsGeometry()
        {
            return OnConvertToGeometry();
        }

        //public virtual IGeometry3D AsGeometry3D(IMapTransform transform)
        //{
        //    return new Geometry3D(AsGeometry(), transform);
        //}

        protected virtual IGeometry OnConvertToGeometry()
        {
            return this;
        }

        public GeometryValidationException Validate()
        {
            throw new NotImplementedException();
        }

        public virtual IGeometry MakeValid()
        {
            return this;
        }

        public abstract bool IsEquivalent(IGeometry other);

        public abstract Locations Locate(ICoordinate c);

        public static IGeometry Read(string wellKnownText)
        {
            return ReadInternal(Tokenizer.Parse(wellKnownText));
        }

        protected static IGeometry ReadInternal(ITokenEnumerator e)
        {
            if (e == null)
            {
                return default;
            }

            if (e.ReadWord(out WordToken typeName))
            {
                var g = (Geometry)GeometryFactory.Default.Create(string.Concat('I', typeName.StringValue));

                if (g == null)
                {
                    return Envelope.Read(e);
                }
                else
                {
                    try
                    {
                        g.BeginInit();
                        g.Read(e);
                        return g.MakeValid();
                    }
                    finally
                    {
                        g.EndInit();
                    }
                }
            }

            return default;
        }

        public static IGeometry Read(byte[] bytes)
        {
            if (bytes.HasValue())
            {
                using (var stream = new MemoryStream(bytes))
                {
                    return Read(stream);
                }
            }

            return default;
        }

        public static IGeometry Read(Stream input)
        {
            using (var reader = new BinaryReader(input))
            {
                return Read(reader);
            }
        }

        public static IGeometry Read(BinaryReader reader)
        {
            return GeometrySerializer.Read(reader);
        }

        public static double GetAngle(ICoordinateCollection coordinates)
        {
            return coordinates.StartPoint.Angle(coordinates.EndPoint);
        }

        public static double GetLength(ICoordinateCollection coordinates)
        {
            IList<ICoordinate> array = coordinates.Where(c => c.IsEmpty() == false).ToList();

            if (array.Count > 1)
            {
                double length = 0.0;

                for (int n = 1; n < array.Count; n++)
                {
                    length += array[n - 1].Distance(array[n]);
                }

                return length;
            }

            return 0.0;
        }

        public virtual object GetValue(GeometryProperty property)
        {
            if (!IsEmpty())
            {
                switch (property)
                {
                    case GeometryProperty.X:
                        return Coordinates.StartPoint.X;

                    case GeometryProperty.Y:
                        return Coordinates.StartPoint.Y;

                    case GeometryProperty.Altitude:
                        return Coordinates.GetAltitude();

                    case GeometryProperty.Area:
                        return GetArea();

                    case GeometryProperty.Angle:
                        return GetAngle();

                    case GeometryProperty.Azimuth:
                        return GetAzimuth();

                    case GeometryProperty.Length:
                    case GeometryProperty.Perimeter:
                        return GetLength();

                    //case GeometryProperty.NumPoints:
                    //return new GeometryContainer(this);

                    case GeometryProperty.WellKnownText:
                        return ToString();
                }
            }

            return null;
        }

        public virtual void SetValue(GeometryProperty property, object value)
        {
            switch (property)
            {
                case GeometryProperty.X:
                    Coordinates.StartPoint.X = (double)value;
                    break;

                case GeometryProperty.Y:
                    Coordinates.StartPoint.Y = (double)value;
                    break;

                case GeometryProperty.Altitude:
                    Coordinates.SetAltitude((double)value);
                    break;
            }
        }

        public abstract IEnumerable<PropertyDescriptor> GetProperties(IApplicationComponent component);

        public new IGeometry Clone()
        {
            return OnClone();
        }

        protected virtual IGeometry OnClone()
        {
            return base.Clone() as IGeometry;
        }

        public int CompareTo(object obj)
        {
            if (this != obj)
            {
                var other = obj as IGeometry;

                if (other.HasValue())
                {
                    return GetBounds().CompareTo(other.GetBounds());
                }

                return 1;
            }

            return 0;
        }

        public bool Equals(IGeometry g0, IGeometry g1)
        {
            return g0.IsEquivalent(g1);
        }

        public int GetHashCode(IGeometry g)
        {
            return g.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (IsEmpty())
            {
                return base.GetHashCode();
            }

            return Hash.Get(GetGeometries());
        }

        public Type LoadValue(ILGenerator e)
        {
            MethodInfo c = typeof(GeometryFactory).GetMethod("CreateGeometry", BindingFlags.Public | BindingFlags.Static, Types.String);

            e.Emit(OpCodes.Ldstr, ToString());
            e.Emit(OpCodes.Call, c);

            return typeof(IGeometry);
        }

        public string AsText()
        {
            return ToString();
        }

        public byte[] AsBinary()
        {
            throw new NotImplementedException();
            //using (var output = new MemoryStream())
            //{
            //    using (var writer = new BinaryWriter(output))
            //    {
            //        Write(writer);
            //    }

            //    output.Flush();
            //    output.Close();

            //    return output.ToArray();
            //}
        }

        public override string ToString()
        {
            return string.Concat(TypeName.ToUpperInvariant(), ToText());
        }

        public virtual IEnvelope GetBounds()
        {
            return Coordinates.GetBounds();
        }

        public virtual ICoordinate GetCentroid()
        {
            return OnCalculateCentroid();
        }

        public virtual double GetArea()
        {
            return Coordinates.GetArea();
        }

        public virtual double GetLength()
        {
            return Coordinates.GetLength();
        }

        public virtual double GetAngle()
        {
            return Coordinates.GetAngle();
        }

        public virtual double GetAzimuth()
        {
            return Coordinates.GetAzimuth();
        }

        public virtual IEnumerable<ILineSegment> GetSegments()
        {
            throw new NotImplementedException();
        }

        public virtual IApplicationComponent GetComponent(IApplicationComponentDesigner designer)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<ICoordinate> GetCoordinates()
        {
            return Coordinates;
        }

        public virtual bool Read(ITokenEnumerator e)
        {
            return Coordinates.Read(e);
        }

        protected virtual void OnWrite(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void AppendToPath(Graphics g, GraphicsPath path, Func<ICoordinate, PointF> transform)
        {
            AppendToPath(g, path, transform, false);
        }

        public abstract void AppendToPath(Graphics g, GraphicsPath path, Func<ICoordinate, PointF> transform, bool raiseEvents);

        public void Paint(Graphics g, PaintStyle style, Func<ICoordinate, PointF> transform)
        {
            Paint(g, style, transform, false);
        }

        public virtual void Paint(Graphics g, PaintStyle style, Func<ICoordinate, PointF> transform, bool raiseEvents)
        {
            using (var path = new GraphicsPath())
            {
                AppendToPath(g, path, transform, raiseEvents);

                if (path.PointCount > 0)
                {
                    OnFillPath(g, path, style);
                    OnDrawPath(g, path, style);
                }
            }
        }

        public virtual bool IsEquivalent(ICoordinateCollection other, int compareAt, ArrayComparison comparisonType)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnDrawPath(Graphics g, GraphicsPath path, PaintStyle style)
        {
            g.TryDrawPath(style.LineStyle.GetPen(), path);
        }

        protected virtual void OnFillPath(Graphics g, GraphicsPath path, PaintStyle style)
        {
            if (style.PolyStyle.Fill && IsClosed())
            {
                g.TryFillPath(style.PolyStyle.GetBrush(), path);
            }
        }

        public virtual ICoordinate StartPoint
        {
            get { return Coordinates.StartPoint; }
        }

        public virtual ICoordinate EndPoint
        {
            get { return Coordinates.EndPoint; }
        }

        protected virtual bool IsOnLine(ICoordinate c, double tolerance)
        {
            return Coordinates.IsOnLine(c, tolerance);
        }

        protected virtual ICoordinate OnCalculateCentroid()
        {
            return Coordinates.GetCentroid();
        }

        public virtual IEnumerable<ICoordinate> GetEditableCoordinates(IApplicationComponent component)
        {
            return Coordinates;
        }

        public virtual void SetAltitude(double value, bool append)
        {
            Coordinates.SetAltitude(value, append);
        }

        public virtual string ToText()
        {
            return Coordinates.ToText();
        }

        public virtual void SetObjects(object[] values)
        {
            throw new NotImplementedException();
        }

        public virtual void Normalize(bool clockwise)
        {
            Coordinates.Normalize(clockwise);
        }

        public virtual IEnumerable<ILineSegment> GetSegments(ICoordinate c)
        {
            return Coordinates.GetSegments(c);
        }

        public void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Scroll(ICoordinate point)
        {
            Coordinates.Scroll(point);
        }

        public ICoordinate GetPointMin()
        {
            return Coordinates.GetPointMin();
        }

        public bool IsClockwise()
        {
            return Coordinates.IsClockwise();
        }

        public void Reverse()
        {
            Coordinates.Reverse();
        }
    }
}
