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
using VIETTEL.Helpers;
using Viettel.Data;
using Viettel.Services;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_TongHopCuc_PBQP_TBController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_TongHopCuc_PBQP_TB.xls";
        private string _xLNS = "-1",_xDonVi="-1";
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        public ActionResult Index()
        {            
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_PBQP_TB.aspx";
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
            String loaiBaoCao = Request.Form[ParentID + "_loaiBaoCao"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["loaiBaoCao"] = loaiBaoCao;
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_TongHopCuc_PBQP_TB.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND, String iID_MaPhongBan, string loaiBaoCao = "0")
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

            String sTenDonVi = PhongBanModels.Get_TenPhongBan(iID_MaPhongBan);
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
                sTenDonVi = "BỘ QUỐC PHÒNG";
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaPhongBan, loaiBaoCao);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_TongHop_1");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("Cap2", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;

        }


        public DataTable DT_rptDuToan_TongHop_1(String MaND, String iID_MaPhongBan, string loaiBaoCao = "0")
        {
            SqlCommand cmd = new SqlCommand();
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHop_PBQP.sql");
            sql = sql.Replace("@@xsLNS", _xLNS);
            sql = sql.Replace("@@xDonVi", _xDonVi);
            string loai = "";
            if (loaiBaoCao != "0")
            {
                loai = "1";
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam = ReportModels.LayNamLamViec(MaND),
                        dvt = 1000,
                        loai = loai.ToParamString(),
                    });
                dt.NullToZero("rTuChi,rChiTapTrung".Split(','));

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    double count = 0;
                    var dtr = dt.Rows[i];

                    for (int j = 2; j < dt.Columns.Count; j++)
                    {
                        var colName = dt.Columns[j].ToString();
                        count += Convert.ToDouble(dtr[colName]);
                    }
                    if (count == 0)
                    {
                        dt.Rows.RemoveAt(i);
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan, string loaiBaoCao = "0")
        {
            DataTable data = DT_rptDuToan_TongHop_1(MaND, iID_MaPhongBan, loaiBaoCao);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan, string loaiBaoCao = "0")
        {
            DataTable dtCauHinh = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            _xLNS = dtCauHinh.Rows[0]["xLNS"].ToString();
            _xDonVi = dtCauHinh.Rows[0]["xDonVi"].ToString();
            clsExcelResult clsResult = new clsExcelResult();
            string path = sFilePath;            
            ExcelFile xls = CreateReport(Server.MapPath(path), MaND, iID_MaPhongBan, loaiBaoCao);

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
        public ActionResult ViewPDF(String MaND, String iID_MaPhongBan, string loaiBaoCao = "0")
        {
            DataTable dtCauHinh = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            _xLNS = dtCauHinh.Rows[0]["xLNS"].ToString();
            _xDonVi = dtCauHinh.Rows[0]["xDonVi"].ToString();
            string path = sFilePath;
            
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(path), MaND, iID_MaPhongBan, loaiBaoCao);
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

