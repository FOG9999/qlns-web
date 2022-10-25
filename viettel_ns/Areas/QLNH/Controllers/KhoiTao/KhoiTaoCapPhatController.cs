using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Flexcel;
using Viettel.Models.QLNH.KhoiTao;
using System.Web;

namespace VIETTEL.Areas.QLNH.Controllers.KhoiTao
{
    public class KhoiTaoCapPhatController : FlexcelReportController
    {
        // GET: QLNH/KhoiTaoCapPhat
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly ICPNHService _cpnhService = CPNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        public ActionResult Index()
        {
            KhoiTao_KhoiTaoCapPhatModel vm = new KhoiTao_KhoiTaoCapPhatModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListKhoiTaoCapPhat(ref vm._paging, null, null, null);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<Dropdown_SelectValue> lstNamKhoiTao = GetListNamKeHoach();
            lstNamKhoiTao.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKhoiTao = lstNamKhoiTao;
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, DateTime? dNgayKhoiTao, Guid? iDonVi, int? iNamKhoiTao)
        {
            KhoiTao_KhoiTaoCapPhatModel vm = new KhoiTao_KhoiTaoCapPhatModel();
            vm._paging = _paging;
            vm.Items = _qlnhService.GetListKhoiTaoCapPhat(ref vm._paging
                , (dNgayKhoiTao == null ? null : dNgayKhoiTao), (iDonVi == Guid.Empty ? null : iDonVi)
                , (iNamKhoiTao == null ? null : iNamKhoiTao));

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<Dropdown_SelectValue> lstNamKhoiTao = GetListNamKeHoach();
            lstNamKhoiTao.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKhoiTao = lstNamKhoiTao;

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            NH_KT_KhoiTaoCapPhat data = new NH_KT_KhoiTaoCapPhat();

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<NH_DM_TiGia> lstTiGia = _qlnhService.GetTiGiaQuyetToan().ToList();
            lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
            ViewBag.ListTiGia = lstTiGia;

            List<Dropdown_SelectValue> lstNamKhoiTao = GetListNamKeHoach();
            lstNamKhoiTao.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKhoiTao = lstNamKhoiTao;


            //ViewBag.IsTongHop = false;

            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinKhoiTaoCapPhatById(id.Value);
                //ViewBag.IsTongHop = data.sTongHop != null ? true : false;

            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public JsonResult Save(NH_KT_KhoiTaoCapPhat data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);

            var returnData = _qlnhService.SaveKhoiTaoCapPhat(data, Username);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = returnData.errorMess }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true, dataID = returnData.KhoiTaoCapPhatData.ID }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            NH_KT_KhoiTaoCapPhatData data = new NH_KT_KhoiTaoCapPhatData();
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinKhoiTaoCapPhatById(id.Value);
                var donvi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), data.iID_DonViID.ToString());
                data.sTenDonVi = donvi != null ? donvi.iID_MaDonVi + " - " + donvi.sTen : string.Empty;
                var tiGia = _qlnhService.GetTiGiaQuyetToan().FirstOrDefault();
                data.sTenTiGia = tiGia != null ? tiGia.sTenTiGia  : string.Empty;
                data.sMaTienTeGoc = tiGia.sMaTienTeGoc;
            }

            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult GetHopDongAll()
        {
            var result = new List<dynamic>();
            var listModel = _cpnhService.GetListHopDong().ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.ID,
                        text = item.sSoHopDong + " - " + item.sTenHopDong
                    });
                }
            }
            return Json(new { status = true, data = result });
        }
        [HttpPost]
        public JsonResult GetDuAnAll()
        {
            var result = new List<dynamic>();
            var listModel = _qlnhService.GetNHDADuAnList().ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.ID,
                        text = item.sMaDuAn + " - " + item.sTenDuAn
                    });
                }
            }
            return Json(new { status = true, data = result });
        }

        public ActionResult Detail(string id,bool edit)
        {
            NH_QT_KhoiTaoCapPhat_ChiTietView vm = new NH_QT_KhoiTaoCapPhat_ChiTietView();
            vm.KhoiTaoCapPhatDetail = _qlnhService.GetThongTinKhoiTaoCapPhatById(new Guid(id));
            var donvi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), vm.KhoiTaoCapPhatDetail.iID_DonViID.ToString());
            vm.KhoiTaoCapPhatDetail.sTenDonVi = donvi != null ? donvi.iID_MaDonVi + " - " + donvi.sTen : string.Empty;
            
            vm.ListDetailKhoiTaoCapPhat = _qlnhService.GetListKhoiTaoCapPhatChiTiet(new Guid(id)).ToList();
            List<string> arr = new List<string>()
            {
                "USD",
                "VND"
            };
            var tiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTiet(vm.KhoiTaoCapPhatDetail.iID_TiGiaID,false).Where(x => arr.Contains(x.sMaTienTeQuyDoi)).FirstOrDefault();

            vm.KhoiTaoCapPhatDetail.fDonViTiGia = tiGiaChiTiet != null ? tiGiaChiTiet.fTiGia : 1;

            var tiGia = _qlnhService.GetNHDMTiGiaList(vm.KhoiTaoCapPhatDetail.iID_TiGiaID).FirstOrDefault();
            vm.KhoiTaoCapPhatDetail.sMaTienTeGoc = tiGia.sMaTienTeGoc;

            ViewBag.IsEdit = edit;
            ViewBag.IdQuyetToan = id;
            return View(vm);
        }

        [HttpPost]
        public JsonResult SaveDetail(List<NH_KT_KhoiTaoCapPhat_ChiTiet> data, List<NH_KT_KhoiTaoCapPhat_ChiTiet> dataDelete)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            var returnData = _qlnhService.SaveKhoiTaoCapPhatDetail(data, dataDelete, Username);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Xoa(string id)
        {
            if (!_qlnhService.DeleteKhoiTaoCapPhat(Guid.Parse(id)))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        public List<Dropdown_SelectValue> GetListNamKeHoach()
        {
            List<Dropdown_SelectValue> listNam = new List<Dropdown_SelectValue>();
            int namHienTai = DateTime.Now.Year + 1;
            for (int i = 10; i > 0; i--)
            {
                namHienTai -= 1;
                Dropdown_SelectValue namKeHoachOpt = new Dropdown_SelectValue()
                {
                    Value = namHienTai,
                    Label = namHienTai.ToString()
                };
                listNam.Add(namKeHoachOpt);
            }
            return listNam;
        }

    }
}