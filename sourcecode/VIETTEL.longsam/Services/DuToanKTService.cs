using Dapper;
using FlexCel.Report;
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
    public interface IDuToanKTService
    {
        Dictionary<string, string> GetRequestList();

        Dictionary<string, string> GetRequestList_PhongBan(string id_phongban = null);

        DataTable Get_DonVi_TheoBql_ExistData(string namLamViec, string username = null, string iID_MaPhongBan = null, int? request = 0);
        DataTable Nganh_ExistData_GetAll(string nam, string username = null);
        DataTable GetNganh(string nam, string id_phongban = null, string username = null);
        DataTable GetNganhBD(string nam, string id_phongban, string id_phongban_dich = null, string username = null, int times = 0);
        FlexCelReport FillDataTable(FlexCelReport fr, DataTable data, string fields, string iNamLamViec, string field = null);
        DataTable AddOrdinalsNum(DataTable dt, int level, string rang = null);
        Dictionary<string, string> UpdateParent(string NamLamViec, string code, string Id = "");

        #region dtkt lock

        DTKT_Lock GetLockById(int nam, string id_donvi);
        IEnumerable<DTKT_Lock> GetAllLock(int nam);
        bool CheckLock(int nam, string id_donvi, string id_phongban, string username, string ireq);
        DataTable GetChungTu_Lock(int nam, int isread, string iLoai = null, string iRequest = null, string id_phongban = null, string id_user = null, string id_donvi = null);

        #endregion
    }

    public class DuToanKTService : IDuToanKTService
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private static IDuToanKTService _default;

        public static IDuToanKTService Default
        {
            get { return _default ?? (_default = new DuToanKTService()); }
        }

        public Dictionary<string, string> GetRequestList_PhongBan(string id_phongban = null)
        {
            //TODO: kiem tra dot nhap tang giam theo phongban

            var requestList = new Dictionary<string, string>()
                {
                    {"-1", "-- Tổng hợp --" },
                    {"1", "Đợt 1" },
                    {"2", "Đợt 2" },
                    {"3", "Đợt 3" },
                    {"4", "Đợt 4" },
                    {"5", "Đợt 5" },
                };

            return requestList;
        }

        public Dictionary<string, string> GetRequestList()
        {
            return new Dictionary<string, string>()
            {
                { "0", "1 - Số đề nghị & đặc thù" },
                { "1", "2 - Số tăng, giảm NV" },
            };
        }

        public Dictionary<string, string> UpdateParent(string NamLamViec, string code, string Id = "")
        {
            var codes = code.Split('-');

            if (codes.Length == 1)
            {
                return new Dictionary<string, string>()
                {
                    { "Id_Parent", "00000000-0000-0000-0000-000000000000" },
                    { "IsParent", "true" },
                };
            }
            else
            {
                var isParent = "false";
                var Id_Parent = "";
                if (codes.Length > 1 && codes.Length < 5)
                {
                    isParent = "true";
                }

                var len = codes.Length - 1;
                while ((Id_Parent == "" || Id_Parent == Id || Id_Parent == "00000000-0000-0000-0000-000000000000") && len > 0)
                {
                    var code_parent = codes.Take(len).Join("-");

                    var sql =
                            @"

                        select      Id                                          
                        from        DTKT_MucLuc 
                        where       NamLamViec = @NamLamViec
                                    and Code = @code_parent         
                        order by    Code

                        ";
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {

                        Id_Parent = conn.QueryFirstOrDefault<Guid>(
                            sql,
                            new
                            {
                                NamLamViec,
                                code_parent,
                            }).ToString();
                    }
                    len = len - 1;
                }

                var result = new Dictionary<string, string>()
                {
                    { "Id_Parent", Id_Parent },
                    { "IsParent", isParent },
                };
                return result;
            }
        }

        public Dictionary<int, string> GetRequestList(string id_phongban)
        {
            var dic = new Dictionary<int, string>();
            if (id_phongban == "02" || id_phongban == "11")
            {
                dic.Add(1, "B2");
                dic.Add(2, "Cục duyệt");
            }
            else
            {
                dic.Add(0, "Bql");
            }

            return dic;
        }

        public DataTable Get_DonVi_TheoBql_ExistData(
            string nam,
            string username = null,
            string id_phongban = null,
            int? request = 0)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var iRequest = 0;
                if (id_phongban == "11")
                {
                    iRequest = 1;
                }
                var sql = FileHelpers.GetSqlQuery("dtkt_donvi.sql");
                var dt = conn.ExecuteDataTable(sql,
                    new
                    {
                        nam,
                        iRequest,
                        id_phongban = id_phongban.ToParamString(),
                        username = username.ToParamString(),
                        request = request,
                    },
                    CommandType.Text);

                return dt;
            }
        }

        public FlexCelReport FillDataTable(FlexCelReport fr, DataTable data, string fields, string iNamLamViec, string field = null)
        {

            //var fields = "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM";
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

            var checkColumns = "STT,Ng,Nganh,sMoTa";
            checkColumns.Split(',').ToList()
                .ForEach(c =>
                {
                    if (!columnNames.Contains(c))
                    {
                        dt.Columns.Add(c);
                    }
                });


            var lns = "Code".Split(',').ToList();

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

                    //var result = getResults(iNamLamViec, key.ToString());

                    //if (!string.IsNullOrWhiteSpace(result))
                    //{

                    //    string[] results = result.Split('/');
                    //    r["sMoTa"] = results[1];
                    //    r["STT"] = results[0];
                    //    r["Ng"] = results[2];
                    //    r["Nganh"] = results[3];
                    //}

                    var entity = getMoTa(iNamLamViec, key);
                    if (entity != null)
                    {
                        if (r.Field<string>("sMoTa").IsEmpty())
                            r["sMoTa"] = entity.sMoTa;

                        if (r.Field<string>("Ng").IsEmpty())
                            r["Ng"] = entity.Ng;

                        if (r.Field<string>("STT").IsEmpty())
                            r["STT"] = entity.STT;

                        if (r.Field<string>("Nganh").IsEmpty())
                            r["Nganh"] = entity.Nganh;
                    }
                });
            return dt;
        }

        protected string getResults(string iNamLamViec, string lns)
        {
            var sql =
    @"

        SELECT      TOP(1) LTRIM(RTRIM(STT)) + '/' + LTRIM(RTRIM(sMoTa)) + '/' + ISNULL(LTRIM(RTRIM(Ng)), '')  + '/' + ISNULL(LTRIM(RTRIM(Nganh)), '') as result
        FROM        DTKT_MucLuc
        WHERE       iTrangThai = 1
                    and NamLamViec=@iNamLamViec 
                    AND Code= @lns
        ORDER BY    Code

        ";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {

                var result = conn.QueryFirstOrDefault<string>(
                    sql,
                    new
                    {
                        iNamLamViec,
                        lns
                    });
                return result;
            }
        }

        protected DTKT_MucLuc getMoTa(string iNamLamViec, string lns)
        {
            var sql =
    @"

        SELECT      TOP(1) *
        FROM        DTKT_MucLuc
        WHERE       iTrangThai = 1
                    and NamLamViec=@iNamLamViec 
                    AND Code= @lns
        ORDER BY    Code

        ";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {

                var result = conn.QueryFirstOrDefault<DTKT_MucLuc>(
                    sql,
                    new
                    {
                        iNamLamViec,
                        lns
                    });
                return result;
            }

        }

        public DataTable Nganh_ExistData_GetAll(string nam, string username = null)
        {
            var sql = @"
                    select      Distinct iID_MaNganh, sTenNganh, iID_MaNganhMLNS
                    from        (
                                select distinct Ng 
                                from            DTKT_ChungTuChiTiet
                                where           iTrangThai = 1                                                
                                                and NamLamViec = @nam
                                                and iLoai = 2
                                                and (TuChi <> 0 or HangMua <> 0 or HangNhap <> 0)
                                                and (@user is null or UserCreator = @user)

                                union all 

                                select distinct Ng 
                                from            DTKT_ChungTuChiTiet
                                where           iTrangThai = 1                                                
                                                and NamLamViec = @nam
                                                and iLoai = 1
                                                and TuChi <> 0
                                ) as ng
                                
                                inner join 

                                (
                                select  iID_MaNganh, sTenNganh = (iID_MaNganh + ' - ' + sTenNganh), iID_MaNganhMLNS 
                                from    NS_MucLucNganSach_Nganh
                                where   iTrangThai=1
                                        and NamLamViec = @nam
                                        and (@username is null or sMaNguoiQuanLy like @username)
                                ) as ng_ng

                                on ng.Ng = ng_ng.iID_MaNganh
                    where       iID_MaNganh <> '02'
                    order by    iID_MaNganh
                    ";


            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (string.IsNullOrEmpty(username))
                {
                    cmd.AddParams(new
                    {
                        user = DBNull.Value,
                        username = DBNull.Value
                    });
                }
                else
                {
                    cmd.AddParams(new
                    {
                        user = username,
                        username = $"%{username}%"
                    });
                }
                cmd.AddParams(new
                {
                    nam
                });
                return cmd.GetTable();
            }

        }

        public DataTable AddOrdinalsNum(DataTable dt, int level, string rang = null)
        {
            DataTable vR = dt;
            vR.Columns.Add(new DataColumn($"STT", typeof(string)));
            int start = 1;

            for (int i = 0; i < vR.Rows.Count; i++)
            {
                if (level == 1)
                {
                    vR.Rows[i]["STT"] = (char)(i + 65);
                }
                else
                {
                    if (rang != null)
                    {
                        var count = vR.AsEnumerable().ToList()
                            .Where(x => !string.IsNullOrEmpty(x.Field<string>(rang)) && x.Field<string>(rang) == vR.Rows[i][rang].ToString())
                            .Select(x => x.Field<string>(rang)).Count();
                        if (level == 2)
                        {
                            vR.Rows[i]["STT"] = NumberExtension.NToR(start);
                        }
                        else if (level == 3)
                        {
                            vR.Rows[i]["STT"] = start;
                        }
                        if (start < count)
                        {
                            start++;
                        }
                        else
                        {
                            start = 1;
                        }
                    }
                    else if (level > 1 && level < 4)
                    {
                        if (level == 2)
                        {
                            vR.Rows[i]["STT"] = NumberExtension.NToR(i + 1);
                        }
                        else if (level == 3)
                        {
                            vR.Rows[i]["STT"] = i + 1;
                        }
                    }
                    else
                    {
                        vR.Rows[i]["STT"] = "-";
                    }
                }
            }
            return vR;
        }

        #region muc luc

        public Dictionary<string, string> updateParent(string NamLamViec, string code)
        {
            var codes = code.Split('-');

            if (codes.Length == 1)
            {
                return new Dictionary<string, string>()
                {
                    { "Id_Parent", "00000000-0000-0000-0000-000000000000" },
                    { "IsParent", "1" },
                };
            }
            else
            {
                var isParent = "false";
                var Id_Parent = "";
                if (codes.Length > 1 && codes.Length < 4)
                {
                    isParent = "true";
                }

                var code_parent = codes.Take(codes.Length - 1).Join("-");

                var sql =
                        @"

                        select      Id                                          
                        from        DTKT_MucLuc 
                        where       NamLamViec = @NamLamViec
                                    and Code = @code_parent         
                        order by    Code

                        ";
                using (var conn = ConnectionFactory.Default.GetConnection())
                {

                    Id_Parent = conn.QueryFirstOrDefault<Guid>(
                        sql,
                        new
                        {
                            NamLamViec,
                            code_parent,
                        }).ToString();
                }

                var result = new Dictionary<string, string>()
                {
                    { "Id_Parent", Id_Parent },
                    { "IsParent", isParent },
                };
                return result;
            }
        }

        #endregion

        #region dtkt lock


        public DTKT_Lock GetLockById(int nam, string id_donvi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"
select * from dtkt_lock
where   NamLamViec=@nam
        and Id_DonVi=@id_donvi
";
                var entity = conn.QueryFirstOrDefault<DTKT_Lock>(sql, new { nam, id_donvi });

                return entity;
            }
        }

        public IEnumerable<DTKT_Lock> GetAllLock(int nam)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"
