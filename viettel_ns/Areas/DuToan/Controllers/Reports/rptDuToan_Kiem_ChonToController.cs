using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.DuToan.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.DuToan.Models
{
    public class rptDuToan_Kiem_ChonToViewModel
    {
        public SelectList PhongBanList { get; set; }

        public SelectList ToList { get; set; }

    }
}

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_Kiem_ChonToController : FlexcelReportController
    {
        #region var def 
        public string _viewPath = "~/Areas/DuToan/Views/Report/";
        private string _filePath = "~/Report_ExcelFrom/DuToan/rptDuToan_Kiem_ChonTo.xls";
        private string _filePath_to2 = "~/Report_ExcelFrom/DuToan/rptDuToan_Kiem_ChonTo_To2.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private string _idphongban;
        private int _to = 1;
        private int _nam;
        private int _dvt = 1000;
        private int _columnCount = 6;


        public ActionResult Index()
        {
            var phongbanList = _ngansachService.GetPhongBanQuanLyNS(GetPhongBanId(PhienLamViec.iID_MaPhongBan));

            var vm = new rptDuToan_Kiem_ChonToViewModel
            {
                PhongBanList = phongbanList.ToSelectList("sKyHieu", "sMoTa", "-1", "-- Chọn phòng ban --"),
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string id_phongban,
            int to = 1,
            string ext = "pdf")
        {
            _idphongban = id_phongban;
            _dvt = Request.GetQueryStringValue("dvt", 1000);
            _nam = int.Parse(PhienLamViec.iNamLamViec);
            _to = to;
            _filePath = _to == 1 ? _filePath : _filePath_to2;

            var xls = createReport();
            return Print(xls, ext);
        }

        public JsonResult Ds_To(string id_phongban)
        {
            var data = getTable(id_phongban);
            var nganhs = data.SelectDistinct("donvi", "iID_MaDonVi");
            return ds_ToIn(nganhs.Rows.Count == 0 ? 0 : nganhs.Rows.Count + 1, _columnCount);
        }

        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            loadData(fr);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(_filePath));

            fr.UseCommonValue()
              .SetValue(new
              {
                  header2 = $"Đơn vị tính: {_dvt.ToStringDvt()}     Tờ: {_to}",
                  TieuDe1 = $"Dự toán chi ngân sách năm {PhienLamViec.iNamLamViec}",
                  TieuDe2 = "Phần Bảo hiểm xã hội, y tế",
                  TenPhongBan = PhienLamViec.sTenPhongBan,
              })
              .UseChuKy(Username)
              .UseChuKyForController(this.ControllerName())
              .UseForm(this).Run(xls, _to);

            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var dt = CacheService.Default.CachePerRequest(getCacheKey(),
                () => getTable(_idphongban),
                CacheTimes.OneMinute,
                false);

            var data = dt.SelectDistinct("ChiTiet", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa");

            var dtX = dt.SelectDistinct("DonVi", "iID_MaDonVi,sTenDonVi");
            // them cot
            for (int i = 1; i <= _columnCount; i++)
            {
                data.Columns.Add($"C{i}", typeof(decimal));
            }

            var columns = new List<DataRow>();
            if (_to == 1)
            {
                var r = dtX.NewRow();
                r["sTenDonVi"] = "Tổng cộng".ToUpper();
                dtX.Rows.InsertAt(r, 0);
            }


            columns = _to == 1 ?
                dtX.AsEnumerable().Take(_columnCount).ToList() :
                dtX.AsEnumerable().Skip((_to - 1) * _columnCount - 1).Take(_columnCount).ToList();

            for (int i = 0; i < _columnCount; i++)
            {
                var colName = $"C{i + 1}";
                var colIndex = $"i{i + 1}";
                if (i < columns.Count)
                {

                    var row = columns[i];
                    fr.SetValue(colName, row.Field<string>("sTenDonVi"));

                    data.AsEnumerable()
                       .ToList()
                       .ForEach(r =>
                       {
                           // lay tong cong
                           if (row.Field<string>("iID_MaDonVi").IsEmpty())
                           {
                               var value = dt.AsEnumerable()
                                     .ToList()
                                     .Where(x => x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa"))
                                     .Sum(x => x.Field<decimal>("rTuChi", 0));
                               r[colName] = value;
                           }
                           else
                           {
                               var rowValue = dt.AsEnumerable()
                                     .ToList()
                                     .FirstOrDefault(x => x.Field<string>("iID_MaDonVi") == row.Field<string>("iID_MaDonVi") &&
                                                  x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa"));
                               if (rowValue != null)
                                   r[colName] = rowValue.Field<decimal>("rTuChi");
                           }
                       });
                    fr.SetValue(colIndex, (_to - 1) * _columnCount + i);
                }
                else
                {
                    fr.SetValue($"C{i + 1}", "");
                    fr.SetValue(colIndex, "");
                }

            }

            //fr.AddTable(data);
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private DataTable getTable(string id_phongban)
        {
            return getTable(PhienLamViec.iNamLamViec, id_phongban, PhienLamViec.iID_MaDonVi);
        }

        private DataTable getTable(string namLamViec, string id_phongban, string id_donvi)
        {
            var sql = FileHelpers.GetSqlQuery("rptDuToan_Kiem_ChonTo.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var dt = conn.GetTable(sql,
                    new
                    {
                        namLamViec,
                        id_phongban = id_phongban.ToParamString(),
                        id_donvi = id_donvi.ToParamString(),
                        dvt = _dvt,
                    });

                return dt;
            }
        }

        private string getCacheKey()
        {
            return $"{this.ControllerName()}_{Username}_{_nam}_{_idphongban}_{_dvt}";
        }

        #endregion
    }
}
