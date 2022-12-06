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
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;
using VIETTEL.Report_Controllers.BaoHiem;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptSKT_PL05Controller : FlexcelReportController
    {
        #region var def
        //private string _filePath = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL05.xls";
        private string _filePath = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL05_PhuLuc.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _loaiBC;
        private int _loai;
        private int _to = 1;
        private int _to_tong = 1;
        private int _dvt = 1000;
        private int _columnCount = 12;
        private int _page = 0;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var loaiList = SharedModels.GetLoaiHinhSKT();

            var vm = new rptSKT_BC04ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                LoaiList = loaiList.ToSelectList("Loai", "Ten"),
            };

            var view = $"~/Areas/SKT/Views/Reports/PhuLuc/{this.ControllerName()}.cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string Id_PhongBan,
            int to,
            int loaiBC = 1,
            int loai = 1,
            int page = 0,
            string ext = "pdf")
        {
            _id_phongban = Id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : Id_PhongBan;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _loaiBC = loaiBC;
            _loai = loai;
            _to = to;
            _page = page;



            //_columnCount = _page == 0 ? 10 : 6;

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
                      header1 = _id_phongban == "-1" || _id_phongban == "11" || _id_phongban == "02" ? "(Tổng hợp)" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                      header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}/{_to_tong}",

                      TieuDe1 = "Tổng hợp số kiểm tra dự toán ngân sách năm " + PhienLamViec.iNamLamViec,
                      TieuDe2 = "Số phân cấp ngân sách sử dụng",

                      Nam = PhienLamViec.NamLamViec,
                      To = _to,
                      PL = "SKT-PC",
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
            //var dt = getTable();
            var dt = getTable_all();

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
                //var rows_X = dtX.AsEnumerable().Skip(_to == 0 ? 0 : (_to - 1) * _columnCount + 1);

                _to_tong = dtX.Rows.Count.ToPageCount(_columnCount);
                var columns = new List<DataRow>();
                //columns = dtX.AsEnumerable().ToList();
                columns = dtX.AsEnumerable().Skip(_to == 1 ? 0 : (_to - 1) * _columnCount).Take(_columnCount).ToList();

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

                fr.AddTable("DonVi", columns.Select(r => new { TenDonVi = r.Field<string>("TenDonVi") }));
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

            var donvis = _SKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec,
                    PhienLamViec.iID_MaDonVi,
                    PhienLamViec.iID_MaPhongBan == "11" || PhienLamViec.iID_MaPhongBan == "02" ? "06,07,08,10" : PhienLamViec.iID_MaPhongBan,
                    _id_phongban)
                .AsEnumerable()
                .Select(x => x.Field<string>("Id"))
                .Distinct()
                .Join();

            var sql = "skt_report_bc04_ng";

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

        //public DataTable getTable_all()
        //{
        //    #region get data

        //    //var donvis = _SKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec,
        //    //        PhienLamViec.iID_MaDonVi,
        //    //        PhienLamViec.iID_MaPhongBan == "11" || PhienLamViec.iID_MaPhongBan == "02" ? "06,07,08,10" : PhienLamViec.iID_MaPhongBan,
        //    //        _id_phongban)
        //    //    .AsEnumerable()
        //    //    .Select(x => x.Field<string>("Id"))
        //    //    .Distinct()
        //    //    .Join();

        //    var donvis = PhienLamViec.iID_MaDonVi;

        //    var sql = "skt_report_bc04_ng";

        //    using (var conn = ConnectionFactory.Default.GetConnection())
        //    using (var cmd = new SqlCommand(sql, conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@phongban", _id_phongban.ToParamString());
        //        cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
        //        cmd.Parameters.AddWithValue("@dvt", _dvt);
        //        //cmd.Parameters.AddWithValue("@donvis", ("," + donvis).Split(',').Skip((_to - 1) * _columnCount).Take(_columnCount).Join(","));
        //        cmd.Parameters.AddWithValue("@donvis", "," + donvis);
        //        cmd.Parameters.AddWithValue("@donvisql", donvis.ToParamString());
        //        cmd.Parameters.AddWithValue("@b", _loaiBC);

        //        var dt = cmd.GetDataset().Tables[0];
        //        return dt;
        //    }
        //    #endregion   
        //}

        public DataTable getTable_all()
        {

            //if (_loai == 0)
            //{
            //    _id_phongban = "02";
            //    _id_phongban_dich = "06,07,08,10";
            //}
            //else
            //{
            //    if (_id_phongban.IsEmpty("-1"))
            //        _id_phongban = "06,07,08,10";

            //    _id_phongban_dich = _id_phongban;
            //}

            //_id_phongban = "02";
            var _id_phongban_dich = "06,07,08,10";

            #region get data
            var sql = FileHelpers.GetSqlQuery("nc_report_pl05.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        PhienLamViec.NamLamViec,
                        id_phongban = "02".ToParamString(),
                        id_phongban_dich = _id_phongban.ToParamString(),
                        dvt = _dvt
                    });

                return dt;
            }
            #endregion   
        }

        public DataTable getTable_cache()
        {
            //var cache_key = $"skt_{PhienLamViec.iNamLamViec}_{this.ControllerName()}_{_id_phongban}";
            //return CacheService.Default.CachePerRequest(cache_key, () => getTable_all(), CacheTimes.OneMinute);
            return getTable_all();
        }


        public JsonResult Ds_To(string id_phongban, int loaiBC = 1, int loai = 1)
        {
            var id = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;

            var count = _SKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan == "11" || PhienLamViec.iID_MaPhongBan == "02" ? "06,07,08,10" : PhienLamViec.iID_MaPhongBan, id_phongban).Rows.Count;
            return ds_ToIn(count > 0 ? count + 1 : 0, _columnCount);
        }



        #endregion
    }
}
