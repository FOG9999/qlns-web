using Dapper;
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
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.DanhMuc.Models;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.z.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;
using VIETTEL.Mvc;
using static VIETTEL.Common.Constants;
using static VTS.QLNS.CTC.App.Service.UserFunction.FormatNumber;
using VTS.QLNS.CTC.App.Service.UserFunction;
using Spire.Xls.Core.Spreadsheet;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Ajax.Utilities;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class KeHoachTrungHanDeXuatController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        IDanhMucService _danhMucService = DanhMucService.Default;

        private readonly IDanhMucService _dmService = DanhMucService.Default;
        private static List<KeHoach5NamDeXuatModel> _lstTongHop = new List<KeHoach5NamDeXuatModel>();
        private static List<DuAnKeHoach5Nam> _lstDuAnChecked = new List<DuAnKeHoach5Nam>();

        public const string NGHIN_DONG = "Nghìn đồng";
        public const string DONG = "Đồng";
        public const string NGHIN_DONG_VALUE = "1000";
        public const string DONG_VALUE = "1";
        public const string TRIEU_DONG = "Triệu đồng";
        public const string TRIEU_VALUE = "1000000";
        public const string TY_DONG = "Tỷ đồng";
        public const string TY_VALUE = "1000000000";
        public string pathString = System.Configuration.ConfigurationManager.AppSettings["FtpPath"];
        public string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FtpUsername"];
        public string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        private const string sControlName = "KeHoachTrungHanDeXuat";

        // GET: QLVonDauTu/KeHoachTrungHanDeXuat
        /*   Sửa load chứng từ theo loai tổng hợp và chưa được tổng hợp
        public ActionResult Index()
        {
            KeHoach5NamDeXuatViewModel vm = new KeHoach5NamDeXuatViewModel();

            try
            {
                vm._paging.CurrentPage = 1;
                _lstTongHop = new List<KeHoach5NamDeXuatModel>();

                List<KeHoach5NamDeXuatModel> lstQuery = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, "", null, null, null, "", null, null, 0).ToList();

                List<KeHoach5NamDeXuatModel> lstVoucherAgregateParent = lstQuery.Where(x => !string.IsNullOrEmpty(x.sTongHop)).ToList();
                Dictionary<string, List<string>> dctVoucher = lstVoucherAgregateParent.GroupBy(x => x.iID_KeHoach5Nam_DeXuatID).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

                List<KeHoach5NamDeXuatModel> lstVoucherAgregate = new List<KeHoach5NamDeXuatModel>();

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = "--Tất cả--" });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");

                List<KeHoach5NamDeXuatModel> lstVoucherTypes = new List<KeHoach5NamDeXuatModel>()
                {
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "--Tất cả--", iLoai = 0},
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "Khởi công mới", iLoai = 1},
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "Chuyển tiếp", iLoai = 2}
                };
                ViewBag.ListVoucherTypes = lstVoucherTypes.ToSelectList("iLoai", "SVoucherTypes");

                foreach (var key in dctVoucher.Keys)
                {
                    string lstValue = dctVoucher[key].ToList().FirstOrDefault();
                    List<string> lstChildrentId = new List<string>();
                    if (lstValue.Contains(","))
                    {
                        lstChildrentId = lstValue.Split(',').ToList();
                    }
                    else
                    {
                        lstChildrentId.Add(lstValue);
                    }

                    KeHoach5NamDeXuatModel itemParent = lstQuery.Where(x => x.iID_KeHoach5Nam_DeXuatID == Guid.Parse(key)).FirstOrDefault();
                    List<KeHoach5NamDeXuatModel> lstChildrent = lstQuery.Where(x => lstChildrentId.Any(y => Guid.Parse(y) == x.iID_KeHoach5Nam_DeXuatID)).ToList();

                    lstVoucherAgregate.Add(itemParent);
                    lstVoucherAgregate.AddRange(lstChildrent);
                }

                List<KeHoach5NamDeXuatModel> lstVoucher = lstQuery.Where(x => !lstVoucherAgregate.Any(y => y.iID_KeHoach5Nam_DeXuatID == x.iID_KeHoach5Nam_DeXuatID)).ToList();

                vm.Items = lstVoucher;
                vm._paging.TotalItems = vm.Items.Count();

                ViewBag.VoucherTabIndex = "checked";
                ViewBag.VoucherAggregateTabIndex = "";
                ViewBag.TabIndex = 1;


            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            return View(vm);
        }

        */
        public ActionResult Index() // Hàm sau khi sửa 
        {
            KeHoach5NamDeXuatViewModel vm = new KeHoach5NamDeXuatViewModel();

            try
            {
                vm._paging.CurrentPage = 1;
                _lstTongHop = new List<KeHoach5NamDeXuatModel>();

                //List<KeHoach5NamDeXuatModel> lstQuery = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, "", null, null, null, "", null, null, 0,null).ToList();

                //List<KeHoach5NamDeXuatModel> lstVoucherAgregateParent = lstQuery.Where(x => !string.IsNullOrEmpty(x.sTongHop)).ToList();
                //Dictionary<string, List<string>> dctVoucher = lstVoucherAgregateParent.GroupBy(x => x.iID_KeHoach5Nam_DeXuatID).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

                //List<KeHoach5NamDeXuatModel> lstVoucherAgregate = new List<KeHoach5NamDeXuatModel>();


                //foreach (var key in dctVoucher.Keys)
                //{
                //    string lstValue = dctVoucher[key].ToList().FirstOrDefault();
                //    List<string> lstChildrentId = new List<string>();
                //    if (lstValue.Contains(","))
                //    {
                //        lstChildrentId = lstValue.Split(',').ToList();
                //    }
                //    else
                //    {
                //        lstChildrentId.Add(lstValue);
                //    }

                //    KeHoach5NamDeXuatModel itemParent = lstQuery.Where(x => x.iID_KeHoach5Nam_DeXuatID == Guid.Parse(key)).FirstOrDefault();
                //    List<KeHoach5NamDeXuatModel> lstChildrent = lstQuery.Where(x => lstChildrentId.Any(y => Guid.Parse(y) == x.iID_KeHoach5Nam_DeXuatID)).ToList();

                //    lstVoucherAgregate.Add(itemParent);
                //    lstVoucherAgregate.AddRange(lstChildrent);
                //}

                List<KeHoach5NamDeXuatModel> lstVoucher = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, "", null, null, null, "", null, null, 0, 1).ToList();

                vm.Items = lstVoucher;
                //vm._paging.TotalItems = vm.Items.Count();

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả--" });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

                List<KeHoach5NamDeXuatModel> lstVoucherTypes = new List<KeHoach5NamDeXuatModel>()
                {
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "--Tất cả--", iLoai = 0},
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "Khởi công mới", iLoai = 1},
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "Chuyển tiếp", iLoai = 2}
                };
                ViewBag.ListVoucherTypes = lstVoucherTypes.ToSelectList("iLoai", "SVoucherTypes");

                ViewBag.VoucherTabIndex = "checked";
                ViewBag.VoucherAggregateTabIndex = "";
                ViewBag.TabIndex = 1;


            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult KeHoach5NamDeXuatListView(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? iID_DonViQuanLyID, string sMoTaChiTiet, int? iGiaiDoanTu, int? iGiaiDoanDen, int? iLoai, int? tabIndex)
        {
            KeHoach5NamDeXuatViewModel vm = new KeHoach5NamDeXuatViewModel();

            try
            {
                vm._paging = _paging;
                ViewBag.TabIndex = tabIndex;
                //List<KeHoach5NamDeXuatModel> lstQuery = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, null).ToList();

                //List<KeHoach5NamDeXuatModel> lstVoucherAgregateParent = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, "", null, null, null, "", null, null, 0, 1).ToList();

                //Dictionary<string, List<string>> dctVoucher = lstVoucherAgregateParent.GroupBy(x => x.iID_KeHoach5Nam_DeXuatID).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

                //List<KeHoach5NamDeXuatModel> lstVoucherAgregate = new List<KeHoach5NamDeXuatModel>();

                //foreach (var key in dctVoucher.Keys)
                //{
                //    string lstValue = dctVoucher[key].ToList().FirstOrDefault();
                //    List<string> lstChildrentId = new List<string>();
                //    if (lstValue.Contains(","))
                //    {
                //        lstChildrentId = lstValue.Split(',').ToList();
                //    }
                //    else
                //    {
                //        lstChildrentId.Add(lstValue);
                //    }

                //    KeHoach5NamDeXuatModel itemParent = lstQuery.Where(x => x.iID_KeHoach5Nam_DeXuatID == Guid.Parse(key)).FirstOrDefault();
                //    List<KeHoach5NamDeXuatModel> lstChildrent = lstQuery.Where(x => lstChildrentId.Any(y => Guid.Parse(y) == x.iID_KeHoach5Nam_DeXuatID)).ToList();

                //    lstVoucherAgregate.Add(itemParent);
                //    lstVoucherAgregate.AddRange(lstChildrent);
                //}

                //List<KeHoach5NamDeXuatModel> lstVoucher = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai).ToList();

                List<KeHoach5NamDeXuatModel> lstVoucherAgregate = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, tabIndex).ToList();

                List<KeHoach5NamDeXuatModel> lstVoucher = _iQLVonDauTuService.GetAllKeHoach5NamDeXuat(ref vm._paging, PhienLamViec.NamLamViec, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, tabIndex).ToList();

                if (tabIndex.Equals(1))
                {
                    ViewBag.VoucherTabIndex = "checked";
                    ViewBag.VoucherAggregateTabIndex = "";
                    vm.Items = lstVoucher;
                }
                else
                {
                    ViewBag.VoucherAggregateTabIndex = "checked";
                    ViewBag.VoucherTabIndex = "";
                    vm.Items = lstVoucherAgregate;
                }
                //vm._paging.TotalItems = vm.Items.Count();

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả--" });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

                List<KeHoach5NamDeXuatModel> lstVoucherTypes = new List<KeHoach5NamDeXuatModel>()
                {
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "--Tất cả--", iLoai = 0},
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "Khởi công mới", iLoai = 1},
                    new KeHoach5NamDeXuatModel(){SVoucherTypes = "Chuyển tiếp", iLoai = 2}
                };
                ViewBag.ListVoucherTypes = lstVoucherTypes.ToSelectList("iLoai", "SVoucherTypes");
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            KeHoach5NamDeXuatModel data = new KeHoach5NamDeXuatModel();
            try
            {
                data = _iQLVonDauTuService.GetKeHoach5NamDeXuatByIdForDetail(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            data.sTenDonvVi = $"{data.iID_MaDonViQuanLy} - {data.sTenDonvVi}";
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public ActionResult GetModalCt(string idDonVi, Guid? id)
        {
            DuAnKeHoach5NamModel data = new DuAnKeHoach5NamModel();
            try
            {
                if (!string.IsNullOrEmpty(idDonVi))
                {
                    var itemDvQuery = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, idDonVi);
                    List<DuAnKeHoach5Nam> lstDuAn = GetDuAn(itemDvQuery != null ? itemDvQuery.iID_MaDonVi : string.Empty);

                    List<DuAnKeHoach5Nam> duAnExisted = new List<DuAnKeHoach5Nam>();
                    if (id.HasValue)
                    {
                        DataTable dt = _iQLVonDauTuService.GetListKH5NamDeXuatChiTietById(id.ToString(), PhienLamViec.NamLamViec, new Dictionary<string, string>());
                        if (dt == null) dt = new DataTable();
                        duAnExisted = dt.AsEnumerable().Select(row => new DuAnKeHoach5Nam()
                        {
                            IDDuAnID = Guid.Parse(row["iID_DuAnID"].ToString())
                        }).ToList();

                        List<DuAnKeHoach5Nam> lstChecked = _iQLVonDauTuService.GetIDDuAnKHTHDeXuatChiTietByIDDeXuat(id.HasValue ? id.Value : Guid.Empty).ToList();
                        lstDuAn.Select(item =>
                        {
                            if (lstChecked.Where(x => x.IDDuAnID == item.IDDuAnID).Count() > 0)
                            {
                                item.IsChecked = true;
                            }
                            else
                            {
                                item.IsChecked = false;
                            }
                            return item;
                        }).ToList();

                        //lstDuAn.Select(item =>
                        //{
                        //    if (duAnExisted.Where(x => x.IDDuAnID == item.IDDuAnID).Count() > 0)
                        //    {
                        //        item.IsChecked = true;
                        //    }
                        //    else
                        //    {
                        //        item.IsChecked = false;
                        //    }
                        //    return item;
                        //}).ToList();
                    }

                    data.Items = lstDuAn;
                }
                else
                {
                    data.Items = new List<DuAnKeHoach5Nam>();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_modalDialogCt", data);
        }

        [HttpPost]
        public ActionResult GetModalMm(string idDonVi, Guid? id)
        {
            DuAnKeHoach5NamModel data = new DuAnKeHoach5NamModel();
            try
            {
                if (!string.IsNullOrEmpty(idDonVi))
                {                    
                    List<DuAnKeHoach5Nam> lstDuAn = GetDuAnDaLapCTDT(idDonVi);

                    List<DuAnKeHoach5Nam> duAnExisted = new List<DuAnKeHoach5Nam>();
                    var itemDexuat = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(id);
                    if(itemDexuat != null)
                    {
                        DataTable dt = _iQLVonDauTuService.GetListKH5NamDeXuatChiTietById(id.ToString(), PhienLamViec.NamLamViec, new Dictionary<string, string>());
                        if (dt == null) dt = new DataTable();
                        duAnExisted = dt.AsEnumerable().Select(row => new DuAnKeHoach5Nam()
                        {
                            IDDuAnID = Guid.Parse(row["iID_DuAnID"].ToString())
                        }).ToList();
                        List<DuAnKeHoach5Nam> lstChecked = _iQLVonDauTuService.GetIDDuAnKHTHDeXuatChiTietByIDDeXuat(id.HasValue ? id.Value : Guid.Empty).ToList();
                        lstDuAn.Select(item =>
                        {
                            if (lstChecked.Where(x => x.IDDuAnID == item.IDDuAnID).Count() > 0)
                            {
                                item.IsChecked = true;
                            }
                            else
                            {
                                item.IsChecked = false;
                            }
                            return item;
                        }).ToList();
                    }                   

                    data.Items = lstDuAn;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_modalDialogMm", data);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id, bool isAggregate, List<KeHoach5NamDeXuatModel> lstItem)
        {
            VDT_KHV_KeHoach5Nam_DeXuat data = new VDT_KHV_KeHoach5Nam_DeXuat();
            try
            {
                _lstTongHop = new List<KeHoach5NamDeXuatModel>();

                if (id.HasValue)
                {
                    data = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(id);
                }
                else if (!id.HasValue && !isAggregate)
                {
                    //data.iGiaiDoanTu = DateTime.Now.Year;
                    //data.iGiaiDoanDen = DateTime.Now.Year + 4;
                    data.iGiaiDoanTu = _iQLVonDauTuService.FindCurrentPeriod(PhienLamViec.NamLamViec);
                    data.iGiaiDoanDen = data.iGiaiDoanTu + 4;
                }

                if (isAggregate)
                {
                    ViewBag.IsAggregate = true;

                    if (lstItem != null && lstItem.Count() > 0 && lstItem.FirstOrDefault() != null)
                    {
                        var lstValue = lstItem.GroupBy(x => x.iID_KeHoach5Nam_DeXuatID).Select(grp => grp.LastOrDefault()).Where(x => x.isChecked).ToList();

                        foreach (var item in lstValue)
                        {
                            var itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatByIdForDetail(item.iID_KeHoach5Nam_DeXuatID);
                            if (itemQuery != null)
                            {
                                _lstTongHop.Add(itemQuery);
                            }
                        }

                        _lstTongHop = _lstTongHop.GroupBy(item => item.iID_KeHoach5Nam_DeXuatID).Select(grp => grp.FirstOrDefault()).ToList();

                        if (_lstTongHop.Count() == 0)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ để tổng hợp!" }, JsonRequestBehavior.AllowGet);
                        }

                        if (_lstTongHop.GroupBy(x => new { x.iGiaiDoanTu, x.iGiaiDoanDen }).Count() > 1)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có cùng giai đoạn !" }, JsonRequestBehavior.AllowGet);
                        }

                        if (_lstTongHop.Any(x => x.iKhoa.HasValue && !x.iKhoa.Value))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn các chứng từ đã khóa !" }, JsonRequestBehavior.AllowGet);
                        }

                        if (_lstTongHop.Any(x => !string.IsNullOrEmpty(x.sTongHop)))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Chứng từ đã được tổng hợp !" }, JsonRequestBehavior.AllowGet);
                        }



                        ViewBag.ListVoucherTypes = CreateVoucherTypes().Where((n => n.iLoai == _lstTongHop.FirstOrDefault().iLoai)).ToSelectList("iLoai", "SVoucherTypes");
                        ViewBag.LstTongHop = _lstTongHop;
                        data.iGiaiDoanTu = _lstTongHop.FirstOrDefault().iGiaiDoanTu;
                        data.iGiaiDoanDen = _lstTongHop.FirstOrDefault().iGiaiDoanDen;

                    }
                    else
                    {
                        ViewBag.LstTongHop = new List<KeHoach5NamDeXuatModel>();

                        if (_lstTongHop.Count() == 0)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ để tổng hợp!" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    //TODO Confirm
                    //List<NS_DonVi> lstDonViTongHop = new List<NS_DonVi>();
                    //NS_DonVi itemDonVi = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).TO;
                    //lstDonViTongHop.Add(itemDonVi);
                    //ViewBag.ListDonViQuanLy = lstDonViTongHop.ToSelectList("iID_Ma", "sTen");
                    ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");

                }
                else
                {
                    ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                    ViewBag.IsAggregate = false;
                    ViewBag.ListVoucherTypes = CreateVoucherTypes().ToSelectList("iLoai", "SVoucherTypes");
                }


            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public JsonResult KeHoach5NamDeXuatSave(VDT_KHV_KeHoach5Nam_DeXuat data, bool isModified, bool isAggregate, List<DuAnKeHoach5Nam> lstDuAnChecked)
        {
            try
            {
                List<VdtKhvKeHoachTrungHanDeXuatChiTietModel> details = new List<VdtKhvKeHoachTrungHanDeXuatChiTietModel>();
                _lstDuAnChecked = new List<DuAnKeHoach5Nam>();

                if (lstDuAnChecked != null && lstDuAnChecked.Count() > 0 && lstDuAnChecked.FirstOrDefault() != null)
                {
                    _lstDuAnChecked = lstDuAnChecked.GroupBy(item => item.IDDuAnID).Select(grp => grp.LastOrDefault()).Where(item => item.IsChecked).ToList();
                }
                if (isAggregate && _lstTongHop != null && _lstTongHop.Count() > 0)
                {
                    foreach (var itemId in _lstTongHop.Select(x => x.iID_KeHoach5Nam_DeXuatID).ToList())
                    {
                        var lstDuAnCheckedChild = _iQLVonDauTuService.GetIDDuAnKHTHDeXuatChiTietByIDDeXuat(itemId).ToList();
                        _lstDuAnChecked.AddRange(lstDuAnCheckedChild);
                    }
                }
                TempData["lstDuAnChecked"] = _lstDuAnChecked;

                if (data.iID_KeHoach5Nam_DeXuatID == new Guid())
                {
                    if (!isAggregate)
                    {
                        if (_iQLVonDauTuService.CheckExistSoKeHoach(data.sSoQuyetDinh, PhienLamViec.NamLamViec, null))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                        }

                        //if (_iQLVonDauTuService.CheckExistGiaiDoanKeHoach(data.iGiaiDoanTu, data.iGiaiDoanDen, PhienLamViec.NamLamViec, data.iID_DonViQuanLyID, null))
                        //{
                        //    return Json(new { bIsComplete = false, sMessError = "Đơn vị quản lý trong giai đoạn này đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                        //}

                        if (_iQLVonDauTuService.CheckExistDonVi_LoaiDuAn_GiaiDoanKeHoach(data.iGiaiDoanTu, data.iGiaiDoanDen, PhienLamViec.NamLamViec, data.iID_DonViQuanLyID, data.iLoai, null))
                        {
                            var objDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec.ToString(), data.iID_DonViQuanLyID.ToString());
                            var strDonVi = string.Format("{0} - {1}", objDonVi.iID_MaDonVi, objDonVi.sTen);
                            var tenLoaiDuAn = CreateVoucherTypes().FirstOrDefault(n => n.iLoai == data.iLoai).sLoaiDuAn;
                            /*return Json(new
                            {
                                bIsComplete = false,
                                sMessError = string.Format("Đơn vị {0}, giai đoạn từ năm {1} đến năm {2} loại dự án {3} đã được lập.", strDonVi, data.iGiaiDoanTu, data.iGiaiDoanDen, tenLoaiDuAn)
                            },
                            JsonRequestBehavior.AllowGet);*/
                            return Json(new
                            {
                                bIsComplete = true,
                                bIsTrung = true,
                                sMessWarning = string.Format("{0} đã lập KHTH đề xuất giai đoạn từ năm {1} đến năm {2}, bạn chắc chắn muốn lập bản ghi này không?", strDonVi, data.iGiaiDoanTu, data.iGiaiDoanDen, tenLoaiDuAn),
                            }, JsonRequestBehavior.AllowGet);
                        }

                        if (_iQLVonDauTuService.CheckPeriodValid(data.iGiaiDoanTu, PhienLamViec.NamLamViec, data.iID_DonViQuanLyID))
                        {
                            var objDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec.ToString(), data.iID_DonViQuanLyID.ToString());
                            var strDonVi = string.Format("{0} - {1}", objDonVi.iID_MaDonVi, objDonVi.sTen);
                            var tenLoaiDuAn = CreateVoucherTypes().FirstOrDefault(n => n.iLoai == data.iLoai).sLoaiDuAn;
                            var sGiaiDoanTuTrung = _iQLVonDauTuService.GetGiaiDoanTuDenDaTrung(data.iGiaiDoanTu, PhienLamViec.NamLamViec, data.iID_DonViQuanLyID).iGiaiDoanTu.ToString();
                            var sGiaiDoanDenTrung = _iQLVonDauTuService.GetGiaiDoanTuDenDaTrung(data.iGiaiDoanTu, PhienLamViec.NamLamViec, data.iID_DonViQuanLyID).iGiaiDoanDen.ToString();
                            //return Json(new { bIsComplete = false, sMessError = "Giai đoạn không hợp lệ !" }, JsonRequestBehavior.AllowGet);
                            return Json(new
                            {
                                bIsComplete = true,
                                bIsTrung = true,
                                sMessWarning = string.Format("{0} đã lập KHTH đề xuất giai đoạn {1}-{2} chứa giai đoạn {3}-{4} (giai đoạn trùng năm KHTH đã lập)  đã được lập, bạn chắc chắn muốn lập bản ghi này không?", strDonVi, data.iGiaiDoanTu, data.iGiaiDoanDen, sGiaiDoanTuTrung, sGiaiDoanDenTrung),
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    int intIndex = 0;
                    foreach (var item in _lstTongHop.Select(x => x.iID_KeHoach5Nam_DeXuatID).ToList())
                    {
                        intIndex += 1;
                        List<VdtKhvKeHoachTrungHanDeXuatChiTietModel> lstItem = _iQLVonDauTuService.GetAllKH5NamDeXuatChiTiet(item).ToList();
                        lstItem.Select(x => { x.sMaOrder = string.Format("{0}_{1}", intIndex.ToString("D3"), x.sMaOrder); x.iID_TongHop = item; return x; }).ToList();
                        details.AddRange(lstItem);
                    }

                    if (_lstTongHop != null && _lstTongHop.Count() > 0)
                    {
                        data.sTongHop = string.Join(",", _lstTongHop.GroupBy(x => x.iID_KeHoach5Nam_DeXuatID).Select(x => x.Key).ToList());
                        data.iLoai = _lstTongHop.FirstOrDefault().iLoai;
                    }

                    if (!_iQLVonDauTuService.SaveKeHoach5NamDeXuat(ref data, Username, PhienLamViec.NamLamViec, isModified, isAggregate, details))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    details = _iQLVonDauTuService.GetAllKH5NamDeXuatDieuChinhChiTiet(data.iID_KeHoach5Nam_DeXuatID).ToList();
                    if (isModified && details.Count() == 0)
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không thể điều chỉnh,chưa có dự án nào được duyệt !" }, JsonRequestBehavior.AllowGet);
                    }

                    //if (_iQLVonDauTuService.CheckExistSoKeHoach(data.sSoQuyetDinh, PhienLamViec.NamLamViec, data.iID_KeHoach5Nam_DeXuatID))
                    if (isModified && _iQLVonDauTuService.CheckExistSoKeHoach(data.sSoQuyetDinh, PhienLamViec.NamLamViec, data.iID_KeHoach5Nam_DeXuatID))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (!_iQLVonDauTuService.SaveKeHoach5NamDeXuat(ref data, Username, PhienLamViec.NamLamViec, isModified, isAggregate, details))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true, iID_KeHoach5Nam_DeXuatID = data.iID_KeHoach5Nam_DeXuatID, bIsTrung = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KeHoach5NamDeXuatSaveTrung(VDT_KHV_KeHoach5Nam_DeXuat data, bool isModified, bool isAggregate, List<DuAnKeHoach5Nam> lstDuAnChecked)
        {
            try
            {
                List<VdtKhvKeHoachTrungHanDeXuatChiTietModel> details = new List<VdtKhvKeHoachTrungHanDeXuatChiTietModel>();
                _lstDuAnChecked = new List<DuAnKeHoach5Nam>();

                if (lstDuAnChecked != null && lstDuAnChecked.Count() > 0 && lstDuAnChecked.FirstOrDefault() != null)
                {
                    _lstDuAnChecked = lstDuAnChecked.GroupBy(item => item.IDDuAnID).Select(grp => grp.LastOrDefault()).Where(item => item.IsChecked).ToList();
                }
                if (isAggregate && _lstTongHop != null && _lstTongHop.Count() > 0)
                {
                    foreach (var itemId in _lstTongHop.Select(x => x.iID_KeHoach5Nam_DeXuatID).ToList())
                    {
                        var lstDuAnCheckedChild = _iQLVonDauTuService.GetIDDuAnKHTHDeXuatChiTietByIDDeXuat(itemId).ToList();
                        _lstDuAnChecked.AddRange(lstDuAnCheckedChild);
                    }
                }
                TempData["lstDuAnChecked"] = _lstDuAnChecked;
                if (data.iID_KeHoach5Nam_DeXuatID == new Guid())
                {
                    if (!isAggregate)
                    {
                        if (_iQLVonDauTuService.CheckExistSoKeHoach(data.sSoQuyetDinh, PhienLamViec.NamLamViec, null))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                        }

                        if (!_iQLVonDauTuService.SaveKeHoach5NamDeXuat(ref data, Username, PhienLamViec.NamLamViec, isModified, isAggregate, details))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new
                        {
                            bIsComplete = true,
                            iID_KeHoach5Nam_DeXuatID = data.iID_KeHoach5Nam_DeXuatID
                        }, JsonRequestBehavior.AllowGet);
                    }

                    int intIndex = 0;
                    foreach (var item in _lstTongHop.Select(x => x.iID_KeHoach5Nam_DeXuatID).ToList())
                    {
                        intIndex += 1;
                        List<VdtKhvKeHoachTrungHanDeXuatChiTietModel> lstItem = _iQLVonDauTuService.GetAllKH5NamDeXuatChiTiet(item).ToList();
                        lstItem.Select(x => { x.sMaOrder = string.Format("{0}_{1}", intIndex.ToString("D3"), x.sMaOrder); x.iID_TongHop = item; return x; }).ToList();
                        details.AddRange(lstItem);
                    }

                    if (_lstTongHop != null && _lstTongHop.Count() > 0)
                    {
                        data.sTongHop = string.Join(",", _lstTongHop.GroupBy(x => x.iID_KeHoach5Nam_DeXuatID).Select(x => x.Key).ToList());
                        data.iLoai = _lstTongHop.FirstOrDefault().iLoai;
                    }

                    if (!_iQLVonDauTuService.SaveKeHoach5NamDeXuat(ref data, Username, PhienLamViec.NamLamViec, isModified, isAggregate, details))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { bIsComplete = true, iID_KeHoach5Nam_DeXuatID = data.iID_KeHoach5Nam_DeXuatID }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KeHoach5NamDeXuatExport(Guid? idKeHoach5NamDeXuat)
        {
            try
            {
                List<VdtKhvKeHoach5NamDeXuatExportModel> lstQuery = _iQLVonDauTuService.GetDataExportKeHoachTrungHanDeXuat(idKeHoach5NamDeXuat.Value).ToList();

                ExcelFile xls = CreateReportExport(lstQuery, idKeHoach5NamDeXuat);
                xls.PrintLandscape = true;

                TempData["DataExport"] = xls;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public ExcelFile CreateReportExport(List<VdtKhvKeHoach5NamDeXuatExportModel> lstData, Guid? idKeHoach5NamDeXuat)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/rptKeHoachTrungHan_DeXuat.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            int projectType = 2;
            lstData.Select(item =>
            {
                item.FTongSo = (item.FGiaTriNamThuNhat ?? 0) + (item.FGiaTriNamThuHai ?? 0) + (item.FGiaTriNamThuBa ?? 0)
                                + (item.FGiaTriNamThuTu ?? 0) + (item.FGiaTriNamThuNam ?? 0);
                item.FTongSoNhuCau = (item.FGiaTriBoTri ?? 0) + (item.FTongSo);
                return item;
            }).ToList();

            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(idKeHoach5NamDeXuat);
            int iNamBatDau = 0;
            string sTenDonVi = string.Empty;
            if (itemQuery != null)
            {
                iNamBatDau = itemQuery.iGiaiDoanTu;
                NS_DonVi itemDV = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, itemQuery.iID_DonViQuanLyID.ToString());
                if (itemDV != null)
                {
                    sTenDonVi = itemDV.sTen;
                }
            }

            List<VDT_DM_DonViThucHienDuAn> lstUnit = _danhMucService.GetListDonViThucHienDuAn().ToList();

            VdtKhvKeHoach5NamDeXuatExportModel RptSummary = new VdtKhvKeHoach5NamDeXuatExportModel();
            RptSummary.FHanMucDauTu = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FHanMucDauTu);
            RptSummary.FTongSoNhuCau = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FTongSoNhuCau);
            RptSummary.FGiaTriNamThuNhat = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FGiaTriNamThuNhat);
            RptSummary.FGiaTriNamThuHai = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FGiaTriNamThuHai);
            RptSummary.FGiaTriNamThuBa = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FGiaTriNamThuBa);
            RptSummary.FGiaTriNamThuTu = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FGiaTriNamThuTu);
            RptSummary.FGiaTriNamThuNam = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FGiaTriNamThuNam);
            RptSummary.FGiaTriBoTri = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FGiaTriBoTri);
            RptSummary.FTongSo = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.FTongSo);

            RptSummary.fHanmucNganhDX = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fHanmucNganhDX);
            RptSummary.fTongVonBoTriNganh = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fTongVonBoTriNganh);
            RptSummary.fVon5namNganhDX = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fVon5namNganhDX);
            RptSummary.fVonsaunamNganhDX = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fVonsaunamNganhDX);

            RptSummary.fHanmucCucTCDX = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fHanmucCucTCDX);
            RptSummary.fTongVonBoTriCuc = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fTongVonBoTriCuc);
            RptSummary.fVon5namCTCDX = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fVon5namCTCDX);
            RptSummary.fVonnamthunhatCTC = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fVonnamthunhatCTC);
            RptSummary.fVonsaunamCTCDexuat = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fVonsaunamCTCDexuat);
            RptSummary.fCucTCDeXuat = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fCucTCDeXuat);
            RptSummary.fDuKienBoTriNamThu2 = lstData.Where(x => x.IsStatus.Equals(projectType)).Sum(x => x.fDuKienBoTriNamThu2);

            fr.AddTable<VdtKhvKeHoach5NamDeXuatExportModel>("Items", lstData);
            fr.AddTable<VDT_DM_DonViThucHienDuAn>("dv", lstUnit);

            fr.SetValue("Period", string.Format("{0}-{1}", iNamBatDau, iNamBatDau + 4));
            fr.SetValue("TenDonVi", !string.IsNullOrEmpty(sTenDonVi) ? sTenDonVi.ToUpper() : string.Empty);
            fr.SetValue("iNamLamViec", PhienLamViec.iNamLamViec);
            fr.SetValue("Year1", iNamBatDau);
            fr.SetValue("Year2", iNamBatDau + 1);
            fr.SetValue("Year3", iNamBatDau + 2);
            fr.SetValue("Year4", iNamBatDau + 3);
            fr.SetValue("Year5", iNamBatDau + 4);
            fr.SetValue("FHanMucDauTuSum", RptSummary.FHanMucDauTu);
            fr.SetValue("FTongSoNhuCauSum", RptSummary.FTongSoNhuCau);
            fr.SetValue("FTongSoSum", RptSummary.FTongSo);
            fr.SetValue("FGiaTriNamThuNhatSum", RptSummary.FGiaTriNamThuNhat);
            fr.SetValue("FGiaTriNamThuHaiSum", RptSummary.FGiaTriNamThuHai);
            fr.SetValue("FGiaTriNamThuBaSum", RptSummary.FGiaTriNamThuBa);
            fr.SetValue("FGiaTriNamThuTuSum", RptSummary.FGiaTriNamThuTu);
            fr.SetValue("FGiaTriNamThuNamSum", RptSummary.FGiaTriNamThuNam);
            fr.SetValue("FGiaTriBoTriSum", RptSummary.FGiaTriBoTri);

            fr.SetValue("fHanmucNganhDXSum", RptSummary.fHanmucNganhDX);
            fr.SetValue("fTongVonBoTriNganhSum", RptSummary.fTongVonBoTriNganh);
            fr.SetValue("fVon5namNganhDXSum", RptSummary.fVon5namNganhDX);
            fr.SetValue("fVonsaunamNganhDXSum", RptSummary.fVonsaunamNganhDX);
            fr.SetValue("fHanmucCucTCDXSum", RptSummary.fHanmucCucTCDX);
            fr.SetValue("fTongVonBoTriCucSum", RptSummary.fTongVonBoTriCuc);
            fr.SetValue("fVon5namCTCDXSum", RptSummary.fVon5namCTCDX);
            fr.SetValue("fVonnamthunhatCTCSum", RptSummary.fVonnamthunhatCTC);
            fr.SetValue("fVonsaunamCTCDexuatSum", RptSummary.fVonsaunamCTCDexuat);
            fr.SetValue("fCucTCDeXuatSum", RptSummary.fCucTCDeXuat);
            fr.SetValue("fDuKienBoTriNamThu2Sum", RptSummary.fDuKienBoTriNamThu2);

            fr.Run(Result);
            return Result;
        }

        public FileContentResult ExportExcel5NDeXuat()
        {
            string sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string sFileName = "KeHoachTrungHanDeXuat.xlsx";
            ExcelFile xls = (ExcelFile)TempData["DataExport"];
            xls.PrintLandscape = true;
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return File(stream.ToArray(), sContentType, sFileName);
            }
        }
   
        [HttpPost]
        public ActionResult GetGridListExcelFromFTP(string idDonVi, string giaiDoanTu, string giaiDoanDen)
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
                pathString = pathString + "/" + tenDonVi + "/VDT/KeHoachTrungHanDeXuat/send/" + giaiDoanTu.Trim() + "-" + giaiDoanDen.Trim() + "/";
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
        public bool KeHoach5NamDeXuatDelete(Guid id)
        {
            try
            {
                return _iQLVonDauTuService.deleteKeHoach5NamDeXuat(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        [HttpPost]
        public bool KeHoach5NamDeXuatLock(Guid id)
        {
            try
            {
                VDT_KHV_KeHoach5Nam_DeXuat entity = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(id);
                if (entity != null)
                {
                    bool isLockOrUnlock = entity.iKhoa.HasValue ? entity.iKhoa.Value : false;
                    return _iQLVonDauTuService.LockOrUnLockKeHoach5NamDeXuat(id, !isLockOrUnlock);
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        #region Data grid kế hoạch trung hạn đề xuất chi tiết
        public ActionResult Detail(Guid id, bool isDetail = false, bool isTongHop = false)
        {
            VDT_KHV_KeHoach5Nam_DeXuat data = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(id);
            KeHoach5NamDeXuatChiTietGridViewModel vm = new KeHoach5NamDeXuatChiTietGridViewModel
            {
                KH5NamDeXuat = data,
                isDetail = isDetail,
                isTongHop = isTongHop,
            };
            //TempData["isTongHop"] = isTongHop;
            ViewBag.isDetail = isDetail ? "True" : "False";
            return View(vm);
        }

        public ActionResult SheetFrame(string id, bool isDetail = false, bool isTongHop = false, string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            VDT_KHV_KeHoach5Nam_DeXuat KH5NamDX = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(Guid.Parse(id));
            List<DuAnKeHoach5Nam> lstChecked = new List<DuAnKeHoach5Nam>();
            var itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(Guid.Parse(id));
            if ((List<DuAnKeHoach5Nam>)TempData["lstDuAnChecked"] != null)
            {
                lstChecked = (List<DuAnKeHoach5Nam>)TempData["lstDuAnChecked"];
            }
            else if (itemQuery.iLoai == 2)
            {
                lstChecked = _iQLVonDauTuService.GetIDDuAnKHTHDeXuatChiTietByIDDeXuat(KH5NamDX.iID_KeHoach5Nam_DeXuatID).ToList();                            
            }                                                         
            var sheet = new KeHoach5NamDeXuat_ChiTiet_SheetTable(id, int.Parse(PhienLamViec.iNamLamViec), KH5NamDX.iGiaiDoanTu, lstChecked, filters);
            var vm = new KeHoach5NamDeXuatChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "KeHoachTrungHanDeXuat", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrame", "KeHoachTrungHanDeXuat", new { area = "QLVonDauTu" })
                   ),
                KH5NamDeXuat = KH5NamDX,
                isDetail = isDetail,
                isTongHop = isTongHop,

            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            vm.KH5NamDeXuat = KH5NamDX;
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            try
            {
                //var rows = vm.Rows.Where(x => !x.IsParent || x.IsDeleted).ToList();
                var rows = vm.Rows.ToList();
                if (rows.Count > 0)
                {
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();

                        #region crud
                        //var columns = new KeHoach5NamDeXuat_ChiTiet_SheetTable().Columns.Where(x => !x.IsReadonly);
                        var columns = new KeHoach5NamDeXuat_ChiTiet_SheetTable().Columns.Where(x => !x.IsReadonly || x.ColumnName == "fGiaTriBoTri");
                        rows.ForEach(r =>
                        {
                            var trans = conn.BeginTransaction();
                            var iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.Empty;
                            if (r.Id != null)
                                Guid.TryParse(r.Id, out iID_KeHoach5Nam_DeXuat_ChiTietID);
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                            if (iID_KeHoach5Nam_DeXuat_ChiTietID != null && r.IsDeleted)
                            {
                                #region delete
                                var entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(iID_KeHoach5Nam_DeXuat_ChiTietID, trans);
                                if (entity != null)
                                {
                                    conn.Delete(entity, trans);
                                }
                                #endregion
                            }
                            else if (changes.Any())
                            {
                                string sTenDuAnCha = "";
                                string sMaDonVi = "";
                                int? iID_NguonVonID = 0;
                                string sMaLoaiCongTrinh = "";
                                string sDiadiem = "";
                                int? iGiaiDoanTu = 0;
                                int? iGiaiDoanDen = 0;
                                Guid? iID_DuAn = null;
                                Guid? iID_LoaiCongTrinhID = null;
                                VDT_DM_DonViThucHienDuAn donvi = null;
                                VDT_DM_LoaiCongTrinh congtrinh = null;
                                int Level = 0;
                                int isParrnt = 0;
                                if (r.Columns.ContainsKey("sDuAnCha"))
                                {
                                    sTenDuAnCha = r.Columns["sDuAnCha"];
                                }
                                if (r.Columns.ContainsKey("sDonViThucHienDuAn"))
                                {
                                    sMaDonVi = r.Columns["sDonViThucHienDuAn"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                    donvi = _danhMucService.GetDonViThucHienDuAn(sMaDonVi);
                                }
                                if (r.Columns.ContainsKey("sTenLoaiCongTrinh"))
                                {
                                    sMaLoaiCongTrinh = r.Columns["sTenLoaiCongTrinh"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                    congtrinh = _iQLVonDauTuService.GetDMLoaiCongTrinhByMa(sMaLoaiCongTrinh);
                                }
                                if (r.Columns.ContainsKey("sTenNganSach"))
                                {
                                    if (!string.IsNullOrEmpty(r.Columns["sTenNganSach"]))
                                    {
                                        string strId = r.Columns["sTenNganSach"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                        iID_NguonVonID = Int16.Parse(strId);
                                    }
                                }
                                if (r.Columns.ContainsKey("iID_DuAnID"))
                                {
                                    if (!string.IsNullOrEmpty(r.Columns["iID_DuAnID"]))
                                    {
                                        iID_DuAn = Guid.Parse(r.Columns["iID_DuAnID"].ToString());
                                    }
                                }
                                if (r.Columns.ContainsKey("iID_LoaiCongTrinhID"))
                                {
                                    if (!string.IsNullOrEmpty(r.Columns["iID_LoaiCongTrinhID"]))
                                    {
                                        iID_LoaiCongTrinhID = Guid.Parse(r.Columns["iID_LoaiCongTrinhID"].ToString());
                                    }
                                }
                                if (r.Columns.ContainsKey("sDiaDiem"))
                                {
                                    if (!string.IsNullOrEmpty(r.Columns["sDiaDiem"]))
                                    {
                                        sDiadiem = r.Columns["sDiaDiem"].ToString();
                                    }
                                }
                                if (r.Columns.ContainsKey("iGiaiDoanTu"))
                                {
                                    if (!string.IsNullOrEmpty(r.Columns["iGiaiDoanTu"]))
                                    {
                                        string igdTu = (r.Columns["iGiaiDoanTu"].ToString());
                                        iGiaiDoanTu = Int16.Parse(igdTu);
                                    }
                                }
                                if (r.Columns.ContainsKey("iGiaiDoanDen"))
                                {
                                    if (!string.IsNullOrEmpty(r.Columns["iGiaiDoanDen"]))
                                    {
                                        string igdDen = (r.Columns["iGiaiDoanDen"].ToString());
                                        iGiaiDoanDen = Int16.Parse(igdDen);
                                    }
                                }
                                if (r.Columns.ContainsKey("iLevel"))
                                {
                                    Level = Int16.Parse(r.Columns["iLevel"]);
                                }

                                var entity1 = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(iID_KeHoach5Nam_DeXuat_ChiTietID, trans);

                                if ((iID_KeHoach5Nam_DeXuat_ChiTietID == Guid.Empty && entity1 == null) || entity1 == null)
                                {
                                    #region create
                                    var entity = new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet()
                                    {
                                        bIsParent = (Level == 1) ? true : false
                                    };

                                    entity.iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.Parse(r.Id);
                                    entity.iID_ParentID = Guid.Parse(r.Columns["iID_ParentID"].ToString());
                                    if (r.Columns.ContainsKey("sDonViThucHienDuAn"))
                                    {
                                        entity.iID_DonViQuanLyID = donvi?.iID_DonVi;
                                        entity.iID_MaDonVi = donvi?.iID_MaDonVi;
                                    }
                                    if (r.Columns.ContainsKey("sTenLoaiCongTrinh"))
                                    {
                                        entity.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                    }
                                    if (r.Columns.ContainsKey("sTenNganSach"))
                                    {
                                        entity.iID_NguonVonID = iID_NguonVonID > 0 ? iID_NguonVonID : null;
                                    }
                                    if (r.Columns.ContainsKey("iID_DuAnID"))
                                    {
                                        entity.iID_DuAnID = iID_DuAn != null ? iID_DuAn : Guid.NewGuid();
                                    }
                                    if (r.Columns.ContainsKey("iID_LoaiCongTrinhID"))
                                    {
                                        entity.iID_LoaiCongTrinhID = iID_LoaiCongTrinhID != null ? iID_LoaiCongTrinhID : (congtrinh?.iID_LoaiCongTrinh);
                                    }

                                    if (r.Columns.ContainsKey("fGiaTriBoTri"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fGiaTriBoTri"]) ? Double.Parse(r.Columns["fGiaTriBoTri"]) : 0;
                                    }
                                    
                                    if (r.Columns.ContainsKey("fHanmucNganhDX"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fHanmucNganhDX"]) ? Double.Parse(r.Columns["fHanmucNganhDX"]) : 0;
                                    }
                                                                   
                                    
                                    if (r.Columns.ContainsKey("fVon5namNganhDX"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fVon5namNganhDX"]) ? Double.Parse(r.Columns["fVon5namNganhDX"]) : 0;
                                    }
                                    
                                    if (r.Columns.ContainsKey("fVonsaunamNganhDX"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fVonsaunamNganhDX"]) ? Double.Parse(r.Columns["fVonsaunamNganhDX"]) : 0;
                                    }
                                    
                                    if (r.Columns.ContainsKey("fHanmucCucTCDX"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fHanmucCucTCDX"]) ? Double.Parse(r.Columns["fHanmucCucTCDX"]) : 0;
                                    }
                                    
                                    if (r.Columns.ContainsKey("fVon5namCTCDX"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fVon5namCTCDX"]) ? Double.Parse(r.Columns["fVon5namCTCDX"]) : 0;
                                    } 
                                    
                                    if (r.Columns.ContainsKey("fVonnamthunhatCTC"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fVonnamthunhatCTC"]) ? Double.Parse(r.Columns["fVonnamthunhatCTC"]) : 0;
                                    }
                                    
                                    if (r.Columns.ContainsKey("fVonsaunamCTCDexuat"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fVonsaunamCTCDexuat"]) ? Double.Parse(r.Columns["fVonsaunamCTCDexuat"]) : 0;
                                    }
                                    
                                    if (r.Columns.ContainsKey("fCucTCDeXuat"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fCucTCDeXuat"]) ? Double.Parse(r.Columns["fCucTCDeXuat"]) : 0;
                                    }
                                    
                                    if (r.Columns.ContainsKey("fDuKienBoTriNamThu2"))
                                    {
                                        entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fDuKienBoTriNamThu2"]) ? Double.Parse(r.Columns["fDuKienBoTriNamThu2"]) : 0;
                                    }

                                    if (Level == 1)
                                    {
                                        SetSTTEntityGroup(entity, vm.Id);
                                    }
                                    else
                                    {
                                        SetSTTEntity(entity, sTenDuAnCha, vm.Id, Level);
                                    }
                                    entity.MapFrom(changes);
                                    entity.iID_KeHoach5Nam_DeXuatID = Guid.Parse(vm.Id);
                                    entity.iID_DonViID = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(vm.Id, trans).iID_DonViQuanLyID;
                                    entity.iID_MaDonVi = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(vm.Id, trans).iID_MaDonViQuanLy;
                                    entity.sSTT = r.Columns["sSTT"];
                                    conn.Insert(entity, trans);
                                    if (Level == 2)
                                    {
                                        entity.iIDReference = entity.iID_KeHoach5Nam_DeXuat_ChiTietID;
                                        conn.Update(entity, trans);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edit
                                    var entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(iID_KeHoach5Nam_DeXuat_ChiTietID, trans);
                                    if (r.Columns.ContainsKey("sDonViThucHienDuAn"))
                                    {
                                        entity.iID_DonViQuanLyID = donvi?.iID_DonVi;
                                        entity.iID_MaDonVi = donvi.iID_MaDonVi;
                                    }
                                    if (r.Columns.ContainsKey("sTenLoaiCongTrinh"))
                                    {
                                        entity.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                    }
                                    if (r.Columns.ContainsKey("sTenNganSach"))
                                    {
                                        entity.iID_NguonVonID = iID_NguonVonID > 0 ? iID_NguonVonID : null;
                                    }
                                    if (r.Columns.ContainsKey("iID_LoaiCongTrinhID"))
                                    {
                                        entity.iID_LoaiCongTrinhID = iID_LoaiCongTrinhID != null ? iID_LoaiCongTrinhID : null;
                                    }


                                    if (Level != 1)
                                    {
                                        if (r.Columns.ContainsKey("sDuAnCha"))
                                        {
                                            //Lấy dự án cha
                                            string sttDuAnCha = sTenDuAnCha.Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                            VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet KH5NamDeXuatCha = _iQLVonDauTuService.GetKH5NamDeXuatChaBySTTDuAn(vm.Id, sttDuAnCha);
                                            if ((KH5NamDeXuatCha != null && entity.iID_ParentID != KH5NamDeXuatCha.iID_KeHoach5Nam_DeXuat_ChiTietID) || (KH5NamDeXuatCha == null && entity.iID_ParentID != null))
                                            {
                                                SetSTTEntity(entity, sTenDuAnCha, vm.Id, Level);
                                            }
                                        }
                                    }
                                    //SetSTTEntity(entity, sTenDuAnCha, vm.Id);
                                    entity.MapFrom(changes);
                                    if (entity != null && entity.iID_ParentModified != null && entity.iID_ParentModified != Guid.Empty)
                                    {
                                        if (r.Columns.ContainsKey("fGiaTriNamThuNhatDc"))
                                        {
                                            entity.fGiaTriNamThuNhat = !string.IsNullOrEmpty(r.Columns["fGiaTriNamThuNhatDc"]) ? Double.Parse(r.Columns["fGiaTriNamThuNhatDc"]) : 0;
                                        }
                                        if (r.Columns.ContainsKey("fGiaTriNamThuHaiDc"))
                                        {
                                            entity.fGiaTriNamThuHai = !string.IsNullOrEmpty(r.Columns["fGiaTriNamThuHaiDc"]) ? Double.Parse(r.Columns["fGiaTriNamThuHaiDc"]) : 0;
                                        }
                                        if (r.Columns.ContainsKey("fGiaTriNamThuBaDc"))
                                        {
                                            entity.fGiaTriNamThuBa = !string.IsNullOrEmpty(r.Columns["fGiaTriNamThuBaDc"]) ? Double.Parse(r.Columns["fGiaTriNamThuBaDc"]) : 0;
                                        }
                                        if (r.Columns.ContainsKey("fGiaTriNamThuTuDc"))
                                        {
                                            entity.fGiaTriNamThuTu = !string.IsNullOrEmpty(r.Columns["fGiaTriNamThuTuDc"]) ? Double.Parse(r.Columns["fGiaTriNamThuTuDc"]) : 0;
                                        }
                                        if (r.Columns.ContainsKey("fGiaTriNamThuNamDc"))
                                        {
                                            entity.fGiaTriNamThuNam = !string.IsNullOrEmpty(r.Columns["fGiaTriNamThuNamDc"]) ? Double.Parse(r.Columns["fGiaTriNamThuNamDc"]) : 0;
                                        }
                                        if (r.Columns.ContainsKey("fGiaTriBoTriDc"))
                                        {
                                            entity.fGiaTriBoTri = !string.IsNullOrEmpty(r.Columns["fGiaTriBoTriDc"]) ? Double.Parse(r.Columns["fGiaTriBoTriDc"]) : 0;
                                        }


                                    }
                                    conn.Update(entity, trans);
                                    #endregion
                                }
                            }
                            // commit to db
                            trans.Commit();
                        });
                        conn.Close();
                        #endregion
                    }
                }
                //update isparent
                _iQLVonDauTuService.UpdateIsParent(vm.Id);
                //update fGiaTriKeHoach
                _iQLVonDauTuService.UpdateGiaTriKeHoach(vm.Id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            // clear cache
            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filters = vm.FiltersString });
        }

        public void SetSTTEntityGroup(VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet entity, string iID_KeHoach5NamID)
        {
            string STTMax = "";
            string sMaOrder = "";
            string newSTT = "";
            int indexCode = 0;
            Guid? IdReference = null;

            int numChild = _iQLVonDauTuService.GetNumchild(iID_KeHoach5NamID);
            if (numChild > 0)
            {
                STTMax = _iQLVonDauTuService.GetMaxSTTDuAn(iID_KeHoach5NamID);
                int intSTT = Int32.Parse(STTMax);
                sMaOrder = (intSTT + 1).ToString();
                newSTT = (intSTT + 1).ToString();
                indexCode = intSTT + 1;
            }
            else
            {
                sMaOrder = "1";
                newSTT = "1";
                indexCode = 1;
            }
            entity.iID_ParentID = null;
            entity.sMaOrder = sMaOrder;
            entity.sSTT = newSTT;
            entity.iLevel = 1;
            entity.iIndexCode = indexCode;
            entity.iIDReference = IdReference;
        }

        public void SetSTTEntity(VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet entity, string sTenDuAnCha, string iID_KeHoach5NamID, int Level)
        {
            string STTMax = "";
            string SMaOrder = "";
            string newSTT = "";
            int indexCode = 0;
            if (string.IsNullOrEmpty(sTenDuAnCha))
            {
                int numChild = _iQLVonDauTuService.GetNumchild(iID_KeHoach5NamID);
                if (numChild > 0)
                {
                    STTMax = _iQLVonDauTuService.GetMaxSTTDuAn(iID_KeHoach5NamID);
                    //int intSTT = arabic(STTMax);
                    int intSTT = Int32.Parse(STTMax);
                    SMaOrder = (intSTT + 1).ToString();
                    //newSTT = roman(intSTT + 1);
                    newSTT = (intSTT + 1).ToString();
                    indexCode = intSTT + 1;
                }
                else
                {
                    SMaOrder = "1";
                    newSTT = "1";
                    indexCode = 1;
                }
                //entity.iID_ParentID = null;
            }
            else
            {
                //Lấy dự án cha
                string sttDuAnCha = sTenDuAnCha.Split(new string[] { "-" }, StringSplitOptions.None)[0];
                VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet KH5NamDeXuatCha = _iQLVonDauTuService.GetKH5NamDeXuatChaBySTTDuAn(iID_KeHoach5NamID, sttDuAnCha);
                //Lấy max indexCode của các dự án con
                int maxIndexCode = _iQLVonDauTuService.GetMaxIndexCode(iID_KeHoach5NamID, KH5NamDeXuatCha.iID_KeHoach5Nam_DeXuat_ChiTietID);

                if (maxIndexCode == 0)
                {
                    indexCode = 1;
                    newSTT = KH5NamDeXuatCha.sSTT + "." + indexCode;
                }
                else
                {
                    indexCode = maxIndexCode + 1;
                    newSTT = KH5NamDeXuatCha.sSTT + "." + indexCode;
                }
                SMaOrder = KH5NamDeXuatCha.sMaOrder + "." + indexCode;
                //entity.iID_ParentID = KH5NamDeXuatCha.iID_KeHoach5Nam_DeXuat_ChiTietID;
                if (Level == 3)
                {
                    entity.iIDReference = KH5NamDeXuatCha.iIDReference;
                }
            }
            entity.sMaOrder = SMaOrder;
            entity.sSTT = newSTT;
            entity.iLevel = Level;
            entity.iIndexCode = indexCode;
            //entity.IdReference = IdReference;
        }

        public void SetSTTEntityCopy(VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet entity, string IdReference)
        {
            //Lấy dự án Reference
            VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet KH5NamDeXuatReference = _iQLVonDauTuService.GetKH5NamDeXuatChiTietById(Guid.Parse(IdReference));
            //entity.iID_ParentID = KH5NamDeXuatReference.iID_ParentID;
            entity.sMaOrder = KH5NamDeXuatReference.sMaOrder;
            entity.sSTT = KH5NamDeXuatReference.sSTT;
            entity.iLevel = KH5NamDeXuatReference.iLevel;
            entity.iIndexCode = KH5NamDeXuatReference.iIndexCode;
            entity.iIDReference = Guid.Parse(IdReference);
        }

        [HttpPost]
        public JsonResult ValidateBeforeSave(List<KeHoach5NamDeXuatChiTietGridViewModel> aListModel)
        {
            try
            {
                var listErrMess = new List<string>();
                if (aListModel == null || !aListModel.Any())
                {
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }

                if (listErrMess != null && listErrMess.Any())
                {
                    return Json(new { status = false, listErrMess = listErrMess }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult ImportKH5NDX()
        {
            KeHoach5NamDeXuatChiTietGridViewModel vm = new KeHoach5NamDeXuatChiTietGridViewModel();
            VDT_KHV_KeHoach5Nam_DeXuat KH5NamDeXuat = new VDT_KHV_KeHoach5Nam_DeXuat();
            //KH5NamDeXuat.iGiaiDoanTu = DateTime.Now.Year;
            //KH5NamDeXuat.iGiaiDoanDen = DateTime.Now.Year + 4;
            KH5NamDeXuat.iGiaiDoanTu = _iQLVonDauTuService.FindCurrentPeriod(PhienLamViec.NamLamViec);
            KH5NamDeXuat.iGiaiDoanDen = KH5NamDeXuat.iGiaiDoanTu + 4;
            vm.KH5NamDeXuat = KH5NamDeXuat;

            //ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
            var listDonViQuanLyAll = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            listDonViQuanLyAll.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn--" });
            ViewBag.ListDonViQuanLy = listDonViQuanLyAll.ToSelectList("iID_Ma", "sMoTa");

            ViewBag.ListChungTu = _iQLVonDauTuService.GetKeHoach5NamDeXuatByCondition().ToSelectList("iID_KeHoach5Nam_DeXuatID", "sSoQuyetDinh");
            ViewBag.LoaiChungTu = LoadLoaiChungTu().ToSelectList("ValueItem", "DisplayItem");
            ViewBag.LoaiCongTrinh = LoadLoaiCongTrinh().ToSelectList("ValueItem", "DisplayItem");
            return View(vm);
        }

        [HttpPost]
        public JsonResult LoadDataExcel(HttpPostedFileBase file)
        {
            try
            {
                var file_data = getBytes(file);
                var dt = ExcelHelpers.LoadExcelDataTable(file_data);
                IEnumerable<KeHoach5NamDeXuatDataImportModel> dataImport = excel_result(dt);
                TempData["dataImport"] = dataImport;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet); ;
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
                            IEnumerable<KeHoach5NamDeXuatDataImportModel> dataImport = excel_result(dt);
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

        private List<ComBoBoxItem> LoadLoaiChungTu()
        {
            List<ComBoBoxItem> loaiChungTu = new List<ComBoBoxItem>()
            {
                new ComBoBoxItem(){DisplayItem = "Mở mới", ValueItem = "1"},
                new ComBoBoxItem(){DisplayItem = "Điều chỉnh", ValueItem = "2"}
            };

            return loaiChungTu;
        }

        private List<ComBoBoxItem> LoadLoaiCongTrinh()
        {
            List<ComBoBoxItem> loaiCongTrinh = new List<ComBoBoxItem>()
            {
                new ComBoBoxItem(){DisplayItem = "--Chọn--", ValueItem = "0"},
                new ComBoBoxItem(){DisplayItem = "Khởi công mới", ValueItem = "1"},
                new ComBoBoxItem(){DisplayItem = "Chuyển tiếp", ValueItem = "2"}
            };

            return loaiCongTrinh;
        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private List<VDT_DA_ChuTruongDauTu_HangMuc> TaoMoiListChuTruongHangMucFrom_VDT_DA_DuAn_HangMucModel(List<VDT_DA_DuAn_HangMucModel> list, List<Guid> listCTDT_ID, List<VDT_DA_ChuTruongDauTu_DM_HangMuc> listDM)
        {
            List<VDT_DA_ChuTruongDauTu_HangMuc> output = new List<VDT_DA_ChuTruongDauTu_HangMuc>();
            for (int i = 0; i < list.Count(); i++)
            {
                VDT_DA_ChuTruongDauTu_HangMuc hm = new VDT_DA_ChuTruongDauTu_HangMuc();
                hm.iID_HangMucID = listDM[i].iID_ChuTruongDauTu_DM_HangMucID;
                hm.iID_ChuTruongDauTuID = listCTDT_ID[i];
                output.Add(hm);
            }
            return output;
        }

        private List<VDT_DA_ChuTruongDauTu_DM_HangMuc> TaoMoiListChuTruong_DMHangMuc_From_VDT_DA_DuAn_HangMucModel(List<VDT_DA_DuAn_HangMucModel> list)
        {
            List<VDT_DA_ChuTruongDauTu_DM_HangMuc> output = new List<VDT_DA_ChuTruongDauTu_DM_HangMuc>();
            list.ForEach(i =>
            {
                VDT_DA_ChuTruongDauTu_DM_HangMuc hm = new VDT_DA_ChuTruongDauTu_DM_HangMuc();
                hm.iID_ChuTruongDauTu_DM_HangMucID = Guid.NewGuid();
                hm.iID_DuAnID = i.iID_DuAnID;
                hm.sMaHangMuc = "000-000";
                hm.sTenHangMuc = i.sTenHangMuc;
                hm.iID_LoaiCongTrinhID = i.iID_LoaiCongTrinhID;
                output.Add(hm);
            });
            return output;
        }

        private List<VDT_DA_ChuTruongDauTu_HangMuc> TaoMoiListChuTruongHangMucFrom_VDT_DA_DuAn_HangMuc(List<VDT_DA_DuAn_HangMuc> list, List<Guid> listCTDT_ID, List<VDT_DA_ChuTruongDauTu_DM_HangMuc> listDM)
        {
            List<VDT_DA_ChuTruongDauTu_HangMuc> output = new List<VDT_DA_ChuTruongDauTu_HangMuc>();

            for (int i = 0; i < list.Count(); i++)
            {
                VDT_DA_ChuTruongDauTu_HangMuc hm = new VDT_DA_ChuTruongDauTu_HangMuc();
                hm.iID_HangMucID = listDM[i].iID_ChuTruongDauTu_DM_HangMucID;
                hm.iID_ChuTruongDauTuID = listCTDT_ID[i]; 
                output.Add(hm);
            }
            /*
            list.ForEach(i =>
            {
                VDT_DA_ChuTruongDauTu_HangMuc hm = new VDT_DA_ChuTruongDauTu_HangMuc();
                hm.iID_HangMucID = i.iID_DuAn_HangMucID;
                hm.iID_ChuTruongDauTuID = iID_ChuTruongDauTuID;
                output.Add(hm);
            });
            */
            return output;
        }

        private List<VDT_DA_ChuTruongDauTu_DM_HangMuc> TaoMoiListChuTruong_DMHangMuc_From_VDT_DA_DuAn_HangMuc(List<VDT_DA_DuAn_HangMuc> list)
        {
            List<VDT_DA_ChuTruongDauTu_DM_HangMuc> output = new List<VDT_DA_ChuTruongDauTu_DM_HangMuc>();
            list.ForEach(i =>
            {
                VDT_DA_ChuTruongDauTu_DM_HangMuc hm = new VDT_DA_ChuTruongDauTu_DM_HangMuc();
                hm.iID_ChuTruongDauTu_DM_HangMucID = Guid.NewGuid();
                hm.iID_DuAnID = i.iID_DuAnID;
                hm.sMaHangMuc = "000-000";
                hm.sTenHangMuc = i.sTenHangMuc;
                hm.iID_LoaiCongTrinhID = i.iID_LoaiCongTrinhID;
                output.Add(hm);
            });
            return output;
        }

        private List<VDT_DA_ChuTruongDauTu_NguonVon> TaoMoiListChuTruongNguonVonFrom_VDTDuAnListNguonVonTTDuAnModel(List<VDTDuAnListNguonVonTTDuAnModel> list, List<Guid> listCTDT_ID)
        {
            List<VDT_DA_ChuTruongDauTu_NguonVon> output = new List<VDT_DA_ChuTruongDauTu_NguonVon>();
            for (int i = 0; i < list.Count(); i++)
            {
                VDT_DA_ChuTruongDauTu_NguonVon hm = new VDT_DA_ChuTruongDauTu_NguonVon();
                hm.iID_NguonVonID = list[i].iID_NguonVonID;
                hm.iID_ChuTruongDauTuID = listCTDT_ID[i];
                hm.fTienPheDuyet = list[i].fThanhTien;
                output.Add(hm);
            }
            return output;
        }

        private List<VDT_DA_ChuTruongDauTu_NguonVon> TaoMoiListChuTruongNguonVonFrom_VDT_DA_DuAn_NguonVon(List<VDT_DA_DuAn_NguonVon> list, List<Guid> listCTDT_ID)
        {
            List<VDT_DA_ChuTruongDauTu_NguonVon> output = new List<VDT_DA_ChuTruongDauTu_NguonVon>();
            for (int i = 0; i < list.Count(); i++)
            {
                VDT_DA_ChuTruongDauTu_NguonVon hm = new VDT_DA_ChuTruongDauTu_NguonVon();
                hm.iID_NguonVonID = list[i].iID_NguonVonID;
                hm.iID_ChuTruongDauTuID = listCTDT_ID[i];
                hm.fTienPheDuyet = list[i].fThanhTien;
                output.Add(hm);
            }
            return output;
        }

        private bool CheckIfDuAnCoChiTiet(string stt, EnumerableRowCollection<DataRow> items)
        {
            bool output = false;
            for (var i = 10; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];
                var sSTT = r.Field<string>(0);
                if (sSTT.Contains(stt) && sSTT != stt)
                {
                    output = true;
                    break;
                }
            }
            return output;
        }

        private IEnumerable<KeHoach5NamDeXuatDataImportModel> excel_result(DataTable dt)
        {
            List<KeHoach5NamDeXuatDataImportModel> dataImport = new List<KeHoach5NamDeXuatDataImportModel>();

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) != "" || x.Field<string>(0) != null || x.Field<string>(0) != "null");
            List<VDT_DA_DuAn> listDuAnPendingSave = new List<VDT_DA_DuAn>();
            List<VDT_DA_ChuTruongDauTu> ctdtNewPendingSave = new List<VDT_DA_ChuTruongDauTu>();
            List<VDT_DA_DuAn_HangMuc> listDuAnHangMucPendingSave = new List<VDT_DA_DuAn_HangMuc>();
            List<VDT_DA_DuAn_NguonVon> listDuAnNguonVonPendingSave = new List<VDT_DA_DuAn_NguonVon>();
            List<VDT_DA_ChuTruongDauTu_HangMuc> listCTDTHangMucPendingSave = new List<VDT_DA_ChuTruongDauTu_HangMuc>();
            List<VDT_DA_ChuTruongDauTu_DM_HangMuc> listCTDT_DMHangMuc_PendingSave = new List<VDT_DA_ChuTruongDauTu_DM_HangMuc>();
            List<VDT_DA_ChuTruongDauTu_NguonVon> listCTDTNguonVonPendingSave = new List<VDT_DA_ChuTruongDauTu_NguonVon>();
            List<Guid> listCTDT_ID = new List<Guid>();
            for (var i = 10; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];

                var sSTT = r.Field<string>(0);
                var iLevel = "0";
                string iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.NewGuid().ToString();
                string iID_ParentID = null;
                var bIsParent = 0;

                // STT - 1,2,3,... -> Nhóm dự án
                // STT - 1.1; 1.2; 1.3; ... -> Dự án
                // STT - 1.1.1; 1.1.2; ... -> Chi tiết dự án
                if (!sSTT.Contains('.'))
                {
                    iLevel = "1";
                }
                else if(sSTT.Split('.').Length == 2)
                {
                    iLevel = "2";
                    string parentSTT = sSTT.Split('.')[0];
                    KeHoach5NamDeXuatDataImportModel parent = dataImport.Where(x => x.sSTT == parentSTT).FirstOrDefault();
                    if (parent != null)
                    {
                        parent.bIsParent = 1;
                        iID_ParentID = parent.iID_KeHoach5Nam_DeXuat_ChiTietID;
                    }
                }
                else if(sSTT.Split('.').Length == 3)
                {
                    iLevel = "3";
                    string parentSTT = sSTT.Split('.')[0] + '.' + sSTT.Split('.')[1];
                    KeHoach5NamDeXuatDataImportModel parent = dataImport.Where(x => x.sSTT == parentSTT).FirstOrDefault();
                    if (parent != null)
                    {
                        parent.bIsParent = 1;
                        iID_ParentID = parent.iID_KeHoach5Nam_DeXuat_ChiTietID;
                    }
                }

                var sSoQuyetDinhCTDT = r.Field<string>(3);
                var sTen = r.Field<string>(1);
                string sMaDonVi_ThucHienDuAn = r.Field<string>(5);
                var sDiaDiem = r.Field<string>(6);
                var iGiaiDoanTu = r.Field<string>(7) != null ? r.Field<string>(7).Split(new string[] { "-" }, StringSplitOptions.None)[0] : "";
                var iGiaiDoanDen = r.Field<string>(7) != null ? r.Field<string>(7).Split(new string[] { "-" }, StringSplitOptions.None)[1] : "";
                string sMaLoaiCongTrinh = r.Field<string>(8);
                string sMaNguonVon = r.Field<string>(9);
                var arrSTT = sSTT.Split(new string[] { "." }, StringSplitOptions.None);
                var iIndexCode = arrSTT[arrSTT.Length - 1];
                var sMaDuAn = r.Field<string>(2);
                var fHanMucDauTu = r.Field<string>(10);
                var fTongSoNhuCauNSQP = r.Field<string>(11);
                var fTongSo = r.Field<string>(12);
                var fGiaTriNamThuNhat = r.Field<string>(13);
                var fGiaTriNamThuHai = r.Field<string>(14);
                var fGiaTriNamThuBa = r.Field<string>(15);
                var fGiaTriNamThuTu = r.Field<string>(16);
                var fGiaTriNamThuNam = r.Field<string>(17);
                var fGiaTriBoTri = r.Field<string>(18);
                var sGhiChu = r.Field<string>(19);
                var sMaOrder = sSTT;
                var iIDReference = iID_ParentID;
                string sMaChuDauTu = r.Field<string>(4);

                // kiểm tra số quyết định CTDT này đã tồn tại chưa
                bool ctdtExisted = _iQLVonDauTuService.CheckEzistSoQuyetDinhChuTruongDauTu(sSoQuyetDinhCTDT);
                if (ctdtExisted)
                {
                    VDT_DA_ChuTruongDauTu ctdtSaved = _iQLVonDauTuService.GetChuTruongDauTUBySoQuyetDinh(sSoQuyetDinhCTDT);                    
                    // kiểm tra xem mã dự án trong dòng hiện tại có map với mã dự án đang được lưu cho CTDT này hay k
                    string maDuanSaved = _iQLVonDauTuService.GetDuAnById(ctdtSaved.iID_DuAnID).sMaDuAn;
                    if (sMaDuAn == maDuanSaved)
                    {                        
                        NS_DonVi dv_ThucHienDuAn = _iQLVonDauTuService.GetNSDonViByMaDonVi(sMaDonVi_ThucHienDuAn);  
                        VDT_DM_LoaiCongTrinh lct = _iQLVonDauTuService.GetDMLoaiCongTrinhByMa(sMaLoaiCongTrinh);
                        NS_NguonNganSach nv = sMaNguonVon != null ? _iQLVonDauTuService.GetNganSachByMa(sMaNguonVon) : null;
                        DM_ChuDauTu cdt = _danhMucService.GetChuDauTuByMa(sMaChuDauTu);
                        var e = new KeHoach5NamDeXuatDataImportModel
                        {
                            sSTT = sSTT,
                            sTen = sTen,
                            sTenDonViQL = dv_ThucHienDuAn != null ? dv_ThucHienDuAn.sTen : "", 
                            sDiaDiem = sDiaDiem,
                            iGiaiDoanTu = iGiaiDoanTu,
                            iGiaiDoanDen = iGiaiDoanDen,
                            sTenLoaiCongTrinh = sMaLoaiCongTrinh != null ? lct.sTenLoaiCongTrinh : "",
                            iID_LoaiCongTrinhID = sMaLoaiCongTrinh != null ? lct.iID_LoaiCongTrinh.ToString() : null,
                            sTenNganSach = sMaNguonVon != null ? nv.sTen : null,
                            fHanMucDauTu = fHanMucDauTu,
                            fTongSoNhuCauNSQP = fTongSoNhuCauNSQP,
                            fTongSo = fTongSo,
                            fGiaTriNamThuNhat = fGiaTriNamThuNhat,
                            fGiaTriNamThuHai = fGiaTriNamThuHai,
                            fGiaTriNamThuBa = fGiaTriNamThuBa,
                            fGiaTriNamThuTu = fGiaTriNamThuTu,
                            fGiaTriNamThuNam = fGiaTriNamThuNam,
                            fGiaTriBoTri = fGiaTriBoTri,
                            sGhiChu = sGhiChu,
                            sMaOrder = sMaOrder,
                            iIDReference = iIDReference,
                            iID_ParentID = iID_ParentID,
                            iID_KeHoach5Nam_DeXuat_ChiTietID = iID_KeHoach5Nam_DeXuat_ChiTietID,
                            bIsParent = bIsParent,
                            iLevel = iLevel,
                            iIndexCode = iIndexCode,
                            sMaLoaiCongTrinh = sMaLoaiCongTrinh,
                            iID_NguonVonID = sMaNguonVon != null ? nv.iID_MaNguonNganSach.ToString() : null,
                            iID_MaDonVi = dv_ThucHienDuAn != null ? dv_ThucHienDuAn.iID_MaDonVi.ToString() : null, 
                            isMap = 1,
                            sTenChuDauTu = iLevel != "1" ? cdt.sTenCDT : "",
                            sMaChuDauTu = iLevel != "1" ? cdt.sId_CDT : "",
                            sMaDuAn = iLevel != "1" ? sMaDuAn : "",
                            sSoQuyetDinhCTDT = iLevel != "1" ? sSoQuyetDinhCTDT: ""
                        };
                        dataImport.Add(e);
                    }
                    else
                    {
                        // tạm thời chưa xử lý
                        continue;
                    }
                }
                else
                {
                    // tạm thời bỏ qua dòng mà khai báo CTDT k tồn tại
                    if (iLevel != "1")
                    {
                        if (iLevel == "2")
                        {
                            Guid idCTDT = Guid.NewGuid();                            
                            // tạo mới Dụ án, nêu chưa có trong DB
                            // nếu dự án chỉ có 1 dòng, insert 1 hạng mục và 1 nguồn vốn
                            if (listDuAnPendingSave.Where(x => x.sMaDuAn == sMaDuAn).Count() == 0)
                            {
                                bool isDuAnExist = _iQLVonDauTuService.GetDuAnBysMaDuAn(sMaDuAn).Count() > 0;
                                if (!isDuAnExist)
                                {
                                    VDT_DA_DuAn newDuAn = new VDT_DA_DuAn();
                                    Guid iđuAnTemp = Guid.NewGuid();
                                    newDuAn.iID_DuAnID = iđuAnTemp;
                                    newDuAn.sMaDuAn = sMaDuAn;
                                    newDuAn.iID_DonViQuanLyID = null; // tý lưu sau vì tạm thời chưa có
                                    NS_DonVi dv_ThucHienDuAn1 = _iQLVonDauTuService.GetNSDonViByMaDonVi(sMaDonVi_ThucHienDuAn);
                                    newDuAn.iID_DonViThucHienDuAnID = dv_ThucHienDuAn1.iID_Ma;
                                    newDuAn.sTenDuAn = sTen;
                                    DM_ChuDauTu cdt2 = _danhMucService.GetChuDauTuByMa(sMaChuDauTu);
                                    newDuAn.iID_ChuDauTuID = cdt2.ID;
                                    newDuAn.iID_CapPheDuyetID = null;
                                    newDuAn.iID_LoaiCongTrinhID = null;
                                    newDuAn.sDiaDiem = sDiaDiem;
                                    newDuAn.sSuCanThietDauTu = null;
                                    newDuAn.sTrangThaiDuAn = "KhoiTao";
                                    newDuAn.dDateCreate = DateTime.Now;
                                    newDuAn.iID_MaDonViThucHienDuAnID = sMaDonVi_ThucHienDuAn;
                                    newDuAn.iID_MaCDT = cdt2.sId_CDT;
                                    newDuAn.iID_MaDonVi = null;
                                    newDuAn.sUserCreate = Username;
                                    newDuAn.bIsDeleted = false;
                                    newDuAn.sMucTieu = "";
                                    newDuAn.sQuyMo = null;
                                    newDuAn.sKhoiCong = iGiaiDoanTu;
                                    newDuAn.sKetThuc = iGiaiDoanDen;
                                    newDuAn.iID_NhomDuAnID = null;
                                    newDuAn.iID_HinhThucQuanLyID = null;
                                    newDuAn.fHanMucDauTu = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                                    newDuAn.bIsDuPhong = false;
                                    listDuAnPendingSave.Add(newDuAn);

                                    bool hasChiTiet = CheckIfDuAnCoChiTiet(sSTT, items);
                                    if (!hasChiTiet)
                                    {
                                        VDT_DA_DuAn_HangMuc hmNew = new VDT_DA_DuAn_HangMuc();
                                        hmNew.iID_DuAnID = listDuAnPendingSave.LastOrDefault().iID_DuAnID;
                                        hmNew.iID_DuAn_HangMucID = Guid.NewGuid();
                                        hmNew.sMaHangMuc = "000-000";
                                        VDT_DM_LoaiCongTrinh lct1 = _iQLVonDauTuService.GetDMLoaiCongTrinhByMa(sMaLoaiCongTrinh);
                                        hmNew.sTenHangMuc = sTen + " - " + lct1.sTenLoaiCongTrinh;
                                        hmNew.fTienHangMuc = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                                        hmNew.fHanMucDauTu = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                                        hmNew.iID_NguonVonID = Int16.Parse(sMaNguonVon);
                                        hmNew.iID_LoaiCongTrinhID = lct1.iID_LoaiCongTrinh;
                                        listDuAnHangMucPendingSave.Add(hmNew);

                                        VDT_DA_DuAn_NguonVon danv = new VDT_DA_DuAn_NguonVon();
                                        danv.iID_DuAn = listDuAnPendingSave.LastOrDefault().iID_DuAnID;
                                        danv.iID_NguonVonID = Int16.Parse(sMaNguonVon);
                                        danv.fThanhTien = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                                        listDuAnNguonVonPendingSave.Add(danv);
                                        listCTDT_ID.Add(idCTDT);

                                        listCTDT_DMHangMuc_PendingSave = TaoMoiListChuTruong_DMHangMuc_From_VDT_DA_DuAn_HangMuc(listDuAnHangMucPendingSave);
                                        listCTDTHangMucPendingSave = TaoMoiListChuTruongHangMucFrom_VDT_DA_DuAn_HangMuc(listDuAnHangMucPendingSave, listCTDT_ID, listCTDT_DMHangMuc_PendingSave);
                                        listCTDTNguonVonPendingSave = TaoMoiListChuTruongNguonVonFrom_VDT_DA_DuAn_NguonVon(listDuAnNguonVonPendingSave, listCTDT_ID);
                                    }
                                }
                            }
                            // tạo mới CTDT
                            // kiểm tra dự án đã có trong DB chưa, có thì lấy ID k thì tý lấy trong listDuAnPending (vì chắc j đã lưu :) )
                            VDT_DA_DuAn duanExist = _iQLVonDauTuService.GetDuAnBysMaDuAn(sMaDuAn).FirstOrDefault();
                            VDT_DA_ChuTruongDauTu ctdtNew = new VDT_DA_ChuTruongDauTu();
                            if (duanExist != null)
                            {
                                ctdtNew.iID_DuAnID = duanExist.iID_DuAnID;
                            }
                            else ctdtNew.iID_DuAnID = listDuAnPendingSave.LastOrDefault().iID_DuAnID;
                            ctdtNew.sSoQuyetDinh = sSoQuyetDinhCTDT;
                            ctdtNew.dNgayQuyetDinh = DateTime.Now;
                            ctdtNew.iID_DonViQuanLyID = null; // tý lưu
                            DM_ChuDauTu cdt1 = _danhMucService.GetChuDauTuByMa(sMaChuDauTu);
                            ctdtNew.iID_ChuDauTuID = cdt1.ID;
                            ctdtNew.iID_NhomDuAnID = null;
                            ctdtNew.sKhoiCong = iGiaiDoanTu;
                            ctdtNew.sUserCreate = Username;
                            ctdtNew.dDateCreate = DateTime.Now;
                            ctdtNew.iID_MaDonViQuanLy = null;
                            ctdtNew.iID_MaChuDauTuID = cdt1.sId_CDT;
                            ctdtNew.dDateUpdate = DateTime.Now;
                            ctdtNew.sHoanThanh = iGiaiDoanDen;
                            ctdtNew.bActive = true;
                            ctdtNew.iID_CapPheDuyetID = null;
                            ctdtNew.bActive = true;
                            ctdtNew.fTMDTDuKienPheDuyet = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                            ctdtNew.iID_ChuTruongDauTuID = idCTDT;
                            ctdtNewPendingSave.Add(ctdtNew);
                        }
                        else
                        {
                            // nếu dự án đã được lưu trước đó -> lấy danh sách chi tiết dự án lên để tạo ra các chi tiết CTDT 
                            VDT_DA_DuAn duanExist = _iQLVonDauTuService.GetDuAnBysMaDuAn(sMaDuAn).FirstOrDefault();
                            List<VDTDuAnListNguonVonTTDuAnModel> duAnNguonVon = new List<VDTDuAnListNguonVonTTDuAnModel>();
                            List<VDT_DA_DuAn_HangMucModel> duAnHangMuc = new List<VDT_DA_DuAn_HangMucModel>();
                            Guid idCTDT = ctdtNewPendingSave.LastOrDefault().iID_ChuTruongDauTuID;
                            listCTDT_ID.Add(idCTDT);
                            if (duanExist != null)
                            {
                                duAnNguonVon = _iQLVonDauTuService.GetListDuAnNguonVonTTDuAn(duanExist.iID_DuAnID).ToList();
                                duAnHangMuc = _iQLVonDauTuService.GetListDuAnHangMucTTDuAn(duanExist.iID_DuAnID).ToList();
                                listCTDT_DMHangMuc_PendingSave = TaoMoiListChuTruong_DMHangMuc_From_VDT_DA_DuAn_HangMucModel(duAnHangMuc);
                                listCTDTHangMucPendingSave = TaoMoiListChuTruongHangMucFrom_VDT_DA_DuAn_HangMucModel(duAnHangMuc, listCTDT_ID, listCTDT_DMHangMuc_PendingSave);
                                listCTDTNguonVonPendingSave = TaoMoiListChuTruongNguonVonFrom_VDTDuAnListNguonVonTTDuAnModel(duAnNguonVon, listCTDT_ID);
                            }                            
                            // nếu dự án chưa được lưu DB -> tạo mới -> tạo chi tiết CTDT từ các chi tiết dự án tạo mới
                            else
                            {
                                VDT_DA_DuAn_HangMuc hmNew = new VDT_DA_DuAn_HangMuc();
                                hmNew.iID_DuAnID = listDuAnPendingSave.LastOrDefault().iID_DuAnID;
                                hmNew.iID_DuAn_HangMucID = Guid.NewGuid();
                                hmNew.sMaHangMuc = "000-000";
                                VDT_DM_LoaiCongTrinh lct1 = _iQLVonDauTuService.GetDMLoaiCongTrinhByMa(sMaLoaiCongTrinh);
                                hmNew.sTenHangMuc = sTen + " - " + lct1.sTenLoaiCongTrinh;
                                hmNew.fTienHangMuc = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                                hmNew.fHanMucDauTu = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                                hmNew.iID_NguonVonID = Int16.Parse(sMaNguonVon);
                                hmNew.iID_LoaiCongTrinhID = lct1.iID_LoaiCongTrinh;
                                listDuAnHangMucPendingSave.Add(hmNew);

                                VDT_DA_DuAn_NguonVon danv = new VDT_DA_DuAn_NguonVon();
                                danv.iID_DuAn = listDuAnPendingSave.LastOrDefault().iID_DuAnID;
                                danv.iID_NguonVonID = Int16.Parse(sMaNguonVon);
                                danv.fThanhTien = fHanMucDauTu != "" ? Double.Parse(fHanMucDauTu) : 0;
                                listDuAnNguonVonPendingSave.Add(danv);

                                listCTDT_DMHangMuc_PendingSave = TaoMoiListChuTruong_DMHangMuc_From_VDT_DA_DuAn_HangMuc(listDuAnHangMucPendingSave);
                                listCTDTHangMucPendingSave = TaoMoiListChuTruongHangMucFrom_VDT_DA_DuAn_HangMuc(listDuAnHangMucPendingSave, listCTDT_ID, listCTDT_DMHangMuc_PendingSave);
                                listCTDTNguonVonPendingSave = TaoMoiListChuTruongNguonVonFrom_VDT_DA_DuAn_NguonVon(listDuAnNguonVonPendingSave, listCTDT_ID);
                            }
                        }
                    }
                    NS_DonVi dv_ThucHienDuAn = _iQLVonDauTuService.GetNSDonViByMaDonVi(sMaDonVi_ThucHienDuAn);
                    VDT_DM_LoaiCongTrinh lct = _iQLVonDauTuService.GetDMLoaiCongTrinhByMa(sMaLoaiCongTrinh);
                    NS_NguonNganSach nv = sMaNguonVon != null ? _iQLVonDauTuService.GetNganSachByMa(sMaNguonVon) : null;
                    DM_ChuDauTu cdt = _danhMucService.GetChuDauTuByMa(sMaChuDauTu);
                    var e = new KeHoach5NamDeXuatDataImportModel
                    {
                        sSTT = sSTT,
                        sTen = sTen,
                        sTenDonViQL = dv_ThucHienDuAn != null ? dv_ThucHienDuAn.sTen : "", 
                        sDiaDiem = sDiaDiem,
                        iGiaiDoanTu = iGiaiDoanTu,
                        iGiaiDoanDen = iGiaiDoanDen,
                        sTenLoaiCongTrinh = sMaLoaiCongTrinh != null ? lct.sTenLoaiCongTrinh : "",
                        iID_LoaiCongTrinhID = sMaLoaiCongTrinh != null ? lct.iID_LoaiCongTrinh.ToString() : null,
                        sTenNganSach = sMaNguonVon != null ? nv.sTen : null,
                        fHanMucDauTu = fHanMucDauTu,
                        fTongSoNhuCauNSQP = fTongSoNhuCauNSQP,
                        fTongSo = fTongSo,
                        fGiaTriNamThuNhat = fGiaTriNamThuNhat,
                        fGiaTriNamThuHai = fGiaTriNamThuHai,
                        fGiaTriNamThuBa = fGiaTriNamThuBa,
                        fGiaTriNamThuTu = fGiaTriNamThuTu,
                        fGiaTriNamThuNam = fGiaTriNamThuNam,
                        fGiaTriBoTri = fGiaTriBoTri,
                        sGhiChu = sGhiChu,
                        sMaOrder = sMaOrder,
                        iIDReference = iIDReference,
                        iID_ParentID = iID_ParentID,
                        iID_KeHoach5Nam_DeXuat_ChiTietID = iID_KeHoach5Nam_DeXuat_ChiTietID,
                        bIsParent = bIsParent,
                        iLevel = iLevel,
                        iIndexCode = iIndexCode,
                        sMaLoaiCongTrinh = sMaLoaiCongTrinh,
                        iID_NguonVonID = sMaNguonVon != null ? nv.iID_MaNguonNganSach.ToString() : null,
                        iID_MaDonVi = dv_ThucHienDuAn != null ? dv_ThucHienDuAn.iID_MaDonVi.ToString() : null, // đơn vị thực hiện dự án
                        isMap = 1,
                        sTenChuDauTu = iLevel != "1" ? cdt.sTenCDT : "",
                        sMaChuDauTu = iLevel != "1" ? cdt.sId_CDT : "",
                        sMaDuAn = iLevel != "1" ? sMaDuAn : "",
                        sSoQuyetDinhCTDT = iLevel != "1" ? sSoQuyetDinhCTDT : ""
                    };
                    dataImport.Add(e);
                }
            }
            TempData["listDuAnPendingSave"] = listDuAnPendingSave;
            TempData["ctdtNewPendingSave"] = ctdtNewPendingSave;
            TempData["listDuAnHangMucPendingSave"] = listDuAnHangMucPendingSave;
            TempData["listDuAnNguonVonPendingSave"] = listDuAnNguonVonPendingSave;
            TempData["listCTDTHangMucPendingSave"] = listCTDTHangMucPendingSave;
            TempData["listCTDT_DMHangMuc_PendingSave"] = listCTDT_DMHangMuc_PendingSave;
            TempData["listCTDTNguonVonPendingSave"] = listCTDTNguonVonPendingSave;
            return dataImport.AsEnumerable();
        }

        public ActionResult SheetFrameImport(string id, string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            List<KeHoach5NamDeXuatDataImportModel> dataImport;
            // nếu mới import mà chưa lưu thì lưu lại tempdata, nếu đã lưu rồi -> tempdata clear, thì lấy dữ liệu lại từ DB
            if (TempData["dataImport"] != null)
            {
                dataImport = (List<KeHoach5NamDeXuatDataImportModel>)TempData["dataImport"];
            }
            else
            {
                if (TempData["VDT_KHV_KeHoach5Nam_DeXuat"] != null)
                {
                    IEnumerable<VdtKhvKeHoachTrungHanDeXuatChiTietModel> dataValue = _iQLVonDauTuService.GetAllKH5NamDeXuatChiTiet(((VDT_KHV_KeHoach5Nam_DeXuat)TempData["VDT_KHV_KeHoach5Nam_DeXuat"]).iID_KeHoach5Nam_DeXuatID);
                    dataImport = new List<KeHoach5NamDeXuatDataImportModel>();
                    foreach (var data in dataValue)
                    {
                        var donViQL = data.iID_MaDonVi != null ? _iNganSachService.GetDonViThucHienDuAnByMa(data.iID_MaDonVi.ToString()) : null;
                        var loaiCongTrinh = data.iID_LoaiCongTrinhID.HasValue ? _iQLVonDauTuService.GetDMLoaiCongTrinhById(data.iID_LoaiCongTrinhID.Value) : null;
                        var nganSach = data.iID_NguonVonID != null ? _iQLVonDauTuService.GetNganSachByMa(data.iID_NguonVonID.ToString()) : null;
                        var e = new KeHoach5NamDeXuatDataImportModel
                        {
                            //new KeHoach5NamDeXuatDataImportModel()
                            //{
                            sSTT = data.sSTT,
                            sTen = data.sTen,
                            sTenDonViQL = donViQL != null ? donViQL.sTenDonVi : "",
                            sDiaDiem = data.sDiaDiem,
                            iGiaiDoanTu = data.iGiaiDoanTu != 0 ? data.iGiaiDoanTu.ToString() : "",
                            iGiaiDoanDen = data.iGiaiDoanDen != 0 ? data.iGiaiDoanDen.ToString() : "",
                            sDuAnCha = "",
                            //sTenLoaiCongTrinh = "",
                            sTenLoaiCongTrinh = loaiCongTrinh != null ? loaiCongTrinh.sTenLoaiCongTrinh : "",
                            sTenNganSach = nganSach != null ? nganSach.sTen : "",
                            fHanMucDauTu = data.fHanMucDauTu.ToString(),
                            fTongSoNhuCauNSQP = "",
                            fTongSo = "",
                            fGiaTriNamThuNhat = data.fGiaTriNamThuNhat.ToString(),
                            fGiaTriNamThuHai = data.fGiaTriNamThuHai.ToString(),
                            fGiaTriNamThuBa = data.fGiaTriNamThuBa.ToString(),
                            fGiaTriNamThuTu = data.fGiaTriNamThuTu.ToString(),
                            fGiaTriNamThuNam = data.fGiaTriNamThuNam.ToString(),
                            fGiaTriBoTri = data.fGiaTriBoTri.ToString(),
                            sGhiChu = data.sGhiChu,
                            iID_KeHoach5Nam_DeXuat_ChiTietID = data.iID_KeHoach5Nam_DeXuat_ChiTietID.ToString(),
                            iID_ParentID = data.iID_ParentID.ToString(),
                            iID_KeHoach5Nam_DeXuatID = data.iID_KeHoach5Nam_DeXuatID.ToString(),
                            iID_DonViQuanLyID = data.iID_DonViQuanLyID.ToString(),
                            iID_LoaiCongTrinhID = data.iID_LoaiCongTrinhID.ToString(),
                            iID_NguonVonID = data.iID_NguonVonID.ToString(),
                            numChild = "",
                            iIDReference = data.iIDReference.ToString(),
                            iLevel = data.iLevel.ToString(),
                            sId_CDT = "",
                            iID_MaDonVi = data.iID_MaDonVi,
                            isMap = 1,
                            bIsParent = (bool)data.bIsParent ? 1 : 0,
                            sMaOrder = data.sMaOrder,
                            iIndexCode = data.iIndexCode.ToString(),
                            sMaLoaiCongTrinh = ""
                            //}
                        };

                        dataImport.Add(e);
                    }

                }
                else
                    dataImport = new List<KeHoach5NamDeXuatDataImportModel>();
            }
            //IEnumerable<KeHoach5NamDeXuatDataImportModel> dataImport = (IEnumerable<KeHoach5NamDeXuatDataImportModel>)TempData["dataImport"];
            /*if (dataImport == null)
            {
                dataImport = new List<KeHoach5NamDeXuatDataImportModel>().AsEnumerable();
            }*/
            var iGiaiDoanTu = Int16.Parse(id);
            var datatableData = ToDataTable(dataImport.AsEnumerable());
            var sheet = new KeHoach5NamDeXuat_Import_SheetTable(datatableData, int.Parse(PhienLamViec.iNamLamViec), iGiaiDoanTu, filters);
            var vm = new KeHoach5NamDeXuatChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("SaveImport", "KeHoachTrungHanDeXuat", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrameImport", "KeHoachTrungHanDeXuat", new { area = "QLVonDauTu" })
                   ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrameImport", vm);
        }

        public DataTable ToDataTable<KeHoach5NamDeXuatDataImportModel>(IEnumerable<KeHoach5NamDeXuatDataImportModel> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(KeHoach5NamDeXuatDataImportModel));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (KeHoach5NamDeXuatDataImportModel item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        private void SaveAllListPendingSave(Guid iID_DonViQuanLyID)
        {
            try
            { // lấy các list đang pending save để lưu
                List<VDT_DA_DuAn> listDuAnPendingSave = (List<VDT_DA_DuAn>)TempData["listDuAnPendingSave"];
                List<VDT_DA_ChuTruongDauTu> ctdtNewPendingSave = (List<VDT_DA_ChuTruongDauTu>)TempData["ctdtNewPendingSave"];
                List<VDT_DA_DuAn_HangMuc> listDuAnHangMucPendingSave = (List<VDT_DA_DuAn_HangMuc>)TempData["listDuAnHangMucPendingSave"];
                List<VDT_DA_DuAn_NguonVon> listDuAnNguonVonPendingSave = (List<VDT_DA_DuAn_NguonVon>)TempData["listDuAnNguonVonPendingSave"];
                List<VDT_DA_ChuTruongDauTu_HangMuc> listCTDTHangMucPendingSave = (List<VDT_DA_ChuTruongDauTu_HangMuc>)TempData["listCTDTHangMucPendingSave"];
                List<VDT_DA_ChuTruongDauTu_DM_HangMuc> listCTDT_DMHangMuc_PendingSave = (List<VDT_DA_ChuTruongDauTu_DM_HangMuc>)TempData["listCTDT_DMHangMuc_PendingSave"];
                List<VDT_DA_ChuTruongDauTu_NguonVon> listCTDTNguonVonPendingSave = (List<VDT_DA_ChuTruongDauTu_NguonVon>)TempData["listCTDTNguonVonPendingSave"];

                NS_DonVi dvql = _danhMucService.GetNSDonViById(iID_DonViQuanLyID);

                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    listDuAnPendingSave.ForEach(i =>
                    {
                        i.iID_DonViQuanLyID = iID_DonViQuanLyID;
                        i.iID_MaDonVi = dvql.iID_MaDonVi;
                        conn.Insert<VDT_DA_DuAn>(i, trans);
                    });
                    ctdtNewPendingSave.ForEach(i =>
                    {
                        i.iID_DonViQuanLyID = iID_DonViQuanLyID;
                        i.iID_MaDonViQuanLy = dvql.iID_MaDonVi;
                        conn.Insert<VDT_DA_ChuTruongDauTu>(i, trans);
                    });
                    listDuAnNguonVonPendingSave.ForEach(i =>
                    {
                        conn.Insert<VDT_DA_DuAn_NguonVon>(i, trans);
                    });
                    listDuAnHangMucPendingSave.ForEach(i =>
                    {
                        conn.Insert<VDT_DA_DuAn_HangMuc>(i, trans);
                    });
                    listCTDT_DMHangMuc_PendingSave.ForEach(i =>
                    {
                        conn.Insert<VDT_DA_ChuTruongDauTu_DM_HangMuc>(i, trans);
                    });
                    listCTDTNguonVonPendingSave.ForEach(i =>
                    {
                        conn.Insert<VDT_DA_ChuTruongDauTu_NguonVon>(i, trans);
                    });
                    listCTDTHangMucPendingSave.ForEach(i =>
                    {
                        conn.Insert<VDT_DA_ChuTruongDauTu_HangMuc>(i, trans);
                    });
                    trans.Commit();
                }
            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SaveImport(SheetEditViewModel vm)
        {
            try
            {
                #region save VDT_KHV_KeHoach5Nam_DeXuat
                VDT_KHV_KeHoach5Nam_DeXuat data = (VDT_KHV_KeHoach5Nam_DeXuat)TempData["VDT_KHV_KeHoach5Nam_DeXuat"];
                #endregion

                #region import crud
                if (data.bIsGoc.Value)
                {
                    // Tạo kế hoạch 5 năm nếu chưa có
                    var entityKH5NDX = new VDT_KHV_KeHoach5Nam_DeXuat();
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();
                        var trans = conn.BeginTransaction();
                        if (data.iID_KeHoach5Nam_DeXuatID == null || data.iID_KeHoach5Nam_DeXuatID == Guid.Empty)
                        {
                            entityKH5NDX.MapFrom(data);
                            entityKH5NDX.dNgayQuyetDinh = DateTime.Now;
                            entityKH5NDX.bActive = true;
                            entityKH5NDX.bIsGoc = true;
                            entityKH5NDX.iID_ParentId = null;
                            entityKH5NDX.iNamLamViec = PhienLamViec.NamLamViec;
                            entityKH5NDX.sUserCreate = Username;
                            entityKH5NDX.dDateCreate = DateTime.Now;
                            var returnData = conn.Insert(entityKH5NDX, trans);
                            ((VDT_KHV_KeHoach5Nam_DeXuat)TempData["VDT_KHV_KeHoach5Nam_DeXuat"]).iID_KeHoach5Nam_DeXuatID = returnData;
                        }
                        // commit to db
                        trans.Commit();
                    }

                    SaveAllListPendingSave(data.iID_DonViQuanLyID);

                    // lưu giá trị các dòng chi tiết của kế hoạch 5 năm
                    var rows = vm.Rows.ToList();
                    if (rows.Count > 0)
                    {
                        using (var conn = ConnectionFactory.Default.GetConnection())
                        {
                            conn.Open();
                            var columns = new KeHoach5NamDeXuat_Import_SheetTable().Columns.Where(x => !x.IsReadonly);
                            //var columns = new KeHoach5NamDeXuat_Import_SheetTable().Columns;
                            var mapParentId = new Dictionary<string, string>();
                            rows.ForEach(r =>
                            {
                                var trans = conn.BeginTransaction();
                                var iID_KeHoach5Nam_DeXuat_ChiTietID = r.Id;
                                var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                                if (changes.Any())
                                {
                                    string sMaDonVi = "";
                                    int? iID_NguonVonID = 0;
                                    string sMaLoaiCongTrinh = "";
                                    NS_DonVi donvi = null;
                                    //VDT_DM_DonViThucHienDuAn donvi = null;
                                    VDT_DM_LoaiCongTrinh congtrinh = null;
                                    NS_NguonNganSach nS_NguonNganSach = null;
                                    int iLevel = 0;
                                    if (r.Columns.ContainsKey("iID_MaDonVi"))
                                    {
                                        sMaDonVi = r.Columns["iID_MaDonVi"];
                                        //donvi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, sMaDonVi);
                                        if (!String.IsNullOrEmpty(sMaDonVi))
                                        {
                                            donvi = _iNganSachService.GetDonViByMaDonVi(PhienLamViec.iNamLamViec, sMaDonVi);
                                        }
                                        else
                                        {
                                            donvi = null;
                                        }                                        
                                    }
                                    if (r.Columns.ContainsKey("sMaLoaiCongTrinh"))
                                    {
                                        sMaLoaiCongTrinh = r.Columns["sMaLoaiCongTrinh"];
                                        if (!String.IsNullOrEmpty(sMaLoaiCongTrinh))
                                        {
                                            congtrinh = _iQLVonDauTuService.GetDMLoaiCongTrinhByMa(sMaLoaiCongTrinh);
                                        }
                                        else
                                        {
                                            congtrinh = null;
                                        }                                   
                                    }
                                    if (r.Columns.ContainsKey("iID_NguonVonID"))
                                    {
                                        /*if (!string.IsNullOrEmpty(r.Columns["iID_NguonVonID"]))
                                        {
                                            string strId = r.Columns["iID_NguonVonID"];
                                            iID_NguonVonID = Int16.Parse(strId);
                                        }*/
                                        string strId = r.Columns["iID_NguonVonID"];
                                        if (!String.IsNullOrEmpty(strId))
                                        {
                                            nS_NguonNganSach = _iQLVonDauTuService.GetNganSachByMa(strId);
                                        }
                                        else
                                        {
                                            nS_NguonNganSach = null;
                                        }                                            
                                    }
                                    if (r.Columns.ContainsKey("iLevel"))
                                    {
                                        iLevel = Int16.Parse(r.Columns["iLevel"]);
                                    }

                                    #region create
                                    var entityChiTiet = new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet()
                                    {
                                        bIsParent = false
                                    };
                                    entityChiTiet.MapFrom(changes);
                                    if (r.Columns.ContainsKey("iID_MaDonVi"))
                                    {
                                        entityChiTiet.iID_DonViQuanLyID = ((VDT_KHV_KeHoach5Nam_DeXuat)TempData["VDT_KHV_KeHoach5Nam_DeXuat"]).iID_DonViQuanLyID;
                                        entityChiTiet.iID_MaDonVi = donvi?.iID_MaDonVi;
                                        entityChiTiet.iID_DonViID = donvi?.iID_Ma;
                                    }
                                    if (r.Columns.ContainsKey("sMaLoaiCongTrinh"))
                                    {
                                        entityChiTiet.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                    }
                                    if (r.Columns.ContainsKey("iID_NguonVonID"))
                                    {
                                        //entityChiTiet.iID_NguonVonID = iID_NguonVonID > 0 ? iID_NguonVonID : null;
                                        entityChiTiet.iID_NguonVonID = nS_NguonNganSach?.iID_MaNguonNganSach;
                                    }
                                    if (r.Columns.ContainsKey("iID_ParentID") && !string.IsNullOrEmpty(r.Columns["iID_ParentID"]))
                                    {
                                        entityChiTiet.iID_ParentID = Guid.Parse(r.Columns["iID_ParentID"]);
                                    }

                                    entityChiTiet.iID_KeHoach5Nam_DeXuatID = entityKH5NDX.iID_KeHoach5Nam_DeXuatID;
                                    string mapValue;
                                    if (mapParentId.TryGetValue(entityChiTiet.iID_ParentID.ToString(), out mapValue))
                                    {
                                        Console.WriteLine(mapValue);
                                        entityChiTiet.iID_ParentID = Guid.Parse(mapValue);
                                    }
                                    if (iLevel == 3)
                                    {
                                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet KH5NDXChiTiettCha = _iQLVonDauTuService.GetKH5NamDeXuatChiTietById(entityChiTiet.iID_ParentID);
                                        entityChiTiet.iIDReference = KH5NDXChiTiettCha.iIDReference;
                                    }

                                    conn.Insert(entityChiTiet, trans);
                                    mapParentId.Add(iID_KeHoach5Nam_DeXuat_ChiTietID, entityChiTiet.iID_KeHoach5Nam_DeXuat_ChiTietID.ToString());

                                    if (iLevel == 2)
                                    {
                                        entityChiTiet.iIDReference = entityChiTiet.iID_KeHoach5Nam_DeXuat_ChiTietID;
                                        conn.Update(entityChiTiet, trans);
                                    }
                                    #endregion
                                }
                                // commit to db
                                trans.Commit();
                            });

                        }
                    }

                    //update isparent
                    _iQLVonDauTuService.UpdateIsParent(entityKH5NDX.iID_KeHoach5Nam_DeXuatID.ToString());
                    //update fGiaTriKeHoach
                    _iQLVonDauTuService.UpdateGiaTriKeHoach(entityKH5NDX.iID_KeHoach5Nam_DeXuatID.ToString());
                }
                else
                {
                    var entityKH5NDX = new VDT_KHV_KeHoach5Nam_DeXuat();
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();
                        var trans = conn.BeginTransaction();
                        // Insert New Record
                        entityKH5NDX.MapFrom(data);
                        entityKH5NDX.dNgayQuyetDinh = DateTime.Now;
                        entityKH5NDX.bActive = true;
                        entityKH5NDX.bIsGoc = false;
                        entityKH5NDX.iNamLamViec = PhienLamViec.NamLamViec;
                        entityKH5NDX.sUserCreate = Username;
                        entityKH5NDX.dDateCreate = DateTime.Now;
                        var returnData = conn.Insert(entityKH5NDX, trans);
                        ((VDT_KHV_KeHoach5Nam_DeXuat)TempData["VDT_KHV_KeHoach5Nam_DeXuat"]).iID_KeHoach5Nam_DeXuatID = returnData;

                        // select lại bản ghi đề xuất cũ và update lại active = false
                        if (data.iID_ParentId != null && data.iID_ParentId != Guid.Empty)
                        {
                            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(data.iID_ParentId);
                            if (itemQuery != null)
                            {
                                itemQuery.bActive = false;
                            }
                            //conn.Update(itemQuery, trans);

                            //lưu các giá trị điều chỉnh
                            List<VdtKhvKeHoachTrungHanDeXuatChiTietModel> lstDetail = _iQLVonDauTuService.GetAllKH5NamDeXuatDieuChinhChiTiet(data.iID_ParentId.Value).ToList();
                            conn.Update(itemQuery, trans);
                            if (lstDetail != null && lstDetail.Count() > 0)
                            {
                                lstDetail = lstDetail.Select(x =>
                                {
                                    x.IdParentNew = x.iID_KeHoach5Nam_DeXuat_ChiTietID;
                                    x.iID_ParentModified = x.iID_KeHoach5Nam_DeXuat_ChiTietID;
                                    x.iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.NewGuid();
                                    x.iID_KeHoach5Nam_DeXuatID = entityKH5NDX.iID_KeHoach5Nam_DeXuatID;
                                    x.sGhiChu = string.Empty;
                                    return x;
                                }).OrderBy(x => x.sMaOrder).ToList();

                                var refDictionary = lstDetail.ToDictionary(x => x.IdParentNew, x => x.iID_KeHoach5Nam_DeXuat_ChiTietID);

                                lstDetail.Select(item =>
                                {
                                    // Cập nhật IdReference, IdParent
                                    if (item.iIDReference != null) item.iIDReference = refDictionary[item.iIDReference];
                                    //if (item.iID_ParentID != null) item.iID_ParentID = refDictionary[item.iID_ParentID];          // khi duyệt để điều chỉnh sẽ không còn quan hệ cha con trong chi tiết KHTH
                                    item.iID_ParentID = null;                                                                       // nên iID_ParentID của các chi tiết trong KHTH điều chỉnh sẽ = null

                                    return item;
                                }).ToList();
                                data = entityKH5NDX;

                                var rows = vm.Rows.ToList();
                                int count = 0;
                                if (rows.Count > 0)
                                {
                                    rows.ForEach(r =>
                                    {
                                        lstDetail.ForEach(l =>
                                        {
                                            // && r.Columns["iID_MaDonVi"] == l.iID_DonViQuanLyID.ToString() trong ham if dang luu k dung MaDonViQuanLy                                            
                                            if (r.Columns["sTen"] == l.sTen && r.Columns["sDiaDiem"] == l.sDiaDiem
                                            && r.Columns["iGiaiDoanTu"] == l.iGiaiDoanTu.ToString() && r.Columns["iGiaiDoanDen"] == l.iGiaiDoanDen.ToString()
                                            && r.Columns["sMaLoaiCongTrinh"] == l.iID_LoaiCongTrinhID.ToString().ToUpper() && r.Columns["iID_NguonVonID"] == l.iID_NguonVonID.ToString()
                                            && r.Columns["fHanMucDauTu"] == l.fHanMucDauTu.ToString())
                                            {
                                                var columns = new KeHoach5NamDeXuat_Import_SheetTable().Columns.Where(x => !x.IsReadonly);
                                                var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                                                l.MapFrom(changes);
                                                count++;
                                            }
                                        });
                                    });
                                }
                                if (count == 0)
                                {
                                    TempData["MessageError"] = "Import file error";
                                    return RedirectToAction("SheetFrameImport", new { vm.Id, vm.Option, filters = vm.FiltersString });
                                }
                                //conn.Insert(lstDetail, trans);
                                conn.Insert<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(lstDetail, trans);
                            }
                            // lưu các đề xuất chi tiết vừa được import, và map trường id đề xuất bằng với returnData (chính là id của đề xuất vừa được thêm)
                            /*var rows = vm.Rows.ToList();
                            if (rows.Count > 0)
                            {
                                var columns = new KeHoach5NamDeXuat_Import_SheetTable().Columns.Where(x => !x.IsReadonly);
                                var mapParentId = new Dictionary<string, string>();
                                rows.ForEach(r =>
                                {
                                    var iID_KeHoach5Nam_DeXuat_ChiTietID = r.Id;
                                    var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                                    if (changes.Any())
                                    {
                                        string sMaDonVi = "";
                                        int? iID_NguonVonID = 0;
                                        string sMaLoaiCongTrinh = "";
                                        //NS_DonVi donvi = null;
                                        VDT_DM_DonViThucHienDuAn donvi = null;
                                        VDT_DM_LoaiCongTrinh congtrinh = null;
                                        NS_NguonNganSach nS_NguonNganSach = null;
                                        int iLevel = 0;
                                    }
                                });
                            }*/
                        }


                        // lưu các đề xuất chi tiết vừa được import, và map trường id đề xuất bằng với returnData (chính là id của đề xuất vừa được thêm)                           


                        /*var rows = vm.Rows.ToList();
                        if (rows.Count > 0)
                        {                                    
                                var columns = new KeHoach5NamDeXuat_Import_SheetTable().Columns.Where(x => !x.IsReadonly);
                                //var columns = new KeHoach5NamDeXuat_Import_SheetTable().Columns;
                                var mapParentId = new Dictionary<string, string>();
                            rows.ForEach(r =>
                            {
                                var iID_KeHoach5Nam_DeXuat_ChiTietID = r.Id;
                                var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                                if (changes.Any())
                                {
                                    string sMaDonVi = "";
                                    int? iID_NguonVonID = 0;
                                    string sMaLoaiCongTrinh = "";
                                    //NS_DonVi donvi = null;
                                    VDT_DM_DonViThucHienDuAn donvi = null;
                                    VDT_DM_LoaiCongTrinh congtrinh = null;
                                    NS_NguonNganSach nS_NguonNganSach = null;
                                    int iLevel = 0;
                                    if (r.Columns.ContainsKey("iID_MaDonVi"))
                                    {
                                        sMaDonVi = r.Columns["iID_MaDonVi"];
                                        //donvi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, sMaDonVi);
                                        donvi = _iNganSachService.GetDonViThucHienDuAnByID(sMaDonVi);
                                    }
                                    if (r.Columns.ContainsKey("sMaLoaiCongTrinh"))
                                    {
                                        sMaLoaiCongTrinh = r.Columns["sMaLoaiCongTrinh"];
                                        congtrinh = _iQLVonDauTuService.GetDMLoaiCongTrinhById(Guid.Parse(sMaLoaiCongTrinh));
                                    }
                                    if (r.Columns.ContainsKey("iID_NguonVonID"))
                                    {
                                        *//*if (!string.IsNullOrEmpty(r.Columns["iID_NguonVonID"]))
                                        {
                                            string strId = r.Columns["iID_NguonVonID"];
                                            iID_NguonVonID = Int16.Parse(strId);
                                        }*//*
                                        if (!string.IsNullOrEmpty(r.Columns["iID_NguonVonID"]))
                                        {
                                            string strId = r.Columns["iID_NguonVonID"];
                                            nS_NguonNganSach = _iQLVonDauTuService.GetNganSachByMa(strId);
                                        }
                                    }
                                    if (r.Columns.ContainsKey("iLevel"))
                                    {
                                        iLevel = Int16.Parse(r.Columns["iLevel"]);
                                    }

                                    #region create
                                    var entityChiTiet = new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet()
                                    {
                                        bIsParent = false
                                    };
                                    entityChiTiet.MapFrom(changes);
                                    if (r.Columns.ContainsKey("iID_MaDonVi"))
                                    {
                                        entityChiTiet.iID_DonViQuanLyID = ((VDT_KHV_KeHoach5Nam_DeXuat)TempData["VDT_KHV_KeHoach5Nam_DeXuat"]).iID_DonViQuanLyID;
                                        entityChiTiet.iID_MaDonVi = donvi?.iID_MaDonVi;
                                    }
                                    if (r.Columns.ContainsKey("sMaLoaiCongTrinh"))
                                    {
                                        entityChiTiet.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                    }
                                    if (r.Columns.ContainsKey("iID_NguonVonID"))
                                    {
                                        //entityChiTiet.iID_NguonVonID = iID_NguonVonID > 0 ? iID_NguonVonID : null;
                                        entityChiTiet.iID_NguonVonID = nS_NguonNganSach?.iID_MaNguonNganSach;
                                    }
                                    if (r.Columns.ContainsKey("iID_ParentID") && !string.IsNullOrEmpty(r.Columns["iID_ParentID"]))
                                    {
                                        entityChiTiet.iID_ParentID = Guid.Parse(r.Columns["iID_ParentID"]);
                                    }

                                    entityChiTiet.iID_KeHoach5Nam_DeXuatID = entityKH5NDX.iID_KeHoach5Nam_DeXuatID;
                                    string mapValue;
                                    if (mapParentId.TryGetValue(entityChiTiet.iID_ParentID.ToString(), out mapValue))
                                    {
                                        Console.WriteLine(mapValue);
                                        entityChiTiet.iID_ParentID = Guid.Parse(mapValue);
                                    }
                                    if (iLevel == 3)
                                    {
                                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet KH5NDXChiTiettCha = _iQLVonDauTuService.GetKH5NamDeXuatChiTietById(entityChiTiet.iID_ParentID);
                                        entityChiTiet.iIDReference = KH5NDXChiTiettCha.iIDReference;
                                    }
                                    entityChiTiet.iID_KeHoach5Nam_DeXuatID = (Guid)returnData;
                                    conn.Insert(entityChiTiet, trans);
                                    mapParentId.Add(iID_KeHoach5Nam_DeXuat_ChiTietID, entityChiTiet.iID_KeHoach5Nam_DeXuat_ChiTietID.ToString());

                                    if (iLevel == 2)
                                    {
                                        entityChiTiet.iIDReference = entityChiTiet.iID_KeHoach5Nam_DeXuat_ChiTietID;
                                        conn.Update(entityChiTiet, trans);
                                    }
                                    #endregion
                                }
                            });

                        }
                    }*/
                        trans.Commit();
                    }
                }

                #endregion
                // clear cache
                TempData["MessageSave"] = "Import file successfuly";
                TempData["dataImport"] = null; // chỉ được assign khi đã lưu 
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return RedirectToAction("SheetFrameImport", new { vm.Id, vm.Option, filters = vm.FiltersString });
        }

        [HttpPost]
        public JsonResult KH5NDXPreSaveImport(VDT_KHV_KeHoach5Nam_DeXuat data)
        {
            try
            {
                if (_iQLVonDauTuService.CheckExistSoKeHoach(data.sSoQuyetDinh, PhienLamViec.NamLamViec, null))
                {
                    return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                }
                TempData["VDT_KHV_KeHoach5Nam_DeXuat"] = data;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadImportExample()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/importExp.xlsx"));
            string fileName = "FileImportExp.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public FileResult DownloadImportExample2()
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/importExp.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            List<NS_DonVi> lstDV = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            List<NS_NguonNganSach> lstNv = _iQLVonDauTuService.LayNguonVon().ToList();
            List<VDT_DM_LoaiCongTrinh> lstLct = _iQLVonDauTuService.GetAllDmLoaiCongTrinh().ToList();
            fr.AddTable("dv", lstDV);
            fr.AddTable("nv", lstNv);
            fr.AddTable("lct", lstLct);
            fr.Run(Result);
            ExcelFile file = Result;
            string sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string sFileName = "KeHoachTrungHanDeXuatImportExample.xlsx";
            using (MemoryStream stream = new MemoryStream())
            {
                file.Save(stream);
                return File(stream.ToArray(), sContentType, sFileName);
            }
        }

        public ActionResult ViewInBaoCao(bool isModified, bool isCt)
        {
            KH5NDXPrintDataExportModel vm = new KH5NDXPrintDataExportModel();

            IEnumerable<NS_NguonNganSach> lstDMNguonNganSach = _iQLVonDauTuService.GetListDMNguonNganSach();
            vm.lstDMNguonNganSach = lstDMNguonNganSach;
            IEnumerable<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec);
            IEnumerable<VDT_DM_DonViThucHienDuAn> lstDonViThucHienDuAn = _danhMucService.GetListDonViThucHienDuAn();
            vm.lstDonViThucHienDuAn = lstDonViThucHienDuAn;
            vm.lstDonViQuanLy = lstDonViQuanLy;
            //vm.iGiaiDoanTu = DateTime.Now.Year;
            //vm.iGiaiDoanDen = DateTime.Now.Year + 4;
            var iGiaiDoanTu = _iQLVonDauTuService.FindCurrentPeriod(PhienLamViec.NamLamViec);
            vm.iGiaiDoanTu =  iGiaiDoanTu;
            vm.iGiaiDoanDen = iGiaiDoanTu + 4;

            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");

            List<VDT_DM_LoaiCongTrinh> lstLoaiCongTrinh = _iQLVonDauTuService.GetListParentDMLoaiCongTrinh();
            lstLoaiCongTrinh.Insert(0, new VDT_DM_LoaiCongTrinh { iID_LoaiCongTrinh = Guid.Empty, sTenLoaiCongTrinh = "--Tất cả--" });
            ViewBag.ListLoaiCongTrinh = lstLoaiCongTrinh.ToSelectList("iID_LoaiCongTrinh", "sTenLoaiCongTrinh");
            ViewBag.LstDonViTinh = LoadDonViTinh().ToSelectList("ValueItem", "DisplayItem");
            if (isModified && !isCt)
            {
                ViewBag.TitleDx = "In báo cáo kế hoạch trung hạn đề xuất(Điều chỉnh)";
                ViewBag.TitleHeader1 = "DỰ KIẾN KẾ HOẠCH ĐẦU TƯ TRUNG HẠN NGUỒN NSQP GIAI ĐOẠN " + iGiaiDoanTu + " - " + (iGiaiDoanTu + 4) + " (CÔNG TRÌNH PHỔ THÔNG)";
                ViewBag.TitleHeader2 = "(Công trình mở mới)";
                ViewBag.isModified = "true";
                ViewBag.isCt = "false";
            }
            else if (!isModified && isCt)
            {
                ViewBag.TitleDx = "In báo cáo kế hoạch trung hạn đề xuất(Chuyển tiếp)";
                ViewBag.TitleHeader1 = "KẾ HOẠCH ĐẦU TƯ TRUNG HẠN NGUỒN NSQP NĂM "+ iGiaiDoanTu +" - "+ (iGiaiDoanTu+4) + " (CÔNG TRÌNH PHỔ THÔNG)";
                ViewBag.TitleHeader2 = "(Công trình chuyển tiếp)";
                ViewBag.isModified = "false";
                ViewBag.isCt = "true";
            }
            else
            {
                ViewBag.TitleDx = "In báo cáo kế hoạch trung hạn đề xuất";
                ViewBag.TitleHeader1 = "DỰ KIẾN KẾ HOẠCH ĐẦU TƯ TRUNG HẠN NGUỒN NSQP GIAI ĐOẠN "+ iGiaiDoanTu +" - "+ (iGiaiDoanTu+4) + " (CÔNG TRÌNH PHỔ THÔNG)";
                ViewBag.TitleHeader2 = "(Công trình mở mới)";
                ViewBag.isModified = "false";
                ViewBag.isCt = "false";
            }

            return View(vm);
        }

        public JsonResult LayDanhSachChungTuDeXuatTheoDonViQuanLy(string iID_DonViQuanLyID, int iGiaiDoanTu, int iGiaiDoanden, bool isModified, bool isCt)
        {
            List<VDT_KHV_KeHoach5Nam_DeXuat> lstChungTuDeXuat = _iQLVonDauTuService.GetLstChungTuDeXuat(Guid.Parse(iID_DonViQuanLyID), iGiaiDoanTu, iGiaiDoanden, PhienLamViec.NamLamViec, isModified, isCt).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", "", Constants.CHON);
            if (lstChungTuDeXuat != null && lstChungTuDeXuat.Count > 0)
            {
                for (int i = 0; i < lstChungTuDeXuat.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}'>{1}</option>", lstChungTuDeXuat[i].iID_KeHoach5Nam_DeXuatID, lstChungTuDeXuat[i].sSoQuyetDinh);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayGiaTriThayDoiTheoChungTuDuocChon(Guid? iID_KeHoach5Nam_DeXuatID)
        {
            var item = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(iID_KeHoach5Nam_DeXuatID);
            NS_DonVi donvi = _iQLVonDauTuService.GetDonViQuanLyById(item.iID_DonViQuanLyID);                        
            return Json(new { iIdMaDonVi = donvi.iID_Ma, iGiaiDoanTu = item.iGiaiDoanTu, iGiaiDoanDen = item.iGiaiDoanDen, iLoai = item.iLoai}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool ExportBCTheoDonVi(KH5NDXPrintDataExportModel data, List<string> arrIdNguonVon, List<string> arrIdDVQL, bool isModified, bool isCt)
        {
            try
            {
                TempData["lstDonViQL"] = arrIdDVQL;

                List<int> arrYearStartAndYearEnd = new List<int>();
                arrYearStartAndYearEnd.Add(data.iGiaiDoanTu);
                arrYearStartAndYearEnd.Add(data.iGiaiDoanDen);
                TempData["arrYearStartAndYearEnd"] = arrYearStartAndYearEnd;
                string lstDonVi = string.Empty;
                if (arrIdDVQL != null)
                {
                    lstDonVi = string.Join(",", arrIdDVQL);
                }
                //Lay list Chứng từ theo giai đoạn và danh sách đơn vị
                var listID_KeHoach5Nam_DeXuatID = _iQLVonDauTuService.GetLstIdChungTuDeXuat(data.iGiaiDoanTu, data.iGiaiDoanDen, isModified, isCt, PhienLamViec.NamLamViec, lstDonVi);

                if (listID_KeHoach5Nam_DeXuatID == null || listID_KeHoach5Nam_DeXuatID.Count() == 0)
                {
                    return false;
                }

                data.isTongHop = false;

                string strIds_KeHoach5Nam_DeXuat = string.Join(",", listID_KeHoach5Nam_DeXuatID);
                string iID_NguonVonIDs = string.Join(",", arrIdNguonVon);               

                if (isModified && !isCt)
                {
                    List<KH5NDXPrintDataDieuChinhExportModel> dataReport = _iQLVonDauTuService.FindSuggestionDcReport(2, strIds_KeHoach5Nam_DeXuat, lstDonVi, data.fDonViTinh.Value).ToList();
                    dataReport = HandleDataDeXuatDieuChinh(dataReport, isTongHop: true);
                    TempData["dataReportDc"] = dataReport;
                }
                else if (!isModified && isCt)
                {
                  //  List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> dataReport = _iQLVonDauTuService.FindByReportKeHoachTrungHanDeXuatChuyenTiep(strIds_KeHoach5Nam_DeXuat, iID_NguonVonIDs, data.iID_LoaiCongTrinh, lstDonVi, 2, data.fDonViTinh.Value).ToList();
                    List<KH5NDXPrintDataExportNewModel> dataReport = _iQLVonDauTuService.ReportKeHoachTrungHanDeXuatChuyenTiepNew(strIds_KeHoach5Nam_DeXuat, iID_NguonVonIDs, data.iID_LoaiCongTrinh, 1, data.fDonViTinh.Value).ToList();

                    //dataReport = CalculateDataReportDeXuatChuyenTiepDonVi(dataReport);
                    //dataReport = HandleDataReportDeXuatChuyenTiepDonVi(dataReport);
                    dataReport = HandleDataReportDeXuatChuyenTiepDonViNew(dataReport);
                    TempData["dataReportCt"] = dataReport;
                }
                else
                {
                   // List<KH5NDXPrintDataExportModel> dataReport = _iQLVonDauTuService.GetDataReportKH5NDeXuat(strIds_KeHoach5Nam_DeXuat, data.iID_LoaiCongTrinh.ToString(), iID_NguonVonIDs, lstDonVi, 2, data.fDonViTinh.Value, PhienLamViec.NamLamViec).ToList();
                    List<KH5NDXPrintDataExportModel> dataReport = _iQLVonDauTuService.GetDataReportKH5NDeXuatNew(strIds_KeHoach5Nam_DeXuat, data.iID_LoaiCongTrinh.ToString(), iID_NguonVonIDs, data.fDonViTinh.Value, PhienLamViec.NamLamViec,1).ToList();

                   // dataReport = HandleDataHanMucDauTu(dataReport);
                    //dataReport = CalculateDataReportDeXuatDonVi(dataReport);
                    //dataReport = HandleDataReportDeXuatDonVi(dataReport);
                    dataReport = HandleDataReportDeXuatDonViNew(dataReport);


                    TempData["dataReport"] = dataReport;
                }

                TempData["paramReport"] = data;
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        [HttpPost]
        public bool ExportBCTongHop(KH5NDXPrintDataExportModel data, List<string> arrIdNguonVon, List<string> arrIdDVQL, bool isModified, bool isCt)
        {
            try
            {
                string iID_NguonVonIDs = string.Join(",", arrIdNguonVon);
                string lstDonVi = string.Empty;
                if (arrIdDVQL != null)
                {
                    lstDonVi = string.Join(",", arrIdDVQL);
                }

                data.isTongHop = true;

                if (isModified && !isCt)
                {
                    List<KH5NDXPrintDataDieuChinhExportModel> dataReport = _iQLVonDauTuService.FindSuggestionDcReport(1, data.iID_KeHoach5Nam_DeXuatID.ToString(), lstDonVi, data.fDonViTinh.Value).ToList();
                    dataReport = HandleDataDeXuatDieuChinh(dataReport, isTongHop: true);
                    TempData["dataReportDc"] = dataReport;
                }
                else if (!isModified && isCt)
                {
                    //List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> dataReport = _iQLVonDauTuService.FindByReportKeHoachTrungHanDeXuatChuyenTiep(data.iID_KeHoach5Nam_DeXuatID.ToString(), iID_NguonVonIDs, data.iID_LoaiCongTrinh, lstDonVi, 1, data.fDonViTinh.Value).ToList();
                    List<KH5NDXPrintDataExportNewModel> dataReport = _iQLVonDauTuService.ReportKeHoachTrungHanDeXuatChuyenTiepNew(data.iID_KeHoach5Nam_DeXuatID.ToString(), iID_NguonVonIDs, data.iID_LoaiCongTrinh, 2, data.fDonViTinh.Value).ToList();
                    //dataReport = CalculateDataReportDeXuatChuyenTiepTongHop(dataReport);
                    //dataReport = HandleDataReportDeXuatChuyenTiepTongHop(dataReport);
                    dataReport = HandleDataReportDeXuatChuyenTiepTongHopNew(dataReport);

                    TempData["dataReportCt"] = dataReport;
                }
                else
                {
                    //List<KH5NDXPrintDataExportModel> dataReport = _iQLVonDauTuService.GetDataReportKH5NDeXuat(data.iID_KeHoach5Nam_DeXuatID.ToString(), data.iID_LoaiCongTrinh.ToString(), iID_NguonVonIDs, lstDonVi, 1, data.fDonViTinh.Value, PhienLamViec.NamLamViec).ToList();
                    List<KH5NDXPrintDataExportModel> dataReport = _iQLVonDauTuService.GetDataReportKH5NDeXuatNew(data.iID_KeHoach5Nam_DeXuatID.ToString(), data.iID_LoaiCongTrinh.ToString(), iID_NguonVonIDs, data.fDonViTinh.Value, PhienLamViec.NamLamViec, 2).ToList();

                    //dataReport = HandleDataHanMucDauTu(dataReport);
                    //dataReport = CalculateDataReportDeXuatTongHop(dataReport);
                    //dataReport = HandleDataReportDeXuatTongHop(dataReport);
                    dataReport = HandleDataReportDeXuatTongHopNew(dataReport);

                    TempData["dataReport"] = dataReport;
                }

                TempData["paramReport"] = data;
                List<int> arrYearStartAndYearEnd = new List<int>();
                arrYearStartAndYearEnd.Add(data.iGiaiDoanTu);
                arrYearStartAndYearEnd.Add(data.iGiaiDoanDen);
                TempData["arrYearStartAndYearEnd"] = arrYearStartAndYearEnd;

                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public ActionResult ExportExcelFile(bool isModified, bool isCt, bool isBaoCaoTheoDV)
        {
            string sFileName = isBaoCaoTheoDV ? "BaoCao_KeHoachTrungHan_DeXuat_TheoDonVi.xlsx" : "BaoCao_KeHoachTrungHan_DeXuat_TongHop.xlsx";
            List<KH5NDXPrintDataExportModel> dataReport = null;
            List<KH5NDXPrintDataDieuChinhExportModel> dataReportDc = null;
            List<KH5NDXPrintDataExportNewModel> dataReportCt = null;
            KH5NDXPrintDataExportModel paramReport = null;

            paramReport = (KH5NDXPrintDataExportModel)TempData["paramReport"];

            if (isModified && !isCt)
            {
                if (TempData["dataReportDc"] != null)
                {
                    dataReportDc = (List<KH5NDXPrintDataDieuChinhExportModel>)TempData["dataReportDc"];
                }
                else
                    return RedirectToAction("ViewInBaoCao");
            }
            else if (!isModified && isCt)
            {
                if (TempData["dataReportCt"] != null)
                {
                    dataReportCt = (List<KH5NDXPrintDataExportNewModel>)TempData["dataReportCt"];
                }
                else
                    return RedirectToAction("ViewInBaoCao");
            }
            else
            {
                if (TempData["dataReport"] != null)
                {
                    dataReport = (List<KH5NDXPrintDataExportModel>)TempData["dataReport"];
                }
                else
                    return RedirectToAction("ViewInBaoCao");
            }

            ExcelFile xls = null;
            if (isModified && !isCt)
            {
                xls = (XlsFile) CreateReportDc(dataReportDc, paramReport);
            }
            else if (!isModified && isCt)
            {
                xls = (XlsFile)CreateReportCt(dataReportCt, paramReport);
            }
            else
            {
                xls = (XlsFile)CreateReport(dataReport, paramReport);
            }

            return xls.ToFileResult(sFileName);
        }

        public ActionResult ExportExcel(bool isModified, bool isCt)
        {
            string sContentType = "application/pdf";
            string sFileName = "KeHoachTrungHan_DeXuat_Report.pdf";
            List<KH5NDXPrintDataExportModel> dataReport = null;
            List<KH5NDXPrintDataDieuChinhExportModel> dataReportDc = null;
            List<KH5NDXPrintDataExportNewModel> dataReportCt = null;
            KH5NDXPrintDataExportModel paramReport = null;

            paramReport = (KH5NDXPrintDataExportModel)TempData["paramReport"];

            if (isModified && !isCt)
            {
                if (TempData["dataReportDc"] != null)
                {
                    dataReportDc = (List<KH5NDXPrintDataDieuChinhExportModel>)TempData["dataReportDc"];
                }
                else
                    return RedirectToAction("ViewInBaoCao");
            }
            else if (!isModified && isCt)
            {
                if (TempData["dataReportCt"] != null)
                {
                    dataReportCt = (List<KH5NDXPrintDataExportNewModel>)TempData["dataReportCt"];
                }
                else
                    return RedirectToAction("ViewInBaoCao");
            }
            else
            {
                if (TempData["dataReport"] != null)
                {
                    dataReport = (List<KH5NDXPrintDataExportModel>)TempData["dataReport"];
                }
                else
                    return RedirectToAction("ViewInBaoCao");
            }

            ExcelFile xls = null;
            if (isModified && !isCt)
            {
                xls = CreateReportDc(dataReportDc, paramReport);
            }
            else if (!isModified && isCt)
            {
                xls = CreateReportCt(dataReportCt, paramReport);
            }
            else
            {
                xls = CreateReport(dataReport, paramReport);
            }

            xls.PrintLandscape = true;
            FlexCelPdfExport pdf = new FlexCelPdfExport(xls, true);
            //var bufferPdf = new MemoryStream();

            //pdf.Export(bufferPdf);

            //Response.ContentType = sContentType;
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + sFileName);
            //Response.BinaryWrite(bufferPdf.ToArray());

            //Response.Flush();
            //Response.End();
            return xls.ToPdfContentResult(sFileName);
        }

        public ExcelFile CreateReportCt(List<KH5NDXPrintDataExportNewModel> dataReport, KH5NDXPrintDataExportModel paramReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/rpt_KeHoachTrungHanDeXuat_ChuyenTiep_new.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel reportSummary = new VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel();
            //reportSummary.FHanMucDauTu = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTu);
            //reportSummary.FHanMucDauTuQP = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuQP);
            //reportSummary.FHanMucDauTuNN = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuNN);
            //reportSummary.FHanMucDauTuDP = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuDP);
            //reportSummary.FHanMucDauTuOrther = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuOrther);
            ////reportSummary.TongLuyKe = dataReport.Where(x => !x.IsHangCha).Sum(x => x.TongLuyKe);
            //reportSummary.LuyKeVonNSQPDaBoTri = dataReport.Where(x => !x.IsHangCha).Sum(x => x.LuyKeVonNSQPDaBoTri);
            //reportSummary.LuyKeVonNSQPDeNghiBoTri = dataReport.Where(x => !x.IsHangCha).Sum(x => x.LuyKeVonNSQPDeNghiBoTri);
            //reportSummary.FTongSo = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FTongSo);
            //reportSummary.FGiaTriNamThuNhat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuNhat);
            //reportSummary.FGiaTriNamThuHai = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuHai);
            //reportSummary.FGiaTriNamThuBa = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuBa);
            //reportSummary.FGiaTriNamThuTu = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuTu);
            //reportSummary.FGiaTriNamThuNam = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuNam);

            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(paramReport.iID_KeHoach5Nam_DeXuatID);

            int iNamBatDau = DateTime.Now.Year;
            int iNamKetThuc = DateTime.Now.Year + 4;
            string STenDonVi = string.Empty;

            if (itemQuery != null)
            {
                iNamBatDau = itemQuery.iGiaiDoanTu;
                iNamKetThuc = itemQuery.iGiaiDoanDen;
                //if (paramReport != null && paramReport.isTongHop)
                //{
                //    NS_DonVi itemDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, paramReport.iID_DonViQuanLyID.ToString());
                //    if (itemDonVi != null)
                //    {
                //        STenDonVi = itemDonVi.sTen;
                //    }
                //}
            }
            else
            {
                iNamBatDau = paramReport.iGiaiDoanTu;
                iNamKetThuc = paramReport.iGiaiDoanDen;
            }
            TempData.Keep("lstDonViQL");
            // string STenDonVi = string.Empty;
            var lstDVQL = TempData["lstDonViQL"] as IEnumerable<string>;
            string sMaDV = "";
            if (lstDVQL != null)
            {
                if (lstDVQL.Count() == 1)
                {
                    sMaDV = lstDVQL.First();
                    STenDonVi = _iQLVonDauTuService.GetNameDonViQLByMaDV(sMaDV).sTen;
                }
            }

            string sMuc = string.Join("+", dataReport.Where(x => x.LoaiParent.Equals(0)).Select(x => x.STT).ToList());

            FormatNumber formatNumber = new FormatNumber(1, ExportType.PDF);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.AddTable<KH5NDXPrintDataExportNewModel>("Items", dataReport);

            fr.SetValue("TenDonVi", !string.IsNullOrEmpty(STenDonVi) ? STenDonVi.ToUpper() : string.Empty);
            fr.SetValue("Header1", paramReport.txt_TieuDe1);
            fr.SetValue("Header2", paramReport.txt_TieuDe2);
            fr.SetValue("Header3", paramReport.txt_TieuDe3);
            fr.SetValue("BeforeYear", iNamBatDau - 2);
            fr.SetValue("YearPlan", iNamBatDau - 1);
            fr.SetValue("Year1", iNamBatDau);
            fr.SetValue("Year2", iNamBatDau + 1);
            fr.SetValue("Year3", iNamBatDau + 2);
            fr.SetValue("Year4", iNamBatDau + 3);
            fr.SetValue("Year5", iNamBatDau + 4);
            fr.SetValue("Period", string.Format("{0} - {1}", iNamBatDau, iNamBatDau + 4));
            fr.SetValue("DonViTinh", paramReport.sDonViTinh);
            //fr.SetValue("FHanMucDauTuSum", reportSummary.FHanMucDauTu);
            //fr.SetValue("FHanMucDauTuQPSum", reportSummary.FHanMucDauTuQP);
            //fr.SetValue("FHanMucDauTuNNSum", reportSummary.FHanMucDauTuNN);
            //fr.SetValue("FHanMucDauTuDPSum", reportSummary.FHanMucDauTuDP);
            //fr.SetValue("FHanMucDauTuOrtherSum", reportSummary.FHanMucDauTuOrther);
            //fr.SetValue("TongLuyKeSum", reportSummary.TongLuyKe);
            //fr.SetValue("LuyKeVonNSQPDaBoTriSum", reportSummary.LuyKeVonNSQPDaBoTri);
            //fr.SetValue("LuyKeVonNSQPDeNghiBoTriSum", reportSummary.LuyKeVonNSQPDeNghiBoTri);
            //fr.SetValue("FTongSoSum", reportSummary.FTongSo);
            //fr.SetValue("FGiaTriNamThuNhatSum", reportSummary.FGiaTriNamThuNhat);
            //fr.SetValue("FGiaTriNamThuHaiSum", reportSummary.FGiaTriNamThuHai);
            //fr.SetValue("FGiaTriNamThuBaSum", reportSummary.FGiaTriNamThuBa);
            //fr.SetValue("FGiaTriNamThuTuSum", reportSummary.FGiaTriNamThuTu);
            //fr.SetValue("FGiaTriNamThuNamSum", reportSummary.FGiaTriNamThuNam);
            fr.SetValue("Muc", sMuc);
            fr.UseChuKy(Username)
                   .UseChuKyForController(sControlName)
                   .UseForm(this);

            fr.Run(Result);

            _iQLVonDauTuService.FormatAllRowHeight(dataReport, "STenDuAn", 12, 30, Result, 600);         //set height neu dong qua nho
            Result.SetRowHeight(11, 700);

            return Result;
        }

        public ExcelFile CreateReportDc(List<KH5NDXPrintDataDieuChinhExportModel> dataReport, KH5NDXPrintDataExportModel paramReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/rpt_KHTH_DeXuat_Dieu_Chinh.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            KH5NDXPrintDataDieuChinhExportModel dataSummary = new KH5NDXPrintDataDieuChinhExportModel();
            if (dataReport != null && dataReport.Count() > 0)
            {
                dataSummary.FHanMucDauTuDuocDuyet = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuDuocDuyet);
                dataSummary.FTongSoDuocDuyet = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FTongSoDuocDuyet);
                dataSummary.FVonBoTriTuNamDenNamDuocDuyet = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FVonBoTriTuNamDenNamDuocDuyet);
                dataSummary.FVonBoTriSauNamDuocDuyet = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FVonBoTriSauNamDuocDuyet);
                dataSummary.FHanMucDauTuDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuDeXuat);
                dataSummary.FTongSoDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FTongSoDeXuat);
                dataSummary.FTongCongDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FTongCongDeXuat);
                dataSummary.FGiaTriNamThuNhatDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuNhatDeXuat);
                dataSummary.FGiaTriNamThuHaiDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuHaiDeXuat);
                dataSummary.FGiaTriNamThuBaDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuBaDeXuat);
                dataSummary.FGiaTriNamThuTuDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuTuDeXuat);
                dataSummary.FGiaTriNamThuNamDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuNamDeXuat);
                dataSummary.FGiaTriSauNamDeXuat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriSauNamDeXuat);
                dataSummary.FHanMucDauTuChenhLech = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuChenhLech);
                dataSummary.FTongSoChenhLech = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FTongSoChenhLech);
                dataSummary.FVonBoTriTuNamDenNamChenhLech = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FVonBoTriTuNamDenNamChenhLech);
                dataSummary.FVonBoTriSauNamChenhLech = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FVonBoTriSauNamChenhLech);
            }

            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(paramReport.iID_KeHoach5Nam_DeXuatID);

            TempData.Keep("arrYearStartAndYearEnd");
            var arrYearStartAndYearEnd = TempData["arrYearStartAndYearEnd"] as IEnumerable<int>;
            int YearStart = DateTime.Now.Year; ;
            int YearEnd = DateTime.Now.Year + 4;
            if (arrYearStartAndYearEnd != null)
            {

                YearStart = arrYearStartAndYearEnd.First();
                YearEnd = arrYearStartAndYearEnd.Last();


            }

            string STenDonVi = string.Empty;
            if (itemQuery != null)
            {
                if (paramReport != null && paramReport.isTongHop)
                {
                    NS_DonVi itemDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, paramReport.iID_DonViQuanLyID.ToString());
                    if (itemDonVi != null)
                    {
                        STenDonVi = itemDonVi.sTen;
                    }
                }
            }
            TempData.Keep("lstDonViQL");
            // string STenDonVi = string.Empty;
            var lstDVQL = TempData["lstDonViQL"] as IEnumerable<string>;
            string sMaDV = "";
            if (lstDVQL != null)
            {
                if (lstDVQL.Count() == 1)
                {
                    sMaDV = lstDVQL.First();
                    STenDonVi = _iQLVonDauTuService.GetNameDonViQLByMaDV(sMaDV).sTen;
                }
            }

            FormatNumber formatNumber = new FormatNumber(1, ExportType.PDF);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.AddTable<KH5NDXPrintDataDieuChinhExportModel>("Items", dataReport);

            fr.SetValue("TenDonVi", !string.IsNullOrEmpty(STenDonVi) ? STenDonVi.ToUpper() : string.Empty);
            fr.SetValue("Header1", paramReport.txt_TieuDe1);
            fr.SetValue("Header2", paramReport.txt_TieuDe2);
            fr.SetValue("Header3", paramReport.txt_TieuDe3);
            fr.SetValue("YearPeriod", string.Format("{0} - {1}", YearStart, YearEnd));
            fr.SetValue("Year1", YearStart.ToString());
            fr.SetValue("Year2", (YearStart + 1).ToString());
            fr.SetValue("Year3", (YearStart + 2).ToString());
            fr.SetValue("Year4", (YearStart + 3).ToString());
            fr.SetValue("Year5", (YearStart + 4).ToString());
            fr.SetValue("DonViTinh", paramReport.sDonViTinh);
            fr.SetValue("FHanMucDauTuDuocDuyetSum", dataSummary.FHanMucDauTuDuocDuyet);
            fr.SetValue("FTongSoDuocDuyetSum", dataSummary.FTongSoDuocDuyet);
            fr.SetValue("FVonBoTriTuNamDenNamDuocDuyetSum", dataSummary.FVonBoTriTuNamDenNamDuocDuyet);
            fr.SetValue("FVonBoTriSauNamDuocDuyetSum", dataSummary.FVonBoTriSauNamDuocDuyet);
            fr.SetValue("FHanMucDauTuDeXuatSum", dataSummary.FHanMucDauTuDeXuat);
            fr.SetValue("FTongSoDeXuatSum", dataSummary.FTongSoDeXuat);
            fr.SetValue("FTongCongDeXuatSum", dataSummary.FTongCongDeXuat);
            fr.SetValue("FGiaTriNamThuNhatDeXuatSum", dataSummary.FGiaTriNamThuNhatDeXuat);
            fr.SetValue("FGiaTriNamThuHaiDeXuatSum", dataSummary.FGiaTriNamThuHaiDeXuat);
            fr.SetValue("FGiaTriNamThuBaDeXuatSum", dataSummary.FGiaTriNamThuBaDeXuat);
            fr.SetValue("FGiaTriNamThuTuDeXuatSum", dataSummary.FGiaTriNamThuTuDeXuat);
            fr.SetValue("FGiaTriNamThuNamDeXuatSum", dataSummary.FGiaTriNamThuNamDeXuat);
            fr.SetValue("FGiaTriSauNamDeXuatSum", dataSummary.FGiaTriSauNamDeXuat);
            fr.SetValue("FHanMucDauTuChenhLechSum", dataSummary.FHanMucDauTuChenhLech);
            fr.SetValue("FTongSoChenhLechSum", dataSummary.FTongSoChenhLech);
            fr.SetValue("FVonBoTriTuNamDenNamChenhLechSum", dataSummary.FVonBoTriTuNamDenNamChenhLech);
            fr.SetValue("FVonBoTriSauNamChenhLechSum", dataSummary.FVonBoTriSauNamChenhLech);
            fr.UseChuKy(Username)
                   .UseChuKyForController(sControlName)
                   .UseForm(this);

            fr.Run(Result);

            _iQLVonDauTuService.FormatAllRowHeight(dataReport, "STenDuAn", 14, 30, Result, 600);         //set height neu dong qua nho
            Result.SetRowHeight(13, 700);

            return Result;
        }

        public ExcelFile CreateReport(List<KH5NDXPrintDataExportModel> dataReport, KH5NDXPrintDataExportModel paramReport)
        {
            TempData.Keep("lstDonViQL");
            string STenDonVi = string.Empty;
            var lstDVQL = TempData["lstDonViQL"] as IEnumerable<string>;
            string sMaDV = "";
            if (lstDVQL != null)
            {
                if (lstDVQL.Count() == 1)
                {
                    sMaDV = lstDVQL.First();
                    STenDonVi = _iQLVonDauTuService.GetNameDonViQLByMaDV(sMaDV).sTen;
                }
            }

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/rpt_KeHoachtrungHanDeXuat_Export_New.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            string sMuc = string.Join("+", dataReport.Where(x => x.LoaiParent.Equals(0)).Select(x => x.STT).ToList());

            KH5NDXPrintDataExportModel dataSummary = new KH5NDXPrintDataExportModel();

            dataSummary.FHanMucDauTu = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTu);
            dataSummary.FHanMucDauTuQP = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuQP);
            dataSummary.FHanMucDauTuNN = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuNN);
            dataSummary.FHanMucDauTuDP = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuDP);
            dataSummary.FHanMucDauTuOrther = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FHanMucDauTuOrther);
            dataSummary.FTongSoNhuCau = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FTongSoNhuCau);
            dataSummary.FGiaTriNamThuNhat = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuNhat);
            dataSummary.FGiaTriNamThuHai = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuHai);
            dataSummary.FGiaTriNamThuBa = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuBa);
            dataSummary.FGiaTriNamThuTu = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuTu);
            dataSummary.FGiaTriNamThuNam = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriNamThuNam);
            dataSummary.FGiaTriBoTri = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FGiaTriBoTri);
            dataSummary.FTongSo = dataReport.Where(x => !x.IsHangCha).Sum(x => x.FTongSo);

            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(paramReport.iID_KeHoach5Nam_DeXuatID);

            int iNamBatDau = paramReport.iGiaiDoanTu;
            int iNamKetThuc = paramReport.iGiaiDoanTu + 4;


            if (itemQuery != null)
            {
                iNamBatDau = itemQuery.iGiaiDoanTu;
                iNamKetThuc = itemQuery.iGiaiDoanDen;
                //if (paramReport != null && paramReport.isTongHop)
                //{
                //    NS_DonVi itemDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, paramReport.iID_DonViQuanLyID.ToString());
                //    if (itemDonVi != null)
                //    {
                //        STenDonVi = itemDonVi.sTen;
                //    }
                //}
            }
            FormatNumber formatNumber = new FormatNumber(1, ExportType.PDF);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.AddTable<KH5NDXPrintDataExportModel>("Items", dataReport);

            fr.SetValue("Period", string.Format("{0}-{1}", iNamBatDau, iNamKetThuc));
            fr.SetValue("Year1", iNamBatDau);
            fr.SetValue("Year2", iNamBatDau + 1);
            fr.SetValue("Year3", iNamBatDau + 2);
            fr.SetValue("Year4", iNamBatDau + 3);
            fr.SetValue("Year5", iNamBatDau + 4);
            fr.SetValue("TenDonVi", !string.IsNullOrEmpty(STenDonVi) ? STenDonVi.ToUpper() : string.Empty);
            fr.SetValue("iNamLamViec", PhienLamViec.iNamLamViec);
            fr.SetValue("Header1", paramReport.txt_TieuDe1);
            fr.SetValue("Header2", paramReport.txt_TieuDe2);
            fr.SetValue("Header3", paramReport.txt_TieuDe3);
            fr.SetValue("DonViTinh", paramReport.sDonViTinh);
            fr.SetValue("Muc", sMuc);
            fr.SetValue("FHanMucDauTuSum", dataSummary.FHanMucDauTu);
            fr.SetValue("FHanMucDauTuQPSum", dataSummary.FHanMucDauTuQP);
            fr.SetValue("FHanMucDauTuNNSum", dataSummary.FHanMucDauTuNN);
            fr.SetValue("FHanMucDauTuDPSum", dataSummary.FHanMucDauTuDP);
            fr.SetValue("FHanMucDauTuOrtherSum", dataSummary.FHanMucDauTuOrther);
            fr.SetValue("FTongSoNhuCauSum", dataSummary.FTongSoNhuCau);
            fr.SetValue("FTongSoSum", dataSummary.FTongSo);
            fr.SetValue("FGiaTriNamThuNhatSum", dataSummary.FGiaTriNamThuNhat);
            fr.SetValue("FGiaTriNamThuHaiSum", dataSummary.FGiaTriNamThuHai);
            fr.SetValue("FGiaTriNamThuBaSum", dataSummary.FGiaTriNamThuBa);
            fr.SetValue("FGiaTriNamThuTuSum", dataSummary.FGiaTriNamThuTu);
            fr.SetValue("FGiaTriNamThuNamSum", dataSummary.FGiaTriNamThuNam);
            fr.SetValue("FGiaTriBoTriSum", dataSummary.FGiaTriBoTri);
            fr.UseChuKy(Username)
                   .UseChuKyForController(sControlName)
                   .UseForm(this);

            fr.Run(Result);

            //_iQLVonDauTuService.FormatAllRowHeight(dataReport, "STenDuAn", 12, 30, Result, 700);         //set height neu dong qua nho
            Result.SetRowHeight(11, 800);                                                                // set co dinh height Dong Tong so

            return Result;
        }

        private List<KeHoach5NamDeXuatModel> CreateVoucherTypes()
        {
            List<KeHoach5NamDeXuatModel> lstVoucherTypes = new List<KeHoach5NamDeXuatModel>()
            {
                new KeHoach5NamDeXuatModel(){SVoucherTypes = "Mở mới", iLoai = 1},
                new KeHoach5NamDeXuatModel(){SVoucherTypes = "Chuyển tiếp", iLoai = 2}
            };

            return lstVoucherTypes;
        }

        private List<DuAnKeHoach5Nam> GetDuAn(string iMaDonVi)
        {
            List<DuAnKeHoach5Nam> lstDuAn = new List<DuAnKeHoach5Nam>();
            if (!string.IsNullOrEmpty(iMaDonVi))
            {
                lstDuAn = _iQLVonDauTuService.GetAllDuAnChuyenTiep(iMaDonVi).ToList();
            }

            return lstDuAn;
        }

        private List<DuAnKeHoach5Nam> GetDuAnDaLapCTDT(string idDonVi)
        {
            List<DuAnKeHoach5Nam> lstDuAn = new List<DuAnKeHoach5Nam>();
            if (!string.IsNullOrEmpty(idDonVi))
            {
                lstDuAn = _iQLVonDauTuService.GetAllDuAnDaLapCTDT(idDonVi).ToList();
            }

            return lstDuAn;
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

        private List<KH5NDXPrintDataExportModel> HandleDataHanMucDauTu(List<KH5NDXPrintDataExportModel> dataReport)
        {
            foreach (KH5NDXPrintDataExportModel item in dataReport)
            {
                if (item.IIdNguonVon == 1)
                {
                    item.FHanMucDauTuQP = item.FHanMucDauTu;
                }
                else if (item.IIdNguonVon == 2)
                {
                    item.FHanMucDauTuNN = item.FHanMucDauTu;
                }
                else if (item.IIdNguonVon == 3)
                {
                    item.FHanMucDauTuDP = item.FHanMucDauTu;
                }
                else
                {
                    item.FHanMucDauTuOrther = item.FHanMucDauTu;
                }

                if (item.IIdNguonVon != 1)
                {
                    item.FTongSoNhuCau = 0;
                }
            }

            return dataReport;
        }

        // tính hạn mức đầu tư dựa trên iLevel khi export, chạy 1 lần duy nhất cho 1 parent
        private void calculateForParent(KH5NDXPrintDataExportModel pr, List<KH5NDXPrintDataExportModel> lstChildren)
        {
            // filter children level 1
            List<KH5NDXPrintDataExportModel> levelOneChildren = lstChildren.Where(child => child.iLevel == 1).ToList();
            // filter children level 2
            List<KH5NDXPrintDataExportModel> levelTwoChildren = lstChildren.Where(child => child.iLevel == 2).ToList();
            // filter children level 3
            List<KH5NDXPrintDataExportModel> levelThreeChildren = lstChildren.Where(child => child.iLevel == 3).ToList();

            if (levelOneChildren.Count > 0)
            {
                // loại bỏ các con level 2,3
                foreach (KH5NDXPrintDataExportModel lvOne in levelOneChildren)
                {
                    // cộng dồn giá trị vào parent
                    pr.FHanMucDauTu += lvOne.FHanMucDauTu;
                    pr.FTongSoNhuCau += lvOne.FTongSoNhuCau;
                    pr.FTongSo += lvOne.FTongSo;
                    pr.FGiaTriNamThuNhat += lvOne.FGiaTriNamThuNhat;
                    pr.FGiaTriNamThuHai += lvOne.FGiaTriNamThuHai;
                    pr.FGiaTriNamThuBa += lvOne.FGiaTriNamThuBa;
                    pr.FGiaTriNamThuTu += lvOne.FGiaTriNamThuTu;
                    pr.FGiaTriNamThuNam += lvOne.FGiaTriNamThuNam;
                    pr.FGiaTriBoTri += lvOne.FGiaTriBoTri;
                    pr.TongLuyKe += lvOne.TongLuyKe;
                    pr.LuyKeVonNSQPDaBoTri += lvOne.LuyKeVonNSQPDaBoTri;
                    pr.LuyKeVonNSQPDeNghiBoTri += lvOne.LuyKeVonNSQPDeNghiBoTri;
                    pr.FHanMucDauTuQP += lvOne.FHanMucDauTuQP;
                    pr.FHanMucDauTuNN += lvOne.FHanMucDauTuNN;
                    pr.FHanMucDauTuDP += lvOne.FHanMucDauTuDP;
                    pr.FHanMucDauTuOrther += lvOne.FHanMucDauTuOrther;
                    // loại bỏ các con tương ứng với level hiện tại
                    var currentSTTLvOne = lvOne.STT;
                    // loại bỏ 1.1 hoặc 1.1.1 nếu stt hiện tại đang là 1
                    levelTwoChildren.RemoveAll(ele => ele.STT.Split('.')[0] == currentSTTLvOne);
                    levelThreeChildren.RemoveAll(ele => ele.STT.Split('.')[0] == currentSTTLvOne);
                }
            }
            if (levelTwoChildren.Count > 0)
            {
                // loại bỏ các con level 3
                foreach (KH5NDXPrintDataExportModel lvTwo in levelTwoChildren)
                {
                    // cộng dồn giá trị vào parent
                    pr.FHanMucDauTu += lvTwo.FHanMucDauTu;
                    pr.FTongSoNhuCau += lvTwo.FTongSoNhuCau;
                    pr.FTongSo += lvTwo.FTongSo;
                    pr.FGiaTriNamThuNhat += lvTwo.FGiaTriNamThuNhat;
                    pr.FGiaTriNamThuHai += lvTwo.FGiaTriNamThuHai;
                    pr.FGiaTriNamThuBa += lvTwo.FGiaTriNamThuBa;
                    pr.FGiaTriNamThuTu += lvTwo.FGiaTriNamThuTu;
                    pr.FGiaTriNamThuNam += lvTwo.FGiaTriNamThuNam;
                    pr.FGiaTriBoTri += lvTwo.FGiaTriBoTri;
                    pr.TongLuyKe += lvTwo.TongLuyKe;
                    pr.LuyKeVonNSQPDaBoTri += lvTwo.LuyKeVonNSQPDaBoTri;
                    pr.LuyKeVonNSQPDeNghiBoTri += lvTwo.LuyKeVonNSQPDeNghiBoTri;
                    pr.FHanMucDauTuQP += lvTwo.FHanMucDauTuQP;
                    pr.FHanMucDauTuNN += lvTwo.FHanMucDauTuNN;
                    pr.FHanMucDauTuDP += lvTwo.FHanMucDauTuDP;
                    pr.FHanMucDauTuOrther += lvTwo.FHanMucDauTuOrther;
                    // loại bỏ các con tương ứng với level hiện tại
                    var currentSTTLvTwo = lvTwo.STT;
                    // loại bỏ 1.1.1 nếu stt hiện tại đang là 1.1
                    levelThreeChildren.RemoveAll(ele => ele.STT.Split('.')[1] == currentSTTLvTwo);
                }
            }
            if (levelThreeChildren.Count > 0)
            {
                // cộng hết những thằng level 3 k có bố
                foreach (KH5NDXPrintDataExportModel lvThree in levelThreeChildren)
                {
                    pr.FHanMucDauTu += lvThree.FHanMucDauTu;
                    pr.FTongSoNhuCau += lvThree.FTongSoNhuCau;
                    pr.FTongSo += lvThree.FTongSo;
                    pr.FGiaTriNamThuNhat += lvThree.FGiaTriNamThuNhat;
                    pr.FGiaTriNamThuHai += lvThree.FGiaTriNamThuHai;
                    pr.FGiaTriNamThuBa += lvThree.FGiaTriNamThuBa;
                    pr.FGiaTriNamThuTu += lvThree.FGiaTriNamThuTu;
                    pr.FGiaTriNamThuNam += lvThree.FGiaTriNamThuNam;
                    pr.FGiaTriBoTri += lvThree.FGiaTriBoTri;
                    pr.TongLuyKe += lvThree.TongLuyKe;
                    pr.LuyKeVonNSQPDaBoTri += lvThree.LuyKeVonNSQPDaBoTri;
                    pr.LuyKeVonNSQPDeNghiBoTri += lvThree.LuyKeVonNSQPDeNghiBoTri;
                    pr.FHanMucDauTuQP += lvThree.FHanMucDauTuQP;
                    pr.FHanMucDauTuNN += lvThree.FHanMucDauTuNN;
                    pr.FHanMucDauTuDP += lvThree.FHanMucDauTuDP;
                    pr.FHanMucDauTuOrther += lvThree.FHanMucDauTuOrther;
                }
            }
        }

        private List<KH5NDXPrintDataExportModel> CalculateDataReportDeXuatTongHop(List<KH5NDXPrintDataExportModel> ItemsReportDeXuat)
        {
            try
            {
                List<KH5NDXPrintDataExportModel> childrent = ItemsReportDeXuat.Where(x => !x.IsHangCha).ToList();
                List<KH5NDXPrintDataExportModel> parent = ItemsReportDeXuat.Where(x => x.IsHangCha && (x.LoaiParent.Equals(1) || x.LoaiParent.Equals(0))).ToList();

                ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.FHanMucDauTu = 0;
                    x.FTongSoNhuCau = 0;
                    x.FTongSo = 0;
                    x.FGiaTriNamThuNhat = 0;
                    x.FGiaTriNamThuHai = 0;
                    x.FGiaTriNamThuBa = 0;
                    x.FGiaTriNamThuTu = 0;
                    x.FGiaTriNamThuNam = 0;
                    x.TongLuyKe = 0;
                    x.LuyKeVonNSQPDaBoTri = 0;
                    x.LuyKeVonNSQPDeNghiBoTri = 0;
                    x.FHanMucDauTuQP = 0;
                    x.FHanMucDauTuNN = 0;
                    x.FHanMucDauTuDP = 0;
                    x.FHanMucDauTuOrther = 0;
                    x.FGiaTriBoTri = 0;
                    return x;
                }).ToList();

                foreach (var pr in parent)
                {
                    Boolean funcIsCalled = false;
                    List<KH5NDXPrintDataExportModel> lstChilrent = childrent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                             || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                             || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.TongLuyKe != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)))
                    {
                        // check if calculateForParent is called
                        if (!funcIsCalled)
                        {
                            calculateForParent(pr, lstChilrent);
                            funcIsCalled = true;
                        }

                        //pr.FHanMucDauTu += item.FHanMucDauTu ?? 0;
                        //pr.FTongSoNhuCau = item.FTongSoNhuCau ?? 0;
                        //pr.FTongSo += item.FTongSo ?? 0;
                        //pr.FGiaTriNamThuNhat += item.FGiaTriNamThuNhat ?? 0;
                        //pr.FGiaTriNamThuHai += item.FGiaTriNamThuHai ?? 0;
                        //pr.FGiaTriNamThuBa += item.FGiaTriNamThuBa ?? 0;
                        //pr.FGiaTriNamThuTu += item.FGiaTriNamThuTu ?? 0;
                        //pr.FGiaTriNamThuNam += item.FGiaTriNamThuNam ?? 0;
                        //pr.FGiaTriBoTri += item.FGiaTriBoTri ?? 0;
                        //pr.TongLuyKe += item.TongLuyKe ?? 0;
                        //pr.LuyKeVonNSQPDaBoTri += item.LuyKeVonNSQPDaBoTri ?? 0;
                        //pr.LuyKeVonNSQPDeNghiBoTri += item.LuyKeVonNSQPDeNghiBoTri ?? 0;
                        //pr.FHanMucDauTuQP += item.FHanMucDauTuQP ?? 0;
                        //pr.FHanMucDauTuNN += item.FHanMucDauTuNN ?? 0;
                        //pr.FHanMucDauTuDP += item.FHanMucDauTuDP ?? 0;
                        //pr.FHanMucDauTuOrther += item.FHanMucDauTuOrther ?? 0;
                    }
                }

                foreach (var item in parent.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                            || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                            || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.TongLuyKe != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0
                            || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)))
                {
                    CalculateParentDeXuat(item, item, ItemsReportDeXuat);
                }

                List<KH5NDXPrintDataExportModel> lstDataChildrent = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)).ToList();

                List<KH5NDXPrintDataExportModel> lstDataParent = ItemsReportDeXuat.Where(x => x.LoaiParent.Equals(0)
                            && lstDataChildrent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh)).ToList();

                List<KH5NDXPrintDataExportModel> listGroup = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0
                                || lstDataParent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh))).ToList().GroupBy(x => new
                                {
                                    x.STT,
                                    x.IdLoaiCongTrinh,
                                    x.IdLoaiCongTrinhParent,
                                    x.SMaLoaiCongTrinh,
                                    x.STenDuAn,
                                    x.STenDonVi,
                                    x.SDiaDiem,
                                    x.SThoiGianThucHien,
                                    x.SGhiChu,
                                    x.Loai,
                                    x.LoaiParent,
                                    x.IsHangCha
                                }).Select(x => new KH5NDXPrintDataExportModel()
                                {
                                    STT = x.Key.STT,
                                    IdLoaiCongTrinh = x.Key.IdLoaiCongTrinh,
                                    IdLoaiCongTrinhParent = x.Key.IdLoaiCongTrinhParent,
                                    SMaLoaiCongTrinh = x.Key.SMaLoaiCongTrinh,
                                    STenDuAn = x.Key.STenDuAn,
                                    STenDonVi = x.Key.STenDonVi,
                                    SDiaDiem = x.Key.SDiaDiem,
                                    SThoiGianThucHien = x.Key.SThoiGianThucHien,
                                    FHanMucDauTu = x.Sum(rpt => rpt.FHanMucDauTu),
                                    FTongSoNhuCau = x.Sum(rpt => rpt.FTongSoNhuCau),
                                    FTongSo = x.Sum(rpt => rpt.FTongSo),
                                    FGiaTriNamThuNhat = x.Sum(rpt => rpt.FGiaTriNamThuNhat),
                                    FGiaTriNamThuHai = x.Sum(rpt => rpt.FGiaTriNamThuHai),
                                    FGiaTriNamThuBa = x.Sum(rpt => rpt.FGiaTriNamThuBa),
                                    FGiaTriNamThuTu = x.Sum(rpt => rpt.FGiaTriNamThuTu),
                                    FGiaTriNamThuNam = x.Sum(rpt => rpt.FGiaTriNamThuNam),
                                    FGiaTriBoTri = x.Sum(rpt => rpt.FGiaTriBoTri),
                                    SGhiChu = x.Key.SGhiChu,
                                    FHanMucDauTuQP = x.Sum(rpt => rpt.FHanMucDauTuQP),
                                    FHanMucDauTuNN = x.Sum(rpt => rpt.FHanMucDauTuNN),
                                    FHanMucDauTuDP = x.Sum(rpt => rpt.FHanMucDauTuDP),
                                    FHanMucDauTuOrther = x.Sum(rpt => rpt.FHanMucDauTuOrther),
                                    LuyKeVonNSQPDaBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDaBoTri),
                                    LuyKeVonNSQPDeNghiBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDeNghiBoTri),
                                    Loai = x.Key.Loai,
                                    LoaiParent = x.Key.LoaiParent,
                                    IsHangCha = x.Key.IsHangCha
                                }).ToList();

                return listGroup;
            }
            catch (Exception ex)
            {
                return new List<KH5NDXPrintDataExportModel>();
            }
        }

        private List<KH5NDXPrintDataExportModel> HandleDataReportDeXuatTongHop(List<KH5NDXPrintDataExportModel> ItemsReportDeXuat)
        {
            try
            {
                List<KH5NDXPrintDataExportModel> lstItem = ItemsReportDeXuat.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
                List<KH5NDXPrintDataExportModel> lstItemLevel = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                dctItemLevel.Keys.Select(x =>
                {
                    List<KH5NDXPrintDataExportModel> lstItemStt = dctItemLevel[x].ToList();
                    lstItemStt.Select(y => { y.STT = (lstItemStt.IndexOf(y) + 1).ToString(); return y; }).ToList();
                    return x;
                }).ToList();

                foreach (var item in lstItemLevel)
                {
                    List<KH5NDXPrintDataExportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && !x.IsHangCha).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }
                List<KH5NDXPrintDataExportModel> lstLctParent = ItemsReportDeXuat.Where(x => x.LoaiParent.Equals(0)).ToList();
                var dctItemParent = ItemsReportDeXuat.Where(x => !x.IsHangCha && x.IdLoaiCongTrinh.HasValue
                    && lstLctParent.Select(y => y.IdLoaiCongTrinh).Contains(x.IdLoaiCongTrinh)).GroupBy(z => z.IdLoaiCongTrinh).ToDictionary(z => z.Key.ToString(), z => z.ToList());
                foreach (var item in dctItemParent.Keys)
                {
                    List<KH5NDXPrintDataExportModel> itemStt = dctItemParent[item];
                    itemStt.Select(x => { x.STT = string.Format("{0}", (itemStt.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                return ItemsReportDeXuat;
            }
            catch (Exception ex)
            {
                return new List<KH5NDXPrintDataExportModel>();
            }
        }

        private List<KH5NDXPrintDataExportModel> HandleDataReportDeXuatTongHopNew(List<KH5NDXPrintDataExportModel> ItemsReportDeXuat)
        {
            try
            {
                List<KH5NDXPrintDataExportModel> ItemsReportTongHop = new List<KH5NDXPrintDataExportModel>();
                var dicLoaiCongTrinh = ItemsReportDeXuat.Where(x => x.Loai ==0).ToDictionary(k => k.IdLoaiCongTrinh,x =>x.sTenLoaiCongTrinh);
                var sSTT = 1;
                var sSTTChild = 1;

                foreach(var item in ItemsReportDeXuat)
                {
                    if (dicLoaiCongTrinh.ContainsKey(item.IdLoaiCongTrinh))
                    {
                        var listItems = ItemsReportDeXuat.Where(x => !x.IsHangCha && x.IdLoaiCongTrinh == item.IdLoaiCongTrinh).ToList();

                        if (listItems.Any() && item.IsHangCha)
                        {
                            sSTTChild = 1;
                            item.FHanMucDauTu = listItems.Sum(s => s.FHanMucDauTu);
                            item.FTongSo = listItems.Sum(x => x.FTongSo);
                            item.FGiaTriBoTri = listItems.Sum(x => x.FGiaTriBoTri);
                            item.fTongVonBoTri = listItems.Sum(x => x.FTongSo) + listItems.Sum(x => x.FGiaTriBoTri);
                            item.fHanmucNganhDX = listItems.Sum(x => x.fHanmucNganhDX);
                            item.fTongVonBoTriNganh = listItems.Sum(x => x.fTongVonBoTriNganh);
                            item.fVon5namNganhDX = listItems.Sum(x => x.fVon5namNganhDX);
                            item.fVonsaunamNganhDX = listItems.Sum(x => x.fVonsaunamNganhDX);
                            item.fHanmucCucTCDX = listItems.Sum(x => x.fHanmucCucTCDX);
                            item.fTongVonBoTriCuc = listItems.Sum(x => x.fTongVonBoTriCuc);
                            item.fVon5namCTCDX = listItems.Sum(x => x.fVon5namCTCDX);
                            item.fVonnamthunhatCTC = listItems.Sum(x => x.fVonnamthunhatCTC);
                            item.fVonsaunamCTCDexuat = listItems.Sum(x => x.fVonsaunamCTCDexuat);
                            item.fCucTCDeXuat = listItems.Sum(x => x.fCucTCDeXuat);
                            item.fDuKienBoTriNamThu2 = listItems.Sum(x => x.fDuKienBoTriNamThu2);
                            item.STenDuAn = dicLoaiCongTrinh[item.IdLoaiCongTrinh];
                            item.STT = sSTT.ToString();

                            ItemsReportTongHop.Add(item);
                            foreach(var itemChild in listItems)
                            {
                                itemChild.STT = sSTTChild.ToString();
                                itemChild.fTongVonBoTri = (itemChild.FTongSo + itemChild.FGiaTriBoTri);
                                ItemsReportTongHop.Add(itemChild);
                                sSTTChild++;
                            }
                            sSTT++;
                        }
                    }
                }

                return ItemsReportTongHop;
            }
            catch (Exception ex)
            {
                return new List<KH5NDXPrintDataExportModel>();

            }
        }
        
        private List<KH5NDXPrintDataExportNewModel> HandleDataReportDeXuatChuyenTiepTongHopNew(List<KH5NDXPrintDataExportNewModel> ItemsReportDeXuat)
        {
            try
            {
                List<KH5NDXPrintDataExportNewModel> ItemsReportTongHop = new List<KH5NDXPrintDataExportNewModel>();
                var dicLoaiCongTrinh = ItemsReportDeXuat.Where(x => x.LoaiParent == 2).ToDictionary(k => k.IdLoaiCongTrinh,x =>x.sTenLoaiCongTrinh);
                var sSTT = 1;
                var sSTTChild = 1;

                foreach(var item in ItemsReportDeXuat)
                {
                    if (dicLoaiCongTrinh.ContainsKey(item.IdLoaiCongTrinh))
                    {
                        var listItems = ItemsReportDeXuat.Where(x => !x.IsHangCha && x.IdLoaiCongTrinh == item.IdLoaiCongTrinh).ToList();

                        if (listItems.Any() && item.IsHangCha)
                        {
                            sSTTChild = 1;
                            item.fNSQP = listItems.Sum(s => s.fNSQP);
                            item.fChiPhiDuPhong = listItems.Sum(x => x.fChiPhiDuPhong);
                            item.fVonBoTriHetNam = listItems.Sum(x => x.fVonBoTriHetNam);
                            item.fSoConLaiPhaiBoTri = listItems.Sum(x => x.fSoConLaiPhaiBoTri);
                            item.fCucTCdexuat = listItems.Sum(x => x.fCucTCdexuat);
                            item.fBoDaGiaoDuToan = listItems.Sum(x => x.fBoDaGiaoDuToan);
                            item.fDuKienbotrinamthu2 = listItems.Sum(x => x.fDuKienbotrinamthu2);
                            item.fSoConLaiConBotri = listItems.Sum(x => x.fSoConLaiConBotri);
                            item.fSoTienConLaiChuaDeXuat = listItems.Sum(x => x.fSoTienConLaiChuaDeXuat);

                            item.STenDuAn = dicLoaiCongTrinh[item.IdLoaiCongTrinh];
                            item.STT = sSTT.ToString();

                            ItemsReportTongHop.Add(item);
                            foreach(var itemChild in listItems)
                            {
                                itemChild.STT = sSTTChild.ToString();
                                ItemsReportTongHop.Add(itemChild);
                                sSTTChild++;
                            }
                            sSTT++;
                        }
                    }
                }

                return ItemsReportTongHop;
            }
            catch (Exception ex)
            {
                return new List<KH5NDXPrintDataExportNewModel>();

            }
        }


        private List<KH5NDXPrintDataExportModel> CalculateDataReportDeXuatDonVi(List<KH5NDXPrintDataExportModel> ItemsReportDeXuat)
        {
            try
            {
                List<KH5NDXPrintDataExportModel> lstDonViparent = ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                List<KH5NDXPrintDataExportModel> lstLoaiCongTrinhparent = ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).ToList();

                ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.FHanMucDauTu = 0;
                    x.FTongSoNhuCau = 0;
                    x.FTongSo = 0;
                    x.FGiaTriNamThuNhat = 0;
                    x.FGiaTriNamThuHai = 0;
                    x.FGiaTriNamThuBa = 0;
                    x.FGiaTriNamThuTu = 0;
                    x.FGiaTriNamThuNam = 0;
                    x.FHanMucDauTuQP = 0;
                    x.FHanMucDauTuNN = 0;
                    x.FHanMucDauTuDP = 0;
                    x.FHanMucDauTuOrther = 0;
                    x.LuyKeVonNSQPDeNghiBoTri = 0;
                    x.LuyKeVonNSQPDaBoTri = 0;
                    x.FGiaTriBoTri = 0;
                    return x;
                }).ToList();

                foreach (var pr in lstLoaiCongTrinhparent)
                {
                    List<KH5NDXPrintDataExportModel> lstChilrent = lstDonViparent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.FHanMucDauTu != 0 || x.FHanMucDauTuQP != 0 || x.FHanMucDauTuNN != 0
                             || x.FHanMucDauTuNN != 0 || x.FHanMucDauTuOrther != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                             || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                             || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0 || x.LuyKeVonNSQPDaBoTri != 0)))
                    {
                        pr.FHanMucDauTu += item.FHanMucDauTu ?? 0;
                        pr.FTongSoNhuCau += item.FTongSoNhuCau ?? 0;
                        pr.FTongSo += item.FTongSo ?? 0;
                        pr.FGiaTriBoTri += item.FGiaTriBoTri ?? 0;
                        pr.FGiaTriNamThuNhat += item.FGiaTriNamThuNhat ?? 0;
                        pr.FGiaTriNamThuHai += item.FGiaTriNamThuHai ?? 0;
                        pr.FGiaTriNamThuBa += item.FGiaTriNamThuBa ?? 0;
                        pr.FGiaTriNamThuTu += item.FGiaTriNamThuTu ?? 0;
                        pr.FGiaTriNamThuNam += item.FGiaTriNamThuNam ?? 0;
                        pr.FHanMucDauTuQP += item.FHanMucDauTuQP ?? 0;
                        pr.FHanMucDauTuNN += item.FHanMucDauTuNN ?? 0;
                        pr.FHanMucDauTuDP += item.FHanMucDauTuDP ?? 0;
                        pr.FHanMucDauTuOrther += item.FHanMucDauTuOrther ?? 0;
                        pr.LuyKeVonNSQPDaBoTri += item.LuyKeVonNSQPDaBoTri ?? 0;
                        pr.LuyKeVonNSQPDeNghiBoTri += item.LuyKeVonNSQPDeNghiBoTri ?? 0;
                    }
                }

                foreach (var item in lstLoaiCongTrinhparent.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                            || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                            || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)))
                {
                    CalculateParentDeXuat(item, item, ItemsReportDeXuat);
                }

                List<KH5NDXPrintDataExportModel> lstDataChildrent = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)).ToList();

                List<KH5NDXPrintDataExportModel> lstDataParent = ItemsReportDeXuat.Where(x => x.LoaiParent.Equals(0)
                            && lstDataChildrent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh)).ToList();

                List<KH5NDXPrintDataExportModel> listGroup = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0
                                || lstDataParent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh))).ToList().GroupBy(x => new
                                {
                                    x.STT,
                                    x.IdLoaiCongTrinh,
                                    x.IdLoaiCongTrinhParent,
                                    x.SMaLoaiCongTrinh,
                                    x.STenDuAn,
                                    x.STenDonVi,
                                    x.SDiaDiem,
                                    x.SThoiGianThucHien,
                                    x.SGhiChu,
                                    x.Loai,
                                    x.LoaiParent,
                                    x.IsHangCha
                                }).Select(x => new KH5NDXPrintDataExportModel()
                                {
                                    STT = x.Key.STT,
                                    IdLoaiCongTrinh = x.Key.IdLoaiCongTrinh,
                                    IdLoaiCongTrinhParent = x.Key.IdLoaiCongTrinhParent,
                                    SMaLoaiCongTrinh = x.Key.SMaLoaiCongTrinh,
                                    STenDuAn = x.Key.STenDuAn,
                                    STenDonVi = x.Key.STenDonVi,
                                    SDiaDiem = x.Key.SDiaDiem,
                                    SThoiGianThucHien = x.Key.SThoiGianThucHien,
                                    FHanMucDauTu = x.Sum(rpt => rpt.FHanMucDauTu),
                                    FTongSoNhuCau = x.Sum(rpt => rpt.FTongSoNhuCau),
                                    FTongSo = x.Sum(rpt => rpt.FTongSo),
                                    FGiaTriNamThuNhat = x.Sum(rpt => rpt.FGiaTriNamThuNhat),
                                    FGiaTriNamThuHai = x.Sum(rpt => rpt.FGiaTriNamThuHai),
                                    FGiaTriNamThuBa = x.Sum(rpt => rpt.FGiaTriNamThuBa),
                                    FGiaTriNamThuTu = x.Sum(rpt => rpt.FGiaTriNamThuTu),
                                    FGiaTriNamThuNam = x.Sum(rpt => rpt.FGiaTriNamThuNam),
                                    FGiaTriBoTri = x.Sum(rpt => rpt.FGiaTriBoTri),
                                    SGhiChu = x.Key.SGhiChu,
                                    FHanMucDauTuQP = x.Sum(rpt => rpt.FHanMucDauTuQP),
                                    FHanMucDauTuNN = x.Sum(rpt => rpt.FHanMucDauTuNN),
                                    FHanMucDauTuDP = x.Sum(rpt => rpt.FHanMucDauTuDP),
                                    FHanMucDauTuOrther = x.Sum(rpt => rpt.FHanMucDauTuOrther),
                                    LuyKeVonNSQPDaBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDaBoTri),
                                    LuyKeVonNSQPDeNghiBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDeNghiBoTri),
                                    Loai = x.Key.Loai,
                                    LoaiParent = x.Key.LoaiParent,
                                    IsHangCha = x.Key.IsHangCha
                                }).OrderByDescending(x => x.IdLoaiCongTrinh).ToList();
                return listGroup;
            }
            catch (Exception ex)
            {
                return new List<KH5NDXPrintDataExportModel>();
            }
        }

        private List<KH5NDXPrintDataExportModel> HandleDataReportDeXuatDonVi(List<KH5NDXPrintDataExportModel> ItemsReportDeXuat)
        {
            try
            {
                List<KH5NDXPrintDataExportModel> lstItem = ItemsReportDeXuat.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
                List<KH5NDXPrintDataExportModel> lstItemLevel = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                dctItemLevel.Keys.Select(x =>
                {
                    List<KH5NDXPrintDataExportModel> lstItemStt = dctItemLevel[x].ToList();
                    lstItemStt.Select(y => { y.STT = (lstItemStt.IndexOf(y) + 1).ToString(); return y; }).ToList();
                    return x;
                }).ToList();

                foreach (var item in lstItemLevel)
                {
                    List<KH5NDXPrintDataExportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                foreach (var item in ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(2)))
                {
                    List<KH5NDXPrintDataExportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && !x.IsHangCha).ToList();
                    lstChildrent.Select(x =>
                    {
                        x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString());
                        return x;
                    }).ToList();
                }

                List<KH5NDXPrintDataExportModel> lstParentDonVi = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent == null && x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                lstParentDonVi.Select(x => { x.STT = (lstParentDonVi.IndexOf(x) + 1).ToString(); return x; }).ToList();
                lstParentDonVi.Select(item =>
                {
                    List<KH5NDXPrintDataExportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent == null && !x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x =>
                    {
                        x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString());
                        return x;
                    }).ToList();
                    return item;
                }).ToList();

                return ItemsReportDeXuat;
            }
            catch (Exception ex)
            {
                return new List<KH5NDXPrintDataExportModel>();
            }
        }

        private List<KH5NDXPrintDataExportModel> HandleDataReportDeXuatDonViNew(List<KH5NDXPrintDataExportModel> ItemsReportDeXuat)
        {
            try
            {
                var sTenDonVis = new HashSet<string>(ItemsReportDeXuat.Select(x => x.STenDonVi).Distinct());
                var lstLoaiCongTrinh = ItemsReportDeXuat.Select(x => x.sTenLoaiCongTrinh).Distinct();
                List<KH5NDXPrintDataExportModel> lstReport = new List<KH5NDXPrintDataExportModel>();
                var count = 1;
                var sSTT = 1;
                var sLevel = 1;
                foreach (var item in sTenDonVis)
                {
                    sLevel = 1;
                    lstReport.Add(new KH5NDXPrintDataExportModel()
                    {
                        STT = (count).ToString(),
                        STenDuAn = item,
                        IsHangCha = true,
                        LoaiParent = 1


                    });
                    foreach (var index in lstLoaiCongTrinh)
                    {
                        sSTT = 1;
                        var listItems = ItemsReportDeXuat.Where(x => x.STenDonVi == item && x.sTenLoaiCongTrinh == index).ToList();
                        if (listItems.Any())
                        {
                            lstReport.Add(new KH5NDXPrintDataExportModel()
                            {
                                STT = (count + "." + sLevel).ToString(),
                                sTenLoaiCongTrinh = index,
                                STenDuAn = index,
                                FHanMucDauTu = listItems.Sum(x => x.FHanMucDauTu),
                                FTongSo = listItems.Sum(x => x.FTongSo),
                                FGiaTriBoTri = listItems.Sum(x => x.FGiaTriBoTri),
                                fTongVonBoTri = listItems.Sum(x => x.FTongSo) + listItems.Sum(x => x.FGiaTriBoTri),
                                fHanmucNganhDX = listItems.Sum(x => x.fHanmucNganhDX),
                                fTongVonBoTriNganh = listItems.Sum(x => x.fTongVonBoTriNganh),
                                fVon5namNganhDX = listItems.Sum(x => x.fVon5namNganhDX),
                                fVonsaunamNganhDX = listItems.Sum(x => x.fVonsaunamNganhDX),
                                fHanmucCucTCDX = listItems.Sum(x => x.fHanmucCucTCDX),
                                fTongVonBoTriCuc = listItems.Sum(x => x.fTongVonBoTriCuc),
                                fVon5namCTCDX = listItems.Sum(x => x.fVon5namCTCDX),
                                fVonnamthunhatCTC = listItems.Sum(x => x.fVonnamthunhatCTC),
                                fVonsaunamCTCDexuat = listItems.Sum(x => x.fVonsaunamCTCDexuat),
                                fCucTCDeXuat = listItems.Sum(x => x.fCucTCDeXuat),
                                fDuKienBoTriNamThu2 = listItems.Sum(x => x.fDuKienBoTriNamThu2),
                                IsHangCha = true,
                                LoaiParent = 2,
                                STenDonVi = item

                            });

                            foreach (var itemReport in listItems)
                            {
                                itemReport.STT = sSTT.ToString();
                                itemReport.fTongVonBoTri = itemReport.FTongSo + itemReport.FGiaTriBoTri;
                                lstReport.Add(itemReport);
                                sSTT++;
                            }
                            
                            sLevel ++;

                            //lstReport.AddRange(listItems);
                        }

                    }
                   
                    lstReport.Where(x => x.IsHangCha && x.LoaiParent == 1 && x.STenDuAn.Equals(item))
                        .Select(s =>
                        {
                            s.FHanMucDauTu = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.FHanMucDauTu);
                            s.FTongSo = lstReport.Where(x =>x.LoaiParent ==2 && x.STenDonVi.Equals(item)).Sum(i => i.FTongSo);
                            s.FGiaTriBoTri = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.FTongSo);
                            s.fTongVonBoTri = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fTongVonBoTri);
                            s.fHanmucNganhDX = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fHanmucNganhDX);
                            s.fTongVonBoTriNganh = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fTongVonBoTriNganh);
                            s.fVon5namNganhDX = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fVon5namNganhDX);
                            s.fHanmucCucTCDX = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fHanmucCucTCDX);
                            s.fTongVonBoTriCuc = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fTongVonBoTriCuc);
                            s.fVon5namCTCDX = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fVon5namCTCDX);
                            s.fVonnamthunhatCTC = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fVonnamthunhatCTC);
                            s.fVonsaunamCTCDexuat = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fVonsaunamCTCDexuat);
                            s.fCucTCDeXuat = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fCucTCDeXuat);
                            s.fCucTCDeXuat = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fCucTCDeXuat);
                            s.fDuKienBoTriNamThu2 = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fDuKienBoTriNamThu2);
                            return s;
                        }).ToList();
                    
                    count++;
                }
                return lstReport;
            }catch(Exception ex)
            {
                return new List<KH5NDXPrintDataExportModel>();

            }
        }
        private List<KH5NDXPrintDataExportNewModel> HandleDataReportDeXuatChuyenTiepDonViNew(List<KH5NDXPrintDataExportNewModel> ItemsReportDeXuat)
        {
            try
            {
                var sTenDonVis = new HashSet<string>(ItemsReportDeXuat.Select(x => x.STenDonVi).Distinct());
                var lstLoaiCongTrinh = ItemsReportDeXuat.Select(x => x.sTenLoaiCongTrinh).Distinct();
                List<KH5NDXPrintDataExportNewModel> lstReport = new List<KH5NDXPrintDataExportNewModel>();
                var count = 1;
                var sSTT = 1;
                var sLevel = 1;
                foreach (var item in sTenDonVis)
                {
                    sLevel = 1;
                    lstReport.Add(new KH5NDXPrintDataExportNewModel()
                    {
                        STT = (count).ToString(),
                        STenDuAn = item,
                        IsHangCha = true,
                        LoaiParent = 1


                    });
                    foreach (var index in lstLoaiCongTrinh)
                    {
                        sSTT = 1;
                        var listItems = ItemsReportDeXuat.Where(x => x.STenDonVi == item && x.sTenLoaiCongTrinh == index).ToList();
                        if (listItems.Any())
                        {
                            lstReport.Add(new KH5NDXPrintDataExportNewModel()
                            {
                                STT = (count + "." + sLevel).ToString(),
                                sTenLoaiCongTrinh = index,
                                STenDuAn = index,
                                fNSQP = listItems.Sum(x => x.fNSQP),
                                fChiPhiDuPhong = listItems.Sum(x => x.fChiPhiDuPhong),
                                fVonBoTriHetNam = listItems.Sum(x => x.fVonBoTriHetNam),
                                fSoConLaiPhaiBoTri = listItems.Sum(x => x.fSoConLaiPhaiBoTri),
                                fCucTCdexuat = listItems.Sum(x => x.fCucTCdexuat),
                                fBoDaGiaoDuToan = listItems.Sum(x => x.fBoDaGiaoDuToan),
                                fDuKienbotrinamthu2 = listItems.Sum(x => x.fDuKienbotrinamthu2),
                                fSoConLaiConBotri = listItems.Sum(x => x.fSoConLaiConBotri),
                                fSoTienConLaiChuaDeXuat = listItems.Sum(x => x.fSoTienConLaiChuaDeXuat),
                                IsHangCha = true,
                                LoaiParent = 2,
                                STenDonVi = item

                            });

                            foreach (var itemReport in listItems)
                            {
                                itemReport.STT = sSTT.ToString();
                                lstReport.Add(itemReport);
                                sSTT++;
                            }
                            
                            sLevel ++;

                            //lstReport.AddRange(listItems);
                        }

                    }
                   
                    lstReport.Where(x => x.IsHangCha && x.LoaiParent == 1 && x.STenDuAn.Equals(item))
                        .Select(s =>
                        {
                            s.fNSQP = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fNSQP);
                            s.fChiPhiDuPhong = lstReport.Where(x =>x.LoaiParent ==2 && x.STenDonVi.Equals(item)).Sum(i => i.fChiPhiDuPhong);
                            s.fVonBoTriHetNam = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fVonBoTriHetNam);
                            s.fSoConLaiPhaiBoTri = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fSoConLaiPhaiBoTri);
                            s.fCucTCdexuat = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fCucTCdexuat);
                            s.fBoDaGiaoDuToan = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fBoDaGiaoDuToan);
                            s.fDuKienbotrinamthu2 = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fDuKienbotrinamthu2);
                            s.fSoConLaiConBotri = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fSoConLaiConBotri);
                            s.fSoTienConLaiChuaDeXuat = lstReport.Where(x => x.LoaiParent == 2 && x.STenDonVi.Equals(item)).Sum(i => i.fSoTienConLaiChuaDeXuat);
                            return s;
                        }).ToList();
                    
                    count++;
                }
                return lstReport;
            }catch(Exception ex)
            {
                return new List<KH5NDXPrintDataExportNewModel>();

            }
        }


        private void CalculateParentDeXuat(KH5NDXPrintDataExportModel currentItem, KH5NDXPrintDataExportModel seftItem, List<KH5NDXPrintDataExportModel> ItemsReportDeXuat)
        {
            var parrentItem = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinh == currentItem.IdLoaiCongTrinhParent).FirstOrDefault();
            if (parrentItem == null) return;
            parrentItem.FHanMucDauTu += seftItem.FHanMucDauTu;
            parrentItem.FTongSoNhuCau += seftItem.FTongSoNhuCau;
            parrentItem.FTongSo += seftItem.FTongSo;
            parrentItem.FGiaTriNamThuNhat += seftItem.FGiaTriNamThuNhat;
            parrentItem.FGiaTriNamThuHai += seftItem.FGiaTriNamThuHai;
            parrentItem.FGiaTriNamThuBa += seftItem.FGiaTriNamThuBa;
            parrentItem.FGiaTriNamThuTu += seftItem.FGiaTriNamThuTu;
            parrentItem.FGiaTriNamThuNam += seftItem.FGiaTriNamThuNam;
            parrentItem.FGiaTriBoTri += seftItem.FGiaTriBoTri;
            parrentItem.TongLuyKe += seftItem.TongLuyKe;
            parrentItem.LuyKeVonNSQPDaBoTri += seftItem.LuyKeVonNSQPDaBoTri;
            parrentItem.LuyKeVonNSQPDeNghiBoTri += seftItem.LuyKeVonNSQPDeNghiBoTri;
            parrentItem.FHanMucDauTuQP += seftItem.FHanMucDauTuQP;
            parrentItem.FHanMucDauTuNN += seftItem.FHanMucDauTuNN;
            parrentItem.FHanMucDauTuDP += seftItem.FHanMucDauTuDP;
            parrentItem.FHanMucDauTuOrther += seftItem.FHanMucDauTuOrther;
            parrentItem.LuyKeVonNSQPDeNghiBoTri += seftItem.LuyKeVonNSQPDeNghiBoTri;
            parrentItem.LuyKeVonNSQPDaBoTri += seftItem.LuyKeVonNSQPDaBoTri;
            CalculateParentDeXuat(parrentItem, seftItem, ItemsReportDeXuat);
        }

        private List<KH5NDXPrintDataDieuChinhExportModel> HandleDataDeXuatDieuChinh(List<KH5NDXPrintDataDieuChinhExportModel> ItemsReportDeXuatDieuChinh, bool isTongHop)
        {
            try
            {
                if (isTongHop)
                {
                    ItemsReportDeXuatDieuChinh.Select(x => { x.STT = (ItemsReportDeXuatDieuChinh.IndexOf(x) + 1).ToString(); return x; }).ToList();
                }
                else
                {
                    List<KH5NDXPrintDataDieuChinhExportModel> lstDv = ItemsReportDeXuatDieuChinh.Where(x => x.IsHangCha).ToList();
                    lstDv.Select(x => { x.STT = (lstDv.IndexOf(x) + 1).ToString(); return x; }).ToList();
                    lstDv.Select(item =>
                    {
                        List<KH5NDXPrintDataDieuChinhExportModel> lstChildrent = ItemsReportDeXuatDieuChinh.Where(x => !x.IsHangCha && x.IdDonViQuanLy == item.IdDonViQuanLy).ToList();
                        lstChildrent.Select(x => { x.STT = (string.Format("{0}.{1}", item.STT, lstChildrent.IndexOf(x) + 1)).ToString(); return x; }).ToList();
                        return item;
                    }).ToList();
                }

                return ItemsReportDeXuatDieuChinh;
            }
            catch (Exception ex)
            {
                return new List<KH5NDXPrintDataDieuChinhExportModel>();
            }
        }

        private List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> HandleDataReportDeXuatChuyenTiepDonVi(List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> ItemsReportDeXuat)
        {
            try
            {
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstItem = ItemsReportDeXuat.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstItemLevel = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                dctItemLevel.Keys.Select(x =>
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstItemStt = dctItemLevel[x].ToList();
                    lstItemStt.Select(y => { y.STT = (lstItemStt.IndexOf(y) + 1).ToString(); return x; }).ToList();
                    return x;
                }).ToList();

                foreach (var item in lstItemLevel)
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                foreach (var item in ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(2)))
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && !x.IsHangCha).ToList();
                    lstChildrent.Select(x =>
                    {
                        x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString());
                        return x;
                    }).ToList();
                }

                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstParentDonVi = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent == null && x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                lstParentDonVi.Select(x => { x.STT = (lstParentDonVi.IndexOf(x) + 1).ToString(); return x; }).ToList();
                lstParentDonVi.Select(item =>
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent == null && !x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x =>
                    {
                        x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString());
                        return x;
                    }).ToList();
                    return item;
                }).ToList();

                return ItemsReportDeXuat;
            }
            catch (Exception ex)
            {
                return ItemsReportDeXuat;
            }
        }

        private List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> HandleDataReportDeXuatChuyenTiepTongHop(List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> ItemsReportDeXuat)
        {
            try
            {
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstItem = ItemsReportDeXuat.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstItemLevel = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                dctItemLevel.Keys.Select(x =>
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstItemStt = dctItemLevel[x].ToList();
                    lstItemStt.Select(y => { y.STT = (lstItemStt.IndexOf(y) + 1).ToString(); return y; }).ToList();
                    return x;
                }).ToList();

                foreach (var item in lstItemLevel)
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstChildrent = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && !x.IsHangCha).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstLctParent = ItemsReportDeXuat.Where(x => x.LoaiParent.Equals(0)).ToList();
                var dctItemParent = ItemsReportDeXuat.Where(x => !x.IsHangCha && x.IdLoaiCongTrinh.HasValue
                    && lstLctParent.Select(y => y.IdLoaiCongTrinh).Contains(x.IdLoaiCongTrinh)).GroupBy(z => z.IdLoaiCongTrinh).ToDictionary(z => z.Key.ToString(), z => z.ToList());
                foreach (var item in dctItemParent.Keys)
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> itemStt = dctItemParent[item];
                    itemStt.Select(x => { x.STT = string.Format("{0}", (itemStt.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                return ItemsReportDeXuat;
            }
            catch (Exception ex)
            {
                return ItemsReportDeXuat;
            }
        }

        private List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> CalculateDataReportDeXuatChuyenTiepDonVi(List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> ItemsReportDeXuat)
        {
            try
            {
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstDonViparent = ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstLoaiCongTrinhparent = ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).ToList();

                ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.FHanMucDauTu = 0;
                    x.FTongSoNhuCau = 0;
                    x.FTongSo = 0;
                    x.FGiaTriNamThuNhat = 0;
                    x.FGiaTriNamThuHai = 0;
                    x.FGiaTriNamThuBa = 0;
                    x.FGiaTriNamThuTu = 0;
                    x.FGiaTriNamThuNam = 0;
                    x.FHanMucDauTuQP = 0;
                    x.FHanMucDauTuNN = 0;
                    x.FHanMucDauTuDP = 0;
                    x.FHanMucDauTuOrther = 0;
                    x.LuyKeVonNSQPDeNghiBoTri = 0;
                    x.LuyKeVonNSQPDaBoTri = 0;
                    x.FGiaTriBoTri = 0;
                    return x;
                }).ToList();

                foreach (var pr in lstLoaiCongTrinhparent)
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstChilrent = lstDonViparent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.FHanMucDauTu != 0 || x.FHanMucDauTuQP != 0 || x.FHanMucDauTuNN != 0
                             || x.FHanMucDauTuNN != 0 || x.FHanMucDauTuOrther != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                             || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                             || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0 || x.LuyKeVonNSQPDaBoTri != 0)))
                    {
                        pr.FHanMucDauTu += item.FHanMucDauTu ?? 0;
                        pr.FTongSoNhuCau += item.FTongSoNhuCau ?? 0;
                        pr.FTongSo += item.FTongSo ?? 0;
                        pr.FGiaTriBoTri += item.FGiaTriBoTri ?? 0;
                        pr.FGiaTriNamThuNhat += item.FGiaTriNamThuNhat ?? 0;
                        pr.FGiaTriNamThuHai += item.FGiaTriNamThuHai ?? 0;
                        pr.FGiaTriNamThuBa += item.FGiaTriNamThuBa ?? 0;
                        pr.FGiaTriNamThuTu += item.FGiaTriNamThuTu ?? 0;
                        pr.FGiaTriNamThuNam += item.FGiaTriNamThuNam ?? 0;
                        pr.FHanMucDauTuQP += item.FHanMucDauTuQP ?? 0;
                        pr.FHanMucDauTuNN += item.FHanMucDauTuNN ?? 0;
                        pr.FHanMucDauTuDP += item.FHanMucDauTuDP ?? 0;
                        pr.FHanMucDauTuOrther += item.FHanMucDauTuOrther ?? 0;
                        pr.LuyKeVonNSQPDaBoTri += item.LuyKeVonNSQPDaBoTri ?? 0;
                        pr.LuyKeVonNSQPDeNghiBoTri += item.LuyKeVonNSQPDeNghiBoTri ?? 0;
                    }
                }

                foreach (var item in lstLoaiCongTrinhparent.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                            || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                            || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)))
                {
                    CalculateParentDeXuatChuyenTiep(item, item, ItemsReportDeXuat);
                }

                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstDataChildrent = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)).ToList();

                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstDataParent = ItemsReportDeXuat.Where(x => x.LoaiParent.Equals(0)
                            && lstDataChildrent.Select(y => x.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh)).ToList();

                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> listGroup = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0
                                || lstDataParent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh))).ToList().GroupBy(x => new
                                {
                                    x.STT,
                                    x.IdLoaiCongTrinh,
                                    x.IdLoaiCongTrinhParent,
                                    x.SMaLoaiCongTrinh,
                                    x.STenDuAn,
                                    x.STenDonVi,
                                    x.SDiaDiem,
                                    x.SThoiGianThucHien,
                                    x.SGhiChu,
                                    x.Loai,
                                    x.LoaiParent,
                                    x.IsHangCha,
                                    x.SSoQuyetDinh,
                                    x.DNgayQuyetDinh
                                }).Select(x => new VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel()
                                {
                                    STT = x.Key.STT,
                                    IdLoaiCongTrinh = x.Key.IdLoaiCongTrinh,
                                    IdLoaiCongTrinhParent = x.Key.IdLoaiCongTrinhParent,
                                    SMaLoaiCongTrinh = x.Key.SMaLoaiCongTrinh,
                                    STenDuAn = x.Key.STenDuAn,
                                    STenDonVi = x.Key.STenDonVi,
                                    SDiaDiem = x.Key.SDiaDiem,
                                    SThoiGianThucHien = x.Key.SThoiGianThucHien,
                                    FHanMucDauTu = x.Sum(rpt => rpt.FHanMucDauTu),
                                    FTongSoNhuCau = x.Sum(rpt => rpt.FTongSoNhuCau),
                                    FTongSo = x.Sum(rpt => rpt.FTongSo),
                                    FGiaTriNamThuNhat = x.Sum(rpt => rpt.FGiaTriNamThuNhat),
                                    FGiaTriNamThuHai = x.Sum(rpt => rpt.FGiaTriNamThuHai),
                                    FGiaTriNamThuBa = x.Sum(rpt => rpt.FGiaTriNamThuBa),
                                    FGiaTriNamThuTu = x.Sum(rpt => rpt.FGiaTriNamThuTu),
                                    FGiaTriNamThuNam = x.Sum(rpt => rpt.FGiaTriNamThuNam),
                                    FGiaTriBoTri = x.Sum(rpt => rpt.FGiaTriBoTri),
                                    SGhiChu = x.Key.SGhiChu,
                                    FHanMucDauTuQP = x.Sum(rpt => rpt.FHanMucDauTuQP),
                                    FHanMucDauTuNN = x.Sum(rpt => rpt.FHanMucDauTuNN),
                                    FHanMucDauTuDP = x.Sum(rpt => rpt.FHanMucDauTuDP),
                                    FHanMucDauTuOrther = x.Sum(rpt => rpt.FHanMucDauTuOrther),
                                    LuyKeVonNSQPDaBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDaBoTri),
                                    LuyKeVonNSQPDeNghiBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDeNghiBoTri),
                                    Loai = x.Key.Loai,
                                    LoaiParent = x.Key.LoaiParent,
                                    IsHangCha = x.Key.IsHangCha,
                                    SSoQuyetDinh = x.Key.SSoQuyetDinh,
                                    DNgayQuyetDinh = x.Key.DNgayQuyetDinh
                                }).ToList();
                foreach(var item in listGroup)
                {
                    if(!String.IsNullOrEmpty(item.SSoQuyetDinh) || !String.IsNullOrEmpty(item.DNgayQuyetDinh.ToString()))
                    {
                        item.SSoQuyetDinhNgayQuyetDinh = item.SSoQuyetDinh + "," + item.DNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                    }                    
                }
                return listGroup;
            }
            catch (Exception ex)
            {
                return ItemsReportDeXuat;
            }
        }

        private List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> CalculateDataReportDeXuatChuyenTiepTongHop(List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> ItemsReportDeXuat)
        {
            try
            {
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> childrent = ItemsReportDeXuat.Where(x => !x.IsHangCha).ToList();
                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> parent = ItemsReportDeXuat.Where(x => x.IsHangCha && (x.LoaiParent.Equals(1) || x.LoaiParent.Equals(0))).ToList();

                ItemsReportDeXuat.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.FHanMucDauTu = 0;
                    x.FTongSoNhuCau = 0;
                    x.FTongSo = 0;
                    x.FGiaTriNamThuNhat = 0;
                    x.FGiaTriNamThuHai = 0;
                    x.FGiaTriNamThuBa = 0;
                    x.FGiaTriNamThuTu = 0;
                    x.FGiaTriNamThuNam = 0;
                    //x.TongLuyKe = 0;
                    x.LuyKeVonNSQPDaBoTri = 0;
                    x.LuyKeVonNSQPDeNghiBoTri = 0;
                    x.FHanMucDauTuQP = 0;
                    x.FHanMucDauTuNN = 0;
                    x.FHanMucDauTuDP = 0;
                    x.FHanMucDauTuOrther = 0;
                    x.FGiaTriBoTri = 0;
                    return x;
                }).ToList();

                foreach (var pr in parent)
                {
                    List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstChilrent = childrent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                             || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                             || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0  || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)))
                    {
                        pr.FHanMucDauTu += item.FHanMucDauTu ?? 0;
                        pr.FTongSoNhuCau += item.FTongSoNhuCau ?? 0;
                        pr.FTongSo += item.FTongSo ?? 0;
                        pr.FGiaTriNamThuNhat += item.FGiaTriNamThuNhat ?? 0;
                        pr.FGiaTriNamThuHai += item.FGiaTriNamThuHai ?? 0;
                        pr.FGiaTriNamThuBa += item.FGiaTriNamThuBa ?? 0;
                        pr.FGiaTriNamThuTu += item.FGiaTriNamThuTu ?? 0;
                        pr.FGiaTriNamThuNam += item.FGiaTriNamThuNam ?? 0;
                        pr.FGiaTriBoTri += item.FGiaTriBoTri ?? 0;
                        //pr.TongLuyKe += item.TongLuyKe ?? 0;
                        pr.LuyKeVonNSQPDaBoTri += item.LuyKeVonNSQPDaBoTri ?? 0;
                        pr.LuyKeVonNSQPDeNghiBoTri += item.LuyKeVonNSQPDeNghiBoTri ?? 0;
                        pr.FHanMucDauTuQP += item.FHanMucDauTuQP ?? 0;
                        pr.FHanMucDauTuNN += item.FHanMucDauTuNN ?? 0;
                        pr.FHanMucDauTuDP += item.FHanMucDauTuDP ?? 0;
                        pr.FHanMucDauTuOrther += item.FHanMucDauTuOrther ?? 0;
                    }
                }

                foreach (var item in parent.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                            || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0 || x.FGiaTriBoTri != 0
                            || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.TongLuyKe != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0
                            || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)))
                {
                    CalculateParentDeXuatChuyenTiep(item, item, ItemsReportDeXuat);
                }

                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstDataChildrent = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0)).ToList();

                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> lstDataParent = ItemsReportDeXuat.Where(x => x.LoaiParent.Equals(0)
                            && lstDataChildrent.Select(y => x.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh)).ToList();

                List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> listGroup = ItemsReportDeXuat.Where(x => (x.FHanMucDauTu != 0 || x.FTongSoNhuCau != 0 || x.FTongSo != 0
                                || x.FGiaTriNamThuNhat != 0 || x.FGiaTriNamThuHai != 0 || x.FGiaTriNamThuBa != 0
                                || x.FGiaTriNamThuTu != 0 || x.FGiaTriNamThuNam != 0 || x.LuyKeVonNSQPDaBoTri != 0 || x.LuyKeVonNSQPDeNghiBoTri != 0
                                || lstDataParent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh))).ToList().GroupBy(x => new
                                {
                                    x.STT,
                                    x.IdLoaiCongTrinh,
                                    x.IdLoaiCongTrinhParent,
                                    x.SMaLoaiCongTrinh,
                                    x.STenDuAn,
                                    x.STenDonVi,
                                    x.SDiaDiem,
                                    x.SThoiGianThucHien,
                                    x.SGhiChu,
                                    x.Loai,
                                    x.LoaiParent,
                                    x.IsHangCha,
                                    x.SSoQuyetDinh,
                                    x.DNgayQuyetDinh
                                }).Select(x => new VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel()
                                {
                                    STT = x.Key.STT,
                                    IdLoaiCongTrinh = x.Key.IdLoaiCongTrinh,
                                    IdLoaiCongTrinhParent = x.Key.IdLoaiCongTrinhParent,
                                    SMaLoaiCongTrinh = x.Key.SMaLoaiCongTrinh,
                                    STenDuAn = x.Key.STenDuAn,
                                    STenDonVi = x.Key.STenDonVi,
                                    SDiaDiem = x.Key.SDiaDiem,
                                    SThoiGianThucHien = x.Key.SThoiGianThucHien,
                                    FHanMucDauTu = x.Sum(rpt => rpt.FHanMucDauTu),
                                    FTongSoNhuCau = x.Sum(rpt => rpt.FTongSoNhuCau),
                                    FTongSo = x.Sum(rpt => rpt.FTongSo),
                                    FGiaTriNamThuNhat = x.Sum(rpt => rpt.FGiaTriNamThuNhat),
                                    FGiaTriNamThuHai = x.Sum(rpt => rpt.FGiaTriNamThuHai),
                                    FGiaTriNamThuBa = x.Sum(rpt => rpt.FGiaTriNamThuBa),
                                    FGiaTriNamThuTu = x.Sum(rpt => rpt.FGiaTriNamThuTu),
                                    FGiaTriNamThuNam = x.Sum(rpt => rpt.FGiaTriNamThuNam),
                                    FGiaTriBoTri = x.Sum(rpt => rpt.FGiaTriBoTri),
                                    SGhiChu = x.Key.SGhiChu,
                                    FHanMucDauTuQP = x.Sum(rpt => rpt.FHanMucDauTuQP),
                                    FHanMucDauTuNN = x.Sum(rpt => rpt.FHanMucDauTuNN),
                                    FHanMucDauTuDP = x.Sum(rpt => rpt.FHanMucDauTuDP),
                                    FHanMucDauTuOrther = x.Sum(rpt => rpt.FHanMucDauTuOrther),
                                    LuyKeVonNSQPDaBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDaBoTri),
                                    LuyKeVonNSQPDeNghiBoTri = x.Sum(rpt => rpt.LuyKeVonNSQPDeNghiBoTri),
                                    Loai = x.Key.Loai,
                                    LoaiParent = x.Key.LoaiParent,
                                    IsHangCha = x.Key.IsHangCha,
                                    SSoQuyetDinh = x.Key.SSoQuyetDinh,
                                    DNgayQuyetDinh = x.Key.DNgayQuyetDinh
                                }).ToList();
                foreach (var item in listGroup)
                {
                    if (!String.IsNullOrEmpty(item.SSoQuyetDinh) || !String.IsNullOrEmpty(item.DNgayQuyetDinh.ToString()))
                    {
                        item.SSoQuyetDinhNgayQuyetDinh = item.SSoQuyetDinh + "," + item.DNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                    }
                }
                return listGroup;
            }
            catch (Exception ex)
            {
                return ItemsReportDeXuat;
            }
        }

        private void CalculateParentDeXuatChuyenTiep(VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel currentItem, VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel seftItem, List<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> ItemsReportDeXuat)
        {
            var parrentItem = ItemsReportDeXuat.Where(x => x.IdLoaiCongTrinh == currentItem.IdLoaiCongTrinhParent).FirstOrDefault();
            if (parrentItem == null) return;
            parrentItem.FHanMucDauTu += seftItem.FHanMucDauTu;
            parrentItem.FTongSoNhuCau += seftItem.FTongSoNhuCau;
            parrentItem.FTongSo += seftItem.FTongSo;
            parrentItem.FGiaTriNamThuNhat += seftItem.FGiaTriNamThuNhat;
            parrentItem.FGiaTriNamThuHai += seftItem.FGiaTriNamThuHai;
            parrentItem.FGiaTriNamThuBa += seftItem.FGiaTriNamThuBa;
            parrentItem.FGiaTriNamThuTu += seftItem.FGiaTriNamThuTu;
            parrentItem.FGiaTriNamThuNam += seftItem.FGiaTriNamThuNam;
            parrentItem.FGiaTriBoTri += seftItem.FGiaTriBoTri;
            //parrentItem.TongLuyKe += seftItem.TongLuyKe;
            parrentItem.LuyKeVonNSQPDaBoTri += seftItem.LuyKeVonNSQPDaBoTri;
            parrentItem.LuyKeVonNSQPDeNghiBoTri += seftItem.LuyKeVonNSQPDeNghiBoTri;
            parrentItem.FHanMucDauTuQP += seftItem.FHanMucDauTuQP;
            parrentItem.FHanMucDauTuNN += seftItem.FHanMucDauTuNN;
            parrentItem.FHanMucDauTuDP += seftItem.FHanMucDauTuDP;
            parrentItem.FHanMucDauTuOrther += seftItem.FHanMucDauTuOrther;
            parrentItem.LuyKeVonNSQPDeNghiBoTri += seftItem.LuyKeVonNSQPDeNghiBoTri;
            parrentItem.LuyKeVonNSQPDaBoTri += seftItem.LuyKeVonNSQPDaBoTri;
            CalculateParentDeXuatChuyenTiep(parrentItem, seftItem, ItemsReportDeXuat);
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
    }
}