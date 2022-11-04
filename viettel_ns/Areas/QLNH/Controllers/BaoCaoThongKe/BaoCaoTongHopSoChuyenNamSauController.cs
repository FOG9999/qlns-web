using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH.QuyetToan;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Flexcel;
using System.Web;

namespace VIETTEL.Areas.QLNH.Controllers.BaoCaoThongKe
{
    public class BaoCaoTongHopSoChuyenNamSauController : FlexcelReportController
    {
        // GET: QLNH/BaoCaoTongHopSoChuyenNamSau
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sFilePathBaoCao = "/Report_ExcelFrom/QLNH/rpt_BaoCaoSoChuyenNamSau.xlsx";
        private const string sControlName = "BaoCaoTongHopSoChuyenNamSau";

        public List<Dropdown_SelectValue> lstDonViVND = new List<Dropdown_SelectValue>()
            {
                new Dropdown_SelectValue()
                {
                    Value = 1,
                    Label = "Đồng"
                },
                 new Dropdown_SelectValue()
                {
                    Value = 1000,
                    Label = "Nghìn đồng"
                }, new Dropdown_SelectValue()
                {
                    Value = 1000000000,
                    Label = "Tỉ đồng"
                }
            };
        public List<Dropdown_SelectValue> lstDonViUSD = new List<Dropdown_SelectValue>()
            {
                new Dropdown_SelectValue()
                {
                    Value = 1,
                    Label = "USD"
                },
                 new Dropdown_SelectValue()
                {
                    Value = 1000,
                    Label = "Nghìn USD"
                }, new Dropdown_SelectValue()
                {
                    Value = 1000000000,
                    Label = "Tỉ USD"
                }
            };

        public ActionResult Index()
        {
            NH_QT_QuyetToanNienDo_ChiTietView vm = new NH_QT_QuyetToanNienDo_ChiTietView();
            vm.ListDetailQuyetToanNienDo = new List<NH_QT_QuyetToanNienDo_ChiTietData>();


            List<Dropdown_SelectValue> lstNamKeHoach = GetListNamKeHoach();
            lstNamKeHoach.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKeHoach = lstNamKeHoach;
            return View(vm);
        }
        [HttpPost]
        public ActionResult TimKiem(int? iNamKeHoach)
        {
            NH_QT_QuyetToanNienDo_ChiTietView vm = new NH_QT_QuyetToanNienDo_ChiTietView();
            List<NS_DonVi> lstDonViQL = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<Dropdown_SelectValue> lstNamKeHoach = GetListNamKeHoach();
            lstNamKeHoach.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKeHoach = lstNamKeHoach;

            var listResult = getListDetailChiTiet(iNamKeHoach, false, null, null);
            vm.ListDetailQuyetToanNienDo = listResult;

            return PartialView("_baoCaoDetail", vm);
        }

        [ValidateInput(false)]
        public ActionResult ExportFile(string txtTieuDe1, string txtTieuDe2, string txtDonViCapTren, string txtDonVi, Guid? txtIdDonVi, int txtNamKeHoach, int? slbDonViUSD, int? slbDonViVND, string ext = "xlsx", int to = 1)
        {
            txtTieuDe1 = HttpUtility.UrlDecode(txtTieuDe1);
            txtTieuDe2 = HttpUtility.UrlDecode(txtTieuDe2);
            txtDonViCapTren = HttpUtility.UrlDecode(txtDonViCapTren);
            txtDonVi = HttpUtility.UrlDecode(txtDonVi);
            string fileName = string.Format("{0}.{1}", "Bao cao chi tiet so chuyen nam sau nam" + txtNamKeHoach, ext);
            ExcelFile xls = TaoFileExel(txtTieuDe1, txtTieuDe2, txtDonViCapTren, txtDonVi, txtNamKeHoach, slbDonViUSD, slbDonViVND, to);
            return Print(xls, ext, fileName);
        }
        public ExcelFile TaoFileExel(string txtTieuDe1, string txtTieuDe2, string txtDonViCapTren, string txtDonVi, int iNamKeHoach, int? slbDonViUSD, int? slbDonViVND, int to = 1)
        {
            var donViVND = lstDonViVND.Find(x => x.Value == slbDonViVND);
            var donViUSD = lstDonViUSD.Find(x => x.Value == slbDonViUSD);
            List<NH_QT_QuyetToanNienDo_ChiTietData> data = new List<NH_QT_QuyetToanNienDo_ChiTietData>();
            data = getListDetailChiTiet(iNamKeHoach, true, donViUSD.Value, donViVND.Value);
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao));
            FlexCelReport fr = new FlexCelReport();

