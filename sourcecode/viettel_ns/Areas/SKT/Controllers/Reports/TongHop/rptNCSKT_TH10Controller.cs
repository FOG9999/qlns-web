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
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_TH10ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptNCSKT_TH10Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/TongHop/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH10/rptNCSKT_TH10.xls";
        private string _filePath_ncskt = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH10/rptNCSKT_TH10_ncskt.xls";

        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _id_phongban;
        private string _id_nganh;
        private string _loai;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            if (User.Identity.Name.EndsWith("b2"))
            {
                var phongbanList = _nganSachService.GetPhongBansQuanLyNS("07,10");
                var nganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, null, PhienLamViec.iID_MaDonVi)
                                                .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
                var vm = new rptNCSKT_TH10ViewModel
                {
                    NganhList = nganhList,
                    PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                };

                var view = _viewPath + "rptNCSKT_TH10.cshtml";

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
            string loai,
            string id_nganh,
            int dvt = 1000)
        {
            _id_phongban = id_phongban;
            _id_nganh = id_nganh;
            _loai = loai;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            if (loai == "2")
            {
                _filePath = _filePath_ncskt;
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
            var list = _sKTService.Get_Nganh_ExistData(PhienLamViec.iNamLamViec, 5, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id_phongban)
                                          .ToSelectList("Id", "Ten", "-1", "-- Chọn ngành --");
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
                xls.Open(Server.MapPath(_filePath));

                var nganh = _id_nganh == "-1"
                    ? string.Empty
                    : _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;

                var dictieude = _sKTService.GetTieuDeDuLieuNamTruoc(PhienLamViec.NamLamViec);

                fr.SetValue(new
                {
                    h1 = $"Đơn vị tính: {_dvt.ToStringNumber()}đ ",
                    h2 = nganh.IsEmpty() ? "(Tổng hợp ngành)" : $"Ngành: {nganh}",
                    date = DateTime.Now.ToStringNgay(),
                    nam = PhienLamViec.NamLamViec,
                    NamNay = PhienLamViec.NamLamViec - 1,
                    tieude1 = dictieude[1],
                    tieude2 = dictieude[2],
                    tieude3 = dictieude[3],
                    Cap1 = "Cục Tài chính",
                    Cap2 = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
                    TenPhongBan = _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                });

                fr.UseChuKyForController(this.ControllerName())
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
            var dt = GetData();
            if (dt.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var dtNganh = dt.SelectDistinct("Nganh", "Nganh_Parent,TenNganhQL,Nganh,TenNganh");
                var dtDonVi = dtNganh.SelectDistinct("NganhQL", "Nganh_Parent,TenNganhQL");

                _sKTService.AddOrdinalsNum(dtDonVi, 2);
                _sKTService.AddOrdinalsNum(dtNganh, 3, "Nganh_Parent");
                _sKTService.AddOrdinalsNum(dt, 4);

                fr.AddTable("ChiTiet", dt);
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

        private DataTable GetData()
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_report_th10", conn))
            {
                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                    nganh = _id_nganh.ToParamString(),
                    loai = _loai.ToParamString(),
                    phongban = _id_phongban.ToParamString(),
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
