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

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class TongHopThongTinDuAnController : AppController
    {
        INganSachService _iNganSachService = NganSachService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;

        // GET: QLVonDauTu/TongHopThongTinDuAn
        public ActionResult Index()
        {
            ViewBag.ListDonViQuanLy = _iQLVonDauTuService.GetDanhSachDonVi(PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
            ViewBag.iNamLamViec = PhienLamViec.iNamLamViec;
            List<VDTTongHopThongTinDuAnViewModel> arrData = new List<VDTTongHopThongTinDuAnViewModel>();
            return View(arrData);
        }

        [HttpPost]
        public ActionResult GetThongTinTongHopDuAn(Guid iID_DonViQuanLyID, int? iNamKeHoach)
        {
            IEnumerable<VDTTongHopThongTinDuAnViewModel> lstData = _iQLVonDauTuService.GetThongTinTongHopDuAn_v2(iID_DonViQuanLyID, iNamKeHoach);

            return PartialView("_partialListTongHopThongTinDuAn", lstData);
        }
        
        [HttpPost]
        public bool ExportBCTongHopTTDuAn(IEnumerable<VDTTongHopThongTinDuAnViewModel> dataReport)
        {
            TempData["dataReport"] = dataReport;
            return true;
        }

        public ActionResult ExportExcel(Guid iID_DonViQuanLyID, int? iNamKeHoach)
        {
            IEnumerable<VDTTongHopThongTinDuAnViewModel> dataReport = null;
            if (TempData["dataReport"] != null)
            {
                dataReport = (IEnumerable<VDTTongHopThongTinDuAnViewModel>)TempData["dataReport"];
            }
            else
                return RedirectToAction("Index");

            var excel = CreateReport(dataReport, iID_DonViQuanLyID, iNamKeHoach);
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);
                
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TongHopThongTinDuAn.xlsx");
            }

            //return RedirectToAction("Index");
        }

        public ExcelFile CreateReport(IEnumerable<VDTTongHopThongTinDuAnViewModel> dataReport, Guid iID_DonViQuanLyID, int? iNamKeHoach)
        {
            IEnumerable<VDTTongHopThongTinDuAnViewModel> lstData = _iQLVonDauTuService.GetThongTinTongHopDuAn_v2(iID_DonViQuanLyID, iNamKeHoach);
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rpt_VDT_Tong_Hop_Thong_Tin_Du_An_v2.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            fr.AddTable<VDTTongHopThongTinDuAnViewModel>("TongHop", dataReport);
            fr.AddTable<VDTTongHopThongTinDuAnViewModel>("DaTaDuAn", lstData);
            fr.SetValue("iNamKeHoach", iNamKeHoach);
            fr.SetValue("sTenDVQL", lstData.First().sTen);
            fr.Run(Result);
            return Result;
        }
    }
}