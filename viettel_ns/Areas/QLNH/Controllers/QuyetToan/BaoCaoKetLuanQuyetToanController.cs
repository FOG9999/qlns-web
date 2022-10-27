using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Models.CPNH;
using Viettel.Services;
using DapperExtensions;
using VIETTEL.Helpers;
using System.Data;
using FlexCel.Core;
using FlexCel.Report;
using VIETTEL.Flexcel;
using FlexCel.XlsAdapter;
using System.Text;
using System.Globalization;
using DomainModel;
using Viettel.Models.QLNH.QuyetToan;

namespace VIETTEL.Areas.QLNH.Controllers.CPNH
{
    public class BaoCaoKetLuanQuyetToanController : FlexcelReportController
    {
        private readonly ICPNHService _cpnhService = CPNHService.Default;
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly IQLNguonNganSachService _nnsService = QLNguonNganSachService.Default;
        private const string sFilePathBaoCao1 = "/Report_ExcelFrom/QLNH/rpt_BaoCaoKetLuanQuyetToan.xlsx";
        private int _columnCountBC1 = 7;
        private const string sControlName = "ThucHienNganSach";

        // GET: QLVonDauTu/QLDMTyGia
        public ActionResult Index()
        {
            BaoCaoKetLuanQuyetToan_ModelPaging vm = new BaoCaoKetLuanQuyetToan_ModelPaging();
            List<NH_QT_QuyetToanDAHT_ChiTietData> list = _qlnhService.getListBaoCaoKetLuanQuyetToanModels(null, null, null).ToList();
            List<NH_QT_QuyetToanDAHT_ChiTietData> getlistGiaiDoan = list.Where(x => x.iGiaiDoanTu != 0 && x.iGiaiDoanDen != 0).OrderBy(x => x.iGiaiDoanTu).OrderBy(x => x.iGiaiDoanDen).ToList();
            List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan = getlistGiaiDoan
                    .GroupBy(x => new { x.iGiaiDoanTu, x.iGiaiDoanDen }).Select(x => x.First())
                    .Select(x => new ThucHienNganSach_GiaiDoan_Model
                    {
                        sGiaiDoan = "Giai đoạn " + x.iGiaiDoanTu + " - " + x.iGiaiDoanDen,
                        iGiaiDoanTu = x.iGiaiDoanTu,
                        iGiaiDoanDen = x.iGiaiDoanDen
                    }).ToList();
            List<NH_QT_QuyetToanDAHT_ChiTietData> listData = getList(list , lstGiaiDoan);
            vm.Items = listData;
            ViewBag.Count = vm.Items.Count();
            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả đơn vị--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            ViewBag.GiaiDoan = lstGiaiDoan;
            ViewBag.SoGiaiDoan = lstGiaiDoan.Count;
            return View(vm);
        }

        [HttpPost]
        public ActionResult BaoCaoKetLuanQuyetToanSearch( DateTime? dTuNgay, DateTime? dDenNgay, Guid? iDonvi)
        {
            BaoCaoKetLuanQuyetToan_ModelPaging vm = new BaoCaoKetLuanQuyetToan_ModelPaging();
            List<NH_QT_QuyetToanDAHT_ChiTietData> list = _qlnhService.getListBaoCaoKetLuanQuyetToanModels(dTuNgay, dDenNgay, iDonvi).ToList();
            List<NH_QT_QuyetToanDAHT_ChiTietData> getlistGiaiDoan = list.Where(x => x.iGiaiDoanTu != 0 && x.iGiaiDoanDen != 0).OrderBy(x => x.iGiaiDoanTu).OrderBy(x => x.iGiaiDoanDen).ToList();
            List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan = getlistGiaiDoan
                    .GroupBy(x => new { x.iGiaiDoanTu, x.iGiaiDoanDen }).Select(x => x.First())
                    .Select(x => new ThucHienNganSach_GiaiDoan_Model
                    {
                        sGiaiDoan = "Giai đoạn " + x.iGiaiDoanTu + " - " + x.iGiaiDoanDen,
                        iGiaiDoanTu = x.iGiaiDoanTu,
                        iGiaiDoanDen = x.iGiaiDoanDen
                    }).ToList();
            List<NH_QT_QuyetToanDAHT_ChiTietData> listData = getList(list, lstGiaiDoan);
            vm.Items = listData;
            ViewBag.Count = vm.Items.Count();

            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            ViewBag.GiaiDoan = lstGiaiDoan;
            ViewBag.SoGiaiDoan = lstGiaiDoan.Count;

            return PartialView("_list", vm);
        }

