using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.DanhMuc.Models;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.DanhMuc.Controllers
{
    public class DMLoaiCongTrinhController : AppController
    {
        private readonly IDanhMucService _aService = DanhMucService.Default;
        // GET: DanhMuc/DMLoaiCongTrinh
        public ActionResult Index()
        {
            var data = _aService.GetListLoaicongTrinhInPartial();
            ViewBag.iTotalItem = data.Count();
            return View();
        }

        [HttpPost]
        public ActionResult DMLoaiCongTrinhListView(string sTenLoaiCongTrinh)
        {
            var data = _aService.GetListLoaicongTrinhInPartial(sTenLoaiCongTrinh);
            ViewBag.iTotalItem = data.Count();
            return PartialView("_list");
        }
        [HttpPost]
        public JsonResult GetListLoaiCongTrinhInPartial(string sTenLoaiCongTrinh)
        {
            var lstData = _aService.GetListLoaicongTrinhInPartial(sTenLoaiCongTrinh).ToList();
            return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListLoaiCongTrinhByName(string sTenLoaiCongTrinh, string sTenVietTat, string sMaLoaiCongTrinh, int? iThuTu, string sMoTa)
        {
            var lstData = _aService.GetListLoaiCongTrinhByName(sTenLoaiCongTrinh, sTenVietTat, sMaLoaiCongTrinh, iThuTu, sMoTa).ToList();
            return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            var listIdCode = new List<dynamic>();
            var listModel = _aService.GetAllDMLoaiCongTrinh();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    listIdCode.Add(new { iID_LoaiCongTrinh = item.iID_LoaiCongTrinh, sMaLoaiCongTrinh = item.sMaLoaiCongTrinh });
                }
            }
            ViewBag.ListDMLoaiCongTrinh = listIdCode;
            var data = new VDT_DM_LoaiCongTrinh();
            if (id.HasValue)
            {
                data = _aService.GetDMLoaiCongTrinhById(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            var model = new DMLoaiCongTrinhViewDetailModel();
            var data = new VDT_DM_LoaiCongTrinh();
            if (!id.HasValue)
            {
                return PartialView("_modalDetail", data);
            }

            data = _aService.GetDMLoaiCongTrinhById(id.Value);

            if (data != null)
            {
                model.iID_LoaiCongTrinh = data.iID_LoaiCongTrinh;
                model.iID_Parent = data.iID_Parent;
                model.sMaLoaiCongTrinh = data.sMaLoaiCongTrinh;
                model.sTenLoaiCongTrinh = data.sTenLoaiCongTrinh;
                model.sTenVietTat = data.sTenVietTat;
                model.sMoTa = data.sMoTa;
                model.iThuTu = data.iThuTu;
                var listModel = _aService.GetAllDMLoaiCongTrinh();
                if (listModel != null && listModel.Any())
                {
                    var modelParent = listModel.FirstOrDefault(x => x.iID_LoaiCongTrinh == data.iID_LoaiCongTrinh);
                    if (modelParent != null)
                    {
                        model.sMaLoaiCongTrinhCha = modelParent.sMaLoaiCongTrinh;
                    }
                }
            }

            return PartialView("_modalDetail", model);
        }

        [HttpPost]
        public JsonResult DMLoaiCongTrinhSave(VDT_DM_LoaiCongTrinh data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không lấy được dữ liệu truyền nên !" }, JsonRequestBehavior.AllowGet);
            }

            var listModel = _aService.GetAllDMLoaiCongTrinh();
            if (data.iID_LoaiCongTrinh == null || data.iID_LoaiCongTrinh == Guid.Empty)
            {
                if (listModel != null && listModel.Any())
                {
                    var model = listModel.FirstOrDefault(x => x.sMaLoaiCongTrinh == data.sMaLoaiCongTrinh && x.bActive == true);
                    if (model != null)
                    {
                        return Json(new { bIsComplete = false, sMessError = $"Mã loại công trình {data.sMaLoaiCongTrinh} đã có trong dữ liệu !" }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (!_aService.InsertDMLoaiCongTrinh(data, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Lưu dữ liệu bị lỗi !" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (listModel == null || !listModel.Any())
                {
                    return Json(new { bIsComplete = false, sMessError = "Dữ liệu đã bị thay đổi hoặc đã bị xóa trước đó !" }, JsonRequestBehavior.AllowGet);
                }
                var model = listModel.FirstOrDefault(x => x.iID_LoaiCongTrinh != data.iID_LoaiCongTrinh && x.sMaLoaiCongTrinh == data.sMaLoaiCongTrinh && x.bActive == true);
                if (model != null)
                {
                    return Json(new { bIsComplete = false, sMessError = $"Mã loại công trình {data.sMaLoaiCongTrinh} đã có trong dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }

                if (!_aService.UpdateDMLoaiCongTrinh(data, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Lưu dữ liệu bị lỗi !" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteItem(Guid? id)
        {
            if (id == null || !id.HasValue)
            {
                return Json(new { bIsComplete = false, sMessError = "Thiếu dữ liệu Id truyền nên !" }, JsonRequestBehavior.AllowGet);
            }

            var model = _aService.GetDMLoaiCongTrinhById(id.Value);
            if (model == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Dữ liệu đã bị thay đổi hoặc đã bị xóa !" }, JsonRequestBehavior.AllowGet);
            }

            if (!_aService.DeleteLoaiCongTrinh(model, Request.UserHostAddress, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}