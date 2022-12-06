using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace System
{
    public static class ReflectionExtension
    {
        // <summary>
        // Get the name of a static or instance property from a property access lambda.
        // </summary>
        // <typeparam name="T">Type of the property</typeparam>
        // <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
        // <returns>The name of the property</returns>

        public static string GetPropertyName<T>(this object obj, Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }

        public static T MapFrom<T>(this T obj, Dictionary<string, string> source)
        {
            typeof(T).GetProperties()
                .Select((p, index) =>
                    new
                    {
                        index,
                        prop = p
                    }
                ).ToList()
                .ForEach(p =>
                {

                    source.ToList().ForEach(x =>
                    {
                        if (p.prop.Name == x.Key)
                        {

                            if (p.prop.PropertyType == typeof(Guid))
                            {
                                p.prop.SetValue(obj, x.Value == null ? Guid.Empty : Guid.Parse(x.Value));
                            }
                            else
                            {
                                //p.prop.SetValue(obj, x.Value == null ? null : Convert.ChangeType(x.Value, p.prop.PropertyType));
                                p.prop.SetValue(obj, x.Value == null ? null : x.Value);

                            }
                        }
                    });
                });


            return obj;
        }

        public static T MapFrom<T>(this T obj, IEnumerable<KeyValuePair<string, string>> source, int rate = 1)
        {
            typeof(T).GetProperties()
                .Select((p, index) =>
                    new
                    {
                        index,
                        prop = p
                    }
                ).ToList()
                .ForEach(p =>
                {

                    source.ToList().ForEach(x =>
                    {
                        if (p.prop.Name == x.Key)
                        {
                            if (p.prop.PropertyType == typeof(decimal) || p.prop.PropertyType == typeof(decimal?))
                            {
                                p.prop.SetValue(obj, string.IsNullOrWhiteSpace(x.Value) ? 0 : decimal.Parse(x.Value) * rate);
                            }
                            else if (p.prop.PropertyType == typeof(int) || p.prop.PropertyType == typeof(int?))
                            {
                                p.prop.SetValue(obj, string.IsNullOrWhiteSpace(x.Value) ? 0 : int.Parse(x.Value) * rate);
                            }
                            else if (p.prop.PropertyType == typeof(double) || p.prop.PropertyType == typeof(double?))
                            {
                                p.prop.SetValue(obj, string.IsNullOrWhiteSpace(x.Value) ? 0 : double.Parse(x.Value) * rate);
                            }
                            else if (p.prop.PropertyType == typeof(DateTime))
                            {
                                p.prop.SetValue(obj, string.IsNullOrWhiteSpace(x.Value) ? DateTime.MinValue : DateTime.Parse(x.Value));
                            }
                            else if (p.prop.PropertyType == typeof(string))
                            {
                                p.prop.SetValue(obj, string.IsNullOrWhiteSpace(x.Value) ? "" : x.Value.ToString());
                            }
                            else if (p.prop.PropertyType == typeof(bool) || p.prop.PropertyType == typeof(bool?))
                            {
                                p.prop.SetValue(obj, (string.IsNullOrWhiteSpace(x.Value) || x.Value == "0") ? false : true);
                            }
                            else
                                p.prop.SetValue(obj, string.IsNullOrWhiteSpace(x.Value) ? null : Convert.ChangeType(x.Value, p.prop.PropertyType));
                        }
                    });
                });


            return obj;
        }

        public static T Mapper<T>(this T aValue, Dictionary<string, string> aDict) where T : class, new()
        {
            if(aValue == null)
            {
                aValue = new T();
            }

            var result = aValue;
            if (aDict == null || !aDict.Any())
            {
                return result;
            }

            var type = result.GetType();
            var listProperties = type.GetProperties();
            if(listProperties != null && listProperties.Any())
            {
                foreach (var prop in listProperties)
                {
                    if (aDict.ContainsKey(prop.Name))
                    {
                        if(prop.PropertyType == typeof(System.Guid) || prop.PropertyType == typeof(Nullable<System.Guid>))
                        {
                            if(aDict[prop.Name] != null && !string.IsNullOrEmpty(aDict[prop.Name].ToString()))
                            {
                                var guid = Guid.Parse(aDict[prop.Name]);
                                prop.SetValue(result, guid);
                            }
                            continue;
                        }
                        if(aDict[prop.Name] != null)
                        {
                            var value = aDict[prop.Name].ChangeType(prop.PropertyType);
                            prop.SetValue(result, value);
                        }
                    }
                }
            }

            return result;
        }

        public static object ChangeType(this object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        public static T MapFrom<T, TSource>(this T obj, TSource source, string excludeProperties = "")
        {

            var excludes = excludeProperties.Split(',');
            var sourceList = typeof(TSource).GetProperties().Select((p, index) =>
                    new
                    {
                        index,
                        prop = p
                    }
                ).Where(p => !excludes.Any(x => p.prop.Name == x))
                .ToList();


            typeof(T).GetProperties()
                .Select((p, index) =>
                    new
                    {
                        index,
                        prop = p
                    }
                ).ToList()
                .ForEach(p =>
                {

                    sourceList.ForEach(x =>
                    {
                        if (p.prop.Name == x.prop.Name)
                        {
                            //p.prop.SetValue(obj, x.prop.GetValue(source) == null || string.IsNullOrWhiteSpace(x.prop.GetValue(source).ToString()) ? null : Convert.ChangeType(x.Value, p.prop.PropertyType));

                            p.prop.SetValue(obj, x.prop.GetValue(source));
                        }
                    });
                });


            return obj;
        }

        public static Dictionary<string, TValue> ToDictionary<TValue>(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }


        public static T ToValue<T>(this string source)
        {
            T value;
            TryParse(source, out value);
            return value;
        }

        public static bool TryParse<T>(this string input, out T value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter != null && converter.IsValid(input))
            {
                value = (T)converter.ConvertFromString(input);
                return true;
            }

            value = default(T);
            return false;
        }


    }
}
