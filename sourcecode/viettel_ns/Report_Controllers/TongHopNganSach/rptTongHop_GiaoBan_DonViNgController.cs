using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Application.Flexcel;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptTongHop_GiaoBan_DonViNgViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.TongHopNganSach
{
    public partial class rptTongHop_GiaoBan_DonViNgController : FlexcelReportController
    {
        private string _viewPath = "~/Views/Report_Views/TongHopNganSach/";

        public ActionResult Index()
        {

            var vm = new rptTongHop_GiaoBan_DonViNgViewModel()
            {
                iNamLamViec = PhienLamViec.iNamLamViec,
                DonViList = PhienLamViec.DonViList.ToSelectList(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

    }

    public partial class rptTongHop_GiaoBan_DonViNgController : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_GiaoBan_DonVi_Ng.xls";

        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(string iID_MaDonVi, string iNamLamViec, string iID_MaPhongBan, string ext = "pdf")
        {
            iNamLamViec = iNamLamViec ?? ReportModels.LayNamLamViec(Username);
            var xls = createReport(iNamLamViec, iID_MaDonVi, iID_MaPhongBan);
            return Print(xls, ext);
        }

        #endregion

        private ExcelFile createReport(string nam, string iID_MaDonVi, string iID_MaPhongBan)
        {
            var xls = new XlsFile(true);
            xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.Internal, new ToPercent());

            xls.Open(Server.MapPath(sFilePath));
            var fr = new FlexCelReport();
            //fr.SetUserFunction("ToPercent", new TToPercentImpl(xls));

            iID_MaPhongBan = iID_MaPhongBan ?? PhienLamViec.iID_MaPhongBan;

            var data = getDataTable(nam, iID_MaDonVi, iID_MaPhongBan);
            FillDataTable_NG(fr, data, nam);

            var tenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, nam);
            xls.Recalc(true);

            fr.SetValue("Nam", nam);
            fr.SetValue("TenDonVi", tenDonVi);
            fr.SetValue("TenPhongBan", PhienLamViec.sTenPhongBan);
            fr.UseCommonValue()
              .UseForm(this)
              .UseChuKy()
              .Run(xls);

            return xls;
        }

        private DataTable getDataTable(string nam, string iID_MaDonVi, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("rpt_TongHop_THGB_Ng.sql");
            if (iID_MaPhongBan == "02" || iID_MaPhongBan == "11")
            {
                iID_MaPhongBan = "-1";
            }
            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("iNamLamViec", nam);
                cmd.Parameters.AddWithValue("iID_MaDonVi", iID_MaDonVi);

                var dt = cmd.GetTable();
                return dt;
            }

            #endregion
        }

    }

}
