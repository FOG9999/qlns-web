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
    public class rptDuToan_K11_M00ViewModel
    {
        public string iNamLamViec { get; set; }

        public SelectList PhongBanList { get; set; }

        public string TieuDe { get; set; }

        public ChecklistModel DonViList { get; set; }
    }
}

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_K11_M00Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToan/KiemTra/rptDuToan_K11_M00.xls";

        //private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
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
            //var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, getParamUsername(), GetPhongBanId(PhienLamViec.iID_MaPhongBan), requestRole);
            var vm = new rptDuToan_K11_M00ViewModel
            {
                iNamLamViec = iNamLamViec,
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                DonViList = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten")),
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
            int request = 1,
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

            data.Columns.Add(new DataColumn("TuChi2", typeof(decimal)));


            // fill ghichu
            var dtDuToan = getTable_DuToan();
            var dtTemp = data.AsEnumerable().ToList();
            var _nganhNghiepVuIndex = "";
            dtTemp.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var xaunoima = r.Field<string>("XauNoiMa");

                    // nganh 00
                    var nganhNghiepVu = r.Field<string>("Nganh");
                    if (nganhNghiepVu == "00")
                    {
                        if (!string.IsNullOrWhiteSpace(xaunoima))
                        {
                            var xaunoima_list = xaunoima.ToList();
                            var value = dtDuToan.AsEnumerable()
                                    .Where(x => r["XauNoiMa"] != DBNull.Value && xaunoima_list.Any(s => x.Field<string>("sXauNoiMa").Contains(s)))
                                    .Sum(x => x.Field<decimal>("rTuChi"));

                            var value2 = dtDuToan.AsEnumerable()
                                    .Where(x => r["XauNoiMa_x"] != DBNull.Value && x.Field<string>("sXauNoiMa").Contains(r.Field<string>("XauNoiMa_x")))
                                    .Sum(x => x.Field<decimal>("rTuChi"));
                            //r["TuChi2"] = value - value2;

                            var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));
                            if (row != null)
                            {
                                row["TuChi2"] = value - value2;
                            }
                        }
                    }
                    else
                    {
                        if (_nganhNghiepVuIndex != nganhNghiepVu)
                        {
                            var sumNganh = dtDuToan.AsEnumerable()
                                    .Where(x => x.Field<string>("sNG") == nganhNghiepVu && x.Field<string>("sLNS") == "1020100")
                                    .Sum(x => x.Field<decimal>("rTuChi"));
                            var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));

                            var nganhCount = data.AsEnumerable().Count(x => x.Field<string>("Nganh") == nganhNghiepVu);
                            // chen them hang cho nganh nghiep vu, hang cuoi cung cua tung nganh
                            if (nganhCount > 1)
                            {
                                var newrow = data.NewRow();
                                newrow.Field<string>("Code");
                                newrow.ItemArray = r.ItemArray.Clone() as object[];
                                // ko can mo ta, de giau hang
                                //newrow["MoTa"] = string.Empty;
                                newrow["TuChi"] = 0;
                                newrow["TuChi2"] = sumNganh;
                                newrow["id"] = "00";


                                //var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));
                                var currentRowIndex = data.Rows.IndexOf(row);
                                data.Rows.InsertAt(newrow, currentRowIndex);
                            }
                            else
                            {
                                row["TuChi2"] = sumNganh;
                            }

                        }
                        _nganhNghiepVuIndex = nganhNghiepVu;
                    }
                });

            var sum = _dvt * data.AsEnumerable().Sum(r => r.Field<double>("TuChi", 0) +
                        (_loaiBaoCao == 0 ? 0 : r.Field<double>("DacThu", 0)));

            fr.SetValue("Tien", sum.ToStringMoney());
            _duToanKTService.FillDataTable(fr, data, "Code1 Code2 Code3 Code4", namDTKT());


            // khong hien thi mo ta cua hang them, cua nganh nghiep vu
            data.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    if (r.Field<string>("id") == "00")
                    {
                        r["sMoTa"] = "(dự toán)";
                    }
                });

        }


        public DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_K11_M00_dtkt.sql");
            //var donvis_b6 = "30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94".ToList();
            //if (_loaiBaoCao == 0)
            //{
            //    if (donvis_b6.Contains(_id_donvi))
            //    {
            //        if (_id_phongban == "06")
            //        {
            //            _id_phongban = "06,07,08,10";
            //        }
            //        else
            //        {
            //            _id_phongban = "9999";
            //        }
            //    }

            //}

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", _id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@request", _request);
                cmd.Parameters.AddWithValue("@NamLamViec", namDTKT());
                cmd.Parameters.AddWithValue("@dvt", _dvt);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }


        public DataTable getTable_DuToan()
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_K11_M00_dt.sql");

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
            var nam = int.Parse(PhienLamViec.iNamLamViec);
            var tieude = $"So sánh số kiểm tra và số dự toán ngân sách năm {nam}";

            return tieude;
        }

        private string namDTKT()
        {
            return (PhienLamViec.NamLamViec - 1).ToString();
        }

        public JsonResult Ds_DonVi(string id_PhongBan, int request)
        {
            var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(namDTKT(), getParamUsername(), GetPhongBanId(id_PhongBan), request);
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
