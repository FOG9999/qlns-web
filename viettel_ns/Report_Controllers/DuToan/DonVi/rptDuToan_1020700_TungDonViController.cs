using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;
using VIETTEL.Flexcel;
using Viettel.Services;
using Viettel.Data;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1020700_TungDonViController : AppController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const string sFilePath = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_1020700_TungDonVi.xls";

        private readonly INganSachService _nganSachService;

        public rptDuToan_1020700_TungDonViController()
        {
            _nganSachService = NganSachService.Default;
        }


        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1020700_TungDonVi.aspx";
            ViewData["PageLoad"] = "0";
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
        public ActionResult EditSubmit(String ParentID)
        {
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1020700_TungDonVi.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        private ExcelFile createReport(string iID_MaDonVi)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, iID_MaDonVi, "1020700");

            var sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, Username);
            fr.SetValue("sTenDonVi", sTenDonVi);
            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);
            fr.UseCommonValue()
              .UseChuKy()
              .Run(xls);
            return xls;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        //        private  DataTable getDuToan_1010100_TungDonVi(string iID_MaDonVi)
        //        {
        //            String DKDonVi = "", DKPhongBan = "", DK = "";
        //            SqlCommand cmd = new SqlCommand();
        //            DKDonVi = ThuNopModels.DKDonVi(Username, cmd);
        //            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(Username, cmd);
        //            int DVT = 1000;
        //            String iID_MaPhongBanQuanLy = DonViModels.getPhongBanCuaDonVi(iID_MaDonVi, User.Identity.Name);
        //            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
        //,rTuChi=SUM(rTuChi/{3})
        //,rChiTapChung=SUM(CASE WHEN iID_MaPhongBan='06' THEN rTuChi/{3}	ELSE rChiTapTrung/{3} END)
        // FROM DT_ChungTuChiTiet
        // WHERE iTrangThai=1  AND sLNS='1010100' AND iID_MaDonVi =@iID_MaDonVi AND {4} AND iNamLamViec=@iNamLamViec {1} {2}
        // GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
        //HAVING SUM(rTuChi)<> 0 OR SUM(rChiTapTrung)<>0 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT, iID_MaPhongBanQuanLy);
        //            cmd.CommandText = SQL;
        //            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(Username));
        //            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
        //            DataTable dt = Connection.GetDataTable(cmd);
        //            cmd.Dispose();
        //            return dt;
        //        }

        private DataTable getDuToan_1010100_TungDonVi(string iID_MaDonVi, string sLns)
        {
            #region sql

            var sql = @"
SELECT	iID_MaDonVi,
		sLNS,sL,sK,sM,sTM,STTM,sNG,sTNG,sMoTa,
		SUM(rTuChi+rDuPhong)/{0} AS rTuChi,
		SUM(rHienVat/{0}) AS rHienVat 
FROM	DT_ChungTuChiTiet 
WHERE	
        -- dk LNS
        {1}                
        -- dk Phongban
        {2}
        iID_MaDonVi = @iID_MaDonVi AND
		iNamLamViec=@iNamLamViec AND 
		iTrangThai=1 
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaDonVi
HAVING	SUM(rTuChi+rDuPhong)>0 OR 
		SUM(rHienVat)>0";

            var dkLns = string.Format("sLNS = '{0}' AND", sLns);
            var dkPhongban = "iID_MaPhongBan = '08' AND";

            sql = string.Format(sql, 1000, dkLns, dkPhongban);

            #endregion

            #region get data

            var config = NganSachService.Default.GetCauHinh(Username);            
            var nam = config.iNamLamViec;
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iNamLamViec = nam,
                         iID_MaDonVi,
                     },
                     commandType: CommandType.Text
                 );

                #endregion

                return dt;
            }
        }


        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, string iID_MaDonVi, string sLns)
        {
            DataTable data = getDuToan_1010100_TungDonVi(iID_MaDonVi, sLns);
            data.TableName = "ChiTiet";

            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK", "sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "sLNS,sMoTa", "sLNS,sL");

            fr.AddTable("ChiTiet", data);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
          
        }

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string iID_MaDonVi)
        {
            HamChung.Language();

            ExcelFile xls = createReport(iID_MaDonVi);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaDonVi)
        {
            HamChung.Language();

            ExcelFile xls = createReport(iID_MaDonVi);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}

