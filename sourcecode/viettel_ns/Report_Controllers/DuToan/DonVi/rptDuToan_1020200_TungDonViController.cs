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
using Viettel.Extensions;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1020200_TungDonViController : AppController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const string sFilePath = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_1020200_TungDonVi.xls";
        private const string sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_1020200_TungDonVi_TrinhKy.xls";


        public ActionResult Index(int trinhky = 0)
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1020200_TungDonVi.aspx";
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
        public ActionResult EditSubmit(string ParentID, int trinhky = 0)
        {
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["trinhky"] = trinhky;

            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1020200_TungDonVi.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string iID_MaDonVi, int trinhky = 0)
        {
            HamChung.Language();

            var xls = CreateReport(iID_MaDonVi, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaDonVi, int trinhky = 0)
        {
            HamChung.Language();

            var xls = CreateReport(iID_MaDonVi, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ExcelFile CreateReport(string iID_MaDonVi, int trinhky = 0)
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);

            var file = trinhky == 1 ? sFilePath_TrinhKy : sFilePath;
            xls.Open(Server.MapPath(file));

            LoadData(fr, Username, iID_MaDonVi);

            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);

            var sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, Username);
            fr.SetValue("sTenDonVi", sTenDonVi);

            var loaiKhoan = HamChung.GetLoaiKhoanText("1020200");
            fr.SetValue("LoaiKhoan", loaiKhoan);

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




        //public ExcelFile CreateReport(String path, String MaND, String iID_MaDonVi)
        //{
        //    DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
        //    String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);


        //    String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
        //    XlsFile Result = new XlsFile(true);
        //    Result.Open(path);
        //    FlexCelReport fr = new FlexCelReport();
        //    LoadData(fr, MaND, iID_MaDonVi);
        //    fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1020200_TungDonVi");
        //    fr.SetValue("Nam", iNamLamViec);
        //    fr.SetValue("sTenDonVi", sTenDonVi);
        //    fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
        //    fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
        //    fr.Run(Result);
        //    return Result;

        //}

        private DataTable DT_rptDuToan_1020200_TungDonVi(String MaND, String iID_MaDonVi)
        {
            var v = Request.GetQueryStringValue("v");
            return v == 2 ?
                DT_rptDuToan_1020200_TungDonVi_v2(MaND, iID_MaDonVi) :
                DT_rptDuToan_1020200_TungDonVi_v1(MaND, iID_MaDonVi);
        }

        private DataTable DT_rptDuToan_1020200_TungDonVi_v2(string MaND, string iID_MaDonVi)
        {
            IDuToanReportService _duToanService = new DuToanReportService();
            var dt = _duToanService.TungDonVi_1020200(MaND, iID_MaDonVi);
            return dt;
        }

        //1020200
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        private  DataTable DT_rptDuToan_1020200_TungDonVi_v1(String MaND,String iID_MaDonVi)
        {
            var _duToanReportService = new DuToanReportService();

            //var dt = _duToanReportService.DT_rptDuToan_1020200_TungDonVi(MaND, iID_MaDonVi);
            //return dt;

            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi/{3})
,rHienVat=SUM(rHienVat/{3})
,rHangNhap=SUM(rHangNhap/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND sLNS='1020200' AND iID_MaDonVi =@iID_MaDonVi  AND iNamLamViec=@iNamLamViec {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi", iID_MaDonVi, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
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
            DataTable data = DT_rptDuToan_1020200_TungDonVi(MaND, iID_MaDonVi);
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
         
           

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
          
        }
    }
}

