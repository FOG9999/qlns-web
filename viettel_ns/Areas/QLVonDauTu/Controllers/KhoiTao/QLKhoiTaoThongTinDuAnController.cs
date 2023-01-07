using DocumentFormat.OpenXml.Drawing.Charts;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.z.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.KhoiTao
{
    public class QLKhoiTaoThongTinDuAnController : FlexcelReportController
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;

        // GET: QLVonDauTu/QLKhoiTaoThongTinDuAn
        public ActionResult Index()
        {
            KhoiTaoThongTinDuAnViewModel vm = new KhoiTaoThongTinDuAnViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qLVonDauTuService.GetAllKhoiTaoThongTinDuAn(ref vm._paging);

            return View(vm);
        }

        public ActionResult Import()
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            ViewBag.drpDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            return View();
        }

        [HttpPost]
        public ActionResult KhoiTaoTTDAListView(PagingInfo _paging, int? iNamKhoiTao, string sTenDonVi)
        {
            KhoiTaoThongTinDuAnViewModel vm = new KhoiTaoThongTinDuAnViewModel();
            vm._paging = _paging;
            vm.Items = _qLVonDauTuService.GetAllKhoiTaoThongTinDuAn(ref vm._paging, iNamKhoiTao, sTenDonVi);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            VDT_KT_KhoiTao_DuLieu_ViewModel data = new VDT_KT_KhoiTao_DuLieu_ViewModel();
            if (id.HasValue)
            {
                data = _qLVonDauTuService.GetKhoiTaoTTDAById(id.Value);
            }
            else
            {
                data.iNamKhoiTao = DateTime.Now.Year;
                data.dNgayKhoiTao = DateTime.Now;
            }

            List<NS_DonVi> lstDonViQL = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn--" });
            ViewBag.ListDonViQL = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDT_KT_KhoiTao_DuLieu_ViewModel data = _qLVonDauTuService.GetKhoiTaoTTDAById(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool DeleteKhoiTaoTTDA(Guid id)
        {
            return _qLVonDauTuService.DeleteKhoiTaoTTDA(id);
        }

        [HttpPost]
        public JsonResult KhoiTaoTTDASave(VDT_KT_KhoiTao_DuLieu data)
        {
            Guid iID_KhoiTao = Guid.Empty;
            string sMaDonVi = string.Empty;
            if (!_qLVonDauTuService.SaveKhoiTaoTTDA(ref iID_KhoiTao, ref sMaDonVi, data, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true, iID_KhoiTao = iID_KhoiTao, sMaDonVi = sMaDonVi }, JsonRequestBehavior.AllowGet);
        }

        #region Chi tiết khởi tạo TTDA
        public ActionResult Detail(Guid iID_KhoiTao, /*Guid iID_DonViQL*/ string sMaDonVi)
        {
            KhoiTaoThongTinDuAnChiTietViewModel vm = new KhoiTaoThongTinDuAnChiTietViewModel();
            vm._paging.CurrentPage = 1;
            vm.iID_KhoiTaoID = iID_KhoiTao;
            vm.sMaDonVi = sMaDonVi;
            vm.Items = _qLVonDauTuService.GetKhoiTaoTTDAChiTietByIdKhoiTao(iID_KhoiTao);
            TempData["KhoiTaoChiTiet"] = vm.Items;
            List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.GetDuAnByMaDonViQL(sMaDonVi).ToList();
            List<VDTDuAnDropViewModel> lstDuAnShow = new List<VDTDuAnDropViewModel>();

            List<VDT_DM_LoaiCongTrinh> drpLoaiCongTrinh = new List<VDT_DM_LoaiCongTrinh>() {
                new VDT_DM_LoaiCongTrinh(){iID_LoaiCongTrinh = Guid.Empty, sTenLoaiCongTrinh = "--Chọn--"}
            };
            var lstLoaiCongTrinh = _qLVonDauTuService.GetAllDmLoaiCongTrinh();
            if (lstLoaiCongTrinh != null) drpLoaiCongTrinh.AddRange(lstLoaiCongTrinh);
            ViewBag.LoaiCongTrinh = drpLoaiCongTrinh.ToSelectList("iID_LoaiCongTrinh", "sTenLoaiCongTrinh");

            foreach (var item in lstDuAn)
            {
                lstDuAnShow.Add(new VDTDuAnDropViewModel
                {
                    iID_DuAnID = item.iID_DuAnID,
                    sTenDuAn = item.sTenDuAn,
                    sMaDuAn = item.sMaDuAn,
                    iID_DonViQuanLyID = item.iID_DonViQuanLyID,
                    iID_ChuDauTuID = item.iID_ChuDauTuID,
                    iID_DonViThucHienDuAnID = item.iID_DonViThucHienDuAnID,
                    iID_LoaiDuAnId = item.iID_LoaiDuAnId,
                    iID_MaDonVi = item.iID_MaDonVi,
                    iID_MaCDT = item.iID_MaCDT,
                    iID_NhomDuAnID= item.iID_NhomDuAnID
                });
            }
            lstDuAnShow.Insert(0, new VDTDuAnDropViewModel { iID_DuAnID = Guid.Empty, sTenDuAn = "--Chọn--" });
            ViewBag.ListDuAn = lstDuAnShow.ToSelectList("iID_DuAnID", "sTenDuAnShow");

            List<NS_NguonNganSach> lstNguonVon = _qLVonDauTuService.GetListAllNguonNganSach();
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = _qLVonDauTuService.GetListHopDongKhoiTaoTTDAByKhoiTaoID(iID_KhoiTao).ToList();
            TempData["TotalHopDongKhoiTao"] = lstTotalHopDong;

            return View(vm);
        }

        [HttpPost]
        public ActionResult ChiTietKhoiTaoTTDAListView(byte type, int STT, Guid iID_KhoiTao, /*Guid iID_DonViQL*/ string sMaDonVi)
        {
            KhoiTaoThongTinDuAnChiTietViewModel vm = new KhoiTaoThongTinDuAnChiTietViewModel();
            vm._paging.CurrentPage = 1;
            vm.iID_KhoiTaoID = iID_KhoiTao;
            vm.sMaDonVi = sMaDonVi;
            var lstData = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel>();
            if (TempData["KhoiTaoChiTiet"] != null)
            {
                lstData = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel>)TempData["KhoiTaoChiTiet"];
            }
            //type = 0: Xóa
            if (type == 0) {
                var itemRemove = lstData.SingleOrDefault(x => x.STT == STT);
                lstData.Remove(itemRemove);
            }
            //Thêm dòng
            if (type == 1)
            {
                var itemAdd = new VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel
                {
                    iID_KhoiTao_ChiTietID = Guid.NewGuid(),
                    STT = lstData.LastOrDefault() != null ? lstData.LastOrDefault().STT + 1 : 1,
                    iID_KhoiTaoDuLieuID = lstData.LastOrDefault() != null ? lstData.LastOrDefault().iID_KhoiTaoDuLieuID : iID_KhoiTao
                };
                lstData.Add(itemAdd);
            }
            vm.Items = lstData;
            TempData["KhoiTaoChiTiet"] = vm.Items;

            List<NS_NguonNganSach> lstNguonVon = _qLVonDauTuService.GetListAllNguonNganSach();
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.GetDuAnByMaDonViQL(sMaDonVi).ToList();
            lstDuAn.Insert(0, new VDT_DA_DuAn { iID_DuAnID = Guid.Empty, sTenDuAn = "--Chọn--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("iID_DuAnID", "sTenDuAn");

            List<VDT_DM_LoaiCongTrinh> drpLoaiCongTrinh = new List<VDT_DM_LoaiCongTrinh>() {
                new VDT_DM_LoaiCongTrinh(){iID_LoaiCongTrinh = Guid.Empty, sTenLoaiCongTrinh = "--Chọn--"}
            };
            var lstLoaiCongTrinh = _qLVonDauTuService.GetAllDmLoaiCongTrinh();
            if (lstLoaiCongTrinh != null) drpLoaiCongTrinh.AddRange(lstLoaiCongTrinh);
            ViewBag.LoaiCongTrinh = drpLoaiCongTrinh.ToSelectList("iID_LoaiCongTrinh", "sTenLoaiCongTrinh");
            return PartialView("_listDetail", vm);
        }

        [HttpGet]
        public JsonResult GetQdDauTuNguonVonByDuAn(Guid iIdDuAn)
        {
            var datas = _qLVonDauTuService.LayDanhSachNguonVonTheoDuAnInQDDauTu(iIdDuAn).ToList();
            if(datas != null && datas.Count != 0)
                return Json(new { strCombobox = datas.Select(n => string.Format("<option value='{0}'>{1}</option>", n.iID_MaNguonNganSach, n.sTen)) }, JsonRequestBehavior.AllowGet);
            List<NS_NguonNganSach> lstNguonVon = _qLVonDauTuService.GetListAllNguonNganSach();
            return Json(new { strCombobox  = lstNguonVon.Select(n => string.Format("<option value='{0}'>{1}</option>", n.iID_MaNguonNganSach, n.sTen)) }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetVDTNhaThauAndGiaTriHDByHopDong(Guid iID_HopDongID)
        {
            var objHopDong = _qLVonDauTuService.LayChiTietThongTinHopDong(iID_HopDongID);
            if(objHopDong != null)
                return Json(new { sTenNhaThau = objHopDong.sTenNhaThau, fGiaTriHopDong = (objHopDong.fTienHopDong??0) }, JsonRequestBehavior.AllowGet);

            return Json(new { sTenNhaThau = string.Empty, fGiaTriHopDong = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChiTietKhoiTaoTTDASave(List<VDT_KT_KhoiTao_DuLieu_ChiTiet> data)
        {
            foreach (var item in data)
            {
                item.sMaDuAn = _qLVonDauTuService.GetDuAnByIdDuAn(item.iID_DuAnID).sMaDuAn;
            }

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>();
            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan> lstHopDongSave = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan>();
            if (TempData["TotalHopDongKhoiTao"] != null)
            {
                lstTotalHopDong = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];
            }

            foreach(var item in lstTotalHopDong)
            {
                VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan itemSave = new VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan();
                itemSave.iID_KhoiTaoDuLieuChiTietThanhToanID = item.iID_KhoiTaoDuLieuChiTietThanhToanID;
                itemSave.iId_KhoiTaoDuLieuChiTietId = item.iId_KhoiTaoDuLieuChiTietId;
                itemSave.iID_HopDongId = item.iID_HopDongId;
                itemSave.fLuyKeTTKLHTTN_KHVN = item.fLuyKeTTKLHTTN_KHVN;
                itemSave.fLuyKeTUChuaThuHoiTN_KHVN = item.fLuyKeTUChuaThuHoiTN_KHVN;
                itemSave.fLuyKeTTKLHTNN_KHVN = item.fLuyKeTTKLHTNN_KHVN;
                itemSave.fLuyKeTUChuaThuHoiNN_KHVN = item.fLuyKeTUChuaThuHoiNN_KHVN;
                itemSave.fLuyKeTTKLHTTN_KHVU = item.fLuyKeTTKLHTTN_KHVU;
                itemSave.fLuyKeTUChuaThuHoiTN_KHVU = item.fLuyKeTUChuaThuHoiTN_KHVU;
                itemSave.fLuyKeTTKLHTNN_KHVU = item.fLuyKeTTKLHTNN_KHVU;
                itemSave.fLuyKeTUChuaThuHoiNN_KHVU = item.fLuyKeTUChuaThuHoiNN_KHVU;
                lstHopDongSave.Add(itemSave);
            }

            if (!_qLVonDauTuService.SaveChiTietKhoiTaoTTDA(data, lstTotalHopDong))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModalChiTietThanhToan(Guid iID_KhoiTao_ChiTietID, Guid iID_DuAnID)
        {
            HopDongKhoiTaoThongTinDuAnViewModel data = new HopDongKhoiTaoThongTinDuAnViewModel();
            data._paging.CurrentPage = 1;
            data.iID_KhoiTaoDuLieuChiTietID = iID_KhoiTao_ChiTietID;
            data.iID_DuAnID = iID_DuAnID;
            

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>();
            
            if (TempData["TotalHopDongKhoiTao"] != null)
            {
                lstTotalHopDong = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];
                TempData.Keep("TotalHopDongKhoiTao");
            }

            data.Items = lstTotalHopDong.Where(x => x.iId_KhoiTaoDuLieuChiTietId == iID_KhoiTao_ChiTietID);

            List<VDT_DA_TT_HopDong> lstHopDong = _qLVonDauTuService.GetHopDongByThanhToanDuAnId(iID_DuAnID).ToList();
            lstHopDong.Insert(0, new VDT_DA_TT_HopDong { iID_HopDongID = Guid.Empty, sSoHopDong = "--Chọn--" });
            ViewBag.ListHopDong = lstHopDong.Select(n => new SelectListItem { Text = string.Format("{0} - {1}", n.sSoHopDong, n.sTenHopDong), Value = n.iID_HopDongID.ToString() });

            return PartialView("_modalHopDong", data);
        }

        [HttpPost]
        public ActionResult ChiTietHopDongListView(byte type, int STT, Guid iID_KhoiTaoDuLieuChiTietID, Guid iID_DuAnID)
        {
            HopDongKhoiTaoThongTinDuAnViewModel vm = new HopDongKhoiTaoThongTinDuAnViewModel();
            vm._paging.CurrentPage = 1;
            vm.iID_KhoiTaoDuLieuChiTietID = iID_KhoiTaoDuLieuChiTietID;
            vm.iID_DuAnID = iID_DuAnID;
            var lstData = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>();
            if (TempData["TotalHopDongKhoiTao"] != null)
            {
                lstData = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];
            }
            //type = 0: Xóa
            if (type == 0)

            {
                var itemRemove = lstData.SingleOrDefault(x => x.STT == STT);
                lstData.Remove(itemRemove);
            }
            //Thêm dòng
            if (type == 1)
            {
                var itemAdd = new VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel
                {
                    iID_KhoiTaoDuLieuChiTietThanhToanID = Guid.NewGuid(),
                    STT = lstData.LastOrDefault() != null ? lstData.LastOrDefault().STT + 1 : 1,
                    iId_KhoiTaoDuLieuChiTietId = iID_KhoiTaoDuLieuChiTietID
                };
                lstData.Add(itemAdd);
            }
            vm.Items = lstData.Where(x => x.iId_KhoiTaoDuLieuChiTietId == iID_KhoiTaoDuLieuChiTietID);
            TempData["TotalHopDongKhoiTao"] = lstData;

            List<VDT_DA_TT_HopDong> lstHopDong = _qLVonDauTuService.GetHopDongByThanhToanDuAnId(iID_DuAnID).ToList();
            lstHopDong.Insert(0, new VDT_DA_TT_HopDong { iID_HopDongID = Guid.Empty, sSoHopDong = "--Chọn--" });
            ViewBag.ListHopDong = lstHopDong.ToSelectList("iID_HopDongID", "sSoHopDong");

            return PartialView("_partialModalHopDong", vm);
        }

        [HttpPost]
        public JsonResult ChangeListHopDong(List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan> data)
        {
            if(TempData["TotalHopDongKhoiTao"] == null || data== null || data.Count ==0)
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];

            foreach(var item in data)
            {
                foreach (var item2 in lstTotalHopDong)
                {
                    if(item.iID_KhoiTaoDuLieuChiTietThanhToanID == item2.iID_KhoiTaoDuLieuChiTietThanhToanID)
                    {
                        item2.iID_HopDongId = item.iID_HopDongId;
                        item2.fLuyKeTTKLHTTN_KHVN = item.fLuyKeTTKLHTTN_KHVN;
                        item2.fLuyKeTUChuaThuHoiTN_KHVN = item.fLuyKeTUChuaThuHoiTN_KHVN;
                        item2.fLuyKeTTKLHTNN_KHVN = item.fLuyKeTTKLHTNN_KHVN;
                        item2.fLuyKeTUChuaThuHoiNN_KHVN = item.fLuyKeTUChuaThuHoiNN_KHVN;

                        item2.fLuyKeTTKLHTTN_KHVU = item.fLuyKeTTKLHTTN_KHVU;
                        item2.fLuyKeTUChuaThuHoiTN_KHVU = item.fLuyKeTUChuaThuHoiTN_KHVU;
                        item2.fLuyKeTTKLHTNN_KHVU = item.fLuyKeTTKLHTNN_KHVU;
                        item2.fLuyKeTUChuaThuHoiNN_KHVU = item.fLuyKeTUChuaThuHoiNN_KHVU;
                    }
                }
            }
            TempData["TotalHopDongKhoiTao"] = lstTotalHopDong;
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Export, Import
        [HttpPost]
        public JsonResult OnExport(Guid id)
        {
            try
            {
                var data = _qLVonDauTuService.GetKhoiTaoTTDAChiTietByIdKhoiTao(id);
                XlsFile Result = new XlsFile(true);
                FlexCelReport fr = new FlexCelReport();
                fr.AddTable("Items", data);
                Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rptVDT_TongHopKhoiTao.xlsx"));
                fr.Run(Result);
                TempData["DataExportKhoiTaoXls"] = Result;
                FlexCelPdfExport pdf = new FlexCelPdfExport(Result, true);
                var bufferPdf = new MemoryStream();
                pdf.Export(bufferPdf);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportReport()
        {
            ExcelFile xls = (ExcelFile)TempData["DataExportKhoiTaoXls"];
            return Print(xls, "xls", string.Format("rptVDT_TongHopKhoiTao_{0}.xlsx", DateTime.Now.ToString("ddMMyyyy_HHmm")));
        }


        [HttpPost]
        public JsonResult LoadDataExcel(HttpPostedFileBase file, Guid iIdDonVi)
        {
            try
            {
                List<string> lstError = new List<string>();
                if (file == null) return Json(new { bIsComplete = false, sMessError = "Chưa chọn file import !" }, JsonRequestBehavior.AllowGet);
                var file_data = getBytes(file);
                var dt = ExcelHelpers.LoadExcelDataTable(file_data);
                string sMaDonVi = string.Empty;
                var objDonVi = _qLVonDauTuService.GetDonViQuanLyById(iIdDonVi);
                if (objDonVi != null) sMaDonVi = objDonVi.iID_MaDonVi;
                IEnumerable<VdtKtKhoiTaoDuLieuChiTietImportModel> dataImport = excel_result(sMaDonVi, dt, ref lstError);
                return Json(new { bIsComplete = true, data = dataImport, ListError = lstError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = false, sMessError = "Có lỗi xảy ra trong file import !" }, JsonRequestBehavior.AllowGet);
        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private IEnumerable<VdtKtKhoiTaoDuLieuChiTietImportModel> excel_result(string sMaDonVi, System.Data.DataTable dt, ref List<string> lstError)
        {
            Dictionary<string, Guid> dicMaDuAn = new Dictionary<string, Guid>();
            Dictionary<string, Guid> dicLoaiCongTrinh = new Dictionary<string, Guid>();

            var lstAllDuAn = _qLVonDauTuService.GetDuAnByMaDonViQL(sMaDonVi);
            if (lstAllDuAn != null)
            {
                foreach (var item in lstAllDuAn)
                {
                    if (!dicMaDuAn.ContainsKey(item.sMaDuAn)) dicMaDuAn.Add(item.sMaDuAn, item.iID_DuAnID);
                }
            }
            var lstAllLoaiCongTrinh = _qLVonDauTuService.GetAllDmLoaiCongTrinh();
            if (lstAllLoaiCongTrinh != null)
            {
                foreach (var item in lstAllLoaiCongTrinh)
                {
                    if (!dicLoaiCongTrinh.ContainsKey(item.sMaLoaiCongTrinh)) dicLoaiCongTrinh.Add(item.sMaLoaiCongTrinh, item.iID_LoaiCongTrinh);
                }
            }

            List<VdtKtKhoiTaoDuLieuChiTietImportModel> dataImport = new List<VdtKtKhoiTaoDuLieuChiTietImportModel>();

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) != "" || x.Field<string>(0) != null || x.Field<string>(0) != "null");
            int index = 1;
            for (var i = 7; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];

                var sMaDuAn = r.Field<string>(0);
                var sTenDuAn = r.Field<string>(1);
                var sMaLoaiCongTrinh = r.Field<string>(2);
                var fKHVN_VonBoTriHetNamTruoc = r.Field<string>(3);
                var fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = r.Field<string>(4);
                var fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi = r.Field<string>(5);
                var fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc = r.Field<string>(6);
                var fKHVN_KeHoachVonKeoDaiSangNam = r.Field<string>(7);
                var fKHUT_VonBoTriHetNamTruoc = r.Field<string>(8);
                var fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = r.Field<string>(9);
                var fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi = r.Field<string>(10);
                var fKHUT_KeHoachUngTruocKeoDaiSangNam = r.Field<string>(11);
                var fKHUT_KeHoachUngTruocChuaThuHoi = r.Field<string>(12);

                var e = new VdtKtKhoiTaoDuLieuChiTietImportModel
                {
                    sMaDuAn = string.IsNullOrEmpty(sMaDuAn) ? string.Empty : sMaDuAn,
                    sTenDuAn = string.IsNullOrEmpty(sTenDuAn) ? string.Empty : sTenDuAn,
                    sMaLoaiCongTrinh = string.IsNullOrEmpty(sMaLoaiCongTrinh) ? string.Empty : sMaLoaiCongTrinh,
                    fKHVN_VonBoTriHetNamTruoc = string.IsNullOrEmpty(fKHVN_VonBoTriHetNamTruoc) ? string.Empty : fKHVN_VonBoTriHetNamTruoc,
                    fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = string.IsNullOrEmpty(fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc) ? string.Empty : fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc,
                    fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi = string.IsNullOrEmpty(fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi) ? string.Empty : fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi,
                    fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc = string.IsNullOrEmpty(fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc) ? string.Empty : fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc,
                    fKHVN_KeHoachVonKeoDaiSangNam = string.IsNullOrEmpty(fKHVN_KeHoachVonKeoDaiSangNam) ? string.Empty : fKHVN_KeHoachVonKeoDaiSangNam,
                    fKHUT_VonBoTriHetNamTruoc = string.IsNullOrEmpty(fKHUT_VonBoTriHetNamTruoc) ? string.Empty : fKHUT_VonBoTriHetNamTruoc,
                    fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = string.IsNullOrEmpty(fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc) ? string.Empty : fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc,
                    fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi = string.IsNullOrEmpty(fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi) ? string.Empty : fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi,
                    fKHUT_KeHoachUngTruocKeoDaiSangNam = string.IsNullOrEmpty(fKHUT_KeHoachUngTruocKeoDaiSangNam) ? string.Empty : fKHUT_KeHoachUngTruocKeoDaiSangNam,
                    fKHUT_KeHoachUngTruocChuaThuHoi = string.IsNullOrEmpty(fKHUT_KeHoachUngTruocChuaThuHoi) ? string.Empty : fKHUT_KeHoachUngTruocChuaThuHoi,
                };
                e.bIsError = ValidateChungTuChiTietImport(index, e, dicMaDuAn, dicLoaiCongTrinh, ref lstError);
                dataImport.Add(e);
                index++;
            }
            return dataImport.AsEnumerable();
        }

        public bool ValidateChungTuChiTietImport(int index, VdtKtKhoiTaoDuLieuChiTietImportModel item, Dictionary<string, Guid> dicMaDuAn, Dictionary<string, Guid> dicLoaiCongTrinh, ref List<string> lstError)
        {
            double dDoublePare = 0;
            bool bIsError = false;

            if (string.IsNullOrEmpty(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "mã dự án"));
                bIsError = true;
            }
            else if (!dicMaDuAn.ContainsKey(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Không tìm thấy dự án [{1}] !", index, item.sMaDuAn));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.sMaLoaiCongTrinh))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "mã loại công trình"));
                bIsError = true;
            }else if (!dicLoaiCongTrinh.ContainsKey(item.sMaLoaiCongTrinh))
            {
                lstError.Add(string.Format("Dòng {0} - Không tìm thấy loại công trình [{1}] !", index, item.sMaLoaiCongTrinh));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.fKHVN_VonBoTriHetNamTruoc) && !double.TryParse(item.fKHVN_VonBoTriHetNamTruoc, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Vốn bố trí hết năm trước KHVN không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc) && !double.TryParse(item.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Lũy kế vốn đã thanh toán từ khởi công đến hết năm trước KHVN không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi) && !double.TryParse(item.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Vốn tạm ứng theo chế độ chưa thu hồi KHVN không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc) && !double.TryParse(item.fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Lũy kế vốn tạm ứng theo chế độ chưa thu hồi  nộp điều chỉnh giảm đến hết năm trước  không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHVN_KeHoachVonKeoDaiSangNam) && !double.TryParse(item.fKHVN_KeHoachVonKeoDaiSangNam, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Kế hoạch vốn kéo dài sang năm KHVN không đúng định dạng !", index));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.fKHUT_VonBoTriHetNamTruoc) && !double.TryParse(item.fKHUT_VonBoTriHetNamTruoc, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Vốn bố trí  hết năm trước KHVU không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc) && !double.TryParse(item.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Lũy kế vốn đã thanh toán từ khởi công đến hết năm trước KHVU không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi) && !double.TryParse(item.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Trong đó vốn tạm ứng theo chế độ chưa thu hồi KHVU không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHUT_KeHoachUngTruocKeoDaiSangNam) && !double.TryParse(item.fKHUT_KeHoachUngTruocKeoDaiSangNam, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Kế hoạch ứng trước kéo dài sang năm KHVU không đúng định dạng !", index));
                bIsError = true;
            }
            if (!string.IsNullOrEmpty(item.fKHUT_KeHoachUngTruocChuaThuHoi) && !double.TryParse(item.fKHUT_KeHoachUngTruocChuaThuHoi, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Kế hoạch ứng trc chưa thu hồi KHVU không đúng định dạng !", index));
                bIsError = true;
            }
            return bIsError;
        }

        public JsonResult OnSaveDataImport(VDT_KT_KhoiTao_DuLieu objKhoiTao, List<VdtKtKhoiTaoDuLieuChiTietImportModel> lstData)
        {
            try
            {
                Dictionary<string, Guid> dicMaDuAn = new Dictionary<string, Guid>();
                Dictionary<string, Guid> dicLoaiCongTrinh = new Dictionary<string, Guid>();

                var lstAllDuAn = _qLVonDauTuService.GetVDTDADuAn();
                if (lstAllDuAn != null)
                {
                    foreach (var item in lstAllDuAn)
                    {
                        if (!dicMaDuAn.ContainsKey(item.sMaDuAn)) dicMaDuAn.Add(item.sMaDuAn, item.iID_DuAnID);
                    }
                }
                var lstAllLoaiCongTrinh = _qLVonDauTuService.GetAllDmLoaiCongTrinh();
                if (lstAllLoaiCongTrinh != null)
                {
                    foreach (var item in lstAllLoaiCongTrinh)
                    {
                        if (!dicLoaiCongTrinh.ContainsKey(item.sMaLoaiCongTrinh)) dicLoaiCongTrinh.Add(item.sMaLoaiCongTrinh, item.iID_LoaiCongTrinh);
                    }
                }

                Guid iIdKhoiTao = Guid.NewGuid();
                objKhoiTao.iID_KhoiTaoID = iIdKhoiTao;
                objKhoiTao.dDateCreate = DateTime.Now;
                objKhoiTao.sUserCreate = Username;
                var objDonVi = _qLVonDauTuService.GetDonViQuanLyById(objKhoiTao.iID_DonViID.Value);
                if (objDonVi != null)
                    objKhoiTao.iID_MaDonVi = objDonVi.iID_MaDonVi;
                _qLVonDauTuService.InsertVdtKtKhoiTaoDuLieu(objKhoiTao);

                List<VDT_KT_KhoiTao_DuLieu_ChiTiet> lstGoiThau = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet>();
                foreach (var item in lstData)
                {
                    Guid iId = Guid.NewGuid();
                    VDT_KT_KhoiTao_DuLieu_ChiTiet obj = new VDT_KT_KhoiTao_DuLieu_ChiTiet();
                    obj.iID_KhoiTao_ChiTietID = iId;
                    obj.iID_KhoiTaoDuLieuID = iIdKhoiTao;
                    obj.sMaDuAn = item.sMaDuAn;
                    obj.iID_DuAnID = dicMaDuAn[item.sMaDuAn];
                    obj.iID_LoaiCongTrinh = dicLoaiCongTrinh[item.sMaLoaiCongTrinh];
                    if (!string.IsNullOrEmpty(item.fKHVN_VonBoTriHetNamTruoc))
                        obj.fKHVN_VonBoTriHetNamTruoc = double.Parse(item.fKHVN_VonBoTriHetNamTruoc);
                    if (!string.IsNullOrEmpty(item.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc))
                        obj.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = double.Parse(item.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc);
                    if (!string.IsNullOrEmpty(item.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi))
                        obj.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi = double.Parse(item.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi);
                    if (!string.IsNullOrEmpty(item.fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc))
                        obj.fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc = double.Parse(item.fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc);
                    if (!string.IsNullOrEmpty(item.fKHVN_KeHoachVonKeoDaiSangNam))
                        obj.fKHVN_KeHoachVonKeoDaiSangNam = double.Parse(item.fKHVN_KeHoachVonKeoDaiSangNam);
                    if (!string.IsNullOrEmpty(item.fKHUT_VonBoTriHetNamTruoc))
                        obj.fKHUT_VonBoTriHetNamTruoc = double.Parse(item.fKHUT_VonBoTriHetNamTruoc);
                    if (!string.IsNullOrEmpty(item.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc))
                        obj.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = double.Parse(item.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc);
                    if (!string.IsNullOrEmpty(item.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi))
                        obj.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi = double.Parse(item.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi);
                    if (!string.IsNullOrEmpty(item.fKHUT_KeHoachUngTruocKeoDaiSangNam))
                        obj.fKHUT_KeHoachUngTruocKeoDaiSangNam = double.Parse(item.fKHUT_KeHoachUngTruocKeoDaiSangNam);
                    if (!string.IsNullOrEmpty(item.fKHUT_KeHoachUngTruocChuaThuHoi))
                        obj.fKHUT_KeHoachUngTruocChuaThuHoi = double.Parse(item.fKHUT_KeHoachUngTruocChuaThuHoi);
                    lstGoiThau.Add(obj);
                }
                _qLVonDauTuService.AddRangerVdtKtKhoiTaoDuLieuChiTiet(lstGoiThau);
                return Json(new { bIsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json(new { bIsSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}