using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.CPNH;
using Viettel.Models.QLNH.QuyetToan;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class QLKeHoachChiQuyController : FlexcelReportController
    {
        private QLKeHoachVonNamModel _modelKHV = new QLKeHoachVonNamModel();
        private GiaiNganThanhToanModel _model = new GiaiNganThanhToanModel();
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;
        private const string sFilePath = "/Report_ExcelFrom/VonDauTu/rpt_BaoCaoNhucauKeHoachChiQuy.xlsx";
        private const string sControlName = "QLKeHoachChiQuy";
        private string pathString = System.Configuration.ConfigurationManager.AppSettings["FtpPath"];
        private string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FtpUsername"];
        private string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        // GET: QLVonDauTu/QLKeHoachChiQuy
        public ActionResult Index()
        {
            string sMaNguoiDung = Username;

            List<SelectListItem> lstDataDonVi = new List<SelectListItem>();
            DataTable dt = NganSach_HamChungModels.DSDonViCuaNguoiDung(sMaNguoiDung);
            foreach (DataRow dr in dt.Rows)
            {
                lstDataDonVi.Add(new SelectListItem() { Text = Convert.ToString(dr["TenHT"]), Value = Convert.ToString(dr["iID_MaDonVi"]) });
            }

            lstDataDonVi.Insert(0, new SelectListItem { Text = "--Tất cả--", Value = "" });
            ViewBag.drpDonViQuanLy = lstDataDonVi;

            List<NS_NguonNganSach> lstNguonVon = _vdtService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = "--Tất cả--" });
            ViewBag.drpNguonNganSach = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            List<SelectListItem> lstQuy = new List<SelectListItem> {
                new SelectListItem{Text = "--Tất cả--", Value = ""},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_1, Value=((int)Constants.LoaiQuy.Type.QUY_1).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_2, Value=((int)Constants.LoaiQuy.Type.QUY_2).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_3, Value=((int)Constants.LoaiQuy.Type.QUY_3).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_4, Value=((int)Constants.LoaiQuy.Type.QUY_4).ToString()},
            };
            ViewBag.drpQuy = lstQuy.ToSelectList("Value", "Text");

            KHChiQuyPagingModel data = new KHChiQuyPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _vdtService.GetAllKHNhuCauChiPaging(ref data._paging);

            return View(data);
        }

        public ActionResult Insert()
        {
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);

            List<NS_NguonNganSach> lstNguonVon = _vdtService.LayNguonVon().ToList();
            ViewBag.drpNguonNganSach = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            List<SelectListItem> lstQuy = new List<SelectListItem> {
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_1, Value=((int)Constants.LoaiQuy.Type.QUY_1).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_2, Value=((int)Constants.LoaiQuy.Type.QUY_2).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_3, Value=((int)Constants.LoaiQuy.Type.QUY_3).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_4, Value=((int)Constants.LoaiQuy.Type.QUY_4).ToString()},
            };
            ViewBag.drpQuy = lstQuy.ToSelectList("Value", "Text");

            return View();
        }
        private List<SelectListItem> GetDataDropdownDonViQuanLy(string sMaNguoiDung)
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            DataTable dt = _vdtService.GetDataDropdownDonViQuanLy(sMaNguoiDung, PhienLamViec.NamLamViec);
            foreach (DataRow dr in dt.Rows)
            {
                lstData.Add(new SelectListItem() { Text = Convert.ToString(dr["TenHT"]), Value = Convert.ToString(dr["iId_MaDonVi"]) });
            }
            return lstData;
        }

        public ActionResult Update(Guid id)
        {
            var data = _vdtService.GetNhuCauChiByID(id);

            if (data == null)
                return RedirectToAction("Index");

            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLy = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);

            List<NS_NguonNganSach> lstNguonVon = _vdtService.LayNguonVon().ToList();
            ViewBag.drpNguonNganSach = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            List<SelectListItem> lstQuy = new List<SelectListItem> {
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_1, Value=((int)Constants.LoaiQuy.Type.QUY_1).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_2, Value=((int)Constants.LoaiQuy.Type.QUY_2).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_3, Value=((int)Constants.LoaiQuy.Type.QUY_3).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_4, Value=((int)Constants.LoaiQuy.Type.QUY_4).ToString()},
            };
            ViewBag.drpQuy = lstQuy.ToSelectList("Value", "Text");

            return View(data);
        }

        public ActionResult Detail(Guid id)
        {
            var data = _vdtService.GetNhuCauChiByID(id);

            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        [HttpPost]
        public ActionResult GetModalInBaoCaoChiQuy()
        {
            List<KHChiQuyPagingModel> lstDVTinh = new List<KHChiQuyPagingModel>()
                {
                    new KHChiQuyPagingModel(){sDVTinh = "Đồng", iDVTinh = 1},
                    new KHChiQuyPagingModel(){sDVTinh = "Nghìn", iDVTinh = 1000},
                    new KHChiQuyPagingModel(){sDVTinh = "Triệu", iDVTinh = 1000000},
                    new KHChiQuyPagingModel(){sDVTinh = "Tỷ", iDVTinh = 1000000000000},
                };
            ViewBag.ListDVTinh = lstDVTinh.ToSelectList("iDVTinh", "sDVTinh");
            return PartialView("_modalBaoCao");
        }

        public ActionResult ExportFile(string txtTieuDe1, string txtTieuDe2, long iVND, string iId, string ext = "xlsx")
        {
            string fileName = "Bao cao nhu cau Ke hoach chi quy." + ext;
            Guid iID_NhuCauChiID = new Guid(iId);
            ExcelFile xls = TaoFileExel(txtTieuDe1, txtTieuDe2, iID_NhuCauChiID, iVND);
            return Print(xls, ext, fileName);
        }
        public ExcelFile TaoFileExel(string txtTieuDe1, string txtTieuDe2, Guid iID_NhuCauChiID, long iVND)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));

            if (iID_NhuCauChiID != Guid.Empty)
            {
                var objNhuCauChi = _vdtService.GetNhuCauChiByID(iID_NhuCauChiID);
                if (objNhuCauChi == null)
                    return null;

                List<NcNhuCauChi_ChiTiet> lstNhuCauChiChiTiet = _vdtService.GetNhuCauChiChiTiet(objNhuCauChi.iNamKeHoach ?? 0, objNhuCauChi.iID_MaDonViQuanLy, objNhuCauChi.iID_NguonVonID ?? 0, objNhuCauChi.iQuy ?? 0, iVND);

                List<VDT_NC_NhuCauChi_ChiTiet> detailData = _vdtService.GetNhuCauChiChiTietByNhuCauChiID(iID_NhuCauChiID);
                if (detailData != null)
                {
                    foreach (VDT_NC_NhuCauChi_ChiTiet item in detailData)
                    {
                        NcNhuCauChi_ChiTiet currentData = lstNhuCauChiChiTiet.FirstOrDefault(n => n.iID_DuAnId == item.iID_DuAnId && n.sLoaiThanhToan == item.sLoaiThanhToan);
                        if (currentData != null)
                        {
                            currentData.fGiaTriDeNghi = item.fGiaTriDeNghi ?? 0;
                            currentData.sGhiChu = item.sGhiChu;
                        }
                    }
                }


                int namKeHoach = Convert.ToInt32(objNhuCauChi.iNamKeHoach);
                int idNguonVon = Convert.ToInt32(objNhuCauChi.iID_NguonVonID);
                int quy = Convert.ToInt32(objNhuCauChi.iQuy);
                string idDonVi = Convert.ToString(objNhuCauChi.iID_DonViQuanLyID);
                var dataKinhPhi = GetKinhPhi(namKeHoach, idDonVi, idNguonVon, quy);

                string tempQuyTruocGiaiNgan = String.Format("{0:0,0}", dataKinhPhi.fQuyTruocChuaGiaiNgan);
                string tempQuyNayDuocCap = String.Format("{0:0,0}", dataKinhPhi.fQuyNayDuocCap);
                string tempGiaiNganQuyNay = String.Format("{0:0,0}", dataKinhPhi.fGiaiNganQuyNay);
                string tempChuaGiaiNganChuyenQuySau = String.Format("{0:0,0}", dataKinhPhi.fChuaGiaiNganChuyenQuySau);

                if (tempQuyTruocGiaiNgan == "00") { tempQuyTruocGiaiNgan = "0"; }
                if (tempQuyNayDuocCap == "00") { tempQuyNayDuocCap = "0"; }
                if (tempGiaiNganQuyNay == "00") { tempGiaiNganQuyNay = "0"; }
                if (tempChuaGiaiNganChuyenQuySau == "00") { tempChuaGiaiNganChuyenQuySau = "0"; }

                FlexCelReport fr = new FlexCelReport();
                fr.SetValue("sTenDonVi", objNhuCauChi.sDonViQuanLy);
                fr.SetValue("iQuy", objNhuCauChi.iQuy);
                fr.SetValue("iNam", objNhuCauChi.iNamKeHoach);
                fr.SetValue("sTenNguonVon", objNhuCauChi.sNguonVon);
                fr.SetValue("sNgayThangNam", string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.ToString("dd"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("yyyy")));
                fr.SetValue("soKinhPhi1", tempQuyTruocGiaiNgan);
                fr.SetValue("soKinhPhi2", tempQuyNayDuocCap);
                fr.SetValue("soKinhPhi3", tempGiaiNganQuyNay);
                fr.SetValue("soKinhPhi4", tempChuaGiaiNganChuyenQuySau);

                fr.AddTable<NcNhuCauChi_ChiTiet>("Items", lstNhuCauChiChiTiet);
                fr.UseChuKy(Username)
                     .UseChuKyForController(sControlName)
                     .UseForm(this);

                fr.Run(Result);
            }

            return Result;
        }

        #region Event
        #region PartialView
        [HttpPost]
        public ActionResult KHNhuCauChiQuyView(PagingInfo _paging, string sSoDeNghi, int? iNamKeHoach, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo, string iIDMaDonViQuanLy, int? iIDNguonVon, int? iQuy)
        {
            KHChiQuyPagingModel data = new KHChiQuyPagingModel();
            data._paging = _paging;
            data.lstData = _vdtService.GetAllKHNhuCauChiPaging(ref data._paging, sSoDeNghi, iNamKeHoach, dNgayDeNghiFrom, dNgayDeNghiTo, iIDMaDonViQuanLy, iIDNguonVon, iQuy);

            string sMaNguoiDung = Username;
            List<SelectListItem> lstDataDonVi = new List<SelectListItem>();
            DataTable dt = NganSach_HamChungModels.DSDonViCuaNguoiDung(sMaNguoiDung);
            foreach (DataRow dr in dt.Rows)
            {
                lstDataDonVi.Add(new SelectListItem() { Text = Convert.ToString(dr["TenHT"]), Value = Convert.ToString(dr["iID_MaDonVi"]) });
            }

            lstDataDonVi.Insert(0, new SelectListItem { Text = "--Tất cả--", Value = "" });
            ViewBag.drpDonViQuanLy = lstDataDonVi;

            List<NS_NguonNganSach> lstNguonVon = _vdtService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = "--Tất cả--" });
            ViewBag.drpNguonNganSach = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            List<SelectListItem> lstQuy = new List<SelectListItem> {
                new SelectListItem{Text = "--Tất cả--", Value = ""},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_1, Value=((int)Constants.LoaiQuy.Type.QUY_1).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_2, Value=((int)Constants.LoaiQuy.Type.QUY_2).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_3, Value=((int)Constants.LoaiQuy.Type.QUY_3).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_4, Value=((int)Constants.LoaiQuy.Type.QUY_4).ToString()},
            };
            ViewBag.drpQuy = lstQuy.ToSelectList("Value", "Text");

            return PartialView("_list", data);
        }
        #endregion

        /// <summary>
        /// get kinh phi ctc cap
        /// </summary>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="iID_HopDongID"></param>
        /// <returns></returns>
        public JsonResult GetKinhPhiCucTaiChinhCap(int iNamKeHoach, string iIdDonVi, int iIdNguonVon, int iQuy)
        {
            var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, iIdDonVi).iID_MaDonVi;
            KinhPhiCucTaiChinhCap kinhPhiCTCCap = _vdtService.GetKinhPhiCucTaiChinhCap(iNamKeHoach, sMaDonViQuanLy, iIdNguonVon, iQuy);
            return Json(new { data = kinhPhiCTCCap }, JsonRequestBehavior.AllowGet);
        }

        public KinhPhiCucTaiChinhCap GetKinhPhi(int iNamKeHoach, string iIdDonVi, int iIdNguonVon, int iQuy)
        {
            var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, iIdDonVi).iID_MaDonVi;
            KinhPhiCucTaiChinhCap kinhPhiCTCCap = _vdtService.GetKinhPhiCucTaiChinhCap(iNamKeHoach, sMaDonViQuanLy, iIdNguonVon, iQuy);
            return kinhPhiCTCCap;
        }

        /// <summary>
        /// get nhu cau chi chi tiet
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNhuCauChiChiTiet(int iNamKeHoach, string iIdDonVi, int iIdNguonVon, int iQuy, Guid? iIDNhuCauChiID)
        {
            var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, iIdDonVi).iID_MaDonVi;
            List<NcNhuCauChi_ChiTiet> data = _vdtService.GetNhuCauChiChiTiet(iNamKeHoach, sMaDonViQuanLy, iIdNguonVon, iQuy);
            if (iIDNhuCauChiID != null && iIDNhuCauChiID != Guid.Empty)
            {
                List<VDT_NC_NhuCauChi_ChiTiet> detailData = _vdtService.GetNhuCauChiChiTietByNhuCauChiID(iIDNhuCauChiID.Value);
                if (detailData != null)
                {
                    foreach (VDT_NC_NhuCauChi_ChiTiet item in detailData)
                    {
                        NcNhuCauChi_ChiTiet currentData = data.FirstOrDefault(n => n.iID_DuAnId == item.iID_DuAnId && n.sLoaiThanhToan == item.sLoaiThanhToan);
                        if (currentData != null)
                        {
                            currentData.fGiaTriDeNghi = item.fGiaTriDeNghi ?? 0;
                            currentData.sGhiChu = item.sGhiChu;
                        }
                    }
                }
            }
            return Json(this.RenderRazorViewToString("_list_nhucauchi_chitiet", data), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Process
        [HttpPost]
        public JsonResult LuuKeHoachChiQuy(VDT_NC_NhuCauChi data, List<VDT_NC_NhuCauChi_ChiTiet> lstChiTiet, bool? isUpdate)
        {
            if (isUpdate != null)
            {
                if (_vdtService.LuuNhuCauChi(data, lstChiTiet, Username, isUpdate))
                {
                    return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (data != null || lstChiTiet.Any())
                {
                    if (_vdtService.LuuNhuCauChi(data, lstChiTiet, Username, null))
                    {
                        return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                
                return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult ValidationKHChiQuy(int iNamKeHoach, string iIdDonVi, int iIdNguonVon, int iQuy, string sSoDeNghi, string sNguoiLap)
        {
            
            if (sSoDeNghi == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Số đề nghị không được để trống." }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                if (sSoDeNghi.Length > 50)
                {
                    return Json(new { bIsComplete = false, sMessError = "Số đề nghị không được quá 50 ký tự." }, JsonRequestBehavior.AllowGet);
                }
            }

            if (sNguoiLap != null && sNguoiLap.Length > 255)
            {
                return Json(new { bIsComplete = false, sMessError = "Số đề nghị không được quá 255 ký tự." }, JsonRequestBehavior.AllowGet);
            }

            var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, iIdDonVi).iID_MaDonVi;
            VDT_NC_NhuCauChi objNhuCauChi = _vdtService.GetNhuCauChiByiNamKeHoachMaDonVi(iNamKeHoach, sMaDonViQuanLy, iIdNguonVon, iQuy);
            if (objNhuCauChi != null)
            {
                return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool XoaKeHoachChiQuy(Guid id)
        {
            if (!_vdtService.XoaNhuCauChi(id)) return false;
            return true;
        }

        #endregion

        #region Import, Export
        public ActionResult ImportData()
        {
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLyImport = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);

            List<SelectListItem> lstQuy = new List<SelectListItem> {
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_1, Value=((int)Constants.LoaiQuy.Type.QUY_1).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_2, Value=((int)Constants.LoaiQuy.Type.QUY_2).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_3, Value=((int)Constants.LoaiQuy.Type.QUY_3).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_4, Value=((int)Constants.LoaiQuy.Type.QUY_4).ToString()},
            };
            ViewBag.drpQuyImport = lstQuy.ToSelectList("Value", "Text");
            FileFtpModel data = new FileFtpModel();
            return PartialView("_modalDownloadFile", data);
        }

        [HttpPost]
        public ActionResult GetGridListExcelFromFTP(string idDonVi, string nam, string quy)
        {
            string sMaNguoiDung = Username;
            ViewBag.drpDonViQuanLyImport = _modelKHV.GetDataDropdownDonViQuanLy(sMaNguoiDung);

            List<SelectListItem> lstQuy = new List<SelectListItem> {
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_1, Value=((int)Constants.LoaiQuy.Type.QUY_1).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_2, Value=((int)Constants.LoaiQuy.Type.QUY_2).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_3, Value=((int)Constants.LoaiQuy.Type.QUY_3).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_4, Value=((int)Constants.LoaiQuy.Type.QUY_4).ToString()},
            };
            ViewBag.drpQuyImport = lstQuy.ToSelectList("Value", "Text");
            FileFtpModel data = new FileFtpModel();
            try
            {
                var lstResponse = new List<string>();
                DateTime CrTime = DateTime.Now;
                string tenDonVi = "0";
                NS_DonVi donVi = _iNganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), idDonVi);
                if (donVi != null)
                {
                    tenDonVi = donVi.iID_MaDonVi;
                }
                pathString = pathString + "/" + tenDonVi + "/VDT/QLKeHoachChiQuy/send/" + nam.Trim() + "/" + quy.Trim() + "/";
                var r = (FtpWebRequest)WebRequest.Create(pathString);
                r.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                r.Method = WebRequestMethods.Ftp.ListDirectory;
                using (var response = (FtpWebResponse)r.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                lstResponse.Add(reader.ReadLine());
                            }
                        }
                    }
                }
                if (lstResponse.Count != 0)
                {
                    int iIndex = 1;
                    List<FileFtp> lstFile = new List<FileFtp>();
                    foreach (var line in lstResponse)
                    {
                        FileFtp item = new FileFtp();
                        item.IStt = iIndex;
                        item.BIsCheck = false;
                        item.SNameFile = line;
                        item.SUrl = Path.Combine(pathString, line);
                        lstFile.Add(item);
                        iIndex++;
                    }
                    data.Items = lstFile;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_modalDownloadFile", data);
        }
        public ActionResult DownloadFileExcel(string url)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            responseStream.CopyTo(stream);
                            byte[] file_array = stream.ToArray();
                            return File(file_array, "application/vnd.ms-excel", $"Bao cao nhu cau Ke hoach chi quy.xlsx");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index");
        }
        public ActionResult XuatFile(Guid id)
        {
            ExcelFile excel = TaoFile(id);
            if (excel != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    excel.Save(stream);
                    return File(stream.ToArray(), "application/vnd.ms-excel", $"Bao cao nhu cau Ke hoach chi quy.xlsx");
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SendFile(Guid id, string idDonVi, string nam, string quy)
        {
            ExcelFile excel = TaoFile(id);
            if (excel != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    excel.Save(stream);
                    SaveFile(stream.ToArray(), "Bao cao nhu cau Ke hoach chi quy.xlsx", idDonVi, nam, quy);
                }
            }
            return RedirectToAction("Index");
        }

        public ExcelFile TaoFile(Guid iID_NhuCauChiID)
        {
            var objNhuCauChi = _vdtService.GetNhuCauChiByID(iID_NhuCauChiID);
            if (objNhuCauChi == null)
                return null;

            List<NcNhuCauChi_ChiTiet> lstNhuCauChiChiTiet = _vdtService.GetNhuCauChiChiTiet(objNhuCauChi.iNamKeHoach ?? 0, objNhuCauChi.iID_MaDonViQuanLy, objNhuCauChi.iID_NguonVonID ?? 0, objNhuCauChi.iQuy ?? 0);

            List<VDT_NC_NhuCauChi_ChiTiet> detailData = _vdtService.GetNhuCauChiChiTietByNhuCauChiID(iID_NhuCauChiID);
            if (detailData != null)
            {
                foreach (VDT_NC_NhuCauChi_ChiTiet item in detailData)
                {
                    NcNhuCauChi_ChiTiet currentData = lstNhuCauChiChiTiet.FirstOrDefault(n => n.iID_DuAnId == item.iID_DuAnId && n.sLoaiThanhToan == item.sLoaiThanhToan);
                    if (currentData != null)
                    {
                        currentData.fGiaTriDeNghi = item.fGiaTriDeNghi ?? 0;
                        currentData.sGhiChu = item.sGhiChu;
                    }
                }
            }

            var dataKinhPhi = GetKinhPhi((int)objNhuCauChi.iNamKeHoach, Convert.ToString( objNhuCauChi.iID_DonViQuanLyID), (int)objNhuCauChi.iID_NguonVonID, (int)objNhuCauChi.iQuy);

            string tempQuyTruocGiaiNgan = String.Format("{0:0,0}", dataKinhPhi.fQuyTruocChuaGiaiNgan);
            string tempQuyNayDuocCap = String.Format("{0:0,0}", dataKinhPhi.fQuyNayDuocCap);
            string tempGiaiNganQuyNay = String.Format("{0:0,0}", dataKinhPhi.fGiaiNganQuyNay);
            string tempChuaGiaiNganChuyenQuySau = String.Format("{0:0,0}", dataKinhPhi.fChuaGiaiNganChuyenQuySau);

            if (tempQuyTruocGiaiNgan == "00") { tempQuyTruocGiaiNgan = "0"; }
            if (tempQuyNayDuocCap == "00") { tempQuyNayDuocCap = "0"; }
            if (tempGiaiNganQuyNay == "00") { tempGiaiNganQuyNay = "0"; }
            if (tempChuaGiaiNganChuyenQuySau == "00") { tempChuaGiaiNganChuyenQuySau = "0"; }


            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));

            FlexCelReport fr = new FlexCelReport();
            fr.SetValue("sTenDonVi", objNhuCauChi.sDonViQuanLy);
            fr.SetValue("iQuy", objNhuCauChi.iQuy);
            fr.SetValue("iNam", objNhuCauChi.iNamKeHoach);
            fr.SetValue("sTenNguonVon", objNhuCauChi.sNguonVon);
            fr.SetValue("sNgayThangNam", string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.ToString("dd"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("yyyy")));
            fr.SetValue("soKinhPhi1", tempQuyTruocGiaiNgan);
            fr.SetValue("soKinhPhi2", tempQuyNayDuocCap);
            fr.SetValue("soKinhPhi3", tempGiaiNganQuyNay);
            fr.SetValue("soKinhPhi4", tempChuaGiaiNganChuyenQuySau);

            fr.AddTable<NcNhuCauChi_ChiTiet>("Items", lstNhuCauChiChiTiet);
            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this);
            fr.Run(Result);
            return Result;
        }
        public ActionResult SaveFile(byte[] contentFile, string filename, string idDonVi, string nam, string quy)
        {
            string tenDonVi = "0";
            NS_DonVi donVi = _iNganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), idDonVi);
            if (donVi != null)
            {
                tenDonVi = donVi.iID_MaDonVi;
            }
            string folder = "VDT/QLKeHoachChiQuy/send/";
            string uri = pathString + "/" + tenDonVi + "/" + folder + nam.Trim() + "/" + quy.Trim();
            //var path = string.Format("{0}\\{1}", pathString, CommonFunction.UCS2Convert(filename).Replace(" ", ""));
            DateTime crTime = DateTime.Now;
            string path = uri + "/" + crTime.ToString("ddMMyyyyhhmmssfff") + "_" + filename;
            FtpWebRequest reqFTP = null;
            try
            {
                Stream ftpStream = null;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                //directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
            }
            try
            {
                FtpWebRequest reqFile = (FtpWebRequest)FtpWebRequest.Create(path);
                reqFile.Method = WebRequestMethods.Ftp.UploadFile;
                reqFile.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                using (Stream requestStream = reqFile.GetRequestStream())
                {
                    requestStream.Write(contentFile, 0, contentFile.Length);
                    requestStream.Close();
                }
            }
            catch (Exception ex)
            {
                //directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
            }
            try
            {
                VDT_FtpRoot rootInfo = new VDT_FtpRoot();
                rootInfo = _vdtService.GetFtpRoot(tenDonVi, pathString, folder);
                if (rootInfo == null)
                {
                    rootInfo = new VDT_FtpRoot();
                    rootInfo.Id = Guid.NewGuid();
                    rootInfo.sMaDonVi = tenDonVi;
                    rootInfo.sIpAddress = pathString;
                    rootInfo.sFolderRoot = folder;
                    rootInfo.sNguoiTao = Username;
                    rootInfo.dNgayTao = DateTime.Now;
                    _vdtService.InsertFtpRoot(rootInfo);
                }
                var fileInfo = new VDT_FtpFile();
                fileInfo.Id = Guid.NewGuid();
                fileInfo.iID_FtpRoot = rootInfo.Id;
                fileInfo.sFileName = crTime.ToString("ddMMyyyyhhmmssfff") + "_" + filename;
                fileInfo.sRootPath = path;
                fileInfo.sNguoiTao = Username;
                fileInfo.dNgayTao = DateTime.Now;
                _vdtService.InsertFtpFile(fileInfo);
            }
            catch (Exception ex)
            {

            }
            return Json("");
        }
        #endregion
    }
}