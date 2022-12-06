using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{

    public class rptDuToanBS_PhuLuc_NganhController : FlexcelReportController
    {
        #region ctor

        private const string _viewPath = "~/Views/Report_Views/DuToanBS/";

        private const string _filePath = "/Report_ExcelFrom/DuToanBS/rptDuToanBS_PhuLuc_Nganh.xls";

        private int dvt = 1000;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IDuToanBsService _dutoanbsService = DuToanBsService.Default;

        public ActionResult Index(int trinhKy = 0)
        {
            var _namLamViec = PhienLamViec.iNamLamViec;

            var dtPhongBan = DuToanBS_ReportModels.LayDSPhongBan(_namLamViec, Username);
            //var dtDot = DuToanBS_ReportModels.LayDSDot(_namLamViec, Username, "");


            var isTLTH = _nganSachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop;
            var dtDot = isTLTH ?
                _dutoanbsService.GetDots_Gom(_namLamViec, Username) :
                _dutoanbsService.GetDots(_namLamViec, Username);

            var dotList = dtDot
                .ToSelectList("iID_MaDot", "sMoTa", "-1", "-- Chọn đợt --");


            var vm = new rptDuToanBS_ChiTieuNganSachViewModel
            {
                iNamLamViec = _namLamViec,
                PhongBanList = dtPhongBan.ToSelectList("iID_MaPhongBan", "sTenPhongBan"),
                DotList = dotList,
                //DotList = dtDot.ToSelectList("iID_MaDot", "iID_MaDot", "-1", "-- Chọn đợt --"),
                TieuDe = "Giao bổ sung dự toán ngân sách",
                QuyetDinh = $"số           /QĐ-BQP ngày      tháng      năm {PhienLamViec.iNamLamViec} của Bộ Quốc phòng",
                TrinhKy = trinhKy,
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }

        #endregion

        #region public methods

        public ActionResult Print(
            string ext,
            string iID_MaDot,
            string sNG,
            string iID_MaPhongBan,
            int loaiBaoCao,
            string tenPhuLuc = null,
            string quyetDinh = null,
            string ghiChu = null)
        {
            var xls = createReport(Server.MapPath(_filePath), iID_MaDot, sNG, iID_MaPhongBan, tenPhuLuc, quyetDinh, ghiChu, loaiBaoCao);
            return Print(xls, ext);
        }

        public JsonResult Ds_NG(string iID_MaDot, string iID_MaPhongBan)
        {
            var data = _dutoanbsService.GetNganhs_Gom(PhienLamViec.iNamLamViec, iID_MaDot, Username);
            var vm = new ChecklistModel("sNG", data.ToSelectList("iID_MaNganh", "sTenNganh"));
            return ToCheckboxList(vm);
        }

        #endregion

        #region private methods

        private ExcelFile createReport(string path,
            string iID_MaDot,
            string sNG,
            string iID_MaPhongBan,
            string tenPhuLuc,
            string quyetDinh,
            string ghiChu,
            int loaiBaoCao)
        {
            var fr = new FlexCelReport();

            //Lấy dữ liệu chi tiết
            //var data = getDataTable(iID_MaDot, iID_MaPhongBan, iID_MaDonVi);
            var ds = LoadData(fr, iID_MaDot, iID_MaPhongBan, sNG, loaiBaoCao);

            var sTenPB = "";

            if (iID_MaPhongBan == "-1")
            {
                sTenPB = "Tất cả các phòng ban ";
            }
            else
                sTenPB = "B " + iID_MaPhongBan;

            string sqlNganh = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + sNG + "'");
            SqlCommand cmdNganh = new SqlCommand(sqlNganh);
            cmdNganh.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
            DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

            var tenNganh = Convert.ToString(dtNganhChon.Rows[0]["sTenNganh"]);
            fr.SetValue("sTenDonVi", tenNganh);


            fr.SetValue("sTenPB", sTenPB);
            fr.SetValue("TenPhuLuc", string.IsNullOrWhiteSpace(tenPhuLuc) ? "TenPhuLuc" : tenPhuLuc.ToUpper());
            fr.SetValue("QuyetDinh", string.IsNullOrWhiteSpace(quyetDinh) ? "QuyetDinh" : quyetDinh);

            #region ghi chus

            var noteBuilder = new StringBuilder();
            var notes = ds[1].AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x["sGhiChu"].ToString().Replace(" ", "")))
                .Select(x => $"{x["sXauNoiMa"]} - {x["sMoTa"]} - {x["sGhiChu"]}: {double.Parse(x["rTuChi"].ToString()).ToString("###.###")}")
                .ToList();
            if (notes.Count > 0)
            {
                notes.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x)) noteBuilder.AppendLine(x);
                });
            }

            if (!string.IsNullOrWhiteSpace(ghiChu))
                noteBuilder.AppendLine(ghiChu);

            fr.SetValue("GhiChu", noteBuilder.ToString());

            #endregion

            var sTenPhongBan = NganSachService.Default.GetPhongBanById(iID_MaPhongBan).sMoTa;
            fr.SetValue("sTenPhongBan", sTenPhongBan);

            var xls = new XlsFile(true);
            xls.Open(path);

            fr.UseChuKy(Username, iID_MaPhongBan)
                .UseCommonValue()
                .UseChuKyForController(this.ControllerName(), iID_MaPhongBan)
                .UseForm(this)
                .Run(xls);
            return xls;
        }

        private IList<DataTable> getDataTable(string iID_MaDot, string iID_MaPhongBan, string sNG, int loaiBaoCao)
        {
            var ds = new List<DataTable>();

            #region sql

            var sql = FileHelpers.GetSqlQuery("dtbs_chitieu_nganh.sql");

            #endregion

            #region load data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaNganh", sNG);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaDot);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    //cmd.Parameters.AddWithValue("@sLNS", sLNS.ToParamString());
                    cmd.Parameters.AddWithValue("@dvt", 1000);

                    var dt = cmd.GetTable();
                    ds.Add(dt);
                }

                // ghi chu
                sql = FileHelpers.GetSqlQuery("dtbs_chitieu_nganh_ghichu.sql");
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_MaNganh", sNG);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaDot);
                    cmd.Parameters.AddWithValue("@dvt", 1000);

                    var dt = cmd.GetTable();
                    ds.Add(dt);
                }

            }

            #endregion

            return ds;
        }

        private IList<DataTable> LoadData(FlexCelReport fr, string iID_MaDot, string iID_MaPhongBan, string iID_MaDonVi, int loaiBaoCao)
        {
            var ds = getDataTable(iID_MaDot, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao);

            var data = ds[0];
            FillDataTableLNS(fr, data, FillLNSType.LNS1, PhienLamViec.iNamLamViec);

            long TongTien = 0;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i]["rTongSo"].ToString() != "")
                {
                    TongTien += Convert.ToInt64(data.Rows[i]["rTongSo"]);
                }

            }

            //In loại tiền bằng chữ
            var bangChu = "";
            TongTien *= dvt;
            if (TongTien < 0)
            {
                TongTien *= -1;
                bangChu = "Giảm " + CommonFunction.TienRaChu(TongTien).ToLower();
            }
            else
            {
                bangChu = CommonFunction.TienRaChu(TongTien);
            }

            fr.SetValue("Tien", bangChu);

            return ds;
        }

        private IList<DataTable> LoadData2(FlexCelReport fr, string iID_MaDot, string iID_MaPhongBan, string iID_MaDonVi, int loaiBaoCao)
        {
            DataRow r;
            DataTable data = new DataTable();

            var ds = getDataTable(iID_MaDot, iID_MaPhongBan, iID_MaDonVi, loaiBaoCao);

            data = ds[0];

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "iID_MaDonVi,sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "iID_MaDonVi,sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "iID_MaDonVi,sLNS1,sLNS3,sLNS5,sLNS,sL,sK", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "iID_MaDonVi,sLNS1,sLNS3,sLNS5,sLNS", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sLNS,sMoTa", "sLNS,sL");

            DataTable dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", dtsLNS, "iID_MaDonVi,sLNS1,sLNS3,sLNS5", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sLNS5,sMoTa");
            for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            {
                r = dtsLNS5.Rows[i];
                r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS5"]));
            }

            DataTable dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "iID_MaDonVi,sLNS1,sLNS3", "iID_MaDonVi,sTenDonVi,sLNS1,sLNS3,sMoTa");
            for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            {
                r = dtsLNS3.Rows[i];
                r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS3"]));
            }

            DataTable dtsLNS1 = HamChung.SelectDistinct("dtsLNS1", dtsLNS3, "iID_MaDonVi,sLNS1", "sTenDonVi,iID_MaDonVi,sLNS1,sMoTa");
            for (int i = 0; i < dtsLNS1.Rows.Count; i++)
            {
                r = dtsLNS1.Rows[i];
                r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS1"]));
            }

            long TongTien = 0;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i]["rTongSo"].ToString() != "")
                {
                    TongTien += Convert.ToInt64(data.Rows[i]["rTongSo"]);
                }

            }

            //In loại tiền bằng chữ
            String Tien = "";
            TongTien *= dvt;
            if (TongTien < 0)
            {
                TongTien *= -1;
                Tien = "Giảm " + CommonFunction.TienRaChu(TongTien).ToLower();
            }
            else
            {
                Tien = CommonFunction.TienRaChu(TongTien);
            }

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS1", dtsLNS1);
            fr.AddTable("dtsLNS3", dtsLNS3);
            fr.AddTable("dtsLNS5", dtsLNS5);

            int count = 0;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (!Convert.ToString(data.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }
            for (int i = 0; i < dtsTM.Rows.Count; i++)
            {
                if (!Convert.ToString(dtsTM.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }
            for (int i = 0; i < dtsM.Rows.Count; i++)
            {
                if (!Convert.ToString(dtsM.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }
            for (int i = 0; i < dtsL.Rows.Count; i++)
            {
                if (!Convert.ToString(dtsL.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }
            for (int i = 0; i < dtsLNS.Rows.Count; i++)
            {
                if (!Convert.ToString(dtsLNS.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }
            for (int i = 0; i < dtsLNS1.Rows.Count; i++)
            {
                if (!Convert.ToString(dtsLNS1.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }
            for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            {
                if (!Convert.ToString(dtsLNS3.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }
            for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            {
                if (!Convert.ToString(dtsLNS5.Rows[i]["sMoTa"]).Equals(""))
                {
                    count++;
                }
            }

            int row = 20;
            DataTable dtTrang = new DataTable();
            dtTrang.Columns.Add("sTen", typeof(String));
            for (int i = 0; i < row - count; i++)
            {
                DataRow dr = dtTrang.NewRow();
                dtTrang.Rows.Add(dr);
            }
            fr.AddTable("dtTrang", dtTrang);

            fr.SetValue("Tien", Tien);
            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtsLNS1.Dispose();
            dtsLNS3.Dispose();
            dtsLNS5.Dispose();

            return ds;
        }

        #endregion

    }
}
