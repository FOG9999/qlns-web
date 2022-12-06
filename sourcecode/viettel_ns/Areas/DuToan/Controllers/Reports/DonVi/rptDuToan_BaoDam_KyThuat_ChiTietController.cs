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
    public class rptDuToan_BaoDam_KyThuat_ChiTietController : FlexcelReportController
    {
        #region var def
        private string _path = "~/Areas/DuToan";
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToan/DonVi/rptDuToan_BaoDam_KyThuat_ChiTiet.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 9;
        private string _nganh;

        public ActionResult Index()
        {
            var nganhList = _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec, Username).Select("iID_MaNganh in ('29','31','33','40','41','42','43','44','45','46','47','51','53','56','99')").CopyToDataTable()
                                            .ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- chọn ngành --");

            var vm = new rptDuToan_K11_NG_THViewModel
            {
                NganhList = nganhList,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(            
            int to = 1,
            string nganh = null,
            string ext = "pdf")
        {
            var check = ((_ngansachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyPhongBan && PhienLamViec.PhongBan.sKyHieu == "10") 
                            || PhienLamViec.PhongBan.sKyHieu == "02") ? true 
                            : _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec,Username,PhienLamViec.PhongBan.sKyHieu)
                                              .AsEnumerable()
                                              .Select(r => r.Field<string>("iID_MaNganhMLNS"))
                                              .JoinDistinct().Contains("60,61,62,64,65,66,83,84,85");
            if (check) { 
                _dvt = Request.GetQueryStringValue("dvt", 1000);
                _nam = int.Parse(PhienLamViec.iNamLamViec);
                _to = to;
                _nganh = nganh;

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

                return Print(xls, ext);
            } else
            {
                return RedirectToAction("Index", "DuToan_Report", new { area = "", sLoai = 1 });
            }
        }

        public JsonResult Ds_To(string nganh = null)
        {
            _nganh = nganh;
            var data = getTable(PhienLamViec.iNamLamViec);
            var nganhs = data.SelectDistinct("nganh", "Nganh,sNG");
            var nganhsTong = data.SelectDistinct("nganh", "sNG");

            var count = nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + nganhsTong.Rows.Count + 1;
            return ds_ToIn(count, _columnCount);
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var tenNganh = string.IsNullOrWhiteSpace(_nganh) || _nganh == "-1" ? string.Empty : "Ngành: " + _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, _nganh).sTenNganh;
            fr.SetValue(new
            {
                header1 = tenNganh,
                header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                TieuDe1 = $"Tổng hợp dự toán ngân sách năm {PhienLamViec.iNamLamViec}",
                TieuDe2 = $"Tổng hợp chi tiết số bảo đảm kỹ thuật",
                soPL = $"1d",
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

            // merge cot tong cong
            if (_to == 1)
            {
                xls.MergeCells(4, 4, 5, 4);
                xls.MergeH(4, 5, 8);
                xls.MergeH(5, 5, 8);
            }
            else
            {
                xls.MergeH(4, 4, 9);
                xls.MergeH(5, 4, 9);
            }
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var dt = getTable(PhienLamViec.iNamLamViec);

            var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtX = dt.SelectDistinct("Nganh", "sNG,Nganh,TenNganh,sXauNoiMa");
            var dtNganh = dt.SelectDistinct("Nganh", "sNG,TenNganh");

            // them cot tong cong cua tung Nganh
            var rowsCountX = dtNganh.Rows.Count;
            for (int i = 0; i < rowsCountX; i++)
            {
                var ng = dtNganh.Rows[i]["sNG"].ToString();
                var ngcount = dtX.Select($"sNG = '{ng}'").Count();
                if (ngcount > 1) { 
                    var nrow = dtX.NewRow();
                    nrow["sNG"] = dtNganh.Rows[i]["sNG"];
                    nrow["TenNganh"] = dtNganh.Rows[i]["TenNganh"];
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
            if (dtNganh.Rows.Count > 1)
            {
                var row_tuchi = dtX.NewRow();
                row_tuchi["TenNganh"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(row_tuchi, 0);
            }

            var dtXList = dtX.AsEnumerable().OrderBy(x => x.Field<string>("sNG"))
                .ThenBy(x => x.Field<string>("Nganh"));

            columns = _to == 1 ?
                dtXList.AsEnumerable().Take(_columnCount).ToList() :
                dtXList.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();

            var mlns = _ngansachService.MLNS_GetAll(PhienLamViec.iNamLamViec);

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colNameMuc = $"D{i + 1}";
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

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           var value = dt.AsEnumerable()
                                 .ToList()
                                 .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("sNG")) || (x.Field<string>("sNG") == row.Field<string>("sNG") && (string.IsNullOrWhiteSpace(row.Field<string>("sXauNoiMa")) || x.Field<string>("sXauNoiMa") == row.Field<string>("sXauNoiMa")))) &&                                            
                                            x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                 .Sum(x => x.Field<decimal>("TuChi", 0));
                           r[colName] = value;
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colNameMuc, "");
                }

            }

            data.NullToZero(colNames.Split(','));

            for(int i = data.Rows.Count - 1; i >= 0; i--)
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

        private DataTable getTable(string nam)
        {
            string id_donvi = PhienLamViec.iID_MaDonVi;
            var ng = string.IsNullOrEmpty(_nganh) || _nganh == "-1" ? "" : _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, _nganh).iID_MaNganhMLNS;

            var path_sql = _path + "/Sql/rptDuToan_BaoDam_KyThuat_ChiTiet.sql";

            var sql = FileHelpers.GetSqlQueryPath(path_sql);
            using (var conn = _connectionFactory.GetConnection())
            {                
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        id_donvi = id_donvi.ToParamString(),
                        dvt = _dvt,
                        Ng = ng.ToParamString(),
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
            return $"{this.ControllerName()}_{Username}_{_nam}_{_dvt}";
        }

        #endregion
    }

}
