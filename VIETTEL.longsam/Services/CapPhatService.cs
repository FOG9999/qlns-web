using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using VIETTEL.Helpers;

namespace Viettel.Services
{
    public interface ICapPhatService : IServiceBase
    {
        DataTable GetPhongBans(string iNamLamViec, string username);
        CP_CapPhat GetChungTu(Guid id);
        IEnumerable<dynamic> GetAll(string username, string lns1 = "1", int page = 1, int pageSize = 10);
        int GetMax();
        Dictionary<string, string> GetLoaiThongTri();
        Dictionary<string, string> GetLoaiCapPhat();
        DataTable GetLnsThongTri(string iID_MaCapPhat, string loaiThongTri = null);
        DataTable GetDonViThongTri(string iID_MaCapPhat, string loaiThongTri = null, string sLNS = null);
        DataTable GetNgThongTri(string iID_MaCapPhat);
        DataTable GetDviNgThongTri(string iID_MaCapPhat, string NG);
        string GetGhiChuThongTri(string username, string sTen, string iID_MaDonVi);
        string GetValueThongTriNT(string username, string sTen, string iID_MaDonVi, string loai);
        bool UpdateGhiChuThongTri(string username, string sTen, string iID_MaDonVi, string ghiChu);
        bool UpdateValueThongTriNT(string username, string sTen, string iID_MaDonVi, string value, string loai);
        DataTable CapPhatThongTriChiTiet(string iNamLamViec, string sLNS, string iID_MaDonVi, string iID_MaCapPhat);
        DataTable CapPhatThongTriTongHop(string iNamLamViec, string sLNS, string iID_MaDonVi, string iID_MaCapPhat);
        DataTable getDanhSachNganh(string iNamLamViec, string username);
        DataTable CapPhatTongHopChiTieuSec(string iNamLamViec, string username, string sLNS);
        DataTable CapPhatThongTriSecChiTiet(string iID_MaCapPhat, string nganh, string iID_MaDonVi);
        DataTable CapPhatThongTriSecTongHop(string iID_MaCapPhat, string nganh, string iID_MaDonVi);
        DataTable GetLoaiThongTri(int loai, string sDSLNS);
    }
    public class CapPhatService : ICapPhatService
    {
        public int iID_MaPhanHe = 4;
        protected readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        protected readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly string strDSTruong = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
        private static string strDSTruongTien = "rChiTaiKhoBac,rTonKho,rTuChi,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rPhanCap,rDuPhong";
        private readonly string strDSDuocNhapTruongTien = "b" + strDSTruongTien.Replace(",", ",b");
        private readonly string strDSTruongSapXep = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG";

        #region singleton

        private static ICapPhatService _default;

        public static ICapPhatService Default
        {
            get { return _default ?? (_default = new CapPhatService()); }
        }
        #endregion

        public DataTable GetPhongBans(string iNamLamViec, string username)
        {
            #region sql

            var sql = @"

            SELECT  DISTINCT iID_MaPhongBan,sTenPhongBan
            FROM    CP_CapPhatChiTiet
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

        public CP_CapPhat GetChungTu(Guid id)
        {
            var sql = "SELECT * FROM CP_CapPhat WHERE iTrangThai=1 AND iID_MaCapPhat=@iID_MaCapPhat";

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<CP_CapPhat>(
                    sql,
                    param: new
                    {
                        iID_MaCapPhat = id,
                    },
                    commandType: CommandType.Text);

                return entity;
            }

        }
        public int GetMax()
        {
            var sql = "SELECT MAX(iSoChungTu) FROM CP_CapPhat WHERE iTrangThai=1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<int>(
                    sql,
                    commandType: CommandType.Text);

