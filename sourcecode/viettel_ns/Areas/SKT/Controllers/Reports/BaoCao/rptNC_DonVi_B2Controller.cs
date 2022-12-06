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

    //public class rptNC_DonViViewModel : ReportModels
    //{
    //    public SelectList PhongBanList { get; set; }
    //    public SelectList DonViList { get; set; }
    //    public SelectList ToList { get; set; }
    //}

    public class rptNC_DonVi_B2Controller : FlexcelReportController
    {
        //private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_B2.xls";
        //private const string _filePath_DonVi = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_DonVi_B2.xls";
        //private const string _filePath_TongHop = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_TongHop_B2.xls";

        private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_B2_BuTru.xls";
        private const string _filePath_DonVi = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_DonVi_B2_BuTru.xls";
        private const string _filePath_TongHop = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_TongHop_B2_BuTru.xls";


        //private const string _filePath_ChiTiet_TrinhKy = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_TrinhKy.xls";

        // báo kiểm 
        private const string _filePath_ChiTiet_TrinhKy = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_BieuKiem.xls";

        // phu luc trinh bo
        private const string _filePath_ChiTiet_PhuLuc = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_PhuLuc.xls";




        private const string _filePath_GhiChu = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_B2_GhiChu.xls";
        private const string _filePath_DonVi_a4 = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_DonVi_DonVi_B2_a4.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban_ns = "06,07,08,10";
        private string id_phongban;
        private string id_donvi;
        private string loaiBaoCao;
        private int dvt;

        public ActionResult Index()
        {

            var phongbanList = _nganSachService.GetPhongBanQuanLyNS(id_phongban_ns).ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "<- TOÀN CỤC ->");
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptNC_DonViViewModel()
            {
                PhongBanList = phongbanList,
                DonViList = donviList,
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptNC_DonVi_B2.cshtml", vm);
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
            //if (id_donvi.IsEmpty())
            //    id_donvi = PhienLamViec.iID_MaDonVi;


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
            if (loaiBaoCao == "chitiet")
            {
                filename = _filePath_GhiChu;
            }
            else if (loaiBaoCao == "trinhky")
            {
                filename = _filePath_ChiTiet_TrinhKy;

            }
            else if (loaiBaoCao == "phuluc") filename = _filePath_ChiTiet_PhuLuc;
            else if (loaiBaoCao == "tonghop-donvi") filename = _filePath_DonVi;
            else if (loaiBaoCao == "tonghop-donvi-a4") filename = _filePath_DonVi_a4;
            else if (loaiBaoCao == "tonghop-all") filename = _filePath_TongHop;


            //var filename = loaiBaoCao == "tonghop-donvi" ?
            //  _filePath_DonVi :
            //  (loaiBaoCao == "tonghop-all" ? _filePath_TongHop : _filePath);

            xls.Open(Server.MapPath(filename));

            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
                ? string.Empty
                : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            var PhongBan = id_phongban.IsEmpty("-1")
                ? "(Tổng hợp)"
                : _nganSachService.GetPhongBanById(id_phongban).sTen;


            fr.SetValue(new
            {
                h1 = $"Đơn vị tính: {dvt.ToStringNumber()}đ ",
                h2 = donvi.IsEmpty() ? PhongBan : $"{PhongBan} - {donvi}",
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.ToStringNgay(),
                NamTruoc = int.Parse(PhienLamViec.iNamLamViec) - 2,
                NamNay = int.Parse(PhienLamViec.iNamLamViec) - 1,
                Nam = int.Parse(PhienLamViec.iNamLamViec),

                Cap1 = "Cục Tài chính",
                //Cap2 = _nganSachService.GetPhongBanById("02").sMoTa,
                Cap2 = id_phongban.IsEmpty("-1") ? "" : _nganSachService.GetPhongBanById(id_phongban).sMoTa,

                PhongBan = id_phongban.IsEmpty("-1") ? "(Tổng hợp cục)" : _nganSachService.GetPhongBanById(id_phongban).sMoTa,
                DonVi = donvi.IsEmpty() ? "(Tổng hợp đơn vị)" : $"Đơn vị: {donvi}",
            });

            fr.UseCommonValue()
                .UseForm(this)
                .UseChuKy()
                .Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getTable();

            if (loaiBaoCao == "chitiet" || loaiBaoCao == "nganh")
                _SKTService.FillDataTable_NC(fr, data);
            else
                _SKTService.FillDataTable_NC(fr, data, fields: "X1 X2 X3 X4 KyHieu");


            //if (loaiBaoCao == "chitiet")
            //fr.AddTable("dtGhiChu", getTable_GhiChu(id_phongban, id_donvi));



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
            //var sql = loaiBaoCao == "chitiet" ?
            //    FileHelpers.GetSqlQuery("rpt_nc_donvi_b2.sql") :
            //    (loaiBaoCao == "nganh" ? FileHelpers.GetSqlQuery("rpt_nc_donvi_nganh.sql") : FileHelpers.GetSqlQuery("rpt_nc_donvi_sum.sql"));

            var sql = string.Empty;
            var kyhieu = string.Empty;

            if (loaiBaoCao == "chitiet")
            {
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_b2.sql");
            }
            else if (loaiBaoCao == "trinhky")
            {
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_b2_sum.sql");
            }
            else if (loaiBaoCao == "nganh")
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_nganh.sql");
            else if (loaiBaoCao == "tonghop-all" || loaiBaoCao == "tonghop-donvi"
                || loaiBaoCao == "phuluc")
            {
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_b2_sum.sql");
            }
            else if (loaiBaoCao == "tonghop-donvi-a4")
            {
                sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_b2_sum.sql");
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
                    id_phongban = (id_phongban.IsEmpty("-1") ? id_phongban_ns : id_phongban).ToParamString(),
                    id_phongban_dich = (id_phongban.IsEmpty("-1") ? id_phongban_ns : id_phongban).ToParamString(),
                    id_donvi = id_donvi.ToParamString(),
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
                    id_phongban = PhienLamViec.iID_MaPhongBan,
                    id_phongbandich = id_phongban.ToParamString(),
                    id_donvi = id_donvi.ToParamString(),
                    loai = 1,
                });
                return dt;
            }
        }

        private string getTieuDe()
        {
            var tieude = $"Tổng hợp số kiểm tra dự toán ngân sách năm {PhienLamViec.iNamLamViec}";

            return tieude;
        }

        #endregion
    }
}
