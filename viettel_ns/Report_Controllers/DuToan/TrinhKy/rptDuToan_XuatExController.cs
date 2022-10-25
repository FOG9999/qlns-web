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
    public class rptDuToan_XuatExController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_XuatEx.xls";
        private const String sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/rptDuToan_XuatEx_TrinhKy.xls";
        public string sTrinhKy = "";
        [Authorize]
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_XuatEx.aspx";
                ViewData["PageLoad"] = "0";
                sTrinhKy = Request.QueryString["sTrinhKy"];
                ViewData["sTrinhKy"] = sTrinhKy;
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
            sTrinhKy = Request.Form[ParentID + "_sTrinhKy"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["sTrinhKy"] = sTrinhKy;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_XuatEx.aspx";
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
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_XuatEx", MaND);
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

            var loaiKhoan = HamChung.GetLoaiKhoanText("1010000");
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
        public DataTable DT_rptDuToan_XuatEx(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMota,
SUM(rtuchi/{2}) as rTuChi,
SUM(rhienvat/{2}) as rHienVat
  FROM DT_ChungTuChiTiet
  WHERE itrangThai=1
  AND iNamLamViec=@iNamLamViec AND iid_madonvi=@iID_MaDonVi
  AND MaLoai<>1 {0} {1}
  GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMota
  HAVING SUM(rtuchi)<>0 OR  SUM(rHienVat)<>0 
  UNION 
  
  SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMota,
SUM(rtuchi/{2}) as rTuChi,
SUM(rhienvat/{2}) as rHienVat
  FROM DT_ChungTuChiTiet_phancap
  WHERE itrangThai=1 {0} {1}
  AND iNamLamViec=@iNamLamViec AND iid_madonvi=@iID_MaDonVi
  AND MaLoai<>1
  GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMota
  HAVING SUM(rtuchi)<>0 OR  SUM(rHienVat)<>0 
    UNION
  
  SELECT sLNS=1040200,sL,sK,sM,sTM,sTTM,sNG,sMota,
SUM(rHangNhap/{2}) as rTuChi,
rHienVat=0
  FROM DT_ChungTuChiTiet
  WHERE itrangThai=1 {0} {1}
  AND iNamLamViec=@iNamLamViec AND iid_madonvi=@iID_MaDonVi
  AND MaLoai<>1
  GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMota
HAVING SUM(rHangNhap)<>0

    UNION
  
  SELECT sLNS=1040300,sL,sK,sM,sTM,sTTM,sNG,sMota,
SUM(rHangMua/{2}) as rTuChi,
rHienVat=0
  FROM DT_ChungTuChiTiet
  WHERE itrangThai=1 {0} {1}
  AND iNamLamViec=@iNamLamViec AND iid_madonvi=@iID_MaDonVi
  AND MaLoai<>1
  GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMota
HAVING SUM(rHangMua)<>0

  ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG ", DKPhongBan, DKDonVi, DVT);
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
            DataTable data = DT_rptDuToan_XuatEx(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK", "sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "sLNS,sMoTa", "sLNS,sL");



            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);



            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();

        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi)
        {
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "THSL_"+iID_MaDonVi+"_"+DateTime.Now.ToString("dd/mm/yyyy")+".xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi, String sTrinhKy)
        {
            HamChung.Language();
            String DuongDan = "";
            if (String.IsNullOrEmpty(sTrinhKy))
            {
                DuongDan = sFilePath;
            }
            else
                DuongDan = sFilePath_TrinhKy;
            ExcelFile xls = CreateReport(Server.MapPath(DuongDan), MaND, iID_MaDonVi);
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

