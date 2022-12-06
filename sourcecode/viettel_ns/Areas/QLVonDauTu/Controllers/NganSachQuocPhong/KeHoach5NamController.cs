using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class KeHoach5NamController : AppController
    {
        INganSachService _iNganSachService = NganSachService.Default;
        private readonly INganSachNewService _nsService = NganSachNewService.Default;
        private readonly IQLVonDauTuService _vonDauTuService = QLVonDauTuService.Default;
        private QLKeHoachVonNamModel _modelVonNam = new QLKeHoachVonNamModel();
        // GET: QLVonDauTu/KeHoach5Nam
        public ActionResult Index()
        {
            KeHoach5NamModel vm = new KeHoach5NamModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _vonDauTuService.LayDanhSachKeHoach5Nam(ref vm._paging);
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.TAT_CA });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sTen");
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, int? iGiaiDoanTu, int? iGiaiDoanDen, string sMaDonVi)
        {
            KeHoach5NamModel vm = new KeHoach5NamModel();
            vm._paging = _paging;
            vm.Items = _vonDauTuService.LayDanhSachKeHoach5Nam(ref vm._paging, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iGiaiDoanTu, iGiaiDoanDen, sMaDonVi);
            List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = Constants.TAT_CA });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_MaDonVi", "sTen");
            return PartialView("_list", vm);
        }

        public ActionResult Add()
        {
            var listDonvi = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            ViewBag.ListDonViQuanLy = listDonvi.ToSelectList("iID_Ma", "sTen");
            ViewBag.ListNguonVon = _vonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
            //ViewBag.ListLoaiNganSach = _modelVonNam.GetDataDropdownLoaiNganSach(PhienLamViec.NamLamViec);
            ViewBag.ListDuAn = _vonDauTuService.GetVDTDADuAn().ToSelectList("iID_DuAnID", "sTenDuAn");
            ViewBag.NamLamViec = PhienLamViec.NamLamViec;
            return View();
        }

        [HttpPost]
        public JsonResult AddKeHoach5Nam(VDT_KHV_KeHoach5Nam data)
        {
            data.bActive = true;
            data.bIsGoc = true;
            data.dDateCreate = DateTime.Now;
            data.sUserCreate = Username;
            //tạo tạm để insert
            data.sLoaiDieuChinh = "1";

            var result = _vonDauTuService.InsertKeHoach5Nam(ref data);

            if (result)
            {
                return Json(new { data = data.iID_KeHoach5NamID, bIsComplete = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddKeHoach5NamChiTiet(List<VDT_KHV_KeHoach5Nam_ChiTiet> lstData)
        {
            return Json(new { bIsComplete = _vonDauTuService.InsertKeHoach5NamChiTiet(ref lstData) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Guid id)
        {
            var data = _vonDauTuService.Get_KeHoach5Nam(id);
            ViewBag.ListDuAn = _vonDauTuService.GetVDTDADuAn().ToSelectList("iID_DuAnID", "sTenDuAn");
            ViewBag.NamLamViec = PhienLamViec.NamLamViec;
            return View(data);
        }


        public JsonResult EditKeHoach5Nam(KeHoach5NamViewModel data)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                #region Sua Kehoach5nam
                var entity = conn.Get<VDT_KHV_KeHoach5Nam>(data.iID_KeHoach5NamID, trans);
                entity.sSoQuyetDinh = data.sSoQuyetDinh;
                entity.fGiaTriDuocDuyet = data.fGiaTriDuocDuyet;
                entity.sUserUpdate = Username;
                entity.dDateUpdate = DateTime.Now;
                conn.Update(entity, trans);
                #endregion

                #region Them moi VDT_KHV_KeHoachVonUng_ChiTiet
                //delete all KHChiTiet
                _vonDauTuService.DeleteKeHoachChiTiet(data.iID_KeHoach5NamID);
                //insert new
                if (data.listChiTiet != null && data.listChiTiet.Count() > 0)
                {
                    for (int i = 0; i < data.listChiTiet.Count(); i++)
                    {
                        var entityChiTiet = new VDT_KHV_KeHoach5Nam_ChiTiet();
                        entityChiTiet.MapFrom(data.listChiTiet.ToList()[i]);
                        entityChiTiet.iID_KeHoach5NamID = data.iID_KeHoach5NamID;
                        conn.Insert(entityChiTiet, trans);
                    }
                }
                #endregion

                // commit to db
                trans.Commit();
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetThongTinLoaiKhoan(string sLNS)
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            DataTable dt = _nsService.GetDataDropdownLoaiAndKhoanByNganSach(sLNS, PhienLamViec.NamLamViec);
            foreach (DataRow dr in dt.Rows)
            {
                lstData.Add(new SelectListItem()
                {
                    Text = string.Format("Loại {0} - Khoản {1}", Convert.ToString(dr["sL"]), Convert.ToString(dr["sK"])),
                    //Value = string.Format(Convert.ToString(dr["iID_MaMucLucNganSach"]))
                    Value = ""
                });
            }
            return Json(new { data = lstData, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DieuChinh(Guid id)
        {
            var data = _vonDauTuService.Get_KeHoach5Nam(id);
            ViewBag.ListDuAn = _vonDauTuService.ListDuAnTheoDonViQuanLy(data.iID_DonViQuanLyID).ToSelectList("iID_DuAnID", "sTenDuAn");
            ViewBag.NamLamViec = PhienLamViec.NamLamViec;
            return View(data);
        }


        [HttpPost]
        public JsonResult GetListKhChiTietToGridView(Guid id, int iGiaiDoanTu,int iGiaiDoanDen, DateTime dNgayLap)
        {

            List<KeHoach5NamChiTietViewModel> lstData = _vonDauTuService.ListKeHoach5NamChiTiet(id, iGiaiDoanTu,iGiaiDoanDen, dNgayLap).ToList();

            return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDieuChinh(KeHoach5NamViewModel data)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                #region Sua Kehoach5nam cua parrent
                var entity = conn.Get<VDT_KHV_KeHoach5Nam>(data.iID_KeHoach5NamID, trans);
                entity.bActive = false;
                entity.sUserUpdate = Username;
                entity.dDateUpdate = DateTime.Now;
                conn.Update(entity, trans);
                #endregion
                VDT_KHV_KeHoach5Nam objNew = new VDT_KHV_KeHoach5Nam();
                objNew.sSoQuyetDinh = data.sSoQuyetDinh;
                objNew.dNgayQuyetDinh = data.dNgayQuyetDinh;
                objNew.fGiaTriDuocDuyet = data.fGiaTriDuocDuyet;
                objNew.iID_DonViQuanLyID = entity.iID_DonViQuanLyID;
                objNew.iGiaiDoanTu = entity.iGiaiDoanTu;
                objNew.iGiaiDoanDen = entity.iGiaiDoanDen;
                objNew.iID_ParentId = entity.iID_KeHoach5NamID;
                //objNew.bIsGoc = false;
                objNew.bActive = true;
                objNew.iID_NguonVonID = entity.iID_NguonVonID;
                objNew.iID_LoaiNguonVonID = entity.iID_LoaiNguonVonID;
                objNew.iID_LoaiNganSachID = entity.iID_LoaiNganSachID;
                objNew.iID_KhoanNganSachID = entity.iID_KhoanNganSachID;
                objNew.dDateCreate = DateTime.Now;
                objNew.sUserCreate = Username;
                objNew.sLoaiDieuChinh = "1";
                conn.Insert(objNew, trans);
                #region them moi ban ghi ke hoach 5 nam

                #endregion

                #region Them moi VDT_KHV_KeHoachVonUng_ChiTiet
                //delete all KHChiTiet
                // _vonDauTuService.DeleteKeHoachChiTiet(data.iID_KeHoach5NamID);
                //insert new
                if (data.listChiTiet != null && data.listChiTiet.Count() > 0)
                {
                    for (int i = 0; i < data.listChiTiet.Count(); i++)
                    {
                        var entityChiTiet = new VDT_KHV_KeHoach5Nam_ChiTiet();
                        entityChiTiet.MapFrom(data.listChiTiet.ToList()[i]);
                        entityChiTiet.iID_KeHoach5NamID = objNew.iID_KeHoach5NamID;
                        entityChiTiet.sTrangThai = "2";
                        conn.Insert(entityChiTiet, trans);
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
            var data = _vonDauTuService.Get_KeHoach5Nam(id);
            return View(data);
        }

        [HttpPost]
        public bool Delete(Guid id)
        {
            if (!_vonDauTuService.deleteKH5Nam(id, Username)) 
                return false;
            return true;
        }

        public JsonResult ListDuAnTheoDonViQLAndNgayLap(string iID_DonViQuanLyID, DateTime dNgayLap)
        {
            List<VDT_DA_DuAn> lstDuAn = _vonDauTuService.ListDuAnTheoDonViQLAndNgayLap(Guid.Parse(iID_DonViQuanLyID), dNgayLap).ToList();
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

        [HttpPost]
        public JsonResult ListLoaiNganSach(int giaiDoanTu, int giaiDoanDen)
        {
            List<int> lstNamLamViec = new List<int>();
            for (int i = 0; i <= (giaiDoanDen - giaiDoanTu); i++)
            {
                lstNamLamViec.Add(giaiDoanTu + i);
            }
            List<NS_MucLucNganSach> listMucLucNS = _nsService.GetDataDropdownMucLucNganSach(lstNamLamViec).ToList();
            StringBuilder htmlString = new StringBuilder();
            if (listMucLucNS != null && listMucLucNS.Count > 0)
            {
                for (int i = 0; i < listMucLucNS.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' data-sL='{1}'>{2}</option>", listMucLucNS[i].iID_MaMucLucNganSach, listMucLucNS[i].sLNS, listMucLucNS[i].sMoTa);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicate(Guid iDDonVi, int giaiDoanTu, int giaiDoanDen, string soKeHoach)
        {
            byte error;
            var result = _vonDauTuService.CheckDuplicate(iDDonVi, giaiDoanTu, giaiDoanDen, soKeHoach, out error);
            return Json(new { result, error }, JsonRequestBehavior.AllowGet);
        }
    }
}