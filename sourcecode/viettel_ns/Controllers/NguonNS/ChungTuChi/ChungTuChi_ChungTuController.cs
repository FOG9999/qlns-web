using System;
using System.Web.Mvc;
using DomainModel.Abstract;
using VIETTEL.Models;
using Viettel.Services;
using DomainModel.Controls;
using System.Data;

namespace VIETTEL.Controllers.NguonNS
{
    public class ChungTuChi_ChungTuController : Controller
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;
        private readonly ISharedService _sharedService = SharedService.Default;

        public string sViewPath = "~/Views/NguonNS/ChungTuChi/ChungTu/";
        [Authorize]
        public ActionResult Index()
        {
            return View(sViewPath + "ChungTu_Index.aspx");
        }
        
        [Authorize]
        public ActionResult Edit(string Id)
        {
            ViewData["Id_CTChi"] = Id;
            ViewData["DuLieuMoi"] = "0";

            return View(sViewPath + "ChungTu_Edit.aspx");
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(string ParentID, string Id_CTChi)
        {
            string MaND = User.Identity.Name;

            Bang bang = new Bang("Nguon_CTChi");

            bang.TruyenGiaTri(ParentID, Request.Form);
            if (Request.Form[ParentID + "_DuLieuMoi"] == "1")
            {
                string Nam = ReportModels.LayNamLamViec(User.Identity.Name);
                int iSoChungTu = _nguonNSService.getSoChungTu("Nguon_CTChi", "chi", Nam);

                bang.CmdParams.Parameters.AddWithValue("@Nam", Nam);
                bang.CmdParams.Parameters.AddWithValue("@SoChungTu", iSoChungTu);
                bang.CmdParams.Parameters.AddWithValue("@NguoiTao", User.Identity.Name);
                bang.CmdParams.Parameters["@NgayQD"].Value = Request.Form[ParentID + "_viNgayQD"].ToString();
                string MaChungTuAddNew = Convert.ToString(bang.Save());

                return RedirectToAction("Index", "ChungTuChi_ChungTuChiTiet", new { Id_CTChi = MaChungTuAddNew });
            }
            else
            {
                bang.GiaTriKhoa = Id_CTChi;
                bang.DuLieuMoi = false;
                bang.CmdParams.Parameters.AddWithValue("@NguoiSua", User.Identity.Name);
                bang.CmdParams.Parameters["@NgayQD"].Value = Request.Form[ParentID + "_viNgayQD"].ToString();
                bang.IPSua = Request.UserHostAddress;
                bang.Save();
                _nguonNSService.Update("Nguon_SLGOCChi", null, "Nguon_CTChi", Id_CTChi);
                return RedirectToAction("Index", "ChungTuChi_ChungTu");
            }
        }
        public ActionResult Delete(string Id)
        {
            _nguonNSService.Delete_ChungTuChiTiet("Nguon_CTChi", "Id", Id, User.Identity.Name, Request.UserHostAddress);
            return RedirectToAction("Index", "ChungTuChi_ChungTu");
        }
    }
}
