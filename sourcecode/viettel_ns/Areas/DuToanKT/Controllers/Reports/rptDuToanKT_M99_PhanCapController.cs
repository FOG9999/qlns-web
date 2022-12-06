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


namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M99_PhanCapController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99_PhanCap.xls";
        private const string _filePath_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99_PhanCap_to2.xls";
        private const string _filePath_TrinhKy = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99_TrinhKy_PhanCap.xls";

        private string _filePath_a4 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99_PhanCap_a4.xls";


        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _dvt = 1000;
        private int _to = 1;
        private int _loaiBaoCao = 0;
        private string _request = "-1";

        //private int _columnCount = 12;
        private int _columnCount { get { return _loaiBaoCao == 4 ? 8 : 12; } }


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

            if (loaiBaoCao == 4)
            {
                _filePath = _filePath_a4;
            }
            else if (loaiBaoCao == 1)
            {
                _filePath = _filePath_TrinhKy;
            }

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
            xls.Open(Server.MapPath(_filePath));

            var tenPhongBan = string.Empty;
            if (!string.IsNullOrWhiteSpace(_nganSachService.CheckParam_PhongBan(_id_phongban)))
            {
                tenPhongBan = _nganSachService.GetPhongBanById(_id_phongban).sMoTa;
            }

            fr
              .UseCommonValue()
              .SetValue(new
              {
                  header1 = tenPhongBan,
                  header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                  TenPhongBan = string.IsNullOrWhiteSpace(tenPhongBan) ? "Tổng hợp Cục" : $"Kính gửi: {tenPhongBan}",
                  TieuDe1 = L("DTKT_TieuDe1", PhienLamViec.iNamLamViec.ToValue<int>() + 1),
                  QuyetDinh = L("DTKT_QuyetDinh"),
                  TieuDe2 = "(Ngành kỹ thuật - Phân cấp đơn vị)",
                  To = _to,
              })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls, _to);
            if (_to == 1)
                xls.MergeCells(8, 3, 9, 3);

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
            var dtNganh = data.SelectDistinct("Nganh", "Nganh,TenNganh,NganhCon,sMoTa");

            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                dt.Columns.Add(new DataColumn($"C{i + 1}", typeof(double)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {

                var r1 = dtNganh.NewRow();
                r1["TenNganh"] = "Tổng cộng";
                r1["Nganh"] = "";
                r1["NganhCon"] = "02";
                r1["sMoTa"] = "BĐKT";

                dtNganh.Rows.InsertAt(r1, 0);
            }


            columns = _to == 1 ?
                dtNganh.AsEnumerable().Take(_columnCount).ToList() :
                dtNganh.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();
            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colName1 = $"C{i + 1}_1";
                var colIndex = $"i{i + 1}";

                if (i < columns.Count)
                {

                    fr.SetValue(colName, columns[i].Field<string>("TenNganh"));
                    fr.SetValue(colName1, columns[i].Field<string>("sMoTa"));

                    dt.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           var value =
                                data.AsEnumerable()
                                   .ToList()
                                   .Where(x =>
                                    x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")) &&
                                    columns[i].Field<string>("NganhCon") == x.Field<string>("NganhCon"))
                                   .Sum(x => x.Field<double>("TuChi"));

                           r[colName] = value;

                       });

                    if (_to == 1)
                    {
                        fr.SetValue(colIndex, i == 0 ? "" : $"({i})");
                    }
                    else
                    {
                        fr.SetValue(colIndex, $"({ (_to - 1) * _columnCount + i})");
                    }
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colName1, "");
                    fr.SetValue(colIndex, "");
                }

            }

            fr.AddTable(dt);
            fr.AddTableEmpty(dt.Rows.Count, 10);
        }

        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m99_phancap.sql");

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
                        dvt = _dvt,
                        loai = _loaiBaoCao,
                    });
                return dt;
            }
            #endregion
        }

        public JsonResult Ds_To(string id_phongban, string request, int loaiBaoCao)
        {
            _loaiBaoCao = loaiBaoCao;


            var data = getTable_Nganh(id_phongban, request, loaiBaoCao);
            var dt = data.SelectDistinct("aa", "Nganh,NganhCon");
            return ds_ToIn(dt.Rows.Count == 0 ? 0 : dt.Rows.Count + 1, _columnCount);
        }

        private DataTable getTable_Nganh(string id_phongban, string request, int loaiBaoCao)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m99_phancap.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                        new
                        {
                            nam = PhienLamViec.iNamLamViec,
                            request = request.ToParamString(),
                            id_phongban = id_phongban.ToParamString(),
                            id_donvi = PhienLamViec.iID_MaDonVi.ToParamString(),
                            //loai = loaiBaoCao,
                            dvt = _dvt,
                        });
                return dt;
            }
        }


        #endregion
    }
}
