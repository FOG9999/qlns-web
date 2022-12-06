using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VIETTEL.Areas.QLVonDauTu.Model.QuyetToan;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using System.Data;
using Viettel.Models.QLVonDauTu;
using VIETTEL.Models;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using VIETTEL.Common;
using VIETTEL.Helpers;
using System.Web;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class TraoDoiDuLieuController : AppController
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;
        // GET: QLVonDauTu/VDT_QT_TraoDoiDuLieu
        public ActionResult Index()
        {
            string sMaNguoiDung = Username;

            List<SelectListItem> lstDataDonVi = new List<SelectListItem>();
            DataTable dt = NganSach_HamChungModels.DSDonViCuaNguoiDung(sMaNguoiDung);
            foreach (DataRow dr in dt.Rows)
            {
                lstDataDonVi.Add(new SelectListItem() { Text = Convert.ToString(dr["TenHT"]), Value = Convert.ToString(dr["iID_Ma"]) });
            }
            List<SelectListItem> lstLoaiTraoDoi = new List<SelectListItem>() {
            new SelectListItem{Text = "Tất cả", Value = ""},
            new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.DU_TOAN, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.DU_TOAN)},
            new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.QUYET_TOAN, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.QUYET_TOAN)},
            new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.THONG_TRI, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.THONG_TRI) }
            };

            ViewBag.drpLoaiTraoDoi = lstLoaiTraoDoi;
            List<SelectListItem> lstTrangThai = new List<SelectListItem>() {
            new SelectListItem{Text = "Tất cả", Value = ""},
            new SelectListItem{Text = Constants.TrangThai.TypeName.TAO_MOI, Value = Convert.ToString((int) Constants.TrangThai.Type.TAO_MOI)},
            new SelectListItem{Text = Constants.TrangThai.TypeName.DA_CHUYEN, Value = Convert.ToString((int) Constants.TrangThai.Type.DA_CHUYEN)},
            new SelectListItem{Text = Constants.TrangThai.TypeName.DANG_XU_LY, Value = Convert.ToString((int) Constants.TrangThai.Type.DANG_XU_LY) },
            new SelectListItem{Text = Constants.TrangThai.TypeName.HOAN_THANH, Value = Convert.ToString((int) Constants.TrangThai.Type.HOAN_THANH) }
            };

            ViewBag.drpTrangThai = lstTrangThai;

            lstDataDonVi.Insert(0, new SelectListItem { Text = "", Value = "" });
            ViewBag.drpDonViQuanLy = lstDataDonVi;


            VDT_NS_TraoDoiDuLieuPagingInfo data = new VDT_NS_TraoDoiDuLieuPagingInfo();
            data._paging.CurrentPage = 1;
            data.lstData = _vdtService.GetAllTraoDoiDuLieuPaging(ref data._paging);

            return View(data);
        }


        [HttpPost]
        public ActionResult GetListView(PagingInfo _paging, string sSoChungTu, int? iNamKeHoach, DateTime? dNgayChungtu, string iIDMaDonVi, int? sLoaiTraoDoi, int? sTrangThai)
        {
            VDT_NS_TraoDoiDuLieuPagingInfo data = new VDT_NS_TraoDoiDuLieuPagingInfo();
            var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, iIDMaDonVi).iID_MaDonVi;
            data._paging = _paging;
            data.lstData = _vdtService.GetAllTraoDoiDuLieuPaging(ref data._paging, sSoChungTu, iNamKeHoach, dNgayChungtu, sMaDonViQuanLy, sLoaiTraoDoi, sTrangThai).OrderBy(x => x.dNgayTao);
            string sMaNguoiDung = Username;

            List<SelectListItem> lstDataDonVi = new List<SelectListItem>();
            DataTable dt = NganSach_HamChungModels.DSDonViCuaNguoiDung(sMaNguoiDung);
            foreach (DataRow dr in dt.Rows)
            {
                lstDataDonVi.Add(new SelectListItem() { Text = Convert.ToString(dr["TenHT"]), Value = Convert.ToString(dr["iID_Ma"]) });
            }
            List<SelectListItem> lstLoaiTraoDoi = new List<SelectListItem>() {
            new SelectListItem{Text = "Tất cả", Value = ""},
            new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.DU_TOAN, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.DU_TOAN)},
            new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.QUYET_TOAN, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.QUYET_TOAN)},
            new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.THONG_TRI, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.THONG_TRI) }
            };
            ViewBag.drpLoaiTraoDoi = lstLoaiTraoDoi;

            List<SelectListItem> lstTrangThai = new List<SelectListItem>() {
            new SelectListItem{Text = "Tất cả", Value = ""},
            new SelectListItem{Text = Constants.TrangThai.TypeName.TAO_MOI, Value = Convert.ToString((int) Constants.TrangThai.Type.TAO_MOI)},
            new SelectListItem{Text = Constants.TrangThai.TypeName.DA_CHUYEN, Value = Convert.ToString((int) Constants.TrangThai.Type.DA_CHUYEN)},
            new SelectListItem{Text = Constants.TrangThai.TypeName.DANG_XU_LY, Value = Convert.ToString((int) Constants.TrangThai.Type.DANG_XU_LY) },
            new SelectListItem{Text = Constants.TrangThai.TypeName.HOAN_THANH, Value = Convert.ToString((int) Constants.TrangThai.Type.HOAN_THANH) }
            };
            ViewBag.drpTrangThai = lstTrangThai;

            lstDataDonVi.Insert(0, new SelectListItem { Text = "", Value = "" });
            ViewBag.drpDonViQuanLy = lstDataDonVi;

            return PartialView("_list", data);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {

            VDT_NS_TraoDoiDuLieuViewModel data = new VDT_NS_TraoDoiDuLieuViewModel();
            try
            {
                if (id.HasValue)
                {
                    // data = _vdtService.GetKeHoach5NamDuocDuyetById(id);

                }
                else
                {
                    data.dNgayChungTu = DateTime.Now;
                }
                GetALLDrpTraoDoiDuLieu();

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);

            }

            return PartialView("_modalUpdate", data);
        }

        public void GetALLDrpTraoDoiDuLieu()
        {
            string sMaNguoiDung = Username;

            List<SelectListItem> lstDataDonVi = new List<SelectListItem>();
            DataTable dt = NganSach_HamChungModels.DSDonViCuaNguoiDung(sMaNguoiDung);
            foreach (DataRow dr in dt.Rows)
            {
                lstDataDonVi.Add(new SelectListItem() { Text = Convert.ToString(dr["TenHT"]), Value = Convert.ToString(dr["iID_Ma"]) });
            }
            List<SelectListItem> lstLoaiTraoDoi = new List<SelectListItem>() {
                new SelectListItem{Text = "Tất cả", Value = ""},
                new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.DU_TOAN, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.DU_TOAN)},
                new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.QUYET_TOAN, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.QUYET_TOAN)},
                new SelectListItem{Text = Constants.LoaiTraoDoi.TypeName.THONG_TRI, Value = Convert.ToString((int) Constants.LoaiTraoDoi.Type.THONG_TRI) }
                };
            List<SelectListItem> lstQuy = new List<SelectListItem> {
                new SelectListItem{Text = "Tất cả", Value = "0"},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_1, Value=((int)Constants.LoaiQuy.Type.QUY_1).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_2, Value=((int)Constants.LoaiQuy.Type.QUY_2).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_3, Value=((int)Constants.LoaiQuy.Type.QUY_3).ToString()},
                new SelectListItem{Text = Constants.LoaiQuy.TypeName.QUY_4, Value=((int)Constants.LoaiQuy.Type.QUY_4).ToString()},
                };
            List<SelectListItem> lstLoaiThongTri = new List<SelectListItem> {
                new SelectListItem{Text = "", Value = ""},
                new SelectListItem{Text = Constants.LoaiThongTri.TypeName.TAM_UNG, Value=((int)Constants.LoaiThongTri.Type.TAM_UNG).ToString()},
                new SelectListItem{Text = Constants.LoaiThongTri.TypeName.THANH_TOAN, Value=((int)Constants.LoaiThongTri.Type.THANH_TOAN).ToString()},
                new SelectListItem{Text = Constants.LoaiThongTri.TypeName.CAP_HOP_THUC, Value=((int)Constants.LoaiThongTri.Type.CAP_HOP_THUC).ToString()},
                new SelectListItem{Text = Constants.LoaiThongTri.TypeName.CAP_KINH_DOANH, Value=((int)Constants.LoaiThongTri.Type.CAP_KINH_DOANH).ToString()},
                };
            List<SelectListItem> lstLoaiDuToan = new List<SelectListItem> {
                new SelectListItem{Text = "", Value = ""},
                new SelectListItem{Text = Constants.LoaiDuToan.TypeName.DAU_NAM, Value=((int)Constants.LoaiDuToan.Type.DAU_NAM).ToString()},
                new SelectListItem{Text = Constants.LoaiDuToan.TypeName.BO_SUNG, Value=((int)Constants.LoaiDuToan.Type.BO_SUNG).ToString()},
                new SelectListItem{Text = Constants.LoaiDuToan.TypeName.NAM_TRUOC_CHUYEN_SANG, Value=((int)Constants.LoaiDuToan.Type.NAM_TRUOC_CHUYEN_SANG).ToString()},
                };


            List<NS_NguonNganSach> lstNguonVon = _vdtService.LayNguonVon().ToList();
            //lstNguonVon.Insert(0, new NS_NguonNganSach { iID_MaNguonNganSach = 0, sTen = "" });
            ViewBag.drpNguonNganSach = lstNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            ViewBag.drpQuy = lstQuy.ToSelectList("Value", "Text");
            ViewBag.drpLoaiTraoDoi = lstLoaiTraoDoi.ToSelectList("Value", "Text");
            ViewBag.drpLoaiThongTri = lstLoaiThongTri.ToSelectList("Value", "Text");
            ViewBag.drpLoaiDuToan = lstLoaiDuToan.ToSelectList("Value", "Text");
            lstDataDonVi.Insert(0, new SelectListItem { Text = "", Value = "" });
            ViewBag.drpDonViQuanLy = lstDataDonVi;

        }

        [HttpPost]
        public JsonResult TraoDoiDuLieuSave(VDT_NS_TraoDoiDuLieuViewModel data, bool isUpdate, List<VDT_NS_TraoDoiDuLieu_ChiTiet> lstDataChiTiet)
        {
            try
            {
                List<VDT_NS_TraoDoiDuLieu_ChiTiet> lstDetails = new List<VDT_NS_TraoDoiDuLieu_ChiTiet>();
                var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, data.iID_DonViQuanLyID.ToString()).iID_MaDonVi;
                data.iID_MaDonVi = sMaDonViQuanLy;

                if (data.ID == new Guid())
                {
                    if (_vdtService.CheckExistSoChungTuTraoDoiDL(data.sSoChungTu, PhienLamViec.NamLamViec, null))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số chứng từ đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (!_vdtService.SaveTraoDoiDuDLieu(ref data, Username, PhienLamViec.NamLamViec, isUpdate, lstDataChiTiet))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không lưu được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    if (_vdtService.CheckExistSoChungTuTraoDoiDL(data.sSoChungTu, PhienLamViec.NamLamViec, data.ID))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số chứng từ đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (!_vdtService.SaveTraoDoiDuDLieu(ref data, Username, PhienLamViec.NamLamViec, isUpdate, lstDataChiTiet))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = true, iID_TraoDoiDuLieuId = data.ID }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetdataTraoDoiDuLieuChiTiet(Guid? id, bool? isDetail)
        {
            List<VDT_NS_TraoDoiDuLieu_ChiTiet> lstdata = new List<VDT_NS_TraoDoiDuLieu_ChiTiet>();
            var lstMucLucNganSach = _vdtService.GetAlTraoDoiDuLieuChiTiet(PhienLamViec.NamLamViec).ToList();
            if (id == null || id == Guid.Empty)
            {

                if (lstMucLucNganSach.Any())
                {
                    foreach (var item in lstMucLucNganSach)
                    {
                        VDT_NS_TraoDoiDuLieu_ChiTiet entity = new VDT_NS_TraoDoiDuLieu_ChiTiet();
                        entity.MapFrom(item);
                        lstdata.Add(entity);
                    }
                }

            }
            else
            {
                lstdata = _vdtService.GetAlTraoDoiDuLieuChiTietById(id).ToList();
                if (isDetail != null && isDetail == true)
                {
                    ViewBag.isDetail = 1;
                    return Json(this.RenderRazorViewToString("_list_traodoidulieu_chitiet", lstdata), JsonRequestBehavior.AllowGet);
                }
                else
                {                  
                    foreach (var item in lstdata)
                    {
                        lstMucLucNganSach.RemoveAll(x => x.iID_MaMucLucNganSach == item.iID_MaMucLucNganSach);
                    }

                    if (lstMucLucNganSach.Any())
                    {
                        foreach (var item in lstMucLucNganSach)
                        {
                            VDT_NS_TraoDoiDuLieu_ChiTiet entity = new VDT_NS_TraoDoiDuLieu_ChiTiet();
                            entity.MapFrom(item);
                            lstdata.Add(entity);
                        }
                    }
                }
               
            }

            return Json(this.RenderRazorViewToString("_list_traodoidulieu_chitiet", lstdata), JsonRequestBehavior.AllowGet);

        }

        public ActionResult Insert(Guid? id)
        {

            VDT_NS_TraoDoiDuLieuViewModel vdtTraoDoiDuLieuViewModel = new VDT_NS_TraoDoiDuLieuViewModel();
            GetALLDrpTraoDoiDuLieu();
            if (id != null)
            {
                VDT_NS_Traodoidulieu entity = new VDT_NS_Traodoidulieu();
                entity = _vdtService.GetVDTraoDoiDuLieuByID(id);
                vdtTraoDoiDuLieuViewModel.MapFrom(entity);
            }
            return View(vdtTraoDoiDuLieuViewModel);
        }

        [HttpPost]
        public bool XoaTraoDoiDuLieu(Guid id)
        {
            if (!_vdtService.XoaTraoDoiDuLieu(id)) return false;
            return true;
        }

        public ActionResult Detail(Guid id)
        {
            var data = _vdtService.GetDetailVDTraoDoiDuLieuByID(id);
            ViewBag.isDetail = 1;

            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }

        [HttpPost]
        public JsonResult ChuyentrangThai(List<Guid> listId)
        {
            try
            {
                List<VDT_NS_Traodoidulieu> lstData= _vdtService.GetListVDTraoDoiDuLieuByIds(listId,PhienLamViec.NamLamViec).ToList();

                //List<VDT_NS_Traodoidulieu> lss = new List<VDT_NS_Traodoidulieu>();
                //// var sMaDonViQuanLy = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, data.iID_DonViQuanLyID.ToString()).iID_MaDonVi;
                //Dictionary<string, List<VDT_NS_Traodoidulieu>> dicData = new Dictionary<string, List<VDT_NS_Traodoidulieu>>();
                //foreach (var item in lstData)
                //{
                //    string sKey = string.Format("{0}\t{1}", item.iID_MaDonVi, item.iLoaiDuToan);
                //    if (!dicData.ContainsKey(sKey))
                //        dicData.Add(sKey, new List<VDT_NS_Traodoidulieu>());
                //    dicData[sKey].Add(item);
                //}
                //var a = lstData.GroupBy(x => new { x.iNamLamViec, x.iID_MaDonVi, x.iLoaiTraoDoi }).Count();

                if (lstData.Count() <= 0)
                {
                    return Json(new { bIsComplete = true, sMessError = "Vui lòng chọn chứng từ!" }, JsonRequestBehavior.AllowGet);
                }

                if (lstData.GroupBy(x=> new {x.iNamLamViec}).Count() >1)
                {
                    return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ cùng năm !" }, JsonRequestBehavior.AllowGet);
                }
                
                if (lstData.GroupBy(x=> new {x.iLoaiTraoDoi}).Count() >1)
                {
                    return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ  cùng loại trao đổi !" }, JsonRequestBehavior.AllowGet);
                }
                
                if (lstData.GroupBy(x=> new {x.iID_MaDonVi}).Count() >1)
                {
                    return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ cùng loại đơn vị!" }, JsonRequestBehavior.AllowGet);
                }

                if(lstData.Count() > 1)
                {
                    if (lstData.Where(x => x.iTrangThai != 1).Count() > 1)
                    {
                        return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có trạng thái là tạo mới!" }, JsonRequestBehavior.AllowGet);
                    }
                }else
                {
                    if (lstData.Where(x => x.iTrangThai != 1).Count() == 1)
                    {
                        return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có trạng thái là tạo mới!" }, JsonRequestBehavior.AllowGet);
                    }

                }

                if (!_vdtService.ChuyenTrangThaiLstTraoDoiDuLieu(lstData, PhienLamViec.NamLamViec))
                {
                    return Json(new { bIsComplete = false, sMessError = "Thực hiện chuyển dữ liệu thất bại!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { bIsComplete = false, sMessError = "Thực hiện chuyển dữ liệu thất bại!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true, sMessError = "Thực hiện chuyển dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
        }

    }

}
