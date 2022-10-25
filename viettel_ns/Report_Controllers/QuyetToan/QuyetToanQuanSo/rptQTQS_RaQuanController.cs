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
    public class rptQTQS_RaQuanController : Controller
    {

        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuanSo/rptQTQS_RaQuan.xls";
        private String MaND = "", iID_MaDonVi = "-1", iID_MaPhongBan = "-1", iThang_Quy = "-1", LoaiThang_Quy = "0", bCheckTongHop = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_RaQuan.aspx";
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
            String iThang = Request.Form[ParentID + "_iThang"];
            String iQuy = Request.Form[ParentID + "_iQuy"];
            String LoaiThang_Quy = Request.Form[ParentID + "_LoaiThang_Quy"];
            String bCheckTongHop = Request.Form[ParentID + "_bCheckTongHop"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["iThang"] = iThang;
            ViewData["iQuy"] = iQuy;
            ViewData["LoaiThang_Quy"] = LoaiThang_Quy;
            ViewData["bCheckTongHop"] = bCheckTongHop;
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_RaQuan.aspx";
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
            if (LoaiThang_Quy == "1")
            {
                iNamLamViec = "Quý " + iThang_Quy + " năm " + iNamLamViec;

                if (iID_MaPhongBan == "-1")
                {
                    sTenDonVi = "Toàn quân";
                    sTenPhuLuc = "Mẫu số QS08-TH";
                }
                else if (String.IsNullOrEmpty(bCheckTongHop))
                {
                    sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
                    sTenPhuLuc = "Mẫu số QS08";
                }
                else
                {
                    sTenDonVi = "B " + iID_MaPhongBan;
                    sTenPhuLuc = "Mẫu số QS08-B";
                }
            }
            else
            {
                iNamLamViec = "Tháng " + iThang_Quy + " năm " + iNamLamViec;
                if (iID_MaPhongBan == "-1")
                {
                    sTenDonVi = "Toàn quân";
                    sTenPhuLuc = "Mẫu số QS08-TH";
                }
                else if (String.IsNullOrEmpty(bCheckTongHop))
                {
                    sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
                    sTenPhuLuc = "Mẫu số QS08";
                }
                else
                {
                    sTenDonVi = "B " + iID_MaPhongBan;
                    sTenPhuLuc = "Mẫu số QS08-B";
                }
            }
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);
            fr = ReportModels.LayThongTinChuKy(fr, "rptQTQS_RaQuan", MaND);
            fr.SetValue("Nam", iNamLamViec);
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
        public DataTable DT_rptQTQS_RaQuan()
        {
            DataTable vR;
            String SQL, DK, DKDonVi, DKPhongBan;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String sTruongTien = MucLucQuanSoModels.strDSTruongTien_So;
            String[] arrDSTruongTien = sTruongTien.Split(',');
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));

            cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            DK = "  iTrangThai=1";
            DK += " AND iThang_Quy<>0 AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            if (LoaiThang_Quy == "1")
            {
                if (iThang_Quy == "1")
                    DK += " AND iThang_Quy IN (1,2,3)";
                else if (iThang_Quy == "2")
                    DK += " AND iThang_Quy IN (4,5,6)";
                else if (iThang_Quy == "3")
                    DK += " AND iThang_Quy IN (7,8,9)";
                else if (iThang_Quy == "4")
                    DK += " AND iThang_Quy IN (10,11,12)";
                else
                    DK += " AND iThang_Quy IN (-1)";
            }
            else
            {
                DK += " AND iThang_Quy=@iThang_Quy";
                cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
            }
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
            SQL = String.Format(@"
SELECT * FROM 
(
SELECT iID_MaDonVi
,Huu=SUM(CASE WHEN sKyHieu='310' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
,PhucVien=SUM(CASE WHEN sKyHieu='331' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
,XuatNgu=SUM(CASE WHEN sKyHieu='320' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
,ThoiViec=SUM(CASE WHEN sKyHieu='330' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
 FROM QTQS_ChungTuChiTiet
 WHERE   {0} {1} {2} 
 GROUP BY
 iID_MaDonVi
 HAVING SUM(CASE WHEN sKyHieu='310' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END) <>0
OR SUM(CASE WHEN sKyHieu='331' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)<>0
OR SUM(CASE WHEN sKyHieu='320' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)<>0
OR SUM(CASE WHEN sKyHieu='330' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)<>0
) as a
INNER JOIN
(SELECT sTen,iID_MaDonVi as MaDonVi FROM NS_DonVi WHERE iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec) as b
ON a.iID_MaDonVi=b.MaDonVi
 ORDER BY iID_MaDonVi
 
 
", DK, DKDonVi, DKPhongBan);
            cmd.CommandText = SQL;
            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
            return dtChungTuChiTiet;
        }


        public DataTable DenThangTruocRaQuan(int iThang)
        {
            DataTable vR;
            String SQL, DK, DKDonVi, DKPhongBan;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String sTruongTien = MucLucQuanSoModels.strDSTruongTien_So;
            String[] arrDSTruongTien = sTruongTien.Split(',');
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));

            cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            DK = "  iTrangThai=1";
            if (iThang == 1)
            {
                DK += " AND (iNamLamViec=@iNamLamViec AND iThang_Quy=0)";
               // cmd.Parameters.AddWithValue("@iNamLamViecTruoc", iNamLamViec - 1);
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            }
            else
            {
                DK += " AND ((iNamLamViec=@iNamLamViec AND iThang_Quy<@iThang_Quy) OR (iNamLamViec<=@iNamLamViecTruoc AND iThang_Quy<=12)) ";
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iNamLamViecTruoc", iNamLamViec - 1);
                cmd.Parameters.AddWithValue("@iThang_Quy", iThang);
            }
           
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
            SQL = String.Format(@"

SELECT 
Huu=SUM(CASE WHEN sKyHieu='310' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
,PhucVien=SUM(CASE WHEN sKyHieu='331' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
,XuatNgu=SUM(CASE WHEN sKyHieu='320' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
,ThoiViec=SUM(CASE WHEN sKyHieu='330' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)
 FROM QTQS_ChungTuChiTiet
 WHERE   {0} {1} {2} 
 
 HAVING SUM(CASE WHEN sKyHieu='310' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END) <>0
OR SUM(CASE WHEN sKyHieu='331' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)<>0
OR SUM(CASE WHEN sKyHieu='320' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)<>0
OR SUM(CASE WHEN sKyHieu='330' THEN (rThieuUy+rTrungUy+rThuongUy+rDaiUy+rThieuTa+rTrungTa+rThuongTa+rDaiTa+rTuong+rTSQ+rBinhNhi+rBinhNhat+rHaSi+rTrungSi+rThuongSi+rQNCN+rCNVQP+rLDHD) ELSE 0 END)<>0


", DK, DKDonVi, DKPhongBan);
            cmd.CommandText = SQL;
            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
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
            DataTable data = DT_rptQTQS_RaQuan();
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();

            DataTable dtThangTruoc = DenThangTruocRaQuan(Convert.ToInt32(iThang_Quy));
            if (dtThangTruoc.Rows.Count == 0)
            {
                dtThangTruoc.Rows.Add(dtThangTruoc.NewRow());
            }
            fr.AddTable("dtThangTruoc", dtThangTruoc);
            dtThangTruoc.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi, String iID_MaPhongBan, String iThang_Quy, String LoaiThang_Quy)
        {
            this.MaND = MaND;
            this.iID_MaDonVi = iID_MaDonVi;
            this.iID_MaPhongBan = iID_MaPhongBan;
            this.iThang_Quy = iThang_Quy;
            this.LoaiThang_Quy = LoaiThang_Quy;
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
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi, String iID_MaPhongBan, String iThang_Quy, String LoaiThang_Quy, String bCheckTongHop)
        {
            HamChung.Language();
            this.MaND = MaND;
            this.iID_MaDonVi = iID_MaDonVi;
            this.iID_MaPhongBan = iID_MaPhongBan;
            this.iThang_Quy = iThang_Quy;
            this.LoaiThang_Quy = LoaiThang_Quy;
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

