using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class QLPheDuyetDuAnController : AppController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;

        #region view
        // GET: QLVonDauTu/ChuTruongDauTu
        public ActionResult Index()
        {
            VDTPheDuyetDuAnViewModel vm = new VDTPheDuyetDuAnViewModel();
            try
            {
                vm._paging.CurrentPage = 1;
                vm.Items = _iQLVonDauTuService.LayDanhSachPheDuyetDuAn(ref vm._paging, PhienLamViec.NamLamViec, Username);

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                //lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.TAT_CA });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sMoTa");
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoQuyetDinh, string sTenDuAn, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, string sMaDonVi)
        {
            VDTPheDuyetDuAnViewModel vm = new VDTPheDuyetDuAnViewModel();
            try
            {
                vm._paging = _paging;
                vm.Items = _iQLVonDauTuService.LayDanhSachPheDuyetDuAn(ref vm._paging, PhienLamViec.NamLamViec, Username, sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi);
                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sMoTa");
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_list", vm);
        }

        public ActionResult TaoMoi()
        {
            try
            {
                #region data selectlist

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

                List<VDT_DM_HinhThucQuanLy> lstHinhThucQuanLy = _iQLVonDauTuService.LayHinhThucQuanLy().ToList();
                lstHinhThucQuanLy.Insert(0, new VDT_DM_HinhThucQuanLy { iID_HinhThucQuanLyID = Guid.Empty, sTenHinhThucQuanLy = Constants.CHON });
                ViewBag.ListHinhThucQLDA = lstHinhThucQuanLy.ToSelectList("iID_HinhThucQuanLyID", "sTenHinhThucQuanLy");

                List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
                lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
                ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
                #endregion
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View();
        }

        [HttpPost]
        public JsonResult Save(VDTPheDuyetDuAnCreateModel model, bool isDieuChinh = false, bool isTaoMoi = false, string fTongMucPheDuyetTheoChuTruong = null)
        {
            var iID_QDDauTuID = Guid.Empty;
            var sMessage = string.Format("Thêm mới bản ghi {0} thành công", model.sSoQuyetDinh);
            if (model.iID_QDDauTuID == null || model.iID_QDDauTuID == Guid.Empty)
            {
                if (model.iID_ParentID != null) sMessage = sMessage.Replace("Thêm mới", "Điều chỉnh");
            }
            else
            {
                sMessage = sMessage.Replace("Thêm mới", "Cập nhật");
            }

            if (model == null)
            {
                return Json(new { status = false, message = "Dữ liệu truyền lên không chính xác." });
            }

            //if (!isDieuChinh && !_iQLVonDauTuService.KiemTraTrungSoQuyetDinhQuyetDinhDauTu(model.sSoQuyetDinh, model.iID_QDDauTuID.ToString()))
            if (isDieuChinh && !_iQLVonDauTuService.KiemTraTrungSoQuyetDinhQuyetDinhDauTu(model.sSoQuyetDinh, model.iID_QDDauTuID.ToString()) || isTaoMoi && !_iQLVonDauTuService.KiemTraTrungSoQuyetDinhQuyetDinhDauTu(model.sSoQuyetDinh, model.iID_QDDauTuID.ToString()))
            {
                return Json(new { status = false, message = $"Đã tồn tại số quyết định {model.sSoQuyetDinh}." });
            }
            double TongMucPheDuyetTheoChuTruong = (fTongMucPheDuyetTheoChuTruong == null || fTongMucPheDuyetTheoChuTruong == "") ? 0 : double.Parse(fTongMucPheDuyetTheoChuTruong);
            
            if (TongMucPheDuyetTheoChuTruong < model.fTongMucDauTuPheDuyet)
            {
                return Json(new { status = false, message = "Giá trị phê duyệt nguồn vốn lớn hơn tổng mức phê duyệt theo chủ trương đầu tư!" });
            }
            if (model.ListNguonVon != null && model.ListNguonVon.Where(o => !o.isDelete).GroupBy(x => x.iID_NguonVonID).Any(g => g.Count() > 1))
            {
                return Json(new { status = false, message = "Nguồn vốn đã tồn tại. Vui lòng chọn lại!" });
            }

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    // Tạo MaHangMuc tự động
                    //string sMaDuAn = _iQLVonDauTuService.LayMaDuAnTheoDuAnId(model.iID_DuAnID);
                    //string sMaHangMuc = sMaDuAn.Substring(sMaDuAn.Length - 4);
                    //int indexMaHangMuc = _iQLVonDauTuService.GetIndexMaHangMuc();
                    var qdDauTu = new VDT_DA_QDDauTu();
                    if (model.iID_QDDauTuID == null || model.iID_QDDauTuID == Guid.Empty)
                    {
                        qdDauTu.iID_QDDauTuID = Guid.NewGuid();
                        qdDauTu.iID_DuAnID = model.iID_DuAnID;
                        qdDauTu.sSoQuyetDinh = model.sSoQuyetDinh;
                        qdDauTu.dNgayQuyetDinh = model.dNgayQuyetDinh;
                        qdDauTu.fTongMucDauTuPheDuyet = model.fTongMucDauTuPheDuyet;
                        qdDauTu.SMoTa = model.SMoTa;
                        qdDauTu.sDiaDiem = model.sDiaDiem;
                        qdDauTu.sKhoiCong = model.sKhoiCong;
                        qdDauTu.sKetThuc = model.sKetThuc;
                        qdDauTu.iID_HinhThucQuanLyID = model.iID_HinhThucQuanLyID;
                        qdDauTu.sUserCreate = Username;
                        qdDauTu.dDateCreate = DateTime.Now;
                        qdDauTu.sUserUpdate = Username;
                        qdDauTu.dDateUpdate = DateTime.Now;
                        qdDauTu.bActive = true;

                        model.iID_QDDauTuID = qdDauTu.iID_QDDauTuID;
                        iID_QDDauTuID = qdDauTu.iID_QDDauTuID;
                        if (isDieuChinh)
                        {
                            qdDauTu.bIsGoc = false;
                            qdDauTu.iID_ParentID = model.iID_ParentID;

                            //update qddautu cha bactive = 0
                            var entityParent = conn.Get<VDT_DA_QDDauTu>(model.iID_ParentID, trans);
                            if (entityParent != null)
                            {
                                entityParent.bActive = false;
                                conn.Update(entityParent, trans);
                            }
                        }
                        else
                        {
                            qdDauTu.bIsGoc = true;
                        }

                        VDT_DA_ChuTruongDauTu chutruong = _iQLVonDauTuService.FindChuTruongDauTuByDuAnId(model.iID_DuAnID.Value);
                        if (chutruong != null)
                        {
                            qdDauTu.iID_ChuTruongDauTuID = chutruong.iID_ChuTruongDauTuID;
                        }

                        conn.Insert<VDT_DA_QDDauTu>(qdDauTu, trans);
                        // update du an
                        if (model.iID_DuAnID != null && model.iID_DuAnID != Guid.Empty && !model.isDieuChinh)
                        {
                            var duAn = conn.Get<VDT_DA_DuAn>(model.iID_DuAnID, trans);
                            if (duAn != null)
                            {
                                duAn.sMaDuAn = model.sMaDuAn;
                                //duAn.iID_NhomDuAnID = model.iID_NhomDuAnID;
                                duAn.sKhoiCong = model.sKhoiCong;
                                duAn.sKetThuc = model.sKetThuc;
                                duAn.iID_HinhThucQuanLyID = model.iID_HinhThucQuanLyID;
                                //duAn.iID_ChuDauTuID = model.iID_ChuDauTuID;
                                duAn.fTongMucDauTu = model.fTongMucDauTuPheDuyet;
                                duAn.sDiaDiem = model.sDiaDiem;
                                duAn.sUserUpdate = Username;
                                duAn.dDateUpdate = DateTime.Now;
                                conn.Update<VDT_DA_DuAn>(duAn, trans);
                            }
                        }
                    }
                    else
                    {
                        qdDauTu = conn.Get<VDT_DA_QDDauTu>(model.iID_QDDauTuID, trans);
                        if (qdDauTu == null)
                            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

                        qdDauTu.sSoQuyetDinh = model.sSoQuyetDinh;
                        qdDauTu.dNgayQuyetDinh = model.dNgayQuyetDinh;
                        qdDauTu.sKhoiCong = model.sKhoiCong;
                        qdDauTu.sKetThuc = model.sKetThuc;
                        qdDauTu.iID_HinhThucQuanLyID = model.iID_HinhThucQuanLyID;
                        qdDauTu.sDiaDiem = model.sDiaDiem;
                        qdDauTu.fTongMucDauTuPheDuyet = model.fTongMucDauTuPheDuyet;
                        qdDauTu.SMoTa = model.SMoTa;
                        qdDauTu.sUserUpdate = Username;
                        qdDauTu.dDateUpdate = DateTime.Now;
                        iID_QDDauTuID = model.iID_QDDauTuID;
                        conn.Update(qdDauTu, trans);
                    }

                    List<VDTQuyetDinhDauTuNguonVonModel> listNguonVonAdd = model.ListNguonVon.Where(x => (x.iID_QDDauTu_NguonVonID == null || x.iID_QDDauTu_NguonVonID == Guid.Empty) && !x.isDelete).ToList();
                    List<VDTQuyetDinhDauTuNguonVonModel> listNguonVonEdit = model.ListNguonVon.Where(x => x.iID_QDDauTu_NguonVonID != null && x.iID_QDDauTu_NguonVonID != Guid.Empty && !x.isDelete).ToList();
                    List<VDTQuyetDinhDauTuNguonVonModel> listNguonVonDelete = model.ListNguonVon.Where(x => x.iID_QDDauTu_NguonVonID != null && x.iID_QDDauTu_NguonVonID != Guid.Empty && x.isDelete).ToList();

                    if (listNguonVonAdd.Count > 0)
                    {
                        foreach (var item in listNguonVonAdd)
                        {
                            string thuocTinhKhongMap = "iID_QDDauTu_NguonVonID";
                            var nguonVon = new VDT_DA_QDDauTu_NguonVon();
                            nguonVon.MapFrom(item, thuocTinhKhongMap);
                            if (isDieuChinh)
                            {
                                nguonVon.fGiaTriDieuChinh = (item.fTienPheDuyet ?? 0) - (item.fGiaTriTruocDieuChinh ?? 0);
                            }
                            nguonVon.iID_QDDauTuID = qdDauTu.iID_QDDauTuID;
                            //nguonVon.iID_QDDauTu_NguonVonID = item.Id.Value;
                            conn.Insert<VDT_DA_QDDauTu_NguonVon>(nguonVon, trans);
                        }
                    }
                    if (listNguonVonEdit.Count > 0)
                    {
                        foreach (var item in listNguonVonEdit)
                        {
                            VDT_DA_QDDauTu_NguonVon qdNguonVon = conn.Get<VDT_DA_QDDauTu_NguonVon>(item.iID_QDDauTu_NguonVonID, trans);
                            if (qdNguonVon != null)
                            {
                                qdNguonVon.iID_NguonVonID = item.iID_NguonVonID;
                                qdNguonVon.fTienPheDuyet = item.fTienPheDuyet;
                                conn.Update<VDT_DA_QDDauTu_NguonVon>(qdNguonVon, trans);
                            }
                        }
                    }
                    if (listNguonVonDelete.Count > 0)
                    {
                        foreach (var item in listNguonVonDelete)
                        {
                            VDT_DA_QDDauTu_NguonVon qdNguonVon = conn.Get<VDT_DA_QDDauTu_NguonVon>(item.iID_QDDauTu_NguonVonID, trans);
                            if (qdNguonVon != null)
                            {
                                conn.Delete<VDT_DA_QDDauTu_NguonVon>(qdNguonVon, trans);
                            }
                        }
                    }

                    List<VDTQuyetDinhDauTuChiPhiCreateModel> listQDChiPhiAdd = model.ListChiPhi.Where(x => (x.iID_QDDauTu_ChiPhiID == null || x.iID_QDDauTu_ChiPhiID == Guid.Empty) && !x.isDelete).ToList();
                    List<VDTQuyetDinhDauTuChiPhiCreateModel> listQDChiPhiEdit = model.ListChiPhi.Where(x => x.iID_QDDauTu_ChiPhiID != null && x.iID_QDDauTu_ChiPhiID != Guid.Empty && !x.isDelete).ToList();
                    List<VDTQuyetDinhDauTuChiPhiCreateModel> listChiPhiDelete = model.ListChiPhi.Where(x => x.iID_QDDauTu_ChiPhiID != null && x.iID_QDDauTu_ChiPhiID != Guid.Empty && x.isDelete).ToList();

                    if (listQDChiPhiAdd.Count > 0)
                    {
                        foreach (var item in listQDChiPhiAdd)
                        {
                            var qdChiPhi = new VDT_DA_QDDauTu_ChiPhi();
                            qdChiPhi.iID_QDDauTu_ChiPhiID = item.Id.Value;
                            qdChiPhi.iID_QDDauTuID = qdDauTu.iID_QDDauTuID;
                            qdChiPhi.fTienPheDuyet = item.fTienPheDuyet;
                            if (isDieuChinh)
                            {
                                qdChiPhi.fGiaTriDieuChinh = (item.fTienPheDuyet ?? 0) - (item.fGiaTriTruocDieuChinh ?? 0);
                            }
                            //tao DM_DuAn_ChiPhi moi
                            var dmDuAnChiPhi = new VDT_DM_DuAn_ChiPhi
                            {
                                iID_DuAn_ChiPhi = item.iID_DuAn_ChiPhi,
                                iID_ChiPhi = item.iID_ChiPhiID,
                                iID_ChiPhi_Parent = item.iID_ChiPhi_Parent,
                                sTenChiPhi = item.sTenChiPhi,
                                sMaChiPhi = item.sMaChiPhi,
                                dNgayTao = DateTime.Now,
                                sID_MaNguoiDungTao = Username,
                                dNgaySua = DateTime.Now,
                                sID_MaNguoiDungSua = Username,
                                iThuTu = (int)item.iThuTu
                            };

                            conn.Insert<VDT_DM_DuAn_ChiPhi>(dmDuAnChiPhi, trans);
                            qdChiPhi.iID_DuAn_ChiPhi = dmDuAnChiPhi.iID_DuAn_ChiPhi;

                            conn.Insert<VDT_DA_QDDauTu_ChiPhi>(qdChiPhi, trans);
                        }
                    }

                    if (listQDChiPhiEdit.Count > 0)
                    {
                        foreach (var item in listQDChiPhiEdit)
                        {
                            VDT_DA_QDDauTu_ChiPhi qdChiPhi = conn.Get<VDT_DA_QDDauTu_ChiPhi>(item.iID_QDDauTu_ChiPhiID, trans);
                            if (qdChiPhi != null)
                            {
                                qdChiPhi.fTienPheDuyet = item.fTienPheDuyet;

                                // update DM_DuAn_ChiPhi
                                VDT_DM_DuAn_ChiPhi dmDuAnChiPhi = conn.Get<VDT_DM_DuAn_ChiPhi>(item.iID_DuAn_ChiPhi, trans);
                                if (dmDuAnChiPhi != null)
                                {
                                    dmDuAnChiPhi.sTenChiPhi = item.sTenChiPhi;
                                    dmDuAnChiPhi.dNgaySua = DateTime.Now;
                                    dmDuAnChiPhi.sID_MaNguoiDungSua = Username;

                                    conn.Update<VDT_DM_DuAn_ChiPhi>(dmDuAnChiPhi, trans);
                                }

                                conn.Update<VDT_DA_QDDauTu_ChiPhi>(qdChiPhi, trans);
                            }
                        }
                    }

                    if (listChiPhiDelete.Count > 0)
                    {
                        foreach (var item in listChiPhiDelete)
                        {
                            VDT_DA_QDDauTu_ChiPhi qdChiPhi = conn.Get<VDT_DA_QDDauTu_ChiPhi>(item.iID_QDDauTu_ChiPhiID, trans);
                            if (qdChiPhi != null)
                            {
                                qdChiPhi.fTienPheDuyet = item.fTienPheDuyet;

                                conn.Delete<VDT_DA_QDDauTu_ChiPhi>(qdChiPhi, trans);
                            }
                        }
                    }

                    if (model.ListHangMuc != null)
                    {
                        List<VDTQuyetDinhDauTuDMHangMucModel> listQDHangMucAdd = model.ListHangMuc.Where(x => (x.iID_QDDauTu_HangMuciID == null || x.iID_QDDauTu_HangMuciID == Guid.Empty) && !x.isDelete).ToList();
                        List<VDTQuyetDinhDauTuDMHangMucModel> listQDHangMucEdit = model.ListHangMuc.Where(x => x.iID_QDDauTu_HangMuciID != null && x.iID_QDDauTu_HangMuciID != Guid.Empty && !x.isDelete).ToList();
                        List<VDTQuyetDinhDauTuDMHangMucModel> listQDHangMucDelete = model.ListHangMuc.Where(x => x.iID_QDDauTu_HangMuciID != null && x.iID_QDDauTu_HangMuciID != Guid.Empty && x.isDelete).ToList();

                        //List<VDTQuyetDinhDauTuDMHangMucModel> listQDHangMucDieuChinhAdd = model.ListHangMuc.Where(x => !x.isDelete && x.iID_HangMucID != null && x.iID_HangMucID != Guid.Empty
                        //                                                                                        && (x.iID_QDDauTu_HangMuciID == null || x.iID_QDDauTu_HangMuciID == Guid.Empty)).ToList();

                        if (listQDHangMucAdd.Count > 0)
                        {
                            foreach (var item in listQDHangMucAdd)
                            {
                                //sMaHangMuc += indexMaHangMuc.ToString().PadLeft(3, '0');
                                //indexMaHangMuc++;
                                var dmHangMuc = new VDT_DA_QDDauTu_DM_HangMuc();
                                dmHangMuc.iID_DuAnID = qdDauTu.iID_DuAnID;
                                //hangMuc.//sMaHangMuc = sMaHangMuc,
                                dmHangMuc.sTenHangMuc = item.sTenHangMuc;
                                dmHangMuc.iID_ParentID = item.iID_ParentID;
                                dmHangMuc.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
                                dmHangMuc.smaOrder = item.smaOrder;
                                dmHangMuc.iID_QDDauTu_DM_HangMucID = item.iID_QDDauTu_DM_HangMucID;

                                conn.Insert(dmHangMuc, trans);

                                var qdHangMuc = new VDT_DA_QDDauTu_HangMuc();
                                qdHangMuc.iID_QDDauTuID = qdDauTu.iID_QDDauTuID;
                                qdHangMuc.iID_HangMucID = dmHangMuc.iID_QDDauTu_DM_HangMucID;
                                qdHangMuc.iID_DuAn_ChiPhi = item.iID_DuAn_ChiPhi;
                                qdHangMuc.fTienPheDuyet = item.fTienPheDuyet;
                                if (isDieuChinh)
                                    qdHangMuc.fGiaTriDieuChinh = (item.fTienPheDuyet ?? 0) - (item.fGiaTriTruocDieuChinh ?? 0);
                                conn.Insert<VDT_DA_QDDauTu_HangMuc>(qdHangMuc, trans);
                            }
                        }

                        if (listQDHangMucEdit.Count > 0)
                        {
                            foreach (var item in listQDHangMucEdit)
                            {
                                VDT_DA_QDDauTu_DM_HangMuc danhMucHM = conn.Get<VDT_DA_QDDauTu_DM_HangMuc>(item.iID_QDDauTu_DM_HangMucID, trans);
                                if (danhMucHM != null)
                                {
                                    danhMucHM.sTenHangMuc = item.sTenHangMuc;
                                    danhMucHM.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
                                    conn.Update<VDT_DA_QDDauTu_DM_HangMuc>(danhMucHM, trans);
                                }

                                VDT_DA_QDDauTu_HangMuc hangMuc = conn.Get<VDT_DA_QDDauTu_HangMuc>(item.iID_QDDauTu_HangMuciID, trans);
                                if (hangMuc != null)
                                {
                                    hangMuc.fTienPheDuyet = item.fTienPheDuyet;
                                    conn.Update<VDT_DA_QDDauTu_HangMuc>(hangMuc, trans);
                                }
                            }
                        }
                        if (listQDHangMucDelete.Count > 0)
                        {
                            foreach (var item in listQDHangMucDelete)
                            {
                                VDT_DA_QDDauTu_DM_HangMuc danhMucHM = conn.Get<VDT_DA_QDDauTu_DM_HangMuc>(item.iID_QDDauTu_DM_HangMucID, trans);
                                VDT_DA_QDDauTu_HangMuc hangMuc = conn.Get<VDT_DA_QDDauTu_HangMuc>(item.iID_QDDauTu_HangMuciID, trans);
                                if (danhMucHM != null)
                                {
                                    conn.Delete<VDT_DA_QDDauTu_DM_HangMuc>(danhMucHM, trans);
                                }
                                if (hangMuc != null)
                                {
                                    conn.Delete<VDT_DA_QDDauTu_HangMuc>(hangMuc, trans);
                                }
                            }
                        }
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return Json(new { status = false });
                }
            }

            return Json(new { status = true, ID = iID_QDDauTuID, sMessage = sMessage });
        }

        public ActionResult ChiTiet(string id)
        {
            VDTQuyetDinhDauTuViewModel item = new VDTQuyetDinhDauTuViewModel();
            try
            {
                item = _iQLVonDauTuService.LayThongTinChiTietQuyetDinhDauTu(Guid.Parse(id));
                if (item == null)
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(item);
        }

        public ActionResult Sua(string id)
        {
            VDTQuyetDinhDauTuViewModel item = new VDTQuyetDinhDauTuViewModel();
            try
            {
                item = _iQLVonDauTuService.LayThongTinChiTietQuyetDinhDauTu(Guid.Parse(id));
                if (item == null)
                    return RedirectToAction("Index");
                #region data selectlist
                List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnLapKeHoachTrungHanDuocDuyet().ToList();
                lstDuAn.Insert(0, new VDT_DA_DuAn { iID_DuAnID = Guid.Empty, sMaDuAn = Constants.CHON });
                ViewBag.ListDuAn = lstDuAn.ToSelectList("iID_DuAnID", "sMaDuAn");

                List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec).ToList();
                lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
                ViewBag.ListChuDauTu = lstChuDauTu.ToSelectList("ID", "sTenCDT");

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

                List<VDT_DM_NhomDuAn> lstNhomDuAn = _iQLVonDauTuService.LayNhomDuAn().ToList();
                lstNhomDuAn.Insert(0, new VDT_DM_NhomDuAn { iID_NhomDuAnID = Guid.Empty, sTenNhomDuAn = Constants.CHON });
                ViewBag.ListNhomDuAn = lstNhomDuAn.ToSelectList("iID_NhomDuAnID", "sTenNhomDuAn");

                List<VDT_DM_HinhThucQuanLy> lstHinhThucQuanLy = _iQLVonDauTuService.LayHinhThucQuanLy().ToList();
                lstHinhThucQuanLy.Insert(0, new VDT_DM_HinhThucQuanLy { iID_HinhThucQuanLyID = Guid.Empty, sTenHinhThucQuanLy = Constants.CHON });
                ViewBag.ListHinhThucQLDA = lstHinhThucQuanLy.ToSelectList("iID_HinhThucQuanLyID", "sTenHinhThucQuanLy");

                List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _iQLVonDauTuService.LayPhanCapDuAn().ToList();
                lstPhanCapDuAn.Insert(0, new VDT_DM_PhanCapDuAn { iID_PhanCapID = Guid.Empty, sTen = Constants.CHON });
                ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

                List<VDT_DM_ChiPhi> lstChiPhi = _iQLVonDauTuService.LayChiPhi().ToList();
                lstChiPhi.Insert(0, new VDT_DM_ChiPhi { iID_ChiPhi = Guid.Empty, sTenChiPhi = Constants.CHON });
                ViewBag.ListChiPhi = lstChiPhi.ToSelectList("iID_ChiPhi", "sTenChiPhi");

                List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
                lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
                ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
                #endregion
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(item);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            bool xoa = false;
            var sSoQuyetDinh = "";
            var sMessage = "";

            try
            {
                List<VDT_DA_DuToan> lstItem = _iQLVonDauTuService.GetListDuToanByQDDT(!string.IsNullOrEmpty(id) ? Guid.Parse(id) : Guid.Empty).ToList();
                if (lstItem != null && lstItem.Count() > 0)
                {
                    return Json(xoa, JsonRequestBehavior.AllowGet);
                }

                // update du an
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var qdDauTu = conn.Get<VDT_DA_QDDauTu>(id, trans);
                    if (qdDauTu != null && qdDauTu.iID_ParentID != null)
                    {
                        sSoQuyetDinh = qdDauTu.sSoQuyetDinh;
                        var qdDauTuParent = conn.Get<VDT_DA_QDDauTu>(qdDauTu.iID_ParentID, trans);
                        if (qdDauTuParent != null && qdDauTuParent.iID_DuAnID != null && qdDauTuParent.iID_DuAnID != Guid.Empty)
                        {
                            var duAn = conn.Get<VDT_DA_DuAn>(qdDauTuParent.iID_DuAnID, trans);
                            if (duAn != null)
                            {
                                duAn.fTongMucDauTu = qdDauTuParent.fTongMucDauTuPheDuyet;
                                duAn.sDiaDiem = qdDauTuParent.sDiaDiem;
                                duAn.sUserUpdate = Username;
                                duAn.dDateUpdate = DateTime.Now;
                                conn.Update<VDT_DA_DuAn>(duAn, trans);
                            }
                        }
                    }
                    trans.Commit();
                }
                

                xoa = _iQLVonDauTuService.XoaQuyetDinhDauTu(Guid.Parse(id));
                sMessage = string.Format("Xóa bản ghi {0} thành công", sSoQuyetDinh);

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new {status =xoa, sMessage = sMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DieuChinh(string id)
        {
            VDTQuyetDinhDauTuViewModel item = new VDTQuyetDinhDauTuViewModel();
            try
            {
                item = _iQLVonDauTuService.LayThongTinChiTietQuyetDinhDauTu(Guid.Parse(id));
                if (item == null)
                    return RedirectToAction("Index");
                #region data selectlist
                List<VDT_DM_ChiPhi> lstChiPhi = _iQLVonDauTuService.LayChiPhi().ToList();
                lstChiPhi.Insert(0, new VDT_DM_ChiPhi { iID_ChiPhi = Guid.Empty, sTenChiPhi = Constants.CHON });
                ViewBag.ListChiPhi = lstChiPhi.ToSelectList("iID_ChiPhi", "sTenChiPhi");

                List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
                lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
                ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

                #endregion
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(item);
        }

        public ActionResult SuaDieuChinh(string id)
        {
            VDTQuyetDinhDauTuViewModel item = new VDTQuyetDinhDauTuViewModel();
            try
            {
                item = _iQLVonDauTuService.LayThongTinChiTietQuyetDinhDauTu(Guid.Parse(id));
                if (item == null)
                    return RedirectToAction("Index");
                #region data selectlist

                List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnLapKeHoachTrungHanDuocDuyet().ToList();
                lstDuAn.Insert(0, new VDT_DA_DuAn { iID_DuAnID = Guid.Empty, sMaDuAn = Constants.CHON });
                ViewBag.ListDuAn = lstDuAn.ToSelectList("iID_DuAnID", "sMaDuAn");

                List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec).ToList();
                lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
                ViewBag.ListChuDauTu = lstChuDauTu.ToSelectList("ID", "sTenCDT");

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

                List<VDT_DM_NhomDuAn> lstNhomDuAn = _iQLVonDauTuService.LayNhomDuAn().ToList();
                lstNhomDuAn.Insert(0, new VDT_DM_NhomDuAn { iID_NhomDuAnID = Guid.Empty, sTenNhomDuAn = Constants.CHON });
                ViewBag.ListNhomDuAn = lstNhomDuAn.ToSelectList("iID_NhomDuAnID", "sTenNhomDuAn");

                List<VDT_DM_HinhThucQuanLy> lstHinhThucQuanLy = _iQLVonDauTuService.LayHinhThucQuanLy().ToList();
                lstHinhThucQuanLy.Insert(0, new VDT_DM_HinhThucQuanLy { iID_HinhThucQuanLyID = Guid.Empty, sTenHinhThucQuanLy = Constants.CHON });
                ViewBag.ListHinhThucQLDA = lstHinhThucQuanLy.ToSelectList("iID_HinhThucQuanLyID", "sTenHinhThucQuanLy");

                List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _iQLVonDauTuService.LayPhanCapDuAn().ToList();
                lstPhanCapDuAn.Insert(0, new VDT_DM_PhanCapDuAn { iID_PhanCapID = Guid.Empty, sTen = Constants.CHON });
                ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

                List<VDT_DM_ChiPhi> lstChiPhi = _iQLVonDauTuService.LayChiPhi().ToList();
                lstChiPhi.Insert(0, new VDT_DM_ChiPhi { iID_ChiPhi = Guid.Empty, sTenChiPhi = Constants.CHON });
                ViewBag.ListChiPhi = lstChiPhi.ToSelectList("iID_ChiPhi", "sTenChiPhi");

                List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
                lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
                ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

                #endregion
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(item);
        }

        public JsonResult LayThongTinChiPhiDieuChinh(string iID_QDDauTuID, DateTime dNgayPheDuyet)
        {
            IEnumerable<VDTQuyetDinhDauTuChiPhiModel> lstChiPhi = _iQLVonDauTuService.LayDanhSachQuyetDinhDauTuChiPhiDieuChinh(Guid.Parse(iID_QDDauTuID), dNgayPheDuyet);
            return Json(new { lstCp = lstChiPhi }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinNguonVonDieuChinh(string iID_QDDauTuID, DateTime dNgayPheDuyet)
        {
            IEnumerable<VDTQuyetDinhDauTuNguonVonModel> lstNguonVon = _iQLVonDauTuService.LayDanhSachQuyetDinhDauTuNguonVonDieuChinh(Guid.Parse(iID_QDDauTuID), dNgayPheDuyet);
            return Json(new { lstNv = lstNguonVon }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VDTQuyetDinhDauTuHangMucModel(string iID_QDDauTuID, DateTime dNgayPheDuyet)
        {
            IEnumerable<VDTQuyetDinhDauTuHangMucModel> lstHangMuc = _iQLVonDauTuService.LayDanhSachQuyetDinhDauTuHangMucDieuChinh(Guid.Parse(iID_QDDauTuID), dNgayPheDuyet);
            return Json(new { lstHm = lstHangMuc }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckExistSoQuyetDinh(string checkVal)
        {
            bool isExisted = _iQLVonDauTuService.CheckExistSoQuyetDinhShare("sSoQuyetDinh", "VDT_DA_QDDauTu", checkVal);
            return Json(new { isExisted }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region common

        public JsonResult LayDanhSachDuAnTheoDonViQuanLy(string iID_DonViQuanLyID)
        {
            List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnTaoMoiPheDuyetDuAn(Guid.Parse(iID_DonViQuanLyID)).ToList();
            StringBuilder htmlString = new StringBuilder();
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' data-sMaDuAn='{1}'>{2}</option>", lstDuAn[i].iID_DuAnID, lstDuAn[i].sMaDuAn, lstDuAn[i].sTenDuAn);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiTietDuAn(string iID)
        {
            var d_DuAn = _iQLVonDauTuService.GetThongTinDuAnByDuAnId(Guid.Parse(iID));
            return Json(d_DuAn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTMPDTheoChuTruongDT(string idDuAn)
        {
            if (string.IsNullOrEmpty(idDuAn))
            {
                return Json(new { status = true, data = 0 });
            }
            var result = _iQLVonDauTuService.GetTMPDTheoChuTruongDauTu(Guid.Parse(idDuAn));

            return Json(new { status = true, data = result });
        }

        public JsonResult GetHangMucTheoDuAn(string idDuAn)
        {
            if (string.IsNullOrEmpty(idDuAn))
            {
                return Json(new { status = false, data = 0 });
            }

            IEnumerable<VDT_DA_DuAn_HangMuc> result = _iQLVonDauTuService.LayDanhSachHangMucTheoDuAnId(Guid.Parse(idDuAn));

            return Json(new { status = true, data = result });
        }

        public JsonResult FillDropdown()
        {
            var trees = BuildTrees();
            return Json(trees, JsonRequestBehavior.AllowGet);
        }

        public List<DM_LoaiCongTrinhTreeView> BuildTrees()
        {
            var allDmLoaiCongTrinh = _iQLVonDauTuService.GetAllDMLoaiCongTrinh();
            var dtos = allDmLoaiCongTrinh.Select(c => new DM_LoaiCongTrinhTreeView
            {
                id = c.iID_LoaiCongTrinh.ToString(),
                title = c.sTenLoaiCongTrinh,
                iID_LoaiCongTrinh = c.iID_LoaiCongTrinh,
                iID_Parent = c.iID_Parent,
            }).ToList();

            return BuildTrees(null, dtos);
        }

        private List<DM_LoaiCongTrinhTreeView> BuildTrees(Guid? pid, List<DM_LoaiCongTrinhTreeView> allLoaiCongTrinh)
        {
            var childrens = allLoaiCongTrinh.Where(c => c.iID_Parent == pid).ToList();
            if (childrens.Count() == 0)
            {
                return new List<DM_LoaiCongTrinhTreeView>();
            }
            foreach (var i in childrens)
            {
                i.subs = BuildTrees(i.iID_LoaiCongTrinh, allLoaiCongTrinh);
                if (i.subs.Count > 0)
                    i.isSelectable = false;
                else
                    i.isSelectable = true;
            }
            return childrens;
        }

        /// <summary>
        /// kiem tra trung so quyet dinh
        /// </summary>
        /// <param name="sSoQuyetDinh"></param>
        /// <returns></returns>
        public JsonResult KiemTraTrungSoQuyetDinh(string sSoQuyetDinh, string iID_QDDauTuID = "")
        {
            bool status = _iQLVonDauTuService.KiemTraTrungSoQuyetDinhQuyetDinhDauTu(sSoQuyetDinh, iID_QDDauTuID);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListNguonTheoChuTruongDauTu(Guid idDuAn)
        {
            var listModel = _iQLVonDauTuService.GetListNguonVonTheoChuTruongDauTu(idDuAn);
            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListNguonVonTheoQDDauTuDauTu(Guid qdDauTuId)
        {
            var listModel = _iQLVonDauTuService.GetListNguonVonTheoQDDauTuDauTu(qdDauTuId);
            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetNguonVonAll()
        {
            var result = new List<dynamic>();
            var listModel = _iQLVonDauTuService.LayNguonVon().ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.iID_MaNguonNganSach,
                        text = item.sTen
                    });
                }
            }
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetLoaiChiPhiAll()
        {
            var result = new List<dynamic>();

            var listModel = _iQLVonDauTuService.LayChiPhi().ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.iID_ChiPhi,
                        text = item.sTenChiPhi
                    });
                }
            }

            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetListChiPhiDefault()
        {
            var listModel = _iQLVonDauTuService.LayChiPhi().ToList();

            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListChiPhiByQDDauTu(Guid qdDauTuId, bool isDieuChinh = false)
        {
            List<VDTQuyetDinhDauTuChiPhiCreateModel> listModel = new List<VDTQuyetDinhDauTuChiPhiCreateModel>();
            try
            {
                listModel = _iQLVonDauTuService.GetListChiPhiTheoQDDauTuDauTu(qdDauTuId).ToList();
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { status = false, data = listModel });
            }

            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListChiPhiVaHangMucByQDDauTuDieuChinh(Guid qdDauTuId, bool isDieuChinh = false)
        {
            List<VDTQuyetDinhDauTuChiPhiCreateModel> listChiPhi = new List<VDTQuyetDinhDauTuChiPhiCreateModel>();
            List<VDTQuyetDinhDauTuDMHangMucModel> listHangMuc = new List<VDTQuyetDinhDauTuDMHangMucModel>();
            try
            {
                listChiPhi = _iQLVonDauTuService.GetListChiPhiTheoQDDauTuDauTu(qdDauTuId).ToList();
                listHangMuc = _iQLVonDauTuService.GetListHangMucTheoQDDauTu(qdDauTuId).ToList();
                if (isDieuChinh)
                {
                    // reset iID_DuAn_ChiPhi
                    Dictionary<Guid, Guid> listNewIDDuAnChiPhi = new Dictionary<Guid, Guid>();
                    foreach (VDTQuyetDinhDauTuChiPhiCreateModel item in listChiPhi)
                    {
                        Guid newId = Guid.NewGuid();
                        listNewIDDuAnChiPhi.Add(item.iID_DuAn_ChiPhi, newId);
                        item.iID_DuAn_ChiPhi = newId;
                    }
                    List<VDTQuyetDinhDauTuChiPhiCreateModel> listChiPhiChild = listChiPhi.Where(x => x.iID_ChiPhi_Parent != null).ToList();
                    foreach (VDTQuyetDinhDauTuChiPhiCreateModel item in listChiPhiChild)
                    {
                        item.iID_ChiPhi_Parent = listNewIDDuAnChiPhi[item.iID_ChiPhi_Parent.Value];
                    }

                    // update giá trị trước điều chỉnh = giá trị phê duyệt của bản ghi hiện tại
                    foreach (VDTQuyetDinhDauTuChiPhiCreateModel item in listChiPhi)
                    {
                        item.fGiaTriTruocDieuChinh = item.fTienPheDuyet;
                    }

                    // reset iID_QDDauTu_DM_HangMuc
                    Dictionary<Guid, Guid> listNewIDQDDauTuDMHangMuc = new Dictionary<Guid, Guid>();
                    foreach (VDTQuyetDinhDauTuDMHangMucModel item in listHangMuc)
                    {
                        Guid newId = Guid.NewGuid();
                        listNewIDQDDauTuDMHangMuc.Add(item.iID_QDDauTu_DM_HangMucID, newId);
                        item.iID_QDDauTu_DM_HangMucID = newId;
                        item.iID_DuAn_ChiPhi = listNewIDDuAnChiPhi.ContainsKey(item.iID_DuAn_ChiPhi) ? listNewIDDuAnChiPhi[item.iID_DuAn_ChiPhi] : Guid.Empty;
                    }
                    List<VDTQuyetDinhDauTuDMHangMucModel> listHangMucChild = listHangMuc.Where(x => x.iID_ParentID != null).ToList();
                    foreach (VDTQuyetDinhDauTuDMHangMucModel item in listHangMucChild)
                    {
                        item.iID_ParentID = listNewIDQDDauTuDMHangMuc[item.iID_ParentID.Value];
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true, dataChiPhi = listChiPhi, dataHangMuc = listHangMuc });
        }

        [HttpPost]
        public JsonResult GetListHangMucTheoChuTruongDauTu(Guid? idDuAn)
        {
            List<VDTQuyetDinhDauTuDMHangMucModel> listHangMuc = new List<VDTQuyetDinhDauTuDMHangMucModel>();
            try
            {
                listHangMuc = _iQLVonDauTuService.GetListHangMucTheoChuTruongDauTu(idDuAn).ToList();

                // reset iID_QDDauTu_DM_HangMuc
                Dictionary<Guid, Guid> listNewIDQDDauTuDMHangMuc = new Dictionary<Guid, Guid>();
                foreach (VDTQuyetDinhDauTuDMHangMucModel item in listHangMuc)
                {
                    Guid newId = Guid.NewGuid();
                    listNewIDQDDauTuDMHangMuc.Add(item.iID_QDDauTu_DM_HangMucID, newId);
                    item.iID_QDDauTu_DM_HangMucID = newId;
                }
                List<VDTQuyetDinhDauTuDMHangMucModel> listHangMucChild = listHangMuc.Where(x => x.iID_ParentID != null).ToList();
                foreach (VDTQuyetDinhDauTuDMHangMucModel item in listHangMucChild)
                {
                    item.iID_ParentID = listNewIDQDDauTuDMHangMuc[item.iID_ParentID.Value];
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true, data = listHangMuc });
        }

        [HttpPost]
        public JsonResult GetListHangMucTheoQDDauTu(Guid qdDauTuId)
        {
            List<VDTQuyetDinhDauTuDMHangMucModel> listModel = new List<VDTQuyetDinhDauTuDMHangMucModel>();
            try
            {
                listModel = _iQLVonDauTuService.GetListHangMucTheoQDDauTu(qdDauTuId).ToList();
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListQuyetDinhDauTuHangMuc(Guid? iID_QDDauTuID, Guid? iID_DuAn_ChiPhi)
        {
            List<VDTQuyetDinhDauTuDMHangMucModel> listModel = new List<VDTQuyetDinhDauTuDMHangMucModel>();
            try
            {
                listModel = _iQLVonDauTuService.GetListQuyetDinhDauTuHangMuc(iID_QDDauTuID, iID_DuAn_ChiPhi).ToList();
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { status = true, data = listModel });
        }
        #endregion
    }
}