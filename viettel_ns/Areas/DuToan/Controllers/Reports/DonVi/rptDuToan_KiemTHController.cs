using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Areas.DuToan.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_KiemTHController : FlexcelReportController
    {
        private string _filePath = "~/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_KiemTH.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int dvt;
        private string id_phongban;
        private string id_donvi;

        public ActionResult Index()
        {
            var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

            var vm = new rptDuToan_SKTViewModel()
            {
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa"),
                DonViList = donviList.ToSelectList("iID_MaDonVi", "sMoTa"),
            };

            return View(@"~/Areas\DuToan\Views\Report\Kiem\rptDuToan_KiemTH.cshtml", vm);
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
            string ext = "pdf",
            int dvt = 1000)
        {            
            this.id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            this.id_donvi = id_donvi;
            this.dvt = dvt;

            var xls = createReport();

            var ten =_nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            var filename = $"{ten}_k_skt_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls, ext, filename);
        }

        #region public
        public JsonResult Ds_DonVi(string id_PhongBan)
        {
            var id = id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : id_PhongBan;
            var data = _nganSachService.GetDonviByPhongBan(PhienLamViec.iNamLamViec,id);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("iID_MaDonVi", "sMoTa"));
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

            fr.SetValue(new
            {
                Donvi = $"Đơn vị: {_nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen}",
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
            FillDataTableLNS(fr, dt, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }

        private DataSet getDataSet()
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("DT_report_kiemth_a3", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", dvt.ToParamString());
                cmd.Parameters.AddWithValue("@donvis", id_donvi.ToParamString());

                var dt = cmd.GetDataset();
                return dt;
            }
        }
        #endregion
    }
}
