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
    public class rptDuToan_BaoDam_TongHop_ChiTietController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToan/Cuc/rptDuToan_BaoDam_TongHop_ChiTiet.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private int _loaiBaoCao;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 8;

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
            int loaiBaoCao = 0,
            int to = 1,
            string ext = "pdf")
        {
            _loaiBaoCao = loaiBaoCao;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;

            var xls = createReport();
            return Print(xls, ext);
        }        

        public JsonResult Ds_To(int loaiBaoCao)
        {
            var data = getTable(PhienLamViec.iNamLamViec, loaiBaoCao);
            var nganhs = data.SelectDistinct("nganh", "sNG");

            var count = nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + (nganhs.Rows.Count > 1 ? 1 : 0);
            return ds_ToIn(count, _columnCount);
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            fr.SetValue(new
            {
                header1 = "BỘ QUỐC PHÒNG",
                header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                TieuDe1 = $"Tổng hợp dự toán ngân sách năm {PhienLamViec.iNamLamViec}",
                TieuDe2 = $"",
                soPL = "1a-C",
            });

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);

            // merge cot tong cong
            
            xls.MergeH(6, 4, 9);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var dt = getTable(PhienLamViec.iNamLamViec, _loaiBaoCao);

            var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtNganh = dt.SelectDistinct("Nganh", "sNG,TenNganh");

            for (int i = 0; i < _columnCount; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));               
            }


            if (dtNganh.Rows.Count > 1)
            {
                var nr = dtNganh.NewRow();
                nr["sNG"] = "";
                nr["TenNganh"] = "Tổng cộng".ToUpper();
                dtNganh.Rows.InsertAt(nr, 0);
            }

            var columns = new List<DataRow>();

            var dtXList = dtNganh.AsEnumerable().OrderBy(x => x.Field<string>("sNG"));

            columns = dtXList.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();

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

            data.NullToZero("C1,C2,C3,C4,C5,C6,C7,C8".Split(','));

            for(int i = data.Rows.Count - 1; i >= 0; i--)
            {
                double count = 0;
                var dtr = data.Rows[i];

                for (int j = 2; j < data.Columns.Count; j++)
                {
                    var colName = data.Columns[j].ToString();
                    count += Convert.ToDouble(dtr[colName]);
                }
                if (count == 0)
                {
                    data.Rows.RemoveAt(i);
                }
            }
            fr.AddTable(data);
        }        

        private DataTable getTable(string nam, int loaiBaoCao)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_BaoDam_TongHop_ChiTiet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        loai = loaiBaoCao.ToParamString(),
                        dvt = _dvt,
                    });               

                return dt;
            }
        }
        #endregion
    }

}
