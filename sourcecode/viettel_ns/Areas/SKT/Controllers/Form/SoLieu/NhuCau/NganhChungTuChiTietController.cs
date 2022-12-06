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
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class NganhChungTuChiTietController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly ISKTService _sKTService = SKTService.Default;
        private int _dvt = 1000;

        // GET: Admin/MLNS
        public ActionResult Index(Guid id)
        {
            ChungTuDetailsViewModel chungtuViewModel = null;
            var isReadonly = false;
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_ChungTu>(id);
                chungtuViewModel = entity.MapTo<ChungTuDetailsViewModel>();

                isReadonly = _sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username) ||
                    (entity.Locked && !Username.ToLower().StartsWith("b")) || (!entity.Locked && entity.B_Locked && _ngansachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyTongHop);
            }
            var vm = new SKTNganhSheetViewModel()
            {
                Id = id,
                ChungTuViewModel = chungtuViewModel,
                Sheet = new SheetViewModel(isReadonly: isReadonly),
            };

            return View(vm);
        }

        public ActionResult SheetFrame(Guid id, string filters = null)
        {
            var isReadonly = false;
            var entity = new SKT_ChungTu();
            string excludeKyHieu = "";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                entity = conn.Get<SKT_ChungTu>(id);
                isReadonly = _sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username) ||
                    (entity.Locked && !Username.ToLower().StartsWith("b")) || (!entity.Locked && entity.B_Locked && _ngansachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyTongHop && !Username.ToLower().StartsWith("b"));                
            }

            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new SKTNganhSheetTable(Request.QueryString, entity, isReadonly, false) :
             new SKTNganhSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), entity, isReadonly, false);

            sheet.readonlyKyHieu = excludeKyHieu;
            sheet.FillSheet(getTable(id, sheet.Filters));

            var vm = new SKTNganhSheetViewModel
            {
                Id = id,
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id.ToString(),
                    filters: sheet.Filters,
                    urlPost: isReadonly ? "" : Url.Action("Save", "NganhChungTuChiTiet", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "NganhChungTuChiTiet", new { area = "SKT" }),
                    dvt: _dvt),
            };
            
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                {
                    {"F2" , "Thêm dòng" },
                    {"F10" , "Lưu" },
                    {"Del" , "Xóa" },
                };
            
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
                    var chungtu = conn.Get<SKT_ChungTu>(vm.Id);
                    var columns = new SKTNganhSheetTable(chungtu).Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id_mucluc = r.Id.Split('_')[0];
                        var id_chungtuchitiet = r.Id.Split('_')[1];

                        if (r.IsDeleted)
                        {
                            #region delete

                            if (!string.IsNullOrWhiteSpace(id_chungtuchitiet))
                            {
                                var entity = conn.Get<SKT_NganhChungTuChiTiet>(id_chungtuchitiet);
                                if (entity != null)
                                {
                                    conn.Delete(entity);
                                }
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

                                var mucluc = conn.Get<SKT_MLNhuCau>(id_mucluc);
                                var entity = new SKT_NganhChungTuChiTiet()
                                {
                                    Id = Guid.NewGuid(),
                                    Id_ChungTu = chungtu.Id,

                                    Id_MucLuc = mucluc.Id,

                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };

                                entity.MapFrom(changes, 1000);

                                if ((chungtu.iLoai == 5 && (entity.Tang != 0 || entity.Giam != 0) && !string.IsNullOrEmpty(entity.iID_MaDonVi) && !string.IsNullOrEmpty(entity.iID_MaPhongBanDich))
                                    || (chungtu.iLoai == 6 && (entity.MuaHang != 0 || entity.HangNhap != 0 || entity.PhanCap != 0) && !string.IsNullOrEmpty(entity.iID_MaDonVi) && !string.IsNullOrEmpty(entity.iID_MaPhongBanDich)))
                                    conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<SKT_NganhChungTuChiTiet>(id_chungtuchitiet);
                                entity.MapFrom(changes, 1000);

                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;

                                conn.Update(entity);

                                #endregion
                            }
                        }

                    });
                }
            }

            return RedirectToAction("SheetFrame", new { id = vm.Id, filters = vm.FiltersString });
        }

        #region private methods
        private DataTable getTable(Guid id_chungtu, Dictionary<string, string> filters)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_nganhctct", conn))
            {
                var chungTu = conn.Get<SKT_ChungTu>(id_chungtu);
                cmd.CommandType = CommandType.StoredProcedure;

                if (chungTu.Id_DonVi != "00") {
                    if (filters.ContainsKey("Nganh_Parent"))
                        filters["Nganh_Parent"] = chungTu.Id_DonVi;
                    else
                    {
                        filters.Add("Nganh_Parent", chungTu.Id_DonVi);
                    }
                } else
                {
                    if (filters.ContainsKey("Nganh"))
                        filters["Nganh"] = chungTu.Id_DonVi;
                    else
                    {
                        filters.Add("Nganh", chungTu.Id_DonVi);
                    }
                }

                cmd.AddParams(new
                {
                    id = id_chungtu.ToParamString(),
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
                    return dt;
                }
                catch (Exception ex)
                {
                    throw;
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
        #endregion
    }
}