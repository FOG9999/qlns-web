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
using System.Text.RegularExpressions;
using System.Collections;
using Viettel.Services;
using VIETTEL.Helpers;
using Viettel.Data;
using VIETTEL.Flexcel;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_TongHopCuc_THB2Controller : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_TongHop_THB2.xls";
        String sTyGia = "-1", sSoQuyetDinh = "-1", sThuCanDoi = "-1", sThuQuanLy = "-1", sThuNSNN = "-1", sNganSachBaoDam = "-1",
            sLuong = "-1", sNghiepVu = "-1", sDoanhNghiep = "-1", sXDCB = "-1", sNganSachKhac = "-1", sNhaNuocGiao = "-1", sKinhPhiKhac = "-1";
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        [Authorize]
        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_THB2.aspx";
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
        /// 
        [Authorize]
        public ActionResult EditSubmit(String ParentID)
        {
            String sMoTa = Request.Form[ParentID + "_sMoTa"];
            
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_THB2.aspx";
             ViewData["sMoTa"] = sMoTa;
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        [Authorize]
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
            fr = ReportModels.LayThongTinChuKy(fr,"rptDuToan_TongHop_THB2");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("Cap2", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);

            var count = Result.TotalPageCount();

            if (count > 1)
            {
                Result.AddPageFirstPage();
            }

            return Result;

        }

        
        public  DataTable DT_rptDuToan_TongHop_THB2(String MaND,String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);

            //BaoDam
            String DKCauHinhBaoDam = "";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            String[] arrCauHinhBaoDam = sNganSachBaoDam.Split(',');
            for (int i = 0; i < arrCauHinhBaoDam.Length; i++)
            {
                DKCauHinhBaoDam += "sLNS LIKE @sLNSBD" + i;
                if (i < arrCauHinhBaoDam.Length - 1)
                    DKCauHinhBaoDam += " OR ";
                cmd.Parameters.AddWithValue("@sLNSBD" + i, arrCauHinhBaoDam[i] + "%");
            }


            //tx
            String DKCauHinhTX = "";
            if (String.IsNullOrEmpty(sLuong)) sLuong = "-1";
            if (String.IsNullOrEmpty(sLuong)) sLuong = "-1";
            String[] arrCauHinhTX = sLuong.Split(',');
            for (int i = 0; i < arrCauHinhTX.Length; i++)
            {
                DKCauHinhTX += "sLNS LIKE @sLNSTX" + i;
                if (i < arrCauHinhTX.Length - 1)
                    DKCauHinhTX += " OR ";
                cmd.Parameters.AddWithValue("@sLNSTX" + i, arrCauHinhTX[i] + "%");
            }
            //NV
            String DKCauHinhNV = "";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            String[] arrCauHinhNV = sNghiepVu.Split(',');
            for (int i = 0; i < arrCauHinhNV.Length; i++)
            {
                DKCauHinhNV += "sLNS LIKE @sLNSNV" + i;
                if (i < arrCauHinhNV.Length - 1)
                    DKCauHinhNV += " OR ";
                cmd.Parameters.AddWithValue("@sLNSNV" + i, arrCauHinhNV[i] + "%");
            }
            //XDCB
            String DKCauHinhXDCB = "";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            String[] arrCauHinhXDCB = sXDCB.Split(',');
            for (int i = 0; i < arrCauHinhXDCB.Length; i++)
            {
                DKCauHinhXDCB += "sLNS LIKE @sLNSXDCB" + i;
                if (i < arrCauHinhXDCB.Length - 1)
                    DKCauHinhXDCB += " OR ";
                cmd.Parameters.AddWithValue("@sLNSXDCB" + i, arrCauHinhXDCB[i] + "%");
            }
            //DN
            String DKCauHinhDN = "";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            String[] arrCauHinhDN = sDoanhNghiep.Split(',');
            for (int i = 0; i < arrCauHinhDN.Length; i++)
            {
                DKCauHinhDN += "sLNS LIKE @sLNSDN" + i;
                if (i < arrCauHinhDN.Length - 1)
                    DKCauHinhDN += " OR ";
                cmd.Parameters.AddWithValue("@sLNSDN" + i, arrCauHinhDN[i] + "%");
            }

            //QPK
            String DKCauHinhQPK = "";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            String[] arrCauHinhQPK = sNganSachKhac.Split(',');
            for (int i = 0; i < arrCauHinhQPK.Length; i++)
            {
                DKCauHinhQPK += "sLNS LIKE @sLNSQPK" + i;
                if (i < arrCauHinhQPK.Length - 1)
                    DKCauHinhQPK += " OR ";
                cmd.Parameters.AddWithValue("@sLNSQPK" + i, arrCauHinhQPK[i] + "%");
            }

            //NN
            String DKCauHinhNN = "";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            String[] arrCauHinhNN = sNhaNuocGiao.Split(',');
            for (int i = 0; i < arrCauHinhNN.Length; i++)
            {
                DKCauHinhNN += "sLNS LIKE @sLNSNN" + i;
                if (i < arrCauHinhNN.Length - 1)
                    DKCauHinhNN += " OR ";
                cmd.Parameters.AddWithValue("@sLNSNN" + i, arrCauHinhNN[i] + "%");
            }
            //KPK
            String DKCauHinhKPK = "";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            String[] arrCauHinhKPK = sKinhPhiKhac.Split(',');
            for (int i = 0; i < arrCauHinhKPK.Length; i++)
            {
                DKCauHinhKPK += "sLNS LIKE @sLNSKPK" + i;
                if (i < arrCauHinhKPK.Length - 1)
                    DKCauHinhKPK += " OR ";
                cmd.Parameters.AddWithValue("@sLNSKPK" + i, arrCauHinhKPK[i] + "%");
            }

            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_THB2.sql");
            sql = sql.Replace("@@sLNSBD", DKCauHinhBaoDam);
            sql = sql.Replace("@@sLNSTX", DKCauHinhTX);
            sql = sql.Replace("@@sLNSNV", DKCauHinhNV);
            sql = sql.Replace("@@sLNSXDCB", DKCauHinhXDCB);
            sql = sql.Replace("@@sLNSDN", DKCauHinhDN);
            sql = sql.Replace("@@sLNSQPK", DKCauHinhQPK);
            sql = sql.Replace("@@sLNSNN", DKCauHinhNN);
            sql = sql.Replace("@@sLNSKHAC", DKCauHinhKPK);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;

            var dt = Connection.GetDataTable(cmd);
            return dt;
        }

        public  DataTable DT_rptDuToan_TongHop_THB2_ChoPhanCap(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            //BaoDam
            String DKCauHinhBaoDam = "";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            String[] arrCauHinhBaoDam = sNganSachBaoDam.Split(',');
            for (int i = 0; i < arrCauHinhBaoDam.Length; i++)
            {
                DKCauHinhBaoDam += "sLNS LIKE @sLNSBD" + i;
                if (i < arrCauHinhBaoDam.Length - 1)
                    DKCauHinhBaoDam += " OR ";
                cmd.Parameters.AddWithValue("@sLNSBD" + i, arrCauHinhBaoDam[i] + "%");
            }


            //tx
            String DKCauHinhTX = "";
            if (String.IsNullOrEmpty(sLuong)) sLuong = "-1";
            if (String.IsNullOrEmpty(sLuong)) sLuong = "-1";
            String[] arrCauHinhTX = sLuong.Split(',');
            for (int i = 0; i < arrCauHinhTX.Length; i++)
            {
                DKCauHinhTX += "sLNS LIKE @sLNSTX" + i;
                if (i < arrCauHinhTX.Length - 1)
                    DKCauHinhTX += " OR ";
                cmd.Parameters.AddWithValue("@sLNSTX" + i, arrCauHinhTX[i] + "%");
            }
            //NV
            String DKCauHinhNV = "";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            String[] arrCauHinhNV = sNghiepVu.Split(',');
            for (int i = 0; i < arrCauHinhNV.Length; i++)
            {
                DKCauHinhNV += "sLNS LIKE @sLNSNV" + i;
                if (i < arrCauHinhNV.Length - 1)
                    DKCauHinhNV += " OR ";
                cmd.Parameters.AddWithValue("@sLNSNV" + i, arrCauHinhNV[i] + "%");
            }
            //XDCB
            String DKCauHinhXDCB = "";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            String[] arrCauHinhXDCB = sXDCB.Split(',');
            for (int i = 0; i < arrCauHinhXDCB.Length; i++)
            {
                DKCauHinhXDCB += "sLNS LIKE @sLNSXDCB" + i;
                if (i < arrCauHinhXDCB.Length - 1)
                    DKCauHinhXDCB += " OR ";
                cmd.Parameters.AddWithValue("@sLNSXDCB" + i, arrCauHinhXDCB[i] + "%");
            }
            //DN
            String DKCauHinhDN = "";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            String[] arrCauHinhDN = sDoanhNghiep.Split(',');
            for (int i = 0; i < arrCauHinhDN.Length; i++)
            {
                DKCauHinhDN += "sLNS LIKE @sLNSDN" + i;
                if (i < arrCauHinhDN.Length - 1)
                    DKCauHinhDN += " OR ";
                cmd.Parameters.AddWithValue("@sLNSDN" + i, arrCauHinhDN[i] + "%");
            }

            //QPK
            String DKCauHinhQPK = "";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            String[] arrCauHinhQPK = sNganSachKhac.Split(',');
            for (int i = 0; i < arrCauHinhQPK.Length; i++)
            {
                DKCauHinhQPK += "sLNS LIKE @sLNSQPK" + i;
                if (i < arrCauHinhQPK.Length - 1)
                    DKCauHinhQPK += " OR ";
                cmd.Parameters.AddWithValue("@sLNSQPK" + i, arrCauHinhQPK[i] + "%");
            }

            //NN
            String DKCauHinhNN = "";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            String[] arrCauHinhNN = sNhaNuocGiao.Split(',');
            for (int i = 0; i < arrCauHinhNN.Length; i++)
            {
                DKCauHinhNN += "sLNS LIKE @sLNSNN" + i;
                if (i < arrCauHinhNN.Length - 1)
                    DKCauHinhNN += " OR ";
                cmd.Parameters.AddWithValue("@sLNSNN" + i, arrCauHinhNN[i] + "%");
            }
            //KPK
            String DKCauHinhKPK = "";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            String[] arrCauHinhKPK = sKinhPhiKhac.Split(',');
            for (int i = 0; i < arrCauHinhKPK.Length; i++)
            {
                DKCauHinhKPK += "sLNS LIKE @sLNSKPK" + i;
                if (i < arrCauHinhKPK.Length - 1)
                    DKCauHinhKPK += " OR ";
                cmd.Parameters.AddWithValue("@sLNSKPK" + i, arrCauHinhKPK[i] + "%");
            }
            String SQL = String.Format(@"SELECT a.iID_MaDonVi,sTen
,SUM(BD) AS BD
,SUM(TX) AS TX
,SUM(NV) AS NV
,SUM(XDCB) AS XDCB
,SUM(DN) AS DN
,SUM(QPKhac) AS QPKhac
,SUM(NN) AS NN
,SUM(NSKhac) AS NSKhac
,SUM(TonKho) AS TonKho
,SUM(HienVat) AS HienVat
FROM
(
SELECT 
iID_MaDonVi
,BD=SUM(CASE WHEN {2} AND MaLoai<>1 THEN (rDuPhong)/{0} ELSE 0 END)
,TX=SUM(CASE WHEN {3} THEN rDuPhong /{0} ELSE 0 END)
,NV=SUM(CASE WHEN {4} THEN rDuPhong/{0}  ELSE 0 END)
,XDCB=SUM(CASE WHEN {5} THEN rDuPhong/{0} ELSE 0 END)
,DN=SUM(CASE WHEN {6} THEN rDuPhong/{0} ELSE 0 END)
,QPKhac=SUM(CASE WHEN {7} THEN rDuPhong/{0} ELSE 0 END)
,NN=SUM(CASE WHEN {8} THEN rDuPhong/{0} ELSE 0 END)
,NSKhac=SUM(CASE WHEN {9} THEN rDuPhong/{0} ELSE 0 END)
,TonKho=0
,HienVat=0
FROM DT_ChungTuChiTiet
WHERE iTrangThai in (1,2)  AND iNamLamViec=@iNamLamViec {1} AND iID_MaDonVi<>'C1'
GROUP BY iID_MaDonVi

)  as a
INNER JOIN (SELECT iid_madonvi,sTen FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) as b
ON a.iID_MaDonVi=b.iID_MaDonVi
GROUP BY a.iID_MaDonVi,sTen
HAVING
SUM(BD)<>0 OR 
SUM(TX)<>0 OR 
SUM(NV)<>0 OR 
SUM(NV)<>0 OR 
SUM(XDCB)<>0 OR 
SUM(DN)<>0 OR 
SUM(QPKhac)<>0 OR 
SUM(NN)<>0 OR 
SUM(NSKhac)<>0 OR 
SUM(TonKho)<>0 OR 
SUM(HienVat)<>0 
ORDER BY a.iID_MaDonVi", DVT, DKPhongBan, DKCauHinhBaoDam, DKCauHinhTX, DKCauHinhNV, DKCauHinhXDCB, DKCauHinhDN, DKCauHinhQPK, DKCauHinhNN, DKCauHinhKPK);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public DataTable DT_rptDuToan_TongHop_THB2_DuPhong(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            //BaoDam
            String DKCauHinhBaoDam = "";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            if (String.IsNullOrEmpty(sNganSachBaoDam)) sNganSachBaoDam = "-1";
            String[] arrCauHinhBaoDam = sNganSachBaoDam.Split(',');
            for (int i = 0; i < arrCauHinhBaoDam.Length; i++)
            {
                DKCauHinhBaoDam += "sLNS LIKE @sLNSBD" + i;
                if (i < arrCauHinhBaoDam.Length - 1)
                    DKCauHinhBaoDam += " OR ";
                cmd.Parameters.AddWithValue("@sLNSBD" + i, arrCauHinhBaoDam[i] + "%");
            }


            //tx
            String DKCauHinhTX = "";
            if (String.IsNullOrEmpty(sLuong)) sLuong = "-1";
            if (String.IsNullOrEmpty(sLuong)) sLuong = "-1";
            String[] arrCauHinhTX = sLuong.Split(',');
            for (int i = 0; i < arrCauHinhTX.Length; i++)
            {
                DKCauHinhTX += "sLNS LIKE @sLNSTX" + i;
                if (i < arrCauHinhTX.Length - 1)
                    DKCauHinhTX += " OR ";
                cmd.Parameters.AddWithValue("@sLNSTX" + i, arrCauHinhTX[i] + "%");
            }
            //NV
            String DKCauHinhNV = "";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            if (String.IsNullOrEmpty(sNghiepVu)) sNghiepVu = "-1";
            String[] arrCauHinhNV = sNghiepVu.Split(',');
            for (int i = 0; i < arrCauHinhNV.Length; i++)
            {
                DKCauHinhNV += "sLNS LIKE @sLNSNV" + i;
                if (i < arrCauHinhNV.Length - 1)
                    DKCauHinhNV += " OR ";
                cmd.Parameters.AddWithValue("@sLNSNV" + i, arrCauHinhNV[i] + "%");
            }
            //XDCB
            String DKCauHinhXDCB = "";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            if (String.IsNullOrEmpty(sXDCB)) sXDCB = "-1";
            String[] arrCauHinhXDCB = sXDCB.Split(',');
            for (int i = 0; i < arrCauHinhXDCB.Length; i++)
            {
                DKCauHinhXDCB += "sLNS LIKE @sLNSXDCB" + i;
                if (i < arrCauHinhXDCB.Length - 1)
                    DKCauHinhXDCB += " OR ";
                cmd.Parameters.AddWithValue("@sLNSXDCB" + i, arrCauHinhXDCB[i] + "%");
            }
            //DN
            String DKCauHinhDN = "";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            if (String.IsNullOrEmpty(sDoanhNghiep)) sDoanhNghiep = "-1";
            String[] arrCauHinhDN = sDoanhNghiep.Split(',');
            for (int i = 0; i < arrCauHinhDN.Length; i++)
            {
                DKCauHinhDN += "sLNS LIKE @sLNSDN" + i;
                if (i < arrCauHinhDN.Length - 1)
                    DKCauHinhDN += " OR ";
                cmd.Parameters.AddWithValue("@sLNSDN" + i, arrCauHinhDN[i] + "%");
            }

            //QPK
            String DKCauHinhQPK = "";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            if (String.IsNullOrEmpty(sNganSachKhac)) sNganSachKhac = "-1";
            String[] arrCauHinhQPK = sNganSachKhac.Split(',');
            for (int i = 0; i < arrCauHinhQPK.Length; i++)
            {
                DKCauHinhQPK += "sLNS LIKE @sLNSQPK" + i;
                if (i < arrCauHinhQPK.Length - 1)
                    DKCauHinhQPK += " OR ";
                cmd.Parameters.AddWithValue("@sLNSQPK" + i, arrCauHinhQPK[i] + "%");
            }

            //NN
            String DKCauHinhNN = "";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            if (String.IsNullOrEmpty(sNhaNuocGiao)) sNhaNuocGiao = "-1";
            String[] arrCauHinhNN = sNhaNuocGiao.Split(',');
            for (int i = 0; i < arrCauHinhNN.Length; i++)
            {
                DKCauHinhNN += "sLNS LIKE @sLNSNN" + i;
                if (i < arrCauHinhNN.Length - 1)
                    DKCauHinhNN += " OR ";
                cmd.Parameters.AddWithValue("@sLNSNN" + i, arrCauHinhNN[i] + "%");
            }
            //KPK
            String DKCauHinhKPK = "";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            if (String.IsNullOrEmpty(sKinhPhiKhac)) sKinhPhiKhac = "-1";
            String[] arrCauHinhKPK = sKinhPhiKhac.Split(',');
            for (int i = 0; i < arrCauHinhKPK.Length; i++)
            {
                DKCauHinhKPK += "sLNS LIKE @sLNSKPK" + i;
                if (i < arrCauHinhKPK.Length - 1)
                    DKCauHinhKPK += " OR ";
                cmd.Parameters.AddWithValue("@sLNSKPK" + i, arrCauHinhKPK[i] + "%");
            }
            String SQL = String.Format(@"SELECT a.iID_MaDonVi,sTen
,SUM(BD) AS BD
,SUM(TX) AS TX
,SUM(NV) AS NV
,SUM(XDCB) AS XDCB
,SUM(DN) AS DN
,SUM(QPKhac) AS QPKhac
,SUM(NN) AS NN
,SUM(NSKhac) AS NSKhac
,SUM(TonKho) AS TonKho
,SUM(HienVat) AS HienVat
FROM
(
SELECT 
iID_MaDonVi
,BD=SUM(CASE WHEN {2} AND MaLoai<>1 THEN (rDuPhong)/{0} ELSE 0 END)
,TX=SUM(CASE WHEN {3} THEN rDuPhong /{0} ELSE 0 END)
,NV=SUM(CASE WHEN {4} THEN rDuPhong/{0}  ELSE 0 END)
,XDCB=SUM(CASE WHEN {5} THEN rDuPhong/{0} ELSE 0 END)
,DN=SUM(CASE WHEN {6} THEN rDuPhong/{0} ELSE 0 END)
,QPKhac=SUM(CASE WHEN {7} THEN rDuPhong/{0} ELSE 0 END)
,NN=SUM(CASE WHEN {8} THEN rDuPhong/{0} ELSE 0 END)
,NSKhac=SUM(CASE WHEN {9} THEN rDuPhong/{0} ELSE 0 END)
,TonKho=0
,HienVat=0
FROM DT_ChungTuChiTiet
WHERE iTrangThai in (1,2)  AND iNamLamViec=@iNamLamViec {1} AND iID_MaDonVi='C1'
GROUP BY iID_MaDonVi

)  as a
INNER JOIN (SELECT iid_madonvi,sTen FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) as b
ON a.iID_MaDonVi=b.iID_MaDonVi
GROUP BY a.iID_MaDonVi,sTen
HAVING
SUM(BD)<>0 OR 
SUM(TX)<>0 OR 
SUM(NV)<>0 OR 
SUM(NV)<>0 OR 
SUM(XDCB)<>0 OR 
SUM(DN)<>0 OR 
SUM(QPKhac)<>0 OR 
SUM(NN)<>0 OR 
SUM(NSKhac)<>0 OR 
SUM(TonKho)<>0 OR 
SUM(HienVat)<>0 
ORDER BY a.iID_MaDonVi", DVT, DKPhongBan, DKCauHinhBaoDam, DKCauHinhTX, DKCauHinhNV, DKCauHinhXDCB, DKCauHinhDN, DKCauHinhQPK, DKCauHinhNN, DKCauHinhKPK);
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
            DataTable data = DT_rptDuToan_TongHop_THB2(MaND, iID_MaPhongBan);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            var dtLoai = HamChung.SelectDistinct("Loai", data, "iLoai", "iLoai,sLoai");
            fr.AddTable("Loai", dtLoai);
            fr.AddRelationship("Loai", "ChiTiet", "iLoai".Split(','), "iLoai".Split(','));
            dtLoai.Dispose();
            data.Dispose();

            DataTable dtChoPhanCap = DT_rptDuToan_TongHop_THB2_ChoPhanCap(MaND, iID_MaPhongBan);
            fr.AddTable("ChoPhanCap", dtChoPhanCap);
            dtChoPhanCap.Dispose();

            DataTable dtDuPhong = DT_rptDuToan_TongHop_THB2_DuPhong(MaND, iID_MaPhongBan);
            fr.AddTable("DuPhong", dtDuPhong);
            dtDuPhong.Dispose();

            DataTable dt = new DataTable();
            dt.Columns.Add("sGhiChu", typeof(String));
            int soChu1Trang = 80;

            String SQL =
                String.Format(
                    @"SELECT sTen FROM KT_DanhMucThamSo_BaoCao  WHERE iID_MaBaoCao=@iID_MaBaoCao AND iNamLamViec=@iNamLamViec");
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaBaoCao", ReportModels.LayNamLamViec(MaND) + "rptDuToan_TongHop_THB2");
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            String sGhiChu = Connection.GetValueString(cmd, "");
            ArrayList arrDongTong = new ArrayList();
            String[] arrDong = Regex.Split(sGhiChu, "&#10;");
            for (int i = 0; i < arrDong.Length; i++)
            {
                if (arrDong[i] != "")
                {
                    r = dt.NewRow();
                    r["sGhiChu"] = arrDong[i];
                    dt.Rows.Add(r);
                }
            }
            fr.AddTable("GhiChu", dt);
            String sGhiChuText = "";
            if (dt.Rows.Count > 0) sGhiChuText = " * Ghi chú: ";
            fr.SetValue("GhiChuText", sGhiChuText);
            dt.Dispose();
        }
        [Authorize]
        public ActionResult Update_GhiChu(String sMoTa)
        {
            String SQL = "";
            String MaND = User.Identity.Name;
            SQL = String.Format("DELETE KT_DanhMucThamSo_BaoCao WHERE iID_MaBaoCao=@iID_MaBaoCao AND iNamLamViec=@iNamLamViec");
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaBaoCao", ReportModels.LayNamLamViec(MaND) + "rptDuToan_TongHop_THB2");
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            Connection.UpdateDatabase(cmd);

            SQL = "INSERT INTO KT_DanhMucThamSo_BaoCao  (iID_MaBaoCao,sTenBaoCao,sID_MaNguoiDungTao,sTen,iNamLamViec) values(@iID_MaBaoCao,@sTenBaoCao,@sID_MaNguoiDungTao ,@sTen,@iNamLamViec)";
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaBaoCao", ReportModels.LayNamLamViec(MaND) + "rptDuToan_TongHop_THB2");
            cmd.Parameters.AddWithValue("@sTenBaoCao", "rptDuToan_TongHop_THB2");
            cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", MaND);
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            sMoTa = sMoTa.Replace("^", "&#10;");
            cmd.Parameters.AddWithValue("@sTen", sMoTa);
            Connection.UpdateDatabase(cmd);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        [Authorize]
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
                clsResult.FileName = "DuToan_TongHop_NganSachQuocPhong.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        public ActionResult ViewPDF(String MaND, String iID_MaPhongBan)
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

