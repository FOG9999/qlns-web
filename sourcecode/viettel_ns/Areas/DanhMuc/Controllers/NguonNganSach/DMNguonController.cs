using DapperExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.DanhMuc.Models;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Controllers
{
    public class DMNguonController : AppController
    {
        private readonly IDanhMucService _dmNguonService = DanhMucService.Default;
        // GET: DanhMuc/DMNguon
        public ActionResult Index()
        {
            DMNguonViewModel vm = new DMNguonViewModel();
            return View(vm);
        }

        #region Data grid
        public ActionResult SheetFrame(string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new DMNguon_SheetTable(int.Parse(PhienLamViec.iNamLamViec), filters);
            var vm = new DMNguonViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "DMNguon", new { area = "DanhMuc" }),
                   urlGet: Url.Action("SheetFrame", "DMNguon", new { area = "DanhMuc" })
                   ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();

                    #region crud
                    var columns = new DMNguon_SheetTable().Columns.Where(x => !x.IsHidden);
                    rows.ForEach(r =>
                    {
                        var trans = conn.BeginTransaction();
                        var iID_Nguon = r.Id;
                        var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                        if (!string.IsNullOrWhiteSpace(iID_Nguon) && r.IsDeleted)
                        {
                            #region delete
                            var entity = conn.Get<DM_Nguon>(iID_Nguon, trans);
                            if (entity != null)
                            {
                                entity.bPublic = false;
                                entity.iTrangThai = 0;
                                entity.dNgaySua = DateTime.Now;
                                entity.sID_MaNguoiDungSua = Username;
                                entity.iSoLanSua = entity.iSoLanSua != null ? entity.iSoLanSua + 1 : 0;
                                entity.sIPSua = Request.UserHostAddress;
                                conn.Update(entity, trans);
                            }
                            #endregion
                        }
                        else if (changes.Any())
                        {
                            string sMaNguonCha = "";
                            DM_Nguon nguonCha = null;
                            if (r.Columns.ContainsKey("sMaNguonCha"))
                            {
                                sMaNguonCha = r.Columns["sMaNguonCha"];
                                nguonCha = _dmNguonService.GetDMNguonByMaNguon(sMaNguonCha, PhienLamViec.NamLamViec);
                            }
                            var isNew = string.IsNullOrWhiteSpace(iID_Nguon);
                            if (isNew)
                            {
                                #region create
                                var entity = new DM_Nguon()
                                {
                                    bPublic = true,
                                    iTrangThai = 1,
                                    iNamLamViec = PhienLamViec.NamLamViec,
                                    dNgayTao = DateTime.Now,
                                    sID_MaNguoiDungTao = Username,
                                };
                                entity.iID_NguonCha = nguonCha?.iID_Nguon;
                                if (r.Columns.ContainsKey("sLoaiNganSach"))
                                {
                                    string iLNS = r.Columns["iLoaiNganSach"];
                                    entity.iLoaiNganSach = string.IsNullOrEmpty(iLNS) ? (int?)null : int.Parse(iLNS);
                                }
                                entity.MapFrom(changes);
                                conn.Insert(entity, trans);
                                #endregion
                            }
                            else
                            {
                                #region edit
                                var entity = conn.Get<DM_Nguon>(iID_Nguon, trans);
                                if (nguonCha != null)
                                    entity.iID_NguonCha = nguonCha?.iID_Nguon;
                                entity.dNgaySua = DateTime.Now;
                                entity.sID_MaNguoiDungSua = Username;
                                entity.iSoLanSua = entity.iSoLanSua != null ? entity.iSoLanSua + 1 : 0;
                                entity.sIPSua = Request.UserHostAddress;
                                if (r.Columns.ContainsKey("iLoaiNganSach"))
                                {
                                    string iLNS = r.Columns["iLoaiNganSach"];
                                    entity.iLoaiNganSach = string.IsNullOrEmpty(iLNS) ? (int?)null : int.Parse(iLNS);
                                }
                                entity.MapFrom(changes);
                                conn.Update(entity, trans);
                                #endregion
                            }
                        }
                        // commit to db
                        trans.Commit();
                    });

                    #endregion
                }
            }

            // clear cache

            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }

        [HttpPost]
        public JsonResult ValidateBeforeSave(List<DMNguonViewModel> aListModel)
        {
            var listErrMess = new List<string>();
            if (aListModel == null || !aListModel.Any())
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }

            var lstDMNguon = _dmNguonService.GetAllDMNguonForCheck(PhienLamViec.NamLamViec);
            if (lstDMNguon != null && lstDMNguon.Any())
            {
                foreach (var item in aListModel)
                {
                    if (item.bDelete == false)
                    {
                        if (aListModel.Where(x => x.sMaNguon == item.sMaNguon && x.bDelete == false).Count() > 1)
                        {
                            listErrMess.Add("Mã nguồn " + item.sMaNguon + " BTC cấp dòng số " + item.STT + " đã tồn tại!");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.iID_Nguon))
                            {
                                var model = lstDMNguon.Where(x => x.sMaNguon == item.sMaNguon).ToList();
                                if (model != null && model.Any())
                                {
                                    if (aListModel.Where(x => model.Select(y => y.iID_Nguon.ToString()).Contains(x.iID_Nguon) && x.bDelete == true).Count() < model.Count())
                                        //add mess error
                                        listErrMess.Add("Mã nguồn " + item.sMaNguon + " BTC cấp dòng số " + item.STT + " đã tồn tại!");
                                }
                            }

                            if (!string.IsNullOrEmpty(item.iID_Nguon))
                            {
                                var model = lstDMNguon.Where(x => x.iID_Nguon != Guid.Parse(item.iID_Nguon) && x.sMaNguon == item.sMaNguon).ToList();
                                if (model != null && model.Any())
                                {
                                    if (aListModel.Where(x => model.Select(y => y.iID_Nguon.ToString()).Contains(x.iID_Nguon) && x.bDelete == true).Count() < model.Count())
                                        //add mess error
                                        listErrMess.Add("Mã nguồn " + item.sMaNguon + " BTC cấp dòng số " + item.STT + " đã tồn tại!");
                                }
                            }
                        }
                    }
                }
            }


            if (listErrMess != null && listErrMess.Any())
            {
                return Json(new { status = false, listErrMess = listErrMess }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            DM_Nguon data = new DM_Nguon();
            if (id.HasValue)
            {
                data = _dmNguonService.GetDMNguon(id.Value);
            }
            List<DM_Nguon> listMaNguonCha = _dmNguonService.GetAllMaNguonCha(id, int.Parse(PhienLamViec.iNamLamViec));
            ViewBag.ListMaNguonCha = listMaNguonCha;
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            var result = new DanhMucNguonViewModel();
            if (!id.HasValue)
            {
                return PartialView("_modalDetail", result);
            }
            var model = _dmNguonService.GetDMNguon(id.Value);
            if (model == null)
            {
                return PartialView("_modalDetail", result);
            }
            result.sMaNguon = model.sMaNguon;
            result.sLoai = model.sLoai;
            result.sMaCTMT = model.sMaCTMT;
            result.sKhoan = model.sKhoan;
            result.sNoiDung = model.sNoiDung;
            result.iID_Nguon = model.iID_Nguon;
            result.iID_NguonCha = model.iID_NguonCha;
            if (model.iID_NguonCha != null)
            {
                var listModel = _dmNguonService.GetAllDMNguon();
                if (listModel.Any())
                {
                    var nguonCha = listModel.FirstOrDefault(x => x.iID_Nguon == model.iID_NguonCha);
                    if (nguonCha != null)
                    {
                        result.sMaNguonCha = nguonCha.sMaNguon;
                    }
                }
            }

            return PartialView("_modalDetail", result);
        }

        [HttpPost]
        public ActionResult TimKiemDMNguon(string sMaNguon, string sNoiDung, Guid? iID_NguonCha, PagingInfo _paging)
        {
            DMNguonPagingModel vm = new DMNguonPagingModel();
            vm._paging = _paging;
            int iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);
            if (string.IsNullOrEmpty(sMaNguon) && string.IsNullOrEmpty(sNoiDung) && (iID_NguonCha == null || iID_NguonCha == Guid.Empty))
                vm.lstData = _dmNguonService.GetTreeAllDMNguonPaging(ref vm._paging, iNamLamViec);
            else
                vm.lstData = _dmNguonService.GetAllDMNguonPaging(ref vm._paging, sMaNguon, sNoiDung, iID_NguonCha, iNamLamViec);
            List<DM_Nguon> listMaNguonCha = _dmNguonService.GetAllMaNguonCha(null, iNamLamViec);
            ViewBag.ListMaNguonCha = listMaNguonCha;
            return PartialView("_list", vm);
        }
        [HttpGet]
        public ActionResult CreateDMNguon()
        {
            return View();
        }

        public ActionResult EditDMNguon(Guid id)
        {
            var objDMNguon = _dmNguonService.GetDMNguon(id);
            return View(objDMNguon);
        }

        public ActionResult Update(Guid? id)
        {
            DM_Nguon data = new DM_Nguon();
            if (id.HasValue)
            {
                data = _dmNguonService.GetDMNguon(id.Value);
            }
            List<DM_Nguon> listMaNguonCha = _dmNguonService.GetAllMaNguonCha(null, int.Parse(PhienLamViec.iNamLamViec));
            listMaNguonCha.Insert(0, new DM_Nguon { sMaNguon = "--Chọn--" });
            ViewBag.ListMaNguonCha = listMaNguonCha.ToSelectList("iID_Nguon", "sMaNguon");
            return View(data);
        }

        [HttpPost]
        public JsonResult DMNguonSave(DM_Nguon data)
        {
            var listModel = _dmNguonService.GetAllDMNguon();
            if (data.iID_Nguon == new Guid())
            {
                if (listModel.Any())
                {
                    var model = listModel.FirstOrDefault(x => x.sMaNguon == data.sMaNguon && x.bPublic == true && x.iNamLamViec == int.Parse(PhienLamViec.iNamLamViec));
                    if (model != null)
                    {
                        return Json(new { bIsComplete = false, sMessError = $"Mã {data.sMaNguon} đã tồn tại. !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!_dmNguonService.InsertDMNguon(data.sMaCTMT, data.sMaNguon, data.sLoai, data.sKhoan, data.sNoiDung, data.iID_NguonCha, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không lưu được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (listModel.Any())
                {
                    var model = listModel.FirstOrDefault(x => x.iID_Nguon != data.iID_Nguon && x.sMaNguon == data.sMaNguon && x.bPublic == true);
                    if (model != null)
                    {
                        return Json(new { bIsComplete = false, sMessError = $"Mã {data.sMaNguon} đã tồn tại. !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!_dmNguonService.UpdateDMNguon(data.iID_Nguon, data.sMaCTMT, data.sMaNguon, data.sLoai, data.sKhoan, data.sNoiDung, data.iID_NguonCha, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không lưu được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { bIsComplete = true, sMessError = "Lưu dữ liệu thành công !" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool DMNguonDelete(Guid id)
        {
            if (!_dmNguonService.UpdateDMNguon(id, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, Username, false))
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public JsonResult CheckHasChild(Guid iID_Nguon)
        {
            if (_dmNguonService.CheckHasChild(iID_Nguon))
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(Guid id)
        {
            DanhMucNguonViewModel data = _dmNguonService.DMNguonViewMoDel(id);

            return View(data);
        }

        [HttpPost]
        public JsonResult CheckDeleteDmNguon(Guid iID_Nguon)
        {
            bool deleteStatus = _dmNguonService.CheckCanDeleteDMNguon(iID_Nguon, PhienLamViec.NamLamViec);
            return Json(new { bStatus = deleteStatus }, JsonRequestBehavior.AllowGet);
        }

        #region get danh sach/ gia tri
        [Authorize]
        public JsonResult get_DanhSach(String Truong, String GiaTri, String DSGiaTri)
        {
            List<Object> list = new List<Object>();
            list.Add(new
            {
                value = "0",
                label = "Chi nhà nước thường xuyên"
            });
            list.Add(new
            {
                value = "1",
                label = "Chi thường xuyên quốc phòng"
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult get_GiaTri(String Truong, String GiaTri, String DSGiaTri)
        {
            Object item = new
            {
                value = "",
                label = ""
            };
            if (GiaTri == "0")
                item = new
                {
                    value = "0",
                    label = "Chi nhà nước thường xuyên"
                };
            else if (GiaTri == "1")
                item = new
                {
                    value = "1",
                    label = "Chi thường xuyên quốc phòng"
                };
            
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}