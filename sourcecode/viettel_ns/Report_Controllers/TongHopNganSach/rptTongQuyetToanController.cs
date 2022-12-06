using System.Web.Mvc;

namespace VIETTEL.Controllers
{
    public class rptTongQuyetToanController : AppController
    {
        //
        // GET: /Report/
        public ActionResult Index()
        {
            string view = "~/Views/Report_Views/TongHopNganSach/Index_TongQuyetToan.cshtml";
            return View(view);
        }
    }
}
