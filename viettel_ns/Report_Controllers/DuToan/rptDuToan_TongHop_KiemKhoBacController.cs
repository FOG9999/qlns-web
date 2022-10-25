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
    public class rptDuToan_TongHop_KiemKhoBacController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_TongHop_KiemKhoBac.xls";
        private const String sFilePath_TongHop = "/Report_ExcelFrom/DuToan/rptDuToan_TongHop_KiemKhoBac_TongHop.xls";
        public String iID_MaPhongBan = "", iID_MaNguon = "", iID_Muc = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_KiemKhoBac.aspx";
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
            iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            iID_MaNguon = Request.Form[ParentID + "_iID_MaNguon"];
            iID_Muc = Request.Form[ParentID + "_iID_Muc"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["iID_MaNguon"] = iID_MaNguon;
            ViewData["iID_Muc"] = iID_Muc;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_KiemKhoBac.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);


            String sTenDonVi = "B -  " + iID_MaPhongBan;
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
                sTenDonVi = ReportModels.CauHinhTenDonViSuDung(2, MaND);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_TongHop_KiemKhoBac");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;

        }


        public DataTable DT_rptDuToan_TongHop_KiemKhoBac(String MaND)
        {
            String DKDonVi = "", DKPhongBan = "",DKLNS="";;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            // nếu NSQP
            if (iID_MaNguon.Equals("1"))
            {

                String SQL = String.Format(@"SELECT iID_MaPhongBan,sLNS,iID_MaDonVi,sTenDonVi,SUM(rTuChi) as rTuChi,SUM(rChiTapChung) as rChiTapChung
FROM(
SELECT iID_MaPhongBan,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND sLNS IN ('1010000','1020000','1020100')THEN (rTuChi-rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS LIKE '1%'  AND iNamLamViec=@iNamLamViec  AND MaLoai<>1 {0} {1} 
AND iID_MaDonVi NOT IN('03','75')
 GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaPhongBanDich,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1010000','1020000','1020100')THEN (rTuChi-rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND sLNS LIKE '1%'  AND iNamLamViec=@iNamLamViec {0} {1}
AND iID_MaDonVi NOT IN('03','75')
 GROUP BY iID_MaPhongBanDich,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0


-- xu ly rieng 03 
UNION

SELECT iID_MaPhongBan,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND (sLNS IN ('1010000','1020000','1020100') OR sLNS like '109%')THEN (rTuChi-rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS LIKE '1%'  AND iNamLamViec=@iNamLamViec  AND MaLoai<>1 {0} {1} 
AND iID_MaDonVi  IN('03')
 GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaPhongBanDich,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND (sLNS IN ('1010000','1020000','1020100') OR sLNS like '109%') THEN (rTuChi-rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND sLNS LIKE '1%'  AND iNamLamViec=@iNamLamViec {0} {1}
AND iID_MaDonVi  IN('03')
 GROUP BY iID_MaPhongBanDich,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

-- xu ly rieng 75
UNION

SELECT iID_MaPhongBan,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND sLNS IN ('1010000','1020000','1020100','1091500')THEN (rTuChi-rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS LIKE '1%'  AND iNamLamViec=@iNamLamViec  AND MaLoai<>1 {0} {1} 
AND iID_MaDonVi IN ('75')
 GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaPhongBanDich,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1010000','1020000','1020100','1091500')THEN (rTuChi-rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND sLNS LIKE '1%'  AND iNamLamViec=@iNamLamViec {0} {1}
AND iID_MaDonVi  IN ('75')
 GROUP BY iID_MaPhongBanDich,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

) as a
GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLNS
ORDER BY iID_MaDonVi,sLNS,iID_MaPhongBan
", DKPhongBan, DKDonVi, DVT);
                cmd.CommandText = SQL;
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                DataTable dt = Connection.GetDataTable(cmd);
                cmd.Dispose();
                return dt;
            }
            else //NSNN
            {
                String SQL = String.Format(@"SELECT iID_MaPhongBan,sLNS,iID_MaDonVi,sTenDonVi,SUM(rTuChi) as rTuChi,SUM(rChiTapChung) as rChiTapChung
FROM(
SELECT iID_MaPhongBan,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND sLNS IN ('1010000','1020000','1020100')THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')  AND iNamLamViec=@iNamLamViec  AND MaLoai<>1 {0} {1} 
 GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaPhongBanDich,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1010000','1020000','1020100')THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')   AND iNamLamViec=@iNamLamViec {0} {1} 
 GROUP BY iID_MaPhongBanDich,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaPhongBan,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN  sLNS NOT  IN ('0') THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')  AND iNamLamViec=@iNamLamViec  AND MaLoai<>1 {0} {1} --AND iID_MaDonVi  IN ('03','75')
 GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0

UNION

SELECT iID_MaPhongBanDich,sLNS,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi
,rTuChi=SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2})
,rChiTapChung=SUM(CASE WHEN sLNS  NOT IN ('0') THEN (rChiTapTrung)/{2} ELSE 0 END)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND (sLNS LIKE '2%' OR sLNS LIKE '3%')   AND iNamLamViec=@iNamLamViec {0} {1}  --AND iID_MaDonVi  IN ('03','75')
 GROUP BY iID_MaPhongBanDich,iID_MaDonVi,sTenDonVi,sLNS
HAVING SUM(rTuChi/{2})+SUM(rDuPhong/{2})+SUM(rHangMua/{2})+SUM(rHangNhap/{2}) <>0 OR SUM(rChiTapTrung/{2})<>0
) as a
GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLNS
ORDER BY iID_MaDonVi,sLNS,iID_MaPhongBan
", DKPhongBan, DKDonVi, DVT);
                cmd.CommandText = SQL;
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                DataTable dt = Connection.GetDataTable(cmd);
                cmd.Dispose();
                return dt;
            }
        }

        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_TongHop_KiemKhoBac(MaND);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);


            DataTable dtLNS = HamChung.SelectDistinct("LNS", data, "sLNS,iID_MaDonVi", "sLNS,iID_MaDonVi,sTenDonVi");
            fr.AddTable("LNS", dtLNS);
            DataTable dtDonVi = HamChung.SelectDistinct("DonVi", dtLNS, "iID_MaDonVi", "iID_MaDonVi,sTenDonVi");
            dtDonVi.Columns.Add("STT", typeof(String));
            for (int i = 0; i < dtDonVi.Rows.Count; i++)
            {
                dtDonVi.Rows[i]["STT"] = i + 1;
            }
            fr.AddTable("DonVi", dtDonVi);
            dtDonVi.Dispose();
            data.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan, String iID_MaNguon, String iID_Muc)
        {
            clsExcelResult clsResult = new clsExcelResult();
            this.iID_MaPhongBan = iID_MaPhongBan;
            this.iID_MaNguon = iID_MaNguon;
            this.iID_Muc = iID_Muc;
            String DuongDan = "";
            if (iID_Muc.Equals("1"))
            {
                DuongDan = sFilePath;
            }
            else
                DuongDan = sFilePath_TongHop;
            ExcelFile xls = CreateReport(Server.MapPath(DuongDan), MaND);

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
        public ActionResult ViewPDF(String MaND,String iID_MaPhongBan,String iID_MaNguon,String iID_Muc)
        {
            HamChung.Language();
            this.iID_MaPhongBan = iID_MaPhongBan;
            this.iID_MaNguon = iID_MaNguon;
            this.iID_Muc = iID_Muc;
            String DuongDan = "";
            if (iID_Muc.Equals("1"))
            {
                DuongDan = sFilePath;
            }
            else
                DuongDan = sFilePath_TongHop;
            ExcelFile xls = CreateReport(Server.MapPath(DuongDan), MaND);
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