        public ActionResult ExportExcelBaoCao(string ext = "xlsx", int dvt = 1, int to = 1, DateTime? dTuNgay = null , DateTime? dDenNgay = null, Guid? iDonvi = null)
        {

            if (Request.QueryString["dTuNgay"].ToString() == "")
            {
                dTuNgay = null;
            }
            else
            {
                dTuNgay = Convert.ToDateTime(Request.QueryString["dTuNgay"]);
            }
            if (Request.QueryString["dDenNgay"].ToString() == "")
            {
                dDenNgay = null;
            }
            else
            {
                dDenNgay = Convert.ToDateTime(Request.QueryString["dDenNgay"]);
            }
            string fileName = string.Format("{0}.{1}", "BaoCaoKetLuanQuyetToan", ext);
            List<NH_QT_QuyetToanDAHT_ChiTietData> list = _qlnhService.getListBaoCaoKetLuanQuyetToanModels(dTuNgay, dDenNgay, iDonvi).ToList();
            ExcelFile xls = null;
            xls = TaoFileBaoCao1(dvt, to , list , dTuNgay , dDenNgay);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCao1(int dvt = 1, int to = 1 , List<NH_QT_QuyetToanDAHT_ChiTietData> list = null, DateTime? dTuNgay = null , DateTime? dDenNgay = null)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao1));
            FlexCelReport fr = new FlexCelReport();

            List<NH_QT_QuyetToanDAHT_ChiTietData> getlistGiaiDoan = list.Where(x => x.iGiaiDoanTu != 0 && x.iGiaiDoanDen != 0).OrderBy(x => x.iGiaiDoanTu).OrderBy(x => x.iGiaiDoanDen).ToList();
            List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan = getlistGiaiDoan
                    .GroupBy(x => new { x.iGiaiDoanTu, x.iGiaiDoanDen }).Select(x => x.First())
                    .Select(x => new ThucHienNganSach_GiaiDoan_Model
                    {
                        sGiaiDoan = "Giai đoạn " + x.iGiaiDoanTu + " - " + x.iGiaiDoanDen,
                        iGiaiDoanTu = x.iGiaiDoanTu,
                        iGiaiDoanDen = x.iGiaiDoanDen
                    }).ToList();
            int columnStart = _columnCountBC1 * (to - 1);
                        List<NH_QT_QuyetToanDAHT_ChiTietData> listData = getList(list , lstGiaiDoan);
            string sTuNgay = dTuNgay!= null ? dTuNgay.Value.ToString("dd/MM/yyyy") : string.Empty;
            string sDenNgay = dDenNgay != null ? dDenNgay.Value.ToString("dd/MM/yyyy") : string.Empty;

            fr.SetValue(new
            {
                dvt = dvt.ToStringDvt(),
                To = to,
                sTuNgay = sTuNgay,
                sDenNgay = sDenNgay
            });
            fr.AddTable<NH_QT_QuyetToanDAHT_ChiTietData>("dt", listData);
            fr.AddTable<ThucHienNganSach_GiaiDoan_Model>("lstGiaiDoan", lstGiaiDoan);
            fr.AddTable<ThucHienNganSach_GiaiDoan_Model>("lstGiaiDoan2", lstGiaiDoan);
            fr.AddTable<ThucHienNganSach_GiaiDoan_Model>("lstGiaiDoan3", lstGiaiDoan);
            fr.UseChuKy(Username)
            .UseChuKyForController(sControlName)
            .UseForm(this).Run(Result);

            var col1 = 3 + lstGiaiDoan.Count();
            var col2 = col1 + ((lstGiaiDoan.Count() + 1) * 2);
            Result.MergeCells(6, 3, 6, col1);
            Result.MergeCells(6, col1 + 1, 6, col1 + ((lstGiaiDoan.Count() + 1) * 2));
            Result.MergeCells(6, col2 + 1, 6, col2 + ((lstGiaiDoan.Count() + 1) * 2));
            //tạo border format
            var b = Result.GetDefaultFormat;
            b.Borders.Left.Style = TFlxBorderStyle.Thin;
            b.Borders.Right.Style = TFlxBorderStyle.Thin;
            b.Borders.Top.Style = TFlxBorderStyle.Thin;
            b.Borders.Bottom.Style = TFlxBorderStyle.Thin;
            var ApplyFormat = new TFlxApplyFormat();
            ApplyFormat.SetAllMembers(false);
            ApplyFormat.Borders.SetAllMembers(true);
            TCellAddress Cell = null;
            //tìm dòng cuối cùng của bảng
            Cell = Result.Find("Cộng", null, Cell, false, true, true, false);
            //set border cho bảng
            Result.SetCellFormat(6, 2, 8 + listData.Count(), 11 + lstGiaiDoan.Count() * 5, b, ApplyFormat, false);


