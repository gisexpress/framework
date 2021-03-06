//////////////////////////////////////////////////////////////////////////////////////////////////
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

namespace System.CoordinateSystems
{
    internal class AffineProjection : ProjectionCls
    {
        public AffineProjection(string name, Authority authority, IEllipsoid ellipsoid, ILinearUnit unit, ProjectionParameterCollection parameters)
            : base(name, authority, ellipsoid, unit, parameters)
        {
        }

        public override IMathTransform CreateTransform(int sourceSrid, int targetSrid)
        {
            return new MatrixMathTransform(sourceSrid, targetSrid, Parameters[ProjectionParameterKind.Elt_0_1].Value,
                Parameters[ProjectionParameterKind.Elt_0_0].Value,
                Parameters[ProjectionParameterKind.Elt_1_1].Value,
                Parameters[ProjectionParameterKind.Elt_1_0].Value,
                Parameters[ProjectionParameterKind.Elt_0_2].Value,
                Parameters[ProjectionParameterKind.Elt_1_2].Value);
        }

        public override string ToString()
        {
            var s = string.Concat(@"PARAM_MT[""", Name, @"""");

            foreach (ProjectionParameter p in Parameters)
            {
                s = string.Concat(s, ",{0}", p);
            }

            return string.Concat(s, "]");
        }
    }
}
