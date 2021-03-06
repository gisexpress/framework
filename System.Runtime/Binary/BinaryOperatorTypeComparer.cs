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

namespace System.Runtime
{
    internal class BinaryOperatorTypeComparer : IComparer<BinaryOperatorType>
    {
        int IComparer<BinaryOperatorType>.Compare(BinaryOperatorType x, BinaryOperatorType y)
        {
            return Compare(x, y);
        }

        public static int Compare(BinaryOperatorType x, BinaryOperatorType y)
        {
            return GetPriority(x).CompareTo(GetPriority(y));
        }

        static int GetPriority(BinaryOperatorType x)
        {
            switch (x)
            {
                case BinaryOperatorType.And:
                case BinaryOperatorType.Or:
                    return -2;
                case BinaryOperatorType.Equals:
                case BinaryOperatorType.NotEquals:
                case BinaryOperatorType.Less:
                case BinaryOperatorType.Greater:
                case BinaryOperatorType.LessOrEqual:
                case BinaryOperatorType.GreaterOrEqual:
                    return -1;
                case BinaryOperatorType.Divide:
                case BinaryOperatorType.Multiply:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
