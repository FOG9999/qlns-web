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

namespace VIETTEL.Areas.QLNH.Controllers.CPNH
{
    public class KinhPhiDuocCapTheoDonViController : FlexcelReportController
    {
        private readonly ICPNHService _cpnhService = CPNHService.Default;
        private readonly IQLNguonNganSachService _nnsService = QLNguonNganSachService.Default;
        private const string sFilePathBaoCao1 = "/Report_ExcelFrom/QLNH/rpt_KinhPhiDuocCapTheoDonVi.xlsx";
        private int _columnCountBC1 = 7;
        private const string sControlName = "KinhPhiDuocCapTheoDonVi";

        // GET: QLVonDauTu/QLDMTyGia
        public ActionResult Index()
        {
            CPNHThucHienNganSach_ModelPaging vm = new CPNHThucHienNganSach_ModelPaging();

            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();

            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            List<Dropdown_ByYear_ThucHienNganSach> lstNam = GetListNamKeHoach().ToList();
            ViewBag.ListYear = lstNam;
            ViewBag.YearNow = DateTime.Now.Year;

            List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                };
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            List<CPNHThucHienNganSach_Model> list = _cpnhService.getListKinhPhiDuocCapTheoDonViModels(1, DateTime.Now.Year, lstDonViQuanLy[0].iID_Ma).ToList();
            List<CPNHThucHienNganSach_Model> listData = getList(list);
            vm.Items = listData;


            return View(vm);
        }

