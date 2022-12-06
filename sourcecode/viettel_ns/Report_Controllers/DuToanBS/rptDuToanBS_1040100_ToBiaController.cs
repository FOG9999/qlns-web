using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptDuToanBS_1040100_ToBiaViewModel
    {
        public string iNamLamViec { get; set; }
        public string iID_MaDot { get; set; }
        public SelectList DotList { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToanBS_1040100_ToBiaController : FlexcelReportController
    {
        #region ctor

        private const string _viewPath = "~/Views/Report_Views/DuToanBS/";
        private const string _filePath = "~/Report_ExcelFrom/DuToanBS/rptDuToanBS_1040100_Tobia.xls";
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _bsService = DuToanBsService.Default;

        public ActionResult Index()
        {

            var iNamLamViec = PhienLamViec.iNamLamViec;

            //var dtDot = CacheService.Default.CachePerRequest(this.ControllerName() + "_Dot",
            //    () => DuToanBS_ReportModels.LayDSDot(iNamLamViec, Username, "1040100"),
            //    Viettel.Domain.DomainModel.CacheTimes.OneMinute);


            var dotList = DuToanBsService.Default.GetDots_NSBD(iNamLamViec, Username)
                .AsEnumerable()
                .Select(x => new
                {
                    iID_MaDot = x.Field<string>("iID_MaDot"),
                    sMoTa = x.Field<string>("iID_MaDot") + " - " + _bsService.GetDot_MoTa(PhienLamViec.iNamLamViec, Username, PhienLamViec.iID_MaPhongBan, x.Field<string>("iID_MaDot"), "104,109")
                })
                .ToDictionary(x => x.iID_MaDot, x => x.sMoTa)
                .ToSelectList("-1", "-- Chọn đợt --");

            //var dtPhongBan = DuToanBS_ReportModels.LayDSPhongBan(iNamLamViec, Username);

            var vm = new rptDuToanBS_1040100_ToBiaViewModel
            {
                iNamLamViec = iNamLamViec,
                DotList = dotList,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region reports

        public ActionResult Print(
            string iID_MaDot,
            string iID_MaNganh,
            string ext)
        {
            var iNamLamViec = ReportModels.LayNamLamViec(Username);
            var xls = createReport(iNamLamViec, iID_MaDot, iID_MaNganh);
            return Print(xls, ext);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Lấy danh sách Ngành dựa vào đợt, phòng ban, 
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="iID_MaDot">Mã Đợt</param>
        /// <param name="iID_MaDonVi">Mã Đơn Vị</param>
        /// <returns></returns>
        public JsonResult Ds_Nganh(string iID_MaDot)
        {
            var vm = ChecklistModel.Default;

            try
            {
                var data = DuToanBsService.Default.GetNganh_NSBD(PhienLamViec.iNamLamViec, iID_MaDot, Username);
                vm = new ChecklistModel("Nganh", data.ToSelectList("iID_MaNganh", "sTenNganh"));

            }
            catch (System.Exception ex)
            {
            }

            return ToCheckboxList(vm);
        }

        #endregion

        #region private methods

        private ExcelFile createReport(
            string iNamLamViec,
            string iID_MaDot,
            string iID_MaNganh)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));
            var fr = new FlexCelReport();

            //var data = CacheService.Default.CachePerRequest(this.ControllerName(),
            //    () => getDataTable(iNamLamViec, iID_MaDot, iID_MaNganh), Viettel.Domain.DomainModel.CacheTimes.OneMinute); ;

            var data = getDataTable(iNamLamViec, iID_MaDot, iID_MaNganh);
            FillDataTableLNS(fr, data, FillLNSType.LNS, iNamLamViec);

            string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
            SqlCommand cmdNganh = new SqlCommand(sql);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            var tenNganh = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);

            fr.SetExpression("NgayDot", iID_MaDot.ToParamDate().ToStringNgay().ToLower());
            fr.SetValue("TenNganh", tenNganh);


            fr.UseCommonValue(
            //    new Application.Flexcel.FlexcelModel
            //{
            //    Thang = "Ngày ...... tháng 12 năm 2020",
            //    Ngay = "Ngày ...... tháng ...... năm 2020",
            //}
                )
              .UseChuKy(Username, PhienLamViec.iID_MaPhongBan)
              .UseChuKyForController(this.ControllerName(), PhienLamViec.iID_MaPhongBan)
              .UseForm(this)
              .Run(xls);

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;

        }

        private DataTable getDataTable(
          string iNamLamViec,
          string iID_MaDot,
          string iID_MaNganh)
        {
            #region sql

            //            var sql = @"

            //SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            //        rTuChi      =SUM(rTuChi)/@dvt,
            //        rHangNhap   =SUM(rHangNhap)/@dvt,
            //        rHangMua    =SUM(rHangMua)/@dvt,
            //        rTonKho     =SUM(rTonKho)/@dvt,
            //        rPhanCap    =SUM(rPhanCap)/@dvt,
            //        rDuPhong    =SUM(rDuPhong)/@dvt 
            //FROM    DTBS_ChungTuChiTiet 
            //WHERE   iTrangThai=1  
            //        --AND sLNS='1040100'  
            //        AND LEFT(sLNS,3) IN (104,109)
            //        AND (MaLoai='' OR MaLoai='2') 
            //        AND iNamLamViec=@iNamLamViec
            //        AND sNG IN (SELECT * FROM F_Split((SELECT TOP(1) iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh WHERE iID_MaNganh=@iID_MaNganh)))
            //        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))

            //GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
            //HAVING  SUM(rTuChi) <>0
            //        OR SUM(rHangMua) <>0
            //        OR SUM(rHangNhap) <>0
            //        OR SUM(rTonkho) <>0
            //        OR SUM(rPhanCap) <>0
            //        OR SUM(rDuPhong) <>0

            //";

            #endregion

            var sql = FileHelpers.GetSqlQuery("dtbs_tobia_104.sql");

            #region  dieukien

            //var iID_MaChungTu = DuToanBS_ReportModels.GetChungTuList_TheoDot(iID_MaDot, Username);
            var iID_MaChungTu = _bsService.GetChungTus_DotNgay_Nam(iID_MaDot, Username, PhienLamViec.iID_MaPhongBan, PhienLamViec.iNamLamViec).Join();

            #endregion

            #region load data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@iID_MaNganh", iID_MaNganh);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", PhienLamViec.iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@dMaDot", iID_MaDot.ToParamDate());
                    cmd.Parameters.AddWithValue("@dvt", 1000);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }

            #endregion
        }
        #endregion
    }
}
