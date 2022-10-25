using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptDuToanBS_1040100_NganhViewModel
    {
        public string iNamLamViec { get; set; }
        public string iID_MaDot { get; set; }
        public SelectList DotList { get; set; }
        public SelectList PhongList { get; set; }
        public SelectList NganhList { get; set; }
        public SelectList NguonList { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToanBS_1040100_NganhController : FlexcelReportController
    {
        private const string _filePath1 = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Nganh_1.xls";
        private const string _filePath2 = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Nganh_2.xls";
        private const string _filePath1_TrinhKy = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Nganh_1_TrinhKy.xls";
        private const string _filePath2_TrinhKy = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Nganh_2_TrinhKy.xls";

        #region ctor

        private const string _viewPath = "~/Views/Report_Views/DuToanBS/";
        private const string _filePath = "~/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Tobia.xls";
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _bsService = DuToanBsService.Default;

        public ActionResult Index()
        {

            var namLamViec = ReportModels.LayNamLamViec(Username);

            //var dotList = DuToanBS_ReportModels.LayDSDot(namLamViec, Username, "1040100")
            //    .ToSelectList("iID_MaDot", "iID_MaDot", "-1", "-- Chọn đợt --");
            //var nguonList = DuToan_ReportModels.dtNguonInBaoDam().ToSelectList("iID", "sTen");


            var dotList = DuToanBsService.Default.GetDots_NSBD(PhienLamViec.iNamLamViec, Username)
                .AsEnumerable()
                .Select(x => new
                {
                    iID_MaDot = x.Field<string>("iID_MaDot"),
                    sMoTa = x.Field<string>("iID_MaDot") + " - " + _bsService.GetDot_MoTa(PhienLamViec.iNamLamViec, Username, PhienLamViec.iID_MaPhongBan, x.Field<string>("iID_MaDot"), "104,109")
                })
                .ToDictionary(x => x.iID_MaDot, x => x.sMoTa)
                .ToSelectList("-1", "-- Chọn đợt --");


            var phongList = _nganSachService.GetPhongBanQuanLyNS().ToSelectList("iID_MaPhongBan", "sMoTa");
            var nguonList = new Dictionary<string, string>()
            {
                {"104", "Từ 104"},
                {"109", "Từ 109"},
            }
            .ToSelectList("104,109");
            var vm = new rptDuToanBS_1040100_NganhViewModel
            {
                iNamLamViec = namLamViec,
                DotList = dotList,
                NguonList = nguonList,
                PhongList = phongList,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        private ExcelFile createReport(String path, String Nganh, String ToSo, String sLNS, String iID_MaPhongBan, String iID_MaDot)
        {
            var xls = new XlsFile(true);
            xls.Open(path);
            var fr = new FlexCelReport();

            #region mota

            var reportVM = getData(Nganh, ToSo, sLNS, iID_MaPhongBan, iID_MaDot);
            DataTable data = reportVM.dtDuLieu;
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            data.Dispose();

            int i = 1;
            foreach (object obj in reportVM.arrMoTa1)
            {
                fr.SetValue("MoTa1_" + i, obj);
                i++;
            }
            i = 1;
            foreach (object obj in reportVM.arrMoTa2)
            {
                fr.SetValue("MoTa2_" + i, obj);
                i++;
            }
            //i = 1;
            //foreach (object obj in reportVM.arrMoTa3)
            //{
            //    fr.SetValue("MoTa3_" + i, obj);
            //    i++;
            //}

            #endregion

            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + Nganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sql);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            var sTenDonVi = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            var dvt = Request.GetQueryStringValue("dvt", 1000).ToStringNumber();

            fr.SetValue(new
            {
                sTenPhongBan = _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa,
                sTenDonVi = sTenDonVi,
                DotNgay = iID_MaDot.ToParamDate().ToStringNgay().ToLower(),
            })
            .UseCommonValue(
                new Application.Flexcel.FlexcelModel()
                {
                    header = $"Đơn vị tính: {dvt} đồng - Tờ số: {ToSo}",
                    header2 = $"Ngành: {sTenDonVi}",
                    footer = $"Đợt: {iID_MaDot.ToParamDate().ToStringNgay().ToLower()}",
                    //Thang = "Ngày ...... tháng 12 năm 2020",
                    //Ngay = "Ngày ...... tháng ...... năm 2020",
                })
            .UseChuKy(Username, PhienLamViec.PhongBan.sKyHieu)
            .UseChuKyForController(this.ControllerName())
            .UseForm(this)
            .Run(xls);

            return xls;
        }

        public ActionResult Print(
            string iID_MaNganh,
            string toSo,
            string sLNS,
            string iID_MaPhongBan,
            string iID_MaDot,
            string ext = "pdf")
        {
            var filePath = "";
            if (string.IsNullOrWhiteSpace(iID_MaPhongBan) || iID_MaPhongBan == "-1")
            {
                filePath = toSo == "1" ? _filePath1 : _filePath2;
            }
            else
            {
                filePath = toSo == "1" ? _filePath1_TrinhKy : _filePath2_TrinhKy;
            }

            var xls = createReport(Server.MapPath(filePath), iID_MaNganh, toSo, sLNS, iID_MaPhongBan, iID_MaDot);
            var filename = string.Format("{0}_{1}.{2}", this.ControllerName(), DateTime.Now.GetTimeStamp(), ext);

            return Print(xls, ext);
        }

        #region public methods

        public JsonResult Ds_Nganh(string Nganh, string iID_MaDot)
        {
            var data = DuToanBS_ReportModels.dtNganh(iID_MaDot, User.Identity.Name, "1");

            var vm = new ChecklistModel("Nganh", data.ToSelectList("iID_MaNganh", "sTenNganh"));
            return ToDropdownList(vm);
        }

        public JsonResult Ds_PhongBan(string iID_MaNganh, string iID_MaDot, string iID_MaPhongBan)
        {
            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sql);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            iID_MaNganh = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganh"]);
            var data = DuToan_ReportModels.dtPhongBanInBaoDamBS(iID_MaDot, iID_MaNganh, Username);

            var vm = new ChecklistModel("Nganh", data.ToSelectList("iID_MaPhongBan", "iID_MaPhongBan", "-1"));
            return ToDropdownList(vm);
        }

        public JsonResult Ds_ToIn(
            string iID_MaNganh,
            string toSo,
            string sLNS,
            string iID_MaPhongBan,
            string iID_MaDot)
        {
            try
            {
                var data = getData(iID_MaNganh, toSo, sLNS, iID_MaPhongBan, iID_MaDot);
                return ds_ToIn(data.ColumnsCount);
            }
            catch (Exception)
            {
            }

            return ds_ToIn();
        }

        #endregion

        #region private methods

        private ReportDataModel getData(String Nganh, String ToSo, String sLNS, String iID_MaPhongBan, String iID_MaDot)
        {
            DataTable dtNG, dt;

            #region params

            string
                iNamLamViec,
                iID_MaNamNganSach,
                iID_MaNguonNganSach,
                sNG,
                iID_MaChungTu;



            var config = NguoiDungCauHinhModels.LayCauHinh(Username).Rows[0];
            iNamLamViec = config["iNamLamViec"].ToString();
            iID_MaNamNganSach = config["iID_MaNamNganSach"].ToString();
            iID_MaNguonNganSach = config["iID_MaNguonNganSach"].ToString();

            string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + Nganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            sNG = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);

            #endregion

            #region nganh
            //var sql = FileHelpers.GetSqlQuery("dtbs_nganh_nsbd.sql");

            //using (var conn = ConnectionFactory.Default.GetConnection())
            //{
            //    using (var cmd = new SqlCommand(sql, conn))
            //    {
            //        cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            //        cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
            //        cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            //        cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
            //        cmd.Parameters.AddWithValue("@sLNS", sLNS.ToParamString());
            //        cmd.Parameters.AddWithValue("@sNG", sNG);

            //        dtNG = cmd.GetTable();
            //    }
            //}

            #endregion

            #region sql - du lieu


            var sql = FileHelpers.GetSqlQuery("dtbs_nganh_nsbd_donvi.sql");

            iID_MaChungTu = DuToanBS_ReportModels.GetChungTuList_TheoDot(iID_MaDot, Username);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@sLNS", sLNS.ToParamString());
                    cmd.Parameters.AddWithValue("@sNG", sNG);
                    cmd.Parameters.AddWithValue("@dvt", 1000);

                    dt = cmd.GetTable();
                }
            }

            #endregion

            return getReportData(dt, ToSo);
        }

        private ReportDataModel getReportData(DataTable dt, string ToSo)
        {
            var dtDonVi = HamChung.SelectDistinct("dtDonVi", dt, "iID_MaDonVi", "iID_MaDonVi,TenDonVi");
            var dtNG = dt.SelectDistinct("NG", "NG,sMoTa");

            var i = 0;

            //cs = 3;//tờ 1 4 cột
            dtDonVi.Columns.Add("TongTuChi", typeof(Decimal));
            //dtDonVi.Columns.Add("TongHienVat", typeof(Decimal));
            while (i < dtNG.Rows.Count)
            {
                if (dtDonVi.Columns.IndexOf(dtNG.Rows[i]["NG"].ToString() + "_TuChi") < 0)
                    dtDonVi.Columns.Add(dtNG.Rows[i]["NG"].ToString() + "_TuChi", typeof(Decimal));
                //if (dtDonVi.Columns.IndexOf(dtNG.Rows[i]["NG"].ToString() + "_HienVat") < 0)
                //    dtDonVi.Columns.Add(dtNG.Rows[i]["NG"].ToString() + "_HienVat", typeof(Decimal));
                i = i + 1;
            }

            i = 0;
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
                        //dtDonVi.Rows[i][TenCot + "_HienVat"] = dt.Rows[j]["rHienVat"];
                        dt.Rows.RemoveAt(j);
                        j = j - 1;
                    }
                }
            }
            i = 0;
            //j=4 vì trừ cột madv, đơn vị và 2 cột tổng cộng
            Double Tong = 0;
            var removeColumns = new List<DataColumn>();
            for (int j = 3; j < dtDonVi.Columns.Count; j++)
            {
                #region  viettel

                //Tong = 0;
                //for (i = 0; i < dtDonVi.Rows.Count; i++)
                //{
                //    if (dtDonVi.Rows[i][j] != DBNull.Value)
                //    {
                //        Tong = Tong + Convert.ToDouble(dtDonVi.Rows[i][j]);
                //    }
                //}
                //if (Tong == 0)
                //{
                //    dtDonVi.Columns.RemoveAt(j);
                //    if (j == 1) j = 1;
                //    else j = j - 1;
                //}

                #endregion

                // lay ca so am
                var hasValue = false;
                for (i = 0; i < dtDonVi.Rows.Count; i++)
                {
                    hasValue = dtDonVi.Rows[i][j] != DBNull.Value && Convert.ToDouble(dtDonVi.Rows[i][j]) != 0;
                    if (hasValue)
                    {
                        break;
                    }
                }

                if (!hasValue)
                {
                    removeColumns.Add(dtDonVi.Columns[j]);
                }
            }

            removeColumns.ForEach(c => dtDonVi.Columns.Remove(c));


            Double TongHienVat = 0, TongTuChi = 0;
            for (i = 0; i < dtDonVi.Rows.Count; i++)
            {
                TongHienVat = 0; TongTuChi = 0;
                //j=4 vì trừ cột MaDV, đơn vị và 2 cột tổng cộng
                for (int j = 3; j < dtDonVi.Columns.Count; j++)
                {
                    if (dtDonVi.Rows[i][j] != DBNull.Value)
                    {
                        //if (dtDonVi.Columns[j].ColumnName.IndexOf("_HienVat") >= 0)
                        //{
                        //    TongHienVat = TongHienVat + Convert.ToDouble(dtDonVi.Rows[i][j]);
                        //}
                        //else
                        //{
                            TongTuChi = TongTuChi + Convert.ToDouble(dtDonVi.Rows[i][j]);
                        //}
                    }
                }
                dtDonVi.Rows[i]["iID_MaDonVi"] = (i + 1).ToString();
                //dtDonVi.Rows[i]["TongHienVat"] = TongHienVat;
                dtDonVi.Rows[i]["TongTuChi"] = TongTuChi;
            }
            DataTable _dtDonVi = new DataTable();
            DataTable _dtDonVi1 = new DataTable();

            int TongSoCot = 0;
            int SoTrang = 1;
            int SoCotCanThem = 0;
            if ((dtDonVi.Columns.Count - 3) == 0)
            {
                SoCotCanThem = 5;
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
                int SoCotDu = (dtDonVi.Columns.Count - 3 - 5) % 6;
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
                    //else if (TenCot.IndexOf("_HienVat") >= 0) BangTien_HienVat = "Bằng hiện vật";
                    else BangTien_HienVat = "";
                    arrMoTa3.Add(BangTien_HienVat);

                    //Đổi tên cột
                    _dtDonVi.Columns[i].ColumnName = "Cot" + (i - 2);
                }
            }
            else
            {
                int tg = 5 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 2);
                int dem = 1;
                for (i = 5 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 2); i < 5 + SoCotTrang1 + SoCotTrangLonHon1 * (_ToSo - 1); i++)
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
                    //else if (TenCot.IndexOf("_HienVat") >= 0) BangTien_HienVat = "Bằng hiện vật";
                    else BangTien_HienVat = "";
                    arrMoTa3.Add(BangTien_HienVat);

                    //Đổi tên cột
                    _dtDonVi.Columns[i].ColumnName = "Cot" + dem;
                    dem++;
                }
            }

            var vm = new ReportDataModel();
            vm.dtDuLieu = _dtDonVi;
            vm.arrMoTa1 = arrMoTa1;
            vm.arrMoTa2 = arrMoTa2;
            vm.arrMoTa3 = arrMoTa3;
            vm.dtDuLieuAll = dtDonVi;
            vm.ColumnsCount = dtDonVi.Rows.Count == 0 ? 0 : TongSoCot + 2;
            return vm;
        }

        #endregion

    }
}
