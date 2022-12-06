using System;
using System.Web.Mvc;
using VIETTEL.Models;

namespace VIETTEL.Controllers.DuToan
{
    public class DuToanController : AppController
    {
        //
        // GET: /DuToan/
        public string sViewPath = "~/Views/DuToan/";
        [Authorize]
        public ActionResult Index()
        {
            return View(sViewPath + "DuToan_Index.aspx");
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(String ParentID)
        {
            int iNamLamViec = Convert.ToInt16(Request.Form[ParentID + "_MaNam"]);
            int iID_MaNamNganSach = Convert.ToInt16(Request.Form[ParentID + "_iID_MaNamNganSach"]);
            int iID_MaNguonNganSach = Convert.ToInt16(Request.Form[ParentID + "_iID_MaNguonNganSach"]);

            var ok = NguoiDungCauHinhModels.SuaCauHinh(User.Identity.Name, new { iNamLamViec = iNamLamViec, iID_MaNamNganSach = iID_MaNamNganSach, iID_MaNguonNganSach = iID_MaNguonNganSach });

            if (ok)
            {
                ChangePhienLamViec();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
