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

namespace VIETTEL.Areas.Admin.Controllers
{
    public class rptAdmin_IK02Controller : FlexcelReportController
    {
        private string _filePath = "~/Areas/Admin/FlexcelForm/rptAdmin_IK02.xls";
        
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        
        public ActionResult Print(
            string ext = "pdf")
        {
            var xls = CreateReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var filename = $"Danh_sách_đơn_vị_đã_nhập_số_liệu_qtqs_{DateTime.Now.GetTimeStamp()}.{ext}";
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
                var Khoi = data.SelectDistinct("Khoi", "Id_PhongBan,TenKhoi");
                Khoi.AddOrdinalsNum(2);
                data.AddOrdinalsNum(3, "Id_PhongBan");
                fr.AddTable("ChiTiet", data);
                fr.AddTable("Khoi", Khoi);
                fr.AddRelationship("Khoi", "ChiTiet", "Id_PhongBan".Split(','), "Id_PhongBan".Split(','));

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataTable GetData()
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_qtqs_dsdvidanhap", conn))
            {
                cmd.AddParams(new
                {
                    nam = PhienLamViec.iNamLamViec,
                });

                cmd.CommandType = CommandType.StoredProcedure;
                var dt = cmd.GetTable();
                return dt;
            }
        }

        #endregion
    }
}
