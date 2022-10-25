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
    public class rptNCSKT_TH99ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }
    public class rptNCSKT_TH99Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/TongHop/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH99/rptNCSKT_TH99.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _loai;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 9;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptNCSKT_TH99ViewModel()
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa"),
            };

            var view = _viewPath + "rptNCSKT_TH99.cshtml";

            return View(view, vm);
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
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;
            _loai = loai;

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
                      header1 = _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                      header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                      To = _to,
                      L = _loai.StartsWith("1") ? $"(BQL ĐỀ NGHỊ) NĂM {PhienLamViec.iNamLamViec}" : _loai.StartsWith("2") ? $"(PHỤ LỤC GỬI ĐƠN VỊ) NĂM {PhienLamViec.iNamLamViec}" : $"(PHỤ LỤC GỬI ĐƠN VỊ) NĂM {PhienLamViec.NamLamViec - 1}",
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
                                        .Where(x => x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi") &&
                                                    x.Field<string>("KyHieu") == r.Field<string>("KyHieu"))
                                        .Sum(x => x.Field<double>("C", 0));
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
            var donvis = ',' + _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, _loai == "3" ? 11 : _loai == "4" ? 5 : 4, PhienLamViec.iID_MaDonVi, _loai == "2" ? "02" : PhienLamViec.iID_MaPhongBan, _id_phongban)
                                .AsEnumerable().Select(x => x.Field<string>("Id")).Join(",");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_report_th99", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongbandich", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", _nam);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@loai", _loai);
                cmd.Parameters.AddWithValue("@donvis", donvis);

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        public JsonResult Ds_To(string id_phongban, string loai)
        {
            _loai = loai;  
            var count = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, _loai == "3" ? 11 : _loai == "4" ? 5 : 4, PhienLamViec.iID_MaDonVi, _loai == "2" ? "02" : PhienLamViec.iID_MaPhongBan, id_phongban).AsEnumerable().Count();
            return ds_ToIn(count > 0 ? (count + 1) : 0, _columnCount);
        }
        #endregion
    }
}
