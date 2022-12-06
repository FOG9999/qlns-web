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
using VIETTEL.Application.Flexcel;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptQuyetToanNam_SoChuyenNamSauViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptQuyetToanNam_SoChuyenNamSauController : FlexcelReportController
    {
        public string sViewPath = "~/Report_Views/";

        private const String sFilePath_tonghop_lns = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoChuyenNamSau_LNS.xls";
        private const String sFilePath_tonghop_lns_muc = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoChuyenNamSau_LNS_Muc_DonVi.xls";
        private const String sFilePath_tonghop_lns_ng = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoChuyenNamSau_LNS_NG_DonVi.xls";
        private const String sFilePath_tonghop_donvi = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoChuyenNamSau_DonVi.xls";
        private const String sFilePath_tonghop_donvi_lns = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoChuyenNamSau_ChiTiet.xls";

        private readonly INganSachService _nganSachService;
        private readonly IQuyetToanService _qtService;

        private string _iID_MaPhongBan;
        private string _iID_MaDonVi;

        public rptQuyetToanNam_SoChuyenNamSauController()
        {
            _nganSachService = NganSachService.Default;
            _qtService = QuyetToanService.Default;
        }

        public rptQuyetToanNam_SoChuyenNamSauController(INganSachService ngansachService, IQuyetToanService quyettoanService)
        {
            _nganSachService = ngansachService ?? NganSachService.Default;
            _qtService = quyettoanService ?? QuyetToanService.Default;
        }

        public ActionResult Index()
        {
            var vm = new rptQuyetToanNam_SoChuyenNamSauViewModel
            {
                PhongBanList = GetPhongBanList(_nganSachService),
            };

            var _viewPath = "~/Views/Report_Views/QuyetToan/";
            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);

        }

        public ActionResult Print(
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string loaiBaoCao,
            string mauBaoCao,
            string lns,
            string ext)
        {

            if (string.IsNullOrWhiteSpace(iID_MaDonVi))
                iID_MaDonVi = PhienLamViec.DonViList.Select(x => x.Key).Join();

            _iID_MaPhongBan = iID_MaPhongBan;
            _iID_MaDonVi = iID_MaDonVi;

            if (mauBaoCao == "100")
            {
                var xls = PrintTH();
                return Print(xls, ext);
            }
            else
            {
                var filepath = getXls(loaiBaoCao, mauBaoCao);
                var xls = createReport(filepath, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao, mauBaoCao, lns);
                return Print(xls, ext);
            }
        }       

        public JsonResult Ds_DonVi(string iID_MaPhongBan)
        {
            //var data = _qtService.GetDonVis(PhienLamViec.iNamLamViec, Username, null, iID_MaPhongBan, PhienLamViec.DonViList.Select(x => x.Key).Join());
            var data = getDonVis(iID_MaPhongBan, PhienLamViec.DonViList.Select(x => x.Key).Join());
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sTenDonVi"));
            return ToCheckboxList(vm);
        }

        #region private methods

        private DataTable getDataTable(string iID_MaPhongBan, string iID_MaDonVi)
        {
            var sql = FileHelpers.GetSqlQuery("qt_sochuyennamsau.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.NamLamViec + 1);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", ToParamPhongBan(iID_MaPhongBan));
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());

                return cmd.GetTable();
            }
        }

        private DataTable getDonVis(string iID_MaPhongBan, string iID_MaDonVi)
        {
            var sql = FileHelpers.GetSqlQuery("qt_sochuyennamsau_donvi.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                // dung cho 2017 -> 2018
                //cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);

                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.NamLamViec + 1);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", ToParamPhongBan(iID_MaPhongBan));
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());

                return cmd.GetTable();
            }
        }
        private void loadData(FlexCelReport fr, String iID_MaPhongBan, String iID_MaDonVi)
        {
            DataRow r;
            DataTable data = getDataTable(iID_MaPhongBan, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            DataTable dtLNS = HamChung.SelectDistinct("dtLNS", data, "sLNS1,sLNS3,sLNS5,sLNS", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sMoTa");



            DataTable dtLNS5 = HamChung.SelectDistinct("dtLNS5", dtLNS, "sLNS1,sLNS3,sLNS5", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sMoTa");
            for (int i = 0; i < dtLNS5.Rows.Count; i++)
            {
                r = dtLNS5.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS5"]), 5);
            }

            DataTable dtLNS3 = HamChung.SelectDistinct("dtLNS3", dtLNS5, "sLNS1,sLNS3", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sMoTa");
            for (int i = 0; i < dtLNS3.Rows.Count; i++)
            {
                r = dtLNS3.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS3"]), 3);
            }

            DataTable dtLNS1 = HamChung.SelectDistinct("dtLNS1", dtLNS3, "sLNS1", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sMoTa");

            for (int i = 0; i < dtLNS1.Rows.Count; i++)
            {
                r = dtLNS1.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS1"]), 1);
            }
            fr.AddTable("dtLNS", dtLNS);
            fr.AddTable("dtLNS5", dtLNS5);
            fr.AddTable("dtLNS3", dtLNS3);
            fr.AddTable("dtLNS1", dtLNS1);
            dtLNS.Dispose();
            dtLNS5.Dispose();
            dtLNS3.Dispose();
            dtLNS1.Dispose();
            data.Dispose();
        }

        public ExcelFile createReport(string path,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string loaiBaoCao,
            string mauBaoCao,
            string lns)
        {
            var xls = new XlsFile(true);
            xls.Open(path);

            var fr = new FlexCelReport();
            //fr = ReportModels.LayThongTinChuKy(fr, "rptQuyetToanNam_ThongBaoSoChuyenNamSau");
            if (mauBaoCao == "3" || mauBaoCao == "4")
            {
                loadData_ChiTiet(fr, iID_MaPhongBan, iID_MaDonVi, lns);

                var tenLNS = string.Empty;
                if (lns == "1")
                {
                    lns = "Ngân sách quốc phòng";
                }
                else if (lns == "4")
                {
                    lns = "Ngân sách khác";
                }
                else
                {
                    lns = "Ngân sách nhà nước";
                }
                fr.SetValue("lns", lns);
            }
            else if (mauBaoCao == "11")
            {
                loadData_LNS_Muc_DonVi(fr, iID_MaPhongBan, iID_MaDonVi, lns);
            }
            else if (mauBaoCao == "12")
            {
                loadData_LNS_NG_DonVi(fr, iID_MaPhongBan, iID_MaDonVi, lns);
            }
            else
                loadData(fr, iID_MaPhongBan, iID_MaDonVi);


            if (loaiBaoCao == "chitiet")
            {
                iID_MaDonVi = iID_MaDonVi.Split(',').FirstOrDefault();
                fr.SetValue("sTenDonVi", _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen);
            }
            else
                fr.SetValue("sTenDonVi", "");



            //fr.SetValue("sTenPhongBan", string.IsNullOrWhiteSpace(iID_MaPhongBan) || iID_MaPhongBan == "-1" ? "" : _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa);
            fr.SetValue("sTenPhongBan", ToParamPhongBan(iID_MaPhongBan) == DBNull.Value ? "" : _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa);

            fr.UseCommonValue(model: new Application.Flexcel.FlexcelModel
            {
                //header = 
                header2 = "Đơn vị tính: đồng   Trang: ",
            }, dvt: 1)
                .UseChuKy()
                .UseChuKyForController(this.ControllerName(), iID_MaPhongBan)
                .UseForm(this);

            // voi nam 2018, dung tam cho chuyen nam 2017
            //fr.SetValue("Nam", PhienLamViec.iNamLamViec == "2018" ? int.Parse(PhienLamViec.iNamLamViec) - 1 : int.Parse(PhienLamViec.iNamLamViec));
            fr.Run(xls);
            return xls;
        }


        private string getXls(string loaiBaoCao, string mauBaoCao)
        {
            var filePath = string.Empty;
            if (loaiBaoCao == "tonghop")
            {
                if (mauBaoCao == "1")
                {
                    filePath = sFilePath_tonghop_lns;
                }
                else if (mauBaoCao == "2")
                {
                    filePath = sFilePath_tonghop_donvi;
                }
                else if (mauBaoCao == "11")
                {
                    filePath = sFilePath_tonghop_lns_muc;
                }
                else if (mauBaoCao == "12")
                {
                    filePath = sFilePath_tonghop_lns_ng;
                }
                else
                {
                    filePath = sFilePath_tonghop_donvi_lns;
                }
            }
            else
            {
                if (mauBaoCao == "3")
                    filePath = sFilePath_tonghop_donvi_lns;
                else
                    filePath = sFilePath_tonghop_lns;
            }

            return Server.MapPath(filePath);
        }

        #endregion

        #region chitiet

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        private void loadData_ChiTiet(FlexCelReport fr, String iID_MaPhongBan, String iID_MaDonVi, string lns)
        {
            DataRow r;
            DataTable data = getDataTable_ChiTiet(iID_MaPhongBan, iID_MaDonVi, lns);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,iID_MaDonVi,sL,sK,sM,sTM", "sLNS,iID_MaDonVi,sTenDonVi,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,iID_MaDonVi,sL,sK,sM", "sLNS,iID_MaDonVi,sTenDonVi,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,iID_MaDonVi,sL,sK", "sLNS,iID_MaDonVi,sTenDonVi,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            //DataTable dtDonVi = HamChung.SelectDistinct("dtDonVi", dtsL, "sLNS,iID_MaDonVi,sL,sK", "sLNS,iID_MaDonVi,sTenDonVi,sL,sK,sMoTa", "");
            DataTable dtDonVi = HamChung.SelectDistinct("dtDonVi", dtsL, "sLNS,iID_MaDonVi,sTenDonVi", "sLNS,iID_MaDonVi,sTenDonVi", "");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "sLNS,sMoTa", "");

            dtsLNS.AsEnumerable().ToList()
                .ForEach(row =>
                {
                    row["sMoTa"] = _nganSachService.GetMLNS_MoTa(row.Field<string>("sLNS"));
                });

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtDonVi", dtDonVi);
            fr.AddTable("dtsLNS", dtsLNS);

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtDonVi.Dispose();
            dtsLNS.Dispose();

        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        private void loadData_LNS_Muc_DonVi(FlexCelReport fr, String iID_MaPhongBan, String iID_MaDonVi, string lns)
        {
            DataRow r;
            DataTable data = getDataTable_ChiTiet_Muc(iID_MaPhongBan, iID_MaDonVi, lns);
            data.TableName = "ChiTiet";

            DataTable dtM = HamChung.SelectDistinct("dtM", data, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa");
            DataTable dtLNS = HamChung.SelectDistinct("dtLNS", dtM, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");
            DataTable dtLNS5 = HamChung.SelectDistinct("dtLNS5", dtLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");


            for (int i = 0; i < dtLNS.Rows.Count; i++)
            {
                r = dtLNS.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS"]), 7);
            }

            for (int i = 0; i < dtLNS5.Rows.Count; i++)
            {
                r = dtLNS5.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS5"]), 5);
            }

            DataTable dtLNS3 = HamChung.SelectDistinct("dtLNS3", dtLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");
            for (int i = 0; i < dtLNS3.Rows.Count; i++)
            {
                r = dtLNS3.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS3"]), 3);
            }

            DataTable dtLNS1 = HamChung.SelectDistinct("dtLNS1", dtLNS3, "sLNS1", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");

            for (int i = 0; i < dtLNS1.Rows.Count; i++)
            {
                r = dtLNS1.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS1"]), 1);
            }
            fr.AddTable(data);
            fr.AddTable(dtM);
            fr.AddTable(dtLNS);
            fr.AddTable(dtLNS5);
            fr.AddTable(dtLNS3);
            fr.AddTable(dtLNS1);

            dtM.Dispose();
            dtLNS.Dispose();
            dtLNS5.Dispose();
            dtLNS3.Dispose();
            dtLNS1.Dispose();
            data.Dispose();

        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        private void loadData_LNS_NG_DonVi(FlexCelReport fr, String iID_MaPhongBan, String iID_MaDonVi, string lns)
        {
            DataRow r;
            DataTable data = getDataTable_ChiTiet_NG(iID_MaPhongBan, iID_MaDonVi, lns);
            data.TableName = "ChiTiet";

            var dtNG = HamChung.SelectDistinct("dtNG", data, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa");
            var dtTM = HamChung.SelectDistinct("dtTM", dtNG, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa");
            var dtM = HamChung.SelectDistinct("dtM", dtTM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa");
            var dtL = HamChung.SelectDistinct("dtL", dtM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sMoTa");
            var dtLNS = HamChung.SelectDistinct("dtLNS", dtM, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");
            var dtLNS5 = HamChung.SelectDistinct("dtLNS5", dtLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");


            for (int i = 0; i < dtTM.Rows.Count; i++)
            {
                r = dtTM.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa($"{r["sLNS"]}-{r["sL"]}-{r["sK"]}-{r["sM"]}-{r["sTM"]}");
            }
            for (int i = 0; i < dtM.Rows.Count; i++)
            {
                r = dtM.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa($"{r["sLNS"]}-{r["sL"]}-{r["sK"]}-{r["sM"]}");
            }

            for (int i = 0; i < dtL.Rows.Count; i++)
            {
                r = dtL.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa($"{r["sLNS"]}-{r["sL"]}-{r["sK"]}");
            }

            for (int i = 0; i < dtLNS.Rows.Count; i++)
            {
                r = dtLNS.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS"]), 7);
            }

            for (int i = 0; i < dtLNS5.Rows.Count; i++)
            {
                r = dtLNS5.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS5"]), 5);
            }

            DataTable dtLNS3 = HamChung.SelectDistinct("dtLNS3", dtLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");
            for (int i = 0; i < dtLNS3.Rows.Count; i++)
            {
                r = dtLNS3.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS3"]), 3);
            }

            DataTable dtLNS1 = HamChung.SelectDistinct("dtLNS1", dtLNS3, "sLNS1", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa");

            for (int i = 0; i < dtLNS1.Rows.Count; i++)
            {
                r = dtLNS1.Rows[i];
                r["sMoTa"] = _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, Convert.ToString(r["sLNS1"]), 1);
            }
            fr.AddTable(data);
            fr.AddTable(dtNG);
            fr.AddTable(dtTM);
            fr.AddTable(dtM);
            fr.AddTable(dtL);
            fr.AddTable(dtLNS);
            fr.AddTable(dtLNS5);
            fr.AddTable(dtLNS3);
            fr.AddTable(dtLNS1);

            dtM.Dispose();
            dtLNS.Dispose();
            dtLNS5.Dispose();
            dtLNS3.Dispose();
            dtLNS1.Dispose();
            data.Dispose();

        }

        private DataTable getDataTable_ChiTiet(string iID_MaPhongBan, string iID_MaDonVi, string lns)
        {
            var sql = FileHelpers.GetSqlQuery("qt_sochuyennamsau_chitiet.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", ToParamPhongBan(iID_MaPhongBan));
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                cmd.Parameters.AddWithValue("@lns", lns.ToParamString());

                return cmd.GetTable();
            }
        }
        private DataTable getDataTable_ChiTiet_Muc(string iID_MaPhongBan, string iID_MaDonVi, string lns)
        {

            var sTM = Request.GetQueryString("sTM", string.Empty);
            var sTTM = Request.GetQueryString("sTTM", string.Empty);
            var sNG = Request.GetQueryString("sNG", string.Empty);

            var sql = FileHelpers.GetSqlQuery("qt_sochuyennamsau_chitiet_muc.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", ToParamPhongBan(iID_MaPhongBan));
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());

                cmd.Parameters.AddWithValue("@lns", lns.ToParamString());
                cmd.Parameters.AddWithValue("@sTM", sTM.ToParamString());
                cmd.Parameters.AddWithValue("@sTTM", sTTM.ToParamString());
                cmd.Parameters.AddWithValue("@sNG", sNG.ToParamString());

                return cmd.GetTable();
            }
        }

        private DataTable getDataTable_ChiTiet_NG(string iID_MaPhongBan, string iID_MaDonVi, string lns)
        {

            var sM = Request.GetQueryString("sM", string.Empty);
            var sTM = Request.GetQueryString("sTM", string.Empty);
            var sTTM = Request.GetQueryString("sTTM", string.Empty);
            var sNG = Request.GetQueryString("sNG", string.Empty);

            var sql = FileHelpers.GetSqlQuery("qt_sochuyennamsau_chitiet_ng.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", ToParamPhongBan(iID_MaPhongBan));
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());

                cmd.Parameters.AddWithValue("@lns", lns.ToParamString());
                cmd.Parameters.AddWithValue("@sM", sM.ToParamString());
                cmd.Parameters.AddWithValue("@sTM", sTM.ToParamString());
                cmd.Parameters.AddWithValue("@sTTM", sTTM.ToParamString());
                cmd.Parameters.AddWithValue("@sNG", sNG.ToParamString());

                return cmd.GetTable();
            }
        }
        #endregion

        private ExcelFile PrintTH()
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath("~/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_SoChuyenNamSau_TongHop.xls"));
            var fr = new FlexCelReport();

            var data = new DataTable();

            var sql = FileHelpers.GetSqlQuery("qt_sochuyennamsau_tonghop_donvi.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                data = conn.GetTable(sql, new
                {
                    nam = PhienLamViec.NamLamViec + 1,
                    PhienLamViec.iNamLamViec,
                    iID_MaPhongBan = _iID_MaPhongBan.ToParamString(),
                    iID_MaDonVi = _iID_MaDonVi.ToParamString(),
                });
            }
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            
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
            fr.SetValue("N", PhienLamViec.iNamLamViec);
            fr.SetValue("N+1", PhienLamViec.NamLamViec + 1);
            fr.SetValue("BQuanLy", BQuanLy);

            fr.UseCommonValue(new FlexcelModel
            {
                header2 = "Đơn vị tính: đồng - Trang: "
            }
                )
                .UseChuKy()
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }
    }
}
