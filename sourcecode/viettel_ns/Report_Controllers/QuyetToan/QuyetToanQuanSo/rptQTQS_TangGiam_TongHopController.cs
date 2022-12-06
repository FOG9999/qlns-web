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
    public class rptQTQS_TangGiam_TongHopController : Controller
    {

        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuanSo/rptQTQS_TangGiam_TongHop.xls";
        private String MaND = "", iID_MaDonVi = "-1", iID_MaPhongBan = "-1", iThang_Quy = "-1", LoaiThang_Quy = "0", bCheckTongHop = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_TangGiam_TongHop.aspx";
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
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_TangGiam_TongHop.aspx";
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
                    sTenPhuLuc = "Mẫu số QS07-TH";
                }
                else if (String.IsNullOrEmpty(bCheckTongHop))
                {
                    sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
                    sTenPhuLuc = "Mẫu số QS07";
                }
                else
                {
                    sTenDonVi = "B " + iID_MaPhongBan;
                    sTenPhuLuc = "Mẫu số QS07-B";
                }
            }
            else
            {
                iNamLamViec = "Tháng " + iThang_Quy + " năm " + iNamLamViec;
                if (iID_MaPhongBan == "-1")
                {
                    sTenDonVi = "Toàn quân";
                    sTenPhuLuc = "Mẫu số QS07-TH";
                }
                else if (String.IsNullOrEmpty(bCheckTongHop))
                {
                    sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
                    sTenPhuLuc = "Mẫu số QS07";
                }
                else
                {
                    sTenDonVi = "B " + iID_MaPhongBan;
                    sTenPhuLuc = "Mẫu số QS07-B";
                }
            }
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);
            fr = ReportModels.LayThongTinChuKy(fr, "rptQTQS_TangGiam_TongHop", MaND);
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
        public DataTable DT_rptQTQS_TangGiam_TongHop()
        {
            String SQL, DK, DKDonVi, DKPhongBan;
            SqlCommand cmd = new SqlCommand();
            
            String sTruongTien = MucLucQuanSoModels.strDSTruongTien_So;
            String[] arrDSTruongTien = sTruongTien.Split(',');
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));

            
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            DK = " AND iTrangThai=1";
            //DK += " AND iThang_Quy<>0 AND iNamLamViec=@iNamLamViec";
            //cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            //if (LoaiThang_Quy == "1")
            //{
            //    if (iThang_Quy == "1")
            //        DK += " AND iThang_Quy IN (1,2,3)";
            //    else if (iThang_Quy == "2")
            //        DK += " AND iThang_Quy IN (4,5,6)";
            //    else if (iThang_Quy == "3")
            //        DK += " AND iThang_Quy IN (7,8,9)";
            //    else if (iThang_Quy == "4")
            //        DK += " AND iThang_Quy IN (10,11,12)";
            //    else
            //        DK += " AND iThang_Quy IN (-1)";
            //}
            //else
            //{
            //    DK += " AND iThang_Quy=@iThang_Quy";
            //    cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
            //}
            if (String.IsNullOrEmpty(bCheckTongHop))
            {
                DK += " AND iID_MaDonVi=@iID_MaDonViA";
                cmd.Parameters.AddWithValue("@iID_MaDonViA", iID_MaDonVi);
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
            String DKThangTruoc="", DKThangNay="";
            if (iThang_Quy == "1")
            {
                DKThangTruoc = " AND ((iNamLamViec<=@iNamLamViecTruoc AND iThang_Quy<=12) OR (iNamLamViec=@iNamLamViec AND iThang_Quy=0))";
                DKThangNay = " AND ((iNamLamViec=@iNamLamViec AND iThang_Quy<=1) OR (iNamLamViec<=@iNamLamViecTruoc AND iThang_Quy<=12)) ";
                cmd.Parameters.AddWithValue("@iNamLamViecTruoc", iNamLamViec - 1);
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            }
            else
            {
                DKThangTruoc += " AND ((iNamLamViec=@iNamLamViec AND iThang_Quy<@iThang_Quy) OR (iNamLamViec<=@iNamLamViecTruoc AND iThang_Quy<=12)) ";
                DKThangNay += " AND ((iNamLamViec=@iNamLamViec AND iThang_Quy<=@iThang_Quy) OR (iNamLamViec<=@iNamLamViecTruoc AND iThang_Quy<=12)) ";
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iNamLamViecTruoc", iNamLamViec - 1);
                
            }
            cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
            SQL = String.Format(@"
SELECT * FROM(
SELECT iID_madonvi
--thieu uy
,ThieuUy_NamTruoc=SUM(CASE WHEN sKyHieu=2 {0} THEN rThieuUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rThieuUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rThieuUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rThieuUy ELSE 0 END)
,ThieuUy_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThieuUy ELSE 0 END)
,ThieuUy_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThieuUy ELSE 0 END)
,ThieuUy_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rThieuUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rThieuUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rThieuUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rThieuUy ELSE 0 END)

