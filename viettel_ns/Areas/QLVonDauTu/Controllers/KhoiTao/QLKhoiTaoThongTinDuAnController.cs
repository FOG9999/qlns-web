using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.KhoiTao
{
    public class QLKhoiTaoThongTinDuAnController : AppController
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;

        // GET: QLVonDauTu/QLKhoiTaoThongTinDuAn
        public ActionResult Index()
        {
            KhoiTaoThongTinDuAnViewModel vm = new KhoiTaoThongTinDuAnViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qLVonDauTuService.GetAllKhoiTaoThongTinDuAn(ref vm._paging);

            return View(vm);
        }

        [HttpPost]
        public ActionResult KhoiTaoTTDAListView(PagingInfo _paging, int? iNamKhoiTao, string sTenDonVi)
        {
            KhoiTaoThongTinDuAnViewModel vm = new KhoiTaoThongTinDuAnViewModel();
            vm._paging = _paging;
            vm.Items = _qLVonDauTuService.GetAllKhoiTaoThongTinDuAn(ref vm._paging, iNamKhoiTao, sTenDonVi);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            VDT_KT_KhoiTao_DuLieu_ViewModel data = new VDT_KT_KhoiTao_DuLieu_ViewModel();
            if (id.HasValue)
            {
                data = _qLVonDauTuService.GetKhoiTaoTTDAById(id.Value);
            }
            else
            {
                data.iNamKhoiTao = DateTime.Now.Year;
                data.dNgayKhoiTao = DateTime.Now;
            }

            List<NS_DonVi> lstDonViQL = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn--" });
            ViewBag.ListDonViQL = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            VDT_KT_KhoiTao_DuLieu_ViewModel data = _qLVonDauTuService.GetKhoiTaoTTDAById(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool DeleteKhoiTaoTTDA(Guid id)
        {
            return _qLVonDauTuService.DeleteKhoiTaoTTDA(id);
        }

        [HttpPost]
        public JsonResult KhoiTaoTTDASave(VDT_KT_KhoiTao_DuLieu data)
        {
            Guid iID_KhoiTao = Guid.Empty;
            string sMaDonVi = string.Empty;
            if (!_qLVonDauTuService.SaveKhoiTaoTTDA(ref iID_KhoiTao, ref sMaDonVi, data, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true, iID_KhoiTao = iID_KhoiTao, sMaDonVi = sMaDonVi }, JsonRequestBehavior.AllowGet);
        }

        #region Chi tiết khởi tạo TTDA
        public ActionResult Detail(Guid iID_KhoiTao, /*Guid iID_DonViQL*/ string sMaDonVi)
        {
            KhoiTaoThongTinDuAnChiTietViewModel vm = new KhoiTaoThongTinDuAnChiTietViewModel();
            vm._paging.CurrentPage = 1;
            vm.iID_KhoiTaoID = iID_KhoiTao;
            vm.sMaDonVi = sMaDonVi;
            vm.Items = _qLVonDauTuService.GetKhoiTaoTTDAChiTietByIdKhoiTao(iID_KhoiTao);
            TempData["KhoiTaoChiTiet"] = vm.Items;
            List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.GetDuAnByMaDonViQL(sMaDonVi).ToList();
            List<VDTDuAnDropViewModel> lstDuAnShow = new List<VDTDuAnDropViewModel>();
            foreach (var item in lstDuAn)
            {
                lstDuAnShow.Add(new VDTDuAnDropViewModel
                {
                    iID_DuAnID = item.iID_DuAnID,
                    sTenDuAn = item.sTenDuAn,
                    sMaDuAn = item.sMaDuAn,
                    iID_DonViQuanLyID = item.iID_DonViQuanLyID,
                    iID_ChuDauTuID = item.iID_ChuDauTuID,
                    iID_DonViThucHienDuAnID = item.iID_DonViThucHienDuAnID,
                    iID_LoaiDuAnId = item.iID_LoaiDuAnId,
                    iID_MaDonVi = item.iID_MaDonVi,
                    iID_MaCDT = item.iID_MaCDT,
                    iID_NhomDuAnID= item.iID_NhomDuAnID
                });
            }
            lstDuAnShow.Insert(0, new VDTDuAnDropViewModel { iID_DuAnID = Guid.Empty, sTenDuAn = "--Chọn--" });
            ViewBag.ListDuAn = lstDuAnShow.ToSelectList("iID_DuAnID", "sTenDuAnShow");

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = _qLVonDauTuService.GetListHopDongKhoiTaoTTDAByKhoiTaoID(iID_KhoiTao).ToList();
            TempData["TotalHopDongKhoiTao"] = lstTotalHopDong;

            return View(vm);
        }

        [HttpPost]
        public ActionResult ChiTietKhoiTaoTTDAListView(byte type, int STT, Guid iID_KhoiTao, /*Guid iID_DonViQL*/ string sMaDonVi)
        {
            KhoiTaoThongTinDuAnChiTietViewModel vm = new KhoiTaoThongTinDuAnChiTietViewModel();
            vm._paging.CurrentPage = 1;
            vm.iID_KhoiTaoID = iID_KhoiTao;
            vm.sMaDonVi = sMaDonVi;
            var lstData = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel>();
            if (TempData["KhoiTaoChiTiet"] != null)
            {
                lstData = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel>)TempData["KhoiTaoChiTiet"];
            }
            //type = 0: Xóa
            if (type == 0) {
                var itemRemove = lstData.SingleOrDefault(x => x.STT == STT);
                lstData.Remove(itemRemove);
            }
            //Thêm dòng
            if (type == 1)
            {
                var itemAdd = new VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel
                {
                    iID_KhoiTao_ChiTietID = Guid.NewGuid(),
                    STT = lstData.LastOrDefault() != null ? lstData.LastOrDefault().STT + 1 : 1,
                    iID_KhoiTaoDuLieuID = lstData.LastOrDefault() != null ? lstData.LastOrDefault().iID_KhoiTaoDuLieuID : iID_KhoiTao
                };
                lstData.Add(itemAdd);
            }
            vm.Items = lstData;
            TempData["KhoiTaoChiTiet"] = vm.Items;

            List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.GetDuAnByMaDonViQL(sMaDonVi).ToList();
            lstDuAn.Insert(0, new VDT_DA_DuAn { iID_DuAnID = Guid.Empty, sTenDuAn = "--Chọn--" });
            ViewBag.ListDuAn = lstDuAn.ToSelectList("iID_DuAnID", "sTenDuAn");

            return PartialView("_listDetail", vm);
        }

        [HttpGet]
        public JsonResult GetQdDauTuNguonVonByDuAn(Guid iIdDuAn)
        {
            var datas = _qLVonDauTuService.LayDanhSachNguonVonTheoDuAnInQDDauTu(iIdDuAn).ToList();
            if(datas != null)
                return Json(new { strCombobox = datas.Select(n => string.Format("<option value='{0}'>{1}</option>", n.iID_MaNguonNganSach, n.sTen)) }, JsonRequestBehavior.AllowGet);

            return Json(new { strCombobox  = string.Empty}, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetVDTNhaThauAndGiaTriHDByHopDong(Guid iID_HopDongID)
        {
            var objHopDong = _qLVonDauTuService.LayChiTietThongTinHopDong(iID_HopDongID);
            if(objHopDong != null)
                return Json(new { sTenNhaThau = objHopDong.sTenNhaThau, fGiaTriHopDong = (objHopDong.fTienHopDong??0) }, JsonRequestBehavior.AllowGet);

            return Json(new { sTenNhaThau = string.Empty, fGiaTriHopDong = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChiTietKhoiTaoTTDASave(List<VDT_KT_KhoiTao_DuLieu_ChiTiet> data)
        {
            foreach (var item in data)
            {
                item.sMaDuAn = _qLVonDauTuService.GetDuAnByIdDuAn(item.iID_DuAnID).sMaDuAn;
            }

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>();
            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan> lstHopDongSave = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan>();
            if (TempData["TotalHopDongKhoiTao"] != null)
            {
                lstTotalHopDong = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];
            }

            foreach(var item in lstTotalHopDong)
            {
                VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan itemSave = new VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan();
                itemSave.iID_KhoiTaoDuLieuChiTietThanhToanID = item.iID_KhoiTaoDuLieuChiTietThanhToanID;
                itemSave.iId_KhoiTaoDuLieuChiTietId = item.iId_KhoiTaoDuLieuChiTietId;
                itemSave.iID_HopDongId = item.iID_HopDongId;
                itemSave.fLuyKeTTKLHTTN_KHVN = item.fLuyKeTTKLHTTN_KHVN;
                itemSave.fLuyKeTUChuaThuHoiTN_KHVN = item.fLuyKeTUChuaThuHoiTN_KHVN;
                itemSave.fLuyKeTTKLHTNN_KHVN = item.fLuyKeTTKLHTNN_KHVN;
                itemSave.fLuyKeTUChuaThuHoiNN_KHVN = item.fLuyKeTUChuaThuHoiNN_KHVN;
                itemSave.fLuyKeTTKLHTTN_KHVU = item.fLuyKeTTKLHTTN_KHVU;
                itemSave.fLuyKeTUChuaThuHoiTN_KHVU = item.fLuyKeTUChuaThuHoiTN_KHVU;
                itemSave.fLuyKeTTKLHTNN_KHVU = item.fLuyKeTTKLHTNN_KHVU;
                itemSave.fLuyKeTUChuaThuHoiNN_KHVU = item.fLuyKeTUChuaThuHoiNN_KHVU;
                lstHopDongSave.Add(itemSave);
            }

            if (!_qLVonDauTuService.SaveChiTietKhoiTaoTTDA(data, lstTotalHopDong))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModalChiTietThanhToan(Guid iID_KhoiTao_ChiTietID, Guid iID_DuAnID)
        {
            HopDongKhoiTaoThongTinDuAnViewModel data = new HopDongKhoiTaoThongTinDuAnViewModel();
            data._paging.CurrentPage = 1;
            data.iID_KhoiTaoDuLieuChiTietID = iID_KhoiTao_ChiTietID;
            data.iID_DuAnID = iID_DuAnID;
            

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>();
            
            if (TempData["TotalHopDongKhoiTao"] != null)
            {
                lstTotalHopDong = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];
                TempData.Keep("TotalHopDongKhoiTao");
            }

            data.Items = lstTotalHopDong.Where(x => x.iId_KhoiTaoDuLieuChiTietId == iID_KhoiTao_ChiTietID);

            List<VDT_DA_TT_HopDong> lstHopDong = _qLVonDauTuService.GetHopDongByThanhToanDuAnId(iID_DuAnID).ToList();
            lstHopDong.Insert(0, new VDT_DA_TT_HopDong { iID_HopDongID = Guid.Empty, sSoHopDong = "--Chọn--" });
            ViewBag.ListHopDong = lstHopDong.Select(n => new SelectListItem { Text = string.Format("{0} - {1}", n.sSoHopDong, n.sTenHopDong), Value = n.iID_HopDongID.ToString() });

            return PartialView("_modalHopDong", data);
        }

        [HttpPost]
        public ActionResult ChiTietHopDongListView(byte type, int STT, Guid iID_KhoiTaoDuLieuChiTietID, Guid iID_DuAnID)
        {
            HopDongKhoiTaoThongTinDuAnViewModel vm = new HopDongKhoiTaoThongTinDuAnViewModel();
            vm._paging.CurrentPage = 1;
            vm.iID_KhoiTaoDuLieuChiTietID = iID_KhoiTaoDuLieuChiTietID;
            vm.iID_DuAnID = iID_DuAnID;
            var lstData = new List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>();
            if (TempData["TotalHopDongKhoiTao"] != null)
            {
                lstData = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];
            }
            //type = 0: Xóa
            if (type == 0)

            {
                var itemRemove = lstData.SingleOrDefault(x => x.STT == STT);
                lstData.Remove(itemRemove);
            }
            //Thêm dòng
            if (type == 1)
            {
                var itemAdd = new VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel
                {
                    iID_KhoiTaoDuLieuChiTietThanhToanID = Guid.NewGuid(),
                    STT = lstData.LastOrDefault() != null ? lstData.LastOrDefault().STT + 1 : 1,
                    iId_KhoiTaoDuLieuChiTietId = iID_KhoiTaoDuLieuChiTietID
                };
                lstData.Add(itemAdd);
            }
            vm.Items = lstData.Where(x => x.iId_KhoiTaoDuLieuChiTietId == iID_KhoiTaoDuLieuChiTietID);
            TempData["TotalHopDongKhoiTao"] = lstData;

            List<VDT_DA_TT_HopDong> lstHopDong = _qLVonDauTuService.GetHopDongByThanhToanDuAnId(iID_DuAnID).ToList();
            lstHopDong.Insert(0, new VDT_DA_TT_HopDong { iID_HopDongID = Guid.Empty, sSoHopDong = "--Chọn--" });
            ViewBag.ListHopDong = lstHopDong.ToSelectList("iID_HopDongID", "sSoHopDong");

            return PartialView("_partialModalHopDong", vm);
        }

        [HttpPost]
        public JsonResult ChangeListHopDong(List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan> data)
        {
            if(TempData["TotalHopDongKhoiTao"] == null || data== null || data.Count ==0)
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);

            List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstTotalHopDong = (List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>)TempData["TotalHopDongKhoiTao"];

            foreach(var item in data)
            {
                foreach (var item2 in lstTotalHopDong)
                {
                    if(item.iID_KhoiTaoDuLieuChiTietThanhToanID == item2.iID_KhoiTaoDuLieuChiTietThanhToanID)
                    {
                        item2.iID_HopDongId = item.iID_HopDongId;
                        item2.fLuyKeTTKLHTTN_KHVN = item.fLuyKeTTKLHTTN_KHVN;
                        item2.fLuyKeTUChuaThuHoiTN_KHVN = item.fLuyKeTUChuaThuHoiTN_KHVN;
                        item2.fLuyKeTTKLHTNN_KHVN = item.fLuyKeTTKLHTNN_KHVN;
                        item2.fLuyKeTUChuaThuHoiNN_KHVN = item.fLuyKeTUChuaThuHoiNN_KHVN;

                        item2.fLuyKeTTKLHTTN_KHVU = item.fLuyKeTTKLHTTN_KHVU;
                        item2.fLuyKeTUChuaThuHoiTN_KHVU = item.fLuyKeTUChuaThuHoiTN_KHVU;
                        item2.fLuyKeTTKLHTNN_KHVU = item.fLuyKeTTKLHTNN_KHVU;
                        item2.fLuyKeTUChuaThuHoiNN_KHVU = item.fLuyKeTUChuaThuHoiNN_KHVU;
                    }
                }
            }
            TempData["TotalHopDongKhoiTao"] = lstTotalHopDong;
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}