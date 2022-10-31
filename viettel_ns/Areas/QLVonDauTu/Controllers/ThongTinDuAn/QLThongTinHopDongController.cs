using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLThongTinHopDong;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Common;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class QLThongTinHopDongController : AppController
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;

        private readonly INganSachService _nganSachService = NganSachService.Default;

        // GET: QLVonDauTu/QLDMThongTinHopDong
        public ActionResult Index()
        {
            VDTQuanLyTTHopDongViewModel vm = new VDTQuanLyTTHopDongViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qLVonDauTuService.GetAllVDTQuanLyTTHopDong(ref vm._paging, Username, PhienLamViec.NamLamViec);

            //Lay danh sach don vi quan ly theo user login
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonVi = lstDonViQL.ToSelectList("iID_MaDonVi", "sTen");
            //Lay danh sach chu dau tu theo nam lam viec
            List<DM_ChuDauTu> lstChuDT = _qLVonDauTuService.GetChuDTListByNamLamViec(PhienLamViec.NamLamViec).ToList();
            lstChuDT.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDT = lstChuDT.ToSelectList("sId_CDT", "sTenCDT");

            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoHopDong, double? fTienHopDongTu, double? fTienHopDongDen, DateTime? dHopDongTuNgay, DateTime? dHopDongDenNgay, string sTenDuAn, string sTenDonVi, string sChuDautu)
        {
            VDTQuanLyTTHopDongViewModel vm = new VDTQuanLyTTHopDongViewModel();
            vm._paging = _paging;
            vm.Items = _qLVonDauTuService.GetAllVDTQuanLyTTHopDong(ref vm._paging, Username, PhienLamViec.NamLamViec, sSoHopDong, fTienHopDongTu, fTienHopDongDen, dHopDongTuNgay, dHopDongDenNgay, sTenDuAn, sTenDonVi, sChuDautu);
            //Lay danh sach don vi quan ly theo user login
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonVi = lstDonViQL.ToSelectList("iID_MaDonVi", "sTen");
            //Lay danh sach chu dau tu theo nam lam viec
            List<DM_ChuDauTu> lstChuDT = _qLVonDauTuService.GetChuDTListByNamLamViec(PhienLamViec.NamLamViec).ToList();
            lstChuDT.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDT = lstChuDT.ToSelectList("sId_CDT", "sTenCDT");

            return PartialView("_list", vm);
        }

        public ActionResult ThemMoi()
        {
            //Lay danh sach don vi quan ly theo user login
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonViQL = lstDonViQL.ToSelectList("iID_Ma", "sTen");

            //Lay danh muc phan loai hop dong
            List<VDT_DM_LoaiHopDong> lstPhanLoaiHopDong = _qLVonDauTuService.GetDMPhanLoaiHopDong().ToList();
            lstPhanLoaiHopDong.Insert(0, new VDT_DM_LoaiHopDong { iID_LoaiHopDongID = Guid.Empty, sTenLoaiHopDong = Constants.CHON });
            ViewBag.ListPhanLoaiHopDong = lstPhanLoaiHopDong.ToSelectList("iID_LoaiHopDongID", "sTenLoaiHopDong");

            //Lay danh muc nha thau
            List<VDT_DM_NhaThau> lstNhaThau = _qLVonDauTuService.GetAllNhaThau().ToList();
            lstNhaThau.Insert(0, new VDT_DM_NhaThau { iID_NhaThauID = Guid.Empty, sTenNhaThau = Constants.CHON });
            ViewBag.ListNhaThau = lstNhaThau.ToSelectList("iID_NhaThauID", "sTenNhaThau");
            ViewBag.dThoiGianBatDauDuKien = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.dThoiGianKetThucDuKien = DateTime.Now.ToString("dd/MM/yyyy");

            return View();
        }

        public JsonResult LayDuAnTheoDonViQL(string iID_DonViQuanLyID)
        {
            //List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.LayDanhSachDuAnTheoDonViQuanLy(Guid.Parse(iID_DonViQuanLyID)).ToList();
            List<VDT_DA_DuAn> lstDuAn = _qLVonDauTuService.ListDuAnTheoDonViQuanLy(Guid.Parse(iID_DonViQuanLyID)).ToList();
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

        public JsonResult LayThongTinChiTietDuAn(string iID_DuAnID, string hopDongId)
        {
            //VDT_DA_DuAn duAn = _qLVonDauTuService.LayThongTinChiTietDuAn(Guid.Parse(iID_DuAnID));
            VDT_DA_TT_HopDong_ViewModel duAn = _qLVonDauTuService.LayThongTinChiTietDuAnTheoId(Guid.Parse(iID_DuAnID));
            List<GoiThauInfoModel> goithau = _qLVonDauTuService.GetThongTinGoiThauByDuAnIdAndHopDongId(Guid.Parse(iID_DuAnID),
                !string.IsNullOrEmpty(hopDongId) ? Guid.Parse(hopDongId) : Guid.Empty).ToList();
            goithau = string.IsNullOrEmpty(hopDongId) ? goithau : goithau.Where(n => n.iID_HopDongID != Guid.Empty).ToList();
            return Json(new { duan = duAn, goithau = goithau }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiTietGoiThauDb(string hopDongId)
        {
            if (string.IsNullOrEmpty(hopDongId))
            {
                return Json(new { goithau = new List<GoiThauInfoModel>() }, JsonRequestBehavior.AllowGet);
            }
            List<GoiThauInfoModel> goithau = _qLVonDauTuService.GetThongTinGoiThauByHopDongId(Guid.Parse(hopDongId)).ToList();
            return Json(new { goithau = goithau }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinHangMucPhuLuc(string iID_DuAnID, string hopDongId, string chiphiId, string goiThauId)
        {
            if (string.IsNullOrEmpty(chiphiId) || string.IsNullOrEmpty(iID_DuAnID))
            {
                return Json(new List<HangMucInfoModel>(), JsonRequestBehavior.AllowGet);
            }

            List<GoiThauInfoModel> goithau; // trường hợp thêm mới sẽ k có hopdongID, list này trống -> k có gói thầu nào
            List<VDT_DA_GoiThau> ltGoiThau = new List<VDT_DA_GoiThau>();
            if (hopDongId != "")
            {
                goithau = _qLVonDauTuService.GetThongTinGoiThauByDuAnIdAndHopDongId(Guid.Parse(iID_DuAnID),
                    !string.IsNullOrEmpty(hopDongId) ? Guid.Parse(hopDongId) : Guid.Empty).ToList();
            }
            else
            {
                ltGoiThau = _qLVonDauTuService.getListGoiThauKHLCNhaThau(Guid.Parse(goiThauId)).ToList();
                goithau = new List<GoiThauInfoModel>();
            }

            List<HangMucInfoModel> hangMucAll = new List<HangMucInfoModel>();
            if (goithau != null && goithau.Count() > 0)
            {
                hangMucAll = _qLVonDauTuService.GetThongTinHangMucAll(Guid.Empty, goithau.Select(n => n.IIDGoiThauID).ToList()).ToList();
            }

            if (ltGoiThau.Count() > 0)
            {
                hangMucAll = _qLVonDauTuService.GetThongTinHangMucAll(Guid.Empty, ltGoiThau.Select(n => n.iID_GoiThauID).ToList()).ToList();
            }

            List<HangMucInfoModel> list = hangMucAll.Where(n => n.IIDChiPhiID.HasValue && n.IIDChiPhiID.Value != Guid.Empty && n.IIDChiPhiID.Value == Guid.Parse(chiphiId)).OrderBy(n => n.MaOrDer).ToList();
            list.Select(n => { n.IdFake = (n.IIDHangMucID.ToString() + "_" + list.IndexOf(n).ToString()); return n; }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiPhiPhuLuc(string goiThauid, string idGoiThauNhaThau, string hopDongId, string iID_DuAnID)
        {
            if (string.IsNullOrEmpty(goiThauid))
            {
                return Json(new List<ChiPhiInfoModel>(), JsonRequestBehavior.AllowGet);
            }
            List<ChiPhiInfoModel> list = new List<ChiPhiInfoModel>();
            if (string.IsNullOrEmpty(hopDongId))
            {
                list = _qLVonDauTuService.GetThongTinChiPhiByGoiThauId(Guid.Parse(goiThauid)).ToList();
            }
            else
            {
                List<ChiPhiInfoModel> listDb = _qLVonDauTuService.GetThongTinChiPhiByHopDongId(Guid.Parse(hopDongId)).ToList();
                if (listDb != null && listDb.Where(n => n.Id.ToString() == idGoiThauNhaThau).ToList().Count() > 0)
                {
                    list = listDb.Where(n => n.Id.ToString() == idGoiThauNhaThau).ToList();
                }
                else
                {
                    list = _qLVonDauTuService.GetThongTinChiPhiByGoiThauId(Guid.Parse(goiThauid)).ToList();
                }
            }

            foreach (var thongTinChiPhi in list)
            {
                if (!IsHasHangMuc(iID_DuAnID, thongTinChiPhi.IIDChiPhiID.ToString(), thongTinChiPhi.IIDGoiThauID.ToString()))
                {
                    thongTinChiPhi.FTienGoiThau = thongTinChiPhi.FGiaTriDuocDuyet;
                    thongTinChiPhi.IsHasHangMuc = false;
                }
                else
                {
                    thongTinChiPhi.IsHasHangMuc = true;
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiPhiPhuLucByHopDongId(string hopdongId)
        {
            if (string.IsNullOrEmpty(hopdongId))
            {
                return Json(new List<ChiPhiInfoModel>(), JsonRequestBehavior.AllowGet);
            }
            List<ChiPhiInfoModel> list = _qLVonDauTuService.GetThongTinChiPhiByHopDongId(Guid.Parse(hopdongId)).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinHangMucByHopDongId(string hopdongId, string isGoc)
        {
            if (string.IsNullOrEmpty(hopdongId) || string.IsNullOrEmpty(isGoc))
            {
                return Json(new List<HangMucInfoModel>(), JsonRequestBehavior.AllowGet);
            }

            List<HangMucInfoModel> hangMucAll = new List<HangMucInfoModel>();
            if (isGoc == "True")
            {
                hangMucAll = _qLVonDauTuService.GetThongTinHangMucAllByHopDongId(Guid.Parse(hopdongId)).ToList();
            }
            else
            {
                hangMucAll = _qLVonDauTuService.GetThongTinDieuChinhHangMucAllByHopDongId(Guid.Parse(hopdongId)).ToList();
            }
            hangMucAll.Select(n => { n.IdFake = (n.IIDHangMucID.ToString() + "db" + "_" + hangMucAll.IndexOf(n).ToString()); return n; }).ToList();
            return Json(hangMucAll, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayGoiThauTheoDuAnId(string iID_DuAnID)
        {
            List<VDT_DA_GoiThau> lstGoiThau = _qLVonDauTuService.LayGoiThauTheoDuAnId(Guid.Parse(iID_DuAnID)).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
            if (lstGoiThau != null && lstGoiThau.Count > 0)
            {
                for (int i = 0; i < lstGoiThau.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}'>{1}</option>", lstGoiThau[i].iID_GoiThauID, lstGoiThau[i].sTenGoiThau);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiTietGoiThau(string iID_GoiThauID)
        {
            VDT_DA_GoiThau goiThau = _qLVonDauTuService.LayThongTinChiTietGoiThau(Guid.Parse(iID_GoiThauID));
            return Json(goiThau, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiTietNhaThau(string iID_NhaThauID)
        {
            VDT_DM_NhaThau nhaThau = _qLVonDauTuService.GetAllNhaThau().Where(x => x.iID_NhaThauID == Guid.Parse(iID_NhaThauID)).FirstOrDefault();
            return Json(nhaThau, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(VDT_DA_TT_HopDong_ViewModel model, List<GoiThauInfoModel> goithau, List<ChiPhiInfoModel> chiphi, List<HangMucInfoModel> hangmuc, bool isDieuChinh = false)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (!isDieuChinh)
                    {
                        if (model.iID_DuAnID == Guid.Empty)
                            model.iID_DuAnID = null;
                        if (model.iID_GoiThauID == Guid.Empty)
                            model.iID_GoiThauID = null;
                        if (model.iID_NhaThauThucHienID == Guid.Empty)
                            model.iID_NhaThauThucHienID = null;
                        if (model.iID_LoaiHopDongID == Guid.Empty)
                            model.iID_LoaiHopDongID = null;
                        Guid IdHopDong = Guid.Empty;
                        if (model.iID_HopDongID == Guid.Empty || model.iID_HopDongID == null)
                        {
                            #region Thêm mới Quản lý thông tin hợp đồng
                            var entity = new VDT_DA_TT_HopDong();
                            entity.MapFrom(model);
                            entity.bActive = true;
                            entity.bIsGoc = true;
                            entity.iTinhTrangHopDong = 1;
                            entity.sUserCreate = Username;
                            entity.dDateCreate = DateTime.Now;
                            conn.Insert(entity, trans);

                            //Update iID_HopDongGocID = iID_HopDongID
                            var entityUpdate = conn.Get<VDT_DA_TT_HopDong>(entity.iID_HopDongID, trans);
                            if (entity == null)
                                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                            entityUpdate.iID_HopDongGocID = entity.iID_HopDongID;
                            conn.Update(entityUpdate, trans);
                            IdHopDong = entity.iID_HopDongID;
                            #endregion
                        }
                        else
                        {
                            #region Update Quản lý thông tin hợp đồng
                            var entity = conn.Get<VDT_DA_TT_HopDong>(model.iID_HopDongID, trans);
                            if (entity == null)
                                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                            entity.sSoHopDong = model.sSoHopDong;
                            entity.sTenHopDong = model.sTenHopDong;
                            entity.dNgayHopDong = model.dNgayHopDong;
                            entity.iThoiGianThucHien = model.iThoiGianThucHien;
                            entity.dKhoiCongDuKien = model.dKhoiCongDuKien;
                            entity.dKetThucDuKien = model.dKetThucDuKien;
                            entity.iID_LoaiHopDongID = model.iID_LoaiHopDongID;
                            entity.sHinhThucHopDong = model.sHinhThucHopDong;
                            entity.fTienHopDong = model.fTienHopDong;
                            entity.iID_NhaThauThucHienID = model.iID_NhaThauThucHienID;
                            entity.NoiDungHopDong = model.NoiDungHopDong;
                            entity.sUserUpdate = Username;
                            entity.dDateUpdate = DateTime.Now;
                            conn.Update(entity, trans);
                            IdHopDong = entity.iID_HopDongID;
                            #endregion
                        }

                        _qLVonDauTuService.DeleteHopDongDetail(IdHopDong);

                        //Add goi thau
                        if (goithau != null)
                        {
                            foreach (var item in goithau)
                            {
                                VDT_DA_HopDong_GoiThau_NhaThau itemGoiThau = new VDT_DA_HopDong_GoiThau_NhaThau();
                                itemGoiThau.dDateCreate = DateTime.Now;
                                itemGoiThau.sUserCreate = Username;
                                itemGoiThau.fGiaTri = item.fGiaTriGoiThau;
                                itemGoiThau.iID_GoiThauID = item.IIDGoiThauID;
                                itemGoiThau.fGiaTriTrungThau = item.FGiaTriTrungThau;
                                itemGoiThau.iID_HopDongID = IdHopDong;
                                itemGoiThau.iID_NhaThauID = item.IIdNhaThauId;
                                itemGoiThau.Id = item.Id;
                                itemGoiThau.fGiaTriHD = item.fGiaTriHD;
                                conn.Insert(itemGoiThau, trans);
                            }
                        }
                        if (chiphi != null)
                        {
                            foreach (var item in chiphi)
                            {
                                VDT_DA_HopDong_GoiThau_ChiPhi itemChiPhi = new VDT_DA_HopDong_GoiThau_ChiPhi();
                                itemChiPhi.dDateCreate = DateTime.Now;
                                itemChiPhi.sUserCreate = Username;
                                itemChiPhi.fGiaTri = item.FTienGoiThau;
                                itemChiPhi.iID_ChiPhiID = item.IIDChiPhiID;
                                itemChiPhi.iID_HopDongGoiThauNhaThauID = item.IdGoiThauNhaThau;
                                conn.Insert(itemChiPhi, trans);
                            }
                        }
                        if (hangmuc != null)
                        {
                            foreach (var item in hangmuc)
                            {
                                VDT_DA_HopDong_GoiThau_HangMuc itemHangMuc = new VDT_DA_HopDong_GoiThau_HangMuc();
                                itemHangMuc.dDateCreate = DateTime.Now;
                                itemHangMuc.sUserCreate = Username;
                                itemHangMuc.fGiaTri = item.FTienGoiThau;
                                itemHangMuc.iID_ChiPhiID = item.IIDChiPhiID;
                                itemHangMuc.iID_HangMucID = item.IIDHangMucID;
                                itemHangMuc.iID_HopDongGoiThauNhaThauID = item.IdGoiThauNhaThau;
                                conn.Insert(itemHangMuc, trans);
                            }
                        }
                    }
                    else
                    {
                        #region Dieu chinh
                        VDT_DA_TT_HopDong entityCha = conn.Get<VDT_DA_TT_HopDong>(model.iID_HopDongID, trans);

                        // tao moi VDT_DA_TT_HopDong
                        VDT_DA_TT_HopDong entityMoi = new VDT_DA_TT_HopDong();
                        entityMoi.sSoHopDong = model.sSoHopDong;
                        entityMoi.dNgayHopDong = model.dNgayHopDong;
                        entityMoi.iThoiGianThucHien = model.iThoiGianThucHien;
                        entityMoi.iID_DuAnID = entityCha.iID_DuAnID;
                        entityMoi.iID_GoiThauID = entityCha.iID_GoiThauID;
                        entityMoi.iID_HopDongGocID = entityCha.iID_HopDongGocID;
                        entityMoi.iID_NhaThauThucHienID = entityCha.iID_NhaThauThucHienID;
                        entityMoi.iID_LoaiHopDongID = entityCha.iID_LoaiHopDongID;
                        entityMoi.fTienHopDong = model.fGiaTriDieuChinh;
                        entityMoi.iID_ParentID = entityCha.iID_HopDongID;
                        entityMoi.bIsGoc = false;
                        entityMoi.bActive = true;
                        entityMoi.iTinhTrangHopDong = 0;
                        entityMoi.NoiDungHopDong = model.NoiDungHopDong;
                        entityMoi.sUserCreate = Username;
                        entityMoi.dDateCreate = DateTime.Now;
                        conn.Insert(entityMoi, trans);

                        // cap nhat VDT_DA_TT_HopDong cha
                        entityCha.bActive = false;
                        entityCha.sUserUpdate = Username;
                        entityCha.dDateUpdate = DateTime.Now;
                        conn.Update(entityCha, trans);

                        //Add goi thau
                        if (goithau != null)
                        {
                            foreach (var item in goithau)
                            {
                                VDT_DA_HopDong_GoiThau_NhaThau itemGoiThau = new VDT_DA_HopDong_GoiThau_NhaThau();
                                itemGoiThau.dDateCreate = DateTime.Now;
                                itemGoiThau.sUserCreate = Username;
                                itemGoiThau.fGiaTri = item.fGiaTriGoiThau;
                                itemGoiThau.iID_GoiThauID = item.IIDGoiThauID;
                                itemGoiThau.fGiaTriTrungThau = item.FGiaTriTrungThau;
                                itemGoiThau.iID_HopDongID = entityMoi.iID_HopDongID;
                                itemGoiThau.iID_NhaThauID = item.IIdNhaThauId;
                                itemGoiThau.fGiaTriHD = item.fGiaTriHD;
                                itemGoiThau.Id = Guid.NewGuid();
                                chiphi.Where(n => n.IdGoiThauNhaThau.Value == item.Id).Select(n => { n.IdGoiThauNhaThau = itemGoiThau.Id; return n; }).ToList();
                                hangmuc.Where(n => n.IdGoiThauNhaThau.Value == item.Id).Select(n => { n.IdGoiThauNhaThau = itemGoiThau.Id; return n; }).ToList();
                                conn.Insert(itemGoiThau, trans);
                            }
                        }
                        if (chiphi != null)
                        {
                            foreach (var item in chiphi)
                            {
                                VDT_DA_HopDong_GoiThau_ChiPhi itemChiPhi = new VDT_DA_HopDong_GoiThau_ChiPhi();
                                itemChiPhi.dDateCreate = DateTime.Now;
                                itemChiPhi.sUserCreate = Username;
                                itemChiPhi.fGiaTri = item.FTienGoiThau;
                                itemChiPhi.iID_ChiPhiID = item.IIDChiPhiID;
                                itemChiPhi.iID_HopDongGoiThauNhaThauID = item.IdGoiThauNhaThau;
                                conn.Insert(itemChiPhi, trans);
                            }
                        }
                        if (hangmuc != null)
                        {
                            foreach (var item in hangmuc.Where(n => !string.IsNullOrEmpty(n.STenHangMuc)))
                            {
                                VDT_DA_HopDong_GoiThau_HangMuc itemHangMuc = new VDT_DA_HopDong_GoiThau_HangMuc();
                                itemHangMuc.dDateCreate = DateTime.Now;
                                itemHangMuc.sUserCreate = Username;
                                itemHangMuc.fGiaTri = item.FTienGoiThau;
                                itemHangMuc.iID_ChiPhiID = item.IIDChiPhiID;
                                itemHangMuc.iID_HangMucID = item.IIDHangMucID;
                                itemHangMuc.iID_HopDongGoiThauNhaThauID = item.IdGoiThauNhaThau;
                                conn.Insert(itemHangMuc, trans);

                                VDT_DA_HopDong_DM_HangMuc danhMucHangMuc = new VDT_DA_HopDong_DM_HangMuc();
                                danhMucHangMuc.Id = item.IIDHangMucID.Value;
                                danhMucHangMuc.iID_ChiPhiID = item.IIDChiPhiID;
                                danhMucHangMuc.sTenHangMuc = item.STenHangMuc;
                                danhMucHangMuc.iID_HopDongGoiThauNhaThauID = item.IdGoiThauNhaThau;
                                if (item.HangMucParentId.HasValue)
                                {
                                    danhMucHangMuc.iID_ParentID = item.HangMucParentId.Value;
                                }
                                danhMucHangMuc.maOrder = item.MaOrDer;
                                conn.Insert(danhMucHangMuc, trans);
                            }
                        }
                        #endregion
                    }
                    // commit to db
                    trans.Commit();
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckTrungSoHopDong(string val)
        {
            bool status = _qLVonDauTuService.KiemTraTrung("NH_DA_HopDong", "sSoHopDong", val);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            bool xoa = _qLVonDauTuService.XoaQLThongTinHopDong(Guid.Parse(id));
            _qLVonDauTuService.DeleteHopDongDetail(Guid.Parse(id));
            return Json(xoa, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sua(string id)
        {
            VDT_DA_TT_HopDong_ViewModel item = _qLVonDauTuService.LayChiTietThongTinHopDong(Guid.Parse(id));
            if (item == null)
                return RedirectToAction("Index");

            //Lay danh muc phan loai hop dong
            List<VDT_DM_LoaiHopDong> lstPhanLoaiHopDong = _qLVonDauTuService.GetDMPhanLoaiHopDong().ToList();
            lstPhanLoaiHopDong.Insert(0, new VDT_DM_LoaiHopDong { iID_LoaiHopDongID = Guid.Empty, sTenLoaiHopDong = Constants.CHON });
            ViewBag.ListPhanLoaiHopDong = lstPhanLoaiHopDong.ToSelectList("iID_LoaiHopDongID", "sTenLoaiHopDong");

            //Lay danh muc nha thau
            List<VDT_DM_NhaThau> lstNhaThau = _qLVonDauTuService.GetAllNhaThau().ToList();
            lstNhaThau.Insert(0, new VDT_DM_NhaThau { iID_NhaThauID = Guid.Empty, sTenNhaThau = Constants.CHON });
            ViewBag.ListNhaThau = lstNhaThau.ToSelectList("iID_NhaThauID", "sTenNhaThau");

            return View(item);
        }

        public JsonResult GetListNhaThau()
        {
            List<VDT_DM_NhaThau> lstNhaThau = _qLVonDauTuService.GetAllNhaThau().ToList();
            lstNhaThau.Insert(0, new VDT_DM_NhaThau { iID_NhaThauID = Guid.Empty, sTenNhaThau = Constants.CHON });
            return Json(lstNhaThau, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Adjusted(string id)
        {
            VDT_DA_TT_HopDong_ViewModel item = _qLVonDauTuService.LayChiTietThongTinHopDong(Guid.Parse(id));
            if (item == null)
                return RedirectToAction("Index");

            //Lay danh muc phan loai hop dong
            List<VDT_DM_LoaiHopDong> lstPhanLoaiHopDong = _qLVonDauTuService.GetDMPhanLoaiHopDong().ToList();
            lstPhanLoaiHopDong.Insert(0, new VDT_DM_LoaiHopDong { iID_LoaiHopDongID = Guid.Empty, sTenLoaiHopDong = Constants.CHON });
            ViewBag.ListPhanLoaiHopDong = lstPhanLoaiHopDong.ToSelectList("iID_LoaiHopDongID", "sTenLoaiHopDong");

            //Lay danh muc nha thau
            List<VDT_DM_NhaThau> lstNhaThau = _qLVonDauTuService.GetAllNhaThau().ToList();
            lstNhaThau.Insert(0, new VDT_DM_NhaThau { iID_NhaThauID = Guid.Empty, sTenNhaThau = Constants.CHON });
            ViewBag.ListNhaThau = lstNhaThau.ToSelectList("iID_NhaThauID", "sTenNhaThau");

            return View(item);
        }

        public ActionResult ChiTiet(string id)
        {
            VDT_DA_TT_HopDong_ViewModel item = _qLVonDauTuService.LayChiTietThongTinHopDong(Guid.Parse(id));
            if (item == null)
                return RedirectToAction("Index");
            return View(item);
        }

        public ActionResult DieuChinh(string id)
        {
            VDT_DA_TT_HopDong_ViewModel item = _qLVonDauTuService.LayChiTietThongTinHopDong(Guid.Parse(id));

            List<SelectListItem> lstLoaiDieuChinh = new List<SelectListItem> {
                new SelectListItem{Text = "", Value=""},
                new SelectListItem{Text = "Điều chỉnh(-)", Value="0"},
                new SelectListItem{Text = "Bổ sung(+)", Value="1"}
            };
            ViewBag.ListLoaiDieuChinh = lstLoaiDieuChinh.ToSelectList("Value", "Text");

            if (item == null)
                return RedirectToAction("Index");
            return View(item);
        }

        public JsonResult LayGiaTriTruocDieuChinh(string id, DateTime dNgayHopDong)
        {
            double? fTien = _qLVonDauTuService.LayGiaTriTruocDieuChinhHopDong(Guid.Parse(id), dNgayHopDong);
            return Json(fTien, JsonRequestBehavior.AllowGet);
        }

        public bool IsHasHangMuc(string iID_DuAnID,string chiphiId, string goiThauId)
        {
            if (string.IsNullOrEmpty(chiphiId) || string.IsNullOrEmpty(iID_DuAnID))
            {
                return false;
            }

            List<GoiThauInfoModel> goithau; 
            List<VDT_DA_GoiThau> ltGoiThau = new List<VDT_DA_GoiThau>();
         
            ltGoiThau = _qLVonDauTuService.getListGoiThauKHLCNhaThau(Guid.Parse(goiThauId)).ToList();
            goithau = new List<GoiThauInfoModel>();
            

            List<HangMucInfoModel> hangMucAll = new List<HangMucInfoModel>();
            if (goithau != null && goithau.Count() > 0)
            {
                hangMucAll = _qLVonDauTuService.GetThongTinHangMucAll(Guid.Empty, goithau.Select(n => n.IIDGoiThauID).ToList()).ToList();
            }

            if (ltGoiThau.Count() > 0)
            {
                hangMucAll = _qLVonDauTuService.GetThongTinHangMucAll(Guid.Empty, ltGoiThau.Select(n => n.iID_GoiThauID).ToList()).ToList();
            }

            List<HangMucInfoModel> list = hangMucAll.Where(n => n.IIDChiPhiID.HasValue && n.IIDChiPhiID.Value != Guid.Empty && n.IIDChiPhiID.Value == Guid.Parse(chiphiId)).OrderBy(n => n.MaOrDer).ToList();
            
            return list.Count > 0;
        }
    }
}