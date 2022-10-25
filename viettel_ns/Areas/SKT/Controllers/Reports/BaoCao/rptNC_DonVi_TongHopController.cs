using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{

    public class rptNC_DonVi_TongHopViewModel : ReportModels
    {
        public SelectList PhongBanList { get; set; }
        public SelectList ToList { get; set; }
        public int ToCount { get; set; }
    }

    public class rptNC_DonVi_TongHopController : FlexcelReportController
    {
        private const string _filePath_To1 = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_To1.xls";
        private const string _filePath_To2 = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_To2.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban;
        private int to;
        private int to_count;
        private int dvt;

        public ActionResult Index()
        {

            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa");

            var donviCount = PhienLamViec.iID_MaDonVi.ToList().Count;
            to_count = 1 + donviCount / 2 +
                         (donviCount % 2 == 0 ? 0 : 1);

            var toList = new Dictionary<int, string>();
            for (int i = 1; i <= to_count; i++)
            {
                toList.Add(i, $"Tờ {i}");
            }
            var vm = new rptNC_DonVi_TongHopViewModel()
            {
                PhongBanList = phongbanList,
                ToList = toList.ToSelectList("Key", "Value"),
                ToCount = to_count,
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptNC_DonVi_TongHop.cshtml", vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="loai">1: NSSD, 2: NSBĐ ngành</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(string id_phongban, int to, int to_count, string ext = "pdf", int dvt = 1000)
        {
            this.id_phongban = id_phongban;
            this.to = to;
            this.to_count = to_count;
            this.dvt = dvt;

            var xls = createReport();

            //var filename = $"BC_NhuCau_{(donvi.IsEmpty() ? id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_{DateTime.Now.GetTimeStamp()}.{ext}";
            var filename = $"BC_NhuCau_TongHop_ToSo{to}_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls, ext, filename);
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
            var file = to == 1 ? _filePath_To1 : _filePath_To2;
            xls.Open(Server.MapPath(file));

            var Cap2 = id_phongban.IsEmpty() ? "" : _nganSachService.GetPhongBanById(id_phongban).sMoTa;

            fr.SetValue(new
            {
                h1 = $"Tờ số: {to}/{to_count}  Đơn vị tính: {dvt.ToStringNumber()}đ  ",
                h2 = Cap2,
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.ToStringNgay(),
                NamTruoc = int.Parse(PhienLamViec.iNamLamViec) - 2,
                NamNay = int.Parse(PhienLamViec.iNamLamViec) - 1,

                Cap1 = "Cục Tài chính",
                Cap2 = Cap2,
                //DonVi = donvi.IsEmpty() ? "(Tổng hợp đơn vị)" : $"Đơn vị: {donvi}",
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = to == 1 ? getTable_tonghop() : getTable();
            _SKTService.FillDataTable_NC(fr, data);


            // ten donvi

            if (to != 1)
            {
                var donviList = PhienLamViec.iID_MaDonVi.ToList().Skip((to - 2) * 2).Take(2).ToList();
                for (int i = 0; i < donviList.Count; i++)
                {
                    var donvi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, donviList[i]);
                    fr.SetValue($"Donvi_{i + 1}", $"{donvi.iID_MaDonVi} - {donvi.sMoTa}");
                }

            }
        }

        private DataTable getTable_tonghop()
        {
            var sql = FileHelpers.GetSqlQuery("rpt_nc_donvi.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = id_phongban.ToParamString(),
                    id_donvi = PhienLamViec.iID_MaDonVi.ToParamString(),
                    dvt,
                });
                return dt;
            }
        }

        private DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("rpt_nc_donvi.sql");
            var donviList = PhienLamViec.iID_MaDonVi.ToList().Skip((to - 2) * 2).Take(2).ToList();

            // lay data1

            using (var conn = _connectionFactory.GetConnection())
            {
                var id_donvi1 = donviList.FirstOrDefault();
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = id_phongban.ToParamString(),
                    id_donvi = id_donvi1,
                    dvt,
                });

                var columnsNumber = (from DataColumn col in dt.Columns where col.DataType == typeof(double) select $"{col.ColumnName}").ToList();

                // them cot tiep theo
                for (int i = 2; i <= 2; i++)
                {
                    foreach (var col in columnsNumber)
                    {
                        dt.Columns.Add($"{col}_{i}", typeof(double));
                    }
                }


                var id_donvi2 = donviList.ElementAtOrDefault(1);
                if (id_donvi2.IsNotEmpty())
                {
                    var dt2 = conn.GetTable(sql, new
                    {
                        NamLamViec = PhienLamViec.iNamLamViec,
                        id_phongban = id_phongban.ToParamString(),
                        id_donvi = id_donvi2,
                        dvt,
                    });


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var row = dt.Rows[i];
                        var row_x = dt2.Rows[i];
                        foreach (var col in columnsNumber)
                        {
                            row[$"{col}_2"] = row_x[col];
                        }
                    }
                }

                return dt;
            }
        }

        private string getTieuDe()
        {
            var tieude = $"Báo cáo chi tiết dự toán nhu cầu ngân sách năm {PhienLamViec.iNamLamViec}";
            return tieude;
        }

        #endregion
    }
}
