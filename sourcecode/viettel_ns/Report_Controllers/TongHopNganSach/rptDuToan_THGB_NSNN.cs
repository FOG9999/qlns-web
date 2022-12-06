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
using VIETTEL.Application.Flexcel;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_THGB_NSNNController : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptDuToan_THGB_NSNN.xls";

        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(string nam, string iID_MaPhongBan, string ext)
        {
            var xls = createReport(nam, iID_MaPhongBan);
            return Print(xls, ext);
        }

        #endregion

        private ExcelFile createReport(string nam, string iID_MaPhongBanDich = null)
        {
            var xls = new XlsFile(true);
            xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.Internal, new ToPercent());
            xls.Open(Server.MapPath(sFilePath));

            var fr = new FlexCelReport();

            //fillDataTable(fr, nam, iID_MaPhongBanDich);
            //var data = CacheService.Default.CachePerRequest(this.ControllerName(),
            //    () => getDataTable(nam, iID_MaPhongBanDich),
            //    Viettel.Domain.DomainModel.CacheTimes.OneMinute);
            var data = getNSNN(nam, iID_MaPhongBanDich);

            FillDataTableLNS(fr, data, "sLNS1", nam);

            var tenPhongBan = new NganSachService().GetPhongBan(Username).sMoTa.ToUpper();

            xls.Recalc();

            fr.SetValue("Nam", nam);
            fr.SetValue("TenPhongBan", tenPhongBan);
            fr.UseCommonValue()
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        private DataTable getNSNN(string nam, string iID_MaPhongBan = null)
        {
            //var sql = FileHelpers.GetSqlQuery("rptDuToan_THGB_NSNN.sql");

            // long sua loi ko in duoc bao cao cho B7, ngay 17/4
            var sql = FileHelpers.GetSqlQuery("rptDuToan_THGB_NSNN_v3.sql"); ;


            var id_donvi = _nganSachService.GetDonviByPhongBanId(nam, iID_MaPhongBan)
               .Select(x => x.iID_MaDonVi)
               .Join();


            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", _nganSachService.CheckParam_PhongBan(iID_MaPhongBan).ToParamString());
                cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", id_donvi);

                var dt = Connection.GetDataTable(cmd);
                return dt;
            }
        }
    }
}
