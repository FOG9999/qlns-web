using Dapper;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
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

}

namespace VIETTEL.Controllers
{
    public class rptQuyetToanNam_ChiTietColumn
    {
        public string Name { get; set; }
        public string sLNS3 { get; set; }
        public string sLNS { get; set; }
        public string sL { get; set; }
        public string sK { get; set; }
        public string sMoTa { get; set; }

        public bool IsSum { get; set; }

    }

    public class rptQuyetToanNam_ChiTiet_ReportData
    {
        public DataTable dtChiTiet { get; set; }

        public DataTable dtsLNS { get; set; }
        public DataTable dtsLK { get; set; }
        public DataTable dtsLNS3 { get; set; }

        public IEnumerable<rptQuyetToanNam_ChiTietColumn> Columns { get; set; }

        public int GroupCount { get; set; }

        public int ColumnsCount { get; set; }
    }

    public class rptQuyetToanNam_ChiTietViewModel
    {
        //public SelectList Ds_To { get; set; }
    }

    public class rptQuyetToanNam_ChiTietController : FlexcelReportController
    {
        //private const string sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToan_TongHop_NamQuy_ChonTo.xls";
        private const string sFilePath_To1 = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_ChiTiet_To1.xls";
        private const string sFilePath_To2 = "/Report_ExcelFrom/QuyetToan/QuyetToanNam/rptQuyetToanNam_ChiTiet_To2.xls";

        #region ctors

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;

        public ActionResult Index()
        {

            var iNamLamViec = PhienLamViec.iNamLamViec;

            var vm = new rptQuyetToanNam_ChiTietViewModel()
            {
            };


            var view = string.Format("{0}{1}.cshtml", "~/Views/Report_Views/QuyetToan/", this.ControllerName());
            return View(view, vm);
        }

        #endregion

        #region public methods

        private int _loaiBaoCao;
        public ActionResult Print(
            string ext,
            string toSo,
            int loaiBaoCao = 0)
        {

            _loaiBaoCao = loaiBaoCao;

            var xls = createReport(toSo);
            return Print(xls, ext);
        }


        public ActionResult Ds_To()
        {
            var data = getDataTable(PhienLamViec.iNamLamViec, "2");
            var vm = calColumns(data);
            var count = vm.ColumnsCount + 3;

            return ds_ToIn(count, 11);


        }

        protected override JsonResult ds_ToIn(int count = 0, int colCount = 6, string id = "To")
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

        private DataTable getDataTable(string iNamLamViec, string sLNS, string sL = null, string sK = null)
        {
            var sql = FileHelpers.GetSqlQuery("qt_nam_chitiet.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@sLNS", $"{sLNS}%");
                cmd.Parameters.AddWithValue("@sL", sL.ToParamString());
                cmd.Parameters.AddWithValue("@sK", sK.ToParamString());

                var dt = cmd.GetTable();
                return dt;
            }
        }

