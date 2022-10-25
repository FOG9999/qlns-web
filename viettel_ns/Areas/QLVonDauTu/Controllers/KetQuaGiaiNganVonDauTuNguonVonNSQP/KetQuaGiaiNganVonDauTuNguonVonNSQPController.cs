//using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.KetQuaGiaiNganVonDauTuNguonVonNSQP
{
    public class KetQuaGiaiNganVonDauTuNguonVonNSQPController : FlexcelReportController
    {
        INganSachService _iNganSachService = NganSachService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INguonNSService _iNguonNSService = NguonNSService.Default;
        private static string[] _lstDonViExclude = new string[] { "0", "1" };
        // GET: QLVonDauTu/KetQuaGiaiNganVonDauTuNguonVonNSQP
        public ActionResult Index()
        {
            ViewBag.ListDonViQuanLy = _iQLVonDauTuService.GetDanhSachDonVi(PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
            ViewBag.ListNguonVonDauTu = CommonFunction.GetDataDropDownNguonNganSach();
            List<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2> arrData = new List<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2>();
            IEnumerable<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2> lstData = arrData.AsEnumerable();
            return View(lstData);
        }
        [HttpPost]
        public ActionResult GetKetQuaGiaiNganChiKinhPhiDauTu(Guid? iID_DonViQuanLyID, int iID_NguonVonID, int? iNamKeHoach)
        {
            IEnumerable<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2> lstData = _iQLVonDauTuService.GetKetQuaGiaiNganChiKinhPhiDauTu(iID_DonViQuanLyID, iID_NguonVonID, iNamKeHoach);
            ViewBag.iNamKeHoach = iNamKeHoach ?? 0;
            ViewBag.iNamTruoc = (iNamKeHoach ?? 0) - 1;
            return PartialView("_partialKetQuaGiaiNganVonDauTuNguonVonNSQP", lstData);
        }
        //[HttpPost]
        //public bool ExportBCKQGiaNganKinhPhiDauTuNSQL(IEnumerable<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel> dataReport)
        //{
        //    TempData["dataReport"] = dataReport;
        //    return true;
        //} 

        [HttpGet]
        public ActionResult ExportExcel(Guid? iID_DonViQuanLyID, string sTenDonVi, string sTenNguonVon, int iID_NguonVonID, int? iNamKeHoach)
        {
            var excel = CreateReport(iID_DonViQuanLyID, sTenDonVi, sTenNguonVon, iID_NguonVonID, iNamKeHoach);
            return Print(excel, "xls", "rpt_VDT_Ket_Qua_Giai_Ngan_Von_Dau_Tu_Nguon_Von_NSQP.xlsx");

        }

        public ExcelFile CreateReport(Guid? iID_DonViQuanLyID, string sTenDonVi,string sTenNguonVon, int iID_NguonVonID, int? iNamKeHoach)
        {
            IEnumerable<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2> lstData = _iQLVonDauTuService.GetKetQuaGiaiNganChiKinhPhiDauTu(iID_DonViQuanLyID, iID_NguonVonID, iNamKeHoach);

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rpt_VDT_Ket_Qua_Giai_Ngan_Von_Dau_Tu_Nguon_Von_NSQP.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2>("Items", lstData);
            fr.SetValue("iNamKeHoach", iNamKeHoach);
            fr.SetValue("iNamTruoc", (iNamKeHoach - 1));
            fr.SetValue("sTenNguonVon", sTenNguonVon);
            fr.SetValue("sTenDVQL", sTenDonVi.ToUpper());
            fr.SetValue("sTenNguonVon", sTenNguonVon.ToUpper());
            fr.Run(Result);
            return Result;
        }
    }
}