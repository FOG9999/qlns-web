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
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptSKT_BC05THController : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/BaoCao/";
        private string _filePath = "~/Report_ExcelFrom/SKT/BaoCao/rptSKT_BC05.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _loaiBC = 1;
        private int _to = 1;
        private int _dvt = 1000;
        private int _columnCount = 10;              

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptSKT_BC05ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
            };

            var view = _viewPath + "rptSKT_BC05TH.cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string Id_PhongBan,
            int loaiBC,
            int to,
            string ext = "pdf")
        {
            _id_phongban = Id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : Id_PhongBan;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _loaiBC = loaiBC;
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
                      header1 = _id_phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                      header2 = _loaiBC == 1 ? $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to} - KT-ĐCT" : $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ: {_to} - KT-ĐCG",

                      T = _loaiBC == 1 ? "TĂNG" : "GIẢM",
                      To = _to,
                      mau = _loaiBC == 1 ? "KT-ĐCT" : "KT-ĐCG",
                  })
                  .UseChuKy(Username)
                  .UseChuKyForController(this.ControllerName());

                xls.Open(Server.MapPath(_filePath));

                fr.UseForm(this).Run(xls,_to);
               
                xls.MergeH(9, 6, 10);

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
                var data = dt.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu,MoTa");
                var dtX = dt.SelectDistinct("X", "Id_DonVi,TenDonVi");
                                               
                for (int i = 0; i < _columnCount; i++)
                {
                    data.Columns.Add(new DataColumn($"C{i+1}", typeof(decimal)));
                }

                var columns = new List<DataRow>();                

                columns = dtX.AsEnumerable().ToList();

                for (int i = 0; i < _columnCount; i++)
                {
                    var colName = $"C{i+1}";
                    if (i < columns.Count)
                    {
                        var row = columns[i];

                        data.AsEnumerable()
                           .ToList()
                           .ForEach(r =>
                           {
                               var value = dt.AsEnumerable()
                                       .ToList()
                                       .Where(x => (x.Field<string>("Id_DonVi") == row.Field<string>("Id_DonVi")) &&
                                                    x.Field<string>("KyHieu") == r.Field<string>("KyHieu"))
                                       .Sum(x => x.Field<double>("C", 0));
                               r[colName] = value;                               
                           });
                    }    
                }

                fr.AddTable("DonVi", dtX);
                _SKTService.FillDataTable_NC(fr, data);
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
        public DataTable getTable()
        {
            #region get data
            var donvis = "," + PhienLamViec.iID_MaDonVi;
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("skt_report_bc05th", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.NamLamViec);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@donvis", donvis.Split(',').Skip((_to-1)*(_columnCount)).Take(_columnCount).Join(","));
                cmd.Parameters.AddWithValue("@loai", _loaiBC);
                cmd.Parameters.AddWithValue("@b", PhienLamViec.iID_MaPhongBan);

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        public JsonResult Ds_To(string id_PhongBan)
        {
            var count = PhienLamViec.DonViList.Count;
            return ds_ToIn(count > 0 ? count + 1 : 0, _columnCount);
        }        
        #endregion
    }
}
