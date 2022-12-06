using DapperExtensions;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
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
    public class rptDuToanKT_M03ViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }

        public string TieuDe { get; set; }

        public SelectList RequestList { get; set; }

        public ChecklistModel DonViList { get; set; }

        public int Request { get; set; }
    }
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M03Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M03a.xls";
        private const string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M03a_trinhky.xls";
        private const string _filePath_trinhky2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M03a_trinhky2.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_donvi;
        private string _id_phongban;
        private string _id_chungtu;
        private int _dvt = 1000;
        private int _request;
        private int _trinhky = 0;
        private int _loaiBaoCao;
        private bool _isEmpty;


        public ActionResult Index()
        {
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));


            var requestRole = 0;
            var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, getParamUsername(), GetPhongBanId(PhienLamViec.iID_MaPhongBan), requestRole);
            var vm = new rptDuToanKT_M03ViewModel
            {
                iNamLamViec = iNamLamViec,
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Tất cả phòng ban --"),
                DonViList = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten")),
                RequestList = _duToanKTService.GetRequestList().ToSelectList(),
                Request = requestRole,
                TieuDe = getTieuDe(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }


        #endregion

        #region public methods

        public ActionResult Print(
            string Id_DonVi,
            string Id_PhongBan,
            string Id_ChungTu,
            int request = 0,
            int loaiBaoCao = 0,
            string tieuDe = "",
            string ext = "pdf")
        {
            // neu in theo 1 chung tu
            Guid id_chungtu = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(Id_ChungTu) && Guid.TryParse(Id_ChungTu, out id_chungtu))
            {
                _id_chungtu = Id_ChungTu;

                using (var conn = _connectionFactory.GetConnection())
                {
                    var entity = conn.Get<DTKT_ChungTu>(id_chungtu);
                    _id_phongban = entity.Id_PhongBanDich;
                    _id_donvi = entity.Id_DonVi;
                    _request = entity.iRequest;
                }
            }
            else
            {
                _id_phongban = GetPhongBanId(Id_PhongBan);
                _id_donvi = Id_DonVi;
                _request = request;
            }

            _loaiBaoCao = loaiBaoCao;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport(tieuDe);
            if (_isEmpty)
            {
                return PrintEmpty($"Không có số liệu của: {Environment.NewLine}- Phòng ban:{Id_PhongBan} {Environment.NewLine}- Đơn vị: {Id_DonVi}{Environment.NewLine}- Loại báo cáo: {loaiBaoCao}");
            }

            return Print(xls, ext);
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport(string tieuDe = null)
        {
            var fr = new FlexCelReport();
            if (_loaiBaoCao == 0)
            {
                LoadData(fr);
            }
            else
            {
                LoadData_TongHop(fr);
            }

            var xls = new XlsFile(true);
            xls.Open(getFileXls());

            if (!_isEmpty)
            {
                var tenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        TieuDe1 = string.IsNullOrWhiteSpace(tieuDe) ? getTieuDe() : tieuDe,
                        TieuDe2 = _loaiBaoCao != 0 ? string.Empty : (_id_phongban.StartsWith("06") ? "Khối Doanh nghiệp" : "Khối Dự toán"),
                        DonVi = tenDonVi,
                        TenPhongBan = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                        header1 = tenDonVi,
                        header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                    })
                  .UseChuKy(Username)
                  .UseChuKyForController(this.ControllerName())
                  .UseForm(this)
                  .Run(xls);
            }

            return xls;
        }

        private string getFileXls()
        {
            var file = string.Empty;
            if (_loaiBaoCao == 0)
            {
                file = _filePath;
            }
            else if (_loaiBaoCao == 3)
            {
                file = _filePath_trinhky2;
            }
            else
                file = _filePath_trinhky;

            //file = _loaiBaoCao == 0 || _loaiBaoCao == 3 ? _filePath : _filePath_trinhky;

            return Server.MapPath(file);
        }
        private void LoadData(FlexCelReport fr)
        {
            var data = getTable();
            if (data == null || data.Rows.Count == 0)
            {
                _isEmpty = true;
                return;
            }

            // fill ghichu
            var ghichu = rptGetData_Ghichu();
            data.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var note = ghichu.AsEnumerable().Where(x => x.Field<string>("Code") == r.Field<string>("Code")).Select(x => x.Field<string>("GhiChu")).Join("; ");
                    r["GhiChu"] = note;
                });


            var sum = _dvt * data.AsEnumerable().Sum(r => r.Field<double>("TuChi", 0) +
                        (_loaiBaoCao == 0 ? 0 : r.Field<double>("DacThu", 0)));

            fr.SetValue("Tien", sum.ToStringMoney());
            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", PhienLamViec.NamLamViec.ToString());

        }

        private void LoadData_TongHop(FlexCelReport fr)
        {
            var data = getTable();
            var dtPhanCap = getTable_PhanCap();

            data.Columns.Add("TuChi1", typeof(double));

            // fill ghichu
            var ghichu = rptGetData_Ghichu();
            data.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    // ghi chu
                    var note = ghichu.AsEnumerable().Where(x => x.Field<string>("Code") == r.Field<string>("Code")).Select(x => x.Field<string>("GhiChu")).Join("; ");
                    r["GhiChu"] = note;

                    // tu chi - phan cap dau nam
                    var value = dtPhanCap.AsEnumerable().Where(x => x.Field<string>("Nganh") == r.Field<string>("Nganh")).Sum(x => (double)x.Field<decimal>("TuChi"));
                    r["TuChi1"] = value;

                });


            var sum = _dvt * data.AsEnumerable().Sum(r => r.Field<double>("TuChi", 0) + r.Field<double>("DacThu", 0));

            fr.SetValue("Tien", sum.ToStringMoney());
            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", PhienLamViec.iNamLamViec);
        }


        public DataTable getTable()
        {
            var sql = _loaiBaoCao == 0 ? FileHelpers.GetSqlQuery("dtkt_report_m03.sql") : FileHelpers.GetSqlQuery("dtkt_report_m03_th.sql");


            //var donvis_b6 = "30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94".ToList();
            //if ((string.IsNullOrWhiteSpace(_id_phongban) || _id_phongban == "-1") &&
            //    donvis_b6.Contains(_id_donvi))
            //{
            //    if (_id_phongban == "06")
            //    {
            //        _id_phongban = "06,07,08,10";
            //    }
            //    else
            //    {
            //        _id_phongban = "9999";
            //    }
            //}

            #region get data

            var username = _loaiBaoCao == 1 ? Username : getParamUsername();

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", _id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@Id_ChungTu", _id_chungtu.ToParamString());
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@request", _request);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@username", getParamUsername().ToParamString());

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }


        /// <summary>
        /// Lấy số phân cấp đầu năm
        /// </summary>
        /// <returns></returns>
        public DataTable getTable_PhanCap()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m03_phancap.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", _id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", _dvt);

                var dt = conn.GetTable(sql,
                    new
                    {
                        id_phongban = _id_phongban.ToParamString(),
                        id_donvi = _id_donvi,
                        nam = PhienLamViec.iNamLamViec,
                        nganh = _nganSachService.Nganh_GetAll_ChuyenNganh(Username, _id_phongban),
                        dvt = _dvt,
                    });
                return dt;
            }
            #endregion
        }
        public DataTable rptGetData_Ghichu()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m03_ghichu.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", _id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@Id_ChungTu", _id_chungtu.ToParamString());
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@request", _request);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@username", getParamUsername().ToParamString());

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

        public JsonResult Ds_DonVi(string id_PhongBan, int request)
        {
            var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, getParamUsername(), GetPhongBanId(id_PhongBan), request);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten"));
            return ToCheckboxList(vm);
        }
        private string getParamUsername()
        {
            var username = Username;
            if (_nganSachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyPhongBan ||
                string.IsNullOrWhiteSpace(GetPhongBanId(PhienLamViec.iID_MaPhongBan)))
            {
                username = "";
            }
            return username;
        }


        #endregion
    }
}
