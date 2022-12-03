using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Controllers;
using DapperExtensions;
using VIETTEL.Helpers;
using VIETTEL.Flexcel;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using FlexCel.Report;
using System.Globalization;
using System.Text;
using VIETTEL.Common;
using Viettel.Models.CPNH;

namespace VIETTEL.Areas.QLNH.Controllers.DanhMucNgoaiHoi
{
    public class DeNghiThanhToanController : FlexcelReportController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sFilePathGiayDeNghiThanhToanNgoaiTe = "/Report_ExcelFrom/QLNH/rpt_GiayDeNghi_ThanhToan_NgoaiTe.xlsx";
        private const string sFilePathGiayDeNghiThanhToanTrongNuoc = "/Report_ExcelFrom/QLNH/rpt_GiayDeNghi_ThanhToan_ChiTrongNuoc.xlsx";
        private const string sFilePathThongBaoChiNganSach = "/Report_ExcelFrom/QLNH/rpt_ThongBaoChiNganSach.xlsx";
        private const string sFilePathThongBaoCapKinhPhiNgoaiTe = "/Report_ExcelFrom/QLNH/rpt_ThongBao_ChiNgoaiTe.xlsx";
        private const string sFilePathThongBaoCapKinhPhiTrongNuoc = "/Report_ExcelFrom/QLNH/rpt_ThongBaoChiTrongNuoc.xlsx";
        private const string sControlName = "DeNghiThanhToan";
        public List<Dropdown_SelectValue> lstDonViTinh= new List<Dropdown_SelectValue>()
            {
                new Dropdown_SelectValue()
                {
                    Value = 1,
                    Label = "Nghìn USD"
                },
                 new Dropdown_SelectValue()
                {
                    Value = 2,
                    Label = "Nghìn VND"
                }, new Dropdown_SelectValue()
            };
       
