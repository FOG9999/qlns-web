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
    public class rptSKT_PL03_NgController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/PhuLuc/";
        private string _filePath = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL03_Ng.xls";
        private const string _filePath_a3 = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL03_Ng_TH.xls";
        private const string _filePath_a3_less = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL03_Ng_TH_less.xls";

        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _dvt = 1000;
        private int _loai = 4;
        private int _loaiBC = 1;
        private int _loaiBaoCao = 1;
        private int _to = 1;
        private int _columnsCount = 9;

        public ActionResult Index(int LoaiBaoCao)
        {
            var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var nganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username)
                                            .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            var mucList = SharedModels.GetMucChiTietNS().Select("Loai in ('2','4')").CopyToDataTable().ToSelectList("Loai", "Ten");

            var vm = new rptSKT_PL03ViewModel
            {
                LoaiBC = LoaiBaoCao,
                NganhList = nganhList,
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                MucList = mucList,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        public JsonResult Ds_To(string id_nganh, string id_phongban = null, int loaiBC = 1, int loai = 4)
        {
            var nganh = _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh, Username);
            string nganhstr = "";

            if (nganh == null)
            {
                nganhstr = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id_phongban).AsEnumerable()
                                .Where(x => !string.IsNullOrEmpty(x.Field<string>("iID_MaNganhMLNS")) && (id_nganh == "-1" || x.Field<string>("MaNganh") == id_nganh))
                                .Select(x => x.Field<string>("iID_MaNganhMLNS")).Join(",");
            }
            else
            {
                nganhstr = nganh.iID_MaNganhMLNS;
            }


            var sql = loaiBC == 1 ? "skt_report_pl03" : "skt_report_pl03_th";
            if (loai == 2)
            {
                sql = loaiBC == 1 ? "skt_report_pl03_m" : "skt_report_pl03_m_th";
            }


            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                    nganh = nganhstr.ToParamString(),
                    phongban = PhienLamViec.iID_MaPhongBan.ToParamString(),
                    dvt = _dvt,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh,KyHieu,MoTa");
                var dtMuc = dtNganh.SelectDistinct("Muc", "Nganh,TenNganh");
                var c = dtMuc.Rows.Count == 1 ? 0 : dtMuc.Rows.Count;
                return ds_ToIn(dtNganh.Rows.Count == 0 ? 0 : dtNganh.Rows.Count + c + 1, loaiBC == 1 ? _columnsCount : 14 );
            }            
        }

        public ActionResult Ds_Nganh(string id_phongban)
        {
            var id = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            var list = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id)
                                          .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            return ToDropdownList(new VIETTEL.Models.ChecklistModel("id_nganh", list));
        }

        public ActionResult Print(
            string ext,
            string id_phongban,
            string id_nganh,
            int loaiBaoCao,
            int to,
            int loaiBC = 1,
            int loai = 4,
            int dvt = 1000)
        {
            _id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            _id_nganh = id_nganh;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _loaiBC = loaiBC;
            _loaiBaoCao = loaiBaoCao;
            _loai = loai;
            _to = to;
            _filePath = loaiBC == 1 ? _filePath : _loai == 4 ? _filePath_a3_less : _filePath_a3;
            _columnsCount = loaiBC == 1 ? 9 : 14;

            var xls = CreateReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                return Print(xls[true], ext);
            }
        }

        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private Dictionary<bool, ExcelFile> CreateReport()
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);

            var check = LoadData(fr);

            if (check.ContainsKey(false))
            {
                return new Dictionary<bool, ExcelFile>
                {
                    {false, xls}
                };
            }
            else
            {
                xls.Open(Server.MapPath(_filePath));

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        headerl = _loaiBaoCao == 1 ? _id_nganh == "-1" ? "" : "Đơn vị: " + _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_nganh).sTen : _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
                        headerr = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                        nganh = _id_nganh == "-1" ? "" : "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh,
                        QuyetDinh = _loaiBaoCao == 1 ? L("SKT_QuyetDinh_2020") : "",
                        nam = PhienLamViec.NamLamViec,
                        loai = _loai,
                    })
                    .UseChuKyForController(this.ControllerName())
                    .UseForm(this)
                    .Run(xls, _to);

                if (_to == 1 && _loai == 2)
                {
                    if (_loaiBC == 1)
                    {
                        xls.MergeCells(8, 4, 9, 4);
                        xls.MergeH(8, 5, 9);
                    }
                    else
                    {
                        xls.MergeCells(8, 4, 9, 4);
                        xls.MergeH(8, 5, 14);
                    }
                }
                else
                {
                    if (_loaiBC == 1)
                    {
                        xls.MergeH(8, 4, 9);
                    }
                    else
                    {
                        xls.MergeH(8, 4, 14);
                    }
                }

                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> LoadData(FlexCelReport fr)
        {
            var dt = GetTable();
            if (dt.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                if (_loai == 2)
                {
                    var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
                    var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh,KyHieu,MoTa");
                    var dtMuc = dtNganh.SelectDistinct("Muc", "Nganh,TenNganh");

                    if (dtMuc.Rows.Count > 1) { 
                        for (int i = dtMuc.Rows.Count - 1; i >= 0; i--)
                        {                        
                            var rN = dtNganh.NewRow();
                            rN["TenNganh"] = dtMuc.Rows[i]["TenNganh"];
                            rN["Nganh"] = dtMuc.Rows[i]["Nganh"];
                            rN["KyHieu"] = "";
                            rN["MoTa"] = "Cộng";
                            dtNganh.Rows.InsertAt(rN, 0);
                        }
                    }

                    var view = dtNganh.DefaultView;
                    view.Sort = "Nganh,KyHieu";
                    dtNganh = view.ToTable();

                    for (int i = 0; i < _columnsCount; i++)
                    {
                        data.Columns.Add(new DataColumn($"C{i + 1}", typeof(double)));
                    }

                    var columns = new List<DataRow>();

                    var r1 = dtNganh.NewRow();
                    r1["TenNganh"] = "Tổng cộng";
                    r1["Nganh"] = "";
                    r1["KyHieu"] = "";
                    r1["MoTa"] = "";
                    dtNganh.Rows.InsertAt(r1, 0);

                    columns = _to == 1 ?
                        dtNganh.AsEnumerable().Take(_columnsCount).ToList() :
                        dtNganh.AsEnumerable().Skip((_to - 1) * _columnsCount).Take(_columnsCount).ToList();

                    for (int i = 0; i < _columnsCount; i++)
                    {
                        var colName = $"C{i + 1}";
                        var colName1 = $"C{i + 1}_1";

                        if (i < columns.Count)
                        {
                            fr.SetValue(colName, columns[i].Field<string>("TenNganh"));
                            fr.SetValue(colName1, columns[i].Field<string>("MoTa"));

                            data.AsEnumerable()
                               .ToList()
                               .ForEach(r =>
                               {
                                   var value =
                                        dt.AsEnumerable()
                                           .ToList()
                                           .Where(x =>
                                            x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                            (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")) &&
                                            (string.IsNullOrWhiteSpace(columns[i].Field<string>("KyHieu")) || columns[i].Field<string>("KyHieu").Contains(x.Field<string>("KyHieu"))))
                                           .Sum(x => x.Field<double>("C", 0));

                                   r[colName] = value;
                               });
                        }
                        else
                        {
                            fr.SetValue(colName, "");
                            fr.SetValue(colName1, "");
                        }
                    }

                    fr.AddTable(data);
                }
                else
                {
                    var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
                    var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh,KyHieu,MoTa");

                    for (int i = 0; i < _columnsCount; i++)
                    {
                        data.Columns.Add(new DataColumn($"C{i + 1}", typeof(double)));
                    }

                    var columns = new List<DataRow>();

                    var r1 = dtNganh.NewRow();
                    r1["TenNganh"] = "Tổng cộng";
                    r1["Nganh"] = "";
                    r1["KyHieu"] = "";
                    r1["MoTa"] = "";
                    dtNganh.Rows.InsertAt(r1, 0);

                    columns = _to == 1 ?
                        dtNganh.AsEnumerable().Take(_columnsCount).ToList() :
                        dtNganh.AsEnumerable().Skip((_to - 1) * _columnsCount).Take(_columnsCount).ToList();

                    for (int i = 0; i < _columnsCount; i++)
                    {
                        var colName = $"C{i + 1}";
                        var colName1 = $"C{i + 1}_1";

                        if (i < columns.Count)
                        {
                            fr.SetValue(colName, columns[i].Field<string>("TenNganh"));
                            fr.SetValue(colName1, columns[i].Field<string>("MoTa"));

                            data.AsEnumerable()
                               .ToList()
                               .ForEach(r =>
                               {
                                   var value =
                                        dt.AsEnumerable()
                                           .ToList()
                                           .Where(x =>
                                            x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi") &&
                                            (string.IsNullOrWhiteSpace(columns[i].Field<string>("Nganh")) || x.Field<string>("Nganh") == columns[i].Field<string>("Nganh")) &&
                                            (string.IsNullOrWhiteSpace(columns[i].Field<string>("KyHieu")) || columns[i].Field<string>("KyHieu").Contains(x.Field<string>("KyHieu"))))
                                           .Sum(x => x.Field<double>("C", 0));

                                   r[colName] = value;
                               });
                        }
                        else
                        {
                            fr.SetValue(colName, "");
                            fr.SetValue(colName1, "");
                        }
                    }

                    fr.AddTable(data);
                }

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable GetTable()
        {
            var sql = "skt_report_pl03";


            sql = _loaiBC == 1 ? "skt_report_pl03" : "skt_report_pl03_th";
            if (_loai == 2)
            {
                sql = _loaiBC == 1 ? "skt_report_pl03_m" : "skt_report_pl03_m_th";
            }




            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                var nganh = _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh, Username);
                string nganhstr = "";

                if (nganh == null)
                {
                    nganhstr = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, _id_phongban).AsEnumerable()
                                    .Where(x => !string.IsNullOrEmpty(x.Field<string>("iID_MaNganhMLNS")) && (_id_nganh == "-1" || x.Field<string>("MaNganh") == _id_nganh))
                                    .Select(x => x.Field<string>("iID_MaNganhMLNS")).Join(",");
                }
                else
                {
                    nganhstr = nganh.iID_MaNganhMLNS;
                }

                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                    nganh = nganhstr.ToParamString(),
                    phongban = PhienLamViec.iID_MaPhongBan.ToParamString(),
                    dvt = _dvt,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
        }

        #endregion


    }
}
