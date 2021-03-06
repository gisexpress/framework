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

using System.ComponentModel.Design;
using System.Drawing;

namespace System.Geometries
{
    internal class CircleComponent : RectangleComponent
    {
        public CircleComponent(IApplicationComponentDesigner designer, Circle value)
            : base(designer, value)
        {
            AllowAlignments = false;
        }

        protected ICoordinate Center;

        protected override bool IsClosed
        {
            get { return false; }
        }

        protected override void OnCoordinateBeginEdit(IApplicationComponent component, ICoordinate c)
        {
            if (!Detached)
            {
                Center = Value.GetCentroid();
            }
            else if (Value.NumPoints() > 1)
            {
                Center = Value.StartPoint.Clone();
            }

            base.OnCoordinateBeginEdit(component, c);
        }

        protected override void OnRectangleBeginEdit(ICoordinate c)
        {
            //Center = null;
        }

        protected override void OnCoordinatePaint(ApplicationComponentPaintEventArgs e)
        {
            if (Center.HasValue())
            {
                e.Path.Reset();
                e.Path.AddEllipse(Designer.Mouse.Location.X - 4F, Designer.Mouse.Location.Y - 4F, 8F, 8F);
                e.Graphics.DrawLine(ApplicationAppereance.Pens.SnapHotTrack, Designer.Transform.WorldToClient(Center), Designer.Mouse.Location);
            }
        }

        protected override bool OnValidateRect(ICoordinate location)
        {
            if (!base.OnValidateRect(location))
            {
                if (Value.NumPoints() > 1 && Center.HasValue())
                {
                    double radius = Center.Distance(location);

                    Value.StartPoint.SetValues(Center.X - radius, Center.Y - radius);
                    Value.EndPoint.SetValues(Center.X + radius, Center.Y + radius);

                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
