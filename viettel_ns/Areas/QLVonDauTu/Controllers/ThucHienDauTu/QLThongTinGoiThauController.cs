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

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThucHienDauTu
{
    public class QLThongTinGoiThauController : AppController
    {
        private readonly IQLVonDauTuService _vonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        // GET: QLVonDauTu/QLThongTinGoiThau
        public ActionResult Index()
        {
            IEnumerable<ThongTinGoiThauViewModel> listData = _vonDauTuService.GetAllThongTinGoiThau();
            return View(listData);
        }

        [HttpPost]
        public ActionResult GoiThauListView(string tenDuAn, string tenGoiThau, int giaTriMin = 0, int giaTriMax = 0)
        {
            IEnumerable<ThongTinGoiThauViewModel> lstData = _vonDauTuService.GetAllThongTinGoiThau(tenDuAn, tenGoiThau, giaTriMin, giaTriMax);

            return PartialView("_list", lstData);
        }

        public ActionResult Update(Guid id)
        {
            ViewBag.ListNhaThau = _vonDauTuService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");
            ThongTinGoiThauViewModel objDotnhan = _vonDauTuService.GetThongTinGoiThau(id);
            ViewBag.ChiTietDuAn = _vonDauTuService.GetThongTinDuAnByDuAnId(objDotnhan.iID_DuAnID.Value);
            objDotnhan.dNgayQuyetDinh = DateTime.Now;
            ViewBag.dBatDauNhaThau = objDotnhan.dBatDauChonNhaThau.HasValue ? objDotnhan.dBatDauChonNhaThau.Value.ToString("dd/MM/yyyy"): string.Empty;
            ViewBag.dKetThucNhaThau = objDotnhan.dKetThucChonNhaThau.HasValue ? objDotnhan.dKetThucChonNhaThau.Value.ToString("dd/MM/yyyy") : string.Empty;

            //ViewBag.ListChiPhi = _vonDauTuService.LayChiPhi().ToSelectList("iID_ChiPhi", "sTenChiPhi");
            //ViewBag.ListNguonVon = _vonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");

            //List<VDT_DA_TT_HopDong> lstHangMuc = _vonDauTuService.GetListHTHopDong(objDotnhan.iID_DuAnID.Value).ToList();
            //lstHangMuc.Insert(0, new VDT_DA_TT_HopDong { iID_LoaiHopDongID = Guid.Empty, sHinhThucHopDong = "--Chọn--" });
            //ViewBag.listHTHopDong = lstHangMuc.ToSelectList("iID_DuAnID", "sHinhThucHopDong");
            return View(objDotnhan);
        }
        

        public ActionResult Add()
        {
            ThongTinGoiThauViewModel objDotnhan = new ThongTinGoiThauViewModel();
            var listDonvi = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            ViewBag.ListDonViQuanLy = listDonvi.ToSelectList("iID_Ma", "sTen");
            ViewBag.ListNhaThau = _vonDauTuService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");
            return View(objDotnhan);
        }

        public JsonResult LayThongTinChiTietDuAn(string iID)
        {
            var d_DuAn = _vonDauTuService.LayThongTinChiTietDuAn(Guid.Parse(iID));
            var d_DauTu = _vonDauTuService.LayThongTinQDDauTu(Guid.Parse(iID));
            return Json(new { d_DuAn = d_DuAn, d_DauTu = d_DauTu }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(ThongTinGoiThauViewModel model)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var sMessage = string.Format("Thêm mới bản ghi {0} thành công", model.sSoQuyetDinh);
                if (model.iID_GoiThauID != null || model.iID_GoiThauID != Guid.Empty)
                {
                    sMessage = sMessage.Replace("Thêm mới", "Cập nhật");

                } 

                try
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var entity = new VDT_DA_GoiThau();
                    if (model.goiThau.iID_GoiThauID != Guid.Empty)
                    {
                        #region Sua
                        entity = conn.Get<VDT_DA_GoiThau>(model.goiThau.iID_GoiThauID, trans);
                        entity.sSoQuyetDinh = model.goiThau.sSoQuyetDinh;
                        entity.dNgayQuyetDinh = model.goiThau.dNgayQuyetDinh;
                        entity.sTenGoiThau = model.goiThau.sTenGoiThau;
                        entity.sHinhThucChonNhaThau = model.goiThau.sHinhThucChonNhaThau;
                        entity.sPhuongThucDauThau = model.goiThau.sPhuongThucDauThau;
                        entity.sHinhThucHopDong = model.goiThau.sHinhThucHopDong;
                        //entity.iThoiGianThucHien = model.goiThau.iThoiGianThucHien;
                        entity.dBatDauChonNhaThau = model.goiThau.dBatDauChonNhaThau;
                        entity.dKetThucChonNhaThau = model.goiThau.dKetThucChonNhaThau;
                        entity.iID_NhaThauID = model.goiThau.iID_NhaThauID;
                        entity.dDateUpdate = DateTime.Now;
                        entity.sUserUpdate = Username;
                        conn.Update(entity, trans);
                        #endregion

                    }

                    // commit to db
                    trans.Commit();
                }
                catch (Exception ex)
                {

                }
                
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public bool Delete(Guid id)
        {
            return _vonDauTuService.DeleteGoiThau(id, Username);
        }
        public ActionResult DieuChinh(Guid id)
        {
            ThongTinGoiThauViewModel objGoiThau = _vonDauTuService.GetThongTinGoiThau(id);
            ViewBag.ListChiPhi = _vonDauTuService.LayChiPhi().ToSelectList("iID_ChiPhi", "sTenChiPhi");
            ViewBag.ListNguonVon = _vonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
            ViewBag.ListHangMucDauTu = _vonDauTuService.ListHangMucByDuAn(objGoiThau.iID_DuAnID.Value,objGoiThau.dNgayQuyetDinh.Value).ToSelectList("iID_DuAn_HangMucID", "sTenHangMuc");
            ViewBag.ThongTinDuAn = _vonDauTuService.GetThongTinDuAnByGoiThauId(id);

            return View(objGoiThau);
        }

        [HttpPost]
        public JsonResult AddGoiThauDieuChinh(ThongTinGoiThauViewModel model,string iID_GoiThauID)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                var entity = new VDT_DA_GoiThau();
                if (model.goiThau.iID_GoiThauID == Guid.Empty)
                {
                    #region add goi thau bo sung
                    entity.MapFrom(model.goiThau);
                    entity.iID_ParentID = Guid.Parse(iID_GoiThauID);
                    entity.sUserCreate = Username;
                    entity.dDateCreate = DateTime.Now;
                    entity.bActive = true;
                    entity.bIsGoc = false;
                    entity.sUserCreate = Username;
                    entity.dDateCreate = DateTime.Now;
                    conn.Insert(entity, trans);
                    #endregion
                    // sua goi thau cha
                    VDT_DA_GoiThau objGoiThauCha = conn.Get<VDT_DA_GoiThau>(Guid.Parse(iID_GoiThauID), trans);
                    objGoiThauCha.bActive = false;
                    conn.Update(objGoiThauCha,trans);
                }
                
                //them moi data vao cac bang phu lien quan
                if (model.listChiPhi.Count() > 0)
                {
                    for (int i = 0; i < model.listChiPhi.Count(); i++)
                    {
                        var entityChiPhi = new VDT_DA_GoiThau_ChiPhi();
                        entityChiPhi.MapFrom(model.listChiPhi.ToList()[i]);
                        entityChiPhi.iID_GoiThauID = entity.iID_GoiThauID;
                        conn.Insert(entityChiPhi, trans);
                    }
                }

                #region Them moi nguon von
                if (model.listNguonVon.Count() > 0)
                {
                    for (int i = 0; i < model.listNguonVon.Count(); i++)
                    {
                        var entityNguonVon = new VDT_DA_GoiThau_NguonVon();
                        entityNguonVon.MapFrom(model.listNguonVon.ToList()[i]);
                        entityNguonVon.iID_GoiThauID = entity.iID_GoiThauID;
                        conn.Insert(entityNguonVon, trans);
                    }
                }
                #endregion
                #region Them moi hang muc
                if (model.listHangMuc.Count() > 0)
                {
                    for (int i = 0; i < model.listHangMuc.Count(); i++)
                    {
                        var entityHangMuc = new VDT_DA_GoiThau_HangMuc();
                        entityHangMuc.MapFrom(model.listHangMuc.ToList()[i]);
                        entityHangMuc.iID_GoiThauID = entity.iID_GoiThauID;
                        conn.Insert(entityHangMuc, trans);
                    }
                }
                #endregion
                // commit to db
                trans.Commit();
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(Guid id)
        {
            ViewBag.ListNhaThau = _vonDauTuService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");
            ThongTinGoiThauViewModel objDotnhan = _vonDauTuService.GetThongTinGoiThau(id);
            ViewBag.ChiTietDuAn = _vonDauTuService.GetThongTinDuAnByDuAnId(objDotnhan.iID_DuAnID.Value);

            return View(objDotnhan);
        }

        public JsonResult ListDuAnTheoDonViQuanLy(string iID_DonViQuanLyID)
        {
            List<VDT_DA_DuAn> lstDuAn = _vonDauTuService.ListDuAnTheoDonViQuanLy(Guid.Parse(iID_DonViQuanLyID)).ToList();
            StringBuilder htmlString = new StringBuilder();
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' data-sMaDuAn='{1}'>{2}</option>", lstDuAn[i].iID_DuAnID, lstDuAn[i].sMaDuAn, lstDuAn[i].sTenDuAn);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDataTheoDuAn(string iID_DuAnID,DateTime dNgayLap)
        {
            VDT_DA_DuAn d_DuAn = _vonDauTuService.LayThongTinChiTietDuAn(Guid.Parse(iID_DuAnID));
            List<VDT_DM_ChiPhi> listChiPhiByDuAn = _vonDauTuService.ListChiPhiByDuAn(Guid.Parse(iID_DuAnID),dNgayLap).ToList();
            List<NS_NguonNganSach> listNguonVonByDuAn = _vonDauTuService.ListNguonVonByDuAn(Guid.Parse(iID_DuAnID),dNgayLap).ToList();
            List<VDT_DA_DuAn_HangMuc> listHangMucByDuAn = _vonDauTuService.ListHangMucByDuAn(Guid.Parse(iID_DuAnID),dNgayLap).ToList();
            StringBuilder htmlStringCP = new StringBuilder();
            StringBuilder htmlStringNV = new StringBuilder();
            StringBuilder htmlStringHM = new StringBuilder();
            if (listChiPhiByDuAn != null && listChiPhiByDuAn.Count > 0)
            {
                htmlStringCP.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < listChiPhiByDuAn.Count; i++)
                {
                    htmlStringCP.AppendFormat("<option value='{0}' data-sMaChiPhi='{1}'>{2}</option>", listChiPhiByDuAn[i].iID_ChiPhi, listChiPhiByDuAn[i].sMaChiPhi, listChiPhiByDuAn[i].sTenChiPhi);
                }
            }
            if (listNguonVonByDuAn != null && listNguonVonByDuAn.Count > 0)
            {
                htmlStringNV.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < listNguonVonByDuAn.Count; i++)
                {
                    htmlStringNV.AppendFormat("<option value='{0}' >{1}</option>", listNguonVonByDuAn[i].iID_MaNguonNganSach,  listNguonVonByDuAn[i].sTen);
                }
            }
            if (listHangMucByDuAn != null && listHangMucByDuAn.Count > 0)
            {
                htmlStringHM.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < listHangMucByDuAn.Count; i++)
                {
                    htmlStringHM.AppendFormat("<option value='{0}' >{1}</option>", listHangMucByDuAn[i].iID_DuAn_HangMucID, listHangMucByDuAn[i].sTenHangMuc);
                }
            }
            return Json(new {listChiPhi = htmlStringCP.ToString(),listNguonVon = htmlStringNV.ToString(), listHangMuc = htmlStringHM.ToString(),duAn = d_DuAn }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTongMucDauTuChiPhi(Guid iID,Guid iID_DuAnID, DateTime dNgayLap)
        {
            var tongMucDauTuChiPhi = _vonDauTuService.GetTongMucDauTuChiPhi(iID, iID_DuAnID, dNgayLap);
            return Json(new { tongMucDauTuChiPhi }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTongMucDauTuNV(int iID, Guid iID_DuAnID, DateTime dNgayLap)
        {
            var tongMucDauTuNV = _vonDauTuService.GetTongMucDauTuNguonVon(iID, iID_DuAnID, dNgayLap);
            return Json(new { tongMucDauTuNV }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTongMucDauTuHM(Guid iID, Guid iID_DuAnID, DateTime dNgayLap)
        {
            var tongMucDauTuHM = _vonDauTuService.GetTongMucDauTuHangMuc(iID, iID_DuAnID, dNgayLap);
            return Json(new { tongMucDauTuHM }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiPhiDieuChinh(string iID_GoiThauGoc, DateTime dNgayLap)
        {
            IEnumerable<GoiThauChiPhiViewModel> lstChiPhi = _vonDauTuService.GetListChiPhiDieuChinh(Guid.Parse(iID_GoiThauGoc), dNgayLap);
            IEnumerable<GoiThauNguonVonViewModel> lstNguonVon = _vonDauTuService.GetListNguonVonDieuChinh(Guid.Parse(iID_GoiThauGoc), dNgayLap);
            IEnumerable<GoiThauHangMucViewModel> lstHangMuc = _vonDauTuService.GetListHangMucDieuChinh(Guid.Parse(iID_GoiThauGoc), dNgayLap);
            return Json(new { lstCp = lstChiPhi , lstNguonVon  = lstNguonVon , lstHangMuc = lstHangMuc }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListChiTietGoiThau(Guid id)
        {
            var listGoiThauNguonVon = _vonDauTuService.GetListNguonVonChuaDieuChinhByGoiThau(id);
            var listGoiThauChiPhi = _vonDauTuService.GetListChiPhiChuaDieuChinhByGoiThau(id);
            var listGoiThauHangMuc = _vonDauTuService.GetListHangMucChuaDieuChinhByGoiThau(id);
            var listHopDong = _vonDauTuService.GetThongTinHopDong(id);
            return Json(new { status = true, nguonvon = listGoiThauNguonVon,chiphi = listGoiThauChiPhi, hangmuc = listGoiThauHangMuc, hopdong = listHopDong });
        }
    }
}