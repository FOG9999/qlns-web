using DapperExtensions;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.z.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using Constants = VIETTEL.Common.Constants;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class QLDuAnController : FlexcelReportController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;

        public ActionResult Index()
        {
            ViewBag.ListChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
            List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _iQLVonDauTuService.LayPhanCapDuAn().ToList();
            ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

            VDTDuAnPagingModel dataDuAn = new VDTDuAnPagingModel();
            dataDuAn._paging.CurrentPage = 1;
            dataDuAn.lstData = _iQLVonDauTuService.GetAllDuAnTheoTrangThai(ref dataDuAn._paging, string.Empty, string.Empty, string.Empty, null, null, null, 1);

            return View(dataDuAn);
        }

        [HttpPost]
        public ActionResult TimKiemDuAn(PagingInfo _paging, string sTenDuAn, string sKhoiCong, string sKetThuc, Guid? iID_DonViQuanLyID, Guid? iID_CapPheDuyetID, Guid? iID_LoaiCongTrinhID, int iTrangThai)
        {
            VDTDuAnPagingModel dataDuAn = new VDTDuAnPagingModel();
            dataDuAn._paging = _paging;
            dataDuAn.lstData = _iQLVonDauTuService.GetAllDuAnTheoTrangThai(ref dataDuAn._paging, sTenDuAn, sKhoiCong, sKetThuc, iID_DonViQuanLyID, iID_CapPheDuyetID, iID_LoaiCongTrinhID, iTrangThai);

            ViewBag.ListChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sMoTa");
            List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _iQLVonDauTuService.LayPhanCapDuAn().ToList();
            ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");
            return PartialView("_partialListDuAnChuaThucHien", dataDuAn);
        }

        public ActionResult CreateNew(Guid? id)
        {
            List<DM_ChuDauTu> lstChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec).ToList();
            lstChuDauTu.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDauTu = lstChuDauTu.ToSelectList("ID", "sTenCDT");

            List<VDT_DM_PhanCapDuAn> lstPhanCapDuAn = _iQLVonDauTuService.LayPhanCapDuAn().ToList();
            lstPhanCapDuAn.Insert(0, new VDT_DM_PhanCapDuAn { iID_PhanCapID = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListPhanCapPheDuyet = lstPhanCapDuAn.ToSelectList("iID_PhanCapID", "sTen");

            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            List<VDT_DM_NhomDuAn> lstNhomDuAn = _iQLVonDauTuService.LayNhomDuAn().ToList();
            lstNhomDuAn.Insert(0, new VDT_DM_NhomDuAn { iID_NhomDuAnID = Guid.Empty, sTenNhomDuAn = Constants.CHON });
            ViewBag.ListNhomDuAn = lstNhomDuAn.ToSelectList("iID_NhomDuAnID", "sTenNhomDuAn");

            List<VDT_DM_HinhThucQuanLy> lstHinhThucQuanLy = _iQLVonDauTuService.LayHinhThucQuanLy().ToList();
            lstHinhThucQuanLy.Insert(0, new VDT_DM_HinhThucQuanLy { iID_HinhThucQuanLyID = Guid.Empty, sTenHinhThucQuanLy = Constants.CHON });
            ViewBag.ListHinhThucQLDA = lstHinhThucQuanLy.ToSelectList("iID_HinhThucQuanLyID", "sTenHinhThucQuanLy");

            List<VDT_DM_ChiPhi> lstChiPhi = _iQLVonDauTuService.LayChiPhi().ToList();
            lstChiPhi.Insert(0, new VDT_DM_ChiPhi { iID_ChiPhi = Guid.Empty, sTenChiPhi = Constants.CHON });
            ViewBag.ListChiPhi = lstChiPhi.ToSelectList("iID_ChiPhi", "sTenChiPhi");

            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            VDT_DA_DuAn data = new VDT_DA_DuAn();
            if (id.HasValue)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    data = conn.Get<VDT_DA_DuAn>(id, trans);
                    // commit to db
                    trans.Commit();
                }
            }
            return View(data);
        }

        public ActionResult Import()
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            ViewBag.drpDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            return View();
        }

        public JsonResult TTQLDuAnSave(VDTQuanLyDuAnModel data)
        {
            var sMessage = string.Empty;
            var iID_DuAnID = Guid.Empty;

            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    sMessage = string.Format(Constants.THEM_BAN_GHI, data.duAn.sTenDuAn);
                    
                    if (data.duAn.iID_DuAnID == Guid.Empty)
                    {
                        #region Them moi VDT_DA_DuAn
                        var entityDuAn = new VDT_DA_DuAn();
                        entityDuAn.MapFrom(data.duAn);

                        // tao sMaDuAn
                        int iMaDuAnIndex = _iQLVonDauTuService.GetiMaDuAnIndex();
                        iMaDuAnIndex++;
                        var objDonViQuanLy = conn.Get<NS_DonVi>(entityDuAn.iID_DonViQuanLyID, trans);
                        if (objDonViQuanLy == null)
                            return Json(new { status = false, sMessage = string.Empty }, JsonRequestBehavior.AllowGet);
                        string sMaDonViQuanLy = objDonViQuanLy.iID_MaDonVi;

                        var objChuDauTu = conn.Get<DM_ChuDauTu>(entityDuAn.iID_ChuDauTuID, trans);
                        if (objChuDauTu == null)
                            return Json(new { status = false, sMessage = string.Empty }, JsonRequestBehavior.AllowGet);
                        string sMaChuDauTu = objChuDauTu.sId_CDT;

                        entityDuAn.iID_MaDonVi = sMaDonViQuanLy;
                        entityDuAn.iID_MaDonViThucHienDuAnID = sMaDonViQuanLy;
                        entityDuAn.sMaDuAn = string.Format("{0}-{1}-{2}", sMaDonViQuanLy, sMaChuDauTu, iMaDuAnIndex.ToString("0000"));
                        entityDuAn.iMaDuAnIndex = iMaDuAnIndex;

                        entityDuAn.sTrangThaiDuAn = "KhoiTao";
                        entityDuAn.bIsDeleted = false;
                        entityDuAn.sUserCreate = Username;
                        entityDuAn.dDateCreate = DateTime.Now;
                        conn.Insert(entityDuAn, trans);
                        #endregion

                        //#region Them moi VDT_DA_ChuTruongDauTu
                        //var entityCTDT = new VDT_DA_ChuTruongDauTu();
                        //entityCTDT.MapFrom(data.chuTruongDauTu);
                        //entityCTDT.iID_DuAnID = entityDuAn.iID_DuAnID;
                        //entityCTDT.bActive = true;
                        //entityCTDT.sUserCreate = Username;
                        //entityCTDT.dDateCreate = DateTime.Now;
                        //if (!String.IsNullOrEmpty(entityCTDT.sSoQuyetDinh))
                        //{
                        //    conn.Insert(entityCTDT, trans);
                        //}
                        //#endregion

                        //#region Them moi VDT_DA_QuyetDinhDauTu
                        //var entityQDDT = new VDT_DA_QDDauTu();
                        //entityQDDT.MapFrom(data.quyetDinhDauTu);
                        //entityQDDT.iID_DuAnID = entityDuAn.iID_DuAnID;
                        //entityQDDT.bIsGoc = true;
                        //entityQDDT.bActive = true;
                        //entityQDDT.sUserCreate = Username;
                        //entityQDDT.dDateCreate = DateTime.Now;
                        //if (!String.IsNullOrEmpty(entityQDDT.sSoQuyetDinh))
                        //{
                        //    conn.Insert(entityQDDT, trans);
                        //}
                        //#endregion

                        #region Them moi VDT_DA_DuAn_NguonVon
                        if (data.listChuTruongDauTuNguonVon != null && data.listChuTruongDauTuNguonVon.Count() > 0)
                        {
                            for (int i = 0; i < data.listChuTruongDauTuNguonVon.Count(); i++)
                            {
                                var entityNguonVon = new VDT_DA_DuAn_NguonVon();
                                entityNguonVon.MapFrom(data.listChuTruongDauTuNguonVon.ToList()[i]);
                                entityNguonVon.iID_DuAn = entityDuAn.iID_DuAnID;
                                conn.Insert(entityNguonVon, trans);
                            }
                        }
                        #endregion

                        #region Them moi VDT_DA_DuAn_HangMuc
                        int indexMaHangMuc = _iQLVonDauTuService.GetIndexMaHangMuc();
                        indexMaHangMuc++;
                        if (data.listDuAnHangMuc != null && data.listDuAnHangMuc.Count() > 0)
                        {
                            entityDuAn.iID_LoaiCongTrinhID = null;
                            conn.Update(entityDuAn, trans);
                            for (int i = 0; i < data.listDuAnHangMuc.Count(); i++)
                            {
                                var entityDuAnHangMuc = new VDT_DA_DuAn_HangMuc();
                                entityDuAnHangMuc.MapFrom(data.listDuAnHangMuc.ToList()[i]);
                                entityDuAnHangMuc.indexMaHangMuc = indexMaHangMuc;
                                entityDuAnHangMuc.sMaHangMuc = string.Format("{0}-{1}", iMaDuAnIndex, indexMaHangMuc.ToString("000"));
                                entityDuAnHangMuc.iID_DuAnID = entityDuAn.iID_DuAnID;
                                conn.Insert(entityDuAnHangMuc, trans);
                                iMaDuAnIndex++;
                            }
                        }else 
                        {
                            var entityDuAnHangMuc = new VDT_DA_DuAn_HangMuc();
                            entityDuAnHangMuc.indexMaHangMuc = indexMaHangMuc;
                            entityDuAnHangMuc.sMaHangMuc = string.Format("{0}-{1}", iMaDuAnIndex, indexMaHangMuc.ToString("000"));
                            entityDuAnHangMuc.iID_DuAnID = entityDuAn.iID_DuAnID;
                            conn.Insert(entityDuAnHangMuc, trans);
                            iMaDuAnIndex++;
                        }
                        iID_DuAnID = entityDuAn.iID_DuAnID;
                        #endregion
                    }
                    else
                    {
                        #region Sua du an
                        iID_DuAnID = data.duAn.iID_DuAnID;
                        var entity = conn.Get<VDT_DA_DuAn>(data.duAn.iID_DuAnID, trans);
                        sMessage = sMessage.Replace("Thêm mới", "Cập nhật");
                        // tao sMaDuAn
                        var objDonViQuanLy = conn.Get<NS_DonVi>(entity.iID_DonViQuanLyID, trans);
                        if (objDonViQuanLy == null)
                            return Json(new { status = false, sMessage = string.Empty }, JsonRequestBehavior.AllowGet);
                        string sMaDonViQuanLy = objDonViQuanLy.iID_MaDonVi;

                        var objChuDauTu = conn.Get<NS_DonVi>(entity.iID_ChuDauTuID, trans);
                        if (objChuDauTu == null)
                            return Json(new { status = false, sMessage = string.Empty }, JsonRequestBehavior.AllowGet);
                        string sMaChuDauTu = objChuDauTu.iID_MaDonVi;

                        entity.sMaDuAn = string.Format("{0}-{1}-{2}", sMaDonViQuanLy, sMaChuDauTu, (entity.iMaDuAnIndex ?? 0).ToString("0000"));

                        entity.iID_DonViQuanLyID = data.duAn.iID_DonViQuanLyID;
                        //entity.iID_DonViThucHienDuAnID = data.duAn.iID_DonViThucHienDuAnID;
                        entity.sTenDuAn = data.duAn.sTenDuAn;
                        entity.iID_ChuDauTuID = data.duAn.iID_ChuDauTuID;
                        entity.iID_CapPheDuyetID = data.duAn.iID_CapPheDuyetID;
                        entity.iID_LoaiCongTrinhID = data.duAn.iID_LoaiCongTrinhID;
                        entity.sDiaDiem = data.duAn.sDiaDiem;
                        entity.sSuCanThietDauTu = data.duAn.sSuCanThietDauTu;
                        entity.sMucTieu = data.duAn.sMucTieu;
                        entity.sDienTichSuDungDat = data.duAn.sDienTichSuDungDat;
                        entity.sNguonGocSuDungDat = data.duAn.sNguonGocSuDungDat;
                        entity.sQuyMo = data.duAn.sQuyMo;
                        entity.sKhoiCong = data.duAn.sKhoiCong;
                        entity.sKetThuc = data.duAn.sKetThuc;
                        entity.iID_NhomDuAnID = data.duAn.iID_NhomDuAnID;
                        entity.iID_HinhThucQuanLyID = data.duAn.iID_HinhThucQuanLyID;
                        entity.sCanBoPhuTrach = data.duAn.sCanBoPhuTrach;
                        entity.fHanMucDauTu = data.duAn.fHanMucDauTu;
                        entity.bIsDuPhong = data.duAn.bIsDuPhong;
                        entity.sUserUpdate = Username;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);
                        #endregion

                        #region Sua VDT_DA_DuAn_NguonVon
                        if (data.listChuTruongDauTuNguonVon != null && data.listChuTruongDauTuNguonVon.Count() > 0)
                        {
                            foreach (var nguonVonDuAn in data.listChuTruongDauTuNguonVon)
                            {
                                var entityNguonVon = conn.Get<VDT_DA_DuAn_NguonVon>(nguonVonDuAn.Id, trans);
                                if(entityNguonVon == null)
                                {
                                    entityNguonVon = new VDT_DA_DuAn_NguonVon();
                                    entityNguonVon.MapFrom(nguonVonDuAn);
                                    entityNguonVon.iID_DuAn = entity.iID_DuAnID;
                                    conn.Insert(entityNguonVon, trans);
                                }
                                else
                                {
                                    entityNguonVon.iID_NguonVonID = nguonVonDuAn.iID_NguonVonID;
                                    entityNguonVon.fThanhTien = nguonVonDuAn.fThanhTien;
                                    conn.Update(entityNguonVon, trans);
                                }
                                
                            }
                        }
                        #endregion
                    }
                    // commit to db
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, sMessage = string.Empty }, JsonRequestBehavior.AllowGet);

            }


            return Json(new { status = true, sMessage = sMessage, iID_DuAnID = iID_DuAnID }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult xemChiTietDuAn(Guid? id)
        {
            VDTQuanLyDuAnInfoModel topHopThongTin = new VDTQuanLyDuAnInfoModel();
            VDTDuAnInfoModel dataDuAn = new VDTDuAnInfoModel();
            VDT_DA_ChuTruongDauTu dataCTDT = new VDT_DA_ChuTruongDauTu();
            VDTDuAnThongTinPheDuyetModel dataQDDT = new VDTDuAnThongTinPheDuyetModel();
            VDTDuAnThongTinDuToanModel dataDuToan = new VDTDuAnThongTinDuToanModel();
            VDT_QT_QuyetToan dataQuyetToan = new VDT_QT_QuyetToan();
            VDT_DM_PhanCapDuAn dataPhanCapDuAn = new VDT_DM_PhanCapDuAn();
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                #region tab thong tin du an
                dataDuAn = _iQLVonDauTuService.GetDuAnById(id);
                dataCTDT = _iQLVonDauTuService.GetVDTChuTruongDauTu(id);
                if (dataCTDT != null)
                    dataPhanCapDuAn = _iQLVonDauTuService.GetPhanCapDuanByChuTruongDauTu(dataCTDT.iID_CapPheDuyetID);
                dataQDDT = _iQLVonDauTuService.GetVDTQDDauTu(id);
                dataDuToan = _iQLVonDauTuService.GetVDTDuToan(id);
                dataQuyetToan = _iQLVonDauTuService.GetVDTQuyetToan(id);

                topHopThongTin.dataDuAn = dataDuAn != null ? dataDuAn : new VDTDuAnInfoModel();
                topHopThongTin.dataCTDT = dataCTDT != null ? dataCTDT : new VDT_DA_ChuTruongDauTu();
                topHopThongTin.dataQDDT = dataQDDT != null ? dataQDDT : new VDTDuAnThongTinPheDuyetModel();
                topHopThongTin.dataDuToan = dataDuToan != null ? dataDuToan : new VDTDuAnThongTinDuToanModel();
                topHopThongTin.dataQuyetToan = dataQuyetToan != null ? dataQuyetToan : new VDT_QT_QuyetToan();
                topHopThongTin.dataPhanCapDuAn = dataPhanCapDuAn != null ? dataPhanCapDuAn : new VDT_DM_PhanCapDuAn();
                /*Bổ sung hiển thị thông tin nguồn vốn - Tab thông tin dự án*/
                IEnumerable<VDTDuAnListNguonVonTTDuAnModel> listNguonVonDuAn = _iQLVonDauTuService.GetListDuAnNguonVonTTDuAn(id).OrderBy(x => x.sTenNguonVon);
                topHopThongTin.listNguonVonDuAn = listNguonVonDuAn;

                IEnumerable<VDT_DA_DuAn_HangMucModel> listDuAnHangMuc = _iQLVonDauTuService.GetListDuAnHangMucTTDuAn(id);
                topHopThongTin.listDuAnHangMuc = listDuAnHangMuc;

                #endregion

                /* tab Chủ trương đầu tư */
                IEnumerable<VDT_DA_ChuTruongDauTu> listDSPDCTDT = _iQLVonDauTuService.GetListCTDTByIdCTDT(topHopThongTin.dataCTDT.iID_ChuTruongDauTuID);
                topHopThongTin.listDSPDCTDT = listDSPDCTDT;
                IEnumerable<VDTDuAnListCTDTChiPhiModel> listCTDTChiPhi = _iQLVonDauTuService.GetListCTDTChiPhi(topHopThongTin.dataCTDT.iID_ChuTruongDauTuID);
                topHopThongTin.listCTDTChiPhi = listCTDTChiPhi;
                IEnumerable<VDTDuAnListCTDTNguonVonModel> listCTDTNguonVon = _iQLVonDauTuService.GetListCTDTNguonVon(topHopThongTin.dataCTDT.iID_ChuTruongDauTuID);
                topHopThongTin.listCTDTNguonVon = listCTDTNguonVon;

                /* tab Quyết định đầu tư - thông tin phê duyệt dự án */

                IEnumerable<VDT_DA_DuToan> listDSTKTC = _iQLVonDauTuService.GetListDuToanByIdDuAn(topHopThongTin.dataDuAn.iID_DuAnID);
                topHopThongTin.listDSTKTC = listDSTKTC;
                IEnumerable<VDTDuAnListQDDTChiPhiModel> listQDDTChiPhi = _iQLVonDauTuService.GetListQDDTChiPhi(topHopThongTin.dataQDDT.iID_QDDauTuID, topHopThongTin.dataDuAn.iID_DuAnID);
                topHopThongTin.listQDDTChiPhi = listQDDTChiPhi;
                IEnumerable<VDTDuAnListQDDTNguonVonModel> listQDDTNguonVon = _iQLVonDauTuService.GetListQDDTNguonVon(topHopThongTin.dataQDDT.iID_QDDauTuID, topHopThongTin.dataDuAn.iID_DuAnID);
                topHopThongTin.listQDDTNguonVon = listQDDTNguonVon;
                IEnumerable<VDTDuAnListQDDTHangMucModel> listQDDTHangMuc = _iQLVonDauTuService.GetListQDDTHangMuc(topHopThongTin.dataQDDT.iID_QDDauTuID, topHopThongTin.dataDuAn.iID_DuAnID);
                topHopThongTin.listQDDTHangMuc = listQDDTHangMuc;

                /*tab Thông tin phê duyệt TKT&TDT*/
                IEnumerable<VDTDuAnListDuToanChiPhiModel> listDuToanChiPhi = _iQLVonDauTuService.GetListDuToanChiPhi(topHopThongTin.dataDuToan.iID_DuToanID, topHopThongTin.dataDuAn.iID_DuAnID);
                topHopThongTin.listDuToanChiPhi = listDuToanChiPhi;
                IEnumerable<VDTDuAnListDuToanNguonVonModel> listDuToanNguonVon = _iQLVonDauTuService.GetListDuToanNguonVon(topHopThongTin.dataDuToan.iID_DuToanID, topHopThongTin.dataDuAn.iID_DuAnID);
                topHopThongTin.listDuToanNguonVon = listDuToanNguonVon;
                IEnumerable<VDT_DA_DuToan> listDuToanDieuChinh = _iQLVonDauTuService.GetListDuToanDieuChinh(topHopThongTin.dataDuAn.iID_DuAnID);
                topHopThongTin.listDuToanDieuChinh = listDuToanDieuChinh;

                #region tab phê duyệt quyết toán
                VDTQLPheDuyetQuyetToanModel data = new VDTQLPheDuyetQuyetToanModel();
                data.quyetToan = new VDTQLPheDuyetQuyetToanViewModel();
                data.listQuyetToanChiPhi = new List<VDTChiPhiDauTuModel>();
                data.listQuyetToanNguonVon = new List<VDTNguonVonDauTuModel>();
                data.listNguonVonChenhLech = new List<VDTNguonVonDauTuViewModel>();
                if (id.HasValue)
                {
                    //Lay iID_QuyetToanID by id du an
                    Guid? iID_QuyetToanID = _iQLVonDauTuService.getQuyetToanID(id);
                    if (iID_QuyetToanID.HasValue && iID_QuyetToanID != Guid.Empty)
                    {
                        //Lay thong tin phe duyet quyet toan
                        data.quyetToan = _iQLVonDauTuService.GetVdtQuyetToanById(iID_QuyetToanID);
                        //Lay danh sach chi phi dau tu
                        data.listQuyetToanChiPhi = _iQLVonDauTuService.GetLstChiPhiDauTu(iID_QuyetToanID);
                        //Lay danh sach nguon von dau tu
                        data.listQuyetToanNguonVon = _iQLVonDauTuService.GetLstNguonVonDauTu(iID_QuyetToanID);

                        //Lay thong tin du an
                        VDTQLPheDuyetQuyetToanViewModel dataDuAnQT = new VDTQLPheDuyetQuyetToanViewModel();
                        dataDuAnQT = _iQLVonDauTuService.GetThongTinDuAn(topHopThongTin.dataDuAn.iID_DonViQuanLyID, topHopThongTin.dataDuAn.iID_DuAnID, data.quyetToan.dNgayQuyetDinh);
                        data.dataDuAnQT = dataDuAnQT;
                    }
                }
                topHopThongTin.dataPDQT = data;
                #endregion

                // commit to db
                trans.Commit();
            }

            return View(topHopThongTin);
        }

        [HttpPost]
        public JsonResult checkDeleteDuAn(Guid id)
        {
            string errMes = "";
            bool bIsComplete = true;
            bool isExistQDDT = _iQLVonDauTuService.CheckExistDuAnInQDDT(id);
            bool isExistCTDT = _iQLVonDauTuService.CheckExistDuAnInCTDT(id);
            if (isExistQDDT && isExistCTDT)
            {
                errMes = Constants.MESSAGE_DELETE_DUAN;
            }
            else if (isExistQDDT)
            {
                errMes = Constants.MESSAGE_DELETE_DUAN;
            }
            else if (isExistCTDT)
            {
                errMes = Constants.MESSAGE_DELETE_DUAN;
            }

            if (errMes != "")
            {
                bIsComplete = false;
            }
            return Json(new { bIsComplete, errMes = errMes }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult VDTDuAnDelete(Guid id)
        {
            var entity = _iQLVonDauTuService.GetDuAnById(id);
            var sMessage = Constants.XOA_BAN_GHI;
            if (entity != null)
            {
                sMessage = string.Format(sMessage, entity.sTenDuAn);
            }
            bool status = _iQLVonDauTuService.deleteVDTDuAn(id);
            return Json(new { status = status, sMessage = sMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool CheckExistMaDuAn(Guid iID_DuAnID, string sMaDuAn)
        {
            var isExist = _iQLVonDauTuService.CheckExistMaDuAn(iID_DuAnID, sMaDuAn);
            return isExist;
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
        public JsonResult GetLoaiCongTrinh()
        {
            var result = new List<dynamic>();
            var listModel = _dmService.GetAllDMLoaiCongTrinh().ToList();
            if (listModel != null && listModel.Any())
            {
                result.Add(new { id = "", text = "--Chọn--" });
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.iID_LoaiCongTrinh,
                        text = item.sTenLoaiCongTrinh
                    });
                }
            }
            return Json(new { status = true, data = result });
        }

        public JsonResult GetListNguonVonByDuAn(Guid id)
        {
            IEnumerable<VDTDuAnListNguonVonTTDuAnModel> listNguonVonDuAn = _iQLVonDauTuService.GetListDuAnNguonVonTTDuAn(id).OrderBy(x => x.sTenNguonVon);
            if (!listNguonVonDuAn.Any()) listNguonVonDuAn = new List<VDTDuAnListNguonVonTTDuAnModel>();
            return Json(new { status = true, data = listNguonVonDuAn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OnExport(List<Guid> ids)
        {
            try
            {
                List<DuAnViewModel> lstGoiThau = GetDataExport(ids);
                if (lstGoiThau != null)
                {
                    int i = 1;
                    foreach (var item in lstGoiThau)
                    {
                        item.IStt = i;
                        i++;
                    }
                }
                var lstNguonVon = _iQLVonDauTuService.GetListAllNguonNganSach();
                var lstLoaiCongTrinh = _iQLVonDauTuService.GetAllDmLoaiCongTrinh();
                XlsFile Result = new XlsFile(true);
                FlexCelReport fr = new FlexCelReport();
                fr.AddTable("Items", lstGoiThau);
                fr.AddTable("ItemsLoaiCongTrinh", lstLoaiCongTrinh);
                fr.AddTable("ItemsNguonVon", lstNguonVon);
                Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/tmp_Vdt_Da_ThongTinDuAn.xlsx"));
                fr.Run(Result);
                TempData["DataExportDuAnXls"] = Result;
                FlexCelPdfExport pdf = new FlexCelPdfExport(Result, true);
                var bufferPdf = new MemoryStream();
                pdf.Export(bufferPdf);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportReport()
        {
            ExcelFile xls = (ExcelFile)TempData["DataExportDuAnXls"];
            return Print(xls, "xls", string.Format("tmp_Vdt_Da_ThongTinDuAn_{0}.xlsx", DateTime.Now.ToString("ddMMyyyy_HHmm")));
        }

        private List<DuAnViewModel> GetDataExport(List<Guid> lstId)
        {
            Dictionary<Guid, VDT_DM_LoaiCongTrinh> dicLoaiCongTrinh = _iQLVonDauTuService.GetAllDmLoaiCongTrinh().ToDictionary(n => n.iID_LoaiCongTrinh, n => n);
            List<DuAnViewModel> lstDuAn = _iQLVonDauTuService.GetAllDuAnByListId(lstId);
            List<DuAnViewModel> results = new List<DuAnViewModel>();
            List<VDT_DA_DuAn_NguonVon> lstNguonVon = _iQLVonDauTuService.GetDuAnNguonVonByListDuAnId(lstId);
            List<VDT_DA_DuAn_HangMuc> lstHangMuc = _iQLVonDauTuService.GetDuAnHangMucByListDuAnId(lstId);
            Dictionary<Guid, List<VDT_DA_DuAn_NguonVon>> dicNguonVon = new Dictionary<Guid, List<VDT_DA_DuAn_NguonVon>>();
            Dictionary<Guid, List<VDT_DA_DuAn_HangMuc>> dicHangMuc = new Dictionary<Guid, List<VDT_DA_DuAn_HangMuc>>();
            if (lstNguonVon != null)
            {
                dicNguonVon = lstNguonVon.GroupBy(n => n.iID_DuAn).ToDictionary(n => n.Key, n => n.ToList());
            }
            if (lstHangMuc != null)
            {
                dicHangMuc = lstHangMuc.GroupBy(n => n.iID_DuAnID.Value).ToDictionary(n => n.Key, n => n.ToList());
            }
            int iStt = 1;
            foreach (var item in lstDuAn)
            {
                DuAnViewModel data = new DuAnViewModel()
                {
                    sTenDuAn = item.sTenDuAn,
                    sMaDuAn = item.sMaDuAn,
                    sKhoiCong = item.sKhoiCong,
                    sKetThuc = item.sKetThuc,
                    sDiaDiem = item.sDiaDiem,
                    sMucTieu = item.sMucTieu,
                    fHanMucDauTu = item.fHanMucDauTu
                };
                var hangmucs = new List<VDT_DA_DuAn_HangMuc>();
                var nguonvons = new List<VDT_DA_DuAn_NguonVon>();
                if (dicHangMuc.ContainsKey(item.iID_DuAnID)) hangmucs = dicHangMuc[item.iID_DuAnID];
                if (dicNguonVon.ContainsKey(item.iID_DuAnID)) nguonvons = dicNguonVon[item.iID_DuAnID];
                if (hangmucs.Count() >= nguonvons.Count())
                {
                    results.AddRange(GetDataByHangMuc(dicLoaiCongTrinh, data, hangmucs, nguonvons));
                }
                else
                {
                    results.AddRange(GetDataByNguonVon(dicLoaiCongTrinh, data, hangmucs, nguonvons));
                }
                iStt++;
            }

            return results;
        }

        private List<DuAnViewModel> GetDataByHangMuc(Dictionary<Guid, VDT_DM_LoaiCongTrinh> _dicLoaiCongTrinh, DuAnViewModel data, List<VDT_DA_DuAn_HangMuc> lstHangMuc, List<VDT_DA_DuAn_NguonVon> lstnguonVon)
        {
            List<DuAnViewModel> results = new List<DuAnViewModel>();
            VDT_DA_DuAn_NguonVon objNguonVon = new VDT_DA_DuAn_NguonVon();
            int countNguonVon = lstnguonVon.Count();
            int indexNguonVon = 0;
            foreach (var item in lstHangMuc)
            {
                if (indexNguonVon < countNguonVon)
                {
                    objNguonVon = lstnguonVon[indexNguonVon];
                    indexNguonVon++;
                }
                var current = data.Clone();
                current.sTenHangMuc = item.sTenHangMuc;
                if (item.iID_LoaiCongTrinhID.HasValue && _dicLoaiCongTrinh.ContainsKey(item.iID_LoaiCongTrinhID.Value))
                {
                    current.sMaLoaiCongTrinh = _dicLoaiCongTrinh[item.iID_LoaiCongTrinhID.Value].sMaLoaiCongTrinh;
                }
                current.iID_NguonVonID = (objNguonVon.iID_NguonVonID ?? 0);
                results.Add(current);
            }
            return results;
        }

        private List<DuAnViewModel> GetDataByNguonVon(Dictionary<Guid, VDT_DM_LoaiCongTrinh> _dicLoaiCongTrinh, DuAnViewModel data, List<VDT_DA_DuAn_HangMuc> lstHangMuc, List<VDT_DA_DuAn_NguonVon> lstnguonVon)
        {
            List<DuAnViewModel> results = new List<DuAnViewModel>();
            VDT_DA_DuAn_HangMuc objHangMuc = new VDT_DA_DuAn_HangMuc();
            int countHangMuc = lstHangMuc.Count();
            int indexHangMuc = 0;
            foreach (var item in lstnguonVon)
            {
                if (indexHangMuc < countHangMuc)
                {
                    objHangMuc = lstHangMuc[indexHangMuc];
                    indexHangMuc++;
                }
                var current = data.Clone();
                current.sTenHangMuc = objHangMuc.sTenHangMuc;
                if (objHangMuc.iID_LoaiCongTrinhID.HasValue && _dicLoaiCongTrinh.ContainsKey(objHangMuc.iID_LoaiCongTrinhID.Value))
                {
                    current.sMaLoaiCongTrinh = _dicLoaiCongTrinh[objHangMuc.iID_LoaiCongTrinhID.Value].sMaLoaiCongTrinh;
                }
                current.iID_NguonVonID = (item.iID_NguonVonID ?? 0);
                results.Add(current);
            }
            return results;
        }

        [HttpPost]
        public JsonResult LoadDataExcel(HttpPostedFileBase file)
        {
            try
            {
                List<string> lstError = new List<string>();
                if (file == null) return Json(new { bIsComplete = false, sMessError = "Chưa chọn file import !" }, JsonRequestBehavior.AllowGet);
                var file_data = getBytes(file);
                var dt = ExcelHelpers.LoadExcelDataTable(file_data);
                IEnumerable<VdtDaDuAnImportModel> dataImport = excel_result(dt, ref lstError);
                return Json(new { bIsComplete = true, data = dataImport, ListError = lstError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = false, sMessError = "Có lỗi xảy ra trong file import !" }, JsonRequestBehavior.AllowGet);
        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private IEnumerable<VdtDaDuAnImportModel> excel_result(System.Data.DataTable dt, ref List<string> lstError)
        {
            Dictionary<string, string> dicMaDuAn = new Dictionary<string, string>();
            Dictionary<string, Guid> dicLoaiCongTrinh = new Dictionary<string, Guid>();
            Dictionary<string, int> dicNguonVon = new Dictionary<string, int>();


            var lstAllDuAn = _iQLVonDauTuService.GetVDTDADuAn();
            if (lstAllDuAn != null)
            {
                foreach (var item in lstAllDuAn)
                {
                    if (!dicMaDuAn.ContainsKey(item.sMaDuAn)) dicMaDuAn.Add(item.sMaDuAn, item.sMaDuAn);
                }
            }
            var lstAllLoaiCongTrinh = _iQLVonDauTuService.GetAllDmLoaiCongTrinh();
            if (lstAllLoaiCongTrinh != null)
            {
                foreach (var item in lstAllLoaiCongTrinh)
                {
                    if (!dicLoaiCongTrinh.ContainsKey(item.sMaLoaiCongTrinh)) dicLoaiCongTrinh.Add(item.sMaLoaiCongTrinh, item.iID_LoaiCongTrinh);
                }
            }
            var lstNguonVon = _iQLVonDauTuService.GetListAllNguonNganSach();
            if (lstNguonVon != null)
            {
                foreach (var item in lstNguonVon)
                {
                    if (!dicNguonVon.ContainsKey(item.iID_MaNguonNganSach.ToString())) dicNguonVon.Add(item.iID_MaNguonNganSach.ToString(), item.iID_MaNguonNganSach);
                }
            }

            List<VdtDaDuAnImportModel> dataImport = new List<VdtDaDuAnImportModel>();

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) != "" || x.Field<string>(0) != null || x.Field<string>(0) != "null");
            int index = 1;
            for (var i = 1; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];

                var IStt = r.Field<string>(0);
                var sTenDuAn = r.Field<string>(1);
                var sMaDuAn = r.Field<string>(2);
                var sKhoiCong = r.Field<string>(3);
                var sKetThuc = r.Field<string>(4);
                var sTenHangMuc = r.Field<string>(5);
                var sMaLoaiCongTrinh = r.Field<string>(6);
                var iID_NguonVonID = r.Field<string>(7);
                var sDiaDiem = r.Field<string>(8);
                var sMucTieu = r.Field<string>(9);
                var fHanMucDauTu = r.Field<string>(10);

                var e = new VdtDaDuAnImportModel
                {
                    IStt = string.IsNullOrEmpty(IStt) ? string.Empty : IStt,
                    sTenDuAn = string.IsNullOrEmpty(sTenDuAn) ? string.Empty : sTenDuAn,
                    sMaDuAn = string.IsNullOrEmpty(sMaDuAn) ? string.Empty : sMaDuAn,
                    sKhoiCong = string.IsNullOrEmpty(sKhoiCong) ? string.Empty : sKhoiCong,
                    sKetThuc = string.IsNullOrEmpty(sKetThuc) ? string.Empty : sKetThuc,
                    sTenHangMuc = string.IsNullOrEmpty(sTenHangMuc) ? string.Empty : sTenHangMuc,
                    sMaLoaiCongTrinh = string.IsNullOrEmpty(sMaLoaiCongTrinh) ? string.Empty : sMaLoaiCongTrinh,
                    iID_NguonVonID = string.IsNullOrEmpty(iID_NguonVonID) ? string.Empty : iID_NguonVonID,
                    sDiaDiem = string.IsNullOrEmpty(sDiaDiem) ? string.Empty : sDiaDiem,
                    sMucTieu = string.IsNullOrEmpty(sMucTieu) ? string.Empty : sMucTieu,
                    fHanMucDauTu = string.IsNullOrEmpty(fHanMucDauTu) ? string.Empty : fHanMucDauTu,
                };
                e.bIsError = ValidateChungTuChiTietImport(index, e, dicLoaiCongTrinh, dicNguonVon, ref dicMaDuAn, ref lstError);
                dataImport.Add(e);
                index++;
            }
            return dataImport.AsEnumerable();
        }

        public bool ValidateChungTuChiTietImport(int index, VdtDaDuAnImportModel item, Dictionary<string, Guid> dicLoaiCongTrinh, Dictionary<string, int> dicNguonVon, ref Dictionary<string, string> dicMaDuAn, ref List<string> lstError)
        {
            double dDoublePare = 0;
            bool bIsError = false;

            if (string.IsNullOrEmpty(item.sTenDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "tên dự án"));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "mã dự án"));
                bIsError = true;
            }
            else if (dicMaDuAn.ContainsKey(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Đã tồn tại dự án [{1}] !", index, item.sMaDuAn));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.sMaLoaiCongTrinh) && !dicLoaiCongTrinh.ContainsKey(item.sMaLoaiCongTrinh))
            {
                lstError.Add(string.Format("Dòng {0} - Mã loại công trình [{1}] không tồn tại !", index, item.sMaLoaiCongTrinh));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.iID_NguonVonID) && !dicNguonVon.ContainsKey(item.iID_NguonVonID))
            {
                lstError.Add(string.Format("Dòng {0} - Mã nguồn vốn [{1}] không tồn tại !", index, item.sMaLoaiCongTrinh));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.fHanMucDauTu) && !double.TryParse(item.fHanMucDauTu, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Hạn mức đầu tư không phải kiểu dữ liệu số !", index));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.sMaLoaiCongTrinh) != string.IsNullOrEmpty(item.sTenHangMuc))
            {
                if (string.IsNullOrEmpty(item.sMaLoaiCongTrinh))
                {
                    lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "mã loại công trình"));
                    bIsError = true;
                }
                if (string.IsNullOrEmpty(item.sTenHangMuc))
                {
                    lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "hạng mục"));
                    bIsError = true;
                }
            }
            return bIsError;
        }

        public JsonResult OnSaveDataImport(Guid iIDDonViQuanLy, List<VdtDaDuAnImportModel> lstData)
        {
            try
            {
                Dictionary<string, VDT_DA_DuAn> dicDuAn = new Dictionary<string, VDT_DA_DuAn>();
                Dictionary<string, VDT_DA_DuAn_HangMuc> dicHangMuc = new Dictionary<string, VDT_DA_DuAn_HangMuc>();
                Dictionary<string, VDT_DA_DuAn_NguonVon> dicNguonVon = new Dictionary<string, VDT_DA_DuAn_NguonVon>();
                Dictionary<string, Guid> dicLoaiCongTrinh = new Dictionary<string, Guid>();
                var lstAllLoaiCongTrinh = _iQLVonDauTuService.GetAllDmLoaiCongTrinh();
                if (lstAllLoaiCongTrinh != null)
                {
                    foreach (var item in lstAllLoaiCongTrinh)
                    {
                        if (!dicLoaiCongTrinh.ContainsKey(item.sMaLoaiCongTrinh)) dicLoaiCongTrinh.Add(item.sMaLoaiCongTrinh, item.iID_LoaiCongTrinh);
                    }
                }
                var objDonVi = _iQLVonDauTuService.GetDonViQuanLyById(iIDDonViQuanLy);

                foreach (var item in lstData)
                {
                    if (!dicDuAn.ContainsKey(item.sMaDuAn))
                    {
                        dicDuAn.Add(item.sMaDuAn, new VDT_DA_DuAn()
                        {
                            iID_DuAnID = Guid.NewGuid(),
                            sTenDuAn = item.sTenDuAn,
                            sMaDuAn = item.sMaDuAn,
                            sKhoiCong = item.sKhoiCong,
                            sKetThuc = item.sKetThuc,
                            dDateCreate = DateTime.Now,
                            iID_DonViQuanLyID = objDonVi.iID_Ma,
                            iID_MaDonVi = objDonVi.iID_MaDonVi,
                            iID_DonViThucHienDuAnID = objDonVi.iID_Ma,
                            iID_MaDonViThucHienDuAnID = objDonVi.iID_MaDonVi,
                            sUserCreate = Username,
                            sTrangThaiDuAn = "KhoiTao"
                        });
                    }

                    if (!string.IsNullOrEmpty(item.sTenHangMuc))
                    {
                        string sKeyHangMuc = string.Format("{0}\n{1}\n{2}", item.sMaDuAn, item.sTenHangMuc, item.sMaLoaiCongTrinh);
                        if (!dicHangMuc.ContainsKey(sKeyHangMuc))
                        {
                            dicHangMuc.Add(sKeyHangMuc, new VDT_DA_DuAn_HangMuc()
                            {
                                iID_DuAn_HangMucID = Guid.NewGuid(),
                                sTenHangMuc = item.sTenHangMuc,
                                iID_LoaiCongTrinhID = dicLoaiCongTrinh[item.sMaLoaiCongTrinh],
                                iID_DuAnID = dicDuAn[item.sMaDuAn].iID_DuAnID
                            });
                        }
                    }

                    if (!string.IsNullOrEmpty(item.iID_NguonVonID))
                    {
                        if (!dicNguonVon.ContainsKey(item.iID_NguonVonID))
                        {
                            dicNguonVon.Add(item.iID_NguonVonID, new VDT_DA_DuAn_NguonVon()
                            {
                                Id = Guid.NewGuid(),
                                iID_NguonVonID = int.Parse(item.iID_NguonVonID),
                                iID_DuAn = dicDuAn[item.sMaDuAn].iID_DuAnID
                            });
                        }
                    }
                }

                if (dicDuAn.Count != 0)
                {
                    if (!_iQLVonDauTuService.AddRangerDuAn(dicDuAn.Values))
                    {
                        return Json(new { bIsSuccess = false }, JsonRequestBehavior.AllowGet);
                    }
                    _iQLVonDauTuService.AddRangerDuAnHangMuc(dicHangMuc.Values);
                    _iQLVonDauTuService.AddRangerDuAnNguonVon(dicNguonVon.Values);
                }

                return Json(new { bIsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json(new { bIsSuccess = false }, JsonRequestBehavior.AllowGet);
        }
    }
}