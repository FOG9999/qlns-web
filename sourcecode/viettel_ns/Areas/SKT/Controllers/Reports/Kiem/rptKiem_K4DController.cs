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
    public class rptKiem_K4DController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/Kiem/rptKiem_K4D.xls";
        private const string _filePath_tg = "~/Report_ExcelFrom/SKT/Kiem/rptKiem_K4DTG.xls";

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
            var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec).ToSelectList("iID_MaDonVi", "sMoTa");
            var loaiList = SharedModels.GetLoaiHinhSKT().Select("Loai in ('1','2','4')").CopyToDataTable();

            var vm = new rptKiem_K4DViewModel()
            {
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
                DonViList = donviList,
                LoaiList = loaiList.ToSelectList("Loai", "Ten"),
            };

            return View(@"~/Areas\SKT\Views\Reports\Kiem\rptKiem_K4D.cshtml", vm);
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

            var filename = $"{(donvi.IsEmpty() ? id_phongban : id_donvi + "-" + donvi.ToStringAccent().Replace(" ", ""))}_K4D_{DateTime.Now.GetTimeStamp()}.{ext}";
            return Print(xls, ext, filename);
        }
        #region public
        public JsonResult Ds_DonVi(string id_PhongBan, int loai = 1)
        {
            var id = id_PhongBan == "s" ? PhienLamViec.iID_MaPhongBan : id_PhongBan;
            var data = _SKTService.Get_DonVi_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id);
            if (loai == 2)
            {
                data = _SKTService.Get_DonViTG_TheoBql_ExistData(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi, PhienLamViec.iID_MaPhongBan, id);
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
        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_loai == 2 ? _filePath_tg : _filePath));

            var donvi = id_donvi.IsEmpty() || id_donvi.Contains(",")
                ? string.Empty
                : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen;

            fr.SetValue(new
            {
                h1 = $"Đơn vị tính: {dvt.ToHeaderMoney()}",

                TenPhongBan = _nganSachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan).sMoTa,
                DonVi = donvi.IsEmpty() ? "Tổng hợp đơn vị" : $"Đơn vị: {donvi}",
                soPL = _loai == 1 ? "K4D" : _loai == 4 ? "K4DTH" : "",
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getDataSet();
            var dt = data.Tables[0];
            if (_loai != 2)
            {
                fr.SetValue(new
                {
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
        }

        private DataSet getDataSet()
        {
            var sql = "skt_report_thxdsktd";

            if (_loai == 2)
            {
                sql = "skt_report_k4dtg";
            }
            else if (_loai == 4)
            {
                sql = "skt_report_thxdsktdth";
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nam = PhienLamViec.NamLamViec,
                    dvt = dvt.ToParamString(),
                    donvi = id_donvi.ToParamString(),
                    phongban = id_phongban.ToParamString(),
                });
            }
        }
        #endregion
    }
}
