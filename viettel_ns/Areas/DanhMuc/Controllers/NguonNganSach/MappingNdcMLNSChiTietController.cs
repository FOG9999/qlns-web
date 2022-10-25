using DapperExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.DanhMuc.Models;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Controllers
{
    public class MappingNdcMLNSChiTietController : AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        // GET: DanhMuc/MappingNdcMLNSChiTiet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewMappingNdcMLNSChiTiet(Guid iID_NoiDungChi, string sMaNoiDungChi, string sTenNoiDungChi)
        {
            var vm = new MappingNdcMlncViewModel()
            {
                iID_NoiDungChi = iID_NoiDungChi,
                sMaNoiDungChi = sMaNoiDungChi,
                sTenNoiDungChi = sTenNoiDungChi
            };
            return View(vm);
        }

        public ActionResult SheetFrame(string id, string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new MappingNdcMLNSChiTiet_SheetTable(Guid.Parse(id), int.Parse(PhienLamViec.iNamLamViec), filters);
            var vm = new MappingNdcMlncViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "MappingNdcMLNSChiTiet", new { area = "DanhMuc" }),
                   urlGet: Url.Action("SheetFrame", "MappingNdcMLNSChiTiet", new { area = "DanhMuc" })
                   ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                {
                    //{"F2" , "Chọn tất cả" },
                    //{"F3" , "Bỏ chọn tất cả" },
                    //{"F10" , "Lưu" },
                    //{"Del" , "Xóa" },
                };
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                int iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);
                Guid iID_NoiDungChi = Guid.Parse(vm.Id);

                conn.Open();
                var rows = vm.Rows.Where(x => !x.IsParent).ToList();
                var rowsInsert = vm.Rows.Where(x => int.Parse(x.Columns["isMap"]) == 1).ToList();
                var rowsDelete = vm.Rows.Where(x => (x.Id.ToList("_", true))[1].IsNotEmpty()).ToList();

                List<MappingNdcMLNSModel> dataDelete = new List<MappingNdcMLNSModel>();
                List<MappingNdcMLNSModel> dataSaveNew = new List<MappingNdcMLNSModel>();
                rowsDelete.ForEach(r =>
                {
                    var valuesId = r.Id.ToList("_", true);
                    var iID_IdMap = Guid.Parse(valuesId[1]);
                    dataDelete.Add(new MappingNdcMLNSModel() { Id = iID_IdMap });
                });

                rowsInsert.ForEach(r =>
                {
                    var valuesId = r.Id.ToList("_", true);
                    Guid iID_MaMucLucNganSach = Guid.Parse(valuesId[0]);
                    dataSaveNew.Add(new MappingNdcMLNSModel() { iID_MaMucLucNganSach = iID_MaMucLucNganSach });
                });

                IEnumerable<MappingNdcMLNSModel> dataIenumDelete = dataDelete.AsEnumerable();
                IEnumerable<MappingNdcMLNSModel> dataIenumNew = dataSaveNew.AsEnumerable();
                bool isIns = _dmService.InsertAllNNSNdcMLNS(iID_NoiDungChi, dataIenumDelete, dataIenumNew, Username, int.Parse(PhienLamViec.iNamLamViec));
            }

            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }
    }
}