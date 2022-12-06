using ClosedXML.Excel;
using VIETTEL.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Areas.z.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.QLNH.Controllers.DuAnHopDong
{
    public class ThongTinGoiThauController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly List<string> MaTienTeList = new List<string>() { "USD", "VND", "EUR" };
        private readonly List<NH_DA_GoiThau_LoaiModel> loaiList = new List<NH_DA_GoiThau_LoaiModel>() {
            new NH_DA_GoiThau_LoaiModel()
            {
                Value = 0,
                Text = "--Chọn loại--"
            },
            new NH_DA_GoiThau_LoaiModel()
            {
                Value = 1,
                Text = "Dự án, Trong nước"
            },
            new NH_DA_GoiThau_LoaiModel()
            {
                Value = 2,
                Text = "Mua sắm, Trong nước"
            },
            new NH_DA_GoiThau_LoaiModel()
            {
                Value = 3,
                Text = "Dự án, Ngoại thương"
            },
            new NH_DA_GoiThau_LoaiModel()
            {
                Value = 4,
                Text = "Mua sắm, Ngoại thương"
            }
        };

        // GET: QLNH/ThongTinGoiThau
        public ActionResult Index()
        {
            NHDAGoiThauViewModel vm = new NHDAGoiThauViewModel();
            vm._paging.CurrentPage = 1;
            vm._paging.ItemsPerPage = 10;
            vm.Items = _qlnhService.GetAllNHThongTinGoiThau(ref vm._paging, null, null, null, null, null, null);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
            ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDADuAnList().ToList();
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

            ViewBag.ListLoai = loaiList.ToSelectList("Value", "Text");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetListGoiThauById(Guid id, string sTenGoiThau, Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, int? iLoai, int? iThoiGianThucHien)
        {
            sTenGoiThau = HttpUtility.HtmlDecode(sTenGoiThau);
            IEnumerable<NH_DA_GoiThauModel> nhiemVuChiList = _qlnhService.GetAllNHThongTinGoiThauById(id, sTenGoiThau
                , (iDonVi == Guid.Empty ? null : iDonVi)
                , (iChuongTrinh == Guid.Empty ? null : iChuongTrinh)
                , (iDuAn == Guid.Empty ? null : iDuAn)
                , (iLoai == null ? 0 : iLoai)
                , iThoiGianThucHien);
            StringBuilder htmlResult = new StringBuilder();
            int index = 1;
            foreach (NH_DA_GoiThauModel item in nhiemVuChiList)
            {
                htmlResult.AppendLine("<tr class='child-" + id + "' style='display:none'>");
                htmlResult.AppendLine("<td class='text-center'>" + index++ + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenGoiThau) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenDonVi) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenChuongTrinh) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sTenDuAn) + "</td>");
                htmlResult.AppendLine("<td class='text-center'>" + HttpUtility.HtmlEncode(item.sLoai) + "</td>");
                htmlResult.AppendLine("<td class='text-center'>" + HttpUtility.HtmlEncode(item.iThoiGianThucHien.ToString()) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriUSD.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriUSD.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriVND.HasValue ? CommonFunction.DinhDangSo(Math.Round(item.fGiaTriVND.Value).ToString(CultureInfo.InvariantCulture), 0) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriEUR.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriEUR.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-right'>" + HttpUtility.HtmlEncode(item.fGiaTriNgoaiTeKhac.HasValue ? CommonFunction.DinhDangSo(item.fGiaTriNgoaiTeKhac.Value.ToString(CultureInfo.InvariantCulture), 2) : string.Empty) + "</td>");
                htmlResult.AppendLine("<td class='text-center'>" + HttpUtility.HtmlEncode(item.sSoLanDieuChinh) + "</td>");
                htmlResult.AppendLine("<td class='text-left'>" + HttpUtility.HtmlEncode(item.sDieuChinhTu) + "</td>");
                if (item.bIsActive)
                {
                    htmlResult.AppendLine("<td class='text-center' style='padding:0;'>"
                        + "<button type='button' class='btn-adjust' title='Điều chỉnh' onclick=\"OpenPackageInfo('" + item.ID + "', true);\"><i class='fa fa-copy fa-lg' aria-hidden='true'></i></button>"
                        + "<button type='button' class='btn-detail' title='Xem chi tiết' onclick=\"OpenPackageInfoDetail('" + item.ID + "');\"><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>"
                        + "<button type='button' class='btn-edit' title='Sửa' onclick=\"OpenPackageInfo('" + item.ID + "', false);\"><i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i></button>"
                        + "<button type='button' class='btn-delete' title='Xóa' onclick=\"Xoa('" + item.ID + "');\"><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>"
                        + "</td>");
                }
                else
                {
                    htmlResult.AppendLine("<td class='text-center' style='padding:0;'><button type='button' class='btn-detail' title='Xem chi tiết' onclick=\"OpenPackageInfoDetail('" + item.ID + "');\"><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button></td>");
                }
                htmlResult.AppendLine("</tr>");
            }
            return Json(new { datas = htmlResult.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sTenGoiThau, string maDonVi, Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, int? iLoai, int? iThoiGianThucHien)
        {
            NHDAGoiThauViewModel vm = new NHDAGoiThauViewModel();
            sTenGoiThau = HttpUtility.HtmlDecode(sTenGoiThau ?? string.Empty);
            vm._paging = _paging;
            vm._paging.ItemsPerPage = 10;
            vm.Items = _qlnhService.GetAllNHThongTinGoiThau(ref vm._paging, sTenGoiThau
                , (iDonVi == Guid.Empty ? null : iDonVi)
                , (iChuongTrinh == Guid.Empty ? null : iChuongTrinh)
                , (iDuAn == Guid.Empty ? null : iDuAn)
                , (iLoai == null ? 0 : iLoai)
                , iThoiGianThucHien);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHNhiemVuChiTietTheoDonViId(HttpUtility.HtmlDecode(maDonVi), iDonVi).ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
            ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDuAnTheoKHCTBQPChuongTrinhId(iChuongTrinh).ToList();
            lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

            ViewBag.ListLoai = loaiList.ToSelectList("Value", "Text");

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
            return Json(new { htmlCT = htmlChuongtrinh.ToString(), htmlDA = htmlDA.ToString() }, JsonRequestBehavior.AllowGet);
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
                    htmlDA.AppendFormat("<option value='{0}'>{1}</option>", lstDuAn[i].ID, HttpUtility.HtmlEncode(lstDuAn[i].sTenDuAn));
                }
            }
            return Json(htmlDA.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNhaThauTheoLoai(int? iLoai)
        {
            List<NH_DM_NhaThau> lstNhaThau = _qlnhService.GetNHDMNhaThauList(null, iLoai).ToList();
            StringBuilder htmlNT = new StringBuilder();
            htmlNT.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, GetOptionDefaultNhaThau(iLoai));
            if (lstNhaThau != null && lstNhaThau.Count() > 0)
            {
                for (int i = 0; i < lstNhaThau.Count; i++)
                {
                    htmlNT.AppendFormat("<option value='{0}'>{1}</option>", lstNhaThau[i].Id, HttpUtility.HtmlEncode(lstNhaThau[i].sTenNhaThau));
                }
            }
            return Json(htmlNT.ToString(), JsonRequestBehavior.AllowGet);
        }

        private string GetOptionDefaultNhaThau(int? iLoai)
        {
            string sOptionDefault = "--Chọn nhà thầu/đơn vị ủy thác--";
            if (iLoai != null && iLoai.HasValue)
            {
                switch (iLoai.Value)
                {
                    case 1:
                    case 2:
                        sOptionDefault = "--Chọn nhà thầu--";
                        break;
                    case 3:
                    case 4:
                        sOptionDefault = "--Chọn đơn vị ủy thác--";
                        break;
                    default:
                        break;
                }
            }
            return sOptionDefault;
        }

        public ActionResult GetPackageInfo(Guid? id, bool? isDieuChinh)
        {
            NH_DA_GoiThauModel data = new NH_DA_GoiThauModel();
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh;
            List<NH_DA_DuAn> lstDuAn;
            List<NH_DM_LoaiHopDong> lstLoaiHD;
            List<NH_DM_NhaThau> lstNhaThau;
            List<NH_DM_TiGia> lstTiGia;
            List<NH_DM_TiGia_ChiTiet> lstTiGiaChiTiet;
            List<NH_DM_LoaiTienTe> lstTienTe;
            List<NH_DM_HinhThucChonNhaThau> lstHinhThucCNT;
            List<NH_DM_PhuongThucChonNhaThau> lstPhuongThucCNT;

            ViewBag.IsDieuChinh = isDieuChinh != null ? isDieuChinh : false;

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;
            data.sThanhToanBang = data.sThanhToanBang ?? string.Empty;
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinGoiThauById(id.Value);
                if (data != null)
                {
                    if (data.iID_TiGiaID != null)
                    {
                        NH_DM_TiGia tiGia = _qlnhService.GetNHDMTiGiaList(data.iID_TiGiaID).ToList().FirstOrDefault();
                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = _qlnhService.GetNHDMTiGiaChiTietList(data.iID_TiGiaID, false).ToList();
                        ViewBag.HtmlTienTe = GetHtmlTienteQuyDoi(tiGiaChiTietList, tiGia.sMaTienTeGoc, data.iID_TiGia_ChiTietID, data.sMaNgoaiTeKhac);
                    }

                    lstChuongTrinh = _qlnhService.GetNHNhiemVuChiTietTheoDonViId(data.iID_MaDonVi, data.iID_DonViID).ToList();
                    lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
                    ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi", data.iID_KHCTBQP_ChuongTrinhID.ToString());

                    lstDuAn = _qlnhService.GetNHDuAnTheoKHCTBQPChuongTrinhId(data.iID_KHCTBQP_ChuongTrinhID).ToList();
                    lstDuAn.Insert(0, new NH_DA_DuAn { ID = Guid.Empty, sTenDuAn = "--Chọn dự án--" });
                    ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn", data.iID_DuAnID.ToString());

                    lstLoaiHD = _qlnhService.GetNHDMLoaiHopDongList().ToList();
                    lstLoaiHD.Insert(0, new NH_DM_LoaiHopDong { ID = Guid.Empty, sTenLoaiHopDong = "--Chọn loại hợp đồng--" });
                    ViewBag.ListLoaiHopDong = lstLoaiHD.ToSelectList("ID", "sTenLoaiHopDong", data.iID_LoaiHopDongID.ToString());

                    lstNhaThau = _qlnhService.GetNHDMNhaThauList(null, data.iPhanLoai).ToList();
                    lstNhaThau.Insert(0, new NH_DM_NhaThau { Id = Guid.Empty, sTenNhaThau = GetOptionDefaultNhaThau(data.iPhanLoai) });
                    ViewBag.ListNhaThau = lstNhaThau.ToSelectList("Id", "sTenNhaThau", data.iID_NhaThauThucHienID.ToString());

                    lstTiGia = _qlnhService.GetNHDMTiGiaList().ToList();
                    lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
                    ViewBag.ListTiGia = lstTiGia;

                    lstTienTe = _qlnhService.GetNHDMLoaiTienTeByCode(null).ToList();
                    lstTienTe.Insert(0, new NH_DM_LoaiTienTe { sMaTienTe = ""});
                    ViewBag.ListTiente = lstTienTe;

                    lstHinhThucCNT = _qlnhService.GetNHDMHinhThucChonNhaThauList().ToList();
                    lstHinhThucCNT.Insert(0, new NH_DM_HinhThucChonNhaThau { ID = Guid.Empty, sTenHinhThuc = "--Chọn hình thức--" });
                    ViewBag.ListHinhThucCNT = lstHinhThucCNT.ToSelectList("ID", "sTenHinhThuc", data.iID_HinhThucChonNhaThauID.ToString());

                    lstPhuongThucCNT = _qlnhService.GetNHDMPhuongThucChonNhaThauList().ToList();
                    lstPhuongThucCNT.Insert(0, new NH_DM_PhuongThucChonNhaThau { ID = Guid.Empty, sTenPhuongThuc = "--Chọn phương thức--" });
                    ViewBag.ListPhuongThucCNT = lstPhuongThucCNT.ToSelectList("ID", "sTenPhuongThuc", data.iID_PhuongThucChonNhaThauID.ToString());

                    lstTiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTietList(data.iID_TiGiaID).ToList();
                    lstTiGiaChiTiet.Insert(0, new NH_DM_TiGia_ChiTiet { ID = Guid.Empty, sMaTienTeQuyDoi = "--Chọn mã ngoại tệ khác--" });
                    ViewBag.ListTiGiaChiTiet = lstTiGiaChiTiet.ToSelectList("ID", "sMaTienTeQuyDoi", data.iID_TiGia_ChiTietID.ToString());
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
                ViewBag.ListDuAn = lstDuAn.ToSelectList("ID", "sTenDuAn");

                lstLoaiHD = _qlnhService.GetNHDMLoaiHopDongList().ToList();
                lstLoaiHD.Insert(0, new NH_DM_LoaiHopDong { ID = Guid.Empty, sTenLoaiHopDong = "--Chọn loại hợp đồng--" });
                ViewBag.ListLoaiHopDong = lstLoaiHD.ToSelectList("ID", "sTenLoaiHopDong");

                lstNhaThau = _qlnhService.GetNHDMNhaThauList().ToList();
                lstNhaThau.Insert(0, new NH_DM_NhaThau { Id = Guid.Empty, sTenNhaThau = "--Chọn nhà thầu/đơn vị ủy thác--" });
                ViewBag.ListNhaThau = lstNhaThau.ToSelectList("Id", "sTenNhaThau");

                lstTiGia = _qlnhService.GetNHDMTiGiaList().ToList();
                lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
                ViewBag.ListTiGia = lstTiGia;

                lstTienTe = _qlnhService.GetNHDMLoaiTienTeByCode(null).ToList();
                lstTienTe.Insert(0, new NH_DM_LoaiTienTe { sMaTienTe = "" });
                ViewBag.ListTiente = lstTienTe;

                lstHinhThucCNT = _qlnhService.GetNHDMHinhThucChonNhaThauList().ToList();
                lstHinhThucCNT.Insert(0, new NH_DM_HinhThucChonNhaThau { ID = Guid.Empty, sTenHinhThuc = "--Chọn hình thức--" });
                ViewBag.ListHinhThucCNT = lstHinhThucCNT.ToSelectList("ID", "sTenHinhThuc");

                lstPhuongThucCNT = _qlnhService.GetNHDMPhuongThucChonNhaThauList().ToList();
                lstPhuongThucCNT.Insert(0, new NH_DM_PhuongThucChonNhaThau { ID = Guid.Empty, sTenPhuongThuc = "--Chọn phương thức--" });
                ViewBag.ListPhuongThucCNT = lstPhuongThucCNT.ToSelectList("ID", "sTenPhuongThuc");

                lstTiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTietList(null).ToList();
                lstTiGiaChiTiet.Insert(0, new NH_DM_TiGia_ChiTiet { ID = Guid.Empty, sMaTienTeQuyDoi = "--Chọn mã ngoại tệ khác--" });
                ViewBag.ListTiGiaChiTiet = lstTiGiaChiTiet.ToSelectList("ID", "sMaTienTeQuyDoi");
            }

            return View("Update", data);
        }

        public ActionResult GetPackageInfoDetail(Guid? id)
        {
            NH_DA_GoiThauModel data = new NH_DA_GoiThauModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinGoiThauById(id.Value);

                if (data == null)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View("Detail", data);
        }

        [HttpPost]
        public JsonResult Save(NH_DA_GoiThau data, NH_DA_GoiThau_TienTeModel giaTriTienData, bool isDieuChinh)
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

            data.sTenGoiThau = HttpUtility.HtmlDecode(data.sTenGoiThau ?? string.Empty);
            data.sMaNgoaiTeKhac = HttpUtility.HtmlDecode(data.sMaNgoaiTeKhac ?? string.Empty);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi ?? string.Empty);
            data.sSoKeHoachLCNT = HttpUtility.HtmlDecode(data.sSoKeHoachLCNT ?? string.Empty);
            data.sSoKetQuaLCNT = HttpUtility.HtmlDecode(data.sSoKetQuaLCNT ?? string.Empty);
            data.sSoPANK = HttpUtility.HtmlDecode(data.sSoPANK ?? string.Empty);
            data.sSoKetQuaDamPhan = HttpUtility.HtmlDecode(data.sSoKetQuaDamPhan ?? string.Empty);

            if (!_qlnhService.SaveThongTinGoiThau(data, isDieuChinh, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            if (!_qlnhService.DeleteThongTinGoiThau(Guid.Parse(id)))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeTiGia(Guid? idTiGia, NH_DA_GoiThau_TienTeModel giaTriTienData)
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
                return Json(new
                {
                    bIsComplete = true,
                    htmlMNTK = htmlMNTK.ToString(),
                    htmlTienTe = htmlTienTe
                    ,
                    isChangeInputUSD = isChangeInputUSD,
                    isChangeInputVND = isChangeInputVND,
                    isChangeInputEUR = isChangeInputEUR
                    ,
                    sGiaTriUSD = sGiaTriUSD,
                    sGiaTriVND = sGiaTriVND,
                    sGiaTriEUR = sGiaTriEUR
                    ,
                    isReadonlyTxtMaNTKhac = isReadonlyTxtMaNTKhac
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { bIsComplete = false, sMessError = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ChangeTiGiaNgoaiTeKhac(Guid? idTiGia, Guid? idNgoaiTeKhac, string maNgoaiTeKhac, NH_DA_GoiThau_TienTeModel giaTriTienData)
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
        public JsonResult ChangeGiaTien(Guid? idTiGia, Guid? idNgoaiTeKhac, string maNgoaiTeKhac, string txtBlur, NH_DA_GoiThau_TienTeModel giaTriTienData)
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
                                                isChangeInputVND = true;
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

        [HttpPost]
        public ActionResult OpenModalImport()
        {
            return PartialView("_modalImport");
        }

        [HttpPost]
        public JsonResult SaveImport(List<NH_DA_GoiThau> packageList)
        {
            if (packageList == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không import được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            foreach (var contract in packageList)
            {
                contract.sTenGoiThau = HttpUtility.HtmlDecode(contract.sTenGoiThau);
                contract.iID_MaDonVi = HttpUtility.HtmlDecode(contract.iID_MaDonVi);
                contract.dNgayTao = DateTime.Now;
                contract.sNguoiTao = Username;
                contract.bIsActive = true;
                contract.bIsGoc = true;
            }

            if (!_qlnhService.SaveImportThongTinGoiThau(packageList))
            {
                return Json(new { bIsComplete = false, sMessError = "Không import được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadDataExcel(HttpPostedFileBase file)
        {
            string data;
            try
            {
                byte[] file_data = GetBytes(file);
                DataTable dataTable = ExcelHelpers.LoadExcelDataTable(file_data);
                IEnumerable<NH_DA_GoiThau_ImportModel> dataImport = GetExcelResult(dataTable);
                data = GetHtmlDataExcel(dataImport);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { sMessError = "Không thể tải dữ liệu từ file đã chọn!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<NH_DA_GoiThau_ImportModel> GetExcelResult(DataTable dt)
        {
            List<NH_DA_GoiThau_ImportModel> dataImportList = new List<NH_DA_GoiThau_ImportModel>();
            NH_DA_GoiThau_ImportModel data = new NH_DA_GoiThau_ImportModel();
            DataRow row;

            string sSTT = string.Empty;
            string sTenGoiThau = string.Empty;
            string sMaLoai = string.Empty;
            string sTenLoai = string.Empty;
            string sSoQuyetDinh1 = string.Empty;
            string sSoQuyetDinh2 = string.Empty;
            string sNgayQuyetDinh1 = string.Empty;
            string sNgayQuyetDinh2 = string.Empty;
            string sMaLoaiHopDong = string.Empty;
            string sMaNhaThau = string.Empty;
            string sMaTienTe = string.Empty;
            string sMaHinhThucCNT = string.Empty;
            string sMaPhuongThucCNT = string.Empty;
            string sThoiGianThucHien = string.Empty;
            DateTime? dNgayQuyetDinh1;
            DateTime? dNgayQuyetDinh2;
            int? thoiGianThucHien;
            int? loai;
            Guid? iID_LoaiHopDong = null;
            Guid? iID_NhaThau = null;
            Guid? iID_HinhThucChonNhaThauID = null;
            Guid? iID_PhuongThucChonNhaThauID = null;

            StringBuilder sErrorMessage = new StringBuilder();
            bool isDataWrong = false;
            bool IsTenGoiThauWrong = false;
            bool IsMaLoaiWrong = false;
            bool IsSoQuyetDinh1Wrong = false;
            bool IsSoQuyetDinh2Wrong = false;
            bool IsNgayQuyetDinh1Wrong = false;
            bool IsNgayQuyetDinh2Wrong = false;
            bool IsMaLoaiHopDongWrong = false;
            bool IsMaNhaThauWrong = false;
            bool IsMaTienTeWrong = false;
            bool IsMaHinhThucCNTWrong = false;
            bool IsMaPhuongThucCNTWrong = false;
            bool IsThoiGianThucHienWrong = false;

            NH_DM_LoaiHopDong dmLoaiHopDong;
            NH_DM_NhaThau dmNhaThau;
            NH_DM_HinhThucChonNhaThau dmHinhThucCNT;
            NH_DM_PhuongThucChonNhaThau dmPhuongThucCNT;
            NH_DM_LoaiTienTe dmLoaiTienTe;
            IEnumerable<NH_DM_LoaiHopDong> loaiHopDongList = _qlnhService.GetNHDMLoaiHopDongList();
            IEnumerable<NH_DM_NhaThau> nhaThauList = _qlnhService.GetNHDMNhaThauList();
            IEnumerable<NH_DM_HinhThucChonNhaThau> hinhThucCNTList = _qlnhService.GetNHDMHinhThucChonNhaThauList();
            IEnumerable<NH_DM_PhuongThucChonNhaThau> phuongThucCNTList = _qlnhService.GetNHDMPhuongThucChonNhaThauList();
            IEnumerable<NH_DM_LoaiTienTe> loaiTienTeList = _qlnhService.GetNHDMLoaiTienTeList();

            var items = dt.AsEnumerable();
            for (var i = 17; i < items.Count(); i++)
            {
                isDataWrong = false;
                IsTenGoiThauWrong = false;
                IsMaLoaiWrong = false;
                IsSoQuyetDinh1Wrong = false;
                IsSoQuyetDinh2Wrong = false;
                IsNgayQuyetDinh1Wrong = false;
                IsNgayQuyetDinh2Wrong = false;
                IsMaLoaiHopDongWrong = false;
                IsMaNhaThauWrong = false;
                IsMaTienTeWrong = false;
                IsMaHinhThucCNTWrong = false;
                IsMaPhuongThucCNTWrong = false;
                IsThoiGianThucHienWrong = false;
                sErrorMessage.Clear();
                row = items.ToList()[i];

                sSTT = row.Field<string>(0);
                sMaLoai = !string.IsNullOrEmpty(row.Field<string>(1)) ? row.Field<string>(1).Trim() : string.Empty;
                loai = CommonFunction.TryParseInt(sMaLoai);
                if (!string.IsNullOrEmpty(sMaLoai))
                {
                    if (loai == null || loaiList.FirstOrDefault(x => loai.HasValue && x.Value == loai.Value) == null)
                    {
                        isDataWrong = true;
                        IsMaLoaiWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã loại không tồn tại.");
                    }
                    else
                    {
                        sTenLoai = loaiList.FirstOrDefault(x => loai.HasValue && x.Value == loai.Value).Text;
                    }
                }
                else
                {
                    isDataWrong = true;
                    IsMaLoaiWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã loại chưa được chọn.");
                }

                sSoQuyetDinh1 = !string.IsNullOrEmpty(row.Field<string>(2)) ? row.Field<string>(2).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sSoQuyetDinh1) && sSoQuyetDinh1.Length > 100)
                {
                    isDataWrong = true;
                    IsSoQuyetDinh1Wrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu quá 100 kí tự.");
                }

                sNgayQuyetDinh1 = !string.IsNullOrEmpty(row.Field<string>(3)) ? row.Field<string>(3).Trim() : string.Empty;
                dNgayQuyetDinh1 = CommonFunction.TryParseDateTime(sNgayQuyetDinh1);
                if (!string.IsNullOrEmpty(sNgayQuyetDinh1))
                {
                    if (!dNgayQuyetDinh1.HasValue)
                    {
                        isDataWrong = true;
                        IsNgayQuyetDinh1Wrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Ngày quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu không hợp lệ.");
                    }
                    else
                    {
                        sNgayQuyetDinh1 = dNgayQuyetDinh1.Value.ToString("dd/MM/yyyy");
                    }
                }

                sSoQuyetDinh2 = !string.IsNullOrEmpty(row.Field<string>(4)) ? row.Field<string>(4).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sSoQuyetDinh2) && sSoQuyetDinh2.Length > 100)
                {
                    isDataWrong = true;
                    IsSoQuyetDinh2Wrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán quá 100 kí tự.");
                }

                sNgayQuyetDinh2 = !string.IsNullOrEmpty(row.Field<string>(5)) ? row.Field<string>(5).Trim() : string.Empty;
                dNgayQuyetDinh2 = CommonFunction.TryParseDateTime(sNgayQuyetDinh2);
                if (!string.IsNullOrEmpty(sNgayQuyetDinh2))
                {
                    if (!dNgayQuyetDinh2.HasValue)
                    {
                        isDataWrong = true;
                        IsNgayQuyetDinh2Wrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Ngày quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán không hợp lệ.");
                    }
                    else
                    {
                        sNgayQuyetDinh2 = dNgayQuyetDinh2.Value.ToString("dd/MM/yyyy");
                    }
                }

                sTenGoiThau = !string.IsNullOrEmpty(row.Field<string>(6)) ? row.Field<string>(6).Trim() : string.Empty;
                if (string.IsNullOrEmpty(sTenGoiThau))
                {
                    isDataWrong = true;
                    IsTenGoiThauWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Tên gói thầu chưa được nhập.");
                }
                if (!string.IsNullOrEmpty(sTenGoiThau) && sTenGoiThau.Length > 300)
                {
                    isDataWrong = true;
                    IsTenGoiThauWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Tên gói thầu quá 100 kí tự.");
                }

                sThoiGianThucHien = !string.IsNullOrEmpty(row.Field<string>(7)) ? row.Field<string>(7).Trim() : string.Empty;
                thoiGianThucHien = CommonFunction.TryParseInt(sThoiGianThucHien);
                if (!string.IsNullOrEmpty(sThoiGianThucHien) && thoiGianThucHien == null)
                {
                    isDataWrong = true;
                    IsThoiGianThucHienWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Thời gian thực hiện không hợp lệ.");
                }

                sMaNhaThau = !string.IsNullOrEmpty(row.Field<string>(8)) ? row.Field<string>(8).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sMaNhaThau))
                {
                    if (loai.HasValue && loaiList.FirstOrDefault(x => x.Value == loai.Value && loai.Value != 0) != null)
                    {
                        dmNhaThau = nhaThauList.Where(x => x.sMaNhaThau.Equals(sMaNhaThau) && x.iLoai.HasValue && x.iLoai == loai.Value).FirstOrDefault();
                        if (dmNhaThau == null)
                        {
                            isDataWrong = true;
                            IsMaNhaThauWrong = true;
                            sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã nhà thầu/đơn vị ủy thác đã nhập không tồn tại.");
                        }
                        else
                        {
                            iID_NhaThau = dmNhaThau.Id;
                        }
                    }
                    else
                    {
                        isDataWrong = true;
                        IsMaNhaThauWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã nhà thầu/đơn vị ủy thác không tồn tại.");
                    }
                }

                sMaTienTe = !string.IsNullOrEmpty(row.Field<string>(9)) ? row.Field<string>(9).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sMaTienTe))
                {
                    dmLoaiTienTe = loaiTienTeList.Where(x => x.sMaTienTe.Equals(sMaTienTe)).FirstOrDefault();
                    if (dmLoaiTienTe == null)
                    {
                        isDataWrong = true;
                        IsMaTienTeWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã tiền tệ thanh toán không tồn tại.");
                    }
                }

                sMaHinhThucCNT = !string.IsNullOrEmpty(row.Field<string>(10)) ? row.Field<string>(10).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sMaHinhThucCNT))
                {
                    dmHinhThucCNT = hinhThucCNTList.Where(x => x.sMaHinhThuc.Equals(sMaHinhThucCNT)).FirstOrDefault();
                    if (dmHinhThucCNT == null)
                    {
                        isDataWrong = true;
                        IsMaHinhThucCNTWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã hình thức chọn nhà thầu không tồn tại.");
                    }
                    else
                    {
                        iID_HinhThucChonNhaThauID = dmHinhThucCNT.ID;
                    }
                }

                sMaPhuongThucCNT = !string.IsNullOrEmpty(row.Field<string>(11)) ? row.Field<string>(11).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sMaPhuongThucCNT))
                {
                    dmPhuongThucCNT = phuongThucCNTList.Where(x => x.sMaPhuongThuc.Equals(sMaPhuongThucCNT)).FirstOrDefault();
                    if (dmPhuongThucCNT == null)
                    {
                        isDataWrong = true;
                        IsMaPhuongThucCNTWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã phương thức chọn nhà thầu không tồn tại.");
                    }
                    else
                    {
                        iID_PhuongThucChonNhaThauID = dmPhuongThucCNT.ID;
                    }
                }

                sMaLoaiHopDong = !string.IsNullOrEmpty(row.Field<string>(12)) ? row.Field<string>(12).Trim() : string.Empty;
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

                data = new NH_DA_GoiThau_ImportModel
                {
                    sSTT = sSTT,
                    sMaLoai = sMaLoai,
                    sTenLoai = sTenLoai,
                    loai = loai,
                    IsMaLoaiWrong = IsMaLoaiWrong,
                    sSoQuyetDinh1 = sSoQuyetDinh1,
                    IsSoQuyetDinh1Wrong = IsSoQuyetDinh1Wrong,
                    sNgayQuyetDinh1 = sNgayQuyetDinh1,
                    IsNgayQuyetDinh1Wrong = IsNgayQuyetDinh1Wrong,
                    sSoQuyetDinh2 = sSoQuyetDinh2,
                    IsSoQuyetDinh2Wrong = IsSoQuyetDinh2Wrong,
                    sNgayQuyetDinh2 = sNgayQuyetDinh2,
                    IsNgayQuyetDinh2Wrong = IsNgayQuyetDinh2Wrong,
                    sTenGoiThau = sTenGoiThau,
                    IsTenGoiThauWrong = IsTenGoiThauWrong,
                    sThoiGianThucHien = sThoiGianThucHien,
                    IsThoiGianThucHienWrong = IsThoiGianThucHienWrong,
                    sMaNhaThau = sMaNhaThau,
                    IsMaNhaThauWrong = IsMaNhaThauWrong,
                    iID_NhaThau = iID_NhaThau,
                    sMaTienTe = sMaTienTe,
                    IsMaTienTeWrong = IsMaTienTeWrong,
                    sMaHinhThucCNT = sMaHinhThucCNT,
                    IsMaHinhThucCNTWrong = IsMaHinhThucCNTWrong,
                    iID_HinhThucChonNhaThauID = iID_HinhThucChonNhaThauID,
                    sMaPhuongThucCNT = sMaPhuongThucCNT,
                    IsMaPhuongThucCNTWrong= IsMaPhuongThucCNTWrong,
                    iID_PhuongThucChonNhaThauID= iID_PhuongThucChonNhaThauID,
                    sMaLoaiHopDong = sMaLoaiHopDong,
                    IsMaLoaiHopDongWrong = IsMaLoaiHopDongWrong,
                    iID_LoaiHopDong = iID_LoaiHopDong,
                    sErrorMessage = sErrorMessage.ToString(),
                    IsDataWrong = isDataWrong
                };

                dataImportList.Add(data);
            }
            return dataImportList.AsEnumerable();
        }

        public ActionResult DownloadTemplateImport()
        {
            try
            {
                XLWorkbook w_b = new XLWorkbook();
                var wbContractInfo = w_b.Worksheets.Add("Biểu mẫu thông tin gói thầu");
                var wbLoai = w_b.Worksheets.Add("Loại");
                var wbNhaThau = w_b.Worksheets.Add("Nhà thầu - Đơn vị ủy thác");
                var wbTienTe = w_b.Worksheets.Add("Đơn vị tiền tệ");
                var wbHinhThucCNT = w_b.Worksheets.Add("Hình thức chọn nhà thầu");
                var wbPhuongThucCNT = w_b.Worksheets.Add("Phương thức chọn nhà thầu");
                var wbLoaiHopDong = w_b.Worksheets.Add("Loại hợp đồng");
                wbContractInfo.Column(1).Width = 15;
                wbContractInfo.Column(2).Width = 15;
                wbContractInfo.Column(3).Width = 40;
                wbContractInfo.Column(4).Width = 25;
                wbContractInfo.Column(5).Width = 40;
                wbContractInfo.Column(6).Width = 25;
                wbContractInfo.Column(7).Width = 40;
                wbContractInfo.Column(8).Width = 15;
                wbContractInfo.Column(9).Width = 25;
                wbContractInfo.Column(10).Width = 25;
                wbContractInfo.Column(11).Width = 25;
                wbContractInfo.Column(12).Width = 25;
                wbContractInfo.Column(13).Width = 25;

                wbLoai.Column(1).Width = 15;
                wbLoai.Column(2).Width = 20;
                wbLoai.Column(3).Width = 30;

                wbTienTe.Column(1).Width = 15;
                wbTienTe.Column(2).Width = 40;
                wbTienTe.Column(3).Width = 40;
                wbTienTe.Column(4).Width = 50;

                wbLoaiHopDong.Column(1).Width = 15;
                wbLoaiHopDong.Column(2).Width = 40;
                wbLoaiHopDong.Column(3).Width = 40;
                wbLoaiHopDong.Column(4).Width = 50;
                wbLoaiHopDong.Column(5).Width = 60;

                wbHinhThucCNT.Column(1).Width = 15;
                wbHinhThucCNT.Column(2).Width = 40;
                wbHinhThucCNT.Column(3).Width = 40;
                wbHinhThucCNT.Column(4).Width = 50;
                wbHinhThucCNT.Column(5).Width = 60;

                wbPhuongThucCNT.Column(1).Width = 15;
                wbPhuongThucCNT.Column(2).Width = 40;
                wbPhuongThucCNT.Column(3).Width = 40;
                wbPhuongThucCNT.Column(4).Width = 50;
                wbPhuongThucCNT.Column(5).Width = 60;

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
                wbContractInfo.Row(17).Height = 55;
                wbContractInfo.Cell(2, 1).Value = "BIỂU MẪU IMPORT THÔNG TIN GÓI THẦU";
                wbContractInfo.Range(2, 1, 2, 9).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbContractInfo.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true);
                wbContractInfo.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("0");
                wbContractInfo.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");
                wbContractInfo.Column(13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetWrapText(true).NumberFormat.SetFormat("@");

                wbContractInfo.Cell(4, 1).Value = "Loại: Chỉ nhập mã loại đã có trong sheet Loại";
                wbContractInfo.Cell(4, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(5, 1).Value = "Số quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu: Nhập số, chữ, tối đa 100 ký tự";
                wbContractInfo.Cell(5, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(6, 1).Value = "Ngày quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu: Chỉ nhập dạng dd/mm/yyyy";
                wbContractInfo.Cell(6, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(7, 1).Value = "Số quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán: Nhập số, chữ, tối đa 100 ký tự";
                wbContractInfo.Cell(7, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(8, 1).Value = "Ngày quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán: Chỉ nhập dạng dd/mm/yyyy";
                wbContractInfo.Cell(8, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(9, 1).Value = "Tên gói thầu: Nhập số, chữ, tối đa 300 ký tự";
                wbContractInfo.Cell(9, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(10, 1).Value = "Thời gian thực hiện: Nhập số";
                wbContractInfo.Cell(10, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(11, 1).Value = "Nhà thầu/Đơn vị ủy thác: Chỉ nhập mã nhà thầu/ đơn vị ủy thác đã có trong sheet Nhà thầu/Đơn vị ủy thác";
                wbContractInfo.Cell(11, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(12, 1).Value = "Thanh toán bằng: Chỉ nhập mã đơn vị tiền tệ đã có trong sheet Đơn vị tiền tệ";
                wbContractInfo.Cell(12, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(13, 1).Value = "Hình thức chọn nhà thầu: Chỉ nhập mã hình thức chọn nhà thầu đã có trong sheet Hình thức chọn nhà thầu";
                wbContractInfo.Cell(13, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(14, 1).Value = "Phương thức chọn nhà thầu: Chỉ nhập mã phương thức chọn nhà thầu đã có trong sheet Phương thức chọn nhà thầu";
                wbContractInfo.Cell(14, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(15, 1).Value = "Loại hợp đồng: Chỉ nhập mã loại hợp đồng đã có trong sheet Loại hợp đồng";
                wbContractInfo.Cell(15, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);

                wbContractInfo.Cell(17, 1).Value = "STT";
                wbContractInfo.Cell(17, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 2).Value = "Loại";
                wbContractInfo.Cell(17, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 3).Value = "Số quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu";
                wbContractInfo.Cell(17, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 4).Value = "Ngày quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu";
                wbContractInfo.Cell(17, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 5).Value = "Số quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán";
                wbContractInfo.Cell(17, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 6).Value = "Ngày quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán";
                wbContractInfo.Cell(17, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 7).Value = "Tên gói thầu";
                wbContractInfo.Cell(17, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 8).Value = "Thời gian thực hiện (ngày)";
                wbContractInfo.Cell(17, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 9).Value = "Nhà thầu/Đơn vị ủy thác";
                wbContractInfo.Cell(17, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 10).Value = "Thanh toán bằng";
                wbContractInfo.Cell(17, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 11).Value = "Hình thức chọn nhà thầu";
                wbContractInfo.Cell(17, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 12).Value = "Phương thức chọn nhà thầu";
                wbContractInfo.Cell(17, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                wbContractInfo.Cell(17, 13).Value = "Loại hợp đồng";
                wbContractInfo.Cell(17, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                //Sheet Loại
                wbLoai.Style.Font.FontName = "Times New Roman";
                wbLoai.Style.Font.FontSize = 13;
                wbLoai.PageSetup.FitToPages(1, 1);
                wbLoai.Row(4).Height = 30;
                wbLoai.Cell(2, 1).Value = "DANH MỤC LOẠI";
                wbLoai.Range(2, 1, 2, 3).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbLoai.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbLoai.Cell(4, 1).Value = "STT";
                wbLoai.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbLoai.Cell(4, 2).Value = "Mã loại";
                wbLoai.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbLoai.Cell(4, 3).Value = "Tên loại";
                wbLoai.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                int i = 5;
                int j = 1;
                foreach (NH_DA_GoiThau_LoaiModel item in loaiList.Where(x => x.Value != 0))
                {
                    wbLoai.Cell(i, 1).Value = j;
                    wbLoai.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbLoai.Cell(i, 2).Value = item.Value;
                    wbLoai.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbLoai.Cell(i, 3).Value = item.Text;
                    wbLoai.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }

                //Sheet Đơn vị tiền tệ
                wbTienTe.Style.Font.FontName = "Times New Roman";
                wbTienTe.Style.Font.FontSize = 13;
                wbTienTe.PageSetup.FitToPages(1, 1);
                wbTienTe.Row(4).Height = 30;
                wbTienTe.Cell(2, 1).Value = "DANH MỤC ĐƠN VỊ TIỀN TỆ";
                wbTienTe.Range(2, 1, 2, 4).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbTienTe.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbTienTe.Cell(4, 1).Value = "STT";
                wbTienTe.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbTienTe.Cell(4, 2).Value = "Mã tiền tệ";
                wbTienTe.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbTienTe.Cell(4, 3).Value = "Tên tiền tệ";
                wbTienTe.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbTienTe.Cell(4, 4).Value = "Mô tả chi tiết";
                wbTienTe.Cell(4, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                IEnumerable<NH_DM_LoaiTienTe> tienTeList = _qlnhService.GetNHDMLoaiTienTeList();
                i = 5;
                j = 1;
                foreach (NH_DM_LoaiTienTe item in tienTeList)
                {
                    wbTienTe.Cell(i, 1).Value = j;
                    wbTienTe.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbTienTe.Cell(i, 2).Value = item.sMaTienTe;
                    wbTienTe.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbTienTe.Cell(i, 3).Value = item.sTenTienTe;
                    wbTienTe.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbTienTe.Cell(i, 4).Value = item.sMoTaChiTiet;
                    wbTienTe.Cell(i, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }

                //Sheet Hình thức chọn nhà thầu
                wbHinhThucCNT.Style.Font.FontName = "Times New Roman";
                wbHinhThucCNT.Style.Font.FontSize = 13;
                wbHinhThucCNT.PageSetup.FitToPages(1, 1);
                wbHinhThucCNT.Row(4).Height = 30;
                wbHinhThucCNT.Cell(2, 1).Value = "DANH MỤC HÌNH THỨC CHỌN NHÀ THẦU";
                wbHinhThucCNT.Range(2, 1, 2, 5).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbHinhThucCNT.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbHinhThucCNT.Cell(4, 1).Value = "STT";
                wbHinhThucCNT.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbHinhThucCNT.Cell(4, 2).Value = "Mã hình thức";
                wbHinhThucCNT.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbHinhThucCNT.Cell(4, 3).Value = "Tên viết tắt";
                wbHinhThucCNT.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbHinhThucCNT.Cell(4, 4).Value = "Tên hình thức";
                wbHinhThucCNT.Cell(4, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbHinhThucCNT.Cell(4, 5).Value = "Mô tả";
                wbHinhThucCNT.Cell(4, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                IEnumerable<NH_DM_HinhThucChonNhaThau> hinhThucCNTList = _qlnhService.GetNHDMHinhThucChonNhaThauList();
                i = 5;
                j = 1;
                foreach (NH_DM_HinhThucChonNhaThau item in hinhThucCNTList)
                {
                    wbHinhThucCNT.Cell(i, 1).Value = j;
                    wbHinhThucCNT.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbHinhThucCNT.Cell(i, 2).Value = item.sMaHinhThuc;
                    wbHinhThucCNT.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbHinhThucCNT.Cell(i, 3).Value = item.sTenVietTat;
                    wbHinhThucCNT.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbHinhThucCNT.Cell(i, 4).Value = item.sTenHinhThuc;
                    wbHinhThucCNT.Cell(i, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbHinhThucCNT.Cell(i, 5).Value = item.sMoTa;
                    wbHinhThucCNT.Cell(i, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }

                //Sheet Phương thức chọn nhà thầu
                wbPhuongThucCNT.Style.Font.FontName = "Times New Roman";
                wbPhuongThucCNT.Style.Font.FontSize = 13;
                wbPhuongThucCNT.PageSetup.FitToPages(1, 1);
                wbPhuongThucCNT.Row(4).Height = 30;
                wbPhuongThucCNT.Cell(2, 1).Value = "DANH MỤC PHƯƠNG THỨC CHỌN NHÀ THẦU";
                wbPhuongThucCNT.Range(2, 1, 2, 5).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbPhuongThucCNT.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbPhuongThucCNT.Cell(4, 1).Value = "STT";
                wbPhuongThucCNT.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhuongThucCNT.Cell(4, 2).Value = "Mã phương thức";
                wbPhuongThucCNT.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhuongThucCNT.Cell(4, 3).Value = "Tên viết tắt";
                wbPhuongThucCNT.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhuongThucCNT.Cell(4, 4).Value = "Tên phương thức";
                wbPhuongThucCNT.Cell(4, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhuongThucCNT.Cell(4, 5).Value = "Mô tả";
                wbPhuongThucCNT.Cell(4, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                IEnumerable<NH_DM_PhuongThucChonNhaThau> phuongThucCNTList = _qlnhService.GetNHDMPhuongThucChonNhaThauList();
                i = 5;
                j = 1;
                foreach (NH_DM_PhuongThucChonNhaThau item in phuongThucCNTList)
                {
                    wbPhuongThucCNT.Cell(i, 1).Value = j;
                    wbPhuongThucCNT.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhuongThucCNT.Cell(i, 2).Value = item.sMaPhuongThuc;
                    wbPhuongThucCNT.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhuongThucCNT.Cell(i, 3).Value = item.sTenVietTat;
                    wbPhuongThucCNT.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhuongThucCNT.Cell(i, 4).Value = item.sTenPhuongThuc;
                    wbPhuongThucCNT.Cell(i, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhuongThucCNT.Cell(i, 5).Value = item.sMoTa;
                    wbPhuongThucCNT.Cell(i, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }

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
                i = 5;
                j = 1;
                foreach (NH_DM_LoaiHopDong item in loaiHopDongList)
                {
                    wbLoaiHopDong.Cell(i, 1).Value = j;
                    wbLoaiHopDong.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbLoaiHopDong.Cell(i, 2).Value = item.sMaLoaiHopDong;
                    wbLoaiHopDong.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
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

                //Sheet Nhà thầu - Đơn vị ủy thác
                wbNhaThau.Style.Font.FontName = "Times New Roman";
                wbNhaThau.Style.Font.FontSize = 13;
                wbNhaThau.PageSetup.FitToPages(1, 1);
                wbNhaThau.Row(4).Height = 30;
                wbNhaThau.Cell(2, 1).Value = "DANH MỤC NHÀ THẦU/ ĐƠN VỊ ỦY THÁC";
                wbNhaThau.Range(2, 1, 2, 6).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
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
                foreach (NH_DM_NhaThau nhaThau in nhaThauList)
                {
                    wbNhaThau.Cell(i, 1).Value = j;
                    wbNhaThau.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbNhaThau.Cell(i, 2).Value = nhaThau.sMaNhaThau;
                    wbNhaThau.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
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

                wbContractInfo.Column(3).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(3).AsRange().SetDataValidation().TextLength.Between(0, 100);
                wbContractInfo.Column(3).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(3).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(3).AsRange().SetDataValidation().ErrorMessage = "Số quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu vượt quá 100 kí tự! Vui lòng nhập lại.";
                wbContractInfo.Column(3).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 3, 17, 3).SetDataValidation().ClearRanges();

                wbContractInfo.Column(5).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(5).AsRange().SetDataValidation().TextLength.Between(0, 100);
                wbContractInfo.Column(5).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(5).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(5).AsRange().SetDataValidation().ErrorMessage = "Số quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán vượt quá 100 kí tự! Vui lòng nhập lại.";
                wbContractInfo.Column(5).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 5, 17, 5).SetDataValidation().ClearRanges();

                wbContractInfo.Column(7).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(7).AsRange().SetDataValidation().TextLength.Between(0, 300);
                wbContractInfo.Column(7).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(7).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(7).AsRange().SetDataValidation().ErrorMessage = "Tên gói thầu vượt quá 300 kí tự! Vui lòng nhập lại.";
                wbContractInfo.Column(7).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 7, 17, 7).SetDataValidation().ClearRanges();

                wbContractInfo.Column(2).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                wbContractInfo.Column(2).AsRange().SetDataValidation().List(wbLoai.Range(5, 2, wbLoai.RowCount(), 2), true);
                wbContractInfo.Column(2).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorMessage = "Mã loại không tồn tại!";
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 2, 17, 2).SetDataValidation().ClearRanges();
                if (nhaThauList.Count() > 0)
                {
                    wbContractInfo.Column(9).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(9).AsRange().SetDataValidation().List(wbNhaThau.Range(5, 2, wbNhaThau.RowCount(), 2), true);
                    wbContractInfo.Column(9).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(9).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(9).AsRange().SetDataValidation().ErrorMessage = "Mã nhà thầu/đơn vị ủy thác không tồn tại!";
                    wbContractInfo.Column(9).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 9, 17, 9).SetDataValidation().ClearRanges();
                }
                if (tienTeList.Count() > 0)
                {
                    wbContractInfo.Column(10).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(10).AsRange().SetDataValidation().List(wbTienTe.Range(5, 2, wbTienTe.RowCount(), 2), true);
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ErrorMessage = "Mã đơn vị tiền tệ không tồn tại!";
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 10, 17, 10).SetDataValidation().ClearRanges();
                }
                if (hinhThucCNTList.Count() > 0)
                {
                    wbContractInfo.Column(11).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(11).AsRange().SetDataValidation().List(wbHinhThucCNT.Range(5, 2, wbHinhThucCNT.RowCount(), 2), true);
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ErrorMessage = "Mã hình thức chọn nhà thầu không tồn tại!";
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 11, 17, 11).SetDataValidation().ClearRanges();
                }
                if (phuongThucCNTList.Count() > 0)
                {
                    wbContractInfo.Column(12).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(12).AsRange().SetDataValidation().List(wbPhuongThucCNT.Range(5, 2, wbPhuongThucCNT.RowCount(), 2), true);
                    wbContractInfo.Column(12).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(12).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(12).AsRange().SetDataValidation().ErrorMessage = "Mã phương thức chọn nhà thầu không tồn tại!";
                    wbContractInfo.Column(12).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 12, 17, 12).SetDataValidation().ClearRanges();
                }
                if (loaiHopDongList.Count() > 0)
                {
                    wbContractInfo.Column(13).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(13).AsRange().SetDataValidation().List(wbLoaiHopDong.Range(5, 2, wbLoaiHopDong.RowCount(), 2), true);
                    wbContractInfo.Column(13).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(13).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(13).AsRange().SetDataValidation().ErrorMessage = "Mã loại hợp đồng không tồn tại!";
                    wbContractInfo.Column(13).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 13, 17, 13).SetDataValidation().ClearRanges();
                }

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Biểu mẫu import thông tin gói thầu.xlsx");
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

        private string GetHtmlDataExcel(IEnumerable<NH_DA_GoiThau_ImportModel> dataImport)
        {
            StringBuilder sb = new StringBuilder();
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, true).ToList();
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
            List<NH_DA_DuAn> lstDuAn = _qlnhService.GetNHDADuAnList().ToList();
            List<NH_DM_LoaiHopDong> lstLoaiHopDong = _qlnhService.GetNHDMLoaiHopDongList().ToList();
            List<NH_DM_NhaThau> lstNhaThau = _qlnhService.GetNHDMNhaThauList().ToList();
            List<NH_DM_LoaiTienTe> lstTienTe = _qlnhService.GetNHDMLoaiTienTeList().ToList();
            List<NH_DM_HinhThucChonNhaThau> lstHinhThuc = _qlnhService.GetNHDMHinhThucChonNhaThauList().ToList();
            List<NH_DM_PhuongThucChonNhaThau> lstPhuongThuc = _qlnhService.GetNHDMPhuongThucChonNhaThauList().ToList();

            string htmlDonVi = GetHtmlSelectOptionDonVi(lstDonViQL);
            string htmlChuongTrinh = GetHtmlSelectOptionChuongTrinh(lstChuongTrinh);
            string htmlDuAn = GetHtmlSelectOptionDuAn(lstDuAn);
            string htmlLoai = GetHtmlSelectOptionLoai();
            string htmlLoaiHopDong = GetHtmlSelectOptionLoaiHopDong(lstLoaiHopDong);
            string htmlNhaThau = GetHtmlSelectOptionNhaThau(lstNhaThau, null);
            string htmlTienTe = GetHtmlSelectOptionLoaiTiente(lstTienTe);
            string htmlHinhThuc = GetHtmlSelectOptionHinhThucCNT(lstHinhThuc);
            string htmlPhuongThuc = GetHtmlSelectOptionPhuongThucCNT(lstPhuongThuc);

            int i = 0;
            foreach (NH_DA_GoiThau_ImportModel item in dataImport)
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

                //Tên gói thầu
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsTenGoiThauWrong ? "cellWrong" : "") + "'>");
                if (item.IsTenGoiThauWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtTenGoiThau" + i + "' value='" + HttpUtility.HtmlEncode(item.sTenGoiThau) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanTenGoiThau" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sTenGoiThau) + "</span>");
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
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsMaLoaiWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaLoaiWrong)
                {
                    sb.AppendLine("<select id=\"slbLoai" + i + "\" name=\"slbLoai" + i + "\" class=\"form-control selectLoai\" data-index=\"" + i + "\" onchange=\"ChangeSelectLoai(this);\">");
                    sb.AppendLine(htmlLoai);
                    sb.AppendLine("</select>");
                }
                else
                {
                    sb.AppendLine("<span id='spanLoai" + i + "' class='spanCommon' data-maloai='" + HttpUtility.HtmlEncode(item.sMaLoai) + "'>" + HttpUtility.HtmlEncode(item.sTenLoai) + "</span>");
                }
                sb.AppendLine("</td>");

                //Dự án
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<div class='divDuAn " + (item.loai.HasValue && item.loai.Value != 1 && item.loai.Value != 3 ? "hidden" : string.Empty) + "'>");
                sb.AppendLine("<select id=\"slbDuAn" + i + "\" name=\"slbDuAn" + i + "\" class=\"form-control selectDuAn\" data-index=\"" + i + "\">");
                sb.AppendLine(htmlDuAn);
                sb.AppendLine("</select>");
                sb.AppendLine("</div>");
                sb.AppendLine("</td>");

                //Số quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsSoQuyetDinh1Wrong ? "cellWrong" : "") + "'>");
                if (item.IsSoQuyetDinh1Wrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtSoQuyetDinh1" + i + "' value='" + HttpUtility.HtmlEncode(item.sSoQuyetDinh1) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanSoQuyetDinh1" + i + "' class='spanCommon' data-content='" + HttpUtility.HtmlEncode(item.sSoQuyetDinh1) + "'>" + HttpUtility.HtmlEncode(item.sSoQuyetDinh1) + "</span>");
                }
                sb.AppendLine("</td>");

                //Ngày quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsNgayQuyetDinh1Wrong ? "cellWrong" : "") + "'>");
                if (item.IsNgayQuyetDinh1Wrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate' placeholder='dd/MM/yyyy' autocomplete='off' id='txtNgayQuyetDinh1" + i + "' value='" + HttpUtility.HtmlEncode(item.sNgayQuyetDinh1) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanNgayQuyetDinh1" + i + "' class='spanCommon' data-content='" + HttpUtility.HtmlEncode(item.sNgayQuyetDinh1) + "'>" + HttpUtility.HtmlEncode(item.sNgayQuyetDinh1) + "</span>");
                }
                sb.AppendLine("</td>");

                //Số quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsSoQuyetDinh2Wrong ? "cellWrong" : "") + "'>");
                if (item.IsSoQuyetDinh2Wrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtSoQuyetDinh2" + i + "' value='" + HttpUtility.HtmlEncode(item.sSoQuyetDinh2) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanSoQuyetDinh2" + i + "' class='spanCommon' data-content='" + HttpUtility.HtmlEncode(item.sSoQuyetDinh2) + "'>" + HttpUtility.HtmlEncode(item.sSoQuyetDinh2) + "</span>");
                }
                sb.AppendLine("</td>");

                //Ngày quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsNgayQuyetDinh2Wrong ? "cellWrong" : "") + "'>");
                if (item.IsNgayQuyetDinh2Wrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate' placeholder='dd/MM/yyyy' autocomplete='off' id='txtNgayQuyetDinh2" + i + "' value='" + HttpUtility.HtmlEncode(item.sNgayQuyetDinh2) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanNgayQuyetDinh2" + i + "' class='spanCommon' data-content='" + HttpUtility.HtmlEncode(item.sNgayQuyetDinh2) + "'>" + HttpUtility.HtmlEncode(item.sNgayQuyetDinh2) + "</span>");
                }
                sb.AppendLine("</td>");

                //Nhà thầu/Đơn vị ủy thác
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaNhaThauWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaNhaThauWrong)
                {
                    sb.AppendLine("<div class='inputCommon'>");
                    sb.AppendLine("<select id=\"slbNhaThau" + i + "\" name=\"slbNhaThau" + i + "\" class=\"form-control inputCommon selectNhaThau\">");
                    if (item.loai.HasValue && item.loai.Value != 0 && loaiList.FirstOrDefault(x => x.Value == item.loai.Value) != null)
                    {
                        switch(item.loai.Value)
                        {
                            case 1:
                            case 2:
                                htmlNhaThau = GetHtmlSelectOptionNhaThau(lstNhaThau.Where(x => x.iLoai.HasValue && x.iLoai.Value == 1).ToList(), item.loai);
                                break;
                            case 3:
                            case 4:
                                htmlNhaThau = GetHtmlSelectOptionNhaThau(lstNhaThau.Where(x => x.iLoai.HasValue && x.iLoai.Value == 2).ToList(), item.loai);
                                break;
                        }
                    }
                    sb.AppendLine(htmlNhaThau);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaNhaThau" + i + "' data-id='" + item.iID_NhaThau + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sMaNhaThau) + "</span>");
                }
                sb.AppendLine("</td>");

                //Thanh toán bằng
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaTienTeWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaTienTeWrong)
                {
                    sb.AppendLine("<div class='inputCommon'>");
                    sb.AppendLine("<select id=\"slbTienTe" + i + "\" name=\"slbTienTe" + i + "\" class=\"form-control selectTienTe\">");
                    sb.AppendLine(htmlTienTe);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaTienTe" + i + "' class='spanCommon' data-matien='" + HttpUtility.HtmlEncode(item.sMaTienTe) + "'>" + HttpUtility.HtmlEncode(item.sMaTienTe) + "</span>");
                }
                sb.AppendLine("</td>");

                //Hình thức chọn nhà thầu
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaHinhThucCNTWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaHinhThucCNTWrong)
                {
                    sb.AppendLine("<div class='inputCommon divLoaiTrongNuoc" + i + " " + (item.loai.HasValue && item.loai.Value != 1 && item.loai.Value != 2 ? "hidden" : string.Empty) + "'>");
                    sb.AppendLine("<select id=\"slbHinhThucCNT" + i + "\" name=\"slbHinhThucCNT" + i + "\" class=\"form-control selectHinhThucCNT\">");
                    sb.AppendLine(htmlHinhThuc);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaHinhThucCNT" + i + "' data-id='" + item.iID_HinhThucChonNhaThauID + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sMaHinhThucCNT) + "</span>");
                }
                sb.AppendLine("</td>");

                //Phương thức chọn nhà thầu
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaPhuongThucCNTWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaPhuongThucCNTWrong)
                {
                    sb.AppendLine("<div class='inputCommon divLoaiTrongNuoc" + i + " " + (item.loai.HasValue && item.loai.Value != 1 && item.loai.Value != 2 ? "hidden" : string.Empty) + "'>");
                    sb.AppendLine("<select id=\"slbPhuongThucCNT" + i + "\" name=\"slbPhuongThucCNT" + i + "\" class=\"form-control selectPhuongThucCNT\">");
                    sb.AppendLine(htmlTienTe);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaPhuongThucCNT" + i + "' data-id='" + item.iID_PhuongThucChonNhaThauID + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sMaPhuongThucCNT) + "</span>");
                }
                sb.AppendLine("</td>");

                //Loại hợp đồng
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaLoaiHopDongWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaLoaiHopDongWrong)
                {
                    sb.AppendLine("<div class='inputCommon divLoaiTrongNuoc" + i + " " + (item.loai.HasValue && item.loai.Value != 1 && item.loai.Value != 2 ? "hidden" : string.Empty) + "'>");
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

        private string GetHtmlSelectOptionNhaThau(List<NH_DM_NhaThau> nhaThauList, int? iLoai)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, GetOptionDefaultNhaThau(iLoai));
            if (nhaThauList != null)
            {
                for (int i = 0; i < nhaThauList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", nhaThauList[i].Id, HttpUtility.HtmlEncode(nhaThauList[i].sTenNhaThau));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionLoaiTiente(List<NH_DM_LoaiTienTe> tienTeList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", string.Empty, "--Chọn thanh toán--");
            if (tienTeList != null)
            {
                for (int i = 0; i < tienTeList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", HttpUtility.HtmlEncode(tienTeList[i].sMaTienTe), HttpUtility.HtmlEncode(tienTeList[i].sMaTienTe));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionHinhThucCNT(List<NH_DM_HinhThucChonNhaThau> hinhThucCNTList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn hình thức--");
            if (hinhThucCNTList != null)
            {
                for (int i = 0; i < hinhThucCNTList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", hinhThucCNTList[i].ID, HttpUtility.HtmlEncode(hinhThucCNTList[i].sTenHinhThuc));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionPhuongThucCNT(List<NH_DM_PhuongThucChonNhaThau> phuongThucCNTList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn phương thức--");
            if (phuongThucCNTList != null)
            {
                for (int i = 0; i < phuongThucCNTList.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", phuongThucCNTList[i].ID, HttpUtility.HtmlEncode(phuongThucCNTList[i].sTenPhuongThuc));
                }
            }
            return sb.ToString();
        }

        private byte[] GetBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
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