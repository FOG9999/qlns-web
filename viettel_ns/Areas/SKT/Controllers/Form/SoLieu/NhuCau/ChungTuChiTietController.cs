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
    public class ChungTuChiTietController : AppController
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
                var exUser = "thuyb2,duongb2,tuyetb2,qanhb2,tungb2,trolyphongbanb2".Split(',');
                isReadonly = _sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username) ||
                    (entity.Locked && !Username.ToLower().StartsWith("b") && !exUser.Contains(Username)) || (!entity.Locked && entity.B_Locked && _ngansachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyTongHop);
            }
            var vm = new SKTSheetViewModel()
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
            DataRow dtrMap;
            string excludeKyHieu = "";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                entity = conn.Get<SKT_ChungTu>(id);
                var exUser = "thuyb2,duongb2,tuyetb2,qanhb2,tungb2,trolyphongbanb2".Split(',');
                isReadonly = _sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username) ||
                    (entity.Locked && !Username.ToLower().StartsWith("b") && !exUser.Contains(Username)) || (!entity.Locked && entity.B_Locked && _ngansachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyTongHop && !Username.ToLower().StartsWith("b"));
                dtrMap = _sKTService.GetCauHinhNamDuLieu(PhienLamViec.NamLamViec);
                if (entity.iLoai == 1 || entity.iLoai == 3 || entity.iLoai == 4)
                {
                    var cmd1 = new SqlCommand("select * from SKT_EXCLUDE where @id_phongban in (select * from f_split(Id_PhongBans)) and NamLamViec = @nam order by KyHieu", conn);
                    cmd1.AddParams(new
                    {
                        id_phongban = PhienLamViec.iID_MaPhongBan,
                        nam = PhienLamViec.iNamLamViec,
                    });

                    excludeKyHieu = cmd1.GetTable().AsEnumerable().Select(r => r.Field<string>("KyHieu")).Join();
                }
                dtrMap["UserInput"] = Username;
            }

            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new SKTSheetTable(Request.QueryString, entity, dtrMap, isReadonly, false) :
             new SKTSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), entity, dtrMap, isReadonly, false);

            sheet.readonlyKyHieu = excludeKyHieu;
            sheet.FillSheet(getTable(id, sheet.Filters));

            var vm = new SKTSheetViewModel
            {
                Id = id,
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id.ToString(),
                    filters: sheet.Filters,
                    urlPost: isReadonly ? "" : Url.Action("Save", "ChungTuChiTiet", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "ChungTuChiTiet", new { area = "SKT" }),
                    dvt: _dvt),
            };
            if (PhienLamViec.iID_MaPhongBan == "02")
            {
                vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                    {
                        {"F2" , "Copy số liệu đề nghị của bql sang!" },
                        {"F10" , "Lưu" },
                        {"Del" , "Xóa" },
                    };
            }
            else if (PhienLamViec.iID_MaPhongBan == "11")
            {
                vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                    {
                        {"F2" , "Copy số liệu đề nghị của bql sang!" },
                        {"F3" , "Copy số liệu đề nghị của B2 sang!" },
                        {"F10" , "Lưu" },
                        {"Del" , "Xóa" },
                    };
            }
            else
            {
                vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                    {
                        //{"F8" , "Copy số liệu dự toán đầu năm trước sang làm số đề nghị chi bằng tiền!" },
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
                    var chungtu = conn.Get<SKT_ChungTu>(vm.Id);
                    var loaiDL = $"select * from SKT_MapDataNS where NamLamViec = @NamLamViec";
                    var cmd1 = new SqlCommand(loaiDL, conn);
                    cmd1.AddParams(new
                    {
                        NamLamViec = chungtu.NamLamViec,
                    });
                    DataRow dtrMap = _sKTService.GetCauHinhNamDuLieu(PhienLamViec.NamLamViec);
                    dtrMap["UserInput"] = Username;

                    var columns = new SKTSheetTable(chungtu, dtrMap).Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id_mucluc = r.Id.Split('_')[0];
                        var id_chungtuchitiet = r.Id.Split('_')[1];

                        if (r.IsDeleted)
                        {
                            #region delete

                            if (!string.IsNullOrWhiteSpace(id_chungtuchitiet))
                            {
                                var entity = conn.Get<SKT_ChungTuChiTiet>(id_chungtuchitiet);
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
                                var entity = new SKT_ChungTuChiTiet()
                                {
                                    Id = Guid.NewGuid(),
                                    Id_ChungTu = chungtu.Id,

                                    Id_MucLuc = mucluc.Id,

                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };

                                entity.MapFrom(changes, 1000);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<SKT_ChungTuChiTiet>(id_chungtuchitiet);
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
            using (var cmd = new SqlCommand("sp_ncskt_ctct", conn))
            {
                var chungTu = conn.Get<SKT_ChungTu>(id_chungtu);
                cmd.CommandType = CommandType.StoredProcedure;                

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