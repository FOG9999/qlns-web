using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIETTEL
{
    public static class MvcExtension
    {

        #region httprequest

        public static int GetQueryStringValue(this HttpRequestBase request, string key, int defaultValue = 0)
        {
            if (request.QueryString.AllKeys.Contains(key))
            {
                try
                {
                    var v = 0;
                    var isValid = int.TryParse(request.QueryString[key], out v);
                    //if (!isValid)
                    //{
                    //    int.TryParse(request.QueryString[defaultValue], out v);
                    //}
                    return v;
                }
                catch (Exception)
                {

                }
            }

            return defaultValue;
        }

        public static string GetQueryString(this HttpRequestBase request, string key, string defaultValue = "")
        {
            if (request.QueryString.AllKeys.Contains(key))
            {
                try
                {
                    var value = request.QueryString[key];
                    return value;
                }
                catch (Exception)
                {
                }
            }

            return defaultValue;
        }

        public static int GetQueryStringValue(this HttpRequest request, string key, int defaultValue = 0)
        {
            if (request.QueryString.AllKeys.Contains(key))
            {
                try
                {
                    var v = 0;
                    var isValid = int.TryParse(request.QueryString[key], out v);
                    //if (!isValid)
                    //{
                    //    int.TryParse(request.QueryString[defaultValue], out v);
                    //}
                    return v;
                }
                catch (Exception)
                {

                }
            }

            return defaultValue;
        }

        public static string GetQueryString(this HttpRequest request, string key, string defaultValue = "")
        {
            if (request.QueryString.AllKeys.Contains(key))
            {
                try
                {
                    var value = request.QueryString[key];
                    return value;
                }
                catch (Exception)
                {
                }
            }

            return defaultValue;
        }


        #endregion


        public static SelectList ToSelectList(this DataTable dt,
            string valueField,
            string textField,
            string defaultValueField = null,
            string defaultTextField = "-- Tất cả --")
        {
            var list = new List<dynamic>();

            if (dt == null || valueField == null || valueField.Trim().Length == 0
                || textField == null || textField.Trim().Length == 0)
                return new SelectList(list);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var value = row[valueField].ToString();
                var text = row[textField].ToString();

                if (!string.IsNullOrWhiteSpace(value) &&
                    !string.IsNullOrWhiteSpace(text))
                    list.Add(new
                    {
                        value = value,
                        text = text
                    });
            }

            if (!string.IsNullOrWhiteSpace(defaultValueField))
            {
                list.Insert(0, new
                {
                    value = defaultValueField,
                    text = defaultTextField,
                });
            }

            return new SelectList(list.AsEnumerable(), "value", "text");
        }

        public static SelectList ToSelectList(this DataTable dt,
            string valueField,
            string textField,
            object selectField)
        {
            var list = new List<dynamic>();

            if (dt == null || valueField == null || valueField.Trim().Length == 0
                || textField == null || textField.Trim().Length == 0)
                return new SelectList(list);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var value = row[valueField].ToString();
                var text = row[textField].ToString();

                if (!string.IsNullOrWhiteSpace(value) &&
                    !string.IsNullOrWhiteSpace(text))
                    list.Add(new
                    {
                        value = value,
                        text = text
                    });
            }

            return new SelectList(list.AsEnumerable(), "value", "text", selectField);
        }

        public static SelectList ToSelectList(this IEnumerable source,
            string value,
            string text,
            string defaultValueField = null,
            string defaultTextField = "-- Tất cả --")
        {
            return new SelectList(source, value, text);
        }

        public static SelectList ToSelectList(this IEnumerable source)
        {
            return new SelectList(source, "value", "text");
        }


        public static SelectList ToSelectList(this Dictionary<string, string> source,
            string defaultValueField = null,
            string defaultTextField = "-- Tất cả --",
            string group = null,
            object selectedValues = null)
        {



            var list = new List<dynamic>();
            if (!string.IsNullOrWhiteSpace(defaultValueField))
            {
                list.Add(new
                {
                    value = defaultValueField,
                    text = defaultTextField,
                    group = group,
                });
            }

            source.ToList().ForEach(x =>
            {
                list.Add(new
                {
                    value = x.Key,
                    text = x.Value,
                    group = group,
                });
            });


            return new SelectList(list, "value", "text", "group", selectedValues);
        }

        public static SelectList ToSelectList<T>(this Dictionary<T, string> source,
           T defaultValueField,
           string defaultTextField) where T : class
        {
            var list = new List<dynamic>() {
                     new
                {
                    value = defaultValueField,
                    text = defaultTextField,
                } };

            source.ToList().ForEach(x =>
            {
                list.Add(new
                {
                    value = x.Key,
                    text = x.Value,
                });
            });

            return new SelectList(list, "value", "text");
        }

        public static SelectList ToSelectList(this IEnumerable source,
            string value,
            string text, 
            string selectedValue)
        {
            return new SelectList(source, value, text, selectedValue);
        }

        //public static SelectList ToSelectList<T>(this IEnumerable<T> source,
        //    string value = "value",
        //    string text = "text",
        //    string defaultValueField = null,
        //    string defaultTextField = "-- Tất cả --")
        //{
        //    if (!string.IsNullOrWhiteSpace(defaultValueField))
        //    {
        //        source.ToList<T>().Add(new
        //        {
        //            value = defaultValueField,
        //            text = defaultTextField
        //        });
        //    }
            
        //    return new SelectList(source, value, text);
        //}

        #region url

        public static Uri UrlOriginal(this HttpRequestBase request)
        {
            string hostHeader = request.Headers["host"];

            return new Uri(string.Format("{0}://{1}{2}",
               request.Url.Scheme,
               hostHeader,
               request.RawUrl));
        }


        /// &lt;summary&gt;
        /// Constructs a QueryString (string).
        /// Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"
        /// &lt;/summary&gt;
        public static string ConstructQueryString(this NameValueCollection parameters)
        {
            var items = new List<string>();

            foreach (string name in parameters)
                items.Add(string.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));

            return string.Join("&", items.ToArray());
            //return string.Join("&amp;", items.ToArray());
        }

        //public static Dictionary<string, object> ToDictionary(this NameValueCollection nvc)
        //{
        //    return nvc.AllKeys.ToDictionary(k => k, k => (object)nvc[k]);
        //}

        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }

        public static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public static string GetBaseUrl(this HtmlHelper helper)
        {
            return GetBaseUrl();
        }

        public static string GetBaseUrl(this Controller controller)
        {
            return GetBaseUrl();
        }

        public static string GetFullUrl(string url)
        {
            var fullUrl = GetBaseUrl() + url;
            return fullUrl;
        }

        public static string GetFullUrl(this HtmlHelper helper, string url)
        {
            var fullUrl = GetBaseUrl() + url;
            return fullUrl.Replace("//", "/");
        }


        #endregion
    }
}
