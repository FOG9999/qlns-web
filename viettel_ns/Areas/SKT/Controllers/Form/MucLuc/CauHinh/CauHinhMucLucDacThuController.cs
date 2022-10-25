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

    public class CauHinhMucLucDacThuController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        // GET: Admin/MLNS
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong)
            {
                var vm = new CauHinhMucLucDacThuViewModel()
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
            new CauHinhMucLucDacThuSheetTable(Request.QueryString, true) :
             new CauHinhMucLucDacThuSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(sheet.Filters));

            var vm = new CauHinhMucLucDacThuViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "CauHinhMucLucDacThu", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "CauHinhMucLucDacThu", new { area = "SKT" })
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
                    var columns = new CauHinhMucLucDacThuSheetTable().Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id_dacthu = r.Id.Split('_')[0];
                        
                        var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                        #region edit

                        var entity = conn.Get<SKT_MLDacThu>(id_dacthu);

                        if (r.Columns["DacThu"] == "Không phải đặc thù")
                        {
                            entity.DacThu = false;
                        } else
                        {
                            entity.DacThu = true;
                        }
                        entity.DateModified = DateTime.Now;
                        entity.UserModifier = Username;

                        conn.Update(entity);

                        #endregion

                    });
                }
            }
            return RedirectToAction("SheetFrame", new { filters = vm.FiltersString });
        }

        #region private methods
        private DataTable getTable(Dictionary<string, string> filters)
        {
            var sql = FileHelpers.GetSqlQuery("skt_cauhinhmuclucdacthusheet.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    NamLamViec = PhienLamViec.NamLamViec,
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
