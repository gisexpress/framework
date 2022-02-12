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

using System.Reflection;

namespace System.Security
{
    internal sealed class ScopeProtection : ProtectionBase
    {
        static ScopeProtection()
        {
            if (DeveloperEnvironment.MSFramework)
            {
                Provider = Type.GetType("System.Security.Cryptography.ProtectedData, System.Security, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                Protect = Provider.GetMethod("Protect");
                Unprotect = Provider.GetMethod("Unprotect");
            }
            else
            {
                MonoProtect = CryptographyFactory.Create(ProtectionScope.AES);
            }
        }

        public ScopeProtection(int protectionScope)
        {
            Scope = protectionScope;
        }

        private readonly int Scope;
        private readonly static Type Provider;
        private readonly static MethodInfo Protect;
        private readonly static MethodInfo Unprotect;
        private readonly static IProtection MonoProtect;

        protected override byte[] OnTransform(byte[] value, byte[] password, bool inverse)
        {
            if (DeveloperEnvironment.MSFramework)
            {
                if (inverse)
                {
                    return Invoke<byte[]>(Unprotect, value, password, Scope);
                }
                else
                {
                    return Invoke<byte[]>(Protect, value, password, Scope);
                }
            }
            else
            {
                if (inverse)
                {
                    return MonoProtect.Decrypt(value, password);
                }
                else
                {
                    return MonoProtect.Encrypt(value, password);
                }
            }
        }

        T Invoke<T>(MethodInfo method, params object[] parameters)
        {
            return (T)method.Invoke(default(object), parameters);
        }
    }
}
