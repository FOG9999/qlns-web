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
    public class rptDuToan_TongHopCuc_BiaController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_TongHop_TongHop.xls";
        private const String sFilePath_2017 = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_TongHop_TongHop_2017.xls";
        public const int DVT = 1000;
        string sTyGia = "23.300", sSoQuyetDinh = "số .../QĐ-BQP ngày ../../....", sThuCanDoi = "80101", sThuQuanLy = "80102", sThuNSNN = "802", sNganSachBaoDam = "1040100",
           sLuong = "1010000,1050100", sNghiepVu = "102,1050000", sDoanhNghiep = "-1", sXDCB = "1030100", sNganSachKhac = "109", sNhaNuocGiao = "2", sKinhPhiKhac = "4";
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                sTyGia = "20000";
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_Bia.aspx";
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
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_Bia.aspx";
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
            
            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaDonVi);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_TongHop_TongHop");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("sTyGia", sTyGia);
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public DataTable DT_rptDuToan_TongHop_1010000(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sLuong)) sLuong = "-1";
            String[] arrCauHinh = sLuong.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }   
                     
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_TX.sql");
            sql = sql.Replace("@@DKLNSNV", DKCauHinh);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;

            var dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public DataTable DT_rptDuToan_TongHop_NV(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            String[] arrCauHinh = sNghiepVu.Split(',');

            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }

            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_NV.sql");
            sql = sql.Replace("@@DKLNSNV", DKCauHinh);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;

            var dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public DataTable DT_rptDuToan_TongHop_XDCB(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            String[] arrCauHinh = sXDCB.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }           
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_XDCB.sql");
            sql = sql.Replace("@@DKLNSNV", DKCauHinh);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_rptDuToan_TongHop_DN(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            String[] arrCauHinh = sDoanhNghiep.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_DN.sql");
            sql = sql.Replace("@@DKLNSNV", DKCauHinh);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_rptDuToan_TongHop_Khac(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            String[] arrCauHinh = sNganSachKhac.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }  
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_QPK.sql");
            sql = sql.Replace("@@DKLNSNV", DKCauHinh);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_rptDuToan_ThuNop(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sThuCanDoi)) sThuCanDoi = "-1";
            if (String.IsNullOrEmpty(sThuCanDoi)) sThuCanDoi = "-1";
            String[] arrCauHinh = sThuCanDoi.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }
            String DKCauHinh1 = "";
            if (String.IsNullOrEmpty(sThuQuanLy)) sThuQuanLy = "-1";
            if (String.IsNullOrEmpty(sThuQuanLy)) sThuQuanLy = "-1";
            String[] arrCauHinh1 = sThuQuanLy.Split(',');
            for (int i = 0; i < arrCauHinh1.Length; i++)
            {
                DKCauHinh1 += "sLNS LIKE @sLNS1" + i;
                if (i < arrCauHinh1.Length - 1)
                    DKCauHinh1 += " OR ";
                cmd.Parameters.AddWithValue("@sLNS1" + i, arrCauHinh1[i] + "%");
            }

            String DKCauHinh2 = "";
            if (String.IsNullOrEmpty(sThuNSNN)) sThuNSNN = "-1";
            if (String.IsNullOrEmpty(sThuNSNN)) sThuNSNN = "-1";
            String[] arrCauHinh2 = sThuNSNN.Split(',');
            for (int i = 0; i < arrCauHinh2.Length; i++)
            {
                DKCauHinh2 += "sLNS LIKE @sLNS2" + i;
                if (i < arrCauHinh2.Length - 1)
                    DKCauHinh2 += " OR ";
                cmd.Parameters.AddWithValue("@sLNS2" + i, arrCauHinh2[i] + "%");
            }

            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_TN.sql");
            sql = sql.Replace("@@DKTCD", DKCauHinh);
            sql = sql.Replace("@@DKTQL", DKCauHinh1);
            sql = sql.Replace("@@DKTNN", DKCauHinh2);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_rptDuToan_TongHop_BD(String MaND, String iID_MaPhongBan)
        {
            SqlCommand cmd = new SqlCommand();
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            String[] arrCauHinh = sNganSachBaoDam.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }
            
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_BD.sql");
            sql = sql.Replace("@@DKLNSBD", DKCauHinh);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;

            var dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public static DataTable DT_rptDuToan_TongHop_TonKho(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_TK.sql");
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public DataTable DT_rptDuToan_TongHop_NN(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            String[] arrCauHinh = sNhaNuocGiao.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }
            
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_NN.sql");
            sql = sql.Replace("@@DKCauHinh", DKCauHinh);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_rptDuToan_TongHop_KPKhac(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String DKCauHinh = "";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            String[] arrCauHinh = sKinhPhiKhac.Split(',');
            for (int i = 0; i < arrCauHinh.Length; i++)
            {
                DKCauHinh += "sLNS LIKE @sLNS" + i;
                if (i < arrCauHinh.Length - 1)
                    DKCauHinh += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, arrCauHinh[i] + "%");
            }
            String DKDN = "";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            String[] arrDM = sDoanhNghiep.Split(',');
            for (int i = 0; i < arrDM.Length; i++)
            {
                DKDN += "sLNS LIKE @sLNS" + i + 'a';
                if (i < arrDM.Length - 1)
                    DKDN += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i + 'a', arrDM[i] + "%");
            }
            
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_Bia_KPK.sql");
            sql = sql.Replace("@@DKCauHinh", DKCauHinh);
            sql = sql.Replace("@@DKNDN", DKDN);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public DataTable DT_ChoPhanCapB1(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi,
rSoTien=ISNULL(SUM(rDuPhong/{3}),0)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2) AND sLNS='1040100'  AND iNamLamViec=@iNamLamViec {1} {2} AND rDuPhong>0
GROUP BY iID_MaDonVi,sTenDonVi

 ", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_ChoPhanCapB2(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi,
