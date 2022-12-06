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
    public class rptKiem_K5Controller : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/Kiem/rptKiem_K5.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int dvt;
        private int loai;
        private string nganh;

        public ActionResult Index()
        {
            var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
            var vm = new rptKiem_K5ViewModel()
            {
                NganhList = dNganh.ToSelectList("Id","Nganh","-1","----Chọn chuyên ngành----"),
            };

            return View(@"~/Areas\SKT\Views\Reports\Kiem\rptKiem_K5.cshtml", vm);
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
            string nganh, 
            int loai = 1,
            string ext = "pdf", 
            int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly           
            this.dvt = dvt;
            this.loai = loai;
            this.nganh = nganh;
            var xls = createReport();           

            var filename = $"Báo_cáo_K5_{DateTime.Now.GetTimeStamp()}.{ext}";
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
            xls.Open(Server.MapPath(_filePath));
            
            fr.SetValue(new
            {
                headerr = $"Đơn vị tính: {dvt.ToHeaderMoney()}",
                nam = PhienLamViec.NamLamViec - 1,
                namn = PhienLamViec.NamLamViec,
                phongban = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
            });

            fr.UseCommonValue()
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var DonVi = getDataSet().Tables[0];
            var Muc = DonVi.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu,MoTa");          
            _SKTService.FillDataTable_NC(fr, Muc);
            fr.AddTable("Muc", DonVi);
            fr.AddRelationship("ChiTiet", "Muc", "X1,X2,X3,X4,KyHieu".Split(','), "X1,X2,X3,X4,KyHieu".Split(','));
        }

        private DataSet getDataSet()
        {
            var sql = "skt_report_k5";            
            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = nganh.ToParamString(),
                    phongban = PhienLamViec.iID_MaPhongBan.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
