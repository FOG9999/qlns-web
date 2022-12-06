using System;
using System.Linq;
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
using VIETTEL.Helpers;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1010000_ML_TungDonViKiemController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private  String sFilePath = "/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_1010000_ML_TungDonViKiem.xls";
        String iCapTongHop = "";
        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/BieuKiem/rptDuToan_1010000_ML_TungDonViKiem.aspx";
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
            String iID_MaDonVi = Request.Form[ParentID + "_iID_MaDonVi"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["path"] = "~/Report_Views/DuToan/BieuKiem/rptDuToan_1010000_ML_TungDonViKiem.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND, String iID_MaDonVi)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi,MaND);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaDonVi);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1010000_TungDonVi");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

            var loaiKhoan = HamChung.GetLoaiKhoanText("1010000");
            fr.SetValue("LoaiKhoan", loaiKhoan);


            fr.Run(Result);
            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public  DataTable DT_rptDuToan_1010000_TungDonVi(String MaND,String iID_MaDonVi)
        {
            String DKPhongBan = "";            
            DataTable dtNĐonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
            var sql = FileHelpers.GetSqlQuery("rptDuToan_1010000_TungDonViKiem.sql");            
            SqlCommand cmd = new SqlCommand();
            string donvis = string.Join(",", dtNĐonVi.Rows.OfType<DataRow>().Select(r => r["iID_MaDonVi"].ToString()));
            DKPhongBan = ThuNopModels.DKPhongBan_Dich(MaND, cmd,"-1");
            int DVT = 1000;
            String iID_MaPhongBanQuanLy = DonViModels.getPhongBanCuaDonVi(iID_MaDonVi, User.Identity.Name);
            sql = sql.Replace("@@1", DKPhongBan);
            sql = sql.Replace("@@2", "and " + iID_MaPhongBanQuanLy);
            
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            cmd.Parameters.AddWithValue("@id_donvi", donvis);
            cmd.Parameters.AddWithValue("@dvt", DVT);
            DataTable dt = Connection.GetDataSet(cmd).Tables[0];
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
            DataTable data = DT_rptDuToan_1010000_TungDonVi(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");

            foreach (DataRow dtr in dtsTM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM,sTM".Split(','), MaND);
            }
            foreach (DataRow dtr in dtsM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM".Split(','), MaND);
            }

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
          
        }
      
        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi,String iCapTongHop)
        {
            this.iCapTongHop = iCapTongHop;
            
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath),MaND,iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1010000_Kiem_TungDonVi.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi,String iCapTongHop)
        {
            this.iCapTongHop = iCapTongHop;            
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi);
            using (FlexCelPdfExport pdf = new FlexCelPdfExport())
            {
                pdf.Workbook = xls;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.BeginExport(ms);
                    pdf.ExportAllVisibleSheets(false, "BaoCao");
                    pdf.EndExport();
                    ms.Position = 0;
                    return File(ms.ToArray(), "application/pdf");
                }
            }
        }
    }
}

