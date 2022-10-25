using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.KeHoachChiTietBQP;
using Viettel.Models.Shared;
using Viettel.Services;
using VIETTEL.Helpers;
using VIETTEL.Controllers;
using Viettel.Extensions;
using Newtonsoft.Json;
using System.Globalization;
using DomainModel;
using VIETTEL.Models.QLNH;
using VIETTEL.Models;
using Viettel.Models.QLNH;

namespace VIETTEL.Areas.QLNH.Controllers.DuAnHopDong
{
    public class KeHoachChiTietBQPController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        // GET: QLNH/KeHoachChiTietBQP
        public ActionResult Index()
        {
            var result = new NH_KHChiTietBQPViewModel();
            result = _qlnhService.getListKHChiTietBQP(result._paging, null, null, null, null);
            return View(result);
        }

        // Tìm kiếm
        public ActionResult TimKiem(NH_KHChiTietBQPFilter input, PagingInfo paging)
        {
            if (paging == null)
            {
                paging = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = Constants.ITEMS_PER_PAGE
                };
            }
            input.SoKeHoach = HttpUtility.HtmlDecode(input.SoKeHoach);
            var result = _qlnhService.getListKHChiTietBQP(paging, input.SoKeHoach, input.NgayBanHanh, input.GiaiDoanTu, input.GiaiDoanDen);
            return PartialView("_list", result);
        }

        [HttpPost]
        // Get modal thêm, sửa, điều chỉnh
        public ActionResult GetModal(Guid? id, string state)
        {
            NH_KHChiTietBQPModel result = new NH_KHChiTietBQPModel();
            var lstKHBQP = new List<LookupKHBQP>();
            if (id.HasValue && id != Guid.Empty)
            {
                result = _qlnhService.GetKeHoachChiTietBQPById(id.Value);

                // Nếu trạng thái là điều chỉnh thì check TTCP có đang active
                if (state == "ADJUST" && result.iID_KHTongTheTTCPID.HasValue)
                {
                    var checker = _qlnhService.CheckKHTongTheTTCPIsActive(result.iID_KHTongTheTTCPID.Value);
                    ViewBag.ParentIsActive = checker.HasValue ? checker.Value : true;
                }
                else
                {
                    ViewBag.ParentIsActive = true;
                }
            }
            else
            {
                result.dNgayKeHoach = DateTime.Now;
            }

            ViewBag.State = state;
            //ViewBag.ListKHChiTietBQP = _qlnhService.getLookupKHBQP().ToSelectList("Id", "DisplayName");
            ViewBag.ListKHChiTietBQP = lstKHBQP;
            ViewBag.ListKHTongTheTTCP = _qlnhService.getLookupKHTTCP().ToList();

            var lstTiGia = _qlnhService.GetNHDMTiGiaList().ToList();
            lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
            ViewBag.ListTiGia = lstTiGia;

            return PartialView("_modalCreateOrUpdate", result);
        }

        // Lưu
        public ActionResult SaveKeHoachChiTietBQP(List<NH_KHChiTietBQP_NhiemVuChiCreateDto> lstNhiemVuChis, string keHoachChiTietBQP, string state)
        {
            keHoachChiTietBQP = HttpUtility.HtmlDecode(keHoachChiTietBQP);
            foreach(var item in lstNhiemVuChis)
            {
                item.iID_MaDonVi = HttpUtility.HtmlDecode(item.iID_MaDonVi);
                item.sTenNhiemVuChi = HttpUtility.HtmlDecode(item.sTenNhiemVuChi);
            }
            var khct = JsonConvert.DeserializeObject<NH_KHChiTietBQP>(keHoachChiTietBQP);
            return Json(new {
                result = _qlnhService.SaveKHBQP(lstNhiemVuChis, khct, state)
            });
        }

        // Get chi tiết tỉ giá theo id
        public string ChangeTiGia(Guid? tiGiaID)
        {
            if (tiGiaID.HasValue && tiGiaID != Guid.Empty)
            {
                NH_DM_TiGia tiGia = _qlnhService.GetNHDMTiGiaList(tiGiaID).ToList().SingleOrDefault();
                List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList = _qlnhService.GetNHDMTiGiaChiTietList(tiGiaID, false).ToList();
                return GetHtmlTienteQuyDoi(tiGiaChiTietList, tiGia.sMaTienTeGoc);
            }
            else
            {
                return "";
            }
        }

        public Boolean DeleteKeHoachChiTietBQP(Guid id)
        {
            return _qlnhService.DeleteKHBQP(id);
        }

        // View chi tiết kế hoạch chi tiết BQP
        public ActionResult ViewDetailKHChiTietBQP(NH_KHChiTietBQPModel input, string state, bool isUseLastTTCP = false)
        {
            input.sSoKeHoach = HttpUtility.HtmlDecode(input.sSoKeHoach);
            input.sMoTaChiTiet = HttpUtility.HtmlDecode(input.sMoTaChiTiet);
            if (state != "DETAIL")
            {
                // Check loại kế hoạch:
                // iLoai = 1: Theo giai đoạn
                if (input.iLoai == 1)
                {
                    input.iID_ParentID = null;
                    input.iNamKeHoach = null;
                }
                // iLoai = 2: Theo năm
                else if (input.iLoai == 2)
                {
                    input.iGiaiDoanDen = null;
                    input.iGiaiDoanTu = null;

                    if (input.iLoaiTTCP == 2)
                    {
                        input.iID_ParentID = null;
                    }
                }
                // iLoai = 3: Theo giai đoạn con
                else
                {
                    // Chuyển về theo giai đoạn
                    input.iLoai = 1;
                    input.iNamKeHoach = null;
                }

                // Check state cập nhật thông tin user.
                // Chỉnh sửa
                if (state == "UPDATE")
                {
                    input.dNgaySua = DateTime.Now;
                    input.sNguoiSua = Username;
                }
                // Thêm mới hoặc điều chỉnh
                else
                {
                    input.dNgayTao = DateTime.Now;
                    input.sNguoiTao = Username;
                }

                ViewBag.KHChiTietBQP = JsonConvert.SerializeObject(input);
                ViewBag.LookupDonVi = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            }

            var result = _qlnhService.GetDetailKeHoachChiTietBQP(state, input.iID_KHTongTheTTCPID, input.ID, input.iID_BQuanLyID, input.iID_DonViID, isUseLastTTCP);
            result.State = state;

            if (state == "DETAIL")
            {
                // Nếu trạng thái là xem chi tiết thì hiển thị view chi tiết, không được chỉnh sửa.
                result.IsEdit = false;

                var lstDonViQuanLy = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "-- Chọn đơn vị --" });
                ViewBag.LookupDonVi = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

                var lstPhongBan = _qlnhService.getLookupPhongBan().ToList();
                lstPhongBan.Insert(0, new LookupDto<Guid, string> { Id = Guid.Empty, DisplayName = "-- Chọn B quản lý --" });
                ViewBag.LookupPhongBan = lstPhongBan.ToSelectList("Id", "DisplayName");
            }
            else
            {
                // Nếu trạng thái là thêm mới, sửa, điều chỉnh thì hiển thị view chi tiết edit, được chỉnh sửa. Cập nhật thêm 1 số thông đã chỉnh sửa ở màn trước.
                result.IsEdit = true;
                result.iLoai = input.iLoai;
                result.iGiaiDoanTu = input.iGiaiDoanTu;
                result.iGiaiDoanDen = input.iGiaiDoanDen;
                result.iNamKeHoach = input.iNamKeHoach;
                result.sSoKeHoachBQP = input.sSoKeHoach;
                result.dNgayKeHoachBQP = input.dNgayKeHoach;

                if (input.iID_TiGiaID.HasValue)
                {
                    var lstTiGiaChiTiet = _qlnhService.GetTiGiaChiTietByTiGiaId(input.iID_TiGiaID.Value);
                    var checkTiGiaFromVND = lstTiGiaChiTiet.FirstOrDefault(x => x.sMaTienTeGoc.Trim().ToUpper() == "VND");
                    ViewBag.IsVNDToUSD = checkTiGiaFromVND != null;
                }
                else
                {
                    ViewBag.ListTiGiaChiTiet = new List<NH_DM_TiGia_ChiTiet_ViewModel>();
                    ViewBag.IsVNDToUSD = false;
                }
                
            }

            return View(result);
        }

        // Lấy lookup đơn vị, tỉ giá màn chi tiết
        [HttpPost]
        public ActionResult GetDataLookupDetail(Guid iID_TiGiaID)
        {
            var lstDonVi = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            var tiGiaChiTiet = _qlnhService.GetTiGiaChiTietByTiGiaId(iID_TiGiaID);
            return Json(new { 
                ListDonVi = lstDonVi,
                ListTiGiaChiTiet = tiGiaChiTiet
            });
        }

        [HttpPost]
        public ActionResult CalcMoneyByTiGia(string number, string numTiGia)
        {
            decimal result = 0;
            var money = decimal.TryParse(number, NumberStyles.Float, new CultureInfo("en-US"), out decimal num) ? num : 0;
            var tiGia = decimal.TryParse(numTiGia, NumberStyles.Float, new CultureInfo("en-US"), out decimal tg) ? tg : 0;
            result = money * tiGia;

            return Json(new {
                result = result.ToString("0.00" + new string('#', 397), new CultureInfo("en-US"))
            });
        }

        [HttpPost]
        public ActionResult CalcListMoneyByTiGia(List<CalcTiGiaModel> datas, string numTiGia)
        {
            var tiGia = decimal.TryParse(numTiGia, NumberStyles.Float, new CultureInfo("en-US"), out decimal tg) ? tg : 0;

            foreach (var item in datas)
            {
                var money = decimal.TryParse(item.sMoney, NumberStyles.Float, new CultureInfo("en-US"), out decimal num) ? num : 0;
                item.dResult = money * tiGia;
            }

            return Json(new
            {
                result = datas
            });
        }

        [HttpPost]
        public ActionResult SumTwoList(List<string> lstNumVND, List<string> lstNumUSD)
        {
            decimal resultVND = 0;
            decimal resultUSD = 0;
            if (lstNumVND != null)
            {
                foreach (var num in lstNumVND)
                {
                    resultVND += decimal.TryParse(num, NumberStyles.Float, new CultureInfo("en-US"), out decimal nf) ? nf : 0;
                }
            }

            if (lstNumUSD != null)
            {
                foreach (var num in lstNumUSD)
                {
                    resultUSD += decimal.TryParse(num, NumberStyles.Float, new CultureInfo("en-US"), out decimal nf) ? nf : 0;
                }
            }
            
            return Json(new
            {
                resultVND = CommonFunction.DinhDangSo(resultVND.ToString("0." + new string('#', 399 ), new CultureInfo("en-US"))),
                resultUSD = CommonFunction.DinhDangSo(resultUSD.ToString("0.00" + new string('#', 397), new CultureInfo("en-US"))),
            });
        }

        // Get modal confirm save nhiệm vụ chi khi có giá trị BQP > giá trị TTCP
        [HttpPost]
        public ActionResult ShowModalConfirmSave(string state, string title, List<string> messages)
        {
            var model = new ModalModels();
            model.Title = title;
            model.Messages = messages;
            model.Category = 0;
            return PartialView("_modalConfirmSaveNVC", model);
        }

        [HttpPost]
        public ActionResult GetLookupBQPByTTCPId(Guid? id)
        {
            var result = new List<LookupKHBQP>();
            if (id.HasValue)
            {
                result = _qlnhService.getLookupKHBQPByKHTTCPId(id.Value).ToList();

            }
            return Json(result);
        }

        // Get chi tiết tỉ giá theo id
        private string GetHtmlTienteQuyDoi(List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList, string maTienGoc)
        {
            StringBuilder htmlTienTe = new StringBuilder();
            var lstMaTienTe = new List<string>();
            lstMaTienTe.Add("VND");
            lstMaTienTe.Add("USD");

            foreach (var tgct in tiGiaChiTietList)
            {
                // Nếu mã tiền gốc là VND hoặc USD thì check trong tỉ giá chi tiết xem mã tiền tệ quy đổi có chứa VND or USD?
                if (tgct.fTiGia.HasValue && lstMaTienTe.Contains(maTienGoc.ToUpper().Trim()))
                {
                    if (lstMaTienTe.Contains(tgct.sMaTienTeQuyDoi.ToUpper().Trim()))
                    {
                        htmlTienTe.AppendFormat("1 {0} = {1} {2}; ", HttpUtility.HtmlEncode(maTienGoc.Trim()), tgct.fTiGia.Value.ToString("0." + new string('#', 339)), HttpUtility.HtmlEncode(tgct.sMaTienTeQuyDoi.Trim()));
                    }
                }
                // Nếu mã tiền gốc không phải là VND or USD thì check thêm mà tiền tệ quy đổi có VND or USD?
                else if (tgct.fTiGia.HasValue && !lstMaTienTe.Contains(maTienGoc.ToUpper().Trim()) && lstMaTienTe.Contains(tgct.sMaTienTeQuyDoi.ToUpper().Trim()))
                {
                    htmlTienTe.AppendFormat("1 {0} = {1} {2}; ", HttpUtility.HtmlEncode(maTienGoc.Trim()), tgct.fTiGia.Value.ToString("0." + new string('#', 339)), HttpUtility.HtmlEncode(tgct.sMaTienTeQuyDoi.Trim()));
                }
            }

            return htmlTienTe.ToString();
        }
    }
}