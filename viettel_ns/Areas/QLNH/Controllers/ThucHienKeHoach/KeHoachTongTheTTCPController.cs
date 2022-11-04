using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Models.Shared;
using Viettel.Services;
using VIETTEL.Areas.QLNH.Models.DuAnHopDong.KeHoachTongTheTTCP;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNH.Controllers.ThucHienKeHoach
{
    public class KeHoachTongTheTTCPController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        // GET: QLNH/KeHoachTongTheTTCP
        public ActionResult Index()
        {
            var result = new NH_KHTongTheTTCPViewModel();
            result = _qlnhService.getListKHTongTheTTCP(result._paging, null, null, null, null);
            return View(result);
        }

        // Filter list kế hoạch tổng thể TTCP
        public ActionResult ListPage(NH_KHTongTheTTCPFilter input, PagingInfo paging)
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
            var result = _qlnhService.getListKHTongTheTTCP(paging, input?.SoKeHoach, input?.NgayBanHanh, input?.GiaiDoanTu, input?.GiaiDoanDen);
            return PartialView(result);
        }

        // Get model create, edit, adjust TTCP
        [HttpPost]
        public ActionResult GetDetail(Guid? id, string state)
        {
            NH_KHTongTheTTCP result = new NH_KHTongTheTTCP();
            if (id.HasValue && id != Guid.Empty)
            {
                result = _qlnhService.Get_KHTT_TTCP_ById(id.Value);

                // Nếu trạng thái là điều chỉnh và có parent thì check xem parent có đang active
                if (state == "ADJUST" && result.iID_ParentID.HasValue)
                {
                    var checker = _qlnhService.CheckKHTongTheTTCPIsActive(result.iID_ParentID.Value);
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
            ViewBag.ListKHTongTheTTCP = _qlnhService.getLookupKHTTCPByStage().ToList();

            return PartialView(result);
        }

        #region Kế hoạch chi tiết TTCP
        // Get list nhiệm vụ chi
        public ActionResult NhiemVuChiDetail(NH_KHTongTheTTCPModel input, string state, bool isUseLastTTCP = false)
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

                // Lưu giá trị Kế hoạch TTCP
                ViewBag.KHTongTheTTCP = JsonConvert.SerializeObject(input);
                TempData["KHTongTheTTCP"] = input;
                // Get lookup phòng ban
                ViewBag.LookupPhongBan = _qlnhService.getLookupPhongBan().ToList();
            }

            // Lấy thông tin TTCP and NVC
            NH_KHTongTheTTCP_NVCViewModel result;
            if (state == "DETAIL")
            {
                result = _qlnhService.GetDetailKeHoachTongTheTTCP(state, state == "CREATE" || (state == "ADJUST" && isUseLastTTCP) ? input.iID_ParentID : input.ID, input.iID_BQuanLyID, "", "");
                result.State = state;
                // Nếu trạng thái là xem chi tiết thì hiển thị view chi tiết, không được chỉnh sửa.
                result.IsEdit = false;

                // Get lookup filter phòng ban
                var lstPhongBan = _qlnhService.getLookupPhongBan().ToList();
                lstPhongBan.Insert(0, new LookupDto<Guid, string> { Id = Guid.Empty, DisplayName = "-- Chọn B quản lý --" });
                ViewBag.LookupPhongBan = lstPhongBan.ToSelectList("Id", "DisplayName");
                return View(result);
            }
            else
            {
                result = _qlnhService.GetDetailKeHoachTongTheTTCP(state, state == "CREATE" || (state == "ADJUST" && isUseLastTTCP) ? input.iID_ParentID : input.ID, input.iID_BQuanLyID, "", "");
                result.State = state;
                // Nếu trạng thái là thêm mới, sửa, điều chỉnh thì hiển thị view chi tiết edit, được chỉnh sửa. Cập nhật thêm 1 số thông đã chỉnh sửa ở màn trước.
                result.IsEdit = true;
                result.iLoai = input.iLoai;
                result.iGiaiDoanTu = input.iGiaiDoanTu;
                result.iGiaiDoanDen = input.iGiaiDoanDen;
                result.iNamKeHoach = input.iNamKeHoach;
                result.sSoKeHoach = input.sSoKeHoach;
                result.dNgayKeHoach = input.dNgayKeHoach;
                KeHoachTongTheTTCPChiTietGridViewModel KHTTGridView = new KeHoachTongTheTTCPChiTietGridViewModel();
                KHTTGridView.NH_KHTongTheTTCP_NVCViewModel = result;
                KHTTGridView.NH_KHTongTheTTCPModel = input;
                KHTTGridView.IsUseLastTTCP = isUseLastTTCP;
                return PartialView("_sheet", KHTTGridView);
            }
        }

        // Lưu data TTCP and NVC
        public ActionResult SaveKHTongTheTTCP(List<NH_KHTongTheTTCP_NhiemVuChiDto> lstNhiemVuChis, string state)
        {
            foreach (var item in lstNhiemVuChis)
            {
                item.sTenNhiemVuChi = HttpUtility.HtmlDecode(item.sTenNhiemVuChi);
            }

            if (TempData["KHTongTheTTCP"] != null)
            {
                TempData.Keep("KHTongTheTTCP");
            }

            var khct = (NH_KHTongTheTTCP)TempData["KHTongTheTTCP"];
            return Json(new { 
                result = _qlnhService.SaveKHTongTheTTCP(lstNhiemVuChis, khct, state)
            });
        }

        // Tìm kế hoạch TTCP đang active từ 1 id TTCP chưa active
        public ActionResult FindParentTTCPActive(Guid id)
        {
            var result = _qlnhService.FindParentTTCPActive(id);
            return Json(new
            {
                result = result
            }); 
        }

        // Lấy lookup đơn vị, tỉ giá màn chi tiết
        [HttpPost]
        public ActionResult GetDataLookupDetail()
        {
            var lstBQuanLy = _qlnhService.getLookupPhongBan().ToList();
            return Json(new
            {
                ListPhongBan = lstBQuanLy,
            });
        }
        #endregion

        #region Not used

        [HttpPost]
        public JsonResult KH_TTCPSave(NH_KHTongTheTTCP data)
        {
            if (data.ID == Guid.Empty)
            {
                if (!_qlnhService.SaveKeHoachTTCP(data, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không thêm mới được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var detail = _qlnhService.Get_KHTT_TTCP_ById(data.ID);
                detail.iLoai = data.iLoai;
                detail.iGiaiDoanTu = data.iGiaiDoanTu;
                detail.iGiaiDoanDen = data.iGiaiDoanDen;
                detail.dNgayKeHoach = data.dNgayKeHoach;
                detail.iNamKeHoach = data.iNamKeHoach;
                detail.iID_ParentID = data.iID_ParentID;
                detail.sSoKeHoach = data.sSoKeHoach;
                detail.sMoTaChiTiet = data.sMoTaChiTiet;
                if (!_qlnhService.SaveKeHoachTTCP(detail, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult KH_TTCPDelete(Guid id)
        {
            if (!_qlnhService.DeleteKeHoachTTCP(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PlanDetailList(Guid khtt_id, Guid program_id)
        {
            var model = new KeHoachChiTietTTCPViewModel();
            model.Parents = _qlnhService.Get_KHCT_TTCP_GetListOfParent(khtt_id, program_id);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AddNew_NVC(Guid khtt_id)
        {
            var model = new KeHoachChiTietTTCPViewModel();
            model.DetailKHTT = _qlnhService.Get_KHTT_TTCP_ById(khtt_id);
            model.ListPhongBan = _qlnhService.GetListDM_BQL();
            model.ListSoKeHoach = _qlnhService.GetListKHTT_ActiveWithNumber();
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult Detail_NVC(Guid? NVC_ID, Guid khtt_id)
        {
            var model = new KeHoachChiTietTTCPViewModel();
            if (NVC_ID != null)
            {
                model.DetailKHCT = _qlnhService.GetNhiemVuChiById(NVC_ID.Value);
                model.DetailKHTT = _qlnhService.Get_KHTT_TTCP_ById(model.DetailKHCT.iID_KHTongTheID.Value);

            }
            else
            {
                model.DetailKHTT = _qlnhService.Get_KHTT_TTCP_ById(khtt_id);
            }
            model.ListPhongBan = _qlnhService.GetListDM_BQL();
            model.ListSoKeHoach = _qlnhService.GetListKHTT_ActiveWithNumber();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Detail_NVC_AddChild(Guid? NVC_ID, Guid khtt_id)
        {
            var model = new KeHoachChiTietTTCPViewModel();
            if (NVC_ID != null)
            {
                model.DetailKHCT = _qlnhService.GetNhiemVuChiById(NVC_ID.Value);
                model.DetailKHTT = _qlnhService.Get_KHTT_TTCP_ById(model.DetailKHCT.iID_KHTongTheID.Value);

            }
            else
            {
                model.DetailKHTT = _qlnhService.Get_KHTT_TTCP_ById(khtt_id);
            }
            model.ListPhongBan = _qlnhService.GetListDM_BQL();
            model.ListSoKeHoach = _qlnhService.GetListKHTT_ActiveWithNumber();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult KH_TTCP_NVC_Save(NH_KHTongTheTTCP_NhiemVuChi data, double? tongGiaTri)
        {
            try
            {
                if (data.ID == Guid.Empty)
                {
                    if (!_qlnhService.SaveKeHoachTTCP_NVC(data, Username))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không thêm mới được dữ liệu NVC !" }, JsonRequestBehavior.AllowGet);
                    }
                    if (tongGiaTri != null)
                    {
                        var detail_khtt = _qlnhService.Get_KHTT_TTCP_ById(data.ID);
                        detail_khtt.fTongGiaTri = tongGiaTri;
                        if (!_qlnhService.SaveKeHoachTTCP(detail_khtt, Username))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Không cập nhật tổng giá trị NVC này !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var detail = _qlnhService.GetNhiemVuChiById(data.ID);
                    detail.iID_KHTongTheID = data.iID_KHTongTheID;
                    detail.sTenNhiemVuChi = data.sTenNhiemVuChi;
                    detail.iID_BQuanLyID = data.iID_BQuanLyID;
                    detail.fGiaTri = data.fGiaTri;
                    detail.sMaThuTu = data.sMaThuTu;
                    detail.iID_ParentID = data.iID_ParentID;
                    if (!_qlnhService.SaveKeHoachTTCP_NVC(detail, Username))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                    if (tongGiaTri != null)
                    {
                        var detail_khtt = _qlnhService.Get_KHTT_TTCP_ById(detail.iID_KHTongTheID.Value);
                        detail_khtt.fTongGiaTri = tongGiaTri;
                        if (!_qlnhService.SaveKeHoachTTCP(detail_khtt, Username))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Không cập nhật tổng giá trị NVC này !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
        }
        [HttpPost]
        public JsonResult KH_TTCP_NVC_AddChild_Save(NH_KHTongTheTTCP_NhiemVuChi data, double? tongGiaTri)
        {
            try
            {
                if (!_qlnhService.SaveKeHoachTTCP_NVC(data, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không thêm mới được dữ liệu NVC con!" }, JsonRequestBehavior.AllowGet);
                }
                if (tongGiaTri != null)
                {
                    var detail_khtt = _qlnhService.Get_KHTT_TTCP_ById(data.iID_KHTongTheID.Value);
                    detail_khtt.fTongGiaTri = tongGiaTri;
                    if (!_qlnhService.SaveKeHoachTTCP(detail_khtt, Username))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật tổng giá trị NVC này !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
        }

        #endregion

        public ActionResult SheetFrame(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan, string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new KeHoachTongTheTTCP_ChiTIet_SheetTable(state, KHTTCP_ID, iID_BQuanLyID, sTenNhiemVuChi, sTenPhongBan, filters);
            var kHTongTheTTCP_NVCViewModel = _qlnhService.GetDetailKeHoachTongTheTTCP(state, KHTTCP_ID, iID_BQuanLyID, sTenNhiemVuChi, sTenPhongBan);
            kHTongTheTTCP_NVCViewModel.State = state;
            var vm = new KeHoachTongTheTTCPChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "KeHoachTongTheTTCP", new { area = "QLNH" }),
                   urlGet: Url.Action("SheetFrame", "KeHoachTongTheTTCP", new { area = "QLNH" })
                   ),
                NH_KHTongTheTTCP_NVCViewModel = kHTongTheTTCP_NVCViewModel

            };
            if (TempData["KHTongTheTTCP"] != null)
            {
                TempData.Keep("KHTongTheTTCP");
            }
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return PartialView("_sheetFrame", vm);
        }
    }
}