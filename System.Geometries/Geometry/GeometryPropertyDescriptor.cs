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
using System.Diagnostics;
using System.Linq;

namespace System.Geometries
{
    internal class GeometryPropertyDescriptor : PropertyDescriptor<IGeometry>, IGeometryPropertyDescriptor, IApplicationComponentValidator
    {
        public GeometryPropertyDescriptor(IApplicationComponent component, GeometryProperty property, params Attribute[] attributes)
            : base(GetName(property), attributes)
        {
            Component = component;
            PropertyKind = property;
            Property.Category = GetCategory();
        }

        public int Priority
        {
            get { return 0; }
        }

        public IApplicationComponent Component
        {
            get;
            set;
        }

        public IUnitConverter UnitConverter
        {
            get { return Component.Designer.UnitConverter; }
        }

        public GeometryProperty PropertyKind
        {
            get;
            protected set;
        }

        public UnitTypes? PropertyUnitType
        {
            get
            {
                switch (PropertyKind)
                {
                    case GeometryProperty.Area:
                        return UnitTypes.Square;
                    case GeometryProperty.Angle:
                    case GeometryProperty.Azimuth:
                    case GeometryProperty.Rotation:
                    case GeometryProperty.StartAngle:
                    case GeometryProperty.SweepAngle:
                    case GeometryProperty.SegmentAngle1:
                    case GeometryProperty.SegmentAngle2:
                        return UnitTypes.Angular;
                    case GeometryProperty.Width:
                    case GeometryProperty.Height:
                    case GeometryProperty.Length:
                    case GeometryProperty.Radius:
                    case GeometryProperty.OffsetX:
                    case GeometryProperty.OffsetY:
                    case GeometryProperty.Buffer:
                    case GeometryProperty.BufferWidth:
                    case GeometryProperty.BufferHeight:
                    case GeometryProperty.Perimeter:
                    case GeometryProperty.SegmentLength1:
                    case GeometryProperty.SegmentLength2:
                    case GeometryProperty.Tolerance:
                        return UnitTypes.Linear;
                }

                return null;
            }
        }

