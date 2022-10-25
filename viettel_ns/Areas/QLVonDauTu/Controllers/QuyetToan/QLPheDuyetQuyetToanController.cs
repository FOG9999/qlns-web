using Dapper;
using DapperExtensions;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class QLPheDuyetQuyetToanController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        // GET: QLVonDauTu/QLPheDuyetQuyetToan
        public ActionResult Index()
        {
            ViewBag.ListChuDauTu = _iQLVonDauTuService.LayChuDauTu(PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");

            VDTPheDuyetQuyetToanPagingModel dataQuyetToan = new VDTPheDuyetQuyetToanPagingModel();
            dataQuyetToan._paging.CurrentPage = 1;
            dataQuyetToan.lstData = _iQLVonDauTuService.GetAllPheDuyetQuyetToan(ref dataQuyetToan._paging, string.Empty, null, null, string.Empty, null, null);

            return View(dataQuyetToan);
        }

        [HttpPost]
        public ActionResult TimKiemPheDuyetQuyetToan(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, string sTenDuAn, double? fTienQuyetToanPheDuyetFrom, double? fTienQuyetToanPheDuyetTo)
        {
            VDTPheDuyetQuyetToanPagingModel vm = new VDTPheDuyetQuyetToanPagingModel();
            vm._paging = _paging;
            vm.lstData = _iQLVonDauTuService.GetAllPheDuyetQuyetToan(ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sTenDuAn, fTienQuyetToanPheDuyetFrom, fTienQuyetToanPheDuyetTo);
            return PartialView("_partialListPheDuyetQuyetToanDA", vm);
        }

        public ActionResult CreateNew(Guid? id)
        {
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.CHON });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");

            List<VDT_DM_ChiPhi> lstChiPhi = _iQLVonDauTuService.LayChiPhi().ToList();
            lstChiPhi.Insert(0, new VDT_DM_ChiPhi { iID_ChiPhi = Guid.Empty, sTenChiPhi = Constants.CHON });
            ViewBag.ListChiPhi = lstChiPhi.ToSelectList("iID_ChiPhi", "sTenChiPhi");

            List<NS_NguonNganSach> lstNguonVon = _iQLVonDauTuService.LayNguonVon().ToList();
            lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = Constants.CHON });
            ViewBag.ListNguonVon = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");

            VDTQLPheDuyetQuyetToanModel data = new VDTQLPheDuyetQuyetToanModel();
            data.quyetToan = new VDTQLPheDuyetQuyetToanViewModel();
            data.listQuyetToanChiPhi = new List<VDTChiPhiDauTuModel>();
            data.listQuyetToanNguonVon = new List<VDTNguonVonDauTuModel>();
            data.listNguonVonChenhLech = new List<VDTNguonVonDauTuViewModel>();
            data.listChiPhi = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            data.listHangMuc = new List<VDT_DA_DuToan_HangMuc_ViewModel>();
            if (id.HasValue)
            {
                //Lay thong tin phe duyet quyet toan
                data.quyetToan = _iQLVonDauTuService.GetVdtQuyetToanById(id);
                //Lay danh sach chi phi dau tu
                data.listQuyetToanChiPhi = _iQLVonDauTuService.GetLstChiPhiDauTu(id);
                //Lay danh sach nguon von dau tu
                data.listQuyetToanNguonVon = _iQLVonDauTuService.GetLstNguonVonDauTu(id);

            }
            ViewBag.isDetail = 0;
            return View(data);
        }

        public bool QLPheDuyetQuyetToanSave(VDTQLPheDuyetQuyetToanModel data)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                if (data.quyetToan.iID_QuyetToanID == Guid.Empty)
                {
                    #region Them moi VDT_QT_QuyetToan
                    var entityQuyetToan = new VDT_QT_QuyetToan();
                    entityQuyetToan.MapFrom(data.quyetToan);
                    entityQuyetToan.iID_QuyetToanID = Guid.NewGuid();
                    entityQuyetToan.sUserCreate = Username;
                    entityQuyetToan.dDateCreate = DateTime.Now;
                    conn.Insert(entityQuyetToan, trans);
                    #endregion

                    #region Them moi VDT_QT_QuyetToan_ChiTiet
                    //if (data.listQuyetToanChiPhi != null && data.listQuyetToanChiPhi.Count() > 0)
                    //{
                    //    for (int i = 0; i < data.listQuyetToanChiPhi.Count(); i++)
                    //    {
                    //        var entityQTChiPhi = new VDT_QT_QuyetToan_ChiPhi();
                    //        entityQTChiPhi.MapFrom(data.listQuyetToanChiPhi.ToList()[i]);
                    //        entityQTChiPhi.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                    //        conn.Insert(entityQTChiPhi, trans);
                    //    }
                    //}
                    if(data.listChiPhi != null && data.listChiPhi.Count > 0)
                    {                        
                        foreach (VDT_DA_DuToan_ChiPhi_ViewModel itemCp in data.listChiPhi)
                        {
                            VDT_QT_QuyetToan_ChiTiet objChiTiet = new VDT_QT_QuyetToan_ChiTiet();
                            objChiTiet.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                            objChiTiet.iID_ChiPhiId = itemCp.iID_ChiPhiID;
                            objChiTiet.fGiaTriQuyetToan = itemCp.fGiaTriQuyetToan;
                            objChiTiet.fGiaTriThamTra = itemCp.fGiaTriThamTra;
                            objChiTiet.sUserCreate = Username;
                            objChiTiet.dDateCreate = DateTime.Now;
                            conn.Insert<VDT_QT_QuyetToan_ChiTiet>(objChiTiet, trans);
                        }
                    }
                    if (data.listHangMuc != null && data.listHangMuc.Count > 0)
                    {
                        foreach (VDT_DA_DuToan_HangMuc_ViewModel itemHm in data.listHangMuc)
                        {
                            VDT_QT_QuyetToan_ChiTiet objChiTiet = new VDT_QT_QuyetToan_ChiTiet();
                            objChiTiet.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                            objChiTiet.iID_HangMucId = itemHm.iID_HangMucID;
                            objChiTiet.fGiaTriQuyetToan = itemHm.fGiaTriQuyetToan;
                            objChiTiet.fGiaTriThamTra = itemHm.fGiaTriThamTra;
                            objChiTiet.sUserCreate = Username;
                            objChiTiet.dDateCreate = DateTime.Now;
                            conn.Insert<VDT_QT_QuyetToan_ChiTiet>(objChiTiet, trans);
                        }
                    }
                    #endregion

                    //#region Them moi VDT_QT_QuyetToan_Nguonvon
                    //if (data.listQuyetToanNguonVon != null && data.listQuyetToanNguonVon.Count() > 0)
                    //{
                    //    for (int i = 0; i < data.listQuyetToanNguonVon.Count(); i++)
                    //    {
                    //        var entityQTNguonVon = new VDT_QT_QuyetToan_Nguonvon();
                    //        entityQTNguonVon.MapFrom(data.listQuyetToanNguonVon.ToList()[i]);
                    //        entityQTNguonVon.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                    //        conn.Insert(entityQTNguonVon, trans);
                    //    }
                    //}
                    //#endregion

                    //#region Them moi VDT_QT_QuyetToan_NguonVon_ChenhLech
                    //if (data.listNguonVonChenhLech != null && data.listNguonVonChenhLech.Count() > 0)
                    //{
                    //    for (int i = 0; i < data.listNguonVonChenhLech.Count(); i++)
                    //    {
                    //        var entityQTNVCL = new VDT_QT_QuyetToan_NguonVon_ChenhLech();
                    //        entityQTNVCL.MapFrom(data.listNguonVonChenhLech.ToList()[i]);
                    //        entityQTNVCL.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                    //        entityQTNVCL.iID_NguonVonID = data.listNguonVonChenhLech.ToList()[i].iID_MaNguonNganSach;
                    //        entityQTNVCL.sTenNguonVonCL = data.listNguonVonChenhLech.ToList()[i].sTen;
                    //        entityQTNVCL.fTienChenhLech = data.listNguonVonChenhLech.ToList()[i].fGiaTriChenhLech;
                    //        conn.Insert(entityQTNVCL, trans);
                    //    }
                    //}
                    //#endregion
                }
                else
                {
                    #region Sua phe duyet quyet toan
                    var entityQuyetToan = conn.Get<VDT_QT_QuyetToan>(data.quyetToan.iID_QuyetToanID, trans);
                    entityQuyetToan.sSoQuyetDinh = data.quyetToan.sSoQuyetDinh;
                    entityQuyetToan.sCoQuanPheDuyet = data.quyetToan.sCoQuanPheDuyet;
                    entityQuyetToan.sNguoiKy = data.quyetToan.sNguoiKy;
                    entityQuyetToan.sNoiDung = data.quyetToan.sNoiDung;
                    entityQuyetToan.fTienQuyetToanPheDuyet = data.quyetToan.fTienQuyetToanPheDuyet;
                    entityQuyetToan.fChiPhiThietHai = data.quyetToan.fChiPhiThietHai;
                    entityQuyetToan.fChiPhiKhongTaoNenTaiSan = data.quyetToan.fChiPhiKhongTaoNenTaiSan;
                    entityQuyetToan.fTaiSanDaiHanThuocCDTQuanLy = data.quyetToan.fTaiSanDaiHanThuocCDTQuanLy;
                    entityQuyetToan.fTaiSanDaiHanDonViKhacQuanLy = data.quyetToan.fTaiSanDaiHanDonViKhacQuanLy;
                    entityQuyetToan.fTaiSanNganHanThuocCDTQuanLy = data.quyetToan.fTaiSanNganHanThuocCDTQuanLy;                    
                    entityQuyetToan.fTaiSanNganHanDonViKhacQuanLy = data.quyetToan.fTaiSanNganHanDonViKhacQuanLy;                    
                    entityQuyetToan.sUserUpdate = Username;
                    entityQuyetToan.dDateUpdate = DateTime.Now;
                    conn.Update(entityQuyetToan, trans);
                    #endregion

                    #region Sua VDT_QT_QuyetToan_ChiTiet                    
                    var lstQuyetToanChiTiet = conn.Query<VDT_QT_QuyetToan_ChiTiet>(string.Format("SELECT * FROM VDT_QT_QuyetToan_ChiTiet WHERE iID_QuyetToanID = '{0}'", data.quyetToan.iID_QuyetToanID), null, trans);
                    if(data.listChiPhi.Count > 0)
                    {
                        foreach(var chiPhi in data.listChiPhi)
                        {
                            foreach(var quyetToanChiTiet in lstQuyetToanChiTiet)
                            {
                                if(quyetToanChiTiet.iID_ChiPhiId == chiPhi.iID_ChiPhiID)
                                {
                                    quyetToanChiTiet.fGiaTriThamTra = chiPhi.fGiaTriThamTra;
                                    quyetToanChiTiet.fGiaTriQuyetToan = chiPhi.fGiaTriQuyetToan;
                                }
                                conn.Update<VDT_QT_QuyetToan_ChiTiet>(quyetToanChiTiet, trans);
                            }                            
                        }
                    }

                    if(data.listHangMuc.Count > 0)
                    {
                        foreach (var hangMuc in data.listHangMuc)
                        {
                            foreach (var quyetToanChiTiet in lstQuyetToanChiTiet)
                            {
                                if (quyetToanChiTiet.iID_HangMucId == hangMuc.iID_HangMucID)
                                {
                                    quyetToanChiTiet.fGiaTriThamTra = hangMuc.fGiaTriThamTra;
                                    quyetToanChiTiet.fGiaTriQuyetToan = hangMuc.fGiaTriQuyetToan;
                                }
                                conn.Update<VDT_QT_QuyetToan_ChiTiet>(quyetToanChiTiet, trans);
                            }
                        }
                    }
                    #endregion

                    //delete all ChiPhi, NguonVon, NguonVon_ChenhLech
                    _iQLVonDauTuService.deleteQTChiPhiQTNguonVonQTChenhLech(data.quyetToan.iID_QuyetToanID);

                    #region Them moi VDT_QT_QuyetToan_ChiPhi
                    if (data.listQuyetToanChiPhi != null && data.listQuyetToanChiPhi.Count() > 0)
                    {
                        for (int i = 0; i < data.listQuyetToanChiPhi.Count(); i++)
                        {
                            var entityQTChiPhi = new VDT_QT_QuyetToan_ChiPhi();
                            entityQTChiPhi.MapFrom(data.listQuyetToanChiPhi.ToList()[i]);
                            entityQTChiPhi.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                            conn.Insert(entityQTChiPhi, trans);
                        }
                    }
                    #endregion

                    #region Them moi VDT_QT_QuyetToan_Nguonvon
                    if (data.listQuyetToanNguonVon != null && data.listQuyetToanNguonVon.Count() > 0)
                    {
                        for (int i = 0; i < data.listQuyetToanNguonVon.Count(); i++)
                        {
                            var entityQTNguonVon = new VDT_QT_QuyetToan_Nguonvon();
                            entityQTNguonVon.MapFrom(data.listQuyetToanNguonVon.ToList()[i]);
                            entityQTNguonVon.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                            conn.Insert(entityQTNguonVon, trans);
                        }
                    }
                    #endregion

                    #region Them moi VDT_QT_QuyetToan_NguonVon_ChenhLech
                    if (data.listNguonVonChenhLech != null && data.listNguonVonChenhLech.Count() > 0)
                    {
                        for (int i = 0; i < data.listNguonVonChenhLech.Count(); i++)
                        {
                            var entityQTNVCL = new VDT_QT_QuyetToan_NguonVon_ChenhLech();
                            entityQTNVCL.MapFrom(data.listNguonVonChenhLech.ToList()[i]);
                            entityQTNVCL.iID_QuyetToanID = entityQuyetToan.iID_QuyetToanID;
                            entityQTNVCL.iID_NguonVonID = data.listNguonVonChenhLech.ToList()[i].iID_MaNguonNganSach;
                            entityQTNVCL.sTenNguonVonCL = data.listNguonVonChenhLech.ToList()[i].sTen;
                            entityQTNVCL.fTienChenhLech = data.listNguonVonChenhLech.ToList()[i].fGiaTriChenhLech;
                            conn.Insert(entityQTNVCL, trans);
                        }
                    }
                    #endregion
                }
                // commit to db
                trans.Commit();
            }

            return true;
        }

        [HttpPost]
        public bool VDTQTDADelete(Guid id)
        {
            if (!_iQLVonDauTuService.deleteQTChiPhiQTNguonVonQTChenhLech(id)) return false;
            if (!_iQLVonDauTuService.deleteVDTQTDA(id)) return false;               //xoa ban ghi Quyet toan va Quyet toan chi tiet
            return true;
        }

        public ActionResult ViewDetailQTDA(Guid? id)
        {
            VDTQLPheDuyetQuyetToanModel data = new VDTQLPheDuyetQuyetToanModel();
            data.quyetToan = new VDTQLPheDuyetQuyetToanViewModel();
            data.listQuyetToanChiPhi = new List<VDTChiPhiDauTuModel>();
            data.listQuyetToanNguonVon = new List<VDTNguonVonDauTuModel>();
            data.listNguonVonChenhLech = new List<VDTNguonVonDauTuViewModel>();
            if (id.HasValue)
            {
                //Lay thong tin phe duyet quyet toan
                data.quyetToan = _iQLVonDauTuService.GetVdtQuyetToanById(id);
                //Lay danh sach chi phi dau tu
                data.listQuyetToanChiPhi = _iQLVonDauTuService.GetLstChiPhiDauTu(id);
                //Lay danh sach nguon von dau tu
                data.listQuyetToanNguonVon = _iQLVonDauTuService.GetLstNguonVonDauTu(id);

            }
            ViewBag.isDetail = 1;
            return View(data);
        }

        public JsonResult LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(string iID_DonViQuanLyID, DateTime? dNgayQuyetDinh)
        {
            List<VDTQLPheDuyetQuyetToanViewModel> lstDuAn = _iQLVonDauTuService.LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhDNQT(Guid.Parse(iID_DonViQuanLyID), dNgayQuyetDinh).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' >{1}</option>", lstDuAn[i].iID_DuAnID, lstDuAn[i].sMaDuAn + " - " + lstDuAn[i].sTenDuAn);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LayThongTinDuAn(Guid iID_DonViQuanLyID, Guid iID_DuAnID, DateTime dNgayQuyetDinh)
        {
            VDTQLPheDuyetQuyetToanViewModel data = new VDTQLPheDuyetQuyetToanViewModel();
            data = _iQLVonDauTuService.GetThongTinDuAn(iID_DonViQuanLyID, iID_DuAnID, dNgayQuyetDinh);
            return Json(new { bIsComplete = (data == null ? false : true), data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public double GetGiaTriDuToan(Guid iID_DuAnID, Guid iID_ChiPhiID, DateTime dNgayQuyetDinh)
        {
            double fTongMucDauTu = _iQLVonDauTuService.GetGiaTriDuToan(iID_DuAnID, iID_ChiPhiID, dNgayQuyetDinh);
            return fTongMucDauTu;
        }

        [HttpPost]
        public double GetGiaTriDuToanNguonVon(Guid iID_DuAnID, int iID_MaNguonNganSach, DateTime dNgayQuyetDinh)
        {
            double fTongMucDauTu = _iQLVonDauTuService.GetGiaTriDuToanNguonVon(iID_DuAnID, iID_MaNguonNganSach, dNgayQuyetDinh);
            return fTongMucDauTu;
        }

        [HttpPost]
        public JsonResult GetNoiDungQuyetToan(VDTQLPheDuyetQuyetToanViewModel data)
        {
            VDTQLPheDuyetQuyetToanViewModel dataResult = new VDTQLPheDuyetQuyetToanViewModel();
            double fTongGiaTriPhanBo = _iQLVonDauTuService.GetTongGiaTriPhanBo(data.iID_DonViQuanLyID, data.iID_DuAnID, data.dNgayQuyetDinh);
            IEnumerable<VDTNguonVonDauTuViewModel> lstNoiDungQuyetToan = _iQLVonDauTuService.GetLstNoiDungQuyetToan(data.arrNguonVon, data.iID_DonViQuanLyID, data.iID_DuAnID, data.dNgayQuyetDinh, PhienLamViec.NamLamViec);

            dataResult.tongGiaTriPheDuyet = data.tongGiaTriPheDuyet;
            dataResult.fTongGiaTriPhanBo = fTongGiaTriPhanBo;
            dataResult.fTongGiaTriChenhLech = fTongGiaTriPhanBo - data.tongGiaTriPheDuyet;
            dataResult.lstNoiDungQuyetToan = lstNoiDungQuyetToan;
            return Json(new { bIsComplete = (dataResult == null ? false : true), data = dataResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetListDataNoiDungQuyetToan(IEnumerable<VDTNguonVonDauTuViewModel> lstNoiDungQuyetToan)
        {
            VDTQLPheDuyetQuyetToanModel data = new VDTQLPheDuyetQuyetToanModel();
            data.listNguonVonChenhLech = lstNoiDungQuyetToan;
            return PartialView("_partialListNoiDungQuyetToan", data);
        }

        [HttpPost]
        public JsonResult GetListChiPhiHangMucTheoDuAn(Guid? iID_DuToanID, Guid? iID_DeNghiQuyetToanID, Guid? iID_QuyetToanID)
        {
            List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            List<VDT_DA_DuToan_HangMuc_ViewModel> listHangMuc = new List<VDT_DA_DuToan_HangMuc_ViewModel>();
            if (iID_DuToanID != null && iID_DuToanID != Guid.Empty)
            {
                listChiPhi = _iQLVonDauTuService.GetListChiPhiTheoTKTC(iID_DuToanID.Value).ToList();
                listHangMuc = _iQLVonDauTuService.GetListHangMucTheoTKTC(iID_DuToanID.Value).ToList();
            }
            if (iID_DeNghiQuyetToanID != null && iID_DeNghiQuyetToanID != Guid.Empty)
            {
                List<VDT_QT_DeNghiQuyetToan_ChiTiet> lstDNQuyetToanChiTiet = _iQLVonDauTuService.GetDeNghiQuyetToanChiTiet(iID_DeNghiQuyetToanID.Value);
                if (lstDNQuyetToanChiTiet != null && lstDNQuyetToanChiTiet.Any())
                {
                    if (listChiPhi != null && listChiPhi.Any())
                    {
                        foreach (VDT_DA_DuToan_ChiPhi_ViewModel itemCp in listChiPhi)
                        {
                            VDT_QT_DeNghiQuyetToan_ChiTiet objDNQuyetToanChiTiet = lstDNQuyetToanChiTiet.Where(x => x.iID_ChiPhiId == itemCp.iID_DuAn_ChiPhi).FirstOrDefault();
                            if (objDNQuyetToanChiTiet != null)
                            {
                                itemCp.fGiaTriDeNghiQuyetToan = objDNQuyetToanChiTiet.fGiaTriDeNghiQuyetToan;
                            }
                        }
                    }

                    if (listHangMuc != null && listHangMuc.Any())
                    {
                        foreach (VDT_DA_DuToan_HangMuc_ViewModel itemHm in listHangMuc)
                        {
                            VDT_QT_DeNghiQuyetToan_ChiTiet objDNQuyetToanChiTiet = lstDNQuyetToanChiTiet.Where(x => x.iID_HangMucId == itemHm.iID_HangMucID).FirstOrDefault();
                            if (objDNQuyetToanChiTiet != null)
                            {
                                itemHm.fGiaTriDeNghiQuyetToan = objDNQuyetToanChiTiet.fGiaTriDeNghiQuyetToan;
                            }
                        }
                    }
                }
            }
            if(iID_QuyetToanID != null && iID_QuyetToanID != Guid.Empty)
            {
                List<VDT_QT_QuyetToan_ChiTiet> lstQuyetToanChiTiet = _iQLVonDauTuService.GetQuyetToanChiTiet(iID_QuyetToanID.Value);
                if(lstQuyetToanChiTiet != null && lstQuyetToanChiTiet.Any())
                {
                    if (listChiPhi != null && listChiPhi.Any())
                    {
                        foreach (VDT_DA_DuToan_ChiPhi_ViewModel itemCp in listChiPhi)
                        {
                            VDT_QT_QuyetToan_ChiTiet objQuyetToanChiTiet = lstQuyetToanChiTiet.Where(x => x.iID_ChiPhiId == itemCp.iID_ChiPhiID).FirstOrDefault();
                            if (objQuyetToanChiTiet != null)
                            {
                                itemCp.fGiaTriQuyetToan = objQuyetToanChiTiet.fGiaTriQuyetToan;
                                itemCp.fGiaTriThamTra = objQuyetToanChiTiet.fGiaTriThamTra;
                            }
                        }
                    }
                    if (listHangMuc != null && listHangMuc.Any())
                    {
                        foreach (VDT_DA_DuToan_HangMuc_ViewModel itemHm in listHangMuc)
                        {
                            VDT_QT_QuyetToan_ChiTiet objQuyetToanChiTiet = lstQuyetToanChiTiet.Where(x => x.iID_HangMucId == itemHm.iID_HangMucID).FirstOrDefault();
                            if (objQuyetToanChiTiet != null)
                            {
                                itemHm.fGiaTriQuyetToan = objQuyetToanChiTiet.fGiaTriQuyetToan;
                                itemHm.fGiaTriThamTra = objQuyetToanChiTiet.fGiaTriThamTra;
                            }
                        }
                    }
                }
            }

            var sumGiaTriThamTra = listChiPhi.Sum(x => x.fGiaTriThamTra).HasValue ? listChiPhi.Sum(x => x.fGiaTriThamTra).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            var sumGiaTriQuyetToan = listChiPhi.Sum(x => x.fGiaTriQuyetToan).HasValue ? listChiPhi.Sum(x => x.fGiaTriQuyetToan).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";

            return Json(new { lstChiPhi = listChiPhi, lstHangMuc = listHangMuc, sumGiaTriThamTra = sumGiaTriThamTra, sumGiaTriQuyetToan = sumGiaTriQuyetToan }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ExportQuyetToan(Guid? iID_QuyetToanID)
        {
            try
            {
                List<VDTQLPheDuyetQuyetToanViewModel> lstData = _iQLVonDauTuService.GetDataExportQuyetToan(iID_QuyetToanID).ToList();
                List<VDTNguonVonDauTuViewModel> lstNguonVonData = _iQLVonDauTuService.GetDataChiPhiExportQuyetToan(iID_QuyetToanID).ToList();

                ExcelFile xls = CreateReportExport(lstData, lstNguonVonData, iID_QuyetToanID);
                xls.PrintLandscape = true;

                TempData["DataExport"] = xls;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public ExcelFile CreateReportExport(List<VDTQLPheDuyetQuyetToanViewModel> lstData, List<VDTNguonVonDauTuViewModel> lstNguonVonData, Guid? iID_QuyetToanID)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/QuyetToan/eptVDT_TongHopPheDuyetQuyetToan.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            int count = 1;
            foreach(var item in lstData)
            {
                item.SttExport = count;
                count++;
            }

            List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            listChiPhi = _iQLVonDauTuService.GetListChiPhiTheoTKTC(lstData.FirstOrDefault().iID_DuToanID.Value).ToList();
            List<VDT_QT_QuyetToan_ChiTiet> lstQuyetToanChiTiet = _iQLVonDauTuService.GetQuyetToanChiTiet(iID_QuyetToanID.Value);
            int count2 = 1;
            if (lstQuyetToanChiTiet != null && lstQuyetToanChiTiet.Any())
            {
                foreach (VDT_DA_DuToan_ChiPhi_ViewModel itemCp in listChiPhi)
                {
                    VDT_QT_QuyetToan_ChiTiet objQuyetToanChiTiet = lstQuyetToanChiTiet.Where(x => x.iID_ChiPhiId == itemCp.iID_ChiPhiID).FirstOrDefault();
                    if (objQuyetToanChiTiet != null)
                    {
                        itemCp.sTenDuAn = lstData.FirstOrDefault().sTenDuAn;
                        itemCp.sMaDuAn = lstData.FirstOrDefault().sMaDuAn;
                        itemCp.iThuTu = count2;
                        itemCp.fGiaTriQuyetToan = objQuyetToanChiTiet.fGiaTriQuyetToan;
                        itemCp.fGiaTriThamTra = objQuyetToanChiTiet.fGiaTriThamTra;
                        count2++;
                    }
                }
            }            

            fr.AddTable("ChungTu", lstData);
            fr.AddTable("NguonVon", lstNguonVonData);            
            fr.AddTable("ChiPhi", listChiPhi);

            fr.Run(Result);
            return Result;
        }
        public FileContentResult ExportExcelQuyetToan()
        {
            string sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string sFileName = "PheDuyetQuyetToan.xlsx";
            ExcelFile xls = (ExcelFile)TempData["DataExport"];
            xls.PrintLandscape = true;
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return File(stream.ToArray(), sContentType, sFileName);
            }
        }
    }
}