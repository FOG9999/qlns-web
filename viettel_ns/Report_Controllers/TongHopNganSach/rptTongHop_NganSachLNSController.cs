using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptTongHop_NganSachLNSViewModel
    {
        public string iNamLamViec { get; set; }
        public string iID_MaPhongBan { get; set; }
        public SelectList loaiNSList { get; set; }

    }
}

namespace VIETTEL.Report_Controllers.TongHopNganSach
{
    public class rptTongHop_NganSachLNSController : AppController
    {
        private const string _viewPath = "~/Views/Report_Views/TongHopNganSach/";

        public ActionResult Index()
        {
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var iID_MaPhongBan = PhienLamViec.iID_MaPhongBan;
            var dtLNS = Ds_LNS();

            var vm = new rptTongHop_NganSachLNSViewModel
            {
                iNamLamViec = iNamLamViec,
                iID_MaPhongBan = iID_MaPhongBan,
                loaiNSList = dtLNS.ToSelectList("sLNS", "TenLNS")
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #region public methods

        //public ActionResult Print(
        //    string iID_MaPhongBan,
        //    string dsLNS,
        //    string option,
        //    string ext)
        //{      

        //    this.iID_MaPhongBan = iID_MaPhongBan;
        //    this.dsLNS = dsLNS;

        //    var xls = createReport();
        //    return Print(xls, ext);
        //}
        public DataTable Ds_LNS()
        {
            DataTable dt = new DataTable();

            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptMucLucNganSach_PB_LNS.sql");


            #endregion

            #region dieu kien

            int iNamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec);
            if (iNamLamViec < 2017) iNamLamViec = 2017;

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", PhienLamViec.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);

                dt = cmd.GetTable();
            }

            if (PhienLamViec.iID_MaPhongBan == "02" || PhienLamViec.iID_MaPhongBan == "11")
            {
                DataRow dtr = dt.NewRow();
                dtr["sLNS"] = "1";
                dtr["TenLNS"] = "1 - Ngân sách quốc phòng";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "2";
                dtr["TenLNS"] = "2 - Ngân sách nhà nước";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "3";
                dtr["TenLNS"] = "3 - Chi đặc biệt";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "4";
                dtr["TenLNS"] = "4 - Chi bằng nguồn kinh phí khác";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "8";
                dtr["TenLNS"] = "8 - Thu nộp";
                dt.Rows.Add(dtr);
                DataView dv = dt.DefaultView;
                dv.Sort = "sLNS";

                return dv.ToTable();
            }
            else
            {
                return dt;
            }
            #endregion
        }

        #endregion

    }
}