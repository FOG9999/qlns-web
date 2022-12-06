using Dapper;
using DomainModel;
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


namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToanBS_NSNN_Nganh2Controller : FlexcelReportController
    {
        private const string _filePath1 = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_Nganh_1.xls";
        private const string _filePath2 = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_Nganh_2.xls";
        private const string _filePath1_TrinhKy = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_Nganh_1_TrinhKy.xls";

        private string _loai;
        private string _tenPhuLuc;

        #region ctor

        private const string _viewPath = "~/Views/Report_Views/DuToanBS/";
        private const string _filePath = "~/Report_ExcelFrom/DuToanBS/rptDuToanBS_207_Tobia.xls";
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _bsService = DuToanBsService.Default;
        public ActionResult Index()
        {
            var namLamViec = PhienLamViec.iNamLamViec;

            //var dotList = DuToanBsService.Default.GetDots(namLamViec, Username, "207%")
            //.ToSelectList("iID_MaDot", "iID_MaDot", "-1", "-- Chọn đợt --");

            //var dotList = CacheService.Default.CachePerRequest(this.ControllerName() + "_Dot",
            //                        () => DuToanBS_ReportModels.LayDSDot(namLamViec, Username, "207"),
            //                        Viettel.Domain.DomainModel.CacheTimes.OneMinute)
            //    .ToSelectList("iID_MaDot", "iID_MaDot", "-1", "-- Chọn đợt --");


            var dotList = _bsService.GetDots(PhienLamViec.iNamLamViec, Username, "2,3,4,107")
             .ToSelectList("iID_MaDot", "iID_MaDot", "-1", "-- Chọn đợt --");

            var phongBanList = NganSachService.Default
                .GetPhongBans(PhienLamViec.iID_MaPhongBan)
                .Select(x => new
                {
                    value = x.sKyHieu,
                    text = x.sKyHieu + " - " + x.sMoTa,
                })
                .ToSelectList();

            var vm = new rptDuToanBS_207_NganhViewModel
            {
                TieuDe = "Giao dự toán ngân sách nhà nước",
                iNamLamViec = namLamViec,
                DotList = dotList,
                PhongList = phongBanList,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion


        public ActionResult Print(
            string iID_MaNganh,
            int toSo,
            string sLNS,
            string iID_MaPhongBan,
            string iID_MaDot,
            string ghiChu,
            string tenPhuLuc = "Giao dự toán ngân sách",
            string loai = "2",
            int loaibaocao = 1,
            int trinhKy = 1,
            string ext = "pdf")
        {
            var filePath = getXlsFile(iID_MaDot, toSo, trinhKy);
            _loai = loai;
            _tenPhuLuc = tenPhuLuc;

            var xls = createReport(filePath, iID_MaNganh, toSo, sLNS, iID_MaPhongBan, iID_MaDot, ghiChu, loaibaocao, trinhKy);
            return Print(xls, ext);
        }

        #region public methods

        public JsonResult Ds_Nganh(string iID_MaDot)
        {
            var vm = ChecklistModel.Default;
            try
            {

                var data = _bsService.GetNganh_NSNN(PhienLamViec.iNamLamViec, iID_MaDot, Username);
                vm = new ChecklistModel("Nganh", data.ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Tất cả --"));
            }
            catch (Exception)
            {
            }
            return ToDropdownList(vm);
        }

        public JsonResult Ds_ToIn(
            string iID_MaNganh,
            string sLNS,
            string iID_MaPhongBan,
            string iID_MaDot,
            int loaibaocao)
        {
            try
            {
                var data = getDataCache(iID_MaPhongBan, iID_MaDot, iID_MaNganh, loaibaocao);
                return ds_ToIn(data.ColumnsCount);
            }
            catch (Exception ex)
            {
                //throw;
            }

            return ds_ToIn(0);
        }


        public JsonResult Get_GhiChu(
            string iID_MaNganh,
            string sLNS,
            string iID_MaPhongBan,
            string iID_MaDot,
            int loaibaocao)
        {
            var ghichu = "";
            try
            {
                var data = getDataCache(iID_MaPhongBan, iID_MaDot, iID_MaNganh, loaibaocao);
                ghichu = data.GhiChu;
            }
            catch (Exception ex)
            {
                //throw;
            }

            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region private methods

        private ExcelFile createReport(String path, String iID_MaNganh, int ToSo, String sLNS, String iID_MaPhongBan, String iID_MaDot, string ghiChu, int loaibaocao, int trinhky = 1)
        {
            var xls = new XlsFile(true);
            xls.Open(path);
            FlexCelReport fr = new FlexCelReport();

            #region mota

            var reportVM = getReportData(iID_MaPhongBan, iID_MaDot, iID_MaNganh, loaibaocao, ToSo);
            fr.AddTable("ChiTiet", reportVM.Data);

            for (int i = 0; i < reportVM.Columns.Count; i++)
            {
                var c = reportVM.Columns[i];

                fr.SetValue($"MoTa1_{i + 1}", c.sNG);
                fr.SetValue($"MoTa2_{i + 1}", c.sMoTa);
                fr.SetValue($"MoTa3_{i + 1}", c.ColumnType == 1 ? "Bằng tiền" : "");
            }

            var tien = 0d;
            if (ToSo == 1)
            {
                tien = reportVM.Data.AsEnumerable().Sum(r => double.Parse(r["TongTuChi"].ToString()));
            }

            #endregion

            string sTenDonVi;
            if (string.IsNullOrWhiteSpace(iID_MaNganh) || iID_MaNganh == "-1")
            {
                sTenDonVi = string.Empty;
            }
            else
            {
                string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
                SqlCommand cmdNganh = new SqlCommand(sql);
                cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

                sTenDonVi = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            }


            var dvt = Request.GetQueryStringValue("dvt", 1000);
            var lns = checkNsnn(iID_MaDot);

            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                ghiChu = ghiChu.Replace("\\n", "\n");
            }
            fr.SetValue(new
            {
                sTenPhongBan = _nganSachService.GetPhongBanById(iID_MaPhongBan).sMoTa,
                sTenDonVi = sTenDonVi,
                DotNgay = iID_MaDot.ToParamDate().ToStringNgay().ToLower(),
                Tien = (tien * dvt).ToStringMoney(),
                //GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? reportVM.GhiChu : reportVM.GhiChu + Environment.NewLine + ghiChu,
                GhiChu = ghiChu,
                TieuDe1 = _tenPhuLuc + $" năm {PhienLamViec.iNamLamViec}",
                TieuDe2 = lns.StartsWith("2") ?
                    string.IsNullOrEmpty(sTenDonVi) ? "" : $"Ngân sách nhà nước ngành: {sTenDonVi}" :
                    "",

            })
            .UseCommonValue(new Application.Flexcel.FlexcelModel
            {
                header = $"Đơn vị tính: {dvt.ToStringNumber()} đồng - Tờ số: {ToSo} - Trang:",
                header2 = string.IsNullOrWhiteSpace(sTenDonVi) ? string.Empty : $"Ngành: {sTenDonVi}",
                footer = trinhky == 0 ? string.Empty : $"Đợt: {iID_MaDot.ToParamDate().ToStringNgay().ToLower()}",

                Thang = "Ngày ...... tháng 12 năm 2020",
                Ngay = "Ngày ...... tháng ...... năm 2020",
            })
            .UseChuKy(Username, iID_MaPhongBan)
            .UseChuKyForController(this.ControllerName())
            .UseForm(this)
            .Run(xls);

            return xls;
        }

        private Dictionary<string, string> _nganhDic = new Dictionary<string, string>();
        private string getMoTaTheoNganh(string lns, string mota)
        {
            if (mota.Contains(" - "))
                return mota;

            //var nganh = lns.Split('.').LastOrDefault();
            var nganh = lns.Split('(')[0].Split('.').LastOrDefault();
            if (!string.IsNullOrWhiteSpace(nganh) && nganh != "00")
            {
                if (_nganhDic.ContainsKey(lns))
                {
                    return _nganhDic[lns];
                }
                else
                {
                    var tenNganh = _nganSachService.GetNganhNhaNuoc(nganh);
                    if (!string.IsNullOrWhiteSpace(tenNganh))
                    {
                        mota = mota + " - " + tenNganh;

                        _nganhDic.Add(lns, mota);
                    }
                }
            }

            return mota;
        }

        #endregion

        #region version 2

        private rptPhanCapColumnList getData(string iID_MaPhongBan, string iID_MaDot, string iID_MaNganh, int loaibaocao)
        {

            DataTable dtNG, dt, dtGhiChu = null;

            #region params

            string sNG;
            if (string.IsNullOrWhiteSpace(iID_MaNganh) || iID_MaNganh == "-1")
            {
                sNG = string.Empty;
            }
            else
            {
                string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaNganh + "'");
                SqlCommand cmdNganh = new SqlCommand(sqlNganh);
                cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

                sNG = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
            }
            var iID_MaChungTu = _bsService.GetChungTus_DotNgay_NSNN(iID_MaDot, Username, PhienLamViec.iID_MaPhongBan).Join();

            #endregion

            #region sql - nganh

            ////var sql = FileHelpers.GetSqlQuery("dtbs_nganh_nsnn.sql");
            var sql = iID_MaPhongBan == "08" ?
                FileHelpers.GetSqlQuery("dtbs_nganh_nsnn_b8.sql") :
                FileHelpers.GetSqlQuery("dtbs_nganh_nsnn.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", PhienLamViec.iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", PhienLamViec.iID_MaNguonNganSach);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                cmd.Parameters.AddWithValue("@sNG", sNG.ToParamString());
                cmd.Parameters.AddWithValue("@loai", loaibaocao);

                dtNG = cmd.GetTable();
            }

            #endregion

            #region sql - du lieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                //sql = FileHelpers.GetSqlQuery("dtbs_nganh_nsnn_donvi.sql");
                sql = iID_MaPhongBan == "08" ?
                     FileHelpers.GetSqlQuery("dtbs_nganh_nsnn_donvi_b8.sql") :
                    FileHelpers.GetSqlQuery("dtbs_nganh_nsnn_donvi.sql");

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", PhienLamViec.iID_MaNamNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", PhienLamViec.iID_MaNguonNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@sNG", sNG.ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", 1000);
                    cmd.Parameters.AddWithValue("@loai", loaibaocao);

                    dt = cmd.GetTable();
                }

                #region sql - ghichu

                sql = FileHelpers.GetSqlQuery("dtbs_nganh_nsnn_donvi_ghichu.sql");
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", PhienLamViec.iID_MaNamNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", PhienLamViec.iID_MaNguonNganSach);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    cmd.Parameters.AddWithValue("@sNG", sNG.ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", 1000);

                    dtGhiChu = cmd.GetTable();
                }

                #endregion

            }

            #endregion

            var columns = new List<rptPhanCapColumn>();
            // build columns
            dtNG.AsEnumerable()
                .ToList()
                .ForEach(x =>
                {

                    var row = dt.AsEnumerable()
                            .ToList()
                            .FirstOrDefault(r => r.Field<string>("id") == x.Field<string>("id"));

                    if (row != null)
                    {
                        if (double.Parse(row["rTuChi"].ToString()) != 0)
                        {
                            var col = new rptPhanCapColumn()
                            {
                                Id = row.Field<string>("id"),
                                sLNS = row["sLNS"].ToString(),
                                sL = row["sL"].ToString(),
                                sK = row["sK"].ToString(),
                                sNG = x.Field<string>("NG"),
                                ColumnType = 1,
                                sMoTa = row.Field<string>("sMoTa")
                            };
                            columns.Add(col);
                        }


                        //if (double.Parse(row["rHienVat"].ToString()) != 0)
                        //{
                        //    var col = new rptPhanCapColumn()
                        //    {
                        //        Id = row.Field<string>("id"),
                        //        sLNS = row["sLNS"].ToString(),
                        //        sL = row["sL"].ToString(),
                        //        sK = row["sK"].ToString(),
                        //        sNG = x.Field<string>("NG"),
                        //        sMoTa = row.Field<string>("sMoTa"),
                        //        ColumnType = 2
                        //    };
                        //    columns.Add(col);
                        //}


                    }

                });


            //var vm = getReportData(dtNG, dt, ToSo);
            //vm.dtGhiChu = dtGhiChu;
            var vm = new rptPhanCapColumnList()
            {
                X = dtNG,
                Y = dt,

                //DtGhiChu = dtGhiChu,
                GhiChu = GhiChu(dtGhiChu, "", x => $"- {x["Ng"]}-{x["sMoTa"]}({x["sGhiChu"]}): {double.Parse(x["rTuChi"].ToString()).ToString("###,###")} đ"),

                Columns = columns,
                ColumnsCount = columns.Count == 0 ? 0 : columns.Count + 2,
            };

            return vm;
        }

        private rptPhanCapColumnList getDataCache(string iID_MaPhongBan, string iID_MaDot, string iID_MaNganh, int loaibaocao)
        {
            var cacheKey = $"{Username}_{this.ControllerName()}_{iID_MaDot}_{iID_MaPhongBan}_{iID_MaNganh}_{loaibaocao}";
            var data = CacheService.Default.CachePerRequest(
                cacheKey,
                () => getData(iID_MaPhongBan, iID_MaDot, iID_MaNganh, loaibaocao),
                Viettel.Domain.DomainModel.CacheTimes.OneMinute);
            return data;
        }

        private rptPhanCapColumnList getReportData(string iID_MaPhongBan, string iID_MaDot, string iID_MaNganh, int loaibaocao, int toSo)
        {
            var data = getDataCache(iID_MaPhongBan, iID_MaDot, iID_MaNganh, loaibaocao);

            var dt = data.Y.SelectDistinct("y", "iID_MaDonVi, TenDonVi");

            int columnsCount;
            if (toSo == 1)
            {
                dt.Columns.Add("TongTuChi", typeof(double));
                //dt.Columns.Add("TongHienVat", typeof(double));

                #region get tong

                dt.AsEnumerable().ToList()
                    .ForEach(r =>
                    {
                        var colName = "rTuChi";
                        var sum = data.Y.AsEnumerable()
                            .Where(x => x.Field<string>("iID_MaDonVi") == r.Field<string>("iID_MaDonVi"))
                           .Sum(x => double.Parse(x[colName].ToString()));

                        r["TongTuChi"] = sum;
                    });

                //dt.AsEnumerable().ToList()
                //  .ForEach(r =>
                //  {
                //      var colName = "rHienVat";
                //      var sum = data.Y.AsEnumerable()
                //          .Where(x => x.Field<string>("iID_MaDonVi") == r.Field<string>("iID_MaDonVi"))
                //         .Sum(x => double.Parse(x[colName].ToString()));

                //      r["TongHienVat"] = sum;
                //  });


                #endregion

                columnsCount = 5;
            }
            else
            {
                columnsCount = 6;
            }

            #region build columns

            var columns = data.Columns.Skip(toSo == 1 ? 0 : (toSo - 1) * 6 - 1).Take(columnsCount).ToList();
            for (int i = columns.Count; i < columnsCount; i++)
            {
                columns.Add(new DuToan.rptPhanCapColumn());
            }

            for (int i = 0; i < columnsCount; i++)
            {
                var colName = $"Cot{i + 1}";
                dt.Columns.Add(colName, typeof(double));
            }

            for (int i = 0; i < columns.Count; i++)
            {
                var colName = $"Cot{i + 1}";
                var c = columns[i];
                dt.AsEnumerable().ToList()
                    .ForEach(r =>
                    {
                        var row = data.Y.AsEnumerable().ToList()
                                .FirstOrDefault(x => x.Field<string>("id") == c.Id &&
                                                     x.Field<string>("iID_MaDonVi") == r.Field<string>("iID_MaDonVi"));
                        if (row != null)
                        {
                            r[colName] = columns[i].ColumnType == 1 ? row["rTuChi"] : r["rHienVat"];
                        }

                    });
            }



            #endregion

            var vm = new rptPhanCapColumnList()
            {
                Data = dt,
                Columns = columns,
                GhiChu = toSo == 1 ? data.GhiChu : "",
            };
            return vm;
        }

        #endregion

        private string checkNsnn(string iID_MaDot)
        {
            var lns = getLnsTheoDot(iID_MaDot);
            return lns;
        }

        //private string getXlsFile(string iID_MaDot, int toSo, int trinhKy)
        //{
        //    var filePath = _filePath;

        //    if (trinhKy == 0)
        //    {
        //        filePath = toSo == 1 ? _filePath1 : _filePath2;
        //    }
        //    else
        //    {
        //        var lns = getLnsTheoDot(iID_MaDot);
        //        if (lns.StartsWith("405"))
        //        {
        //            filePath = toSo == 1 ? _filePath1_TrinhKy_NSK : _filePath2;
        //        }
        //        else
        //        {
        //            filePath = toSo == 1 ? _filePath1_TrinhKy : _filePath2;
        //        }
        //    }



        //    return Server.MapPath(filePath);

        //}

        private string getXlsFile(string iID_MaDot, int toSo, int trinhKy)
        {
            var filePath = _filePath;

            if (trinhKy == 0)
            {
                filePath = toSo == 1 ? _filePath1 : _filePath2;
            }
            else
            {
                filePath = toSo == 1 ? _filePath1_TrinhKy : _filePath2;
            }

            return Server.MapPath(filePath);

        }
        private string getLnsTheoDot(string iID_MaDot)
        {
            var sql = @"

select sDSLNS from DTBS_ChungTu
where   iTrangThai=1
        and iNamLamViec=@iNamLamViec
        and sID_MaNguoiDungTao=@username
        and dNgayChungTu=@dNgayChungTu
        and iID_MaPhongBanDich=@iID_MaPhongBan

";

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var value = conn.QueryFirst<string>(
                    sql,
                    param: new
                    {
                        iNamLamViec = PhienLamViec.iNamLamViec,
                        iID_MaPhongBan = PhienLamViec.iID_MaPhongBan,
                        username = Username,
                        dNgayChungTu = iID_MaDot.ToParamDate(),
                    },
                    commandType: CommandType.Text);

                return value;
            }
        }

    }

    public class rptPhanCapColumn
    {
        public string sMoTa { get; set; }

        public string sLNS { get; set; }

        public string sL { get; set; }
        public string sK { get; set; }
        public string sNG { get; set; }
        public string Id { get; set; }

        public int ColumnType { get; set; }

    }

    public class rptPhanCapColumnList
    {
        public IList<rptPhanCapColumn> Columns { get; set; }

        public int ColumnsCount { get; set; }

        public DataTable Data { get; set; }

        /// <summary>
        /// datatable truc ngang
        /// </summary>
        public DataTable X { get; set; }

        /// <summary>
        /// datatable truc doc
        /// </summary>
        public DataTable Y { get; set; }

        public DataTable DtGhiChu { get; set; }

        public string GhiChu { get; set; }

    }
}
