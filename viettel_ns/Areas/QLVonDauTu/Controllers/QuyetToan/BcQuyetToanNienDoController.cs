using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.QLVonDauTu.Model.QuyetToan;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;
using static VTS.QLNS.CTC.App.Service.UserFunction.FormatNumber;
using VTS.QLNS.CTC.App.Service.UserFunction;
using System.Xml.Linq;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class BcQuyetToanNienDoController : FlexcelReportController
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private BcQuyetToanNienDoModel _model = new BcQuyetToanNienDoModel();
        private static string[] _lstDonViExclude = new string[] { "0", "1" };
        private string pathString = System.Configuration.ConfigurationManager.AppSettings["FtpPath"];
        private string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FtpUsername"];
        private string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        private const string DU_AN_MO_MOI = "Dự án mở mới";
        private const string DU_AN_CHUYEN_TIEP = "Dự án chuyển tiếp";
        private const string sControlName = "BcQuyetToanNienDo";

        // GET: QLVonDauTu/BcQuyetToanNienDo
        #region View
        public ActionResult Index()
        {
            //ViewBag.drpDonViQuanLy = CommonFunction.GetDataDropDownDonViQuanLy(_lstDonViExclude);
            //ViewBag.drpNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            var listDonViQuanLy = CommonFunction.GetDataDropDownDonViQuanLy();
            var allDonVi = listDonViQuanLy.ToList();
            allDonVi.Insert(0, new SelectListItem() { Text = "--Tất cả--", Value = "-1|-1" });
            ViewBag.drpDonViQuanLy = allDonVi.ToSelectList();

            var listNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            var allNguonVon = listNguonVon.ToList();
            allNguonVon.Insert(0, new SelectListItem() { Text = "--Tất cả--", Value = "-1" });
            ViewBag.drpNguonVon = allNguonVon.ToSelectList();

            VdtQtBcQuyetToanNienDoPagingModel data = new VdtQtBcQuyetToanNienDoPagingModel();
            data._paging.CurrentPage = 1;
            //string iIdMaDonVi = "";
            //Guid? iIDQuyetToan = Guid.Empty;
            //int iNamKeHoach = 0; 
            //int iIdNguonVon = 0; 
            //int iCoQuanThanhToan = 0; 
            data.lstData = _model.GetPagingIndex(ref data._paging);
            //  data = _model.GetQuyetToanVonNam_PhanTich(iIDQuyetToan,iIdMaDonVi, iNamKeHoach, iIdNguonVon, iCoQuanThanhToan)  ;

            List<Dropdown_Find> thanhtoan = new List<Dropdown_Find>()
            {
               new Dropdown_Find()
               {
                   valueId =1,
                   labelName = "Thanh toán"
               },
                new Dropdown_Find()
               {
                   valueId =2,
                   labelName = "Tạm ứng"
                }
            };
            List<Dropdown_Find> lstloaithanhtoan = thanhtoan;
            ViewBag.ListLoaiThanhToan = lstloaithanhtoan;
            GetDropdownThanhToan();
            GetDropdownDonVi();
            GetDropdownNguonVon();
            GetListDropDownNamKeHoach();
            return View(data);
        }

        public ActionResult Update(Guid? id, int bIsViewDetail = 0)
        {
            VDT_QT_BCQuyetToanNienDo data = new VDT_QT_BCQuyetToanNienDo();
            var drpNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            var drpCoQuanThanhToan = _model.GetCoQuanThanhToan();
            var drpLoaiThanhToan = _model.GetLoaiThanhToan();
            ViewBag.drpNguonVon = drpNguonVon;
            ViewBag.drpCoQuanThanhToan = drpCoQuanThanhToan;
            ViewBag.drpLoaiThanhToan = drpLoaiThanhToan;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                data = _model.GetBaoCaoQuyetToanById(id.Value);
            }
            ViewBag.dNgayDeNghi = data.dNgayDeNghi.HasValue ? data.dNgayDeNghi.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.bIsViewDetail = bIsViewDetail;
            return View(data);
        }

        public ActionResult Report()
        {
            return View();
        }
        #endregion

        #region PartialView
        [HttpPost]
        public ActionResult QuyetToanNienDoView(PagingInfo _paging, string iIdMaDonViQuanLy, int? iIdNguonVon, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo, int? iNamKeHoach, string txtSoChungTu)
        {
            //ViewBag.drpDonViQuanLy = CommonFunction.GetDataDropDownDonViQuanLy(_lstDonViExclude);
            //ViewBag.drpNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            var listDonViQuanLy = CommonFunction.GetDataDropDownDonViQuanLy();
            var allDonVi = listDonViQuanLy.ToList();
            allDonVi.Insert(0, new SelectListItem() { Text = "--Tất cả--", Value = "-1|-1" });
            ViewBag.drpDonViQuanLy = allDonVi.ToSelectList();

            //ViewBag.drpNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            var listNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            var allNguonVon = listNguonVon.ToList();
            allNguonVon.Insert(0, new SelectListItem() { Text = "--Tất cả--", Value = "-1" });
            ViewBag.drpNguonVon = allNguonVon.ToSelectList();

            VdtQtBcQuyetToanNienDoPagingModel data = new VdtQtBcQuyetToanNienDoPagingModel();
            data._paging.CurrentPage = 1;

            List<Dropdown_Find> thanhtoan = new List<Dropdown_Find>()
            {
               new Dropdown_Find()
               {
                   valueId = 0,
                   labelName = "--Tất cả--"
               },
               new Dropdown_Find()
               {
                   valueId =1,
                   labelName = "Thanh toán"
               },
                new Dropdown_Find()
               {
                   valueId =2,
                   labelName = "Tạm ứng"
                }
            };
            List<Dropdown_Find> lstloaithanhtoan = thanhtoan;
            ViewBag.ListLoaiThanhToan = lstloaithanhtoan;
            data.lstData = _model.GetPagingIndex(ref data._paging, iIdMaDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, txtSoChungTu);
            return PartialView("_list", data);
        }

        [HttpPost]
        public ActionResult QuyetToanNienDoKHVN(Guid? iIDBcQuyetToan, string sMaDonVi, int? iNamKeHoach, int? iIDNguonVonID)
        {
            if (string.IsNullOrEmpty(sMaDonVi) || !iNamKeHoach.HasValue || !iIDNguonVonID.HasValue)
            {
                return PartialView("_listKeHoachVonNam", new List<BcquyetToanNienDoVonNamChiTietViewModel>());
            }
            var data = _model.GetQuyetToanVonNam(iIDBcQuyetToan, sMaDonVi, iNamKeHoach.Value, iIDNguonVonID.Value);
            return PartialView("_listKeHoachVonNam", data);
        }

        [HttpPost]
        public ActionResult QuyetToanNienDoKHVN_PhanTich(Guid? iIDBcQuyetToan, string sMaDonVi, int? iNamKeHoach, int? iIDNguonVonID)
        {
            if (string.IsNullOrEmpty(sMaDonVi) || !iNamKeHoach.HasValue || !iIDNguonVonID.HasValue)
            {
                return PartialView("_listKeHoachVonNam_PhanTich", new List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel>());
            }
            var data = _model.GetQuyetToanVonNam_PhanTich(iIDBcQuyetToan, sMaDonVi, iNamKeHoach.Value, iIDNguonVonID.Value);
            return PartialView("_listKeHoachVonNam_PhanTich", data);
        }

        [HttpPost]
        public ActionResult QuyetToanNienDoKHU(Guid? iIDBcQuyetToan, string sMaDonVi, int? iNamKeHoach, int? iIDNguonVonID)
        {
            if (string.IsNullOrEmpty(sMaDonVi) || !iNamKeHoach.HasValue || !iIDNguonVonID.HasValue)
            {
                return PartialView("_listKeHoachVonUng", new List<BcquyetToanNienDoVonUngChiTietViewModel>());
            }
            var data = _model.GetQuyetToanVonUng(iIDBcQuyetToan, sMaDonVi, iNamKeHoach.Value, iIDNguonVonID.Value);
            return PartialView("_listKeHoachVonUng", data);
        }
        #endregion

        #region Event
        [HttpGet]
        public JsonResult DeleteBCQuyetToanNienDo(Guid iId)
        {
            return Json(new { bIsComplete = _model.DeleteBCQuyetToanNienDo(iId) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data)
        {
            if (data.iID_BCQuyetToanNienDoID == Guid.Empty)
                return Json(new { iIdBcQuyetToanNienDoId = _model.InsertBcQuyetToanNienDo(data, Username) }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { iIdBcQuyetToanNienDoId = _model.UpdateBcQuyetToanNienDo(data, Username) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ViewInBaoCao()
        {
            VDTQtBcquyetToanNienDoExportModel vm = new VDTQtBcquyetToanNienDoExportModel();
            try
            {
                ViewBag.ListDonViQuanLy = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
                ViewBag.iNamLamViec = PhienLamViec.iNamLamViec;

                List<ComBoBoxItem> lstLoaiChungTu = new List<ComBoBoxItem>()
                {
                    new ComBoBoxItem(){DisplayItem = "Tạm ứng", ValueItem = "1"},
                    new ComBoBoxItem(){DisplayItem = "Thanh toán", ValueItem = "2"}
                };

                ViewBag.lstLoaiChungTu = lstLoaiChungTu.ToSelectList("ValueItem", "DisplayItem");

                IEnumerable<NS_NguonNganSach> lstDMNguonNganSach = _vdtService.GetListDMNguonNganSach();
                ViewBag.lstNguonVon = lstDMNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");


                List<ComBoBoxItem> lstDonViTinh = new List<ComBoBoxItem>()
                {
                    new ComBoBoxItem(){DisplayItem = "Đồng", ValueItem = "1"},
                    new ComBoBoxItem(){DisplayItem = "Nghìn đồng", ValueItem = "1000"},
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
        public JsonResult PrintBaoCao(VdtQtBcQuyetToanNienDoPrintDataExportModel dataPrintReport, bool isPdf)
        {
            try
            {
                string ErrMess = string.Empty;
                if (dataPrintReport == null)
                {
                    ErrMess = "Lỗi in báo cáo";
                }
                VDT_QT_BCQuyetToanNienDo data = _vdtService.GetDataBCQuyetToanNienDo(dataPrintReport.ILoaiThanhToan, dataPrintReport.iID_DonViQuanLyID, dataPrintReport.IIdNguonVonId, dataPrintReport.INamKeHoach);
                
                if (dataPrintReport.ILoaiThanhToan == 1)
                {
                    List<BcquyetToanNienDoVonUngChiTietViewModel> dataReportParent = new List<BcquyetToanNienDoVonUngChiTietViewModel>();
                    if (data != null) dataReportParent = _vdtService.GetQuyetToanVonUngById(data.iID_BCQuyetToanNienDoID);
                    if(dataReportParent == null || dataReportParent.Count == 0)
                    {
                        var objDonVi = _vdtService.GetDonViQuanLyById(dataPrintReport.iID_DonViQuanLyID);
                        dataReportParent = _vdtService.GetQuyetToanVonUng(objDonVi.iID_MaDonVi, dataPrintReport.IIdNguonVonId, dataPrintReport.INamKeHoach);
                    }
                    ExcelFile xls = CreateReport(dataReportParent, dataPrintReport, (isPdf ? ExportType.PDF : ExportType.EXCEL));
                    TempData["DataReportXls"] = xls;
                    FlexCelPdfExport pdf = new FlexCelPdfExport(xls, true);
                    var bufferPdf = new MemoryStream();

                    pdf.Export(bufferPdf);
                }
                else
                {
                    Guid? iIDQuyetToan = null;
                    if (data != null && data.iID_BCQuyetToanNienDoID != Guid.Empty)
                        iIDQuyetToan = data.iID_BCQuyetToanNienDoID;

                    var objDonVi = _vdtService.GetDonViQuanLyById(dataPrintReport.iID_DonViQuanLyID);

                    var dataTongHop = _model.GetQuyetToanVonNam(iIDQuyetToan, objDonVi.iID_MaDonVi, dataPrintReport.INamKeHoach, dataPrintReport.IIdNguonVonId);
                    var dataPhanTich = _model.GetQuyetToanVonNam_PhanTich(iIDQuyetToan, objDonVi.iID_MaDonVi, dataPrintReport.INamKeHoach, dataPrintReport.IIdNguonVonId);

                    ExcelFile xls = CreateReportBcQuyetToanVonNam(dataTongHop, dataPhanTich, dataPrintReport, (isPdf ? ExportType.PDF : ExportType.EXCEL));
                    TempData["DataReportXls"] = xls;
                    FlexCelPdfExport pdf = new FlexCelPdfExport(xls, true);
                    var bufferPdf = new MemoryStream();

                    pdf.Export(bufferPdf);
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true, isPdf = isPdf }, JsonRequestBehavior.AllowGet);
        }

        public ExcelFile CreateReport(List<BcquyetToanNienDoVonUngChiTietViewModel> dataReport, VdtQtBcQuyetToanNienDoPrintDataExportModel paramReport, ExportType exportType)
        {
            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();
            if(exportType == ExportType.EXCEL)
            {
                fr.AddTable("Items", dataReport);
            }
            else
            {
                List<BcquyetToanNienDoVonUngChiTietViewModel> dataClone = new List<BcquyetToanNienDoVonUngChiTietViewModel>();
                var lstLoaiCongTrinh = _vdtService.GetAllDmLoaiCongTrinh();
                if(lstLoaiCongTrinh != null && dataReport != null)
                {
                    Dictionary<Guid, List<BcquyetToanNienDoVonUngChiTietViewModel>> dicData = dataReport.GroupBy(n => (n.iID_LoaiCongTrinh ?? Guid.Empty)).ToDictionary(n => n.Key, n => n.ToList());
                    foreach (var item in lstLoaiCongTrinh.Where(n => !n.iID_Parent.HasValue).OrderBy(n => n.sMaLoaiCongTrinh))
                    {
                        bool bHaveData = false;
                        dataClone.AddRange(RecursiveBCVonUngLoaiCongTrinh(item, lstLoaiCongTrinh.ToList(), dicData, ref bHaveData));
                    }
                }
                fr.AddTable("Items", dataClone);
            }
            
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rpt_vdt_quyettoanniendo_vonung.xlsx"));
            fr.SetValue("dNgayHienTai", DateTime.Now.ToShortDateString());
            fr.SetValue("Title1", string.Format(paramReport.txt_TieuDe1, paramReport.INamKeHoach.ToString()));
            fr.SetValue("Title2", paramReport.txt_TieuDe2);
            fr.SetValue("Title3", paramReport.txt_TieuDe3);
            fr.SetValue("sTenDonVi", paramReport.sTenDonViQuanLy);
            fr.SetValue("Cap1", string.Empty);
            fr.SetValue("DiaDiem", string.Empty);
            fr.SetValue("h2", string.Format("Đơn vị tính: {0}", paramReport.sDonViTinh));
            FormatNumber formatNumber = new FormatNumber(int.Parse(paramReport.sValueDonViTinh), exportType);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.UseChuKy(Username)
                  .UseChuKyForController(sControlName)
                  .UseForm(this);
            fr.Run(Result);
            return Result;
        }

        public ExcelFile CreateReportBcQuyetToanVonNam(List<BcquyetToanNienDoVonNamChiTietViewModel> lstDataTongHop, List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> lstDataPhanTich, VdtQtBcQuyetToanNienDoPrintDataExportModel paramReport, ExportType exportType)
        {
            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();
            var lstLoaiCongTrinh = _vdtService.GetAllDmLoaiCongTrinh();
            if(exportType == ExportType.EXCEL)
            {
                fr.AddTable("Items", lstDataTongHop);
            }
            else
            {
                List<BcquyetToanNienDoVonNamChiTietViewModel> dataClone = new List<BcquyetToanNienDoVonNamChiTietViewModel>();
                if(lstLoaiCongTrinh != null && lstDataTongHop != null)
                {
                    Dictionary<Guid, List<BcquyetToanNienDoVonNamChiTietViewModel>> dicData = lstDataTongHop.GroupBy(n => (n.iID_LoaiCongTrinh ?? Guid.Empty)).ToDictionary(n => n.Key, n => n.ToList());
                    foreach (var item in lstLoaiCongTrinh.Where(n => !n.iID_Parent.HasValue).OrderBy(n => n.sMaLoaiCongTrinh))
                    {
                        bool bHaveData = false;
                        dataClone.AddRange(RecursiveBCKeHoachVonNamLoaiCongTrinh(item, lstLoaiCongTrinh.ToList(), dicData, ref bHaveData));
                    }
                }
                fr.AddTable("Items", dataClone);
            }
            
            if (paramReport.IIdNguonVonId == 1)
            {
                Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/QuyetToan/rpt_vdt_quyettoanniendo_vonnam_nsqp.xlsx"));
                if (exportType == ExportType.EXCEL)
                {
                    fr.AddTable("ItemsPhanTich", lstDataPhanTich);
                }
                else
                {
                    List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> dataClone = new List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel>();
                    if (lstLoaiCongTrinh != null && lstDataPhanTich != null)
                    {
                        Dictionary<Guid, List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel>> dicData = lstDataPhanTich.GroupBy(n => (n.iID_LoaiCongTrinh ?? Guid.Empty)).ToDictionary(n => n.Key, n => n.ToList());
                        foreach (var item in lstLoaiCongTrinh.Where(n => !n.iID_Parent.HasValue).OrderBy(n => n.sMaLoaiCongTrinh))
                        {
                            bool bHaveData = false;
                            dataClone.AddRange(RecursiveBCPhanTichLoaiCongTrinh(item, lstLoaiCongTrinh.ToList(), dicData, ref bHaveData));
                        }
                    }
                    fr.AddTable("ItemsPhanTich", dataClone);
                }   
            }
            else
            {
                Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/QuyetToan/rpt_vdt_quyettoanniendo_vonnam.xlsx"));
            }
            fr.SetValue("sTenDonVi", paramReport.sTenDonViQuanLy);
            fr.SetValue("dNgayHienTai", string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.ToString("dd"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("yyyy")));
            fr.SetValue("iNam", paramReport.INamKeHoach);
            fr.SetValue("DiaDiem", string.Empty);
            fr.SetValue("Cap1", string.Empty);
            fr.SetValue("h2", string.Format("Đơn vị tính: {0}", paramReport.sDonViTinh));
            fr.SetValue("Title1", string.Format(paramReport.txt_TieuDe1, paramReport.INamKeHoach));
            fr.SetValue("Title2", paramReport.txt_TieuDe2);
            FormatNumber formatNumber = new FormatNumber(int.Parse(paramReport.sValueDonViTinh), exportType);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.UseChuKy(Username)
                 .UseChuKyForController(sControlName)
                 .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        [HttpGet]
        public ActionResult ExportReport(int iLoaiBaoCao, bool pdf)
        {
            ExcelFile xls = (ExcelFile)TempData["DataReportXls"];
            if (iLoaiBaoCao == 1)
                return Print(xls, pdf ? "pdf" : "xls", pdf ? "rpt_vdt_quyettoanniendo_vonung.pdf" : "rpt_vdt_quyettoanniendo_vonung.xls");
            else
                return Print(xls, pdf ? "pdf" : "xls", pdf ? "rpt_vdt_quyettoanniendo_vonnam.pdf" : "rpt_vdt_quyettoanniendo_vonnam.xls");
        }
        #endregion

        #region Helper
        [HttpGet]
        public JsonResult GetListDataDonViQuanLy()
        {
            return Json(new { results = CommonFunction.GetListDataDonViQuanLy() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBcQuyetToanNienDoChiTiet(Guid iIdBcQuyetToanId, List<VDT_QT_BCQuyetToanNienDo_ChiTiet_01> lstData)
        {
            lstData = lstData.Select(n => { n.iID_BCQuyetToanNienDoChiTiet01ID = Guid.NewGuid(); n.iID_BCQuyetToanNienDo = iIdBcQuyetToanId; return n; }).ToList();
            return Json(new { result = _model.UpdateQuyetToanNienDoChiTiet(iIdBcQuyetToanId, lstData) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBcQuyetToanNienDoPhanTich(Guid iIdBcQuyetToanId, List<VDT_QT_BCQuyetToanNienDo_PhanTich> lstData)
        {
            lstData = lstData.Select(n => { n.Id = Guid.NewGuid(); n.iID_BCQuyetToanNienDo = iIdBcQuyetToanId; return n; }).ToList();
            return Json(new { result = _model.UpdateQuyetToanNienDoChiTietPhanTich(iIdBcQuyetToanId, lstData) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region download file
        public ActionResult ImportData()
        {
            ViewBag.drpDonViQuanLyImport = CommonFunction.GetDataDropDownDonViQuanLy();
            List<SelectListItem> lstLoaiThanhToan = new List<SelectListItem> {
                new SelectListItem{Text = "Thanh toán", Value="ThanhToan"},
                new SelectListItem{Text = "Thanh toán tạm ứng", Value="ThanhToanTamUng"},
            };
            ViewBag.drpLoaiThanhToanImport = lstLoaiThanhToan.ToSelectList("Value", "Text");
            FileFtpModel data = new FileFtpModel();
            return PartialView("_modalDownloadFile", data);
        }

        [HttpPost]
        public ActionResult GetGridListExcelFromFTP(string idDonVi, string nam, string loaiThanhToan)
        {
            ViewBag.drpDonViQuanLyImport = CommonFunction.GetDataDropDownDonViQuanLy();
            List<SelectListItem> lstLoaiThanhToan = new List<SelectListItem> {
                new SelectListItem{Text = "Thanh toán", Value="ThanhToan"},
                new SelectListItem{Text = "Thanh toán tạm ứng", Value="ThanhToanTamUng"},
            };
            ViewBag.drpLoaiThanhToanImport = lstLoaiThanhToan.ToSelectList("Value", "Text");
            FileFtpModel data = new FileFtpModel();
            try
            {
                var lstResponse = new List<string>();
                DateTime CrTime = DateTime.Now;
                string tenDonVi = "0";
                NS_DonVi donVi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), idDonVi);
                if (donVi != null)
                {
                    tenDonVi = donVi.iID_MaDonVi;
                }
                pathString = pathString + "/" + tenDonVi + "/VDT/BcQuyetToanNienDo/" + loaiThanhToan + "/send/" + nam.Trim() + "/";
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
                            return File(file_array, "application/vnd.ms-excel", $"rpt_vdt_quyettoanniendo_vonnam.xlsx");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index");
        }
        #endregion

        [HttpPost]
        public JsonResult GetListDonvi()
        {
            var result = new List<dynamic>();
            var listModel = CommonFunction.GetDataDropDownDonViQuanLy(_lstDonViExclude).ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.Text,
                        text = item.Value
                    });
                }
            }
            return Json(new { status = true, data = result });
        }
        [HttpPost]
        public JsonResult GetListNguonVon()
        {
            var result = new List<dynamic>();
            var listModel = CommonFunction.GetDataDropDownNguonNganSach().ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.Text,
                        text = item.Value
                    });
                }
            }
            return Json(new { status = true, data = result });
        }
        [HttpPost]
        public JsonResult GetListDropDownNamKeHoach()
        {
            var result = new List<dynamic>();
            var listModel = GetListNamKeHoach();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.Value,
                        text = item.Label
                    });
                }
            }
            return Json(new { status = true, data = result });
        }
        public List<Dropdown_QuyetToanNienDo> GetListNamKeHoach()
        {
            List<Dropdown_QuyetToanNienDo> listNam = new List<Dropdown_QuyetToanNienDo>();
            int namHienTai = DateTime.Now.Year + 1;
            for (int i = 20; i > 0; i--)
            {
                namHienTai -= 1;
                Dropdown_QuyetToanNienDo namKeHoachOpt = new Dropdown_QuyetToanNienDo()
                {
                    Value = namHienTai,
                    Label = namHienTai.ToString()
                };
                listNam.Add(namKeHoachOpt);
            }
            return listNam;
        }
        [HttpPost]
        public ActionResult GetListTongHopQuyetToan(string sSoDeNghi, DateTime? dNgayDeNghi, int? iLoaiThanhToan, Guid? iDonVi, int? iNamKeHoach, string sTenNguonVon)
        {
            VdtQtBcQuyetToanNienDoPagingModel vm = new VdtQtBcQuyetToanNienDoPagingModel();
            vm.lstData = _vdtService.GetListTongHopQuyetToanNienDo(sSoDeNghi, dNgayDeNghi, iLoaiThanhToan, iDonVi, iNamKeHoach, sTenNguonVon);
            var ListTongHopQuyetToan = vm.lstData;
            ViewBag.TabIndex = 1;

            return Json(new { data = ListTongHopQuyetToan }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetModalTongHop(string[] listId, PagingInfo _paging, int namKeHoach)
        {
            VdtQtBcQuyetToanNienDoPagingModel vm = new VdtQtBcQuyetToanNienDoPagingModel();
            vm._paging = _paging;
            List<VdtQtBcQuyetToanNienDoViewModel> list = new List<VdtQtBcQuyetToanNienDoViewModel>();
            var stringId = "";
            foreach (var id in listId)
            {
                VdtQtBcQuyetToanNienDoViewModel data = _vdtService.GetBCQuyetToanNienDoById(new Guid(id));
                List<NS_NguonNganSach> lstNguonVonQL = _vdtService.GetNguonNSList().ToList();

                if (data.iID_NguonVonID != null)
                {
                    var nguonvon = lstNguonVonQL.Find(x => x.iID_MaNguonNganSach == data.iID_NguonVonID);
                    data.sTenNguonVon += nguonvon.sTen;
                }
                List<NS_DonVi> lstDonViQL = _vdtService.GetDonviList(PhienLamViec.NamLamViec).ToList();
                if (data.iID_DonViQuanLyID != null)
                {
                    var donvi = lstDonViQL.Find(x => x.iID_Ma == data.iID_DonViQuanLyID);
                    data.sTenDonVi += donvi.sTen;
                }
                stringId += id + ",";
                list.Add(data);
            }
            vm.lstData = list;
            ViewBag.ListNamKeHoach = namKeHoach;
            ViewBag.ListId = stringId.Substring(0, stringId.Length - 1);
            return PartialView("_modelTongHop", vm);
        }
        [HttpPost]
        public JsonResult SaveTongHop(VDT_QT_BCQuyetToanNienDo data, string listId)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            if (!_vdtService.SaveTongHop(data, Username, listId))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Xoa(string id)
        {
            if (!_vdtService.DeleteQuyetToanNienDoTongHop(Guid.Parse(id)))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDropdownThanhToan()
        {
            List<Dropdown_Find> thanhtoan = new List<Dropdown_Find>()
            {
               new Dropdown_Find()
               {
                   valueId = 0,
                   labelName = "--Tất cả--"
               },
               new Dropdown_Find()
               {
                   valueId =1,
                   labelName = "Thanh toán"
               },
                new Dropdown_Find()
               {
                   valueId =2,
                   labelName = "Tạm ứng"
                }
            };
            List<Dropdown_Find> lstloaithanhtoan = thanhtoan;
            ViewBag.ListLoaiThanhToan = lstloaithanhtoan;

            return Json(new
            {
                data = thanhtoan
            });
        }
        public JsonResult GetDropdownDonVi()
        {
            return Json(new
            {
                data = _vdtService.GetDonviList(PhienLamViec.NamLamViec).ToList()
            });
        }
        public JsonResult GetDropdownNguonVon()
        {
            var ddata = _vdtService.GetNguonNSList().ToList();
            return Json(new
            {
                data = _vdtService.GetNguonNSList().ToList()
            });
        }

        [HttpPost]
        public ActionResult ViewDetailsTongHop(Guid? id)
        {
            VdtQtBcQuyetToanNienDoViewModel data = new VdtQtBcQuyetToanNienDoViewModel();
            if (id.HasValue)
            {
                data = _vdtService.GetQuyetToanNienDoById(id.Value);
                if (data.iID_DonViQuanLyID!=null)
                {
                    var donvi = _vdtService.GetDonViQuanLyById(data.iID_DonViQuanLyID??Guid.Empty);
                    data.sTenDonVi = donvi.sTen;
                }
                if (data.iID_NguonVonID != null)
                {
                    var nguonvon = _vdtService.GetTenNguonVonByID(data.iID_NguonVonID??0);
                    data.sTenNguonVon = nguonvon.sTen;
                }
                if (data == null)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return PartialView("_modalDetail", data);
        }
        public ActionResult GetModalUpdate(Guid? id)
        {
            VdtQtBcQuyetToanNienDoViewModel data = new VdtQtBcQuyetToanNienDoViewModel();
            if (id.HasValue)
            {
                data = _vdtService.GetQuyetToanNienDoById(id.Value);
                if (data == null) return RedirectToAction("Index");
            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public JsonResult QuyetToanNienDoTongHopSave(VDT_QT_BCQuyetToanNienDo data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            var returnData = _vdtService.SaveQTNDTongHop(data);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = returnData.errorMess }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        #region Helper
        private List<BcquyetToanNienDoVonNamChiTietViewModel> RecursiveBCKeHoachVonNamLoaiCongTrinh(
            VDT_DM_LoaiCongTrinh current, List<VDT_DM_LoaiCongTrinh> lstLoaiCongTrinh,
            Dictionary<Guid, List<BcquyetToanNienDoVonNamChiTietViewModel>> dicData, ref bool bHaveData)
        {
            List<BcquyetToanNienDoVonNamChiTietViewModel> results = new List<BcquyetToanNienDoVonNamChiTietViewModel>();
            bool bIsAddParent = false;
            var currentData = new BcquyetToanNienDoVonNamChiTietViewModel()
            {
                iStt = current.sMaLoaiCongTrinh,
                sTenDuAn = current.sTenLoaiCongTrinh,
                IsHangCha = true

            };
            if (dicData.ContainsKey(current.iID_LoaiCongTrinh))
            {
                results.Add(currentData);
                var lstChuyenTiep = dicData[current.iID_LoaiCongTrinh].Where(n => n.BIsChuyenTiep);
                var lstMoMoi = dicData[current.iID_LoaiCongTrinh].Where(n => !n.BIsChuyenTiep);
                if (lstMoMoi != null && lstMoMoi.Count() != 0)
                {
                    results.Add(new BcquyetToanNienDoVonNamChiTietViewModel()
                    {
                        sTenDuAn = DU_AN_MO_MOI,
                        iStt = "I",
                        IsHangCha = true
                    });
                    results.AddRange(lstMoMoi);
                }
                if (lstChuyenTiep != null && lstChuyenTiep.Count() != 0)
                {
                    results.Add(new BcquyetToanNienDoVonNamChiTietViewModel()
                    {
                        sTenDuAn = DU_AN_CHUYEN_TIEP,
                        iStt = "II",
                        IsHangCha = true
                    });
                    results.AddRange(lstChuyenTiep);
                }
                bHaveData = true;
                bIsAddParent = true;
            }
            foreach (var item in lstLoaiCongTrinh.Where(n => n.iID_Parent.HasValue && n.iID_Parent == current.iID_LoaiCongTrinh).OrderBy(n => n.sMaLoaiCongTrinh))
            {
                bool bChildData = false;
                var lstChild = RecursiveBCKeHoachVonNamLoaiCongTrinh(item, lstLoaiCongTrinh, dicData, ref bChildData);
                if (bChildData)
                {
                    if (!bIsAddParent)
                    {
                        results.Add(currentData);
                        bIsAddParent = true;
                    }
                    results.AddRange(lstChild);
                    bHaveData = true;
                }
            }
            return results;
        }

        private List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> RecursiveBCPhanTichLoaiCongTrinh(
            VDT_DM_LoaiCongTrinh current, List<VDT_DM_LoaiCongTrinh> lstLoaiCongTrinh,
            Dictionary<Guid, List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel>> dicData, ref bool bHaveData)
        {
            List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> results = new List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel>();
            bool bIsAddParent = false;
            var currentData = new BcquyetToanNienDoVonNamPhanTichChiTietViewModel()
            {
                SSoThuTu = current.sMaLoaiCongTrinh,
                STenDuAn = current.sTenLoaiCongTrinh,
                IsHangCha = true

            };
            if (dicData.ContainsKey(current.iID_LoaiCongTrinh))
            {
                results.Add(currentData);
                var lstChuyenTiep = dicData[current.iID_LoaiCongTrinh].Where(n => n.BIsChuyenTiep);
                var lstMoMoi = dicData[current.iID_LoaiCongTrinh].Where(n => !n.BIsChuyenTiep);
                if (lstMoMoi != null && lstMoMoi.Count() != 0)
                {
                    results.Add(new BcquyetToanNienDoVonNamPhanTichChiTietViewModel()
                    {
                        STenDuAn = DU_AN_MO_MOI,
                        SSoThuTu = "I",
                        IsHangCha = true
                    });
                    results.AddRange(lstMoMoi);
                }
                if (lstChuyenTiep != null && lstChuyenTiep.Count() != 0)
                {
                    results.Add(new BcquyetToanNienDoVonNamPhanTichChiTietViewModel()
                    {
                        STenDuAn = DU_AN_CHUYEN_TIEP,
                        SSoThuTu = "II",
                        IsHangCha = true
                    });
                    results.AddRange(lstChuyenTiep);
                }
                bHaveData = true;
                bIsAddParent = true;
            }
            foreach (var item in lstLoaiCongTrinh.Where(n => n.iID_Parent.HasValue && n.iID_Parent == current.iID_LoaiCongTrinh).OrderBy(n => n.sMaLoaiCongTrinh))
            {
                bool bChildData = false;
                var lstChild = RecursiveBCPhanTichLoaiCongTrinh(item, lstLoaiCongTrinh, dicData, ref bChildData);
                if (bChildData)
                {
                    if (!bIsAddParent)
                    {
                        results.Add(currentData);
                        bIsAddParent = true;
                    }
                    results.AddRange(lstChild);
                    bHaveData = true;
                }
            }
            return results;
        }

        private List<BcquyetToanNienDoVonUngChiTietViewModel> RecursiveBCVonUngLoaiCongTrinh(VDT_DM_LoaiCongTrinh current, List<VDT_DM_LoaiCongTrinh> lstLoaiCongTrinh,
            Dictionary<Guid, List<BcquyetToanNienDoVonUngChiTietViewModel>> dicData, ref bool bHaveData)
        {
            List<BcquyetToanNienDoVonUngChiTietViewModel> results = new List<BcquyetToanNienDoVonUngChiTietViewModel>();
            bool bIsAddParent = false;
            var currentData = new BcquyetToanNienDoVonUngChiTietViewModel()
            {
                iStt = current.sMaLoaiCongTrinh,
                sTenDuAn = current.sTenLoaiCongTrinh,
                IsHangCha = true

            };
            if (dicData.ContainsKey(current.iID_LoaiCongTrinh))
            {
                results.Add(currentData);
                var lstChuyenTiep = dicData[current.iID_LoaiCongTrinh].Where(n => n.BIsChuyenTiep);
                var lstMoMoi = dicData[current.iID_LoaiCongTrinh].Where(n => !n.BIsChuyenTiep);
                if (lstMoMoi != null && lstMoMoi.Count() != 0)
                {
                    results.Add(new BcquyetToanNienDoVonUngChiTietViewModel()
                    {
                        sTenDuAn = DU_AN_MO_MOI,
                        iStt = "I",
                        IsHangCha = true
                    });
                    results.AddRange(lstMoMoi);
                }
                if (lstChuyenTiep != null && lstChuyenTiep.Count() != 0)
                {
                    results.Add(new BcquyetToanNienDoVonUngChiTietViewModel()
                    {
                        sTenDuAn = DU_AN_CHUYEN_TIEP,
                        iStt = "II",
                        IsHangCha = true
                    });
                    results.AddRange(lstChuyenTiep);
                }
                bHaveData = true;
                bIsAddParent = true;
            }
            foreach (var item in lstLoaiCongTrinh.Where(n => n.iID_Parent.HasValue && n.iID_Parent == current.iID_LoaiCongTrinh).OrderBy(n => n.sMaLoaiCongTrinh))
            {
                bool bChildData = false;
                var lstChild = RecursiveBCVonUngLoaiCongTrinh(item, lstLoaiCongTrinh, dicData, ref bChildData);
                if (bChildData)
                {
                    if (!bIsAddParent)
                    {
                        results.Add(currentData);
                        bIsAddParent = true;
                    }
                    results.AddRange(lstChild);
                    bHaveData = true;
                }
            }
            return results;
        }
        #endregion
    }
}