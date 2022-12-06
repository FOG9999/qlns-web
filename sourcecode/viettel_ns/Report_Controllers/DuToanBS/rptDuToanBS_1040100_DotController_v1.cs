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
using VIETTEL.Models.DuToanBS;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToanBS_1040100_Dot_v1Controller : Controller
    {

        public string sViewPath = "~/Report_Views/";
        private String sFilePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Dot.xls";
        private String sFilePath_TongHop = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Dot_TongHop.xls";
        String iCapTongHop = "";
        String dNgayChungTu = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToanBS/rptDuToanBS_1040100_Dot.aspx";
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
            //Lay Gia Tri Tu Form
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            String idDot = Request.Form[ParentID + "_iID_MaDot"];
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];

            //Gán giá trị vào ViewData
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["iID_MaDot"] = idDot;
            ViewData["path"] = "~/Report_Views/DuToanBS/rptDuToanBS_1040100_Dot.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND, String iID_MaDonVi, String iID_MaDot)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

            String sTenDonVi = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh", "iID_MaNganh", iID_MaDonVi, "sTenNganh"));
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaDonVi, iID_MaDot);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToanBS_1040100_Dot");
            fr.SetValue("DNgay", iID_MaDot);
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
        public DataTable DT_rptDuToanBS_1040100_Dot(String MaND, String iID_MaDonVi, String iID_MaDot)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            bool checkTL = LuongCongViecModel.KiemTra_TroLyPhongBan(MaND);

            if (checkTL)
            {
                DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
                //  DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            }

            String dsMaChungTu = DuToanBS_ReportModels.DKMaChungTuCT_Period("", iID_MaDot, MaND);

            int i = 0;
            String DSNganh = "";
            String iID_MaNganhMLNS = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh", "iID_MaNganh", iID_MaDonVi, "iID_MaNganhMLNS"));
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
,CONVERT(nvarchar, dNgayChungTu, 103) as ngayChungTu
    FROM DTBS_ChungTuChiTiet a INNER JOIN (SELECT iID_MaChungTu,dNgayChungTu FROM DTBS_ChungTu) b ON a.iID_MaChungTu = b.iID_MaChungTu
    WHERE iTrangThai=1 AND sLNS='1040100' AND (MaLoai='' OR MaLoai='2') AND iNamLamViec=@iNamLamViec {1} {2} {0} AND a.iID_MaChungTu IN {4}
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,dNgayChungTu
HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0

UNION ALL 

SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
,rTuChi=SUM(rTuChi/{3})
,rTonKho=SUM(rTonKho/{3})
,rHangNhap=SUM(rHangNhap/{3})
,rHangMua=SUM(rHangMua/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
,CONVERT(nvarchar, dNgayChungTu, 103) as ngayChungTu
    FROM DT_ChungTuChiTiet a INNER JOIN (SELECT iID_MaChungTu,dNgayChungTu FROM DT_ChungTu WHERE CONVERT(nvarchar,dNgayChungTu,103)<=@iID_MaDot) b ON a.iID_MaChungTu = b.iID_MaChungTu
    WHERE iTrangThai=1 AND sLNS='1040100' AND (MaLoai='' OR MaLoai='2') AND iNamLamViec=@iNamLamViec {1} {2} {0} 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,dNgayChungTu
HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
", DSNganh, DKPhongBan, DKDonVi, DVT, dsMaChungTu);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDot", iID_MaDot);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }


        public static DataTable DT_rptDuToanBS_1040100_Dot_GhiChu(String MaND, String iID_MaDonVi, String iID_MaDot)
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
            String iID_MaNganhMLNS = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh", "iID_MaNganh", iID_MaDonVi, "iID_MaNganhMLNS"));
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
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaDonVi, String iID_MaDot)
        {
            DataRow r;
            DataTable data = DT_rptDuToanBS_1040100_Dot(MaND, iID_MaDonVi, iID_MaDot);
            DataTable dataGhiChu = DT_rptDuToanBS_1040100_Dot_GhiChu(MaND, iID_MaDonVi, iID_MaDot);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM,ngayChungTu", "sLNS,sL,sK,sM,sTM,sMoTa,ngayChungTu", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM,ngayChungTu", "sLNS,sL,sK,sM,sMoTa,ngayChungTu", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK,ngayChungTu", "sLNS,sL,sK,sMoTa,ngayChungTu", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS,ngayChungTu", "sLNS,sMoTa,ngayChungTu", "sLNS,sL");
            DataTable dtsDot = HamChung.SelectDistinct("dtDot", dtsLNS, "ngayChungTu", "ngayChungTu,sMoTa", "sLNS");

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsDot", dtsDot);

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
            dtsDot.Dispose();

        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi, String iCapTongHop, String dNgayChungTu)
        {
            this.iCapTongHop = iCapTongHop;
            this.dNgayChungTu = dNgayChungTu;
            if (iCapTongHop == "TongHop")
            {
                sFilePath = sFilePath_TongHop;
            }
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi, dNgayChungTu);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1020000_Dot.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi, String iCapTongHop, String dNgayChungTu)
        {
            this.iCapTongHop = iCapTongHop;
            this.dNgayChungTu = dNgayChungTu;
            if (iCapTongHop == "TongHop")
            {
                sFilePath = sFilePath_TongHop;
            }
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi, dNgayChungTu);
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

        /// <summary>
        /// Lấy danh sách Ngành dựa vào đợt, phòng ban, 
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="iID_MaDot">Mã Đợt</param>
        /// <param name="iID_MaPhongBan">Mã Phòng Ban</param>
        /// <param name="iID_MaDonVi">Mã Đơn Vị</param>
        /// <returns></returns>
        public JsonResult LayDanhSachNganh(String ParentID, String iID_MaDot, String iID_MaPhongBan, String iID_MaDonVi)
        {
            String MaND = User.Identity.Name;
            String ViewNam = "~/Views/DungChung/DonVi/Nganh_DanhSach.ascx";

            DataTable dt = DuToanBS_ReportModels.dtNganh(iID_MaDot, MaND, "1040100");

            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, iID_MaDonVi, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);

            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }
    }
}

