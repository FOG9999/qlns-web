using System;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLNH.Controllers.DanhMucNgoaiHoi
{
    public class DanhMucTyGiaController: AppController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        // GET: QLVonDauTu/QLDMTyGia
        public ActionResult Index()
        {
            DanhmucNgoaiHoi_TiGiaModelPaging vm = new DanhmucNgoaiHoi_TiGiaModelPaging();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetAllTiGiaPaging(ref vm._paging, null);

            return View(vm);
        }

        [HttpPost]
        public ActionResult DanhMucTiGiaSearch(PagingInfo _paging, DateTime? dNgayLap, string sMaTiGia, string sTenTiGia, string sMoTaTiGia, string sMaTienTeGoc)
        {
            DanhmucNgoaiHoi_TiGiaModelPaging vm = new DanhmucNgoaiHoi_TiGiaModelPaging();
            sMaTiGia = HttpUtility.HtmlDecode(sMaTiGia);
            sTenTiGia = HttpUtility.HtmlDecode(sTenTiGia);
            sMoTaTiGia = HttpUtility.HtmlDecode(sMoTaTiGia);
            sMaTienTeGoc = HttpUtility.HtmlDecode(sMaTienTeGoc);
            vm._paging = _paging;
            vm.Items = _qlnhService.GetAllTiGiaPaging(ref vm._paging, dNgayLap, sMaTiGia, sTenTiGia, sMoTaTiGia, sMaTienTeGoc);

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            DanhmucNgoaiHoi_TiGiaModel data = new DanhmucNgoaiHoi_TiGiaModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetTyGiaById(id.Value);
                if (data != null)
                {
                    ViewBag.ListTiGiaChiTietTable = GetDataTiGiaChiTietTable(id, data.sMaTienTeGoc, null, null);
                }
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            DanhmucNgoaiHoi_TiGiaModel data = new DanhmucNgoaiHoi_TiGiaModel();
            if (id.HasValue)
            {
                data = _qlnhService.GetTyGiaById(id.Value);
                if (data != null)
                {
                    ViewBag.ListTiGiaChiTietTable = GetDataTiGiaChiTietTable(id, data.sMaTienTeGoc, null, null);
                }
            }

            List<NH_DM_LoaiTienTe> lstTienTe = _qlnhService.GetNHDMLoaiTienTeList().ToList();
            lstTienTe.Insert(0, new NH_DM_LoaiTienTe { ID = Guid.Empty, sMaTienTe = "--Chọn mã tiền tệ gốc--" });
            ViewBag.ListMaTienTe = lstTienTe;

            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public JsonResult ChangeMaTienTeGoc(Guid? idTiGia, Guid? idTienTeGoc, string maTienTeGoc, Guid? idTienTeGocCu)
        {
            StringBuilder htmlStr = new StringBuilder();
            if (idTiGia == null || idTiGia == Guid.Empty)
            {
                List<NH_DM_LoaiTienTe> lstAllLoaiTienTe = _qlnhService.GetNHDMLoaiTienTeList(null, new List<Guid?>() { idTienTeGoc }).ToList();
                foreach (var tiente in lstAllLoaiTienTe)
                {
                    htmlStr.AppendLine("<tr data-idtgct='" + Guid.Empty + "' data-idtiente='" + tiente.ID + "'>");
                    htmlStr.AppendLine("<td>" + maTienTeGoc + "</td>");
                    htmlStr.AppendLine("<td>");
                    htmlStr.AppendLine("<input type='text' class='form-control colGiaTriTiGia' autocomplete='off' />");
                    htmlStr.AppendLine("</td>");
                    htmlStr.AppendLine("<td class='colTienTeQuyDoi'>" + HttpUtility.HtmlEncode(tiente.sMaTienTe) + "</td>");
                    htmlStr.AppendLine("</tr>");
                }
            }
            //else
            //{
            //    List<NH_DM_TiGiaChiTiet_TableModel> tiGiaChiTietTable = GetDataTiGiaChiTietTable(idTiGia, maTienTeGoc, idTienTeGoc, idTienTeGocCu, true);
            //    foreach (var tiente in tiGiaChiTietTable)
            //    {
            //        htmlStr.AppendLine("<tr data-idtgct='" + tiente.IdTiGiaChiTiet + "' data-idtiente='" + tiente.IdTienTe + "'>");
            //        htmlStr.AppendLine("<td>" + HttpUtility.HtmlEncode(maTienTeGoc) + "</td>");
            //        htmlStr.AppendLine("<td>");
            //        htmlStr.AppendLine("<input type='text' class='form-control colGiaTriTiGia' ");
            //        htmlStr.Append("value='" + tiente.sFTiGia + "' ");
            //        htmlStr.Append("autocomplete='off' />");
            //        htmlStr.AppendLine("</td>");
            //        htmlStr.AppendLine("<td class='colTienTeQuyDoi'>" + HttpUtility.HtmlEncode(tiente.sMaTienTeQuyDoi) + "</td>");
            //        htmlStr.AppendLine("</tr>");
            //    }
            //}
            return Json(new { htmlStr = htmlStr.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TyGiaDelete(Guid id)
        {
            if (!_qlnhService.DeleteTyGia(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TyGiaSave(NH_DM_TiGia dataTiGia, IEnumerable<NHDMTiGiaChiTietParam> dataTiGiaChiTietParam)
        {
            List<NH_DM_TiGia_ChiTiet> dataTiGiaChiTiet = new List<NH_DM_TiGia_ChiTiet>();
            NH_DM_TiGia_ChiTiet nhTiGiaChiTiet;
            if (dataTiGiaChiTietParam != null)
            {
                foreach (var item in dataTiGiaChiTietParam)
                {
                    nhTiGiaChiTiet = new NH_DM_TiGia_ChiTiet()
                    {
                        ID = item.ID,
                        iID_TiGiaID = item.iID_TiGiaID,
                        iID_TienTeID = item.iID_TienTeID,
                        sMaTienTeQuyDoi = HttpUtility.HtmlDecode(item.sMaTienTeQuyDoi),
                        fTiGia = CommonFunction.TryParseDouble(item.fTiGia),
                    };
                    dataTiGiaChiTiet.Add(nhTiGiaChiTiet);
                }
            }

            dataTiGia.sMaTiGia = HttpUtility.HtmlDecode(dataTiGia.sMaTiGia);
            dataTiGia.sTenTiGia = HttpUtility.HtmlDecode(dataTiGia.sTenTiGia);
            dataTiGia.sMoTaTiGia = HttpUtility.HtmlDecode(dataTiGia.sMoTaTiGia);
            dataTiGia.sMaTienTeGoc = HttpUtility.HtmlDecode(dataTiGia.sMaTienTeGoc);

            List<NH_DM_TiGia> lstTiGia = _qlnhService.GetNHDMTiGiaList(null).ToList();
            var checkExistMaTiGia = lstTiGia.FirstOrDefault(x => x.sMaTiGia.ToUpper().Equals(dataTiGia.sMaTiGia.ToUpper()) && x.ID != dataTiGia.ID);
            if (checkExistMaTiGia != null)
            {
                return Json(new { bIsComplete = false, sMessError = "Mã tỉ giá đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            if (!_qlnhService.SaveTyGia(dataTiGia, dataTiGiaChiTiet, Username))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        private List<NH_DM_TiGiaChiTiet_TableModel> GetDataTiGiaChiTietTable(Guid? idTiGia, string maTienTeGoc, Guid? idTienTeGoc, Guid? idTienTeGocCu, bool isChangeMaTienGoc = false)
        {
            List<NH_DM_TiGia_ChiTiet> lstTiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTietList(idTiGia, false).ToList();
            if (isChangeMaTienGoc && idTienTeGoc != idTienTeGocCu) lstTiGiaChiTiet = new List<NH_DM_TiGia_ChiTiet>();

            List<Guid?> excludeIds = lstTiGiaChiTiet.Select(x => x.iID_TienTeID).ToList();
            List<NH_DM_LoaiTienTe> lstAllLoaiTienTe = _qlnhService.GetNHDMLoaiTienTeList(null, excludeIds).ToList();
            lstAllLoaiTienTe = lstAllLoaiTienTe.Where(x => x.sMaTienTe != maTienTeGoc).ToList();

            List<NH_DM_TiGiaChiTiet_TableModel> tiGiaChiTietTable = new List<NH_DM_TiGiaChiTiet_TableModel>();
            NH_DM_TiGiaChiTiet_TableModel nH_DM_TiGiaChiTiet_TableModel;
            foreach (NH_DM_TiGia_ChiTiet tgct in lstTiGiaChiTiet)
            {
                nH_DM_TiGiaChiTiet_TableModel = new NH_DM_TiGiaChiTiet_TableModel
                {
                    IdTiGiaChiTiet = tgct.ID,
                    IdTienTe = tgct.iID_TienTeID,
                    sMaTienTeGoc = maTienTeGoc,
                    sFTiGia = tgct.fTiGia.HasValue ? tgct.fTiGia.Value.ToString("#,##0." + new string('#', 339)) : string.Empty,
                    sMaTienTeQuyDoi = tgct.sMaTienTeQuyDoi
                };
                tiGiaChiTietTable.Add(nH_DM_TiGiaChiTiet_TableModel);
            }
            foreach (NH_DM_LoaiTienTe ltt in lstAllLoaiTienTe)
            {
                nH_DM_TiGiaChiTiet_TableModel = new NH_DM_TiGiaChiTiet_TableModel
                {
                    IdTienTe = ltt.ID,
                    sMaTienTeGoc = maTienTeGoc,
                    sFTiGia = string.Empty,
                    sMaTienTeQuyDoi = ltt.sMaTienTe
                };
                tiGiaChiTietTable.Add(nH_DM_TiGiaChiTiet_TableModel);
            }
            return tiGiaChiTietTable;
        }
    }
}