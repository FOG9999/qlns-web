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
using System.Data.OleDb;
using System.Text;
using System.Security.Cryptography;

namespace VIETTEL.Report_Controllers.QuyetToan
{
    public class rptQuyetToan_KiemToan101102Controller : Controller
    {

        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/KiemToan/rptQuyetToan_KiemToan101102.xls";
        public string sTrinhKy = "";
        String sLNS = "", iNamLamViec = "", iID_MaNamNganSach = "";
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {

                ViewData["path"] = "~/Report_Views/QuyetToan/KiemToan/rptQuyetToan_KiemToan101102.aspx";
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
            ViewData["iID_MaNamNganSach"] = Request.Form[ParentID+"_iID_MaNamNganSach"];
            ViewData["iNamLamViec"] = Request.Form[ParentID+"_iNamLamViec"];
            ViewData["path"] = "~/Report_Views/QuyetToan/KiemToan/rptQuyetToan_KiemToan101102.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path)
        {

            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            fr.SetValue("Nam", iNamLamViec);
            string NamNganSach = "";
            if (iID_MaNamNganSach == "1")
                NamNganSach = "NGÂN SÁCH NĂM TRƯỚC CHUYỂN SANG";
            else if (iID_MaNamNganSach == "2")
                NamNganSach = "NGÂN SÁCH NĂM NAY";
            LoadData(fr);
            fr.SetValue("NamNganSach", NamNganSach);
            fr = ReportModels.LayThongTinChuKy(fr, "rptQuyetToan_KiemToan101102");
            fr.Run(Result);
            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public DataTable DT_rptQuyetToan_KiemToan101102()
        {
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            if (!"0".Equals(iID_MaNamNganSach))
            {
                DK = " AND nam=@iID_MaNamNganSach";
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            }
            String SQL = String.Format(@" SELECT Lns as sLNS,l as sL,k as sK,m as sM,QT.Dvi as Dvi,sTen,sMota='',
  SUM(Tuchi) as rTuChi
   FROM QT,NS_DonVi
  WHERE Lns=1020100 AND namlv=@iNamLamViec {0}  AND m IN (6900,9050) AND NS_DonVi.iID_MaDonVi=QT.Dvi AND NS_DonVi.iTrangThai=1 AND iNamLamViec_DonVi=2018
  GROUP BY Lns,l,k,m,Dvi,sTen
HAVING SUM(TuChi)<>0
  ORDER BY Lns,l,k,m,Dvi", DK);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.Dispose();
            DataTable dt = Connection.GetDataTable(cmd);
            return dt;
        }
        public static String LayMoTa(String sLNS)
        {
            String sMoTa = "";

            String SQL = String.Format(@"SELECT sMoTa FROM NS_MucLucNganSach WHERE iTrangThai=1 AND iNamLamViec=2017  AND sLNS={0}", sLNS);
            sMoTa = Connection.GetValueString(SQL, "");
            return sMoTa;
        }
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr)
        {
            DataRow r;
            DataTable data = DT_rptQuyetToan_KiemToan101102();
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            DataTable dtMuc = HamChung.SelectDistinct("dtMuc", data, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTTM");
            fr.AddTable("dtMuc", dtMuc);
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String iNamLamViec, String iID_MaNamNganSach)
        {
            HamChung.Language();
            this.iNamLamViec = iNamLamViec;
            this.iID_MaNamNganSach = iID_MaNamNganSach;
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath));

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "kiemtoanNV.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iNamLamViec, String iID_MaNamNganSach)
        {
            HamChung.Language();
            this.iNamLamViec = iNamLamViec;
            this.iID_MaNamNganSach = iID_MaNamNganSach;

            ExcelFile xls = CreateReport(Server.MapPath(sFilePath));
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

