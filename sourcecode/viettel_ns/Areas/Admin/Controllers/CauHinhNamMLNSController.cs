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
using VIETTEL.Areas.Admin.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.Admin.Controllers
{

    public class CauHinhNamMLNSController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        // GET: Admin/MLNS
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "02")
            {
                var vm = new CauHinhNamMLNSViewModel()
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
            new CauHinhNamMLNSSheetTable(Request.QueryString, true) :
             new CauHinhNamMLNSSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable_Cache(sheet.Filters));

            var vm = new CauHinhNamMLNSViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "CauHinhNamMLNS", new { area = "Admin" }),
                    urlGet: Url.Action("SheetFrame", "CauHinhNamMLNS", new { area = "Admin" })
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
                    var columns = new CauHinhNamMLNSSheetTable().Columns.Where(x => !x.IsReadonly);
                    var ngansachService = NganSachService.Default;
                    rows.ForEach(r =>
                    {
                        var id_chungtuchitiet = r.Id.Split('_')[0];

                        if (r.IsDeleted && !string.IsNullOrWhiteSpace(id_chungtuchitiet))
                        {
                            #region delete

                            var entity = conn.Get<NS_NamMLNS>(id_chungtuchitiet);
                            if (entity != null)
                            {
                                conn.Delete(entity);
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
                                var entity = new NS_NamMLNS()
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

                                var entity = conn.Get<NS_NamMLNS>(id_chungtuchitiet);
                                entity.MapFrom(changes);

                                conn.Update(entity);

                                #endregion
                            }
                            //conn.Open();
                        }

                    });
                }

                CacheService.Default.ClearStartsWith(getCacheKey());
            }

            // clear cache

            return RedirectToAction("SheetFrame", new { filters = vm.FiltersString });
        }

        #region private methods

        private DataTable getTable_Cache(Dictionary<string, string> filters)
        {
            var cachekey = getCacheKey() + filters.ToJson().GetHashCode();
            return CacheService.Default.CachePerRequest(cachekey,
                () => getTable(filters),
                CacheTimes.OneMinute);
        }

        private DataTable getTable(Dictionary<string, string> filters)
        {
            var sql = FileHelpers.GetSqlQuery("cauhinh_nammlnssheet.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });
                var dt = cmd.GetTable();

                return dt;
            }

        }

        private string getCacheKey()
        {
            var cacheKey = $"{Username}_cauhinh_nammlnssheet";
            return cacheKey;
        }

        #endregion
    }
}