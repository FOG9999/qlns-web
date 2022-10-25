using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.DanhMuc
{
    public class VDTDMLoaiCongTrinhController : AppController
    {
        private readonly IQLVonDauTuService _service = QLVonDauTuService.Default;

        // GET: QLVonDauTu/VDTDMLoaiCongTrinh
        public ActionResult Index()
        {
            var data = _service.GetListLoaicongTrinhInPartial();
            ViewBag.iTotalItem = data.Count();
            return View();
        }

        public ActionResult Update(Guid? id)
        {
            ViewBag.lstMenuItem = _service.GetComboboxParent(id);

            var data = new VDT_DM_LoaiCongTrinh();
            if (id.HasValue)
            {
                data = _service.GetDMLoaiCongTrinhById(id.Value);
            }
            return View(data);
        }

        public ActionResult Detail(Guid id)
        {
            var data = _service.GetDMLoaiCongTrinhById(id);
            return View(data);
        }

        #region PartialView
        [HttpPost]
        public ActionResult DMLoaiCongTrinhListView()
        {
            var data = _service.GetListLoaicongTrinhInPartial();
            ViewBag.iTotalItem = data.Count();
            return PartialView("_list");
        }
        [HttpPost]
        public JsonResult GetListLoaiCongTrinhInPartial()
        {
            var lstData = _service.GetListLoaicongTrinhInPartial().ToList();
            return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Process
        [HttpPost]
        public JsonResult DMLoaiCongTrinhSave(VDT_DM_LoaiCongTrinh data = null)
        {
            if(data.iID_LoaiCongTrinh == new Guid())
            {
                if(!_service.InsertDMLoaiCongTrinh(data, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (!_service.UpdateDMLoaiCongTrinh(data, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DMLoaiCongTrinhDelete(Guid id)
        {
            if (_service.CheckLoaiCongTrinhHaveChild(id))
                return Json(new { bIsComplete = false , sMessError = "Loại công trình đang có công trình trực thuộc.\nYêu cầu xóa hết công trình trực thuộc." }, JsonRequestBehavior.AllowGet);

            var data = new VDT_DM_LoaiCongTrinh() { iID_LoaiCongTrinh = id };
            if (!_service.UpdateDMLoaiCongTrinh(data, Username, false)) 
                return Json(new { bIsComplete = false, sMessError = "Loại công trình xóa thất bại." }, JsonRequestBehavior.AllowGet);
            return Json(new { bIsComplete = true, sMessError = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}