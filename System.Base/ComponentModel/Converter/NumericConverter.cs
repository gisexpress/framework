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

using System.Globalization;

namespace System.ComponentModel
{
    public class NumericConverter<T> : TypeConverter
    {
        static NumericConverter()
        {
            TypeCode = Type.GetTypeCode(Type = typeof(T));
        }

        public NumericConverter()
        {
            switch (TypeCode)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    Format = "N";
                    break;
            }
        }

        public string Format
        {
            get;
            set;
        }

        protected static readonly Type Type;
        protected static readonly TypeCode TypeCode;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return Types.String.IsEquivalent(sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return Types.String.Equals(destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return FromString(value as string, culture);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (Types.String.Equals(destinationType))
            {
                return ToString(Format, value, culture ?? Localization.Language);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public static string ToString(string format, object value)
        {
            return value as string ?? ToString(format, value ?? default(T), Localization.Language);
        }

        public static T FromString(string value)
        {
            return (T)FromString(value, Localization.Language);
        }

        public string ToString(object value, IFormatProvider provider)
        {
            return ToString(Format, value, provider);
        }

        public static string ToString(string format, object value, IFormatProvider provider)
        {
            if ((value as string).IsNull() && !value.IsNullOrDBNull())
            {
                try
                {
                    switch (TypeCode)
                    {
                        case TypeCode.Byte:
                            return Convert.ToByte(value).ToString(format, provider);
                        case TypeCode.SByte:
                            return Convert.ToSByte(value).ToString(format, provider);
                        case TypeCode.Int16:
                            return Convert.ToInt16(value).ToString(format, provider);
                        case TypeCode.UInt16:
                            return Convert.ToUInt16(value).ToString(format, provider);
                        case TypeCode.Int32:
                            return Convert.ToInt32(value).ToString(format, provider);
                        case TypeCode.UInt32:
                            return Convert.ToUInt32(value).ToString(format, provider);
                        case TypeCode.Int64:
                            return Convert.ToInt64(value).ToString(format, provider);
                        case TypeCode.UInt64:
                            return Convert.ToUInt64(value).ToString(format, provider);
                        case TypeCode.Single:
                            return Convert.ToSingle(value).ToString(format, provider);
                        case TypeCode.Double:
                            return Convert.ToDouble(value).ToString(format, provider);
                        case TypeCode.Decimal:
                            return Convert.ToDecimal(value).ToString(format, provider);
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }

            return value as string;
        }

        public static object FromString(string value, CultureInfo culture)
        {
            object r = default(T);

            if (!string.IsNullOrEmpty(value))
            {
                switch (TypeCode)
                {
                    case TypeCode.Byte:
                        Byte r1;
                        if (Byte.TryParse(value, NumberStyles.Integer, culture, out r1)) r = r1;
                        break;
                    case TypeCode.SByte:
                        SByte r2;
                        if (SByte.TryParse(value, NumberStyles.Integer, culture, out r2)) r = r2;
                        break;
                    case TypeCode.Int16:
                        Int16 r3;
                        if (Int16.TryParse(value, NumberStyles.Integer, culture, out r3)) r = r3;
                        break;
                    case TypeCode.UInt16:
                        UInt16 r4;
                        if (UInt16.TryParse(value, NumberStyles.Integer, culture, out r4)) r = r4;
                        break;
                    case TypeCode.Int32:
                        Int32 r5;
                        if (Int32.TryParse(value, NumberStyles.Integer, culture, out r5)) r = r5;
                        break;
                    case TypeCode.UInt32:
                        UInt32 r6;
                        if (UInt32.TryParse(value, NumberStyles.Integer, culture, out r6)) r = r6;
                        break;
                    case TypeCode.Int64:
                        Int64 r7;
                        if (Int64.TryParse(value, NumberStyles.Integer, culture, out r7)) r = r7;
                        break;
                    case TypeCode.UInt64:
                        UInt64 r8;
                        if (UInt64.TryParse(value, NumberStyles.Integer, culture, out r8)) r = r8;
                        break;
                    case TypeCode.Single:
                        Single r9;
                        if (Single.TryParse(value, NumberStyles.Float, culture, out r9)) r = r9;
                        break;
                    case TypeCode.Double:
                        Double r10;
                        if (Double.TryParse(value, NumberStyles.Float, culture, out r10)) r = r10;
                        break;
                    case TypeCode.Decimal:
                        Decimal r11;
                        if (Decimal.TryParse(value, NumberStyles.Float, culture, out r11)) r = r11;
                        break;
                }
            }

            return r;
        }
    }
}
