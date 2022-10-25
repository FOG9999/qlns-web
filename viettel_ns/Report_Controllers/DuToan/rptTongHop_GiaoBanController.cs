using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;
using VIETTEL.Services;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptTongHop_GiaoBanController : AppController
    {
        //private const string sFilePath_NSNN = "/Report_ExcelFrom/DuToan/rptDuToan_B7_NSNN.xls";

        private string sViewPath = "~/Report_Views/DuToan/";

        public ActionResult Index()
        {
            String iNamLamViec = ReportModels.LayNamLamViec(Username);

            ViewData["iNamLamViec"] = iNamLamViec;
            ViewData["iID_MaPhongBan"] = new SelectList(new List<SelectListItem>()
           {
                new SelectListItem() { Text = "-- TẤT CẢ --", Value= "-1"},
                new SelectListItem() { Text = "B7", Value= "07"},
                new SelectListItem() { Text = "B10", Value= "10"},
            });


            ViewData["path"] = sViewPath + this.ControllerName() + ".aspx";
            return View(sViewPath + "ReportView.aspx");
        }


    }
}