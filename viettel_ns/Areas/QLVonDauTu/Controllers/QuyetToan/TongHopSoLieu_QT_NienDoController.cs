using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.QuyetToan;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class TongHopSoLieu_QT_NienDoController : AppController
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        // GET: QLVonDauTu/TongHopSoLieu_QT_NienDo
        public ActionResult Index()
        {
            var listDonVi = _vdtService.GetListDataDonVi(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");
            var listNguonVon = _vdtService.GetListDataNguonNganSach();
            ViewBag.ListNguonVon = listNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            var result = new VDT_QT_TongHopSoLieuPagingModel();
            result._paging.CurrentPage = 1;
            result.lstData = _vdtService.GetAllTongHopSoLieuPaging(ref result._paging, null, null, null);
            return View(result);
        }

        [HttpPost]
        public ActionResult GetListView(PagingInfo _paging, Guid? iID_DonViQuanLy, int? iID_NguonVon, int? iNamKeHoach)
        {
            var listDonVi = _vdtService.GetListDataDonVi(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");
            var listNguonVon = _vdtService.GetListDataNguonNganSach();
            ViewBag.ListNguonVon = listNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            var result = new VDT_QT_TongHopSoLieuPagingModel();
            result._paging = _paging;
            result.lstData = _vdtService.GetAllTongHopSoLieuPaging(ref result._paging, iID_DonViQuanLy, iID_NguonVon, iNamKeHoach);
            return PartialView("_list", result);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var listDonVi = _vdtService.GetListDataDonVi(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");
            var listNguonVon = _vdtService.GetListDataNguonNganSach();
            ViewBag.ListNguonVon = listNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            return View();
        }

        [HttpPost]
        public JsonResult GetDataDonViQuanLy()
        {
            var result = new List<dynamic>();
            result.Add(new { id = "CHON_TAT_CA", text = "--Chọn--" });
            var dt = NganSach_HamChungModels.DSDonViCuaNguoiDung(Username);
            if (dt == null || dt.Rows == null)
            {
                return Json(new { status = true, data = result });
            }

            foreach (DataRow item in dt.Rows)
            {
                if (item["iID_Ma"] == null || item["sTen"] == null)
                {
                    continue;
                }
                var id = item["iID_Ma"];
                var text = item["sTen"];
                result.Add(new { id = id, text = text });
            }

            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetDataNguonVon()
        {
            var result = new List<dynamic>();
            result.Add(new { id = "CHON_TAT_CA", text = "--Chọn--" });

            var listModel = _vdtService.GetListDataNguonNganSach();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new { id = item.iID_MaNguonNganSach, text = item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        [HttpGet]
        public ActionResult Update(Guid? id)
        {
            var listDonVi = _vdtService.GetListDataDonVi(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");
            var listNguonVon = _vdtService.GetListDataNguonNganSach();
            ViewBag.ListNguonVon = listNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            var model = _vdtService.GetTongHopSoLieuById(id.Value);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetChiTiet(string id)
        {
            var result = new List<VDT_QT_TongHopSoLieu_ChiTiet>();

            result = _vdtService.GetListDataTongHopSoLieuChiTiet(id);

            return Json(new { status = true, data = result });
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            var listDonVi = _vdtService.GetListDataDonVi(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");
            var listNguonVon = _vdtService.GetListDataNguonNganSach();
            ViewBag.ListNguonVon = listNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            var model = _vdtService.GetDetailTongHopSoLieu(id);
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            var result = _vdtService.DeleteTongHopSoLieu(id);
            return Json(new { status = result });
        }

        [HttpPost]
        public JsonResult ChangeStatus(Guid? id, string typeChange)
        {
            var result = _vdtService.ChangeStatusTongHopSoLieu(id, typeChange);

            return Json(new { status = result });
        }

        [HttpPost]
        public JsonResult TongHopSoLieu(int iNamThucHien, Guid? iID_DonViQuanLy, int? iID_NguonVon, DateTime? dNgayLap)
        {
            var result = _vdtService.TongHopSoLieu(iNamThucHien, iID_DonViQuanLy, iID_NguonVon, dNgayLap);
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult Save(Guid? iID_TongHopSoLieu, int? iNamThucHien, Guid? iID_DonViQuanLy, int? iID_NguonVon, DateTime? dNgayLap, string typeSave)
        {
            if (iID_TongHopSoLieu.HasValue)
            {
                var model = _vdtService.GetTongHopSoLieuById(iID_TongHopSoLieu.Value);
                if(model != null)
                {
                    if(model.bIsCanBoDuyet != null && model.bIsCanBoDuyet == true)
                    {
                        return Json(new { status = false, message = "Số quyết toán niên độ đã được cán bộ phê duyệt. Bạn không được phép chỉnh sửa!" });
                    }
                }
            }

            var result = _vdtService.TongHopSoLieuSave(iID_TongHopSoLieu, iNamThucHien, iID_DonViQuanLy, iID_NguonVon, dNgayLap, typeSave, Username);
            return Json(new { status = result });
        }
    }
}