        protected static string GetName(GeometryProperty property)
        {
            switch (property)
            {
                case GeometryProperty.X:
                    return "02C3863098B6461E92F7D70FAA74C64A";
                case GeometryProperty.Y:
                    return "AD8AB705E8184F89808741C368B90FCC";
                case GeometryProperty.Area:
                    return "D3510B4007D24DC8A61EC7314CBE135B";
                case GeometryProperty.Angle:
                    return "8E3EA7C57FB64F03A5A8E72AC4FAA431";
                case GeometryProperty.Azimuth:
                    return "19DEC5FD21F94F72B53A8C82DACD0D54";
                case GeometryProperty.Width:
                    return "ED1DEBEED318409D8506F83B69C3D6F9";
                case GeometryProperty.Height:
                    return "C94C65985DDC4689AF3A355950DCDD4C";
                case GeometryProperty.Radius:
                    return "B8E62A6B40394433AD1B746D9362413C";
                case GeometryProperty.Length:
                    return "05FBF85E967849D585A34E43199CBA4D";
                case GeometryProperty.Perimeter:
                    return "D9349B1AB9DF4E94BF278C5FD393B6E0";
                case GeometryProperty.Altitude:
                    return "BF908AFA2ED84A4DB0C18E54DD07D628";
                case GeometryProperty.StartAngle:
                    return "44FA5B3BF9F34536B08E8248E66A8AE6";
                case GeometryProperty.SweepAngle:
                    return "C5794260F9554ABE90A885EC839736E7";
                case GeometryProperty.Offset:
                    return "1CB2FF79DDBD4AACB09CB2CF11E1CAE0";
                case GeometryProperty.OffsetX:
                    return "F206328121954EBC8D7A8E5EDD6420A2";
                case GeometryProperty.OffsetY:
                    return "F46DBD1824404A67AA23B21DAF5DC118";
                case GeometryProperty.ScaleOffset:
                    return "909C5794BDD747BDA376F518787CCD13";
                case GeometryProperty.Scale:
                    return "3718CDA7978047ECA53C1623718C5799";
                case GeometryProperty.ScaleX:
                    return "F6B2E234E292438CB1F87D410C94155F";
                case GeometryProperty.ScaleY:
                    return "86D304CC247847DC89278E7344AFD9A0";
                case GeometryProperty.RotationOffset:
                    return "610537B3E7854E49B8DB1776A97E8F80";
                case GeometryProperty.Rotation:
                    return "549960DB1DA64168A52B7C31FA603503";
                case GeometryProperty.Buffer:
                    return "6E19472BAB6F42BF966FEF6C01507371";
                case GeometryProperty.BufferWidth:
                    return "1548C7BD8E7B40DBB583BF70571A7C84";
                case GeometryProperty.BufferHeight:
                    return "A6CBC58413674E64B15F7AA41BA71A6E";
                case GeometryProperty.BufferLineJoin:
                    return "FBBE57985DAD4295A28FCB4C54930DE5";
                case GeometryProperty.BufferEndCapStyle:
                    return "5B10CBC703BE4E83B82C93D6AC4C57B7";
                //case GeometryProperty.NumPoints:
                //    return "74B23A024CA04A0189EBDFD7E7006EF6";
                case GeometryProperty.SegmentAngle1:
                    return "502EDC367DE449AFA13F98E546F7E5C3";
                case GeometryProperty.SegmentAngle2:
                    return "4B55110BD69F4473B984345739DCD193";
                case GeometryProperty.SegmentLength1:
                    return "9B7C56C108B540AC8FFFA72AFBCC4824";
                case GeometryProperty.SegmentLength2:
                    return "A01B4C737369479DBDD1F2244ADC99C4";
                case GeometryProperty.Tolerance:
                    return "64BDBAE6BD5E4DFF9D0541C95E382FAC";
                case GeometryProperty.WellKnownText:
                    return "F24482996D2040EE9D666BF890A74D9D";
            }

            Debug.Fail("NotImplemented");

            return string.Empty;
        }

        protected string GetCategory()
        {
            switch (PropertyKind)
            {
                case GeometryProperty.X:
                case GeometryProperty.Y:
                case GeometryProperty.Altitude:
                    return Categories.Location;
                
                case GeometryProperty.Offset:
                case GeometryProperty.OffsetX:
                case GeometryProperty.OffsetY:
                    return Categories.Transformation.Translate;
                
                case GeometryProperty.ScaleOffset:
                case GeometryProperty.Scale:
                case GeometryProperty.ScaleX:
                case GeometryProperty.ScaleY:
                    return Categories.Transformation.Scale;

                case GeometryProperty.RotationOffset:
                case GeometryProperty.Rotation:
                    return Categories.Transformation.Rotate;
                
                case GeometryProperty.Buffer:
                case GeometryProperty.BufferWidth:
                case GeometryProperty.BufferHeight:
                case GeometryProperty.BufferLineJoin:
                case GeometryProperty.BufferEndCapStyle:
                    return Categories.Buffer;
                
                case GeometryProperty.SegmentAngle1:
                case GeometryProperty.SegmentLength1:
                    return Categories.LineSegment;
                
                case GeometryProperty.SegmentAngle2:
                case GeometryProperty.SegmentLength2:
                    return string.Concat(Localization.Localize(Categories.LineSegment), " (", Localization.Localize(Categories.Other), ")");
                
                case GeometryProperty.Tolerance:
                    return Categories.General;
                
                case GeometryProperty.WellKnownText:
                    return Categories.Debug;
            }

            return Categories.Measurements;
        }

        public override string Description
        {
            get
            {
                switch (PropertyKind)
                {
                    case GeometryProperty.Buffer:
                        return Localization.Localize("Buffer.Description");
                    case GeometryProperty.BufferWidth:
                    case GeometryProperty.BufferHeight:
                        return Localization.Localize("BufferWidthHeight.Description");
                    case GeometryProperty.BufferLineJoin:
                        return Localization.Localize("BufferLineJoin.Description");
                    case GeometryProperty.BufferEndCapStyle:
                        return Localization.Localize("BufferEndCapStyle.Description");
                }

                return base.Description;
            }
        }

