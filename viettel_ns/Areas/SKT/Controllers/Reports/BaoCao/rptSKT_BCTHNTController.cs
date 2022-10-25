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
    public class rptSKT_BCTHNTController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT.xls";
        private const string _filePath_ct = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT_ct.xls";
        private const string _filePath_ct_dn = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT_ct_dn.xls";
        private const string _filePath_ct_dn_less = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT_ct_dn_less.xls";
        private const string _filePath_ct_dn_bql = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT_ct_dn_bql.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int dvt;
        private int loai;
        private string nganh;

        public ActionResult Index()
        {
            var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
            var vm = new rptSKT_BCTHNTViewModel()
            {
                NganhList = dNganh.ToSelectList("Id","Nganh","-1","----Chọn chuyên ngành----"),
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptSKT_BCTHNT.cshtml", vm);
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

            var filename = $"BCTHNT_{DateTime.Now.GetTimeStamp()}.{ext}";
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
            xls.Open(Server.MapPath(loai == 1 ? _filePath : loai == 2 ? _filePath_ct : loai == 3 ? _filePath_ct_dn : loai == 4 ? _filePath_ct_dn_less : _filePath_ct_dn_bql));
            
            fr.SetValue(new
            {
                headerr = $"Đơn vị tính: {dvt.ToHeaderMoney()}",
                nam = PhienLamViec.NamLamViec - 1,
                namn = PhienLamViec.NamLamViec,
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
            var sql = "skt_report_bcthnt_th_ct";
            if (loai == 2) sql = "skt_report_bcthnt_th";
            else if (loai == 1) sql = "skt_report_bcthnt";
            else if (loai == 5) sql = "skt_report_bcthnt_th_ct_bql";
            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = nganh.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
