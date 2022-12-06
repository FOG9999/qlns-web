using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIETTEL.Areas.QuyetToan.Controllers
{
    public class HomeController : Controller
    {
        // GET: QuyetToan/Home
        public ActionResult Index()
        {
            return View();
        }
        // GET: QuyetToan/Home
        public ActionResult BaoCao()
        {
            return View();
        }

    }
}