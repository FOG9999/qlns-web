using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class QLKeHoachVonNamController : AppController
    {
        private QLKeHoachVonNamModel _model = new QLKeHoachVonNamModel();
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;

        // GET: QLVonDauTu/QLKeHoachVonNam
        public ActionResult Index()
        {
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLy = _model.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            VDTKHVPhanBoVonPagingModel data = new VDTKHVPhanBoVonPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _vdtService.GetAllPhanBoVonPaging(ref data._paging);
            return View(data);
        }

        public ActionResult Insert()
        {
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLy = _model.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            ViewBag.drpNguonVon = _model.GetDataDropdownNguonNganSach();
            ViewBag.drpLoaiNganSach = _model.GetDataDropdownLoaiNganSach(DateTime.Now.Year);
            ViewBag.drpCapPheDuyet = _model.GetDataDropdownCapPheDuyet();
            return View();
        }

        public ActionResult Update(Guid id)
        {
            var data = _vdtService.GetPhanBoVonByID(id);
            string sMaNguoiDung = data.sUserCreate;
            ViewBag.drpDonViQuanLy = _model.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            ViewBag.drpNguonVon = _model.GetDataDropdownNguonNganSach();
            ViewBag.drpLoaiNganSach = _model.GetDataDropdownLoaiNganSach(DateTime.Now.Year);
            ViewBag.drpCapPheDuyet = _model.GetDataDropdownCapPheDuyet();

            var drpDonViQuanLy = _model.GetDataDropdownDonViQuanLy(data.sUserCreate);
            if (drpDonViQuanLy != null) ViewBag.DonViQuanLy = drpDonViQuanLy.Where(n => n.Value.ToUpper() == data.iID_DonViQuanLyID.ToString().ToUpper()).Select(n => n.Text).FirstOrDefault();

            var drpNguonVon = _model.GetDataDropdownNguonNganSach();
            if (drpNguonVon != null) ViewBag.NguonVon = drpNguonVon.Where(n => n.Value.ToUpper() == data.iID_NguonVonID.ToString().ToUpper()).Select(n => n.Text).FirstOrDefault();

            var drpLoaiNganSach = _model.GetDataDropdownLoaiNganSach(data.iNamKeHoach ?? 0).FirstOrDefault(n => n.Value != string.Empty && n.Value.Split('|')[1].ToUpper() == data.iID_LoaiNguonVonID.ToString().ToUpper());
            if (drpLoaiNganSach != null)
            {
                ViewBag.LoaiNganSach = drpLoaiNganSach.Text;
                ViewBag.LoaiNganSachId = drpLoaiNganSach.Value;
            }

            ViewBag.fChiTieuDuyet = (data.fGiaTrPhanBo ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));

            return View(data);
        }

        public ActionResult ChinhSua(Guid id)
        {
            var data = _vdtService.GetPhanBoVonByID(id); 
            string sMaNguoiDung = data.sUserCreate;
            ViewBag.drpDonViQuanLy = _model.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            ViewBag.drpNguonVon = _model.GetDataDropdownNguonNganSach();
            ViewBag.drpLoaiNganSach = _model.GetDataDropdownLoaiNganSach(DateTime.Now.Year);
            ViewBag.drpCapPheDuyet = _model.GetDataDropdownCapPheDuyet();

            var drpDonViQuanLy = _model.GetDataDropdownDonViQuanLy(data.sUserCreate);
            if (drpDonViQuanLy != null) ViewBag.DonViQuanLy = drpDonViQuanLy.Where(n => n.Value.ToUpper() == data.iID_DonViQuanLyID.ToString().ToUpper()).Select(n => n.Text).FirstOrDefault();

            var drpNguonVon = _model.GetDataDropdownNguonNganSach();
            if (drpNguonVon != null) ViewBag.NguonVon = drpNguonVon.Where(n => n.Value.ToUpper() == data.iID_NguonVonID.ToString().ToUpper()).Select(n => n.Text).FirstOrDefault();

            var drpLoaiNganSach = _model.GetDataDropdownLoaiNganSach(data.iNamKeHoach ?? 0).FirstOrDefault(n => n.Value != string.Empty && n.Value.Split('|')[1].ToUpper() == data.iID_LoaiNguonVonID.ToString().ToUpper());
            if (drpLoaiNganSach != null)
            {
                ViewBag.LoaiNganSach = drpLoaiNganSach.Text;
                ViewBag.LoaiNganSachId = drpLoaiNganSach.Value;
            }


            return View(data);
        }

        #region PartialView
        [HttpPost]
        public ActionResult VDTKHVPhanBoVonView(PagingInfo _paging, string sSoKeHoach, int? iNamKeHoach, DateTime? dNgayLapFrom, DateTime? dNgayLapTo, Guid? sDonViQuanLy)
        {
            VDTKHVPhanBoVonPagingModel data = new VDTKHVPhanBoVonPagingModel();
            data._paging = _paging;
            data.lstData = _vdtService.GetAllPhanBoVonPaging(ref data._paging, sSoKeHoach, iNamKeHoach, dNgayLapFrom, dNgayLapTo, sDonViQuanLy);
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLy = _model.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            return PartialView("_list", data);
        }
        #endregion

        #region Event
        public JsonResult GetPhanBoVonDuplicate(Guid iDonViQuanLy, int iNguonVonId, int iNamKeHoach)
        {
            return Json(new { data = _vdtService.GetPhanBoVonDuplicate(iDonViQuanLy,iNguonVonId,iNamKeHoach), bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataDropdownLoaiAndKhoanByLoaiNganSach(string sLNS, int iNamKeHoach)
        {
            if (string.IsNullOrEmpty(sLNS)) return Json(new { data = new List<SelectListItem>(), bIsComplete = true }, JsonRequestBehavior.AllowGet);
            var data = _model.GetDataDropdownLoaiAndKhoanByLoaiNganSach(sLNS, iNamKeHoach);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataDropdownLoaiNganSach(int iNamKeHoach)
        {
            var data = _model.GetDataDropdownLoaiNganSach(iNamKeHoach);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataThongTinChiTietLoaiNganSach(string sLNS, string sLoai, string sKhoan, int iNamKeHoach, string sMuc = "", string sTieuMuc = "", string sTietMuc = "")
        {
            if (string.IsNullOrEmpty(sLNS) || string.IsNullOrEmpty(sLoai) || string.IsNullOrEmpty(sKhoan))
                return Json(new { data = new List<SelectListItem>(), bIsComplete = true }, JsonRequestBehavior.AllowGet);
            var data = _model.GetDataThongTinChiTietLoaiNganSach(sLNS, sLoai, sKhoan, iNamKeHoach);
            return Json(new { data = data.Distinct().ToList(), bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataDropDownDuAnByLoaiCongTrinh(Guid iId)
        {
            var data = _model.GetDataDropDownDuAnByLoaiCongTrinh(iId);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongTinDuAn(Guid iId)
        {
            var data = _model.GetVDT_DA_DuAn(iId);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy thông tin chi tiết đầu tư theo dự án
        /// </summary>
        /// <param name="iId">iId_DuAnID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetThongTinDauTuDuAn(Guid iId, int iNamKeHoach, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, Guid iNganh, DateTime? dNgayLap = null)
        {
            var data = _model.GetDataByChooseDuAn(iId, iNamKeHoach, iDonViQuanLy, iNguonVon, iLoaiNganSach, iNganh, dNgayLap);
            return Json(new { bIsComplete = (data == null ? false : true), data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataGridViewDefault()
        {
            return Json(new { data = _model.GetDataGridViewDefault(), bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BtnLayDuLieu_Click(int iNamKeHoach, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, DateTime? dNgayLap = null)
        {
            //var data = 0;
            var data = _model.TimKiemDuLieu(iNamKeHoach, iDonViQuanLy, iNguonVon, iLoaiNganSach, dNgayLap);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInfoPhanBoVonChiTietInGridViewByPhanBoVonID(Guid iId)
        {
            return Json(new { data = _vdtService.GetInfoPhanBoVonChiTietInGridViewByPhanBoVonID(iId), bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Process
        [HttpPost]
        public JsonResult InsertPhanBoVon(VDT_KHV_PhanBoVon data)
        {
            if (_model.InsertVdtKhvPhanBoVon(ref data, Username))
            {
                return Json(new { data = data.iID_PhanBoVonID, bIsComplete = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertPhanBoVonChiTiet(List<VDT_KHV_PhanBoVon_ChiTiet> lstData)
        {
            return Json(new { bIsComplete = _model.InsertVdtKhvPhanBoVonChiTiet(ref lstData) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePhanBoVon(VDT_KHV_PhanBoVon data)
        {
            return Json(new { bIsComplete = _model.UpdateVdtKhvPhanBoVon(data, Username) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool DeleteVdtKhvPhanBoVon(Guid id)
        {
            //if (!_model.DeleteVdtKhvPhanBoVon(id, Username)) return false;
            if (!_vdtService.deleteKeHoachVonNam(id)) return false;
            return true;
        }

        [HttpPost]
        public JsonResult DeleteKeHoachNamChiTietByKeHoachNamId(Guid iId)
        {
            return Json(new { bIsComplete = _vdtService.DeleteKeHoachNamChiTietByKeHoachNamId(iId) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChinhSuaPhanBoVon(VDT_KHV_PhanBoVon data, List<VDT_KHV_PhanBoVon_ChiTiet> lstDetail)
        {
            return Json(new { bIsComplete = _model.ChinhSuaPhanBoVon(data, lstDetail, Username) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Check validate NinhNV
        [HttpPost]
        public JsonResult checkWhenSaveData(Guid iID_PhanBoVonID, Guid iID_DonViQuanLyID, int iID_NguonVonID, int iNamKeHoach, string sSoQuyetDinh)
        {
            string errMes = "";
            bool bIsComplete = true;
            bool isExistPBV = false;
            if (iID_PhanBoVonID == new Guid())
            {
                isExistPBV = _vdtService.CheckDupKeHoachNam(iID_PhanBoVonID, iID_DonViQuanLyID, iID_NguonVonID, iNamKeHoach);
            }
            bool isExistSoQuyetDinh = _vdtService.CheckDupSoKeHoach(iID_PhanBoVonID, iID_DonViQuanLyID, sSoQuyetDinh);
            if (isExistPBV)
            {
                errMes = "Đơn vị quản lý và loại nguồn vốn trong năm kế hoạch này đã tồn tại.";
            }
            else if (isExistSoQuyetDinh)
            {
                errMes = "Số quyết định đã tồn tại với đơn vị này.";
            }

            if (errMes != "")
            {
                bIsComplete = false;
            }
            return Json(new { bIsComplete, errMes = errMes }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}