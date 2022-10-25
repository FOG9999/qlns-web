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

namespace VIETTEL.Areas.Admin.Controllers
{
    public class rptAdmin_IK04Controller : FlexcelReportController
    {
        private string _path = "~/Areas/Admin";
        private string _filePath = "~/Areas/Admin/FlexcelForm/rptAdmin_IK04.xls";
        
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        private string _loai;

        public ActionResult Print(
            int loai = 1,
            string ext = "pdf")
        {
            _loai = loai == 1 ? "1" : "1,2";

            var xls = CreateReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var tail = loai == 1 ? "theo_skt" : "cả_số_xin_bổ_sung";
                var filename = $"Tổng_hợp_số_liệu_dự_toán_{tail}_đã_nhập_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }
        }

        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private Dictionary<bool, ExcelFile> CreateReport()
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);

            var check = LoadData(fr);

            if (check.ContainsKey(false))
            {
                return new Dictionary<bool, ExcelFile>
                {
                    {false, xls}
                };
            }
            else
            {
                xls.Open(Server.MapPath(_filePath));

                fr.SetValue(new
                {
                    nam = PhienLamViec.NamLamViec,
                });

                fr.Run(xls);

                var count = xls.TotalPageCount();
                if (count > 1)
                {
                    xls.AddPageFirstPage();
                }

                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> LoadData(FlexCelReport fr)
        {
            var data = GetData();
            if (data.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var khoi = data.SelectDistinct("Khoi", "iID_MaPhongBan");
                khoi.AddOrdinalsNum(2);
                data.AddOrdinalsNum(3, "iID_MaPhongBan");
                fr.AddTable("DonVi", data);
                fr.AddTable("Khoi", khoi);
                fr.AddRelationship("Khoi", "DonVi", "iID_MaPhongBan", "iID_MaPhongBan");

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable GetData()
        {
            var sql = FileHelpers.GetSqlQueryPath(_path + "/Sql/dt_chitieu_thcuc_phuluc_ngang_tonghop.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.AddParams(new
                {
                    iNamLamViec = PhienLamViec.iNamLamViec,
                    loai = _loai.ToParamString(),
                    dvt = 1000,
                });

                var dt = cmd.GetTable();
                return dt;
            }
        }

        #endregion
    }
}
