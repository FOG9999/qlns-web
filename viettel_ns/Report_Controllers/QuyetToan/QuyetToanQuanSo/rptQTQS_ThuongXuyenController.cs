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

namespace VIETTEL.Report_Controllers.QuyetToan.QuyetToanQuanSo
{
    public class rptQTQS_ThuongXuyenController : Controller
    {
        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuanSo/rptQTQS_ThuongXuyen.xls";
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_ThuongXuyen.aspx";
                ViewData["PageLoad"] = "0";
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        public ActionResult EditSubmit(string ParentID)
        {
            string iIdMaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            string iThang1 = Request.Form[ParentID + "_iThang1"];
            string iThang2 = Request.Form[ParentID + "_iThang2"];
            string iThang3 = Request.Form[ParentID + "_iThang3"];
            string iThang4 = Request.Form[ParentID + "_iThang4"];
            string bQuy = Request.Form[ParentID + "_bQuy"];
            ViewData["iIdMaPhongBan"] = iIdMaPhongBan;
            ViewData["iThang1"] = iThang1;
            ViewData["iThang2"] = iThang2;
            ViewData["iThang3"] = iThang3;
            ViewData["iThang4"] = iThang4;
            ViewData["bQuy"] = bQuy;
            ViewData["PageLoad"] = 1;
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_ThuongXuyen.aspx";
            return View(sViewPath + "ReportView.aspx");
        }

        public clsExcelResult ExportToExcel(String MaND, string iID_MaPhongBan, string iThang1, string iThang2, string iThang3, string iThang4, String bQuy)
        {
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan, iThang1, iThang2, iThang3, iThang4, bQuy);
            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "rptQTQS_ThuongXuyen.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }

        public ActionResult ViewPDF(string MaND, string iID_MaPhongBan, string iThang1, string iThang2, string iThang3, string iThang4, String bQuy)
        {
            HamChung.Language();
            string path = Server.MapPath(sFilePath);
            ExcelFile xls = CreateReport(path, MaND, iID_MaPhongBan, iThang1, iThang2, iThang3, iThang4, bQuy);
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
        private ExcelFile CreateReport(string path, string MaND, string iID_MaPhongBan, string iThang1, string iThang2, string iThang3, string iThang4, String bQuy)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            string iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);
            if (string.IsNullOrEmpty(iID_MaPhongBan))
            {
                iID_MaPhongBan = "-1";
            }
            String thang = "", sTenPhuLuc = "";
            if (bQuy == "on")
            {
                thang = "Quý";
                if (iID_MaPhongBan == "-1") sTenPhuLuc = "QS02/Q-TH";
                else sTenPhuLuc = "QS02/Q-B";
            }
            else
            {
                thang = "Tháng";
                if (iID_MaPhongBan == "-1") sTenPhuLuc = "QS02/T-TH";
                else sTenPhuLuc = "QS02/T-B";
            }
          

            String sTenPB = LayTenPhongBan(iID_MaPhongBan);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaPhongBan, iThang1, iThang2, iThang3, iThang4, bQuy);

