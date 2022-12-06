using System;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Viettel.Data;
using Viettel.Extensions;
using Viettel.Services;
using VIETTEL.Application.Flexcel;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptTongHop_GiaoBan_DonViViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.TongHopNganSach
{
    public partial class rptTongHop_GiaoBan_DonViController : FlexcelReportController
    {
        private string _viewPath = "~/Views/Report_Views/TongHopNganSach/";

        public ActionResult Index()
        {

            var dtPhongBan = _nganSachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptTongHop_GiaoBan_DonViViewModel()
            {
                iNamLamViec = PhienLamViec.iNamLamViec,
                DonViList = PhienLamViec.DonViList.ToSelectList(),
                PhongBanList = GetPhongBanId(PhienLamViec.iID_MaPhongBan).IsEmpty() ?
                    dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "<-- Tổng hợp cục -->") :
                    dtPhongBan.ToSelectList("iID_MaPhongBan", "sMoTa"),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

    }

    public partial class rptTongHop_GiaoBan_DonViController : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string _filePath = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_GiaoBan_DonVi.xls";
        private const string _filePath_A4 = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_GiaoBan_DonVi_A4.xls";

        private const string _filePath_Bql = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_GiaoBan_DonVi_PhongBan.xls";
        private const string _filePath_Bql_A4 = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_GiaoBan_DonVi_PhongBan_A4.xls";

        private const string _filePath_CT_Bql_A4 = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_DonVi_PhongBan_A4.xls";

        private int _loaiBaoCao;

        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(string iID_MaDonVi, string iNamLamViec, string iID_MaPhongBan,
            int loaiBaoCao,
            string ext = "pdf")
        {

            _loaiBaoCao = loaiBaoCao;

            iNamLamViec = iNamLamViec ?? ReportModels.LayNamLamViec(Username);
            var xls = createReport(iNamLamViec, iID_MaDonVi, iID_MaPhongBan);
            return Print(xls, ext);
        }

        #endregion

        private ExcelFile createReport(string nam, string iID_MaDonVi, string iID_MaPhongBan)
        {
            var xls = new XlsFile(true);
            xls.AddUserDefinedFunction(TUserDefinedFunctionScope.Local, TUserDefinedFunctionLocation.Internal, new ToPercent());
            var fr = new FlexCelReport();

            if (_loaiBaoCao == 8 || _loaiBaoCao == 84 || _loaiBaoCao == 841)           // chi tiet theo B
            {
                #region load report

                xls.Open(Server.MapPath(_loaiBaoCao == 8 ? _filePath_Bql : _loaiBaoCao == 84 ?_filePath_Bql_A4 : _filePath_CT_Bql_A4));

                iID_MaPhongBan = iID_MaPhongBan ?? PhienLamViec.iID_MaPhongBan;

                var data = _loaiBaoCao == 841 ? getDataTablect_Bql(nam, iID_MaDonVi, iID_MaPhongBan) : getDataTable_Bql(nam, iID_MaDonVi, iID_MaPhongBan);
                FillDataTable(fr, data, "PhongBan,sMoTa_PhongBan sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM sNG", nam);

                #endregion
            }
            else if (_loaiBaoCao == 9 || _loaiBaoCao == 94)      // chi tiet tong hop tat ca Bql
            {
                #region load report

                xls.Open(Server.MapPath(_loaiBaoCao == 9 ? _filePath : _filePath_A4));
                //fr.SetUserFunction("ToPercent", new TToPercentImpl(xls));

                iID_MaPhongBan = iID_MaPhongBan ?? PhienLamViec.iID_MaPhongBan;

                var data = getDataTable(nam, iID_MaDonVi, iID_MaPhongBan);
                FillDataTable(fr, data, "sLNS1 sLNS3 sLNS5 sLNS sL,sK sM sTM sTTM sNG", nam);

                #endregion
            }

            var tenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, nam);

            tenDonVi = tenDonVi.IsEmpty() ? "(Tổng hợp tất cả đơn vị)" : $"Đơn vị: {tenDonVi}";


            xls.Recalc(true);
            var tenPhongBan = string.Empty;
            if (iID_MaPhongBan.Contains(",") || iID_MaPhongBan.IsEmpty("-1"))
            {
            }
            else
            {
                tenPhongBan = _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa.ToUpper();
            }
            
            fr.UseCommonValue()
                .SetValue(new
                {
                    h1 = tenDonVi,
                    nam,
                    tenDonVi,
                    tenPhongBan
                })
              .UseForm(this)
              .UseChuKy()
              .Run(xls);

            return xls;
        }




        private DataTable getDataTable(string nam, string iID_MaDonVi, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_THGB_NG.sql");

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("iNamLamViec", nam);
                cmd.Parameters.AddWithValue("iID_MaDonVi", iID_MaDonVi.ToParamString());

                var dt = cmd.GetTable();
                return dt;
            }

            #endregion
        }

        private DataTable getDataTable_Bql(string nam, string iID_MaDonVi, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_THGB_NG_Bql.sql");

            iID_MaPhongBan = iID_MaPhongBan.IsEmpty("-1") ?
                "05,06,07,08,10,16,17" :
                iID_MaPhongBan;

            var id_phongban = iID_MaPhongBan.ToList();

            DataTable dt = null;


            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                id_phongban.ForEach(x =>
                {
                    DataTable t = null;
                    using (var cmd = new SqlCommand(sql, conn))
                    {

                        cmd.Parameters.AddWithValue("iID_MaPhongBan", x.ToParamString());
                        cmd.Parameters.AddWithValue("iNamLamViec", nam);
                        cmd.Parameters.AddWithValue("iID_MaDonVi", iID_MaDonVi.ToParamString());

                        t = cmd.GetTable();
                    }

                    if (dt == null) dt = t;
                    else
                    {
                        t.AsEnumerable().ToList()
                        .ForEach(r =>
                        {
                            var row = dt.NewRow();
                            row.ItemArray = r.ItemArray;
                            dt.Rows.Add(row);
                        });
                    }

                });


            }


            return dt;

            #endregion
        }

        private DataTable getDataTablect_Bql(string nam, string iID_MaDonVi, string iID_MaPhongBan)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_Ct_Bql.sql");

            iID_MaPhongBan = iID_MaPhongBan.IsEmpty("-1") ?
                "05,06,07,08,10,16,17" :
                iID_MaPhongBan;

            var id_phongban = iID_MaPhongBan.ToList();

            DataTable dt = null;


            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                id_phongban.ForEach(x =>
                {
                    DataTable t = null;
                    using (var cmd = new SqlCommand(sql, conn))
                    {

                        cmd.Parameters.AddWithValue("iID_MaPhongBan", x.ToParamString());
                        cmd.Parameters.AddWithValue("iNamLamViec", nam);
                        cmd.Parameters.AddWithValue("iID_MaDonVi", iID_MaDonVi.ToParamString());

                        t = cmd.GetTable();
                    }

                    if (dt == null) dt = t;
                    else
                    {
                        t.AsEnumerable().ToList()
                        .ForEach(r =>
                        {
                            var row = dt.NewRow();
                            row.ItemArray = r.ItemArray;
                            dt.Rows.Add(row);
                        });
                    }

                });


            }


            return dt;

            #endregion
        }

    }

}
