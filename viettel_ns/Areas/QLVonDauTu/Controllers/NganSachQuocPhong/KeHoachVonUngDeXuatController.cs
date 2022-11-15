using DapperExtensions;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.BaoHiemXaHoi;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.z.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;
using static System.Net.WebRequestMethods;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class KeHoachVonUngDeXuatController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        IQLVonDauTuService _qlVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;

        #region View
        // GET: QLVonDauTu/KeHoachVonUngDeXuat
        public ActionResult Index()
        {
            VDTKeHoachVonUngDeXuatPagingModel vm = new VDTKeHoachVonUngDeXuatPagingModel();
            vm._paging.CurrentPage = 1;

            ViewBag.ListDonViQuanLy = CreateSelectListDonVi();
            ViewBag.LstNguonVon = CommonFunction.GetDataDropDownNguonNganSach();

            List<VDTKeHoachVonUngDeXuatModel> lstKHVUDX = _qlVonDauTuService.GetKeHoachVonUngDeXuatByCondition(ref vm._paging).ToList();
            List<VDTKeHoachVonUngDeXuatModel> lstChungTuTongHopParent = lstKHVUDX.Where(x => !string.IsNullOrEmpty(x.sTongHop)).ToList();
            Dictionary<string, List<string>> dctChungTu = lstChungTuTongHopParent.GroupBy(x => x.Id).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

            List<VDTKeHoachVonUngDeXuatModel> lstAllChungTuTongHopParent = new List<VDTKeHoachVonUngDeXuatModel>();
            foreach (var key in dctChungTu.Keys)
            {
                string lstValue = dctChungTu[key].ToList().FirstOrDefault();
                List<string> lstChildrentId = new List<string>();
                if (lstValue.Contains(","))
                {
                    lstChildrentId = lstValue.Split(',').ToList();
                }
                else
                {
                    lstChildrentId.Add(lstValue);
                }

                VDTKeHoachVonUngDeXuatModel itemParent = lstKHVUDX.Where(x => x.Id == Guid.Parse(key)).FirstOrDefault();
                List<VDTKeHoachVonUngDeXuatModel> lstChildrent = lstKHVUDX.Where(x => lstChildrentId.Any(y => Guid.Parse(y) == x.Id)).ToList();
                // lstChungTuTongHopParent = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                lstAllChungTuTongHopParent.Add(itemParent);
                lstAllChungTuTongHopParent.AddRange(lstChildrent);
            }
            List<VDTKeHoachVonUngDeXuatModel> lstChungTu = lstKHVUDX.Where(x => !lstAllChungTuTongHopParent.Any(y => y.Id == x.Id)).ToList();

            vm.chungTuTabIndex = "checked";
            vm.chungTuTongHopTabIndex = "";
            vm.lstData = lstChungTu;
            vm._paging.TotalItems = lstChungTu.Count();
            return View(vm);
        }

        public ActionResult Update(Guid? id = null, int? iNamKeHoach = null, int? iNguonVon = null, string lstTongHop = null, bool isDieuChinh = false)
        {
            ViewBag.ListDonViQuanLy = CreateSelectListDonVi();
            ViewBag.LstNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            ViewBag.lstIdTongHop = lstTongHop;
            VdtKhvuDXChiTietModel data = new VdtKhvuDXChiTietModel();
            if (isDieuChinh)
            {
                ViewBag.Title = "Điều chỉnh kế hoạch vốn ứng đề xuất";
                data = _qlVonDauTuService.GetKeHoachVonUngChiTietById(id.Value, PhienLamViec.NamLamViec);
                if (!string.IsNullOrEmpty(data.sTongHop))
                    ViewBag.lstIdTongHop = data.sTongHop;
            }
            else
            {
                if (!id.HasValue || id.Value == Guid.Empty)
                {
                    ViewBag.Title = "Thêm mới kế hoạch vốn ứng đề xuất";
                    data.dNgayDeNghi = DateTime.Now;

                    if (lstTongHop != null && lstTongHop.Count() != 0)
                    {
                        data.iNamKeHoach = iNamKeHoach;
                        data.iID_NguonVonID = iNguonVon;
                    }
                }
                else
                {
                    ViewBag.Title = "Cập nhật kế hoạch vốn ứng đề xuất";
                    data = _qlVonDauTuService.GetKeHoachVonUngChiTietById(id.Value, PhienLamViec.NamLamViec);
                    if (!string.IsNullOrEmpty(data.sTongHop))
                        ViewBag.lstIdTongHop = data.sTongHop;
                }
            }
            @ViewBag.isDieuChinh = isDieuChinh ? "true" : "false";

            return View(data);
        }

        public ActionResult Detail(Guid id)
        {
            VdtKhvuDXChiTietModel data = _qlVonDauTuService.GetKeHoachVonUngChiTietById(id, PhienLamViec.NamLamViec);
            return View(data);
        }
        #endregion

        #region Event
        [HttpGet]
        public JsonResult GetDonViQuanLy()
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            if (lstDonViQuanLy == null || lstDonViQuanLy.Count == 0) return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            StringBuilder strDonVi = new StringBuilder();
            foreach (NS_DonVi item in lstDonViQuanLy)
            {
                strDonVi.AppendFormat("<option value='{0}' data-iIdDonVi='{1}'>{2}</option>", item.iID_MaDonVi, item.iID_Ma, string.Format(item.iID_MaDonVi + " - " + item.sTen));
            }
            return Json(new { status = true, datas = strDonVi.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TimKiemKHVUDeXuat(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom,
            DateTime? dNgayQuyetDinhTo, int? iNamKeHoach, int? iIdNguonVon, string iID_DonViQuanLyID, int? tabIndex)
        {
            if (string.IsNullOrEmpty(sSoQuyetDinh))
                sSoQuyetDinh = null;
            if (string.IsNullOrEmpty(iID_DonViQuanLyID))
                iID_DonViQuanLyID = null;
            VDTKeHoachVonUngDeXuatPagingModel vm = new VDTKeHoachVonUngDeXuatPagingModel();
            vm._paging = _paging;

            ViewBag.ListDonViQuanLy = CreateSelectListDonVi();
            ViewBag.LstNguonVon = CommonFunction.GetDataDropDownNguonNganSach();

            List<VDTKeHoachVonUngDeXuatModel> lstKHVUDX = _qlVonDauTuService.GetKeHoachVonUngDeXuatByCondition
                (ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iIdNguonVon, iID_DonViQuanLyID).ToList();
            List<VDTKeHoachVonUngDeXuatModel> lstChungTuTongHopParent = lstKHVUDX.Where(x => !string.IsNullOrEmpty(x.sTongHop)).ToList();
            Dictionary<string, List<string>> dctChungTu = lstChungTuTongHopParent.GroupBy(x => x.Id).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

            List<VDTKeHoachVonUngDeXuatModel> lstAllChungTuTongHopParent = new List<VDTKeHoachVonUngDeXuatModel>();
            foreach (var key in dctChungTu.Keys)
            {
                string lstValue = dctChungTu[key].ToList().FirstOrDefault();
                List<string> lstChildrentId = new List<string>();
                if (lstValue.Contains(","))
                {
                    lstChildrentId = lstValue.Split(',').ToList();
                }
                else
                {
                    lstChildrentId.Add(lstValue);
                }

                VDTKeHoachVonUngDeXuatModel itemParent = lstKHVUDX.Where(x => x.Id == Guid.Parse(key)).FirstOrDefault();
                List<VDTKeHoachVonUngDeXuatModel> lstChildrent = lstKHVUDX.Where(x => lstChildrentId.Any(y => Guid.Parse(y) == x.Id)).ToList();
                // lstChungTuTongHopParent = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                lstAllChungTuTongHopParent.Add(itemParent);
                lstAllChungTuTongHopParent.AddRange(lstChildrent);
            }
            List<VDTKeHoachVonUngDeXuatModel> lstChungTu = lstKHVUDX.Where(x => !lstAllChungTuTongHopParent.Any(y => y.Id == x.Id)).ToList();

            if (tabIndex == null || tabIndex.Value == 1)
            {
                vm.chungTuTabIndex = "checked";
                vm.chungTuTongHopTabIndex = "";
                vm.lstData = lstChungTu;
            }
            else
            {
                vm.chungTuTabIndex = "";
                vm.chungTuTongHopTabIndex = "checked";
                vm.lstData = lstAllChungTuTongHopParent;
            }
            vm._paging.TotalItems = vm.lstData.Count();
            return PartialView("_list", vm);
        }

        [HttpPost]
        public bool Delete(Guid id)
        {
            if (!_qlVonDauTuService.deleteKHVUDXChiTiet(id)) return false;
            if (!_qlVonDauTuService.deleteKHVUDX(id,Username)) return false;
            return true;
        }

        [HttpPost]
        public JsonResult GetKeHoachVonUngChiTiet(Guid id, bool bIsTongHop = false, string sMaDonVi = null)
        {
            var listdataChitiet = _qlVonDauTuService.GetKeHoachVonUngDeXuatDetailById(id).ToList();
            var lstAllData = _qlVonDauTuService.GetDuAnInVonUngDeXuatByIdDonVi(sMaDonVi).ToList();

            if (listdataChitiet == null || !listdataChitiet.Any())
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (bIsTongHop)
                {
                    List<string> lstCbxDuAn =
                    listdataChitiet.Select(n => string.Format(@"<option value='{0}' data-fTongMucDauTu='{1}' data-sMaDuAn='{2}' data-sTenDuAn='{3}' data-sTrangThaiDuAnDangKy='{4}'>{2} - {3}</option>",
                    n.iID_DuAnID, n.fTongMucDauTu, n.sMaDuAn, n.sTenDuAn, n.sTrangThaiDuAnDangKy)).ToList();
                    return Json(new { status = true, lstDetail = listdataChitiet, sCbxDuAn = string.Join("", lstCbxDuAn) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in listdataChitiet)
                    {
                        lstAllData.RemoveAll(x => x.iID_DuAnID == item.iID_DuAnID);
                    }
                    listdataChitiet.AddRange(lstAllData);

                    return Json(new { status = true, lstDetail = listdataChitiet }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult CheckExistSoQuyetDinh(Guid? iID_KeHoachUngID, string sSoQuyetDinh)
        {
            var isExist = _qlVonDauTuService.CheckExistSoDeNghiKHVNDX(iID_KeHoachUngID, sSoQuyetDinh);
            return Json(new { status = isExist }, JsonRequestBehavior.AllowGet); ;
        }

        public JsonResult QLKeHoachVonUngDxSave(VDT_KHV_KeHoachVonUng_DX data, List<VdtKhcKeHoachVonUngDeXuatChiTietModel> lstData, string id = null, bool isInsert = true, bool isTongHop = false, List<Guid> listIdChild = null, bool isDieuChinh = false)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (isDieuChinh)
                {
                    var entityGoc = conn.Get<VDT_KHV_KeHoachVonUng_DX>(data.Id, trans);
                    var entity = new VDT_KHV_KeHoachVonUng_DX();
                    entity.MapFrom(data);
                    entity.sUserCreate = Username;
                    entity.dDateCreate = DateTime.Now;
                    entity.Id = Guid.NewGuid();
                    entity.iID_ParentID = data.Id;
                    entity.bActive = true;
                    entity.bIsGoc = false;
                    conn.Insert<VDT_KHV_KeHoachVonUng_DX>(entity, trans);

                    entityGoc.bActive = false;
                    entityGoc.sUserUpdate = Username;
                    entityGoc.dDateUpdate = DateTime.Now;
                    conn.Update<VDT_KHV_KeHoachVonUng_DX>(entityGoc, trans);

                    id = entity.Id.ToString();
                }
                else
                {
                    if (data.Id == Guid.Empty)
                    {
                        #region Them moi VDT_KHV_KeHoachVonUng_DeXuat
                        var entityParent = new VDT_KHV_KeHoachVonUng_DX();
                        entityParent.MapFrom(data);

                        entityParent.sUserCreate = Username;
                        entityParent.dDateCreate = DateTime.Now;
                        entityParent.Id = Guid.NewGuid();
                        entityParent.bActive = true;
                        entityParent.bIsGoc = true;
                        entityParent.fGiaTriUng = lstData.Where(x => x.isDelete == false).Sum(n => n.fGiaTriDeNghi);
                        if (isTongHop)
                        {
                            if (listIdChild == null)
                            {
                                return Json(new { status = false, isinsert = isInsert }, JsonRequestBehavior.AllowGet);
                            }
                            entityParent.sTongHop = string.Join(",", listIdChild.Select(n => n.ToString()));
                            foreach (var item in listIdChild)
                            {
                                var entityChild = conn.Get<VDT_KHV_KeHoachVonUng_DX>(item, trans);
                                entityChild.iID_TonghopParent = entityParent.Id;
                                entityChild.sUserUpdate = Username;
                                entityChild.dDateUpdate = DateTime.Now;
                                conn.Update<VDT_KHV_KeHoachVonUng_DX>(entityChild, trans);
                            }

                        }
                        conn.Insert(entityParent, trans);
                        id = entityParent.Id.ToString();
                        #endregion
                        isInsert = true;
                        #region Them moi VDT_KHV_KeHoachVonUng_DeXuat_ChiTiet
                        //if (lstData != null && lstData.Count() > 0)
                        //{
                        //    for (int i = 0; i < lstData.Count(); i++)
                        //    {
                        //        var entityKHVUChiTiet = new VDT_KHV_KeHoachVonUng_DX_ChiTiet();
                        //        entityKHVUChiTiet.MapFrom(lstData.ToList()[i]);
                        //        if (lstData.ToList()[i].isDelete)
                        //            entityKHVUChiTiet.fGiaTriDeNghi = 0;
                        //        entityKHVUChiTiet.iID_KeHoachUngID = entityKHVU.Id;
                        //        conn.Insert(entityKHVUChiTiet, trans);
                        //    }
                        //}

                        #endregion
                    }
                    else
                    {
                        #region Sua KHVU
                        var entity = conn.Get<VDT_KHV_KeHoachVonUng_DX>(data.Id, trans);
                        entity.sSoDeNghi = data.sSoDeNghi;
                        entity.dNgayDeNghi = data.dNgayDeNghi;
                        //entity.fGiaTriUng = data.fGiaTriUng;
                        entity.sUserUpdate = Username;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);
                        #endregion

                        #region Them moi VDT_KHV_KeHoachVonUng_ChiTiet
                        //delete all KHVUChiTiet
                        //_qlVonDauTuService.deleteKHVUDXChiTiet(data.Id);
                        //insert new
                        //if (lstData != null && lstData.Count() > 0)
                        //{
                        //    for (int i = 0; i < lstData.Count(); i++)
                        //    {
                        //        var entityKHVUChiTiet = new VDT_KHV_KeHoachVonUng_DX_ChiTiet();
                        //        entityKHVUChiTiet.MapFrom(lstData.ToList()[i]);
                        //        if (lstData.ToList()[i].isDelete)
                        //            entityKHVUChiTiet.fGiaTriDeNghi = 0;
                        //        entityKHVUChiTiet.iID_KeHoachUngID = entity.Id;
                        //        conn.Insert(entityKHVUChiTiet, trans);
                        //    }
                        //}
                        id = entity.Id.ToString();
                        isInsert = false;
                        #endregion
                    }
                }
                // commit to db
                trans.Commit();
            }
            return Json(new { status = true, ID = id, isinsert = isInsert, isDieuChinh = isDieuChinh }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDuAnByCondition(string sMaDonVi = null, DateTime? dNgayDeNghi = null, string sTongHop = null)
        {
            if ((string.IsNullOrEmpty(sMaDonVi) || !dNgayDeNghi.HasValue))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            var data = _qlVonDauTuService.GetDuAnInVonUngDeXuatByCondition(sMaDonVi, dNgayDeNghi, sTongHop);
            return Json(new { status = true, lstDuAn = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDuAnByMaDonVi(string sMaDonVi = null)
        {
            if (sMaDonVi == null)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            var data = _qlVonDauTuService.GetDuAnInVonUngDeXuatByIdDonVi(sMaDonVi);
            return Json(new { status = true, lstDuAn = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDuAnByChungTuTongHop(List<Guid> lstChungTuId)
        {
            List<VdtKhcKeHoachVonUngDeXuatChiTietModel> lstDuAn = new List<VdtKhcKeHoachVonUngDeXuatChiTietModel>();
            List<VdtKhvuDXChiTietModel> lstKhvuDX = new List<VdtKhvuDXChiTietModel>();
            foreach (var iId in lstChungTuId)
            {
                VdtKhvuDXChiTietModel objKhvu = _qlVonDauTuService.GetKeHoachVonUngChiTietById(iId, PhienLamViec.NamLamViec);
                if (objKhvu != null)
                {
                    lstKhvuDX.Add(objKhvu);
                    if (objKhvu.listKhvuChiTiet != null && objKhvu.listKhvuChiTiet.Any())
                        lstDuAn.AddRange(objKhvu.listKhvuChiTiet);
                }
            }
            return Json(new { status = true, lstKhvu = lstKhvuDX, lstDuAn = lstDuAn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool KeHoachVonUngDeXuatLock(Guid id)
        {
            return _qlVonDauTuService.LockOrUnLockKeHoachVonUngDeXuat(id);
        }

        public SelectList CreateSelectListDonVi()
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            List<SelectListItem> lstCbx = new List<SelectListItem>();
            foreach (NS_DonVi item in lstDonViQuanLy)
            {
                lstCbx.Add(new SelectListItem()
                {
                    Text = string.Format("{0} - {1}", item.iID_MaDonVi, item.sMoTa),
                    Value = string.Format("{0}", item.iID_MaDonVi),
                });
            }
            return lstCbx.ToSelectList();
        }
        #endregion

        #region kế hoạch vốn ứng chi tiết

        public ActionResult DetailChiTiet(Guid? idKHVU, bool isUpdate, bool isTongHop, bool isDieuChinh = false)
        {

            var data = TempData["ListIdDuAn"];
            var lstChungTuId = (List<Guid>)TempData["ListIdDuAn"];
            TempData.Keep("ListIdDuAn");
            ViewBag.iID_KeHoachUngID = idKHVU;
            ViewBag.isUpdate = 0;
            ViewBag.isTongHop = 0;
            ViewBag.isDieuChinh = isDieuChinh ? "true" : "false";
            ViewBag.isDieuChinhParrent = "false";
            
            

            if (isUpdate)
            {
                ViewBag.isUpdate = 1;

            }
            if (isTongHop)
            {
                ViewBag.isTongHop = 1;

                if (idKHVU != null || idKHVU != Guid.Empty)
                {
                    var dataTongHop = _qlVonDauTuService.GetKeHoachVonUngDeXuatChiTietTongHopById(idKHVU);
                    if (dataTongHop == null)
                    {
                        return View("DetailChiTiet", dataTongHop = new List<VdtKhcKeHoachVonUngDeXuatChiTietModel>());
                    }
                    return View("DetailChiTiet", dataTongHop);
                }
            }

            if (!lstChungTuId.Any())
            {
                var lstDataCt = new List<VdtKhcKeHoachVonUngDeXuatChiTietModel>();
                return View("DetailChiTiet", lstDataCt);
            }

            var lstdata = new HashSet<Guid>(lstChungTuId).ToList();
            var lstDataChiTiet  = new List<VdtKhcKeHoachVonUngDeXuatChiTietModel>();

            if (isDieuChinh)
            {
                var entity = _qlVonDauTuService.GetKHVonUngDXByID(idKHVU);
                if(entity != null && !entity.bIsGoc)
                {
                    ViewBag.isDieuChinhParrent = "true";
                }
                lstDataChiTiet = _qlVonDauTuService.GetKeHoachVonUngDeXuatChiTietByIdAndListIDDuAn(entity.iID_ParentID, lstdata).ToList();
                if (idKHVU != null)
                {
                    foreach (var item in lstDataChiTiet)
                    {
                        lstdata.RemoveAll(x => x == item.iID_DuAnID);
                    }

                    if (lstdata.Any())
                    {
                        var lstDataByLstId = _qlVonDauTuService.GetKeHoachVonUngDeXuatChiTietByIdAndListIDDuAn(null, lstdata).ToList();
                        lstDataChiTiet.AddRange(lstDataByLstId);
                    }
                }
            }
            else
            {
                 lstDataChiTiet = _qlVonDauTuService.GetKeHoachVonUngDeXuatChiTietByIdAndListIDDuAn(idKHVU, lstdata).ToList();
                if (idKHVU != null)
                {
                    foreach (var item in lstDataChiTiet)
                    {
                        lstdata.RemoveAll(x => x == item.iID_DuAnID);
                    }

                    if (lstdata.Any())
                    {
                        var lstDataByLstId = _qlVonDauTuService.GetKeHoachVonUngDeXuatChiTietByIdAndListIDDuAn(null, lstdata).ToList();
                        lstDataChiTiet.AddRange(lstDataByLstId);
                    }
                }
            }


            if (!lstDataChiTiet.Any())
            {
                return View("DetailChiTiet", lstDataChiTiet);
            }

            return View("DetailChiTiet", lstDataChiTiet);
        }

        public JsonResult KeHoachVonUngChiTietSave(List<VdtKhcKeHoachVonUngDeXuatChiTietModel> listdata, bool isUpdate, Guid iID_KeHoachUngID, bool isDieuChinh)
        {
            if (!listdata.Any())
            {
                throw new ArgumentNullException("model");
            }
            else
            {
                if (isDieuChinh)
                {
                    if (!_qlVonDauTuService.SaveKHVNCT(listdata, isUpdate, iID_KeHoachUngID, isDieuChinh))
                    {
                        return Json(new { status = false, desc = "Điều chỉnh kế hoạch vốn ứng chi tiết thất bại!" });
                    }
                }
                else
                {
                    if (isUpdate)
                    {
                        if (!_qlVonDauTuService.SaveKHVNCT(listdata, isUpdate, iID_KeHoachUngID))
                        {
                            return Json(new { status = false, desc = "Cập nhật kế hoạch vốn ứng chi tiết thất bại!" });
                        }
                    }
                    else
                    {
                        if (!_qlVonDauTuService.SaveKHVNCT(listdata, isUpdate, iID_KeHoachUngID))
                        {
                            return Json(new { status = false, desc = "Thêm mới kế hoạch vốn ứng chi tiết thất bại!" });
                        }
                    }

                }

            }
            if (isUpdate)
            {
                return Json(new { status = true, desc = "Cập nhật kế hoạch vốn ứng chi tiết thành công!" });

            }
            return Json(new { status = true, desc = "Thêm mới kế hoạch vốn ứng chi tiết thành công!" });
        }

        [HttpPost]
        public JsonResult GetListDuAnKHVUDC(List<Guid> listData)
        {
            TempData["ListIdDuAn"] = listData;
            // TempData.Keep("ListIdDuAn");
            if (TempData["ListIdDuAn"] != null)
            {
                TempData.Keep("ListIdDuAn");
                return Json(new { status = true, desc = " " });
            }
            return Json(new { status = false, desc = " Không có dự án" });

        }

        [HttpPost]
        public JsonResult GetDataKHVUDXById(Guid? Id)
        {
            var data = _qlVonDauTuService.GetKHVonUngDXByID(Id);
            var dataDetails = new VDTKeHoachVonUngDeXuatModel();

            if (data == null)
            {
                return Json(new { data = dataDetails, status = false });
            }
            string dNgayDN = data.dNgayDeNghi.Value.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            return Json(new { data = data, status = true, dNgayDeNghi = dNgayDN });
        }

        #endregion

        #region Kế hoạch vốn ứng đề xuất tổng hợp

        public ActionResult GetModel(Guid? id, List<Guid> listKHVids, bool isTonghop)
        {
            VdtKhvuDXChiTietModel model = new VdtKhvuDXChiTietModel();
            if (isTonghop)
            {
                if (listKHVids.Any())
                {
                    var listKhvTonghop = _qlVonDauTuService.GetKeHoachVonUngDeXuatTongHopByListKhvIds(listKHVids);
                    ViewBag.LstTongHop = new List<VDTKeHoachVonUngDeXuatModel>();

                    if (listKhvTonghop.Any())
                    {
                        ViewBag.LstTongHop = listKhvTonghop;
                        model.sTenDonVi = listKhvTonghop.First().sTenDonVi;
                        model.iID_DonViQuanLyID = listKhvTonghop.First().iID_DonViQuanLyID;
                        model.iID_MaDonViQuanLy = listKhvTonghop.First().iID_MaDonViQuanLy;
                        model.iNamKeHoach = listKhvTonghop.First().iNamKeHoach;
                        model.sTenNguonVon = listKhvTonghop.First().sTenNguonVon;
                        model.iID_NguonVonID = listKhvTonghop.First().iID_NguonVonID;
                        model.dNgayDeNghi = listKhvTonghop.First().dNgayDeNghi != null ? listKhvTonghop.First().dNgayDeNghi : DateTime.Now;
                    }
                }
            }
            else
            {
                if (id != null || id != Guid.Empty)
                {
                    return PartialView("_modelTongHop", model);
                }
            }

            return PartialView("_modelTongHop", model);
        }
        #endregion

        #region Import,export
        public ActionResult ViewImport()
        {
            ViewBag.ListDonViQuanLy = CreateSelectListDonVi();
            ViewBag.LstNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            VdtKhvuDXChiTietModel data = new VdtKhvuDXChiTietModel();
            ViewBag.Title = "Import kế hoạch vốn ứng đề xuất";
            data.dNgayDeNghi = DateTime.Now;

            return View(data);
        }

        [HttpPost]
        public JsonResult ReadDataToFileExcel()
        {
            try
            {
                var file = Request.Files[0];
                var data = getBytes(file);
                var dt = ExcelHelpers.LoadExcelDataTable(data);
                Dictionary<string, VdtKhcKeHoachVonUngDeXuatChiTietModel> dicData = excel_result(dt);
                if (dicData == null)
                {
                    return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet); ;

                }
                List<VdtKhcKeHoachVonUngDeXuatChiTietModel> dataImport = new List<VdtKhcKeHoachVonUngDeXuatChiTietModel>();
                var listMaDuAns = new List<string>();
                foreach (var item in dicData)
                {
                    dataImport.Add(item.Value);
                    listMaDuAns.Add(item.Key);
                }
                Dictionary<string, VDT_DA_DuAn> listDuAns = _qlVonDauTuService.GetListidDuAnByListMaDuAn(listMaDuAns).ToDictionary(n => n.sMaDuAn, n => n); ;
                if (listDuAns == null || listDuAns.Count != listMaDuAns.Count())
                {
                    return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet); ;
                }
                var dataOut = new List<VdtKhcKeHoachVonUngDeXuatChiTietModel>();
                foreach (var entity in dataImport)
                {
                    if (listDuAns != null && listDuAns.ContainsKey(entity.sMaDuAn))
                    {
                        entity.iID_DuAnID = listDuAns[entity.sMaDuAn].iID_DuAnID;
                        entity.sTrangThaiDuAnDangKy = listDuAns[entity.sMaDuAn].sTrangThaiDuAn;
                        dataOut.Add(entity);
                    }
                }
                TempData["dataImport"] = dataOut;
                TempData.Keep("dataImport");

                return Json(new { bIsComplete = true, fGiaTriUng = dataOut.Sum(x => x.fGiaTriDeNghi) }, JsonRequestBehavior.AllowGet);
                //return Json(file);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);

            }

        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private Dictionary<string, VdtKhcKeHoachVonUngDeXuatChiTietModel> excel_result(DataTable dt)
        {
            var dicData = new Dictionary<string, VdtKhcKeHoachVonUngDeXuatChiTietModel>();

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) != "" || x.Field<string>(0) != null || x.Field<string>(0) != "null");
            for (var i = 5; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];
                Regex rg = new Regex(@"^...-...-....$");
                var sSTT = r.Field<string>(0);
                var sMaDuAn = r.Field<string>(1);
                double fGiaTriDeNghi;
                double.TryParse(r.Field<string>(2), out fGiaTriDeNghi);
                var sGhiChu = r.Field<string>(3);
                if (rg.IsMatch(sMaDuAn))
                {
                    if (sMaDuAn == null || fGiaTriDeNghi == null || fGiaTriDeNghi == 0 || sMaDuAn == "")
                        continue;
                    var dataImp = new VdtKhcKeHoachVonUngDeXuatChiTietModel
                    {
                        sMaDuAn = sMaDuAn,
                        fGiaTriDeNghi = fGiaTriDeNghi,
                        sGhiChu = sGhiChu,


                    };

                    dicData.Add(sMaDuAn, dataImp);
                }
                else
                {
                    continue;
                }
            }
            if (dicData.Count != dicData.Count)
            {
                return null;
            }
            return dicData;
        }

        [HttpPost]
        public JsonResult KhvuSaveImport(VdtKhvuDXChiTietModel data)
        {
            var listDataDetail = (List<VdtKhcKeHoachVonUngDeXuatChiTietModel>)TempData["dataImport"];

            TempData.Keep("dataImport");
            #region save KHUV
            if (listDataDetail != null)
            {
                if (!_qlVonDauTuService.KhvuImportSave(data, listDataDetail, Username))
                {
                    return Json(new { bIsComplete = false, desc = "Lỗi lưu dữ liệu import! " }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(new { bIsComplete = false, desc = "không có dữ liệu import! " }, JsonRequestBehavior.AllowGet);

            }

            #endregion

            return Json(new { bIsComplete = true, desc = "Lưu dữ liệu impport thành công! " }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadImportExample()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachVonUng/import_KHVU.xlsx"));
            string fileName = "import_KHVU.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public JsonResult KeHoachVonUngDeXuatExport(Guid? id)
        {
            try
            {
                List<VdtKhvuDXChiTietModel> listDataQuery = _qlVonDauTuService.GetDataReportKHVUDeXuatById(id).ToList();

                ExcelFile xls = CreateReportExport(listDataQuery, id);
                xls.PrintLandscape = true;

                TempData["DataExport"] = xls;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public ExcelFile CreateReportExport(List<VdtKhvuDXChiTietModel> lstData, Guid? id)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachVonUng/rpt_KeHoachVonUng_DeXuat.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            var dataModel = _qlVonDauTuService.GetKHVonUngDXByID(id);
            VdtKhvuDXChiTietModel objKHVU = new VdtKhvuDXChiTietModel();
            objKHVU.sTenDonVi = dataModel.sTenDonVi.ToUpper();
            objKHVU.sTenNguonVon = dataModel.sTenNguonVon.Substring(dataModel.sTenNguonVon.LastIndexOf(".") + 1).ToUpper();
            objKHVU.iNamKeHoach = dataModel.iNamKeHoach;
            objKHVU.sSoDeNghi = dataModel.sSoDeNghi;
            objKHVU.day = dataModel.dNgayDeNghi.Value.ToString("dd", CultureInfo.InvariantCulture);
            objKHVU.month = dataModel.dNgayDeNghi.Value.ToString("MM", CultureInfo.InvariantCulture);
            objKHVU.year = dataModel.dNgayDeNghi.Value.ToString("yyyy", CultureInfo.InvariantCulture);

            fr.AddTable<VdtKhvuDXChiTietModel>("Items", lstData);

            fr.SetValue("sTenDonVi", objKHVU.sTenDonVi);
            fr.SetValue("sTenNguonVon", objKHVU.sTenNguonVon);
            fr.SetValue("sSoDeNghi", objKHVU.sSoDeNghi);
            fr.SetValue("iNamLamViec", objKHVU.iNamKeHoach);
            fr.SetValue("day", objKHVU.day);
            fr.SetValue("month", objKHVU.month);
            fr.SetValue("year", objKHVU.year);
            fr.SetValue("sMaDuAn", objKHVU.sMaDuAn);
            fr.SetValue("sTenDuAn", objKHVU.sTenDuAn);
            fr.SetValue("fGiaTriDeNghi", String.Format("##,#", objKHVU.fGiaTriDeNghi));
            fr.SetValue("sGhiChu", objKHVU.sGhiChu);
            fr.SetValue("sSTT", objKHVU.sSTT);
            fr.Run(Result);
            return Result;
        }

        public FileContentResult ExportExcelKHVUDeXuat()
        {
            string sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string sFileName = "KeHoachVonUngDeXuat.xlsx";
            ExcelFile xls = (ExcelFile)TempData["DataExport"];
            xls.PrintLandscape = true;
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return File(stream.ToArray(), sContentType, sFileName);
            }
        }

        #endregion
    }
}