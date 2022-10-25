using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.BaoHiemXaHoi;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.BaoHiemXaHoi.Controllers
{
    public class BaoCaoBHXHController : FlexcelReportController
    {
        private readonly IBaoHiemXaHoiService _bHXHService = BaoHiemXaHoiService.Default;
        private const string sFilePathChiTiet = "/Report_ExcelFrom/BHXH/rpt_TheoDoiBenhNhanDieuTriNoiTru_ChiTiet.xls";
        private const string sFilePathTongHopCap1 = "/Report_ExcelFrom/BHXH/rpt_TheoDoiBenhNhanDieuTriNoiTru_TongHop_Cap1.xls";
        private const string sFilePathTongHopCap2 = "/Report_ExcelFrom/BHXH/rpt_TheoDoiBenhNhanDieuTriNoiTru_TongHop_Cap2.xls";
        // GET: BaoHiemXaHoi/BaoCaoBHXH
        //Bao cao QL benh nhan dieu tri noi tru
        public ActionResult Index()
        {
            //List<BHXH_DonVi> lstDonViBHXHParent = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec)).ToList();
            //ViewBag.ListDonViBHXHParent = lstDonViBHXHParent.Where(x=>x.iID_ParentID == null && x.iID_MaDonViBHXH.Length == 2).ToSelectList("iID_MaDonViBHXH", "sTen");
            return View();
        }

        public JsonResult GetDataComboBoxDonViBHParent()
        {
            var result = new List<dynamic>();
            var listDonViBHParent = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec), Username).Where(x => x.iID_ParentID == null && x.iID_MaDonViBHXH.Length == 2).ToList();
            if (listDonViBHParent != null && listDonViBHParent.Any())
            {
                result.Insert(0, new { id = "", text = "--Chọn--" });
                foreach (var item in listDonViBHParent)
                {
                    result.Add(new { id = item.iID_MaDonViBHXH, text = item.iID_MaDonViBHXH + " - " + item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        //public ActionResult BaoCaoTheoDoiQLBenhNhanNoiTru(int? iThang, string sMaDonViNS, string sMaDonViBHXH)
        //{
        //    List<QLBenhNhanBHXHViewModel> data = _bHXHService.GetBaoCaoBenhNhanNoiTru(PhienLamViec.NamLamViec, iThang, sMaDonViNS, sMaDonViBHXH).ToList();
        //    TempData["QLBenhNhanNoiTru"] = data;
        //    return PartialView("TheoDoiQLBenhNhanNoiTruPartial", data);
        //}

        public ActionResult BaoCaoTheoDoiQLBenhNhanNoiTru(int? iThangBatDau, int? iThangKetThuc, string sMaDonViBHXHParent)
        {
            List<QLBenhNhanBHXHViewModel> data = _bHXHService.GetBaoCaoBenhNhanNoiTru(PhienLamViec.NamLamViec, iThangBatDau, iThangKetThuc, sMaDonViBHXHParent).ToList();
            TempData["QLBenhNhanNoiTru"] = data;
            return PartialView("TheoDoiQLBenhNhanNoiTruPartial", data);
        }

        public ActionResult ExportFile(string sMaDonViBHXH, string sMaDonViNS, int? iThangFrom, int? iThangTo, int? iSoNgayDieuTri, string sHoTen, string sMaThe, string ext = "pdf")
        {
            string fileName = string.Format("{0}.{1}", "Theo doi QL Benh nhan dieu tri noi tru", ext);
            ExcelFile xls = TaoFileExel(sMaDonViBHXH, sMaDonViNS, iThangFrom, iThangTo, iSoNgayDieuTri, sHoTen, sMaThe);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileExel(string sMaDonViBHXH, string sMaDonViNS, int? iThangFrom, int? iThangTo, int? iSoNgayDieuTri, string sHoTen, string sMaThe)
        {
            var datas = new List<QLBenhNhanBHXHViewModel>();

            QLBenhNhanBHXHPagingViewModel vm = new QLBenhNhanBHXHPagingViewModel();
            vm._paging = new PagingInfo();
            vm.Items = _bHXHService.GetAllBenhNhanDieuTriBHXH(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), iThangFrom, iThangTo, sMaDonViBHXH, sMaDonViNS, iSoNgayDieuTri, sHoTen, sMaThe);
            List<string> lstDonViBHXH = vm.Items.Select(x => x.iID_MaDonVi).Distinct().ToList();

            string strDonviBHXH = lstDonViBHXH.Join();

            List<BHXHDonViTree> treeDonVi = _bHXHService.GetTreeBHXHDonVi(sMaDonViNS, sMaDonViBHXH, strDonviBHXH);
            //List<BHXHDonViTree> dtLv1 = treeDonVi.Where(x => x.depth == 1).ToList();
            //List<BHXHDonViTree> dtLv2 = treeDonVi.Where(x => x.depth == 2).ToList();

            List<BHXHDonViTree> dtLv1 = new List<BHXHDonViTree>();
            List<BHXHDonViTree> dtLv2 = new List<BHXHDonViTree>();

            foreach(var item in treeDonVi)
            {
                foreach (var item2 in lstDonViBHXH)
                {
                    if (item.iID_MaDonViBHXH == item2 && item.depth == 2)
                    {
                        dtLv2.Add(item);
                        break;
                    }
                }
            }

            foreach(var item in treeDonVi)
            {
                foreach (var item2 in dtLv2)
                {
                    item2.iTongSoNguoiDieuTri = vm.Items.Count(x => x.iID_MaDonVi == item2.iID_MaDonViBHXH);
                    item2.iTongSoNgayDieuTri = vm.Items.Where(x => x.iID_MaDonVi == item2.iID_MaDonViBHXH).Sum(x => x.iSoNgayDieuTri != null ? x.iSoNgayDieuTri.Value : 0);
                    if (item.iID_BHXH_DonViID == item2.iID_ParentID && item.depth == 1)
                    {
                        dtLv1.Add(item);
                        break;
                    }
                    
                }
            }

            string donviNS = string.Empty;
            string donviBHXH = string.Empty;

            if (vm.Items.Count() > 0)
            {
                if (!string.IsNullOrEmpty(sMaDonViNS))
                    donviNS = vm.Items.First().sTenDonViMapping;

                if (!string.IsNullOrEmpty(sMaDonViBHXH))
                    donviBHXH = vm.Items.First().sTenDonViBHXH;
            }

            int iTongSoLuotDieuTri = vm.Items.Count();
            int iTongSoNgayDieuTri = vm.Items.Sum(x => x.iSoNgayDieuTri ?? 0);

            int thangFrom = iThangFrom == null ? 1 : iThangFrom.Value;
            int thangTo = iThangTo == null ? 12 : iThangTo.Value;

            int tongSoNguoiLv2 = 0;
            int tongSoNgayLv2 = 0;

            int iThangHienTai = DateTime.Now.Month;
            int iThangTruoc = iThangHienTai == 1 ? 12 : iThangHienTai - 1;

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathChiTiet));
            FlexCelReport fr = new FlexCelReport();
            fr.SetValue("iNamLamViec", PhienLamViec.NamLamViec);
            fr.SetValue("iTongSoLuotDieuTri", iTongSoLuotDieuTri.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")));
            fr.SetValue("iTongSoNgayDieuTri", iTongSoNgayDieuTri.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")));

            fr.SetValue("iThangFrom", thangFrom.ToString());
            fr.SetValue("iThangTo", thangTo.ToString());

            fr.SetValue("iTongSoNguoi", tongSoNguoiLv2.ToString());
            fr.SetValue("iTongSoNgay", tongSoNgayLv2.ToString());

            fr.SetValue("iThangHienTai", iThangHienTai.ToString());
            fr.SetValue("iThangTruoc", iThangTruoc.ToString());

            fr.AddTable<BHXHDonViTree>("dt1", dtLv1);
            fr.AddTable<BHXHDonViTree>("dt2", dtLv2);

            if (!string.IsNullOrEmpty(donviNS) || !string.IsNullOrEmpty(donviBHXH))
                fr.SetValue("donvi", string.Format("Đơn vị: {0} {1}", donviNS, donviBHXH));
            else
                fr.SetValue("donvi", string.Empty);
            fr.AddTable<QLBenhNhanBHXHViewModel>("dtChiTiet", vm.Items);
            fr.UseForm(this).Run(Result);
            return Result;
        }

        #region báo cáo tổng hợp
        public ActionResult ExportBaoCaoTongHop(string ext = "pdf", int kieuBC = 1)
        {
            string fileName = string.Format("{0}.{1}", "Tong hop Benh nhan dieu tri noi tru", ext);
            ExcelFile xls = TaoFileExelTongHop(kieuBC);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileExelTongHop(int kieuBC)
        {
            var datas = new List<QLBenhNhanBHXHViewModel>();

            string lstDonViBHXHByUser = _bHXHService.GetListDonViBHXH(PhienLamViec.NamLamViec, Username).Select(x => x.iID_MaDonViBHXH).Join();

            List<TongHopBenhNhanDonVi> listTongHop = _bHXHService.GetTongHopBenhNhanTheoDonVi(PhienLamViec.NamLamViec, lstDonViBHXHByUser);
            List<BHXHDonViTree> treeDonVi = _bHXHService.GetDataBaoCaoTongHop(PhienLamViec.NamLamViec, lstDonViBHXHByUser);

            List<BHXHDonViTree> dtLv1 = treeDonVi.Where(x => x.depth == 1).ToList();

            List<BHXHDonViTree> dtLv2 = new List<BHXHDonViTree>();
            if (kieuBC == 2)
                dtLv2 = treeDonVi.Where(x => x.depth == 2).ToList();

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(kieuBC == 1 ? sFilePathTongHopCap1 : sFilePathTongHopCap2));
            FlexCelReport fr = new FlexCelReport();
            fr.SetValue("iNamLamViec", PhienLamViec.NamLamViec);
            fr.AddTable<BHXHDonViTree>("dt1", dtLv1);
            if (kieuBC == 2)
                fr.AddTable<BHXHDonViTree>("dt2", dtLv2);
            fr.UseForm(this).Run(Result);
            return Result;
        }
        #endregion
    }
}