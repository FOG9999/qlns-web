using System.Web.Mvc;
using System;
using VIETTEL.Models;
using FlexCel.Core;
using FlexCel.Render;
using System.IO;
using FlexCel.XlsAdapter;
using FlexCel.Report;
using System.Data;
using VIETTEL.Models.CapPhat;
using VIETTEL.Controllers;
using Viettel.Services;
using VIETTEL.Flexcel;

namespace VIETTEL.Models
{
    public class rptCapPhat_THChiTieuViewModel
    {

        public string iNamLamViec { get; set; }
        public string khoangCachDong { get; set; }
        public SelectList loaiNSList { get; set; }

    }
}

namespace VIETTEL.Report_Controllers.CapPhat
{
    public class rptCapPhat_THChiTieuController : FlexcelReportController
    {

        #region var def
        public string _viewPath = "~/Views/Report_Views/CapPhat/";
        private const String sFilePath_ChiTieu = "/Report_ExcelFrom/CapPhat/rptCapPhat_THChiTieu.xls";

        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string khoangcachdong;
        private string _filePath;
        private string loaiNS;

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var dtLoaiNS = CapPhat_ReportModels.getLoaiNS();

            var vm = new rptCapPhat_THChiTieuViewModel
            {
                iNamLamViec = iNamLamViec,
                loaiNSList = dtLoaiNS.ToSelectList("MaLoai", "sTen", 0),
                khoangCachDong = "120"
            };

            return View(view, vm);
        }
        #endregion

        #region public methods
        public ActionResult Print(
            string loaiNS,
            string khoangCachDong,
            string ext)
        {

            this.loaiNS = loaiNS;
            this.khoangcachdong = khoangCachDong;
            this._filePath = sFilePath_ChiTieu;

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
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName());

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(this._filePath));

            fr.Run(xls);

            return xls;
        }

        #endregion

        #region private methods
        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        private void LoadData(FlexCelReport fr)
        {
            DataTable data = new DataTable();
            DataTable dtDonVi = new DataTable();

            dtDonVi = CapPhat_ReportModels.rptCapPhat_THChiTieu(Username, loaiNS);
            data = HamChung.SelectDistinct("ChiTiet", dtDonVi, "iID_MaDonVi,sTenDonVi", "iID_MaDonVi,sTenDonVi,sMoTa");

            fr.AddTable("dtDonVi", dtDonVi);
            dtDonVi.Dispose();

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            fr.SetExpression("RowH", "<#Row height(Autofit;" + khoangcachdong + ")>");
            data.Dispose();
        }

        #endregion        

    }
}
