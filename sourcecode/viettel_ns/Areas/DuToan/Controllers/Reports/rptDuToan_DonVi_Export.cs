using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptDuToan_DonVi_ExportViewModel
    {
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_DonVi_ExportController : FlexcelReportController
    {
        #region var def

        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToan/KiemTra/rptDuToan_Export.xls";
        //private const string _filePath = "~/Report_ExcelFrom/DuToan/KiemTra/rptDuToan_Export_B8.xls";
        //private const string _filePath = "~/Report_ExcelFrom/DuToan/KiemTra/rptDuToan_Export_fox.xls";

        //private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_donvi;
        private int _dvt = 1000;

        public ActionResult Index()
        {

            var data = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

            var vm = new rptDuToan_DonVi_ExportViewModel
            {
                DonViList = data.ToSelectList("iID_MaDonVi", "sMoTa"),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }


        #endregion

        #region public methods

        public ActionResult Print(
            string id_donvi,
            string lns,
            string ext = "pdf")
        {

            _id_donvi = id_donvi;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            var tenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;


            var xls = createReport();
            return Print(xls, ext, filename: $"DuToan_{PhienLamViec.iNamLamViec}_{id_donvi}_{tenDonVi.ToStringAccent().Replace(" ", "")}_{DateTime.Now.GetTimeStamp()}.xls");
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport(string tieuDe = null)
        {
            var fr = new FlexCelReport();
            LoadData(fr);

            var xls = new XlsFile(true);
            xls.Open(getFileXls());

            var tenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;
            fr.UseCommonValue()
                .SetValue(new
                {
                    h1 = tenDonVi,
                    h2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                    TenDonVi = tenDonVi,
                })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            return xls;
        }

        private string getFileXls()
        {
            var file = string.Empty;
            file = _filePath;
            return Server.MapPath(file);
        }
        private void LoadData(FlexCelReport fr)
        {
            var data = getTable_DuToan();
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }

        public DataTable getTable_DuToan()
        {
            //var sql = FileHelpers.GetSqlQuery("rptDuToan_Export_1010000.sql");
            var sql = FileHelpers.GetSqlQuery("rptDuToan_Export_all.sql");
            //var sql = FileHelpers.GetSqlQuery("rptDuToan_Export_V2019");

            #region get data

            //using (var conn = ConnectionFactory.Default.GetConnection())
            //using (var cmd = new SqlCommand(sql, conn))
            //{
            //    cmd.Parameters.AddWithValue("@Id_DonVi", _id_donvi.ToParamString());
            //    cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
            //    cmd.Parameters.AddWithValue("@dvt", _dvt);



            //    var dt = cmd.GetTable();`
            //    return dt;
            //}


            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    Id_DonVi = _id_donvi.ToParamString(),
                    dvt = _dvt,
                });
                return dt;
            }
            #endregion
        }

        #endregion
    }
}