rSoTien=ISNULL(SUM(rDuPhong/{3}),0)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2) AND iID_MaDonVi='C3'  AND iNamLamViec=@iNamLamViec {1} {2} AND rDuPhong>0
GROUP BY iID_MaDonVi,sTenDonVi

 ", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_ChoPhanCapB3(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi,sLNS,
rSoTien=ISNULL(SUM(rDuPhong/{3}),0)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2) AND (iID_MaDonVi='C2' OR iID_MaDonVi='B5') AND iNamLamViec=@iNamLamViec {1} {2} AND rDuPhong>0
GROUP BY iID_MaDonVi,sTenDonVi,sLNS

 ", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_ChoPhanCapB4(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi,
rSoTien=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2) AND iID_MaDonVi='L1'  AND iNamLamViec=@iNamLamViec {1} {2} AND rDuPhong>0
GROUP BY iID_MaDonVi,sTenDonVi

 ", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_ChoPhanCapB6(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi,
rSoTien=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2) AND iID_MaDonVi='B6'  AND iNamLamViec=@iNamLamViec {1} {2} AND rDuPhong>0
GROUP BY iID_MaDonVi,sTenDonVi

 ", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_ChoPhanCapC4(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi,sLNS,
rSoTien=ISNULL(SUM(rDuPhong/{3}),0)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2) AND iID_MaDonVi='C4'  AND iNamLamViec=@iNamLamViec {1} {2} AND rDuPhong>0
GROUP BY iID_MaDonVi,sTenDonVi,sLNS

 ", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_BaoDamMucTieuNgoaiTe(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT SUM(rHangNhap/{3}) as rHangNhap FROM DT_ChungTuChiTiet
WHERE iID_MaDonVi='56'
AND sM=7000
AND sTM=7049
AND sNG=11
AND iTrangThai in (1,2)
AND iID_MaChungTu='4D1DEE6A-710C-445F-A818-D0C63A29D105' {1} {2}
 ", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_BaoDamMucTieuTuChi(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT SUM(rTuChi/{3}) as rTuChi FROM DT_ChungTuChiTiet
