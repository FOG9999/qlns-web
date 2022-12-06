using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptKiem_K4Controller : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/Kiem/rptKiem_K4.xls";
        private const string _filePath_tg = "~/Report_ExcelFrom/SKT/Kiem/rptKiem_K4TG.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string id_phongban;
        private int _loai;
        private string id_donvi;
        private int dvt;

        public ActionResult Index()
        {

            var phongbanList = _nganSachService.GetPhongBansQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));
            var loaiList = SharedModels.GetLoaiHinhSKT().Select("Loai in ('1','2','4')").CopyToDataTable();

            var vm = new rptKiem_K4ViewModel()
            {
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                LoaiList = loaiList.ToSelectList("Loai", "Ten"),
            };

            return View(@"~/Areas\SKT\Views\Reports\Kiem\rptKiem_K4.cshtml", vm);
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
            string id_phongban, 
            string id_donvi, 
            string ext = "pdf", 
            int loai = 1, 
            int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly
            if (id_donvi.IsEmpty())
                id_donvi = PhienLamViec.iID_MaDonVi;
            this.id_phongban = id_phongban == "s" ? PhienLamViec.iID_MaPhongBan : id_phongban;
            this.id_donvi = id_donvi;
            this._loai = loai;
            this.dvt = dvt;

            var xls = createReport();

            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
              ? string.Empty
              : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            var filename = $"{(donvi.IsEmpty() ? id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_K4_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls, ext, filename);
        }        
        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_loai == 2 ? _filePath_tg : _filePath));           

            fr.SetValue(new
            {
                h1 = $"Đơn vị tính: {dvt.ToHeaderMoney()}",
                nam = int.Parse(PhienLamViec.iNamLamViec),
                TenPhongBan = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,

                soPL = _loai == 1 ? "K4" : _loai == 4 ? "K4TH" : "",
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getDataSet();
            var dv = data.Tables[0];
            var dt = data.Tables[1];
            if (_loai != 2)
            {
                fr.SetValue(new
                {
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
        }

        private DataSet getDataSet()
        {
            var sql = "skt_report_thxdskt";

            if (_loai == 2)
            {
                sql = "skt_report_k4tg";
            }           
            else if (_loai == 4)
            {
                sql = "skt_report_thxdsktth";
            }
            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                    donvis = id_donvi.ToParamString(),
                    phongban = id_phongban.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
