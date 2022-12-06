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

    public class rptNCSKT_IK01ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }

    public class rptNCSKT_IK01Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/InKiem/";
        private const string _filePath = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK01/rptNCSKT_IK01.xls";
        private const string _filePath_B2 = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK01/rptNCSKT_IK01_B2.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_donvi;
        private string _loaiBaoCao;
        private int _dvt;

        public ActionResult Index()
        {
            var phongbanList = PhienLamViec.iID_MaPhongBan != "02" ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa"); ;

            var vm = new rptNCSKT_IK01ViewModel()
            {
                PhongBanList = phongbanList,
            };
            var view = _viewPath + "rptNCSKT_IK01.cshtml";

            return View(view, vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="loaiBaoCao">chitiet, tonghop</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(string id_phongban, string id_donvi, string loaiBaoCao = "chitiet", string ext = "pdf", int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly
            if (id_donvi.IsEmpty())
                id_donvi = PhienLamViec.iID_MaDonVi;

            this._id_phongban = id_phongban;
            this._id_donvi = id_donvi;
            this._loaiBaoCao = loaiBaoCao;
            this._dvt = dvt;

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

                var filename = $"{(donvi.IsEmpty() ? _id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_Inkiem_NhuCauNSSD_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }
        }
        public JsonResult Ds_DonVi(string id_phongban)
        {
            var data = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 1, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id_phongban);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten"));
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

                var donvi = _id_donvi.IsEmpty() || _id_donvi.Contains(",")
                    ? string.Empty
                    : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen +  (_id_phongban == "06" ? " - Khối doanh nghiệp" : " - Khối dự toán");

                var dictieude = _SKTService.GetTieuDeDuLieuNamTruoc(PhienLamViec.NamLamViec);
                fr.SetValue(new
                {
                    h1 = $"Đơn vị tính: {_dvt.ToStringNumber()}đ ",
                    h2 = donvi.IsEmpty() ? "(Tổng hợp đơn vị)" : $"Đơn vị: {donvi}",
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
                } else
                {
                    var ghiChu = getTable_GhiChu().Clone();
                    var dtNganhGhiChu = ghiChu.SelectDistinct("Nganh", "Nganh,TenNganh");
                    fr.AddTable("dtGhiChu", ghiChu);
                    fr.AddTable("Nganh", dtNganhGhiChu);
                    fr.AddRelationship("Nganh", "dtGhiChu", "Nganh".Split(','), "Nganh".Split(','));
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
                var dt = conn.GetTable(PhienLamViec.iID_MaPhongBan != "02" ? "sp_ncskt_report_ik01" : "sp_ncskt_report_ik01_b2", new
                {
                    nam = PhienLamViec.iNamLamViec,
                    phongban = _id_phongban.ToParamString(),
                    donvi = _id_donvi.ToParamString(),
                    dvt = _dvt,
                    loai = _loaiBaoCao == "tonghop-bv" ? 1 : 2,
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
                    donvi = _id_donvi.ToParamString(),
                    loai = 1,
                }, CommandType.StoredProcedure);
                return dt;
            }
        }
        #endregion
    }
}
