using System.Web.Mvc;

namespace VIETTEL.Controllers
{
    public class rptTongHopNSController : AppController
    {
        //
        // GET: /Report/
        public ActionResult Index()
        {
            string view = "~/Views/Report_Views/TongHopNganSach/Index.cshtml";
            return View(view);
        }
    }
}
