using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.QuyetToan;
using VIETTEL.Common;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class VDT_ThongTriController : AppController
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        // GET: QLVonDauTu/VDT_ThongTri
        public ActionResult Index()
        {
            var listDonVi = _vdtService.GetListDataDonViByUser(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");
            var result = new VDT_ThongTriPagingModel();
            result._paging.CurrentPage = 1;
            result.lstData = _vdtService.GetAllThongTriPaging(ref result._paging, Username);
            return View(result);
        }

        [HttpPost]
        public ActionResult GetListView(PagingInfo _paging, Guid? iID_DonViQuanLy = null, string sMaThongTri = "", DateTime? dNgayTaoThongTri = null, int? iNamThucHien = null, string sNguoiLap = "", string sTruongPhong = "", string sThuTruongDonVi = "")
        {
            var listDonVi = _vdtService.GetListDataDonViByUser(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");
            var result = new VDT_ThongTriPagingModel();
            result._paging = _paging;
            result.lstData = _vdtService.GetAllThongTriPaging(ref result._paging, Username, iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi);
            return PartialView("_list", result);
        }

        [HttpPost]
        public JsonResult GetDataComboBoxDonViQuanLy()
        {
            var result = new List<dynamic>();

            result.Add(new { id = "CHON_TAT_CA", text = "--Tất cả--" });
            var items = _vdtService.GetListDataDonViByUser(Username);
            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    result.Add(new { id = item.iID_Ma, text = item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        [HttpGet]
        public ActionResult Create()
        {
            var listDonVi = _vdtService.GetListDataDonViByUser(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");

            var listNguonNganSach = _vdtService.GetListAllNguonNganSach();
            ViewBag.ListNguonNganSach = listNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");
            return View();
        }

        [HttpPost]
        public ActionResult GetDataLoaiCongTrinh()
        {
            var result = new List<dynamic>();
            var listData = _vdtService.GetListDMLoaiCongTrinh();

            foreach (var item in listData)
            {
                var model = new
                {
                    id = item.iID_LoaiCongTrinh,
                    pid = item.iID_Parent,
                    name = item.sTenLoaiCongTrinh
                };

                result.Add(model);
            }

            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetUserName()
        {
            return Json(new { status = true, data = Username });
        }

        [HttpPost]
        public JsonResult Filter(VDT_ThongTriFilterModel model)
        {
            var result = _vdtService.GetDataThongTriQuyetToanTheoFilter(model, Username);
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult Save(VDT_ThongTriFilterModel model)
        {
            var checkCode = _vdtService.CheckExistMaThongTri(model.iID_ThongTriID, model.MaThongTri);
            if (!checkCode)
            {
                return Json(new { status = false, message = "Mã thông tri đã tồn tại dữ liệu !" });
            }
            var result = _vdtService.SaveThongTri(model, Username);

            return Json(new { status = result });
        }

        [HttpGet]
        public ActionResult Update(string id)
        {
            var listDonVi = _vdtService.GetListDataDonViByUser(Username);
            ViewBag.ListDonVi = listDonVi.ToSelectList("iID_Ma", "sTen");

            var listNguonNganSach = _vdtService.GetListAllNguonNganSach();
            ViewBag.ListNguonNganSach = listNguonNganSach.ToSelectList("iID_MaNguonNganSach", "sTen");

            var model = _vdtService.GetThongTriById(id);
            return View(model);
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            var model = _vdtService.GetThongTriById(id);
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            var result = _vdtService.DeleteThongTri(id);
            return Json(new { status = result });
        }

        [HttpPost]
        public ActionResult ExportData(Guid? iID_DonViQuanLy = null, string sMaThongTri = "", DateTime? dNgayTaoThongTri = null, int? iNamThucHien = null, string sNguoiLap = "", string sTruongPhong = "", string sThuTruongDonVi = "")
        {
            var model = _vdtService.ExportData(Username, iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi);
            var excel = CreateReport(model);
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Danh sách thông tri.xlsx");
            }
        }

        public ExcelFile CreateReport(VDT_ThongTriExportDataModel model)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rpt_VDT_ThongTri.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VDT_ThongTriViewModel>("master", model.ListMaster.AsEnumerable());
            fr.AddTable<VDT_ThongTri_ChiTietViewModel>("chitiet", model.ListDetail.AsEnumerable());
            fr.Run(Result);
            return Result;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ExportWordThongTri(Guid? iID_DonViQuanLy = null, string sMaThongTri = "", DateTime? dNgayTaoThongTri = null, int? iNamThucHien = null, string sNguoiLap = "", string sTruongPhong = "", string sThuTruongDonVi = "")
        {
            var model = _vdtService.ExportData(Username, iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi);
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename = VDT_QT_DeNghiQuyetToan.doc");
            Response.ContentType = "application/ms-word";
            Response.Charset = "";

            var html = "<table style='border-collapse: collapse;width:100%'>";
            html += "<thead>";
            html += "<tr>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='5%'>STT</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10'>Tên đơn vị</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Nguồn vốn</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Loại công trình</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Mã thông tri</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Ngày tạo</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Năm</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Người lập</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Trưởng phòng</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Thủ trưởng</th>";
            html += "</tr>";
            html += "</thead>";
            html += "<tbody>";

            if (model.ListMaster.Any())
            {
                var count = 1;
                foreach (var item in model.ListMaster)
                {
                    html += "<tr>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + count + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.TenDonVi + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.TenNguonVon + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.sTenLoaiCongTrinh + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.sMaThongTri + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.dNgayThongTri.Value.ToString("dd/MM/yyyy") + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.iNamThongTri + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.sNguoiLap + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.sTruongPhong + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.sThuTruongDonVi + "</td>";
                    html += "</tr>";

                    var htmlChiTiet = "";
                    if (model.ListDetail.Any())
                    {
                        var listChiTiet = model.ListDetail.Where(x => x.iID_ThongTriID == item.iID_ThongTriID).ToList();
                        if (listChiTiet != null && listChiTiet.Any())
                        {

                            htmlChiTiet += "<tr>";
                            htmlChiTiet += "<td colspan='10'>";

                            htmlChiTiet = "<table style='border-collapse: collapse;width:100%'>";
                            htmlChiTiet += "<thead>";
                            htmlChiTiet += "<tr>";
                            htmlChiTiet += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='5%'>STT</th>";
                            htmlChiTiet += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10'>Mục</th>";
                            htmlChiTiet += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Tiểu mục</th>";
                            htmlChiTiet += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Tiết mục</th>";
                            htmlChiTiet += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Ngành</th>";
                            htmlChiTiet += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Nội dung</th>";
                            htmlChiTiet += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Số tiền</th>";
                            htmlChiTiet += "</tr>";
                            htmlChiTiet += "</thead>";
                            htmlChiTiet += "<tbody>";

                            var countChiTiet = 1;
                            foreach (var chitiet in listChiTiet)
                            {
                                htmlChiTiet += "<tr>";
                                htmlChiTiet += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + countChiTiet + "</td>";
                                htmlChiTiet += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + chitiet.TenMuc + "</td>";
                                htmlChiTiet += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + chitiet.TieuMuc + "</td>";
                                htmlChiTiet += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + chitiet.TietMuc + "</td>";
                                htmlChiTiet += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + chitiet.Nganh + "</td>";
                                htmlChiTiet += "<td style='border:1px solid #ddd;padding:8px' align='center'>Nội dung</td>";
                                htmlChiTiet += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + chitiet.fSoTien + "</td>";
                                htmlChiTiet += "</tr>";
                                countChiTiet++;
                            }

                            htmlChiTiet += "</tbody>";
                            htmlChiTiet += "</table>";
                            htmlChiTiet += "</td>";
                            htmlChiTiet += "</tr>";
                        }
                    }
                    html += htmlChiTiet;

                    count++;
                }
            }

            html += "</tbody>";
            html += "</table>";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    Response.Output.Write(html);
                    Response.Flush();
                    Response.End();
                }
            }

            return Json(new { status = true });
        }
    }
}