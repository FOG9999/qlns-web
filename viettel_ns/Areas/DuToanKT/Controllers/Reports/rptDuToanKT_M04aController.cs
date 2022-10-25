using DapperExtensions;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptDuToanKT_M04aViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M04aController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04a.xls";
        private const string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04a_trinhky.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _loaiBaoCao;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;
            string user;
            if (PhienLamViec.PhongBan.sKyHieu == "02" || PhienLamViec.PhongBan.sKyHieu == "11")
            {
                user = null;
            }
            else
            {
                user = Username;
            }
            var nganhList = _duToanKTService.GetNganhBD(PhienLamViec.iNamLamViec, PhienLamViec.PhongBan.sKyHieu, PhienLamViec.PhongBan.sKyHieu, user)
                                            .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            var vm = new rptDuToanKT_M04aViewModel
            {
                iNamLamViec = iNamLamViec,
                NganhList = nganhList,
                PhongBanList = GetPhongBanList(_nganSachService),
            };

            return View(view, vm);
        }

        public ActionResult Print(
            string ext,
            string id_phongban,
            string id_nganh,
            int dvt = 1000,
            int loaiBaoCao = 0,
            string id_chungTu = "")
        {
            if (!string.IsNullOrEmpty(id_chungTu))
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    var entity = conn.Get<DTKT_ChungTu>(id_chungTu);
                    _id_phongban = entity.Id_PhongBanDich;
                    _id_nganh = entity.Id_DonVi;
                }
            }
            else
            {
                _loaiBaoCao = loaiBaoCao;
                _id_phongban = id_phongban;
                _id_nganh = id_nganh;
            }
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _filePath = _loaiBaoCao == 0 ? _filePath : _filePath_trinhky;

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

        public ActionResult Ds_Nganh(string id_phongban)
        {
            var list = _duToanKTService.GetNganhBD(PhienLamViec.iNamLamViec, PhienLamViec.PhongBan.sKyHieu, id_phongban, Username)
                                          .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            return ToDropdownList(new VIETTEL.Models.ChecklistModel("id_nganh", list));
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
                fr.SetValue("dvt", _dvt.ToStringDvt());
                fr.SetValue("Nam", int.Parse(PhienLamViec.iNamLamViec) + 1);
                xls.Open(Server.MapPath(_filePath));

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        trinhky = _loaiBaoCao,
                        TenPhongBan = _nganSachService.CheckParam_PhongBan(_id_phongban).IsEmpty() ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                        header = _id_nganh == "-1" ? "" : "Đơn vị: " + _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_nganh).sTen,

                        TieuDe1 = L("DTKT_TieuDe1", PhienLamViec.iNamLamViec.ToValue<int>() + 1),
                        QuyetDinh = L("DTKT_QuyetDinh"),
                    })
                    .UseChuKyForController(this.ControllerName())
                    .UseForm(this)
                    .Run(xls);

                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var data = getTable(PhienLamViec.iNamLamViec, _id_phongban, _id_nganh, Username);
            if (data.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var dtNganh = data.SelectDistinct("Nganh", "Ng,TenDonVi,Nganh,TenNganh");
                var dtDonVi = dtNganh.SelectDistinct("DonVi", "Ng,TenDonVi");

                _duToanKTService.AddOrdinalsNum(dtDonVi, 2);
                _duToanKTService.AddOrdinalsNum(dtNganh, 3, "Ng");
                _duToanKTService.AddOrdinalsNum(data, 4);
                data.CountDistinctRow("Ng,Nganh");

                var dtDongTrang = new DataTable();
                dtDongTrang.Columns.Add("dongTrang");

                var emptyCount = _loaiBaoCao == 0 ? 10 : 5;
                if (data.Rows.Count < emptyCount)
                {
                    var delta = emptyCount - data.Rows.Count;
                    for (int i = 0; i < delta; i++)
                    {
                        var row = dtDongTrang.NewRow();
                        dtDongTrang.Rows.Add(row);
                    }
                }

                fr.AddTable("ChiTiet", data);
                fr.AddTable("Nganh", dtNganh);
                fr.AddTable("DonVi", dtDonVi);
                fr.AddTable("DongTrang", dtDongTrang);
                fr.AddRelationship("DonVi", "Nganh", "Ng,TenDonVi".Split(','), "Ng,TenDonVi".Split(','));
                fr.AddRelationship("Nganh", "ChiTiet", "Ng,TenDonVi,Nganh,TenNganh".Split(','), "Ng,TenDonVi,Nganh,TenNganh".Split(','));
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable getTable(string nam, string id_phongban_dich, string nganhBaoDam, string username)
        {
            username = _nganSachService.CheckParam_Username(username, PhienLamViec.PhongBan.sKyHieu);

            var sql = FileHelpers.GetSqlQuery("dtkt_report_m04a.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                var nganh = _nganSachService.Nganh_Get(nganhBaoDam, Username);

                if (nganh == null)
                {
                    if (PhienLamViec.PhongBan.sKyHieu == "02" || PhienLamViec.PhongBan.sKyHieu == "11")
                    {
                        cmd.AddParams(new
                        {
                            nganh = _duToanKTService.GetNganhBD(nam, PhienLamViec.PhongBan.sKyHieu, _id_phongban).AsEnumerable()
                                    .Where(x => !string.IsNullOrEmpty(x.Field<string>("iID_MaNganhMLNS")) && (nganhBaoDam == "-1" || x.Field<string>("MaNganh") == nganhBaoDam))
                                    .Select(x => x.Field<string>("iID_MaNganhMLNS")).Join(",")
                        });
                    }
                    else
                    {
                        cmd.AddParams(new
                        {
                            nganh = _duToanKTService.GetNganhBD(nam, _id_phongban, _id_phongban, Username).AsEnumerable()
                                    .Where(x => !string.IsNullOrEmpty(x.Field<string>("iID_MaNganhMLNS")))
                                    .Select(x => x.Field<string>("iID_MaNganhMLNS")).Join(",")
                        });
                    }
                }
                else
                {
                    cmd.AddParams(new
                    {
                        nganh = nganh.iID_MaNganhMLNS
                    });
                }


                cmd.AddParams(new
                {
                    nam,
                    id_phongban_dich = id_phongban_dich.ToParamString(),
                    username = username.ToParamString(),
                    dvt = _dvt,
                });

                var dt = cmd.GetTable();
                return dt;
            }
        }

        #endregion
    }
}
