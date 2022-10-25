using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1040100_TungDonVi_TrinhKyController : AppController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        public string sViewPath = "~/Report_Views/DuToan/";
        private String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TungDonVi_TrinhKy.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TungDonVi_TrinhKy_Muc.xls";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TungDonVi_TrinhKy.aspx";
                ViewData["PageLoad"] = "0";
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        /// <summary>
        /// hàm lấy các giá trên form
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public ActionResult EditSubmit(String ParentID)
        {
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TungDonVi_TrinhKy.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>


        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult ViewPDF(string iID_MaDonVi, string iCapTongHop)
        {
            HamChung.Language();
            var xls = createReport(iID_MaDonVi, iCapTongHop);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaDonVi, string iCapTongHop)
        {
            HamChung.Language();
            var xls = createReport(iID_MaDonVi, iCapTongHop);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        private ExcelFile createReport(string iID_MaDonVi, string iCapTongHop)
        {
            var file = iCapTongHop == "Muc" ? sFilePath_Muc : sFilePath;
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            var fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaDonVi);

            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(Username));
            DataTable dtNganhChon = Connection.GetDataTable(cmd);

            String sTenDonVi = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            var loaiKhoan = HamChung.GetLoaiKhoanText("1040100");

            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("LoaiKhoan", loaiKhoan);

            fr.UseCommonValue()
                .UseChuKy()
                .UseChuKyForController(this.ControllerName())
                .Run(xls);

            var count = xls.TotalPageCount();
            
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            
            return xls;
        }

        private DataTable DT_rptDuToan_1040100_TungDonVi_TrinhKy(string maND, string iID_MaDonVi, int v = 0)
        {
            v = Request.GetQueryStringValue("v");

            return v == 2 ?
                DT_rptDuToan_1040100_TungDonVi_TrinhKy2(maND, iID_MaDonVi) :
                DT_rptDuToan_1040100_TungDonVi_TrinhKy1(maND, iID_MaDonVi);
        }

        private DataTable DT_rptDuToan_1040100_TungDonVi_TrinhKy2(string maND, string iID_MaDonVi)
        {
            var _duToanService = new DuToanReportService();
            var dt = _duToanService.TungDonVi(maND, iID_MaDonVi, "1040100", ",2");
            return dt;
        }



        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public DataTable DT_rptDuToan_1040100_TungDonVi_TrinhKy1(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            //  DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);

            int i = 0;
            String DSNganh = "";
            string SQL = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iTrangThai=1 AND iNamLamViec = @iNamLamViec AND iID_MaNganh IN (" + iID_MaDonVi + ")");
            SqlCommand cmdNganh = new SqlCommand(SQL);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);
            string iID_MaNganhMLNS = "";
            if (dtNganhChon.Rows.Count > 0)
            {
                iID_MaNganhMLNS = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
            }
            if (String.IsNullOrEmpty(iID_MaNganhMLNS))
                iID_MaNganhMLNS = "-1";
            DSNganh = " AND sNG IN (" + iID_MaNganhMLNS + ")";

            int DVT = 1000;

            var sql = @"
SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
        ,rTuChi=SUM(rTuChi/{3})
        ,rTonKho=SUM(rTonKho/{3})
        ,rHangNhap=SUM(rHangNhap/{3})
        ,rHangMua=SUM(rHangMua/{3})
        ,rPhanCap=SUM(rPhanCap/{3})
        ,rDuPhong=SUM(rDuPhong/{3})
FROM DT_ChungTuChiTiet
WHERE   (iTrangThai=1 OR (iTrangThai = 2 AND iID_MaTrangThaiDuyet = 2)) AND 
        sLNS='1040100' AND 
        --sNG <> '53'  AND 
        (MaLoai='' OR MaLoai='2') AND 
        iNamLamViec=@iNamLamViec {1} {2} {0}

        --ls: loc theo nganh, de kiem tra du lieu
        and (@sNG is null or sNG=@sNG)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING  SUM(rTuChi)<>0 OR 
        SUM(rHangNhap)<>0 OR 
        SUM(rTonKho)<>0  OR 
        SUM(rHangMua)<>0  OR 
        SUM(rPhanCap)<>0 OR 
        SUM(rDuPhong)<>0
";


            sql = String.Format(sql, DSNganh, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));

            // long them
            cmd.Parameters.AddWithValue("@sNG", Request.GetQueryString("sNG").ToParamString());

            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static DataTable DT_rptDuToan_1040100_TungDonVi_TrinhKy_GhiChu(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            //  DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);

            int i = 0;
            String DSNganh = "";
            string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iTrangThai=1 AND iNamLamViec = @iNamLamViec AND iID_MaNganh IN (" + iID_MaDonVi + ")");
            SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);
            string iID_MaNganhMLNS = "";
            if (dtNganhChon.Rows.Count > 0)
            {
                iID_MaNganhMLNS = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
            }
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

        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaDonVi)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_1040100_TungDonVi_TrinhKy(MaND, iID_MaDonVi);
            DataTable dataGhiChu = DT_rptDuToan_1040100_TungDonVi_TrinhKy_GhiChu(MaND, iID_MaDonVi);


            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK", "sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "sLNS,sMoTa", "sLNS,sL");



            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);

            fr.AddTable("GhiChu", dataGhiChu);
            DataTable dtMoTaGhiChu = HamChung.SelectDistinct("dtMoTaGhiChu", dataGhiChu, "sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu", "");
            fr.AddTable("dtMoTaGhiChu", dtMoTaGhiChu);
            bool checkCoGhiChu = false;
            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
            {
                r = dtMoTaGhiChu.Rows[i];
                if (!String.IsNullOrEmpty(r["sGhiChu"].ToString()))
                {
                    checkCoGhiChu = true;
                }
            }
            if (checkCoGhiChu)
                fr.SetValue("sGhiChu", " * Ghi chú: ");
            else
                fr.SetValue("sGhiChu", "");
            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();

        }
    }
}
