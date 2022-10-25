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
using VIETTEL.Helpers;


namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_PL02ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptNCSKT_PL02Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/PhuLuc/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/PhuLuc/rptNCSKT_PL02/rptNCSKT_PL02.xls";
        private const string _filePath_ng = "~/Areas/SKT/FlexcelForm/PhuLuc/rptNCSKT_PL02/rptNCSKT_PL02b.xls";

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _sktService = SKTService.Default;

        private string _id_phongban_dich;
        private string _id_nganh;
        private int _trinhky;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            var userb11 = "chinp,duongdt,thuypt,tuyetna".Split(',');
            if (userb11.Contains(User.Identity.Name))
            {
                string user;
                if (PhienLamViec.PhongBan.sKyHieu == "02" || PhienLamViec.PhongBan.sKyHieu == "11")
                {
                    user = null;
                }
                else
                {
                    user = Username;
                }            

                var nganhList = _nganSachService.Nganh_GetAll_ByPhongBan(PhienLamViec.iNamLamViec, string.Empty)
                   .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Chọn ngành --");
                var vm = new rptNCSKT_PL02ViewModel
                {
                    PhongBanList = PhienLamViec.iID_MaPhongBan == "11" ? GetPhongBanList(_nganSachService, "07,10") : GetPhongBanList(_nganSachService, "10"),
                    NganhList = nganhList,
                };

                var view = _viewPath + "rptNCSKT_PL02.cshtml";

                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult Print(
            string ext,
            string id_phongban,
            string id_nganh,
            int dvt = 1000,
            int trinhky = 0)
        {
            _trinhky = trinhky;
            _id_phongban_dich = id_phongban;
            _id_nganh = id_nganh;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var nganh = _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;
                var soPL = trinhky == 0 ? 2 : 1;

                var filename = $"Phụ_lục_{soPL}_Ngành_{nganh}.{ext}";
                return Print(xls[true], ext, filename);
            }
        }

        public ActionResult Ds_Nganh(string id_phongban)
        {            
            var list = _nganSachService.Nganh_GetAll_ByPhongBan(PhienLamViec.iNamLamViec, id_phongban)
                .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Chọn ngành --");

            return ToDropdownList(new VIETTEL.Models.ChecklistModel("id_nganh", list));
        }

        public JsonResult GhiChu(string nganh, string trinhky)
        {
            var Ten = $"ghichu_nhucau_sokiemtra_pl02_{PhienLamViec.iNamLamViec}_nganh_{nganh}_{trinhky}";
            var ghichu = _sktService.GetGhiChu(Username, Ten);
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GhiChu_Update(string nganh, string GhiChu, string trinhky)
        {
            var Ten = $"ghichu_nhucau_sokiemtra_pl02_{PhienLamViec.iNamLamViec}_nganh_{nganh}_{trinhky}";
            var success = _sktService.UpdateGhiChu(Username, Ten, GhiChu);
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private Dictionary<bool, ExcelFile> createReport()
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
                var filename = _trinhky == 0 ? _filePath : _filePath_ng;
                xls.Open(Server.MapPath(filename));

                var donvi = _id_nganh == "-1" ? "" : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_nganh).sTen;
                fr.UseCommonValue()
                    .SetValue(new
                    {
                        trinhky = _trinhky,
                        TenNganh = _id_nganh == "-1" ? "" : "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh,
                        PhongBan = _id_phongban_dich.IsEmpty() || _id_phongban_dich.Contains(",") ? "" : _nganSachService.GetPhongBanById(_id_phongban_dich).sMoTa,
                        h1 = $"Đơn vị tính: {_dvt.ToStringDvt()} ",
                        h2 = donvi,
                        nam = PhienLamViec.NamLamViec - 1,
                        namn = PhienLamViec.NamLamViec,
                        donvi = donvi.IsEmpty() ? "" : $"Đơn vị: {donvi}",
                        qd = _trinhky == 1 ? _sktService.GetCauHinhNamDuLieu(PhienLamViec.NamLamViec)["CVKHNS"].ToString() : _sktService.GetCauHinhNamDuLieu(PhienLamViec.NamLamViec)["QuyetDinh"].ToString(),
                        TieuDe1 = L("DTKT_TieuDe1", PhienLamViec.NamLamViec),
                        TieuDe2 = L("DTKT_QuyetDinh", PhienLamViec.NamLamViec - 1),
                    })
                    .UseChuKyForController(this.ControllerName())
                    .UseForm(this)
                    .Run(xls);

                var count = xls.TotalPageCount();
                if (count > 1)
                {
                    xls.AddPageFirstPage();
                }
                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> LoadData(FlexCelReport fr)
        {
            var data = getTable();

            if (data == null)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var sum = _dvt * data.AsEnumerable().Sum(r => r.Field<double>("TuChi", 0));
                fr.SetValue("Tien", sum.ToStringMoney());
                _sktService.FillDataTable_NC(fr, data);

                var dtGhiChu = getGhiChu();

                //dtGhiChu = getTable_GhiChu();
                fr.AddTable("dtGhiChu", dtGhiChu);
                //var dtNganhGhiChu = dtGhiChu.SelectDistinct("Nganh", "Nganh,TenNganh");
                //fr.AddTable("Nganh", dtNganhGhiChu);
                //fr.AddRelationship("Nganh", "dtGhiChu", "Nganh".Split(','), "Nganh".Split(','));

                if (dtGhiChu.Rows.Count >=1 && !string.IsNullOrEmpty(dtGhiChu.Rows[0][0].ToString()))
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

        private DataTable getTable()
        {
            var sql = _trinhky == 0 ? "sp_ncskt_report_pl02" : "sp_ncskt_report_pl02_ng";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                var dt = cmd.GetDataset(
                    new
                    {
                        nam = PhienLamViec.NamLamViec,
                        id_phongban_dich = _id_phongban_dich.ToParamString(),
                        nganh = _id_nganh.ToParamString(),
                        dvt = _dvt
                    }).Tables[0];

                return dt;
            }
        }

        private DataTable getGhiChu()
        {
            var Ten = $"ghichu_nhucau_sokiemtra_pl02_{PhienLamViec.iNamLamViec}_nganh_{_id_nganh}_{_trinhky}";
            var ghichu = _sktService.GetGhiChu(Username, Ten);

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

        private DataTable getTable_GhiChu()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable("sp_ncskt_report_ghichu", new
                {
                    nam = PhienLamViec.iNamLamViec,
                    phongban = "02",
                    phongbandich = _id_phongban_dich.ToParamString(),
                    donvi = _id_nganh.ToParamString(),
                    loai = 2,
                }, CommandType.StoredProcedure);
                return dt;
            }
        }
        #endregion
    }
}
