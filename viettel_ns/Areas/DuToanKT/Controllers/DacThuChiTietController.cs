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

    public class DacThuChiTietController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly IDuToanKTService _dutoanKTService = DuToanKTService.Default;
        private int _dvt = 1000;

        // GET: Admin/MLNS
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "02")
            {
                var vm = new DuToanKTDacThuChiTietViewModel()
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
            new DacThuChiTietSheetTable(Request.QueryString, true) :
             new DacThuChiTietSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable_Cache(sheet.Filters));

            var vm = new DuToanKTDacThuChiTietViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "DacThuChiTiet", new { area = "DuToanKT" }),
                    urlGet: Url.Action("SheetFrame", "DacThuChiTiet", new { area = "DuToanKT" })
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
                    var columns = new DacThuChiTietSheetTable().Columns.Where(x => !x.IsReadonly);
                    var ngansachService = NganSachService.Default;
                    rows.ForEach(r =>
                    {
                        var id_mucluc = r.Id.Split('_')[1];
                        var id_chungtuchitiet = r.Id.Split('_')[0];

                        if (r.IsDeleted && !string.IsNullOrWhiteSpace(id_chungtuchitiet))
                        {
                            #region delete

                            var entity = conn.Get<DTKT_DacThuChiTiet>(id_chungtuchitiet);
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
                            var isNew = string.IsNullOrWhiteSpace(id_chungtuchitiet);
                            if (isNew)
                            {
                                #region create
                                var mucluc = conn.Get<NS_MucLucNganSach>(id_mucluc);
                                var entity = new DTKT_DacThuChiTiet()
                                {
                                    Id = Guid.NewGuid(),
                                    NamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec),

                                    M = mucluc.sM,
                                    Tm = mucluc.sTM,
                                    TTm = mucluc.sTTM,
                                    Ng = mucluc.sNG,
                                    MoTa = mucluc.sMoTa,

                                    iTrangThai = 1,
                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };

                                entity.MapFrom(changes, _dvt);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<DTKT_DacThuChiTiet>(id_chungtuchitiet);
                                entity.MapFrom(changes, _dvt);

                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;
                                entity.IpModified = Request.UserHostAddress;

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
            var sql = FileHelpers.GetSqlQuery("dtkt_dacthuchitietsheet.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    dvt = _dvt,
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
            var cacheKey = $"{Username}_dtkt_dacthuchitietsheet";
            return cacheKey;
        }

        #endregion
    }
}
