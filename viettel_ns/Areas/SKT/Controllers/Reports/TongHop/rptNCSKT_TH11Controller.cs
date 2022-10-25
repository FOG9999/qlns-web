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
    public class rptNCSKT_TH11ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }
    public class rptNCSKT_TH11Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/TongHop/";
        private string _filePath_ngang = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH11/rptNCSKT_TH11_Land.xls";
        private string _filePath_doc = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH11/rptNCSKT_TH11_Port.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        // Loại số liệu 1: Tất cả; 2: Lương, pc, tiền ăn, nghiệp vụ 00; 3: Nghiệp vụ ngành
        private string _loai;
        // Loại báo cáo 1: Nganh; 2: Dọc
        private string _loaiBC;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 3;

        public ActionResult Index()
        {
            if (User.Identity.Name.EndsWith("b2"))
            {
                var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

                var vm = new rptNCSKT_TH11ViewModel()
                {
                    PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1" ,"--- Tất cả phòng ban ---"),
                };

                var view = _viewPath + "rptNCSKT_TH11.cshtml";

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
            string loaiBC = "1",
            string ext = "pdf")
        {
            _id_phongban = id_phongban;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;
            _loai = loai;
            _loaiBC = loaiBC;

            var xls = createReport();
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var filename = $"{(_loai.StartsWith("1") ? "BQL" : "PhuLuc")}_NSSD_00_BQL {_nganSachService.GetPhongBanById(_id_phongban).sMoTa}_{DateTime.Now.GetTimeStamp()}.{ext}";
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
                      header1 = _id_phongban != "-1" ? _nganSachService.GetPhongBanById(_id_phongban).sMoTa : "",
                      header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                      loai = _loai,
                      To = _to,                      
                  })
                  .UseChuKy(Username)
                  .UseChuKyForController(this.ControllerName());

                xls.Open(Server.MapPath(_loaiBC == "1" ? _filePath_ngang : _filePath_doc));

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
                if (_loaiBC == "1") {
                    var data = dt.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu,MoTa");
                    var dtX = dt.SelectDistinct("X", "Id_DonVi,TenDonVi");

                    var columns = new List<DataRow>();

                    columns = dtX.AsEnumerable().Skip((_to - 1) * (_columnCount)).Take(_columnCount).ToList();

                    for (int i = 0; i < _columnCount; i++)
                    {
                        data.Columns.Add(new DataColumn($"C{i + 1}_1", typeof(decimal)));
                        data.Columns.Add(new DataColumn($"C{i + 1}_2", typeof(decimal)));
                    }

                    for (int i = 0; i < _columnCount; i++)
                    {
                        var colName = $"C{i + 1}";
                        var colName1 = $"C{i + 1}_1";
                        var colName2 = $"C{i + 1}_2";
                        if (i < columns.Count)
                        {
                            var row = columns[i];
                            fr.SetValue(colName, row.Field<string>("TenDonVi"));
                            fr.SetValue(colName1, "QT Quí IV/2019");
                            fr.SetValue(colName2, "Dự kiến");

                            data.AsEnumerable()
                                .ToList()
                                .ForEach(r =>
                                {
                                    var value = dt.AsEnumerable()
                                            .ToList()
                                            .Where(x => x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi") &&
                                                        x.Field<string>("KyHieu") == r.Field<string>("KyHieu"))
                                            .Sum(x => x.Field<double>("C1", 0));
                                    r[colName1] = value;
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
                            fr.SetValue($"C{i + 1}_1", "");
                            fr.SetValue($"C{i + 1}_2", "");
                        }

                    }
                    _SKTService.FillDataTable_NC(fr, data);
                }
                else
                {
                    fr.AddTable("DonVi", dt);
                    var phongban = dt.SelectDistinct("PhongBan", "Id_PhongBan,TenPhongBan");
                    fr.AddTable("PhongBan", phongban);
                    fr.AddRelationship("PhongBan", "DonVi", "Id_PhongBan".Split(','), "Id_PhongBan".Split(','));
                    //_SKTService.FillDataTable_NC(fr, dt.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu"));
                    //var donvi = dt.SelectDistinct("DonVi", "X1,X2,X3,X4,KyHieu,Id_DonVi,TenDonVi");
                    //fr.AddTable("PhongBan", dt);
                    //fr.AddTable("DonVi", donvi);
                    //fr.AddRelationship("ChiTiet", "DonVi", "X1,X2,X3,X4,KyHieu".Split(','), "X1,X2,X3,X4,KyHieu".Split(','));
                    //fr.AddRelationship("DonVi", "PhongBan", "X1,X2,X3,X4,KyHieu,Id_DonVi".Split(','), "X1,X2,X3,X4,KyHieu,Id_DonVi".Split(','));
                }
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
            int vari;
            switch (_loai)
            {
                case "2":
                    vari = 42;
                    break;
                case "3":
                    vari = 43;
                    break;
                default:
                    vari = 41;
                    break;
            }
            var donvis = ',' + _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, vari, PhienLamViec.iID_MaDonVi, "99", _id_phongban)
                                .AsEnumerable().Select(x => x.Field<string>("Id")).Join(",");
            donvis = _loaiBC == "2" ? "" : donvis;
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(_loaiBC == "1" ? "sp_ncskt_report_th11_land" : "sp_ncskt_report_th11_port", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongbandich", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", _nam);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@loai", _loai);
                cmd.Parameters.AddWithValue("@donvis", donvis.ToParamString());

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        public JsonResult Ds_To(string id_phongban, string loai)
        {
            int vari;
            switch (loai)
            {
                case "2":
                    vari = 42;
                    break;
                case "3":
                    vari = 43;
                    break;
                default:
                    vari = 41;
                    break;
            }
            var count = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, vari, PhienLamViec.iID_MaDonVi, "99", id_phongban).AsEnumerable().Count();
            return ds_ToIn(count > 0 ? (count + 1) : 0, _columnCount);
        }
        #endregion
    }
}
