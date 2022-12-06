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
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_TH14ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }
    public class rptNCSKT_TH14Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/TongHop/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH14/rptNCSKT_TH14.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        
        private string _id_phongban = "-1";
        private string _loai;
        private int _to = 1;
        private int _dvt = 1000;
        private int _columnCount = 9;

        public ActionResult Index()
        {
            if (User.Identity.Name.EndsWith("b2"))
            {
                var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

                var vm = new rptNCSKT_TH14ViewModel()
                {
                    PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "--- Tất cả phòng ban ---"),
                };

                var view = _viewPath + "rptNCSKT_TH14.cshtml";

                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            int to,
            string loai = "1",
            string ext = "pdf")
        {
            _id_phongban = id_phongban;
            _to = to;
            _loai = loai;

            var xls = createReport();
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var filename = $"Tổng hợp số liệu theo đợt dự toán bổ sung.{ext}";
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
                      header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
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

                if (dtX.Rows.Count > 1)
                {
                    var nr = dtX.NewRow();
                    nr["Id_DonVi"] = "";
                    nr["TenDonVi"] = "Tổng cộng";
                    dtX.Rows.InsertAt(nr, 0);
                }
                var columns = new List<DataRow>();

                columns = dtX.AsEnumerable().Skip((_to - 1) * (_columnCount)).Take(_columnCount).ToList();
                
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
                        fr.SetValue(colName, row.Field<string>("TenDonVi"));                        

                        data.AsEnumerable()
                            .ToList()
                            .ForEach(r =>
                            {
                                var value = dt.AsEnumerable()
                                        .ToList()
                                        .Where(x => (string.IsNullOrEmpty(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                    x.Field<string>("KyHieu") == r.Field<string>("KyHieu"))
                                        .Sum(x => x.Field<double>("C1", 0));
                                r[colName] = value;                                
                            });
                    }
                    else
                    {
                        fr.SetValue($"C{i + 1}", "");
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
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_report_th14", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongbandich", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@loai", _loai);
                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        public JsonResult Ds_To(string id_phongban, string loai)
        {
            _id_phongban = id_phongban;
            _loai = loai;
            _dvt = 1000;
            var count = getTable().SelectDistinct("X", "Id_DonVi").Rows.Count;
            return ds_ToIn(count > 1 ? (count + 1) : (count == 0 ? 0 : 1), _columnCount);
        }
        #endregion
    }
}
