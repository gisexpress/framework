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

using System.CoordinateSystems;

namespace System.Geometries
{
    public interface ILineSegment : IComparable
    {
        ICoordinate P0
        {
            get;
            set;
        }

        ICoordinate P1
        {
            get;
            set;
        }

        ICoordinate GetCoordinate(int i);
        void SetCoordinates(ICoordinate p0, ICoordinate p1);

        double GetAngle();
        double GetLength();
        double Distance(ICoordinate c);
        double Distance(double x, double y);
        double Distance(ILineSegment other);
        double EdgeDistance(ICoordinate c);

        bool IsIntersects(ILineSegment other);

        ICoordinate GetMidPoint();
        ICoordinate Intersection(ILineSegment other);

        int OrientationIndex(ICoordinate c);
        int ReverseOrientationIndex(ICoordinate c);
        int CollinearIntersection(ILineSegment other, ref ICoordinate[] inputPoints);

        void Reverse();
        void Rotate(double angle);
        void Extend(double distance);
        void Extend(double start, double end);
        bool Transform(IMathTransform transform);

        bool Project(ICoordinate c);

        //ILineString ToLineString();
        ILineSegment Subtract(ICoordinate c);
        ILineSegment Clone();

        int GetHashCode(bool reverse);

        bool IsEquivalent(ILineSegment other);
    }
}