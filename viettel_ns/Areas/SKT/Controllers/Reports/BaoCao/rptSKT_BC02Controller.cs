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
    public class rptSKT_BC02Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/BaoCao/";
        private string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC02.xls";
        private string _filePath_pc = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC02_PC.xls";
        private string _filePath_hv = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC02_HV.xls";


        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _loaiBC;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            var view = _viewPath + "rptSKT_BC02.cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;

            var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var nganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username)
                                            .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            var vm = new rptSKT_BC02ViewModel
            {
                NganhList = nganhList,
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
            };

            return View(view, vm);
        }

        public ActionResult Print(
            string ext,
            string id_phongban,
            string id_nganh,
            int loaiBC,
            int dvt = 1000)
        {
            _id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            _id_nganh = id_nganh;
            _loaiBC = loaiBC;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            if (_loaiBC == 3)
            {
                _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC02_HV.xls";
            }

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
                var filename = _loaiBC == 3 ? _filePath_hv : _loaiBC == 2 ? _filePath_pc : _filePath ;
                xls.Open(Server.MapPath(filename));

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        headerl = _id_nganh == "-1" ? "" : "Đơn vị: " + _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_nganh).sTen,
                        headerr = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                        T = _loaiBC == 1 ? "Mua hàng cấp hiện vật" : "Ngành phân cấp bằng tiền",
                        ND = _loaiBC == 1 ? "Mua hàng cấp hiện vật" : "Nội dung chi đặc thù",

                        nam = PhienLamViec.NamLamViec,
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
            var dt = GetDataSet();
            var data = dt.Tables[1];
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

                fr.AddTable("ChiTiet", data);
                fr.AddTable("Nganh", dtNganh);
                fr.AddTable("NganhQL", dtDonVi);
                fr.AddRelationship("NganhQL", "Nganh", "Nganh_Parent,TenNganhQL".Split(','), "Nganh_Parent,TenNganhQL".Split(','));
                fr.AddRelationship("Nganh", "ChiTiet", "Nganh_Parent,TenNganhQL,Nganh,TenNganh".Split(','), "Nganh_Parent,TenNganhQL,Nganh,TenNganh".Split(','));

                fr.SetValue(new
                {
                    Cot1 = dt.Tables[0].Rows[0]["N_2"],
                    Cot2 = dt.Tables[0].Rows[0]["N_1"],
                });
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataSet GetDataSet()
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("skt_report_bc02", conn))
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
                    dvt = _dvt,
                    loai = _loaiBC,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetDataset();
                return dt;
            }
        }

        #endregion
    }
}
