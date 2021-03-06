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

using System.Geometries;
using System.Transformations;

namespace System.CoordinateSystems
{
    internal class MatrixMathTransform : MathTransform
    {
        public MatrixMathTransform(int sourceSrid, int targetSrid)
            : this(sourceSrid, targetSrid, 1.0, 0.0, 0.0, 1.0, 0, 0)
        {
        }

        public MatrixMathTransform(int sourceSrid, int targetSrid, double[] elements)
            : this(sourceSrid, targetSrid, elements[0], elements[1], elements[2], elements[3], elements[4], elements[5])
        {
        }

        public MatrixMathTransform(int sourceSrid, int targetSrid, double m11, double m12, double m21, double m22, double dx, double dy)
            : base(sourceSrid, targetSrid)
        {
            Matrix = new MatrixTransform(m11, m12, m21, m22, dx, dy);
            InverseMatrix = Matrix.Clone();
            InverseMatrix.Invert();
        }

        protected readonly MatrixTransform Matrix;
        protected readonly MatrixTransform InverseMatrix;

        protected override bool OnTransform(ICoordinate c)
        {
            if (IsInverse)
            {
                return InverseMatrix.Transform(c);
            }

            return Matrix.Transform(c);
        }
    }
}
