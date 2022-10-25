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
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptQuyetToan_TongHop_NamViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
        public SelectList TypeList { get; set; }
        public SelectList ReportList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptQuyetToan_DonVi_PL02_ChonToController : FlexcelReportController
    {
        private const string sFilePath = "~/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToan_DonVi_PL02_ChonTo.xls";
        private const string sFilePath6 = "~/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToan_DonVi_PL06.xls";
        private const string sFilePath7 = "~/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToan_DonVi_PL07_ChonTo.xls";

        #region ctors

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;
        private int _columnsCount = 3;

        public ActionResult Index()
        {

            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sTen");
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptQuyetToan_TongHop_NamViewModel()
            {
                iNamLamViec = PhienLamViec.iNamLamViec,
                PhongBanList = phongbanList,
                DonViList = donviList,
                TypeList = _quyetToanService.IncludeDonVi().ToSelectList("loai", "Type"),
                ReportList = _quyetToanService.ReportType().ToSelectList("baocao", "Report")
            };

            var view = string.Format("{0}{1}.cshtml", "~/Views/Report_Views/QuyetToan/TQT/", "rptTQT_PhuLuc2_ChonTo");
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string type,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string loai,
            int toSo)
        {
            var xls = createReport(ext, type, iID_MaPhongBan, iID_MaDonVi, loai, toSo);
            var loaiPl = loai == "6" ? "6" : loai == "7" ? "7" : "2";
            var filename = $"Quyết_toán_{NganSachService.Default.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sMoTa}_năm_{PhienLamViec.iNamLamViec}_Phụ_lục_{loaiPl}_tờ_số_{toSo}.xls";
            return Print(xls, ext, filename);
        }

        public ActionResult Ds_To(string iID_MaPhongBan, string iID_MaDonVi, string loai)
        {
            var tempDt = getDataSet(iID_MaPhongBan, iID_MaDonVi, loai).Tables[1];
            var count = tempDt.Rows.Count + 1;
            var tempDtL = HamChung.SelectDistinct("dt", tempDt, "L", "L");
            for (int i = 0; i < tempDtL.Rows.Count; i++)
            {
                var temp = tempDt.Select("L=" + tempDtL.Rows[i]["L"]).Count();
                if (temp > 2)
                    count++;
            }

            return ds_ToIn(count, 3);
        }

        #endregion

        #region private methods

        private DataSet getDataSet(string iID_MaPhongBan, string iID_MaDonVi,string loai)
        {
            //var sql = loai == "7" ? "qt_report_pl7I" : "qt_report_plI";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand("qt_report_plI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@donvi", iID_MaDonVi.ToParamString());
                cmd.Parameters.AddWithValue("@phongban", GetPhongBanId(iID_MaPhongBan).ToParamString());
                cmd.Parameters.AddWithValue("@loai", loai);

                var dt = cmd.GetDataset();
                return dt;
            }
        }

        private ExcelFile createReport(
            string ext,
            string type,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string loai,
            int toSo)
        {
            string dv = (!string.IsNullOrEmpty(iID_MaPhongBan) ? NganSachService.Default.GetPhongBanById(iID_MaPhongBan).sMoTa + " - " : "") + (type == "1" ? NganSachService.Default.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sMoTa + " - " : "");
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(loai == "6" ? sFilePath6 : loai == "7" ? sFilePath7 : sFilePath));

            var fr = new FlexCelReport();

            var dtSet = getDataSet(iID_MaPhongBan, iID_MaDonVi,loai);

            var data = dtSet.Tables[0];
            var dtCol = dtSet.Tables[1];
            var datas = dtSet.Tables[2];
            var dtL = dtCol.SelectDistinct("Loai", "L");

            for (int i = 0; i < dtL.Rows.Count; i++)
            {
                var temp = dtCol.Select("L=" + dtL.Rows[i]["L"]).Count();
                if (temp > 1)
                {
                    var r = dtCol.NewRow();
                    r["L"] = dtL.Rows[i]["L"].ToString();
                    r["sL"] = "Loại " + dtL.Rows[i]["L"].ToString();
                    r["K"] = "";
                    r["sK"] = "Tổng số";
                    dtCol.Rows.InsertAt(r, 0);
                }
            }

            var Nr = dtCol.NewRow();
            Nr["L"] = "";
            Nr["sL"] = "Tổng số";
            Nr["K"] = "";
            Nr["sK"] = "Tổng số";
            dtCol.Rows.InsertAt(Nr, 0);

            var view = dtCol.DefaultView;
            view.Sort = "L,K";
            dtCol = view.ToTable();

            for (int i = 0; i < _columnsCount; i++)
            {
                data.Columns.Add(new DataColumn($"V{i + 1}1", typeof(decimal)));
                data.Columns.Add(new DataColumn($"V{i + 1}2", typeof(decimal)));
            }

            var columns = new List<DataRow>();

            columns = toSo == 1 ?
                        dtCol.AsEnumerable().Take(_columnsCount).ToList() :
                        dtCol.AsEnumerable().Skip((toSo - 1) * _columnsCount).Take(_columnsCount).ToList();

            for (int i = 0; i < _columnsCount; i++)
            {
                var colName = $"T1{i + 1}";
                var colName1 = $"T2{i + 1}";

                if (i < columns.Count)
                {
                    fr.SetValue(colName, columns[i].Field<string>("sL"));
                    fr.SetValue(colName1, columns[i].Field<string>("sK"));
                    fr.SetValue($"T{i + 1}_31", "Số báo cáo");
                    fr.SetValue($"T{i + 1}_32", "Số xét duyệt");
                    fr.SetValue($"T{i + 1}_33", "Chênh lệch");

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           var value =
                                datas.AsEnumerable()
                                   .ToList()
                                   .Where(x =>
                                    x.Field<string>("Code") == r.Field<string>("Code") &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("L")) || x.Field<string>("L") == columns[i].Field<string>("L")) &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("K")) || x.Field<string>("K") == columns[i].Field<string>("K")))
                                   .Sum(x => x.Field<double>("N1", 0));

                           r[$"V{i + 1}1"] = value;

                           value =
                                datas.AsEnumerable()
                                   .ToList()
                                   .Where(x =>
                                    x.Field<string>("Code") == r.Field<string>("Code") &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("L")) || x.Field<string>("L") == columns[i].Field<string>("L")) &&
                                    (string.IsNullOrWhiteSpace(columns[i].Field<string>("K")) || x.Field<string>("K") == columns[i].Field<string>("K")))
                                   .Sum(x => x.Field<double>("N2", 0));

                           r[$"V{i + 1}2"] = value;
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colName1, "");
                    fr.SetValue($"T{i + 1}_31", "");
                    fr.SetValue($"T{i + 1}_32", "");
                    fr.SetValue($"T{i + 1}_33", "");
                }
            }

            data.TableName = "Data";
            fr.AddTable(data);

            fr.UseCommonValue(new Application.Flexcel.FlexcelModel()
            {
                header2 = dtCol.Rows.Count / 3 > 1 ? $"{dv}Đơn vị tính: đồng - Tờ số: {toSo} - Trang: " : $"{dv}Đơn vị tính: đồng - Trang: ",
            })
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls, toSo);

            if (toSo == 1)
            {
                if (loai == "1")
                { 
                    xls.MergeCells(8, 4, 9, 6);
                    xls.MergeH(8, 7, 6);
                    xls.MergeH(9, 7, 6);
                } else if (loai == "7")
                {
                    xls.MergeCells(7, 4, 8, 6);
                    xls.MergeH(7, 7, 6);
                    xls.MergeH(8, 7, 6);
                }
            }
            else
            {
                if (loai == "1")
                {
                    xls.MergeH(8, 4, 9);
                    xls.MergeH(9, 4, 9);
                }
                else if (loai == "7")
                {
                    xls.MergeH(7, 4, 9);
                    xls.MergeH(8, 4, 9);
                }
            }

            return xls;
        }
        #endregion
    }
}
