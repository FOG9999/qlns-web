using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_K11_M104bController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToan/KiemTra/rptDuToan_K11_M104b.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_nganh;
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
            var nganhList = _nganSachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username);


            var vm = new rptDuToan_K11_M104ViewModel
            {
                iNamLamViec = iNamLamViec,
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Tất cả phòng ban --"),
                NganhList = nganhList.ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Tất cả --"),
                TieuDe = getTieuDe(),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }


        #endregion

        #region public methods

        public ActionResult Print(
            string id_nganh,
            string id_phongban,
            int request = 1,
            int loaiBaoCao = 0,
            string tieuDe = "",
            string ext = "pdf")
        {

            _id_phongban = GetPhongBanId(id_phongban);
            _id_nganh = id_nganh;
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
            LoadData(fr);


            var xls = new XlsFile(true);
            xls.Open(getFileXls());

            if (!_isEmpty)
            {

                var nganh = _id_nganh == "-1" ?
                    "Các ngành bảo đảm" :
                    _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        TieuDe1 = string.IsNullOrWhiteSpace(tieuDe) ? getTieuDe() : tieuDe,
                        TieuDe2 = nganh,
                        TenPhongBan = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                        header1 = nganh,
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
            data.Columns.Add(new DataColumn("TuChi2", typeof(decimal)));
            data.Columns.Add(new DataColumn("HangMua2", typeof(decimal)));
            data.Columns.Add(new DataColumn("HangNhap2", typeof(decimal)));
            data.Columns.Add(new DataColumn("DacThu2", typeof(decimal)));
            data.Columns.Add(new DataColumn("PhanCap2a", typeof(decimal)));
            data.Columns.Add(new DataColumn("PhanCap2b", typeof(decimal)));

            // fill ghichu
            var dtDuToan = getTable_DuToan();
            var dtTemp = data.AsEnumerable().ToList();
            var _nganhNghiepVuIndex = "";
            dtTemp.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));

                    // nganh 00
                    var nganhNghiepVu = r.Field<string>("Nganh");
                    if (nganhNghiepVu == "00")
                    {

                    }
                    else
                    {
                        if (_nganhNghiepVuIndex != nganhNghiepVu)
                        {
                            row["TuChi2"] = dtDuToan.AsEnumerable().Where(x => x.Field<string>("sNG") == nganhNghiepVu).Sum(x => x.Field<decimal>("rTuChi"));
                            row["HangNhap2"] = dtDuToan.AsEnumerable().Where(x => x.Field<string>("sNG") == nganhNghiepVu).Sum(x => x.Field<decimal>("rHangNhap"));
                            row["HangMua2"] = dtDuToan.AsEnumerable().Where(x => x.Field<string>("sNG") == nganhNghiepVu).Sum(x => x.Field<decimal>("rHangMua"));
                            row["PhanCap2a"] = dtDuToan.AsEnumerable().Where(x => x.Field<string>("sNG") == nganhNghiepVu).Sum(x => x.Field<decimal>("rPhanCap2a"));
                            row["PhanCap2b"] = dtDuToan.AsEnumerable().Where(x => x.Field<string>("sNG") == nganhNghiepVu).Sum(x => x.Field<decimal>("rPhanCap2b"));
                        }
                        _nganhNghiepVuIndex = nganhNghiepVu;
                    }
                });

            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", namDTKT());
        }


        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_K11_M104b_dtkt.sql");

            #region get data
            var id_nganh = _id_nganh;
            if (_id_nganh != "-1")
                id_nganh = _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).iID_MaNganhMLNS;

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@id_nganh", id_nganh.ToParamString());
                cmd.Parameters.AddWithValue("@NamLamViec", namDTKT());
                cmd.Parameters.AddWithValue("@dvt", _dvt);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }


        public DataTable getTable_DuToan()
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_K11_M104b_dt.sql");

            #region get data

            var id_nganh = _id_nganh;
            if (_id_nganh != "-1")
                id_nganh = _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).iID_MaNganhMLNS;

            var id_donvi = Request.GetQueryString("id_donvi", PhienLamViec.iID_MaDonVi);

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@id_nganh", id_nganh.ToParamString());
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@id_donvi", id_donvi);
                cmd.Parameters.AddWithValue("@dvt", _dvt);


                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }

        private string getTieuDe()
        {
            var nam = int.Parse(PhienLamViec.iNamLamViec);
            var tieude = $"So sánh số kiểm tra và số dự toán ngân sách năm {nam}";

            return tieude;
        }

        private string namDTKT()
        {
            return (PhienLamViec.NamLamViec - 1).ToString();
        }

        //private string getParamUsername()
        //{
        //    var username = Username;
        //    if (_nganSachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyPhongBan ||
        //        string.IsNullOrWhiteSpace(GetPhongBanId(PhienLamViec.iID_MaPhongBan)))
        //    {
        //        username = "";
        //    }
        //    return username;
        //}


        #endregion
    }
}
