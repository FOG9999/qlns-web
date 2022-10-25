using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.DanhMucQLVonDauTu
{
    public class QLDMNhaThauController : AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;

        // GET: QLVonDauTu/QLDMNhaThau
        public ActionResult Index()
        {
            DanhMucNhaThauViewModel vm = new DanhMucNhaThauViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _dmService.GetAllDanhMucNhaThau(ref vm._paging);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucNhaThauSearch(PagingInfo _paging, string sMaNhaThau, string sTenNhaThau, string sDaiDien, string sChucVu, string sDiaChi, string sDienThoai, string sFax, string sEmail, string sWebsite, string sSoTaiKhoan, string sNganHang, string sMaSoThue, string sNguoiLienHe, string sDienThoaiLienHe, string sMaNganHang)
        {
            DanhMucNhaThauViewModel vm = new DanhMucNhaThauViewModel();
            vm._paging = _paging;
            vm.Items = _dmService.GetAllDanhMucNhaThau(ref vm._paging, sMaNhaThau, sTenNhaThau, sDaiDien, sChucVu, sDiaChi, sDienThoai, sFax, sEmail, sWebsite, sSoTaiKhoan, sNganHang, sMaSoThue, sNguoiLienHe, sDienThoaiLienHe, sMaNganHang);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            VDT_DM_NhaThau_ViewModel data = new VDT_DM_NhaThau_ViewModel();
            if (id.HasValue)
            {
                data = _dmService.GetDanhMucNhaThauById(id.Value);
            }

            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDT_DM_NhaThau_ViewModel data = _dmService.GetDanhMucNhaThauById(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool NhaThauDelete(Guid id)
        {
            return _dmService.DeleteNhaThau(id);
        }

        [HttpPost]
        public JsonResult NhaThauSave(VDT_DM_NhaThau data)
        {
            if (!_dmService.SaveNhaThau(data))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}