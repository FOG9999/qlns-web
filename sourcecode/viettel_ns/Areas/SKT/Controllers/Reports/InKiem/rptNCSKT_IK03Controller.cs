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
    public class rptNCSKT_IK03ViewModel
    {
        public SelectList LoaiList { get; set; }
    }
    public class rptNCSKT_IK03Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/InKiem/";
        private const string _filePath = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK03/rptNCSKT_IK03.xls";
        private const string _filePath_tg = "~/Areas/SKT/FlexcelForm/InKiem/rptNCSKT_IK03/rptNCSKT_IK03TG.xls";

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

            var vm = new rptNCSKT_IK03ViewModel()
            {
                LoaiList = loaiList.ToSelectList("Loai", "Ten"),
            };

            var view = _viewPath + "rptNCSKT_IK03.cshtml";

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
            int loai = 1, 
            string ext = "pdf",
            int dvt = 1000)
        {
            this._id_phongban = PhienLamViec.iID_MaPhongBan;
            this._id_donvi = PhienLamViec.iID_MaDonVi;
            this._loai = loai;
            this._dvt = dvt;

            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var filename = $"{_id_phongban}_IK03_{DateTime.Now.GetTimeStamp()}.{ext}";
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
                xls.Open(Server.MapPath(_loai == 2 ? _filePath_tg : _filePath));           

                fr.SetValue(new
                {
                    h1 = $"Đơn vị tính: {_dvt.ToHeaderMoney()}",
                    nam = int.Parse(PhienLamViec.iNamLamViec),
                    TenPhongBan = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,

                    soPL = _loai == 1 ? "K4" : "",
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
            else {
                var dv = data.Tables[0];
                var dt = data.Tables[1];
                if (_loai != 2)
                {
                    fr.SetValue(new
                    {
                        N3 = data.Tables[0].Rows[0]["N_3"],
                        N2 = data.Tables[0].Rows[0]["N_2"],
                        N1 = data.Tables[0].Rows[0]["N_1"],
                    });
                    dv = data.Tables[1];
                    dt = data.Tables[2];
                } 
                _SKTService.AddOrdinalsNum(dv, 3);
                fr.AddTable("DonVi", dv);
                fr.AddTable("ChiTiet", dt.SelectDistinct("ChiTiet", "KyHieu,DonVi"));
                fr.AddRelationship("DonVi", "ChiTiet", "DonVi", "DonVi");
                fr.AddTable("Data", dt);

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataSet getDataSet()
        {
            var sql = "sp_ncskt_report_ik03";

            if (_loai == 2)
            {
                sql = "sp_ncskt_report_ik03_tg";
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