        private DataTable getDataTable_ChiTiet(string iNamLamViec, string sLNS, string sL = null, string sK = null)
        {
            var sql = FileHelpers.GetSqlQuery("qt_nam_chitiet_lns.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@sLNS", $"{sLNS}%");
                cmd.Parameters.AddWithValue("@sL", sL.ToParamString());
                cmd.Parameters.AddWithValue("@sK", sK.ToParamString());
                cmd.Parameters.AddWithValue("@loai", _loaiBaoCao.ToParamString());

                var dt = cmd.GetTable();
                return dt;
            }
        }

        private dynamic getReportData(string iNamLamViec, string sLNS)
        {
            var data = getDataTable(iNamLamViec, sLNS);

            var sLNSColumns = data.SelectDistinct("sLNSColumns", "sLNS3,sLNS");
            var sLNS3Columns = sLNSColumns.SelectDistinct("sLNS3Columns", "sLNS3");

            return new
            {
                data,
                sLNSColumns,
                sLNS3Columns
            };
        }

        private rptQuyetToanNam_ChiTiet_ReportData buildDataTable(string iNamLamViec, string toSo)
        {
            var sLNS = "";
            var dtChiTiet = getDataTable_ChiTiet(iNamLamViec, sLNS);
            var data = getDataTable(iNamLamViec, "2");
            var vm = calColumns(data);

            //var vm = getReportData(iNamLamViec, sLNS);

            IEnumerable<rptQuyetToanNam_ChiTietColumn> cols = null;
            var columnsCount = 0;
            var skip = 0;

            if (toSo == "1")
            {
                dtChiTiet.Columns.Add("Nsqp", typeof(double));
                dtChiTiet.Columns.Add("Nsnn", typeof(double));

                var colName = "Nsqp";
                var dtNsqp = getDataTable_ChiTiet(iNamLamViec, "1");
                dtNsqp.AsEnumerable()
                          .ToList()
                          .ForEach(x =>
                          {
                              var row = dtChiTiet.AsEnumerable()
                               .FirstOrDefault(r => r.Field<string>("sM") == x.Field<string>("sM") &&
                                                       r.Field<string>("sTM") == x.Field<string>("sTM"));
                              if (row != null)
                              {
                                  row[colName] = (string.IsNullOrWhiteSpace(row[colName].ToString()) ? 0d : double.Parse(row[colName].ToString()))
                                                   + double.Parse(x["rTuChi"].ToString());
                              }
                          });

                colName = "Nsnn";
                var dtNsnn = getDataTable_ChiTiet(iNamLamViec, "2");
                dtNsnn.AsEnumerable()
                          .ToList()
                          .ForEach(x =>
                          {
                              var row = dtChiTiet.AsEnumerable()
                               .FirstOrDefault(r => r.Field<string>("sM") == x.Field<string>("sM") &&
                                                       r.Field<string>("sTM") == x.Field<string>("sTM"));
                              if (row != null)
                              {
                                  row[colName] = (string.IsNullOrWhiteSpace(row[colName].ToString()) ? 0d : double.Parse(row[colName].ToString()))
                                                   + double.Parse(x["rTuChi"].ToString());
                              }
                          });



                columnsCount = 8;
                skip = 0;
            }
            else
            {
                columnsCount = 11;
                skip = (int.Parse(toSo) - 1) * 11 - 3;
            }




            // add columns
            for (int i = 0; i < columnsCount; i++)
            {
                var colName = $"C{i + 1}";
                dtChiTiet.Columns.Add(colName, typeof(double));
            }

            cols = vm.Columns.Skip(skip).Take(columnsCount);

            // fill columns
            for (int i = 0; i < cols.Count(); i++)
            {
                var colName = $"C{i + 1}";
                //dtChiTiet.Columns.Add(colName, typeof(double));

                var c = cols.ToList()[i];
                if (c.IsSum)
                {
                    var colsSum = vm.Columns.Where(x => x.sLNS3 == c.sLNS);

                    colsSum.ToList()
                        .ForEach(col =>
                        {
                            var dt = getDataTable_ChiTiet(iNamLamViec, col.sLNS, col.sL, col.sK);
                            dt.AsEnumerable()
                           .ToList()
                           .ForEach(x =>
                           {
                               var row = dtChiTiet.AsEnumerable()
                                .FirstOrDefault(r => r.Field<string>("sM") == x.Field<string>("sM") &&
                                                        r.Field<string>("sTM") == x.Field<string>("sTM"));
                               if (row != null)
                               {
                                   row[colName] = (string.IsNullOrWhiteSpace(row[colName].ToString()) ? 0d : double.Parse(row[colName].ToString()))
                                                    + double.Parse(x["rTuChi"].ToString());
                               }
                           });

                        });
                }
                else
                {
                    var dt = getDataTable_ChiTiet(iNamLamViec, c.sLNS, c.sL, c.sK);
                    dt.AsEnumerable()
                   .ToList()
                   .ForEach(x =>
                   {
                       var row = dtChiTiet.AsEnumerable()
                               .FirstOrDefault(r => r.Field<string>("sM") == x.Field<string>("sM") &&
                                                       r.Field<string>("sTM") == x.Field<string>("sTM"));
                       if (row != null)
                       {
                           row[colName] = double.Parse(x["rTuChi"].ToString());
                       }
                   });

                }
            }

            vm.Columns = cols;
            vm.ColumnsCount = columnsCount;
            vm.dtChiTiet = dtChiTiet;

            return vm;
        }

        private ExcelFile createReport(
            string toSo)
        {
            var xls = new XlsFile(true);
            xls.Open(getFilePath(toSo));

            var fr = new FlexCelReport();
            var data = buildDataTable(PhienLamViec.iNamLamViec, toSo);
            loadData(fr, data);

            #region fill excel

            var sumFrom = 0;
            for (int i = 0; i < data.Columns.Count(); i++)
            {
                var c = data.Columns.ToList()[i];


                var x = 7;
                var y = 5;

                if (toSo != "1")
                {
                    x = 4;
                }
                if (c.IsSum)
                {
                    xls.SetCellValue(y + 1, x + i, $"Cộng");
                    xls.MergeCells(y + 1, x + i, y + 2, x + i);
                    //xls.SetCellFormat(y + 1, x + i, y + 2, x + i, TFlxFormat.CreateStandard2007(), new TFlxApplyFormat() { })


                    //Add a rectangle around the cells
                    TFlxApplyFormat ApplyFormat = new TFlxApplyFormat();
                    ApplyFormat.SetAllMembers(true);
                    //ApplyFormat.Borders.SetAllMembers(true);  //We will only apply the borders to the existing cell formats
                    TFlxFormat fmt = xls.GetDefaultFormat;
                    fmt.Font.Style = TFlxFontStyles.Bold;
                    fmt.Font.Size20 = 172;

                    fmt.HAlignment = THFlxAlignment.center;
                    fmt.VAlignment = TVFlxAlignment.center;

                    fmt.Borders.SetAllBorders(TFlxBorderStyle.Thin, TExcelColor.Automatic);
                    fmt.Borders.Diagonal.Color = TExcelColor.FromArgb(100);

                    xls.SetCellFormat(y + 1, x + i, y + 1, x + i, fmt, ApplyFormat, true);  //Set last parameter to true so it draws a box.


                    // merge cells
                    xls.SetCellValue(y, x + sumFrom, _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, c.sLNS));
                    xls.MergeCells(y, x + sumFrom, y, x + i);

                    sumFrom = i + 1;
                }
                else
                {
                    // loai - khoan
                    xls.SetCellValue(y + 1, x + i, $"L: {c.sL} - K: {c.sK}");
                    //xls.SetCellValue(y + 1, x + i, $"{c.sL}-{c.sK}-{c.sLNS}");

                    // mota
                    xls.SetCellValue(y + 2, x + i, _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, $"{c.sLNS}"));
                }

                if (i == data.ColumnsCount - 1 && sumFrom < data.ColumnsCount)
                {
                    xls.SetCellValue(y, x + sumFrom, _nganSachService.GetMLNS_MoTa(PhienLamViec.iNamLamViec, c.sLNS3));
                    xls.MergeCells(y, x + sumFrom, y, x + i);
                }
            }




            #endregion

            fr.SetValue("TieuDe1", $"Báo cáo số liệu quyết toán chi ngân sách nhà nước năm {PhienLamViec.iNamLamViec}");
            fr.SetValue("TieuDe2", $"Kèm theo văn bản số       /BC-BQP ngày      tháng     năm {int.Parse(PhienLamViec.iNamLamViec) + 1} của Bộ Quốc phòng");

            fr.UseCommonValue(new Application.Flexcel.FlexcelModel()
            {
                header2 = $"Đơn vị tính: đồng    Tờ số: {toSo}  Trang: ",
            })
                .UseChuKy(Username)
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        private string getFilePath(string to)
        {
            var file = to == "1" ? sFilePath_To1 : sFilePath_To2;
            return Server.MapPath(file);
        }

        #endregion

        private void loadData(FlexCelReport fr, rptQuyetToanNam_ChiTiet_ReportData data)
        {
            //data.dtChiTiet.AddLnsMoTa();

            addLnsMoTa_TieuMuc(data.dtChiTiet);

            var dtsM = data.dtChiTiet.SelectDistinct("dtsM", "sM");
            addLnsMoTa_Muc(dtsM);

            fr.AddTable("ChiTiet", data.dtChiTiet);
            fr.AddTable("dtsM", dtsM);

            data.dtChiTiet.Dispose();
            dtsM.Dispose();

            //FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);
        }

        private IEnumerable<string> getDonVis(string iID_MaDonVi, string toSo)
        {
            var donvis = iID_MaDonVi.Split(',').ToList();
            var to = int.Parse(toSo);

            var items = donvis.Skip((to - 1) * 12).Take(12);
            return items;

        }

        private rptQuyetToanNam_ChiTiet_ReportData calColumns(DataTable data)
        {
            var cols = new List<rptQuyetToanNam_ChiTietColumn>();

            var sLKColumns = data.SelectDistinct("sTMColumns", "sLNS3,sLNS,sL,sK");
            var sLNSColumns = data.SelectDistinct("sLNSColumns", "sLNS3,sLNS");
            var sLNS3Columns = sLNSColumns.SelectDistinct("sLNS3Columns", "sLNS3");

            sLNS3Columns.AsEnumerable()
                .ToList()
                .ForEach(x =>
                {
                    var lns = x.Field<string>("sLNS3");
                    sLKColumns.AsEnumerable()
                        .Where(c => c.Field<string>("sLNS3") == lns)
                        .ToList()
                        .ForEach(r =>
                        {
                            cols.Add(new rptQuyetToanNam_ChiTietColumn()
                            {
                                sLNS = r.Field<string>("sLNS"),
                                sLNS3 = lns,
                                sL = r.Field<string>("sL"),
                                sK = r.Field<string>("sK"),
                                IsSum = false,
                            });

                        });

                    cols.Add(new rptQuyetToanNam_ChiTietColumn()
                    {
                        sLNS = lns,
                        IsSum = true,
                    });

                });


            return new rptQuyetToanNam_ChiTiet_ReportData()
            {
                dtChiTiet = data,
                dtsLNS = sLNSColumns,
                dtsLNS3 = sLNS3Columns,
                dtsLK = sLKColumns,

                Columns = cols,
                GroupCount = sLNS3Columns.Rows.Count,
                ColumnsCount = sLKColumns.Rows.Count + sLNS3Columns.Rows.Count,
            };
        }

        private void addLnsMoTa_Muc(DataTable dt)
        {
            if (dt.GetColumnNames().Contains("sMoTa"))
                return;


            dt.Columns.Add("sMoTa", typeof(string));

            dt.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {

                    var sM = r.Field<string>("sM");
                    var sql = @"

select top(1) sMoTa from NS_MucLucNganSach where sM=@sM and sTm='' and sTTm='' and sNG=''

";

                    using (var con = ConnectionFactory.Default.GetConnection())
                    {
                        var sMota = con.QueryFirstOrDefault<string>(sql,
                           param: new
                           {
                               sM
                           },
                           commandType: CommandType.Text);

                        r["sMoTa"] = sMota;
                    }


                });
        }

        private void addLnsMoTa_TieuMuc(DataTable dt)
        {
            if (dt.GetColumnNames().Contains("sMoTa"))
                return;


            dt.Columns.Add("sMoTa", typeof(string));

            dt.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {

                    var sM = r.Field<string>("sM");
                    var sTM = r.Field<string>("sTM");
                    var sql = @"

select top(1) sMoTa from NS_MucLucNganSach where sM=@sM and sTm=@sTM order by sXauNoiMa

";

                    using (var con = ConnectionFactory.Default.GetConnection())
                    {
                        var sMota = con.QueryFirstOrDefault<string>(sql,
                           param: new
                           {
                               sM,
                               sTM
                           },
                           commandType: CommandType.Text);

                        r["sMoTa"] = sMota;
                    }


                });
        }
    }
}
