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
    public class rptDuToan_DuToan_SKTCuc_A3ViewModel
    {
        public SelectList NganhList { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
    public class rptDuToan_DuToan_SKTCuc_A3Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/DuToan/Views/Report/Cuc/";
        private string _filePath = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_DuToan_SKTCuc_A3/rptDuToan_DuToan_SKTCuc_A3.xls";
        private string _filePath_dv = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_DuToan_SKTCuc_A3/rptDuToan_DuToan_SKTCuc_A3_dv.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt;
        private int _loai;
        private string _nganh;
        private string _donvi;
        private string _phongban;

        public ActionResult Index()
        {
            if (User.Identity.Name.EndsWith("b2"))
            {
                var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
                var phongbanList = (PhienLamViec.iID_MaPhongBan != "02" && PhienLamViec.iID_MaPhongBan != "11") ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "-- Tất cả phòng ban --"); ;
                var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

                var vm = new rptDuToan_DuToan_SKTCuc_A3ViewModel()
                {
                    PhongBanList = phongbanList,
                    NganhList = dNganh.ToSelectList("Id","Nganh", "-1","-- Chọn chuyên ngành --"),
                    DonViList = donviList.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"),
                };

                var view = _viewPath + "rptDuToan_DuToan_SKTCuc_A3.cshtml";

                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
        public JsonResult Ds_DonVi(string phongban)
        {
            var id = phongban == "s" ? PhienLamViec.iID_MaPhongBan : phongban;          
            
            var data = _nganSachService.GetDonviByPhongBan(PhienLamViec.iNamLamViec, id);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"));

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
            string ext = "pdf",
            int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._loai = loai;
            this._nganh = nganh;
            this._donvi = donvi;
            this._phongban = phongban;
            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var filename = $"Báo_cáo_DuToan_SKTCuc_A3_{DateTime.Now.GetTimeStamp()}.{ext}";
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

                xls.Open(Server.MapPath(_loai == 2 ? _filePath : _filePath_dv));

                fr.SetValue(new
                {
                    headerr = $"Đơn vị tính: {_dvt.ToHeaderMoney()}",
                    namn = PhienLamViec.NamLamViec - 1,
                    nam = PhienLamViec.NamLamViec,
                    phongban = _phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_phongban).sMoTa,
                });

                fr.UseCommonValue()
                  .UseChuKyForController(this.ControllerName())
                  .UseForm(this).Run(xls);              

                var file = xls.TotalPageCount();
                if (file > 1)
                {
                    xls.ClearDiffFirstPage();
                }
                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var DonVi = getDataSet().Tables[0];
            if (DonVi.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var Muc = DonVi.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu,MoTa,STTBC");
                var view = Muc.AsDataView();
                view.Sort = "STTBC";
                Muc = view.ToTable();
                var PhongBan = DonVi.SelectDistinct("PhongBan", "X1,X2,X3,X4,KyHieu,Id_PhongBan,TenPhongBan");          
                _SKTService.FillDataTable_NC(fr, Muc);
                fr.AddTable("Muc", DonVi);
                fr.AddTable("PhongBan", PhongBan);
                fr.AddRelationship("ChiTiet", "PhongBan", "X1,X2,X3,X4,KyHieu".Split(','), "X1,X2,X3,X4,KyHieu".Split(','));
                fr.AddRelationship("PhongBan", "Muc", "X1,X2,X3,X4,KyHieu,Id_PhongBan".Split(','), "X1,X2,X3,X4,KyHieu,Id_PhongBan".Split(','));
                                
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataSet getDataSet()
        {
            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand("sp_dutoan_sktcuc_a3", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = _nganh.ToParamString(),
                    phongban = _phongban.ToParamString(),
                    donvi = _donvi.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = _dvt.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
