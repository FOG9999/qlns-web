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
    public class QLDMChuDauTuController : AppController
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
            lstChuDauTuCha.Insert(0, new VDT_DM_ChuDauTu_ViewModel { ID = Guid.Empty, sTenChuDauTuCha = "--Chọn--" });
            ViewBag.ListChuDauTuCha = lstChuDauTuCha.ToSelectList("ID", "sTenChuDauTuCha");
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDT_DM_ChuDauTu_ViewModel data = _dmService.GetDanhMucChuDauTuDetailById(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool ChuDauTuDelete(Guid id)
        {
            return _dmService.DeleteChuDauTu(id, Username);
        }

        [HttpPost]
        public JsonResult ChuDauTuSave(DM_ChuDauTu data)
        {
            List<VDT_DM_ChuDauTu_ViewModel> lstChuDauTu = _dmService.GetListChuDauTuCha(data.ID, PhienLamViec.NamLamViec).ToList();
            var check = lstChuDauTu.FirstOrDefault(x => x.sId_CDT == data.sId_CDT && x.ID != data.ID);
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