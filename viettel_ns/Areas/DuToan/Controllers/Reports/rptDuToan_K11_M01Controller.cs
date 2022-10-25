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
    public class rptDuToan_K11_M01ViewModel
    {
        public string iNamLamViec { get; set; }

        public SelectList PhongBanList { get; set; }

        public string TieuDe { get; set; }

        public ChecklistModel DonViList { get; set; }
    }
}

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_K11_M01Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private const string _filePath = "~/Report_ExcelFrom/DuToan/KiemTra/rptDuToan_K11_M01.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _to;
        private string _id_phongban;
        private int _dvt = 1000;
        private int _request;
        private int _loaiBaoCao;
        private int _columnCount = 5;


        public ActionResult Index()
        {
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));


            var requestRole = 0;
            var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, getParamUsername(), GetPhongBanId(PhienLamViec.iID_MaPhongBan), requestRole);
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
            string Id_PhongBan,
            int to = 1,
            int request = 1,
            int loaiBaoCao = 0,
            string tieuDe = "",
            string ext = "pdf")
        {

            _id_phongban = GetPhongBanId(Id_PhongBan);
            _to = to;
            _request = request;
            _loaiBaoCao = loaiBaoCao;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport(tieuDe);
            var filename = string.Format("{0}_toso {1}_{2}.{3}", this.ControllerName(), _to, DateTime.Now.GetTimeStamp(), ext);
            return Print(xls, ext, filename);
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

            fr.UseCommonValue()
                .SetValue(new
                {
                    TieuDe1 = string.IsNullOrWhiteSpace(tieuDe) ? getTieuDe() : tieuDe,
                    TieuDe2 = "",
                    TenPhongBan = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                    header1 = "",
                    header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}  Tờ số: {_to}",
                })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

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
            var dtX = _duToanKTService.Get_DonVi_TheoBql_ExistData(namDTKT(), getParamUsername(), GetPhongBanId(_id_phongban), _request);
            var totalRow = dtX.NewRow();
            totalRow["Ten"] = "Tổng cộng";
            dtX.Rows.InsertAt(totalRow, 0);

            // ko lay 54 - Cuc Tai chinh
            var row54 = dtX.AsEnumerable().FirstOrDefault(x => x.Field<string>("Id") == "54");
            if (row54 != null)
                dtX.Rows.Remove(row54);


            // lay columns
            //var columns = _to == 1 ?
            //   dtX.AsEnumerable().Take(_columnCount).ToList() :
            //   dtX.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();
            var columns = dtX.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();


            var data = getTable("");
            // buid columns for report
            for (int i = 0; i < _columnCount; i++)
            {
                var colTuChi = new DataColumn($"TuChi_C{i + 1}", typeof(decimal));
                var colTuChi2 = new DataColumn($"TuChi2_C{i + 1}", typeof(decimal));

                data.Columns.Add(colTuChi);
                data.Columns.Add(colTuChi2);
            }

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                if (i < columns.Count)
                {

                    var row_donvi = columns[i];
                    fr.SetValue(colName, row_donvi.Field<string>("Ten"));

                    #region fill dulieu

                    var id_donvi = row_donvi.Field<string>("Id");

                    var dtDTKT = getTable(id_donvi);
                    var dtDuToan = getTable_DuToan(id_donvi);
                    var dtTemp = data.AsEnumerable().ToList();
                    var _nganhNghiepVuIndex = "";
                    dtTemp.AsEnumerable().ToList()
                        .ForEach(r =>
                        {
                            var xaunoima = r.Field<string>("XauNoiMa");
                            var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));


                            #region cot sokiemtra

                            var rowDTKT = dtDTKT.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));
                            if (rowDTKT != null)
                            {
                                row[$"TuChi_{colName}"] = rowDTKT.Field<double>("TuChi");
                            }

                            #endregion

                            // nganh 00
                            var nganhNghiepVu = r.Field<string>("Nganh");
                            if (nganhNghiepVu == "00")
                            {
                                if (!string.IsNullOrWhiteSpace(xaunoima))
                                {
                                    #region cot dutoan

                                    var xaunoima_list = xaunoima.ToList();
                                    var value = dtDuToan.AsEnumerable()
                                            .Where(x => r["XauNoiMa"] != DBNull.Value
                                                    && xaunoima_list.Any(s => x.Field<string>("sXauNoiMa").Contains(s))
                                                    )
                                            .Sum(x => x.Field<decimal>("rTuChi"));

                                    var value2 = dtDuToan.AsEnumerable()
                                            .Where(x => r["XauNoiMa_x"] != DBNull.Value && x.Field<string>("sXauNoiMa").Contains(r.Field<string>("XauNoiMa_x")))
                                            .Sum(x => x.Field<decimal>("rTuChi"));
                                    //r["TuChi2"] = value - value2;

                                    if (row != null)
                                    {
                                        row[$"TuChi2_{colName}"] = value - value2;
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                if (_nganhNghiepVuIndex != nganhNghiepVu)
                                {
                                    var sumNganh = dtDuToan.AsEnumerable()
                                            .Where(x => x.Field<string>("sNG") == nganhNghiepVu && x.Field<string>("sLNS").IsContains("1020100,1020000,1020700"))
                                            .Sum(x => x.Field<decimal>("rTuChi"));

                                    var nganhCount = data.AsEnumerable().Count(x => x.Field<string>("Nganh") == nganhNghiepVu);
                                    // chen them hang cho nganh nghiep vu, hang cuoi cung cua tung nganh
                                    if (nganhCount > 1)
                                    {
                                        //var newrow = data.NewRow();
                                        //newrow.Field<string>("Code");
                                        //newrow.ItemArray = r.ItemArray.Clone() as object[];
                                        //// ko can mo ta, de giau hang
                                        ////newrow["MoTa"] = string.Empty;
                                        //newrow["TuChi"] = 0;
                                        //newrow["TuChi2"] = sumNganh;
                                        //newrow["id"] = "00";


                                        //var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));
                                        //var currentRowIndex = data.Rows.IndexOf(row);
                                        //data.Rows.InsertAt(newrow, currentRowIndex);

                                        row[$"TuChi2_{colName}"] = sumNganh;
                                    }
                                    else
                                    {
                                        row[$"TuChi2_{colName}"] = sumNganh;
                                    }

                                }
                                _nganhNghiepVuIndex = nganhNghiepVu;
                            }
                        });


                    #endregion


                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           //var value = dt.AsEnumerable()
                           //      .ToList()
                           //      .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("sNG")) || (x.Field<string>("sNG") == row.Field<string>("sNG") && (string.IsNullOrWhiteSpace(row.Field<string>("sXauNoiMa")) || x.Field<string>("sXauNoiMa") == row.Field<string>("sXauNoiMa")))) &&
                           //                 x.Field<int>("iPhanCap") == row.Field<int>("iPhanCap") &&
                           //                 x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                           //      .Sum(x => x.Field<decimal>("TuChi", 0));
                           //r[colName] = value;
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                }
            }
            //var data = getTable();
            //data.Columns.Add(new DataColumn("TuChi2", typeof(decimal)));

            //// fill ghichu
            //var dtDuToan = getTable_DuToan();
            //var dtTemp = data.AsEnumerable().ToList();
            //var _nganhNghiepVuIndex = "";
            //dtTemp.AsEnumerable().ToList()
            //    .ForEach(r =>
            //    {
            //        var xaunoima = r.Field<string>("XauNoiMa");

            //        // nganh 00
            //        var nganhNghiepVu = r.Field<string>("Nganh");
            //        if (nganhNghiepVu == "00")
            //        {
            //            if (!string.IsNullOrWhiteSpace(xaunoima))
            //            {
            //                var xaunoima_list = xaunoima.ToList();
            //                var value = dtDuToan.AsEnumerable()
            //                        .Where(x => r["XauNoiMa"] != DBNull.Value && xaunoima_list.Any(s => x.Field<string>("sXauNoiMa").Contains(s)))
            //                        .Sum(x => x.Field<decimal>("rTuChi"));

            //                var value2 = dtDuToan.AsEnumerable()
            //                        .Where(x => r["XauNoiMa_x"] != DBNull.Value && x.Field<string>("sXauNoiMa").Contains(r.Field<string>("XauNoiMa_x")))
            //                        .Sum(x => x.Field<decimal>("rTuChi"));
            //                //r["TuChi2"] = value - value2;

            //                var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));
            //                if (row != null)
            //                {
            //                    row["TuChi2"] = value - value2;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (_nganhNghiepVuIndex != nganhNghiepVu)
            //            {
            //                var sumNganh = dtDuToan.AsEnumerable()
            //                        .Where(x => x.Field<string>("sNG") == nganhNghiepVu)
            //                        .Sum(x => x.Field<decimal>("rTuChi"));
            //                var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));

            //                var nganhCount = data.AsEnumerable().Count(x => x.Field<string>("Nganh") == nganhNghiepVu);
            //                // chen them hang cho nganh nghiep vu, hang cuoi cung cua tung nganh
            //                if (nganhCount > 1)
            //                {
            //                    var newrow = data.NewRow();
            //                    newrow.Field<string>("Code");
            //                    newrow.ItemArray = r.ItemArray.Clone() as object[];
            //                    // ko can mo ta, de giau hang
            //                    //newrow["MoTa"] = string.Empty;
            //                    newrow["TuChi"] = 0;
            //                    newrow["TuChi2"] = sumNganh;
            //                    newrow["id"] = "00";


            //                    //var row = data.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code") == r.Field<string>("Code"));
            //                    var currentRowIndex = data.Rows.IndexOf(row);
            //                    data.Rows.InsertAt(newrow, currentRowIndex);
            //                }
            //                else
            //                {
            //                    row["TuChi2"] = sumNganh;
            //                }

            //            }
            //            _nganhNghiepVuIndex = nganhNghiepVu;
            //        }
            //    });

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

        public DataTable getTable(string id_donvi)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_K11_M01_dtkt.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@request", _request);
                cmd.Parameters.AddWithValue("@NamLamViec", namDTKT());
                cmd.Parameters.AddWithValue("@dvt", _dvt);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }

        public DataTable getTable_DuToan(string id_donvi)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_K11_M00_dt.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@Id_DonVi", id_donvi.ToParamString());
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

        public JsonResult Ds_To(string id_phongban, int request = 1)
        {
            var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(namDTKT(), getParamUsername(), GetPhongBanId(id_phongban), request);
            return ds_ToIn(data.Rows.Count == 0 ? 0 : data.Rows.Count + 1, _columnCount);
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
