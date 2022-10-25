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
using VIETTEL.Areas.DuToanKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Controllers
{

    public class MucLucController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly IDuToanKTService _dutoanKTService = DuToanKTService.Default;

        // GET: Admin/MLNS
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11"
                || PhienLamViec.iID_MaPhongBan == "02")
            {
                var vm = new DuToanKTMucLucViewModel()
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
            new MucLucSheetTable(Request.QueryString, true) :
             new MucLucSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable_Cache(sheet.Filters));

            var vm = new DuToanKTMucLucViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "MucLuc", new { area = "DuToanKT" }),
                    urlGet: Url.Action("SheetFrame", "MucLuc", new { area = "DuToanKT" })
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
                    var columns = new MucLucSheetTable().Columns.Where(x => !x.IsReadonly);
                    var ngansachService = NganSachService.Default;
                    rows.ForEach(r =>
                    {
                        var id_mucluc = r.Id.Split('_')[0];

                        if (r.IsDeleted && !string.IsNullOrWhiteSpace(id_mucluc))
                        {
                            #region delete

                            var entity = conn.Get<DTKT_MucLuc>(id_mucluc);
                            if (entity != null)
                            {
                                entity.iTrangThai = 0;
                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;
                                entity.IpModified = Request.UserHostAddress;

                                conn.Update(entity);
                            }

                            #endregion
                        }
                        else
                        {
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var isNew = string.IsNullOrWhiteSpace(id_mucluc);
                            if (isNew)
                            {
                                #region create

                                var entity = new DTKT_MucLuc()
                                {
                                    Id = Guid.NewGuid(),
                                    NamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec),

                                    iTrangThai = 1,
                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };

                                entity.MapFrom(changes);
                                entity.Id_Parent = new Guid(_dutoanKTService.UpdateParent(PhienLamViec.iNamLamViec, entity.Code)["Id_Parent"]);
                                entity.IsParent = Convert.ToBoolean(_dutoanKTService.UpdateParent(PhienLamViec.iNamLamViec, entity.Code)["IsParent"]);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<DTKT_MucLuc>(id_mucluc);
                                entity.MapFrom(changes);
                                entity.Id_Parent = new Guid(_dutoanKTService.UpdateParent(PhienLamViec.iNamLamViec, entity.Code, entity.Id.ToString())["Id_Parent"]);
                                entity.IsParent = Convert.ToBoolean(_dutoanKTService.UpdateParent(PhienLamViec.iNamLamViec, entity.Code, entity.Id.ToString())["IsParent"]);

                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;
                                entity.IpModified = Request.UserHostAddress;

                                conn.Update(entity);

                                #endregion
                            }
                            conn.Open();
                            var sql_update = FileHelpers.GetSqlQuery("dtkt_mucluc_update.sql");
                            using (var cmd = new SqlCommand(sql_update, conn))
                            {
                                cmd.AddParams(new
                                {
                                    nam = PhienLamViec.iNamLamViec,
                                });

                                var result = cmd.ExecuteNonQuery();
                            }
                            conn.Close();
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
            //var cachekey = getCacheKey() + filters.ToJson().GetHashCode();
            //return CacheService.Default.CachePerRequest(cachekey,
            //    () => getTable(filters),
            //    CacheTimes.OneMinute);


            var dt = getTable(filters);
            return dt;
        }

        private DataTable getTable(Dictionary<string, string> filters)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_mlsheet.sql");

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

        private string getCacheKey()
        {
            var cacheKey = $"{Username}_dtkt_mlsheet";
            return cacheKey;
        }

        #endregion
    }
}
