using AutoMapper.Extensions;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Services;
using VIETTEL.Areas.DuToanKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class ChungTuChiTietController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private int _dvt = 1000;

        // GET: Admin/MLNS
        public ActionResult Index(Guid id)
        {
            ChungTuDetailsViewModel chungtuViewModel = null;
            var isReadonly = false;
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<DTKT_ChungTu>(id);
                chungtuViewModel = entity.MapTo<ChungTuDetailsViewModel>();

                isReadonly = entity.UserCreator != Username ||
                    _duToanKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username, entity.iRequest.ToString()) ||
                    entity.IsReadOnly;
            }
            var vm = new DTKTSheetViewModel()
            {
                Id = id,
                ChungTuViewModel = chungtuViewModel,
                Sheet = new SheetViewModel(isReadonly: isReadonly),
            };

            return View(vm);
        }

        public ActionResult SheetFrame(Guid id, string filters = null)
        {
            int loai = 1;
            int iRequest = 0;
            var isReadonly = false;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var entity = conn.Get<DTKT_ChungTu>(id);
                loai = entity.iLoai;
                iRequest = entity.iRequest;
                isReadonly = entity.UserCreator != Username ||
                    _duToanKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username, entity.iRequest.ToString()) ||
                    entity.IsReadOnly;
            }

            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new DTKTSheetTable(Request.QueryString, loai, iRequest, isReadonly, false) :
             new DTKTSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), loai, iRequest, isReadonly, false);

            sheet.FillSheet(getTable_Cache(id, sheet.Filters));

            var vm = new DTKTSheetViewModel
            {
                Id = id,
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id.ToString(),
                    filters: sheet.Filters,
                    urlPost: isReadonly ? "" : Url.Action("Save", "ChungTuChiTiet", new { area = "DuToanKT" }),
                    urlGet: Url.Action("SheetFrame", "ChungTuChiTiet", new { area = "DuToanKT" }),
                    dvt: _dvt),
            };

            if (iRequest == 1)
            {
                vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                {
                    {"F10" , "Lưu" },
                    {"Del" , "Xóa" },
                };
            }
            else if (iRequest == 2)
            {
                vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                {
                    {"F10" , "Lưu" },
                    {"Del" , "Xóa" },
                };
            }


            return View("_sheetFrame", vm.Sheet);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {

                    var chungtu = conn.Get<DTKT_ChungTu>(vm.Id);

                    var columns = new DTKTSheetTable(chungtu.iLoai, chungtu.iRequest).Columns.Where(x => !x.IsReadonly);
                    var ngansachService = NganSachService.Default;
                    rows.ForEach(r =>
                    {
                        var id_mucluc = r.Id.Split('_')[0];
                        var id_chungtuchitiet = r.Id.Split('_')[1];

                        if (r.IsDeleted && !string.IsNullOrWhiteSpace(id_chungtuchitiet))
                        {
                            #region delete

                            //conn.Delete(Guid.Parse(id), conn);
                            var entity = conn.Get<DTKT_ChungTuChiTiet>(id_chungtuchitiet);
                            if (entity != null)
                            {
                                entity.iTrangThai = 0;
                                entity.DateModified = DateTime.Now;
                                entity.UserCreator = Username;

                                conn.Update(entity);
                            }

                            #endregion
                        }
                        else
                        {
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var isNew = string.IsNullOrWhiteSpace(id_chungtuchitiet);
                            if (isNew)
                            {
                                #region create

                                var mucluc = conn.Get<DTKT_MucLuc>(id_mucluc);
                                var entity = new DTKT_ChungTuChiTiet()
                                {
                                    Id = Guid.NewGuid(),
                                    Id_ChungTu = chungtu.Id,
                                    Id_PhongBan = chungtu.Id_PhongBan,
                                    Id_PhongBanDich = chungtu.Id_PhongBanDich,
                                    Id_DonVi = chungtu.Id_DonVi,
                                    iLoai = chungtu.iLoai,
                                    iRequest = chungtu.iRequest,
                                    iLan = chungtu.iLan,
                                    NamLamViec = chungtu.NamLamViec,

                                    // mucluc
                                    Id_Mucluc = mucluc.Id,
                                    Id_Mucluc_Parent = mucluc.Id_Parent,
                                    Code = mucluc.Code,
                                    Ng = mucluc.Ng,
                                    Nganh = mucluc.Nganh,
                                    sMoTa = mucluc.sMoTa,

                                    iTrangThai = 1,
                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };

                                if (chungtu.iRequest == 1)
                                {
                                    entity.TuChi = 0;
                                    entity.DacThu = 0;
                                    entity.HangMua = 0;
                                    entity.DacThu_HM = 0;
                                    entity.HangNhap = 0;
                                    entity.DacThu_HN = 0;
                                }
                                entity.MapFrom(changes, 1000);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<DTKT_ChungTuChiTiet>(id_chungtuchitiet);
                                entity.MapFrom(changes, 1000);

                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;

                                conn.Update(entity);

                                #endregion
                            }
                        }

                    });
                }

                CacheService.Default.ClearStartsWith(getCacheKey());
            }

            // clear cache

            return RedirectToAction("SheetFrame", new { id = vm.Id, filters = vm.FiltersString });
        }

        #region private methods

        private DataTable getTable_Cache(Guid id_chungtu, Dictionary<string, string> filters)
        {
            //#if DEBUG
            //return getTable(id_chungtu, filters, type);
            //#endif

            var cachekey = getCacheKey(id_chungtu.ToString()) + filters.ToJson().GetHashCode();
            return CacheService.Default.CachePerRequest(cachekey,
                () => getTable(id_chungtu, filters),
                CacheTimes.OneMinute);
        }

        private DataTable getTable(Guid id_chungtu, Dictionary<string, string> filters)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_sheet.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                var chungTu = conn.Get<DTKT_ChungTu>(id_chungtu);
                if (chungTu.iLoai == 2)
                {
                    if (filters.ContainsKey("Ng"))
                        filters["Ng"] = chungTu.Id_DonVi;
                    else
                    {
                        filters.Add("Ng", chungTu.Id_DonVi);
                    }
                }

                cmd.AddParams(new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    loai = chungTu.iLoai,
                    id_chungtu,
                    //nganh = nganh.ToParamString(),
                    byNg = (chungTu.iLoai == 2 ? chungTu.Id_DonVi : "").ToParamString(),
                    dvt = _dvt,
                });

                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });
                try
                {
                    var dt = cmd.GetTable();
                    filterParentsRow(dt);
                    addColumnValueByUser(id_chungtu, dt, chungTu.iRequest, 1000);

                    return dt;
                }
                catch (Exception ex)
                {

                    throw;
                }


            }

        }

        private void addColumnValueByUser(Guid id_chungtu, DataTable dt, int value, int rate = 1)
        {
            if (value != 1)
            {
                return;
            }

            var sql = FileHelpers.GetSqlQuery("dtkt_requestvalue.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                var chungTu = conn.Get<DTKT_ChungTu>(id_chungtu);

                cmd.AddParams(new
                {
                    rate,
                    NamLamViec = PhienLamViec.iNamLamViec,
                    Id_PhongBanDich = chungTu.Id_PhongBanDich,
                    chungTu.iLoai,
                    chungTu.Id_DonVi,
                    dvt = _dvt,
                    byNg = (chungTu.iLoai == 2 ? chungTu.Id_DonVi : "").ToParamString(),
                });

                var dtValues = cmd.GetTable();

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var colName = dt.Columns[i].ColumnName;
                    if (colName == "TuChi" || colName == "DacThu" ||
                        colName == "HangNhap" || colName == "DacThu_HN" ||
                        colName == "HangMua" || colName == "DacThu_HM")
                    {
                        dt.AsEnumerable()
                           .ToList()
                           .ForEach(r =>
                           {
                               var values = dtValues.AsEnumerable()
                                       .ToList()
                                       .Where(x => x.Field<string>("Code") == r.Field<string>("Code"))
                                       .Sum(x => x.Field<double>(colName));
                               r[colName] = values;
                           });
                    }
                }
            }
        }

        protected virtual void filterParentsRow(DataTable dt, string checkedColumn = "IsChecked")
        {
            // trong truong hop cache, hoac da check thi ko check lai nua
            var checkColumn = new DataColumn()
            {
                DefaultValue = false,
                ColumnName = checkedColumn,
                DataType = typeof(bool)
            };
            dt.Columns.Add(checkColumn);

            var isParentColumn = "IsParent";
            var idColumn = "Id";
            var idParentColumn = "Id_Parent";

            var items = new List<DataRow>();
            // loc cac hang la cha nhung ko co nhanh
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var row = dt.Rows[i];
                var isParent = row.Field<bool>(isParentColumn);
                if (i == dt.Rows.Count - 1 && isParent)
                {
                    items.Add(row);
                }
                else
                {
                    for (int j = i; j < dt.Rows.Count; j++)
                    {
                        var rowChild = dt.Rows[j];
                        if (!rowChild.Field<bool>(isParentColumn))
                        {
                            rowChild[checkedColumn] = true;
                        }

                        if (row[idColumn].ToString() == rowChild[idParentColumn].ToString() && rowChild.Field<bool>(checkedColumn))
                        {
                            row[checkedColumn] = true;
                            break;
                        }

                    }

                }
            }

            items = dt.AsEnumerable().Where(x => !x.Field<bool>(checkedColumn)).ToList();
            items.ForEach(dt.Rows.Remove);

            dt.Columns.Remove(checkColumn);
        }

        private string getCacheKey(string id = null)
        {
            var cacheKey = $"{Username}_dtkt_sheet_{id}";
            return cacheKey;
        }

        #endregion
    }
}
