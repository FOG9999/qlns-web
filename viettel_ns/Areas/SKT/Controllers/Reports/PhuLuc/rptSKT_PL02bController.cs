using DapperExtensions;
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
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;


namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptSKT_PL02Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/PhuLuc/";
        private string _filePath = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL02.xls";
        private const string _filePath_trinhky = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL02_trinhky.xls";

        //private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _sktService = SKTService.Default;

        private string _id_phongban;
        private string _id_phongban_dich;
        private string _id_nganh;
        private int _loaiBaoCao;
        private int _trinhky;
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
            //var nganhList = _duToanKTService.GetNganhBD(PhienLamViec.iNamLamViec, PhienLamViec.PhongBan.sKyHieu, PhienLamViec.PhongBan.sKyHieu, user)
            //                                .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");

            var nganhList = _nganSachService.Nganh_GetAll_ByPhongBan(PhienLamViec.iNamLamViec, string.Empty)
               .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Chọn ngành --");
            var vm = new rptSKT_ReportModel
            {
                PhongBanList = GetPhongBanList(_nganSachService, "07,10"),
                NganhList = nganhList,
            };

            return View(view, vm);
        }

        public ActionResult Print(
            string ext,
            string id_phongban,
            string id_nganh,
            int dvt = 1000,
            int loaiBaoCao = 0,
            int trinhky = 0)
        {

            _loaiBaoCao = loaiBaoCao;
            _trinhky = trinhky;
            _id_phongban = id_phongban;
            _id_nganh = id_nganh;


            if (loaiBaoCao == 0)
            {
                _id_phongban = "02";
                _id_phongban_dich = "06,07,08,10";
            }
            else
            {
                if (_id_phongban.IsEmpty("-1"))
                    _id_phongban = "06,07,10";

                _id_phongban_dich = _id_phongban;
            }


            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _filePath = trinhky == 0 ? _filePath : _filePath_trinhky;

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
            //var list = _duToanKTService.GetNganhBD(PhienLamViec.iNamLamViec, PhienLamViec.PhongBan.sKyHieu, id_phongban, Username)
            //                              .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");

            var list = _nganSachService.Nganh_GetAll_ByPhongBan(PhienLamViec.iNamLamViec, id_phongban)
                .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Chọn ngành --");

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
                var filename = _trinhky == 0 ? _filePath : _filePath_trinhky;
                xls.Open(Server.MapPath(filename));

                var donvi = _id_nganh == "-1" ? "" : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_nganh).sTen;
                fr.UseCommonValue()
                    .SetValue(new
                    {
                        trinhky = _loaiBaoCao,
                        //PhongBan = _nganSachService.CheckParam_PhongBan(_id_phongban).IsEmpty() ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                        PhongBan = _id_phongban.IsEmpty() || _id_phongban.Contains(",") ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                        h1 = $"Đơn vị tính: {_dvt.ToStringDvt()} ",
                        h2 = donvi,
                        donvi = donvi.IsEmpty() ? "" : $"Đơn vị: {donvi}",
                        TieuDe1 = L("DTKT_TieuDe1", PhienLamViec.NamLamViec),
                        TieuDe2 = L("DTKT_QuyetDinh", PhienLamViec.NamLamViec - 1),
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

        private Dictionary<bool, FlexCelReport> LoadData(FlexCelReport fr)
        {
            var data = getTable(_id_phongban, _id_phongban_dich, _id_nganh);

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
                _sktService.FillDataTable_SKT(fr, data);

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }

        }

        //private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        //{
        //    var data = getTable(PhienLamViec.iNamLamViec, _id_phongban, _id_nganh, Username);
        //    if (data.Rows.Count == 0)
        //    {
        //        return new Dictionary<bool, FlexCelReport>
        //        {
        //            { false, fr }
        //        };
        //    }
        //    else
        //    {
        //        var dtNganh = data.SelectDistinct("Nganh", "Ng,TenDonVi,Nganh,TenNganh");
        //        var dtDonVi = dtNganh.SelectDistinct("DonVi", "Ng,TenDonVi");

        //        _duToanKTService.AddOrdinalsNum(dtDonVi, 2);
        //        _duToanKTService.AddOrdinalsNum(dtNganh, 3, "Ng");
        //        _duToanKTService.AddOrdinalsNum(data, 4);
        //        data.CountDistinctRow("Ng,Nganh");

        //        var dtDongTrang = new DataTable();
        //        dtDongTrang.Columns.Add("dongTrang");

        //        var emptyCount = _loaiBaoCao == 0 ? 10 : 5;
        //        if (data.Rows.Count < emptyCount)
        //        {
        //            var delta = emptyCount - data.Rows.Count;
        //            for (int i = 0; i < delta; i++)
        //            {
        //                var row = dtDongTrang.NewRow();
        //                dtDongTrang.Rows.Add(row);
        //            }
        //        }

        //        fr.AddTable("ChiTiet", data);
        //        fr.AddTable("Nganh", dtNganh);
        //        fr.AddTable("DonVi", dtDonVi);
        //        fr.AddTable("DongTrang", dtDongTrang);
        //        fr.AddRelationship("DonVi", "Nganh", "Ng,TenDonVi".Split(','), "Ng,TenDonVi".Split(','));
        //        fr.AddRelationship("Nganh", "ChiTiet", "Ng,TenDonVi,Nganh,TenNganh".Split(','), "Ng,TenDonVi,Nganh,TenNganh".Split(','));
        //        return new Dictionary<bool, FlexCelReport>
        //        {
        //            { true, fr }
        //        };
        //    }
        //}

        private DataTable getTable(string id_phongban, string id_phongban_dich, string nganhBaoDam)
        {
            //username = _nganSachService.CheckParam_Username(username, PhienLamViec.PhongBan.sKyHieu);

            var sql = FileHelpers.GetSqlQuery("skt_report_pl02.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        PhienLamViec.NamLamViec,
                        id_phongban = id_phongban.ToParamString(),
                        id_phongban_dich = id_phongban_dich.ToParamString(),
                        id_donvi = nganhBaoDam.ToParamString(),
                        dvt = _dvt
                    });

                return dt;
            }

            //using (var conn = _connectionFactory.GetConnection())
            //using (var cmd = new SqlCommand(sql, conn))
            //{
            //var nganh = _nganSachService.Nganh_Get(nganhBaoDam, Username);

            //if (nganh == null)
            //{
            //    if (PhienLamViec.PhongBan.sKyHieu == "02" || PhienLamViec.PhongBan.sKyHieu == "11")
            //    {
            //        cmd.AddParams(new
            //        {
            //            nganh = _duToanKTService.GetNganhBD(nam, PhienLamViec.PhongBan.sKyHieu, _id_phongban).AsEnumerable()
            //                    .Where(x => !string.IsNullOrEmpty(x.Field<string>("iID_MaNganhMLNS")) && (nganhBaoDam == "-1" || x.Field<string>("MaNganh") == nganhBaoDam))
            //                    .Select(x => x.Field<string>("iID_MaNganhMLNS")).Join(",")
            //        });
            //    }
            //    else
            //    {
            //        cmd.AddParams(new
            //        {
            //            nganh = _duToanKTService.GetNganhBD(nam, _id_phongban, _id_phongban, Username).AsEnumerable()
            //                    .Where(x => !string.IsNullOrEmpty(x.Field<string>("iID_MaNganhMLNS")))
            //                    .Select(x => x.Field<string>("iID_MaNganhMLNS")).Join(",")
            //        });
            //    }
            //}
            //else
            //{
            //    cmd.AddParams(new
            //    {
            //        nganh = nganh.iID_MaNganhMLNS
            //    });
            //}


            //cmd.AddParams(new
            //{
            //    nam,
            //    id_phongban_dich = id_phongban_dich.ToParamString(),
            //    username = username.ToParamString(),
            //    dvt = _dvt,
            //});

            //var dt = cmd.GetTable();
            //return dt;


            //}
        }

        #endregion
    }
}
