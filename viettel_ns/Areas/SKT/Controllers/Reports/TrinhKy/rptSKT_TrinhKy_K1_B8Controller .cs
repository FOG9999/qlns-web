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

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptSKT_TrinhKy_K1_B8Controller : FlexcelReportController
    {
        #region var def
        private string _filePath = "~/Report_ExcelFrom/SKT/TrinhKy/rptTrinhKy_K1_B8_To1.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban;
        private string id_donvi;
        private int id_to;
        private int _dvt = 1000;
        private int _columnCount = 8;


        public ActionResult Index()
        {
            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa");
            //var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec), "iID_MaDonVi", "sMoTa");

            var toList = new Dictionary<int, string>();
            var toCount = getToCount();
            for (int i = 1; i <= toCount; i++)
            {
                toList.Add(i, $"Tờ {i}");
            }

            var vm = new rptNC_DonViViewModel()
            {
                PhongBanList = phongbanList,
                DonViList = toList.ToSelectList("Key", "Value"),
            };

            return View(@"~/Areas\SKT\Views\Reports\TrinhKy\rptTrinhKy_K1_B8.cshtml", vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            string id_donvi,
            string ext = "pdf")
        {
            this.id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            this.id_to = int.Parse(id_donvi);
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport();
            return Print(xls, ext);
        }


        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            var check = loadData(fr);


            fr.UseCommonValue()
              .SetValue(new
              {
                  header1 = id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(id_phongban).sMoTa,
                  header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",

                  TieuDe1 = "Tổng hợp số kiểm tra và số điều chỉnh dự toán năm " + (PhienLamViec.NamLamViec - 1).ToString(),
                  TieuDe2 = "Biểu xác nhận số liệu năm trước làm cơ sở xây dựng số kiểm tra " + PhienLamViec.iNamLamViec + " cho các đơn vị trực thuộc bộ",
                  Nam = PhienLamViec.NamLamViec - 1,
                  To = id_to,
              })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName());


            fr.UseForm(this)
                .Run(xls, id_to);

            return xls;
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
                var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");

                var dtX = dt.SelectDistinct("X", "KyHieu,MoTa");
                dtX.Rows.InsertAt(dtX.NewRow(), 0);




                for (int i = 0; i < _columnCount; i++)
                {
                    data.Columns.Add(new DataColumn($"C{i + 1}1", typeof(decimal)));
                    data.Columns.Add(new DataColumn($"C{i + 1}2", typeof(decimal)));
                }

                var columns = new List<DataRow>();

                columns = dtX.AsEnumerable().Skip(id_to == 1 ? 0 : (id_to - 1) * _columnCount).ToList();

                for (int i = 0; i < _columnCount; i++)
                {
                    var colName = $"C{i + 1}";
                    var colName1 = $"C{i + 1}1";
                    var colName2 = $"C{i + 1}2";

                    // cot tong cong
                    if (i == 0 && id_to == 1)
                    {
                        fr.SetValue(colName, "Tổng cộng");
                        fr.SetValue(colName1, "Số kiểm tra năm " + (PhienLamViec.NamLamViec - 1));
                        fr.SetValue(colName2, "Số điều chỉnh");

                        data.AsEnumerable()
                           .ToList()
                           .ForEach(r =>
                           {
                               // Số dự toán kiểm tra năm trước
                               var value = dt.AsEnumerable()
                                           .ToList()
                                           .Where(x => x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                           .Sum(x => x.Field<double>("TuChi", 0));
                               r[colName1] = value;

                               // Số đặc thù đơn vị được lập nhu cầu
                               value = dt.AsEnumerable()
                                           .ToList()
                                           .Where(x => x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                           .Sum(x => x.Field<double>("DacThu", 0));

                               r[colName2] = value;
                           });
                    }
                    else
                    {
                        if (i < columns.Count)
                        {
                            var row = columns[i];

                            fr.SetValue(colName, row.Field<string>("MoTa"));
                            fr.SetValue(colName1, "Số kiểm tra năm " + (PhienLamViec.NamLamViec - 1));
                            fr.SetValue(colName2, "Số điều chỉnh");

                            data.AsEnumerable()
                               .ToList()
                               .ForEach(r =>
                               {
                                   // Số dự toán kiểm tra năm trước
                                   var value = dt.AsEnumerable()
                                               .ToList()
                                               .Where(x => x.Field<string>("KyHieu") == row.Field<string>("KyHieu") &&
                                                            x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                               .Sum(x => x.Field<double>("TuChi", 0));
                                   r[colName1] = value;

                                   // Số đặc thù đơn vị được lập nhu cầu
                                   value = dt.AsEnumerable()
                                               .ToList()
                                               .Where(x => x.Field<string>("KyHieu") == row.Field<string>("KyHieu") &&
                                                            x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                               .Sum(x => x.Field<double>("DacThu", 0));

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

                //_SKTService.FillDataTable_SKT(fr, data);
                fr.AddTable(data.TableName, data);
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

            //var donvis = "," + (id_donvi.IsEmpty() ? PhienLamViec.iID_MaDonVi : id_donvi);


            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("skt_report_k1_b8", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@donvis", PhienLamViec.iID_MaDonVi);
                cmd.Parameters.AddWithValue("@dvt", _dvt);

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }

        public int getToCount()
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("skt_report_k1_b8", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongban", PhienLamViec.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@donvis", PhienLamViec.iID_MaDonVi);
                cmd.Parameters.AddWithValue("@dvt", _dvt);

                var dt = cmd.GetDataset().Tables[0];
                var dtX = dt.SelectDistinct("X", "KyHieu,MoTa").Rows.Count + 1;

                var toCount = dtX / _columnCount + (dtX % _columnCount == 0 ? 0 : 1);
                return toCount;
            }

        }

        #endregion
    }
}
