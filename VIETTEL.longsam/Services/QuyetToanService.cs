using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using VIETTEL.Helpers;

namespace Viettel.Services
{
    public interface IQuyetToanService : IServiceBase
    {
        DataTable GetPhongBans(string iNamLamViec, string username);
        QTA_ChungTu GetChungTu(Guid id);
        //IEnumerable<QTA_ChungTu> GetAll(string iNamLamViec, string username, string lns1 = "1", int page = 1, int pageSize = 10);
        IEnumerable<dynamic> GetAll(string iNamLamViec, string username, string iID_MaPhongBan, string lns1 = "1", int page = 1, int pageSize = 10);
        int GetMax();
        Dictionary<string, string> GetLoaiThongTri();
        Dictionary<string, string> GetLoaiQuyetToan();
        DataTable GetLnsThongTri(string iID_MaChungTu, string iNamLamViec, string loaiThongTri);
        DataTable GetDonViThongTri(string iNamLamViec, string username, string iThang_Quy, string iID_MaNamNganSach, string sLNS, string iID_MaPhongBan);
        string GetGhiChuThongTri(string username, string sTen, string iID_MaDonVi);
        bool UpdateGhiChuThongTri(string username, string sTen, string iID_MaDonVi, string ghiChu);
        DataTable QuyetToanChiTiet(string iID_MaChungTu, string sLNS, int dvt = 1);
        DataTable QuyetToanTongHop(string iID_MaChungTu, string sLNS, int dvt = 1);
        DataTable GetLoaiThongTri(int loai, string sDSLNS);
        DataTable IncludeDonVi();
        DataTable ReportType();

        #region donvis

        DataTable GetDonVis(string iNamLamViec, string username, string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi);


        #endregion

        #region chungtuchitiet

        bool DeleteChungTuChiTiet(string id);
        bool UpdateChungTuChiTiet(QTA_ChungTuChiTiet entity);

        #endregion
    }
    public class QuyetToanService : IQuyetToanService
    {
        protected readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        protected readonly INganSachService _ngansachService = NganSachService.Default;

        #region singleton

        private static IQuyetToanService _default;

        public static IQuyetToanService Default
        {
            get { return _default ?? (_default = new QuyetToanService()); }
        }
        #endregion

        public DataTable GetPhongBans(string iNamLamViec, string username)
        {
            #region sql

            var sql = @"

SELECT  DISTINCT iID_MaPhongBan,sTenPhongBan
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec 
        AND iID_MaPhongBan NOT IN (02)
        AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)

";
            #endregion

            #region get data

            var iID_MaPhongBan = string.Empty;
            var phongban = _ngansachService.GetPhongBan(username);
            iID_MaPhongBan = phongban.sKyHieu;

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());

