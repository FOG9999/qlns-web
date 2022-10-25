using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class QLKhoiTaoThongTinDuAnChuyenTiepController : AppController
    {
        private QLKeHoachVonNamModel _modelKHV = new QLKeHoachVonNamModel();
        private GiaiNganThanhToanModel _model = new GiaiNganThanhToanModel();
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private readonly INganSachNewService _iNganSachNewService = NganSachNewService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;
        private const string sFilePath = "/Report_ExcelFrom/VonDauTu/rpt_KhoiTaoDuAnChuyenTiep_Danhsach.xls";

        // GET: QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep
        public ActionResult Index()
        {
            string sMaNguoiDung = Username;
            var dataDropDown = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            dataDropDown.Insert(0, new SelectListItem { Text = "", Value = "" });
            ViewBag.drpDonViQuanLy = dataDropDown;
            VDTQLKhoiTaoDuAnPagingModel data = new VDTQLKhoiTaoDuAnPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _vdtService.GetAllQLKhoiTaoDuAnPaging(ref data._paging);
            return View(data);
        }

        public ActionResult TaoMoi()
        {
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            ViewBag.drpNguonVon = _modelKHV.GetDataDropdownNguonNganSach();
            ViewBag.drpLoaiNganSach = _modelKHV.GetDataDropdownLoaiNganSach(PhienLamViec.NamLamViec);

            List<DM_ChuDauTu> lstChuDauTu = _vdtService.LayChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Common.Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.ToSelectList("sId_CDT", "sTenCDT");

            List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _vdtService.LayPhanCapDuAn().ToList();
            lstPhanCapDuAn.Insert(0, new VDT_DM_PhanCapDuAn { iID_PhanCapID = Guid.Empty, sTen = Common.Constants.CHON });
            ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Common.Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");

            List<VDT_DM_NhomQuanLy> lstNhomQuanLy = _vdtService.GetNhomQuanLyList().ToList();
            ViewBag.ListNhomQuanLy = lstNhomQuanLy.ToSelectList("iID_NhomQuanLyID", "sTenNhomQuanLy");

            ViewBag.ListNhaThau = _vdtService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");

            ViewBag.ListDuAn = _vdtService.GetVDTDADuAn().ToSelectList("iID_DuAnID", "sTenDuAn");

            VDTQLKhoiTaoDuAnViewModel data = new VDTQLKhoiTaoDuAnViewModel();

            return View(data);
        }

        public ActionResult Sua(Guid id)
        {
            var data = _vdtService.GetDetailQLKhoiTaoDuAn(id);
            string sMaNguoiDung = data.sUserCreate ?? Username;
            ViewBag.drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            ViewBag.drpNguonVon = _modelKHV.GetDataDropdownNguonNganSach();
            ViewBag.drpLoaiNganSach = _modelKHV.GetDataDropdownLoaiNganSach(data.iNamKhoiTao);
            List<DM_ChuDauTu> lstChuDauTu = _vdtService.LayChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Common.Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.ToSelectList("iID_Ma", "sTen");

            List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _vdtService.LayPhanCapDuAn().ToList();
            lstPhanCapDuAn.Insert(0, new VDT_DM_PhanCapDuAn { iID_PhanCapID = Guid.Empty, sTen = Common.Constants.CHON });
            ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Common.Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");

            List<VDT_DM_NhomQuanLy> lstNhomQuanLy = _vdtService.GetNhomQuanLyList().ToList();
            ViewBag.ListNhomQuanLy = lstNhomQuanLy.ToSelectList("iID_NhomQuanLyID", "sTenNhomQuanLy");

            ViewBag.ListNhaThau = _vdtService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");

            ViewBag.ListDuAn = _vdtService.GetVDTDADuAn().ToSelectList("iID_DuAnID", "sTenDuAn");

            return View(data);
        }

        public ActionResult ChiTiet(Guid id)
        {
            var data = _vdtService.GetDetailQLKhoiTaoDuAn(id);
            return View(data);
        }

        #region Event
        [HttpPost]
        public JsonResult GetDataDropDownDuAn(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID)
        {
            var data = _model.GetDataDropdownDuAn(iID_DonViQuanLyID, iID_NguonVonID, iID_LoaiNguonVonID, dNgayQuyetDinh, iNamKeHoach, iID_NganhID);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataThongTinChiTietLoaiNganSachByNganh(Guid iId_Nganh, int iNamKeHoach)
        {
            var data = _modelKHV.GetDataThongTinChiTietLoaiNganSachByNganh(iId_Nganh, iNamKeHoach);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataDropdownHopDong(Guid iIdDuAn)
        {
            var data = _model.GetDataDropdownHopDong(iIdDuAn);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDetailHopDongDuAn(Guid iID_DuAnID, Guid iID_HopDongID, DateTime dNgayDeNghi, int iNamKeHoach, int iID_NguonVonID, Guid iID_LoaiNguonVonID, Guid iID_NganhID)
        {
            var data = _model.GetDetailHopDongDuAn(iID_DuAnID, iID_HopDongID, dNgayDeNghi, iNamKeHoach, iID_NguonVonID, iID_LoaiNguonVonID, iID_NganhID);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataThongTinChiTietLoaiNganSach(string sLNS, int iNamKeHoach)
        {
            var data = _modelKHV.GetDataThongTinChiTietLoaiNganSach(sLNS, null, null, iNamKeHoach);
            return Json(new { data = data.Distinct().ToList(), bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LayThongTinChiTietDuAn(Guid id)
        {
            VDTQuanLyDuAnInfoModel topHopThongTin = new VDTQuanLyDuAnInfoModel();
            VDTDuAnInfoModel dataDuAn = new VDTDuAnInfoModel();
            VDTDuAnThongTinPheDuyetModel dataQDDT = new VDTDuAnThongTinPheDuyetModel();
            VDTDuAnThongTinDuToanModel dataDuToan = new VDTDuAnThongTinDuToanModel();

            dataDuAn = _vdtService.GetDuAnById(id);
            dataQDDT = _vdtService.GetVDTQDDauTu(id);
            dataDuToan = _vdtService.GetVDTDuToan(id);
            return Json(new { objDuAn = dataDuAn, objQDDT = dataQDDT, objDuToan = dataDuToan }, JsonRequestBehavior.AllowGet);
        }
        #region PartialView
        [HttpPost]
        public ActionResult QLKhoiTaoDuAnChuyenTiepView(PagingInfo _paging, string sTenDuAn, int? iNamKhoiTao)
        {
            VDTQLKhoiTaoDuAnPagingModel data = new VDTQLKhoiTaoDuAnPagingModel();
            data._paging = _paging;
            data.lstData = _vdtService.GetAllQLKhoiTaoDuAnPaging(ref data._paging, sTenDuAn, iNamKhoiTao);
            string sMaNguoiDung = Username;
            var dataDropDown = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);
            dataDropDown.Insert(0, new SelectListItem { Text = "", Value = "" });
            ViewBag.drpDonViQuanLy = dataDropDown;

            // luu dieu kien tim kiem
            TempData["sTenDuan"] = sTenDuAn;
            TempData["iNamKhoiTao"] = iNamKhoiTao;

            return PartialView("_list", data);
        }
        #endregion

        #endregion

        #region Process
        [HttpPost]
        public JsonResult LuuKhoiTao(VDT_KT_KhoiTao objKhoiTao, List<VDTKTKhoiTaoChiTietModel> lstKhoiTaoChiTiet, VDT_DA_DuToan objDuToan, VDT_DA_QDDauTu objQDDT, VDT_DA_DuAn objDuAn, List<VDTKTKhoiTaoChiTietNhaThau> lstNhaThau, int isUpdate = 0)
        {
            bool saveData = false;
            if (isUpdate == 1)
                saveData = _vdtService.CapNhatKhoiTao(objKhoiTao, lstKhoiTaoChiTiet, objDuToan, objQDDT, objDuAn, lstNhaThau, Username);
            else
                saveData = _vdtService.LuuKhoiTao(objKhoiTao, lstKhoiTaoChiTiet, objDuToan, objQDDT, objDuAn, lstNhaThau, Username);

            if (saveData)
                return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
            return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool DeleteKhoiTao(Guid id)
        {
            if (!_model.DeleteDeNghiThanhToan(id, Username)) return false;
            return true;
        }

        [HttpPost]
        public ActionResult XuatFile()
        {
            ExcelFile excel = TaoFile();
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Danh sach quan ly khoi tao du an chuyen tiep.xls");
            }
        }

        public ExcelFile TaoFile()
        {
            string sTenDuAn = string.Empty;
            int? iNamKhoiTao = null;

            // get dieu kien tiem kiem
            if (TempData["sTenDuAn"] != null)
                sTenDuAn = (string)TempData["sTenDuAn"];
            if (TempData["iNamKhoiTao"] != null)
                iNamKhoiTao = (int)TempData["iNamKhoiTao"];

            IEnumerable<VDTQLKhoiTaoDuAnViewModel> listData = _vdtService.GetAllQLKhoiTaoDuAnExport(sTenDuAn, iNamKhoiTao);

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VDTQLKhoiTaoDuAnViewModel>("dt", listData);
            fr.Run(Result);
            return Result;
        }
        #endregion

    }
}