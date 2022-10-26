using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Services;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLNH.Controllers.QuyetToan
{
    public class TaiSanController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        List<Dropdown_ChungTuTaiSan> lstTinhTrang = new List<Dropdown_ChungTuTaiSan>()
        {
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 0,
                labelName = "--Chọn trạng thái--"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 1,
                labelName = "Mới"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 2,
                labelName = "Cũ"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 3,
                labelName = "Hết giá trị"
            }
        };
        List<Dropdown_ChungTuTaiSan> lstLoaitaisan = new List<Dropdown_ChungTuTaiSan>()
        {
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 0,
                labelName = "--Loại tài sản--"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 1,
                labelName = "Tài sản hữu hình"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 2,
                labelName = "Tài sản vô hình"
            }
        };
        List<Dropdown_ChungTuTaiSan> lstTinhtrangsudung = new List<Dropdown_ChungTuTaiSan>()
        {
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 0,
                labelName = "--Tình trạng sử dụng--"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 1,
                labelName = "Chưa sử dụng"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 2,
                labelName = "Đang sử dụng"
            },
            new Dropdown_ChungTuTaiSan()
            {
                valueId = 3,
                labelName = "Không sử dụng"
            }
        };

        public ActionResult Index()
        {
            QuyetToan_ChungTuModelPaging vm = new QuyetToan_ChungTuModelPaging();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListChungTuTaiSanModels(ref vm._paging).Items;
            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucTaiSanSearch(PagingInfo _paging, string sTenChungTu, string sSoChungTu, DateTime? dNgayChungTu)
        {
            QuyetToan_ChungTuModelPaging vm = new QuyetToan_ChungTuModelPaging();
            vm._paging = _paging;

            sTenChungTu = HttpUtility.HtmlDecode(sTenChungTu);
            sSoChungTu = HttpUtility.HtmlDecode(sSoChungTu);
            vm.Items = _qlnhService.GetListChungTuTaiSanModels(ref vm._paging, sTenChungTu, sSoChungTu, dNgayChungTu).Items;
            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult OpenDetail(Guid? id)
        {
            QuyetToan_ChungTuTaiSanModel vm = new QuyetToan_ChungTuTaiSanModel();
            QuyetToan_TaiSanModelPaging listTaiSan = new QuyetToan_TaiSanModelPaging();
            listTaiSan._paging.CurrentPage = 1;
            listTaiSan.Items = _qlnhService.GetListTaiSanModels(id);
            if (id.HasValue)
            {
                vm.ChungTuModel = _qlnhService.GetChungTuTaiSanById(id.Value);
                vm.ListTaiSan = listTaiSan;
            }
            return PartialView("_modalDetail", vm);
        }

        [HttpPost]
        public ActionResult OpenUpdate(Guid? id)
        {
            QuyetToan_ChungTuTaiSanModel vm = new QuyetToan_ChungTuTaiSanModel();
            QuyetToan_TaiSanModelPaging listTaiSan = new QuyetToan_TaiSanModelPaging();
            listTaiSan._paging.CurrentPage = 1;
            listTaiSan.Items = _qlnhService.GetListTaiSanModels(id);
            if (id.HasValue)
            {
                vm.ChungTuModel = _qlnhService.GetChungTuTaiSanById(id.Value);
                vm.ListTaiSan = listTaiSan;
            }

            return PartialView("_modalUpdate", vm);
        }

        [HttpPost]
        public ActionResult OpenCreate()
        {
            return PartialView("_modalCreate");
        }

        [HttpPost]
        public JsonResult TaiSanDelete(Guid id)
        {
            if (!_qlnhService.DeleteTaiSan(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChungTuTaiSanDelete(Guid id)
        {
            if (!_qlnhService.DeleteChungTuTaiSan(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TaiSanSave(List<NH_QT_TaiSan> datats, NH_QT_ChungTuTaiSan datactts)
        {
            datactts.sSoChungTu = HttpUtility.HtmlDecode(datactts.sSoChungTu);
            datactts.sTenChungTu = HttpUtility.HtmlDecode(datactts.sTenChungTu);
            if (datats != null) {
                foreach (NH_QT_TaiSan item in datats)
                {
                    item.sTenTaiSan = HttpUtility.HtmlDecode(item.sTenTaiSan);
                    item.sMaTaiSan = HttpUtility.HtmlDecode(item.sMaTaiSan);
                    item.sMoTaTaiSan = HttpUtility.HtmlDecode(item.sMoTaTaiSan);
                    item.sDonViTinh = HttpUtility.HtmlDecode(item.sDonViTinh);
                    item.iID_MaDonVi = HttpUtility.HtmlDecode(item.iID_MaDonVi);
                }
            }

            if (!_qlnhService.SaveChungTuTaiSan(datats, datactts))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDropdownData()
        {
            return Json(new
            {
                tinhTrangList = lstTinhTrang,
                loaiTaiSanList = lstLoaitaisan,
                tinhTrangSuDungList = lstTinhtrangsudung,
            });
        }

        [HttpPost]
        public ActionResult OpenListTaiSan()
        {
             var listTaiSan = _qlnhService.getListTaiSan().ToList();
            return PartialView("_modalTaiSan", listTaiSan);
        }

        [HttpPost]
        public ActionResult ReLoadListTaiSan()
        {
             var listTaiSan = _qlnhService.getListTaiSan().ToList();
            return Json(listTaiSan);
        }

        public JsonResult ListTaiSanSave(List<NH_DM_LoaiTaiSan> data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Lỗi lưu dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            foreach (NH_DM_LoaiTaiSan item in data)
            {
                item.sMaLoaiTaiSan = HttpUtility.HtmlDecode(item.sMaLoaiTaiSan);
                item.sTenLoaiTaiSan = HttpUtility.HtmlDecode(item.sTenLoaiTaiSan);
                item.sMoTa = HttpUtility.HtmlDecode(item.sMoTa);
            }

            if (data.GroupBy(x => x.sMaLoaiTaiSan.ToUpper()).Any(g => g.Count() > 1))
            {
                return Json(new { bIsComplete = false, sMessError = "Mã loại tài sản đã bị trùng !" }, JsonRequestBehavior.AllowGet);
            }

            if (!_qlnhService.SaveListLoaiTaiSan(data))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataDropDownFromDatabase(Guid? id)
        {
            return Json(new
            {
                donViList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false),
                duAnList = _qlnhService.GetLookupDuAn(id),
                hopDongList = _qlnhService.GetLookupHopDong(id),
            });
        }
    }
}