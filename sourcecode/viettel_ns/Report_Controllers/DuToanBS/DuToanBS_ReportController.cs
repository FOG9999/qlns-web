using System.Web.Mvc;

namespace VIETTEL.Report_Controllers.QuyetToan
{
    public class DuToanBS_ReportController : Controller
    {
        // GET: /DuToanBS_Report/
        public string sViewPath = "~/Views/Report_Views/DuToanBS/";
        public ActionResult Index()
        {
            return View(sViewPath + "Report_Index.cshtml");
        }
    }
}
