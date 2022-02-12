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

using System.Drawing;
using System.Geometries;

namespace System.Workspace.Drawing
{
    public class MapCanvas : IDisposable
    {
        public MapCanvas(MapWorkspace workspace)
        {
            Graphics = new MapGraphics(workspace);
        }

        public MapGraphics Graphics
        {
            get;
            protected set;
        }

        public bool KeepAlive
        {
            get { return Graphics.KeepAlive; }
            set { Graphics.KeepAlive = value; }
        }

        public int Width
        {
            get { return Graphics.Transform.ClientWidth; }
            set { Graphics.Transform.ClientWidth = value; }
        }

        public int Height
        {
            get { return Graphics.Transform.ClientHeight; }
            set { Graphics.Transform.ClientHeight = value; }
        }

        public void Clear()
        {
            Graphics.Clear();
        }

        public void Clear(Color color)
        {
            Graphics.Clear(color);
        }

        public bool Init(int width, int height)
        {
            return Graphics.Init(width, height);
        }

        public bool Normalize()
        {
            return Graphics.Normalize();
        }

        public bool ZoomToExtent()
        {
            return Graphics.ZoomToExtent();
        }

        public bool Zoom(int percent)
        {
            return Graphics.Zoom(percent);
        }

        public bool ZoomAt(int percent, PointF point)
        {
            return Graphics.ZoomAt(percent, point);
        }

        public bool ZoomTo(RectangleF rect)
        {
            return Graphics.ZoomTo(rect);
        }

        public bool ZoomTo(IEnvelope bounds)
        {
            return Graphics.ZoomTo(bounds);
        }

        public void Translate(PointF p1, PointF p2)
        {
            Graphics.Translate(p1, p2);
        }

        public void Draw(Graphics g)
        {
            Graphics.Draw(g);
        }

        public void Draw(Graphics g, float x, float y)
        {
            Graphics.Draw(g, x, y);
        }

        public DirectBitmap ToImage()
        {
            return ToImage(false);
        }

        public DirectBitmap ToImage(bool copy)
        {
            return Graphics.ToImage(copy);
        }

        public MapCanvas Clone()
        {
            var c = (MapCanvas)MemberwiseClone();
            c.Graphics = Graphics.Clone();
            return c;
        }

        public void Dispose()
        {
            Graphics.DisposeSafely();
            Graphics = null;
            GC.SuppressFinalize(this);
        }
    }
}