                return entity;
            }
        }
        public IEnumerable<dynamic> GetAll(string username, string lns1 = "1", int page = 1, int pageSize = 10)
        {
            var phongban = _ngansachService.GetPhongBan(username);
            var iID_MaPhongBan = phongban.sKyHieu;

            var sql = FileHelpers.GetSqlQuery("dt_CapPhat_ChungTu_All.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<dynamic>(
                    sql,
                    param: new
                    {
                        _ngansachService.GetCauHinh(username).iNamLamViec,
                        _ngansachService.GetCauHinh(username).iID_MaNamNganSach,
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
                { "101", "Kinh phí Quốc phòng" },
                { "104", "Kinh phí Bảo đảm" },
                { "105", "Kinh phí Doanh nghiệp" },
                { "107", "Kinh phí phòng, chống dịch Covid-19" },
                { "2", "Kinh phí Nhà nước" },
                { "3", "Kinh phí Đặc biệt" },
                { "4", "Kinh phí Khác" },
                { "109", "Kinh phí Quốc phòng khác" },
            };
        }

        public Dictionary<string, string> GetLoaiCapPhat()
        {
            return new Dictionary<string, string>()
            {
                //{ "-1", "<-- Chọn tính chất cấp thu -->" },
                { "-1", "Mặc định" },
                { "0", "Cấp" },
                //{ "1", "Cấp ứng" },
                //{ "2", "Thu" },
                { "3", "Giảm cấp" },
            };
        }
        public DataTable GetLnsThongTri(string iID_MaCapPhat, string loaiThongTri = null)
        {
            var chungTu = GetChungTu(new Guid(iID_MaCapPhat));
            DataTable dt = new DataTable();

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTri_LNS.sql");

            #endregion

            #region dieu kien chitieu  

            string sLNS = "";
            if (loaiThongTri == "101" || loaiThongTri == "102")
                sLNS = "101,102,104,105,107,109";
            else if (loaiThongTri == "104")
                sLNS = "104";
            else if (loaiThongTri == "105")
                sLNS = "105";
            else if (loaiThongTri == "107")
                sLNS = "107";
            else if (loaiThongTri == "2")
                sLNS = "2";
            else if (loaiThongTri == "4")
                sLNS = "4";
            else if (loaiThongTri == "3")
                sLNS = "3";
            else
                sLNS = "109";

            if (String.IsNullOrEmpty(sLNS))
                sLNS = "-100";

            sql = sql.Replace("@@sLNS", sLNS.ToParamLikeStartWith("sLNS"));

            #endregion

            #region get data chitieu

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", chungTu.iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu.iNamLamViec);

                return cmd.GetTable();
            }
            #endregion   
        }
        public DataTable GetDonViThongTri(string iID_MaCapPhat, string loaiThongTri = null, string sLNS = null)
        {
            var chungTu = GetChungTu(new Guid(iID_MaCapPhat));

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTri_DonVi.sql");

            #endregion

            #region dieu kien chitieu  

            var dkDonVi = new NganSachService().GetDonviListByUser(chungTu.sID_MaNguoiDungTao, chungTu.iNamLamViec)
                        .ToDictionary(x => x.iID_MaDonVi, x => x.iID_MaDonVi + " - " + x.sTen).Select(x => x.Key).Join();
            #endregion

            #region get data chitieu

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu.iNamLamViec);
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", dkDonVi);

                return cmd.GetTable();
            }
            #endregion
        }
        public DataTable GetNgThongTri(string iID_MaCapPhat)
        {
            #region definition input

            var chungTu = GetChungTu(new Guid(iID_MaCapPhat));
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTriSec_Nganh.sql");
            var iID_MaPhongBan = chungTu.iID_MaPhongBan;

            #endregion

            #region dieu kien            

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu.iNamLamViec);
                if (iID_MaPhongBan == "07")
                {
                    cmd.Parameters.AddWithValue("@MaND", "%b7%");
                }
                else if (iID_MaPhongBan == "06")
                {
                    cmd.Parameters.AddWithValue("@MaND", "%b6%");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MaND", "%" + chungTu.sID_MaNguoiDungTao + "%");
                }

                return cmd.GetTable();
            }
            #endregion             
        }
        public DataTable GetDviNgThongTri(string iID_MaCapPhat, string NG)
        {
            #region definition input

            var chungTu = GetChungTu(new Guid(iID_MaCapPhat));
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTriSec_Nganh_DonVi.sql");

            #endregion

            #region dieu kien     

            if (String.IsNullOrEmpty(NG))
                NG = "-100";

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu.iNamLamViec);
                cmd.Parameters.AddWithValue("@Nganh", NG);
                return cmd.GetTable();

            }
            #endregion

        }
        public string GetGhiChuThongTri(string username, string sTen, string iID_MaDonVi)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_LayGhiChu.sql");

            #endregion

            #region dieu kien            

            #endregion

            #region get data

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
            #endregion
        }
        public string GetValueThongTriNT(string username, string sTen, string iID_MaDonVi, string loai)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_GetValueNT.sql");

            #endregion

            #region dieu kien            

            #endregion

            #region get data

            using (var conn = _connectionFactory.GetConnection())
            {
                var result = string.Empty;
                var entity = conn.QueryFirstOrDefault<CP_NT_Value>(
                    sql,
                    param: new
                    {
                        username,
                        iID_MaDonVi = iID_MaDonVi.ToParamString(),
                        sTen
                    },
                    commandType: CommandType.Text);
                if (entity != null) { 
                    switch (loai)
                    {
                        case "2":
                            result = entity.sTyGia;
                            break;
                        case "3":
                            result = entity.sThongBao;
                            break;
                        case "4":
                            result = entity.sDonViNhan;
                            break;
                        default:
                            result = entity.sGhiChu;
                            break;
                    }
                }
                else
                {
                    switch (loai)
                    {
                        case "2":
                            result = "...... đ/USD";
                            break;
                        case "3":
                            result = "..../TC2 ngày .../.../.....";
                            break;
                        case "4":
                            result = "....";
                            break;
                        default:
                            result = string.Empty;
                            break;
                    }
                }
                return result ?? string.Empty;
            }
            #endregion
        }
        public bool UpdateGhiChuThongTri(string username, string sTen, string iID_MaDonVi, string sGhiChu)
        {
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_CapNhapGhiChu.sql");
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
        public bool UpdateValueThongTriNT(string username, string sTen, string iID_MaDonVi, string value, string loai)
        {
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_UpdateValueNT.sql");
            var colName = loai == "1" ? "sGhiChu" : loai == "2" ? "sTyGia" : loai == "3" ? "sThongBao" : "sDonViNhan";
            sql = sql.Replace("@@value", colName);

            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        username,
                        iID_MaDonVi = iID_MaDonVi ?? "NULL",
                        sTen,
                        value
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }
        public DataTable CapPhatThongTriChiTiet(string iNamLamViec, string sLNS, string iID_MaDonVi, string iID_MaCapPhat)
        {
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTri_ChiTiet.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@sLNS", sLNS);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }
        public DataTable CapPhatThongTriTongHop(string iNamLamViec, string sLNS, string iID_MaDonVi, string iID_MaCapPhat)
        {
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTri_TongHop.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@sLNS", sLNS);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);

                    return cmd.GetTable();
                }
            }
        }
        public DataTable getDanhSachNganh(string iNamLamViec, string username)
        {
            {
                #region definition input

                var sql = FileHelpers.GetSqlQuery("dtCapPhat_HienVat_Nganh.sql");

                #endregion

                #region dieu kien

                var phongban = _ngansachService.GetPhongBan(username);
                var iID_MaPhongBan = phongban.sKyHieu;

                #endregion

                #region get data

                using (var conn = ConnectionFactory.Default.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                    cmd.Parameters.AddWithValue("@Nganh", DBNull.Value);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", DBNull.Value);
                    cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", DBNull.Value);
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", DBNull.Value);

                    var dt = cmd.GetTable();
                    return dt;
                }
                #endregion
            }
        }
        public DataTable CapPhatTongHopChiTieuSec(string iNamLamViec, string username, string sLNS)
        {
            String sql, DK;
            SqlCommand cmd = new SqlCommand(), cmd1 = new SqlCommand();
            DataTable vR;
            var phongban = _ngansachService.GetPhongBan(username);
            var iID_MaPhongBan = phongban.sKyHieu;

            String sTruongTien = "rTuChi,iID_MaDonVi,sTenDonVi,rTuChi_PhanBo,rTuChi_DaCap";
            String[] arrDSTruong = ("sNG,iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');

            //<--Lay toan bo Muc luc ngan sach
            DK = "iTrangThai=1";

            String DKLoai = "";
            if (!string.IsNullOrEmpty(sLNS))
            {
                DataTable dtNGChiTieu = new DataTable();
                #region definition input

                sql = FileHelpers.GetSqlQuery("dtCapPhat_HienVat_Nganh.sql");

                #endregion

                #region dieu kien

                string iID_MaDonVi = _ngansachService.GetDonviListByUser(username, iNamLamViec)
                            .Select(x => x.iID_MaDonVi)
                            .Join();

                #endregion

                #region get data

                using (var conn = ConnectionFactory.Default.GetConnection())
                using (cmd1 = new SqlCommand(sql, conn))
                {
                    cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd1.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd1.Parameters.AddWithValue("@Nganh", sLNS);
                    cmd1.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd1.Parameters.AddWithValue("@iID_MaNguonNganSach", DBNull.Value);
                    cmd1.Parameters.AddWithValue("@iID_MaNamNganSach", DBNull.Value);

                    dtNGChiTieu = cmd1.GetTable();
                }
                #endregion

                String DKNGChiTieu = "";

                if (dtNGChiTieu.Rows.Count > 0)
                {
                    for (int i = 0; i < dtNGChiTieu.Rows.Count; i++)
                    {
                        DKNGChiTieu += "sNG=@sNGCT" + i;
                        if (i < dtNGChiTieu.Rows.Count - 1)
                        {
                            DKNGChiTieu += " OR ";
                        }
                        cmd.Parameters.AddWithValue("@sNGCT" + i, dtNGChiTieu.Rows[i]["sNG"]);
                    }
                    DKLoai = " AND sLNS = '' AND (" + DKNGChiTieu + ")";
                }
            }
            else
                DKLoai += String.Format("and 1=1");

            sql = String.Format("SELECT sXauNoiMa,sNG,sMoTa " +
                "                FROM   NS_MucLucNganSach " +
                "                WHERE  iNamLamViec=dbo.f_ns_nammlns(@iNamLamViec) AND iTrangThai=1 {0} ORDER BY sXauNoiMa"
                                , DKLoai);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                vR = cmd.GetTable();
            }

            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->
            DataColumn column;

            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {

                if (arrDSTruongTien[j] == "rTuChi_PhanBo" || arrDSTruongTien[j] == "rTuChi_DaCap" || arrDSTruongTien[j] == "rTuChi")
                {
                    column = new DataColumn(arrDSTruongTien[j], typeof(decimal));
                    column.DefaultValue = 0;
                }
                else
                {
                    column = new DataColumn(arrDSTruongTien[j], typeof(String));
                }
                column.AllowDBNull = true;
                vR.Columns.Add(column);
            }

            var donvis = _ngansachService.GetDonviListByUser(username, iNamLamViec);

            int count1 = vR.Rows.Count;
            for (int j = 0; j < count1; j++)
            {
                DataRow dr = vR.Rows[j];
                if (dr["sNG"] != "")
                {
                    donvis.AsEnumerable()
                        .ToList()
                        .ForEach(r =>
                        {
                            DataRow temp = vR.NewRow();
                            for (int c = 0; c < vR.Columns.Count; c++)
                            {
                                temp[c] = dr[c];
                            }
                            temp["iID_MaDonVi"] = r.iID_MaDonVi;
                            temp["sTenDonVi"] = r.sTen;
                            vR.Rows.Add(temp);

                        });
                }
            }
            DataView dv = vR.DefaultView;
            dv.Sort = "iID_MaDonVi,sNG";
            vR = dv.ToTable();

            for (int i = vR.Rows.Count - 1; i >= 0; i--)
            {
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
                if (iID_MaDonVi == "")
                {
                    vR.Rows.RemoveAt(i);
                }
            }
            LayChiTieuDMNB(vR, username);
            LayCapPhatDaCapDMNB(vR, username);

            cmd.Dispose();
            return vR;
        }
        private void LayChiTieuDMNB(DataTable vR, string username)
        {

            String MaND = System.Web.HttpContext.Current.User.Identity.Name;
            var dtCauHinh = _ngansachService.GetCauHinh(username);
            var phongban = _ngansachService.GetPhongBan(username);
            var iID_MaPhongBan = phongban.sKyHieu;
            string iID_MaDonVis = _ngansachService.GetDonviListByUser(username, dtCauHinh.iNamLamViec)
                            .Select(x => x.iID_MaDonVi)
                            .Join();
            DataTable dtChiTieu = new DataTable();

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_TH_HienVat.sql");

            #endregion

            #region dieu kien chitieu  

            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", dtCauHinh.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", dtCauHinh.iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVis);

                dtChiTieu = cmd.GetTable();
            }
            #endregion
            int count = vR.Rows.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);

                DataRow[] dr, dr1;

                dr = dtChiTieu.Select("sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                dr1 = dtChiTieu.Select("sNG='" + sNG + "'");

                if (dr.Length > 0)
                {
                    //gan vao bang chung tu chi tiet VR
                    DataRow row1 = vR.Rows[i];

                    row1["rTuChi_PhanBo"] = Convert.ToDouble(dr[0]["rHienVat"]);
                }
                else if (dr1.Length == 0)
                {
                    if (Convert.ToString(vR.Rows[i]["sNG"]) != "")
                    {
                        vR.Rows.RemoveAt(i);
                    }
                }
                else
                {
                    if (Convert.ToString(vR.Rows[i]["sNG"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
                    {
                        vR.Rows.RemoveAt(i);
                    }
                }
            }
        }
        private void LayCapPhatDaCapDMNB(DataTable vR, string username)
        {
            String MaND = System.Web.HttpContext.Current.User.Identity.Name;
            var dtCauHinh = _ngansachService.GetCauHinh(username);
            var phongban = _ngansachService.GetPhongBan(username);
            var iID_MaPhongBan = phongban.sKyHieu;

            DataTable dtCapPhat = new DataTable();

            #region definition input dacap

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_DaCap.sql");

            #endregion

            #region dieu kien dacap 

            sql = sql.Replace("@@Tien_HienVat", "sTTM = '' AND 1");
            sql = sql.Replace("@@select", "sNG");
            sql = sql.Replace("@@MucCap", "sNG");

            #endregion

            #region get data dacap

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", dtCauHinh.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", dtCauHinh.iID_MaNguonNganSach);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", dtCauHinh.iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@dNgayCapPhat", DateTime.Now);
                cmd.Parameters.AddWithValue("@iSoCapPhat", DBNull.Value);

                dtCapPhat = cmd.GetTable();
            }
            #endregion

            for (int i = 0; i < vR.Rows.Count; i++)
            {
                String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
                DataRow[] dr;
                dr = dtCapPhat.Select("sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");

                if (dr.Length > 0)
                {
                    //gan vao bang chung tu chi tiet VR
                    DataRow row1 = vR.Rows[i];
                    row1["rTuChi_DaCap"] = dr[0]["rTuChi"];
                }
            }
        }
        public DataTable CapPhatThongTriSecChiTiet(string iID_MaCapPhat, string nganh, string iID_MaDonVi)
        {

            #region definition input

            var chungTu = GetChungTu(new Guid(iID_MaCapPhat));
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTriSec_ChiTiet.sql");

            #endregion

            #region dieu kien

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu.iNamLamViec);
                cmd.Parameters.AddWithValue("@Nganh", nganh);

                return cmd.GetTable();
            }
            #endregion
        }
        public DataTable CapPhatThongTriSecTongHop(string iID_MaCapPhat, string nganh, string iID_MaDonVi)
        {

            #region definition input

            var chungTu = GetChungTu(new Guid(iID_MaCapPhat));
            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTriSec_TongHop.sql");

            #endregion

            #region dieu kien

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu.iNamLamViec);
                cmd.Parameters.AddWithValue("@Nganh", nganh);

                return cmd.GetTable();
            }
            #endregion
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
                dtr["TenThongTri"] = "Kinh phí đặc biệt";
                ds.Rows.Add(dtr);
            }
            else if (loai == 3)
            {
                DataRow dtr = ds.NewRow();
                dtr["loaiThongTri"] = "4";
                dtr["TenThongTri"] = "Kinh phí khác";
                ds.Rows.Add(dtr);
            }
            else
            {
                var dsLNS = sDSLNS.Split(',');
                for (int i = 0; i < dsLNS.Count(); i++)
                {
                    if (dsLNS[i].StartsWith("2") || dsLNS[i].StartsWith("3")|| dsLNS[i].StartsWith("4"))
                    {
                        dsLNS[i] = dsLNS[i].Substring(0, 1);
                    } else if (dsLNS[i].StartsWith("102"))
                    {
                        dsLNS[i] = "101";
                    }
                    else 
                    {
                        dsLNS[i] = dsLNS[i].Substring(0, 3);
                    }
                }
                var dsTT = dsLNS.Distinct();
                foreach (string tt in dsTT)
                {
                    if (tt.StartsWith("101") || tt.StartsWith("102"))
                    {                        
                        DataRow dtr = ds.NewRow();
                        dtr["loaiThongTri"] = "101";
                        dtr["TenThongTri"] = "Kinh phí Quốc phòng";
                        ds.Rows.Add(dtr);
                    }
                    else if (tt.StartsWith("104"))
                    {
                        DataRow dtr = ds.NewRow();
                        dtr["loaiThongTri"] = "104";
                        dtr["TenThongTri"] = "Kinh phí Bảo đảm";
                        ds.Rows.Add(dtr);
                    }
                    else
                    {
                        Dictionary<string, string> dsTen = GetLoaiThongTri();
                        DataRow dtr = ds.NewRow();
                        dtr["loaiThongTri"] = tt;
                        dtr["TenThongTri"] = dsTen[tt];
                        ds.Rows.Add(dtr);
                    }
                }
            }
            return ds;
        }

        #endregion

    }
}
