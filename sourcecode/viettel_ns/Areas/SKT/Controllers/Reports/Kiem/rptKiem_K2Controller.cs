using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptKiem_K2Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/Kiem/";
        private string _filePath = "~/Report_ExcelFrom/SKT/Kiem/rptKiem_K2.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly ISKTService _sKTService = SKTService.Default;

        private string _id_phongban;
        private string _id_nganh = "-1";
        private int _loaiBaoCao;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 9;

        public ActionResult Index()
        {
            var nganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, (PhienLamViec.iID_MaPhongBan == "02" || PhienLamViec.iID_MaPhongBan == "11") ? "" : PhienLamViec.iID_MaPhongBan)
                                            .ToSelectList("MaNganh", "TenNganh", "-1", "-- chọn ngành --");
            var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptKiem_K2ViewModel
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
            _id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
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
            var id = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            var list = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id)
                                          .ToSelectList("MaNganh", "TenNganh", "-1", "-- Chọn ngành --");
            return ToDropdownList(new VIETTEL.Models.ChecklistModel("id_nganh", list));
        }
        public JsonResult Ds_To(string id_phongban, string id_nganh, int loaiBaoCao)
        {
            var data = getTable(PhienLamViec.NamLamViec - 1, id_phongban, id_nganh, loaiBaoCao, Username);
            var nganhs = data.SelectDistinct("nganh", "Nganh,sNG");
            var nganhsTong = data.SelectDistinct("nganh", "sNG");

            var count = nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + nganhsTong.Rows.Count + 2;
            return ds_ToIn(count, _columnCount);
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var tenNganh = string.IsNullOrWhiteSpace(_id_nganh) || _id_nganh == "-1" ? string.Empty : "Ngành: " + _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh;
            fr.SetValue(new
            {
                header1 = tenNganh,
                header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                TieuDe1 = $"Tổng hợp dự toán ngân sách năm {PhienLamViec.NamLamViec - 1}",
                TieuDe2 = "Tổng hợp số phân cấp đặc thù",
                soPL = _loaiBaoCao == 1 ? "K2a" : "K2b",
                TenPhongBan = string.IsNullOrWhiteSpace(_nganSachService.CheckParam_PhongBan(_id_phongban)) ? string.Empty : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
            });

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);

            // merge cot tong cong
            if (_to == 1)
            {
                xls.MergeCells(6, 4, 7, 4);
                xls.MergeH(6, 5, 8);
                xls.MergeH(7, 5, 8);
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
            var dt = getTable(PhienLamViec.NamLamViec - 1, _id_phongban, _id_nganh, _loaiBaoCao, Username);

            var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
            var dtX = dt.SelectDistinct("Nganh", "sLNS,sNG,Nganh,TenNganh,sXauNoiMa");
            var dtNganh = dt.SelectDistinct("Nganh", "sNG,TenNganh");

            // them cot tong cong cua tung Nganh
            var rowsCountX = dtNganh.Rows.Count;
            for (int i = 0; i < rowsCountX; i++)
            {
                var nrow = dtX.NewRow();
                nrow["sNG"] = dtNganh.Rows[i]["sNG"];
                nrow["TenNganh"] = dtNganh.Rows[i]["TenNganh"];
                dtX.Rows.Add(nrow);
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
            if (_to == 1)
            {
                var row_tuchi = dtX.NewRow();
                row_tuchi["TenNganh"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(row_tuchi, 0);  
            }

            var dtXList = dtX.AsEnumerable().OrderBy(x => x.Field<string>("sNG"))
                .ThenBy(x => x.Field<string>("Nganh"));

            columns = _to == 1 ?
                dtXList.AsEnumerable().Take(_columnCount).ToList() :
                dtXList.AsEnumerable().Skip((_to - 1) * _columnCount - 2).Take(_columnCount).ToList();

            var mlns = _nganSachService.MLNS_GetAll(PhienLamViec.iNamLamViec);

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
        
        private DataTable getTable(int nam, string id_phongban, string id_nganh, int loaibaocao, string username)
        {
            id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;

            var sql = FileHelpers.GetSqlQuery("rptKiem_K2.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var nganh =  _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id_phongban).AsEnumerable()
                                .Where(x => !string.IsNullOrEmpty(x.Field<string>("iID_MaNganhMLNS")) && (id_nganh == "-1" || x.Field<string>("MaNganh") == id_nganh))
                                .Select(x => x.Field<string>("iID_MaNganhMLNS")).Join(",");
                
                var dt = conn.GetTable(sql,
                    new
                    {
                        nam,
                        id_phongban = "".ToParamString(),
                        nganh = nganh.ToParamString(),
                        loai = loaibaocao.ToParamString(),
                        dvt = _dvt,
                    });               

                return dt;
            }
        }        
        #endregion
    }

}
