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

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_TK07ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }
    public class rptNCSKT_TK07Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/TrinhKy/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/TrinhKy/rptNCSKT_TK07/rptNCSKT_TK07.xls";
        private string _filePath_dv = "~/Areas/SKT/FlexcelForm/TrinhKy/rptNCSKT_TK07/rptNCSKT_TK07_dv.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_donvi;
        private string _loai;
        private int _dvt;

        public ActionResult Index()
        {
            if (User.Identity.Name.EndsWith("b2"))
            {
                var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

                var vm = new rptNCSKT_TK07ViewModel()
                {
                    PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa"),
                };

                var view = _viewPath + "rptNCSKT_TK07.cshtml";

                return View(view, vm);
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
            string id_donvi,
            string loai,
            string ctDV = "1",
            string ext = "pdf",
            int dvt = 1000)
        {           
            this._id_phongban = id_phongban;
            this._id_donvi = id_donvi.IsEmpty() ? _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 4, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id_phongban).AsEnumerable().Select(x => x.Field<string>("Id")).Join(",") : id_donvi;
            this._dvt = dvt;
            this._loai = loai;

            if (loai.EndsWith("1") || ctDV != "1")
            {
                _filePath = _filePath_dv;
            }

            var xls = createReport();            

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
                  ? string.Empty
                  : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

                var filename = $"{(donvi.IsEmpty() ? id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_TH09_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }
        }

        #region public
        public JsonResult Ds_DonVi(string id_phongban)
        {
            var data = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 4, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id_phongban);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten"));
            return ToCheckboxList(vm);
        }
        #endregion

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
                                
                var donvi = _id_donvi.IsEmpty() || _id_donvi.Contains(",")
                    ? string.Empty
                    : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;

                fr.SetValue(new
                {
                    headerl = donvi.IsEmpty() ? "Tổng hợp đơn vị" : $"{_nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen}",
                    headerr = $"Đơn vị tính: {_dvt.ToHeaderMoney()}",
                    nam = int.Parse(PhienLamViec.iNamLamViec),
                    namt = PhienLamViec.NamLamViec - 1,
                    TenPhongBan = _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                    date = DateTime.Now.ToStringNgay(),
                    donvi = donvi.IsEmpty() ? $"Tổng hợp đơn vị - {_nganSachService.GetPhongBanById(_id_phongban).sMoTa}" : $"Đơn vị: {_nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen}",
                    ThuTruong = "6,8,10,17".Contains(_id_phongban) ? "Đại tá Đỗ Mạnh Hùng" : "Đại tá Lê Văn Thuận",
                    //loai = _loai.StartsWith("1") ? $"Nhu cầu đơn vị năm {PhienLamViec.iNamLamViec}" : $"Số kiểm tra năm {PhienLamViec.NamLamViec - 1}",
                });

                fr.UseCommonValue()
                  .UseChuKyForController(this.ControllerName())
                  .UseForm(this).Run(xls);
                
                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var data = getDataSet();
            if (data.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                fr.AddTable("DonVi", data);
                fr.AddRelationship("ChiTiet", "DonVi", "X1,X2,X3,X4,KyHieu".Split(','), "X1,X2,X3,X4,KyHieu".Split(','));
                _SKTService.FillDataTable_NC(fr, data.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu"));

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable getDataSet()
        {           
            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand("sp_ncskt_report_tk07", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    dvt = _dvt.ToParamString(),
                    donvi = _id_donvi.ToParamString(),
                    //loai = _loai.ToParamString(),
                    phongban = _id_phongban.ToParamString(),
                }).Tables[0];
            }
        }        
        #endregion
    }
}
