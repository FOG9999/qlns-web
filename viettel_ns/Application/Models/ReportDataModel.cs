using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace VIETTEL.Models
{
    public class ReportDataModel
    {
        public ReportDataModel()
        {
            arrMoTa1 = new ArrayList();
            arrMoTa2 = new ArrayList();
            arrMoTa3 = new ArrayList();

            Values = new Dictionary<string, object>();
        }

        public DataTable dtDuLieu { get; set; }
        public DataTable dtDuLieuAll { get; set; }

        public DataTable dtGhiChu { get; set; }

        public ArrayList arrMoTa1 { get; set; }
        public ArrayList arrMoTa2 { get; set; }
        public ArrayList arrMoTa3 { get; set; }

        public int ColumnsCount { get; set; }
        public double Sum { get; set; }
        public Dictionary<string, object> Values { get; private set; }

        #region public methods

        public string GhiChu(string ghiChu)
        {
            Func<DataRow, string> func = x => $"- {x["Ng"]}-{x["sMoTa"]}({x["sGhiChu"]}): {double.Parse(x["rTuChi"].ToString()).ToString("###,###")} đ";
            return GhiChu(ghiChu, func);
        }

        public string GhiChu(string ghiChu, Func<DataRow, string> func)
        {
            var result = string.Empty;

            if (dtGhiChu != null && dtGhiChu.Rows.Count != 0)
            {
                var builder = new StringBuilder();

                result = dtGhiChu.AsEnumerable()
                    .Select(x => func(x))
                    .Join(Environment.NewLine);

            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                if (!string.IsNullOrWhiteSpace(ghiChu))
                {
                    result = result + Environment.NewLine + ghiChu;
                }
            }
            else
            {
                result = ghiChu;
            }

            return result;
        }

        #endregion
    }
}
