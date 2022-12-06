using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1050000_TongHop_ChonToController : FlexcelReportController
    {
        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath1 = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_1050000_TongHop_ChonTo.xls";
        private const String sFilePath1_TrinhKy = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_1050000_TongHop_ChonTo_TrinhKy.xls";
        private const String sFilePath2 = "/Report_ExcelFrom/DuToan/Cuc/rptDuToan_1050000_TongHop_ChonTo2.xls";
        public string sTrinhKy = "";

        public class dataDuLieu1
        {
            public DataTable dtDuLieu { get; set; }
            public DataTable dtdtDuLieuAll { get; set; }
            public ArrayList arrMoTa1 { get; set; }
            public ArrayList arrMoTa2 { get; set; }
            public ArrayList arrMoTa3 { get; set; }
        }
        [Authorize]
        public ActionResult Index(int trinhky = 0)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_1050000_TongHop_ChonTo.aspx";
                sTrinhKy = Request.QueryString["sTrinhKy"];
                ViewData["trinhky"] = trinhky;
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        [Authorize]
        public ActionResult EditSubmit(String ParentID, int trinhky = 0)
        {
            String MaTo = Request.Form["MaTo"];
            ViewData["trinhky"] = trinhky;
            ViewData["PageLoad"] = "1";
            ViewData["MaTo"] = MaTo;
            ViewData["path"] = "~/Report_Views/DuToan/Cuc/rptDuToan_1050000_TongHop_ChonTo.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        private static dataDuLieu1 _data;
        
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string ToSo, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(ToSo, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string ToSo, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(ToSo, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ExcelFile CreateReport(string ToSo, int trinhky = 0)
        {
            var file = sFilePath2;
            if (ToSo == "1")
            {
                file = trinhky == 0 ? sFilePath1 : sFilePath1_TrinhKy;
            }

            XlsFile xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            _data = get_dtDuToan_1050000(Username, ToSo);
            DataTable data = _data.dtDuLieu;
            data.TableName = "ChiTiet";

            FlexCelReport fr = new FlexCelReport();
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
            var loaiKhoan = HamChung.GetLoaiKhoanText("1050000");

            fr.SetValue("ToSo", ToSo);
            fr.SetValue("LoaiKhoan", loaiKhoan);

            fr.UseCommonValue()
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .Run(xls);

            return xls;

        }
        
        public static dataDuLieu1 get_dtDuToan_1050000(String MaND, String ToSo)
        {

            int cs = 0, i = 0;

            var dt = getDataTable_1050000_ChonTo(MaND, "");
            var dtDonVi = HamChung.SelectDistinct("dtDonVi", dt, "iID_MaDonVi", "iID_MaDonVi,TenDonVi");
            var dtNG = dt.SelectDistinct("dtNG", "NG,sMoTa");

            i = 0;
            //cs = 3;//tờ 1 4 cột
            dtDonVi.Columns.Add("TongTuChi", typeof(Decimal));
            dtDonVi.Columns.Add("TongHangNhap", typeof(Decimal));
            while (i < dtNG.Rows.Count)
            {
                if (dtDonVi.Columns.IndexOf(dtNG.Rows[i]["NG"].ToString() + "_TuChi") < 0)
                    dtDonVi.Columns.Add(dtNG.Rows[i]["NG"].ToString() + "_TuChi", typeof(Decimal));
                if (dtDonVi.Columns.IndexOf(dtNG.Rows[i]["NG"].ToString() + "_HangNhap") < 0)
                    dtDonVi.Columns.Add(dtNG.Rows[i]["NG"].ToString() + "_HangNhap", typeof(Decimal));
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
                        dtDonVi.Rows[i][TenCot + "_HangNhap"] = dt.Rows[j]["rHangNhap"];
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
            Double TongHangNhap = 0, TongTuChi = 0;
            for (i = 0; i < dtDonVi.Rows.Count; i++)
            {
                TongHangNhap = 0; TongTuChi = 0;
                //j=4 vì trừ cột MaDV, đơn vị và 2 cột tổng cộng
                for (int j = 4; j < dtDonVi.Columns.Count; j++)
                {
                    if (dtDonVi.Rows[i][j] != DBNull.Value)
                    {
                        if (dtDonVi.Columns[j].ColumnName.IndexOf("_HangNhap") >= 0)
                        {
                            TongHangNhap = TongHangNhap + Convert.ToDouble(dtDonVi.Rows[i][j]);
                        }
                        else
                        {
                            TongTuChi = TongTuChi + Convert.ToDouble(dtDonVi.Rows[i][j]);
                        }
                    }
                }
                dtDonVi.Rows[i]["iID_MaDonVi"] = (i + 1).ToString();
                dtDonVi.Rows[i]["TongHangNhap"] = TongHangNhap;
                dtDonVi.Rows[i]["TongTuChi"] = TongTuChi;
            }
            DataTable _dtDonVi = new DataTable();
            DataTable _dtDonVi1 = new DataTable();

            int TongSoCot = 0;
            int SoTrang = 1;
            int SoCotCanThem = 0;
            if ((dtDonVi.Columns.Count - 4) == 0)
            {
                SoCotCanThem = 5;
                TongSoCot = (dtDonVi.Columns.Count - 4) + SoCotCanThem;
            }
            else if ((dtDonVi.Columns.Count - 4) <= 4)
            {

                int SoCotDu = ((dtDonVi.Columns.Count - 4)) % 5;
                if (SoCotDu != 0)
                    SoCotCanThem = 5 - SoCotDu;
                TongSoCot = (dtDonVi.Columns.Count - 4) + SoCotCanThem;
            }
            else
            {
                int SoCotDu = (dtDonVi.Columns.Count - 4 - 5) % 7;
                if (SoCotDu != 0)
                    SoCotCanThem = 7 - SoCotDu;
                TongSoCot = (dtDonVi.Columns.Count - 4) + SoCotCanThem;
                SoTrang = 1 + (TongSoCot - 4) / 7;
            }
            for (i = 0; i < SoCotCanThem; i++)
            {
                dtDonVi.Columns.Add();
            }
            int _ToSo = Convert.ToInt16(ToSo);
            int SoCotTrang1 = 5;
            int SoCotTrangLonHon1 = 7;
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
                    else if (TenCot.IndexOf("_HangNhap") >= 0) BangTien_HienVat = "Bằng ngoại tệ";
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
                    else if (TenCot.IndexOf("_HangNhap") >= 0) BangTien_HienVat = "Bằng ngoại tệ";
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
        public static DataTable DanhSachToIn(String MaND, String Nganh, String ToSo)
        {
            _data = get_dtDuToan_1050000(MaND, ToSo);

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
                for (int i = 0; i < _data.dtdtDuLieuAll.Columns.Count - 9; i = i + 7)
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


        #region longsam

        private static DataTable getDataTable_1050000_ChonTo(string MaND, string id_phongban = null)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_1050000_TongHop_ChonTo.sql");
            
            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@id_donvi", NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND).AsEnumerable().Select(x => x.Field<string>("iID_MaDonVi")).Join(","));
                cmd.Parameters.AddWithValue("@NamLamViec", ReportModels.LayNamLamViec(MaND));
                cmd.Parameters.AddWithValue("@dvt", 1000);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion

        }

        #endregion
    }
}
