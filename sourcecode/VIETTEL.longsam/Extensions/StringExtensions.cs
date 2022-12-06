using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VIETTEL
{
    //public static class StringExtensions
    //{
    //    public static string Join(this IEnumerable<string> values, string separator = ",")
    //    {
    //        return string.Join(separator, values);
    //    }
    //}
}


namespace System
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> values, string separator = ",")
        {
            //if (removeEmpty)
            //{
            //    values = values.Where(x => !string.IsNullOrWhiteSpace(x));
            //}

            return string.Join(separator, values);
        }

        public static string JoinFormat(this IEnumerable<string> values, string separator = ",", string format = null)
        {
            //if (removeEmpty)
            //{
            //    values = values.Where(x => !string.IsNullOrWhiteSpace(x));
            //}

            // dinh dang lai chuoi
            if (!string.IsNullOrEmpty(format))
            {
                values = values.Select(x => string.Format(format, x));
            }

            return string.Join(separator, values);
        }


        public static string JoinDistinct(this IEnumerable<string> values, string separator = ",")
        {
            var text = string.Join(separator, values);
            var list = text.Split(separator.ToCharArray()).Distinct().OrderBy(x => x);
            return string.Join(separator, list);
        }

        public static string Join<T>(this IEnumerable<T> values, string separator = ",")
        {
            return string.Join(separator, values);
        }

        public static string[] ToArray(this string text, string separator = ",")
        {
            return text.Split(separator.ToCharArray());
        }

        public static bool Compare(this string text, string value)
        {
            if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            return string.Compare(text.Trim().ToLower(), value.Trim().ToLower(), true) == 0;
        }

        public static bool IsEmpty(this string text, string defaultText = null)
        {
            return string.IsNullOrWhiteSpace(text) || text == defaultText;
        }

        public static bool IsNotEmpty(this string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        public static bool IsContains(this string text, string values)
        {
            if (values.IsEmpty())
                return false;

            return values.Split(',').Contains(text);
        }

        public static IList<string> ToList(this string text, string separator = ",", bool includeEmpty = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            return text.Split(new string[] { separator }, includeEmpty ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static string ToStringAccent(this string text, bool removeEmpty = false)
        {
            if (text.IsEmpty()) return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            var r = stringBuilder.ToString();
            r = r.Replace('Đ', 'D').Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            return removeEmpty ? r.Replace(" ", "") : r;
        }

        public static string ToStringEmpty(this string text)
        {
            if (text.IsEmpty()) return string.Empty;
            return text;
        }

        public static string ToStringNull(this string text)
        {
            if (text.IsEmpty()) return null;
            return text;
        }
    }
}
