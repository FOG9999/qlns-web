using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.Ajax.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptCapPhat_ThongTriViewModel
    {
        public CP_CapPhat chungTu { get; set; }
        public string iID_MaPhongBan { get; set; }
        public SelectList dsLoaiCapPhat { get; set; }
        public SelectList dsLoaiThongTri { get; set; }
        [DataType(DataType.Date)]
        public DateTime dNgayIn { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.CapPhat
{
    public class rptCapPhat_ThongTriController : FlexcelReportController
    {

        #region var def

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly ICapPhatService _capPhatService = CapPhatService.Default;

        private string _filePath;
        private CP_CapPhat chungTu;
        private string loaiCapPhat;
        private string loaiThongTri;
        private string sLNSs;
        private string iID_MaDonVi;
        private int _muc;
        private string chuky;
        private string thualenh;
        private string loai;
        private string khoangcachdong;
        private DateTime date;
        private string upOrDown;

        public ActionResult Index(string iID_MaCapPhat)
        {

            var view = "~/Views/Report_Views/CapPhat/" + this.ControllerName() + ".cshtml";
            var chungTu = _capPhatService.GetChungTu(new Guid(iID_MaCapPhat));

            var vm = new rptCapPhat_ThongTriViewModel
            {
                chungTu = chungTu,
                iID_MaPhongBan = PhienLamViec.iID_MaPhongBan,
                dsLoaiCapPhat = _capPhatService.GetLoaiCapPhat().ToSelectList(),
                dsLoaiThongTri = _capPhatService.GetLoaiThongTri(chungTu.iLoai.GetValueOrDefault(), chungTu.sDSLNS).ToSelectList("loaiThongTri", "TenThongTri", "-1", "---- Chọn loại thông tri ----"),
                dNgayIn = DateTime.Now
            };

            return View(view, vm);
        }
        #endregion

        #region public methods
        public ActionResult Print(
            string iID_MaCapPhat,
            string loaiCapPhat,
            string loaiThongTri,
            string sLNS,
            string iID_MaDonVi,
            string loai,
            string chuky,
            string thualenh,
            int muc,
            string khoangCachDong,
            string upOrDown,
            string ext,
            string ghiChu,
            string date)
        {
            this.khoangcachdong = khoangCachDong;
            this.chungTu = _capPhatService.GetChungTu(new Guid(iID_MaCapPhat));
            this.loaiCapPhat = loaiCapPhat;
            this.loaiThongTri = loaiThongTri;
            this.sLNSs = sLNS;
            this.iID_MaDonVi = iID_MaDonVi;
            this.loai = loai;
            this.chuky = chuky;
            this.thualenh = thualenh;
            this._muc = muc;
            this.date = Convert.ToDateTime(date);
            this.upOrDown = upOrDown;

            var xls = createReport();
            return Print(xls, ext);
        }

        public JsonResult Ds_LNS(
            string iID_MaCapPhat,
            string loaiThongTri)
        {
            var data = _capPhatService.GetLnsThongTri(iID_MaCapPhat, loaiThongTri);
            var vm = new ChecklistModel("LNS", data.ToSelectList("sLNS", "TenHT"));

            return ToCheckboxList(vm);
        }

        public JsonResult Ds_DonVi(
            string iID_MaCapPhat,
            string loaiThongTri,
            string sLNS)
        {
            var data = _capPhatService.GetDonViThongTri(iID_MaCapPhat, loaiThongTri, sLNS);
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "TenHT"));

            return ToCheckboxList(vm);
        }

        public JsonResult GhiChu(string iID_MaCapPhat, string iID_MaDonVi, string loaiThongTri)
        {
            var sTen = $"thongtri_cp_{PhienLamViec.iNamLamViec}_{iID_MaCapPhat}_{iID_MaDonVi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GhiChu_Update(string iID_MaCapPhat, string iID_MaDonVi, string loaiThongTri, string sGhiChu)
        {
            var sTen = $"thongtri_cp_{PhienLamViec.iNamLamViec}_{iID_MaCapPhat}_{iID_MaDonVi}_{loaiThongTri}";
            var success = _capPhatService.UpdateGhiChuThongTri(Username, sTen, iID_MaDonVi, sGhiChu);
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region private methods       

        private string getFilePath(string loai, int muc)
        {
            if (loai.Compare("1"))
            {
                return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_TongHop.xls";
            }
            else
            {
                if (_muc == 1)
                {
                    return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_ChiTiet_Nganh.xls";
                }
                else if (_muc == 2)
                {
                    return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_ChiTiet_TieuMuc.xls";
                }
                else
                {
                    return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_ChiTiet_Muc.xls";
                }
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
           
            string Thang = date.Year > PhienLamViec.NamLamViec ? "12" : date.Month.ToString();
            string sTenDonVi = "";
            if (loai == "0")
            {
                sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen;
            }
            string capPhat = "";
            if (loaiCapPhat == "-1")
            {
                capPhat = $"Cấp {_capPhatService.GetLoaiThongTri()[loaiThongTri].ToLower()}";
            }
            else
            {
                capPhat = $"{_capPhatService.GetLoaiCapPhat()[loaiCapPhat]} {_capPhatService.GetLoaiThongTri()[loaiThongTri].ToLower()}";
            }

            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, Username));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, Username));
            fr.SetValue("NgayThang", (date.Year < PhienLamViec.NamLamViec) ? "Ngày ... tháng ... năm " + date.Year : "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year);

            fr.SetValue("MTT", L("ThongTri"));
            fr.SetValue("Nam", PhienLamViec.iNamLamViec);
            fr.SetValue("NgayLap", (date.Year < PhienLamViec.NamLamViec) ? " năm " + PhienLamViec.iNamLamViec : "tháng " + Thang + " năm " + PhienLamViec.iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("LoaiCapPhat", capPhat.Trim());
            fr.SetValue("soChuky", chuky);
            fr.SetValue("soTL", thualenh);

            var xls = new XlsFile(true);
            _filePath = getFilePath(loai, _muc);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKyForController(this.ControllerName());

            fr.Run(xls);

            var count = xls.TotalPageCount();
            if (count > 1)
            {
                xls.AddPageFirstPage();
            }

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
                data = _capPhatService.CapPhatThongTriChiTiet(PhienLamViec.iNamLamViec, sLNSs, iID_MaDonVi, chungTu.iID_MaCapPhat.ToString());
                if (loaiCapPhat == "2" || loaiCapPhat == "3")
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
                    r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS5"]),User.Identity.Name);
                }

                dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa");
                for (int i = 0; i < dtsLNS3.Rows.Count; i++)
                {
                    r = dtsLNS3.Rows[i];
                    r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS3"]), User.Identity.Name);
                }

                dtsLNS1 = HamChung.SelectDistinct("dtsLNS1", dtsLNS3, "sLNS1", "sLNS1,sMoTa");
                for (int i = 0; i < dtsLNS1.Rows.Count; i++)
                {
                    r = dtsLNS1.Rows[i];
                    r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS1"]), User.Identity.Name);
                }

            }
            //Hiển thị tổng hợp
            else
            {
                dtDonVi = _capPhatService.CapPhatThongTriTongHop(PhienLamViec.iNamLamViec, sLNSs, iID_MaDonVi, chungTu.iID_MaCapPhat.ToString());
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
            var tien = (sum < 0 ? "Giảm " + sum.ToStringMoney().ToLower() : "" + sum.ToStringMoney());


            //Lấy giá trị ghi chú của đơn vị            
            var dtDongTrangTren = getGhiChuDatatableUp(iID_MaDonVi);
            var dtDongTrangDuoi = getGhiChuDatatableDown(iID_MaDonVi);
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

            fr.SetExpression("RowH", "<#Row height(Autofit;" + khoangcachdong + ")>");
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

        private DataTable getGhiChuDatatableUp(string iID_MaDonVi)
        {
            var sTen = $"thongtri_cp_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaCapPhat.ToString()}_{iID_MaDonVi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);

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
            var sTen = $"thongtri_cp_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaCapPhat.ToString()}_{iID_MaDonVi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);

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
