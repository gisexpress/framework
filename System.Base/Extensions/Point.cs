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

using System.Drawing;
using System.IO;
using System.Linq;

namespace System
{
    public static class PointExtensions
    {
        public static bool IsEquivalent(this Point point, PointF other)
        {
            return point.X == (int)other.X && point.Y == (int)other.Y;
        }

        public static bool IsEquivalent(this PointF point, Point other)
        {
            return (int)point.X == other.X && (int)point.Y == other.Y;
        }

        public static int Distance(this Point point, Point other)
        {
            return ((point.X - other.X).Pow() + (point.Y - other.Y).Pow()).Sqrt();
        }

        public static float Distance(this PointF point, PointF other)
        {
            return ((point.X - other.X).Pow() + (point.Y - other.Y).Pow()).Sqrt();
        }

        public static float DistancePerpendicular(this PointF point, PointF p0, PointF p1)
        {
            return ((((p0.Y - point.Y) * (p1.X - p0.X)) - ((p0.X - point.X) * (p1.Y - p0.Y))) / (((p1.X - p0.X) * (p1.X - p0.X)) + ((p1.Y - p0.Y) * (p1.Y - p0.Y)))).Abs() * p0.Distance(p1);
        }

        public static PointF Read(this PointF point, Stream input)
        {
            float[] values = input.ReadSingle(2).ToArray();
            return new PointF(values[0], values[1]);
        }

        public static void Write(this PointF point, Stream output)
        {
            output.Write(new[] { point.X, point.Y });
        }
    }
}
