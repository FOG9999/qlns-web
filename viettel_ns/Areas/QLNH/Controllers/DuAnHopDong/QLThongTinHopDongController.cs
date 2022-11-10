using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Common;
using System.Web;
using ClosedXML.Excel;
using System.IO;
using VIETTEL.Helpers;
using System.Reflection;
using VIETTEL.Areas.z.Models;
using System.Data;
using System.Net;

namespace VIETTEL.Areas.QLNH.Controllers.DuAnHopDong
{
    public class QLThongTinHopDongController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly List<string> MaTienTeList = new List<string>() { "USD", "VND", "EUR" };

        // GET: QLNH/QLThongTinHopDong
        public ActionResult Index()
        {
            NHDAThongTinHopDongViewModel vm = new NHDAThongTinHopDongViewModel();
            vm._paging.CurrentPage = 1;
            vm._paging.ItemsPerPage = 10;
            vm.Items = _qlnhService.GetAllNHThongTinHopDong(ref vm._paging, null, null, null, null, null, null, null);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
            ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDADuAnList().ToList();
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

            List<NH_DM_LoaiHopDong> lstLoaiHD = _qlnhService.GetNHDMLoaiHopDongList().ToList();
            lstLoaiHD.Insert(0, new NH_DM_LoaiHopDong { ID = Guid.Empty,  sTenLoaiHopDong = "--Chọn loại hợp đồng--" });
            ViewBag.ListLoaiHopDong = lstLoaiHD.ToSelectList("ID", "sTenLoaiHopDong");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetListHopDongById(Guid id, string sTenHopDong, Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, string soHopDong, DateTime? ngayKyHopDong, Guid? iLoaiHopDong)
        {
            sTenHopDong = HttpUtility.HtmlDecode(sTenHopDong);
            soHopDong = HttpUtility.HtmlDecode(soHopDong);

            IEnumerable<NH_DA_HopDongModel> nhiemVuChiList = _qlnhService.GetAllNHThongTinHopDongById(id, ngayKyHopDong
                , (iDonVi == Guid.Empty ? null : iDonVi)
                , (iChuongTrinh == Guid.Empty ? null : iChuongTrinh)
                , (iDuAn == Guid.Empty ? null : iDuAn)
                , (iLoaiHopDong == Guid.Empty ? null : iLoaiHopDong)
                , sTenHopDong, soHopDong);
            StringBuilder htmlResult = new StringBuilder();
            int index = 1;
            foreach (NH_DA_HopDongModel item in nhiemVuChiList)
            {
                htmlResult.AppendLine("<tr class='child-" + id + "' style='display:none;'>");
                htmlResult.AppendLine("<td class='text-center'>" + index++ + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenHopDong) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenDonVi) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenChuongTrinh) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenDuAn) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sSoHopDong) + "</td>");
                htmlResult.AppendLine("<td class='text-center'>" + HttpUtility.HtmlEncode(item.dNgayHopDongStr) + "</td>");
                htmlResult.AppendLine("<td class='text-center'>" + HttpUtility.HtmlEncode(item.sLoai) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriUSD.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriUSD.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriVND.HasValue ? CommonFunction.DinhDangSo(Math.Round(item.fGiaTriVND.Value).ToString(CultureInfo.InvariantCulture), 0) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriEUR.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriEUR.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriNgoaiTeKhac.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriNgoaiTeKhac.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-center'>" + HttpUtility.HtmlEncode(item.sSoLanDieuChinh) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sDieuChinhTu) + "</td>");
                if (item.bIsActive)
                {
                    htmlResult.AppendLine("<td class='text-center' style='padding:0;'>"
                        + "<button type='button' class='btn-adjust' title='Điều chỉnh' onclick=\"OpenContractInfo('"+ item.ID +"', true)\"><i class='fa fa-copy fa-lg' aria-hidden='true'></i></button>"
                        + "<button type='button' class='btn-detail' title='Xem chi tiết' onclick=\"OpenContractInfoDetail('" + item.ID + "')\"><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>"
                        + "<button type='button' class='btn-edit' title='Sửa' onclick=\"OpenContractInfo('"+ item.ID +"', false)\"><i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i></button>"
                        + "<button type='button' class='btn-delete' title='Xóa' onclick=\"Xoa('"+ item.ID +"')\"><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>"
                        + "</td>");
                }
                else
                {
                    htmlResult.AppendLine("<td class='text-center' style='padding:0;'><button type='button' class='btn-detail' title='Xem chi tiết' onclick=\"OpenContractInfoDetail('"+ item.ID +"')\"><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button></td>");
                }
                htmlResult.AppendLine("</tr>");
            }
            return Json(new { datas = htmlResult.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sTenHopDong, Guid? iDonVi, string maDonVi, Guid? iChuongTrinh, Guid? iDuAn, string soHopDong, DateTime? ngayKyHopDong, Guid? iLoaiHopDong)
        {
            NHDAThongTinHopDongViewModel vm = new NHDAThongTinHopDongViewModel();
            sTenHopDong = HttpUtility.HtmlDecode(sTenHopDong);
            soHopDong = HttpUtility.HtmlDecode(soHopDong);
            vm._paging = _paging;
            vm._paging.ItemsPerPage = 10;
            vm.Items = _qlnhService.GetAllNHThongTinHopDong(ref vm._paging, ngayKyHopDong
                , (iDonVi == Guid.Empty ? null : iDonVi)
                , (iChuongTrinh == Guid.Empty ? null : iChuongTrinh)
                , (iDuAn == Guid.Empty ? null : iDuAn)
                , (iLoaiHopDong == Guid.Empty ? null : iLoaiHopDong)
                , sTenHopDong, soHopDong);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHNhiemVuChiTietTheoDonViId(HttpUtility.HtmlDecode(maDonVi), iDonVi).ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
            ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDuAnTheoKHCTBQPChuongTrinhId(iChuongTrinh).ToList();
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

            List<NH_DM_LoaiHopDong> lstLoaiHD = _qlnhService.GetNHDMLoaiHopDongList().ToList();
            lstLoaiHD.Insert(0, new NH_DM_LoaiHopDong { ID = Guid.Empty,  sTenLoaiHopDong = "--Chọn loại hợp đồng--" });
            ViewBag.ListLoaiHopDong = lstLoaiHD.ToSelectList("ID", "sTenLoaiHopDong");

            return PartialView("_list", vm);
        }

        [HttpPost]
        public JsonResult GetChuongTrinhTheoDonVi(Guid? iDonVi, string maDonVi)
        {
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHNhiemVuChiTietTheoDonViId(HttpUtility.HtmlDecode(maDonVi), iDonVi).ToList();
            StringBuilder htmlChuongtrinh = new StringBuilder();
            htmlChuongtrinh.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn chương trình--");
            if (lstChuongTrinh != null && lstChuongTrinh.Count > 0)
            {
                for (int i = 0; i < lstChuongTrinh.Count; i++)
                {
                    htmlChuongtrinh.AppendFormat("<option value='{0}'>{1}</option>", lstChuongTrinh[i].ID, HttpUtility.HtmlEncode(lstChuongTrinh[i].sTenNhiemVuChi));
                }
            }

            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDADuAnList().ToList();
            StringBuilder htmlDA = new StringBuilder();
            htmlDA.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn dự án--");
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlDA.AppendFormat("<option value='{0}' data-bql='{2}'>{1}</option>", lstDuAn[i].ID, HttpUtility.HtmlEncode(lstDuAn[i].sTenDuAn), lstDuAn[i].iID_BQuanLyID);
                }
            }

            List<NH_DA_GoiThau> lstGoiThau = _qlnhService.GetGoiThauTheoCT(iDonVi).ToList();
            StringBuilder htmlGoiThau = new StringBuilder();
            htmlGoiThau.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn gói thầu--");
            if (lstGoiThau != null && lstGoiThau.Count > 0)
            {
                for (int i = 0; i < lstGoiThau.Count; i++)
                {
                    htmlGoiThau.AppendFormat("<option value='{0}'>{1}</option>", lstGoiThau[i].ID, WebUtility.HtmlEncode(lstGoiThau[i].sTenGoiThau));
                }
            }

            return Json(new { htmlCT = htmlChuongtrinh.ToString(), htmlDA = htmlDA.ToString(), htmlGT= htmlGoiThau.ToString() }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult GetDuAnTheoChuongTrinh(Guid? iChuongTrinh)
        {
            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDuAnTheoKHCTBQPChuongTrinhId(iChuongTrinh).ToList();
            StringBuilder htmlDA = new StringBuilder();
            htmlDA.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn dự án--");
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlDA.AppendFormat("<option value='{0}' data-bql='{2}'>{1}</option>", lstDuAn[i].ID, HttpUtility.HtmlEncode(lstDuAn[i].sTenDuAn), lstDuAn[i].iID_BQuanLyID);
                }
            }
            return Json(htmlDA.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContractInfo(Guid? id, bool? isDieuChinh)
        {
            NH_DA_HopDongModel data = new NH_DA_HopDongModel();
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh;
            List<NH_DA_DuAn> lstDuAn;
            List<NH_DM_LoaiHopDong> lstLoaiHD;
            List<NH_DM_NhaThau> lstNhaThau;
            List<NH_DM_TiGia> lstTiGia;
            List<NH_DM_TiGia_ChiTiet> lstTiGiaChiTiet;

            ViewBag.IsDieuChinh = isDieuChinh != null ? isDieuChinh : false;

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinHopDongById(id.Value);
                if (data != null)
                {
                    if (data.iID_TiGiaID != null)
                    {
                        NH_DM_TiGia tiGia = _qlnhService.GetNHDMTiGiaList(data.iID_TiGiaID).ToList().FirstOrDefault();
                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = _qlnhService.GetNHDMTiGiaChiTietList(data.iID_TiGiaID, false).ToList();
                        if(ViewBag.HtmlTienTe != null)
                            ViewBag.HtmlTienTe = GetHtmlTienteQuyDoi(tiGiaChiTietList, tiGia.sMaTienTeGoc, data.iID_TiGia_ChiTietID, data.sMaNgoaiTeKhac);
                    }

                    lstChuongTrinh = _qlnhService.GetNHNhiemVuChiTietTheoDonViId(data.iID_MaDonVi, data.iID_DonViID).ToList();
                    lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
                    ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi", data.iID_KHCTBQP_ChuongTrinhID.ToString());

                    lstDuAn = _qlnhService.GetNHDuAnTheoKHCTBQPChuongTrinhId(data.iID_KHCTBQP_ChuongTrinhID).ToList();
                    lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
                    ViewBag.ListDuAn = lstDuAn;

                    lstLoaiHD = _qlnhService.GetNHDMLoaiHopDongList().ToList();
                    lstLoaiHD.Insert(0, new NH_DM_LoaiHopDong { ID = Guid.Empty, sTenLoaiHopDong = "--Chọn loại hợp đồng--" });
                    ViewBag.ListLoaiHopDong = lstLoaiHD.ToSelectList("ID", "sTenLoaiHopDong", data.iID_LoaiHopDongID.ToString());

                    lstNhaThau = _qlnhService.GetNHDMNhaThauList().ToList();
                    lstNhaThau.Insert(0, new NH_DM_NhaThau { Id = Guid.Empty, sTenNhaThau = "--Chọn nhà thầu--" });
                    ViewBag.ListNhaThau = lstNhaThau.ToSelectList("Id", "sTenNhaThau", data.iID_NhaThauThucHienID.ToString());

                    lstTiGia = _qlnhService.GetNHDMTiGiaList().ToList();
                    lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
                    ViewBag.ListTiGia = lstTiGia;

                    lstTiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTietList(data.iID_TiGiaID).ToList();
                    lstTiGiaChiTiet.Insert(0, new NH_DM_TiGia_ChiTiet { ID = Guid.Empty, sMaTienTeQuyDoi = "--Chọn mã ngoại tệ khác--" });
                    ViewBag.ListTiGiaChiTiet = lstTiGiaChiTiet.ToSelectList("ID", "sMaTienTeQuyDoi", data.iID_TiGia_ChiTietID.ToString());

                    List<NH_DA_GoiThau> lstGoiThau = _qlnhService.GetGoiThauTheoCT().ToList();
                    lstGoiThau.Insert(0, new NH_DA_GoiThau { ID = Guid.Empty, sTenGoiThau = "--Chọn gói thầu--" });
                    ViewBag.ListGoiThau = lstGoiThau;
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
                lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
                ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

                lstDuAn = _qlnhService.GetNHDADuAnList().ToList();
                lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
                ViewBag.ListDuAn = lstDuAn;

                lstLoaiHD = _qlnhService.GetNHDMLoaiHopDongList().ToList();
                lstLoaiHD.Insert(0, new NH_DM_LoaiHopDong { ID = Guid.Empty, sTenLoaiHopDong = "--Chọn loại hợp đồng--" });
                ViewBag.ListLoaiHopDong = lstLoaiHD.ToSelectList("ID", "sTenLoaiHopDong");

                lstNhaThau = _qlnhService.GetNHDMNhaThauList().ToList();
                lstNhaThau.Insert(0, new NH_DM_NhaThau { Id = Guid.Empty, sTenNhaThau = "--Chọn nhà thầu--" });
                ViewBag.ListNhaThau = lstNhaThau.ToSelectList("Id", "sTenNhaThau");
                
                lstTiGia = _qlnhService.GetNHDMTiGiaList().ToList();
                lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
                ViewBag.ListTiGia = lstTiGia;

                lstTiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTietList(null).ToList();
                lstTiGiaChiTiet.Insert(0, new NH_DM_TiGia_ChiTiet { ID = Guid.Empty, sMaTienTeQuyDoi = "--Chọn mã ngoại tệ khác--" });
                ViewBag.ListTiGiaChiTiet = lstTiGiaChiTiet.ToSelectList("ID", "sMaTienTeQuyDoi");

                List<NH_DA_GoiThau> lstGoiThau = _qlnhService.GetGoiThauTheoCT().ToList();
                lstGoiThau.Insert(0, new NH_DA_GoiThau { ID = Guid.Empty, sTenGoiThau = "--Chọn gói thầu--" });
                ViewBag.ListGoiThau = lstGoiThau;

            }

            return View("_modalUpdate", data);
        }

        public ActionResult GetContractInfoDetail(Guid? id)
        {
            NH_DA_HopDongModel data = new NH_DA_HopDongModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinHopDongById(id.Value);

                if (data != null)
                {
                    var donvi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), data.iID_DonViID.ToString());
                    data.sTenDonVi = donvi != null ? donvi.iID_MaDonVi + " - " + donvi.sTen : string.Empty;
                    var chuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList(data.iID_KHCTBQP_ChuongTrinhID).FirstOrDefault();
                    data.sTenChuongTrinh = chuongTrinh != null ? chuongTrinh.sTenNhiemVuChi : string.Empty;
                    var duAn = _qlnhService.GetNHDADuAnList(data.iID_DuAnID).FirstOrDefault();
                    data.sTenDuAn = duAn != null ? duAn.sTenDuAn : string.Empty;
                    var loaiHopDong = _qlnhService.GetNHDMLoaiHopDongList(data.iID_LoaiHopDongID).FirstOrDefault();
                    data.sTenLoaiHopDong = loaiHopDong != null ? loaiHopDong.sTenLoaiHopDong : string.Empty;
                    var tiGia = _qlnhService.GetNHDMTiGiaList(data.iID_TiGiaID).FirstOrDefault();
                    data.sTenTiGia = tiGia != null ? tiGia.sTenTiGia : string.Empty;
                    var nhaThau = _qlnhService.GetNHDMNhaThauList(data.iID_NhaThauThucHienID).FirstOrDefault();
                    data.sTenNhaThau = nhaThau != null ? nhaThau.sTenNhaThau : string.Empty;
                    var goiThau = _qlnhService.GetGoiThauTheoCT(data.iID_DonViID).FirstOrDefault();
                    data.sTenGoiThau = goiThau != null ? goiThau.sTenGoiThau : string.Empty;
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View("_modalDetail", data);
        }

        [HttpPost]
        public ActionResult OpenModalImport()
        {
            return PartialView("_modalImport");
        }

        [HttpPost]
        public ActionResult LoadDataExcel(HttpPostedFileBase file)
        {
            string data;
            try
            {
                byte[] file_data = GetBytes(file);
                DataTable dataTable = ExcelHelpers.LoadExcelDataTable(file_data);
                IEnumerable<NHDAThongTinHopDongImportModel> dataImport = GetExcelResult(dataTable);
                data = GetHtmlDataExcel(dataImport);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { sMessError = "Không thể tải dữ liệu từ file đã chọn!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        private string GetHtmlDataExcel(IEnumerable<NHDAThongTinHopDongImportModel> dataImport)
        {
            StringBuilder sb = new StringBuilder();
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true).ToList();
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDADuAnList().ToList();
            List<NH_DM_LoaiHopDong> lstLoaiHopDong = _qlnhService.GetNHDMLoaiHopDongList().ToList();
            List<NH_DM_NhaThau> lstNhaThau = _qlnhService.GetNHDMNhaThauList().ToList();

            string htmlDonVi = GetHtmlSelectOptionDonVi(lstDonViQL);
            string htmlChuongTrinh = GetHtmlSelectOptionChuongTrinh(lstChuongTrinh);
            string htmlDuAn = GetHtmlSelectOptionDuAn(lstDuAn);
            string htmlLoai = GetHtmlSelectOptionLoai();
            string htmlLoaiHopDong = GetHtmlSelectOptionLoaiHopDong(lstLoaiHopDong);
            string htmlNhaThau = GetHtmlSelectOptionNhaThau(lstNhaThau);

            int i = 0;
            foreach (NHDAThongTinHopDongImportModel item in dataImport)
            {
                sb.AppendLine("<tr class=\"" + (item.IsDataWrong ? "wrong" : "correct") + "\" data-index=\"" + i + "\">");
                //Trạng thái
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='status-icon'>");
                if (item.IsDataWrong)
                {
                    sb.AppendLine("<i class=\"fa fa-close fa-lg color-text-red\" aria-hidden=\"true\"></i>");
                }
                else
                {
                    sb.AppendLine("<i class=\"fa fa-check fa-lg\" style=\"color:green;\" aria-hidden=\"true\"></i>");
                }
                sb.AppendLine("</td>");

                //STT
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine(HttpUtility.HtmlEncode(item.sSTT));
                sb.AppendLine("</td>");

                //Đơn vị
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<select id=\"slbDonVi" + i + "\" name=\"slbDonVi" + i + "\" class=\"form-control selectDonVi\" data-index=\"" + i + "\" onchange=\"ChangeSelectDonVi(this);\">");
                sb.AppendLine(htmlDonVi);
                sb.AppendLine("</select>");
                sb.AppendLine("</td>");

                //Chương trình
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<select id=\"slbChuongTrinh" + i + "\" name=\"slbChuongTrinh" + i + "\" class=\"form-control selectChuongTrinh\" data-index=\"" + i + "\" onchange=\"ChangeSelectChuongTrinh(this);\">");
                sb.AppendLine(htmlChuongTrinh);
                sb.AppendLine("</select>");
                sb.AppendLine("</td>");

                //Loại
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<select id=\"slbLoai" + i + "\" name=\"slbLoai" + i + "\" class=\"form-control selectLoai\" data-index=\"" + i + "\" onchange=\"ChangeSelectLoai(this);\">");
                sb.AppendLine(htmlLoai);
                sb.AppendLine("</select>");
                sb.AppendLine("</td>");

                //Dự án
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<div class='divDuAn hidden'>");
                sb.AppendLine("<select id=\"slbDuAn" + i + "\" name=\"slbDuAn" + i + "\" class=\"form-control selectDuAn\" data-index=\"" + i + "\">");
                sb.AppendLine(htmlDuAn);
                sb.AppendLine("</select>");
                sb.AppendLine("</div>");
                sb.AppendLine("</td>");

                //Tên hợp đồng
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsTenHopDongWrong ? "cellWrong" : "") + "'>");
                if (item.IsTenHopDongWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtTenHopDong" + i + "' value='" + HttpUtility.HtmlEncode(item.sTenHopDong) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanTenHopDong" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sTenHopDong) + "</span>");
                }
                sb.AppendLine("</td>");

                //Số hợp đồng
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsSoHopDongWrong ? "cellWrong" : "") + "'>");
                if (item.IsSoHopDongWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' required autocomplete='off' id='txtSoHopDong" + i + "' value='" + HttpUtility.HtmlEncode(item.sSoHopDong) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanSoHopDong" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sSoHopDong) + "</span>");
                }
                sb.AppendLine("</td>");

                //Ngày kí hợp đồng
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsNgayKiHopDongWrong ? "cellWrong" : "") + "'>");
                if (item.IsNgayKiHopDongWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate' placeholder='dd/MM/yyyy' autocomplete='off' id='txtNgayKiHopDong" + i + "' value='" + HttpUtility.HtmlEncode(item.sNgayKiHopDong) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanNgayKiHopDong" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sNgayKiHopDong) + "</span>");
                }
                sb.AppendLine("</td>");

                //Loại hợp đồng
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaLoaiHopDongWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaLoaiHopDongWrong)
                {
                    sb.AppendLine("<div class='inputCommon'>");
                    sb.AppendLine("<select id=\"slbLoaiHopDong" + i + "\" name=\"slbLoaiHopDong" + i + "\" class=\"form-control selectLoaiHopDong\">");
                    sb.AppendLine(htmlLoaiHopDong);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaLoaiHopDong" + i + "' data-id='" + item.iID_LoaiHopDong + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sMaLoaiHopDong) + "</span>");
                }
                sb.AppendLine("</td>");

                //Nhà thầu thực diện
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaNhaThauWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaNhaThauWrong)
                {
                    sb.AppendLine("<div class='inputCommon'>");
                    sb.AppendLine("<select id=\"slbNhaThau" + i + "\" name=\"slbNhaThau" + i + "\" class=\"form-control inputCommon selectNhaThau\">");
                    sb.AppendLine(htmlNhaThau);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaNhaThau" + i + "' data-id='" + item.iID_NhaThau + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sMaNhaThau) + "</span>");
                }
                sb.AppendLine("</td>");

                //Thời gian thực hiện từ
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsThoiGianThucHienTuWrong ? "cellWrong" : "") + "'>");
                if (item.IsThoiGianThucHienTuWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate' placeholder='dd/MM/yyyy' autocomplete='off' id='txtThoiGianThucHienTu" + i + "' value='" + HttpUtility.HtmlEncode(item.sThoiGianThucHienTu) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanThoiGianThucHienTu" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sThoiGianThucHienTu) + "</span>");
                }
                sb.AppendLine("</td>");

                //Thời gian thực hiện đến
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsThoiGianThucHienDenWrong ? "cellWrong" : "") + "'>");
                if (item.IsThoiGianThucHienDenWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate' placeholder='dd/MM/yyyy' autocomplete='off' id='txtThoiGianThucHienDen" + i + "' value='" + HttpUtility.HtmlEncode(item.sThoiGianThucHienDen) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanThoiGianThucHienDen" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sThoiGianThucHienDen) + "</span>");
                }
                sb.AppendLine("</td>");

                //Thời gian thực hiện (ngày)
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsThoiGianThucHienWrong ? "cellWrong" : "") + "'>");
                if (item.IsThoiGianThucHienWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputThoiGianThucHien' autocomplete='off' id='txtThoiGianThucHien" + i + "' value='" + HttpUtility.HtmlEncode(item.sThoiGianThucHien) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanThoiGianThucHien" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sThoiGianThucHien) + "</span>");
                }
                sb.AppendLine("</td>");

                //Lỗi
                sb.AppendLine("<td align='left' class='color-text-red cellMessageError' style='vertical-align:middle;'>");
                sb.AppendLine(item.sErrorMessage);
                sb.AppendLine("</td>");

                //Thao tác
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<button type=\"button\" id=\"btn-delete" + i + "\" class=\"btn-delete\" title=\"Xóa\" onclick=\"ConfirmRemoveRowImport(" + i + ");\"><i class=\"fa fa-trash-o fa-lg\" aria-hidden=\"true\"></i></button>");
                sb.AppendLine("</td>");

                sb.AppendLine("</tr>");

                i++;
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionDonVi(List<NS_DonVi> donViList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' data-madonvi='' selected>{1}</option>", Guid.Empty, "--Chọn đơn vị--");
            if (donViList != null)
            {
                for (int i = 0; i < donViList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}' data-madonvi='{2}'>{1}</option>", donViList[i].iID_Ma, HttpUtility.HtmlEncode(donViList[i].sMoTa), HttpUtility.HtmlEncode(donViList[i].iID_MaDonVi));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionChuongTrinh(List<NH_KHChiTietBQP_NhiemVuChi> chuongTrinhList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn chương trình--");
            if (chuongTrinhList != null)
            {
                for (int i = 0; i < chuongTrinhList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", chuongTrinhList[i].ID, HttpUtility.HtmlEncode(chuongTrinhList[i].sTenNhiemVuChi));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionLoai()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", 0, "--Chọn loại--");
            sb.AppendFormat("<option value='{0}'>{1}</option>", 1, "Dự án, Trong nước");
            sb.AppendFormat("<option value='{0}'>{1}</option>", 2, "Mua sắm, Trong nước");
            sb.AppendFormat("<option value='{0}'>{1}</option>", 3, "Dự án, Ngoại thương");
            sb.AppendFormat("<option value='{0}'>{1}</option>", 4, "Mua sắm, Ngoại thương");
            return sb.ToString();
        }

        private string GetHtmlSelectOptionDuAn(List<NH_DA_DuAn> duAnList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' data-bql='' selected>{1}</option>", Guid.Empty, "--Chọn dự án--");
            if (duAnList != null)
            {
                for (int i = 0; i < duAnList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}' data-bql='{2}'>{1}</option>", duAnList[i].ID, HttpUtility.HtmlEncode(duAnList[i].sTenDuAn), HttpUtility.HtmlEncode(duAnList[i].iID_BQuanLyID));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionLoaiHopDong(List<NH_DM_LoaiHopDong> loaiHopDongList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn loại hợp đồng--");
            if (loaiHopDongList != null)
            {
                for (int i = 0; i < loaiHopDongList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", loaiHopDongList[i].ID, HttpUtility.HtmlEncode(loaiHopDongList[i].sTenLoaiHopDong));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionNhaThau(List<NH_DM_NhaThau> nhaThauList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn nhà thầu--");
            if (nhaThauList != null)
            {
                for (int i = 0; i < nhaThauList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", nhaThauList[i].Id, HttpUtility.HtmlEncode(nhaThauList[i].sTenNhaThau));
                }
            }
            return sb.ToString();
        }

        private IEnumerable<NHDAThongTinHopDongImportModel> GetExcelResult(DataTable dt)
        {
            List<NHDAThongTinHopDongImportModel> dataImportList = new List<NHDAThongTinHopDongImportModel>();
            NHDAThongTinHopDongImportModel data = new NHDAThongTinHopDongImportModel();
            DataRow row;

            string sSTT = string.Empty;
            string sTenHopDong = string.Empty;
            string sSoHopDong = string.Empty;
            string sNgayKiHopDong = string.Empty;
            string sMaLoaiHopDong = string.Empty;
            string sMaNhaThau = string.Empty;
            string sThoiGianThucHienTu = string.Empty;
            string sThoiGianThucHienDen = string.Empty;
            string sThoiGianThucHien = string.Empty;
            DateTime? dNgayKiHopDong;
            DateTime? dThoiGianThucHienTu;
            DateTime? dThoiGianThucHienDen;
            int? thoiGianThucHien;
            Guid? iID_LoaiHopDong = null;
            Guid? iID_NhaThau = null;

            StringBuilder sErrorMessage = new StringBuilder();
            bool isDataWrong = false;
            bool IsTenHopDongWrong = false;
            bool IsSoHopDongWrong = false;
            bool IsNgayKiHopDongWrong = false;
            bool IsMaLoaiHopDongWrong = false;
            bool IsMaNhaThauWrong = false;
            bool IsThoiGianThucHienTuWrong = false;
            bool IsThoiGianThucHienDenWrong = false;
            bool IsThoiGianThucHienWrong = false;

            NH_DM_LoaiHopDong dmLoaiHopDong;
            NH_DM_NhaThau dmNhaThau;
            IEnumerable<NH_DM_LoaiHopDong> loaiHopDongList = _qlnhService.GetNHDMLoaiHopDongList();
            IEnumerable<NH_DM_NhaThau> nhaThauList = _qlnhService.GetNHDMNhaThauList();

            var items = dt.AsEnumerable();
            for (var i = 13; i < items.Count(); i++)
            {
                isDataWrong = false;
                IsTenHopDongWrong = false;
                IsSoHopDongWrong = false;
                IsNgayKiHopDongWrong = false;
                IsMaLoaiHopDongWrong = false;
                IsMaNhaThauWrong = false;
                IsThoiGianThucHienTuWrong = false;
                IsThoiGianThucHienDenWrong = false;
                IsThoiGianThucHienWrong = false;
                sErrorMessage.Clear();
                row = items.ToList()[i];

                sSTT = row.Field<string>(0);
                sTenHopDong = !string.IsNullOrEmpty(row.Field<string>(1)) ? row.Field<string>(1).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sTenHopDong) && sTenHopDong.Length > 300)
                {
                    isDataWrong = true;
                    IsTenHopDongWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Tên hợp đồng nhập quá 300 kí tự.");
                }

                sSoHopDong = !string.IsNullOrEmpty(row.Field<string>(2)) ? row.Field<string>(2).Trim() : string.Empty;
                if (string.IsNullOrEmpty(sSoHopDong))
                {
                    isDataWrong = true;
                    IsSoHopDongWrong=true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số hợp đồng chưa được nhập.");
                }
                else if (sSoHopDong.Length > 100)
                {
                    isDataWrong = true;
                    IsSoHopDongWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số hợp đồng nhập quá 100 kí tự.");
                }

                sNgayKiHopDong = !string.IsNullOrEmpty(row.Field<string>(3)) ? row.Field<string>(3).Trim() : string.Empty;
                dNgayKiHopDong = CommonFunction.TryParseDateTime(sNgayKiHopDong);
                if (!string.IsNullOrEmpty(sNgayKiHopDong))
                {
                    if (!dNgayKiHopDong.HasValue)
                    {
                        isDataWrong = true;
                        IsNgayKiHopDongWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Ngày kí hợp đồng không hợp lệ.");
                    }
                    else
                    {
                        sNgayKiHopDong = dNgayKiHopDong.Value.ToString("dd/MM/yyyy");
                    }
                }

                sMaLoaiHopDong = !string.IsNullOrEmpty(row.Field<string>(4)) ? row.Field<string>(4).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sMaLoaiHopDong))
                {
                    dmLoaiHopDong = loaiHopDongList.Where(x => x.sMaLoaiHopDong.Equals(sMaLoaiHopDong)).FirstOrDefault();
                    if (dmLoaiHopDong == null)
                    {
                        isDataWrong = true;
                        IsMaLoaiHopDongWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã loại hợp đồng không tồn tại.");
                    }
                    else
                    {
                        iID_LoaiHopDong = dmLoaiHopDong.ID;
                    }
                }

                sMaNhaThau = !string.IsNullOrEmpty(row.Field<string>(5)) ? row.Field<string>(5).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sMaNhaThau))
                {
                    dmNhaThau = nhaThauList.Where(x => x.sMaNhaThau.Equals(sMaNhaThau)).FirstOrDefault();
                    if (dmNhaThau == null)
                    {
                        isDataWrong = true;
                        IsMaNhaThauWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã nhà thầu đại diện không tồn tại.");
                    }
                    else
                    {
                        iID_NhaThau = dmNhaThau.Id;
                    }
                }

                sThoiGianThucHienTu = !string.IsNullOrEmpty(row.Field<string>(6)) ? row.Field<string>(6).Trim() : string.Empty;
                dThoiGianThucHienTu = CommonFunction.TryParseDateTime(sThoiGianThucHienTu);
                if (!string.IsNullOrEmpty(sThoiGianThucHienTu))
                {
                    if (!dThoiGianThucHienTu.HasValue)
                    {
                        isDataWrong = true;
                        IsThoiGianThucHienTuWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Thời gian thực hiện từ không hợp lệ.");
                    }
                    else
                    {
                        sThoiGianThucHienTu = dThoiGianThucHienTu.Value.ToString("dd/MM/yyyy");
                    }
                }

                sThoiGianThucHienDen = !string.IsNullOrEmpty(row.Field<string>(7)) ? row.Field<string>(7).Trim() : string.Empty;
                dThoiGianThucHienDen = CommonFunction.TryParseDateTime(sThoiGianThucHienDen);
                if (!string.IsNullOrEmpty(sThoiGianThucHienDen))
                {
                    if (!dThoiGianThucHienDen.HasValue)
                    {
                        isDataWrong = true;
                        IsThoiGianThucHienDenWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Thời gian thực hiện đến không hợp lệ.");
                    }
                    else
                    {
                        sThoiGianThucHienDen = dThoiGianThucHienDen.Value.ToString("dd/MM/yyyy");
                    }
                }

                sThoiGianThucHien = !string.IsNullOrEmpty(row.Field<string>(8)) ? row.Field<string>(8).Trim() : string.Empty;
                thoiGianThucHien = CommonFunction.TryParseInt(sThoiGianThucHien);
                if (!string.IsNullOrEmpty(sThoiGianThucHien) && thoiGianThucHien == null)
                {
                    isDataWrong = true;
                    IsThoiGianThucHienWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Thời gian thực hiện không hợp lệ.");
                }

                data = new NHDAThongTinHopDongImportModel
                {
                    sSTT = sSTT,
                    sTenHopDong = sTenHopDong,
                    sSoHopDong = sSoHopDong,
                    sNgayKiHopDong = sNgayKiHopDong,
                    sMaLoaiHopDong = sMaLoaiHopDong,
                    sMaNhaThau = sMaNhaThau,
                    iID_LoaiHopDong = iID_LoaiHopDong,
                    iID_NhaThau = iID_NhaThau,
                    sThoiGianThucHienTu = sThoiGianThucHienTu,
                    sThoiGianThucHienDen = sThoiGianThucHienDen,
                    sThoiGianThucHien = sThoiGianThucHien,
                    sErrorMessage = sErrorMessage.ToString(),
                    IsDataWrong = isDataWrong,
                    IsTenHopDongWrong = IsTenHopDongWrong,
                    IsSoHopDongWrong = IsSoHopDongWrong,
                    IsNgayKiHopDongWrong = IsNgayKiHopDongWrong,
                    IsMaLoaiHopDongWrong = IsMaLoaiHopDongWrong,
                    IsMaNhaThauWrong = IsMaNhaThauWrong,
                    IsThoiGianThucHienTuWrong = IsThoiGianThucHienTuWrong,
                    IsThoiGianThucHienDenWrong = IsThoiGianThucHienDenWrong,
                    IsThoiGianThucHienWrong = IsThoiGianThucHienWrong
                };

                dataImportList.Add(data);
            }
            return dataImportList.AsEnumerable();
        }

        private byte[] GetBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        public ActionResult DownloadTemplateImport()
        {
            try
            {
                XLWorkbook w_b = new XLWorkbook();
                var wbContractInfo = w_b.Worksheets.Add("Biểu mẫu thông tin hợp đồng");
                var wbLoaiHopDong = w_b.Worksheets.Add("Loại hợp đồng");
                var wbNhaThau = w_b.Worksheets.Add("Nhà thầu đại diện");
                wbContractInfo.Column(1).Width = 15;
                wbContractInfo.Column(2).Width = 40;
                wbContractInfo.Column(3).Width = 30;
                wbContractInfo.Column(4).Width = 25;
                wbContractInfo.Column(5).Width = 25;
                wbContractInfo.Column(6).Width = 25;
                wbContractInfo.Column(7).Width = 25;
                wbContractInfo.Column(8).Width = 25;
                wbContractInfo.Column(9).Width = 25;

                wbLoaiHopDong.Column(1).Width = 15;
                wbLoaiHopDong.Column(2).Width = 40;
                wbLoaiHopDong.Column(3).Width = 40;
                wbLoaiHopDong.Column(4).Width = 50;
                wbLoaiHopDong.Column(5).Width = 60;

                wbNhaThau.Column(1).Width = 15;
                wbNhaThau.Column(2).Width = 40;
                wbNhaThau.Column(3).Width = 60;
                wbNhaThau.Column(4).Width = 25;
                wbNhaThau.Column(5).Width = 50;
                wbNhaThau.Column(6).Width = 40;
                wbNhaThau.Column(7).Width = 60;
                wbNhaThau.Column(8).Width = 40;
                wbNhaThau.Column(9).Width = 40;
                wbNhaThau.Column(10).Width = 35;
                wbNhaThau.Column(11).Width = 60;
                wbNhaThau.Column(12).Width = 40;
                wbNhaThau.Column(13).Width = 35;
                wbNhaThau.Column(14).Width = 35;
                wbNhaThau.Column(15).Width = 50;
                wbNhaThau.Column(16).Width = 40;
                wbNhaThau.Column(17).Width = 35;
                wbNhaThau.Column(18).Width = 50;
                wbNhaThau.Column(19).Width = 50;
                wbNhaThau.Column(20).Width = 30;

                //Sheet biểu mẫu
                wbContractInfo.Style.Font.FontName = "Times New Roman";
                wbContractInfo.Style.Font.FontSize = 13;
                wbContractInfo.PageSetup.FitToPages(1, 1);
                wbContractInfo.Row(13).Height = 40;
                wbContractInfo.Cell(2, 1).Value = "BIỂU MẪU IMPORT THÔNG TIN HỢP ĐỒNG";
                wbContractInfo.Range(2, 1, 2, 9).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbContractInfo.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true);
                wbContractInfo.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("0");

                wbContractInfo.Cell(4, 1).Value = "Tên hợp đồng: Nhập số, chữ, tối đa 300 ký tự";
                wbContractInfo.Cell(4, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(5, 1).Value = "Số hợp đồng: Nhập số, chữ, tối đa 100 ký tự";
                wbContractInfo.Cell(5, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(6, 1).Value = "Ngày kí hợp đồng: Chỉ nhập dạng dd/mm/yyyy";
                wbContractInfo.Cell(6, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(7, 1).Value = "Loại hợp đồng: Chỉ chọn Mã loại hợp đồng trong danh mục Loại hợp đồng";
                wbContractInfo.Cell(7, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(8, 1).Value = "Nhà thầu đại diện: Chỉ chọn Mã nhà thầu đại diện trong danh mục Nhà thầu đại diện";
                wbContractInfo.Cell(8, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(9, 1).Value = "Thời gian thực hiện từ: Chỉ nhập dạng dd/mm/yyyy";
                wbContractInfo.Cell(9, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(10, 1).Value = "Thời gian thực hiện đến: Chỉ nhập dạng dd/mm/yyyy";
                wbContractInfo.Cell(10, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(11, 1).Value = "Thời gian thực hiện (ngày): Nhập số";
                wbContractInfo.Cell(11, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);

                wbContractInfo.Cell(13, 1).Value = "STT";
                wbContractInfo.Cell(13, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 2).Value = "Tên hợp đồng";
                wbContractInfo.Cell(13, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 3).Value = "Số hợp đồng";
                wbContractInfo.Cell(13, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 4).Value = "Ngày kí hợp đồng";
                wbContractInfo.Cell(13, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 5).Value = "Loại hợp đồng";
                wbContractInfo.Cell(13, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 6).Value = "Nhà thầu đại diện";
                wbContractInfo.Cell(13, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 7).Value = "Thời gian thực hiện từ";
                wbContractInfo.Cell(13, 7).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 8).Value = "Thời gian thực hiện đến";
                wbContractInfo.Cell(13, 8).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(13, 9).Value = "Thời gian thực hiện (ngày)";
                wbContractInfo.Cell(13, 9).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                //Sheet Loại hợp đồng
                wbLoaiHopDong.Style.Font.FontName = "Times New Roman";
                wbLoaiHopDong.Style.Font.FontSize = 13;
                wbLoaiHopDong.PageSetup.FitToPages(1, 1);
                wbLoaiHopDong.Row(4).Height = 30;
                wbLoaiHopDong.Cell(2, 1).Value = "DANH MỤC LOẠI HỢP ĐỒNG";
                wbLoaiHopDong.Range(2, 1, 2, 5).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbLoaiHopDong.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbLoaiHopDong.Cell(4, 1).Value = "STT";
                wbLoaiHopDong.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbLoaiHopDong.Cell(4, 2).Value = "Mã loại hợp đồng";
                wbLoaiHopDong.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbLoaiHopDong.Cell(4, 3).Value = "Tên viết tắt";
                wbLoaiHopDong.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbLoaiHopDong.Cell(4, 4).Value = "Tên loại hợp đồng";
                wbLoaiHopDong.Cell(4, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbLoaiHopDong.Cell(4, 5).Value = "Mô tả";
                wbLoaiHopDong.Cell(4, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                IEnumerable<NH_DM_LoaiHopDong> loaiHopDongList = _qlnhService.GetNHDMLoaiHopDongList();
                int i = 5;
                int j = 1;
                foreach (NH_DM_LoaiHopDong item in loaiHopDongList)
                {
                    wbLoaiHopDong.Cell(i, 1).Value = j;
                    wbLoaiHopDong.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbLoaiHopDong.Cell(i, 2).Value = item.sMaLoaiHopDong;
                    wbLoaiHopDong.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbLoaiHopDong.Cell(i, 3).Value = item.sTenVietTat;
                    wbLoaiHopDong.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbLoaiHopDong.Cell(i, 4).Value = item.sTenLoaiHopDong;
                    wbLoaiHopDong.Cell(i, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbLoaiHopDong.Cell(i, 5).Value = item.sMoTa;
                    wbLoaiHopDong.Cell(i, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }

                //Sheet Nhà thầu đại diện
                wbNhaThau.Style.Font.FontName = "Times New Roman";
                wbNhaThau.Style.Font.FontSize = 13;
                wbNhaThau.PageSetup.FitToPages(1, 1);
                wbNhaThau.Row(4).Height = 30;
                wbNhaThau.Cell(2, 1).Value = "DANH MỤC NHÀ THẦU ĐẠI DIỆN";
                wbNhaThau.Range(2, 1, 2, 20).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbNhaThau.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbNhaThau.Cell(4, 1).Value = "STT";
                wbNhaThau.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 2).Value = "Mã nhà thầu";
                wbNhaThau.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 3).Value = "Tên nhà thầu";
                wbNhaThau.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 4).Value = "Loại nhà thầu";
                wbNhaThau.Cell(4, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 5).Value = "Địa chỉ";
                wbNhaThau.Cell(4, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 6).Value = "Đại diện";
                wbNhaThau.Cell(4, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 7).Value = "Website";
                wbNhaThau.Cell(4, 7).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 8).Value = "Mã số thuế";
                wbNhaThau.Cell(4, 8).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 9).Value = "Ngân hàng";
                wbNhaThau.Cell(4, 9).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 10).Value = "Mã ngân hàng";
                wbNhaThau.Cell(4, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 11).Value = "Số tài khoản";
                wbNhaThau.Cell(4, 11).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 12).Value = "Chức vụ";
                wbNhaThau.Cell(4, 12).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 13).Value = "Điện thoại";
                wbNhaThau.Cell(4, 13).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 14).Value = "Fax";
                wbNhaThau.Cell(4, 14).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 15).Value = "Email";
                wbNhaThau.Cell(4, 15).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 16).Value = "Ngừoi liên hệ";
                wbNhaThau.Cell(4, 16).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 17).Value = "Điện thoại liên hệ";
                wbNhaThau.Cell(4, 17).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 18).Value = "Số CMND";
                wbNhaThau.Cell(4, 18).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 19).Value = "Nơi cấp CMND";
                wbNhaThau.Cell(4, 19).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbNhaThau.Cell(4, 20).Value = "Ngày cấp CMND";
                wbNhaThau.Cell(4, 20).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                IEnumerable<NH_DM_NhaThau> nhaThauList = _qlnhService.GetNHDMNhaThauList();
                i = 5;
                j = 1;
                foreach(NH_DM_NhaThau nhaThau in nhaThauList)
                {
                    wbNhaThau.Cell(i, 1).Value = j;
                    wbNhaThau.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 2).Value = nhaThau.sMaNhaThau;
                    wbNhaThau.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 3).Value = nhaThau.sTenNhaThau;
                    wbNhaThau.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    string loaiNhaThau = string.Empty;
                    if (nhaThau.iLoai == 1)
                    {
                        loaiNhaThau = "Nhà thầu";
                    }
                    else if (nhaThau.iLoai == 2)
                    {
                        loaiNhaThau = "Đơn vị ủy thác";
                    }
                    wbNhaThau.Cell(i, 4).Value = loaiNhaThau;
                    wbNhaThau.Cell(i, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 5).Value = nhaThau.sDiaChi;
                    wbNhaThau.Cell(i, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 6).Value = nhaThau.sDaiDien;
                    wbNhaThau.Cell(i, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 7).Value = nhaThau.sWebsite;
                    wbNhaThau.Cell(i, 7).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 8).Value = nhaThau.sMaSoThue;
                    wbNhaThau.Cell(i, 8).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 9).Value = nhaThau.sNganHang;
                    wbNhaThau.Cell(i, 9).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 10).Value = nhaThau.sMaNganHang;
                    wbNhaThau.Cell(i, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 11).Value = nhaThau.sSoTaiKhoan;
                    wbNhaThau.Cell(i, 11).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 12).Value = nhaThau.sChucVu;
                    wbNhaThau.Cell(i, 12).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 13).Value = nhaThau.sDienThoai;
                    wbNhaThau.Cell(i, 13).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 14).Value = nhaThau.sFax;
                    wbNhaThau.Cell(i, 14).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 15).Value = nhaThau.sEmail;
                    wbNhaThau.Cell(i, 15).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 16).Value = nhaThau.sNguoiLienHe;
                    wbNhaThau.Cell(i, 16).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 17).Value = nhaThau.sDienThoaiLienHe;
                    wbNhaThau.Cell(i, 17).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 18).Value = nhaThau.sSoCMND;
                    wbNhaThau.Cell(i, 18).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 19).Value = nhaThau.sNoiCapCMND;
                    wbNhaThau.Cell(i, 19).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 20).Value = nhaThau.dNgayCapCMND.HasValue ? nhaThau.dNgayCapCMND.Value.ToString("dd/MM/yyyy") : string.Empty;
                    wbNhaThau.Cell(i, 20).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }

                wbContractInfo.Column(2).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(2).AsRange().SetDataValidation().TextLength.Between(0, 300);
                wbContractInfo.Column(2).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorMessage = "Tên hợp đồng vượt quá 300 kí tự! Vui lòng nhập lại.";
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 2, 13, 2).SetDataValidation().ClearRanges();

                wbContractInfo.Column(3).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(3).AsRange().SetDataValidation().TextLength.Between(0, 100);
                wbContractInfo.Column(3).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(3).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(3).AsRange().SetDataValidation().ErrorMessage = "Số hợp đồng vượt quá 100 kí tự! Vui lòng nhập lại.";
                wbContractInfo.Column(3).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 3, 13, 3).SetDataValidation().ClearRanges();

                if (loaiHopDongList.Count() > 0)
                {
                    wbContractInfo.Column(5).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(5).AsRange().SetDataValidation().List(wbLoaiHopDong.Range(5, 2, wbLoaiHopDong.RowCount(), 2), true);
                    wbContractInfo.Column(5).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(5).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(5).AsRange().SetDataValidation().ErrorMessage = "Mã loại hợp đồng không tồn tại!";
                    wbContractInfo.Column(5).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 5, 13, 5).SetDataValidation().ClearRanges();
                }
                if (nhaThauList.Count() > 0)
                {
                    wbContractInfo.Column(6).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(6).AsRange().SetDataValidation().List(wbNhaThau.Range(5, 2, wbNhaThau.RowCount(), 2), true);
                    wbContractInfo.Column(6).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(6).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(6).AsRange().SetDataValidation().ErrorMessage = "Mã nhà thầu đại diện không tồn tại!";
                    wbContractInfo.Column(6).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 6, 13, 6).SetDataValidation().ClearRanges();
                }

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Biểu mẫu import thông tin hợp đồng.xlsx");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    w_b.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        [HttpPost]
        public JsonResult Save(NH_DA_HopDong data, NHDAThongTinHopDongGiaTriTienTeModel giaTriTienData, bool isDieuChinh)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            if (giaTriTienData != null)
            {
                data.fGiaTriUSD = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriUSD);
                data.fGiaTriVND = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriVND);
                data.fGiaTriEUR = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriEUR);
                data.fGiaTriNgoaiTeKhac = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriNgoaiTeKhac);
            }

            data.sTenHopDong = HttpUtility.HtmlDecode(data.sTenHopDong);
            data.sSoHopDong = HttpUtility.HtmlDecode(data.sSoHopDong);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);

            if (!_qlnhService.SaveThongTinHopDong(data, isDieuChinh, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveImport(List<NH_DA_HopDong> contractList)
        {
            if (contractList == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không import được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            foreach(var contract in contractList)
            {
                contract.sTenHopDong = HttpUtility.HtmlDecode(contract.sTenHopDong);
                contract.sSoHopDong = HttpUtility.HtmlDecode(contract.sSoHopDong);
                contract.iID_MaDonVi = HttpUtility.HtmlDecode(contract.iID_MaDonVi);
                contract.dNgayTao = DateTime.Now;
                contract.sNguoiTao = Username;
                contract.bIsActive = true;
                contract.bIsGoc = true;
            }

            if (!_qlnhService.SaveImportThongTinHopDong(contractList))
            {
                return Json(new { bIsComplete = false, sMessError = "Không import được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            if (!_qlnhService.DeleteThongTinHopDong(Guid.Parse(id)))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeTiGia(Guid? idTiGia, NHDAThongTinHopDongGiaTriTienTeModel giaTriTienData)
        {
            try
            {
                StringBuilder htmlMNTK = new StringBuilder();
                string htmlTienTe = string.Empty;
                string sGiaTriUSD = string.Empty;
                string sGiaTriVND = string.Empty;
                string sGiaTriEUR = string.Empty;
                bool isChangeInputUSD = false;
                bool isChangeInputVND = false;
                bool isChangeInputEUR = false;
                bool isReadonlyTxtMaNTKhac = false;

                if (idTiGia != null && idTiGia != Guid.Empty)
                {
                    List<NH_DM_TiGia_ChiTiet> lstMaNgoaiTeKhac = _qlnhService.GetNHDMTiGiaChiTietList(idTiGia).ToList();
                    htmlMNTK.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn mã ngoại tệ khác--");
                    if (lstMaNgoaiTeKhac != null && lstMaNgoaiTeKhac.Count > 0)
                    {
                        for (int i = 0; i < lstMaNgoaiTeKhac.Count; i++)
                        {
                            htmlMNTK.AppendFormat("<option value='{0}'>{1}</option>", lstMaNgoaiTeKhac[i].ID, HttpUtility.HtmlEncode(lstMaNgoaiTeKhac[i].sMaTienTeQuyDoi));
                        }
                    }

                    if (giaTriTienData != null)
                    {
                        NH_DM_TiGia tiGia = _qlnhService.GetNHDMTiGiaList(idTiGia).ToList().SingleOrDefault();
                        if (tiGia != null)
                        {
                            double? giaTriUSDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriUSD);
                            double? giaTriVNDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriVND);
                            double? giaTriEURInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriEUR);

                            List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = _qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                            string maTienTeGoc = tiGia.sMaTienTeGoc;
                            htmlTienTe = GetHtmlTienteQuyDoi(tiGiaChiTietList, maTienTeGoc, null, "");
                            if (MaTienTeList.Contains(maTienTeGoc.ToUpper())) isReadonlyTxtMaNTKhac = true;
                            switch (maTienTeGoc)
                            {
                                case "USD":
                                    if (giaTriUSDInput.HasValue)
                                    {
                                        sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputVND = true;
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputEUR = true;
                                        }
                                    }
                                    else
                                    {
                                        isChangeInputVND = true;
                                        isChangeInputEUR = true;
                                    }
                                    break;
                                case "VND":
                                    if (giaTriVNDInput.HasValue)
                                    {
                                        sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value).ToString(CultureInfo.InvariantCulture), 0);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputUSD = true;
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputEUR = true;
                                        }
                                    }
                                    else
                                    {
                                        isChangeInputUSD = true;
                                        isChangeInputEUR = true;
                                    }
                                    break;
                                case "EUR":
                                    if (giaTriEURInput.HasValue)
                                    {
                                        sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND= true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputVND = true;
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputUSD = true;
                                        }
                                    }
                                    else
                                    {
                                        isChangeInputVND = true;
                                        isChangeInputUSD = true;
                                    }
                                    break;
                                default:
                                    isChangeInputVND = true;
                                    isChangeInputUSD = true;
                                    isChangeInputEUR = true;
                                    isReadonlyTxtMaNTKhac = false;
                                    break;
                            }
                        }
                    }
                }
                return Json(new { bIsComplete = true, htmlMNTK = htmlMNTK.ToString(), htmlTienTe = htmlTienTe
                    , isChangeInputUSD = isChangeInputUSD, isChangeInputVND = isChangeInputVND, isChangeInputEUR = isChangeInputEUR
                    , sGiaTriUSD = sGiaTriUSD, sGiaTriVND = sGiaTriVND, sGiaTriEUR = sGiaTriEUR
                    , isReadonlyTxtMaNTKhac = isReadonlyTxtMaNTKhac
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { bIsComplete = false, sMessError = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ChangeTiGiaNgoaiTeKhac(Guid? idTiGia, Guid? idNgoaiTeKhac, string maNgoaiTeKhac, NHDAThongTinHopDongGiaTriTienTeModel giaTriTienData)
        {
            try
            {
                string sGiaTriUSD = string.Empty;
                string sGiaTriVND = string.Empty;
                string sGiaTriEUR = string.Empty;
                string sGiaTriNTKhac = string.Empty;
                string htmlTienTe = string.Empty;
                bool isChangeInputNgoaiTe = false;
                bool isChangeInputCommon = false;
                bool isReadonlyTxtMaNTKhac = false;
                maNgoaiTeKhac = HttpUtility.HtmlDecode(maNgoaiTeKhac);
                if (idTiGia != null && idTiGia != Guid.Empty && !string.IsNullOrEmpty(maNgoaiTeKhac))
                {
                    NH_DM_TiGia tiGia = _qlnhService.GetNHDMTiGiaList(idTiGia).ToList().SingleOrDefault();
                    if (tiGia != null)
                    {
                        double? giaTriUSDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriUSD);
                        double? giaTriVNDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriVND);
                        double? giaTriEURInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriEUR);
                        double? giaTriNTKhacInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriNgoaiTeKhac);

                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = _qlnhService.GetNHDMTiGiaChiTietList(idTiGia).ToList();
                        string maTienTeGoc = tiGia.sMaTienTeGoc;
                        isReadonlyTxtMaNTKhac = MaTienTeList.Contains(maTienTeGoc.ToUpper());
                        htmlTienTe = GetHtmlTienteQuyDoi(_qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList(), maTienTeGoc, idNgoaiTeKhac, maNgoaiTeKhac);

                        if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty && !maTienTeGoc.ToUpper().Equals(maNgoaiTeKhac.ToUpper()))
                        {
                            NH_DM_TiGia_ChiTiet tiGiaChiTiet = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals(maNgoaiTeKhac.ToUpper())).FirstOrDefault();
                            if (tiGiaChiTiet != null)
                            {
                                double? fTiGia = tiGiaChiTiet.fTiGia;
                                switch (maTienTeGoc)
                                {
                                    case "USD":
                                        if (fTiGia.HasValue && giaTriUSDInput.HasValue)
                                        {
                                            sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGia.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                            isChangeInputNgoaiTe = true;
                                        }
                                        break;
                                    case "VND":
                                        if (fTiGia.HasValue && giaTriVNDInput.HasValue)
                                        {
                                            sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGia.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                            isChangeInputNgoaiTe = true;
                                        }
                                        break;
                                    case "EUR":
                                        if (fTiGia.HasValue && giaTriEURInput.HasValue)
                                        {
                                            sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGia.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                            isChangeInputNgoaiTe = true;
                                        }
                                        break;
                                    default:
                                        if (giaTriNTKhacInput.HasValue && fTiGia.HasValue)
                                        {
                                            double fTienMaGoc = giaTriNTKhacInput.Value / fTiGia.Value;
                                            List<NH_DM_TiGia_ChiTiet> tiGiaChiTietTheoMaGocList = _qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                                            if (tiGiaChiTietTheoMaGocList != null)
                                            {
                                                NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                                                if (tiGiaChiTietUSD != null)
                                                {
                                                    double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                                    if (fTiGiaUSD.HasValue)
                                                    {
                                                        sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                        isChangeInputCommon = true;
                                                    }
                                                }

                                                NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                                                if (tiGiaChiTietVND != null)
                                                {
                                                    double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                                    if (fTiGiaVND.HasValue)
                                                    {
                                                        sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                        isChangeInputCommon = true;
                                                    }
                                                }

                                                NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("EUR")).FirstOrDefault();
                                                if (tiGiaChiTietEUR != null)
                                                {
                                                    double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                                    if (fTiGiaEUR.HasValue)
                                                    {
                                                        sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                        isChangeInputCommon = true;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                return Json(new
                {
                    bIsComplete = true,
                    isChangeInputNgoaiTe = isChangeInputNgoaiTe,
                    isChangeInputCommon = isChangeInputCommon,
                    sGiaTriUSD = sGiaTriUSD,
                    sGiaTriVND = sGiaTriVND,
                    sGiaTriEUR = sGiaTriEUR,
                    sGiaTriNTKhac = sGiaTriNTKhac,
                    htmlTienTe = htmlTienTe,
                    isReadonlyTxtMaNTKhac = isReadonlyTxtMaNTKhac
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { bIsComplete = false, sMessError = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ChangeGiaTien(Guid? idTiGia, Guid? idNgoaiTeKhac, string maNgoaiTeKhac, string txtBlur, NHDAThongTinHopDongGiaTriTienTeModel giaTriTienData)
        {
            try
            {
                string sGiaTriUSD = string.Empty;
                string sGiaTriVND = string.Empty;
                string sGiaTriEUR = string.Empty;
                string sGiaTriNTKhac = string.Empty;
                bool isChangeInputUSD = false;
                bool isChangeInputVND = false;
                bool isChangeInputEUR = false;
                bool isChangeInputNgoaiTe = false;
                maNgoaiTeKhac = HttpUtility.HtmlDecode(maNgoaiTeKhac);
                if (idTiGia != null && idTiGia != Guid.Empty && giaTriTienData != null)
                {
                    NH_DM_TiGia tiGia = _qlnhService.GetNHDMTiGiaList(idTiGia).ToList().SingleOrDefault();
                    if (tiGia != null)
                    {
                        double? giaTriUSDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriUSD);
                        double? giaTriVNDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriVND);
                        double? giaTriEURInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriEUR);
                        double? giaTriNTKhacInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriNgoaiTeKhac);

                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = _qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                        string maTienTeGoc = tiGia.sMaTienTeGoc;
                        if (MaTienTeList.Contains(txtBlur.ToUpper()))
                        {
                            switch(maTienTeGoc)
                            {
                                case "USD":
                                    if (giaTriUSDInput.HasValue)
                                    {
                                        sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputVND = true;
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputEUR = true;
                                        }

                                        if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietNTK = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals(maNgoaiTeKhac.ToUpper())).FirstOrDefault();
                                            if (tiGiaChiTietNTK != null)
                                            {
                                                double? fTiGiaNTK = tiGiaChiTietNTK.fTiGia;
                                                if (fTiGiaNTK.HasValue)
                                                {
                                                    sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaNTK.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputNgoaiTe = true;
                                                }
                                            }
                                            else
                                            {
                                                isChangeInputNgoaiTe = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isChangeInputVND = true;
                                        isChangeInputEUR = true;
                                        isChangeInputNgoaiTe = true;
                                    }
                                    break;
                                case "VND":
                                    if (giaTriVNDInput.HasValue)
                                    {
                                        sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value).ToString(CultureInfo.InvariantCulture), 0);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputUSD = true;
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputEUR = true;
                                        }

                                        if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietNTK = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals(maNgoaiTeKhac.ToUpper())).FirstOrDefault();
                                            if (tiGiaChiTietNTK != null)
                                            {
                                                double? fTiGiaNTK = tiGiaChiTietNTK.fTiGia;
                                                if (fTiGiaNTK.HasValue)
                                                {
                                                    sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaNTK.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputNgoaiTe = true;
                                                }
                                            }
                                            else
                                            {
                                                isChangeInputNgoaiTe = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isChangeInputUSD = true;
                                        isChangeInputEUR = true;
                                        isChangeInputNgoaiTe = true;
                                    }
                                    break;
                                case "EUR":
                                    if (giaTriEURInput.HasValue)
                                    {
                                        sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputVND = true;
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
                                        }
                                        else
                                        {
                                            isChangeInputUSD = true;
                                        }

                                        if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietNTK = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals(maNgoaiTeKhac.ToUpper())).FirstOrDefault();
                                            if (tiGiaChiTietNTK != null)
                                            {
                                                double? fTiGiaNTK = tiGiaChiTietNTK.fTiGia;
                                                if (fTiGiaNTK.HasValue)
                                                {
                                                    sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaNTK.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputNgoaiTe = true;
                                                }
                                            }
                                            else
                                            {
                                                isChangeInputNgoaiTe = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isChangeInputVND = true;
                                        isChangeInputUSD = true;
                                        isChangeInputNgoaiTe = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            NH_DM_TiGia_ChiTiet tiGiaChiTiet = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals(maNgoaiTeKhac.ToUpper())).FirstOrDefault();
                            if (tiGiaChiTiet != null)
                            {
                                double? fTiGia = tiGiaChiTiet.fTiGia;
                                if (giaTriNTKhacInput.HasValue)
                                {
                                    if (fTiGia.HasValue)
                                    {
                                        sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriNTKhacInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        double fTienMaGoc = giaTriNTKhacInput.Value / fTiGia.Value;
                                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietTheoMaGocList = _qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                                        if (tiGiaChiTietTheoMaGocList != null)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                                            if (tiGiaChiTietUSD != null)
                                            {
                                                double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                                if (fTiGiaUSD.HasValue)
                                                {
                                                    sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputUSD = true;
                                                }
                                            }
                                            else
                                            {
                                                isChangeInputUSD = true;
                                            }

                                            NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                                            if (tiGiaChiTietVND != null)
                                            {
                                                double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                                if (fTiGiaVND.HasValue)
                                                {
                                                    sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                    isChangeInputVND = true;
                                                }
                                            }
                                            else 
                                            { 
                                                isChangeInputVND = false;
                                            }
                                            
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("EUR")).FirstOrDefault();
                                            if (tiGiaChiTietEUR != null)
                                            {
                                                double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                                if (fTiGiaEUR.HasValue)
                                                {
                                                    sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputEUR = true;
                                                }
                                            }
                                            else
                                            {
                                                isChangeInputEUR = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    isChangeInputUSD = true;
                                    isChangeInputVND = true;
                                    isChangeInputEUR = true;
                                    isChangeInputNgoaiTe = false;
                                }
                            }
                        }
                    }
                }
                return Json(new
                {
                    bIsComplete = true,
                    isChangeInputNgoaiTe = isChangeInputNgoaiTe,
                    isChangeInputUSD = isChangeInputUSD,
                    isChangeInputVND = isChangeInputVND,
                    isChangeInputEUR = isChangeInputEUR,
                    sGiaTriUSD = sGiaTriUSD,
                    sGiaTriVND = sGiaTriVND,
                    sGiaTriEUR = sGiaTriEUR,
                    sGiaTriNTKhac = sGiaTriNTKhac
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { bIsComplete = false, sMessError = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetHtmlTienteQuyDoi(List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList, string maTienGoc, Guid? idNgoaiTeKhac, string maNgoaiTeKhac)
        {
            StringBuilder htmlTienTe = new StringBuilder();
            NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
            if (tiGiaChiTietUSD != null && !maTienGoc.Equals("USD"))
            {
                double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                if (fTiGiaUSD.HasValue)
                {
                    htmlTienTe.AppendFormat("1 {0} = {1} USD; ", HttpUtility.HtmlEncode(maTienGoc), fTiGiaUSD.Value.ToString("#,##0." + new string('#', 339)));
                }
            }

            NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
            if (tiGiaChiTietVND != null && !maTienGoc.Equals("VND"))
            {
                double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                if (fTiGiaVND.HasValue)
                {
                    htmlTienTe.AppendFormat("1 {0} = {1} VND; ", HttpUtility.HtmlEncode(maTienGoc), fTiGiaVND.Value.ToString("#,##0." + new string('#', 339)));
                }
            }

            NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("EUR")).FirstOrDefault();
            if (tiGiaChiTietEUR != null && !maTienGoc.Equals("EUR"))
            {
                double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                if (fTiGiaEUR.HasValue)
                {
                    htmlTienTe.AppendFormat("1 {0} = {1} EUR; ", HttpUtility.HtmlEncode(maTienGoc), fTiGiaEUR.Value.ToString("#,##0." + new string('#', 339)));
                }
            }
            if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty && !string.IsNullOrEmpty(maNgoaiTeKhac) && !maTienGoc.ToUpper().Equals(maNgoaiTeKhac.ToUpper()))
            {
                NH_DM_TiGia_ChiTiet tiGiaChiTietNTK = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals(maNgoaiTeKhac.ToUpper())).FirstOrDefault();
                if (tiGiaChiTietNTK != null)
                {
                    double? fTiGiaNTK = tiGiaChiTietNTK.fTiGia;
                    if (fTiGiaNTK.HasValue)
                    {
                        htmlTienTe.AppendFormat("1 {0} = {1} {2}; ", HttpUtility.HtmlEncode(maTienGoc), fTiGiaNTK.Value.ToString("#,##0." + new string('#', 339)), HttpUtility.HtmlEncode(maNgoaiTeKhac));
                    }
                }
            }
            return htmlTienTe.ToString();
        }
    }
}