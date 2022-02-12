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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;

namespace System.Geometries
{
    internal class Rectangle : LinearRing, IRectangle
    {
        public Rectangle(XmlDocument doc) : base(Constants.Xml.Rectangle, doc)
        {
        }

        public Rectangle(string localName, XmlDocument doc) : base(localName, doc)
        {
        }

        public override bool IsRing()
        {
            return true;
        }

        public override bool IsRectangle()
        {
            return true;
        }

        public override string TypeName
        {
            get { return "Rectangle"; }
        }

        public override ICoordinate StartPoint
        {
            get
            {
                if (NumPoints() > 0)
                {
                    return Coordinates.Get(0);
                }

                return default(ICoordinate);
            }
        }

        public override ICoordinate EndPoint
        {
            get
            {
                if (NumPoints() > 1)
                {
                    return Coordinates.Get(1);
                }

                return default(ICoordinate);
            }
        }

        public double Width
        {
            get
            {
                if (NumPoints() > 1 && EndPoint.IsEmpty() == false)
                {
                    return (EndPoint.X - StartPoint.X).Abs();
                }

                return 0.0;
            }
            set
            {
                double d = Width;
                ICoordinate center = GetCentroid();
                StartPoint.X = center.X - d / 2.0;
                EndPoint.X = StartPoint.X + value;
            }
        }

        public double Height
        {
            get
            {
                if (NumPoints() > 1 && EndPoint.IsEmpty() == false)
                {
                    return (EndPoint.Y - StartPoint.Y).Abs();
                }

                return 0.0;
            }
            set
            {
                double d = Height;
                ICoordinate center = GetCentroid();
                StartPoint.Y = center.Y + d / 2.0;
                EndPoint.Y = StartPoint.Y - value;
            }
        }

        public override double GetArea()
        {
            return Width * Height;
        }

        public override double GetLength()
        {
            return (Width + Height) * 2.0;
        }

        public override ICoordinate GetCentroid()
        {
            ICoordinate c = LineSegment.GetMidPoint(StartPoint, EndPoint);

            c.SetAltitude(Coordinates.GetAltitude() / 2.0);

            return c;
        }

        public override IApplicationComponent GetComponent(IApplicationComponentDesigner designer)
        {
            return new RectangleComponent(designer, this);
        }

        public override void AppendToPath(Graphics g, GraphicsPath path, Func<ICoordinate, PointF> transform, bool raiseEvents)
        {
            if (NumPoints() > 1)
            {
                PointF p1 = transform(StartPoint);
                PointF p2 = transform(EndPoint);

                float minx = Math.Min(p1.X, p2.X);
                float miny = Math.Min(p1.Y, p2.Y);
                float maxx = Math.Max(p1.X, p2.X);
                float maxy = Math.Max(p1.Y, p2.Y);

                float width = maxx - minx;
                float height = maxy - miny;

                if (width >= 2F && height >= 2F && width < 1e+6 && height < 1e+6)
                {
                    OnAppendToPath(g, path, transform, minx, miny, width, height);

                    if (raiseEvents)
                    {
                        IEnumerator<ICoordinate> e = GetCoordinates().GetEnumerator();

                        if (e.MoveNext())
                        {
                            ICoordinate c = e.Current;

                            while (e.MoveNext())
                            {
                                if (PointVisitorSettings.Current.MidPoint)
                                {
                                    Coordinate.InvokeVisited(LineSegment.GetMidPoint(c, e.Current), transform, true, false);
                                }

                                if (PointVisitorSettings.Current.EndPoint)
                                {
                                    Coordinate.InvokeVisited(c, transform(c));
                                }

                                c = e.Current;
                            }

                            if (PointVisitorSettings.Current.Center)
                            {
                                Coordinate.InvokeVisited(GetCentroid(), transform, false, true);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnAppendToPath(Graphics g, GraphicsPath path, Func<ICoordinate, PointF> transform, float x, float y, float width, float height)
        {
            path.AddRectangle(x, y, width, height);
        }

        protected override IGeometry OnTransform(IMathTransform transform)
        {
            var g = AsGeometry() as IPolygon;
            g = g.Transform(transform) as IPolygon;

            if (g.IsRectangle())
            {
                var rect = Factory.Create<IRectangle>();
                var p0 = StartPoint.Clone();
                var p1 = EndPoint.Clone();

                transform.Transform(p0);
                transform.Transform(p1);

                rect.Coordinates.Add(p0);
                rect.Coordinates.Add(p1);

                return rect;
            }

            return g;
        }

        protected override IGeometry OnConvertToGeometry()
        {
            if (NumPoints() > 1)
            {
                return Factory.Create<IPolygon>(GetCoordinates());
            }

            return default;
        }

        public override IEnumerable<ICoordinate> GetCoordinates()
        {
            if (NumPoints() > 1)
            {
                ICoordinate c1 = StartPoint;
                ICoordinate c2 = EndPoint;

                yield return Coordinate.Create(c1.X, c1.Y, c1.Z);
                yield return Coordinate.Create(c2.X, c1.Y, (c1.Z + c2.Z) / 2.0);
                yield return Coordinate.Create(c2.X, c2.Y, c2.Z);
                yield return Coordinate.Create(c1.X, c2.Y, (c1.Z + c2.Z) / 2.0);
                yield return Coordinate.Create(c1.X, c1.Y, c1.Z);
            }
        }

        public override IEnumerable<ICoordinate> GetEditableCoordinates(IApplicationComponent component)
        {
            IEnumerator<ICoordinate> e = GetCoordinates().GetEnumerator();

            while (e.MoveNext())
            {
                yield return e.Current;
            }

            if (!component.IsDetached())
            {
                yield return GetCentroid();
            }
        }

        public override object GetValue(GeometryProperty property)
        {
            switch (property)
            {
                case GeometryProperty.Width:
                    return Width;
                case GeometryProperty.Height:
                    return Height;
            }

            return base.GetValue(property);
        }

        public override void SetValue(GeometryProperty property, object value)
        {
            switch (property)
            {
                case GeometryProperty.Width:
                    Width = (double)value;
                    break;
                case GeometryProperty.Height:
                    Height = (double)value;
                    break;
            }

            base.SetValue(property, value);
        }

        public override IEnumerable<ILineSegment> GetSegments()
        {
            return AsGeometry().GetSegments();
        }

        public override IEnumerable<PropertyDescriptor> GetProperties(IApplicationComponent component)
        {
            var readOnly = new ReadOnlyAttribute(true);
            var readOnly2 = new ReadOnlyAttribute(component.IsBusy() && !component.IsDetached());

            if (!component.IsBusy())
            {
                yield return GeometryFactory.CreatePropertyDescriptor(component, GeometryProperty.Altitude);
            }

            yield return GeometryFactory.CreatePropertyDescriptor(component, GeometryProperty.Area, readOnly);
            yield return GeometryFactory.CreatePropertyDescriptor(component, GeometryProperty.Perimeter, readOnly);
            yield return GeometryFactory.CreatePropertyDescriptor(component, GeometryProperty.Width, readOnly2);
            yield return GeometryFactory.CreatePropertyDescriptor(component, GeometryProperty.Height, readOnly2);
        }

        protected override void OnWrite(BinaryWriter writer)
        {
            AsGeometry().Write(writer);
        }

        public override string ToString()
        {
            if (NumPoints() > 1)
            {
                return AsGeometry().ToString();
            }

            return base.ToString();
        }
    }
}
