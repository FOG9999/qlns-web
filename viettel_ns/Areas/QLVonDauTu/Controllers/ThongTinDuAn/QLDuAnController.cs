using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class QLDuAnController : AppController
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
            dataDuAn.lstData = _iQLVonDauTuService.GetAllDuAnTheoTrangThai(ref dataDuAn._paging, string.Empty, string.Empty , string.Empty , null, null, null, 1);

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

        public bool TTQLDuAnSave(VDTQuanLyDuAnModel data)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

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
                            return false;
                        string sMaDonViQuanLy = objDonViQuanLy.iID_MaDonVi;

                        var objChuDauTu = conn.Get<NS_DonVi>(entityDuAn.iID_ChuDauTuID, trans);
                        if (objChuDauTu == null)
                            return false;
                        string sMaChuDauTu = objChuDauTu.iID_MaDonVi;

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
                        }
                        #endregion
                    }
                    else
                    {
                        #region Sua du an
                        var entity = conn.Get<VDT_DA_DuAn>(data.duAn.iID_DuAnID, trans);

                        // tao sMaDuAn
                        var objDonViQuanLy = conn.Get<NS_DonVi>(entity.iID_DonViQuanLyID, trans);
                        if (objDonViQuanLy == null)
                            return false;
                        string sMaDonViQuanLy = objDonViQuanLy.iID_MaDonVi;

                        var objChuDauTu = conn.Get<NS_DonVi>(entity.iID_ChuDauTuID, trans);
                        if (objChuDauTu == null)
                            return false;
                        string sMaChuDauTu = objChuDauTu.iID_MaDonVi;

                        entity.sMaDuAn = string.Format("{0}-{1}-{2}", sMaDonViQuanLy, sMaChuDauTu, (entity.iMaDuAnIndex ?? 0).ToString("0000"));

                        entity.iID_DonViQuanLyID = data.duAn.iID_DonViQuanLyID;
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
                    }
                    // commit to db
                    trans.Commit();
                }
            } catch(Exception ex)
            {
                
            }
            

            return true;
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
                if(dataCTDT != null)
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
                IEnumerable<VDTDuAnListNguonVonTTDuAnModel> listNguonVonDuAn = _iQLVonDauTuService.GetListDuAnNguonVonTTDuAn(id);
                topHopThongTin.listNguonVonDuAn = listNguonVonDuAn;

                IEnumerable<VDT_DA_DuAn_HangMucModel> listDuAnHangMuc = _iQLVonDauTuService.GetListDuAnHangMucTTDuAn(id);
                topHopThongTin.listDuAnHangMuc = listDuAnHangMuc;

                #endregion

                /* tab Chủ trương đầu tư */
                IEnumerable <VDT_DA_ChuTruongDauTu> listDSPDCTDT = _iQLVonDauTuService.GetListCTDTByIdCTDT(topHopThongTin.dataCTDT.iID_ChuTruongDauTuID);
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
                    if(iID_QuyetToanID.HasValue && iID_QuyetToanID != Guid.Empty)
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
                errMes = "Bản ghi được sử dụng trong bảng Chủ trương đầu tư. Bạn không thể thực hiện thao tác này.";
            }
            else if (isExistQDDT)
            {
                errMes = "Bản ghi được sử dụng trong bảng Quyết định đầu tư. Bạn không thể thực hiện thao tác này.";
            }
            else if (isExistCTDT)
            {
                errMes = "Bản ghi được sử dụng trong bảng Chủ trương đầu tư. Bạn không thể thực hiện thao tác này.";
            }

            if (errMes != "")
            {
                bIsComplete = false;
            }
            return Json(new { bIsComplete, errMes = errMes }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool VDTDuAnDelete(Guid id)
        {
            if(!_iQLVonDauTuService.deleteVDTDuAn(id)) return false;
            return true;
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
    }
}