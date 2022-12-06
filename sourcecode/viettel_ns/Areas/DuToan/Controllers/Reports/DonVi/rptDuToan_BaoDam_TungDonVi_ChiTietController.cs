using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    public class rptDuToan_BaoDam_TungDonVi_ChiTietController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToan/DonVi/rptDuToan_BaoDam_TungDonVi_ChiTiet.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _loaiBaoCao;
        private int _to = 1;
        private int _nganhCount = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 9;

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
            var count = xls.TotalPageCount();
            if (_to != 1)
            {
                if (count > 1)
                {
                    xls.ClearDiffFirstPage();
                }
            }
            else
            {
                if (count > 1)
                {
                    xls.AddPageFirstPage();
                }
            }
            var duoi = loaiBaoCao == 1 ? "Phụ lục 1b" : "Phụ lục 1c";
            var tenNganh = "Ngành: " + _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;
            var filename = $"{tenNganh}_{duoi}_Tờ số {_to}.{ext}";
            return Print(xls, ext, filename);
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
            var nganhsTong = data.SelectDistinct("nganh", "sNG,Loai");

            var count = nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + (nganhsTong.Rows.Count > 1 ? nganhsTong.Rows.Count : 0) + 1;
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
                TieuDe2 = _loaiBaoCao == -1 ? "Tổng hợp số phân cấp" : (_loaiBaoCao == 1 ? "Phần các đơn vị cấp 2,3 chi tiết số Bộ thông báo" : "Phần số đặc thù ngành phân cấp"),
                soPL = _loaiBaoCao == 1 ? "1b" : "1c",
                TenPhongBan = string.IsNullOrWhiteSpace(_ngansachService.CheckParam_PhongBan(_id_phongban)) ? string.Empty : _ngansachService.GetPhongBanById(_id_phongban).sMoTa,
            });

            DataTable dtCauHinhBia = DuToan_ChungTuModels.GetCauHinhBia(User.Identity.Name);
            string sSoQuyetDinh = "-1";
            if (dtCauHinhBia.Rows.Count > 0)
            {
                sSoQuyetDinh = dtCauHinhBia.Rows[0]["sSoQuyetDinh"].ToString();
            }
            fr.SetValue("sSoQUyetDinh", sSoQuyetDinh);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);

            if (_to == 1 && _nganhCount > 1)
            {
                xls.MergeCells(6, 4, 7, 4);
                xls.MergeH(6, 5, 8);
            }
            else
            {
                xls.MergeH(6, 4, 9);
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

            _nganhCount = dtNganh.Rows.Count;

            // them cot tong cong cua tung Nganh
            for (int i = 0; i < _nganhCount; i++)
            {
                var ng = dtNganh.Rows[i]["sNG"].ToString();
                var ngcount = dtX.Select($"sNG = '{ng}'").Count();
                if (ngcount > 1)
                {
                    var nrow = dtX.NewRow();
                    nrow["sNG"] = dtNganh.Rows[i]["sNG"];
                    nrow["TenNganh"] = dtNganh.Rows[i]["TenNganh"];
                    nrow["Loai"] = dtNganh.Rows[i]["Loai"];
                    dtX.Rows.Add(nrow);
                }
            }

            var colNames = "";
            // them cot
            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
                if (i == 0)
                {
                    colNames += $"C{i + 1}";
                }
                else
                {
                    colNames += $",C{i + 1}";
                }
            }

            var columns = new List<DataRow>();
            if (_nganhCount > 1)
            {
                var row_tuchi = dtX.NewRow();
                row_tuchi["TenNganh"] = "Tổng cộng".ToUpper();
                row_tuchi["Loai"] = 1;
                dtX.Rows.InsertAt(row_tuchi, 0);
            }

            var dtXList = dtX.AsEnumerable().OrderBy(x => x.Field<string>("sNG"))
                .ThenBy(x => x.Field<string>("Nganh"))
                .ThenBy(x => x.Field<int>("Loai"));

            columns = _to == 1 ?
                dtXList.AsEnumerable().Take(_columnCount).ToList() :
                dtXList.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();

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
                        sNG = sNG + Environment.NewLine + Environment.NewLine + Environment.NewLine + mlns.FirstOrDefault(x => x.sXauNoiMa == row.Field<string>("sXauNoiMa")).sMoTa;
                    }
                    else
                    {
                        sNG = Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "(+)";
                    }
                    fr.SetValue(colNameMuc, sNG);
                    fr.SetValue(colIndex, row.Field<int>("Loai") == 1 ? "Bằng tiền" : "Hiện vật");

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           var value = dt.AsEnumerable()
                                 .ToList()
                                 .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("sNG"))
                                                    || (x.Field<string>("sNG") == row.Field<string>("sNG") &&
                                                    (string.IsNullOrWhiteSpace(row.Field<string>("sXauNoiMa"))
                                                    || x.Field<string>("sXauNoiMa") == row.Field<string>("sXauNoiMa")))) &&
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

            data.NullToZero(colNames.Split(','));

            for (int i = data.Rows.Count - 1; i >= 0; i--)
            {
                decimal count = 0;
                var dtr = data.Rows[i];

                for (int j = 2; j < data.Columns.Count; j++)
                {
                    var colName = data.Columns[j].ToString();
                    count += Convert.ToDecimal(dtr[colName]);
                }
                if (count == 0)
                {
                    data.Rows.RemoveAt(i);
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

            using (var conn = _connectionFactory.GetConnection())
            {
                var nganh = id_nganh == "-1" ? "-1" : _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).iID_MaNganhMLNS;

                if (id_nganh != "51")
                {
                    id_nganh = "-1";
                }

                using (var cmd = new SqlCommand("sp_dutoan_report_baodam_tungdonvi_chitiet", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nam", nam);
                    cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                    cmd.Parameters.AddWithValue("@username", username.ToParamString());
                    cmd.Parameters.AddWithValue("@ngcha", id_nganh.ToParamString());
                    cmd.Parameters.AddWithValue("@nganh", nganh.ToParamString());
                    cmd.Parameters.AddWithValue("@id_donvi", id_donvi.ToParamString());
                    cmd.Parameters.AddWithValue("@loai", loaiBaoCao.ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", 1000);
                    var dt = cmd.GetDataset().Tables[0];

                    return dt;
                }

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
