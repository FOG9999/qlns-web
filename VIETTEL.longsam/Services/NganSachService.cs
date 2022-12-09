using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;
using VIETTEL.Helpers;

namespace Viettel.Services
{
    public interface INganSachService
    {
        DC_NguoiDungCauHinh GetCauHinh(string username);
        NS_PhongBan GetPhongBan(string username);
        NS_PhongBan GetPhongBanById(string id);

        string GetTruongPhongBql(string bql);

        #region donvis

        IEnumerable<NS_DonVi> GetDonviListByUser(string username, int namLamViec = 0, bool hasChuongTrinh = false, bool byUsername = true);
        IEnumerable<NS_DonVi> GetDonviListByUser(string username, string namLamViec);
        IEnumerable<NS_DonVi> GetDonviByPhongBanId(string namLamViec, string id_phongban);

        string GetDonvisByUser(string username, int namLamViec);
        string GetDoanhNghiep(string namLamViec);

        NS_DonVi GetDonVi(string namLamViec, string id);
        NS_DonVi GetDonViByMaDonVi(string namLamViec, string id);
        NS_DonVi GetDonViById(string namLamViec, string iID_Ma);
        NS_DonVi GetDonVi_QLNganh(string namLamViec, string id_nganh);

        VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAnByID(string iID_DonVi);

        VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAnByMa(string iID_MaDonVi);

        #endregion

        #region phongban

        IEnumerable<NS_PhongBan> GetBql(string username);
        NS_PhongBan GetPhongBanById(Guid id);

        IEnumerable<NS_PhongBan> GetPhongBans(string kyHieu = null);

        DataTable GetPhongBanQuanLyNS(string id_phongban = null);
        DataTable GetPhongBansQuanLyNS(string id_phongban = null);

        DataTable GetDonviByPhongBan(string namLamViec, string id_phongban);

        string CheckParam_PhongBan(string id_phongban);
        string CheckParam_Username(string username, string id_phongban = null);

        #endregion

        #region users

        int GetUserRoleType(string username);

        bool IsUserRoleType(string username, UserRoleType roleType);

        #endregion

        #region lns

        DataTable GetLNS(string iID_MaPhongBan, string iNamLamViec);

        NS_MucLucNganSach GetMLNS(string iNamLamViec, Guid id);
        string GetMLNS_MoTa(string sLNS);
        //string GetMLNS_MoTa(string iNamLamViec, string sLNS);
        string GetMLNS_MoTa_ByXauNoiMa(string iNamLamViec, string sLNS);
        string GetMLNS_MoTa(string iNamLamViec, string sLNS, int left = 7);
        //string GetMLNS_MoTa(string iNamLamViec, string sLNS, string sL, string sK, string sM = null, s);

        IEnumerable<NS_MucLucNganSach> GetMLNS_All(string iNamLamViec);
        DataTable GetMLNS_DataTable(string iNamLamViec);

        #endregion

        #region mlns

        string MLNS_MaNganh(string nam, string maDonVi);
        string MLNS_MaNganh_NhaNuoc(string maDonVi);
        DataTable AddMoTaM(DataTable dt, string iNamLamViec = null);
        DataTable MLNS_GetAll(string iNamLamViec, Dictionary<string, string> filters = null);

        /// <summary>
        /// Dung de su dung trong bao cao, mac dich cache 5m
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        IEnumerable<NS_MucLucNganSach> MLNS_GetAll(string iNamLamViec);

        bool MLNS_Delete(Guid id, IDbConnection conn = null);
        bool MLNS_Update(NS_MucLucNganSach entity, IDbConnection conn = null);

        DataTable MucLucDTKT_GetAll(string iNamLamViec, Dictionary<string, string> filters = null);
        //bool MucLucDTKT_Delete(Guid id, IDbConnection conn = null);
        //bool MucLucDTKT_Update(DTKT_MucLuc entity, IDbConnection conn = null);

        #endregion

        #region namns

        DataTable GetNamNganSachs();

        #endregion

        #region nganh

        string GetNganhNhaNuoc(string maNganh);

        DataTable Nganh_GetAll(string nam, string username, string id_phongban = null);
        DataTable ChuyenNganh_GetAll(string nam, string user_name, string id_phongban = null);
        DataTable ChuyenNganh_GetAll(string nam);
        string ChuyenNganh_Get(string nam, string nganh);
        DataTable Nganh_GetAll_ByPhongBan(string nam, string id_phongban);
        string Nganh_GetAll_ChuyenNganh(string username, string id_phongban = null);
        string Nganh_GetAll_ChuyenNganh(string username, int nam);
        NS_MucLucNganSach_Nganh Nganh_Get(string nam, string ng, string username = null);


        #endregion

        #region user

        string User_FullName(string username);
        bool User_FullName_Update(string username, string fullname);

        #endregion

    }
    public class NganSachService : INganSachService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;

