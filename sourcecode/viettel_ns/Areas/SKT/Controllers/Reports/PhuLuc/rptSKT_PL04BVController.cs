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
    public class rptSKT_PL04BVController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/PhuLuc/";
        private string _filePath = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL04_Bia_BV.xls";
        private string _filePath_trinhky = "~/Report_ExcelFrom/SKT/TrinhKy/rptSKT_PL04_Bia_TrinhKy.xls";
        private const string _filePath_ct = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL04_CT_BV.xls";
        private const string _filePath_ct_trinhky = "~/Report_ExcelFrom/SKT/TrinhKy/rptSKT_PL04_CT_TrinhKy.xls";


        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private int _dvt = 1000;
        private int _loaiBC = 1;
        private int _loaiBaoCao = 1;
        private int _loai = 1;
        private int _muc = 1;
        private int _to = 1;
        private int _columnsCount = 9;
        private int _countTD3 = 0;

        public ActionResult Index(int LoaiBaoCao)
        {
            var dt = SharedModels.GetLoaiBaoCao();
            var mucList = SharedModels.GetMucChiTietNS().Select("Loai in ('2','4')").CopyToDataTable().ToSelectList("Loai", "Ten");

            var vm = new rptSKT_PL04ViewModel
            {
                LoaiBC = LoaiBaoCao,
                LoaiList = dt.ToSelectList("Loai", "Ten"),
                MucList = mucList,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        public JsonResult Ds_To(int loaiBC = 1, int muc = 4)
        {
            var sql = "skt_report_pl04b_bv";
            if (muc == 2)
                sql = "skt_report_pl04b_m_bv";
            
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                    phongban = PhienLamViec.iID_MaPhongBan == "11" ? loaiBC == 1 ? "07,10" : "02" : PhienLamViec.iID_MaPhongBan.ToParamString(),
                    dvt = _dvt,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh,KyHieu,MoTa");
                var dtMuc = dt.SelectDistinct("Nganh", "Nganh,TenNganh");
                var count = 0;
                for (int i = 0; i < dtMuc.Rows.Count; i++)
                {
                    var r = dtMuc.Rows[i];
                    var num = dtNganh.AsEnumerable().ToList()
                        .Where(x => !string.IsNullOrEmpty(x.Field<string>("Nganh")) && x.Field<string>("Nganh") == r["Nganh"].ToString())
                        .Select(x => x.Field<string>("Nganh")).Count();
                    if (num > 1)
                    {
                        count++;
                    }
                }
                return ds_ToIn(dtNganh.Rows.Count == 0 ? 0 : dtNganh.Rows.Count + count + 1, _columnsCount);
            }            
        }

        public ActionResult Print(
            string ext,
            int loaiBaoCao,
            int to = 1,
            int loaiBC = 1,
            int muc = 1,
            int loai = 1,
            int dvt = 1000)
        {
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _loaiBC = loaiBC;
            _loaiBaoCao = loaiBaoCao;
            _loai = loai;
            _muc = muc;
            _to = to;
            _filePath = GetFilePath(_loaiBaoCao, _loai);
            var filename = loai != 2 ? "BDKT_GBV_Bìa" + "." + ext : "BDKT_GBV_ChiTiet_Tờ_" + to + "." + ext;

            var xls = CreateReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                return Print(xls[true], ext, filename);
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
                var headerr = "";
                if (_loai == 1)
                {
                    headerr = $"Đơn vị tính: {_dvt.ToStringDvt()}";
                }
                else
                {
                    headerr = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}";
                }

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        headerr,
                        headerl = _loaiBaoCao == 1 ? "Đơn vị: " + _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, "99").sTen : _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
                        nganh = "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, "99").sTenNganh,
                        QuyetDinh = _loaiBaoCao == 1 ? L("SKT_QuyetDinh_2020") : "",
                        muc = _muc,
                        nam = PhienLamViec.NamLamViec,
                    })
                    .UseChuKyForController(this.ControllerName())
                    .UseForm(this)
                    .Run(xls, _to);
                if (_loai == 2)
                { 
                    if (_to == 1 && _muc == 2)
                    {                      
                        xls.MergeCells(8, 4, 9, 4);
                        xls.MergeH(8, 5, 9);
                    }
                    else
                        xls.MergeH(8, 4, 9);
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
                if (_loai == 2) {
                    var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
                    var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh,KyHieu,MoTa");
                    var dtMuc = dt.SelectDistinct("Nganh", "Nganh,TenNganh");

                    for (int i = 0; i < _columnsCount; i++)
                    {
                        data.Columns.Add(new DataColumn($"C{i + 1}", typeof(double)));
                    }

                    for (int i = 0; i < dtMuc.Rows.Count; i++)
                    {
                        var r = dtMuc.Rows[i];
                        var count = dtNganh.AsEnumerable().ToList()
                            .Where(x => !string.IsNullOrEmpty(x.Field<string>("Nganh")) && x.Field<string>("Nganh") == r["Nganh"].ToString())
                            .Select(x => x.Field<string>("Nganh")).Count();
                        if (count > 1)
                        {
                            var rN = dtNganh.NewRow();
                            rN["TenNganh"] = dtMuc.Rows[i]["TenNganh"];
                            rN["Nganh"] = dtMuc.Rows[i]["Nganh"];
                            rN["KyHieu"] = "";
                            rN["MoTa"] = "(+)";
                            dtNganh.Rows.InsertAt(rN, 0);
                        }
                    }
                    var view = dtNganh.DefaultView;
                    view.Sort = "Nganh,KyHieu";
                    dtNganh = view.ToTable();

                    var r1 = dtNganh.NewRow();
                    r1["TenNganh"] = "Tổng cộng";
                    r1["Nganh"] = "";
                    r1["KyHieu"] = "";
                    r1["MoTa"] = "";
                    dtNganh.Rows.InsertAt(r1, 0);

                    var columns = new List<DataRow>();

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
                                            (string.IsNullOrWhiteSpace(columns[i].Field<string>("KyHieu")) || x.Field<string>("KyHieu") == columns[i].Field<string>("KyHieu")))
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
                } else
                {
                    var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh");
                    _sKTService.AddOrdinalsNum(dtNganh, 3);
                    fr.AddTable("Nganh", dtNganh);
                    fr.AddTable("ChiTiet", dt);
                    fr.AddRelationship("Nganh", "ChiTiet", "Nganh,TenNganh".Split(','), "Nganh,TenNganh".Split(','));
                }
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable GetTable()
        {
            var sql = "";
            if (_loai == 1)
            {
                if (_muc == 4)
                    sql = "skt_report_pl04a_bv";
                else
                    sql = "skt_report_pl04a_m_bv";
            } else
            {
                if (_muc == 4)
                    sql = "skt_report_pl04b_bv";
                else
                    sql = "skt_report_pl04b_m_bv";
            }
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                    phongban = PhienLamViec.iID_MaPhongBan == "11" ? _loaiBC == 1 ? "07,10" : "02" : PhienLamViec.iID_MaPhongBan.ToParamString(),
                    dvt = _dvt,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
        }

        private string GetFilePath(int loaiBaoCao, int loai)
        {
            if (loaiBaoCao == 2)
            {
                if (loai == 1)
                {
                    return _filePath_trinhky;
                } else
                {
                    return _filePath_ct_trinhky;
                }
            }
            else
            {
                if (loai == 1)
                {
                    return _filePath;
                }
                else
                {
                    return _filePath_ct;
                }
            }
        }

        #endregion


    }
}
