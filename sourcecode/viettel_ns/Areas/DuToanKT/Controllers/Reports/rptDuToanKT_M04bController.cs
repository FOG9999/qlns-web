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
    public class rptDuToanKT_M04bViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
        public SelectList RequestList { get; set; }

    }
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M04bController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04b.xls";
        private const string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04b_trinhky.xls";
        private const string _filePath_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04b_to2.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _dvt = 1000;
        private int _to = 1;
        private int _request = 0;
        private int _columnsCount = 12;



        public ActionResult Index()
        {
            var phongbanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            //var nganhList = _duToanKTService.GetNganh(PhienLamViec.iNamLamViec, phongbanList.Rows[0].Field<string>("sKyHieu"), Username)
            //                                .ToSelectList("NG", "TenNganh", "-1", "-- chọn ngành --");


            var nganhList = _nganSachService.Nganh_GetAll(PhienLamViec.iNamLamViec,Username)
                                            .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");


            var vm = new rptDuToanKT_M04bViewModel
            {
                NganhList = nganhList,
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                RequestList = _duToanKTService.GetRequestList().ToSelectList(group: "request", selectedValues: "0"),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }



        public JsonResult Ds_To(string id_phongban, string id_nganh, int request)
        {
            var data = getTable(PhienLamViec.iNamLamViec, id_phongban, id_nganh, request, Username);
            var nganhs = data.SelectDistinct("nganh", "sNG");
            return ds_ToIn(nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + 1, _columnsCount);
        }

        public ActionResult Ds_Nganh(string id_phongban)
        {
            //var list = _duToanKTService.GetNganh(PhienLamViec.iNamLamViec, id_phongban, Username)
            //                              .ToSelectList("Ng", "TenNganh", "-1", " -- chọn ngành --");
            var list = _nganSachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username)
                                          .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");


            return ToDropdownList(new VIETTEL.Models.ChecklistModel("id_nganh", list));
        }

        public ActionResult Print(
            string ext,
            string id_phongban,
            string id_nganh,
            int request,
            int loaiBaoCao,
            int to,
            int dvt = 1000)
        {
            _id_phongban = id_phongban;
            _id_nganh = id_nganh;
            _request = request;
            _to = to;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            _filePath = loaiBaoCao == 0 ? _filePath : _filePath_trinhky;

            //if (_id_nganh == "-1")
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    var xls = createReport();
            //    return Print(xls, ext);
            //}

            var xls = createReport();
            return Print(xls, ext);
        }

        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));
            //xls.Open(Server.MapPath(_to == 1 ? _filePath : _filePath_to2));
            var fr = new FlexCelReport();
            loadData(fr);

            fr.SetValue(new
            {
                header1 = string.IsNullOrWhiteSpace(_id_nganh) || _id_nganh == "-1" ? "(Tất cả các ngành)" : "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh,
                header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}     Tờ: {_to}",
                To = _to,
                Nam = PhienLamViec.iNamLamViec.ToValue<int>() + 1,
                TenPhongBan = string.IsNullOrWhiteSpace(_nganSachService.CheckParam_PhongBan(_id_phongban)) ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                Request = _duToanKTService.GetRequestList()[_request.ToString()],
            });



            fr.UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls, _to);

            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getTable(PhienLamViec.iNamLamViec, _id_phongban, _id_nganh, _request, Username);

            var dt = data.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtNganh = data.SelectDistinct("Nganh", "Nganh,TenNganh");

            // them cot
            for (int i = 0; i < _columnsCount; i++)
            {
                dt.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var rNganh = dtNganh.NewRow();
                rNganh["TenNganh"] = "Tổng cộng";
                dtNganh.Rows.InsertAt(rNganh, 0);
            }


            columns = dtNganh.AsEnumerable().Skip((_to - 1) * _columnsCount - 1).Take(_columnsCount).ToList();
            for (int i = 0; i < _columnsCount; i++)
            {
                if (i < columns.Count)
                {
                    var colName = $"C{i + 1}";
                    fr.SetValue(colName, columns[i].Field<string>("TenNganh"));

                    dt.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {

                           var value = data.AsEnumerable()
                                   .ToList()
                                   .Where(x => x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")))
                                   .Sum(x => x.Field<double>("TuChi"));
                           r[colName] = value;

                       });
                }
                else
                {
                    fr.SetValue($"C{i + 1}", "");
                }

            }

            // tien bang chu tai trang 1
            var tien = string.Empty;
            if (_to == 1)
            {
                var sum = dt.AsEnumerable().Sum(x => x["C1"] == null || x["C1"] == DBNull.Value ? 0 : x.Field<decimal>("C1"));
                tien = (sum * _dvt).ToStringMoney();
            }
            fr.SetValue("Tien", tien);

            fr.AddTable(dt);
            fr.AddTableEmpty(dt.Rows.Count, 10);

        }

        private DataTable getTable(string nam, string id_phongban, string id_nganh, int request, string username)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m04b.sql");

            username = _nganSachService.CheckParam_Username(username, id_phongban);

            string id_donvi = PhienLamViec.iID_MaDonVi;
            // b tong hop
            if (id_phongban == "10" || string.IsNullOrWhiteSpace(_nganSachService.CheckParam_PhongBan(id_phongban)))
            {
                id_donvi = "";
                id_phongban = "";
            }
            //id_phongban = _nganSachService.CheckParam_PhongBan(id_phongban);

            using (var conn = _connectionFactory.GetConnection())
            {
                //var nganh = id_nganh == "-1" ? "-1" : _ngansachService.Nganh_Get(id_nganh).iID_MaNganhMLNS;



                var nganh = id_nganh == "-1" ?
                    // lay nganh theo phong ban va nguoi dung
                    _nganSachService.Nganh_GetAll(_nganSachService.CheckParam_Username(Username, PhienLamViec.iID_MaPhongBan), _id_phongban)
                        .AsEnumerable()
                        .Select(r => r.Field<string>("iID_MaNganhMLNS"))
                        .JoinDistinct() :
                    _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).iID_MaNganhMLNS;

                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        id_phongban = id_phongban.ToParamString(),
                        username = username.ToParamString(),
                        nganh = nganh.ToParamString(),
                        id_donvi = id_donvi.ToParamString(),
                        request,
                        dvt = _dvt,
                    });

                return dt;
            }
        }

        #endregion


    }
}
