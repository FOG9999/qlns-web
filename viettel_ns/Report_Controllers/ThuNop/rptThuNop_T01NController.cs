using System;
using System.Linq;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using VIETTEL.Models;
using VIETTEL.Controllers;
using Viettel.Extensions;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using Viettel.Data;

namespace VIETTEL.Models
{
    public class rptThuNop_T01NViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList ToSoList { get; set; }
        public SelectList DonViTinhList { get; set; }
        public SelectList LoaiThoiGianList { get; set; }
        public SelectList iThoiGianList { get; set; }
        public SelectList MaChuKyList { get; set; }
        public string sMoTa { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.ThuNop
{
    public class rptThuNop_T01NController : FlexcelReportController
    {
        // GET: /rptThuNop_T01N/

        #region var def
        public string _viewPath = "~/Views/Report_Views/ThuNop/";
        private const String sFilePath = "~/Report_ExcelFrom/ThuNop/rptThuNop_T01N";
        private const String sFilePath_To2 = "~/Report_ExcelFrom/ThuNop/rptThuNop_T01N_To2";
        private const String sFilePath_B = "~/Report_ExcelFrom/ThuNop/rptThuNop_T01N_B";
        private const String sFilePath_B_To2 = "~/Report_ExcelFrom/ThuNop/rptThuNop_T01N_B_To2";
        private const String sFilePathTH = "~/Report_ExcelFrom/ThuNop/rptThuNop_T01NTH";

        private readonly IThuNopService _thuNopService = ThuNopService.Default;
        private string _filePath;
        private string iID_MaPhongBan;
        private string tSo;
        private string rSo;
        private string sLoaiThoiGian;
        private string iThoiGian;
        private string ChuKy;
        private string sMoTa; 

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var dtPhongBan = SharedModels.getDSPhongBan(iNamLamViec, Username);
            var dtTSo = _thuNopService.getDSTO();
            var dtDvt = CurrencyExtension.getDSDonViTinh().Select("rSo = 1").CopyToDataTable();
            var dtLoaiThoiGian = DateTimeExtension.getDsLoaiThoigian();
            var dtIThoiGian = _thuNopService.getTime("0");
            var MaChuKy = SharedModels.getDsLoaiChuKy();
            var sMoTa = _thuNopService.getSMoTaQD(this.ControllerName(), Username);

            var vm = new rptThuNop_T01NViewModel
            {
                iNamLamViec = iNamLamViec,
                PhongBanList = dtPhongBan.ToSelectList("iID_MaPhongBan", "sTenPhongBan", "-1", "-- Chọn phòng ban --"),
                ToSoList = dtTSo.ToSelectList("MaLoai", "sTen"),
                DonViTinhList = dtDvt.ToSelectList("rSo", "sTen", 1),
                LoaiThoiGianList = dtLoaiThoiGian.ToSelectList("MaLoaiTG", "TenLoaiTG", 0),
                iThoiGianList = dtIThoiGian.ToSelectList("MaTG", "TenTG", 0),
                MaChuKyList = MaChuKy.ToSelectList("MaLoaiCK", "TenLoaiCK"),
                sMoTa = sMoTa
            };

            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string iID_MaPhongBan,
            string tSo,
            string rSo,
            string sLoaiThoiGian,
            string iThoiGian,
            string sMoTa,
            string MaChuKy,
            string ext)
        {

            if (ext == "xlsth")
            {
                ext = "xls";
                this._filePath = sFilePathTH + ".xls";
            }
            else
            {
                if (iID_MaPhongBan == "-1")
                {
                    if (tSo == "1")
                    {
                        if (rSo == "1")
                        {
                            _filePath = sFilePath + ".xls";
                        }
                        else if (rSo == "1000")
                        {
                            _filePath = sFilePath + "_k.xls";
                        }
                        else if (rSo == "1000000")
                        {
                            _filePath = sFilePath + "_m.xls";
                        }
                    }
                    else if (tSo == "2")
                    {
                        if (rSo == "1")
                        {
                            _filePath = sFilePath_To2 + ".xls";
                        }
                        else if (rSo == "1000")
                        {
                            _filePath = sFilePath_To2 + "_k.xls";
                        }
                        else if (rSo == "1000000")
                        {
                            _filePath = sFilePath_To2 + "_m.xls";
                        }
                    }

                }
                else
                {
                    if (tSo == "1")
                    {
                        if (rSo == "1")
                        {
                            _filePath = sFilePath_B + ".xls";
                        }
                        else if (rSo == "1000")
                        {
                            _filePath = sFilePath_B + "_k.xls";
                        }
                        else if (rSo == "1000000")
                        {
                            _filePath = sFilePath_B + "_m.xls";
                        }
                    }
                    else if (tSo == "2")
                    {
                        if (rSo == "1")
                        {
                            _filePath = sFilePath_B_To2 + ".xls";
                        }
                        else if (rSo == "1000")
                        {
                            _filePath = sFilePath_B_To2 + "_k.xls";
                        }
                        else if (rSo == "1000000")
                        {
                            _filePath = sFilePath_B_To2 + "_m.xls";
                        }
                    }
                }
            }
            if (PhienLamViec.iID_MaPhongBan == "02" || PhienLamViec.iID_MaPhongBan == "11")
            {
                this.iID_MaPhongBan = iID_MaPhongBan;
            }
            else
            {
                this.iID_MaPhongBan = PhienLamViec.iID_MaPhongBan;
            }
            this.tSo = tSo;
            this.rSo = rSo;
            this.sLoaiThoiGian = sLoaiThoiGian;
            this.iThoiGian = iThoiGian;
            this.ChuKy = MaChuKy;
            this.sMoTa = sMoTa;
            _thuNopService.updateSMoTaQD(this.ControllerName(), Username, sMoTa);

            var xls = createReport();
            return Print(xls, ext);
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        public ExcelFile createReport()
        {
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);

            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, Username));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, Username));
            fr.SetValue("ChuKy", ChuKy);

            if (iID_MaPhongBan != "-1")
                fr.SetValue("TenPB", " B - " + iID_MaPhongBan);

            String ThoiGian = _thuNopService.getTNTieuDe(sLoaiThoiGian, iThoiGian, PhienLamViec.iNamLamViec);

            if (sLoaiThoiGian == "0")
            {
                fr.SetValue("Tit01", ThoiGian);
                fr.SetValue("Tit02", "(" + sMoTa + ")");
                fr.SetValue("Tit03", "");
            }
            else
            {
                fr.SetValue("Tit01", "");
                fr.SetValue("Tit02", ThoiGian);
                fr.SetValue("Tit03", "(" + sMoTa + ")");
            }

            if (iID_MaPhongBan == "02")
            {
                fr.SetValue("NgayThang", "");
            }
            else
            {
                fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            }

            DataTable dvt = CurrencyExtension.getDSDonViTinh();
            foreach (DataRow dtr in dvt.Rows)
            {
                if (Convert.ToString(dtr["rSo"]) == rSo)
                {
                    if (tSo == "1")
                        fr.SetValue("DVT", dtr["sTen"]);
                    else                    
                        fr.SetValue("DVT", "Mẫu số: T01/N - " + dtr["sTen"]);                    
                } 
            }

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(this._filePath));

            fr.Run(xls);

            return xls;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        private void LoadData(FlexCelReport fr)
        {
            DataTable dataDN = rptThuNop_T01N("DN");
            ReportModels.numberCount(dataDN);
            dataDN.TableName = "ChiTietDN";
            fr.AddTable("ChiTietDN", dataDN);
            DataTable dtPhongBanDN = HamChung.SelectDistinct("PhongBanDN", dataDN, "iID_MaPhongBan", "iID_MaPhongBan");
            fr.AddTable("PhongBanDN", dtPhongBanDN);

            DataTable dataDT = rptThuNop_T01N("DT");
            ReportModels.numberCount(dataDT);
            dataDN.TableName = "ChiTietDT";
            fr.AddTable("ChiTietDT", dataDT);
            DataTable dtPhongBanDT = HamChung.SelectDistinct("PhongBanDT", dataDT, "iID_MaPhongBan", "iID_MaPhongBan");
            fr.AddTable("PhongBanDT", dtPhongBanDT);

            dtPhongBanDT.Dispose();
            dataDT.Dispose();
            dtPhongBanDN.Dispose();
            dataDN.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public DataTable rptThuNop_T01N(String loaiDvi)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptThuNop_T01N.sql");

            #endregion

            #region dieu kien

            decimal DVT = Convert.ToDecimal(rSo);
            string iID_MaDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(Username)
                                .AsEnumerable()
                                .Select(x => x.Field<string>("iID_MaDonVi"))
                                .Join();

            var DKThoigian = _thuNopService.DKThoiGian(sLoaiThoiGian, iThoiGian);

            if (loaiDvi == "DT")
            {
                sql = sql.Replace("@@DS_Dvi", "TN_GetDviDToan(@username)");
            }
            else if (loaiDvi == "DN")
            {
                sql = sql.Replace("@@DS_Dvi", "TN_GetDviDNghiep(@username)");
            }
            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.Trim().ToParamString());
                cmd.Parameters.AddWithValue("@iThang_Quy", DKThoigian.ToParamString());
                cmd.Parameters.AddWithValue("@dvt", DVT);
                cmd.Parameters.AddWithValue("@username", Username);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion   
        }

        public JsonResult Ds_ThoiGian(String sLoaiThoiGian)
        {

            var data = _thuNopService.getTime(sLoaiThoiGian);
            var vm = new ChecklistModel(null, null);
            var i = DateTime.Now.Month;

            switch (sLoaiThoiGian)
            {
                case ("0"):
                    vm = new ChecklistModel("iThoiGian", data.ToSelectList("MaTG", "TenTG", 2));
                    break;
                case ("1"):
                    vm = new ChecklistModel("iThoiGian", data.ToSelectList("MaTG", "TenTG", (i / 3) + 1));
                    break;
                case ("2"):
                    vm = new ChecklistModel("iThoiGian", data.ToSelectList("MaTG", "TenTG", data.Rows[i]));
                    break;
                default:
                    break;
            }
            return ToDropdownList(vm);
        }

        #endregion
    }
}

