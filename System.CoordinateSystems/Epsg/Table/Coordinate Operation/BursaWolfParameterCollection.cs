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
using System.IO;

namespace System.CoordinateSystems.Epsg
{
    internal class BursaWolfParameterCollection
    {
        public BursaWolfParameterCollection()
        {
            Items = new Dictionary<int, SortedList<int, BursaWolfParameter>>();
        }

        protected readonly Dictionary<int, SortedList<int, BursaWolfParameter>> Items;

        public IList<BursaWolfParameter> this[int datumCode]
        {
            get
            {
                if (Items.ContainsKey(datumCode))
                {
                    return Items[datumCode].Values;
                }

                return new BursaWolfParameter[] { };
            }
        }

        public BursaWolfParameter this[int datumCode, int variant]
        {
            get
            {
                if (Items.ContainsKey(datumCode) && Items[datumCode].ContainsKey(variant))
                {
                    return Items[datumCode][variant];
                }

                return null;
            }
        }

        internal void Load()
        {
            foreach (EpsgDatumRecord r in EpsgDatumTable.Current.Rows)
            {
                if (r.IsValid())
                {
                    SortedList<int, BursaWolfParameter> parameters = GetParameters(r.GetCode());

                    if (parameters.Count > 0)
                    {
                        Items.Add(r.GetCode(), parameters);
                    }
                }
            }
        }

        internal void Read(BinaryReader reader)
        {
            int numItems = reader.ReadInt32();

            for (int n = 0; n < numItems; n++)
            {
                var variants = new SortedList<int, BursaWolfParameter>();

                int datumCode = reader.ReadInt32();
                int numVariants = reader.ReadByte();

                for (int n2 = 0; n2 < numVariants; n2++)
                {
                    var parameter = new BursaWolfParameter
                    {
                        Variant = reader.ReadByte(),
                        AreaCode = reader.ReadInt32(),
                        NumParams = reader.ReadByte()
                    };

                    if (parameter.NumParams >= 3)
                    {
                        parameter.Dx = reader.ReadDouble();
                        parameter.Dy = reader.ReadDouble();
                        parameter.Dz = reader.ReadDouble();
                    }

                    if (parameter.NumParams == 7)
                    {
                        parameter.Ex = reader.ReadDouble();
                        parameter.Ey = reader.ReadDouble();
                        parameter.Ez = reader.ReadDouble();
                        parameter.Ppm = reader.ReadDouble();
                    }

                    variants.Add(parameter.Variant, parameter);
                }

                Items.Add(datumCode, variants);
            }
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(Items.Count);

            foreach (int datumCode in Items.Keys)
            {
                SortedList<int, BursaWolfParameter> variants = Items[datumCode];

                writer.Write(datumCode);
                writer.Write((byte)variants.Count);

                foreach (int variant in variants.Keys)
                {
                    BursaWolfParameter parameter = variants[variant];

                    writer.Write((byte)parameter.Variant);
                    writer.Write(parameter.AreaCode);
                    writer.Write((byte)parameter.NumParams);

                    if (parameter.NumParams >= 3)
                    {
                        writer.Write(parameter.Dx);
                        writer.Write(parameter.Dy);
                        writer.Write(parameter.Dz);
                    }

                    if (parameter.NumParams == 7)
                    {
                        writer.Write(parameter.Ex);
                        writer.Write(parameter.Ey);
                        writer.Write(parameter.Ez);
                        writer.Write(parameter.Ppm);
                    }
                }
            }
        }

        SortedList<int, BursaWolfParameter> GetParameters(int datumCode)
        {
            var conversions = new SortedList<int, BursaWolfParameter>();

            foreach (EpsgCoordinateReferenceSystemRecord crs in EpsgCoordinateReferenceSystemTable.Current.FindByDatum(datumCode))
            {
                foreach (EpsgCoordinateOperationRecord coordOp in EpsgCoordinateOperationTable.Current.Find(crs.GetCode(), 4326))
                {
                    if (coordOp.GetType().Equals("transformation", StringComparison.OrdinalIgnoreCase) && !conversions.ContainsKey(coordOp.GetVariant()))
                    {
                        IList<EpsgOperationParameter> parameters = EpsgCoordinateOperationTable.ParameterTable.FindParameters(coordOp.GetCode());

                        if (parameters.Count == 3 || parameters.Count == 7)
                        {
                            var info = new BursaWolfParameter
                            {
                                Dx = parameters[0].GetValue(EpsgUnit.Meter),
                                Dy = parameters[1].GetValue(EpsgUnit.Meter),
                                Dz = parameters[2].GetValue(EpsgUnit.Meter)
                            };

                            if (parameters.Count == 7)
                            {
                                info.Ex = parameters[3].GetValue(EpsgUnit.ArcSeconds);
                                info.Ey = parameters[4].GetValue(EpsgUnit.ArcSeconds);
                                info.Ez = parameters[5].GetValue(EpsgUnit.ArcSeconds);
                                info.Ppm = parameters[6].GetValue(EpsgUnit.PartsPerMillion);
                            }

                            info.NumParams = parameters.Count;
                            info.Variant = coordOp.GetVariant();
                            info.AreaCode = coordOp.GetAreaCode();
                            conversions.Add(coordOp.GetVariant(), info);
                        }
                    }
                }
            }

            return conversions;
        }
    }

    internal class BursaWolfParameter
    {
        public bool IsEmpty
        {
            get { return AreaCode == 0; }
        }

        public int Variant;
        public int AreaCode;

        public int NumParams;

        public double Dx;
        public double Dy;
        public double Dz;

        public double Ex;
        public double Ey;
        public double Ez;

        public double Ppm;

        public override string ToString()
        {
            var record = EpsgAreaTable.Current.Find(AreaCode);

            if (!IsEmpty && record.HasValue())
            {
                return string.Concat(record.GetName(), " (v", Variant, ")");
            }

            return "User-Defined";
        }
    }
}
