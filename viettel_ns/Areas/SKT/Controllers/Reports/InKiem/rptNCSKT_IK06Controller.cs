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
    public class rptNCSKT_IK06ViewModel
    {
        public SelectList NganhList { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
    public class rptNCSKT_IK06Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/InKiem/";
        private const string _filePath = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK06/rptNCSKT_IK06.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt;
        private int _loai;
        private int _loaiBC;
        private string _nganh;
        private string _donvi;
        private string _phongban;

        public ActionResult Index()
        {
            var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
            var phongbanList = PhienLamViec.iID_MaPhongBan != "02" ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa"); ;
            var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

            var vm = new rptNCSKT_IK06ViewModel()
            {
                PhongBanList = phongbanList,
                NganhList = dNganh.ToSelectList("Id","Nganh","-1","-- Chọn chuyên ngành --"),
                DonViList = donviList.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"),
            };

            var view = _viewPath + "rptNCSKT_IK06.cshtml";

            return View(view, vm);
        }
        public JsonResult Ds_DonVi(string phongban)
        {
            var data = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 4, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, phongban);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten", "-1", "-- Chọn đơn vị --"));

            return ToDropdownList(vm);
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
            string donvi,
            string phongban,
            int loai = 1,
            int loaiBC = 1,
            string ext = "pdf", 
            int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._loai = loai;
            this._loaiBC = loaiBC;
            this._nganh = nganh;
            this._donvi = donvi;
            this._phongban = phongban;
            var xls = createReport();           

            var filename = $"Báo_cáo_IK06_{DateTime.Now.GetTimeStamp()}.{ext}";
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
                headerr = $"Đơn vị tính: {_dvt.ToHeaderMoney()}",
                t1 = _loaiBC == 1 ? "NHU CẦU ĐƠN VỊ " : "QUYẾT TOÁN 2019",
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
            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand(_loaiBC == 1 ? "sp_ncskt_report_ik06" : "sp_ncskt_report_ik06_qt", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = _nganh.ToParamString(),
                    phongban = _phongban.ToParamString(),
                    donvi = _donvi.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    loai = _loai.ToParamString(),
                    dvt = _dvt.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
