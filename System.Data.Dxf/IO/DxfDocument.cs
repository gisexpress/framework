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

using System.IO;

namespace System.Data.Dxf
{
    internal class DxfDocument : IDisposable
    {
        DxfDocument()
        {
            Sections = new DxfSectionCollection();
        }

        public DxfTablesSection Tables
        {
            get { return (DxfTablesSection)Sections[DxfSectionName.Tables]; }
        }

        public DxfBlocksSection Blocks
        {
            get { return (DxfBlocksSection)Sections[DxfSectionName.Blocks]; }
        }

        public DxfEntitiesSection Entities
        {
            get { return (DxfEntitiesSection)Sections[DxfSectionName.Entities]; }
        }

        public DxfSectionCollection Sections
        {
            get;
            protected set;
        }

        public static DxfDocument Read(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return Read(File.OpenRead(path));
            }

            return default(DxfDocument);
        }

        public static DxfDocument Read(Stream s)
        {
            var section = default(DxfSection);
            var document = default(DxfDocument);

            using (var reader = new DxfReader(s))
            {
                if (reader.Read())
                {
                    while (DxfSection.Read(reader, out section))
                    {
                        document = document ?? new DxfDocument();
                        section.Document = document;
                        section.Read(reader);
                        document.Sections.Add(section);
                    }
                }
            }

            return document;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
