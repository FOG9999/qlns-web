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
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_THNSNN_ToBiaViewModel
    {
        public SelectList NganhList { get; set; }
    }
    public class rptDuToan_THNSNN_ToBiaController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/DuToan/Views/Report/Cuc/";
        private const string _filePath = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN_ToBia/rptDuToan_THNSNN_ToBia.xls";
        private const string _filePath_phancap = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN_ToBia/rptDuToan_THNSNN_PhanCap.xls";
        private const string _filePath_gng = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN_ToBia/rptDuToan_THNSNN_ToBia_GNg.xls";
        //private const string _filePath_gng = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN_ToBia/rptDuToan_THNSNN_ToBia_GNg104.xls";
        private const string _filePath_phancap_gng = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN_ToBia/rptDuToan_THNSNN_PhanCap_GNg.xls";
        private const string _filePath_tachb = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN_ToBia/rptDuToan_THNSNN_TachB.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly IDuToanService _dtService = DuToanService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt;
        private string _loai;
        private string _nganh;

        public ActionResult Index()
        {
            var dNganh = _nganSachService.Nganh_GetAll_ByPhongBan(PhienLamViec.iNamLamViec, !"07,10".Contains(PhienLamViec.iID_MaPhongBan) ? string.Empty : PhienLamViec.iID_MaPhongBan);

            var vm = new rptDuToan_THNSNN_ToBiaViewModel()
            {
                NganhList = dNganh.ToSelectList("iID_MaNganh", "sTenNganh"),
            };

            var view = _viewPath + "rptDuToan_THNSNN_ToBia.cshtml";

            return View(view, vm);            
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
            string loai = "1",
            string ext = "pdf",
            int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._nganh = nganh;
            this._loai = loai;
            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var duoi = _loai == "1" ? "Tờ bìa" : _loai == "3" ? "Tách BQl" : "Chi tiết phân cấp";
                var filename = $"Báo_cáo_THNSNN_{duoi}_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }

        }
        
        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private Dictionary<bool, ExcelFile> createReport()
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);

            var check = loadData(fr);

            if (check.ContainsKey(false))
            {
                return new Dictionary<bool, ExcelFile>
                {
                    {false, xls}
                };
            }
            else
            {

                xls.Open(Server.MapPath(_loai == "1" ? _filePath : _loai == "3" ? _filePath_tachb : _loai == "11" ? _filePath_gng : _loai == "21" ? _filePath_phancap_gng : _filePath_phancap));
                
                var soQd = _dtService.GetCauHinhBia(PhienLamViec.iNamLamViec);

                fr.SetValue(new
                {
                    headerr = $"Đơn vị tính: {_dvt.ToHeaderMoney()}",
                    TenNganh = _nganh == "-1" ? "" : "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _nganh).sTenNganh,
                    nam = PhienLamViec.NamLamViec,
                    namt = PhienLamViec.NamLamViec - 1,
                    qd = _loai == "1" ? $"(Kèm theo công văn {soQd["sSoCVKHNS"].ToString()} của Cục Tài chính/BQP)" : $"(Phụ lục kèm theo Quyết định {soQd["sSoQuyetDinh"].ToString()} của Bộ trưởng BQP về việc giao dự toán Ngân sách năm {PhienLamViec.iNamLamViec})",
                });

                fr.UseCommonValue()
                  .UseChuKyForController(this.ControllerName())
                  .UseForm(this).Run(xls);              
               
                var file = xls.TotalPageCount();
                if (file > 1)
                {
                    xls.AddPageFirstPage();
                }
                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var data = getDataSet().Tables[0];
            if (data.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                if (_loai == "1")
                    FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
                else
                    FillDataTable_NG(fr, data, iNamLamViec: PhienLamViec.iNamLamViec);
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataSet getDataSet()
        {
            var sql = "sp_dutoan_report_thnsnn_tobia";
            //var sql = "sp_dutoan_report_thnsnn_tobia_104";

            if (_loai.StartsWith("2"))
                sql = "sp_dutoan_report_thnsnn_phancap";
            else if (_loai == "3")
                sql = "sp_dutoan_report_thnsnn_tachB";

            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    loai = _loai.EndsWith("1") ? 1 : 0,
                    nganh = _nganh.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = _dvt.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
