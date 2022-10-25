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
    public class rptDuToanKT_M99ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
        public SelectList RequestList { get; set; }

        public string TieuDe { get; set; }
    }
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M99Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private const string _filePath_a = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99.xls";
        private const string _filePath_a_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99_to2.xls";

        private const string _filePath_b = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M99b.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _dvt = 1000;
        private int _to = 1;
        private int _loaiBaoCao = 0;
        private string _request = "-1";
        private string _id_nganh;

        private int _columnCount
        {
            get { return _to == 1 ? 12 : 14; }
        }



        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var nganhList = _nganSachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username)
                                         .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");


            var vm = new rptDuToanKT_M99ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                RequestList = _duToanKTService.GetRequestList().ToSelectList(),
                NganhList = nganhList,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }


        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            string id_nganh,
            string request,
            int loaiBaoCao = 0,
            int to = 1,
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(id_phongban);
            _loaiBaoCao = loaiBaoCao;
            _id_nganh = id_nganh;
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
            xls.Open(Server.MapPath(_to == 1 ? _filePath_a : _filePath_a_to2));

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
                  header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                  TenPhongBan = string.IsNullOrWhiteSpace(tenPhongBan) ? "Tổng hợp Cục" : $"Kính gửi: {tenPhongBan}",
                  TieuDe2 = _loaiBaoCao == 1 ? "Phân cấp đơn vị" : "Tự chi tại ngành",
                  To = _to,
              })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);
            if (_to == 1)
                xls.MergeH(8, 3, 18);
            else
                xls.MergeH(1, 3, 18);

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
                r1["NganhCon"] = "00,01";
                r1["sMoTa"] = "Nghiệp vụ";

                var r2 = dtNganh.NewRow();
                r2["TenNganh"] = "Tổng cộng";
                r2["Nganh"] = "";
                r2["NganhCon"] = "02";
                r2["sMoTa"] = "BĐKT";

                dtNganh.Rows.InsertAt(r2, 0);
                dtNganh.Rows.InsertAt(r1, 0);

            }


            columns = _to == 1 ?
                dtNganh.AsEnumerable().Take(_columnCount).ToList() :
                dtNganh.AsEnumerable().Skip((_to - 1) * _columnCount - 4).Take(_columnCount).ToList();
            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colName1 = $"C{i + 1}_1";

                if (i < columns.Count)
                {

                    fr.SetValue(colName, columns[i].Field<string>("TenNganh"));
                    fr.SetValue(colName1, columns[i].Field<string>("sMoTa"));

                    dt.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {

                           // bdkt
                           //var value = columns[i].Field<string>("NganhCon") == "02" ?
                           //     data.AsEnumerable()
                           //        .ToList()
                           //        .Where(x =>
                           //         x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                           //         (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")) &&
                           //         x.Field<string>("NganhCon") == "02")
                           //        .Sum(x => x.Field<double>("TuChi")) :
                           //     data.AsEnumerable()
                           //        .ToList()
                           //        .Where(x =>
                           //         x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                           //         (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")) &&
                           //         x.Field<string>("NganhCon") != "02")
                           //        .Sum(x => x.Field<double>("TuChi"));


                           var value =
                                data.AsEnumerable()
                                   .ToList()
                                   .Where(x =>
                                    x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")) &&
                                    columns[i].Field<string>("NganhCon").Contains(x.Field<string>("NganhCon")))
                                   .Sum(x => x.Field<double>("TuChi"));

                           r[colName] = value;

                           //value = data.AsEnumerable()
                           //        .ToList()
                           //        .Where(x => x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                           //         (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")))
                           //        .Sum(x => x.Field<double>("GiamNV"));
                           //r[colName2] = value;

                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colName1, "");
                }

            }

            fr.AddTable(dt);
            fr.AddTableEmpty(dt.Rows.Count, 10);
        }

        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m99_nganh.sql");

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

        public JsonResult Ds_To(string id_phongban, string request, int loaiBaoCao, string id_nganh)
        {
            var data = getTable_Nganh(id_phongban, request, loaiBaoCao, id_nganh);
            var dt = data.SelectDistinct("aa", "Nganh,NganhCon");
            return ds_ToIn(dt.Rows.Count == 0 ? 0 : dt.Rows.Count + 1, 14);
        }

        private DataTable getTable_Nganh(string id_phongban, string request, int loaiBaoCao, string id_nganh)
        {
            var nganh = id_nganh == "-1" ?
               // lay nganh theo phong ban va nguoi dung
               _nganSachService.Nganh_GetAll(_nganSachService.CheckParam_Username(Username, PhienLamViec.iID_MaPhongBan), _id_phongban)
                   .AsEnumerable()
                   .Select(r => r.Field<string>("iID_MaNganhMLNS"))
                   .JoinDistinct() :
               _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).iID_MaNganhMLNS;


            var sql = FileHelpers.GetSqlQuery("dtkt_report_m99_nganh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var dt = conn.GetTable(sql,
                        new
                        {
                            nam = PhienLamViec.iNamLamViec,
                            request = request.ToParamString(),
                            id_phongban = id_phongban.ToParamString(),
                            id_donvi = PhienLamViec.iID_MaDonVi.ToParamString(),
                            nganh = nganh.ToParamString(),
                            loai = loaiBaoCao,
                            dvt = _dvt,
                        });
                return dt;
            }
        }



        #endregion
    }
}
