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
using System.Data.OleDb;
using System.Text;
using System.Security.Cryptography;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptQuyetToan_1010000_TungDonVi1Controller : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptQuyetToan_1010000_TungDonVi1.xls";
        private const String sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/rptQuyetToan_1010000_TungDonVi1_TrinhKy.xls";
        public string sTrinhKy = "";
        string sLNS = "", iNamLamViec = "", iIDMaNamNganSach = "";
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1010000_TungDonVi.aspx";
                ViewData["PageLoad"] = "0";
                sTrinhKy = Request.QueryString["sTrinhKy"];
                ViewData["sTrinhKy"] = sTrinhKy;
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        public void loadData(FlexCelReport fr)
        {
            string sFileName = String.Format(@"D:\VIETTEL\Code\Work\VIETTEL_NS\Libraries\Book1.xls");
            String ConnectionString = String.Format(ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes'", sFileName);
            OleDbConnection connExcel = new OleDbConnection(ConnectionString);
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = null;
            conn = new OleDbConnection(ConnectionString);
            OleDbCommand cmdExcel = new OleDbCommand();
            try
            {

                cmdExcel.Connection = connExcel;
                connExcel.Open();
                conn.Open();

                DataTable dtSheet = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String Sheetname = "";
                DataTable dt = new DataTable();
                OleDbDataAdapter adapter;
                for (int i = 0; i < dtSheet.Rows.Count; i++)
                {
                    Sheetname = Convert.ToString(dtSheet.Rows[i]["TABLE_NAME"]);
                    cmd.CommandText = String.Format(@"SELECT * FROM [{0}]", Sheetname);
                    cmd.Connection = conn;
                    adapter = new OleDbDataAdapter(cmd);
                    DataTable dt1 = new DataTable();
                    adapter.Fill(dt1);
                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {


                        DataRow dr1 = dt1.Rows[j];
                        if (dt.Columns.Count == 0)
                        {
                            for (int c = 0; c < dt1.Columns.Count; c++)
                            {
                                dt.Columns.Add(dt1.Columns[c].ColumnName, typeof(string));
                            }

                        }
                        DataRow dr = dt.NewRow();
                        for (int z = 0; z < dt1.Columns.Count; z++)
                        {
                            dr[z] = Encrypt(Convert.ToString(dr1[z]), true);
                        }

                        dt.Rows.Add(dr);
                    }
                }
                fr.AddTable("ChiTiet", dt);
                DataTable dtMaHoa = dt.Copy();
                for (int i = 0; i < dtMaHoa.Rows.Count; i++)
                {
                    for (int j = 0; j < dtMaHoa.Columns.Count; j++)
                    {
                        dtMaHoa.Rows[i][j] = Decrypt(Convert.ToString(dt.Rows[i][j]), true);
                    }
                }



            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                // Đóng chuỗi kết nối
                conn.Close();
            }



        }
        public string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("nguoibaypro"));
            }
            else keyArray = Encoding.UTF8.GetBytes("iloveit1208");
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string Decrypt(string toDecrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("nguoibaypro"));
            }
            else keyArray = Encoding.UTF8.GetBytes("iloveit1208");
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }


        /// <summary>
        /// hàm lấy các giá trên form
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public ActionResult EditSubmit(String ParentID)
        {
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            sTrinhKy = Request.Form[ParentID + "_sTrinhKy"];
            ViewData["PageLoad"] = "1";
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["sTrinhKy"] = sTrinhKy;
            ViewData["path"] = "~/Report_Views/DuToan/rptQuyetToan_1010000_TungDonVi1.aspx";
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

            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            loadData(fr);
            fr = ReportModels.LayThongTinChuKy(fr, "rptQuyetToan_1010000_TungDonVi1", MaND);
            fr.Run(Result);
            return Result;

        }

        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public DataTable DT_rptQuyetToan_1010000_TungDonVi1(String MaND, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);
            int DVT = 1000;
            String iID_MaPhongBanQuanLy = DonViModels.getPhongBanCuaDonVi(iID_MaDonVi, User.Identity.Name);
            String SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi/{3})
,rChiTapChung=SUM(CASE WHEN iID_MaPhongBan='06' THEN rTuChi/{3}	ELSE rChiTapTrung/{3} END)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND sLNS='1010000' AND iID_MaDonVi =@iID_MaDonVi AND {4} AND iNamLamViec=@iNamLamViec {1} {2}
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi)<> 0 OR SUM(rChiTapTrung)<>0 ", iID_MaDonVi, DKPhongBan, DKDonVi, DVT, iID_MaPhongBanQuanLy);
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
            DataTable data = DT_rptQuyetToan_1010000_TungDonVi1(MaND, iID_MaDonVi);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);


        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaDonVi)
        {
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1010000_TungDonVi.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaDonVi, String sTrinhKy)
        {
            HamChung.Language();
            String DuongDan = "";
            if (String.IsNullOrEmpty(sTrinhKy))
            {
                DuongDan = sFilePath;
            }
            else
                DuongDan = sFilePath_TrinhKy;
            ExcelFile xls = CreateReport(Server.MapPath(DuongDan), MaND, iID_MaDonVi);
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

