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
    public interface IDuToanBsService : IServiceBase
    {
        IEnumerable<DTBS_ChungTu> GetDots_TongHop(string nam, string username);
        DTBS_ChungTu_TLTH GetDotGom(string dot);
        DataTable GetDots(string nam, string username);
        DataTable GetDotNhaps(string nam, string username);
        DataTable GetDots_Gom(string nam, string username);
        DataTable GetDotsTLTHCuc(string nam, string username);

        DataTable GetDots(string nam, string username, string lns = null);
        DataTable GetDots_NSBD(string nam, string username);
        DataTable GetDots_NSNN(string nam, string username);

        string GetDot_MoTa(string nam, string username, string iID_MaPhongBan, string iID_MaDot, string sLNS);

        DataTable GetNganhs(string nam, string dot, string username, string lns = "");
        DataTable GetNganh_NSBD(string nam, string dot, string username);
        DataTable GetNganh_NSNN(string nam, string dot, string username);
        DataTable GetNganh_NSNN_207(string nam, string dot, string username);

        DataTable GetNganhs_Gom(string nam, string dot, string username, string lns = "");


        IEnumerable<string> GetChungTus_Gom(string iID_MaChungTuGom);
        IEnumerable<string> GetChungTus_GomTHCuc(string iID_MaChungTuGom);

        IEnumerable<string> GetChungTus_DotNgay(string iID_MaDot, string username);
        IEnumerable<string> GetChungTus_DotNgay(string iID_MaDot, string username, string iID_MaPhongBan);
        IEnumerable<string> GetChungTus_DotNgay_Nam(string iID_MaDot, string username, string iID_MaPhongBan, string nam);
        IEnumerable<string> GetChungTus_DotNgay_NSNN(string iID_MaDot, string username, string iID_MaPhongBan);

        //Dictionary<string, string> GetDonviTheoChungTus(string nam, string iID_MaDot, string iID_MaDonVi, string iID_MaPhongBan);
        Dictionary<string, string> GetDonviTheoChungTus(string nam, string iID_MaDot, string iID_MaDonVi, string iID_MaPhongBan = null, int lenId_DV = 0, string sDSLNS = null);
        Dictionary<string, string> GetDonviDT_DNs(string nam, string iID_MaDot, string iID_MaDonVi, string iID_MaPhongBan = null, string iID_MaPhongBanNguon = null, int lenId_DV = 0, string sDSLNS = null);
        Dictionary<string, string> GetPhongBanTheoChungTus(string nam, string iID_MaDot, string iID_MaDonVi, string sDSLNS = null);

        Dictionary<string, string> GetDonviTheoDot(string iID_MaDot);

        DataTable GetLNS(string nam, string username, string dDotNgay, string iID_MaPhongBan, string iID_MaDonVi);
        DataTable GetLNS_Gom(string nam, string username, string iID_MaChungTu_TLTH, string iID_MaPhongBan, string iID_MaDonVi);
        string GetGhiChu(string username, string ten);
        bool UpdateGhiChu(string username, string ten, string ghiChu);
    }

    public class DuToanBsService : IDuToanBsService
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private static IDuToanBsService _default;

        public static IDuToanBsService Default
        {
            get { return _default ?? (_default = new DuToanBsService()); }
        }

        public IEnumerable<DTBS_ChungTu> GetDots_TongHop(string nam, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DTBS_ChungTu>(sql,
                    param: new
                    {
                        iNamLamViec = nam,
                        username,
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public DTBS_ChungTu_TLTH GetDotGom(string dot)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_gom.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<DTBS_ChungTu_TLTH>(sql,
                    param: new
                    {
                        dot
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public DataTable GetDots_Gom(string nam, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_tonghop.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@username", username);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetDotsTLTHCuc(string nam, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_tonghopcuc.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@username", username.ToParamString());

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetDots(string nam, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_troly.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", _ngansachService.GetPhongBan(username).sKyHieu);
                    cmd.Parameters.AddWithValue("@username", username);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetDotNhaps(string nam, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_troly_nhap.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", _ngansachService.GetPhongBan(username).sKyHieu);
                    cmd.Parameters.AddWithValue("@username", username);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetDots(string nam, string username, string lns)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_lns.sql");

            using (var conn = _connectionFactory.GetConnection())
            {

                sql = sql.Replace("@sLNS", lns.ToParamLikeStartWith("sDSLNS"));

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", NganSachService.Default.GetPhongBan(username).sKyHieu);
                    cmd.Parameters.AddWithValue("@username", username);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetDots_NSBD(string nam, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_nsbd.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", _ngansachService.GetPhongBan(username).sKyHieu);
                    cmd.Parameters.AddWithValue("@username", username);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetDots_NSNN(string nam, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_nsnn.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", NganSachService.Default.GetPhongBan(username).sKyHieu);

                    return cmd.GetTable();
                }
            }
        }

        public string GetDot_MoTa(string nam, string username, string iID_MaPhongBan, string iID_MaDot, string sLNS)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_dot_mota.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                sql = sql.Replace("@sLNS", sLNS.ToParamLikeStartWith("sDSLNS"));

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@dNgayChungTu", iID_MaDot.ToParamDate());
                    //cmd.Parameters.AddWithValue("@sLNS", sLNS.ToParamLikeStartWith("dDSLNS"));

                    return cmd.GetValue();
                }
            }
        }


        #region chung tu

        public IEnumerable<string> GetChungTus_Gom(string iID_MaChungTuGom)
        {
            var sql = @"

SELECT  iID_MaChungTu
FROM    DTBS_ChungTu_TLTH
WHERE   iTrangThai=1 
        AND iID_MaChungTu_TLTH=@iID_MaChungTu

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<string>(sql,
                    param: new
                    {
                        iID_MaChungTu = iID_MaChungTuGom,
                    },
                    commandType: CommandType.Text).ToList();

                //if (items.Count() == 0)
                //{
                //    items.Add(iID_MaChungTuGom);
                //}

                return items;
            }

        }
        public IEnumerable<string> GetChungTus_GomTHCuc(string iID_MaChungTuGom)
        {
            var sql = @"

SELECT  iID_MaChungTu_TLTH
FROM    DTBS_ChungTu_TLTHCuc
WHERE   iTrangThai=1 
        AND iID_MaChungTu_TLTHCuc=@iID_MaChungTu

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<string>(sql,
                    param: new
                    {
                        iID_MaChungTu = iID_MaChungTuGom,
                    },
                    commandType: CommandType.Text).ToList();                

                return items;
            }

        }

        public IEnumerable<string> GetChungTus_DotNgay(string iID_MaDot, string username)
        {
            return GetChungTus_DotNgay(iID_MaDot, username, null);
        }

        public IEnumerable<string> GetChungTus_DotNgay(string iID_MaDot, string username, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_chungtu_dotngay.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<Guid>(sql,
                    param: new
                    {
                        dDotNgay = iID_MaDot,
                        username,
                        iID_MaPhongBan = iID_MaPhongBan.ToParamString(),
                        //sLNS = sLNS.ToParamString(),
                    },
                    commandType: CommandType.Text)
                    .Select(x => x.ToString())
                    .ToList();

                return items;
            }
        }

        public IEnumerable<string> GetChungTus_DotNgay_Nam(string iID_MaDot, string username, string iID_MaPhongBan, string nam)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_chungtu_dotngay_nam.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<Guid>(sql,
                    param: new
                    {
                        dDotNgay = iID_MaDot,
                        username,
                        iID_MaPhongBan = iID_MaPhongBan.ToParamString(),
                        nam,
                    },
                    commandType: CommandType.Text)
                    .Select(x => x.ToString())
                    .ToList();

                return items;
            }
        }

        public IEnumerable<string> GetChungTus_DotNgay_NSNN(string iID_MaDot, string username, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_chungtu_dotngay_nsnn.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<Guid>(sql,
                    param: new
                    {
                        dDotNgay = iID_MaDot.ToParamDate(),
                        username,
                        iID_MaPhongBan = iID_MaPhongBan.ToParamString(),
                    },
                    commandType: CommandType.Text)
                    .Select(x => x.ToString())
                    .ToList();

                return items;
            }
        }

        #endregion

        //public Dictionary<string, string> GetDonviTheoChungTus(string nam, string iID_MaDot, string iID_MaDonVi, string iID_MaPhongBan)
        //{
        //    var sql = FileHelpers.GetSqlQuery("dtbs_get_donvi_theodot.sql");
        //    using (var conn = _connectionFactory.GetConnection())
        //    {
        //        var items = conn.Query<dynamic>(sql,
        //            param: new
        //            {
        //                iNamLamViec = nam,
        //                iID_MaChungTu = iID_MaDot,
        //                iID_MaDonVi = iID_MaDonVi.IsEmpty() ? null : iID_MaDonVi,
        //                iID_MaPhongBan = iID_MaPhongBan.ToParamString(),
        //            },
        //            commandType: CommandType.Text);

        //        var dic = new Dictionary<string, string>();
        //        items.ToList()
        //            .ForEach(x =>
        //            {
        //                dic.Add(x.value, x.text);
        //            });

        //        return dic;
        //    }
        //}

        public Dictionary<string, string> GetDonviTheoChungTus(string nam, string iID_MaDot, string iID_MaDonVi, string iID_MaPhongBan, int lenDV = 0, string sDSLNS = null)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_donvi_theodot.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@lenDV", lenDV);
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaDot);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                    cmd.Parameters.AddWithValue("@sDSLNS", sDSLNS.ToParamString());

                    var dt = cmd.GetTable();
                    var dic = new Dictionary<string, string>();
                    dt.AsEnumerable().ToList()
                        .ForEach(x =>
                        {
                            dic.Add(x.Field<string>(0), x.Field<string>(1));
                        });

                    return dic;
                }
            }
        }

        public Dictionary<string, string> GetDonviDT_DNs(string nam, string iID_MaDot, string iID_MaDonVi, string iID_MaPhongBan, string iID_MaPhongBanNguon, int lenDV = 0, string sDSLNS = null)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_donvi_theodot_dt_dn.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@lenDV", lenDV);
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaDot);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaPhongBanNguon", iID_MaPhongBanNguon.ToParamString());
                    cmd.Parameters.AddWithValue("@sDSLNS", sDSLNS.ToParamString());

                    var dt = cmd.GetTable();
                    var dic = new Dictionary<string, string>();
                    dt.AsEnumerable().ToList()
                        .ForEach(x =>
                        {
                            dic.Add(x.Field<string>(0), x.Field<string>(1));
                        });

                    return dic;
                }
            }
        }

        public Dictionary<string, string> GetPhongBanTheoChungTus(string nam, string iID_MaDot, string iID_MaDonVi, string sDSLNS = null)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_phongban_theodot.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<dynamic>(sql,
                    param: new
                    {
                        iNamLamViec = nam,
                        iID_MaChungTu = iID_MaDot,
                        iID_MaDonVi = iID_MaDonVi.IsEmpty() ? null : iID_MaDonVi,
                        sDSLNS = sDSLNS.ToParamString()
                    },
                    commandType: CommandType.Text);

                var dic = new Dictionary<string, string>();
                items.ToList()
                    .ForEach(x =>
                    {
                        dic.Add(x.value, x.text);
                    });

                return dic;
            }
        }

        public Dictionary<string, string> GetDonviTheoDot(string iID_MaDot)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_donvi_theodot_2.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<dynamic>(sql,
                    param: new
                    {
                        iID_MaChungTu = iID_MaDot,
                    },
                    commandType: CommandType.Text);

                var dic = new Dictionary<string, string>();
                items.ToList()
                    .ForEach(x =>
                    {
                        dic.Add(x.value, x.text);
                    });

                return dic;
            }
        }

        #region nganhs


        public DataTable GetNganh_NSBD(string nam, string dot, string username)
        {
            return GetNganhs(nam, dot, username, "104,109");
        }

        public DataTable GetNganh_NSNN(string nam, string dot, string username)
        {
            return GetNganhs(nam, dot, username, "2%,3,4,107%");
            //return GetNganhs(nam, dot, username, "2%,3010000");
        }

        public DataTable GetNganh(string nam, string dot, string username)
        {
            return GetNganhs(nam, dot, username, "1,2,3,4");
        }


        public DataTable GetNganhs(string nam, string dot, string username, string lns = "")
        {
            var sql = @"

select  distinct sNG
from    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1
        and iNamLamViec=@iNamLamViec
        and (iID_MaPhongBanDich=@iID_MaPhongBan or @iID_MaPhongBan is null)
        and sID_MaNguoiDungTao=@username
        and (sLNS like @sLNS)
        and iID_MaChungTu in 
            (select iID_MaChungTu 
             from   DTBS_ChungTu 
             where  iTrangThai=1 
                    and iNamLamViec=@iNamLamViec 
                    and (iID_MaPhongBanDich=@iID_MaPhongBan or @iID_MaPhongBan is null)
                    and sID_MaNguoiDungTao=@username
                    and dNgayChungTu=@dDotNgay)


";
            using (var conn = _connectionFactory.GetConnection())
            {
                sql = sql.Replace("@sLNS", lns.ToParamLike("sLNS", "'{0}%'"));

                var iID_MaPhongBan = _ngansachService.GetPhongBan(username).sKyHieu;
                if (iID_MaPhongBan == "02")
                {
                    iID_MaPhongBan = "";
                }
                var nganhList = conn.Query<string>(
                    sql: sql,
                    param: new
                    {
                        iNamLamViec = nam,
                        username,
                        iID_MaPhongBan = iID_MaPhongBan.ToParamString(),
                        dDotNgay = dot.ToParamDate(),

                    },
                    commandType: CommandType.Text);

                //sql = lns.Split(new char[] { ',' }).Any(x => x[0] == '2' || x[0] == '3') ?
                //    @"


                //select iID_MaNganh, sTenNganh
                //from NS_MucLucNganSach_Nganh_NhaNuoc
                //where iTrangThai = 1
                //        and(iID_MaDonVi like @iID_MaNganhMLNS)  
                //        and sMaNguoiQuanLy like '%' + @username + '%'


                //" :
                //    @"

                //select  iID_MaNganh, sTenNganh
                //from    NS_MucLucNganSach_Nganh
                //where   iTrangThai=1
                //        and (iID_MaNganhMLNS like @iID_MaNganhMLNS)
                //        and sMaNguoiQuanLy like '%'+@username+'%'


                //";

                sql =
                  @"

select  iID_MaNganh, sTenNganh = (iID_MaNganh + ' - ' +  sTenNganh)
from    NS_MucLucNganSach_Nganh
where   iTrangThai=1
        and iNamLamViec = @iNamLamViec
        and (iID_MaNganhMLNS like @iID_MaNganhMLNS)
        --and sMaNguoiQuanLy like '%'+@username+'%'
order by iID_MaNganh
                            

                ";

                sql = sql.Replace("@iID_MaNganhMLNS", nganhList.ToParamLike("iID_MaNganhMLNS"));
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@username", username);
                    return cmd.GetTable();
                }

            }


        }

        public DataTable GetNganh_NSNN_207(string nam, string dot, string username)
        {
            var lns = "207";
            var sql = @"

select  distinct sNG
from    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1
        and iNamLamViec=@iNamLamViec
        and iID_MaPhongBanDich=@iID_MaPhongBan
        and sID_MaNguoiDungTao=@username
        and (sLNS like @sLNS)
        and iID_MaChungTu in 
            (select iID_MaChungTu 
             from   DTBS_ChungTu 
             where  iTrangThai=1 
                    and iNamLamViec=@iNamLamViec 
                    and iID_MaPhongBanDich=@iID_MaPhongBan
                    and sID_MaNguoiDungTao=@username
                    and dNgayChungTu=@dDotNgay)


";
            using (var conn = _connectionFactory.GetConnection())
            {
                sql = sql.Replace("@sLNS", lns.ToParamLike("sLNS", "'{0}%'"));

                var nganhList = conn.Query<string>(
                    sql: sql,
                    param: new
                    {
                        iNamLamViec = nam,
                        username,
                        iID_MaPhongBan = NganSachService.Default.GetPhongBan(username).sKyHieu,
                        dDotNgay = dot.ToParamDate(),

                    },
                    commandType: CommandType.Text);


                sql =
                  @"

                  select  iID_MaNganh, sTenNganh
                                from    NS_MucLucNganSach_Nganh_NhaNuoc
                                where   iTrangThai=1
                                        and iNamLamViec = @iNamLamViec
                                        --and (iID_MaNganhMLNS like @iID_MaNganhMLNS)
                                        and iID_MaNganh in (select * from f_split(@sNG))
                                        and sMaNguoiQuanLy like '%'+@username+'%'

                ";

                //sql = sql.Replace("@iID_MaNganhMLNS", nganhList.ToParamLike("iID_MaNganhMLNS"));
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@sNG", nganhList.Join());
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    return cmd.GetTable();
                }

            }

        }

        public DataTable GetNganhs_Gom(string nam, string dot, string username, string lns = "")
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_nganh_dot_gom.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                sql = sql.Replace("@sLNS", lns.ToParamLike("sLNS", "'{0}%'"));

                var nganhList = conn.Query<string>(
                    sql: sql,
                    param: new
                    {
                        iNamLamViec = nam,
                        username,
                        iID_MaPhongBan = NganSachService.Default.GetPhongBan(username).sKyHieu,
                        iID_MaChungTu = dot,
                    },
                    commandType: CommandType.Text);

                sql = @"

