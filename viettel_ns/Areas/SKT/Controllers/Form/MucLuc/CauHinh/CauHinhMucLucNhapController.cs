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

    public class CauHinhMucLucNhapController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        // GET: Admin/MLNS
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong)
            {
                var vm = new CauHinhMucLucNhapViewModel()
                {
                };

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult SheetFrame(string filters = null)
        {
            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new CauHinhMucLucNhapSheetTable(Request.QueryString, true) :
             new CauHinhMucLucNhapSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(sheet.Filters));

            var vm = new CauHinhMucLucNhapViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "CauHinhMucLucNhap", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "CauHinhMucLucNhap", new { area = "SKT" })
                    ),
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
                    var columns = new CauHinhMucLucNhapSheetTable().Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id_mapping = r.Id.Split('_')[0];
                        var id_mlnc = r.Id.Split('_')[1];

                        if (r.IsDeleted)
                        {
                            #region delete

                            if (!string.IsNullOrWhiteSpace(id_mapping))
                            {
                                var entity = conn.Get<SKT_EXCLUDE>(id_mapping);
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
                                if (r.Columns["Map"] != "Không khóa")
                                {
                                    var mucluc = conn.Get<SKT_MLNhuCau>(id_mlnc);
                                    var entity = new SKT_EXCLUDE()
                                    {
                                        Id = Guid.NewGuid(),
                                        NamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec),
                                        Id_MLNC = new Guid(id_mlnc),
                                        KyHieu = mucluc.KyHieu,
                                    };
                                    if (!r.Columns.ContainsKey("Id_PhongBans"))
                                        entity.Id_PhongBans = "";

                                    entity.MapFrom(changes);

                                    conn.Insert(entity);
                                }
                                #endregion
                            }
                            else
                            {
                                #region edit
                                if (r.Columns.ContainsKey("Map")) { 
                                    if (r.Columns["Map"] == "Không khóa")
                                    {
                                        var entity = conn.Get<SKT_EXCLUDE>(id_mapping);
                                        conn.Delete(entity);
                                    }
                                    else
                                    {
                                        var entity = conn.Get<SKT_EXCLUDE>(id_mapping);
                                        entity.MapFrom(changes);
                                        conn.Update(entity);
                                    }
                                } else
                                {
                                    var entity = conn.Get<SKT_EXCLUDE>(id_mapping);
                                    entity.MapFrom(changes);
                                    conn.Update(entity);
                                }
                                #endregion
                            }
                        }
                    });
                }
            }
            return RedirectToAction("SheetFrame", new { filters = vm.FiltersString });
        }

        #region private methods
        private DataTable getTable(Dictionary<string, string> filters)
        {
            var sql = FileHelpers.GetSqlQuery("skt_cauhinhmuclucnhapsheet.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
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
