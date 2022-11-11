using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Common;
using System.Text;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.TongHopCapPhat
{
    public class TongHopCapPhatController : AppController
    {
        INganSachService _iNganSachService = NganSachService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INguonNSService _iNguonNSService = NguonNSService.Default;
        private static string[] _lstDonViExclude = new string[] { "0", "1" };
        // GET: QLVonDauTu/TongHopCapPhat
        public ActionResult Index()
        {
            ViewBag.ListNguonVonDauTu = CommonFunction.GetDataDropDownNguonNganSach();
            List<VDTBaoCaoCapPhatViewModel> arrData = new List<VDTBaoCaoCapPhatViewModel>();
            return View(arrData);
        }
        public JsonResult GetNSDonVi()
        {
            var lstDonVi = _iQLVonDauTuService.GetDanhSachDonVi(PhienLamViec.NamLamViec).ToList();
            StringBuilder htmlDuAn = new StringBuilder();
            htmlDuAn.AppendFormat("<option value='{0}' selected>{1}</option>", string.Empty, "--Tất cả--");
            if (lstDonVi != null && lstDonVi.Count > 0)
            {
                for (int i = 0; i < lstDonVi.Count; i++)
                {
                    htmlDuAn.AppendFormat("<option value='{0}'>{1}</option>", lstDonVi[i].iID_Ma, HttpUtility.HtmlEncode(lstDonVi[i].iID_MaDonVi + " - " + lstDonVi[i].sTen));
                }
            }
            return Json(new { htmlCT = htmlDuAn.ToString() }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult GetThongTinBaoCaoCapPhat(Guid? iID_DonViQuanLyID, int? iIDNguonNganSach, int? iNamKeHoach)
        {
            IEnumerable<VDTBaoCaoCapPhatViewModel> lstData = _iQLVonDauTuService.GetThongTinBaoCaoCapPhat(iID_DonViQuanLyID, iIDNguonNganSach, iNamKeHoach, PhienLamViec.NamLamViec);
            ViewBag.iNamKeHoach = iNamKeHoach ?? 0;
            ViewBag.iNamTruoc = (iNamKeHoach ?? 0) - 1;
            return PartialView("_partialTongHopCapPhat", lstData);
        }
        public ActionResult ExportExcelBaoCaoCapPhat(Guid? iID_DonViQuanLyID, int? iIDNguonNganSach, int? iNamKeHoach, string sTenNganSach)
        {
            var excel = CreateReportBaoCaoCapPhat(iID_DonViQuanLyID, iIDNguonNganSach, iNamKeHoach, sTenNganSach);
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCaoCapPhat.xlsx");
            }
        }

        public ExcelFile CreateReportBaoCaoCapPhat(Guid? iID_DonViQuanLyID, int? iIDNguonNganSach, int? iNamKeHoach, string sTenNganSach)
        {
            IEnumerable<VDTBaoCaoCapPhatViewModel> lstData = _iQLVonDauTuService.GetThongTinBaoCaoCapPhat(iID_DonViQuanLyID, iIDNguonNganSach, iNamKeHoach, PhienLamViec.NamLamViec);
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rpt_BaoCaoCapPhat.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            string sTenDonVi = "";
            var donvi = _iQLVonDauTuService.GetDanhSachDonVi(PhienLamViec.NamLamViec).Where(x => x.iID_Ma == iID_DonViQuanLyID).FirstOrDefault();
            if(donvi != null)
            {
                sTenDonVi = donvi.sTen;
            }
            double? fTongKhoBac_DT = lstData.Sum(x => x.fCapBacTaiKhoBac_DTNN);
            double? fTongLenhChi_DT = lstData.Sum(x => x.fCapBangLenhChi_DTNN);
            double? fTongKhoBac_VU = lstData.Sum(x => x.fCapBacTaiKhoBac_VUNCT);
            double? fTongLenhChi_VU = lstData.Sum(x => x.fCapBangLenhChi_VUNCT);
            double? fTongKhac = 0 ;
            double? fTong = (fTongKhoBac_VU.HasValue  ? fTongKhoBac_VU :0) + (fTongLenhChi_VU.HasValue  ? fTongLenhChi_VU :0) + fTongKhac;
            double? fTongKLHT = lstData.Sum(x => x.fThanhToanKLHT);
            double? fTongTamUng = lstData.Sum(x => x.fTamUng);
            double? fTongThuHoiTamUng = lstData.Sum(x=>x.fThuHoiTamUng);

            fr.AddTable<VDTBaoCaoCapPhatViewModel>("DaTaDuAn", lstData);
            fr.SetValue("To", 1);
            fr.SetValue("iNamKeHoach", iNamKeHoach);
            fr.SetValue("iNamTruoc", (iNamKeHoach - 1));
            fr.SetValue("iIDNguonNganSach", iIDNguonNganSach);
            fr.SetValue("sTenNganSach", sTenNganSach.ToUpper());
            fr.SetValue("sTenDonVi", sTenDonVi.ToUpper());

            fr.SetValue("fTongKhoBac_DT", fTongKhoBac_DT);
            fr.SetValue("fTongLenhChi_DT", fTongLenhChi_DT);
            fr.SetValue("fTongKhoBac_VU", fTongKhoBac_VU);
            fr.SetValue("fTongLenhChi_VU", fTongLenhChi_VU);
            fr.SetValue("fTongKhac", fTongKhac);
            fr.SetValue("fTong", fTong);
            fr.SetValue("fTongKLHT", fTongKLHT);
            fr.SetValue("fTongTamUng", fTongTamUng);
            fr.SetValue("fTongThuHoiTamUng", fTongThuHoiTamUng);

            fr.Run(Result);
            return Result;
        }
    }
}