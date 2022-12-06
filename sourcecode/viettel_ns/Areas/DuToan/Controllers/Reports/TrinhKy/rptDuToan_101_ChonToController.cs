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

namespace VIETTEL.Areas.DuToan.Models
{
    public class rptDuToan_101_ChonToViewModel
    {
        public SelectList PhongBanList { get; set; }

        public SelectList ToList { get; set; }

    }
}

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_101_ChonToController : FlexcelReportController
    {
        #region var def 
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private string _path = "~/Areas/DuToan";
        private string _filePath = "~/Areas/DuToan/FlexcelForm/DonVi/101_ChonTo/rptDuToan_101_ChonTo.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private string _id_phongban;
        private int _to = 1;
        private int _dvt = 1000;
        private int _columnCount = 6;


        public ActionResult Index()
        {
            var phongBanList = _ngansachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptDuToan_101_ChonToViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Tất cả phòng ban --"),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            int to = 1,
            string ext = "pdf")
        {
            _to = to;
            _id_phongban = id_phongban;

            var xls = createReport();

            var filename = $"Phụ_lục_1_{_id_phongban}_Tờ {to}.{ext}";
            return Print(xls, ext, filename);        
        }

        public JsonResult Ds_To(string id_phongban)
        {
            _id_phongban = id_phongban;
            var data = getTable();
            var nganhs = data.SelectDistinct("donvi", "iID_MaDonVi");
            return ds_ToIn(nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + 1, _columnCount);
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
                  header1 = $"LNS: 1010000    Loại: 010    Khoản: 011",
                  header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ số: {_to}",
                  TieuDe1 = $"Dự toán chi ngân sách năm {PhienLamViec.iNamLamViec}",
                  TieuDe2 = "Phần chi tập trung tại BQP",
                  TenPhongBan = PhienLamViec.sTenPhongBan,
              })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);
            
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
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var dt = getTable();

            var data = dt.SelectDistinct("ChiTiet", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa");

            var dtX = dt.SelectDistinct("DonVi", "iID_MaDonVi,sTenDonVi");
            // them cot
            for (int i = 1; i <= _columnCount; i++)
            {
                data.Columns.Add($"C{i}", typeof(decimal));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["sTenDonVi"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(r, 0);
            }


            columns = _to == 1 ?
                dtX.AsEnumerable().Take(_columnCount).ToList() :
                dtX.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colIndex = $"i{i + 1}";
                if (i < columns.Count)
                {
                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("sTenDonVi"));

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           // lay tong cong
                           if (row.Field<string>("iID_MaDonVi").IsEmpty())
                           {
                               var value = dt.AsEnumerable()
                                     .ToList()
                                     .Where(x => x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa"))
                                     .Sum(x => x.Field<decimal>("rTuChi", 0));
                               r[colName] = value;
                           }
                           else
                           {
                               var rowValue = dt.AsEnumerable()
                                     .ToList()
                                     .FirstOrDefault(x => x.Field<string>("iID_MaDonVi") == row.Field<string>("iID_MaDonVi") &&
                                                  x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa"));
                               if (rowValue != null)
                                   r[colName] = rowValue.Field<decimal>("rTuChi");
                           }
                       });
                    fr.SetValue(colIndex, (_to - 1) * _columnCount + i);
                }
                else
                {
                    fr.SetValue($"C{i + 1}", "");
                    fr.SetValue(colIndex, "");
                }

            }

            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }
        
        private DataTable getTable()
        {
            var path_sql = _path + "/Sql/dt_chitieu_chitaptrung.sql";

            var sql = FileHelpers.GetSqlQueryPath(path_sql);
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam = PhienLamViec.iNamLamViec,
                        id_phongban = _id_phongban.ToParamString(),
                        id_donvi = PhienLamViec.iID_MaDonVi.ToParamString(),
                        dvt = _dvt,
                    });

                return dt;
            }
        }
        #endregion
    }
}