            fr.SetValue(new
            {
                To = to,
                txtTieuDe1 = txtTieuDe1.ToUpper(),
                txtTieuDe2 = txtTieuDe2,
                donViUSD = donViUSD.Label,
                donViVND = donViVND.Label,
                txtDonVi = "",
                txtNam = "Năm:  " + iNamKeHoach.ToString(),
                txtDonViCapTren = txtDonViCapTren.ToUpper(),
                txtDonViCapDuoi = txtDonVi.ToUpper()
            });
            //fr.SetValue("iTongSoNgayDieuTri", iTongSoNgayDieuTri.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")));
            foreach (var item in data)
            {
                item.sTenNoiDungChi = (item.STT != null ? item.STT + "," : string.Empty) + item.sTenNoiDungChi;
            }
            fr.AddTable<NH_QT_QuyetToanNienDo_ChiTietData>("dt", data);

            fr.UseChuKy(Username)
              .UseChuKyForController(sControlName)
              .UseForm(this).Run(Result);

            return Result;
        }


        [HttpPost]
        public ActionResult GetModalInBaoCao(int slbNamKeHoach)
        {
            lstDonViVND.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn đơn vị VND--" });
            ViewBag.ListDVVND = lstDonViVND;


            lstDonViUSD.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn đơn vị USD--" });
            ViewBag.ListDVUSD = lstDonViUSD;

            ViewBag.iNamKeHoach = slbNamKeHoach;

            return PartialView("_modalInBaoCao");
        }
        public List<NH_QT_QuyetToanNienDo_ChiTietData> getListDetailChiTiet(int? iNamKeHoach, bool isPrint, int? donViUSD, int? donViVND)
        {
            var listResult = new List<NH_QT_QuyetToanNienDo_ChiTietData>();

            var listData = _qlnhService.GetBaoCaoTongHopSoChuyenNamSauDetail(iNamKeHoach).ToList();
            //var listTitle = new List<NH_QT_QuyetToanNienDo_ChiTietData>();
            var listDonVi = listData.Select(x => x.iID_DonVi).Distinct().ToList();
            List<string> arr = new List<string>()
            {
                "USD",
                "VND"
            };
            var listTiGia = listData.Select(x => new { x.iID_TiGiaID, x.iID_QuyetToanNienDoID }).Distinct().ToList();
            var listTiGiaTienTe = new List<TiGia_BaoCaoSoChuyenNamSau>();
            listTiGia.ForEach(x =>
            {
                var tiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTiet(x.iID_TiGiaID, false).Where(y => arr.Contains(y.sMaTienTeQuyDoi)).FirstOrDefault();
                var tiGia = _qlnhService.GetNHDMTiGiaList(x.iID_TiGiaID).FirstOrDefault();

                listTiGiaTienTe.Add(new TiGia_BaoCaoSoChuyenNamSau()
                {
                    fDonViTiGia = tiGiaChiTiet != null ? tiGiaChiTiet.fTiGia : 1,
                    sMaTienTeGoc = tiGiaChiTiet != null ? tiGia.sMaTienTeGoc : "",
                    iID_QuyetToanNienDoID = x.iID_QuyetToanNienDoID,
                });
            });

            foreach (var idMaDonVi in listDonVi)
            {
                var donVi = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).FirstOrDefault(x => x.iID_Ma == idMaDonVi);
                var newObjDonVi = new NH_QT_QuyetToanNienDo_ChiTietData()
                {
                    sTenNoiDungChi = donVi.iID_MaDonVi + '-' + donVi.sTen,
                    bIsTittle = true
                };
                listResult.Add(newObjDonVi);

                var listTitle = listData.Where(x => x.iID_ParentID != null).ToList();

                var getAllChuongTrinh = listData.Where(x => x.iID_DonVi == idMaDonVi && x.iID_KHCTBQP_NhiemVuChiID != null).Select(x => new { x.sTenNhiemVuChi, x.iID_KHCTBQP_NhiemVuChiID, x.iID_QuyetToanNienDoID }).Distinct().ToList();

                var iCountChuongTrinh = 0;

                foreach (var chuongTrinh in getAllChuongTrinh)
                {
                    iCountChuongTrinh++;
                    var dataSumChuongTrinh = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_ParentID == null).ToList();

                    var getListDuAn = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DonVi == idMaDonVi && x.iID_DuAnID != null && x.iID_ParentID == null).ToList();
                    var getListHopDong = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DonVi == idMaDonVi && x.iID_DuAnID == null && x.iID_HopDongID != null && x.iID_ParentID == null).ToList();
                    var iCountDuAn = 0;
                    var newObj = new NH_QT_QuyetToanNienDo_ChiTietData()
                    {
                        STT = convertLetter(iCountChuongTrinh),
                        sTenNoiDungChi = chuongTrinh.sTenNhiemVuChi,
                        iID_QuyetToanNienDoID = chuongTrinh.iID_QuyetToanNienDoID,
                        iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID,
                        bIsTittle = true,
                        sLevel = "0",
                        iParentId = 0
                    };
                    listResult.Add(newObj);

                    if (getListDuAn.Any())
                    {
                        var getNameDuAn = getListDuAn.Select(x => new { x.sTenDuAn, x.iID_DuAnID, x.fHopDong_VND_DuAn, x.fHopDong_USD_DuAn })
                        .Distinct()
                        .ToList();
                        foreach (var hopDongDuAn in getNameDuAn)
                        {
                            iCountDuAn++;
                            var newObjHopDongDuAn = new NH_QT_QuyetToanNienDo_ChiTietData();
                            var findTittle = listTitle.Find(x => x.iID_ParentID == hopDongDuAn.iID_DuAnID && x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == hopDongDuAn.iID_DuAnID);
                            var dataSumDuAn = getListDuAn.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == hopDongDuAn.iID_DuAnID).ToList();
                            if (findTittle != null)
                            {
                                newObjHopDongDuAn.MapFrom(findTittle);

                            }
                            //newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_USD = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamNay_USD : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD);
                            //newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_VND = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamNay_VND : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND);
                            //newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD = findTittle != null ? findTittle.fQTKinhPhiDuocCap_TongSo_USD : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD);
                            //newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND = findTittle != null ? findTittle.fQTKinhPhiDuocCap_TongSo_VND : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND);
                            //newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD);
                            //newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND);

                            newObjHopDongDuAn.STT = convertLaMa(Decimal.Parse(iCountDuAn.ToString()));
                            newObjHopDongDuAn.sTenNoiDungChi = hopDongDuAn.sTenDuAn;
                            newObjHopDongDuAn.bIsTittle = true;
                            newObjHopDongDuAn.bIsData = true;
                            newObjHopDongDuAn.sLevel = "1";
                            newObjHopDongDuAn.iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID;
                            newObjHopDongDuAn.iID_DuAnID = hopDongDuAn.iID_DuAnID;
                            newObjHopDongDuAn.iSum = 1;
                            newObjHopDongDuAn.iID_QuyetToanNienDoID = chuongTrinh.iID_QuyetToanNienDoID;

                            listResult.Add(newObjHopDongDuAn);
                            listResult.AddRange(returnLoaiChi(chuongTrinh.iID_KHCTBQP_NhiemVuChiID, null, true
                                , getListDuAn, listTitle.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == hopDongDuAn.iID_DuAnID).ToList()
                                ));

                        }
                    }
                    if (getListHopDong.Any())
                    {
                        iCountDuAn++;
                        var getSumHopDong = getListHopDong.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == null && x.iID_HopDongID != null).ToList();
                        var getThisList = listTitle.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == null && x.iID_HopDongID != null).ToList();
                        var newObjHopDong = new NH_QT_QuyetToanNienDo_ChiTietData()
                        {
                            STT = convertLaMa(Decimal.Parse(iCountDuAn.ToString())),
                            sTenNoiDungChi = "Chi hợp đồng",
                            bIsTittle = true,
                            fQTKinhPhiDuocCap_NamNay_USD = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD),
                            fQTKinhPhiDuocCap_NamNay_VND = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND),
                            fQTKinhPhiDuocCap_TongSo_USD = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD),
                            fQTKinhPhiDuocCap_TongSo_VND = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND),
                            fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD),
                            fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND),

                            fDeNghiQTNamNay_USD = getThisList.Any() ? getThisList.Sum(x => x.fDeNghiQTNamNay_USD) : getListHopDong.Sum(x => x.fDeNghiQTNamNay_USD),
                            fDeNghiQTNamNay_VND = getThisList.Any() ? getThisList.Sum(x => x.fDeNghiQTNamNay_VND) : getListHopDong.Sum(x => x.fDeNghiQTNamNay_VND),
                            fDeNghiChuyenNamSau_USD = getThisList.Any() ? getThisList.Sum(x => x.fDeNghiChuyenNamSau_USD) : getListHopDong.Sum(x => x.fDeNghiChuyenNamSau_USD),
                            fDeNghiChuyenNamSau_VND = getThisList.Any() ? getThisList.Sum(x => x.fDeNghiChuyenNamSau_VND) : getListHopDong.Sum(x => x.fDeNghiChuyenNamSau_VND),
                            fThuaThieuKinhPhiTrongNam_USD = getThisList.Any() ? getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD) : getListHopDong.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD),
                            fThuaThieuKinhPhiTrongNam_VND = getThisList.Any() ? getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND) : getListHopDong.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND),
                            iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID,
                            iID_QuyetToanNienDoID = chuongTrinh.iID_QuyetToanNienDoID,
                            iSum = 1,

                        };
                        listResult.Add(newObjHopDong);
                        listResult.AddRange(returnLoaiChi(chuongTrinh.iID_KHCTBQP_NhiemVuChiID, null, true, getListHopDong, listTitle.Where(x => x.iID_DuAnID == null && x.iID_HopDongID != null && x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).ToList()));
                        //
                    }
                  
                    var obj = listResult.FirstOrDefault(x => x.sLevel == "0" && x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID);
                    if (obj != null)
                    {
                        var getThisList = listResult.Where(x => x.iSum == 1 && x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).ToList();

                        obj.fDeNghiQTNamNay_USD = getThisList.Sum(x => x.fDeNghiQTNamNay_USD);
                        obj.fDeNghiQTNamNay_VND = getThisList.Sum(x => x.fDeNghiQTNamNay_VND);
                        obj.fDeNghiChuyenNamSau_USD = getThisList.Sum(x => x.fDeNghiChuyenNamSau_USD);
                        obj.fDeNghiChuyenNamSau_VND = getThisList.Sum(x => x.fDeNghiChuyenNamSau_VND);
                        obj.fThuaNopNSNN_USD = getThisList.Sum(x => x.fThuaNopNSNN_USD);
                        obj.fThuaNopNSNN_VND = getThisList.Sum(x => x.fThuaNopNSNN_VND);
                        obj.fThuaThieuKinhPhiTrongNam_USD = getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD);
                        obj.fThuaThieuKinhPhiTrongNam_VND = getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND);

                        getThisList.ForEach(x =>
                        {
                            var findTiGia = listTiGiaTienTe.Find(y => y.iID_QuyetToanNienDoID == x.iID_QuyetToanNienDoID);
                            if (findTiGia.sMaTienTeGoc.ToUpper() == "USD")
                            {
                                x.fQTKinhPhiDuocCap_NamNay_USD = x.fQTKinhPhiDuocCap_NamNay_USD == null || x.fQTKinhPhiDuocCap_NamNay_USD == 0 ? x.fQTKinhPhiDuocCap_NamNay_VND / findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamNay_USD;
                                x.fQTKinhPhiDuocCap_NamNay_VND = x.fQTKinhPhiDuocCap_NamNay_VND == null || x.fQTKinhPhiDuocCap_NamNay_VND == 0 ? x.fQTKinhPhiDuocCap_NamNay_USD * findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_TongSo_USD = x.fQTKinhPhiDuocCap_TongSo_USD == null || x.fQTKinhPhiDuocCap_TongSo_USD == 0 ? x.fQTKinhPhiDuocCap_TongSo_VND / findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_TongSo_USD;
                                x.fQTKinhPhiDuocCap_TongSo_VND = x.fQTKinhPhiDuocCap_TongSo_VND == null || x.fQTKinhPhiDuocCap_TongSo_VND == 0 ? x.fQTKinhPhiDuocCap_TongSo_USD * findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND / findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD * findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND;
                            }
                            else
                            {
                                x.fQTKinhPhiDuocCap_NamNay_USD = x.fQTKinhPhiDuocCap_NamNay_USD == null || x.fQTKinhPhiDuocCap_NamNay_USD == 0 ? x.fQTKinhPhiDuocCap_NamNay_VND * findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamNay_USD;
                                x.fQTKinhPhiDuocCap_NamNay_VND = x.fQTKinhPhiDuocCap_NamNay_VND == null || x.fQTKinhPhiDuocCap_NamNay_VND == 0 ? x.fQTKinhPhiDuocCap_NamNay_USD / findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_TongSo_USD = x.fQTKinhPhiDuocCap_TongSo_USD == null || x.fQTKinhPhiDuocCap_TongSo_USD == 0 ? x.fQTKinhPhiDuocCap_TongSo_VND * findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_TongSo_USD;
                                x.fQTKinhPhiDuocCap_TongSo_VND = x.fQTKinhPhiDuocCap_TongSo_VND == null || x.fQTKinhPhiDuocCap_TongSo_VND == 0 ? x.fQTKinhPhiDuocCap_TongSo_USD / findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND * findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD / findTiGia.fDonViTiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND;
                            }
                        });

                        obj.fQTKinhPhiDuocCap_NamNay_USD = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD);
                        obj.fQTKinhPhiDuocCap_NamNay_VND = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND);
                        obj.fQTKinhPhiDuocCap_TongSo_USD = getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD);
                        obj.fQTKinhPhiDuocCap_TongSo_VND = getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND);
                        obj.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD);
                        obj.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND);
                        obj.fKeHoachChuaGiaiNgan_USD = obj.fKeHoach_BQP_USD - obj.fQTKinhPhiDuocCap_TongSo_USD;

                    }
                }
            }

            return listResult;
        }
        public List<NH_QT_QuyetToanNienDo_ChiTietData> returnLoaiChi(Guid? idChuongTrinh, Guid? idDuAn, bool isDuAn, List<NH_QT_QuyetToanNienDo_ChiTietData> list, List<NH_QT_QuyetToanNienDo_ChiTietData> listTittle)
        {
            List<NH_QT_QuyetToanNienDo_ChiTietData> returnData = new List<NH_QT_QuyetToanNienDo_ChiTietData>();
            var listLoaiChiPhi = list.Select(x => new { x.iLoaiNoiDungChi }).Distinct().OrderBy(x => x.iLoaiNoiDungChi)
                  .ToList();
            var countLoaiChiPhi = 0;
            foreach (var loaiChiPhi in listLoaiChiPhi)
            {
                var dataSumLoaiChi =
                    listTittle.Any()
                    ? listTittle.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi)
                    : list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi && x.iID_ParentID == null).ToList();

                countLoaiChiPhi++;
                var newObjLoaiChiPhi = new NH_QT_QuyetToanNienDo_ChiTietData()
                {
                    STT = countLoaiChiPhi.ToString(),
                    sTenNoiDungChi = loaiChiPhi.iLoaiNoiDungChi == 1 ? "Chi ngoại tệ" : "Chi trong nước",
                    bIsTittle = true,
                    fQTKinhPhiDuocCap_TongSo_USD = dataSumLoaiChi.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD),
                    fQTKinhPhiDuocCap_TongSo_VND = dataSumLoaiChi.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND),
                    fQTKinhPhiDuocCap_NamNay_USD = dataSumLoaiChi.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD),
                    fQTKinhPhiDuocCap_NamNay_VND = dataSumLoaiChi.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND),
                    fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = dataSumLoaiChi.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD),
                    fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = dataSumLoaiChi.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND),
                    fDeNghiQTNamNay_USD = dataSumLoaiChi.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fDeNghiQTNamNay_USD),
                    fDeNghiQTNamNay_VND = dataSumLoaiChi.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fDeNghiQTNamNay_VND),
                    fDeNghiChuyenNamSau_USD = dataSumLoaiChi.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fDeNghiChuyenNamSau_USD),
                    fDeNghiChuyenNamSau_VND = dataSumLoaiChi.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fDeNghiChuyenNamSau_VND),
                    fThuaThieuKinhPhiTrongNam_USD = dataSumLoaiChi.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD),
                    fThuaThieuKinhPhiTrongNam_VND = dataSumLoaiChi.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND),
                };
                returnData.Add(newObjLoaiChiPhi);

                if (isDuAn)
                {
                    var listNameHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Select(x => new { x.sTenHopDong, x.iID_HopDongID, x.fHopDong_VND_HopDong, x.fHopDong_USD_HopDong }).Distinct()
                    .ToList();
                    var countHopDong = 0;
                    foreach (var nameHopDong in listNameHopDong)
                    {
                        countHopDong++;
                        var newObjHopDongDuAn = new NH_QT_QuyetToanNienDo_ChiTietData();
                        var findTittle = listTittle.Find(x => x.iID_HopDongID == nameHopDong.iID_HopDongID && x.iID_KHCTBQP_NhiemVuChiID == idChuongTrinh && x.iID_DuAnID == idDuAn && x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi);
                        var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi && x.iID_HopDongID == nameHopDong.iID_HopDongID).ToList();


                        newObjHopDongDuAn.MapFrom(findTittle);
                        newObjHopDongDuAn.fDeNghiQTNamNay_USD = findTittle.fDeNghiQTNamNay_USD != null ? findTittle.fDeNghiQTNamNay_USD : listHopDong.Sum(x => x.fDeNghiQTNamNay_USD);
                        newObjHopDongDuAn.fDeNghiQTNamNay_VND = findTittle.fDeNghiQTNamNay_VND != null ? findTittle.fDeNghiQTNamNay_VND : listHopDong.Sum(x => x.fDeNghiQTNamNay_VND);
                        newObjHopDongDuAn.fDeNghiChuyenNamSau_USD = findTittle.fDeNghiChuyenNamSau_USD != null ? findTittle.fDeNghiChuyenNamSau_USD : listHopDong.Sum(x => x.fDeNghiChuyenNamSau_USD);
                        newObjHopDongDuAn.fDeNghiChuyenNamSau_VND = findTittle.fDeNghiChuyenNamSau_VND != null ? findTittle.fDeNghiChuyenNamSau_VND : listHopDong.Sum(x => x.fDeNghiChuyenNamSau_VND);
                        newObjHopDongDuAn.fThuaThieuKinhPhiTrongNam_USD = findTittle.fThuaThieuKinhPhiTrongNam_USD != null ? findTittle.fThuaThieuKinhPhiTrongNam_USD : listHopDong.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD);
                        newObjHopDongDuAn.fThuaThieuKinhPhiTrongNam_VND = findTittle.fThuaThieuKinhPhiTrongNam_VND != null ? findTittle.fThuaThieuKinhPhiTrongNam_VND : listHopDong.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND);


                        newObjHopDongDuAn.STT = countLoaiChiPhi.ToString() + "." + countHopDong.ToString();
                        newObjHopDongDuAn.sTenNoiDungChi = nameHopDong.sTenHopDong;
                        newObjHopDongDuAn.fHopDong_VND = nameHopDong.fHopDong_VND_HopDong;
                        newObjHopDongDuAn.fHopDong_USD = nameHopDong.fHopDong_USD_HopDong;
                        newObjHopDongDuAn.bIsData = true;
                        newObjHopDongDuAn.bIsTittle = true;
                        newObjHopDongDuAn.sLevel = "2";
                        newObjHopDongDuAn.iID_ParentID = nameHopDong.iID_HopDongID;
                        newObjHopDongDuAn.iID_KHCTBQP_NhiemVuChiID = idChuongTrinh;
                        newObjHopDongDuAn.iID_HopDongID = nameHopDong.iID_HopDongID;
                        newObjHopDongDuAn.iID_DuAnID = idDuAn;

                        newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD = listHopDong.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND = listHopDong.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_USD = listHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_VND = listHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = listHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = listHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND);

                        newObjHopDongDuAn.iID_ThanhToan_ChiTietID = listHopDong.FirstOrDefault().iID_ThanhToan_ChiTietID;
                        returnData.Add(newObjHopDongDuAn);
                    }
                }
            }
            return returnData;
        }
        private string convertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }
        private string convertLaMa(decimal num)
        {
            string strRet = string.Empty;
            decimal _Number = num;
            Boolean _Flag = true;
            string[] ArrLama = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            int[] ArrNumber = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            int i = 0;
            while (_Flag)
            {
                while (_Number >= ArrNumber[i])
                {
                    _Number -= ArrNumber[i];
                    strRet += ArrLama[i];
                    if (_Number < 1)
                        _Flag = false;
                }
                i++;
            }
            return strRet;
        }
        public List<Dropdown_SelectValue> GetListNamKeHoach()
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
    }
}