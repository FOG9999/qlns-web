using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using DomainModel;
using DomainModel.Controls;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;
using System.Collections;
using VIETTEL.Flexcel;
using Viettel.Services;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_2_B8_TongHopController : AppController
    {
        //
        // GET: /rptDuToan_2_B8_TongHop/
        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath1 = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_1.xls";
        private const String sFilePath2 = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_2.xls";
        private const String sFilePath1_KG = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_1_TrinhKy.xls";
        private const String sFilePath2_KG = "/Report_ExcelFrom/DuToan/rptDuToan_2_B8_TongHop_2_TrinhKy.xls";

        private Viettel.Domain.DomainModel.DC_NguoiDungCauHinh _config;
        private readonly INganSachService _nganSachService;

        public rptDuToan_2_B8_TongHopController()
        {
            _nganSachService = NganSachService.Default;
        }
        public class dataDuLieu1
        {
            public DataTable dtDuLieu { get; set; }
            public DataTable dtdtDuLieuAll { get; set; }
            public ArrayList arrMoTa1 { get; set; }
            public ArrayList arrMoTa2 { get; set; }
            public ArrayList arrMoTa3 { get; set; }
        }

        public ActionResult Index(string iID_MaPhongBan)
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_2_B8_TongHop.aspx";
                ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        public ActionResult EditSubmit(string ParentID, string iID_MaPhongBan)
        {
            String MaTo = Request.Form["MaTo"];
            String Nganh = Request.Form[ParentID + "_Nganh"];
            //String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String sLNS = Request.Form[ParentID + "_sLNS"];
            ViewData["PageLoad"] = "1";
            ViewData["MaTo"] = MaTo;
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["sLNS"] = sLNS;
            ViewData["Nganh"] = Nganh;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_2_B8_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        private static dataDuLieu1 _data;
        public ExcelFile CreateReport(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            //DataTable dt = NguoiDungCauHinhModels.LayCauHinh(Username);
            //String  iNamLamViec = DateTime.Now.Year.ToString(), iID_MaNamNganSach = "1", iID_MaNguonNganSach = "1";
            //if (dt.Rows.Count > 0)
            //{
            //    iNamLamViec = Convert.ToString(dt.Rows[0]["iNamLamViec"]);
            //    iID_MaNamNganSach = Convert.ToString(dt.Rows[0]["iID_MaNamNganSach"]);
            //    iID_MaNguonNganSach = Convert.ToString(dt.Rows[0]["iID_MaNguonNganSach"]);
            //}

            //DataTable dtPB = DuToan_ReportModels.dtPhongBanInBaoDam();
            //String sTenPB="";
            //for (int j = 0; j < dtPB.Rows.Count; j++)
            //{
            //    if (iID_MaPhongBan == Convert.ToString(dtPB.Rows[j]["iID"]))
            //    {
            //        sTenPB = Convert.ToString(dtPB.Rows[j]["sTen"]);
            //    }
    
            //}

            var file = "";
            if (iID_MaPhongBan == "0")
            {
                file = ToSo == "1" ? sFilePath1 : sFilePath2; 
            }
            else
            {
                file = ToSo == "1" ? sFilePath1_KG : sFilePath2;
            }

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            _data = get_dtDuToan_1050000(Username, Nganh,ToSo,sLNS,iID_MaPhongBan);
            DataTable data = _data.dtDuLieu;
            data.TableName = "ChiTiet";

            var fr = new FlexCelReport();
            fr.AddTable("ChiTiet", data);
            data.Dispose();

            int i = 1;
            foreach (object obj in _data.arrMoTa1)
            {
                fr.SetValue("MoTa1_" + i, obj);
                i++;
            }
            i = 1;
            foreach (object obj in _data.arrMoTa2)
            {
                fr.SetValue("MoTa2_" + i, obj);
                i++;
            }
            i = 1;
            foreach (object obj in _data.arrMoTa3)
            {
                fr.SetValue("MoTa3_" + i, obj);
                i++;
            }
            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID = '" + Nganh + "'");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmd);

            var sTenDonVi = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("ToSo", ToSo);

            _config = _nganSachService.GetCauHinh(Username);
            fr.SetValue("TieuDe1", string.Format("Dự toán chi ngân sách nhà nước giao năm {0}", _config.iNamLamViec));
            fr.SetValue("TieuDe2", "");

            fr.UseCommonValue()
              .UseChuKy(Username, iID_MaPhongBan)
              .UseChuKyForController(this.ControllerName())
              .Run(xls);
            return xls;
        }

        public static dataDuLieu1 get_dtDuToan_1050000(String MaND, String Nganh, String ToSo, String sLNS, String iID_MaPhongBan)
        {

            //DataTable dtNganh = MucLucNganSach_NganhModels.Get_dtMucLucNganSach_Nganh(Nganh);
            int cs = 0, i = 0;
            String DSNganh = "",DKLNS="",DKPB="";
            sLNS = "2";
            if (sLNS != "0")
                DKLNS = " sLNS LIKE '" + sLNS + "%'";
            else
                DKLNS = " (sLNS LIKE '4%')";
            if (iID_MaPhongBan != "0")
                DKPB = " AND iID_MaPhongBanDich='" + iID_MaPhongBan + "'";
            String SQLNganh = "SELECT distinct sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQLNganh += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQLNganh += " FROM DT_ChungTuChiTiet_PhanCap WHERE 0=1 AND sLNS='1020100' AND MaLoai<>'1' {1} AND iTrangThai in (1,2) {2}  {0} {3}";

             SQLNganh += " UNION SELECT distinct sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQLNganh += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQLNganh += " FROM DT_ChungTuChiTiet WHERE sLNS LIKE '2%' AND sLNS NOT IN ('2060101','2060102') AND iID_MaPhongBan='08'   {1} AND iTrangThai in (1,2) {2}  {0}  {3} ORDER BY sM,sTM,sTTM,sNG";
            SQLNganh = String.Format(SQLNganh, "", ReportModels.DieuKien_NganSach_KhongDV(MaND), "",DKPB);
            SqlCommand cmdNG = new SqlCommand(SQLNganh);
            //cmdNG.Parameters.AddWithValue("@NamLamViec", NamLamViec);
            DataTable dtNG = Connection.GetDataTable(cmdNG);
            cmdNG.Dispose();
            String SQL;
            DataTable dtDonVi;

            String DVT = "1000";
            SQL = "SELECT iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQL += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQL += ",SUM(rTuChi/{3}+rDuPhong/{3}) AS rTuChi";
            SQL += ",SUM(rHienVat/{3}) AS rHienVat";
            SQL += " FROM DT_ChungTuChiTiet_PhanCap WHERE 0=1 AND sLNS='1020100' AND MaLoai<>'1' {1}      AND iTrangThai in (1,2) {2}   {0} {4}";
            SQL += " GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa";
            SQL += " HAVING SUM(rTuChi+rDuPhong)>0 OR SUM(rHienVat)>0";
            SQL += " UNION SELECT iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";
            SQL += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            SQL += ",SUM(rTuChi/{3}+rDuPhong/{3}) AS rTuChi";
            SQL += ",SUM(rHienVat/{3}) AS rHienVat";
            SQL += " FROM DT_ChungTuChiTiet WHERE sLNS LIKE '2%' AND sLNS NOT IN ('2060101','2060102') AND iID_MaPhongBan='08' {1} AND iTrangThai in (1,2) {2}   {0} {4} ";
            SQL += " GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa";
            SQL += " HAVING SUM(rTuChi+rDuPhong)>0 OR SUM(rHienVat)>0";
            SQL = String.Format(SQL, DSNganh, ReportModels.DieuKien_NganSach_KhongDV(MaND), "", DVT,DKPB);

            String strSQL = "SELECT CT.iID_MaDonVi,CT.iID_MaDonVi+' - '+ NS_DonVi.sTen AS TenDonVi";
            strSQL += ",sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,CT.sMoTa";
            strSQL += ",sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG";
            strSQL += ",rTuChi,rHienVat";
            strSQL += " FROM ({0}) CT ";
            strSQL += " INNER JOIN (SELECT iID_MaDonVi as MaDonVi, sTen FROM  NS_DonVi WHERE iTrangThai in (1,2) AND iNamLamViec_DonVi=@iNamLamViec) as NS_DonVi ON NS_DonVi.MaDonVi=CT.iID_MaDonVi";
            strSQL = String.Format(strSQL, SQL);
            SqlCommand cmd = new SqlCommand(strSQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();

            dtDonVi = HamChung.SelectDistinct("dtDonVi", dt, "iID_MaDonVi", "iID_MaDonVi,TenDonVi");

            i = 0;
            //cs = 3;//tờ 1 4 cột
            dtDonVi.Columns.Add("TongTuChi", typeof(Decimal));
            dtDonVi.Columns.Add("TongHienVat", typeof(Decimal));
            while (i < dtNG.Rows.Count)
            {
                if (dtDonVi.Columns.IndexOf(dtNG.Rows[i]["NG"].ToString() + "_TuChi") < 0)
                    dtDonVi.Columns.Add(dtNG.Rows[i]["NG"].ToString() + "_TuChi", typeof(Decimal));
                if (dtDonVi.Columns.IndexOf(dtNG.Rows[i]["NG"].ToString() + "_HienVat") < 0)
                    dtDonVi.Columns.Add(dtNG.Rows[i]["NG"].ToString() + "_HienVat", typeof(Decimal));
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
                        dtDonVi.Rows[i][TenCot + "_HienVat"] = dt.Rows[j]["rHienVat"];
                        dt.Rows.RemoveAt(j);
                        j = j - 1;
                    }
                }
            }
            i = 0;
            //j=4 vì trừ cột madv, đơn vị và 2 cột tổng cộng
            Double Tong = 0;
            for (int j = 4; j < dtDonVi.Columns.Count; j++)
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
                //j=4 vì trừ cột MaDV, đơn vị và 2 cột tổng cộng
                for (int j = 4; j < dtDonVi.Columns.Count; j++)
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
                dtDonVi.Rows[i]["TongHienVat"] = TongHienVat;
                dtDonVi.Rows[i]["TongTuChi"] = TongTuChi;
            }
            DataTable _dtDonVi = new DataTable();
            DataTable _dtDonVi1 = new DataTable();

            int TongSoCot = 0;
            int SoTrang = 1;
            int SoCotCanThem = 0;
            int SoCotTrang1 = 5;
            int SoCotTrangLonHon1 = 6;
            if ((dtDonVi.Columns.Count - SoCotTrang1) == 0)
            {
                SoCotCanThem = SoCotTrang1;
                TongSoCot = (dtDonVi.Columns.Count - SoCotTrang1) + SoCotCanThem;
            }
            else if ((dtDonVi.Columns.Count - SoCotTrang1) <= SoCotTrang1)
            {

                int SoCotDu = ((dtDonVi.Columns.Count - SoCotTrang1)) % SoCotTrang1;
                if (SoCotDu != 0)
                    SoCotCanThem = SoCotTrang1 - SoCotDu;
                TongSoCot = (dtDonVi.Columns.Count - SoCotTrang1) + SoCotCanThem;
            }
            else
            {
                int SoCotDu = (dtDonVi.Columns.Count - SoCotTrang1 - SoCotTrang1) % SoCotTrangLonHon1;
                if (SoCotDu != 0)
                    SoCotCanThem = SoCotTrangLonHon1 - SoCotDu;
                TongSoCot = (dtDonVi.Columns.Count - SoCotTrang1) + SoCotCanThem;
                SoTrang = 1 + (TongSoCot - SoCotTrang1) / SoCotTrangLonHon1;
            }
            for (i = 0; i < SoCotCanThem; i++)
            {
                dtDonVi.Columns.Add();
            }
            int _ToSo = Convert.ToInt16(ToSo);
           
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

                for (i = 4; i < 4 + SoCotTrang1; i++)
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
                    _dtDonVi.Columns[i].ColumnName = "Cot" + (i - 3);
                }
            }
            else
            {
                int tg = 4 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 2);
                int dem = 1;
                for (i = 4 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 2); i < 4 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 1); i++)
                {
                    if(i<0)
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
            if(String.IsNullOrEmpty(ToSo))
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
        public static DataTable DanhSachToIn(String MaND, String Nganh, String ToSo,String sLNS,String iID_MaPhongBan)
        {
            _data = get_dtDuToan_1050000(MaND,Nganh,ToSo,sLNS,iID_MaPhongBan);
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
                for (int i = 0; i < _data.dtdtDuLieuAll.Columns.Count - 10; i = i + 6)
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


        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(Nganh, ToSo, sLNS, iID_MaPhongBan);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string Nganh, string ToSo, string sLNS, string iID_MaPhongBan)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(Nganh, ToSo, sLNS, iID_MaPhongBan);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}
