using System;
using System.Web.Mvc;
using DomainModel.Abstract;
using VIETTEL.Models;
using Viettel.Services;
using DomainModel.Controls;
using System.Data;

namespace VIETTEL.Controllers.NguonNS
{
    public class ChungTuThu_ChungTuController : Controller
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;
        private readonly ISharedService _sharedService = SharedService.Default;

        public string sViewPath = "~/Views/NguonNS/ChungTuThu/ChungTu/";
        [Authorize]
        public ActionResult Index()
        {
            return View(sViewPath + "ChungTu_Index.aspx");
        }       

        [Authorize]
        public ActionResult Edit(string Id)
        {
            ViewData["Id_CTThu"] = Id;
            ViewData["DuLieuMoi"] = "0";

            return View(sViewPath + "ChungTu_Edit.aspx");
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(string ParentID, string Id_CTThu)
        {
            string MaND = User.Identity.Name;

            Bang bang = new Bang("Nguon_CTThu");                        
                       
            bang.TruyenGiaTri(ParentID, Request.Form);
            if (Request.Form[ParentID + "_DuLieuMoi"] == "1")
            {
                string Nam = ReportModels.LayNamLamViec(User.Identity.Name);
                int iSoChungTu = _nguonNSService.getSoChungTu("Nguon_CTThu", "thu", Nam);

                bang.CmdParams.Parameters.AddWithValue("@Nam", Nam);
                bang.CmdParams.Parameters.AddWithValue("@SoChungTu", iSoChungTu);             
                bang.CmdParams.Parameters.AddWithValue("@NguoiTao", User.Identity.Name);
                bang.CmdParams.Parameters["@NgayQD"].Value = Request.Form[ParentID + "_viNgayQD"].ToString();
                string MaChungTuAddNew = Convert.ToString(bang.Save());

                return RedirectToAction("Index", "ChungTuThu_ChungTuChiTiet", new { Id_CTThu = MaChungTuAddNew });
            }
            else
            {
                bang.GiaTriKhoa = Id_CTThu;
                bang.DuLieuMoi = false;
                bang.CmdParams.Parameters.AddWithValue("@NguoiSua", User.Identity.Name);    
                bang.CmdParams.Parameters["@NgayQD"].Value = Request.Form[ParentID + "_viNgayQD"].ToString();
                bang.CmdParams.Parameters.AddWithValue("@IpSua", Request.UserHostAddress);
                bang.CmdParams.Parameters.AddWithValue("@NgaySua", DateTime.Now);
                bang.Save();
                _nguonNSService.Update("Nguon_SLGOCThu", null, "Nguon_CTThu", Id_CTThu);
                return RedirectToAction("Index", "ChungTuThu_ChungTu");
            }            
        }
        public ActionResult Delete(string Id)
        {
            _nguonNSService.Delete_ChungTuChiTiet("Nguon_CTThu", "Id", Id, User.Identity.Name, Request.UserHostAddress);
            return RedirectToAction("Index", "ChungTuThu_ChungTu");
        }
    }
}
