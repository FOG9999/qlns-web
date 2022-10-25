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
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{

    public class rptNC_DonViViewModel : ReportModels
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
        public SelectList ToList { get; set; }
    }

    public class rptNC_DonViController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi.xls";
        private const string _filePath_GhiChu = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_GhiChu.xls";
        private const string _filePath_DonVi = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_DonVi.xls";
        private const string _filePath_TongHop = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_TongHop.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban;
        private string id_donvi;
        private string loaiBaoCao;
        private int dvt;

        public ActionResult Index()
        {

            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa");
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptNC_DonViViewModel()
            {
                PhongBanList = phongbanList,
                DonViList = donviList,
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptNC_DonVi.cshtml", vm);
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
        public ActionResult Print(string id_phongban, string id_donvi, string loaiBaoCao = "chitiet", string ext = "pdf", int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly
            if (id_donvi.IsEmpty())
                id_donvi = PhienLamViec.iID_MaDonVi;


            this.id_phongban = id_phongban;
            this.id_donvi = id_donvi;
            this.loaiBaoCao = loaiBaoCao;
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
            //var filename = loaiBaoCao == "donvi" ?
            //    _filePath_DonVi :
            //    (loaiBaoCao == "tonghop" ? _filePath_TongHop : _filePath);

            var filename = _filePath;
            if (loaiBaoCao == "chitiet") filename = _filePath_GhiChu;
            else if (loaiBaoCao == "tonghop-00-donvi") filename = _filePath_DonVi;
            else if (loaiBaoCao == "tonghop-00") filename = _filePath_TongHop;

            xls.Open(Server.MapPath(filename));

            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
                ? string.Empty
                : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            fr.SetValue(new
            {
                h1 = $"Đơn vị tính: {dvt.ToStringNumber()}đ ",
                h2 = donvi,
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.ToStringNgay(),
                NamTruoc = int.Parse(PhienLamViec.iNamLamViec) - 2,
                NamNay = int.Parse(PhienLamViec.iNamLamViec) - 1,

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

            if (loaiBaoCao == "chitiet" || loaiBaoCao == "tonghop-nganh")
                _SKTService.FillDataTable_NC(fr, data);
            else
                _SKTService.FillDataTable_NC(fr, data, fields: "X1 X2 X3 X4 KyHieu");


            if (loaiBaoCao == "chitiet")
            {
                var ghiChu = getTable_GhiChu(id_phongban, id_donvi);
                var dtNganhGhiChu = ghiChu.SelectDistinct("Nganh", "Nganh,sMoTa");
                fr.AddTable("dtGhiChu", ghiChu);
                fr.AddTable("Nganh", dtNganhGhiChu);
                fr.AddRelationship("Nganh", "dtGhiChu", "Nganh".Split(','), "Nganh".Split(','));
            }
        }

        private DataTable getTable()
        {
            var sql = string.Empty;
            var kyhieu = string.Empty;
            if (loaiBaoCao == "chitiet")
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi.sql");
            else if (loaiBaoCao == "tonghop-nganh")
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_nganh.sql");
            else if (loaiBaoCao == "tonghop")
            {
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_all.sql");
            }
            else
            {
                kyhieu = loaiBaoCao.StartsWith("tonghop-00") ? "1-1%" : null;
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_sum.sql");
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = id_phongban.ToParamString(),
                    id_donvi = id_donvi.ToParamString(),
                    kyhieu = kyhieu.ToParamString(),
                    dvt,
                });
                return dt;
            }
        }

        private DataTable getTable_GhiChu(string id_phongban, string id_donvi)
        {
            var sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_ghichu.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = id_phongban.ToParamString(),
                    id_phongbandich = id_phongban.ToParamString(),
                    id_donvi = id_donvi.ToParamString(),
                    loai = 1,
                });
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
