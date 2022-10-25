using System;
using System.Collections.Generic;
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
using Viettel.Services;
using Viettel.Data;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_KiemNganhController : FlexcelReportController
    {
        public string sViewPath = "~/Report_Views/DuToan/";
        private  String sFilePath = "/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_KiemNganh.xls";
        String iCapTongHop = "";
        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name) && PhienLamViec.iID_MaPhongBan == "02")
            {
                ViewData["path"] = "~/Report_Views/DuToan/BieuKiem/rptDuToan_KiemNganh.aspx";
                ViewData["PageLoad"] = "0";
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
        /// <summary>
        /// hàm lấy các giá trên form
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public ActionResult EditSubmit(String ParentID)
        {
            String sNG = Request.Form["sNG"];
            ViewData["sNG"] = sNG;
            ViewData["PageLoad"] = "1";
            ViewData["path"] = "~/Report_Views/DuToan/BieuKiem/rptDuToan_KiemNganh.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(string path, string Nganh)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, Nganh);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1020100_TungDonVi");
            fr.SetValue("TieuDe", $"SO SÁNH SỐ CHI TIẾT DỰ TOÁN NGÀNH {ReportModels.GetDtNganh(PhienLamViec.iNamLamViec).AsEnumerable().FirstOrDefault(x=> x.Field<string>("sNG") == Nganh)["sMoTa"].ToString().TrimEnd().ToUpper()} NĂM {PhienLamViec.NamLamViec - 1} VÀ {PhienLamViec.iNamLamViec}");
            fr.SetValue("Nam", PhienLamViec.iNamLamViec);

            fr.Run(Result);
            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public DataTable DT_getData(string Nganh)
        {
            DataTable vR;

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmdvr = new SqlCommand("DT_CompareData_B2Y", conn))
            {
                cmdvr.CommandType = CommandType.StoredProcedure;
                
                cmdvr.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmdvr.Parameters.AddWithValue("@ng", Nganh.ToParamString());
                cmdvr.Parameters.AddWithValue("@dvt", 1000);

                vR = cmdvr.GetDataset().Tables[0];
            }
            
            return vR;
        }        
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, string Nganh)
        {
            DataTable data = DT_getData(Nganh);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            data.Dispose();          
        }
      
        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(string Nganh)
        {
            HamChung.Language();      
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath),Nganh);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "Kiem_Nganh.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string Nganh)
        {            
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath),Nganh);
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