        public override Type ComponentType
        {
            get { return typeof(IGeometry); }
        }

        public override Type PropertyType
        {
            get
            {
                switch (PropertyKind)
                {
                    //case GeometryProperty.NumPoints:
                    //    return Types.Int32;
                    case GeometryProperty.X:
                    case GeometryProperty.Y:
                    case GeometryProperty.Area:
                    case GeometryProperty.Angle:
                    case GeometryProperty.Azimuth:
                    case GeometryProperty.Width:
                    case GeometryProperty.Height:
                    case GeometryProperty.Radius:
                    case GeometryProperty.Length:
                    case GeometryProperty.Perimeter:
                    case GeometryProperty.Altitude:
                    case GeometryProperty.StartAngle:
                    case GeometryProperty.SweepAngle:
                    case GeometryProperty.OffsetX:
                    case GeometryProperty.OffsetY:
                    case GeometryProperty.Scale:
                    case GeometryProperty.ScaleX:
                    case GeometryProperty.ScaleY:
                    case GeometryProperty.Rotation:
                    case GeometryProperty.Buffer:
                    case GeometryProperty.BufferWidth:
                    case GeometryProperty.BufferHeight:
                    case GeometryProperty.SegmentAngle1:
                    case GeometryProperty.SegmentAngle2:
                    case GeometryProperty.SegmentLength1:
                    case GeometryProperty.SegmentLength2:
                    case GeometryProperty.Tolerance:
                        return Types.Double;
                    case GeometryProperty.Offset:
                    case GeometryProperty.ScaleOffset:
                    case GeometryProperty.RotationOffset:
                        return typeof(ICoordinate);
                    case GeometryProperty.BufferLineJoin:
                        return typeof(JoinStyle);
                    case GeometryProperty.BufferEndCapStyle:
                        return typeof(EndCapStyle);
                    case GeometryProperty.WellKnownText:
                        return Types.String;
                }

                return Types.Object;
            }
        }

        public override object GetEditor(Type editorBaseType)
        {
            object editor = null;

            switch (PropertyKind)
            {
                case GeometryProperty.Area:
                    editor = ApplicationEnvironment.CreateInstance("System.Windows.Forms.LinearValueEdit, System.Windows.Forms.Core", UnitConverter.GetUnitSymbol(UnitConverter.LinearDisplayUnit, UnitTypes.Square));
                    break;
                case GeometryProperty.Angle:
                case GeometryProperty.Azimuth:
                case GeometryProperty.Rotation:
                case GeometryProperty.StartAngle:
                case GeometryProperty.SweepAngle:
                case GeometryProperty.SegmentAngle1:
                case GeometryProperty.SegmentAngle2:
                    editor = ApplicationEnvironment.CreateInstance("System.Windows.Forms.AngularValueEdit, System.Windows.Forms.Core", UnitConverter.GetUnitSymbol(UnitConverter.AngularDisplayUnit, UnitTypes.Angular));
                    break;
                case GeometryProperty.Width:
                case GeometryProperty.Height:
                case GeometryProperty.Radius:
                case GeometryProperty.Length:
                case GeometryProperty.OffsetX:
                case GeometryProperty.OffsetY:
                case GeometryProperty.Perimeter:
                case GeometryProperty.Buffer:
                case GeometryProperty.BufferWidth:
                case GeometryProperty.BufferHeight:
                case GeometryProperty.SegmentLength1:
                case GeometryProperty.SegmentLength2:
                case GeometryProperty.Tolerance:
                    editor = ApplicationEnvironment.CreateInstance("System.Windows.Forms.LinearValueEdit, System.Windows.Forms.Core", UnitConverter.GetUnitSymbol(UnitConverter.LinearDisplayUnit, UnitTypes.Linear));
                    break;
                case GeometryProperty.Offset:
                    editor = ApplicationEnvironment.CreateInstance("System.Windows.Modules.Controls.DragOffsetPointEdit, System.Windows.Modules");
                    break;
                case GeometryProperty.ScaleOffset:
                    editor = ApplicationEnvironment.CreateInstance("System.Windows.Modules.Controls.ScaleOffsetPointEdit, System.Windows.Modules");
                    break;
                case GeometryProperty.Scale:
                case GeometryProperty.ScaleX:
                case GeometryProperty.ScaleY:
                    editor = ApplicationEnvironment.CreateInstance("System.Windows.Forms.PercentValueEdit, System.Windows.Forms.Core");
                    break;
                case GeometryProperty.RotationOffset:
                    editor = ApplicationEnvironment.CreateInstance("System.Windows.Modules.Controls.RotationOffsetPointEdit, System.Windows.Modules");
                    break;
            }

            if (editorBaseType.IsInstanceOfType(editor))
            {
                return editor;
            }

            return base.GetEditor(editorBaseType);
        }

