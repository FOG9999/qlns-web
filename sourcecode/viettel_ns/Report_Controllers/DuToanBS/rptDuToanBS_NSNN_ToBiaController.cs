using DomainModel;
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
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptDuToanBS_207_ToBiaViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList DotList { get; set; }
        public string TieuDe { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToanBS_NSNN_ToBiaController : FlexcelReportController
    {

        public string _viewPath = "~/Views/Report_Views/DuToanBS/";
        private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_ToBia.xls";
        private const string _filePath_trinhky = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_ToBia_TrinhKy.xls";

        private readonly INganSachService _nganSachService = NganSachService.Default;

        #region ctor

        public ActionResult Index()
        {

            //var dtDot = DuToanBS_ReportModels.LayDSDot(_namLamViec, Username, "207");
            var dtDot = DuToanBsService.Default.GetDots_NSNN(PhienLamViec.iNamLamViec, Username);

            var vm = new rptDuToanBS_207_ToBiaViewModel
            {
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
            int trinhky = 1)
        {
            HamChung.Language();
            var xls = createReport(iID_MaDot, iID_MaNganh, trinhky);
            return Print(xls, ext);
        }

        public JsonResult Ds_Nganh(string iID_MaDot)
        {
            //var data = DuToanBS_ReportModels.dtNganh_NhaNuoc(iID_MaDot, Username);

            var data = DuToanBsService.Default.GetNganh_NSNN(PhienLamViec.iNamLamViec, iID_MaDot, Username);
            var vm = new ChecklistModel("Nganh", data.ToSelectList("iID_MaNganh", "sTenNganh"));
            return ToCheckboxList(vm);
        }

        #endregion

        private ExcelFile createReport(string iID_MaDot, string iID_MaNganh, int trinhky = 1)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));


            var fr = new FlexCelReport();
            var data = getDataTable(iID_MaDot, iID_MaNganh);
            fr.FillDataTable_4(data);

            string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            var tenNganh = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);

            fr.SetExpression("NgayDot", iID_MaDot.ToParamDate().ToStringNgay().ToLower());
            fr.SetValue("TenNganh", tenNganh);

            fr.UseCommonValue()
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            return xls;
        }

        private DataTable getDataTable(string iID_MaDot, string iID_MaNganh)
        {
            #region sql

            var sql = @"

 SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        rTuChi      =SUM(rTuChi)/@dvt,
        rHangNhap   =SUM(rHangNhap)/@dvt,
        rHangMua    =SUM(rHangMua)/@dvt,
        rTonKho     =SUM(rTonKho)/@dvt,
        rPhanCap    =SUM(rPhanCap)/@dvt,
        rDuPhong    =SUM(rDuPhong)/@dvt 
FROM    DTBS_ChungTuChiTiet
WHERE   iTrangThai=1  
        AND sLNS LIKE '207%' 
        AND iNamLamViec=@iNamLamViec
        AND sNG IN (SELECT * FROM F_Split(@sNG))
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangMua) <>0
        OR SUM(rHangNhap) <>0
        OR SUM(rTonkho) <>0
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0

";
            #endregion

            var sNG = "";
            if (iID_MaNganh == "51")
                sNG = "41,44";
            else if (iID_MaNganh == "52")
                sNG = "33,43,40,10,36";

            var iID_MaChungTu = DuToanBS_ReportModels.GetChungTuList_TheoDot(iID_MaDot, Username);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@sNG", sNG);
                    cmd.Parameters.AddWithValue("@dvt", 1000);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }
    }
}
