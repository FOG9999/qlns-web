using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_TK02ViewModel : ReportModels
    {
        public SelectList PhongBanList { get; set; }
    }

    public class rptNCSKT_TK02Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/TrinhKy/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/TrinhKy/rptNCSKT_TK02/rptNCSKT_TK02.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_nganh;
        private int _dvt;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptNCSKT_TK02ViewModel()
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa"),
            };

            var view = _viewPath + "rptNCSKT_TK02.cshtml";

            return View(view, vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>            
        public ActionResult Print(string id_nganh, string id_phongban, string ext = "pdf", int dvt = 1000)
        {
            this._id_phongban = id_phongban;
            this._id_nganh = id_nganh;
            this._dvt = dvt;

            var xls = createReport();
            
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var nganh = id_nganh.ToList().Count > 1
                      ? "(Tổng hợp các ngành)"
                      : _nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, id_nganh).sTenNganh;
                var filename = $"Xác_nhận_số_liệu_3_năm_{(nganh.IsEmpty() ? "TongHop" : id_nganh + "-" + nganh.ToStringAccent().Replace(" ", ""))}_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls[true], ext, filename);
            }
        }
        public JsonResult DS_Nganh(string id_phongban)
        {
            var data = _SKTService.Get_Nganh_ExistData(PhienLamViec.iNamLamViec, 1, null, null, id_phongban);
            var vm = new ChecklistModel("Id_Nganh", data.ToSelectList("Id", "Ten"));
            return ToCheckboxList(vm);
        }
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
                var dictieude = _SKTService.GetTieuDeDuLieuNamTruoc(PhienLamViec.NamLamViec);
                fr.UseCommonValue()
                  .SetValue(new
                  {
                      header1 = _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                      header2 = $"Ngành: {_nganSachService.Nganh_Get(PhienLamViec.iNamLamViec, _id_nganh).sTenNganh}",
                      header3 = $"Đơn vị tính: {_dvt.ToStringDvt()}",
                      tieude1 = dictieude[1],
                      tieude2 = dictieude[2],
                      tieude3 = dictieude[3],
                  })
                  .UseChuKy(Username)
                  .UseChuKyForController(this.ControllerName());

                xls.Open(Server.MapPath(_filePath));

                fr.UseForm(this).Run(xls);

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
                _SKTService.FillDataTable_NC(fr, dt);
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        public DataTable getTable()
        {
            #region get data
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_report_tk02", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@nganh", _id_nganh.ToParamString());

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        #endregion
    }
}