                    return cmd.GetTable();
                }
            }

            #endregion
        }

        #region chung tu

        public QTA_ChungTu GetChungTu(Guid id)
        {
            var sql = "SELECT * FROM QTA_ChungTu WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<QTA_ChungTu>(
                    sql,
                    param: new
                    {
                        iID_MaChungTu = id,
                    },
                    commandType: CommandType.Text);

                return entity;
            }

        }

        public int GetMax()
        {
            var sql = "SELECT MAX(iSoChungTu) FROM QTA_ChungTu WHERE iTrangThai=1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<int>(
                    sql,
                    commandType: CommandType.Text);

                return entity;
            }
        }

        //        public IEnumerable<QTA_ChungTu> GetAll(string iNamLamViec, string username, string lns1 = "1", int page = 1, int pageSize = 10)
        //        {
        //            var sql = @"

        //SELECT * FROM QTA_ChungTu 
        //WHERE   iTrangThai=1 
        //        AND iNamLamViec=@iNamLamViec
        //        AND (@username is null or sID_MaNguoiDungTao=@username)

        //";
        //            using (var conn = _connectionFactory.GetConnection())
        //            {
        //                var items = conn.Query<QTA_ChungTu>(
        //                    sql,
        //                    param: new
        //                    {
        //                        iNamLamViec,
        //                        username,
        //                    },
        //                    commandType: CommandType.Text);

        //                return items;
        //            }
        //        }

        public IEnumerable<dynamic> GetAll(string iNamLamViec, string username, string iID_MaPhongBan, string lns1 = "1", int page = 1, int pageSize = 10)
        {
            var sql = FileHelpers.GetSqlQuery("qt_get_all.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<dynamic>(
                    sql,
                    param: new
                    {
                        iNamLamViec,
                        iID_MaPhongBan,
                        username,
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public Dictionary<string, string> GetLoaiThongTri()
        {
            return new Dictionary<string, string>()
            {
                { "1010000", "Kinh phí thường xuyên" },
                { "102", "Kinh phí nghiệp vụ" },
                { "104", "Kinh phí bảo đảm" },
                { "105", "Kinh phí doanh nghiệp" },
                { "107", "Kinh phí phòng, chống dịch Covid-19" },
                { "2", "Kinh phí nhà nước" },
                { "3", "Quỹ dự trữ ngoại hối" },
                //{ "3", "Kinh phí đặc biệt" },
                { "4", "Kinh phí khác" },
                { "109", "Kinh phí quốc phòng khác" },
            };
        }

        public DataTable GetLnsThongTri(string iID_MaChungTu, string iNamLamViec, string loaiThongTri)
        {
            var sql = FileHelpers.GetSqlQuery("qt_thongtri_lns.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@sLNS", loaiThongTri);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetDonViThongTri(string iNamLamViec, string username, string iThang_Quy, string iID_MaNamNganSach, string sLNS, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("qt_thongtri_donvi.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
                    cmd.Parameters.AddWithValue("@sLNS", sLNS);
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());

                    return cmd.GetTable();
                }
            }
        }

        public string GetGhiChuThongTri(string username, string sTen, string iID_MaDonVi)
        {
            var sql = FileHelpers.GetSqlQuery("qt_thongtri_ghichu.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<string>(
                    sql,
                    param: new
                    {
                        username,
                        iID_MaDonVi = iID_MaDonVi.ToParamString(),
                        sTen
                    },
                    commandType: CommandType.Text);

                return entity ?? string.Empty;
            }
        }

        public bool UpdateGhiChuThongTri(string username, string sTen, string iID_MaDonVi, string sGhiChu)
        {
            var sql = FileHelpers.GetSqlQuery("qt_thongtri_ghichu_update.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        username,
                        iID_MaDonVi = iID_MaDonVi ?? "NULL",
                        sTen,
                        sGhiChu
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public DataTable QuyetToanChiTiet(string iID_MaChungTu, string sLNS, int dvt = 1)
        {
            var sql = FileHelpers.GetSqlQuery("qt_thongtri_chungtu_chitiet.sql");
            var chungTu = GetChungTu(new Guid(iID_MaChungTu));

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", chungTu.iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@sLNS", sLNS);
                    cmd.Parameters.AddWithValue("@dvt", dvt);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable QuyetToanTongHop(string iID_MaChungTu, string sLNS, int dvt = 1)
        {
            var sql = FileHelpers.GetSqlQuery("qt_thongtri_tonghop.sql");

            var chungTu = GetChungTu(new Guid(iID_MaChungTu));

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", chungTu.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iThang_Quy", chungTu.iThang_Quy);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu.iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@sLNS", sLNS);
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", chungTu.iID_MaNamNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", chungTu.iID_MaDonVi);
                    cmd.Parameters.AddWithValue("@dvt", dvt);

                    return cmd.GetTable();
                }
            }
        }



        #endregion

        #region donvis

        public DataTable GetDonVis(string iNamLamViec, string username, string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi)
        {
            var sql = FileHelpers.GetSqlQuery("qt_donvi.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString(iID_MaPhongBan == "-1"));
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach == "0" || string.IsNullOrWhiteSpace(iID_MaNamNganSach) ? "1,2" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());

                return cmd.GetTable();
            }
        }

        public DataTable GetLoaiThongTri(int loai, string sDSLNS)
        {
            DataTable ds = new DataTable();
            ds.Columns.Add("loaiThongTri", typeof(String));
            ds.Columns.Add("TenThongTri", typeof(String));

            if (loai == 2)
            {
                DataRow dtr = ds.NewRow();
                dtr["loaiThongTri"] = "2";
                dtr["TenThongTri"] = "Kinh phí nhà nước";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiThongTri"] = "3";
                //dtr["TenThongTri"] = "Kinh phí đặc biệt";
                dtr["TenThongTri"] = "Quỹ dự trữ ngoại hối";

                ds.Rows.Add(dtr);
            }
            else if (loai == 0)
            {
                DataRow dtr = ds.NewRow();
                dtr["loaiThongTri"] = "4";
                dtr["TenThongTri"] = "Kinh phí khác";
                ds.Rows.Add(dtr);
            }
            else
            {
                DataRow dtr = ds.NewRow();
                if (sDSLNS.Compare("1010000"))
                {
                    dtr["loaiThongTri"] = "1010000";
                    dtr["TenThongTri"] = "Kinh phí thường xuyên";
                    ds.Rows.Add(dtr);
                }
                else
                {
                    Dictionary<string, string> dsTen = GetLoaiThongTri();
                    dtr["loaiThongTri"] = sDSLNS.Substring(0, 3);
                    dtr["TenThongTri"] = dsTen[sDSLNS.Substring(0, 3)];
                    ds.Rows.Add(dtr);
                }
            }
            return ds;
        }
        public DataTable IncludeDonVi()
        {
            DataTable ds = new DataTable();
            ds.Columns.Add("loai", typeof(String));
            ds.Columns.Add("Type", typeof(String));

            DataRow dtr = ds.NewRow();
            dtr["loai"] = "0";
            dtr["Type"] = "Không";
            ds.Rows.Add(dtr);
            dtr = ds.NewRow();
            dtr["loai"] = "1";
            dtr["Type"] = "Có";
            ds.Rows.Add(dtr);

            return ds;
        }

        public DataTable ReportType()
        {
            DataTable ds = new DataTable();
            ds.Columns.Add("baocao", typeof(String));
            ds.Columns.Add("Report", typeof(String));

            DataRow dtr = ds.NewRow();
            dtr["baocao"] = "1";
            dtr["Report"] = "Phụ lục 2 (Số xét duyệt quyết toán chi thường xuyên NSNN)";
            ds.Rows.Add(dtr);
            dtr = ds.NewRow();
            dtr["baocao"] = "6";
            dtr["Report"] = "Phụ lục 6 (Số xét duyệt quyết toán chi dự trữ quốc gia)";
            ds.Rows.Add(dtr);
            dtr = ds.NewRow();
            dtr["baocao"] = "7";
            dtr["Report"] = "Phụ lục 7 (Số xét duyệt quyết toán chi chương trình mục tiêu)";
            ds.Rows.Add(dtr);

            return ds;
        }

        #endregion

        #region chungtuchitiet

        public bool DeleteChungTuChiTiet(string id)
        {
            var ok = false;
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<QTA_ChungTuChiTiet>(id);
                if (entity != null)
                {
                    entity.iTrangThai = 0;
                    entity.dNgaySua = DateTime.Now;

                    ok = conn.Update(entity);
                }
            }

            return ok;
        }

        public bool UpdateChungTuChiTiet(QTA_ChungTuChiTiet e)
        {
            var ok = false;
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<QTA_ChungTuChiTiet>(e.iID_MaChungTuChiTiet);
                if (entity != null)
                {
                    entity.dNgaySua = DateTime.Now;

                    ok = conn.Update(entity);
                }
            }

            return ok;
        }

        public Dictionary<string, string> GetLoaiQuyetToan()
        {
            return new Dictionary<string, string>()
            {
                { "-1", "Mặc định" },
                { "0", "Xác nhận quyết toán" },
                { "1", "Xác nhận giảm quyết toán" },
            };
        }
        #endregion
    }
}
