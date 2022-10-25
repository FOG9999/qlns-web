﻿using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{

    public class rptNC_NganhViewModel : ReportModels
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }

    public class rptNC_NganhController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptNC_Nganh.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban;
        private string id_nganh;
        private string loaiBaoCao;
        private int loai;
        private int dvt;

        public ActionResult Index()
        {

            //var phongbanList = _nganSachService.Nganh_GetAll(Username, PhienLamViec.iID_MaPhongBan).ToSelectList("iID_MaNganh", "sTenNganh", "-1", "<- TẤT CẢ CÁC NGÀNH ->");
            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa");
            var nganhList = _nganSachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username, id_phongban).ToSelectList("iID_MaNganh", "sTenNganh");

            var vm = new rptNC_NganhViewModel()
            {
                PhongBanList = phongbanList,
                NganhList = nganhList,
            };

            return View(@"~/Areas\SKT\Views\Reports\BaoCao\rptNC_Nganh.cshtml", vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="loai">1: NSSD, 2: NSBĐ ngành</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(string id_phongban, string id_nganh, string loaiBaoCao = "chitiet", string ext = "pdf", int dvt = 1000)
        {

            this.id_phongban = id_phongban;
            this.id_nganh = id_nganh;
            this.loaiBaoCao = loaiBaoCao;
            this.dvt = dvt;

            var xls = createReport();


            //loaiBaoCao = id_nganh.ToList().Count > 0 || id_phongban.ToList().Count > 0 ? "tonghop" : "chitiet";

            var nganh = id_nganh.ToList().Count > 1
              ? "(Tổng hợp các ngành)"
              : _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).sTenNganh;

            var filename = $"BC_NhuCau_NSBĐ_{(nganh.IsEmpty() ? "TongHop" : id_nganh + "-" + nganh.ToStringAccent().Replace(" ", ""))}_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls, ext, filename);
        }

        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            var nganh = id_nganh.ToList().Count > 1
                        ? "(Tổng hợp các ngành)"
                        : _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).sTenNganh;

            fr.SetValue(new
            {
                h1 = $"Đơn vị tính: {dvt.ToStringNumber()}đ",
                h2 = nganh,
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.ToStringNgay(),
                NamTruoc = int.Parse(PhienLamViec.iNamLamViec) - 2,
                NamNay = int.Parse(PhienLamViec.iNamLamViec) - 1,

                Cap1 = "Cục Tài chính",
                Cap2 = id_phongban.IsEmpty() ? "" : _nganSachService.GetPhongBanById(id_phongban).sMoTa,
                DonVi = nganh.IsEmpty() ? "(Tổng hợp ngành)" : $"Ngành: {nganh}",
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getTable();
            var ghiChu = getTable_GhiChu(id_phongban, id_nganh);
            var dtNganhGhiChu = ghiChu.SelectDistinct("Nganh", "Nganh,sMoTa");
            _SKTService.FillDataTable_NC(fr, data);

            fr.SetValue("ghichu", loaiBaoCao == "chitiet" ? 1 : 0);
            
            fr.AddTable("dtGhiChu", ghiChu);
            fr.AddTable("Nganh", dtNganhGhiChu);
            fr.AddRelationship("Nganh", "dtGhiChu", "Nganh".Split(','), "Nganh".Split(','));

        }

        private DataTable getTable()
        {
            var sql = FileHelpers.GetSqlQuery("rpt_nc_nganh.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = id_phongban.ToParamString(),
                    id_donvi = id_nganh.ToParamString(),
                    dvt,
                });
                return dt;
            }
        }

        private DataTable getTable_GhiChu(string id_phongban, string id_nganh)
        {
            var sql = FileHelpers.GetSqlQuery("rpt_nc_donvi_ghichu.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                    id_phongban = id_phongban.ToParamString(),
                    id_donvi = id_nganh.ToParamString(),
                    loai = 2,
                });
                return dt;
            }
        }


        private string getTieuDe()
        {
            var tieude = $"Báo cáo chi tiết dự toán nhu cầu ngân sách năm {PhienLamViec.iNamLamViec} - Ngành bảo đảm";

            return tieude;
        }

        #endregion
    }
}
