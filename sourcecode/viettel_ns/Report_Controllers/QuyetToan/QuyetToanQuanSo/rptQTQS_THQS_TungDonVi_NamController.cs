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

namespace VIETTEL.Report_Controllers.QuyetToan
{
    public class rptQTQS_THQS_TungDonVi_NamController : Controller
    {

        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuanSo/rptQTQS_THQS_TungDonVi_Nam.xls";
        private String MaND = "", iID_MaDonVi = "-1", iID_MaPhongBan = "-1", bCheckTongHop = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_THQS_TungDonVi_Nam.aspx";
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
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String bCheckTongHop = Request.Form[ParentID + "_bCheckTongHop"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["bCheckTongHop"] = bCheckTongHop;
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_THQS_TungDonVi_Nam.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);
            String sTenDonVi = "", sTenPhuLuc = "";
               if (iID_MaPhongBan == "-1")
                {
                    sTenDonVi = "Toàn quân";
                    sTenPhuLuc = "Mẫu số QS04-TH";
                }
                else if (String.IsNullOrEmpty(bCheckTongHop))
                {
                    sTenDonVi = "Đơn vị: " + DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
                    sTenPhuLuc = "Mẫu số QS04";
                }
                else
                {
                    sTenDonVi = "B " + iID_MaPhongBan;
                    sTenPhuLuc = "Mẫu số QS04-B";
                }
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);
            fr = ReportModels.LayThongTinChuKy(fr, "rptQTQS_THQS_TungDonVi_Nam", MaND);
            fr.SetValue("Nam", "Năm "+iNamLamViec);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("sTenPhuLuc", sTenPhuLuc);
            fr.SetValue("Cap2", ReportModels.CauHinhTenDonViSuDung(2, MaND));
            fr.SetValue("sTenDonVi", sTenDonVi);            
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
        public DataTable DT_rptQTQS_THQS_TungDonVi_Nam()
        {
            DataTable vR;
            String SQL, DK, DKDonVi, DKPhongBan;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String sTruongTien = MucLucQuanSoModels.strDSTruongTien_So;
            String[] arrDSTruongTien = sTruongTien.Split(',');
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
            DataTable dtBienChe = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoBienChe(iNamLamViec, 1, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);

            cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            DK = " AND iTrangThai=1";
            DK += " AND iThang_Quy<>0 AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            if (String.IsNullOrEmpty(bCheckTongHop))
            {
                DK += " AND iID_MaDonVi=@iID_MaDonVi";
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            }
            if (iID_MaPhongBan != "-2" && iID_MaPhongBan != "-1")
            {
                DK += " AND iID_MaPhongBan=@iiD_MaPhongBan1";
                cmd.Parameters.AddWithValue("@iID_MaPhongBan1", iID_MaPhongBan);
            }
            //if (iID_MaPhongBan == "-2")
            //{
            //    DKDV = " AND iID_MaDonVi =@iID_MaDonVi";
            //    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            //}
            SQL = String.Format(@"SELECT 
sKyHieu,sMoTa,
SUM(rThieuUy) as rThieuUy,
SUM(rTrungUy) as rTrungUy,
SUM(rThuongUy) as rThuongUy,
SUM(rDaiUy) as rDaiUy,
SUM(rThieuTa)as rThieuTa,
SUM(rTrungTa)as rTrungTa,
SUM(rThuongTa)as rThuongTa,
SUM(rDaiTa)as rDaiTa,
SUM(rTuong)as rTuong,
SUM(rTSQ)as rTSQ,
SUM(rBinhNhi)as rBinhNhi,
SUM(rBinhNhat)as rBinhNhat,
SUM(rHaSi)as rHaSi,
SUM(rTrungSi)as rTrungSi,
SUM(rThuongSi)as rThuongSi,
SUM(rQNCN)as rQNCN,
SUM(rCNVQP)as rCNVQP,
SUM(rLDHD)as rLDHD
FROM QTQS_ChungTuChiTiet
WHERE iTrangThai=1
 AND iThang_Quy<>0 
 AND sKyHieu NOT IN (000,400,500,600,700)
 {0} {1} {2}
 GROUP BY sKyHieu,sMoTa
 ORDER BY sKyHieu
", DK, DKDonVi, DKPhongBan);
            cmd.CommandText = SQL;
            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);

            DataRow dr;
            dr = dtChungTuChiTiet.NewRow();
            for (int i = 0; i < arrDSTruongTien.Length; i++)
            {
                if (dtBienChe.Rows.Count > 0)
                    dr[i + 2] = dtBienChe.Rows[0][i + 1];
            }
            dr["sKyHieu"] = "000";
            dr["sMoTa"] = "Quân số biên chế";
            dtChungTuChiTiet.Rows.InsertAt(dr, 0);
            //theo thang
            
                dr = dtChungTuChiTiet.NewRow();
                DataTable dtThangtruoc = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, 1, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtThangtruoc.Rows.Count > 0)
                        dr[i + 2] = dtThangtruoc.Rows[0][i];
                }
                dr["sKyHieu"] = "100";
                dr["sMoTa"] = "Quân số 1/1";
                dtChungTuChiTiet.Rows.InsertAt(dr, 1);
                DataTable dtThangNay = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, 13, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                dr = dtChungTuChiTiet.NewRow();
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtThangNay.Rows.Count > 0)
                        dr[i + 2] = dtThangNay.Rows[0][i];
                }
                dr["sKyHieu"] = "700";
                dr["sMoTa"] = "Quân số bình quân năm";
                dtChungTuChiTiet.Rows.Add(dr);
                for (int i = 0; i < dtChungTuChiTiet.Rows.Count; i++)
                {
                    if ("2".Equals(dtChungTuChiTiet.Rows[i]["sKyHieu"]))
                    {
                        dtChungTuChiTiet.Rows[i]["sMoTa"] = "Quân số tăng trong năm";
                    }
                    if ("3".Equals(dtChungTuChiTiet.Rows[i]["sKyHieu"]))
                    {
                        dtChungTuChiTiet.Rows[i]["sMoTa"] = "Quân số giảm trong năm";
                    }

                }
            
           
            return dtChungTuChiTiet;
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
        private void LoadData(FlexCelReport fr)
        {
            DataRow r;
            DataTable data = DT_rptQTQS_THQS_TungDonVi_Nam();
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi, String iID_MaPhongBan)
        {
            this.MaND = MaND;
            this.iID_MaDonVi = iID_MaDonVi;
            this.iID_MaPhongBan = iID_MaPhongBan;
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath));

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "QuyetToan_1010000_TungDonVi.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi, String iID_MaPhongBan, String bCheckTongHop)
        {
            HamChung.Language();
            this.MaND = MaND;
            this.iID_MaDonVi = iID_MaDonVi;
            this.iID_MaPhongBan = iID_MaPhongBan;
            this.bCheckTongHop = bCheckTongHop;
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath));
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
        public JsonResult LayDanhSachDonVi(String ParentID, String iID_MaPhongBan, String iID_MaDonVi)
        {
            String MaND = User.Identity.Name;
            String ViewNam = "~/Views/DungChung/DonVi/DonVi_DanhSach.ascx";

            DataTable dt = DonViModels.DanhSach_DonVi_QuyetToanQS_PhongBan(iID_MaPhongBan, User.Identity.Name);
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, iID_MaDonVi, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);

            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }
    }
}

