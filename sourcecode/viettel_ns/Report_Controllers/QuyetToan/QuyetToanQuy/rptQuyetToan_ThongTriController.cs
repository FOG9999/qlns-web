using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.Ajax.Utilities;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Services;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptQuyetToan_ThongTriViewModel
    {
        public QTA_ChungTu chungtu { get; set; }
        public string sTenDonVi { get; set; }
        public string ghiChu { get; set; }
        public SelectList dsloaiThongTri { get; set; }
        public SelectList dsloaiQuyetToan { get; set; }

    }

}

namespace VIETTEL.Controllers
{
    public class rptQuyetToan_ThongTriController : FlexcelReportController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;

        private string _filePath;
        private string _loaiQuyetToan;
        private string loaiThongTri;
        private QTA_ChungTu chungTu;
        private string sLNS;
        private string khoangCachDong;
        private string upOrDown;
        private string loai;
        private string chuky;
        private string muc;
        private DateTime date;

        public ActionResult Index(string iID_MaChungTu)
        {
            var chungTu = _quyetToanService.GetChungTu(new Guid(iID_MaChungTu));
            var sTen = $"thongtri_qt_{PhienLamViec.iNamLamViec}_{iID_MaChungTu}_{chungTu.iID_MaDonVi}";
            var vm = new rptQuyetToan_ThongTriViewModel()
            {
                chungtu = chungTu,
                sTenDonVi = _nganSachService.GetDonVi(chungTu.iNamLamViec.ToString(), chungTu.iID_MaDonVi).sMoTa,
                ghiChu = _quyetToanService.GetGhiChuThongTri(Username, sTen, chungTu.iID_MaDonVi),
                dsloaiQuyetToan = _quyetToanService.GetLoaiQuyetToan().ToSelectList(),
                dsloaiThongTri = _quyetToanService.GetLoaiThongTri(chungTu.iLoai, chungTu.sDSLNS).ToSelectList("loaiThongTri", "TenThongTri", "-1", "---- Chọn loại thông tri ----")
            };


            if (Request.IsAjaxRequest())
            {

                var view = @"~\Views\Report_Views\QuyetToan\rptQuyetToan_ThongTri.cshtml";
                return PartialView(view, vm);
            }
            else
            {

                var view = @"~\Views\Report_Views\QuyetToan\rptQuyetToan_ThongTri.cshtml";
                return View(view, vm);
            }
        }

        #region reports

        public ActionResult Print(string ext,
            string loaiThongTri,
            string loaiQuyetToan,
            string iID_MaChungTu,
            string sLNS,
            string muc,
            string chuky,
            string loai,
            string khoangCachDong,
            string date,
            string upOrDown)
        {
            this._filePath = getFilePath(loai, muc);
            this.loaiThongTri = loaiThongTri;
            this._loaiQuyetToan = loaiQuyetToan;
            this.chungTu = _quyetToanService.GetChungTu(new Guid(iID_MaChungTu));
            this.sLNS = sLNS;
            this.loai = loai;
            this.chuky = chuky;
            this.muc = muc;
            this.khoangCachDong = khoangCachDong;
            this.date = Convert.ToDateTime(date);
            this.upOrDown = upOrDown;

            var xls = createReport();
            return Print(xls, ext);
        }

        #endregion

        #region public methods

