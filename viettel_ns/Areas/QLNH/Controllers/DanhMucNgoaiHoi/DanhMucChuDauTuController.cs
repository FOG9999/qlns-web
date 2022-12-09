using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLNH.Controllers.DanhMucNgoaiHoi
{
    public class DanhMucChuDauTuController : AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;

        // GET: QLVonDauTu/QLDMChuDauTu
        public ActionResult Index()
        {
            DanhMucChuDauTuViewModel vm = new DanhMucChuDauTuViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _dmService.GetAllDanhMucChuDauTu(ref vm._paging, PhienLamViec.NamLamViec);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucChuDauTuSearch(PagingInfo _paging, string sMaChuDauTu, string sTenChuDauTu)
        {
            DanhMucChuDauTuViewModel vm = new DanhMucChuDauTuViewModel();
            vm._paging = _paging;

            sMaChuDauTu = HttpUtility.HtmlDecode(sMaChuDauTu);
            sTenChuDauTu = HttpUtility.HtmlDecode(sTenChuDauTu);

            vm.Items = _dmService.GetAllDanhMucChuDauTu(ref vm._paging, PhienLamViec.NamLamViec, sMaChuDauTu, sTenChuDauTu);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            VDT_DM_ChuDauTu_ViewModel data = new VDT_DM_ChuDauTu_ViewModel();
            if (id.HasValue)
            {
                data = _dmService.GetDanhMucChuDauTuById(id.Value);
            }

            List<VDT_DM_ChuDauTu_ViewModel> lstChuDauTuCha = _dmService.GetListChuDauTuCha(id, PhienLamViec.NamLamViec).ToList();
            lstChuDauTuCha.Insert(0, new VDT_DM_ChuDauTu_ViewModel { ID = Guid.Empty, sTenCDT = "--Chọn--" });
            ViewBag.ListChuDauTuCha = lstChuDauTuCha.ToSelectList("ID", "sTenChuDauTuCha");
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            VDT_DM_ChuDauTu_ViewModel data = new VDT_DM_ChuDauTu_ViewModel();
            if (id.HasValue)
            {
                data = _dmService.GetDanhMucChuDauTuDetailById(id.Value);
                if (data == null)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult ChuDauTuDelete(Guid id)
        {
            if (!_dmService.DeleteChuDauTu(id, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChuDauTuSave(DM_ChuDauTu data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sId_CDT = HttpUtility.HtmlDecode(data.sId_CDT);
            data.sTenCDT = HttpUtility.HtmlDecode(data.sTenCDT);
            data.sKyHieu = HttpUtility.HtmlDecode(data.sKyHieu);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.sLoai = HttpUtility.HtmlDecode(data.sLoai);

            List<VDT_DM_ChuDauTu_ViewModel> lstChuDauTu = _dmService.GetListChuDauTuCha(data.ID, PhienLamViec.NamLamViec).ToList();
            var check = lstChuDauTu.FirstOrDefault(x => x.sId_CDT.ToUpper().Equals(data.sId_CDT.ToUpper()) && x.ID != data.ID);
            if (check != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Mã chủ đầu tư đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            if (!_dmService.SaveChuDauTu(data, PhienLamViec.NamLamViec, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}