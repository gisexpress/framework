//////////////////////////////////////////////////////////////////////////////////////////////////
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

using System.Collections.Generic;
using System.Geometries;
using System.Transformations;

namespace System.Data.Dxf
{
    internal class AcDbBlockReference : AcDbObject
    {
        public AcDbBlockReference()
        {
            ScaleX = 1.0;
            ScaleY = 1.0;
            ScaleZ = 1.0;
            RowCount = 1;
            ColumnCount = 1;
            InsertionPoint = GeometryFactory.CreateCoordinate();
        }

        public string Name
        {
            get;
            set;
        }

        public ICoordinate InsertionPoint
        {
            get;
            set;
        }

        /// <summary>
        /// X scale factor (optional; default = 1)
        /// </summary>
        public double ScaleX
        {
            get;
            set;
        }

        /// <summary>
        /// Y scale factor (optional; default = 1)
        /// </summary>
        public double ScaleY
        {
            get;
            set;
        }

        /// <summary>
        /// Z scale factor (optional; default = 1)
        /// </summary>
        public double ScaleZ
        {
            get;
            set;
        }

        public double RotationAngle
        {
            get;
            set;
        }

        public int RowCount
        {
            get;
            set;
        }

        public int ColumnCount
        {
            get;
            set;
        }

        public int RowSpacing
        {
            get;
            set;
        }

        public int ColumnSpacing
        {
            get;
            set;
        }

        public override IGeometry GetGeometry()
        {
            DxfBlock value;

            if (Document.Blocks.TryGetValue(Name, out value) && value.Objects.Count > 0)
            {
                var transform = new MatrixTransform();
                var geometries = new List<IGeometry>(value.Objects.Count);

                foreach (AcDbObject item in value.Objects)
                {
                    IGeometry g = item.GetGeometry();

                    if (g.HasValue())
                    {
                        geometries.Add(g);
                    }
                }

                if (geometries.Count > 0)
                {
                    transform.Translate(InsertionPoint.X, InsertionPoint.Y);
                    transform.Rotate(RotationAngle);
                    transform.Scale(ScaleX, ScaleY);

                    return GeometryFactory.BuildGeometry(geometries.ToArray()).Transform(transform);
                }
            }

            return null;
        }

        protected override void OnRead(DxfReader reader)
        {
            switch (reader.GroupCode)
            {
                case 2:
                    Name = reader.GetString();
                    break;
                case 10:
                    InsertionPoint.X = reader.GetDouble();
                    break;
                case 20:
                    InsertionPoint.Y = reader.GetDouble();
                    break;
                case 30:
                    InsertionPoint.Z = reader.GetDouble();
                    break;
                case 41:
                    ScaleX = reader.GetDouble();
                    break;
                case 42:
                    ScaleY = reader.GetDouble();
                    break;
                case 43:
                    ScaleY = reader.GetDouble();
                    break;
                case 44:
                    ColumnSpacing = reader.GetInt32();
                    break;
                case 45:
                    RowSpacing = reader.GetInt32();
                    break;
                case 50:
                    RotationAngle = reader.GetDouble();
                    break;
                case 66:
                    // Variable attributes-follow flag (optional; default = 0); 
                    // if the value of attributes-follow flag is 1, a series of attribute entities is expected to follow the insert, terminated by a seqend entity.
                    break;
                case 70:
                    ColumnCount = reader.GetInt32();
                    break;
                case 71:
                    RowCount = reader.GetInt32();
                    break;
            }

            base.OnRead(reader);
        }
    }
}
