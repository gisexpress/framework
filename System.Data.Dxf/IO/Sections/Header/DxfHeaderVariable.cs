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

namespace System.Data.Dxf
{
    internal class DxfHeaderVariable : DxfObject
    {
        public string Name
        {
            get;
            protected set;
        }

        public int GroupCode
        {
            get;
            protected set;
        }

        public object Value
        {
            get;
            protected set;
        }

        public IList<string> Values
        {
            get;
            protected set;
        }

        public override bool Read(DxfReader reader)
        {
            if (reader.GroupCode == 9)
            {
                Name = reader.GetString();

                if (reader.Read())
                {
                    GroupCode = reader.GroupCode;

                    switch (Name)
                    {
                        case DxfHeaderVariables.MaintenanceVersionNumber:
                            if (reader.GroupCode == 70) Value = reader.GetInt32();
                            break;
                        case DxfHeaderVariables.DrawingDatabaseVersion:
                            if (reader.GroupCode == 1) Value = reader.GetString();
                            break;
                        case DxfHeaderVariables.AngleDirection:
                            if (reader.GroupCode == 50) Value = reader.GetDouble();
                            break;
                        case DxfHeaderVariables.DrawingCodePage:
                            if (reader.GroupCode == 3) Value = reader.GetString();
                            break;
                        default:
                            ReadUnhandledLines(reader);
                            break;
                    }

                    return true;
                }
            }

            return false;
        }

        protected void ReadUnhandledLines(DxfReader reader)
        {
            Values = new List<string>();

            while (true)
            {
                Values.Add(reader.GetString());

                if (reader.Read())
                {
                    if (reader.GroupCode == 9)
                    {
                        break;
                    }

                    if (DxfSection.IsEndSection(reader))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public override string ToString()
        {
            return string.Concat(Name, ':', Value ?? Values.Join(", ").TrimEnd());
        }
    }
}
