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

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_TongHop_TungDonViController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_TongHop_TungDonVi.xls";
        public const int DVT = 1000;
        String sTyGia = "-1", sSoQuyetDinh = "-1", sThuCanDoi = "-1", sThuQuanLy = "-1", sThuNSNN = "-1", sNganSachBaoDam = "-1",
           sLuong = "-1", sNghiepVu = "-1", sDoanhNghiep = "-1", sXDCB = "-1", sNganSachKhac = "-1", sNhaNuocGiao = "-1", sKinhPhiKhac = "-1";
        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_TungDonVi.aspx";
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
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_TungDonVi.aspx";
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
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_TongHop_TungDonVi");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
           
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
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
        public  DataTable DT_rptDuToan_TongHop_1010000(String MaND,String iID_MaDonVi)
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
            String SQL = String.Format(@"SELECT 
rTuChi=SUM(rTuChi/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND ({4}) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 ", iID_MaDonVi,DKPhongBan,DKDonVi,DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public  DataTable DT_rptDuToan_TongHop_NV(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
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
            String SQL = String.Format(@"SELECT 
rTuChi=SUM(rTuChi)
,rHienVat=SUM(rHienVat)
,rTuChiTuyVien=SUM(rTuChiTuyVien)
FROM(
SELECT 
rTuChi=SUM((rTuChi+rHangNhap+rHangMua)/{3})-SUM(CASE WHEN sLNS IN (1020200) THEN rHangNhap/{3} ELSE 0 END),
rTuChiTuyVien=SUM(CASE WHEN sLNS IN (1020200) THEN rHangNhap/{3} ELSE 0 END)
,rHienVat=SUM(rHienVat/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( {4}) 
AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}

UNION

SELECT 
rTuChi=SUM(rTuChi/{3})
,rTuChiTuyVien=0
,rHienVat=SUM(rHienVat/{3})
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND( sLNS IN (1020000,1020100) )
AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}) as a



 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public  DataTable DT_rptDuToan_TongHop_XDCB(String MaND, String iID_MaDonVi)
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
            String SQL = String.Format(@"SELECT 
rTuChi=SUM(rTuChi/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( {4}) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public DataTable DT_rptDuToan_TongHop_DN(String MaND, String iID_MaDonVi)
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
            String SQL = String.Format(@"SELECT 
rTuChi=SUM(rTuChi/{3})+SUM(rhangNhap/{3})+SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( {4}) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public  DataTable DT_rptDuToan_TongHop_Khac(String MaND, String iID_MaDonVi)
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
            String SQL = String.Format(@"SELECT 
rTuChi=SUM(rTuChi/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( {4}) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public  DataTable DT_rptDuToan_ThuNop(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            //  DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
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
            String SQL = String.Format(@"SELECT 
ThuCanDoi=SUM(CASE WHEN {3} THEN rTuChi/{2} ELSE 0 END)
,ThuQuanLy=SUM(CASE WHEN {4} THEN rTuChi/{2} ELSE 0 END)
,ThuNhaNuoc=SUM(CASE WHEN {5} THEN rTuChi/{2} ELSE 0 END)
FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1 AND  sLNS LIKE '8%' AND iID_MaDonVi =@iID_MaDonVi  AND iNamLamViec=@iNamLamViec {0} {1}", DKPhongBan, DKDonVi, DVT,DKCauHinh,DKCauHinh1,DKCauHinh2);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public  DataTable DT_rptDuToan_TongHop_BD(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
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
            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
            SqlCommand cmdNganh = new SqlCommand(sql);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            string iID_MaNganhMLNS = "";
            if (dtNganhChon.Rows.Count > 0)
            {
                iID_MaNganhMLNS = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
            }
            if (String.IsNullOrEmpty(iID_MaNganhMLNS)) iID_MaNganhMLNS = "-1";
            String SQL = String.Format(@"SELECT 
rTuChi=ISNULL(SUM((rTuChi+rHangMua)/{3}),0)
,rPhanCap=ISNULL(SUM(rPhanCap/{3}),0)
,rDuPhong=ISNULL(SUM(rDuPhong/{3}),0)
,rHangNhap=ISNULL(SUM(rHangNhap/{3}),0)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND({4}) AND sNG IN ({0}) AND MaLoai<>1   AND iNamLamViec=@iNamLamViec {1} {2}
 ", iID_MaNganhMLNS, DKPhongBan, DKDonVi, DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public static DataTable DT_rptDuToan_TongHop_TonKho(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String SQL = String.Format(@"SELECT 
rTuChi=SUM(rTonKho/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public  DataTable DT_rptDuToan_TongHop_NN(String MaND, String iID_MaDonVi)
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
            String SQL = String.Format(@"SELECT SUM(rTuChi) as rTuChi FROM(SELECT 
rTuChi=SUM(rTuChi/{3})+SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( {4}) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
UNION
SELECT 
rTuChi=SUM(rTuChi/{3})
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND( {4}) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}) as a

 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public  DataTable DT_rptDuToan_TongHop_KPKhac(String MaND, String iID_MaDonVi)
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
            String SQL = String.Format(@"SELECT 
rTuChi=SUM(rTuChi/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( {4}) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT,DKCauHinh);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public static String LayMoTa(String sLNS)
        {
            String sMoTa = "";

            String SQL = String.Format(@"SELECT sMoTa FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sLNS={0}", sLNS);
            sMoTa = Connection.GetValueString(SQL, "");
            return sMoTa;
        }
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaDonVi)
        {
            DataRow r;
            DataTable dataTX = DT_rptDuToan_TongHop_1010000(MaND, iID_MaDonVi);
            fr.AddTable("TX", dataTX);
            DataTable dataNV = DT_rptDuToan_TongHop_NV(MaND, iID_MaDonVi);
            fr.AddTable("NV", dataNV);
            DataTable dataXDCB = DT_rptDuToan_TongHop_XDCB(MaND, iID_MaDonVi);
            fr.AddTable("XDCB", dataXDCB);
            DataTable dataDN = DT_rptDuToan_TongHop_DN(MaND, iID_MaDonVi);
            fr.AddTable("DN", dataDN);
            DataTable dataKhac = DT_rptDuToan_TongHop_Khac(MaND, iID_MaDonVi);
            fr.AddTable("Khac", dataKhac);
            DataTable dataBD = DT_rptDuToan_TongHop_BD(MaND, iID_MaDonVi);
            fr.AddTable("BD", dataBD);
            DataTable dataThuNop = DT_rptDuToan_ThuNop(MaND, iID_MaDonVi);
            fr.AddTable("ThuNop", dataThuNop);
            DataTable dataTonKho = DT_rptDuToan_TongHop_TonKho(MaND, iID_MaDonVi);
            fr.AddTable("TonKho", dataTonKho);

            DataTable dataNN = DT_rptDuToan_TongHop_NN(MaND, iID_MaDonVi);
            fr.AddTable("NN", dataNN);

            DataTable dataKPKhac = DT_rptDuToan_TongHop_KPKhac(MaND, iID_MaDonVi);
            fr.AddTable("KPKhac", dataKPKhac);
            dataTX.Dispose();
            dataNV.Dispose();
            dataXDCB.Dispose();
            dataDN.Dispose();
            dataKhac.Dispose();
            dataThuNop.Dispose();
            dataTonKho.Dispose();
            dataBD.Dispose();

            String TyGia = "";
            bool check = true;
            if (dataBD.Rows.Count > 0)
            {
                if (dataBD.Rows[0]["rHangNhap"] != null && Convert.ToDecimal(dataBD.Rows[0]["rHangNhap"]) != 0)
                    check = false;
            }
            if (dataNV.Rows.Count > 0)
            {
                if (dataNV.Rows[0]["rTuChiTuyVien"] != null && Convert.ToDecimal(dataNV.Rows[0]["rTuChiTuyVien"]) != 0)
                    check = false;
            }
            if(check==false)
            TyGia = "(Tỷ giá : " + sTyGia + " VND/USD)";
            fr.SetValue("sTyGia", TyGia);

            
          
          
        }
      
        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi)
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
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath),MaND,iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1010000_TungDonVi.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi)
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

