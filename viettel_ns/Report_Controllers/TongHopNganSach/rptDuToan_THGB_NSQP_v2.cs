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
using VIETTEL.Application.Flexcel;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_THGB_NSQP_v2Controller : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        //private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptDuToan_THGB_NSQP_v2.xls";

        // long sua
        private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptDuToan_THGB_NSQP_v4.xls";

        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(
            string nam,
            string iID_MaPhongBan = null,
            string ext = "xls")
        {
            HamChung.Language();
            var xls = createReport(nam, iID_MaPhongBan);
            return Print(xls, ext);
        }

        #endregion

        private ExcelFile createReport(string nam, string iID_MaPhongBan = null)
        {
            var xls = new XlsFile(true);

            xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.External, new ToPercent());
            //xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.Internal, new IsPrime());

            xls.Open(Server.MapPath(sFilePath));

            var fr = new FlexCelReport();

            var data = getNSQP(nam, iID_MaPhongBan);

            //FillDataTableLNSTemp(fr, data, nam);
            FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM,sNG", PhienLamViec.iNamLamViec);

            var tenPhongBan = new NganSachService().GetPhongBan(Username).sMoTa.ToUpper();

            xls.Recalc(true);

            fr.SetValue("Nam", nam);
            fr.SetValue("TenPhongBan", tenPhongBan);
            fr.UseCommonValue()
              .UseForm(this)
              .UseChuKyForController(this.ControllerName())
              .Run(xls);

            return xls;
        }

        private DataTable getNSQP(string nam, string iID_MaPhongBan = null)
        {
            var sql = FileHelpers.GetSqlQuery("rpt_TongHop_THGB_Ng.sql"); ;
            var dkDonvi = PhienLamViec.DonViList.Select(x => x.Key).Join();

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", dkDonvi);

                var dt = cmd.GetTable();
                return dt;
            }

            #endregion  
        }
    }
}
