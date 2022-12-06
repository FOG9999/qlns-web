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
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1020700_TongHopController : FlexcelReportController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_1020700_TongHop.xls";


        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                MucLucNganSachModels.CapNhapLai();
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_1020700_TongHop.aspx";
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
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_1020700_TongHop.aspx";
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
            fr = ReportModels.LayThongTinChuKy(fr, this.ControllerName());
            fr.SetValue("Nam", iNamLamViec);
            
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Cap2", sTenDonVi);
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            var loaiKhoan = HamChung.GetLoaiKhoanText("1020700");
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
        public DataTable DT_rptDuToan_1010100_TongHop(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;



            String DKCauHinh = "and sLNS ='1020700'";

            String SQL = String.Format(@"SELECT
        sLNS,sL,sK,sM,sTM,STTM,sNG,sMoTa,
        SUM(rTuChi+rDuPhong)/{0} AS rTuChi,
        SUM(rHienVat/{0}) AS rHienVat 
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)   AND ({3})  AND iNamLamViec=@iNamLamViec {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa", DVT, DKPhongBan, DKDonVi, DKCauHinh);
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
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            //DataTable data = DT_rptDuToan_1010100_TongHop(MaND, iID_MaPhongBan);
            var data = getDataTable_1020700_ChonTo(iID_MaPhongBan);
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
                clsResult.FileName = "DuToan_1010000_TongHop.xls";
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


        #region longsam

        /// <summary>
        /// BHYT
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <returns></returns>
        private DataTable getDataTable_1020700_ChonTo(string id_phongban = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_1020700_TongHop.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@id_donvi", PhienLamViec.iID_MaDonVi);
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@lns", "1020700");
                cmd.Parameters.AddWithValue("@dvt", 1000);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion

        }

        #endregion
    }
}
