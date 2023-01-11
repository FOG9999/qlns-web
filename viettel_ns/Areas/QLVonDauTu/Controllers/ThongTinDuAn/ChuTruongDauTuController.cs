using AutoMapper.Extensions;
using DapperExtensions;
using DocumentFormat.OpenXml.Spreadsheet;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
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
    public class ChuTruongDauTuController : AppController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;

        #region view
        // GET: QLVonDauTu/ChuTruongDauTu
        public ActionResult Index()
        {
            VDTPheDuyetChuTruongDauTuViewModel vm = new VDTPheDuyetChuTruongDauTuViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _iQLVonDauTuService.LayDanhSachChuTruongDauTu(ref vm._paging, PhienLamViec.NamLamViec, Username);

            //List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            //lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.TAT_CA });
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_MaDonVi", "sMoTa");
            TempData["DataSearch"] = vm.Items;

            // Xu ly ho tro xuat excel
            PagingInfo pageInfo = new PagingInfo() { ItemsPerPage = vm._paging.TotalItems };
            pageInfo.CurrentPage = 1;
            var dataExport = _iQLVonDauTuService.LayDanhSachChuTruongDauTu(ref pageInfo, PhienLamViec.NamLamViec, Username);
            TempData["DataExcel"] = dataExport;
            // End Xu ly ho tro xuat excel

            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoQuyetDinh, string sNoiDung, float? fTongMucDauTuFrom,
            float? fTongMucDauTuTo, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, string sMaDonVi)
        {
            VDTPheDuyetChuTruongDauTuViewModel vm = new VDTPheDuyetChuTruongDauTuViewModel();
            vm._paging = _paging;
            vm.Items = _iQLVonDauTuService.LayDanhSachChuTruongDauTu(ref vm._paging, PhienLamViec.NamLamViec, Username, sSoQuyetDinh, sNoiDung, fTongMucDauTuFrom, fTongMucDauTuTo,
                dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi);
            TempData.Keep("DataSearch");
            TempData["DataSearch"] = vm.Items;
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_MaDonVi", "sMoTa");

            // Xu ly ho tro xuat excel
            PagingInfo pageInfo = new PagingInfo() { ItemsPerPage = vm._paging.TotalItems };
            pageInfo.CurrentPage = 1;
            var dataExport = _iQLVonDauTuService.LayDanhSachChuTruongDauTu(ref pageInfo, PhienLamViec.NamLamViec, Username, sSoQuyetDinh, sNoiDung, fTongMucDauTuFrom, fTongMucDauTuTo,
                dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi);
            TempData["DataExcel"] = dataExport;
            // End Xu ly ho tro xuat excel

            return PartialView("_list", vm);
        }

        public JsonResult GetDataComboBoxDuAn(string maDonVi)
        {
            var result = new List<dynamic>();
            var listDuAn = _iQLVonDauTuService.LayDuAnLapKeHoachTrungHanDuocDuyetTheoDonVi(maDonVi).ToList();
            if (listDuAn != null && listDuAn.Any())
            {
                result.Insert(0, new { id = "", text = "--Chọn--" });
                foreach (var item in listDuAn)
                {
                    result.Add(new { id = item.iID_DuAnID, text = item.sMaDuAn + "-" + item.sTenDuAn });
                }
            }

            return Json(new { status = true, data = result });
        }
        public JsonResult LayDuAnTheoDonViQL(string iID_DonViQuanLyID)
        {
            //List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.LayDanhSachDuAnTheoDonViQuanLy(Guid.Parse(iID_DonViQuanLyID)).ToList();
            List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.ListDuAnTheoDonViQuanLy(Guid.Parse(iID_DonViQuanLyID)).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' data-sMaDuAn='{1}'>{2}</option>", lstDuAn[i].iID_DuAnID, lstDuAn[i].sMaDuAn, lstDuAn[i].sMaDuAn + " - " + lstDuAn[i].sTenDuAn);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TaoMoi()
        {
            VDTPheDuyetChuTruongDauTuModel model = new VDTPheDuyetChuTruongDauTuModel();
            model.chuTruongDauTu = new VDT_DA_ChuTruongDauTu();

            #region data selectlist
            ViewBag.sNgayPheDuyet = DateTime.Now.ToString("dd/MM/yyyy");
            List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayDanhMucChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = string.IsNullOrEmpty(c.sId_CDT) ? c.sTenCDT : (c.sId_CDT + " - " + c.sTenCDT)
            });
            List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnLapKeHoachTrungHanDuocDuyet().ToList();
            lstDuAn.Insert(0, new VDT_DA_DuAn { iID_DuAnID = Guid.Empty, sMaDuAn = Constants.CHON });
            ViewBag.ListDuAn = lstDuAn.Select(c => new SelectListItem
            {
                Value = c.iID_DuAnID.ToString(),
                Text = string.IsNullOrEmpty(c.sTenDuAn) ? c.sMaDuAn : (c.sMaDuAn + " - " + c.sTenDuAn)
            });
            //ViewBag.ListChuDauTu = new SelectList(GetCbxChuDauTu(), "Value", "Text");
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_MaDonVi", "sMoTa");
            //List<NS_DonVi> lstDonViQuanLy = _iQLVonDauTuService.GetListAllDonViQLByUserAndCurrentYear(Username, PhienLamViec.NamLamViec).ToList();
            //lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            //ViewBag.ListDonViQuanLy = lstDonViQuanLy.Select(c => new SelectListItem
            //{
            //    Value = c.iID_MaDonVi,
            //    Text = string.IsNullOrEmpty(c.iID_MaDonVi) ? c.sTen : (c.sMoTa)
            //});
            //Lay danh sach don vi quan ly theo user login
            List<VDT_DM_NhomDuAn> lstNhomDuAn = _iQLVonDauTuService.LayNhomDuAn().ToList();
            lstNhomDuAn.Insert(0, new VDT_DM_NhomDuAn { iID_NhomDuAnID = Guid.Empty, sTenNhomDuAn = Constants.CHON });
            ViewBag.ListNhomDuAn = lstNhomDuAn.ToSelectList("iID_NhomDuAnID", "sTenNhomDuAn", lstNhomDuAn[2].iID_NhomDuAnID.ToString());

            List<VDT_DM_HinhThucQuanLy> lstHinhThucQuanLy = _iQLVonDauTuService.LayHinhThucQuanLy().ToList();
            lstHinhThucQuanLy.Insert(0, new VDT_DM_HinhThucQuanLy { iID_HinhThucQuanLyID = Guid.Empty, sTenHinhThucQuanLy = Constants.CHON });
            ViewBag.ListHinhThucQLDA = lstHinhThucQuanLy.ToSelectList("iID_HinhThucQuanLyID", "sTenHinhThucQuanLy");

            List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _iQLVonDauTuService.LayPhanCapDuAn().Where(x => x.sTen != "Cục duyệt").ToList();
            lstPhanCapDuAn.Insert(0, new VDT_DM_PhanCapDuAn { iID_PhanCapID = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

            List<VDT_DM_ChiPhi> lstChiPhi = _iQLVonDauTuService.LayChiPhi().ToList();
            lstChiPhi.Insert(0, new VDT_DM_ChiPhi { iID_ChiPhi = Guid.Empty, sTenChiPhi = Constants.CHON });
            ViewBag.ListChiPhi = lstChiPhi.ToSelectList("iID_ChiPhi", "sTenChiPhi");

            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            //var lstHangMuc = _iQLVonDauTuService.GetDataDMDuAnHangMuc();
            //lstHangMuc.Insert(0, new VDT_DA_DuAn_HangMuc { iID_DuAn_HangMucID = Guid.Empty, sTenHangMuc = Constants.CHON });
            //ViewBag.ListHangMuc = lstHangMuc.ToSelectList("iID_DuAn_HangMucID", "sTenHangMuc");
            #endregion
            return View(model);
        }
        //private List<SelectListItem> GetCbxChuDauTu()
        //{
        //    var lstChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec);
        //    if (lstChuDauTu == null) return new List<SelectListItem>();
        //    return lstChuDauTu.Select(n => new SelectListItem() { Value = n.sId_CDT, Text = n.sTenCDT }).ToList();
        //}

        [HttpGet]
        public JsonResult GetCbxChuTruongDauTu()
        {
            List<DM_ChuDauTu> lstCDT = _iQLVonDauTuService.LayDanhMucChuDauTu(PhienLamViec.NamLamViec).ToList();
            string sCbxData = string.Join(",", lstCDT.Select(n => string.Format("<option value='{0}' data-sID_CDT='{1}'> {0} - {2}</option>", n.sId_CDT, n.ID.ToString(), n.sTenCDT)));
            return Json(new { data = sCbxData }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FindDuAn(Guid iID_MaDonViQuanLyID)
        {

            List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnByIdMaDonViQuanLY(iID_MaDonViQuanLyID).ToList();
            string sCbxData = string.Join(",", lstDuAn.Select(n => string.Format("<option value='{0}' data-iID_DuAnID='{1}'> {0} - {2}</option>", n.sMaDuAn, n.iID_DuAnID, n.sTenDuAn)));
            return Json(new { dataDuAn = sCbxData }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetListNguonVOnVaHangMucByCTDauTuDieuChinh(Guid CTDauTuId, bool isDieuChinh = false)
        {
            List<VDTChuTruongDauTuNguonVonModel> listNguonVon = new List<VDTChuTruongDauTuNguonVonModel>();
            List<VDTDADuAnHangMucModel> listHangMuc = new List<VDTDADuAnHangMucModel>();
            try
            {
                listNguonVon = _iQLVonDauTuService.GetListNguonVonTheoCTDauTuId(CTDauTuId).ToList();
                listHangMuc = _iQLVonDauTuService.GetListHangMucTheoCTDauTuId(CTDauTuId).ToList();
                if (isDieuChinh)
                {
                    // reset iID_ChuTruongDauTu_NguonVonID
                    Dictionary<Guid, Guid> listNewiID_ChuTruongDauTu_NguonVonID = new Dictionary<Guid, Guid>();
                    foreach (VDTChuTruongDauTuNguonVonModel item in listNguonVon)
                    {
                        Guid newId = Guid.NewGuid();
                        listNewiID_ChuTruongDauTu_NguonVonID.Add(item.iID_ChuTruongDauTu_NguonVonID, newId);
                        item.iID_ChuTruongDauTu_NguonVonID = newId;
                    }

                    // reset iID_CTDauTu_DM_HangMuc
                    Dictionary<Guid, Guid> listNewIdCTDauTuDMHangMuc = new Dictionary<Guid, Guid>();
                    foreach (VDTDADuAnHangMucModel item in listHangMuc)
                    {
                        Guid newId = Guid.NewGuid();
                        listNewIdCTDauTuDMHangMuc.Add(item.iID_ChuTruongDauTu_HangMucID, newId);
                        item.iID_ChuTruongDauTu_HangMucID = newId;
                        listNewIdCTDauTuDMHangMuc.Add(item.iID_DuAn_HangMucID, newId);
                        item.iID_DuAn_HangMucID = newId;
                    }
                    List<VDTDADuAnHangMucModel> listHangMucChild = listHangMuc.Where(x => x.iID_ParentID != null).ToList();
                    foreach (VDTDADuAnHangMucModel item in listHangMucChild)
                    {
                        item.iID_ParentID = listNewIdCTDauTuDMHangMuc[item.iID_ParentID.Value];
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true, dataNguonVon = listNguonVon, dataHangMuc = listHangMuc });
        }


        [HttpPost]
        public JsonResult Save(VDT_DA_ChuTruongDauTuCreateModel model)
        {
            var sMessage = string.Format(Constants.THEM_BAN_GHI, model.sSoQuyetDinh);
            if (model.iID_ChuTruongDauTuID == null || model.iID_ChuTruongDauTuID == Guid.Empty)
            {
                if (model.iID_ParentID != null) sMessage = sMessage.Replace("Thêm mới", "Điều chỉnh");
            }
            else
            {
                sMessage = sMessage.Replace("Thêm mới", "Cập nhật");
            }
            if (model.ListNguonVon == null)
                model.ListNguonVon = new List<VDTChuTruongDauTuNguonVonModel>();
            if (model.ListHangMuc == null)
                model.ListHangMuc = new List<VDTDADuAnHangMucModel>();
            if (!string.IsNullOrEmpty(model.iID_MaDonViQuanLy))
            {
                NS_DonVi donVi = _iNganSachService.GetDonViByMaDonVi(PhienLamViec.NamLamViec.ToString(), model.iID_MaDonViQuanLy);
                model.iID_DonViQuanLyID = donVi.iID_Ma;
            }

            foreach (var item in model.ListNguonVon)
            {
                if (item.iID_NguonVonID == 0 && !item.isDelete)
                    return Json(new { status = false, message = "Chưa nhập tên nguồn vốn !" });
            }

            var anyDuplicate = model.ListNguonVon.Where(o => !o.isDelete).GroupBy(x => x.iID_NguonVonID).Any(g => g.Count() > 1);
            if (anyDuplicate)
            {
                return Json(new { status = false, message = "Nguồn vốn đã tồn tại. Vui lòng chọn lại!" });
            }

            var checkSoQuyetDinh = _iQLVonDauTuService.KiemTraTrungSoQuyetDinhChuTruongDauTu(model.sSoQuyetDinh, model.iID_ChuTruongDauTuID.ToString());
            if (!checkSoQuyetDinh)
            {
                return Json(new { status = false, message = "Số quyết định đã tồn tại !" });
            }

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();

                var trans = conn.BeginTransaction();
                _iQLVonDauTuService.CheckDuAnQuyetDinhDauTu(model, Username);
                var entity = new VDT_DA_ChuTruongDauTu();
                //truong hợp thêm mới
                if (model.iID_ChuTruongDauTuID == null || model.iID_ChuTruongDauTuID == Guid.Empty)
                {
                    entity.MapFrom(model);
                    entity.bActive = true;
                    if (model.isDieuChinh)
                    {
                        entity.bIsGoc = false;
                    }
                    else
                    {
                        entity.bIsGoc = true;
                    }
                    entity.iID_MaChuDauTuID = model.iID_MaChuDauTuID;
                    entity.iID_MaDonViQuanLy = model.iID_MaDonViQuanLy;
                    entity.sUserCreate = Username;
                    entity.sUserUpdate = Username;
                    entity.dDateCreate = DateTime.Now;
                    entity.dDateUpdate = DateTime.Now;
                    entity.iID_ChuTruongDauTuID = Guid.NewGuid();
                    conn.Insert<VDT_DA_ChuTruongDauTu>(entity, trans);
                    // nếu là điều chỉnh thì update chủ trương đầu tư gốc bactive = 0;
                    if (model.iID_ParentID != null)
                    {
                        VDT_DA_ChuTruongDauTu entityParent = conn.Get<VDT_DA_ChuTruongDauTu>(model.iID_ParentID, trans);
                        if (entityParent != null)
                        {
                            entityParent.bActive = false;
                            conn.Update(entityParent, trans);
                        }
                    }

                    model.iID_ChuTruongDauTuID = entity.iID_ChuTruongDauTuID;
                }
                else // trường hợp sửa
                {
                    entity = conn.Get<VDT_DA_ChuTruongDauTu>(model.iID_ChuTruongDauTuID, trans);
                    if (entity == null)
                        return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                    string thuocTinhKhongMap = "iID_ChuTruongDauTuID,iID_DuAnID,iID_TienTeID,iID_DonViTienTeID,fTiGiaDonVi,fTiGia,bActive,sTenDuAn,iID_DonViThucHienID,iID_LoaiDuAn,iID_HinhThucDauTuID,iID_DonViQuanLyID,iID_NhomQuanLyID,sUserCreate,dDateCreate,bIsGoc,sMoTa";
                    entity.MapFrom(model, thuocTinhKhongMap);
                    entity.iID_MaChuDauTuID = model.iID_MaChuDauTuID;
                    entity.iID_MaDonViQuanLy = model.iID_MaDonViQuanLy;
                    entity.sUserUpdate = Username;
                    entity.dDateUpdate = DateTime.Now;
                    conn.Update(entity, trans);
                }

                VDT_DA_DuAn objDuAn = conn.Get<VDT_DA_DuAn>(model.iID_DuAnID, trans);
                if (objDuAn != null && objDuAn.iID_DuAnID != Guid.Empty)
                {
                    objDuAn.iID_MaDonVi = model.iID_MaDonViQuanLy;
                    objDuAn.iID_MaCDT = model.iID_MaChuDauTuID;
                    var lstMaDuAn = objDuAn.sMaDuAn.Split('-');
                    if (lstMaDuAn != null && lstMaDuAn.Length >= 3)
                    {
                        objDuAn.sMaDuAn = string.Format("{0}-{1}-{2}", lstMaDuAn[0], string.Format("{0,3}", model.iID_MaChuDauTuID), lstMaDuAn[2]);
                    }
                    conn.Update(objDuAn, trans);
                }

                //Lưu danh sách nguồn vốn, hạng mục
                List<VDTChuTruongDauTuNguonVonModel> listNguonVonAdd = model.ListNguonVon.Where(x => (x.iID_ChuTruongDauTu_NguonVonID == null || x.iID_ChuTruongDauTu_NguonVonID == Guid.Empty) && !x.isDelete).ToList();
                List<VDTChuTruongDauTuNguonVonModel> listNguonVonEdit = model.ListNguonVon.Where(x => x.iID_ChuTruongDauTu_NguonVonID != null && x.iID_ChuTruongDauTu_NguonVonID != Guid.Empty && !x.isDelete).ToList();
                List<VDTChuTruongDauTuNguonVonModel> listNguonVonDelete = model.ListNguonVon.Where(x => x.iID_ChuTruongDauTu_NguonVonID != null && x.iID_ChuTruongDauTu_NguonVonID != Guid.Empty && x.isDelete).ToList();

                if (listNguonVonAdd.Count > 0)
                {
                    foreach (var item in listNguonVonAdd)
                    {
                        string thuocTinhKhongMap = "iID_ChuTruongDauTu_NguonVonID";
                        var nguonVon = new VDT_DA_ChuTruongDauTu_NguonVon();
                        nguonVon.MapFrom(item, thuocTinhKhongMap);
                        nguonVon.iID_ChuTruongDauTuID = entity.iID_ChuTruongDauTuID;
                        nguonVon.iID_ChuTruongDauTu_NguonVonID = Guid.NewGuid();
                        conn.Insert<VDT_DA_ChuTruongDauTu_NguonVon>(nguonVon, trans);
                    }
                }
                if (listNguonVonEdit.Count > 0)
                {
                    foreach (var item in listNguonVonEdit)
                    {
                        VDT_DA_ChuTruongDauTu_NguonVon chuTruongNguonVon = conn.Get<VDT_DA_ChuTruongDauTu_NguonVon>(item.iID_ChuTruongDauTu_NguonVonID, trans);
                        if (chuTruongNguonVon != null)
                        {
                            chuTruongNguonVon.iID_NguonVonID = item.iID_NguonVonID;
                            chuTruongNguonVon.fTienPheDuyet = item.fTienPheDuyet;
                            conn.Update<VDT_DA_ChuTruongDauTu_NguonVon>(chuTruongNguonVon, trans);
                        }
                    }
                }
                if (listNguonVonDelete.Count > 0)
                {
                    foreach (var item in listNguonVonDelete)
                    {
                        VDT_DA_ChuTruongDauTu_NguonVon chuTruongNguonVon = conn.Get<VDT_DA_ChuTruongDauTu_NguonVon>(item.iID_ChuTruongDauTu_NguonVonID, trans);
                        if (chuTruongNguonVon != null)
                        {
                            conn.Delete<VDT_DA_ChuTruongDauTu_NguonVon>(chuTruongNguonVon, trans);
                        }
                    }
                }

                List<VDTDADuAnHangMucModel> listDMHangMucAdd = model.ListHangMuc.Where(x => (x.iID_HangMucID == null || x.iID_HangMucID == Guid.Empty) && !x.isDelete).ToList();
                List<VDTDADuAnHangMucModel> listDMHangMucEdit = model.ListHangMuc.Where(x => x.iID_HangMucID != null && x.iID_HangMucID != Guid.Empty && !x.isDelete).ToList();
                List<VDTDADuAnHangMucModel> listChuTruongHangMucEdit = model.ListHangMuc.Where(x => x.iID_ChuTruongDauTu_HangMucID != null && x.iID_ChuTruongDauTu_HangMucID != Guid.Empty && !x.isDelete).ToList();
                List<VDTDADuAnHangMucModel> listDMHangMucDelete = model.ListHangMuc.Where(x => x.iID_HangMucID != null && x.iID_HangMucID != Guid.Empty && x.isDelete).ToList();

                List<VDTDADuAnHangMucModel> listChuTruongHangMucDieuChinhAdd = model.ListHangMuc.Where(x => !x.isDelete && x.iID_HangMucID != null && x.iID_HangMucID != Guid.Empty
                                                                                                        && (x.iID_ChuTruongDauTu_HangMucID == null || x.iID_ChuTruongDauTu_HangMucID == Guid.Empty)).ToList();

                string sMaDuAn = string.Empty;
                if (model.iID_DuAnID.HasValue)
                    sMaDuAn = conn.Get<VDT_DA_DuAn>(model.iID_DuAnID, trans).sMaDuAn;

                string sMaHangMuc = sMaDuAn.Substring(sMaDuAn.Length - 4);
                int indexMaHangMuc = _iQLVonDauTuService.GetIndexMaHangMuc();

                if (listDMHangMucAdd.Count > 0)
                {
                    foreach (var item in listDMHangMucAdd)
                    {
                        sMaHangMuc += indexMaHangMuc.ToString().PadLeft(3, '0');
                        indexMaHangMuc++;

                        var hangMuc = new VDT_DA_ChuTruongDauTu_DM_HangMuc
                        {
                            iID_ChuTruongDauTu_DM_HangMucID = item.iID_DuAn_HangMucID,
                            iID_DuAnID = entity.iID_DuAnID,
                            sMaHangMuc = sMaHangMuc,
                            sTenHangMuc = item.sTenHangMuc,
                            iID_ParentID = item.iID_ParentID,
                            iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID,
                            smaOrder = item.smaOrder
                        };

                        conn.Insert(hangMuc, trans);
                        var chuTruongHangMuc = new VDT_DA_ChuTruongDauTu_HangMuc();
                        chuTruongHangMuc.iID_ChuTruongDauTuID = entity.iID_ChuTruongDauTuID;
                        chuTruongHangMuc.iID_HangMucID = hangMuc.iID_ChuTruongDauTu_DM_HangMucID;
                        chuTruongHangMuc.fTienPheDuyet = item.fHanMucDauTu;
                        conn.Insert<VDT_DA_ChuTruongDauTu_HangMuc>(chuTruongHangMuc, trans);
                    }
                }

                if (listChuTruongHangMucDieuChinhAdd.Count > 0)
                {
                    foreach (var item in listChuTruongHangMucDieuChinhAdd)
                    {
                        var chuTruongHangMuc = new VDT_DA_ChuTruongDauTu_HangMuc();
                        chuTruongHangMuc.iID_ChuTruongDauTuID = entity.iID_ChuTruongDauTuID;
                        chuTruongHangMuc.iID_HangMucID = item.iID_HangMucID;
                        chuTruongHangMuc.fTienPheDuyet = item.fTienPheDuyet;
                        conn.Insert<VDT_DA_ChuTruongDauTu_HangMuc>(chuTruongHangMuc, trans);
                    }

                }

                if (listDMHangMucEdit.Count > 0)
                {
                    foreach (var item in listDMHangMucEdit)
                    {
                        VDT_DA_ChuTruongDauTu_DM_HangMuc danhMucHM = conn.Get<VDT_DA_ChuTruongDauTu_DM_HangMuc>(item.iID_DuAn_HangMucID, trans);
                        if (danhMucHM != null)
                        {
                            danhMucHM.sTenHangMuc = item.sTenHangMuc;
                            danhMucHM.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
                            conn.Update<VDT_DA_ChuTruongDauTu_DM_HangMuc>(danhMucHM, trans);
                        }
                    }
                }
                if (listDMHangMucDelete.Count > 0)
                {
                    foreach (var item in listDMHangMucDelete)
                    {
                        VDT_DA_ChuTruongDauTu_DM_HangMuc danhMucHM = conn.Get<VDT_DA_ChuTruongDauTu_DM_HangMuc>(item.iID_DuAn_HangMucID, trans);
                        VDT_DA_ChuTruongDauTu_HangMuc chuTruongHM = conn.Get<VDT_DA_ChuTruongDauTu_HangMuc>(item.iID_ChuTruongDauTu_HangMucID, trans);
                        if (danhMucHM != null)
                        {
                            conn.Delete<VDT_DA_ChuTruongDauTu_DM_HangMuc>(danhMucHM, trans);
                        }
                        if (chuTruongHM != null)
                        {
                            conn.Delete<VDT_DA_ChuTruongDauTu_HangMuc>(chuTruongHM, trans);
                        }
                    }
                }
                if (listChuTruongHangMucEdit.Count > 0)
                {
                    foreach (var item in listChuTruongHangMucEdit)
                    {
                        VDT_DA_ChuTruongDauTu_HangMuc chuTruongHM = conn.Get<VDT_DA_ChuTruongDauTu_HangMuc>(item.iID_ChuTruongDauTu_HangMucID, trans);
                        if (chuTruongHM != null)
                        {
                            chuTruongHM.fTienPheDuyet = item.fTienPheDuyet;
                            conn.Update<VDT_DA_ChuTruongDauTu_HangMuc>(chuTruongHM, trans);
                        }
                    }
                }

                trans.Commit();

            }

            return Json(new { status = true, iID_ChuTruongDauTuID = model.iID_ChuTruongDauTuID, sMessage = sMessage });
        }

        [HttpPost]
        public JsonResult Luu(VDTPheDuyetChuTruongDauTuModel model)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    Guid iIDChuTruongDauTuId = Guid.Empty;
                    if (model.chuTruongDauTu.iID_ChuTruongDauTuID == Guid.Empty)
                    {
                        #region Them moi VDT_DA_ChuTruongDauTu
                        var entity = new VDT_DA_ChuTruongDauTu();
                        entity.MapFrom(model.chuTruongDauTu);
                        entity.bActive = true;
                        entity.sUserCreate = Username;
                        entity.dDateCreate = DateTime.Now;
                        conn.Insert(entity, trans);

                        iIDChuTruongDauTuId = entity.iID_ChuTruongDauTuID;
                        #endregion

                    }
                    else
                    {
                        #region Sua VDT_DA_ChuTruongDauTu
                        var entity = conn.Get<VDT_DA_ChuTruongDauTu>(model.chuTruongDauTu.iID_ChuTruongDauTuID, trans);
                        if (entity == null)
                            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                        string thuocTinhKhongMap = "iID_ChuTruongDauTuID,iID_DuAnID,iID_TienTeID,iID_DonViTienTeID,fTiGiaDonVi,fTiGia,bActive,sLoaiDieuChinh,iLanDieuChinh,sTenDuAn,iID_DonViThucHienID,iID_LoaiDuAn,iID_HinhThucDauTuID,iID_DonViQuanLyID,iID_NhomQuanLyID,sUserCreate,dDateCreate,bActive";
                        entity.MapFrom(model.chuTruongDauTu, thuocTinhKhongMap);
                        entity.sUserUpdate = Username;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);
                        iIDChuTruongDauTuId = entity.iID_ChuTruongDauTuID;
                        #endregion
                    }

                    #endregion

                    // commit to db
                    trans.Commit();
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChiTiet(string id)
        {
            VDTChuTruongDauTuViewModel item = _iQLVonDauTuService.LayThongTinChiTietChuTruongDauTu(Guid.Parse(id));
            if (item == null)
                return RedirectToAction("Index");
            item.sDonViQuanLy = $"{item.iID_MaDonViQuanLy} - {item.sDonViQuanLy}";
            return View(item);
        }

        public ActionResult Sua(string id)
        {
            VDTChuTruongDauTuViewModel item = _iQLVonDauTuService.LayThongTinChiTietChuTruongDauTu(Guid.Parse(id));
            if (item == null)
                return RedirectToAction("Index");
            item.sDonViQuanLy = $"{item.iID_MaDonViQuanLy} - {item.sDonViQuanLy}";
            #region data selectlist
            List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnLapKeHoachTrungHanDuocDuyet().ToList();
            lstDuAn.Insert(0, new VDT_DA_DuAn { iID_DuAnID = Guid.Empty, sMaDuAn = Constants.CHON });
            ViewBag.ListDuAn = lstDuAn.Select(c => new SelectListItem
            {
                Value = c.iID_DuAnID.ToString(),
                Text = string.IsNullOrEmpty(c.sTenDuAn) ? c.sMaDuAn : (c.sMaDuAn + " - " + c.sTenDuAn)
            });

            List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayDanhMucChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = string.IsNullOrEmpty(c.sId_CDT) ? c.sTenCDT : (c.sId_CDT + " - " + c.sTenCDT)
            });

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.Select(c => new SelectListItem
            {
                Value = c.iID_Ma.ToString(),
                Text = string.IsNullOrEmpty(c.iID_MaDonVi) ? c.sTen : (c.iID_MaDonVi + " - " + c.sTen)
            });

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
            return View(item);
        }

        public ActionResult DieuChinh(string id)
        {


            VDTChuTruongDauTuViewModel item = _iQLVonDauTuService.LayThongTinChiTietChuTruongDauTu(Guid.Parse(id));
            if (item == null)
                return RedirectToAction("Index");

            VDTChuTruongDauTuViewModel itemDieuChinh = item;

            itemDieuChinh.sSoQuyetDinh = null;
            item.dNgayQuyetDinh = null;
            item.sDonViQuanLy = $"{item.iID_MaDonViQuanLy} - {item.sDonViQuanLy}";

            #region data selectlist
            List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayDanhMucChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = string.IsNullOrEmpty(c.sId_CDT) ? c.sTenCDT : (c.sId_CDT + " - " + c.sTenCDT)
            });

            List<VDT_DM_NhomDuAn> lstNhomDuAn = _iQLVonDauTuService.LayNhomDuAn().ToList();
            lstNhomDuAn.Insert(0, new VDT_DM_NhomDuAn { iID_NhomDuAnID = Guid.Empty, sTenNhomDuAn = Constants.CHON });
            ViewBag.ListNhomDuAn = lstNhomDuAn.ToSelectList("iID_NhomDuAnID", "sTenNhomDuAn");

            List<VDT_DM_HinhThucQuanLy> lstHinhThucQuanLy = _iQLVonDauTuService.LayHinhThucQuanLy().ToList();
            lstHinhThucQuanLy.Insert(0, new VDT_DM_HinhThucQuanLy { iID_HinhThucQuanLyID = Guid.Empty, sTenHinhThucQuanLy = Constants.CHON });
            ViewBag.ListHinhThucQLDA = lstHinhThucQuanLy.ToSelectList("iID_HinhThucQuanLyID", "sTenHinhThucQuanLy");

            List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _iQLVonDauTuService.LayPhanCapDuAn().ToList();
            lstPhanCapDuAn.Insert(0, new VDT_DM_PhanCapDuAn { iID_PhanCapID = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            itemDieuChinh.dNgayQuyetDinh = DateTime.Now;


            #endregion
            return View(itemDieuChinh);
        }

        public ActionResult SuaDieuChinh(string id)
        {
            VDTChuTruongDauTuViewModel item = _iQLVonDauTuService.LayThongTinChiTietChuTruongDauTu(Guid.Parse(id));
            if (item == null)
                return RedirectToAction("Index");
            item.sDonViQuanLy = $"{item.iID_MaDonViQuanLy} - {item.sDonViQuanLy}";
            #region data selectlist
            List<VDT_DA_DuAn> lstDuAn = _iQLVonDauTuService.LayDuAnLapKeHoachTrungHanDuocDuyet().ToList();
            lstDuAn.Insert(0, new VDT_DA_DuAn { iID_DuAnID = Guid.Empty, sMaDuAn = Constants.CHON });
            ViewBag.ListDuAn = lstDuAn.Select(c => new SelectListItem
            {
                Value = c.iID_DuAnID.ToString(),
                Text = string.IsNullOrEmpty(c.sTenDuAn) ? c.sMaDuAn : (c.sMaDuAn + " - " + c.sTenDuAn)
            });

            List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayDanhMucChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = string.IsNullOrEmpty(c.sId_CDT) ? c.sTenCDT : (c.sId_CDT + " - " + c.sTenCDT)
            });

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.Select(c => new SelectListItem
            {
                Value = c.iID_Ma.ToString(),
                Text = string.IsNullOrEmpty(c.iID_MaDonVi) ? c.sTen : (c.iID_MaDonVi + " - " + c.sMoTa)
            });

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
            return View(item);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            var sMessage = Constants.MESSAGE_DELETE_DUAN;
            var entity = _iQLVonDauTuService.GetThongTinChuTruongDauTuById(Guid.Parse(id));
            bool xoa = _iQLVonDauTuService.XoaChuTruongDauTu(Guid.Parse(id));
            if (xoa)
            {
                sMessage = string.Format(Constants.XOA_BAN_GHI, entity.sSoQuyetDinh);
                return Json(new { status = xoa, sMessage = sMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = xoa, sMessage = sMessage }, JsonRequestBehavior.AllowGet);
        }

        #region common
        public JsonResult LayThongTinChiTietDuAn(string iID)
        {
            VDT_DA_DuAn d_DuAn = _iQLVonDauTuService.LayThongTinChiTietDuAn(Guid.Parse(iID));
            return Json(d_DuAn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDanhSachHangMucTheoDuAnId(string iID)
        {
            IEnumerable<VDT_DA_DuAn_HangMuc> vDT_DA_DuAn_HangMuc = _iQLVonDauTuService.LayDanhSachHangMucTheoDuAnId(Guid.Parse(iID));
            return Json(vDT_DA_DuAn_HangMuc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDanhSachHangMucTheoChuTruongId(string iID)
        {
            IEnumerable<ChuTruongHangMucModel> listHangMuc = _iQLVonDauTuService.LayDanhSachHangMucTheoChuTruongId(Guid.Parse(iID));
            return Json(listHangMuc, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LayDanhSachNguonVonTheoDuAnId(string iID)
        {
            IEnumerable<VDTDADSNguonVonTheoIDDuAnModel> vDTDADSNguonVonTheoIDDuAnModel = _iQLVonDauTuService.LayDanhSachNguonVonTheoDuAnId(Guid.Parse(iID));
            return Json(vDTDADSNguonVonTheoIDDuAnModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadComboboxNguonVon()
        {
            var result = new List<dynamic>();
            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            foreach (var item in lstNguonVon)
            {
                result.Add(new
                {
                    id = item.iID_MaNguonNganSach,
                    text = item.sTen
                });
            }
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
        public JsonResult KiemTraTrungSoQuyetDinh(string sSoQuyetDinh, string iID_ChuTruongDauTuID = "")
        {
            bool status = _iQLVonDauTuService.KiemTraTrungSoQuyetDinhChuTruongDauTu(sSoQuyetDinh, iID_ChuTruongDauTuID);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataDMHangMuc(Guid iID_DuAnID)
        {
            var result = new List<dynamic>();
            var listData = _iQLVonDauTuService.GetDataDMDuAnHangMuc(iID_DuAnID);

            if (listData != null && listData.Any())
            {
                foreach (var item in listData)
                {
                    if (!string.IsNullOrEmpty(item.sMaHangMuc))
                    {
                        result.Add(new { id = item.sMaHangMuc, text = item.sTenHangMuc });
                    }
                }
            }

            return Json(new { status = true, data = result });
        }

        #endregion

        #region Export

        [HttpPost]
        public JsonResult ChuTruongDauTuExport()
        {
            try
            {
                //List<VdtKhvKeHoachVonUngChiTietModel> listDataQuery = _iQLVonDauTuService.KehoachVonUngDuocDuyetChiTietExport(id).ToList();
                var listData = (List<VDTChuTruongDauTuViewModel>)TempData["DataExcel"];

                var grouplistData = listData.GroupBy(x => x.sDonViQuanLy?.Trim());

                var results = new List<VDTChuTruongDauTuViewModel>();

                var index = 0;
                foreach (var item in grouplistData)
                {
                    //var rowTitle = new VDTChuTruongDauTuViewModel();
                    //rowTitle.sSTT = "";
                    //rowTitle.sRowTitle = item.Key;

                    //results.Add(rowTitle);
                    item.ToList().First().sRowTitle = item.Key;
                    results.AddRange(item.Select(x =>
                    {
                        x.sSTT = (++index).ToString();
                        return x;
                    }).ToList());
                }

                ExcelFile xls = CreateReportExport(results);
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

        public ExcelFile CreateReportExport(List<VDTChuTruongDauTuViewModel> lstData)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/ChuTruongDauTu/Export_DS_ChuTruongDauTu.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            //var dataModel = _iQLVonDauTuService.GetKHVUDuocDuyetById(id);
            //VDTKhvkeHoachVonUngViewModel objKHVU = new VDTKhvkeHoachVonUngViewModel();
            //objKHVU.sTenDonViQuanLy = dataModel.sTenDonViQuanLy.ToUpper();
            //objKHVU.sTenNguonVon = dataModel.sTenNguonVon.Substring(dataModel.sTenNguonVon.LastIndexOf(".") + 1).ToUpper();
            //objKHVU.iNamKeHoach = dataModel.iNamKeHoach;
            //objKHVU.sSoQuyetDinh = dataModel.sSoQuyetDinh;

            fr.AddTable<VDTChuTruongDauTuViewModel>("Items", lstData);

            //fr.SetValue("sTenDonVi", objKHVU.sTenDonViQuanLy);
            //fr.SetValue("sTenNguonVon", objKHVU.sTenNguonVon);
            //fr.SetValue("sSoQuyetDinh", objKHVU.sSoQuyetDinh);
            //fr.SetValue("iNamLamViec", objKHVU.iNamKeHoach);
            //fr.SetValue("day", dataModel.dNgayQuyetDinh.Value.ToString("dd", CultureInfo.InvariantCulture));
            //fr.SetValue("month", dataModel.dNgayQuyetDinh.Value.ToString("MM", CultureInfo.InvariantCulture));
            //fr.SetValue("year", dataModel.dNgayQuyetDinh.Value.ToString("yyyy", CultureInfo.InvariantCulture));
            fr.Run(Result);
            return Result;
        }

        public FileContentResult ExportExcelChuTruongDauTu()
        {
            string sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string sFileName = "ChuTruongDauTuExport.xlsx";
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