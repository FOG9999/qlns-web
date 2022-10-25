using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;

namespace VIETTEL.Areas.QLNH.Controllers.DanhMucNgoaiHoi
{
    public class DanhMucPhanCapPheDuyetController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        // GET: QLVonDauTu/QLDMTyGia
        public ActionResult Index()
        {
            DanhmucNgoaiHoi_PhanCapPheDuyetModelPaging vm = new DanhmucNgoaiHoi_PhanCapPheDuyetModelPaging();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.getListPhanCapPheDuyetModels(ref vm._paging, null);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucPhanCapPheDuyetSearch(PagingInfo _paging, string sMa, string sTenVietTat, string sMoTa, string sTen)
        {
            DanhmucNgoaiHoi_PhanCapPheDuyetModelPaging vm = new DanhmucNgoaiHoi_PhanCapPheDuyetModelPaging();
            sMa = HttpUtility.HtmlDecode(sMa);
            sTenVietTat = HttpUtility.HtmlDecode(sTenVietTat);
            sMoTa = HttpUtility.HtmlDecode(sMoTa);
            sTen = HttpUtility.HtmlDecode(sTen);
            vm._paging = _paging;
            vm.Items = _qlnhService.getListPhanCapPheDuyetModels(ref vm._paging, sMa, sTenVietTat, sMoTa, sTen);
             
            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DanhmucNgoaiHoi_PhanCapPheDuyetModel data = new DanhmucNgoaiHoi_PhanCapPheDuyetModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetPhanCapPheDuyetById(id.Value);
                if (data == null) return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            DanhmucNgoaiHoi_PhanCapPheDuyetModel data = new DanhmucNgoaiHoi_PhanCapPheDuyetModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetPhanCapPheDuyetById(id.Value);
                if (data == null) return RedirectToAction("Index");
            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult PhanCapPheDuyetPopupDelete(Guid? id)
        {
            DanhmucNgoaiHoi_PhanCapPheDuyetModel data = new DanhmucNgoaiHoi_PhanCapPheDuyetModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetPhanCapPheDuyetById(id.Value);
                if (data == null) return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
            return PartialView("_modalDelete", data);
        }

        [HttpPost]
        public JsonResult PhanCapPheDuyetDelete(Guid id)
        {
            if (!_qlnhService.DeletePhanCapPheDuyet(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PhanCapPheDuyetSave(NH_DM_PhanCapPheDuyet data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sMa = HttpUtility.HtmlDecode(data.sMa);
            data.sTenVietTat = HttpUtility.HtmlDecode(data.sTenVietTat);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.sTen = HttpUtility.HtmlDecode(data.sTen);

            List<NH_DM_PhanCapPheDuyet> lstPhanCapPheDuyet = _qlnhService.GetNHDMPhanCapPheDuyetList(null).ToList();
            var checkExistMaPCPD = lstPhanCapPheDuyet.FirstOrDefault(x => x.sMa.ToUpper().Equals(data.sMa.ToUpper()) && x.ID != data.ID);
            if (checkExistMaPCPD != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Mã phân cấp phê duyệt đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            if (!_qlnhService.SavePhanCapPheDuyet(data, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}