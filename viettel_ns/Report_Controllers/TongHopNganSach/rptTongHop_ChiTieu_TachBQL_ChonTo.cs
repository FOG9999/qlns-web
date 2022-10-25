using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptTongHop_ChiTieu_TachBQL_ChonToViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList QuyList { get; set; }
        public SelectList NganSachList { get; set; }
        public SelectList NamNganSachList { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
}

namespace VIETTEL.Controllers
{
    public class rptTongHop_ChiTieu_TachBQL_ChonToController : FlexcelReportController
    {
        private const string sFilePath_Excel = "/Report_ExcelFrom/TongHopNganSach/rptTongHop_ChiTieu_TachBQL_ChonTo.xls";

        #region ctors

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;

        public ActionResult Index()
        {

            var iNamLamViec = PhienLamViec.iNamLamViec;
            var phongbanList = new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sTen");
            var donviList = new SelectList(_nganSachService.GetDonviListByUser(Username, iNamLamViec), "iID_MaDonVi", "sMoTa");

            var vm = new rptTongHop_ChiTieu_TachBQL_ChonToViewModel()
            {
                iNamLamViec = PhienLamViec.iNamLamViec,
                QuyList = DataHelper.GetQuys().ToSelectList(),
                NganSachList = getNganSachList().ToSelectList(),
                NamNganSachList = getNamNganSachList().ToSelectList(),
                PhongBanList = phongbanList,
                DonViList = donviList,
            };


            var view = string.Format("{0}{1}.cshtml", "~/Views/Report_Views/TongHopNganSach/", this.ControllerName());
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iThang_Quy,
            string iID_MaNamNganSach,
            string iID_MaNganSach,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string toSo)
        {
            var xls = createReport(iThang_Quy, iID_MaNamNganSach, iID_MaNganSach, iID_MaPhongBan, iID_MaDonVi, toSo);
            var filename = $"Tờ số {toSo}.xls";

            return Print(xls, ext, filename);
        }


        public ActionResult Ds_To(string donvis)
        {
            var count = string.IsNullOrWhiteSpace(donvis) ? 0 : donvis.Split(',').ToList().Count + 1;
            return ds_ToIn(count, 2);
        }

        protected override JsonResult ds_ToIn(int count = 0, int colCount = 2, string id = "To")
        {
            var list = new List<SelectListItem>();

            if (count > 0)
            {
                var count_to = count <= colCount ? 1 : (count % colCount) == 0 ? (count / colCount) : (count / colCount) + 1;
                for (int i = 1; i <= count_to; i++)
                {
                    var to = i.ToString();
                    list.Add(new SelectListItem()
                    {
                        Value = to,
                        Text = "Tờ " + to,
                    });
                }
            }

            var vm = new ChecklistModel(id, new SelectList(list, "Value", "Text"));
            return ToCheckboxList(vm);

        }

        #endregion

        #region private methods

        private Dictionary<string, string> getNganSachList()
        {
            return new Dictionary<string, string>
            {
                { "1,2", "Tổng hợp" },
                { "1", "Đầu năm" },
                { "2", "Bổ sung" },
            };
        }
        private Dictionary<string, string> getNamNganSachList()
        {
            return new Dictionary<string, string>
            {
                { "1,2,4", "Tổng hợp" },
                { "2", "Năm nay" },
                { "1", "Năm trước" },
                { "4", "Năm trước chuyển sang" },
            };
        }
        private DataTable getDataTable(string iThang_Quy, string iID_MaNamNganSach, string iID_MaNganSach, string iID_MaPhongBan, string iID_MaDonVi, string toSo)
        {
            var sql = FileHelpers.GetSqlQuery("rpt_th_ct_tachbql.sql");
            var dNgay = new DateTime(int.Parse(PhienLamViec.iNamLamViec), 1, 1).AddMonths((int.Parse(iThang_Quy)) * 3);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
                cmd.Parameters.AddWithValue("@dNgay", dNgay);
                cmd.Parameters.AddWithValue("@sLNS", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                cmd.Parameters.AddWithValue("@iID_MaNganSach", string.IsNullOrWhiteSpace(iID_MaNganSach) || iID_MaNganSach == "0" ? "1,2" : iID_MaNganSach);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", string.IsNullOrWhiteSpace(iID_MaNamNganSach) || iID_MaNamNganSach == "0" ? "1,2,4" : iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", _nganSachService.CheckParam_PhongBan(iID_MaPhongBan).ToParamString());

                var dt = cmd.GetTable();
                return dt;
            }
        }

