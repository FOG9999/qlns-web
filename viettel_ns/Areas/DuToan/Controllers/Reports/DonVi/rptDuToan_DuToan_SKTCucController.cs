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
    public class rptDuToan_DuToan_SKTCucController : FlexcelReportController
    {
        private string _filePath = "~/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_DuToan_SKTCuc.xls";
        private string _filePath_m = "~/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_DuToan_SKTCuc_M.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int dvt;
        private string id_phongban;
        private string loai;
        private string kyhieu;
        private string ty;
        private string ng;
        private string m;

        public ActionResult Index()
        {
            if (PhienLamViec.iID_MaPhongBan == "02") { 
                var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

                var vm = new rptDuToan_SKTViewModel()
                {
                    PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "---Tổng hợp cục---"),
                };

                return View(@"~/Areas\DuToan\Views\Report\Kiem\rptDuToan_DuToan_SKTCuc.cshtml", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
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
            string ext = "pdf",
            int dvt = 1000)
        {            
            this.id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            this.dvt = dvt;
            this.loai = Request.GetQueryStringValue("ns").ToString();
            this.kyhieu = Request.GetQueryStringValue("kh").ToString();
            this.ty = Request.GetQueryStringValue("t").ToString();
            this.ng = Request.GetQueryStringValue("ng").ToString();
            this.m = Request.GetQueryStringValue("m").ToString();

            var xls = createReport();

            var ten =_nganSachService.GetPhongBanById(id_phongban).sMoTa;

            var filename = $"{ten}_k_skt_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls, ext, filename);
        }
        
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
            xls.Open(Server.MapPath(m == "0" ?_filePath : _filePath_m));            

            fr.SetValue(new
            {
                headerr = $"Đơn vị tính: {dvt.ToHeaderMoney()}",
                nam = int.Parse(PhienLamViec.iNamLamViec),
                TenPhongBan = id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(id_phongban).sMoTa,   
                hide = Request.GetQueryStringValue("loai").ToString(),
                khoi = Request.GetQueryStringValue("kh").ToString() == "0" ? "Toàn quân" : Request.GetQueryStringValue("kh").ToString() == "1" ? "Khối đơn vị dự toán + doanh nghiệp": "Khối bệnh viện",
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
            fr.AddTable("DonVi",dt);
            fr.AddRelationship("ChiTiet", "DonVi", "KyHieu".Split(','), "KyHieu".Split(','));
            _SKTService.FillDataTable_NC(fr, dt.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu"));
        }

        private DataSet getDataSet()
        {
            var sql = "DT_report_dutoan_skt_thcuc";
            
            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                    phongban = id_phongban.ToParamString(),
                    loai = loai.ToParamString(),
                    kh = kyhieu.ToParamString(),
                    ng = ng.ToParamString(),
                    ty = ty.ToParamString(),
                });
            }
        }
        #endregion
    }
}
