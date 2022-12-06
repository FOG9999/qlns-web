using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M04d1Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04d1.xls";
        private string _filePath_trinhky = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04d1_TrinhKy.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private string _id_nganh;
        private int _loaiBaoCao;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            var phongbanList = _ngansachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var nganhList = _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec,Username)
                                            .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");


            var vm = new rptDuToanKT_M04dViewModel
            {
                NganhList = nganhList,
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_nganh,
            int loaiBaoCao = 0,
            string ext = "pdf")
        {
            _id_nganh = id_nganh;
            _loaiBaoCao = loaiBaoCao;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _filePath = _loaiBaoCao == 1 ? _filePath : _filePath_trinhky;

            var xls = createReport();
            return Print(xls, ext);
        }

        public ActionResult Ds_Nganh(string id_phongban)
        {
            var isTongHop = _ngansachService.CheckParam_PhongBan(PhienLamViec.iID_MaPhongBan).IsEmpty();
            var list = _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec,isTongHop ? id_phongban : Username)
                                            .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");

            return ToDropdownList(new ChecklistModel("id_nganh", list));
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseCommonValue()
                .SetValue(new
                {
                    header1 = string.IsNullOrWhiteSpace(_id_nganh) || _id_nganh == "-1" ? "(Tất cả các ngành)" : "Ngành: " + _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec,_id_nganh).sTenNganh,
                    header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                    TieuDe1 = getTieuDe(),

                    QuyetDinh = L("DTKT_QuyetDinh"),
                    //TieuDe2 = _loaiBaoCao == 1 ? "Số đặc thù ngân sách sử dụng" : "Số đặc thù các ngành bảo đảm",
                    TieuDe2 = "Số đặc thù ngành bảo đảm",
                    Nam = PhienLamViec.iNamLamViec.ToValue<int>() + 1,
                })
                .UseChuKy(Username)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }


        private void loadData(FlexCelReport fr)
        {
            var data = CacheService.Default.CachePerRequest(getCacheKey(),
                () => getTable(),
                CacheTimes.OneMinute);
            data.TableName = "ChiTiet";

            //var data = dt.SelectDistinct("ChiTiet", "M,Tm,TTm,Ng,MoTa");
            var dtNganh = data.SelectDistinct("dtNganh", "Ng,TenNganh");


            fr.AddTable(data)
                .AddTable(dtNganh);
            fr.AddRelationship(dtNganh.TableName, data.TableName, "Ng", "Ng");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private DataTable getTable()
        {
            return getTable(PhienLamViec.iNamLamViec, _id_nganh);
        }

        private DataTable getTable(string nam, string id_nganh)
        {
            var sql = FileHelpers.GetSqlQuery("dtkt_report_m04d_chitiet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var nganh = id_nganh == "-1" ? "-1" : _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec,id_nganh).iID_MaNganhMLNS;

                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        //id_phongban = id_phongban.ToParamString(),
                        nganh = nganh.ToParamString(),
                        dvt = _dvt,
                    });

                return dt;
            }
        }


        private string getTieuDe()
        {
            var TieuDe1 = L("DTKT_TieuDe1", PhienLamViec.iNamLamViec.ToValue<int>() + 1);
            return TieuDe1;
        }

        private string getCacheKey()
        {
            return $"{this.ControllerName()}_{Username}_{_nam}_{_id_nganh}_{_dvt}";
        }

        #endregion
    }
}
