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
    public class rptDuToan_TongHopCuc_PBNN_TBController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_TongHopCuc_PBNN_TB.xls";
        private string _xLNS = "(1030100,1030900)";

        public ActionResult Index()
        {
            DataTable dtCauHinh = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            _xLNS = dtCauHinh.Rows[0]["xLNS"].ToString();
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_PBNN_TB.aspx";
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
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_PBNN_TB.aspx";
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


            String sTenDonVi = "B - "+iID_MaPhongBan;
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
                sTenDonVi = ReportModels.CauHinhTenDonViSuDung(2,MaND);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaPhongBan);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_TongHop_2");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("Cap2", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;

        }

        
        public static DataTable DT_rptDuToan_TongHop_2(String MaND,String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi,SUM(rTuChi) as rTuChi,SUM(rChiTapChung) as rChiTapChung
FROM(
SELECT iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND sLNS IN ('1010000','1020000','1020100')THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')  AND iNamLamViec=@iNamLamViec  AND MaLoai<>1 {0} {1} -- AND iID_MaDonVi NOT IN ('03','75')
 GROUP BY iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1010000','1020000','1020100')THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai in (1,2)  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')   AND iNamLamViec=@iNamLamViec {0} {1} --AND iID_MaDonVi NOT IN ('03','75') 
 GROUP BY iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN  sLNS NOT  IN ('0') THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')  AND iNamLamViec=@iNamLamViec  AND MaLoai<>1 {0} {1} --AND iID_MaDonVi  IN ('03','75')
 GROUP BY iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN sLNS  NOT IN ('0') THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai in (1,2)  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')   AND iNamLamViec=@iNamLamViec {0} {1} -- AND iID_MaDonVi  IN ('03','75')
 GROUP BY iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0
) as a
GROUP BY iID_MaDonVi,sTenDonVi
ORDER BY iID_MaDonVi
", DKPhongBan, DKDonVi, DVT);
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
            DataTable data = DT_rptDuToan_TongHop_2(MaND, iID_MaPhongBan);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
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

