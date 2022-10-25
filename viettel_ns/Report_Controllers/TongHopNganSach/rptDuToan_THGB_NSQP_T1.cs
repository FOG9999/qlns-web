﻿using FlexCel.Core;
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
    public class rptDuToan_THGB_NSQP_T1Controller : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        //private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptDuToan_THGB_NSQP_T1_A4n.xls";
        private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptDuToan_THGB_NSQP_T1.xls";

        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(string ext,
            string nam,
            string iID_MaPhongBan = null)
        {
            HamChung.Language();
            var xls = createReport(nam, iID_MaPhongBan);
            return Print(xls, ext);
        }

        #endregion

        private ExcelFile createReport(string nam, string iID_MaPhongBan = null)
        {
            var xls = new XlsFile(true);

            xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.Internal, new ToPercent());

            xls.Open(Server.MapPath(sFilePath));


            var fr = new FlexCelReport();

            //var cacheKey = new[] { this.ControllerName(), nam, Username, iID_MaPhongBan }.Join("_");
            //var data = CacheService.Default.CachePerRequest(cacheKey,
            //    () => getNSQP(nam, iID_MaPhongBan),
            //    Viettel.Domain.DomainModel.CacheTimes.OneMinute);

            var data = getNSQP(nam, iID_MaPhongBan);

            FillDataTableLNS(fr, data, "sLNS1 sLNS3 sLNS5 sLNS", nam);

            var tenPhongBan = new NganSachService().GetPhongBan(Username).sMoTa.ToUpper();
            var donvi = _nganSachService.GetDonVi("2019", Request.GetQueryString("id_donvi"));
            var dvt = Request.GetQueryStringValue("dvt");

            xls.Recalc();

            fr.SetValue("Nam", nam);
            fr.SetValue("TenPhongBan", tenPhongBan);
            fr.SetValue("DonVi", donvi.sTen);
            fr.SetValue("h1", $"Đơn vị tính: {dvt.ToStringDvt()}");
            fr.UseCommonValue()
              .UseForm(this)
              .UseChuKy()
              .Run(xls);

            return xls;
        }

        private DataTable getNSQP(string nam, string iID_MaPhongBan = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_THGB_NSQP_T1.sql");

            var id_donvi = _nganSachService.GetDonviByPhongBanId(nam, iID_MaPhongBan)
                .Select(x => x.iID_MaDonVi)
                .Join();

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                //cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                //cmd.Parameters.AddWithValue("@iID_MaDonVi", id_donvi);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", Request.GetQueryStringValue("id_donvi"));
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", Request.GetQueryStringValue("id_phongban"));
                cmd.Parameters.AddWithValue("@dvt", Request.GetQueryStringValue("dvt", 1));

                var dt = cmd.GetTable();
                return dt;
            }

            #endregion  
        }
    }
}
