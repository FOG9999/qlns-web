using DomainModel;
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
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.QLNH.Controllers.DuAnHopDong
{
    public class ChenhLechTiGiaHoiDoaiController : FlexcelReportController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sControlName = "ChenhLechTiGiaHoiDoai";
        private readonly string TITLE_FIRST_DEFAULT_VALUE = "BÁO CÁO CHÊNH LỆCH TỈ GIÁ HỐI ĐOÁI THEO HỢP ĐỒNG CỦA NGUỒN QUỸ DỰ TRỮ NGOẠI HỐI";

        // GET: QLNH/ChenhLechTiGiaHoiDoai
        public ActionResult Index()
        {
            ChenhLechTiGiaViewModel vm = new ChenhLechTiGiaViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetAllChenhLechTiGia(ref vm._paging, null, null, null);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.DonViList = lstDonViQL;

            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
            vm.ChuongTrinhList = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            List<NH_DA_HopDong> lstHopDong = _qlnhService.GetNHDAHopDongList(null).ToList();
            lstHopDong.Insert(0, new NH_DA_HopDong { ID = Guid.Empty, sTenHopDong = "--Chọn hợp đồng--" });
            vm.HopDongList = lstHopDong;
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, Guid? iDonVi, string maDonVi, Guid? iChuongTrinh, Guid? iHopDong)
        {
            ChenhLechTiGiaViewModel vm = new ChenhLechTiGiaViewModel();
            vm._paging = _paging;
            vm.Items = _qlnhService.GetAllChenhLechTiGia(ref vm._paging,
                iDonVi == Guid.Empty ? null : iDonVi,
                iChuongTrinh == Guid.Empty ? null : iChuongTrinh,
                iHopDong == Guid.Empty ? null : iHopDong);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.DonViList = lstDonViQL;

            maDonVi = HttpUtility.HtmlDecode(maDonVi);
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHNhiemVuChiTietTheoDonViId(maDonVi, iDonVi).ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
            vm.ChuongTrinhList = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            List<NH_DA_HopDong> lstHopDong = _qlnhService.GetNHDAHopDongList(iChuongTrinh).ToList();
            lstHopDong.Insert(0, new NH_DA_HopDong { ID = Guid.Empty, sTenHopDong = "--Chọn hợp đồng--" });
            vm.HopDongList = lstHopDong;
            return PartialView("_list", vm);
        }

        [HttpPost]
        public JsonResult ChangeSelectDonVi(Guid? iDonVi, string maDonVi)
        {
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHNhiemVuChiTietTheoDonViId(maDonVi, iDonVi).ToList();
            StringBuilder htmlChuongtrinh = new StringBuilder();
            htmlChuongtrinh.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn chương trình--");
            if (lstChuongTrinh != null && lstChuongTrinh.Count() > 0)
            {
                for (int i = 0; i < lstChuongTrinh.Count; i++)
                {
                    htmlChuongtrinh.AppendFormat("<option value='{0}'>{1}</option>", lstChuongTrinh[i].ID, HttpUtility.HtmlEncode(lstChuongTrinh[i].sTenNhiemVuChi));
                }
            }

            List<NH_DA_HopDong> lstHopDong = _qlnhService.GetNHDAHopDongList(null).ToList();
            StringBuilder htmlHD = new StringBuilder();
            htmlHD.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn hợp đồng--");
            if (lstHopDong != null && lstHopDong.Count() > 0)
            {
                for (int i = 0; i < lstHopDong.Count; i++)
                {
                    htmlHD.AppendFormat("<option value='{0}'>{1}</option>", lstHopDong[i].ID, HttpUtility.HtmlEncode(lstHopDong[i].sTenHopDong));
                }
            }
            return Json(new { htmlCT = htmlChuongtrinh.ToString(), htmlHD = htmlHD.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeSelectChuongTrinh(Guid? iChuongTrinh)
        {
            List<NH_DA_HopDong> lstHopDong = _qlnhService.GetNHDAHopDongList(iChuongTrinh).ToList();
            StringBuilder htmlHD = new StringBuilder();
            htmlHD.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn hợp đồng--");
            if (lstHopDong != null && lstHopDong.Count() > 0)
            {
                for (int i = 0; i < lstHopDong.Count; i++)
                {
                    htmlHD.AppendFormat("<option value='{0}'>{1}</option>", lstHopDong[i].ID, HttpUtility.HtmlEncode(lstHopDong[i].sTenHopDong));
                }
            }
            return Json(htmlHD.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult OpenModalBaoCao()
        {
            ViewBag.Title = "BÁO CÁO CHÊNH LỆCH TỈ GIÁ HỐI ĐOÁI";
            ViewBag.TieuDe1 = TITLE_FIRST_DEFAULT_VALUE;
            return PartialView("_reportModal");
        }

        [ValidateInput(false)]
        public ActionResult ExportChenhLechTiGia(Guid? iDonVi, Guid? iChuongTrinh, Guid? iHopDong, string tieude1, string tieude2, string tieude3, string tendonvicaptren, string tendonvi, string ext = "xlsx")
        {
            tieude1 = HttpUtility.UrlDecode(tieude1);
            tieude2 = HttpUtility.UrlDecode(tieude2);
            tieude3 = HttpUtility.UrlDecode(tieude3);
            tendonvicaptren = HttpUtility.UrlDecode(tendonvicaptren);
            tendonvi = HttpUtility.UrlDecode(tendonvi);

            ExcelFile xls = FileBaoCaoChenhLech(iDonVi == Guid.Empty ? null : iDonVi
                                            , iChuongTrinh == Guid.Empty ? null : iChuongTrinh
                                            , iHopDong == Guid.Empty ? null : iHopDong
                                            , tieude1, tieude2, tieude3, tendonvicaptren, tendonvi);
            string sFileName = "Báo cáo chênh lệch tỉ giá hối đoái";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName);
        }

        private ExcelFile FileBaoCaoChenhLech(Guid? iDonVi, Guid? iChuongTrinh, Guid? iHopDong, string tieude1, string tieude2, string tieude3, string tendonvicaptren, string tendonvi)
        {
            XlsFile Result = new XlsFile(true);
            string sFilePathBaoCaoChenhLechTiGia = "/Report_ExcelFrom/QLNH/rpt_ChenhLechTiGiaHoiDoai.xlsx";
            Result.Open(Server.MapPath(sFilePathBaoCaoChenhLechTiGia));

            FlexCelReport fr = new FlexCelReport();

            List<ChenhLechTiGiaExportModel> lstChenhLechTiGia = _qlnhService.GetDataExportChenhLechTiGia(iDonVi, iChuongTrinh, iHopDong).Select(x => new ChenhLechTiGiaExportModel
            {
                sTen = x.sTenDisplay,
                sTienKHTTBQPCapUSD = CommonFunction.DinhDangSo((x.fTienKHTTBQPCapUSD.HasValue ? x.fTienKHTTBQPCapUSD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sTienKHTTBQPCapVND = CommonFunction.DinhDangSo((x.fTienKHTTBQPCapVND.HasValue ? x.fTienKHTTBQPCapVND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sTienTheoHopDongUSD = CommonFunction.DinhDangSo((x.fTienTheoHopDongUSD.HasValue ? x.fTienTheoHopDongUSD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sTienTheoHopDongVND = CommonFunction.DinhDangSo((x.fTienTheoHopDongVND.HasValue ? x.fTienTheoHopDongVND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sKinhPhiDuocCapChoCDTUSD = CommonFunction.DinhDangSo((x.fKinhPhiDuocCapChoCDTUSD.HasValue ? x.fKinhPhiDuocCapChoCDTUSD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sKinhPhiDuocCapChoCDTVND = CommonFunction.DinhDangSo((x.fKinhPhiDuocCapChoCDTVND.HasValue ? x.fKinhPhiDuocCapChoCDTVND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sKinhPhiDaThanhToanUSD = CommonFunction.DinhDangSo((x.fKinhPhiDaThanhToanUSD.HasValue ? x.fKinhPhiDaThanhToanUSD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 2),
                sKinhPhiDaThanhToanVND = CommonFunction.DinhDangSo((x.fKinhPhiDaThanhToanVND.HasValue ? x.fKinhPhiDaThanhToanVND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty), 0),
                sTiGiaCLHopDongVsCDTUSD = GetGiaTienChenhLech(x.fTiGiaCLHopDongVsCDTUSD, 2),
                sTiGiaCLHopDongVsCDTVND = GetGiaTienChenhLech(x.fTiGiaCLHopDongVsCDTVND, 0),
                sTiGiaCLKinhPhiDuocCapVsGiaiNganUSD = GetGiaTienChenhLech(x.fTiGiaCLKinhPhiDuocCapVsGiaiNganUSD, 2),
                sTiGiaCLKinhPhiDuocCapVsGiaiNganVND = GetGiaTienChenhLech(x.fTiGiaCLKinhPhiDuocCapVsGiaiNganVND, 0),
                IsBold = x.IsBold
            }).ToList();

            fr.AddTable<ChenhLechTiGiaExportModel>("dt", lstChenhLechTiGia);
            fr.SetValue("sTenDonViCapTren", tendonvicaptren);
            fr.SetValue("sTenDonViCapDuoi", tendonvi);
            fr.SetValue("TieuDe1", tieude1.IsEmpty("") ? TITLE_FIRST_DEFAULT_VALUE : tieude1);
            fr.SetValue("TieuDe2", tieude2);
            fr.SetValue("TieuDe3", tieude3);

            fr.UseChuKy(Username).UseChuKyForController(sControlName).UseForm(this).Run(Result);

            return Result;
        }

        private string GetGiaTienChenhLech(double? giaTien, int toFixed)
        {
            string sGiaTienChenhLech = string.Empty;
            if (giaTien.HasValue)
            {
                if (giaTien.Value == 0)
                {
                    sGiaTienChenhLech = toFixed == 2 ? "0,00" : "0";
                }
                else
                {
                    sGiaTienChenhLech = CommonFunction.DinhDangSo(giaTien.Value.ToString(CultureInfo.InvariantCulture), toFixed);
                    if (giaTien.Value < 0)
                    {
                        sGiaTienChenhLech = "-" + sGiaTienChenhLech;
                    }
                }
            }
            return sGiaTienChenhLech;
        }
    }
}