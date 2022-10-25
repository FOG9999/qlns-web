using System;
using System.Web.Mvc;

namespace VIETTEL.Controllers.ThuNop {
    public class ThuNop_DuLieuDaNhapController : Controller
    {
        //
        // GET: /TN_DuLieuDaNhap/
        public string sViewPath = "~/Views/ThuNop/DuLieuDaNhap/";
        [Authorize]
        public ActionResult Index()
        {
            return View(sViewPath + "ThuNop_DuLieuDaNhap_Index.aspx");
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SearchSubmit(String ParentID)
        {       
            return RedirectToAction("Index", "ThuNop_DuLieuDaNhap", new {});
        }        
    }
}
