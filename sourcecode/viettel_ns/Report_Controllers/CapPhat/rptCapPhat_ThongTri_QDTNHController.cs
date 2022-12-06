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
    public class rptCapPhat_ThongTri_QDTNHViewModel
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
    public class rptCapPhat_ThongTri_QDTNHController : FlexcelReportController
    {

        #region var def

        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly ICapPhatService _capPhatService = CapPhatService.Default;

        private string _filePath;
        private CP_CapPhat chungTu;
        private string loaiThongTri;
        private string loaiCapPhat;
        private string iID_MaDonVi;
        private string khoangcachdong;
        private DateTime date;

        public ActionResult Index(string iID_MaCapPhat)
        {

            var view = "~/Views/Report_Views/CapPhat/" + this.ControllerName() + ".cshtml";
            var chungTu = _capPhatService.GetChungTu(new Guid(iID_MaCapPhat));
            var loaiThongTri = _capPhatService.GetLoaiThongTri(chungTu.iLoai.GetValueOrDefault(), chungTu.sDSLNS).Clone();
            var nr = loaiThongTri.NewRow();
            nr["loaiThongTri"] = "1";
            nr["TenThongTri"] = "Cấp Quỹ DTNH chi trong nước";
            loaiThongTri.Rows.Add(nr);
            nr = loaiThongTri.NewRow();
            nr["loaiThongTri"] = "2";
            nr["TenThongTri"] = "Cấp Quỹ DTNH chi ngoại tệ";
            loaiThongTri.Rows.Add(nr);

            var vm = new rptCapPhat_ThongTri_QDTNHViewModel
            {
                chungTu = chungTu,
                iID_MaPhongBan = PhienLamViec.iID_MaPhongBan,
                dsLoaiCapPhat = _capPhatService.GetLoaiCapPhat().ToSelectList(),
                dsLoaiThongTri = loaiThongTri.ToSelectList("loaiThongTri", "TenThongTri", "-1", "---- Chọn loại thông tri ----"),
                dNgayIn = DateTime.Now
            };

            return View(view, vm);
        }
        #endregion

        #region public methods
        public ActionResult Print(
            string iID_MaCapPhat,
            string loaiThongTri,
            string loaiCapPhat,
            string iID_MaDonVi,
            string khoangCachDong,
            string ext,
            string date)
        {
            this.khoangcachdong = khoangCachDong;
            this.chungTu = _capPhatService.GetChungTu(new Guid(iID_MaCapPhat));
            this.loaiThongTri = loaiThongTri;
            this.loaiCapPhat = loaiCapPhat;
            this.iID_MaDonVi = iID_MaDonVi;
            this.date = Convert.ToDateTime(date);

            var xls = createReport();
            return Print(xls, ext);
        }        

        public JsonResult Ds_DonVi(
            string iID_MaCapPhat)
        {
            var data = _capPhatService.GetDonViThongTri(iID_MaCapPhat, null, "3010000");
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "TenHT"));

            return ToCheckboxList(vm);
        }

        public JsonResult GetValue(string iID_MaCapPhat, string iID_MaDonVi, string loaiThongTri, string loai)
        {
            var sTen = $"thongtri_cpnt_{PhienLamViec.iNamLamViec}_{iID_MaCapPhat}_{iID_MaDonVi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetValueThongTriNT(Username, sTen, iID_MaDonVi, loai);
            if (loai == "1")
            {
                if ((string.IsNullOrEmpty(ghichu) || !ghichu.StartsWith("Cấp kinh phí")) && loaiThongTri == "1")
                {
                    ghichu = "Cấp kinh phí nguồn Quỹ DTNH số .......... ngày ..../..../....... giữa (chủ đầu tư) và (nhà thầu) .... thuộc Gói thầu số ...... hoặc thanh toán chi phí trong nước của HĐNK số ... ngày ... tháng ... năm ... để nhập khẩu ...";
                }
                else if ((string.IsNullOrEmpty(ghichu) || !ghichu.StartsWith("Thanh toán")) && loaiThongTri == "2")
                {
                    ghichu = "Thanh toán HĐNK số ... ngày ... tháng ... năm ... để nhập khẩu ... \n Số tiền: \n Ngoại tệ (USD) x tỷ giá = \n hoặc Ngoại tệ (EUR) ~ ... USD x tỷ giá = ";
                }
            }
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Value_Update(string iID_MaCapPhat, string iID_MaDonVi, string loaiThongTri, string value, string loai)
        {
            var sTen = $"thongtri_cpnt_{PhienLamViec.iNamLamViec}_{iID_MaCapPhat}_{iID_MaDonVi}_{loaiThongTri}";
            var success = _capPhatService.UpdateValueThongTriNT(Username, sTen, iID_MaDonVi, value, loai);
            return Json(success, JsonRequestBehavior.AllowGet);
        }    
        #endregion

        #region private methods       

        private string getFilePath(string loai)
        {
            if (loai.Compare("1"))
            {
                return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_QDTNH_TV.xls";
            }
            else
            {
                return "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_QDTNH_NT.xls";
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
            
            var sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, iID_MaDonVi).sTen;                    

            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, Username));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, Username));
            fr.SetValue("NgayThang", (date.Year < PhienLamViec.NamLamViec) ? "Ngày ... tháng ... năm " + date.Year : "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year);

            fr.SetValue("MTT", L("ThongTri"));
            fr.SetValue("Nam", PhienLamViec.iNamLamViec);
            fr.SetValue("NgayLap", (date.Year < PhienLamViec.NamLamViec) ? " năm " + PhienLamViec.iNamLamViec : "tháng " + Thang + " năm " + PhienLamViec.iNamLamViec);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("LoaiCapPhat", $"Cấp kinh phí nguồn Quỹ Dự trữ Ngoại hối");

            var xls = new XlsFile(true);
            _filePath = getFilePath(loaiThongTri);
            xls.Open(Server.MapPath(_filePath));

            fr.UseChuKyForController("rptCapPhat_ThongTri");

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
            data = _capPhatService.CapPhatThongTriChiTiet(PhienLamViec.iNamLamViec, "3010000", iID_MaDonVi, chungTu.iID_MaCapPhat.ToString());
                
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

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

            //In loại tiền bằng chữ
            var tien = data.Sum("rTuChi").ToStringMoney();

            //Lấy giá trị ghi chú của đơn vị            
            var dtGhiChu = getGhiChu(iID_MaDonVi); 
            fr.AddTable("dtGhiChu", dtGhiChu);
            fr.SetValue("Tien", tien);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS1", dtsLNS1);
            fr.AddTable("dtsLNS3", dtsLNS3);
            fr.AddTable("dtsLNS5", dtsLNS5);
            fr.SetValue("TyGia", GetStringValue(iID_MaDonVi, "2"));
            fr.SetValue("ThongBao", GetStringValue(iID_MaDonVi, "3"));
            fr.SetValue("DonViNhan", GetStringValue(iID_MaDonVi, "4"));            

            fr.SetExpression("RowH", "<#Row height(Autofit;" + khoangcachdong + ")>");
            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtsLNS1.Dispose();
            dtsLNS3.Dispose();
            dtsLNS5.Dispose();
            dtGhiChu.Dispose();
        }

        private DataTable getGhiChu(string iID_MaDonVi)
        {
            var sTen = $"thongtri_cpnt_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaCapPhat.ToString()}_{iID_MaDonVi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetValueThongTriNT(Username, sTen, iID_MaDonVi, "1");

            if (string.IsNullOrEmpty(ghichu))
            {
                if (loaiThongTri == "1")
                {
                    ghichu = "Cấp kinh phí nguồn Quỹ DTNH số .......... ngày ..../..../....... giữa (chủ đầu tư) và (nhà thầu) .... thuộc Gói thầu số ...... hoặc thanh toán chi phí trong nước của HĐNK số ... ngày ... tháng ... năm ... để nhập khẩu ...";
                }
                else
                {
                    ghichu = "Thanh toán HĐNK số ... ngày ... tháng ... năm ... để nhập khẩu ... Số tiền: \n Ngoại tệ (USD) x tỷ giá = \n hoặc Ngoại tệ (EUR) ~ ... USD x tỷ giá = ";
                }
            }

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
        public string GetStringValue(string iID_MaDonVi, string loai)
        {
            var sTen = $"thongtri_cpnt_{PhienLamViec.iNamLamViec}_{chungTu.iID_MaCapPhat.ToString()}_{iID_MaDonVi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetValueThongTriNT(Username, sTen, iID_MaDonVi, loai);
            
            return ghichu;
        }
        #endregion

    }
}
