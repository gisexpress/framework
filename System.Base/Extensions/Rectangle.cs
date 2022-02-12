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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace System
{
    public static class RectangleExtensions
    {
        const float Shift = 2F;

        public static RectangleF Read(this RectangleF rect, Stream input)
        {
            float[] ltrb = input.ReadSingle(4).ToArray();
            
            return RectangleF.FromLTRB(ltrb[0], ltrb[1], ltrb[2], ltrb[3]);
        }

        public static void Write(this RectangleF rect, Stream output)
        {
            output.Write(new[] { rect.Left, rect.Top, rect.Right, rect.Bottom });
        }

        public static Rectangle ToRectangle(this RectangleF rect)
        {
            return new Rectangle(rect.Left.Floor(), rect.Top.Floor(), rect.Width.Ceiling(), rect.Height.Ceiling());
        }

        public static bool IsVisible(this RectangleF bounds, PointF point)
        {
            return (point.X > bounds.Left && point.X < bounds.Right) || (point.Y > bounds.Top && point.Y < bounds.Bottom);
        }

        public static RectangleF ExpandToInclude(this RectangleF rect, RectangleF other)
        {
            if (other.IsEmpty)
            {
                return rect;
            }

            if (rect.IsEmpty)
            {
                return other;
            }

            return RectangleF.FromLTRB(Math.Min(rect.Left, other.Left), Math.Min(rect.Top, other.Top), Math.Max(rect.Right, other.Right), Math.Max(rect.Bottom, other.Bottom));
        }

        public static void Validate(this RectangleF bounds, ref PointF point)
        {
            if (point.X < bounds.Left)
            {
                point.X = bounds.Left - Shift;
            }
            else if (point.X > bounds.Right)
            {
                point.X = bounds.Right + Shift;
            }

            if (point.Y < bounds.Top)
            {
                point.Y = bounds.Top - Shift;
            }
            else if (point.Y > bounds.Bottom)
            {
                point.Y = bounds.Bottom + Shift;
            }
        }

        public static Rectangle GetBounds(this Rectangle bounds, ContentAlignment alignment, Size size)
        {
            return GetBounds(bounds, alignment, size, Padding.Empty);
        }

        public static Rectangle GetBounds(this Rectangle bounds, ContentAlignment alignment, Size size, Padding padding)
        {
            var r = Rectangle.Empty;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    r = new Rectangle(padding.Left, padding.Top, size.Width, size.Height);
                    break;
                case ContentAlignment.TopRight:
                    r = new Rectangle(bounds.Width - size.Width - padding.Right, padding.Top, size.Width, size.Height);
                    break;
                case ContentAlignment.TopCenter:
                    r = new Rectangle((bounds.Width - size.Width) / 2, padding.Top, size.Width, size.Height);
                    break;
                case ContentAlignment.MiddleLeft:
                    r = new Rectangle(padding.Left, (bounds.Height - size.Height) / 2, size.Width, size.Height);
                    break;
                case ContentAlignment.MiddleRight:
                    r = new Rectangle(bounds.Width - size.Width - padding.Right, (bounds.Height - size.Height) / 2, size.Width, size.Height);
                    break;
                case ContentAlignment.MiddleCenter:
                    r = new Rectangle((bounds.Width - size.Width) / 2, (bounds.Height - size.Height) / 2, size.Width, size.Height);
                    break;
                case ContentAlignment.BottomLeft:
                    r = new Rectangle(padding.Left, bounds.Height - size.Height - padding.Bottom, size.Width, size.Height);
                    break;
                case ContentAlignment.BottomRight:
                    r = new Rectangle(bounds.Width - size.Width - padding.Right, bounds.Height - size.Height - padding.Bottom, size.Width, size.Height);
                    break;
                case ContentAlignment.BottomCenter:
                    r = new Rectangle((bounds.Width - size.Width) / 2, bounds.Height - size.Height - padding.Bottom, size.Width, size.Height);
                    break;
            }

            r.Offset(bounds.Left, bounds.Top);

            return r;
        }
    }
}
