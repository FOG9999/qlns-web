using System;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using Viettel.Models.QLNH;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net;
using VIETTEL.Common;
using System.Web;
using ClosedXML.Excel;
using System.IO;
using VIETTEL.Helpers;
using System.Reflection;
using VIETTEL.Areas.z.Models;
using System.Data;

namespace VIETTEL.Areas.QLNH.Controllers.DuAnHopDong
{
    public class ThongTinDuAnController : AppController
    {
        private readonly IQLNHService qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private readonly List<string> MaTienTeList = new List<string>() { "USD", "VND", "EUR" };

        public ActionResult Index()
        {
            NHDAThongTinDuAnViewModel vm = new NHDAThongTinDuAnViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = qlnhService.getListThongTinDuAnModels(ref vm._paging);

            List<NS_PhongBan> llstThongTinDuAn = qlnhService.GetLookupQuanLy().ToList();
            llstThongTinDuAn.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sTen = "--Chọn B quán lý--" });
            ViewBag.ListPhongBan = llstThongTinDuAn;

            List<NS_DonVi> lstDonViQuanLy = qlnhService.GetLookupThongTinDonVi().ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQuanLy;

            List<DM_ChuDauTu> listChuDauTu = qlnhService.GetLookupChuDauTu().ToList();
            listChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = "--Chọn chủ đầu tư--" });
            ViewBag.ListChuDauTu = listChuDauTu;

            List<NH_DM_PhanCapPheDuyet> listDmPhanCapPheDuyet = qlnhService.GetLookupThongTinDuAn().ToList();
            listDmPhanCapPheDuyet.Insert(0, new NH_DM_PhanCapPheDuyet { ID = Guid.Empty, sTen = "--Chọn phân cấp phê duyệt--" });
            vm.ListDanhMucPCPD = listDmPhanCapPheDuyet;
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sMaDuAn, string sTenDuAn, Guid? iID_BQuanLyID, Guid? iID_ChuDauTuID, Guid? iID_DonViID, Guid? iID_CapPheDuyetID)
        {
            NHDAThongTinDuAnViewModel vm = new NHDAThongTinDuAnViewModel();

            sMaDuAn = HttpUtility.HtmlDecode(sMaDuAn);
            sTenDuAn = HttpUtility.HtmlDecode(sTenDuAn);
            vm._paging = _paging;
            vm.Items = qlnhService.getListThongTinDuAnModels(ref vm._paging, sMaDuAn, sTenDuAn, iID_BQuanLyID, iID_ChuDauTuID, iID_DonViID, iID_CapPheDuyetID);

            List<NS_PhongBan> llstThongTinDuAn = qlnhService.GetLookupQuanLy().ToList();
            llstThongTinDuAn.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sTen = "--Chọn--" });
            ViewBag.ListPhongBan = llstThongTinDuAn;

            List<NS_DonVi> lstDonViQuanLy = qlnhService.GetLookupThongTinDonVi().ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = "--Chọn--" });
            ViewBag.ListDonVi = lstDonViQuanLy;

            List<DM_ChuDauTu> listChuDauTu = qlnhService.GetLookupChuDauTu().ToList();
            listChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = "--Chọn--" });
            ViewBag.ListChuDauTu = listChuDauTu;

            List<NH_DM_PhanCapPheDuyet> glstThongTinDuAn = qlnhService.GetLookupThongTinDuAn().ToList();
            glstThongTinDuAn.Insert(0, new NH_DM_PhanCapPheDuyet { ID = Guid.Empty, sTen = "--Chọn--" });
            vm.ListDanhMucPCPD = glstThongTinDuAn;

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            NHDAThongTinDuAnModel data = new NHDAThongTinDuAnModel();
            if (id.HasValue)
            {
                data = qlnhService.GetThongTinDuAnById(id.Value);
            }
            ViewBag.State = "DETAIL";
            return PartialView("Detail", data);
        }

        [HttpPost]
        public ActionResult OpenUpdate(Guid? id, bool isDieuChinh = false)
        {
            ViewBag.State = id.HasValue ? (isDieuChinh ? "ADJUST" : "UPDATE") : "CREATE";
            NHDAThongTinDuAnViewModel vm = new NHDAThongTinDuAnViewModel();
            NHDAThongTinDuAnModel data = new NHDAThongTinDuAnModel();
            vm.ThongTinDuAnDetail = new NHDAThongTinDuAnModel();
            if (id.HasValue)
            {
                vm.ThongTinDuAnDetail = qlnhService.GetThongTinDuAnById(id.Value);
            }
            if (data.iID_TiGiaID != null)
            {
                NH_DM_TiGia tiGia = qlnhService.GetNHDMTiGiaList(data.iID_TiGiaID).ToList().FirstOrDefault();
                List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = qlnhService.GetNHDMTiGiaChiTietList(data.iID_TiGiaID, false).ToList();
                ViewBag.HtmlTienTe = GetHtmlTienteQuyDoi(tiGiaChiTietList, tiGia.sMaTienTeGoc, data.iID_TiGia_ChiTietID, data.sMaNgoaiTeKhac);
            }
            List<NS_PhongBan> llstThongTinDuAn = qlnhService.GetLookupQuanLy().ToList();
            llstThongTinDuAn.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sTen = "--Chọn B quản lý--" });
            ViewBag.ListPhongBan = llstThongTinDuAn;

            List<DM_ChuDauTu> listChuDauTu = qlnhService.GetLookupChuDauTu().ToList();
            listChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = "--Chọn chủ đầu tư--" });
            ViewBag.ListChuDauTu = listChuDauTu;

            List<NH_DM_PhanCapPheDuyet> listDmPhanCapPheDuyet = qlnhService.GetLookupThongTinDuAn().ToList();
            listDmPhanCapPheDuyet.Insert(0, new NH_DM_PhanCapPheDuyet { ID = Guid.Empty, sTen = "--Chọn phân cấp phê duyệt--" });
            ViewBag.ListDanhMucPCPD = listDmPhanCapPheDuyet;

            List<NH_DM_TiGia> lstTiGia = qlnhService.GetNHDMTiGiaList().ToList();
            lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
            ViewBag.ListTiGia = lstTiGia;

            List<NH_KHChiTietBQP_NhiemVuChiModel> listChiTieBQL = qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiListDuAn().ToList();
            listChiTieBQL.Insert(0, new NH_KHChiTietBQP_NhiemVuChiModel { ID = Guid.Empty, sSoKeHoachBQP = "--Chọn kế hoạch chi tiết BQP--" });
            ViewBag.ListBQP = listChiTieBQL;

            List<NH_DM_TiGia_ChiTiet> lstTiGiaChiTiet = qlnhService.GetNHDMTiGiaChiTietList(null).ToList();
            lstTiGiaChiTiet.Insert(0, new NH_DM_TiGia_ChiTiet { ID = Guid.Empty, sMaTienTeQuyDoi = "--Chọn mã ngoại tệ khác--" });
            ViewBag.ListTiGiaChiTiet = lstTiGiaChiTiet;

            List<NS_DonVi> lstDonViQuanLy = qlnhService.GetLookupThongTinDonVi().ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQuanLy;

            List<NH_KHChiTietBQP_NhiemVuChi> lstct = qlnhService.GetListCTbyDV(vm.ThongTinDuAnDetail.iID_DonViID).ToList();
            lstct.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { iID_KHTTTTCP_NhiemVuChiID = Guid.Empty, sTenNhiemVuChi = "--Chọn chương trình--" });
            ViewBag.ListChuongTrinh = lstct;

            return PartialView("Update", vm);
        }
        [HttpPost]
        public JsonResult GetChuongTrinhTheoKHBQP(Guid? id)
        {
            List<NS_DonVi> lstDonVi = qlnhService.GetListDonViToBQP(id).ToList();
            StringBuilder htmlDonVi = new StringBuilder();
            htmlDonVi.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn đơn vị--");
            if (lstDonVi != null && lstDonVi.Count > 0)
            {
                for (int i = 0; i < lstDonVi.Count; i++)
                {
                    htmlDonVi.AppendFormat("<option value='{0}' data-madonvi={1}>{2}</option>", lstDonVi[i].iID_Ma, lstDonVi[i].iID_MaDonVi, WebUtility.HtmlEncode(lstDonVi[i].iID_MaDonVi + " - " + lstDonVi[i].sTen));
                }
            }
            return Json(new { htmlDV = htmlDonVi.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetChuongTrinhTheoDV(Guid? id, Guid? idBQP)
        {
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = qlnhService.GetListCTbyDV(id, idBQP).ToList();
            StringBuilder htmlChuongTrinh = new StringBuilder();
            htmlChuongTrinh.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn chương trình--");
            if (lstChuongTrinh != null && lstChuongTrinh.Count > 0)
            {
                for (int i = 0; i < lstChuongTrinh.Count; i++)
                {
                    htmlChuongTrinh.AppendFormat("<option value='{0}'>{1}</option>", lstChuongTrinh[i].ID, WebUtility.HtmlEncode(lstChuongTrinh[i].sTenNhiemVuChi));
                }
            }
            return Json(new { htmlCT = htmlChuongTrinh.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKHBQPTheoNHV(Guid? id)
        {
            List<NHDAThongTinDuAnModel> lstBQP = qlnhService.GetListBQPToNHC(id).ToList();
            StringBuilder htmlBQP = new StringBuilder();
            htmlBQP.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn kế hoạch tổng thể BQP--");
            if (lstBQP != null && lstBQP.Count > 0)
            {
                for (int i = 0; i < lstBQP.Count; i++)
                {
                    htmlBQP.AppendFormat("<option value='{0}'>{1}</option>", lstBQP[i].iID_KHCTBQP_ChuongTrinhID, WebUtility.HtmlEncode(lstBQP[i].sSoKeHoachBQP));
                }
            }
            return Json(new { htmlBQP = htmlBQP.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ThongTinDuAnDelete(Guid id)
        {
            if (!qlnhService.DeleteThongTinDuAn(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLookupChiPhi()
        {
            return Json(new
            {
                data = qlnhService.GetLookupChiPhi()
            });
        }

        [HttpPost]
        public ActionResult ImportThongTinDuAn()
        {
            return PartialView("Import");
        }

        [HttpPost]
        public ActionResult LoadDataExcel(HttpPostedFileBase file)
        {
            string data;
            try
            {
                byte[] file_data = GetBytes(file);
                DataTable dataTable = ExcelHelpers.LoadExcelDataTable(file_data);
                IEnumerable<NHDAThongTinDuAnImportModel> dataImport = GetExcelResult(dataTable);
                data = GetHtmlDataExcel(dataImport);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { sMessError = "Không thể tải dữ liệu từ file đã chọn!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        private string GetHtmlDataExcel(IEnumerable<NHDAThongTinDuAnImportModel> dataImport)
        {
            StringBuilder sb = new StringBuilder();
            List<NH_KHChiTietBQP_NhiemVuChiModel> lstBQP = qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiListDuAn().ToList();

            List<NS_DonVi> lstDonVi = qlnhService.GetLookupThongTinDonVi().ToList();
            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = qlnhService.GetListChuongTrinh().ToList();


            List<NS_PhongBan> lstQuanLy = qlnhService.GetLookupQuanLy().ToList();
            List<NH_DM_PhanCapPheDuyet> lstPhanCapPheDuyet = qlnhService.GetLookupThongTinDuAn().ToList();
            List<DM_ChuDauTu> lstChuDauTu = qlnhService.GetLookupChuDauTuu().ToList();

            string htmlBQP = GetHtmlSelectOptionBQP(lstBQP);
            string htmlDonVi = GetHtmlSelectOptionDonVi(lstDonVi);
            string htmlChuongTrinh = GetHtmlSelectOptionChuongTrinh(lstChuongTrinh);



            string htmlQuanLy = GetHtmlSelectOptionQuanLy(lstQuanLy);
            string htmlPhanCapPheDuyet = GetHtmlSelectOptionPhanCapPheDuyet(lstPhanCapPheDuyet);
            string htmlChuDauTu = GetHtmlSelectOptionChuDauTu(lstChuDauTu);

            int i = 0;
            foreach (NHDAThongTinDuAnImportModel item in dataImport)
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
                sb.AppendLine(HttpUtility.HtmlEncode(item.STT));
                sb.AppendLine("</td>");

                //BQP   
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<select id=\"slbKHTongTheBQP" + i + "\" name=\"slbKHTongTheBQP" + i + "\" class=\"form-control selectBQP\" data-index=\"" + i + "\" onchange=\"ChangeBQPSelectImport(this);\">");
                sb.AppendLine(htmlBQP);
                sb.AppendLine("</select>");
                sb.AppendLine("</td>");

                //B Quan Ly
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<select id=\"slbBQuanLy" + i + "\" name=\"slbBQuanLy" + i + "\" class=\"form-control selectBQuanLy\" data-index=\"" + i + "\">");
                sb.AppendLine(htmlQuanLy);
                sb.AppendLine("</select>");
                sb.AppendLine("</td>");

                // Don Vi
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<select id=\"slbDonVi" + i + "\" name=\"slbDonVi" + i + "\" class=\"form-control selectDonVi\" data-index=\"" + i + "\" onchange=\"ChangeDVSelectImport(this);\" >");
                sb.AppendLine(htmlDonVi);
                sb.AppendLine("</select>");
                sb.AppendLine("</td>");

                //Chuong Trinh
                sb.AppendLine("<td align='center' style='vertical-align:middle;'>");
                sb.AppendLine("<select id=\"slbChuongTrinh" + i + "\" name=\"slbChuongTrinh" + i + "\" class=\"form-control selectChuongTrinh\" data-index=\"" + i + "\">");
                sb.AppendLine(htmlChuongTrinh);
                sb.AppendLine("</select>");
                sb.AppendLine("</td>");

                //Ma Du An
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsMaDuAnWrong ? "cellWrong" : "") + "'>");
                if (item.IsMaDuAnWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtMaDuAn" + i + "' value='" + HttpUtility.HtmlEncode(item.sMaDuAn) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaDuAn" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sMaDuAn) + "</span>");
                }
                sb.AppendLine("</td>");

                //Tên dự án
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsTenDuAnWrong ? "cellWrong" : "") + "'>");
                if (item.IsTenDuAnWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtTenDuAn" + i + "' value='" + HttpUtility.HtmlEncode(item.sTenDuAn) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanTenDuAn" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sTenDuAn) + "</span>");
                }
                sb.AppendLine("</td>");

                //số chủ truonwg đầu tư

                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsSoChuTruongDauTuWrong ? "cellWrong" : "") + "'>");
                if (item.IsSoChuTruongDauTuWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtSoChuTruongDauTu" + i + "' value='" + HttpUtility.HtmlEncode(item.sSoChuTruongDauTu) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanSoChuTruongDauTu" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sSoChuTruongDauTu) + "</span>");
                }
                sb.AppendLine("</td>");

                //ngày chủ trương đầu tư

                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IssNgayChuTruongDauTuWrong ? "cellWrong" : "") + "'>");
                if (item.IssNgayChuTruongDauTuWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate' required autocomplete='off' id='txtNgayBanHanhCTDT" + i + "' value='" + HttpUtility.HtmlEncode(item.sNgayChuTruongDauTu) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanNgayChuTruongDauTu" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sNgayChuTruongDauTu) + "</span>");
                }
                sb.AppendLine("</td>");


                //số quyết định đầu tư

                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsSoQuetDinhDauTuWrong ? "cellWrong" : "") + "'>");
                if (item.IsSoQuetDinhDauTuWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtSoQuyetDinhDauTu" + i + "' value='" + HttpUtility.HtmlEncode(item.sSoQuetDinhDauTu) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanSoQuetDinhDauTu" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sSoQuetDinhDauTu) + "</span>");
                }
                sb.AppendLine("</td>");

                //ngày quyết định đầu tư
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsNgayQuyetDinhDauTuWrong ? "cellWrong" : "") + "'>");
                if (item.IsNgayQuyetDinhDauTuWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate' placeholder='dd/MM/yyyy' autocomplete='off' id='txtNgayQuetDinhDauTu" + i + "' value='" + HttpUtility.HtmlEncode(item.sNgayQuyetDinhDauTu) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanNgayQuetDinhDauTu" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sNgayQuyetDinhDauTu) + "</span>");
                }
                sb.AppendLine("</td>");

                //sổ dự toán
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsSoDuToanWrong ? "cellWrong" : "") + "'>");
                if (item.IsSoDuToanWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputThoiGianThucHien' autocomplete='off' id='txtSoDuToanTu" + i + "' value='" + HttpUtility.HtmlEncode(item.sSoDuToan) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanSoDuToan" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sSoDuToan) + "</span>");
                }
                sb.AppendLine("</td>");

                //Ngày dự toán
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsNgayDuToanWrong ? "cellWrong" : "") + "'>");
                if (item.IsNgayDuToanWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon inputDate inputThoiGianThucHien' autocomplete='off' id='txtNgayBanHanhDuToan" + i + "' value='" + HttpUtility.HtmlEncode(item.sNgayDuToan) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanNgayDuToan" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sNgayDuToan) + "</span>");
                }
                sb.AppendLine("</td>");

                //Phan Cap Phe Duyet
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsPhanCapPheDuyetWrong ? "cellWrong" : "") + "'>");
                if (item.IsPhanCapPheDuyetWrong)
                {
                    sb.AppendLine("<div class='inputCommon'>");
                    sb.AppendLine("<select id=\"slbPhanCapPheDuyet" + i + "\" name=\"slbPhanCapPheDuyet" + i + "\" class=\"form-control inputCommon selectPhanCapPheDuyet\">");
                    sb.AppendLine(htmlPhanCapPheDuyet);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaPhanCap" + i + "' data-id='" + item.iID_PhanCapPheDuyet + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sPhanCapPheDuyet) + "</span>");
                }
                sb.AppendLine("</td>");

                //Chu Dau Tu
                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsChuDauTuWrong ? "cellWrong" : "") + "'>");
                if (item.IsChuDauTuWrong)
                {
                    sb.AppendLine("<div class='inputCommon'>");
                    sb.AppendLine("<select id=\"slbChuDauTu" + i + "\" name=\"slbChuDauTu" + i + "\" class=\"form-control inputCommon selectChuDauTu\">");
                    sb.AppendLine(htmlChuDauTu);
                    sb.AppendLine("</select>");
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine("<span id='spanMaChuDauTu" + i + "' data-id='" + item.iID_ChuDauTu + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sChuDauTu) + "</span>");
                }
                sb.AppendLine("</td>");


                //thời gian thực hiện từ
                sb.AppendLine("<td align='center' style='vertical-align:middle;' class='" + (item.IsKhoiCongWrong ? "cellWrong" : "") + "'>");
                if (item.IsKhoiCongWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon' autocomplete='off' id='txtThoiGianThucHienTu" + i + "' value='" + HttpUtility.HtmlEncode(item.sKhoiCong) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanThoiGianThucHienTu" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sKhoiCong) + "</span>");
                }
                sb.AppendLine("</td>");


                //Thời gian thực hiện đến

                sb.AppendLine("<td align='left' style='vertical-align:middle;' class='" + (item.IsKetThucWrong ? "cellWrong" : "") + "'>");
                if (item.IsKetThucWrong)
                {
                    sb.AppendLine("<input type='text' class='form-control inputCommon ' autocomplete='off' id='txtThoiGianThucHienDen" + i + "' value='" + HttpUtility.HtmlEncode(item.sKetThuc) + "'/>");
                }
                else
                {
                    sb.AppendLine("<span id='spanThoiGianThucHienDen" + i + "' class='spanCommon'>" + HttpUtility.HtmlEncode(item.sKetThuc) + "</span>");
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

        private string GetHtmlSelectOptionDonVi(List<NS_DonVi> lstDonVi)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' data-madonvi='' selected>{1}</option>", Guid.Empty, "--Chọn đơn vị--");
            if (lstDonVi != null)
            {
                for (int i = 0; i < lstDonVi.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}' data-madonvi='{2}'>{1}</option>", lstDonVi[i].iID_Ma, HttpUtility.HtmlEncode(lstDonVi[i].sTen), HttpUtility.HtmlEncode(lstDonVi[i].iID_MaDonVi));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionBQP(List<NH_KHChiTietBQP_NhiemVuChiModel> lstBQP)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' data-madonvi='' selected>{1}</option>", Guid.Empty, "--Chọn--");
            if (lstBQP != null)
            {
                for (int i = 0; i < lstBQP.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}' data-madonvi=''>{1}</option>", lstBQP[i].ID, HttpUtility.HtmlEncode(lstBQP[i].sSoKeHoachBQP));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionQuanLy(List<NS_PhongBan> lstQuanLy)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' data-madonvi='' selected>{1}</option>", Guid.Empty, "--Chọn đơn vị--");
            if (lstQuanLy != null)
            {
                for (int i = 0; i < lstQuanLy.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}' data-madonvi=''>{1}</option>", lstQuanLy[i].iID_MaPhongBan, HttpUtility.HtmlEncode(lstQuanLy[i].sTen));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionChuongTrinh(List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn chương trình--");
            if (lstChuongTrinh != null)
            {
                for (int i = 0; i < lstChuongTrinh.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", lstChuongTrinh[i].ID, HttpUtility.HtmlEncode(lstChuongTrinh[i].sTenNhiemVuChi));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionPhanCapPheDuyet(List<NH_DM_PhanCapPheDuyet> lstPhanCapPheDuyet)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn phân cấp phê duyệt--");
            if (lstPhanCapPheDuyet != null)
            {
                for (int i = 0; i < lstPhanCapPheDuyet.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", lstPhanCapPheDuyet[i].ID, HttpUtility.HtmlEncode(lstPhanCapPheDuyet[i].sTen));
                }
            }
            return sb.ToString();
        }

        private string GetHtmlSelectOptionChuDauTu(List<DM_ChuDauTu> lstChuDauTu)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn chủ đầu tư--");
            if (lstChuDauTu != null)
            {
                for (int i = 0; i < lstChuDauTu.Count(); i++)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", lstChuDauTu[i].ID, HttpUtility.HtmlEncode(lstChuDauTu[i].sTenCDT));
                }
            }
            return sb.ToString();
        }

        private IEnumerable<NHDAThongTinDuAnImportModel> GetExcelResult(DataTable dt)
        {
            List<NHDAThongTinDuAnImportModel> dataImportList = new List<NHDAThongTinDuAnImportModel>();
            NHDAThongTinDuAnImportModel data = new NHDAThongTinDuAnImportModel();
            DataRow row;

            string STT = string.Empty;
            string sMaDuAn = string.Empty;
            string sTenDuAn = string.Empty;
            string sSoChuTruongDauTu = string.Empty;
            string sNgayChuTruongDauTu = string.Empty;
            string sSoQuetDinhDauTu = string.Empty;
            string sNgayQuyetDinhDauTu = string.Empty;
            string sSoDuToan = string.Empty;
            string sNgayDuToan = string.Empty;
            string sChuDauTu = string.Empty;
            string sPhanCapPheDuyet = string.Empty;
            string sKhoiCong = string.Empty;
            string sKetThuc = string.Empty;
            DateTime? dNgayChuTruongDauTu;
            DateTime? dNgayQuyetDinhDauTu;
            DateTime? dNgayDuToan;
            Guid? iID_PhanCapPheDuyet = null;
            Guid? iID_ChuDauTu = null;

            StringBuilder sErrorMessage = new StringBuilder();
            bool isDataWrong = false;
            bool IsMaDuAnWrong = false;
            bool IsTenDuAnWrong = false;
            bool IsSoChuTruongDauTuWrong = false;
            bool IssNgayChuTruongDauTuWrong = false;
            bool IsSoQuetDinhDauTuWrong = false;
            bool IsNgayQuyetDinhDauTuWrong = false;
            bool IsSoDuToanWrong = false;
            bool IsNgayDuToanWrong = false;
            bool IsPhanCapPheDuyetWrong = false;
            bool IsChuDauTuWrong = false;
            bool IsKhoiCongWrong = false;
            bool IsKetThucWrong = false;

            DM_ChuDauTu dmChuDauTu;
            NH_DM_PhanCapPheDuyet dmPhanCapPheDuyet;
            IEnumerable<DM_ChuDauTu> chuDauTuList = qlnhService.GetLookupChuDauTuu();
            IEnumerable<NH_DM_PhanCapPheDuyet> phanCapPheDuyetList = qlnhService.GetLookupThongTinDuAn();

            var items = dt.AsEnumerable();
            for (var i = 17; i < items.Count(); i++)
            {

                isDataWrong = false;
                IsMaDuAnWrong = false;
                IsTenDuAnWrong = false;
                IsSoChuTruongDauTuWrong = false;
                IssNgayChuTruongDauTuWrong = false;
                IsSoQuetDinhDauTuWrong = false;
                IsNgayQuyetDinhDauTuWrong = false;
                IsSoDuToanWrong = false;
                IsNgayDuToanWrong = false;
                IsChuDauTuWrong = false;
                IsPhanCapPheDuyetWrong = false;
                IsKhoiCongWrong = false;
                IsKetThucWrong = false;
                sErrorMessage.Clear();
                row = items.ToList()[i];

                STT = row.Field<string>(0);
                sMaDuAn = !string.IsNullOrEmpty(row.Field<string>(1)) ? row.Field<string>(1).Trim() : string.Empty;
                if (string.IsNullOrEmpty(sMaDuAn))
                {
                    isDataWrong = true;
                    IsMaDuAnWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã dự án chưa được nhập.");
                }
                else if (sMaDuAn.Length > 100)
                {
                    isDataWrong = true;
                    IsMaDuAnWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã dự án nhập quá 100 kí tự.");
                }

                sTenDuAn = !string.IsNullOrEmpty(row.Field<string>(2)) ? row.Field<string>(2).Trim() : string.Empty;
                if (string.IsNullOrEmpty(sTenDuAn))
                {
                    isDataWrong = true;
                    IsTenDuAnWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Tên dự án chưa được nhập.");
                }

                sSoChuTruongDauTu = !string.IsNullOrEmpty(row.Field<string>(3)) ? row.Field<string>(3).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sSoChuTruongDauTu) && sSoChuTruongDauTu.Length > 100)
                {
                    isDataWrong = true;
                    IsSoChuTruongDauTuWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số chủ trương đầu tư nhập quá 100 kí tự.");
                }


                sNgayChuTruongDauTu = !string.IsNullOrEmpty(row.Field<string>(4)) ? row.Field<string>(4).Trim() : string.Empty;
                dNgayChuTruongDauTu = CommonFunction.TryParseDateTime(sNgayChuTruongDauTu);
                if (!string.IsNullOrEmpty(sNgayChuTruongDauTu))
                {
                    if (!dNgayChuTruongDauTu.HasValue)
                    {
                        isDataWrong = true;
                        IssNgayChuTruongDauTuWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Ngày chủ trương đầu tư không hợp lệ.");
                    }
                    else
                    {
                        sNgayChuTruongDauTu = dNgayChuTruongDauTu.Value.ToString("dd/MM/yyyy");
                    }
                }

                sSoQuetDinhDauTu = !string.IsNullOrEmpty(row.Field<string>(5)) ? row.Field<string>(5).Trim() : string.Empty;
                if (string.IsNullOrEmpty(sSoQuetDinhDauTu))
                {
                    isDataWrong = true;
                    IsSoQuetDinhDauTuWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số quyết định đầu tư chưa nhập.");
                }
                else if (sSoQuetDinhDauTu.Length > 100)
                {
                    isDataWrong = true;
                    IsSoQuetDinhDauTuWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số quyết định đầu tư nhập quá 100 kí tự.");
                }

                sNgayQuyetDinhDauTu = !string.IsNullOrEmpty(row.Field<string>(6)) ? row.Field<string>(6).Trim() : string.Empty;
                dNgayQuyetDinhDauTu = CommonFunction.TryParseDateTime(sNgayQuyetDinhDauTu);
                if (!string.IsNullOrEmpty(sNgayQuyetDinhDauTu))
                {
                    if (!dNgayQuyetDinhDauTu.HasValue)
                    {
                        isDataWrong = true;
                        IsNgayQuyetDinhDauTuWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Ngày quyết định đầu tư không hợp lệ.");
                    }
                    else
                    {
                        sNgayQuyetDinhDauTu = dNgayQuyetDinhDauTu.Value.ToString("dd/MM/yyyy");
                    }
                }

                sSoDuToan = !string.IsNullOrEmpty(row.Field<string>(7)) ? row.Field<string>(7).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sSoDuToan) && sSoDuToan.Length > 100)
                {
                    isDataWrong = true;
                    IsSoDuToanWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Số dự toán nhập quá 100 kí tự.");
                }

                sNgayDuToan = !string.IsNullOrEmpty(row.Field<string>(8)) ? row.Field<string>(8).Trim() : string.Empty;
                dNgayDuToan = CommonFunction.TryParseDateTime(sNgayDuToan);
                if (!string.IsNullOrEmpty(sNgayDuToan))
                {
                    if (!dNgayDuToan.HasValue)
                    {
                        isDataWrong = true;
                        IsNgayDuToanWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Ngày dự toán không hợp lệ.");
                    }
                    else
                    {
                        sNgayDuToan = dNgayDuToan.Value.ToString("dd/MM/yyyy");
                    }
                }

                sChuDauTu = !string.IsNullOrEmpty(row.Field<string>(9)) ? row.Field<string>(9).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sChuDauTu))
                {
                    dmChuDauTu = chuDauTuList.Where(x => x.sId_CDT.Equals(sChuDauTu)).FirstOrDefault();
                    if (dmChuDauTu == null)
                    {
                        isDataWrong = true;
                        IsChuDauTuWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã chủ đầu tư không tồn tại.");
                    }
                    else
                    {
                        iID_ChuDauTu = dmChuDauTu.ID;
                    }
                }

                sPhanCapPheDuyet = !string.IsNullOrEmpty(row.Field<string>(10)) ? row.Field<string>(10).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sPhanCapPheDuyet))
                {
                    dmPhanCapPheDuyet = phanCapPheDuyetList.Where(x => x.sMa.Equals(sPhanCapPheDuyet)).FirstOrDefault();
                    if (dmPhanCapPheDuyet == null)
                    {
                        isDataWrong = true;
                        IsPhanCapPheDuyetWrong = true;
                        sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Mã phân cấp phê duyệt không tồn tại.");
                    }
                    else
                    {
                        iID_PhanCapPheDuyet = dmPhanCapPheDuyet.ID;
                    }
                }
                sKhoiCong = !string.IsNullOrEmpty(row.Field<string>(11)) ? row.Field<string>(11).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sKhoiCong) && sKhoiCong.Length > 50)
                {
                    isDataWrong = true;
                    IsKhoiCongWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Thời gian thực hiện từ nhập quá 50 kí tự.");
                }

                sKetThuc = !string.IsNullOrEmpty(row.Field<string>(12)) ? row.Field<string>(12).Trim() : string.Empty;
                if (!string.IsNullOrEmpty(sKetThuc) && sKetThuc.Length > 50)
                {
                    isDataWrong = true;
                    IsKetThucWrong = true;
                    sErrorMessage.AppendLine((sErrorMessage.Length > 0 ? "<br/>" : "") + "* Thời gian thực hiện đến nhập quá 50 kí tự.");
                }


                data = new NHDAThongTinDuAnImportModel
                {
                    STT = STT,
                    sMaDuAn = sMaDuAn,
                    IsMaDuAnWrong = IsMaDuAnWrong,
                    sTenDuAn = sTenDuAn,
                    IsTenDuAnWrong = IsTenDuAnWrong,
                    sSoChuTruongDauTu = sSoChuTruongDauTu,
                    IsSoChuTruongDauTuWrong = IsSoChuTruongDauTuWrong,
                    sNgayChuTruongDauTu = sNgayChuTruongDauTu,
                    IssNgayChuTruongDauTuWrong = IssNgayChuTruongDauTuWrong,
                    sSoQuetDinhDauTu = sSoQuetDinhDauTu,
                    IsSoQuetDinhDauTuWrong = IsSoQuetDinhDauTuWrong,
                    sNgayQuyetDinhDauTu = sNgayQuyetDinhDauTu,
                    IsNgayQuyetDinhDauTuWrong = IsNgayQuyetDinhDauTuWrong,
                    sSoDuToan = sSoDuToan,
                    IsSoDuToanWrong = IsSoDuToanWrong,
                    sNgayDuToan = sNgayDuToan,
                    IsNgayDuToanWrong = IsNgayDuToanWrong,
                    sChuDauTu = sChuDauTu,
                    iID_ChuDauTu = iID_ChuDauTu,
                    IsChuDauTuWrong = IsChuDauTuWrong,
                    sPhanCapPheDuyet = sPhanCapPheDuyet,
                    iID_PhanCapPheDuyet = iID_PhanCapPheDuyet,
                    IsPhanCapPheDuyetWrong = IsPhanCapPheDuyetWrong,
                    sKhoiCong = sKhoiCong,
                    IsKhoiCongWrong = IsKhoiCongWrong,
                    sKetThuc = sKetThuc,
                    IsKetThucWrong = IsKetThucWrong,
                    sErrorMessage = sErrorMessage.ToString(),
                    IsDataWrong = isDataWrong,
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
                var wbContractInfo = w_b.Worksheets.Add("Biểu mẫu thông tin dự án");
                var wbChuDauTu = w_b.Worksheets.Add("Chủ đầu tư");
                var wbPhanCapPheDuyet = w_b.Worksheets.Add("Phân cấp phê duyệt");
                wbContractInfo.Column(1).Width = 15;
                wbContractInfo.Column(2).Width = 40;
                wbContractInfo.Column(3).Width = 30;
                wbContractInfo.Column(4).Width = 25;
                wbContractInfo.Column(5).Width = 25;
                wbContractInfo.Column(6).Width = 25;
                wbContractInfo.Column(7).Width = 25;
                wbContractInfo.Column(8).Width = 25;
                wbContractInfo.Column(9).Width = 25;
                wbContractInfo.Column(10).Width = 25;
                wbContractInfo.Column(11).Width = 25;
                wbContractInfo.Column(12).Width = 25;
                wbContractInfo.Column(13).Width = 25;


                wbChuDauTu.Column(1).Width = 15;
                wbChuDauTu.Column(2).Width = 40;
                wbChuDauTu.Column(3).Width = 40;
                wbChuDauTu.Column(4).Width = 50;
                wbChuDauTu.Column(5).Width = 50;
                wbChuDauTu.Column(6).Width = 50;



                wbPhanCapPheDuyet.Column(1).Width = 15;
                wbPhanCapPheDuyet.Column(2).Width = 40;
                wbPhanCapPheDuyet.Column(3).Width = 60;
                wbPhanCapPheDuyet.Column(4).Width = 25;
                wbPhanCapPheDuyet.Column(5).Width = 50;

                //Sheet biểu mẫu
                wbContractInfo.Style.Font.FontName = "Times New Roman";
                wbContractInfo.Style.Font.FontSize = 13;
                wbContractInfo.PageSetup.FitToPages(1, 1);
                wbContractInfo.Row(17).Height = 40;
                wbContractInfo.Cell(2, 1).Value = "BIỂU MẪU IMPORT THÔNG TIN DỰ ÁN";
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
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;
                wbContractInfo.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;
                wbContractInfo.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;
                wbContractInfo.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;
                wbContractInfo.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).DateFormat.SetFormat("dd/MM/yyyy");
                wbContractInfo.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;
                wbContractInfo.Column(11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;
                wbContractInfo.Column(12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;
                wbContractInfo.Column(13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                              .Alignment.SetWrapText(true).NumberFormat.SetFormat("@"); ;

                wbContractInfo.Cell(4, 1).Value = "Mã dự án*: Nhập số, chữ, tối đa 100 kí tự";
                wbContractInfo.Cell(4, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(5, 1).Value = "Tên dự án: Nhập số, chữ, tối đa 300 kí tự";
                wbContractInfo.Cell(5, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(6, 1).Value = "Số chủ trương đầu tư*: Nhập số, chữ, tối đa 100 kí tự";
                wbContractInfo.Cell(6, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(7, 1).Value = "Ngày chủ trương đầu tư: Chỉ nhập định dạng dd/mm/yyyy";
                wbContractInfo.Cell(7, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(8, 1).Value = "Sổ quyết định đầu tư*: Nhập số, chữ, tối đa 100 kí tự";
                wbContractInfo.Cell(8, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(9, 1).Value = "Ngày quyết định đầu tư: Chỉ nhập định dạng dd/mm/yyyy";
                wbContractInfo.Cell(9, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(10, 1).Value = "Số dự toán: Nhập số, chữ, tối đa 100 kí tự";
                wbContractInfo.Cell(10, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(11, 1).Value = "Ngày dự toán: Chỉ nhập định dạng dd/mm/yyyy";
                wbContractInfo.Cell(11, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(12, 1).Value = "Chủ đầu tư: Chỉ điền mã chủ đầu tư đã có trong Sheet Chủ đầu tư";
                wbContractInfo.Cell(12, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(13, 1).Value = "Phân cấp phê duyệt: Chỉ điền mã phân cấp phê duyệt có trong Sheet Phân cấp phê duyệt";
                wbContractInfo.Cell(13, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(14, 1).Value = "Thời gian thực hiện từ: Nhập số, chữ, tối đa 50 kí tự";
                wbContractInfo.Cell(14, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);
                wbContractInfo.Cell(15, 1).Value = "Thời gian thực hiện đến: Nhập số, chữ, tối đa 50 kí tự";
                wbContractInfo.Cell(15, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetWrapText(false);

                wbContractInfo.Cell(17, 1).Value = "STT";
                wbContractInfo.Cell(17, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 2).Value = "Mã dự án";
                wbContractInfo.Cell(17, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 3).Value = "Tên dự án";
                wbContractInfo.Cell(17, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 4).Value = "Số chủ trương đầu tư";
                wbContractInfo.Cell(17, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 5).Value = "Ngày chủ trương đầu tư";
                wbContractInfo.Cell(17, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 6).Value = "Số quyết định đầu tư";
                wbContractInfo.Cell(17, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 7).Value = "Ngày quyết định đầu tư";
                wbContractInfo.Cell(17, 7).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 8).Value = "Sổ dự toán";
                wbContractInfo.Cell(17, 8).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 9).Value = "Ngày dự toán";
                wbContractInfo.Cell(17, 9).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 10).Value = "Chủ đầu tư";
                wbContractInfo.Cell(17, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 11).Value = "Phân cấp phê duyệt";
                wbContractInfo.Cell(17, 11).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 12).Value = "Thời gian thực hiện từ";
                wbContractInfo.Cell(17, 12).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbContractInfo.Cell(17, 13).Value = "Thời gian thực hiện đến";
                wbContractInfo.Cell(17, 13).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                //Sheet chu dau tu
                wbChuDauTu.Style.Font.FontName = "Times New Roman";
                wbChuDauTu.Style.Font.FontSize = 13;
                wbChuDauTu.PageSetup.FitToPages(1, 1);
                wbChuDauTu.Row(4).Height = 30;
                wbChuDauTu.Cell(2, 1).Value = "DANH MỤC CHỦ ĐẦU TƯ";
                wbChuDauTu.Range(2, 1, 2, 6).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbChuDauTu.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbChuDauTu.Cell(4, 1).Value = "STT";
                wbChuDauTu.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbChuDauTu.Cell(4, 2).Value = "Mã chủ đầu tư";
                wbChuDauTu.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbChuDauTu.Cell(4, 3).Value = "Tên chủ đầu tư";
                wbChuDauTu.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbChuDauTu.Cell(4, 4).Value = "Mô tả";
                wbChuDauTu.Cell(4, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbChuDauTu.Cell(4, 5).Value = "Năm làm việc";
                wbChuDauTu.Cell(4, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbChuDauTu.Cell(4, 6).Value = "Kí hiệu";
                wbChuDauTu.Cell(4, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                IEnumerable<DM_ChuDauTu> chuDauTuList = qlnhService.GetLookupChuDauTuu();
                int i = 5;
                int j = 1;
                foreach (DM_ChuDauTu item in chuDauTuList)
                {
                    wbChuDauTu.Cell(i, 1).Value = j;
                    wbChuDauTu.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbChuDauTu.Cell(i, 2).Value = item.sId_CDT;
                    wbChuDauTu.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbChuDauTu.Cell(i, 3).Value = item.sTenCDT;
                    wbChuDauTu.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbChuDauTu.Cell(i, 4).Value = item.sMoTa;
                    wbChuDauTu.Cell(i, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbChuDauTu.Cell(i, 5).Value = item.iNamLamViec;
                    wbChuDauTu.Cell(i, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbChuDauTu.Cell(i, 6).Value = item.sKyHieu;
                    wbChuDauTu.Cell(i, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }

                //Sheet Phan cap phe duyet
                wbPhanCapPheDuyet.Style.Font.FontName = "Times New Roman";
                wbPhanCapPheDuyet.Style.Font.FontSize = 13;
                wbPhanCapPheDuyet.PageSetup.FitToPages(1, 1);
                wbPhanCapPheDuyet.Row(4).Height = 30;
                wbPhanCapPheDuyet.Cell(2, 1).Value = "DANH MỤC PHÂN CẤP PHÊ DUYỆT";
                wbPhanCapPheDuyet.Range(2, 1, 2, 5).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Font.SetBold(true).Alignment.SetWrapText(true).Font.SetFontSize(15);

                wbPhanCapPheDuyet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                             .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                             .Alignment.SetWrapText(true)
                                             .NumberFormat.SetFormat("@");

                wbPhanCapPheDuyet.Cell(4, 1).Value = "STT";
                wbPhanCapPheDuyet.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhanCapPheDuyet.Cell(4, 2).Value = "Mã phân cấp";
                wbPhanCapPheDuyet.Cell(4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhanCapPheDuyet.Cell(4, 3).Value = "Tên phân cấp";
                wbPhanCapPheDuyet.Cell(4, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhanCapPheDuyet.Cell(4, 4).Value = "Tên viết tắt";
                wbPhanCapPheDuyet.Cell(4, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);
                wbPhanCapPheDuyet.Cell(4, 5).Value = "Mô tả";
                wbPhanCapPheDuyet.Cell(4, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                    .Alignment.SetWrapText(true);

                IEnumerable<NH_DM_PhanCapPheDuyet> phanCapPheDuyetList = qlnhService.GetLookupThongTinDuAn();
                i = 5;
                j = 1;
                foreach (NH_DM_PhanCapPheDuyet phanCapPheDuyet in phanCapPheDuyetList)
                {
                    wbPhanCapPheDuyet.Cell(i, 1).Value = j;
                    wbPhanCapPheDuyet.Cell(i, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhanCapPheDuyet.Cell(i, 2).Value = phanCapPheDuyet.sMa;
                    wbPhanCapPheDuyet.Cell(i, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhanCapPheDuyet.Cell(i, 3).Value = phanCapPheDuyet.sTen;
                    wbPhanCapPheDuyet.Cell(i, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhanCapPheDuyet.Cell(i, 4).Value = phanCapPheDuyet.sTenVietTat;
                    wbPhanCapPheDuyet.Cell(i, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    wbPhanCapPheDuyet.Cell(i, 5).Value = phanCapPheDuyet.sMoTa;
                    wbPhanCapPheDuyet.Cell(i, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                                        .Alignment.SetWrapText(true);
                    i++;
                    j++;
                }
                wbContractInfo.Column(2).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(2).AsRange().SetDataValidation().TextLength.Between(0, 100);
                wbContractInfo.Column(2).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorMessage = "Mã dự án vượt quá 100 kí tự. Vui lòng nhập lại!";
                wbContractInfo.Column(2).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 2, 17, 2).SetDataValidation().ClearRanges();

                wbContractInfo.Column(4).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(4).AsRange().SetDataValidation().TextLength.Between(0, 100);
                wbContractInfo.Column(4).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(4).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(4).AsRange().SetDataValidation().ErrorMessage = "Số chủ trương đầu tư vượt quá 100 kí tự. Vui lòng nhập lại!";
                wbContractInfo.Column(4).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 4, 17, 4).SetDataValidation().ClearRanges();

                wbContractInfo.Column(6).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(6).AsRange().SetDataValidation().TextLength.Between(0, 100);
                wbContractInfo.Column(6).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(6).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(6).AsRange().SetDataValidation().ErrorMessage = "Số quyết định đầu tư vượt quá 100 kí tự. Vui lòng nhập lại!";
                wbContractInfo.Column(6).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 6, 17, 6).SetDataValidation().ClearRanges();

                wbContractInfo.Column(8).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(8).AsRange().SetDataValidation().TextLength.Between(0, 100);
                wbContractInfo.Column(8).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(8).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(8).AsRange().SetDataValidation().ErrorMessage = "Số dự toán vượt quá 100 kí tự. Vui lòng nhập lại!";
                wbContractInfo.Column(8).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 8, 17, 8).SetDataValidation().ClearRanges();

                wbContractInfo.Column(12).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(12).AsRange().SetDataValidation().TextLength.Between(0, 50);
                wbContractInfo.Column(12).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(12).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(12).AsRange().SetDataValidation().ErrorMessage = "Thời gian thực hiện từ vượt quá 50 kí tự. Vui lòng nhập lại!";
                wbContractInfo.Column(12).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 12, 17, 12).SetDataValidation().ClearRanges();

                wbContractInfo.Column(13).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.TextLength;
                wbContractInfo.Column(13).AsRange().SetDataValidation().TextLength.Between(0, 50);
                wbContractInfo.Column(13).AsRange().SetDataValidation().ShowErrorMessage = true;
                wbContractInfo.Column(13).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                wbContractInfo.Column(13).AsRange().SetDataValidation().ErrorMessage = "Thời gian thực hiện đến vượt quá 50 kí tự. Vui lòng nhập lại!";
                wbContractInfo.Column(13).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                wbContractInfo.Range(1, 13, 17, 13).SetDataValidation().ClearRanges();

                if (chuDauTuList.Count() > 0)
                {
                    wbContractInfo.Column(10).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(10).AsRange().SetDataValidation().List(wbChuDauTu.Range(5, 2, wbChuDauTu.RowCount(), 2), true);
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ErrorMessage = "Chủ đầu tư không tồn tại!";
                    wbContractInfo.Column(10).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 10, 17, 10).SetDataValidation().ClearRanges();
                }

                if (phanCapPheDuyetList.Count() > 0)
                {
                    wbContractInfo.Column(11).AsRange().SetDataValidation().AllowedValues = XLAllowedValues.List;
                    wbContractInfo.Column(11).AsRange().SetDataValidation().List(wbPhanCapPheDuyet.Range(5, 2, wbPhanCapPheDuyet.RowCount(), 2), true);
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ShowErrorMessage = true;
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ErrorStyle = XLErrorStyle.Stop;
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ErrorMessage = "Mã phân cấp phê duyệt không tồn tại!";
                    wbContractInfo.Column(11).AsRange().SetDataValidation().ErrorTitle = "Lỗi";
                    wbContractInfo.Range(1, 11, 17, 11).SetDataValidation().ClearRanges();
                }

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Biểu mẫu import thông tin dự án.xlsx");
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

        public JsonResult SaveImport(List<NH_DA_DuAn> contractList)
        {
            if (contractList == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không import được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            foreach (var contract in contractList)
            {
                contract.sMaDuAn = HttpUtility.HtmlDecode(contract.sMaDuAn);
                contract.sTenDuAn = HttpUtility.HtmlDecode(contract.sTenDuAn);
                contract.sSoQuyetDinhDauTu = HttpUtility.HtmlDecode(contract.sSoQuyetDinhDauTu);

                contract.dNgayTao = DateTime.Now;
                contract.sNguoiTao = Username;
                contract.bIsActive = true;
                contract.bIsGoc = true;
            }

            if (!qlnhService.SaveImportThongTinDuAn(contractList))
            {
                return Json(new { bIsComplete = false, sMessError = "Không import được dữ liệu !" }, JsonRequestBehavior.AllowGet);
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
                    List<NH_DM_TiGia_ChiTiet> lstMaNgoaiTeKhac = qlnhService.GetNHDMTiGiaChiTietList(idTiGia).ToList();
                    htmlMNTK.AppendFormat("<option value='{0}' selected>{1}</option>", Guid.Empty, "--Chọn mã ngoại tệ khác--");
                    if (lstMaNgoaiTeKhac != null && lstMaNgoaiTeKhac.Count > 0)
                    {
                        for (int i = 0; i < lstMaNgoaiTeKhac.Count; i++)
                        {
                            htmlMNTK.AppendFormat("<option value='{0}'>{1}</option>", lstMaNgoaiTeKhac[i].ID, WebUtility.HtmlEncode(lstMaNgoaiTeKhac[i].sMaTienTeQuyDoi));
                        }
                    }

                    if (giaTriTienData != null)
                    {
                        NH_DM_TiGia tiGia = qlnhService.GetNHDMTiGiaList(idTiGia).ToList().SingleOrDefault();
                        if (tiGia != null)
                        {
                            double? giaTriUSDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriUSD);
                            double? giaTriVNDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriVND);
                            double? giaTriEURInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriEUR);

                            List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                            string maTienTeGoc = tiGia.sMaTienTeGoc;
                            htmlTienTe = GetHtmlTienteQuyDoi(tiGiaChiTietList, maTienTeGoc, null, "");
                            if (MaTienTeList.Contains(maTienTeGoc)) isReadonlyTxtMaNTKhac = true;
                            switch (maTienTeGoc)
                            {
                                case "USD":
                                    if (giaTriUSDInput.HasValue)
                                    {
                                        sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND = true;
                                            }
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
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
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
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
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND = true;
                                            }
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
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
                    isChangeInputEUR = isChangeInputEUR,
                    sGiaTriUSD = sGiaTriUSD,
                    sGiaTriVND = sGiaTriVND,
                    sGiaTriEUR = sGiaTriEUR,
                    isReadonlyTxtMaNTKhac = isReadonlyTxtMaNTKhac
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

                if (idTiGia != null && idTiGia != Guid.Empty
                    && idNgoaiTeKhac != null && !string.IsNullOrEmpty(maNgoaiTeKhac))
                {
                    NH_DM_TiGia tiGia = qlnhService.GetNHDMTiGiaList(idTiGia).ToList().SingleOrDefault();
                    if (tiGia != null)
                    {
                        double? giaTriUSDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriUSD);
                        double? giaTriVNDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriVND);
                        double? giaTriEURInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriEUR);
                        double? giaTriNTKhacInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriNgoaiTeKhac);

                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = qlnhService.GetNHDMTiGiaChiTietList(idTiGia).ToList();
                        string maTienTeGoc = tiGia.sMaTienTeGoc;
                        isReadonlyTxtMaNTKhac = MaTienTeList.Contains(maTienTeGoc);
                        htmlTienTe = GetHtmlTienteQuyDoi(qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList(), maTienTeGoc, idNgoaiTeKhac, maNgoaiTeKhac);
                        if (!maTienTeGoc.Equals(maNgoaiTeKhac))
                        {
                            NH_DM_TiGia_ChiTiet tiGiaChiTiet = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals(maNgoaiTeKhac)).FirstOrDefault();
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
                                            List<NH_DM_TiGia_ChiTiet> tiGiaChiTietTheoMaGocList = qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                                            if (tiGiaChiTietTheoMaGocList != null)
                                            {
                                                NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.Equals("USD")).FirstOrDefault();
                                                if (tiGiaChiTietUSD != null)
                                                {
                                                    double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                                    if (fTiGiaUSD.HasValue)
                                                    {
                                                        sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                        isChangeInputCommon = true;
                                                    }
                                                }

                                                NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.Equals("VND")).FirstOrDefault();
                                                if (tiGiaChiTietVND != null)
                                                {
                                                    double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                                    if (fTiGiaVND.HasValue)
                                                    {
                                                        sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                        isChangeInputCommon = true;
                                                    }
                                                }

                                                NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.Equals("EUR")).FirstOrDefault();
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
        public ActionResult GetListChiPhiThongTinDuAn(Guid? ID, string state)
        {

            return Json(qlnhService.getListChiPhiTTDuAn(ID, state));
        }

        [HttpPost]
        public JsonResult Save(NHDAThongTinDuAnModel data, string state, List<NH_DA_DuAn_ChiPhiDto> dataTableChiPhi, Guid? oldId)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);
            data.sMaChuDauTu = HttpUtility.HtmlDecode(data.sMaChuDauTu);
            data.sTenDuAn = HttpUtility.HtmlDecode(data.sTenDuAn);
            data.sSoChuTruongDauTu = HttpUtility.HtmlDecode(data.sSoChuTruongDauTu);
            data.sSoQuyetDinhDauTu = HttpUtility.HtmlDecode(data.sSoQuyetDinhDauTu);
            data.sSoDuToan = HttpUtility.HtmlDecode(data.sSoDuToan);
            data.sKhoiCong = HttpUtility.HtmlDecode(data.sKhoiCong);
            data.sKetThuc = HttpUtility.HtmlDecode(data.sKetThuc);
            data.sMaNgoaiTeKhac = HttpUtility.HtmlDecode(data.sMaNgoaiTeKhac);

            List<NH_DA_DuAn> lstma = new List<NH_DA_DuAn>();
            lstma = qlnhService.GetMaDuAn(data.iID_MaDonVi, data.sMaChuDauTu);
            int checkma = 0;
            if (lstma.Count() != 0)
            {
                foreach (var item in lstma)
                {
                    var a = item.sMaDuAn.Split('/');
                    int b = Int32.Parse(a[a.Length - 1]);
                    if (b > checkma)
                    {
                        checkma = b;
                    }
                }
            }
            data.sMaDuAn = HttpUtility.HtmlDecode(data.iID_MaDonVi) + "/" + HttpUtility.HtmlDecode(data.sMaChuDauTu) + "/" + (checkma + 1);

            if (!qlnhService.SaveThongTinDuAn(data, Username, state, dataTableChiPhi, oldId))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
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

                if (idTiGia != null && idTiGia != Guid.Empty && giaTriTienData != null)
                {
                    NH_DM_TiGia tiGia = qlnhService.GetNHDMTiGiaList(idTiGia).ToList().SingleOrDefault();
                    if (tiGia != null)
                    {
                        double? giaTriUSDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriUSD);
                        double? giaTriVNDInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriVND);
                        double? giaTriEURInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriEUR);
                        double? giaTriNTKhacInput = CommonFunction.TryParseDouble(giaTriTienData.sGiaTriNgoaiTeKhac);

                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = _ = qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                        string maTienTeGoc = tiGia.sMaTienTeGoc;
                        if (MaTienTeList.Contains(txtBlur))
                        {
                            switch (maTienTeGoc)
                            {
                                case "USD":
                                    if (giaTriUSDInput.HasValue)
                                    {
                                        sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND = true;
                                            }
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
                                        }
                                        if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietNTK = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals(maNgoaiTeKhac)).FirstOrDefault();
                                            if (tiGiaChiTietNTK != null)
                                            {
                                                double? fTiGiaNTK = tiGiaChiTietNTK.fTiGia;
                                                if (fTiGiaNTK.HasValue)
                                                {
                                                    sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriUSDInput.Value * fTiGiaNTK.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputNgoaiTe = true;
                                                }
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
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("EUR")).FirstOrDefault();
                                        if (tiGiaChiTietEUR != null)
                                        {
                                            double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                            if (fTiGiaEUR.HasValue)
                                            {
                                                sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputEUR = true;
                                            }
                                        }
                                        if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietNTK = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals(maNgoaiTeKhac)).FirstOrDefault();
                                            if (tiGiaChiTietNTK != null)
                                            {
                                                double? fTiGiaNTK = tiGiaChiTietNTK.fTiGia;
                                                if (fTiGiaNTK.HasValue)
                                                {
                                                    sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriVNDInput.Value * fTiGiaNTK.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputNgoaiTe = true;
                                                }
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
                                        NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("VND")).FirstOrDefault();
                                        if (tiGiaChiTietVND != null)
                                        {
                                            double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                            if (fTiGiaVND.HasValue)
                                            {
                                                sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                isChangeInputVND = true;
                                            }
                                        }

                                        NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals("USD")).FirstOrDefault();
                                        if (tiGiaChiTietUSD != null)
                                        {
                                            double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                            if (fTiGiaUSD.HasValue)
                                            {
                                                sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                isChangeInputUSD = true;
                                            }
                                        }
                                        if (idNgoaiTeKhac != null && idNgoaiTeKhac != Guid.Empty)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietNTK = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals(maNgoaiTeKhac)).FirstOrDefault();
                                            if (tiGiaChiTietNTK != null)
                                            {
                                                double? fTiGiaNTK = tiGiaChiTietNTK.fTiGia;
                                                if (fTiGiaNTK.HasValue)
                                                {
                                                    sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriEURInput.Value * fTiGiaNTK.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputNgoaiTe = true;
                                                }
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
                            NH_DM_TiGia_ChiTiet tiGiaChiTiet = tiGiaChiTietList.Where(x => x.sMaTienTeQuyDoi.Equals(maNgoaiTeKhac)).FirstOrDefault();
                            if (tiGiaChiTiet != null)
                            {
                                double? fTiGia = tiGiaChiTiet.fTiGia;
                                if (giaTriNTKhacInput.HasValue)
                                {
                                    if (fTiGia.HasValue)
                                    {
                                        sGiaTriNTKhac = CommonFunction.DinhDangSo(Math.Round(giaTriNTKhacInput.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                        double fTienMaGoc = giaTriNTKhacInput.Value / fTiGia.Value;
                                        List<NH_DM_TiGia_ChiTiet> tiGiaChiTietTheoMaGocList = qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
                                        if (tiGiaChiTietTheoMaGocList != null)
                                        {
                                            NH_DM_TiGia_ChiTiet tiGiaChiTietUSD = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.Equals("USD")).FirstOrDefault();
                                            if (tiGiaChiTietUSD != null)
                                            {
                                                double? fTiGiaUSD = tiGiaChiTietUSD.fTiGia;
                                                if (fTiGiaUSD.HasValue)
                                                {
                                                    sGiaTriUSD = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaUSD.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputUSD = true;
                                                }
                                            }

                                            NH_DM_TiGia_ChiTiet tiGiaChiTietVND = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.Equals("VND")).FirstOrDefault();
                                            if (tiGiaChiTietVND != null)
                                            {
                                                double? fTiGiaVND = tiGiaChiTietVND.fTiGia;
                                                if (fTiGiaVND.HasValue)
                                                {
                                                    sGiaTriVND = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaVND.Value).ToString(CultureInfo.InvariantCulture), 0);
                                                    isChangeInputVND = true;
                                                }
                                            }

                                            NH_DM_TiGia_ChiTiet tiGiaChiTietEUR = tiGiaChiTietTheoMaGocList.Where(x => x.sMaTienTeQuyDoi.Equals("EUR")).FirstOrDefault();
                                            if (tiGiaChiTietEUR != null)
                                            {
                                                double? fTiGiaEUR = tiGiaChiTietEUR.fTiGia;
                                                if (fTiGiaEUR.HasValue)
                                                {
                                                    sGiaTriEUR = CommonFunction.DinhDangSo(Math.Round(fTienMaGoc * fTiGiaEUR.Value, 2).ToString(CultureInfo.InvariantCulture), 2);
                                                    isChangeInputEUR = true;
                                                }
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