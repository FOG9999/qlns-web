using System;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using VIETTEL.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1020800_TongHopController : AppController
    {
        public string sViewPath = "~/Report_Views/DuToan/";
        private const string sFilePath = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_1020800_TongHop.xls";


        public rptDuToan_1020800_TongHopController()
        {
        }


        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_1020800_TongHop.aspx";
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
            ViewData["PageLoad"] = "1";
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_1020800_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        private ExcelFile createReport()
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);

            fr.SetValue("sTenDonVi", "BỘ QUỐC PHÒNG");

            fr.UseCommonValue()
              .UseChuKy()
              .Run(xls);
            var count = xls.TotalPageCount();

            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;

        }       

        private DataTable getDuToan_1020800_TongHop()
        {
            #region sql

            var sql = FileHelpers.GetSqlQuery("rptDuToan_1020800_TongHop.sql");
            var config = NganSachService.Default.GetCauHinh(Username);
            var nam = config.iNamLamViec;
            #endregion

            #region get data
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@nam", nam);
            cmd.Parameters.AddWithValue("@dvt", 1000);
            cmd.CommandText = sql;

            var dt = Connection.GetDataTable(cmd);
            return dt;
            #endregion
        }


        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr)
        {
            DataTable data = getDuToan_1020800_TongHop();
            data.TableName = "ChiTiet";

            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");

            fr.AddTable("ChiTiet", data);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();          
        }

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF()
        {
            HamChung.Language();

            ExcelFile xls = createReport();
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download()
        {
            HamChung.Language();

            ExcelFile xls = createReport();
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}