            return Result;
        }
        private string convertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }
        private List<NH_QT_QuyetToanDAHT_ChiTietData> getList(List<NH_QT_QuyetToanDAHT_ChiTietData> list, List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan)
        {
            List<NH_QT_QuyetToanDAHT_ChiTietData> listData = new List<NH_QT_QuyetToanDAHT_ChiTietData>().ToList();
            int SttLoai = 0;
            int SttHopDong = 0;
            int SttDuAn = 0;
            int SttChuongTrinh = 0;
            Guid? idDuAn = null;
            Guid? idHopDong = null;
            Guid? idChuongTrinh = null;
            int? idLoai = null;
            int sttTong = 0;
            NH_QT_QuyetToanDAHT_ChiTietData DataTong = new NH_QT_QuyetToanDAHT_ChiTietData();
            DataTong.listDataTTCP = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
            DataTong.listDataKPDC = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
            DataTong.listDataQTDD = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
            DataTong.sTenNoiDungChi = "Tổng Cộng: ";
            if (lstGiaiDoan != null)
            {
                foreach (var giaiDoan in lstGiaiDoan)
                {
                    List<NH_QT_QuyetToanDAHT_ChiTietData> listDataChaGiaiDoan = list.Where(x => x.iNamBaoCaoTu == giaiDoan.iGiaiDoanTu && x.iNamBaoCaoDen == giaiDoan.iGiaiDoanDen).ToList();
                    DataTong.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fKeHoach_TTCP_USD) });
                    DataTong.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_VND) });
                    DataTong.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND) });
                }
            }
            DataTong.fHopDong_USD = list.Sum(x => x.fHopDong_USD);
            DataTong.fHopDong_VND = list.Sum(x => x.fHopDong_VND);

            DataTong.fKeHoach_TTCP_USD = list.Sum(x => x.fKeHoach_TTCP_USD);

            DataTong.fKinhPhiDuocCap_Tong_USD = list.Sum(x => x.fKinhPhiDuocCap_Tong_USD);
            DataTong.fKinhPhiDuocCap_Tong_VND = list.Sum(x => x.fKinhPhiDuocCap_Tong_VND);

            DataTong.fQuyetToanDuocDuyet_Tong_USD = list.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD);
            DataTong.fQuyetToanDuocDuyet_Tong_VND = list.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND);

            DataTong.fSoSanhKinhPhi_USD = list.Sum(x => x.fSoSanhKinhPhi_USD);
            DataTong.fSoSanhKinhPhi_VND = list.Sum(x => x.fSoSanhKinhPhi_VND);

            DataTong.fThuaTraNSNN_USD = list.Sum(x => x.fThuaTraNSNN_USD);
            DataTong.fThuaTraNSNN_VND = list.Sum(x => x.fThuaTraNSNN_VND);
            DataTong.isTitle = "font-bold";
            DataTong.bIsTittle = true;
            if (list != null || list.Count == 0)
            {
                foreach (var item in list)
                {
                    sttTong++;
                    if (lstGiaiDoan != null)
                    {
                        item.listDataTTCP = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        item.listDataKPDC = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        item.listDataQTDD = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        foreach (var giaiDoan in lstGiaiDoan)
                        {

                            if (item.iNamBaoCaoTu == giaiDoan.iGiaiDoanTu && item.iNamBaoCaoDen == giaiDoan.iGiaiDoanDen)
                            {

                                item.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = item.fKeHoach_TTCP_USD });
                                item.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = item.fKinhPhiDuocCap_Tong_USD, valueVND = item.fKinhPhiDuocCap_Tong_VND });
                                item.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = item.fQuyetToanDuocDuyet_Tong_USD, valueVND = item.fQuyetToanDuocDuyet_Tong_VND });
                            }
                            else
                            {
                                item.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                item.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                item.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                            }
                        }
                    }
                    if (item.iID_KHCTBQP_NhiemVuChiID != idChuongTrinh /*&& item.iID_KHCTBQP_NhiemVuChiID != Guid.Empty*/)
                    {
                        SttChuongTrinh++;
                        SttDuAn = 0;
                        SttLoai = 0;
                        SttDuAn = 0;
                        idDuAn = null;
                        idLoai = null;
                        idHopDong = null;
                        NH_QT_QuyetToanDAHT_ChiTietData DataCha = new NH_QT_QuyetToanDAHT_ChiTietData();

                        List<NH_QT_QuyetToanDAHT_ChiTietData> listDataCha = list.Where(x => x.iID_KHCTBQP_NhiemVuChiID == item.iID_KHCTBQP_NhiemVuChiID).ToList();
                        DataCha.fHopDong_USD = listDataCha.Sum(x => x.fHopDong_USD);
                        DataCha.fHopDong_VND = listDataCha.Sum(x => x.fHopDong_VND);

                        DataCha.fKeHoach_TTCP_USD = listDataCha.FirstOrDefault() != null ? listDataCha.FirstOrDefault().fKeHoach_TTCP_USD : 0;

                        DataCha.fKinhPhiDuocCap_Tong_USD = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_USD);
                        DataCha.fKinhPhiDuocCap_Tong_VND = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_VND);

                        DataCha.fQuyetToanDuocDuyet_Tong_USD = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD);
                        DataCha.fQuyetToanDuocDuyet_Tong_VND = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND);

                        DataCha.fSoSanhKinhPhi_USD = listDataCha.Sum(x => x.fSoSanhKinhPhi_USD);
                        DataCha.fSoSanhKinhPhi_VND = listDataCha.Sum(x => x.fSoSanhKinhPhi_VND);

                        DataCha.fThuaTraNSNN_USD = listDataCha.Sum(x => x.fThuaTraNSNN_USD);
                        DataCha.fThuaTraNSNN_VND = listDataCha.Sum(x => x.fThuaTraNSNN_VND);

                        if (item.iID_KHCTBQP_NhiemVuChiID != Guid.Empty)
                        {
                            DataCha.sTenNoiDungChi = item.sTenNhiemVuChi;
                        }
                        else
                        {
                            DataCha.sTenNoiDungChi = "Nội dung chi khác";
                        }
                        DataCha.STT = convertLetter(SttChuongTrinh) + ".";
                        DataCha.isTitle = "font-bold-red";
                        idChuongTrinh = item.iID_KHCTBQP_NhiemVuChiID;
                        DataCha.listDataTTCP = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataKPDC = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataQTDD = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.iNamBaoCaoDen = item.iNamBaoCaoDen;
                        DataCha.iNamBaoCaoTu = item.iNamBaoCaoTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<NH_QT_QuyetToanDAHT_ChiTietData> listDataChaGiaiDoan = listDataCha.Where(x => x.iNamBaoCaoTu == giaiDoan.iGiaiDoanTu && x.iNamBaoCaoDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.FirstOrDefault() != null ? listDataChaGiaiDoan.FirstOrDefault().fKeHoach_TTCP_USD : 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_VND) });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND) });
                                }
                                else
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                }

                            }
                        }

                        listData.Add(DataCha);
                    }
                    if (item.iID_DuAnID != idDuAn /*&& item.iID_DuAnID != Guid.Empty*/)
                    {
                        SttDuAn++;
                        SttLoai = 0;
                        SttHopDong = 0;
                        idLoai = null;
                        idHopDong = null;
                        NH_QT_QuyetToanDAHT_ChiTietData DataCha = new NH_QT_QuyetToanDAHT_ChiTietData();
                        List<NH_QT_QuyetToanDAHT_ChiTietData> listDataCha = list.Where(x => x.iID_DuAnID == item.iID_DuAnID && x.iID_KHCTBQP_NhiemVuChiID == item.iID_KHCTBQP_NhiemVuChiID).ToList();

                        DataCha.fHopDong_USD = listDataCha.Sum(x => x.fHopDong_USD);
                        DataCha.fHopDong_VND = listDataCha.Sum(x => x.fHopDong_VND);

                        DataCha.fKeHoach_TTCP_USD = 0;

                        DataCha.fKinhPhiDuocCap_Tong_USD = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_USD);
                        DataCha.fKinhPhiDuocCap_Tong_VND = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_VND);

                        DataCha.fQuyetToanDuocDuyet_Tong_USD = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD);
                        DataCha.fQuyetToanDuocDuyet_Tong_VND = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND);

                        DataCha.fSoSanhKinhPhi_USD = listDataCha.Sum(x => x.fSoSanhKinhPhi_USD);
                        DataCha.fSoSanhKinhPhi_VND = listDataCha.Sum(x => x.fSoSanhKinhPhi_VND);

                        DataCha.fThuaTraNSNN_USD = listDataCha.Sum(x => x.fThuaTraNSNN_USD);
                        DataCha.fThuaTraNSNN_VND = listDataCha.Sum(x => x.fThuaTraNSNN_VND);

                        if (item.iID_DuAnID != Guid.Empty)
                        {
                            DataCha.sTenNoiDungChi = item.sTenDuAn;
                        }
                        else if (item.iID_HopDongID != Guid.Empty)
                        {
                            DataCha.sTenNoiDungChi = "Chi hợp đồng";
                        }
                        else
                        {
                            DataCha.sTenNoiDungChi = "Chi khác";
                        }
                        DataCha.STT = _cpnhService.GetSTTLAMA(SttDuAn) + ".";
                        DataCha.isTitle = "font-bold";
                        idDuAn = item.iID_DuAnID;
                        DataCha.listDataTTCP = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataKPDC = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataQTDD = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.iNamBaoCaoDen = item.iNamBaoCaoDen;
                        DataCha.iNamBaoCaoTu = item.iNamBaoCaoTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<NH_QT_QuyetToanDAHT_ChiTietData> listDataChaGiaiDoan = listDataCha.Where(x => x.iNamBaoCaoTu == giaiDoan.iGiaiDoanTu && x.iNamBaoCaoDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_VND) });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND) });
                                }
                                else
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                }

                            }
                        }

                        listData.Add(DataCha);
                    }
                    if (item.iLoaiNoiDungChi != idLoai /*&& item.iLoaiNoiDungChi != 0*/)
                    {
                        SttLoai++;
                        SttHopDong = 0;
                        idHopDong = null;
                        NH_QT_QuyetToanDAHT_ChiTietData DataCha = new NH_QT_QuyetToanDAHT_ChiTietData();
                        List<NH_QT_QuyetToanDAHT_ChiTietData> listDataCha = list.Where(x => x.iLoaiNoiDungChi == item.iLoaiNoiDungChi && x.iID_DuAnID == item.iID_DuAnID && x.iID_KHCTBQP_NhiemVuChiID == item.iID_KHCTBQP_NhiemVuChiID).ToList();

                        DataCha.fHopDong_USD = listDataCha.Sum(x => x.fHopDong_USD);
                        DataCha.fHopDong_VND = listDataCha.Sum(x => x.fHopDong_VND);

                        DataCha.fKeHoach_TTCP_USD = 0;

                        DataCha.fKinhPhiDuocCap_Tong_USD = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_USD);
                        DataCha.fKinhPhiDuocCap_Tong_VND = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_VND);

                        DataCha.fQuyetToanDuocDuyet_Tong_USD = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD);
                        DataCha.fQuyetToanDuocDuyet_Tong_VND = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND);

                        DataCha.fSoSanhKinhPhi_USD = listDataCha.Sum(x => x.fSoSanhKinhPhi_USD);
                        DataCha.fSoSanhKinhPhi_VND = listDataCha.Sum(x => x.fSoSanhKinhPhi_VND);

                        DataCha.fThuaTraNSNN_USD = listDataCha.Sum(x => x.fThuaTraNSNN_USD);
                        DataCha.fThuaTraNSNN_VND = listDataCha.Sum(x => x.fThuaTraNSNN_VND);

                        if (item.iLoaiNoiDungChi == 1)
                        {
                            DataCha.sTenNoiDungChi = "Chi ngoại tệ";
                        }
                        else if (item.iLoaiNoiDungChi == 2)
                        {
                            DataCha.sTenNoiDungChi = "Chi trong nước";
                        }
                        else
                        {
                            DataCha.sTenNoiDungChi = "Chi khác";
                        }
                        DataCha.STT = SttLoai.ToString() + ".";
                        DataCha.isTitle = "font-bold";
                        idLoai = item.iLoaiNoiDungChi;
                        DataCha.listDataTTCP = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataKPDC = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataQTDD = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.iNamBaoCaoDen = item.iNamBaoCaoDen;
                        DataCha.iNamBaoCaoTu = item.iNamBaoCaoTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<NH_QT_QuyetToanDAHT_ChiTietData> listDataChaGiaiDoan = listDataCha.Where(x => x.iNamBaoCaoTu == giaiDoan.iGiaiDoanTu && x.iNamBaoCaoDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_VND) });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND) });
                                }
                                else
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                }

                            }
                        }
                    }
                    if (item.iID_HopDongID != idHopDong && item.iID_HopDongID != Guid.Empty)
                    {
                        SttHopDong++;
                        
                        NH_QT_QuyetToanDAHT_ChiTietData DataCha = new NH_QT_QuyetToanDAHT_ChiTietData();
                        List<NH_QT_QuyetToanDAHT_ChiTietData> listDataCha = list.Where(x => x.iID_HopDongID == item.iID_HopDongID && x.iLoaiNoiDungChi == item.iLoaiNoiDungChi && x.iID_DuAnID == item.iID_DuAnID && x.iID_KHCTBQP_NhiemVuChiID == item.iID_KHCTBQP_NhiemVuChiID).ToList();

                        DataCha.fHopDong_USD = listDataCha.Sum(x => x.fHopDong_USD);
                        DataCha.fHopDong_VND = listDataCha.Sum(x => x.fHopDong_VND);

                        DataCha.fKeHoach_TTCP_USD = 0;

                        DataCha.fKinhPhiDuocCap_Tong_USD = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_USD);
                        DataCha.fKinhPhiDuocCap_Tong_VND = listDataCha.Sum(x => x.fKinhPhiDuocCap_Tong_VND);

                        DataCha.fQuyetToanDuocDuyet_Tong_USD = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD);
                        DataCha.fQuyetToanDuocDuyet_Tong_VND = listDataCha.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND);

                        DataCha.fSoSanhKinhPhi_USD = listDataCha.Sum(x => x.fSoSanhKinhPhi_USD);
                        DataCha.fSoSanhKinhPhi_VND = listDataCha.Sum(x => x.fSoSanhKinhPhi_VND);

                        DataCha.fThuaTraNSNN_USD = listDataCha.Sum(x => x.fThuaTraNSNN_USD);
                        DataCha.fThuaTraNSNN_VND = listDataCha.Sum(x => x.fThuaTraNSNN_VND);

                        DataCha.sTenNoiDungChi = item.sTenHopDong;
                        DataCha.STT = SttLoai.ToString() + "." + SttHopDong.ToString() + ".";
                        idHopDong = item.iID_HopDongID;
                        DataCha.listDataTTCP = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataKPDC = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.listDataQTDD = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                        DataCha.iNamBaoCaoDen = item.iNamBaoCaoDen;
                        DataCha.iNamBaoCaoTu = item.iNamBaoCaoTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<NH_QT_QuyetToanDAHT_ChiTietData> listDataChaGiaiDoan = listDataCha.Where(x => x.iNamBaoCaoTu == giaiDoan.iGiaiDoanTu && x.iNamBaoCaoDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fKinhPhiDuocCap_Tong_VND) });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND) });
                                }
                                else
                                {
                                    DataCha.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0 });
                                    DataCha.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                    DataCha.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = 0, valueVND = 0 });
                                }

                            }
                        }
                        listData.Add(DataCha);
                    }


                    item.fKeHoach_TTCP_USD = 0;
                    listData.Add(item);
                }
                
                listData.Add(DataTong);
            }

            return listData;
        } 
        public List<Dropdown_ByYear_ThucHienNganSach> GetListNamKeHoach()
        {
            List<Dropdown_ByYear_ThucHienNganSach> listNam = new List<Dropdown_ByYear_ThucHienNganSach>();
            int namHienTai = DateTime.Now.Year + 1;
            for (int i = 20; i > 0; i--)
            {
                namHienTai -= 1;
                Dropdown_ByYear_ThucHienNganSach namKeHoachOpt = new Dropdown_ByYear_ThucHienNganSach()
                {
                    Value = namHienTai,
                    Text = "Năm " + namHienTai.ToString()
                };
                listNam.Add(namKeHoachOpt);
            }
            return listNam;
        }
    }
}