using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;


namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M99_ToBiaController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99_ToBia.xls";
        private const string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99_ToBia_TrinhKy.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _dvt = 1000;
        private int _to = 1;
        private int _loaiBaoCao = 0;
        private string _request = "-1";

        private int _columnCount
        {
            get { return _to == 1 ? 12 : 14; }
        }

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptDuToanKT_M99ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                RequestList = _duToanKTService.GetRequestList().ToSelectList(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }


        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            string request,
            int loaiBaoCao = 0,
            int to = 1,
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(id_phongban);
            _loaiBaoCao = loaiBaoCao;
            _request = request;
            _to = to;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport();
            return Print(xls, ext);
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_loaiBaoCao == 0 ? _filePath : _filePath_trinhky));

            fr
              .UseCommonValue()
              .SetValue(new
              {
                  header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                  TieuDe1 = L("DTKT_TieuDe1", PhienLamViec.iNamLamViec.ToValue<int>() + 1),
                  TieuDe2 = "",
                  QuyetDinh = L("DTKT_QuyetDinh"),
                  To = _to,
              })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            return xls;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        private void loadData(FlexCelReport fr)
        {
            var data = getTable().SetTableName("ChiTiet");
            fr.AddTable(data);
        }

        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m99_tobia.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam = PhienLamViec.iNamLamViec,
                        id_phongban = _id_phongban.ToParamString(),
                        request = _request.ToParamString(),
                        id_donvi = PhienLamViec.iID_MaDonVi.ToParamString(),
                        dvt = _dvt
                    });
                return dt;
            }
            #endregion
        }
        #endregion
    }
}
