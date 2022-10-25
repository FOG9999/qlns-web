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
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanBS.Controllers
{
    public class rptDuToanBS_InPhuLucTHViewModel
    {
        public SelectList DotList { get; set; }
        public SelectList PhongBanList { get; set; }
        public string TieuDe { get; set; }
        public string QuyetDinh { get; set; }
        public string GhiChu { get; set; }
    }

    public class rptDuToanBS_InPhuLucTHController : FlexcelReportController
    {
        #region ctor
        public string _viewPath = "~/Areas/DuToanBS/Views/Reports/InPhuLucTH/";
        private string _path = "~/Areas/DuToanBS";
        private string _filePath = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_Doc.xls";
        private string _filePath_ngang = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_Ngang_NSQP.xls";
        private string _filePath_ngang_nn = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_Ngang_NN.xls";
        private string _filePath_th = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_Doc_TH.xls";
        private string _filePath_th_a3 = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_TH_A3.xls";
        private string _filePath_th_a3_nv00 = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_TH_A3_nv00.xls";
        private string _filePath_th_a3_nv = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_TH_A3_nv.xls";

        private int _dvt = 1000;
        private string _dn = "30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94";
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _dutoanbsService = DuToanBsService.Default;

        public ActionResult Index()
        {
            var isTLTH = _nganSachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyB11 || _nganSachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyB2;
            var dtDot = _dutoanbsService.GetDotsTLTHCuc(PhienLamViec.iNamLamViec, null);

            var dtPhongBan = _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17");

            var dotList = dtDot
                .ToSelectList("iID_MaDot", "sMoTa", "-1", "-- Chọn đợt --");
            
            var vm = new rptDuToanBS_InPhuLucTHViewModel
            {
                DotList = dotList,
                PhongBanList = dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "---- Tất cả phòng ban ----"),
                TieuDe = "Giao dự toán ngân sách",
                QuyetDinh = $"số           /QĐ-BQP ngày     /     /{PhienLamViec.iNamLamViec} của Bộ trưởng Bộ Quốc phòng",
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
            string phongban,
            string tenPhuLuc = null,
            string quyetDinh = null,
            string ghiChu = null,
            int loaiBaoCao = 0,
            int dvt = 1000)
        {
            var path = getFileXls(loaiBaoCao);
            _dvt = dvt == 0 ? 1000 : dvt;

            var xls = createReport(Server.MapPath(path), iID_MaDot, dNgay, iID_MaDonVi, phongban, tenPhuLuc, quyetDinh, ghiChu, loaiBaoCao);

            var donvi = loaiBaoCao == 1 || loaiBaoCao == 2 || loaiBaoCao == 4 ? _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen : "";
            var filename = $"Phụ_lục_bổ_sung_{donvi}_dọc.{ext}";

            if (loaiBaoCao == 2)
            {
                filename = $"Phụ_lục_bổ_sung_{donvi}_ngang.{ext}";
            }
            else if (loaiBaoCao == 8)
            {
                filename = $"Phụ_lục_bổ_sung_{donvi}_ngân_sách_nhà_nước.{ext}";
            }
            else if (loaiBaoCao == 3)
            {
                filename = $"Phụ_lục_bổ_sung_tổng_hợp_dọc.{ext}";
            }
            else if (loaiBaoCao == 4 && !_dn.Contains(iID_MaDonVi))
            {
                filename = $"Phụ_lục_bổ_sung_{donvi}_DN_dọc.{ext}";
            }
            else if (loaiBaoCao == 5)
            {
                filename = $"Phụ lục tổng hợp đề nghị điều chỉnh dự toán chi ngân sách năm {PhienLamViec.iNamLamViec}.{ext}";
            }
            else if (loaiBaoCao == 6)
            {
                filename = $"Phụ lục tổng hợp đề nghị điều chỉnh dự toán chi ngân sách nghiệp vụ 00 năm {PhienLamViec.iNamLamViec}.{ext}";
            }
            else if (loaiBaoCao == 7)
            {
                filename = $"Phụ lục tổng hợp đề nghị điều chỉnh dự toán chi ngân sách nghiệp vụ năm {PhienLamViec.iNamLamViec}.{ext}";
            }
            return Print(xls, ext, filename);
        }


        public JsonResult Ds_DonVi(string iID_MaDot, int loaiBaoCao, string phongban)
        {
            iID_MaDot = _dutoanbsService.GetChungTus_GomTHCuc(iID_MaDot).Join();

            var data = _dutoanbsService.GetDonviDT_DNs(PhienLamViec.iNamLamViec, iID_MaDot, PhienLamViec.iID_MaDonVi, loaiBaoCao == 4 ? "06" : "07,08,10,17", phongban == "-1" ? "06,07,08,10,17" : phongban);

            var vm = new ChecklistModel("DonVi", data.ToSelectList());
            return ToCheckboxList(vm);            
        }        
        #endregion

        #region private methods

        private string getFileXls(int loaiBaoCao)
        {
            var path = _filePath;

            if (loaiBaoCao == 2)
            {
                path = _filePath_ngang;
            }
            else if (loaiBaoCao == 3)
            {
                path = _filePath_th;
            }
            else if (loaiBaoCao == 5)
            {
                path = _filePath_th_a3;
            }
            else if (loaiBaoCao == 6)
            {
                path = _filePath_th_a3_nv00;
            }
            else if (loaiBaoCao == 7)
            {
                path = _filePath_th_a3_nv;
            }
            else if (loaiBaoCao == 8)
            {
                path = _filePath_ngang_nn;
            }

            return path;
        }

        private ExcelFile createReport(string path,
            string iID_MaDot,
            string dNgay,
            string iID_MaDonVi,
            string phongban,
            string tenPhuLuc,
            string quyetDinh,
            string ghiChu,
            int loaiBaoCao)
        {
            var fr = new FlexCelReport();

            //Lấy dữ liệu chi tiết
            var ds = LoadData(fr, iID_MaDot, iID_MaDonVi, phongban, loaiBaoCao);

            String sTenDonVi = "";
            String sTenPB = "";            

            if (iID_MaDonVi == "-1")
            {
                sTenDonVi = "Tất cả các đơn vị ";
            }
            else
                sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen;
            
            var duoi = (loaiBaoCao == 4 && !_dn.Contains(iID_MaDonVi)) ? " (Khối doanh nghiệp)" : "";
            fr.SetValue("DotNgay", dNgay.ToParamDate().ToStringNgay().ToLower());
            fr.SetValue("sTenPB", sTenPB);
            fr.SetValue("sTenDonVi", sTenDonVi + duoi);
            fr.SetValue("TenPhuLuc", string.IsNullOrWhiteSpace(tenPhuLuc) ? "TenPhuLuc" : tenPhuLuc.ToUpper());
            fr.SetValue("QuyetDinh", string.IsNullOrWhiteSpace(quyetDinh) ? "QuyetDinh" : quyetDinh);

            #region ghi chus

            var noteBuilder = new StringBuilder();
            if (loaiBaoCao != 8 && loaiBaoCao != 2) { 
                var dtGhiChu = ds[0];
                var notes = dtGhiChu.AsEnumerable()
                    .Where(x => !string.IsNullOrWhiteSpace(x["sGhiChu"].ToString().Replace(" ", "")))
                    .Select(x => $"{x["sXauNoiMa"]} - {x["sMoTa"]} - {x["sGhiChu"]}"
//: {decimal.Parse(x["rTuChi"].ToString()).ToString("#,##0.00")} triệu đồng"
)
                    .ToList();
                if (notes.Count > 0)
                {                    notes.ForEach(x =>
                    {
                        if (!string.IsNullOrWhiteSpace(x)) noteBuilder.AppendLine("+ " + x);
                    });
                }
            }

            var Ten = $"ghichu_{this.ControllerName()}_{PhienLamViec.iNamLamViec}_{iID_MaDot}_{loaiBaoCao}_{iID_MaDonVi}";
            var list = _dutoanbsService.GetGhiChu(Username, Ten).Replace("\n","").Split('@');
            for (int i = 0; i < list.Count(); i++)
            {
                var text = list[i];
                if (!string.IsNullOrEmpty(list[i])) { 
                    noteBuilder.AppendLine(text);
                }
            }            

            fr.SetValue("GhiChu", noteBuilder.ToString());

            #endregion            

            var xls = new XlsFile(true);

            xls.Open(path);            

            fr
                .SetValue(new
                {
                    h1 = sTenDonVi,
                    h2 = $"Đơn vị tính: {_dvt.ToStringDvt()}"
                })
                .UseChuKy(Username)
                .UseCommonValue()
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);
            xls.Recalc(true);

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;
        }
        public JsonResult GhiChu(string id_donvi, string iID_MaDot, int loaiBaoCao)
        {
            var Ten = $"ghichu_{this.ControllerName()}_{PhienLamViec.iNamLamViec}_{iID_MaDot}_{loaiBaoCao}_{id_donvi}";
            var ghichu = _dutoanbsService.GetGhiChu(Username, Ten);
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GhiChu_Update(string id_donvi, string GhiChu, string iID_MaDot, int loaiBaoCao)
        {
            var Ten = $"ghichu_{this.ControllerName()}_{PhienLamViec.iNamLamViec}_{iID_MaDot}_{loaiBaoCao}_{id_donvi}";
            var success = _dutoanbsService.UpdateGhiChu(Username, Ten, GhiChu);
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        private DataTable getDataTable(string iID_MaDonVi, string iID_MaChungTu, string phongban, int loaiBaoCao)
        {
            #region sql

            var sql = string.Empty;
            var path_sql = _path + "/Sql/";

            if (loaiBaoCao == 2 || loaiBaoCao == 8)
            {
                path_sql += "dtbs_chitieu_thcuc_phuluc_ngang.sql";
            }
            else if (loaiBaoCao == 3)
            {
                path_sql += "dtbs_chitieu_thcuc_phuluc_doc_tonghop.sql";
            }
            else if (loaiBaoCao == 5)
            {
                path_sql += "dtbs_chitieu_thcuc_phuluc_ngang_tonghop.sql";
            }
            else if (loaiBaoCao == 6)
            {
                path_sql += "dtbs_chitieu_thcuc_phuluc_ngang_tonghop_nv00.sql";
            }
            else if (loaiBaoCao == 7)
            {
                path_sql += "dtbs_chitieu_thcuc_phuluc_ngang_tonghop_nv.sql";
            }
            else
            {
                path_sql += "dtbs_chitieu_thcuc_phuluc_doc.sql";
            }

            sql = FileHelpers.GetSqlQueryPath(path_sql);
            #endregion

            #region load data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@phongban", phongban.ToParamString());
                    //cmd.Parameters.AddWithValue("@pb", iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@dvt", _dvt);
                    if (loaiBaoCao == 1 || loaiBaoCao == 4 || loaiBaoCao == 3 || loaiBaoCao == 2 || loaiBaoCao == 8)
                        cmd.Parameters.AddWithValue("@loai", loaiBaoCao);

                    var dt = cmd.GetTable();
                    return dt;
                }

            }

            #endregion
        }

        private DataTable getDataTable_GhiChu(string iID_MaDonVi, string iID_MaPhongBan, string iID_MaChungTu)
        {
            var path_sql = _path + "/Sql/dtbs_chitieu_ghichu.sql";
            var sql = FileHelpers.GetSqlQueryPath(path_sql);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                // ghi chu
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd.Parameters.AddWithValue("@phongban", iID_MaPhongBan.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@dvt", 1000000);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }

        private IList<DataTable> LoadData(FlexCelReport fr, string iID_MaDot, string iID_MaDonVi, string phongban, int loaiBaoCao)
        {
            var isTLTH = _nganSachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop;
            var iID_MaChungTu = _dutoanbsService.GetChungTus_GomTHCuc(iID_MaDot).Join();

            var data = getDataTable(iID_MaDonVi, iID_MaChungTu, phongban, loaiBaoCao);
            var ghichu = getDataTable_GhiChu(iID_MaDonVi, loaiBaoCao == 4 ? "06" : "07,08,10,17", iID_MaChungTu);

            if (loaiBaoCao == 3)
            {
                fr.AddTable("DonVi", data);
            }
            else if (loaiBaoCao == 5 || loaiBaoCao == 6 || loaiBaoCao == 7)
            {
                var khoi = data.SelectDistinct("Khoi", "iID_MaPhongBan");
                khoi.AddOrdinalsNum(2);
                data.AddOrdinalsNum(3, "iID_MaPhongBan");
                fr.AddTable("DonVi", data);
                fr.AddTable("Khoi", khoi);
                fr.AddRelationship("Khoi", "DonVi", "iID_MaPhongBan", "iID_MaPhongBan");
            }
            else {
                FillDataTable_sTM(fr, data, PhienLamViec.iNamLamViec);        

                #region tong tien

                var totalSum = data.AsEnumerable().Sum(x => x.Field<decimal>(loaiBaoCao == 1 || loaiBaoCao == 4 ? "rTuChi" : "rTongSo")) * _dvt;
            
                //In loại tiền bằng chữ
                var bangChu = "";
                if (totalSum < 0)
                {
                    totalSum *= -1;
                    bangChu = "Giảm " + totalSum.ToStringMoney().ToLower();
                }
                else
                {
                    bangChu = totalSum.ToStringMoney();
                }

                fr.SetValue("Tien", bangChu);
                #endregion
            }

            return new List<DataTable> { ghichu };
        }
        
        #endregion

    }
}
