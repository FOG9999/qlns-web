using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_MuclucController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_Mucluc.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;

        public ActionResult Print(string ext)
        {
            var xls = createReport();
            return Print(xls, ext);
        }

        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.SetValue(new
            {
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getTable(PhienLamViec.iNamLamViec);
            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", PhienLamViec.iNamLamViec);
        }

        private DataTable getTable(string NamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_mucluc_report.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec,
                    code = DBNull.Value,
                    nganh = DBNull.Value,
                    ng = DBNull.Value
                });
                return dt;
            }
        }

        private string getTieuDe()
        {
            var nam = int.Parse(PhienLamViec.iNamLamViec) + 1;
            var tieude = $"Mục lục thông báo số kiểm tra dự toán ngân sách năm {nam}";

            return tieude;
        }

        #endregion
    }
}
