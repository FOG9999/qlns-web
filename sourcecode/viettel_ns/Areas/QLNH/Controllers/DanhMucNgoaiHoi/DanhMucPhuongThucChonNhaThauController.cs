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
    public class DanhMucPhuongThucChonNhaThauController : AppController
    {
        // GET: QLNH/DanhMucPhuongThucChonNhaThau
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        public ActionResult Index()
        {
            DanhMucNgoaiHoi_DanhMucPhuongThucChonNhaThauModelPaging vm = new DanhMucNgoaiHoi_DanhMucPhuongThucChonNhaThauModelPaging();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListDanhMucPhuongThucChonNhaThauPaging(ref vm._paging, null);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucPhuongThucChonNhaThauSearch(PagingInfo _paging, string sMaPhuongThuc, string sTenVietTat,string sTenPhuongThuc, string sMoTa)
        {
            DanhMucNgoaiHoi_DanhMucPhuongThucChonNhaThauModelPaging vm = new DanhMucNgoaiHoi_DanhMucPhuongThucChonNhaThauModelPaging();
            vm._paging = _paging;

            sMaPhuongThuc = HttpUtility.HtmlDecode(sMaPhuongThuc);
            sTenVietTat = HttpUtility.HtmlDecode(sTenVietTat);
            sTenPhuongThuc = HttpUtility.HtmlDecode(sTenPhuongThuc);
            sMoTa = HttpUtility.HtmlDecode(sMoTa);
            vm.Items = _qlnhService.GetListDanhMucPhuongThucChonNhaThauPaging(ref vm._paging, sMaPhuongThuc, sTenVietTat,sTenPhuongThuc, sMoTa);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DanhmucNgoaiHoi_PhuongThucChonNhaThauModel data = new DanhmucNgoaiHoi_PhuongThucChonNhaThauModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucPhuongThucChonNhaThauById(id.Value);
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
            DanhmucNgoaiHoi_PhuongThucChonNhaThauModel data = new DanhmucNgoaiHoi_PhuongThucChonNhaThauModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucPhuongThucChonNhaThauById(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }


        [HttpPost]
        public JsonResult PhuongThucChonNhaThauDelete(Guid id)
        {
            if (!_qlnhService.DeleteDanhMucPhuongThucChonNhaThau(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PhuongThucChonNhaThauSave(NH_DM_PhuongThucChonNhaThau data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            data.sMaPhuongThuc = HttpUtility.HtmlDecode(data.sMaPhuongThuc);
            data.sTenVietTat = HttpUtility.HtmlDecode(data.sTenVietTat);
            data.sTenPhuongThuc = HttpUtility.HtmlDecode(data.sTenPhuongThuc);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.sNguoiTao = Username;

            List<NH_DM_PhuongThucChonNhaThau> lstPhuongThucChonNhaThau = _qlnhService.GetNHDMPhuongThucChonNhaThauList(null).ToList();
            var checkExistPhuongThucChonNhaThau = lstPhuongThucChonNhaThau.FirstOrDefault(x => x.sMaPhuongThuc.ToUpper().Equals(data.sMaPhuongThuc.ToUpper()) && x.ID != data.ID);
            if (checkExistPhuongThucChonNhaThau != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Mã phương thức chọn nhà thầu đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            NH_DM_PhuongThucChonNhaThau phuongThucChonNhaThau = lstPhuongThucChonNhaThau.FirstOrDefault();
            int? soThuTu = 0;
            if (phuongThucChonNhaThau != null) soThuTu = phuongThucChonNhaThau.iThuTu;

            if (!_qlnhService.SavePhuongThucChonNhaThau(data, soThuTu))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}