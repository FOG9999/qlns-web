using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Services;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.Component
{
    public class CommonController : Controller
    {
        private readonly IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        // GET: QLVonDauTu/Common
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult KiemTraTrung(string sTable, string sColumn, string val)
        {
            bool status = _iQLVonDauTuService.KiemTraTrung(sTable, sColumn, val);
            return Json(status, JsonRequestBehavior.AllowGet);
        }
    }
}