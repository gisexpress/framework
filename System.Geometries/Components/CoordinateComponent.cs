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
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Location = System.Drawing.Point;

namespace System.Geometries
{
    internal class CoordinateComponent : ApplicationComponent<ICoordinate>
    {
        public CoordinateComponent(IApplicationComponentDesigner designer, ICoordinate c)
            : base(designer, c)
        {
            PropertyX = GeometryFactory.CreatePropertyDescriptor(this, GeometryProperty.X);
            PropertyY = GeometryFactory.CreatePropertyDescriptor(this, GeometryProperty.Y);
            PropertyZ = GeometryFactory.CreatePropertyDescriptor(this, GeometryProperty.Altitude);

            if (c.IsEmpty())
            {
                return;
            }

            Original = new Coordinate(c.X, c.Y);
        }

        public override event ApplicationComponentPaintEventHandler Paint;

        protected Location CurrentLocation;
        protected readonly ICoordinate Original;
        protected readonly PropertyDescriptor PropertyX;
        protected readonly PropertyDescriptor PropertyY;
        protected readonly PropertyDescriptor PropertyZ;

        public override bool IsBusy()
        {
            return true;
        }

        public override void AddProperties(PropertyDescriptorCollection properties)
        {
            properties.Add(PropertyX);
            properties.Add(PropertyY);
            properties.Add(PropertyZ);

            if (Parent.HasValue())
            {
                Parent.AddProperties(properties);
            }
        }

        protected override void OnBeginEdit()
        {
            base.OnBeginEdit();

            if (Parent.HasValue() && Parent.AllowBeginValidation)
            {
                if (CurrentLocation.IsEmpty && Designer.Mouse.HasValue())
                {
                    MouseOver = true;
                    CurrentLocation = Designer.Mouse.Location;

                    Validate();
                    Designer.Flush();
                }
            }
        }

        protected override void OnValidate()
        {
            double x, y;

            var fixedX = (double?)Properties[GeometryProperty.X];
            var fixedY = (double?)Properties[GeometryProperty.Y];
            var fixedZ = (double?)Properties[GeometryProperty.Altitude];

            if (CurrentLocation.IsEmpty == false)
            {
                Designer.Transform.ClientToWorld(CurrentLocation.X, CurrentLocation.Y, out x, out y);
            }
            else if (Value.IsEmpty() == false)
            {
                x = Value.X;
                y = Value.Y;
            }

            if (fixedX.HasValue && fixedY.HasValue)
            {
                if (fixedZ.HasValue)
                {
                    Value.SetValues(fixedX.Value, fixedY.Value, fixedZ.Value);
                }
                else
                {
                    Value.SetValues(fixedX.Value, fixedY.Value);
                }
            }
            else
            {
                Designer.Transform.ClientToWorld(CurrentLocation.X, CurrentLocation.Y, out x, out y);

                if (fixedZ.HasValue)
                {
                    Value.SetValues(fixedX.HasValue ? fixedX.Value : x, fixedY.HasValue ? fixedY.Value : y, fixedZ.Value);
                }
                else
                {
                    Value.SetValues(fixedX.HasValue ? fixedX.Value : x, fixedY.HasValue ? fixedY.Value : y);
                }
            }

            base.OnValidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (MouseOver && Value.IsEmpty() == false)
            {
                int n = 0;
                PointF location = Designer.Transform.WorldToClient(Value);

                PaintArgs.Reset(e.Graphics);

                if (!ValidatingArgs.Cancel)
                {
                    PaintArgs.Path.AddEllipse(location.X - 4F, location.Y - 4F, 8F, 8F);
                    n = PaintArgs.Path.PointCount;
                }

                Paint.InvokeSafely(PaintArgs);

                if (!PaintArgs.Handled && PaintArgs.Path.PointCount > 0)
                {
                    if (PaintArgs.Pen.HasValue())
                    {
                        if (PaintArgs.Path.PointCount == n)
                        {
                            e.Graphics.TryFillPath(ApplicationAppereance.Brushes.ActivePoint, PaintArgs.Path);
                        }

                        e.Graphics.TryDrawPath(PaintArgs.Pen, PaintArgs.Path);
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!ValidatingArgs.Cancel)
            {
                if (!Designer.Keyboard.Control && e.Location.Equals(CurrentLocation))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        OnEndEdit(ComponentEditCompleteAction.Complete);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        OnEndEdit(ComponentEditCompleteAction.Cancel);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!e.Location.IsEmpty && (CurrentLocation.IsEmpty || !e.Location.Equals(CurrentLocation)))
            {
                CurrentLocation = e.Location;
                Validate();
                Designer.Flush();
            }

            base.OnMouseMove(e);
        }

        protected override object GetPropertyOwner(PropertyDescriptor descriptor)
        {
            var pd = descriptor as GeometryPropertyDescriptor;

            if (pd.HasValue() && (pd.PropertyKind == GeometryProperty.X || pd.PropertyKind == GeometryProperty.Y || pd.PropertyKind == GeometryProperty.Altitude))
            {
                return Value;
            }

            if (Parent.HasValue())
            {
                return Parent.GetPropertyOwner(descriptor);
            }

            return this;
        }

        protected override void OnEndEdit(ComponentEditCompleteAction action)
        {
            if (action == ComponentEditCompleteAction.Cancel)
            {
                if (Properties.Count > 0)
                {
                    Properties.Remove(Properties.Keys.Cast<object>().Last());
                    return;
                }

                if (Original.HasValue())
                {
                    CurrentLocation = Location.Empty;
                    Original.CopyTo(Value);
                }
            }

            base.OnEndEdit(action);
        }
    }
}
