using System.Web.Mvc;

namespace VIETTEL.Report_Controllers.CapPhat
{
    public class CapPhat_ReportController : Controller
    {
        public string sViewPath = "~/Views/Report_Views/CapPhat/";

        public ActionResult Index()
        {
            return View(sViewPath + "Report_Index.cshtml");
        }
    }
}
