using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Extensions
{
    public static class HelperDataTableExtensions
    {
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    tb.Columns.Add(prop.Name, GetCheckType(prop.PropertyType));
                    continue;
                }

                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        public static Type GetCheckType(Type aType)
        {
            if (aType == typeof(DateTime?))
            {
                return typeof(DateTime);
            }
            if (aType == typeof(bool?))
            {
                return typeof(bool);
            }
            if (aType == typeof(decimal?))
            {
                return typeof(decimal);
            }
            if (aType == typeof(double?))
            {
                return typeof(double);
            }
            if (aType == typeof(int?))
            {
                return typeof(int);
            }
            if (aType == typeof(Guid?))
            {
                return typeof(Guid);
            }
            return null;
        }
    }
}
