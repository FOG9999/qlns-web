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

    public class DMKhoiDonViQLController : AppController
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        // GET: DanhMuc/DMKhoiDonViQL
        public ActionResult Index()

        {
            DMKhoiDonViQLPagingModel data = new DMKhoiDonViQLPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _dmService.GetAllDMKhoiDonviQL(ref data._paging, int.Parse(PhienLamViec.iNamLamViec));
            return View(data);
        }


        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            DM_KhoiDonViQuanLy data = new DM_KhoiDonViQuanLy();
            if (id.HasValue)
            {
                data = _dmService.GetDMKhoiDonViQL(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }


        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DM_KhoiDonViQuanLy data = new DM_KhoiDonViQuanLy();
            if (!id.HasValue)
            {
                return PartialView("_modalDetail", data);
            }

            data = _dmService.GetDMKhoiDonViQL(id.Value);

            return PartialView("_modalDetail", data);
        }


        public ActionResult Update(Guid? id)
        {
            DM_KhoiDonViQuanLy data = new DM_KhoiDonViQuanLy();
            if (id.HasValue)
            {
                data = _dmService.GetDMKhoiDonViQL(id.Value);
            }
            return View(data);
        }

        #region PartialView
        [HttpPost]
        public ActionResult DMKhoiDonviQLListView(PagingInfo _paging, string code, string name)
        {
            DMKhoiDonViQLPagingModel data = new DMKhoiDonViQLPagingModel();
            data._paging = _paging;
            data.lstData = _dmService.GetAllDMKhoiDonviQL(ref _paging, int.Parse(PhienLamViec.iNamLamViec), code, name);

            return PartialView("_list", data);
        }
        #endregion


        #region Process
        [HttpPost]
        public JsonResult DMKhoiDonviQLSave(DM_KhoiDonViQuanLy data)
        {
            if (data.iID_Khoi == new Guid())
            {
                if (!_dmService.CheckExistDMKhoiDonVIQL(data.iID_Khoi, data.sMaKhoi, PhienLamViec.NamLamViec))
                {
                    return Json(new { bIsComplete = false, sMessError = "Mã khối đơn vị quản lý đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                }
                if (!_dmService.InsertDMKhoiDonviQL(data.sMaKhoi, data.sTenKhoi, data.sGhiChu, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không lưu được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (!_dmService.CheckExistDMKhoiDonVIQL(data.iID_Khoi, data.sMaKhoi, PhienLamViec.NamLamViec))
                {
                    return Json(new { bIsComplete = false, sMessError = "Mã khối đơn vị quản lý đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                }
                if (!_dmService.UpdateDMKhoiDonviQL(data.iID_Khoi, data.sMaKhoi, data.sTenKhoi, data.sGhiChu, Username))
                {
                    return Json(new { bIsComplete = false, sMessError = "Không lưu được dữ liệu !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { bIsComplete = true, sMessError = "Lưu dữ liệu thành công !" }, JsonRequestBehavior.AllowGet); ;
        }

        [HttpPost]
        public bool DMKhoiDonviQLDelete(Guid id)
        {
            if (!_dmService.UpdateDMKhoiDonviQL(id, string.Empty, string.Empty, string.Empty, Username, false)) return false;
            return true;
        }

        public ActionResult Detail(Guid id)
        {
            DM_KhoiDonViQuanLy data = _dmService.GetDMKhoiDonViQL(id);
            return View(data);
        }

        #endregion
    }
}