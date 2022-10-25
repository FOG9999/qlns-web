using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    //public class rptDuToanKT_M04dViewModel
    //{
    //    public SelectList PhongBanList { get; set; }

    //    public SelectList NganhList { get; set; }

    //}
}

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class rptDuToanKT_M04d2Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanKT/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04d2.xls";
        private string _filePath_to2 = "~/Report_ExcelFrom/DuToanKT/rptDuToanKT_M04d2_to2.xls";

        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _loaiBaoCao;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 12;

        public ActionResult Index()
        {
            var phongbanList = _ngansachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            //var nganhList = _duToanKTService.GetNganh(PhienLamViec.iNamLamViec, phongbanList.Rows[0].Field<string>("sKyHieu"), Username)
            //                                .ToSelectList("NG", "TenNganh", "-1", "-- chọn ngành --");


            var nganhList = _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username)
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
            string id_phongban,
            string id_nganh,
            int loaiBaoCao = 0,
            int to = 1,
            string ext = "pdf")
        {
            _id_phongban = GetPhongBanId(id_phongban);
            _id_nganh = id_nganh;
            _loaiBaoCao = loaiBaoCao;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;
            _filePath = _to == 1 ? _filePath : _filePath_to2;

            var xls = createReport();
            return Print(xls, ext);
        }

        public ActionResult Ds_Nganh(string id_phongban)
        {
            //var list = _duToanKTService.GetNganh(PhienLamViec.iNamLamViec, id_phongban, Username)
            //                              .ToSelectList("Ng", "TenNganh", "-1", " -- chọn ngành --");

            var isTongHop = _ngansachService.CheckParam_PhongBan(PhienLamViec.iID_MaPhongBan).IsEmpty();
            var list = _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec, isTongHop ? id_phongban : Username)
                                            .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");

            return ToDropdownList(new ChecklistModel("id_nganh", list));
        }

        public JsonResult Ds_To(string id_phongban, string id_nganh, int loaiBaoCao)
        {
            var data = getTable(PhienLamViec.iNamLamViec, id_phongban, id_nganh, loaiBaoCao, Username);
            var nganhs = data.SelectDistinct("nganh", "sNG");
            return ds_ToIn(nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + 1);
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            fr.SetValue(new
            {
                header1 = string.IsNullOrWhiteSpace(_id_nganh) || _id_nganh == "-1" ? string.Empty : "Ngành: " + _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec,_id_nganh).sTenNganh,
                header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}     Tờ: {_to}",
                TieuDe1 = getTieuDe(),
                TieuDe2 = _loaiBaoCao == 1 ? "Số đặc thù ngân sách sử dụng" : "Số đặc thù các ngành bảo đảm",
                Nam = PhienLamViec.iNamLamViec.ToValue<int>() + 1,
                TenPhongBan = string.IsNullOrWhiteSpace(_ngansachService.CheckParam_PhongBan(_id_phongban)) ? string.Empty : _ngansachService.GetPhongBanById(_id_phongban).sMoTa,
            });

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls);

            return xls;
        }


        private void loadData(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(),
                () => getTable(),
                CacheTimes.OneMinute);

            var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtX = dt.SelectDistinct("Nganh", "Nganh,TenNganh");


            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(double)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["TenNganh"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(r, 0);
            }


            columns = _to == 1 ?
                dtX.AsEnumerable().Skip(0).Take(_columnCount).ToList() :
                dtX.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                if (i < columns.Count)
                {

                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("TenNganh"));

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           var value = dt.AsEnumerable()
                                 .ToList()
                                 .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("Nganh")) || x.Field<string>("Nganh") == row.Field<string>("Nganh")) &&
                                              x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                 .Sum(x => x.Field<double>("DacThu", 0));
                           r[colName] = value;
                       });
                }
                else
                {
                    fr.SetValue($"C{i + 1}", "");
                }

            }
            fr.AddTable(data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private DataTable getTable()
        {
            return getTable(PhienLamViec.iNamLamViec, _id_phongban, _id_nganh, _loaiBaoCao, Username);
        }

        private DataTable getTable(string nam, string id_phongban, string id_nganh, int loaiBaoCao, string username)
        {
            string id_donvi = PhienLamViec.iID_MaDonVi;
            username = _ngansachService.CheckParam_Username(username, id_phongban);

            var sql = FileHelpers.GetSqlQuery("dtkt_report_m04d.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var nganh = id_nganh == "-1" ? "-1" : _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec,id_nganh).iID_MaNganhMLNS;

                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        id_phongban = id_phongban.ToParamString(),
                        username = username.ToParamString(),
                        nganh = nganh.ToParamString(),
                        id_donvi = id_donvi.ToParamString(),
                        loai = loaiBaoCao,
                        dvt = _dvt,
                    });

                return dt;
            }
        }


        private string getTieuDe()
        {
            var nam = int.Parse(PhienLamViec.iNamLamViec) + 1;
            var tieude = $"Biểu thống kê số đặc thù dự toán kiểm tra ngân sách năm {nam}";

            return tieude;
        }

        private string getCacheKey()
        {
            return $"{this.ControllerName()}_{Username}_{_nam}_{_id_phongban}_{_id_nganh}_{_loaiBaoCao}_{_dvt}";
        }

        #endregion
    }
}
