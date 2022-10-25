using DapperExtensions;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class KeHoachTrungHanDuocDuyetController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        IDanhMucService _danhMucService = DanhMucService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;

        public const string NGHIN_DONG = "Nghìn đồng";
        public const string DONG = "Đồng";
        public const string NGHIN_DONG_VALUE = "1000";
        public const string DONG_VALUE = "1";
        public const string TRIEU_DONG = "Triệu đồng";
        public const string TRIEU_VALUE = "1000000";
        public const string TY_DONG = "Tỷ đồng";
        public const string TY_VALUE = "1000000000";
        public string pathString = System.Configuration.ConfigurationManager.AppSettings["FtpPath"];
        public string ftpUsername = System.Configuration.ConfigurationManager.AppSettings["FtpUsername"];
        public string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        private const string sControlName = "KeHoachTrungHanDuocDuyet";

        List<KeHoach5NamChiTietDuocDuyetTempForSave> listTemp = new List<KeHoach5NamChiTietDuocDuyetTempForSave>();

        // GET: QLVonDauTu/KeHoachTrungHanDuocDuyet
        public ActionResult Index()
        {
            KeHoach5NamDuocDuyetViewModel vm = new KeHoach5NamDuocDuyetViewModel();
            try
            {
                // clear tempdata
                if (TempData["listNewKHV5NamChiTietNew"] != null)
                {
                    TempData["listNewKHV5NamChiTietNew"] = null;
                }
                if (TempData["currentListKHV5NamChiTiet"] != null)
                {
                    TempData["currentListKHV5NamChiTiet"] = null;
                }

                vm._paging.CurrentPage = 1;
                vm.Items = _iQLVonDauTuService.GetAllKeHoach5NamDuocDuyet(ref vm._paging, PhienLamViec.NamLamViec, "", null, null, null, "", null, null, 0);

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = "--Tất cả--" });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");
                List<KeHoach5NamDeXuatModel> lstVoucherTypes = new List<KeHoach5NamDeXuatModel>()
            {
                new KeHoach5NamDeXuatModel(){SVoucherTypes = "Tất cả", iLoai = 0}
            };
                lstVoucherTypes.AddRange(CreateVoucherTypes());
                ViewBag.ListVoucherTypes = lstVoucherTypes.ToSelectList("iLoai", "SVoucherTypes");
            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            return View(vm);
        }

        [HttpPost]
        public ActionResult KeHoach5NamDuocDuyetListView(PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? iID_DonViQuanLyID, string sMoTaChiTiet, int? iGiaiDoanTu, int? iGiaiDoanDen, int iLoai)
        {
            KeHoach5NamDuocDuyetViewModel vm = new KeHoach5NamDuocDuyetViewModel();

            try
            {
                vm._paging = _paging;
                vm.Items = _iQLVonDauTuService.GetAllKeHoach5NamDuocDuyet(ref vm._paging, PhienLamViec.NamLamViec, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai);

                List<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sTen = "--Tất cả--" });
                ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sTen");
                List<KeHoach5NamDeXuatModel> lstVoucherTypes = new List<KeHoach5NamDeXuatModel>()
            {
                new KeHoach5NamDeXuatModel(){SVoucherTypes = "Tất cả", iLoai = 0}
            };
                lstVoucherTypes.AddRange(CreateVoucherTypes());
                ViewBag.ListVoucherTypes = lstVoucherTypes.ToSelectList("iLoai", "SVoucherTypes");
            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            VDT_KHV_KeHoach5Nam data = new VDT_KHV_KeHoach5Nam();
            try
            {
                if (id.HasValue)
                {
                    data = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(id);
                }
                else
                {
                    data.iGiaiDoanTu = DateTime.Now.Year;
                    data.iGiaiDoanDen = DateTime.Now.Year + 4;
                }
                ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
                ViewBag.ListVoucherTypes = CreateVoucherTypes().ToSelectList("iLoai", "SVoucherTypes");
            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public JsonResult KeHoach5NamSave(VDT_KHV_KeHoach5Nam data, bool isDieuChinh)
        {
            try
            {
                List<VDT_KHV_KeHoach5Nam_ChiTiet> lstDetails = new List<VDT_KHV_KeHoach5Nam_ChiTiet>();

                if (data.iID_KeHoach5NamID == new Guid())
                {
                    if (_iQLVonDauTuService.CheckExistSoKeHoachKH5NDD(data.sSoQuyetDinh, PhienLamViec.NamLamViec, null))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                    }

                    if (!_iQLVonDauTuService.SaveKeHoach5NamDuocDuyet(ref data, Username, PhienLamViec.NamLamViec, isDieuChinh, lstDetails))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (isDieuChinh)
                    {
                        lstDetails = _iQLVonDauTuService.GetListKH5NamByIdKHDD(data.iID_KeHoach5NamID).ToList();
                        if (_iQLVonDauTuService.CheckExistSoKeHoachKH5NDD(data.sSoQuyetDinh, PhienLamViec.NamLamViec, null))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (_iQLVonDauTuService.CheckExistSoKeHoachKH5NDD(data.sSoQuyetDinh, PhienLamViec.NamLamViec, data.iID_KeHoach5NamID))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Số kế hoạch đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (!_iQLVonDauTuService.SaveKeHoach5NamDuocDuyet(ref data, Username, PhienLamViec.NamLamViec, isDieuChinh, lstDetails))
                    {
                        return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            return Json(new { bIsComplete = true, iID_KeHoach5NamID = data.iID_KeHoach5NamID}, JsonRequestBehavior.AllowGet); ;
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            KeHoach5NamDuocDuyetModel data = new KeHoach5NamDuocDuyetModel();
            try
            {
                data = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetByIdForDetail(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool KeHoach5NamDuocDuyetDelete(Guid id)
        {
            try
            {
                return _iQLVonDauTuService.deleteKeHoach5NamDuocDuyet(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        [HttpPost]
        public JsonResult LayThongTinChungTuDeXuat(Guid Id_KHTHDX)
        {
            VDT_KHV_KeHoach5Nam_DeXuat data = new VDT_KHV_KeHoach5Nam_DeXuat();

            try
            {
                data = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(Id_KHTHDX);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { bIsComplete = (data == null ? false : true), data = data }, JsonRequestBehavior.AllowGet);
        }

        #region Data grid kế hoạch trung hạn được duyệt chi tiết
        public ActionResult Detail(Guid id, string isViewDetail = "0")
        {
            VDT_KHV_KeHoach5Nam data = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(id);
            KeHoach5NamDuocDuyetChiTietGridViewModel vm = new KeHoach5NamDuocDuyetChiTietGridViewModel
            {
                KH5NamDuocDuyet = data
            };
            ViewBag.IsViewDetail = isViewDetail;
            return View(vm);
        }

        private void cloneFromTempData()
        {
            var lstData = (List<KeHoach5NamChiTietDuocDuyetTempForSave>)TempData["listNewKHV5NamChiTietNew"];
            if(lstData != null)
            {
                listTemp = lstData;
            }
            TempData.Keep();
            //().ForEach(item =>
            //{
            //    KeHoach5NamChiTietDuocDuyetTempForSave newItem = new KeHoach5NamChiTietDuocDuyetTempForSave();
            //    var entityKH5NChiTiet = new KeHoach5NamChiTietDuocDuyetTempForSave();
            //    entityKH5NChiTiet.iID_KeHoach5NamID = item.iID_KeHoach5NamID;
            //    entityKH5NChiTiet.iID_DuAnID = item.iID_DuAnID;
            //    entityKH5NChiTiet.iID_DonViQuanLyID = item.iID_DonViQuanLyID;
            //    entityKH5NChiTiet.sTen = item.sTen;
            //    entityKH5NChiTiet.iID_NguonVonID = item.iID_NguonVonID;
            //    entityKH5NChiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
            //    entityKH5NChiTiet.fHanMucDauTu = item.fHanMucDauTu;
            //    entityKH5NChiTiet.iID_MaDonVi = item.iID_MaDonVi;
            //    entityKH5NChiTiet.iID_KeHoach5Nam_ChiTietID = item.iID_KeHoach5Nam_ChiTietID;
            //    entityKH5NChiTiet.sDiaDiem = item.sDiaDiem;
            //    TempData.Keep();
            //    listTemp.Add(newItem);
            //});
        }

        public ActionResult SheetFrame(string id, string filter = null)
        {
            if (TempData["listNewKHV5NamChiTietNew"] != null)
            {
                TempData.Keep();
                cloneFromTempData(); 
            }
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            VDT_KHV_KeHoach5Nam KH5Nam = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(Guid.Parse(id));
            var sheet = new KeHoach5NamDuocDuyet_ChiTiet_SheetTable(id, int.Parse(PhienLamViec.iNamLamViec), KH5Nam.iGiaiDoanTu, filters, listTemp.Count > 0 ? listTemp : null);
            var vm = new KeHoach5NamDuocDuyetChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("SaveNew", "KeHoachTrungHanDuocDuyet", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrame", "KeHoachTrungHanDuocDuyet", new { area = "QLVonDauTu" })
                   ),
                KH5NamDuocDuyet = KH5Nam
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            TempData["currentListKHV5NamChiTiet"] = sheet.dtChiTiet;
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult SaveNew(SheetEditViewModel vm)
        {
            try
            {
                cloneFromTempData();
                var rows = vm.Rows.Where(x => !x.IsParent).ToList();
                //var rows = vm.Rows.ToList();
                if (rows.Count > 0)
                {
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();
                        #region crud
                        var columns = new KeHoach5NamDuocDuyet_ChiTiet_SheetTable().Columns.Where(x => !x.IsReadonly);
                        rows.ForEach(r =>
                        {
                            var trans = conn.BeginTransaction();
                            var iID_KeHoach5Nam_ChiTietID = r.Id;
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var entity = (VDT_KHV_KeHoach5Nam_ChiTiet)listTemp.Where(x => x.iID_KeHoach5Nam_ChiTietID.ToString() == iID_KeHoach5Nam_ChiTietID).FirstOrDefault();
                            bool isUpdate = false;
                            if(entity == null)
                            {
                                entity = conn.Get<VDT_KHV_KeHoach5Nam_ChiTiet>(iID_KeHoach5Nam_ChiTietID, trans);
                                isUpdate = true;
                            }

                            if (changes.Any())
                            {
                                if (r.Columns.ContainsKey("fVonDaGiao"))
                                {
                                    entity.fVonDaGiao = Double.Parse(r.Columns["fVonDaGiao"]);
                                }
                                if (r.Columns.ContainsKey("fVonBoTriTuNamDenNam"))
                                {
                                    entity.fVonBoTriTuNamDenNam = Double.Parse(r.Columns["fVonBoTriTuNamDenNam"]);
                                }
                                // entity.MapFrom(changes);
                                if (r.Columns.ContainsKey("fVonDaGiaoDc"))
                                {
                                    entity.fVonDaGiao = Double.Parse(r.Columns["fVonDaGiaoDc"]);
                                }
                                if (r.Columns.ContainsKey("fVonBoTriTuNamDenNamDc"))
                                {
                                    entity.fVonBoTriTuNamDenNam = Double.Parse(r.Columns["fVonBoTriTuNamDenNamDc"]);
                                }
                                if (r.Columns.ContainsKey("sGhiChu"))
                                {
                                    entity.sGhiChu = r.Columns["sGhiChu"];
                                }
                                entity.fGiaTriBoTri = (entity.fHanMucDauTu ?? 0) - (entity.fVonBoTriTuNamDenNam ?? 0);                                
                                
                            }
                            if (isUpdate)
                            {
                                if (r.IsDeleted)
                                {
                                    conn.Delete(entity, trans);
                                }
                                else conn.Update(entity, trans);
                            }
                            else conn.Insert(entity, trans);
                            // commit to db
                            trans.Commit();
                        });
                        #endregion
                    }
                }

                // Update Gia Tri Duoc Duyet
                _iQLVonDauTuService.UpdateGiaTriKeHoachDuocDuyet(vm.Id);
                TempData["listNewKHV5NamChiTietNew"] = null;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            // clear cache
            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filters = vm.FiltersString });
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            try
            {
                var rows = vm.Rows.Where(x => !x.IsParent).ToList();
                //var rows = vm.Rows.ToList();
                if (rows.Count > 0)
                {
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();
                        #region crud
                        var columns = new KeHoach5NamDuocDuyet_ChiTiet_SheetTable().Columns.Where(x => !x.IsReadonly);
                        rows.ForEach(r =>
                        {
                            var trans = conn.BeginTransaction();
                            var iID_KeHoach5Nam_ChiTietID = r.Id;
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var entity = conn.Get<VDT_KHV_KeHoach5Nam_ChiTiet>(iID_KeHoach5Nam_ChiTietID, trans);

                            if (!string.IsNullOrWhiteSpace(iID_KeHoach5Nam_ChiTietID) && r.IsDeleted)
                            {
                                #region delete
                                if (entity != null)
                                {
                                    conn.Delete(entity, trans);
                                }
                                #endregion
                            }
                            else if (changes.Any())
                            {
                                entity.MapFrom(changes);
                                if (entity.iID_ParentID != null && entity.iID_ParentID.HasValue)
                                {
                                    if (r.Columns.ContainsKey("fVonDaGiaoDc"))
                                    {
                                        entity.fVonDaGiao = Double.Parse(r.Columns["fVonDaGiaoDc"]);
                                    }

                                    if (r.Columns.ContainsKey("fVonBoTriTuNamDenNamDc"))
                                    {
                                        entity.fVonBoTriTuNamDenNam = Double.Parse(r.Columns["fVonBoTriTuNamDenNamDc"]);
                                    }
                                }
                                entity.fGiaTriBoTri = (entity.fHanMucDauTu ?? 0) - (entity.fVonBoTriTuNamDenNam ?? 0);
                                conn.Update(entity, trans);
                            }
                            // commit to db
                            trans.Commit();
                        });
                        #endregion
                    }
                }

                // Update Gia Tri Duoc Duyet
                _iQLVonDauTuService.UpdateGiaTriKeHoachDuocDuyet(vm.Id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            // clear cache
            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filters = vm.FiltersString });
        }

        public ActionResult ViewInBaoCao(bool isCt)
        {
            KH5NDDPrintDataExportModel vm = new KH5NDDPrintDataExportModel();

            IEnumerable<NS_NguonNganSach> lstDMNguonNganSach = _iQLVonDauTuService.GetListDMNguonNganSach();
            vm.lstDMNguonNganSach = lstDMNguonNganSach;
            IEnumerable<NS_DonVi> lstDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec);
            IEnumerable<VDT_DM_DonViThucHienDuAn> lstDonViThucHienDuAn = _danhMucService.GetListDonViThucHienDuAn();
            vm.lstDonViThucHienDuAn = lstDonViThucHienDuAn;
            vm.lstDonViQuanLy = lstDonViQuanLy;
            vm.iGiaiDoanTu = DateTime.Now.Year;
            vm.iGiaiDoanDen = DateTime.Now.Year + 4;

            ViewBag.ListDonViQuanLy = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToSelectList("iID_Ma", "sTen");
            ViewBag.ListNguonNganSach = lstDMNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");

            List<VDT_DM_LoaiCongTrinh> lstLoaiCongTrinh = _iQLVonDauTuService.GetListParentDMLoaiCongTrinh();
            lstLoaiCongTrinh.Insert(0, new VDT_DM_LoaiCongTrinh { iID_LoaiCongTrinh = Guid.Empty, sTenLoaiCongTrinh = "--Tất cả--" });
            ViewBag.ListLoaiCongTrinh = lstLoaiCongTrinh.ToSelectList("iID_LoaiCongTrinh", "sTenLoaiCongTrinh");
            ViewBag.LstDonViTinh = LoadDonViTinh().ToSelectList("ValueItem", "DisplayItem");
            if (isCt)
            {
                ViewBag.TitleDx = "In báo cáo kế hoạch trung hạn được duyệt(Chuyển tiếp)";
                ViewBag.TieuDe2 = "Công trình chuyển tiếp";
                ViewBag.isCt = "true";
            }
            else
            {
                ViewBag.TitleDx = "In báo cáo kế hoạch trung hạn được duyệt";
                ViewBag.TieuDe2 = "Công trình mở mới";
                ViewBag.isCt = "false";
            }

            return View(vm);
        }

        public JsonResult LayDanhSachChungTuDuocDuyetTheoDonViQuanLy(string iID_DonViQuanLyID, bool isCt)
        {
            List<VDT_KHV_KeHoach5Nam> lstChungTuDuocDuyet = _iQLVonDauTuService.GetLstChungTuDuocDuyet(Guid.Parse(iID_DonViQuanLyID), PhienLamViec.NamLamViec, isCt).ToList();
            StringBuilder htmlString = new StringBuilder();
            htmlString.AppendFormat("<option value='{0}'>{1}</option>", "", Common.Constants.CHON);
            if (lstChungTuDuocDuyet != null && lstChungTuDuocDuyet.Count > 0)
            {
                for (int i = 0; i < lstChungTuDuocDuyet.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}'>{1}</option>", lstChungTuDuocDuyet[i].iID_KeHoach5NamID, lstChungTuDuocDuyet[i].sSoQuyetDinh);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool ExportBCTongHop(KH5NDDPrintDataExportModel data, bool isCt)
        {
            try
            {
                data.isTongHop = true;

                if (isCt)
                {
                    List<KH5NDDPrintDataChuyenTiepExportModel> dataReport = _iQLVonDauTuService.FindByReportKeHoachTrungHanChuyenTiep(data.iID_KeHoach5NamID.ToString(), data.iID_NguonVonID.ToString(), string.Empty, 1, data.fDonViTinh.Value).ToList();
                    dataReport = HandleDataDuocDuyetChuyenTiepTongHop(dataReport);
                    TempData["dataReportDc"] = dataReport;
                }
                else
                {
                    List<KH5NDDPrintDataExportModel> dataReport = _iQLVonDauTuService.FindByReportKeHoachTrungHan(data.iID_KeHoach5NamID.ToString(), data.iID_LoaiCongTrinh, data.iID_NguonVonID.Value, 1, data.fDonViTinh.Value, string.Empty).ToList();

                    dataReport = CalculateDataReportDuocDuyetTongHop(dataReport);
                    dataReport = HandleDataReportTongHop(dataReport);
                    if (data.iID_NguonVonID.Value != 1)
                    {
                        dataReport.Select(x =>
                        {
                            x.FHanMucDauTu = 0;
                            return x;
                        }).ToList();
                    }

                    TempData["dataReport"] = dataReport;
                }

                TempData["paramReport"] = data;
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        [HttpPost]
        public JsonResult KeHoach5NamDuocDuyetExport(Guid? idKeHoach5NamDuocDuyet)
        {
            try
            {
                List<VdtKhvKeHoach5NamExportModel> lstQuery = _iQLVonDauTuService.GetDataExportKeHoachTrungHan(idKeHoach5NamDuocDuyet.ToString()).ToList();

                ExcelFile xls = CreateReportExport(lstQuery, idKeHoach5NamDuocDuyet);
                xls.PrintLandscape = true;

                TempData["DataExport"] = xls;
            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public ExcelFile CreateReportExport(List<VdtKhvKeHoach5NamExportModel> lstQuery, Guid? idKeHoach5Nam)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/rptKeHoachTrungHan.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            VDT_KHV_KeHoach5Nam item = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(idKeHoach5Nam);

            VdtKhvKeHoach5NamExportModel RptSummary = new VdtKhvKeHoach5NamExportModel();

            RptSummary.FHanMucDauTu = lstQuery.Sum(x => x.FHanMucDauTu);
            RptSummary.FVonDaGiao = lstQuery.Sum(x => x.FVonDaGiao);
            RptSummary.FTongSoNhuCauNSQP = lstQuery.Sum(x => x.FTongSoNhuCauNSQP);
            RptSummary.FVonBoTriTuNamDenNam = lstQuery.Sum(x => x.FVonBoTriTuNamDenNam);
            RptSummary.FGiaTriSau5Nam = lstQuery.Sum(x => x.FGiaTriSau5Nam);

            string STenDonVi = string.Empty;
            if(item != null)
            {
                NS_DonVi donVi = _iQLVonDauTuService.GetDonViQuanLyById(item.iID_DonViQuanLyID);
                if(donVi != null)
                {
                    STenDonVi = donVi.sTen;
                }
            }

            fr.AddTable<VdtKhvKeHoach5NamExportModel>("Items", lstQuery);

            fr.SetValue("TenDonVi", !string.IsNullOrEmpty(STenDonVi) ? STenDonVi.ToUpper() : string.Empty);
            fr.SetValue("Year", item.iGiaiDoanTu);
            fr.SetValue("YearAfter", item.iGiaiDoanDen);
            fr.SetValue("Period", string.Format("{0} - {1}", item.iGiaiDoanTu, item.iGiaiDoanDen));
            fr.SetValue("FHanMucDauTuSum", RptSummary.FHanMucDauTu);
            fr.SetValue("FVonDaGiaoSum", RptSummary.FVonDaGiao);
            fr.SetValue("FTongSoNhuCauNSQPSum", RptSummary.FTongSoNhuCauNSQP);
            fr.SetValue("FVonBoTriTuNamDenNamSum", RptSummary.FVonBoTriTuNamDenNam);
            fr.SetValue("FGiaTriSau5NamSum", RptSummary.FGiaTriSau5Nam);
            fr.SetValue("iNamLamViec", PhienLamViec.iNamLamViec);          

            fr.Run(Result);
            return Result;
        }

        public FileContentResult ExportExcel5NDuocDuyet()
        {
            string sContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string sFileName = "KeHoachTrungHanDuocDuyet.xlsx";
            ExcelFile xls = (ExcelFile)TempData["DataExport"];
            xls.PrintLandscape = true;
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return File(stream.ToArray(), sContentType, sFileName);
            }
        }
        [HttpPost]
        public ActionResult SendFile(Guid id, string idDonVi, string giaiDoanTu, string giaiDoanDen)
        {
            var excel = KeHoach5NamDuocDuyetExport(id);
            ExcelFile xls = (ExcelFile)TempData["DataExport"];
            string sFileName = "KeHoachTrungHanDuocDuyet.xlsx";
            using (MemoryStream stream = new MemoryStream())
            {
                xls.Save(stream);
                return SaveFile(stream.ToArray(), sFileName, idDonVi, giaiDoanTu, giaiDoanDen);
            }
        }
        public ActionResult SaveFile(byte[] contentFile, string filename, string idDonVi, string giaiDoanTu, string giaiDoanDen)
        {
            string tenDonVi = "0";
            NS_DonVi donVi = _iNganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), idDonVi);
            if (donVi != null)
            {
                tenDonVi = donVi.iID_MaDonVi;
            }
            string folder = "VDT/KeHoachTrungHanDuocDuyet/receive/";
            var uri = pathString + "/" + tenDonVi + "/" + folder + giaiDoanTu.Trim() + "-" + giaiDoanDen.Trim();
            //var path = string.Format("{0}\\{1}", pathString, CommonFunction.UCS2Convert(filename).Replace(" ", ""));
            DateTime crTime = DateTime.Now;
            string path = uri + "/" + crTime.ToString("ddMMyyyyhhmmssfff") + "_" + filename;
            FtpWebRequest reqFTP = null;
            try
            {
                Stream ftpStream = null;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                //directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
            }
            try
            {
                FtpWebRequest reqFile = (FtpWebRequest)FtpWebRequest.Create(path);
                reqFile.Method = WebRequestMethods.Ftp.UploadFile;
                reqFile.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                using (Stream requestStream = reqFile.GetRequestStream())
                {
                    requestStream.Write(contentFile, 0, contentFile.Length);
                    requestStream.Close();
                }
            }
            catch (Exception ex)
            {
                //directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
            }
            try
            {
                VDT_FtpRoot rootInfo = new VDT_FtpRoot();
                rootInfo = _iQLVonDauTuService.GetFtpRoot(tenDonVi, pathString, folder);
                if(rootInfo == null)
                {
                    rootInfo = new VDT_FtpRoot();
                    rootInfo.Id = Guid.NewGuid();
                    rootInfo.sMaDonVi = tenDonVi;
                    rootInfo.sIpAddress = pathString;
                    rootInfo.sFolderRoot = folder;
                    rootInfo.sNguoiTao = Username;
                    rootInfo.dNgayTao = DateTime.Now;
                    _iQLVonDauTuService.InsertFtpRoot(rootInfo);
                }
                var fileInfo = new VDT_FtpFile();
                fileInfo.Id = Guid.NewGuid();
                fileInfo.iID_FtpRoot = rootInfo.Id;
                fileInfo.sFileName = crTime.ToString("ddMMyyyyhhmmssfff") + "_" + filename;
                fileInfo.sRootPath = path;
                fileInfo.sNguoiTao = Username;
                fileInfo.dNgayTao = DateTime.Now;
                _iQLVonDauTuService.InsertFtpFile(fileInfo);
            }
            catch (Exception ex)
            {

            }
            return Json("");
        }
        public bool ExportBCTheoDonVi(KH5NDDPrintDataExportModel data, List<string> arrDVTHDA, bool isCt)
        {
            string lstDonViThucHienDuAn = string.Empty;
            if(arrDVTHDA != null && arrDVTHDA.Count() > 0)
            {
                lstDonViThucHienDuAn = string.Join(",", arrDVTHDA);
            }

            string lstId = string.Empty;
            var itemQuery = _iQLVonDauTuService.GetLstIdChungTuDuocDuyet(data.iGiaiDoanTu, data.iGiaiDoanDen, isCt, PhienLamViec.NamLamViec);
            if(itemQuery != null)
            {
                lstId = string.Join(",", itemQuery);
            }

            if(isCt)
            {
                List<KH5NDDPrintDataChuyenTiepExportModel> dataReport = _iQLVonDauTuService.FindByReportKeHoachTrungHanChuyenTiep(lstId, data.iID_NguonVonID.ToString(),lstDonViThucHienDuAn, 2, data.fDonViTinh.Value).ToList();
                dataReport = HandleDataDuocDuyetChuyenTiepDonVi(dataReport);
                TempData["dataReportDc"] = dataReport;
            }
            else
            {
                List<KH5NDDPrintDataExportModel> dataReport = _iQLVonDauTuService.FindByReportKeHoachTrungHan(lstId, data.iID_LoaiCongTrinh, data.iID_NguonVonID.Value, 2, data.fDonViTinh.Value, lstDonViThucHienDuAn).ToList();

                dataReport = CalculateDataReportDuocDuyetDonVi(dataReport);
                dataReport = HandleDataReportDonVi(dataReport);

                TempData["dataReport"] = dataReport;
            }

            TempData["paramReport"] = data;

            return true;
        }

        public ActionResult ExportExcel(bool isCt)
        {
            string sContentType = "application/pdf";
            string sFileName = "KeHoachTrungHan_DuocDuyet_Report.pdf";
            List<KH5NDDPrintDataExportModel> dataReport = null;
            List<KH5NDDPrintDataChuyenTiepExportModel> dataReportCt = null;

            KH5NDDPrintDataExportModel paramReport = null;

            paramReport = (KH5NDDPrintDataExportModel)TempData["paramReport"];

            if (isCt)
            {
                if (TempData["dataReportDc"] != null)
                {
                    dataReportCt = (List<KH5NDDPrintDataChuyenTiepExportModel>)TempData["dataReportDc"];
                }
            }
            else
            {
                if (TempData["dataReport"] != null)
                {
                    dataReport = (List<KH5NDDPrintDataExportModel>)TempData["dataReport"];
                }
                else
                    return RedirectToAction("ViewInBaoCao");
            }

            ExcelFile xls = null;
            if (isCt)
            {
                xls = CreateReportCt(dataReportCt, paramReport);
            }
            else
            {
                xls = CreateReport(dataReport, paramReport);
            }

            xls.PrintLandscape = true;
            FlexCelPdfExport pdf = new FlexCelPdfExport(xls, true);
            //var bufferPdf = new MemoryStream();

            //pdf.Export(bufferPdf);

            //Response.ContentType = sContentType;
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + sFileName);
            //Response.BinaryWrite(bufferPdf.ToArray());

            //Response.Flush();
            //Response.End();
            return xls.ToPdfContentResult(sFileName);
        }

        public ExcelFile CreateReport(List<KH5NDDPrintDataExportModel> dataReport, KH5NDDPrintDataExportModel paramReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/rptKeHoachTrungHanReport.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            VDT_KHV_KeHoach5Nam itemQuery = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(paramReport.iID_KeHoach5NamID);

            int iNamBatDau = DateTime.Now.Year;
            int iNamKetThuc = DateTime.Now.Year + 4;
            string STenDonVi = string.Empty;
            iNamBatDau = paramReport.iGiaiDoanTu;
            iNamKetThuc = paramReport.iGiaiDoanDen;

            if (itemQuery != null)
            {
                iNamBatDau = itemQuery.iGiaiDoanTu;
                iNamKetThuc = itemQuery.iGiaiDoanDen;
                if (paramReport != null && paramReport.isTongHop)
                {
                    NS_DonVi itemDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, paramReport.iID_DonViQuanLyID.ToString());
                    if (itemDonVi != null)
                    {
                        STenDonVi = itemDonVi.sTen;
                    }
                }
            }

            fr.AddTable<KH5NDDPrintDataExportModel>("Items", dataReport);

            fr.SetValue("YearBefore", iNamBatDau - 1);
            fr.SetValue("YearAfter", iNamKetThuc);
            fr.SetValue("Period", string.Format("{0} - {1}", iNamBatDau, iNamKetThuc));
            fr.SetValue("TenDonVi", !string.IsNullOrEmpty(STenDonVi) ? STenDonVi.ToUpper() : string.Empty);
            fr.SetValue("NguonNganSach", paramReport.sTenNguonVon);
            fr.SetValue("Header1", paramReport.txt_TieuDe1);
            fr.SetValue("Header3", paramReport.txt_TieuDe3);
            fr.SetValue("DonViTinh", paramReport.sDonViTinh);
            fr.UseChuKy(Username)
                  .UseChuKyForController(sControlName)
                  .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        public ExcelFile CreateReportCt(List<KH5NDDPrintDataChuyenTiepExportModel> dataReport, KH5NDDPrintDataExportModel paramReport)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/KeHoachTrungHan/rptKeHoachTrungHan_ChuyenTiep_Report.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            KH5NDDPrintDataChuyenTiepExportModel dataSummary = new KH5NDDPrintDataChuyenTiepExportModel();
            dataSummary.TongMucDauTu = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.TongMucDauTu);
            dataSummary.TongMucDauTuNSQP = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.TongMucDauTuNSQP);
            dataSummary.ChiPhiDuPhong = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.ChiPhiDuPhong);
            dataSummary.VonBoTriHetNam = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.VonBoTriHetNam);
            dataSummary.VonDaBoTriNam = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.VonDaBoTriNam);
            dataSummary.TongMucDauTuPhaiBoTri = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.TongMucDauTuPhaiBoTri);
            dataSummary.KeHoachVonNamDoDuyet = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.KeHoachVonNamDoDuyet);
            dataSummary.KeHoachVonDeNghiBoTriNam = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.KeHoachVonDeNghiBoTriNam);
            dataSummary.ChenhLechSoVoiQuyetDinhBo = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.ChenhLechSoVoiQuyetDinhBo);
            dataSummary.TongSo = dataReport.Where(x => !x.IsHangCha.Value).Sum(x => x.TongSo);

            VDT_KHV_KeHoach5Nam itemQuery = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(paramReport.iID_KeHoach5NamID);

            int iNamBatDau = DateTime.Now.Year;
            int iNamKetThuc = DateTime.Now.Year + 4;
            string STenDonVi = string.Empty;

            if (itemQuery != null)
            {
                iNamBatDau = itemQuery.iGiaiDoanTu;
                iNamKetThuc = itemQuery.iGiaiDoanDen;
                if (paramReport != null && paramReport.isTongHop)
                {
                    NS_DonVi itemDonVi = _iNganSachService.GetDonViById(PhienLamViec.iNamLamViec, paramReport.iID_DonViQuanLyID.ToString());
                    if (itemDonVi != null)
                    {
                        STenDonVi = itemDonVi.sTen;
                    }
                }
            }

            fr.AddTable<KH5NDDPrintDataChuyenTiepExportModel>("Items", dataReport);

            fr.SetValue("BeforeYear", iNamBatDau - 1);
            fr.SetValue("Year1", iNamBatDau);
            fr.SetValue("YearPeriod", string.Format("{0}-{1}", iNamBatDau, iNamKetThuc));
            fr.SetValue("TenDonVi", !string.IsNullOrEmpty(STenDonVi) ? STenDonVi.ToUpper() : string.Empty);
            fr.SetValue("Header1", paramReport.txt_TieuDe1);
            fr.SetValue("Header2", paramReport.txt_TieuDe2);
            fr.SetValue("Header3", paramReport.txt_TieuDe3);
            fr.SetValue("DonViTinh", paramReport.sDonViTinh);
            fr.SetValue("TongMucDauTuSum", dataSummary.TongMucDauTu);
            fr.SetValue("TongMucDauTuNSQPSum", dataSummary.TongMucDauTuNSQP);
            fr.SetValue("ChiPhiDuPhongSum", dataSummary.ChiPhiDuPhong);
            fr.SetValue("TongSoSum", dataSummary.TongSo);
            fr.SetValue("VonBoTriHetNamSum", dataSummary.VonBoTriHetNam);
            fr.SetValue("VonDaBoTriNamSum", dataSummary.VonDaBoTriNam);
            fr.SetValue("TongMucDauTuPhaiBoTriSum", dataSummary.TongMucDauTuPhaiBoTri);
            fr.SetValue("KeHoachVonNamDoDuyetSum", dataSummary.KeHoachVonNamDoDuyet);
            fr.SetValue("KeHoachVonDeNghiBoTriNamSum", dataSummary.KeHoachVonDeNghiBoTriNam);
            fr.SetValue("ChenhLechSoVoiQuyetDinhBoSum", dataSummary.ChenhLechSoVoiQuyetDinhBo);
            fr.UseChuKy(Username)
                  .UseChuKyForController(sControlName)
                  .UseForm(this);
            fr.Run(Result);
            return Result;
        }

        private List<KH5NDDPrintDataChuyenTiepExportModel> HandleDataDuocDuyetChuyenTiepDonVi(List<KH5NDDPrintDataChuyenTiepExportModel> ItemsReportChuyenTiep)
        {
            try
            {
                ItemsReportChuyenTiep.Where(x => !x.IsHangCha.Value).Select(item =>
                {
                    item.TongSo = item.VonBoTriHetNam + item.VonDaBoTriNam;
                    item.TongMucDauTuPhaiBoTri = item.KeHoachVonNamDoDuyet + item.KeHoachVonDeNghiBoTriNam;
                    item.ChenhLechSoVoiQuyetDinhBo = item.KeHoachVonDeNghiBoTriNam - item.KeHoachVonNamDoDuyet;
                    return item;
                }).ToList();

                List<KH5NDDPrintDataChuyenTiepExportModel> childrent = ItemsReportChuyenTiep.Where(x => !x.IsHangCha.Value).ToList();
                List<KH5NDDPrintDataChuyenTiepExportModel> parent = ItemsReportChuyenTiep.Where(x => x.IsHangCha.Value).ToList();
                List<KH5NDDPrintDataChuyenTiepExportModel> result = new List<KH5NDDPrintDataChuyenTiepExportModel>();

                parent.Select(x =>
                {
                    x.TongMucDauTu = 0;
                    x.TongMucDauTuNSQP = 0;
                    x.ChiPhiDuPhong = 0;
                    x.TongSo = 0;
                    x.VonBoTriHetNam = 0;
                    x.TongMucDauTuPhaiBoTri = 0;
                    x.KeHoachVonNamDoDuyet = 0;
                    x.KeHoachVonDeNghiBoTriNam = 0;
                    x.ChenhLechSoVoiQuyetDinhBo = 0;

                    return x;
                }).ToList();

                foreach (var pr in parent)
                {
                    List<KH5NDDPrintDataChuyenTiepExportModel> lstChilrent = childrent.Where(x => x.IIdMaDonVi.Equals(pr.IIdMaDonVi)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.TongMucDauTu != 0 || x.TongMucDauTuNSQP != 0
                                || x.ChiPhiDuPhong != 0 || x.TongSo != 0 || x.VonBoTriHetNam != 0 || x.TongMucDauTuPhaiBoTri != 0 || x.KeHoachVonNamDoDuyet != 0
                                || x.KeHoachVonDeNghiBoTriNam != 0 || x.ChenhLechSoVoiQuyetDinhBo != 0)))
                    {
                        pr.TongMucDauTu += item.TongMucDauTu;
                        pr.TongMucDauTuNSQP += item.TongMucDauTuNSQP;
                        pr.ChiPhiDuPhong += item.ChiPhiDuPhong;
                        pr.TongSo += item.TongSo;
                        pr.VonBoTriHetNam += item.VonBoTriHetNam;
                        pr.TongMucDauTuPhaiBoTri += item.TongMucDauTuPhaiBoTri;
                        pr.KeHoachVonNamDoDuyet += item.KeHoachVonNamDoDuyet;
                        pr.KeHoachVonDeNghiBoTriNam += item.KeHoachVonDeNghiBoTriNam;
                        pr.ChenhLechSoVoiQuyetDinhBo += item.ChenhLechSoVoiQuyetDinhBo;
                    }
                }

                ItemsReportChuyenTiep = ItemsReportChuyenTiep.Where(x => (x.TongMucDauTu != 0 || x.TongMucDauTuNSQP != 0
                                || x.ChiPhiDuPhong != 0 || x.TongSo != 0 || x.VonBoTriHetNam != 0 || x.TongMucDauTuPhaiBoTri != 0 || x.KeHoachVonNamDoDuyet != 0
                                || x.KeHoachVonDeNghiBoTriNam != 0 || x.ChenhLechSoVoiQuyetDinhBo != 0)).ToList();

                return ItemsReportChuyenTiep;
            }
            catch (Exception ex)
            {
                return ItemsReportChuyenTiep;
            }
        }

        private List<KH5NDDPrintDataChuyenTiepExportModel> HandleDataDuocDuyetChuyenTiepTongHop(List<KH5NDDPrintDataChuyenTiepExportModel> ItemsReportChuyenTiep)
        {
            try
            {
                ItemsReportChuyenTiep.Where(x => !x.IsHangCha.Value).Select(item =>
                {
                    item.TongSo = item.VonBoTriHetNam + item.VonDaBoTriNam;
                    item.TongMucDauTuPhaiBoTri = item.TongMucDauTuNSQP -  item.VonBoTriHetNam;
                    item.ChenhLechSoVoiQuyetDinhBo = item.KeHoachVonDeNghiBoTriNam - item.KeHoachVonNamDoDuyet;
                    return item;
                }).ToList();

                ItemsReportChuyenTiep.Where(x => x.IsHangCha.Value).Select((item, index) => { item.STT = ToRoman(index + 1); return item; }).ToList();
                ItemsReportChuyenTiep.Where(x => !x.IsHangCha.Value).Select((item, index) => { item.STT = (index + 1).ToString(); return item; }).ToList();

                return ItemsReportChuyenTiep;
            }
            catch(Exception ex)
            {
                return ItemsReportChuyenTiep;
            }
        }

        private List<KH5NDDPrintDataExportModel> HandleDataReportTongHop(List<KH5NDDPrintDataExportModel> ItemsReport)
        {
            try
            {
                List<KH5NDDPrintDataExportModel> lstItem = ItemsReport.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();

                List<KH5NDDPrintDataExportModel> lstItemLevel = ItemsReport.Where(x => x.IdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var key in dctItemLevel.Keys)
                {
                    List<KH5NDDPrintDataExportModel> lstItemStt = dctItemLevel[key].ToList();
                    lstItemStt.Select(x => { x.STT = (lstItemStt.IndexOf(x) + 1).ToString(); return x; }).ToList();
                }

                foreach (var item in lstItemLevel)
                {
                    List<KH5NDDPrintDataExportModel> lstChildrent = ItemsReport.Where(x => x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && !x.IsHangCha).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                List<KH5NDDPrintDataExportModel> lstLctParent = ItemsReport.Where(x => x.LoaiParent.Equals(0)).ToList();
                var dctItemParent = ItemsReport.Where(x => !x.IsHangCha && x.IdLoaiCongTrinh.HasValue
                    && lstLctParent.Select(y => y.IdLoaiCongTrinh).Contains(x.IdLoaiCongTrinh)).GroupBy(z => z.IdLoaiCongTrinh).ToDictionary(z => z.Key.ToString(), z => z.ToList());
                foreach (var item in dctItemParent.Keys)
                {
                    List<KH5NDDPrintDataExportModel> itemStt = dctItemParent[item];
                    itemStt.Select(x => { x.STT = string.Format("{0}", (itemStt.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                return ItemsReport;
            }
            catch (Exception ex)
            {
                return ItemsReport;
            }
        }

        private List<KH5NDDPrintDataExportModel> HandleDataReportDonVi(List<KH5NDDPrintDataExportModel> ItemsReport)
        {
            try
            {
                List<KH5NDDPrintDataExportModel> lstItem = ItemsReport.Where(n => n.LoaiParent.Equals(0)).ToList();
                lstItem.Select(n => { n.STT = ToRoman((lstItem.IndexOf(n) + 1)).ToString(); return n; }).ToList();

                List<KH5NDDPrintDataExportModel> lstItemLevel = ItemsReport.Where(x => x.IdLoaiCongTrinhParent != null && x.IsHangCha && x.LoaiParent.Equals(1)).ToList();
                var dctItemLevel = lstItemLevel.GroupBy(x => x.IdLoaiCongTrinhParent).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var key in dctItemLevel.Keys)
                {
                    List<KH5NDDPrintDataExportModel> lstItemStt = dctItemLevel[key].ToList();
                    lstItemStt.Select(x => { x.STT = (lstItemStt.IndexOf(x) + 1).ToString(); return x; }).ToList();
                }
                foreach (var item in lstItemLevel)
                {
                    List<KH5NDDPrintDataExportModel> lstChildrent = ItemsReport.Where(x => x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x => { x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString()); return x; }).ToList();
                }

                foreach (var item in ItemsReport.Where(x => x.IsHangCha && x.LoaiParent.Equals(2)))
                {
                    List<KH5NDDPrintDataExportModel> lstChildrent = ItemsReport.Where(x => x.IdLoaiCongTrinh == item.IdLoaiCongTrinh && !x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                    lstChildrent.Select(x =>
                    {
                        if (string.IsNullOrEmpty(item.STT))
                        {
                            x.STT = string.Format("{0}", (lstChildrent.IndexOf(x) + 1).ToString());
                        }
                        else
                        {
                            x.STT = string.Format("{0}.{1}", item.STT, (lstChildrent.IndexOf(x) + 1).ToString());
                        }
                        return x;
                    }).ToList();
                }

                return ItemsReport;
            }
            catch (Exception ex)
            {
                return ItemsReport;
            }
        }

        private List<KH5NDDPrintDataExportModel> CalculateDataReportDuocDuyetTongHop(List<KH5NDDPrintDataExportModel> ItemsReport)
        {
            try
            {
                List<KH5NDDPrintDataExportModel> childrent = ItemsReport.Where(x => !x.IsHangCha).ToList();
                List<KH5NDDPrintDataExportModel> parent = ItemsReport.Where(x => x.IsHangCha && (x.LoaiParent.Equals(0) || x.LoaiParent.Equals(1))).ToList();

                ItemsReport.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.FHanMucDauTu = 0;
                    x.FGiaTriKeHoach = 0;
                    x.FVonDaGiao = 0;
                    x.FVonBoTri = 0;
                    x.FTongVonBoTri = 0;
                    return x;
                }).ToList();

                foreach (var pr in parent)
                {
                    List<KH5NDDPrintDataExportModel> lstChilrent = childrent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0 || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0)))
                    {
                        pr.FHanMucDauTu += item.FHanMucDauTu ?? 0;
                        pr.FGiaTriKeHoach += item.FGiaTriKeHoach ?? 0;
                        pr.FVonDaGiao += item.FVonDaGiao ?? 0;
                        pr.FVonBoTri += item.FVonBoTri ?? 0;
                        pr.FTongVonBoTri += item.FTongVonBoTri ?? 0;
                    }
                }

                foreach (var item in parent.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0 || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0)))
                {
                    CalculateParent(item, item, ItemsReport);
                }

                List<KH5NDDPrintDataExportModel> lstDataChildrent = ItemsReport.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0
                                        || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0)).ToList();

                List<KH5NDDPrintDataExportModel> lstDataParent = ItemsReport.Where(x => x.LoaiParent.Equals(0)
                            && lstDataChildrent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh)).ToList();

                List<KH5NDDPrintDataExportModel> listGroup = ItemsReport.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0
                                        || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0
                                        || lstDataParent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh))).ToList().GroupBy(x => new
                                        {
                                            x.IdLoaiCongTrinh,
                                            x.IdLoaiCongTrinhParent,
                                            x.Loai,
                                            x.LoaiParent,
                                            x.STenDuAn,
                                            x.GhiChu,
                                            x.IsHangCha,
                                            x.STT
                                        }).Select(x => new KH5NDDPrintDataExportModel()
                                        {
                                            STT = x.Key.STT,
                                            IsHangCha = x.Key.IsHangCha,
                                            GhiChu = x.Key.GhiChu,
                                            STenDuAn = x.Key.STenDuAn,
                                            LoaiParent = x.Key.LoaiParent,
                                            Loai = x.Key.Loai,
                                            IdLoaiCongTrinhParent = x.Key.IdLoaiCongTrinhParent,
                                            IdLoaiCongTrinh = x.Key.IdLoaiCongTrinh,
                                            FHanMucDauTu = x.Sum(rpt => rpt.FHanMucDauTu),
                                            FVonDaGiao = x.Sum(rpt => rpt.FVonDaGiao),
                                            FTongVonBoTri = x.Sum(rpt => rpt.FTongVonBoTri),
                                            FGiaTriKeHoach = x.Sum(rpt => rpt.FGiaTriKeHoach),
                                            FVonBoTri = x.Sum(rpt => rpt.FVonBoTri)
                                        }).ToList();
                return listGroup;
            }
            catch(Exception ex)
            {
                return new List<KH5NDDPrintDataExportModel>();
            }
        }

        private List<KH5NDDPrintDataExportModel> CalculateDataReportDuocDuyetDonVi(List<KH5NDDPrintDataExportModel> ItemsReport)
        {
            try
            {
                List<KH5NDDPrintDataExportModel> lstDonViparent = ItemsReport.Where(x => x.IsHangCha && x.LoaiParent.Equals(2)).ToList();
                List<KH5NDDPrintDataExportModel> lstLoaiCongTrinhparent = ItemsReport.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).ToList();

                ItemsReport.Where(x => x.IsHangCha && x.LoaiParent.Equals(1)).Select(x =>
                {
                    x.FHanMucDauTu = 0;
                    x.FGiaTriKeHoach = 0;
                    x.FVonDaGiao = 0;
                    x.FVonBoTri = 0;
                    x.FTongVonBoTri = 0;
                    return x;
                }).ToList();
                int count = 0;
                foreach (var pr in lstLoaiCongTrinhparent)
                {
                    List<KH5NDDPrintDataExportModel> lstChilrent = lstDonViparent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                    foreach (var item in lstChilrent.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0 || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0)))
                    {
                        pr.FHanMucDauTu += item.FHanMucDauTu ?? 0;
                        pr.FGiaTriKeHoach += item.FGiaTriKeHoach ?? 0;
                        pr.FVonDaGiao += item.FVonDaGiao ?? 0;
                        pr.FVonBoTri += item.FVonBoTri ?? 0;
                        pr.FTongVonBoTri += item.FTongVonBoTri ?? 0;
                        count++;
                    }
                }

                // chi co 1 loai cong trinh thi cong tong cac hang con
                if(count == 0)
                {
                    List<KH5NDDPrintDataExportModel> childrent = ItemsReport.Where(x => !x.IsHangCha).ToList();
                    List<KH5NDDPrintDataExportModel> parent = ItemsReport.Where(x => x.IsHangCha && (x.LoaiParent.Equals(0) || x.LoaiParent.Equals(1))).ToList();
                    foreach (var pr in parent)
                    {
                        List<KH5NDDPrintDataExportModel> lstChilrent = childrent.Where(x => x.IdLoaiCongTrinh.Equals(pr.IdLoaiCongTrinh)).ToList();
                        foreach (var item in lstChilrent.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0 || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0)))
                        {
                            pr.FHanMucDauTu += item.FHanMucDauTu ?? 0;
                            pr.FGiaTriKeHoach += item.FGiaTriKeHoach ?? 0;
                            pr.FVonDaGiao += item.FVonDaGiao ?? 0;
                            pr.FVonBoTri += item.FVonBoTri ?? 0;
                            pr.FTongVonBoTri += item.FTongVonBoTri ?? 0;
                        }
                    }
                }               

                foreach (var item in lstLoaiCongTrinhparent.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0 || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0)))
                {
                    CalculateParent(item, item, ItemsReport);
                }

                List<KH5NDDPrintDataExportModel> lstDataChildrent = ItemsReport.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0
                                        || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0)).ToList();

                List<KH5NDDPrintDataExportModel> lstDataParent = ItemsReport.Where(x => x.LoaiParent.Equals(0)
                            && lstDataChildrent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh)).ToList();

                List<KH5NDDPrintDataExportModel> listGroup = ItemsReport.Where(x => (x.FHanMucDauTu != 0 || x.FGiaTriKeHoach != 0
                                        || x.FVonDaGiao != 0 || x.FVonBoTri != 0 || x.FTongVonBoTri != 0
                                        || lstDataParent.Select(y => y.IdLoaiCongTrinh).ToList().Contains(x.IdLoaiCongTrinh))).ToList().GroupBy(x => new
                                        {
                                            x.IdLoaiCongTrinh,
                                            x.IdLoaiCongTrinhParent,
                                            x.Loai,
                                            x.LoaiParent,
                                            x.STenDuAn,
                                            x.GhiChu,
                                            x.IsHangCha,
                                            x.STT
                                        }).Select(x => new KH5NDDPrintDataExportModel()
                                        {
                                            STT = x.Key.STT,
                                            IsHangCha = x.Key.IsHangCha,
                                            GhiChu = x.Key.GhiChu,
                                            STenDuAn = x.Key.STenDuAn,
                                            LoaiParent = x.Key.LoaiParent,
                                            Loai = x.Key.Loai,
                                            IdLoaiCongTrinhParent = x.Key.IdLoaiCongTrinhParent,
                                            IdLoaiCongTrinh = x.Key.IdLoaiCongTrinh,
                                            FHanMucDauTu = x.Sum(rpt => rpt.FHanMucDauTu),
                                            FVonDaGiao = x.Sum(rpt => rpt.FVonDaGiao),
                                            FTongVonBoTri = x.Sum(rpt => rpt.FTongVonBoTri),
                                            FGiaTriKeHoach = x.Sum(rpt => rpt.FGiaTriKeHoach),
                                            FVonBoTri = x.Sum(rpt => rpt.FVonBoTri)
                                        }).ToList();
                return listGroup;
            }
            catch (Exception ex)
            {
                return new List<KH5NDDPrintDataExportModel>();
            }
        }

        private void CalculateParent(KH5NDDPrintDataExportModel currentItem, KH5NDDPrintDataExportModel seftItem, List<KH5NDDPrintDataExportModel> ItemsReport)
        {
            var parrentItem = ItemsReport.Where(x => x.IdLoaiCongTrinh == currentItem.IdLoaiCongTrinhParent).FirstOrDefault();
            if (parrentItem == null) return;
            parrentItem.FHanMucDauTu += seftItem.FHanMucDauTu;
            parrentItem.FGiaTriKeHoach += seftItem.FGiaTriKeHoach;
            parrentItem.FVonDaGiao += seftItem.FVonDaGiao;
            parrentItem.FVonBoTri += seftItem.FVonBoTri;
            parrentItem.FTongVonBoTri += seftItem.FTongVonBoTri;
            CalculateParent(parrentItem, seftItem, ItemsReport);
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        private List<KeHoach5NamDeXuatModel> CreateVoucherTypes()
        {
            List<KeHoach5NamDeXuatModel> lstVoucherTypes = new List<KeHoach5NamDeXuatModel>()
            {
                new KeHoach5NamDeXuatModel(){SVoucherTypes = "Khởi công mới", iLoai = 1},
                new KeHoach5NamDeXuatModel(){SVoucherTypes = "Chuyển tiếp", iLoai = 2}
            };

            return lstVoucherTypes;
        }

        private List<DonViTinhModel> LoadDonViTinh()
        {
            List<DonViTinhModel> lstDonViTinh = new List<DonViTinhModel>()
            {
                new DonViTinhModel(){DisplayItem = TRIEU_DONG, ValueItem = TRIEU_VALUE},
                new DonViTinhModel(){DisplayItem = DONG, ValueItem = DONG_VALUE},
                new DonViTinhModel(){DisplayItem = NGHIN_DONG, ValueItem = NGHIN_DONG_VALUE},
                new DonViTinhModel(){DisplayItem = TY_DONG, ValueItem = TY_VALUE}
            };
            return lstDonViTinh;
        }
        #endregion
    }
}