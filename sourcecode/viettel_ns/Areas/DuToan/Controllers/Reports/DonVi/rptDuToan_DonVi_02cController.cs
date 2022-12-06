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
using VIETTEL.Areas.DuToan.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_DonVi_02cController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToan/DonVi/rptDuToan_DonVi_02b.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _loaiBaoCao;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 8;

        public ActionResult Index()
        {
            var phongbanList = _ngansachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var nganhList = _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username)
                                            .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");

            var vm = new rptDuToan_K11_NG_THViewModel
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

            var xls = createReport();
            return Print(xls, ext);
        }

        public ActionResult Ds_Nganh(string id_phongban)
        {
            var isTongHop = _ngansachService.CheckParam_PhongBan(PhienLamViec.iID_MaPhongBan).IsEmpty();
            var list = _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec, isTongHop ? id_phongban : Username)
                                            .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");

            return ToDropdownList(new ChecklistModel("id_nganh", list));
        }

        public JsonResult Ds_To(string id_phongban, string id_nganh, int loaiBaoCao)
        {

            var data = getTable(PhienLamViec.iNamLamViec, id_phongban, id_nganh, loaiBaoCao, Username);
            var nganhs = data.SelectDistinct("nganh", "Nganh,sNG,Loai");
            var nganhsTong = data.SelectDistinct("nganh", "Nganh,Loai");

            var count = nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + nganhsTong.Rows.Count + 2;
            return ds_ToIn(count, _columnCount);
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var tenNganh = string.IsNullOrWhiteSpace(_id_nganh) || _id_nganh == "-1" ? string.Empty : "Ngành: " + _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;
            fr.SetValue(new
            {
                header1 = tenNganh,
                header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                TieuDe1 = $"Tổng hợp dự toán ngân sách năm {PhienLamViec.iNamLamViec}",
                TenPhongBan = string.IsNullOrWhiteSpace(_ngansachService.CheckParam_PhongBan(_id_phongban)) ? string.Empty : _ngansachService.GetPhongBanById(_id_phongban).sMoTa,
            });

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);

            // merge cot tong cong
            if (_to == 1)
            {
                xls.MergeCells(6, 4, 7, 5);
                xls.MergeH(6, 6, 7);
                xls.MergeH(7, 6, 7);
            }
            else
            {
                xls.MergeH(6, 4, 9);
                xls.MergeH(7, 4, 9);
            }


            return xls;
        }


        private void loadData(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(),
                () => getTable(),
                CacheTimes.OneMinute,
                false);

            var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtX = dt.SelectDistinct("Nganh", "sLNS,sNG,Nganh,TenNganh,sXauNoiMa,Loai");
            var dtNganh = dt.SelectDistinct("Nganh", "sNG,TenNganh,Loai");

            // them cot tong cong cua tung Nganh
            var rowsCountX = dtNganh.Rows.Count;
            for (int i = 0; i < rowsCountX; i++)
            {
                var nrow = dtX.NewRow();
                nrow["sNG"] = dtNganh.Rows[i]["sNG"];
                nrow["TenNganh"] = dtNganh.Rows[i]["TenNganh"];
                nrow["Loai"] = dtNganh.Rows[i]["Loai"];
                dtX.Rows.Add(nrow);
            }

            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var row_tuchi = dtX.NewRow();
                row_tuchi["TenNganh"] = "Tổng cộng".ToUpper();
                row_tuchi["Loai"] = 1;
                dtX.Rows.InsertAt(row_tuchi, 0);

                var row_hienvat = dtX.NewRow();
                row_hienvat["TenNganh"] = "Tổng cộng".ToUpper();
                row_hienvat["Loai"] = 2;
                dtX.Rows.InsertAt(row_hienvat, 0);

            }

            var dtXList = dtX.AsEnumerable().OrderBy(x => x.Field<string>("sNG"))
                .ThenBy(x => x.Field<string>("Nganh"))
                .ThenBy(x => x.Field<int>("Loai"));

            columns = _to == 1 ?
                dtXList.AsEnumerable().Take(_columnCount).ToList() :
                dtXList.AsEnumerable().Skip((_to - 1) * _columnCount - 2).Take(_columnCount).ToList();

            var mlns = _ngansachService.MLNS_GetAll(PhienLamViec.iNamLamViec);

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colNameMuc = $"D{i + 1}";
                var colIndex = $"e{i + 1}";
                if (i < columns.Count)
                {

                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("TenNganh"));

                    var sNG = row.Field<string>("Nganh");
                    if (!string.IsNullOrWhiteSpace(sNG))
                    {
                        sNG = sNG + Environment.NewLine + Environment.NewLine + mlns.FirstOrDefault(x => x.sXauNoiMa == row.Field<string>("sXauNoiMa")).sMoTa;
                    }
                    else
                    {
                        sNG = Environment.NewLine + Environment.NewLine + "(+)";
                    }
                    fr.SetValue(colNameMuc, sNG);
                    fr.SetValue(colIndex, row.Field<int>("Loai") == 1 ? "Bằng tiền" : "Hiện vật");

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           var value = dt.AsEnumerable()
                                 .ToList()
                                 .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("sNG")) || (x.Field<string>("sNG") == row.Field<string>("sNG") && (string.IsNullOrWhiteSpace(row.Field<string>("sXauNoiMa")) || x.Field<string>("sXauNoiMa") == row.Field<string>("sXauNoiMa")))) &&
                                            x.Field<int>("Loai") == row.Field<int>("Loai") &&
                                            x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                 .Sum(x => x.Field<decimal>("TuChi", 0));
                           r[colName] = value;
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colNameMuc, "");
                    fr.SetValue(colIndex, "");
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

            var sql = FileHelpers.GetSqlQuery("rptDuToan_DonVi_02b.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var nganh = id_nganh == "-1" ? "-1" : _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).iID_MaNganhMLNS;

                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        id_phongban = id_phongban.ToParamString(),
                        username = username.ToParamString(),
                        nganh = nganh.ToParamString(),
                        id_donvi = id_donvi.ToParamString(),
                        loai = loaiBaoCao.ToParamString(),
                        dvt = _dvt,
                    });

                return dt;
            }
        }

        private DataTable getMucMoTa(string nam)
        {
            var sql = @"


select distinct sM, sMoTa from NS_MucLucNganSach
where iTrangThai=1
        and iNamLamViec=dbo.f_ns_nammlns(@NamLamViec)
        and sTM='' and sLNS <>'' and sM<>''

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        NamLamViec = nam,
                    });

                return dt;
            }

        }

        private DataTable getMoTa_Nganh(string nam)
        {
            var sql = @"


select distinct sM, sMoTa from NS_MucLucNganSach
where iTrangThai=1
        and iNamLamViec=dbo.f_ns_nammlns(@NamLamViec)
        and sTM='' and sLNS <>'' and sM<>''

";
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        NamLamViec = nam,
                    });

                return dt;
            }
        }
        private string getCacheKey()
        {
            return $"{this.ControllerName()}_{Username}_{_nam}_{_id_phongban}_{_id_nganh}_{_loaiBaoCao}_{_dvt}";
        }

        #endregion
    }

}
