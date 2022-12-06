using DapperExtensions;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;
using static Viettel.Extensions.Constants;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class QLKeHoachVonUngDuocDuyetController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly INganSachNewService _nsService = NganSachNewService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        private QLKeHoachVonNamModel _modelKHVN = new QLKeHoachVonNamModel();

        // GET: QLVonDauTu/QLKeHoachVonUngDuocDuyet
        public ActionResult Index()
        {
            ViewBag.ListDonViQuanLy = CreateSelectListDonVi();
            ViewBag.LstNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            VDTKeHoachVonUngPagingModel dataKHVU = new VDTKeHoachVonUngPagingModel();
            dataKHVU._paging.CurrentPage = 1;
            dataKHVU.lstData = _iQLVonDauTuService.GetAllKHVUDuocDuyet(ref dataKHVU._paging, PhienLamViec.NamLamViec);
            return View(dataKHVU);
        }

        [HttpPost]
        public ActionResult TimKiemKHVUDuocDuyet(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom,
            DateTime? dNgayQuyetDinhTo, int? iNamKeHoach, int? iIdNguonVon, string iID_DonViQuanLyID)
        {
            if (string.IsNullOrEmpty(sSoQuyetDinh))
                sSoQuyetDinh = null;
            if (string.IsNullOrEmpty(iID_DonViQuanLyID))
                iID_DonViQuanLyID = null;
            VDTKeHoachVonUngPagingModel dataKHVU = new VDTKeHoachVonUngPagingModel();
            dataKHVU._paging = _paging;
            dataKHVU.lstData = _iQLVonDauTuService.GetAllKHVUDuocDuyet
                (ref dataKHVU._paging, PhienLamViec.NamLamViec, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iIdNguonVon, iID_DonViQuanLyID);
            ViewBag.ListDonViQuanLy = CreateSelectListDonVi();
            ViewBag.LstNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            return PartialView("_partialListKeHoachVonUngDuocDuyet", dataKHVU);
        }

        public ActionResult CreateNew(Guid? id, bool isDieuChinh = false)
        {
            ViewBag.LstNguonVon = CommonFunction.GetDataDropDownNguonNganSach();
            ViewBag.isDieuChinh = isDieuChinh ? "true" : "false";

            VDTQLKeHoachVonUngDuocDuyetModel data = new VDTQLKeHoachVonUngDuocDuyetModel();
            if (id.HasValue)
            {
                VDT_KHV_KeHoachVonUng dataKHVU = new VDT_KHV_KeHoachVonUng();
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    dataKHVU = conn.Get<VDT_KHV_KeHoachVonUng>(id, trans);

                    // commit to db
                    trans.Commit();
                }
                data.dataKHVU = dataKHVU;
            }
            else
            {
                VDT_KHV_KeHoachVonUng dataKHVU = new VDT_KHV_KeHoachVonUng();
                dataKHVU.dNgayQuyetDinh = DateTime.Now;
                data.dataKHVU = dataKHVU;
            }

            ViewBag.dNgayQuyetDinh = data.dataKHVU.dNgayQuyetDinh.HasValue ? data.dataKHVU.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
            return View(data);
        }

        [HttpPost]
        public bool VDTKHVUDelete(Guid id)
        {
            // update bang tong hop
            _iQLVonDauTuService.InsertTongHopNguonDauTu_Tang(LOAI_CHUNG_TU.KE_HOACH_VON_UNG, (int)TypeExecute.Delete, id);

            if (!_iQLVonDauTuService.deleteKHVUChiTiet(id)) return false;
            if (!_iQLVonDauTuService.deleteKHVU(id, Username)) return false;
            return true;
        }

        public ActionResult ViewDetail(Guid id)
        {
            VDTKeHoachVonUngDuocDuyetViewModel dataView = new VDTKeHoachVonUngDuocDuyetViewModel();
            dataView = _iQLVonDauTuService.GetKHVUById(id, PhienLamViec.NamLamViec);
            return View(dataView);
        }

        public JsonResult QLKeHoachVonUngSave(VDTQLKeHoachVonUngDuocDuyetModel data, string id = null, bool isDieuChinh = false)
        {
            if (isDieuChinh)
            {
                if (CheckExistSoQuyetDinh(null, data.dataKHVU.sSoQuyetDinh))
                {
                    return Json(new { status = true, messError = string.Format("Số quyết định {0} đã tồn tại.", data.dataKHVU.sSoQuyetDinh) }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (CheckExistSoQuyetDinh(data.dataKHVU.Id, data.dataKHVU.sSoQuyetDinh))
                {
                    return Json(new { status = true, messError = string.Format("Số quyết định {0} đã tồn tại.", data.dataKHVU.sSoQuyetDinh) }, JsonRequestBehavior.AllowGet);
                }

            }

            ConvertMucLucNganSach(data.listKHVUChiTiet);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                bool isUpdate = false;
                Guid iID = Guid.Empty;
                var config = _iNganSachService.GetCauHinh(Username);
                int iMaNamNS = config.iID_MaNamNganSach;
                var entityParent = conn.Get<VDT_KHV_KeHoachVonUng>(data.dataKHVU.Id, trans);


                #region Dieu chinh ke hoach von ung duoc duyet
                if (isDieuChinh)
                {
                    var entity = new VDT_KHV_KeHoachVonUng();
                    entity.MapFrom(data.dataKHVU);

                    //if (data.listKHVUChiTiet != null && data.listKHVUChiTiet.Count() > 0)
                    //    entityKHVU.fGiaTriUng = data.listKHVUChiTiet.Where(x => x.isDelete == false).Sum(x => x.fCapPhatBangLenhChi) + data.listKHVUChiTiet.Where(x => x.isDelete == false).Sum(x => x.fCapPhatTaiKhoBac);
                    entity.Id = Guid.NewGuid();
                    entity.bActive = true;
                    entity.bIsGoc = false;
                    entity.iID_ParentID = entityParent.Id;
                    entity.sUserCreate = Username;
                    entity.dDateCreate = DateTime.Now;
                    conn.Insert(entity, trans);

                    entityParent.bActive = false;
                    entityParent.dDateUpdate = DateTime.Now;
                    entityParent.sUserUpdate = Username;
                    conn.Update<VDT_KHV_KeHoachVonUng>(entityParent, trans);

                    iID = entity.Id;
                    id = entity.Id.ToString();
                }
                #endregion
                else
                {
                    if (data.dataKHVU.Id == Guid.Empty)
                    {
                        #region Them moi VDT_KHV_KeHoachVonUng

                        var entity = new VDT_KHV_KeHoachVonUng();
                        entity.MapFrom(data.dataKHVU);

                        //if (data.listKHVUChiTiet != null && data.listKHVUChiTiet.Count() > 0)
                        //    entityKHVU.fGiaTriUng = data.listKHVUChiTiet.Where(x => x.isDelete == false).Sum(x => x.fCapPhatBangLenhChi) + data.listKHVUChiTiet.Where(x => x.isDelete == false).Sum(x => x.fCapPhatTaiKhoBac);
                        entity.bIsGoc = true;
                        entity.bActive = true;
                        entity.sUserCreate = Username;
                        entity.dDateCreate = DateTime.Now;
                        conn.Insert(entity, trans);

                        iID = entity.Id;
                        id = entity.Id.ToString();
                        #endregion

                        #region Them moi VDT_KHV_KeHoachVonUng_ChiTiet
                        //if (data.listKHVUChiTiet != null && data.listKHVUChiTiet.Count() > 0)
                        //{
                        //    for (int i = 0; i < data.listKHVUChiTiet.Count(); i++)
                        //    {
                        //        var entityKHVUChiTiet = new VDT_KHV_KeHoachVonUng_ChiTiet();
                        //        entityKHVUChiTiet.MapFrom(data.listKHVUChiTiet.ToList()[i]);
                        //        if (data.listKHVUChiTiet.ToList()[i].isDelete)
                        //        {
                        //            entityKHVUChiTiet.fCapPhatTaiKhoBac = 0;
                        //            entityKHVUChiTiet.fCapPhatBangLenhChi = 0;
                        //        }

                        //        entityKHVUChiTiet.iID_KeHoachUngID = entityKHVU.Id;
                        //        conn.Insert(entityKHVUChiTiet, trans);
                        //    }
                        //}
                        #endregion
                    }
                    else
                    {
                        #region sửa kế hoạch vốn ứng được duyệt
                        isUpdate = true;
                        //var entity = conn.Get<VDT_KHV_KeHoachVonUng>(data.dataKHVU.Id, trans);
                        entityParent.sSoQuyetDinh = data.dataKHVU.sSoQuyetDinh;
                        entityParent.iID_NhomQuanLyID = data.dataKHVU.iID_NhomQuanLyID;
                        if (data.listKHVUChiTiet != null && data.listKHVUChiTiet.Count() > 0)
                            entityParent.fGiaTriUng = data.listKHVUChiTiet.Where(x => x.isDelete == false).Sum(x => x.fCapPhatBangLenhChi) + data.listKHVUChiTiet.Where(x => x.isDelete == false).Sum(x => x.fCapPhatTaiKhoBac);
                        entityParent.sUserUpdate = Username;
                        entityParent.dDateUpdate = DateTime.Now;
                        conn.Update<VDT_KHV_KeHoachVonUng>(entityParent, trans);

                        iID = entityParent.Id;
                        id = iID.ToString();
                        #endregion

                        #region Them moi VDT_KHV_KeHoachVonUng_ChiTiet
                        //delete all KHVUChiTiet
                        //_iQLVonDauTuService.deleteKHVUChiTiet(data.dataKHVU.Id);
                        ////insert new
                        //if (data.listKHVUChiTiet != null && data.listKHVUChiTiet.Count() > 0)
                        //{
                        //    for (int i = 0; i < data.listKHVUChiTiet.Count(); i++)
                        //    {
                        //        var entityKHVUChiTiet = new VDT_KHV_KeHoachVonUng_ChiTiet();
                        //        entityKHVUChiTiet.MapFrom(data.listKHVUChiTiet.ToList()[i]);
                        //        if (data.listKHVUChiTiet.ToList()[i].isDelete)
                        //        {
                        //            entityKHVUChiTiet.fCapPhatTaiKhoBac = 0;
                        //            entityKHVUChiTiet.fCapPhatBangLenhChi = 0;
                        //        }
                        //        entityKHVUChiTiet.iID_KeHoachUngID = data.dataKHVU.Id;
                        //        conn.Insert(entityKHVUChiTiet, trans);
                        //    }
                        //}
                        #endregion
                    }
                }

                // commit to db
                trans.Commit();


                // luu bang tong hop
                if (isUpdate)
                {
                    _iQLVonDauTuService.InsertTongHopNguonDauTu_Tang(LOAI_CHUNG_TU.KE_HOACH_VON_UNG, (int)TypeExecute.Update, iID);
                }
                else
                {
                    _iQLVonDauTuService.InsertTongHopNguonDauTu_Tang(LOAI_CHUNG_TU.KE_HOACH_VON_UNG, (int)TypeExecute.Insert, iID);
                }
            }
            return Json(new { status = true, ID = id }, JsonRequestBehavior.AllowGet);
        }

        private bool ConvertMucLucNganSach(IEnumerable<VdtKhvKeHoachVonUngChiTietModel> lstData)
        {
            if (lstData == null) return false;
            var lstMlns = _iQLVonDauTuService.GetAllMucLucNganSachByNamLamViec(PhienLamViec.NamLamViec);
            if (lstMlns == null) return false;
            Dictionary<string, Guid> dicMlns = new Dictionary<string, Guid>();
            foreach (var item in lstMlns)
            {
                if (!dicMlns.ContainsKey(item.sXauNoiMa))
                    dicMlns.Add(item.sXauNoiMa, item.iID_MaMucLucNganSach);
            }

            foreach (var item in lstData)
            {
                if (!string.IsNullOrEmpty(item.sXauNoiMa))
                {
                    if (!dicMlns.ContainsKey(item.sXauNoiMa)) continue;
                    item.iID_NganhID = dicMlns[item.sXauNoiMa];
                }
            }
            return true;
        }

        #region Event
        [HttpGet]
        public JsonResult GetDonViQuanLy()
        {
            //var lstData = _iQLVonDauTuService.GetListDonViByNamLamViec(PhienLamViec.NamLamViec);
            List<NS_DonVi> lstData = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            return Json(new { status = true, datas = lstData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetKeHoachVonUngDeXuat(string sMaDonVi, int? iNamKeHoach, DateTime? dNgayDenghi)
        {
            if (string.IsNullOrEmpty(sMaDonVi) || !iNamKeHoach.HasValue || !dNgayDenghi.HasValue)
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            List<VDT_KHV_KeHoachVonUng_DX> lstdata = _iQLVonDauTuService.GetKeHoachVonUngDeXuatInVonUngDuocDuyetScreen(sMaDonVi, iNamKeHoach.Value, dNgayDenghi.Value).ToList();
            List<VDT_KHV_KeHoachVonUng_DX> lstChungTuTongHop = lstdata.Where(x => !string.IsNullOrEmpty(x.sTongHop)).ToList();
            Dictionary<string, List<string>> dctCT = lstChungTuTongHop.GroupBy(x => x.Id).ToDictionary(x => x.Key.ToString(), x => x.Select(y => y.sTongHop).ToList());

            List<VDT_KHV_KeHoachVonUng_DX> lstAllChungTuTongHop = new List<VDT_KHV_KeHoachVonUng_DX>();
            foreach (var key in dctCT.Keys)
            {
                string lstValue = dctCT[key].ToList().FirstOrDefault();
                List<string> lstChildId = new List<string>();
                if (lstValue.Contains(","))
                {
                    lstChildId = lstValue.Split(',').ToList();
                }
                else
                {
                    lstChildId.Add(lstValue);
                }

                VDT_KHV_KeHoachVonUng_DX itemParent = lstdata.Where(x => x.Id == Guid.Parse(key)).FirstOrDefault();
                List<VDT_KHV_KeHoachVonUng_DX> lstChildrent = lstdata.Where(x => lstChildId.Any(y => Guid.Parse(y) == x.Id)).ToList();
                // lstChungTuTongHopParent = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                lstAllChungTuTongHop.Add(itemParent);
                lstAllChungTuTongHop.AddRange(lstChildrent);
            }
            if (lstdata == null || !lstdata.Any())

                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            return Json(new { status = true, datas = lstAllChungTuTongHop }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDuAnByKeHoachVonUngDeXuat(Guid id)
        {
            var data = _iQLVonDauTuService.GetKeHoachVonUngDeXuatDetailById(id);
            if (data == null || !data.Any())
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            return Json(new { status = true, datas = data }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private bool CheckExistSoQuyetDinh(Guid? iID_KeHoachUngID, string sSoQuyetDinh)
        {
            var isExist = _iQLVonDauTuService.CheckExistSoQuyetDinh(iID_KeHoachUngID, sSoQuyetDinh);
            return isExist;
        }

        [HttpGet]
        public JsonResult CheckExistMLNS(string sXauNoiMa)
        {
            bool checkExist = _iQLVonDauTuService.CheckExistXauNoiMa(PhienLamViec.NamLamViec, sXauNoiMa);
            return Json(new { status = checkExist ? 1 : 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckExistMLNSAndGetIId(string sXauNoiMa)
        {
            Guid iIdNganhID = _iQLVonDauTuService.CheckExistXauNoiMaAndGetIdNganh(PhienLamViec.NamLamViec, sXauNoiMa);
            if (iIdNganhID == Guid.Empty || iIdNganhID == null)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, iID_NganhID = iIdNganhID }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetKeHoachVonUngDetail(Guid id)
        {
            var lstDetail = _iQLVonDauTuService.GetKHVUChiTietList(id);
            if (lstDetail == null) return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            return Json(new { status = true, datas = lstDetail }, JsonRequestBehavior.AllowGet);
        }

        public SelectList CreateSelectListDonVi()
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            List<SelectListItem> lstCbx = new List<SelectListItem>();
            foreach (NS_DonVi item in lstDonViQuanLy)
            {
                lstCbx.Add(new SelectListItem()
                {
                    Text = string.Format("{0} - {1}", item.iID_MaDonVi, item.sTen),
                    Value = string.Format("{0}|{1}", item.iID_Ma, item.iID_MaDonVi),
                });
            }
            return lstCbx.ToSelectList();
        }


        #region Kế hoạch vốn ứng được duyệt chi tiết


        [HttpPost]
        public JsonResult GetListDuAnKHVUDX(List<Guid> listData, Guid? iId_KHVUDX_ID)
        {
            TempData["ListIdDuAn"] = listData;
            TempData["iId_KHVUDX_ID"] = iId_KHVUDX_ID;
            // TempData.Keep("ListIdDuAn");
            if (TempData["ListIdDuAn"] != null)
            {
                TempData.Keep("ListIdDuAn");
                TempData.Keep("iId_KHVUDX_ID");
                return Json(new { status = true, desc = " " });
            }
            return Json(new { status = false, desc = " Không có dự án" });

        }

        public ActionResult Detail(Guid iID_KeHoachUngID, bool isUpdate, bool isDieuChinh = false)
        {
            //var data = TempData["ListIdDuAn"];
            var data = new VDTQLKeHoachVonUngDuocDuyetViewModel();
            var lstChungTuIds = (List<Guid>)TempData["ListIdDuAn"];
            var iId_KHVUDX_ID = (Guid)TempData["iId_KHVUDX_ID"];
            TempData.Keep("ListIdDuAn");
            TempData.Keep("iId_KHVUDX_ID");
            ViewBag.iID_KeHoachUngID = iID_KeHoachUngID;
            ViewBag.isUpdate = 0;
            ViewBag.isDieuChinh = isDieuChinh ? "true" : "false";
            ViewBag.isDieuChinhParrent = "false";

            if (isUpdate)
            {
                ViewBag.isUpdate = 1;
            }

            if (!lstChungTuIds.Any())
            {
                //var data = new VDTQLKeHoachVonUngDuocDuyetModel();
                return View("Detail", data);
            }
            var listDuAnIds = new HashSet<Guid>(lstChungTuIds).ToList();
            var listDataChiTiet = new List<VdtKhvKeHoachVonUngChiTietModel>();

            if (isDieuChinh)
            {
                var entity = _iQLVonDauTuService.GetKHVUDuocDuyetById(iID_KeHoachUngID);
                if (entity != null && !entity.bIsGoc)
                {
                    ViewBag.isDieuChinhParrent = "true";
                }
                listDataChiTiet = _iQLVonDauTuService.GetKHVUDuocDuyetChiTietByIdAndListDuAnIds(entity.iID_ParentID, listDuAnIds, iId_KHVUDX_ID).ToList();
                if (listDataChiTiet.Any())
                {
                    //dataUpdate neu chua tich het du an
                    foreach (var item in listDataChiTiet)
                    {
                        listDuAnIds.RemoveAll(x => x == item.iID_DuAnID);
                    }

                    if (listDuAnIds.Any())
                    {
                        var dataUpdate = _iQLVonDauTuService.GetKHVUDuocDuyetChiTietByIdAndListDuAnIds(null, listDuAnIds, iId_KHVUDX_ID).ToList();
                        listDataChiTiet.AddRange(dataUpdate);
                    }

                    if (listDataChiTiet.Any())
                    {
                        data.listKHVUChiTiet = listDataChiTiet;
                        data.dataKHVU = _iQLVonDauTuService.GetKHVUDuocDuyetById(iID_KeHoachUngID);
                    }
                }
            }
            else
            {
                //dataChiTiet
                listDataChiTiet = _iQLVonDauTuService.GetKHVUDuocDuyetChiTietByIdAndListDuAnIds(iID_KeHoachUngID, listDuAnIds, iId_KHVUDX_ID).ToList();
                if (listDataChiTiet.Any())
                {
                    //dataUpdate neu chua tich het du an
                    foreach (var item in listDataChiTiet)
                    {
                        listDuAnIds.RemoveAll(x => x == item.iID_DuAnID);
                    }

                    if (listDuAnIds.Any())
                    {
                        var dataUpdate = _iQLVonDauTuService.GetKHVUDuocDuyetChiTietByIdAndListDuAnIds(null, listDuAnIds, iId_KHVUDX_ID).ToList();
                        listDataChiTiet.AddRange(dataUpdate);
                    }

                    if (listDataChiTiet.Any())
                    {
                        data.listKHVUChiTiet = listDataChiTiet;
                        data.dataKHVU = _iQLVonDauTuService.GetKHVUDuocDuyetById(iID_KeHoachUngID);
                    }
                }
                else
                {
                    return View("Detail", data);
                }
            }

            return View("Detail", data);
        }

        [HttpPost]
        public ActionResult KeHoachVonUngDuocDuyetChiTietSave(List<VdtKhvKeHoachVonUngChiTietModel> listData, bool isUpdate, Guid? iId_KeHoachUngID, bool isDieuChinh = false)
        {
            if (!listData.Any())
            {
                throw new ArgumentNullException("model");
            }
            else
            {
                if (isDieuChinh)
                {
                    if (!_iQLVonDauTuService.KehoachVonUngDuocDuyetCtSave(listData, isUpdate, iId_KeHoachUngID, isDieuChinh))
                    {
                        return Json(new { status = false, desc = "Điều chỉnh kế hoạch vốn ứng được duyệt chi tiết thất bại!" });
                    }
                }
                else
                {
                    if (isUpdate)
                    {
                        if (!_iQLVonDauTuService.KehoachVonUngDuocDuyetCtSave(listData, isUpdate, iId_KeHoachUngID))
                        {
                            return Json(new { status = false, desc = "Cập nhật kế hoạch vốn ứng được duyệt chi tiết thất bại!" });
                        }
                    }
                    else
                    {
                        if (!_iQLVonDauTuService.KehoachVonUngDuocDuyetCtSave(listData, isUpdate, iId_KeHoachUngID))
                        {
                            return Json(new { status = false, desc = "Thêm mới kế hoạch vốn ứng được duyệt chi tiết thất bại!" });
                        }
                    }

                }

            }

            if (isDieuChinh)
            {
                return Json(new { status = true, desc = "Điều chỉnh kế hoạch vốn ứng được duyệt chi tiết thành công!" });

            }
            else
            {
                if (isUpdate)
                {
                    return Json(new { status = true, desc = "Cập nhật kế hoạch vốn ứng được duyệt chi tiết thành công!" });

                }
                return Json(new { status = true, desc = "Thêm mới kế hoạch vốn ứng được duyệt chi tiết thành công!" });
            }

        }

        #endregion

        #region export kế hoạch vốn ứng được duyệt

        [HttpPost]
        public JsonResult KeHoachVonUngDuocDuyetExport(Guid? id)
        {
            try
            {
                List<VdtKhvKeHoachVonUngChiTietModel> listDataQuery = _iQLVonDauTuService.KehoachVonUngDuocDuyetChiTietExport(id).ToList();

                ExcelFile xls = CreateReportExport(listDataQuery, id);
                xls.PrintLandscape = true;
                TempData["DataExport"] = xls;

                return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
        }

        public ExcelFile CreateReportExport(List<VdtKhvKeHoachVonUngChiTietModel> lstData, Guid? id)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachVonUng/rpt_KeHoachVonUng_DuocDuyet.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            var dataModel = _iQLVonDauTuService.GetKHVUDuocDuyetById(id);
            VDTKhvkeHoachVonUngViewModel objKHVU = new VDTKhvkeHoachVonUngViewModel();
            objKHVU.sTenDonViQuanLy = dataModel.sTenDonViQuanLy.ToUpper();
            objKHVU.sTenNguonVon = dataModel.sTenNguonVon.Substring(dataModel.sTenNguonVon.LastIndexOf(".") + 1).ToUpper();
            objKHVU.iNamKeHoach = dataModel.iNamKeHoach;
            objKHVU.sSoQuyetDinh = dataModel.sSoQuyetDinh;

            fr.AddTable<VdtKhvKeHoachVonUngChiTietModel>("Items", lstData);

            fr.SetValue("sTenDonVi", objKHVU.sTenDonViQuanLy);
            fr.SetValue("sTenNguonVon", objKHVU.sTenNguonVon);
            fr.SetValue("sSoQuyetDinh", objKHVU.sSoQuyetDinh);
            fr.SetValue("iNamLamViec", objKHVU.iNamKeHoach);
            fr.SetValue("day", dataModel.dNgayQuyetDinh.Value.ToString("dd", CultureInfo.InvariantCulture));
            fr.SetValue("month", dataModel.dNgayQuyetDinh.Value.ToString("MM", CultureInfo.InvariantCulture));
            fr.SetValue("year", dataModel.dNgayQuyetDinh.Value.ToString("yyyy", CultureInfo.InvariantCulture));
            fr.Run(Result);
            return Result;
        }

        public FileContentResult ExportExcelKHVUDuocDuyet()
        {
            string sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string sFileName = "KeHoachVonUngDuocDuyet.xlsx";
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