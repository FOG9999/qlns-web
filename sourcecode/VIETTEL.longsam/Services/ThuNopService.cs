using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using VIETTEL;
using FlexCel.Report;

namespace Viettel.Services
{
    public interface IThuNopService
    {
        #region sMoTaQD

        string getSMoTaQD(string sTenBaoCao, string maND);

        void updateSMoTaQD(string sTenBaoCao, string maND, string sMoTa);

        #endregion

        #region Time
        DataTable getTime(string sLoaiThoiGian);
        String DKThoiGian(String sLoaiThoiGian, String iThoiGian);
        #endregion

        #region DS
        DataTable getDSLoaiBaoCao();
        DataTable getDSTO();
        DataTable getDSLoaiThongTri();
        string getTNTieuDe(string sLoaiThoiGian, string iThoiGian, string namLamViec);
        #endregion

        #region checkDuLieuNhap
        DataTable Get_DanhSachDuLieu(String username);
        DataTable getDSPhongBanDaNhap(string iNamLamViec, string Username);
        DataTable GetDonViListByUser_PhongBan_TN(string iID_MaPhongBan, string username, string namLamViec);
        Dictionary<string, string> UpdateParent(string NamLamViec, string code, string Id = "");
        FlexCelReport FillDataTable(FlexCelReport fr, DataTable data, string fields, string iNamLamViec, string field = null);
        #endregion

    }

    public class ThuNopService : IThuNopService
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly ILocalizationService _languageService;

        public ThuNopService(IConnectionFactory connectionFactory = null, ILocalizationService languageService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
        }

        private static ThuNopService _default;
        public static ThuNopService Default
        {
            get { return _default ?? (_default = new ThuNopService()); }
        }

        #region sMoTaQD

        public string getSMoTaQD(string sTenBaoCao, string maND)
        {

            var sql = @"SELECT  sTen 
                        FROM    KT_DanhMucThamSo_BaoCao 
                        WHERE   sTenBaoCao=@sTenBaoCao 
                                AND sID_MaNguoiDungTao=@sID_MaNguoiDungTao";

            using (var conn = _connectionFactory.GetConnection())
            {
                var value = conn.QueryFirstOrDefault<string>(
                    sql,
                    param: new
                    {
                        @sTenBaoCao = sTenBaoCao,
                        @sID_MaNguoiDungTao = maND
                    },
                    commandType: CommandType.Text);

                if (string.IsNullOrEmpty(value))
                {
                    value = "Kèm theo công văn số ........../CTC-KHNS ngày ..... tháng ..... năm ..... của Cục Tài chính";
                }
                return value;
            }
        }

        public void updateSMoTaQD(string sTenBaoCao, string maND, string sMoTa)
        {

            var sql1 = @"DELETE     KT_DanhMucThamSo_BaoCao 
                        WHERE       iID_MaBaoCao=@iID_MaBaoCao 
                                    AND sID_MaNguoiDungTao=@sID_MaNguoiDungTao";
            var sql2 = @"INSERT INTO    KT_DanhMucThamSo_BaoCao (iID_MaBaoCao,sTenBaoCao,sID_MaNguoiDungTao,sTen) 
                        values          (@iID_MaBaoCao,@sTenBaoCao,@sID_MaNguoiDungTao ,@sTen)";

            using (var conn = _connectionFactory.GetConnection())
            {
                var value1 = conn.Execute(
                    sql1,
                    param: new
                    {
                        @iID_MaBaoCao = maND,
                        @sID_MaNguoiDungTao = maND
                    },
                    commandType: CommandType.Text);

                var value2 = conn.Execute(
                    sql2,
                    param: new
                    {
                        @iID_MaBaoCao = maND,
                        @sTenBaoCao = sTenBaoCao,
                        @sID_MaNguoiDungTao = maND,
                        @sTen = sMoTa
                    },
                    commandType: CommandType.Text);
            }
        }

        #endregion

        #region Time
        public DataTable getTime(string sLoaiThoiGian)
        {

            switch (sLoaiThoiGian)
            {
                case ("0"):
                    return DateTimeExtension.getDSTGTongHop();
                case ("1"):
                    return DateTimeExtension.getDSQuy();
                case ("2"):
                    return DateTimeExtension.getDSThang();
                default:
                    return null;
            }
        }

