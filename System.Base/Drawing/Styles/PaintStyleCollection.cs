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

using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace System.Drawing
{
    [DebuggerDisplay("Count : {Count}")]
    public class PaintStyleCollection : XmlElementCollectionBase<PaintStyle>
    {
        public PaintStyleCollection(XmlDocument document) : base(document, Constants.Xml.Style)
        {
        }

        public PaintStyle AddNewStyle(string name, bool color = true)
        {
            PaintStyle item = Find(name);

            if (item == null)
            {
                item = (PaintStyle)OwnerDocument.CreateElement(Constants.Xml.Style);
                item.Id = name;

                if (color)
                {
                    item.LineStyle.Color =
                    item.PolyStyle.Color = ColorTranslator.FromHtml(Constants.PaletteColors.ElementAt(Count % Constants.PaletteColors.Count));
                }

                Add(item);
            }

            return item;
        }

        public PaintStyle this[object name]
        {
            get { return this[name as string, false]; }
        }

        public PaintStyle this[string name, bool appendOnMissing]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return default;
                }

                PaintStyle item = Find(name = name.TrimStart('#'));

                if (appendOnMissing && item == null)
                {
                    item = AddNewStyle(name);
                }

                return item ?? GetItems().FirstOrDefault();
            }
        }
    }
}
