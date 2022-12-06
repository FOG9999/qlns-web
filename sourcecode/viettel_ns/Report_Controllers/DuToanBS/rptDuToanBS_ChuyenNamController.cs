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
    public class rptDuToanBS_ChuyenNamViewModel
    {
        public string iNamLamViec { get; set; }
        public string TieuDe { get; set; }
        public string CanCu { get; set; }

        public SelectList DonViList { get; set; }
        public SelectList PhongBanList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptDuToanBS_ChuyenNamController : FlexcelReportController
    {

        public string _viewPath = "~/Views/Report_Views/DuToanBS/";
        //private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ChuyenNam.xls";
        private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ChuyenNam_NSBD.xls";

        private readonly IConnectionFactory _connFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _bsService = DuToanBsService.Default;

        #region ctor

        public ActionResult Index()
        {
            var namNganSachList = _nganSachService.GetNamNganSachs()
                .ToSelectList("iID_MaNamNganSach", "sTen", "-1", "-- Chọn năm ngân sách --");

            var donviList = getDonViChuyenNam(PhienLamViec.NamLamViec, "1", GetPhongBanId(PhienLamViec.iID_MaPhongBan))
                                    .ToSelectList("iID_MaDonVi", "sTenDonVi");

            var vm = new rptDuToanBS_ChuyenNamViewModel
            {
                iNamLamViec = PhienLamViec.iNamLamViec,

                TieuDe = "Phê duyệt giao chuyển dự toán ngân sách",
                CanCu = "Kèm theo Quyết định số: ............./QĐ-BQP ngày ....../....../20......  của Bộ Quốc phòng",
                DonViList = donviList,
                PhongBanList = GetPhongBanList(_nganSachService)
            };


            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iID_MaNamNganSach,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string tieuDe,
            string cancu,
            int trinhKy)
        {
            var xls = createReport(iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, tieuDe, cancu, trinhKy);
            return Print(xls, ext);
        }

        public JsonResult Ds_DonVi(string iID_MaNamNganSach, string iID_MaPhongBan)
        {
            iID_MaPhongBan = GetPhongBanId(iID_MaPhongBan);

            var data = getDonViChuyenNam(PhienLamViec.NamLamViec, iID_MaNamNganSach, iID_MaPhongBan);
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sTenDonVi"));

            return ToCheckboxList(vm);
        }

        #endregion

        private ExcelFile createReport(string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi, string tieuDe, string cancu, int trinhKy)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            var fr = new FlexCelReport();
            var data = getDataTable(iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi);
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);

            fr.SetValue("PhongBan", _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa);
            fr.SetValue("sTenDonVi", _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sMoTa);
            fr.SetValue("TenPhuLuc", string.Format("{0} năm {1} sang năm {2}", tieuDe, PhienLamViec.NamLamViec, PhienLamViec.NamLamViec + 1));
            fr.SetValue("CanCu", cancu);
            fr.SetValue("TrinhKy", trinhKy);

            var tien = data.AsEnumerable().Select(x => double.Parse(x["rTongCong"].ToString())).Sum().ToStringMoney();
            fr.SetValue("Tien", tien);

            var namNganSach = iID_MaNamNganSach == "4" ? "Năm trước chuyển sang: chưa cấp tiền" : "Năm trước chuyển sang: đã cấp tiền";
            fr.SetValue("NamNganSach", namNganSach);


            fr.UseCommonValue()
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            return xls;
        }

        private DataTable getDataTable(string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi)
        {

            var sql = FileHelpers.GetSqlQuery("dtbs_chuyennam_nsbd.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.NamLamViec + 1);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", GetPhongBanId(iID_MaPhongBan).ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@username", DBNull.Value);
                    //cmd.Parameters.AddWithValue("@username", Username.ToParamString(!_nganSachService.IsUserRoleType(Username, Viettel.Domain.DomainModel.UserRoleType.TroLyPhongBan)));
                    cmd.Parameters.AddWithValue("@dvt", 1);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }

        private DataTable getDonViChuyenNam(int iNamLamViec, string iID_MaNamNganSach, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_chuyennam_donvi.sql");

            using (var conn = _connFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    iNamLamViec = iNamLamViec + 1,
                    iID_MaNamNganSach,
                    iID_MaPhongBan = iID_MaPhongBan.ToParamString(),
                    username = Username.ToParamString(!_nganSachService.IsUserRoleType(Username, Viettel.Domain.DomainModel.UserRoleType.TroLyPhongBan))
                });
                return cmd.GetTable();
            }
        }

    }
}
