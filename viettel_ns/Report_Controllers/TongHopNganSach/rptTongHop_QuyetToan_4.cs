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

namespace VIETTEL.Report_Controllers
{
    public class rptTongHop_QuyetToan_4Controller : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_QuyetToan_4.xls";
        private int _dvt = 1000;
        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(string ext,
            string iID_MaPhongBan = null)
        {
            HamChung.Language();
            var xls = createReport(iID_MaPhongBan);
            return Print(xls, ext);
        }

        #endregion

        private ExcelFile createReport(string iID_MaPhongBan = null)
        {
            var xls = new XlsFile(true);

            xls.Open(Server.MapPath(sFilePath));


            var fr = new FlexCelReport();

            var data = getData(iID_MaPhongBan);

            //FillDataTableLNS(fr, data, "sLNS1 sLNS3 sLNS5 sLNS", nam);

            var tenPhongBan = new NganSachService().GetPhongBanById(iID_MaPhongBan).sMoTa.ToUpper();
            fr.AddTable("ChiTiet", data);

            fr.SetValue("TenPhongBan", tenPhongBan);
            fr.SetValue(new
            {
                h1 = $"Đơn vị tính: {_dvt.ToStringDvt()}"
            })
            .UseCommonValue()
              .UseForm(this)
              .UseChuKy()
              .Run(xls);

            return xls;
        }

        private DataTable getData(string iID_MaPhongBan = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptTongHop_QuyetToan_4.sql");

            var id_donvi = _nganSachService.GetDonviByPhongBanId(PhienLamViec.iNamLamViec, iID_MaPhongBan == "-1" && PhienLamViec.iID_MaPhongBan == "02" ? "-1" : PhienLamViec.iID_MaPhongBan)
                .Select(x => x.iID_MaDonVi)
                .Join();

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    iID_MaPhongBan,
                    iID_MaDonVi = id_donvi,
                    dvt = _dvt
                });
                return dt;
            }

            #endregion  
        }
    }
}
