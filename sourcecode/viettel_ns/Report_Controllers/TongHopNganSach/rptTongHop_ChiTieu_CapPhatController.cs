using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptTongHop_ChiTieu_CapPhatViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList NamNganSachList { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptTongHop_ChiTieu_CapPhatController : FlexcelReportController
    {
        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_CapPhat.xls";
        private const String sFilePath_TongHop = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_CapPhat_TongHop.xls";
        private const String sFilePath_TongHop_Muc = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_CapPhat_TongHop_Muc.xls";

        // quyet toan

        private const String sFilePath_QT = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_CapPhat_QT.xls";
        private const String sFilePath_TongHop_QT = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_CapPhat_TongHop_QT.xls";
        private const String sFilePath_TongHop_Muc_QT = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_CapPhat_TongHop_Muc_QT.xls";


        //private const String sFilePath_TongHop_denLNS = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denLNS.xls";
        //private const String sFilePath_TongHop_denTM = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denTM.xls";


        //private readonly string _sqlChiTiet = FileHelpers.GetSqlQuery("rptQuyetToan_TongHop_NamQuy_ChiTiet.sql");
        //private readonly string _sqlTongHop = FileHelpers.GetSqlQuery("rptQuyetToan_TongHop_NamQuy_TongHop.sql");
        private readonly string _sqlChiTiet = FileHelpers.GetSqlQuery("rptTongHop_CapPhat_ChiTiet.sql");
        private readonly string _sqlTongHop = FileHelpers.GetSqlQuery("rptTongHop_CapPhat_TongHop.sql");
        private readonly string _sqlTongHop_Muc = FileHelpers.GetSqlQuery("rptTongHop_CapPhat_TongHop_Muc.sql");

        #region ctors

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;

        public ActionResult Index()
        {

            var iNamLamViec = PhienLamViec.iNamLamViec;
            //var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sTen");
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, iNamLamViec), "iID_MaDonVi", "sMoTa");


            var phongBan = _nganSachService.GetPhongBanQuanLyNS(_nganSachService.CheckParam_PhongBan(PhienLamViec.iID_MaPhongBan));
            var phongbanList = _nganSachService.CheckParam_PhongBan(PhienLamViec.iID_MaPhongBan).IsEmpty() ?
                                    phongBan.ToSelectList("sKyHieu", "sMota", "-1", "-- Toàn cục --") :
                                    phongBan.ToSelectList("sKyHieu", "sMota");


            var vm = new rptQuyetToan_TongHop_NamQuyViewModel()
            {
                iNamLamViec = PhienLamViec.iNamLamViec,
                QuyList = DataHelper.GetQuys().ToSelectList(),
                NamNganSachList = DataHelper.GetNamNganSachList().ToSelectList(),
                PhongBanList = phongbanList,
                DonViList = donviList,
            };

            var view = string.Format("{0}{1}.cshtml", "~/Views/Report_Views/TongHopNganSach/", this.ControllerName());
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iThang_Quy,
            string iID_MaNamNganSach,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            int kieu,
            int loaiBaoCao)
        {
            var xls = createReport(iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, kieu, loaiBaoCao);
            return Print(xls, ext);
        }


        public JsonResult Ds_DonVi(string iID_MaPhongBan)
        {
            var data = _nganSachService.GetDonviByPhongBanId(PhienLamViec.iNamLamViec, iID_MaPhongBan)
                .Select(x => new
                {
                    x.iID_MaDonVi,
                    sTenDonVi = $"{x.iID_MaDonVi} - {x.sTen}"
                });
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sTenDonVi"));
            return ToCheckboxList(vm);
        }

        #endregion

        #region private methods

        private DataTable getDataTable(string iThang_Quy, string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi, int loaiBaoCao = 0)
        {
            var sql = _sqlChiTiet;
            var isTongHop = false;
            if (loaiBaoCao == 1)
            {
            }
            else if (loaiBaoCao == 2)
            {
                sql = _sqlTongHop;
                isTongHop = true;
            }
            else if (loaiBaoCao == 3)
            {
                sql = _sqlTongHop_Muc;
                isTongHop = true;
            }

            //var dNgay = new DateTime(int.Parse(PhienLamViec.iNamLamViec), 1, 1).AddMonths((int.Parse(iThang_Quy)) * 3);
            var dNgay = DateTime.Now;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = 600;

                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dNgay", dNgay.ToParamDate());
                cmd.Parameters.AddWithValue("@sLNS", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", string.IsNullOrWhiteSpace(iID_MaNamNganSach) || iID_MaNamNganSach == "0" ? "2" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());

                var dtAll = cmd.GetTable();

                if (isTongHop)
                    return dtAll;
                else
                {
                    DataView view = new DataView(dtAll);
                    var dtChiTiet = view.ToTable(false,
                        "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,rTuChi,rCapPhat,rQuyetToan".ToArray());
                    return dtChiTiet;
                }
            }
        }
        private ExcelFile createReport(
            string iThang_Quy,
            string iID_MaNamNganSach,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            int kieu,
            int loaiBaoCao)
        {
            var path = getFilePath(kieu, loaiBaoCao);

            var xls = new XlsFile(true);
            xls.Open(path);
            FlexCelReport fr = new FlexCelReport();

            LoadData(fr, iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao);

            var iNamLamViec = PhienLamViec.iNamLamViec;

            //lay ten nam ngan sach
            var NamNganSach = "";
            if (iID_MaNamNganSach == "1")
                NamNganSach = "QUYẾT TOÁN NĂM TRƯỚC";
            else if (iID_MaNamNganSach == "2")
                NamNganSach = "QUYẾT TOÁN NĂM NAY";
            else
            {
                NamNganSach = "TỔNG HỢP";
            }

            String sTenDonVi = loaiBaoCao == 1 ? DonViModels.Get_TenDonVi(iID_MaDonVi, Username) : "";
            String sTenPhongBan = "";
            if (iID_MaPhongBan != "-1")
            {
                sTenPhongBan = PhongBanModels.Get_TenPhongBan(iID_MaPhongBan);
            }
            fr.SetValue("Quy", iThang_Quy);
            fr.SetValue("NamNganSach", NamNganSach);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("sTenPhongBan", sTenPhongBan);

            fr.UseCommonValue(dvt: 1)
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        private string getFilePath(int kieu, int loaiBaoCao)
        {
            var filePath = "";
            if (loaiBaoCao == 1)
            {
                filePath = kieu == 1 ? sFilePath : sFilePath_QT;
            }
            else if (loaiBaoCao == 3)
            {
                filePath = kieu == 1 ? sFilePath_TongHop_Muc : sFilePath_TongHop_Muc_QT;
            }
            else
            {
                filePath = kieu == 1 ? sFilePath_TongHop : sFilePath_TongHop_QT;
            }

            return Server.MapPath(filePath);
        }

        #endregion

        private void LoadData(FlexCelReport fr, String iThang_Quy, String iID_MaNamNganSach, String iID_MaPhongBan, String iID_MaDonVi, int LoaiBaoCao)
        {
            DataTable data = new DataTable();
            var dt = getDataTable(iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, LoaiBaoCao);
            if (LoaiBaoCao == 1)
            {
                data = dt;
                FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);

            }
            else if (LoaiBaoCao == 3)
            {
                data = dt;
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS sL,sK", PhienLamViec.iNamLamViec);
            }
            else
            {
                data = HamChung.SelectDistinct("ChiTiet", dt, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG");
                fr.AddTable("dtDonVi", dt);
                dt.Dispose();

                FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
            }

        }

    }
}
