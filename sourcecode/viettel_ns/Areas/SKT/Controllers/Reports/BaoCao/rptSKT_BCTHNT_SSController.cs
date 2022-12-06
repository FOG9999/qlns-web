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
    public class rptSKT_BCTHNT_SSController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT_ct_bql.xls";
        private const string _filePath_2 = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT_ct_bql_2.xls";
        private const string _filePath_3 = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BCTHNT_ct_bql_3.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int dvt;
        private int loai;
        private string nganh;
        private string id_phongban;

        public ActionResult Index()
        {
            var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
            var vm = new rptSKT_BCTHNT_SSViewModel()
            {
                NganhList = dNganh.ToSelectList("Id","Nganh","-1","----Chọn chuyên ngành----"),
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptSKT_BCTHNT_SS.cshtml", vm);
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
            string nganh,
            int loai = 1,
            string ext = "pdf", 
            int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly           
            this.dvt = dvt;
            this.loai = loai;
            this.nganh = nganh;
            this.id_phongban = id_phongban;
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
            xls.Open(Server.MapPath(loai == 1 ? _filePath : loai == 2 ?_filePath_2 : _filePath_3));
            
            fr.SetValue(new
            {
                T1 = loai == 1 ? "Số kiểm tra" : "Khung báo cáo Bộ",
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
            var Phongban = getDataSet().Tables[0];
            var DonVi = Phongban.SelectDistinct("Phongban", "X1,X2,X3,X4,KyHieu,MoTa,Id_Phongban,sPb");          
            var Muc = DonVi.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu,MoTa");          
            _SKTService.FillDataTable_NC(fr, Muc);
            fr.AddTable("Muc", DonVi);
            fr.AddTable("Phongban", Phongban);
            fr.AddRelationship("ChiTiet", "Muc", "X1,X2,X3,X4,KyHieu".Split(','), "X1,X2,X3,X4,KyHieu".Split(','));
            fr.AddRelationship("Muc", "Phongban", "X1,X2,X3,X4,KyHieu,Id_Phongban".Split(','), "X1,X2,X3,X4,KyHieu,Id_Phongban".Split(','));
        }

        private DataSet getDataSet()
        {
            var sql = loai == 1 ? "skt_report_bcth_ct_bql" : loai == 2 ? "skt_report_bcth_ct_bql_2" : "skt_report_bcth_ct_bql_3";


            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = nganh.ToParamString(),
                    id_phongban = id_phongban.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
