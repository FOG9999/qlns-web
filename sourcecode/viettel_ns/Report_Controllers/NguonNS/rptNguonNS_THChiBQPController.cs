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
using Viettel.Data;

namespace VIETTEL.Report_Controllers.NguonNS
{
    public class rptNguonNS_THChiBQPController : AppController
    {
        public string sViewPath = "~/Report_Views/NguonNS/";
        private const string sFilePath = "/Report_ExcelFrom/NguonNS/rptNguonNS_THChiBQP.xls";

        private readonly INguonNSService _nguonNSService = NguonNSService.Default;
        private readonly ISharedService _sharedService = SharedService.Default;

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/NguonNS/rptNguonNS_THChiBQP.aspx";
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
            ViewData["PageLoad"] = "1";
            ViewData["path"] = "~/Report_Views/NguonNS/rptNguonNS_THChiBQP.aspx";
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

            fr.SetValue("Nam", _sharedService.LayNamLamViec(User.Identity.Name));
            fr.SetValue("rheader", "Đơn vị tính: 1.000 đồng - Trang: ");
            fr.Run(xls);
            return xls;

        }

        private DataSet getDS()
        {
            SqlCommand cmd = new SqlCommand("sp_report_CTCHIBQP");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nam", _sharedService.LayNamLamViec(User.Identity.Name));
            cmd.Parameters.AddWithValue("@dvt", 1000);
            var ds = Connection.GetDataSet(cmd);
            return ds;
        }


        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr)
        {
            var ds = getDS();

            var data1 = ds.Tables[0];
            data1.TableName = "dtNguon";

            var data2 = ds.Tables[1];
            data2.TableName = "dtDauNam";

            var data3 = ds.Tables[2];
            data3.TableName = "dtDaDuyet";

            DataTable data31 = _sharedService.SelectDistinct("dtKhoi", data3, "Khoi", "Khoi,TenKhoi");
            fr.AddRelationship("dtKhoi", "dtDaDuyet", "Khoi".Split(','), "Khoi".Split(','));

            var data4 = ds.Tables[3];
            data4.TableName = "dtChoPheDuyet";

            fr.AddTable("dtNguon", data1);
            fr.AddTable("dtDauNam", data2);
            fr.AddTable("dtDaDuyet", data3);
            fr.AddTable("dtKhoi", data31);
            fr.AddTable("dtChoPheDuyet", data4);

            data1.Dispose();
            data2.Dispose();
            data3.Dispose();
            data31.Dispose();
            data4.Dispose();
        }

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF()
        {
            ExcelFile xls = createReport();
            return _sharedService.ViewPDF(xls);
        }

        public ActionResult DownloadExcel()
        {
            ExcelFile xls = createReport();
            var filename = "Phụ_lục_tổng_hợp__giao_dự_toán_ngân_sách_năm" + _sharedService.LayNamLamViec(User.Identity.Name) + DateTime.Now.ToShortDateString() + ".xls";
            return _sharedService.ExportToExcel(xls, filename);
        }
    }
}

