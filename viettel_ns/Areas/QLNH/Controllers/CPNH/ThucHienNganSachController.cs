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
    public class ThucHienNganSachController : FlexcelReportController
    {
        private readonly ICPNHService _cpnhService = CPNHService.Default;
        private readonly IQLNguonNganSachService _nnsService = QLNguonNganSachService.Default;
        private const string sFilePathBaoCao1 = "/Report_ExcelFrom/QLNH/rpt_ThucHienNganSach.xlsx";
        private const string sFilePathBaoCao2 = "/Report_ExcelFrom/QLNH/rpt_ThucHienNganSach_GiaiDoan.xlsx";
        private int _columnCountBC1 = 7;
        private const string sControlName = "ThucHienNganSach";

        // GET: QLVonDauTu/QLDMTyGia
        public ActionResult Index()
        {
            CPNHThucHienNganSach_ModelPaging vm = new CPNHThucHienNganSach_ModelPaging();
            List<CPNHThucHienNganSach_Model> list = _cpnhService.getListThucHienNganSachModels(1, DateTime.Now.Year, DateTime.Now.Year  ,null, 1, DateTime.Now.Year).ToList();
            List<CPNHThucHienNganSach_Model> getlistGiaiDoan = list.Where(x => x.iGiaiDoanTu != 0 && x.iGiaiDoanDen != 0).OrderBy(x => x.iGiaiDoanDen).OrderBy(x => x.iGiaiDoanTu).ToList();
            List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan = getlistGiaiDoan
                    .GroupBy(x => new { x.iGiaiDoanTu, x.iGiaiDoanDen }).Select(x => x.First())
                    .Select(x => new ThucHienNganSach_GiaiDoan_Model
                    {
                        sGiaiDoan = "Giai đoạn " + x.iGiaiDoanTu + " - " + x.iGiaiDoanDen,
                        iGiaiDoanTu = x.iGiaiDoanTu,
                        iGiaiDoanDen = x.iGiaiDoanDen
                    }).ToList();

            List<CPNHThucHienNganSach_Model> listData = getList(list , lstGiaiDoan , 1);
            vm.Items = listData;
            List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                };
            ViewBag.Count = vm.Items.Count();
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");

            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả đơn vị--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            ViewBag.GiaiDoan = lstGiaiDoan;
            ViewBag.SoGiaiDoan = lstGiaiDoan.Count;
            List<Dropdown_ByYear_ThucHienNganSach> lstNam = GetListNamKeHoach().ToList();
            ViewBag.ListYear = lstNam;
            ViewBag.YearNow = DateTime.Now.Year;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ThucHienNganSachSearch(int tabTable, int iTuNam, int iDenNam, Guid? iDonvi, int iQuyList, int iNam)
        {
            CPNHThucHienNganSach_ModelPaging vm = new CPNHThucHienNganSach_ModelPaging();
            List<CPNHThucHienNganSach_Model> list = _cpnhService.getListThucHienNganSachModels(tabTable, iTuNam, iDenNam, iDonvi, iQuyList, iNam).ToList();
            List<CPNHThucHienNganSach_Model> getlistGiaiDoan = list.Where(x => x.iGiaiDoanTu != 0 && x.iGiaiDoanDen != 0).OrderBy(x => x.iGiaiDoanDen).OrderBy(x => x.iGiaiDoanTu).ToList();
            List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan = getlistGiaiDoan
                    .GroupBy(x => new { x.iGiaiDoanTu, x.iGiaiDoanDen }).Select(x => x.First())
                    .Select(x => new ThucHienNganSach_GiaiDoan_Model
                    {
                        sGiaiDoan = "Giai đoạn " + x.iGiaiDoanTu + " - " + x.iGiaiDoanDen,
                        iGiaiDoanTu = x.iGiaiDoanTu,
                        iGiaiDoanDen = x.iGiaiDoanDen
                    }).ToList();
            List<CPNHThucHienNganSach_Model> listData = getList(list, lstGiaiDoan, tabTable);
            vm.Items = listData;
            List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                };
            ViewBag.Count = vm.Items.Count();
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");

            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            ViewBag.GiaiDoan = lstGiaiDoan;
            ViewBag.SoGiaiDoan = lstGiaiDoan.Count;
            List<Dropdown_ByYear_ThucHienNganSach> lstNam = GetListNamKeHoach().ToList();
            ViewBag.ListYear = lstNam;
            ViewBag.YearNow = DateTime.Now.Year;

            return PartialView("_list", vm);
        }

        public ActionResult ExportExcelBaoCao(string ext = "xls", int dvt = 1, int to = 1, int tabTable = 1, int iTuNam = 2022, int iDenNam = 2022, Guid? iDonvi = null, int iQuyList = 0, int iNam = 2022)
        {
            string fileName = string.Format("{0}.{1}", "BaoCaoTinhHinhThucHienNganSach", ext);
            List<CPNHThucHienNganSach_Model> list = _cpnhService.getListThucHienNganSachModels(tabTable, iTuNam, iDenNam, iDonvi, iQuyList, iNam).ToList();
            ExcelFile xls = null;
            if (tabTable == 1 )
            {
                xls = TaoFileBaoCao1(dvt, to , list, tabTable);
            }
            else
            {
                xls = TaoFileBaoCao2(dvt, to , list, tabTable);
            }
            return Print(xls, ext, fileName);
        }

        public ExcelFile TaoFileBaoCao1(int dvt = 1, int to = 1 , List<CPNHThucHienNganSach_Model> list = null , int tabTable = 1)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao1));
            FlexCelReport fr = new FlexCelReport();

            int columnStart = _columnCountBC1 * (to - 1);
                        List<CPNHThucHienNganSach_Model> listData = getList(list , null, tabTable);
            fr.AddTable<CPNHThucHienNganSach_Model>("dt", listData);
            fr.SetValue(new
            {
                dvt = dvt.ToStringDvt(),
                To = to,
                iQuy = 0,
                iNam = 0
            });
            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this).Run(Result);


            return Result;
        }
        public ExcelFile TaoFileBaoCao2(int dvt = 1, int to = 1, List<CPNHThucHienNganSach_Model> list = null, int tabTable = 1)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao2));
            FlexCelReport fr = new FlexCelReport();
            List<CPNHThucHienNganSach_Model> getlistGiaiDoan = list.Where(x => x.iGiaiDoanTu != 0 && x.iGiaiDoanDen != 0).OrderBy(x => x.iGiaiDoanTu).OrderBy(x => x.iGiaiDoanDen).ToList();
            List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan = getlistGiaiDoan
                    .GroupBy(x => new { x.iGiaiDoanTu, x.iGiaiDoanDen }).Select(x => x.First())
                    .Select(x => new ThucHienNganSach_GiaiDoan_Model
                    {
                        sGiaiDoan = "Giai đoạn " + x.iGiaiDoanTu + " - " + x.iGiaiDoanDen,
                        iGiaiDoanTu = x.iGiaiDoanTu,
                        iGiaiDoanDen = x.iGiaiDoanDen
                    }).ToList();
            int columnStart = _columnCountBC1 * (to - 1);
            List<CPNHThucHienNganSach_Model> listData = getList(list , lstGiaiDoan, tabTable);
            var listColumn = new List<ThucHienNganSach_GiaiDoan_Model>();
            if (lstGiaiDoan != null)
            {
                var countColumn = 10 + lstGiaiDoan.Count() + (lstGiaiDoan.Count() * 4);
                for (var i = 1; i <= countColumn; i++)
                {
                    var startColumn1 = 3 + lstGiaiDoan.Count() + 1;
                    var startColumn2 = startColumn1 + (lstGiaiDoan.Count() * 2) + 2;
                    var startColumn3 = startColumn2 + (lstGiaiDoan.Count() * 2) + 2;

                    if (i == 3)
                    {
                        ThucHienNganSach_GiaiDoan_Model SoBC = new ThucHienNganSach_GiaiDoan_Model();
                        var nowColumn = i.ToString() + " =";
                        for (var j = 1; j <= lstGiaiDoan.Count(); j++)
                        {
                            nowColumn += (i + j).ToString() + " +";
                        }
                        SoBC.sGiaiDoan = nowColumn.Remove(nowColumn.Length - 1);
                        listColumn.Add(SoBC);
                    }
                    else if (i == startColumn1 || i == startColumn1 + 1 || i == startColumn2 || i == startColumn2 + 1)
                    {
                        ThucHienNganSach_GiaiDoan_Model SoBC = new ThucHienNganSach_GiaiDoan_Model();
                        var nowColumn = i.ToString() + " =";
                        for (var j = 1; j <= lstGiaiDoan.Count(); j++)
                        {
                            nowColumn += (i + (j * 2)).ToString() + " +";
                        }
                        SoBC.sGiaiDoan = nowColumn.Remove(nowColumn.Length - 1);
                        listColumn.Add(SoBC);
                    }
                    else if (i == startColumn3)
                    {
                        ThucHienNganSach_GiaiDoan_Model SoBC = new ThucHienNganSach_GiaiDoan_Model();
                        var nowColumn = i.ToString() + " =" + startColumn1.ToString() + " -" + startColumn2.ToString();
                        SoBC.sGiaiDoan = nowColumn;
                        listColumn.Add(SoBC);
                    }
                    else if (i == startColumn3 + 1)
                    {
                        ThucHienNganSach_GiaiDoan_Model SoBC = new ThucHienNganSach_GiaiDoan_Model();
                        var nowColumn = i.ToString() + " =" + (startColumn1 + 1).ToString() + " -" + (startColumn2 + 1).ToString();
                        SoBC.sGiaiDoan = nowColumn;
                        listColumn.Add(SoBC);
                    }
                    else if (i == countColumn)
                    {
                        ThucHienNganSach_GiaiDoan_Model SoBC = new ThucHienNganSach_GiaiDoan_Model();
                        var nowColumn = i.ToString() + " =" + 3 + " -" + (startColumn1).ToString();
                        SoBC.sGiaiDoan = nowColumn;
                        listColumn.Add(SoBC);
                    }
                    else
                    {
                        ThucHienNganSach_GiaiDoan_Model SoBC = new ThucHienNganSach_GiaiDoan_Model();
                        SoBC.sGiaiDoan = i.ToString();
                        listColumn.Add(SoBC);
                    }
                }

            }

            fr.SetValue(new
            {
                dvt = dvt.ToStringDvt(),
                To = to,
                iQuy = 0,
                iNam = 0,
            });
            fr.AddTable<CPNHThucHienNganSach_Model>("dt", listData);
            fr.AddTable<ThucHienNganSach_GiaiDoan_Model>("listColumn", listColumn);
            fr.AddTable<ThucHienNganSach_GiaiDoan_Model>("lstGiaiDoan", lstGiaiDoan);
            fr.AddTable<ThucHienNganSach_GiaiDoan_Model>("lstGiaiDoan2", lstGiaiDoan);
            fr.AddTable<ThucHienNganSach_GiaiDoan_Model>("lstGiaiDoan3", lstGiaiDoan);
            fr.UseChuKy(Username)
            .UseChuKyForController(sControlName)
            .UseForm(this).Run(Result);

            var col1 = 6 + lstGiaiDoan.Count();
            var col2 = col1 + ((lstGiaiDoan.Count() + 1) * 2);
            Result.MergeCells(6, 6, 6, col1);
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
            Result.SetCellFormat(6, 2, 8+ listData.Count(), 13 + lstGiaiDoan.Count() * 5, b, ApplyFormat, false);

            return Result;
        }
        private string convertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }
        private List<CPNHThucHienNganSach_Model> getList(List<CPNHThucHienNganSach_Model> list, List<ThucHienNganSach_GiaiDoan_Model> lstGiaiDoan, int tabTable)
        {
            if (lstGiaiDoan != null)
            {
                var countColumn = 10 + lstGiaiDoan.Count() + (lstGiaiDoan.Count() * 4);
                var listColumn = new List<string>();
                for (var i = 1; i <= countColumn; i++)
                {
                    var startColumn1 = 3 + lstGiaiDoan.Count() + 1;
                    var startColumn2 = startColumn1 + (lstGiaiDoan.Count() * 2) + 2;
                    var startColumn3 = startColumn2 + (lstGiaiDoan.Count() * 2) + 2;

                    if (i == 3)
                    {
                        var nowColumn = i.ToString() + " =";
                        for (var j = 1; j <= lstGiaiDoan.Count(); j++)
                        {
                            nowColumn += (i + j).ToString() + " +";
                        }
                        listColumn.Add(nowColumn.Remove(nowColumn.Length - 1));
                    }
                    else if (i == startColumn1 || i == startColumn1 + 1 || i == startColumn2 || i == startColumn2 + 1)
                    {
                        var nowColumn = i.ToString() + " =";
                        for (var j = 1; j <= lstGiaiDoan.Count(); j++)
                        {
                            nowColumn += (i + (j * 2)).ToString() + " +";
                        }
                        listColumn.Add(nowColumn.Remove(nowColumn.Length - 1));
                    }
                    else if (i == startColumn3)
                    {
                        var nowColumn = i.ToString() + " =" + startColumn1.ToString() + " -" + startColumn2.ToString();
                        listColumn.Add(nowColumn);
                    }
                    else if (i == startColumn3 + 1)
                    {
                        var nowColumn = i.ToString() + " =" + (startColumn1 + 1).ToString() + " -" + (startColumn2 + 1).ToString();
                        listColumn.Add(nowColumn);
                    }
                    else if (i == countColumn)
                    {
                        var nowColumn = i.ToString() + " =" + 3 + " -" + (startColumn1).ToString();
                        listColumn.Add(nowColumn);
                    }
                    else
                    {
                        listColumn.Add(i.ToString());
                    }
                }

                ViewBag.ListColumn = listColumn;
            }
            
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
            DataTong.lstGiaiDoanTTCP = new List<ThucHienNganSach_GiaiDoan_Model>();
            DataTong.lstGiaiDoanKinhPhiDuocCap = new List<ThucHienNganSach_GiaiDoan_Model>();
            DataTong.lstGiaiDoanKinhPhiDaGiaiNgan = new List<ThucHienNganSach_GiaiDoan_Model>();
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
            if (lstGiaiDoan != null)
            {
                foreach (var giaiDoan in lstGiaiDoan)
                {
                    List<CPNHThucHienNganSach_Model> listDataChaGiaiDoan = listTong.Where(x => x.iGiaiDoanTu == giaiDoan.iGiaiDoanTu && x.iGiaiDoanDen == giaiDoan.iGiaiDoanDen).ToList();
                    DataTong.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.GroupBy(x => x.IDNhiemVuChi).Select(x => x.First()).Sum(x => x.NCVTTCP) });
                    DataTong.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_VND) });
                    DataTong.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_VND) });
                }
            }
            if(list != null)
            {
                foreach (var item in list)
                {
                    sttTong++;
                    item.TongKinhPhiUSD = item.KinhPhiUSD + item.KinhPhiToYUSD;
                    item.TongKinhPhiVND = item.KinhPhiVND + item.KinhPhiToYVND;
                   
                    item.TongKinhPhiDaChiUSD = item.KinhPhiDaChiUSD + item.KinhPhiDaChiToYUSD;
                    item.TongKinhPhiDaChiVND = item.KinhPhiDaChiVND + item.KinhPhiDaChiToYVND;

                    item.KinhPhiDuocCapChuaChiUSD = item.TongKinhPhiUSD - item.TongKinhPhiDaChiUSD;
                    item.KinhPhiDuocCapChuaChiVND = item.TongKinhPhiVND - item.TongKinhPhiDaChiVND;
                    item.QuyGiaiNganTheoQuy = item.NhiemVuChi - item.TongKinhPhiUSD;

                    item.KinhPhiChuaQuyetToanUSD = item.fLuyKeKinhPhiDuocCap_USD - item.fDeNghiQTNamNay_USD;
                    item.KinhPhiChuaQuyetToanVND = item.fLuyKeKinhPhiDuocCap_VND - item.fDeNghiQTNamNay_VND;
                    item.KeHoachGiaiNgan = item.NCVTTCP - item.fLuyKeKinhPhiDuocCap_USD;

                    if (lstGiaiDoan != null)
                    {
                        item.lstGiaiDoanTTCP = new List<ThucHienNganSach_GiaiDoan_Model>();
                        item.lstGiaiDoanKinhPhiDuocCap = new List<ThucHienNganSach_GiaiDoan_Model>();
                        item.lstGiaiDoanKinhPhiDaGiaiNgan = new List<ThucHienNganSach_GiaiDoan_Model>();
                        foreach (var giaiDoan in lstGiaiDoan)
                        {
                            
                            if (item.iGiaiDoanTu == giaiDoan.iGiaiDoanTu && item.iGiaiDoanDen == giaiDoan.iGiaiDoanDen)
                            {

                                item.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = item.NCVTTCP });
                                item.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = item.fLuyKeKinhPhiDuocCap_USD, valueVND = item.fLuyKeKinhPhiDuocCap_VND });
                                item.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = item.fDeNghiQTNamNay_USD, valueVND = item.fDeNghiQTNamNay_VND });
                            }
                            else
                            {
                                item.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                item.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                item.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                            }
                        }
                    }
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
                        DataCha.depth = convertLetter(SttChuongTrinh)+".";
                        DataCha.isTitle = "font-bold-red";
                        idChuongTrinh = item.IDNhiemVuChi;
                        DataCha.lstGiaiDoanTTCP = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDuocCap = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDaGiaiNgan = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.iGiaiDoanDen = item.iGiaiDoanDen;
                        DataCha.iGiaiDoanTu = item.iGiaiDoanTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<CPNHThucHienNganSach_Model> listDataChaGiaiDoan = listDataCha.Where(x => x.iGiaiDoanTu == giaiDoan.iGiaiDoanTu && x.iGiaiDoanDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = item.NCVTTCP });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_VND) });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_VND) });
                                }
                                else
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                }
                                
                            }
                        }
                           
                        listData.Add(DataCha);
                    }
                    if (item.IDDuAn != idDuAn /*&& item.IDDuAn != Guid.Empty*/)
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
                        DataCha.lstGiaiDoanTTCP = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDuocCap = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDaGiaiNgan = new List<ThucHienNganSach_GiaiDoan_Model>();

                        DataCha.iGiaiDoanDen = item.iGiaiDoanDen;
                        DataCha.iGiaiDoanTu = item.iGiaiDoanTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<CPNHThucHienNganSach_Model> listDataChaGiaiDoan = listDataCha.Where(x => x.iGiaiDoanTu == giaiDoan.iGiaiDoanTu && x.iGiaiDoanDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_VND) });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_VND) });
                                }
                                else
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                }

                            }
                        }

                        listData.Add(DataCha);
                    }
                    if (item.iLoaiNoiDungChi != idLoai && item.iLoaiNoiDungChi != 0)
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
                        DataCha.lstGiaiDoanTTCP = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDuocCap = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDaGiaiNgan = new List<ThucHienNganSach_GiaiDoan_Model>();

                        DataCha.iGiaiDoanDen = item.iGiaiDoanDen;
                        DataCha.iGiaiDoanTu = item.iGiaiDoanTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<CPNHThucHienNganSach_Model> listDataChaGiaiDoan = listDataCha.Where(x => x.iGiaiDoanTu == giaiDoan.iGiaiDoanTu && x.iGiaiDoanDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_VND) });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_VND) });
                                }
                                else
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                }

                            }
                        }

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
                        DataCha.lstGiaiDoanTTCP = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDuocCap = new List<ThucHienNganSach_GiaiDoan_Model>();
                        DataCha.lstGiaiDoanKinhPhiDaGiaiNgan = new List<ThucHienNganSach_GiaiDoan_Model>();

                        DataCha.iGiaiDoanDen = item.iGiaiDoanDen;
                        DataCha.iGiaiDoanTu = item.iGiaiDoanTu;
                        if (lstGiaiDoan != null)
                        {
                            foreach (var giaiDoan in lstGiaiDoan)
                            {
                                List<CPNHThucHienNganSach_Model> listDataChaGiaiDoan = listDataCha.Where(x => x.iGiaiDoanTu == giaiDoan.iGiaiDoanTu && x.iGiaiDoanDen == giaiDoan.iGiaiDoanDen).ToList();
                                if (listDataChaGiaiDoan != null)
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fLuyKeKinhPhiDuocCap_VND) });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_USD), valueVND = listDataChaGiaiDoan.Sum(x => x.fDeNghiQTNamNay_VND) });
                                }
                                else
                                {
                                    DataCha.lstGiaiDoanTTCP.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDuocCap.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                    DataCha.lstGiaiDoanKinhPhiDaGiaiNgan.Add(new ThucHienNganSach_GiaiDoan_Model() { valueUSD = 0, valueVND = 0 });
                                }

                            }
                        }

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

                    DataTong.KinhPhiChuaQuyetToanUSD += item.KinhPhiChuaQuyetToanUSD;
                    DataTong.KinhPhiChuaQuyetToanVND += item.KinhPhiChuaQuyetToanVND;

                    //DataTong.lstGiaiDoanTTCP = new List<ThucHienNganSach_GiaiDoan_Model>();
                    //DataTong.lstGiaiDoanKinhPhiDuocCap = new List<ThucHienNganSach_GiaiDoan_Model>();
                    //DataTong.lstGiaiDoanKinhPhiDaGiaiNgan = new List<ThucHienNganSach_GiaiDoan_Model>();
                    //DataTong.iGiaiDoanDen = item.iGiaiDoanDen;
                    //DataTong.iGiaiDoanTu = item.iGiaiDoanTu;

                    if (tabTable == 2)
                    {
                        item.HopDongUSD = 0;
                        item.HopDongVND = 0;
                        item.NCVTTCP = 0;
                        item.NhiemVuChi = 0;
                        item.KinhPhiDuocCapChuaChiUSD = 0;
                        item.KinhPhiDuocCapChuaChiVND = 0;
                        item.QuyGiaiNganTheoQuy = 0;
                        item.KeHoachGiaiNgan = 0;
                        listData.Add(item);
                    }
                    
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