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
using System.IO.Compression;

namespace System.Diagnostics
{
    internal class IOTest : IUnitTest
    {
        public int Priority
        {
            get { return 100; }
        }

        public void Run()
        {
            Binary();
            Compression();
        }

        public void Binary()
        {
            string fileName = Path.GetTempFileName();

            BinaryInternal(default(string));
            BinaryInternal(fileName);

            File.Delete(fileName);
        }

        void BinaryInternal(string fileName)
        {
            using (var file = new BinaryFile(fileName))
            {
                var prefix = "Değer ";
                var s0 = "Türkçe Karakter Test ş ü ö İ ğ ı ç";

                using (BinaryFileConnection connection = file.CreateConnection())
                {
                    using (var output = new BinaryFilePageStream(connection, 1))
                    {
                        output.Write(s0);

                        foreach (SampleData.NorthwindDatabase.Customer item in SampleData.Northwind.Customers)
                        {
                            output.Write(item.ContactName);
                        }
                    }
                }

                using (BinaryFileConnection connection = file.CreateConnection())
                {
                    using (var output = new BinaryFilePageStream(connection, 2))
                    {
                        for (int n = 1; n < 250; n++)
                        {
                            output.Write(string.Concat(prefix, Convert.ToString(n).PadLeft(6, '3')));
                        }

                        for (int n = 250; n < 500; n++)
                        {
                            output.Write(string.Concat(prefix, Convert.ToString(n).PadLeft(6, '3')));
                        }
                    }
                }

                using (BinaryFileConnection connection = file.CreateConnection())
                {
                    using (var input = new BinaryFilePageStream(connection, 1))
                    {
                        string s;

                        Debug.Assert(s0.Equals(s = input.ReadString()));

                        foreach (SampleData.NorthwindDatabase.Customer item in SampleData.Northwind.Customers)
                        {
                            Debug.Assert(Equals(item.ContactName, s = input.ReadString()));
                        }
                    }
                }

                using (BinaryFileConnection connection = file.CreateConnection())
                {
                    using (var input = new BinaryFilePageStream(connection, 2))
                    {
                        int n = 1;

                        while (input.CanRead())
                        {
                            Debug.Assert(Equals(string.Concat(prefix, Convert.ToString(n++).PadLeft(6, '3')), input.ReadString()));
                        }
                    }
                }
            }
        }

        void Compression()
        {
            string hexString = "504B0304140000000800A8786849566D2587CE010000300300000E0000004261A76C616E748D20322E74787465523D6FDB3010DD0DF83FDC661BA0856C45A7C26D1AC028E0295DBA51E23921445101491595FFB0574F997C461F252B31126811EE78EFDEC73D9E437D64DA38ED6963643E7B1C0B3A17B4115A5E0BB50EBA4E1C9C25D336DA0EDD157936367C9BCFBE5B67BDB1274BE7030756643D1E7B4E09A39D039A64C498B4373AA409DFE920746C6BAA254A7242A50D6FABA83E371CADB7D40EAFD707459BF51F4577EBAFF49769B15E90618F6E7769300142A1A07B8E896BC71E9D8F488629CBC1F6BD2EAF851EED81CDAB3AAA8B127552677AD661EF3840AB7EB60E1B1A809F04DC3DE43BDD8BA2C4B1420F066019D9681B4C00709AF5E45B908A7240C90D68BDCE625454957ACA0ABA376FA46F83E98AF96CFB63B3DB2D0456B558F5EAB9C91CF54BC27A3230D73B6E1868797E7BBF5B8CAB96BF5D17E126BE08A72F4982B8462E43B28268B3D35EFC8A1E6CE07DFB4FD1F6E797E28E9E6C095E0C5E1E86384A40E8A5024618FEF757D193ABD9C7DE710ECA59784B9FEFE3365B642DF5FB0594D6358C67389A827E49D4959E00A613CC61038AEC5E43F9E82ECEE908996360E9C3C251FF4D7A2B2A3BD779D39DBA5B2EB0613919B6BA6ECDA98D391655DB80AD867238DD3BFD9E2F83B50DF9100FC57F504B0304140000000000837868490D14754C22000000220000000E0000004261A76C616E748D20312E74787454FC726BE765204B6172616B746572205465737420FE20FC20F620DD20F020FD20E7504B01021400140000000800A8786849566D2587CE010000300300000E00000000000000010020000000000000004261A76C616E748D20322E747874504B01021400140000000000837868490D14754C22000000220000000E00000000000000000020000000FA0100004261A76C616E748D20312E747874504B0506000000000200020078000000480200000000";

            using (var s = hexString.HexStringToByteArray().ToStream())
            {
                Zip value = Zip.Open(s);

                Debug.Assert(value.Entries["Bağlantı 1.txt"].Text.Equals("Türkçe Karakter Test ş ü ö İ ğ ı ç"));
                Debug.Assert(value.Entries["Bağlantı 2.txt"].Text.ContainsIgnoreCase("(ö,ç,ş,ı,ğ,ü)"));

                Debug.Assert(Equals(value.ToArray().ToHexString(), hexString));
            }
        }
    }
}