        public NganSachService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
        }

        private static INganSachService _default;
        public static INganSachService Default
        {
            get { return _default ?? (_default = new NganSachService()); }
        }

        public DC_NguoiDungCauHinh GetCauHinh(string username)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<DC_NguoiDungCauHinh>(new CommandDefinition(
                     commandText: "Select iNamLamViec, iThangLamViec, iID_MaNamNganSach, iID_MaNguonNganSach from DC_NguoiDungCauHinh  WHERE sID_MaNguoiDungTao=@sID_MaNguoiDungTao",
                     parameters: new
                     {
                         sID_MaNguoiDungTao = username,
                     },
                     commandType: CommandType.Text
                 ));


                return entity;
            }
        }

        #region phongban

        public IEnumerable<NS_PhongBan> GetBql(string username)
        {
            var list = new List<NS_PhongBan>();
            var roleType = GetUserRoleType(username);
            if (roleType == (int)UserRoleType.TroLyTongHopCuc ||
                roleType == (int)UserRoleType.QuanTri ||
                roleType == (int)UserRoleType.ThuTruong)
            {
                list.AddRange(new[]
                {
                    GetPhongBanById("06"),
                    GetPhongBanById("07"),
                    GetPhongBanById("08"),
                    GetPhongBanById("10"),
                });
            }
            else
            {
                list.Add(GetPhongBan(username));
            }

            return list;
        }

        public IEnumerable<NS_PhongBan> GetPhongBans(string kyHieu = null)
        {
            var list = new List<NS_PhongBan>();
            if (string.IsNullOrWhiteSpace(kyHieu) || kyHieu == "07" || kyHieu == "10" || kyHieu == "02")
            {
                list.AddRange(new List<NS_PhongBan>()
                {
                    GetPhongBanById("06"),
                    GetPhongBanById("07"),
                    GetPhongBanById("08"),
                    GetPhongBanById("10"),
                });
            }
            else if (kyHieu == "06" || kyHieu == "08")
            {
                list.Add(GetPhongBanById(kyHieu));
            }

            return list;
        }

        public DataTable GetPhongBanQuanLyNS(string id_phongban = null)
        {
            if (string.IsNullOrWhiteSpace(id_phongban))
            {
                id_phongban = "05,06,07,08,10,16,17";
            }

            var sql = @"

select  sKyHieu, sKyHieu as iID_MaPhongBan, sMoTa = sKyHieu +' - ' + sMoTa 
from    ns_phongban 
where   iTrangThai=1 and sKyHieu in (select * from f_split(@id_phongban))

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new { id_phongban });
                return dt;
            }
        }
        public DataTable GetPhongBansQuanLyNS(string id_phongban = null)
        {
            if (string.IsNullOrWhiteSpace(id_phongban))
            {
                id_phongban = "06,07,08,10,17";
            }

            var sql = @"

select  sKyHieu, sKyHieu as iID_MaPhongBan, sMoTa = sKyHieu +' - ' + sMoTa 
from    ns_phongban 
where   iTrangThai=1 and sKyHieu in (select * from f_split(@id_phongban))

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new { id_phongban });
                return dt;
            }
        }

        public NS_PhongBan GetPhongBan(string username)
        {
            var sql =
                @"  SELECT 
                        NS_PhongBan.iID_MaPhongBan,
                        NS_PhongBan.sKyHieu,
                        NS_PhongBan.sTen, 
                        NS_PhongBan.sMoTa 
                    FROM NS_NguoiDung_PhongBan 
                    INNER JOIN NS_PhongBan ON NS_PhongBan.iID_MaPhongBan=NS_NguoiDung_PhongBan.iID_MaPhongBan
                    WHERE NS_NguoiDung_PhongBan.iTrangThai=1 AND NS_NguoiDung_PhongBan.sMaNguoiDung=@sMaNguoiDung";

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_PhongBan>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         sMaNguoiDung = username,
                     },
                     commandType: CommandType.Text
                 ));


                if (entity == null)
                    entity = new NS_PhongBan();

                return entity;
            }


            //var entity = NganSach_HamChungModels.DSBQLCuaNguoiDung(username)
            //      .AsEnumerable()
            //      .Select(x => new NS_PhongBan
            //      {
            //          sKyHieu = x.Field<string>("sKyHieu"),
            //          sTen = x.Field<string>("sTen")
            //      })
            //      .First();
        }

        public NS_PhongBan GetPhongBanById(string id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_PhongBan>(new CommandDefinition(
                     commandText: "SELECT iID_MaPhongBan, sKyHieu, sTen, sMota FROM NS_PhongBan  WHERE sKyHieu=@sKyHieu AND iTrangThai=1",
                     parameters: new
                     {
                         sKyHieu = id,
                     },
                     commandType: CommandType.Text
                 ));

                if (entity == null)
                    entity = new NS_PhongBan();
                return entity;
            }
        }

        public string GetTruongPhongBql(string bql)
        {
            var result = string.Empty;

            var maPhongBan = 1;
            var valid = int.TryParse(bql, out maPhongBan);

            result = valid ?
                _languageService.Translate(string.Format("ChuKy_B{0}_TruongPhong", maPhongBan)) :
                string.Format("[Trưởng phòng {0}]", bql);
            return result;
        }
        #endregion

        #region donvis

        public IEnumerable<NS_DonVi> GetDonviListByUser(string username, int namLamViec = 0, bool hasChuongTrinh = false, bool byUsername = true)
        {
            if (namLamViec == 0)
            {
                var config = GetCauHinh(username);
                namLamViec = config.iNamLamViec;
            }

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT b.iID_MaDonVi, b.sTen, (b.iID_MaDonVi + ' - ' + b.sTen) as sMoTa, b.iID_Ma ");
            if (byUsername)
            {
                sql.AppendLine("FROM NS_NguoiDung_DonVi a, NS_DonVi b ");
            }
            else
            {
                sql.AppendLine("FROM NS_DonVi b ");
            }
            if (hasChuongTrinh)
            {
                sql.AppendLine("INNER JOIN NH_KHChiTietBQP_NhiemVuChi c ON b.iID_Ma = c.iID_DonViID AND b.iID_MaDonVi = c.iID_MaDonVi ");
            }
            sql.AppendLine("WHERE 1=1 AND ");

            if (byUsername)
            {
                sql.AppendLine("a.iID_MaDonVi = b.iID_MaDonVi AND ");
                sql.AppendLine("a.iNamLamViec = @namLamViec AND ");
                sql.AppendLine("a.sMaNguoiDung = @username AND ");
                sql.AppendLine("a.iTrangThai = 1 AND ");
            }
            sql.AppendLine("b.iNamLamViec_DonVi = @namLamViec AND b.iTrangThai = 1 ");
            sql.AppendLine("ORDER BY iID_MaDonVi ");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(new CommandDefinition(
                     commandText: sql.ToString(),
                     parameters: new {
                         username,
                         namLamViec,
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }
        public IEnumerable<NS_DonVi> GetDonviListByUser(string username, string namLamViec)
        {
            return GetDonviListByUser(username, int.Parse(namLamViec));
        }

        public string GetDonvisByUser(string username, int namLamViec)
        {
            var items = GetDonviListByUser(username, namLamViec);
            var result = string.Join(",", items.Select(x => x.iID_MaDonVi));
            return result;
        }
        public string GetDoanhNghiep(string namLamViec)
        {
            var items = GetDonviByPhongBan(namLamViec, "06");
            //var result = string.Join(",", items.AsEnumerable().Select(x => string..iID_MaDonVi));
            return "";
        }
        public NS_DonVi GetDonVi(string namLamViec, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new NS_DonVi();

            var sql = @"

select  iID_MaDonVi, sTen, sTenTomTat, sMoTa=iID_MaDonVi +' - '+sTen
from    NS_DonVi
where   iTrangThai=1
        and iID_MaDonVi=@iID_MaDonVi
        and iNamLamViec_DonVi=@iNamLamViec

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_DonVi>(sql,
                    new
                    {
                        iNamLamViec = namLamViec,
                        iID_MaDonVi = id,
                    }, null, null, CommandType.Text);

                if (string.IsNullOrWhiteSpace(entity.sMoTa))
                    entity.sMoTa = entity.sTen;

                return entity;
            }
        }
        public NS_DonVi GetDonVi_QLNganh(string namLamViec, string id)
        {
            var id_donvi = "";
            if ("01".Contains(id)) {
                id_donvi = "29";
            }
            else if ("12".Contains(id))
            {
                id_donvi = "31";
            }
            else if ("02".Contains(id))
            {
                id_donvi = "33";
            }
            else if ("69".Contains(id))
            {
                id_donvi = "40";
            }
            else if ("04".Contains(id))
            {
                id_donvi = "41";
            }
            else if ("08".Contains(id))
            {
                id_donvi = "42";
            }
            else if ("06".Contains(id))
            {
                id_donvi = "43";
            }
            else if ("05".Contains(id))
            {
                id_donvi = "44";
            }
            else if ("07".Contains(id))
            {
                id_donvi = "45";
            }
            else if ("09".Contains(id))
            {
                id_donvi = "46";
            }
            else if ("10".Contains(id))
            {
                id_donvi = "47";
            }
            else if ("20,21,22,23,24,25,26,27,28,29,41,42,44,67,70,71,72,73,74,75,76,77,78,81,82".Contains(id))
            {
                id_donvi = "51";
            }
            else if ("30,31,32,33,35,36,37,38,39,40,43,45,46,47".Contains(id))
            {
                id_donvi = "52";
            }
            else if ("50,51,53,54,55,56,57".Contains(id))
            {
                id_donvi = "53";
            }
            else if ("34".Contains(id))
            {
                id_donvi = "55";
            }
            else if ("11".Contains(id))
            {
                id_donvi = "56";
            }
            else if ("80".Contains(id))
            {
                id_donvi = "61";
            }
            else if ("60,61,62,64,65,66".Contains(id))
            {
                id_donvi = "99";
            }
            var donvi = GetDonVi(namLamViec, id_donvi);
            return donvi;
        }


        #endregion

        #region User

        public int GetUserRoleType(string username)
        {
            var sql = "SELECT iDoiTuongNguoiDung FROM QT_NguoiDung WHERE sID_MaNguoiDung = @sID_MaNguoiDung";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<QT_NguoiDung>(
                    new CommandDefinition(sql, new
                    {
                        sID_MaNguoiDung = username,
                    }));

                return entity == null ? -1 : entity.iDoiTuongNguoiDung.GetValueOrDefault();
            }
        }

        public bool IsUserRoleType(string username, UserRoleType roleType)
        {
            var role = GetUserRoleType(username);
            return role == (int)roleType;
        }

        #endregion

        #region mlns

        public DataTable GetLNS(string iID_MaPhongBan, string iNamLamViec)
        {
            //var phongban = GetPhongBanById(iID_MaPhongBan);
            //iID_MaPhongBan = phongban.iID_MaPhongBan.ToString();

            //            var sql =
            //@"

            //SELECT  A.sLNS, A.sMoTa, A.sLNS +' - '+ A.sMoTa AS TenHT 
            //FROM    NS_MucLucNganSach as A 
            //INNER JOIN NS_PhongBan_LoaiNganSach AS B ON A.sLNS = B.sLNS 
            //WHERE   B.iID_MaPhongBan=@iID_MaPhongBan AND 
            //        B.iTrangThai=1 
            //        AND LEN(A.sLNS)=7  
            //        AND A.sL = '' 
            //        AND A.iNamLamViec=@iNamLamViec 
            //ORDER By A.sLNS

            //";

            var sql = @"

select * from F_NS_LNS_PhongBan(@iNamLamViec,@iID_MaPhongBan)
where LEFT(sLNS,1) in (1,2,3,4)

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iID_MaPhongBan,
                         iNamLamViec
                     }
                 );
                return dt;
            }
        }

        public string MLNS_MaNganh(string nam, string maDonVi)
        {
            var sql =
@"SELECT    iID_MaNganhMLNS 
    FROM    NS_MucLucNganSach_Nganh
    WHERE   iTrangThai = 1 AND 
            iID_MaNganh = @iID_MaNganh AND iNamLamViec=@nam";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<NS_MucLucNganSach_Nganh>(
                     new CommandDefinition(
                         sql,
                         new
                         {
                             iID_MaNganh = maDonVi,
                             nam,
                         }, null, null, CommandType.Text))
                     .FirstOrDefault();
                return entity == null ? string.Empty : entity.iID_MaNganhMLNS;
            }
        }

        public string MLNS_MaNganh_NhaNuoc(string maDonVi)
        {
            var sql =
@"SELECT    iID_MaNganhMLNS 
    FROM    NS_MucLucNganSach_Nganh_NhaNuoc
    WHERE   iTrangThai = 1 AND 
            iID_MaNganh = @iID_MaNganh";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<NS_MucLucNganSach_Nganh_NhaNuoc>(
                     new CommandDefinition(
                         sql,
                         new
                         {
                             iID_MaNganh = maDonVi,
                         }, null, null, CommandType.Text))
                     .FirstOrDefault();
                return entity == null ? string.Empty : entity.iID_MaNganhMLNS;
            }
        }

        public NS_MucLucNganSach GetMLNS(string sLNS)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var nam = DateTime.Now.Year;
                if (HttpContext.Current.User != null)
                {
                    var config = GetCauHinh(HttpContext.Current.User.Identity.Name);
                    nam = config.iNamLamViec;
                }

                var sql = @"SELECT * FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sLNS=@sLNS AND iNamLamViec=@iNamLamViec";
                var entity = conn.QueryFirstOrDefault<NS_MucLucNganSach>(
                    new CommandDefinition(
                        commandText: sql,
                        parameters: new
                        {
                            sLNS,
                            iNamLamViec = nam,
                        }));

                return entity;
            }
        }

        public string GetMLNS_MoTa(string sLNS)
        {
            //var entity = GetMLNS(sLNS);
            //return entity.sMoTa;

            using (var conn = _connectionFactory.GetConnection())
            {
                var nam = DateTime.Now.Year;
                if (HttpContext.Current.User != null)
                {
                    var config = GetCauHinh(HttpContext.Current.User.Identity.Name);
                    nam = config.iNamLamViec;
                }

                return GetMLNS_MoTa_ByXauNoiMa(nam.ToString(), sLNS);
            }
        }

        public string GetMLNS_MoTa_ByXauNoiMa(string iNamLamViec, string sLNS)
        {
            //var entity = GetMLNS(sLNS);
            //return entity.sMoTa;

            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"
SELECT  sMoTa 
FROM    NS_MucLucNganSach 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND sXauNoiMa=@sLNS

";
                var entity = conn.QueryFirstOrDefault<dynamic>(
                    new CommandDefinition(
                        commandText: sql,
                        parameters: new
                        {
                            sLNS,
                            iNamLamViec,
                        }));

                return entity == null ? string.Empty : entity.sMoTa;
            }
        }

        public string GetMLNS_MoTa(string iNamLamViec, string sLNS, int left = 7)
        {
            //var entity = GetMLNS(sLNS);
            //return entity.sMoTa;

            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"
SELECT  top(1) sMoTa 
FROM    NS_MucLucNganSach 
WHERE   iTrangThai=1  
        and iNamLamViec=@iNamLamViec
        and left(sLNS,@left)=@sLNS and sL = '' and sK = ''
ORDER BY sXauNoiMa";
                var entity = conn.QueryFirstOrDefault<dynamic>(
                    new CommandDefinition(
                        commandText: sql,
                        parameters: new
                        {
                            sLNS,
                            iNamLamViec,
                            left
                        }));

                return entity == null ? string.Empty : entity.sMoTa;
            }
        }

        #endregion

        #region nam ns


        public DataTable GetNamNganSachs()
        {
            var sql = "SELECT iID_MaNamNganSach,sTen FROM NS_NamNganSach WHERE iTrangThai=1";
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                return cmd.GetTable();
            }
        }


        #endregion

        #region nganh

        public string GetNganhNhaNuoc(string maNganh)
        {
            var sql = @"


select sTenNganh from NS_MucLucNganSach_Nganh_NhaNuoc
where iTrangThai=1 and iID_MaNganh=@iID_MaNganh

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var value = conn.QueryFirstOrDefault<string>(
                    sql,
                    param: new
                    {
                        @iID_MaNganh = maNganh,
                    },
                    commandType: CommandType.Text);

                return value;
            }

        }

        public NS_MucLucNganSach GetMLNS(string iNamLamViec, Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"select * from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and iID_MaMucLucNganSach=@id";
                return conn.QueryFirstOrDefault<NS_MucLucNganSach>(
                    sql,
                    param: new
                    {
                        id,
                        iNamLamViec
                    });
            }
        }

        public DataTable MLNS_GetAll(string iNamLamViec, Dictionary<string, string> filters = null)
        {
            filters = filters ?? new Dictionary<string, string>();

            //var sql = @"select * from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and sLNS !='' order by sXauNoiMa";
            var sql = FileHelpers.GetSqlQuery("mlns_table.sql");
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new { iNamLamViec });
                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });
                return cmd.GetTable();
            }
        }

        public bool MLNS_Delete(Guid id, IDbConnection conn = null)
        {
            var result = false;
            var dispose = conn == null;

            conn = conn ?? _connectionFactory.GetConnection();
            var entity = conn.Get<NS_MucLucNganSach>(id);
            entity.iTrangThai = 0;
            entity.dNgaySua = DateTime.Now;

            result = conn.Update(entity);

            if (dispose)
                conn.Dispose();

            return result;
        }

        public bool MLNS_Update(NS_MucLucNganSach entity, IDbConnection conn = null)
        {
            var result = false;
            var dispose = conn == null;

            conn = conn ?? _connectionFactory.GetConnection();
            entity.dNgaySua = DateTime.Now;

            // kiem tra cap nhat lai ma hang cha
            var sXauNoiMa = new List<string>()
            {
                entity.sLNS,
                entity.sL,
                entity.sK,
                entity.sM,
                entity.sTM,
                entity.sTTM,
                entity.sNG,
                entity.sTNG
            }.Where(x => !string.IsNullOrWhiteSpace(x)).Join("-");

            entity.sTNG = entity.sTNG ?? "";
            entity.sNG = entity.sNG ?? "";
            entity.sTTM = entity.sTTM ?? "";
            entity.sTM = entity.sTM ?? "";
            entity.sM = entity.sM ?? "";
            entity.sL = entity.sL ?? "";
            entity.sK = entity.sK ?? "";

            #region hang cha

            // khong xac dinh nganh la hang cha
            entity.bLaHangCha = string.IsNullOrWhiteSpace(entity.sNG);

            if (entity.sXauNoiMa != sXauNoiMa || !entity.iID_MaMucLucNganSach_Cha.HasValue)
            {
                entity.sXauNoiMa = sXauNoiMa;

                var parent = MLNS_GetParent(entity);
                if (parent != null)
                {
                    entity.iID_MaMucLucNganSach_Cha = parent.iID_MaMucLucNganSach;
                }
            }
            #endregion

            if (entity.iID_MaMucLucNganSach == Guid.Empty)
            {
                entity.iID_MaMucLucNganSach = Guid.NewGuid();
                entity.iTrangThai = 1;
                entity.dNgayTao = DateTime.Now;
                entity.sNhapTheoTruong = "sLNS";

                result = conn.Insert(entity) != null;
            }
            else
            {
                result = conn.Update(entity);
            }

            if (dispose)
                conn.Dispose();

            return result;
        }

        private NS_MucLucNganSach MLNS_GetParent(NS_MucLucNganSach entity)
        {
            var xauNoiMas = new List<string>()
            {
                entity.sLNS,
                entity.sL,
                entity.sK,
                entity.sM,
                entity.sTM
            }.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            var sql = @"

select top(1) * from NS_MucLucNganSach
where   iTrangThai=1
        and iNamLamViec=@iNamLamViec
        and sXauNoiMa=@sXauNoiMa
";
            using (var conn = _connectionFactory.GetConnection())
            {
                var count = xauNoiMas.Count();
                for (int i = count - 1; i >= 0; i--)
                {
                    var sXauNoiMa = xauNoiMas.Join("-");
                    if (entity.sXauNoiMa != sXauNoiMa)
                    {
                        var parent = conn.QueryFirstOrDefault<NS_MucLucNganSach>(sql, new
                        {
                            iNamLamViec = entity.iNamLamViec,
                            sXauNoiMa
                        });

                        if (parent == null)
                        {
                            xauNoiMas.RemoveAt(i);
                        }
                        else
                        {
                            return parent;
                        }
                    }
                    else
                    {
                        xauNoiMas.RemoveAt(i);
                    }

                }
            }

            return null;

        }

        public DataTable MucLucDTKT_GetAll(string iNamLamViec, Dictionary<string, string> filters = null)
        {
            filters = filters ?? new Dictionary<string, string>();

            var sql = FileHelpers.GetSqlQuery("muclucdtkt_table.sql");
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new { iNamLamViec });
                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });
                return cmd.GetTable();
            }
        }

        //public bool MucLucDTKT_Delete(Guid id, IDbConnection conn = null)
        //{
        //    var result = false;
        //    var dispose = conn == null;

        //    conn = conn ?? _connectionFactory.GetConnection();
        //    var entity = conn.Get<DTKT_MucLuc>(id);
        //    //entity.Status = false;
        //    //entity.EditedDate = DateTime.Now;

        //    result = conn.Update(entity);

        //    if (dispose)
        //        conn.Dispose();

        //    return result;
        //}

        //public bool MucLucDTKT_Update(DTKT_MucLuc entity, IDbConnection conn = null)
        //{
        //    var result = false;
        //    var dispose = conn == null;

        //    conn = conn ?? _connectionFactory.GetConnection();
        //    //entity.EditedDate = DateTime.Now;
        //    entity.Code = entity.Code ?? "";

        //    #region hang cha

        //    // khong xac dinh nganh la hang cha
        //    entity.IsParent = (entity.Code.Length == 9);

        //    if (!entity.Id_Parent.HasValue)
        //    {

        //        var parent = MucLucDTKT_GetParent(entity);
        //        if (parent != null)
        //        {
        //            entity.Id_Parent = parent.Id;
        //        }
        //    }
        //    #endregion

        //    if (entity.Id == Guid.Empty)
        //    {
        //        entity.Id = Guid.NewGuid();
        //        //entity.Status = true;
        //        entity.DateCreated = DateTime.Now;

        //        result = conn.Insert(entity) != null;
        //    }
        //    else
        //    {
        //        result = conn.Update(entity);
        //    }

        //    if (dispose)
        //        conn.Dispose();

        //    return result;
        //}

        //private DTKT_MucLuc MucLucDTKT_GetParent(DTKT_MucLuc entity)
        //{

        //    var xauNoiMas = entity.Code.Split('-').ToList();

        //    var sql = @"

        //                select  top(1) * from DTKT_MucLuc
        //                where   Status=1
        //                        and WorkingYear=@year
        //                        and Code=@code
        //                ";
        //    using (var conn = _connectionFactory.GetConnection())
        //    {
        //        var count = xauNoiMas.Count();
        //        for (int i = count - 1; i >= 0; i--)
        //        {
        //            var sXauNoiMa = xauNoiMas.Join("-");
        //            if (entity.Code != sXauNoiMa)
        //            {
        //                var parent = conn.QueryFirstOrDefault<DTKT_MucLuc>(sql, new
        //                {
        //                    //iNamLamViec = entity.WorkingYear,
        //                    sXauNoiMa
        //                });

        //                if (parent == null)
        //                {
        //                    xauNoiMas.RemoveAt(i);
        //                }
        //                else
        //                {
        //                    return parent;
        //                }
        //            }
        //            else
        //            {
        //                xauNoiMas.RemoveAt(i);
        //            }

        //        }
        //    }

        //    return null;

        //}

        public DataTable Nganh_GetAll(string nam, string username, string id_phongban = null)
        {
            if (CheckParam_PhongBan(id_phongban).IsEmpty() || username.IsEmpty())
            {
                username = id_phongban;
            }

            var sql = @"

select iID_MaNganh, iID_MaNganhMLNS, sTenNganh = (iID_MaNganh + ' - ' + sTenNganh) from NS_MucLucNganSach_Nganh
where   iTrangThai=1
        and iNamLamViec = @nam
        and sMaNguoiQuanLy like @username
order by iID_MaNganh

";
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new { username = $"%{username}%", nam });
                var dt = cmd.GetTable();
                return dt;
            }

        }
        public DataTable ChuyenNganh_GetAll(string nam)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("ns_getchuyennganh", conn))
            {
                cmd.AddParams(new { nam });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }

        }

        public DataTable ChuyenNganh_GetAll(string nam, string user_name, string id_phongban = null)
        {
            var nganh = CheckParam_PhongBan(id_phongban).IsEmpty() ?
              "-1" :
              Nganh_GetAll(nam, CheckParam_Username(user_name, id_phongban), id_phongban)
                  .AsEnumerable()
                  .Select(r => r.Field<string>("iID_MaNganhMLNS"))
                  .JoinDistinct();

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ns_getchuyennganh", conn))
            {
                cmd.AddParams(new { nam, nganh = nganh.ToParamString() });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }

        }

        public string ChuyenNganh_Get(string nam, string nganh)
        {
            var sql = @"
                select sMoTa from NS_MucLucNganSach
                where iTrangThai=1 and sNG=@iID_MaNganh and sLNS = '' and iNamLamViec = @nam

                ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var value = conn.QueryFirstOrDefault<string>(
                    sql,
                    param: new
                    {
                        @iID_MaNganh = nganh,
                        @nam = nam,
                    },
                    commandType: CommandType.Text);

                return value;
            }

        }

        public DataTable Nganh_GetAll_ByPhongBan(string nam, string id_phongban)
        {
            if (id_phongban.IsNotEmpty() && id_phongban != "-1")
            {
                id_phongban = $"b{id_phongban}";
            }
            else
            {
                id_phongban = string.Empty;
            }
            var sql = @"

select iID_MaNganh, iID_MaNganhMLNS, sTenNganh = (iID_MaNganh + ' - ' + sTenNganh) from NS_MucLucNganSach_Nganh
where   iTrangThai=1
        and iNamLamViec = @nam
        and (sMaNguoiQuanLy like @id_phongban)
order by iID_MaNganh

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new { nam, id_phongban = $"%{id_phongban}%" });
                return dt;
            }
        }




        public NS_MucLucNganSach_Nganh Nganh_Get(string nam, string ng, string username = null)
        {

            if (ng.IsEmpty())
                return new NS_MucLucNganSach_Nganh();

            var sql = @"

select iID_MaNganh, iID_MaNganhMLNS, sMoTa = (iID_MaNganh + ' - ' + sTenNganh), sTenNganh from NS_MucLucNganSach_Nganh
where   iTrangThai=1
        and iNamLamViec = @nam
        and iID_MaNganh=@ng
        and sMaNguoiQuanLy like @username
order by iID_MaNganh

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_MucLucNganSach_Nganh>(
                    sql,
                    param: new { ng, username = $"%{username ?? ""}%", nam });

                return entity;
            }

        }

        public string CheckParam_PhongBan(string id_phongban)
        {
            if (id_phongban == "-1" ||
                id_phongban == "02" ||
                id_phongban == "11")
            {
                return string.Empty;
            }

            return id_phongban;
        }

        public string CheckParam_Username(string username, string id_phongban = null)
        {
            if ((!string.IsNullOrWhiteSpace(username) && GetUserRoleType(username) != (int)UserRoleType.TroLyPhongBan) ||
                string.IsNullOrWhiteSpace(CheckParam_PhongBan(id_phongban)))
            {
                username = string.Empty;
            }

            return username;
        }

        public string User_FullName(string username)
        {
            var fullname = username;

            var sql = FileHelpers.GetSqlQuery("user_fullname");
            using (var conn = _connectionFactory.GetConnection())
            {
                var user = conn.QueryFirstOrDefault(sql,
                    param: new { username },
                    commandType: CommandType.Text);
                if (user != null)
                {
                    fullname = user.FullName;
                }
            }


            return fullname;
        }

        public bool User_FullName_Update(string username, string fullname)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"


