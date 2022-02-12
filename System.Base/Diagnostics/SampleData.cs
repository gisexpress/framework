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

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace System.Data.Sqn.Diagnostics
{
    public static class SampleData
    {
        static SampleData()
        {
            Northwind = new NorthwindDatabase();
        }

        public static readonly NorthwindDatabase Northwind;

        public class NorthwindDatabase : Database
        {
            public NorthwindDatabase()
            {
                Shippers = GetRows<Shipper>(Files.Shippers).ToList();
                Products = GetRows<Product>(Files.Products).ToList();
                Employees = GetRows<Employee>(Files.Employees).ToList();
                Customers = GetRows<Customer>(Files.Customers).ToList();
                Orders = GetRows<Order>(Files.Orders).ToList();
                OrderDetails = GetRows<OrderDetail>(Files.OrderDetails).ToList();

                Tables.Add(new DataSource(this, "Shippers"));
                Tables.Add(new DataSource(this, "Products"));
                Tables.Add(new DataSource(this, "Employees"));
                Tables.Add(new DataSource(this, "Customers"));
                Tables.Add(new DataSource(this, "Orders"));
                Tables.Add(new DataSource(this, "Order Details"));
            }

            public List<Shipper> Shippers
            {
                get;
                protected set;
            }

            public List<Product> Products
            {
                get;
                protected set;
            }

            public List<Employee> Employees
            {
                get;
                protected set;
            }

            public List<Customer> Customers
            {
                get;
                protected set;
            }

            public List<Order> Orders
            {
                get;
                protected set;
            }

            public List<OrderDetail> OrderDetails
            {
                get;
                protected set;
            }

            public override IEnumerable<IDataRecord> GetRows(string name)
            {
                switch (name)
                {
                    case "Shippers":
                        return Shippers.Cast<IDataRecord>();
                    case "Products":
                        return Products.Cast<IDataRecord>();
                    case "Employees":
                        return Employees.Cast<IDataRecord>();
                    case "Customers":
                        return Customers.Cast<IDataRecord>();
                    case "Orders":
                        return Orders.Cast<IDataRecord>();
                    case "Order Details":
                        return OrderDetails.Cast<IDataRecord>();
                }

                return base.GetRows(name);
            }

            protected IEnumerable<T> GetRows<T>(string data)
            {
                ConstructorInfo objNew = typeof(T).GetConstructor(new Type[] { typeof(IEnumerator<string>) });

                foreach (string line in data.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return (T)objNew.Invoke(new object[] { line.Split(';').Cast<string>().GetEnumerator() });
                }
            }

            public class Shipper : DataRecord<Shipper>
            {
                public Shipper()
                {
                }

                public Shipper(IEnumerator<string> e)
                {
                    if (e.MoveNext()) ShipperID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) CompanyName = e.Current;
                    if (e.MoveNext()) Phone = e.Current;
                }

                public int ShipperID
                {
                    get;
                    set;
                }

                public string CompanyName
                {
                    get;
                    set;
                }

                public string Phone
                {
                    get;
                    set;
                }
            }

            public class Product : DataRecord<Product>
            {
                public Product()
                {
                }

                public Product(IEnumerator<string> e)
                {
                    if (e.MoveNext()) ProductID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) ProductName = e.Current;
                    if (e.MoveNext()) SupplierID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) CategoryID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) QuantityPerUnit = e.Current;
                    if (e.MoveNext()) UnitPrice = decimal.Parse(e.Current, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) UnitsInStock = short.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) UnitsOnOrder = short.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) ReorderLevel = short.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) Discontinued = bool.Parse(e.Current);
                }

                public int ProductID
                {
                    get;
                    set;
                }

                public string ProductName
                {
                    get;
                    set;
                }

                public int SupplierID
                {
                    get;
                    set;
                }

                public int CategoryID
                {
                    get;
                    set;
                }

                public string QuantityPerUnit
                {
                    get;
                    set;
                }

                public decimal UnitPrice
                {
                    get;
                    set;
                }

                public short UnitsInStock
                {
                    get;
                    set;
                }

                public short UnitsOnOrder
                {
                    get;
                    set;
                }

                public short ReorderLevel
                {
                    get;
                    set;
                }

                public bool Discontinued
                {
                    get;
                    set;
                }

                public void Read(BinaryFilePageStream input)
                {
                    ProductName = input.ReadString();
                    SupplierID = input.ReadInt32();
                    CategoryID = input.ReadInt32();
                    QuantityPerUnit = input.ReadString();
                    UnitPrice = input.ReadDecimal();
                    UnitsInStock = input.ReadInt16();
                    UnitsOnOrder = input.ReadInt16();
                    ReorderLevel = input.ReadInt16();
                    Discontinued = input.ReadBoolean();
                }

                public void Write(BinaryFilePageStream output)
                {
                    output.Write(ProductName);
                    output.Write(SupplierID);
                    output.Write(CategoryID);
                    output.Write(QuantityPerUnit);
                    output.Write(UnitPrice);
                    output.Write(UnitsInStock);
                    output.Write(UnitsOnOrder);
                    output.Write(ReorderLevel);
                    output.Write(Discontinued);
                }

                public object[] ToArray()
                {
                    return new object[] { ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued };
                }

                public override int GetHashCode()
                {
                    return Hash.Get(ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued);
                }

                public override bool Equals(object obj)
                {
                    return Hash.Equals(this, obj as Product);
                }
            }

            public class Customer : DataRecord<Customer>
            {
                public Customer()
                {
                }

                public Customer(IEnumerator<string> e)
                {
                    if (e.MoveNext()) CustomerID = e.Current;
                    if (e.MoveNext()) CompanyName = e.Current;
                    if (e.MoveNext()) ContactName = e.Current;
                    if (e.MoveNext()) ContactTitle = e.Current;
                    if (e.MoveNext()) Address = e.Current;
                    if (e.MoveNext()) City = e.Current;
                    if (e.MoveNext()) Region = e.Current;
                    if (e.MoveNext()) PostalCode = e.Current;
                    if (e.MoveNext()) Country = e.Current;
                    if (e.MoveNext()) Phone = e.Current;
                    if (e.MoveNext()) Fax = e.Current;
                }

                public string CustomerID
                {
                    get;
                    set;
                }

                public string CompanyName
                {
                    get;
                    set;
                }

                public string ContactName
                {
                    get;
                    set;
                }

                public string ContactTitle
                {
                    get;
                    set;
                }

                public string Address
                {
                    get;
                    set;
                }

                public string City
                {
                    get;
                    set;
                }

                public string Region
                {
                    get;
                    set;
                }

                public string PostalCode
                {
                    get;
                    set;
                }

                public string Country
                {
                    get;
                    set;
                }

                public string Phone
                {
                    get;
                    set;
                }

                public string Fax
                {
                    get;
                    set;
                }
            }

            public class Employee : DataRecord<Employee>
            {
                public Employee()
                {
                }

                public Employee(IEnumerator<string> e)
                {
                    if (e.MoveNext()) EmployeeID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) LastName = e.Current;
                    if (e.MoveNext()) FirstName = e.Current;
                    if (e.MoveNext()) Title = e.Current;
                    if (e.MoveNext()) TitleOfCourtesy = e.Current;
                    if (e.MoveNext()) BirthDate = DateTime.Parse(e.Current);
                    if (e.MoveNext()) HireDate = DateTime.Parse(e.Current);
                    if (e.MoveNext()) Address = e.Current;
                    if (e.MoveNext()) City = e.Current;
                    if (e.MoveNext()) Region = e.Current;
                    if (e.MoveNext()) PostalCode = e.Current;
                    if (e.MoveNext()) Country = e.Current;
                    if (e.MoveNext()) HomePhone = e.Current;
                    if (e.MoveNext()) Extension = e.Current;
                    if (e.MoveNext()) Photo = Encoding.UTF8.GetBytes(e.Current);
                    if (e.MoveNext()) Notes = e.Current;
                    if (e.MoveNext() && !e.Current.Equals("NULL")) ReportsTo = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) PhotoPath = e.Current;
                }

                public int EmployeeID
                {
                    get;
                    set;
                }

                public string LastName
                {
                    get;
                    set;
                }

                public string FirstName
                {
                    get;
                    set;
                }

                public string Title
                {
                    get;
                    set;
                }

                public string TitleOfCourtesy
                {
                    get;
                    set;
                }

                public DateTime BirthDate
                {
                    get;
                    set;
                }

                public DateTime HireDate
                {
                    get;
                    set;
                }

                public string Address
                {
                    get;
                    set;
                }

                public string City
                {
                    get;
                    set;
                }

                public string Region
                {
                    get;
                    set;
                }

                public string PostalCode
                {
                    get;
                    set;
                }

                public string Country
                {
                    get;
                    set;
                }

                public string HomePhone
                {
                    get;
                    set;
                }

                public string Extension
                {
                    get;
                    set;
                }

                public byte[] Photo
                {
                    get;
                    set;
                }

                public string Notes
                {
                    get;
                    set;
                }

                public int ReportsTo
                {
                    get;
                    set;
                }

                public string PhotoPath
                {
                    get;
                    set;
                }
            }

            public class Order : DataRecord<Order>
            {
                public Order()
                {
                }

                public Order(IEnumerator<string> e)
                {
                    if (e.MoveNext()) OrderID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) CustomerID = e.Current;
                    if (e.MoveNext()) EmployeeID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) OrderDate = ParseDateTime(e.Current);
                    if (e.MoveNext()) RequiredDate = ParseDateTime(e.Current);
                    if (e.MoveNext()) ShippedDate = ParseDateTime(e.Current);
                    if (e.MoveNext()) ShipVia = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) Freight = decimal.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) ShipName = e.Current;
                    if (e.MoveNext()) ShipAddress = e.Current;
                    if (e.MoveNext()) ShipCity = e.Current;
                    if (e.MoveNext()) ShipRegion = e.Current;
                    if (e.MoveNext()) ShipPostalCode = e.Current;
                    if (e.MoveNext()) ShipCountry = e.Current;
                }

                DateTime ParseDateTime(string value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return default(DateTime);
                    }

                    return DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                }

                public int OrderID
                {
                    get;
                    set;
                }

                public string CustomerID
                {
                    get;
                    set;
                }

                public int EmployeeID
                {
                    get;
                    set;
                }

                public DateTime OrderDate
                {
                    get;
                    set;
                }

                public DateTime RequiredDate
                {
                    get;
                    set;
                }

                public DateTime ShippedDate
                {
                    get;
                    set;
                }

                public int ShipVia
                {
                    get;
                    set;
                }

                public decimal Freight
                {
                    get;
                    set;
                }

                public string ShipName
                {
                    get;
                    set;
                }

                public string ShipAddress
                {
                    get;
                    set;
                }

                public string ShipCity
                {
                    get;
                    set;
                }

                public string ShipRegion
                {
                    get;
                    set;
                }

                public string ShipPostalCode
                {
                    get;
                    set;
                }

                public string ShipCountry
                {
                    get;
                    set;
                }
            }

            public class OrderDetail : DataRecord<OrderDetail>
            {
                public OrderDetail()
                {
                }

                public OrderDetail(IEnumerator<string> e)
                {
                    if (e.MoveNext()) OrderID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) ProductID = int.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) UnitPrice = decimal.Parse(e.Current);
                    if (e.MoveNext()) Quantity = short.Parse(e.Current, NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (e.MoveNext()) Discount = float.Parse(e.Current);
                }

                public int OrderID
                {
                    get;
                    set;
                }

                public int ProductID
                {
                    get;
                    set;
                }

                public decimal UnitPrice
                {
                    get;
                    set;
                }

                public short Quantity
                {
                    get;
                    set;
                }

                public float Discount
                {
                    get;
                    set;
                }
            }
        }
    }
}