        public override object GetValue(object component)
        {
            if (Component.HasValue())
            {
                var property = component as ISupportGeometryProperty;

                if (property.HasValue())
                {
                    object r;

                    switch (PropertyKind)
                    {
                        case GeometryProperty.Azimuth:
                        case GeometryProperty.Rotation:
                            r = ((double)property.GetValue(PropertyKind)).DegreeToRadian();
                            break;
                        
                        case GeometryProperty.SegmentAngle1:
                        case GeometryProperty.SegmentAngle2:
                        case GeometryProperty.SegmentLength1:
                        case GeometryProperty.SegmentLength2:
                            r = GetSegmentValue(component as IGeometry, Component.Value as ICoordinate);
                            break;

                        default:
                            r = property.GetValue(PropertyKind);
                            break;
                    }

                    if (!r.IsNullOrDBNull())
                    {
                        UnitTypes? target = PropertyUnitType;

                        if (target.HasValue)
                        {
                            return UnitConverter.ConvertToDisplayUnit((double)r, target.Value);
                        }

                        return r;
                    }
                }
            }

            return default(object);
        }

        protected object GetSegmentValue(IGeometry g, ICoordinate c)
        {
            if (g.HasValue() && c.HasValue())
            {
                var segment1 = default(ILineSegment);
                var segment2 = default(ILineSegment);

                foreach (ILineSegment segment in g.Coordinates.GetSegments(c))
                {
                    if (segment1 == null)
                    {
                        segment1 = segment;
                    }
                    else
                    {
                        segment2 = segment;
                    }
                }

                switch (PropertyKind)
                {
                    case GeometryProperty.SegmentAngle1:
                        return segment1 == null ? 0.0 : segment1.GetAngle();
                    case GeometryProperty.SegmentAngle2:
                        return segment2 == null ? 0.0 : segment2.GetAngle();
                    case GeometryProperty.SegmentLength1:
                        return segment1 == null ? 0.0 : segment1.GetLength();
                    case GeometryProperty.SegmentLength2:
                        return segment2 == null ? 0.0 : segment2.GetLength();
                }
            }

            return null;
        }

