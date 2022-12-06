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
using Viettel.Domain.DomainModel;
using Viettel.Data;

namespace VIETTEL.DuToan_Report_Controllers
{
    public class DuToan_ReportController : FlexcelReportController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        // GET: /Report/
        public string sViewPath = "~/Report_Views/DuToan/";
        public ActionResult Index()
        {
            return View(sViewPath + "Report_Index.aspx");
        }

        public ActionResult TrinhKy()
        {
            return View(sViewPath + "Report_TrinhKy.aspx");
        }

        public ActionResult getDLBDKT()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11")
            {
                using (var conn = _connectionFactory.GetConnection())
                using (var cmd = new SqlCommand("sp_dt_getbdkt", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParams(new
                    {
                        nam = PhienLamViec.NamLamViec,
                    });
                    conn.Open();
                    var result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
