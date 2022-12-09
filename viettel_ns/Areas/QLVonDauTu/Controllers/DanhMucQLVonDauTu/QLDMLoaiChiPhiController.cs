using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.DanhMuc
{
    public class QLDMLoaiChiPhiController : VIETTEL.Controllers.AppController
    {
        private readonly IQLDMLoaiChiPhiService qLDMLoaiChiPhiService = QLDMLoaiChiPhiService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;

        // GET: QLVonDauTu/QLDMLoaiChiPhi
        public ActionResult Index()
        {
            DanhMucLoaiChiPhiViewModel vm = new DanhMucLoaiChiPhiViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _dmService.GetAllDanhMucLoaiChiPhi(ref vm._paging);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucLoaiChiPhiSearch(PagingInfo _paging, string sMaChiPhi, string sTenVietTat, string sTenChiPhi)
        {
            DanhMucLoaiChiPhiViewModel vm = new DanhMucLoaiChiPhiViewModel();
            vm._paging = _paging;
            vm.Items = _dmService.GetAllDanhMucLoaiChiPhi(ref vm._paging, sMaChiPhi, sTenVietTat, sTenChiPhi);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            VDT_DM_ChiPhi_ViewModel data = new VDT_DM_ChiPhi_ViewModel();
            if (id.HasValue)
            {
                data = _dmService.GetDanhMucLoaiChiPhiById(id.Value);
            }

            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDT_DM_ChiPhi_ViewModel data = _dmService.GetDanhMucLoaiChiPhiById(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool LoaiChiPhiDelete(Guid id)
        {
            return _dmService.DeleteLoaiChiPhi(id);
        }

        [HttpPost]
        public JsonResult LoaiChiPhiSave(VDT_DM_ChiPhi data)
        {
            if(data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không có dữ liệu. Vui lòng nhập dữ liệu !" }, JsonRequestBehavior.AllowGet);

            }
            else
            {

                if (_dmService.CheckExistMaLoaiChiPhi(data.sMaChiPhi,data.iID_ChiPhi))
                {
                    return Json(new { bIsComplete = false, sMessError = "Mã chi phí không được trùng nhau. Vui lòng nhập lại mã chi phí !" }, JsonRequestBehavior.AllowGet);

                }

                if (_dmService.CheckExistIthuTu(data.iThuTu,data.iID_ChiPhi))
                {
                    return Json(new { bIsComplete = false, sMessError = "Thứ tự không được trùng nhau. Vui lòng nhập lại thứ tự !" }, JsonRequestBehavior.AllowGet);

                }

                if (!_dmService.SaveLoaiChiPhi(data, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
            }


        }

    }
}