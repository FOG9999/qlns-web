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

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptKiem_CauHinhNCMLNSController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/SKT/Kiem/rptKiem_CauHinhNCMLNS.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _id_donvi;
        public ActionResult Index()
        {

            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptKiem_CauHinhNCMLNSViewModel()
            {
                DonViList = donviList,
            };

            return View(@"~/Areas\SKT\Views\Reports\Kiem\rptKiem_CauHinhNCMLNS.cshtml", vm);
        }
        public ActionResult Print(
            string Id_DonVi,
            string ext)
        {
            _id_donvi = Id_DonVi;
            var xls = createReport();
            return Print(xls, ext);
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
            xls.Open(Server.MapPath(_filePath));

            var tenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _id_donvi).sTen;
            
            fr.SetValue(new
            {
                TieuDe1 = getTieuDe(),
                date = DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year,
                DonVi = tenDonVi
            });

            fr.UseForm(this).Run(xls);
            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = getDataSet(PhienLamViec.iNamLamViec);            
            fr.SetValue(new
            {
                Cot1 = data.Tables[0].Rows[0]["N_2"],
                Cot2 = data.Tables[0].Rows[0]["N_1"],
            });
            fr.AddTable("ChiTiet", data.Tables[1]);
        }

        private DataSet getDataSet(string NamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand("skt_checkmapmlns", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                return cmd.GetDataset(new
                {
                    nam = NamLamViec,
                    donvi = _id_donvi.ToParamString(),
                    phongban = PhienLamViec.iID_MaPhongBan.ToParamString(),
                });
            }
        }

        private string getTieuDe()
        {
            var tieude = $"Kiểm tra cấu hình mục lục ngân sách với mục lục số kiểm tra năm {PhienLamViec.iNamLamViec}";

            return tieude;
        }

        #endregion
    }
}
