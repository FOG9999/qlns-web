using DapperExtensions;
using VIETTEL.Helpers;
using VIETTEL.Flexcel;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using FlexCel.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Common;
using System.Web;

namespace VIETTEL.Areas.QLNH.Controllers.CPNH
{
    public class ThongTriCapPhatController : FlexcelReportController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly ICPNHService _cpnhService = CPNHService.Default;
        private const string sFilePathGiayThongTriCapPhatNgoaiTe= "/Report_ExcelFrom/QLNH/rpt_GiayThongTriCapPhat_NgoaiTe.xlsx";
        private const string sFilePathGiayThongTriCapPhatNguyenTe= "/Report_ExcelFrom/QLNH/rpt_GiayThongTriCapPhat_NguyenTe.xlsx";
        private const string sControlName = "ThongTriCapPhat";


        // GET: QLNH/ThongTriCapPhat
        public ActionResult Index()
        {
            ThongTriCapPhatModelPaging tt = new ThongTriCapPhatModelPaging();
            tt._paging.CurrentPage = 1;
            tt.thongtri = _qlnhService.GetListThongTriCapPhatPaging(ref tt._paging, null, null, null, null);
            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
    
            return View(tt);
        }

        public ActionResult ThongTriCapPhatSearch(ThongTriCapPhatModelSearch data)
        {
            ThongTriCapPhatModelPaging tt = new ThongTriCapPhatModelPaging();
            tt._paging.CurrentPage = data._paging.CurrentPage;
            data.sSongThongTri = HttpUtility.HtmlDecode(data.sSongThongTri);
            tt.thongtri = _qlnhService.GetListThongTriCapPhatPaging(ref tt._paging, data.iDonVi, data.sSongThongTri, data.dNgayLap, data.iNam);
            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            return PartialView("_list", tt);
        }

        public PartialViewResult Create(ThongTriCapPhatModelSearch data)
        {
            ThongTriCapPhatCreateViewModel model = new ThongTriCapPhatCreateViewModel();
            ThongTriCapPhatModel tt = new ThongTriCapPhatModel();
            ThanhToan_ThongTriModelPaging vm = new ThanhToan_ThongTriModelPaging();

            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            tt = _qlnhService.GetThongTriByID(data.iThongTri);
            if (data._paging == null)
            {
                vm._paging.CurrentPage = 1;
            }
            else
            {
                vm._paging.CurrentPage = data._paging.CurrentPage;
            }
            vm._paging.ItemsPerPage = 5;

            if (data.iThongTri == null)
            {
                ViewBag.Title = "Thêm mới thông tri cấp kinh phí";
                vm.thanhtoan_thongtri = _qlnhService.GetListThanhToanTaoThongTri(ref vm._paging, data.iThongTri, data.iDonVi, data.iNam, data.iLoaiThongTri, data.iLoaiNoiDung, data.iTypeAction);
            }
            else
            {
                ViewBag.Title = "Sửa đổi thông tri cấp kinh phí";
                vm.thanhtoan_thongtri = _qlnhService.GetListThanhToanTaoThongTri(ref vm._paging, tt.ID, tt.iID_DonViID, tt.iNamThongTri, tt.iLoaiThongTri, tt.iLoaiNoiDungChi, data.iTypeAction);
            }

            model.thanhtoan_thongtri = vm;
            model.thongtri_capphat = tt;

            return PartialView("_create", model);
        }

        public ActionResult GetListThanhToanThongTri(ThongTriCapPhatModelSearch data)
        {
            ThanhToan_ThongTriModelPaging vm = new ThanhToan_ThongTriModelPaging();
            if (data._paging == null)
            {
                vm._paging.CurrentPage = 1;
            }
            else
            {
                vm._paging.CurrentPage = data._paging.CurrentPage;
            }
            vm._paging.ItemsPerPage = 5;
            vm.thanhtoan_thongtri = _qlnhService.GetListThanhToanTaoThongTri(ref vm._paging, data.iThongTri, data.iDonVi, data.iNam, data.iLoaiThongTri, data.iLoaiNoiDung, data.iTypeAction);
            
            return PartialView("_tt_thongtri", vm);
        }

