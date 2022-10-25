using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;

namespace VIETTEL.Areas.QLNH.Controllers.QuyetToan
{
    public class BaoCaoTaiSanController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        public ActionResult Index()
        {
            BaoCaoTaiSanModelViewModel vm = new BaoCaoTaiSanModelViewModel();

            vm._paging.CurrentPage = 1;
            
            vm.Items = _qlnhService.getListBaoCaoTaiSanModels(ref vm._paging, null);
            if (vm.Items == null)
                vm.Items = new List<BaoCaoTaiSanModel>();
            vm.Items2 = _qlnhService.getListBaoCaoTaiSanModelstb2(ref vm._paging, null);
            if (vm.Items2 == null)
                vm.Items2 = new List<BaoCaoTaiSanModel2>();
            ViewBag.ChangeTable =  true;
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonViModel { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.ListDonVi = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");

            List<NS_DonVi> lstDonViQL2 = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL2.Insert(0, new NS_DonViModel { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.ListDonVi2 = lstDonViQL2.ToSelectList("iID_Ma", "sMoTa");

            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetLookupDuAnTaiSan().ToList();
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            vm.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

            List<NH_DA_HopDong> lstHopDong = _qlnhService.GetLookupHopDongTaiSan().ToList();
            lstHopDong.Insert(0, new NH_DA_HopDong { ID = Guid.Empty, sTenHopDong = "--Chọn hợp đồng--" });
            vm.ListHopDong = lstHopDong.ToSelectList("ID", "sTenHopDong");

            return View(vm);
        }
        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, Guid? iID_DonViID = null, Guid? iID_DuAnID = null, Guid? iID_HopDongID = null)
        {
            BaoCaoTaiSanModelViewModel vm = new BaoCaoTaiSanModelViewModel();

            vm._paging = _paging;

            vm.Items = _qlnhService.getListBaoCaoTaiSanModels(ref vm._paging, iID_DonViID, iID_DuAnID, iID_HopDongID);

            vm.Items2 = _qlnhService.getListBaoCaoTaiSanModelstb2(ref vm._paging, iID_DonViID);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonViModel { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.ListDonVi = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");

            List<NS_DonVi> lstDonViQL2 = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL2.Insert(0, new NS_DonViModel { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            vm.ListDonVi2 = lstDonViQL2.ToSelectList("iID_Ma", "sMoTa");

            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetLookupDuAnTaiSan().ToList();
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            vm.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

            List<NH_DA_HopDong> lstHopDong = _qlnhService.GetLookupHopDongTaiSan().ToList();
            lstHopDong.Insert(0, new NH_DA_HopDong { ID = Guid.Empty, sTenHopDong = "--Chọn hợp đồng--" });
            vm.ListHopDong = lstHopDong.ToSelectList("ID", "sTenHopDong");

            return PartialView("_list", vm);
        }   

    }
}