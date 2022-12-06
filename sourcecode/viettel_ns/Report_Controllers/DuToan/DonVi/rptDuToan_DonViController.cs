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
    public class rptDuToan_DonViController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_DonVi.xls";


        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_DonVi.aspx";
                ViewData["PageLoad"] = "0";
               // DanhSachBaoCao("trolyphongban", "110", "");
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
            String MaTo = Request.Form["MaTo"];
            String iID_MaDonVi = Request.Form[ParentID + "_iID_MaDonVi"];
            ViewData["PageLoad"] = "1";
            ViewData["MaTo"] = MaTo;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["path"] = "~/Report_Views/DuToan/DonVi/rptDuToan_DonVi.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        public JsonResult Ds_DonVi(String ParentID, String iID_MaDonVi,String BaoCao, String MaTo)
        {
            String MaND = User.Identity.Name;
            if (String.IsNullOrEmpty(iID_MaDonVi))
                iID_MaDonVi = "1";
            DataTable dt = DanhSachBaoCao(MaND, iID_MaDonVi, MaTo);


            String ViewNam = "~/Views/DungChung/DonVi/BaoCao_DanhSach.ascx";
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, BaoCao, dt, ParentID);
            dt.Dispose();
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }
        public static DataTable DanhSachBaoCao(String MaND, String iID_MaDonVi, String MaTo)
        {
            DataTable dtCoSoLieu = new DataTable();
            DataTable vR = DanhMucModels.DT_DanhMuc_All("DanhSachBaoCaoDonVi");
            dtCoSoLieu = vR.Copy();
            String sConTroller = "", sLNS = "";
            for (int i = vR.Rows.Count-1; i >=0; i--)
            {

                bool bCoSoLieu = true;
                DataRow dr = vR.Rows[i];
                sConTroller = Convert.ToString(vR.Rows[i]["sTenKhoa"]);
                sLNS = Convert.ToString(vR.Rows[i]["sGhiChu"]);
                String DK = "";
                SqlCommand cmd = new SqlCommand();
                if (!String.IsNullOrEmpty(sLNS))
                {
                    String[] arrLNS = sLNS.Split(',');
                    for (int j = 0; j < arrLNS.Length; j++)
                    {
                        DK += " sLNS LIKE @sLNS" + j;
                        if (j < arrLNS.Length - 1)
                            DK += " OR ";
                        cmd.Parameters.AddWithValue("@sLNS"+j,arrLNS[j]+"%");
                    }
                    DK = "(" + DK + ")";
                    String SQL="";
                    if (sConTroller == "rptDuToan_1010000_TungDonVi_DoanhNghiep" ||sConTroller== "rptDuToan_1020100_TungDonVi_DoanhNghiep")
                    {
                        DK += " AND iID_MaPhongBan='06'";
                    }


                    String DSNganhNN = "", iID_MaNganhMLNSNN = "";

                    String SQLNganhNN = "SELECT * FROM NS_MucLucNganSach_Nganh_NhaNuoc WHERE iTrangThai=1 AND iID_MaDonVi=@iID_MaDonVi";
                    SqlCommand cmdNganhNN = new SqlCommand(SQLNganhNN);
                    cmdNganhNN.Parameters.AddWithValue(@"iID_MaDonVi", iID_MaDonVi);
                    DataTable dtNganhNN = Connection.GetDataTable(cmdNganhNN);



                    if (dtNganhNN.Rows.Count <= 0)
                        DSNganhNN = " AND sNG IN (123)";
                    else
                    {
                        for (int c = 0; c < dtNganhNN.Rows.Count; c++)
                        {
                            iID_MaNganhMLNSNN += dtNganhNN.Rows[c]["iID_MaNganhMLNS"];
                            if (c < dtNganhNN.Rows.Count - 1)
                                iID_MaNganhMLNSNN += ",";
                        }
                        DSNganhNN = " AND sNG IN (" + iID_MaNganhMLNSNN + ")";
                    }

                    //nganh quoc phong
                    if (sConTroller == "rptDuToan_1040100_TungNganh")
                    {
                        dtCoSoLieu.Rows.RemoveAt(i);
                        String DSNganh = "";

                        string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
                        SqlCommand cmdNganh = new SqlCommand(sql);
                        cmdNganh.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                        DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

                        string iID_MaNganhMLNS = "";
                        if (dtNganhChon.Rows.Count > 0)
                        {
                            iID_MaNganhMLNS = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
                        }
                        DSNganh = " AND sNG IN (" + iID_MaNganhMLNS + ")";
                        if (String.IsNullOrEmpty(iID_MaNganhMLNS)) DSNganh = " AND sNG IN (123)";
                        String SQLNganh = "SELECT distinct sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
                        SQLNganh += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
                        SQLNganh += " FROM DT_ChungTuChiTiet_PhanCap WHERE sLNS='1020100' AND MaLoai<>'1' {1} AND iTrangThai=1   {0} ";

                        SQLNganh += " UNION SELECT distinct sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
                        SQLNganh += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
                        SQLNganh += " FROM DT_ChungTuChiTiet WHERE sLNS=1040100 AND iKyThuat=1 AND MaLoai=1 {1} AND iTrangThai in (1,2)   {0}   ORDER BY sM,sTM,sTTM,sNG";
                        SQLNganh = String.Format(SQLNganh, DSNganh, ReportModels.DieuKien_NganSach_KhongDV(MaND));
                        SqlCommand cmdNG = new SqlCommand(SQLNganh);
                        //cmdNG.Parameters.AddWithValue("@NamLamViec", NamLamViec);
                        DataTable dtNG = Connection.GetDataTable(cmdNG);
                        cmdNG.Dispose();

                        //co so lieu
                        if (dtNG.Rows.Count > 0)
                        {
                            //sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
                            //SqlCommand cmdNganhC = new SqlCommand(sql);
                            //cmdNganhC.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                            //DataTable dtNganhC = Connection.GetDataTable(cmdNganhC);

                            //var MaNganh = Convert.ToString(dtNganhChon.Rows[0]["iID"]);
                            //DataTable dtTo = rptDuToan_1040100_TungNganhController.DanhSachToIn(MaND, MaNganh, "1", "0", "0");
                            //if (dtTo.Rows.Count >= 1)
                            //{
                            //    for (int j = 1; j <= dtTo.Rows.Count; j++)
                            //    {
                            //        DataRow dr1 = dtCoSoLieu.NewRow();
                            //        dr1["sTenKhoa"] = "/rptDuToan_1040100_TungNganh/viewpdf?ToSo=" + j + "&MaND=" + MaND + "&Nganh=" + MaNganh + "&sLNS=0&iID_MaPhongBan=0";
                            //        dr1["sGhiChu"] = "1020100,1020000";
                            //        dr1["sTen"] = "Bảo đảm chi tiết tờ " + j;
                            //        dtCoSoLieu.Rows.InsertAt(dr1, i + j - 1);

                            //    }

                            //}
                            //dtTo.Dispose();

                        }
                        //else if (sConTroller == "rptDuToan_DonVi_02b")
                        //{
                        //    sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
                        //    SqlCommand cmdNganhC = new SqlCommand(sql);
                        //    cmdNganhC.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                        //    DataTable dtNganhC = Connection.GetDataTable(cmdNganhC);

                        //    var MaNganh = Convert.ToString(dtNganhChon.Rows[0]["iID"]);
                        //}
                        //                        else
                        //                        {
                        //                            i--;
                        //                        }
                        //                        SQL = String.Format(@"SELECT SUM(count)
                        //FROM(
                        //SELECT COUNT(*) as count
                        //FROM DT_ChungTuChiTiet
                        //WHERE iTrangThai=1 AND iID_MaDonVi=@iID_MaDonVi AND ({0})
                        // AND (rTuChi<>0 OR rHienVat<>0 OR rDuPhong<>0 OR rPhanCap<>0 OR rHangNhap<>0 OR rHangMua<>0)
                        //UNION
                        //SELECT COUNT(*) as count
                        //FROM DT_ChungTuChiTiet_PhanCap
                        //WHERE iTrangThai=1 AND iID_MaDonVi=@iID_MaDonVi AND ({0})
                        // AND (rTuChi<>0 OR rHienVat<>0 OR rDuPhong<>0 OR rPhanCap<>0 OR rHangNhap<>0 OR rHangMua<>0)) as a
                        //", DK);
                        //                        cmd.CommandText = SQL;
                        //                        cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                        //                        int a = Convert.ToInt16(Connection.GetValue(cmd, 0));
                        //                        if (a <= 0)
                        //                            bCoSoLieu = false;
                        //                        if (bCoSoLieu == false)
                        //                        {
                        //                            dtCoSoLieu.Rows.RemoveAt(i);
                        //                        }
                        //                        else
                        //                        {
                        //                            //Dem so tờ có trong báo cáo
                        //                        }
                    }
                    //neu la ngan sach nganh nha nuoc to bia
                    else if (sConTroller == "rptDuToan_1030100_TungDonVi")
                    {
                        dtCoSoLieu.Rows.RemoveAt(i);
                    }
                    else if (sConTroller == "rptDuToan_207_TungDonVi")
                    {
                        dtCoSoLieu.Rows.RemoveAt(i);


                        String SQLNganh = "SELECT DISTINCT sNG";
                        SQLNganh += " FROM DT_ChungTuChiTiet WHERE sLNS LIKE '207%'  {1} AND iTrangThai in (1,2)   {0} ";
                        SQLNganh = String.Format(SQLNganh, DSNganhNN, ReportModels.DieuKien_NganSach_KhongDV(MaND));
                        SqlCommand cmdNG = new SqlCommand(SQLNganh);
                        //cmdNG.Parameters.AddWithValue("@NamLamViec", NamLamViec);
                        DataTable dtNG = Connection.GetDataTable(cmdNG);
                        cmdNG.Dispose();

                        if (dtNG.Rows.Count > 0)
                        {

                            for (int j = 0; j < dtNG.Rows.Count; j++)
                            {
                                for (int z = 0; z < dtNganhNN.Rows.Count; z++)
                                {
                                    if (dtNganhNN.Rows[z]["iID_MaNganhMLNS"].ToString().Contains(dtNG.Rows[j]["sNG"].ToString()))
                                    {
                                        DataRow dr1 = dtCoSoLieu.NewRow();
                                        dr1["sTenKhoa"] = "/rptDuToan_207_TungDonVi/viewpdf?MaND=" + MaND + "&iID_MaDonVi=" + dtNG.Rows[j]["sNG"];
                                        dr1["sGhiChu"] = "207";
                                        dr1["sTen"] = "QLHC-Ngành " + dtNganhNN.Rows[z]["sTenNganh"];
                                        dtCoSoLieu.Rows.InsertAt(dr1, i + j);
                                        break;
                                    }
                                }
                            }
                        }

                    }
                    else if (sConTroller == "rptDuToan_207_TungNganh")
                    {
                        dtCoSoLieu.Rows.RemoveAt(i);
                        String SQLNganh = "SELECT DISTINCT sNG";
                        SQLNganh += " FROM DT_ChungTuChiTiet_PhanCap WHERE sLNS LIKE '207%'  {1} AND iTrangThai=1   {0} ";
                        SQLNganh = String.Format(SQLNganh, DSNganhNN, ReportModels.DieuKien_NganSach_KhongDV(MaND));
                        SqlCommand cmdNG = new SqlCommand(SQLNganh);
                        //cmdNG.Parameters.AddWithValue("@NamLamViec", NamLamViec);
                        DataTable dtNG = Connection.GetDataTable(cmdNG);
                        cmdNG.Dispose();
                        if (dtNG.Rows.Count > 0)
                        {

                            for (int j = 0; j < dtNG.Rows.Count; j++)
                            {
                                int temp = 0;
                                for (int z = 0; z < dtNganhNN.Rows.Count; z++)
                                {
                                    if (dtNganhNN.Rows[z]["iID_MaNganhMLNS"].ToString().Contains(dtNG.Rows[j]["sNG"].ToString()))
                                    {
                                        DataTable dtTo = rptDuToan_207_TungNganhController.DanhSachToIn(MaND, dtNganhNN.Rows[z]["iID"].ToString(), "1", "207", "0");
                                        for (int c = 1; c <= dtTo.Rows.Count; c++)
                                        {
                                            DataRow dr1 = dtCoSoLieu.NewRow();
                                            dr1["sTenKhoa"] = "/rptDuToan_207_TungNganh/viewpdf?ToSo=" + c + "&MaND=" + MaND + "&iID_MaDonVi=" + dtNG.Rows[j]["sNG"] + "&Nganh=" + dtNganhNN.Rows[z]["iID"] + "&sLNS=0&iID_MaPhongBan=0"; ;
                                            dr1["sGhiChu"] = "207";
                                            dr1["sTen"] = "QLHC-Ngành " + dtNganhNN.Rows[z]["sTenNganh"] + " phân cấp tờ số " + c;
                                            temp++;
                                            dtCoSoLieu.Rows.InsertAt(dr1, i + temp - 1);
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        SQL = String.Format(@"
SELECT SUM(count)  as  count FROM
(
SELECT COUNT(*) as count
FROM DT_ChungTuChiTiet
WHERE iTrangThai in (1,2) AND iID_MaDonVi=@iID_MaDonVi AND ({0}) AND iNamLamViec=@iNamLamViec
 AND (rTuChi<>0 OR rHienVat<>0 OR rDuPhong<>0 OR rPhanCap<>0 OR rHangNhap<>0 OR rHangMua<>0)
 UNION ALL
 SELECT COUNT(*) as count
FROM DT_ChungTuChiTiet_PhanCap
WHERE iTrangThai=1 AND iID_MaDonVi=@iID_MaDonVi AND ({0}) AND iNamLamViec=@iNamLamViec
 AND (rTuChi<>0 OR rHienVat<>0 OR rDuPhong<>0 OR rPhanCap<>0 OR rHangNhap<>0 OR rHangMua<>0)
) as a

", DK);

                        cmd.CommandText = SQL;
                        cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                        cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                        int a = Convert.ToInt16(Connection.GetValue(cmd, 0));
                        if (a <= 0)
                            bCoSoLieu = false;
                        if (bCoSoLieu == false)
                        {
                            dtCoSoLieu.Rows.RemoveAt(i);
                        }
                    }
                        
                }
                cmd.Dispose();
                //Check co so lieu
            }
            dtCoSoLieu.Dispose();
            vR.Dispose();
            return dtCoSoLieu;
        }

    }
}

