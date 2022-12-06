using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Controllers;
using VIETTEL.Models;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using FlexCel.Core;
using FlexCel.Render;
using System.IO;
using VIETTEL.Helpers;
using System.Net;
using VIETTEL.Common;
using VIETTEL.Flexcel;
using static VTS.QLNS.CTC.App.Service.UserFunction.FormatNumber;
using VTS.QLNS.CTC.App.Service.UserFunction;
using System.Linq.Expressions;
using Viettel;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class KeHoachVonNamPhanBoVonDVPheDuyetController : FlexcelReportController
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private static List<NS_MucLucNganSach> _lstMucLucNganSach = new List<NS_MucLucNganSach>();
        private static List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet> _lstPhanBoVonDVChiTiet = new List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>();
        private const int KHOI_CONG_MOI = 1;
        private const int CHUYEN_TIEP = 2;
        private string pathString = System.Configuration.ConfigurationManager.AppSettings["FtpPath"];
        private string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FtpUsername"];
        private string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        private const string sControlName = "KeHoachVonNamPhanBoVonDVPheDuyet";
        private const string RPT_DONVI = "2";
        private const string RPT_GOC = "1";
        private const string RPT_CONGTRINH_MOMOI = "1";
        private const string RPT_CONGTRINH_CHUYENTIEP = "3";

        // GET: QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet
        public ActionResult Index()
        {
            KeHoachPhanBoVonDonViPheDuyetViewModel vm = new KeHoachPhanBoVonDonViPheDuyetViewModel();

            try
            {
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");

                vm._paging.CurrentPage = 1;
                vm.Items = _qLVonDauTuService.GetAllKeHoachVonNamDonViPheDuyet(ref vm._paging);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult KeHoachVonNamPhanBoVonDVPheDuyetSearch(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, int? iNamKeHoach, int? iID_NguonVonID, Guid? iID_DonViQuanLyID)
        {
            KeHoachPhanBoVonDonViPheDuyetViewModel vm = new KeHoachPhanBoVonDonViPheDuyetViewModel();

            try
            {
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");

                vm._paging.CurrentPage = 1;
                vm._paging = _paging;
                vm.Items = _qLVonDauTuService.GetAllKeHoachVonNamDonViPheDuyet(ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id, Guid? idDonViQuanLy, bool isModified, bool isView)
        {
            VDT_KHV_PhanBoVon_DonVi_PheDuyet data = new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
            try
            {
                if (id.HasValue)
                {
                    data = _qLVonDauTuService.GetKeHoachVonPBVDonViPheDuyetById(id);
                }
                else
                {
                    data.iNamKeHoach = DateTime.Now.Year;
                }

                var lstDonVi = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                ViewBag.ListDonViQuanLy = lstDonVi;

                if (idDonViQuanLy.HasValue && idDonViQuanLy != Guid.Empty)
                {
                    data.iID_DonViQuanLyID = idDonViQuanLy;
                }
                else if (data.Id == null || data.Id == Guid.Empty)
                {
                    data.iID_DonViQuanLyID = lstDonVi.Count() > 0 ? Guid.Parse(lstDonVi.FirstOrDefault().Value) : Guid.Empty;
                }

                ViewBag.IsModified = isModified ? "true" : "false";
                ViewBag.IsView = isView ? "true" : "false";
                ViewBag.ListVoucherSuggestion = _qLVonDauTuService.GetAllKeHoachVonNamDeXuatByIdDonVi(data.iID_DonViQuanLyID).ToSelectList("iID_KeHoachVonNamDeXuatID", "sSoQuyetDinh");
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("InsertAndUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDTKHVPhanBoVonDonViPheDuyetViewModel data = new VDTKHVPhanBoVonDonViPheDuyetViewModel();
            try
            {
                data = _qLVonDauTuService.GetKeHoachVonPBVDonViPheDuyetByIdDetail(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public ActionResult KeHoachVonNamPhanBoVonDVPheDuyetDelete(Guid id)
        {
            bool status = false;
            try
            {
                status = _qLVonDauTuService.DeleteKeHoachVonNamPhanBoVonDVPheDuyet(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult KeHoachVonNamPhanBoVonDVPheDuyetSave(VDT_KHV_PhanBoVon_DonVi_PheDuyet data, bool isModified)
        {
            Guid iID = Guid.Empty;
            try
            {
                if (data.Id == null || data.Id == Guid.Empty)
                {
                    if (_qLVonDauTuService.CheckExistSoKeHoachVonNamPhanBoVonDVPheDuyet(data.sSoQuyetDinh, data.iNamKeHoach.Value, null))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (_qLVonDauTuService.CheckExistSoKeHoachVonNamPhanBoVonDVPheDuyet(data.sSoQuyetDinh, data.iNamKeHoach.Value, data.Id) && !isModified)
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                /*
                if (_qLVonDauTuService.CheckExistKeHoachVonNamPhanBoVonDVPheDuyet(data.iID_DonViQuanLyID.ToString(), data.iID_NguonVonID ?? 0, data.iNamKeHoach ?? 0, data.Id))
                {
                    //var objDonVi = _nganSachService.GetDonViById(PhienLamViec.iNamLamViec, data.iID_MaDonViQuanLy);
                    var objDonVi = _nganSachService.GetDonViById(PhienLamViec.iNamLamViec, data.iID_DonViQuanLyID.ToString());
                    var objNguonVon = _qLVonDauTuService.GetListAllNguonNganSach().FirstOrDefault(n => n.iID_MaNguonNganSach == data.iID_NguonVonID);
                    var strDonVi = string.Format("{0} - {1}", objDonVi.iID_MaDonVi, objDonVi.sTen);
                    return Json(new
                    {
                        bIsComplete = false,
                        sMessError = string.Format("Đơn vị {0} và nguồn vốn {1} trong năm kế hoạch {2} đã tồn tại.", strDonVi, objNguonVon.sTen, data.iNamKeHoach)
                    },
                        JsonRequestBehavior.AllowGet);
                }
                */
                if (!_qLVonDauTuService.SaveKeHoachVonNamPhanBoVonDVPheDuyet(ref iID, data, Username, isModified))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !", iID = data.Id }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true, iID = iID }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(Guid id, bool isViewDetail = false)
        {
            VDT_KHV_PhanBoVon_DonVi_PheDuyet data = _qLVonDauTuService.GetKeHoachVonNamPhanBoVonDonViPheDuyetById(id);
            KeHoachVonNamPhanBoVonDonViPheDuyetChiTietGridViewModel vm = new KeHoachVonNamPhanBoVonDonViPheDuyetChiTietGridViewModel
            {
                KHVonNamPhanBoVon_DonVi_PheDuyet = data
            };
            
            List<VDT_KHV_KeHoachVonNam_DeXuat> lstAggregate = new List<VDT_KHV_KeHoachVonNam_DeXuat>();
            if (data != null && data.Id != Guid.Empty)
            {
                lstAggregate = _qLVonDauTuService.GetKeHoachVonNamDeXuatTongHopByCondition(data.iNamKeHoach.Value, data.iID_DonViQuanLyID,data.iID_NguonVonID).ToList();
            }

            ViewBag.LstVoucherAggregate = lstAggregate.ToSelectList("iID_KeHoachVonNamDeXuatID", "sSoQuyetDinh");
            ViewBag.IsViewDetail = isViewDetail;
            return View(vm);
        }

        [HttpGet]
        public ActionResult SheetFrame(string id, string idParent, string filter = null, bool isActive = true, string idDx = "")
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);

            var sheet = new KeHoachVonNamPhanBoVonDonViPheDuyetChiTietSheetTable(id, idParent, PhienLamViec.NamLamViec, filters, idDx);

            var vm = new KeHoachVonNamPhanBoVonDonViPheDuyetChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "KeHoachVonNamPhanBoVonDVPheDuyet", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrame", "KeHoachVonNamPhanBoVonDVPheDuyet", new { area = "QLVonDauTu" })
                 ),
            };

            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            ViewBag.modelActive = isActive;
            Guid idDxConvert = Guid.Empty;
            Guid.TryParse(idDx, out idDxConvert);
            if (vm.KHVonNamPhanBoVon_DonVi_PheDuyet == null)
                vm.KHVonNamPhanBoVon_DonVi_PheDuyet = new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
            if (idDx != null)
            {
                Guid.TryParse(idDx, out idDxConvert);
                vm.KHVonNamPhanBoVon_DonVi_PheDuyet.iID_VonNamDeXuatID = idDxConvert;
            }
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var voucherId = string.Empty;
            var voucherParent = string.Empty;
            bool isActive = true;
            Guid iID_KeHoachVonNamDeXuatID = Guid.Empty;
            if (TempData["iIDKHVNDX"] != null)
                iID_KeHoachVonNamDeXuatID = (Guid)TempData["iIDKHVNDX"];

            try
            {
                var afterSave = _qLVonDauTuService.SaveKHVonNamDonViPheDuyetChiTiet(_lstPhanBoVonDVChiTiet, iID_KeHoachVonNamDeXuatID);
                voucherId = _lstPhanBoVonDVChiTiet.FirstOrDefault().iID_PhanBoVon_DonVi_PheDuyet_ID.ToString();
                VDT_KHV_PhanBoVon_DonVi_PheDuyet item = _qLVonDauTuService.GetKeHoachVonNamPhanBoVonDonViPheDuyetById(Guid.Parse(voucherId));
                if (afterSave && _lstPhanBoVonDVChiTiet.Count() > 0)
                {
                    voucherParent = item.iID_ParentId.ToString();
                    isActive = item.bActive.Value;
                }

                _lstPhanBoVonDVChiTiet = new List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>();
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return SheetFrame(voucherId, voucherParent, null, isActive, string.Empty);
        }

        [HttpPost]
        public JsonResult ValidateBeforeSave(List<VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel> aListModel, string iID_KeHoachVonNamDeXuatID)
        {
            var listErrMess = new List<string>();
            TempData["iIDKHVNDX"] = Guid.Parse(iID_KeHoachVonNamDeXuatID);
            if (aListModel != null && aListModel.Any())
            {
                var listVonNamPheDuyetChiTietIds = aListModel.Select(model => model.iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID).ToList();
                _lstPhanBoVonDVChiTiet = aListModel.Select(model => new
                    VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet
                    ()
                {
                    Id = model.iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
                    iID_PhanBoVon_DonVi_PheDuyet_ID = model.iID_PhanBoVon_DonVi_PheDuyet_ID,
                    iID_DuAnID = model.iID_DuAnID,
                    fGiaTriPhanBo = model.fGiaTriPhanBo,
                    fGiaTriThuHoi = model.fGiaTriThuHoi,
                    iID_DonViTienTeID = model.iID_DonViTienTeID,
                    iID_TienTeID = model.iID_TienTeID,
                    fTiGiaDonVi = model.fTiGiaDonVi,
                    fTiGia = model.fTiGia,
                    iID_LoaiCongTrinh = model.iID_LoaiCongTrinh,
                    iId_Parent = model.iID_Parent,
                    bActive = model.bActive,
                    ILoaiDuAn = model.iLoaiDuAn,
                    sGhiChu = model.sGhiChu,     
                    fGiaTriDeNghi = model.fGiaTriDeNghi,
                }).ToList();
                
                //if (!_qLVonDauTuService.checkExistDonViVonNamPheDuyetChiTiet(listVonNamPheDuyetChiTietIds))
                //{
                //    listErrMess.Add("Không tồn tại bản ghi vốn năm phê duyệt chi tiết");
                //}
            }

            if (listErrMess != null && listErrMess.Any())
            {
                return Json(new { status = false, listErrMess = listErrMess }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KeHoachVonNamPhanBoVonDVPheDuyetExport(Guid? idKeHoachVonNamDuocDuyet)
        {
            try
            {
                List<VDTKHVPhanBoVonDuocDuyetViewModel> dataReport = new List<VDTKHVPhanBoVonDuocDuyetViewModel>();
                List<VDTKHVPhanBoVonDuocDuyetChiTietViewModel> dataDetailReport = new List<VDTKHVPhanBoVonDuocDuyetChiTietViewModel>();

                dataReport = _qLVonDauTuService.GetKeHoachVonNamExport(idKeHoachVonNamDuocDuyet.ToString()).ToList();
                dataReport.Select((x, index) => { x.STT = (index + 1).ToString(); return x; }).ToList();
                dataDetailReport = _qLVonDauTuService.GetKeHoachVonNamChiTietExport(idKeHoachVonNamDuocDuyet.ToString()).ToList();
                dataDetailReport.Select((x, index) => { x.STT = (index + 1).ToString(); return x; }).ToList();

                ExcelFile xls = CreateReport(dataReport, dataDetailReport);
                xls.PrintLandscape = true;

                TempData["DataReport"] = xls;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult ExportExcel(string idDonVi, string nam)
        {
            ExcelFile xls = (ExcelFile)TempData["DataReport"];
            string sFileName = "KeHoachVonNamDuocDuyet.xlsx";
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                //SaveFile(stream.ToArray(), sFileName, idDonVi, nam);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"KeHoachVonNamDuocDuyet.xlsx");
            }
        }
        public ActionResult SaveFile(byte[] contentFile, string filename, string idDonVi, string nam)
        {
            string tenDonVi = "0";
            NS_DonVi donVi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), idDonVi);
            if (donVi != null)
            {
                tenDonVi = donVi.iID_MaDonVi;
            }
            pathString = pathString + "/" + tenDonVi + "/VDT/KeHoachVonNamDuocDuyet/receive/" + nam.Trim();
            //var path = string.Format("{0}\\{1}", pathString, CommonFunction.UCS2Convert(filename).Replace(" ", ""));
            DateTime crTime = DateTime.Now;
            string path = pathString + "/" + crTime.ToString("ddMMyyyyhhmmssfff") + "_" + filename;
            FtpWebRequest reqFTP = null;
            try
            {
                Stream ftpStream = null;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(pathString);
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
            return Json("");
        }

        [HttpGet]
        public ActionResult ViewInBaoCao()
        {
            VDTKeHoachVonNamDuocDuyetExport vm = new VDTKeHoachVonNamDuocDuyetExport();
            try
            {
                vm.lstDonViQuanLy = new List<NS_DonVi>();
                string[] lstDonViInclude = new string[] { "0", "1" };
                IEnumerable<NS_DonVi> lstDonViQuanLy = _qLVonDauTuService.GetDonviList(PhienLamViec.NamLamViec);
                if(lstDonViQuanLy != null)
                {
                    vm.lstDonViQuanLy = lstDonViQuanLy.Where(n => (n.iTrangThai ?? 1) == 1);
                }

                IEnumerable<NS_NguonNganSach> lstDMNguonNganSach = _qLVonDauTuService.GetListDMNguonNganSach();
                ViewBag.lstNguonVon = lstDMNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");

                List<ComBoBoxItem> lstDonViTinh = new List<ComBoBoxItem>()
                {
                    new ComBoBoxItem(){DisplayItem = "Đồng", ValueItem = "1"},
                    new ComBoBoxItem(){DisplayItem = "Nghìn đống", ValueItem = "1000"},
                    new ComBoBoxItem(){DisplayItem = "Triệu đồng", ValueItem = "1000000"},
                    new ComBoBoxItem(){DisplayItem = "Tỷ đồng", ValueItem = "1000000000"},
                };

                ViewBag.lstDonViTinh = lstDonViTinh.ToSelectList("ValueItem", "DisplayItem");
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }

        [HttpPost]
        public JsonResult PrintBaoCao(VDTKeHoachVonNamDuocDuyetExport dataReport, List<string> arrIdDVQL, bool isPdf)
        {
            try
            {
                XlsFile Result = new XlsFile(true);
                FlexCelReport fr = new FlexCelReport();
                string templateFileName = string.Empty;

                if (dataReport.sLoaiChungTu == RPT_GOC)
                {
                    if(dataReport.sNguonVon == "1")
                    {
                        fr = HandleDataExportChungTuGocNSQP(dataReport, arrIdDVQL);
                        if (dataReport.sLoaiCongTrinh == RPT_CONGTRINH_MOMOI)
                        {
                            templateFileName = "rpt_BaoCao_KHNam_DonVi_PheDuyet_CTMM.xlsx";
                        }
                        else
                        {
                            templateFileName = "rpt_BaoCao_KHNam_DonVi_PheDuyet_CTCT.xlsx";
                        }
                    }
                    else
                    {
                        fr = HandleDataExportBudgetOther(dataReport, arrIdDVQL);
                        templateFileName = "rpt_BaoCao_KHNam_DonVi_PheDuyet_NguonVonKhac.xlsx";
                    }
                }
                else
                {
                    if (dataReport.sNguonVon == "1")
                    {
                        fr = HandleDataExportDieuChinh(dataReport, arrIdDVQL);
                        templateFileName = "rpt_BaoCao_KHNam_DonVi_PheDuyet_DieuChinh.xlsx";
                    }
                    else
                    {
                        fr = HandleDataExportBudgetOtherDieuChinh(dataReport, arrIdDVQL);
                        templateFileName = "rpt_BaoCao_KHNam_DeXuat_DieuChinh_NguonVonKhac.xlsx";
                    }
                }

                Result.Open(Server.MapPath(Path.Combine("~/Areas/QLVonDauTu/ReportExcelForm", templateFileName)));
                FormatNumber formatNumber = new FormatNumber(int.Parse(dataReport.sValueDonViTinh), (isPdf ? ExportType.PDF : ExportType.EXCEL));
                string sTenDonVi = GetNameUnit(dataReport);
                if (!string.IsNullOrEmpty(sTenDonVi))
                {
                    sTenDonVi = sTenDonVi.ToUpper();
                }
                if(fr == null)
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
                fr.SetUserFunction("FormatNumber", formatNumber);
                fr.SetValue("Year", dataReport.iNamLamViec);
                fr.SetValue("TitleFirst", dataReport.txtHeader1);
                fr.SetValue("TitleSecond", dataReport.txtHeader2);
                fr.SetValue("DonViTinh", dataReport.sDonViTinh);
                fr.SetValue("DonVi", sTenDonVi);
                fr.SetValue("DonViCapTren", "BỘ QUỐC PHÒNG");
                fr.UseChuKy(Username).UseChuKyForController(sControlName).UseForm(this);
                fr.Run(Result);
                Result.PrintLandscape = false;
                TempData["DataReportGocXls"] = Result;
                FlexCelPdfExport pdf = new FlexCelPdfExport(Result, true);
                var bufferPdf = new MemoryStream();
                pdf.Export(bufferPdf);
                TempData["DataReportGoc"] = bufferPdf;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true, isPdf = isPdf, isGoc = dataReport.sLoaiChungTu.Equals("1") ? true : false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportReport(bool pdf)
        {
            ExcelFile xls = (ExcelFile)TempData["DataReportGocXls"];

            return Print(xls, pdf ? "pdf" : "xls", pdf ? "BaoCaoKeHoachVonNamDuocDuyet.pdf" : "BaoCaoKeHoachVonNamDuocDuyet.xlsx");
        }

        #region Helper
        public ExcelFile CreateReportDieuChinh(List<KeHoachVonNamDuocDuyetExportDieuChinh> dataDetailReport, VDTKeHoachVonNamDuocDuyetExport parameter)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rptBaoCaoKeHoachVonNamDuocDuyetDieuChinh.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<KeHoachVonNamDuocDuyetExportDieuChinh>("Items", dataDetailReport);
            fr.SetValue("Year", parameter.iNamLamViec);
            fr.SetValue("TitleFirst", parameter.txtHeader1);
            fr.SetValue("TitleSecond", parameter.txtHeader2);
            fr.SetValue("DonVi", string.Empty);
            fr.SetValue("DonViTinh", parameter.sDonViTinh);
            fr.UseChuKy(Username)
                    .UseChuKyForController(sControlName)
                    .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        public ExcelFile CreateReportGoc(List<VDTKeHoachVonNamDuocDuyetExport> dataDetailReport, VDTKeHoachVonNamDuocDuyetExport parameter)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rptBaoCaoKeHoachVonNamDuocDuyet.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VDTKeHoachVonNamDuocDuyetExport>("Items", dataDetailReport);
            fr.SetValue("Year", parameter.iNamLamViec);
            fr.SetValue("Header1", parameter.txtHeader1);
            fr.SetValue("Header2", parameter.txtHeader2);
            fr.SetValue("DonViTinh", parameter.sDonViTinh);
            fr.UseChuKy(Username)
                    .UseChuKyForController(sControlName)
                    .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        public ExcelFile CreateReport(List<VDTKHVPhanBoVonDuocDuyetViewModel> dataReport, List<VDTKHVPhanBoVonDuocDuyetChiTietViewModel> dataDetailReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rpt_VDT_KeHoachVonNamDuocDuyet.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VDTKHVPhanBoVonDuocDuyetViewModel>("Voucher", dataReport);
            fr.AddTable<VDTKHVPhanBoVonDuocDuyetChiTietViewModel>("VoucherDetail", dataDetailReport);
            fr.UseChuKy(Username)
                    .UseChuKyForController(sControlName)
                    .UseForm(this);
            fr.Run(Result);
            return Result;
        }

        private List<KeHoachVonNamDuocDuyetExportDieuChinh> CalculateDataDieuChinhReport(List<KeHoachVonNamDuocDuyetExportDieuChinh> data)
        {
            List<KeHoachVonNamDuocDuyetExportDieuChinh> childrent = data.Where(x => x.Loai.Equals(2)).ToList();
            List<KeHoachVonNamDuocDuyetExportDieuChinh> parent = data.Where(x => x.Loai.Equals(1)).ToList();

            data.Where(x => x.Loai.Equals(1)).Select(x =>
            {
                x.TongSoVonDauTu = 0;
                x.TongSoVonDauTuTrongNuoc = 0;
                x.KeHoachVonDauTuGiaiDoan = 0;
                x.VonThanhToanLuyKe = 0;
                x.TongSoKeHoachVonNam = 0;
                x.ThuHoiVonDaUngTruoc = 0;
                x.VonThucHienTuDauNamDenNay = 0;
                x.TongSoVonNamDieuChinh = 0;
                x.ThuHoiVonDaUngTruocDieuChinh = 0;
                x.TraNoXDCB = 0;
                return x;
            }).ToList();

            foreach (var pr in parent)
            {
                List<KeHoachVonNamDuocDuyetExportDieuChinh> lstChilrent = childrent.Where(x => x.IdNhomDuAn.Equals(pr.IdNhomDuAn)).ToList();
                foreach (var item in lstChilrent.Where(x => (x.TongSoVonDauTu != 0 || x.TongSoVonDauTuTrongNuoc != 0 || x.KeHoachVonDauTuGiaiDoan != 0
                                                            || x.VonThanhToanLuyKe != 0 || x.TongSoKeHoachVonNam != 0 || x.ThuHoiVonDaUngTruoc != 0
                                                            || x.VonThucHienTuDauNamDenNay != 0 || x.TongSoVonNamDieuChinh != 0
                                                            || x.ThuHoiVonDaUngTruocDieuChinh != 0 || x.TraNoXDCB != 0)))
                {
                    pr.TongSoVonDauTu += item.TongSoVonDauTu.HasValue ? item.TongSoVonDauTu.Value : 0;
                    pr.TongSoVonDauTuTrongNuoc += item.TongSoVonDauTuTrongNuoc.HasValue ? item.TongSoVonDauTuTrongNuoc.Value : 0;
                    pr.KeHoachVonDauTuGiaiDoan += item.KeHoachVonDauTuGiaiDoan.HasValue ? item.KeHoachVonDauTuGiaiDoan.Value : 0;
                    pr.VonThanhToanLuyKe += item.VonThanhToanLuyKe.HasValue ? item.VonThanhToanLuyKe.Value : 0;
                    pr.TongSoKeHoachVonNam += item.TongSoKeHoachVonNam.HasValue ? item.TongSoKeHoachVonNam.Value : 0;
                    pr.ThuHoiVonDaUngTruoc += item.ThuHoiVonDaUngTruoc.HasValue ? item.ThuHoiVonDaUngTruoc.Value : 0;
                    pr.VonThucHienTuDauNamDenNay += item.VonThucHienTuDauNamDenNay.HasValue ? item.VonThucHienTuDauNamDenNay.Value : 0;
                    pr.TongSoVonNamDieuChinh += item.TongSoVonNamDieuChinh.HasValue ? item.TongSoVonNamDieuChinh.Value : 0;
                    pr.ThuHoiVonDaUngTruocDieuChinh += item.ThuHoiVonDaUngTruocDieuChinh.HasValue ? item.ThuHoiVonDaUngTruocDieuChinh.Value : 0;
                    pr.TraNoXDCB += item.TraNoXDCB.HasValue ? item.TraNoXDCB.Value : 0;
                }
            }

            List<KeHoachVonNamDuocDuyetExportDieuChinh> lstItem = data.Where(n => n.Loai.Equals(1)).ToList();
            lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
            List<KeHoachVonNamDuocDuyetExportDieuChinh> lstItemLevel = data.Where(x => x.Loai.Equals(2)).ToList();
            lstItemLevel.Select(x => { x.STT = (lstItemLevel.IndexOf(x) + 1).ToString(); return x; }).ToList();

            return data;
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        private List<VDTKeHoachVonNamDuocDuyetExport> CalculateDataReport(List<VDTKeHoachVonNamDuocDuyetExport> data)
        {
            try
            {
                List<VDTKeHoachVonNamDuocDuyetExport> result = new List<VDTKeHoachVonNamDuocDuyetExport>();

                List<VDTKeHoachVonNamDuocDuyetExport> childrent = data.Where(x => !x.IsHangCha).ToList();
                List<VDTKeHoachVonNamDuocDuyetExport> parent = data.Where(x => x.IsHangCha && (x.LoaiParent.Equals(0) || x.LoaiParent.Equals(1))).ToList();

                data.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.FCapPhatTaiKhoBac = 0;
                    x.FCapPhatBangLenhChi = 0;
                    x.FGiaTriThuHoiNamTruocKhoBac = 0;
                    x.FGiaTriThuHoiNamTruocLenhChi = 0;
                    x.TongSo = 0;
                    return x;
                }).ToList();

                foreach (var pr in parent.Where(x => x.IdLoaiCongTrinh != null))
                {
                    List<VDTKeHoachVonNamDuocDuyetExport> lstChilrent = childrent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.FCapPhatTaiKhoBac != 0 || x.FCapPhatBangLenhChi != 0 || x.FGiaTriThuHoiNamTruocKhoBac != 0 || x.FGiaTriThuHoiNamTruocLenhChi != 0)))
                    {
                        pr.FCapPhatTaiKhoBac += item.FCapPhatTaiKhoBac;
                        pr.FCapPhatBangLenhChi += item.FCapPhatBangLenhChi;
                        pr.FGiaTriThuHoiNamTruocKhoBac += item.FGiaTriThuHoiNamTruocKhoBac;
                        pr.FGiaTriThuHoiNamTruocLenhChi += item.FGiaTriThuHoiNamTruocLenhChi;
                        pr.TongSo += item.TongSo;
                    }
                }

                foreach (var item in parent.Where(x => (x.FCapPhatTaiKhoBac != 0 || x.FCapPhatBangLenhChi != 0 || x.FGiaTriThuHoiNamTruocKhoBac != 0 || x.FGiaTriThuHoiNamTruocLenhChi != 0 && x.IdLoaiCongTrinh != null)))
                {
                    CalculateParent(item, item, data);
                }

                result = data.Where(x => (x.FCapPhatTaiKhoBac != 0 || x.FCapPhatBangLenhChi != 0 || x.FGiaTriThuHoiNamTruocKhoBac != 0 || x.FGiaTriThuHoiNamTruocLenhChi != 0) || (x.IdLoaiCongTrinh == null && x.IdLoaiCongTrinhParent == null)).ToList();

                return result;
            }
            catch (Exception ex)
            {
                return new List<VDTKeHoachVonNamDuocDuyetExport>();
            }
        }

        private void CalculateParent(VDTKeHoachVonNamDuocDuyetExport currentItem, VDTKeHoachVonNamDuocDuyetExport seftItem, List<VDTKeHoachVonNamDuocDuyetExport> data)
        {
            var parrentItem = data.Where(x => x.IdLoaiCongTrinh != null && x.IdLoaiCongTrinh == currentItem.IdLoaiCongTrinhParent).FirstOrDefault();
            if (parrentItem == null) return;
            parrentItem.FCapPhatTaiKhoBac += seftItem.FCapPhatTaiKhoBac;
            parrentItem.FCapPhatBangLenhChi += seftItem.FCapPhatBangLenhChi;
            parrentItem.FGiaTriThuHoiNamTruocKhoBac += seftItem.FGiaTriThuHoiNamTruocKhoBac;
            parrentItem.FGiaTriThuHoiNamTruocLenhChi += seftItem.FGiaTriThuHoiNamTruocLenhChi;
            parrentItem.TongSo += seftItem.TongSo;
            CalculateParent(parrentItem, seftItem, data);
        }

        #region Print
        public FlexCelReport HandleDataExportChungTuGocNSQP(VDTKeHoachVonNamDuocDuyetExport dataReport, List<string> arrIdDVQL)
        {
            List<PhanBoVonDonViPheDuyetReportQuery> results = new List<PhanBoVonDonViPheDuyetReportQuery>();
            List<VDT_DA_QDDauTu> ListAllDaQddauTu = _qLVonDauTuService.GetAllVdtDaQdDauTu();
            List<VDT_KHV_PhanBoVon_DonVi_PheDuyet> lstQuery = _qLVonDauTuService.GetVDTKhvPhanBoVonDonViPheDuyetByCondition(dataReport);
            if (lstQuery == null || lstQuery.Count == 0) return null;

            results = _qLVonDauTuService.GetPhanBoVonDonViPheDuyetReport(string.Join(",", lstQuery.Select(n => n.Id)), Int32.Parse(dataReport.iNamLamViec), int.Parse(dataReport.sLoaiCongTrinh), 
                string.Join(",", arrIdDVQL), Double.Parse(dataReport.sValueDonViTinh)).ToList();
            BoSungDuLieuNSQP(results, ListAllDaQddauTu, dataReport.sValueDonViTinh);
            results = CalculateDataReport(results, dataReport);
            foreach (var item in results)
            {
                if (item.NgayPheDuyet != null)
                    item.dNgayPheDuyet = item.NgayPheDuyet.Value.ToString("dd/MM/yyyy");
            }
            double TongMucDauTu = results.Where(x => !x.IsHangCha).Sum(x => x.TongMucDauTu);
            double TongMucDauTuNSQP = results.Where(x => !x.IsHangCha).Sum(x => x.TongMucDauTuNSQP);
            double VonBoTriDenHetNamTruoc = results.Where(x => !x.IsHangCha).Sum(x => x.VonBoTriDenHetNamTruoc);
            double KeHoachVonDauTuNam = results.Where(x => !x.IsHangCha).Sum(x => x.KeHoachVonDauTuNam);
            double VonGiaiNganNam = results.Where(x => !x.IsHangCha).Sum(x => x.VonGiaiNganNam);
            double DieuChinhVonNam = results.Where(x => !x.IsHangCha).Sum(x => x.DieuChinhVonNam);
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<PhanBoVonDonViPheDuyetReportQuery>("Items", results);
            fr.SetValue("TongMucDauTuSum", TongMucDauTu);
            fr.SetValue("TongMucDauTuNSQPSum", TongMucDauTuNSQP);
            fr.SetValue("VonBoTriDenHetNamTruocSum", VonBoTriDenHetNamTruoc);
            fr.SetValue("KeHoachVonDauTuNamSum", KeHoachVonDauTuNam);
            fr.SetValue("VonGiaiNganNamSum", VonGiaiNganNam);
            fr.SetValue("DieuChinhVonNamSum", DieuChinhVonNam);
            return fr;
        }

        public FlexCelReport HandleDataExportBudgetOther(VDTKeHoachVonNamDuocDuyetExport dataReport, List<string> arrIdDVQL)
        {
            List<VdtKhvVonNamDonViPheDuyetNguonVonQuery> results = new List<VdtKhvVonNamDonViPheDuyetNguonVonQuery>();
            List<VDT_DA_QDDauTu> ListAllDaQddauTu = _qLVonDauTuService.GetAllVdtDaQdDauTu();
            List<VDT_KHV_PhanBoVon_DonVi_PheDuyet> lstQuery = _qLVonDauTuService.GetVDTKhvPhanBoVonDonViPheDuyetByCondition(dataReport);
            if (lstQuery == null || lstQuery.Count == 0) return null;

            results = _qLVonDauTuService.GetPhanBoVonDonViPheDuyetNguonVonReport(string.Join(",", lstQuery.Select(n => n.Id)),
                string.Join(",", arrIdDVQL), Double.Parse(dataReport.sValueDonViTinh));
            BoSungDuLieuNSKhac(results, ListAllDaQddauTu, dataReport.sValueDonViTinh);

            results.Select(item => { item.TongSoVonNamDieuChinh = item.ThuHoiVonDaUngTruocDieuChinh + item.TraNoXDCB; return item; }).ToList();

            List<VdtKhvVonNamDonViPheDuyetNguonVonQuery> lstParent = results.Where(x => x.IsHangCha).ToList();
            lstParent.Select(x => { x.STT = (lstParent.IndexOf(x) + 1).ToString(); return x; }).ToList();
            List<VdtKhvVonNamDonViPheDuyetNguonVonQuery> lstChildrent = results.Where(x => !x.IsHangCha).ToList();
            lstParent.Select(x =>
            {
                List<VdtKhvVonNamDonViPheDuyetNguonVonQuery> item = lstChildrent.Where(y => y.IdNhomDuAn == x.IdNhomDuAn).ToList();
                item.Select(y => { y.STT = string.Format("{0}.{1}", x.STT, item.IndexOf(y) + 1); return y; }).ToList();
                return x;
            }).ToList();

            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VdtKhvVonNamDonViPheDuyetNguonVonQuery>("Items", results);

            fr.SetValue("TongSoVonDauTuSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonDauTu));
            fr.SetValue("TongSoVonDauTuTrongNuocSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonDauTuTrongNuoc));
            fr.SetValue("KeHoachVonDauTuGiaiDoanSum", results.Where(x => !x.IsHangCha).Sum(x => x.KeHoachVonDauTuGiaiDoan));
            fr.SetValue("VonThanhToanLuyKeSum", results.Where(x => !x.IsHangCha).Sum(x => x.VonThanhToanLuyKe));
            fr.SetValue("TongSoKeHoachVonNamSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoKeHoachVonNam));
            fr.SetValue("ThuHoiVonDaUngTruocSum", results.Where(x => !x.IsHangCha).Sum(x => x.ThuHoiVonDaUngTruoc));
            fr.SetValue("VonThucHienTuDauNamDenNaySum", results.Where(x => !x.IsHangCha).Sum(x => x.VonThucHienTuDauNamDenNay));
            fr.SetValue("TongSoVonNamDieuChinhSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonNamDieuChinh));
            fr.SetValue("ThuHoiVonDaUngTruocDieuChinhSum", results.Where(x => !x.IsHangCha).Sum(x => x.ThuHoiVonDaUngTruocDieuChinh));
            fr.SetValue("TraNoXDCBSum", results.Where(x => !x.IsHangCha).Sum(x => x.TraNoXDCB));
            return fr;
        }

        public FlexCelReport HandleDataExportDieuChinh(VDTKeHoachVonNamDuocDuyetExport dataReport, List<string> arrIdDVQL)
        {
            List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> results = new List<PhanBoVonDonViPheDuyetDieuChinhReportQuery>();
            List<VDT_DA_QDDauTu> ListAllDaQddauTu = _qLVonDauTuService.GetAllVdtDaQdDauTu();
            List<VDT_KHV_PhanBoVon_DonVi_PheDuyet> lstQuery = _qLVonDauTuService.GetVDTKhvPhanBoVonDonViPheDuyetByCondition(dataReport);
            if (lstQuery == null || lstQuery.Count == 0) return null;

            results = _qLVonDauTuService.GetPhanBoVonDonViPheDuyetDieuChinhReport(string.Join(",", lstQuery.Select(n => n.Id)), Int32.Parse(dataReport.iNamLamViec), int.Parse(dataReport.sLoaiCongTrinh),
                string.Join(",", arrIdDVQL), Double.Parse(dataReport.sValueDonViTinh)).ToList();

            results = CalculateDataReportDieuChinh(results);
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<PhanBoVonDonViPheDuyetDieuChinhReportQuery>("Items", results);
            fr.SetValue("TongMucDauTuSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongMucDauTu));
            fr.SetValue("TongMucDauTuNSQPSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongMucDauTuNSQP));
            fr.SetValue("VonBoTriDenHetNamTruocSum", results.Where(x => !x.IsHangCha).Sum(x => x.VonBoTriDenHetNamTruoc));
            fr.SetValue("KeHoachVonDauTuNamSum", results.Where(x => !x.IsHangCha).Sum(x => x.KeHoachVonDauTuNam));
            fr.SetValue("VonGiaiNganNamSum", results.Where(x => !x.IsHangCha).Sum(x => x.VonGiaiNganNam));
            fr.SetValue("DieuChinhVonNamSum", results.Where(x => !x.IsHangCha).Sum(x => x.DieuChinhVonNam));
            return fr;
        }

        public FlexCelReport HandleDataExportBudgetOtherDieuChinh(VDTKeHoachVonNamDuocDuyetExport dataReport, List<string> arrIdDVQL)
        {
            List<VdtKhvVonNamDonViPheDuyetNguonVonDieuChinhQuery> results = new List<VdtKhvVonNamDonViPheDuyetNguonVonDieuChinhQuery>();
            List<VDT_DA_QDDauTu> ListAllDaQddauTu = _qLVonDauTuService.GetAllVdtDaQdDauTu();
            List<VDT_KHV_PhanBoVon_DonVi_PheDuyet> lstQuery = _qLVonDauTuService.GetVDTKhvPhanBoVonDonViPheDuyetByCondition(dataReport);
            if (lstQuery == null || lstQuery.Count == 0) return null;

            results = _qLVonDauTuService.GetPhanBoVonDonViPheDuyetNguonVonDieuChinhReport(string.Join(",", lstQuery.Select(n => n.Id)),
                string.Join(",", arrIdDVQL), Double.Parse(dataReport.sValueDonViTinh));

            List<VdtKhvVonNamDonViPheDuyetNguonVonDieuChinhQuery> lstParent = results.Where(x => x.IsHangCha).ToList();
            lstParent.Select(x => { x.STT = (lstParent.IndexOf(x) + 1).ToString(); return x; }).ToList();
            List<VdtKhvVonNamDonViPheDuyetNguonVonDieuChinhQuery> lstChildrent = results.Where(x => !x.IsHangCha).ToList();
            lstParent.Select(x =>
            {
                List<VdtKhvVonNamDonViPheDuyetNguonVonDieuChinhQuery> item = lstChildrent.Where(y => y.IdNhomDuAn == x.IdNhomDuAn).ToList();
                item.Select(y => { y.STT = string.Format("{0}.{1}", x.STT, item.IndexOf(y) + 1); return y; }).ToList();
                return x;
            }).ToList();
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VdtKhvVonNamDonViPheDuyetNguonVonDieuChinhQuery>("Items", results);
            fr.SetValue("TongSoVonDauTuSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonDauTu));
            fr.SetValue("TongSoVonDauTuTrongNuocSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonDauTuTrongNuoc));
            fr.SetValue("KeHoachVonDauTuGiaiDoanSum", results.Where(x => !x.IsHangCha).Sum(x => x.KeHoachVonDauTuGiaiDoan));
            fr.SetValue("VonThanhToanLuyKeSum", results.Where(x => !x.IsHangCha).Sum(x => x.VonThanhToanLuyKe));
            fr.SetValue("TongSoKeHoachVonNamSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoKeHoachVonNam));
            fr.SetValue("ThuHoiVonDaUngTruocSum", results.Where(x => !x.IsHangCha).Sum(x => x.ThuHoiVonDaUngTruoc));
            fr.SetValue("VonThucHienTuDauNamDenNaySum", results.Where(x => !x.IsHangCha).Sum(x => x.VonThucHienTuDauNamDenNay));
            fr.SetValue("TongSoVonNamDieuChinhSum", results.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonNamDieuChinh));
            fr.SetValue("ThuHoiVonDaUngTruocDieuChinhSum", results.Where(x => !x.IsHangCha).Sum(x => x.ThuHoiVonDaUngTruocDieuChinh));
            fr.SetValue("TraNoXDCBSum", results.Where(x => !x.IsHangCha).Sum(x => x.TraNoXDCB));
            return fr;
        }

        private List<PhanBoVonDonViPheDuyetReportQuery> CalculateDataReport(List<PhanBoVonDonViPheDuyetReportQuery> data, VDTKeHoachVonNamDuocDuyetExport dataReport)
        {
            try
            {
                List<PhanBoVonDonViPheDuyetReportQuery> result = new List<PhanBoVonDonViPheDuyetReportQuery>();
                List<PhanBoVonDonViPheDuyetReportQuery> childrent = data.Where(x => !x.IsHangCha).ToList();
                List<PhanBoVonDonViPheDuyetReportQuery> parent = data.Where(x => x.IsHangCha && (x.LoaiParent.Equals(0) || x.LoaiParent.Equals(1))).ToList();

                data.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.TongMucDauTu = 0;
                    x.TongMucDauTuNSQP = 0;
                    x.VonBoTriDenHetNamTruoc = 0;
                    x.KeHoachVonDauTuNam = 0;
                    x.VonGiaiNganNam = 0;
                    x.DieuChinhVonNam = 0;
                    return x;
                }).ToList();

                foreach (var pr in parent.Where(x => x.IIdLoaiCongTrinh != null))
                {
                    List<PhanBoVonDonViPheDuyetReportQuery> lstChilrent = childrent.Where(x => x.IIdLoaiCongTrinh.Equals(pr.IIdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => ((x.TongMucDauTu != 0)
                        || (x.TongMucDauTuNSQP != 0)
                        || (x.VonBoTriDenHetNamTruoc != 0)
                        || (x.KeHoachVonDauTuNam != 0)
                        || (x.VonGiaiNganNam != 0)
                        || (x.DieuChinhVonNam != 0))))
                    {
                        pr.TongMucDauTu += item.TongMucDauTu;
                        pr.TongMucDauTuNSQP += item.TongMucDauTuNSQP;
                        pr.VonBoTriDenHetNamTruoc += item.VonBoTriDenHetNamTruoc;
                        pr.KeHoachVonDauTuNam += item.KeHoachVonDauTuNam;
                        pr.VonGiaiNganNam += item.VonGiaiNganNam;
                        pr.DieuChinhVonNam += item.DieuChinhVonNam;
                    }
                }

                foreach (var item in parent.Where(x => ((x.TongMucDauTu != 0)
                    || (x.TongMucDauTuNSQP != 0)
                    || (x.VonBoTriDenHetNamTruoc != 0)
                    || (x.KeHoachVonDauTuNam != 0)
                    || (x.VonGiaiNganNam != 0)
                    || (x.DieuChinhVonNam != 0)) && x.IIdLoaiCongTrinh != null))
                {
                    CalculateParent(item, item, data);
                }

                result = data.Where(x => ((x.TongMucDauTu != 0)
                    || (x.TongMucDauTuNSQP != 0)
                    || (x.VonBoTriDenHetNamTruoc != 0)
                    || (x.KeHoachVonDauTuNam != 0)
                    || (x.VonGiaiNganNam != 0)
                    || (x.DieuChinhVonNam != 0)) || (x.IIdLoaiCongTrinh == null && x.IIdLoaiCongTrinhParent == null)).ToList();

                foreach (var child in result)
                {
                    if (child.IdDuAn != null && child.IdDuAn != Guid.Empty)
                    {
                        var vonBoTri5Nam = _qLVonDauTuService.GetVonBoTri5Nam(child.IdDuAn.ToString(), Int32.Parse(dataReport.iNamLamViec));
                        child.VonBoTri5Nam = (Double)vonBoTri5Nam / Int32.Parse(dataReport.sValueDonViTinh);
                    }
                }

                List<PhanBoVonDonViPheDuyetReportQuery> lstItem = data.Where(n => n.LoaiParent.Equals(0) && !n.Loai.Equals(1)).ToList();
                lstItem.Select(n => { n.STT = CommonFunction.ConvertLaMa((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
                List<PhanBoVonDonViPheDuyetReportQuery> lstItemLevel = data.Where(x => !x.Loai.Equals(1) && x.LoaiParent.Equals(2)).ToList();
                lstItemLevel.Select(x => { x.STT = (lstItemLevel.IndexOf(x) + 1).ToString(); return x; }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return new List<PhanBoVonDonViPheDuyetReportQuery>();
            }
        }

        private void CalculateParent(PhanBoVonDonViPheDuyetReportQuery currentItem, PhanBoVonDonViPheDuyetReportQuery seftItem, List<PhanBoVonDonViPheDuyetReportQuery> data)
        {
            try
            {
                var parrentItem = data.Where(x => x.IIdLoaiCongTrinh != null && x.IIdLoaiCongTrinh == currentItem.IIdLoaiCongTrinhParent).FirstOrDefault();
                if (parrentItem == null) return;
                parrentItem.TongMucDauTu += seftItem.TongMucDauTu;
                parrentItem.TongMucDauTuNSQP += seftItem.TongMucDauTuNSQP;
                parrentItem.VonBoTriDenHetNamTruoc += seftItem.VonBoTriDenHetNamTruoc;
                parrentItem.KeHoachVonDauTuNam += seftItem.KeHoachVonDauTuNam;
                parrentItem.VonGiaiNganNam += seftItem.VonGiaiNganNam;
                parrentItem.DieuChinhVonNam += seftItem.DieuChinhVonNam;
                CalculateParent(parrentItem, seftItem, data);
            }
            catch (Exception ex)
            {
            }
        }

        private List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> CalculateDataReportDieuChinh(List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> data)
        {
            try
            {
                List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> result = new List<PhanBoVonDonViPheDuyetDieuChinhReportQuery>();
                List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> childrent = data.Where(x => !x.IsHangCha).ToList();
                List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> parent = data.Where(x => x.IsHangCha && (x.LoaiParent.Equals(0) || x.LoaiParent.Equals(1))).ToList();

                data.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.TongMucDauTu = 0;
                    x.TongMucDauTuNSQP = 0;
                    x.VonBoTriDenHetNamTruoc = 0;
                    x.KeHoachVonDauTuNam = 0;
                    x.VonGiaiNganNam = 0;
                    x.DieuChinhVonNam = 0;
                    return x;
                }).ToList();

                foreach (var pr in parent.Where(x => x.IIdLoaiCongTrinh != null))
                {
                    List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> lstChilrent = childrent.Where(x => x.IIdLoaiCongTrinh.Equals(pr.IIdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => ((x.TongMucDauTu != 0)
                        || (x.TongMucDauTuNSQP != 0)
                        || (x.VonBoTriDenHetNamTruoc != 0)
                        || (x.KeHoachVonDauTuNam != 0)
                        || (x.VonGiaiNganNam != 0)
                        || (x.DieuChinhVonNam != 0))))
                    {
                        pr.TongMucDauTu += item.TongMucDauTu;
                        pr.TongMucDauTuNSQP += item.TongMucDauTuNSQP;
                        pr.VonBoTriDenHetNamTruoc += item.VonBoTriDenHetNamTruoc;
                        pr.KeHoachVonDauTuNam += item.KeHoachVonDauTuNam;
                        pr.VonGiaiNganNam += item.VonGiaiNganNam;
                        pr.DieuChinhVonNam += item.DieuChinhVonNam;
                    }
                }

                foreach (var item in parent.Where(x => ((x.TongMucDauTu != 0)
                    || (x.TongMucDauTuNSQP != 0)
                    || (x.VonBoTriDenHetNamTruoc != 0)
                    || (x.KeHoachVonDauTuNam != 0)
                    || (x.VonGiaiNganNam != 0)
                    || (x.DieuChinhVonNam != 0) && x.IIdLoaiCongTrinh != null)))
                {
                    CalculateParentDieuChinh(item, item, data);
                }

                result = data.Where(x => ((x.TongMucDauTu != 0)
                    || (x.TongMucDauTuNSQP != 0)
                    || (x.VonBoTriDenHetNamTruoc != 0)
                    || (x.KeHoachVonDauTuNam != 0)
                    || (x.VonGiaiNganNam != 0)
                    || (x.DieuChinhVonNam != 0 )) || (x.IIdLoaiCongTrinh == null && x.IIdLoaiCongTrinhParent == null)).ToList();

                List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> lstItem = data.Where(n => n.LoaiParent.Equals(0) && !n.Loai.Equals(1)).ToList();
                lstItem.Select(n => { n.STT = CommonFunction.ConvertLaMa((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
                List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> lstItemLevel = data.Where(x => !x.Loai.Equals(1) && x.LoaiParent.Equals(2)).ToList();
                lstItemLevel.Select(x => { x.STT = (lstItemLevel.IndexOf(x) + 1).ToString(); return x; }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return new List<PhanBoVonDonViPheDuyetDieuChinhReportQuery>();
            }
        }

        private void CalculateParentDieuChinh(PhanBoVonDonViPheDuyetDieuChinhReportQuery currentItem, PhanBoVonDonViPheDuyetDieuChinhReportQuery seftItem, List<PhanBoVonDonViPheDuyetDieuChinhReportQuery> data)
        {
            try
            {
                var parrentItem = data.Where(x => x.IIdLoaiCongTrinh != null && x.IIdLoaiCongTrinh == currentItem.IIdLoaiCongTrinhParent).FirstOrDefault();
                if (parrentItem == null) return;
                parrentItem.TongMucDauTu += seftItem.TongMucDauTu;
                parrentItem.TongMucDauTuNSQP += seftItem.TongMucDauTuNSQP;
                parrentItem.VonBoTriDenHetNamTruoc += seftItem.VonBoTriDenHetNamTruoc;
                parrentItem.KeHoachVonDauTuNam += seftItem.KeHoachVonDauTuNam;
                parrentItem.VonGiaiNganNam += seftItem.VonGiaiNganNam;
                parrentItem.DieuChinhVonNam += seftItem.DieuChinhVonNam;
                CalculateParentDieuChinh(parrentItem, seftItem, data);
            }
            catch (Exception ex)
            {
            }
        }

        public void BoSungDuLieuNSQP(List<PhanBoVonDonViPheDuyetReportQuery> data, List<VDT_DA_QDDauTu> ListAllDaQddauTu, string donViTinh)
        {
            foreach (var it in data)
            {
                if (it.IdDuAn.HasValue)
                {
                    var lstQddauTu = ListAllDaQddauTu.Where(x => x.iID_DuAnID.Equals(it.IdDuAn.Value)).FirstOrDefault();
                    if (lstQddauTu != null)
                    {
                        var lstNguonVon = _qLVonDauTuService.FindNguonVonByParentId(lstQddauTu.iID_QDDauTuID);
                        it.SoPheDuyet = lstQddauTu.sSoQuyetDinh;
                        it.NgayPheDuyet = lstQddauTu.dNgayQuyetDinh;
                        it.TongMucDauTu = lstQddauTu.fTongMucDauTuPheDuyet.GetValueOrDefault() / double.Parse(donViTinh);
                        it.TongMucDauTuNSQP = 0;
                        if(lstNguonVon != null)
                            it.TongMucDauTuNSQP = lstNguonVon.Sum(x => x.fTienPheDuyet.GetValueOrDefault()) / double.Parse(donViTinh);
                    }
                }
            }
        }

        public void BoSungDuLieuNSKhac(List<VdtKhvVonNamDonViPheDuyetNguonVonQuery> data, List<VDT_DA_QDDauTu> ListAllDaQddauTu, string donViTinh)
        {
            foreach (var it in data)
            {
                if (it.IdDuAn.HasValue)
                {
                    var lstQddauTu = ListAllDaQddauTu.Where(x => x.iID_DuAnID.Equals(it.IdDuAn.Value)).FirstOrDefault();
                    if (lstQddauTu != null)
                    {
                        var lstNguonVon = _qLVonDauTuService.FindNguonVonByParentId(lstQddauTu.iID_QDDauTuID);
                        it.SSoQuyetDinh = lstQddauTu.sSoQuyetDinh;
                        it.DNgayQuyetDinh = lstQddauTu.dNgayQuyetDinh;
                        it.TongSoVonDauTu = lstQddauTu.fTongMucDauTuPheDuyet.GetValueOrDefault() / double.Parse(donViTinh);
                        it.TongSoVonDauTuTrongNuoc = 0;
                        if(lstNguonVon != null)
                            it.TongSoVonDauTuTrongNuoc = lstNguonVon.Sum(x => x.fTienPheDuyet.GetValueOrDefault()) / double.Parse(donViTinh);
                    }
                }
            }
        }

        private string GetNameUnit(VDTKeHoachVonNamDuocDuyetExport dataReport)
        {
            string sTenDonVi = string.Empty;
            try
            {
                Dictionary<string, NS_DonVi> dicDonVi = new Dictionary<string, NS_DonVi>();
                var lstDonVi = _qLVonDauTuService.GetDanhSachDonVi(PhienLamViec.NamLamViec);
                if(lstDonVi != null)
                {
                    foreach(var item in lstDonVi)
                    {
                        if (!dicDonVi.ContainsKey(item.iID_MaDonVi))
                            dicDonVi.Add(item.iID_MaDonVi, item);
                    }
                }
                if (!string.IsNullOrEmpty(PhienLamViec.iID_MaDonVi) && dicDonVi.ContainsKey(PhienLamViec.iID_MaDonVi))
                    sTenDonVi = dicDonVi[PhienLamViec.iID_MaDonVi].sTen;
                
                //if (ListDonVi.Where(x => x.IsChecked).Count() == 1)
                //{
                //    string sTen = ListDonVi.Where(x => x.IsChecked).FirstOrDefault().DisplayItem;
                //    if (!string.IsNullOrEmpty(sTen) && sTen.Contains("-"))
                //    {
                //        sTen = sTen.Split("-")[1];
                //    }
                //    sTenDonVi = sTen;
                //}
            }
            catch (Exception ex)
            {
                
            }
            return sTenDonVi;
        }
        #endregion
        #endregion Helper
    }
}