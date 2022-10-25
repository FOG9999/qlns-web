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
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class MappingSKTChiTietController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        // GET: Admin/MLNS
        public ActionResult Index(Guid id, int loai)
        {
            SKT_MLNhuCau sktViewModel = null;
            using (var conn = _connectionFactory.GetConnection())
            {
                sktViewModel = conn.Get<SKT_MLNhuCau>(id);
            }
            var vm = new MappingSKTChiTietViewModel()
            {
                Id = id,
                SKTViewModel = sktViewModel,
                Loai = loai,
            };
            return View(vm);
          
        }

        public ActionResult SheetFrame(Guid id, int loai = 1, string filters = null)
        {
            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new MappingSKTChiTietSheetTable(Request.QueryString, true) :
             new MappingSKTChiTietSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(id, loai, sheet.Filters));

            var vm = new MappingSKTChiTietViewModel
            {
                Id = id,
                Loai = loai,
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "MappingSKTChiTiet", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "MappingSKTChiTiet", new { area = "SKT" , id = id, loai = loai })
                    ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                {
                    {"F2" , "Chọn tất cả" },
                    {"F3" , "Bỏ chọn tất cả" },
                    {"F10" , "Lưu" },
                    {"Del" , "Xóa" },
                };
            return View("_sheetFrame", vm.Sheet);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var rows = vm.Rows.ToList();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    var columns = new MappingSKTChiTietSheetTable().Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id_mapping = r.Id.Split('_')[0];
                        var id_mlskt = r.Id.Split('_')[1];

                        if (r.IsDeleted)
                        {
                            #region delete
                            if (!string.IsNullOrWhiteSpace(id_mapping))
                            {
                                var entity = conn.Get<SKT_NCSKT>(id_mapping);
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
                            var isNew = string.IsNullOrWhiteSpace(id_mapping);
                            if (isNew)
                            {
                                #region create
                                if (r.Columns["Map"] != "Không chọn")
                                {
                                    var entity = new SKT_NCSKT()
                                    {
                                        Id = Guid.NewGuid(),
                                        NamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec),
                                        Id_MLSKTNT = new Guid(id_mlskt),
                                        KyHieu_SKT = conn.Get<SKT_MLSKTNT>(id_mlskt).KyHieu,
                                        Id_MLNhuCau = new Guid(vm.Id),
                                        KyHieu = conn.Get<SKT_MLNhuCau>(vm.Id).KyHieu,

                                        DateCreated = DateTime.Now,
                                        UserCreator = Username,
                                    };

                                    entity.MapFrom(changes);

                                    conn.Insert(entity);
                                }
                                #endregion
                            }
                            else
                            {
                                #region edit
                                if (r.Columns["Map"] == "Không chọn") { 
                                    var entity = conn.Get<SKT_NCSKT>(id_mapping);
                                    conn.Delete(entity);
                                }
                                #endregion
                            }
                        }

                    });
                }
            }

            return RedirectToAction("SheetFrame", new { id = vm.Id, loai = 1, filters = vm.FiltersString });
        }

        #region private methods

        private DataTable getTable(Guid id, int loai, Dictionary<string, string> filters)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_mapncmskt_chitietsheet", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParams(new
                {
                    Id_MLNhuCau = id,
                    NamLamViec = PhienLamViec.iNamLamViec,
                    loai,
                });

                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });
                var dt = cmd.GetTable();

                return dt;
            }

        }
        #endregion
    }
}