        private List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
        {
            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "--Tất cả quý--", iQuy = 0},
            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý I", iQuy = 1},
            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý II", iQuy = 2},
            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý III", iQuy = 3},
            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý IV", iQuy = 4}
        };

        public ActionResult Index()
        {
            ThongTinThanhToanPagingViewModel tt = new ThongTinThanhToanPagingViewModel();
            tt._paging.CurrentPage = 1;
            tt.Items = _qlnhService.GetAllThongTinThanhToanPaging(ref tt._paging, null, null, null, null, null, null, null, null, null, null, null, null, null);
            if (tt.Items == null) tt.Items = new List<ThongTinThanhToanModel>();

            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            ViewBag.lstNSDonVi = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToSelectList("iID_Ma", "sMoTa");
            ViewBag.lstNhiemVuChi = _qlnhService.GetAllNhiemVuChiByDonVi().ToSelectList("ID", "sTenNhiemVuChi");
            ViewBag.lstChuDauTu = _qlnhService.GetAllDMChuDauTu().ToSelectList("ID", "sTenChuDauTu");
            ViewBag.lstDanhMucNhaThau = _qlnhService.GetAllDMNhaThau().ToSelectList("Id", "sTenNhaThau");
            return View(tt);
        }

        [HttpPost]
        public ActionResult DeNghiThanhToanSearch(ThongTinThanhToanSearchModel data)
        {
            ThongTinThanhToanPagingViewModel tt = new ThongTinThanhToanPagingViewModel();
            data.sSoDeNghi = HttpUtility.HtmlDecode(data.sSoDeNghi);
            tt._paging.CurrentPage = data._paging.CurrentPage;
            tt.Items = _qlnhService.GetAllThongTinThanhToanPaging(ref tt._paging, data.iID_DonVi, data.sSoDeNghi, data.dNgayDeNghi, data.iLoaiNoiDungChi, data.iLoaiDeNghi, data.iID_ChuDauTuID, data.iID_KHCTBQP_NhiemVuChiID, data.iQuyKeHoach, data.iNamKeHoach, data.iNamNganSach, data.iCoQuanThanhToan, data.iID_NhaThauID, data.iTrangThai);

            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            ViewBag.lstNSDonVi = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToSelectList("iID_Ma", "sMoTa");
            ViewBag.lstChuDauTu = _qlnhService.GetAllDMChuDauTu().ToSelectList("ID", "sTenChuDauTu");
            ViewBag.lstNhiemVuChi = _qlnhService.GetAllNhiemVuChiByDonVi(data.iID_DonVi).ToSelectList("ID", "sTenNhiemVuChi");
            ViewBag.lstDanhMucNhaThau = _qlnhService.GetAllDMNhaThau().ToSelectList("Id", "sTenNhaThau");
            return PartialView("_list", tt);
        }

        [HttpPost]
        public JsonResult GetAllNhiemVuChiByDonVi(Guid? iDDonVi)
        {
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetAllNhiemVuChiByDonVi(iDDonVi).ToList();
            StringBuilder htmlChuongtrinh = new StringBuilder();
            htmlChuongtrinh.AppendFormat("<option value='{0}' selected>{1}</option>", string.Empty, "--Chọn chương trình--");
            if (lstChuongTrinh != null && lstChuongTrinh.Count > 0)
            {
                for (int i = 0; i < lstChuongTrinh.Count; i++)
                {
                    htmlChuongtrinh.AppendFormat("<option value='{0}'>{1}</option>", lstChuongTrinh[i].ID, HttpUtility.HtmlEncode(lstChuongTrinh[i].sTenNhiemVuChi));
                }
            }
            return Json(new { htmlCT = htmlChuongtrinh.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert()
        {
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            ViewBag.lstNSDonVi = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            ViewBag.lstNhiemVuChi = _qlnhService.GetAllNhiemVuChiByDonVi().ToSelectList("ID", "sTenNhiemVuChi");
            ViewBag.lstChuDauTu = _qlnhService.GetAllDMChuDauTu().ToList();
            ViewBag.lstTyGiaDeNghi = _qlnhService.GetThongTinTyGia().ToSelectList("ID", "sTenTiGia");
            ViewBag.lstTyGiaPheDuyet = _qlnhService.GetThongTinTyGia().ToSelectList("ID", "sTenTiGia");
            ViewBag.lstDonViHuongThu = _qlnhService.GetAllDMNhaThau().ToSelectList("Id", "sTenNhaThau");
            return View();
        }

        public ActionResult Update(Guid? id)
        {
            ThongTinThanhToanDetaiModel model = new ThongTinThanhToanDetaiModel();
            var thanhtoan = _qlnhService.GetThongTinThanhToanByID(id);
            var thanhtoan_chitiet = _qlnhService.GetThongTinThanhToanChiTietById(id);
            model.thongtinthanhtoan = thanhtoan; 
            model.thanhtoan_chitiet = thanhtoan_chitiet;

            var lstduan = _qlnhService.GetDADuAn(thanhtoan == null ? null : thanhtoan.iID_KHCTBQP_NhiemVuChiID, thanhtoan == null ? null : thanhtoan.iID_ChuDauTuID);
            var lsthopdong = _qlnhService.GetThongTinHopDong(thanhtoan == null ? null : thanhtoan.iID_KHCTBQP_NhiemVuChiID);

            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            ViewBag.lstNSDonVi = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            ViewBag.lstNhiemVuChi = _qlnhService.GetAllNhiemVuChiByDonVi(thanhtoan == null ? null : thanhtoan.iID_DonVi).ToSelectList("ID", "sTenNhiemVuChi");
            ViewBag.lstChuDauTu = _qlnhService.GetAllDMChuDauTu().ToList();
            ViewBag.lstTyGiaDeNghi = _qlnhService.GetThongTinTyGia().ToSelectList("ID", "sTenTiGia");
            ViewBag.lstTyGiaPheDuyet = _qlnhService.GetThongTinTyGia().ToSelectList("ID", "sTenTiGia");
            ViewBag.lstDonViHuongThu = _qlnhService.GetAllDMNhaThau().ToSelectList("Id", "sTenNhaThau");
            ViewBag.lstHopDong = lsthopdong.ToSelectList("ID", "sTenHopDong"); ;
            ViewBag.lstDuAn = lstduan.ToSelectList("ID", "sTenDuAn");
            return View(model);
        }

        public ActionResult Detail(Guid id, bool isModal = false)
        {
            ThongTinThanhToanDetaiModel model = new ThongTinThanhToanDetaiModel();
            var thanhtoan = _qlnhService.GetThongTinThanhToanByID(id);
            var thanhtoan_chitiet = _qlnhService.GetThongTinThanhToanChiTietById(id);
            model.thongtinthanhtoan = thanhtoan;
            ViewBag.sTongDeNghi_USD = thanhtoan.fTongDeNghi_USD != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(thanhtoan.fTongDeNghi_USD.Value, 2).ToString(CultureInfo.InvariantCulture), 2) : String.Empty;
            ViewBag.sTongPheDuyet_USD = thanhtoan.fTongPheDuyet_USD != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(thanhtoan.fTongPheDuyet_USD.Value, 2).ToString(CultureInfo.InvariantCulture), 2) : String.Empty;
            ViewBag.sLuyKeUSD = thanhtoan.fLuyKeUSD != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(thanhtoan.fLuyKeUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2) : String.Empty;
            ViewBag.sLuyKeVND = thanhtoan.fLuyKeVND != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(thanhtoan.fLuyKeVND.Value, 2).ToString(CultureInfo.InvariantCulture), 0) : String.Empty;
            model.thanhtoan_chitiet = thanhtoan_chitiet;
            if (model.thanhtoan_chitiet != null)
            {
                foreach (var item in model.thanhtoan_chitiet)
                {
                    item.sDeNghiCapKyNay_USD = item.fDeNghiCapKyNay_USD != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(item.fDeNghiCapKyNay_USD.Value, 2).ToString(CultureInfo.InvariantCulture), 2) : String.Empty;
                    item.sPheDuyetCapKyNay_USD = item.fPheDuyetCapKyNay_USD != null ? DomainModel.CommonFunction.DinhDangSo(Math.Round(item.fPheDuyetCapKyNay_USD.Value, 2).ToString(CultureInfo.InvariantCulture), 2) : String.Empty;
                }
            }

            model.thongtinthanhtoan.sTiGiaDeNghi = _qlnhService.GetThongTinTyGia().Where(x => x.ID == thanhtoan.iID_TiGiaDeNghiID).FirstOrDefault().sTenTiGia;
            model.thongtinthanhtoan.sTiGiaPheDuyet = _qlnhService.GetThongTinTyGia().Where(x => x.ID == thanhtoan.iID_TiGiaPheDuyetID).FirstOrDefault().sTenTiGia;
            if (!isModal)
            {
                return View(model);
            }
            else
            {
                return PartialView("_modalDetail" , model);
            }
        }

        [HttpPost]
        public JsonResult GetThongTinDuAn(Guid? iID_NhiemVuChi, Guid? iID_ChuDauTu)
        {
            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetDADuAn(iID_NhiemVuChi, iID_ChuDauTu).ToList();
            StringBuilder htmlDA = new StringBuilder();
            htmlDA.AppendFormat("<option value='{0}' selected>{1}</option>", string.Empty, "--Chọn dự án--");
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlDA.AppendFormat("<option value='{0}'>{1}</option>", lstDuAn[i].ID, HttpUtility.HtmlEncode(lstDuAn[i].sTenDuAn));
                }
            }
            return Json(new { htmlDA = htmlDA.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongTinHopDong(Guid? iID_NhiemVuChi)
        {
            List<NH_DA_HopDong> lstThongTinHopDong = _qlnhService.GetThongTinHopDong(iID_NhiemVuChi).ToList();
            StringBuilder htmlHD = new StringBuilder();
            htmlHD.AppendFormat("<option value='{0}' selected>{1}</option>", string.Empty, "--Chọn hợp đồng--");
            if (lstThongTinHopDong != null && lstThongTinHopDong.Count > 0)
            {
                for (int i = 0; i < lstThongTinHopDong.Count; i++)
                {
                    htmlHD.AppendFormat("<option value='{0}'>{1}</option>", lstThongTinHopDong[i].ID, HttpUtility.HtmlEncode(lstThongTinHopDong[i].sTenHopDong));
                }
            }
            return Json(new { htmlHD = htmlHD.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetChuyenDoiTyGia(Guid? matygia, float sotiennhap, int loaitiennhap)
        {
            double tygiasauchuyendoi = _qlnhService.ChuyenDoiTyGia(matygia, sotiennhap, loaitiennhap);
            return Json(new { chuyendoi = tygiasauchuyendoi, status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult OpenMucLucNganSach()
        {
            MucLucNganSachPagingViewModel vm = new MucLucNganSachPagingViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetAllMucLucNganSach(ref vm._paging, Username, PhienLamViec.NamLamViec, null);
            return PartialView("_listmuclucngansachsearch", vm);
        }

        [HttpPost]
        public ActionResult MucLucNganSachSearch(ThongTinThanhToanSearchModel data)
        {
            MucLucNganSachPagingViewModel vm = new MucLucNganSachPagingViewModel();
            if (data._paging == null)
            {
                vm._paging.CurrentPage = 1;
            }
            else
            {
                vm._paging.CurrentPage = data._paging.CurrentPage;
            }
            data.sLNS = HttpUtility.HtmlDecode(data.sLNS);
            data.sL = HttpUtility.HtmlDecode(data.sL);
            data.sK = HttpUtility.HtmlDecode(data.sK);
            data.sM = HttpUtility.HtmlDecode(data.sM);
            data.sTM = HttpUtility.HtmlDecode(data.sTM);
            data.sTTM = HttpUtility.HtmlDecode(data.sTTM);
            data.sNG = HttpUtility.HtmlDecode(data.sNG);
            data.sTNG = HttpUtility.HtmlDecode(data.sTNG);
            data.sNoiDung = HttpUtility.HtmlDecode(data.sNoiDung);

            vm.Items = _qlnhService.GetAllMucLucNganSach(ref vm._paging, Username, PhienLamViec.NamLamViec, data);
            return PartialView("_listmuclucngansachsearch", vm);
        }

        public JsonResult CheckTrungMaDeNghi(string sodenghi, int type_action, Guid? idenghi)
        {
            bool results = _qlnhService.CheckTrungMaDeNghi(sodenghi, type_action, idenghi);
            return Json(new { results = results, status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool SaveDeNghiThanhToan(NH_TT_ThanhToanModel data, string listIDChiTietXoa)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entityThongTinThanhToan = new NH_TT_ThanhToan();

                    if (data.lstTTThanToan_ChiTiet != null)
                    {
                        //Convert so tien
                        foreach (var item in data.lstTTThanToan_ChiTiet)
                        {
                            item.fPheDuyetCapKyNay_VND = CommonFunction.TryParseDouble(item.sPheDuyetCapKyNay_VND);
                            item.fPheDuyetCapKyNay_USD = CommonFunction.TryParseDouble(item.sPheDuyetCapKyNay_USD);
                            item.fDeNghiCapKyNay_USD = CommonFunction.TryParseDouble(item.sDeNghiCapKyNay_USD);
                            item.fDeNghiCapKyNay_VND = CommonFunction.TryParseDouble(item.sDeNghiCapKyNay_VND);
                        }
                    }

                    if (data.TT_ThanToan.iThanhToanTheo == 1 && data.TT_ThanToan.iID_HopDongID != null)
                    {
                        var lstThongTinHopDong = _qlnhService.GetThongTinHopDong(data.TT_ThanToan.iID_KHCTBQP_NhiemVuChiID.Value);
                        NH_DA_HopDong hopdong = lstThongTinHopDong.Where(x => x.ID == data.TT_ThanToan.iID_HopDongID).FirstOrDefault();
                        data.TT_ThanToan.iID_DuAnID = hopdong != null ? hopdong.iID_DuAnID : null;
                    }

                    data.TT_ThanToan.sSoDeNghi = HttpUtility.HtmlDecode(data.TT_ThanToan.sSoDeNghi);
                    data.TT_ThanToan.sCanCu = HttpUtility.HtmlDecode(data.TT_ThanToan.sCanCu);
                    data.TT_ThanToan.sSoTaiKhoan = HttpUtility.HtmlDecode(data.TT_ThanToan.sSoTaiKhoan);
                    data.TT_ThanToan.sNganHang = HttpUtility.HtmlDecode(data.TT_ThanToan.sNganHang);
                    data.TT_ThanToan.sChuyenKhoan_BangChu = HttpUtility.HtmlDecode(data.TT_ThanToan.sChuyenKhoan_BangChu);
                    data.TT_ThanToan.iID_MaDonVi = HttpUtility.HtmlDecode(data.TT_ThanToan.iID_MaDonVi);
                    data.TT_ThanToan.iID_MaChuDauTu = HttpUtility.HtmlDecode(data.TT_ThanToan.iID_MaChuDauTu);

                    entityThongTinThanhToan.MapFrom(data.TT_ThanToan);


                    if (data.lstTTThanToan_ChiTiet != null)
                    {
                        entityThongTinThanhToan.fTongDeNghi_USD = data.lstTTThanToan_ChiTiet.Sum(x => x.fDeNghiCapKyNay_USD);
                        entityThongTinThanhToan.fTongDeNghi_VND = data.lstTTThanToan_ChiTiet.Sum(x => x.fDeNghiCapKyNay_VND);
                        entityThongTinThanhToan.fTongPheDuyet_USD = data.lstTTThanToan_ChiTiet.Sum(x => x.fPheDuyetCapKyNay_USD);
                        entityThongTinThanhToan.fTongPheDuyet_VND = data.lstTTThanToan_ChiTiet.Sum(x => x.fPheDuyetCapKyNay_VND);
                    }
                    entityThongTinThanhToan.fLuyKeUSD = CommonFunction.TryParseDouble(data.TT_ThanToan.sLuyKeUSD);
                    entityThongTinThanhToan.fLuyKeVND = CommonFunction.TryParseDouble(data.TT_ThanToan.sLuyKeVND);
                    double? fChuyenKhoan = CommonFunction.TryParseDouble(data.TT_ThanToan.sChuyenKhoanBangSo);
                    if (data.TT_ThanToan.iLoaiNoiDungChi == 1)
                    {
                        entityThongTinThanhToan.fChuyenKhoan_BangSo_USD = fChuyenKhoan;
                        entityThongTinThanhToan.fChuyenKhoan_BangSo_VND = _qlnhService.ChuyenDoiTyGia(data.TT_ThanToan.iID_TiGiaPheDuyetID, (float)fChuyenKhoan, 1);
                    }
                    else
                    {
                        entityThongTinThanhToan.fChuyenKhoan_BangSo_VND = fChuyenKhoan;
                        entityThongTinThanhToan.fChuyenKhoan_BangSo_USD = _qlnhService.ChuyenDoiTyGia(data.TT_ThanToan.iID_TiGiaPheDuyetID, (float)fChuyenKhoan, 2);
                    }


                    //Thực hiện thêm mới đề nghị thanh toán
                    if (data.TT_ThanToan.ID == Guid.Empty || data.TT_ThanToan.ID == null)
                    {

                        
                        entityThongTinThanhToan.dNgayTao = DateTime.Now;
                        entityThongTinThanhToan.sNguoiTao = Username;
                        conn.Insert(entityThongTinThanhToan, trans);
                      
                        //Thực hiện thêm mới thông tin thanh toán chi tiết
                        if (data.lstTTThanToan_ChiTiet != null && data.lstTTThanToan_ChiTiet.Count() > 0)
                        {
                            for (int i = 0; i < data.lstTTThanToan_ChiTiet.Count(); i++)
                            {
                                var entityChiTiet = new NH_TT_ThanhToan_ChiTiet();
                                data.lstTTThanToan_ChiTiet.ToList()[i].sTenNoiDungChi = HttpUtility.HtmlDecode(data.lstTTThanToan_ChiTiet.ToList()[i].sTenNoiDungChi);
                                entityChiTiet.MapFrom(data.lstTTThanToan_ChiTiet.ToList()[i]);
                                entityChiTiet.iID_ThanhToanID = entityThongTinThanhToan.ID;
                                conn.Insert(entityChiTiet, trans);
                            }
                        }

                        // Thực hiện insert vào bảng tổng hợp
                        if (entityThongTinThanhToan.iLoaiDeNghi == 1 && entityThongTinThanhToan.iTrangThai == 3)
                        {
                            _qlnhService.InsertNHTongHop_Tang(conn, trans, "TTCP", 1, entityThongTinThanhToan.ID, null);
                        }
                        if (entityThongTinThanhToan.iLoaiDeNghi != 1 && entityThongTinThanhToan.iTrangThai == 3)
                        {
                            _qlnhService.InsertNHTongHop_Giam(conn, trans, "TTCP", 1, entityThongTinThanhToan.ID, null);
                        }

                    }
                    else
                    {
                        //Thực hiện update đề nghị thanh toán
                        ThongTinThanhToanModel thanhtoan = _qlnhService.GetThongTinThanhToanByID(data.TT_ThanToan.ID);
                        entityThongTinThanhToan.sNguoiSua = Username;
                        entityThongTinThanhToan.dNgaySua = DateTime.Now;
                       
                        entityThongTinThanhToan.dNgayTao = thanhtoan.dNgayTao;
                        conn.Update(entityThongTinThanhToan, trans);

                        //Thực hiện update các thông tin thanh toán chi tiết
                        if (data.lstTTThanToan_ChiTiet != null && data.lstTTThanToan_ChiTiet.Count() > 0)
                        {
                            for (int i = 0; i < data.lstTTThanToan_ChiTiet.Count(); i++)
                            {
                                var entityChiTiet = new NH_TT_ThanhToan_ChiTiet();
                                data.lstTTThanToan_ChiTiet.ToList()[i].sTenNoiDungChi = HttpUtility.HtmlDecode(data.lstTTThanToan_ChiTiet.ToList()[i].sTenNoiDungChi);
                                entityChiTiet.MapFrom(data.lstTTThanToan_ChiTiet.ToList()[i]);
                                if (data.lstTTThanToan_ChiTiet.ToList()[i].ID == Guid.Empty)
                                {
                                    entityChiTiet.iID_ThanhToanID = entityThongTinThanhToan.ID;
                                    conn.Insert(entityChiTiet, trans);
                                }
                                else
                                {
                                    entityChiTiet.iID_ThanhToanID = entityThongTinThanhToan.ID;
                                    conn.Update(entityChiTiet, trans);
                                }
                            }
                        }
                        //Xóa các data đã bị xóa
                        string[] lsID = listIDChiTietXoa.Split(',');
                        for (int i = 0; i < lsID.Length; i++)
                        {
                            if (lsID[i] != "")
                            {
                                var entityXoa = _qlnhService.GetThongTinThanhToanChiTiet(Guid.Parse(lsID[i]));
                                conn.Delete(entityXoa, trans);
                            }
                        }

                        // Thực hiện update vào bảng tổng hợp
                        if (entityThongTinThanhToan.iLoaiDeNghi == 1 && entityThongTinThanhToan.iTrangThai == 3)
                        {
                            _qlnhService.InsertNHTongHop_Tang(conn, trans, "TTCP", 2, entityThongTinThanhToan.ID, null);
                        }
                        if (entityThongTinThanhToan.iLoaiDeNghi != 1 && entityThongTinThanhToan.iTrangThai == 3)
                        {
                            _qlnhService.InsertNHTongHop_Giam(conn, trans, "TTCP", 2, entityThongTinThanhToan.ID, null);
                        }
                    }
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        [HttpPost]
        public JsonResult GetHopDongByID(Guid id)
        {
            var result = _qlnhService.GetThongTinHopDongById(id);
            return Json(new { result = result, status = true }, JsonRequestBehavior.AllowGet) ;
        }

        [HttpPost]
        public JsonResult GetDuAnByID(Guid id)
        {
            var result = _qlnhService.GetDuAnById(id);
            return Json(new { result = result, status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LayThongTinLuyKe(Guid? id, Guid? idonvi, Guid? inhiemvuchi, Guid? ichudautu)
        {
            ThongTinThanhToanModel model = _qlnhService.GetThongTinThanhToanByID(id);
            DateTime? dngaytao = model == null ? DateTime.Now : model.dNgayTao;
            var thanhtoan = _qlnhService.GetThanhToanGanNhat(dngaytao.Value, idonvi, inhiemvuchi, ichudautu);         
            return Json(new { thanhtoan = thanhtoan,  status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool DeleteDeNghiThanhToan(Guid id)
        {
            try
            {
                return _qlnhService.DeleteDeNghiThanhToan(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
        public JsonResult ConvertNumberToText(string number, bool suffix)
        {
            string sConvert = CommonFunction.NumberToText(Convert.ToInt64(number), suffix);
            return Json(new { result = sConvert, status = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult onThongBaoXoa(Guid id)
        {
            ThongTinThanhToanModel model = _qlnhService.GetThongTinThanhToanByID(id);
            return PartialView("_modalDelete", model);
        }

        [HttpPost]
        public ActionResult OnModalBaoCao(int type)
        {   
           
            if (type == 1)
            {
                ViewBag.Title = "GIẤY ĐỀ NGHỊ THANH TOÁN KINH PHÍ TỪ NGUỒN DỰ TRỮ NGOẠI HỐI (CHI NGOẠI TỆ)";
            }
            if (type == 2)
            {
                ViewBag.Title = "ĐỀ NGHỊ THANH TOÁN KINH PHÍ TỪ NGUỒN QUỸ DỰ TRỮ NGOẠI HỐI (CHI TRONG NƯỚC)";
            }
            if (type == 3)
            {
                ViewBag.Title = "THÔNG BÁO CHI NGÂN SÁCH TỪ NGUỒN QUỸ DỰ TRỮ NGOẠI HỐI";
            }
            if (type == 4)
            {
                ViewBag.Title = "THÔNG BÁO CẤP KINH PHÍ BẰNG NGOẠI TỆ TỪ NGUỒN QUỸ DỰ TRỮ NGOẠI HỐI";
            }
            if (type == 5)
            {
                ViewBag.Title = "THÔNG BÁO CẤP KINH PHÍ CHI TRONG NƯỚC TỪ NGUỒN QUỸ DỰ TRỮ NGOẠI HỐI";
            }

            ViewBag.lstNSPhongBan = _qlnhService.GetAllNSPhongBan().ToSelectList("iID_MaPhongBan", "sMoTa");
            ViewBag.Type = type;
            return PartialView("_viewbaocao",type);
        }

        [ValidateInput(false)]
        public ActionResult ExportGiayDeNghiThanhToan(Guid? idThanhtoan, Guid? idPhongBan, string sNoiDung, string idDonVi, int dvt = 1, int type = 1, string tieuDe1= "", string tieuDe2="", string ext = "pdf")
        {
            sNoiDung = HttpUtility.UrlDecode(sNoiDung);
            idDonVi = HttpUtility.UrlDecode(idDonVi);
            ExcelFile xls = FileGiayDeNghiThanhToan(idThanhtoan, idPhongBan, sNoiDung, idDonVi, tieuDe1, tieuDe2, dvt, type);
            string sFileName = "Giấy đề nghị thanh toán";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName);
        }

        public ExcelFile FileGiayDeNghiThanhToan(Guid? idThanhtoan, Guid? idPhongBan, string sNoiDung = "",string idDonVi = "", string tieuDe1 = "",string tieuDe2="", int dvt = 1, int type = 1)
        {
            XlsFile Result = new XlsFile(true);
            string sTiTle = "";
            if (type == 1)
            {
                sTiTle = "CHI NGOẠI TỆ";
                Result.Open(Server.MapPath(sFilePathGiayDeNghiThanhToanNgoaiTe));
            }
            else
            {
                sTiTle = "CHI TRONG NƯỚC";
                Result.Open(Server.MapPath(sFilePathGiayDeNghiThanhToanTrongNuoc));
            }
            FlexCelReport fr = new FlexCelReport();
            ThongTinThanhToanModel thanhtoan = _qlnhService.GetThongTinThanhToanByID(idThanhtoan);
            NS_PhongBan phongban = _qlnhService.GetAllNSPhongBan().Where(x => x.iID_MaPhongBan == idPhongBan).FirstOrDefault();
            NH_DA_HopDong hopdong = _qlnhService.GetThongTinHopDong(thanhtoan.iID_KHCTBQP_NhiemVuChiID.Value).Where(x => x.ID == thanhtoan.iID_HopDongID).FirstOrDefault();
            List<ThanhToanChiTietViewModel> lst = _qlnhService.GetThongTinThanhToanChiTietById(idThanhtoan).Select(x => new ThanhToanChiTietViewModel
            {
            STT = x.STT,
                ID = x.ID,
                iID_ThanhToanID = x.iID_ThanhToanID,
                sTenNoiDungChi = x.sTenNoiDungChi,
                fDeNghiCapKyNay_USD = x.fDeNghiCapKyNay_USD,
                fDeNghiCapKyNay_VND = x.fDeNghiCapKyNay_VND,
                fPheDuyetCapKyNay_USD = x.fPheDuyetCapKyNay_USD,
                fPheDuyetCapKyNay_VND = x.fPheDuyetCapKyNay_VND,
                sMucLucNganSach = x.sMucLucNganSach
            }).ToList();
            string sLoaiKinhPhi = "";
            if (thanhtoan.iLoaiDeNghi == 1)
            {
                sLoaiKinhPhi = "cấp kinh phí";
            }
            if (thanhtoan.iLoaiDeNghi == 2)
            {
                sLoaiKinhPhi = "thanh toán";
            }
            if (thanhtoan.iLoaiDeNghi == 3)
            {
                sLoaiKinhPhi = "tạm ứng";
            }
            double? sDuToanDuocDuyetUSD = Double.Parse(thanhtoan.sHopDongPheDuyet_USD);
            double? sDuToanDuocDuyetVND = Double.Parse(thanhtoan.sHopDongPheDuyet_VND);
            double? fLuyKeVND = (lst.Count == 0) ? null : thanhtoan.fLuyKeVND;
            double? fLuyKeUSD = (lst.Count == 0) ? null : thanhtoan.fLuyKeUSD;
            double? fBangSoUSD = (lst.Count == 0) ? null : thanhtoan.fChuyenKhoan_BangSo_USD;
            double? fBangSoVND = (lst.Count == 0) ? null : thanhtoan.fChuyenKhoan_BangSo_VND;
            double? fTongCTDeNghiCapKyNay_USD = (lst.Count == 0) ? null : thanhtoan.fTongCTDeNghiCapKyNay_USD;
            double? fTongCTDeNghiCapKyNay_VND = (lst.Count == 0) ? null : thanhtoan.fTongCTDeNghiCapKyNay_VND;
            double? fTongCtPheDuyetCapKyNay_USD = (lst.Count == 0) ? null : thanhtoan.fTongCtPheDuyetCapKyNay_USD;
            double? fTongCtPheDuyetCapKyNay_VND = (lst.Count == 0) ? null : thanhtoan.fTongCTPheDuyetCapKyNay_VND;

            if (dvt == 1)
            {
                fLuyKeUSD = fLuyKeUSD / 1000;
                fTongCTDeNghiCapKyNay_USD = fTongCTDeNghiCapKyNay_USD / 1000;
                fTongCtPheDuyetCapKyNay_USD = fTongCtPheDuyetCapKyNay_USD / 1000;
                sDuToanDuocDuyetUSD = sDuToanDuocDuyetUSD / 1000;
                foreach (var item in lst)
                {
                    double? fDeNghiCapKyNay_USD = item.fDeNghiCapKyNay_USD;
                    fDeNghiCapKyNay_USD = fDeNghiCapKyNay_USD / 1000;
                    item.fDeNghiCapKyNay_USD = fDeNghiCapKyNay_USD;
                }
            }
            else
            {
                fLuyKeVND = fLuyKeVND / 1000;
                fTongCTDeNghiCapKyNay_VND = fTongCTDeNghiCapKyNay_VND / 1000;
                fTongCtPheDuyetCapKyNay_VND = fTongCtPheDuyetCapKyNay_VND / 1000;
                sDuToanDuocDuyetVND = sDuToanDuocDuyetVND / 1000;
                foreach (var item in lst)
                {
                    double? fDeNghiCapKyNay_VND = item.fDeNghiCapKyNay_VND;
                    fDeNghiCapKyNay_VND = fDeNghiCapKyNay_VND / 1000;
                    item.fDeNghiCapKyNay_VND = fDeNghiCapKyNay_VND;
                }
            }
            
            fr.AddTable<ThanhToanChiTietViewModel>("dt", lst);
            fr.SetValue("sDuToanDuocDuyetVND", sDuToanDuocDuyetVND);
            fr.SetValue("sDuToanDuocDuyetUSD", sDuToanDuocDuyetUSD);
            fr.SetValue("sPhongBan", phongban.sMoTa.ToUpper());
            fr.SetValue("sTenChuongTrinh", thanhtoan.sTenNhiemVuChi);
            fr.SetValue("sTenCDT", thanhtoan.sTenCDT);
            fr.SetValue("sCanCu", thanhtoan.sCanCu);
            fr.SetValue("sSoDeNghi", thanhtoan.sSoDeNghi);
            fr.SetValue("sTieuDe1", tieuDe1);
            fr.SetValue("sTieuDe2", tieuDe2);
            fr.SetValue("sDonViThuHuong", thanhtoan.sTenNhaThau);
            fr.SetValue("fBangSoVND", fBangSoVND);
            fr.SetValue("fBangSoUSD", fBangSoUSD);
            fr.SetValue("sBangChu", thanhtoan.sChuyenKhoan_BangChu);
            fr.SetValue("sNganHang", thanhtoan.sNganHang);
            fr.SetValue("sSoTaiKhoan", thanhtoan.sSoTaiKhoan);
            fr.SetValue("fTongCTDeNghiCapKyNay_USD",fTongCTDeNghiCapKyNay_USD);
            fr.SetValue("fTongCTDeNghiCapKyNay_VND", fTongCtPheDuyetCapKyNay_VND);
            fr.SetValue("fTongCtPheDuyetCapKyNay_USD", fTongCtPheDuyetCapKyNay_USD);
            fr.SetValue("fTongCTPheDuyetCapKyNay_VND", fTongCtPheDuyetCapKyNay_VND);
            fr.SetValue("sTiTle", sTiTle);
            fr.SetValue("sNoiDung", sNoiDung);
            fr.SetValue("idDonVi", idDonVi);
            fr.SetValue("fLuyKeVND", fLuyKeVND);
            fr.SetValue("fLuyKeUSD", fLuyKeUSD);
            fr.SetValue("dvt", dvt);
            fr.SetValue("sLoaiKinhPhi", sLoaiKinhPhi);
            fr.SetValue(new
            {
                To = 1
            });

            fr.UseChuKy(Username)
               .UseChuKyForController(sControlName)
               .UseForm(this).Run(Result);

            return Result;
        }

        [ValidateInput(false)]
        public ActionResult ExportThongBaoChiNganSach(Guid? idPhongBan, int nam, int thang = 1, int quy = 1, string ext = "pdf", string lstIdThanhToan = "", string sNoiDung = "", string idDonVi = "", string idDonViCapDuoi = "", string sCanCu = "", string tieuDe1 = "", string tieuDe2 = "", int dvt = 1)
        {
            sNoiDung = HttpUtility.UrlDecode(sNoiDung);
            idDonVi = HttpUtility.UrlDecode(idDonVi);
            idDonViCapDuoi = HttpUtility.UrlDecode(idDonViCapDuoi);
            sCanCu = HttpUtility.UrlDecode(sCanCu);
            ExcelFile xls = FileThongBaoChiNganSach(idPhongBan, nam, thang, quy, lstIdThanhToan, sNoiDung, idDonVi, idDonViCapDuoi, sCanCu, tieuDe1, tieuDe2, dvt);
            string sFileName = "Giấy đề nghị thanh toán";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName);
        }

        public ExcelFile FileThongBaoChiNganSach(Guid? idPhongBan, int? nam, int? thang, int? quy, string lstIdThanhToan, string sNoiDung,string idDonVi,string idDonViCapDuoi, string sCanCu, string tieuDe1, string tieuDe2, int dvt)
        {
            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();
            Result.Open(Server.MapPath(sFilePathThongBaoChiNganSach));
            fr.SetValue(new
            {
                To = 1
            });

            List<ThanhToanBaoCaoModel> lst = _qlnhService.ExportBaoCaoChiThanhToan(lstIdThanhToan, thang.Value, quy.Value, nam.Value).Select(x => new ThanhToanBaoCaoModel
            {
                NoiDung = x.NoiDung,
                TongSo_VND = x.TongSo_VND,
                ChiNgoaiTen_USD = x.ChiNgoaiTen_USD,
                ChiNgoaiTe_VND = x.ChiNgoaiTe_VND,
                ChiTrongNuocVND = x.ChiTrongNuocVND,
                IsBold = x.IsBold,
                depth = x.depth,
                IDParent = x.IDParent,
                Position = x.Position,
                Muc = x.Muc
            }).ToList();
            decimal? fTongSo_VND = lst.Where(x => x.IDParent == null).Sum(x => x.TongSo_VND);
            decimal? fChiNgoaiTen_USD = lst.Where(x => x.IDParent == null).Sum(x => x.ChiNgoaiTen_USD);
            decimal? fChiNgoaiTe_VND = lst.Where(x => x.IDParent == null).Sum(x => x.ChiNgoaiTe_VND);
            decimal? fChiTrongNuocVND = lst.Where(x => x.IDParent == null).Sum(x => x.ChiTrongNuocVND);
            NS_PhongBan phongban = _qlnhService.GetAllNSPhongBan().Where(x => x.iID_MaPhongBan == idPhongBan).FirstOrDefault();
            fr.AddTable<ThanhToanBaoCaoModel>("dt", lst);
            fr.SetValue("fTongSo_VND", fTongSo_VND);
            fr.SetValue("fChiNgoaiTen_USD", fChiNgoaiTen_USD);
            fr.SetValue("fChiNgoaiTe_VND", fChiNgoaiTe_VND);
            fr.SetValue("fChiTrongNuocVND", fChiTrongNuocVND);
            fr.SetValue("thang", thang);
            fr.SetValue("quy", quy);
            fr.SetValue("nam", nam);
            fr.SetValue("sPhongBan", phongban.sMoTa);
            fr.SetValue("sCanCu", sCanCu);
            fr.SetValue("sNoiDung", sNoiDung);
            fr.SetValue("idDonVi", idDonVi);
            fr.SetValue("sTieuDe1", tieuDe1);
            fr.SetValue("sTieuDe2", tieuDe2);
            fr.SetValue("idDonViCapDuoi", idDonViCapDuoi);
            fr.SetValue("dvt", dvt);

            fr.UseChuKy(Username)
              .UseChuKyForController(sControlName)
              .UseForm(this).Run(Result);
            return Result;
        }

        [ValidateInput(false)]
        public ActionResult ExportThongBaoCapKinhPhi(string lstIdThanhToan, string tungay, string denngay, string sNoiDung,string idDonVi,string idDonViCapDuoi, string tieuDe1, string tieuDe2, Guid? idquanly, int dvt = 1, string ext = "pdf", int type = 4)
        {
            sNoiDung = HttpUtility.UrlDecode(sNoiDung);
            idDonVi = HttpUtility.UrlDecode(idDonVi);
            idDonViCapDuoi = HttpUtility.UrlDecode(idDonViCapDuoi);
            ExcelFile xls = FileThongBaoCapKinhPhi(lstIdThanhToan, tungay, denngay, sNoiDung,idDonVi, idDonViCapDuoi,tieuDe1,tieuDe2, idquanly, dvt, type);
            string sFileName = "Thông báo cấp kinh phí bằng ngoại tệ";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName);
        }

        public ExcelFile FileThongBaoCapKinhPhi(string lstIdThanhToan, string tungay, string denngay, string sNoiDung, string idDonVi, string idDonViCapDuoi,string tieuDe1,string tieuDe2, Guid? idquanly, int dvt = 1, int type = 4)
        {
            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();
            if (type == 4)
            {
                Result.Open(Server.MapPath(sFilePathThongBaoCapKinhPhiNgoaiTe));
            }
            else
            {
                Result.Open(Server.MapPath(sFilePathThongBaoCapKinhPhiTrongNuoc));
            }

            DateTime dtungay = DateTime.Parse(tungay);
            DateTime ddenngay = DateTime.Parse(denngay);
            List<ThanhToanBaoCaoModel> lst = new List<ThanhToanBaoCaoModel>();
            decimal? fTongUSD = null;
            decimal? fTongVND = null;
            if (type == 4)
            {
                lst = _qlnhService.ExportBaoCaoThongBaoCapKinhPhiChiNgoaiTe(lstIdThanhToan, dtungay, ddenngay).Select(x => new ThanhToanBaoCaoModel
                {
                    STT = x.STT,
                    sTenNhaThau = x.sTenNhaThau,
                    sTenHopDong = x.sTenHopDong,
                    sNoiDungChi = x.sNoiDungChi,
                    fPheDuyetCapKyNay_USD = x.fPheDuyetCapKyNay_USD,
                    fTyGia = x.fTyGia,
                    fPheDuyetCapKyNay_VND = x.fPheDuyetCapKyNay_VND
                }).ToList();

                fTongUSD = lst.Sum(x => x.fPheDuyetCapKyNay_USD);
                fTongVND = lst.Sum(x => x.fPheDuyetCapKyNay_VND);
            }
            else
            {
                lst = _qlnhService.ExportBaoCaoThongBaoCapKinhPhiChiTrongNuoc(lstIdThanhToan, dtungay, ddenngay).Select(x => new ThanhToanBaoCaoModel
                {
                    STT = x.STT,
                    sTenNhaThau = x.sTenNhaThau,
                    sTenHopDong = x.sTenHopDong,
                    sNoiDungChi = x.sNoiDungChi,
                    fPheDuyetCapKyNay_VND = x.fPheDuyetCapKyNay_VND
                }).ToList();
                fTongVND = lst.Sum(x => x.fPheDuyetCapKyNay_VND);
            }

            NS_PhongBan phongban = _qlnhService.GetAllNSPhongBan().Where(x => x.iID_MaPhongBan == idquanly).FirstOrDefault();
            fr.AddTable<ThanhToanBaoCaoModel>("dt", lst);
            fr.SetValue("tungay", tungay);
            fr.SetValue("sPhongBan", phongban.sMoTa);
            fr.SetValue("denngay", denngay);
            fr.SetValue("idDonViCapDuoi", idDonViCapDuoi);
            fr.SetValue("sNoiDung", sNoiDung);
            fr.SetValue("idDonVi", idDonVi);
            fr.SetValue("dvt", dvt);
            fr.SetValue("fTongUSD", fTongUSD);
            fr.SetValue("fTongVND", fTongVND);
            fr.SetValue("sTieuDe1", tieuDe1);
            fr.SetValue("sTieuDe2", tieuDe2);
            fr.SetValue(new
            {
                To = 1
            });

            fr.UseChuKy(Username)
              .UseChuKyForController(sControlName)
              .UseForm(this).Run(Result);
            return Result;
        }
    }

}