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

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1030100_TungDonViController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private  String sFilePath = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_1030100_TungDonVi.xls";
        String iCapTongHop = "";
        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1030100_TungDonVi.aspx";
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
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1030100_TungDonVi.aspx";
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
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1020100_TungDonVi");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

            var loaiKhoan = HamChung.GetLoaiKhoanText("1020100");
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
        public  DataTable DT_rptDuToan_1020100_TungDonVi(String MaND,String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_Dich(MaND, cmd,"-1");
            int DVT = 1000;
            String iID_MaPhongBanQuanLy = DonViModels.getPhongBanCuaDonVi(iID_MaDonVi, User.Identity.Name);
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,SUM(rTuChi) as rTuChi,SUM(rHienVat) as rHienVat,SUM(rChiTapTrung) as rChiTapTrung
FROM
(
SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi
,rTuChi=SUM(rTuChi/{0})
,rHienVat=SUM(rHienVat/{0})
,rChiTapTrung=SUM(CASE WHEN iID_MaPhongBan='06' THEN rTuChi/{0}	ELSE rChiTapTrung/{0} END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND sLNS='1030100'  AND {3}
 AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec 
  {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi
 HAVING SUM(rTuChi/{0})<>0 OR SUM(rHienVat/{0})<>0 OR SUM(rChiTapTrung/{0})<>0
 UNION ALL
 SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi
,rTuChi=SUM(rTuChi/{0})
,rHienVat=SUM(rHienVat/{0})
,rChiTapTrung=SUM(CASE WHEN iID_MaPhongBanDich='06' THEN rTuChi/{0}	ELSE rChiTapTrung/{0} END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND sLNS='1030100'  AND {3}
 AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec
  {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi
HAVING SUM(rTuChi/{0})<>0 OR SUM(rHienVat/{0})<>0 OR SUM(rChiTapTrung/{0})<>0) a
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi
HAVING SUM(rTuChi/{0})<>0 OR SUM(rHienVat/{0})<>0
 ORDER BY sM,sTM,sTTM,sNG", DVT, DKPhongBan, DKDonVi, iID_MaPhongBanQuanLy);


            if (iCapTongHop == "TheoB")
            {
                SQL = String.Format(@"SELECT iID_MaPhongBan,MaLoai,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,SUM(rTuChi) as rTuChi,SUM(rHienVat) as rHienVat,SUM(rChiTapTrung) as rChiTapTrung
FROM
(
SELECT iID_MaPhongBan,MaLoai,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi/{0})
,rHienVat=SUM(rHienVat/{0})
,rChiTapTrung=SUM(CASE WHEN iID_MaPhongBan='06' THEN rTuChi/{0}	ELSE rChiTapTrung/{0} END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND sLNS='1030100'  AND {3}
 AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec 
  {1} {2}
 GROUP BY iID_MaPhongBan,MaLoai,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
 HAVING SUM(rTuChi/{0})<>0 OR SUM(rHienVat/{0})<>0 OR SUM(rChiTapTrung/{0})<>0
 UNION ALL
 SELECT iID_MaPhongBan,MaLoai,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi/{0})
,rHienVat=SUM(rHienVat/{0})
,rChiTapTrung=SUM(rChiTapTrung/{0})
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND sLNS='1030100'  AND {3}
 AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec
  {1} {2}
 GROUP BY  iID_MaPhongBan,MaLoai,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi/{0})<>0 OR SUM(rHienVat/{0})<>0 OR SUM(rChiTapTrung/{0})<>0) a
GROUP BY iID_MaPhongBan,MaLoai,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi
HAVING SUM(rTuChi/{0})<>0 OR SUM(rHienVat/{0})<>0 
 ORDER BY sM,sTM,sTTM,sNG", DVT, DKPhongBan, DKDonVi, iID_MaPhongBanQuanLy);
            }
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public static DataTable DT_rptDuToan_1020100_TungDonVi_GhiChu(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi/{3})
,rHienVat=SUM(rHienVat/{3})
,rChiTapTrung=SUM(CASE WHEN iID_MaPhongBan='06' THEN rTuChi/{3}	ELSE rChiTapTrung/{3} END)
,sGhiChu
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND sLNS='1030100'
 AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec 
  {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,sGhiChu
 HAVING SUM(rTuChi/{3})<>0 OR SUM(rHienVat/{3})<>0 OR SUM(rChiTapTrung/{3})<>0", iID_MaDonVi, DKPhongBan, DKDonVi, DVT);
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
            DataTable data = DT_rptDuToan_1020100_TungDonVi(MaND, iID_MaDonVi);
            DataTable dataGhiChu = DT_rptDuToan_1020100_TungDonVi_GhiChu(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
                       
            DataTable dtMoTaGhiChu = HamChung.SelectDistinct("dtMoTaGhiChu", dataGhiChu, "sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu", "");
            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
            {
                DataRow dr = dtMoTaGhiChu.Rows[i];
                var items = new List<string>();
                dr["sMoTa"] += "-";
                for (int j = 0; j < dataGhiChu.Rows.Count; j++)
                {
                    DataRow dr1 = dataGhiChu.Rows[j];


                    if (Convert.ToString(dr["sM"]) == Convert.ToString(dr1["sM"]) && Convert.ToString(dr["sTM"]) == Convert.ToString(dr1["sTM"]) && Convert.ToString(dr["sTTM"]) == Convert.ToString(dr1["sTTM"]) && Convert.ToString(dr["sNG"]) == Convert.ToString(dr1["sNG"]))
                    {
                        //dr["sMoTa"] += dr1["sGhiChu"] + " " + Convert.ToDecimal(dr1["rTuChi"]).ToString("###,###");

                        //dr["sMoTa"] += ", ";

                        items.Add(dr1["sGhiChu"] + ": " + Convert.ToDecimal(dr1["rTuChi"]).ToString("###,###"));

                        //  dr["sMoTa"] = dr["sMoTa"] + (j < dataGhiChu.Rows.Count - 1 ? ", " :"");

                    }
                }
                dr["sMoTa"] += items.Join(", ");

            }


            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("GhiChu", dataGhiChu);
            fr.AddTable("dtMoTaGhiChu", dtMoTaGhiChu);
            bool checkCoGhiChu = false;
            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
            {
                r = dtMoTaGhiChu.Rows[i];
                if (!String.IsNullOrEmpty(r["sGhiChu"].ToString()))
                {
                    checkCoGhiChu = true;
                }
            }
            if (checkCoGhiChu)
                fr.SetValue("sGhiChu", " * Ghi chú: ");
            else
                fr.SetValue("sGhiChu", "");
           

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
                clsResult.FileName = "DuToan_1020100_KT_TungDonVi.xls";
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