        public JsonResult CheckTrungMaThongTri(string mathongtri, int type_action, Guid? imathongtri)
        {
            Boolean results = _qlnhService.CheckTrungMaThongTri(mathongtri, type_action, imathongtri);
            return Json(new { results = results,status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool SaveData(ThongTriCapPhatModel data, int type_action)
        {
            var result = false;
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    data.fThongTri_USD = CommonFunction.TryParseDouble(data.sThongTri_USD);
                    data.fThongTri_VND = CommonFunction.TryParseDouble(data.sThongTri_VND);
                    data.sSoThongTri = HttpUtility.HtmlDecode(data.sSoThongTri);
                    var enitityCapPhat = new NH_TT_ThongTriCapPhat();
                    enitityCapPhat.MapFrom(data);

                    //Thực hiện insert
                    if (data.ID == Guid.Empty)
                    {
                        //Lưu thông tri cấp phát
                        enitityCapPhat.dNgayTao = DateTime.Now;
                        conn.Insert(enitityCapPhat, trans);
                        //Lưu chi tiết thông tri cấp phát
                    }
                    else
                    {
                        var entity_org = _qlnhService.GetThongTriByID(data.ID);
                        enitityCapPhat.dNgayTao = entity_org != null ? entity_org.dNgayTao : null;
                        conn.Update(enitityCapPhat, trans);
                    }

                    result = _qlnhService.SaveDanhSachPheDuyetThanhToan(enitityCapPhat.ID, data.sIdThanhToans, type_action);

                    trans.Commit();
                }    
            }
            catch(Exception ex)
            {

            }
            return result;
        }

        public PartialViewResult Detail(Guid? IdThongTri, int iTypeAction)
        {
            ThongTriCapPhatCreateViewModel model = new ThongTriCapPhatCreateViewModel();
            ThongTriCapPhatModel tt = new ThongTriCapPhatModel();
            tt = _qlnhService.GetThongTriByID(IdThongTri);
            ThanhToan_ThongTriModelPaging vm = new ThanhToan_ThongTriModelPaging();
            vm._paging.CurrentPage = 1;
            vm._paging.ItemsPerPage = 5;
            vm.thanhtoan_thongtri = _qlnhService.GetListThanhToanTaoThongTri(ref vm._paging, IdThongTri, null, null, null, null, iTypeAction);

            model.thanhtoan_thongtri = vm;
            model.thongtri_capphat = tt;
            return PartialView("_details", model);
        }

        [HttpPost]
        public JsonResult GetThongTriByID(Guid? id)
        {
            var thongtri = _qlnhService.GetThongTriByID(id);
            var chitiets = _qlnhService.GetListhongTriChiTietByID(id);
            return Json(new { thongtri = thongtri, chitiets = chitiets, status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool DeleteThongTriCapPhat(Guid? id)
        {
            try
            {
                return _qlnhService.DeleteThongTriCapPhat(id);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public ActionResult onModelThongBaoXoa(Guid? id)
        {
            ThongTriCapPhatModel model = _qlnhService.GetThongTriByID(id);
            return PartialView("_modalDelete", model);
        }

        [ValidateInput(false)]
        public ActionResult ExportBaoCaoThongTriCapPhat(Guid? idThongTri, string ext = "pdf" , string donvicaptren = "", string donvi = "")
        {
            donvicaptren = HttpUtility.UrlDecode(donvicaptren);
            donvi = HttpUtility.UrlDecode(donvi);
            ExcelFile xls = FileThongTriCapPhatNgoaiTe(idThongTri, donvicaptren, donvi);
            string sFileName = "Thông tri cấp phát kinh phí bằng ngoại tệ";
            sFileName = string.Format("{0}.{1}", sFileName, ext);
            return Print(xls, ext, sFileName );
        }

        public ExcelFile FileThongTriCapPhatNgoaiTe(Guid? idThongTri, string donvicaptren,string donvi)
        {
            XlsFile Result = new XlsFile(true);
            FlexCelReport fr = new FlexCelReport();
            Result.Open(Server.MapPath(sFilePathGiayThongTriCapPhatNgoaiTe));
            ThongTriCapPhatModel model = new ThongTriCapPhatModel();
            model = _qlnhService.GetThongTriByID(idThongTri);
            string sTiTle = "";
            if(model.iLoaiNoiDungChi == 1)
            {
                sTiTle = "THÔNG TRI CẤP KINH PHÍ BẰNG NGOẠI TỆ";
            }
            else
            {
                sTiTle = "THÔNG TRI CẤP KINH PHÍ BẰNG NỘI TỆ";
            }
            int iNam = model.iNamThongTri;
            string sDonVi = model.sTen;
            string soThongTri = model.sSoThongTri;
            List<ThongTriBaoCaoModel> lst = _qlnhService.ExportBaoCaoThongTriCapPhat(idThongTri).Select(x => new ThongTriBaoCaoModel
            {
                sM = x.sM,
                sTM = x.sTM,
                sTTM = x.sTTM,
                sNG = x.sNG,
                sTenNoiDungChi = x.sTenNoiDungChi,
                fPheDuyetCapKyNay_USD = x.fPheDuyetCapKyNay_USD,
                fPheDuyetCapKyNay_VND = x.fPheDuyetCapKyNay_VND
            }).ToList();
            decimal? fTongPheDuyet_USD = lst.Sum(x => x.fPheDuyetCapKyNay_USD);
            decimal? fTongPheDuyet_VND = lst.Sum(x => x.fPheDuyetCapKyNay_VND);
            fr.SetValue(new
            {
                To = 1
            });
            fr.AddTable<ThongTriBaoCaoModel>("dt", lst);
            fr.SetValue("sTiTle", sTiTle);
            fr.SetValue("iNam", iNam);
            fr.SetValue("sDonVi", sDonVi);
            fr.SetValue("sSoThongTri", soThongTri);
            fr.SetValue("fTongPheDuyet_USD", fTongPheDuyet_USD);
            fr.SetValue("fTongPheDuyet_VND", fTongPheDuyet_VND);
            fr.SetValue("sTongPheDuyet_VND", CommonFunction.NumberToText(fTongPheDuyet_VND.HasValue ? (double)fTongPheDuyet_VND.Value : 0));
            fr.SetValue("sDonViCapTren", donvicaptren);
            fr.SetValue("sDonViCapDuoi", donvi);
            fr.UseChuKy(Username)
             .UseChuKyForController(sControlName)
             .UseForm(this).Run(Result);
            return Result;
        }
        [HttpPost]
        public ActionResult UpdateChuKy()
        {
            return PartialView("_modalGetpopupBaoCao");
        }
    }
    
}