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
    public class rptDuToan_KhoBac_TachBController : FlexcelReportController
    {
        #region ctor
        private string _path = "~/Areas/DuToan";
        private string _filePath = "~/Areas/DuToan/FlexcelForm/Cuc/KhoBac/rptDuToan_KhoBac.xls";

        private int _dvt = 1000;
        private string _xLNS = "", _xDonVi = "";
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanService _dutoanService = DuToanService.Default;

        #endregion

        #region public methods

        public ActionResult Print(
            string ext)
        {
            if (PhienLamViec.iID_MaPhongBan == "02")
            {
                DataTable dtCauHinh = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);

                    _xLNS = dtCauHinh.Rows[0]["xLNS"].ToString();
                    _xDonVi = dtCauHinh.Rows[0]["xDonVi"].ToString();

                    var xls = createReport(Server.MapPath(_filePath));

                    var filename = $"Phụ_lục_kho_bạc_tách_bql.{ext}";    
                   
                    return Print(xls, ext, filename);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
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

            String sTenDonVi = "BỘ QUỐC PHÒNG";            
                        
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
            var path_sql = _path + "/Sql/dt_chitieu_tonghop_khobac.sql";
            
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
                    });
                dt.NullToZero("rB5,rB7,rB8,rB10".Split(','));
                
                return dt;
            }
            #endregion
        }        

        private void LoadData(FlexCelReport fr)
        {
            var data = getDataTable();          
            fr.AddTable("ChiTiet", data);
            data.Dispose();
        }

        #endregion

    }
}