        [HttpPost]
        public ActionResult KinhPhiDuocCapTheoDonViSearch(int? iQuyList, int? iNam, Guid? iDonvi)
        {
            CPNHThucHienNganSach_ModelPaging vm = new CPNHThucHienNganSach_ModelPaging();
            List<CPNHThucHienNganSach_Model> list = _cpnhService.getListKinhPhiDuocCapTheoDonViModels(iQuyList, iNam, iDonvi).ToList();
            List<CPNHThucHienNganSach_Model> listData = getList(list);
            List<Dropdown_ByYear_ThucHienNganSach> lstNam = GetListNamKeHoach().ToList();
            ViewBag.ListYear = lstNam;
            ViewBag.YearNow = DateTime.Now.Year;

            List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                };
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            vm.Items = listData;

            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            return PartialView("_list", vm);
        }

        public ActionResult ExportExcelBaoCao(string ext = "xls", int dvt = 1, int to = 1, int? iQuyList = null, int? iNam = null, Guid? iDonvi = null)
        {
            //if (Request.QueryString["dTuNgay"].ToString() + "" == "")
            //{
            //    dTuNgay = null;
            //}
            //else
            //{
            //    dTuNgay = Convert.ToDateTime(Request.QueryString["dTuNgay"]);
            //}
            //if (Request.QueryString["dDenNgay"].ToString() + "" == "")
            //{
            //    dDenNgay = null;
            //}
            //else
            //{
            //    dDenNgay = Convert.ToDateTime(Request.QueryString["dDenNgay"]);
            //}
            NS_DonVi lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList().Where(x => x.iID_Ma == iDonvi).FirstOrDefault();
            var DonVi = lstDonViQuanLy != null ? lstDonViQuanLy.sTen + " - " + lstDonViQuanLy.sMoTa : "";
            string fileName = string.Format("{0}.{1}", "BaoCaoKinhPhiDuocCapTheoDonVi", ext);
            List<CPNHThucHienNganSach_Model> list = _cpnhService.getListKinhPhiDuocCapTheoDonViModels(iQuyList, iNam, iDonvi).ToList();
            ExcelFile xls = null;
            xls = TaoFileBaoCao1(dvt, to, list, iQuyList, iNam, DonVi);
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCao1(int dvt = 1, int to = 1 , List<CPNHThucHienNganSach_Model> list = null , int? iQuyList = null, int? iNam = null, string DonVi = null)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao1));
            FlexCelReport fr = new FlexCelReport();
            //var TuNgay = dTuNgay != null ? dTuNgay.Value.ToString("dd/MM/yyyy") : "";
            //var DenNgay = dTuNgay != null ? dDenNgay.Value.ToString("dd/MM/yyyy") : "";
            int columnStart = _columnCountBC1 * (to - 1);
            List<CPNHThucHienNganSach_Model> listData = getList(list);
            fr.AddTable<CPNHThucHienNganSach_Model>("dt", listData);
            fr.SetValue(new
            {
                dvt = dvt.ToStringDvt(),
                To = to,
                iQuy = 0,
                iNam = 0,
                DonVi = DonVi,
                TuNgay = iQuyList,
                DenNgay = iNam
            });
            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this).Run(Result);


            return Result;
        }
        private string convertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }
        private List<CPNHThucHienNganSach_Model> getList(List<CPNHThucHienNganSach_Model> list)
        {
            List<CPNHThucHienNganSach_Model> listData = new List<CPNHThucHienNganSach_Model>().ToList();
            int SttLoai = 0;
            int SttHopDong = 0;
            int SttDuAn = 0;
            int SttChuongTrinh = 0;
            Guid? idDuAn = null;
            Guid? idHopDong = null;
            Guid? idChuongTrinh = null;
            int? idLoai = null;
            int sttTong = 0;
            List<CPNHThucHienNganSach_Model> listTong = list;
            CPNHThucHienNganSach_Model DataTong = new CPNHThucHienNganSach_Model();
            DataTong.NCVTTCP = listTong.GroupBy(x => x.IDNhiemVuChi).Select(x => x.First()).Sum(x => x.NCVTTCP);
            DataTong.NhiemVuChi = listTong.GroupBy(x => x.IDNhiemVuChi).Select(x => x.First()).Sum(x => x.NhiemVuChi);
            DataTong.KinhPhiUSD = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiUSD);
            DataTong.KinhPhiVND = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiVND);
            DataTong.KinhPhiToYUSD = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYUSD);
            DataTong.KinhPhiToYVND = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYVND);
            DataTong.KinhPhiDaChiUSD = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiUSD);
            DataTong.KinhPhiDaChiVND = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiVND);
            DataTong.KinhPhiDaChiToYUSD = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYUSD);
            DataTong.KinhPhiDaChiToYVND = listTong.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYVND);
            DataTong.TongKinhPhiUSD = DataTong.KinhPhiUSD + DataTong.KinhPhiToYUSD;
            DataTong.TongKinhPhiVND = DataTong.KinhPhiVND + DataTong.KinhPhiToYVND;

            DataTong.TongKinhPhiDaChiUSD = DataTong.KinhPhiDaChiUSD + DataTong.KinhPhiDaChiToYUSD;
            DataTong.TongKinhPhiDaChiVND = DataTong.KinhPhiDaChiVND + DataTong.KinhPhiDaChiToYVND;

            DataTong.KinhPhiDuocCapChuaChiUSD = DataTong.TongKinhPhiUSD - DataTong.TongKinhPhiDaChiUSD;
            DataTong.KinhPhiDuocCapChuaChiVND = DataTong.TongKinhPhiVND - DataTong.TongKinhPhiDaChiVND;
            DataTong.QuyGiaiNganTheoQuy = DataTong.NhiemVuChi - DataTong.TongKinhPhiUSD;
            DataTong.sTenNoiDungChi = "Tổng Cộng: ";
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.TongKinhPhiUSD = item.KinhPhiUSD + item.KinhPhiToYUSD;
                    item.TongKinhPhiVND = item.KinhPhiVND + item.KinhPhiToYVND;

                    item.TongKinhPhiDaChiUSD = item.KinhPhiDaChiUSD + item.KinhPhiDaChiToYUSD;
                    item.TongKinhPhiDaChiVND = item.KinhPhiDaChiVND + item.KinhPhiDaChiToYVND;

                    item.KinhPhiDuocCapChuaChiUSD = item.TongKinhPhiUSD - item.TongKinhPhiDaChiUSD;
                    item.KinhPhiDuocCapChuaChiVND = item.TongKinhPhiVND - item.TongKinhPhiDaChiVND;
                    item.QuyGiaiNganTheoQuy = item.NhiemVuChi - item.TongKinhPhiUSD;
                    sttTong++;
                    if (item.IDNhiemVuChi != idChuongTrinh/* && item.IDNhiemVuChi != Guid.Empty*/)
                    {
                        SttChuongTrinh++;
                        SttDuAn = 0;
                        SttLoai = 0;
                        SttDuAn = 0;
                        idDuAn = null;
                        idLoai = null;
                        idHopDong = null;
                        CPNHThucHienNganSach_Model DataCha = new CPNHThucHienNganSach_Model();

                        List<CPNHThucHienNganSach_Model> listDataCha = list.Where(x => x.IDNhiemVuChi == item.IDNhiemVuChi).ToList();
                        //DataCha.HopDongUSD = listDataCha.Sum(x => x.HopDongUSD);
                        //DataCha.HopDongVND = listDataCha.Sum(x => x.HopDongVND);

                        DataCha.KinhPhiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiUSD);
                        DataCha.KinhPhiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiVND);
                        DataCha.KinhPhiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYUSD);
                        DataCha.KinhPhiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYVND);
                        DataCha.KinhPhiDaChiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiUSD);
                        DataCha.KinhPhiDaChiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiVND);
                        DataCha.KinhPhiDaChiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYUSD);
                        DataCha.KinhPhiDaChiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYVND);

                        DataCha.fLuyKeKinhPhiDuocCap_USD = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_USD);
                        DataCha.fLuyKeKinhPhiDuocCap_VND = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_VND);
                        DataCha.fDeNghiQTNamNay_USD = listDataCha.Sum(x => x.fDeNghiQTNamNay_USD);
                        DataCha.fDeNghiQTNamNay_VND = listDataCha.Sum(x => x.fDeNghiQTNamNay_VND);

                        //DataCha.NCVTTCP = listDataCha.Sum(x => x.NCVTTCP);
                        //DataCha.NhiemVuChi = listDataCha.Sum(x => x.NhiemVuChi);
                        DataCha.NCVTTCP = item.NCVTTCP;
                        DataCha.NhiemVuChi = item.NhiemVuChi;

                        DataCha.TongKinhPhiUSD = DataCha.KinhPhiUSD + DataCha.KinhPhiToYUSD;
                        DataCha.TongKinhPhiVND = DataCha.KinhPhiVND + DataCha.KinhPhiToYVND;

                        DataCha.TongKinhPhiDaChiUSD = DataCha.KinhPhiDaChiUSD + DataCha.KinhPhiDaChiToYUSD;
                        DataCha.TongKinhPhiDaChiVND = DataCha.KinhPhiDaChiVND + DataCha.KinhPhiDaChiToYVND;

                        DataCha.KinhPhiDuocCapChuaChiUSD = DataCha.TongKinhPhiUSD - DataCha.TongKinhPhiDaChiUSD;
                        DataCha.KinhPhiDuocCapChuaChiVND = DataCha.TongKinhPhiVND - DataCha.TongKinhPhiDaChiVND;
                        DataCha.QuyGiaiNganTheoQuy = DataCha.NhiemVuChi - DataCha.TongKinhPhiUSD;

                        DataCha.KinhPhiChuaQuyetToanUSD = DataCha.fLuyKeKinhPhiDuocCap_USD - DataCha.fDeNghiQTNamNay_USD;
                        DataCha.KinhPhiChuaQuyetToanVND = DataCha.fLuyKeKinhPhiDuocCap_VND - DataCha.fDeNghiQTNamNay_VND;
                        DataCha.KeHoachGiaiNgan = DataCha.NCVTTCP - DataCha.fLuyKeKinhPhiDuocCap_USD;

                        if (item.IDNhiemVuChi != Guid.Empty)
                        {
                            DataCha.sTenNoiDungChi = item.sTenNhiemVuChi;
                        }
                        else
                        {
                            DataCha.sTenNoiDungChi = "Nội dung chi khác";
                        }
                        DataCha.depth = convertLetter(SttChuongTrinh) + ".";
                        DataCha.isTitle = "font-bold-red";
                        idChuongTrinh = item.IDNhiemVuChi;
                        listData.Add(DataCha);
                    }
                    if (item.IDDuAn != idDuAn/* && item.IDDuAn != Guid.Empty*/)
                    {
                        SttDuAn++;
                        SttLoai = 0;
                        SttHopDong = 0;
                        idLoai = null;
                        idHopDong = null;
                        CPNHThucHienNganSach_Model DataCha = new CPNHThucHienNganSach_Model();
                        List<CPNHThucHienNganSach_Model> listDataCha = list.Where(x => x.IDDuAn == item.IDDuAn && x.IDNhiemVuChi == item.IDNhiemVuChi).ToList();

                        DataCha.HopDongUSD = listDataCha.Sum(x => x.HopDongUSD);
                        DataCha.HopDongVND = listDataCha.Sum(x => x.HopDongVND);

                        DataCha.KinhPhiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiUSD);
                        DataCha.KinhPhiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiVND);
                        DataCha.KinhPhiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYUSD);
                        DataCha.KinhPhiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYVND);
                        DataCha.KinhPhiDaChiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiUSD);
                        DataCha.KinhPhiDaChiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiVND);
                        DataCha.KinhPhiDaChiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYUSD);
                        DataCha.KinhPhiDaChiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYVND);

                        DataCha.fLuyKeKinhPhiDuocCap_USD = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_USD);
                        DataCha.fLuyKeKinhPhiDuocCap_VND = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_VND);
                        DataCha.fDeNghiQTNamNay_USD = listDataCha.Sum(x => x.fDeNghiQTNamNay_USD);
                        DataCha.fDeNghiQTNamNay_VND = listDataCha.Sum(x => x.fDeNghiQTNamNay_VND);

                        //DataCha.NCVTTCP = listDataCha.Sum(x => x.NCVTTCP);
                        //DataCha.NhiemVuChi = listDataCha.Sum(x => x.NhiemVuChi);

                        DataCha.TongKinhPhiUSD = DataCha.KinhPhiUSD + DataCha.KinhPhiToYUSD;
                        DataCha.TongKinhPhiVND = DataCha.KinhPhiVND + DataCha.KinhPhiToYVND;

                        DataCha.TongKinhPhiDaChiUSD = DataCha.KinhPhiDaChiUSD + DataCha.KinhPhiDaChiToYUSD;
                        DataCha.TongKinhPhiDaChiVND = DataCha.KinhPhiDaChiVND + DataCha.KinhPhiDaChiToYVND;

                        //DataCha.KinhPhiDuocCapChuaChiUSD = DataCha.TongKinhPhiUSD - DataCha.TongKinhPhiDaChiUSD;
                        //DataCha.KinhPhiDuocCapChuaChiVND = DataCha.TongKinhPhiVND - DataCha.TongKinhPhiDaChiVND;
                        //DataCha.QuyGiaiNganTheoQuy = DataCha.NhiemVuChi - DataCha.TongKinhPhiUSD;

                        DataCha.KinhPhiChuaQuyetToanUSD = DataCha.fLuyKeKinhPhiDuocCap_USD - DataCha.fDeNghiQTNamNay_USD;
                        DataCha.KinhPhiChuaQuyetToanVND = DataCha.fLuyKeKinhPhiDuocCap_VND - DataCha.fDeNghiQTNamNay_VND;
                        //DataCha.KeHoachGiaiNgan = DataCha.NCVTTCP - DataCha.fLuyKeKinhPhiDuocCap_USD;

                        if (item.IDDuAn != Guid.Empty)
                        {
                            DataCha.sTenNoiDungChi = item.sTenDuAn;
                        }
                        else if (item.IDHopDong != Guid.Empty)
                        {
                            DataCha.sTenNoiDungChi = "Chi hợp đồng";
                        }
                        else
                        {
                            DataCha.sTenNoiDungChi = "Chi khác";
                        }
                        DataCha.isTitle = "font-bold";
                        DataCha.isDuAn = true;
                        DataCha.depth = _cpnhService.GetSTTLAMA(SttDuAn) + ".";
                        idDuAn = item.IDDuAn;
                        listData.Add(DataCha);
                    }
                    if (item.iLoaiNoiDungChi != idLoai /*&& item.iLoaiNoiDungChi != 0*/)
                    {
                        SttLoai++;
                        SttHopDong = 0;
                        idHopDong = null;
                        CPNHThucHienNganSach_Model DataCha = new CPNHThucHienNganSach_Model();
                        List<CPNHThucHienNganSach_Model> listDataCha = list.Where(x => x.iLoaiNoiDungChi == item.iLoaiNoiDungChi && x.IDDuAn == item.IDDuAn && x.IDNhiemVuChi == item.IDNhiemVuChi).ToList();

                        //DataCha.HopDongUSD = listDataCha.Sum(x => x.HopDongUSD);
                        //DataCha.HopDongVND = listDataCha.Sum(x => x.HopDongVND);

                        DataCha.KinhPhiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiUSD);
                        DataCha.KinhPhiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiVND);
                        DataCha.KinhPhiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYUSD);
                        DataCha.KinhPhiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYVND);
                        DataCha.KinhPhiDaChiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiUSD);
                        DataCha.KinhPhiDaChiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiVND);
                        DataCha.KinhPhiDaChiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYUSD);
                        DataCha.KinhPhiDaChiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYVND);

                        DataCha.fLuyKeKinhPhiDuocCap_USD = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_USD);
                        DataCha.fLuyKeKinhPhiDuocCap_VND = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_VND);
                        DataCha.fDeNghiQTNamNay_USD = listDataCha.Sum(x => x.fDeNghiQTNamNay_USD);
                        DataCha.fDeNghiQTNamNay_VND = listDataCha.Sum(x => x.fDeNghiQTNamNay_VND);

                        //DataCha.NCVTTCP = listDataCha.Sum(x => x.NCVTTCP);
                        //DataCha.NhiemVuChi = listDataCha.Sum(x => x.NhiemVuChi);

                        DataCha.TongKinhPhiUSD = DataCha.KinhPhiUSD + DataCha.KinhPhiToYUSD;
                        DataCha.TongKinhPhiVND = DataCha.KinhPhiVND + DataCha.KinhPhiToYVND;

                        DataCha.TongKinhPhiDaChiUSD = DataCha.KinhPhiDaChiUSD + DataCha.KinhPhiDaChiToYUSD;
                        DataCha.TongKinhPhiDaChiVND = DataCha.KinhPhiDaChiVND + DataCha.KinhPhiDaChiToYVND;

                        //DataCha.KinhPhiDuocCapChuaChiUSD = DataCha.TongKinhPhiUSD - DataCha.TongKinhPhiDaChiUSD;
                        //DataCha.KinhPhiDuocCapChuaChiVND = DataCha.TongKinhPhiVND - DataCha.TongKinhPhiDaChiVND;
                        //DataCha.QuyGiaiNganTheoQuy = DataCha.NhiemVuChi - DataCha.TongKinhPhiUSD;

                        DataCha.KinhPhiChuaQuyetToanUSD = DataCha.fLuyKeKinhPhiDuocCap_USD - DataCha.fDeNghiQTNamNay_USD;
                        DataCha.KinhPhiChuaQuyetToanVND = DataCha.fLuyKeKinhPhiDuocCap_VND - DataCha.fDeNghiQTNamNay_VND;
                        //DataCha.KeHoachGiaiNgan = DataCha.NCVTTCP - DataCha.fLuyKeKinhPhiDuocCap_USD;

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
                        DataCha.depth = SttLoai.ToString() + ".";
                        DataCha.isTitle = "font-bold";
                        idLoai = item.iLoaiNoiDungChi;
                        listData.Add(DataCha);
                    }
                    if (item.IDHopDong != idHopDong && item.IDHopDong != Guid.Empty)
                    {
                        SttHopDong++;
                        CPNHThucHienNganSach_Model DataCha = new CPNHThucHienNganSach_Model();
                        List<CPNHThucHienNganSach_Model> listDataCha = list.Where(x => x.IDHopDong == item.IDHopDong && x.iLoaiNoiDungChi == item.iLoaiNoiDungChi && x.IDDuAn == item.IDDuAn && x.IDNhiemVuChi == item.IDNhiemVuChi).ToList();

                        DataCha.HopDongUSD = listDataCha.Sum(x => x.HopDongUSD);
                        DataCha.HopDongVND = listDataCha.Sum(x => x.HopDongVND);

                        DataCha.KinhPhiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiUSD);
                        DataCha.KinhPhiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiVND);
                        DataCha.KinhPhiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYUSD);
                        DataCha.KinhPhiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiToYVND);
                        DataCha.KinhPhiDaChiUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiUSD);
                        DataCha.KinhPhiDaChiVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiVND);
                        DataCha.KinhPhiDaChiToYUSD = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYUSD);
                        DataCha.KinhPhiDaChiToYVND = listDataCha.GroupBy(x => x.iID_ThanhToanID).Select(x => x.First()).Sum(x => x.KinhPhiDaChiToYVND);

                        DataCha.fLuyKeKinhPhiDuocCap_USD = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_USD);
                        DataCha.fLuyKeKinhPhiDuocCap_VND = listDataCha.Sum(x => x.fLuyKeKinhPhiDuocCap_VND);
                        DataCha.fDeNghiQTNamNay_USD = listDataCha.Sum(x => x.fDeNghiQTNamNay_USD);
                        DataCha.fDeNghiQTNamNay_VND = listDataCha.Sum(x => x.fDeNghiQTNamNay_VND);

                        //DataCha.NCVTTCP = listDataCha.Sum(x => x.NCVTTCP);
                        //DataCha.NhiemVuChi = listDataCha.Sum(x => x.NhiemVuChi);

                        DataCha.TongKinhPhiUSD = DataCha.KinhPhiUSD + DataCha.KinhPhiToYUSD;
                        DataCha.TongKinhPhiVND = DataCha.KinhPhiVND + DataCha.KinhPhiToYVND;

                        DataCha.TongKinhPhiDaChiUSD = DataCha.KinhPhiDaChiUSD + DataCha.KinhPhiDaChiToYUSD;
                        DataCha.TongKinhPhiDaChiVND = DataCha.KinhPhiDaChiVND + DataCha.KinhPhiDaChiToYVND;

                        //DataCha.KinhPhiDuocCapChuaChiUSD = DataCha.TongKinhPhiUSD - DataCha.TongKinhPhiDaChiUSD;
                        //DataCha.KinhPhiDuocCapChuaChiVND = DataCha.TongKinhPhiVND - DataCha.TongKinhPhiDaChiVND;
                        //DataCha.QuyGiaiNganTheoQuy = DataCha.NhiemVuChi - DataCha.TongKinhPhiUSD;

                        DataCha.KinhPhiChuaQuyetToanUSD = DataCha.fLuyKeKinhPhiDuocCap_USD - DataCha.fDeNghiQTNamNay_USD;
                        DataCha.KinhPhiChuaQuyetToanVND = DataCha.fLuyKeKinhPhiDuocCap_VND - DataCha.fDeNghiQTNamNay_VND;
                        //DataCha.KeHoachGiaiNgan = DataCha.NCVTTCP - DataCha.fLuyKeKinhPhiDuocCap_USD;

                        DataCha.sTenNoiDungChi = item.sTenHopDong;
                        DataCha.isHopDong = true;
                        DataCha.depth = SttLoai.ToString() + "." + SttHopDong.ToString() + ".";
                        idHopDong = item.IDHopDong;
                        listData.Add(DataCha);
                    }

                    DataTong.HopDongUSD += item.HopDongUSD;
                    DataTong.HopDongVND += item.HopDongVND;
                    //DataTong.KinhPhiUSD += item.KinhPhiUSD;
                    //DataTong.KinhPhiVND += item.KinhPhiVND;
                    //DataTong.KinhPhiToYUSD += item.KinhPhiToYUSD;
                    //DataTong.KinhPhiToYVND += item.KinhPhiToYVND;
                    //DataTong.KinhPhiDaChiUSD += item.KinhPhiDaChiUSD;
                    //DataTong.KinhPhiDaChiVND += item.KinhPhiDaChiVND;
                    //DataTong.KinhPhiDaChiToYUSD += item.KinhPhiDaChiToYUSD;
                    //DataTong.KinhPhiDaChiToYVND += item.KinhPhiDaChiToYVND;
                    //DataTong.KinhPhiDuocCapChuaChiUSD += item.KinhPhiDuocCapChuaChiUSD;
                    //DataTong.KinhPhiDuocCapChuaChiVND += item.KinhPhiDuocCapChuaChiVND;


                    //DataTong.TongKinhPhiUSD += item.TongKinhPhiUSD;
                    //DataTong.TongKinhPhiVND += item.TongKinhPhiVND;

                    //DataTong.TongKinhPhiDaChiUSD += item.TongKinhPhiDaChiUSD;
                    //DataTong.TongKinhPhiDaChiVND += item.TongKinhPhiDaChiVND;
                    //DataTong.QuyGiaiNganTheoQuy += item.QuyGiaiNganTheoQuy;

                    DataTong.fLuyKeKinhPhiDuocCap_USD += item.fLuyKeKinhPhiDuocCap_USD;
                    DataTong.fLuyKeKinhPhiDuocCap_VND += item.fLuyKeKinhPhiDuocCap_VND;
                    DataTong.fDeNghiQTNamNay_USD += item.fDeNghiQTNamNay_USD;
                    DataTong.fDeNghiQTNamNay_VND += item.fDeNghiQTNamNay_VND;

                    //DataTong.KinhPhiChuaQuyetToanUSD += item.KinhPhiChuaQuyetToanUSD;
                    //DataTong.KinhPhiChuaQuyetToanVND += item.KinhPhiChuaQuyetToanVND;

                    if (sttTong == list.Count())
                    {
                        DataTong.KeHoachGiaiNgan = DataTong.NCVTTCP - DataTong.fLuyKeKinhPhiDuocCap_USD;
                        DataTong.sTenNoiDungChi = "Tổng Cộng: ";
                        DataTong.isDuAn = true;
                        DataTong.isTitle = "font-bold";
                        DataTong.isSum = true;
                        listData.Add(DataTong);
                    }
                }
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