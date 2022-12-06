using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{

    public class rptSKT_DonViViewModel : ReportModels
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }

    public class rptSKT_DonViController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_DonVi.xls";
        private const string _filePath_A4 = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_DonVi_A4.xls";
        private const string _filePath_A4_HienVat = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_DonVi_A4_HienVat.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban;
        private string id_donvi;
        private int data;
        private int dvt;
        private string page;

        public ActionResult Index()
        {

            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa");
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptSKT_DonViViewModel()
            {
                PhongBanList = phongbanList,
                DonViList = donviList,
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptSKT_DonVi.cshtml", vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="loai">1: NSSD, 2: NSBĐ ngành</param>
        /// <param name="data">1: Chi bằng tiền, 2: Hiện vật</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(string id_phongban, string id_donvi, int data = 1, string ext = "pdf", string page = null, int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly
            if (id_donvi.IsEmpty())
                id_donvi = PhienLamViec.iID_MaDonVi;

            this.id_phongban = id_phongban;
            this.id_donvi = id_donvi;
            this.page = page;
            this.data = data;
            this.dvt = dvt;

            var xls = createReport();


            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
              ? string.Empty
              : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            //var filename = $"BC_NhuCau_{(donvi.IsEmpty() ? id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_{DateTime.Now.GetTimeStamp()}.{ext}";
            var filename = $"{(donvi.IsEmpty() ? id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_BC_NhuCau_{DateTime.Now.GetTimeStamp()}.{ext}";
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

            #region get filename

            string fileName = String.Empty;

            if (data == 1)
            {
                fileName = page.IsEmpty() ? _filePath : _filePath_A4;
            }
            else
            {
                fileName = page.IsEmpty() ? _filePath : _filePath_A4_HienVat;
            }
            xls.Open(Server.MapPath(fileName));


            #endregion


            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
                ? string.Empty
                : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            fr.SetValue(new
            {
                h1 = $"Đơn vị tính: {dvt.ToStringNumber()}đ",
                h2 = donvi,
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.ToStringNgay(),
                NamTruoc = int.Parse(PhienLamViec.iNamLamViec) - 2,
                NamNay = int.Parse(PhienLamViec.iNamLamViec) - 1,
                Nam = int.Parse(PhienLamViec.iNamLamViec),

                Cap1 = "Cục Tài chính",
                Cap2 = id_phongban.IsEmpty() ? "" : _nganSachService.GetPhongBanById(id_phongban).sMoTa,
                DonVi = donvi.IsEmpty() ? "(Tổng hợp đơn vị)" : $"Đơn vị: {donvi}",
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getTable();
            _SKTService.FillDataTable_SKT(fr, data);
        }

        private DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("rpt_skt_donvi.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = id_phongban.ToParamString(),
                    id_donvi = id_donvi.ToParamString(),
                    dvt,
                });

                // filter row
                if (data == 2)
                {
                    var rows = dt.AsEnumerable().Where(r =>
                        r.Field<double>("TonKho_DV") == 0 &&
                        r.Field<double>("HuyDong_DV") == 0 &&
                        r.Field<double>("HuyDong") == 0 &&
                        r.Field<double>("HuyDong_B2") == 0
                        ).ToList();

                    rows.ForEach(r => dt.Rows.Remove(r));
                }

                return dt;
            }
        }

        private string getTieuDe()
        {
            var tieude = $"Phương án phân bổ số kiểm tra ngân sách năm {PhienLamViec.iNamLamViec} - {(data == 1 ? "Chi bằng tiền" : "Hiện vật")}";

            return tieude;
        }

        #endregion
    }
}
