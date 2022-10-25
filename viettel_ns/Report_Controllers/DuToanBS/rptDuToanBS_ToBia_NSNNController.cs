using Dapper;
using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;


namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToanBS_ToBia_NSNNController : FlexcelReportController
    {

        public string _viewPath = "~/Views/Report_Views/DuToanBS/";
        //private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_ToBia.xls";
        private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_ToBia2.xls";
        private const string _filePath_nsk = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ToBia_nsk.xls";

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _bsService = DuToanBsService.Default;

        private string _tenPhuLuc;

        #region ctor

        public ActionResult Index()
        {
            //var dtDot = DuToanBS_ReportModels.LayDSDot(_namLamViec, Username, "207");
            var dtDot = DuToanBsService.Default.GetDots_NSNN(PhienLamViec.iNamLamViec, Username);

            var vm = new rptDuToanBS_207_ToBiaViewModel
            {
                TieuDe = "Giao dự toán ngân sách nhà nước",
                iNamLamViec = PhienLamViec.iNamLamViec,
                DotList = dtDot.ToSelectList("iID_MaDot", "iID_MaDot", "-1", "-- Chọn đợt --"),
            };


            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods


        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult Print(
            string ext,
            string iID_MaDot,
            string iID_MaNganh,
            string tenPhuLuc = "Giao dự toán ngân sách",
            int trinhky = 1)
        {
            HamChung.Language();
            _tenPhuLuc = tenPhuLuc;
            var xls = createReport(iID_MaDot, iID_MaNganh, trinhky);
            return Print(xls, ext);
        }

        public JsonResult Ds_Nganh(string iID_MaDot)
        {
            var data = _bsService.GetNganh_NSNN(PhienLamViec.iNamLamViec, iID_MaDot, Username);
            var vm = new ChecklistModel("Nganh", data.ToSelectList("iID_MaNganh", "sTenNganh"));
            return ToCheckboxList(vm);
        }

        #endregion

        private ExcelFile createReport(string iID_MaDot, string iID_MaNganh, int trinhky = 1)
        {
            var xls = new XlsFile(true);

            var file = getXlsFile(iID_MaDot);
            xls.Open(file);


            var fr = new FlexCelReport();
            var data = getDataTable(iID_MaDot, iID_MaNganh);
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);

            //string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
            //SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            //cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            //DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            //var tenNganh = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);

            fr.SetExpression("NgayDot", iID_MaDot.ToParamDate().ToStringNgay().ToLower());
            fr.SetExpression("TieuDe1", _tenPhuLuc + $" năm {PhienLamViec.iNamLamViec}");

            var totalSum = data.AsEnumerable().Sum(x => x.Field<decimal>("rTongSo")) * 1000;

            //In loại tiền bằng chữ
            var bangChu = "";
            if (totalSum < 0)
            {
                totalSum *= -1;
                bangChu = "Giảm " + totalSum.ToStringMoney().ToLower();
            }
            else
            {
                bangChu = totalSum.ToStringMoney();
            }

            fr.SetValue("Tien", bangChu);
            //fr.SetValue("TenNganh", tenNganh);

            fr.UseCommonValue()
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;
        }

        private string getXlsFile(string iID_MaDot)
        {
            var file = _filePath;

            var lns = getLnsTheoDot(iID_MaDot);
            if (lns.StartsWith("4"))
            {
                file = _filePath_nsk;
            }

            return Server.MapPath(file);

        }

        private string getLnsTheoDot(string iID_MaDot)
        {
            var sql = @"

select sDSLNS from DTBS_ChungTu
where   iTrangThai=1
        and iNamLamViec=@iNamLamViec
        and sID_MaNguoiDungTao=@username
        and dNgayChungTu=@dNgayChungTu
        and iID_MaPhongBanDich=@iID_MaPhongBan

";

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var value = conn.QueryFirst<string>(
                    sql,
                    param: new
                    {
                        iNamLamViec = PhienLamViec.iNamLamViec,
                        iID_MaPhongBan = PhienLamViec.iID_MaPhongBan,
                        username = Username,
                        dNgayChungTu = iID_MaDot.ToParamDate(),
                    },
                    commandType: CommandType.Text);

                return value;
            }
        }

        private DataTable getDataTable(string iID_MaDot, string iID_MaNganh)
        {
            #region sql

            var sql = FileHelpers.GetSqlQuery("dtbs_tobia_nsnn.sql");

            #endregion

            //var sNG = "";
            ////if (iID_MaNganh == "51")
            ////    sNG = "41,44";
            ////else if (iID_MaNganh == "52")
            ////    sNG = "33,43,40";

            ////if (iID_MaNganh == "51")
            ////    sNG = "41,44";
            ////else if (iID_MaNganh == "52")
            ////    sNG = "33,43,40,30,31,32,35,36,37,38,39,45,46,47,10";

            //string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
            //SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            //cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            //DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            //sNG = dtNganhChon.Rows.Count > 0 ? Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]) : "";


            //var iID_MaChungTu = DuToanBS_ReportModels.GetChungTuList_TheoDot(iID_MaDot, Username);
            var iID_MaChungTu = _bsService.GetChungTus_DotNgay_NSNN(iID_MaDot, Username, PhienLamViec.iID_MaPhongBan).Join();


            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", PhienLamViec.iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@sLNS", "2,4");
                    cmd.Parameters.AddWithValue("@iID_MaNganh", "".ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", 1000);
                    cmd.Parameters.AddWithValue("@dot", iID_MaDot.ToDateTime());

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }
    }
}
