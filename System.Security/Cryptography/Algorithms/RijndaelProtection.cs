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

using System.IO;
using System.Security.Cryptography;

namespace System.Security
{
    internal sealed class RijndaelProtection : ProtectionBase
    {
        protected override byte[] OnTransform(byte[] value, byte[] password, bool inverse)
        {
            if (value.HasValue() && value.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    using (var aes = new RijndaelManaged())
                    {
                        aes.KeySize = 0x100;
                        aes.BlockSize = 0x80;

                        using (SHA256 algorithm = SHA256.Create())
                        {
                            using (var key = new Rfc2898DeriveBytes(algorithm.ComputeHash(password ?? Key), SaltBytes, 1000))
                            {
                                aes.Key = key.GetBytes(aes.KeySize / 8);
                                aes.IV = key.GetBytes(aes.BlockSize / 8);

                                aes.Mode = CipherMode.CBC;

                                using (ICryptoTransform transform = inverse ? aes.CreateDecryptor() : aes.CreateEncryptor())
                                {
                                    using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                                    {
                                        cs.Write(value, 0, value.Length);
                                    }
                                }
                            }
                        }

                        return stream.ToArray();
                    }
                }
            }

            return null;
        }
    }
}