        public JsonResult Ds_LNS(string iID_MaChungTu, string loaiThongTri)
        {
            var data = _quyetToanService.GetLnsThongTri(iID_MaChungTu, PhienLamViec.iNamLamViec, loaiThongTri);
            var list = data.ToSelectList("sLNS", "TenHT");
            return ToCheckboxList(new ChecklistModel("lns", list));
        }
        [HttpPost]
        public JsonResult GhiChu_Update(string iID_MaChungTu, string iID_MaDonVi, string sGhiChu)
        {
            var sTen = $"thongtri_qt_{PhienLamViec.iNamLamViec}_{iID_MaChungTu}_{iID_MaDonVi}";
            var success = _quyetToanService.UpdateGhiChuThongTri(Username, sTen, iID_MaDonVi, sGhiChu);
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region private methods


        private ExcelFile createReport()
        {
            var fr = new FlexCelReport();
            LoadData(fr);

            //Lấy tháng của đợt cấp phát            
            string sTenDonVi = "";
            if (loai == "0")
            {
                sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, chungTu.iID_MaDonVi).sTen;
            }
            string quyetToan = "";
            if (_loaiQuyetToan == "-1")
            {
                quyetToan = $"Xác nhận quyết toán {_quyetToanService.GetLoaiThongTri()[loaiThongTri].ToLower()}";
            }
            else
            {
                quyetToan = $"{_quyetToanService.GetLoaiQuyetToan()[_loaiQuyetToan]} {_quyetToanService.GetLoaiThongTri()[loaiThongTri].ToLower()}";
            }

            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, Username));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, Username));
            fr.SetValue("NgayThang", "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year);
            fr.SetValue("Nam", PhienLamViec.iNamLamViec);
            fr.SetValue("NamNganSach", chungTu.iID_MaNamNganSach == 1 ? "Ngân sách năm trước chuyển sang" : "");
            fr.SetValue("NgayLap", "Quý " + chungTu.iThang_Quy + " năm " + PhienLamViec.iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("LoaiCapPhat", quyetToan.Trim());
            fr.SetValue("soChuky", chuky);
            fr.SetValue("MTT", L("ThongTri"));

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(this._filePath));

            fr.UseChuKyForController(this.ControllerName());

            fr.Run(xls);

            return xls;

        }

        private void LoadData(FlexCelReport fr)
        {
            DataRow r;
            DataTable data = new DataTable();
            DataTable dtDonVi = new DataTable();
            DataTable dtsTM = new DataTable();
            DataTable dtsM = new DataTable();
            DataTable dtsL = new DataTable();
            DataTable dtsLNS = new DataTable();
            DataTable dtsLNS1 = new DataTable();
            DataTable dtsLNS3 = new DataTable();
            DataTable dtsLNS5 = new DataTable();

            //Hiển thị chi tiết
            if (loai == "0")
            {
                data = _quyetToanService.QuyetToanChiTiet(chungTu.iID_MaChungTu.ToString(), sLNS);

                if (_loaiQuyetToan == "1")
                {
                    data.AsEnumerable()
                           .ToList()
                           .ForEach(row =>
                           {
                               row["rTuChi"] = (-1) * Convert.ToDecimal(row["rTuChi"]);
                           });
                }
                dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
                dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
                dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
                dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa", "sLNS,sL");

                dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", dtsLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5,sMoTa");
                for (int i = 0; i < dtsLNS5.Rows.Count; i++)
                {
                    r = dtsLNS5.Rows[i];
                    r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS5"]));
                }

                dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa");
                for (int i = 0; i < dtsLNS3.Rows.Count; i++)
                {
                    r = dtsLNS3.Rows[i];
                    r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS3"]));
                }

                dtsLNS1 = HamChung.SelectDistinct("dtsLNS1", dtsLNS3, "sLNS1", "sLNS1,sMoTa");
                for (int i = 0; i < dtsLNS1.Rows.Count; i++)
                {
                    r = dtsLNS1.Rows[i];
                    r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS1"]));
                }
            }

            //Hiển thị tổng hợp
            else
            {
                dtDonVi = _quyetToanService.QuyetToanTongHop(chungTu.iID_MaChungTu.ToString(), sLNS);
                data = HamChung.SelectDistinct("ChiTiet", dtDonVi, "iID_MaDonVi,sTenDonVi", "iID_MaDonVi,sTenDonVi,sMoTa");
                ReportModels.numberCount(data);
                fr.AddTable("dtDonVi", dtDonVi);
                dtDonVi.Dispose();
            }

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            var sum = 0.0;
            if (loai == "0")
            {
                sum = data.Sum("rTuChi");
            }
            else
            {
                sum = dtDonVi.Sum("rTuChi");
            }

            //In loại tiền bằng chữ
            var tien = sum < 0 ? "Giảm" : "" + sum.ToStringMoney();


            //Lấy giá trị ghi chú của đơn vị            
            var dtDongTrangTren = getGhiChuDatatableUp(chungTu.iID_MaDonVi);
            var dtDongTrangDuoi = getGhiChuDatatableDown(chungTu.iID_MaDonVi);
            fr.AddTable("dtDongTrangUp", dtDongTrangTren);
            fr.AddTable("dtDongTrangDown", dtDongTrangDuoi);
            fr.SetValue("Tien", tien);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS1", dtsLNS1);
            fr.AddTable("dtsLNS3", dtsLNS3);
            fr.AddTable("dtsLNS5", dtsLNS5);
            fr.SetValue("upOrDown", upOrDown);

            fr.SetExpression("RowH", "<#Row height(Autofit;" + khoangCachDong + ")>");
            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtsLNS1.Dispose();
            dtsLNS3.Dispose();
            dtsLNS5.Dispose();
            dtDongTrangTren.Dispose();
            dtDongTrangDuoi.Dispose();
        }

        private string getFilePath(string loai, string muc)
        {
            if (loai.Compare("1"))
            {
                return "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri_TongHop.xls";
            }
            else
            {
                if (muc.Compare("1"))
                {
                    return "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri_ChiTiet_Nganh.xls";
                }
                else
                {
                    return "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri_ChiTiet_Muc.xls";
                }
            }
        }

        private DataTable getGhiChuDatatableUp(string iID_MaDonVi)
        {
            var sTen = $"thongtri_qt_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaChungTu.ToString()}_{iID_MaDonVi}";
            var ghichu = _quyetToanService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);

            var dt = new DataTable();
            dt.Columns.Add("sGhiChu");

            var words = ghichu.Split(new string[] { "\n", "&#10;" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            words.ForEach(w =>
            {
                var lines = w.Replace("  ", " ");               

               
                var row = dt.NewRow();
                row[0] = lines;
                dt.Rows.Add(row);
               
            });

            return dt;
        }

        private DataTable getGhiChuDatatableDown(string iID_MaDonVi)
        {
            var sTen = $"thongtri_qt_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaChungTu.ToString()}_{iID_MaDonVi}";
            var ghichu = _quyetToanService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);

            var dt = new DataTable();
            dt.Columns.Add("sGhiChu");

            var words = ghichu.Split(new string[] { "\n", "&#10;" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            words.ForEach(w =>
            {
                var lines = w.Replace("  ", " ");
                              
                var row = dt.NewRow();
                row[0] = lines;
                dt.Rows.Add(row);
                
            });

            return dt;
        }
        #endregion
    }
}
