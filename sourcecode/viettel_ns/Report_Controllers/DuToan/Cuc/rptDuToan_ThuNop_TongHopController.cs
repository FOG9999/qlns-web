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
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_ThuNop_TongHopController : AppController  
    {
        private readonly INganSachService _nganSachService;

        public rptDuToan_ThuNop_TongHopController()
        {
            _nganSachService = new NganSachService();
        }


        public string sViewPath = "~/Report_Views/DuToan/";
        private  String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_ThuNop_TongHop.xls";
        private  String sFilePath_NN = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_ThuNop_TongHop_NN.xls";
        private  String sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/rptDuToan_ThuNop_TongHop_TrinhKy.xls";
        private  String sFilePath_TrinhKy_NN = "/Report_ExcelFrom/DuToan/rptDuToan_ThuNop_TongHop_TrinhKy_NN.xls";
       
        [Authorize]
        public ActionResult Index(int trinhky = 0)
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_ThuNop_TongHop.aspx";
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
       [Authorize]
        public ActionResult EditSubmit(String ParentID, int trinhky = 0)
        {
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String MaLoai = Request.Form[ParentID + "_MaLoai"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["trinhky"] = trinhky;
            ViewData["MaLoai"] = MaLoai;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_ThuNop_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        [Authorize]
        public ExcelFile CreateReport(String path, String MaND, String iID_MaPhongBan)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            var iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);
          
            XlsFile xls = new XlsFile(true);
            xls.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, iID_MaPhongBan);

            var sTenDonVi = "";
            var sTenPhongBan = "";
            if (iID_MaPhongBan == "-1") sTenDonVi = "";
            if (iID_MaPhongBan != null && iID_MaPhongBan != "-1")
            {
                sTenDonVi = "B " + iID_MaPhongBan;
                var phongban = _nganSachService.GetPhongBanById(iID_MaPhongBan);
                if (!string.IsNullOrWhiteSpace(phongban.sMoTa))
                    sTenPhongBan = phongban.sMoTa.ToUpper();
            }

            DataTable dtThongTinBaoCao = ThuNopModels.getThongTinCotBaoCao("3");
            for (int i = 0; i <15; i++)
            {
                fr.SetValue("sTen" + i, dtThongTinBaoCao.Rows[i]["sTenVietTat"]);
            }

            fr.SetValue("sTenDonVi", sTenDonVi);
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

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_ThuNop_TongHop(String MaND,String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "", DKHAVING = ""; ;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;
            var dtBaocao = ThuNopModels.getThongTinCotBaoCao("3");

            int SoCot = dtBaocao.Rows.Count;
            for (int i = 0; i < SoCot; i++)
            {
                DK += ",COT" + i + "=SUM(CASE WHEN (sLNS LIKE '" + dtBaocao.Rows[i]["sLoaiHinh"] + "%' OR sLNS IN (" + dtBaocao.Rows[i]["sLoaiHinh"] + ") ) THEN rTuChi/" + DVT + " ELSE 0 END)";
                DKHAVING += "SUM(CASE WHEN (sLNS LIKE '" + dtBaocao.Rows[i]["sLoaiHinh"] + "%' OR sLNS IN (" + dtBaocao.Rows[i]["sLoaiHinh"] + ") ) THEN rTuChi/" + DVT + " ELSE 0 END) <>0  ";
                if (i < SoCot - 1)
                    DKHAVING += " OR ";
            }
            //            String SQL = String.Format(@"SELECT iID_MaDonVi, sTenDonVi
            //{2}
            //FROM DT_ChungTuChiTiet
            // WHERE iTrangThai=1 AND  sLNS LIKE '8%'  AND iNamLamViec=@iNamLamViec {0} {1}
            // GROUP BY iID_MaDonVi,sTenDonVi
            //HAVING {3} ", DKPhongBan, DKDonVi, DK,DKHAVING);

            var sql = FileHelpers.GetSqlQuery("rptDuToan_TongHopCuc_ThuNop_ChiTiet.sql");            
            sql = sql.Replace("@@DKSELECT", DK);
            sql = sql.Replace("@@DKHAVING", DKHAVING);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@DKPB", DKPhongBan);
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
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
        private void LoadData(FlexCelReport fr, String iID_MaPhongBan = "-1")
        {
            DataRow r;

            if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
            {
                iID_MaPhongBan = "-1";
            }

            DataTable data = DT_rptDuToan_ThuNop_TongHop(Username, iID_MaPhongBan);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();
            
          
        }
      
        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        [Authorize]
        public clsExcelResult ExportToExcel(string iID_MaPhongBan = "-1", int trinhky = 0, String MaLoai = "QP")
        {
            if (MaLoai == "NN")
            {
                sFilePath = sFilePath_NN;
                sFilePath_TrinhKy = sFilePath_TrinhKy_NN;

            }
            clsExcelResult clsResult = new clsExcelResult();
            var file = (iID_MaPhongBan != null && iID_MaPhongBan != "-1" && trinhky == 1) ? 
                sFilePath : 
                sFilePath_TrinhKy;

            ExcelFile xls = CreateReport(Server.MapPath(file), User.Identity.Name, iID_MaPhongBan);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1010000_TongHop.xls";
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
        public ActionResult ViewPDF(string iID_MaPhongBan, int trinhky = 0,String MaLoai="QP")
        {
            HamChung.Language();
            if (MaLoai == "NN")
            {
                sFilePath = sFilePath_NN;
                sFilePath_TrinhKy = sFilePath_TrinhKy_NN;

            }
            var file = (iID_MaPhongBan != null && iID_MaPhongBan != "-1" && trinhky == 1) ?
                          sFilePath_TrinhKy :
                          sFilePath;
            ExcelFile xls = CreateReport(Server.MapPath(file), User.Identity.Name, iID_MaPhongBan);

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