            fr = ReportModels.LayThongTinChuKy(fr, "rptQTQS_TongHop");
            fr.SetValue("iNam", iNamLamViec);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("sTenPhuLuc", sTenPhuLuc);
            fr.SetValue("Cap2", ReportModels.CauHinhTenDonViSuDung(2, MaND));
            fr.SetValue("thang", thang);
            fr.SetValue("iThang1", !string.IsNullOrEmpty(iThang1) && iThang1.Equals("-1") ? "" : iThang1);
            fr.SetValue("iThang2", !string.IsNullOrEmpty(iThang2) && iThang2.Equals("-1") ? "" : iThang2);
            fr.SetValue("iThang3", !string.IsNullOrEmpty(iThang3) && iThang3.Equals("-1") ? "" : iThang3);
            fr.SetValue("iThang4", !string.IsNullOrEmpty(iThang4) && iThang4.Equals("-1") ? "" : iThang4);
            fr.SetValue("sTenPB", sTenPB);
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;
        }
        private string LayTenPhongBan(string iID_MaPhongBan)
        {
            String sTenPB = string.Empty;
            String SQL = String.Format(@"SELECT sTen FROM NS_PhongBan WHERE sKyHieu=@sKyHieu AND iTrangThai=1");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@sKyHieu", iID_MaPhongBan);
            sTenPB = Connection.GetValueString(cmd, "");
            return sTenPB;
        }
        private void LoadData(FlexCelReport fr, string MaND, string iID_MaPhongBan, string iThang1, string iThang2, string iThang3, string iThang4, String bQuy)
        {
            DataTable data = DT_rptQTQS_Thang(MaND, iID_MaPhongBan, iThang1, iThang2, iThang3, iThang4, bQuy);
            data.TableName = "TongHop";
            fr.AddTable("TongHop", data);
            DataTable dtsPhong = HamChung.SelectDistinct("dtsPhong", data, "iID_MaPhongBan,sTenPhongBan", "iID_MaPhongBan,sTenPhongBan", "iID_MaPhongBan,sTenPhongBan,iID_MaDonVi");
            fr.AddTable("dtsPhong", dtsPhong);
            dtsPhong.Dispose();
            data.Dispose();
        }

        private DataTable DT_rptQTQS_Thang(string MaND, string iID_MaPhongBan, string iThang1, string iThang2, string iThang3, string iThang4, String bQuy)
        {
            String DKDonVi = string.Empty;
            String DKPhongBan = string.Empty;
            String DK = "", DK1 = string.Empty, DK2 = "", DK3 = "", DK4 = "", DKThangQuy = ""; ;
            String SelectDV = string.Empty;
            String SelectFROM = string.Empty;

            string sql = string.Empty;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            if (bQuy == "on")
            {
                if (iThang1 == "1")
                    DKThangQuy = " AND iThang_Quy IN (1,2,3) ";
                else if (iThang1 == "2")
                    DKThangQuy = " AND iThang_Quy IN (4,5,6) ";
                else if (iThang1 == "3")
                    DKThangQuy = " AND iThang_Quy IN (7,8,9) ";
                else if (iThang1 == "4")
                    DKThangQuy = " AND iThang_Quy IN (10,11,12) ";
                else
                    DKThangQuy = " AND 0=1";
            }
            else
            {
                DKThangQuy = " AND iThang_Quy=@iThang ";
                cmd.Parameters.AddWithValue("@iThang", iThang1);
            }
            String SQL = String.Format(@"SELECT COUNT(*) FROM QTQS_ChungTuChiTiet WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {2} {0} {1}", DKDonVi, DKPhongBan,DKThangQuy);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            int count = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (count <= 0)
                DK1 = " AND 0=1";
            else
                DK1 = "";
            //check don vi co so lieu
            cmd = new SqlCommand();
            if (bQuy == "on")
            {
                if (iThang1 == "1")
                    DKThangQuy = " AND iThang_Quy IN (1,2,3) ";
                else if (iThang1 == "2")
                    DKThangQuy = " AND iThang_Quy IN (4,5,6) ";
                else if (iThang1 == "3")
                    DKThangQuy = " AND iThang_Quy IN (7,8,9) ";
                else if (iThang1 == "4")
                    DKThangQuy = " AND iThang_Quy IN (10,11,12) ";
                else
                    DKThangQuy = " AND 0=1";
            }
            else
            {
                DKThangQuy = " AND iThang_Quy=@iThang ";
                cmd.Parameters.AddWithValue("@iThang", iThang1);
            }
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            SQL = String.Format(@"SELECT DISTINCT iID_MaDonVi FROM QTQS_ChungTu WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {2} {0} {1}", DKDonVi, DKPhongBan, DKThangQuy);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtDonViDV1 = Connection.GetDataTable(cmd);

           SqlCommand cmdMain = new SqlCommand();
            String DKDonViCoSoLieu = "";
            if (dtDonViDV1.Rows.Count > 0)
            {
                for (int i = 0; i < dtDonViDV1.Rows.Count; i++)
                {
                    DKDonViCoSoLieu += "iID_MaDonVi=@DonVi1" + i;
                    if (i < dtDonViDV1.Rows.Count - 1)
                        DKDonViCoSoLieu += " OR ";
                    cmdMain.Parameters.AddWithValue("@DonVi1" + i, dtDonViDV1.Rows[i]["iID_MaDonVi"]);
                }
               
            }
            else
            {
            }



            //dk2
            cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            if (bQuy == "on")
            {
                if (iThang2 == "1")
                    DKThangQuy = " AND iThang_Quy IN (1,2,3) ";
                else if (iThang2 == "2")
                    DKThangQuy = " AND iThang_Quy IN (4,5,6) ";
                else if (iThang2 == "3")
                    DKThangQuy = " AND iThang_Quy IN (7,8,9) ";
                else if (iThang2 == "4")
                    DKThangQuy = " AND iThang_Quy IN (10,11,12) ";
                else
                    DKThangQuy = " AND 0=1";
            }
            else
            {
                DKThangQuy = " AND iThang_Quy=@iThang ";
                cmd.Parameters.AddWithValue("@iThang", iThang2);
            }
            SQL = String.Format(@"SELECT COUNT(*) FROM QTQS_ChungTuChiTiet WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {2} {0} {1}", DKDonVi, DKPhongBan,DKThangQuy);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            count = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (count <= 0)
                DK2 = " AND 0=1";
            else
                DK2 = "";


            //dk3
            cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            if (bQuy == "on")
            {
                if (iThang3 == "1")
                    DKThangQuy = " AND iThang_Quy IN (1,2,3) ";
                else if (iThang3 == "2")
                    DKThangQuy = " AND iThang_Quy IN (4,5,6) ";
                else if (iThang3 == "3")
                    DKThangQuy = " AND iThang_Quy IN (7,8,9) ";
                else if (iThang3 == "4")
                    DKThangQuy = " AND iThang_Quy IN (10,11,12) ";
                else
                    DKThangQuy = " AND 0=1";
            }
            else
            {
                DKThangQuy = " AND iThang_Quy=@iThang ";
                cmd.Parameters.AddWithValue("@iThang", iThang3);
            }
            SQL = String.Format(@"SELECT COUNT(*) FROM QTQS_ChungTuChiTiet WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {2} {0} {1}", DKDonVi, DKPhongBan,DKThangQuy);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            count = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (count <= 0)
                DK3 = " AND 0=1";
            else
                DK3 = "";

            //dk4
            cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            if (bQuy == "on")
            {
                if (iThang4 == "1")
                    DKThangQuy = " AND iThang_Quy IN (1,2,3) ";
                else if (iThang4 == "2")
                    DKThangQuy = " AND iThang_Quy IN (4,5,6) ";
                else if (iThang4 == "3")
                    DKThangQuy = " AND iThang_Quy IN (7,8,9) ";
                else if (iThang4 == "4")
                    DKThangQuy = " AND iThang_Quy IN (10,11,12) ";
                else
                    DKThangQuy = " AND 0=1";
            }
            else
            {
                DKThangQuy = " AND iThang_Quy=@iThang ";
                cmd.Parameters.AddWithValue("@iThang", iThang4);
            }
            SQL = String.Format(@"SELECT COUNT(*) FROM QTQS_ChungTuChiTiet WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {2} {0} {1}", DKDonVi, DKPhongBan, DKThangQuy);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));

            count = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (count <= 0)
                DK4 = " AND 0=1";
            else
                DK4 = "";

            if (dtDonViDV1.Rows.Count > 0)
            {
                DK1 = DK1 + "AND (" + DKDonViCoSoLieu + ")";
                DK2 = DK2 + "AND (" + DKDonViCoSoLieu + ")";
                DK3 = DK3 + "AND (" + DKDonViCoSoLieu + ")";
                DK4 = DK4 + "AND (" + DKDonViCoSoLieu + ")";
            }


            DKDonVi = ThuNopModels.DKDonVi(MaND, cmdMain);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmdMain, "A");
            iThang1 = string.IsNullOrEmpty(iThang1) ? "-1" : iThang1;
            iThang2 = string.IsNullOrEmpty(iThang2) ? "-1" : iThang2;
            iThang3 = string.IsNullOrEmpty(iThang3) ? "-1" : iThang3;
            iThang4 = string.IsNullOrEmpty(iThang4) ? "-1" : iThang4;
            if (string.IsNullOrEmpty(iID_MaPhongBan))
            {
                iID_MaPhongBan = "-1";
            }
            if (!iID_MaPhongBan.Equals("-1"))
            {
                DK = " AND iID_MaPhongBan=@MaPhong ";
            }

            //Tinh theo quy

            if (bQuy == "on")
            {
                iThang1 = string.IsNullOrEmpty(iThang1) ? "-1" : Convert.ToString(Convert.ToInt32(iThang1) * 3);
                iThang2 = string.IsNullOrEmpty(iThang1) ? "-1" : Convert.ToString(Convert.ToInt32(iThang2) * 3);
                iThang3 = string.IsNullOrEmpty(iThang1) ? "-1" : Convert.ToString(Convert.ToInt32(iThang3) * 3);
                iThang4 = string.IsNullOrEmpty(iThang1) ? "-1" : Convert.ToString(Convert.ToInt32(iThang4) * 3);
            }
            sql = string.Format(@"
           				SELECT 
				rSQ1=SUM(CASE WHEN ((iThang_Quy <=@iThang1 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {3} THEN rSQ ELSE 0 END),
				rSQ2=SUM(CASE WHEN ((iThang_Quy <=@iThang2 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {4} THEN rSQ ELSE 0 END),
				rSQ3=SUM(CASE WHEN ((iThang_Quy <=@iThang3 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {5} THEN rSQ ELSE 0 END),
				rSQ4=SUM(CASE WHEN ((iThang_Quy <=@iThang4 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {6} THEN rSQ ELSE 0 END),
				
				rQNCN1=SUM(CASE WHEN ((iThang_Quy <=@iThang1 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {3} THEN rQNCN ELSE 0 END),
				rQNCN2=SUM(CASE WHEN ((iThang_Quy <=@iThang2 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {4} THEN rQNCN ELSE 0 END),
				rQNCN3=SUM(CASE WHEN ((iThang_Quy <=@iThang3 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {5} THEN rQNCN ELSE 0 END),
				rQNCN4=SUM(CASE WHEN ((iThang_Quy <=@iThang4 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {6} THEN rQNCN ELSE 0 END),
				
				rCNVHD1=SUM(CASE WHEN ((iThang_Quy <=@iThang1 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {3} THEN rCNVHD ELSE 0 END),
				rCNVHD2=SUM(CASE WHEN ((iThang_Quy <=@iThang2 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {4} THEN rCNVHD ELSE 0 END),
				rCNVHD3=SUM(CASE WHEN ((iThang_Quy <=@iThang3 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {5} THEN rCNVHD ELSE 0 END),
				rCNVHD4=SUM(CASE WHEN ((iThang_Quy <=@iThang4 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {6} THEN rCNVHD ELSE 0 END),
				
				rHSQCS1=SUM(CASE WHEN ((iThang_Quy <=@iThang1 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {3} THEN rHSQCS ELSE 0 END),
				rHSQCS2=SUM(CASE WHEN ((iThang_Quy <=@iThang2 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {4} THEN rHSQCS ELSE 0 END),
				rHSQCS3=SUM(CASE WHEN ((iThang_Quy <=@iThang3 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {5} THEN rHSQCS ELSE 0 END),
				rHSQCS4=SUM(CASE WHEN ((iThang_Quy <=@iThang4 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))  {6} THEN rHSQCS ELSE 0 END),
				iID_MaDonVi,sTen,iID_MaPhongBan,sTenPhongBan
				FROM (
				SELECT
				rSQ=(
                SUM(CASE WHEN sKyHieu='2' THEN rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+ rTrungTa+rThuongTa+rDaiTa+ rTuong ELSE 0 END) -
                SUM(CASE WHEN sKyHieu='3' THEN rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+ rTrungTa+rThuongTa+rDaiTa+ rTuong ELSE 0 END)+
                SUM(CASE WHEN sKyHieu='500' THEN rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+ rTrungTa+rThuongTa+rDaiTa+ rTuong ELSE 0 END)-
                SUM(CASE WHEN sKyHieu='600' THEN rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+ rTrungTa+rThuongTa+rDaiTa+ rTuong ELSE 0 END)
                ),
                rQNCN=(
                SUM(CASE WHEN sKyHieu='2' THEN rQNCN ELSE 0 END) -
                SUM(CASE WHEN sKyHieu='3' THEN rQNCN ELSE 0 END)+
                SUM(CASE WHEN sKyHieu='500' THEN rQNCN ELSE 0 END)-
                SUM(CASE WHEN sKyHieu='600' THEN rQNCN ELSE 0 END)
                )
                ,
                rCNVHD=(
                SUM(CASE WHEN sKyHieu='2' THEN rCNVQP+rLDHD ELSE 0 END) -
                SUM(CASE WHEN sKyHieu='3' THEN rCNVQP+rLDHD ELSE 0 END)+
                SUM(CASE WHEN sKyHieu='500' THEN rCNVQP+rLDHD ELSE 0 END)-
                SUM(CASE WHEN sKyHieu='600' THEN rCNVQP+rLDHD ELSE 0 END)
                )
                ,
                rHSQCS=(
                SUM(CASE WHEN sKyHieu='2' THEN rTSQ+ rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi ELSE 0 END) -
                SUM(CASE WHEN sKyHieu='3' THEN rTSQ+ rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi ELSE 0 END)+
                SUM(CASE WHEN sKyHieu='500' THEN rTSQ+ rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi ELSE 0 END)-
                SUM(CASE WHEN sKyHieu='600' THEN rTSQ+ rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi ELSE 0 END)
                )
				,iThang_Quy,iID_MaDonVi,iID_MaPhongBan,sTenPhongBan,iNamLamViec
				from QTQS_CHungTuChiTiet 
				WHERE iTrangThai=1 {0} {1} AND ((iThang_Quy <=@iThang4 AND iNamLamViec=@iNamLamViec) OR (iThang_Quy <=12 AND iNamLamViec<=@iNamLamViecTruoc))
				group by  iID_MaDonVi,iThang_Quy,iID_MaPhongBan,sTenPhongBan,iNamLamViec 
				) as A
				 INNER JOIN  
				
				(SELECT iID_MaDonVi as MaDonVi,sTen FROM NS_DonVi WHERE iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec) as B
				 ON A.iID_MaDonVi = B.MaDonVi								
				group by  iID_MaDonVi,sTen,iID_MaPhongBan,sTenPhongBan
                
            ", DK, DKDonVi, "", DK1, DK2, DK3, DK4);
            cmdMain.CommandText = sql;
            if (!iID_MaPhongBan.Equals("-1"))
            {
                cmdMain.Parameters.AddWithValue("@MaPhong", iID_MaPhongBan);
            }
            cmdMain.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmdMain.Parameters.AddWithValue("@iNamLamViecTruoc", int.Parse(ReportModels.LayNamLamViec(MaND))-1);
            cmdMain.Parameters.AddWithValue("@iThang1", iThang1);
            cmdMain.Parameters.AddWithValue("@iThang2", iThang2);
            cmdMain.Parameters.AddWithValue("@iThang3", iThang3);
            cmdMain.Parameters.AddWithValue("@iThang4", iThang4);
            DataTable dt = new DataTable();
            dt = Connection.GetDataTable(cmdMain);
            cmdMain.Dispose();
            return dt;
        }

    }
}