        public string DKThoiGian(String sLoaiThoiGian, String iThoiGian)
        {
            string dkThoiGian = "";
            switch (sLoaiThoiGian + iThoiGian)
            {
                case ("00"):
                    dkThoiGian = null;
                    break;
                case ("01"):
                    dkThoiGian = "(1,2,3,4,5,6,7,8,9,10,11,12)";
                    break;
                case ("02"):
                    dkThoiGian = "(13)";
                    break;
                case ("11"):
                    dkThoiGian = "(1,2,3)";
                    break;
                case ("12"):
                    dkThoiGian = "(4,5,6)";
                    break;
                case ("13"):
                    dkThoiGian = "(7,8,9)";
                    break;
                case ("14"):
                    dkThoiGian = "(10,11,12)";
                    break;
                default:
                    dkThoiGian = "(" + iThoiGian + ")";
                    break;
            }
            return dkThoiGian;
        }
        #endregion

        #region DS

        public DataTable getDSLoaiBaoCao()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(string));
            dt.Columns.Add("sTen", typeof(string));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Loại hình--Đơn vị";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Đơn vị--Loại hình";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }

        public DataTable getDSTO()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(string));
            dt.Columns.Add("sTen", typeof(string));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Tờ 1";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Tờ 2";
            dt.Rows.InsertAt(dr, 1);
            return dt;
        }

        public DataTable getDSLoaiThongTri()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(string));
            dt.Columns.Add("sTen", typeof(string));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Nộp NSQP";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Thuế TNDN qua BQP";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "3";
            dr["sTen"] = "Nộp NSNN Khác";
            dt.Rows.InsertAt(dr, 2);
            return dt;
        }

        public string getTNTieuDe(string sLoaiThoiGian, string iThoiGian, string namLamViec)
        {
            string ThoiGian = "";
            switch (sLoaiThoiGian)
            {
                case ("0"):
                    ThoiGian = "NĂM " + namLamViec;
                    break;
                case ("1"):
                    ThoiGian = "QUÝ " + NumberExtension.NToR(Convert.ToInt16(iThoiGian)) + " NĂM " + namLamViec;
                    break;
                default:
                    ThoiGian = "THÁNG " + iThoiGian + " NĂM " + namLamViec;
                    break;
            }
            return ThoiGian;
        }

        public DataTable getDSPhongBanDaNhap(string iNamLamViec, string Username)
        {
            string sql = @"select distinct iID_MaPhongBan
                                           , sTenPhongBan
                           from            TN_ChungTuChiTiet
                           where           iTrangThai = 1 
                                           and iNamLamViec = @iNamLamViec
                                           and (@user is null or sID_MaNguoiDungTao = @user)
                                           and (@id_phongban is null or iID_MaPhongBan = @id_phongban)
                                           and iID_MaPhongBan NOT IN (02)";

            var id_phongban = _ngansachService.GetPhongBan(Username).sKyHieu;
            var user = Username;

            if (id_phongban == "02" || id_phongban == "11")
            {
                id_phongban = "";
                user = "";
            }

            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop)
            {
                user = "";
            }
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    iNamLamViec,
                    user = user.ToParamString(),
                    id_phongban = id_phongban.ToParamString()
                });

                var dt = cmd.GetTable();

                return dt;
            }
        }

        public DataTable Get_DanhSachDuLieu(string username)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDonViListByUser_PhongBan_TN(string iID_MaPhongBan, string username, string iNamLamViec)
        {
            #region sql
            var sql = @"select distinct LTRIM(RTRIM(iID_MaDonVi)) as iID_MaDonVi
                                        , sTenDonVi as sTen 
                        from            TN_ChungTuChiTiet 
                        where           iTrangThai=1 
                                        and (@id_donvi is null or iID_MaDonVi in (select * from F_Split(@id_donvi))) 
				                        and (@id_phongban is null or iID_MaPhongBan = @id_phongban)  
                                        and (@user is null or sID_MaNguoiDungTao = @user)
                                        and iNamLamViec = @iNamLamViec                                         
                        order by        iID_MaDonVi";
            #endregion

            #region dieu kien
            var iID_MaDonVi = _ngansachService.GetDonviListByUser(username, int.Parse(iNamLamViec))
                                .Select(x => x.iID_MaDonVi)
                                .Join();
            var user = username;
            if (_ngansachService.IsUserRoleType(username, UserRoleType.TroLyPhongBan) && _ngansachService.GetPhongBan(username).sKyHieu == "02" ||
                _ngansachService.IsUserRoleType(username, UserRoleType.TroLyTongHop) ||
                _ngansachService.IsUserRoleType(username, UserRoleType.TroLyTongHopCuc) ||
                _ngansachService.IsUserRoleType(username, UserRoleType.TruongPhong) ||
                _ngansachService.IsUserRoleType(username, UserRoleType.ThuTruong))
            {
                user = "";
            }
            #endregion

            #region data
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    iNamLamViec,
                    user = user.ToParamString(),
                    id_phongban = iID_MaPhongBan.ToParamString(),
                    id_donvi = iID_MaDonVi.Trim().ToParamString()
                });

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }

        public Dictionary<string, string> UpdateParent(string NamLamViec, string code, string Id = "")
        {
            if (code.Length == 1)
            {
                return new Dictionary<string, string>()
                {
                    { "iID_MaLoaiHinh_Cha", "0" },
                    { "bLaHangCha", "1" },
                    { "bLaTong", "1" },
                };
            }
            else
            {
                var bLaHangCha = "false";
                var iID_MaLoaiHinh_Cha = "";
                var bLaTong = "false";
                if (code.Length > 1 && code.Length < 7)
                {
                    bLaHangCha = "true";
                    bLaTong = "true";
                }

                var len = code.Length - 2;
                while ((iID_MaLoaiHinh_Cha == "" || iID_MaLoaiHinh_Cha == Id || iID_MaLoaiHinh_Cha == "0") && len > 0)
                {
                    var code_parent = code.Substring(1, len);

                    var sql =
                            @"

                        select      iID_MaLoaiHinh                                         
                        from        TN_DanhMucLoaiHinh 
                        where       sKyHieu = @code_parent         
                        order by    sKyHieu

                        ";
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {

                        iID_MaLoaiHinh_Cha = conn.QueryFirstOrDefault<Guid>(
                            sql,
                            new
                            {
                                code_parent,
                            }).ToString();
                    }
                    len = len - 2;
                }

                var result = new Dictionary<string, string>()
                {
                    { "iID_MaLoaiHinh_Cha", iID_MaLoaiHinh_Cha },
                    { "bLaHangCha", bLaHangCha },
                    { "bLaTong", bLaTong },
                };
                return result;
            }
        }
        public FlexCelReport FillDataTable(FlexCelReport fr, DataTable data, string fields, string iNamLamViec, string field = null)
        {
            List<string> lnsList = (fields).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("sMoTa") || !data.GetColumnNames().Contains("STT"))
            {
                AddMoTa(data, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", fields.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count(); i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : string.Format("{0},{1}", lns.Join(), field);
                var dt = dtSource.SelectDistinct(tableName, distinctField);//.AddLnsMoTa(iNamLamViec);
                AddMoTa(dt, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

                // nho relationshipo
                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            return fr;
        }

        protected DataTable AddMoTa(DataTable dt, string iNamLamViec = null)
        {
            var count = dt.Columns.Count;
            var columnNames = dt.GetColumnNames();

            var checkColumns = "STT,sTen";
            checkColumns.Split(',').ToList()
                .ForEach(c =>
                {
                    if (!columnNames.Contains(c))
                    {
                        dt.Columns.Add(c);
                    }
                });


            var lns = "sKyHieu".Split(',').ToList();

            dt.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var items = new List<string>();
                    lns.ForEach(l =>
                    {
                        if (columnNames.Contains(l))
                        {
                            var value = r[l].ToString();
                            items.Add(value);
                        }
                    });

                    var key = items.Count == 0 ?
                    r.ItemArray[count - 1].ToString() :
                    items.Join("-");

                    if (string.IsNullOrWhiteSpace(iNamLamViec))
                        iNamLamViec = DateTime.Now.Year.ToString();

                    var entity = getMoTa(iNamLamViec, key);
                    if (entity != null)
                    {
                        if (r.Field<string>("sTen").IsEmpty())
                            r["sTen"] = entity.sTen;

                        if (r.Field<string>("sTT").IsEmpty())
                            r["sTT"] = entity.sTT;
                    }
                });
            return dt;
        }

        protected TN_DanhMucLoaiHinh getMoTa(string iNamLamViec, string lns)
        {
            var sql =
    @"

        SELECT      TOP(1) *
        FROM        TN_DanhMucLoaiHinh
        WHERE       iTrangThai = 1
                    and iNamLamViec=@iNamLamViec 
                    AND sKyHieu= @lns
        ORDER BY    sKyHieu

        ";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {

                var result = conn.QueryFirstOrDefault<TN_DanhMucLoaiHinh>(
                    sql,
                    new
                    {
                        iNamLamViec,
                        lns
                    });
                return result;
            }

        }
        #endregion
    }
}