        public override void SetValue(object component, object value)
        {
            if (Component.HasValue() && value.HasValue())
            {
                var property = component as ISupportGeometryProperty;

                if (property.HasValue())
                {
                    var c = component as ICoordinate;

                    if (c.HasValue() && (PropertyKind == GeometryProperty.X || PropertyKind == GeometryProperty.Y || PropertyKind == GeometryProperty.Altitude))
                    {
                        Component.Properties[PropertyKind] = value;
                        Component.Validate();

                        if (Component.Properties.ContainsKey(GeometryProperty.X) && Component.Properties.ContainsKey(GeometryProperty.Y))
                        {
                            Component.Designer.Transform.SetCenter(c.X, c.Y);
                            Component.EndEdit(ComponentEditCompleteAction.Complete);
                        }

                        Component.Designer.Redraw();
                    }
                    else
                    {
                        if (value is double)
                        {
                            var isCompleted = false;
                            var sourceUnitValue = ConvertToSourceUnit(PropertyKind, (double)value);

                            if (Component.IsBusy())
                            {
                                switch (PropertyKind)
                                {
                                    case GeometryProperty.Width:
                                    case GeometryProperty.Height:
                                    case GeometryProperty.Length:
                                    case GeometryProperty.SegmentLength1:
                                    case GeometryProperty.SegmentLength2:
                                    case GeometryProperty.SegmentAngle1:
                                    case GeometryProperty.SegmentAngle2:
                                        Component.Properties[PropertyKind] = sourceUnitValue;
                                        break;
                                }

                                switch (PropertyKind)
                                {
                                    case GeometryProperty.Radius:
                                        isCompleted = true;
                                        property.SetValue(GeometryProperty.Radius, sourceUnitValue);
                                        break;
                                    case GeometryProperty.Width:
                                    case GeometryProperty.Height:
                                        Component.Validating -= ValidateRect;
                                        Component.Validating += ValidateRect;
                                        Component.Validate();
                                        break;
                                    case GeometryProperty.SegmentAngle1:
                                    case GeometryProperty.SegmentAngle2:
                                        Component.Validating -= ValidateAngle;
                                        Component.Validating += ValidateAngle;
                                        Component.Validate();
                                        break;
                                    case GeometryProperty.Length:
                                    case GeometryProperty.SegmentLength1:
                                    case GeometryProperty.SegmentLength2:
                                        Component.Validating -= ValidateLength;
                                        Component.Validating += ValidateLength;
                                        Component.Validate();
                                        break;
                                }

                                isCompleted |= Component.Properties.ContainsKey(GeometryProperty.Angle) && Component.Properties.ContainsKey(GeometryProperty.Length);
                                isCompleted |= Component.Properties.ContainsKey(GeometryProperty.SegmentAngle1) && Component.Properties.ContainsKey(GeometryProperty.SegmentLength1);
                                isCompleted |= Component.Properties.ContainsKey(GeometryProperty.SegmentAngle2) && Component.Properties.ContainsKey(GeometryProperty.SegmentLength2);
                                isCompleted |= Component.Properties.ContainsKey(GeometryProperty.Width) && Component.Properties.ContainsKey(GeometryProperty.Height);

                                if (isCompleted)
                                {
                                    Component.EndEdit(ComponentEditCompleteAction.Complete);
                                }
                            }
                            else
                            {
                                property.SetValue(PropertyKind, sourceUnitValue);

                                switch (PropertyKind)
                                {
                                    case GeometryProperty.StartAngle:
                                    case GeometryProperty.SweepAngle:
                                        isCompleted = true;
                                        break;
                                }

                                if (Component.IsStartedEditing())
                                {
                                    Component.Designer.Flush();
                                }
                                else
                                {
                                    Component.Designer.Redraw();
                                }
                            }
                        }
                        else
                        {
                            property.SetValue(PropertyKind, value);
                        }
                    }
                }
            }
        }

        protected double ConvertToSourceUnit(GeometryProperty property, double value)
        {
            switch (property)
            {
                case GeometryProperty.X:
                case GeometryProperty.Y:
                case GeometryProperty.Altitude:
                    return value;
                case GeometryProperty.Azimuth:
                case GeometryProperty.Rotation:
                    return Component.Designer.UnitConverter.ConvertToSourceUnit(value, UnitTypes.Angular).RadianToDegree();
                case GeometryProperty.Area:
                    return Component.Designer.UnitConverter.ConvertToSourceUnit(value, UnitTypes.Square);
                case GeometryProperty.Angle:
                case GeometryProperty.SegmentAngle1:
                case GeometryProperty.SegmentAngle2:
                    return Component.Designer.UnitConverter.ConvertToSourceUnit(value, UnitTypes.Angular);
                default:
                    return Component.Designer.UnitConverter.ConvertToSourceUnit(value, UnitTypes.Linear);
            }
        }

