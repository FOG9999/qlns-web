using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_BaoDam_TungDonVi_BiaController : FlexcelReportController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private String sFilePath = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_BaoDam_TungDonVi_Bia.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_BaoDam_TungDonVi_Bia_DenMuc.xls";
        String iCapTongHop = "";
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_BaoDam_TungDonVi_Bia.aspx";
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
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_BaoDam_TungDonVi_Bia.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND, String iID_MaDonVi)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);          

            string iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmd);

            string sTenDonVi = "";
            if (dtNganhChon.Rows.Count > 0) {
                sTenDonVi = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            }

            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaDonVi);
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(MaND);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);

            var loaiKhoan = HamChung.GetLoaiKhoanText("1040100");
            fr.SetValue("LoaiKhoan", loaiKhoan);

            fr.Run(Result);

            var count = Result.TotalPageCount();

            if (count > 1)
            {
                Result.AddPageFirstPage();
            }

            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public DataTable DT_rptDuToan_1040100_TungDonVi(String MaND, String iID_MaDonVi)
        {
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
            if (iID_MaDonVi != "51")
            {
                iID_MaDonVi = "-1";
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_dutoan_report_baodam_tungdonvi_bia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nam", ReportModels.LayNamLamViec(MaND).ToParamString());
                    cmd.Parameters.AddWithValue("@ng", iID_MaNganhMLNS.ToParamString());
                    cmd.Parameters.AddWithValue("@ngcha", iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@id_donvi", "-1".ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", 1000);
                    var dt = cmd.GetDataset().Tables[0];

                    return dt;
                }
            }           
        }


        public static DataTable DT_rptDuToan_1040100_TungDonVi_GhiChu(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
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
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND sLNS='1040100'  AND (MaLoai='' OR MaLoai='2') AND iNamLamViec=@iNamLamViec AND sGhiChu IS NOT NULL AND LTRIM(RTRIM(sGhiChu)) <> '' {1} {2} {0}
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
            DataTable data = DT_rptDuToan_1040100_TungDonVi(MaND, iID_MaDonVi);
            DataTable dataGhiChu = DT_rptDuToan_1040100_TungDonVi_GhiChu(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "iID_MaNganh,sM,sTM", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sMoTa", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "iID_MaNganh,sM", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sMoTa", "iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM");
            DataTable dtsNganh = HamChung.SelectDistinct("dtsNganh", dtsM, "iID_MaNganh", "iID_MaNganh,sTenNganh", "iID_MaNganh,sTenNganh");

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsNganh", dtsNganh);

            foreach (DataRow dtr in dtsTM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM,sTM".Split(','), MaND);
            }
            foreach (DataRow dtr in dtsM.Rows)
            {
                dtr["sMoTa"] = ReportModels.LayMoTa(dtr, "sLNS,sL,sK,sM".Split(','), MaND);
            }

            fr.AddTable("GhiChu", dataGhiChu);
            DataTable dtMoTaGhiChu = HamChung.SelectDistinct("dtMoTaGhiChu", dataGhiChu, "sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu", "");
            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
            {
                DataRow dr = dtMoTaGhiChu.Rows[i];
                var items = new List<string>();
                dr["sMoTa"] += ": ";
                for (int j = 0; j < dataGhiChu.Rows.Count; j++)
                {
                    DataRow dr1 = dataGhiChu.Rows[j];

                    if (Convert.ToString(dr["sM"]) == Convert.ToString(dr1["sM"]) && Convert.ToString(dr["sTM"]) == Convert.ToString(dr1["sTM"]) && Convert.ToString(dr["sTTM"]) == Convert.ToString(dr1["sTTM"]) && Convert.ToString(dr["sNG"]) == Convert.ToString(dr1["sNG"]))
                    {
                        items.Add(dr1["sGhiChu"].ToString());
                    }
                }
                dr["sMoTa"] += items.Join(", ");
            }
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
            dtsNganh.Dispose();
            dtMoTaGhiChu.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi, String iCapTongHop)
        {
            this.iCapTongHop = iCapTongHop;
            if (iCapTongHop == "Muc")
            {
                sFilePath = sFilePath_Muc;
            }
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_BaoDam_TungDonVi.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi, String iCapTongHop, string ext)
        {
            this.iCapTongHop = iCapTongHop;
            if (iCapTongHop == "Muc")
            {
                sFilePath = sFilePath_Muc;
            }
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi);
            
            return Print(xls, ext);
        }
    }
}
