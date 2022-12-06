using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptDuToanKT_M05ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList RequestList { get; set; }

        public string TieuDe { get; set; }
    }
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M05Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private const string _filePath_5 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M05.xls";
        //private const string _filePath_5b = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M05b.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _dvt = 1000;
        private int _to = 1;
        private int _columnCount = 7;
        private int _loaiBaoCao = 0;
        private string _request = "-1";


        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var requestList = new Dictionary<string, string>()
            {
                {"-1", "-- Tổng hợp --" },
                {"1", "Đợt 1" },
                {"2", "Đợt 2" },
                {"3", "Đợt 3" },
                {"4", "Đợt 4" },
                {"5", "Đợt 5" },
            };

            var vm = new rptDuToanKT_M05ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                RequestList = requestList.ToSelectList(),
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
            xls.Open(Server.MapPath(_filePath_5));

            var tenPhongBan = string.Empty;
            if (!string.IsNullOrWhiteSpace(_nganSachService.CheckParam_PhongBan(_id_phongban)))
            {
                tenPhongBan = _nganSachService.GetPhongBanById(_id_phongban).sMoTa;
            }

            fr
              .UseCommonValue()
              .SetValue(new
              {
                  TenPhongBan = string.IsNullOrWhiteSpace(tenPhongBan) ? "Tổng hợp Cục" : $"Kính gửi: {tenPhongBan}",
                  header1 = tenPhongBan,
                  header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
              })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls, _to);

            return xls;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        private void loadData(FlexCelReport fr)
        {
            var data = getTable();

            var dt = data.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtNganh = data.SelectDistinct("Nganh", "Nganh,TenNganh");

            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                dt.Columns.Add(new DataColumn($"C{i + 1}1", typeof(double)));
                dt.Columns.Add(new DataColumn($"C{i + 1}2", typeof(double)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var rNganh = dtNganh.NewRow();
                rNganh["TenNganh"] = "Tổng cộng";
                dtNganh.Rows.InsertAt(rNganh, 0);
            }


            columns = _to == 1 ?
                dtNganh.AsEnumerable().Take(_columnCount).ToList() :
                dtNganh.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();
            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colName1 = $"C{i + 1}1";
                var colName2 = $"C{i + 1}2";

                if (i < columns.Count)
                {

                    fr.SetValue(colName, columns[i].Field<string>("TenNganh"));
                    fr.SetValue(colName1, "Tăng nhiệm vụ");
                    fr.SetValue(colName2, "Giảm nhiệm vụ");

                    dt.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {

                           var value = data.AsEnumerable()
                                   .ToList()
                                   .Where(x => x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")))
                                   .Sum(x => x.Field<double>("TangNV"));
                           r[colName1] = value;

                           value = data.AsEnumerable()
                                   .ToList()
                                   .Where(x => x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")))
                                   .Sum(x => x.Field<double>("GiamNV"));
                           r[colName2] = value;

                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colName1, "");
                    fr.SetValue(colName2, "");
                }

            }

            fr.AddTable(dt);
            fr.AddTableEmpty(dt.Rows.Count, 10);
        }

        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m05.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam = PhienLamViec.iNamLamViec,
                        id_phongban = _id_phongban.ToParamString(),
                        request = _request.ToParamString(),
                        dvt = _dvt,
                        loai = _loaiBaoCao.ToParamString(),
                    });
                return dt;
            }
            #endregion
        }

        public JsonResult Ds_To(string id_phongban, string loaiBaoCao)
        {
            var data = getTable_Nganh(id_phongban, loaiBaoCao);
            return ds_ToIn(data.Rows.Count == 0 ? 0 : data.Rows.Count + 1, _columnCount);
        }

        private DataTable getTable_Nganh(string id_phongban, string loaiBaoCao)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m05_nganh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                        new
                        {
                            nam = PhienLamViec.iNamLamViec,
                            loai = loaiBaoCao.ToParamString(),
                            id_phongban = id_phongban.ToParamString(),
                        });
                return dt;
            }
        }

        #endregion
    }
}
