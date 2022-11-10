using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using FlexCel.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.QuyetToan;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using DocumentFormat.OpenXml.Bibliography;
using VIETTEL.Flexcel;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.z.Models;
using System.Data;
using System.Globalization;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class VDT_QT_DeNghiQuyetToanController : FlexcelReportController
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private const string sFilePathBaoCao = "/Areas/QLVonDauTu/ReportExcelForm/rpt_VDT_QT_DeNghiQuyetToan.xlsx";
        public const string NGHIN_DONG = "Nghìn đồng";
        public const string DONG = "Đồng";
        public const string NGHIN_DONG_VALUE = "1000";
        public const string DONG_VALUE = "1";
        public const string TRIEU_DONG = "Triệu đồng";
        public const string TRIEU_VALUE = "1000000";
        public const string TY_DONG = "Tỷ đồng";
        public const string TY_VALUE = "1000000000";
        private const string sControlName = "VDT_QT_DeNghiQuyetToan";
        // GET: QLVonDauTu/VDT_QT_DeNghiQuyetToan
        public ActionResult Index()
        {
            VDT_QT_DeNghiQuyetToanPagingModel data = new VDT_QT_DeNghiQuyetToanPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _vdtService.GetAllDeNghiQuyetToanPaging(ref data._paging, "", null, null, "", "", Username).OrderBy(x => x.dThoiGianKhoiCong);
            return View(data);
        }

        [HttpPost]
        public ActionResult GetListView(PagingInfo _paging, string sSoBaoCao, decimal? sGiaDeNghiTu, decimal? sGiaDeNghiDen, string sTenDuAn, string sMaDuAn)
        {
            VDT_QT_DeNghiQuyetToanPagingModel data = new VDT_QT_DeNghiQuyetToanPagingModel();
            data._paging = _paging;
            data.lstData = _vdtService.GetAllDeNghiQuyetToanPaging(ref data._paging, sSoBaoCao, sGiaDeNghiTu, sGiaDeNghiDen, sTenDuAn, sMaDuAn, Username).OrderBy(x => x.dThoiGianKhoiCong);
            return PartialView("_list", data);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            var data = new VDT_QT_DeNghiQuyetToan();
            if (id.HasValue)
            {
                data = _vdtService.Get_VDT_QT_DeNghiQuyetToanById(id.Value);
            }

            return PartialView("_modalUpdate", data);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.dNgayDeNghi = DateTime.Now.ToString("dd/MM/yyyy");
            return View();
        }

        [HttpGet]
        public ActionResult Update(Guid? id)
        {
            var model = new VDT_QT_DeNghiQuyetToan();
            if (!id.HasValue)
            {
                return View();
            }
            model = _vdtService.GetDeNghiQuyetToanDetail(id.Value.ToString(), Username);
            ViewBag.isDetail = 0;
            return View(model);
        }

        [HttpGet]
        public ActionResult Detail(Guid? id)
        {
            var model = new VDT_QT_DeNghiQuyetToan();
            if (!id.HasValue)
            {
                return View();
            }
            model = _vdtService.GetDeNghiQuyetToanDetail(id.Value.ToString(), Username);
            ViewBag.isDetail = 1;
            return View("/Areas/QLVonDauTu/Views/VDT_QT_DeNghiQuyetToan/Update.cshtml", model);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            var data = new VDT_QT_DeNghiQuyetToan();
            if (id.HasValue)
            {
                data = _vdtService.GetDeNghiQuyetToanDetail(id.Value.ToString(), Username);
            }

            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult GetDonViQuanLy()
        {
            var result = new List<dynamic>();
            var listModel = _vdtService.GetListAllDonViByCurrentYear(Username, PhienLamViec.iNamLamViec);
            if (listModel != null && listModel.Any())
            {
                result.Add(new { id = string.Empty, text = "--Chọn--" });
                foreach (var item in listModel)
                {
                    result.Add(new { id = item.iID_Ma, text = $"{item.iID_MaDonVi} - {item.sTen}" });
                }
            }
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetDuAnTheoDonViQuanLy(string idDonVi, string iIdDeNghiQuyetToanId)
        {
            var result = new List<dynamic>();
            var listModel = _vdtService.GetListAllDuAn(idDonVi, iIdDeNghiQuyetToanId);
            if (listModel != null && listModel.Any())
            {
                if(listModel.Count() != 1)
                {
                    result.Add(new { id = string.Empty, text = "--Chọn--" });
                }                
                foreach (var item in listModel)
                {
                    result.Add(new { id = item.iID_DuAnID, text = item.sTenDuAn });
                }
            }
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetDuLieuDuAnByIdDonViQuanLy(string iIdDuToanId, string iIdDuAnId)
        {
            VDT_QT_DeNghiQuyetToanGetDuAnModel result = _vdtService.GetDuLieuDuAnById(iIdDuAnId, Username);
            List<VDTDuToanNguonVonModel> lstNguonVon = _vdtService.GetListDuToanNguonVonByDuToanId(iIdDuToanId);
            string sKhoiCong = _vdtService.GetPheDuyetTKTCvaTDTByID(Guid.Parse(iIdDuToanId)).sKhoiCong;
            string sKetThuc = _vdtService.GetPheDuyetTKTCvaTDTByID(Guid.Parse(iIdDuToanId)).sKetThuc;
            return Json(new { status = true, data = result, lstNguonVon = lstNguonVon, sKhoiCong = sKhoiCong, sKetThuc = sKetThuc });
        }

        [HttpPost]
        public JsonResult GetListDuToanByDuAn(Guid iIdDuAnId)
        {
            var result = new List<dynamic>();
            var data = _vdtService.GetAllDuToanIdByDuAnId(iIdDuAnId);
            if (data != null && data.Any())
            {                               
                foreach (var item in data)
                {
                    result.Add(new { id = item.iID_DuToanID, text = item.sSoQuyetDinh });
                }
            }
            string sKhoiCong = _vdtService.GetPheDuyetTKTCvaTDTByID(data.FirstOrDefault().iID_DuToanID).sKhoiCong;
            string sKetThuc = _vdtService.GetPheDuyetTKTCvaTDTByID(data.FirstOrDefault().iID_DuToanID).sKetThuc;
            return Json(new { datas = result, sKhoiCong = sKhoiCong, sKetThuc = sKetThuc }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListChiPhiHangMucTheoDuAn(Guid? iIdDuToanId, Guid? iIdDeNghiQuyetToan = null)
        {
            List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhiParent = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            List<VDT_DA_DuToan_HangMuc_ViewModel> listHangMuc = new List<VDT_DA_DuToan_HangMuc_ViewModel>();
            if (iIdDuToanId != null && iIdDuToanId != Guid.Empty)
            {
                listChiPhi = _vdtService.GetListChiPhiTheoTKTC(iIdDuToanId.Value).ToList();
                listHangMuc = _vdtService.GetListHangMucTheoTKTC(iIdDuToanId.Value).ToList();
            }
            if (iIdDeNghiQuyetToan != null && iIdDeNghiQuyetToan != Guid.Empty)
            {
                List<VDT_QT_DeNghiQuyetToan_ChiTiet> lstQuyetToanChiTiet = _vdtService.GetDeNghiQuyetToanChiTiet(iIdDeNghiQuyetToan.Value);
                if (lstQuyetToanChiTiet != null && lstQuyetToanChiTiet.Any())
                {
                    if (listChiPhi != null && listChiPhi.Any())
                    {
                        foreach (VDT_DA_DuToan_ChiPhi_ViewModel itemCp in listChiPhi)
                        {
                            VDT_QT_DeNghiQuyetToan_ChiTiet objQuyetToanChiTiet = lstQuyetToanChiTiet.Where(x => x.iID_ChiPhiId == itemCp.iID_DuAn_ChiPhi).FirstOrDefault();
                            if (objQuyetToanChiTiet != null)
                            {
                                itemCp.fGiaTriDeNghiQuyetToan = objQuyetToanChiTiet.fGiaTriDeNghiQuyetToan;
                                itemCp.fGiaTriKiemToan = objQuyetToanChiTiet.fGiaTriKiemToan;
                                itemCp.fGiaTriQuyetToanAB = objQuyetToanChiTiet.fGiaTriQuyetToanAB;
                            }
                        }
                    }

                    if (listHangMuc != null && listHangMuc.Any())
                    {
                        foreach (VDT_DA_DuToan_HangMuc_ViewModel itemHm in listHangMuc)
                        {
                            VDT_QT_DeNghiQuyetToan_ChiTiet objQuyetToanChiTiet = lstQuyetToanChiTiet.Where(x => x.iID_HangMucId == itemHm.iID_HangMucID).FirstOrDefault();
                            if (objQuyetToanChiTiet != null)
                            {
                                itemHm.fGiaTriDeNghiQuyetToan = objQuyetToanChiTiet.fGiaTriDeNghiQuyetToan;
                                itemHm.fGiaTriKiemToan = objQuyetToanChiTiet.fGiaTriKiemToan;
                                itemHm.fGiaTriQuyetToanAB = objQuyetToanChiTiet.fGiaTriQuyetToanAB;
                            }
                        }
                    }
                }
            }

            if (listChiPhi.Any())
            {
                // update iSTT chi phi con, insert chiphi con theo chi phi cha

                listChiPhiParent = listChiPhi.Where(x => x.iID_ChiPhi_Parent is null).ToList();
                var listAllChild = listChiPhi.Except(listChiPhiParent).ToList();
                Dictionary<string, List<Guid?>> dicChiPhi = listChiPhiParent.GroupBy(x => x.iID_DuAn_ChiPhi.ToString()).ToDictionary(x => x.Key, x => x.Select(y => y.iID_DuAn_ChiPhi).ToList());
                List<VDT_DA_DuToan_ChiPhi_ViewModel> listAllChiPhiChild = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
                foreach (var key in dicChiPhi.Keys)
                {
                    Guid iID_ChiPhi_Parrent = (Guid)dicChiPhi[key].ToList().FirstOrDefault();
                    var listChild = listAllChild.Where(x => x.iID_ChiPhi_Parent == iID_ChiPhi_Parrent).Distinct().OrderBy(x => x.sTenChiPhi);
                    var index = 0;
                    if (listChild.Any())
                    {
                        foreach (var item in listChild)
                        {
                            index++;
                            listAllChild.Remove(item);
                            item.iSTT = String.Concat(listChiPhi.Where(x => x.iID_DuAn_ChiPhi == iID_ChiPhi_Parrent).First().iSTT, "-", index);
                            listAllChiPhiChild.Add(item);
                        }
                        listChiPhiParent.InsertRange(listChiPhiParent.IndexOf(listChiPhi.Where(x => x.iID_DuAn_ChiPhi == iID_ChiPhi_Parrent).First()) + 1, listAllChiPhiChild);
                        //listAllChild.Except(listChild);

                    }
                }

                if (listAllChild.Any())
                {
                    RecursiveChiPhi(listChiPhiParent, listAllChild, listChiPhi);
                }

            }

            var sumGiaTriQuyetToanAB = listChiPhi.Sum(x => x.fGiaTriQuyetToanAB).HasValue ? listChiPhi.Sum(x => x.fGiaTriQuyetToanAB).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            var sumKetQuaKiemToan = listChiPhi.Sum(x => x.fGiaTriKiemToan).HasValue ? listChiPhi.Sum(x => x.fGiaTriKiemToan).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            var sumCDTDeNghiQuyetToan = listChiPhi.Sum(x => x.fGiaTriDeNghiQuyetToan).HasValue ? listChiPhi.Sum(x => x.fGiaTriDeNghiQuyetToan).Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";

            return Json(new { lstChiPhi = listChiPhiParent, lstHangMuc = listHangMuc, sumGiaTriQuyetToanAB = sumGiaTriQuyetToanAB, sumKetQuaKiemToan = sumKetQuaKiemToan, sumCDTDeNghiQuyetToan = sumCDTDeNghiQuyetToan }, JsonRequestBehavior.AllowGet);
        }


        // ham de quy update iSTT chi phi con, insert chiphi con theo chi phi cha
        private void RecursiveChiPhi(List<VDT_DA_DuToan_ChiPhi_ViewModel> lstParent, List<VDT_DA_DuToan_ChiPhi_ViewModel> lstChild, List<VDT_DA_DuToan_ChiPhi_ViewModel> lstAll)
        {

            var listIds = lstChild.Select(x => x.iID_ChiPhi_Parent).Distinct().ToList();
            List<VDT_DA_DuToan_ChiPhi_ViewModel> listAllChiPhiChild = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            var index = 0;

            foreach (var parent in listIds)
            {
                var listChild = lstChild.Where(x => x.iID_ChiPhi_Parent == parent).ToList();
                if (listChild.Any())
                {
                    foreach (var item in listChild)
                    {
                        index++;
                        lstChild.Remove(item);
                        item.iSTT = String.Concat(lstAll.Where(x => x.iID_DuAn_ChiPhi == parent).First().iSTT, "-", index);
                        listAllChiPhiChild.Add(item);
                    }

                    lstParent.InsertRange(lstParent.IndexOf(lstAll.Where(x => x.iID_DuAn_ChiPhi == parent).First()) + 1, listChild);

                }

                //lstChild.Except(listChild);
            }
            if (lstChild.Any())
            {
                RecursiveChiPhi(lstParent, lstChild, lstAll);
            }
        }

        [HttpPost]
        public JsonResult GetDuLieuDonViQuanLyByIdDuAn(string idDuAn)
        {
            var result = new NS_DonVi();
            var donVi = _vdtService.GetDuLieuDonViQuanLyByIdDuAn(idDuAn, Username);
            if (donVi != null)
            {
                result = donVi;
            }

            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult SetValueComboBoxDuAn(string id)
        {
            var result = new VDT_QT_DeNghiQuyetToan();
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { status = false });
            }
            result = _vdtService.Get_VDT_QT_DeNghiQuyetToanById(Guid.Parse(id));
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult SaveData(VDT_QT_DeNghiQuyetToanViewModel data)
        {
            var result = _vdtService.VDT_QT_DeNghiQuyetToan_SaveData(data, Username);
            if (result == false)
                return Json(new { status = false, sMessage = "Lưu dữ liệu không thành công." });

            return Json(new { status = true, sMessage = "Lưu dữ liệu thành công." });
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { status = false, sMessage = "Xóa dữ liệu không thành công" });
            }

            var result = _vdtService.VDT_QT_DeNghiQuyetToan_Delete(id, Username);

            if (!result)
            {
                return Json(new { status = false, sMessage = "Xóa dữ liệu không thành công" });
            }

            return Json(new { status = result, sMessage = "Xóa dữ liệu thành công" });
        }

        

        public ActionResult ExportExcelDeNghiQuyetToan( string ext, string listId)
        {
          
                ExcelFile xls = CreateReport(listId);
                string fileName = string.Format("{0}.{1}", "Quyet toan du an hoan thanh", ext);
            return    Print(xls, ext, fileName);
         
        }

        public ExcelFile CreateReport(string id)
        {
            var listData = _vdtService.Get_VDT_QT_DeNghiQuyetToanChiTietById(new Guid(id));
            var listDataVDT = _vdtService.Get_VDT_QT_DeNghiQuyetToan_VonDauTuById(new Guid(id));
            var listDataChiPhi = _vdtService.Get_VDT_QT_DeNghiQuyetToan_ChiPhiById(new Guid(id));
            var listDataTaiSan = _vdtService.Get_VDT_QT_DeNghiQuyetToan_TaiSanById(new Guid(id));
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<VDT_QT_DeNghiQuyetToanViewModel>("listData", listData);
            fr.AddTable<VDT_QT_DeNghiQuyetToanViewModel>("listData1", listDataVDT);
            fr.AddTable<VDT_QT_DeNghiQuyetToanChiTietViewModel>("listData2", listDataChiPhi);
            fr.AddTable<VDT_QT_DeNghiQuyetToanViewModel>("ChiPhi", listData);
            fr.AddTable<VDT_QT_DeNghiQuyetToanViewModel>("listData3", listDataTaiSan);
            fr.UseForm(this).Run(Result);
            return Result;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ExportWordDeNghiQuyetToan(string sSoBaoCao, decimal? sGiaDeNghiTu, decimal? sGiaDeNghiDen, string sTenDuAn, string sMaDuAn)
        {
            var listModel = _vdtService.ExportData(sSoBaoCao, sGiaDeNghiTu, sGiaDeNghiDen, sTenDuAn, sMaDuAn, Username);
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename = VDT_QT_DeNghiQuyetToan.doc");
            Response.ContentType = "application/ms-word";
            Response.Charset = "";

            var html = "<table style='border-collapse: collapse;width:100%'>";
            html += "<thead>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='5%'>STT</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10'>Mã dự án</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Tên dự án</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Chủ đầu tư</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Số báo cáo</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Thời gian khởi công</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='10%'>Thời gian hoàn thành</th>";
            html += "<th style='border:1px solid #ddd;padding:8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #04AA6D;color: white;' width='15%'>Giá trị đề nghị quyết toán</th>";
            html += "</thead>";
            html += "<tbody>";
            if (listModel != null && listModel.Any())
            {
                var count = 1;
                foreach (var item in listModel)
                {
                    html += "<tr>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + count + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='left'>" + item.sMaDuAn + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='left'>" + item.sTenDuAn + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.sTenChuDauTu + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='left'>" + item.sSoBaoCao + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.dThoiGianKhoiCong.Value.ToString("dd/MM/yyyy") + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + item.dThoiGianHoanThanh.Value.ToString("dd/MM/yyyy") + "</td>";
                    html += "<td style='border:1px solid #ddd;padding:8px' align='center'>" + string.Format("{0:0,0}", item.fGiaTriDeNghiQuyetToan.Value) + "</td>";
                    html += "</tr>";

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

        private List<SelectListItem> GetCbxLoaiBC()
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            lstData.Add(new SelectListItem() { Value = "0", Text = "Tổng hợp quyết toán dự án hoàn thành tờ trình" });
            lstData.Add(new SelectListItem() { Value = "1", Text = "Tổng hợp quyết toán dự án hoàn thành phụ lục" });
            return lstData;
        }

        public ActionResult ViewInBaoCao()
        {
            VDT_QT_DeNghiQuyetToanPagingModel data = new VDT_QT_DeNghiQuyetToanPagingModel();
            ViewBag.TitleDx = "Báo cáo tổng hợp quyết toán";
            ViewBag.sSoDeNghi = _vdtService.GetDeNghiQuyetToan(Username).ToSelectList("iID_DeNghiQuyetToanID", "sSoBaoCao");
            ViewBag.iItemLoaiBC = new SelectList(GetCbxLoaiBC(), "Value", "Text");
            ViewBag.NgayChungTu = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.sDonViTinh = LoadDonViTinh().ToSelectList("ValueItem", "DisplayItem");
            return View(data);
        }

        public bool ExportBC(DeNghiQuyetToanPrintDataExportModel data)
        {
            try
            {
                IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> denghiQuery = _vdtService.FindDeNghiQuyetToan(Username);
                VDT_QT_DeNghiQuyetToanViewModel denghiItem = denghiQuery.Where(n => n.iID_DeNghiQuyetToanID.ToString().ToUpper() == data.iID_DeNghiQuyetToanID.ToUpper()).FirstOrDefault();
                TempData["denghiItem"] = denghiItem;
                TempData["dataNhap"] = data;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public ActionResult ExportExcel()
        {
            string sContentType = "application/pdf";
            string sFileName = "DeNghiQuyetToanHoanThanh_ReportToTrinh.pdf";
            DeNghiQuyetToanPrintDataExportModel dataNhap = new DeNghiQuyetToanPrintDataExportModel();
            VDT_QT_DeNghiQuyetToanViewModel denghiItem = new VDT_QT_DeNghiQuyetToanViewModel();

            if (TempData["dataNhap"] != null && TempData["denghiItem"] != null)
            {
                dataNhap = (DeNghiQuyetToanPrintDataExportModel)TempData["dataNhap"];
                denghiItem = (VDT_QT_DeNghiQuyetToanViewModel)TempData["denghiItem"];
            }
            else
                return RedirectToAction("ViewInBaoCao");

            ExcelFile xls = null;
            if (dataNhap.iItemLoaiBC == 0)
            {
                xls = CreateReportToTrinh(dataNhap, denghiItem);
            }
            else if (dataNhap.iItemLoaiBC == 1)
            {
                sFileName = "DeNghiQuyetToanHoanThanh_ReportPhuLuc.pdf";
                xls = CreateReportPhuLuc(dataNhap, denghiItem);
            }
            xls.PrintLandscape = true;

            return xls.ToPdfContentResult(sFileName);
        }

        public ExcelFile CreateReportToTrinh(DeNghiQuyetToanPrintDataExportModel dataNhap, VDT_QT_DeNghiQuyetToanViewModel denghiItem)
        {
            VDT_DA_QDDauTu quyetDinhDauTu = new VDT_DA_QDDauTu();
            double giaTriDuToan = 0;
            List<ReportTongHopQuyetToanDuAnHoanThanhModel> result = new List<ReportTongHopQuyetToanDuAnHoanThanhModel>();
            List<ReportTongHopQuyetToanDuAnHoanThanhModel> chiPhi = new List<ReportTongHopQuyetToanDuAnHoanThanhModel>();

            if (denghiItem.iID_DuAnID.HasValue)
            {
                quyetDinhDauTu = _vdtService.FindQDDauTuByDuAnId(Guid.Parse(denghiItem.iID_DuAnID.ToString()));
                giaTriDuToan = _vdtService.GetGiaTriDuToanIdByDuAnId(denghiItem.iID_DuAnID);
                result = GetDataNguonVonByDuAnId(denghiItem.iID_DuAnID);
                result.Select(n => { n.Stt = (result.IndexOf(n) + 1).ToString(); return n; }).ToList();

                chiPhi = GetDataChiPhi(denghiItem.iID_DuAnID, denghiItem);
                chiPhi.Select(n => { n.Stt = (chiPhi.IndexOf(n) + 1).ToString(); return n; }).ToList();
            }

            double sumDieuChinhCuoi = result.Sum(n => n.DieuChinhCuoi) / (Double)(dataNhap.fDonViTinh);
            double sumKeHoach = result.Sum(n => n.KeHoach) / (Double)(dataNhap.fDonViTinh);
            double sumDaThanhToan = result.Sum(n => n.DaThanhToan) / (Double)(dataNhap.fDonViTinh);

            double sumMLNSDieuChinhCuoi = chiPhi.Sum(n => n.DieuChinhCuoi) / (Double)(dataNhap.fDonViTinh);
            double sumMLNSKeHoach = chiPhi.Sum(n => n.KeHoach) / (Double)(dataNhap.fDonViTinh);
            double sumMLNSDaThanhToan = chiPhi.Sum(n => n.DaThanhToan) / (Double)(dataNhap.fDonViTinh);

            double taiSanDaiHan = (denghiItem.fTaiSanDaiHanDonViKhacQuanLy.HasValue ? denghiItem.fTaiSanDaiHanDonViKhacQuanLy.Value : 0) +
                        (denghiItem.fTaiSanDaiHanThuocCDTQuanLy.HasValue ? denghiItem.fTaiSanDaiHanThuocCDTQuanLy.Value : 0);
            double taiSanNganHan = (denghiItem.fTaiSanNganHanDonViKhacQuanLy.HasValue ? denghiItem.fTaiSanNganHanDonViKhacQuanLy.Value : 0) +
                (denghiItem.fTaiSanNganHanThuocCDTQuanLy.HasValue ? denghiItem.fTaiSanNganHanThuocCDTQuanLy.Value : 0);

            result.Select(n => { n.DieuChinhCuoi = n.DieuChinhCuoi / (Double)(dataNhap.fDonViTinh); n.KeHoach = n.KeHoach / (Double)(dataNhap.fDonViTinh); n.DaThanhToan = n.DaThanhToan / (Double)(dataNhap.fDonViTinh); return n; }).ToList();
            chiPhi.Select(n => { n.DieuChinhCuoi = n.DieuChinhCuoi / (Double)(dataNhap.fDonViTinh); n.KeHoach = n.KeHoach / (Double)(dataNhap.fDonViTinh); n.DaThanhToan = n.DaThanhToan / (Double)(dataNhap.fDonViTinh); return n; }).ToList();

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rptVDT_TongHopQuyetToanDuAnHoanThanh.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            fr.AddTable<ReportTongHopQuyetToanDuAnHoanThanhModel>("Items", result);
            fr.AddTable<ReportTongHopQuyetToanDuAnHoanThanhModel>("MLNS", chiPhi);

            fr.SetValue("Diadiem", dataNhap.txt_DiaDiem);
            fr.SetValue("Ngay", dataNhap.dNgayChungTu.HasValue ? dataNhap.dNgayChungTu.Value.Day.ToString() : "...");
            fr.SetValue("Thang", dataNhap.dNgayChungTu.HasValue ? dataNhap.dNgayChungTu.Value.Month.ToString() : "...");
            fr.SetValue("Nam", dataNhap.dNgayChungTu.HasValue ? dataNhap.dNgayChungTu.Value.Year.ToString() : "...");
            fr.SetValue("TieuDe", dataNhap.txt_TieuDe);
            fr.SetValue("Kinhgui", dataNhap.txt_KinhGui);
            fr.SetValue("TenDuAn", denghiItem.sTenDuAn);
            fr.SetValue("ChuDauTu", denghiItem.sTenChuDauTu);
            fr.SetValue("DuyetDieuChinhLanCuoi", (quyetDinhDauTu != null && quyetDinhDauTu.iID_QDDauTuID != Guid.Empty && quyetDinhDauTu.fTongMucDauTuPheDuyet.HasValue) ? quyetDinhDauTu.fTongMucDauTuPheDuyet / dataNhap.fDonViTinh : 0);
            fr.SetValue("DuToanDuocDuyetCuoi", giaTriDuToan / dataNhap.fDonViTinh);
            fr.SetValue("DonViTinh", dataNhap.sDonViTinh);
            fr.SetValue("NguyenNhanBatKhaKhang", denghiItem.fChiPhiThietHai.HasValue ? denghiItem.fChiPhiThietHai.Value : 0);
            fr.SetValue("ChiPhiKhongTaoTaiSan", denghiItem.fChiPhiKhongTaoNenTaiSan.HasValue ? denghiItem.fChiPhiKhongTaoNenTaiSan.Value : 0);
            fr.SetValue("TaiSanDaiHan", taiSanDaiHan);
            fr.SetValue("TaiSanNganHan", taiSanNganHan);
            fr.SetValue("SumTaiSan", taiSanDaiHan + taiSanNganHan);
            fr.SetValue("TinhHinhThucHienDuAn", dataNhap.txt_TinhHinh);
            fr.SetValue("NhanXetDanhGia", dataNhap.txt_NhanXet);
            fr.SetValue("KienNghi", dataNhap.txt_KienNghi);
            fr.SetValue("sumDieuChinhCuoi", sumDieuChinhCuoi);
            fr.SetValue("sumKeHoach", sumKeHoach);
            fr.SetValue("sumDaThanhToan", sumDaThanhToan);
            fr.SetValue("sumMLNSDieuChinhCuoi", sumMLNSDieuChinhCuoi);
            fr.SetValue("sumMLNSKeHoach", sumMLNSKeHoach);
            fr.SetValue("sumMLNSDaThanhToan", sumMLNSDaThanhToan);
            fr.UseChuKy(Username)
                 .UseChuKyForController(sControlName)
                 .UseForm(this);


            fr.Run(Result);
            return Result;
        }

        private List<ReportTongHopQuyetToanDuAnHoanThanhModel> GetDataChiPhi(Guid? duanId, VDT_QT_DeNghiQuyetToanViewModel denghiItem)
        {
            string duToanId = _vdtService.GetDuToanIdByDuAnId(Guid.Parse(duanId.ToString())).iID_DuToanID.ToString();
            if (string.IsNullOrEmpty(duToanId))
            {
                return new List<ReportTongHopQuyetToanDuAnHoanThanhModel>();
            }
            List<VdtDaDuToanChiPhiDataQuery> listDuToanChiPhi = _vdtService.FindListDuToanChiPhiByDuAn(duanId);
            List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi = _vdtService.GetListChiPhiTheoTKTC(Guid.Parse(duToanId)).ToList();
            List<DeNghiQuyetToanChiTietModel> listDeNghiQuyetToan = listChiPhi.Select(x => new DeNghiQuyetToanChiTietModel()
            {
                ChiPhiId = x.iID_DuAn_ChiPhi.HasValue ? x.iID_DuAn_ChiPhi.Value : Guid.Empty,
                GiaTriPheDuyet = x.fTienPheDuyet,
                TenChiPhi = x.sTenChiPhi
            }).ToList();
            List<VDT_QT_DeNghiQuyetToan_ChiTiet> ListDbData = _vdtService.FindByDeNghiQuyetToanId(denghiItem.iID_DeNghiQuyetToanID);
            if (ListDbData != null && ListDbData.Count > 0)
            {
                foreach (DeNghiQuyetToanChiTietModel data in listDeNghiQuyetToan)
                {
                    VDT_QT_DeNghiQuyetToan_ChiTiet entity = ListDbData.Where(n => n.iID_ChiPhiId == data.ChiPhiId).FirstOrDefault();
                    if (entity != null)
                    {
                        data.FGiaTriKiemToan = entity.fGiaTriKiemToan.HasValue ? entity.fGiaTriKiemToan.Value : 0;
                        data.FGiaTriDeNghiQuyetToan = entity.fGiaTriDeNghiQuyetToan.HasValue ? entity.fGiaTriDeNghiQuyetToan.Value : 0;
                    }
                }
            }

            List<ReportTongHopQuyetToanDuAnHoanThanhModel> listResult = new List<ReportTongHopQuyetToanDuAnHoanThanhModel>();
            foreach (DeNghiQuyetToanChiTietModel item in listDeNghiQuyetToan)
            {
                listResult.Add(new ReportTongHopQuyetToanDuAnHoanThanhModel
                {
                    DieuChinhCuoi = item.GiaTriPheDuyet.HasValue ? item.GiaTriPheDuyet.Value : 0,
                    KeHoach = item.FGiaTriDeNghiQuyetToan,
                    DaThanhToan = (item.GiaTriPheDuyet.HasValue ? item.GiaTriPheDuyet.Value : 0) - item.FGiaTriDeNghiQuyetToan,
                    NoiDung = item.TenChiPhi
                });
            }
            return listResult;
        }

        private List<ReportTongHopQuyetToanDuAnHoanThanhModel> GetDataNguonVonByDuAnId(Guid? duanId)
        {
            string duToanId = _vdtService.GetDuToanIdByDuAnId(Guid.Parse(duanId.ToString())).iID_DuToanID.ToString();
            if (string.IsNullOrEmpty(duToanId))
            {
                return new List<ReportTongHopQuyetToanDuAnHoanThanhModel>();
            }
            List<NguonVonQuyetToanKeHoachQuery> listDuToanNguonVonQuery = _vdtService.GetNguonVonByDuToanId(duToanId).ToList();
            List<ReportTongHopQuyetToanDuAnHoanThanhModel> listResult = new List<ReportTongHopQuyetToanDuAnHoanThanhModel>();
            foreach (NguonVonQuyetToanKeHoachQuery item in listDuToanNguonVonQuery)
            {
                listResult.Add(new ReportTongHopQuyetToanDuAnHoanThanhModel
                {
                    DieuChinhCuoi = item.GiaTriPheDuyet,
                    NoiDung = item.TenNguonVon,
                    KeHoach = item.fCapPhatBangLenhChi + item.fCapPhatTaiKhoBac
                });
            }
            return listResult;
        }

        public ExcelFile CreateReportPhuLuc(DeNghiQuyetToanPrintDataExportModel dataNhap, VDT_QT_DeNghiQuyetToanViewModel denghiItem)
        {
            string duToanId = _vdtService.GetDuToanIdByDuAnId(Guid.Parse(denghiItem.iID_DuAnID.ToString())).iID_DuToanID.ToString();
            if (string.IsNullOrEmpty(duToanId))
            {
                return null;
            }
            List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi = _vdtService.GetListChiPhiTheoTKTC(Guid.Parse(duToanId)).ToList();
            List<DeNghiQuyetToanChiTietModel> listDeNghiQuyetToan = listChiPhi.Select(x => new DeNghiQuyetToanChiTietModel()
            {
                ChiPhiId = x.iID_DuAn_ChiPhi.HasValue ? x.iID_DuAn_ChiPhi.Value : Guid.Empty,
                GiaTriPheDuyet = x.fTienPheDuyet,
                TenChiPhi = x.sTenChiPhi
            }).ToList();

            listDeNghiQuyetToan.Where(n => n.PhanCap == 1).Select(n => { n.IsShow = true; return n; }).ToList();
            listDeNghiQuyetToan.Select(n => { n.IsChiPhi = true; return n; }).ToList();
            CreateMaOrderItem(ref listDeNghiQuyetToan);

            List<VDT_QT_DeNghiQuyetToan_ChiTiet> listDbData = _vdtService.FindByDeNghiQuyetToanId(denghiItem.iID_DeNghiQuyetToanID);
            if (listDbData != null && listDbData.Count > 0)
            {
                foreach (DeNghiQuyetToanChiTietModel dataQt in listDeNghiQuyetToan)
                {
                    VDT_QT_DeNghiQuyetToan_ChiTiet entity = listDbData.Where(n => n.iID_ChiPhiId == dataQt.ChiPhiId).FirstOrDefault();
                    if (entity != null)
                    {
                        dataQt.FGiaTriKiemToan = entity.fGiaTriKiemToan ?? 0;
                        dataQt.FGiaTriDeNghiQuyetToan = entity.fGiaTriDeNghiQuyetToan ?? 0;
                    }
                }
            }
            listDeNghiQuyetToan.Where(n => n.FGiaTriKiemToan != 0 || n.FGiaTriDeNghiQuyetToan != 0).Select(n => { n.IsShow = true; return n; }).ToList();
            listDeNghiQuyetToan = listDeNghiQuyetToan.Where(n => n.IsShow).OrderBy(n => n.MaOrderDb).ToList();

            CheckHangCha(ref listDeNghiQuyetToan);
            listDeNghiQuyetToan.Select(n =>
            {
                n.Stt = (listDeNghiQuyetToan.IndexOf(n) + 1);
                n.GiaTriPheDuyet = n.GiaTriPheDuyet.HasValue ? n.GiaTriPheDuyet.Value / (Double)(dataNhap.fDonViTinh) : 0;
                n.FGiaTriKiemToan = n.FGiaTriKiemToan.HasValue ? n.FGiaTriKiemToan.Value / (Double)(dataNhap.fDonViTinh) : 0;
                n.FGiaTriDeNghiQuyetToan /= (Double)(dataNhap.fDonViTinh);
                n.FGiaTriAB /= (Double)(dataNhap.fDonViTinh);
                return n;
            }).ToList();

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/rptVDT_TongHopQuyetToanDuAnHoanThanhPhuLuc.xlsx"));
            FlexCelReport fr = new FlexCelReport();

            fr.AddTable("Items", listDeNghiQuyetToan);

            fr.SetValue("ChuDauTu", denghiItem.sTenChuDauTu);
            fr.SetValue("TieuDe", dataNhap.txt_TieuDe);
            fr.SetValue("DonViTinh", dataNhap.sDonViTinh);
            fr.SetValue("Diadiem", dataNhap.txt_DiaDiem);
            fr.SetValue("Ngay", dataNhap.dNgayChungTu.HasValue ? dataNhap.dNgayChungTu.Value.Day.ToString() : "...");
            fr.SetValue("Thang", dataNhap.dNgayChungTu.HasValue ? dataNhap.dNgayChungTu.Value.Month.ToString() : "...");
            fr.SetValue("Nam", dataNhap.dNgayChungTu.HasValue ? dataNhap.dNgayChungTu.Value.Year.ToString() : "...");
            fr.UseChuKy(Username)
                 .UseChuKyForController(sControlName)
                 .UseForm(this);

            fr.Run(Result);
            return Result;
        }

        private void CheckHangCha(ref List<DeNghiQuyetToanChiTietModel> listDeNghiQuyetToan)
        {
            foreach (DeNghiQuyetToanChiTietModel item in listDeNghiQuyetToan)
            {
                if (item.IsChiPhi)
                {
                    DeNghiQuyetToanChiTietModel child = listDeNghiQuyetToan.Where(n => n.IdChiPhiDuAnParent == item.ChiPhiId).FirstOrDefault();
                    if (child != null)
                    {
                        item.IsChiPhi = true;
                    }
                    else
                    {
                        item.IsHangCha = false;
                    }
                    if (item.ListHangMuc != null && item.ListHangMuc.Count > 0)
                    {
                        foreach (DeNghiQuyetToanChiTietModel hangMucChild in item.ListHangMuc)
                        {
                            DeNghiQuyetToanChiTietModel checkItem = listDeNghiQuyetToan.Where(n => n.HangMucId == hangMucChild.HangMucId).FirstOrDefault();
                            if (checkItem != null)
                            {
                                item.IsHangCha = true;
                            }
                        }
                    }
                }
                else
                {
                    DeNghiQuyetToanChiTietModel child = listDeNghiQuyetToan.Where(n => n.IdHangMucParent == item.HangMucId).FirstOrDefault();
                    if (child != null)
                    {
                        item.IsHangCha = true;
                    }
                    else
                    {
                        item.IsHangCha = false;
                    }
                }
            }
        }

        public void CreateMaOrderItem(ref List<DeNghiQuyetToanChiTietModel> listDeNghiQuyetToan)
        {
            if (listDeNghiQuyetToan == null || listDeNghiQuyetToan.Count == 0)
                return;
            DeNghiQuyetToanChiTietModel root = listDeNghiQuyetToan.Where(n => n.IsChiPhi && n.IdChiPhiDuAnParent == Guid.Empty && n.PhanCap == 1).FirstOrDefault();

            if (root != null)
            {
                root.MaOrderDb = "1";
                CreateMaOrderItemChild(root, ref listDeNghiQuyetToan);
            }
        }
        public void CreateMaOrderItemChild(DeNghiQuyetToanChiTietModel parent, ref List<DeNghiQuyetToanChiTietModel> listDeNghiQuyetToan)
        {
            List<DeNghiQuyetToanChiTietModel> listChild = listDeNghiQuyetToan.Where(n => n.IdChiPhiDuAnParent == parent.ChiPhiId).ToList();
            if (listChild == null || listChild.Count == 0)
            {
                return;
            }
            for (int i = 0; i < listChild.Count; i++)
            {
                listChild[i].MaOrderDb = parent.MaOrderDb + "_" + (i + 1).ToString();
                CreateMaOrderItemChild(listChild[i], ref listDeNghiQuyetToan);
            }
        }
        public ActionResult ImportDNQT()
        {
            DeNghiQuyetToanChiTietModel vm = new DeNghiQuyetToanChiTietModel();
            ViewBag.dNgayDeNghi = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public JsonResult LoadDataExcel(HttpPostedFileBase file)
        {
            try
            {
                var iIdDuToanId = Request["iIdDuToanId"];
                var file_data = getBytes(file);                
                var chiPhi = ExcelHelpers.LoadExcelDataTable(file_data, 0, 0, 2);
                var taiSan = ExcelHelpers.LoadExcelDataTable(file_data, 0, 0, 3);
                var nguonVon = ExcelHelpers.LoadExcelDataTable(file_data, 0, 0, 1);
                IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> dataImportChiPhiKhac = excel_result(chiPhi, 1);
                IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> dataImportTaiSan = excel_result(taiSan, 2);
                IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> dataImportChiPhi = excel_result(chiPhi, 3);
                //IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> dataImportNguonVon = excel_result(nguonVon, 4);
                List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi = _vdtService.GetListChiPhiTheoTKTC(Guid.Parse(iIdDuToanId)).ToList();
                List<VDTDuToanNguonVonModel> listNguonVon = _vdtService.GetListDuToanNguonVonByDuToanId(iIdDuToanId);
                foreach (var item in listChiPhi)
                {
                    foreach(var itemImport in dataImportChiPhi.FirstOrDefault().listChiPhi)
                    {
                        if(item.sTenChiPhi == itemImport.sTenChiPhi)
                        {
                            item.fGiaTriDeNghiQuyetToan = itemImport.fGiaTriDeNghiQuyetToan;
                            item.fGiaTriQuyetToanAB = itemImport.fGiaTriQuyetToanAB;
                            item.fGiaTriKiemToan = itemImport.fGiaTriKiemToan;
                        }
                    }
                }

                return Json(new
                {
                    bIsComplete = true,
                    dataImportChiPhiKhac = dataImportChiPhiKhac,
                    dataImportTaiSan = dataImportTaiSan,
                    listChiPhi = listChiPhi,
                    //listNguonVon = dataImportNguonVon.FirstOrDefault().listNguonVon
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { bIsComplete = false}, JsonRequestBehavior.AllowGet);
        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> excel_result(DataTable dt, int loai)
        {
            List<VDT_QT_DeNghiQuyetToanViewModel> dataImport = new List<VDT_QT_DeNghiQuyetToanViewModel>();
            var items = dt.AsEnumerable();
            var listChiPhi = new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
            var listNguonVon = new List<VDTDuToanNguonVonModel>();
            if (loai == 1)           //lay chi phi khac
            {
                var fChiPhiThietHai = items.ToList()[5].Field<string>(5);
                var fChiPhiKhongTaoNenTaiSan = items.ToList()[6].Field<string>(5);
                var e = new VDT_QT_DeNghiQuyetToanViewModel
                {
                    fChiPhiThietHai = Convert.ToDouble(fChiPhiThietHai),
                    fChiPhiKhongTaoNenTaiSan = Convert.ToDouble(fChiPhiKhongTaoNenTaiSan),
                };
                dataImport.Add(e);
            } else if (loai == 2)           //lay tai san
            {
                var fTaiSanDaiHanThuocCDTQuanLy = items.ToList()[5].Field<string>(5);
                var fTaiSanDaiHanDonViKhacQuanLy = items.ToList()[5].Field<string>(6);
                //var fTaiSanNganHanThuocCDTQuanLy = items.ToList()[7].Field<string>(3);
                //var fTaiSanNganHanDonViKhacQuanLy = items.ToList()[7].Field<string>(4);
                var e = new VDT_QT_DeNghiQuyetToanViewModel
                {
                    fTaiSanDaiHanThuocCDTQuanLy = Convert.ToDouble(fTaiSanDaiHanThuocCDTQuanLy),
                    fTaiSanDaiHanDonViKhacQuanLy = Convert.ToDouble(fTaiSanDaiHanDonViKhacQuanLy),
                    //fTaiSanNganHanThuocCDTQuanLy = Convert.ToDouble(fTaiSanNganHanThuocCDTQuanLy),
                    //fTaiSanNganHanDonViKhacQuanLy = Convert.ToDouble(fTaiSanNganHanDonViKhacQuanLy)
                };
                dataImport.Add(e);
            } else if(loai == 3)        //lay chi phi chinh
            {
                for(var i = 11; i < items.Count(); i++)
                {
                    DataRow r = items.ToList()[i];
                    var sTenChiPhi = r.Field<string>(3);
                    var fGiaTriDeNghiQuyetToan = r.Field<string>(6);
                    var fGiaTriQuyetToanAB = r.Field<string>(7);
                    var fGiaTriKiemToan = r.Field<string>(8);  
                    var e = new VDT_DA_DuToan_ChiPhi_ViewModel
                    {
                        sTenChiPhi = sTenChiPhi,
                        fGiaTriDeNghiQuyetToan = Convert.ToDouble(fGiaTriDeNghiQuyetToan),
                        fGiaTriQuyetToanAB = Convert.ToDouble(fGiaTriQuyetToanAB),
                        fGiaTriKiemToan = Convert.ToDouble(fGiaTriKiemToan)
                    };
                    listChiPhi.Add(e);
                }
                dataImport.Add(new VDT_QT_DeNghiQuyetToanViewModel { listChiPhi = listChiPhi });
            } else if(loai == 4)        //lay nguon von
            {
                for(var i = 5; i < items.Count(); i++)
                {
                    DataRow r = items.ToList()[i];
                    var sTenNguonVon = r.Field<string>(3);
                    var iID_NguonVonID = r.Field<string>(4);
                    var fTienCDTQuyetToan = r.Field<string>(8);
                    var e = new VDTDuToanNguonVonModel
                    {
                        sTenNguonVon = sTenNguonVon,
                        iID_NguonVonID = int.Parse(iID_NguonVonID),
                        fTienCDTQuyetToan = Convert.ToDouble(fTienCDTQuyetToan)
                    };
                    listNguonVon.Add(e);
                }
                dataImport.Add(new VDT_QT_DeNghiQuyetToanViewModel { listNguonVon = listNguonVon });
            }
            return dataImport.AsEnumerable();
        }
        public FileResult DownloadImportExample()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/QuyetToan/Import_Example_QuyetToanDuAnHoanThanh.xlsx"));
            string fileName = "FileImportExpQuyetToanDuAnHoanThanh.xls";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}