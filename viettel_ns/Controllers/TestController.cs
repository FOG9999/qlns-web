using System.Web.Mvc;
using Viettel.Services;

namespace VIETTEL.Controllers
{
    public class TestController : AppController
    {
        public TestController()
        {

        }

        public ActionResult Index()
        {
            var _duToanService = new DuToanReportService();
            //var dt = _duToanService.GetDonviListByLNS(Username, "1010000,1020000");

            //var dt = _duToanService.DT_rptDuToan_1010000_ChonTo(Username, "1010000", 1);

            return View();
        }

        public ActionResult Date()
        {
            return View();
        }


    }
}
