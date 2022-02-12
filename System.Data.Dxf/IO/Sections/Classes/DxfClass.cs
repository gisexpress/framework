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

namespace System.Data.Dxf
{
    internal class DxfClass
    {
        /// <summary>
        /// Identifies beginning of a CLASS record.
        /// </summary>
        public string RecordType
        {
            get;
            set;
        }

        /// <summary>
        /// Class DXF record name; always unique.
        /// </summary>
        public string RecordName
        {
            get;
            set;
        }

        /// <summary>
        /// C++ class name. 
        /// Used to bind with software that defines object class behavior; always unique.
        /// </summary>
        public string ClassName
        {
            get;
            set;
        }

        /// <summary>
        /// Posted in Alert box when a class definition listed in this section is not currently loaded.
        /// </summary>
        public string ApplicationName
        {
            get;
            set;
        }

        /// <summary>
        /// Bit-coded value that indicates the capabilities of this object as a proxy:
        /// 0 = No operations allowed (0)
        /// 1 = Erase allowed (0x1)
        /// 2 = Transform allowed (0x2)
        /// 4 = Color change allowed (0x4)
        /// 8 = Layer change allowed (0x8)
        /// 16 = Linetype change allowed (0x10)
        /// 32 = Linetype scale change allowed (0x20)
        /// 64 = Visibility change allowed (0x40)
        /// 128 = Cloning allowed (0x80)
        /// 256 = Lineweight change allowed (0x100)
        /// 512 = Plot Style Name change allowed (0x200)
        /// 895 = All operations except cloning allowed (0x37F)
        /// 1023 = All operations allowed (0x3FF)
        /// 1024 = Disables proxy warning dialog (0x400)
        /// 32768 = R13 format proxy (0x8000)
        /// </summary>
        public int ProxyCapabilities
        {
            get;
            set;
        }

        /// <summary>
        /// Instance count for a custom class.
        /// </summary>
        public int InstanceCount
        {
            get;
            set;
        }

        /// <summary>
        /// Set to 1 if class was not loaded when this DXF file was created, and 0 otherwise.
        /// </summary>
        public byte ProxyFlag
        {
            get;
            set;
        }

        /// <summary>
        ///  Set to 1 if class was derived from the AcDbEntity class and can reside in the BLOCKS or ENTITIES section.
        ///  If 0, instances may appear only in the OBJECTS section.
        /// </summary>
        public byte EntityFlag
        {
            get;
            set;
        }

        public bool Read(DxfReader reader)
        {
            if (reader.GroupCode == 0 && reader.Current.Equals("CLASS"))
            {
                RecordType = reader.Current;

                while (reader.Read())
                {
                    switch (reader.GroupCode)
                    {
                        case 0:
                            RecordType = reader.Current;
                            break;
                        case 1:
                            RecordName = reader.Current;
                            break;
                        case 2:
                            ClassName = reader.Current;
                            break;
                        case 3:
                            ApplicationName = reader.Current;
                            break;
                        case 90:
                            ProxyCapabilities = reader.GetInt32();
                            break;
                        case 91:
                            InstanceCount = reader.GetInt32();
                            break;
                        case 280:
                            ProxyFlag = reader.GetByte();
                            break;
                        case 281:
                            EntityFlag = reader.GetByte();
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
