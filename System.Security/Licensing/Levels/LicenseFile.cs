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

using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Web;
using System.Xml;

namespace System.Security.Accounts
{
    partial class AccountManagerInternal
    {
        internal class LicenseFile
        {
            internal LicenseFile()
            {
                string name = DeveloperEnvironment.MSFramework ? "C7A1A909" : "D1DD875B";

                if (Assembly.GetCallingAssembly().IsProductAssembly())
                {
                    Values = HttpUtility.ParseQueryString(string.Empty);
                }

                if (HttpContext.Current.HasValue())
                {
                    Info = new FileInfo(HttpContext.Current.Server.MapPath(string.Concat("~/", Path.ChangeExtension(name, "lic"))));
                }
                else
                {
                    Info = new FileInfo(Path.Combine(ApplicationEnvironment.GetFolderPath(ApplicationFolder.UserProductData).FullName, Path.ChangeExtension(name, "lic")));
                }
            }

            static LicenseFile()
            {
                Address = new UriBuilder("http", "gisexpress.net", 80, "license.ashx").Uri;
                //Address = new UriBuilder("http", "localhost", 14893, "license.ashx").Uri;
            }

            internal DateTime NetworkTime = default(DateTime);
            internal static readonly LicenseFile Current = new LicenseFile();
            internal static readonly DateTime DeadLine = new DateTime(2018, 1, 1);

            protected static readonly Uri Address;

            protected bool IsPrinted;
            protected bool IsInitialized;
            protected NameValueCollection Values;

            public Guid ClientId
            {
                get { return Values["ClientId"].ToGuid(); }
                set { Values["ClientId"] = XmlConvert.ToString(value); }
            }

            public Guid ProductId
            {
                get { return Values["ProductId"].ToGuid(); }
                set { Values["ProductId"] = XmlConvert.ToString(value); }
            }

            public Guid LicenseId
            {
                get { return Values["LicenseId"].ToGuid(); }
                set { Values["LicenseId"] = XmlConvert.ToString(value); }
            }

            public DateTime UniversalTime
            {
                get { return XmlConvert.ToDateTime(Values["UniversalTime"], XmlDateTimeSerializationMode.Local); }
                set { Values["UniversalTime"] = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc); }
            }

            public DateTime SubscriptionStart
            {
                get { return XmlConvert.ToDateTime(Values["SubscriptionStart"], XmlDateTimeSerializationMode.Local); }
                set { Values["SubscriptionStart"] = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc); }
            }

