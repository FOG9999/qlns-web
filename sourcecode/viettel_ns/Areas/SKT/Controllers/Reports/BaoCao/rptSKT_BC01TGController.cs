using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptSKT_BC01TGController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC01TG.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban;
        private string id_donvi;
        private int loaiBC;
        private int dvt;

        public ActionResult Index()
        {

            var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

            var vm = new rptSKT_BC01ViewModel()
            {
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                DonViList = donviList.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"),
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptSKT_BC01TG.cshtml", vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="loai">1: NSSD, 2: NSBĐ ngành</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(
            string id_phongban,
            string id_donvi, 
            int loaiBC = 1,
            string ext = "pdf", int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly
            if (id_donvi.IsEmpty())
                id_donvi = PhienLamViec.iID_MaDonVi;
            this.id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            this.id_donvi = id_donvi;
            this.loaiBC = loaiBC;
            this.dvt = dvt;

            var xls = createReport();

            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
              ? string.Empty
              : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            var filename = $"{(donvi.IsEmpty() ? id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_K4_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls, ext, filename);
        }
        #region public
        public JsonResult Ds_DonVi(string id_PhongBan, int loaiBC = 1)
        {
            var id = id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : id_PhongBan;

            var data = _SKTService.Get_DonViTG_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, loaiBC == 1 ? "06,07,08,10" : "02", id);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id","Ten"));
            return ToCheckboxList(vm);
        }
        #endregion
        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
                ? string.Empty
                : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            fr.SetValue(new
            {
                headerl = donvi.IsEmpty() ? "Tổng hợp đơn vị" : $"Đơn vị: {_nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen}",
                headerr = $"Đơn vị tính: {dvt.ToHeaderMoney()}",
                nam = int.Parse(PhienLamViec.iNamLamViec),
                TenPhongBan = id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(id_phongban).sMoTa,
            });

            fr.UseCommonValue()
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getDataSet();
            var dt = data.Tables[0];
            fr.AddTable("DonVi", dt);
            fr.AddRelationship("ChiTiet", "DonVi", "X1,X2,X3,X4,KyHieu".Split(','), "X1,X2,X3,X4,KyHieu".Split(','));
            _SKTService.FillDataTable_NC(fr, dt.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu"));
        }

        private DataSet getDataSet()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand("skt_report_bc01tg", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                    donvi = id_donvi.ToParamString(),
                    phongban = id_phongban.ToParamString(),
                    b = loaiBC,
            });
            }
        }
        #endregion
    }
}
