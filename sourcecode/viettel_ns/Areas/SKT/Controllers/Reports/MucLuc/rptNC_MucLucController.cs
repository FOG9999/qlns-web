using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNC_MucLucController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/MucLuc/rptNC_MucLuc.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;

        public ActionResult Print(string ext = "pdf")
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
                h1 = string.Empty,
                h2 = string.Empty,
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.ToStringNgay(),
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getTable(PhienLamViec.iNamLamViec);
            _SKTService.FillDataTable_NC(fr, data);
        }

        private DataTable getTable(string NamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("rpt_nc_mucluc.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec
                });
                return dt;
            }
        }

        private string getTieuDe()
        {
            var tieude = $"Mục lục nhu cầu số kiểm tra dự toán ngân sách năm {PhienLamViec.iNamLamViec}";

            return tieude;
        }

        #endregion
    }
}
