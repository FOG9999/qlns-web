using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.Ajax.Utilities;
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
    public class rptNCSKT_PL03ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
        public SelectList MucList { get; set; }
        public int LoaiBC { get; set; }
    }
    public class rptNCSKT_PL03Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/PhuLuc/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/PhuLuc/rptNCSKT_PL03/rptNCSKT_PL03.xls";
        //private string _filePath_ng = "~/Areas/SKT/FlexcelForm/PhuLuc/rptNCSKT_PL03/rptNCSKT_PL03_ng.xls";
        private const string _filePath_ng = "~/Areas/SKT/FlexcelForm/PhuLuc/rptNCSKT_PL03/rptNCSKT_PL03b.xls";

        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _dvt = 1000;
        private int _loai = 4;
        private int _loaiBaoCao = 1;
        private int _to = 1;
        private int _columnsCount = 9;
        private int _nganhCount = 1;

        public ActionResult Index(int LoaiBaoCao)
        {
            var userb11 = "chinp,duongdt,thuypt,tuyetna".Split(',');
            if (userb11.Contains(User.Identity.Name))
            {
                var nganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username)
                                            .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
                var mucList = SharedModels.GetMucChiTietNS().Select("Loai in ('2')").CopyToDataTable().ToSelectList("Loai","Ten");

                var vm = new rptNCSKT_PL03ViewModel
                {
                    LoaiBC = LoaiBaoCao,
                    NganhList = nganhList,
                    PhongBanList = PhienLamViec.iID_MaPhongBan == "11" ? GetPhongBanList(_nganSachService, "07,10") : GetPhongBanList(_nganSachService, "10"),
                    MucList = mucList,
                };

                var view = _viewPath + "rptNCSKT_PL03.cshtml";

                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public JsonResult Ds_To(string id_nganh, string id_phongban = null, int loai = 4, int loaiBaoCao = 1)
        {
            var sql = "sp_ncskt_report_pl03";
            if (loai == 2 || loaiBaoCao == 0)
                sql = "sp_ncskt_report_pl03_m";
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                    nganh = id_nganh.ToParamString(),
                    dvt = _dvt,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                var dtNganh = dt.SelectDistinct("Nganh", "Nganh,KyHieu");
                var dtNganhTo = dt.SelectDistinct("Nganh", "Nganh");
                var rowsCountX = dtNganhTo.Rows.Count;
                var countNT = 0;
                for (int i = 0; i < rowsCountX; i++)
                {
                    var ng = dtNganhTo.Rows[i]["Nganh"].ToString();
                    var ngcount = dtNganh.Select($"Nganh = '{ng}'").Count();
                    if (ngcount > 1)
                    {
                        countNT++;
                    }
                }
                if (loai == 4)
                    dtNganh = dt.SelectDistinct("Nganh", "Nganh");
                return ds_ToIn(dtNganh.Rows.Count == 0 ? 0 : loai == 4 ? ((dtNganh.Rows.Count > 1) ? dtNganh.Rows.Count + 1 : dtNganh.Rows.Count) : ((rowsCountX > 1) ? (countNT + dtNganh.Rows.Count + 1) : (countNT + dtNganh.Rows.Count)), _columnsCount);
            }            
        }

        public ActionResult Ds_Nganh(string id_phongban)
        {
            var list = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id_phongban)
                                          .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            return ToDropdownList(new VIETTEL.Models.ChecklistModel("id_nganh", list));
        }

        public JsonResult GhiChu(string nganh, string trinhky)
        {
            var Ten = $"ghichu_nhucau_sokiemtra_pl03_{PhienLamViec.iNamLamViec}_nganh_{nganh}_{trinhky}";
            var ghichu = _sKTService.GetGhiChu(Username, Ten);
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GhiChu_Update(string nganh, string GhiChu, string trinhky)
        {
            var Ten = $"ghichu_nhucau_sokiemtra_pl03_{PhienLamViec.iNamLamViec}_nganh_{nganh}_{trinhky}";
            var success = _sKTService.UpdateGhiChu(Username, Ten, GhiChu);
            return Json(success, JsonRequestBehavior.AllowGet);
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
            _id_phongban = id_phongban;
            _id_nganh = id_nganh;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _loaiBaoCao = loaiBaoCao;
            _loai = loai;
            _to = to;
            _filePath = loaiBaoCao == 1 ? _filePath : _filePath_ng;            

            var xls = CreateReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var nganh = _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;
                var soPL = loaiBaoCao == 1 ? 3 : 2;

                var filename = $"Phụ_lục_{soPL}_Ngành_{nganh}_Tờ_{to}.{ext}";
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

                var donvi = _id_nganh == "-1" ? "" : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_nganh).sTen;

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        headerl = donvi,
                        headerr = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                        nganh = _id_nganh == "-1" ? "" : "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh,
                        donvi = donvi.IsEmpty() ? "" : $"Đơn vị: {donvi}",
                        QuyetDinh = _loaiBaoCao == 1 ? L("SKT_QuyetDinh") : L("SKT_KHNS"),
                        qd = _loaiBaoCao == 1 ? _sKTService.GetCauHinhNamDuLieu(PhienLamViec.NamLamViec)["QuyetDinh"].ToString() : _sKTService.GetCauHinhNamDuLieu(PhienLamViec.NamLamViec)["CVKHNS"].ToString(),
                        nam = PhienLamViec.NamLamViec,
                        namn = PhienLamViec.NamLamViec - 1,
                        loai = _loai,
                    })
                    .UseChuKyForController(this.ControllerName())
                    .UseForm(this)
                    .Run(xls, _to);
                if(_to == 1 && _loai == 2 && _nganhCount > 1)
                {                   
                    xls.MergeCells(8, 4, 9, 4);
                    xls.MergeH(8, 5, 9);
                }
                else
                {
                    xls.MergeH(8, 4, 9);
                }

                var count = xls.TotalPageCount();
                if (_to != 1)
                {
                    if (count > 1)
                    {
                        xls.ClearDiffFirstPage();
                    }
                } else
                {
                    if (count > 1)
                    {
                        xls.AddPageFirstPage();
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
                var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
                var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh,KyHieu,MoTa,STTBC");
                var dtNganhTo = dt.SelectDistinct("Nganh", "Nganh,TenNganh");

                for (int i = 0; i < _columnsCount; i++)
                {
                    data.Columns.Add(new DataColumn($"C{i + 1}", typeof(double)));
                }

                _nganhCount = dtNganhTo.Rows.Count;
                for (int i = 0; i < _nganhCount; i++)
                {
                    var ng = dtNganhTo.Rows[i]["Nganh"].ToString();
                    var ngcount = dtNganh.Select($"Nganh = '{ng}'").Count();
                    if (ngcount > 1)
                    {
                        var r1 = dtNganh.NewRow();
                        r1["TenNganh"] = dtNganhTo.Rows[i]["TenNganh"];
                        r1["Nganh"] = dtNganhTo.Rows[i]["Nganh"];
                        r1["KyHieu"] = "";
                        r1["STTBC"] = "";
                        r1["MoTa"] = "(+)";
                        dtNganh.Rows.InsertAt(r1, 0);
                    }
                }

                var columns = new List<DataRow>();                
                
                if (_nganhCount > 1) { 
                    var r1 = dtNganh.NewRow();
                    r1["TenNganh"] = "Tổng cộng";
                    r1["Nganh"] = "";
                    r1["KyHieu"] = "";
                    r1["MoTa"] = "";
                    dtNganh.Rows.InsertAt(r1, 0);
                }

                var dtXList = dtNganh.AsEnumerable().OrderBy(x => x.Field<string>("Nganh"))
                            .ThenBy(x => x.Field<string>("STTBC"));

                columns = _to == 1 ?
                    dtXList.AsEnumerable().Take(_columnsCount).ToList() :
                    dtXList.AsEnumerable().Skip((_to - 1) * _columnsCount).Take(_columnsCount).ToList();

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

                var dtGhiChu = getGhiChu();

                fr.AddTable("dtGhiChu", dtGhiChu);

                if (dtGhiChu.Rows.Count >= 1 && !string.IsNullOrEmpty(dtGhiChu.Rows[0][0].ToString()))
                {
                    fr.SetValue("?GhiChu", 1);
                }
                else
                {
                    fr.SetValue("?GhiChu", 0);
                }

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable GetTable()
        {
            var sql = _loaiBaoCao == 1 ? "sp_ncskt_report_pl03_m" : "sp_ncskt_report_pl03_m_ng";
            //if (_loai == 2 || _loaiBaoCao == 0)
            //    sql = "sp_ncskt_report_pl03_m";
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                    nganh = _id_nganh.ToParamString(),
                    dvt = _dvt,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
        }

        private DataTable getGhiChu()
        {
            var Ten = $"ghichu_nhucau_sokiemtra_pl03_{PhienLamViec.iNamLamViec}_nganh_{_id_nganh}_{_loaiBaoCao}";
            var ghichu = _sKTService.GetGhiChu(Username, Ten);

            var dt = new DataTable();
            dt.Columns.Add("sGhiChu");

            var words = ghichu.Split(new string[] { "\n", "&#10;" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            words.ForEach(w =>
            {
                var lines = w.Replace("  ", " ");

                var row = dt.NewRow();
                row[0] = lines;
                dt.Rows.Add(row);

            });

            return dt;
        }
        #endregion


    }
}
