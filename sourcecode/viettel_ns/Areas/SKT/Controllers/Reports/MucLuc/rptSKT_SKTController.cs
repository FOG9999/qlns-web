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
    public class rptSKT_SKTController : FlexcelReportController
    {
        private const string _filePath = "~/Areas/SKT/FlexcelForm/MucLuc/rptSKT_Skt.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;

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
            _SKTService.FillDataTable(fr, data, "KyHieu1 KyHieu2 KyHieu3 KyHieu4", PhienLamViec.iNamLamViec, "SKT_MLSKT");
        }

        private DataTable getTable(string NamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("skt_skt_report.sql");

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
            var tieude = $"Mục lục thông báo số kiểm tra dự toán ngân sách năm {PhienLamViec.iNamLamViec}";

            return tieude;
        }

        #endregion
    }
}
