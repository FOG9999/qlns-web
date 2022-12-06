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

namespace VIETTEL.Areas.DuToanBS.Controllers
{
    public class rptDuToanBS_InPhuLucTHPCViewModel
    {
        public SelectList DotList { get; set; }
        public SelectList NganhList { get; set; }
        public string TieuDe { get; set; }
        public string QuyetDinh { get; set; }
    }
    public class rptDuToanBS_InPhuLucTHPCController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/DuToanBS/Views/Reports/InPhuLucTH/";
        private string _path = "~/Areas/DuToanBS";
        private string _filePath = "~/Areas/DuToanBS/FlexcelForm/InPhuLucTH/rptDuToanBS_PhuLuc_Ngang_NNPC.xls";

        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly IDuToanBsService _dutoanbsService = DuToanBsService.Default;

        private string _id_nganh;
        private int _to = 1;
        private int _nganhCount = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 9;

        public ActionResult Index()
        {
            var dtDot = _dutoanbsService.GetDotsTLTHCuc(PhienLamViec.iNamLamViec, null);
            var dotList = dtDot
                .ToSelectList("iID_MaDot", "sMoTa", "-1", "-- Chọn đợt --");

            var nganhList = _ngansachService.Nganh_GetAll_ByPhongBan(PhienLamViec.iNamLamViec, string.Empty)
                   .ToSelectList("iID_MaNganh", "sTenNganh");

            var vm = new rptDuToanBS_InPhuLucTHPCViewModel
            {
                DotList = dotList,
                NganhList = nganhList,
                TieuDe = "Giao dự toán ngân sách",
                QuyetDinh = $"số           /QĐ-BQP ngày     /     /{PhienLamViec.iNamLamViec} của Bộ trưởng Bộ Quốc phòng",
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string iID_MaDot,
            string id_nganh,
            string tenPhuLuc,
            string quyetDinh,
            int to = 1,
            string ext = "pdf")
        {
            _id_nganh = id_nganh;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;

            var xls = createReport(iID_MaDot,tenPhuLuc,quyetDinh);
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
            var tenNganh = "Ngành: " + _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;
            var filename = $"{tenNganh}_Tờ số {_to}.{ext}";
            return Print(xls, ext, filename);
        }       

        public JsonResult Ds_To(string iID_MaDot, string id_nganh)
        {
            iID_MaDot = _dutoanbsService.GetChungTus_GomTHCuc(iID_MaDot).Join();
            var data = getTable(iID_MaDot, id_nganh);
            var count = 0;
            if (data.Rows.Count > 0)
            {
                var nganhs = data.SelectDistinct("nganh", "Nganh,sNG");
                var nganhsTong = data.SelectDistinct("nganh", "sNG");

                count = nganhs.Rows.Count + (nganhsTong.Rows.Count > 1 ? nganhsTong.Rows.Count : 0) + 1;
            }
            return ds_ToIn(count, _columnCount);
        }

        private ExcelFile createReport(string iID_MaDot, string tenPhuLuc, string quyetDinh)
        {
            var fr = new FlexCelReport();
            loadData(fr, iID_MaDot);

            var sTenDonVi = _ngansachService.GetDonVi(PhienLamViec.iNamLamViec, _id_nganh).sTen;

            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("TenPhuLuc", string.IsNullOrWhiteSpace(tenPhuLuc) ? "TenPhuLuc" : tenPhuLuc.ToUpper());
            fr.SetValue("QuyetDinh", string.IsNullOrWhiteSpace(quyetDinh) ? "QuyetDinh" : quyetDinh);
            fr.SetValue(new
            {
                h1 = sTenDonVi,
                h2 = $"Tờ số {_to} - Đơn vị tính: {_dvt.ToStringDvt()}",
            });            

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);

            if (_to == 1 && _nganhCount > 1)
            {
                xls.MergeCells(7, 4, 8, 4);
                xls.MergeH(7, 5, 8);
            }
            else
            {
                xls.MergeH(7, 4, 9);
            }

            return xls;
        }

        private void loadData(FlexCelReport fr, string iID_MaDot)
        {
            var iID_MaChungTu = _dutoanbsService.GetChungTus_GomTHCuc(iID_MaDot).Join();
            var dt = getTable(iID_MaChungTu, _id_nganh);

            var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtX = dt.SelectDistinct("Nganh", "sLNS,sNG,Nganh,TenNganh,sXauNoiMa");
            var dtNganh = dt.SelectDistinct("Lns", "sLNS,TenNganh");

            _nganhCount = dtNganh.Rows.Count;

            // them cot tong cong cua tung Nganh
            for (int i = 0; i < _nganhCount; i++)
            {
                var ng = dtNganh.Rows[i]["sLNS"].ToString();
                var ngcount = dtX.Select($"sLNS = '{ng}'").Count();
                if (ngcount > 1)
                {
                    var nrow = dtX.NewRow();
                    nrow["sLNS"] = dtNganh.Rows[i]["sLNS"];
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
            if (_nganhCount > 1)
            {
                var row_tuchi = dtX.NewRow();
                row_tuchi["TenNganh"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(row_tuchi, 0);
            }

            var dtXList = dtX.AsEnumerable().OrderBy(x => x.Field<string>("sLNS"))
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
                                 .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("sLNS"))
                                                    || (x.Field<string>("sLNS") == row.Field<string>("sLNS") &&
                                                    (string.IsNullOrWhiteSpace(row.Field<string>("sXauNoiMa"))
                                                    || x.Field<string>("sXauNoiMa") == row.Field<string>("sXauNoiMa")))) &&
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

        private DataTable getTable(string iID_MaChungTu, string id_nganh)
        {
            var sql = string.Empty;
            var path_sql = _path + "/Sql/dtbs_chitieu_thcuc_phuluc_ngang_phancap.sql";

            sql = FileHelpers.GetSqlQueryPath(path_sql);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", id_nganh.ToParamString());
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@dvt", _dvt);

                    var dt = cmd.GetTable();
                    return dt;
                }
            }
        }        
        #endregion
    }

}
