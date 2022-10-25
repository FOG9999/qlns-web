using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.Admin.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.Admin.Controllers
{
    public class MLNSSheetViewModel
    {
        public MLNSSheetViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }

        public string Id { get; set; }

        public Dictionary<string, string> FilterOptions { get; set; }
    }

    public class MLNSController : AppController
    {
        // GET: Admin/MLNS
        public ActionResult Index()
        {
            var vm = new MLNSSheetViewModel()
            {

            };

            return View(vm);
        }


        public ActionResult SheetFrame()
        {
            //var sheet = new MLNSSheetTable(Request.QueryString);
            var sheet = new MLNSSheetTable(Request.QueryString, false, true);
            sheet.FillSheet(getTable(sheet.Filters));

            var vm = new MLNSSheetViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: "mlns",
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "MLNS", new { area = "Admin" }),
                    urlGet: Url.Action("SheetFrame", "MLNS", new { area = "Admin" })
                    ),

                //FilterOptions = option,
            };

            return View("_sheetFrame", vm.Sheet);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {

            var rows = vm.Rows.ToList();
            if (rows.Count > 0)
            {
                var columns = new MLNSSheetTable().Columns.Where(x => !x.IsReadonly);
                var ngansachService = NganSachService.Default;

                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    rows.ForEach(r =>
                    {
                        var id = r.Id;
                        if (r.IsDeleted)
                        {
                            #region delete

                            ngansachService.MLNS_Delete(Guid.Parse(id), conn);

                            #endregion
                        }
                        else
                        {
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var isNew = string.IsNullOrWhiteSpace(id);
                            if (isNew)
                            {
                                #region create

                                var entity = new NS_MucLucNganSach();
                                entity.MapFrom(changes);
                                entity.iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);
                                entity.sID_MaNguoiDungTao = Username;

                                ngansachService.MLNS_Update(entity, conn);
                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<NS_MucLucNganSach>(id);
                                entity.MapFrom(changes);
                                ngansachService.MLNS_Update(entity, conn);

                                #endregion
                            }
                        }

                    });
                }

                CacheService.Default.ClearStartsWith(getCacheKey());
            }

            // clear cache

            return RedirectToAction("SheetFrame", new { id = "", vm.Option });
        }

        #region private methods

        private DataTable getTable(Dictionary<string, string> filters)
        {

            return CacheService.Default.CachePerRequest(getCacheKey(),
                () => NganSachService.Default.MLNS_GetAll(PhienLamViec.iNamLamViec, filters),
                CacheTimes.OneMinute);
        }

        private string getCacheKey()
        {
            var cacheKey = $"{Username}_mlns_sheet";
            return cacheKey;
        }

        #endregion
    }
}