select  iID_MaNganh, sTenNganh
from    NS_MucLucNganSach_Nganh
where   iTrangThai=1
        and iNamLamViec = @iNamLamViec
        and (iID_MaNganhMLNS like @iID_MaNganhMLNS)
        and sMaNguoiQuanLy like '%'+@username+'%'
        

";
                sql = sql.Replace("@iID_MaNganhMLNS", nganhList.ToParamLike("iID_MaNganhMLNS"));
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@username", username);
                    var dt = cmd.GetTable();
                    return cmd.GetTable();
                }

            }
        }

        public DataTable GetLNS(string nam, string username, string dDotNgay, string iID_MaPhongBan, string iID_MaDonVi)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_donvi_lns.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@dNgayChungTu", dDotNgay.ToParamDate());
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);

                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetLNS_Gom(string nam, string username, string iID_MaChungTu_TLTH, string iID_MaPhongBan, string iID_MaDonVi)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_get_donvi_lns_gom.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu_TLTH);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);

                    return cmd.GetTable();
                }
            }
        }

        public string GetGhiChu(string username, string Ten)
        {
            #region definition input

            var sql = @"select	NoiDung 
                        from    DTBS_GHICHU
                        where   Ten = @Ten
                                and UserCreator = @username";

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
                        Ten
                    },
                    commandType: CommandType.Text);

                return entity ?? string.Empty;
            }
            #endregion
        }

        public bool UpdateGhiChu(string username, string ten, string GhiChu)
        {
            var sql = @"if not exists	(
                                        select	NoiDung 
                                        from    DTBS_GHICHU
                                        where   Ten = @ten
                                                and UserCreator = @username
				                        )

				                        insert into DTBS_GHICHU(Ten, NoiDung, UserCreator)
                                        values(@ten, @GhiChu, @username)

                        else

                                        update  DTBS_GHICHU
                                        set     NoiDung = @GhiChu
                                        where   Ten = @ten
                                                and UserCreator = @username";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        username,
                        ten,
                        GhiChu
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        #endregion
    }
}
