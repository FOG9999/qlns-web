using Dapper;
using DapperExtensions;
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
    public interface ISKTService
    {
        #region Dùng chung
        FlexCelReport FillDataTable(FlexCelReport fr, DataTable data, string fields, string iNamLamViec, string tName, string field = null);
        FlexCelReport FillDataTable_NC(FlexCelReport fr, DataTable data, string fields = "X1 X2 X3 X4", string field = null);
        FlexCelReport FillDataTable_NC_S(FlexCelReport fr, DataTable data, string fields = "X1 X2 X3 X4", string field = null);
        FlexCelReport FillDataTable_SKT(FlexCelReport fr, DataTable data, string fields = "X1 X2 X3 X4", string field = null);
        DataTable AddOrdinalsNum(DataTable dt, int level, string rang = null);
        DataRow GetCauHinhNamDuLieu(int namLamViec);
        Dictionary<int,string> GetTieuDeDuLieuNamTruoc(int namLamViec);
        #endregion
        #region Mục lục số kiểm tra
        Dictionary<string, string> UpdateParent(string table, string NamLamViec, string code, string Id = "");
        Dictionary<string, string> UpdateParentId(string table, string NamLamViec, string code, string code_parent);
        string getIdMucLucNhuCau(string NamLamViec, string code);
        #endregion
        #region Chứng từ số kiểm tra
        DataTable GetNganhAll(int nam, string username, string id_phongban = null, string id_donvi = null);
        DataTable GetDonViAll(int nam, string id_phongban = null, string id_donvi = null);
        void DeleteChungTuCT(Guid id, string tablename);
        void LockChungTu(Guid id, string tablename);

        DataTable GetChungTu_BLock(int nam, int id_lock, string phongban, string iloais = null);
        #endregion
        #region Báo cáo        
        DataTable Get_DonVi_ExistData(string nam, int loai = 1, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataTable Get_Nganh_ExistData(string nam, int loai = 1, string donvis = null, string id_phongban = null, string id_phongbandich = null, string nganhs = null);
        DataTable Get_DonVi_TheoBql_ExistData(string nam, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataTable Get_DonViTG_TheoBql_ExistData(string nam, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataTable Get_DonVisDT_TheoBql_ExistData(string nam, string donvis = null, string id_phongbandich = null);
        DataTable Get_DonVisBVNC_TheoBql_ExistData(string nam, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataTable Get_DonVisGBV_TheoBql_ExistData(string nam, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataTable Get_Nganh_TheoBql_ExistData(string nam, string nganh = null, string id_phongban = null);
        DataTable Get_NganhBDKT_ExistData(string nam, string id_phongban = null);
        #endregion
        #region Khóa số liệu số kiểm tra
        SKT_Lock GetLockById(int nam, string id_donvi);
        IEnumerable<SKT_Lock> GetAllLock(int nam);
        bool CheckLock(int nam, string id_donvi, string id_phongban, string username);
        DataTable GetChungTu_Lock(int nam, int isread, string iLoai = null, string id_phongban = null, string id_user = null, string id_donvi = null);
        string GetGhiChu(string username, string Ten);
        bool UpdateGhiChu(string username, string ten, string GhiChu);
        #endregion
    }

    public class SKTService : ISKTService
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private static ISKTService _default;

        public static ISKTService Default
        {
            get { return _default ?? (_default = new SKTService()); }
        }

        #region Dùng chung
        public FlexCelReport FillDataTable(FlexCelReport fr, DataTable data, string fields, string iNamLamViec, string tName, string field = null)
        {
            List<string> lnsList = (fields).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("MoTa") || !data.GetColumnNames().Contains("STT"))
            {
                AddMoTa(data, tName, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", fields.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count(); i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : string.Format("{0},{1}", lns.Join(), field);
                var dt = dtSource.SelectDistinct(tableName, distinctField);
                AddMoTa(dt, tName, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

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

        public FlexCelReport FillDataTable_NC(FlexCelReport fr,
            DataTable data,
            string fields = "X1 X2 X3 X4",
            string field = null)
        {
            var iNamLamViec = PhienLamViecViewModel.Current.iNamLamViec;
            var table_name = "SKT_MLNhuCau";
            List<string> lnsList = (fields).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("MoTa") || !data.GetColumnNames().Contains("STT"))
            {
                AddMoTa(data, table_name, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", fields.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count(); i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : $"{lns.Join()},{field}";
                var dt = dtSource.SelectDistinct(tableName, distinctField);

                AddMoTa(dt, table_name, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            #region relation

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            #endregion

            return fr;
        }

        public FlexCelReport FillDataTable_NC_S(FlexCelReport fr,
            DataTable data,
            string fields = "X1 X2 X3 X4",
            string field = null)
        {
            var iNamLamViec = PhienLamViecViewModel.Current.iNamLamViec;
            var table_name = "SKT_MLNhuCau";
            List<string> lnsList = (fields).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("MoTa") || !data.GetColumnNames().Contains("STT"))
            {
                AddMoTa(data, table_name, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", fields.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count(); i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : $"{lns.Join()},{field}";
                var dt = dtSource.SelectDistinct(tableName, distinctField + ",STTBC");

                var view = dt.AsDataView();
                view.Sort = "STTBC";
                dt = view.ToTable();

                AddMoTa(dt, table_name, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            #region relation

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            #endregion

            return fr;
        }

        public FlexCelReport FillDataTable_SKT(FlexCelReport fr,
           DataTable data,
           string fields = "X1 X2 X3 X4",
           string field = null)
        {
            var iNamLamViec = PhienLamViecViewModel.Current.iNamLamViec;
            var table_name = "SKT_MLSKTNT";
            List<string> lnsList = (fields).Split(new char[] { ' ' }).ToList();

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);
            if (!data.GetColumnNames().Contains("MoTa") || !data.GetColumnNames().Contains("STT"))
            {
                AddMoTa(data, table_name, iNamLamViec);
            }

            var relations = new Dictionary<string, string>();
            relations.Add("ChiTiet", fields.Split(' ').Join());

            var dtSource = data;
            var lns = lnsList.ToList();
            for (int i = 0; i < lnsList.Count(); i++)
            {
                var tableName = "dt" + lns.Last().Split(new char[] { ',' })[0];
                var distinctField = string.IsNullOrWhiteSpace(field) ? lns.Join() : $"{lns.Join()},{field}";
                var dt = dtSource.SelectDistinct(tableName, distinctField);

                AddMoTa(dt, table_name, iNamLamViec);
                fr.AddTable(tableName, dt, TDisposeMode.DisposeAfterRun);

                relations.Add(tableName, distinctField);

                lns.Remove(lns.Last());
                dtSource = dt;
            }

            #region relation

            var items = relations.ToList();
            for (int i = items.Count() - 1; i > 0; i--)
            {
                var item1 = items[i];
                var item2 = items[i - 1];

                fr.AddRelationship(item1.Key, item2.Key, item1.Value.Split(','), item1.Value.Split(','));
            }

            #endregion

            return fr;
        }


        protected DataTable AddMoTa(DataTable dt, string tableName, string iNamLamViec = null)
        {
            var count = dt.Columns.Count;
            var columnNames = dt.GetColumnNames();

            var checkColumns = "STT,Nganh_Parent,Nganh,MoTa";
            checkColumns.Split(',').ToList()
                .ForEach(c =>
                {
                    if (!columnNames.Contains(c))
                    {
                        dt.Columns.Add(c);
                    }
                });


            var lns = "KyHieu".Split(',').ToList();

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

                    if (tableName == "SKT_MLNhuCau")
                    {
                        var entity = getMoTaNhuCau(iNamLamViec, key);
                        if (entity != null)
                        {
                            if (r.Field<string>("MoTa").IsEmpty())
                                r["MoTa"] = entity.MoTa;

                            if (r.Field<string>("Nganh_Parent").IsEmpty())
                                r["Nganh_Parent"] = entity.Nganh_Parent;

                            if (r.Field<string>("STT").IsEmpty())
                                r["STT"] = entity.STT;

                            if (r.Field<string>("Nganh").IsEmpty())
                                r["Nganh"] = entity.Nganh;
                        }
                    }
                    else if (tableName == "SKT_MLSKTNT")
                    {
                        var entity = getMoTaSKT(iNamLamViec, key);
                        if (entity != null)
                        {
                            if (r.Field<string>("MoTa").IsEmpty())
                                r["MoTa"] = entity.MoTa;

                            if (r.Field<string>("Nganh_Parent").IsEmpty())
                                r["Nganh_Parent"] = entity.Nganh_Parent;

                            if (r.Field<string>("STT").IsEmpty())
                                r["STT"] = entity.STT;

                            if (r.Field<string>("Nganh").IsEmpty())
                                r["Nganh"] = entity.Nganh;
                        }
                    }


                });
            return dt;
        }
        protected SKT_MLNhuCau getMoTaNhuCau(string iNamLamViec, string lns)
        {
            var sql = @"
                        SELECT      TOP(1) *
                        FROM        SKT_MLNhuCau
                        WHERE       NamLamViec=@iNamLamViec 
                                    AND KyHieu= @lns
                        ORDER BY    KyHieu

                        ";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {

                var result = conn.QueryFirstOrDefault<SKT_MLNhuCau>(
                    sql,
                    new
                    {
                        iNamLamViec,
                        lns
                    });
                return result;
            }

        }
        protected SKT_MLSKTNT getMoTaSKT(string iNamLamViec, string lns)
        {
            var sql = @"
                        SELECT      TOP(1) *
                        FROM        SKT_MLSKTNT
                        WHERE       NamLamViec=@iNamLamViec 
                                    AND KyHieu= @lns
                        ORDER BY    KyHieu

                        ";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {

                var result = conn.QueryFirstOrDefault<SKT_MLSKTNT>(
                    sql,
                    new
                    {
                        iNamLamViec,
                        lns
                    });
                return result;
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
        public DataRow GetCauHinhNamDuLieu(int namLamViec)
        {
            var loaiDL = $"select * from SKT_MapDataNS where NamLamViec = @NamLamViec";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.ExecuteDataTable(loaiDL,
                            new
                            {
                                NamLamViec = namLamViec,
                            },
                            CommandType.Text);

                dt.Columns.Add("UserInput", typeof(string));
                if (dt.Rows.Count == 0)
                {                   
                    var dtrMap = dt.NewRow();
                    dtrMap["NamNS_2"] = "QT_" + (namLamViec - 2);
                    dtrMap["NamNS_1"] = "QT_" + (namLamViec - 3);
                    dtrMap["QuyetDinh"] ="";
                    dtrMap["CVKHNS"] ="";
                    dt.Rows.Add(dtrMap);
                }
                return dt.Rows[0];
            }            
        }
        public Dictionary<int, string> GetTieuDeDuLieuNamTruoc(int namLamViec)
        {
            var result = new Dictionary<int, string>();
            var dtr = GetCauHinhNamDuLieu(namLamViec);
            var lstNT = new List<List<string>>();
            lstNT.Add(dtr["NamNS_1"].ToString().Split('_').ToList());
            lstNT.Add(dtr["NamNS_2"].ToString().Split('_').ToList());
            if (lstNT[0][0] == "QT")
            {
                result.Add(1, "Quyết toán năm " + lstNT[0][1]);
            }
            else
            {
                result.Add(1, "Dự toán đầu năm " + lstNT[0][1]);
            }
            if (lstNT[1][0] == "QT")
            {
                result.Add(2, "Quyết toán năm " + lstNT[1][1]);
            }
            else
            {
                result.Add(2, "Dự toán đầu năm " + lstNT[1][1]);
            }
            
            result.Add(3, "Số kiểm tra năm " + (namLamViec - 1));
            return result;
        }

        #endregion

        #region Mục lục số kiểm tra
        public Dictionary<string, string> UpdateParent(string table, string NamLamViec, string code, string Id = "")
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
                            $"select Id from {table} where NamLamViec = @NamLamViec and KyHieu = @code_parent order by KyHieu";
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
        public Dictionary<string, string> UpdateParentId(string table, string NamLamViec, string code, string code_parent)
        {
            var codes = code.Split('-');

            if (codes.Length == 1 || string.IsNullOrEmpty(code_parent))
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

                while (Id_Parent == "")
                {
                    var sql =
                            $"select Id from {table} where NamLamViec = @NamLamViec and KyHieu = @code_parent order by KyHieu";
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
                }

                var result = new Dictionary<string, string>()
                {
                    { "Id_Parent", Id_Parent },
                    { "IsParent", isParent },
                };
                return result;
            }
        }

        public string getIdMucLucNhuCau(string NamLamViec, string code)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var entity = conn.Get<SKT_MLNhuCau>(code);

                return entity.Id.ToString();
            }
        }
        #endregion

        #region Chứng từ số kiểm tra        
        public DataTable GetNganhAll(int nam, string username, string id_phongban = null, string id_donvi = null)
        {
            var user = "";
            var pb = _ngansachService.GetPhongBan(username);
            if (pb.sKyHieu != "02" && pb.sKyHieu != "11")
            {
                if (_ngansachService.GetUserRoleType(username) != (int)UserRoleType.TroLyPhongBan)
                {
                    user = $"%b{pb.sKyHieu}%";
                }
                else
                {
                    user = $"%{username}%";
                }
            }
            else
            {
                if (id_phongban != null && id_phongban != "undefined" && id_phongban != "" && id_phongban != "-1")
                    user = $"%b{id_phongban}%";
            }

            var sql = FileHelpers.GetSqlQuery("skt_dsnganhbd_user.sql");
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam,
                    user = user.ToParamString(),
                    id_donvi = id_donvi.ToParamString(),
                });

                var dt = cmd.GetTable();
                return dt;
            }
        }
        public DataTable GetDonViAll(int nam, string id_phongban = null, string id_donvi = null)
        {            
            var sql = FileHelpers.GetSqlQuery("skt_dsdonvi_user_pb.sql");
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam,
                    id_phongban = id_phongban.ToParamString(),
                    id_donvi = id_donvi.ToParamString(),
                });

                var dt = cmd.GetTable();
                return dt;
            }
        }
        public void DeleteChungTuCT(Guid id, string tablename)
        {
            var sql = "";
            if (tablename == "SKT_ChungTuChiTiet")
            {
                sql = FileHelpers.GetSqlQuery("skt_delete_chungtuct.sql");
            }
            else
            {
                sql = FileHelpers.GetSqlQuery("skt_delete_dacthuct.sql");
            }
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    id_chungtu = id,
                });
                conn.Open();
                var count = cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public void LockChungTu(Guid id, string tablename)
        {
            var sql = "";
            if (tablename == "SKT_ChungTu")
            {
                sql = FileHelpers.GetSqlQuery("skt_lock_chungtu.sql");
            }
            else
            {
                sql = FileHelpers.GetSqlQuery("skt_lock_dacthu.sql");
            }
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    id_chungtu = id,
                });
                conn.Open();
                var count = cmd.ExecuteNonQuery();
                conn.Close();
            }

        }
        #endregion

        #region Báo cáo
        public DataTable Get_DonVi_ExistData(
            string nam,
            int loai = 1,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_report_donviexist", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        loai,
                        donvis = donvis.ToParamString(),
                        phongban = id_phongban.ToParamString(),
                        phongbandich = id_phongbandich.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }
        public DataTable Get_Nganh_ExistData(string nam, int loai = 1, string donvis = null, string id_phongban = null, string id_phongbandich = null, string nganhs = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_report_nganhexist", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        loai,
                        donvis = donvis.ToParamString(),
                        nganhs = nganhs.ToParamString(),
                        phongban = id_phongban.ToParamString(),
                        phongbandich = id_phongbandich.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }
        public DataTable Get_DonVi_TheoBql_ExistData(
            string nam,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = FileHelpers.GetSqlQuery("skt_donvi.sql");
                var dt = conn.ExecuteDataTable(sql,
                    new
                    {
                        nam,
                        donvis = donvis.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_phongbandich = id_phongbandich.ToParamString(),
                    },
                    CommandType.Text);

                return dt;
            }
        }        
        public DataTable Get_DonViTG_TheoBql_ExistData(
            string nam,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_donvitg", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        donvis = donvis.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_phongbandich = id_phongbandich.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }
        public DataTable Get_DonVisDT_TheoBql_ExistData(
            string nam,
            string donvis = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_donvidt", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        donvis = donvis.ToParamString(),
                        id_phongbandich = id_phongbandich.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }            
        }
        public DataTable Get_DonVisBVNC_TheoBql_ExistData(
            string nam,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_donvibv", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        donvis = donvis.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_phongbandich = id_phongbandich.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }
        public DataTable Get_DonVisGBV_TheoBql_ExistData(
            string nam,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_donvigbv", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        donvis = donvis.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_phongbandich = id_phongbandich.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }
        public DataTable Get_Nganh_TheoBql_ExistData(
            string nam, 
            string nganh = null,
            string id_phongban = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_nganh", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        nganh = nganh.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }
        public DataTable Get_NganhBDKT_ExistData(
            string nam,
            string id_phongban = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_nganhbdkt", conn))
            {
                cmd.AddParams(
                    new
                    {
                        nam,
                        id_phongban = id_phongban.ToParamString(),
                    });
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }
        #endregion
        
        #region skt lock

        public SKT_Lock GetLockById(int nam, string id_donvi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"
                        select * from SKT_Lock
                        where   NamLamViec=@nam
                                and Id_DonVi=@id_donvi
                        ";
                var entity = conn.QueryFirstOrDefault<SKT_Lock>(sql, new { nam, id_donvi });

                return entity;
            }
        }

        public IEnumerable<SKT_Lock> GetAllLock(int nam)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = @"
select * from SKT_Lock
where   NamLamViec=@nam
        and iTrangThai=1
";
                var list = conn.Query<SKT_Lock>(sql, new { nam });
                return list;
            }
        }

        public bool CheckLock(int nam, string id_donvi, string id_phongban, string username)
        {
            var lockEntity = GetLockById(nam, id_donvi);
            var locked = lockEntity != null &&
                ((!string.IsNullOrWhiteSpace(lockEntity.Id_PhongBan) && lockEntity.Id_PhongBan.Split(',').Contains(id_phongban)
                    && string.IsNullOrWhiteSpace(lockEntity.Id_User)) ||
                 (!string.IsNullOrWhiteSpace(lockEntity.Id_User) && lockEntity.Id_User.Split(',').Contains(username)) ||
                 (!string.IsNullOrWhiteSpace(lockEntity.Id_PhongBan) && lockEntity.Id_PhongBan.Split(',').Contains(id_phongban)
                    && string.IsNullOrWhiteSpace(lockEntity.Id_User)) ||
                 (!string.IsNullOrWhiteSpace(lockEntity.Id_User) && lockEntity.Id_User.Split(',').Contains(username)));
            return locked;
        }

        public DataTable GetChungTu_Lock(int nam, int isreadonly, string iloai = null, string id_phongban = null, string id_user = null, string id_donvi = null)
        {
            var sql = FileHelpers.GetSqlQuery("skt_lockoropen_list.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.AddParams(new
                    {
                        nam,
                        isreadonly,
                        iloai = iloai.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_user = id_user.ToParamString(),
                        id_donvi = id_donvi.ToParamString(),
                    });
                    return cmd.GetTable();
                }
            }
        }
        public DataTable GetChungTu_BLock(int nam, int isreadonly, string id_phongban, string iloai = null)
        {
            var sql = FileHelpers.GetSqlQuery("skt_lockoropen_blist.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.AddParams(new
                    {
                        nam,
                        isreadonly,
                        id_phongban = id_phongban.ToParamString(),
                        iloai = iloai.ToParamString(),
                    });
                    return cmd.GetTable();
                }
            }
        }

        public string GetGhiChu(string username, string Ten)
        {
            #region definition input

            var sql = @"select	NoiDung 
                        from    SKT_GHICHU
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
                                        from    SKT_GHICHU
                                        where   Ten = @ten
                                                and UserCreator = @username
				                        )

				                        insert into SKT_GHICHU(Ten, NoiDung, UserCreator)
                                        values(@ten, @GhiChu, @username)

                        else

                                        update  SKT_GHICHU
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
