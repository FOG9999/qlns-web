using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.QLNH.Controllers.DuAnHopDong
{
    public class BaoCaoTinhHinhThucHienDuAnController : FlexcelReportController
    {
        // GET: QLNH/BaoCaoTinhHinhThucHienDuAn
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sFilePathChiTiett = "/Report_ExcelFrom/QLNH/rpt_BaoCaoTinhHinhThucHienDuAn.xlsx";

        public ActionResult Index()
        {
            BaoCaoTHTHDuAnViewModel vm = new BaoCaoTHTHDuAnViewModel();
            vm.ListChiTiet = new BaoCaoTHTHDuAnModelPaging();
            vm.ListChiTiet._paging.CurrentPage = 1;
            vm.DuAnModel = new NH_DA_DuAnViewModel();

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");

            List<NH_DA_DuAn> lstDuAn = new List<NH_DA_DuAn>();
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

            return View(vm);
        }

        [HttpPost]
        public JsonResult GetDuAnByDonVi(Guid idDonVi)
        {
            var duAnList = _qlnhService.GetListDuAnByDonViID(idDonVi);

            return Json(new { data = duAnList, status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchBaoBaoThucHienDuAn(DateTime? dBatDau, DateTime? dKetThuc, Guid? iID_DuAnID, Guid? iID_DonViID)
        {
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");

            
            List<NH_DA_DuAn> lstDuAn = new List<NH_DA_DuAn>();
            if (iID_DonViID.HasValue)
            {
                lstDuAn = _qlnhService.GetListDuAnByDonViID(iID_DonViID.Value).ToList();
            }
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn", iID_DuAnID.HasValue ? iID_DuAnID.Value.ToString() : Guid.Empty.ToString());

            BaoCaoTHTHDuAnViewModel vm = new BaoCaoTHTHDuAnViewModel();
            vm.ListChiTiet = new BaoCaoTHTHDuAnModelPaging();
            vm.ListChiTiet._paging.CurrentPage = 1;
            NH_DA_DuAnViewModel duAnModel = _qlnhService.GetDuAnById(iID_DuAnID);
            vm.DuAnModel = duAnModel == null ? new NH_DA_DuAnViewModel() : duAnModel;

            var model = _qlnhService.GetBaoCaoTinhHinhThucHienDuAn(ref vm.ListChiTiet._paging, dBatDau, dKetThuc, iID_DuAnID);
            vm.ListChiTiet.Items = model.Items;
            ViewBag.Sum = model.Sum;
            ViewBag.Sumgn = model.Sumgn;
            return PartialView("_list", vm);
        }

        public ActionResult ExportGiayDeNghiThanhToan(string ext = "xlsx", Guid? iID_DuAnID = null, string dBatDau = "", string dKetThuc = "")
        {
            dBatDau = HttpUtility.UrlDecode(dBatDau ?? "");
            dKetThuc = HttpUtility.UrlDecode(dKetThuc ?? "");
            ExcelFile xls = FileBaoCaoTHTHDuAn(iID_DuAnID, CommonFunction.TryParseDateTime(dBatDau), CommonFunction.TryParseDateTime(dKetThuc));
            string sFileName = "Báo cáo tình hình thực hiện dự án";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName);
        }

        private ExcelFile FileBaoCaoTHTHDuAn(Guid? iID_DuAnID, DateTime? dBatDau, DateTime? dKetThuc)
        {
            BaoCaoTHTHDuAnViewModel vm = new BaoCaoTHTHDuAnViewModel();
            vm.ListChiTiet = new BaoCaoTHTHDuAnModelPaging();
            NH_DA_DuAnViewModel duAnModel = _qlnhService.GetDuAnById(iID_DuAnID);
            vm.DuAnModel = duAnModel == null ? new NH_DA_DuAnViewModel() : duAnModel;

            var model = _qlnhService.GetDataExportBaoCaoTinhHinhThucHienDuAn(dBatDau, dKetThuc, iID_DuAnID);
            vm.ListChiTiet.Items = model.Items;
            string sTenDonVi = "";
            if (vm.DuAnModel.iID_DonViID.HasValue)
            {
                NS_DonVi donViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false)
                                                   .Where(x => x.iID_Ma.Equals(vm.DuAnModel.iID_DonViID.Value)).FirstOrDefault();
                if (donViQL != null) sTenDonVi = donViQL.sMoTa;
            }
            string sTenDuAn = "";
            string sSoQuyetDinhDauTu = "";
            string dNgayQuyetDinhDauTu = "";
            string ChuDauTu = "";
            string sKhoiCong = "";
            string sTen = "";
            string sKetThuc = "";
            string fGiaTriUSD = "";
            if (!string.IsNullOrEmpty(vm.DuAnModel.sTenDuAn))
            {
                sTenDuAn = vm.DuAnModel.sTenDuAn;
            }
            if (!string.IsNullOrEmpty(vm.DuAnModel.sSoQuyetDinhDauTu))
            {
                sSoQuyetDinhDauTu = vm.DuAnModel.sSoQuyetDinhDauTu;
            }
            if (vm.DuAnModel.dNgayQuyetDinhDauTu.HasValue)
            {
                dNgayQuyetDinhDauTu = vm.DuAnModel.dNgayQuyetDinhDauTu.Value.ToString("dd/MM/yyyy");
            }
            if (!string.IsNullOrEmpty(vm.DuAnModel.SChuDauTu))
            {
                ChuDauTu = vm.DuAnModel.SChuDauTu;
            }
            if (!string.IsNullOrEmpty(vm.DuAnModel.STen))
            {
                sTen = vm.DuAnModel.STen;
            }
            if (vm.DuAnModel.fGiaTriUSD.HasValue)
            {
                fGiaTriUSD = CommonFunction.DinhDangSo(vm.DuAnModel.fGiaTriUSD.Value.ToString(CultureInfo.InvariantCulture), 2);
            }
            if (!string.IsNullOrEmpty(vm.DuAnModel.sKhoiCong))
            {
                sKhoiCong = vm.DuAnModel.sKhoiCong;
            }
            if (!string.IsNullOrEmpty(vm.DuAnModel.sKetThuc))
            {
                sKetThuc = vm.DuAnModel.sKetThuc;
            }

            double Sum = model.Sum;
            double Sumgn = model.Sumgn;

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathChiTiett));
            FlexCelReport fr = new FlexCelReport();
            fr.SetValue(new
            {
                dvt = "1",
                To = 1,
                sTenDuAn = sTenDuAn,
                sTenDonVi = sTenDonVi,
                sSoQuyetDinhDauTu = sSoQuyetDinhDauTu,
                dNgayQuyetDinhDauTu = dNgayQuyetDinhDauTu,
                ChuDauTu = ChuDauTu,
                sTen = sTen,
                fGiaTriUSD = fGiaTriUSD,
                sKhoiCong = sKhoiCong,
                sKetThuc = sKetThuc,
                Sum = Sum,
                Sumgn = Sumgn
            });

            foreach (var item in vm.ListChiTiet.Items)
            {
                item.SfTongDeNghi_USD = CommonFunction.DinhDangSo(item.fTongDeNghi_USD.HasValue ? item.fTongDeNghi_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty, 2);
                item.SfTongDeNghi_VND = CommonFunction.DinhDangSo(item.fTongDeNghi_VND.HasValue ? item.fTongDeNghi_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty, 0);
                item.SfTongPheDuyet_USD = CommonFunction.DinhDangSo(item.fTongPheDuyet_USD.HasValue ? item.fTongPheDuyet_USD.Value.ToString(CultureInfo.InvariantCulture) : string.Empty, 2);
                item.SfTongPheDuyet_VND = CommonFunction.DinhDangSo(item.fTongPheDuyet_VND.HasValue ? item.fTongPheDuyet_VND.Value.ToString(CultureInfo.InvariantCulture) : string.Empty, 0);
            }

            fr.AddTable("dt", vm.ListChiTiet.Items);

            fr.UseForm(this).Run(Result);
            return Result;
        }
    }

}