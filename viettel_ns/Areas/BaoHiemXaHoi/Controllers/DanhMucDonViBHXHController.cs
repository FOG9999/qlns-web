using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.BaoHiemXaHoi;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.BaoHiemXaHoi.Controllers
{
    public class DanhMucDonViBHXHController : AppController
    {
        private readonly IBaoHiemXaHoiService _bHXHService = BaoHiemXaHoiService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;
        // GET: BaoHiemXaHoi/DanhMucDonViBHXH
        public ActionResult Index()
        {
            DanhMucBHXHDonViPagingViewModel vm = new DanhMucBHXHDonViPagingViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _bHXHService.GetAllDanhMucBHXHDonVi(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec));
            //ViewBag.iID_DonViBHXH_ID = string.Empty;
            //ViewBag.iID_MaDonViNS = string.Empty;
            //ViewBag.ListDonViParent = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("iID_BHXH_DonViID", "sTen");
            //ViewBag.ListDonViNS = _bHXHService.GetListAllDonViNS(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("iID_MaDonVi", "sTen");
            return View(vm);
        }

        public JsonResult GetDataComboBoxDonViBHParent()
        {
            var result = new List<dynamic>();
            var listDonViBHParent = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec), Username).ToList();
            if (listDonViBHParent != null && listDonViBHParent.Any())
            {
                result.Insert(0, new { id = "", text = "--Chọn--" });
                foreach (var item in listDonViBHParent)
                {
                    result.Add(new { id = item.iID_BHXH_DonViID, text = item.iID_MaDonViBHXH + " - " + item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        public JsonResult GetDataComboBoxDonViNSSearch()
        {
            var result = new List<dynamic>();
            //var listDonViNS = _bHXHService.GetListAllDonViNS(int.Parse(PhienLamViec.iNamLamViec)).ToList();
            var listDonViNS = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            if (listDonViNS != null && listDonViNS.Any())
            {
                result.Insert(0, new { id = "", text = "--Chọn--" });
                foreach (var item in listDonViNS)
                {
                    result.Add(new { id = item.iID_MaDonVi, text = item.iID_MaDonVi + " - " + item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public ActionResult DonViBHXHListView(PagingInfo _paging, string sMaDonVi, string sTenDonVi, Guid? iID_BHXH_DonVi_ParentID, string iID_MaDonVi_NS)
        {
            DanhMucBHXHDonViPagingViewModel vm = new DanhMucBHXHDonViPagingViewModel();
            vm._paging = _paging;
            vm.Items = _bHXHService.GetAllDanhMucBHXHDonVi(ref vm._paging, PhienLamViec.NamLamViec, sMaDonVi, sTenDonVi, iID_BHXH_DonVi_ParentID, iID_MaDonVi_NS);
            ViewBag.iID_DonViBHXH_ID = iID_BHXH_DonVi_ParentID;
            ViewBag.iID_MaDonViNS = iID_MaDonVi_NS;
            //ViewBag.ListDonViParent = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("iID_BHXH_DonViID", "sTen");
            //ViewBag.ListDonViNS = _bHXHService.GetListAllDonViNS(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("iID_MaDonVi", "sTen");
            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            DanhMucBHXHDonViViewModel data = new DanhMucBHXHDonViViewModel();
            if (id.HasValue)
            {
                data = _bHXHService.GetDanhMucBHXHDonViById(id.Value);
            }
            //ViewBag.ListDonViParentModal = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec), id).ToSelectList("iID_BHXH_DonViID", "sTen");
            //ViewBag.ListDonViNSModal = _bHXHService.GetListDonViNS(int.Parse(PhienLamViec.iNamLamViec), id).ToSelectList("iID_MaDonVi", "sTen");
            return PartialView("_modalUpdate", data);
        }

        public JsonResult GetDataComboBoxDonViParent(Guid? id)
        {
            var result = new List<dynamic>();
            var listDonViParent = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec), Username, id).ToList();
            if (listDonViParent != null && listDonViParent.Any())
            {
                result.Insert(0, new { id = "", text = "--Chọn--" });
                foreach (var item in listDonViParent)
                {
                    result.Add(new { id = item.iID_BHXH_DonViID, text = item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        public JsonResult GetDataComboBoxDonViNS(Guid? id)
        {
            var result = new List<dynamic>();
            var listDonViNS = _bHXHService.GetListDonViNS(int.Parse(PhienLamViec.iNamLamViec), id).ToList();
            if (listDonViNS != null && listDonViNS.Any())
            {
                result.Insert(0, new { id = "", text = "--Chọn--" });
                foreach (var item in listDonViNS)
                {
                    result.Add(new { id = item.iID_MaDonVi, text = item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult DonViSave(BHXH_DonVi data)
        {
            if (_bHXHService.CheckExistMaDonVi(data.iID_MaDonViBHXH, PhienLamViec.NamLamViec, data.iID_BHXH_DonViID))
                return Json(new { status = false, sMessError = "Đã tồn tại mã đơn vị BHXH của năm " + PhienLamViec.iNamLamViec + "!" }, JsonRequestBehavior.AllowGet);

            if (!_bHXHService.InsertDonViBHXH(data, PhienLamViec.NamLamViec, Username))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            DanhMucBHXHDonViViewModel data = _bHXHService.GetDanhMucBHXHDonViById(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public bool DeleteDonVi(Guid id)
        {
            return _bHXHService.DeleteDonViBHXH(id);
        }

        //Import Danh mục BHXH
        public ActionResult ImportDonViBHXH()
        {
            //ViewBag.ListDonViBHXH = _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec)).ToSelectList("iID_MaDonViBHXH", "sTen");
            return View();
        }

        [HttpPost]
        public ActionResult LoadDataXLSX()
        {
            List<DanhMucBHXHDonViViewModel> lstDonVi = new List<DanhMucBHXHDonViViewModel>();
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)   
                        {
                            var donvi = new DanhMucBHXHDonViViewModel();
                            donvi.iID_MaDonViBHXH = string.IsNullOrEmpty(workSheet.Cells[rowIterator, 1].Value.ToString().Trim()) ? "0" : workSheet.Cells[rowIterator, 1].Value.ToString().Trim();
                            donvi.sTen = workSheet.Cells[rowIterator, 6].Value.ToString().Trim();
                            donvi.iID_BHXH_DonViID = Guid.NewGuid();
                            if (donvi.iID_MaDonViBHXH.Length <= 2)
                            {
                                donvi.isParent = true;
                                if (donvi.iID_MaDonViBHXH.Length == 1)
                                    donvi.iID_MaDonViBHXH = "0" + donvi.iID_MaDonViBHXH;
                                donvi.sMaDonViParent = donvi.iID_MaDonViBHXH;
                            }
                            else
                            {
                                donvi.isParent = false;
                                donvi.sMaDonViParent = donvi.iID_MaDonViBHXH.Substring(0,2);
                            }

                            lstDonVi.Add(donvi);
                        }
                    }
                    var lstDonViParent = lstDonVi.Where(x => x.isParent == true);
                    foreach(var itemParent in lstDonViParent)
                    {
                        foreach(var item in lstDonVi)
                        {
                            if(!item.isParent && item.sMaDonViParent == itemParent.sMaDonViParent)
                            {
                                item.sTenBHXHDonViParent = itemParent.sTen;
                                item.iID_ParentID = itemParent.iID_BHXH_DonViID;
                            }
                        }
                    }
                    TempData["DonViImPort"] = lstDonVi;
                }
                catch (Exception ex)
                {
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            return PartialView("_listFileDonVi", lstDonVi);
        }

        [HttpPost]
        public JsonResult DonViSaveFile()
        {
            if(TempData["DonViImPort"] == null)
                return Json(new { status = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            List<DanhMucBHXHDonViViewModel> lstDonVi = (List<DanhMucBHXHDonViViewModel>)TempData["DonViImPort"];
            if(lstDonVi.Count <= 0)
                return Json(new { status = false, sMessError = "Không có danh sách đơn vị !" }, JsonRequestBehavior.AllowGet);
            if (!_bHXHService.InsertFileDonViBHXH(lstDonVi, PhienLamViec.NamLamViec, Username))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
    }
}