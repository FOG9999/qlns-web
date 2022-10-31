using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH.QuyetToan;
using Viettel.Services;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.QLNH.Controllers.BaoCaoThongKe
{
    public class BaoCaoQuyetToanNguonChiDacBietController : FlexcelReportController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        // GET: QLNH/BaoCaoQuyetToanNguonChiDacBiet
        public ActionResult Index()
        {
            NH_QT_QuyetToanNienDo_ChiTietView vm = new NH_QT_QuyetToanNienDo_ChiTietView();
            vm.ListQTNDNguonChiDacBiet = _qlnhService.GetBaoCaoQuyetToanNguonChiDacBiet(null).ToList();

            List<Dropdown_SelectValue> lstNamKeHoach = GetListNamKeHoach();
            lstNamKeHoach.Insert(0, new Dropdown_SelectValue { Value = null, Label = "--Chọn năm--" });
            ViewBag.ListNamKeHoach = lstNamKeHoach.ToSelectList("Value", "Label");
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(int? iNamKeHoach)
        {
            NH_QT_QuyetToanNienDo_ChiTietView vm = new NH_QT_QuyetToanNienDo_ChiTietView();

            List<Dropdown_SelectValue> lstNamKeHoach = GetListNamKeHoach();
            lstNamKeHoach.Insert(0, new Dropdown_SelectValue { Value = null, Label = "--Chọn năm--" });
            ViewBag.ListNamKeHoach = lstNamKeHoach.ToSelectList("Value", "Label", iNamKeHoach?.ToString());

            List<NH_QTND_NguonChiDacBietReport> listQTNDNguonChiDacBiet = _qlnhService.GetBaoCaoQuyetToanNguonChiDacBiet(iNamKeHoach).ToList();
            List<NH_QTND_NguonChiDacBietReport> listQTND = new List<NH_QTND_NguonChiDacBietReport>();
            List<NH_QTND_NguonChiDacBietReport> listNotSum = listQTNDNguonChiDacBiet.Where(x => x.IsData.HasValue && x.IsData.Value != 3).ToList();
            List<NH_QTND_NguonChiDacBietReport> listSum = listQTNDNguonChiDacBiet.Where(x => x.IsData.HasValue && x.IsData.Value == 3).ToList();

            listQTND.AddRange(listNotSum);
            listQTND.AddRange(listSum);

            vm.ListQTNDNguonChiDacBiet = listQTND;

            return PartialView("_list", vm);
        }

        public ActionResult ExportQuyetToanNguonChiDacBiet(string txtTieuDe1, string txtTieuDe2, string sTenDonViCapTren, string sTenDonViCapDuoi, int txtNamKeHoach, string ext = "xlsx")
        {
            txtTieuDe1 = HttpUtility.UrlDecode(txtTieuDe1);
            txtTieuDe2 = HttpUtility.UrlDecode(txtTieuDe2);
            sTenDonViCapTren = HttpUtility.UrlDecode(sTenDonViCapTren);
            sTenDonViCapDuoi = HttpUtility.UrlDecode(sTenDonViCapDuoi);

            ExcelFile xls = FileBaoCaoQuyetToanNguonChiDacBiet(txtTieuDe1, txtTieuDe2, sTenDonViCapTren, sTenDonViCapDuoi, txtNamKeHoach);
            string sFileName = "Báo cáo quyết toán thuộc các nguồn chi đặc biệt";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName);
        }

        private ExcelFile FileBaoCaoQuyetToanNguonChiDacBiet(string txtTieuDe1, string txtTieuDe2, string sTenDonViCapTren, string sTenDonViCapDuoi, int? iNamKeHoach)
        {
            XlsFile Result = new XlsFile(true);
            string sFilePathBaoCaoQuyetToanNguonChiDacBiet = "/Report_ExcelFrom/QLNH/rpt_QuyetToan_NguonChiDacBiet.xlsx";
            Result.Open(Server.MapPath(sFilePathBaoCaoQuyetToanNguonChiDacBiet));

            FlexCelReport fr = new FlexCelReport();

            List<NH_QTND_NguonChiDacBietReport> listQTNDNguonChiDacBiet = _qlnhService.GetBaoCaoQuyetToanNguonChiDacBiet(iNamKeHoach).ToList();
            List<NH_QTND_NguonChiDacBietReport> listQTND = new List<NH_QTND_NguonChiDacBietReport>();
            List<NH_QTND_NguonChiDacBietReport> listNotSum = listQTNDNguonChiDacBiet.Where(x => x.IsData.HasValue && x.IsData.Value != 3).ToList();
            List<NH_QTND_NguonChiDacBietReport> listSum = listQTNDNguonChiDacBiet.Where(x => x.IsData.HasValue && x.IsData.Value == 3).ToList();

            listQTND.AddRange(listNotSum);
            listQTND.AddRange(listSum);

            List<NH_QTND_NguonChiDacBiet_ExportModel> listExportQTND = listQTND.Select(x => new NH_QTND_NguonChiDacBiet_ExportModel
            {
                position = x.position,
                IsBold = x.IsBold,
                IsData = x.IsData,
                sTen = x.sTen,
                sQTKinhPhiDuocCap_TongSo_USD = CommonFunction.DinhDangSo((x.fQTKinhPhiDuocCap_TongSo_USD.HasValue ? x.fQTKinhPhiDuocCap_TongSo_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sQTKinhPhiDuocCap_TongSo_VND = CommonFunction.DinhDangSo((x.fQTKinhPhiDuocCap_TongSo_VND.HasValue ? x.fQTKinhPhiDuocCap_TongSo_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sQTKinhPhiDuocCap_NamNay_USD = CommonFunction.DinhDangSo((x.fQTKinhPhiDuocCap_NamNay_USD.HasValue ? x.fQTKinhPhiDuocCap_NamNay_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sQTKinhPhiDuocCap_NamNay_VND = CommonFunction.DinhDangSo((x.fQTKinhPhiDuocCap_NamNay_VND.HasValue ? x.fQTKinhPhiDuocCap_NamNay_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sQTKinhPhiDuocCap_NamTruocChuyenSang_USD = CommonFunction.DinhDangSo((x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD.HasValue ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sQTKinhPhiDuocCap_NamTruocChuyenSang_VND = CommonFunction.DinhDangSo((x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND.HasValue ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sDeNghiQTNamNay_USD = CommonFunction.DinhDangSo((x.fDeNghiQTNamNay_USD.HasValue ? x.fDeNghiQTNamNay_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sDeNghiQTNamNay_VND = CommonFunction.DinhDangSo((x.fDeNghiQTNamNay_VND.HasValue ? x.fDeNghiQTNamNay_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sDeNghiChuyenNamSau_USD = CommonFunction.DinhDangSo((x.fDeNghiChuyenNamSau_USD.HasValue ? x.fDeNghiChuyenNamSau_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sDeNghiChuyenNamSau_VND = CommonFunction.DinhDangSo((x.fDeNghiChuyenNamSau_VND.HasValue ? x.fDeNghiChuyenNamSau_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sThuaThieuKinhPhiTrongNam_USD = CommonFunction.DinhDangSo((x.fThuaThieuKinhPhiTrongNam_USD.HasValue ? x.fThuaThieuKinhPhiTrongNam_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sThuaThieuKinhPhiTrongNam_VND = CommonFunction.DinhDangSo((x.fThuaThieuKinhPhiTrongNam_VND.HasValue ? x.fThuaThieuKinhPhiTrongNam_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
            }).ToList();

            fr.AddTable<NH_QTND_NguonChiDacBiet_ExportModel>("dt", listExportQTND);
            fr.SetValue(new
            {
                txtTieuDe1 = txtTieuDe1?.ToUpper(),
                txtTieuDe2 = txtTieuDe2,
                year = iNamKeHoach?.ToString(),
                sTenDonViCapTren = sTenDonViCapTren?.ToUpper(),
                sTenDonViCapDuoi = sTenDonViCapDuoi?.ToUpper()
            });

            fr.UseChuKy(Username).UseChuKyForController("BaoCaoQuyetToanNguonChiDacBiet").UseForm(this).Run(Result);

            return Result;
        }

        private List<Dropdown_SelectValue> GetListNamKeHoach()
        {
            List<Dropdown_SelectValue> listNam = new List<Dropdown_SelectValue>();
            int namHienTai = DateTime.Now.Year + 1;
            for (int i = 20; i > 0; i--)
            {
                namHienTai -= 1;
                Dropdown_SelectValue namKeHoachOpt = new Dropdown_SelectValue()
                {
                    Value = namHienTai,
                    Label = namHienTai.ToString()
                };
                listNam.Add(namKeHoachOpt);
            }
            return listNam;
        }

        [HttpPost]
        public ActionResult GetModalInBaoCao(int slbNamKeHoach)
        {
            ViewBag.iNamKeHoach = slbNamKeHoach;
            return PartialView("_modalReport");
        }
    }
}