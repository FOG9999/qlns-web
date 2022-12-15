using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Common;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class QLThongTriQuyetToanController : FlexcelReportController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        private const string sFilePath = "/Report_ExcelFrom/VonDauTu/rpt_ThongTriQuyetToan.xls";

        // GET: QLVonDauTu/QLThongTriQuyetToan
        public ActionResult Index()
        {
            VDTThongTriViewModel vm = new VDTThongTriViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _iQLVonDauTuService.GetThongTriQuyetToanPaging(ref vm._paging);
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.TAT_CA });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sMoTa");
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sMaDonVi, string sMaThongTri, DateTime? dNgayThongTri, int? iNamThongTri)
        {
            VDTThongTriViewModel vm = new VDTThongTriViewModel();
            vm._paging = _paging;
            vm.Items = _iQLVonDauTuService.GetThongTriQuyetToanPaging(ref vm._paging, sMaDonVi, sMaThongTri, iNamThongTri, dNgayThongTri);
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.TAT_CA });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sMoTa");

            // luu dieu kien tim kiem
            TempData["sMaDonvi"] = sMaDonVi;
            TempData["sMaThongTri"] = sMaThongTri;
            TempData["dNgayThongTri"] = dNgayThongTri;
            TempData["iNamThongTri"] = iNamThongTri;

            return PartialView("_list", vm);
        }

        public ActionResult TaoMoi()
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            ViewBag.dNgayThongTri = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.iIdLoaiThongTri = _iQLVonDauTuService.GetLoaiThongTri().FirstOrDefault(n => n.iKieuLoaiThongTri == (int)Viettel.Extensions.Constants.VDT_KIEULOAI_THONGTRI.THONG_TRI_QUYET_TOAN).iID_LoaiThongTriID;
            return View(new List<VDTGetThongTriChiTietViewModel>());
        }
        public ActionResult ChiTiet(string id)
        {
            VDTThongTriModel model = _iQLVonDauTuService.LayChiTietThongTri(id);
            ViewBag.iLoaiCap = (model.bThanhToan.HasValue && model.bThanhToan.Value) ? 1 : 0;
            return View(model);
        }

        public ActionResult Sua(string id, int bIsViewDetail = 0)
        {
            ViewBag.bIsViewDetail = bIsViewDetail;
            VDTThongTriModel model = _iQLVonDauTuService.LayChiTietThongTri(id);

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sMoTa");
            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("sMoTa", "sTen");
            ViewBag.dNgayThongTri = (model != null && model.dNgayThongTri.HasValue) ? model.dNgayThongTri.Value.ToString("dd/MM/yyyy") : string.Empty;

            var objQuyetToan = _iQLVonDauTuService.GetBaoCaoQuyetToanById(model.iID_BCQuyetToanNienDo);
            List<SelectListItem> lstQuyetToan = new List<SelectListItem>();
            if (objQuyetToan != null)
                lstQuyetToan.Add(new SelectListItem() { Value = objQuyetToan.iID_BCQuyetToanNienDoID.ToString(), Text = objQuyetToan.sSoDeNghi });
            ViewBag.lstQuyetToan = lstQuyetToan.ToSelectList();

            return View(model);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            bool xoa = _iQLVonDauTuService.XoaThongTri(Guid.Parse(id));
            return Json(xoa, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongTriQuyetToanDetail(Guid iIDQuyetToanId, Guid? iIdThongTri)
        {
            List<VDTGetThongTriChiTietViewModel> lstDataDeNghiThanhToan = new List<VDTGetThongTriChiTietViewModel>();
            if (iIdThongTri.HasValue)
            {
                lstDataDeNghiThanhToan = _iQLVonDauTuService.GetThongTriQuyetToanChiTietByThongTriId(iIdThongTri.Value).ToList();
                if (lstDataDeNghiThanhToan.Count != 0) return Json(new { data = lstDataDeNghiThanhToan }, JsonRequestBehavior.AllowGet);
            }
            lstDataDeNghiThanhToan = _iQLVonDauTuService.GetThongTriChiTiet(iIDQuyetToanId).ToList();
            return Json(new { data = lstDataDeNghiThanhToan }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDeNghiThanhToanChiTietUng(string sMaDonVi, int iNamThongTri, int iNguonVon, DateTime? dNgayLapGanNhat, DateTime? dNgayTaoThongTri)
        {
            string sMaNhomQuanLy = string.Empty;
            if (iNguonVon == (int)Viettel.Extensions.Constants.NS_NGUON_NGAN_SACH.NS_QUOC_PHONG)
                sMaNhomQuanLy = "CTC";
            else if (iNguonVon == (int)Viettel.Extensions.Constants.NS_NGUON_NGAN_SACH.NS_NHA_NUOC
                || iNguonVon == (int)Viettel.Extensions.Constants.NS_NGUON_NGAN_SACH.NS_DAC_BIET
                || iNguonVon == (int)Viettel.Extensions.Constants.NS_NGUON_NGAN_SACH.NS_KHAC)
                sMaNhomQuanLy = "CKHDT";
            var lstDataDeNghiThanhToanUng = _iQLVonDauTuService.GetDeNghiThanhToanChiTietUng(sMaDonVi, iNamThongTri, sMaNhomQuanLy, dNgayLapGanNhat, dNgayTaoThongTri);
            return Json(new { data = lstDataDeNghiThanhToanUng }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayNgayLapGanNhat(string iIDDonViID, int iNamThongTri, int iNguonVon)
        {
            string dNgayLapGanNhat = _iQLVonDauTuService.LayNgayLapGanNhat(iIDDonViID, iNamThongTri, iNguonVon);
            return Json(dNgayLapGanNhat == null ? "" : dNgayLapGanNhat, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDanhSachNguonVonTheoLoaiCap(int iLoaiCap)
        {
            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            if (iLoaiCap == (int)Viettel.Extensions.Constants.LOAI_CAP.UNG_NGOAI)
            {
                List<string> listSMoTaKeep = new List<string> { "0212", "0213", "0216", "0220" };
                lstNguonVon = lstNguonVon.Where(x => listSMoTaKeep.Contains(x.sMoTa)).ToList();
            }

            StringBuilder htmlString = new StringBuilder();
            if (lstNguonVon != null && lstNguonVon.Count > 0)
            {
                htmlString.AppendFormat("<option value='{0}'>{1}</option>", "", Constants.CHON);
                for (int i = 0; i < lstNguonVon.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}'>{1}</option>", lstNguonVon[i].iID_MaNguonNganSach, lstNguonVon[i].sTen);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// kiem tra trung ma thong tri
        /// </summary>
        /// <param name="sMaThongTri"></param>
        /// <returns></returns>
        public JsonResult KiemTraTrungMaThongTri(string sMaThongTri, string iID_ThongTriID = "")
        {
            bool status = _iQLVonDauTuService.KiemTraTrungMaThongTri(sMaThongTri, iID_ThongTriID);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Luu(VDT_ThongTri model, List<VDT_ThongTri_ChiTiet> lstDetail)
        {
            bool status = _iQLVonDauTuService.LuuThongTinThongTriQuyetToan(model, lstDetail, Username);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiTietThongTriChiTiet(string iID_ThongTriID)
        {
            VDTThongTriModel model = _iQLVonDauTuService.LayChiTietThongTri(iID_ThongTriID);
            IEnumerable<VDT_DM_KieuThongTri> danhSachKieuThongTri = _iQLVonDauTuService.LayDanhSachKieuThongTri();
            if (model.bThanhToan.HasValue)
            {
                int iThanhToan = model.bThanhToan.Value ? 1 : 0;
                if (iThanhToan == (int)Viettel.Extensions.Constants.LOAI_CAP.THANH_TOAN)
                {
                    var lstTab1 = _iQLVonDauTuService.LayThongTriChiTietTheoKieuThongTri(iID_ThongTriID, danhSachKieuThongTri.Where(x => x.sMaKieuThongTri == Viettel.Extensions.Constants.CAP_TT_KPQP).First().iID_KieuThongTriID.ToString());
                    var lstTab2 = _iQLVonDauTuService.LayThongTriChiTietTheoKieuThongTri(iID_ThongTriID, danhSachKieuThongTri.Where(x => x.sMaKieuThongTri == Viettel.Extensions.Constants.CAP_TAM_UNG_KPQP).First().iID_KieuThongTriID.ToString());
                    var lstTab3 = _iQLVonDauTuService.LayThongTriChiTietTheoKieuThongTri(iID_ThongTriID, danhSachKieuThongTri.Where(x => x.sMaKieuThongTri == Viettel.Extensions.Constants.THU_UNG_KPQP).First().iID_KieuThongTriID.ToString());
                    return Json(new { lstTab1 = lstTab1, lstTab2 = lstTab2, lstTab3 = lstTab3, lstTab4 = "", lstTab5 = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult XuatFile(Guid Id, string type )
        {
            var obj = _iQLVonDauTuService.GetThongTriById(Id.ToString());
            var lstData = _iQLVonDauTuService.LayDanhSachThongTriXuatFile(Id);
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VdtThongTriQuyetToanReportModel>("dt", lstData);
            fr.SetValue("STenDonVi", obj.TenDonVi);
            fr.SetValue("iNamThongTri", obj.iNamThongTri);
            fr.SetValue("sNgayHienTai", string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year));
            fr.SetValue("SumTotal", lstData.Sum(n => n.FSoTien));
            fr.SetValue("sTienBangChu", DataHelper.NumberToText(lstData.Sum(n => n.FSoTien), true));
            fr.Run(Result);
            return Print(Result, type, "rpt_vdt_thongtriquyettoan.xlsx");
        }

        public JsonResult GetChungTuQuyetToanNienDo(Guid? iIdThongTri, string iIdMaDonVi, int iNamThucHien, int iIdNguonVon)
        {
            List<VDT_QT_BCQuyetToanNienDo> lstChungTu = _iQLVonDauTuService.GetQuyetToanNienDoInThongTri(iIdThongTri ?? Guid.Empty, iIdMaDonVi, iNamThucHien, iIdNguonVon).ToList();
            return Json(new { lstChungTu }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListDataDonViQuanLy()
        {
            return Json(new { results = CommonFunction.GetListDataDonViQuanLy() }, JsonRequestBehavior.AllowGet);
        }
    }
}
