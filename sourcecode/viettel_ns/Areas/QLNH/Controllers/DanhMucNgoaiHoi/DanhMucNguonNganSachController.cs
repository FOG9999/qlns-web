using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLNH.Controllers.DanhMucNgoaiHoi
{
    public class DanhMucNguonNganSachController : AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private List<object> lstVoucherTypes = new List<object>()
        {
            new {sTrangThai = "--Tất cả--", iTrangThai = -1},
            new {sTrangThai = "Không sử dụng", iTrangThai = 0},
            new {sTrangThai = "Đang sử dụng", iTrangThai = 1},
        };

        public ActionResult Index()
        {
            DanhMucNgoaiHoi_DanhMucNguonNganSachModelPaging vm = new DanhMucNgoaiHoi_DanhMucNguonNganSachModelPaging();
            vm._paging.CurrentPage = 1;
            ViewBag.ListTrangThai = lstVoucherTypes.ToSelectList("iTrangThai", "sTrangThai");
            vm.Items = _qlnhService.GetListDanhMucNguonNganSachPaging(ref vm._paging, null , null , -1);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucNguonNganSachSearch(PagingInfo _paging, string sMaNguonNganSach, string sTenNguonNganSach, int? iTrangThai)
        {
            DanhMucNgoaiHoi_DanhMucNguonNganSachModelPaging vm = new DanhMucNgoaiHoi_DanhMucNguonNganSachModelPaging();
            vm._paging = _paging;

            sMaNguonNganSach = HttpUtility.HtmlDecode(sMaNguonNganSach);
            sTenNguonNganSach = HttpUtility.HtmlDecode(sTenNguonNganSach);
            vm.Items = _qlnhService.GetListDanhMucNguonNganSachPaging(ref vm._paging, sMaNguonNganSach, sTenNguonNganSach, iTrangThai);

            ViewBag.ListTrangThai = lstVoucherTypes.ToSelectList("iTrangThai", "sTrangThai");
            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(int? id)
        {
            DanhmucNgoaiHoi_NguonNganSachModel data = new DanhmucNgoaiHoi_NguonNganSachModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucNguonNganSachById(id.Value);
                if (data == null)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public ActionResult GetModal(int? id)
        {
            DanhmucNgoaiHoi_NguonNganSachModel data = new DanhmucNgoaiHoi_NguonNganSachModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetDanhMucNguonNganSachById(id.Value);
            }
            ViewBag.ListTrangThai = lstVoucherTypes.ToSelectList("iTrangThai", "sTrangThai");
            return PartialView("_modalUpdate", data);
        }


        [HttpPost]
        public JsonResult NguonNganSachDelete(int id)
        {
            if (!_qlnhService.DeleteDanhMucNguonNganSach(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NguonNganSachSave(DanhmucNgoaiHoi_NguonNganSachModel data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sTen = HttpUtility.HtmlDecode(data.sTen);

            var returnData = _qlnhService.SaveNguonNganSach(data, Request.UserHostAddress, Username);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = returnData.errorMess }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}