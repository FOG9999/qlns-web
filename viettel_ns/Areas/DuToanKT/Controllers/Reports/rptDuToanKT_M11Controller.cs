using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
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
    public class rptDuToanKT_M11ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList RequestList { get; set; }

        public string TieuDe { get; set; }
    }
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M11Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private const string _filePath_a = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M11a.xls";
        private const string _filePath_a2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M11a2.xls";
        private const string _filePath_a2_bql = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M11a2_bql.xls";
        private const string _filePath_a2_cuc = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M11a2_cuc.xls";

        private const string _filePath_b = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M11b.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_donvi;
        private string _id_phongban;
        private int _dvt = 1000;
        private int _request = 0;
        private int _loaiBaoCao = 0;


        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptDuToanKT_M11ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                TieuDe = getTieuDe(),
                RequestList = _duToanKTService.GetRequestList().ToSelectList(group: "request", selectedValues: "0"),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }


        #endregion

        #region public methods

        public ActionResult Print(
            string id_donvi,
            string id_phongban,
            int loaiBaoCao = 0,
            int request = 0,
            string tieuDe = "",
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(id_phongban);
            _id_donvi = id_donvi;
            _request = request;
            _loaiBaoCao = loaiBaoCao;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport(tieuDe);
            return Print(xls, ext);
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport(string tieuDe = null)
        {
            var fr = new FlexCelReport();
            loadData(fr);


            var xls = new XlsFile(true);
            xls.Open(getFileXls());

            var tenPhongBan = string.Empty;
            if (!string.IsNullOrWhiteSpace(_nganSachService.CheckParam_PhongBan(_id_phongban)))
            {
                tenPhongBan = _nganSachService.GetPhongBanById(_id_phongban).sMoTa;
            }

            fr.UseCommonValue()
                .SetValue(new
                {
                    Request = _duToanKTService.GetRequestList()[_request.ToString()],
                    TieuDe1 = tieuDe ?? getTieuDe(),
                    TieuDe2 = "Tự chi tại ngành",
                    header1 = tenPhongBan,
                    header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            return xls;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        private void loadData(FlexCelReport fr)
        {
            var data = getTable();
            data.TableName = "ChiTiet";
            fr.AddTable(data);
        }

        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m11a.sql");

            #region get data

            var nganh = _nganSachService.CheckParam_PhongBan(_id_phongban).IsEmpty() ?
                "-1" :
                _nganSachService.Nganh_GetAll(_nganSachService.CheckParam_Username(Username, PhienLamViec.iID_MaPhongBan), _id_phongban)
                    .AsEnumerable()
                    .Select(r => r.Field<string>("iID_MaNganhMLNS"))
                    .JoinDistinct();

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        id_phongban = _id_phongban.ToParamString(),
                        id_donvi = _id_donvi.ToParamString(),
                        nam = PhienLamViec.iNamLamViec,
                        nganh = nganh.ToParamString(),
                        request = _request,
                        dvt = _dvt,

                    });
                return dt;
            }
            #endregion
        }

        private string getFileXls()
        {
            var file = string.Empty;

            if (_loaiBaoCao == 0)
            {
                file = _filePath_a2;
            }
            else if (_loaiBaoCao == 1)
            {
                file = _filePath_a2_bql;
            }
            else if (_loaiBaoCao == 2)
            {
                file = _filePath_a2_cuc;
            }

            return Server.MapPath(file);
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
