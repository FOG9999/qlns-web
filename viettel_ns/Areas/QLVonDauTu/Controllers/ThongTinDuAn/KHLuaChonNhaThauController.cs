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
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Common;
using VIETTEL.Controllers;
using static VIETTEL.Common.Constants;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class KHLuaChonNhaThauController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        private QLKeHoachVonNamModel _modelKHV = new QLKeHoachVonNamModel();
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;

        #region Index
        public ActionResult Index()
        {
            var dataDropDown = _modelKHV.GetDataDropdownDonViQuanLy(Username);
            dataDropDown.Insert(0, new SelectListItem { Text = "", Value = "" });
            ViewBag.drpDonViQuanLy = dataDropDown;

            VDTKHLuaChonNhaThauPagingModel data = new VDTKHLuaChonNhaThauPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _iQLVonDauTuService.GetAllKHLuaChonNhaThauPaging(ref data._paging, string.Empty, string.Empty, null, null, null);

            return View(data);
        }

        [HttpPost]
        public ActionResult KHLuaChonNhaThauView(PagingInfo _paging, string sSoQuyetDinh, string sTenDuAn, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? sDonViQuanLy)
        {
            VDTKHLuaChonNhaThauPagingModel data = new VDTKHLuaChonNhaThauPagingModel();
            data._paging = _paging;
            data.lstData = _iQLVonDauTuService.GetAllKHLuaChonNhaThauPaging(ref data._paging, sSoQuyetDinh, sTenDuAn, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sDonViQuanLy);
            var dataDropDown = _modelKHV.GetDataDropdownDonViQuanLy(Username);
            dataDropDown.Insert(0, new SelectListItem { Text = "", Value = "" });
            ViewBag.drpDonViQuanLy = dataDropDown;
            return PartialView("_list", data);
        }

        #endregion

        #region Update
        public ActionResult Update(Guid? id, bool bIsDieuChinh = false)
        {
            VDTKHLuaChonNhaThauViewModel data = new VDTKHLuaChonNhaThauViewModel();
            ViewBag.Title = "Thêm mới kế hoạch lựa chọn nhà thầu";
            if (id.HasValue && id != Guid.Empty)
            {
                if (bIsDieuChinh)
                    ViewBag.Title = "Điều chỉnh kế hoạch lựa chọn nhà thầu";
                else
                    ViewBag.Title = "Cập nhật kế hoạch lựa chọn nhà thầu";

                data = _iQLVonDauTuService.GetDetailKHLuaChonNhaThau(id.Value);
            }
            if (data == null) return View("Index");
            ViewBag.bIsDieuChinh = bIsDieuChinh ? 1 : 0;
            ViewBag.ItemsChuDauTu = new SelectList(GetCbxChuDauTu(), "Value", "Text");
            if(data.iID_ChuTruongDauTuID != null)
            {
                ViewBag.ItemsLoaiChungTu = new SelectList(GetCbxLoaiChungTu(), "Value", "Text", ((int)CanCuType.Type.CHU_TRUONG_DAU_TU).ToString());
            }
            else if (data.iID_QDDauTuID != null)
            {
                ViewBag.ItemsLoaiChungTu = new SelectList(GetCbxLoaiChungTu(), "Value", "Text", ((int)CanCuType.Type.QUYET_DINH_DAU_TU).ToString());
            }
            else ViewBag.ItemsLoaiChungTu = new SelectList(GetCbxLoaiChungTu(), "Value", "Text", ((int)CanCuType.Type.TKTC_TONG_DU_TOAN).ToString());
            ViewBag.sNgayQuyetDinhDefault = data.dNgayQuyetDinh.HasValue ? data.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.iTypeDuToan = (int)CanCuType.Type.TKTC_TONG_DU_TOAN;
            return View(data);
        }

        [HttpGet]
        public JsonResult GetCbxDonViQuanLy()
        {
            List<NS_DonVi> lstDonViQL = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            string sCbxData = string.Join(",", lstDonViQL.Select(n => string.Format("<option value='{0}' data-iIdDonVi='{1}'>{2}</option>", n.iID_MaDonVi, n.iID_Ma.ToString(), n.sTen)));
            return Json(new { data = sCbxData }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FindDuAn(string iID_MaDonViQuanLyID, int iLoaiChungTu)
        {
            List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnTaoMoiKHLCNT(iID_MaDonViQuanLyID, iLoaiChungTu).ToList();
            return Json(new { data = lstDuAn}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get data dropdown
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDataDropdown()
        {
            var phuongThucLuaChonNT = new List<SelectListItem>();
            var hinhThucChonNT = new List<SelectListItem>();
            var loaiHopDong = new List<SelectListItem>();
            GetDataComboboxGoiThau(ref phuongThucLuaChonNT, ref hinhThucChonNT, ref loaiHopDong);
            phuongThucLuaChonNT.Insert(0, new SelectListItem { Text = "", Value = "" });
            hinhThucChonNT.Insert(0, new SelectListItem { Text = "", Value = "" });
            loaiHopDong.Insert(0, new SelectListItem { Text = "", Value = "" });
            return Json(new { phuongThucLuaChonNT = phuongThucLuaChonNT, hinhThucChonNT = hinhThucChonNT, loaiHopDong = loaiHopDong }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetListChungTu(Guid iIdDuAnId, int iLoaiChungTu)
        {
            var data = _iQLVonDauTuService.GetChungTuByDuAnAndLoaiChungTu(iIdDuAnId, iLoaiChungTu);

            var phuongThucLuaChonNT = new List<SelectListItem>();
            var hinhThucChonNT = new List<SelectListItem>();
            var loaiHopDong = new List<SelectListItem>();
            GetDataComboboxGoiThau(ref phuongThucLuaChonNT, ref hinhThucChonNT, ref loaiHopDong);
            ViewBag.lstPhuongThucLuaChonNT = phuongThucLuaChonNT;
            ViewBag.lstHinhThucChonNT = hinhThucChonNT;
            ViewBag.lstLoaiHopDong = loaiHopDong;

            if (data == null)
                return PartialView("_listChungTu", new List<VDTKHLCNhaThauChungTuViewModel>());
            return PartialView("_listChungTu", data.ToList());
        }

        [HttpPost]
        public JsonResult GetAllChungTuDetail(List<Guid> lstChungTu, int iLoaiChungTu)
        {
            return Json(new { data = _iQLVonDauTuService.GetChungTuDetailByListChungTuId(lstChungTu, iLoaiChungTu) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetChungTuDetailByKhlcntId(Guid id)
        {
            return Json(new { data = _iQLVonDauTuService.GetChungTuDetailByKHLCNTId(id) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllGoiThauByKhlcntId(Guid id,bool bIsDieuChinh = false)
        {
            var lstData = _iQLVonDauTuService.GetListGoiThauByKHLuaChonNhaThauID(id);
            if(!bIsDieuChinh || lstData == null)
            {
                return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstDataDieuChinh = lstData.Select(n => { n.iID_ParentID = n.iID_GoiThauID; n.iID_GoiThauID = Guid.NewGuid(); return n; });
                return Json(new { data = lstDataDieuChinh }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveKeHoachLuaChonNhaThau( List<VDT_DA_GoiThau> lstGoiThau, bool bIsDieuChinh = false)
        {
            return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Helper
        private List<SelectListItem> GetCbxLoaiChungTu()
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            lstData.Add(new SelectListItem() { Value = ((int)CanCuType.Type.QUYET_DINH_DAU_TU).ToString(), Text = CanCuType.TypeName.QUYET_DINH_DAU_TU });
            lstData.Add(new SelectListItem() { Value = ((int)CanCuType.Type.TKTC_TONG_DU_TOAN).ToString(), Text = CanCuType.TypeName.TKTC_TONG_DU_TOAN });
            lstData.Add(new SelectListItem() { Value = ((int)CanCuType.Type.CHU_TRUONG_DAU_TU).ToString(), Text = CanCuType.TypeName.CHU_TRUONG_DAU_TU });
            return lstData;
        }

        private List<SelectListItem> GetCbxChuDauTu()
        {
            var lstChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec);
            if (lstChuDauTu == null) return new List<SelectListItem>();
            return lstChuDauTu.Select(n => new SelectListItem() { Value = n.sId_CDT, Text = n.sTenCDT }).ToList();
        }

        private void GetDataComboboxGoiThau(ref List<SelectListItem> phuongThucLuaChonNT, ref List<SelectListItem> hinhThucChonNT, ref List<SelectListItem> loaiHopDong)
        {
            phuongThucLuaChonNT.Add(new SelectListItem() { Value = PTDauThauTypeName.PT_1, Text = PTDauThauTypeName.PT_1 });
            phuongThucLuaChonNT.Add(new SelectListItem() { Value = PTDauThauTypeName.PT_2, Text = PTDauThauTypeName.PT_2 });
            phuongThucLuaChonNT.Add(new SelectListItem() { Value = PTDauThauTypeName.PT_3, Text = PTDauThauTypeName.PT_3 });
            phuongThucLuaChonNT.Add(new SelectListItem() { Value = PTDauThauTypeName.PT_4, Text = PTDauThauTypeName.PT_4 });

            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_1, Text = HTChonNhaThauTypeName.HT_1 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_2, Text = HTChonNhaThauTypeName.HT_2 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_3, Text = HTChonNhaThauTypeName.HT_3 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_4, Text = HTChonNhaThauTypeName.HT_4 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_5, Text = HTChonNhaThauTypeName.HT_5 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_6, Text = HTChonNhaThauTypeName.HT_6 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_7, Text = HTChonNhaThauTypeName.HT_7 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_8, Text = HTChonNhaThauTypeName.HT_8 });
            hinhThucChonNT.Add(new SelectListItem() { Value = HTChonNhaThauTypeName.HT_10, Text = HTChonNhaThauTypeName.HT_10 });

            loaiHopDong.Add(new SelectListItem() { Value = HTHopDongTypeName.HD_1, Text = HTHopDongTypeName.HD_1 });
            loaiHopDong.Add(new SelectListItem() { Value = HTHopDongTypeName.HD_2, Text = HTHopDongTypeName.HD_2 });
            loaiHopDong.Add(new SelectListItem() { Value = HTHopDongTypeName.HD_3, Text = HTHopDongTypeName.HD_3 });
        }
        #endregion





        public ActionResult CreateNew()
        {
            List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.ToSelectList("ID", "sTenCDT");

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");

            return View();
        }

        public ActionResult DieuChinh(Guid id)
        {
            var data = _iQLVonDauTuService.GetDetailKHLuaChonNhaThau(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            data.sSoQuyetDinh = null;
            data.dNgayQuyetDinh = null;
            return View(data);
        }

        public JsonResult CheckExistSoQuyetDinh(string checkVal, Guid? id)
        {
            bool isExisted = _iQLVonDauTuService.CheckExitsSoQuyetDinhKHLCNT(checkVal, id);
            return Json(new { isExisted }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OnSave(VDTKHLuaChonNhaThauModel data)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var entityKHLuaChonNhaThau = new VDT_QDDT_KHLCNhaThau();
                    if (data.objKHLuaChonNhaThau.Id == Guid.Empty)
                    {
                        #region Them moi VDT_QDDT_KHLCNhaThau
                        entityKHLuaChonNhaThau.MapFrom(data.objKHLuaChonNhaThau);

                        if (data.isDieuChinh)
                        {
                            entityKHLuaChonNhaThau.bIsGoc = false;
                            // update khlcnt cha bactive = 0
                            var entityParent = conn.Get<VDT_QDDT_KHLCNhaThau>(data.objKHLuaChonNhaThau.iID_ParentID, trans);
                            if (entityParent != null)
                            {
                                entityParent.bActive = false;
                                conn.Update(entityParent, trans);
                            }
                        }
                        else
                        {
                            entityKHLuaChonNhaThau.bIsGoc = true;
                        }
                       
                        entityKHLuaChonNhaThau.bActive = true;
                        entityKHLuaChonNhaThau.sUserCreate = Username;
                        entityKHLuaChonNhaThau.dDateCreate = DateTime.Now;
                        conn.Insert(entityKHLuaChonNhaThau, trans);
                        if (data.isDieuChinh)
                        {
                            entityKHLuaChonNhaThau.iID_LCNhaThauGocID = data.objKHLuaChonNhaThau.iID_LCNhaThauGocID;
                        }
                        else
                        {
                            entityKHLuaChonNhaThau.iID_LCNhaThauGocID = entityKHLuaChonNhaThau.Id;
                            conn.Update(entityKHLuaChonNhaThau, trans);
                        }
                        
                        #endregion

                    }
                    else
                    {

                        entityKHLuaChonNhaThau = conn.Get<VDT_QDDT_KHLCNhaThau>(data.objKHLuaChonNhaThau.Id, trans);
                        entityKHLuaChonNhaThau.sSoQuyetDinh = data.objKHLuaChonNhaThau.sSoQuyetDinh;
                        entityKHLuaChonNhaThau.dNgayQuyetDinh = data.objKHLuaChonNhaThau.dNgayQuyetDinh;
                        entityKHLuaChonNhaThau.sMoTa = data.objKHLuaChonNhaThau.sMoTa;

                        conn.Update(entityKHLuaChonNhaThau, trans);

                    }

                    if (data.objKHLuaChonNhaThau.Id != null && data.objKHLuaChonNhaThau.Id != Guid.Empty)
                    {
                        bool deleteChild = _iQLVonDauTuService.DeleteChildGoiThau(data.objKHLuaChonNhaThau.Id);//xóa các gói thầu cũ vào tạo các gói thầu mới
                    }

                    #region Them moi VDT_DA_GoiThau
                    if (data.lstGoiThau != null && data.lstGoiThau.Count() > 0)
                    {
                        for (int i = 0; i < data.lstGoiThau.Count(); i++)
                        {
                            var entityGoiThau = new VDT_DA_GoiThau();
                            entityGoiThau.MapFrom(data.lstGoiThau.ToList()[i]);

                            if (entityGoiThau.iID_ParentID != null && entityGoiThau.iID_ParentID != Guid.Empty)
                            {
                                entityGoiThau.bIsGoc = false;
                                //entityKHLuaChonNhaThau.iID_ParentID = data.objKHLuaChonNhaThau.iID_ParentID

                                // update khlcnt cha bactive = 0
                                var entityParent = conn.Get<VDT_DA_GoiThau>(entityGoiThau.iID_ParentID, trans);
                                if (entityParent != null)
                                {
                                    entityParent.bActive = false;
                                    conn.Update(entityParent, trans);
                                }
                            }
                            else
                            {
                                entityGoiThau.bIsGoc = true;
                            }

                            
                            entityGoiThau.bActive = true;
                            entityGoiThau.iId_KHLCNhaThau = entityKHLuaChonNhaThau.Id;

                            entityGoiThau.sUserCreate = Username;
                            entityGoiThau.dDateCreate = DateTime.Now;
                            conn.Insert(entityGoiThau, trans);

                            #region Them moi VDT_DA_GoiThau_NguonVon
                            //List<VDT_DA_GoiThau_NguonVon> lstNguonVon = data.lstDetail.Where(x => x.iID_GoiThauID == entityGoiThau.iID_GoiThauID).ToList();
                            //if (lstNguonVon != null && lstNguonVon.Count() > 0)
                            //{
                            //    for (int j = 0; j < lstNguonVon.Count(); j++)
                            //    {
                            //        var entityGoiThauNguonVon = new VDT_DA_GoiThau_NguonVon();
                            //        entityGoiThauNguonVon.MapFrom(lstNguonVon[j]);

                            //        conn.Insert(entityGoiThauNguonVon, trans);
                            //    }
                            //}
                            //#endregion

                            //#region Them moi VDT_DA_GoiThau_ChiPhi
                            //List<VDT_DA_GoiThau_ChiPhi> lstChiPhi = data.lstGoiThauChiPhi.ToList().Where(x =>  x.iID_GoiThauID == entityGoiThau.iID_GoiThauID).ToList();
                            //if (lstChiPhi != null && lstChiPhi.Count() > 0)
                            //{
                            //    for (int j = 0; j < lstChiPhi.Count(); j++)
                            //    {
                            //        var entityGoiThauChiPhi = new VDT_DA_GoiThau_ChiPhi();
                            //        entityGoiThauChiPhi.MapFrom(lstChiPhi[j]);

                            //        conn.Insert(entityGoiThauChiPhi, trans);

                            //        #region Them moi VDT_DA_GoiThau_HangMuc
                            //        if (data.lstGoiThauHangMuc != null)
                            //        {
                            //            List<VDT_DA_GoiThau_HangMuc> lstHangMuc = data.lstGoiThauHangMuc.ToList().Where(x => x.iID_GoiThauID == entityGoiThau.iID_GoiThauID && x.iID_ChiPhiID == entityGoiThauChiPhi.iID_ChiPhiID).ToList();
                            //            if (lstHangMuc != null && lstHangMuc.Count() > 0)
                            //            {
                            //                for (int k = 0; k < lstHangMuc.Count(); k++)
                            //                {
                            //                    var entityGoiThauHangMuc = new VDT_DA_GoiThau_HangMuc();
                            //                    entityGoiThauHangMuc.iID_GoiThauID = lstHangMuc[k].iID_GoiThauID;
                            //                    entityGoiThauHangMuc.iID_HangMucID = lstHangMuc[k].iID_HangMucID;
                            //                    entityGoiThauHangMuc.iID_ChiPhiID = lstHangMuc[k].iID_ChiPhiID;
                            //                    entityGoiThauHangMuc.fTienGoiThau = lstHangMuc[k].fTienGoiThau;
                            //                    conn.Insert(entityGoiThauHangMuc, trans);
                            //                }
                            //            }
                            //        }
                            //        #endregion
                            //    }
                            //}
                            #endregion
                        }
                    }

                    SaveDetail(data.lstDetail);
                    #endregion
                    // commit to db
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(Guid id)
        {
            var data = _iQLVonDauTuService.GetDetailKHLuaChonNhaThau(id);
            return View(data);
        }

        [HttpPost]
        public JsonResult DeleteItem(Guid id)
        {
            bool status = _iQLVonDauTuService.DeleteKHLuaChonNhaThau(id);
            return Json(new { bIsComplete = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListGoiThauGoc(Guid lcntParentId)
        {
            var listGoiThau = new List<VDT_DA_GoiThau>();
            //listGoiThau.Insert(0, new VDT_DA_GoiThau { iID_GoiThauID = Guid.Empty, sTenGoiThau = Constants.CHON });
            listGoiThau = _iQLVonDauTuService.GetListGoiThauByKHLuaChonNhaThauID(lcntParentId);
            
            return Json(listGoiThau, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDanhSachDuToanTheoDuAn(string iID, int loaiChungTu)
        {
            List<VDTDADuToanModel> lstDuToan = _iQLVonDauTuService.GetDuToanByDuAnId(Guid.Parse(iID), loaiChungTu);
            List<VDT_DA_DuToan_ViewModel> lstDuToanChiTiet = new List<VDT_DA_DuToan_ViewModel>();
            foreach (var item in lstDuToan)
            {
                lstDuToanChiTiet.Add(_iQLVonDauTuService.GetPheDuyetTKTCvaTDTByIDFix(item.iID_DuToanID, loaiChungTu));
            }
            return Json(lstDuToanChiTiet, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDanhSachDuToanDieuChinhTheoDuAn(string iID)
        {
            List<VDTDADuToanModel> lstDuToan = _iQLVonDauTuService.GetDuToanDieuChinhByDuAnId(Guid.Parse(iID));
            List<VDT_DA_DuToan_ViewModel> lstDuToanChiTiet = new List<VDT_DA_DuToan_ViewModel>();
            foreach (var item in lstDuToan)
            {
                lstDuToanChiTiet.Add(_iQLVonDauTuService.GetPheDuyetTKTCvaTDTByID(item.iID_DuToanID));
            }
            return Json(lstDuToanChiTiet, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDanhSachDuToanTheoKHLCNhaThauId(Guid duToanId)
        {
            List<VDT_DA_DuToan_ViewModel> lstDuToanChiTiet = new List<VDT_DA_DuToan_ViewModel>();

            lstDuToanChiTiet.Add(_iQLVonDauTuService.GetPheDuyetTKTCvaTDTByID(duToanId));

            return Json(lstDuToanChiTiet, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// lay thong tin goi thau va goi thau chi tiet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LayThongTinGoiThauChiTietTheoKHLuaChonNhaThau(Guid? id)
        {
            List<VDT_DA_GoiThau_NguonVon> lstGoiThauNguonVon = new List<VDT_DA_GoiThau_NguonVon>();
            List<VDT_DA_GoiThau_ChiPhi> lstGoiThauChiPhi = new List<VDT_DA_GoiThau_ChiPhi>();
            List<VDT_DA_GoiThau_HangMuc> lstGoiThauHangMuc = new List<VDT_DA_GoiThau_HangMuc>();
            List<VDT_DA_GoiThau> lstGoiThau = _iQLVonDauTuService.GetListGoiThauByKHLuaChonNhaThauID(id);
            List<VDT_DM_DuAn_ChiPhi> lstDuAnChiPhis = new List<VDT_DM_DuAn_ChiPhi>();
            foreach (var item in lstGoiThau)
            {
                lstGoiThauNguonVon.AddRange(_iQLVonDauTuService.GetListGoiThauNguonVonByGoiThauID(item.iID_GoiThauID));
                lstGoiThauChiPhi.AddRange(_iQLVonDauTuService.GetListGoiThauChiPhiByGoiThauID(item.iID_GoiThauID));
                lstGoiThauHangMuc.AddRange(_iQLVonDauTuService.GetListGoiThauHangMucByGoiThauID(item.iID_GoiThauID));
            }

            foreach (var item in lstGoiThauChiPhi)
            {
                lstDuAnChiPhis.AddRange(_iQLVonDauTuService.GetListDuAnChiPhis(item.iID_ChiPhiID));
            }
            return Json(new { lstGoiThau = lstGoiThau, lstGoiThauNguonVon = lstGoiThauNguonVon, lstGoiThauChiPhi = lstGoiThauChiPhi, lstGoiThauHangMuc = lstGoiThauHangMuc, lstDuAnChiPhis = lstDuAnChiPhis }, JsonRequestBehavior.AllowGet);
        }

        private void SaveDetail(IEnumerable<VDTKHLCNTDetailViewModel> lstDetail)
        {
            if(lstDetail != null)
            {
                Dictionary<Guid, Guid> dicChiPhi = new Dictionary<Guid, Guid>();
                List<VDTKHLCNTDetailViewModel> lstNguonVon = lstDetail.Where(n => n.iID_NguonVonID != 0).ToList();
                if (lstNguonVon != null) SaveNguonVon(lstNguonVon);
            
                List<VDTKHLCNTDetailViewModel> lstChiPhi = lstDetail.Where(n => n.iID_NguonVonID == 0 && !n.iID_HangMucID.HasValue).ToList();
                if (lstChiPhi != null) SaveChiPhi(lstChiPhi, ref dicChiPhi);

                List<VDTKHLCNTDetailViewModel> lstHangMuc = lstDetail.Where(n => n.iID_HangMucID.HasValue).ToList();
                if (lstHangMuc != null) SaveHangMuc(lstHangMuc, dicChiPhi);
            }
        }

        private void SaveNguonVon(List<VDTKHLCNTDetailViewModel> lstDetail)
        {
            List<VDT_DA_GoiThau_NguonVon> lstData = lstDetail.Select(n => new VDT_DA_GoiThau_NguonVon()
            {
                fTienGoiThau = n.fGiaTriGoiThau,
                iID_GoiThauID = n.iID_GoiThauID,
                iID_GoiThau_NguonVonID = Guid.NewGuid(),
                iID_NguonVonID = n.iID_NguonVonID,
            }).ToList();
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                conn.Insert<VDT_DA_GoiThau_NguonVon>(lstData, trans);
                trans.Commit();
            }
        }

        private void SaveChiPhi(List<VDTKHLCNTDetailViewModel> lstDetail, ref Dictionary<Guid, Guid> dicChiPhi)
        {
            dicChiPhi = new Dictionary<Guid, Guid>();
            Dictionary<Guid, VDT_DM_DuAn_ChiPhi> lstDmChiPhi = new Dictionary<Guid, VDT_DM_DuAn_ChiPhi>();
            List<VDT_DA_GoiThau_ChiPhi> lstData = new List<VDT_DA_GoiThau_ChiPhi>();
            foreach(var item in lstDetail)
            {
                SetDmDuAnChiPhi(item, ref dicChiPhi, ref lstDmChiPhi);
                lstData.Add(SetGoiThauChiPhi(item, dicChiPhi));
            }
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                conn.Insert<VDT_DM_DuAn_ChiPhi>(lstDmChiPhi.Values, trans);
                conn.Insert<VDT_DA_GoiThau_ChiPhi>(lstData, trans);
                trans.Commit();
            }
        }

        private void SaveHangMuc(List<VDTKHLCNTDetailViewModel> lstDetail, Dictionary<Guid, Guid> dicChiPhi)
        {
            if (lstDetail == null) return;
            Dictionary<Guid, Guid> dicHangMucId = new Dictionary<Guid, Guid>();
            List<VDT_DA_GoiThau_HangMuc> lstDuToanHangMuc = new List<VDT_DA_GoiThau_HangMuc>();

            foreach (var item in lstDetail)
            {
                VDT_DA_GoiThau_HangMuc child = new VDT_DA_GoiThau_HangMuc();
                child.iID_GoiThau_HangMucID = Guid.NewGuid();
                if(item.iID_ChiPhiID.HasValue && dicChiPhi.ContainsKey(item.iID_ChiPhiID.Value))
                {
                    child.iID_ChiPhiID = dicChiPhi[item.iID_ChiPhiID.Value];
                }
                child.iID_GoiThauID = item.iID_GoiThauID;
                child.iID_HangMucID = item.iID_HangMucID;
                child.fTienGoiThau = item.fGiaTriGoiThau;
                lstDuToanHangMuc.Add(child);
            }

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    conn.Insert<VDT_DA_GoiThau_HangMuc>(lstDuToanHangMuc, trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }
        }

        private void SetDmDuAnChiPhi(VDTKHLCNTDetailViewModel item, ref Dictionary<Guid, Guid> dicChiPhiId, ref Dictionary<Guid, VDT_DM_DuAn_ChiPhi> lstDmChiPhi)
        {
            VDT_DM_DuAn_ChiPhi data = new VDT_DM_DuAn_ChiPhi();
            if (!dicChiPhiId.ContainsKey(item.iID_ChiPhiID.Value))
                dicChiPhiId.Add(item.iID_ChiPhiID.Value, Guid.NewGuid());

            data.iID_DuAn_ChiPhi = dicChiPhiId[item.iID_ChiPhiID.Value];
            data.sTenChiPhi = item.sNoiDung;
            data.iThuTu = item.iThuTu;
            data.iID_ChiPhi = item.iID_ChiPhiID;

            if (item.iID_ParentId.HasValue)
            {
                if (!dicChiPhiId.ContainsKey(item.iID_ParentId.Value))
                    dicChiPhiId.Add(item.iID_ParentId.Value, Guid.NewGuid());
                data.iID_ChiPhi_Parent = dicChiPhiId[item.iID_ParentId.Value];
            }
            if(!lstDmChiPhi.ContainsKey(item.iID_ChiPhiID.Value))
                lstDmChiPhi.Add(item.iID_ChiPhiID.Value, data);
        }

        private VDT_DA_GoiThau_ChiPhi SetGoiThauChiPhi(VDTKHLCNTDetailViewModel item, Dictionary<Guid, Guid> dicChiPhi)
        {
            VDT_DA_GoiThau_ChiPhi data = new VDT_DA_GoiThau_ChiPhi();
            data.iID_GoiThau_ChiPhiID = Guid.NewGuid();
            data.iID_GoiThauID = item.iID_GoiThauID;
            data.iID_ChiPhiID = dicChiPhi[item.iID_ChiPhiID.Value];
            data.fTienGoiThau = item.fGiaTriGoiThau;
            return data;
        }
    }
}