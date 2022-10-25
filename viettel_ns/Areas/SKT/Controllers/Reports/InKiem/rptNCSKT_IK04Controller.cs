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
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_IK04ViewModel
    {
        public SelectList LoaiList { get; set; }
    }
    public class rptNCSKT_IK04Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/InKiem/";
        private const string _filePath = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK04/rptNCSKT_IK04.xls";
        private const string _filePath_tg = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK04/rptNCSKT_IK04TG.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_phongban;
        private int _loai;
        private string _id_donvi;
        private int _dvt;

        public ActionResult Index()
        {
            var loaiList = SharedModels.GetLoaiHinhSKT().Select("Loai in ('1','2')").CopyToDataTable();

            var vm = new rptNCSKT_IK04ViewModel()
            {
                LoaiList = loaiList.ToSelectList("Loai", "Ten"),
            };

            var view = _viewPath + "rptNCSKT_IK04.cshtml";

            return View(view, vm);
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
            string id_donvi, 
            string ext = "pdf", 
            int loai = 1, 
            int dvt = 1000)
        {
            this._id_phongban = PhienLamViec.iID_MaPhongBan;
            this._id_donvi = id_donvi.IsEmpty() ? PhienLamViec.iID_MaDonVi : id_donvi;
            this._loai = loai;
            this._dvt = dvt;

            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {

                var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
                    ? string.Empty
                    : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;

                var filename = $"{(donvi.IsEmpty() ? _id_phongban : _id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_IK04_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }
        }
        #region public
        public JsonResult Ds_DonVi(int loai = 1)
        {
            var data = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 1, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, PhienLamViec.iID_MaPhongBan);
            if (loai == 2)
            {
                data = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 3, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, PhienLamViec.iID_MaPhongBan);
            }
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten"));
            return ToCheckboxList(vm);
        }
        #endregion
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
                xls.Open(Server.MapPath(_loai == 2 ? _filePath_tg : _filePath));

                var donvi = _id_donvi.IsEmpty() || _id_donvi.Contains(",")
                    ? string.Empty
                    : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;

                fr.SetValue(new
                {
                    h1 = $"Đơn vị tính: {_dvt.ToHeaderMoney()}",

                    TenPhongBan = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
                    DonVi = donvi.IsEmpty() ? "Tổng hợp đơn vị" : $"Đơn vị: {donvi}",
                    soPL = _loai == 1 ? "IK4" : "",
                });

                fr.UseForm(this).Run(xls);

                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var data = getDataSet();
            if (data == null)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
                else
                {
                    var dt = data.Tables[0];
                if (_loai != 2)
                {
                    fr.SetValue(new
                    {
                        N3 = data.Tables[0].Rows[0]["N_3"],
                        N2 = data.Tables[0].Rows[0]["N_2"],
                        N1 = data.Tables[0].Rows[0]["N_1"],
                    });
                    dt = data.Tables[1];
                }
                var dtDonVi = dt.SelectDistinct("DonVi", "Id_DonVi,Nganh,TenNganh");
                var dtNganh = dt.SelectDistinct("Nganh", "Nganh,TenNganh");
                fr.AddTable("ChiTiet", dt);
                fr.AddTable("DonVi", dtDonVi);
                fr.AddTable("Nganh", dtNganh);
                fr.AddRelationship("Nganh", "DonVi", "Nganh,TenNganh".Split(','), "Nganh,TenNganh".Split(','));
                fr.AddRelationship("DonVi", "ChiTiet", "Id_DonVi,Nganh,TenNganh".Split(','), "Id_DonVi,Nganh,TenNganh".Split(','));
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataSet getDataSet()
        {
            var sql = "sp_ncskt_report_ik04";

            if (_loai == 2)
            {
                sql = "sp_ncskt_report_ik04_tg";
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                var ds = cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    dvt = _dvt.ToParamString(),
                    donvis = _id_donvi.ToParamString(),
                    phongban = _id_phongban.ToParamString(),
                });

                return ds;
            }
        }
        #endregion
    }
}
