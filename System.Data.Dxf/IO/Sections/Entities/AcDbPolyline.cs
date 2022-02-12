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

using System.Collections.ObjectModel;
using System.Geometries;
using System.Linq;

namespace System.Data.Dxf
{
    internal class AcDbPolyline : AcDbObject
    {
        public AcDbPolyline()
        {
            Points = new Collection<ICoordinate>();
        }

        /// <summary>
        /// Number of vertices
        /// </summary>
        public int NumVertices
        {
            get;
            set;
        }

        /// <summary>
        /// Polyline flag
        /// </summary>
        public AcDbPolylineFlag Flag
        {
            get;
            set;
        }

        /// <summary>
        /// Curves and smooth surface type (optional; default = 0); integer codes, not bit-coded:
        /// 0 = No smooth surface fitted
        /// 5 = Quadratic B-spline surface
        /// 6 = Cubic B-spline surface
        /// 8 = Bezier surface
        /// </summary>
        public int SurfaceType
        {
            get;
            set;
        }

        /// <summary>
        /// Constant width (optional; default = 0). 
        /// Not used if variable width (codes 40 and/or 41) is set.
        /// </summary>
        public int ConstantWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Elevation (optional; default = 0)
        /// </summary>
        public int Elevation
        {
            get;
            set;
        }

        public int VertexIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Starting width (multiple entries; one entry for each vertex) (optional; default = 0; multiple entries). 
        /// Not used if constant width (code 43) is set.
        /// </summary>
        public double StartingWidth
        {
            get;
            set;
        }

        /// <summary>
        /// End width (multiple entries; one entry for each vertex) (optional; default = 0; multiple entries).
        /// Not used if constant width (code 43) is set.
        /// </summary>
        public double EndWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Bulge (multiple entries; one entry for each vertex) (optional; default = 0)
        /// </summary>
        public double Bulge
        {
            get;
            set;
        }

        public float Thickness
        {
            get;
            set;
        }

        public Collection<ICoordinate> Points
        {
            get;
            set;
        }

        public override IGeometry GetGeometry()
        {
            if (NumVertices > 0 && NumVertices == Points.Count)
            {
                if (Points.Count > 2 && (Flag & AcDbPolylineFlag.Closed) == AcDbPolylineFlag.Closed)
                {
                    return GeometryFactory.CreatePolygon(GeometryFactory.CreateLinearRing(Points.ToArray()));
                }
                else
                {
                    return GeometryFactory.CreateLineString(Points.ToArray());
                }
            }

            return null;
        }

        protected override void OnRead(DxfReader reader)
        {
            switch (reader.GroupCode)
            {
                case 10:
                    Points.Add(GeometryFactory.CreateCoordinate(reader.GetDouble(), 0.0));
                    break;
                case 20:
                    Points.Last().Y = reader.GetDouble();
                    break;
                case 30:
                    Points.Last().Z = reader.GetDouble();
                    break;
                case 38:
                    // Elevation (optional; default = 0)
                    break;
                case 39:
                    Thickness = reader.GetSingle();
                    break;
                case 40:
                    StartingWidth = reader.GetDouble();
                    break;
                case 41:
                    EndWidth = reader.GetDouble();
                    break;
                case 42:
                case 43:
                    // Constant width (optional; default = 0). Not used if variable width (codes 40 and/or 41) is set
                    break;
                case 66:
                    break;
                case 70:
                    Flag = (AcDbPolylineFlag)reader.GetByte();
                    break;
                case 71:
                    // Polygon mesh M vertex count (optional; default = 0)
                    break;
                case 72:
                    // Polygon mesh N vertex count (optional; default = 0)
                    break;
                case 73:
                    // Smooth surface M density (optional; default = 0)
                    break;
                case 74:
                    // Smooth surface N density (optional; default = 0)
                    break;
                case 75:
                    SurfaceType = reader.GetInt32();
                    break;
                case 90:
                    NumVertices = reader.GetInt32();
                    break;
                case 91:
                    VertexIdentifier = reader.GetInt32();
                    break;
            }

            base.OnRead(reader);
        }
    }
}