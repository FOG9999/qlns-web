using DomainModel;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_NghiepVu_TongHopController : FlexcelReportController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_NghiepVu_TongHop.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_NghiepVu_TongHop_Muc.xls";
        String iCapTongHop = "";
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_NghiepVu_TongHop.aspx";
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
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_NghiepVu_TongHop.aspx";
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


            string sTenDonVi = PhongBanModels.Get_TenPhongBan(iID_MaPhongBan);
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
                sTenDonVi = "BỘ QUỐC PHÒNG";

            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaPhongBan);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1020000_01_TongHop");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("Cap2", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

            var loaiKhoan = HamChung.GetLoaiKhoanText("1020000");
            fr.SetValue("LoaiKhoan", loaiKhoan);


            fr.Run(Result);

            var count = Result.TotalPageCount();

            if (count > 1)
            {
                Result.AddPageFirstPage();
            }
            return Result;

        }

        //1020200
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_1020000_01_TongHop(String MaND, String iID_MaPhongBan)
        {
            if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
                iID_MaPhongBan = "-1";

            string DKDonVi = "", DKPhongBan = "";
            string sTenB10 = "TC,BTTM";
            string sTenB6 = "DN";
            string sTenB = "QK,QĐ,HVNT";
            string sTenBV = "BV";

            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_Dich(MaND, cmd, iID_MaPhongBan);

            var sql = FileHelpers.GetSqlQuery("rptDuToan_NghiepVu_TongHop.sql");

            sql = sql.Replace("@@DKPB", DKPhongBan);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@sTenB10", sTenB10);
            sql = sql.Replace("@@sTenB6", sTenB6);
            sql = sql.Replace("@@sTenB", sTenB);
            sql = sql.Replace("@@sTenHos", sTenBV);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
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
            DataTable data = DT_rptDuToan_1020000_01_TongHop(MaND, iID_MaPhongBan);

            data.Columns.Add("CheckTrung", typeof(String));
            for (int i = 1; i < data.Rows.Count; i++)
            {
                String sXau1, sXau2;
                sXau1 = Convert.ToString(data.Rows[i - 1]["sM"]) + Convert.ToString(data.Rows[i - 1]["sTM"]) + Convert.ToString(data.Rows[i - 1]["sTTM"]) + Convert.ToString(data.Rows[i - 1]["sNG"]);
                sXau2 = Convert.ToString(data.Rows[i]["sM"]) + Convert.ToString(data.Rows[i]["sTM"]) + Convert.ToString(data.Rows[i]["sTTM"]) + Convert.ToString(data.Rows[i]["sNG"]);
                if (sXau1.Equals(sXau2))
                {
                    data.Rows[i]["CheckTrung"] = "1";
                }
            }

            //DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "iID_MaNganh,sM,sTM", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sMoTa", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM");
            //DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "iID_MaNganh,sM", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sMoTa", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM");
            //DataTable dtsNganh = HamChung.SelectDistinct("dtsNganh", dtsM, "iID_MaNganh", "iID_MaNganh,sTenNganh", "iID_MaNganh,sTenNganh");

            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");

            fr.AddTable("ChiTiet", data);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            //fr.AddTable("dtsNganh", dtsNganh);

            foreach (DataRow dtr in dtsTM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM,sTM".Split(','), MaND);
            }
            foreach (DataRow dtr in dtsM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM".Split(','), MaND);
            }

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            //dtsNganh.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan, String iCapTongHop)
        {
            this.iCapTongHop = iCapTongHop;
            if (iCapTongHop == "Muc")
            {
                sFilePath = sFilePath_Muc;
            }
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1020200_TongHop.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaPhongBan, String iCapTongHop)
        {
            this.iCapTongHop = iCapTongHop;
            if (iCapTongHop == "Muc")
            {
                sFilePath = sFilePath_Muc;
            }
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



        #region longsam

        /// <summary>
        /// Nghiep vu chung
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <returns></returns>
        private DataTable getDataTable_1020000_ChonTo(string id_phongban = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_1020000_TongHop.sql");

            var id_donvi = Request.GetQueryString("id_donvi", PhienLamViec.iID_MaDonVi);

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@id_donvi", id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", 1000);
                var dt = cmd.GetTable();
                return dt;
            }
            #endregion

        }

        #endregion
    }
}
