using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptSKT_TrinhKy_K1N_fixController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/TrinhKy/";
        private string _filePath = "~/Report_ExcelFrom/SKT/TrinhKy/rptTrinhKy_K1N.xls";

        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            var view = _viewPath + "rptTrinhKy_K1N.cshtml";
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
            var nganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, PhienLamViec.iID_MaPhongBan)
                                            .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            var vm = new rptTrinhKy_K1NViewModel
            {
                NganhList = nganhList,
                PhongBanList = GetPhongBanList(_nganSachService),
            };

            return View(view, vm);
        }

        public ActionResult Print(
            string ext,
            string id_phongban,
            string id_nganh,
            int dvt = 1000)
        {
           
            _id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            _id_nganh = id_nganh;           
            _dvt = Request.GetQueryStringValue("dvt", 1000);

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
            var id = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            var list = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id)
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
                xls.Open(Server.MapPath(_filePath));

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        header1 = _nganSachService.CheckParam_PhongBan(_id_phongban).IsEmpty() ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                        header = _id_nganh == "-1" ? "" : "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh,
                        header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",

                        TieuDe1 = "Tổng hợp số kiểm tra và số điều chỉnh dự toán năm " + (PhienLamViec.NamLamViec - 1).ToString(),
                        TieuDe2 = "Biểu xác nhận số liệu năm trước làm cơ sở xây dựng số kiểm tra " + PhienLamViec.iNamLamViec + " cho ngành bảo đảm",
                        dvt = 1.ToStringDvt(),
                        Nam = PhienLamViec.NamLamViec - 1,
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
            var data = getTable();
            if (data.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var dtNganh = data.SelectDistinct("Nganh", "Nganh_Parent,TenNganhQL,Nganh,TenNganh");
                var dtDonVi = dtNganh.SelectDistinct("NganhQL", "Nganh_Parent,TenNganhQL");

                _sKTService.AddOrdinalsNum(dtDonVi, 2);
                _sKTService.AddOrdinalsNum(dtNganh, 3, "Nganh_Parent");
                _sKTService.AddOrdinalsNum(data, 4);
                data.CountDistinctRow("Nganh_Parent,Nganh");              

                fr.AddTable("ChiTiet", data);
                fr.AddTable("Nganh", dtNganh);
                fr.AddTable("NganhQL", dtDonVi);
                fr.AddRelationship("NganhQL", "Nganh", "Nganh_Parent,TenNganhQL".Split(','), "Nganh_Parent,TenNganhQL".Split(','));
                fr.AddRelationship("Nganh", "ChiTiet", "Nganh_Parent,TenNganhQL,Nganh,TenNganh".Split(','), "Nganh_Parent,TenNganhQL,Nganh,TenNganh".Split(','));
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable getTable()
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_report_k1n", conn))
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
                    dvt = 1,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
        }

        #endregion
    }
}