--trung uy
,TrungUy_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rTrungUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rTrungUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rTrungUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rTrungUy ELSE 0 END)
,TrungUy_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTrungUy ELSE 0 END)
,TrungUy_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTrungUy ELSE 0 END)
,TrungUy_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rTrungUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rTrungUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rTrungUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rTrungUy ELSE 0 END)

--thuong uy
,ThuongUy_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rThuongUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rThuongUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rThuongUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rThuongUy ELSE 0 END)
,ThuongUy_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThuongUy ELSE 0 END)
,ThuongUy_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThuongUy ELSE 0 END)
,ThuongUy_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rThuongUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rThuongUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rThuongUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rThuongUy ELSE 0 END)

--Dai uy
,DaiUy_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rDaiUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rDaiUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rDaiUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rDaiUy ELSE 0 END)
,DaiUy_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rDaiUy ELSE 0 END)
,DaiUy_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rDaiUy ELSE 0 END)
,DaiUy_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rDaiUy ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rDaiUy ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rDaiUy ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rDaiUy ELSE 0 END)

--thieu ta
,ThieuTa_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rThieuTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rThieuTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rThieuTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rThieuTa ELSE 0 END)
,ThieuTa_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThieuTa ELSE 0 END)
,ThieuTa_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThieuTa ELSE 0 END)
,ThieuTa_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rThieuTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rThieuTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rThieuTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rThieuTa ELSE 0 END)

--trungta
,TrungTa_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rTrungTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rTrungTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rTrungTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rTrungTa ELSE 0 END)
,TrungTa_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTrungTa ELSE 0 END)
,TrungTa_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTrungTa ELSE 0 END)
,TrungTa_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rTrungTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rTrungTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rTrungTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rTrungTa ELSE 0 END)

--thuong ta
,ThuongTa_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rThuongTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rThuongTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rThuongTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rThuongTa ELSE 0 END)
,ThuongTa_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThuongTa ELSE 0 END)
,ThuongTa_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThuongTa ELSE 0 END)
,ThuongTa_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rThuongTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rThuongTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rThuongTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rThuongTa ELSE 0 END)

--Dai ta
,DaiTa_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rDaiTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rDaiTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rDaiTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rDaiTa ELSE 0 END)
,DaiTa_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rDaiTa ELSE 0 END)
,DaiTa_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rDaiTa ELSE 0 END)
,DaiTa_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rDaiTa ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rDaiTa ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rDaiTa ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rDaiTa ELSE 0 END)

--thieu tuong
,Tuong_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rTuong ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rTuong ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rTuong ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rTuong ELSE 0 END)
,Tuong_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTuong ELSE 0 END)
,Tuong_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTuong ELSE 0 END)
,Tuong_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rTuong ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rTuong ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rTuong ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rTuong ELSE 0 END)

--TSQ
,TSQ_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rTSQ ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rTSQ ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rTSQ ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rTSQ ELSE 0 END)
,TSQ_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTSQ ELSE 0 END)
,TSQ_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTSQ ELSE 0 END)
,TSQ_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rTSQ ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rTSQ ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rTSQ ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rTSQ ELSE 0 END)

