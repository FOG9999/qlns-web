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
    public class rptTongHop_ChiTieu_DonViViewModel
    {
        public string iNamLamViec { get; set; }
        public string iID_MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }

        public SelectList DotList { get; set; }
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.TongHopNganSach
{
    public class rptTongHop_ChiTieu_DonViController : FlexcelReportController
    {
        #region ctor

        private const string _viewPath = "~/Views/Report_Views/TongHopNganSach/";
        //private const string _filePath = "~/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_DonVi.xls";
        private const string _filePath = "~/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_DonVi2.xls";
        private string _namLamViec = DateTime.Now.Year.ToString();
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _dutoanbsService = DuToanBsService.Default;

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            _namLamViec = ReportModels.LayNamLamViec(Username);

            //var dtDot = DuToanBS_ReportModels.LayDSDot(_namLamViec, Username, "");
            //var phongban = _nganSachService.GetPhongBan(Username);

            var dotList = _dutoanbsService.GetDots_Gom(PhienLamViec.iNamLamViec, Username)
                .ToSelectList("dDotNgay", "sMoTa", DateTime.Now.ToStringDate(), "-- Tới hiện tại --");

            var vm = new rptTongHop_ChiTieu_DonViViewModel
            {
                iNamLamViec = _namLamViec,
                iID_MaPhongBan = PhienLamViec.iID_MaPhongBan,
                TenPhongBan = PhienLamViec.sTenPhongBanFull,

                DotList = dotList,
                DonViList = PhienLamViec.DonViList.ToSelectList(),
            };

            return View(view, vm);
        }

        #endregion

        #region properties

        #endregion

        #region reports

        public ActionResult Print(
            string iID_MaDot,
            string iID_MaDonVi,
            string iID_MaPhongBan,
            string loaiBaoCao,
            string namNganSach,
            string ext = "pdf")
        {
            HamChung.Language();

            var iNamLamViec = ReportModels.LayNamLamViec(Username);
            var xls = createReport(iNamLamViec, iID_MaDot, iID_MaDonVi, iID_MaPhongBan, loaiBaoCao, namNganSach);
            return Print(xls, ext);
        }

        #endregion

        #region public methods

        public JsonResult Ds_DonVi(
            string iID_MaDot,
            string iID_MaPhongBan,
            string iID_MaDonVi)
        {
            var data = NganSachService.Default.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);
            return ToCheckboxList(new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sMoTa")));
        }

        #endregion

        #region private methods

        private ExcelFile createReport(
            string iNamLamViec,
            string iID_MaDot,
            string iID_MaDonVi,
            string iID_MaPhongBan,
            string loaiBaoCao,
            string namNganSach)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));
            var fr = new FlexCelReport();


            if (string.IsNullOrWhiteSpace(iID_MaDot) || iID_MaDot == "-1")
                iID_MaDot = DateTime.Now.ToStringDate();

            var data = getDataTable(iNamLamViec, iID_MaDot, iID_MaDonVi, iID_MaPhongBan, loaiBaoCao, namNganSach);
            FillDataTableLNS(fr, data, FillLNSType.LNS1, iNamLamViec);


            var tenDonVi = string.IsNullOrWhiteSpace(iID_MaDonVi) || iID_MaDonVi == "-1" || iID_MaDonVi == "02" ?
                "Tất cả các đơn vị" :
                DonViModels.Get_TenDonVi(iID_MaDonVi, iNamLamViec);

            fr.SetValue("TenDonVi", tenDonVi);
            fr.SetValue("MaDot", iID_MaDot);
            fr.SetValue("TongTienBangChu", "tong tien bang chu");

            var mota = string.Empty;

            namNganSach = namNganSach.StartsWith("1") ?
                      "Ngân sách năm trước" :
                      "Ngân sách năm nay";
            fr.SetValue("NamNganSach", namNganSach);

            var dvt = Request.GetQueryStringValue("dvt", 1000);
            fr.UseCommonValue(new Application.Flexcel.FlexcelModel()
            {
                dvt = dvt,
            })
              .UseChuKy(Username, iID_MaPhongBan)
              .UseForm(this)
              .Run(xls);

            return xls;

        }

        private DataTable getDataTable(
          string iNamLamViec,
          string iID_MaDot,
          string iID_MaDonVi,
          string iID_MaPhongBan,
          string loaiBaoCao,
          string namNganSach)
        {
            var dvt = Request.GetQueryStringValue("dvt", 1000);
            var sql = FileHelpers.GetSqlQuery(@"rpt_TongHopChiTieu_DonVi.sql");

            #region load data

            if (iID_MaPhongBan == "02")
                iID_MaPhongBan = string.Empty;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", namNganSach.ToParamString());
                    cmd.Parameters.AddWithValue("@username", Username);
                    cmd.Parameters.AddWithValue("@loai", loaiBaoCao.ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", dvt);
                    cmd.Parameters.AddWithValue("@dMaDot", iID_MaDot.ToParamDateVN());

                    var dt = cmd.GetTable();
                    return dt;
                }
            }

            #endregion
        }

        #endregion

    }
}
