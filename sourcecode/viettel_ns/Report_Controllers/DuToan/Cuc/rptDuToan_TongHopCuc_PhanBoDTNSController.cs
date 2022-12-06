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
using VIETTEL.Helpers;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_TongHopCuc_PhanBoDTNSController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptPhanBoDuToanNganSachNam.xls";


        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_PhanBoDTNS.aspx";
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
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_PhanBoDTNS.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND, String iID_MaPhongBan)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

            String sTenDonVi = iID_MaPhongBan;
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaPhongBan);
            fr = ReportModels.LayThongTinChuKy(fr, "rptPhanBoDuToanNganSachNam");
            fr.SetValue("Nam", iNamLamViec);
            if (iID_MaPhongBan != "-1")
                fr.SetValue("sTenDonVi", "B " + sTenDonVi);
            else
            {
                fr.SetValue("sTenDonVi", "");
            }
            fr.SetValue("COT1", "Sách, báo, tạp chí");
            fr.SetValue("COT2", "Văn hóa, V.nghệ, CLB");
            fr.SetValue("COT3", "Huấn luyện CĐ-TDTT");
            fr.SetValue("COT4", "Khen thưởng");
            fr.SetValue("COT5", "Phúc lợi tập thể");
            fr.SetValue("COT6", "Công tác phí");
            fr.SetValue("COT7", "Bảo quản SC xe máy");
            fr.SetValue("COT8", "Thanh toán tiền điện, nước");
            fr.SetValue("COT9", "Bảo quản, SC C.trình PT");
            fr.SetValue("COT10", "Doanh cụ các loại");
            fr.SetValue("COT11", "XD CTrình phổ thông");
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            fr.SetValue("Cap2", ReportModels.CauHinhTenDonViSuDung(2,MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

             DataTable dtCauHinhbia = DuToan_ChungTuModels.GetCauHinhBia(MaND);
             String sSoQuyetDinh = "";
             if (dtCauHinh.Rows.Count > 0)
             {
                 sSoQuyetDinh = dtCauHinhbia.Rows[0]["sSoCongKhai"].ToString();

             }
             fr.SetValue("sSoQuyetDinh", sSoQuyetDinh);

            fr.Run(Result);
            return Result;

        }

        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptPhanBoDuToanNganSachNam(String MaND, String iID_MaPhongBan)
        {
            SqlCommand cmd = new SqlCommand();
            
            String DKPhongBan = "";
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);

            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_PhanBoDTNS.sql");
            
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000000);
            cmd.CommandText = sql;

            var dt = Connection.GetDataTable(cmd);
            return dt;
        }
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            DataRow r;
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(System.Web.HttpContext.Current.User.Identity.Name);
            int iNamLamViec = Convert.ToInt32(dtCauHinh.Rows[0]["iNamLamViec"]);
            DataTable data;
            
            data = DT_rptPhanBoDuToanNganSachNam(MaND, iID_MaPhongBan);           

            DataTable dataLoai = HamChung.SelectDistinct("dataLoai", data, "Loai", "Loai");
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            fr.AddTable("dataLoai", dataLoai);
            data.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan)
        {
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "CongKhaiPhanBoDuToan.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaPhongBan)
        {
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan);
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

