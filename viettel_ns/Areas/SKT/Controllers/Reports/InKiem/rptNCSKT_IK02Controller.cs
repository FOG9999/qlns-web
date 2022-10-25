using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{

    public class rptNCSKT_IK02ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }

    public class rptNCSKT_IK02Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/InKiem/";
        private const string _filePath = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK02/rptNCSKT_IK02.xls";
        private const string _filePath_B2 = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK02/rptNCSKT_IK02_B2.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_nganh;
        private string _loaiBaoCao;
        private int _dvt;

        public ActionResult Index()
        {
            var phongbanList = PhienLamViec.iID_MaPhongBan != "02" ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa"); ;

            var vm = new rptNCSKT_IK02ViewModel()
            {
                PhongBanList = phongbanList,
            };
            var view = _viewPath + "rptNCSKT_IK02.cshtml";

            return View(view, vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_nganh"></param>
        /// <param name="loaiBaoCao">chitiet, tonghop</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(string id_phongban, string id_nganh, string loaiBaoCao = "chitiet", string ext = "pdf", int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly
            if (id_nganh.IsEmpty())
                id_nganh = PhienLamViec.iID_MaDonVi;

            this._id_phongban = id_phongban;
            this._id_nganh = id_nganh;
            this._loaiBaoCao = loaiBaoCao;
            this._dvt = dvt;

            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var nganh = id_nganh.IsEmpty() || id_nganh.Contains(",")
                        ? string.Empty
                        : _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).sTenNganh;

                var filename = $"{(nganh.IsEmpty() ? _id_phongban : id_nganh + "-" + nganh.ToStringAccent().Replace(" ", ""))}_Inkiem_NhuCauNSBD_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }
        }
        public JsonResult Ds_Nganh(string id_phongban)
        {
            var data = _SKTService.Get_Nganh_ExistData(PhienLamViec.iNamLamViec, 1, PhienLamViec.iID_MaDonVi, null, id_phongban);
            var vm = new ChecklistModel("Id_Nganh", data.ToSelectList("Id", "Ten"));
            return ToCheckboxList(vm);
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
                var filename = PhienLamViec.iID_MaPhongBan != "02" ? _filePath : _filePath_B2;

                xls.Open(Server.MapPath(filename));

                var nganh = _id_nganh.IsEmpty() || _id_nganh.Contains(",")
                    ? string.Empty
                    : _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;

                var dictieude = _SKTService.GetTieuDeDuLieuNamTruoc(PhienLamViec.NamLamViec);
                fr.SetValue(new
                {
                    h1 = $"Đơn vị tính: {_dvt.ToStringNumber()}đ ",
                    h2 = nganh.IsEmpty() ? "(Tổng hợp ngành)" : $"Ngành: {nganh}",
                    date = DateTime.Now.ToStringNgay(),
                    nam = PhienLamViec.NamLamViec,
                    NamNay = PhienLamViec.NamLamViec - 1,
                    tieude1 = dictieude[1],
                    tieude2 = dictieude[2],
                    tieude3 = dictieude[3],
                    Cap1 = "Cục Tài chính",
                    Cap2 = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
                });

                fr.UseForm(this).Run(xls);

                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var data = getTable();
            var loai = "2";

            if (data.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                if (_loaiBaoCao == "chitiet")
                    _SKTService.FillDataTable_NC(fr, data);

                if (_loaiBaoCao == "chitiet")
                {
                    var ghiChu = getTable_GhiChu();
                    var dtNganhGhiChu = ghiChu.SelectDistinct("Nganh", "Nganh,TenNganh");
                    fr.AddTable("dtGhiChu", ghiChu);
                    fr.AddTable("Nganh", dtNganhGhiChu);
                    fr.AddRelationship("Nganh", "dtGhiChu", "Nganh".Split(','), "Nganh".Split(','));
                    if (ghiChu.Rows.Count > 0)
                        loai = "1";
                }

                fr.SetValue("Loai", loai);
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable getTable()
        {            
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(PhienLamViec.iID_MaPhongBan != "02" ? "sp_ncskt_report_ik02" : "sp_ncskt_report_ik02_b2", new
                {
                    nam = PhienLamViec.iNamLamViec,
                    phongban = _id_phongban.ToParamString(),
                    nganh = _id_nganh.ToParamString(),
                    dvt = _dvt,
                },CommandType.StoredProcedure);
                return dt;
            }
        }

        private DataTable getTable_GhiChu()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable("sp_ncskt_report_ghichu", new
                {
                    nam = PhienLamViec.iNamLamViec,
                    phongban = PhienLamViec.iID_MaPhongBan,
                    phongbandich = _id_phongban.ToParamString(),
                    donvi = _id_nganh.ToParamString(),
                    loai = 2,
                }, CommandType.StoredProcedure);
                return dt;
            }
        }
        #endregion
    }
}
