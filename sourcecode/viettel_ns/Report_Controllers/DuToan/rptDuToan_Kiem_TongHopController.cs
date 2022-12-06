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
    public class rptDuToan_Kiem_TongHopController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_Kiem_TongHop.xls";
        private const String sFilePath_NN = "/Report_ExcelFrom/DuToan/rptDuToan_Kiem_TongHop_NN.xls";


        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_Kiem_TongHop.aspx";
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
            String iID_MaNguon = Request.Form[ParentID + "_iID_MaNguon"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["iID_MaNguon"] = iID_MaNguon;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_Kiem_TongHop.aspx";
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
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_Kiem_TongHop");
            fr.SetValue("Nam", iNamLamViec);
            if (iID_MaPhongBan != "-1")
                fr.SetValue("sTenDonVi", "B " + sTenDonVi);
            else
            {
                fr.SetValue("sTenDonVi", "");
            }
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Cap2", ReportModels.CauHinhTenDonViSuDung(2, MaND));
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
        public static DataTable DT_rptDuToan_Kiem_TongHop(String MaND, String iID_MaPhongBan)
        {
            String SQL = "";
            SqlCommand cmd;
            DataTable dt;
           
            SQL = String.Format(@"SELECT iID_MaDonVi,sTen,iID_MaPhongBan='' FROM NS_DonVi
WHERE iNamLamViec_DonVi=@iNamLamViec AND iID_MaDonVi IN (SELECT DISTINCT iID_MaDonVi FROM DT_ChungTuChiTiet WHERE iTrangThai=1)
ORDER BY iID_MaDonVi");
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec",ReportModels.LayNamLamViec(MaND));
            dt = Connection.GetDataTable(cmd);
            String B6 = "30,34,35,50,69,70,71,72,73,78,88,89,90,91,92,93,94";
            String B7 = "10,11,12,13,14,15,17,19,21,22,23,24,29,31,33,41,42,43,44,45,46,47,81,82,83,84,87,95,96,97,98";
            String B10 = "02,03,51,52,53,55,56,57,61,65,75,76,77,79,99";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String iID_MaDonVi = dt.Rows[i]["iID_MaDonVi"].ToString();
                if (B6.Contains(iID_MaDonVi))
                {
                    dt.Rows[i]["iID_MaPhongBan"] = "06";
                }
                else if (B7.Contains(iID_MaDonVi))
                {
                    dt.Rows[i]["iID_MaPhongBan"] = "07";
                }
                else if (B10.Contains(iID_MaDonVi))
                {
                    dt.Rows[i]["iID_MaPhongBan"] = "10";
                }
                else
                    dt.Rows[i]["iID_MaPhongBan"] = "DuPhong";
            }
            cmd = new SqlCommand();
            String DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            String DKPhongBan = ThuNopModels.DKPhongBan_Dich(MaND, cmd, iID_MaPhongBan);

            int DVT = 1000000;
            SQL = String.Format(@"
SELECT iID_MaDonVi
,SUM(THUNOP_DT) as THUNOP_DT
,SUM(THUNOP_DN) as THUNOP_DN
,SUM(THUNOP_B17) as THUNOP_B17
,SUM(BD_1A) as BD_1A
,SUM(BD_1B) as BD_1B
,SUM(TX_2A) as TX_2A
,SUM(TX_2A_DN) as TX_2A_DN
,SUM(NV_2B) as NV_2B
,SUM(NV_2B_DN) as NV_2B_DN
,SUM(NV_KT) as NV_KT
,SUM(NV_CK) as NV_CK
,SUM(TVIEN_2D) as TVIEN_2D
,SUM(XDCB_3) as XDCB_3
,SUM(DN_4A) as DN_4A
,SUM(QPKHAC_B8) as QPKHAC_B8
,SUM(QPKHAC_4B) as QPKHAC_4B
,SUM(THENGHE) as THENGHE
,SUM(NCC) as NCC
,SUM(MOLSI) as MOLSI
,SUM(THEBHYT) as THEBHYT
,SUM(TCKK_HSQ) as TCKK_HSQ
,SUM(QLHC_KSAT) as QLHC_KSAT
,SUM(QLHC_KSAT_PC) as QLHC_KSAT_PC
,SUM(QLHC_DTRA) as QLHC_DTRA
,SUM(QLHC_DTRA_PC) as QLHC_DTRA_PC
,SUM(QLHC_BAOVE) as QLHC_BAOVE
,SUM(QLHC_BAOVE_PC) as QLHC_BAOVE_PC
,SUM(QLHC_TAN) as QLHC_TAN
,SUM(QLHC_TAN_PC) as QLHC_TAN_PC
,SUM(QLHC_THA) as QLHC_THA
,SUM(QLHC_THA_PC) as QLHC_THA_PC
FROM(
SELECT 
iID_MaDonVi,
THUNOP_DT=SUM(CASE WHEN sLNS LIKE '8%' AND iID_MaPhongBan IN ('10','07') THEN rTuChi/{0} ELSE 0 END)
,THUNOP_DN=SUM(CASE WHEN sLNS LIKE '8%' AND iID_MaPhongBan IN ('06') THEN rTuChi/{0} ELSE 0 END)
,THUNOP_B17=SUM(CASE WHEN sLNS LIKE '8%' AND iID_MaPhongBan IN ('17') THEN rTuChi/{0} ELSE 0 END)
,BD_1A=SUM(CASE WHEN sLNS LIKE '1040100%' AND (MaLoai='' OR MaLoai='2')THEN (rHienVat+rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,BD_1B=SUM(CASE WHEN sLNS=1040100 AND iKyThuat=1 AND MaLoai=1 THEN (rHienVat+rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,TX_2A=SUM(CASE WHEN sLNS IN ('1010000') AND (iID_MaPhongBan<>'06' OR (iID_MaPhongBan='06' AND iID_MaDonVi IN ('30','34','35','50','69','70','71','72','73','78','88','89','90','91','92','93','94'))) THEN (rHienVat+rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,TX_2A_DN=SUM(CASE WHEN sLNS IN ('1010000') AND iID_MaPhongBan='06' AND iID_MaDonVi NOT IN ('30','34','35','50','69','70','71','72','73','78','88','89','90','91','92','93','94') THEN (rHienVat+rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,NV_2B=SUM(CASE WHEN  (sLNS='1020100' OR sLNS='1020000')  AND (iID_MaPhongBan<>'06' OR (iID_MaPhongBan='06' AND iID_MaDonVi IN ('30','34','35','50','69','70','71','72','73','78','88','89','90','91','92','93','94'))) THEN (rHienVat+rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,NV_2B_DN=SUM(CASE WHEN (sLNS='1020100' OR sLNS='1020000') AND iID_MaPhongBan='06' AND iID_MaDonVi NOT IN ('30','34','35','50','69','70','71','72','73','78','88','89','90','91','92','93','94') THEN (rHienVat+rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,NV_KT=SUM(CASE WHEN sLNS='1020100' AND sM IN (6900,7000,7750,9050) THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,NV_CK=SUM(CASE WHEN sLNS IN ('1050100') THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,TVIEN_2D=SUM(CASE WHEN sLNS IN ('1020200') THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,XDCB_3=SUM(CASE WHEN sLNS IN ('1030100')  THEN  (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,DN_4A=SUM(CASE WHEN sLNS IN ('1050000')  THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QPKHAC_B8=SUM(CASE WHEN sLNS LIKE '109%' AND iID_MaPhongBan='08'  THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QPKHAC_4B=SUM(CASE WHEN sLNS LIKE '109%' AND iID_MaPhongBan!='08'  THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,THENGHE=SUM(CASE WHEN sLNS LIKE '2041100%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,NCC=SUM(CASE WHEN sLNS LIKE '2060101%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,MOLSI=SUM(CASE WHEN sLNS LIKE '2060300%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,THEBHYT=SUM(CASE WHEN sLNS LIKE '2060701%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,TCKK_HSQ=SUM(CASE WHEN sLNS LIKE '2060708%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_KSAT=SUM(CASE WHEN sLNS LIKE '2070000%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_KSAT_PC=0
,QLHC_DTRA=SUM(CASE WHEN sLNS LIKE '2070100%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_DTRA_PC=0
,QLHC_BAOVE=SUM(CASE WHEN sLNS LIKE '2070800%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_BAOVE_PC=0
,QLHC_TAN=SUM(CASE WHEN sLNS LIKE '2070300%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_TAN_PC=0
,QLHC_THA=SUM(CASE WHEN sLNS LIKE '2070400%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_THA_PC=0
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 {1} {2}
AND iID_MaDonVi<>''
GROUP BY iID_MaDonVi
UNION

SELECT 
iID_MaDonVi,
THUNOP_DT=0
,THUNOP_DN=0
,THUNOP_B17=0
,BD_1A=0
,BD_1B=SUM(CASE WHEN sLNS='1020100' AND MaLoai<>'1' AND MaLoai <>'3'  THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,TX_2A=0
,TX_2A_DN=0
,NV_2B=0
,NV_2B_DN=0
,NV_KT=0
,NV_CK=0
,TVIEN_2D=0
,XDCB_3=0
,DN_4A=0
,QPKHAC_B8=0
,QPKHAC_4B=0
,THENGHE=0
,NCC=0
,MOLSI=0
,THEBHYT=0
,TCKK_HSQ=0
,QLHC_KSAT=0
,QLHC_KSAT_PC=SUM(CASE WHEN sLNS LIKE '2070000%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_DTRA=0
,QLHC_DTRA_PC=SUM(CASE WHEN sLNS LIKE '2070100%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_BAOVE=0
,QLHC_BAOVE_PC=SUM(CASE WHEN sLNS LIKE '2070800%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_TAN=0
,QLHC_TAN_PC=SUM(CASE WHEN sLNS LIKE '2070300%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
,QLHC_THA=0
,QLHC_THA_PC=SUM(CASE WHEN sLNS LIKE '2070400%' THEN (rTuChi+rTonKho+rHangNhap+rHangMua+rPhanCap+rDuPhong)/{0} ELSE 0 END)
FROM DT_ChungTuChiTiet_PhanCap
WHERE  iNamLamViec=@iNamLamViec AND iTrangThai=1 {1} {2}
AND iID_MaDonVi<>''
GROUP BY iID_MaDonVi) as a
GROUP BY iID_MaDonVi
HAVING 
SUM(THUNOP_DT) <>0 OR
SUM(THUNOP_DN) <>0 OR
SUM(THUNOP_B17) <>0 OR
SUM(BD_1A) <>0 OR
SUM(BD_1B)<>0 OR
SUM(TX_2A) <>0 OR
SUM(TX_2A_DN) <>0 OR
SUM(NV_2B) <>0 OR
SUM(NV_2B_DN) <>0 OR
SUM(NV_KT) <>0 OR
SUM(NV_CK) <>0 OR
SUM(TVIEN_2D) <>0 OR
SUM(XDCB_3) <>0 OR
SUM(DN_4A) <>0 OR
SUM(QPKHAC_B8) <>0 OR
SUM(QPKHAC_4B) <>0 OR
SUM(THENGHE) <>0 OR
SUM(NCC) <>0 OR
SUM(MOLSI) <>0 OR
SUM(THEBHYT) <>0 OR
SUM(TCKK_HSQ) <>0 OR
SUM(QLHC_KSAT) <>0 OR
SUM(QLHC_KSAT_PC) <>0 OR
SUM(QLHC_DTRA) <>0 OR
SUM(QLHC_DTRA_PC) <>0 OR
SUM(QLHC_BAOVE) <>0 OR
SUM(QLHC_BAOVE_PC) <>0 OR
SUM(QLHC_TAN) <>0 OR
SUM(QLHC_TAN_PC) <>0 OR
SUM(QLHC_THA) <>0 OR
SUM(QLHC_THA_PC) <>0 
ORDER BY iID_MaDonVi", DVT, DKPhongBan, DKDonVi);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);

            for (int i = 1; i < dtChungTuChiTiet.Columns.Count; i++)
            {
                dt.Columns.Add(dtChungTuChiTiet.Columns[i].ColumnName, typeof(Decimal));
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dtChungTuChiTiet.Rows.Count; j++)
                {
                    if (dt.Rows[i]["iID_MaDonVi"] .Equals(dtChungTuChiTiet.Rows[j]["iID_MaDonVi"]))
                    {
                        for (int c = 1;c<dtChungTuChiTiet.Columns.Count; c++)
                        {
                            dt.Rows[i][dtChungTuChiTiet.Columns[c].ColumnName] = dtChungTuChiTiet.Rows[j][dtChungTuChiTiet.Columns[c].ColumnName];
                        }
                        break;
                    }
                }
            }
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
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_Kiem_TongHop(MaND, iID_MaPhongBan);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();

            DataTable dtPhongBan = HamChung.SelectDistinct("PhongBan", data, "iID_MaPhongBan", "iID_MaPhongBan");
            fr.AddTable("PhongBan", dtPhongBan);
            dtPhongBan.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan, String iID_MaNguon)
        {
            clsExcelResult clsResult = new clsExcelResult();
            String DuongDan = "";
            if (iID_MaNguon == "1")
            {
                DuongDan = sFilePath;
            }
            else
            {
                DuongDan = sFilePath_NN;
            }

            ExcelFile xls = CreateReport(Server.MapPath(DuongDan), MaND, iID_MaPhongBan);

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
        public ActionResult ViewPDF(String MaND, String iID_MaPhongBan,String iID_MaNguon)
        {
            HamChung.Language();
            String DuongDan = "";
            if (iID_MaNguon == "1")
            {
                DuongDan = sFilePath;
            }
            else
            {
                DuongDan = sFilePath_NN;
            }

            ExcelFile xls = CreateReport(Server.MapPath(DuongDan), MaND, iID_MaPhongBan);
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

