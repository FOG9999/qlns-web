using System;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using DomainModel;
using VIETTEL.Models;
using System.Text.RegularExpressions;
using System.Collections;
using VIETTEL.Models.CapPhat;
using System.Collections.Specialized;
using VIETTEL.Controllers;
using Viettel.Services;
using VIETTEL.Flexcel;
using System.Linq;
using Microsoft.Ajax.Utilities;
using Viettel.Extensions;
using Viettel;
using Viettel.Domain.DomainModel;

namespace VIETTEL.Models
{
    public class rptCapPhat_ThongTri_SecDMNBViewModel
    {
        public string iID_MaCapPhat { get; set; }
        public string iID_MaPhongBan { get; set; }
        public ChecklistModel dsNganh { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.CapPhat
{
    public class rptCapPhat_ThongTri_SecDMNBController : FlexcelReportController
    {
        #region var def

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly ICapPhatService _capPhatService = CapPhatService.Default;

        private string _filePath;
        private CP_CapPhat chungTu;
        private string nganhs;
        private string iID_MaDonVi;
        private string loai;
        private string chuky;
        private string thualenh;
        private string khoangcachdong;
        private DateTime date;
        private string upOrDown;

        public ActionResult Index(string iID_MaCapPhat)
        {

            var view = "~/Views/Report_Views/CapPhat/" + this.ControllerName() + ".cshtml";

            var vm = new rptCapPhat_ThongTri_SecDMNBViewModel
            {
                iID_MaCapPhat = iID_MaCapPhat,
                iID_MaPhongBan = PhienLamViec.iID_MaPhongBan,
                dsNganh = new ChecklistModel("Nganh", _capPhatService.GetNgThongTri(iID_MaCapPhat).ToSelectList("sLNS", "TenHT"))
            };

            return View(view, vm);
        }
        #endregion

        #region public methods
        public ActionResult Print(
            string iID_MaCapPhat,
            string nganhs,
            string iID_MaDonVi,
            string loai,
            string chuky,
            string thualenh,
            string upOrDown,
            string khoangCachDong,
            string ext,
            string date)
        {
            this._filePath = getFilePath(loai);
            this.chungTu = _capPhatService.GetChungTu(new Guid(iID_MaCapPhat));
            this.nganhs = nganhs;
            this.iID_MaDonVi = iID_MaDonVi;
            this.loai = loai;
            this.chuky = chuky;
            this.thualenh = thualenh;
            this.khoangcachdong = khoangCachDong;
            this.date = Convert.ToDateTime(date);
            this.upOrDown = upOrDown;

            var xls = createReport();
            return Print(xls, ext);
        }

        public JsonResult Ds_DonVi(
            string iID_MaCapPhat,
            string nganh)
        {
            var data = _capPhatService.GetDviNgThongTri(iID_MaCapPhat, nganh);
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "TenHT"));

            return ToCheckboxList(vm);
        }
        public JsonResult GhiChu(string iID_MaCapPhat, string iID_MaDonVi)
        {
            var sTen = $"thongtri_cpsec_{PhienLamViec.iNamLamViec}_{iID_MaCapPhat}_{iID_MaDonVi}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GhiChu_Update(string iID_MaCapPhat, string iID_MaDonVi, string sGhiChu)
        {
            var sTen = $"thongtri_cpsec_{PhienLamViec.iNamLamViec}_{iID_MaCapPhat}_{iID_MaDonVi}";
            var success = _capPhatService.UpdateGhiChuThongTri(Username, sTen, iID_MaDonVi, sGhiChu);
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region private methods        
        private string getFilePath(string loai)
        {
            if (loai.Compare("1"))
            {
                return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_TongHop_SecDMNB.xls";
            }
            else
            {
                return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_ChiTiet_SecDMNB.xls";
            }
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);

            //Lấy tháng của đợt cấp phát
            string Thang = chungTu.dNgayCapPhat.Month.ToString();
            string sTenDonVi = "";
            if (loai == "0")
            {
                sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen;
            }

            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, Username));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, Username));
            fr.SetValue("NgayThang", "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year);
            fr.SetValue("Nam", PhienLamViec.iNamLamViec);
            fr.SetValue("NgayLap", "tháng " + Thang + " năm " + PhienLamViec.iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("LoaiCapPhat", "Cấp Séc Định mức nội bộ");
            fr.SetValue("soChuky", chuky);
            fr.SetValue("soTL", thualenh);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(this._filePath));

            fr.UseChuKyForController(this.ControllerName());

            fr.Run(xls);

            return xls;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        private void LoadData(FlexCelReport fr)
        {
            DataRow r;
            DataTable data = new DataTable();
            DataTable dtDonVi = new DataTable();

            //Hiển thị chi tiết
            if (loai == "0")
            {
                data = _capPhatService.CapPhatThongTriSecChiTiet(chungTu.iID_MaCapPhat.ToString(), nganhs, iID_MaDonVi);
            }

            //Hiển thị tổng hợp
            else
            {
                dtDonVi = _capPhatService.CapPhatThongTriSecTongHop(chungTu.iID_MaCapPhat.ToString(), nganhs, iID_MaDonVi);
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
            var dtDongTrangTren = getGhiChuDatatableUp(iID_MaDonVi);
            var dtDongTrangDuoi = getGhiChuDatatableDown(iID_MaDonVi);
            fr.AddTable("dtDongTrangUp", dtDongTrangTren);
            fr.AddTable("dtDongTrangDown", dtDongTrangDuoi);
            fr.SetValue("Tien", tien);
            fr.SetValue("upOrDown", upOrDown);

            fr.SetExpression("RowH", "<#Row height(Autofit;" + khoangcachdong + ")>");
            data.Dispose();
            dtDongTrangTren.Dispose();
            dtDongTrangDuoi.Dispose();
        }

        private DataTable getGhiChuDatatableUp(string iID_MaDonVi)
        {
            var sTen = $"thongtri_cpsec_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaCapPhat.ToString()}_{iID_MaDonVi}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);

            var dt = new DataTable();
            dt.Columns.Add("sGhiChu");

            var words = ghichu.Split(new string[] { "\n", "&#10;" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            words.ForEach(w =>
            {
                var lines = w.Replace("  ", " ").Split(new char[] { ' ' })
                .ToList()
                .ChunkByChar();

                lines.ForEach(x =>
                {
                    var row = dt.NewRow();
                    row[0] = x.Join(" ");
                    dt.Rows.Add(row);
                });
            });

            return dt;
        }

        private DataTable getGhiChuDatatableDown(string iID_MaDonVi)
        {
            var sTen = $"thongtri_cpsec_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaCapPhat.ToString()}_{iID_MaDonVi}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);

            var dt = new DataTable();
            dt.Columns.Add("sGhiChu");

            var words = ghichu.Split(new string[] { "\n", "&#10;" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            words.ForEach(w =>
            {
                var row = dt.NewRow();
                row[0] = w;
                dt.Rows.Add(row);
            });

            return dt;
        }
        #endregion

    }
}

