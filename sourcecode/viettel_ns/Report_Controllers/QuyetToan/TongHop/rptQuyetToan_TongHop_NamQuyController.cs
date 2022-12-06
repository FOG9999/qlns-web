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
    public class rptQuyetToan_TongHop_NamQuyViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList QuyList { get; set; }
        public SelectList NamNganSachList { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptQuyetToan_TongHop_NamQuyController : FlexcelReportController
    {
        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy.xls";
        private const String sFilePath_TongHop = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop.xls";
        private const String sFilePath_TongHop_denLNS = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denLNS.xls";
        private const String sFilePath_TongHop_denM = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denM.xls";
        private const String sFilePath_TongHop_denTM = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denTM.xls";


        private readonly string _sqlChiTiet = FileHelpers.GetSqlQuery("rptQuyetToan_TongHop_NamQuy_ChiTiet.sql");
        private readonly string _sqlTongHop = FileHelpers.GetSqlQuery("rptQuyetToan_TongHop_NamQuy_TongHop.sql");

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
                //NamNganSachList = DataHelper.GetNamNganSachList().ToSelectList(),
                NamNganSachList = new List<KeyViewModel>()
                        {
                            new KeyViewModel("1,2,4,5", "Tổng hợp"),
                            new KeyViewModel("2,4", "Năm nay"),
                            new KeyViewModel("1,5", "Năm trước"),
                        }.ToSelectList(),
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
            string loaiBaoCao)
        {
            var xls = createReport(iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao);

            return Print(xls, ext);
        }

        #endregion

        #region private methods

        private DataTable getDataTable(string iThang_Quy, string iID_MaNamNganSach, string iID_MaPhongBan, string iID_MaDonVi, string loaiBaoCao = "chitiet")
        {
            var isTongHop = !string.IsNullOrWhiteSpace(loaiBaoCao) && loaiBaoCao.ToLower() == "tonghop";
            var sql = isTongHop ? _sqlTongHop : _sqlChiTiet;


            var dNgay = iThang_Quy == "4" || iThang_Quy == "5" ? DateTime.Now : new DateTime(int.Parse(PhienLamViec.iNamLamViec), 1, 1).AddMonths((int.Parse(iThang_Quy)) * 3);
            //var dNgay = new DateTime(int.Parse(PhienLamViec.iNamLamViec), 1, 1).AddMonths((int.Parse(iThang_Quy) - 1) * 3);
            var dvt = Request.GetQueryStringValue("dvt", 1);
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn) { CommandTimeout = 600 };

                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
                cmd.Parameters.AddWithValue("@dNgay", dNgay.ToParamDate());
                cmd.Parameters.AddWithValue("@sLNS", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", string.IsNullOrWhiteSpace(iID_MaNamNganSach) || iID_MaNamNganSach == "0" ? "2,4" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("@dvt", dvt);

                var dtAll = cmd.GetTable();

                if (isTongHop)
                    return dtAll;

                else
                {
                    DataView view = new DataView(dtAll);
                    var dtChiTiet = view.ToTable(false,
                        "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,rTuChi,Quy1,Quy2,Quy3,Quy4".ToArray());
                    return dtChiTiet;
                }
            }
        }
        private ExcelFile createReport(
            string iThang_Quy,
            string iID_MaNamNganSach,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string loaiBaoCao)
        {
            var path = getFilePath(loaiBaoCao);

            var xls = new XlsFile(true);
            xls.Open(path);
            FlexCelReport fr = new FlexCelReport();

            LoadData(fr, iThang_Quy, iID_MaNamNganSach, GetPhongBanId(iID_MaPhongBan), iID_MaDonVi, loaiBaoCao);

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

            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, Username);
            String sTenPhongBan = "";
            if (iID_MaPhongBan != "-1")
            {
                sTenPhongBan = PhongBanModels.Get_TenPhongBan(iID_MaPhongBan);
            }
            fr.SetValue("Quy", iThang_Quy);
            fr.SetValue("NamNganSach", NamNganSach);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("sTenPhongBan", sTenPhongBan);

            fr
                .UseCommonValue(new Application.Flexcel.FlexcelModel
                {
                    header2 = $"Đơn vị tính: {Request.GetQueryStringValue("dvt").ToStringDvt()}"
                })
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        private string getFilePath(string loaiBaoCao, string tuyChon = "1")
        {
            var filePath = "";
            if (loaiBaoCao.Compare("ChiTiet"))
            {
                filePath = sFilePath;
            }
            else
            {
                if (tuyChon == "1")
                {
                    filePath = sFilePath_TongHop;
                }
                else if (tuyChon == "2")
                {
                    filePath = sFilePath_TongHop_denLNS;
                }
                else if (tuyChon == "3")
                {
                    filePath = sFilePath_TongHop_denM;
                }
                else if (tuyChon == "4")
                {
                    filePath = sFilePath_TongHop_denTM;
                }
            }

            return Server.MapPath(filePath);
        }

        #endregion

        private void LoadData(FlexCelReport fr, String iThang_Quy, String iID_MaNamNganSach, String iID_MaPhongBan, String iID_MaDonVi, String LoaiBaoCao)
        {
            DataTable data = new DataTable();
            var dt = getDataTable(iThang_Quy, iID_MaNamNganSach, iID_MaPhongBan, iID_MaDonVi, LoaiBaoCao);
            if (LoaiBaoCao == "chitiet")
            {
                data = dt;
            }
            else
            {
                data = HamChung.SelectDistinct("ChiTiet", dt, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG");
                fr.AddTable("dtDonVi", dt);
                dt.Dispose();
            }

            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }
    }
}
