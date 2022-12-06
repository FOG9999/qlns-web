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
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Controllers
{
    public class rptDuToan_Kiem_NNController : FlexcelReportController
    {
        private const string _filePath = "~/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_NN_NG.xls";
        private const string _filePath_TongHop = "~/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_NN_NG_TongHop.xls";
        private const string _filePath_TongHop_To1 = "~/Report_ExcelFrom/DuToan/BieuKiem/rptDuToan_NN_NG_TongHop_To1.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt;
        private string _nganh;
        private string _donvi;
        private string _phongban;
        private string _loaibaocao;
        private int _to;

        public ActionResult Index()
        {
            var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
            var phongbanList = (PhienLamViec.iID_MaPhongBan != "02" && PhienLamViec.iID_MaPhongBan != "11") ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "-- Tất cả phòng ban --"); ;
            var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

            var vm = new rptDuToan_THNSNNViewModel()
            {
                PhongBanList = phongbanList,
                NganhList = dNganh.ToSelectList("Id", "Nganh", "-1", "-- Chọn chuyên ngành --"),
                DonViList = donviList.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"),
            };

            var view = "~/Areas/DuToan/Views/Report/kiem/rptDuToan_Kiem_NN.cshtml";

            return View(view, vm);
        }
        public JsonResult Ds_DonVi(string id_phongban)
        {
            //var data = _nganSachService.GetDonviByPhongBan(PhienLamViec.iNamLamViec, iID_MaPhongBan).AsEnumerable().Where(x => PhienLamViec.iID_MaDonVi.Contains(x.Field<string>("iID_MaDonVi"))).CopyToDataTable();

            var sql = FileHelpers.GetSqlQuery(@"rpt_dt_kiem_nn_donvi_list.sql");


            if (_phongban == "02") _phongban = string.Empty;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    PhienLamViec.iNamLamViec,
                    id_donvi = PhienLamViec.iID_MaDonVi.ToParamString(),
                    id_phongban = id_phongban.ToParamString(),
                });
                var vm = new ChecklistModel("DonVi", dt.ToSelectList("iID_MaDonVi", "sTenDonVi"));
                return ToCheckboxList(vm);
            }



        }

        public JsonResult Ds_To(string id_donvi)
        {
            return ds_ToIn(id_donvi.ToList().Count + 1, 3);
        }

        #region in_chitiet

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
            string phongban,
            string donvi,
            string loaibaocao,
            string ext = "pdf",
            int dvt = 1)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._donvi = donvi;
            this._phongban = phongban;
            this._loaibaocao = loaibaocao;

            HamChung.Language();

            var xls = createReport();
            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return Print(xls, ext);

            
        }


        private ExcelFile createReport()
        {
            var xls = new XlsFile(true);

            xls.Open(Server.MapPath(_filePath));
            var fr = new FlexCelReport();

            var data = getDataTable();

            FillDataTable(fr, data, iNamLamViec: PhienLamViec.iNamLamViec);



            var tenDonVi = _donvi.ToList().Count > 2 ?
                "Tất cả các đơn vị" :
                _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _donvi).sTen;
            var tenPhongBan = _phongban == "-1" || string.IsNullOrEmpty(_phongban)? "" : _nganSachService.GetPhongBanById(_phongban).sMoTa.ToUpper();
           
            fr.SetValue(new
            {
                h1 = tenDonVi,
                h2 = $"Đơn vị tính: {_dvt.ToStringDvt()} ",
                TieuDe1 = "Dự kiến số dự toán ngân sách nhà nước chi sự nghiệp",
                TieuDe2 = "",
                tenDonVi,
                tenPhongBan,
                SoTien = data.AsEnumerable().Sum(r => r.Field<decimal>("TuChi")).ToStringMoney(),
                NamTruoc = PhienLamViec.NamLamViec - 1,
                Nam = PhienLamViec.NamLamViec,
            })
              .UseCommonValue(new Application.Flexcel.FlexcelModel())
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return xls;

        }

        #endregion

        #region in_tonghop

        public ActionResult Print_TongHop(
           string phongban,
           string donvi,
           string loaibaocao,
           string ext = "pdf",
           int dvt = 1)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._donvi = donvi;
            this._phongban = phongban;
            this._loaibaocao = loaibaocao;

            HamChung.Language();

            var xls = createReport_tonghop();
            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return Print(xls, ext);

        }

        private ExcelFile createReport_tonghop()
        {
            var xls = new XlsFile(true);

            xls.Open(Server.MapPath(_filePath_TongHop));
            var fr = new FlexCelReport();

            var data = getDataTable();
            FillDataTable_NG(fr, data, iNamLamViec: PhienLamViec.iNamLamViec);

            var tenDonVi = _donvi.ToList().Count > 1 ?
             "Tất cả các đơn vị" :
             _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, _donvi).sTen;
            var tenPhongBan = _phongban == "-1" || string.IsNullOrEmpty(_phongban)? "" : _nganSachService.GetPhongBanById(_phongban).sMoTa.ToUpper();
            
            fr.SetValue(new
            {
                h1 = tenDonVi,
                h2 = $"Đơn vị tính: {_dvt.ToStringDvt()} ",
                TieuDe1 = "Dự kiến số dự toán ngân sách nhà nước chi sự nghiệp",
                TieuDe2 = "",
                tenDonVi,
                tenPhongBan,
                NamTruoc = PhienLamViec.NamLamViec - 1,
                Nam = PhienLamViec.NamLamViec,
            })
              .UseCommonValue(new Application.Flexcel.FlexcelModel())
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls);

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }

            return xls;
        }



        #endregion

        #region in_tonghop_to

        public ActionResult Print_TongHop_To(
          string phongban,
          string donvi,
          int to,
          string ext = "pdf",
          int dvt = 1)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._donvi = donvi;
            this._phongban = phongban;
            this._to = to;

            HamChung.Language();

            var xls = createReport_tonghop_to();
            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }
            return Print(xls, ext);

        }

        private ExcelFile createReport_tonghop_to()
        {
            var xls = new XlsFile(true);

            xls.Open(Server.MapPath(_filePath_TongHop_To1));
            var fr = new FlexCelReport();

            var dt = getDataTable();
            var data = dt.SelectDistinct("ChiTiet", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa");

            #region to

            var donvi_list = dt.AsEnumerable().Select(r => r.Field<string>("iID_MaDonVi")).JoinDistinct().ToList();
            donvi_list.Insert(0, string.Empty);

            donvi_list = donvi_list.Skip((_to - 1) * 3).Take(3).ToList();
            for (int i = 0; i < 3; i++)
            {
                var col_ = $"C{i + 1}";
                var col_DuToan = $"C{i + 1}_DuToan";
                var col_TuChi = $"C{i + 1}_TuChi";
                var col_1 = $"C{i + 1}_1";      // namtruoc
                var col_2 = $"C{i + 1}_2";      // nam nay
                var col_3 = $"C{i + 1}_3";      // tanggiam

                data.Columns.Add(col_DuToan, typeof(decimal));
                data.Columns.Add(col_TuChi, typeof(decimal));


                if (i >= donvi_list.Count)
                {
                    fr.SetValue(col_, "");
                    fr.SetValue(col_1, "");
                    fr.SetValue(col_2, "");
                    fr.SetValue(col_3, "");



                }
                else
                {
                    var donvi = donvi_list[i];
                    fr.SetValue(col_, donvi.IsEmpty() ? "Tổng cộng" : _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, donvi).sMoTa);
                    fr.SetValue(col_1, PhienLamViec.NamLamViec - 1);
                    fr.SetValue(col_2, PhienLamViec.NamLamViec);
                    fr.SetValue(col_3, "(+/-)");


                    data.AsEnumerable()
                        .ToList()
                        .ForEach(r =>
                        {
                            if (donvi.IsEmpty())
                            {
                                var dutoan = dt.AsEnumerable().Where(x => x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa")).Sum(x => x.Field<decimal>("DuToan"));
                                var tuchi = dt.AsEnumerable().Where(x => x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa")).Sum(x => x.Field<decimal>("TuChi"));

                                r[col_DuToan] = dutoan;
                                r[col_TuChi] = tuchi;
                            }
                            else
                            {
                                var dutoan = dt.AsEnumerable().Where(x => x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa") && x.Field<string>("iID_MaDonVi") == donvi).Sum(x => x.Field<decimal>("DuToan"));
                                var tuchi = dt.AsEnumerable().Where(x => x.Field<string>("sXauNoiMa") == r.Field<string>("sXauNoiMa") && x.Field<string>("iID_MaDonVi") == donvi).Sum(x => x.Field<decimal>("TuChi"));

                                r[col_DuToan] = dutoan;
                                r[col_TuChi] = tuchi;
                            }

                        });
                }



            }

            var rows_empty =
                data.AsEnumerable()
                    .Where(r => r.Field<decimal>("C1_DuToan", 0) + r.Field<decimal>("C1_TuChi", 0) +
                    r.Field<decimal>("C2_DuToan", 0) + r.Field<decimal>("C2_TuChi", 0) +
                    r.Field<decimal>("C3_DuToan", 0) + r.Field<decimal>("C3_TuChi", 0) == 0
                    )
                    .ToList();
            rows_empty.ForEach(r => data.Rows.Remove(r));


            FillDataTable(fr, data, iNamLamViec: PhienLamViec.iNamLamViec);

            #endregion




            var tenDonVi = _donvi.ToList().Count > 2 ?
                "Tất cả các đơn vị" :
                DonViModels.Get_TenDonVi(_donvi);
            var tenPhongBan = _phongban == "-1" || string.IsNullOrEmpty(_phongban) ? "" : _nganSachService.GetPhongBanById(_phongban).sMoTa.ToUpper();
            
            fr.SetValue(new
            {
                h1 = tenDonVi,
                h2 = $"Đơn vị tính: {_dvt.ToStringDvt()} - Tờ số {_to}",
                TieuDe1 = "Dự kiến số dự toán ngân sách nhà nước chi sự nghiệp",
                TieuDe2 = "",
                tenDonVi,
                tenPhongBan,
            })
              .UseCommonValue(new Application.Flexcel.FlexcelModel())
              .UseChuKyForController(this.ControllerName())
              .UseForm(this)
              .Run(xls, _to);

            return xls;
        }

        #endregion


        private DataTable getDataTable()
        {
            var sql = FileHelpers.GetSqlQuery(@"rpt_dt_kiem_nn_donvi.sql");

            #region load data

            if (_phongban == "02") _phongban = string.Empty;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql, new
                {
                    PhienLamViec.iNamLamViec,
                    id_donvi = _donvi.ToParamString(),
                    id_phongban = _phongban.ToParamString(),
                    dvt = _dvt,
                });
                return dt;
            }

            #endregion
        }

        #region private methods



        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var DonVi = getDataSet().Tables[0];
            if (DonVi.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var PhongBan = DonVi.SelectDistinct("PhongBan", "sLNS1,sLNS3,sLNS5,sLNS,iID_MaPhongBan,TenPhongBan");
                var sLNS = DonVi.SelectDistinct("dtsLNS", "sLNS1,sLNS3,sLNS5,sLNS");
                FillDataTable(fr, sLNS, "sLNS1 sLNS3 sLNS5", PhienLamViec.iNamLamViec);
                fr.AddTable("Muc", DonVi);
                fr.AddTable("PhongBan", PhongBan);
                fr.AddRelationship("ChiTiet", "PhongBan", "sLNS1,sLNS3,sLNS5,sLNS".Split(','), "sLNS1,sLNS3,sLNS5,sLNS".Split(','));
                fr.AddRelationship("PhongBan", "Muc", "sLNS1,sLNS3,sLNS5,sLNS,iID_MaPhongBan".Split(','), "sLNS1,sLNS3,sLNS5,sLNS,iID_MaPhongBan".Split(','));

                if (_phongban != "-1")
                {
                    fr.SetValue("bPB", 1);
                }
                else
                {
                    fr.SetValue("bPB", 0);
                }

                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataSet getDataSet()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var cmd = new SqlCommand("sp_dutoan_report_thnsnn", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = _nganh.ToParamString(),
                    phongban = _phongban.ToParamString(),
                    donvi = _donvi == "-1" ? PhienLamViec.iID_MaDonVi : _donvi.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    dvt = _dvt.ToParamString(),
                });
            }
        }
        #endregion
    }
}
