using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1040100_TungNganhController : FlexcelReportController
    {
        private static INganSachService _nganSachService;

        public rptDuToan_1040100_TungNganhController()
        {
            _nganSachService = NganSachService.Default;
        }

        //
        // GET: /rptDuToan_1040100_TungNganh/
        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath1 = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_1040100_TungNganh_1.xls";
        private const String sFilePath2 = "/Report_ExcelFrom/DuToan/DonVi/rptDuToan_1040100_TungNganh_2.xls";
        private const String sFilePath1_KG = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TungNganh_1_KG.xls";
        private const String sFilePath2_KG = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TungNganh_2_KG.xls";
        private static DataTable dtSoTo;
        public class dataDuLieu1
        {
            public DataTable dtDuLieu { get; set; }
            public DataTable dtdtDuLieuAll { get; set; }
            public ArrayList arrMoTa1 { get; set; }
            public ArrayList arrMoTa2 { get; set; }
            public ArrayList arrMoTa3 { get; set; }
        }

        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TungNganh.aspx";
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        public ActionResult EditSubmit(String ParentID)
        {
            string type = Request.Form["Type"];
            String MaTo = Request.Form["MaTo"];
            String Nganh = Request.Form[ParentID + "_Nganh"];
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String sLNS = Request.Form[ParentID + "_sLNS"];
            ViewData["PageLoad"] = "1";
            ViewData["MaTo"] = MaTo;
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["sLNS"] = sLNS;
            ViewData["Nganh"] = Nganh;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TungNganh.aspx";
            return View(sViewPath + "ReportView.aspx");
        }

        private static dataDuLieu1 _data;
        //public ExcelFile CreateReport(String path, String MaND, String Nganh, string ToSo, String sLNS, String iID_MaPhongBan)
        //{
        //    XlsFile Result = new XlsFile(true);
        //    DataTable dt = NguoiDungCauHinhModels.LayCauHinh(MaND);
        //    String  iNamLamViec = DateTime.Now.Year.ToString(), iID_MaNamNganSach = "1", iID_MaNguonNganSach = "1";
        //    if (dt.Rows.Count > 0)
        //    {
        //        iNamLamViec = Convert.ToString(dt.Rows[0]["iNamLamViec"]);
        //        iID_MaNamNganSach = Convert.ToString(dt.Rows[0]["iID_MaNamNganSach"]);
        //        iID_MaNguonNganSach = Convert.ToString(dt.Rows[0]["iID_MaNguonNganSach"]);
        //    }
        //    DataTable dtPB = DuToan_ReportModels.dtPhongBanInBaoDam();
        //    String sTenPB="";
        //    for (int j = 0; j < dtPB.Rows.Count; j++)
        //    {
        //        if (iID_MaPhongBan == Convert.ToString(dtPB.Rows[j]["iID"]))
        //        {
        //            sTenPB = Convert.ToString(dtPB.Rows[j]["sTen"]);
        //        }

        //    }
        //    Result.Open(path);
        //    FlexCelReport fr = new FlexCelReport();
        //    fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_1040100_TungNganh",MaND);
        //    _data = get_dtDuToan_1050000(MaND, Nganh,ToSo,sLNS,iID_MaPhongBan);
        //    DataTable data = _data.dtDuLieu;
        //    data.TableName = "ChiTiet";
        //    fr.AddTable("ChiTiet", data);
        //    data.Dispose();
        //    ArrayList arrMoTa1 = _data.arrMoTa1;
        //    ArrayList arrMoTa2 = _data.arrMoTa2;   
        //    ArrayList arrMoTa3 = _data.arrMoTa3;
        //    fr.SetValue("Nam", iNamLamViec);
        //    fr.SetValue("ToSo", ToSo);
        //    int i = 1;
        //    foreach (object obj in arrMoTa1)
        //    {
        //        fr.SetValue("MoTa1_" + i, obj);
        //        i++;
        //    }
        //    i = 1;
        //    foreach (object obj in arrMoTa2)
        //    {
        //        fr.SetValue("MoTa2_" + i, obj);
        //        i++;
        //    }
        //    i = 1;
        //    foreach (object obj in arrMoTa3)
        //    {
        //        fr.SetValue("MoTa3_" + i, obj);
        //        i++;
        //    }
        //    String sTenDonVi = "";
        //    sTenDonVi = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh", "iID", Nganh, "sTenNganh")); ;
        //    fr.SetValue("Cap2", sTenDonVi);
        //    fr.SetValue("sTenDonVi", sTenDonVi);
        //    fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1,MaND));
        //    fr.SetValue("ngaythang", ReportModels.Ngay_Thang_Nam_HienTai());
        //    fr.SetValue("thang", ReportModels.Thang_Nam_HienTai());
        //    fr.SetValue("sTenPhongBan", sTenPB);
        //    fr.Run(Result);
        //    return Result;

        //}

        public ExcelFile CreateReport(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            var file = getLoaiBaoCao(ToSo, iID_MaPhongBan);
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            var phongbanDich = _nganSachService.GetPhongBanById(iID_MaPhongBan ?? "10");

            _data = get_dtDuToan_1050000(Username, Nganh, ToSo, sLNS, iID_MaPhongBan);
            DataTable data = _data.dtDuLieu;
            data.TableName = "ChiTiet";

            var fr = new FlexCelReport();
            fr.AddTable("ChiTiet", data);
            data.Dispose();

            #region mota

            ArrayList arrMoTa1 = _data.arrMoTa1;
            ArrayList arrMoTa2 = _data.arrMoTa2;
            ArrayList arrMoTa3 = _data.arrMoTa3;
            int i = 1;
            foreach (object obj in arrMoTa1)
            {
                fr.SetValue("MoTa1_" + i, obj);
                i++;
            }
            i = 1;
            foreach (object obj in arrMoTa2)
            {
                fr.SetValue("MoTa2_" + i, obj);
                i++;
            }
            i = 1;
            foreach (object obj in arrMoTa3)
            {
                fr.SetValue("MoTa3_" + i, obj);
                i++;
            }
            #endregion


            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID = '" + Nganh + "'");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmd);

            var sTenDonVi = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            var loaiKhoan = HamChung.GetLoaiKhoanText("1040100");

            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("sTenPhongBan", phongbanDich.sMoTa);
            fr.SetValue("ToSo", ToSo);
            fr.SetValue("LoaiKhoan", loaiKhoan);

            fr.UseCommonValue()
             .UseChuKy(Username)
             .UseChuKyForController(this.ControllerName())
             //.UseChuKyForController(this.ControllerName(), iID_MaPhongBan)
             .Run(xls);

            var count = xls.TotalPageCount();
            if (ToSo != "1")
            {
                if (count > 1)
                {
                    xls.ClearDiffFirstPage();
                }
            }
            else
            {
                if (count > 1)
                {
                    xls.AddPageFirstPage();
                }
            }
            return xls;
        }

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan, string ext)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(Nganh, ToSo, sLNS, iID_MaPhongBan);
            //return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));

            return Print(xls, ext);
        }

        public ActionResult Download(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(Nganh, ToSo, sLNS, iID_MaPhongBan);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        private string getLoaiBaoCao(string toSo = "1", string maPhongBan = "0")
        {
            var file = string.Empty;

            if (maPhongBan == "0")
            {
                file = toSo == "1" ? sFilePath1 : sFilePath2;
            }
            else
            {
                file = toSo == "1" ? sFilePath1_KG : sFilePath2_KG;
            }

            return file;
        }

        public static dataDuLieu1 get_dtDuToan_1050000(String MaND, String Nganh, String ToSo, String sLNS, String iID_MaPhongBan)
        {

            //DataTable dtNganh = MucLucNganSach_NganhModels.Get_dtMucLucNganSach_Nganh(Nganh);
            int cs = 0, i = 0;
            String DSNganh = "", DKLNS = "", DKPB = "";
            if (sLNS != "0")
                DKLNS = " sLNS LIKE '" + sLNS + "%'";
            else
                DKLNS = " (sLNS LIKE '1040100%' OR sLNS LIKE '109%')";
            if (iID_MaPhongBan != "0")
                DKPB = " AND iID_MaPhongBanDich='" + iID_MaPhongBan + "'";
            string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID = '" + Nganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            string iID_MaNganhMLNS = "";
            if (dtNganhChon.Rows.Count > 0) { 
                iID_MaNganhMLNS = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
            }
            
            if (!String.IsNullOrEmpty(iID_MaNganhMLNS)) DSNganh = " AND sNG IN ("+ iID_MaNganhMLNS+")";
            else DSNganh = " AND sNG IN (" + _nganSachService.Nganh_GetAll_ChuyenNganh(MaND, Convert.ToInt32(ReportModels.LayNamLamViec(MaND))) + ")";
            String SQLNganh = "SELECT distinct sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQLNganh += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQLNganh += " FROM DT_ChungTuChiTiet_PhanCap WHERE sLNS='1020100' AND MaLoai<>'1' AND MaLoai <>'3' {1} AND iTrangThai=1 {2}  {0} {3}";

            SQLNganh += " UNION SELECT distinct sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQLNganh += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQLNganh += " FROM DT_ChungTuChiTiet WHERE sLNS=1040100 AND iKyThuat=1 AND MaLoai=1 {1} AND iTrangThai=1 {2}  {0}  {3} ORDER BY sM,sTM,sTTM,sNG";
            SQLNganh = String.Format(SQLNganh, DSNganh, ReportModels.DieuKien_NganSach_KhongDV(MaND), "", DKPB);
            SqlCommand cmdNG = new SqlCommand(SQLNganh);
            DataTable dtNG = Connection.GetDataTable(cmdNG);
            cmdNG.Dispose();
            String SQL;
            DataTable dtDonVi;           

            String DVT = "1000";
            SQL = "SELECT iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQL += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQL += ",SUM(rTuChi/{3}) AS rTuChi";
            SQL += ",SUM(rHienVat/{3}) AS rHienVat";
            SQL += " FROM DT_ChungTuChiTiet_PhanCap WHERE sLNS='1020100' AND MaLoai<>'1' AND MaLoai <>'3' {1}      AND iTrangThai=1 {2}   {0} {4}";
            SQL += " GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa";
            SQL += " HAVING SUM(rTuChi)>0";
            //SQL += " OR SUM(rHienVat)>0";
            SQL += " UNION SELECT iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQL += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQL += ",SUM(rTuChi/{3}) AS rTuChi";
            SQL += ",SUM(rHienVat/{3}) AS rHienVat";
            SQL += " FROM DT_ChungTuChiTiet WHERE sLNS='1040100' AND iKyThuat=1   AND MaLoai=1 {1} AND iTrangThai=1 {2}   {0} {4} ";
            SQL += " GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa";
            SQL += " HAVING SUM(rTuChi)>0";
            //SQL += " OR SUM(rHienVat)>0";
            SQL = String.Format(SQL, DSNganh, ReportModels.DieuKien_NganSach_KhongDV(MaND), "", DVT, DKPB);

            String strSQL = "SELECT CT.iID_MaDonVi,CT.iID_MaDonVi+' - '+ NS_DonVi.sTen AS TenDonVi";
            strSQL += ",sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,CT.sMoTa";
            strSQL += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            strSQL += ",rTuChi, rHienVat";
            strSQL += " FROM ({0}) CT ";
            strSQL += " INNER JOIN (SELECT iID_MaDonVi as MaDonVi, sTen FROM  NS_DonVi WHERE iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec) as NS_DonVi ON NS_DonVi.MaDonVi=CT.iID_MaDonVi";
            strSQL = String.Format(strSQL, SQL);
            SqlCommand cmd = new SqlCommand(strSQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));

            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();

            dtDonVi = HamChung.SelectDistinct("dtDonVi", dt, "iID_MaDonVi", "iID_MaDonVi,TenDonVi");

            i = 0;
            //cs = 3;//tờ 1 4 cột
            dtDonVi.Columns.Add("TongTuChi", typeof(Decimal));
            while (i < dtNG.Rows.Count)
            {
                if (dtDonVi.Columns.IndexOf(dtNG.Rows[i]["NG"].ToString() + "_TuChi") < 0)
                    dtDonVi.Columns.Add(dtNG.Rows[i]["NG"].ToString() + "_TuChi", typeof(Decimal));
                i = i + 1;
            }

            i = 0;
            cs = 0;
            String MaDonVi, MaDonVi1, TenCot;
            for (i = 0; i < dtDonVi.Rows.Count; i++)
            {
                MaDonVi = Convert.ToString(dtDonVi.Rows[i]["iID_MaDonVi"]).Trim();
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    MaDonVi1 = Convert.ToString(dt.Rows[j]["iID_MaDonVi"]).Trim();
                    TenCot = Convert.ToString(dt.Rows[j]["NG"]).Trim();
                    if (MaDonVi == MaDonVi1 && dtDonVi.Columns.IndexOf(TenCot + "_TuChi") >= 0)
                    {
                        dtDonVi.Rows[i][TenCot + "_TuChi"] = dt.Rows[j]["rTuChi"];
                        dt.Rows.RemoveAt(j);
                        j = j - 1;
                    }
                }
            }
            i = 0;
            //j=3 vì trừ cột madv, đơn vị và cột tổng cộng
            Double Tong = 0;
            for (int j = 3; j < dtDonVi.Columns.Count; j++)
            {
                Tong = 0;
                for (i = 0; i < dtDonVi.Rows.Count; i++)
                {
                    if (dtDonVi.Rows[i][j] != DBNull.Value)
                    {
                        Tong = Tong + Convert.ToDouble(dtDonVi.Rows[i][j]);
                    }
                }
                if (Tong == 0)
                {
                    dtDonVi.Columns.RemoveAt(j);
                    if (j == 1) j = 1;
                    else j = j - 1;
                }
            }
            Double TongHienVat = 0, TongTuChi = 0;
            for (i = 0; i < dtDonVi.Rows.Count; i++)
            {
                TongHienVat = 0; TongTuChi = 0;
                //j=3 vì trừ cột MaDV, đơn vị và cột tổng cộng
                for (int j = 3; j < dtDonVi.Columns.Count; j++)
                {
                    if (dtDonVi.Rows[i][j] != DBNull.Value)
                    {
                        if (dtDonVi.Columns[j].ColumnName.IndexOf("_HienVat") >= 0)
                        {
                            TongHienVat = TongHienVat + Convert.ToDouble(dtDonVi.Rows[i][j]);
                        }
                        else
                        {
                            TongTuChi = TongTuChi + Convert.ToDouble(dtDonVi.Rows[i][j]);
                        }
                    }
                }
                dtDonVi.Rows[i]["iID_MaDonVi"] = (i + 1).ToString();
                dtDonVi.Rows[i]["TongTuChi"] = TongTuChi;
            }
            DataTable _dtDonVi = new DataTable();
            DataTable _dtDonVi1 = new DataTable();

            int TongSoCot = 0;
            int SoTrang = 1;
            int SoCotCanThem = 0;
            if ((dtDonVi.Columns.Count - 3) == 0)
            {
                SoCotCanThem = 3;
                TongSoCot = (dtDonVi.Columns.Count - 3) + SoCotCanThem;
            }
            else if ((dtDonVi.Columns.Count - 3) <= 5)
            {

                int SoCotDu = ((dtDonVi.Columns.Count - 3)) % 5;
                if (SoCotDu != 0)
                    SoCotCanThem = 5 - SoCotDu;
                TongSoCot = (dtDonVi.Columns.Count - 3) + SoCotCanThem;
            }
            else
            {
                int SoCotDu = (dtDonVi.Columns.Count - 8) % 6;
                if (SoCotDu != 0)
                    SoCotCanThem = 6 - SoCotDu;
                TongSoCot = (dtDonVi.Columns.Count - 3) + SoCotCanThem;
                SoTrang = 1 + (TongSoCot - 3) / 6;
            }
            for (i = 0; i < SoCotCanThem; i++)
            {
                dtDonVi.Columns.Add();
            }
            int _ToSo = Convert.ToInt16(ToSo);
            int SoCotTrang1 = 5;
            int SoCotTrangLonHon1 = 6;
            _dtDonVi = dtDonVi.Copy();
            int _CS = 0;
            String BangTien_HienVat = "";
            //Mổ tả xâu nối mã
            ArrayList arrMoTa1 = new ArrayList();
            //Mỏ tả ngành
            ArrayList arrMoTa2 = new ArrayList();
            //Bằng Tiền hay bằng hiện vật
            ArrayList arrMoTa3 = new ArrayList();
            if (ToSo == "1")
            {

                for (i = 3; i < 3 + SoCotTrang1; i++)
                {
                    TenCot = _dtDonVi.Columns[i].ColumnName;
                    _CS = TenCot.IndexOf("_");
                    //Thêm dữ liệu arrMota1 va 2
                    if (_CS == -1)
                    {
                        arrMoTa1.Add("");
                        arrMoTa2.Add("");
                    }
                    else
                    {
                        arrMoTa1.Add(Convert.ToString(TenCot.Substring(0, _CS)));
                        DataRow[] R = dtNG.Select("NG='" + TenCot.Substring(0, _CS) + "'");
                        arrMoTa2.Add(Convert.ToString(R[0]["sMoTa"]));
                    }
                    //Thêm dữ liệu arrmota 3
                    if (TenCot.IndexOf("_TuChi") >= 0) BangTien_HienVat = "Bằng tiền";
                    else if (TenCot.IndexOf("_HienVat") >= 0) BangTien_HienVat = "Bằng hiện vật";
                    else BangTien_HienVat = "";
                    arrMoTa3.Add(BangTien_HienVat);

                    //Đổi tên cột
                    _dtDonVi.Columns[i].ColumnName = "Cot" + (i - 2);
                }
            }
            else
            {
                int tg = 3 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 2);
                int dem = 1;
                for (i = 3 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 2); i < 3 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 1); i++)
                {
                    if (i < 0)
                        break;
                    TenCot = _dtDonVi.Columns[i].ColumnName;
                    _CS = TenCot.IndexOf("_");
                    //Thêm dữ liệu arrMota1 va 2
                    if (_CS == -1)
                    {
                        arrMoTa1.Add("");
                        arrMoTa2.Add("");
                    }
                    else
                    {
                        arrMoTa1.Add(Convert.ToString(TenCot.Substring(0, _CS)));
                        DataRow[] R = dtNG.Select("NG='" + TenCot.Substring(0, _CS) + "'");
                        arrMoTa2.Add(Convert.ToString(R[0]["sMoTa"]));
                    }
                    //Thêm dữ liệu arrmota 3
                    if (TenCot.IndexOf("_TuChi") >= 0) BangTien_HienVat = "Bằng tiền";
                    else if (TenCot.IndexOf("_HienVat") >= 0) BangTien_HienVat = "Bằng hiện vật";
                    else BangTien_HienVat = "";
                    arrMoTa3.Add(BangTien_HienVat);

                    //Đổi tên cột
                    _dtDonVi.Columns[i].ColumnName = "Cot" + dem;
                    dem++;
                }
            }

            dataDuLieu1 _data = new dataDuLieu1();
            _data.dtDuLieu = _dtDonVi;
            _data.arrMoTa1 = arrMoTa1;
            _data.arrMoTa2 = arrMoTa2;
            _data.arrMoTa3 = arrMoTa3;
            _data.dtdtDuLieuAll = dtDonVi;
            return _data;
        }
        public JsonResult Ds_DonVi(String ParentID, String Nganh, String ToSo, String sLNS, String iID_MaPhongBan)
        {
            String MaND = User.Identity.Name;
            if (String.IsNullOrEmpty(ToSo))
                ToSo = "1";
            DataTable dt = DanhSachToIn(MaND, Nganh, "1", sLNS, iID_MaPhongBan);

            if (String.IsNullOrEmpty(ToSo))
            {
                ToSo = Guid.Empty.ToString();
            }
            String ViewNam = "~/Views/DungChung/DonVi/To_DanhSach.ascx";
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, ToSo, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }
        public static DataTable DanhSachToIn(String MaND, String Nganh, String ToSo, String sLNS, String iID_MaPhongBan)
        {
            _data = get_dtDuToan_1050000(MaND, Nganh, ToSo, sLNS, iID_MaPhongBan);
            DataTable dtToIn = new DataTable();
            dtToIn.Columns.Add("MaTo", typeof(String));
            dtToIn.Columns.Add("TenTo", typeof(String));
            DataRow R = dtToIn.NewRow();
            dtToIn.Rows.Add(R);
            R[0] = "1";
            R[1] = "Tờ 1";
            if (_data.dtdtDuLieuAll != null)
            {
                int a = 2;
                for (int i = 0; i < _data.dtdtDuLieuAll.Columns.Count - 8; i = i + 6)
                {
                    DataRow R1 = dtToIn.NewRow();
                    dtToIn.Rows.Add(R1);
                    R1[0] = a;
                    R1[1] = "Tờ " + a;
                    a++;
                }
            }
            return dtToIn;
        }
    }
}
