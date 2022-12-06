using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Application.Flexcel;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptQuyetToanNam_PheDuyetViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
        public SelectList NamNganSachList { get; set; }
        public SelectList NguonNganSachList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptQuyetToanNam_PheDuyetController : FlexcelReportController
    {
        private const string sFilePath_TongHop_LNS =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_TongHopDenLNS.xls";

        private const string sFilePath_TongHop_Khoi =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_TongHopDenKhoiDonVi.xls";

        private const string sFilePath_TongHop_Khoi2 =
           //"/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_ChiTiet_PhuLuc_Khoi.xls";
           "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_ChiTiet_PhuLuc_Khoi_Gop.xls";
        private const string sFilePath_TongHop_Khoi_DonVi =
         //"/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_ChiTiet_PhuLuc_Khoi_DonVi.xls";
         "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_ChiTiet_PhuLuc_Khoi_DonVi_Gop.xls";


        private const string sFilePath_TongHop_DonVi =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_TongHopDenDonVi.xls";

        private const string sFilePath_TongHop_PhuLuc_LNS =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_PhuLuc_TongHopDenLNS.xls";

        private const string sFilePath_TongHop_PhuLuc_Khoi =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_PhuLuc_TongHopDenKhoiDonVi.xls";

        private const string sFilePath_TongHop_PhuLuc_DonVi =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_PhuLuc_TongHopDenDonVi.xls";
        //"/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_PhuLuc_TongHopDenDonVi_Gop.xls";

        private const string sFilePath_ChiTiet_PhuLuc =
            //"/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_ChiTiet_PhuLuc.xls";
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_ChiTiet_PhuLuc_Gop.xls";

        private const string sFilePath_ChiTiet_PhuLuc_PhongBan =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_ChiTiet_PhuLuc_PhongBan.xls";


        public static string NameFile = "";

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _qtService = QuyetToanService.Default;

        private string _loaiBaoCao;
        private string _iID_MaNamNganSach;
        private string _iID_MaPhongBan;
        public string sViewPath = "~/Report_Views/";

        public ActionResult Index()
        {
            var isTLTHCuc = GetPhongBanId(PhienLamViec.iID_MaPhongBan).IsEmpty();
            var phongBanList = new Dictionary<string, string>();
            if (isTLTHCuc)
            {
                phongBanList.Add("-1", "<-- Tổng hợp cục -->");

                _nganSachService.GetPhongBanQuanLyNS().AsEnumerable().ToList()
                    .ForEach(r => { phongBanList.Add(r.Field<string>("iID_MaPhongBan"), r.Field<string>("sMoTa")); });
            }
            else
            {
                phongBanList.Add(PhienLamViec.iID_MaPhongBan, PhienLamViec.sTenPhongBanFull);
            }

            var vm = new rptQuyetToanNam_PheDuyetViewModel
            {
                PhongBanList = phongBanList.ToSelectList(),
                NamNganSachList = DataHelper.GetNamNganSachList().ToSelectList(),
                NguonNganSachList = DataHelper.GetNguonNganSachList().ToSelectList()
            };

            var _viewPath = "~/Views/Report_Views/QuyetToan/";
            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        public ActionResult Print(
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string iID_MaNamNganSach,
            string iID_MaNguonNganSach,
            string loaiBaoCao,
            string mauBaoCao,
            string ext)
        {
            _loaiBaoCao = loaiBaoCao;
            _iID_MaNamNganSach = iID_MaNamNganSach;
            _iID_MaPhongBan = iID_MaPhongBan;

            if (loaiBaoCao == "100")
            {
                var xls = PrintTH();
                return Print(xls, ext);
            }
            else { 
                var filepath = getXls(loaiBaoCao, mauBaoCao);
                var xls = createReport(filepath, Username, iID_MaPhongBan, iID_MaDonVi, iID_MaNamNganSach, loaiBaoCao,
                    iID_MaNguonNganSach, mauBaoCao);
                return Print(xls, ext);
            }
        }


        public JsonResult Ds_DonVi(string iID_MaPhongBan, string iID_MaNamNganSach)
        {
            var data = _qtService.GetDonVis(PhienLamViec.iNamLamViec, Username, iID_MaNamNganSach,
                GetPhongBanId(iID_MaPhongBan), PhienLamViec.DonViList.Select(x => x.Key).Join());
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sTenDonVi"));
            return ToCheckboxList(vm);
        }

        private ExcelFile createReport(string path, string MaND, string iID_MaPhongBan, string iID_MaDonVi,
            string iID_MaNamNganSach, string iLoai, string iID_MaNguonNganSach, string iID_MaMauBaoCao)
        {
            var xls = new XlsFile(true);
            xls.Open(path);
            var fr = new FlexCelReport();

            LoadData(fr, MaND, iID_MaPhongBan, iID_MaDonVi, iID_MaNamNganSach, iID_MaNguonNganSach);

            //lay ten nam ngan sach
            var NamNganSach = "";
            if (iID_MaNamNganSach == "1,5")
                NamNganSach = "NĂM TRƯỚC CHUYỂN SANG";
            else if (iID_MaNamNganSach == "2,4")
                NamNganSach = "NGÂN SÁCH NĂM NAY";
            else
            {
                NamNganSach = "TỔNG HỢP";
            }
            //Ten mâu bieu
            var TenBieu = "Phụ lục";
            var NguonNganSach = "";
            if (iID_MaNguonNganSach == "1")
            {
                if (iLoai == "2") TenBieu = "Phụ lục 4";
                else if (iLoai == "9") TenBieu = "Phụ lục 4";
                NguonNganSach = "NGÂN SÁCH QUỐC PHÒNG";
            }
            else if (iID_MaNguonNganSach.Contains("2"))
            {
                if (iLoai == "2") TenBieu = "Phụ lục 4";
                else if (iLoai == "9") TenBieu = "Phụ lục 4";
                NguonNganSach = "NGÂN SÁCH NHÀ NƯỚC GIAO";
            }
            else if (iID_MaNguonNganSach == "-1")
            {
                NguonNganSach = "NGÂN SÁCH";
            }
            else
            {
                if (iLoai == "2") TenBieu = "Phụ lục 4";
                else if (iLoai == "9") TenBieu = "Phụ lục 4";
                NguonNganSach = "NGÂN SÁCH KHÁC";
            }
            // dat chung la Phu luc 4
            //TenBieu = "Phụ lục 4";


            //Ten BQL
            var BQuanLy = string.Empty;
            if (!GetPhongBanId(iID_MaPhongBan).IsEmpty())
                BQuanLy = _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa;


            //Ten Don vi
            var sTenDonVi = "";
            if (iID_MaDonVi != "-1" && (iLoai == "99" || iLoai == "98"))
                sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);

            var TenPB = string.Empty;
            if (iID_MaPhongBan != "-1")
                TenPB = " B - " + iID_MaPhongBan;


            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, MaND));
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("NamNganSach", NamNganSach);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("TenBieu", TenBieu);
            fr.SetValue("NguonNganSach", NguonNganSach);
            fr.SetValue("BQuanLy", BQuanLy);

            fr.UseCommonValue(new FlexcelModel
            {
                header1 = NamNganSach,
                header2 = "Đơn vị tính: đồng  Trang: "
            }
                )
                .UseChuKy()
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        #region private methods

        private DataTable rptQuyetToanNam_SoPheDuyet(string Username, string iID_MaPhongBan, string iID_MaDonVi,
            string iID_MaNamNganSach, string iID_MaNguonNganSach)
        {
            if (iID_MaDonVi == "-1")
                iID_MaDonVi = PhienLamViec.DonViList.Select(x => x.Key).Join();


            //if (PhienLamViec.iID_MaPhongBan == "11" || PhienLamViec.iID_MaPhongBan == "02")
            //{
            //    iID_MaPhongBan = string.Empty;
            //}
            //else if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
            //{
            //    iID_MaPhongBan = PhienLamViec.iID_MaPhongBan;
            //}
            if (!string.IsNullOrWhiteSpace(iID_MaPhongBan) || iID_MaPhongBan == "-1")
            {
                if (PhienLamViec.iID_MaPhongBan != "11" && PhienLamViec.iID_MaPhongBan != "02")
                {
                    iID_MaPhongBan = PhienLamViec.iID_MaPhongBan;
                }
            }

            #region fix benh vien tu chu cua b6

            if (iID_MaDonVi == "50") iID_MaDonVi = "50,501,502";
            else if (iID_MaDonVi == "34") iID_MaDonVi = "34,341,342";

            #endregion

            var sql = FileHelpers.GetSqlQuery("qt_sopheduyet_phongban.sql");
            //var sql = _loaiBaoCao == "98" ?
            //    FileHelpers.GetSqlQuery("qt_sopheduyet_phongban.sql") :
            //    FileHelpers.GetSqlQuery("qt_sopheduyet.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                //cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                //cmd.Parameters.AddWithValue("@iID_MaPhongBan", ToParamPhongBan(iID_MaPhongBan));
                //cmd.Parameters.AddWithValue("@iID_MaNamNganSach",
                //    iID_MaNamNganSach == "0" || string.IsNullOrWhiteSpace(iID_MaNamNganSach) ? "1,2" : iID_MaNamNganSach);
                ////cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach.ToParamString(iID_MaNguonNganSach == "-1"));
                //cmd.Parameters.AddWithValue("@iID_MaNguonNganSach",
                //    iID_MaNguonNganSach.ToParamString(iID_MaNguonNganSach == "-1"));
                //cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                //cmd.Parameters.AddWithValue("@lns", iID_MaNguonNganSach.ToParamString());

                //var dt = cmd.GetTable();
                var dt = conn.GetTable(sql, new
                {
                    PhienLamViec.iNamLamViec,
                    iID_MaPhongBan = ToParamPhongBan(iID_MaPhongBan),
                    iID_MaNamNganSach = iID_MaNamNganSach == "0" || string.IsNullOrWhiteSpace(iID_MaNamNganSach) ? "1,2" : iID_MaNamNganSach,
                    iID_MaNguonNganSach = iID_MaNguonNganSach.ToParamString(iID_MaNguonNganSach == "-1"),
                    iID_MaDonVi = iID_MaDonVi.ToParamString(),
                    lns = iID_MaNguonNganSach.ToParamString(),
                });
                return dt;
            }
        }

        private void LoadData(FlexCelReport fr, string MaND, string iID_MaPhongBan, string iID_MaDonVi,
            string iID_MaNamNganSach, string iID_MaNguonNganSach)
        {
            DataRow r;
            var data = rptQuyetToanNam_SoPheDuyet(MaND, iID_MaPhongBan, iID_MaDonVi, iID_MaNamNganSach,
                iID_MaNguonNganSach);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            #region theo phongban

            if (_loaiBaoCao == "98")
            {
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS", PhienLamViec.iNamLamViec);
                fr.AddRelationship("dtsLNS", "ChiTiet", "sLNS1,sLNS3,sLNS5,sLNS".Split(','), "sLNS1,sLNS3,sLNS5,sLNS".Split(','));
            }

            #endregion

            #region binh thuong

            else
            {
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS PhongBan,sMoTa_PhongBan_Khoi", PhienLamViec.iNamLamViec);


                ////DataTable dtKhoiDonVi = HamChung.SelectDistinct("KhoiDV", data, "iID_MaPhongBan,sLNS1,sLNS3,sLNS5,sLNS,iID_MaKhoiDonVi", "iID_MaKhoiDonVi,sTenKhoiDonVi,iID_MaDonVi,sTenDonVi,iID_MaPhongBan,sLNS1,sLNS3,sLNS5,sLNS,sMoTa,iSTT");
                //DataTable dtPhongBan = HamChung.SelectDistinct("PhongBan", data, "sLNS1,sLNS3,sLNS5,sLNS,iID_MaPhongBan", "iID_MaPhongBan,sLNS1,sLNS3,sLNS5,sLNS");

                //DataTable dtLNS = HamChung.SelectDistinct("dtLNS", dtPhongBan, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS");

                //DataTable dtLNS5 = HamChung.SelectDistinct("dtLNS5", dtLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5");
                //for (int i = 0; i < dtLNS5.Rows.Count; i++)
                //{
                //    r = dtLNS5.Rows[i];
                //    //r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS5"]));
                //    r["sMoTa"] = getMLNS_MoTa(PhienLamViec.iNamLamViec, r.Field<string>("sLNS5"), r.Field<string>("sLNS5"));
                //}

                //DataTable dtLNS3 = HamChung.SelectDistinct("dtLNS3", dtLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa");
                //for (int i = 0; i < dtLNS3.Rows.Count; i++)
                //{
                //    r = dtLNS3.Rows[i];
                //    //r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS3"]));
                //    r["sMoTa"] = getMLNS_MoTa(PhienLamViec.iNamLamViec, r.Field<string>("sLNS3"), r.Field<string>("sLNS3"));

                //}

                //DataTable dtLNS1 = HamChung.SelectDistinct("dtLNS1", dtLNS3, "sLNS1", "sLNS1,sMoTa");

                //for (int i = 0; i < dtLNS1.Rows.Count; i++)
                //{
                //    r = dtLNS1.Rows[i];
                //    r["sMoTa"] = getMLNS_MoTa(PhienLamViec.iNamLamViec, r.Field<string>("sLNS1"));
                //    //r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS1"]));
                //}
                //fr.AddTable("KhoiDV", dtPhongBan);
                //fr.AddTable("dtLNS", dtLNS);
                //fr.AddTable("dtLNS5", dtLNS5);
                //fr.AddTable("dtLNS3", dtLNS3);
                //fr.AddTable("dtLNS1", dtLNS1);
                //dtPhongBan.Dispose();
                //dtLNS.Dispose();
                //dtLNS5.Dispose();
                //dtLNS3.Dispose();
                //dtLNS1.Dispose();
                //data.Dispose();
            }

            //else
            //{
            //    //DataTable dtKhoiDonVi = HamChung.SelectDistinct("KhoiDV", data, "iID_MaPhongBan,sLNS1,sLNS3,sLNS5,sLNS,iID_MaKhoiDonVi", "iID_MaKhoiDonVi,sTenKhoiDonVi,iID_MaDonVi,sTenDonVi,iID_MaPhongBan,sLNS1,sLNS3,sLNS5,sLNS,sMoTa,iSTT");
            //    DataTable dtKhoiDonVi = HamChung.SelectDistinct("KhoiDV", data, "sLNS1,sLNS3,sLNS5,sLNS,iID_MaKhoiDonVi,sTenKhoiDonVi", "iID_MaKhoiDonVi,sTenKhoiDonVi,sLNS1,sLNS3,sLNS5,sLNS,iSTT");
            //    DataView dv = dtKhoiDonVi.DefaultView;
            //    dv.Sort = "iSTT";
            //    dtKhoiDonVi = dv.ToTable();

            //    DataTable dtLNS = HamChung.SelectDistinct("dtLNS", dtKhoiDonVi, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS");

            //    DataTable dtLNS5 = HamChung.SelectDistinct("dtLNS5", dtLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5");
            //    for (int i = 0; i < dtLNS5.Rows.Count; i++)
            //    {
            //        r = dtLNS5.Rows[i];
            //        //r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS5"]));
            //        r["sMoTa"] = getMLNS_MoTa(PhienLamViec.iNamLamViec, r.Field<string>("sLNS5"), r.Field<string>("sLNS5"));
            //    }

            //    DataTable dtLNS3 = HamChung.SelectDistinct("dtLNS3", dtLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa");
            //    for (int i = 0; i < dtLNS3.Rows.Count; i++)
            //    {
            //        r = dtLNS3.Rows[i];
            //        //r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS3"]));
            //        r["sMoTa"] = getMLNS_MoTa(PhienLamViec.iNamLamViec, r.Field<string>("sLNS3"), r.Field<string>("sLNS3"));

            //    }

            //    DataTable dtLNS1 = HamChung.SelectDistinct("dtLNS1", dtLNS3, "sLNS1", "sLNS1,sMoTa");

            //    for (int i = 0; i < dtLNS1.Rows.Count; i++)
            //    {
            //        r = dtLNS1.Rows[i];
            //        r["sMoTa"] = getMLNS_MoTa(PhienLamViec.iNamLamViec, r.Field<string>("sLNS1"));
            //        //r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS1"]));
            //    }
            //    fr.AddTable("KhoiDV", dtKhoiDonVi);
            //    fr.AddTable("dtLNS", dtLNS);
            //    fr.AddTable("dtLNS5", dtLNS5);
            //    fr.AddTable("dtLNS3", dtLNS3);
            //    fr.AddTable("dtLNS1", dtLNS1);
            //    dtKhoiDonVi.Dispose();
            //    dtLNS.Dispose();
            //    dtLNS5.Dispose();
            //    dtLNS3.Dispose();
            //    dtLNS1.Dispose();
            //    data.Dispose();
            //}

            #endregion
        }

        private string getXls(string loaiBaoCao, string mauBaoCao)
        {
            var filePath = string.Empty;
            if (mauBaoCao == "1") // phu luc
            {
                if (loaiBaoCao == "9")
                    filePath = sFilePath_TongHop_DonVi;
                else if (loaiBaoCao == "2")
                    filePath = sFilePath_TongHop_Khoi;
                else if (loaiBaoCao == "3")
                    filePath = sFilePath_TongHop_Khoi2;
                else if (loaiBaoCao == "30")
                    filePath = sFilePath_TongHop_Khoi_DonVi;
                else if (loaiBaoCao == "1")
                    filePath = sFilePath_TongHop_LNS;
                else if (loaiBaoCao == "99")
                    filePath = sFilePath_ChiTiet_PhuLuc;
                else if (loaiBaoCao == "98")
                    filePath = sFilePath_ChiTiet_PhuLuc_PhongBan;
            }
            else
            {
                if (loaiBaoCao == "9")
                    filePath = sFilePath_TongHop_PhuLuc_DonVi;
                else if (loaiBaoCao == "2")
                    filePath = sFilePath_TongHop_PhuLuc_Khoi;
                else if (loaiBaoCao == "1")
                    filePath = sFilePath_TongHop_PhuLuc_LNS;
                else if (loaiBaoCao == "99")
                    filePath = sFilePath_ChiTiet_PhuLuc;
                else if (loaiBaoCao == "98")
                    filePath = sFilePath_ChiTiet_PhuLuc_PhongBan;
            }

            return Server.MapPath(filePath);
        }

        public ExcelFile PrintTH()
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath("~/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoPheDuyet_TongHop.xls"));
            var fr = new FlexCelReport();

            var data = new DataTable();

            var sql = FileHelpers.GetSqlQuery("qt_sopheduyet_donvi.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                data = conn.GetTable(sql, new
                {
                    PhienLamViec.iNamLamViec,
                    iID_MaPhongBan = ToParamPhongBan(_iID_MaPhongBan),
                    iID_MaNamNganSach = _iID_MaNamNganSach == "0" || string.IsNullOrWhiteSpace(_iID_MaNamNganSach) ? "1,2" : _iID_MaNamNganSach,
                });
            }
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            //lay ten nam ngan sach
            var NamNganSach = "";
            if (_iID_MaNamNganSach == "1,5")
                NamNganSach = "NĂM TRƯỚC CHUYỂN SANG";
            else if (_iID_MaNamNganSach == "2,4")
                NamNganSach = "NGÂN SÁCH NĂM NAY";
            else
            {
                NamNganSach = "TỔNG HỢP";
            }
            //Ten mâu bieu
            var TenBieu = "Phụ lục";      

            //Ten BQL
            var BQuanLy = string.Empty;
            if (!GetPhongBanId(_iID_MaPhongBan).IsEmpty())
                BQuanLy = _nganSachService.GetPhongBanById(_iID_MaPhongBan).sMoTa;
            
            var TenPB = string.Empty;
            if (_iID_MaPhongBan != "-1")
                TenPB = " B - " + _iID_MaPhongBan;


            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, User.Identity.Name));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, User.Identity.Name));
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("NamNganSach", NamNganSach);
            fr.SetValue("TenBieu", TenBieu);
            fr.SetValue("BQuanLy", BQuanLy);

            fr.UseCommonValue(new FlexcelModel
            {
                header1 = NamNganSach,
                header2 = "Đơn vị tính: đồng - Trang: "
            }
                )
                .UseChuKy()
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);
            
            return xls;
        }

        #endregion
    }
}