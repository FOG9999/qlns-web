using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThucHienDauTu
{
    public class BaoCaoDuToanNSQPNamController : AppController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        private const string sFilePath = "/Report_ExcelFrom/VonDauTu/rpt_baocaodutoannsqpnam.xlsx";

        public ActionResult Index()
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Common.Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sTen");

            return View();
        }

        public JsonResult LayDataBaoCaoDuToanNSQPNam(int iNamKeHoach, string sMaDonVi)
        {
            List<RptDuToanNSQPNam> listData = _iQLVonDauTuService.LayDataBaoCaoDuToanNSQPNam(iNamKeHoach, sMaDonVi);
            // luu dieu kien tim kiem
            TempData["iNamKeHoach"] = iNamKeHoach;
            TempData["sMaDonVi"] = sMaDonVi;
            return Json(listData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult XuatFile()
        {
            ExcelFile excel = TaoFile();
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Bao cao du toan NSQP theo nam.xlsx");
            }
        }

        public ExcelFile TaoFile()
        {
            int iNamKeHoach = 0; 
            string sMaDonVi = null;
            // get dieu kien tiem kiem
            if (TempData["iNamKeHoach"] != null)
                iNamKeHoach = (int)TempData["iNamKeHoach"];
            if (TempData["sMaDonVi"] != null)
                sMaDonVi = (string)TempData["sMaDonVi"];
            
            List<RptDuToanNSQPNam> listData = _iQLVonDauTuService.LayDataBaoCaoDuToanNSQPNam(iNamKeHoach, sMaDonVi);
            double? fTongSo = listData.Where(x => (bool)x.bIsHangCha && x.iLoaiDuAn == 1).Select(x => x.fGiaTrPhanBo).Sum();

            String sTenDonVi = "";
            sTenDonVi = _iNganSachService.GetDonVi(PhienLamViec.iNamLamViec, sMaDonVi).sTen;

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable("dt", listData);
            fr.SetValue("DonVi", sTenDonVi);
            fr.SetValue("iNam", iNamKeHoach);
            fr.SetValue("fTongSo", fTongSo);
            fr.Run(Result);
            return Result;
        }
    }
}