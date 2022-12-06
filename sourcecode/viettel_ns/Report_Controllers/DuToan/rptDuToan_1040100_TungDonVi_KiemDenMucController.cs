using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1040100_TungDonVi_DenMucController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TungDonVi_DenMuc_TheoB.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TungDonVi_DenMuc.xls";


        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TungDonVi_DenMuc.aspx";
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
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            ViewData["PageLoad"] = "1";
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TungDonVi_DenMuc.aspx";
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
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmd);

            String sTenDonVi = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaDonVi);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1040100_TungDonVi_DenMuc");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_1040100_TungDonVi_DenMuc(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            //  DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);

            String DSNganh = "";
            String SQL = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iTrangThai=1 AND iNamLamViec = @iNamLamViec");
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtDSNganh = Connection.GetDataTable(cmd);

            SQL = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iTrangThai=1 AND iNamLamViec = @iNamLamViec AND iID_MaNganh IN (" + iID_MaDonVi + ")");
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtNganhChon = Connection.GetDataTable(cmd);
            String iID_MaNganhMLNS = "";
            for (int c = 0; c < dtNganhChon.Rows.Count; c++)
            {
                iID_MaNganhMLNS += dtNganhChon.Rows[c]["iID_MaNganhMLNS"] + ",";
            }

            if (String.IsNullOrEmpty(iID_MaNganhMLNS))
                iID_MaNganhMLNS = "-1";
            else
            {
                iID_MaNganhMLNS = iID_MaNganhMLNS.Substring(0, iID_MaNganhMLNS.Length - 1);
            }
            DSNganh = " AND sNG IN (" + iID_MaNganhMLNS + ")";

            int DVT = 1000;
            SQL = String.Format(@"SELECT iID_MaPhongBan,MaLoai,iID_MaNganh='',sTenNganh='',iID_MaNganhCon='',sTenNganhCon='',sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
,rTuChi=SUM(rTuChi/{3})
,rTonKho=SUM(rTonKho/{3})
,rHangNhap=SUM(rHangNhap/{3})
,rHangMua=SUM(rHangMua/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS='1040100'  AND (MaLoai='' OR MaLoai='2') AND iNamLamViec=@iNamLamViec {1} {2} {0}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBan,MaLoai
HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rTonKho)<>0  OR SUM(rHangMua)<>0  OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
", DSNganh, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            DataRow R, RNganh;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                for (int j = 0; j < dtDSNganh.Rows.Count; j++)
                {
                    RNganh = dtDSNganh.Rows[j];
                    if (RNganh["iID_MaNganhMLNS"].ToString().Contains(R["sNG"].ToString()))
                    {
                        R["iID_MaNganh"] = RNganh["iID_MaNganh"];
                        R["sTenNganh"] = RNganh["sTenNganh"];
                        R["iID_MaNganhCon"] = R["sNG"];
                        //R["sTenNganhCon"] = R["sNG"];
                    }

                }
            }
            return dt;
        }
        public static String LayMoTa(String sLNS)
        {
            String sMoTa = "";

            String SQL = String.Format(@"SELECT sMoTa FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sLNS={0}", sLNS);
            sMoTa = Connection.GetValueString(SQL, "");
            return sMoTa;
        }
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaDonVi)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_1040100_TungDonVi_DenMuc(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "iID_MaNganh,iID_MaNganhCon,sLNS,sL,sK,sM,sTM", "iID_MaNganh,iID_MaNganhCon,sTenNganh,sTenNganhCon,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "iID_MaNganh,iID_MaNganhCon,sLNS,sL,sK,sM", "iID_MaNganh,iID_MaNganhCon,sTenNganh,sTenNganhCon,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "iID_MaNganh,iID_MaNganhCon,sLNS,sL,sK", "iID_MaNganh,iID_MaNganhCon,sTenNganh,sTenNganhCon,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "iID_MaNganh,iID_MaNganhCon,sLNS", "iID_MaNganh,iID_MaNganhCon,sTenNganh,sTenNganhCon,sLNS,sMoTa", "sLNS,sL");

            DataTable dtNganhCon = HamChung.SelectDistinct("dtNganhCon", dtsLNS, "iID_MaNganh,iID_MaNganhCon", "iID_MaNganh,iID_MaNganhCon,sTenNganh,sTenNganhCon");
            DataTable dtNganh = HamChung.SelectDistinct("dtNganh", dtNganhCon, "iID_MaNganh", "iID_MaNganh,sTenNganh");
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtNganhCon", dtNganhCon);
            fr.AddTable("dtNganh", dtNganh);


            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();

        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi, String iCapTongHop)
        {
            String sDuongDan = "";
            if (iCapTongHop.Equals("B"))
                sDuongDan = sFilePath;
            else
                sDuongDan = sFilePath_Muc;
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sDuongDan), MaND, iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1020000_TungDonVi.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi, String iCapTongHop)
        {
            HamChung.Language();
            String sDuongDan = "";
            if (iCapTongHop.Equals("B"))
                sDuongDan = sFilePath;
            else
                sDuongDan = sFilePath_Muc;
            ExcelFile xls = CreateReport(Server.MapPath(sDuongDan), MaND, iID_MaDonVi);
            using (FlexCelPdfExport pdf = new FlexCelPdfExport())
            {
                pdf.Workbook = xls;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.BeginExport(ms);
                    pdf.ExportAllVisibleSheets(false, "BaoCao");
                    pdf.EndExport();
                    ms.Position = 0;
                    return File(ms.ToArray(), "application/pdf");
                }
            }
        }
    }
}

