using System;
using System.Data;

namespace Viettel.Extensions
{
    public static class CurrencyExtension
    {

        #region Vnd
        public static DataTable getDSDonViTinh() {
            DataTable dt = new DataTable();
            dt.Columns.Add("rSo", typeof(int));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["rSo"] = "1";
            dr["sTen"] = "đồng";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["rSo"] = "1000";
            dr["sTen"] = "1.000 đồng";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["rSo"] = "1000000";
            dr["sTen"] = "triệu đồng";
            dt.Rows.InsertAt(dr, 2);
            return dt;
        }

        #endregion
    }
}
