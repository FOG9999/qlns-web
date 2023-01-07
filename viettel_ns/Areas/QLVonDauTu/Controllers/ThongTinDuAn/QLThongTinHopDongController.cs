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
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLThongTinHopDong;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.z.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThongTinDuAn
{
    public class QLThongTinHopDongController : FlexcelReportController
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
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
            ViewBag.ListDonVi = lstDonViQL.ToSelectList("iID_MaDonVi", "sMoTa");
            //Lay danh sach chu dau tu theo nam lam viec
            List<DM_ChuDauTu> lstChuDT = _qLVonDauTuService.GetChuDTListByNamLamViec(PhienLamViec.NamLamViec).ToList();
            lstChuDT.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDT = lstChuDT.ToSelectList("sId_CDT", "sTenCDT");

            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoHopDong, double? fTienHopDongTu, double? fTienHopDongDen, DateTime? dHopDongTuNgay, DateTime? dHopDongDenNgay, string sTenDuAn, string sTenDonVi, string sChuDautu, string sTenHopDong)
        {
            VDTQuanLyTTHopDongViewModel vm = new VDTQuanLyTTHopDongViewModel();
            vm._paging = _paging;
            vm.Items = _qLVonDauTuService.GetAllVDTQuanLyTTHopDong(ref vm._paging, Username, PhienLamViec.NamLamViec, sSoHopDong, fTienHopDongTu, fTienHopDongDen, dHopDongTuNgay, dHopDongDenNgay, sTenDuAn, sTenDonVi, sChuDautu, sTenHopDong);
            //Lay danh sach don vi quan ly theo user login
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
            ViewBag.ListDonVi = lstDonViQL.ToSelectList("iID_MaDonVi", "sMoTa");
            //Lay danh sach chu dau tu theo nam lam viec
            List<DM_ChuDauTu> lstChuDT = _qLVonDauTuService.GetChuDTListByNamLamViec(PhienLamViec.NamLamViec).ToList();
            lstChuDT.Insert(0, new DM_ChuDauTu { ID = Guid.Empty, sTenCDT = Constants.CHON });
            ViewBag.ListChuDT = lstChuDT.ToSelectList("sId_CDT", "sTenCDT");

            return PartialView("_list", vm);
        }

        public ActionResult Import()
        {
            return View();
        }

        public ActionResult ThemMoi()
        {
            //Lay danh sach don vi quan ly theo user login
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = Constants.CHON });
            ViewBag.ListDonViQL = lstDonViQL.ToSelectList("iID_Ma", "sMoTa");

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

        public JsonResult LayThongTinHangMucByHopDongId(string hopdongId, string isGoc, List<GoiThauInfoModel> listGoiThau)
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

            if(listGoiThau != null)
            {
                hangMucAll = hangMucAll.Where(x => listGoiThau.Select(y => y?.Id).Contains(x.IdGoiThauNhaThau)).ToList();
            }

            hangMucAll.Select(n => { n.IdFake = (n.IIDHangMucID.ToString() + "_" + hangMucAll.IndexOf(n).ToString()); return n; }).ToList();
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
                var sMessage = string.Format(Constants.THEM_BAN_GHI, model.sSoHopDong);
                if (model.iID_HopDongID != null && model.iID_HopDongID != Guid.Empty)
                {
                    if (isDieuChinh)
                    {
                        sMessage = sMessage.Replace("Thêm mới", "Điều chỉnh");
                    }
                    else
                    {
                        sMessage = sMessage.Replace("Thêm mới", "Cập nhật");

                    }
                }

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
                            entity.dThoiGianBaoLanhHopDongTu = model.dThoiGianBaoLanhHopDongTu;
                            entity.dThoiGianBaoLanhHopDongDen = model.dThoiGianBaoLanhHopDongDen;
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

                        entityMoi.sTenHopDong = model.sTenHopDong;
                        entityMoi.dKhoiCongDuKien = model.dKhoiCongDuKien;
                        entityMoi.dKetThucDuKien = model.dKetThucDuKien;
                        entityMoi.sHinhThucHopDong = model.sHinhThucHopDong;
                        entityMoi.fTienHopDong = model.fTienHopDong;
                        entityMoi.iID_NhaThauThucHienID = model.iID_NhaThauThucHienID;
                        entityMoi.dThoiGianBaoLanhHopDongTu = model.dThoiGianBaoLanhHopDongTu;
                        entityMoi.dThoiGianBaoLanhHopDongDen = model.dThoiGianBaoLanhHopDongDen;

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
                                if (chiphi != null)
                                {
                                    chiphi.Where(n => n.IdGoiThauNhaThau.Value == item.Id).Select(n => { n.IdGoiThauNhaThau = itemGoiThau.Id; return n; }).ToList();
                                }
                                if (hangmuc != null)
                                {
                                    hangmuc.Where(n => n.IdGoiThauNhaThau.Value == item.Id).Select(n => { n.IdGoiThauNhaThau = itemGoiThau.Id; return n; }).ToList();
                                }
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
                return Json(new { status = true, sMessage = sMessage }, JsonRequestBehavior.AllowGet);
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
            var entity = _qLVonDauTuService.GetThongTinHopdongById(Guid.Parse(id));
            bool xoa = _qLVonDauTuService.XoaQLThongTinHopDong(Guid.Parse(id));
            var sMessage = string.Format(Constants.XOA_BAN_GHI, entity.sSoHopDong);

            _qLVonDauTuService.DeleteHopDongDetail(Guid.Parse(id));
            return Json(new { status = xoa, sMessage = sMessage }, JsonRequestBehavior.AllowGet);
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

        public bool IsHasHangMuc(string iID_DuAnID, string chiphiId, string goiThauId)
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

        public JsonResult GetSoTaiKhoanNhaThauByIdNhaThau(Guid? iID_NhaThauID)
        {
            var data = _qLVonDauTuService.GetSoTaiKhoanNhaThauByIdNhaThau(iID_NhaThauID);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OnExport(List<Guid> ids)
        {
            try
            {
                List<VDT_DA_TT_HopDong_ViewModel> lstHopDong = _qLVonDauTuService.GetDataExportHopDongByListId(ids);
                var lstLoaiHopDong = _qLVonDauTuService.GetDMPhanLoaiHopDong();
                XlsFile Result = new XlsFile(true);
                FlexCelReport fr = new FlexCelReport();
                if(lstHopDong != null)
                {
                    int i = 1;
                    foreach(var item in lstHopDong)
                    {
                        item.IStt = i;
                        i++;
                    }
                }
                fr.AddTable("Items", lstHopDong);
                fr.AddTable("ItemsLoaiHD", lstLoaiHopDong);
                Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/Import-HopDong-DuAn.xlsx"));
                fr.Run(Result);
                TempData["DataExportHopDongXls"] = Result;
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
            ExcelFile xls = (ExcelFile)TempData["DataExportHopDongXls"];
            return Print(xls, "xls", string.Format("Import-HopDong-DuAn_{0}.xlsx", DateTime.Now.ToString("ddMMyyyy_HHmm")));
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
                IEnumerable<VdtDaTtHopDongImportModel> dataImport = excel_result(dt, ref lstError);
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

        private IEnumerable<VdtDaTtHopDongImportModel> excel_result(System.Data.DataTable dt, ref List<string> lstError)
        {
            Dictionary<string, Guid> dicMaDuAn = new Dictionary<string, Guid>();
            Dictionary<string, Guid> dicNhaThau = new Dictionary<string, Guid>();
            Dictionary<string, Guid> dicLoaiHopDong = new Dictionary<string, Guid>();

            var lstAllDuAn = _qLVonDauTuService.GetVDTDADuAn();
            if (lstAllDuAn != null)
            {
                foreach (var item in lstAllDuAn)
                {
                    if (!dicMaDuAn.ContainsKey(item.sMaDuAn)) dicMaDuAn.Add(item.sMaDuAn, item.iID_DuAnID);
                }
            }
            var lstAllNhaThau = _qLVonDauTuService.GetAllNhaThau();
            if (lstAllNhaThau != null)
            {
                foreach (var item in lstAllNhaThau)
                {
                    if (!dicNhaThau.ContainsKey(item.sMaNhaThau)) dicNhaThau.Add(item.sMaNhaThau, item.iID_NhaThauID);
                }
            }
            var lstAllLoaiHd = _qLVonDauTuService.GetDMPhanLoaiHopDong();
            if (lstAllLoaiHd != null)
            {
                foreach (var item in lstAllLoaiHd)
                {
                    if (!dicLoaiHopDong.ContainsKey(item.sMaLoaiHopDong)) dicLoaiHopDong.Add(item.sMaLoaiHopDong, item.iID_LoaiHopDongID);
                }
            }

            List<VdtDaTtHopDongImportModel> dataImport = new List<VdtDaTtHopDongImportModel>();

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) != "" || x.Field<string>(0) != null || x.Field<string>(0) != "null");
            int index = 1;
            for (var i = 2; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];

                var IStt = r.Field<string>(0);
                var sMaDuAn = r.Field<string>(1);
                var sSoHopDong = r.Field<string>(2);
                var sTenHopDong = r.Field<string>(3);
                var sNgayHopDong = r.Field<string>(4);
                var sMaLoaiHopDong = r.Field<string>(5);
                var sMaNhaThau = r.Field<string>(6);
                var iThoiGianThucHien = r.Field<string>(7);
                var fTienHopDong = r.Field<string>(8);

                var e = new VdtDaTtHopDongImportModel
                {
                    IStt = string.IsNullOrEmpty(IStt) ? string.Empty : IStt,
                    sMaDuAn = string.IsNullOrEmpty(sMaDuAn) ? string.Empty : sMaDuAn,
                    sSoHopDong = string.IsNullOrEmpty(sSoHopDong) ? string.Empty : sSoHopDong,
                    sTenHopDong = string.IsNullOrEmpty(sTenHopDong) ? string.Empty : sTenHopDong,
                    sNgayHopDong = string.IsNullOrEmpty(sNgayHopDong) ? string.Empty : sNgayHopDong,
                    sMaLoaiHopDong = string.IsNullOrEmpty(sMaLoaiHopDong) ? string.Empty : sMaLoaiHopDong,
                    sMaNhaThau = string.IsNullOrEmpty(sMaNhaThau) ? string.Empty : sMaNhaThau,
                    iThoiGianThucHien = string.IsNullOrEmpty(iThoiGianThucHien) ? string.Empty : iThoiGianThucHien,
                    fTienHopDong = string.IsNullOrEmpty(fTienHopDong) ? string.Empty : fTienHopDong,
                };
                e.bIsError = ValidateChungTuChiTietImport(index, e, dicMaDuAn, dicNhaThau, dicLoaiHopDong, ref lstError);
                dataImport.Add(e);
                index++;
            }
            return dataImport.AsEnumerable();
        }

        public bool ValidateChungTuChiTietImport(int index, VdtDaTtHopDongImportModel item, Dictionary<string, Guid> dicMaDuAn, Dictionary<string, Guid> dicNhaThau, Dictionary<string,Guid> dicLoaiHopDong, ref List<string> lstError)
        {
            double dDoublePare = 0;
            int iIntPare = 0;
            DateTime dDatePare = DateTime.Now;
            bool bIsError = false;

            if (string.IsNullOrEmpty(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "mã dự án"));
                bIsError = true;
            }
            else if (!dicMaDuAn.ContainsKey(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Không tìm thấy dự án [{1}] !", index, item.sMaDuAn));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.sSoHopDong))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "Số hợp đồng"));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.sTenHopDong))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "Tên hợp đồng"));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.sNgayHopDong) && !DateTime.TryParseExact(item.sNgayHopDong, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dDatePare))
            {
                lstError.Add(string.Format("Dòng {0} - Ngày lập không đúng định dạng !", index));
                bIsError = true;
            }

            //if (string.IsNullOrEmpty(item.sMaLoaiHopDong))
            //{
            //    lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "mã loại hợp đồng"));
            //    bIsError = true;
            //}else 
            if (!string.IsNullOrEmpty(item.sMaLoaiHopDong) && !dicLoaiHopDong.ContainsKey(item.sMaLoaiHopDong))
            {
                lstError.Add(string.Format("Dòng {0} - Không tìm thấy loại hợp đồng [{1}] !", index, item.sMaLoaiHopDong));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.sMaNhaThau) && !dicNhaThau.ContainsKey(item.sMaNhaThau))
            {
                lstError.Add(string.Format("Dòng {0} - Không tìm thấy nhà thầu [{1}] !", index, item.sMaNhaThau));
                bIsError = true;
            }

            if (!string.IsNullOrEmpty(item.iThoiGianThucHien) && !int.TryParse(item.iThoiGianThucHien, out iIntPare))
            {
                lstError.Add(string.Format("Dòng {0} - Thời gian thực hiện không đúng định dạng !", index));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.fTienHopDong))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "giá trị hợp đồng"));
                bIsError = true;
            }
            else if (!double.TryParse(item.fTienHopDong, out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Giá trị hợp đồng không đúng định dạng !", index));
                bIsError = true;
            }
            return bIsError;
        }

        public JsonResult OnSaveDataImport(List<VdtDaTtHopDongImportModel> lstData)
        {
            try
            {
                Dictionary<string, Guid> dicMaDuAn = new Dictionary<string, Guid>();
                Dictionary<string, Guid> dicNhaThau = new Dictionary<string, Guid>();
                Dictionary<string, Guid> dicLoaiHopDong = new Dictionary<string, Guid>();
                var lstAllDuAn = _qLVonDauTuService.GetVDTDADuAn();
                if (lstAllDuAn != null)
                {
                    foreach (var item in lstAllDuAn)
                    {
                        if (!dicMaDuAn.ContainsKey(item.sMaDuAn)) dicMaDuAn.Add(item.sMaDuAn, item.iID_DuAnID);
                    }
                }
                var lstAllNhaThau = _qLVonDauTuService.GetAllNhaThau();
                if (lstAllNhaThau != null)
                {
                    foreach (var item in lstAllNhaThau)
                    {
                        if (!dicNhaThau.ContainsKey(item.sMaNhaThau)) dicNhaThau.Add(item.sMaNhaThau, item.iID_NhaThauID);
                    }
                }
                var lstAllLoaiHd = _qLVonDauTuService.GetDMPhanLoaiHopDong();
                if (lstAllLoaiHd != null)
                {
                    foreach (var item in lstAllLoaiHd)
                    {
                        if (!dicLoaiHopDong.ContainsKey(item.sMaLoaiHopDong)) dicLoaiHopDong.Add(item.sMaLoaiHopDong, item.iID_LoaiHopDongID);
                    }
                }

                List<VDT_DA_TT_HopDong> lstGoiThau = new List<VDT_DA_TT_HopDong>();
                foreach (var item in lstData)
                {
                    Guid iId = Guid.NewGuid();
                    VDT_DA_TT_HopDong obj = new VDT_DA_TT_HopDong();
                    obj.iID_HopDongID = iId;
                    obj.iID_HopDongGocID = iId;
                    obj.bActive = true;
                    obj.iID_DuAnID = dicMaDuAn[item.sMaDuAn];
                    obj.sSoHopDong = item.sSoHopDong;
                    obj.sTenHopDong = item.sTenHopDong;
                    if (!string.IsNullOrEmpty(item.sNgayHopDong))
                        obj.dNgayHopDong = DateTime.ParseExact(item.sNgayHopDong, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    obj.iID_LoaiHopDongID = dicLoaiHopDong[item.sMaLoaiHopDong];
                    if (!string.IsNullOrEmpty(item.sMaNhaThau))
                        obj.iID_NhaThauThucHienID = dicNhaThau[item.sMaNhaThau];
                    if (!string.IsNullOrEmpty(item.iThoiGianThucHien))
                        obj.iThoiGianThucHien = int.Parse(item.iThoiGianThucHien);
                    obj.fTienHopDong = double.Parse(item.fTienHopDong);
                    obj.dDateCreate = DateTime.Now;
                    obj.sUserCreate = Username;
                    lstGoiThau.Add(obj);
                }
                _qLVonDauTuService.AddRangerVdtDaTtHopDong(lstGoiThau);
                return Json(new { bIsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json(new { bIsSuccess = false }, JsonRequestBehavior.AllowGet);
        }
    }
}