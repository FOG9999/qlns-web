using System;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using Viettel.Models.QLNH;
using System.Collections.Generic;
using System.Linq;
using FlexCel.Core;
using FlexCel.Report;
using VIETTEL.Flexcel;
using FlexCel.XlsAdapter;
using DomainModel;
using System.Globalization;

namespace VIETTEL.Areas.QLNH.Controllers.DuAnHopDong
{
    public class TongHopDuAnController : FlexcelReportController
    {
        private readonly IQLNHService qlnhService =QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sFilePathBaoCao1 = "/Report_ExcelFrom/QLNH/rpt_TongHopThongTinDuAn.xlsx";
        private const string sControlName = "TongHopThongTinDuAn";
        public ActionResult Index()
        {
            NHDAThongTinDuAnViewModel vm = new NHDAThongTinDuAnViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = qlnhService.getListTongHopDuAnModels(ref vm._paging);

            List<NS_PhongBan> listPhongBan = qlnhService.GetLookupQuanLy().ToList();
            listPhongBan.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sTen = "--Chọn ban quản lý--" });
            vm.ListPhongBan = listPhongBan.ToSelectList("iID_MaPhongBan", "sTen");

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.ListDonVi = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");

            //List<DM_ChuDauTu> listChuDauTu = qlnhService.GetLookupChuDauTu().ToList();
            //listChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = "--Chọn--" });
            //vm.ListChuDauTu = listChuDauTu;

            //List<NH_DM_PhanCapPheDuyet> listDmPhanCapPheDuyet = qlnhService.GetLookupThongTinDuAn().ToList();
            //listDmPhanCapPheDuyet.Insert(0, new NH_DM_PhanCapPheDuyet { ID = Guid.Empty, sTen = "--Chọn--" });
            //vm.ListDanhMucPCPD = listDmPhanCapPheDuyet;
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, Guid? iID_DonViID, Guid? iID_BQuanLyID)
        {
            NHDAThongTinDuAnViewModel vm = new NHDAThongTinDuAnViewModel();
            vm._paging = _paging;

            vm.Items = qlnhService.getListTongHopDuAnModels(ref vm._paging, iID_BQuanLyID, iID_DonViID);

            List<NS_PhongBan> listPhongBan = qlnhService.GetLookupQuanLy().ToList();
            listPhongBan.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sTen = "--Chọn ban quản lý--" });
            vm.ListPhongBan = listPhongBan.ToSelectList("iID_MaPhongBan", "sTen", iID_BQuanLyID != null && iID_BQuanLyID.HasValue ? iID_BQuanLyID.Value.ToString() : Guid.Empty.ToString());

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.ListDonVi = lstDonViQL.ToSelectList("iID_Ma", "sMoTa", iID_DonViID != null && iID_DonViID.HasValue ? iID_DonViID.Value.ToString() : Guid.Empty.ToString());

            //List<DM_ChuDauTu> olstThongTinDuAn = qlnhService.GetLookupChuDauTu().ToList();
            //olstThongTinDuAn.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = "--Chọn chủ đầu tư--" });
            //vm.ListChuDauTu = olstThongTinDuAn;

            //List<NH_DM_PhanCapPheDuyet> glstThongTinDuAn = qlnhService.GetLookupThongTinDuAn().ToList();
            //glstThongTinDuAn.Insert(0, new NH_DM_PhanCapPheDuyet { ID = Guid.Empty, sTen = "--Chọn dự án--" });
            //vm.ListDanhMucPCPD = glstThongTinDuAn;

            return PartialView("_list", vm);
        }

        public ActionResult ExportExcelBaoCao(string ext = "xlsx", int dvt = 1, int to = 1, Guid? iDonVi = null, Guid? iBQuanLy = null)
        {
            string fileName = string.Format("{0}.{1}", "BaoCaoTongHopThongTinDuAn", ext);
            ExcelFile xls = TaoFileBaoCao1(dvt, to , iDonVi , iBQuanLy);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCao1(int dvt = 1, int to = 1, Guid? iDonVi = null, Guid? iBQuanLy = null)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao1));
            FlexCelReport fr = new FlexCelReport();

            var Items = qlnhService.getListTongHopDuAn_BaoCaoModels(iBQuanLy, iDonVi);
            List<NHDAThongTinDuAnModel> ListData = new List<NHDAThongTinDuAnModel>();
            var index = 0;
            foreach (var item in Items)
            {
                index++;
                item.depth = index;
                item.sGiaTriUSD = item.fGiaTriNgoaiTeKhac.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriUSD.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty;
                item.sGiaTriVND = item.fGiaTriVND.HasValue ? CommonFunction.DinhDangSo(Math.Round(item.fGiaTriVND.Value).ToString(CultureInfo.InvariantCulture), 0) : string.Empty;
                item.sGiaTriEUR = item.fGiaTriEUR.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriEUR.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty;
                item.sGiaTriNgoaiTeKhac = item.fGiaTriNgoaiTeKhac.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriNgoaiTeKhac.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty;
                ListData.Add(item);
            }
            
            fr.AddTable<NHDAThongTinDuAnModel>("dt",ListData);
            fr.SetValue(new
            {
                dvt = dvt.ToStringDvt(),
                To = to,
            });
            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this).Run(Result);

            return Result;
        }
    }
}