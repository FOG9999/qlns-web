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

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_BaoDam_KyThuat_ChiTietNgController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToan/DonVi/rptDuToan_BaoDam_KyThuat_ChiTietNg.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 9;

        public ActionResult Index()
        {

            var vm = new rptDuToan_K11_NG_THViewModel
            {
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(            
            int to = 1,
            string ext = "pdf")
        {
            var check = ((_ngansachService.GetUserRoleType(Username) != (int)UserRoleType.TroLyPhongBan && PhienLamViec.PhongBan.sKyHieu == "10") 
                            || PhienLamViec.PhongBan.sKyHieu == "02") ? true 
                            : _ngansachService.Nganh_GetAll(PhienLamViec.iNamLamViec,Username,PhienLamViec.PhongBan.sKyHieu)
                                              .AsEnumerable()
                                              .Select(r => r.Field<string>("iID_MaNganhMLNS"))
                                              .JoinDistinct().Contains("60,61,62,64,65,66");
            if (check) { 
                _dvt = Request.GetQueryStringValue("dvt", 1000);
                _nam = int.Parse(PhienLamViec.iNamLamViec);
                _to = to;

                var xls = createReport();
                return Print(xls, ext);
            } else
            {
                return RedirectToAction("Index", "DuToan_Report", new { area = "", sLoai = 1 });
            }
        }

        public JsonResult Ds_To()
        {
            var data = getTable(PhienLamViec.iNamLamViec);
            var nganhsTong = data.SelectDistinct("nganh", "sNG");

            var count = nganhsTong.Rows.Count + 1;
            return ds_ToIn(count, _columnCount);
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            fr.SetValue(new
            {
                header1 = $"Ngành Kỹ thuật",
                header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                TieuDe1 = $"Tổng hợp dự toán ngân sách năm {PhienLamViec.iNamLamViec}",
                TieuDe2 = $"Tổng hợp chi tiết số bảo đảm kỹ thuật",
                soPL = $"1d",
            });

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);

            // merge cot tong cong
           
            xls.MergeH(6, 4, 9);
            xls.MergeH(7, 4, 9);
            
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(),
                () => getTable(),
                CacheTimes.OneMinute,
                false);

            var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtNganh = dt.SelectDistinct("Nganh", "sNG,TenNganh");            

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
            if (_to == 1)
            {
                var row_tuchi = dtNganh.NewRow();
                row_tuchi["sNG"] = "";
                row_tuchi["TenNganh"] = "Tổng cộng".ToUpper();
                dtNganh.Rows.InsertAt(row_tuchi, 0);
            }

            var dtXList = dtNganh.AsEnumerable().OrderBy(x => x.Field<string>("sNG"));

            columns = _to == 1 ?
                dtXList.AsEnumerable().Take(_columnCount).ToList() :
                dtXList.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();

            var mlns = _ngansachService.MLNS_GetAll(PhienLamViec.iNamLamViec);

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
                                 .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("sNG")) || (x.Field<string>("sNG") == row.Field<string>("sNG"))) &&                                           
                                            x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                 .Sum(x => x.Field<decimal>("TuChi", 0));
                           r[colName] = value;
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private DataTable getTable()
        {
            return getTable(PhienLamViec.iNamLamViec);
        }

        private DataTable getTable(string nam)
        {
            string id_donvi = PhienLamViec.iID_MaDonVi;

            var sql = FileHelpers.GetSqlQuery("rptDuToan_BaoDam_KyThuat_ChiTietNg.sql");
            using (var conn = _connectionFactory.GetConnection())
            {                
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        id_donvi = id_donvi.ToParamString(),
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
            return $"{this.ControllerName()}_{Username}_{_nam}_{_dvt}";
        }

        #endregion
    }

}
