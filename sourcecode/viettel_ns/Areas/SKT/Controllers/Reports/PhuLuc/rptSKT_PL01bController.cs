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
    public class rptSKT_PL01Controller : FlexcelReportController
    {
        #region var def

        private const string _filePath = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL01.xls";
        private const string _filePath_bql = "~/Report_ExcelFrom/SKT/PhuLuc/rptSKT_PL01b.xls";

        private readonly ISKTService _sKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_donvi;
        private string _id_phongban;
        private string _id_phongban_dich;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var data = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec);
            var vm = new rptSKT_PL01ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Tất cả phòng ban --"),
                DonViList = data.ToSelectList("iID_MaDonVi", "sMoTa"),
            };

            var view = $"~/Areas/SKT/Views/Reports/PhuLuc/{this.ControllerName()}.cshtml";
            return View(view, vm);
        }


        #endregion

        #region public methods
        public JsonResult Ds_DonVi(string id_PhongBan)
        {
            var data = _nganSachService.GetDonviByPhongBan(PhienLamViec.iNamLamViec, id_PhongBan);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("iID_MaDonVi", "sMoTa"));
            return ToCheckboxList(vm);
        }

        public ActionResult Print(
            string Id_DonVi,
            string id_phongban,
            string id_phongban_dich,
            string ext = "pdf")
        {
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            this._id_donvi = Id_DonVi;
            this._id_phongban = id_phongban;
            this._id_phongban_dich = id_phongban_dich;

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

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>

        public DataTable getTable()
        {
            #region get data

            var sql = "skt_report_pl01";

            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                return cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    id_phongban = _id_phongban.ToParamString(),
                    id_phongban_dich = _id_phongban_dich.ToParamString(),
                    id_donvi = _id_donvi.ToParamString(),
                    dvt = _dvt
                }).Tables[0];                
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
                var filename = _id_phongban == "02" ? _filePath : _filePath_bql;
                xls.Open(Server.MapPath(filename));

                var tenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;

                fr.UseCommonValue()
                    .SetValue(new
                    {
                        TieuDe1 = $"Thông báo số kiểm tra ngân sách năm {PhienLamViec.NamLamViec}",
                        TieuDe2 = _id_phongban_dich.StartsWith("06") ? "Khối Doanh nghiệp" : (_id_donvi.Length <= 3 ? "Khối Dự toán" : "Nội dung chi không thực hiện tự chủ"),
                        DonVi = tenDonVi,
                        PhongBan = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                        h1 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                        h2 = tenDonVi,
                        b = _id_phongban == "02" ? 1 : 0,
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

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }

        }
        #endregion
    }
}
