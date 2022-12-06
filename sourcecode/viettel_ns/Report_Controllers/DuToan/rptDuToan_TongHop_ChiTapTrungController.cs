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
    public class rptDuToan_TongHop_ChiTapTrungController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_TongHop_ChiTapTrung.xls";
        public String iID_MaPhongBan = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_ChiTapTrung.aspx";
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
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_ChiTapTrung.aspx";
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
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_TongHop_ChiTapTrung");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;

        }


        public DataTable DT_rptDuToan_TongHop_ChiTapTrung(String MaND)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
                String SQL="";
                SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,SUM(rTuChi) as rTuChi,SUM(rChiTapTrung) as rChiTapTrung
FROM
(SELECT iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,SUM(rTuChi/{2}) as rTuChi,SUM(rChiTapTrung/{2}) as rChiTapTrung
 FROM DT_ChungTuChiTiet
  WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} {1}
GROUP BY iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING SUM(rChiTapTrung)<>0

UNION
SELECT iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,SUM(rTuChi/1000) as rTuChi,SUM(rChiTapTrung/1000) as rChiTapTrung
 FROM DT_ChungTuChiTiet_PhanCap
  WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} {1}
GROUP BY iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING SUM(rTuChi)<>0) as a
GROUP BY iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING SUM(rChiTapTrung)<>0
ORDER BY iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM", DKDonVi, DKPhongBan, DVT);
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
        private void LoadData(FlexCelReport fr, String MaND)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_TongHop_ChiTapTrung(MaND);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtDonVi = HamChung.SelectDistinct("DonVi", data, "iID_MaDonVi", "iID_MaDonVi,sTenDonVi");
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
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan)
        {
            clsExcelResult clsResult = new clsExcelResult();
            this.iID_MaPhongBan = iID_MaPhongBan;
            String DuongDan = "";
           
                DuongDan = sFilePath;
           
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
        public ActionResult ViewPDF(String MaND,String iID_MaPhongBan)
        {
            HamChung.Language();
            this.iID_MaPhongBan = iID_MaPhongBan;
           
            String DuongDan = "";
            DuongDan = sFilePath;
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

