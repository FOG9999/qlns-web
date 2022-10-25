using DapperExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.DanhMuc.Models;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Controllers;
using VIETTEL.Models;
using System.Data;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.NganSachQuocPhong
{
    public class DuAnKeHoachTrungHanController : AppController
    {
        IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        IDanhMucService _danhMucService = DanhMucService.Default;
        // GET: QLVonDauTu/DuAnKeHoachTrungHan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewDanhSachDuAn(Guid? id)
        {
            VDT_KHV_KeHoach5Nam kh5ndd = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(id);
            List<VDT_KHV_KeHoach5Nam_DeXuat> data = new List<VDT_KHV_KeHoach5Nam_DeXuat>();

            if (kh5ndd != null)
            {
                data = _iQLVonDauTuService.GetAllKeHoachTrungHanDeXuatByCondition(kh5ndd.iNamLamViec.Value, kh5ndd.iGiaiDoanTu, kh5ndd.iGiaiDoanDen, kh5ndd.iID_DonViQuanLyID).ToList();
                if (data == null)
                {
                    data = new List<VDT_KHV_KeHoach5Nam_DeXuat>();
                }
            }

            DuAnKH5NDuocDuyetGridViewModel vm = new DuAnKH5NDuocDuyetGridViewModel();
            DuAnKH5NDXGridViewModel DuAnKH5NDX = new DuAnKH5NDXGridViewModel
            {
                KH5NamDeXuat = new VDT_KHV_KeHoach5Nam_DeXuat(),
                iID_KeHoach5NamID = id
            };
            DanhMucDuAnGridViewModel DanhMucDuAn = new DanhMucDuAnGridViewModel
            {
                iID_KeHoach5NamID = id
            };
            vm.DuAnKH5NDX = DuAnKH5NDX;
            vm.DanhMucDuAn = DanhMucDuAn;
            ViewBag.ListKhthDxTongHop = data.ToSelectList("iID_KeHoach5Nam_DeXuatID", "sSoQuyetDinh");
            TempData["isAddDuAn"] = false;
            TempData["IdPageReference"] = id;
            TempData["isAddDxProject"] = false;
            return View(vm);
        }

        public ActionResult SheetFrameDuAnKH5NDX(string id, string filter = null)
        {
            var isAddDxProject = (bool)TempData["isAddDxProject"];
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            VDT_KHV_KeHoach5Nam_DeXuat KH5NamDX = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(Guid.Parse(id));
            var sheet = new DuAn_KeHoach5NamDeXuat_ChiTiet_SheetTable(id, int.Parse(PhienLamViec.iNamLamViec), KH5NamDX != null ? KH5NamDX.iGiaiDoanTu : DateTime.Now.Year, filters, (DataTable)TempData["currentListKHV5NamChiTiet"]);
            TempData.Keep();
            var vm = new KeHoach5NamDeXuatChiTietGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("SaveDuAnKH5NDX", "DuAnKeHoachTrungHan", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrameDuAnKH5NDX", "DuAnKeHoachTrungHan", new { area = "QLVonDauTu" })
                   ),
                isAddDxProject = isAddDxProject
            };

            TempData["isAddDxProject"] = false;
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrameDuAnKH5NDX", vm);
        }

        [HttpPost]
        public ActionResult SaveDuAnKH5NDX(SheetEditViewModel vm)
        {
            try
            {
                //var rows = vm.Rows.ToList();
                var rows = vm.Rows.Where(x => x.Columns.ContainsKey("isMap")).ToList();
                var rowsDuAn = rows.Where(x => x.Columns["iLevel"] == "2").ToList();
                var rowsChiTiet = rows.Where(x => x.Columns["iLevel"] == "3").ToList();
                List<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> lstProject = new List<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>();
                List<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> lstDetailProject = new List<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>();

                if (rowsDuAn.Count > 0)
                {
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();

                        #region crud
                        var columns = new DuAn_KeHoach5NamDeXuat_ChiTiet_SheetTable().Columns.Where(x => !x.IsReadonly);
                        var idKhddParent = TempData["IdPageReference"] != null ? (Guid)TempData["IdPageReference"] : Guid.Empty;

                        var itemDdQuery = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(idKhddParent);

                        rowsDuAn.ForEach(r =>
                        {
                            //var trans = conn.BeginTransaction();
                            var Id = r.Id;
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var rowsChiTietCuaDuAn = rowsChiTiet.Where(x => x.Columns["iIDReference"] == Id).ToList();
                            VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet itemDuAnDx = new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet();
                            if (changes.Any())
                            {
                                #region VDT_DA_DuAn
                                var trans = conn.BeginTransaction();
                                var entityKH5NDXCT = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(Id, trans);
                                itemDuAnDx = entityKH5NDXCT;
                                VDT_DA_DuAn entityDuAn = _iQLVonDauTuService.GetVDTDuAnByIdKH5NDXCT(Id);

                                if (entityDuAn == null)
                                {
                                    //Them moi VDT_DA_DuAn
                                    entityDuAn = new VDT_DA_DuAn()
                                    {
                                        sUserCreate = Username,
                                        dDateCreate = DateTime.Now
                                    };
                                    entityDuAn.fHanMucDauTu = entityKH5NDXCT.fHanMucDauTu;
                                    entityDuAn.sTenDuAn = entityKH5NDXCT.sTen;
                                    entityDuAn.sKhoiCong = entityKH5NDXCT.iGiaiDoanTu.ToString();
                                    entityDuAn.sKetThuc = entityKH5NDXCT.iGiaiDoanDen.ToString();
                                    entityDuAn.sDiaDiem = entityKH5NDXCT.sDiaDiem;
                                    entityDuAn.iID_DuAnKHTHDeXuatID = entityKH5NDXCT.iID_KeHoach5Nam_DeXuat_ChiTietID;
                                    entityDuAn.sTrangThaiDuAn = "KhoiTao";
                                    int iMaDuAnIndex = _iQLVonDauTuService.GetMaxMaDuAnIndex() + 1;
                                    entityDuAn.iMaDuAnIndex = iMaDuAnIndex;
                                    entityDuAn.iID_DonViThucHienDuAnID = entityKH5NDXCT.iID_DonViQuanLyID;
                                    entityDuAn.iID_MaDonViThucHienDuAnID = entityKH5NDXCT.iID_MaDonVi;
                                    entityDuAn.iID_MaDonVi = itemDdQuery != null ? itemDdQuery.iID_MaDonViQuanLy : string.Empty;
                                    entityDuAn.iID_LoaiCongTrinhID = entityKH5NDXCT.iID_LoaiCongTrinhID;

                                    VDT_DM_DonViThucHienDuAn donVi = _danhMucService.GetDonViThucHienDuAn(entityDuAn.iID_MaDonViThucHienDuAnID);

                                    string strMaDuAnIndex = getStrMaDuAnIndex(iMaDuAnIndex);
                                    string sMaDuAn = donVi.iID_MaDonVi + "-xxx-" + strMaDuAnIndex;
                                    entityDuAn.sMaDuAn = sMaDuAn;
                                    conn.Insert(entityDuAn, trans);
                                    if (itemDuAnDx.iID_LoaiCongTrinhID != null && itemDuAnDx.iID_NguonVonID != null)
                                    {
                                        VDT_DA_DuAn_NguonVon entityDuAnNguonVon = _iQLVonDauTuService.GetVDTDuAnNguonVon(entityDuAn.iID_DuAnID, itemDuAnDx.iID_NguonVonID.Value);
                                        if (entityDuAnNguonVon == null)
                                        {
                                            // Them moi VDT_DA_DuAn_NguonVon
                                            entityDuAnNguonVon = new VDT_DA_DuAn_NguonVon();
                                            entityDuAnNguonVon.iID_DuAn = entityDuAn.iID_DuAnID;
                                            entityDuAnNguonVon.iID_NguonVonID = itemDuAnDx.iID_NguonVonID;
                                           
                                            entityDuAnNguonVon.fThanhTien = entityKH5NDXCT.fHanMucDauTu;
                                            conn.Insert(entityDuAnNguonVon, trans);
                                        }
                                        else
                                        {
                                            entityDuAnNguonVon.fThanhTien = entityKH5NDXCT.fHanMucDauTu;
                                            conn.Update(entityDuAnNguonVon, trans);
                                        }                                     
                                    }
                                }
                                else
                                {
                                    entityDuAn.sUserUpdate = Username;
                                    entityDuAn.dDateUpdate = DateTime.Now;
                                    entityDuAn.fHanMucDauTu = entityKH5NDXCT.fHanMucDauTu;
                                    entityDuAn.sTenDuAn = entityKH5NDXCT.sTen;
                                    entityDuAn.sKhoiCong = entityKH5NDXCT.iGiaiDoanTu.ToString();
                                    entityDuAn.sKetThuc = entityKH5NDXCT.iGiaiDoanDen.ToString();
                                    entityDuAn.sDiaDiem = entityKH5NDXCT.sDiaDiem;
                                    entityDuAn.iID_DonViThucHienDuAnID = entityKH5NDXCT.iID_DonViQuanLyID;
                                    entityDuAn.iID_MaDonViThucHienDuAnID = entityKH5NDXCT.iID_MaDonVi;
                                    entityDuAn.iID_MaDonVi = itemDdQuery != null ? itemDdQuery.iID_MaDonViQuanLy : string.Empty;
                                    entityDuAn.iID_LoaiCongTrinhID = entityKH5NDXCT.iID_LoaiCongTrinhID;

                                    VDT_DM_DonViThucHienDuAn donVi = _danhMucService.GetDonViThucHienDuAn(entityDuAn.iID_MaDonViThucHienDuAnID);

                                    string sMaDuAn = entityDuAn.sMaDuAn;
                                    var arrMaDuAn = sMaDuAn.Split(new string[] { "-" }, StringSplitOptions.None);
                                    string sMaDuAnNew = donVi.iID_MaDonVi + "-" + arrMaDuAn[1] + "-" + arrMaDuAn[2];
                                    entityDuAn.sMaDuAn = sMaDuAn;
                                    conn.Update(entityDuAn, trans);
                                    if (itemDuAnDx.iID_LoaiCongTrinhID != null && itemDuAnDx.iID_NguonVonID != null)
                                    {
                                        VDT_DA_DuAn_NguonVon entityDuAnNguonVon = _iQLVonDauTuService.GetVDTDuAnNguonVon(entityDuAn.iID_DuAnID, itemDuAnDx.iID_NguonVonID.Value);
                                        if (entityDuAnNguonVon == null)
                                        {
                                            // Them moi VDT_DA_DuAn_NguonVon
                                            entityDuAnNguonVon = new VDT_DA_DuAn_NguonVon();
                                            entityDuAnNguonVon.iID_DuAn = entityDuAn.iID_DuAnID;
                                            entityDuAnNguonVon.iID_NguonVonID = itemDuAnDx.iID_NguonVonID;                                          
                                            entityDuAnNguonVon.fThanhTien = entityKH5NDXCT.fHanMucDauTu;
                                            conn.Insert(entityDuAnNguonVon, trans);
                                        }
                                        else
                                        {                                       
                                            entityDuAnNguonVon.fThanhTien = entityKH5NDXCT.fHanMucDauTu;
                                            conn.Update(entityDuAnNguonVon, trans);
                                        }                                    
                                    }
                                }
                                itemDuAnDx.iID_DuAnID = entityDuAn.iID_DuAnID;
                                lstProject.Add(itemDuAnDx);
                                trans.Commit();
                                #endregion

                                var mapCTDAHangMuc = new Dictionary<string, string>();
                                rowsChiTietCuaDuAn.ForEach(rCT =>
                                {
                                    #region VDT_DA_DuAn_HangMuc
                                    trans = conn.BeginTransaction();
                                    var IdCTDuAn = rCT.Id;
                                    VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet entityChiTietDuAn = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(IdCTDuAn, trans);
                                    var itemChiTiet = entityChiTietDuAn;
                                    itemChiTiet.iID_DuAnID = entityDuAn.iID_DuAnID;
                                    lstDetailProject.Add(itemChiTiet);

                                    var loaiCongTrinh = _iQLVonDauTuService.GetDMLoaiCongTrinhById(entityChiTietDuAn.iID_LoaiCongTrinhID.Value);
                                    VDT_DA_DuAn_HangMuc entityDuAnHangMuc = _iQLVonDauTuService.GetVDTDuAnHangMuc(entityDuAn.iID_DuAnID, entityChiTietDuAn.iID_LoaiCongTrinhID.Value, entityChiTietDuAn.iID_NguonVonID.Value);
                                    if (entityDuAnHangMuc == null)
                                    {
                                        // Them moi VDT_DA_DuAn_HangMuc
                                        entityDuAnHangMuc = new VDT_DA_DuAn_HangMuc();
                                        entityDuAnHangMuc.iID_DuAnID = entityDuAn.iID_DuAnID;
                                        entityDuAnHangMuc.sMoTa = entityChiTietDuAn.sGhiChu;
                                        entityDuAnHangMuc.iID_NguonVonID = entityChiTietDuAn.iID_NguonVonID;
                                        entityDuAnHangMuc.iID_LoaiCongTrinhID = entityChiTietDuAn.iID_LoaiCongTrinhID;
                                        entityDuAnHangMuc.fHanMucDauTu = entityChiTietDuAn.fHanMucDauTu;
                                        int indexMaHangMuc = _iQLVonDauTuService.GetMaxIndexMaHangMuc() + 1;
                                        entityDuAnHangMuc.indexMaHangMuc = indexMaHangMuc;
                                        string strIndexDuAn = getStrMaDuAnIndex(entityDuAn.iMaDuAnIndex.Value);
                                        string strIndexHangMuc = getStrMaDuAnIndex(indexMaHangMuc);
                                        entityDuAnHangMuc.sMaHangMuc = strIndexDuAn + strIndexHangMuc;
                                        entityDuAnHangMuc.sTenHangMuc = entityDuAn.sTenDuAn + " - " + loaiCongTrinh.sTenLoaiCongTrinh;
                                        string mapValue;
                                        if (mapCTDAHangMuc.TryGetValue(entityChiTietDuAn.iID_ParentID.ToString(), out mapValue))
                                        {
                                            Console.WriteLine(mapValue);
                                            entityDuAnHangMuc.iID_ParentID = Guid.Parse(mapValue);
                                        }
                                        conn.Insert(entityDuAnHangMuc, trans);

                                        mapCTDAHangMuc.Add(IdCTDuAn, entityDuAnHangMuc.iID_DuAn_HangMucID.ToString());
                                    }
                                    else
                                    {
                                        entityDuAnHangMuc.sMoTa = entityChiTietDuAn.sGhiChu;
                                        entityDuAnHangMuc.fHanMucDauTu = entityChiTietDuAn.fHanMucDauTu;
                                        entityDuAnHangMuc.sTenHangMuc = entityDuAn.sTenDuAn + " - " + loaiCongTrinh.sTenLoaiCongTrinh;
                                        conn.Update(entityDuAnHangMuc, trans);
                                        mapCTDAHangMuc.Add(IdCTDuAn, entityDuAnHangMuc.iID_DuAn_HangMucID.ToString());
                                    }
                                    trans.Commit();
                                    #endregion

                                    #region VDT_DA_DuAn_NguonVon
                                    trans = conn.BeginTransaction();
                                    VDT_DA_DuAn_NguonVon entityDuAnNguonVon = _iQLVonDauTuService.GetVDTDuAnNguonVon(entityDuAn.iID_DuAnID, entityChiTietDuAn.iID_NguonVonID.Value);
                                    if (entityDuAnNguonVon == null)
                                    {
                                        // Them moi VDT_DA_DuAn_NguonVon
                                        entityDuAnNguonVon = new VDT_DA_DuAn_NguonVon();
                                        entityDuAnNguonVon.iID_DuAn = entityDuAn.iID_DuAnID;
                                        entityDuAnNguonVon.iID_NguonVonID = entityChiTietDuAn.iID_NguonVonID;
                                        double sThanhTien = _iQLVonDauTuService.GetTongHanMucDauTuNguonVon(entityDuAn.iID_DuAnID, entityChiTietDuAn.iID_NguonVonID.Value);
                                        entityDuAnNguonVon.fThanhTien = sThanhTien;
                                        conn.Insert(entityDuAnNguonVon, trans);
                                    }
                                    else
                                    {
                                        double sThanhTien = _iQLVonDauTuService.GetTongHanMucDauTuNguonVon(entityDuAn.iID_DuAnID, entityChiTietDuAn.iID_NguonVonID.Value);
                                        entityDuAnNguonVon.fThanhTien = sThanhTien;
                                        conn.Update(entityDuAnNguonVon, trans);
                                    }
                                    trans.Commit();
                                    #endregion
                                });

                                #region VDT_KHV_KeHoach5Nam_ChiTiet
                                List<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> lstInsert = lstDetailProject;
                                var lstItem = lstProject.Where(x => !lstDetailProject.Any(y => y.iIDReference == x.iID_KeHoach5Nam_DeXuat_ChiTietID)).ToList();
                                lstInsert.AddRange(lstItem);

                                List<VDT_KHV_KeHoach5Nam_ChiTiet> lstKhthDetail = _iQLVonDauTuService.GetListKH5NamByIdKHDD(idKhddParent).ToList();
                                lstInsert = lstInsert.Where(item => !lstKhthDetail.Any(x => x.iID_DuAnID == item.iID_DuAnID && x.iID_LoaiCongTrinhID == item.iID_LoaiCongTrinhID
                                    && x.iID_DonViQuanLyID == item.iID_DonViQuanLyID && x.iID_NguonVonID == item.iID_NguonVonID)).ToList();

                                /* KHÔNG THÊM NGAY VÀO DB MỘT CHI TIẾT MỚI VÌ CHƯA LƯU
                                //Them moi VDT_KHV_KeHoach5Nam_ChiTiet
                                lstInsert.Select(item =>
                                {
                                    trans = conn.BeginTransaction();
                                    var entityKH5NChiTiet = new VDT_KHV_KeHoach5Nam_ChiTiet();
                                    entityKH5NChiTiet.iID_KeHoach5NamID = idKhddParent;
                                    entityKH5NChiTiet.iID_DuAnID = item.iID_DuAnID.Value;
                                    entityKH5NChiTiet.iID_DonViQuanLyID = item.iID_DonViQuanLyID;
                                    entityKH5NChiTiet.sTen = item.sTen;
                                    entityKH5NChiTiet.iID_NguonVonID = item.iID_NguonVonID;
                                    entityKH5NChiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
                                    entityKH5NChiTiet.fHanMucDauTu = item.fHanMucDauTu;
                                    entityKH5NChiTiet.iID_MaDonVi = item.iID_MaDonVi;

                                    conn.Insert(entityKH5NChiTiet, trans);
                                    trans.Commit();

                                    return item;
                                }).ToList();
                                */
                                List<KeHoach5NamChiTietDuocDuyetTempForSave> list = new List<KeHoach5NamChiTietDuocDuyetTempForSave>();
                                lstInsert.ForEach(item =>
                                {
                                    KeHoach5NamChiTietDuocDuyetTempForSave entityKH5NChiTiet = createTempSaveFromKHVDeXuatChiTiet(item);
                                    list.Add(entityKH5NChiTiet);
                                });
                                TempData["listNewKHV5NamChiTietNew"] = list;

                                #endregion
                            }
                        });
                        #endregion
                    }
                }

                // clear cache
                return RedirectToAction("SheetFrameDuAnKH5NDX", new { vm.Id, vm.Option, filters = vm.FiltersString });
            }
            catch (NullReferenceException e)
            {
                return RedirectToAction("SheetFrameDuAnKH5NDX", new { vm.Id, vm.Option, filters = vm.FiltersString });
            }
        }

        private KeHoach5NamChiTietDuocDuyetTempForSave createTempSaveFromKHVDeXuatChiTiet(VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet item)
        {
            var idKhddParent = TempData["IdPageReference"] != null ? (Guid)TempData["IdPageReference"] : Guid.Empty;
            var entityKH5NChiTiet = new KeHoach5NamChiTietDuocDuyetTempForSave();
            entityKH5NChiTiet.iID_KeHoach5NamID = idKhddParent;
            entityKH5NChiTiet.iID_DuAnID = item.iID_DuAnID.Value;
            entityKH5NChiTiet.iID_DonViQuanLyID = item.iID_DonViQuanLyID;
            entityKH5NChiTiet.sTen = item.sTen;
            entityKH5NChiTiet.iID_NguonVonID = item.iID_NguonVonID;
            entityKH5NChiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
            entityKH5NChiTiet.fHanMucDauTu = item.fHanMucDauTu;
            entityKH5NChiTiet.iID_MaDonVi = item.iID_MaDonVi;
            entityKH5NChiTiet.iID_KeHoach5Nam_ChiTietID = Guid.NewGuid();
            entityKH5NChiTiet.sDiaDiem = item.sDiaDiem;
            entityKH5NChiTiet.sThoiGianThucHien = item.iGiaiDoanTu.ToString() + " - " + item.iGiaiDoanDen.ToString();
            return entityKH5NChiTiet;
        }

        public string getStrMaDuAnIndex(int iMaDuAnIndex)
        {
            if (iMaDuAnIndex < 10)
            {
                return "000" + iMaDuAnIndex;
            }
            else if (iMaDuAnIndex < 100)
            {
                return "00" + iMaDuAnIndex;
            }
            else if (iMaDuAnIndex < 1000)
            {
                return "0" + iMaDuAnIndex;
            }
            return iMaDuAnIndex.ToString();
        }

        public ActionResult SheetFrameDMDuAn(string id, string filter = null)
        {
            var isAddDuAn = (bool)TempData["isAddDuAn"];
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            VDT_KHV_KeHoach5Nam KH5Nam = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(Guid.Parse(id));
            var sheet = new DanhMucDuAn_KH5NDD_SheetTable(id, int.Parse(PhienLamViec.iNamLamViec), KH5Nam.iGiaiDoanTu, filters, (DataTable)TempData["currentListKHV5NamChiTiet"]);
            TempData.Keep();
            var vm = new DanhMucDuAnGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("SaveDMDuAn", "DuAnKeHoachTrungHan", new { area = "QLVonDauTu" }),
                   urlGet: Url.Action("SheetFrameDMDuAn", "DuAnKeHoachTrungHan", new { area = "QLVonDauTu" })
                   ),
                isAddDuAn = isAddDuAn,
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            if(TempData["isRedirectToKHTHDD"] != null)
            {
                TempData.Keep();
                vm.isRedirectToKHTHDD = (bool)TempData["isRedirectToKHTHDD"];
                TempData["isRedirectToKHTHDD"] = false;
            }            
            TempData["isAddDuAn"] = false;
            return View("_sheetFrameDMDuAn", vm);
        }

        [HttpPost]
        public ActionResult SaveDMDuAn(SheetEditViewModel vm)
        {
            var isAddDuAn = (bool)TempData["isAddDuAn"];
            TempData["isRedirectToKHTHDD"] = true;

            if (!isAddDuAn)
            {
                saveDuAnAndHangMuc(vm);
            }
            else
            {
                addDuAnKH5NDD(vm);
            }

            // clear cache
            return RedirectToAction("SheetFrameDMDuAn", new { vm.Id, filters = vm.FiltersString });
        }

        public void addDuAnKH5NDD(SheetEditViewModel vm)
        {
            var rows = vm.Rows.Where(x => x.Columns.ContainsKey("isMap") && x.Columns["isMap"] == "1").ToList();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var iID_KeHoach5NamID = vm.Id;
                    rows.ForEach(r =>
                    {
                        var trans = conn.BeginTransaction();
                        var iID_DuAn_HangMucID = r.Id;
                        var entityDuAnHangMuc = conn.Get<VDT_DA_DuAn_HangMuc>(iID_DuAn_HangMucID, trans);
                        var entityDuAn = conn.Get<VDT_DA_DuAn>(entityDuAnHangMuc.iID_DuAnID, trans);

                        //Them moi VDT_KHV_KeHoach5Nam_ChiTiet
                        var entityKH5NChiTiet = new VDT_KHV_KeHoach5Nam_ChiTiet();
                        entityKH5NChiTiet.iID_KeHoach5NamID = Guid.Parse(iID_KeHoach5NamID);
                        entityKH5NChiTiet.iID_DuAnID = entityDuAn.iID_DuAnID;
                        entityKH5NChiTiet.iID_DonViQuanLyID = entityDuAn.iID_DonViQuanLyID;
                        entityKH5NChiTiet.sTen = entityDuAn.sTenDuAn;
                        entityKH5NChiTiet.iID_NguonVonID = entityDuAnHangMuc.iID_NguonVonID;
                        entityKH5NChiTiet.iID_LoaiCongTrinhID = entityDuAnHangMuc.iID_LoaiCongTrinhID;
                        entityKH5NChiTiet.fHanMucDauTu = entityDuAnHangMuc.fHanMucDauTu;

                        conn.Insert(entityKH5NChiTiet, trans);

                        // commit to db
                        trans.Commit();
                    });
                }
            }
        }
        public void saveDuAnAndHangMuc(SheetEditViewModel vm)
        {
            //var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            var rows = vm.Rows.Where(x => x.Columns.ContainsKey("isMap")).ToList();
            var rowsDuAn = rows.Where(x => x.Columns["iLevel"] == "1").ToList();
            var rowsChiTiet = rows.Where(x => x.Columns["iLevel"] == "2").ToList();
            var rowsDelete = vm.Rows.Where(x => x.IsDeleted).ToList();

            if (rowsDelete.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    #region delete
                    rowsDelete.ForEach(rDel =>
                    {
                        var trans = conn.BeginTransaction();
                        var Id = rDel.Id;
                        var entityDuAn = conn.Get<VDT_DA_DuAn>(Id, trans);
                        if (entityDuAn != null)
                        {
                            conn.Delete(entityDuAn, trans);
                        }

                        var entityHangMuc = conn.Get<VDT_DA_DuAn_HangMuc>(Id, trans);
                        if (entityHangMuc != null)
                        {
                            conn.Delete(entityHangMuc, trans);
                        }
                        // commit to db
                        trans.Commit();
                    });
                    #endregion
                }
            }

            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    #region save VDT_DA_DuAn
                    var columns = new DanhMucDuAn_KH5NDD_SheetTable().Columns.Where(x => !x.IsReadonly);
                    // Lưu tạm chứ k lưu vào DB
                    List<KeHoach5NamChiTietDuocDuyetTempForSave> listTempData = new List<KeHoach5NamChiTietDuocDuyetTempForSave>();
                    rowsDuAn.ForEach(r =>
                    {
                        var trans = conn.BeginTransaction();
                        var iID_DuAnID = r.Id;
                        //Edit hang cha
                        if (r.IsParent)
                        {
                            iID_DuAnID = r.Columns["iID_DuAnID"];
                        }
                        var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                        if (changes.Any())
                        {
                            string sMaDonVi = "";
                            NS_DonVi donvi = null;
                            string sId_CDT = "";
                            DM_ChuDauTu chudautu = null;
                            if (r.Columns.ContainsKey("sTenDonViQL"))
                            {
                                sMaDonVi = r.Columns["sTenDonViQL"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                donvi = _iNganSachService.GetDonViByMaDonVi(PhienLamViec.iNamLamViec, sMaDonVi);
                            }
                            if (r.Columns.ContainsKey("sTenCDT"))
                            {
                                sId_CDT = r.Columns["sTenCDT"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                chudautu = _iQLVonDauTuService.GetChuDauTuByMaCDT(PhienLamViec.NamLamViec, sId_CDT);
                            }

                            var isNew = string.IsNullOrWhiteSpace(iID_DuAnID);
                            if (isNew)
                            {
                                //Them moi VDT_DA_DuAn
                                var entityDuAn = new VDT_DA_DuAn()
                                {
                                    sUserCreate = Username,
                                    dDateCreate = DateTime.Now,
                                    sTrangThaiDuAn = "KHTH"
                                };
                                if (r.Columns.ContainsKey("sTenDonViQL"))
                                {
                                    entityDuAn.iID_DonViQuanLyID = donvi?.iID_Ma;
                                    entityDuAn.iID_MaDonViThucHienDuAnID = sMaDonVi;
                                    entityDuAn.iID_DonViThucHienDuAnID = donvi?.iID_Ma;
                                }
                                if (r.Columns.ContainsKey("sTenCDT"))
                                {
                                    entityDuAn.iID_ChuDauTuID = chudautu?.ID;
                                    entityDuAn.iID_MaCDT = sId_CDT;
                                }

                                entityDuAn.MapFrom(changes);
                                int iMaDuAnIndex = _iQLVonDauTuService.GetMaxMaDuAnIndex() + 1;
                                entityDuAn.iMaDuAnIndex = iMaDuAnIndex;

                                string sMaDuAn = entityDuAn.sMaDuAn;
                                var arrMaDuAn = sMaDuAn.Split(new string[] { "-" }, StringSplitOptions.None);
                                string strMaDuAnIndex = getStrMaDuAnIndex(iMaDuAnIndex);
                                string sMaDuAnNew = arrMaDuAn[0] + "-" + arrMaDuAn[1] + "-" + strMaDuAnIndex;
                                entityDuAn.sMaDuAn = sMaDuAnNew;

                                KeHoach5NamChiTietDuocDuyetTempForSave kehoachChiTiet = createKHV5NamChiTietFromDuAn(entityDuAn, null);

                                listTempData.Add(kehoachChiTiet);

                                // conn.Insert(kehoachChiTiet, trans);
                                conn.Insert(entityDuAn, trans);
                            }
                            else
                            {
                                var entityDuAn = _iQLVonDauTuService.GetDuAnByIdDuAn(Guid.Parse(iID_DuAnID));
                                entityDuAn.sUserUpdate = Username;
                                entityDuAn.dDateUpdate = DateTime.Now;
                                if (r.Columns.ContainsKey("sTenDonViQL"))
                                {
                                    entityDuAn.iID_DonViQuanLyID = donvi?.iID_Ma;
                                }
                                if (r.Columns.ContainsKey("sTenCDT"))
                                {
                                    entityDuAn.iID_ChuDauTuID = chudautu?.ID;
                                }
                                entityDuAn.MapFrom(changes);
                                KeHoach5NamChiTietDuocDuyetTempForSave kehoachChiTiet = createKHV5NamChiTietFromDuAn(entityDuAn, null);

                                listTempData.Add(kehoachChiTiet);

                                // conn.Insert(kehoachChiTiet, trans);
                                conn.Update(entityDuAn, trans);
                            }
                        }
                        // commit to db
                        trans.Commit();
                    });

                    
                    #endregion

                    #region save Chi tiết dự án
                    rowsChiTiet.ForEach(rCT =>
                    {
                        #region VDT_DA_DuAn_HangMuc, VDT_DA_DuAn_NguonVon
                        var trans = conn.BeginTransaction();
                        var iID_DuAn_HangMucID = rCT.Id;
                        string iID_DuAnID = rCT.Columns["iID_DuAnID"];
                        var entityDuAn = _iQLVonDauTuService.GetDuAnByIdDuAn(Guid.Parse(iID_DuAnID));
                        var changes = rCT.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                        if (changes.Any())
                        {
                            int? iID_NguonVonID = 0;
                            string sMaLoaiCongTrinh = "";
                            VDT_DM_LoaiCongTrinh congtrinh = null;
                            if (rCT.Columns.ContainsKey("sTenLoaiCongTrinh"))
                            {
                                sMaLoaiCongTrinh = rCT.Columns["sTenLoaiCongTrinh"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                congtrinh = _iQLVonDauTuService.GetDMLoaiCongTrinhByMa(sMaLoaiCongTrinh);
                            }
                            if (rCT.Columns.ContainsKey("sTenNganSach"))
                            {
                                if (!string.IsNullOrEmpty(rCT.Columns["sTenNganSach"]))
                                {
                                    string strId = rCT.Columns["sTenNganSach"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                    iID_NguonVonID = Int16.Parse(strId);
                                }

                            }
                            string sMaDonVi = "";
                            NS_DonVi donvi = null;
                            string sId_CDT = "";
                            DM_ChuDauTu chudautu = null;
                            if (rCT.Columns.ContainsKey("sTenDonViQL"))
                            {
                                entityDuAn.iID_DonViQuanLyID = donvi?.iID_Ma;
                                entityDuAn.iID_MaDonViThucHienDuAnID = sMaDonVi;
                                entityDuAn.iID_DonViThucHienDuAnID = donvi?.iID_Ma;
                                sMaDonVi = rCT.Columns["sTenDonViQL"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                donvi = _iNganSachService.GetDonViByMaDonVi(PhienLamViec.iNamLamViec, sMaDonVi);
                            }
                            if (rCT.Columns.ContainsKey("sTenCDT"))
                            {
                                entityDuAn.iID_ChuDauTuID = chudautu?.ID;
                                entityDuAn.iID_MaCDT = sId_CDT;
                                sId_CDT = rCT.Columns["sTenCDT"].Split(new string[] { "-" }, StringSplitOptions.None)[0];
                                chudautu = _iQLVonDauTuService.GetChuDauTuByMaCDT(PhienLamViec.NamLamViec, sId_CDT);
                            }

                            // lưu VDT_DA_DuAn_HangMuc
                            var entityDuAnHangMuc = new VDT_DA_DuAn_HangMuc();
                            var isNew = string.IsNullOrWhiteSpace(iID_DuAn_HangMucID);
                            if (isNew)
                            {
                                entityDuAnHangMuc.iID_DuAnID = entityDuAn.iID_DuAnID;
                                if (rCT.Columns.ContainsKey("sTenLoaiCongTrinh"))
                                {
                                    entityDuAnHangMuc.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                    entityDuAn.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                }
                                if (rCT.Columns.ContainsKey("sTenNganSach"))
                                {
                                    entityDuAnHangMuc.iID_NguonVonID = iID_NguonVonID > 0 ? iID_NguonVonID : null;
                                }
                                entityDuAnHangMuc.MapFrom(changes);
                                int indexMaHangMuc = _iQLVonDauTuService.GetMaxIndexMaHangMuc() + 1;
                                entityDuAnHangMuc.indexMaHangMuc = indexMaHangMuc;
                                string strIndexDuAn = getStrMaDuAnIndex(entityDuAn.iMaDuAnIndex.Value);
                                string strIndexHangMuc = getStrMaDuAnIndex(indexMaHangMuc);
                                entityDuAnHangMuc.sMaHangMuc = strIndexDuAn + strIndexHangMuc;
                                entityDuAnHangMuc.sTenHangMuc = entityDuAn.sTenDuAn + " - " + congtrinh.sTenLoaiCongTrinh;
                                conn.Insert(entityDuAnHangMuc, trans);
                            }
                            else
                            {
                                entityDuAnHangMuc = _iQLVonDauTuService.GetVDTDuAnHangMucById(Guid.Parse(iID_DuAn_HangMucID));
                                if (rCT.Columns.ContainsKey("sTenLoaiCongTrinh"))
                                {
                                    entityDuAnHangMuc.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                    entityDuAnHangMuc.sTenHangMuc = entityDuAn.sTenDuAn + " - " + congtrinh.sTenLoaiCongTrinh;
                                    entityDuAn.iID_LoaiCongTrinhID = congtrinh?.iID_LoaiCongTrinh;
                                }
                                if (rCT.Columns.ContainsKey("sTenNganSach"))
                                {
                                    entityDuAnHangMuc.iID_NguonVonID = iID_NguonVonID > 0 ? iID_NguonVonID : null;
                                }
                                entityDuAnHangMuc.MapFrom(changes);
                                conn.Update(entityDuAnHangMuc, trans);
                            }
                            trans.Commit();
                            _iQLVonDauTuService.updateHanMucDauTuDuAn(entityDuAn.iID_DuAnID);

                            trans = conn.BeginTransaction();
                            // lưu VDT_DA_DuAn_NguonVon
                            VDT_DA_DuAn_NguonVon entityDuAnNguonVon = _iQLVonDauTuService.GetVDTDuAnNguonVon(entityDuAn.iID_DuAnID, entityDuAnHangMuc.iID_NguonVonID.Value);
                            if (entityDuAnNguonVon == null)
                            {
                                entityDuAnNguonVon = new VDT_DA_DuAn_NguonVon();
                                entityDuAnNguonVon.iID_DuAn = entityDuAn.iID_DuAnID;
                                entityDuAnNguonVon.iID_NguonVonID = entityDuAnHangMuc.iID_NguonVonID;
                                double sThanhTien = _iQLVonDauTuService.GetTongHanMucDauTuNguonVon(entityDuAn.iID_DuAnID, entityDuAnHangMuc.iID_NguonVonID.Value);
                                entityDuAnNguonVon.fThanhTien = sThanhTien;
                                conn.Insert(entityDuAnNguonVon, trans);
                            }
                            else
                            {
                                double sThanhTien = _iQLVonDauTuService.GetTongHanMucDauTuNguonVon(entityDuAn.iID_DuAnID, entityDuAnHangMuc.iID_NguonVonID.Value);
                                entityDuAnNguonVon.fThanhTien = sThanhTien;
                                conn.Update(entityDuAnNguonVon, trans);
                            }
                            trans.Commit();
                            KeHoach5NamChiTietDuocDuyetTempForSave kehoachChiTiet = createKHV5NamChiTietFromDuAn(entityDuAn, iID_NguonVonID);

                            listTempData.Add(kehoachChiTiet);
                        }
                        #endregion
                    });

                    TempData["listNewKHV5NamChiTietNew"] = listTempData;
                    #endregion
                }
            }
        }

        // khi chọn dự án trong danh mục dự án, cần lưu vào VDT_KHV_KeHoach5Nam_ChiTiet để mang ra màn Detail của kế hoạch trung hạn được duyệt
        private KeHoach5NamChiTietDuocDuyetTempForSave createKHV5NamChiTietFromDuAn(VDT_DA_DuAn item, int? iID_NguonVonID)
        {
            Guid idKhddParent = (Guid)TempData["IdPageReference"];
            var entityKH5NChiTiet = new KeHoach5NamChiTietDuocDuyetTempForSave();
            entityKH5NChiTiet.iID_KeHoach5NamID = idKhddParent;
            entityKH5NChiTiet.iID_DuAnID = item.iID_DuAnID;
            entityKH5NChiTiet.iID_DonViQuanLyID = item.iID_DonViThucHienDuAnID;
            entityKH5NChiTiet.sTen = item.sTenDuAn;
            entityKH5NChiTiet.iID_NguonVonID = iID_NguonVonID;
            entityKH5NChiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinhID;
            entityKH5NChiTiet.fHanMucDauTu = item.fHanMucDauTu;
            entityKH5NChiTiet.iID_MaDonVi = item.iID_MaDonVi;
            entityKH5NChiTiet.sDiaDiem = item.sDiaDiem;
            entityKH5NChiTiet.sThoiGianThucHien = item.sKhoiCong + " - " + item.sKetThuc;
            return entityKH5NChiTiet;
        }

        [HttpPost]
        public JsonResult setActionAddDxProject(bool isAddDxProject)
        {
            TempData["isAddDxProject"] = isAddDxProject;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult setActionAddDuAn(bool isAddDuAn)
        {
            TempData["isAddDuAn"] = isAddDuAn;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
    }
}