update aspnet_Membership
set FullName=@fullname
where   UserId in (                 
                select UserId from aspnet_Users                 
                where   UserName=@username)



";
                return conn.Execute(sql,
                        param: new { username, fullname },
                        commandType: CommandType.Text) > 0;

            }

        }

        public string Nganh_GetAll_ChuyenNganh(string username, string id_phongban)
        {
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(id_phongban))
                return string.Empty;

            var phongban = string.IsNullOrWhiteSpace(username) ? id_phongban : GetPhongBan(username).sKyHieu;
            var nganh = CheckParam_PhongBan(id_phongban).IsEmpty() ?
              "-1" :
              Nganh_GetAll(CheckParam_Username(username, id_phongban), phongban)
                  .AsEnumerable()
                  .Select(r => r.Field<string>("iID_MaNganhMLNS"))
                  .JoinDistinct();
            return nganh;
        }

        /// <summary>
        /// Lay danh sach chuyen nganh theo Username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="nam"></param>
        /// <returns></returns>
        public string Nganh_GetAll_ChuyenNganh(string username, int nam)
        {
            var sql = @"
select  iID_MaNganhMLNS from NS_MucLucNganSach_Nganh
where   iNamLamViec=@nam
		and sMaNguoiQuanLy like '%'+ @username +'%'
";

            using (var con = _connectionFactory.GetConnection())
            {
                var r = con.Query<string>(sql, new
                {
                    nam,
                    username
                }).Join()
                    .ToList()
                    .JoinDistinct();

                return r;
            }
        }


        public IEnumerable<NS_DonVi> GetDonviByPhongBanId(string namLamViec, string id_phongban)
        {
            var sql = @"

select iID_MaDonVi, sMoTa=iID_MaDonVi + ' - ' + sTen from NS_DonVi
where   iTrangThai=1
        and iNamLamViec_DonVi=@NamLamViec
        and iID_MaDonVi in (
                    select iID_MaDonVi from (select iID_MaPhongBan, iID_MaDonVi from NS_PhongBan_DonVi where iTrangThai=1 and iNamLamViec=@NamLamViec) as a
                    inner join 
                    (select iID_MaPhongBan, sKyHieu from NS_PhongBan where iTrangThai=1) as b
                    on b.iID_MaPhongBan=a.iID_MaPhongBan 
                    where (@id_phongban is null or sKyHieu=@id_phongban)
                )

order by iID_MaDonVi
";
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<NS_DonVi>(sql,
                    new
                    {
                        namLamViec,
                        id_phongban = id_phongban.ToParamString(),
                    });
            }
        }

        public DataTable GetDonviByPhongBan(string namLamViec, string id_phongban)
        {
            var sql = @"

select iID_MaDonVi, sMoTa=iID_MaDonVi + ' - ' + sTen from NS_DonVi
where   iTrangThai=1
        and iNamLamViec_DonVi=@NamLamViec
        and iID_MaDonVi in (
                    select iID_MaDonVi from (select iID_MaPhongBan, iID_MaDonVi from NS_PhongBan_DonVi where iTrangThai=1 and iNamLamViec=@NamLamViec) as a
                    inner join 
                    (select iID_MaPhongBan, sKyHieu from NS_PhongBan where iTrangThai=1) as b
                    on b.iID_MaPhongBan=a.iID_MaPhongBan 
                    where (@id_phongban is null or sKyHieu=@id_phongban)
                )

order by iID_MaDonVi
";
            using (var conn = _connectionFactory.GetConnection())
            {
                return
                    conn.GetTable(sql, new
                    {
                        namLamViec,
                        id_phongban = id_phongban.ToParamString(),
                    });
            }
        }

        public IEnumerable<NS_MucLucNganSach> MLNS_GetAll(string namLamViec)
        {
            var cacheKey = $"mlns_{namLamViec}";
            return _cacheService.CachePerRequest(
                cacheKey,
                () => mlns_GetAll(namLamViec), CacheTimes.FiveMinute);
        }

        private IEnumerable<NS_MucLucNganSach> mlns_GetAll(string namLamViec)
        {
            var sql = @"
select  sXauNoiMa,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,bLaHangCha
        ,iID_MaMucLucNganSach
        ,iID_MaMucLucNganSach_Cha        
from NS_MucLucNganSach
where   iTrangThai=1
        and iNamLamViec=dbo.f_ns_nammlns(@NamLamViec)
        and sLNS<>''
order by sXauNoiMa

";

            using (var conn = _connectionFactory.GetConnection())
            {
                var query = conn.Query<NS_MucLucNganSach>(sql, new { namLamViec });
                return query;
            }
        }

        /// <summary>
        /// Longsam - 25/4/2019
        /// 
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public IEnumerable<NS_MucLucNganSach> GetMLNS_All(string iNamLamViec)
        {
            return _cacheService.CachePerRequest($"mlns_{iNamLamViec}_full", () => getMLNS_All(iNamLamViec), CacheTimes.FiveMinute);
        }

        private IEnumerable<NS_MucLucNganSach> getMLNS_All(string iNamLamViec)
        {
            var sql = "select * from f_mlns_full(@iNamLamViec)";
            using (var conn = _connectionFactory.GetConnection())
            {
                var query = conn.Query<NS_MucLucNganSach>(sql, new { iNamLamViec }).ToList();

                #region them mo ta

                query.AddRange(new[]
                {
                    new NS_MucLucNganSach()
                    {
                        sLNS = "1040400",
                        sXauNoiMa = "1040400",
                        sMoTa = "Phân cấp toàn quân"
                    },
                    new NS_MucLucNganSach()
                    {
                        sLNS = "1040500",
                        sXauNoiMa = "1040500",
                        sMoTa = "Phân cấp bản thân"
                    },
                });


                #endregion

                return query;
            }
        }


        public DataTable GetMLNS_DataTable(string iNamLamViec)
        {
            return _cacheService.CachePerRequest($"mlns_{iNamLamViec}_full_dt", () => getMLNS_DataTable(iNamLamViec), CacheTimes.OneHour);
        }
        private DataTable getMLNS_DataTable(string iNamLamViec)
        {
            var sql = "select * from f_mlns_full(@iNamLamViec)";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new { iNamLamViec });

                var sXauNoiMa = "sXauNoiMa";
                var sXauNoiMa_Cha = "sXauNoiMa_Cha";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    var xauNoiMa = row.Field<string>(sXauNoiMa);
                    if (xauNoiMa.Length == 1)
                    {
                        row.SetField(sXauNoiMa_Cha, string.Empty);
                    }
                    else if (xauNoiMa.Length == 3)
                    {
                        row.SetField(sXauNoiMa_Cha, xauNoiMa.PadLeft(1));
                    }
                    else if (xauNoiMa.Length == 5)
                    {
                        row.SetField(sXauNoiMa_Cha, xauNoiMa.PadLeft(3));
                    }
                    else if (row.Field<string>("sM").IsEmpty())
                    {
                        row.SetField(sXauNoiMa_Cha, row.Field<string>("sLNS"));
                    }
                    else if (row.Field<string>("sTM").IsEmpty())
                    {
                        row.SetField(sXauNoiMa_Cha, row.Field<string>("sLNS"));

                        //row.SetField(sXauNoiMa_Cha, new List<string> {
                        //    row.Field<string>("sLNS"),
                        //    row.Field<string>("sL"),
                        //    row.Field<string>("sK")
                        //}.Join("-"));
                    }
                    else if (!row.Field<string>("sTM").IsEmpty())
                    {
                        row.SetField(sXauNoiMa_Cha, new List<string> {
                            row.Field<string>("sLNS"),
                            row.Field<string>("sL"),
                            row.Field<string>("sK"),
                            row.Field<string>("sM")
                        }.Join("-"));
                    }
                    //else if (row.Field<string>("sTTM").IsEmpty())
                    //{
                    //    row.SetField(sXauNoiMa_Cha, new List<string> {
                    //        row.Field<string>("sLNS"),
                    //        row.Field<string>("sL"),
                    //        row.Field<string>("sK"),
                    //        row.Field<string>("sM"),
                    //        row.Field<string>("sTM")
                    //    }.Join("-"));
                    //}
                    else
                    {
                    }
                }


                return dt;
            }
        }

        #endregion

        public DataTable AddMoTaM(DataTable dt, string iNamLamViec = null)
        {
            if (dt.GetColumnNames().Any(x => x == "sMoTa")) return dt;

            var count = dt.Columns.Count;
            dt.Columns.Add("sMoTa");

            var lns = "sM,sTM".Split(',').ToList();
            var columnNames = dt.GetColumnNames();

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
                    r.ItemArray[count - 1] :
                    items.Join("-");

                    if (string.IsNullOrWhiteSpace(iNamLamViec))
                        iNamLamViec = DateTime.Now.Year.ToString();

                    r["sMoTa"] = getMLNS_MTM(iNamLamViec, key.ToString()).Trim();
                });
            return dt;
        }
        protected string getMLNS_MTM(string iNamLamViec, string lns)
        {
            var sql = @"select  distinct sXauNoiMa, sMoTa
                        from
                                (
                                select  sXauNoiMa = case when sTM <> '' then sM + '-' + sTM else sM end
                                        , sMoTa from NS_MucLucNganSach 
                                where   iNamLamViec = @nam and sM <> '0000' and sTTM = '' and sM <> '' and iTrangThai = 1 and(sLNS like '1%' or sLNS like '2%')) a";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new { nam = iNamLamViec });
                var entity = dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("sXauNoiMa") == lns);

                return entity==null ? "" : entity.Field<string>("sMoTa");

            }
        }

        public NS_DonVi GetDonViByMaDonVi(string namLamViec, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new NS_DonVi();

            var sql = @"
            select  *
            from    NS_DonVi
            where   iTrangThai=1
                    and iID_MaDonVi=@iID_MaDonVi
                    and iNamLamViec_DonVi=@iNamLamViec
            ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_DonVi>(sql, 
                    new
                    {
                    iID_MaDonVi = id,
                    iNamLamViec = namLamViec,                        
                    }, null, null, CommandType.Text);

                return entity;
            }
        }

        public NS_DonVi GetDonViById(string namLamViec, string iID_Ma)
        {
            if (string.IsNullOrWhiteSpace(iID_Ma))
                return new NS_DonVi();

            var sql = @"
            select  *
            from    NS_DonVi
            where   iTrangThai=1
                    and iID_Ma=@iID_Ma
                    and iNamLamViec_DonVi=@iNamLamViec
            ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_DonVi>(sql,
                    new
                    {
                        iNamLamViec = namLamViec,
                        iID_Ma = iID_Ma,
                    }, null, null, CommandType.Text);

                return entity;
            }
        }
        public NS_PhongBan GetPhongBanById(Guid id)
        {
           
            var sql = @"
            select  *
            from    NS_PhongBan
            where  iID_MaPhongBan=@iID_Ma
            ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_PhongBan>(sql,
                    new
                    {
                        iID_Ma = id,
                    }, null, null, CommandType.Text);

                return entity;
            }
        }
        public VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAnByID(string iID_DonVi)
        {
            if (string.IsNullOrWhiteSpace(iID_DonVi))
                return new VDT_DM_DonViThucHienDuAn();

            var sql = @"
            select  *
            from    VDT_DM_DonViThucHienDuAn
            where   iID_DonVi=@iID_DonVi
            ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<VDT_DM_DonViThucHienDuAn>(sql,
                    new
                    {
                        iID_DonVi = iID_DonVi,
                    }, null, null, CommandType.Text);

                return entity;
            }
        }

        public VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAnByMa(string iID_MaDonVi)
        {
            if (string.IsNullOrWhiteSpace(iID_MaDonVi))
                return new VDT_DM_DonViThucHienDuAn();

            var sql = @"
            select  *
            from    VDT_DM_DonViThucHienDuAn
            where   iID_MaDonVi=@iID_MaDonVi
            ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<VDT_DM_DonViThucHienDuAn>(sql,
                    new
                    {
                        iID_MaDonVi = iID_MaDonVi,
                    }, null, null, CommandType.Text);

                return entity;
            }
        }
    }
}
