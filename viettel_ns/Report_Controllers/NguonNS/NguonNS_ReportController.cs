using System.Web.Mvc;

namespace VIETTEL.Report_Controllers.NguonNS {
    public class NguonNS_ReportController : Controller
    {        
        public string sViewPath = "~/Report_Views/NguonNS/";
        public ActionResult Index()
        {
            return View(sViewPath + "Report_Index.aspx");
        }

    }
}
