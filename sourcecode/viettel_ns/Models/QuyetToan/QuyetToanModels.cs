using DomainModel;
using System;
using System.Data;
using System.Data.SqlClient;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Models
{
    public class QuyetToanListModels
    {
        public QuyetToanListModels(String Loai, String MaND, String iSoChungTu, String dTuNgay, String dDenNgay,
                                   String iID_MaTrangThaiDuyet, String page)
        {
            this.Loai = Loai;
            this.MaND = MaND;
            this.iSoChungTu = iSoChungTu;
            this.dTuNgay = dTuNgay;
            this.dDenNgay = dDenNgay;
            this.iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet;
            this.page = page;
        }

        public String Loai { get; set; }
        public String MaND { get; set; }
        public String iSoChungTu { get; set; }
        public String dTuNgay { get; set; }
        public String dDenNgay { get; set; }
        public String iID_MaTrangThaiDuyet { get; set; }
        public String page { get; set; }
    }

    public class QuyetToanModels
    {
        public static int iID_MaPhanHeQuyetToan = PhanHeModels.iID_MaPhanHeQuyetToan;

        public static DataTable getDSPhongBan(String iNamLamViec, String MaND)
        {
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            String DK = "";


            if (sTenPB == "02" || sTenPB == "2")
            {

            }
            else
            {
                DK = " AND iID_MaPhongBan=@iID_MaPhongBan";
            }

            String SQL = String.Format(@"SELECT DISTINCT iID_MaPhongBan,sTenPhongBan
FROM QTA_ChungTuChiTiet
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} AND iID_MaPhongBan NOT IN (02)
", DK);
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
            DataTable dt = Connection.GetDataTable(cmd);
            DataRow dr = dt.NewRow();
            dr["iID_MaPhongBan"] = "-1";
            dr["sTenPhongBan"] = "--Chọn tất cả các B--";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }



        public static DataTable getDSPhongBan_QuanSo(String iNamLamViec, String MaND)
        {
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            String DK = "";


            if (sTenPB == "02" || sTenPB == "2")
            {

            }
            else
            {
                DK = " AND iID_MaPhongBan=@iID_MaPhongBan";
            }

            String SQL = String.Format(@"SELECT DISTINCT iID_MaPhongBan,sTenPhongBan
FROM QTQS_ChungTuChiTiet
WHERE iTrangThai=1 AND iNamLamViec<=@iNamLamViec {0}
", DK);
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
            DataTable dt = Connection.GetDataTable(cmd);
            DataRow dr = dt.NewRow();
            dr["iID_MaPhongBan"] = "-1";
            dr["sTenPhongBan"] = "--Chọn tất cả các B--";
            dt.Rows.InsertAt(dr, 0);
            dt.Dispose();
            cmd.Dispose();
            return dt;
        }

        public static DataTable getDSNamNganSach()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "0";
            dr["sTen"] = "Tổng hợp";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Năm nay";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Năm trước";
            dt.Rows.InsertAt(dr, 2);
            dt.Dispose();
            return dt;
        }

        public static DataTable getDSTongHop()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "LNS";
            dr["sTen"] = "Đến LNS";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "DonVi";
            dr["sTen"] = "Đến đơn vị";
            dt.Rows.InsertAt(dr, 1);

            dt.Dispose();
            return dt;
        }

        public static DataTable getDSTongHop_SoPheDuyet()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "LNS";
            dr["sTen"] = "Đến LNS";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "KhoiDonVi";
            dr["sTen"] = "Đến khối đơn vị";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "DonVi";
            dr["sTen"] = "Đến đơn vị";
            dt.Rows.InsertAt(dr, 2);

            dt.Dispose();
            return dt;
        }

        public static DataTable getDSMauBaoCao()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "TongHop";
            dr["sTen"] = "Tổng hợp";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "PhuLuc";
            dr["sTen"] = "Phụ lục";
            dt.Rows.InsertAt(dr, 1);
            dt.Dispose();
            return dt;
        }
        public static DataTable getDSNguonNganSach()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "TongHop";
            dr["sTen"] = "Tổng hợp";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "QuocPhong";
            dr["sTen"] = "Quốc phòng";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "NhaNuoc";
            dr["sTen"] = "Nhà nước giao";
            dt.Rows.InsertAt(dr, 2);

            dr = dt.NewRow();
            dr["MaLoai"] = "Khac";
            dr["sTen"] = "Kinh phí khác";
            dt.Rows.InsertAt(dr, 3);
            dt.Dispose();
            return dt;
        }
        public static DataTable getDanhSachTuyChinh()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "TatCa";
            dr["sTen"] = "Tất cả mục lục ngân sách";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "ChiTieu";
            dr["sTen"] = "Chỉ hiện mục có chỉ tiêu";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "KhongChiTieu";
            dr["sTen"] = "Chỉ hiện mục không có chỉ tiêu";
            dt.Rows.InsertAt(dr, 2);

            dr = dt.NewRow();
            dr["MaLoai"] = "DonViDeNghiKhac";
            dr["sTen"] = "Hiện mục đơn vị đề nghị khác số phê duyệt";
            dt.Rows.InsertAt(dr, 3);

            dr = dt.NewRow();
            dr["MaLoai"] = "QuyetToan";
            dr["sTen"] = "Hiện dữ liệu quyết toán đã nhập";
            dt.Rows.InsertAt(dr, 4);
            dt.Dispose();
            return dt;
        }
        public static DataTable getDanhSachTuyChinh_DuToan(String sLNS = "")
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "TatCa";
            dr["sTen"] = "Tất cả mục lục ngân sách";
            dt.Rows.InsertAt(dr, 0);
            dr = dt.NewRow();
            dr["MaLoai"] = "DuToan";
            dr["sTen"] = "Hiện dữ liệu dự toán đã nhập";
            dt.Rows.InsertAt(dr, 1);
            dr = dt.NewRow();
            dr["MaLoai"] = "ChuaDuToan";
            dr["sTen"] = "Hiện dữ liệu dự toán chưa nhập";
            dt.Rows.InsertAt(dr, 2);
            //các ngành phân cấp
            if (@"1040100,1040200,1040300,
                1090100,1090200,1090300,1090400,1090500,1090600,1090700,1090800,1099800,2021100,
                2070000,2070100,2070200,2070300,2070400,2070500,2070600,2070601,2070602,2070700,2070800,2070900,2071000,2071100,2071101,2071102,2071200,
                2071300,2071400,2071401,2071402,2071403,2071404,2071500,2071600,2071700,2071800,2071900,2079900,4050000,4450000,4080300".IndexOf(sLNS) >= 0 || sLNS.Contains("207"))
            {
                dr = dt.NewRow();
                dr["MaLoai"] = "PhanCap";
                dr["sTen"] = "Hiện dữ liệu các ngành phân cấp";
                dt.Rows.InsertAt(dr, 3);

                dr = dt.NewRow();
                dr["MaLoai"] = "SapXepNganhCon";
                dr["sTen"] = "Sắp xếp dữ liêu theo ngành con";
                dt.Rows.InsertAt(dr, 4);
            }
            dt.Dispose();
            return dt;
        }
        public static DataTable GetDanhSachLoaiNSQuyetToan_ThongTri()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "ThuongXuyen";
            dr["sTen"] = "Kinh phí thường xuyên";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "NghiepVu";
            dr["sTen"] = "Kinh phí nghiệp vụ";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "BaoDam";
            dr["sTen"] = "Kinh phí bảo đảm";
            dt.Rows.InsertAt(dr, 2);


            dr = dt.NewRow();
            dr["MaLoai"] = "DoanhNghiep";
            dr["sTen"] = "Kinh phí doanh nghiệp";
            dt.Rows.InsertAt(dr, 3);


            dr = dt.NewRow();
            dr["MaLoai"] = "NhaNuoc";
            dr["sTen"] = "Kinh phí nhà nước";
            dt.Rows.InsertAt(dr, 4);

            dr = dt.NewRow();
            dr["MaLoai"] = "Khac";
            dr["sTen"] = "Kinh phí khác";
            dt.Rows.InsertAt(dr, 5);


            dr = dt.NewRow();
            dr["MaLoai"] = "DacBiet";
            dr["sTen"] = "Kinh phí đặc biệt";
            dt.Rows.InsertAt(dr, 6);

            dr = dt.NewRow();
            dr["MaLoai"] = "QPKhac";
            dr["sTen"] = "Kinh phí quốc phòng khác";
            dt.Rows.InsertAt(dr, 7);

            dt.Dispose();
            return dt;
        }

        public static DataTable GetDotQuyetToan(string iID_MaChungTu, String SoChungTu = "", String TuNgay = "", String DenNgay = "", String iID_MaTrangThaiDuyet = "", int Trang = 1, int SoBanGhi = 0)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);

            if (String.IsNullOrEmpty(SoChungTu) == false && SoChungTu != "")
            {
                DK += " AND iSoChungTu = @iSoChungTu";
                cmd.Parameters.AddWithValue("@iSoChungTu", SoChungTu);
            }
            if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) == false && iID_MaTrangThaiDuyet != Convert.ToString(Guid.Empty))
            {
                DK += " AND iID_MaTrangThaiDuyet = @iID_MaTrangThaiDuyet";
                cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", iID_MaTrangThaiDuyet);
            }
            if (String.IsNullOrEmpty(TuNgay) == false && TuNgay != "")
            {
                DK += " AND dNgayChungTu >= @dTuNgayChungTu";
                cmd.Parameters.AddWithValue("@dTuNgayChungTu", CommonFunction.LayNgayTuXau(TuNgay));
            }
            if (String.IsNullOrEmpty(DenNgay) == false && DenNgay != "")
            {
                DK += " AND dNgayChungTu <= @dDenNgayChungTu";
                cmd.Parameters.AddWithValue("@dDenNgayChungTu", CommonFunction.LayNgayTuXau(DenNgay));
            }

            String SQL = String.Format("SELECT * FROM DT_ChungTu WHERE {0}", DK);
            cmd.CommandText = SQL;
            vR = CommonFunction.dtData(cmd, "dNgayChungTu DESC", Trang, SoBanGhi);
            cmd.Dispose();
            return vR;
        }

        public static DataTable QT_LoaiNganSachQuocPhong(String iID_MaPhongBan)
        {
            #region definition input
            var sql = FileHelpers.GetSqlQuery("qt_GetDS_LNS.sql"); ;
            #endregion

            //String iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
            //if (Convert.ToInt32(iNamLamViec) < 2018) iNamLamViec = "2017";
            //else iNamLamViec = "2018";

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViecViewModel.Current.iNamLamViec);
                cmd.Parameters.AddWithValue("@LNS", "1");
                var dt = cmd.GetTable();

                //DataRow dtr = dt.NewRow();
                //dtr["sLNS"] = "104";
                //dtr["TenHT"] = "104 - Ngân sách bảo đảm";
                //dt.Rows.Add(dtr);

                DataView dv = dt.DefaultView;
                dv.Sort = "sLNS ASC";
                return dv.ToTable();
            }
            #endregion
        }

    }
}
