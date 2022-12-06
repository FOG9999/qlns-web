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
using Microsoft.Ajax.Utilities;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class KeHoachVonNamDuocDuyetController : FlexcelReportController
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private static List<NS_MucLucNganSach> _lstMucLucNganSach = new List<NS_MucLucNganSach>();
        private static List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet> _lstVonNamChiTiet = new List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet>();
        private const int KHOI_CONG_MOI = 1;
        private const int CHUYEN_TIEP = 2;
        private string pathString = System.Configuration.ConfigurationManager.AppSettings["FtpPath"];
        private string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FtpUsername"];
        private string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        private const string sControlName = "KeHoachVonNamDuocDuyet";
        // GET: QLVonDauTu/QLKeHoachVonNamDuocDuyet
        public ActionResult Index()
        {
            KeHoachVonNamDuocDuyetViewModel vm = new KeHoachVonNamDuocDuyetViewModel();

            try
            {
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");

                vm._paging.CurrentPage = 1;
                vm.Items = _qLVonDauTuService.GetAllKeHoachVonNamDuocDuyet(ref vm._paging);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult KeHoachVonNamDuocDuyetSearch(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, int? iNamKeHoach, int? iID_NguonVonID, Guid? iID_DonViQuanLyID)
        {
            KeHoachVonNamDuocDuyetViewModel vm = new KeHoachVonNamDuocDuyetViewModel();
            sSoQuyetDinh = HttpUtility.HtmlDecode(sSoQuyetDinh);
            try
            {
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");

                vm._paging.CurrentPage = 1;
                vm._paging = _paging;
                vm.Items = _qLVonDauTuService.GetAllKeHoachVonNamDuocDuyet(ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID);
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
            VDT_KHV_KeHoachVonNam_DuocDuyet data = new VDT_KHV_KeHoachVonNam_DuocDuyet();
            try
            {
                if (id.HasValue)
                {
                    data = _qLVonDauTuService.GetKeHoachVonNamDuocDuyetById(id);
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
                else if (data.iID_KeHoachVonNam_DuocDuyetID == null || data.iID_KeHoachVonNam_DuocDuyetID == Guid.Empty)
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
        public ActionResult KeHoach5NamDuocDuyetDelete(Guid id)
        {
            bool status = false;
            try
            {
                status = _qLVonDauTuService.DeleteKeHoachVonNamDuocDuyet(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult KeHoachVonNamDuocDuyetSave(VDT_KHV_KeHoachVonNam_DuocDuyet data, bool isModified)
        {
            data.sSoQuyetDinh = HttpUtility.HtmlDecode(data.sSoQuyetDinh);
            Guid iID = Guid.Empty;
            try
            {
                if (data.iID_KeHoachVonNam_DuocDuyetID == null || data.iID_KeHoachVonNam_DuocDuyetID == Guid.Empty)
                {
                    if (_qLVonDauTuService.CheckExistSoKeHoachVonNamDuocDuyet(data.sSoQuyetDinh, data.iNamKeHoach.Value, null))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (_qLVonDauTuService.CheckExistSoKeHoachVonNamDuocDuyet(data.sSoQuyetDinh, data.iNamKeHoach.Value, data.iID_KeHoachVonNam_DuocDuyetID) && !isModified)
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (_qLVonDauTuService.CheckExistKeHoachVonNamDuocDuyet(data.iID_DonViQuanLyID.ToString(), data.iID_NguonVonID ?? 0, data.iNamKeHoach ?? 0, data.iID_KeHoachVonNam_DuocDuyetID))
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

                if (!_qLVonDauTuService.SaveKeHoachVonNamDuocDuyet(ref iID, data, Username, isModified))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true, iID = iID }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(Guid id, bool isDetail)
        {
            VDT_KHV_KeHoachVonNam_DuocDuyet data = _qLVonDauTuService.GetKeHoachVonNamDuocDuyetById(id);
            KeHoachVonNamDuocDuyetChiTietGridViewModel vm = new KeHoachVonNamDuocDuyetChiTietGridViewModel
            {
                KHVonNamDuocDuyet = data
            };

            List<VDT_KHV_KeHoachVonNam_DeXuat> lstAggregate = new List<VDT_KHV_KeHoachVonNam_DeXuat>();
            if (data != null)
            {
                lstAggregate = _qLVonDauTuService.GetKeHoachVonNamDeXuatTongHopByCondition(data.iNamKeHoach.Value, data.iID_DonViQuanLyID,data.iID_NguonVonID).ToList();
            }

            ViewBag.LstVoucherAggregate = lstAggregate.ToSelectList("iID_KeHoachVonNamDeXuatID", "sSoQuyetDinh");
            TempData["isDetail"] = isDetail;

            return View(vm);
        }

        [HttpGet]
        public ActionResult SheetFrame(string id, string idParent, string filter = null, bool isActive = true, string idDx = "")
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var isCheck = TempData["isDetail"];
            TempData.Keep("isDetail");
            var isDetail = false;
            if (isCheck != null )
            {
                 isDetail = (bool)TempData["isDetail"];
            }
            var sheet = new KeHoachVonNamDuocDuyetChiTietSheetTable(id, idParent, PhienLamViec.NamLamViec, filters, idDx, isDetail);

            var vm = new KeHoachVonNamDuocDuyetChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   id: id,
                   bang: sheet,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "KeHoachVonNamDuocDuyet", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrame", "KeHoachVonNamDuocDuyet", new { area = "QLVonDauTu" })
                 ),
            };

            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            ViewBag.modelActive = isActive;
            Guid idDxConvert = Guid.Empty;
            Guid.TryParse(idDx, out idDxConvert);
            if (vm.KHVonNamDuocDuyet == null)
                vm.KHVonNamDuocDuyet = new VDT_KHV_KeHoachVonNam_DuocDuyet();
            vm.KHVonNamDuocDuyet.iID_KeHoachVonNamDeXuatID = idDxConvert;
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var voucherId = string.Empty;
            var voucherParent = string.Empty;
            bool isActive = true;
            Guid iID_KeHoachVonNamDuocDuyetID = Guid.Parse(vm.Id);
            Guid iID_KeHoachVonNamDeXuatID = Guid.Empty;
            if (TempData["iIDKHVNDX"] != null)
                iID_KeHoachVonNamDeXuatID = (Guid)TempData["iIDKHVNDX"];

            try
            {
                _qLVonDauTuService.SaveKeHoachVonNamDuocDuyetChiTiet(_lstVonNamChiTiet, _lstMucLucNganSach, iID_KeHoachVonNamDeXuatID, iID_KeHoachVonNamDuocDuyetID);
                if (_lstVonNamChiTiet != null && _lstVonNamChiTiet.Count() > 0)
                {
                    voucherId = _lstVonNamChiTiet.FirstOrDefault().iID_KeHoachVonNam_DuocDuyetID.ToString();
                    VDT_KHV_KeHoachVonNam_DuocDuyet item = _qLVonDauTuService.GetKeHoachVonNamDuocDuyetById(Guid.Parse(voucherId));
                    if (item != null)
                    {
                        voucherParent = item.iID_ParentId.ToString();
                        isActive = item.bActive.Value;

                        // update khv nam duoc duyet
                        _qLVonDauTuService.UpdateGiaTriCapPhatKHVnamDuocDuyet(Guid.Parse(voucherId));
                    }
                }

                _lstMucLucNganSach = new List<NS_MucLucNganSach>();
                _lstVonNamChiTiet = new List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet>();
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return SheetFrame(voucherId, voucherParent, null, isActive, string.Empty);
        }

        [HttpPost]
        public JsonResult ValidateBeforeSave(List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet> aListModel, Guid? iID_KeHoachVonNamDeXuatID)
        {
            var listErrMess = new List<string>();
            TempData["iIDKHVNDX"] = iID_KeHoachVonNamDeXuatID;
            if (aListModel != null && aListModel.Any())
            {
                _lstMucLucNganSach = _qLVonDauTuService.GetAllMucLucNganSachByNamLamViec(PhienLamViec.NamLamViec).ToList();
                Dictionary<string, string> dicMucLucNganSach = new Dictionary<string, string>();

                if (_lstMucLucNganSach != null && _lstMucLucNganSach.Any())
                {
                    List<string> lstItem = _lstMucLucNganSach.Select(n => (n.sLNS + "-" + n.sL + "-" + n.sK + "-" + n.sM + "-" + n.sTM + "-" + n.sTTM + "-" + n.sNG)).ToList();
                    dicMucLucNganSach = lstItem.Distinct().ToDictionary(n => n, n => n);
                }

                _lstVonNamChiTiet = aListModel.Where(x => ((x.fCapPhatTaiKhoBac.HasValue && x.fCapPhatTaiKhoBac != 0)
                                        || (x.fCapPhatBangLenhChi.HasValue && x.fCapPhatBangLenhChi != 0))
                                        || (x.fGiaTriThuHoiNamTruocKhoBac.HasValue && x.fGiaTriThuHoiNamTruocKhoBac != 0)
                                        || (x.fGiaTriThuHoiNamTruocLenhChi.HasValue && x.fGiaTriThuHoiNamTruocLenhChi != 0)).ToList();

                foreach (var item in _lstVonNamChiTiet)
                {
                    item.LNS = HttpUtility.HtmlDecode(item.LNS);
                    item.K = HttpUtility.HtmlDecode(item.K);
                    item.M = HttpUtility.HtmlDecode(item.M);
                    item.TM = HttpUtility.HtmlDecode(item.TM);
                    item.TTM = HttpUtility.HtmlDecode(item.TTM);
                    item.NG = HttpUtility.HtmlDecode(item.NG);
                    item.L = HttpUtility.HtmlDecode(item.L);

                    string sXauNoiChuoi = item.LNS + "-" + item.L + "-" + item.K + "-" + item.M + "-" + item.TM + "-" + item.TTM + "-" + item.NG;
                    if (!dicMucLucNganSach.ContainsKey(sXauNoiChuoi))
                    {
                        listErrMess.Add(string.Format("Mục lục ngân sách {0} không tồn tại.{1}", sXauNoiChuoi, Environment.NewLine));
                    }
                }
            }

            if (listErrMess != null && listErrMess.Any())
            {
                return Json(new { status = false, listErrMess = listErrMess }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KeHoachVonNamDuocDuyetExport(Guid? idKeHoachVonNamDuocDuyet)
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
        public FileContentResult ExportExcel()
        {
            ExcelFile xls = (ExcelFile)TempData["DataReport"];
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"KeHoachVonNamDuocDuyet.xlsx");
            }
        }
        [HttpPost]
        public ActionResult SendFile(Guid id, string idDonVi, string nam)
        {
            var excel = KeHoachVonNamDuocDuyetExport(id);
            ExcelFile xls = (ExcelFile)TempData["DataReport"];
            string sFileName = "KeHoachVonNamDuocDuyet.xlsx";
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return SaveFile(stream.ToArray(), sFileName, idDonVi, nam);
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
            string folder = "VDT/KeHoachVonNamDuocDuyet/receive/";
            string uri = pathString + "/" + tenDonVi + "/" + folder + nam.Trim();
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
                rootInfo = _qLVonDauTuService.GetFtpRoot(tenDonVi, pathString, folder);
                if (rootInfo == null)
                {
                    rootInfo = new VDT_FtpRoot();
                    rootInfo.Id = Guid.NewGuid();
                    rootInfo.sMaDonVi = tenDonVi;
                    rootInfo.sIpAddress = pathString;
                    rootInfo.sFolderRoot = folder;
                    rootInfo.sNguoiTao = Username;
                    rootInfo.dNgayTao = DateTime.Now;
                    _qLVonDauTuService.InsertFtpRoot(rootInfo);
                }
                var fileInfo = new VDT_FtpFile();
                fileInfo.Id = Guid.NewGuid();
                fileInfo.iID_FtpRoot = rootInfo.Id;
                fileInfo.sFileName = crTime.ToString("ddMMyyyyhhmmssfff") + "_" + filename;
                fileInfo.sRootPath = path;
                fileInfo.sNguoiTao = Username;
                fileInfo.dNgayTao = DateTime.Now;
                _qLVonDauTuService.InsertFtpFile(fileInfo);
            }
            catch (Exception ex)
            {

            }
            return Json("");
        }

        [HttpGet]
        public ActionResult ViewInBaoCao()
        {
            VDTKeHoachVonNamDuocDuyetExport vm = new VDTKeHoachVonNamDuocDuyetExport();
            try
            {
                IEnumerable<NS_DonVi> lstDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec);
                vm.lstDonViQuanLy = lstDonViQuanLy;

                List<ComBoBoxItem> lstLoaiChungTu = new List<ComBoBoxItem>()
                {
                    new ComBoBoxItem(){DisplayItem = "Gốc", ValueItem = "1"}
                };

                ViewBag.lstLoaiChungTu = lstLoaiChungTu.ToSelectList("ValueItem", "DisplayItem");

                IEnumerable<NS_NguonNganSach> lstDMNguonNganSach = _qLVonDauTuService.GetListDMNguonNganSach();
                ViewBag.lstNguonVon = lstDMNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");

                List<VDT_DM_LoaiCongTrinh> lstLoaiCongTrinh = _qLVonDauTuService.GetListParentDMLoaiCongTrinh();
                lstLoaiCongTrinh.Insert(0, new VDT_DM_LoaiCongTrinh { iID_LoaiCongTrinh = Guid.Empty, sTenLoaiCongTrinh = "--Tất cả--" });
                ViewBag.lstLoaiCongTrinh = lstLoaiCongTrinh.ToSelectList("iID_LoaiCongTrinh", "sTenLoaiCongTrinh");

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
                var listErrMess = new List<string>();
                if (dataReport == null || arrIdDVQL.Count() == 0)
                {
                    listErrMess.Add("Lỗi in báo cáo");
                }

                if (listErrMess.Count() > 0)
                {
                    return Json(new { status = false, listErrMess = listErrMess }, JsonRequestBehavior.AllowGet);
                }

                string lstDonViQuanLy = string.Join(",", arrIdDVQL.ToList());
                List<VDT_KHV_KeHoachVonNam_DuocDuyet> itemQuery = _qLVonDauTuService.GetKeHoachVonNamDuocDuyetByCondition(dataReport.iNamLamViec, lstDonViQuanLy, dataReport.sNguonVon).ToList();
                if (itemQuery == null || itemQuery.Count() == 0)
                {
                    listErrMess.Add("Không tìm thấy dữ liệu");
                    return Json(new { status = false, listErrMess = listErrMess }, JsonRequestBehavior.AllowGet);
                }
                string lstLct = dataReport.sLoaiCongTrinh;

                Dictionary<string, string> lstGroupUnit = itemQuery.GroupBy(x => x.iID_DonViQuanLyID)
                       .ToDictionary(x => x.Key.ToString(), x => string.Join(",", itemQuery.Where(y => y.iID_DonViQuanLyID == x.Key).Select(z => z.iID_KeHoachVonNam_DuocDuyetID.ToString()).ToList()));

                List<VDTKeHoachVonNamDuocDuyetExport> lstGroup = new List<VDTKeHoachVonNamDuocDuyetExport>();

                foreach (var itemUnit in lstGroupUnit.Keys)
                {
                    string lstId = lstGroupUnit[itemUnit];
                    List<VDTKeHoachVonNamDuocDuyetExport> lstCongTrinhMoMoi = _qLVonDauTuService.GetKeHoachVonNamDuocDuyetReport(lstId, lstLct, KHOI_CONG_MOI, double.Parse(dataReport.sValueDonViTinh)).ToList();
                    string sNameUnit = _qLVonDauTuService.GetDonViQuanLyById(Guid.Parse(itemUnit)).sTen;
                    List<VDTKeHoachVonNamDuocDuyetExport> lstDataExportCongTrinhMoMoi = CalculateDataReport(lstCongTrinhMoMoi);
                    List<VDTKeHoachVonNamDuocDuyetExport> lstCongTrinhChuyenTiep = _qLVonDauTuService.GetKeHoachVonNamDuocDuyetReport(lstId, lstLct, CHUYEN_TIEP, double.Parse(dataReport.sValueDonViTinh)).ToList();
                    List<VDTKeHoachVonNamDuocDuyetExport> lstDataExportCongTrinhChuyenTiep = CalculateDataReport(lstCongTrinhChuyenTiep);
                    if(!(lstDataExportCongTrinhMoMoi.Count == 1 && lstDataExportCongTrinhChuyenTiep.Count == 1))
                    {
                        lstGroup.Add(new VDTKeHoachVonNamDuocDuyetExport() { sTenDuAn = sNameUnit.ToUpper(), IsHangCha = true });
                        lstGroup.AddRange(lstDataExportCongTrinhMoMoi);
                        lstGroup.AddRange(lstDataExportCongTrinhChuyenTiep);
                    }
                }

                ExcelFile xls = CreateReportGoc(lstGroup, dataReport);
                xls.PrintLandscape = false;

                TempData["DataReportGocXls"] = xls;

                FlexCelPdfExport pdf = new FlexCelPdfExport(xls, true);
                var bufferPdf = new MemoryStream();

                pdf.Export(bufferPdf);

                TempData["DataReportGoc"] = bufferPdf;

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true, isPdf = isPdf, isGoc = dataReport.sLoaiChungTu.Equals("1") ? true : false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportReport(bool pdf)
        {
            ExcelFile xls = (ExcelFile)TempData["DataReportGocXls"];

            return Print(xls, pdf ? "pdf" : "xls", pdf ? "BaoCaoKeHoachVonNamDuocDuyet.pdf" : "BaoCaoKeHoachVonNamDuocDuyet.xlsx");
 
            //if (pdf)
            //{
            //    MemoryStream stream = (MemoryStream)TempData["DataReportGoc"];
            //    return File(stream.ToArray(), "application/pdf", $"BaoCaoKeHoachVonNamDuocDuyet.pdf");
            //}
            //else
            //{
            //    ExcelFile xls = (ExcelFile)TempData["DataReportGocXls"];

            //    using (MemoryStream stream = new MemoryStream())
            //    {
            //        xls.Save(stream);

            //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCaoKeHoachVonNamDuocDuyet.xlsx");
            //    }
            //}
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
        #endregion Helper
    }
}