        protected void ValidateAngle(ApplicationComponentEventArgs e)
        {
            if (PropertyKind == GeometryProperty.Angle || PropertyKind == GeometryProperty.SegmentAngle1 || PropertyKind == GeometryProperty.SegmentAngle2)
            {
                var angle = (double?)Component.Properties[PropertyKind];

                if (angle.HasValue)
                {
                    var c = e.Component.Value as ICoordinate;
                    var g = e.Component.Parent.Value as IGeometry;

                    if (c.HasValue() && g.HasValue())
                    {
                        double distance;
                        ICoordinate other;
                        ILineSegment segment;

                        switch (PropertyKind)
                        {
                            case GeometryProperty.SegmentAngle2:
                                angle = (angle + 180) % 360;
                                segment = g.Coordinates.GetSegments(c).Last();
                                break;
                            default:
                                segment = g.Coordinates.GetSegments(c).First();
                                break;
                        }

                        if (segment.P0 == c)
                        {
                            other = segment.P1;
                        }
                        else
                        {
                            other = segment.P0;
                        }

                        distance = ((c.X - other.X).Pow() + (c.Y - other.Y).Pow()).Sqrt();

                        c.SetValues(other.X + distance * Math.Cos(angle.Value), other.Y + distance * Math.Sin(angle.Value));
                    }
                }
            }
        }

        protected void ValidateLength(ApplicationComponentEventArgs e)
        {
            if (PropertyKind == GeometryProperty.Length || PropertyKind == GeometryProperty.SegmentLength1 || PropertyKind == GeometryProperty.SegmentLength2)
            {
                var length = (double?)Component.Properties[PropertyKind];

                if (length.HasValue)
                {
                    var c = e.Component.Value as ICoordinate;
                    var g = e.Component.Parent.Value as IGeometry;

                    if (c.HasValue() && g.HasValue())
                    {
                        double angle;
                        ICoordinate other;
                        ILineSegment segment;

                        switch (PropertyKind)
                        {
                            case GeometryProperty.SegmentLength2:
                                segment = g.Coordinates.GetSegments(c).Last();
                                break;
                            default:
                                segment = g.Coordinates.GetSegments(c).First();
                                break;
                        }

                        if (segment.P0 == c)
                        {
                            other = segment.P1;
                        }
                        else
                        {
                            other = segment.P0;
                        }

                        angle = c.Angle(other);

                        c.SetValues(other.X - (length.Value * Math.Cos(angle)), other.Y - (length.Value * Math.Sin(angle)));
                    }
                }
            }
        }

        protected void ValidateRect(ApplicationComponentEventArgs e)
        {
            if (Component.Parent.HasValue() && Component.Parent.IsDetached())
            {
                var width = (double?)Component.Properties[GeometryProperty.Width];
                var height = (double?)Component.Properties[GeometryProperty.Height];

                if (width.HasValue || height.HasValue)
                {
                    var c = e.Component.Value as ICoordinate;

                    if (c.HasValue() && e.Component.Parent.HasValue())
                    {
                        var g = e.Component.Parent.Value as IRectangle;

                        if (c.HasValue() && g.HasValue())
                        {
                            ICoordinate other = g.Coordinates.IndexOf(c) == 0 ? g.Coordinates.EndPoint : g.Coordinates.StartPoint;

                            if (other.HasValue())
                            {
                                double x = width.HasValue ? other.X + (((c.X - other.X) < 0.0 ? -1.0 : 1.0) * width.Value) : c.X;
                                double y = height.HasValue ? other.Y + (((c.Y - other.Y) < 0.0 ? -1.0 : 1.0) * height.Value) : c.Y;

                                c.SetValues(x, y);
                            }
                        }
                    }
                }
            }
        }

        public int CompareTo(object other)
        {
            return CompareTo(other as IApplicationComponentValidator);
        }

        public int CompareTo(IApplicationComponentValidator other)
        {
            return Priority.CompareTo(other.HasValue() ? other.Priority : int.MaxValue);
        }
    }
}