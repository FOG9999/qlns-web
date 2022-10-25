using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_NNController : FlexcelReportController
    {
        private const string _filePath = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_NN/rptDuToan_NN.xls";
        private string _path = "~/Areas/DuToan";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt;
        private string _nganh;
        private string _donvi;
        private string _phongban;
        private string _loaibaocao;
        private int _to;

        public ActionResult Index()
        {
            if (PhienLamViec.userRole == 2)
            {
                var phongbanList = (PhienLamViec.iID_MaPhongBan != "02" && PhienLamViec.iID_MaPhongBan != "11") ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "-- Tất cả phòng ban --"); ;

                var vm = new rptDuToan_THNSNNViewModel()
                {
                    PhongBanList = phongbanList,
                };

                var view = "~/Areas/DuToan/Views/Report/kiem/rptDuToan_NN.cshtml";

                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }            
        }        
        
        #region in_tonghop

        public ActionResult Print_TongHop(
           string phongban,
           string ext = "pdf",
           int dvt = 1)
        {
            this._dvt = dvt;
            this._phongban = phongban;

            HamChung.Language();

            var xls = createReport_tonghop();
            return Print(xls, ext);

        }

        private ExcelFile createReport_tonghop()
        {
            var xls = new XlsFile(true);

            xls.Open(Server.MapPath(_filePath));
            var fr = new FlexCelReport();

            var data = getDataTable();
            FillDataTable_NG(fr, data, iNamLamViec: PhienLamViec.iNamLamViec);
            
            var tenPhongBan = _nganSachService.GetPhongBanById(_phongban).sMoTa.ToUpper();


            fr.SetValue(new
            {
                h1 = "Toàn quân",
                h2 = $"Đơn vị tính: {_dvt.ToStringDvt()} ",
                TieuDe1 = "Dự kiến số dự toán ngân sách nhà nước chi sự nghiệp",
                TieuDe2 = "",
                tenPhongBan,
                NamTruoc = PhienLamViec.NamLamViec - 1,
                Nam = PhienLamViec.NamLamViec,
            })
              .UseCommonValue(new Application.Flexcel.FlexcelModel())
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }

            return xls;
        }

        private DataTable getDataTable()
        {
            var path_sql = _path + "/Sql/dt_nsnn.sql";

            var sql = FileHelpers.GetSqlQueryPath(path_sql);

            #region load data

            if (_phongban == "02") _phongban = string.Empty;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    PhienLamViec.iNamLamViec,
                    id_phongban = _phongban.ToParamString(),
                    dvt = _dvt,
                });
                return dt;
            }

            #endregion
        }        
        #endregion
    }
}
