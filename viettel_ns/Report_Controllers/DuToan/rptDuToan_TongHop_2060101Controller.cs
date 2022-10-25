﻿using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_TongHop_2060101Controller : AppController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_TongHop_2060101.xls";
        private const String sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/rptDuToan_TongHop_2060101_TrinhKy.xls";
        String sTrinhKy = "";

        [Authorize]
        public ActionResult Index(int trinhky = 0)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_2060101.aspx";
                ViewData["PageLoad"] = "0";
                sTrinhKy = Request.QueryString["sTrinhKy"];
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
        /// 
        [Authorize]
        public ActionResult EditSubmit(string ParentID, int trinhky = 0)
        {
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            sTrinhKy = Request.Form[ParentID + "_sTrinhKy"];
            ViewData["trinhky"] = trinhky;
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_TongHop_2060101.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
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


        //    String sTenDonVi = "B -  "+iID_MaPhongBan;
        //    if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
        //        sTenDonVi = ReportModels.CauHinhTenDonViSuDung(2,MaND);
        //    XlsFile Result = new XlsFile(true);
        //    Result.Open(path);
        //    FlexCelReport fr = new FlexCelReport();
        //    LoadData(fr, MaND, iID_MaPhongBan);
        //    fr = ReportModels.LayThongTinChuKy(fr,"rptDuToan_TongHop_2060101");
        //    fr.SetValue("Nam", iNamLamViec);
        //    fr.SetValue("Cap2", sTenDonVi);
        //    fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
        //    fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
        //    fr.SetValue("Thang", ReportModels.Thang_Nam_HienTai());
        //    fr.Run(Result);
        //    return Result;

        //}

        public ExcelFile CreateReport(string iID_MaPhongBan, int trinhky = 0)
        {
            var file = !string.IsNullOrWhiteSpace(iID_MaPhongBan) && iID_MaPhongBan != "-1" && trinhky == 1 ?
                    sFilePath_TrinhKy :
                    sFilePath;
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            var fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaPhongBan);

            fr.UseCommonValue()
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .Run(xls);
            return xls;
        }

        public static DataTable DT_rptDuToan_TongHop_2060101(String MaND, String iID_MaPhongBan)
        {
            if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
                iID_MaPhongBan = "-1";

            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT iID_MaDonVi,sTenDonVi
,TroCap_Tien=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(00,01) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,TroCap_Nguoi=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(00,01) AND sNG=38) THEN rSoNguoi ELSE 0 END)
,PhuCap_Tien=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(02) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,PhuCap_Nguoi=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(02) AND sNG=38) THEN rSoNguoi ELSE 0 END)
,TienKhoiNghia_Tien=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(03) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,TienKhoiNghia_Nguoi=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(03) AND sNG=38) THEN rSoNguoi ELSE 0 END)
,AnhHung_Tien=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(04) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,AnhHung_Nguoi=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(04) AND sNG=38) THEN rSoNguoi ELSE 0 END)
,ThuongBinhA_Tien=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(05) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,ThuongBinhA_Nguoi=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(05) AND sNG=38) THEN rSoNguoi ELSE 0 END)
,ThuongBinhB_Tien=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(06) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,ThuongBinhB_Nguoi=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(06) AND sNG=38) THEN rSoNguoi ELSE 0 END)
,BaoTu_Tien=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7199 AND sTTM IN(70) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,BaoTu_Nguoi=SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7199 AND sTTM IN(70) AND sNG=38) THEN rSoNguoi ELSE 0 END)
,LePhi=SUM(CASE WHEN sLNS=2060102 THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
,DieuDuong=SUM(CASE WHEN (sM=7150 AND sTM=7166) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)   {0} AND iNamLamViec=@iNamLamViec {1} {2} AND (rTuChi+rDuPhong)<>0
 GROUP BY iID_MaDonVi,sTenDonVi
 HAVING SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(00,01) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END) <>0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(00,01) AND sNG=38) THEN rSoNguoi ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(02) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(02) AND sNG=38) THEN rSoNguoi ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(03) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(03) AND sNG=38) THEN rSoNguoi ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(04) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(04) AND sNG=38) THEN rSoNguoi ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(05) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(05) AND sNG=38) THEN rSoNguoi ELSE 0 END) <> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(06) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END)<> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7151 AND sTTM IN(06) AND sNG=38) THEN rSoNguoi ELSE 0 END) <> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7199 AND sTTM IN(70) AND sNG=38) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END) <> 0 OR
SUM(CASE WHEN (sLNS=2060101 AND sM=7150 AND sTM=7199 AND sTTM IN(70) AND sNG=38) THEN rSoNguoi ELSE 0 END) <> 0 OR
SUM(CASE WHEN sLNS=2060102 THEN (rTuChi+rDuPhong)/{3} ELSE 0 END) <> 0 OR
 SUM(CASE WHEN (sM=7150 AND sTM=7166) THEN (rTuChi+rDuPhong)/{3} ELSE 0 END) <>0
", "", DKPhongBan, DKDonVi, DVT);
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
            var data = DT_rptDuToan_TongHop_2060101(MaND, iID_MaPhongBan);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        //  [Authorize]
        //public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan)
        //{
        //    clsExcelResult clsResult = new clsExcelResult();
        //    String DuongDan = "";
        //    if (String.IsNullOrEmpty(sTrinhKy))
        //    {
        //        DuongDan = sFilePath;
        //    }
        //    else
        //        DuongDan = sFilePath_TrinhKy;
        //    ExcelFile xls = CreateReport(Server.MapPath(DuongDan), MaND, iID_MaPhongBan);

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        xls.Save(ms);
        //        ms.Position = 0;
        //        clsResult.ms = ms;
        //        clsResult.FileName = "DuToan_TongHop_NganSachQuocPhong.xls";
        //        clsResult.type = "xls";
        //        return clsResult;
        //    }

        //}
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult ViewPDF(string iID_MaPhongBan, int trinhky = 0)
        {
            HamChung.Language();

            var xls = CreateReport(iID_MaPhongBan, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaPhongBan, int trinhky = 0)
        {
            HamChung.Language();

            var xls = CreateReport(iID_MaPhongBan, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }


        #region longsam

        /// <summary>
        /// BHYT
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <returns></returns>
        private DataTable rptDuToan_TongHop_2060101(string id_phongban = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHop_2060101.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@id_donvi", PhienLamViec.iID_MaDonVi);
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", 1000);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion

        }

        #endregion


    }
}
