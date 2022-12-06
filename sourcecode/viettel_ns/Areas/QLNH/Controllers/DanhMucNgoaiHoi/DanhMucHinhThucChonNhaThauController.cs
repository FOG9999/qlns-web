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
    public class DanhMucHinhThucChonNhaThauController : AppController
    {
        // GET: QLNH/DanhMucHinhThucChonNhaThau
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        public ActionResult Index()
        {
            DanhMucNgoaiHoi_DanhMucHinhThucChonNhaThauModelPaging vm = new DanhMucNgoaiHoi_DanhMucHinhThucChonNhaThauModelPaging();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListDanhMucHinhThucChonNhaThauPaging(ref vm._paging);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucHinhThucChonNhaThauSearch(PagingInfo _paging, string sMaHinhThuc, string sTenVietTat, string sTenHinhThuc, string sMoTa)
        {
            DanhMucNgoaiHoi_DanhMucHinhThucChonNhaThauModelPaging vm = new DanhMucNgoaiHoi_DanhMucHinhThucChonNhaThauModelPaging();
            vm._paging = _paging;

            sMaHinhThuc = HttpUtility.HtmlDecode(sMaHinhThuc);
            sTenVietTat = HttpUtility.HtmlDecode(sTenVietTat);
            sTenHinhThuc = HttpUtility.HtmlDecode(sTenHinhThuc);
            sMoTa = HttpUtility.HtmlDecode(sMoTa);
            vm.Items = _qlnhService.GetListDanhMucHinhThucChonNhaThauPaging(ref vm._paging, sMaHinhThuc, sTenVietTat, sTenHinhThuc, sMoTa);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DanhmucNgoaiHoi_HinhThucChonNhaThauModel data = new DanhmucNgoaiHoi_HinhThucChonNhaThauModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucHinhThucChonNhaThauById(id.Value);
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
            DanhmucNgoaiHoi_HinhThucChonNhaThauModel data = new DanhmucNgoaiHoi_HinhThucChonNhaThauModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucHinhThucChonNhaThauById(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }


        [HttpPost]
        public JsonResult HinhThucChonNhaThauDelete(Guid id)
        {
            if (!_qlnhService.DeleteDanhMucHinhThucChonNhaThau(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult HinhThucChonNhaThauSave(NH_DM_HinhThucChonNhaThau data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            data.sMaHinhThuc = HttpUtility.HtmlDecode(data.sMaHinhThuc);
            data.sTenVietTat = HttpUtility.HtmlDecode(data.sTenVietTat);
            data.sTenHinhThuc = HttpUtility.HtmlDecode(data.sTenHinhThuc);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.sNguoiTao = Username;

            List<NH_DM_HinhThucChonNhaThau> lstHinhThucChonNhaThau = _qlnhService.GetNHDMHinhThucChonNhaThauList(null).ToList();
            var checkExistHinhThucChonNhaThau = lstHinhThucChonNhaThau.FirstOrDefault(x => x.sMaHinhThuc.ToUpper().Equals(data.sMaHinhThuc.ToUpper()) && x.ID != data.ID);
            if (checkExistHinhThucChonNhaThau != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Mã hình thức chọn nhà thầu đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            NH_DM_HinhThucChonNhaThau hinhThucChonNhaThau = lstHinhThucChonNhaThau.FirstOrDefault();
            int? soThuTu = 0;
            if (hinhThucChonNhaThau != null) soThuTu = hinhThucChonNhaThau.iThuTu;
            if (!_qlnhService.SaveHinhThucChonNhaThau(data, soThuTu))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}