using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.TheoDoiChiTieuCapPhatDuAn
{
    public class TheoDoiChiTieuCapPhatDuAnController : FlexcelReportController
    {
        INganSachService _iNganSachService = NganSachService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;

        // GET: QLVonDauTu/TheoDoiChiTieuCapPhatDuAn
        public ActionResult Index()
        {
            List<VdtBcTheoDoiChiTieuCapPhatModel> lstData = new List<VdtBcTheoDoiChiTieuCapPhatModel>();
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_MaDonVi", "sMoTa");
            return View(lstData);
        }

        [HttpGet]
        public ActionResult Search(string sMaDonVi, int iNamLamViec)
        {
            List<VdtBcTheoDoiChiTieuCapPhatModel> lstData = _iQLVonDauTuService.GetBcTheoDoiChiTieuCapPhatBySearch(sMaDonVi, iNamLamViec);
            return PartialView("_partialTheoDoiChiTieuCapPhatDuAn", lstData);
        }

        #region Helper
        [HttpGet]
        public ActionResult ExportReport(string sTenDonVi, string sMaDonVi, int iNamLamViec)
        {
            List<VdtBcTheoDoiChiTieuCapPhatModel> lstData = _iQLVonDauTuService.GetBcTheoDoiChiTieuCapPhatBySearch(sMaDonVi, iNamLamViec);
            if (lstData != null)
            {
                int i = 1;
                foreach (var item in lstData) { item.iStt = i; ++i; }
            }
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/tmp_BaoCaoTheoDoiChiTieuCapPhat.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("iNamKeHoach", iNamLamViec);
            fr.AddTable("Items", lstData);
            fr.Run(Result);
            return Print(Result, "xls", "rpt_BC_TheoDoiChiTieuCapPhatDuAn.xls");
        }
        #endregion
    }
}