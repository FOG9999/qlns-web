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
using Viettel.Services;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_2_TongHopController : AppController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private  String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_2_TongHop.xls";
        private  string sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/rptDuToan_2_TongHop_TrinhKy.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_2_TongHop_Muc.xls";
        String iCapTongHop = "";
        public ActionResult Index(int trinhky = 0)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_2_TongHop.aspx";
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
        public ActionResult EditSubmit(string ParentID, int trinhky = 0)
        {
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["PageLoad"] = "1";
            ViewData["trinhky"] = trinhky;
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_2_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string iID_MaPhongBan, string iCapTongHop, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaPhongBan, iCapTongHop, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaPhongBan, string iCapTongHop, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaPhongBan, iCapTongHop, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        #region private methods


        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        //public ExcelFile CreateReport(String path, String MaND, String iID_MaPhongBan)
        //{
        //    DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
        //    String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);


        //    String sTenDonVi = "B -  " + iID_MaPhongBan;
        //    if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
        //        sTenDonVi = ReportModels.CauHinhTenDonViSuDung(2,MaND);
        //    XlsFile Result = new XlsFile(true);
        //    Result.Open(path);
        //    FlexCelReport fr = new FlexCelReport();
        //    LoadData(fr, MaND, iID_MaPhongBan);
        //    fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_2_TongHop");
        //    fr.SetValue("Nam", iNamLamViec);
        //    fr.SetValue("Cap2", sTenDonVi);
        //    fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
        //    fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
        //    fr.Run(Result);
        //    return Result;

        //}


        public ExcelFile CreateReport(string iID_MaPhongBan, string iCapTongHop, int trinhky = 0)
        {
            var file = sFilePath;
            if (iCapTongHop == "Muc")
            {
                file = sFilePath_Muc;
            }
            else
            {
                file = iID_MaPhongBan != null && iID_MaPhongBan != "-1" && trinhky == 1 ?
                    sFilePath_TrinhKy :
                    sFilePath;
            }
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaPhongBan);

            fr.UseCommonValue()
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .Run(xls);
            var count = xls.TotalPageCount();

            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;

        }

        private DataTable DT_rptDuToan_2_TongHop(String MaND, String iID_MaPhongBan)
        {
            var v = Request.GetQueryStringValue("v");
            return v == 2 ?
                DT_rptDuToan_2_TongHop_v2(MaND, iID_MaPhongBan) :
                DT_rptDuToan_2_TongHop_v1(MaND, iID_MaPhongBan);
        }
        private DataTable DT_rptDuToan_2_TongHop_v2(String MaND, String iID_MaPhongBan)
        {
            var _duToanService = new DuToanReportService();
            var dt = _duToanService.TongHop(MaND, iID_MaPhongBan, "2");
            return dt;
        }

        //1020200
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        private DataTable DT_rptDuToan_2_TongHop_v1(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT SUBSTRING(sLNS,1,3) as sLNS3,SUBSTRING(sLNS,1,5) as sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
,rTuChi=SUM(rTuChi/{3})
,rChiTapTrung=SUM(rTuChi/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0  AND sLNS LIKE '2%' AND iNamLamViec=@iNamLamViec {0} {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa", "", DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
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
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_2_TongHop(MaND, iID_MaPhongBan);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS3,sLNS5,sLNS,sL,sK", "sLNS3,sLNS5,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS3,sLNS5,sLNS", "sLNS3,sLNS5,sLNS,sMoTa", "sLNS,sL");
            DataTable dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", dtsLNS, "sLNS3,sLNS5", "sLNS3,sLNS5,sMoTa", "sLNS,sL");
            DataTable dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS3", "sLNS3,sMoTa", "");

            for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            {
                r = dtsLNS3.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS3"]));
            }
            for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            {
                r = dtsLNS5.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS5"]));
            }

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS5", dtsLNS5);
            fr.AddTable("dtsLNS3", dtsLNS3);

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtsLNS3.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        //public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan, String iCapTongHop)
        //{
        //    this.iCapTongHop = iCapTongHop;
        //    if (iCapTongHop == "Muc")
        //    {
        //        sFilePath = sFilePath_Muc;
        //    }
        //    clsExcelResult clsResult = new clsExcelResult();
        //    ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan);

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        xls.Save(ms);
        //        ms.Position = 0;
        //        clsResult.ms = ms;
        //        clsResult.FileName = "DuToan_1020200_TongHop.xls";
        //        clsResult.type = "xls";
        //        return clsResult;
        //    }

        //}

        #endregion
    }
}

