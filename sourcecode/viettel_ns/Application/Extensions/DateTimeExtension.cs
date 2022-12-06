using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL
{
    public static class DateTimeExtension
    {
        public static string GetTimeStamp(this DateTime dt)
        {
            return dt.ToString("ddMMyyyyHHmm");
        }
    }
}