select * from dtkt_lock
where   NamLamViec=@nam
        and iTrangThai=1
";
                var list = conn.Query<DTKT_Lock>(sql, new { nam });
                return list;
            }
        }

        public bool CheckLock(int nam, string id_donvi, string id_phongban, string username, string ireq)
        {
            var lockEntity = GetLockById(nam, id_donvi);
            var locked = lockEntity != null &&
                ((!string.IsNullOrWhiteSpace(lockEntity.Id_PhongBan) && lockEntity.Id_PhongBan.Split(',').Contains(id_phongban)
                    && string.IsNullOrWhiteSpace(lockEntity.iRequest) && string.IsNullOrWhiteSpace(lockEntity.Id_User)) ||
                 (!string.IsNullOrWhiteSpace(lockEntity.Id_User) && lockEntity.Id_User.Split(',').Contains(username)
                    && string.IsNullOrWhiteSpace(lockEntity.iRequest)) ||
                 (!string.IsNullOrWhiteSpace(lockEntity.iRequest) && lockEntity.iRequest.Split(',').Contains(ireq)
                    && !string.IsNullOrWhiteSpace(lockEntity.Id_PhongBan) && lockEntity.Id_PhongBan.Split(',').Contains(id_phongban)
                    && string.IsNullOrWhiteSpace(lockEntity.Id_User)) ||
                 (!string.IsNullOrWhiteSpace(lockEntity.iRequest) && lockEntity.iRequest.Split(',').Contains(ireq)
                    && !string.IsNullOrWhiteSpace(lockEntity.Id_User) && lockEntity.Id_User.Split(',').Contains(username)));
            return locked;
        }

        public DataTable GetChungTu_Lock(int nam, int isreadonly, string iloai = null, string irequest = null, string id_phongban = null, string id_user = null, string id_donvi = null)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_lockoropen_list.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.AddParams(new
                    {
                        nam,
                        isreadonly,
                        irequest = irequest.ToParamString(),
                        iloai = iloai.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_user = id_user.ToParamString(),
                        id_donvi = id_donvi.ToParamString(),
                    });
                    return cmd.GetTable();
                }
            }
        }

        #endregion

        #region nganh

        public DataTable GetNganhBD(string nam, string id_phongban, string id_phongban_dich = null, string username = null, int ireq = 0)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_nganh_bd.sql");
            if (ireq != 0)
            {
                sql = FileHelpers.GetSqlQuery("dtkt_nganh_bd_tg.sql");
            }
            username = _ngansachService.CheckParam_Username(username, id_phongban);
            id_phongban_dich = _ngansachService.CheckParam_PhongBan(id_phongban_dich);
            var user = username;

            if (_ngansachService.GetUserRoleType(username) != (int)UserRoleType.TroLyPhongBan)
            {
                user = "";
            }
            if (username != "")
            {
                username = $"%{username}%";
            }

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam,
                    irequest = ireq,
                    id_phongban_dich = id_phongban_dich.ToParamString(),
                    user = user.ToParamString(),
                    username = username.ToParamString(),
                });

                var dt = cmd.GetTable();
                return dt;
            }
        }

        public DataTable GetNganh(string nam, string id_phongban, string username = null)
        {
            username = _ngansachService.CheckParam_Username(username, id_phongban);

            var sql = FileHelpers.GetSqlQuery("dtkt_nganh.sql");
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam,
                    id_phongban = id_phongban.ToParamString(),
                    username = username.ToParamString(),
                    //ng = ng.ToParamString(),
                });

                var dt = cmd.GetTable();
                return dt;
            }

            #endregion



        }


    }
}
