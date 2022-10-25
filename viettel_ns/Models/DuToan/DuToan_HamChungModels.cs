using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using DomainModel.Abstract;
using System.Text;
namespace VIETTEL.Models
{
    public class DuToan_HamChungModels
    {
        public static String SelectoptGroup(String ParentID, DataTable dt,String Ma, String iID, String sTen)
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("<select id=\"{0}\" name=\"{0}\"  style=\"width: 100%;\">", ParentID+"_"+Ma);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[iID].ToString().Length == 1)
                {
                    str.AppendFormat("<optgroup label=\"{0}\">", dr[sTen]);
                }
                else if (dr[iID].ToString().Length == 2)
                {
                    str.AppendFormat("<optgroup label=\"&nbsp;&nbsp;{0}\">", dr[sTen]);
                }
                else
                {
                    str.AppendFormat("<option value=\"{0}\" title=\"{1}\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{2}</option>"
                                    , dr[iID], dr[sTen], dr[sTen]);
                }

            }
            str.Append("</select>");
            dt.Dispose();
            return str.ToString();
        }
    }
}