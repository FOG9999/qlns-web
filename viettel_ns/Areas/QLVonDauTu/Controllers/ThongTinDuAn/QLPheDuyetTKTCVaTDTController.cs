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
using static VIETTEL.Common.Constants;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class QLPheDuyetTKTCVaTDTController : AppController
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        #region View
        // GET: QLVonDauTu/QLPheDuyetTKTCVaTDT
        public ActionResult Index()
        {
            VDTPheDuyetTKTCVaTDTViewModel vm = new VDTPheDuyetTKTCVaTDTViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qLVonDauTuService.GetAllVDTPheDuyetTKTCVaTDT(ref vm._paging, Username, PhienLamViec.NamLamViec, Constants.TONG_DU_TOAN).OrderByDescending(x => x.dDateCreate);
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.TAT_CA });
            ViewBag.ListDonViQL = lstDonViQL.ToSelectList("iID_Ma", "sTen");
            return View(vm);
        }

        public ActionResult Update(Guid? id, bool bIsDieuChinh = false)
        {
            VDT_DA_DuToan_ViewModel data = new VDT_DA_DuToan_ViewModel();

            if (id.HasValue && id != Guid.Empty)
            {
                data = _qLVonDauTuService.GetPheDuyetTKTCvaTDTByID(id.Value);
            }
            if (data == null) return View("Index");

            ViewBag.bIsDieuChinh = bIsDieuChinh;
            if (id == null || id == Guid.Empty)
                ViewBag.title = "Tạo mới thông tin phê duyệt TKTC&TDT'";
            else
                ViewBag.title = "Cập nhật thông tin phê duyệt TKTC&TDT";
            ViewBag.ItemsDuToanType = new SelectList(GetCbxLoaiDuToan(), "Value", "Text", (data.bLaTongDuToan ? 1 : 0));
            ViewBag.sNgayQuyetDinhDefault = data.dNgayQuyetDinh.HasValue ? data.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
            return View(data);
        }
        #endregion

        #region Event
        #region Index
        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sTenDuAn, string sSoQuyetDinh, DateTime? dPheduyetTuNgay, DateTime? dPheduyetDenNgay, Guid? sDonViQL, byte bIsTongDuToan)
        {
            if (sDonViQL.HasValue && sDonViQL.Value == Guid.Empty)
                sDonViQL = null;
            //if (dPheduyetDenNgay.HasValue)
            //    dPheduyetDenNgay = dPheduyetDenNgay.Value.AddDays(1);
            VDTPheDuyetTKTCVaTDTViewModel vm = new VDTPheDuyetTKTCVaTDTViewModel();
            vm._paging = _paging;
            vm.Items = _qLVonDauTuService.GetAllVDTPheDuyetTKTCVaTDT(ref vm._paging, Username, PhienLamViec.NamLamViec, bIsTongDuToan, sTenDuAn, sSoQuyetDinh, dPheduyetTuNgay, dPheduyetDenNgay, sDonViQL);
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.TAT_CA });
            ViewBag.ListDonViQL = lstDonViQL.ToSelectList("iID_Ma", "sTen");
            return PartialView("_partialList", vm);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            bool xoa = _qLVonDauTuService.XoaQLPheDuyetTKTCvaTDT(Guid.Parse(id));
            return Json(xoa, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Update
        [HttpGet]
        public JsonResult FindDuAn(string iID_DonViQuanLyID, int iLoaiQuyetDinh, Guid? iIdDuToanId)
        {
            List<VDT_DA_DuAn> lstDuAn = new List<VDT_DA_DuAn>();
            if (iIdDuToanId.Value != null && iIdDuToanId.Value != Guid.Empty)
            {
                lstDuAn = _qLVonDauTuService.GetDuAnByIdDuToan(iIdDuToanId.Value).ToList();
            }
            else
            {
                lstDuAn = _qLVonDauTuService.LayDuAnByDonViQLVaLoaiQuyetDinh(iID_DonViQuanLyID, iLoaiQuyetDinh).ToList();
            }
            return Json(new { data = lstDuAn }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCbxDonViQuanLy()
        {
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            string sCbxData = string.Join(",", lstDonViQL.Select(n => string.Format("<option value='{0}' data-iIdDonVi='{1}'>{2}</option>", n.iID_MaDonVi, n.iID_Ma.ToString(), n.sTen)));
            return Json(new { data = sCbxData }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNguonVonDauTu()
        {
            List<NS_NguonNganSach> lstNguonVon = _qLVonDauTuService.LayNguonVon().ToList();
            return Json(new { data = lstNguonVon }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNguonVonByDuToan(Guid iIdDuAnId, Guid? iIdDuToan = null, bool bIsDieuChinh = false)
        {
            List<VDT_DA_DuToan_Nguonvon_ViewModel> lstData = new List<VDT_DA_DuToan_Nguonvon_ViewModel>();
            if (iIdDuToan == null || iIdDuToan == Guid.Empty || bIsDieuChinh == false)
            {
                lstData = _qLVonDauTuService.GetNguonVonTKTCTDTByDuAnId(iIdDuAnId);
            }
            else
            {
                lstData = _qLVonDauTuService.GetListNguonVonTheoTKTC(iIdDuToan.Value).ToList();
            }
            return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetChiPhiByDuToan(Guid iIdDuAnId, Guid? iIdDuToan = null, bool bIsDieuChinh = false)
        {
            List<VDT_DA_DuToan_ChiPhi_ViewModel> lstData = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            if (iIdDuToan == null || iIdDuToan == Guid.Empty || bIsDieuChinh == false)
            {
                lstData = _qLVonDauTuService.GetChiPhiTKTCTDTByDuAnId(iIdDuAnId);
            }
            else
            {
                lstData = _qLVonDauTuService.GetListChiPhiTheoTKTC(iIdDuToan.Value).ToList();
            }
            //return Json(new { data = SortChiPhi(lstData) }, JsonRequestBehavior.AllowGet);
            return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHangMucByDuToan(Guid iIdDuAnId, Guid? iIdDuToan = null, bool bIsDieuChinh = false)
        {
            List<VDT_DA_DuToan_HangMuc_ViewModel> lstData = new List<VDT_DA_DuToan_HangMuc_ViewModel>();
            if (iIdDuToan == null || iIdDuToan == Guid.Empty || bIsDieuChinh)
            {
                lstData = _qLVonDauTuService.GetListHangMucTheoPheDuyetDuAn(iIdDuAnId);
            }
            else
            {
                lstData = _qLVonDauTuService.GetListHangMucTheoTKTC(iIdDuToan.Value).ToList();
            }
            return Json(new { data = SortHangMuc(lstData) }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Save(VDT_DA_DuToan objDuToan,
            List<VDT_DA_DuToan_Nguonvon_ViewModel> lstNguonVon,
            List<VDT_DA_DuToan_ChiPhi_ViewModel> lstChiPhi,
            List<VDT_DA_DuToan_HangMuc_ViewModel> lstHangMuc,
            bool bIsDieuChinh = false)
        {
            var iID_DuToanID = objDuToan.iID_DuToanID;
            if (lstNguonVon != null && lstNguonVon.Where(o => !o.isDelete).GroupBy(x => x.iID_NguonVonID).Any(g => g.Count() > 1))
            {
                return Json(new { status = false, sMessError = "Nguồn vốn đã tồn tại. Vui lòng chọn lại!" });
            }

            Guid iIdQdDauTu = Guid.Empty;
            var lstQdDauTu = _qLVonDauTuService.FindQDDauTuByDuAnId(objDuToan.iID_DuAnID);
            if (lstQdDauTu != null)
                iIdQdDauTu = lstQdDauTu.iID_QDDauTuID;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    if (objDuToan.iID_DuToanID == Guid.Empty || bIsDieuChinh)
                    {
                        objDuToan.bIsGoc = true;
                        if (bIsDieuChinh)
                        {
                            var objParent = conn.Get<VDT_DA_DuToan>(objDuToan.iID_DuToanID, trans);
                            if (objParent != null)
                            {
                                objParent.bActive = false;
                                conn.Update(objParent, trans);
                            }
                            objDuToan.iID_ParentID = objDuToan.iID_DuToanID;
                            objDuToan.iID_DuToanID = Guid.Empty;
                            objDuToan.bIsGoc = false;
                            objDuToan.iID_DuToanGocID = objParent.iID_DuToanGocID;
                        }
                        objDuToan.iID_QDDauTuID = iIdQdDauTu;
                        objDuToan.bActive = true;
                        objDuToan.dDateCreate = DateTime.Now;
                        objDuToan.sUserCreate = Username;
                        objDuToan.iID_DuToanID = Guid.NewGuid();
                        if (!bIsDieuChinh)
                            objDuToan.iID_DuToanGocID = objDuToan.iID_DuToanID;

                        conn.Insert(objDuToan, trans);
                        iID_DuToanID = objDuToan.iID_DuToanID;

                    }
                    else
                    {
                        var objEdit = conn.Get<VDT_DA_DuToan>(objDuToan.iID_DuToanID, trans);
                        if (objEdit == null)
                        {
                            trans.Rollback();
                            return Json(new { status = false, sMessError = "Not found !" });
                        }
                        objEdit.sSoQuyetDinh = objDuToan.sSoQuyetDinh;
                        objEdit.dNgayQuyetDinh = objDuToan.dNgayQuyetDinh;
                        objEdit.sMoTa = objDuToan.sMoTa;
                        objEdit.sTenDuToan = objDuToan.sTenDuToan;
                        objEdit.dDateUpdate = DateTime.Now;
                        objEdit.sUserUpdate = Username;
                        objEdit.fTongDuToanPheDuyet = objDuToan.fTongDuToanPheDuyet;
                        conn.Update(objEdit, trans);
                    }

                    _qLVonDauTuService.XoaDanhSachTKTCvaTDTChiPhiNguonVonCu(objDuToan.iID_DuToanID);
                    Dictionary<Guid, Guid> dicChiPhiId = new Dictionary<Guid, Guid>();
                    SaveDataNguonVon(objDuToan.iID_DuToanID, lstNguonVon);
                    SaveDataChiPhi(objDuToan.iID_DuToanID, lstChiPhi, ref dicChiPhiId);
                    SaveDataHangMuc(objDuToan.iID_DuToanID, lstHangMuc, dicChiPhiId);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }

            }

            return Json(new { status = true, iID_DuToanID = iID_DuToanID });
        }

        #endregion
        #endregion

        #region Helper
        private void SaveDataNguonVon(Guid iIdDuToanId, List<VDT_DA_DuToan_Nguonvon_ViewModel> lstNguonVon)
        {
            if (lstNguonVon == null || lstNguonVon.Count == 0) return;
            var lstData = lstNguonVon.Where(o => !o.isDelete).Select(n => new VDT_DA_DuToan_Nguonvon()
            {
                iID_DuToan_NguonVonID = Guid.NewGuid(),
                iID_DuToanID = iIdDuToanId,
                fTienPheDuyet = n.fTienPheDuyet,
                iID_NguonVonID = n.iID_NguonVonID,
                fGiaTriDieuChinh = n.fGiaTriDieuChinh,
                fTienPheDuyetQDDT = n.fTienPheDuyetQDDT
            });
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    conn.Insert(lstData, trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        private void SaveDataChiPhi(Guid iIdDuToanId, List<VDT_DA_DuToan_ChiPhi_ViewModel> lstChiPhi, ref Dictionary<Guid, Guid> dicChiPhiId)
        {
            if (lstChiPhi == null) return;
            List<VDT_DM_DuAn_ChiPhi> lstDmChiPhi = new List<VDT_DM_DuAn_ChiPhi>();
            List<VDT_DA_DuToan_ChiPhi> lstDuToanChiPhi = new List<VDT_DA_DuToan_ChiPhi>();

            foreach (var item in lstChiPhi)
            {
                SetDmDuAnChiPhi(item, ref dicChiPhiId, ref lstDmChiPhi);
                lstDuToanChiPhi.Add(SetDuToanChiPhi(iIdDuToanId, item, dicChiPhiId));
            }

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    conn.Insert<VDT_DM_DuAn_ChiPhi>(lstDmChiPhi, trans);
                    conn.Insert<VDT_DA_DuToan_ChiPhi>(lstDuToanChiPhi, trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        private void SetDmDuAnChiPhi(VDT_DA_DuToan_ChiPhi_ViewModel item, ref Dictionary<Guid, Guid> dicChiPhiId, ref List<VDT_DM_DuAn_ChiPhi> lstDmChiPhi)
        {
            VDT_DM_DuAn_ChiPhi data = new VDT_DM_DuAn_ChiPhi();
            if (!dicChiPhiId.ContainsKey(item.iID_DuAn_ChiPhi.Value))
                dicChiPhiId.Add(item.iID_DuAn_ChiPhi.Value, Guid.NewGuid());

            data.iID_DuAn_ChiPhi = dicChiPhiId[item.iID_DuAn_ChiPhi.Value];
            data.sTenChiPhi = item.sTenChiPhi;
            data.iThuTu = item.iThuTu;
            data.iID_ChiPhi = item.iID_ChiPhiID;

            if (item.iID_ChiPhi_Parent.HasValue)
            {
                if (!dicChiPhiId.ContainsKey(item.iID_ChiPhi_Parent.Value))
                    dicChiPhiId.Add(item.iID_ChiPhi_Parent.Value, Guid.NewGuid());
                data.iID_ChiPhi_Parent = dicChiPhiId[item.iID_ChiPhi_Parent.Value];
            }
            lstDmChiPhi.Add(data);
        }

        private VDT_DA_DuToan_ChiPhi SetDuToanChiPhi(Guid iIdDuToanId, VDT_DA_DuToan_ChiPhi_ViewModel item, Dictionary<Guid, Guid> dicChiPhiId)
        {
            VDT_DA_DuToan_ChiPhi data = new VDT_DA_DuToan_ChiPhi();
            data.iID_DuToan_ChiPhiID = Guid.NewGuid();
            data.iID_DuToanID = iIdDuToanId;
            data.iID_ChiPhiID = item.iID_ChiPhiID;
            data.iID_DuAn_ChiPhi = dicChiPhiId[item.iID_DuAn_ChiPhi.Value];
            data.fTienPheDuyet = item.fTienPheDuyet;
            data.fGiaTriDieuChinh = item.fGiaTriDieuChinh;
            data.fTienPheDuyetQDDT = item.fTienPheDuyetQDDT;
            return data;
        }

        private void SaveDataHangMuc(Guid iIdDuToanId, List<VDT_DA_DuToan_HangMuc_ViewModel> lstHangMuc, Dictionary<Guid, Guid> dicChiPhiId)
        {
            if (lstHangMuc == null) return;
            Dictionary<Guid, Guid> dicHangMucId = new Dictionary<Guid, Guid>();
            List<VDT_DA_DuToan_DM_HangMuc> lstDmHangMuc = new List<VDT_DA_DuToan_DM_HangMuc>();
            List<VDT_DA_DuToan_HangMuc> lstDuToanHangMuc = new List<VDT_DA_DuToan_HangMuc>();

            foreach (var item in lstHangMuc)
            {
                SetDmDuToanHangMuc(item, ref dicHangMucId, ref lstDmHangMuc);
                lstDuToanHangMuc.Add(SetDuToanHangMuc(iIdDuToanId, item, dicHangMucId, dicChiPhiId));
            }

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    conn.Insert<VDT_DA_DuToan_DM_HangMuc>(lstDmHangMuc, trans);
                    conn.Insert<VDT_DA_DuToan_HangMuc>(lstDuToanHangMuc, trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        private void SetDmDuToanHangMuc(VDT_DA_DuToan_HangMuc_ViewModel item, ref Dictionary<Guid, Guid> dicHangMucId, ref List<VDT_DA_DuToan_DM_HangMuc> lstDmHangMuc)
        {
            VDT_DA_DuToan_DM_HangMuc data = new VDT_DA_DuToan_DM_HangMuc();
            if (!dicHangMucId.ContainsKey(item.iID_HangMucID))
                dicHangMucId.Add(item.iID_HangMucID, Guid.NewGuid());

            data.Id = dicHangMucId[item.iID_HangMucID];
            data.sTenHangMuc = item.sTenHangMuc;
            if (item.iID_ParentID.HasValue)
            {
                if (!dicHangMucId.ContainsKey(item.iID_ParentID.Value))
                    dicHangMucId.Add(item.iID_ParentID.Value, Guid.NewGuid());
                data.iID_ParentID = dicHangMucId[item.iID_ParentID.Value];
            }
            data.maOrder = item.smaOrder;
            data.iID_LoaiCongTrinh = item.iID_LoaiCongTrinhID;
            data.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
            data.iID_HangMucPhanChia = item.iID_HangMucPhanChia;
            lstDmHangMuc.Add(data);
        }

        private VDT_DA_DuToan_HangMuc SetDuToanHangMuc(Guid iIdDuToanId, VDT_DA_DuToan_HangMuc_ViewModel item, Dictionary<Guid, Guid> dicHangMucId, Dictionary<Guid, Guid> dicChiPhiId)
        {
            VDT_DA_DuToan_HangMuc data = new VDT_DA_DuToan_HangMuc();
            data.iID_DuToanID = iIdDuToanId;
            data.iID_HangMucID = dicHangMucId[item.iID_HangMucID];
            double tryParse = 0;
            data.fTienPheDuyet = Double.TryParse(item.fTienPheDuyet.ToString(), out tryParse) && tryParse > 0 ? item.fTienPheDuyet : 0;
            data.iID_DuAn_ChiPhi = dicChiPhiId[item.iID_DuAn_ChiPhi];
            data.fGiaTriDieuChinh = item.fGiaTriDieuChinh;
            data.fTienPheDuyetQDDT = item.fTienPheDuyetQDDT;
            return data;
        }

        private List<SelectListItem> GetCbxLoaiDuToan()
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            lstData.Add(new SelectListItem() { Value = ((int)DuToanType.Type.TONG_DU_TOAN).ToString(), Text = DuToanType.TypeName.TONG_DU_TOAN });
            lstData.Add(new SelectListItem() { Value = ((int)DuToanType.Type.DU_TOAN).ToString(), Text = DuToanType.TypeName.DU_TOAN });
            return lstData;
        }

        private List<VDT_DA_DuToan_ChiPhi_ViewModel> SortChiPhi(List<VDT_DA_DuToan_ChiPhi_ViewModel> defaultData)
        {
            if (defaultData == null) return new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            List<VDT_DA_DuToan_ChiPhi_ViewModel> lstData = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            foreach (var item in defaultData.Where(n => !n.iID_ChiPhi_Parent.HasValue).OrderBy(n => (n.iThuTu ?? 0)))
            {
                lstData.AddRange(RecursiveChiPhi(item, defaultData));
            }
            return lstData;
        }

        private List<VDT_DA_DuToan_HangMuc_ViewModel> SortHangMuc(List<VDT_DA_DuToan_HangMuc_ViewModel> defaultData)
        {
            if (defaultData == null) return new List<VDT_DA_DuToan_HangMuc_ViewModel>();
            List<VDT_DA_DuToan_HangMuc_ViewModel> lstData = new List<VDT_DA_DuToan_HangMuc_ViewModel>();
            foreach (var item in defaultData.Where(n => !n.iID_ParentID.HasValue).OrderBy(n => (n.smaOrder)))
            {
                lstData.AddRange(RecursiveHangMuc(item, defaultData));
            }
            return lstData;
        }

        private List<VDT_DA_DuToan_ChiPhi_ViewModel> RecursiveChiPhi(VDT_DA_DuToan_ChiPhi_ViewModel parent, List<VDT_DA_DuToan_ChiPhi_ViewModel> defaultData)
        {
            List<VDT_DA_DuToan_ChiPhi_ViewModel> lstData = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            if (defaultData.Any(n => n.iID_ChiPhi_Parent == parent.iID_DuAn_ChiPhi))
            {
                parent.isParent = true;
            }
            lstData.Add(parent);
            foreach (var item in defaultData.Where(n => n.iID_ChiPhi_Parent == parent.iID_DuAn_ChiPhi))
            {
                lstData.AddRange(RecursiveChiPhi(item, defaultData));
            }
            return lstData;
        }

        private List<VDT_DA_DuToan_HangMuc_ViewModel> RecursiveHangMuc(VDT_DA_DuToan_HangMuc_ViewModel parent, List<VDT_DA_DuToan_HangMuc_ViewModel> defaultData)
        {
            List<VDT_DA_DuToan_HangMuc_ViewModel> lstData = new List<VDT_DA_DuToan_HangMuc_ViewModel>();
            if (defaultData.Any(n => n.iID_ParentID == parent.iID_HangMucID))
            {
                parent.isParent = true;
            }
            lstData.Add(parent);
            foreach (var item in defaultData.Where(n => n.iID_ParentID == parent.iID_HangMucID))
            {
                lstData.AddRange(RecursiveHangMuc(item, defaultData));
            }
            return lstData;
        }
        #endregion

        //public JsonResult LayThongTinChiTietDuAn(string iID)
        //{
        //    VDT_DA_DuAn duAn = _qLVonDauTuService.LayThongTinChiTietDuAn(Guid.Parse(iID));
        //    return Json(duAn, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult LayDuAnByDonViQLVaLoaiQuyetDinh(string iID_DonViQuanLyID, byte loaiQuyetDinh)
        //{
        //    List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.LayDuAnByDonViQLVaLoaiQuyetDinh(Guid.Parse(iID_DonViQuanLyID), loaiQuyetDinh).ToList();
        //    StringBuilder htmlString = new StringBuilder();
        //    if (lstDuAn != null && lstDuAn.Count > 0)
        //    {
        //        htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
        //        for (int i = 0; i < lstDuAn.Count; i++)
        //        {
        //            htmlString.AppendFormat("<option value='{0}' data-sMaDuAn='{1}'>{2}</option>", lstDuAn[i].iID_DuAnID, lstDuAn[i].sMaDuAn, lstDuAn[i].sMaDuAn + " - " + lstDuAn[i].sTenDuAn);
        //        }
        //    }
        //    return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetChiPhiDauTu()
        {
            List<VDT_DM_ChiPhi> lstChiPhi = _qLVonDauTuService.LayChiPhi().ToList();
            StringBuilder htmlString = new StringBuilder();
            if (lstChiPhi != null && lstChiPhi.Count > 0)
            {
                htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < lstChiPhi.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' >{1}</option>", lstChiPhi[i].iID_ChiPhi, lstChiPhi[i].sTenChiPhi);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Kiểm tra trùng số QĐ QL TKTC và TDT
        /// </summary>
        /// <param name="sSoQuyetDinh">Số quyết định</param>
        /// <param name="iID_DuToanID">ID Dự toán TKTC và TDT</param>
        /// <returns></returns>
        public JsonResult KiemTraTrungSoQuyetDinh(string sSoQuyetDinh, string iID_DuToanID = "")
        {
            bool status = _qLVonDauTuService.KiemTraTrungSoQuyetDinhQLTKTCvaTDT(sSoQuyetDinh, iID_DuToanID);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult DieuChinh(Guid id)
        //{
        //    VDT_DA_DuToan_ViewModel item = _qLVonDauTuService.GetPheDuyetTKTCvaTDTByID(id);
        //    if (item == null)
        //        return RedirectToAction("Index");
        //    item.sSoQuyetDinh = null;
        //    item.dNgayQuyetDinh = null;
        //    return View(item);
        //}

        //public ActionResult SuaDieuChinh(Guid id)
        //{
        //    VDT_DA_DuToan_ViewModel item = _qLVonDauTuService.GetPheDuyetTKTCvaTDTByID(id);
        //    if (item == null)
        //        return RedirectToAction("Index");

        //    return View(item);
        //}

        public ActionResult Detail(Guid id)
        {
            VDT_DA_DuToan_ViewModel data = _qLVonDauTuService.GetPheDuyetTKTCvaTDTByID(id);
            bool bIsDieuChinh = false;
            if (data.bIsGoc == null && !data.bIsGoc.Value)
                bIsDieuChinh = true;

            ViewBag.bIsDieuChinh = bIsDieuChinh;
            ViewBag.ItemsDuToanType = new SelectList(GetCbxLoaiDuToan(), "Value", "Text", (data.bLaTongDuToan ? 1 : 0));
            ViewBag.sNgayQuyetDinhDefault = data.dNgayQuyetDinh.HasValue ? data.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
            return View(data);
        }

        [HttpPost]
        public JsonResult GetListChiPhiByPheDuyetDuAn(Guid idDuAn)
        {
            var qdDauTu = _qLVonDauTuService.FindQDDauTuByDuAnId(idDuAn);
            if (qdDauTu == null)
            {
                return Json(new { status = false });
            }
            var listModel = _qLVonDauTuService.GetListChiPhiTheoQDDauTuDauTu(qdDauTu.iID_QDDauTuID);
            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListNguonVonByPheDuyetDuAn(Guid idDuAn)
        {
            var qdDauTu = _qLVonDauTuService.FindQDDauTuByDuAnId(idDuAn);
            if (qdDauTu == null)
            {
                return Json(new { status = false });
            }
            var listModel = _qLVonDauTuService.GetListNguonVonTheoQDDauTuDauTu(qdDauTu.iID_QDDauTuID);
            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListNguonVonByTKTC(Guid id)
        {
            var listModel = _qLVonDauTuService.GetListNguonVonTheoTKTC(id);
            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListChiPhiByTKTC(Guid id)
        {
            var listModel = _qLVonDauTuService.GetListChiPhiTheoTKTC(id);
            return Json(new { status = true, data = listModel });
        }

        [HttpPost]
        public JsonResult GetListHangMucByPheDuyetDuAn(Guid idDuAn)
        {
            var qdDauTu = _qLVonDauTuService.FindQDDauTuByDuAnId(idDuAn);
            if (qdDauTu == null)
            {
                return Json(new { status = false });
            }
            var listModel = _qLVonDauTuService.GetListHangMucTheoQDDauTu(qdDauTu.iID_QDDauTuID);
            return Json(new { status = true, data = listModel });
        }
        public JsonResult GetListHangMucByTKTC(Guid id)
        {
            var listModel = _qLVonDauTuService.GetListHangMucTheoTKTC(id);
            return Json(new { status = true, data = listModel });
        }

    }
}