        private DataTable buildDataTable(string iThang_Quy, string iID_MaNamNganSach, string iID_MaNganSach, string iID_MaPhongBan, string iID_MaDonVi, string toSo)
        {
            var data = getDataTable(iThang_Quy, iID_MaNamNganSach, iID_MaNganSach, iID_MaPhongBan, iID_MaDonVi, toSo);
            var donvis = getDonVis(iID_MaDonVi, toSo).ToList();

            for (int i = 0; i < 8; i++)
            {
                var index = i + 1;

                var colCTB7 = $"CTB7{index}";
                var colCTB10 = $"CTB10{index}";

                data.Columns.Add(colCTB7, typeof(double));
                data.Columns.Add(colCTB10, typeof(double));

                if (donvis.Count() > i)
                {
                    var donvi = donvis[i];
                    var dt = getDataTable(iThang_Quy, iID_MaNamNganSach, iID_MaNganSach, iID_MaPhongBan, donvi, toSo);


                    for (int x = 0; x < data.Rows.Count; x++)
                    {
                        var row = data.Rows[x];
                        for (int y = 0; y < dt.Rows.Count; y++)
                        {
                            if (row["sXauNoiMa"].ToString() == dt.Rows[y]["sXauNoiMa"].ToString())
                            {
                                var ctb7 = dt.Rows[y]["CTB7"].ToString();
                                var ctb10 = dt.Rows[y]["CTB10"].ToString();

                                row[colCTB7] = ctb7;
                                row[colCTB10] = ctb10;
                            }
                        }
                    }
                }


            }

            return data;
        }

        private ExcelFile createReport(
            string iThang_Quy,
            string iID_MaNamNganSach,
            string iID_MaNganSach,
            string iID_MaPhongBan,
            string iID_MaDonVi,
            string toSo)
        {
            var xls = new XlsFile(true);
            xls.Open(getFilePath(toSo));

            var fr = new FlexCelReport();

            loadData(fr, iThang_Quy, iID_MaNamNganSach, iID_MaNganSach, iID_MaPhongBan, iID_MaDonVi, toSo);

            var iNamLamViec = PhienLamViec.iNamLamViec;

            //lay ten nam ngan sach
            var NamNganSach = "";
            if (iID_MaNganSach == "1")
                NamNganSach = "DỰ TOÁN ĐẦU NĂM";
            else if (iID_MaNganSach == "2")
                NamNganSach = "DỰ TOÁN BỔ SUNG";
            else
            {
                NamNganSach = "TỔNG HỢP";
            }

            var donvis = getDonVis(iID_MaDonVi, toSo).ToList();
            for (int i = 0; i < 8; i++)
            {
                var index = i + 1;
                var tenDonVi = string.Empty;
                if (donvis.Count() > i)
                {
                    var dv = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, donvis[i]);
                    tenDonVi = $"{dv.iID_MaDonVi} - {dv.sMoTa}";
                }
                fr.SetValue($"DonVi{index}", tenDonVi);
            }


            fr.SetValue("Quy", iThang_Quy);


            fr.UseCommonValue(new Application.Flexcel.FlexcelModel()
            {
                header2 = $"Đơn vị tính: đồng    Tờ số: {toSo}  Trang: ",
            })
                .UseChuKy(Username, iID_MaPhongBan)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        private string getFilePath(string to)
        {
            return Server.MapPath(sFilePath_Excel);
        }

        #endregion

        private void loadData(FlexCelReport fr, string iThang_Quy, string iID_MaNamNganSach, string iID_MaNganSach, string iID_MaPhongBan, string iID_MaDonVi, string toSo)
        {
            var data = buildDataTable(iThang_Quy, iID_MaNamNganSach, iID_MaNganSach, iID_MaPhongBan, iID_MaDonVi, toSo);
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }

        private IEnumerable<string> getDonVis(string iID_MaDonVi, string toSo)
        {
            var donvis = iID_MaDonVi.Split(',').ToList();
            var to = int.Parse(toSo);

            //var items = donvis.Skip((to - 1) * 8).Take(8);
            var items = donvis.Skip((to - 1) * 2).Take(2);
            return items;

        }
    }
}
