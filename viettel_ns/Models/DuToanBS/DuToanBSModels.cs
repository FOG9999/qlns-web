using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Models.DuToanBS
{
    public class DuToanBSModels
    {
        private static readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private static readonly INganSachService _nganSachService = NganSachService.Default;

        public static int iID_MaPhanHeQuyetToan = PhanHeModels.iID_MaPhanHeQuyetToan;
        public const int iID_MaPhanHe = PhanHeModels.iID_MaPhanHeDuToan;
        public static string strDSTruongTienTieuDe = "Tên công trình,Ngày,Người,Chi tại kho bạc,Tồn kho,Hàng nhập,Tự chi,Chi tại ngành,Chi tập trung,Hàng mua,Hiện vật,Phân cấp,Dự phòng";
        public static string strDSTruongTienTieuDe_ThuChi = "Tên công trình,Ngày,Người,Chi tại kho bạc,Tồn kho,Hàng nhập,Số tiền,Chi tập trung,Hàng mua,Hiện vật,Phân cấp,Dự phòng";
        public static string strDSTruongTien_So = "rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rHangNhap,rTuChi,rChiTaiNganh,rChiTapTrung,rHangMua,rHienVat,rPhanCap,rDuPhong";
        public static string strDSTruongTien_Xau = "sTenCongTrinh";
        public static string strDSTruongTien = strDSTruongTien_Xau + "," + strDSTruongTien_So;
        public static string strDSDuocNhapTruongTien = "b" + strDSTruongTien.Replace(",", ",b");
        public static string strDSTruongTienDoRong_So = "130,130,130,130,130,130,130,130,130,130,130,130";
        public static string strDSTruongTienDoRong_Xau = "150";
        public static string strDSTruongTienDoRong = strDSTruongTienDoRong_Xau + "," + strDSTruongTienDoRong_So;
        public static string strDSTruongTien_Full = strDSTruongTien;
        public static string strDSTruongTienDoRong_Full = strDSTruongTienDoRong;
        public static string strDSTruongTienTieuDe_Full = strDSTruongTienTieuDe;

        public static bool checkLNSPC(string maChungTu, string sLNS)
        {
            NameValueCollection thongTinCT = DuToanBSChungTuModels.LayThongTinChungTu(maChungTu);
            string[] dsLNS = Convert.ToString(thongTinCT["sDSLNS"]).Split(',');
            if (sLNS.StartsWith("1040100"))
            {
                return true;
            }
            else
            {
                //var lnsPhanCap = System.Configuration.ConfigurationManager.AppSettings["ns_lns_phancap"] ??
                //    "1040100,109,207,2,4050100";

                var lnsPhanCapDefault = "1040100,1040300,109,207,2,4";
                var lnsPhanCap = (System.Configuration.ConfigurationManager.AppSettings["ns_lns_phancap"] + "," + lnsPhanCapDefault).ToArray().JoinDistinct();

                for (int i = 0; i < dsLNS.Length; i++)
                {
                    //if (dsLNS[i].StartsWith("207") ||
                    //    dsLNS[i].StartsWith("109") ||
                    //    dsLNS[i].StartsWith("209") ||
                    //    dsLNS[i].StartsWith("4050000") ||
                    //    dsLNS[i].StartsWith("1040100"))
                    //{
                    //    return true;
                    //}

                    var ok = lnsPhanCap.Split(',').ToList().Any(x => dsLNS[i].StartsWith(x));
                    if (ok)
                        return true;

                }
            }

            return false;
        }
        public static DataTable GetDots(string username, string iID_MaChungTu = "")
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_troly_pm.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", NguoiDungCauHinhModels.LayNamLamViec(username));
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", _nganSachService.GetPhongBan(username).sKyHieu);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@id", iID_MaChungTu.ToParamString());

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }

        public static DataTable GetCapDV()
        {
            var dt = new DataTable();
            dt.Columns.Add("Cap", typeof(String));
            dt.Columns.Add("TenCap", typeof(String));
            
            DataRow dtr = dt.NewRow();
            dtr["Cap"] = "2";
            dtr["TenCap"] = "Cấp 2";
            dt.Rows.Add(dtr);
            dtr = dt.NewRow();
            dtr["Cap"] = "3";
            dtr["TenCap"] = "Cấp 3";
            dt.Rows.Add(dtr);
            
            return dt;
        }

        public static DataTable GetDonviTinh()
        {
            var dt = new DataTable();
            dt.Columns.Add("Dvt", typeof(String));
            dt.Columns.Add("TenDvt", typeof(String));

            DataRow dtr = dt.NewRow();
            dtr["Dvt"] = "1";
            dtr["TenDvt"] = "Dương";
            dt.Rows.Add(dtr);
            dtr = dt.NewRow();
            dtr["Dvt"] = "-1";
            dtr["TenDvt"] = "Âm";
            dt.Rows.Add(dtr);
            return dt;
        }
    }

}
