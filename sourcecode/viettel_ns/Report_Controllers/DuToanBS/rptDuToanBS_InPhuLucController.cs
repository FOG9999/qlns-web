using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{

    public class rptDuToanBS_InPhuLucController : FlexcelReportController
    {
        #region ctor

        private const string _viewPath = "~/Views/Report_Views/DuToanBS/";
        
        private const string _filePath_ngang = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ChiTieuNganSach_Ngang.xls";
        private const string _filePath_104 = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ChiTieuNganSach_104.xls";

        private const string _filePath_trinhky = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ChiTieuNganSach_TrinhKy.xls";
        private const string _filePath_ngang_trinhky = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ChiTieuNganSach_TrinhKy.xls";
        private const string _filePath_104_trinhky = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_ChiTieuNganSach_104_TrinhKy.xls";

        private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_PhuLuc.xls";
        private const string _filePath_nsbd = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_PhuLuc_NSBD.xls";

        private int _dvt = 1000;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _dutoanbsService = DuToanBsService.Default;

        public ActionResult Index(int trinhKy = 0)
        {
            var _namLamViec = PhienLamViec.iNamLamViec;

            var dtPhongBan = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            
            var isTLTH = _nganSachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop;
            var dtDot = isTLTH ?
                _dutoanbsService.GetDots_Gom(_namLamViec, Username) :
                _dutoanbsService.GetDotNhaps(_namLamViec, Username);

            var dotList = dtDot
                .ToSelectList("iID_MaDot", "sMoTa", "-1", "-- Chọn đợt --");

            var vm = new rptDuToanBS_ChiTieuNganSachViewModel
            {
                iNamLamViec = _namLamViec,
                PhongBanList = dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa"),
                DotList = dotList,
                TieuDe = "Giao dự toán ngân sách",
                QuyetDinh = $"số           /QĐ-BQP ngày      tháng      năm {PhienLamViec.iNamLamViec} của Bộ trưởng Bộ Quốc phòng",
                TrinhKy = trinhKy,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iID_MaDot,
            string dNgay,
            string iID_MaDonVi,
            string iID_MaPhongBan,
            string tenPhuLuc = null,
            string quyetDinh = null,
            string ghiChu = null,
            int loaiBaoCao = 0,
            int dvt = 1000)
        {
            var path = getFileXls(loaiBaoCao);
            _dvt = dvt == 0 ? 1000 : dvt;

            var xls = createReport(Server.MapPath(path), iID_MaDot, dNgay, iID_MaDonVi, iID_MaPhongBan, tenPhuLuc, quyetDinh, ghiChu, loaiBaoCao);
            return Print(xls, ext);
        }


        public JsonResult Ds_DonVi(string iID_MaDot, string iID_MaPhongBan)
        {           

            var isTLTH = _nganSachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop;
            if (isTLTH)
            {
                iID_MaDot = _dutoanbsService.GetChungTus_Gom(iID_MaDot).Join();
            }           

            var data = _dutoanbsService.GetDonviTheoChungTus(PhienLamViec.iNamLamViec, iID_MaDot, PhienLamViec.iID_MaDonVi, iID_MaPhongBan);
            var vm = new ChecklistModel("DonVi", data.ToSelectList());
            return ToCheckboxList(vm);
        }

        public JsonResult Ds_LNS(
            string iID_MaDot,
            string iID_MaPhongBan,
            string iID_MaDonVi)
        {
            var data = DuToanBS_ReportModels.dtLNS_Dot(iID_MaDot, iID_MaPhongBan, iID_MaDonVi, Username);
            var vm = new ChecklistModel("LNS", data.ToSelectList("sLNS", "TenHT"));

            return ToCheckboxList(vm);
        }
        #endregion

        #region private methods

        private string getFileXls(int loaiBaoCao)
        {
            var path = _filePath;
            // 104 - 7 cột, ngang
            if (loaiBaoCao == 104 || loaiBaoCao == 207 || loaiBaoCao == 2070)
            {
                path = _filePath_nsbd;
            }
            // 2 cột, dọc
            else
            {
                path = _filePath;
            }

            return path;
        }

        private ExcelFile createReport(string path,
            string iID_MaDot,
            string dNgay,
            string iID_MaDonVi,
            string iID_MaPhongBan,
            string tenPhuLuc,
            string quyetDinh,
            string ghiChu,
            int loaiBaoCao)
        {
            var fr = new FlexCelReport();

            //Lấy dữ liệu chi tiết
            var ds = LoadData(fr, iID_MaDot, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao);

            String sTenDonVi = "";
            String sTenPB = "";

            if (iID_MaPhongBan == "-1")
            {
                sTenPB = "Tất cả các phòng ban ";
            }
            else
                sTenPB = "B " + iID_MaPhongBan;

            if (iID_MaDonVi == "-1")
            {
                sTenDonVi = "Tất cả các đơn vị ";
            }
            else
                sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen;

            fr.SetValue("DotNgay", dNgay.ToParamDate().ToStringNgay().ToLower());
            fr.SetValue("sTenPB", sTenPB);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("TenPhuLuc", string.IsNullOrWhiteSpace(tenPhuLuc) ? "TenPhuLuc" : tenPhuLuc.ToUpper());
            fr.SetValue("QuyetDinh", string.IsNullOrWhiteSpace(quyetDinh) ? "QuyetDinh" : quyetDinh);

            #region ghi chus

            var noteBuilder = new StringBuilder();
            var dtGhiChu = ds[1];
            var notes = dtGhiChu.AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x["sGhiChu"].ToString().Replace(" ", "")))
                .Select(x => $"{x["sXauNoiMa"]} - {x["sMoTa"]} - {x["sGhiChu"]}: {double.Parse(x["rTuChi"].ToString()).ToString("###.###")}")
                .ToList();
            if (notes.Count > 0)
            {
                notes.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x)) noteBuilder.AppendLine(x);
                });
            }

            if (!string.IsNullOrWhiteSpace(ghiChu))
                noteBuilder.AppendLine(ghiChu);

            fr.SetValue("GhiChu", noteBuilder.ToString());

            #endregion

            var sTenPhongBan = NganSachService.Default.GetPhongBanById(iID_MaPhongBan).sMoTa;
            fr.SetValue("sTenPhongBan", sTenPhongBan);

            var xls = new XlsFile(true);

            xls.Open(path);

            fr
                .SetValue(new
                {
                    h1 = sTenDonVi,
                    h2 = $"Đơn vị tính: {_dvt.ToStringDvt()}"
                })
                .UseChuKy(Username, iID_MaPhongBan)
                .UseCommonValue()
                .UseChuKyForController(this.ControllerName(), iID_MaPhongBan)
                .UseForm(this)
                .Run(xls);
            xls.Recalc(true);

            return xls;
        }

        private DataTable getDataTable(string iID_MaPhongBan, string iID_MaDonVi, string iID_MaChungTu, int loaiBaoCao)
        {
            #region sql

            var sql = string.Empty;
            var sLNS = string.Empty;
                       
            if (loaiBaoCao == 1)
            {
                // bieu doc
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_phuluc_doc.sql");
            }
            else if (loaiBaoCao == 2)
            {
                // bieu doc
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_phancap.sql");
            }
            else if (loaiBaoCao == 3)
            {
                // bieu doc
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_phuluc_tuchi2.sql");
            }

            if (loaiBaoCao == 7)
            {
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_b7.sql");
            }
            else if (loaiBaoCao == 104)
            {
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_phuluc.sql");
                sLNS = "104,109";
            }
            else if (loaiBaoCao == 207)
            {
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_phuluc.sql");
                sLNS = "2,3";
            }
            else if (loaiBaoCao == 2070)
            {
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_phuluc_tuchi.sql");
                sLNS = "2,3";
            }
            #endregion

            #region load data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@sLNS", sLNS.ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", _dvt);

                    var dt = cmd.GetTable();
                    return dt;
                }

            }

            #endregion
        }

        private DataTable getDataTable_GhiChu(string iID_MaDonVi, string iID_MaPhongBan, string iID_MaChungTu)
        {
            var sql = FileHelpers.GetSqlQuery("dtbs_chitieu_ghichu.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                // ghi chu
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@dvt", _dvt);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }

        private IList<DataTable> LoadData(FlexCelReport fr, string iID_MaDot, string iID_MaPhongBan, string iID_MaDonVi, int loaiBaoCao)
        {
            var isTLTH = _nganSachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop;
            var iID_MaChungTu = isTLTH ?
                _dutoanbsService.GetChungTus_Gom(iID_MaDot).Join() :
                iID_MaDot;

            var data = getDataTable(iID_MaPhongBan, iID_MaDonVi, iID_MaChungTu, loaiBaoCao);
            var ghichu = getDataTable_GhiChu(iID_MaPhongBan, iID_MaDonVi, iID_MaChungTu);
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);


            // them 5 dong trang neu qua it row trong bang.
            var count = data.Rows.Count < 5 ? 5 : 0;
            var dtTrang = new DataTable();
            dtTrang.Columns.Add("sTen", typeof(String));
            for (int i = 0; i < count; i++)
            {
                var dr = dtTrang.NewRow();
                dtTrang.Rows.Add(dr);
            }
            fr.AddTable("dtTrang", dtTrang);


            #region tong tien

            //long TongTien = 0;
            //for (int i = 0; i < data.Rows.Count; i++)
            //{
            //    if (data.Rows[i]["rTongSo"].ToString() != "")
            //    {
            //        TongTien += Convert.ToInt64(data.Rows[i]["rTongSo"]);
            //    }
            //}

            var totalSum = data.AsEnumerable().Sum(x => x.Field<decimal>("rTongSo")) * _dvt;


            //In loại tiền bằng chữ
            var bangChu = "";
            if (totalSum < 0)
            {
                totalSum *= -1;
                bangChu = "Giảm " + totalSum.ToStringMoney().ToLower();
                //bangChu = "Giảm " + CommonFunction.TienRaChu(TongTien).ToLower();
            }
            else
            {
                bangChu = totalSum.ToStringMoney();
            }

            fr.SetValue("Tien", bangChu);

            #endregion

            return new List<DataTable> { data, ghichu };
        }

        #endregion

    }
}
