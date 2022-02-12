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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace System.Geometries
{
    internal class GeometryComponent<TValue> : ApplicationComponent<TValue> where TValue : IGeometry
    {
        const float EditablePointRadius = 4F;
        const float EditablePointDiameter = EditablePointRadius * 2;
        const float EditableCenterPointRadius = 3F;

        protected GeometryComponent(IApplicationComponentDesigner designer, TValue value)
            : base(designer, value)
        {
            Detached = Value.IsEmpty();
        }

        public override event ApplicationComponentPaintEventHandler Paint;

        public IApplicationComponent ActivePoint;

        public override bool IsBusy()
        {
            return ActivePoint.HasValue();
        }

        public override object ValueOwner
        {
            get { return Value.GetCollection() as IGeometry ?? Value; }
        }

        protected virtual bool Fill
        {
            get { return ReferenceEquals(Value, ValueOwner); }
        }

        protected virtual bool IsClosed
        {
            get { return false; }
        }

        protected virtual bool IsValid(TValue g)
        {
            return g.NumPoints() > 1;
        }

        protected override void OnBeginEdit()
        {
            base.OnBeginEdit();

            if (Detached)
            {
                OnCoordinateBeginEdit(Value.Coordinates.Add());
            }
        }

        protected virtual void OnBeginEditing()
        {
            Started = true;
        }

        protected void OnCoordinateBeginEdit(ICoordinate c)
        {
            if (c.IsEmpty() && Value.NumPoints() > 0)
            {
                ICoordinate endPoint = Value.Coordinates.LastOrDefault(item => item.IsEmpty() == false);

                if (endPoint.HasValue())
                {
                    endPoint.CopyTo(c);
                }
            }

            OnCoordinateBeginEdit(c.GetComponent(Designer), c);
        }

        protected virtual void OnCoordinateBeginEdit(IApplicationComponent component, ICoordinate c)
        {
            ActivePoint = component;

            component.Parent = this;
            component.AllowAlignments = AllowAlignments;
            component.AllowProperties = AllowProperties;

            component.Paint += Paint;
            component.Paint += OnCoordinatePaint;
            component.Validating += OnCoordinateValidating;
            component.EditCompleted += CoordinateEditCompleted;

            Designer.BeginEdit(component);
        }

        protected virtual void OnCoordinatePaint(ApplicationComponentPaintEventArgs e)
        {
        }

        protected virtual void OnCoordinateValidating(ApplicationComponentEventArgs e)
        {
            //var c = e.Component.Value as ICoordinate;

            //if (!e.Cancel && Value.Count > 1)
            //{
            //    e.Cancel = Value.IsOnLine(c, e.Designer.Transform.PixelSize, segment => segment.P0 != c && segment.P1 != c);
            //}
        }

        protected virtual void OnCoordinateEndEdit(ApplicationComponentEditCompletedEventArgs e)
        {
            if (Detached)
            {
                if (e.Action == ComponentEditCompleteAction.Complete)
                {
                    OnCoordinateBeginEdit(Value.Coordinates.Add());
                }
                else if (e.Action == ComponentEditCompleteAction.Cancel)
                {
                    Detached = false;
                    Value.Coordinates.Remove(Value.Coordinates.EndPoint);
                }
            }
        }

        protected void CoordinateEditCompleted(ApplicationComponentEditCompletedEventArgs e)
        {
            ActivePoint.Paint -= Paint;
            ActivePoint.Paint -= OnCoordinatePaint;
            ActivePoint.Validating -= OnCoordinateValidating;
            ActivePoint.EditCompleted -= CoordinateEditCompleted;

            OnCoordinateEditCompleted(e);
        }

        protected virtual void OnCoordinateEditCompleted(ApplicationComponentEditCompletedEventArgs e)
        {
            ActivePoint = null;

            //Value.EndInit();
            OnCoordinateEndEdit(e);

            if (e.Action == ComponentEditCompleteAction.Cancel && !AllowProperties)
            {
                OnEndEdit(ComponentEditCompleteAction.Complete);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Value.IsEmpty())
            {
                return;
            }
            
            PaintArgs.Reset(e.Graphics);
            OnAppendToPath(e, ValueOwner as IGeometry);
            Paint.InvokeSafely(PaintArgs);

            if (PaintArgs.Path.PointCount > 0)
            {
                if (Designer.ActiveComponent != this && Designer.ActiveComponent != ActivePoint)
                {
                    if (Detached)
                    {
                        ICoordinate endPoint;

                        if ((endPoint = Value.Coordinates.EndPoint).IsEmpty() == false)
                        {
                            endPoint.SetNull();
                        }
                    }
                    else if (ActivePoint.HasValue())
                    {
                        IApplicationComponent c = ActivePoint;
                        ActivePoint = null;
                        c.EndEdit(ComponentEditCompleteAction.Cancel);
                    }
                }

                if (Detached)
                {
                    OnAppendCurrentPointToPath(e, Designer.PointToClient(Cursor.Position));
                }

                OnDrawPath(e);
                OnDrawCenterPoint(e, OnDrawEditablePoints(e));
            }
        }

        protected virtual void OnAppendToPath(PaintEventArgs e, IGeometry geometry)
        {
            geometry.AppendToPath(e.Graphics, PaintArgs.Path, Designer.Transform.WorldToClient, false);
        }

        protected virtual void OnAppendCurrentPointToPath(PaintEventArgs e, PointF point)
        {
            ICoordinate c;

            if (PaintArgs.Path.PointCount == 0 && !(c = Value.Coordinates.StartPoint).IsEmpty())
            {
                PaintArgs.Path.StartFigure();
                PaintArgs.Path.AddLine(Designer.Transform.WorldToClient(c), Designer.PointToClient(Cursor.Position));
            }
        }

        protected virtual void OnDrawPath(PaintEventArgs e)
        {
            if (PaintArgs.Pen.HasValue())
            {
                e.Graphics.TryDrawPath(PaintArgs.Pen, PaintArgs.Path);
            }
        }

        protected virtual IEnumerable<ICoordinate> GetEditableCoordinates()
        {
            return Value.Coordinates.GetEditableCoordinates(this);
        }

        protected virtual RectangleF OnDrawEditablePoints(PaintEventArgs e)
        {
            using (var path = new GraphicsPath(FillMode.Winding))
            {
                IEnumerator<ICoordinate> points = GetEditableCoordinates().GetEnumerator();

                if (points.MoveNext())
                {
                    RectangleF bounds = e.Graphics.VisibleClipBounds;

                    PointF p1 = Designer.Transform.WorldToClient(points.Current);
                    PointF p2;

                    float minx = bounds.Width, miny = bounds.Height, maxx = 0, maxy = 0;

                    if (bounds.IsVisible(p1))
                    {
                        minx = p1.X;
                        miny = p1.Y;
                        maxx = p1.X;
                        maxy = p1.Y;

                        path.AddEllipse(p1.X - EditablePointRadius, p1.Y - EditablePointRadius, EditablePointDiameter, EditablePointDiameter);
                    }

                    while (points.MoveNext())
                    {
                        ICoordinate item = points.Current;

                        if (!item.IsEmpty())
                        {
                            p2 = Designer.Transform.WorldToClient(item);

                            if (bounds.IsVisible(p2) && p1.Distance(p2) > EditablePointDiameter)
                            {
                                p1 = p2;

                                minx = Math.Min(minx, p1.X);
                                miny = Math.Min(miny, p1.Y);
                                maxx = Math.Max(maxx, p1.X);
                                maxy = Math.Max(maxy, p1.Y);

                                path.AddEllipse(p1.X - EditablePointRadius, p1.Y - EditablePointRadius, EditablePointDiameter, EditablePointDiameter);
                            }
                        }
                    }

                    if (path.PointCount > 0)
                    {
                        e.Graphics.TryFillPath(ApplicationAppereance.Brushes.Point, path);
                        e.Graphics.TryDrawPath(ApplicationAppereance.Pens.ActiveBorder, path);

                        return new RectangleF(minx, miny, maxx - minx, maxy - miny);
                    }
                }
            }

            return RectangleF.Empty;
        }

        protected virtual void OnDrawCenterPoint(PaintEventArgs e, RectangleF bounds)
        {
            if (Value.IsRing())
            {
                ICoordinate centroid = Value.GetCentroid();

                if (centroid.HasValue())
                {
                    PointF center = Designer.Transform.WorldToClient(centroid);

                    if (bounds.IsEmpty || bounds.IsVisible(center))
                    {
                        using (var path = new GraphicsPath())
                        {
                            path.StartFigure();
                            path.AddLine(center.X - EditableCenterPointRadius, center.Y, center.X + EditableCenterPointRadius, center.Y);

                            path.StartFigure();
                            path.AddLine(center.X, center.Y - EditableCenterPointRadius, center.X, center.Y + EditableCenterPointRadius);

                            e.Graphics.TryDrawPath(ApplicationAppereance.Pens.Border, path);
                        }
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Designer.Keyboard.Control)
            {
                if (!Detached)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (ActivePoint.IsNull())
                        {
                            OnEditCoordinate();
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        OnEndEdit(ComponentEditCompleteAction.Complete);
                    }
                }
            }
        }

        protected virtual void OnEditCoordinate()
        {
            ICoordinate point = GetEditableCoordinates().FirstOrDefault(c => c.IsEmpty() == false && Designer.GetCursorBounds(5).Contains(c));

            if (point.HasValue())
            {
                Value.BeginInit();

                if (!Detached && !IsStartedEditing())
                {
                    OnBeginEditing();
                }

                OnCoordinateBeginEdit(point);
            }
        }

        public override void AddProperties(PropertyDescriptorCollection properties)
        {
            if (IsValid(Value))
            {
                foreach (var item in Value.GetProperties(this).Cast<PropertyDescriptor>())
                {
                    properties.Add(item);
                }
            }
        }

        protected override object GetPropertyOwner(PropertyDescriptor descriptor)
        {
            IGeometryPropertyDescriptor geometryDescriptor;

            if ((geometryDescriptor = descriptor as IGeometryPropertyDescriptor).HasValue())
            {
                if (ActivePoint.HasValue())
                {
                    geometryDescriptor.Component = ActivePoint;
                }

                return Value;
            }

            return base.GetPropertyOwner(descriptor);
        }

        protected override void OnDispose()
        {
            ActivePoint.DisposeSafely();
            ActivePoint = null;
            base.OnDispose();
        }
    }
}
