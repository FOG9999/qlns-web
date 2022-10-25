using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1020100_KT_TungDonViController : AppController
    {
        public string sViewPath = "~/Report_Views/DuToan/";
        private const string sFilePath = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_1020100_KT_TungDonVi.xls";
        private const string sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/TrinhKy/rptDuToan_1020100_KT_TungDonVi_TrinhKy.xls";

        public ActionResult Index(int trinhky = 0)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1020100_KT_TungDonVi.aspx";
                ViewData["PageLoad"] = "0";
                ViewData["trinhky"] = trinhky;

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
        public ActionResult EditSubmit(String ParentID, string iID_MaDonVi, int trinhky = 0)
        {
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["trinhky"] = trinhky;
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_1020100_KT_TungDonVi.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        //public ExcelFile CreateReport(string path, string iID_MaDonVi, string loai)
        //{
        //    var username = User.Identity.Name;
        //    DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(username);
        //    String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

        //    String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi,username);
        //    XlsFile Result = new XlsFile(true);
        //    Result.Open(path);
        //    FlexCelReport fr = new FlexCelReport();
        //    LoadData(fr, username, iID_MaDonVi);
        //    fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1020100_KT_TungDonVi");
        //    fr.SetValue("Nam", iNamLamViec);
        //    fr.SetValue("sTenDonVi", sTenDonVi);
        //    fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,username));
        //    fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

        //    fr.SetValue("Thang", ReportModels.Thang_Nam_HienTai());
        //    fr.SetValue("Ngay2", ReportModels.Ngay_Thang_Nam_HienTai().ToLower());
        //    fr.Run(Result);
        //    return Result;
        //}

        private ExcelFile CreateReport(string iID_MaDonVi, int trinhky = 0)
        {
            var file = trinhky == 1 ?
               sFilePath_TrinhKy :
               sFilePath;
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(file));

            var fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaDonVi);

            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, Username);
            fr.SetValue("sTenDonVi", sTenDonVi);

            fr.UseCommonValue()
                .UseChuKy()
                .UseChuKyForController(this.ControllerName())
                .Run(Result);
            return Result;

        }

        private DataTable DT_rptDuToan_1020100_KT_TungDonVi(string maND, string iID_MaDonVi)
        {
            var v = Request.GetQueryStringValue("v");
            return v == 2 ?
                DT_rptDuToan_1020100_KT_TungDonVi2(maND, iID_MaDonVi) :
                DT_rptDuToan_1020100_KT_TungDonVi1(maND, iID_MaDonVi);
        }

        private DataTable DT_rptDuToan_1020100_KT_TungDonVi2(string maND, string iID_MaDonVi)
        {
            var _duToanService = new DuToanReportService();
            var dt = _duToanService.TungDonVi_1020100_KT(maND, iID_MaDonVi);
            return dt;
        }


        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        private DataTable DT_rptDuToan_1020100_KT_TungDonVi1(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi/{3})
,rHienVat=SUM(rHienVat/{3})
,rTonKho=SUM(rTonKho/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS='1020100' AND sM IN (6900,6950,7000,7750,9050) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi", iID_MaDonVi, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaDonVi)
        {
            DataTable data = DT_rptDuToan_1020100_KT_TungDonVi(MaND, iID_MaDonVi);
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
        //public clsExcelResult ExportToExcel(string iID_MaDonVi, string loai)
        //{
        //    clsExcelResult clsResult = new clsExcelResult();
        //    ExcelFile xls = CreateReport(Server.MapPath(sFilePath), iID_MaDonVi,loai);

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        xls.Save(ms);
        //        ms.Position = 0;
        //        clsResult.ms = ms;
        //        clsResult.FileName = "DuToan_1020100_KT_TungDonVi.xls";
        //        clsResult.type = "xls";
        //        return clsResult;
        //    }

        //}
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string iID_MaDonVi, int trinhky)
        {
            HamChung.Language();
            var xls = CreateReport(iID_MaDonVi, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaDonVi, int trinhky)
        {
            HamChung.Language();
            var xls = CreateReport(iID_MaDonVi, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}
