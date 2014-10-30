using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;

namespace FixSchema
{
    public class FixDataTypes
    {
        public enum FixDataType
        {
            INT,
            FLOAT,
            QTY,
            PRICE,
            PRICEOFFSET,
            AMT,
            CHAR,
            BOOLEAN,
            STRING,
            MULTIPLEVALUESTRING,
            CURRENCY,
            EXCHANGE,
            UTCTIMESTAMP,
            UTCTIMEONLY,
            LOCALMKTDATE,
            UTCDATE,
            DATA,
            MONTHYEAR,
            DAYOFMONTH
        }

        public enum FixBaseType
        {
            INT,
            FLOAT,
            CHAR,
            STRING,
        }

        public static FixBaseType GetFixBaseType(FixDataType fdt)
        {
            switch (fdt)
            {
                case FixDataType.INT:
                case FixDataType.DAYOFMONTH:
                    return FixBaseType.INT;
                
                case FixDataType.FLOAT:
                case FixDataType.QTY:
                case FixDataType.PRICE:
                case FixDataType.PRICEOFFSET:
                case FixDataType.AMT:
                    return FixBaseType.FLOAT;

                case FixDataType.CHAR:
                case FixDataType.BOOLEAN:
                    return FixBaseType.CHAR;

                case FixDataType.STRING:
                case FixDataType.MULTIPLEVALUESTRING:
                case FixDataType.CURRENCY:
                case FixDataType.EXCHANGE:
                case FixDataType.UTCTIMESTAMP:
                case FixDataType.UTCTIMEONLY:
                case FixDataType.LOCALMKTDATE:
                case FixDataType.UTCDATE:
                case FixDataType.MONTHYEAR:
                    return FixBaseType.STRING;

                default:
                    return FixBaseType.STRING;
            }
        }

        public static Type GetNativeBaseType(FixDataType fdt)
        {
            switch (fdt)
            {
                case FixDataType.INT:
                case FixDataType.DAYOFMONTH:
                    return typeof(int);

                case FixDataType.FLOAT:
                case FixDataType.QTY:
                case FixDataType.PRICE:
                case FixDataType.PRICEOFFSET:
                case FixDataType.AMT:
                    return typeof(Double);

                case FixDataType.CHAR:
                    return typeof(char);

                case FixDataType.BOOLEAN:
                    return typeof(bool);

                case FixDataType.STRING:
                case FixDataType.MULTIPLEVALUESTRING:
                case FixDataType.CURRENCY:
                case FixDataType.EXCHANGE:
                case FixDataType.UTCTIMESTAMP:
                case FixDataType.UTCTIMEONLY:
                case FixDataType.LOCALMKTDATE:
                case FixDataType.UTCDATE:
                case FixDataType.MONTHYEAR:
                    return typeof(string);

                default:
                    return typeof(string);
            }
        }

        public static FixDataType StringToEnumTransformer(string t)
        {
            return (FixDataType)Enum.Parse(typeof(FixDataType), t);
        } 
      
        public static bool Compare(Field f, string l, string r)
        {
            Type fType = GetNativeBaseType(f.FixType);

            var v1 = Convert.ChangeType(l, fType, CultureInfo.InvariantCulture);
            var v2 = Convert.ChangeType(r, fType, CultureInfo.InvariantCulture);

            if (v1.Equals(v2))
            {
                return true;
            }

            return false;
        }

        public T CastType<T>(object input, Type t)
        {
            return (T) Convert.ChangeType(input, t);
        }


    }
}
