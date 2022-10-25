using System;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using VIETTEL.Models;
using VIETTEL.Controllers;
using Viettel.Services;
using VIETTEL.Helpers;
using Viettel.Data;
using VIETTEL.Flexcel;
using Viettel.Extensions;

namespace VIETTEL.Models
{
    public class rptThuNop_01DTNViewModel
    {

        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
        public SelectList LoaiThoiGianList { get; set; }
        public SelectList DonViTinhList { get; set; }
        public SelectList LoaiBaoCaoList { get; set; }
        public SelectList iThoiGianList { get; set; }
        public string sMoTa { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.ThuNop
{
    public class rptThuNop_01DTNController : FlexcelReportController
    {
        // GET: /rptThuNop_01DTN/

        #region var def
        public string _viewPath = "~/Views/Report_Views/ThuNop/";
        private const string sFilePath = "~/Report_ExcelFrom/ThuNop/rptThuNop_01DTN";
        private const string sFilePath_B = "~/Report_ExcelFrom/ThuNop/rptThuNop_01DTN_B";
        private const string sFilePath1 = "~/Report_ExcelFrom/ThuNop/rptThuNop_01DTN_DonVi";
        private const string sFilePath1_B = "~/Report_ExcelFrom/ThuNop/rptThuNop_01DTN_DonVi_B";

        private readonly IThuNopService _thuNopService = ThuNopService.Default;
        private string _filePath;
        private string iID_MaPhongBan;
        private string iLoaiBaoCao;
        private string iID_MaDonVi;
        private string rSo;
        private string sLoaiThoiGian;
        private string iThoiGian;
        private string sMoTa;

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var dtDonVi = _thuNopService.GetDonViListByUser_PhongBan_TN("-1", Username, iNamLamViec);
            var dtPhongBan = SharedModels.getDSPhongBan(iNamLamViec, Username);
            var dtDvt = CurrencyExtension.getDSDonViTinh().Select("rSo = 1").CopyToDataTable();
            var dtLoaiThoiGian = DateTimeExtension.getDsLoaiThoigian();
            var dtIThoiGian = _thuNopService.getTime("0");
            var dtLoaiBaoCao = _thuNopService.getDSLoaiBaoCao();
            var sMoTa = _thuNopService.getSMoTaQD(this.ControllerName(), Username);

            var vm = new rptThuNop_01DTNViewModel
            {
                iNamLamViec = iNamLamViec,
                DonViList = dtDonVi.ToSelectList("iID_MaDonVi", "sTen", "-1", "-- Chọn đơn vị --"),
                PhongBanList = dtPhongBan.ToSelectList("iID_MaPhongBan", "sTenPhongBan", "-1", "-- Chọn phòng ban --"),
                LoaiBaoCaoList = dtLoaiBaoCao.ToSelectList("MaLoai", "sTen", 1),
                DonViTinhList = dtDvt.ToSelectList("rSo", "sTen", 1),
                LoaiThoiGianList = dtLoaiThoiGian.ToSelectList("MaLoaiTG", "TenLoaiTG", 0),
                iThoiGianList = dtIThoiGian.ToSelectList("MaTG", "TenTG", 0),
                sMoTa = sMoTa
            };

            return View(view, vm);
        }
        #endregion

        #region public methods

        public ActionResult Print(
            string iID_MaPhongBan,
            string iLoaiBaoCao,
            string iID_MaDonVi,
            string rSo,
            string sLoaiThoiGian,
            string iThoiGian,
            string sMoTa,
            string ext)
        {

            if (iID_MaPhongBan == "-1")
            {
                if (iLoaiBaoCao == "1")
                {
                    if (rSo == "1")
                    {
                        this._filePath = sFilePath + ".xls";
                    }
                    else if (rSo == "1000")
                    {
                        this._filePath = sFilePath + "_k.xls";
                    }
                    else if (rSo == "1000000")
                    {
                        this._filePath = sFilePath + "_m.xls";
                    }
                }
                else if (iLoaiBaoCao == "2")
                {
                    if (rSo == "1")
                    {
                        this._filePath = sFilePath1 + ".xls";
                    }
                    else if (rSo == "1000")
                    {
                        this._filePath = sFilePath1 + "_k.xls";
                    }
                    else if (rSo == "1000000")
                    {
                        this._filePath = sFilePath1 + "_m.xls";
                    }
                }
            }
            else
            {
                if (iLoaiBaoCao == "1")
                {
                    if (rSo == "1")
                    {
                        this._filePath = sFilePath_B + ".xls";
                    }
                    else if (rSo == "1000")
                    {
                        this._filePath = sFilePath_B + "_k.xls";
                    }
                    else if (rSo == "1000000")
                    {
                        this._filePath = sFilePath_B + "_m.xls";
                    }
                }
                else if (iLoaiBaoCao == "2")
                {
                    if (rSo == "1")
                    {
                        this._filePath = sFilePath1_B + ".xls";
                    }
                    else if (rSo == "1000")
                    {
                        this._filePath = sFilePath1_B + "_k.xls";
                    }
                    else if (rSo == "1000000")
                    {
                        this._filePath = sFilePath1_B + "_m.xls";
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
            this.iLoaiBaoCao = iLoaiBaoCao;
            this.iID_MaDonVi = iID_MaDonVi;
            this.rSo = rSo;
            this.sLoaiThoiGian = sLoaiThoiGian;
            this.iThoiGian = iThoiGian;
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
                    fr.SetValue("DVT", dtr["sTen"]);
                }                
            }

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName());

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
            DataTable data = rptThuNop_01DTN();
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtLoaiHinh = new DataTable();
            DataTable dtLoaiHinh1 = new DataTable();
            if (iLoaiBaoCao == "1")
            {
                dtLoaiHinh = HamChung.SelectDistinct("LoaiHinh", data, "iID_MaPhongBan,sKyHieuCap2,sKyHieuCap3", "iID_MaPhongBan,sKyHieuCap2,sTenLoaiHinhCap2,sKyHieuCap3,sTenLoaiHinhCap3");
                dtLoaiHinh1 = HamChung.SelectDistinct("LoaiHinh1", dtLoaiHinh, "iID_MaPhongBan,sKyHieuCap2", "iID_MaPhongBan,sKyHieuCap2,sTenLoaiHinhCap2");
            }
            else
            {
                dtLoaiHinh = HamChung.SelectDistinct("LoaiHinh", data, "iID_MaPhongBan,sKyHieuCap2,iID_MaDonVi", "iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sKyHieuCap2,sKyHieuCap3,sTenLoaiHinhCap2");
                dtLoaiHinh1 = HamChung.SelectDistinct("LoaiHinh1", dtLoaiHinh, "iID_MaPhongBan,iID_MaDonVi", "iID_MaPhongBan,iID_MaDonVi,sTenDonVi");
            }
            DataTable dtPhongBan = HamChung.SelectDistinct("PhongBan", dtLoaiHinh1, "iID_MaPhongBan", "iID_MaPhongBan");

            fr.AddTable("LoaiHinh", dtLoaiHinh);
            fr.AddTable("LoaiHinh1", dtLoaiHinh1);
            fr.AddTable("PhongBan", dtPhongBan);
            dtLoaiHinh.Dispose();
            dtLoaiHinh1.Dispose();
            data.Dispose();
            dtPhongBan.Dispose();
        }

        /// <summary>
        /// Lay da ta du lieu do vao bao cao 01DTN
        /// </summary>
        /// <returns></returns>
        public DataTable rptThuNop_01DTN()
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptThuNop_01DTN.sql");

            #endregion

            #region dieu kien

            decimal DVT = Convert.ToDecimal(rSo);
            if (iID_MaDonVi == "-1")
            {
                iID_MaDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(Username)
                            .AsEnumerable()
                            .Select(x => x.Field<string>("iID_MaDonVi"))
                            .Join();
            }
            String DKThoigian = _thuNopService.DKThoiGian(sLoaiThoiGian, iThoiGian);

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

        public JsonResult Ds_DonVi(String iID_MaPhongBan)
        {
            var data = _thuNopService.GetDonViListByUser_PhongBan_TN(iID_MaPhongBan, Username, PhienLamViec.iNamLamViec);

            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sTen", "-1", "-- Chọn đơn vị --"));
            return ToDropdownList(vm);
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

