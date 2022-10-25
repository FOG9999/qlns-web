using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using Viettel.Models.QLNH;
using System.Web;

namespace VIETTEL.Areas.QLNH.Controllers.DanhMucNgoaiHoi
{
    public class DanhMucTienTeController: AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;

        // GET: QLNH/DanhMucTienTe
        public ActionResult Index()
        {
            DMDonViTienTeViewModel vm = new DMDonViTienTeViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _dmService.GetAllDanhMucDonViTienTe(ref vm._paging);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucTienTeSearch(PagingInfo _paging, string sMaTienTe = "", string sTenTienTe = "", string sMoTaChiTiet = "")
        {
            DMDonViTienTeViewModel vm = new DMDonViTienTeViewModel();
            sMaTienTe = HttpUtility.HtmlDecode(sMaTienTe);
            sTenTienTe = HttpUtility.HtmlDecode(sTenTienTe);
            sMoTaChiTiet = HttpUtility.HtmlDecode(sMoTaChiTiet);
            vm._paging = _paging;
            vm.Items = _dmService.GetAllDanhMucDonViTienTe(ref vm._paging, sMaTienTe, sTenTienTe, sMoTaChiTiet);
            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            NH_DM_LoaiTienTeModel data = new NH_DM_LoaiTienTeModel();
            if (id.HasValue)
            {
                data = _dmService.GetDanhMucDonViTienTeByID(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            NH_DM_LoaiTienTeModel data = _dmService.GetDanhMucDonViTienTeByID(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult TienTeDelete(Guid id)
        {
            if (!_dmService.DeleteDonViTienTe(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TienTeSave(NH_DM_LoaiTienTe data)
        {
            data.sMaTienTe = HttpUtility.HtmlDecode(data.sMaTienTe);
            data.sTenTienTe = HttpUtility.HtmlDecode(data.sTenTienTe);
            data.sMoTaChiTiet = HttpUtility.HtmlDecode(data.sMoTaChiTiet);
            List<NH_DM_LoaiTienTe> lstTienTe = _dmService.GetListTienTe(data.ID).ToList();
            var checkExistMaTienTe = lstTienTe.FirstOrDefault(x => x.sMaTienTe.ToUpper().Equals(data.sMaTienTe.ToUpper()) && x.ID != data.ID);
            if (checkExistMaTienTe != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Đơn vị tiền tệ đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            if (!_dmService.SaveDonViTienTe(data))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}