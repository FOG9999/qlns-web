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

namespace VIETTEL.Models
{
    public class rptDuToanBS_TongHopDotViewModel
    {
        public SelectList DotList { get; set; }
    }
}

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptDuToanBS_TongHopDotController : FlexcelReportController
    {
        #region var def
        private string _viewPath = "~/Views/Report_Views/DuToanBS/";
        private string _filePath = "~/Report_ExcelFrom/DuToanBS/rptDuToanBS_TongHopDot.xls";

        private string _iID_MaDot;
        private int _dvt = 1000;
        private int _to = 1;
        private int _columnCount = 6;

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _dutoanbsService = DuToanBsService.Default;


        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;

            var dtDot = _dutoanbsService.GetDots_Gom(PhienLamViec.iNamLamViec, Username);

            var vm = new rptDuToanBS_TongHopDotViewModel
            {
                DotList = dtDot.ToSelectList("iID_MaDot", "sMoTa", DateTime.Now.ToString("dd/MM/yyyy"), "-- Chọn đợt --"),
            };

            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(    
            string iID_MaDot,       
            int to,
            string ext = "pdf")
        {
            _iID_MaDot = iID_MaDot;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _to = to;

            var xls = createReport();
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                return Print(xls[true], ext);
            }
        }

        public JsonResult Ds_ToIn(
            string iID_MaDot)
        {
            try
            {
                var data = getDataSet(iID_MaDot).Tables[1];
                return ds_ToIn(data.Rows.Count + 1,_columnCount);
            }
            catch (Exception ex)
            {
                //throw;
            }

            return ds_ToIn(0);
        }

        #endregion

        #region private methods

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

                fr.UseCommonValue()
                  .SetValue(new
                  {
                      phongban = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
                      header2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to}",
                      dot = _dutoanbsService.GetDotGom(_iID_MaDot).dNgayChungTu.ToString("dd/MM/yyyy") + " - " + _dutoanbsService.GetDotGom(_iID_MaDot).sNoiDung.ToString(),

                      Nam = PhienLamViec.NamLamViec,
                      To = _to,
                  })
                  .UseChuKy(Username)
                  .UseChuKyForController(this.ControllerName());

                xls.Open(Server.MapPath(_filePath));

                fr.UseForm(this).Run(xls, _to);

                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }
        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var dt = getDataSet();

            if (dt.Tables[0].Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var data = dt.Tables[0].SelectDistinct("ChiTiet", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa");
                var dtX = dt.Tables[1];

                var columns = new List<DataRow>();

                if (dtX.Rows.Count > 1)
                {
                    var nr = dtX.NewRow();
                    nr["Id"] = "";
                    nr["sTen"] = "Tổng cộng";
                    dtX.Rows.InsertAt(nr, 0);
                }

                columns =
                    _to == 1 ?
                    dtX.AsEnumerable().Take(_columnCount).ToList() :
                    dtX.AsEnumerable().Skip((_to - 1) * _columnCount).Take(_columnCount).ToList();

                for (int i = 0; i < _columnCount; i++)
                {
                    data.Columns.Add(new DataColumn($"C{i + 1}", typeof(decimal)));
                }

                for (int i = 0; i < _columnCount; i++)
                {
                    var colName = $"C{i + 1}";

                    if (i < columns.Count)
                    {
                        var row = columns[i];

                        fr.SetValue($"C{i + 1}", row["sTen"]);

                        data.AsEnumerable()
                            .ToList()
                            .ForEach(r =>
                            {
                                var value = dt.Tables[0].AsEnumerable()
                                        .ToList()
                                        .Where(x => (x.Field<string>("iID_MaDonVi") == row.Field<string>("Id") || string.IsNullOrEmpty(row.Field<string>("Id"))) &&
                                                     x.Field<string>("sLNS") == r.Field<string>("sLNS") &&
                                                     x.Field<string>("sL") == r.Field<string>("sL") &&
                                                     x.Field<string>("sK") == r.Field<string>("sK") &&
                                                     x.Field<string>("sM") == r.Field<string>("sM") &&
                                                     x.Field<string>("sTM") == r.Field<string>("sTM") &&
                                                     x.Field<string>("sTTM") == r.Field<string>("sTTM") &&
                                                     x.Field<string>("sNG") == r.Field<string>("sNG"))
                                        .Sum(x => x.Field<decimal>("C", 0));
                                r[colName] = value;
                            });
                    }
                    else
                    {
                        fr.SetValue($"C{i + 1}", "");
                    }
                }

                FillDataTableLNS(fr, data, FillLNSType.LNS, PhienLamViec.iNamLamViec);

                fr.AddTable("ChiTiet", data);

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public DataSet getDataSet(string dot = null)
        {
            #region get data
            var sql = "dtbs_tonghopdotgom";    

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@id", dot == null ? _iID_MaDot.ToParamString() : dot.ToParamString());

                var dt = cmd.GetDataset();
                return dt;
            }
            #endregion   
        }
        #endregion
    }
}
