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
    public class QLDMDonViThucHienDuAnController : AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;

        // GET: QLVonDauTu/QLDMDonViThucHienDuAn
        public ActionResult Index()
        {
            DanhMucDonViThucHienDuAnViewModel vm = new DanhMucDonViThucHienDuAnViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _dmService.GetAllDanhMucDonViThucHienDuAn(ref vm._paging);
            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucDonViThucHienDuAnSearch(PagingInfo _paging, string sMaDonViTHDA, string sTenDonViTHDA)
        {
            DanhMucDonViThucHienDuAnViewModel vm = new DanhMucDonViThucHienDuAnViewModel();
            vm._paging = _paging;
            vm.Items = _dmService.GetAllDanhMucDonViThucHienDuAn(ref vm._paging, sMaDonViTHDA, sTenDonViTHDA);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            VDT_DM_DonViThucHienDuAn_ViewModel data = new VDT_DM_DonViThucHienDuAn_ViewModel();
            if (id.HasValue)
            {
                data = _dmService.GetDanhMucDonViThucHienDuAnById(id.Value);
            }

            List<VDT_DM_DonViThucHienDuAn> lstDonViCha = _dmService.GetListDonViThucHienDuAnCha(id).ToList();
            lstDonViCha.Insert(0, new VDT_DM_DonViThucHienDuAn { iID_DonVi = Guid.Empty, sTenDonVi = "--Chọn--" });
            ViewBag.ListDonViCha = lstDonViCha.ToSelectList("iID_DonVi", "sTenDonVi");
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDT_DM_DonViThucHienDuAn_ViewModel data = _dmService.GetDanhMucDonViThucHienDuAnById(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool DonViThucHienDuAnDelete(Guid id)
        {
            return _dmService.DeleteDonViThucHienDuAn(id);
        }

        [HttpPost]
        public JsonResult DonViThucHienDuAnSave(VDT_DM_DonViThucHienDuAn data)
        {
            if (!_dmService.SaveDonViThucHienDuAn(data))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}