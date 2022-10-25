using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNguonNganSach;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.QLNguonNganSach.Controllers
{
    public class BaoCaoNguonNSController : FlexcelReportController
    {
        private readonly IDanhMucService _danhMucService = DanhMucService.Default;
        private readonly IQLNguonNganSachService _nnsService = QLNguonNganSachService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sFilePathBaoCao1 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao1.xlsx";
        private const string sFilePathBaoCao5_TheoDonVi = "/Report_ExcelFrom/NguonNS/rpt_TongHopDuToanNS_TheoDonVi.xls";
        private const string sFilePathBaoCao5_TheoDonViBQL = "/Report_ExcelFrom/NguonNS/rpt_TongHopDuToanNS_TheoDonViBQL.xls";
        private const string sFilePathBaoCao5_TheoBQLDonVi = "/Report_ExcelFrom/NguonNS/rpt_TongHopDuToanNS_TheoBQLDonVi.xls";
        private const string sFilePathBaoCao2 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao2.xls";
        private const string sFilePathBaoCao3_0 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_0.xls";
        private const string sFilePathBaoCao3_0_T2 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_0_T2.xls";
        private const string sFilePathBaoCao3_0_Hide_TongCongNguon = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_0_Hide_TongCongNguon.xls";
        private const string sFilePathBaoCao3_0_Dynamic_TongCongNguon = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_0_Dynamic_TongCongNguon.xls";
        private const string sFilePathBaoCao3_1 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_1.xls";
        private const string sFilePathBaoCao3_ChiTiet_0 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_ChiTiet_0.xls";
        private const string sFilePathBaoCao3_ChiTiet_1 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_ChiTiet_1.xls";
        private const string sFilePathBaoCao3_ChiTietPhanCap_0 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_ChiTietPhanCap_0.xls";
        private const string sFilePathBaoCao3_ChiTietPhanCap_1 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao3_ChiTietPhanCap_1.xls";
        private const string sFilePathBaoCao4_ChiTiet = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao4_ChiTiet.xls";
        private const string sFilePathBaoCao4_ChiTietPhanCap = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao4_ChiTietPhanCap.xls";
        private const string sFilePathBaoCao4 = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao4.xls";
        private const string sFilePathBaoCao6_TheoDonVi = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao6_TheoDonVi.xls";
        private const string sFilePathBaoCao6_TheoBQLDonVi = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao6_TheoBQLDonVi.xls";
        private const string sFilePathBaoCao6_TheoDonViBQL = "/Report_ExcelFrom/NguonNS/rpt_NguonNS_BaoCao6_TheoDonViBQL.xls";
        private const string sControlName = "BaoCaoNguonNS";
        private int _columnCountBC1 = 7;
        private int _columnCountBC3 = 7;
        private int _columnCountBC3_ChiTiet = 8;
        private int _columnCountBC6 = 5;

        // GET: QLNguonNganSach/BaoCaoNguonNS
        public ActionResult Index()
        {
            List<NNSDMNguonViewModel> lstNguon = _danhMucService.GetAllDMNguonBaoCao(PhienLamViec.NamLamViec).ToList();
            lstNguon.Insert(0, new NNSDMNguonViewModel { iID_Nguon = Guid.Empty, sNoiDung = Constants.TAT_CA });
            ViewBag.ListDanhMucNguon = lstNguon.ToSelectList("iID_Nguon", "sNoiDung");

            DateTime firstDay = new DateTime(PhienLamViec.NamLamViec, 1, 1);
            DateTime lastDay = new DateTime(PhienLamViec.NamLamViec, 12, 31);

            ViewBag.dNgayFromDefault = firstDay.ToString("dd/MM/yyyy");
            ViewBag.dNgayToDefault = lastDay.ToString("dd/MM/yyyy");
            return View();
        }

        public JsonResult LayDanhSachDMNguonTheoLoaiNganSach(int iLoaiNganSach)
        {
            List<NNSDMNguonViewModel> lstNguon = _danhMucService.GetAllDMNguonBaoCao(PhienLamViec.NamLamViec, iLoaiNganSach).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.TAT_CA);
            if (lstNguon != null && lstNguon.Count > 0)
            {
                for (int i = 0; i < lstNguon.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}'>{1}</option>", lstNguon[i].iID_Nguon, lstNguon[i].sNoiDung);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BaoCaoTongHopNguonPartial(Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
            {
                iNguonNganSach = null;
            }

            int iNamLamViec = PhienLamViec.NamLamViec;//2021;

            var dotNhanBoSungNam = _nnsService.GetAllDotNhanBoSungNam(iNguonNganSach, iNamLamViec, dDateFrom, dDateTo);
            var dotDuToanNam = _nnsService.GetAllDotDuToan(iNguonNganSach, iNamLamViec, dotNhanBoSungNam.Count(), dDateFrom, dDateTo);

            string sTenCot = string.Empty;
            foreach(var item in dotNhanBoSungNam)
            {
                sTenCot += string.Format("[{0}],", item.SoCot);
            }

            foreach (var item in dotDuToanNam)
            {
                sTenCot += string.Format("[{0}],", item.SoCot);
            }

            sTenCot = sTenCot.Substring(0, sTenCot.Length - 1);
            var dataBaoCaoTongHop = _nnsService.BaoCaoTongHopNguon(iNguonNganSach, iNamLamViec, dotNhanBoSungNam.Count(), dDateFrom, dDateTo, dvt);
            var lstChiTietDotBoSungDotDuToan = _nnsService.GetChiTietDotNhanVaDotBoSung(iNguonNganSach, iNamLamViec, dDateFrom, dDateTo, dvt, sTenCot);


            // update tong tien hang cha
            int dataCount = dataBaoCaoTongHop.Count();
            for (int i = dataCount - 1; i >= 0; i--)
            {
                decimal tongDaGiaoDauNam = 0, tongDaGiaoChuyenSang = 0, tongDaGiaoDuToan = 0, tongDaGiaoTongDuToan = 0, tongConLai = 0;
                List<decimal> listTongDotDuToan = Enumerable.Repeat(decimal.Zero, dotDuToanNam.Count()).ToList();
                if (!string.IsNullOrEmpty(dataBaoCaoTongHop[i].bLaHangCha))
                {
                    for (int j = i + 1; j < dataCount; j++)
                    {
                        if (int.Parse(dataBaoCaoTongHop[i].depth) + 1 == int.Parse(dataBaoCaoTongHop[j].depth))
                        {
                            if (lstChiTietDotBoSungDotDuToan != null)
                            {
                                var dataChiTietDotDuToanDotBoSung = lstChiTietDotBoSungDotDuToan.Select("iID_Nguon = '" + dataBaoCaoTongHop[j].iID_Nguon + "'")[0];
                                if (dataChiTietDotDuToanDotBoSung != null)
                                {
                                    for (int iDotDuToan = 0; iDotDuToan < dotDuToanNam.Count(); iDotDuToan++)
                                    {
                                        string soTien = dataChiTietDotDuToanDotBoSung["c" + dotDuToanNam.ToList()[iDotDuToan].SoCot].ToString();
                                        listTongDotDuToan[iDotDuToan] += string.IsNullOrEmpty(soTien) ? (decimal)0 : decimal.Parse(soTien);
                                    }
                                }
                            }

                            tongDaGiaoDauNam += dataBaoCaoTongHop[j].dagiaoDauNam;
                            tongDaGiaoChuyenSang += dataBaoCaoTongHop[j].dagiaoChuyenSang;
                            tongDaGiaoDuToan += dataBaoCaoTongHop[j].dagiaoDuToan;
                            tongDaGiaoTongDuToan += dataBaoCaoTongHop[j].dagiaoTongDuToan;
                            tongConLai += dataBaoCaoTongHop[j].conLai;
                        }
                        if (int.Parse(dataBaoCaoTongHop[i].depth) >= int.Parse(dataBaoCaoTongHop[j].depth) || j == dataCount - 1)
                        {
                            if (lstChiTietDotBoSungDotDuToan != null)
                            {
                                var dataChiTietCha = lstChiTietDotBoSungDotDuToan.Select("iID_Nguon = '" + dataBaoCaoTongHop[i].iID_Nguon + "'")[0];
                                if (dataChiTietCha != null)
                                {
                                    for (int iDotDuToan = 0; iDotDuToan < dotDuToanNam.Count(); iDotDuToan++)
                                    {
                                        string soTien = dataChiTietCha["c" + dotDuToanNam.ToList()[iDotDuToan].SoCot].ToString();
                                        dataChiTietCha["c" + dotDuToanNam.ToList()[iDotDuToan].SoCot] = (string.IsNullOrEmpty(soTien) ? (decimal)0 : decimal.Parse(soTien)) + listTongDotDuToan[iDotDuToan];
                                    }
                                }
                            }
                            dataBaoCaoTongHop[i].dagiaoDauNam = tongDaGiaoDauNam;
                            dataBaoCaoTongHop[i].dagiaoChuyenSang = tongDaGiaoChuyenSang;
                            dataBaoCaoTongHop[i].dagiaoDuToan = tongDaGiaoDuToan;
                            dataBaoCaoTongHop[i].dagiaoTongDuToan = tongDaGiaoTongDuToan;
                            dataBaoCaoTongHop[i].conLai = tongConLai;

                            break;
                        }
                    }
                }
            }

            ViewBag.ListChiTietDotDuToanDotBoSung = lstChiTietDotBoSungDotDuToan;
            ViewBag.ListDotNhanBoSung = dotNhanBoSungNam;
            ViewBag.ListDotDuToan = dotDuToanNam;
            ViewBag.SoCotBoSung = dotNhanBoSungNam.Count();
            ViewBag.SoCotDaGiao = dotDuToanNam.Count();
            var dataBoSung = dotNhanBoSungNam.Select(x => x.SoCot);
            var tinhTong = _nnsService.GetTongSoTienBaoCao(dataBaoCaoTongHop, dotNhanBoSungNam, dotDuToanNam);

            string TongSoBoSung = string.Join("+", dataBoSung.Select(s => string.Format("({0})", s)));
            if (dataBoSung.Count() > 0)
                TongSoBoSung = "=" + TongSoBoSung;

            var dataDuToan = dotDuToanNam.Select(x => x.SoCot);
            string sTongSoDaGiao = string.Join("+", dataDuToan.Select(s => string.Format("({0})", s)));
            if (dataDuToan.Count() > 0)
                sTongSoDaGiao = "=" + sTongSoDaGiao;

            ViewBag.TongsoBoSung = TongSoBoSung;
            ViewBag.TongsoDuToan = sTongSoDaGiao;
            ViewBag.TinhTong = tinhTong;
            ViewBag.iNamLamViec = iNamLamViec;
            TempData["BaoCaoTongHopNguonBTC"] = dataBaoCaoTongHop;
            TempData["DotNhanBoXung"] = dotNhanBoSungNam;
            TempData["DotDuToan"] = dotDuToanNam;
            TempData["TongTienBaoCaoTongHopNguonBTC"] = tinhTong;
            TempData["ChiTietDotBoSungDuToan"] = lstChiTietDotBoSungDotDuToan;
            return PartialView("_tonghopnguon", dataBaoCaoTongHop);
        }

        [HttpPost]
        public ActionResult TongHopGiaoDuToanNewPartial(int iLoaiNganSach, Guid? iID_Nguon, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            int iTotalCol = 0;
            if (iID_Nguon == Guid.Empty)
                iID_Nguon = null;
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            data.lstTongNNS = _nnsService.GetTotalNganSachBQPByNamLamViec(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon, dvt);
            if (data.lstTongNNS != null && data.lstTongNNS.Any(n => n.fTongTien.HasValue))
            {
                ViewBag.iTotalMoney = data.lstTongNNS.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }

            data.lstNoiDungChi = _danhMucService.GetAllDMNoiDungChiBQP(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon);

            var lstTreeNoiDungChi = _danhMucService.GetTreeAllDMNoiDungChiForReport(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon);
            if (lstTreeNoiDungChi == null || !lstTreeNoiDungChi.Any())
                return null;

            foreach (DMNoiDungChiViewModel item in lstTreeNoiDungChi)
                item.fSoCon = CountChild(item, lstTreeNoiDungChi.ToList());

            data.treeNoiDungChi = lstTreeNoiDungChi;
            iTotalCol = data.treeNoiDungChi.Count(n => !n.bLaHangCha);

            data.lstTongTienDuToan = _nnsService.GetTotalDuToanByNamLamViec(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon, dDateFrom, dDateTo, dvt);
            if (data.lstTongTienDuToan != null && data.lstTongTienDuToan.Any(n => n.fTongTien.HasValue))
            {
                ViewBag.sSoQuyetDinhTong = data.lstTongTienDuToan.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).FirstOrDefault() != null ? data.lstTongTienDuToan.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).FirstOrDefault().sSoQuyetDinh : "";
                if (data.lstTongTienDuToan.FirstOrDefault().dNgayQuyetDinh.HasValue)
                {
                    ViewBag.dNgayQuyetDinhTong = data.lstTongTienDuToan.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).FirstOrDefault().dNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                }
                ViewBag.sTongTienDuToan = data.lstTongTienDuToan.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }

            var lstDataDuToanTheoNam = _nnsService.GetDetailDuToanByNamLamViec(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon, dDateFrom, dDateTo, sSoQuyetDinh, dvt);

            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();
            data.lstDuToanChoPheDuyet = lstDataDuToanTheoNam.Where(x => string.IsNullOrEmpty(x.sSoQuyetDinh) && x.iID_NhiemVu != Guid.Empty && !string.IsNullOrEmpty(x.sMaNoiDungChi)).GroupBy(g => new { g.iID_NhiemVu, g.sMaNoiDungChi }).Select(x => new RptNNSDuToanChiTietModel()
            {
                dNgayQuyetDinh = x.First().dNgayQuyetDinh,
                sMaNoiDungChi = x.Key.sMaNoiDungChi,
                iID_NhiemVu = x.Key.iID_NhiemVu,
                iTongTien = x.Sum(g => g.iTongTien),
                GhiChu = x.First().GhiChu,
                iID_NoiDungChi = x.First().iID_NoiDungChi,
                iID_Parent = x.First().iID_Parent
            }).ToList();

            List<DMNoiDungChiViewModel> listNDC = data.treeNoiDungChi.Where(x => x.depth == "0").ToList();
            ViewBag.listNDC = listNDC;
            ViewBag.iTotalCol = iTotalCol;
            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            ViewBag.iLoaiNganSach = iLoaiNganSach;
            TempData["BaoCaoTongHopGiaoDuToan"] = data;
            ViewBag.dvt = dvt;
            if (iLoaiNganSach == (int)Constants.LoaiNganSach.Type.CHI_NGAN_SACH_NHA_NUOC)
                return PartialView("TongHopGiaoDuToanPartial_NSNN", data);
            else if (iLoaiNganSach == (int)Constants.LoaiNganSach.Type.CHI_THUONG_XUYEN_QP)
                return PartialView("TongHopGiaoDuToanPartial_TXQP", data);
            return null;
        }

        public int CountChild(DMNoiDungChiViewModel category, List<DMNoiDungChiViewModel> listNDC)
        {
            int fOldSoCon = category.fSoCon;
            foreach (var innerCategory in listNDC)
            {
                if (innerCategory.iID_Parent == category.iID_NoiDungChi)
                {
                    if (innerCategory.bLaHangCha)
                    {
                        fOldSoCon += CountChild(innerCategory, listNDC);
                    }
                    else
                    {
                        fOldSoCon++;
                    }
                }

            }
            return fOldSoCon;
        }

        public ActionResult ChiTietQuyetDinhPartial(int iLoaiNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();

            var lstTreeNoiDungChi = _danhMucService.GetTreeAllDMNoiDungChiForReport(PhienLamViec.NamLamViec, iLoaiNganSach, null);
            if (lstTreeNoiDungChi == null || !lstTreeNoiDungChi.Any())
                return RedirectToAction("Index");

            foreach (DMNoiDungChiViewModel item in lstTreeNoiDungChi)
                item.fSoCon = CountChild(item, lstTreeNoiDungChi.ToList());

            data.treeNoiDungChi = lstTreeNoiDungChi;
            var lstDataDuToanTheoNam = _nnsService.GetDuToanTheoNhiemVu(PhienLamViec.NamLamViec, iLoaiNganSach, sSoQuyetDinh, dDateFrom, dDateTo, dvt);

            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();
            data.lstDuToanChoPheDuyet = lstDataDuToanTheoNam.Where(x => string.IsNullOrEmpty(x.sSoQuyetDinh) && x.iID_NhiemVu != Guid.Empty && !string.IsNullOrEmpty(x.sMaNoiDungChi)).GroupBy(g => new { g.iID_NhiemVu, g.sMaNoiDungChi }).Select(x => new RptNNSDuToanChiTietModel()
            {
                dNgayQuyetDinh = x.First().dNgayQuyetDinh,
                sMaNoiDungChi = x.Key.sMaNoiDungChi,
                iID_NhiemVu = x.Key.iID_NhiemVu,
                iTongTien = x.Sum(g => g.iTongTien),
                GhiChu = x.First().GhiChu
            }).ToList();

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            ViewBag.iLoaiNganSach = iLoaiNganSach;
            ViewBag.dvt = dvt;
            TempData["DataChiTietQuyetDinhPartial"] = data;
            return View("PopupChiTietQuyetDinhPartial", data);
        }

        public JsonResult Ds_To_BaoCaoChiTietQuyetDinh()
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["DataChiTietQuyetDinhPartial"] != null)
            {
                data = (RptTongHopGiaoDuToanModel)TempData["DataChiTietQuyetDinhPartial"];
                TempData.Keep("DataChiTietQuyetDinhPartial");
            }
            int totalColumn = 0;
            if (data.treeNoiDungChi != null && data.treeNoiDungChi.Any())
                totalColumn = data.treeNoiDungChi.Where(x => x.fSoCon == 0).Count();

            return ds_ToIn(totalColumn, _columnCountBC3_ChiTiet);
        }

        public ActionResult ChiTietQuyetDinhPartialExport(string ext = "pdf", int dvt = 1, int iLoaiNganSach = 0, int to = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong_hop_du_toan_nsqp_giao_cho_dv_theo_soquyetdinh", ext);
            ExcelFile xls = TaoFileBaoCaoChiTietQuyetDinhPartial(dvt, iLoaiNganSach, to);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCaoChiTietQuyetDinhPartial(int dvt = 1, int iLoaiNganSach = 0, int to =1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["DataChiTietQuyetDinhPartial"] != null)
            {
                data = (RptTongHopGiaoDuToanModel)TempData["DataChiTietQuyetDinhPartial"];
                TempData.Keep("DataChiTietQuyetDinhPartial");
            }

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(iLoaiNganSach == 0 ? sFilePathBaoCao3_ChiTiet_0 : sFilePathBaoCao3_ChiTiet_1));
            FlexCelReport fr = new FlexCelReport();

            Header objHeader = new Header();
            objHeader.lstHeaderLv1 = new List<HeaderInfo>();
            objHeader.lstHeaderLv2 = new List<HeaderInfo>();
            objHeader.lstHeaderLv3 = new List<HeaderInfo>();

            foreach (DMNoiDungChiViewModel item in data.treeNoiDungChi)
            {
                if (iLoaiNganSach == 0)
                {
                    // header 3 tang
                    // level1
                    if (item.depth == "0")
                    {
                        if (item.fSoCon == 0)
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                        else if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv1.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 2
                    else if (item.depth == "1")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv2.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 3
                    else if (item.depth == "2")
                    {
                        objHeader.lstHeaderLv3.Add(new HeaderInfo
                        {
                            iID_Header = item.iID_NoiDungChi,
                            sTen = item.sTenNoiDungChi
                        });
                    }
                }
                else if (iLoaiNganSach == 1)
                {
                    // level 1
                    if (item.depth == "0")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv1.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 2
                    else if (item.depth == "1")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv3.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                }
            }

            List<RptNNSDuToanChiTietModel> lstChiTiet = data.lstDuToanChiTiet.ToList().Where(x => x.iID_Parent == null).ToList();
            lstChiTiet = lstChiTiet.GroupBy(x => new
            {
                x.iID_MaDonVi,
                x.sMaPhongBan,
                x.iID_NhiemVu
            }).Select(g => new RptNNSDuToanChiTietModel()
            {
                iID_MaDonVi = g.Key.iID_MaDonVi,
                sMaPhongBan = g.Key.sMaPhongBan,
                iID_NhiemVu = g.Key.iID_NhiemVu,
                sTenPhongBan = g.First().sTenPhongBan,
                TenDonVi = g.First().TenDonVi,
                GhiChu = g.First().GhiChu,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            List<PhongBanNhiemVu> lstPhongBan = lstChiTiet.GroupBy(x => new
            {
                x.sMaPhongBan,
                x.iID_NhiemVu
            }).Select(g => new PhongBanNhiemVu
            {
                sMaPhongBan = g.Key.sMaPhongBan,
                iID_NhiemVu = g.Key.iID_NhiemVu,
                sTenPhongBan = g.First().sTenPhongBan,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            List<NhiemVu> lstNhiemVu = lstChiTiet.GroupBy(x => x.iID_NhiemVu).Select(g => new NhiemVu
            {
                iID_NhiemVu = g.Key,
                sGhiChu = g.First().GhiChu,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            #region set value for row chitiet
            foreach (RptNNSDuToanChiTietModel itemCt in lstChiTiet)
            {
                itemCt.lstGiaTriPhanBo = new List<GiaTri>();
                foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
                {
                    var value = data.lstDuToanChiTiet.Where(x => x.sMaPhongBan == itemCt.sMaPhongBan && x.iID_NoiDungChi == itemHd.iID_Header && x.iID_MaDonVi == itemCt.iID_MaDonVi).ToList();
                    if (value != null && value.Any())
                    {
                        itemCt.lstGiaTriPhanBo.Add(new GiaTri
                        {
                            fSoTien = (double?)value.Sum(x => x.iTongTien ?? 0)
                        });
                    }
                    else
                    {
                        itemCt.lstGiaTriPhanBo.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                    }
                }
            }
            #endregion

            #region set value for row phong ban
            foreach (PhongBanNhiemVu itemCt in lstPhongBan)
            {
                itemCt.lstGiaTriPhongBan = new List<GiaTri>();
                foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
                {
                    var value = data.lstDuToanChiTiet.Where(x => x.sMaPhongBan == itemCt.sMaPhongBan && x.iID_NoiDungChi == itemHd.iID_Header && x.iID_NhiemVu == itemCt.iID_NhiemVu).ToList();
                    if (value != null && value.Any())
                    {
                        itemCt.lstGiaTriPhongBan.Add(new GiaTri
                        {
                            fSoTien = (double?)value.Sum(x => x.iTongTien ?? 0)
                        });
                    }
                    else
                    {
                        itemCt.lstGiaTriPhongBan.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                    }
                }
            }
            #endregion

            #region set value for row nhiem vu
            foreach (NhiemVu itemCt in lstNhiemVu)
            {
                itemCt.lstGiaTriNhiemVu = new List<GiaTri>();
                foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
                {
                    var value = data.lstDuToanChiTiet.Where(x => x.iID_NoiDungChi == itemHd.iID_Header && x.iID_NhiemVu == itemCt.iID_NhiemVu).ToList();
                    if (value != null && value.Any())
                    {
                        itemCt.lstGiaTriNhiemVu.Add(new GiaTri
                        {
                            fSoTien = (double?)value.Sum(x => x.iTongTien ?? 0)
                        });
                    }
                    else
                    {
                        itemCt.lstGiaTriNhiemVu.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                    }
                }
            }
            #endregion

            #region set value for row Tong
            List<GiaTri> lstTong = new List<GiaTri>();
            foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
            {
                var value = data.lstDuToanChiTiet.Where(x => x.iID_NoiDungChi == itemHd.iID_Header).ToList();
                if (value != null && value.Any())
                {
                    lstTong.Add(new GiaTri
                    {
                        fSoTien = (double?)value.Sum(x => x.iTongTien ?? 0)
                    });
                }
                else
                {
                    lstTong.Add(new GiaTri
                    {
                        fSoTien = null
                    });
                }
            }
            #endregion

            double? fTongTien = null;

            if (lstChiTiet != null && lstChiTiet.Any())
                fTongTien = lstChiTiet.Sum(n => n.iTongTien ?? 0);

            string sSoQuyetDinh = string.Empty;
            string sNgayQuyetDinh = string.Empty;
            foreach (var item in data.lstDuToanChiTiet)
            {
                sSoQuyetDinh = item.sSoQuyetDinh;
                sNgayQuyetDinh = item.sNgayQuyetDinh;
                break;
            }

            #region Lấy data in theo tờ
            int columnStart = _columnCountBC3_ChiTiet * (to - 1);

            // set value header
            for (int i = 1; i <= _columnCountBC3_ChiTiet; i++)
            {
                if (columnStart + i <= objHeader.lstHeaderLv1.Count)
                {
                    fr.SetValue(string.Format("C{0}", i), objHeader.lstHeaderLv1[columnStart + i - 1].sTen);
                    if (iLoaiNganSach == 0)
                        fr.SetValue(string.Format("D{0}", i), objHeader.lstHeaderLv2[columnStart + i - 1].sTen);
                    fr.SetValue(string.Format("E{0}", i), objHeader.lstHeaderLv3[columnStart + i - 1].sTen);
                }
                else
                {
                    fr.SetValue(string.Format("C{0}", i), "");
                    if (iLoaiNganSach == 0)
                        fr.SetValue(string.Format("D{0}", i), "");
                    fr.SetValue(string.Format("E{0}", i), "");
                }
            }

            foreach (RptNNSDuToanChiTietModel item in lstChiTiet)
            {
                item.lstGiaTriPhanBo = item.lstGiaTriPhanBo.Skip(columnStart).Take(_columnCountBC3_ChiTiet).ToList();
            }

            foreach (PhongBanNhiemVu item in lstPhongBan)
            {
                item.lstGiaTriPhongBan = item.lstGiaTriPhongBan.Skip(columnStart).Take(_columnCountBC3_ChiTiet).ToList();
            }

            foreach (NhiemVu item in lstNhiemVu)
            {
                item.lstGiaTriNhiemVu = item.lstGiaTriNhiemVu.Skip(columnStart).Take(_columnCountBC3_ChiTiet).ToList();
            }

            lstTong = lstTong.Skip(columnStart).Take(_columnCountBC3_ChiTiet).ToList();
            #endregion

            fr.AddTable<GiaTri>("lstTong", lstTong);
            fr.AddTable<RptNNSDuToanChiTietModel>("ChiTiet", lstChiTiet);
            fr.AddTable<PhongBanNhiemVu>("PhongBan", lstPhongBan);
            fr.AddTable<NhiemVu>("NhiemVu", lstNhiemVu);

            fr.SetValue(new
            {
                sSoQuyetDinh = sSoQuyetDinh,
                sNgayQuyetDinh = sNgayQuyetDinh,
                fTongTien = fTongTien,
                dNow = DateTime.Now.ToString("dd/MM/yyyy"),
                dvt = dvt.ToStringDvt(),
                iNam = PhienLamViec.iNamLamViec,
                To = to
            });

            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName)
             .UseForm(this).Run(Result);

            // merge
            Result.MergeH(5, 9, 200);
            Result.MergeH(6, 9, 200);
            Result.MergeH(7, 9, 200);

            Result.MergeC(5, 7, 9, 200);

            return Result;
        }

        public ActionResult ChiTietPhanCapDauNamTheoQuyetDinhPartial(int iLoaiNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();

            var lstTreeNoiDungChi = _danhMucService.GetTreeAllDMNoiDungChiForReport(PhienLamViec.NamLamViec, iLoaiNganSach, null);
            if (lstTreeNoiDungChi == null || !lstTreeNoiDungChi.Any())
                return RedirectToAction("Index");

            foreach (DMNoiDungChiViewModel item in lstTreeNoiDungChi)
                item.fSoCon = CountChild(item, lstTreeNoiDungChi.ToList());

            data.treeNoiDungChi = lstTreeNoiDungChi;

            var lstDataDuToanTheoNam = _nnsService.GetDuToanPhanCapDauNam(PhienLamViec.NamLamViec, iLoaiNganSach, sSoQuyetDinh, dDateFrom, dDateTo, dvt);

            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            ViewBag.iLoaiNganSach = iLoaiNganSach;
            ViewBag.dvt = dvt;
            TempData["DataChiTietQuyetDinhPhanCapPartial"] = data;
            return View("PopupChiTietQuyetDinhPhanCapDauNamPartial", data);
        }

        public JsonResult Ds_To_BaoCaoChiTietQuyetDinhPhanCap()
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["DataChiTietQuyetDinhPhanCapPartial"] != null)
            {
                data = (RptTongHopGiaoDuToanModel)TempData["DataChiTietQuyetDinhPhanCapPartial"];
                TempData.Keep("DataChiTietQuyetDinhPhanCapPartial");
            }
            int totalColumn = 0;
            if (data.treeNoiDungChi != null && data.treeNoiDungChi.Any())
                totalColumn = data.treeNoiDungChi.Where(x => x.fSoCon == 0).Count();

            return ds_ToIn(totalColumn, _columnCountBC3_ChiTiet);
        }

        public ActionResult ChiTietQuyetDinhPhanCapPartialExport(string ext = "pdf", int dvt = 1, int iLoaiNganSach = 0, int to = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong_hop_du_toan_nsqp_giao_cho_dv_theo_soquyetdinh", ext);
            ExcelFile xls = TaoFileBaoCaoChiTietQuyetDinhPhanCapPartial(dvt, iLoaiNganSach, to);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCaoChiTietQuyetDinhPhanCapPartial(int dvt = 1, int iLoaiNganSach = 0, int to = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["DataChiTietQuyetDinhPhanCapPartial"] != null)
            {
                data = (RptTongHopGiaoDuToanModel)TempData["DataChiTietQuyetDinhPhanCapPartial"];
                TempData.Keep("DataChiTietQuyetDinhPhanCapPartial");
            }

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(iLoaiNganSach == 0 ? sFilePathBaoCao3_ChiTietPhanCap_0 : sFilePathBaoCao3_ChiTietPhanCap_1));
            FlexCelReport fr = new FlexCelReport();

            Header objHeader = new Header();
            objHeader.lstHeaderLv1 = new List<HeaderInfo>();
            objHeader.lstHeaderLv2 = new List<HeaderInfo>();
            objHeader.lstHeaderLv3 = new List<HeaderInfo>();

            foreach (DMNoiDungChiViewModel item in data.treeNoiDungChi)
            {
                if (iLoaiNganSach == 0)
                {
                    // header 3 tang
                    // level1
                    if (item.depth == "0")
                    {
                        if (item.fSoCon == 0)
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                        else if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv1.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 2
                    else if (item.depth == "1")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv2.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 3
                    else if (item.depth == "2")
                    {
                        objHeader.lstHeaderLv3.Add(new HeaderInfo
                        {
                            iID_Header = item.iID_NoiDungChi,
                            sTen = item.sTenNoiDungChi
                        });
                    }
                }
                else if (iLoaiNganSach == 1)
                {
                    // level 1
                    if (item.depth == "0")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv1.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 2
                    else if (item.depth == "1")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv3.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                }
            }

            List<RptNNSDuToanChiTietModel> lstChiTiet = data.lstDuToanChiTiet.ToList().Where(x => x.iID_Parent == null).ToList();
            lstChiTiet = lstChiTiet.GroupBy(x => new
            {
                x.iID_MaDonVi,
                x.sMaPhongBan,
                x.iID_NhiemVu
            }).Select(g => new RptNNSDuToanChiTietModel()
            {
                iID_MaDonVi = g.Key.iID_MaDonVi,
                sMaPhongBan = g.Key.sMaPhongBan,
                iID_NhiemVu = g.Key.iID_NhiemVu,
                sTenPhongBan = g.First().sTenPhongBan,
                TenDonVi = g.First().TenDonVi,
                GhiChu = g.First().GhiChu,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            List<PhongBan> lstPhongBan = lstChiTiet.GroupBy(x => new
            {
                x.sMaPhongBan,
            }).Select(g => new PhongBan
            {
                sMaPhongBan = g.Key.sMaPhongBan,
                sTenPhongBan = g.First().sTenPhongBan,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            #region set value for row chitiet
            foreach (RptNNSDuToanChiTietModel itemCt in lstChiTiet)
            {
                itemCt.lstGiaTriPhanBo = new List<GiaTri>();
                foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
                {
                    var value = data.lstDuToanChiTiet.Where(x => x.sMaPhongBan == itemCt.sMaPhongBan && x.iID_NoiDungChi == itemHd.iID_Header && x.iID_MaDonVi == itemCt.iID_MaDonVi).ToList();
                    if (value != null && value.Any())
                    {
                        itemCt.lstGiaTriPhanBo.Add(new GiaTri
                        {
                            fSoTien = (double?)value.Sum(x => x.iTongTien ?? 0)
                        });
                    }
                    else
                    {
                        itemCt.lstGiaTriPhanBo.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                    }
                }
            }
            #endregion

            #region set value for row phong ban
            foreach (PhongBan itemCt in lstPhongBan)
            {
                itemCt.lstGiaTriPhongBan = new List<GiaTri>();
                foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
                {
                    var value = data.lstDuToanChiTiet.Where(x => x.iID_NoiDungChi == itemHd.iID_Header && x.sMaPhongBan == itemCt.sMaPhongBan).ToList();
                    if (value != null && value.Any())
                    {
                        itemCt.lstGiaTriPhongBan.Add(new GiaTri
                        {
                            fSoTien = (double?)value.Sum(x => x.iTongTien ?? 0)
                        });
                    }
                    else
                    {
                        itemCt.lstGiaTriPhongBan.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                    }
                }
            }
            #endregion

            #region set value for row Tong
            List<GiaTri> lstTong = new List<GiaTri>();
            foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
            {
                var value = data.lstDuToanChiTiet.Where(x => x.iID_NoiDungChi == itemHd.iID_Header).ToList();
                if (value != null && value.Any())
                {
                    lstTong.Add(new GiaTri
                    {
                        fSoTien = (double?)value.Sum(x => x.iTongTien ?? 0)
                    });
                }
                else
                {
                    lstTong.Add(new GiaTri
                    {
                        fSoTien = null
                    });
                }
            }
            #endregion

            double? fTongTien = null;

            if (lstChiTiet != null && lstChiTiet.Any())
                fTongTien = lstChiTiet.Sum(n => n.iTongTien ?? 0);

            string sSoQuyetDinh = string.Empty;
            string sNgayQuyetDinh = string.Empty;
            foreach (var item in data.lstDuToanChiTiet)
            {
                sSoQuyetDinh = item.sSoQuyetDinh;
                sNgayQuyetDinh = item.sNgayQuyetDinh;
                break;
            }

            #region Lấy data in theo tờ
            int columnStart = _columnCountBC3_ChiTiet * (to - 1);

            // set value header
            for (int i = 1; i <= _columnCountBC3_ChiTiet; i++)
            {
                if (columnStart + i <= objHeader.lstHeaderLv1.Count)
                {
                    fr.SetValue(string.Format("C{0}", i), objHeader.lstHeaderLv1[columnStart + i - 1].sTen);
                    if (iLoaiNganSach == 0)
                        fr.SetValue(string.Format("D{0}", i), objHeader.lstHeaderLv2[columnStart + i - 1].sTen);
                    fr.SetValue(string.Format("E{0}", i), objHeader.lstHeaderLv3[columnStart + i - 1].sTen);
                }
                else
                {
                    fr.SetValue(string.Format("C{0}", i), "");
                    if (iLoaiNganSach == 0)
                        fr.SetValue(string.Format("D{0}", i), "");
                    fr.SetValue(string.Format("E{0}", i), "");
                }
            }

            foreach (RptNNSDuToanChiTietModel item in lstChiTiet)
            {
                item.lstGiaTriPhanBo = item.lstGiaTriPhanBo.Skip(columnStart).Take(_columnCountBC3_ChiTiet).ToList();
            }

            foreach (PhongBan item in lstPhongBan)
            {
                item.lstGiaTriPhongBan = item.lstGiaTriPhongBan.Skip(columnStart).Take(_columnCountBC3_ChiTiet).ToList();
            }

            lstTong = lstTong.Skip(columnStart).Take(_columnCountBC3_ChiTiet).ToList();
            #endregion

            fr.AddTable<GiaTri>("lstTong", lstTong);
            fr.AddTable<RptNNSDuToanChiTietModel>("ChiTiet", lstChiTiet);
            fr.AddTable<PhongBan>("PhongBan", lstPhongBan);
            fr.SetValue(new
            {
                sSoQuyetDinh = sSoQuyetDinh,
                sNgayQuyetDinh = sNgayQuyetDinh,
                fTongTien = fTongTien,
                dNow = DateTime.Now.ToString("dd/MM/yyyy"),
                dvt = dvt.ToStringDvt(),
                iNam = PhienLamViec.iNamLamViec,
                To = to
            });
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName)
             .UseForm(this).Run(Result);

            // merge
            Result.MergeH(5, 9, 200);
            Result.MergeH(6, 9, 200);
            Result.MergeH(7, 9, 200);

            Result.MergeC(5, 7, 9, 200);

            return Result;
        }

        public ActionResult TongHopGiaoDuToan()
        {
            DateTime firstDay = new DateTime(PhienLamViec.NamLamViec, 1, 1);
            DateTime lastDay = new DateTime(PhienLamViec.NamLamViec, 12, 31);

            ViewBag.dNgayFromDefault = firstDay.ToString("dd/MM/yyyy");
            ViewBag.dNgayToDefault = lastDay.ToString("dd/MM/yyyy");

            List<SelectListItem> lstLoaiNganSach = new List<SelectListItem> {
                new SelectListItem{Text = Constants.LoaiNganSach.TypeName.CHI_NGAN_SACH_NHA_NUOC, Value=((int)Constants.LoaiNganSach.Type.CHI_NGAN_SACH_NHA_NUOC).ToString()},
                new SelectListItem{Text = Constants.LoaiNganSach.TypeName.CHI_THUONG_XUYEN_QP, Value=((int)Constants.LoaiNganSach.Type.CHI_THUONG_XUYEN_QP).ToString()}
            };
            ViewBag.drpLoaiNganSach = lstLoaiNganSach.ToSelectList("Value", "Text");

            List<NNSDMNguonViewModel> lstNguon = _danhMucService.GetAllDMNguonBaoCao(PhienLamViec.NamLamViec, (int)Constants.LoaiNganSach.Type.CHI_NGAN_SACH_NHA_NUOC).ToList();
            lstNguon.Insert(0, new NNSDMNguonViewModel { iID_Nguon = Guid.Empty, sNoiDung = Constants.TAT_CA });
            ViewBag.ListDanhMucNguon = lstNguon.ToSelectList("iID_Nguon", "sNoiDung");

            return View();
        }

        #region Xuất Excel báo cáo Tổng hợp nguồn BTC cấp trong năm

        public JsonResult Ds_To_BaoCao1(Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
            {
                iNguonNganSach = null;
            }

            int iNamLamViec = PhienLamViec.NamLamViec;//2021;

            var dotNhanBoSungNam = _nnsService.GetAllDotNhanBoSungNam(iNguonNganSach, iNamLamViec, dDateFrom, dDateTo);
            var dotDuToanNam = _nnsService.GetAllDotDuToan(iNguonNganSach, iNamLamViec, dotNhanBoSungNam.Count(), dDateFrom, dDateTo);
            var dataBaoCaoTongHop = _nnsService.BaoCaoTongHopNguon(iNguonNganSach, iNamLamViec, dotNhanBoSungNam.Count(), dDateFrom, dDateTo, dvt);

            string sTenCot = string.Empty;
            foreach (var item in dotNhanBoSungNam)
            {
                sTenCot += string.Format("[{0}],", item.SoCot);
            }

            foreach (var item in dotDuToanNam)
            {
                sTenCot += string.Format("[{0}],", item.SoCot);
            }

            sTenCot = sTenCot.Substring(0, sTenCot.Length - 1);
            var lstChiTietDotBoSungDotDuToan = _nnsService.GetChiTietDotNhanVaDotBoSung(iNguonNganSach, iNamLamViec, dDateFrom, dDateTo, dvt, sTenCot);

            // update tong tien hang cha
            int dataCount = dataBaoCaoTongHop.Count();
            for (int i = dataCount - 1; i >= 0; i--)
            {
                decimal tongDaGiaoDauNam = 0, tongDaGiaoChuyenSang = 0, tongDaGiaoDuToan = 0, tongDaGiaoTongDuToan = 0, tongConLai = 0;
                List<decimal> listTongDotDuToan = Enumerable.Repeat(decimal.Zero, dotDuToanNam.Count()).ToList();
                if (!string.IsNullOrEmpty(dataBaoCaoTongHop[i].bLaHangCha))
                {
                    for (int j = i + 1; j < dataCount; j++)
                    {
                        if (int.Parse(dataBaoCaoTongHop[i].depth) + 1 == int.Parse(dataBaoCaoTongHop[j].depth))
                        {
                            if (lstChiTietDotBoSungDotDuToan != null)
                            {
                                var dataChiTietDotDuToanDotBoSung = lstChiTietDotBoSungDotDuToan.Select("iID_Nguon = '" + dataBaoCaoTongHop[j].iID_Nguon + "'")[0];
                                if (dataChiTietDotDuToanDotBoSung != null)
                                {
                                    for (int iDotDuToan = 0; iDotDuToan < dotDuToanNam.Count(); iDotDuToan++)
                                    {
                                        string soTien = dataChiTietDotDuToanDotBoSung["c" + dotDuToanNam.ToList()[iDotDuToan].SoCot].ToString();
                                        listTongDotDuToan[iDotDuToan] += string.IsNullOrEmpty(soTien) ? (decimal)0 : decimal.Parse(soTien);
                                    }
                                }
                            }

                            tongDaGiaoDauNam += dataBaoCaoTongHop[j].dagiaoDauNam;
                            tongDaGiaoChuyenSang += dataBaoCaoTongHop[j].dagiaoChuyenSang;
                            tongDaGiaoDuToan += dataBaoCaoTongHop[j].dagiaoDuToan;
                            tongDaGiaoTongDuToan += dataBaoCaoTongHop[j].dagiaoTongDuToan;
                            tongConLai += dataBaoCaoTongHop[j].conLai;
                        }
                        if (int.Parse(dataBaoCaoTongHop[i].depth) >= int.Parse(dataBaoCaoTongHop[j].depth) || j == dataCount - 1)
                        {
                            if (lstChiTietDotBoSungDotDuToan != null)
                            {
                                var dataChiTietCha = lstChiTietDotBoSungDotDuToan.Select("iID_Nguon = '" + dataBaoCaoTongHop[i].iID_Nguon + "'")[0];
                                if (dataChiTietCha != null)
                                {
                                    for (int iDotDuToan = 0; iDotDuToan < dotDuToanNam.Count(); iDotDuToan++)
                                    {
                                        string soTien = dataChiTietCha["c" + dotDuToanNam.ToList()[iDotDuToan].SoCot].ToString();
                                        dataChiTietCha["c" + dotDuToanNam.ToList()[iDotDuToan].SoCot] = (string.IsNullOrEmpty(soTien) ? (decimal)0 : decimal.Parse(soTien)) + listTongDotDuToan[iDotDuToan];
                                    }
                                }
                            }
                            dataBaoCaoTongHop[i].dagiaoDauNam = tongDaGiaoDauNam;
                            dataBaoCaoTongHop[i].dagiaoChuyenSang = tongDaGiaoChuyenSang;
                            dataBaoCaoTongHop[i].dagiaoDuToan = tongDaGiaoDuToan;
                            dataBaoCaoTongHop[i].dagiaoTongDuToan = tongDaGiaoTongDuToan;
                            dataBaoCaoTongHop[i].conLai = tongConLai;

                            break;
                        }
                    }
                }
            }

            TempData["BaoCaoTongHopNguonBTC"] = dataBaoCaoTongHop;
            TempData["DotNhanBoXung"] = dotNhanBoSungNam;
            TempData["DotDuToan"] = dotDuToanNam;
            TempData["ChiTietDotBoSungDuToan"] = lstChiTietDotBoSungDotDuToan;

            var count = 9 + (dotNhanBoSungNam == null ? 0 : dotNhanBoSungNam.Count()) + (dotDuToanNam == null ? 0 : dotDuToanNam.Count());
            return ds_ToIn(count, _columnCountBC1);
        }

        public ActionResult PrintBCTongHopNguonBTC(string ext = "pdf", int dvt = 1, int to = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong_Hop_Nguon_BTC_Cap_Trong_Nam", ext);
            ExcelFile xls = TaoFileBaoCao1(dvt, to);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCao1(int dvt = 1, int to = 1)
        {
            if (TempData["BaoCaoTongHopNguonBTC"] != null && TempData["DotNhanBoXung"] != null && TempData["DotDuToan"] != null)
            {
                List<BaoCaoTongHopNguonViewModel> data = (List<BaoCaoTongHopNguonViewModel>)TempData["BaoCaoTongHopNguonBTC"];
                IEnumerable<DotNhanBoSungTrongNamViewModel> dotDuToanNam = (IEnumerable<DotNhanBoSungTrongNamViewModel>)TempData["DotDuToan"];
                IEnumerable<DotNhanBoSungTrongNamViewModel> dotNhanBoSung = (IEnumerable<DotNhanBoSungTrongNamViewModel>)TempData["DotNhanBoXung"];
                DataTable lstChiTietDotDuToanDotBoSung = (DataTable)TempData["ChiTietDotBoSungDuToan"];

                TempData.Keep("BaoCaoTongHopNguonBTC");
                TempData.Keep("DotNhanBoXung");
                TempData.Keep("DotDuToan");
                TempData.Keep("ChiTietDotBoSungDuToan");
                List<string> lstHeaderBoSung = dotNhanBoSung.Select(x => x.sSoQuyetDinh).ToList();
                List<string> lstHeaderDaGiao = dotDuToanNam.Select(x => x.sSoQuyetDinh).ToList();

                int iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);

                Header objHeader = new Header();
                objHeader.lstHeaderLv1 = new List<HeaderInfo>();
                objHeader.lstHeaderLv2 = new List<HeaderInfo>();
                objHeader.lstHeaderLv3 = new List<HeaderInfo>();

                #region header Du toan dc giao
                // level 1
                for (int i = 0; i < 4 + lstHeaderBoSung.Count; i++)
                {
                    objHeader.lstHeaderLv1.Add(new HeaderInfo
                    {
                        sTen = "Dự toán năm được giao",
                    });
                }

                // level 2
                objHeader.lstHeaderLv2.Add(new HeaderInfo
                {
                    sTen = string.Format("Dự toán năm {0} được giao đầu năm", iNamLamViec),
                });


                objHeader.lstHeaderLv2.Add(new HeaderInfo
                {
                    sTen = string.Format("Năm {0} chuyển sang", iNamLamViec - 1),
                });

                for (int i = 0; i < lstHeaderBoSung.Count + 1; i++)
                {
                    objHeader.lstHeaderLv2.Add(new HeaderInfo
                    {
                        sTen = "Nhà nước bổ sung trong năm",
                    });
                }

                objHeader.lstHeaderLv2.Add(new HeaderInfo
                {
                    sTen = string.Format("Tổng dự toán năm {0}", iNamLamViec),
                });

                // level 3
                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = string.Format("Dự toán năm {0} được giao đầu năm", iNamLamViec),
                });

                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = string.Format("Năm {0} chuyển sang", iNamLamViec - 1),
                });

                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = "Cộng",
                });

                for (int i = 1; i <= lstHeaderBoSung.Count; i++)
                {
                    objHeader.lstHeaderLv3.Add(new HeaderInfo
                    {
                        sTen = lstHeaderBoSung[i - 1],
                    });
                }

                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = string.Format("Tổng dự toán năm {0}", iNamLamViec),
                });

                #endregion

                #region header Du toan da giao
                // level 1
                for (int i = 0; i < 4 + lstHeaderDaGiao.Count; i++)
                {
                    objHeader.lstHeaderLv1.Add(new HeaderInfo
                    {
                        sTen = "Dự toán năm đã giao cho đơn vị",
                    });
                }

                // level 2
                objHeader.lstHeaderLv2.Add(new HeaderInfo
                {
                    sTen = string.Format("Dự toán năm {0} được giao đầu năm", iNamLamViec),
                });


                objHeader.lstHeaderLv2.Add(new HeaderInfo
                {
                    sTen = string.Format("Năm {0} chuyển sang", iNamLamViec - 1),
                });

                for (int i = 0; i < lstHeaderDaGiao.Count + 1; i++)
                {
                    objHeader.lstHeaderLv2.Add(new HeaderInfo
                    {
                        sTen = "Số dự toán đã giao",
                    });
                }

                objHeader.lstHeaderLv2.Add(new HeaderInfo
                {
                    sTen = string.Format("Tổng dự toán năm {0}", iNamLamViec),
                });

                // level 3
                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = string.Format("Dự toán năm {0} được giao đầu năm", iNamLamViec),
                });

                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = string.Format("Năm {0} chuyển sang", iNamLamViec - 1),
                });

                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = "Cộng",
                });

                for (int i = 1; i <= lstHeaderDaGiao.Count; i++)
                {
                    objHeader.lstHeaderLv3.Add(new HeaderInfo
                    {
                        sTen = lstHeaderDaGiao[i - 1],
                    });
                }

                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = string.Format("Tổng dự toán năm {0}", iNamLamViec),
                });

                #endregion

                #region Con lai
                objHeader.lstHeaderLv1.Add(new HeaderInfo
                {
                    sTen = "Còn lại",
                });
                objHeader.lstHeaderLv2.Add(new HeaderInfo
                {
                    sTen = "Còn lại",
                });
                objHeader.lstHeaderLv3.Add(new HeaderInfo
                {
                    sTen = "Còn lại",
                });
                #endregion

                #region Update list gia tri
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].lstGiaTri = new List<SoTien>();
                    // cot fixed du toan dc giao
                    data[i].lstGiaTri.AddRange(new List<SoTien> {
                    new SoTien
                    {
                        fSoTien = data[i].dTDauNam
                    }, new SoTien
                    {
                        fSoTien = data[i].dtChuyenSang
                    }, new SoTien
                    {
                        fSoTien = data[i].NhaNuocBosung
                    }
                    });

                    foreach (DotNhanBoSungTrongNamViewModel a in dotNhanBoSung)
                    {
                        if (lstChiTietDotDuToanDotBoSung != null)
                        {
                            var chitiet = lstChiTietDotDuToanDotBoSung.Select("iID_Nguon = '" + data[i].iID_Nguon + "'")[0];
                            decimal? sSoTien = string.IsNullOrEmpty(chitiet["c" + a.SoCot].ToString()) ? (decimal?)null : decimal.Parse(chitiet["c" + a.SoCot].ToString());
                            data[i].lstGiaTri.Add(new SoTien
                            {
                                fSoTien = sSoTien
                            });
                        }
                        else
                            data[i].lstGiaTri.Add(new SoTien
                            {
                                fSoTien = null
                            });
                    }

                    data[i].lstGiaTri.Add(new SoTien
                    {
                        fSoTien = data[i].TongDuToan
                    });


                    // cot fixed du toan da giao
                    data[i].lstGiaTri.AddRange(new List<SoTien> {
                    new SoTien
                    {
                        fSoTien = data[i].dagiaoDauNam
                    }, new SoTien
                    {
                        fSoTien = data[i].dagiaoChuyenSang
                    }, new SoTien
                    {
                        fSoTien = data[i].dagiaoDuToan
                    }
                    });

                    foreach (DotNhanBoSungTrongNamViewModel a in dotDuToanNam)
                    {
                        if (lstChiTietDotDuToanDotBoSung != null)
                        {
                            var chitiet = lstChiTietDotDuToanDotBoSung.Select("iID_Nguon = '" + data[i].iID_Nguon + "'")[0];
                            decimal? sSoTien = string.IsNullOrEmpty(chitiet["c" + a.SoCot].ToString()) ? (decimal?)null : decimal.Parse(chitiet["c" + a.SoCot].ToString());
                            data[i].lstGiaTri.Add(new SoTien
                            {
                                fSoTien = sSoTien
                            });
                        }
                        else
                        {
                            data[i].lstGiaTri.Add(new SoTien
                            {
                                fSoTien = null
                            }); ;
                        }
                    }

                    data[i].lstGiaTri.Add(new SoTien
                    {
                        fSoTien = data[i].dagiaoTongDuToan
                    });

                    data[i].lstGiaTri.Add(new SoTien
                    {
                        fSoTien = data[i].conLai
                    });
                }
                #endregion

                XlsFile Result = new XlsFile(true);
                Result.Open(Server.MapPath(sFilePathBaoCao1));
                FlexCelReport fr = new FlexCelReport();

                int columnStart = _columnCountBC1 * (to - 1);

                // set value header
                for (int i = 1; i <= _columnCountBC1; i++)
                {
                    if (columnStart + i <= objHeader.lstHeaderLv1.Count)
                    {
                        fr.SetValue(string.Format("C{0}", i), objHeader.lstHeaderLv1[columnStart + i - 1].sTen);
                        fr.SetValue(string.Format("D{0}", i), objHeader.lstHeaderLv2[columnStart + i - 1].sTen);
                        fr.SetValue(string.Format("E{0}", i), objHeader.lstHeaderLv3[columnStart + i - 1].sTen);
                    }
                    else
                    {
                        fr.SetValue(string.Format("C{0}", i), "");
                        fr.SetValue(string.Format("D{0}", i), "");
                        fr.SetValue(string.Format("E{0}", i), "");
                    }
                }

                foreach(BaoCaoTongHopNguonViewModel item in data)
                {
                    item.lstGiaTri = item.lstGiaTri.Skip(columnStart).Take(_columnCountBC1).ToList();
                }

                fr.AddTable<BaoCaoTongHopNguonViewModel>("dt", data);
                fr.SetValue(new
                {
                    dvt = dvt.ToStringDvt(),
                    To = to,
                    iNam = PhienLamViec.iNamLamViec
                });
                fr.UseChuKy(Username)
                 .UseChuKyForController(sControlName)
                 .UseForm(this).Run(Result);

                // merge
                Result.MergeH(5, 5, 13);
                Result.MergeH(6, 5, 13);

                Result.MergeC(5, 7, 5, 13);

                return Result;
            }
            return null;
        }
        #endregion

        private float ConvertStringToFloat(string sConvert)
        {
            if (string.IsNullOrEmpty(sConvert))
                return 0;
            return float.Parse(sConvert.Replace(".", ""));
        }

        [HttpPost]
        public bool ExportBCTongHopGiaoDuToan(List<List<string>> dataReport)
        {
            TempData["dataReport"] = dataReport;
            return true;
        }

        public JsonResult Ds_To_BaoCao3(int iLoaiNganSach, Guid? iID_Nguon, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            int iTotalCol = 0;
            if (iID_Nguon == Guid.Empty)
                iID_Nguon = null;
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            data.lstTongNNS = _nnsService.GetTotalNganSachBQPByNamLamViec(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon, dvt);
            data.lstNoiDungChi = _danhMucService.GetAllDMNoiDungChiBQP(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon);

            var lstTreeNoiDungChi = _danhMucService.GetTreeAllDMNoiDungChiForReport(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon);
            if (lstTreeNoiDungChi == null || !lstTreeNoiDungChi.Any())
                return ds_ToIn(0, _columnCountBC3);

            foreach (DMNoiDungChiViewModel item in lstTreeNoiDungChi)
                item.fSoCon = CountChild(item, lstTreeNoiDungChi.ToList());

            data.treeNoiDungChi = lstTreeNoiDungChi;
            iTotalCol = data.treeNoiDungChi.Count(n => !n.bLaHangCha);

            data.lstTongTienDuToan = _nnsService.GetTotalDuToanByNamLamViec(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon, dDateFrom, dDateTo, dvt);

            var lstDataDuToanTheoNam = _nnsService.GetDetailDuToanByNamLamViec(PhienLamViec.NamLamViec, iLoaiNganSach, iID_Nguon, dDateFrom, dDateTo, sSoQuyetDinh, dvt);
            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();
            data.lstDuToanChoPheDuyet = lstDataDuToanTheoNam.Where(x => string.IsNullOrEmpty(x.sSoQuyetDinh) && x.iID_NhiemVu != Guid.Empty && !string.IsNullOrEmpty(x.sMaNoiDungChi)).GroupBy(g => new { g.iID_NhiemVu, g.sMaNoiDungChi }).Select(x => new RptNNSDuToanChiTietModel()
            {
                dNgayQuyetDinh = x.First().dNgayQuyetDinh,
                sMaNoiDungChi = x.Key.sMaNoiDungChi,
                iID_NhiemVu = x.Key.iID_NhiemVu,
                iTongTien = x.Sum(g => g.iTongTien),
                GhiChu = x.First().GhiChu,
                iID_NoiDungChi = x.First().iID_NoiDungChi,
                iID_Parent = x.First().iID_Parent
            }).ToList();

            TempData["BaoCaoTongHopGiaoDuToan"] = data;

            return ds_ToIn(iTotalCol, _columnCountBC3);
        }

        public ActionResult ExportExcel(string ext = "pdf", int dvt = 1, int iLoaiNganSach = 0, int to = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong_Hop_Giao_Du_Toan_Trong_Nam", ext);
            ExcelFile xls = TaoFileBaoCao3(dvt, iLoaiNganSach, to);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCao3(int dvt = 1, int iLoaiNganSach = 0, int to = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["BaoCaoTongHopGiaoDuToan"] != null)
            {
                data = (RptTongHopGiaoDuToanModel)TempData["BaoCaoTongHopGiaoDuToan"];
            }

            TempData.Keep("BaoCaoTongHopGiaoDuToan");   

            XlsFile Result = new XlsFile(true);
            /*
            if(to == 1)
                Result.Open(Server.MapPath(iLoaiNganSach == 0 ? sFilePathBaoCao3_0 : sFilePathBaoCao3_1));
            else
            {
            */
                Result.Open(Server.MapPath(iLoaiNganSach == 0 ? sFilePathBaoCao3_0_Dynamic_TongCongNguon : sFilePathBaoCao3_1));
            // }
            FlexCelReport fr = new FlexCelReport();

            Header objHeader = new Header();
            objHeader.lstHeaderLv1 = new List<HeaderInfo>();
            objHeader.lstHeaderLv2 = new List<HeaderInfo>();
            objHeader.lstHeaderLv3 = new List<HeaderInfo>();

            foreach (DMNoiDungChiViewModel item in data.treeNoiDungChi)
            {
                if (iLoaiNganSach == 0)
                {
                    // header 3 tang
                    // level1
                    if (item.depth == "0")
                    {
                        if (item.fSoCon == 0)
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                        else if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv1.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 2
                    else if (item.depth == "1")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv2.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv2.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 3
                    else if (item.depth == "2")
                    {
                        objHeader.lstHeaderLv3.Add(new HeaderInfo
                        {
                            iID_Header = item.iID_NoiDungChi,
                            sTen = item.sTenNoiDungChi
                        });
                    }
                }
                else if (iLoaiNganSach == 1)
                {
                    // level 1
                    if (item.depth == "0")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv1.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv1.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                    // level 2
                    else if (item.depth == "1")
                    {
                        if (item.bLaHangCha)
                        {
                            for (int i = 0; i < item.fSoCon; i++)
                            {
                                objHeader.lstHeaderLv3.Add(new HeaderInfo
                                {
                                    iID_Header = item.iID_NoiDungChi,
                                    sTen = item.sTenNoiDungChi
                                });
                            }
                        }
                        else
                        {
                            objHeader.lstHeaderLv3.Add(new HeaderInfo
                            {
                                iID_Header = item.iID_NoiDungChi,
                                sTen = item.sTenNoiDungChi
                            });
                        }
                    }
                }
            }

            List<RptNNSDuToanChiTietModel> lstChiTiet = data.lstDuToanChiTiet.ToList().Where(x => x.iID_Parent == null).ToList();
            lstChiTiet = data.lstDuToanChiTiet.ToList().GroupBy(x => new { x.sMaPhongBan, x.iID_NhiemVu }).Select(x => new RptNNSDuToanChiTietModel()
            {
                sMaPhongBan = x.First().sMaPhongBan,
                iID_NhiemVu = x.First().iID_NhiemVu,
                sTenPhongBan = x.First().sTenPhongBan,
                GhiChu = x.First().GhiChu,
                sSoQuyetDinh = x.First().sSoQuyetDinh,
                dNgayQuyetDinh = x.First().dNgayQuyetDinh

            }).ToList();
            List<RptNNSDuToanChiTietModel> lstChoPheDuyet = data.lstDuToanChoPheDuyet.ToList().Where(x => x.iID_Parent == null).ToList();

            List<string> listPhongBan = data.lstDuToanChiTiet.Select(x => x.sTenPhongBan).Distinct().ToList();
            List<PhongBan> lstPhongBan = new List<PhongBan>();
            listPhongBan.ForEach(x =>
            {
                lstPhongBan.Add(new PhongBan
                {
                    sTenPhongBan = x
                });
            });

            #region set value for column
            foreach (RptNNSDuToanChiTietModel itemCt in lstChiTiet)
            {
                itemCt.lstGiaTriPhanBo = new List<GiaTri>();
                foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
                {
                    var value = data.lstDuToanChiTiet.Where(x => x.sMaPhongBan == itemCt.sMaPhongBan && x.iID_NoiDungChi == itemHd.iID_Header && x.iID_NhiemVu == itemCt.iID_NhiemVu).FirstOrDefault();
                    if (value != null)
                    {
                        itemCt.lstGiaTriPhanBo.Add(new GiaTri
                        {
                            fSoTien = (double?)value.iTongTien
                        });
                    }
                    else
                    {
                        itemCt.lstGiaTriPhanBo.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                    }
                }
                itemCt.iTongTien = itemCt.lstGiaTriPhanBo.Sum(x => x.fSoTien ?? 0);
            }

            foreach (RptNNSDuToanChiTietModel itemChoPheDuyet in lstChoPheDuyet)
            {
                itemChoPheDuyet.lstGiaTriChoPheDuyet = new List<GiaTri>();
                foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
                {
                    var value = data.lstDuToanChoPheDuyet.Where(x => x.iID_NoiDungChi == itemHd.iID_Header && x.iID_NhiemVu == itemChoPheDuyet.iID_NhiemVu).FirstOrDefault();
                    if (value != null)
                    {
                        itemChoPheDuyet.lstGiaTriChoPheDuyet.Add(new GiaTri
                        {
                            fSoTien = (double?)value.iTongTien
                        });
                    }
                    else
                    {
                        itemChoPheDuyet.lstGiaTriChoPheDuyet.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                    }
                }
            }

            #endregion

            decimal? fTongTien = null;
            string sSoQuyetDinhTong = string.Empty;
            string sNgayQuyetDinhTong = string.Empty;
            decimal? fTongTienDuToan = null;

            if (data.lstTongNNS != null && data.lstTongNNS.Any(n => n.fTongTien.HasValue))
                fTongTien = data.lstTongNNS.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien);

            if (data.lstTongTienDuToan != null && data.lstTongTienDuToan.Any(n => n.fTongTien.HasValue))
            {
                sSoQuyetDinhTong = data.lstTongTienDuToan.FirstOrDefault().sSoQuyetDinh;
                if (data.lstTongTienDuToan.FirstOrDefault().dNgayQuyetDinh.HasValue)
                {
                    sNgayQuyetDinhTong = data.lstTongTienDuToan.FirstOrDefault().dNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                }
                fTongTienDuToan = data.lstTongTienDuToan.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien).Value;
            }

            #region I TỔNG NGUỒN NSQP
            List<GiaTri> lstGiaTriTongNguon = new List<GiaTri>();
            foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
            {
                var objTong = data.lstTongNNS.Where(x => x.iID_NoiDungChi == itemHd.iID_Header).FirstOrDefault();
                if (objTong != null)
                {
                    lstGiaTriTongNguon.Add(new GiaTri
                    {
                        fSoTien = (double?)objTong.fTongTien
                    });
                }
                else
                {
                    lstGiaTriTongNguon.Add(new GiaTri
                    {
                        fSoTien = null
                    });
                }
            }
            #endregion

            #region a Phân cấp đầu năm 
            List<GiaTri> lstGiaTriPhanCapDauNam = new List<GiaTri>();
            foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
            {
                var objTong = data.lstTongTienDuToan.Where(x => x.iID_NoiDungChi == itemHd.iID_Header).FirstOrDefault();
                if (objTong != null)
                {
                    lstGiaTriPhanCapDauNam.Add(new GiaTri
                    {
                        fSoTien = (double?)objTong.fTongTien
                    });
                }
                else
                {
                    lstGiaTriPhanCapDauNam.Add(new GiaTri
                    {
                        fSoTien = null
                    });
                }
            }
            #endregion

            #region b Phân bổ, bổ sung trong năm 
            List<GiaTri> lstGiaTriPhanBoBSTrongNam = new List<GiaTri>();
            foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
            {
                var objTong = data.lstDuToanChiTiet.Where(x => x.iID_NoiDungChi == itemHd.iID_Header);
                if (objTong != null && objTong.Any())
                {
                    lstGiaTriPhanBoBSTrongNam.Add(new GiaTri
                    {
                        fSoTien = (double?)objTong.Sum(x => x.iTongTien)
                    });
                }
                else
                {
                    lstGiaTriPhanBoBSTrongNam.Add(new GiaTri
                    {
                        fSoTien = null
                    });
                }
            }
            #endregion

            #region 2 Các nội dung đang trình bộ phê duyệt 
            List<GiaTri> lstGiaTriTrinhBoPheDuyet = new List<GiaTri>();
            foreach (HeaderInfo itemHd in objHeader.lstHeaderLv3)
            {
                var objTong = data.lstDuToanChoPheDuyet.Where(x => x.iID_NoiDungChi == itemHd.iID_Header);
                if (objTong != null && objTong.Any())
                {
                    lstGiaTriTrinhBoPheDuyet.Add(new GiaTri
                    {
                        fSoTien = (double?)objTong.Sum(x => x.iTongTien)
                    });
                }
                else
                {
                    lstGiaTriTrinhBoPheDuyet.Add(new GiaTri
                    {
                        fSoTien = null
                    });
                }
            }
            #endregion

            #region CHỜ PHÂN CẤP (I-a)
            List<GiaTri> lstGiaTriChoPhanCap = new List<GiaTri>();
            for (int i = 0; i < objHeader.lstHeaderLv3.Count; i++)
            {
                lstGiaTriChoPhanCap.Add(new GiaTri
                {
                    fSoTien = (lstGiaTriTongNguon[i].fSoTien ?? 0) - (lstGiaTriPhanCapDauNam[i].fSoTien ?? 0)
                });
            }
            #endregion

            #region 1 Đã giao cho các ngành, đơn vị (a+b)
            List<GiaTri> lstGiaTriDaGiao = new List<GiaTri>();
            for (int i = 0; i < objHeader.lstHeaderLv3.Count; i++)
            {
                lstGiaTriDaGiao.Add(new GiaTri
                {
                    fSoTien = (lstGiaTriPhanCapDauNam[i].fSoTien ?? 0) + (lstGiaTriPhanBoBSTrongNam[i].fSoTien ?? 0)
                });
            }
            #endregion

            #region II GIAO CHO ĐƠN VỊ (1+2)
            List<GiaTri> lstGiaTriGiaoChoDonVi = new List<GiaTri>();
            for (int i = 0; i < objHeader.lstHeaderLv3.Count; i++)
            {
                lstGiaTriGiaoChoDonVi.Add(new GiaTri
                {
                    fSoTien = (lstGiaTriDaGiao[i].fSoTien ?? 0) + (lstGiaTriTrinhBoPheDuyet[i].fSoTien ?? 0)
                });
            }
            #endregion

            #region III CÒN LẠI TẠI BỘ (I-II)
            List<GiaTri> lstGiaTriConLai = new List<GiaTri>();
            for (int i = 0; i < objHeader.lstHeaderLv3.Count; i++)
            {
                lstGiaTriConLai.Add(new GiaTri
                {
                    fSoTien = (lstGiaTriTongNguon[i].fSoTien ?? 0) - (lstGiaTriGiaoChoDonVi[i].fSoTien ?? 0)
                });
            }
            #endregion

            #region Lấy data in theo tờ
            int columnStart = _columnCountBC3 * (to - 1);

            // set value header
            for (int i = 1; i <= _columnCountBC3; i++)
            {
                if (columnStart + i <= objHeader.lstHeaderLv1.Count)
                {
                    fr.SetValue(string.Format("C{0}", i), objHeader.lstHeaderLv1[columnStart + i - 1].sTen);
                    if (iLoaiNganSach == 0)
                        fr.SetValue(string.Format("D{0}", i), objHeader.lstHeaderLv2[columnStart + i - 1].sTen);
                    fr.SetValue(string.Format("E{0}", i), objHeader.lstHeaderLv3[columnStart + i - 1].sTen);
                }
                else
                {
                    fr.SetValue(string.Format("C{0}", i), "");
                    if (iLoaiNganSach == 0)
                        fr.SetValue(string.Format("D{0}", i), "");
                    fr.SetValue(string.Format("E{0}", i), "");
                }
            }

            foreach (RptNNSDuToanChiTietModel item in lstChiTiet)
            {
                item.lstGiaTriPhanBo = item.lstGiaTriPhanBo.Skip(columnStart).Take(_columnCountBC3).ToList();
            }

            foreach (RptNNSDuToanChiTietModel item in lstChoPheDuyet)
            {
                item.lstGiaTriChoPheDuyet = item.lstGiaTriChoPheDuyet.Skip(columnStart).Take(_columnCountBC3).ToList();
            }

            lstGiaTriTongNguon = lstGiaTriTongNguon.Skip(columnStart).Take(_columnCountBC3).ToList();
            lstGiaTriPhanCapDauNam = lstGiaTriPhanCapDauNam.Skip(columnStart).Take(_columnCountBC3).ToList();
            lstGiaTriPhanBoBSTrongNam = lstGiaTriPhanBoBSTrongNam.Skip(columnStart).Take(_columnCountBC3).ToList();
            lstGiaTriChoPhanCap = lstGiaTriChoPhanCap.Skip(columnStart).Take(_columnCountBC3).ToList();
            lstGiaTriDaGiao = lstGiaTriDaGiao.Skip(columnStart).Take(_columnCountBC3).ToList();
            lstGiaTriTrinhBoPheDuyet = lstGiaTriTrinhBoPheDuyet.Skip(columnStart).Take(_columnCountBC3).ToList();
            lstGiaTriGiaoChoDonVi = lstGiaTriGiaoChoDonVi.Skip(columnStart).Take(_columnCountBC3).ToList();
            lstGiaTriConLai = lstGiaTriConLai.Skip(columnStart).Take(_columnCountBC3).ToList();
            #endregion

            fr.AddTable<RptNNSDuToanChiTietModel>("ChiTiet", lstChiTiet);
            fr.AddTable<RptNNSDuToanChiTietModel>("ChoPheDuyet", lstChoPheDuyet);
            fr.AddTable<GiaTri>("lstGiaTriTongNguon", lstGiaTriTongNguon);
            fr.AddTable<GiaTri>("lstGiaTriPhanCapDauNam", lstGiaTriPhanCapDauNam);
            fr.AddTable<GiaTri>("lstGiaTriPhanBoBSTrongNam", lstGiaTriPhanBoBSTrongNam);
            fr.AddTable<GiaTri>("lstGiaTriChoPhanCap", lstGiaTriChoPhanCap);
            fr.AddTable<GiaTri>("lstGiaTriDaGiao", lstGiaTriDaGiao);
            fr.AddTable<GiaTri>("lstGiaTriTrinhBoPheDuyet", lstGiaTriTrinhBoPheDuyet);
            fr.AddTable<GiaTri>("lstGiaTriGiaoChoDonVi", lstGiaTriGiaoChoDonVi);
            fr.AddTable<GiaTri>("lstGiaTriConLai", lstGiaTriConLai);
            fr.AddTable<PhongBan>("PhongBan", lstPhongBan);

            fr.SetValue(new
            {
                sSoQuyetDinhTong = sSoQuyetDinhTong,
                sNgayQuyetDinhTong = sNgayQuyetDinhTong,
                fTongTien = fTongTien,
                fTongTienGiaoDuToan = fTongTienDuToan,

                dvt = dvt.ToStringDvt(),
                To = to,
                iNam = PhienLamViec.iNamLamViec
            });

            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this).Run(Result);

            /* 1 template dynamic column */
            if (to != 1)
            {
                Result.SetColHidden(12, true);
                Result.SetColHidden(13, true);
                Result.SetColHidden(14, true);
            }

            if (to == 1)
            {
                // merge
                Result.MergeH(5, 11, 11);
                Result.MergeH(6, 11, 11);
                Result.MergeH(7, 11, 11);
            }
            else
            {
                // merge
                Result.MergeH(5, 11, 11);
                Result.MergeH(6, 11, 11);
                Result.MergeH(7, 11, 11);
            }
            

            /* Use 2 templates with merge
            if(to == 1)
                Result.MergeC(5, 7, 14, 8);
            else
            {
                Result.MergeC(5, 7, 11, 8);
            }
            */

            /* Use 2 template without merge */
            // Result.MergeC(5, 7, 14, 8);

            /* Use 1 template dynamic column */
            if (to == 1)
            {
                Result.MergeC(5, 7, 14, 8);
            }
            else
            {
                Result.MergeC(5, 7, 11, 11);
            }
            
            return Result;
        }

        #region Bao cao 4
        public ActionResult TongHopGiaoDuToanBaoCao4()
        {
            List<NNSDMNguonViewModel> lstNguon = _danhMucService.GetAllDMNguonBaoCao(PhienLamViec.NamLamViec).ToList();
            lstNguon.Insert(0, new NNSDMNguonViewModel { iID_Nguon = Guid.Empty, sNoiDung = Constants.TAT_CA });
            ViewBag.ListDanhMucNguon = lstNguon.ToSelectList("iID_Nguon", "sNoiDung");

            DateTime firstDay = new DateTime(PhienLamViec.NamLamViec, 1, 1);
            DateTime lastDay = new DateTime(PhienLamViec.NamLamViec, 12, 31);

            ViewBag.dNgayFromDefault = firstDay.ToString("dd/MM/yyyy");
            ViewBag.dNgayToDefault = lastDay.ToString("dd/MM/yyyy");

            return View();
        }

        [HttpPost]
        public ActionResult TongHopGiaoDuToanBaoCao4Partial(Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            data.lstTongNNS = _nnsService.GetTotalNganSachBQPByNamLamViec(PhienLamViec.NamLamViec, null, iNguonNganSach, dvt);
            if (data.lstTongNNS != null && data.lstTongNNS.Any(n => n.fTongTien.HasValue))
            {
                ViewBag.iTotalMoney = data.lstTongNNS.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }

            data.lstTongTienDuToan = _nnsService.GetTotalDuToanByNamLamViec(PhienLamViec.NamLamViec, null, iNguonNganSach, dDateFrom, dDateTo, dvt);
            if (data.lstTongTienDuToan != null && data.lstTongTienDuToan.Any(n => n.fTongTien.HasValue))
            {
                ViewBag.sSoQuyetDinhTong = data.lstTongTienDuToan.FirstOrDefault().sSoQuyetDinh;
                if (data.lstTongTienDuToan.FirstOrDefault().dNgayQuyetDinh.HasValue)
                {
                    ViewBag.dNgayQuyetDinhTong = data.lstTongTienDuToan.FirstOrDefault().dNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                }
                ViewBag.sTongTienDuToan = data.lstTongTienDuToan.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }

            var lstDataDuToanTheoNam = _nnsService.GetDetailDuToanTheoNoiDungChi(iNguonNganSach, PhienLamViec.NamLamViec, dDateFrom, dDateTo, sSoQuyetDinh, dvt);

            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();
            data.lstDuToanChoPheDuyet = lstDataDuToanTheoNam.Where(x => string.IsNullOrEmpty(x.sSoQuyetDinh) && x.iID_NhiemVu != Guid.Empty).GroupBy(g => new { g.iID_NhiemVu, g.sMaNoiDungChi }).Select(x => new RptNNSDuToanChiTietModel()
            {
                dNgayQuyetDinh = x.First().dNgayQuyetDinh,
                sMaNoiDungChi = x.Key.sMaNoiDungChi,
                iID_NhiemVu = x.Key.iID_NhiemVu,
                iTongTien = x.Sum(g => g.iTongTien),
                GhiChu = x.First().GhiChu
            }).ToList();

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            ViewBag.dvt = dvt;
            TempData["BaoCaoTongHopGiaoDuToan"] = data;
            return PartialView("TongHopGiaoDuToanBaoCao4Partial", data);
        }

        [HttpPost]
        public bool ExportBCTongHopGiaoDuToanBaoCao4(List<List<string>> dataReport)
        {
            TempData["dataReport"] = dataReport;
            return true;
        }

        public ActionResult ExportBaoCao4(string ext = "pdf", int dvt = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong_Hop_Giao_Du_Toan_Theo_Noi_Dung_Nguon_BTC", ext);
            ExcelFile xls = TaoFileBaoCao4(dvt);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCao4(int dvt = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["BaoCaoTongHopGiaoDuToan"] != null)
                data = (RptTongHopGiaoDuToanModel)TempData["BaoCaoTongHopGiaoDuToan"];

            TempData.Keep("BaoCaoTongHopGiaoDuToan");

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao4));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<RptNNSDuToanChiTietModel>("ChiTiet", data.lstDuToanChiTiet);
            fr.AddTable<RptNNSDuToanChiTietModel>("ChoPheDuyet", data.lstDuToanChoPheDuyet);
            List<string> listPhongBan = data.lstDuToanChiTiet.Select(x => x.sTenPhongBan).Distinct().ToList();
            List<PhongBan> lstPhongBan = new List<PhongBan>();
            listPhongBan.ForEach(x =>
            {
                lstPhongBan.Add(new PhongBan
                {
                    sTenPhongBan = x
                });
            });

            decimal? fTongTien = null;
            string sSoQuyetDinhTong = string.Empty;
            string sNgayQuyetDinhTong = string.Empty;
            decimal? fTongTienDuToan = null;

            if (data.lstTongNNS != null && data.lstTongNNS.Any(n => n.fTongTien.HasValue))
                fTongTien = data.lstTongNNS.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien);

            if (data.lstTongTienDuToan != null && data.lstTongTienDuToan.Any(n => n.fTongTien.HasValue))
            {
                sSoQuyetDinhTong = data.lstTongTienDuToan.FirstOrDefault().sSoQuyetDinh;
                if (data.lstTongTienDuToan.FirstOrDefault().dNgayQuyetDinh.HasValue)
                {
                    sNgayQuyetDinhTong = data.lstTongTienDuToan.FirstOrDefault().dNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                }
                fTongTienDuToan = data.lstTongTienDuToan.Where(x => x.iID_Parent == null).Sum(n => n.fTongTien).Value;
            }

            fr.SetValue("sSoQuyetDinhTong", sSoQuyetDinhTong);
            fr.SetValue("sNgayQuyetDinhTong", sNgayQuyetDinhTong);
            fr.SetValue("fTongTien", fTongTien);
            fr.SetValue("fTongTienGiaoDuToan", fTongTienDuToan);
            fr.AddTable("PhongBan", lstPhongBan);
            fr.SetValue("dNow", DateTime.Now.ToString("dd/MM/yyyy"));
            fr.SetValue("dvt", dvt.ToStringDvt());
            fr.SetValue("iNam", PhienLamViec.iNamLamViec);
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName);
            fr.Run(Result);
            return Result;
        }

        public ActionResult ChiTietQuyetDinhBaoCao4Partial(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();

            //data.treeNoiDungChi = _danhMucService.GetTreeAllDMNoiDungChiForReport(PhienLamViec.NamLamViec);

            var lstDataDuToanTheoNam = _nnsService.GetDuToanTheoNhiemVuVaDanhMucNguon(iNguonNganSach, PhienLamViec.NamLamViec, sSoQuyetDinh, dDateFrom, dDateTo, dvt);

            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();
            data.lstDuToanChoPheDuyet = lstDataDuToanTheoNam.Where(x => string.IsNullOrEmpty(x.sSoQuyetDinh) && x.iID_NhiemVu != Guid.Empty && !string.IsNullOrEmpty(x.sMaNoiDungChi)).GroupBy(g => new { g.iID_NhiemVu, g.sMaNoiDungChi }).Select(x => new RptNNSDuToanChiTietModel()
            {
                dNgayQuyetDinh = x.First().dNgayQuyetDinh,
                sMaNoiDungChi = x.Key.sMaNoiDungChi,
                iID_NhiemVu = x.Key.iID_NhiemVu,
                iTongTien = x.Sum(g => g.iTongTien),
                GhiChu = x.First().GhiChu
            }).ToList();

            TempData["DataChiTietQuyetDinhBaoCao4Partial"] = data;
            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            ViewBag.dvt = dvt;
            return View("PopupChiTietQuyetDinhBaoCao4Partial", data);
        }

        public ActionResult ChiTietQuyetDinhBaoCao4PartialExport(string ext = "pdf", int dvt = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong_hop_du_toan_nsqp_giao_cho_dv_theo_soquyetdinh", ext);
            ExcelFile xls = TaoFileBaoCaoChiTietQuyetDinhBaoCao4Partial(dvt);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCaoChiTietQuyetDinhBaoCao4Partial(int dvt = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["DataChiTietQuyetDinhBaoCao4Partial"] != null)
            {
                data = (RptTongHopGiaoDuToanModel)TempData["DataChiTietQuyetDinhBaoCao4Partial"];
            }

            TempData.Keep("DataChiTietQuyetDinhBaoCao4Partial");

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao4_ChiTiet));
            FlexCelReport fr = new FlexCelReport();

            List<RptNNSDuToanChiTietModel> lstChiTiet = data.lstDuToanChiTiet.ToList().Where(x => x.iID_Parent == null).ToList();

            List<PhongBanNhiemVu> lstPhongBan = lstChiTiet.GroupBy(x => new
            {
                x.sMaPhongBan,
                x.iID_NhiemVu
            }).Select(g => new PhongBanNhiemVu
            {
                sMaPhongBan = g.Key.sMaPhongBan,
                iID_NhiemVu = g.Key.iID_NhiemVu,
                sTenPhongBan = g.First().sTenPhongBan,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            List<NhiemVu> lstNhiemVu = lstChiTiet.GroupBy(x => x.iID_NhiemVu).Select(g => new NhiemVu
            {
                iID_NhiemVu = g.Key,
                sGhiChu = g.First().GhiChu,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            double? fTongTien = null;

            if (lstChiTiet != null && lstChiTiet.Any())
                fTongTien = lstChiTiet.Sum(n => n.iTongTien ?? 0);

            string sSoQuyetDinh = string.Empty;
            string sNgayQuyetDinh = string.Empty;
            foreach (var item in data.lstDuToanChiTiet)
            {
                sSoQuyetDinh = item.sSoQuyetDinh;
                sNgayQuyetDinh = item.sNgayQuyetDinh;
                break;
            }
            fr.SetValue("sSoQuyetDinh", sSoQuyetDinh);
            fr.SetValue("sNgayQuyetDinh", sNgayQuyetDinh);
            fr.AddTable<RptNNSDuToanChiTietModel>("ChiTiet", lstChiTiet);
            fr.SetValue("fTongTien", fTongTien);
            fr.AddTable<PhongBanNhiemVu>("PhongBan", lstPhongBan);
            fr.AddTable<NhiemVu>("NhiemVu", lstNhiemVu);
            fr.SetValue("dNow", DateTime.Now.ToString("dd/MM/yyyy"));
            fr.SetValue("dvt", dvt.ToStringDvt());
            fr.SetValue("iNam", PhienLamViec.iNamLamViec);
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName);
            fr.Run(Result);

            return Result;
        }

        public ActionResult ChiTietPhanCapDauNamTheoQuyetDinhBaoCao4Partial(string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();

            var lstTreeNoiDungChi = _danhMucService.GetTreeAllDMNoiDungChiForReport(PhienLamViec.NamLamViec, null, null);
            if (lstTreeNoiDungChi == null || !lstTreeNoiDungChi.Any())
                return RedirectToAction("Index");

            foreach (DMNoiDungChiViewModel item in lstTreeNoiDungChi)
                item.fSoCon = CountChild(item, lstTreeNoiDungChi.ToList());

            data.treeNoiDungChi = lstTreeNoiDungChi;
            var lstDataDuToanTheoNam = _nnsService.GetDuToanPhanCapDauNamTheoSoQuyetDinh(PhienLamViec.NamLamViec, null, sSoQuyetDinh, dDateFrom, dDateTo, dvt);

            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            ViewBag.dvt = dvt;
            TempData["DataChiTietQuyetDinhPhanCapBaoCao4Partial"] = data;
            return View("PopupChiTietQuyetDinhPhanCapDauNamBaoCao4Partial", data);
        }

        public ActionResult ChiTietQuyetDinhBaoCao4PhanCapPartialExport(string ext = "pdf", int dvt = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong_hop_du_toan_nsqp_giao_cho_dv_theo_soquyetdinh", ext);
            ExcelFile xls = TaoFileBaoCaoChiTietQuyetDinhBaoCao4PhanCapPartial(dvt);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCaoChiTietQuyetDinhBaoCao4PhanCapPartial(int dvt = 1)
        {
            RptTongHopGiaoDuToanModel data = new RptTongHopGiaoDuToanModel();
            if (TempData["DataChiTietQuyetDinhPhanCapBaoCao4Partial"] != null)
            {
                data = (RptTongHopGiaoDuToanModel)TempData["DataChiTietQuyetDinhPhanCapBaoCao4Partial"];
            }

            TempData.Keep("DataChiTietQuyetDinhPhanCapBaoCao4Partial");

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao4_ChiTietPhanCap));
            FlexCelReport fr = new FlexCelReport();

            List<RptNNSDuToanChiTietModel> lstChiTiet = data.lstDuToanChiTiet.ToList().Where(x => x.iID_Parent == null).ToList();

            List<PhongBan> lstPhongBan = lstChiTiet.GroupBy(x => new
            {
                x.sMaPhongBan
            }).Select(g => new PhongBan
            {
                sMaPhongBan = g.Key.sMaPhongBan,
                sTenPhongBan = g.First().sTenPhongBan,
                iTongTien = g.Sum(x => x.iTongTien)
            }).ToList();

            double? fTongTien = null;

            if (lstChiTiet != null && lstChiTiet.Any())
                fTongTien = lstChiTiet.Sum(n => n.iTongTien ?? 0);

            string sSoQuyetDinh = string.Empty;
            string sNgayQuyetDinh = string.Empty;
            foreach (var item in data.lstDuToanChiTiet)
            {
                sSoQuyetDinh = item.sSoQuyetDinh;
                sNgayQuyetDinh = item.sNgayQuyetDinh;
                break;
            }
            fr.SetValue("sSoQuyetDinh", sSoQuyetDinh);
            fr.SetValue("sNgayQuyetDinh", sNgayQuyetDinh);
            fr.AddTable<RptNNSDuToanChiTietModel>("ChiTiet", lstChiTiet);
            fr.SetValue("fTongTien", fTongTien);
            fr.AddTable<PhongBan>("PhongBan", lstPhongBan);
            fr.SetValue("dNow", DateTime.Now.ToString("dd/MM/yyyy"));
            fr.SetValue("dvt", dvt.ToStringDvt());
            fr.SetValue("iNam", PhienLamViec.iNamLamViec);
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName);
            fr.Run(Result);

            return Result;
        }
        #endregion

        #region Tổng hợp giao dự toán ngân sách năm
        public ActionResult TongHopGiaoDuToanNSNam()
        {
            List<NNSDMNguonViewModel> lstNguon = _danhMucService.GetAllDMNguonBaoCao(PhienLamViec.NamLamViec).ToList();
            lstNguon.Insert(0, new NNSDMNguonViewModel { iID_Nguon = Guid.Empty, sNoiDung = Constants.TAT_CA });
            ViewBag.ListDanhMucNguon = lstNguon.ToSelectList("iID_Nguon", "sNoiDung");

            DateTime firstDay = new DateTime(PhienLamViec.NamLamViec, 1, 1);
            DateTime lastDay = new DateTime(PhienLamViec.NamLamViec, 12, 31);

            ViewBag.dNgayFromDefault = firstDay.ToString("dd/MM/yyyy");
            ViewBag.dNgayToDefault = lastDay.ToString("dd/MM/yyyy");

            return View();
        }

        #region theo don vi
        [HttpPost]
        public ActionResult TongHopGiaoDuToanNSTheoDonViPartial(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopDuToanNganSachNamModel data = new RptTongHopDuToanNganSachNamModel();
            data.lstDataChiTiet = _nnsService.GetTongHopDuToanNSTheoDonVi(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            TempData["TongHopGiaoDuToanNSTheoDonVi"] = data;
            return PartialView("TongHopGiaoDuToanNSTheoDonViPartial", data);
        }

        public ActionResult XuatFileTongHopGiaoDuToanNSTheoDonVi(string ext = "pdf", int dvt = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong hop du toan NS nam_Theo don vi", ext);
            ExcelFile xls = TaoFile(dvt);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFile(int dvt = 1)
        {
            RptTongHopDuToanNganSachNamModel data = new RptTongHopDuToanNganSachNamModel();
            if (TempData["TongHopGiaoDuToanNSTheoDonVi"] != null)
                data = (RptTongHopDuToanNganSachNamModel)TempData["TongHopGiaoDuToanNSTheoDonVi"];
            TempData.Keep("TongHopGiaoDuToanNSTheoDonVi");

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao5_TheoDonVi));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<RptChiTietTongHopDuToanNganSachNamModel>("dt", data.lstDataChiTiet);
            fr.SetValue("iNam", PhienLamViec.iNamLamViec);
            fr.SetValue("dvt", dvt.ToStringDvt());
            fr.UseChuKy(Username)
              .UseChuKyForController(sControlName);
            fr.Run(Result);
            return Result;
        }
        #endregion

        #region theo don vi bql
        [HttpPost]
        public ActionResult TongHopGiaoDuToanNSTheoDonViBQLPartial(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopDuToanNganSachNamModel data = new RptTongHopDuToanNganSachNamModel();
            data.lstDataChiTiet = _nnsService.GetTongHopDuToanNSTheoDonViBQL(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            TempData["TongHopGiaoDuToanNSTheoDonViBQL"] = data;
            return PartialView("TongHopGiaoDuToanNSTheoDonViBQLPartial", data);
        }

        public ActionResult XuatFileTongHopGiaoDuToanNSTheoDonViBQL(string ext = "pdf", int dvt = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong hop du toan NS nam_Theo don vi bql", ext);
            ExcelFile xls = TaoFileTheoDonViBQL(dvt);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileTheoDonViBQL(int dvt = 1)
        {
            RptTongHopDuToanNganSachNamModel data = new RptTongHopDuToanNganSachNamModel();
            if (TempData["TongHopGiaoDuToanNSTheoDonViBQL"] != null)
                data = (RptTongHopDuToanNganSachNamModel)TempData["TongHopGiaoDuToanNSTheoDonViBQL"];
            TempData.Keep("TongHopGiaoDuToanNSTheoDonViBQL");

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao5_TheoDonViBQL));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<RptChiTietTongHopDuToanNganSachNamModel>("ChiTiet", data.lstDataChiTiet);
            IEnumerable<string> listDonVi = data.lstDataChiTiet.Select(x => x.sTenDonVi).Distinct();
            DataTable dtDonVi = new DataTable();
            dtDonVi.Columns.Add("sTenDonVi", typeof(string));
            dtDonVi.Columns.Add("iStt", typeof(int));
            int i = 1;
            foreach (var item in listDonVi)
            {
                DataRow row = dtDonVi.NewRow();
                row["sTenDonVi"] = item;
                row["iStt"] = i;
                dtDonVi.Rows.Add(row);
                i++;
            }

            fr.AddTable("DonVi", dtDonVi);
            fr.SetValue("iNam", PhienLamViec.iNamLamViec);
            fr.SetValue("dvt", dvt.ToStringDvt());
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName);
            fr.Run(Result);
            return Result;
        }
        #endregion

        #region theo bql don vi
        [HttpPost]
        public ActionResult TongHopGiaoDuToanNSTheoBQLDonViPartial(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopDuToanNganSachNamModel data = new RptTongHopDuToanNganSachNamModel();
            data.lstDataChiTiet = _nnsService.GetTongHopDuToanNSTheoBQLDonVi(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            TempData["TongHopGiaoDuToanNSTheoBQLDonVi"] = data;
            return PartialView("TongHopGiaoDuToanNSTheoBQLDonViPartial", data);
        }

        public ActionResult XuatFileTongHopGiaoDuToanNSTheoBQLDonVi(string ext = "pdf", int dvt = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong hop du toan NS nam_Theo bql don vi", ext);
            ExcelFile xls = TaoFileTheoBQLDonVi(dvt);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileTheoBQLDonVi(int dvt = 1)
        {
            RptTongHopDuToanNganSachNamModel data = new RptTongHopDuToanNganSachNamModel();
            if (TempData["TongHopGiaoDuToanNSTheoBQLDonVi"] != null)
                data = (RptTongHopDuToanNganSachNamModel)TempData["TongHopGiaoDuToanNSTheoBQLDonVi"];
            TempData.Keep("TongHopGiaoDuToanNSTheoBQLDonVi");

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao5_TheoBQLDonVi));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<RptChiTietTongHopDuToanNganSachNamModel>("ChiTiet", data.lstDataChiTiet);
            IEnumerable<string> listPhongBan = data.lstDataChiTiet.Select(x => x.sTenPhongBan).Distinct();
            DataTable dtPhongBan = new DataTable();
            dtPhongBan.Columns.Add("sTenPhongBan", typeof(string));
            dtPhongBan.Columns.Add("iStt", typeof(int));
            int i = 1;
            foreach (var item in listPhongBan)
            {
                DataRow row = dtPhongBan.NewRow();
                row["sTenPhongBan"] = item;
                row["iStt"] = i;
                dtPhongBan.Rows.Add(row);
                i++;
            }

            fr.AddTable("dtBQL", dtPhongBan);
            fr.SetValue("iNam", PhienLamViec.iNamLamViec);
            fr.SetValue("dvt", dvt.ToStringDvt());
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName);
            fr.Run(Result);
            return Result;
        }
        #endregion
        #endregion

        #region Tổng hợp QĐ BS dự toán ngân sách năm
        public ActionResult TongHopQDBSDuToanNSNam()
        {
            List<NNSDMNguonViewModel> lstNguon = _danhMucService.GetAllDMNguonBaoCao(PhienLamViec.NamLamViec).ToList();
            lstNguon.Insert(0, new NNSDMNguonViewModel { iID_Nguon = Guid.Empty, sNoiDung = Constants.TAT_CA });
            ViewBag.ListDanhMucNguon = lstNguon.ToSelectList("iID_Nguon", "sNoiDung");

            DateTime firstDay = new DateTime(PhienLamViec.NamLamViec, 1, 1);
            DateTime lastDay = new DateTime(PhienLamViec.NamLamViec, 12, 31);

            ViewBag.dNgayFromDefault = firstDay.ToString("dd/MM/yyyy");
            ViewBag.dNgayToDefault = lastDay.ToString("dd/MM/yyyy");

            return View();
        }

        #region theo don vi
        [HttpPost]
        public ActionResult TongHopQDBSDuToanNSNamTheoDonViPartial(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopQDBSDuToanNSNamModel data = new RptTongHopQDBSDuToanNSNamModel();
            List<RptChiTietTongHopQDBSDuToanNSNamModel> lstChiTiet = _nnsService.GetTongHopQDBSDuToanNSNamTheoDonVi(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);
            if (lstChiTiet == null || lstChiTiet.Count == 0)
                return null;
            List<ThongTinDotBS> lstSoQuyetDinh = lstChiTiet.GroupBy(g => new { g.iID_DuToan, g.sSoQuyetDinh })
                .Select(i => new ThongTinDotBS { iID_DuToan = i.First().iID_DuToan, sSoQuyetDinh = i.First().sSoQuyetDinh }).ToList();
            List<ThongTinDonVi> lstDonVi = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi })
                .Select(x => new ThongTinDonVi { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi }).OrderBy(x => x.sTenDonVi).ToList();

            foreach (ThongTinDonVi item in lstDonVi)
            {
                item.lstGiaTri = new List<GiaTri>();
                item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi).Sum(x => x.iTongTien);
                foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                {
                    RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan).FirstOrDefault();
                    if (objChiTiet != null)
                        item.lstGiaTri.Add(new GiaTri
                        {
                            fSoTien = objChiTiet.iTongTien
                        });
                    else
                        item.lstGiaTri.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                }
            }

            foreach (ThongTinDotBS item in lstSoQuyetDinh)
            {
                item.dTongTien = lstChiTiet.Where(x => x.iID_DuToan == item.iID_DuToan).Sum(x => x.iTongTien);
            }

            data.lstDataChiTiet = lstChiTiet;
            data.listSoQuyetDinh = lstSoQuyetDinh.OrderBy(x => x.sSoQuyetDinh).ToList();
            data.listDonVi = lstDonVi;

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            TempData["TongHopQDBSDuToanNSNamTheoDonVi"] = data;
            return PartialView("TongHopQDBSDuToanNSNamTheoDonViPartial", data);
        }
        #endregion

        #region theo don vi bql
        [HttpPost]
        public ActionResult TongHopQDBSDuToanNSNamTheoDonViBQLPartial(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopQDBSDuToanNSNamModel data = new RptTongHopQDBSDuToanNSNamModel();
            List<RptChiTietTongHopQDBSDuToanNSNamModel> lstChiTiet = _nnsService.GetTongHopQDBSDuToanNSNamTheoDonViBQL(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);
            if (lstChiTiet == null || lstChiTiet.Count == 0)
                return null;
            List<ThongTinDotBS> lstSoQuyetDinh = lstChiTiet.GroupBy(g => new { g.iID_DuToan, g.sSoQuyetDinh })
                .Select(i => new ThongTinDotBS { iID_DuToan = i.First().iID_DuToan, sSoQuyetDinh = i.First().sSoQuyetDinh }).ToList();
            List<ThongTinDonVi> lstDonVi = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi })
                .Select(x => new ThongTinDonVi { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi }).OrderBy(x => x.sTenDonVi).ToList();
            List<ThongTinDonViPBan> lstDonViPBan = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi, g.sTenPhongBan, g.sMaPhongBan })
                .Select(x => new ThongTinDonViPBan { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi, sMaPhongBan = x.First().sMaPhongBan, sTenPhongBan = x.First().sTenPhongBan }).OrderBy(x => x.sTenDonVi).ToList();

            int index = 1;
            foreach (ThongTinDonVi item in lstDonVi)
            {
                item.iSTT = index++;
                item.lstGiaTri = new List<GiaTri>();
                item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi).Sum(x => x.iTongTien);
                foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                {
                    RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan).FirstOrDefault();
                    if (objChiTiet != null)
                        item.lstGiaTri.Add(new GiaTri
                        {
                            fSoTien = objChiTiet.iTongTien
                        });
                    else
                        item.lstGiaTri.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                }
            }

            foreach (ThongTinDotBS item in lstSoQuyetDinh)
            {
                item.dTongTien = lstChiTiet.Where(x => x.iID_DuToan == item.iID_DuToan).Sum(x => x.iTongTien);
            }

            foreach (ThongTinDonViPBan item in lstDonViPBan)
            {
                item.lstGiaTriDonViPBan = new List<GiaTri>();
                item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.sMaPhongBan == item.sMaPhongBan).Sum(x => x.iTongTien);
                foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                {
                    RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan && x.sMaPhongBan == item.sMaPhongBan).FirstOrDefault();
                    if (objChiTiet != null)
                        item.lstGiaTriDonViPBan.Add(new GiaTri
                        {
                            fSoTien = objChiTiet.iTongTien
                        });
                    else
                        item.lstGiaTriDonViPBan.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                }
            }

            data.lstDataChiTiet = lstChiTiet;
            data.listSoQuyetDinh = lstSoQuyetDinh.OrderBy(x => x.sSoQuyetDinh).ToList();
            data.listDonVi = lstDonVi;
            data.listDonViPBan = lstDonViPBan;

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            TempData["TongHopQDBSDuToanNSNamTheoDonViBQL"] = data;
            return PartialView("TongHopQDBSDuToanNSNamTheoDonViBQLPartial", data);
        }
        #endregion

        #region theo bql don vi
        [HttpPost]
        public ActionResult TongHopQDBSDuToanNSNamTheoBQLDonViPartial(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopQDBSDuToanNSNamModel data = new RptTongHopQDBSDuToanNSNamModel();
            List<RptChiTietTongHopQDBSDuToanNSNamModel> lstChiTiet = _nnsService.GetTongHopQDBSDuToanNSNamTheoDonViBQL(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);
            if (lstChiTiet == null || lstChiTiet.Count == 0)
                return null;
            List<ThongTinDotBS> lstSoQuyetDinh = lstChiTiet.GroupBy(g => new { g.iID_DuToan, g.sSoQuyetDinh })
                .Select(i => new ThongTinDotBS { iID_DuToan = i.First().iID_DuToan, sSoQuyetDinh = i.First().sSoQuyetDinh }).ToList();
            List<ThongTinPhongBan> lstPhongBan = lstChiTiet.GroupBy(g => new { g.sMaPhongBan, g.sTenPhongBan })
                .Select(x => new ThongTinPhongBan { sMaPhongBan = x.First().sMaPhongBan, sTenPhongBan = x.First().sTenPhongBan }).OrderBy(x => x.sTenPhongBan).ToList();
            List<ThongTinDonViPBan> lstDonViPBan = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi, g.sTenPhongBan, g.sMaPhongBan })
                .Select(x => new ThongTinDonViPBan { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi, sMaPhongBan = x.First().sMaPhongBan, sTenPhongBan = x.First().sTenPhongBan }).OrderBy(x => x.sTenDonVi).ToList();

            int index = 1;
            foreach (ThongTinPhongBan item in lstPhongBan)
            {
                item.iSTT = index++;
                item.lstGiaTriPhongBan = new List<GiaTri>();
                item.dTongTien = lstChiTiet.Where(x => x.sMaPhongBan == item.sMaPhongBan).Sum(x => x.iTongTien);
                foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                {
                    RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.sMaPhongBan == item.sMaPhongBan && x.iID_DuToan == itemDotBS.iID_DuToan).FirstOrDefault();
                    if (objChiTiet != null)
                        item.lstGiaTriPhongBan.Add(new GiaTri
                        {
                            fSoTien = objChiTiet.iTongTien
                        });
                    else
                        item.lstGiaTriPhongBan.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                }
            }

            foreach (ThongTinDotBS item in lstSoQuyetDinh)
            {
                item.dTongTien = lstChiTiet.Where(x => x.iID_DuToan == item.iID_DuToan).Sum(x => x.iTongTien);
            }

            foreach (ThongTinDonViPBan item in lstDonViPBan)
            {
                item.lstGiaTriDonViPBan = new List<GiaTri>();
                item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.sMaPhongBan == item.sMaPhongBan).Sum(x => x.iTongTien);
                foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                {
                    RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan && x.sMaPhongBan == item.sMaPhongBan).FirstOrDefault();
                    if (objChiTiet != null)
                        item.lstGiaTriDonViPBan.Add(new GiaTri
                        {
                            fSoTien = objChiTiet.iTongTien
                        });
                    else
                        item.lstGiaTriDonViPBan.Add(new GiaTri
                        {
                            fSoTien = null
                        });
                }
            }

            data.lstDataChiTiet = lstChiTiet;
            data.listSoQuyetDinh = lstSoQuyetDinh.OrderBy(x => x.sSoQuyetDinh).ToList();
            data.listPhongBan = lstPhongBan;
            data.listDonViPBan = lstDonViPBan;

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            TempData["TongHopQDBSDuToanNSNamTheoBQLDonVi"] = data;
            return PartialView("TongHopQDBSDuToanNSNamTheoBQLDonViPartial", data);
        }
        #endregion

        public RptTongHopQDBSDuToanNSNamModel GetDataBaoCao6(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1, int iTypeBC = 1)
        {
            if (iNguonNganSach == Guid.Empty)
                iNguonNganSach = null;

            RptTongHopQDBSDuToanNSNamModel data = new RptTongHopQDBSDuToanNSNamModel();
            if(iTypeBC == 1)
            {
                List<RptChiTietTongHopQDBSDuToanNSNamModel> lstChiTiet = _nnsService.GetTongHopQDBSDuToanNSNamTheoDonVi(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);
                if (lstChiTiet == null || lstChiTiet.Count == 0)
                    return null;
                List<ThongTinDotBS> lstSoQuyetDinh = lstChiTiet.GroupBy(g => new { g.iID_DuToan, g.sSoQuyetDinh })
                    .Select(i => new ThongTinDotBS { iID_DuToan = i.First().iID_DuToan, sSoQuyetDinh = i.First().sSoQuyetDinh }).OrderBy(x => x.sSoQuyetDinh).ToList();
                List<ThongTinDonVi> lstDonVi = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi })
                    .Select(x => new ThongTinDonVi { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi }).OrderBy(x => x.sTenDonVi).ToList();

                foreach (ThongTinDonVi item in lstDonVi)
                {
                    item.lstGiaTri = new List<GiaTri>();
                    item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi).Sum(x => x.iTongTien);
                    foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                    {
                        RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan).FirstOrDefault();
                        if (objChiTiet != null)
                            item.lstGiaTri.Add(new GiaTri
                            {
                                fSoTien = objChiTiet.iTongTien
                            });
                        else
                            item.lstGiaTri.Add(new GiaTri
                            {
                                fSoTien = null
                            });
                    }
                }

                foreach (ThongTinDotBS item in lstSoQuyetDinh)
                {
                    item.dTongTien = lstChiTiet.Where(x => x.iID_DuToan == item.iID_DuToan).Sum(x => x.iTongTien);
                }

                data.lstDataChiTiet = lstChiTiet;
                data.listSoQuyetDinh = lstSoQuyetDinh;
                data.listDonVi = lstDonVi;

                TempData["TongHopQDBSDuToanNSNamTheoDonVi"] = data;
            } else if(iTypeBC == 2)
            { 
                List<RptChiTietTongHopQDBSDuToanNSNamModel> lstChiTiet = _nnsService.GetTongHopQDBSDuToanNSNamTheoDonViBQL(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);
                if (lstChiTiet == null || lstChiTiet.Count == 0)
                    return null;
                List<ThongTinDotBS> lstSoQuyetDinh = lstChiTiet.GroupBy(g => new { g.iID_DuToan, g.sSoQuyetDinh })
                    .Select(i => new ThongTinDotBS { iID_DuToan = i.First().iID_DuToan, sSoQuyetDinh = i.First().sSoQuyetDinh }).OrderBy(x => x.sSoQuyetDinh).ToList();
                List<ThongTinDonVi> lstDonVi = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi })
                    .Select(x => new ThongTinDonVi { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi }).OrderBy(x => x.sTenDonVi).ToList();
                List<ThongTinDonViPBan> lstDonViPBan = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi, g.sTenPhongBan, g.sMaPhongBan })
                    .Select(x => new ThongTinDonViPBan { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi, sMaPhongBan = x.First().sMaPhongBan, sTenPhongBan = x.First().sTenPhongBan }).OrderBy(x => x.sTenDonVi).ToList();

                int index = 1;
                foreach (ThongTinDonVi item in lstDonVi)
                {
                    item.iSTT = index++;
                    item.lstGiaTri = new List<GiaTri>();
                    item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi).Sum(x => x.iTongTien);
                    foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                    {
                        RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan).FirstOrDefault();
                        if (objChiTiet != null)
                            item.lstGiaTri.Add(new GiaTri
                            {
                                fSoTien = objChiTiet.iTongTien
                            });
                        else
                            item.lstGiaTri.Add(new GiaTri
                            {
                                fSoTien = null
                            });
                    }
                }

                foreach (ThongTinDotBS item in lstSoQuyetDinh)
                {
                    item.dTongTien = lstChiTiet.Where(x => x.iID_DuToan == item.iID_DuToan).Sum(x => x.iTongTien);
                }

                foreach (ThongTinDonViPBan item in lstDonViPBan)
                {
                    item.lstGiaTriDonViPBan = new List<GiaTri>();
                    item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.sMaPhongBan == item.sMaPhongBan).Sum(x => x.iTongTien);
                    foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                    {
                        RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan && x.sMaPhongBan == item.sMaPhongBan).FirstOrDefault();
                        if (objChiTiet != null)
                            item.lstGiaTriDonViPBan.Add(new GiaTri
                            {
                                fSoTien = objChiTiet.iTongTien
                            });
                        else
                            item.lstGiaTriDonViPBan.Add(new GiaTri
                            {
                                fSoTien = null
                            });
                    }
                }

                data.lstDataChiTiet = lstChiTiet;
                data.listSoQuyetDinh = lstSoQuyetDinh;
                data.listDonVi = lstDonVi;
                data.listDonViPBan = lstDonViPBan;

                ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
                TempData["TongHopQDBSDuToanNSNamTheoDonViBQL"] = data;
            } else if(iTypeBC == 3)
            {
                List<RptChiTietTongHopQDBSDuToanNSNamModel> lstChiTiet = _nnsService.GetTongHopQDBSDuToanNSNamTheoDonViBQL(PhienLamViec.NamLamViec, iNguonNganSach, dDateFrom, dDateTo, sSoQuyetDinh, dvt);
                if (lstChiTiet == null || lstChiTiet.Count == 0)
                    return null;
                List<ThongTinDotBS> lstSoQuyetDinh = lstChiTiet.GroupBy(g => new { g.iID_DuToan, g.sSoQuyetDinh })
                    .Select(i => new ThongTinDotBS { iID_DuToan = i.First().iID_DuToan, sSoQuyetDinh = i.First().sSoQuyetDinh }).OrderBy(x => x.sSoQuyetDinh).ToList();
                List<ThongTinPhongBan> lstPhongBan = lstChiTiet.GroupBy(g => new { g.sMaPhongBan, g.sTenPhongBan })
                    .Select(x => new ThongTinPhongBan { sMaPhongBan = x.First().sMaPhongBan, sTenPhongBan = x.First().sTenPhongBan }).OrderBy(x => x.sTenPhongBan).ToList();
                List<ThongTinDonViPBan> lstDonViPBan = lstChiTiet.GroupBy(g => new { g.iID_MaDonVi, g.sTenDonVi, g.sTenPhongBan, g.sMaPhongBan })
                    .Select(x => new ThongTinDonViPBan { iID_MaDonVi = x.First().iID_MaDonVi, sTenDonVi = x.First().sTenDonVi, sMaPhongBan = x.First().sMaPhongBan, sTenPhongBan = x.First().sTenPhongBan }).OrderBy(x => x.sTenDonVi).ToList();

                int index = 1;
                foreach (ThongTinPhongBan item in lstPhongBan)
                {
                    item.iSTT = index++;
                    item.lstGiaTriPhongBan = new List<GiaTri>();
                    item.dTongTien = lstChiTiet.Where(x => x.sMaPhongBan == item.sMaPhongBan).Sum(x => x.iTongTien);
                    foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                    {
                        RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.sMaPhongBan == item.sMaPhongBan && x.iID_DuToan == itemDotBS.iID_DuToan).FirstOrDefault();
                        if (objChiTiet != null)
                            item.lstGiaTriPhongBan.Add(new GiaTri
                            {
                                fSoTien = objChiTiet.iTongTien
                            });
                        else
                            item.lstGiaTriPhongBan.Add(new GiaTri
                            {
                                fSoTien = null
                            });
                    }
                }

                foreach (ThongTinDotBS item in lstSoQuyetDinh)
                {
                    item.dTongTien = lstChiTiet.Where(x => x.iID_DuToan == item.iID_DuToan).Sum(x => x.iTongTien);
                }

                foreach (ThongTinDonViPBan item in lstDonViPBan)
                {
                    item.lstGiaTriDonViPBan = new List<GiaTri>();
                    item.dTongTien = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.sMaPhongBan == item.sMaPhongBan).Sum(x => x.iTongTien);
                    foreach (ThongTinDotBS itemDotBS in lstSoQuyetDinh)
                    {
                        RptChiTietTongHopQDBSDuToanNSNamModel objChiTiet = lstChiTiet.Where(x => x.iID_MaDonVi == item.iID_MaDonVi && x.iID_DuToan == itemDotBS.iID_DuToan && x.sMaPhongBan == item.sMaPhongBan).FirstOrDefault();
                        if (objChiTiet != null)
                            item.lstGiaTriDonViPBan.Add(new GiaTri
                            {
                                fSoTien = objChiTiet.iTongTien
                            });
                        else
                            item.lstGiaTriDonViPBan.Add(new GiaTri
                            {
                                fSoTien = null
                            });
                    }
                }

                data.lstDataChiTiet = lstChiTiet;
                data.listSoQuyetDinh = lstSoQuyetDinh;
                data.listPhongBan = lstPhongBan;
                data.listDonViPBan = lstDonViPBan;

                ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
                TempData["TongHopQDBSDuToanNSNamTheoBQLDonVi"] = data;
            }

            return data;
        }

        public JsonResult Ds_To_BaoCao6(Guid? iNguonNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1, int iTypeBC = 1)
        {
            RptTongHopQDBSDuToanNSNamModel data = GetDataBaoCao6(iNguonNganSach, sSoQuyetDinh, dDateFrom, dDateTo, dvt, iTypeBC);
            if(data == null || data.listSoQuyetDinh == null)
                return ds_ToIn(0, _columnCountBC6);
            return ds_ToIn(data.listSoQuyetDinh.Count(), _columnCountBC6);
        }

        public ActionResult ExportBaoCaoTongHopQDBSDuToanNSNam(string ext = "pdf", int dvt = 1, int iTypeBC = 1, int to = 1)
        {
            ExcelFile xls = TaoFileBaoCao6TheoDonVi(dvt, to);
            if (iTypeBC == 2)
                xls = TaoFileBaoCao6TheoDonViBQL(dvt, to);
            else if (iTypeBC == 3)
                xls = TaoFileBaoCao6TheoBQLDonVi(dvt, to);

            string sFileName = "Tong hop QD Bo sung Du toan NS Nam_Theo don vi";
            if (iTypeBC == 2)
                sFileName = "Tong hop QD Bo sung Du toan NS Nam_Theo don vi BQL";
            else if (iTypeBC == 3)
                sFileName = "Tong hop QD Bo sung Du toan NS Nam_Theo BQL don vi";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName);
        }

        public ExcelFile TaoFileBaoCao6TheoDonVi(int dvt = 1, int to = 1)
        {
            RptTongHopQDBSDuToanNSNamModel data = new RptTongHopQDBSDuToanNSNamModel();
            if (TempData["TongHopQDBSDuToanNSNamTheoDonVi"] != null)
            {
                data = (RptTongHopQDBSDuToanNSNamModel)TempData["TongHopQDBSDuToanNSNamTheoDonVi"];
                TempData.Keep("TongHopQDBSDuToanNSNamTheoDonVi");
            }

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao6_TheoDonVi));
            FlexCelReport fr = new FlexCelReport();

            int skip = _columnCountBC6 * (to - 1);
            List<ThongTinDonVi> listDonVi = data.listDonVi.Select(x => new ThongTinDonVi()
            {
                iSTT = x.iSTT,
                dTongTien = x.dTongTien,
                iID_MaDonVi = x.iID_MaDonVi,
                lstGiaTri = x.lstGiaTri,
                sTenDonVi = x.sTenDonVi
            }).ToList();
            foreach (ThongTinDonVi item in listDonVi)
            {
                item.lstGiaTri = item.lstGiaTri.Skip(skip).Take(_columnCountBC6).ToList();
            }

            fr.AddTable<ThongTinDotBS>("lstSqd", data.listSoQuyetDinh.Skip(skip).Take(_columnCountBC6));
            fr.AddTable<ThongTinDotBS>("lstSqd1", data.listSoQuyetDinh.Skip(skip).Take(_columnCountBC6));
            fr.AddTable<ThongTinDonVi>("lstDonVi", listDonVi);

            fr.SetValue(new
            {
                dNow = DateTime.Now.ToString("dd/MM/yyyy"),
                dvt = dvt.ToStringDvt(),
                iNam = PhienLamViec.iNamLamViec,
                To = to
            });
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName)
             .UseForm(this).Run(Result);

            if (to != 1)
            {
                Result.SetColHidden(5, true);
            }

            return Result;
        }

        public ExcelFile TaoFileBaoCao6TheoBQLDonVi(int dvt = 1, int to = 1)
        {
            RptTongHopQDBSDuToanNSNamModel data = new RptTongHopQDBSDuToanNSNamModel();
            if (TempData["TongHopQDBSDuToanNSNamTheoBQLDonVi"] != null)
            {
                data = (RptTongHopQDBSDuToanNSNamModel)TempData["TongHopQDBSDuToanNSNamTheoBQLDonVi"];
                TempData.Keep("TongHopQDBSDuToanNSNamTheoBQLDonVi");
            }    

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao6_TheoBQLDonVi));
            FlexCelReport fr = new FlexCelReport();

            int skip = _columnCountBC6 * (to - 1);
            List<ThongTinDonViPBan> listDonViPBan = data.listDonViPBan.Select(x => new ThongTinDonViPBan()
            {
                dTongTien = x.dTongTien,
                iID_MaDonVi = x.iID_MaDonVi,
                lstGiaTriDonViPBan = x.lstGiaTriDonViPBan,
                sTenDonVi = x.sTenDonVi,
                sMaPhongBan = x.sMaPhongBan,
                sTenPhongBan = x.sTenPhongBan
            }).ToList();
            foreach (ThongTinDonViPBan item in listDonViPBan)
            {
                item.lstGiaTriDonViPBan = item.lstGiaTriDonViPBan.Skip(skip).Take(_columnCountBC6).ToList();
            }

            List<ThongTinPhongBan> listPhongBan = data.listPhongBan.Select(x => new ThongTinPhongBan()
            {
                dTongTien = x.dTongTien,
                iSTT = x.iSTT,
                lstGiaTriPhongBan = x.lstGiaTriPhongBan,
                sMaPhongBan = x.sMaPhongBan,
                sTenPhongBan = x.sTenPhongBan
            }).ToList();
            foreach (ThongTinPhongBan item in listPhongBan)
            {
                item.lstGiaTriPhongBan = item.lstGiaTriPhongBan.Skip(skip).Take(_columnCountBC6).ToList();
            }

            fr.AddTable<ThongTinDotBS>("lstSqd", data.listSoQuyetDinh.Skip(skip).Take(_columnCountBC6).ToList());
            fr.AddTable<ThongTinDotBS>("lstSqd1", data.listSoQuyetDinh.Skip(skip).Take(_columnCountBC6).ToList());
            fr.AddTable<ThongTinDonViPBan>("lstDonVi", listDonViPBan);
            fr.AddTable<ThongTinPhongBan>("lstPhongBan", listPhongBan);

            fr.SetValue(new
            {
                dNow = DateTime.Now.ToString("dd/MM/yyyy"),
                dvt = dvt.ToStringDvt(),
                iNam = PhienLamViec.iNamLamViec,
                To = to
            });

            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName)
             .UseForm(this).Run(Result);
            return Result;
        }

        public ExcelFile TaoFileBaoCao6TheoDonViBQL(int dvt = 1, int to = 0)
        {
            RptTongHopQDBSDuToanNSNamModel data = new RptTongHopQDBSDuToanNSNamModel();
            if (TempData["TongHopQDBSDuToanNSNamTheoDonViBQL"] != null)
            {
                data = (RptTongHopQDBSDuToanNSNamModel)TempData["TongHopQDBSDuToanNSNamTheoDonViBQL"];
                TempData.Keep("TongHopQDBSDuToanNSNamTheoDonViBQL");
            }

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao6_TheoDonViBQL));
            FlexCelReport fr = new FlexCelReport();

            int skip = _columnCountBC6 * (to - 1);
            List<ThongTinDonViPBan> listDonViPBan = data.listDonViPBan.Select(x => new ThongTinDonViPBan()
            {
                dTongTien = x.dTongTien,
                iID_MaDonVi = x.iID_MaDonVi,
                lstGiaTriDonViPBan = x.lstGiaTriDonViPBan,
                sTenDonVi = x.sTenDonVi,
                sMaPhongBan = x.sMaPhongBan,
                sTenPhongBan = x.sTenPhongBan
            }).ToList();
            foreach (ThongTinDonViPBan item in listDonViPBan)
            {
                item.lstGiaTriDonViPBan = item.lstGiaTriDonViPBan.Skip(skip).Take(_columnCountBC6).ToList();
            }

            List<ThongTinDonVi> listDonVi = data.listDonVi.Select(x => new ThongTinDonVi()
            {
                iSTT = x.iSTT,
                dTongTien = x.dTongTien,
                iID_MaDonVi = x.iID_MaDonVi,
                lstGiaTri = x.lstGiaTri,
                sTenDonVi = x.sTenDonVi
            }).ToList();
            foreach (ThongTinDonVi item in listDonVi)
            {
                item.lstGiaTri = item.lstGiaTri.Skip(skip).Take(_columnCountBC6).ToList();
            }

            fr.AddTable<ThongTinDotBS>("lstSqd", data.listSoQuyetDinh);
            fr.AddTable<ThongTinDotBS>("lstSqd1", data.listSoQuyetDinh);
            fr.AddTable<ThongTinDonVi>("lstDonVi", listDonVi);
            fr.AddTable<ThongTinDonViPBan>("lstPhongBan", listDonViPBan);

            fr.SetValue(new
            {
                dNow = DateTime.Now.ToString("dd/MM/yyyy"),
                dvt = dvt.ToStringDvt(),
                iNam = PhienLamViec.iNamLamViec,
                To = to
            });

            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName)
             .UseForm(this).Run(Result);
            return Result;
        }

        #endregion

        #region Tổng hợp giao dự toán NSQP theo loại ngân sách
        public ActionResult TongHopGiaoDuToanLNS()
        {
            DateTime firstDay = new DateTime(PhienLamViec.NamLamViec, 1, 1);
            DateTime lastDay = new DateTime(PhienLamViec.NamLamViec, 12, 31);

            ViewBag.dNgayFromDefault = firstDay.ToString("dd/MM/yyyy");
            ViewBag.dNgayToDefault = lastDay.ToString("dd/MM/yyyy");

            List<SelectListItem> lstLoaiNganSach = new List<SelectListItem> {
                new SelectListItem{Text = Constants.LoaiNganSach.TypeName.CHI_NGAN_SACH_NHA_NUOC, Value=((int)Constants.LoaiNganSach.Type.CHI_NGAN_SACH_NHA_NUOC).ToString()},
                new SelectListItem{Text = Constants.LoaiNganSach.TypeName.CHI_THUONG_XUYEN_QP, Value=((int)Constants.LoaiNganSach.Type.CHI_THUONG_XUYEN_QP).ToString()}
            };
            ViewBag.drpLoaiNganSach = lstLoaiNganSach.ToSelectList("Value", "Text");

            List<NNSDMNguonViewModel> lstNguon = _danhMucService.GetAllDMNguonBaoCao(PhienLamViec.NamLamViec, (int)Constants.LoaiNganSach.Type.CHI_NGAN_SACH_NHA_NUOC).ToList();
            lstNguon.Insert(0, new NNSDMNguonViewModel { iID_Nguon = Guid.Empty, sNoiDung = Constants.TAT_CA });
            ViewBag.ListDanhMucNguon = lstNguon.ToSelectList("iID_Nguon", "sNoiDung");

            return View();
        }

        [HttpPost]
        public ActionResult TongHopGiaoDuToanLNSPartial(DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            RptGiaoDuToanTheoLNS data = new RptGiaoDuToanTheoLNS();
            List<TongTien> lstTongPhanNguon = _nnsService.GetTongPhanNguonLNS(PhienLamViec.NamLamViec, dDateFrom, dDateTo, dvt);
            if (lstTongPhanNguon == null || !lstTongPhanNguon.Any())
                return RedirectToAction("Index");
            data.fTongPhanNguonNSNN = lstTongPhanNguon[0].fTongNSNN;
            data.fTongPhanNguonTXQP = lstTongPhanNguon[0].fTongTXQP;

            List<TongTien> lstTongDuToan = _nnsService.GetTongDuToanLNS(PhienLamViec.NamLamViec, dDateFrom, dDateTo, dvt);
            if (lstTongDuToan == null || !lstTongDuToan.Any())
                return RedirectToAction("Index");
            data.fTongDuToanNSNN = lstTongDuToan[0].fTongNSNN;
            data.fTongDuToanTXQP = lstTongDuToan[0].fTongTXQP;

            var lstDataDuToanTheoNam = _nnsService.GetChiTietBoSung(PhienLamViec.NamLamViec, dDateFrom, dDateTo, dvt);

            data.lstDuToanChiTiet = lstDataDuToanTheoNam.Where(x => !string.IsNullOrEmpty(x.sSoQuyetDinh)).ToList();
            data.lstDuToanChoPheDuyet = lstDataDuToanTheoNam.Where(x => string.IsNullOrEmpty(x.sSoQuyetDinh) && x.iID_NhiemVu != Guid.Empty).GroupBy(g => new { g.iID_NhiemVu, g.sMaNoiDungChi }).Select(x => new ChiTietBoSung()
            {
                sMaNoiDungChi = x.Key.sMaNoiDungChi,
                iID_NhiemVu = x.Key.iID_NhiemVu,
                fTienNSNN = x.Sum(g => g.fTienNSNN),
                fTienTXQP = x.Sum(g => g.fTienTXQP),
                GhiChu = x.First().GhiChu
            }).ToList();

            ViewBag.iNamLamViec = PhienLamViec.NamLamViec;
            TempData["BaoCaoTongHopGiaoDuToanLNS"] = data;
            return PartialView("TongHopGiaoDuToanLNSPartial", data);
        }

        [HttpPost]
        public bool ExportBCTongHopGiaoDuToanLNS(List<List<string>> dataReport)
        {
            TempData["dataReportLNS"] = dataReport;
            return true;
        }


        public ActionResult ExportExcelTongHopGiaoDuToanLNS(string ext = "pdf", int dvt = 1)
        {
            ExcelFile xls = TaoFileBaoCao2(dvt);
            string sFileName = string.Format("{0}.{1}", "Tong_Hop_Giao_Du_Toan_Trong_Nam_Theo_Nguon_Ngan_Sach", ext);
            return Print(xls, ext, sFileName);
        }

        public ExcelFile TaoFileBaoCao2(int dvt = 1)
        {
            RptGiaoDuToanTheoLNS data = new RptGiaoDuToanTheoLNS();
            if (TempData["BaoCaoTongHopGiaoDuToanLNS"] != null)
                data = (RptGiaoDuToanTheoLNS)TempData["BaoCaoTongHopGiaoDuToanLNS"];

            TempData.Keep("BaoCaoTongHopGiaoDuToanLNS");

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao2));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<ChiTietBoSung>("ChiTiet", data.lstDuToanChiTiet);
            fr.AddTable<ChiTietBoSung>("ChoPheDuyet", data.lstDuToanChoPheDuyet);
            List<string> listPhongBan = data.lstDuToanChiTiet.Select(x => x.sTenPhongBan).Distinct().ToList();
            List<PhongBan> lstPhongBan = new List<PhongBan>();
            listPhongBan.ForEach(x =>
            {
                lstPhongBan.Add(new PhongBan
                {
                    sTenPhongBan = x
                });
            });

            fr.SetValue("fTongPhanNguonTXQP", data.fTongPhanNguonTXQP);
            fr.SetValue("fTongPhanNguonNSNN", data.fTongPhanNguonNSNN);
            fr.SetValue("fTongDuToanTXQP", data.fTongDuToanTXQP);
            fr.SetValue("fTongDuToanNSNN", data.fTongDuToanNSNN);

            fr.AddTable<PhongBan>("PhongBan", lstPhongBan);
            fr.SetValue("dNow", DateTime.Now.ToString("dd/MM/yyyy"));
            fr.SetValue("dvt", dvt.ToStringDvt());
            fr.SetValue("iNam", PhienLamViec.iNamLamViec);
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName);
            fr.Run(Result);
            return Result;
        }
        #endregion
    }
}