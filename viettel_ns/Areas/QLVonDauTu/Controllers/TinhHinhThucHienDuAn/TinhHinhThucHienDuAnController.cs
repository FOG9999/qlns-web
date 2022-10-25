using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.TinhHinhThucHienDuAn
{
    public class TinhHinhThucHienDuAnController : AppController
    {
        INganSachService _iNganSachService = NganSachService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        // GET: QLVonDauTu/TinhHinhDuAn
        public ActionResult Index()
        {
            List<VDTTinhHinhDuAnViewModel> arrData = new List<VDTTinhHinhDuAnViewModel>();
            return View(arrData);
        }
        [HttpPost]
        public ActionResult GetTinhHinhThucHienDuAn(Guid? iID_DonViQuanLyID, Guid? iID_DuAn,  DateTime? iDenNgay)
        {
            IEnumerable<VDTTinhHinhDuAnViewModel> lstData = _iQLVonDauTuService.GetThongTinTinhHinhThucHienDuAn(iID_DonViQuanLyID, iID_DuAn, iDenNgay);
            return PartialView("_partialListTinhHinhThucHienDuAn", lstData);
        }

        public JsonResult GetNSDonVi()
        {
            var lstDonVi = _iQLVonDauTuService.GetDanhSachDonVi(PhienLamViec.NamLamViec).ToList();
            StringBuilder htmlDuAn = new StringBuilder();
            //htmlDuAn.AppendFormat("<option value='{0}' selected>{1}</option>", string.Empty, "--Chọn đơn vị--");
            if (lstDonVi != null && lstDonVi.Count > 0)
            {
                for (int i = 0; i < lstDonVi.Count; i++)
                {
                    htmlDuAn.AppendFormat("<option value='{0}'>{1}</option>", lstDonVi[i].iID_Ma, HttpUtility.HtmlEncode(lstDonVi[i].sTen));
                }
            }
            return Json(new { htmlCT = htmlDuAn.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDuAnTheoDonVi(Guid? iIDDonVi)
        {
            StringBuilder htmlDonVi = new StringBuilder();
            //htmlDonVi.AppendFormat("<option value='{0}' selected>{1}</option>", string.Empty, "--Chọn dự án--");
            if(iIDDonVi != null)
            {
                var lstDuAn = _iQLVonDauTuService.GetDuAnByIdDonVi(iIDDonVi.Value).ToList();

                if (lstDuAn != null && lstDuAn.Count > 0)
                {
                    for (int i = 0; i < lstDuAn.Count; i++)
                    {
                        htmlDonVi.AppendFormat("<option value='{0}'>{1}</option>", lstDuAn[i].iID_DuAnID, HttpUtility.HtmlEncode(lstDuAn[i].sTenDuAn));
                    }
                }
            }    
           
            return Json(new { htmlCT = htmlDonVi.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcelTinhHinhThucHienDuAn(Guid? iID_DonViQuanLyID, Guid? iID_DuAn, string iDenNgay)
        {
            DateTime? dDenNgay = string.IsNullOrEmpty(iDenNgay) ? (DateTime?)null : DateTime.Parse(iDenNgay);
            var excel = CreateReportTinhHinhThucHienDuAn(iID_DonViQuanLyID, iID_DuAn, dDenNgay);
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TinhHinhThucHienDuAn.xlsx");
            }
        }
        public ExcelFile CreateReportTinhHinhThucHienDuAn(Guid? iID_DonViQuanLyID, Guid? iID_DuAn, DateTime? iDenNgay)
        {
            VDTBaoCaoTinhHinhDATTChungViewModel model = new VDTBaoCaoTinhHinhDATTChungViewModel();
            model = _iQLVonDauTuService.GetThongTinChungBCTTDA(iID_DuAn, iDenNgay.Value.Year);
            IEnumerable<VDTTinhHinhDuAnViewModel> lstData = _iQLVonDauTuService.GetThongTinTinhHinhThucHienDuAn(iID_DonViQuanLyID, iID_DuAn,iDenNgay);
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rpt_VDT_Tinh_Hinh_Thuc_Hien_Du_An.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            double? fTongTienHopDong = lstData.Sum(x => x.fTienHopDong);
            double? fTongSoThanhToan = lstData.Sum(x => x.fSoThanhToan);
            double? fTongSoTamUng = lstData.Sum(x => x.fSoTamUng);
            double? fTongThuHoiTamUng = lstData.Sum(x => x.fThuHoiTamUng);
            double? fTongTongCongGiaiNgan = lstData.Sum(x => x.fTongCongGiaiNgan);
            double? fTongSoDaCapUng = lstData.Sum(x => x.fSoDaCapUng);
            fr.AddTable<VDTTinhHinhDuAnViewModel>("DaTaDuAn", lstData);
            fr.SetValue("fTongTienHopDong", fTongTienHopDong);
            fr.SetValue("fTongSoThanhToan", fTongSoThanhToan);
            fr.SetValue("fTongSoTamUng", fTongSoTamUng);
            fr.SetValue("fTongThuHoiTamUng", fTongThuHoiTamUng);
            fr.SetValue("fTongTongCongGiaiNgan", fTongTongCongGiaiNgan);
            fr.SetValue("fTongSoDaCapUng", fTongSoDaCapUng);

            //Add thông tin chung
            fr.SetValue("sTenDuAn", model.sTenDuAn);
            fr.SetValue("sTenDonVi", model.sTenDonVi);
            fr.SetValue("sSoQuyetDinh", model.sSoQuyetDinh);
            fr.SetValue("dNgayQuyetDinh", model.dNgayQuyetDinh.HasValue? model.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy"): String.Empty);
            fr.SetValue("fTongMucDauTuPheDuyet", model.fTongMucDauTuPheDuyet.HasValue? model.fTongMucDauTuPheDuyet.Value.ToString("###,###"): String.Empty);
            fr.SetValue("sThoiGianThucHien", model.sThoiGianThucHien);
            fr.SetValue("sTenNguonNganSach", model.sTenNguonNganSach);
            fr.SetValue("fLuyKeVonDaBoTri", model.fLuyKeVonDaBoTri.HasValue? model.fLuyKeVonDaBoTri.Value.ToString("###,###"): String.Empty);
            fr.SetValue("fNganSachQuocPhong", model.fNganSachQuocPhong.HasValue ? model.fNganSachQuocPhong.Value.ToString("###,###") : String.Empty);
            fr.SetValue("fLuyKeThanhToanQuaKhoBac", model.fLuyKeThanhToanQuaKhoBac.HasValue ? model.fLuyKeThanhToanQuaKhoBac.Value.ToString("###,###"): String.Empty);
            fr.SetValue("fKeHoachUng", model.fKeHoachUng.HasValue ? model.fKeHoachUng.Value.ToString("###,###"): String.Empty);
            fr.SetValue("fNguonNganSachQuocPhong", model.fNguonNganSachQuocPhong.HasValue ? model.fNguonNganSachQuocPhong.Value.ToString("###,###"): String.Empty);
            fr.SetValue("iDenNgay", iDenNgay.Value.ToString("dd/MM/yyyy"));

            fr.Run(Result);
            return Result;
        }
    }
}