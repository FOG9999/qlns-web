using DapperExtensions;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Newtonsoft.Json;
using OfficeOpenXml;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNguonNganSach;
using Viettel.Services;
using VIETTEL.Areas.DanhMuc.Models;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Controllers
{
    public class QLDotNhanController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        // GET: QLNguonNganSach/QLDotNhan
        public ActionResult Index()
        {
            DotNhanPagingViewModel vm = new DotNhanPagingViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qLNguonNSService.GetAllDotNhanNguonNS(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), "", "", "", "", null, null);
            ViewBag.ListLoaiDuToan = _qLNguonNSService.GetAllDMLoaiDuToan(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");
            return View(vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            NNS_DotNhan data = new NNS_DotNhan();
            if (id.HasValue)
            {
                data = _qLNguonNSService.GetDotNhanNguonNS(id.Value);
            }
            else
            {
                DotNhanPagingViewModel vm = new DotNhanPagingViewModel();
                vm._paging.CurrentPage = 1;
                int? indexMax = _qLNguonNSService.GetAllDotNhanNguonNS(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), "", "", "", "", null, null).Select(x => x.iIndex).Max();
                string sSochungTu = FormatSoChungTu(indexMax);
                data.sSoChungTu = sSochungTu;
                // set default dNgayChungTu, dNgayQuyetDinh = today
                data.dNgayChungTu = DateTime.Now;
                data.dNgayQuyetDinh = DateTime.Now;
            }
            ViewBag.ListLoaiDuToan = _qLNguonNSService.GetAllDMLoaiDuToan(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult NNSDotNhanListView(PagingInfo _paging, string sSoChungTu, string sNoiDung, string sMaLoaiDuToan, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo)
        {
            DotNhanPagingViewModel vm = new DotNhanPagingViewModel();
            vm._paging = _paging;
            vm.Items = _qLNguonNSService.GetAllDotNhanNguonNS(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
            ViewBag.ListLoaiDuToan = _qLNguonNSService.GetAllDMLoaiDuToan(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");
            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            NNS_DotNhan data = _qLNguonNSService.GetDotNhanNguonNS(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult DotNhanNguonNSSave(NNS_DotNhan data)
        {
            if (data.sMaLoaiDuToan == "001" || data.sMaLoaiDuToan == "002")
            {
                bool isExist = false;
                if (data.iID_DotNhan == null || data.iID_DotNhan == Guid.Empty)
                {
                    isExist = _qLNguonNSService.CheckExistLoaiDuToan(int.Parse(PhienLamViec.iNamLamViec), data.sMaLoaiDuToan, null);
                }
                else
                {
                    isExist = _qLNguonNSService.CheckExistLoaiDuToan(int.Parse(PhienLamViec.iNamLamViec), data.sMaLoaiDuToan, data.iID_DotNhan);
                }
                if (isExist)
                {
                    if (data.sMaLoaiDuToan == "001")
                    {
                        return Json(new { status = false, sMessError = "Dự toán đầu năm đã được thêm trong đợt nhận khác của năm " + PhienLamViec.iNamLamViec + "!" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = false, sMessError = "Dự toán đầu năm trước chuyển sang đã được thêm trong đợt nhận khác của năm " + PhienLamViec.iNamLamViec + "!" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (!_qLNguonNSService.InsertDotNhanNguonNS(ref data, Username))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            return Json(new { status = true, iID_DotNhan = data.iID_DotNhan }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public bool DMNguonDelete(Guid id)
        {
            return _qLNguonNSService.DeleteDotNhanNguon(id);
        }


        public ActionResult Detail(string id)
        {
            var vm = new QLDotnhanViewModel
            {
                iID_DotNhan = id
            };

            return View(vm);
        }
        public ActionResult SheetFrame(string id, string filter = null)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var sheet = new QLDotNhan_SheetTable(id, int.Parse(PhienLamViec.iNamLamViec), filters);
            var vm = new QLDotnhanViewModel
            {
                Sheet = new SheetViewModel(
                   bang: sheet,
                   id: id,
                   filters: sheet.Filters,
                   urlPost: Url.Action("Save", "QLDotNhan", new { area = "QLNguonNganSach" })
                   ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrame", vm.Sheet);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var rows = vm.Rows.ToList();
            //var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    #region crud
                    var columns = new QLDotNhan_SheetTable().Columns.Where(x => !x.IsReadonly);

                    var dotNhan = _qLNguonNSService.GetDotNhanNguonNS(new Guid(vm.Id));
                    var nguonNSService = QLNguonNganSachService.Default;
                    //var mlnsList = ngansachService.GetMLNS_All(PhienLamViec.iNamLamViec);
                    rows.ForEach(r =>
                    {
                        var values = r.Id.ToList("_", true);
                        var id_dotNhanChiTiet = values[0];
                        var id_dotNhan = values[1];
                        var id_nguonNS = values[2];

                        var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                        if (changes.Any())
                        {
                            var isNew = string.IsNullOrWhiteSpace(id_dotNhanChiTiet);
                            var entityDNCT = _qLNguonNSService.GetDotNhanChiTietByIdDotNhanIdNguon(Guid.Parse(vm.Id), Guid.Parse(id_nguonNS), PhienLamViec.NamLamViec);
                            if (isNew && entityDNCT == null)
                            {
                                #region create
                                var entity = new NNS_DotNhanChiTiet()
                                {
                                    iID_DotNhan = Guid.Parse(vm.Id),
                                    iID_Nguon = Guid.Parse(id_nguonNS),
                                    iNamLamViec = dotNhan.iNamLamViec,
                                    iID_MaNamNganSach = dotNhan.iID_MaNamNganSach,
                                    iID_MaNguonNganSach = dotNhan.iID_MaNguonNganSach,
                                    dNgayTao = DateTime.Now,
                                    sID_MaNguoiDungTao = Username,

                                };
                                entity.MapFrom(changes);
                                _qLNguonNSService.InsertNNSDotNhanChiTiet(entity, Username);

                                #endregion
                            }
                            else
                            {
                                #region edit
                                var entity = _qLNguonNSService.GetDotNhanChiTiet(Guid.Parse(id_dotNhanChiTiet));
                                entity.sID_MaNguoiDungSua = Username;
                                entity.MapFrom(changes);

                                bool isUpdate = true;
                                if (r.Columns.ContainsKey("SoTien"))
                                {
                                    string sSoTien = r.Columns["SoTien"];
                                    if (string.IsNullOrWhiteSpace(sSoTien) || decimal.Parse(sSoTien) == 0)
                                    {
                                        isUpdate = false;
                                        conn.Delete(entity, trans);
                                    }
                                }

                                if (isUpdate)
                                    _qLNguonNSService.UpdateDotNhanChiTiet(entity, Username);
                                #endregion
                            }
                        }
                    });

                    #endregion

                    // commit to db
                    trans.Commit();
                }
            }

            // clear cache

            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }

        public string FormatSoChungTu(int? iIndex)
        {
            if (!iIndex.HasValue)
            {
                iIndex = 0;
            }
            iIndex = iIndex + 1;

            if (iIndex < 10)
            {
                return "DN-000" + iIndex;
            }
            else if (iIndex < 100)
            {
                return "DN-00" + iIndex;
            }
            else if (iIndex < 1000)
            {
                return "DN-0" + iIndex;
            }
            return "DN-" + iIndex;
        }

        //public ActionResult ExportData(string sMaLoaiDuToan = "", string sSoChungTu = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sNoiDung = "", string sLoaiTep = "")
        //{
        //    var listData = _qLNguonNSService.ExportData(sMaLoaiDuToan, sSoChungTu, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sNoiDung, Username);
        //    using (var excelPackage = new ExcelPackage())
        //    {
        //        excelPackage.Workbook.Worksheets.Add("Sheet1");
        //        var workSheet = excelPackage.Workbook.Worksheets[0];
        //        workSheet.Row(1).Height = 35;
        //        workSheet.Column(1).Width = 10;
        //        workSheet.Column(2).Width = 40;
        //        workSheet.Column(3).Width = 25;
        //        workSheet.Column(4).Width = 25;
        //        workSheet.Column(5).Width = 25;
        //        workSheet.Column(6).Width = 100;
        //        workSheet.Column(7).Width = 25;

        //        workSheet.Cells[1, 1].Value = "STT";
        //        workSheet.Cells[1, 1].Style.Font.Size = 12;
        //        workSheet.Cells[1, 1].Style.Font.Name = "Cambria";
        //        workSheet.Cells[1, 1].Style.Font.Bold = true;
        //        workSheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        workSheet.Cells[1, 2].Value = "Loại dự toán";
        //        workSheet.Cells[1, 2].Style.Font.Size = 12;
        //        workSheet.Cells[1, 2].Style.Font.Name = "Cambria";
        //        workSheet.Cells[1, 2].Style.Font.Bold = true;
        //        workSheet.Cells[1, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells[1, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        workSheet.Cells[1, 3].Value = "Số chứng từ";
        //        workSheet.Cells[1, 3].Style.Font.Size = 12;
        //        workSheet.Cells[1, 3].Style.Font.Name = "Cambria";
        //        workSheet.Cells[1, 3].Style.Font.Bold = true;
        //        workSheet.Cells[1, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells[1, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        workSheet.Cells[1, 4].Value = "Số quyết định";
        //        workSheet.Cells[1, 4].Style.Font.Size = 12;
        //        workSheet.Cells[1, 4].Style.Font.Name = "Cambria";
        //        workSheet.Cells[1, 4].Style.Font.Bold = true;
        //        workSheet.Cells[1, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells[1, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        workSheet.Cells[1, 5].Value = "Ngày quyết định";
        //        workSheet.Cells[1, 5].Style.Font.Size = 12;
        //        workSheet.Cells[1, 5].Style.Font.Name = "Cambria";
        //        workSheet.Cells[1, 5].Style.Font.Bold = true;
        //        workSheet.Cells[1, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells[1, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        workSheet.Cells[1, 6].Value = "Nội dung";
        //        workSheet.Cells[1, 6].Style.Font.Size = 12;
        //        workSheet.Cells[1, 6].Style.Font.Name = "Cambria";
        //        workSheet.Cells[1, 6].Style.Font.Bold = true;
        //        workSheet.Cells[1, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells[1, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        workSheet.Cells[1, 7].Value = "Số tiền";
        //        workSheet.Cells[1, 7].Style.Font.Size = 12;
        //        workSheet.Cells[1, 7].Style.Font.Name = "Cambria";
        //        workSheet.Cells[1, 7].Style.Font.Bold = true;
        //        workSheet.Cells[1, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells[1, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        if (listData != null && listData.Any())
        //        {
        //            var count = 2;
        //            foreach (var item in listData)
        //            {
        //                workSheet.Cells[count, 1].Value = count - 1;
        //                workSheet.Cells[count, 1].Style.Font.Size = 11;
        //                workSheet.Cells[count, 1].Style.Font.Name = "Cambria";
        //                workSheet.Cells[count, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //                workSheet.Cells[count, 2].Value = item.TenLoaiDuToan;
        //                workSheet.Cells[count, 2].Style.Font.Size = 11;
        //                workSheet.Cells[count, 2].Style.Font.Name = "Cambria";
        //                workSheet.Cells[count, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

        //                workSheet.Cells[count, 3].Value = item.SoChungTu;
        //                workSheet.Cells[count, 3].Style.Font.Size = 11;
        //                workSheet.Cells[count, 3].Style.Font.Name = "Cambria";
        //                workSheet.Cells[count, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

        //                workSheet.Cells[count, 4].Value = item.SoQuyetDinh;
        //                workSheet.Cells[count, 4].Style.Font.Size = 11;
        //                workSheet.Cells[count, 4].Style.Font.Name = "Cambria";
        //                workSheet.Cells[count, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

        //                if (item.NgayQuyetDinh.HasValue)
        //                {
        //                    workSheet.Cells[count, 5].Value = item.NgayQuyetDinh.Value.ToString("dd/MM/yyyy");
        //                    workSheet.Cells[count, 5].Style.Font.Size = 11;
        //                    workSheet.Cells[count, 5].Style.Font.Name = "Cambria";
        //                    workSheet.Cells[count, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                }

        //                workSheet.Cells[count, 6].Value = item.NoiDung;
        //                workSheet.Cells[count, 6].Style.Font.Size = 11;
        //                workSheet.Cells[count, 6].Style.Font.Name = "Cambria";
        //                workSheet.Cells[count, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

        //                if (item.SoTien.HasValue)
        //                {
        //                    workSheet.Cells[count, 7].Value = string.Format("{0:0,0}", item.SoTien.Value);
        //                    workSheet.Cells[count, 7].Style.Font.Size = 11;
        //                    workSheet.Cells[count, 7].Style.Font.Name = "Cambria";
        //                    workSheet.Cells[count, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                }

        //                count++;
        //            }
        //        }

        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            excelPackage.SaveAs(stream);
        //            if (!string.IsNullOrEmpty(sLoaiTep))
        //            {
        //                if (sLoaiTep == "EXCEL")
        //                {
        //                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Danh sách đợt nhận {PhienLamViec.iNamLamViec}.xlsx");
        //                }

        //                if (sLoaiTep == "PDF")
        //                {
        //                    var workbook = new Workbook();
        //                    workbook.LoadFromStream(stream);
        //                    Worksheet sheet = workbook.Worksheets[0];
        //                    sheet.SaveToPdfStream(stream);
        //                    return File(stream.ToArray(), "application/pdf", $"Danh sách đợt nhận {PhienLamViec.iNamLamViec}.pdf");
        //                }
        //            }

        //        }
        //    }
        //    if (listData != null && listData.Any())
        //    {

        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public ActionResult ExportData(string sMaLoaiDuToan = "", string sSoChungTu = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sNoiDung = "", string sLoaiTep = "")
        {
            var excel = CreateReport(sMaLoaiDuToan, sSoChungTu, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sNoiDung);
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);

                if (sLoaiTep == "EXCEL")
                {
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Danh sách đợt nhận năm {PhienLamViec.iNamLamViec}.xlsx");
                }

                //if (sLoaiTep == "PDF")
                //{
                //    var workbook = new Workbook();
                //    workbook.LoadFromStream(stream);
                //    Worksheet sheet = workbook.Worksheets[0];
                //    sheet.SaveToPdfStream(stream);
                //    return File(stream.ToArray(), "application/pdf", $"Danh sách dự toán năm {PhienLamViec.iNamLamViec}.pdf");
                //}
            }

            return RedirectToAction(nameof(Index));
        }

        public ExcelFile CreateReport(string sMaLoaiDuToan = "", string sSoChungTu = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sNoiDung = "")
        {
            var listData = _qLNguonNSService.ExportData(sMaLoaiDuToan, sSoChungTu, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sNoiDung, Username);

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLNguonNganSach/ReportExcelForm/rpt_NNS_DotNhan.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<NNSDotNhanExportDataModel>("DotNhan", listData.AsEnumerable());
            fr.SetValue("iNamLamViec", PhienLamViec.iNamLamViec);
            fr.Run(Result);
            return Result;
        }
    }
}