﻿//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright © GISExpress 2015-2017. All Rights Reserved.
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

using System.Geometries;

namespace System.Data.Dxf
{
    internal class AcDbEllipse : AcDbObject
    {
        public AcDbEllipse()
        {
            Center = GeometryFactory.CreateCoordinate();
            EndPoint = GeometryFactory.CreateCoordinate();
        }

        public ICoordinate Center
        {
            get;
            set;
        }

        /// <summary>
        /// Endpoint of major axis, relative to the center
        /// </summary>
        public ICoordinate EndPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Ratio of minor axis to major axis
        /// </summary>
        public double Ratio
        {
            get;
            set;
        }

        /// <summary>
        /// Start parameter (this value is 0.0 for a full ellipse)
        /// </summary>
        public double StartParameter
        {
            get;
            set;
        }

        /// <summary>
        /// End parameter (this value is 2pi for a full ellipse)
        /// </summary>
        public double EndParameter
        {
            get;
            set;
        }

        public override IGeometry GetGeometry()
        {
            var major = (EndPoint.X.Pow() + EndPoint.Y.Pow()).Sqrt();
            var minor = Ratio * major;

            var angle = Math.Atan2(EndPoint.Y, EndPoint.X).RadianToDegree();
            var isArc = StartParameter.Abs() > 1e-4 || (EndParameter - AppConstants.TwoPI).Abs() > 1e-4;

            IGeometry g = isArc ? GeometryFactory.CreateArc() : GeometryFactory.CreateEllipse();

            g.Add(Center.X - major, Center.Y - minor);
            g.Add(Center.X + major, Center.Y + minor);

            if (isArc)
            {
                var arc = g as IArc;

                arc.StartAngle = StartParameter;
                arc.SweepAngle = EndParameter - StartParameter;

                if (!Ratio.IsEquivalent(1.0))
                {
                    g = g.AsGeometry();
                }
            }

            if (angle.Abs() > 0.1)
            {
                return g.RotateAt(angle, Center);
            }

            return g;
        }

        protected override void OnRead(DxfReader reader)
        {
            switch (reader.GroupCode)
            {
                case 10:
                    Center.X = reader.GetDouble();
                    break;
                case 20:
                    Center.Y = reader.GetDouble();
                    break;
                case 30:
                    Center.Z = reader.GetDouble();
                    break;
                case 11:
                    EndPoint.X = reader.GetDouble();
                    break;
                case 21:
                    EndPoint.Y = reader.GetDouble();
                    break;
                case 31:
                    EndPoint.Z = reader.GetDouble();
                    break;
                case 40:
                    Ratio = reader.GetDouble();
                    break;
                case 41:
                    StartParameter = reader.GetDouble();
                    break;
                case 42:
                    EndParameter = reader.GetDouble();
                    break;
            }

            base.OnRead(reader);
        }
    }
}
