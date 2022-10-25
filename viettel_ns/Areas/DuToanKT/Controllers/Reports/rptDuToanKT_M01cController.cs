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
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;


namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M01cController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01c.xls";
        private string _filePath_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01c_to2.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 5;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));


            var vm = new rptDuToanKT_M01ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                TieuDe = getTieuDe(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string Id_PhongBan,
            int to,
            string tieuDe,
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(Id_PhongBan);
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;
            _filePath = _to == 1 ? _filePath : _filePath_to2;

            var xls = createReport(tieuDe);
            return Print(xls, ext);
        }


        private ExcelFile createReport(string tieuDe = null)
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var dvt = _dvt.ToStringDvt();
            fr.SetValue("dvt", dvt);
            fr.SetValue("TieuDe1", tieuDe ?? getTieuDe());

            var tenPhongBan = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa;
            fr.SetValue("TieuDe2", tenPhongBan);

            fr.SetValue("header1", tenPhongBan);
            fr.SetValue("header2", $"Đơn vị tính: {dvt}  Tờ: {_to}");
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls);

            return xls;
        }


        private void loadData(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(), () => getTable(), CacheTimes.OneMinute);

            var data = dt.SelectDistinct("ChiTiet", "Code1,Code2,Code3,Code");
            var dtX = dt.SelectDistinct("X", "Id_DonVi,TenDonVi");

            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
                data.Columns.Add(new DataColumn($"C{i + 1}2", typeof(decimal)));
                data.Columns.Add(new DataColumn($"C{i + 1}3", typeof(decimal)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["TenDonVi"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(r, 0);
            }


            columns = _to == 1 ?
                dtX.AsEnumerable().Skip(0).Take(_columnCount - 1).ToList() :
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
                    fr.SetValue(colName1, "BQL");
                    fr.SetValue(colName2, "B2");
                    fr.SetValue(colName3, "Cục");

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           // BQL
                           var value = dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code") &&
                                                x.Field<int>("iRequest") == 0)
                                   .Sum(x => x.Field<double>("TuChi", 0));
                           r[colName] = value;

                           // B2
                           value = dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code") &&
                                                x.Field<int>("iRequest") == 1)
                                   .Sum(x => x.Field<double>("TuChi", 0));

                           r[colName2] = value;

                           // Cuc
                           value = dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code") &&
                                                x.Field<int>("iRequest") == 3)
                                   .Sum(x => x.Field<double>("TuChi", 0));

                           r[colName3] = value;
                       });
                }
                else
                {
                    fr.SetValue($"C{i + 1}", "");
                    fr.SetValue(colName1, "");
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
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m01.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", _nam);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@Id_DonVi", PhienLamViec.DonViList.Select(x => x.Key).Join());


                var dt = cmd.GetTable();
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

        public JsonResult Ds_To(string id_PhongBan)
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
            return $"{this.ControllerName()}_{Username}_{_nam}_{_id_phongban}_{_dvt}";
        }

        public DataTable GetMucLucAll(int nam)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_mucluc.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.ExecuteDataTable(sql, new { nam }, CommandType.Text);
                return dt;
            }
            #endregion   
        }

        #endregion
    }
}
