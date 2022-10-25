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
    public class rptQTQS_THQS_TungDonVi_1Controller : Controller
    {

        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuanSo/rptQTQS_THQS_TungDonVi_1.xls";
        private String MaND = "", iID_MaDonVi = "-1", iID_MaPhongBan = "-1", iThang_Quy = "-1", LoaiThang_Quy = "0", bCheckTongHop = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_THQS_TungDonVi_1.aspx";
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
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuanSo/rptQTQS_THQS_TungDonVi_1.aspx";
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
                    sTenPhuLuc = "Mẫu số B02/Q-TH";
                }
                else if (String.IsNullOrEmpty(bCheckTongHop))
                {
                    sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
                    sTenPhuLuc = "Mẫu số B02/Q";
                }
                else
                {
                    sTenDonVi = "B " + iID_MaPhongBan;
                    sTenPhuLuc = "Mẫu số B02/Q-B";
                }
            }
            else
            {
                iNamLamViec = "Tháng " + iThang_Quy + " năm " + iNamLamViec;
                if (iID_MaPhongBan == "-1")
                {
                    sTenDonVi = "Toàn quân";
                    sTenPhuLuc = "Mẫu số FB02/T-TH";
                }
                else if (String.IsNullOrEmpty(bCheckTongHop))
                {
                    sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
                    sTenPhuLuc = "Mẫu số FB02/T";
                }
                else
                {
                    sTenDonVi = "B " + iID_MaPhongBan;
                    sTenPhuLuc = "Mẫu số FB02/T-B";
                }
            }
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);
            fr = ReportModels.LayThongTinChuKy(fr, "rptQTQS_THQS_TungDonVi_1", MaND);
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
        public DataTable DT_rptQTQS_THQS_TungDonVi_1()
        {
            DataTable vR;
            String SQL, DK, DKDonVi, DKPhongBan;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            String sTruongTien = MucLucQuanSoModels.strDSTruongTien_So;
            String[] arrDSTruongTien = sTruongTien.Split(',');
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
            DataTable dtBienChe = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoBienChe(iNamLamViec, Convert.ToInt32(iThang_Quy), iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);

            cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            DK = " AND iTrangThai=1";
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
HAVING 
SUM(rThieuUy) <> 0 OR 
SUM(rTrungUy) <> 0 OR 
SUM(rThuongUy) <> 0 OR 
SUM(rDaiUy) <> 0 OR 
SUM(rThieuTa) <> 0 OR 
SUM(rTrungTa) <> 0 OR 
SUM(rThuongTa)<> 0 OR 
SUM(rDaiTa)<> 0 OR 
SUM(rTuong)<> 0 OR 
SUM(rTSQ)<> 0 OR 
SUM(rBinhNhi)<> 0 OR 
SUM(rBinhNhat)<> 0 OR 
SUM(rHaSi)<> 0 OR 
SUM(rTrungSi)<> 0 OR 
SUM(rThuongSi)<> 0 OR 
SUM(rQNCN)<> 0 OR 
SUM(rCNVQP)<> 0 OR 
SUM(rLDHD)<>0
 ORDER BY sKyHieu
", DK, DKDonVi, DKPhongBan);
            cmd.CommandText = SQL;
            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
            //lay muc luc quan so
            SQL = String.Format(@"SELECT sKyHieu,sMoTa FROM NS_MucLucQuanSo
WHERE iTrangThai=1
 AND sKyHieu NOT IN ('001','002','100','000','400','500','600','700','800','100')
ORDER BY sKyHieu");
            DataTable dtDanhMuc = Connection.GetDataTable(SQL);
            //add cot vao dtDanhMuc
            String [] arrDanhMuc=@"rThieuUy,rTrungUy,rThuongUy,rDaiUy,rThieuTa,rTrungTa,rThuongTa,rDaiTa,rTuong,rTSQ,rBinhNhi,rBinhNhat,rHaSi,rTrungSi,rThuongSi,rQNCN,rCNVQP,rLDHD".Split(',');
            for (int i = 0; i < arrDanhMuc.Length; i++)
			{
			 dtDanhMuc.Columns.Add(arrDanhMuc[i],typeof(Int32));
			}
            //ghep dtchitiet vao dt chung tu

            for (int i = 0; i < dtDanhMuc.Rows.Count; i++)
            {
                DataRow r = dtDanhMuc.Rows[i];
                for (int j = 0; j < dtChungTuChiTiet.Rows.Count; j++)
                {
                    if (Convert.ToString(dtDanhMuc.Rows[i]["sKyHieu"]).Equals(Convert.ToString(dtChungTuChiTiet.Rows[j]["sKyHieu"])))
                    {
                        for (int c = 0; c < arrDanhMuc.Length; c++)
                        {
                            dtDanhMuc.Rows[i][arrDanhMuc[c]] = dtChungTuChiTiet.Rows[j][arrDanhMuc[c]];
                        }
                        break;
                    }
                }
            }
            dtChungTuChiTiet = dtDanhMuc;
            
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
            if (LoaiThang_Quy == "0")
            {
                dr = dtChungTuChiTiet.NewRow();
                DataTable dtThangtruoc = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, Convert.ToInt32(iThang_Quy), iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtThangtruoc.Rows.Count > 0)
                        dr[i + 2] = dtThangtruoc.Rows[0][i];
                }
                dr["sKyHieu"] = "100";
                dr["sMoTa"] = "Quân số Q.toán tháng trước";
                dtChungTuChiTiet.Rows.InsertAt(dr, 1);
                DataTable dtThangNay = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, Convert.ToInt32(iThang_Quy) + 1, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                dr = dtChungTuChiTiet.NewRow();
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtThangNay.Rows.Count > 0)
                        dr[i + 2] = dtThangNay.Rows[0][i];
                }
                dr["sKyHieu"] = "700";
                dr["sMoTa"] = "Quân số Q.toán tháng này";
                dtChungTuChiTiet.Rows.Add(dr);
            }
            //thheo quy
            else
            {
                dr = dtChungTuChiTiet.NewRow();
                int iThang1, iThang2, iThang3, iThang1_HienTai, iThang2_HienTai, iThang3_HienTai;
                DataTable dtQuyTruoc, dtThangTruoc1, dtThangTruoc2, dtThangTruoc3, dtThangNay1, dtThangNay2, dtThangNay3;
                switch (iThang_Quy)
                {
                    case "1":
                        iThang1 = 1;
                        iThang2 = 12;
                        iThang3 = 11;
                        iThang1_HienTai = 2;
                        iThang2_HienTai = 3;
                        iThang3_HienTai = 4;
                        break;
                    case "2":
                        iThang1 = 2;
                        iThang2 = 3;
                        iThang3 = 4;
                        iThang1_HienTai = 5;
                        iThang2_HienTai = 6;
                        iThang3_HienTai = 7;
                        break;
                    case "3":
                        iThang1 = 5;
                        iThang2 = 6;
                        iThang3 = 7;
                        iThang1_HienTai = 8;
                        iThang2_HienTai = 9;
                        iThang3_HienTai = 10;
                        break;
                    case "4":
                        iThang1 = 8;
                        iThang2 = 9;
                        iThang3 = 10;
                        iThang1_HienTai = 11;
                        iThang2_HienTai = 12;
                        iThang3_HienTai = 13;
                        break;
                    default:

                        iThang1 = 0;
                        iThang2 = 0;
                        iThang3 = 0;
                        iThang1_HienTai = 0;
                        iThang2_HienTai = 0;
                        iThang3_HienTai = 0;
                        break;
                }
                if (iThang_Quy == "1")
                {
                    dtThangTruoc1 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, iThang1, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                    dtThangTruoc2 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec - 1, iThang2, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                    dtThangTruoc3 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec - 1, iThang3, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                }
                else
                {
                    dtThangTruoc1 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, iThang1, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                    dtThangTruoc2 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, iThang2, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                    dtThangTruoc3 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, iThang3, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                }

                dtQuyTruoc = dtThangTruoc1.Copy();
                for (int i = 0; i < dtQuyTruoc.Columns.Count; i++)
                {
                    dtQuyTruoc.Rows[0][i] = Convert.ToDecimal(dtThangTruoc1.Rows[0][i]) + Convert.ToDecimal(dtThangTruoc2.Rows[0][i]) + Convert.ToDecimal(dtThangTruoc3.Rows[0][i]);
                }
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtQuyTruoc.Rows.Count > 0)
                        dr[i + 2] = dtQuyTruoc.Rows[0][i];
                }
                dr["sKyHieu"] = "100";
                dr["sMoTa"] = "Quân số Q.toán quý trước";
                dtChungTuChiTiet.Rows.InsertAt(dr, 1);

                dtThangNay1 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, iThang1_HienTai, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                dtThangNay2 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, iThang2_HienTai, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                dtThangNay3 = QuyetToan_QuanSo_ChungTuChiTietModels.Get_QuanSoThangTruoc(iNamLamViec, iThang3_HienTai, iID_MaDonVi, "1", bCheckTongHop, iID_MaPhongBan, MaND);
                DataTable dtQuyNay = dtThangNay1.Copy();
                for (int i = 0; i < dtQuyNay.Columns.Count; i++)
                {
                    dtQuyNay.Rows[0][i] = Convert.ToDecimal(dtThangNay1.Rows[0][i]) + Convert.ToDecimal(dtThangNay2.Rows[0][i]) + Convert.ToDecimal(dtThangNay3.Rows[0][i]);
                }
                dr = dtChungTuChiTiet.NewRow();
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtQuyNay.Rows.Count > 0)
                        dr[i + 2] = dtQuyNay.Rows[0][i];
                }
                dr["sKyHieu"] = "700";
                dr["sMoTa"] = "Quân số Q.toán quý này";
                dtChungTuChiTiet.Rows.Add(dr);

                dr = dtChungTuChiTiet.NewRow();
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtThangNay1.Rows.Count > 0)
                        dr[i + 2] = dtThangNay1.Rows[0][i];
                }
                dr["sKyHieu"] = "700";
                dr["sMoTa"] = "Quân số Q.toán tháng " + Convert.ToInt32(iThang1_HienTai - 1);
                dtChungTuChiTiet.Rows.Add(dr);

                dr = dtChungTuChiTiet.NewRow();
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtThangNay2.Rows.Count > 0)
                        dr[i + 2] = dtThangNay2.Rows[0][i];
                }
                dr["sKyHieu"] = "700";
                dr["sMoTa"] = "Quân số Q.toán tháng " + Convert.ToInt32(iThang2_HienTai - 1);
                dtChungTuChiTiet.Rows.Add(dr);

                dr = dtChungTuChiTiet.NewRow();
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (dtThangNay3.Rows.Count > 0)
                        dr[i + 2] = dtThangNay3.Rows[0][i];
                }
                dr["sKyHieu"] = "700";
                dr["sMoTa"] = "Quân số Q.toán tháng " + Convert.ToInt32(iThang3_HienTai - 1);
                dtChungTuChiTiet.Rows.Add(dr);
                for (int i = 0; i < dtChungTuChiTiet.Rows.Count; i++)
                {
                    if ("2".Equals(dtChungTuChiTiet.Rows[i]["sKyHieu"]))
                    {
                        dtChungTuChiTiet.Rows[i]["sMoTa"] = "Quân số tăng trong Quý";
                    }
                    if ("3".Equals(dtChungTuChiTiet.Rows[i]["sKyHieu"]))
                    {
                        dtChungTuChiTiet.Rows[i]["sMoTa"] = "Quân số giảm trong Quý";
                    }

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
            DataTable data = DT_rptQTQS_THQS_TungDonVi_1();
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

