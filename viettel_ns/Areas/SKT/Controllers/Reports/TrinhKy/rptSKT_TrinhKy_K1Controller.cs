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
    public class rptSKT_TrinhKy_K1Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/TrinhKy/";
        private string _filePath = "~/Report_ExcelFrom/SKT/TrinhKy/rptTrinhKy_K1.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 10;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptTrinhKy_K1ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
            };

            var view = _viewPath + "rptTrinhKy_K1.cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string Id_PhongBan,
            int to,
            string tieuDe,
            string ext = "pdf")
        {
            _id_phongban = Id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : Id_PhongBan;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;

            var xls = createReport(tieuDe);
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                return Print(xls[true], ext);
            }
        }


        private Dictionary<bool, ExcelFile> createReport(string tieuDe = null)
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

                      Nam = PhienLamViec.NamLamViec - 1,
                      To = _to,
                  })
                  .UseChuKy(Username)
                  .UseChuKyForController(this.ControllerName());

                xls.Open(Server.MapPath(_filePath));

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

                for (int i = 0; i < _columnCount; i++)
                {
                    data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
                }

                var columns = new List<DataRow>();

                columns = dtX.AsEnumerable().ToList();

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
                                       .Sum(x => x.Field<double>("C", 0));
                               r[colName] = value;
                           });
                    }
                }

                fr.AddTable("DonVi", dtX);
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
            var donvis = (',' + _SKTService.Get_DonVisDT_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, _id_phongban == "-1" ? "06,07,08,10" : _id_phongban)
                                .AsEnumerable().Select(x => x.Field<string>("Id")).Join(","))
                                .Split(',').Skip((_to - 1) * (_columnCount)).Take(_columnCount).Join(",");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("skt_report_k1", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", _nam);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@donvis", donvis);

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        public JsonResult Ds_To(string id_PhongBan)
        {
            var id = id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : id_PhongBan;
            var count = _SKTService.Get_DonVisDT_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, id_PhongBan == "-1" ? "06,07,08,10" : id_PhongBan).AsEnumerable().Count();
            return ds_ToIn(count > 0 ? count + 1 : 0, _columnCount);
        }
        #endregion
    }
}
