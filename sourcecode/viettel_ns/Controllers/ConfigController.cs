using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Services;
using Dapper;
using DapperExtensions;
using System.Data.SqlClient;

namespace VIETTEL.Controllers
{
    public class ConfigController : AppController
    {
        // GET: Config
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangeYear(int year, string url)
        {
            if (year > DateTime.Now.Year + 1)
                year = DateTime.Now.Year + 1;

            if (year < 2010)
                year = DateTime.Now.Year;

            if (string.IsNullOrWhiteSpace(url))
                url = this.GetBaseUrl();

            #region update database

            var sql = @"UPDATE DC_NguoiDungCauHinh SET iNamLamViec=@iNamLamViec WHERE sID_MaNguoiDungTao=@sID_MaNguoiDungTao";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", year);
                    cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", Username);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //throw;
                    }
                }
            }

            #endregion

            ClearPhienLamViec();

            url = this.GetBaseUrl() + url.Replace("%2f", "/");
            return Json(new { url = url }, JsonRequestBehavior.AllowGet);
        }
    }
}