using FlexCel.Core;
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

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_KhoBacViewModel
    {
        public SelectList DonViList { get; set; }
    }
    public class rptDuToan_KhoBacController : FlexcelReportController
    {
        #region ctor
        public string _viewPath = "~/Areas/DuToan/Views/Report/KhoBac/";
        private string _path = "~/Areas/DuToan";
        private string _filePath = "~/Areas/DuToan/FlexcelForm/DonVi/KhoBac/rptDuToan_KhoBac.xls";

        private int _dvt = 1000;
        private string _xLNS = "", _xDonVi = "";
        private string _iID_MaDonVi;
        private string _dn = "30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94";
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanService _dutoanService = DuToanService.Default;

        public ActionResult Index()
        {
            if (PhienLamViec.iID_MaPhongBan == "02") { 
                var donviList = _dutoanService.Get_DonViDT_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, null, PhienLamViec.iID_MaPhongBan != "02" ? PhienLamViec.iID_MaPhongBan : null);

                var vm = new rptDuToan_KhoBacViewModel()
                {
                    DonViList = donviList.ToSelectList("Id", "Ten"),
                };

                var view = _viewPath + this.ControllerName() + ".cshtml";
                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string id_donvi)
        {
            DataTable dtCauHinh = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);

            _xLNS = dtCauHinh.Rows[0]["xLNS"].ToString();
            _xDonVi = dtCauHinh.Rows[0]["xDonVi"].ToString();
            _iID_MaDonVi = id_donvi;

            var xls = createReport(Server.MapPath(_filePath));

            var donvi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;
            var filename = $"Phụ_lục_bổ_sung_{donvi}_dọc.{ext}";    
                   
            return Print(xls, ext, filename);
        }            
        #endregion

        #region private methods
        private ExcelFile createReport(string path)
        {
            var fr = new FlexCelReport();

            //Lấy dữ liệu chi tiết
            LoadData(fr);

            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);

            String sTenDonVi = "";
            
            sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _iID_MaDonVi).sTen;
                        
            fr.SetValue("sTenDonVi", sTenDonVi);
            
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

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;
        }        
        private DataTable getDataTable()
        {
            #region sql

            var sql = string.Empty;
            var path_sql = _path + "/Sql/dt_chitieu_donvi_khobac.sql";
            
            sql = FileHelpers.GetSqlQueryPath(path_sql);

            sql = sql.Replace("@@xsLNS", _xLNS);
            sql = sql.Replace("@@xDonVi", _xDonVi);
            #endregion

            #region load data
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam = PhienLamViec.iNamLamViec,
                        dvt = 1000,
                        iID_MaDonVi = _iID_MaDonVi,
                    });
                dt.NullToZero("rTuChi,rChiTapTrung".Split(','));
                
                return dt;
            }
            #endregion
        }        

        private void LoadData(FlexCelReport fr)
        {
            var data = getDataTable();
            var dtsLNS = data.SelectDistinct("dtsLNS", "sLNS,sMoTa");
            var dtsM = data.SelectDistinct("dtsM", "sLNS,sM");
            fr.AddTable("ChiTiet", data);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsM", dtsM);
            _nganSachService.AddMoTaM(dtsM, PhienLamViec.iNamLamViec);
            data.Columns.Remove("sMoTa");
            _nganSachService.AddMoTaM(data, PhienLamViec.iNamLamViec);
            fr.AddRelationship("dtsLNS", "dtsM", "sLNS", "sLNS");
            fr.AddRelationship("dtsM", "ChiTiet", "sLNS,sM".Split(','), "sLNS,sM".Split(','));
            dtsM.Dispose();
            dtsLNS.Dispose();
            data.Dispose();
        }

        #endregion

    }
}
