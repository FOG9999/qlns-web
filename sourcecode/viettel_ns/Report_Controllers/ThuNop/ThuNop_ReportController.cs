using System.Web.Mvc;

namespace VIETTEL.Report_Controllers.ThuNop {
    public class ThuNop_ReportController : Controller
    {
        //
        // GET: /ThuNop_Report/
        public string sViewPath = "~/Views/Report_Views/ThuNop/";
        public ActionResult Index()
        {
            return View(sViewPath + "Report_Index.cshtml");
        }

    }
}
