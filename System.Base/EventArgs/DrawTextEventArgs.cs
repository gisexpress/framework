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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace System
{
    public class DrawTextEventArgs
    {
        public DrawTextEventArgs()
        {
            ScaleX = 1F;
            ScaleY = 1F;
            ForeColor = SystemColors.ControlText;
        }

        protected Font CurrentFont;
        protected FontFamily CurrentFontFamily;
        protected float? CurrentFontSize;
        protected FontStyle? CurrentFontStyle;
        protected TextFormatFlags Flags;

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Text) || (Origin.IsEmpty && Bounds.IsEmpty); }
        }

        public string Text
        {
            get;
            set;
        }

        public bool Multiline
        {
            get;
            set;
        }

        public Font Font
        {
            get { return CurrentFont ?? (CurrentFont = SystemFonts.DefaultFont); }
            set { CurrentFont = value; }
        }

        public Font GetFont()
        {
            return new Font(FontFamily, FontSize, FontStyle, GraphicsUnit.Point, 162);
        }

        public FontFamily FontFamily
        {
            get { return CurrentFontFamily ?? Font.FontFamily; }
            set { CurrentFontFamily = value; }
        }

        public float FontSize
        {
            get { return CurrentFontSize ?? Font.Size; }
            set { CurrentFontSize = value; }
        }

        public FontStyle FontStyle
        {
            get { return CurrentFontStyle ?? Font.Style; }
            set { CurrentFontStyle = value; }
        }

        public bool Shadow;

        public bool Outline;

        public PointF Origin;

        public Rectangle Bounds;

        public Color BackColor;

        public Color ForeColor;

        public ContentAlignment Alignment;

        public TextFormatFlags FormatFlags
        {
            get
            {
                //if (Multiline && DeveloperEnvironment.MonoFramework)
                //{
                //    return TextFormatFlags.WordBreak;
                //}

                switch (Alignment)
                {
                    case ContentAlignment.TopLeft:
                        Flags = TextFormatFlags.Top | TextFormatFlags.Left;
                        break;
                    case ContentAlignment.TopRight:
                        Flags = TextFormatFlags.Top | TextFormatFlags.Right;
                        break;
                    case ContentAlignment.TopCenter:
                        Flags = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                        break;
                    case ContentAlignment.BottomLeft:
                        Flags = TextFormatFlags.Bottom | TextFormatFlags.Left;
                        break;
                    case ContentAlignment.BottomRight:
                        Flags = TextFormatFlags.Bottom | TextFormatFlags.Right;
                        break;
                    case ContentAlignment.BottomCenter:
                        Flags = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                        break;
                    case ContentAlignment.MiddleLeft:
                        Flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                        break;
                    case ContentAlignment.MiddleRight:
                        Flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                        break;
                    case ContentAlignment.MiddleCenter:
                        Flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                        break;
                }

                if (Multiline)
                {
                    Flags |= TextFormatFlags.WordBreak;
                }
                else
                {
                    Flags |= TextFormatFlags.EndEllipsis;
                }

                return Flags;
            }
            set
            {
                Flags = value;
            }
        }

        public float ScaleX;

        public float ScaleY;

        public float Angle;

        public Pen CreatePen()
        {
            Pen pen = new Pen(Color.Empty) { LineJoin = LineJoin.Round };

            if (ForeColor.GetBrightness() >= .5F)
            {
                pen.Width = 3F;
                pen.Color = Color.FromArgb(200, Color.Black);
            }
            else
            {
                pen.Width = 2.4F;
                pen.Color = Color.FromArgb(60, Color.White);
            }

            return pen;
        }

        public Brush CreateBrush()
        {
            return new SolidBrush(ForeColor);
        }

        public void DrawPath(Graphics g, GraphicsPath path)
        {
            if (Outline)
            {
                using (Pen pen = CreatePen())
                {
                    g.DrawPath(pen, path);
                }
            }

            using (var brush = CreateBrush())
            {
                g.FillPath(brush, path);
            }
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
            //return Hash.Get(FontFamily, FontSize, FontStyle, ForeColor, Outline, Shadow);
        }

        public override bool Equals(object obj)
        {
            var other = obj as DrawTextEventArgs;

            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public DrawTextEventArgs Clone()
        {
            return (DrawTextEventArgs)MemberwiseClone();
        }
    }
}
