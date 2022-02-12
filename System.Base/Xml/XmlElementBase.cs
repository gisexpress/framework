using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace System.Xml
{
    public class XmlElementBase : XmlElement
    {
        protected internal XmlElementBase(string prefix, string localName, string namespaceURI, XmlDocument document) : base(prefix, localName, namespaceURI, document)
        {
        }

        public new XmlDocumentBase OwnerDocument
        {
            get { return (XmlDocumentBase)base.OwnerDocument; }
        }

        public XmlElementBase GetChild(string name)
        {
            return (XmlElementBase)base[name];
        }

        public XmlElementBase GetOrCreate(string name)
        {
            var e = (XmlElementBase)base[name];

            if (e == null)
            {
                AppendChild(e = (XmlElementBase)OwnerDocument.CreateElement(Prefix, name, NamespaceURI));
            }

            return e;
        }

        public IEnumerable<XmlElementBase> GetChilds()
        {
            return ChildNodes.OfType<XmlElementBase>();
        }

        public string Get(string name)
        {
            return Get(default(string), name);
        }

        public T Get<T>(string name, T valueDefault)
        {
            return Get(name, () => valueDefault);
        }

        public T Get<T>(string name, Func<T> valueDefault)
        {
            XmlNode e = base[name] ?? (XmlNode)Attributes[name];

            if (e == null)
            {
                return valueDefault();
            }

            string value = e is XmlAttribute ? e.Value : e.InnerText;

            if (string.IsNullOrEmpty(value))
            {
                return valueDefault();
            }

            return Parse<T>(value);
        }

        public XmlAttribute SetAttribute<T>(string name, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            XmlAttribute attribute = Attributes[name];

            if (attribute == null)
            {
                Attributes.Append(attribute = OwnerDocument.CreateAttribute(name));
            }

            if (value.HasValue())
            {
                attribute.Value = ToString(value);
            }

            return attribute;
        }

        public XmlElementBase Set(string name)
        {
            return Set(name, default(string));
        }

        public XmlElementBase Set<T>(string name, T value)
        {
            XmlElementBase e = GetChild(name);

            if (e == null)
            {
                e = (XmlElementBase)OwnerDocument.CreateElement(name);

                if (value.HasValue())
                {
                    e.InnerText = ToString(value);
                }

                AppendChild(e);
            }
            else
            {
                e.InnerText = ToString(value);
            }

            return e;
        }

        public virtual bool Remove()
        {
            if (ParentNode == null)
            {
                return false;
            }

            ParentNode.RemoveChild(this);
            return true;
        }

        public T Parse<T>(string value)
        {
            return (T)Parse(value, typeof(T));
        }

        public object Parse(string value, Type type)
        {
            return Parse(value, type, Type.GetTypeCode(type));
        }

        public object Parse(string value, Type valueType, TypeCode valueTypeCode)
        {
            if (string.IsNullOrEmpty(value) || valueType == null)
            {
                return null;
            }

            if (valueType.IsEnum)
            {
                return Enum.Parse(valueType, value);
            }

            switch (valueTypeCode)
            {
                case TypeCode.Byte:
                    return XmlConvert.ToByte(value);

                case TypeCode.SByte:
                    return XmlConvert.ToSByte(value);

                case TypeCode.Boolean:
                    return XmlConvert.ToBoolean(value);

                case TypeCode.Char:
                    return XmlConvert.ToChar(value);

                case TypeCode.DateTime:
                    return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Local);

                case TypeCode.Decimal:
                    return XmlConvert.ToDecimal(value);

                case TypeCode.Double:
                    return XmlConvert.ToDouble(value);

                case TypeCode.Int16:
                    return XmlConvert.ToInt16(value);

                case TypeCode.Int32:
                    return XmlConvert.ToInt32(value);

                case TypeCode.Int64:
                    return XmlConvert.ToInt64(value);

                case TypeCode.Single:
                    return XmlConvert.ToSingle(value);

                case TypeCode.UInt16:
                    return XmlConvert.ToUInt16(value);

                case TypeCode.UInt32:
                    return XmlConvert.ToUInt32(value);

                case TypeCode.UInt64:
                    return XmlConvert.ToUInt64(value);

                case TypeCode.String:
                    return value;
            }

            if (Types.Color.Equals(valueType))
            {
                if (ParseHexNumber(value, 0, 2, out int a) && ParseHexNumber(value, 2, 2, out int b) && ParseHexNumber(value, 4, 2, out int g) && ParseHexNumber(value, 6, 2, out int r))
                {
                    return Color.FromArgb(a, r, g, b);
                }
            }
            else if (Types.TimeSpan.Equals(valueType))
            {
                return TimeSpan.ParseExact(value, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
            }
            else if (valueType.IsArray && Types.Bytes.Equals(valueType))
            {
                return value.HexStringToByteArray();
            }
            else if (Types.Bitmap.Equals(valueType))
            {
                return (Parse(value, Types.Bytes, TypeCode.Object) as byte[]).ToBitmap();
            }

            return DeserializeObject(value, valueType);
        }

        public bool ParseHexNumber(string s, int startIndex, int length, out int i)
        {
            return int.TryParse(s.Substring(startIndex, length), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i);
        }

        public string ToString(object value)
        {
            if (value.HasValue())
            {
                return ToString(value, value.GetType());
            }

            return default;
        }

        public string ToString(object value, Type type)
        {
            return ToString(value, type, Type.GetTypeCode(type));
        }

        public string ToString(object value, Type valueType, TypeCode valueTypeCode)
        {
            if (valueType.IsEnum)
            {
                return Enum.GetName(valueType, value);
            }

            switch (valueTypeCode)
            {
                case TypeCode.Byte:
                    return XmlConvert.ToString((byte)value);

                case TypeCode.SByte:
                    return XmlConvert.ToString((sbyte)value);

                case TypeCode.Boolean:
                    return XmlConvert.ToString((bool)value);

                case TypeCode.Char:
                    return XmlConvert.ToString((char)value);

                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);

                case TypeCode.Decimal:
                    return XmlConvert.ToString((decimal)value);

                case TypeCode.Double:
                    return XmlConvert.ToString((double)value);

                case TypeCode.Int16:
                    return XmlConvert.ToString((short)value);

                case TypeCode.Int32:
                    return XmlConvert.ToString((int)value);

                case TypeCode.Int64:
                    return XmlConvert.ToString((long)value);

                case TypeCode.Single:
                    return XmlConvert.ToString((float)value);

                case TypeCode.UInt16:
                    return XmlConvert.ToString((ushort)value);

                case TypeCode.UInt32:
                    return XmlConvert.ToString((uint)value);

                case TypeCode.UInt64:
                    return XmlConvert.ToString((ulong)value);

                case TypeCode.String:
                    return (string)value;
            }

            if (valueType.IsArray)
            {
                if (Types.Bytes.Equals(valueType))
                {
                    return (value as byte[]).ToHexString();
                }
            }

            if (Types.Color.Equals(valueType))
            {
                var c = (Color)value; return "{0:X2}{1:X2}{2:X2}{3:X2}".FormatInvariant(c.A, c.B, c.G, c.R);
            }

            if (Types.TimeSpan.Equals(valueType))
            {
                return ((TimeSpan)value).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
            }

            if (Types.Bitmap.Equals(valueType))
            {
                return (value as Bitmap).GetBytes().ToString();
            }

            return SerializeObject(value, valueType);
        }

        string SerializeObject(object component, Type componentType)
        {
            var builder = new DbConnectionStringBuilder();
            var properties = componentType.GetPropertyDescriptors();

            foreach (PropertyDescriptor property in properties)
            {
                if (property.IsReadOnly)
                {
                    continue;
                }

                builder.Add(property.Name, ToString(property.GetValue(component), property.PropertyType));
            }

            return builder.ConnectionString;
        }

        object DeserializeObject(string value, Type componentType)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string propertyValue;
            PropertyDescriptor property;

            var component = Activator.CreateInstance(componentType);
            var properties = componentType.GetPropertyDescriptors();
            var builder = new DbConnectionStringBuilder { ConnectionString = value };

            foreach (string keyword in builder.Keys)
            {
                property = properties.Find(keyword, true);

                if (property == null || property.IsReadOnly)
                {
                    continue;
                }

                propertyValue = builder[keyword] as string;

                if (propertyValue.HasValue())
                {
                    property.SetValue(component, Convert.ChangeType(propertyValue, property.PropertyType, CultureInfo.InvariantCulture));
                }
            }

            return component;
        }
    }
}
