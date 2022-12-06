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
using VIETTEL.Flexcel;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_ThuNop_TungDonViController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_ThuNop_TungDonVi.xls";

        [Authorize]
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_ThuNop_TungDonVi.aspx";
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
       [Authorize]
        public ActionResult EditSubmit(String ParentID)
        {
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_ThuNop_TungDonVi.aspx";
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
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_ThuNop_TungDonVi");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(MaND);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);
            fr.Run(Result);

            var count = Result.TotalPageCount();

            if (count > 1)
            {
                Result.AddPageFirstPage();
            }

            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_ThuNop_TungDonVi(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
          //  DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String SQL = String.Format(@"
SELECT sLNS1,sLNS3,sLNS5,sLNS,sMoTa,SUM(DoanhNghiep) as DoanhNghiep, SUM(DuToan) as DuToan,Cap
FROM (
SELECT SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
SUBSTRING(sLNS,1,5) as sLNS5, sLNS,sMoTa,Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8010101'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION 
SELECT
sLNS1='8',
sLNS3='801',
sLNS5='80101',
sLNS='8010199',
sMoTa=N'Thu cân đối khác',Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/1000 ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/1000 ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '80101%' AND sLNS NOT IN('8010101')  AND iNamLamViec=@iNamLamViec {0} {1}

UNION 
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
SUBSTRING(sLNS,1,5) as sLNS5, sLNS,sMoTa,Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8010202'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION 
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
SUBSTRING(sLNS,1,5) as sLNS5, sLNS,sMoTa,Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8010204'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa

UNION 
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
SUBSTRING(sLNS,1,5) as sLNS5, sLNS,sMoTa,Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8010203'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
SUBSTRING(sLNS,1,5) as sLNS5, sLNS,sMoTa,Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8010205'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION 
SELECT
sLNS1='8',
sLNS3='801',
sLNS5='80102',
sLNS='8010299',
sMoTa=N'Thu quản lý khác',Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/1000 ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/1000 ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '80102%' AND sLNS NOT IN('8010204','8010202','8010203','8010205')  AND iNamLamViec=@iNamLamViec {0} {1}
UNION
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
SUBSTRING(sLNS,1,5) as sLNS5, sLNS,sMoTa,Cap='2'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8020102'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa

UNION
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
sLNS5='80202', sLNS,sMoTa,Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8020101'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
sLNS5='80202', sLNS,sMoTa,Cap='3'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8020106'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
sLNS5='80203', sLNS,sMoTa,Cap='2'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8020107'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
sLNS5='80204', sLNS,sMoTa,Cap='2'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8020105'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION
SELECT
SUBSTRING(sLNS,1,1) as sLNS1,
SUBSTRING(sLNS,1,3) as sLNS3,
sLNS5='80205', sLNS,sMoTa,Cap='2'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/{2} ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/{2} ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND iID_MaDonVi=@iID_MaDonVi AND  sLNS LIKE '8%' AND sLNS='8020103'  AND iNamLamViec=@iNamLamViec {0} {1}
 GROUP BY sLNS,sMoTa
UNION 
SELECT
sLNS1='8',
sLNS3='802',
sLNS5='80206',
sLNS='8020199',
sMoTa=N'Thu nhà nước khác',Cap='2'
,DoanhNghiep=SUM(CASE WHEN iID_MaPhongBan=06 THEN rTuChi/1000 ELSE 0 END) 
,DuToan=SUM(CASE WHEN iID_MaPhongBan NOT IN (06) THEN rTuChi/1000 ELSE 0 END) 
FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND  sLNS LIKE '802%' AND iID_MaDonVi=@iID_MaDonVi AND sLNS NOT IN('8020101','8020102','8020103','8020105','8020106','8020107')  AND iNamLamViec=@iNamLamViec {0} {1}) as a
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sMoTa,Cap
HAVING SUM(DoanhNghiep)<>0 OR SUM(DuToan)<>0", DKPhongBan, DKDonVi, DVT);
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
            DataTable data = DT_rptDuToan_ThuNop_TungDonVi(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();

            DataTable dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", data, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5,sMoTa");
            dtsLNS5.Columns.Add("STT", typeof(String));
            for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            {
                r = dtsLNS5.Rows[i];                               
                switch (Convert.ToString(r["sLNS5"]))
                {
                    case "80201":
                        r["sMoTa"] = "Thuế thu nhập cá nhân";
                        break;
                    case "80202":
                        r["sMoTa"] = "Thuế thu nhập doanh nghiệp";
                        break;
                    case "80203":
                        r["sMoTa"] = "Khấu hao cơ bản";
                        break;
                    case "80204":
                        r["sMoTa"] = "Thu tiền sử dụng đất";
                        break;
                    case "80205":
                        r["sMoTa"] = "Thu thanh, xử lý tài sản";
                        break;
                    case "80206":
                        r["sMoTa"] = "Thu khác";
                        break;
                    default:
                        r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS5"]), MaND);
                        break;
                }
            }
            DataTable dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa");
            dtsLNS3.Columns.Add("STT", typeof(String));
            for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            {
                r = dtsLNS3.Rows[i];
                int temp = 1;
                r["STT"] = i + 1;
                r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS3"]), MaND);
                for (int j = 0; j < dtsLNS5.Rows.Count; j++)
                {
                    DataRow rt = dtsLNS5.Rows[j];
                    if (Convert.ToString(rt["sLNS3"]) == Convert.ToString(r["sLNS3"]))
                    {
                        rt["STT"] = temp;
                        temp += 1;
                    }
                }
            }
            fr.AddTable("dtsLNS3", dtsLNS3);
            fr.AddTable("dtsLNS5", dtsLNS5);
            dtsLNS3.Dispose();
            dtsLNS5.Dispose();

        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        [Authorize]
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi)
        {
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_ThuNop_TungDonVi.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi)
        {
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

