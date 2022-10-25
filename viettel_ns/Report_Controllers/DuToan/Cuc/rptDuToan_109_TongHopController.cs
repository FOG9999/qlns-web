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
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_109_TongHopController : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_109_TongHop.xls";
        private const String sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_109_TongHop_TrinhKy.xls";


        public ActionResult Index(int trinhky = 0)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_109_TongHop.aspx";
                ViewData["PageLoad"] = "0";
                ViewData["trinhky"] = trinhky;
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        /// <summary>
        /// hàm lấy các giá trên form
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public ActionResult EditSubmit(String ParentID, int trinhky = 0)
        {
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["trinhky"] = trinhky;

            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_109_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }


        private DataTable DT_rptDuToan_109_TongHop(string username, string iID_MaPhongBan)
        {
            return DT_rptDuToan_109_TongHop_v1(Username, iID_MaPhongBan);
        }

        private DataTable DT_rptDuToan_109_TongHop_v2(string username, string iID_MaPhongBan)
        {
            var service = new DuToanReportService();
            return service.TongHop(username, iID_MaPhongBan, "109,1020500");
        }
        //1020200
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_109_TongHop_v1(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            String SQL = String.Format(@"
SELECT SUBSTRING(sLNS,1,3) as sLNS3,SUBSTRING(sLNS,1,5) as sLNS5, sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBan
,rTuChi=sum(rTuChi/{3})
,rChiTapTrung=SUM(CASE WHEN iID_MaDonVi not in ('30','34','35','50','69','70','71','72','73','78','80','88','89','90','91','92','93','94') THEN (rChiTapTrung)/{3} ELSE rTuChi/{3} END)
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND (sLNS LIKE '109%' OR sLNS='1020500') AND iNamLamViec=@iNamLamViec and (rTuChi+rPhanCap+rChiTapTrung)<>0 {0} {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBan
", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public static String LayMoTa(String sLNS)
        {
            String sMoTa = "";

            String SQL = String.Format(@"SELECT sMoTa FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sLNS={0}", sLNS);
            sMoTa = Connection.GetValueString(SQL, "");
            return sMoTa;
        }
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_109_TongHop(MaND, iID_MaPhongBan);
            //DataTable data = getDataTable_109(iID_MaPhongBan);


            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS3,sLNS5,sLNS,sL,sK", "sLNS3,sLNS5,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS3,sLNS5,sLNS", "sLNS3,sLNS5,sLNS,sMoTa", "sLNS,sL");

            DataTable dtLNS5 = HamChung.SelectDistinct("dtLNS5", dtsLNS, "sLNS3,sLNS5", "sLNS3,sLNS5,sMoTa");
            for (int i = 0; i < dtLNS5.Rows.Count; i++)
            {
                r = dtLNS5.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS5"]));
            }

            DataTable dtLNS3 = HamChung.SelectDistinct("dtLNS3", dtLNS5, "sLNS3", "sLNS3,sMoTa");
            for (int i = 0; i < dtLNS3.Rows.Count; i++)
            {
                r = dtLNS3.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS3"]));
            }

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS3", dtLNS3);
            fr.AddTable("dtsLNS5", dtLNS5);



            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtLNS3.Dispose();
            dtLNS5.Dispose();

        }

        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        private ExcelFile createReport(string iID_MaPhongBan, int trinhky = 0)
        {
            var file = !string.IsNullOrWhiteSpace(iID_MaPhongBan) && iID_MaPhongBan != "-1" && trinhky == 1 ?
                   sFilePath_TrinhKy :
                   sFilePath;
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaPhongBan);

            if (!string.IsNullOrWhiteSpace(iID_MaPhongBan))
            {
                var phongban = _nganSachService.GetPhongBanById(iID_MaPhongBan);
                fr.SetValue("sTenPhongBan", phongban.sMoTa);
            }


            fr.UseCommonValue()
               .UseChuKy(Username)
               .UseChuKyForController(this.ControllerName())
               .Run(xls);

            var count = xls.TotalPageCount();

            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string iID_MaPhongBan, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = createReport(iID_MaPhongBan, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaPhongBan, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = createReport(iID_MaPhongBan, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        #region longsam

        private DataTable getDataTable_109(string id_phongban = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_109_TongHop.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(
                        commandText: sql,
                        parameters: new
                        {
                            id_phongban = id_phongban.ToParamString(),
                            id_donvi = Request.GetQueryString("id_donvi", PhienLamViec.iID_MaDonVi),
                            NamLamViec = PhienLamViec.iNamLamViec,
                            dvt = 1000,
                        });
                return dt;
            }
            #endregion

        }
        #endregion
    }
}
