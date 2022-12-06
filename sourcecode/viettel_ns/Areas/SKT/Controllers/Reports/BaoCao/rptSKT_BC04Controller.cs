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
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptSKT_BC04Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/BaoCao/";
        private string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC04.xls";
        private string _filePath_tg = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC04_TG.xls";
        private string _filePath_bv = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC04BV.xls";
        private string _filePath_all = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC04_All.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _loaiBC;
        private int _loai;
        private int _to = 1;
        private int _dvt = 1000;
        private int _columnCount = 4;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var loaiList = SharedModels.GetLoaiHinhSKT();

            var vm = new rptSKT_BC04ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                LoaiList = loaiList.ToSelectList("Loai", "Ten"),
            };

            var view = _viewPath + "rptSKT_BC04.cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string Id_PhongBan,
            int to,
            int loaiBC = 1,
            int loai = 1,
            string ext = "pdf")
        {
            _id_phongban = Id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : Id_PhongBan;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _loaiBC = loaiBC;
            _loai = loai;
            _to = to;
            _columnCount = loai == 3 ? 14 : loai == 5 ? 10 : 4;

            var xls = createReport();
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                return Print(xls[true], ext);
            }
        }


        private Dictionary<bool, ExcelFile> createReport()
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);

            var check = loadData(fr);

            if (check.ContainsKey(false))
            {
                return new Dictionary<bool, ExcelFile>
                {
                    {false, xls}
                };
            }
            else
            {

                fr.UseCommonValue()
                  .SetValue(new
                  {
                      header1 = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                      header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",

                      TieuDe1 = "tổng hợp nhu cầu ngân sách sử dụng năm " + PhienLamViec.iNamLamViec,
                      TieuDe2 = _loai == 3 ? "(Áp dụng đối với đơn vị bệnh viện tự chủ)" : _loai == 2 ? "(Số tăng/giảm nhu nhu cầu nhiêm vụ)" : "",

                      Nam = PhienLamViec.NamLamViec,
                      To = _to,
                      PL = _loai == 1 ? "SKT-NSSD" : _loai == 2 ? "SKT-NSSD-TG" : _loai == 3 ? "SKT - NSSD - BV" : _loai == 5 ? "SKT-TH" : "SKT -NSSD-TH",
                  })
                  .UseChuKy(Username)
                  .UseChuKyForController(this.ControllerName());

                xls.Open(Server.MapPath(_loai == 3 ? _filePath_bv : _loai == 5 ? _filePath_all : _loai == 2 ? _filePath_tg : _filePath));

                fr.UseForm(this).Run(xls, _to);

                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }
        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var dt = getTable();

            if (dt.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var data = dt.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu,MoTa");
                var dtX = dt.SelectDistinct("X", "Id_DonVi,TenDonVi");

                var columns = new List<DataRow>();

                columns = dtX.AsEnumerable().ToList();

                if (_loai == 3 || _loai == 5)
                {
                    for (int i = 0; i < _columnCount; i++)
                    {
                        data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
                    }

                    for (int i = 0; i < _columnCount; i++)
                    {
                        var colName = $"C{i + 1}";
                        if (i < columns.Count)
                        {
                            var row = columns[i];

                            data.AsEnumerable()
                               .ToList()
                               .ForEach(r =>
                               {
                                   var value = dt.AsEnumerable()
                                           .ToList()
                                           .Where(x => (x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                        x.Field<string>("KyHieu") == r.Field<string>("KyHieu"))
                                           .Sum(x => _loai == 3 ? x.Field<double>("C", 0) : x.Field<double>("C2", 0));
                                   r[colName] = value;
                               });
                        }
                        else
                        {
                            fr.SetValue($"C{i + 1}", "");
                            fr.SetValue(colName, "");
                        }
                    }

                    fr.AddTable("DonVi", dtX);
                }
                else
                {
                    for (int i = 0; i < _columnCount; i++)
                    {
                        data.Columns.Add(new DataColumn($"C{i + 1}1", typeof(decimal)));
                        data.Columns.Add(new DataColumn($"C{i + 1}2", typeof(decimal)));
                    }

                    for (int i = 0; i < _columnCount; i++)
                    {
                        var colName = $"C{i + 1}";
                        var colName1 = $"C{i + 1}1";
                        var colName2 = $"C{i + 1}2";
                        if (i < columns.Count)
                        {

                            var row = columns[i];
                            fr.SetValue(colName, row.Field<string>("TenDonVi"));
                            if (_loai == 2)
                            {
                                fr.SetValue(colName1, "Tăng");
                                fr.SetValue(colName2, "Giảm");
                            }
                            else
                            {
                                fr.SetValue(colName1, "Huy động tồn kho");
                                fr.SetValue(colName2, "Chi bằng tiền");
                            }

                            data.AsEnumerable()
                               .ToList()
                               .ForEach(r =>
                               {
                                   // Huy động tồn kho
                                   var value = dt.AsEnumerable()
                                           .ToList()
                                           .Where(x => x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi") &&
                                                        x.Field<string>("KyHieu") == r.Field<string>("KyHieu"))
                                           .Sum(x => x.Field<double>("C1", 0));
                                   r[colName1] = value;

                                   // Số chi bằng tiền
                                   value = dt.AsEnumerable()
                                           .ToList()
                                           .Where(x => x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi") &&
                                                        x.Field<string>("KyHieu") == r.Field<string>("KyHieu"))
                                           .Sum(x => x.Field<double>("C2", 0));

                                   r[colName2] = value;
                               });
                        }
                        else
                        {
                            fr.SetValue($"C{i + 1}", "");
                            fr.SetValue(colName1, "");
                            fr.SetValue(colName2, "");
                        }
                    }
                }
                _SKTService.FillDataTable_NC(fr, data);
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public DataTable getTable()
        {
            #region get data
            var sql = "skt_report_bc04";
            //var donvis = _SKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, 
            //    PhienLamViec.iID_MaDonVi, 
            //    PhienLamViec.iID_MaPhongBan == "11" ? _loaiBC == 1 ? "06,07,08,10" : "02" : PhienLamViec.iID_MaPhongBan, 
            //    _id_phongban).AsEnumerable().Select(x => x.Field<string>("Id")).Join(",");

            var donvis = _SKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec,
                    PhienLamViec.iID_MaDonVi,
                    PhienLamViec.iID_MaPhongBan == "11" || PhienLamViec.iID_MaPhongBan == "02" ? "06,07,08,10" : PhienLamViec.iID_MaPhongBan,
                    _id_phongban)
                .AsEnumerable()
                .Select(x => x.Field<string>("Id"))
                .Join();


            if (_loai == 2)
            {
                sql = "skt_report_bc04tg";
                donvis = _SKTService.Get_DonViTG_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan == "11" ? _loaiBC == 1 ? "06,07,08,10" : "02" : PhienLamViec.iID_MaPhongBan, _id_phongban).AsEnumerable().Select(x => x.Field<string>("Id")).Join(",");
            }
            else if (_loai == 3)
            {
                sql = "skt_report_bc04bv";
                donvis = _SKTService.Get_DonVisGBV_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan == "11" ? _loaiBC == 1 ? "06,07,08,10" : "02" : PhienLamViec.iID_MaPhongBan, _id_phongban).AsEnumerable().Select(x => x.Field<string>("Id")).Join(",");
            }
            else if (_loai == 4)
            {
                sql = "skt_report_bc04th";
            }
            else if (_loai == 5)
            {
                sql = "skt_report_bc04_all";
            }
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@donvis", ("," + donvis).Split(',').Skip((_to - 1) * _columnCount).Take(_columnCount).Join(","));
                cmd.Parameters.AddWithValue("@donvisql", donvis);
                cmd.Parameters.AddWithValue("@b", _loaiBC);

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        public JsonResult Ds_To(string id_phongban, int loaiBC = 1, int loai = 1)
        {
            var id = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            var count = _SKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan == "11" ? loaiBC == 1 ? "06,07,08,10" : "02" : PhienLamViec.iID_MaPhongBan, id_phongban).Rows.Count;
            if (loai == 2)
            {
                count = _SKTService.Get_DonViTG_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan == "11" ? loaiBC == 1 ? "06,07,08,10" : "02" : PhienLamViec.iID_MaPhongBan, id_phongban).Rows.Count;
            }
            else if (loai == 3)
            {
                count = _SKTService.Get_DonVisGBV_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan == "11" ? loaiBC == 1 ? "06,07,08,10" : "02" : PhienLamViec.iID_MaPhongBan, id_phongban).Rows.Count;
            }
            else if (loai == 5)
            {
                //count = _SKTService.Get_DonVisGBV_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan == "11" || PhienLamViec.iID_MaPhongBan == "02" ? "06,07,08,10" : PhienLamViec.iID_MaPhongBan, id_phongban).Rows.Count;
            }
            return ds_ToIn(count > 0 ? count + 1 : 0, loai == 3 ? 14 : _loai == 5 ? 10 : 4);
        }
        #endregion
    }
}
