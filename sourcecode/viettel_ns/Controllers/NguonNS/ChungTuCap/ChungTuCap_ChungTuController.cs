using System;
using System.Web.Mvc;
using DomainModel.Abstract;
using VIETTEL.Models;
using Viettel.Services;
using DomainModel.Controls;
using System.Data;

namespace VIETTEL.Controllers.NguonNS
{
    public class ChungTuCap_ChungTuController : Controller
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;
        private readonly ISharedService _sharedService = SharedService.Default;

        public string sViewPath = "~/Views/NguonNS/ChungTuCap/ChungTu/";
        [Authorize]
        public ActionResult Index()
        {
            return View(sViewPath + "ChungTu_Index.aspx");
        }
        
        [Authorize]
        public ActionResult Edit(string Id)
        {
            ViewData["Id_CTCap"] = Id;
            ViewData["DuLieuMoi"] = "0";

            return View(sViewPath + "ChungTu_Edit.aspx");
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(string ParentID, string Id_CTCap)
        {
            string MaND = User.Identity.Name;

            Bang bang = new Bang("Nguon_CTChi");

            bang.TruyenGiaTri(ParentID, Request.Form);
            if (Request.Form[ParentID + "_DuLieuMoi"] == "1")
            {
                string Nam = ReportModels.LayNamLamViec(User.Identity.Name);
                int iSoChungTu = _nguonNSService.getSoChungTu("Nguon_CTChi", "cap", Nam);

                bang.CmdParams.Parameters.AddWithValue("@Nam", Nam);
                bang.CmdParams.Parameters.AddWithValue("@SoChungTu", iSoChungTu);
                bang.CmdParams.Parameters.AddWithValue("@NguoiTao", User.Identity.Name);
                bang.CmdParams.Parameters["@NgayQD"].Value = Request.Form[ParentID + "_viNgayQD"].ToString();
                string MaChungTuAddNew = Convert.ToString(bang.Save());

                return RedirectToAction("Index", "ChungTuCap_ChungTuChiTiet", new { Id_CTCap = MaChungTuAddNew });
            }
            else
            {
                bang.GiaTriKhoa = Id_CTCap;
                bang.DuLieuMoi = false;
                bang.CmdParams.Parameters.AddWithValue("@NguoiSua", User.Identity.Name);
                bang.CmdParams.Parameters["@NgayQD"].Value = Request.Form[ParentID + "_viNgayQD"].ToString();
                bang.IPSua = Request.UserHostAddress;
                bang.Save();
                _nguonNSService.Update("Nguon_SLGOCChi", null, "Nguon_CTChi", Id_CTCap);
                return RedirectToAction("Index", "ChungTuCap_ChungTu");
            }
        }
        public ActionResult Delete(string Id)
        {
            _nguonNSService.Delete_ChungTuChiTiet("Nguon_CTChi", "Id", Id, User.Identity.Name, Request.UserHostAddress);
            return RedirectToAction("Index", "ChungTuCap_ChungTu");
        }
    }
}
