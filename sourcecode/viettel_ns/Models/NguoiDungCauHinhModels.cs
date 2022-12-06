using DomainModel;
using DomainModel.Abstract;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace VIETTEL.Models
{
    public class NguoiDungCauHinhModels
    {

        public static Object iNamLamViec
        {
            get;
            set;
        }

        public static String MaNguoiDung { get; set; }

        //public static Boolean SuaCauHinh(String sID_MaNguoiDung, Object options)
        //{
        //    Boolean vR = false;
        //    Bang bang = new Bang("DC_NguoiDungCauHinh");
        //    bang.MaNguoiDungSua = sID_MaNguoiDung;

        //    Boolean okUpdate = false;
        //    PropertyInfo[] properties = options.GetType().GetProperties();
        //    for (int i = 0; i < properties.Length; i++)
        //    {
        //        bang.CmdParams.Parameters.AddWithValue("@" + properties[i].Name, properties[i].GetValue(options, null));
        //        okUpdate = true;
        //    }
        //    if (okUpdate)
        //    {
        //        SqlCommand cmd = new SqlCommand("SELECT iID_MaNguoiDungCauHinh FROM DC_NguoiDungCauHinh WHERE sID_MaNguoiDungTao=@sID_MaNguoiDungTao");
        //        cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", sID_MaNguoiDung);
        //        int iID_MaNguoiDungCauHinh = Convert.ToInt32( Connection.GetValue(cmd, -1));
        //        cmd.Dispose();
        //        if (iID_MaNguoiDungCauHinh >= 0)
        //        {
        //            bang.DuLieuMoi = false;
        //            bang.GiaTriKhoa = iID_MaNguoiDungCauHinh;
        //        }
        //        bang.Save();
        //        vR = true;
        //        iNamLamViec=LayCauHinhChiTiet(sID_MaNguoiDung,"iNamLamViec");
        //    }
        //    return vR;
        //}

        public static Boolean SuaCauHinh(String sID_MaNguoiDung, Object options)
        {
            Boolean vR = false;
            Bang bang = new Bang("DC_NguoiDungCauHinh");
            bang.MaNguoiDungSua = sID_MaNguoiDung;

            Boolean okUpdate = false;
            PropertyInfo[] properties = options.GetType().GetProperties();

            if (properties.Count() == 0)
                return false;

            for (int i = 0; i < properties.Length; i++)
            {
                bang.CmdParams.Parameters.AddWithValue("@" + properties[i].Name, properties[i].GetValue(options, null));
            }

            SqlCommand cmd = new SqlCommand("SELECT iID_MaNguoiDungCauHinh FROM DC_NguoiDungCauHinh WHERE sID_MaNguoiDungTao=@sID_MaNguoiDungTao");
            cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", sID_MaNguoiDung);
            int iID_MaNguoiDungCauHinh = Convert.ToInt32(Connection.GetValue(cmd, -1));
            cmd.Dispose();
            if (iID_MaNguoiDungCauHinh >= 0)
            {
                bang.DuLieuMoi = false;
            }
            else
            {
                bang.DuLieuMoi = true;
            }
            bang.GiaTriKhoa = iID_MaNguoiDungCauHinh;
            bang.Save();
            vR = true;
            iNamLamViec = LayCauHinhChiTiet(sID_MaNguoiDung, "iNamLamViec");
            return vR;
        }

        public static DataTable LayCauHinh(String sID_MaNguoiDung)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM DC_NguoiDungCauHinh WHERE sID_MaNguoiDungTao=@sID_MaNguoiDungTao");
            cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", sID_MaNguoiDung);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();

            // longsam fix
            if (vR == null || vR.Rows.Count == 0)
            {
                var config = new
                {
                    iNamLamViec = DateTime.Now.Year,
                    iThangLamViec = DateTime.Now.Month,
                    iID_MaNamNganSach = 2,
                    iID_MaNguonNganSach = 1,
                    sID_MaNguoiDungTao = sID_MaNguoiDung,
                };
                SuaCauHinh(sID_MaNguoiDung, config);
                return LayCauHinh(sID_MaNguoiDung);
            }
            return vR;
        }

        public static Object LayCauHinhChiTiet(String sID_MaNguoiDung, String TenTruong)
        {
            Object vR = null;
            DataTable dt;
            SqlCommand cmd = new SqlCommand("SELECT * FROM DC_NguoiDungCauHinh WHERE sID_MaNguoiDungTao=@sID_MaNguoiDungTao");
            cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", sID_MaNguoiDung);
            dt = Connection.GetDataTable(cmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Columns.IndexOf(TenTruong) >= 0 && dt.Rows.Count > 0)
                {
                    vR = dt.Rows[0][TenTruong];
                }
                dt.Dispose();
            }
            cmd.Dispose();

            return vR;
        }
        public static String ThangTinhSoDu_TKChiTiet(String iNamLamViec)
        {
            String vR = "";
            String SQLTK = "SELECT sThamSo FROM KT_DanhMucThamSo WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND sKyHieu=0";
            SqlCommand cmd = new SqlCommand(SQLTK);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            vR = Connection.GetValueString(cmd, "0");
            cmd.Dispose();
            return vR;
        }

        public static string LayNamLamViec(string MaND)
        {
            DataTable dt = LayCauHinh(MaND);
            string iNamLamViec = DateTime.Now.Year.ToString();
            if (dt.Rows.Count > 0)
            {
                iNamLamViec = Convert.ToString(dt.Rows[0]["iNamLamViec"]);
            }

            return iNamLamViec;
        }

    }
}
