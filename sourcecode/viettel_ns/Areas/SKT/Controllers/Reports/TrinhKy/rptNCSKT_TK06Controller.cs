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
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_TK06ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptNCSKT_TK06Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/TrinhKy/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/TrinhKy/rptNCSKT_TK06/rptNCSKT_TK06.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_nganh;
        private string _loai;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 3;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var nganhList = _SKTService.GetNganhAll(PhienLamViec.NamLamViec, Username)
                                            .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            var vm = new rptNCSKT_TK06ViewModel()
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa"),
                NganhList = nganhList,
            };

            var view = _viewPath + "rptNCSKT_TK06.cshtml";

            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            string id_nganh,
            int to,
            string loai = "Bv",
            string ext = "pdf")
        {
            _id_phongban = id_phongban;
            _id_nganh = id_nganh;
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
                var filename = $"NSSD_NV_BQL {_nganSachService.GetPhongBanById(_id_phongban).sMoTa}_{DateTime.Now.GetTimeStamp()}.{ext}";
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
                      m = "Mẫu 99",
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
                var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
                var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganhCon");

                for (int i = 0; i < _columnCount; i++)
                {
                    data.Columns.Add(new DataColumn($"C{i + 1}_1", typeof(double)));
                    data.Columns.Add(new DataColumn($"C{i + 1}_2", typeof(double)));
                    data.Columns.Add(new DataColumn($"C{i + 1}_3", typeof(double)));
                }
                
                var view = dtNganh.DefaultView;
                view.Sort = "Nganh";
                dtNganh = view.ToTable();

                var r1 = dtNganh.NewRow();
                r1["Nganh"] = "";
                r1["TenNganhCon"] = "Tổng cộng";
                dtNganh.Rows.InsertAt(r1, 0);

                var columns = new List<DataRow>();

                columns = _to == 1 ?
                    dtNganh.AsEnumerable().Take(_columnCount).ToList() :
                    dtNganh.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();

                for (int i = 0; i < _columnCount; i++)
                {
                    var colName = $"C{i + 1}";
                    var colName1 = $"C{i + 1}_1";
                    var colName2 = $"C{i + 1}_2";
                    var colName3 = $"C{i + 1}_3";

                    if (i < columns.Count)
                    {
                        fr.SetValue(colName, columns[i].Field<string>("TenNganhCon"));
                        fr.SetValue(colName1, "Tăng");
                        fr.SetValue(colName2, "Giảm");
                        fr.SetValue(colName3, "(+)");

                        data.AsEnumerable()
                           .ToList()
                           .ForEach(r =>
                           {
                               var value =
                                    dt.AsEnumerable()
                                       .ToList()
                                       .Where(x =>
                                        x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                        (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")))
                                       .Sum(x => x.Field<double>("C1", 0));

                               r[colName1] = value;

                               value =
                                    dt.AsEnumerable()
                                       .ToList()
                                       .Where(x =>
                                        x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                        (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")))
                                       .Sum(x => x.Field<double>("C2", 0));

                               r[colName2] = value;

                               value =
                                    dt.AsEnumerable()
                                       .ToList()
                                       .Where(x =>
                                        x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                        (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")))
                                       .Sum(x => x.Field<double>("C3", 0));

                               r[colName3] = value;
                           });
                    }
                    else
                    {
                        fr.SetValue(colName, "");
                        fr.SetValue(colName1, "");
                        fr.SetValue(colName2, "");
                        fr.SetValue(colName3, "");
                    }
                }

                fr.AddTable(data);
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
            using (var cmd = new SqlCommand("sp_ncskt_report_tk06", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", _nam);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@loai", _loai == "Bv" ? "2" : "1");
                cmd.Parameters.AddWithValue("@donvis", PhienLamViec.iID_MaDonVi);
                cmd.Parameters.AddWithValue("@nganh", _id_nganh.ToParamString());

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        public JsonResult Ds_Nganh(string id_phongban, string loai)
        {
            int vari;
            switch (loai)
            {
                case "Bv":
                    vari = 41;
                    break;
                default:
                    vari = 4;
                    break;
            }
            var data = _SKTService.Get_Nganh_ExistData(PhienLamViec.iNamLamViec, vari, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id_phongban).SelectDistinct("Nganh","Id","Id,Ten","ASC");
            var vm = new ChecklistModel("Id_Nganh", data.ToSelectList("Id", "Ten", "-1", "-- Chọn ngành --"));

            return ToDropdownList(vm);
        }
        public JsonResult Ds_To(string id_phongban, string loai, string id_nganh)
        {
            int vari;
            switch (loai)
            {
                case "Bv":
                    vari = 41;
                    break;
                default:
                    vari = 4;
                    break;
            }
            var dtNganh = _SKTService.Get_Nganh_ExistData(PhienLamViec.iNamLamViec, vari, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id_phongban, id_nganh);
            
            return ds_ToIn(dtNganh.Rows.Count == 0 ? 0 : dtNganh.Rows.Count + 1, _columnCount);
        }
        #endregion
    }
}
