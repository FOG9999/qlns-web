using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Application.Flexcel;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers
{
    public class rptQuyetToanNam_2A2Controller : FlexcelReportController
    {
        private readonly IQuyetToanService _qtService = QuyetToanService.Default;


        #region ctors

        private const string _viewPath = "~/Views/Report_Views/QuyetToan/";

        private const string _filePath_TongHop_Lns =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_2A2_TongHop_LNS.xls";

        private const string _filePath_TongHop_DonVi =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_2A2_TongHop_DonVi.xls";

        private const string _filePath_TongHop_DonVi_RutGon =
            "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_2A2_TongHop_DonVi_RutGon.xls";

        private const string _filePath_DonVi = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_2A2_DonVi.xls";
        private const string _filePath = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_2A2.xls";

        private string _namLamViec = DateTime.Now.Year.ToString();
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private string _loaiBaoCao;
        private int _sOrL;
        private DateTime _date;
        private int _kieuBaoCao;

        public ActionResult Index()
        {
            _namLamViec = ReportModels.LayNamLamViec(Username);

            var dtPhongBan = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var namNganSachList = new List<dynamic>
            {
                new {value = 0, text = "Tổng hợp"},
                new {value = 2, text = "Quyết toán ngân sách năm nay"},
                new {value = 1, text = "Quyết toán ngân sách năm trước chuyển sang"}
            };


            var vm = new rptQuyetToanNam_2A_ViewModel
            {
                iNamLamViec = _namLamViec,
                PhongBanList = GetPhongBanId(PhienLamViec.iID_MaPhongBan).IsEmpty() ?
                    dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "<-- Tổng hợp cục -->") :
                    dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa"),
                NamNganSachList = namNganSachList.ToSelectList("value", "text")
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods


        private DataTable getDataTable(
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string iID_MaNamNganSach)
        {
            #region sql

            var sql = "";

            var namTuChi = string.Empty;
            var namDaCapTien = string.Empty;
            var namChuaCapTien = string.Empty;

            //if (_loaiBaoCao == "2")
            //{
            //    if (iID_MaNamNganSach == "2")
            //    {
            //        sql = FileHelpers.GetSqlQuery("qt_nam_2a2_ng.sql");
            //        namTuChi = "2,4";
            //        namDaCapTien = "1";
            //        namChuaCapTien = "4";
            //    }
            //    else if (iID_MaNamNganSach == "1")
            //    {
            //        sql = FileHelpers.GetSqlQuery("qt_nam_2a1_ng.sql");
            //        namTuChi = "1,5";
            //        namDaCapTien = "5";
            //        namChuaCapTien = "0";
            //    }
            //    else
            //    {
            //        sql = FileHelpers.GetSqlQuery("qt_nam_2a_ng.sql");
            //        namTuChi = "1,2,3,4,5";
            //        namDaCapTien = "1,5";
            //        namChuaCapTien = "4";
            //    }
            //}
            //else
            //{
            //    if (iID_MaNamNganSach == "2")
            //    {
            //        sql = FileHelpers.GetSqlQuery("qt_nam_2a2.sql");
            //    }
            //    else if (iID_MaNamNganSach == "1")
            //    {
            //        sql = FileHelpers.GetSqlQuery("qt_nam_2a1.sql");
            //    }
            //    else
            //    {
            //        sql = FileHelpers.GetSqlQuery("qt_nam_2a.sql");
            //    }
            //}

            if (iID_MaNamNganSach == "2")
            {
                sql = FileHelpers.GetSqlQuery("qt_nam_2a2_ng.sql");
                namTuChi = "2,4";
                namDaCapTien = "1";
                namChuaCapTien = "4";
            }
            else if (iID_MaNamNganSach == "1")
            {
                sql = FileHelpers.GetSqlQuery("qt_nam_2a1_ng.sql");
                namTuChi = "1,5";
                namDaCapTien = "5";
                namChuaCapTien = "0";
            }
            else
            {
                sql = FileHelpers.GetSqlQuery("qt_nam_2a_ng.sql");
                namTuChi = "1,2,3,4,5";
                namDaCapTien = "1,5";
                namChuaCapTien = "4";
            }

            //sql = _loaiBaoCao == "2" ?
            //    FileHelpers.GetSqlQuery("qt_nam_2a_ng.sql") :
            //    FileHelpers.GetSqlQuery("qt_nam_2a.sql");


            sql = FileHelpers.GetSqlQuery("qt_nam_2a_ng_vuotdutoan.sql");

            #endregion

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("iID_MaNamNganSach",
                    string.IsNullOrWhiteSpace(iID_MaNamNganSach) || iID_MaNamNganSach == "0" ? "1,2" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("iNamLamViecNS", Convert.ToInt32(PhienLamViec.iNamLamViec) + 1);
                cmd.Parameters.AddWithValue("iID_MaDonVi", iID_MaDonVi.ToParamString());
                cmd.Parameters.AddWithValue("namTuChi", namTuChi.ToParamString());
                cmd.Parameters.AddWithValue("namDaCapTien", namDaCapTien.ToParamString());
                cmd.Parameters.AddWithValue("namChuaCapTien", namChuaCapTien.ToParamString());

                cmd.Parameters.AddWithValue("sOrL", _sOrL);

                var dt = cmd.GetTable();
                return dt;
            }
        }


        /// <summary>
        ///     Tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private ExcelFile createReport(
            string path,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string iID_MaNamNganSach,
            string loaiBaoCao)
        {
            var fr = new FlexCelReport();

            var data = getDataTable(iID_MaPhongBan, iID_MaDonVi, iID_MaNamNganSach);

            if (loaiBaoCao == "2")
            {
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM,sNG", PhienLamViec.iNamLamViec);
            }
            if (_sOrL == 1)
            {
                FillDataTable(fr, data, "sLNS1", PhienLamViec.iNamLamViec);
            }
            else
            {
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS iID_MaDonVi,sTenDonVi", PhienLamViec.iNamLamViec);
            }

            #region report values

            //lay ten nam ngan sach
            var NamNganSach = "Phần tổng hợp";
            if (iID_MaNamNganSach == "1")
                NamNganSach = GetPhongBanId(iID_MaPhongBan).IsEmpty()
                    ? "Phần quyết toán năm trước"
                    : "Ngân sách năm trước chuyển sang";
            else if (iID_MaNamNganSach == "2")
                NamNganSach = GetPhongBanId(iID_MaPhongBan).IsEmpty() ? "Phần quyết toán năm nay" : "Ngân sách năm nay";

            #region ten mau bieu

            var tenBieu = "";
            if (loaiBaoCao == "9")
            {
                tenBieu = "TQT-2C" + (iID_MaNamNganSach == "0" ? "" : iID_MaNamNganSach);
            }
            else
            {
                if (iID_MaPhongBan == "-1")
                {
                    tenBieu = "TQT-2A" + (iID_MaNamNganSach == "0" ? "" : iID_MaNamNganSach);
                }
                else
                {
                    tenBieu = "TQT-2B" + (iID_MaNamNganSach == "0" ? "" : iID_MaNamNganSach);
                }
            }

            //if (iID_MaDonVi == "-1")
            //{
            //    if (iID_MaPhongBan == "-1")
            //        tenBieu = "Tổng hợp cục - Biểu 2A";
            //    else tenBieu = "Tổng hợp B - Biểu 2A";
            //}
            //else
            //{
            //    tenBieu = "Chi tiết - Biểu 2";
            //}

            #endregion

            var sTenDonVi = "";
            if (iID_MaDonVi != "-1" && loaiBaoCao == "9")
            {
                sTenDonVi = "Đơn vị: " + NganSachService.Default.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sMoTa;
            }


            //Ten BQL
            fr.SetValue("NamNganSach", NamNganSach);
            fr.SetValue("BQuanLy",
                GetPhongBanId(iID_MaPhongBan).IsEmpty() ? "" : _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("TenBieu", tenBieu);
            fr.SetValue("NgayThang2a", "Ngày " + _date.Day + " tháng " + _date.Month + " năm " + _date.Year);
            // che cho mr.chuc
            fr.SetValue("Nam", int.Parse(PhienLamViec.iNamLamViec));
            fr.SetValue("sOrL", _sOrL);
            fr.SetValue("type", _kieuBaoCao);

            #endregion

            var xls = new XlsFile(true);
            xls.Open(path);

            fr.UseCommonValue(new FlexcelModel
            {
                header2 = $"Đơn vị tính: đồng - Trang: "
            }, 1)
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        public DataTable DanhSach_DonVi_QuyetToan_PhongBan(string iID_MaPhongBan, string iID_MaNamNganSach, string MaND)
        {
            var sql = FileHelpers.GetSqlQuery("qt_donvi_vuotdutoan.sql");
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach",
                    iID_MaNamNganSach == "0" || string.IsNullOrWhiteSpace(iID_MaNamNganSach) ? "1,2" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", PhienLamViec.DonViList.Select(x => x.Key).Join());

                return cmd.GetTable();
            }
        }


        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iID_MaPhongBan,
            string iID_MaNamNganSach,
            string iID_MaDonVi,
            string loaiBaoCao,
            int kieuBaoCao,
            string date,
            int sOrL)
        {
            _loaiBaoCao = loaiBaoCao;
            _kieuBaoCao = kieuBaoCao;
            _sOrL = sOrL;
            _date = Convert.ToDateTime(date);

            var filePath = getXlsFile(loaiBaoCao);
            var xls = createReport(filePath, iID_MaPhongBan, iID_MaDonVi, iID_MaNamNganSach, loaiBaoCao);

            return Print(xls, ext);
        }

        private string getXlsFile(string loaiBaoCao)
        {
            var path = string.Empty;

            if (_sOrL == 1)
            {
                path = _filePath_TongHop_DonVi_RutGon;
            }
            else if (loaiBaoCao == "9")
            {
                path = _filePath_TongHop_Lns;
                //path = _filePath_DonVi;
            }
            else if (loaiBaoCao == "0")
            {
                path = _filePath_TongHop_DonVi;
            }
            else if (loaiBaoCao == "1")
            {
                path = _filePath_TongHop_Lns;
            }
            else
            {
                path = _filePath;
            }
            return Server.MapPath(path);
        }

        public JsonResult Ds_DonVi(string iID_MaPhongBan, string iID_MaNamNganSach)
        {
            var data = _qtService.GetDonVis(PhienLamViec.iNamLamViec, Username, iID_MaNamNganSach, iID_MaPhongBan,
                PhienLamViec.DonViList.Select(x => x.Key).Join());

            var donvis = new Dictionary<string, string>();
            PhienLamViec.DonViList.ToList()
                .ForEach(x =>
                {
                    var tenDonVi = x.Value;

                    var row = data.AsEnumerable().FirstOrDefault(r => r.Field<string>("iID_MaDonVi") == x.Key);

                    if (row == null)
                    {
                        //tenDonVi = x.Value;
                        tenDonVi = $"{x.Value} (N)";
                    }
                    else
                    {
                        donvis.Add(x.Key, tenDonVi);
                    }

                });

            //var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "sTenDonVi"));
            var vm = new ChecklistModel("DonVi", donvis.ToSelectList());

            return ToCheckboxList(vm);
        }

        #endregion
    }
}