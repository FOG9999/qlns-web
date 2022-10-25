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
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_THNSNN_PhanCapViewModel
    {
        public SelectList NganhList { get; set; }
    }
    public class rptDuToan_THNSNN_PhanCapController : FlexcelReportController
    {
        public string _viewPath = "~/Areas/DuToan/Views/Report/Cuc/";
        private const string _filePath = "~/Areas/DuToan/FlexcelForm/Cuc/rptDuToan_THNSNN_PhanCap/rptDuToan_THNSNN_PhanCap.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly IDuToanService _dtService = DuToanService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt = 1000;
        private int _to;
        private string _nganh;
        private string _loai;
        private int _columnCount = 8;
        private int _lnsCount = 1;

        public ActionResult Index()
        {
            var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec, Username, PhienLamViec.iID_MaPhongBan);

            var vm = new rptDuToan_THNSNN_PhanCapViewModel()
            {
                NganhList = dNganh.ToSelectList("Id", "Nganh"),
            };

            var view = _viewPath + "rptDuToan_THNSNN_PhanCap.cshtml";

            return View(view, vm);            
        }
        public JsonResult Ds_To(string nganh = null)
        {
            _nganh = nganh;
            var data = getTable();
            var lnss = data.SelectDistinct("lns", "Nganh,sLNS");
            var lnssTong = data.SelectDistinct("lns", "sLNS");
            var rowsCountX = lnssTong.Rows.Count;
            var countNT = 0;
            for (int i = 0; i < rowsCountX; i++)
            {
                var ng = lnssTong.Rows[i]["sLNS"].ToString();
                var ngcount = lnss.Select($"sLNS = '{ng}'").Count();
                if (ngcount > 1)
                {
                    countNT++;
                }
            }
            var count = lnss.Rows.Count == 0 ? 0 : ((rowsCountX > 1) ? (countNT + lnss.Rows.Count + 1) : (countNT + lnss.Rows.Count));
            return ds_ToIn(count, _columnCount);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="loai">1: NSSD, 2: NSBĐ ngành</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(
            int to,
            string nganh,
            string loai = "1",
            string ext = "pdf",
            int dvt = 1)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._to = to;
            this._nganh = nganh;
            this._loai = loai;
            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var duoi = _loai == "1" ? "Gửi ngành" : "Gửi đơn vị";
                var filename = $"Báo_cáo_THNSNN_PC_{duoi}_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }

        }
        
        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private Dictionary<bool, ExcelFile> createReport()
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);

            var check = loadData(fr);

            if (check.ContainsKey(false))
            {
                return new Dictionary<bool, ExcelFile>
                {
                    {false, xls}
                };
            }
            else
            {

                xls.Open(Server.MapPath(_filePath));
                var soQd = _dtService.GetCauHinhBia(PhienLamViec.iNamLamViec);

                fr.SetValue(new
                {
                    headerr = $"Đơn vị tính: {_dvt.ToHeaderMoney().Trim()}",
                    nganh = _nganh == "-1" ? "" : "Ngành: " + _nganSachService.ChuyenNganh_Get(PhienLamViec.iNamLamViec, _nganh).Trim(),
                    qd = _loai == "1" ? $"(Kèm theo công văn {soQd["sSoCVKHNS"].ToString()} của Cục Tài chính/BQP)" : $"(Phụ lục kèm theo Quyết định {soQd["sSoQuyetDinh"].ToString()} của Bộ trưởng BQP về việc giao dự toán Ngân sách năm {PhienLamViec.iNamLamViec})",
                    namn = PhienLamViec.NamLamViec - 1,
                    nam = PhienLamViec.NamLamViec,
                });

                fr.UseCommonValue()
                  .UseChuKyForController(this.ControllerName())
                  .UseForm(this).Run(xls, _to);
                if (_to == 1 && _lnsCount > 1)
                {
                    xls.MergeCells(7, 3, 8, 3);
                    xls.MergeH(7, 4, 7);
                }
                else
                {
                    xls.MergeH(7, 3, 8);
                }

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
                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var dt = getTable();
            if (dt.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var data = dt.SelectDistinct("ChiTiet", "Id_DonVi,TenDonVi");
                var dtLns = dt.SelectDistinct("Lns", "sLNS,Nganh,TenLNS,sXauNoiMa");
                var dtLnsTo = dt.SelectDistinct("Lns", "sLNS,TenLNS");

                for (int i = 0; i < _columnCount; i++)
                {
                    data.Columns.Add(new DataColumn($"C{i + 1}", typeof(double)));
                }

                _lnsCount = dtLnsTo.Rows.Count;
                for (int i = 0; i < _lnsCount; i++)
                {
                    var ng = dtLnsTo.Rows[i]["sLNS"].ToString();
                    var ngcount = dtLns.Select($"sLNS = '{ng}'").Count();
                    if (ngcount > 1)
                    {
                        var r1 = dtLns.NewRow();
                        r1["TenLNS"] = dtLnsTo.Rows[i]["TenLNS"];
                        r1["sLNS"] = dtLnsTo.Rows[i]["sLNS"];
                        r1["sXauNoiMa"] = "";
                        r1["Nganh"] = "";
                        dtLns.Rows.InsertAt(r1, 0);
                    }
                }

                var columns = new List<DataRow>();

                if (_lnsCount > 1)
                {
                    var r1 = dtLns.NewRow();
                    r1["TenLNS"] = "Tổng cộng";
                    r1["sLNS"] = "";
                    r1["sXauNoiMa"] = "";
                    r1["Nganh"] = "";
                    dtLns.Rows.InsertAt(r1, 0);
                }

                var dtXList = dtLns.AsEnumerable().OrderBy(x => x.Field<string>("sLNS")).ThenBy(x=>x.Field<string>("sXauNoiMa"));

                columns = _to == 1 ?
                    dtXList.AsEnumerable().Take(_columnCount).ToList() :
                    dtXList.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();

                var mlns = _nganSachService.MLNS_GetAll(PhienLamViec.iNamLamViec);

                for (int i = 0; i < _columnCount; i++)
                {
                    var colName = $"C{i + 1}";
                    var colName1 = $"D{i + 1}";

                    if (i < columns.Count)
                    {
                        var row = columns[i];
                        fr.SetValue(colName, row.Field<string>("TenLNS"));

                        var sNG = row.Field<string>("Nganh");
                        if (!string.IsNullOrWhiteSpace(sNG))
                        {
                            sNG = sNG + Environment.NewLine + Environment.NewLine + Environment.NewLine + mlns.FirstOrDefault(x => x.sXauNoiMa == row.Field<string>("sXauNoiMa")).sMoTa;
                        }
                        else
                        {
                            sNG = Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "(+)";
                        }
                        fr.SetValue(colName1, sNG);

                        data.AsEnumerable()
                            .ToList()
                            .ForEach(r =>
                            {
                                var value = dt.AsEnumerable()
                                        .ToList()
                                        .Where(x => (string.IsNullOrWhiteSpace(row.Field<string>("sLNS")) || (x.Field<string>("sLNS") == row.Field<string>("sLNS") && (string.IsNullOrWhiteSpace(row.Field<string>("sXauNoiMa")) || x.Field<string>("sXauNoiMa") == row.Field<string>("sXauNoiMa")))) &&
                                                x.Field<string>("Id_DonVi") == r.Field<string>("Id_DonVi"))
                                        .Sum(x => x.Field<decimal>("TuChi", 0));
                                r[colName] = value;
                            });
                    }
                    else
                    {
                        fr.SetValue(colName, "");
                        fr.SetValue(colName1, "");
                    }
                }

                fr.AddTable(data);


                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable getTable()
        {
            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand("sp_dutoan_report_thnsnn_phancap", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = _nganh.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = _dvt.ToParamString(),
                }).Tables[0];
            }
        }        
        #endregion
    }
}
