using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Areas.DanhMuc.Models;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Controllers
{
    public class MappingNdcMLNSController : AppController
    {
        // GET: DanhMuc/MappingNdcMLNS
        public ActionResult Index()
        {
            var vm = new MappingNdcMlncViewModel()
            {
            };
            return View(vm);
        }

        public ActionResult SheetFrame(string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new MappingNdcMLNS_SheetTable(int.Parse(PhienLamViec.iNamLamViec), filters);
            var vm = new MappingNdcMlncViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "MappingNdcMLNS", new { area = "DanhMuc" }),
                   urlGet: Url.Action("SheetFrame", "MappingNdcMLNS", new { area = "DanhMuc" })
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
    }
}