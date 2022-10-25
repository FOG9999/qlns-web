using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
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

}

namespace VIETTEL.Controllers
{
    public class rptQuyetToan_TongHop_NamQuy_ChonToController : FlexcelReportController
    {
        private const string sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToan_TongHop_NamQuy_ChonTo.xls";
        private const string sFilePath_Excel = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToan_TongHop_NamQuy_ChonTo_excel.xls";

        #region ctors

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;

        public ActionResult Index()
        {

            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sTen");
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptQuyetToan_TongHop_NamQuyViewModel()
            {
                iNamLamViec = PhienLamViec.iNamLamViec,
                QuyList = DataHelper.GetQuys().ToSelectList(),
                NamNganSachList = getNamNganSachList().ToSelectList(),
                PhongBanList = phongbanList,
                DonViList = donviList,
            };


            var view = string.Format("{0}{1}.cshtml", "~/Views/Report_Views/QuyetToan/", this.ControllerName());
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
            string toSo)
        {
            var xls = createReport(ext, iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, toSo);
            return Print(xls, ext);
        }


        public ActionResult Ds_To(string donvis)
        {

            var count = string.IsNullOrWhiteSpace(donvis) ? 0 : donvis.Split(',').ToList().Count + 1;
            return ds_ToIn(count, 3);
        }

        #endregion

        #region private methods

        private Dictionary<string, string> getNamNganSachList()
        {
            return new Dictionary<string, string>
            {
                { "1,2,4", "Tổng hợp" },
                { "2", "Năm nay" },
                { "1", "Năm trước" },
                { "4", "Năm trước chuyển sang" },
            };
        }

        private DataTable getDataTable(string iThang_Quy, string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi, string toSo)
        {
            var donvi = getDonVis(iID_MaDonVi, toSo);


            var sql = FileHelpers.GetSqlQuery("qt_namquy_chonto.sql");
            //var dNgay = new DateTime(int.Parse(PhienLamViec.iNamLamViec), 1, 1).AddMonths((int.Parse(iThang_Quy) - 1) * 3);
            var dNgay = new DateTime(int.Parse(PhienLamViec.iNamLamViec), 1, 1).AddMonths((int.Parse(iThang_Quy)) * 3);



            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
                cmd.Parameters.AddWithValue("@dNgay", dNgay);
                cmd.Parameters.AddWithValue("@sLNS", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaDonVi1", donvi.donvi1);
                cmd.Parameters.AddWithValue("@iID_MaDonVi2", donvi.donvi2);
                cmd.Parameters.AddWithValue("@iID_MaDonVi3", donvi.donvi3);

                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", string.IsNullOrWhiteSpace(iID_MaNamNganSach) || iID_MaNamNganSach == "0" ? "1,2,4" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());

                var dt = cmd.GetTable();
                return dt;
            }
        }


        private ExcelFile createReport(
            string ext,
            string iThang_Quy,
            string iID_MaNamNganSach,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string toSo)
        {
            var xls = new XlsFile(true);
            xls.Open(getFilePath(ext, toSo));

            var fr = new FlexCelReport();

            loadData(fr, iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, toSo);

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

            var donvi = getDonVis(iID_MaDonVi, toSo);
            var donvi1 = toSo == "1" ?
                     "Tổng số" :
                     _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, (string)donvi.donvi1).sMoTa;
            var donvi2 = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, (string)donvi.donvi2).sMoTa;
            var donvi3 = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, (string)donvi.donvi3).sMoTa;

            fr.SetValue("DonVi1", donvi1);
            fr.SetValue("DonVi2", donvi2);
            fr.SetValue("DonVi3", donvi3);


            fr.SetValue("Quy", iThang_Quy);


            fr.UseCommonValue(new Application.Flexcel.FlexcelModel()
            {
                header2 = $"Đơn vị tính: đồng    Tờ số: {toSo}  Trang: ",
            })
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        private string getFilePath(string ext, string to)
        {
            return Server.MapPath(ext == "xls" ? sFilePath_Excel : sFilePath);
        }

        #endregion

        private void loadData(FlexCelReport fr, string iThang_Quy, string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi, string toSo)
        {
            var data = getDataTable(iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, toSo);
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }

        private dynamic getDonVis(string iID_MaDonVi, string toSo)
        {
            var donvis = iID_MaDonVi.Split(',').ToList();
            donvis.Insert(0, iID_MaDonVi);
            string donvi1 = string.Empty, donvi2 = string.Empty, donvi3 = string.Empty;
            if (toSo == "1")
            {
                donvi1 = donvis[0];
                donvi2 = donvis[1];
                donvi3 = donvis.Count > 2 ? donvis[2] : string.Empty;
            }
            else
            {
                int to = (int.Parse(toSo) - 1) * 3;

                donvi1 = donvis.Count > to ? donvis[to] : string.Empty;
                donvi2 = donvis.Count > to + 1 ? donvis[to + 1] : string.Empty;
                donvi3 = donvis.Count > to + 2 ? donvis[to + 2] : string.Empty;

            }

            return new { donvi1, donvi2, donvi3 };
        }
    }
}
