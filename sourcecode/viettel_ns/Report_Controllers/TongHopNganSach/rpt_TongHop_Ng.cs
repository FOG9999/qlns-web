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
    public class rpt_TongHop_NgController : FlexcelReportController
    {
        #region var def
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rpt_TongHop_Ng.xls";

        private string namLV;
        private string iID_MaPhongBan;
        private string sLNS;

        #endregion

        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(
            string sLNS,
            string ext)
        {
            this.namLV = PhienLamViec.iNamLamViec;
            this.iID_MaPhongBan = _nganSachService.GetPhongBan(Username).sKyHieu;
            this.sLNS = sLNS;

            var xls = createReport();
            return Print(xls, ext);
        }

        #endregion

        private ExcelFile createReport()
        {
            var xls = new XlsFile(true);

            xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.Internal, new ToPercent());
            xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.Internal, new IsPrime());

            xls.Open(Server.MapPath(sFilePath));

            var fr = new FlexCelReport();

            LoadData(fr);

            var tenPhongBan = new NganSachService().GetPhongBan(Username).sMoTa.ToUpper();

            xls.Recalc();

            fr.SetValue("Nam", namLV);
            fr.SetValue("TenPhongBan", tenPhongBan);
            fr.SetValue("Name", "Bảng số liệu quý I/" + namLV);
            fr.UseCommonValue()
              .UseForm(this)
              .Run(xls);

            return xls;
        }

        private void LoadData(FlexCelReport fr)
        {
            #region get data

            DataSet ds = new DataSet();
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                ds = AdonetHelper.ExecuteSelect(conn, "[dbo].[sql_nsach_chitieu_lns]",
                    new
                    {
                        iNamLamViec = namLV,
                        iID_MaPhongBan,
                        sLNS
                    }
                );
            }

            #endregion

            #region fill data to flex

            fr.AddTable("dtM", ds.Tables[0]);
            fr.AddTable("dtTM", ds.Tables[1]);
            fr.AddTable("dtNG", ds.Tables[2]);
            fr.AddTable("dtDonVi", ds.Tables[3]);
            fr.AddTable("dtChiTiet", ds.Tables[4]);
            ds.Dispose();

            #endregion
        }
    }
}
