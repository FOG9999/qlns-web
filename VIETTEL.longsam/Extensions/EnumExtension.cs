using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Viettel.Extensions
{
    public class EnumHelper
    {
        public static IDictionary<int, string> ToValueDictionary<T>() where T : struct
        {
            var dictionary = new Dictionary<int, string>();

            var values = Enum.GetValues(typeof(T));

            foreach (var value in values)
            {
                int key = (int)value;

                dictionary.Add(key, value.ToString());
            }

            return dictionary;
        }
        public static IDictionary<string, string> ToDictionary<T>() where T : struct
        {
            var dictionary = new Dictionary<string, string>();

            var values = Enum.GetValues(typeof(T));

            foreach (var value in values)
            {
                var key = value.ToString();
                dictionary.Add(key, GetDescription((Enum)value));
            }

            return dictionary;
        }

        public static IDictionary<int, string> ToDescriptionDictionary<T>() where T : struct
        {
            var dictionary = new Dictionary<int, string>();

            var values = Enum.GetValues(typeof(T));

            foreach (var value in values)
            {
                var key = (int)value;
                dictionary.Add(key, GetDescription((Enum)Enum.Parse(typeof(T), value.ToString())));
            }

            return dictionary;
        }

        public static string GetDescription(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute.Description;
        }
    }
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            return EnumHelper.GetDescription(value);
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
        }
    }
}
