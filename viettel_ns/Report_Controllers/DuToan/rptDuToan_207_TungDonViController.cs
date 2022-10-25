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

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_207_TungDonViController : AppController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const string sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_207_TungDonVi.xls";
        private const string sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/rptDuToan_207_TungDonVi_TrinhKy.xls";
       

        public ActionResult Index(int trinhky = 0)
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_207_TungDonVi.aspx";
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
        public ActionResult EditSubmit(String ParentID, string iID_MaDonVi, int trinhky = 0)
        {
            ViewData["PageLoad"] = "1";
            ViewData["trinhky"] = trinhky;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;

            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_207_TungDonVi.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        private ExcelFile CreateReport(string iID_MaDonVi, int trinhky = 0)
        {
            var sTenDonVi = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh_NhaNuoc", "iID_MaNganh", iID_MaDonVi, "sTenNganh"));

            var file = trinhky == 0 ? sFilePath : sFilePath_TrinhKy;
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            var fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaDonVi);
            fr.SetValue("sTenDonVi", sTenDonVi);

            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);

            fr.UseCommonValue()
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .Run(xls);

            return xls;

        }

        private DataTable DT_rptDuToan_207_TungDonVi(string username, string iID_MaDonVi)
        {
            var v = Request.GetQueryStringValue("v");
            return v == 2 ?
                DT_rptDuToan_207_TungDonVi_v2(username, iID_MaDonVi) :
                DT_rptDuToan_207_TungDonVi_v1(username, iID_MaDonVi);
        }

        private DataTable DT_rptDuToan_207_TungDonVi_v2(string username, string iID_MaDonVi)
        {
            var s = new DuToanReportService();
            return s.TungDonVi_NhaNuoc(username, iID_MaDonVi, "207");
        }


        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        private  DataTable DT_rptDuToan_207_TungDonVi_v1(String MaND,String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            //  DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);


            int i = 0;
            String DSNganh = "";
            String iID_MaNganhMLNS = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh_NhaNuoc", "iID_MaNganh", iID_MaDonVi, "iID_MaNganhMLNS"));
            if (String.IsNullOrEmpty(iID_MaNganhMLNS))
                iID_MaNganhMLNS = "-1";
            DSNganh = " AND sNG IN (" + iID_MaNganhMLNS + ")";

            int DVT = 1000;
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
,rTuChi=SUM(rTuChi/{3})
,rTonKho=SUM(rTonKho/{3})
,rHangNhap=SUM(rHangNhap/{3})
,rHangMua=SUM(rHangMua/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS LIKE '207%' AND iNamLamViec=@iNamLamViec {1} {2} {0}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rTonKho)<>0  OR SUM(rHangMua)<>0  OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
", DSNganh, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
       
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaDonVi)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_207_TungDonVi(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK", "sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "sLNS,sMoTa", "sLNS,sL");
           
            

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);

            String sLNS = "";
            if (data.Rows.Count > 0)
                sLNS = data.Rows[0]["sLNS"].ToString();
            fr.SetValue("LNS",sLNS);

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
        public ActionResult ViewPDF(string iID_MaDonVi, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaDonVi, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaDonVi, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaDonVi, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}

