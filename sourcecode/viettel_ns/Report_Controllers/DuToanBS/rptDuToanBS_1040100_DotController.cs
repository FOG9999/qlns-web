using DomainModel;
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
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptDuToanBS_1040100_DotViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList DotList { get; set; }
    }
}


namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToanBS_1040100_DotController : FlexcelReportController
    {

        private string _viewPath = "~/Views/Report_Views/DuToanBS/";
        private string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Dot.xls";
        private string _filePath_TongHop = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Dot_TongHop.xls";
        String iCapTongHop = "";

        private readonly INganSachService _nganSachService = NganSachService.Default;

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;

            var dtDot = DuToanBS_ReportModels.LayDSDot(iNamLamViec, Username, "1040100");

            var vm = new rptDuToanBS_1040100_DotViewModel
            {
                iNamLamViec = iNamLamViec,
                DotList = dtDot.ToSelectList("iID_MaDot", "iID_MaDot", DateTime.Now.ToString("dd/MM/yyyy"), "-- Chọn đợt --"),
            };

            return View(view, vm);
        }


        #region public methods

        public ActionResult Print(
            string iID_MaDot,
            string iID_MaNganh,
            string type,
            string ext)
        {
            this.iCapTongHop = type;
            if (type == "tonghop")
            {
                _filePath = _filePath_TongHop;
            }
            var xls = createReport(Server.MapPath(_filePath), iID_MaDot, iID_MaNganh);
            return Print(xls, ext);
        }

        public JsonResult Ds_Nganh(string iID_MaDot)
        {
            var data = DuToanBS_ReportModels.dtNganh(iID_MaDot, Username, "1040100");
            var vm = new ChecklistModel("Nganh", data.ToSelectList("iID_MaNganh", "sTenNganh"));
            return ToCheckboxList(vm);
        }

        #endregion

        private ExcelFile createReport(string path, string iID_MaDot, string iID_MaNganh)
        {
            var fr = new FlexCelReport();
            loadReport(fr, iID_MaDot, iID_MaNganh);

            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sql);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            var tenNganh = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);

            fr.SetExpression("NgayDot", iID_MaDot.ToParamDate().ToString("dd/MM/yyyy"));
            fr.SetValue("TenNganh", tenNganh);

            fr.UseCommonValue()
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName());

            var xls = new XlsFile(true);
            xls.Open(path);

            fr.Run(xls);
            return xls;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        private DataTable DT_rptDuToanBS_1040100_Dot(string iID_MaDot, string iID_MaNganh)
        {
            //bool checkTL = LuongCongViecModel.KiemTra_TroLyPhongBan(Username);
            //if (checkTL)
            //{
            //    DKDonVi = ThuNopModels.DKDonVi(Username, cmd);
            //}

            #region sql

            var sql = @"

SELECT * FROM
(
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        rTuChi      =SUM(rTuChi)/@dvt,
        rTonKho     =SUM(rTonKho)/@dvt,
        rHangNhap   =SUM(rHangNhap)/@dvt,
        rHangMua    =SUM(rHangMua)/@dvt,
        rPhanCap    =SUM(rPhanCap)/@dvt,
        rDuPhong    =SUM(rDuPhong)/@dvt,
        CONVERT(nvarchar, dNgayChungTu, 103) as ngayChungTu,
        dNgayChungTu
FROM    DTBS_ChungTuChiTiet a INNER JOIN (SELECT iID_MaChungTu,dNgayChungTu FROM DTBS_ChungTu) b ON a.iID_MaChungTu = b.iID_MaChungTu
WHERE   iTrangThai=1 
        AND sLNS='1040100' 
        AND (MaLoai='' OR MaLoai='2') 
        AND iNamLamViec=@iNamLamViec
        AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from F_SplitString2(@iID_MaDonVi)))
        AND a.iID_MaChungTu IN (SELECT * FROM F_SplitString2(@iID_MaChungTu))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,dNgayChungTu
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rTonKho)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0

UNION ALL 

SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        rTuChi      =SUM(rTuChi)/@dvt,
        rTonKho     =SUM(rTonKho)/@dvt,
        rHangNhap   =SUM(rHangNhap)/@dvt,
        rHangMua    =SUM(rHangMua)/@dvt,
        rPhanCap    =SUM(rPhanCap)/@dvt,
        rDuPhong    =SUM(rDuPhong)/@dvt,
        CONVERT(nvarchar, dNgayChungTu, 103) as ngayChungTu,
        dNgayChungTu
FROM    DT_ChungTuChiTiet a INNER JOIN (SELECT iID_MaChungTu,dNgayChungTu FROM DT_ChungTu WHERE dNgayChungTu<=@dMaDot) b ON a.iID_MaChungTu = b.iID_MaChungTu
WHERE   iTrangThai=1 
        AND sLNS='1040100' 
        AND (MaLoai='' OR MaLoai='2') 
        AND iNamLamViec=@iNamLamViec
        AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from F_SplitString2(@iID_MaDonVi)))

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,dNgayChungTu
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rTonKho)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0
) as T1

ORDER BY dNgayChungTu 

";
            #endregion

            #region dieu kien

            string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            var sNG = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
            var iID_MaChungTu = DuToanBS_ReportModels.GetChungTuList_DenDot(iID_MaDot, Username);
            var iID_MaDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(Username)
                            .AsEnumerable()
                            .Select(x => x.Field<string>("iID_MaDonVi"))
                            .Join();
            #endregion

            #region load data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd.Parameters.AddWithValue("@sNG", sNG);
                    cmd.Parameters.AddWithValue("@dMaDot", iID_MaDot.ToParamDate());
                    cmd.Parameters.AddWithValue("@dvt", 1000);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }

            #endregion
        }


        private DataTable DT_rptDuToanBS_1040100_Dot_GhiChu(String MaND, String iID_MaDonVi, String iID_MaDot)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            bool checkTL = LuongCongViecModel.KiemTra_TroLyPhongBan(MaND);
            if (checkTL)
            {
                DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
                //  DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            }

            int i = 0;
            String DSNganh = "";
            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
            SqlCommand cmdNganh = new SqlCommand(sql);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            var iID_MaNganhMLNS = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);

            if (String.IsNullOrEmpty(iID_MaNganhMLNS))
                iID_MaNganhMLNS = "-1";
            DSNganh = " AND sNG IN (" + iID_MaNganhMLNS + ")";

            int DVT = 1000;
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
,rTuChi=SUM(rTuChi/{3})
,rTonKho=SUM(rTonKho/{3})
,rHangNhap=SUM(rHangNhap/{3})
,rHangMua=SUM(rHangMua/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
,sGhiChu
 FROM DTBS_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS='1040100'  AND (MaLoai='' OR MaLoai='2') AND iNamLamViec=@iNamLamViec {1} {2} {0}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu
HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rTonKho)<>0  OR SUM(rHangMua)<>0  OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0

UNION ALL

SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
,rTuChi=SUM(rTuChi/{3})
,rTonKho=SUM(rTonKho/{3})
,rHangNhap=SUM(rHangNhap/{3})
,rHangMua=SUM(rHangMua/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
,sGhiChu
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS='1040100'  AND (MaLoai='' OR MaLoai='2') AND iNamLamViec=@iNamLamViec {1} {2} {0}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu
HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rTonKho)<>0  OR SUM(rHangMua)<>0  OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
", DSNganh, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);

            cmd.Dispose();
            return dt;
        }


        private void loadReport(FlexCelReport fr, string iID_MaDot, string iID_MaNganh)
        {
            //DataTable data = DT_rptDuToanBS_1040100_Dot(iID_MaDot, iID_MaNganh);
            var data = CacheService.Default.CachePerRequest<DataTable>(this.ControllerName(),
                () => DT_rptDuToanBS_1040100_Dot(iID_MaDot, iID_MaNganh),
                Viettel.Domain.DomainModel.CacheTimes.OneMinute);

            // test
            FillDataTableLNS(fr, data, FillLNSType.LNS, PhienLamViec.iNamLamViec, "dNgayChungTu,ngayChungTu");

            var dtsDot = data.SelectDistinct("dtsDot", "dNgayChungTu,ngayChungTu");
            fr.AddTable("dtsDot", dtsDot, TDisposeMode.DisposeAfterRun);


            DataTable dataGhiChu = DT_rptDuToanBS_1040100_Dot_GhiChu(Username, iID_MaNganh, iID_MaDot);
            fr.AddTable("GhiChu", dataGhiChu, TDisposeMode.DisposeAfterRun);

            DataTable dtMoTaGhiChu = dataGhiChu.SelectDistinct("dtMoTaGhiChu", "sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu", "");
            fr.AddTable("dtMoTaGhiChu", dtMoTaGhiChu);
            bool hasGhiChu = false;


            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
            {
                var r = dtMoTaGhiChu.Rows[i];
                if (!String.IsNullOrEmpty(r["sGhiChu"].ToString()))
                {
                    hasGhiChu = true;
                    break;
                }
            }

            fr.SetValue("sGhiChu", hasGhiChu ? " * Ghi chú: " : "");
        }


        //        /// <summary>
        //        /// hàm hiển thị dữ liệu ra báo cáo
        //        /// </summary>
        //        /// <param name="fr"></param>
        //        /// <param name="NamLamViec"></param>
        //        private void LoadData_v1(FlexCelReport fr, String iID_MaDot, String iID_MaNganh)
        //        {
        //            DataRow r;
        //            DataTable data = DT_rptDuToanBS_1040100_Dot(iID_MaDot, iID_MaNganh);
        //            DataTable dataGhiChu = DT_rptDuToanBS_1040100_Dot_GhiChu(Username, iID_MaNganh, iID_MaDot);

        //            // test
        //            var iNamLamViec = ReportModels.LayNamLamViec(Username);

        //            FillDataTableLNS(fr, data, 4, iNamLamViec, "dNgayChungTu,ngayChungTu");

        //            var dtsDot = data.SelectDistinct("dtsDot", "dNgayChungTu,ngayChungTu");
        //            fr.AddTable("dtsDot", dtsDot, TDisposeMode.DisposeAfterRun);



        //            //data.TableName = "ChiTiet";
        //            //fr.AddTable("ChiTiet", data);

        //            //DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM,dNgayChungTu,ngayChungTu", "sLNS,sL,sK,sM,sTM,sMoTa,dNgayChungTu,ngayChungTu", "sLNS,sL,sK,sM,sTM,sTTM");

        //            //DataTable dtsTM = data.SelectDistinct("dtsTM", "sLNS,sL,sK,sM,sTM,dNgayChungTu,ngayChungTu").AddLnsMoTa(iNamLamViec);
        //            //DataTable dtsM = data.SelectDistinct("dtsM", "sLNS,sL,sK,sM,dNgayChungTu,ngayChungTu").AddLnsMoTa(iNamLamViec);
        //            //DataTable dtsL = dtsM.SelectDistinct("dtsL", "sLNS,sL,sK,dNgayChungTu,ngayChungTu").AddLnsMoTa(iNamLamViec);
        //            //DataTable dtsLNS = dtsL.SelectDistinct("dtsLNS", "sLNS,dNgayChungTu,ngayChungTu").AddLnsMoTa(iNamLamViec);
        //            //DataTable dtsDot = dtsLNS.SelectDistinct("dtDot", "dNgayChungTu,ngayChungTu");



        //            //DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM,dNgayChungTu,ngayChungTu", "sLNS,sL,sK,sM,sMoTa,dNgayChungTu,ngayChungTu", "sLNS,sL,sK,sM,sTM");
        //            //DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK,dNgayChungTu,ngayChungTu", "sLNS,sL,sK,sMoTa,dNgayChungTu,ngayChungTu", "sLNS,sL,sK,sM");
        //            //DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS,dNgayChungTu,ngayChungTu", "sLNS,sMoTa,dNgayChungTu,ngayChungTu", "sLNS,sL");
        //            //DataTable dtsDot = HamChung.SelectDistinct("dtDot", dtsLNS, "dNgayChungTu,ngayChungTu", "dNgayChungTu,ngayChungTu,sMoTa", "sLNS");




        //            //fr.AddTable("dtsTM", dtsTM);
        //            //fr.AddTable("dtsM", dtsM);
        //            //fr.AddTable("dtsL", dtsL);
        //            //fr.AddTable("dtsLNS", dtsLNS);
        //            //fr.AddTable("dtsDot", dtsDot);




        //            #region get MLNS 

        //            //for (int i = 0; i < dtsTM.Rows.Count; i++)
        //            //{
        //            //    var row = dtsTM.Rows[i];
        //            //    row["sMoTa"] = getMLNS_MoTa(ReportModels.LayNamLamViec(Username),
        //            //        row["sLNS"].ToString(),
        //            //        row["sL"].ToString(),
        //            //        row["sK"].ToString(),
        //            //        row["sM"].ToString(),
        //            //        row["sTM"].ToString());
        //            //}

        //            //for (int i = 0; i < dtsM.Rows.Count; i++)
        //            //{
        //            //    var row = dtsM.Rows[i];
        //            //    row["sMoTa"] = getMLNS_MoTa(ReportModels.LayNamLamViec(Username),
        //            //        row["sLNS"].ToString(),
        //            //        row["sL"].ToString(),
        //            //        row["sK"].ToString(),
        //            //        row["sM"].ToString());
        //            //}

        //            //for (int i = 0; i < dtsL.Rows.Count; i++)
        //            //{
        //            //    var row = dtsL.Rows[i];
        //            //    row["sMoTa"] = getMLNS_MoTa(ReportModels.LayNamLamViec(Username),
        //            //        row["sLNS"].ToString(),
        //            //        row["sL"].ToString(),
        //            //        row["sK"].ToString());
        //            //}

        //            //for (int i = 0; i < dtsLNS.Rows.Count; i++)
        //            //{
        //            //    var row = dtsLNS.Rows[i];
        //            //    row["sMoTa"] = getMLNS_MoTa(ReportModels.LayNamLamViec(Username),
        //            //        row["sLNS"].ToString());
        //            //}


        //            #endregion

        //            fr.AddTable("GhiChu", dataGhiChu);
        //            DataTable dtMoTaGhiChu = HamChung.SelectDistinct("dtMoTaGhiChu", dataGhiChu, "sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu", "");
        //            fr.AddTable("dtMoTaGhiChu", dtMoTaGhiChu);
        //            bool checkCoGhiChu = false;
        //            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
        //            {
        //                r = dtMoTaGhiChu.Rows[i];
        //                if (!String.IsNullOrEmpty(r["sGhiChu"].ToString()))
        //                {
        //                    checkCoGhiChu = true;
        //                }
        //            }

        //            if (checkCoGhiChu)
        //                fr.SetValue("sGhiChu", " * Ghi chú: ");
        //            else
        //                fr.SetValue("sGhiChu", "");

        //            //data.Dispose();
        //            //dtsTM.Dispose();
        //            //dtsM.Dispose();
        //            //dtsL.Dispose();
        //            //dtsLNS.Dispose();
        //            //dtsDot.Dispose();

        //        }

        //        private string getMLNS_MoTa(string iNamLamViec, string sLNS, string sL = "", string sK = "", string sM = "", string sTM = "", string sTTM = "")
        //        {
        //            var sql = @"
        //SELECT  TOP(1) sMoTa 
        //FROM    NS_MucLucNganSach
        //WHERE   iNamLamViec=@iNamLamViec 
        //        AND sXauNoiMa LIKE (@sXauNoiMa +'%')
        //ORDER BY sXauNoiMa
        //";

        //            var list = new List<string>()
        //            {
        //                sLNS,sL,sK,sM,sTM, sTTM
        //            };
        //            var sXauNoiMa = list.Where(x => !string.IsNullOrWhiteSpace(x)).Join("-");
        //            using (var conn = ConnectionFactory.Default.GetConnection())
        //            {
        //                using (var cmd = new SqlCommand(sql, conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
        //                    cmd.Parameters.AddWithValue("@sXauNoiMa", sXauNoiMa);

        //                    var dt = cmd.Execute();
        //                    return dt == null || dt.Rows.Count == 0 ? "" : dt.Rows[0]["sMoTa"].ToString();
        //                }
        //            }

        //        }

    }
}