            public DateTime SubscriptionEnd
            {
                get { return XmlConvert.ToDateTime(Values["SubscriptionEnd"], XmlDateTimeSerializationMode.Local); }
                set { Values["SubscriptionEnd"] = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc); }
            }

            public DateTime Expiration
            {
                get { return XmlConvert.ToDateTime(Values["Expiration"], XmlDateTimeSerializationMode.Local); }
                set { Values["Expiration"] = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc); }
            }

            public FileInfo Info
            {
                get;
                protected set;
            }

            public bool IsEmbedded
            {
                get { return !string.IsNullOrEmpty(AppDomain.CurrentDomain.GetData("Subscription") as string); }
            }

            public bool IsOnline
            {
                get
                {
                    Info.Refresh();
                    return Info.Exists;
                }
            }

            public bool IsValid()
            {
                if (IsOnline)
                {
                    if (!IsInitialized)
                    {
                        if (!Load(ProtectionScope.CurrentUser))
                        {
                            return false;
                        }
                    }
                }

                if (IsDeadLine())
                {
                    return false;
                }

                return true;
            }

            public bool IsDeadLine()
            {
                DateTime e = IsOnline ? Expiration : DeadLine;
                DateTime time = IsOnline ? UniversalTime : NetworkTime;

                if (e.Year > 1900 || DeadLine.Year > 1900)
                {
                    var date = time > DateTime.Now ? time : DateTime.Now;
                    var n0 = Convert.ToInt32(Math.Ceiling(e.Subtract(date).TotalDays));
                    var n1 = Convert.ToInt32(Math.Ceiling(DeadLine.Subtract(date).TotalDays));
                    var daysLeft = Math.Min(n0, n1);

                    if (!IsPrinted)
                    {
                        IsPrinted = true;
                        string.Concat(daysLeft, " days left for expiry").Print();
                    }

                    if (daysLeft <= 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void Activate()
            {
            }

            public void Deactivate()
            {
            }

            public bool SendRequest()
            {
                var subscription = HttpUtility.ParseQueryString(AppDomain.CurrentDomain.GetData("Subscription") as string ?? string.Empty);

                if (subscription.Count == 2)
                {
                    return SendRequest(subscription["UserName"], Protection.DecryptString(subscription["Password"], SecureKey));
                }

                return false;
            }

            public bool SendRequest(string userName, string password)
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    var data = default(string);
                    var values = HttpUtility.ParseQueryString(string.Empty);

                    values["user"] = userName;
                    values["pass"] = password;

                    values["data"] = ClientInfo.Current.ToString();
                    values["product"] = ApplicationEnvironment.ProductName;

                    data = Protection.EncryptString(values.ToString(), HttpSecureKey);

                    values.Clear();
                    values["data"] = data;

                    return SendRequest(Address, values);
                }

                return false;
            }

            private bool SendRequest(Uri uri, NameValueCollection values)
            {
                using (HttpWebResponse response = uri.GetResponse(RequestCacheLevel.NoCacheNoStore, values, true))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            if (Load(ProtectionScope.AES, stream.ReadBytes()))
                            {
                                return Save(Info, ProtectionScope.CurrentUser);
                            }
                        }
                    }
                }

                return false;
            }

            public bool Synchronize()
            {
                try
                {
                    if (IsOnline)
                    {
                        var values = HttpUtility.ParseQueryString(string.Empty);

                        values["client"] = XmlConvert.ToString(ClientId);
                        values["client"] = Protection.EncryptString(values.ToString(), HttpSecureKey);

                        using (HttpWebResponse response = Address.GetResponse(RequestCacheLevel.NoCacheNoStore, values, true))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                using (Stream stream = response.GetResponseStream())
                                {
                                    var bytes = stream.ReadBytes();

                                    if (bytes.HasValue() && bytes.Length > 0)
                                    {
                                        values = HttpUtility.ParseQueryString(Protection.DecryptString(Protection.GetString(bytes), HttpSecureKey));

                                        SubscriptionEnd = XmlConvert.ToDateTime(values["End"], XmlDateTimeSerializationMode.Local);
                                        Expiration = XmlConvert.ToDateTime(values["Expiration"], XmlDateTimeSerializationMode.Local);

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.Print();
                }

                return false;
            }

            public bool Load(ProtectionScope protectionScope)
            {
                if (IsOnline)
                {
                    if (Load(protectionScope, Info.ReadAllBytes()))
                    {
                        return true;
                    }

                    Delete();
                }

                return false;
            }

            public bool Load(ProtectionScope protectionScope, byte[] data)
            {
                try
                {
                    if (data.HasValue() && data.Length > 0)
                    {
                        IProtection protection = CryptographyFactory.Create(protectionScope);

                        if (protection.HasValue())
                        {
                            byte[] decryptedBytes = protection.Decrypt(data, SecureKey);

                            if (decryptedBytes.HasValue() && decryptedBytes.Length > 0)
                            {
                                Values = HttpUtility.ParseQueryString(protection.GetString(decryptedBytes));

                                if (!ClientId.IsEmpty() && !ProductId.IsEmpty() && !LicenseId.IsEmpty())
                                {
                                    IsInitialized = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.Print();
                }

                return false;
            }

            public bool Save(ProtectionScope protectionScope)
            {
                return Save(Info, protectionScope);
            }

            public bool Save(FileInfo file, ProtectionScope protectionScope)
            {
                if (file.WriteAllBytes(GetBytes(protectionScope)))
                {
                    return true;
                }

                Delete();
                return false;
            }

            public byte[] GetBytes(ProtectionScope protectionScope)
            {
                try
                {
                    IProtection protection = CryptographyFactory.Create(protectionScope);

                    if (protection.HasValue())
                    {
                        return protection.Encrypt(protection.GetBytes(Values.ToString()), SecureKey);
                    }
                }
                catch (Exception e)
                {
                    e.Print();
                }

                return null;
            }

            public bool Update(DateTime networkTime)
            {
                if (!IsOnline)
                {
                    SendRequest();
                }

                if (networkTime.Subtract(UniversalTime).TotalDays > 1.0)
                {
                    Synchronize();
                }

                UniversalTime = networkTime;

                return Save(ProtectionScope.CurrentUser);
            }

            public void Delete()
            {
                try
                {
                    Info.Delete();
                }
                catch (Exception e)
                {
                    e.Print();
                    SystemLicense.Kill();
                }
                finally
                {
                    IsInitialized = false;
                }
            }
        }
    }
}