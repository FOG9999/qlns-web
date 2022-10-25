using System;
using System.Data;

namespace VIETTEL
{
    public static class DateTimeExtension
    {
        #region DateTime
        public static string GetTimeStamp(this DateTime dt)
        {
            return dt.ToString("ddMMyyyyHHmmss");
        }

        public static string ToStringNgay(this DateTime dt)
        {
            var ngay = "Ngày {0} tháng {1} năm {2}";
            var result = string.Format(ngay, dt.Day, dt.Month, dt.Year);
            return result;
        }

        public static string ToStringThang(this DateTime dt)
        {
            var ngay = "Ngày       tháng {0} năm {1}";
            var result = string.Format(ngay, dt.Month, dt.Year);
            return result;
        }

        public static string ToStringNam(this DateTime dt)
        {
            var ngay = "Ngày       tháng      năm {0}";
            var result = string.Format(ngay, dt.Year);
            return result;
        }

        public static string ToStringDate(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy");
        }

        public static string ToStringMinute(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy HH:mm");
        }

        public static DateTime ToDateTime(this string dt)
        {
            return Convert.ToDateTime(dt, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);
        }
        #endregion



        #region DS
        public static DataTable getDsLoaiThoigian()
        {
            DataTable dtILoaiTG = new DataTable();
            dtILoaiTG.Columns.Add("MaLoaiTG", typeof(String));
            dtILoaiTG.Columns.Add("TenLoaiTG", typeof(String));
            DataRow Row;
            Row = dtILoaiTG.NewRow();
            Row[0] = Convert.ToString("0");
            Row[1] = Convert.ToString("Tổng hợp");
            dtILoaiTG.Rows.Add(Row);
            Row = dtILoaiTG.NewRow();
            Row[0] = Convert.ToString("1");
            Row[1] = Convert.ToString("Quý");
            dtILoaiTG.Rows.Add(Row);
            Row = dtILoaiTG.NewRow();
            Row[0] = Convert.ToString("2");
            Row[1] = Convert.ToString("Tháng");
            dtILoaiTG.Rows.Add(Row);
            return dtILoaiTG;
        }

        public static DataTable getDSTGTongHop()
        {
            DataTable dtTGTongHop = new DataTable();
            dtTGTongHop.Columns.Add("MaTG", typeof(String));
            dtTGTongHop.Columns.Add("TenTG", typeof(String));
            DataRow Row;
            Row = dtTGTongHop.NewRow();
            Row[0] = Convert.ToString("0");
            Row[1] = Convert.ToString("Cả năm");
            dtTGTongHop.Rows.Add(Row);
            return dtTGTongHop;
        }

        public static DataTable getDSThang()
        {
            DataTable dtThang = new DataTable();
            dtThang.Columns.Add("MaTG", typeof(String));
            dtThang.Columns.Add("TenTG", typeof(String));
            DataRow Row;
            for (int i = 1; i < 13; i++)
            {
                Row = dtThang.NewRow();
                Row[0] = Convert.ToString(i);
                Row[1] = Convert.ToString(i);
                dtThang.Rows.Add(Row);
            }
            return dtThang;
        }

        public static DataTable getDSQuy()
        {
            DataTable dtQuy = new DataTable();
            dtQuy.Columns.Add("MaTG", typeof(String));
            dtQuy.Columns.Add("TenTG", typeof(String));
            DataRow Row;
            for (int i = 1; i < 5; i++)
            {
                Row = dtQuy.NewRow();
                Row[0] = Convert.ToString(i);
                Row[1] = Convert.ToString(i);
                dtQuy.Rows.Add(Row);
            }
            return dtQuy;
        }
        #endregion
    }
}
