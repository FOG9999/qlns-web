using DapperExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public class DMNoiDungChiController : AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        // GET: DanhMuc/DMNoiDungChi
        public ActionResult Index()
        {
            DMNoiDungChiGridViewModel vm = new DMNoiDungChiGridViewModel();
            return View(vm);
        }

        #region Data grid
        public ActionResult SheetFrame(string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new DMNoiDungChi_SheetTable(int.Parse(PhienLamViec.iNamLamViec), filters);
            var vm = new DMNoiDungChiGridViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "DMNoiDungChi", new { area = "DanhMuc" }),
                   urlGet: Url.Action("SheetFrame", "DMNoiDungChi", new { area = "DanhMuc" })
                   ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            var listIdDelete = new List<string>();
            var listIdUpdate = new List<string>();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();

                    #region crud
                    var columns = new DMNoiDungChi_SheetTable().Columns.Where(x => !x.IsHidden);
                    rows.ForEach(r =>
                    {
                        var trans = conn.BeginTransaction();
                        var iID_NoiDungChi = r.Id;
                        var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                        if (!string.IsNullOrWhiteSpace(iID_NoiDungChi) && r.IsDeleted)
                        {
                            listIdDelete.Add(r.Id);
                            #region delete
                            var entity = conn.Get<DM_NoiDungChi>(iID_NoiDungChi, trans);
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
                            string sMaNoiDungChiCha = "";
                            string sMaNguon = "";
                            DM_NoiDungChi ndcCha = null;
                            DM_Nguon nguon = null;
                            if (r.Columns.ContainsKey("sMaNoiDungChiCha"))
                            {
                                sMaNoiDungChiCha = r.Columns["sMaNoiDungChiCha"];
                                ndcCha = _dmService.GetDMNoiDungChiChaByMaNDCCha(sMaNoiDungChiCha, PhienLamViec.NamLamViec);
                            }
                            if (r.Columns.ContainsKey("sMaNguon"))
                            {
                                sMaNguon = r.Columns["sMaNguon"];
                                nguon = _dmService.GetDMNguonByMaNguon(sMaNguon, PhienLamViec.NamLamViec);
                            }
                            var isNew = string.IsNullOrWhiteSpace(iID_NoiDungChi);
                            if (isNew)
                            {
                                #region create
                                var entity = new DM_NoiDungChi()
                                {
                                    bPublic = true,
                                    iTrangThai = 1,
                                    iNamLamViec = PhienLamViec.NamLamViec,
                                    dNgayTao = DateTime.Now,
                                    sID_MaNguoiDungTao = Username,
                                };
                                if (r.Columns.ContainsKey("sMaNoiDungChiCha"))
                                {
                                    entity.iID_Parent = ndcCha?.iID_NoiDungChi;
                                    if (ndcCha != null)
                                    {
                                        listIdDelete.Add(ndcCha.iID_NoiDungChi.ToString());
                                    }
                                }
                                if (r.Columns.ContainsKey("sMaNguon"))
                                {
                                    entity.iID_Nguon = nguon?.iID_Nguon;
                                }
                                entity.iID_Nguon = nguon?.iID_Nguon;
                                entity.MapFrom(changes);
                                conn.Insert(entity, trans);
                                #endregion
                            }
                            else
                            {
                                #region edit
                                var entity = conn.Get<DM_NoiDungChi>(iID_NoiDungChi, trans);
                                if (r.Columns.ContainsKey("sMaNoiDungChiCha"))
                                {
                                    if (ndcCha == null)
                                    {
                                        listIdUpdate.Add(r.Id);
                                    }
                                    else
                                    {
                                        listIdDelete.Add(ndcCha.iID_NoiDungChi.ToString());
                                    }
                                    entity.iID_Parent = ndcCha?.iID_NoiDungChi;
                                }
                                if (r.Columns.ContainsKey("sMaNguon"))
                                {
                                    entity.iID_Nguon = nguon?.iID_Nguon;
                                }
                                entity.dNgaySua = DateTime.Now;
                                entity.sID_MaNguoiDungSua = Username;
                                entity.iSoLanSua = entity.iSoLanSua != null ? entity.iSoLanSua + 1 : 0;
                                entity.sIPSua = Request.UserHostAddress;
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
            _dmService.DeleteDuToanChiTiet(listIdUpdate, listIdDelete);

            // clear cache

            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }

        [HttpPost]
        public JsonResult ValidateBeforeSave(List<DMNoiDungChiGridViewModel> aListModel)
        {
            var listErrMess = new List<string>();
            if (aListModel == null || !aListModel.Any())
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }

            var lstDMNoiDungChi = _dmService.GetAllDMNoiDungChiForCheck(PhienLamViec.NamLamViec);
            if (lstDMNoiDungChi != null && lstDMNoiDungChi.Any())
            {
                foreach (var item in aListModel)
                {
                    if (item.bDelete == false)
                    {
                        if (aListModel.Where(x => x.sMaNoiDungChi == item.sMaNoiDungChi && x.bDelete == false).Count() > 1)
                        {
                            listErrMess.Add("Mã nội dung chi " + item.sMaNoiDungChi + " dòng số " + item.STT + " đã tồn tại!");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.iID_NoiDungChi))
                            {
                                var model = lstDMNoiDungChi.Where(x => x.sMaNoiDungChi == item.sMaNoiDungChi).ToList();
                                if (model != null && model.Any())
                                {
                                    if (aListModel.Where(x => model.Select(y => y.iID_NoiDungChi.ToString()).Contains(x.iID_NoiDungChi) && x.bDelete == true).Count() < model.Count())
                                        //add mess error
                                        listErrMess.Add("Mã nội dung chi " + item.sMaNoiDungChi + " dòng số " + item.STT + " đã tồn tại!");
                                }
                            }

                            if (!string.IsNullOrEmpty(item.iID_NoiDungChi))
                            {
                                var model = lstDMNoiDungChi.Where(x => x.iID_NoiDungChi != Guid.Parse(item.iID_NoiDungChi) && x.sMaNoiDungChi == item.sMaNoiDungChi).ToList();
                                if (model != null && model.Any())
                                {
                                    if (aListModel.Where(x => model.Select(y => y.iID_NoiDungChi.ToString()).Contains(x.iID_NoiDungChi) && x.bDelete == true).Count() < model.Count())
                                        //add mess error
                                        listErrMess.Add("Mã nội dung chi " + item.sMaNoiDungChi + " dòng số " + item.STT + " đã tồn tại!");
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
            List<DM_NoiDungChi> lstNoiDungChiCha = _dmService.GetNoiDungChiCha(int.Parse(PhienLamViec.iNamLamViec), id).ToList();
            ViewBag.ListNoiDungChiCha = lstNoiDungChiCha.ToSelectList("iID_NoiDungChi", "sMaNoiDungChi");

            DM_NoiDungChi data = new DM_NoiDungChi();
            if (id.HasValue)
            {
                data = _dmService.GetDMNoiDungChiByID(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DMNoiDungChiViewModel data = new DMNoiDungChiViewModel();
            if (!id.HasValue)
            {
                return PartialView("_modalDetail", data);
            }

            data = _dmService.GetDMNoiDungChiByIDForDetail(id.Value);

            return PartialView("_modalDetail", data);
        }

        public ActionResult Update(Guid? id)
        {
            DM_NoiDungChi data = new DM_NoiDungChi();
            if (id.HasValue)
            {
                data = _dmService.GetDMNoiDungChiByID(id.Value);
            }
            return View(data);
        }

        public ActionResult Detail(Guid id)
        {
            DM_NoiDungChi data = _dmService.GetDMNoiDungChiByID(id);
            return View(data);
        }

        #region PartialView
        [HttpPost]
        public ActionResult DMNoiDungChiListView(PagingInfo _paging, string code, string name)
        {
            DMNoiDungChiPagingModel data = new DMNoiDungChiPagingModel();
            data._paging = _paging;
            if (code == "" && name == "")
            {
                data.lstData = _dmService.GetTreeAllDMNoiDungChiBQPPaging(ref data._paging, int.Parse(PhienLamViec.iNamLamViec));
            }
            else
            {
                data.lstData = _dmService.GetAllDMNoiDungChiBQPPaging(ref data._paging, int.Parse(PhienLamViec.iNamLamViec), code, name);
            }
            return PartialView("_list", data);
        }
        #endregion

        #region Process
        [HttpPost]
        public JsonResult DMNoiDungChiSave(DM_NoiDungChi data)
        {
            if (data.iID_NoiDungChi == new Guid())
            {
                if (_dmService.CheckExistMaNoiDungchi(data.sMaNoiDungChi, PhienLamViec.NamLamViec))
                {
                    return Json(new { bIsComplete = false, sMessError = "Mã nội dung chi đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                }
                if (!_dmService.InsertDMNoiDungChiBQP(data.sMaNoiDungChi, data.sTenNoiDungChi, data.iID_Parent, data.sGhiChu, Username, PhienLamViec.NamLamViec))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (_dmService.CheckExistMaNoiDungchi(data.sMaNoiDungChi, PhienLamViec.NamLamViec, data.iID_NoiDungChi))
                {
                    return Json(new { bIsComplete = false, sMessError = "Mã nội dung chi đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                }
                if (!_dmService.UpdateDMNoiDungChiBQP(data.iID_NoiDungChi, data.sMaNoiDungChi, data.sTenNoiDungChi, data.iID_Parent, data.sGhiChu, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet); ;
        }

        [HttpPost]
        public bool DMNoiDungChiDelete(Guid id)
        {
            if (!_dmService.UpdateDMNoiDungChiBQP(id, string.Empty, string.Empty, null, string.Empty, Username, false)) return false;
            return true;
        }

        [HttpPost]
        public JsonResult CheckDeleteDmNoiDungChi(Guid iID_NoiDungChi)
        {
            bool deleteStatus = _dmService.CheckCanDeleteNDChi(iID_NoiDungChi, PhienLamViec.NamLamViec);
            return Json(new { bStatus = deleteStatus }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}