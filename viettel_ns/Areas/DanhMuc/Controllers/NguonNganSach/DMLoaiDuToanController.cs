using Dapper;
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
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Controllers
{
    public class DMLoaiDuToanController : AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;

        // GET: DanhMuc/DMLoaiDuToan
        public ActionResult Index(string filter = null)
        {
            //DMLoaiDuToanPagingModel data = new DMLoaiDuToanPagingModel();
            //data._paging.CurrentPage = 1;
            //data.lstData = _dmService.GetAllDMLoaiDuToanPaging(ref data._paging, int.Parse(PhienLamViec.iNamLamViec));
            //return View(data);
            var listFilter = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new DMLoaiDuToan_SheetTable(listFilter, Username);
            var model = new DMLoaiDuToanViewModel();
            var sheetBase = new SheetViewModel(bang: sheet,
                                               filters: sheet.Filters,
                                               urlPost: Url.Action("Save", "DMLoaiDuToan", new { area = "DanhMuc" }));
            model.Sheet = sheetBase;
            model.Sheet.Height = 470;
            model.Sheet.FixedRowHeight = 50;
            model.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View(model);
        }

        public ActionResult SheetFrame(string filter = null)
        {
            var listFilter = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new DMLoaiDuToan_SheetTable(listFilter, Username);
            var model = new DMLoaiDuToanViewModel();
            var sheetBase = new SheetViewModel(bang: sheet,
                                               filters: sheet.Filters,
                                               urlPost: Url.Action("Save", "DMLoaiDuToan", new { area = "DanhMuc" }));
            model.Sheet = sheetBase;
            model.Sheet.Height = 470;
            model.Sheet.FixedRowHeight = 50;
            model.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrame", model.Sheet);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            DM_LoaiDuToan data = new DM_LoaiDuToan();
            if (id.HasValue)
            {
                data = _dmService.GetDMLoaiDuToanByID(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }

        public ActionResult Update(Guid? id)
        {
            DM_LoaiDuToan data = new DM_LoaiDuToan();
            if (id.HasValue)
            {
                data = _dmService.GetDMLoaiDuToanByID(id.Value);
            }
            return View(data);
        }

        public ActionResult Detail(Guid id)
        {
            DM_LoaiDuToan data = _dmService.GetDMLoaiDuToanByID(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DM_LoaiDuToan data = new DM_LoaiDuToan();
            if (!id.HasValue)
            {
                return PartialView("_modalDetail", data);
            }

            data = _dmService.GetDMLoaiDuToanByID(id.Value);

            return PartialView("_modalDetail", data);
        }

        #region PartialView
        [HttpPost]
        public ActionResult DMLoaiDuToanListView(PagingInfo _paging, string code, string name)
        {
            DMLoaiDuToanPagingModel data = new DMLoaiDuToanPagingModel();
            data._paging = _paging;
            data.lstData = _dmService.GetAllDMLoaiDuToanPaging(ref data._paging, int.Parse(PhienLamViec.iNamLamViec), code, name);
            return PartialView("_list", data);
        }
        #endregion


        #region Process
        [HttpPost]
        public bool DMLoaiDuToanSave(DM_LoaiDuToan data)
        {
            if (data.iID_LoaiDuToan == new Guid())
            {
                if (!_dmService.InsertDMLoaiDuToan(data.sMaLoaiDuToan, data.sTenLoaiDuToan, data.sGhiChu, Username, PhienLamViec.NamLamViec)) return false;
            }
            else
            {
                if (!_dmService.UpdateDMLoaiDuToan(data.iID_LoaiDuToan, data.sMaLoaiDuToan, data.sTenLoaiDuToan, data.sGhiChu, Username)) return false;
            }
            return true;
        }

        [HttpPost]
        public JsonResult ValidateBeforeSave(List<ValidateDMLoaiDuToanModel> aListModel)
        {
            var listResult = new List<string>();
            if (aListModel == null || !aListModel.Any())
            {
                return Json(new { status = true });
            }

            var listMaTrung = new List<string>();
            foreach (var item in aListModel)
            {
                if (item.bDelete == false)
                {
                    if (listMaTrung.Contains(item.sMaLoaiDuToan))
                    {
                        listResult.Add($"Mã dự toán {item.sMaLoaiDuToan} dòng số  {item.STT} đã tồn tại!");
                    }
                    listMaTrung.Add(item.sMaLoaiDuToan);
                }
            }
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var listModel = conn.Query<DM_LoaiDuToan>($"SELECT * FROM DM_LoaiDuToan WHERE iTrangThai = 1");
                if (listModel != null && listModel.Any())
                {
                    foreach (var item in aListModel)
                    {
                        if (string.IsNullOrEmpty(item.iID_LoaiDuToan))
                        {
                            var model = listModel.Where(x => x.sMaLoaiDuToan == item.sMaLoaiDuToan).ToList();
                            if (model != null && model.Any())
                            {
                                if (aListModel.Where(x => model.Select(y => y.iID_LoaiDuToan.ToString()).Contains(x.iID_LoaiDuToan) && x.bDelete == true).Count() < model.Count())
                                    listResult.Add($"Mã dự toán {item.sMaLoaiDuToan} dòng số {item.STT} đã tồn tại!");
                            }
                        }

                        if (!string.IsNullOrEmpty(item.iID_LoaiDuToan))
                        {
                            var model = listModel.Where(x => x.iID_LoaiDuToan != Guid.Parse(item.iID_LoaiDuToan) && x.sMaLoaiDuToan == item.sMaLoaiDuToan).ToList();
                            if (model != null && model.Any())
                            {
                                if (aListModel.Where(x => model.Select(y => y.iID_LoaiDuToan.ToString()).Contains(x.iID_LoaiDuToan) && x.bDelete == true).Count() < model.Count())
                                    listResult.Add($"Mã dự toán {item.sMaLoaiDuToan} dòng số {item.STT} đã tồn tại!");
                            }
                        }
                    }
                }

                if (listResult != null && listResult.Any())
                {
                    return Json(new { status = false, sMessage = listResult });
                }
            }

            return Json(new { status = true });
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            if (vm == null)
            {
                return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
            }

            if (vm.Rows == null || !vm.Rows.Any())
            {
                return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
            }

            var config = new DC_NguoiDungCauHinh();
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                config = conn.QueryFirstOrDefault<DC_NguoiDungCauHinh>(new CommandDefinition(
                     commandText: "Select iNamLamViec, iThangLamViec, iID_MaNamNganSach, iID_MaNguonNganSach from DC_NguoiDungCauHinh  WHERE sID_MaNguoiDungTao=@sID_MaNguoiDungTao",
                     parameters: new
                     {
                         sID_MaNguoiDungTao = Username,
                     },
                     commandType: CommandType.Text
                 ));
            }

            foreach (var item in vm.Rows)
            {
                if ((string.IsNullOrEmpty(item.Id) || item.Id == null || item.Id == "null") && item.IsDeleted == true)
                {
                    continue;
                }

                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    if (string.IsNullOrEmpty(item.Id) ||
                                             item.Id == null ||
                                             item.Id == "null")
                    {
                        var model = new DM_LoaiDuToan();
                        model.iID_LoaiDuToan = Guid.NewGuid();
                        if (item.Columns != null && item.Columns.Any())
                        {
                            if (item.Columns.ContainsKey("sMaLoaiDuToan"))
                            {
                                model.sMaLoaiDuToan = item.Columns["sMaLoaiDuToan"];
                            }
                            if (item.Columns.ContainsKey("sTenLoaiDuToan"))
                            {
                                model.sTenLoaiDuToan = item.Columns["sTenLoaiDuToan"];
                            }
                            if (item.Columns.ContainsKey("sGhiChu"))
                            {
                                model.sGhiChu = item.Columns["sGhiChu"];
                            }
                        }
                        model.dNgayTao = DateTime.Now;
                        model.sID_MaNguoiDungTao = Username;
                        model.dNgaySua = DateTime.Now;
                        model.sID_MaNguoiDungSua = Username;
                        model.iNamLamViec = config.iNamLamViec;
                        model.sIPSua = Request.UserHostAddress;
                        model.iTrangThai = 1;
                        conn.Insert<DM_LoaiDuToan>(model);
                    }
                    else
                    {
                        var entity = conn.QueryFirstOrDefault<DM_LoaiDuToan>($"SELECT * FROM DM_LoaiDuToan WHERE iID_LoaiDuToan = '{item.Id}'");
                        if (entity != null)
                        {
                            if (!item.IsDeleted)
                            {
                                if (item.Columns != null && item.Columns.Any())
                                {
                                    foreach (var col in item.Columns)
                                    {
                                        if (col.Key == "sMaLoaiDuToan")
                                        {
                                            entity.sMaLoaiDuToan = col.Value;
                                        }
                                        if (col.Key == "sTenLoaiDuToan")
                                        {
                                            entity.sTenLoaiDuToan = col.Value;
                                        }
                                        if (col.Key == "sGhiChu")
                                        {
                                            entity.sGhiChu = col.Value;
                                        }
                                    }
                                }
                                entity.sIPSua = Request.UserHostAddress;
                                entity.iNamLamViec = config.iNamLamViec;
                                entity.iTrangThai = 1;
                                entity.iSoLanSua += 1;
                                entity.dNgaySua = DateTime.Now;
                                entity.sID_MaNguoiDungSua = Username;
                                conn.Execute($"UPDATE DM_LoaiDuToan set sMaLoaiDuToan = N'{entity.sMaLoaiDuToan}'," +
                                                                      $"sTenLoaiDuToan = N'{entity.sTenLoaiDuToan}'," +
                                                                      $"sGhiChu = N'{entity.sGhiChu}'," +
                                                                      $"iSoLanSua = '{entity.iSoLanSua + 1}'," +
                                                                      $"sIPSua = N'{Request.UserHostAddress}'," +
                                                                      $"sID_MaNguoiDungSua = N'{Username}'," +
                                                                      $"iNamLamViec = '{config.iNamLamViec}'," +
                                                                      $"iTrangThai = 1 WHERE iID_LoaiDuToan = '{entity.iID_LoaiDuToan}'", null, commandType: CommandType.Text);
                            }
                            else
                            {
                                conn.Execute($"UPDATE DM_LoaiDuToan set iTrangThai = 0 WHERE iID_LoaiDuToan = '{entity.iID_LoaiDuToan}'", null, commandType: CommandType.Text);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }

        [HttpPost]
        public bool DMLoaiDuToanDelete(Guid id)
        {
            if (!_dmService.UpdateDMLoaiDuToan(id, string.Empty, string.Empty, string.Empty, Username, false)) return false;
            return true;
        }

        [HttpPost]
        public bool CheckExistMaDMLoaiDuAn(Guid? id, string sCode)
        {
            return _dmService.CheckExistMaDMLoaiDuToan(id, sCode, PhienLamViec.NamLamViec);
        }

        #endregion
    }
}