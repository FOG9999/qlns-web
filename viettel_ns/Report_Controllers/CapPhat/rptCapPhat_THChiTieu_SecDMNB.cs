using System.Web.Mvc;
using System;
using VIETTEL.Models;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using FlexCel.Report;
using System.Data;
using VIETTEL.Models.CapPhat;
using VIETTEL.Controllers;
using Viettel.Services;
using VIETTEL.Flexcel;

namespace VIETTEL.Models
{
    public class rptCapPhat_THChiTieu_SecDMNBViewModel
    {
        public string iNamLamViec { get; set; }
        public ChecklistModel NganhList { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.CapPhat
{
    public class rptCapPhat_THChiTieu_SecDMNBController : FlexcelReportController
    {

        #region var def
        public string _viewPath = "~/Views/Report_Views/CapPhat/";
        private const String sFilePath_ChiTieu_SecDMNB = "/Report_ExcelFrom/CapPhat/rptCapPhat_THChiTieu_SecDMNB.xls";

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly ICapPhatService _capPhatService = CapPhatService.Default;

        private string khoangcachdong;
        private string _filePath;
        private string nganh;

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var dtNganh = _capPhatService.getDanhSachNganh(iNamLamViec, Username).SelectDistinct("Nganh", "iID_MaNganh,sTenNganh");

            var vm = new rptCapPhat_THChiTieu_SecDMNBViewModel
            {
                iNamLamViec = iNamLamViec,
                NganhList = new ChecklistModel("Nganh", dtNganh.ToSelectList("iID_MaNganh", "sTenNganh"))
            };

            return View(view, vm);
        }
        #endregion

        #region public methods
        public ActionResult Print(
            string Nganh,
            string khoangCachDong,
            string ext)
        {

            this.nganh = Nganh;
            this.khoangcachdong = khoangCachDong;
            this._filePath = sFilePath_ChiTieu_SecDMNB;

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

            dtDonVi = _capPhatService.CapPhatTongHopChiTieuSec(PhienLamViec.iNamLamViec, Username, nganh);
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
