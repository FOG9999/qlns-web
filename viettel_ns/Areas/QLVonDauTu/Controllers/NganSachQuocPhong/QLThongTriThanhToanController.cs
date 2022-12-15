using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Common;
using System.Text;
using DapperExtensions;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using FlexCel.Report;
using System.IO;
using System.Collections.ObjectModel;
using Viettel.Models.QLNguonNganSach;
using VIETTEL.Helpers;
using VIETTEL.Flexcel;
using static VTS.QLNS.CTC.App.Service.UserFunction.FormatNumber;
using VTS.QLNS.CTC.App.Service.UserFunction;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class QLThongTriThanhToanController : FlexcelReportController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        private const string sFilePath = "/Report_ExcelFrom/VonDauTu/rpt_ThongTri_Danhsach.xls";
        private const string sControlName = "QLThongTriThanhToan";


        // GET: QLVonDauTu/QLThongTriThanhToan
        public ActionResult Index()
        {
            VDTThongTriViewModel vm = new VDTThongTriViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _iQLVonDauTuService.LayDanhSachThongTri(ref vm._paging, PhienLamViec.NamLamViec, Username);

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.TAT_CA });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sMoTa");
            return View(vm);
        }

        /// <summary>
        /// tìm kiếm thông tri trên trang thông tri - full 4 loại
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sMaDonVi, string sMaThongTri, DateTime? dNgayThongTri, int? iNamThongTri)
        {
            VDTThongTriViewModel vm = new VDTThongTriViewModel();
            vm._paging = _paging;
            vm.Items = _iQLVonDauTuService.LayDanhSachThongTri(ref vm._paging, PhienLamViec.NamLamViec, Username, false, sMaDonVi, sMaThongTri, iNamThongTri, dNgayThongTri);
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

        /// <summary>
        /// tìm kiếm thông tri trên trang thanh toán - chỉ 2 loại: thanh toán, tạm ứng
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TimKiemThanhToanView(PagingInfo _paging, string sMaDonVi, string sMaThongTri, DateTime? dNgayThongTri, int? iNamThongTri)
        {
            VDTThongTriViewModel vm = new VDTThongTriViewModel();
            vm._paging = _paging;
            vm.Items = _iQLVonDauTuService.LayDanhSachThongTri(ref vm._paging, PhienLamViec.NamLamViec, Username, true, sMaDonVi, sMaThongTri, iNamThongTri, dNgayThongTri);
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
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("sMoTa", "sTen");

            List<SelectListItem> lstLoaiThongTri = new List<SelectListItem>()
            {
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_KINH_PHI,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_KINH_PHI).ToString()},
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_HOP_THUC,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_HOP_THUC).ToString()}
            };
            lstLoaiThongTri.Insert(0, new SelectListItem { Text = Constants.CHON, Value = 0.ToString() });
            ViewBag.ListLoaiThongTri = lstLoaiThongTri.ToSelectList();


            List<SelectListItem> lstNamNganSach = new List<SelectListItem>()
            {
                new SelectListItem{Text=Constants.NamNganSach.TypeName.NAM_TRUOC_CHUYEN_SANG,Value=((int)Constants.NamNganSach.Type.NAM_TRUOC_CHUYEN_SANG).ToString()},
                new SelectListItem{Text=Constants.NamNganSach.TypeName.NAM_NAY,Value=((int)Constants.NamNganSach.Type.NAM_NAY).ToString()}
            };
            lstNamNganSach.Insert(0, new SelectListItem { Text = Constants.CHON, Value = 0.ToString() });
            ViewBag.ListNamNganSach = lstNamNganSach.ToSelectList();
            return View();
        }

        public ActionResult ChiTiet(string id)
        {
            VDTThongTriModel model = _iQLVonDauTuService.LayChiTietThongTri(id);
            //ViewBag.iLoaiCap = (model.bThanhToan.HasValue && model.bThanhToan.Value) ? 1 : 0;
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("sMoTa", "sTen");

            List<SelectListItem> lstLoaiThongTri = new List<SelectListItem>()
            {
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_KINH_PHI,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_KINH_PHI).ToString()},
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_HOP_THUC,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_HOP_THUC).ToString()}
            };
            lstLoaiThongTri.Insert(0, new SelectListItem { Text = Constants.CHON, Value = 0.ToString() });
            ViewBag.ListLoaiThongTri = lstLoaiThongTri.ToSelectList();


            List<SelectListItem> lstNamNganSach = new List<SelectListItem>()
            {
                new SelectListItem{Text=Constants.NamNganSach.TypeName.NAM_TRUOC_CHUYEN_SANG,Value=((int)Constants.NamNganSach.Type.NAM_TRUOC_CHUYEN_SANG).ToString()},
                new SelectListItem{Text=Constants.NamNganSach.TypeName.NAM_NAY,Value=((int)Constants.NamNganSach.Type.NAM_NAY).ToString()}
            };
            lstNamNganSach.Insert(0, new SelectListItem { Text = Constants.CHON, Value = 0.ToString() });
            ViewBag.ListNamNganSach = lstNamNganSach.ToSelectList();
            ViewBag.dNgayThongTri = model.dNgayThongTri.HasValue ? model.dNgayThongTri.Value.ToString("dd/MM/yyyy") : String.Empty;
            return View(model);
        }

        public ActionResult Sua(string id, int? isFromThongTri)
        {
            VDTThongTriModel model = _iQLVonDauTuService.LayChiTietThongTri(id);
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("sMoTa", "sTen");

            List<SelectListItem> lstLoaiThongTri = new List<SelectListItem>()
            {
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_THANH_TOAN,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_THANH_TOAN).ToString()},
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_TAM_UNG,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_TAM_UNG).ToString()},
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_KINH_PHI,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_KINH_PHI).ToString()},
                new SelectListItem{Text=Constants.LoaiThongTriThanhToan.TypeName.CAP_HOP_THUC,Value=((int)Constants.LoaiThongTriThanhToan.Type.CAP_HOP_THUC).ToString()}
            };
            lstLoaiThongTri.Insert(0, new SelectListItem { Text = Constants.CHON, Value = 0.ToString() });
            ViewBag.ListLoaiThongTri = lstLoaiThongTri.ToSelectList();


            List<SelectListItem> lstNamNganSach = new List<SelectListItem>()
            {
                new SelectListItem{Text=Constants.NamNganSach.TypeName.NAM_TRUOC_CHUYEN_SANG,Value=((int)Constants.NamNganSach.Type.NAM_TRUOC_CHUYEN_SANG).ToString()},
                new SelectListItem{Text=Constants.NamNganSach.TypeName.NAM_NAY,Value=((int)Constants.NamNganSach.Type.NAM_NAY).ToString()}
            };
            lstNamNganSach.Insert(0, new SelectListItem { Text = Constants.CHON, Value = 0.ToString() });
            ViewBag.ListNamNganSach = lstNamNganSach.ToSelectList();
            ViewBag.dNgayThongTri = model.dNgayThongTri.HasValue ? model.dNgayThongTri.Value.ToString("dd/MM/yyyy") : String.Empty;
            ViewBag.isFromThongTri = isFromThongTri;
            return View(model);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            bool xoa = _iQLVonDauTuService.XoaThongTri(Guid.Parse(id));
            return Json(xoa, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDeNghiThanhToanChiTiet(string iID_MaDonVi, int iNamThongTri, int iNguonVon, DateTime? dNgayLapGanNhat, DateTime? dNgayTaoThongTri)
        {
            var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, iID_MaDonVi).iID_MaDonVi;
            var lstDataDeNghiThanhToan = _iQLVonDauTuService.GetDeNghiThanhToanChiTiet(sMaDonViQuanLy, iNamThongTri, iNguonVon, dNgayLapGanNhat, dNgayTaoThongTri);
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
            return Json(model.iID_ThongTriID, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public ActionResult XuatFile()
        {
            ExcelFile excel = TaoFile();
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Danh sach thong tri thanh toan.xls");
            }
        }

        public ExcelFile TaoFile()
        {
            string sMaDonVi = string.Empty;
            string sMaThongTri = string.Empty;
            DateTime? dNgayThongTri = null;
            int? iNamThongTri = null;

            // get dieu kien tiem kiem
            if (TempData["sMaDonvi"] != null)
                sMaDonVi = (string)TempData["sMaDonvi"];
            if (TempData["sMaThongTri"] != null)
                sMaThongTri = (string)TempData["sMaThongTri"];
            if (TempData["dNgayThongTri"] != null)
                dNgayThongTri = (DateTime?)TempData["dNgayThongTri"];
            if (TempData["iNamThongTri"] != null)
                iNamThongTri = (int?)TempData["iNamThongTri"];

            IEnumerable<VDTThongTriModel> listData = _iQLVonDauTuService.LayDanhSachThongTriXuatFile(PhienLamViec.NamLamViec, Username, sMaDonVi, sMaThongTri, iNamThongTri, dNgayThongTri);

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VDTThongTriModel>("dt", listData);
            fr.Run(Result);
            return Result;
        }

        [HttpPost]
        public JsonResult Loc(Guid? id, string sMaDonVi, int iLoaiThongTri, int iNamKeHoach, DateTime dNgayThongTri,
            string sMaNguonVon, DateTime? dNgayLapGanNhat, string sMaLoaiCongTrinh = "")
        {
            List<VdtThongTriChiTietQuery> lstThanhToan = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstThuHoi = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstTamUng = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstKinhPhi = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstHopThuc = new List<VdtThongTriChiTietQuery>();

            var objDonVi = _iQLVonDauTuService.GetDonViQuanLyById(Guid.Parse(sMaDonVi));
            List<VdtThongTriChiTietQuery> data = _iQLVonDauTuService.GetVdtThongTriChiTiet(id ?? Guid.Empty, objDonVi.iID_MaDonVi, iLoaiThongTri, iNamKeHoach, dNgayThongTri,
                 sMaNguonVon, dNgayLapGanNhat).ToList();

            if (id != null && id != Guid.Empty)
            {
                data = _iQLVonDauTuService.GetVdtThongTriChiTietByParentId(id.Value).ToList();
            }
            if (data != null)
            {
                data = data.Select(n => { n.id = Guid.NewGuid(); return n; }).ToList();
                ConvertDataViewModel(iLoaiThongTri, data, ref lstThanhToan, ref lstThuHoi, ref lstTamUng, ref lstKinhPhi, ref lstHopThuc);
            }
            return Json(new
            {
                lstThanhToan = lstThanhToan,
                lstThuHoi = lstThuHoi,
                lstTamUng = lstTamUng,
                lstKinhPhi = lstKinhPhi,
                lstHopThuc = lstHopThuc
            }, JsonRequestBehavior.AllowGet);
        }

        private void ConvertDataViewModel(int iLoaiThongTri,
            List<VdtThongTriChiTietQuery> lstDataFind,
            ref List<VdtThongTriChiTietQuery> lstThanhToan,
            ref List<VdtThongTriChiTietQuery> lstThuHoi,
            ref List<VdtThongTriChiTietQuery> lstTamUng,
            ref List<VdtThongTriChiTietQuery> lstKinhPhi,
            ref List<VdtThongTriChiTietQuery> lstHopThuc
            )
        {
            if (lstDataFind != null)
            {
                if (iLoaiThongTri == (int)Constants.LoaiThongTriEnum.Type.CAP_THANH_TOAN)
                {
                    foreach (var item in lstDataFind)
                    {
                        if (item.SMaKieuThongTri == Constants.KieuThongTri.TT_KPQP
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_Cap_KPK
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_Cap_KPNN)

                            lstThanhToan.Add(item);
                        else if (item.SMaKieuThongTri == Constants.KieuThongTri.TT_ThuUng_KPQP
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_ThuUng_KPNN
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_ThuUng_KPK)
                            lstThuHoi.Add(item);
                        else
                            lstThanhToan.Add(item);
                    }
                }
                else
                {
                    switch (iLoaiThongTri)
                    {
                        case (int)Constants.LoaiThongTriEnum.Type.CAP_TAM_UNG:
                            lstTamUng = lstDataFind;
                            break;
                        case (int)Constants.LoaiThongTriEnum.Type.CAP_KINH_PHI:
                            lstKinhPhi = lstDataFind;
                            break;
                        case (int)Constants.LoaiThongTriEnum.Type.CAP_HOP_THUC:
                            lstHopThuc = lstDataFind;
                            break;
                    }
                }
            }
            else
            {
                if (iLoaiThongTri == (int)Constants.LoaiThongTriEnum.Type.CAP_THANH_TOAN)
                {
                    foreach (var item in lstDataFind)
                    {
                        if (item.SMaKieuThongTri == Constants.KieuThongTri.TT_KPQP
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_Cap_KPK
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_Cap_KPNN)

                            lstThanhToan.Add(item);
                        else if (item.SMaKieuThongTri == Constants.KieuThongTri.TT_ThuUng_KPQP
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_ThuUng_KPNN
                            || item.SMaKieuThongTri == Constants.KieuThongTri.TT_ThuUng_KPK)
                            lstThuHoi.Add(item);
                        else
                            lstThanhToan.Add(item);
                    }
                }
                else
                {
                    switch (iLoaiThongTri)
                    {
                        case (int)Constants.LoaiThongTriEnum.Type.CAP_TAM_UNG:
                            lstTamUng = lstDataFind;
                            break;
                        case (int)Constants.LoaiThongTriEnum.Type.CAP_KINH_PHI:
                            lstKinhPhi = lstDataFind;
                            break;
                        case (int)Constants.LoaiThongTriEnum.Type.CAP_HOP_THUC:
                            lstHopThuc = lstDataFind;
                            break;
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult SaveThongTriChiTiet(
             Guid thongTriId,
             List<VdtThongTriChiTietQuery> lstThanhToan,
             List<VdtThongTriChiTietQuery> lstThuHoi,
             List<VdtThongTriChiTietQuery> lstTamUng,
             List<VdtThongTriChiTietQuery> lstKinhPhi,
             List<VdtThongTriChiTietQuery> lstHopThuc
             )
        {
            var objThongTri = _iQLVonDauTuService.FindThongTriById(thongTriId);
            List<VDT_ThongTri_ChiTiet> lstData = new List<VDT_ThongTri_ChiTiet>();
            if (lstThanhToan != null)
            {
                lstData.AddRange(lstThanhToan.Select(n => new VDT_ThongTri_ChiTiet()
                {
                    iID_ThongTriID = thongTriId,
                    sSoThongTri = objThongTri.sMaThongTri,
                    iID_DuAnID = n.IIdDuAnId,
                    iID_NhaThauID = n.IIdNhaThauId,
                    fSoTien = n.FSoTien,
                    iID_MucID = n.IIdMucId,
                    iID_TieuMucID = n.IIdTieuMucId,
                    iID_TietMucID = n.IIdTietMucId,
                    iID_NganhID = n.IIdNganhId,
                    iID_LoaiCongTrinhID = n.IIdLoaiCongTrinhId,
                    iID_LoaiNguonVonID = n.IIdLoaiNguonVonId,
                    iID_CapPheDuyetID = n.IIdCapPheDuyetId
                }));
            }
            if (lstThuHoi != null)
            {
                lstData.AddRange(lstThuHoi.Select(n => new VDT_ThongTri_ChiTiet()
                {
                    iID_ThongTriID = thongTriId,
                    sSoThongTri = objThongTri.sMaThongTri,
                    iID_DuAnID = n.IIdDuAnId,
                    iID_NhaThauID = n.IIdNhaThauId,
                    fSoTien = n.FSoTien,
                    iID_MucID = n.IIdMucId,
                    iID_TieuMucID = n.IIdTieuMucId,
                    iID_TietMucID = n.IIdTietMucId,
                    iID_NganhID = n.IIdNganhId,
                    iID_LoaiCongTrinhID = n.IIdLoaiCongTrinhId,
                    iID_LoaiNguonVonID = n.IIdLoaiNguonVonId,
                    iID_CapPheDuyetID = n.IIdCapPheDuyetId
                }));
            }
            if (lstTamUng != null)
            {
                lstData.AddRange(lstTamUng.Select(n => new VDT_ThongTri_ChiTiet()
                {
                    iID_ThongTriID = thongTriId,
                    sSoThongTri = objThongTri.sMaThongTri,
                    iID_DuAnID = n.IIdDuAnId,
                    iID_NhaThauID = n.IIdNhaThauId,
                    fSoTien = n.FSoTien,
                    iID_MucID = n.IIdMucId,
                    iID_TieuMucID = n.IIdTieuMucId,
                    iID_TietMucID = n.IIdTietMucId,
                    iID_NganhID = n.IIdNganhId,
                    iID_LoaiCongTrinhID = n.IIdLoaiCongTrinhId,
                    iID_LoaiNguonVonID = n.IIdLoaiNguonVonId,
                    iID_CapPheDuyetID = n.IIdCapPheDuyetId
                }));
            }
            if (lstKinhPhi != null)
            {
                lstData.AddRange(lstKinhPhi.Select(n => new VDT_ThongTri_ChiTiet()
                {
                    iID_ThongTriID = thongTriId,
                    sSoThongTri = objThongTri.sMaThongTri,
                    iID_DuAnID = n.IIdDuAnId,
                    iID_NhaThauID = n.IIdNhaThauId,
                    fSoTien = n.FSoTien,
                    iID_MucID = n.IIdMucId,
                    iID_TieuMucID = n.IIdTieuMucId,
                    iID_TietMucID = n.IIdTietMucId,
                    iID_NganhID = n.IIdNganhId,
                    iID_LoaiCongTrinhID = n.IIdLoaiCongTrinhId,
                    iID_LoaiNguonVonID = n.IIdLoaiNguonVonId,
                    iID_CapPheDuyetID = n.IIdCapPheDuyetId
                }));
            }
            if (lstHopThuc != null)
            {
                lstData.AddRange(lstHopThuc.Select(n => new VDT_ThongTri_ChiTiet()
                {
                    iID_ThongTriID = thongTriId,
                    sSoThongTri = objThongTri.sMaThongTri,
                    iID_DuAnID = n.IIdDuAnId,
                    iID_NhaThauID = n.IIdNhaThauId,
                    fSoTien = n.FSoTien,
                    iID_MucID = n.IIdMucId,
                    iID_TieuMucID = n.IIdTieuMucId,
                    iID_TietMucID = n.IIdTietMucId,
                    iID_NganhID = n.IIdNganhId,
                    iID_LoaiCongTrinhID = n.IIdLoaiCongTrinhId,
                    iID_LoaiNguonVonID = n.IIdLoaiNguonVonId,
                    iID_CapPheDuyetID = n.IIdCapPheDuyetId
                }));
            }
            _iQLVonDauTuService.InsertListThongTriChiTiet(thongTriId, lstData);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongTriChiTiet(Guid id, int iLoaiThongTri)
        {
            List<VdtThongTriChiTietQuery> lstThanhToan = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstThuHoi = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstTamUng = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstKinhPhi = new List<VdtThongTriChiTietQuery>();
            List<VdtThongTriChiTietQuery> lstHopThuc = new List<VdtThongTriChiTietQuery>();
            var data = _iQLVonDauTuService.GetVdtThongTriChiTietById(id);
            if (data != null)
            {
                data = data.Select(n => { n.id = Guid.NewGuid(); return n; }).ToList();
                ConvertDataViewModel(iLoaiThongTri, data.ToList(), ref lstThanhToan, ref lstThuHoi, ref lstTamUng, ref lstKinhPhi, ref lstHopThuc);
            }
            return Json(new
            {
                lstThanhToan = lstThanhToan,
                lstThuHoi = lstThuHoi,
                lstTamUng = lstTamUng,
                lstKinhPhi = lstKinhPhi,
                lstHopThuc = lstHopThuc
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CapNhatThongTri(Guid id, string sMoTa, string sMaThongTri)
        {
            VDT_ThongTri obj = new VDT_ThongTri();
            obj.iID_ThongTriID = id;
            obj.sMaThongTri = sMaThongTri;
            obj.sMoTa = sMoTa;
            _iQLVonDauTuService.UpdateThongTriById(obj);
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        #region Helper
        [HttpGet]
        public ActionResult ExportReport(Guid id)
        {
            var objThongTri = _iQLVonDauTuService.FindThongTriById(id);
            var objDonVi = _iQLVonDauTuService.GetDonViQuanLyById(objThongTri.iID_DonViID.Value);
            var lstChiTiet = _iQLVonDauTuService.GetVdtThongTriChiTietById(id);
            var objDonViQuanLy = _iNganSachService.GetDonViByMaDonVi(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi);
            string sDonViQuanLy = string.Empty;
            string sTenDonVi = string.Empty;
            if (objDonViQuanLy != null)
                sDonViQuanLy = objDonViQuanLy.sTen.ToUpper();
            if (objDonVi != null)
                sTenDonVi = objDonVi.sTen;


            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();

            fr.SetValue("iNamKeHoach", objThongTri.iNamThongTri);
            fr.SetValue("Cap1", sDonViQuanLy);
            fr.SetValue("Cap2", string.Empty);
            fr.SetValue("Nam", DateTime.Now.Year.ToString());
            fr.SetValue("DonVi", sTenDonVi);
            fr.SetValue("Ve", string.Format("Tháng {0} năm {1}", DateTime.Now.Month, DateTime.Now.Year));
            fr.SetValue("Mota", "");
            fr.SetValue("NoiDung", "");
            fr.SetValue("Ngay", string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year));
            fr.SetValue("TongChiTieu", lstChiTiet.Sum(n => n.FSoTien));
            fr.SetValue("TienBangChu", DataHelper.NumberToText(lstChiTiet.Sum(n => n.FSoTien), true));
            fr.UseChuKy(Username)
                        .UseChuKyForController(sControlName)
                        .UseForm(this);
            fr.AddTable("Items", lstChiTiet);


            switch (objThongTri.iLoaiThongTri)
            {
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_THANH_TOAN:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_thanhtoan.xlsx"));
                        break;
                    }
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_TAM_UNG:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_tamung.xlsx"));
                        break;
                    }
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_KINH_PHI:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_kinhphi.xlsx"));
                        break;
                    }
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_HOP_THUC:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_hopthuc.xlsx"));
                        break;
                    }

            }
            fr.Run(Result);
            return Print(Result, "xlsx", "rpt_BC_ThongTriThanhToan.xlsx");
        }

        [HttpGet]
        public ActionResult XuatFilePage(Guid id)
        {
            List<SelectListItem> lstDonViTinh = new List<SelectListItem>()
            {
                new SelectListItem{Text=Constants.DonViTinh.TypeName.DONG,Value=((int)Constants.DonViTinh.Type.DONG).ToString()},
                new SelectListItem{Text=Constants.DonViTinh.TypeName.NGHIN_DONG,Value=((int)Constants.DonViTinh.Type.NGHIN_DONG_LOWER).ToString()},
                new SelectListItem{Text=Constants.DonViTinh.TypeName.TRIEU_DONG,Value=((int)Constants.DonViTinh.Type.TRIEU_DONG).ToString()},
                new SelectListItem{Text=Constants.DonViTinh.TypeName.TY_DONG,Value=((int)Constants.DonViTinh.Type.TY_DONG).ToString()}
            };
            ViewBag.lstDonViTinh = lstDonViTinh.ToSelectList();

            ViewBag.id = id;

            return View("_xuatFilePDF");
        }

        private int CheckDonViTinhIsNghinDong(int dvt = 1)
        {
            if (dvt == 1001) return 1000;
            else return dvt;
        }

        [HttpGet]
        public ActionResult ExportReportPDF(Guid id, string sTieuDeMot, string sTieuDeHai, int iDonViTinh)
        {
            var objThongTri = _iQLVonDauTuService.FindThongTriById(id);
            var objDonVi = _iQLVonDauTuService.GetDonViQuanLyById(objThongTri.iID_DonViID.Value);
            var lstChiTiet = _iQLVonDauTuService.GetVdtThongTriChiTietById(id);
            if (lstChiTiet != null)
            {
                lstChiTiet = lstChiTiet.Select(n => { n.FSoTien = n.FSoTien; return n; }).ToList();
            }
            var objDonViQuanLy = _iNganSachService.GetDonViByMaDonVi(PhienLamViec.iNamLamViec, PhienLamViec.iID_MaDonVi);
            string sDonViQuanLy = string.Empty;
            string sTenDonVi = string.Empty;
            if (objDonViQuanLy != null)
                sDonViQuanLy = objDonViQuanLy.sTen.ToUpper();
            if (objDonVi != null)
                sTenDonVi = objDonVi.sTen;


            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();

            FormatNumber formatNumber = new FormatNumber(CheckDonViTinhIsNghinDong(iDonViTinh), ExportType.PDF);
            fr.SetUserFunction("FormatNumber", formatNumber);
            fr.SetValue("DonViTinh", iDonViTinh.ToStringDvt()); // cho đơn vị "nghìn đồng"
            fr.SetValue("iNamKeHoach", objThongTri.iNamThongTri);
            fr.SetValue("Cap1", sDonViQuanLy);
            fr.SetValue("Cap2", string.Empty);
            fr.SetValue("Nam", DateTime.Now.Year.ToString());
            fr.SetValue("DonVi", sTenDonVi);
            fr.SetValue("Ve", string.Format("Tháng {0} năm {1}", DateTime.Now.Month, DateTime.Now.Year));
            fr.SetValue("Mota", "");
            fr.SetValue("NoiDung", "");
            if(sTieuDeMot != "")
            {
                fr.SetValue("TieuDe1", sTieuDeMot);
            }
            if (sTieuDeHai != "")
            {
                fr.SetValue("TieuDe2", sTieuDeHai);
            }            
            fr.SetValue("Ngay", string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year));
            fr.SetValue("TongChiTieu", lstChiTiet.Sum(n => n.FSoTien));
            fr.SetValue("TienBangChu", DataHelper.NumberToText(lstChiTiet.Sum(n => n.FSoTien), true));
            fr.AddTable("Items", lstChiTiet);
            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this);


            switch (objThongTri.iLoaiThongTri)
            {
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_THANH_TOAN:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_thanhtoan.xlsx"));
                        break;
                    }
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_TAM_UNG:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_tamung.xlsx"));
                        break;
                    }
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_KINH_PHI:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_kinhphi.xlsx"));
                        break;
                    }
                case (int)Constants.LoaiThongTriThanhToan.Type.CAP_HOP_THUC:
                    {
                        Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_hopthuc.xlsx"));
                        break;
                    }

            }
            fr.Run(Result);
            return Print(Result, "pdf", "rpt_BC_ThongTriThanhToan.pdf");
        }

        /// <summary>
        /// xuất danh sách trang thông tri - 4 loại
        /// </summary>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult XuatDanhSach(string sMaDonVi, string sMaThongTri, DateTime? dNgayThongTri, int? iNamThongTri)
        {
            PagingInfo temp = new PagingInfo();
            IEnumerable<VDTThongTriModel> items = _iQLVonDauTuService.LayDanhSachThongTri(ref temp, PhienLamViec.NamLamViec, Username, false, sMaDonVi, sMaThongTri, iNamThongTri, dNgayThongTri, true);
            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();

            fr.AddTable("Items", items);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_danhsach.xlsx"));
            fr.Run(Result);
            return Print(Result, "xlsx", "rpt_vdt_thongtri_danhsach.xlsx");
        }

        /// <summary>
        /// xuất danh sách trang thanh toán - 2 loại
        /// </summary>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult XuatDanhSachThanhToanView(string sMaDonVi, string sMaThongTri, DateTime? dNgayThongTri, int? iNamThongTri)
        {
            PagingInfo temp = new PagingInfo();
            IEnumerable<VDTThongTriModel> items = _iQLVonDauTuService.LayDanhSachThongTri(ref temp, PhienLamViec.NamLamViec, Username, true, sMaDonVi, sMaThongTri, iNamThongTri, dNgayThongTri, true);
            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();

            fr.AddTable("Items", items);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ThongTri/rpt_vdt_thongtri_danhsach.xlsx"));
            fr.Run(Result);
            return Print(Result, "xlsx", "rpt_vdt_thongtri_danhsach.xlsx");
        }
        #endregion
    }
}