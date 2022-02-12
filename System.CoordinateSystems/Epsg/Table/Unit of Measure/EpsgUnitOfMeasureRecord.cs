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

using System.Collections.Generic;
using System.Data;

namespace System.CoordinateSystems.Epsg
{
    internal class EpsgUnitOfMeasureRecord : EpsgRecord
    {
        public override object GetKey()
        {
            return GetCode();
        }

        /// <summary>
        /// Unique code (integer) of the unit of measure; primary key.
        /// </summary>
        public int GetCode()
        {
            return (int)base["Code"];
        }

        /// <summary>
        /// Name of the unit of measure.
        /// </summary>
        public string GetUnitName()
        {
            return (string)base["UnitName"];
        }

        public EpsgUnit GetUnit()
        {
            return (EpsgUnit)GetCode();
        }

        public new EpsgUnitOfMeasureKind GetType()
        {
            return Enums.Parse<EpsgUnitOfMeasureKind>(GetTypeName());
        }

        /// <summary>
        /// The type of Unit of Measure: Length, Angle and Scale are the only types allowed in the EPSG database.
        /// </summary>
        public string GetTypeName()
        {
            return (string)base["TypeName"];
        }

        public EpsgUnit GetTargetUnit()
        {
            return (EpsgUnit)GetTargetUnitCode();
        }

        /// <summary>
        /// Other UOM of the same type into which the current UOM can be converted using the formula (POSC); 
        /// POSC factors A and D always equal zero for EPSG supplied units of measure.
        /// </summary>
        public int GetTargetUnitCode()
        {
            return (int)base["TargetUnitCode"];
        }

        public bool FactorIsNull()
        {
            return (bool)base["FactorIsNull"];
        }

        /// <summary>
        /// A quantity in the target UOM (y) is obtained from a quantity in the current UOM (x) through the conversion: y =  (B/C).x
        /// </summary>
        public double GetFactorB()
        {
            return (double)base["FactorB"];
        }

        public double GetFactorC()
        {
            return (double)base["FactorC"];
        }

        public override void Read(IDataReader reader)
        {
            bool factorIsNull;

            base["Code"] = reader.Get<int>("UOM_CODE");
            base["UnitName"] = reader.Get<string>("UNIT_OF_MEAS_NAME");
            base["TypeName"] = reader.Get<string>("UNIT_OF_MEAS_TYPE");
            base["TargetUnitCode"] = reader.Get<int>("TARGET_UOM_CODE");
            base["FactorB"] = reader.Get<double>("FACTOR_B", out factorIsNull);
            base["FactorIsNull"] = factorIsNull;

            if (!FactorIsNull())
            {
                base["FactorC"] = reader.Get<double>("FACTOR_C");
            }

            base.Read(reader);
        }

        public override IDictionary<string, TypeCode> GetProperties()
        {
            var properties = base.GetProperties();

            properties.Add("Code", TypeCode.Int32);
            properties.Add("UnitName", TypeCode.String);
            properties.Add("TypeName", TypeCode.String);
            properties.Add("TargetUnitCode", TypeCode.Int32);
            properties.Add("FactorB", TypeCode.Double);
            properties.Add("FactorC", TypeCode.Double);
            properties.Add("FactorIsNull", TypeCode.Boolean);

            return properties;
        }
    }
}
