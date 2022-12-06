using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLNH.Controllers.DanhMucNgoaiHoi
{
    public class DanhMucNoiDungChiController : AppController
    {
        // GET: QLNH/DanhMucNoiDungChi
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        public ActionResult Index()
        {
            NHDMNoiDungChiViewModel vm = new NHDMNoiDungChiViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListDanhMucNoiDungChiModels(ref vm._paging);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucNoiDungChiSearch(PagingInfo _paging, string sMaNoiDungChi, string sTenNoiDungChi, string sMoTa)
        {
            NHDMNoiDungChiViewModel vm = new NHDMNoiDungChiViewModel();
            vm._paging = _paging;

            sMaNoiDungChi = HttpUtility.HtmlDecode(sMaNoiDungChi);
            sTenNoiDungChi = HttpUtility.HtmlDecode(sTenNoiDungChi);
            sMoTa = HttpUtility.HtmlDecode(sMoTa);
            vm.Items = _qlnhService.GetListDanhMucNoiDungChiModels(ref vm._paging, sMaNoiDungChi, sTenNoiDungChi, sMoTa);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DanhmucNgoaiHoi_NoiDungChiModel data = new DanhmucNgoaiHoi_NoiDungChiModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucNoiDungChiById(id.Value);
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
        public ActionResult GetModal(Guid? id)
        {

            DanhmucNgoaiHoi_NoiDungChiModel data = new DanhmucNgoaiHoi_NoiDungChiModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucNoiDungChiById(id.Value);
                if (data == null) return RedirectToAction("Index");
            }
            return PartialView("_modalUpdate", data);
        }


        [HttpPost]
        public JsonResult NoiDungChiDelete(Guid id)
        {
            if (!_qlnhService.DeleteDanhMucNoiDungChi(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NoiDungChiSave(NH_DM_NoiDungChi data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sMaNoiDungChi = HttpUtility.HtmlDecode(data.sMaNoiDungChi);
            data.sTenNoiDungChi = HttpUtility.HtmlDecode(data.sTenNoiDungChi);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.sNguoiTao = Username;

            List<NH_DM_NoiDungChi> lstNoiDungChi = _qlnhService.GetNHDMNoiDungChiList(null).ToList();
            var checkExistMaNoiDungChi = lstNoiDungChi.FirstOrDefault(x => x.sMaNoiDungChi.ToUpper().Equals(data.sMaNoiDungChi.ToUpper()) && x.ID != data.ID);
            if (checkExistMaNoiDungChi != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Mã nội dung chi đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            if (!_qlnhService.SaveNoiDungChi(data))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}