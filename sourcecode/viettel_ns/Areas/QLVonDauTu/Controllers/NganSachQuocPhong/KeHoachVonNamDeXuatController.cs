using DapperExtensions;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Services.Description;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.z.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;
using VTS.QLNS.CTC.App.Service.UserFunction;
using static System.Net.WebRequestMethods;
using static VTS.QLNS.CTC.App.Service.UserFunction.FormatNumber;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class KeHoachVonNamDeXuatController : FlexcelReportController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        private static List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> _lstTongHop = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
        private static List<DuAnKeHoachVonNam> _lstDuAnChecked = new List<DuAnKeHoachVonNam>();

        public const string NGHIN_DONG = "Nghìn đồng";
        public const string DONG = "Đồng";
        public const string NGHIN_DONG_VALUE = "1000";
        public const string DONG_VALUE = "1";
        public const string TRIEU_DONG = "Triệu đồng";
        public const string TRIEU_VALUE = "1000000";
        public const string TY_DONG = "Tỷ đồng";
        public const string TY_VALUE = "1000000000";

        public const string RPT_DIEUCHINH_NSQP = "2";
        public const string RPT_DIEUCHINH_NSK = "3";
        public string pathString = System.Configuration.ConfigurationManager.AppSettings["FtpPath"];
        public string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FtpUsername"];
        public string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        private const string sControlName = "KeHoachVonNamDeXuat";
        private const string RPT_TONGHOP = "1";
        private const string RPT_DONVI = "2";
        private const string RPT_GOC = "1";
        private const string RPT_DIEUCHINH = "2";
        private const string RPT_CONGTRINH_MOMOI = "1";
        private const string RPT_CONGTRINH_CHUYENTIEP = "2";

        /*
        public ActionResult Index()
        {
            KeHoachVonNamDeXuatViewModel vm = new KeHoachVonNamDeXuatViewModel();

            try
            {
                ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");

                vm._paging.CurrentPage = 1;

                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstKHVNDX = _qLVonDauTuService.GetAllKeHoachVonNamDeXuat(ref vm._paging).ToList();
                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChungTuTongHopParent = lstKHVNDX.Where(x => !string.IsNullOrEmpty(x.sTongHop)).ToList();
                Dictionary<string, List<string>> dctChungTu = lstChungTuTongHopParent.GroupBy(x => x.iID_KeHoachVonNamDeXuatID).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChungTuIndex = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                foreach (var key in dctChungTu.Keys)
                {
                    string lstValue = dctChungTu[key].ToList().FirstOrDefault();
                    List<string> lstChildrentId = new List<string>();
                    if (lstValue.Contains(","))
                    {
                        lstChildrentId = lstValue.Split(',').ToList();
                    }
                    else
                    {
                        lstChildrentId.Add(lstValue);
                    }

                    VDT_KHV_KeHoachVonNam_DeXuat_ViewModel itemParent = lstKHVNDX.Where(x => x.iID_KeHoachVonNamDeXuatID == Guid.Parse(key)).FirstOrDefault();
                    List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChildrent = lstKHVNDX.Where(x => lstChildrentId.Any(y => Guid.Parse(y) == x.iID_KeHoachVonNamDeXuatID)).ToList();
                    lstChungTuTongHopParent = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                    lstChungTuIndex.Add(itemParent);
                    lstChungTuIndex.AddRange(lstChildrent);
                }
                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChungTu = lstKHVNDX.Where(x => !lstChungTuIndex.Any(y => y.iID_KeHoachVonNamDeXuatID == x.iID_KeHoachVonNamDeXuatID)).ToList();

                vm.Items = lstChungTu;
                vm._paging.TotalItems = lstChungTu.Count();
                vm.chungTuTabIndex = "checked";
                vm.chungTuTongHopTabIndex = "";
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }*/ // Index truoc khi sua

        public ActionResult Index()
        {
            KeHoachVonNamDeXuatViewModel vm = new KeHoachVonNamDeXuatViewModel();

            try
            {
                ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");

                vm._paging.CurrentPage = 1;

                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChungTu = _qLVonDauTuService.GetAllKeHoachVonNamDeXuat(ref vm._paging, null, null, null, null, null, null, 1).ToList();


                vm.Items = lstChungTu;
                vm.chungTuTabIndex = "checked";
                vm.chungTuTongHopTabIndex = "";
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }
        /*
        [HttpPost]
        public ActionResult KeHoachVonNamDeXuatSearch(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, int? iNamKeHoach, int? iID_NguonVonID, Guid? iID_DonViQuanLyID, int? tabIndex)
        {
            KeHoachVonNamDeXuatViewModel vm = new KeHoachVonNamDeXuatViewModel();

            try
            {
                ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");

                vm._paging = _paging;

                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstKHVNDX = _qLVonDauTuService.GetAllKeHoachVonNamDeXuat(ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, null).ToList();
                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChungTuTongHopParent = lstKHVNDX.Where(x => !string.IsNullOrEmpty(x.sTongHop)).ToList();
                Dictionary<string, List<string>> dctChungTu = lstChungTuTongHopParent.GroupBy(x => x.iID_KeHoachVonNamDeXuatID).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstAllChungTuTongHopParent = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                foreach (var key in dctChungTu.Keys)
                {
                    string lstValue = dctChungTu[key].ToList().FirstOrDefault();
                    List<string> lstChildrentId = new List<string>();
                    if (lstValue.Contains(","))
                    {
                        lstChildrentId = lstValue.Split(',').ToList();
                    }
                    else
                    {
                        lstChildrentId.Add(lstValue);
                    }

                    VDT_KHV_KeHoachVonNam_DeXuat_ViewModel itemParent = lstKHVNDX.Where(x => x.iID_KeHoachVonNamDeXuatID == Guid.Parse(key)).FirstOrDefault();
                    List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChildrent = lstKHVNDX.Where(x => lstChildrentId.Any(y => Guid.Parse(y) == x.iID_KeHoachVonNamDeXuatID)).ToList();
                    // lstChungTuTongHopParent = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                    lstAllChungTuTongHopParent.Add(itemParent);
                    lstAllChungTuTongHopParent.AddRange(lstChildrent);
                }
                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChungTu = lstKHVNDX.Where(x => !lstAllChungTuTongHopParent.Any(y => y.iID_KeHoachVonNamDeXuatID == x.iID_KeHoachVonNamDeXuatID)).ToList();

                if (tabIndex == null || tabIndex.Value == 1)
                {
                    vm.chungTuTabIndex = "checked";
                    vm.chungTuTongHopTabIndex = "";
                    vm.Items = lstChungTu;
                }
                else
                {
                    vm.chungTuTabIndex = "";
                    vm.chungTuTongHopTabIndex = "checked";
                    vm.Items = lstAllChungTuTongHopParent;
                }
                vm._paging.TotalItems = vm.Items.Count();
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_list", vm);
        }
        */// KeHoachVonNamDeXuatSearch truoc khi sua
        [HttpPost]
        public ActionResult KeHoachVonNamDeXuatSearch(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, int? iNamKeHoach, int? iID_NguonVonID, Guid? iID_DonViQuanLyID, int? tabIndex)
        {
            KeHoachVonNamDeXuatViewModel vm = new KeHoachVonNamDeXuatViewModel();

            try
            {
                ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");

                vm._paging = _paging;



                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstChungTu = _qLVonDauTuService.GetAllKeHoachVonNamDeXuat(ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, tabIndex).ToList();

                List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstAllChungTuTongHopParent = _qLVonDauTuService.GetAllKeHoachVonNamDeXuat(ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, tabIndex).ToList();

                if (tabIndex == null || tabIndex.Value == 1)
                {
                    vm.chungTuTabIndex = "checked";
                    vm.chungTuTongHopTabIndex = "";
                    vm.Items = lstChungTu;
                }
                else
                {
                    vm.chungTuTabIndex = "";
                    vm.chungTuTongHopTabIndex = "checked";
                    vm.Items = lstAllChungTuTongHopParent;
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id, bool isTongHop, bool bIsDetail, List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> lstItem)
        {
            VDT_KHV_KeHoachVonNam_DeXuat_ViewModel data = new VDT_KHV_KeHoachVonNam_DeXuat_ViewModel();
            string lstDuAn = string.Empty;
            try
            {
                _lstTongHop = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();

                if (id.HasValue)
                {
                    data = _qLVonDauTuService.GetKeHoachVonNamDeXuatViewModelById(id);
                    List<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet> dataChiTiet = _qLVonDauTuService.GetKHVNDeXuatChiTietByParentId(id.Value);
                    if (dataChiTiet != null && dataChiTiet.Count > 0)
                        lstDuAn = dataChiTiet.Select(x => x.iID_DuAnID).Join();
                }
                else if (!isTongHop)
                {
                    data.iNamKeHoach = DateTime.Now.Year;
                    data.dNgayQuyetDinh = DateTime.Now;
                }

                if (isTongHop)
                {
                    ViewBag.IsTongHop = true;
                    if (lstItem != null && lstItem.Count() > 0 && lstItem.FirstOrDefault() != null)
                    {
                        var lstValue = lstItem.GroupBy(x => x.iID_KeHoachVonNamDeXuatID).Select(grp => grp.LastOrDefault()).Where(x => x.isChecked).ToList();

                        foreach (var item in lstValue)
                        {
                            var itemQuery = _qLVonDauTuService.GetKeHoachVonNamDeXuatByIdDetail(item.iID_KeHoachVonNamDeXuatID);
                            if (itemQuery != null)
                            {
                                _lstTongHop.Add(itemQuery);
                            }
                        }

                        _lstTongHop = _lstTongHop.GroupBy(item => item.iID_KeHoachVonNamDeXuatID).Select(grp => grp.FirstOrDefault()).ToList();

                        if (_lstTongHop.Count() == 0)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ để tổng hợp." }, JsonRequestBehavior.AllowGet);
                        }

                        if (_lstTongHop.GroupBy(x => new { x.iNamKeHoach }).Count() > 1)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có cùng năm." }, JsonRequestBehavior.AllowGet);
                        }

                        if (_lstTongHop.GroupBy(x => new { x.sTenNguonVon }).Count() > 1)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có cùng nguồn vốn." }, JsonRequestBehavior.AllowGet);
                        }

                        if (_lstTongHop.Any(x => x.iKhoa.HasValue && !x.iKhoa.Value))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn các chứng từ đã khóa." }, JsonRequestBehavior.AllowGet);
                        }

                        if (_lstTongHop.Any(x => !string.IsNullOrEmpty(x.sTongHop)))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Chứng từ đã được tổng hợp." }, JsonRequestBehavior.AllowGet);
                        }

                        ViewBag.LstTongHop = _lstTongHop;
                        data.iNamKeHoach = _lstTongHop.FirstOrDefault().iNamKeHoach;
                        data.dNgayQuyetDinh = DateTime.Now;
                    }
                    else
                    {
                        ViewBag.LstTongHop = new List<VDT_KHV_KeHoachVonNam_DeXuat>();

                        if (_lstTongHop.Count() == 0)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ để tổng hợp." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                }
                else
                {
                    var lstvalue = _qLVonDauTuService.GetSTongHopKeHoachVonNamDeXuat(id);
                    ViewBag.lsttonghop = lstvalue;
                    if (lstvalue.Count > 0)
                    {
                        ViewBag.IsTongHop = true;
                        ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                        //data.dNgayQuyetDinh = DateTime.Now;
                    }
                    else
                    {
                        ViewBag.IsTongHop = false;
                        ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                        //data.dNgayQuyetDinh = DateTime.Now;
                    }
                }
                ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
                ViewBag.lstDuAn = lstDuAn;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDT_KHV_KeHoachVonNam_DeXuat_ViewModel data = new VDT_KHV_KeHoachVonNam_DeXuat_ViewModel();
            try
            {
                data = _qLVonDauTuService.GetKeHoachVonNamDeXuatByIdDetail(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult KeHoachVonNamDeXuatSave(VDT_KHV_KeHoachVonNam_DeXuat data, bool isDieuChinh, bool isTongHop, bool bIsDetail, string strDuAnID)
        {
            Guid iID = Guid.Empty;

            try
            {
                TempData["lstDuAnID"] = strDuAnID;

                List<KeHoachVonNamChiTietViewModel> details = new List<KeHoachVonNamChiTietViewModel>();

                if (data.iID_KeHoachVonNamDeXuatID == Guid.Empty)
                {
                    if (!isTongHop && (string.IsNullOrEmpty(strDuAnID) || strDuAnID.Equals("")))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không thể tạo mới,chưa có dự án nào được duyệt !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (_qLVonDauTuService.CheckExistSoKeHoachVonNamDeXuat(data.sSoQuyetDinh, data.iNamKeHoach, null))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (_lstTongHop != null && _lstTongHop.Count() > 0)
                    {
                        List<Guid> lstIdVonNam = _lstTongHop.GroupBy(item => item.iID_KeHoachVonNamDeXuatID).Select(grp => grp.LastOrDefault().iID_KeHoachVonNamDeXuatID).ToList();
                        lstIdVonNam.Select(item =>
                        {
                            List<KeHoachVonNamChiTietViewModel> lstQuery = _qLVonDauTuService.GetAllVonNamDeXuatByIdDx(item).ToList();
                            details.AddRange(lstQuery);
                            return item;
                        }).ToList();
                        data.sTongHop = string.Join(",", lstIdVonNam);

                        // Confirm TODO
                        //NS_DonVi donVi = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).FirstOrDefault();
                        //if (donVi != null)
                        //{
                        //    //data.iID_DonViQuanLyID = donVi.iID_Ma;
                        //    //data.iID_MaDonViQuanLy = donVi.iID_MaDonVi;
                        //}
                        //Confirm
                    }

                    if (!_qLVonDauTuService.SaveKeHoachVonNamDeXuat(ref iID, data, details, Username, isDieuChinh, isTongHop, bIsDetail))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    details = _qLVonDauTuService.GetAllKHVonNamDeXuatDieuChinhChiTiet(data.iID_KeHoachVonNamDeXuatID).ToList();
                    if (isDieuChinh && details.Count() == 0)
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không thể điều chỉnh,chưa có dự án nào được duyệt !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (_qLVonDauTuService.CheckExistSoKeHoachVonNamDeXuat(data.sSoQuyetDinh, data.iNamKeHoach, data.iID_KeHoachVonNamDeXuatID))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (!_qLVonDauTuService.SaveKeHoachVonNamDeXuat(ref iID, data, details, Username, isDieuChinh, isTongHop, bIsDetail))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true, iID = iID }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool KeHoachVonNamDeXuatDelete(Guid id)
        {
            try
            {
                return _qLVonDauTuService.DeleteKeHoachVonNamDeXuat(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        [HttpPost]
        public ActionResult LoadDataDuAn(Guid iID_DonViID, int iNamKeHoach, int iID_NguonVonID)
        {
            List<VDTDuAnViewModel> data = new List<VDTDuAnViewModel>();
            try
            {
                data = _qLVonDauTuService.GetDuAnKHVNDXByDonVi(iID_DonViID, iNamKeHoach, iID_NguonVonID).ToList();
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_listDuAn", data);
        }

        #region Kế hoạch vốn năm đề xuất chi tiết
        public ActionResult Detail(Guid id, bool isDieuChinh, bool bIsDetail = false)
        {
            VDT_KHV_KeHoachVonNam_DeXuat data = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(id);
            KeHoachVonNamDeXuatChiTietGridViewModel vm = new KeHoachVonNamDeXuatChiTietGridViewModel
            {
                KHVonNamDeXuat = data
            };
            TempData["iNamKeHoach"] = data.iNamKeHoach;
            TempData["isDieuChinh"] = isDieuChinh;
            TempData["BIsDetail"] = bIsDetail;
            TempData.Keep("lstDuAnID");
            ViewBag.BIsDetail = bIsDetail ? "true" : "false";
            return View(vm);
        }

        public ActionResult SheetFrame(string id, string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            //VDT_KHV_KeHoachVonNam_DeXuat KHVonNamDX = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(Guid.Parse(id));
            TempData.Keep("iNamKeHoach");
            TempData.Keep("isDieuChinh");
            TempData.Keep("BIsDetail");
            int iNamKeHoach = (int)TempData["iNamKeHoach"];
            bool isDieuChinh = (bool)TempData["isDieuChinh"];
            bool bIsDetail = (bool)TempData["BIsDetail"];
            string lstDuAnID = (string)TempData["lstDuAnID"];
            TempData.Keep("iNamKeHoach");
            TempData.Keep("isDieuChinh");
            TempData.Keep("BIsDetail");
            TempData.Keep("lstDuAnID");
            if (string.IsNullOrEmpty(lstDuAnID))
            {
                var lstData = _qLVonDauTuService.GetKHVNDeXuatChiTietByParentId(Guid.Parse(id));
                if (lstData != null)
                    lstDuAnID = string.Join(",", lstData.Select(n => n.iID_DuAnID).Distinct());
            }
            var sheet = new KeHoachVonNamDeXuat_ChiTiet_SheetTable(id, iNamKeHoach, DateTime.Now, string.Empty, 0, isDieuChinh, bIsDetail, lstDuAnID, filters);
            var vm = new KeHoachVonNamDeXuatChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("SaveChiTiet", "KeHoachVonNamDeXuat", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrame", "KeHoachVonNamDeXuat", new { area = "QLVonDauTu" })
                   ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public JsonResult SaveChiTiet(List<KeHoachVonNamChiTietViewModel> aListModel, List<KeHoachVonNamChiTietViewModel> listItemDeleted)
        {
            try
            {
                if (aListModel == null)
                {
                    return Json(new { status = false });
                }
                var result = _qLVonDauTuService.SaveKeHoachVonNamDeXuatChiTiet(aListModel, listItemDeleted);
                if (result == false)
                {
                    Json(new { status = false });
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true });
        }

        [HttpPost]
        public bool KeHoachVonNamDeXuatLock(Guid id)
        {
            try
            {
                VDT_KHV_KeHoachVonNam_DeXuat entity = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(id);
                if (entity != null)
                {
                    bool isLockOrUnlock = entity.iKhoa.HasValue ? entity.iKhoa.Value : false;
                    return _qLVonDauTuService.LockOrUnLockKeHoachVonNamDeXuat(id, !isLockOrUnlock);
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        #endregion

        #region Import KH Vốn năm đề xuất
        public ActionResult ImportKHVNDX()
        {
            KeHoachVonNamDeXuatChiTietGridViewModel vm = new KeHoachVonNamDeXuatChiTietGridViewModel();
            VDT_KHV_KeHoachVonNam_DeXuat KHVonNamDeXuat = new VDT_KHV_KeHoachVonNam_DeXuat();
            KHVonNamDeXuat.iNamKeHoach = DateTime.Now.Year;
            vm.KHVonNamDeXuat = KHVonNamDeXuat;

            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
            ViewBag.ListNguonVon = _qLVonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
            return View(vm);
        }

        [HttpPost]
        public JsonResult LoadDataExcel(HttpPostedFileBase file)
        {
            try
            {
                var file_data = getBytes(file);
                var dt = ExcelHelpers.LoadExcelDataTable(file_data);
                IEnumerable<KeHoachVonNamDeXuatDataImportModel> dataImport = excel_result(dt);
                TempData["dataImport"] = dataImport;
                TempData["dataImportNew"] = dataImport;
                TempData.Keep("dataImport");
                TempData.Keep("dataImportNew");

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet); ;
        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private IEnumerable<KeHoachVonNamDeXuatDataImportModel> excel_result(DataTable dt)
        {
            List<KeHoachVonNamDeXuatDataImportModel> dataImport = new List<KeHoachVonNamDeXuatDataImportModel>();

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) != "" || x.Field<string>(0) != null || x.Field<string>(0) != "null");
            for (var i = 10; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];

                var sSTT = r.Field<string>(0);
                var sTenDuAn = r.Field<string>(1);
                var sMaDuAn = r.Field<string>(2);
                var sChuDauTu = r.Field<string>(3);
                var sMaChuDauTu = r.Field<string>(4);
                var sDonViQL = r.Field<string>(5);
                var sMaDonViQL = r.Field<string>(6);
                var sTongMucDauTuDuocDuyet = r.Field<string>(7);
                var sLuyKeVonNamTruoc = r.Field<string>(8);
                var sKeHoachVonDuocDuyetNamNay = r.Field<string>(10);
                var sVonKeoDaiCacNamTruoc = r.Field<string>(11);
                var sUocThucHien = r.Field<string>(12);
                var sThuHoiVonUngTruoc = r.Field<string>(15);
                var sThanhToan = r.Field<string>(16);
                var sLoaiDuAn = r.Field<string>(21) == "2" ? "Chuyển tiếp" : "Mở mới";
                var e = new KeHoachVonNamDeXuatDataImportModel
                {
                    sSTT = sSTT,
                    sTenDuAn = sTenDuAn,
                    sMaDuAn = sMaDuAn,
                    sChuDauTu = sChuDauTu,
                    sMaChuDauTu = sMaChuDauTu,
                    sDonViQuanLy = sDonViQL,
                    sMaDonViQuanLy = sMaDonViQL,
                    fTongMucDauTuDuocDuyet = sTongMucDauTuDuocDuyet,
                    fLuyKeVonNamTruoc = sLuyKeVonNamTruoc,
                    fKeHoachVonDuocDuyetNamNay = sKeHoachVonDuocDuyetNamNay,
                    fVonKeoDaiCacNamTruoc = sVonKeoDaiCacNamTruoc,
                    fUocThucHien = sUocThucHien,
                    fThuHoiVonUngTruoc = sThuHoiVonUngTruoc,
                    fThanhToan = sThanhToan,
                    sLoaiDuAn = sLoaiDuAn
                };

                dataImport.Add(e);
            }
            return dataImport.AsEnumerable();
        }

        public ActionResult SheetFrameImport(string id, string filter = null, bool bIsImport = false)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            IEnumerable<KeHoachVonNamDeXuatDataImportModel> dataImport = null;
             dataImport=  (IEnumerable<KeHoachVonNamDeXuatDataImportModel>)TempData["dataImport"];
            TempData.Keep("dataImport");
            if (bIsImport)
            {
                dataImport = (IEnumerable<KeHoachVonNamDeXuatDataImportModel>)TempData["dataImportNew"];
                TempData.Keep("dataImportNew");
            }
            IEnumerable<VDT_DA_DuAn> listDuAns = (IEnumerable<VDT_DA_DuAn>)TempData["listDuAns"];
            VDT_KHV_KeHoachVonNam_DeXuat entity = (VDT_KHV_KeHoachVonNam_DeXuat)TempData["KhvnDeXuat"];
            Guid iID_KeHoachVonNamDeXuatID = new Guid();
            string listIdDuAn = "";
            if (entity != null)
            {
                iID_KeHoachVonNamDeXuatID = entity.iID_KeHoachVonNamDeXuatID;
            }

            if(listDuAns != null)
            {
                listIdDuAn = string.Join(",",listDuAns.Select(x => x.iID_DuAnID));
            }

            if (dataImport == null)
            {
                dataImport = new List<KeHoachVonNamDeXuatDataImportModel>().AsEnumerable();
            }
            var iNamKeHoach = Int16.Parse(id);
            var datatableData = ToDataTable(dataImport);
            var sheet = new KeHoachVonNamDeXuat_Import_SheetTable(datatableData, int.Parse(PhienLamViec.iNamLamViec), iNamKeHoach, bIsImport, iID_KeHoachVonNamDeXuatID, listIdDuAn, filters);
            var vm = new KeHoachVonNamDeXuatChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("SaveImport", "KeHoachVonNamDeXuat", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrameImport", "KeHoachVonNamDeXuat", new { area = "QLVonDauTu" })
                   ),
            };
            //TempData["dataImport"] = null;
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrameImport", vm);
        }

        public DataTable ToDataTable<KeHoachVonNamDeXuatDataImportModel>(IEnumerable<KeHoachVonNamDeXuatDataImportModel> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(KeHoachVonNamDeXuatDataImportModel));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (KeHoachVonNamDeXuatDataImportModel item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        #endregion

        #region In báo cáo
        public ActionResult ViewInBaoCao(string isStatus)
        {
            KHVNDXPrintDataExportModel vm = new KHVNDXPrintDataExportModel();

            try
            {
                IEnumerable<NS_NguonNganSach> lstDMNguonNganSach = _qLVonDauTuService.GetListDMNguonNganSach();
                vm.lstDMNguonNganSach = lstDMNguonNganSach;
                IEnumerable<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec);
                vm.lstDonViQuanLy = lstDonViQuanLy;
                vm.iNamKeHoach = DateTime.Now.Year;
                ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_MaDonVi", "sMoTa");
                List<VDT_DM_LoaiCongTrinh> lstLoaiCongTrinh = _qLVonDauTuService.GetAllDMLoaiCongTrinhExportKHVNDX();
                List<VDT_KHV_KeHoachVonNam_DeXuat> lstChungTuTH = _qLVonDauTuService.GetListChungTuTH(lstDonViQuanLy.FirstOrDefault().iID_MaDonVi, DateTime.Now.Year, 1).ToList();
                lstLoaiCongTrinh.Insert(0, new VDT_DM_LoaiCongTrinh { iID_LoaiCongTrinh = Guid.Empty, sTenLoaiCongTrinh = "--Tất cả--" });
                ViewBag.ListLoaiCongTrinh = lstLoaiCongTrinh.ToSelectList("iID_LoaiCongTrinh", "sTenLoaiCongTrinh");
                ViewBag.ListChungTuTH = lstChungTuTH.ToSelectList("iID_KeHoachVonNamDeXuatID", "sSoQuyetDinh");

                ViewBag.LstDonViTinh = LoadDonViTinh().ToSelectList("ValueItem", "DisplayItem");
                ViewBag.LstNguonVon = lstDMNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }
        [HttpPost]
        public JsonResult GetDataChungtu(string idMadv, int namKh, int iLoaiChungTu)
        {
            List<VDT_KHV_KeHoachVonNam_DeXuat> lstChungTuTH = _qLVonDauTuService.GetListChungTuTH(idMadv, namKh, iLoaiChungTu).ToList();
            //ViewBag.ListChungTuTH = lstChungTuTH.ToSelectList("iID_KeHoachVonNamDeXuatID", "sSoQuyetDinh");
            return Json(new { lstData = lstChungTuTH });
        }

        [HttpPost]
        public JsonResult ExportBaoCao(KHVNDXPrintDataExportModel data, List<string> arrIdNguonVon, List<string> arrDonVi, bool bIsTongHop, bool isPdf)
        {
            string templateFileName = string.Empty;
            FlexCelReport fr = new FlexCelReport();
            List<string> lstId = new List<string>();
            if (bIsTongHop)
            {
                lstId.Add(data.iID_KeHoachVonNam_DeXuatID.ToString());
            }
            else
            {
                lstId.AddRange(_qLVonDauTuService.GetVoucherVonNamDeXuatIdByCondition(data, arrDonVi, arrIdNguonVon));
            }
            if (lstId == null || lstId.Count == 0)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            if (data.iLoaiChungTu == RPT_GOC)
            {
                if (data.iIDNguonVon == 1)
                {
                    fr = HandleBaoCaoGocNSQP(lstId, data, arrIdNguonVon, arrDonVi);
                    if (data.iIdCongTrinh == RPT_CONGTRINH_MOMOI)
                    {
                        templateFileName = "rpt_BaoCao_KHNam_DonVi_CTMM.xlsx";
                    }
                    else
                    {
                        templateFileName = "rpt_BaoCao_KHNam_DonVi_CTCT.xlsx";
                    }
                }
                else
                {
                    fr = HandleBaoCaoGocNS(lstId, data, arrIdNguonVon, arrDonVi);
                    templateFileName = "rpt_BaoCao_KHNam_DeXuat_Goc_NguonVonKhac.xlsx";
                }
            }
            else
            {
                if (data.iIdBaoCaoMau == 1)
                {
                    fr = HandleBaoCaoDieuChinhNSQP(lstId, data, arrIdNguonVon, arrDonVi);
                    templateFileName = "rpt_BaoCao_KHNam_DonVi_DieuChinh.xlsx";
                }
                else
                {
                    fr = HandleBaoCaoGocNS(lstId, data, arrIdNguonVon, arrDonVi);
                    templateFileName = "rpt_BaoCao_KHNam_DeXuat_DieuChinh_NguonVonKhac.xlsx";
                }
            }


            ExcelFile xls = new XlsFile(true);
            xls.Open(Server.MapPath(Path.Combine("~/Areas/QLVonDauTu/ReportExcelForm", templateFileName)));
            xls.PrintLandscape = false;
            FormatNumber formatNumber = new FormatNumber((int)data.fDonViTinh, (isPdf ? ExportType.PDF : ExportType.EXCEL));
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.SetValue("DonViCapTren", "BỘ QUỐC PHÒNG");
            fr.SetValue("DonVi", "");
            fr.SetValue("TitleFirst", data.txt_TieuDe1);
            fr.SetValue("TitleSecond", data.txt_TieuDe2);
            fr.SetValue("DonViTinh", data.sDonViTinh);
            fr.UseChuKy(Username)
                    .UseChuKyForController(sControlName)
                    .UseForm(this);
            fr.Run(xls);


            TempData["DataReportXls"] = xls;
            FlexCelPdfExport pdf = new FlexCelPdfExport(xls, true);
            var bufferPdf = new MemoryStream();
            pdf.Export(bufferPdf);
            TempData["DataReportPdf"] = bufferPdf;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public FlexCelReport HandleBaoCaoGocNSQP(IEnumerable<string> lstId, KHVNDXPrintDataExportModel data, List<string> arrIdNguonVon, List<string> arrDonVi)
        {
            List<PhanBoVonDonViDieuChinhNSQPReport> results = new List<PhanBoVonDonViDieuChinhNSQPReport>();
            if (data.iIdCongTrinh == "1" || data.iIdCongTrinh == "0")
            {
                var lstItem = _qLVonDauTuService.GetPhanBoVonDieuChinhReport(String.Join(",", lstId), data.iID_LoaiCongTrinh, data.iNamKeHoach, int.Parse(RPT_CONGTRINH_MOMOI), (1));
                if (lstItem != null)
                {
                    lstItem = lstItem.Select(x =>
                    {
                        if (x.IdDuAn.HasValue)
                        {
                            var qddt = _qLVonDauTuService.FindQDDauTuByDuAnId(x.IdDuAn.Value);
                            if (qddt != null)
                            {
                                var lstNguonVon = _qLVonDauTuService.FindNguonVonByParentId(qddt.iID_QDDauTuID).Where(y => y.iID_NguonVonID == x.IdNguonVon);
                                if (lstNguonVon != null)
                                    x.TongMucDauTuNSQP = lstNguonVon.Sum(n => n.fTienPheDuyet.GetValueOrDefault()) / (1);
                            }
                        }
                        return x;
                    }).ToList();
                    results.AddRange(CalculateDataReportGoc(data, lstItem.ToList()));
                }
            }
            if (data.iIdCongTrinh == "2" || data.iIdCongTrinh == "0")
            {
                var lstItem = _qLVonDauTuService.GetPhanBoVonDieuChinhReport(String.Join(",", lstId), data.iID_LoaiCongTrinh, data.iNamKeHoach, int.Parse(RPT_CONGTRINH_CHUYENTIEP), ( 1));
                if (lstItem != null)
                {
                    lstItem = lstItem.Select(x =>
                    {
                        if (x.IdDuAn.HasValue)
                        {
                            var qddt = _qLVonDauTuService.FindQDDauTuByDuAnId(x.IdDuAn.Value);
                            if (qddt != null)
                            {
                                var lstNguonVon = _qLVonDauTuService.FindNguonVonByParentId(qddt.iID_QDDauTuID).Where(y => y.iID_NguonVonID == x.IdNguonVon);
                                if (lstNguonVon != null)
                                    x.TongMucDauTuNSQP = lstNguonVon.Sum(n => n.fTienPheDuyet.GetValueOrDefault()) / ( 1);
                            }
                        }
                        return x;
                    }).ToList();
                    results.AddRange(CalculateDataReportGoc(data, lstItem.ToList()));
                }
            }
            FlexCelReport fr = new FlexCelReport();
            var a = results.Where(n => !n.IsHangCha).Sum(n => n.TongMucDauTuNSQP);
            fr.AddTable<PhanBoVonDonViDieuChinhNSQPReport>("Items", results);
            fr.SetValue("TongMucDauTuSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongMucDauTu));
            fr.SetValue("TongMucDauTuNSQPSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongMucDauTuNSQP));
            fr.SetValue("VonBoTriDenHetNamTruocSum", results.Where(n => !n.IsHangCha).Sum(n => n.VonBoTriDenHetNamTruoc));
            fr.SetValue("KeHoachVonDauTuNamSum", results.Where(n => !n.IsHangCha).Sum(n => n.KeHoachVonDauTuNam));
            fr.SetValue("VonGiaiNganNamSum", results.Where(n => !n.IsHangCha).Sum(n => n.VonGiaiNganNam));
            fr.SetValue("DieuChinhVonNamSum", results.Where(n => !n.IsHangCha).Sum(n => n.DieuChinhVonNam));
            return fr;
        }

        public FlexCelReport HandleBaoCaoDieuChinhNSQP(IEnumerable<string> lstId, KHVNDXPrintDataExportModel data, List<string> arrIdNguonVon, List<string> arrDonVi)
        {
            List<PhanBoVonDonViDieuChinhNSQPReport> results = new List<PhanBoVonDonViDieuChinhNSQPReport>();
            var lstItemMoMoi = _qLVonDauTuService.GetPhanBoVonDieuChinhReport(String.Join(",", lstId), data.iID_LoaiCongTrinh, data.iNamKeHoach, int.Parse(RPT_CONGTRINH_MOMOI), (data.fDonViTinh ?? 1));
            var lstItemChuyenTiep = _qLVonDauTuService.GetPhanBoVonDieuChinhReport(String.Join(",", lstId), data.iID_LoaiCongTrinh, data.iNamKeHoach, int.Parse(RPT_CONGTRINH_CHUYENTIEP), (data.fDonViTinh ?? 1));

            lstItemMoMoi = lstItemMoMoi.Select(x =>
            {
                if (data.iIDNguonVon == 1)
                {
                    x.TongMucDauTuNSQP = x.TongMucDauTu;
                }
                return x;
            }).ToList();

            lstItemChuyenTiep = lstItemChuyenTiep.Select(x =>
            {
                if (data.iIDNguonVon == 1)
                {
                    x.TongMucDauTuNSQP = x.TongMucDauTu;
                }
                return x;
            }).ToList();
            results.AddRange(CalculateDataReportGoc(data, lstItemMoMoi.ToList()));
            results.AddRange(CalculateDataReportGoc(data, lstItemChuyenTiep.ToList()));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<PhanBoVonDonViDieuChinhNSQPReport>("Items", results);
            fr.SetValue("TongMucDauTuSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongMucDauTu));
            fr.SetValue("TongMucDauTuNSQPSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongMucDauTuNSQP));
            fr.SetValue("VonBoTriDenHetNamTruocSum", results.Where(n => !n.IsHangCha).Sum(n => n.VonBoTriDenHetNamTruoc));
            fr.SetValue("KeHoachVonDauTuNamSum", results.Where(n => !n.IsHangCha).Sum(n => n.KeHoachVonDauTuNam));
            fr.SetValue("VonGiaiNganNamSum", results.Where(n => !n.IsHangCha).Sum(n => n.VonGiaiNganNam));
            fr.SetValue("DieuChinhVonNamSum", results.Where(n => !n.IsHangCha).Sum(n => n.DieuChinhVonNam));
            return fr;
        }

        public FlexCelReport HandleBaoCaoGocNS(IEnumerable<string> lstId, KHVNDXPrintDataExportModel data, List<string> arrIdNguonVon, List<string> arrDonVi)
        {
            List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> results = _qLVonDauTuService.GetPhanBoVonDieuChinhNguonVon(0, String.Join(",", lstId), data.iID_LoaiCongTrinh, string.Join(",", arrIdNguonVon), (data.fDonViTinh ?? 1)).ToList();
            results.Select(item => { item.TongSoVonNamDieuChinh = (item.ThuHoiVonDaUngTruocDieuChinh ?? 0) + (item.TraNoXDCB ?? 0); return item; }).ToList();

            List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> lstParent = results.Where(x => x.IsHangCha).ToList();
            lstParent.Select(x => { x.STT = (lstParent.IndexOf(x) + 1).ToString(); return x; }).ToList();
            List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> lstChildrent = results.Where(x => !x.IsHangCha).ToList();
            lstParent.Select(x =>
            {
                List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> item = lstChildrent.Where(y => y.IdNhomDuAn == x.IdNhomDuAn).ToList();
                item.Select(y => { y.STT = string.Format("{0}.{1}", x.STT, item.IndexOf(y) + 1); return y; }).ToList();
                return x;
            }).ToList();

            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel>("Items", results);
            fr.SetValue("Year", data.iNamKeHoach);
            fr.SetValue("TongSoVonDauTuSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongSoVonDauTu));
            fr.SetValue("TongSoVonDauTuTrongNuocSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongSoVonDauTuTrongNuoc));
            fr.SetValue("KeHoachVonDauTuGiaiDoanSum", results.Where(n => !n.IsHangCha).Sum(n => n.KeHoachVonDauTuGiaiDoan));
            fr.SetValue("VonThanhToanLuyKeSum", results.Where(n => !n.IsHangCha).Sum(n => n.VonThanhToanLuyKe));
            fr.SetValue("TongSoKeHoachVonNamSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongSoKeHoachVonNam));
            fr.SetValue("ThuHoiVonDaUngTruocSum", results.Where(n => !n.IsHangCha).Sum(n => n.ThuHoiVonDaUngTruoc));
            fr.SetValue("VonThucHienTuDauNamDenNaySum", results.Where(n => !n.IsHangCha).Sum(n => n.VonThucHienTuDauNamDenNay));
            fr.SetValue("TongSoVonNamDieuChinhSum", results.Where(n => !n.IsHangCha).Sum(n => n.TongSoVonNamDieuChinh));
            fr.SetValue("ThuHoiVonDaUngTruocDieuChinhSum", results.Where(n => !n.IsHangCha).Sum(n => n.ThuHoiVonDaUngTruocDieuChinh));
            fr.SetValue("TraNoXDCBSum", results.Where(n => !n.IsHangCha).Sum(n => n.TraNoXDCB));
            return fr;
        }

        public JsonResult LayDanhSachChungTuDeXuatTheoDonViQuanLy(string iID_DonViQuanLyID, string isStatus)
        {
            List<VDT_KHV_KeHoachVonNam_DeXuat> lstChungTuDeXuat = _qLVonDauTuService.GetListSoChungTuVonNamDeXuat(!string.IsNullOrEmpty(iID_DonViQuanLyID) ? Guid.Parse(iID_DonViQuanLyID) : Guid.Empty, PhienLamViec.NamLamViec, isStatus).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", "", Constants.CHON);
            if (lstChungTuDeXuat != null && lstChungTuDeXuat.Count > 0)
            {
                for (int i = 0; i < lstChungTuDeXuat.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}'>{1}</option>", lstChungTuDeXuat[i].iID_KeHoachVonNamDeXuatID, lstChungTuDeXuat[i].sSoQuyetDinh);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        private List<PhanBoVonDonViDieuChinhNSQPReport> CalculateDataReportGoc(KHVNDXPrintDataExportModel dataSearch, List<PhanBoVonDonViDieuChinhNSQPReport> data)
        {
            List<PhanBoVonDonViDieuChinhNSQPReport> result = new List<PhanBoVonDonViDieuChinhNSQPReport>();
            try
            {
                List<PhanBoVonDonViDieuChinhNSQPReport> childrent = data.Where(x => !x.IsHangCha).ToList();
                List<PhanBoVonDonViDieuChinhNSQPReport> childrentResult = new List<PhanBoVonDonViDieuChinhNSQPReport>();
                var lstIdDuAn = childrent.Select(x => x.IdDuAn).Distinct().ToList();
                foreach (var idDuAn in lstIdDuAn)
                {
                    var it = childrent.FirstOrDefault(x => x.IdDuAn == idDuAn);
                    if (it != null)
                    {
                        it.TongMucDauTu = childrent.Where(x => x.IdDuAn == idDuAn).Sum(x => x.TongMucDauTu.GetValueOrDefault());
                        it.TongMucDauTuNSQP = childrent.Where(x => x.IdDuAn == idDuAn).Sum(x => x.TongMucDauTuNSQP.GetValueOrDefault());
                        it.VonBoTriDenHetNamTruoc = childrent.Where(x => x.IdDuAn == idDuAn).Sum(x => x.VonBoTriDenHetNamTruoc.GetValueOrDefault());
                        it.KeHoachVonDauTuNam = childrent.Where(x => x.IdDuAn == idDuAn).Sum(x => x.KeHoachVonDauTuNam.GetValueOrDefault());
                        it.VonGiaiNganNam = childrent.Where(x => x.IdDuAn == idDuAn).Sum(x => x.VonGiaiNganNam.GetValueOrDefault());
                        it.DieuChinhVonNam = childrent.Where(x => x.IdDuAn == idDuAn).Sum(x => x.DieuChinhVonNam.GetValueOrDefault());
                        childrentResult.Add(it);
                    }
                }
                data.RemoveAll(x => !x.IsHangCha);
                data.AddRange(childrentResult);
                List<PhanBoVonDonViDieuChinhNSQPReport> parent = data.Where(x => x.IsHangCha && (x.LoaiParent.Equals(0) || x.LoaiParent.Equals(1))).ToList();

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
                    List<PhanBoVonDonViDieuChinhNSQPReport> lstChilrent = childrentResult.Where(x => x.IIdLoaiCongTrinh.Equals(pr.IIdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => ((x.TongMucDauTu != 0 && x.TongMucDauTu.HasValue)
                        || (x.TongMucDauTuNSQP != 0 && x.TongMucDauTuNSQP.HasValue)
                        || (x.VonBoTriDenHetNamTruoc != 0 && x.VonBoTriDenHetNamTruoc.HasValue)
                        || (x.KeHoachVonDauTuNam != 0 && x.KeHoachVonDauTuNam.HasValue)
                        || (x.VonGiaiNganNam != 0 && x.VonGiaiNganNam.HasValue)
                        || (x.DieuChinhVonNam != 0 && x.DieuChinhVonNam.HasValue))))
                    {
                        pr.TongMucDauTu += item.TongMucDauTu;
                        if(!pr.TongMucDauTuNSQP.HasValue) pr.TongMucDauTuNSQP = 0;
                        pr.TongMucDauTuNSQP += item.TongMucDauTuNSQP;
                        pr.VonBoTriDenHetNamTruoc += item.VonBoTriDenHetNamTruoc;
                        pr.KeHoachVonDauTuNam += item.KeHoachVonDauTuNam;
                        pr.VonGiaiNganNam += item.VonGiaiNganNam;
                        pr.DieuChinhVonNam += item.DieuChinhVonNam;
                    }
                }

                foreach (var item in parent.Where(x => ((x.TongMucDauTu != 0 && x.TongMucDauTu.HasValue)
                    || (x.TongMucDauTuNSQP != 0 && x.TongMucDauTuNSQP.HasValue)
                    || (x.VonBoTriDenHetNamTruoc != 0 && x.VonBoTriDenHetNamTruoc.HasValue)
                    || (x.KeHoachVonDauTuNam != 0 && x.KeHoachVonDauTuNam.HasValue)
                    || (x.VonGiaiNganNam != 0 && x.VonGiaiNganNam.HasValue)
                    || (x.DieuChinhVonNam != 0 && x.DieuChinhVonNam.HasValue)) && x.IIdLoaiCongTrinh != null))
                {
                    CalculateParentGoc(item, item, data);
                }

                result = data.Where(x => ((x.TongMucDauTu != 0 && x.TongMucDauTu.HasValue)
                    || (x.TongMucDauTuNSQP != 0 && x.TongMucDauTuNSQP.HasValue)
                    || (x.VonBoTriDenHetNamTruoc != 0 && x.VonBoTriDenHetNamTruoc.HasValue)
                    || (x.KeHoachVonDauTuNam != 0 && x.KeHoachVonDauTuNam.HasValue)
                    || (x.VonGiaiNganNam != 0 && x.VonGiaiNganNam.HasValue)
                    || (x.DieuChinhVonNam != 0 && x.DieuChinhVonNam.HasValue)) || (x.IIdLoaiCongTrinh == null && x.IIdLoaiCongTrinhParent == null)).ToList();

                foreach (var child in result)
                {
                    if (child.IdDuAn != null && child.IdDuAn != Guid.Empty)
                    {
                        var vonBoTri5Nam = _qLVonDauTuService.GetVonBoTri5Nam(child.IdDuAn.ToString(), dataSearch.iNamKeHoach);
                        child.VonBoTri5Nam = (Double)vonBoTri5Nam / (dataSearch.fDonViTinh ?? 1);
                    }
                }

                List<PhanBoVonDonViDieuChinhNSQPReport> lstItem = data.Where(n => n.LoaiParent.Equals(0) && !n.Loai.Equals(1)).ToList();
                lstItem.Select(n => { n.STT = CommonFunction.ConvertLaMa((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
                List<PhanBoVonDonViDieuChinhNSQPReport> lstItemLevel = data.Where(x => !x.Loai.Equals(1) && x.LoaiParent.Equals(2)).ToList();
                lstItemLevel.Select(x => { x.STT = (lstItemLevel.IndexOf(x) + 1).ToString(); return x; }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        private void CalculateParentGoc(PhanBoVonDonViDieuChinhNSQPReport currentItem, PhanBoVonDonViDieuChinhNSQPReport seftItem, List<PhanBoVonDonViDieuChinhNSQPReport> data)
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
                CalculateParentGoc(parrentItem, seftItem, data);
            }
            catch (Exception ex)
            {
            }
        }

        public ActionResult ExportExcel(int pdf)
        {
            ExcelFile xls = (ExcelFile)TempData["DataReportXls"];
            return Print(xls, (pdf == 1) ? "pdf" : "xls", (pdf == 1) ? "KeHoachVonNamDeXuat.pdf" : "KeHoachVonNamDeXuat.xls");
        }

        public ExcelFile CreateReportDcOtherBudget(List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> dataReport, KHVNDXPrintDataExportModel paramReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachVonNam/rpt_BaoCao_KHNam_DeXuat_DieuChinh_NguonVonKhac.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel itemSummary = new VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel();
            dataReport.Select(item => { item.TongSoVonNamDieuChinh = (item.ThuHoiVonDaUngTruocDieuChinh ?? 0) + (item.TraNoXDCB ?? 0); return item; }).ToList();
            itemSummary.TongSoVonDauTu = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonDauTu);
            itemSummary.TongSoVonDauTuTrongNuoc = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonDauTuTrongNuoc);
            itemSummary.KeHoachVonDauTuGiaiDoan = dataReport.Where(x => !x.IsHangCha).Sum(x => x.KeHoachVonDauTuGiaiDoan);
            itemSummary.VonThanhToanLuyKe = dataReport.Where(x => !x.IsHangCha).Sum(x => x.VonThanhToanLuyKe);
            itemSummary.TongSoKeHoachVonNam = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TongSoKeHoachVonNam);
            itemSummary.ThuHoiVonDaUngTruoc = dataReport.Where(x => !x.IsHangCha).Sum(x => x.ThuHoiVonDaUngTruoc);
            itemSummary.VonThucHienTuDauNamDenNay = dataReport.Where(x => !x.IsHangCha).Sum(x => x.VonThucHienTuDauNamDenNay);
            itemSummary.TongSoVonNamDieuChinh = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TongSoVonNamDieuChinh);
            itemSummary.ThuHoiVonDaUngTruocDieuChinh = dataReport.Where(x => !x.IsHangCha).Sum(x => x.ThuHoiVonDaUngTruocDieuChinh);
            itemSummary.TraNoXDCB = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TraNoXDCB);

            fr.SetValue("TongSoVonDauTuSum", itemSummary.TongSoVonDauTu);
            fr.SetValue("TongSoVonDauTuTrongNuocSum", itemSummary.TongSoVonDauTuTrongNuoc);
            fr.SetValue("KeHoachVonDauTuGiaiDoanSum", itemSummary.KeHoachVonDauTuGiaiDoan);
            fr.SetValue("VonThanhToanLuyKeSum", itemSummary.VonThanhToanLuyKe);
            fr.SetValue("TongSoKeHoachVonNamSum", itemSummary.TongSoKeHoachVonNam);
            fr.SetValue("ThuHoiVonDaUngTruocSum", itemSummary.ThuHoiVonDaUngTruoc);
            fr.SetValue("VonThucHienTuDauNamDenNaySum", itemSummary.VonThucHienTuDauNamDenNay);
            fr.SetValue("TongSoVonNamDieuChinhSum", itemSummary.TongSoVonNamDieuChinh);
            fr.SetValue("ThuHoiVonDaUngTruocDieuChinhSum", itemSummary.ThuHoiVonDaUngTruocDieuChinh);
            fr.SetValue("TraNoXDCBSum", itemSummary.TraNoXDCB);

            VDT_KHV_KeHoachVonNam_DeXuat itemQuery = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(paramReport.iID_KeHoachVonNam_DeXuatID);

            int iNamKeHoach = 0;
            if (itemQuery != null)
            {
                iNamKeHoach = itemQuery.iNamKeHoach;
            }

            fr.SetValue("Year", iNamKeHoach);
            fr.SetValue("TitleFirst", paramReport.txt_TieuDe1);
            fr.SetValue("TitleSecond", paramReport.txt_TieuDe2);
            fr.SetValue("DonVi", string.Empty);
            fr.SetValue("DonViTinh", paramReport.sDonViTinh);
            fr.AddTable<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel>("Items", dataReport);
            fr.UseChuKy(Username)
                     .UseChuKyForController(sControlName)
                     .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        public ExcelFile CreateReportDcNsqp(List<PhanBoVonDonViDieuChinhNSQPReport> dataReport, KHVNDXPrintDataExportModel paramReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachVonNam/rpt_BaoCao_KHNam_DonVi_DieuChinh.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            PhanBoVonDonViDieuChinhNSQPReport itemSummary = new PhanBoVonDonViDieuChinhNSQPReport();
            itemSummary.TongMucDauTu = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TongMucDauTu);
            itemSummary.TongMucDauTuNSQP = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TongMucDauTuNSQP);
            itemSummary.VonBoTriDenHetNamTruoc = dataReport.Where(x => !x.IsHangCha).Sum(x => x.VonBoTriDenHetNamTruoc);
            itemSummary.KeHoachVonDauTuNam = dataReport.Where(x => !x.IsHangCha).Sum(x => x.KeHoachVonDauTuNam);
            itemSummary.VonGiaiNganNam = dataReport.Where(x => !x.IsHangCha).Sum(x => x.VonGiaiNganNam);
            itemSummary.DieuChinhVonNam = dataReport.Where(x => !x.IsHangCha).Sum(x => x.DieuChinhVonNam);

            fr.AddTable<PhanBoVonDonViDieuChinhNSQPReport>("Items", dataReport);
            fr.SetValue("TongMucDauTuSum", itemSummary.TongMucDauTu);
            fr.SetValue("TongMucDauTuNSQPSum", itemSummary.TongMucDauTuNSQP);
            fr.SetValue("VonBoTriDenHetNamTruocSum", itemSummary.VonBoTriDenHetNamTruoc);
            fr.SetValue("KeHoachVonDauTuNamSum", itemSummary.KeHoachVonDauTuNam);
            fr.SetValue("VonGiaiNganNamSum", itemSummary.VonGiaiNganNam);
            fr.SetValue("DieuChinhVonNamSum", itemSummary.DieuChinhVonNam);
            fr.SetValue("DonVi", string.Empty);
            fr.SetValue("TitleFirst", paramReport == null ? null : paramReport.txt_TieuDe1);
            fr.SetValue("TitleSecond", paramReport == null ? null : paramReport.txt_TieuDe2);
            fr.SetValue("DonViTinh", paramReport == null ? null : paramReport.sDonViTinh);
            fr.UseChuKy(Username)
                     .UseChuKyForController(sControlName)
                     .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        public ExcelFile CreateReport(List<KHVNDXExportModel> dataReport, KHVNDXPrintDataExportModel paramReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachVonNam/rpt_BaoCao_KHNam_DonVi.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            KHVNDXExportModel itemSum = new KHVNDXExportModel()
            {
                TongMucDauTuDuocDuyet = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.TongMucDauTuDuocDuyet ?? 0)),
                LuyKeVonThucHienTruocNam = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.LuyKeVonThucHienTruocNam ?? 0)),
                TongSoKeHoachVon = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.TongSoKeHoachVon ?? 0)),
                KeHoachVonDuocGiao = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.KeHoachVonDuocGiao ?? 0)),
                VonKeoDaiCacNamTruoc = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.VonKeoDaiCacNamTruoc ?? 0)),
                UocThucHien = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.UocThucHien ?? 0)),
                LuyKeVonDaBoTriHetNam = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.LuyKeVonDaBoTriHetNam ?? 0)),
                TongNhuCauVonNamSau = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.TongNhuCauVonNamSau ?? 0)),
                ThuHoiVonUngTruoc = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.ThuHoiVonUngTruoc ?? 0)),
                ThanhToan = dataReport.Where(x => !x.IsHangCha).Sum(m => (m.ThanhToan ?? 0)),
            };

            VDT_KHV_KeHoachVonNam_DeXuat itemQuery = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(paramReport.iID_KeHoachVonNam_DeXuatID);

            string STenDonVi = string.Empty;
            int iNamKeHoach = 0;

            if (itemQuery != null)
            {
                iNamKeHoach = itemQuery.iNamKeHoach;
            }
            else
            {
                iNamKeHoach = paramReport.iNamKeHoach;
            }

            TempData.Keep("lstDonViQL");
            // string STenDonVi = string.Empty;
            var lstDVQL = TempData["lstDonViQL"] as IEnumerable<string>;
            string sMaDV = "";
            if (itemQuery != null)
            {
                if (paramReport != null)
                {
                    NS_DonVi itemDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, paramReport.iID_DonViQuanLyID.ToString());
                    if (itemDonVi != null)
                    {
                        STenDonVi = itemDonVi.sTen;
                    }
                }
            }
            if (lstDVQL != null)
            {
                if (lstDVQL.Count() == 1)
                {
                    sMaDV = lstDVQL.First();
                    STenDonVi = _qLVonDauTuService.GetNameDonViQLByMaDV(sMaDV).sTen;
                }
            }
            //List<KHVNDXExportModel> lstItem = ItemsReport.Where(n => n.LoaiParent.Equals(0)).ToList();

            //lstParent.Select(x => { x.STT = (lstParent.IndexOf(x) + 1).ToString(); return x; }).ToList();

            //fr.AddTable<KHVNDXExportModel>("Items", dataReport.Where(x => x.TongMucDauTuDuocDuyet > 0 && x.VonKeoDaiCacNamTruoc > 0 && x.TongSoKeHoachVon > 0 &&
            //                                        x.LuyKeVonDaBoTriHetNam > 0 && x.TongNhuCauVonNamSau > 0 && x.ThuHoiVonUngTruoc > 0 && x.ThanhToan > 0
            //                                            ));

            fr.AddTable<KHVNDXExportModel>("Items", dataReport.Where(x => x.ThanhToan > 0
                                                        ));
            FormatNumber formatNumber = new FormatNumber(1, ExportType.EXCEL);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.SetValue("fSumTongMucDauTuDuocDuyet", itemSum.TongMucDauTuDuocDuyet);
            fr.SetValue("fSumLuyKeVonNamTruoc", itemSum.LuyKeVonThucHienTruocNam);
            fr.SetValue("fSumTongKeHoachVon", itemSum.TongSoKeHoachVon);
            fr.SetValue("fSumKeHoachVonDuocGiao", itemSum.KeHoachVonDuocGiao);
            fr.SetValue("fSumVonKeoDaiCacNamTruoc", itemSum.VonKeoDaiCacNamTruoc);
            fr.SetValue("fSumUocThucHien", itemSum.UocThucHien);
            fr.SetValue("fSumLuyKeVonDaBoTriHetNamNay", itemSum.LuyKeVonDaBoTriHetNam);
            fr.SetValue("fSumTongNhuCauVonNamSau", itemSum.TongNhuCauVonNamSau);
            fr.SetValue("fSumThuHoiVonUngTruoc", itemSum.ThuHoiVonUngTruoc);
            fr.SetValue("fSumThanhToan", itemSum.ThanhToan);
            fr.SetValue("TitleFirst", paramReport.txt_TieuDe1);
            fr.SetValue("TitleSecond", paramReport.txt_TieuDe2);
            fr.SetValue("DonViCapTren", "BỘ QUỐC PHÒNG"); ;
            fr.SetValue("DonViLap", !string.IsNullOrEmpty(STenDonVi) ? STenDonVi.ToUpper() : string.Empty);
            fr.SetValue("iNamLamViec", iNamKeHoach);
            fr.SetValue("iNamTruoc", iNamKeHoach - 2);
            fr.SetValue("iNamHienTai", iNamKeHoach - 1);
            fr.SetValue("DonViTinh", paramReport.sDonViTinh);
            fr.UseChuKy(Username)
                     .UseChuKyForController(sControlName)
                     .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        private List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> HandleDataDcOtherBudget(List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> data)
        {
            try
            {
                List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> lstParent = data.Where(x => x.IsHangCha).ToList();
                lstParent.Select(x => { x.STT = (lstParent.IndexOf(x) + 1).ToString(); return x; }).ToList();
                List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> lstChildrent = data.Where(x => !x.IsHangCha).ToList();
                lstParent.Select(x =>
                {
                    List<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> item = lstChildrent.Where(y => y.IdNhomDuAn == x.IdNhomDuAn).ToList();
                    item.Select(y => { y.STT = string.Format("{0}.{1}", x.STT, item.IndexOf(y) + 1); return y; }).ToList();
                    return x;
                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                return data;
            }
        }

        private List<KHVNDXExportModel> HandleDataReportDonVi(List<KHVNDXExportModel> ItemsReport)
        {
            try
            {
                List<KHVNDXExportModel> lstItem = ItemsReport.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();

                List<KHVNDXExportModel> lstItemLevel = ItemsReport.Where(x => x.IIdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IIdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                dctItemLevel.Keys.Select(x =>
                {
                    List<KHVNDXExportModel> lstItemStt = dctItemLevel[x].ToList();
                    lstItemStt.Select(y => { y.STT = (lstItemStt.IndexOf(y) + 1).ToString(); return y; }).ToList();
                    return x;
                }).ToList();

                foreach (var item in lstItemLevel)
                {
                    List<KHVNDXExportModel> lstChildrent = ItemsReport.Where(x => x.IIdLoaiCongTrinhParent != null && x.IIdLoaiCongTrinh == item.IIdLoaiCongTrinh && x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                foreach (var item in ItemsReport.Where(x => x.IsHangCha && x.LoaiParent.Equals(2)))
                {
                    List<KHVNDXExportModel> lstChildrent = ItemsReport.Where(x => x.IIdLoaiCongTrinhParent != null && x.IIdLoaiCongTrinh == item.IIdLoaiCongTrinh && !x.IsHangCha).ToList();
                    lstChildrent.Select(x =>
                    {
                        x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString());
                        return x;
                    }).ToList();
                }

                List<KHVNDXExportModel> lstParentDonVi = ItemsReport.Where(x => x.IIdLoaiCongTrinhParent == null && x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                lstParentDonVi.Select(x => { x.STT = (lstParentDonVi.IndexOf(x) + 1).ToString(); return x; }).ToList();
                lstParentDonVi.Select(item =>
                {
                    List<KHVNDXExportModel> lstChildrent = ItemsReport.Where(x => x.IIdLoaiCongTrinhParent == null && !x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x =>
                    {
                        x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString());
                        return x;
                    }).ToList();
                    return item;
                }).ToList();

                return ItemsReport;
            }
            catch (Exception ex)
            {
                return ItemsReport;
            }
        }

        private List<KHVNDXExportModel> HandleDataReportTongHop(List<KHVNDXExportModel> ItemsReport)
        {
            try
            {
                List<KHVNDXExportModel> lstItem = ItemsReport.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();

                List<KHVNDXExportModel> lstItemLevel = ItemsReport.Where(x => x.IIdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IIdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                dctItemLevel.Keys.Select(x =>
                {
                    List<KHVNDXExportModel> lstItemStt = dctItemLevel[x].ToList();
                    lstItemStt.Select(y => { y.STT = (lstItemStt.IndexOf(y) + 1).ToString(); return y; }).ToList();
                    return x;
                }).ToList();

                foreach (var item in lstItemLevel)
                {
                    List<KHVNDXExportModel> lstChildrent = ItemsReport.Where(x => x.IIdLoaiCongTrinh == item.IIdLoaiCongTrinh && !x.IsHangCha).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                List<KHVNDXExportModel> lstLctParent = ItemsReport.Where(x => x.LoaiParent.Equals(0)).ToList();
                var dctItemParent = ItemsReport.Where(x => !x.IsHangCha && x.IIdLoaiCongTrinh.HasValue
                    && lstLctParent.Select(y => y.IIdLoaiCongTrinh).Contains(x.IIdLoaiCongTrinh)).GroupBy(z => z.IIdLoaiCongTrinh).ToDictionary(z => z.Key.ToString(), z => z.ToList());
                foreach (var item in dctItemParent.Keys)
                {
                    List<KHVNDXExportModel> itemStt = dctItemParent[item];
                    itemStt.Select(x => { x.STT = string.Format("{0}", (itemStt.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                return ItemsReport;
            }
            catch (Exception ex)
            {
                return ItemsReport;
            }
        }

        private List<KHVNDXExportModel> CalculateDataGocReport(List<KHVNDXExportModel> ItemsReport)
        {
            try
            {
                List<KHVNDXExportModel> childrent = ItemsReport.Where(x => !x.IsHangCha).ToList();
                List<KHVNDXExportModel> parent = ItemsReport.Where(x => x.IsHangCha && (x.LoaiParent.Equals(0) || x.LoaiParent.Equals(1))).ToList();

                ItemsReport.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.TongMucDauTuDuocDuyet = 0;
                    x.LuyKeVonThucHienTruocNam = 0;
                    x.TongSoKeHoachVon = 0;
                    x.KeHoachVonDuocGiao = 0;
                    x.VonKeoDaiCacNamTruoc = 0;
                    x.UocThucHien = 0;
                    x.LuyKeVonDaBoTriHetNam = 0;
                    x.TongNhuCauVonNamSau = 0;
                    x.ThuHoiVonUngTruoc = 0;
                    x.ThanhToan = 0;
                    return x;
                }).ToList();

                foreach (var pr in parent)
                {
                    List<KHVNDXExportModel> lstChilrent = childrent.Where(x => x.IIdLoaiCongTrinh.Equals(pr.IIdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.TongMucDauTuDuocDuyet != 0 || x.LuyKeVonThucHienTruocNam != 0 ||
                        x.TongSoKeHoachVon != 0 || x.KeHoachVonDuocGiao != 0 || x.VonKeoDaiCacNamTruoc != 0 || x.ThanhToan != 0 ||
                        x.UocThucHien != 0 || x.LuyKeVonDaBoTriHetNam != 0 || x.TongNhuCauVonNamSau != 0 || x.ThuHoiVonUngTruoc != 0)))
                    {
                        pr.TongMucDauTuDuocDuyet += item.TongMucDauTuDuocDuyet ?? 0;
                        pr.LuyKeVonThucHienTruocNam += item.LuyKeVonThucHienTruocNam ?? 0;
                        pr.TongSoKeHoachVon += item.TongSoKeHoachVon ?? 0;
                        pr.KeHoachVonDuocGiao += item.KeHoachVonDuocGiao ?? 0;
                        pr.VonKeoDaiCacNamTruoc += item.VonKeoDaiCacNamTruoc ?? 0;
                        pr.UocThucHien += item.UocThucHien ?? 0;
                        pr.LuyKeVonDaBoTriHetNam += item.LuyKeVonDaBoTriHetNam ?? 0;
                        pr.TongNhuCauVonNamSau += item.TongNhuCauVonNamSau ?? 0;
                        pr.ThuHoiVonUngTruoc += item.ThuHoiVonUngTruoc ?? 0;
                        pr.ThanhToan += item.ThanhToan ?? 0;
                    }
                }

                foreach (var item in parent.Where(x => x.IIdLoaiCongTrinh != null && (x.TongMucDauTuDuocDuyet != 0 || x.LuyKeVonThucHienTruocNam != 0 ||
                        x.TongSoKeHoachVon != 0 || x.KeHoachVonDuocGiao != 0 || x.VonKeoDaiCacNamTruoc != 0 || x.ThanhToan != 0 ||
                        x.UocThucHien != 0 || x.LuyKeVonDaBoTriHetNam != 0 || x.TongNhuCauVonNamSau != 0 || x.ThuHoiVonUngTruoc != 0)))
                {
                    CalculateParent(item, item, ItemsReport);
                }

                ItemsReport = ItemsReport.Where(x => (x.TongMucDauTuDuocDuyet != 0 || x.LuyKeVonThucHienTruocNam != 0 ||
                            x.TongSoKeHoachVon != 0 || x.KeHoachVonDuocGiao != 0 || x.VonKeoDaiCacNamTruoc != 0 || x.ThanhToan != 0 ||
                            x.UocThucHien != 0 || x.LuyKeVonDaBoTriHetNam != 0 || x.TongNhuCauVonNamSau != 0 || x.ThuHoiVonUngTruoc != 0)).ToList();

                return ItemsReport;
            }
            catch (Exception ex)
            {
                return ItemsReport;
            }
        }

        private void CalculateParent(KHVNDXExportModel currentItem, KHVNDXExportModel seftItem, List<KHVNDXExportModel> ItemsReport)
        {
            try
            {
                var parrentItem = ItemsReport.Where(x => x.IIdLoaiCongTrinh == currentItem.IIdLoaiCongTrinhParent).FirstOrDefault();
                if (parrentItem == null) return;
                parrentItem.TongMucDauTuDuocDuyet += seftItem.TongMucDauTuDuocDuyet ?? 0;
                parrentItem.LuyKeVonThucHienTruocNam += seftItem.LuyKeVonThucHienTruocNam ?? 0;
                parrentItem.TongSoKeHoachVon += seftItem.TongSoKeHoachVon ?? 0;
                parrentItem.KeHoachVonDuocGiao += seftItem.KeHoachVonDuocGiao ?? 0;
                parrentItem.VonKeoDaiCacNamTruoc += seftItem.VonKeoDaiCacNamTruoc ?? 0;
                parrentItem.UocThucHien += seftItem.UocThucHien ?? 0;
                parrentItem.LuyKeVonDaBoTriHetNam += seftItem.LuyKeVonDaBoTriHetNam ?? 0;
                parrentItem.TongNhuCauVonNamSau += seftItem.TongNhuCauVonNamSau ?? 0;
                parrentItem.ThuHoiVonUngTruoc += seftItem.ThuHoiVonUngTruoc ?? 0;
                parrentItem.ThanhToan += seftItem.ThanhToan ?? 0;
                CalculateParent(parrentItem, seftItem, ItemsReport);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private List<DonViTinhModel> LoadDonViTinh()
        {
            List<DonViTinhModel> lstDonViTinh = new List<DonViTinhModel>()
            {
                new DonViTinhModel(){DisplayItem = TRIEU_DONG, ValueItem = TRIEU_VALUE},
                new DonViTinhModel(){DisplayItem = DONG, ValueItem = DONG_VALUE},
                new DonViTinhModel(){DisplayItem = NGHIN_DONG, ValueItem = NGHIN_DONG_VALUE},
                new DonViTinhModel(){DisplayItem = TY_DONG, ValueItem = TY_VALUE}
            };
            return lstDonViTinh;
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
        #region XuatExcel
        public ExcelFile CreateReportNew(List<KHVNDXExportExcelModel> dataReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachVonNam/rpt_BaoCao_KHNam_DonVi_Moi.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            KHVNDXExportModel itemSum = new KHVNDXExportModel()
            {
                TongMucDauTuDuocDuyet = dataReport.Sum(m => (m.TongMucDauTuDuocDuyet ?? 0)),
                LuyKeVonThucHienTruocNam = dataReport.Sum(m => (m.LuyKeVonThucHienTruocNam ?? 0)),
                TongSoKeHoachVon = dataReport.Sum(m => (m.TongSoKeHoachVon ?? 0)),
                KeHoachVonDuocGiao = dataReport.Sum(m => (m.KeHoachVonDuocGiao ?? 0)),
                VonKeoDaiCacNamTruoc = dataReport.Sum(m => (m.VonKeoDaiCacNamTruoc ?? 0)),
                UocThucHien = dataReport.Sum(m => (m.UocThucHien ?? 0)),
                LuyKeVonDaBoTriHetNam = dataReport.Sum(m => (m.LuyKeVonDaBoTriHetNam ?? 0)),
                TongNhuCauVonNamSau = dataReport.Sum(m => (m.TongNhuCauVonNamSau ?? 0)),
                ThuHoiVonUngTruoc = dataReport.Sum(m => (m.ThuHoiVonUngTruoc ?? 0)),
                ThanhToan = dataReport.Sum(m => (m.ThanhToan ?? 0)),
            };
            string STenDonVi = string.Empty;
            int iNamKeHoach = 0;
            if (dataReport != null && dataReport.Count > 0)
            {
                iNamKeHoach = Convert.ToInt32(dataReport.FirstOrDefault().INamKeHoach);
            }
            else
            {
                iNamKeHoach = DateTime.Now.Year;
            }
            FormatNumber formatNumber = new FormatNumber(1, ExportType.EXCEL);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.AddTable("Items", dataReport);
            fr.SetValue("fSumTongMucDauTuDuocDuyet", itemSum.TongMucDauTuDuocDuyet);
            fr.SetValue("fSumLuyKeVonNamTruoc", itemSum.LuyKeVonThucHienTruocNam);
            fr.SetValue("fSumTongKeHoachVon", itemSum.TongSoKeHoachVon);
            fr.SetValue("fSumKeHoachVonDuocGiao", itemSum.KeHoachVonDuocGiao);
            fr.SetValue("fSumVonKeoDaiCacNamTruoc", itemSum.VonKeoDaiCacNamTruoc);
            fr.SetValue("fSumUocThucHien", itemSum.UocThucHien);
            fr.SetValue("fSumLuyKeVonDaBoTriHetNamNay", itemSum.LuyKeVonDaBoTriHetNam);
            fr.SetValue("fSumTongNhuCauVonNamSau", itemSum.TongNhuCauVonNamSau);
            fr.SetValue("fSumThuHoiVonUngTruoc", itemSum.ThuHoiVonUngTruoc);
            fr.SetValue("fSumThanhToan", itemSum.ThanhToan);
            fr.SetValue("iNamHienTai", iNamKeHoach);
            fr.SetValue("iNamTruoc", iNamKeHoach - 1);
            fr.SetValue("DonViTinh", "Đồng");
            fr.Run(Result);
            return Result;
        }

        [HttpPost]
        public JsonResult KeHoachVonNamDeXuatExport(Guid iID_KeHoachVonNamDeXuatID)
        {
            try
            {
                List<KHVNDXExportExcelModel> dataReport = new List<KHVNDXExportExcelModel>();
                dataReport = _qLVonDauTuService.GetKeHoachVonNamExcel(iID_KeHoachVonNamDeXuatID.ToString()).ToList();
                ExcelFile xls = CreateReportNew(dataReport);
                xls.PrintLandscape = true;
                TempData["DataReportExcel"] = xls;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public FileContentResult KehoachVonNamDeXuatExportExcel()
        {
            ExcelFile xls = (ExcelFile)TempData["DataReportExcel"];
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"KeHoachVonNamDeXuat.xlsx");
            }
        }
        #endregion
        #endregion
        #region Import Excel FTP
        [HttpPost]
        public ActionResult GetGridListExcelFromFTP(string idDonVi, string nam)
        {
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
                pathString = pathString + "/" + tenDonVi + "/VDT/KeHoachVonNamDeXuat/send/" + nam.Trim() + "/";
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
            return PartialView("_modalImportFile", data);
        }
        [HttpPost]
        public JsonResult ImportDataExcel(string url)
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
                        using (MemoryStream ms = new MemoryStream())
                        {
                            responseStream.CopyTo(ms);
                            var file_data = ms.ToArray();
                            var dt = ExcelHelpers.LoadExcelDataTable(file_data);
                            IEnumerable<KeHoachVonNamDeXuatDataImportModel> dataImport = excel_result(dt);
                            TempData["dataImport"] = dataImport;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet); ;
        }

        #endregion

        #region ValidatePreImport
        [HttpPost]
        public JsonResult ValidatePreImport(VDT_KHV_KeHoachVonNam_DeXuat_ViewModel data)
        {
            string message = "";
            IEnumerable<KeHoachVonNamDeXuatDataImportModel> dataImport = (IEnumerable<KeHoachVonNamDeXuatDataImportModel>)TempData["dataImport"];

            try
            {
                if (data == null)
                {
                    message = "Vui lòng nhập dữ liệu kế hoạch vốn năm đề xuất !";
                    return Json(new { bIsComplete = false, sMessError = message }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "")
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch không được để trống !" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (_qLVonDauTuService.CheckExistSoKeHoachVonNamDeXuat(data.sSoQuyetDinh, data.iNamKeHoach, data.iID_KeHoachVonNamDeXuatID))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    string listsMaDuan = string.Join(",", dataImport.Select(x => x.sMaDuAn));
                    var listDuAns = _qLVonDauTuService.GetDuAnBysMaDuAn(listsMaDuan);
                    TempData["listDuAns"] = listDuAns;

                    if (!listDuAns.Any() || dataImport.Count() != listDuAns.Count())
                    {
                        return Json(new { bIsComplete = false, sMessError = "Dự án chưa hợp lệ, vui lòng kiểm tra lại dữ liệu !" }, JsonRequestBehavior.AllowGet);

                    }

                }
            }catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { bIsComplete = false, sMessError = ex.Message }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

        #region Save Import
        [HttpPost]
        public ActionResult SaveImport(VDT_KHV_KeHoachVonNam_DeXuat_ViewModel data)
        {
            string sMessage = "Lỗi lưu kế hoạch vốn năm";

            try
            {

                if (data == null)
                {
                    sMessage = "Vui lòng kiểm tra lại dữ liệu !";
                    return Json(new { bIsComplete = false, sMessage = sMessage }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    data.sUserCreate = Username;
                    VDT_KHV_KeHoachVonNam_DeXuat resuilt = _qLVonDauTuService.SaveImportKeHoachVonNamDeXuat(data);
                    if (resuilt == null)
                    {
                        sMessage = "Lưu kế hoạch vốn năm đề xuất thất bại !";
                        return Json(new { bIsComplete = false, sMessage = sMessage }, JsonRequestBehavior.AllowGet); ;

                    }
                    else
                    {
                        TempData["KhvnDeXuat"] = resuilt;
                        return Json(new { bIsComplete = true, Id = resuilt.iID_KeHoachVonNamDeXuatID  }, JsonRequestBehavior.AllowGet); ;

                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { bIsComplete = false, sMessage = sMessage }, JsonRequestBehavior.AllowGet); ;

            }

        }


        public JsonResult SaveChiTietimport(List<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet> listDetails, Guid iID_KeHoachVonNamDeXuatID)
        {
            string sMessage = "Lỗi lưu kế hoạch vốn năm chi tiết !";
            VDT_KHV_KeHoachVonNam_DeXuat modelKhvn = (VDT_KHV_KeHoachVonNam_DeXuat)TempData["KhvnDeXuat"];
            if(modelKhvn != null)
            {
                iID_KeHoachVonNamDeXuatID = modelKhvn.iID_KeHoachVonNamDeXuatID;
            }
            if (_qLVonDauTuService.SaveImportKeHoachVonNamDeXuatChiTiet(listDetails, iID_KeHoachVonNamDeXuatID))
            {
                return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet); ;

            }

            return Json(new { bIsComplete = false, sMessage = sMessage }, JsonRequestBehavior.AllowGet); ;

        }

        #endregion
    }
}