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
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_PL01ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
    public class rptNCSKT_PL01Controller : FlexcelReportController
    {
        #region var def

        public string _viewPath = "~/Areas/SKT/Views/Reports/PhuLuc/";
        private const string _filePath = "~/Areas/SKT/FlexcelForm/PhuLuc/rptNCSKT_PL01/rptNCSKT_PL01.xls";
        private const string _filePath_tk = "~/Areas/SKT/FlexcelForm/PhuLuc/rptNCSKT_PL01/rptNCSKT_PL01b.xls";

        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_donvi;
        private string _id_phongban_dich;
        private int _dvt = 1000;
        private int _loaiBaoCao = 0;

        public ActionResult Index()
        {
            var userb11 = "duongdt,thuypt,tuyetna".Split(',');
            if (userb11.Contains(User.Identity.Name)) { 
                var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

                var data = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec);
                var vm = new rptNCSKT_PL01ViewModel
                {
                    PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Tất cả phòng ban --"),
                    DonViList = data.ToSelectList("iID_MaDonVi", "sMoTa"),
                };

                var view = _viewPath + "rptNCSKT_PL01.cshtml";
                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }


        #endregion

        #region public methods
        public JsonResult Ds_DonVi(string id_phongban)
        {
            var data = _sKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 4, PhienLamViec.iID_MaDonVi, "02", id_phongban);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten"));
            return ToCheckboxList(vm);
        }

        public JsonResult GhiChu(string id_donvi, string loai)
        {
            var loaiBC = loai == "0" ? "dvtk" : loai == "1" ? "dvdt" : "dvdn";
            var Ten = $"ghichu_nhucau_sokiemtra_pl01_{PhienLamViec.iNamLamViec}_{loaiBC}_{id_donvi}";
            var ghichu = _sKTService.GetGhiChu(Username, Ten);
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GhiChu_Update(string id_donvi, string GhiChu, string loai)
        {
            var loaiBC = loai == "0" ? "dvtk" : loai == "1" ? "dvdt" : "dvdn";
            var Ten = $"ghichu_nhucau_sokiemtra_pl01_{PhienLamViec.iNamLamViec}_{loaiBC}_{id_donvi}";
            var success = _sKTService.UpdateGhiChu(Username, Ten, GhiChu);
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Print(
            string Id_DonVi,
            string id_phongban_dich,
            int loaiBaoCao = 0,
            string ext = "pdf")
        {
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            this._id_donvi = Id_DonVi;
            this._id_phongban_dich = id_phongban_dich;
            _loaiBaoCao = loaiBaoCao;

            var xls = CreateReport();
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var donvi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;
                var filename = $"Phụ_lục_1_{donvi}.{ext}";
                return Print(xls[true], ext, filename);
            }
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>

        public DataTable getTable()
        {
            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand("sp_ncskt_report_pl01", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                var dt = cmd.GetDataset(
                    new
                    {
                        nam = PhienLamViec.NamLamViec,
                        id_phongban_dich = _id_phongban_dich.ToParamString(),
                        id_donvi = _id_donvi.ToParamString(),
                        dvt = _dvt
                    }).Tables[0];

                return dt;
            }

            #endregion
        }

        #endregion

        #region private methods
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
                var filename = _loaiBaoCao == 0 ? _filePath_tk : _filePath;
                xls.Open(Server.MapPath(filename));

                var tenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        TieuDe1 = $"Thông báo số kiểm tra dự toán ngân sách năm {PhienLamViec.NamLamViec}",
                        TieuDe2 = _id_phongban_dich.StartsWith("06") ? "Khối Doanh nghiệp" : (_id_donvi.Length <= 3 ? "Khối Dự toán" : "Nội dung chi không thực hiện tự chủ"),
                        namn = PhienLamViec.NamLamViec - 1,
                        qd = _sKTService.GetCauHinhNamDuLieu(PhienLamViec.NamLamViec)["QuyetDinh"].ToString(),
                        DonVi = tenDonVi,
                        h1 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                        h2 = tenDonVi,
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
                _sKTService.FillDataTable_NC(fr, data);

                var dtGhiChu = getGhiChu();
                //dtGhiChu = getTable_GhiChu();
                fr.AddTable("dtGhiChu", dtGhiChu);
                //var dtNganhGhiChu = dtGhiChu.SelectDistinct("Nganh", "Nganh,TenNganh");
                //fr.AddTable("Nganh", dtNganhGhiChu);
                //fr.AddRelationship("Nganh", "dtGhiChu", "Nganh".Split(','), "Nganh".Split(','));

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

        private DataTable getGhiChu()
        {
            var loaiBC = _loaiBaoCao == 0 ? "dvtk" : _loaiBaoCao == 1 ? "dvdt" : "dvdn";
            var Ten = $"ghichu_nhucau_sokiemtra_pl01_{PhienLamViec.iNamLamViec}_{loaiBC}_{_id_donvi}";
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

        private DataTable getTable_GhiChu()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable("sp_ncskt_report_ghichu", new
                {
                    nam = PhienLamViec.iNamLamViec,
                    phongban = "02",
                    phongbandich = _id_phongban_dich.ToParamString(),
                    donvi = _id_donvi.ToParamString(),
                    loai = 1,
                }, CommandType.StoredProcedure);
                return dt;
            }
        }
        #endregion
    }
}
