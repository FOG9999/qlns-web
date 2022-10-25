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
using VIETTEL.Flexcel;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_2_TungDonViController : AppController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private String sFilePath = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_2_TungDonVi.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_2_TungDonVi_Muc.xls";
        String iCapTongHop = "";

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_2_TungDonVi.aspx";
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
        public ActionResult EditSubmit(string ParentID, string iID_MaDonVi)
        {
            var iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];

            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iCapTongHop"] = iCapTongHop;

            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_2_TungDonVi.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        //public ExcelFile CreateReport(String path, String MaND, String iID_MaDonVi)
        //{
        //    DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
        //    String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);

        //    String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi,MaND);
        //    XlsFile Result = new XlsFile(true);
        //    Result.Open(path);
        //    FlexCelReport fr = new FlexCelReport();
        //    LoadData(fr, MaND, iID_MaDonVi);
        //    fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_2_TungDonVi");
        //    fr.SetValue("Nam", iNamLamViec);
        //    fr.SetValue("sTenDonVi", sTenDonVi);
        //    fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
        //    fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
        //    fr.SetValue("Thang", ReportModels.Thang_Nam_HienTai());
        //    fr.Run(Result);
        //    return Result;

        //}

        public ExcelFile CreateReport(string iID_MaDonVi, string iCapTongHop)
        {

            var file = sFilePath;
            if (iCapTongHop == "Muc")
            {
                file = sFilePath_Muc;
            }

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));


            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaDonVi);

            var sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, Username);
            fr.SetValue("sTenDonVi", sTenDonVi);
            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);

            fr.UseCommonValue()
            .UseChuKy(Username)
            .UseChuKyForController(this.ControllerName())
            .Run(xls);

            var count = xls.TotalPageCount();

            if (count > 1)
            {
                xls.AddPageFirstPage();
            }

            return xls;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_2_TungDonVi(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT SUBSTRING(sLNS,1,3) as sLNS3,SUBSTRING(sLNS,1,5) as sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,iID_MaPhongBan
,rTuChi=SUM(rTuChi/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai <> 0 AND SUBSTRING(sLNS,1,1) IN (2,3) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,iID_MaPhongBan
HAVING SUM(rTuChi)<>0 OR SUM(rPhanCap/1000)<>0 OR SUM(rDuPhong/1000)<>0

UNION ALL

SELECT SUBSTRING(sLNS,1,3) as sLNS3,SUBSTRING(sLNS,1,5) as sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,iID_MaPhongBan
,rTuChi=SUM(rTuChi/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai <> 0 AND SUBSTRING(sLNS,1,1) IN (2,3) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,iID_MaPhongBan
HAVING SUM(rTuChi)<>0 OR SUM(rPhanCap/1000)<>0 OR SUM(rDuPhong/1000)<>0

", iID_MaDonVi, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static DataTable DT_rptDuToan_2_TungDonVi_GhiChu(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String SQL = String.Format(@"SELECT SUBSTRING(sLNS,1,3) as sLNS3,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,iID_MaPhongBan
,rTuChi=SUM(rTuChi/{3})
,rPhanCap=SUM(rPhanCap/{3})
,rDuPhong=SUM(rDuPhong/{3})
,sGhiChu
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND SUBSTRING(sLNS,1,1) IN (2,3) AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi,iID_MaPhongBan,sGhiChu
HAVING SUM(rTuChi)<>0 OR SUM(rPhanCap/1000)<>0 OR SUM(rDuPhong/1000)<>0", iID_MaDonVi, DKPhongBan, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
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
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaDonVi)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_2_TungDonVi(MaND, iID_MaDonVi);
            DataTable dataGhiChu = DT_rptDuToan_2_TungDonVi_GhiChu(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS3,sLNS5,sLNS,sL,sK", "sLNS3,sLNS5,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS3,sLNS5,sLNS", "sLNS3,sLNS5,sLNS,sMoTa", "sLNS,sL");
            DataTable dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", dtsLNS, "sLNS3,sLNS5", "sLNS3,sLNS5,sMoTa", "sLNS,sL");
            DataTable dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS3", "sLNS3,sMoTa", "");

            for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            {
                r = dtsLNS3.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS3"]));
            }
            for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            {
                r = dtsLNS5.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS5"]));
            }
            fr.AddTable("GhiChu", dataGhiChu);
            DataTable dtMoTaGhiChu = HamChung.SelectDistinct("dtMoTaGhiChu", dataGhiChu, "sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu", "");
            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
            {
                DataRow dr = dtMoTaGhiChu.Rows[i];
                var items = new List<string>();
                dr["sMoTa"] += "-";
                for (int j = 0; j < dataGhiChu.Rows.Count; j++)
                {
                    DataRow dr1 = dataGhiChu.Rows[j];


                    if (Convert.ToString(dr["sM"]) == Convert.ToString(dr1["sM"]) && Convert.ToString(dr["sTM"]) == Convert.ToString(dr1["sTM"]) && Convert.ToString(dr["sTTM"]) == Convert.ToString(dr1["sTTM"]) && Convert.ToString(dr["sNG"]) == Convert.ToString(dr1["sNG"]))
                    {
                        //dr["sMoTa"] += dr1["sGhiChu"] + " " + Convert.ToDecimal(dr1["rTuChi"]).ToString("###,###");
                        //dr["sMoTa"] += ", ";

                        //items.Add(dr1["sGhiChu"] + ": " + Convert.ToDecimal(dr1["rTuChi"]).ToString("###,###"));
                        items.Add(dr1["sGhiChu"].ToString());

                        //  dr["sMoTa"] = dr["sMoTa"] + (j < dataGhiChu.Rows.Count - 1 ? ", " :"");

                    }
                }
                dr["sMoTa"] += items.Join(", ");

            }

            fr.AddTable("dtMoTaGhiChu", dtMoTaGhiChu);


            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS5", dtsLNS5);
            fr.AddTable("dtsLNS3", dtsLNS3);



            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtsLNS3.Dispose();
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

        }

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string iID_MaDonVi, string iCapTongHop)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaDonVi, iCapTongHop);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaDonVi, string iCapTongHop)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaDonVi, iCapTongHop);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}

