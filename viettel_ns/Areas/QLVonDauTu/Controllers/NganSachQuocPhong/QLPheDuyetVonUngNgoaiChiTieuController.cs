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

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class QLPheDuyetVonUngNgoaiChiTieuController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        // GET: QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu
        public ActionResult Index()
        {
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
            VDTPheDuyetVonUngNCTPagingModel dataPDVUNCT = new VDTPheDuyetVonUngNCTPagingModel();
            dataPDVUNCT._paging.CurrentPage = 1;
            dataPDVUNCT.lstData = _iQLVonDauTuService.GetAllPheDuyetVonUngNCT(ref dataPDVUNCT._paging, null, string.Empty, null, null);
            return View(dataPDVUNCT);
        }

        [HttpPost]
        public ActionResult TimKiemPheDuyetVonUngNCT(PagingInfo _paging, Guid? iID_DonViQuanLyID, string sSoDeNghi, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo)
        {
            VDTPheDuyetVonUngNCTPagingModel dataPDVUNCT = new VDTPheDuyetVonUngNCTPagingModel();
            dataPDVUNCT._paging = _paging;
            dataPDVUNCT.lstData = _iQLVonDauTuService.GetAllPheDuyetVonUngNCT(ref dataPDVUNCT._paging, iID_DonViQuanLyID, sSoDeNghi, dNgayDeNghiFrom, dNgayDeNghiTo);
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
            return PartialView("_partialListPDVUNCT", dataPDVUNCT);
        }

        public ActionResult CreateNew(Guid? id)
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");

            List<VDT_DM_NhomQuanLy> lstNhomQuanLy = _iQLVonDauTuService.GetNhomQuanLyList().ToList();
            ViewBag.ListNhomQuanLy = lstNhomQuanLy.ToSelectList("iID_NhomQuanLyID", "sTenNhomQuanLy");

            VDTQLPheDuyetVonUngNCTModel data = new VDTQLPheDuyetVonUngNCTModel();
            if (id.HasValue)
            {
                VDT_TT_DeNghiThanhToanUng dataDNTTU = new VDT_TT_DeNghiThanhToanUng();
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    dataDNTTU = conn.Get<VDT_TT_DeNghiThanhToanUng>(id, trans);
                    // commit to db
                    trans.Commit();
                }
                data.dataDNTTU = dataDNTTU;
                IEnumerable<VDTPheDuyetVonUngNgoaiCTViewModel> listDNTTUChiTiet = _iQLVonDauTuService.GetPDVUNCTChiTietList(id);
                data.listDNTTUChiTiet = listDNTTUChiTiet;
            }
            else
            {
                VDT_TT_DeNghiThanhToanUng dataKHVU = new VDT_TT_DeNghiThanhToanUng();
                data.dataDNTTU = dataKHVU;
            }
            return View(data);
        }

        [HttpPost]
        public JsonResult GetDNTTUChiTiet(Guid iId)
        {
            IEnumerable<VDTPheDuyetVonUngNgoaiCTViewModel> data = _iQLVonDauTuService.GetPDVUNCTChiTietList(iId);
            return Json(new { data = data, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool VDTPDVUNCTDelete(Guid id)
        {
            if (!_iQLVonDauTuService.deletePDVUNCTChiTiet(id)) return false;
            if (!_iQLVonDauTuService.deletePDVUNCT(id)) return false;
            return true;
        }

        public JsonResult LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhKHVU(string iID_DonViQuanLyID, DateTime dNgayQuyetDinh)
        {
            List<VDTPheDuyetVonUngNgoaiCTViewModel> lstDuAn = _iQLVonDauTuService.LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhKHVU(Guid.Parse(iID_DonViQuanLyID), dNgayQuyetDinh).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' sMaCapPheDuyet='{1}'>{2}</option>", lstDuAn[i].iID_DuAnID, lstDuAn[i].sMaCapPheDuyet, lstDuAn[i].sTenDuAn);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDanhSachHopDongTheoDuAn(Guid iID_DuAnID)
        {
            List<VDT_DA_TT_HopDong> lstHopDong = _iQLVonDauTuService.LayDanhSachHopDongTheoDuAn(iID_DuAnID).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
            if (lstHopDong != null && lstHopDong.Count > 0)
            {
                for (int i = 0; i < lstHopDong.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' bIsGoc='{1}'>{2}</option>", lstHopDong[i].iID_HopDongID, lstHopDong[i].bIsGoc, lstHopDong[i].sSoHopDong);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LayLuyKeTheoDuAn(Guid iID_DuAnID, Guid iID_DonViQuanLyID, Guid iID_NhomQuanLyID, DateTime dNgayQuyetDinh)
        {
            double fLKKHVUDuocDuyet = _iQLVonDauTuService.GetLuyKeKHVUDuocDuyet(iID_DuAnID, iID_DonViQuanLyID, iID_NhomQuanLyID, dNgayQuyetDinh);
            VDTPheDuyetVonUngNgoaiCTViewModel data = new VDTPheDuyetVonUngNgoaiCTViewModel();
            data = _iQLVonDauTuService.GetLuyKeUng(iID_DuAnID, null, iID_DonViQuanLyID, iID_NhomQuanLyID, dNgayQuyetDinh);
            data.fLKKHVUDuocDuyet = fLKKHVUDuocDuyet;
            return Json(new { bIsComplete = (data == null ? false : true), data = data }, JsonRequestBehavior.AllowGet);
            //return fLKKHVUDuocDuyet;
        }

        [HttpPost]
        public JsonResult LayThongTinHopDongVaLuyKeUng(Guid iID_DuAnID, Guid iID_HopDongID, Guid iID_DonViQuanLyID, Guid iID_NhomQuanLyID, DateTime dNgayQuyetDinh)
        {
            VDTPheDuyetVonUngNgoaiCTViewModel dataLKUng = new VDTPheDuyetVonUngNgoaiCTViewModel();
            dataLKUng = _iQLVonDauTuService.GetLuyKeUng(iID_DuAnID, iID_HopDongID, iID_DonViQuanLyID, iID_NhomQuanLyID, dNgayQuyetDinh);

            VDTPheDuyetVonUngNgoaiCTViewModel data = new VDTPheDuyetVonUngNgoaiCTViewModel();
            data = _iQLVonDauTuService.GetThongTinHopDong(iID_HopDongID, dNgayQuyetDinh);
            data.fLKThuHoiUng = dataLKUng.fLKThuHoiUng;
            data.fLKSoVonDaTamUng = dataLKUng.fLKSoVonDaTamUng;
            return Json(new { bIsComplete = (data == null ? false : true), data = data }, JsonRequestBehavior.AllowGet);
        }

        public bool QLPheDuyetVonUngNCTSave(VDTQLPheDuyetVonUngNCTModel data)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                if (data.dataDNTTU.iID_DeNghiThanhToanID == Guid.Empty)
                {
                    #region Them moi VDT_TT_DeNghiThanhToanUng

                    var entityDNTTU = new VDT_TT_DeNghiThanhToanUng();
                    entityDNTTU.MapFrom(data.dataDNTTU);
                    entityDNTTU.sUserCreate = Username;
                    entityDNTTU.dDateCreate = DateTime.Now;
                    conn.Insert(entityDNTTU, trans);
                    #endregion

                    #region Them moi VDT_TT_DeNghiThanhToanUng_ChiTiet
                    if (data.listDNTTUChiTiet != null && data.listDNTTUChiTiet.Count() > 0)
                    {
                        for (int i = 0; i < data.listDNTTUChiTiet.Count(); i++)
                        {
                            var entityDNTTUChiTiet = new VDT_TT_DeNghiThanhToanUng_ChiTiet();
                            entityDNTTUChiTiet.MapFrom(data.listDNTTUChiTiet.ToList()[i]);
                            entityDNTTUChiTiet.iID_DeNghiThanhToanID = entityDNTTU.iID_DeNghiThanhToanID;
                            conn.Insert(entityDNTTUChiTiet, trans);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Sửa phê duyệt vốn ứng ngoài chỉ tiêu
                    var entity = conn.Get<VDT_TT_DeNghiThanhToanUng>(data.dataDNTTU.iID_DeNghiThanhToanID, trans);
                    entity.sSoDeNghi = data.dataDNTTU.sSoDeNghi;
                    entity.sNguoiLap = data.dataDNTTU.sNguoiLap;
                    entity.sGhiChu = data.dataDNTTU.sGhiChu;
                    entity.fGiaTriThanhToan = data.dataDNTTU.fGiaTriThanhToan;
                    entity.fGiaTriThuHoiUngNgoaiChiTieu = data.dataDNTTU.fGiaTriThuHoiUngNgoaiChiTieu;
                    entity.sUserUpdate = Username;
                    entity.dDateUpdate = DateTime.Now;
                    conn.Update(entity, trans);
                    #endregion

                    #region Them moi VDT_TT_DeNghiThanhToanUng_ChiTiet
                    //delete all KHVUChiTiet
                    _iQLVonDauTuService.deletePDVUNCTChiTiet(data.dataDNTTU.iID_DeNghiThanhToanID);
                    //insert new
                    if (data.listDNTTUChiTiet != null && data.listDNTTUChiTiet.Count() > 0)
                    {
                        for (int i = 0; i < data.listDNTTUChiTiet.Count(); i++)
                        {
                            var entityDNTTUChiTiet = new VDT_TT_DeNghiThanhToanUng_ChiTiet();
                            entityDNTTUChiTiet.MapFrom(data.listDNTTUChiTiet.ToList()[i]);
                            entityDNTTUChiTiet.iID_DeNghiThanhToanID = data.dataDNTTU.iID_DeNghiThanhToanID;
                            conn.Insert(entityDNTTUChiTiet, trans);
                        }
                    }
                    #endregion
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public ActionResult ViewDetail(Guid id)
        {
            VDTPheDuyetVonUngNgoaiCTViewModel dataView = new VDTPheDuyetVonUngNgoaiCTViewModel();
            dataView = _iQLVonDauTuService.GetPDVUNCTById(id);
            return View(dataView);
        }

        [HttpPost]
        public bool CheckExistSoDeNghi(Guid iID_DeNghiThanhToanID, string sSoDeNghi)
        {
            var isExist = _iQLVonDauTuService.CheckExistSoDeNghi(iID_DeNghiThanhToanID, sSoDeNghi);
            return isExist;
        }
    }
}