WHERE sM=9050 AND sTM=9099
AND iNamLamViec=2017
AND sNG IN (10,12,01)
AND sTTM=00
AND iTrangThai in (1,2)
AND sGhiChu<>'' {1} {2}", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public DataTable DT_BaoDamNghiepVuTuChi(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT SUM(rTuChi/{3}) as rTuChi FROM DT_ChungTuChiTiet
WHERE iID_MaDonVi=76
AND sXauNoiMa='1020100-460-468-9050-9099-00-53' {1} {2}", "", DKPhongBan, DKDonVi, DVT);
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
            DataRow r;
            DataTable dataTX = DT_rptDuToan_TongHop_1010000(MaND, iID_MaPhongBan);
            fr.AddTable("TX", dataTX);
            DataTable dataNV = DT_rptDuToan_TongHop_NV(MaND, iID_MaPhongBan);
            fr.AddTable("NV", dataNV);
            decimal rHienVat = 0;
            if (dataNV.Rows.Count > 0)
                rHienVat = Convert.ToDecimal(dataNV.Rows[0]["rHienVat"]);
            fr.SetValue("rHienVat", rHienVat);

            DataTable dataXDCB = DT_rptDuToan_TongHop_XDCB(MaND, iID_MaPhongBan);
            fr.AddTable("XDCB", dataXDCB);
            DataTable dataDN = DT_rptDuToan_TongHop_DN(MaND, iID_MaPhongBan);
            fr.AddTable("DN", dataDN);
            DataTable dataKhac = DT_rptDuToan_TongHop_Khac(MaND, iID_MaPhongBan);
            fr.AddTable("Khac", dataKhac);
            DataTable dataBD = DT_rptDuToan_TongHop_BD(MaND, iID_MaPhongBan);
            fr.AddTable("BD", dataBD);
            //set du phong
            decimal rDuPhong = 0, rPhanCap = 0;
            if (dataBD.Rows.Count > 0)
            {
                rDuPhong = Convert.ToDecimal(dataBD.Rows[0]["rDuPhongC1"]);
                rPhanCap = Convert.ToDecimal(dataBD.Rows[0]["rPhanCap"]);
            }
            fr.SetValue("rDuPhongBD", rDuPhong);
            fr.SetValue("rPhanCap", rPhanCap);

            DataTable dataThuNop = DT_rptDuToan_ThuNop(MaND, iID_MaPhongBan);
            fr.AddTable("ThuNop", dataThuNop);
            DataTable dataTonKho = DT_rptDuToan_TongHop_TonKho(MaND, iID_MaPhongBan);
            fr.AddTable("TonKho", dataTonKho);

            DataTable dataNN = DT_rptDuToan_TongHop_NN(MaND, iID_MaPhongBan);
            fr.AddTable("NN", dataNN);

            DataTable dataKPKhac = DT_rptDuToan_TongHop_KPKhac(MaND, iID_MaPhongBan);
            fr.AddTable("KPKhac", dataKPKhac);


            DataTable dtChoPhanCapB1 = DT_ChoPhanCapB1(MaND, iID_MaPhongBan);
            fr.AddTable("ChoPhanCapB1", dtChoPhanCapB1);

            DataTable dtChoPhanCapB2 = DT_ChoPhanCapB2(MaND, iID_MaPhongBan);
            if (dtChoPhanCapB2.Rows.Count == 0)
            {
                r = dtChoPhanCapB2.NewRow();
                dtChoPhanCapB2.Rows.Add(r);
            }
            fr.AddTable("ChoPhanCapB2", dtChoPhanCapB2);

            DataTable dtChoPhanCapB3 = DT_ChoPhanCapB3(MaND, iID_MaPhongBan);
            if (dtChoPhanCapB3.Rows.Count == 0 )
            {
                r = dtChoPhanCapB3.NewRow();
                dtChoPhanCapB3.Rows.Add(r);                
            }
            if (dtChoPhanCapB3.Rows.Count == 2)
            {
                fr.SetValue("ChoPhanCapB3188", dtChoPhanCapB3.Rows[1]["rSoTien"]);
                fr.SetValue("ChoPhanCapB3103", dtChoPhanCapB3.Rows[0]["rSoTien"]);
            } else
            {
                if (dtChoPhanCapB3.Rows[0]["iID_MaDonVi"].ToString() == "C2")
                {
                    fr.SetValue("ChoPhanCapB3188", dtChoPhanCapB3.Rows[0]["rSoTien"]);
                    fr.SetValue("ChoPhanCapB3103", 0);
                }
                else
                {
                    fr.SetValue("ChoPhanCapB3188", 0);
                    fr.SetValue("ChoPhanCapB3103", dtChoPhanCapB3.Rows[0]["rSoTien"]);
                }                
            }
            
            DataTable dtChoPhanCapB4 = DT_ChoPhanCapB4(MaND, iID_MaPhongBan);
            if (dtChoPhanCapB4.Rows.Count == 0)
            {
                 r = dtChoPhanCapB4.NewRow();
                dtChoPhanCapB4.Rows.Add(r);
            }
            fr.AddTable("ChoPhanCapB4", dtChoPhanCapB4);

            DataTable dtChoPhanCapB6 = DT_ChoPhanCapB6(MaND, iID_MaPhongBan);
            if (dtChoPhanCapB6.Rows.Count == 0)
            {
                r = dtChoPhanCapB6.NewRow();
                dtChoPhanCapB6.Rows.Add(r);
            }
            fr.AddTable("ChoPhanCapB6", dtChoPhanCapB6);

            DataTable dtChoPhanCapC4 = DT_ChoPhanCapC4(MaND, iID_MaPhongBan);
            if (dtChoPhanCapC4.Rows.Count == 0)
            {
                r = dtChoPhanCapC4.NewRow();
                dtChoPhanCapC4.Rows.Add(r);
            }
            fr.AddTable("ChoPhanCapC4", dtChoPhanCapC4);

            DataTable dtMucTieuHangNhap = DT_BaoDamMucTieuNgoaiTe(MaND, iID_MaPhongBan);
            DataTable dtMucTieuTuChi = DT_BaoDamMucTieuTuChi(MaND, iID_MaPhongBan);
            DataTable dtMucTieuNghiepVu = DT_BaoDamNghiepVuTuChi(MaND, iID_MaPhongBan);
            if (dtMucTieuHangNhap.Rows.Count == 0)
            {
                r = dtMucTieuHangNhap.NewRow();
                dtMucTieuHangNhap.Rows.Add(r);
            }
            if (dtMucTieuTuChi.Rows.Count == 0)
            {
                r = dtMucTieuTuChi.NewRow();
                dtMucTieuTuChi.Rows.Add(r);
            }
            if (dtMucTieuNghiepVu.Rows.Count == 0)
            {
                r = dtMucTieuNghiepVu.NewRow();
                dtMucTieuNghiepVu.Rows.Add(r);
            }
            fr.AddTable("MucTieuHangNhap", dtMucTieuHangNhap);
            fr.AddTable("MucTieuTuChi", dtMucTieuTuChi);
            fr.AddTable("MucTieuNghiepVu", dtMucTieuNghiepVu);

            dataTX.Dispose();
            dataNV.Dispose();
            dataXDCB.Dispose();
            dataDN.Dispose();
            dataKhac.Dispose();
            dataThuNop.Dispose();
            dataTonKho.Dispose();
            dataBD.Dispose();
            dtChoPhanCapB1.Dispose();
            dtChoPhanCapB2.Dispose();
            dtChoPhanCapB3.Dispose();
            dtChoPhanCapB4.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan)
        {
            DataTable dtCauHinh = DuToan_ChungTuModels.GetCauHinhBia(MaND);
            if (dtCauHinh.Rows.Count > 0)
            {
                sTyGia = dtCauHinh.Rows[0]["sTyGia"].ToString();
                sSoQuyetDinh = dtCauHinh.Rows[0]["sSoQuyetDinh"].ToString();
                sThuCanDoi = dtCauHinh.Rows[0]["sThuCanDoi"].ToString();
                sThuQuanLy = dtCauHinh.Rows[0]["sThuQuanLy"].ToString();
                sThuNSNN = dtCauHinh.Rows[0]["sThuNSNN"].ToString();
                sNganSachBaoDam = dtCauHinh.Rows[0]["sNganSachBaoDam"].ToString();
                sLuong = dtCauHinh.Rows[0]["sLuong"].ToString();
                sNghiepVu = dtCauHinh.Rows[0]["sNghiepVu"].ToString();
                sXDCB = dtCauHinh.Rows[0]["sXDCB"].ToString();
                sDoanhNghiep = dtCauHinh.Rows[0]["sDoanhNghiep"].ToString();
                sNganSachKhac = dtCauHinh.Rows[0]["sNganSachKhac"].ToString();
                sNhaNuocGiao = dtCauHinh.Rows[0]["sNhaNuocGiao"].ToString();
                sKinhPhiKhac = dtCauHinh.Rows[0]["sKinhPhiKhac"].ToString();
            }
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_TongHop.xls";
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
            DataTable dtCauHinh = DuToan_ChungTuModels.GetCauHinhBia(MaND);
            String iNamLamViec = ReportModels.LayNamLamViec(MaND);
            String sDuongDan = "";
            if (iNamLamViec.Equals("2017"))
            {
                sDuongDan = sFilePath_2017;
            }
            else
                sDuongDan = sFilePath;
            if (dtCauHinh.Rows.Count > 0)
            {
                sTyGia = dtCauHinh.Rows[0]["sTyGia"].ToString();
                sSoQuyetDinh = dtCauHinh.Rows[0]["sSoQuyetDinh"].ToString();
                sThuCanDoi = dtCauHinh.Rows[0]["sThuCanDoi"].ToString();
                sThuQuanLy = dtCauHinh.Rows[0]["sThuQuanLy"].ToString();
                sThuNSNN = dtCauHinh.Rows[0]["sThuNSNN"].ToString();
                sNganSachBaoDam = dtCauHinh.Rows[0]["sNganSachBaoDam"].ToString();
                sLuong = dtCauHinh.Rows[0]["sLuong"].ToString();
                sNghiepVu = dtCauHinh.Rows[0]["sNghiepVu"].ToString();
                sXDCB = dtCauHinh.Rows[0]["sXDCB"].ToString();
                sDoanhNghiep = dtCauHinh.Rows[0]["sDoanhNghiep"].ToString();
                sNganSachKhac = dtCauHinh.Rows[0]["sNganSachKhac"].ToString();
                sNhaNuocGiao = dtCauHinh.Rows[0]["sNhaNuocGiao"].ToString();
                sKinhPhiKhac = dtCauHinh.Rows[0]["sKinhPhiKhac"].ToString();
            }
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sDuongDan), MaND, iID_MaPhongBan);
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

