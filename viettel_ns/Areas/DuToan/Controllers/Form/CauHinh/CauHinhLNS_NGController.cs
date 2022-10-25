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
using VIETTEL.Areas.DuToan.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{

    public class CauHinhLNS_NGController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        // GET: Admin/MLNS
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong)
            {
                var vm = new CauHinhLNS_NGViewModel()
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
            new CauHinhLNS_NGSheetTable(Request.QueryString, true) :
             new CauHinhLNS_NGSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(sheet.Filters));

            var vm = new CauHinhLNS_NGViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "CauHinhLNS_NG", new { area = "DuToan" }),
                    urlGet: Url.Action("SheetFrame", "CauHinhLNS_NG", new { area = "DuToan" })
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
                    var columns = new CauHinhLNS_NGSheetTable().Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id_chungtuchitiet = r.Id.Split('_')[0];

                        if (r.IsDeleted)
                        {
                            #region delete

                            if (!string.IsNullOrWhiteSpace(id_chungtuchitiet))
                            {
                                var entity = conn.Get<NS_MucLucNganSach_Lns_Ng>(id_chungtuchitiet);
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
                                var entity = new NS_MucLucNganSach_Lns_Ng()
                                {
                                    Id = Guid.NewGuid(),
                                };

                                entity.MapFrom(changes);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<NS_MucLucNganSach_Lns_Ng>(id_chungtuchitiet);
                                entity.MapFrom(changes);

                                conn.Update(entity);

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
            var sql = FileHelpers.GetSqlQuery("dt_cauhinhlns_ngsheet.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });

                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);

                var dt = cmd.GetTable();

                return dt;
            }

        }
        #endregion
    }
}
