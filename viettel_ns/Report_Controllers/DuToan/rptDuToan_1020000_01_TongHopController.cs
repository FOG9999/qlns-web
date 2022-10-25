using DomainModel;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1020000_01_TongHopController : FlexcelReportController
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_1020000_01_TongHop.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/rptDuToan_1020000_01_TongHop_Muc.xls";
        String iCapTongHop = "";
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1020000_01_TongHop.aspx";
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
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1020000_01_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND, String iID_MaPhongBan)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);


            String sTenDonVi = "B -  " + iID_MaPhongBan;
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
                sTenDonVi = ReportModels.CauHinhTenDonViSuDung(2, MaND);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaPhongBan);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1020000_01_TongHop");
            fr.SetValue("Nam", iNamLamViec);
            fr.SetValue("Cap2", sTenDonVi);
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());

            var loaiKhoan = HamChung.GetLoaiKhoanText("1020000");
            fr.SetValue("LoaiKhoan", loaiKhoan);


            fr.Run(Result);
            return Result;

        }

        //1020200
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_1020000_01_TongHop(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_Dich(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;

            //Lấy tên khối


            String SQL = "";
            String sTenB10 = "", sTenB6 = "", sTenB = "";

            sTenB10 = "Tổng cục, BTTM";

            sTenB6 = "D.nghiệp";

            sTenB = "QK,QĐ,HVNT";

            SQL = String.Format(@"
SELECT sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBan,sTenPB,
SUM(rTuChi) as rTuChi,
SUM(rHienVat) as rHienVat,
SUM(rChiTapTrung) as rChiTapTrung
FROM
(
SELECT sM,sTM,sTTM,sNG,sMoTa,
iID_MaPhongBan,sTenPB=CASE WHEN iID_MaPhongBan=10 THEN N'{4}' 
                            WHEN iID_MaPhongBan=06 THEN N'{5}'
                            WHEN iID_MaPhongBan NOT IN (06,10)  THEN N'{6}' END
,rTuChi=SUM(rTuChi/{3})
,rChiTapTrung=SUM(rTuChi/{3})-SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND sLNS IN ('1020000','1020100') THEN (rTuChi-rChiTapTrung)/{3} ELSE 0 END)
,rHienVat=SUM(rHienVat/{3})
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1   AND (sLNS='1020100' OR sLNS='1020000')   AND iNamLamViec=@iNamLamViec {0} {1} {2}
 GROUP BY sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBan
HAVING SUM(rTuChi/{3})<>0 OR SUM(rHienVat/{3})<>0 OR SUM(rChiTapTrung/{3})<>0

UNION all

SELECT sM,sTM,sTTM,sNG,sMoTa,
iID_MaPhongBanDich AS iID_MaPhongBan,sTenPB=CASE WHEN iID_MaPhongBanDich=10 THEN N'{4}' 
                            WHEN iID_MaPhongBanDich=06 THEN N'{5}'
                            WHEN iID_MaPhongBanDich NOT IN (06,10)  THEN N'{6}' END
,rTuChi=SUM(rTuChi/{3})
,rChiTapTrung=SUM(rTuChi/{3})-SUM(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1020000','1020100') THEN (rTuChi-rChiTapTrung)/{3} ELSE 0 END)
,rHienVat=SUM(rHienVat/{3})
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND (sLNS='1020100' OR sLNS='1020000')  AND iNamLamViec=@iNamLamViec {0} {1} {2}
 GROUP BY sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBanDich
HAVING SUM(rTuChi/{3})<>0 OR SUM(rHienVat/{3})<>0 OR SUM(rChiTapTrung/{3})<>0
) as a
GROUP BY sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBan,sTenPB
ORDER BY sM,sTM,sTTM,sNG,sMoTa,sTenPB"
 , "", DKPhongBan, DKDonVi, DVT, sTenB10, sTenB6, sTenB);
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
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            DataRow r;
            //DataTable data = DT_rptDuToan_1020000_01_TongHop(MaND, iID_MaPhongBan);

            var data = getDataTable_1020000_ChonTo(iID_MaPhongBan);

            data.Columns.Add("CheckTrung", typeof(String));
            for (int i = 1; i < data.Rows.Count; i++)
            {
                String sXau1, sXau2;
                sXau1 = Convert.ToString(data.Rows[i - 1]["sM"]) + Convert.ToString(data.Rows[i - 1]["sTM"]) + Convert.ToString(data.Rows[i - 1]["sTTM"]) + Convert.ToString(data.Rows[i - 1]["sNG"]);
                sXau2 = Convert.ToString(data.Rows[i]["sM"]) + Convert.ToString(data.Rows[i]["sTM"]) + Convert.ToString(data.Rows[i]["sTTM"]) + Convert.ToString(data.Rows[i]["sNG"]);
                if (sXau1.Equals(sXau2))
                {
                    data.Rows[i]["CheckTrung"] = "1";
                }
            }

            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sM,sTM", "sM,sTM,sMoTa", "sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sM", "sM,sMoTa", "sM,sTM");
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);



            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan, String iCapTongHop)
        {
            this.iCapTongHop = iCapTongHop;
            if (iCapTongHop == "Muc")
            {
                sFilePath = sFilePath_Muc;
            }
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1020200_TongHop.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaPhongBan, String iCapTongHop)
        {
            this.iCapTongHop = iCapTongHop;
            if (iCapTongHop == "Muc")
            {
                sFilePath = sFilePath_Muc;
            }
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan);
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



        #region longsam

        /// <summary>
        /// Nghiep vu chung
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <returns></returns>
        private DataTable getDataTable_1020000_ChonTo(string id_phongban = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_1020000_TongHop.sql");

            var id_donvi = Request.GetQueryString("id_donvi", PhienLamViec.iID_MaDonVi);

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@id_donvi", id_donvi.ToParamString());
                cmd.Parameters.AddWithValue("@NamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", 1000);
                var dt = cmd.GetTable();
                return dt;
            }
            #endregion

        }

        #endregion
    }
}
