using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.QLVonDauTu.Model.QuyetToan;
using VIETTEL.Common;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class QuyetToanNienDoTroLyController : AppController
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private QLKeHoachVonNamModel _modelKHV = new QLKeHoachVonNamModel();
        private QuyetToanNienDoTroLyModel _model = new QuyetToanNienDoTroLyModel();
        private GiaiNganThanhToanModel _modelGNTT = new GiaiNganThanhToanModel();
        

        // GET: QLVonDauTu/QuyetToanNienDoTroLy
        public ActionResult Index()
        {
            string sMaNguoiDung = Username;
            var drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            drpDonViQuanLy.Insert(0, new SelectListItem() { Text = Constants.TAT_CA });
            ViewBag.drpDonViQuanLy = drpDonViQuanLy;

            var drpNguonVon = _modelKHV.GetDataDropdownNguonNganSach();
            drpNguonVon.Insert(0, new SelectListItem() { Text = Constants.TAT_CA });
            ViewBag.drpNguonVon = drpNguonVon;

            QuyetToanNienDoTroLyPagingModel data = new QuyetToanNienDoTroLyPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _vdtService.GetAllQuyetToanNienDoPaging(ref data._paging);
            return View(data);
        }

        public ActionResult Insert()
        {
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            ViewBag.drpNguonVon = _modelKHV.GetDataDropdownNguonNganSach();
            ViewBag.drpLoaiNganSach = _modelKHV.GetDataDropdownLoaiNganSach(DateTime.Now.Year);
            return View();
        }

        public ActionResult Update(Guid id)
        {
            VDT_QT_DeNghiQuyetToanNienDo_TroLy data = _vdtService.GetQuyetToanNienDoTroLyById(id);
            ViewBag.drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(Username);
            ViewBag.drpNguonVon = _modelKHV.GetDataDropdownNguonNganSach();
            ViewBag.drpLoaiNganSach = _modelKHV.GetDataDropdownLoaiNganSach(DateTime.Now.Year);
            return View(data);
        }

        #region PartialView
        [HttpPost]
        public ActionResult QuyetToanNienDoTroLyView(PagingInfo _paging, Guid? iIdDonViQuanLy , int? iIdNguonVon , DateTime? dNgayDeNghiFrom , DateTime? dNgayDeNghiTo , int? iNamKeHoach )
        {
            string sMaNguoiDung = Username;
            QuyetToanNienDoTroLyPagingModel data = new QuyetToanNienDoTroLyPagingModel();
            data._paging = _paging;
            data.lstData = _vdtService.GetAllQuyetToanNienDoPaging(ref data._paging, iIdDonViQuanLy, iIdNguonVon,dNgayDeNghiFrom,dNgayDeNghiTo,iNamKeHoach);
            var drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            drpDonViQuanLy.Insert(0, new SelectListItem() { Text = Constants.TAT_CA });
            ViewBag.drpDonViQuanLy = drpDonViQuanLy;

            var drpNguonVon = _modelKHV.GetDataDropdownNguonNganSach();
            drpNguonVon.Insert(0, new SelectListItem() { Text = Constants.TAT_CA });
            ViewBag.drpNguonVon = drpNguonVon;
            return PartialView("_list", data);
        }
        #endregion

        #region Event
        [HttpPost]
        public JsonResult GetDataThongTinNganhByLoaiNganSach(string sLNS, int iNamKeHoach)
        {
            if (string.IsNullOrEmpty(sLNS))
                return Json(new { data = new List<SelectListItem>(), bIsComplete = true }, JsonRequestBehavior.AllowGet);
            var data = _model.GetDataThongTinNganhByLoaiNganSach(sLNS, iNamKeHoach);
            return Json(new { data = data.Distinct().ToList(), bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataDropDownDuAn(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID)
        {
            var data = _model.GetDataDropDownDuAn(iID_DonViQuanLyID, iID_NguonVonID, iID_LoaiNguonVonID, dNgayQuyetDinh, iNamKeHoach, iID_NganhID);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDetailDuAn(Guid iIdDuAnId, Guid iIdNganh, int iNamKeHoach, DateTime dNgayQuyetDinh, Guid iIdDonViQuanLyId, int iIdNguonVonId, Guid iIdLoaiNguonVon)
        {
            decimal fChiTieuNganSachNam = _vdtService.GetChiTieuNganSachNamInQTNienDoByDuAn(iIdDuAnId, iIdNganh, iNamKeHoach, dNgayQuyetDinh, iIdDonViQuanLyId, iIdNguonVonId);
            decimal fCapPhatVonNamNay = _vdtService.GetCapPhatVonNamNayInQTNienDoByDuAn(iIdDuAnId, iIdNganh, iNamKeHoach, dNgayQuyetDinh, iIdDonViQuanLyId, iIdNguonVonId, iIdLoaiNguonVon);
            decimal fTongMucDauTuPheDuyet = _vdtService.GetTongMucDauTuInQTNienDoByDuAn(iIdDuAnId, dNgayQuyetDinh, iIdNguonVonId);
            return Json(new { fChiTieuNganSachNam = fChiTieuNganSachNam, fCapPhatVonNamNay = fCapPhatVonNamNay, fTongMucDauTuPheDuyet = fTongMucDauTuPheDuyet, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetQuyetToanNienDoChiTietByParentId(Guid iId)
        {
            return Json(new { data = _vdtService.GetQuyetToanNienDoTroLyChiTietInGridUpdate(iId) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public JsonResult InsertNienDoTroLy(VDT_QT_DeNghiQuyetToanNienDo_TroLy data, List<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet> lstDataDetail) 
        {
            return Json(new { bIsComplete = _model.InsertNienDoTroLy(data, lstDataDetail, Username) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteQuyetToanNienDoTroLy(Guid id)
        {
            return Json(new { data = _vdtService.DeleteQuyetToanNienDoTroLy(id, Username) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateQuyetToanNienDoTroLy(VDT_QT_DeNghiQuyetToanNienDo_TroLy data)
        {
            return Json(new { bIsComplete = _model.UpdateQuyetToanNienDoTroLy(data, Username) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateQuyetToanNienDoTroLyChiTiet(Guid iId, List<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet> data)
        {
            if (_vdtService.RemoveQuyetToanNienDoTroLyChiTietByParentId(iId))
            {
                return Json(new { bIsComplete = _model.InsertQuyetToanNienDoTroLyChiTiet(iId, data) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
        }

    }
}