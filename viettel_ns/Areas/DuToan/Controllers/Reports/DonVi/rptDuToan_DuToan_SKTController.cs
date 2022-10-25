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
    public class rptDuToan_DuToan_SKTController : FlexcelReportController
    {
        private string _filePath = "~/Report_ExcelFrom/DuToan/DonVi/rptDuToan_DuToan_SKT.xls";

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

            return View(@"~/Areas\DuToan\Views\Report\Kiem\rptDuToan_DuToan_SKT.cshtml", vm);
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

            var count = xls.TotalPageCount();

            if (count > 1)
            {
                xls.AddPageFirstPage();
            }

            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getDataSet();           
            var dt = data.Tables[0];
            _SKTService.FillDataTable_NC(fr, dt);
        }

        private DataSet getDataSet()
        {
            var sql = "DT_report_dutoan_skt";
            if (PhienLamViec.NamLamViec > 2020)
                sql = "sp_report_dutoan_skt";

            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                    donvi = id_donvi.ToParamString(),
                    phongban = id_phongban.ToParamString(),
                });
            }
        }
        #endregion
    }
}
