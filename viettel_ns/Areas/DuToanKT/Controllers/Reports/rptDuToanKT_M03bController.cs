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
    public class rptDuToanKT_M03bViewModel
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
    public class rptDuToanKT_M03bController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M03b.xls";
        private const string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M03a_trinhky.xls";

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
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
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
            _id_phongban = GetPhongBanId(Id_PhongBan);
            _id_donvi = Id_DonVi;
            _request = request;

            _loaiBaoCao = loaiBaoCao;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport(tieuDe);
            if (_isEmpty)
            {
                return PrintEmpty($"Không có số liệu của: {Environment.NewLine}- Phòng ban:{Id_PhongBan} {Environment.NewLine}- Đơn vị: {Id_DonVi}{Environment.NewLine}- Loại báo cáo: {loaiBaoCao}");
            }
            //return new EmptyResult();
            return Print(xls, ext);
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport(string tieuDe = null)
        {
            var fr = new FlexCelReport();
            LoadData(fr);

            var xls = new XlsFile(true);
            xls.Open(getFileXls());

            if (!_isEmpty)
            {
                var tenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        TieuDe1 = string.IsNullOrWhiteSpace(tieuDe) ? getTieuDe() : tieuDe,
                        TieuDe2 = _loaiBaoCao != 0 ? string.Empty : (_id_phongban == "06" ? "Doanh nghiệp" : ""),
                        DonVi = tenDonVi,
                        TenPhongBan = _nganSachService.CheckParam_PhongBan(_id_phongban).IsEmpty() ?
                            "" :
                            _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
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

            file = _loaiBaoCao == 0 ? _filePath : _filePath_trinhky;

            return Server.MapPath(file);
        }
        private void LoadData(FlexCelReport fr)
        {
            var dt = getTable();
            if (dt == null || dt.Rows.Count == 0)
            {
                _isEmpty = true;
                return;
            }

            #region fill tables

            var data = dt.SelectDistinct("ChiTiet", "Code1,Code2,Code3,Code4,Code");
            var phongbans = "02,06,07,08,10".ToList();
            phongbans.ToList().ForEach(x =>
            {
                data.Columns.Add($"B{x}", typeof(double));
            });

            data.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    phongbans.ToList().ForEach(b =>
                    {
                        var columnName = $"B{b}";

                        var value = dt.AsEnumerable().Where(x => x.Field<string>("Code") == r.Field<string>("Code") && x.Field<string>("Id_PhongBanDich") == b).Sum(x => x.Field<double>("TuChi"));
                        r[columnName] = value;
                    });
                });

            #endregion

            var sum = _dvt * dt.AsEnumerable().Sum(r => r.Field<double>("TuChi", 0) +
                        (_loaiBaoCao == 0 ? 0 : r.Field<double>("DacThu", 0)));

            fr.SetValue("Tien", sum.ToStringMoney());
            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", PhienLamViec.iNamLamViec);

        }

        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m03b.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", _id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@request", _request);
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", _dvt);

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
