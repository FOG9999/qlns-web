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

        // GET: QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet
        public ActionResult Index()
        {
            KeHoachPhanBoVonDonViPheDuyetViewModel vm = new KeHoachPhanBoVonDonViPheDuyetViewModel();

            try
            {
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");

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
                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");

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

                var lstDonVi = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
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
                lstAggregate = _qLVonDauTuService.GetKeHoachVonNamDeXuatTongHopByCondition(data.iNamKeHoach.Value, data.iID_DonViQuanLyID).ToList();
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
                    sGhiChu = model.sGhiChu
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
                IEnumerable<NS_DonVi> lstDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec);
                vm.lstDonViQuanLy = lstDonViQuanLy;

                List<ComBoBoxItem> lstLoaiChungTu = new List<ComBoBoxItem>()
                {
                    new ComBoBoxItem(){DisplayItem = "Gốc", ValueItem = "1"}
                };

                ViewBag.lstLoaiChungTu = lstLoaiChungTu.ToSelectList("ValueItem", "DisplayItem");

                IEnumerable<NS_NguonNganSach> lstDMNguonNganSach = _qLVonDauTuService.GetListDMNguonNganSach();
                ViewBag.lstNguonVon = lstDMNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");

                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");

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
                    lstGroup.Add(new VDTKeHoachVonNamDuocDuyetExport() { sTenDuAn = sNameUnit.ToUpper(), IsHangCha = true });
                    List<VDTKeHoachVonNamDuocDuyetExport> lstDataExportCongTrinhMoMoi = CalculateDataReport(lstCongTrinhMoMoi);
                    lstGroup.AddRange(lstDataExportCongTrinhMoMoi);
                    List<VDTKeHoachVonNamDuocDuyetExport> lstCongTrinhChuyenTiep = _qLVonDauTuService.GetKeHoachVonNamDuocDuyetReport(lstId, lstLct, CHUYEN_TIEP, double.Parse(dataReport.sValueDonViTinh)).ToList();
                    List<VDTKeHoachVonNamDuocDuyetExport> lstDataExportCongTrinhChuyenTiep = CalculateDataReport(lstCongTrinhChuyenTiep);
                    lstGroup.AddRange(lstDataExportCongTrinhChuyenTiep);
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

            return Print(xls, pdf ? "pdf" : "xls", pdf ? "BaoCaoKeHoachVonNamDuocDuyet.pdf" : "BaoCaoKeHoachVonNamDuocDuyet.xls");

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