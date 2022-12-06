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
using Viettel.Extensions;
using VIETTEL.Helpers;
using Viettel.Data;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_BaoDam_TongHop_BiaController : AppController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        public string sViewPath = "~/Report_Views/DuToan/";
        private  String sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_BaoDam_TongHop_Bia.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_BaoDam_TongHop_Bia.xls";
        String iCapTongHop = "";
        [Authorize]
        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_BaoDam_TongHop_Bia.aspx";
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
        public ActionResult EditSubmit(string ParentID)
        {
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_BaoDam_TongHop_Bia.aspx";

            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        private ExcelFile CreateReport(string iID_MaPhongBan, string iCapTongHop, int trinhky = 0)
        {            
            var file = sFilePath;
            if(iCapTongHop == "Muc"){
                file = sFilePath_Muc;
            }
            
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaPhongBan);

            var loaiKhoan = HamChung.GetLoaiKhoanText("1040100");
            fr.SetValue("LoaiKhoan", loaiKhoan);

            fr.UseCommonValue()
              .UseChuKy(Username, iID_MaPhongBan)
              .UseChuKyForController(this.ControllerName())
              .Run(xls);

            return xls;
        }

        //1020200
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_1040100_TongHop(String MaND,String iID_MaPhongBan)
        {
            if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
                iID_MaPhongBan = "-1";

            string DKDonVi = "", DKPhongBan = "";
            string sTenB10 = "TC,BTTM";
            string sTenB6 = "DN";
            string sTenB = "QBC";

            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_Dich(MaND, cmd, iID_MaPhongBan);

            var sql = FileHelpers.GetSqlQuery("rptDuToan_BaoDam_TongHop_Bia.sql");

            sql = sql.Replace("@@DKPB", DKPhongBan);
            sql = sql.Replace("@@DKDV", DKDonVi);
            sql = sql.Replace("@@sTenB10", sTenB10);
            sql = sql.Replace("@@sTenB6", sTenB6);
            sql = sql.Replace("@@sTenB", sTenB);
            cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;

            var dt = Connection.GetDataTable(cmd);
            return dt;
        }
         
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            DataTable data = DT_rptDuToan_1040100_TongHop(MaND, iID_MaPhongBan);           

            data.Columns.Add("CheckTrung", typeof(String));
            for (int i = 1; i < data.Rows.Count; i++)
            {
                String sXau1, sXau2;
                sXau1 = Convert.ToString(data.Rows[i - 1]["sM"]) + Convert.ToString(data.Rows[i - 1]["sTM"]) + Convert.ToString(data.Rows[i - 1]["sTTM"]) + Convert.ToString(data.Rows[i - 1]["sNG"]);
                sXau2 = Convert.ToString(data.Rows[i]["sM"]) + Convert.ToString(data.Rows[i]["sTM"]) + Convert.ToString(data.Rows[i]["sTTM"]) + Convert.ToString(data.Rows[i]["sNG"]);
                if (sXau1.Equals(sXau2))
                {
                    data.Rows[i]["CheckTrung"] = "1";
                }
            }

            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "iID_MaNganh,sM,sTM", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sMoTa", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "iID_MaNganh,sM", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sMoTa", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM");
            DataTable dtsNganh = HamChung.SelectDistinct("dtsNganh", dtsM, "iID_MaNganh", "iID_MaNganh,sTenNganh", "iID_MaNganh,sTenNganh");

            fr.AddTable("ChiTiet", data);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsNganh", dtsNganh);

            foreach (DataRow dtr in dtsTM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM,sTM".Split(','), MaND);
            }
            foreach (DataRow dtr in dtsM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM".Split(','), MaND);
            }

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsNganh.Dispose();
        }
      
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
    }
}

