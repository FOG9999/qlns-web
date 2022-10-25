using Dapper;
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

namespace VIETTEL.Models
{
    public class rptDuToanKT_M10ViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList NganhList { get; set; }

        public string TieuDe { get; set; }

    }
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M10Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M10.xls";
        private string _filePath_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M10_to2.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 6;

        public ActionResult Index()
        {
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongBanList = _nganSachService.GetBql(Username);


            var vm = new rptDuToanKT_M10ViewModel
            {
                iNamLamViec = iNamLamViec,
                NganhList = _nganSachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username).ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Chọn ngành --"),
                TieuDe = getTieuDe(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            string id_nganh,
            int to,
            string tieuDe,
            string ext = "pdf")
        {
            _id_phongban = id_phongban;
            _id_nganh = id_nganh;
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
            fr.SetValue("TenPhongBan", tenPhongBan);

            fr.SetValue("header1", getTenChuyenNganh(PhienLamViec.iNamLamViec.ToValue<int>(), _id_nganh));
            fr.SetValue("header2", $"Đơn vị tính: {dvt}  Tờ: {_to}");
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls);

            return xls;
        }

        public JsonResult Ds_To(string id_phongban, string id_nganh)
        {
            //var data = getTable_Nganh(PhienLamViec.iNamLamViec.ToValue<int>(), id_phongban, id_nganh);

            var dt = getTable(PhienLamViec.iNamLamViec.ToValue<int>(), id_phongban, id_nganh, _dvt);
            var data = dt.SelectDistinct("", "NG");
            var count = data.Rows.Count;
            return ds_ToIn(count > 0 ? count + 1 : 0, _columnCount);
        }

        public JsonResult Ds_Nganh(string id_nganh)
        {
            var data = getTable_ChuyenNganh(PhienLamViec.iNamLamViec.ToValue<int>(), id_nganh).ToSelectList("sNG", "sMoTa", "-1", "-- chọn chuyên ngành --");
            return ToDropdownList(new ChecklistModel("id_chuyennganh", data));
        }

        private void loadData(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(), () => getTable(), CacheTimes.OneMinute);

            #region tu chi tai nganh 

            var dtNganhTuChi = new DataTable("Nganh");
            for (int i = 1; i <= _columnCount; i++)
            {
                dtNganhTuChi.Columns.Add(new DataColumn($"C{i}", typeof(double)));
            }
            var tuChiRow = dtNganhTuChi.NewRow();
            dtNganhTuChi.Rows.InsertAt(tuChiRow, 0);

            #endregion

            #region khoi don vi

            var dtKhoi = dt.SelectDistinct("Khoi", "iSTT_MaKhoiDonVi,iID_MaKhoiDonVi,TenKhoiDonVi");
            dtKhoi.Columns.Add("stt");
            for (int i = 0; i < dtKhoi.Rows.Count; i++)
            {
                dtKhoi.Rows[i]["stt"] = (i + 1).ToStringRoman();
            }
            #endregion

            var data = dt.SelectDistinct("ChiTiet", "iID_MaDonVi,TenDonVi,iID_MaKhoiDonVi");
            var dtX = dt.SelectDistinct("X", "NG");
            var dtX1 = dt.SelectDistinct("X", "NG,sMoTa");
            dtX.Columns.Add("sMoTa");
            dtX.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var row = dtX1.AsEnumerable().FirstOrDefault(x => x.Field<string>("NG") == r.Field<string>("NG"));
                    if (row != null)
                    {
                        r["sMoTa"] = row["sMoTa"];
                    }
                });



            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["sMoTa"] = "Tổng cộng".ToUpper();
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
                if (i < columns.Count)
                {
                    #region cells
                    var row = columns[i];
                    fr.SetValue(colName1, row.Field<string>("NG"));
                    fr.SetValue(colName2, row.Field<string>("sMoTa"));

                    // phan cap
                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           // BQL
                           var value = dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (x.Field<string>("sLNS").StartsWith("102")) &&
                                                (string.IsNullOrWhiteSpace(row.Field<string>("NG")) || x.Field<string>("NG") == row.Field<string>("NG")) &&
                                                (x.Field<string>("iID_MaDonVi") == r.Field<string>("iID_MaDonVi")))
                                   .Sum(x => x.Field<decimal>("rTuChi", 0));
                           r[colName] = value;
                       });

                    // tu chi tai nganh
                    dtNganhTuChi.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           // BQL
                           var value = dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (x.Field<string>("sLNS").StartsWith("104")) &&
                                                (string.IsNullOrWhiteSpace(row.Field<string>("NG")) || x.Field<string>("NG") == row.Field<string>("NG")))
                                   .Sum(x => x.Field<decimal>("rTuChi", 0));
                           r[colName] = value;
                       });

                    #endregion
                }
                else
                {
                    fr.SetValue(colName1, "");
                    fr.SetValue(colName2, "");
                }
            }


            fr.AddTable(dtNganhTuChi);
            fr.AddTable(dtKhoi);
            fr.AddTable(data);
        }

        private DataTable getTable()
        {
            return getTable(_nam, _id_phongban, _id_nganh, _dvt);
        }

        private DataTable getTable(int _nam, string _id_phongban, string _id_nganh, int _dvt)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m10.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        @iID_MaPhongBan = _id_phongban.ToParamString(),
                        @iNamLamViec = _nam,
                        @sNG = _id_nganh,
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

        private string getCacheKey()
        {
            return $"{this.ControllerName()}_{Username}_{_nam}_{_id_phongban}_{_id_nganh}_{_dvt}";
        }

        private DataTable getTable_Nganh(int nam, string id_phongban, string id_nganh)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m10_nganh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                     new
                     {
                         nam,
                         id_phongban,
                         id_nganh,
                         lns = DBNull.Value,
                     });
                return dt;
            }
        }

        private DataTable getTable_ChuyenNganh(int nam, string id_nganh)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m10_chuyennganh.sql");
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new { nam, id_nganh });
                var dt = cmd.GetTable();
                return dt;
            }
        }

        private string getTenChuyenNganh(int nam, string id_nganh)
        {
            var sql = @"
select sMoTa from NS_MucLucNganSach
where   iTrangThai=1
        and iNamLamViec=@nam
        and sLNS=''
        and sNG=@id_nganh

";
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = conn.QueryFirstOrDefault<string>(
                    sql,
                    param: new { nam, id_nganh },
                    commandType: CommandType.Text);
                return result;
            }
        }


        #endregion
    }
}
