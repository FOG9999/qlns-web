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
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_TK01ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
    public class rptNCSKT_TK01Controller : FlexcelReportController
    {
        #region var def
        public string _viewPath = "~/Areas/SKT/Views/Reports/TrinhKy/";
        private string _filePath = "~/Areas/SKT/FlexcelForm/TrinhKy/rptNCSKT_TK01/rptNCSKT_TK01.xls";

        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private string _id_donvi;
        private int _dvt = 1000;

        public ActionResult Index()
        {
            var phongBanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptNCSKT_TK01ViewModel
            {
                PhongBanList = phongBanList.ToSelectList("sKyHieu", "sMoTa"),
                DonViList = donviList,
            };

            var view = _viewPath + "rptNCSKT_TK01.cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods
        public JsonResult Ds_DonVi(string id_phongban)
        {
            var data = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 1, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id_phongban);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten"));
            return ToCheckboxList(vm);
        }
        public ActionResult Print(
            string id_phongban,
            string id_donvi,
            string ext = "pdf")
        {
            _id_phongban = id_phongban;
            _id_donvi = id_donvi;
            _dvt = Request.GetQueryStringValue("dvt", 1000);

            var xls = createReport();
            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var filename = $"BC_NhuCau_NSSD {(id_donvi.IsEmpty() ? "TongHop" : id_donvi + "-" + _nganSachService.GetDonVi(PhienLamViec.iNamLamViec,id_donvi).sTen)}_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
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
                var dictieude = _SKTService.GetTieuDeDuLieuNamTruoc(PhienLamViec.NamLamViec);
                fr.UseCommonValue()
                  .SetValue(new
                  {
                      header1 = _nganSachService.GetPhongBanById(_id_phongban).sMoTa,
                      header2 = $"Đơn vị: {_id_donvi} - {_nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen}",
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
            using (var cmd = new SqlCommand("sp_ncskt_report_tk01", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nam", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@dvt", _dvt);
                cmd.Parameters.AddWithValue("@phongban", _id_phongban.ToParamString());
                cmd.Parameters.AddWithValue("@donvi", _id_donvi.ToParamString());

                var dt = cmd.GetDataset().Tables[0];
                return dt;
            }
            #endregion   
        }
        #endregion
    }
}
