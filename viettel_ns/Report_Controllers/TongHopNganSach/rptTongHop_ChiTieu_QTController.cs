using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using DomainModel.Controls;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;
using VIETTEL.Flexcel;
using Viettel.Services;
using Viettel.Data;
using VIETTEL.Helpers;

namespace VIETTEL.Models
{
    public class rptTongHop_ChiTieu_QTViewModel
    {
        public SelectList QuyList { get; set; }
        public SelectList NamNganSachList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptTongHop_ChiTieu_QTController : FlexcelReportController
    {
        public string sViewPath = "~/Report_Views/";
        private const string _filePath = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_QT.xls";
        private readonly string _sqlTongHop = FileHelpers.GetSqlQuery("rptTongHop_CT_QT.sql");

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;

        private string _loaiBC;
        private string _namNganSach;

        public ActionResult Index()
        {
            var vm = new rptTongHop_ChiTieu_QTViewModel()
            {
                QuyList = DataHelper.GetQuys().ToSelectList(),
                NamNganSachList = DataHelper.GetNamNganSachList().ToSelectList(),
            };

            var view = string.Format("{0}{1}.cshtml", "~/Views/Report_Views/TongHopNganSach/", this.ControllerName());
            return View(view, vm);
        }

        #region reports

        public ActionResult Print(
            string loaiBaoCao,
            string namNganSach,
            string ext = "pdf")
        {
            HamChung.Language();
            _loaiBC = loaiBaoCao;
            _namNganSach = namNganSach;

            var xls = createReport();
            var _file_name = $"Chênh_lệch_chỉ_tiêu_quyết_toán_B{PhienLamViec.iID_MaPhongBan}.xls";
            return Print(xls, ext, _file_name);
        }

        #endregion        

        #region private methods
        private ExcelFile createReport()
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));
            var fr = new FlexCelReport();

            loadData(fr);
            fr.SetValue("PhongBan", PhongBanModels.Get_TenPhongBan(PhienLamViec.iID_MaPhongBan));

            var mota = string.Empty;

            _namNganSach = _namNganSach.StartsWith("1") ?
                      "Ngân sách năm trước" :
                      "Ngân sách năm nay";
            fr.SetValue("NamNganSach", _namNganSach);
            var dvt = Request.GetQueryStringValue("dvt", 1000);
            fr.SetValue("dvt", dvt.ToStringDvt());

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls);

            xls.MergeH(5, 8, 200);
            xls.MergeH(6, 8, 200);

            return xls;

        }

        private void loadData(FlexCelReport fr)
        {
            var dt = getDataTable();
            var dk = _loaiBC == "1" ? "C3 <> 0" : _loaiBC == "2" ? "C4 <> 0" : "";
            var dtValue = dt.Select(dk).CopyToDataTable();

            var data = dtValue.SelectDistinct("ChiTiet", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa");

            for (int i = 0; i < 200; i++)
            {
                data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
            }

            var dtDonVi = dtValue.SelectDistinct("ChiTietDonVi", "Id_DonVi,TenDonVi");

            var dtX = dtDonVi.Clone();
            dtX.Columns.Add(new DataColumn("loai", typeof(string)));
            foreach (DataRow dtr in dtDonVi.Rows)
            {  
                switch (_loaiBC)
                {
                    case "1":
                        var nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 0;
                        dtX.Rows.Add(nr);
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 1;
                        dtX.Rows.Add(nr);
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 2;
                        dtX.Rows.Add(nr);
                        break;
                    case "2":
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 0;
                        dtX.Rows.Add(nr);
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 1;
                        dtX.Rows.Add(nr);
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 3;
                        dtX.Rows.Add(nr);
                        break;
                    default:
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 0;
                        dtX.Rows.Add(nr);
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 1;
                        dtX.Rows.Add(nr);
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 2;
                        dtX.Rows.Add(nr);
                        nr = dtX.NewRow();
                        nr.ItemArray = dtr.ItemArray;
                        nr["loai"] = 3;
                        dtX.Rows.Add(nr);
                        break;
                }
            }

            switch (_loaiBC)
            {
                case "1":
                    var row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 0;
                    dtX.Rows.InsertAt(row, 0);
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 1;
                    dtX.Rows.InsertAt(row, 0);
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 2;
                    dtX.Rows.InsertAt(row, 0);
                    break;
                case "2":
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 0;
                    dtX.Rows.InsertAt(row, 0);
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 1;
                    dtX.Rows.InsertAt(row, 0);
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 3;
                    dtX.Rows.InsertAt(row, 0);
                    break;
                default:
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 0;
                    dtX.Rows.InsertAt(row, 0);
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 1;
                    dtX.Rows.InsertAt(row, 0);
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 2;
                    dtX.Rows.InsertAt(row, 0);
                    row = dtX.NewRow();
                    row["Id_DonVi"] = "";
                    row["TenDonVi"] = "Tổng cộng".ToUpper();
                    row["loai"] = 3;
                    dtX.Rows.InsertAt(row, 0);
                    break;
            }

            var dtXList = dtX.AsEnumerable().OrderBy(x => x.Field<string>("Id_DonVi"))
                .ThenBy(x => x.Field<string>("loai"));

            var columns = new List<DataRow>();

            columns = dtXList.AsEnumerable().ToList();

            for (int i = 0; i < 200; i++)
            {
                var colName = $"C{i + 1}";
                var colNameMuc = $"D{i + 1}";
                if (i < columns.Count)
                {

                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("TenDonVi"));

                    var loai = row.Field<string>("loai");
                    fr.SetValue(colNameMuc, loai == "0" ? $"Dự toán" : loai == "1" ? $"Quyết toán" : loai == "2" ? $"Thừa" :$"Thiếu");

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           var colNam = loai == "0" ? $"C1" : loai == "1" ? $"C2" : loai == "2" ? $"C3" : $"C4";
                           var value = dtValue.AsEnumerable()
                                         .ToList()
                                         .Where(x => (string.IsNullOrEmpty(row.Field<string>("Id_DonVi")) || x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi"))
                                                        && x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa"))
                                         .Sum(x => x.Field<decimal>(colNam, 0));
                           r[colName] = value;
                       });
                }
                else
                {
                    fr.SetValue(colName, "");
                    fr.SetValue(colNameMuc, "");
                }

            }

            FillDataTableLNS(fr, data, FillLNSType.LNS1, "2020");
        }

        private DataTable getDataTable()
        {
            var dvt = Request.GetQueryStringValue("dvt", 1000);

            #region load data           

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(_sqlTongHop, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", PhienLamViec.iID_MaPhongBan.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", _namNganSach.ToParamString());
                    cmd.Parameters.AddWithValue("@donvi", PhienLamViec.iID_MaDonVi.ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", dvt);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }

            #endregion
        }

        #endregion
    }
}

