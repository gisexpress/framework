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

namespace System.CoordinateSystems
{
    internal static class ProjectionFactory
    {
        public static IProjection Create(string name, Authority authority, IEllipsoid ellipsoid, ILinearUnit unit, ProjectionParameterCollection parameters)
        {
            switch (name.ToLowerInvariant().Replace(' ', '_'))
            {
                case "affine":
                    return new AffineProjection(name, authority, ellipsoid, unit, parameters);
                case "mercator":
                case "mercator_auxiliary_sphere":
                case "popular_visualisation_pseudo_mercator":
                case "mercator_1sp":
                case "mercator_(1sp)":
                case "mercator_variant_a":
                case "mercator_(variant_a)":
                case "mercator_2sp":
                case "mercator_(2sp)":
                case "mercator_variant_b":
                case "mercator_(variant_b)":
                    return new Mercator(name, authority, ellipsoid, unit, parameters);
                case "transverse_mercator":
                    return new TransverseMercator(name, authority, ellipsoid, unit, parameters);
                //case "albers":
                //    return new AlbersProjection(methodName, ellipsoid, unit, parameters, authority, authorityCode);
                //case "albers_equal_area":
                //    return new AlbersEqualAreaProjection(methodName, ellipsoid, unit, parameters, authority, authorityCode);
                //case "albers_conic_equal_area":
                //    return new AlbersConicEqualAreaProjection(methodName, ellipsoid, unit, parameters, authority, authorityCode);
                //case "lambert_conformal_conic":
                //    return new LambertConformalConicProjection(methodName, ellipsoid, unit, parameters, authority, authorityCode);
                //case "lambert_conformal_conic_2sp":
                //case "lambert_conic_conformal_(2sp)":
                //    return new LambertConformalConicProjection2(methodName, ellipsoid, unit, parameters, authority, authorityCode);
            }

            throw new NotSupportedException();
        }
    }
}