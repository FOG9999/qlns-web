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
    public class rptDuToan_THNSNNViewModel
    {
        public SelectList NganhList { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }


    }
    public class rptDuToan_THNSNNController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/DuToan/Views/Report/Cuc/";
        private const string _filePath = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN/rptDuToan_THNSNN.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt;
        private string _nganh;
        private string _donvi;
        private string _phongban;

        public ActionResult Index()
        {
            var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
            var phongbanList = (PhienLamViec.iID_MaPhongBan != "02" && PhienLamViec.iID_MaPhongBan != "11") ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "-- Tất cả phòng ban --"); ;
            var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

            var vm = new rptDuToan_THNSNNViewModel()
            {
                PhongBanList = phongbanList,
                NganhList = dNganh.ToSelectList("Id", "Nganh", "-1", "-- Chọn chuyên ngành --"),
                DonViList = donviList.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"),
            };

            var view = _viewPath + "rptDuToan_THNSNN.cshtml";

            return View(view, vm);
        }
        public JsonResult Ds_DonVi(string Id_PhongBanDich)
        {
            var data = _nganSachService.GetDonviByPhongBan(PhienLamViec.iNamLamViec, Id_PhongBanDich).AsEnumerable().Where(x => PhienLamViec.iID_MaDonVi.Contains(x.Field<string>("iID_MaDonVi"))).CopyToDataTable();

            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"));
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
            string ext = "pdf",
            int dvt = 1)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
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
                var filename = $"Báo_cáo_THNSNN_{DateTime.Now.GetTimeStamp()}.{ext}";
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

                xls.Open(Server.MapPath(_filePath));

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
                var PhongBan = DonVi.SelectDistinct("PhongBan", "sLNS1,sLNS3,sLNS5,sLNS,iID_MaPhongBan,TenPhongBan");
                var sLNS = DonVi.SelectDistinct("dtsLNS", "sLNS1,sLNS3,sLNS5,sLNS");
                FillDataTable(fr, sLNS, "sLNS1 sLNS3 sLNS5", PhienLamViec.iNamLamViec);
                fr.AddTable("Muc", DonVi);
                fr.AddTable("PhongBan", PhongBan);
                fr.AddRelationship("ChiTiet", "PhongBan", "sLNS1,sLNS3,sLNS5,sLNS".Split(','), "sLNS1,sLNS3,sLNS5,sLNS".Split(','));
                fr.AddRelationship("PhongBan", "Muc", "sLNS1,sLNS3,sLNS5,sLNS,iID_MaPhongBan".Split(','), "sLNS1,sLNS3,sLNS5,sLNS,iID_MaPhongBan".Split(','));

                if (_phongban != "-1")
                {
                    fr.SetValue("bPB", 1);
                }
                else
                {
                    fr.SetValue("bPB", 0);
                }

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
                var cmd = new SqlCommand("sp_dutoan_report_thnsnn", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = _nganh.ToParamString(),
                    phongban = _phongban.ToParamString(),
                    donvi = _donvi == "-1" ? PhienLamViec.iID_MaDonVi : _donvi.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = _dvt.ToParamString(),
                });
            }
        }
        #endregion
    }
}