--binh nhi
,BinhNhi_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rBinhNhi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rBinhNhi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rBinhNhi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rBinhNhi ELSE 0 END)
,BinhNhi_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rBinhNhi ELSE 0 END)
,BinhNhi_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rBinhNhi ELSE 0 END)
,BinhNhi_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rBinhNhi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rBinhNhi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rBinhNhi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rBinhNhi ELSE 0 END)

--binh nhat
,BinhNhat_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rBinhNhat ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rBinhNhat ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rBinhNhat ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rBinhNhat ELSE 0 END)
,BinhNhat_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rBinhNhat ELSE 0 END)
,BinhNhat_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rBinhNhat ELSE 0 END)
,BinhNhat_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rBinhNhat ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rBinhNhat ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rBinhNhat ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rBinhNhat ELSE 0 END)

--ha si
,HaSi_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rHaSi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rHaSi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rHaSi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rHaSi ELSE 0 END)
,HaSi_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rHaSi ELSE 0 END)
,HaSi_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rHaSi ELSE 0 END)
,HaSi_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rHaSi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rHaSi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rHaSi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rHaSi ELSE 0 END)

--trung si
,TrungSi_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rTrungSi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rTrungSi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rTrungSi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rTrungSi ELSE 0 END)
,TrungSi_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTrungSi ELSE 0 END)
,TrungSi_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rTrungSi ELSE 0 END)
,TrungSi_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rTrungSi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rTrungSi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rTrungSi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rTrungSi ELSE 0 END)

--thuong si
,ThuongSi_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rThuongSi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rThuongSi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rThuongSi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rThuongSi ELSE 0 END)
,ThuongSi_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThuongSi ELSE 0 END)
,ThuongSi_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rThuongSi ELSE 0 END)
,ThuongSi_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rThuongSi ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rThuongSi ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rThuongSi ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rThuongSi ELSE 0 END)

--QNCN
,QNCN_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rQNCN ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rQNCN ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rQNCN ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rQNCN ELSE 0 END)
,QNCN_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rQNCN ELSE 0 END)
,QNCN_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rQNCN ELSE 0 END)
,QNCN_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rQNCN ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rQNCN ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rQNCN ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rQNCN ELSE 0 END)

--CNVQP
,CNVQP_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rCNVQP ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rCNVQP ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rCNVQP ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rCNVQP ELSE 0 END)
,CNVQP_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rCNVQP ELSE 0 END)
,CNVQP_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rCNVQP ELSE 0 END)
,CNVQP_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rCNVQP ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rCNVQP ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rCNVQP ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rCNVQP ELSE 0 END)

--LDHD
,LDHD_NamTruoc=SUM(CASe WHEN sKyHieu=2 {0} THEN rLDHD ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {0} THEN rLDHD ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {0} THEN rLDHD ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {0} THEN rLDHD ELSE 0 END)
,LDHD_Tang=SUM(CASe WHEN sKyHieu=2 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rLDHD ELSE 0 END)
,LDHD_Giam=SUM(CASe WHEN sKyHieu=3 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy THEN rLDHD ELSE 0 END)
,LDHD_QuyetToan=SUM(CASe WHEN sKyHieu=2 {1} THEN rLDHD ELSE 0 END) -SUM(CASe WHEN sKyHieu=3 {1} THEN rLDHD ELSE 0 END)+SUM(CASe WHEN sKyHieu=500 {1} THEN rLDHD ELSE 0 END)-SUM(CASe WHEN sKyHieu=600 {1} THEN rLDHD ELSE 0 END)



FROM QTQS_ChungTuChiTiet
WHERE iTrangThai=1 {2} {3} {4}
GROUP BY iID_madonvi) as a
INNER JOIN
(SELECT iID_MaDonVi,sTen FROM NS_DonVi WHERE iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec) as b
ON a.iID_MaDonVi=b.iID_MaDonVi
ORDER BY a.iID_MaDonVi




", DKThangTruoc, DKThangNay, DKPhongBan,DKDonVi,DK);
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
            DataTable data = DT_rptQTQS_TangGiam_TongHop();
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();
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

