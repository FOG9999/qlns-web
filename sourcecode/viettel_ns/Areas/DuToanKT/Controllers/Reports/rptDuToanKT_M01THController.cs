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
    public class rptDuToanKT_M01THController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01.xls";
        private string _filePath_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01_to2.xls";

        private string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01_TrinhKy.xls";

        private string _filePath_a4 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01_a4.xls";
        private string _filePath_a4_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M01_a4_to2.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private string _id_phongban;
        private int _to = 1;
        private int _page;
        private int _loaiBaoCao;
        private int _request;
        private int _nam;
        private int _dvt = 1000;
        //private int _columnCount = 10;
        private int _columnCount
        {
            get
            {
                if (_page == 4)
                {
                    //return _to == 1 ? 6 : 8;
                    return 8;
                }
                else
                {
                    //return _to == 1 ? 10 : 12;
                    return 10;
                }
            }
        }

        public ActionResult Index()
        {
            var phongBanList = _ngansachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptDuToanKT_M01ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                RequestList = _duToanKTService.GetRequestList().ToSelectList(),
                TieuDe = getTieuDe(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string Id_PhongBan,
            int request,
            int loaiBaoCao,
            int page,
            int to,
            int dvt,
            string tieuDe,
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(Id_PhongBan);
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;
            _page = page;
            _dvt = dvt;
            _request = request;
            _loaiBaoCao = loaiBaoCao;

            if (_page == 4)
            {
                _filePath = _to == 1 ? _filePath_a4 : _filePath_a4_to2;
            }
            else if (_page == 31)
            {
                _filePath = _to == 1 ? _filePath : _filePath_to2;
            }
            else if (_page == 32)
            {
                _filePath = _to == 1 ? _filePath_trinhky : _filePath_to2;
            }

            var xls = createReport(tieuDe);
            return Print(xls, ext);
        }


        private ExcelFile createReport(string tieuDe = null)
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var TenPhongBan = _id_phongban == "-1" ? "" : _ngansachService.GetPhongBanById(_id_phongban).sMoTa;
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            var tieuDe2 = string.Empty;
            if (_loaiBaoCao == 0)
                tieuDe2 = "(Nghiệp vụ 00)";
            else if (_loaiBaoCao == 1)
            {
                tieuDe2 = "(Các ngành bảo đảm)";
            }
            else if (_loaiBaoCao == 2)
            {
                tieuDe2 = "(Tổng hợp toàn quân)";
            }

            fr.UseCommonValue()
                .SetValue(new
                {
                    TieuDe1 = tieuDe ?? getTieuDe(),
                    TieuDe2 = tieuDe2,
                    TenPhongBan,
                    QuyetDinh = L("DTKT_QuyetDinh"),
                    i = (_to - 1) * _columnCount,
                    header1 = tieuDe2,
                    header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}  Tờ: {_to}",
                })
               .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls, _to);

            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var dt = getTable();

            var data = dt.SelectDistinct("ChiTiet", "Code1,Code2,Code3,Code4,Code,sMoTa");
            var dtX = dt.SelectDistinct("X", "Id_DonVi,TenDonVi");

            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["TenDonVi"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(r, 0);
            }

            columns = _to == 1 ?
                dtX.AsEnumerable().Take(_columnCount).ToList() :
                dtX.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colName1 = $"C{i + 1}_1";
                var colIndex = $"i{i + 1}";

                if (i < columns.Count)
                {

                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("TenDonVi"));
                    fr.SetValue(colName1, "Số kiểm tra");


                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           // BQL
                           var value = dt.AsEnumerable()
                                   .ToList()
                                   .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                x.Field<string>("Code") == r.Field<string>("Code"))
                                   .Sum(x => x.Field<double>("TuChi", 0));
                           r[colName] = value;
                       });

                    fr.SetValue(colIndex, _to == 1 ? i + 1 : ((_to - 1) * _columnCount) + i);
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colName1, "");
                    fr.SetValue(colIndex, "");
                }
            }

            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", PhienLamViec.iNamLamViec);
        }

        public DataTable getTable()
        {
            var sql = _loaiBaoCao == 0 || _loaiBaoCao == 2 ?
                FileHelpers.GetSqlQuery("dtkt_report_m01_th_00.sql") :
                FileHelpers.GetSqlQuery("dtkt_report_m01_th.sql");

            var nganh = string.Empty;
            if (_loaiBaoCao == 0)
                nganh = "00";
            else if (_loaiBaoCao == 1)
            {
                nganh = "!00";
            }

            var code = _page == 31 ? "1" : "0";

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        id_phongban = _id_phongban.ToParamString(),
                        id_donvi = PhienLamViec.DonViList.Select(x => x.Key).Join(),
                        nganh = nganh.ToParamString(),
                        request = _request,

                        // phan biet: phu luc thi bo Du phong tang luong
                        code = code.ToParamString(),

                        nam = _nam,
                        dvt = _dvt,
                    });
                return dt;
            }
            #endregion
        }

        public DataTable GetDonVis(string nam, string id_phongban = null, string nganh = null)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m01_th_donvi.sql");
            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@Nganh", nganh.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", PhienLamViec.DonViList.Select(x => x.Key).Join());

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }

        //private DataTable getTable()
        //{
        //    return getTable(PhienLamViec.iNamLamViec, _id_phongban, _id_nganh, _loaiBaoCao, Username);
        //}

        //private DataTable getTable(string nam, string id_phongban, string id_nganh, int loaiBaoCao, string username)
        //{
        //    string id_donvi = PhienLamViec.iID_MaDonVi;
        //    username = _ngansachService.CheckParam_Username(username, id_phongban);

        //    var sql = FileHelpers.GetSqlQuery("dtkt_report_m01_ng.sql");
        //    using (var conn = _connectionFactory.GetConnection())
        //    {
        //        var nganh = id_nganh == "-1" ? "-1" : _ngansachService.Nganh_Get(id_nganh).iID_MaNganhMLNS;

        //        var dt = conn.GetTable(sql,
        //            new
        //            {
        //                nam,
        //                id_phongban = id_phongban.ToParamString(),
        //                username = username.ToParamString(),
        //                nganh = nganh.ToParamString(),
        //                id_donvi = id_donvi.ToParamString(),
        //                loai = loaiBaoCao,
        //                dvt = _dvt,
        //            });

        //        return dt;
        //    }
        //}

        private string getTieuDe()
        {
            var nam = int.Parse(PhienLamViec.iNamLamViec) + 1;
            //var tieude = $"Báo cáo tổng hợp số kiểm tra dự toán ngân sách năm {nam}";

            var tieude = L("DTKT_TieuDe1", nam);

            return tieude;
        }

        public JsonResult Ds_To(string id_PhongBan, int loaiBaoCao, int page)
        {
            _page = page;

            var nganh = string.Empty;
            if (loaiBaoCao == 0)
                nganh = "00";
            else if (_loaiBaoCao == 1)
            {
                nganh = "!00";
            }
            var data = GetDonVis(PhienLamViec.iNamLamViec, id_PhongBan, nganh);

            var count = data.Rows.Count;
            return ds_ToIn(count > 0 ? count + 2 : 0, _columnCount);
        }
        private string getParamUsername()
        {
            var username = Username;
            if (_ngansachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyPhongBan)
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
