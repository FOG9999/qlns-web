using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Models.Shared;
using Viettel.Services;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;

namespace VIETTEL.Areas.QLNH.Controllers.QuyetToan
{
    public class ThongTriQuyetToanController : FlexcelReportController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private const string sFilePathBaoCao = "/Report_ExcelFrom/QLNH/rpt_ThongTriQuyetToan.xlsx";
        private const string sControlName = "ThongTriQuyetToan";

        // GET: QLNH/ThongTriQuyetToan
        public ActionResult Index()
        {
            var lstDonViQuanLy = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "-- Chọn đơn vị --" });
            ViewBag.LookupDonVi = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "-- Chọn chương trình --" });
            ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            var result = new NH_QT_ThongTriQuyetToanViewModel();
            result = _qlnhService.GetListThongTriQuyetToan(result._paging, new NH_QT_ThongTriQuyetToanFilter());

            return View(result);
        }

        // Tìm kiếm
        [HttpPost]
        public ActionResult TimKiem(PagingInfo paging, NH_QT_ThongTriQuyetToanFilter filter)
        {
            if (paging == null)
            {
                paging = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = Constants.ITEMS_PER_PAGE
                };
            }

            var lstDonViQuanLy = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "-- Chọn đơn vị --" });
            ViewBag.LookupDonVi = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            List<NH_KHChiTietBQP_NhiemVuChi> lstChuongTrinh = _qlnhService.GetNHKeHoachChiTietBQPNhiemVuChiList().ToList();
            lstChuongTrinh.Insert(0, new NH_KHChiTietBQP_NhiemVuChi { ID = Guid.Empty, sTenNhiemVuChi = "-- Chọn chương trình --" });
            ViewBag.ListChuongTrinh = lstChuongTrinh.ToSelectList("ID", "sTenNhiemVuChi");

            filter.sSoThongTri = HttpUtility.HtmlDecode(filter.sSoThongTri);
            var result = _qlnhService.GetListThongTriQuyetToan(paging, filter);

            return PartialView("_list", result);
        }

        // View UI create or update
        [HttpPost]
        public ActionResult ViewToCreateOrUpdate(Guid? id, string state = "CREATE")
        {
            ViewBag.State = state;
            var result = new NH_QT_ThongTriQuyetToanCreateDto();

            if (id.HasValue && id != Guid.Empty)
            {
                // Lấy lookup đơn vị
                var lstDonViQuanLy = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "-- Chọn đơn vị --" });
                ViewBag.LookupDonVi = lstDonViQuanLy;

                // Lấy thông tin thông tri
                result.ThongTriQuyetToan = _qlnhService.GetThongTinQuyetToanById(id.Value);
                result.ThongTriQuyetToan_ChiTiet = _qlnhService.GetListThongTriQuyetToanChiTietByTTQTId(id.Value).ToList();

                // Lấy lookup chương trình theo đơn vị của thông tri
                var lstChuongTrinh = _qlnhService.GetLookupBQPNhiemVuChiByDonViId(result.ThongTriQuyetToan.iID_DonViID);
                ViewBag.ListChuongTrinh = lstChuongTrinh.ToList();
            }
            else
            {
                var lstDonViQuanLy = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
                lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "-- Chọn đơn vị --" });
                ViewBag.LookupDonVi = lstDonViQuanLy;

                var lstChuongTrinh = new List<LookupDto<Guid, string>>();
                lstChuongTrinh.Add(new LookupDto<Guid, string> { Id = Guid.Empty, DisplayName = "-- Chọn chương trình --" });
                ViewBag.ListChuongTrinh = lstChuongTrinh.ToList();

                result.ThongTriQuyetToan.dNgayLap = DateTime.Now;
            }

            return PartialView("_viewCreateOrUpdate", result);
        }

        // Get lookup chương trình khi chọn đơn vị
        [HttpPost]
        public ActionResult GetLookupChuongTrinhByDonViId(Guid? iID_DonViID)
        {
            var result = _qlnhService.GetLookupBQPNhiemVuChiByDonViId(iID_DonViID).ToList();
            return Json(new { 
                data = result
            });
        }

        // Get data json thông tri quyết toán chi tiết
        [HttpPost]
        public ActionResult GetDataThongTriQTCT(Guid? iID_DonViID, Guid? iID_KHCTBQP_ChuongTrinhID, int? iNamThucHien)
        {
            var result = GetChiTietThongTriQuyetToan(iID_DonViID, iID_KHCTBQP_ChuongTrinhID, iNamThucHien);
            return Json(new { 
                data = result
            });
        }

        // Get list thông tri quyết toán chi tiết
        [HttpPost]
        public List<NH_QT_ThongTriQuyetToan_ChiTietModel> GetChiTietThongTriQuyetToan(Guid? iID_DonViID, Guid? iID_KHCTBQP_ChuongTrinhID, int? iNamThucHien)
        {
            /** NOTE **
                * Clone từ BaoCaoSoChuyenNamSauController - getListDetailChiTiet
                * Mặc định chỉ lấy theo 1 chương trình
            ** END - NOTE **/

            // Khỏi tạo kết quả
            var listResult = new List<NH_QT_ThongTriQuyetToan_ChiTietModel>();

            // Lấy data chi tiết
            var listDetail = _qlnhService.GetChiTietThongTriQuyetToan(iID_DonViID, iID_KHCTBQP_ChuongTrinhID, iNamThucHien).ToList();

            // 1 bước trung gian
            var listData = listDetail;

            // Lấy 1 list có ParentID
            var listTitle = listDetail.Where(x => x.iID_ParentID != null).ToList();

            // Get info chương trình
            var chuongTrinh = listData.FirstOrDefault();

            if (chuongTrinh == null)
            {
                return listResult;
            }

            // Lấy nhiệm vụ chi
            var newObj = new NH_QT_ThongTriQuyetToan_ChiTietModel()
            {
                sMaThuTu = ConvertLetter(1),
                sTenNoiDungChi = chuongTrinh.sTenNhiemVuChi,
                bIsTittle = true,
                fDeNghiQuyetToanNam_USD = listData.Sum(x => x.fDeNghiQuyetToanNam_USD),
                fDeNghiQuyetToanNam_VND = listData.Sum(x => x.fDeNghiQuyetToanNam_VND),
                fThuaNopTraNSNN_USD = listData.Sum(x => x.fThuaNopTraNSNN_USD),
                fThuaNopTraNSNN_VND = listData.Sum(x => x.fThuaNopTraNSNN_VND),
                bIsNhiemVuChi = true
            };
            // Add nhiệm vụ chi to result
            listResult.Add(newObj);

            // Lấy những bản ghi có thông tin dự án
            var getListDuAn = listData.Where(x => x.iID_DuAnID != null && x.iID_ParentID == null).ToList();
            // Lấy những bản ghi có thông tin hợp đồng nhưng ko có dự án
            var getListHopDong = listData.Where(x => x.iID_DuAnID == null && x.iID_HopDongID != null && x.iID_ParentID == null).ToList();
            // Lấy những bản ghi không có thông tin hợp đồng và cũng ko có thông tin dự án
            var getListNone = listData.Where(x => x.iID_DuAnID == null && x.iID_HopDongID == null && x.iID_ParentID == null).ToList();
            var iCountDuAn = 0;

            // Nếu có dự án thì sum zô rồi add vào result
            if (getListDuAn.Any())
            {
                var getNameDuAn = getListDuAn.Select(x => new { x.sTenDuAn, x.iID_DuAnID }).Distinct().ToList();
                foreach (var hopDongDuAn in getNameDuAn)
                {
                    iCountDuAn++;
                    var newObjHopDongDuAn = new NH_QT_ThongTriQuyetToan_ChiTietModel();

                    // Tìm title
                    var findTittle = listTitle.Find(x => x.iID_ParentID == hopDongDuAn.iID_DuAnID && x.iID_DuAnID == hopDongDuAn.iID_DuAnID);

                    // Get data theo dự án
                    var dataSumDuAn = listData.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).ToList();

                    // Nếu tìm thấy title
                    if (findTittle != null)
                    {
                        newObjHopDongDuAn.MapFrom(findTittle);
                        newObjHopDongDuAn.fDeNghiQuyetToanNam_USD = findTittle.fDeNghiQuyetToanNam_USD != null ? findTittle.fDeNghiQuyetToanNam_USD : dataSumDuAn.Sum(x => x.fDeNghiQuyetToanNam_USD);
                        newObjHopDongDuAn.fDeNghiQuyetToanNam_VND = findTittle.fDeNghiQuyetToanNam_VND != null ? findTittle.fDeNghiQuyetToanNam_VND : dataSumDuAn.Sum(x => x.fDeNghiQuyetToanNam_VND);
                        newObjHopDongDuAn.fThuaNopTraNSNN_USD = findTittle.fThuaNopTraNSNN_USD != null ? findTittle.fThuaNopTraNSNN_USD : dataSumDuAn.Sum(x => x.fThuaNopTraNSNN_USD);
                        newObjHopDongDuAn.fThuaNopTraNSNN_VND = findTittle.fThuaNopTraNSNN_VND != null ? findTittle.fThuaNopTraNSNN_VND : dataSumDuAn.Sum(x => x.fThuaNopTraNSNN_VND);
                    } 
                    else
                    {
                        newObjHopDongDuAn.fDeNghiQuyetToanNam_USD = dataSumDuAn.Sum(x => x.fDeNghiQuyetToanNam_USD);
                        newObjHopDongDuAn.fDeNghiQuyetToanNam_VND = dataSumDuAn.Sum(x => x.fDeNghiQuyetToanNam_VND);
                        newObjHopDongDuAn.fThuaNopTraNSNN_USD = dataSumDuAn.Sum(x => x.fThuaNopTraNSNN_USD);
                        newObjHopDongDuAn.fThuaNopTraNSNN_VND = dataSumDuAn.Sum(x => x.fThuaNopTraNSNN_VND);
                    }

                    // Gán thêm các thông tin khác
                    newObjHopDongDuAn.sMaThuTu = ConvertLaMa(Decimal.Parse(iCountDuAn.ToString()));
                    newObjHopDongDuAn.sTenNoiDungChi = hopDongDuAn.sTenDuAn;
                    newObjHopDongDuAn.bIsTittle = true;
                    newObjHopDongDuAn.bIsData = true;
                    newObjHopDongDuAn.iID_ParentID = hopDongDuAn.iID_DuAnID;
                    newObjHopDongDuAn.iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID;
                    newObjHopDongDuAn.iID_DuAnID = hopDongDuAn.iID_DuAnID;

                    listResult.Add(newObjHopDongDuAn);
                    listResult.AddRange(returnLoaiChi(chuongTrinh.iID_KHCTBQP_NhiemVuChiID, hopDongDuAn.iID_DuAnID, true, getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).ToList(), listTitle));
                }
            }

            // Nếu có hợp đồng thì cũng sum zô rồi add vòa result
            if (getListHopDong.Any())
            {
                iCountDuAn++;
                var getSumHopDong = listData.Where(x => x.iID_DuAnID == null && x.iID_HopDongID != null).ToList();

                var newObjHopDong = new NH_QT_ThongTriQuyetToan_ChiTietModel()
                {
                    sMaThuTu = ConvertLaMa(Decimal.Parse(iCountDuAn.ToString())),
                    sTenNoiDungChi = "Chi hợp đồng",
                    bIsTittle = true,
                    fDeNghiQuyetToanNam_USD = getSumHopDong.Sum(x => x.fDeNghiQuyetToanNam_USD),
                    fDeNghiQuyetToanNam_VND = getSumHopDong.Sum(x => x.fDeNghiQuyetToanNam_VND),
                    fThuaNopTraNSNN_USD = getSumHopDong.Sum(x => x.fThuaNopTraNSNN_USD),
                    fThuaNopTraNSNN_VND = getSumHopDong.Sum(x => x.fThuaNopTraNSNN_VND)
                };
                listResult.Add(newObjHopDong);
                listResult.AddRange(returnLoaiChi(chuongTrinh.iID_KHCTBQP_NhiemVuChiID, null, true, getListHopDong, listTitle));
            }

            if (getListNone.Any())
            {
                iCountDuAn++;
                var newObjKhac = new NH_QT_ThongTriQuyetToan_ChiTietModel()
                {
                    sMaThuTu = ConvertLaMa(Decimal.Parse(iCountDuAn.ToString())),
                    sTenNoiDungChi = "Chi khác",
                    bIsTittle = true,
                    fDeNghiQuyetToanNam_USD = getListNone.Sum(x => x.fDeNghiQuyetToanNam_USD),
                    fDeNghiQuyetToanNam_VND = getListNone.Sum(x => x.fDeNghiQuyetToanNam_VND),
                    fThuaNopTraNSNN_USD = getListNone.Sum(x => x.fThuaNopTraNSNN_USD),
                    fThuaNopTraNSNN_VND = getListNone.Sum(x => x.fThuaNopTraNSNN_VND),
                };
                listResult.Add(newObjKhac);
                listResult.AddRange(returnLoaiChi(chuongTrinh.iID_KHCTBQP_NhiemVuChiID, null, false, getListNone, listTitle));
            }

            return listResult;
        }

        // Lưu thông tri quyết toán và thông tri quyết toán chi tiết
        public bool SaveThongTriQuyetToan(NH_QT_ThongTriQuyetToanCreateDto input, string state = "CREATE")
        {
            input.ThongTriQuyetToan.sSoThongTri = HttpUtility.HtmlDecode(input.ThongTriQuyetToan.sSoThongTri);
            input.ThongTriQuyetToan.iID_MaDonVi = HttpUtility.HtmlDecode(input.ThongTriQuyetToan.iID_MaDonVi);
            return _qlnhService.SaveThongTriQuyetToan(input, state);
        }

        public bool DeleteThongTriQuyetToan(Guid id)
        {
            return _qlnhService.DeleteThongTriQuyetToan(id);
        }

        // Clone từ BaoCaoSoChuyenNamSauController
        //private List<NH_QT_ThongTriQuyetToan_ChiTietModel> returnLoaiChi(Guid? idChuongTrinh, Guid? idDuAn, bool isDuAn, List<NH_QT_ThongTriQuyetToan_ChiTietModel> list, List<NH_QT_ThongTriQuyetToan_ChiTietModel> listTittle)
        //{
        //    List<NH_QT_ThongTriQuyetToan_ChiTietModel> returnData = new List<NH_QT_ThongTriQuyetToan_ChiTietModel>();
        //    var listLoaiChiPhi = list.Select(x => new { x.iLoaiNoiDungChi }).Distinct().OrderBy(x => x.iLoaiNoiDungChi).ToList();
        //    var countLoaiChiPhi = 0;
        //    foreach (var loaiChiPhi in listLoaiChiPhi)
        //    {
        //        var dataSumLoaiChi = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).ToList();

        //        countLoaiChiPhi++;
        //        var newObjLoaiChiPhi = new NH_QT_ThongTriQuyetToan_ChiTietModel()
        //        {
        //            sMaThuTu = countLoaiChiPhi.ToString(),
        //            sTenNoiDungChi = loaiChiPhi.iLoaiNoiDungChi == 1 ? "Chi ngoại tệ" : "Chi trong nước",
        //            bIsTittle = true,
        //            fDeNghiQuyetToanNam_USD = dataSumLoaiChi.Sum(x => x.fDeNghiQuyetToanNam_USD) + listTittle.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fDeNghiQuyetToanNam_USD),
        //            fDeNghiQuyetToanNam_VND = dataSumLoaiChi.Sum(x => x.fDeNghiQuyetToanNam_VND) + listTittle.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fDeNghiQuyetToanNam_VND),
        //            fThuaNopTraNSNN_USD = dataSumLoaiChi.Sum(x => x.fThuaNopTraNSNN_USD) + listTittle.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fThuaNopTraNSNN_USD),
        //            fThuaNopTraNSNN_VND = dataSumLoaiChi.Sum(x => x.fThuaNopTraNSNN_VND) + listTittle.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Sum(x => x.fThuaNopTraNSNN_VND),
        //        };
        //        returnData.Add(newObjLoaiChiPhi);

        //        if (isDuAn)
        //        {
        //            var listNameHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Select(x => new { x.sTenHopDong, x.iID_HopDongID }).Distinct()
        //            .ToList();
        //            var countHopDong = 0;
        //            foreach (var nameHopDong in listNameHopDong)
        //            {
        //                countHopDong++;
        //                var newObjHopDongDuAn = new NH_QT_ThongTriQuyetToan_ChiTietModel();
        //                var findTittle = listTittle.Find(x => x.iID_HopDongID == nameHopDong.iID_HopDongID && x.iID_DuAnID == idDuAn && x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi);
        //                var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi && x.iID_HopDongID == nameHopDong.iID_HopDongID).ToList();

        //                if (findTittle != null)
        //                {
        //                    newObjHopDongDuAn.MapFrom(findTittle);
        //                    newObjHopDongDuAn.fDeNghiQuyetToanNam_USD = findTittle.fDeNghiQuyetToanNam_USD != null ? findTittle.fDeNghiQuyetToanNam_USD : listHopDong.Sum(x => x.fDeNghiQuyetToanNam_USD);
        //                    newObjHopDongDuAn.fDeNghiQuyetToanNam_VND = findTittle.fDeNghiQuyetToanNam_VND != null ? findTittle.fDeNghiQuyetToanNam_VND : listHopDong.Sum(x => x.fDeNghiQuyetToanNam_VND);
        //                    newObjHopDongDuAn.fThuaNopTraNSNN_USD = findTittle.fThuaNopTraNSNN_USD != null ? findTittle.fThuaNopTraNSNN_USD : listHopDong.Sum(x => x.fThuaNopTraNSNN_USD);
        //                    newObjHopDongDuAn.fThuaNopTraNSNN_VND = findTittle.fThuaNopTraNSNN_VND != null ? findTittle.fThuaNopTraNSNN_VND : listHopDong.Sum(x => x.fThuaNopTraNSNN_VND);
        //                }
        //                else
        //                {
        //                    newObjHopDongDuAn.fDeNghiQuyetToanNam_USD = listHopDong.Sum(x => x.fDeNghiQuyetToanNam_USD);
        //                    newObjHopDongDuAn.fDeNghiQuyetToanNam_VND = listHopDong.Sum(x => x.fDeNghiQuyetToanNam_VND);
        //                    newObjHopDongDuAn.fThuaNopTraNSNN_USD = listHopDong.Sum(x => x.fThuaNopTraNSNN_USD);
        //                    newObjHopDongDuAn.fThuaNopTraNSNN_VND = listHopDong.Sum(x => x.fThuaNopTraNSNN_VND);
        //                }

        //                newObjHopDongDuAn.sMaThuTu = countLoaiChiPhi.ToString() + "." + countHopDong.ToString();
        //                newObjHopDongDuAn.sTenNoiDungChi = nameHopDong.sTenHopDong;
        //                newObjHopDongDuAn.bIsData = true;
        //                newObjHopDongDuAn.bIsTittle = true;
        //                //newObjHopDongDuAn.sLevel = "2";
        //                newObjHopDongDuAn.iID_ParentID = nameHopDong.iID_HopDongID;
        //                newObjHopDongDuAn.iID_KHCTBQP_NhiemVuChiID = idChuongTrinh;
        //                newObjHopDongDuAn.iID_HopDongID = nameHopDong.iID_HopDongID;
        //                newObjHopDongDuAn.iID_DuAnID = idDuAn;
        //                newObjHopDongDuAn.iID_ThanhToan_ChiTietID = listHopDong.FirstOrDefault().iID_ThanhToan_ChiTietID;

        //                returnData.Add(newObjHopDongDuAn);
        //            }
        //        }
        //    }
        //    return returnData;
        //}

        public List<NH_QT_ThongTriQuyetToan_ChiTietModel> returnLoaiChi(Guid? idChuongTrinh, Guid? idDuAn, bool isDuAn, List<NH_QT_ThongTriQuyetToan_ChiTietModel> list, List<NH_QT_ThongTriQuyetToan_ChiTietModel> listTittle)
        {
            List<NH_QT_ThongTriQuyetToan_ChiTietModel> returnData = new List<NH_QT_ThongTriQuyetToan_ChiTietModel>();
            var listLoaiChiPhi = list.Select(x => new { x.iLoaiNoiDungChi }).Distinct().OrderBy(x => x.iLoaiNoiDungChi)
                  .ToList();
            var countLoaiChiPhi = 0;
            foreach (var loaiChiPhi in listLoaiChiPhi)
            {
                countLoaiChiPhi++;
                var newObjLoaiChiPhi = new NH_QT_ThongTriQuyetToan_ChiTietModel()
                {
                    sMaThuTu = countLoaiChiPhi.ToString(),
                    sTenNoiDungChi = loaiChiPhi.iLoaiNoiDungChi == 1 ? "Chi ngoại tệ" : "Chi trong nước",
                    bIsTittle = true
                };
                returnData.Add(newObjLoaiChiPhi);

                if (isDuAn)
                {
                    var listNameHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Select(x => new { x.sTenHopDong, x.iID_HopDongID }).Distinct()
                    .ToList();
                    var countHopDong = 0;
                    foreach (var nameHopDong in listNameHopDong)
                    {
                        countHopDong++;
                        var newObjHopDongDuAn = new NH_QT_ThongTriQuyetToan_ChiTietModel();
                        var findTittle = listTittle.Find(x => x.iID_HopDongID == nameHopDong.iID_HopDongID && x.iID_DuAnID == idDuAn && x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi);
                        if (findTittle != null)
                        {
                            newObjHopDongDuAn.MapFrom(findTittle);
                        }
                        newObjHopDongDuAn.sMaThuTu = countLoaiChiPhi.ToString() + "." + countHopDong.ToString();
                        newObjHopDongDuAn.sTenNoiDungChi = nameHopDong.sTenHopDong;
                        newObjHopDongDuAn.bIsData = true;
                        newObjHopDongDuAn.bIsTittle = true;
                        newObjHopDongDuAn.iID_ParentID = nameHopDong.iID_HopDongID;
                        newObjHopDongDuAn.iID_KHCTBQP_NhiemVuChiID = idChuongTrinh;
                        newObjHopDongDuAn.iID_HopDongID = nameHopDong.iID_HopDongID;
                        newObjHopDongDuAn.iID_DuAnID = idDuAn;

                        var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi && x.iID_HopDongID == nameHopDong.iID_HopDongID).ToList();
                        newObjHopDongDuAn.iID_ThanhToan_ChiTietID = listHopDong.FirstOrDefault().iID_ThanhToan_ChiTietID;

                        returnData.Add(newObjHopDongDuAn);

                        listHopDong.ForEach(x =>
                        {
                            x.bIsData = true;
                        });
                        returnData.AddRange(listHopDong);
                    }
                }
                else
                {

                    var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).ToList();
                    listHopDong.ForEach(x =>
                    {
                        x.bIsData = true;
                    });
                    returnData.AddRange(listHopDong);
                }

            }
            return returnData;
        }

        public ActionResult ViewModalExportReport(Guid id)
        {
            ViewBag.IdThongTriQuyetToan = id;
            return PartialView("_modalReport");
        }

        // Export report
        public ActionResult ExportFile(Guid id, string sDonViTienTe = "USD", string ext = "xlsx")
        {
            // Lấy thông tin thông tri
            var ttqt = _qlnhService.GetThongTinQuyetToanById(id);
            var ttqt_ct = _qlnhService.GetListThongTriQuyetToanChiTietByTTQTId(id).ToList();
            string fileName = string.Format("{0} {1}.{2}", "Báo cáo thông tri quyết toán năm ", ttqt.iNamThongTri, ext);
            ExcelFile xls = TaoFileExel(ttqt, ttqt_ct, sDonViTienTe);
            return Print(xls, ext, fileName);
        }

        // Tạo file report
        public ExcelFile TaoFileExel(NH_QT_ThongTriQuyetToanModel ttqt, List<NH_QT_ThongTriQuyetToan_ChiTietModel> ttqt_ct, string sDonViTienTe)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao));
            FlexCelReport fr = new FlexCelReport();

            var tongtien = sDonViTienTe == "USD" ? ttqt.fThongTri_USD : ttqt.fThongTri_VND;

            fr.SetValue(new
            {
                sSoThongTri = ttqt.sSoThongTri,
                sLoaiThongTri = ttqt.iLoaiThongTri == 2 ? "Giảm quyết toán" : "Quyết toán",
                sTenDonVi = ttqt.sTenDonVi,
                sTongTien = sDonViTienTe == "VND" ? DomainModel.CommonFunction.TienRaChu(tongtien.HasValue ? (long)tongtien : 0) : "",
                fTongTien = tongtien.HasValue ? DomainModel.CommonFunction.DinhDangSo(tongtien.Value.ToString("0.00" + new string('#', 397), CultureInfo.InvariantCulture)) : string.Empty,
                iNamThongTri = ttqt.iNamThongTri
            });

            //fr.SetValue("iTongSoNgayDieuTri", iTongSoNgayDieuTri.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")));
            foreach (var item in ttqt_ct)
            {
                item.sMoneyReport = ttqt.iLoaiThongTri == 1
                    ? sDonViTienTe == "USD"
                        ? item.fDeNghiQuyetToanNam_USD.HasValue ? DomainModel.CommonFunction.DinhDangSo(item.fDeNghiQuyetToanNam_USD.Value.ToString("0.00", CultureInfo.InvariantCulture)) : "0"
                        : item.fDeNghiQuyetToanNam_VND.HasValue ? DomainModel.CommonFunction.DinhDangSo(item.fDeNghiQuyetToanNam_VND.Value.ToString("0.00", CultureInfo.InvariantCulture)) : "0"
                    : sDonViTienTe == "USD"
                        ? item.fThuaNopTraNSNN_USD.HasValue ? DomainModel.CommonFunction.DinhDangSo(item.fThuaNopTraNSNN_USD.Value.ToString("0.00", CultureInfo.InvariantCulture)) : "0"
                        : item.fThuaNopTraNSNN_VND.HasValue ? DomainModel.CommonFunction.DinhDangSo(item.fThuaNopTraNSNN_VND.Value.ToString("0.00", CultureInfo.InvariantCulture)) : "0";
            }

            fr.AddTable<NH_QT_ThongTriQuyetToan_ChiTietModel>("dt", ttqt_ct);

            fr.UseChuKy(Username)
              .UseChuKyForController(sControlName)
              .UseForm(this).Run(Result);

            return Result;
        }

        // Clone từ BaoCaoSoChuyenNamSauController
        private string ConvertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }

        // Clone từ BaoCaoSoChuyenNamSauController
        private string ConvertLaMa(decimal num)
        {
            string strRet = string.Empty;
            decimal _Number = num;
            Boolean _Flag = true;
            string[] ArrLama = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            int[] ArrNumber = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            int i = 0;
            while (_Flag)
            {
                while (_Number >= ArrNumber[i])
                {
                    _Number -= ArrNumber[i];
                    strRet += ArrLama[i];
                    if (_Number < 1)
                        _Flag = false;
                }
                i++;
            }
            return strRet;
        }
    }
}