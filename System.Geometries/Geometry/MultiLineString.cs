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
using System.Xml;

namespace System.Geometries
{
    internal class MultiLineString : GeometryCollection<ILineString>, IMultiLineString
    {
        //public MultiLineString() : base()
        //{
        //}

        public MultiLineString(XmlDocument document) : base(Constants.Xml.MultiLineString, document)
        {
        }

        //public MultiLineString(ILineString[] lines)
        //    : base(lines)
        //{
        //}

        //public MultiLineString(BinaryReader reader)
        //    : base(reader)
        //{
        //}

        public override GeometryType ItemType
        {
            get { return GeometryType.LineString; }
        }

        public override string TypeName
        {
            get { return "MultiLineString"; }
        }

        public void Add(IEnumerable<LineString> items)
        {
            foreach (LineString g in items)
            {
                Add(g);
            }
        }
    }
}
