using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{
    public class rptDuToanBS_BVTCViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList DotList { get; set; }
        public SelectList PhongBanList { get; set; }

        public string TieuDe { get; set; }
        public string QuyetDinh { get; set; }

        public string GhiChu { get; set; }

        public int TrinhKy { get; set; }
    }

    public class rptDuToanBS_BVTCController : FlexcelReportController
    {
        #region ctor

        private const string _viewPath = "~/Views/Report_Views/DuToanBS/";
        
        private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_BVTC.xls";
                
        private int _dvt = 1000;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _dutoanbsService = DuToanBsService.Default;

        public ActionResult Index(int trinhKy = 0)
        {
            var _namLamViec = PhienLamViec.iNamLamViec;

            var dtPhongBan = DuToanBS_ReportModels.LayDSPhongBan(_namLamViec, Username);
            
            var dtDot = _dutoanbsService.GetDots(_namLamViec, Username, "1020900");                

            var dotList = dtDot
                .ToSelectList("iID_MaDot", "sMoTa", "-1", "-- Chọn đợt --");

            var vm = new rptDuToanBS_BVTCViewModel
            {
                iNamLamViec = _namLamViec,
                PhongBanList = dtPhongBan.ToSelectList("iID_MaPhongBan", "sTenPhongBan"),
                DotList = dotList,
                TieuDe = "Giao dự toán ngân sách",
                QuyetDinh = "số           /QĐ-BQP ngày      tháng      năm 20      của Bộ trưởng Bộ Quốc phòng",
                TrinhKy = trinhKy,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iID_MaDot,
            string dNgay,
            string iID_MaDonVi,
            string iID_MaPhongBan,
            string tenPhuLuc = null,
            string quyetDinh = null,
            string ghiChu = null,
            int loaiBaoCao = 0,
            int dvt = 1000)
        {
            _dvt = dvt == 0 ? 1000 : dvt;

            var xls = createReport(Server.MapPath(_filePath), iID_MaDot, dNgay, iID_MaDonVi, iID_MaPhongBan, tenPhuLuc, quyetDinh, ghiChu, loaiBaoCao);
            return Print(xls, ext);
        }

        public JsonResult Ds_PhongBan(string iID_MaDot)
        {
            iID_MaDot = _dutoanbsService.GetChungTus_DotNgay(iID_MaDot, Username, PhienLamViec.iID_MaPhongBan).Join();

            var data = _dutoanbsService.GetPhongBanTheoChungTus(PhienLamViec.iNamLamViec, iID_MaDot, PhienLamViec.iID_MaDonVi,"1020900");
            var vm = new ChecklistModel("PhongBan", data.ToSelectList("-1","---- Chọn phòng ban ----"));
            return ToDropdownList(vm);
        }

        public JsonResult Ds_DonVi(string iID_MaDot, string iID_MaPhongBan)
        {   
            iID_MaDot = _dutoanbsService.GetChungTus_DotNgay(iID_MaDot, Username, PhienLamViec.iID_MaPhongBan).Join();           

            var data = _dutoanbsService.GetDonviTheoChungTus(PhienLamViec.iNamLamViec, iID_MaDot, PhienLamViec.iID_MaDonVi, iID_MaPhongBan, 2, "1020900");
            var vm = new ChecklistModel("DonVi", data.ToSelectList());
            return ToCheckboxList(vm);
        }        
        #endregion

        #region private methods        

        private ExcelFile createReport(string path,
            string iID_MaDot,
            string dNgay,
            string iID_MaDonVi,
            string iID_MaPhongBan,
            string tenPhuLuc,
            string quyetDinh,
            string ghiChu,
            int loaiBaoCao)
        {
            var fr = new FlexCelReport();

            //Lấy dữ liệu chi tiết
            LoadData(fr, iID_MaDot, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao);           
            var sTenPB = "B " + iID_MaPhongBan;
            var sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen;

            fr.SetValue("DotNgay", dNgay.ToParamDate().ToStringNgay().ToLower());
            fr.SetValue("sTenPB", sTenPB);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("TenPhuLuc", string.IsNullOrWhiteSpace(tenPhuLuc) ? "TenPhuLuc" : tenPhuLuc.ToUpper());
            fr.SetValue("QuyetDinh", string.IsNullOrWhiteSpace(quyetDinh) ? "QuyetDinh" : quyetDinh);
            
            var sTenPhongBan = NganSachService.Default.GetPhongBanById(iID_MaPhongBan).sMoTa;
            fr.SetValue("sTenPhongBan", sTenPhongBan);

            var xls = new XlsFile(true);

            xls.Open(path);

            fr
                .SetValue(new
                {
                    h1 = sTenDonVi,
                    h2 = $"Đơn vị tính: {_dvt.ToStringDvt()}"
                })
                .UseChuKy(Username, iID_MaPhongBan)
                .UseCommonValue()
                .UseChuKyForController(this.ControllerName(), iID_MaPhongBan)
                .UseForm(this)
                .Run(xls);
            xls.Recalc(true);

            return xls;
        }

        private DataTable getDataTable(string iID_MaPhongBan, string iID_MaDonVi, string iID_MaChungTu, int loaiBaoCao)
        {
            #region sql

            var sql = string.Empty;
            var sLNS = string.Empty;
            
            sql = FileHelpers.GetSqlQuery("dtbs_chitieu_bvtc.sql");
            #endregion

            #region load data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi + '%');
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@dvt", _dvt);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
            #endregion
        }
        
        private void LoadData(FlexCelReport fr, string iID_MaDot, string iID_MaPhongBan, string iID_MaDonVi, int loaiBaoCao)
        {
            var iID_MaChungTu = _dutoanbsService.GetChungTus_DotNgay(iID_MaDot, Username, PhienLamViec.iID_MaPhongBan).Join();

            var data = getDataTable(iID_MaPhongBan, iID_MaDonVi, iID_MaChungTu, loaiBaoCao);
            
            #region tong tien            

            var totalSum = data.AsEnumerable().Sum(x => x.Field<decimal>("rTuChi")) * _dvt;
            
            //In loại tiền bằng chữ
            var bangChu = "";
            if (totalSum < 0)
            {
                totalSum *= -1;
                bangChu = "Giảm " + totalSum.ToStringMoney().ToLower();
            }
            else
            {
                bangChu = totalSum.ToStringMoney();
            }

            data.Columns.Add("String", typeof(string));
            data.Columns.Add("Number", typeof(string));
            foreach(DataRow dtr in data.Rows)
            {
                var dec = Convert.ToDecimal(dtr["rTuChi"]) * _dvt;
                var str = dec.ToStringMoney();
                dtr["String"] = str;
                dtr["Number"] = dec.ToString("0,0",CultureInfo.InstalledUICulture).Replace(',','.');
            }
            data.AddOrdinalsNum(3);

            fr.SetValue("Total", totalSum/_dvt);
            fr.SetValue("Tien", bangChu);
            fr.AddTable("ChiTiet", data);

            #endregion
        }

        #endregion

    }
}
