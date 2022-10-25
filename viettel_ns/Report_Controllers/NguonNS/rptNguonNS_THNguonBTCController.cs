using System;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using VIETTEL.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using Viettel.Services;
using Viettel.Data;
using System.Data.SqlClient;
using DomainModel;

namespace VIETTEL.Report_Controllers.NguonNS
{
    public class rptNguonNS_THNguonBTCController : AppController
    {
        public string sViewPath = "~/Report_Views/NguonNS/";
        private const string sFilePath = "/Report_ExcelFrom/NguonNS/rptNguonNS_THNguonBTC.xls";

        private readonly INguonNSService _nguonNSService = NguonNSService.Default;
        private readonly ISharedService _sharedService = SharedService.Default;

        public ActionResult Index()
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/NguonNS/rptNguonNS_THNguonBTC.aspx";
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
            ViewData["path"] = "~/Report_Views/NguonNS/rptNguonNS_THNguonBTC.aspx";
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
            var nam = _sharedService.LayNamLamViec(User.Identity.Name);
            var namt = (Convert.ToInt32(nam) - 1).ToString();

            fr.SetValue("nam", nam);
            fr.SetValue("namt", namt);
            fr.SetValue("rheader", "Đơn vị tính: Đồng - Trang: ");
            fr.Run(xls);
            return xls;

        }

        private DataTable getData()
        {
            SqlCommand cmd = new SqlCommand("sp_report_THNGUONBTC");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nam", _sharedService.LayNamLamViec(User.Identity.Name));
            cmd.Parameters.AddWithValue("@dvt", 1);
            var ds = Connection.GetDataTable(cmd);
            return ds;
        }


        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr)
        {
            var data = getData();
            data.TableName = "ChiTiet";
                        
            DataTable dtCap3 = _sharedService.SelectDistinct("dtCap3", data, "Cap1,Cap2,Cap3", "Cap1,Cap2,Cap3,CTMT,L,K,NoiDung_Nguon", "Nguon_DMBTC", "NoiDung_Nguon,CTMT,L,K", "Nam", "Ma_Nguon", "Cap3");
            DataTable dtCap2 = _sharedService.SelectDistinct("dtCap2", data, "Cap1,Cap2", "Cap1,Cap2,CTMT,L,K,NoiDung_Nguon", "Nguon_DMBTC", "NoiDung_Nguon,CTMT,L,K", "Nam", "Ma_Nguon", "Cap2");
            DataTable dtCap1 = _sharedService.SelectDistinct("dtCap1", dtCap2, "Cap1", "Cap1,CTMT,L,K,NoiDung_Nguon", "Nguon_DMBTC", "NoiDung_Nguon,CTMT,L,K", "Nam", "Ma_Nguon", "Cap1");

            fr.AddTable("ChiTiet", data);
            fr.AddTable("dtCap3", dtCap3);
            fr.AddTable("dtCap2", dtCap2);
            fr.AddTable("dtCap1", dtCap1);

            fr.AddRelationship("dtCap1", "dtCap2", "Cap1".Split(','), "Cap1".Split(','));
            fr.AddRelationship("dtCap2", "dtCap3", "Cap1,Cap2".Split(','), "Cap1,Cap2".Split(','));
            fr.AddRelationship("dtCap3", "ChiTiet", "Cap1,Cap2,Cap3".Split(','), "Cap1,Cap2,Cap3".Split(','));

            data.Dispose();
            dtCap3.Dispose();
            dtCap2.Dispose();
            dtCap1.Dispose();
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
            var filename = "Tổng_hợp_giao_dự_toán_ngân_sách_năm" + _sharedService.LayNamLamViec(User.Identity.Name) + DateTime.Now.ToShortDateString() + ".xls";
            return _sharedService.ExportToExcel(xls,filename);
        }
    }
}

