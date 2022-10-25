using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> values, string separator = ",")
        {
            return string.Join(separator, values);
        }
    }
}