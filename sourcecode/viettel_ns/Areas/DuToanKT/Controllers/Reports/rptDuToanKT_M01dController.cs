using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M01dController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";

        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01d.xls";
        private string _filePath_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01d_to2.xls";



        private string _filePath2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01d_tonghop.xls";
        private string _filePath2_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01d_tonghop_to2.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _loaiBaoCao;
        private int _trinhky;
        private int _request;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        //private int _columnCount = 3;

        private int _columnCount
        {
            get
            {
                return _loaiBaoCao == 0 ? 3 : 2;
            }
        }



        public ActionResult Index()
        {
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptDuToanKT_M01ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                RequestList = _duToanKTService.GetRequestList_PhongBan(_nganSachService.CheckParam_PhongBan(PhienLamViec.iID_MaPhongBan)).ToSelectList(),
                TieuDe = getTieuDe(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            int loaiBaoCao = 0,
            int trinhky = 0,
            int request = -1,
            string tieuDe = null,
            int to = 1,
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(id_phongban);
            _loaiBaoCao = loaiBaoCao;
            _trinhky = trinhky;
            _request = request;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;
            _filePath = _to == 1 ?
                    (_loaiBaoCao == 0 ? _filePath : _filePath2) :
                    (_loaiBaoCao == 0 ? _filePath_to2 : _filePath2_to2);

            var xls = createReport(tieuDe);
            return Print(xls, ext);
        }


        private ExcelFile createReport(string tieuDe = null)
        {
            var fr = new FlexCelReport();
            if (_loaiBaoCao == 0)
            {
                loadData(fr);
            }
            else
            {
                loadData_tonghop(fr);
            };

            var tenPhongBan = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa;

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseCommonValue()
                .SetValue(new
                {
                    TieuDe1 = tieuDe ?? getTieuDe(),
                    TieuDe2 = tenPhongBan,
                    NamSau = PhienLamViec.iNamLamViec.ToValue<int>() + 1,
                    TrinhKy = _trinhky,

                    header1 = tenPhongBan,
                    header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}  Tờ: {_to}",
                })
                .UseChuKy(Username)
                .UseChuKyForController(this.ControllerName(), chuky: "1,2")
                .UseForm(this)
                .Run(xls);

            return xls;
        }


        private void loadData(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(), () => getTable(), CacheTimes.OneMinute, false);

            var data = dt.SelectDistinct("ChiTiet", "Code1,Code2,Code3,Code");
            var dtX = dt.SelectDistinct("X", "Id_DonVi,TenDonVi");

            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}1", typeof(double)));
                data.Columns.Add(new DataColumn($"C{i + 1}2", typeof(double)));
                data.Columns.Add(new DataColumn($"C{i + 1}3", typeof(double)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["TenDonVi"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(r, 0);
            }


            columns = _to == 1 ?
                dtX.AsEnumerable().Skip(0).Take(_columnCount).ToList() :
                dtX.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colName1 = $"C{i + 1}1";
                var colName2 = $"C{i + 1}2";
                var colName3 = $"C{i + 1}3";
                if (i < columns.Count)
                {

                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("TenDonVi"));
                    fr.SetValue(colName1, PhienLamViec.iNamLamViec);
                    fr.SetValue(colName2, "Đề nghị");
                    fr.SetValue(colName3, "Đặc thù");

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           // 2018
                           var value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<decimal>("Dt_tuchi", 0));
                           r[colName1] = value;

                           // BQL
                           value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<double>("TuChi", 0));
                           r[colName2] = value;

                           // DacThu
                           value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<double>("DacThu", 0));

                           r[colName3] = value;
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colName1, PhienLamViec.iNamLamViec);
                    fr.SetValue(colName2, "");
                    fr.SetValue(colName3, "");
                }

            }

            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3", PhienLamViec.iNamLamViec);
        }



        private void loadData_tonghop(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(), () => getTable(), CacheTimes.OneMinute, false);

            var data = dt.SelectDistinct("ChiTiet", "Code1,Code2,Code3,Code");
            var dtX = dt.SelectDistinct("X", "Id_DonVi,TenDonVi");

            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}1", typeof(double)));
                data.Columns.Add(new DataColumn($"C{i + 1}2", typeof(double)));
                data.Columns.Add(new DataColumn($"C{i + 1}3", typeof(double)));
                data.Columns.Add(new DataColumn($"C{i + 1}4", typeof(double)));
                data.Columns.Add(new DataColumn($"C{i + 1}5", typeof(double)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["TenDonVi"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(r, 0);
            }


            columns = _to == 1 ?
                dtX.AsEnumerable().Skip(0).Take(_columnCount).ToList() :
                dtX.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colName1 = $"C{i + 1}1";
                var colName2 = $"C{i + 1}2";
                var colName3 = $"C{i + 1}3";
                var colName4 = $"C{i + 1}4";
                var colName5 = $"C{i + 1}5";
                if (i < columns.Count)
                {

                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("TenDonVi"));
                    fr.SetValue(colName1, PhienLamViec.iNamLamViec);
                    fr.SetValue(colName2, "Đề nghị");
                    fr.SetValue(colName3, "Đặc thù");

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           #region sum to cell

                           // 2018
                           var value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<decimal>("Dt_tuchi", 0));
                           r[colName1] = value;

                           // BQL
                           value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<double>("TuChi", 0));
                           r[colName2] = value;

                           // DacThu
                           value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<double>("DacThu", 0));

                           r[colName3] = value;

                           // TangNV
                           value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<double>("TangNV", 0));

                           r[colName4] = value;

                           // GiamNV
                           value = (double)dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<double>("GiamNV", 0));

                           r[colName5] = value;
                           #endregion
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colName1, PhienLamViec.iNamLamViec);
                    fr.SetValue(colName2, "");
                    fr.SetValue(colName3, "");
                }

            }

            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3", PhienLamViec.iNamLamViec);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m01d.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        id_phongban = _id_phongban.ToParamString(),
                        id_donvi = PhienLamViec.DonViList.Select(x => x.Key).Join(),
                        request = _request.ToParamString(),
                        nam = _nam,
                        dvt = _dvt,
                    });
                return dt;
            }
            #endregion
        }

        private string getTieuDe()
        {
            var nam = int.Parse(PhienLamViec.iNamLamViec) + 1;
            var tieude = $"Thông báo số kiểm tra dự toán ngân sách năm {nam}";

            return tieude;
        }

        public JsonResult Ds_To(string id_PhongBan, int? request)
        {
            var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, getParamUsername(), id_PhongBan);
            var count = data.Rows.Count;
            return ds_ToIn(count > 0 ? count + 1 : 0, _columnCount);
        }
        private string getParamUsername()
        {
            var username = Username;
            if (_nganSachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyPhongBan)
            {
                username = "";
            }
            return username;
        }
        private string getCacheKey()
        {
            return $"{this.ControllerName()}_{Username}_{_nam}_{_id_phongban}_{_loaiBaoCao}_{_request}_{_dvt}";
        }

        #endregion
    }
}
