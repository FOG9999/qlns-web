using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.DuToan.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Models
{
    public class rptDuToan_K11_NGViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }

        public string TieuDe { get; set; }

        public ChecklistModel DonViList { get; set; }
    }
}

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_K11_NGController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToan/KiemTra/rptDuToan_K11_NG.xls";
        private const string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M03a_trinhky.xls";
        private const string _filePath_trinhky2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M03a_trinhky2.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_donvi;
        private string _id_phongban;
        private int _dvt = 1000;
        private int _trinhky = 0;
        private bool _isEmpty;


        public ActionResult Index()
        {
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptDuToan_K11_NGViewModel
            {
                iNamLamViec = iNamLamViec,
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Tất cả phòng ban --"),
                //DonViList = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten")),
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
            string tieuDe = "",
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(Id_PhongBan);
            _id_donvi = Id_DonVi;
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
            file = _filePath;

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

            //var sum = _dvt * data.AsEnumerable().Sum(r => r.Field<double>("TuChi", 0));
            //fr.SetValue("Tien", sum.ToStringMoney());
            //_duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", PhienLamViec.iNamLamViec);

            fr.AddTable("ChiTiet", data);
        }



        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_K11_NG.sql");
            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", _id_donvi.ToParamString());
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

        public JsonResult Ds_DonVi(string id_phongban)
        {
            //var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, getParamUsername(), GetPhongBanId(id_PhongBan), request);
            //var data = _nganSachService.GetDonviByPhongBanId(PhienLamViec.iNamLamViec, id_PhongBan);
            var data = getDuToan_Donvis(id_phongban);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("iID_MaDonVi", "sMoTa"));
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

        public DataTable getDuToan_Donvis(string id_phongban)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_donvis.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = GetPhongBanId(id_phongban).ToParamString(),
                });

                return dt;
            }
        }


        #endregion
    }
}
