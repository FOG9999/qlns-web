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
    public class DanhMucLoaiHopDongController : AppController

    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        // GET: QLNH/DanhMucLoaiHopDong
        public ActionResult Index()
        {
            DanhmucNgoaiHoi_LoaiHopDongModelPaging vm = new DanhmucNgoaiHoi_LoaiHopDongModelPaging();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.getListLoaiHopDongModels(ref vm._paging);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucLoaiHopDongSearch(PagingInfo _paging, string sMaLoaiHopDong, string sTenVietTat, string sTenLoaiHopDong, string sMoTa)
        {
            DanhmucNgoaiHoi_LoaiHopDongModelPaging vm = new DanhmucNgoaiHoi_LoaiHopDongModelPaging();
            vm._paging = _paging;

            sMaLoaiHopDong = HttpUtility.HtmlDecode(sMaLoaiHopDong);
            sTenVietTat = HttpUtility.HtmlDecode(sTenVietTat);
            sTenLoaiHopDong = HttpUtility.HtmlDecode(sTenLoaiHopDong);
            sMoTa = HttpUtility.HtmlDecode(sMoTa);
            vm.Items = _qlnhService.getListLoaiHopDongModels(ref vm._paging, sMaLoaiHopDong, sTenVietTat, sTenLoaiHopDong, sMoTa);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DanhmucNgoaiHoi_LoaiHopDongModel data = new DanhmucNgoaiHoi_LoaiHopDongModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetLoaiHopDongById(id.Value);
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
            DanhmucNgoaiHoi_LoaiHopDongModel data = new DanhmucNgoaiHoi_LoaiHopDongModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetLoaiHopDongById(id.Value);
                if (data == null) return RedirectToAction("Index");
            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult OpenModalDelete(Guid? id)
        {
            DanhmucNgoaiHoi_LoaiHopDongModel data = new DanhmucNgoaiHoi_LoaiHopDongModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetLoaiHopDongById(id.Value);
                if (data == null) return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
            return PartialView("_modalDelete", data);
        }
       
        [HttpPost]
        public JsonResult LoaiHopDongDelete(Guid id)
        {
            if (!_qlnhService.DeleteLoaiHopDong(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult LoaiHopDongSave(NH_DM_LoaiHopDong data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sMaLoaiHopDong = HttpUtility.HtmlDecode(data.sMaLoaiHopDong);
            data.sTenVietTat = HttpUtility.HtmlDecode(data.sTenVietTat);
            data.sTenLoaiHopDong = HttpUtility.HtmlDecode(data.sTenLoaiHopDong);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            if (data.ID == null || data.ID == Guid.Empty)
            {
                data.dNgayTao = DateTime.Now;
                data.sNguoiTao = Username;
            }
            List<NH_DM_LoaiHopDong> lstLoaiHopDong = _qlnhService.GetNHDMLoaiHopDongList(null).ToList();
            var checkExistMaLoaiHopDong = lstLoaiHopDong.FirstOrDefault(x => x.sMaLoaiHopDong.ToUpper().Equals(data.sMaLoaiHopDong.ToUpper()) && x.ID != data.ID);
            if (checkExistMaLoaiHopDong != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Mã loại hợp đồng đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            if (!_qlnhService.SaveLoaiHopDong(data))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }

}