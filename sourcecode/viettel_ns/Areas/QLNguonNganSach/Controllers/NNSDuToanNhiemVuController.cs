using Dapper;
using DapperExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNguonNganSach;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Controllers
{
    public class NNSDuToanNhiemVuController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        // GET: QLNguonNganSach/NNSDuToanNhiemVu
        public ActionResult Index(string id, string filter = null, string currentPage = "1")
        {
            //return View();
            var listFilter = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new NNSDuToan_NhiemVu_SheetTable(listFilter, id, Username);
            var model = new NNSDuToanNhiemVuViewModel();
            model.IdDuToan = id;
            var sheetBase = new SheetViewModel(bang: sheet,
                                               filters: sheet.Filters,
                                               id: id,
                                               urlPost: Url.Action("Save", "NNSDuToanNhiemVu", new { area = "QLNguonNganSach" }));
            model.Sheet = sheetBase;
            model.Sheet.Height = 470;
            model.Sheet.FixedRowHeight = 50;
            model.Sheet.AvaiableKeys = new Dictionary<string, string>();

            return View(model);
        }

        public ActionResult SheetFrame(string id, string filter = null)
        {
            var listFilter = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new NNSDuToan_NhiemVu_SheetTable(listFilter, id, Username);
            var model = new NNSDuToanNhiemVuViewModel();
            model.IdDuToan = id;
            var sheetBase = new SheetViewModel(bang: sheet,
                                               filters: sheet.Filters,
                                               id: id,
                                               urlPost: Url.Action("Save", "NNSDuToanNhiemVu", new { area = "QLNguonNganSach" }));
            model.Sheet = sheetBase;
            model.Sheet.Height = 470;
            model.Sheet.FixedRowHeight = 50;
            model.Sheet.AvaiableKeys = new Dictionary<string, string>();

            return View("_sheetFrame", model.Sheet);
        }

        [HttpPost]
        public JsonResult GetListPhongBan()
        {
            var listModel = _qLNguonNSService.GetListDanhSachPhongBan();
            var result = new List<dynamic>();
            result.Add(new { id = "TAT_CA", text = "--Tất cả--" });
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new { id = $"{item.sKyHieu}", text = $"{item.sKyHieu} - {item.sTen}" });
                }
            }

            return Json(new { status = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListNoiDungChi()
        {
            var listModel = _qLNguonNSService.GetListDanhSachNoiDungChi(int.Parse(PhienLamViec.iNamLamViec));
            var result = new List<dynamic>();
            //result.Add(new { id = "TAT_CA", text = "--Tất cả--" });
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new { id = $"{item.sMaNoiDungChi}", text = $"{item.sTenNoiDungChi}: {item.sSoTien}" });
                }
            }

            return Json(new { status = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListChungTuSearch(string maPhongBan)
        {
            var result = new List<Viettel.Models.QLNguonNganSach.NNS_DuToan_NhiemVu_ChungTuViewModel>();
            var listChungTu = _qLNguonNSService.GetListChungTuChiTiet(int.Parse(PhienLamViec.iNamLamViec));
            if (listChungTu != null && listChungTu.Any())
            {
                if (!string.IsNullOrEmpty(maPhongBan) && maPhongBan != "TAT_CA")
                    listChungTu = listChungTu.Where(x => x.iID_MaPhongBan == maPhongBan).ToList();

                if (string.IsNullOrEmpty(maPhongBan))
                    return Json(new { status = true, data = result });

                if (listChungTu != null && listChungTu.Any())
                    result = listChungTu.ToList();
            }

            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public ActionResult GetListChungTu(string idDuToan)
        {
            var result = new NhiemVuChungTuViewModel();
            result.Items = new List<Viettel.Models.QLNguonNganSach.NNS_DuToan_NhiemVu_ChungTuViewModel>();
            var listChungTu = _qLNguonNSService.GetListChungTuChiTiet(int.Parse(PhienLamViec.iNamLamViec));
            if (listChungTu != null && listChungTu.Any())
            {
                result.Items = listChungTu.ToList();
            }
            return PartialView("_listChungTu", result);
        }

        [HttpPost]
        public JsonResult SaveBySelection(NNS_DuToan_NhiemVu_SaveModel model)
        {
            if (model == null)
            {
                return Json(new { status = false });
            }
            var result = _qLNguonNSService.CreateNNSDuToanNhiemVu(model.IdDuToan, model.ListIdChungTu, Username, Request.UserHostAddress);
            if (result == false)
            {
                Json(new { status = false });
            }
            return Json(new { status = true, idDuToan = model.IdDuToan });
        }

        [HttpPost]
        public JsonResult ValidateBeforeSave(List<ValidateNNSDuToanNhiemVuModel> aListValue)
        {
            if (aListValue == null || !aListValue.Any())
            {
                return Json(new { status = true });
            }

            var errors = new List<string>();
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var listNhiemVu = conn.Query<NNS_DuToan_NhiemVu>("SELECT * FROM NNS_DuToan_NhiemVu");
                if (listNhiemVu == null || !listNhiemVu.Any())
                {
                    return Json(new { status = true });
                }
                foreach (var item in aListValue)
                {
                    if (string.IsNullOrEmpty(item.iID_NhiemVu))
                    {
                        continue;
                    }
                    var model = listNhiemVu.FirstOrDefault(x => x.iID_NhiemVu == Guid.Parse(item.iID_NhiemVu));
                    if (model == null)
                    {
                        continue;
                    }
                    if (model.iID_MaChungTu != null && model.iID_MaChungTu != Guid.Empty)
                    {
                        errors.Add($"Nhiệm vụ {model.sNhiemVu} từ bên chứng từ không được sửa. Dòng {item.STT}!");
                    }
                }
            }

            if (errors != null && errors.Any())
            {
                return Json(new { status = false, message = errors });
            }

            return Json(new { status = true });
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
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

                var listId = new List<string>();
                foreach (var item in vm.Rows)
                {
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        string sMaNoiDungChi = string.Empty;
                        string sTenNoiDungChi = string.Empty;
                        var model = new NNS_DuToan_NhiemVu();
                        model.iID_NhiemVu = Guid.NewGuid();
                        if (item.Columns.Count <= 0)
                        {
                            continue;
                        }

                        if (item.Columns.ContainsKey("sNhiemVu"))
                            model.sNhiemVu = item.Columns["sNhiemVu"];

                        if (item.Columns.ContainsKey("iID_MaChungTu"))
                        {
                            if (!string.IsNullOrEmpty(item.Columns["iID_MaChungTu"]))
                            {
                                model.iID_MaChungTu = Guid.Parse(item.Columns["iID_MaChungTu"]);
                            }
                        }

                        if (item.Columns.ContainsKey("sSoTien"))
                        {
                            if (model.iID_MaChungTu != null && model.iID_MaChungTu != Guid.Empty)
                            {
                                model.SoTien = decimal.Parse(item.Columns["sSoTien"]);
                            }
                        }

                        if (item.Columns.ContainsKey("sMaNoiDungChi"))
                            sMaNoiDungChi = item.Columns["sMaNoiDungChi"];

                        if (item.Columns.ContainsKey("sTenNoiDungChi"))
                            sTenNoiDungChi = item.Columns["sTenNoiDungChi"];

                        model.iID_DuToan = Guid.Parse(vm.Id);
                        model.dNgayTao = DateTime.Now;
                        model.sID_MaNguoiDungTao = Username;
                        model.dNgaySua = DateTime.Now;
                        model.sIPSua = Request.UserHostAddress;
                        model.sID_MaNguoiDungSua = Username;
                        conn.Insert<NNS_DuToan_NhiemVu>(model);

                        // insert NNS_DuToanChiTiet
                        if (model.iID_MaChungTu != null && model.iID_MaChungTu != Guid.Empty)
                        {
                            List<NNSDuToanChiTietGetSoTienByDonViModel> listDTChiTiet = _qLNguonNSService.GetSoTienTheoMaChungTu(model.iID_MaChungTu.Value);
                            var dutoan = _qLNguonNSService.GetNNSGiaoDuToanByID(model.iID_DuToan);
                            if (listDTChiTiet != null && listDTChiTiet.Any())
                            {
                                foreach (var objDTCT in listDTChiTiet)
                                {
                                    var entity = new NNS_DuToanChiTiet()
                                    {
                                        //iID_DuToanChiTiet = Guid.NewGuid(),
                                        iID_DuToan = model.iID_DuToan,
                                        iNamLamViec = dutoan.iNamLamViec,
                                        iID_MaNamNganSach = dutoan.iID_MaNamNganSach,
                                        iID_MaNguonNganSach = dutoan.iID_MaNguonNganSach,
                                        sMaNoiDungChi = sMaNoiDungChi,
                                        sTenNoiDungChi = sTenNoiDungChi,
                                        sMaPhongBan = objDTCT.sMaPhongBan,
                                        sTenPhongBan = objDTCT.sTenPhongBan,
                                        iID_MaDonVi = objDTCT.iID_MaDonVi,
                                        TenDonVi = objDTCT.TenDonVi,
                                        SoTien = objDTCT.SoTienXauNoiMa,
                                        iID_NhiemVu = model.iID_NhiemVu,
                                        dNgayTao = DateTime.Now,
                                        sID_MaNguoiDungTao = Username,

                                    };
                                    //entity.MapFrom(changes);
                                    conn.Insert(entity);
                                }
                            }

                        }
                    }
                    else
                    {
                        if (item.IsDeleted == false)
                        {
                            var model = new NNS_DuToan_NhiemVu();
                            model.iID_NhiemVu = Guid.Parse(item.Id);
                            if (item.Columns.Count > 0)
                            {
                                foreach (var col in item.Columns)
                                {
                                    if (col.Key == "sNhiemVu")
                                    {
                                        model.sNhiemVu = col.Value;
                                    }
                                }
                            }

                            var sqlUpdate = new StringBuilder();
                            sqlUpdate.AppendFormat("UPDATE NNS_DuToan_NhiemVu SET sNhiemVu = N'{0}' ," +
                                " dNgaySua = GETDATE() , " +
                                "sIPSua = '{1}' , " +
                                "sID_MaNguoiDungSua = '{2}' WHERE iID_NhiemVu = '{3}'", model.sNhiemVu, Request.UserHostAddress, Username, model.iID_NhiemVu);
                            conn.Execute(sqlUpdate.ToString());
                        }
                        else
                        {
                            conn.Execute(string.Format("DELETE NNS_DuToan_NhiemVu WHERE iID_NhiemVu = '{0}'", item.Id));
                            listId.Add(item.Id);
                        }
                    }
                }

                if (listId != null && listId.Any())
                {
                    _qLNguonNSService.DeleteDuToanChiTietTheoNhiemVu(listId);
                }

                // update tong so tien du toan
                _qLNguonNSService.UpdateSumDuToan(vm.Id);
            }

            //return SheetFrame(vm.Id, vm.FiltersString);
            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }
    }
}