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

using System.Collections.Generic;

namespace System.Data.Dxf
{
    internal class DxfBlocksSection : DxfSection
    {
        public const string Keyword = "BLOCKS";

        public DxfBlocksSection()
        {
            Items = new Dictionary<string, DxfBlock>();
        }

        protected Dictionary<string, DxfBlock> Items;

        public override string Name
        {
            get { return Keyword; }
        }

        public bool TryGetValue(string name, out DxfBlock value)
        {
            return Items.TryGetValue(name, out value);
        }

        public override bool Read(DxfReader reader)
        {
            while (ReadNext(reader))
            {
                if (reader.GroupCode == 0 && reader.Current.Equals("BLOCK"))
                {
                    var item = new DxfBlock();

                    item.Document = Document;
                    item.Read(reader);

                    if (!string.IsNullOrEmpty(item.Name) && !Items.ContainsKey(item.Name))
                    {
                        Items.Add(item.Name, item);
                    }
                }
            }

            return true;
        }
    }
}
