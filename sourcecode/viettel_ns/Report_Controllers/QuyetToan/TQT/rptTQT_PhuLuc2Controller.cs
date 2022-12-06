using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Application.Flexcel;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptTQT_PhuLuc2ViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList NamNganSachList { get; set; }
        public SelectList PhuLucList { get; set; }
    }


}

namespace VIETTEL.Report_Controllers
{
    public class rptTQT_PhuLuc2Controller : FlexcelReportController
    {
        private readonly IQuyetToanService _qtService = QuyetToanService.Default;


        #region ctors

        private const string _viewPath = "~/Views/Report_Views/QuyetToan/TQT/";

        private const string _filePath_NG = "/Report_ExcelFrom/QuyetToan/TQT/rptQuyetToanNam_2A_TongHop_NG_DonVi.xls";
        private const string _filePath_TongHop = "/Report_ExcelFrom/QuyetToan/TQT/rptTQT_PhuLuc2_NG_DonVi.xls";
        private const string _filePath = "/Report_ExcelFrom/QuyetToan/TQT/rptTQT_PhuLuc2_NG.xls";

        private string _namLamViec = DateTime.Now.Year.ToString();
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private int _loaiBaoCao;
        private DateTime _date;

        public ActionResult Index()
        {
            _namLamViec = ReportModels.LayNamLamViec(Username);

            var dtPhongBan = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));


            var vm = new rptTQT_PhuLuc2ViewModel
            {
                iNamLamViec = _namLamViec,
                PhongBanList = GetPhongBanId(PhienLamViec.iID_MaPhongBan).IsEmpty() ?
                    dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "<-- Tổng hợp cục -->") :
                    dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa"),
                PhuLucList = new Dictionary<string, string>()
                {
                    {"1","Phụ lục 2 - Chi thường xuyên NSNN" },
                    {"5","Phụ lục 5 - Quỹ dự trữ ngoại hối (NS đặc biệt)" },
                    {"6","Phụ lục 6 - Chi dự trữ quốc gia" },
                    {"7","Phụ lục 7 - Chương trình mục tiêu" },


                }.ToSelectList()
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods


        /// <summary>
        ///     Tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private ExcelFile createReport(
            string path,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            int loaiBaoCao,
            string phuluc)
        {
            var fr = new FlexCelReport();

            var data = getDataTable(iID_MaPhongBan, iID_MaDonVi, phuluc);

            if (loaiBaoCao == 2 || loaiBaoCao == 3)
            {
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM,sNG", PhienLamViec.iNamLamViec);
            }
            else
            {
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM,sNG", PhienLamViec.iNamLamViec);
            }

            #region report values

            #region ten mau bieu


            #endregion

            var sTenDonVi = "";
            if (iID_MaDonVi != "-1" && loaiBaoCao == 9)
            {
                sTenDonVi = "Đơn vị: " + NganSachService.Default.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sMoTa;
            }


            //Ten BQL
            fr.SetValue("BQuanLy",
                GetPhongBanId(iID_MaPhongBan).IsEmpty() ? "" : _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa);
            fr.SetValue("sTenDonVi", sTenDonVi);

            #endregion

            var xls = new XlsFile(true);
            xls.Open(path);

            var phongban = GetPhongBanId(iID_MaPhongBan).IsEmpty()
                ? ""
                : _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa;

            fr.UseCommonValue(new FlexcelModel
            {
                header2 = $"{phongban}  Đơn vị tính: đồng - Trang: "
            }, 1)
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        public DataTable DanhSach_DonVi_QuyetToan_PhongBan(string iID_MaPhongBan, string iID_MaNamNganSach, string MaND)
        {
            var sql = FileHelpers.GetSqlQuery("qt_donvi.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach",
                    iID_MaNamNganSach == "0" || string.IsNullOrWhiteSpace(iID_MaNamNganSach) ? "1,2" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", PhienLamViec.DonViList.Select(x => x.Key).Join());

                return cmd.GetTable();
            }
        }


        private DataTable getDataTable(
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string phuluc)
        {
            #region sql

            var sql = "";

            var namTuChi = string.Empty;
            var namDaCapTien = string.Empty;
            var namChuaCapTien = string.Empty;

            if (phuluc.IsEmpty()) phuluc = "1";

            sql = FileHelpers.GetSqlQuery("tqt_phuluc2_2.sql");
            //sql = FileHelpers.GetSqlQuery("tqt_phuluc2_2_gom.sql");

            #endregion

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    PhienLamViec.iNamLamViec,
                    iID_MaPhongBan = iID_MaPhongBan.ToParamString(),
                    iID_MaDonVi = iID_MaDonVi.ToParamString(),
                    phuluc = phuluc.ToParamString()
                });
                return dt;
            }
        }



        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string phuluc,
            int loaiBaoCao,
            string date)
        {
            _loaiBaoCao = loaiBaoCao;
            _date = Convert.ToDateTime(date);

            var filePath = getXlsFile(loaiBaoCao);
            var xls = createReport(filePath, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao, phuluc);

            return Print(xls, ext);
        }

        private string getXlsFile(int loaiBaoCao)
        {
            var path = string.Empty;

            if (loaiBaoCao == 1)
                path = _filePath_TongHop;
            else path = _filePath;


            return Server.MapPath(path);
        }

        public JsonResult Ds_DonVi(string iID_MaPhongBan, string iID_MaNamNganSach)
        {
            var data = _qtService.GetDonVis(PhienLamViec.iNamLamViec, Username, iID_MaNamNganSach, iID_MaPhongBan,
                PhienLamViec.DonViList.Select(x => x.Key).Join());

            var donvis = new Dictionary<string, string>();
            PhienLamViec.DonViList.ToList()
                .ForEach(x =>
                {
                    var tenDonVi = x.Value;

                    var row = data.AsEnumerable().FirstOrDefault(r => r.Field<string>("iID_MaDonVi") == x.Key);
                    if (row == null)
                    {
                        //tenDonVi = x.Value;
                        tenDonVi = $"{x.Value} (N)";
                    }
                    else
                        donvis.Add(x.Key, tenDonVi);
                });
            var vm = new ChecklistModel("DonVi", donvis.ToSelectList());

            return ToCheckboxList(vm);
        }

        #endregion
    }
}