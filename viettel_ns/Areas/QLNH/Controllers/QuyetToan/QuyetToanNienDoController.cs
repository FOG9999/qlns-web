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
using Viettel.Models.QLNH.QuyetToan;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Flexcel;
using System.Web;

namespace VIETTEL.Areas.QLNH.Controllers.QuyetToan
{
    public class QuyetToanNienDoController : FlexcelReportController
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sFilePathBaoCao = "/Report_ExcelFrom/QLNH/rpt_QuyetToanNienDo3.xlsx";
        private const string sControlName = "QuyetToanNienDo";
        public List<Dropdown_SelectValue> lstDonViVND = new List<Dropdown_SelectValue>()
            {
                new Dropdown_SelectValue()
                {
                    Value = 1,
                    Label = "Đồng"
                },
                 new Dropdown_SelectValue()
                {
                    Value = 1000,
                    Label = "Nghìn đồng"
                }, new Dropdown_SelectValue()
                {
                    Value = 1000000000,
                    Label = "Tỉ đồng"
                }
            };
        public List<Dropdown_SelectValue> lstDonViUSD = new List<Dropdown_SelectValue>()
            {
                new Dropdown_SelectValue()
                {
                    Value = 1,
                    Label = "USD"
                },
                 new Dropdown_SelectValue()
                {
                    Value = 1000,
                    Label = "Nghìn USD"
                }, new Dropdown_SelectValue()
                {
                    Value = 1000000000,
                    Label = "Tỉ USD"
                }
            };
        public List<Dropdown_SelectValue> lstLoaiQuyetToan = new List<Dropdown_SelectValue>()
            {
                new Dropdown_SelectValue()
                {
                    Value = 1,
                    Label = "Quyết toán theo dự án"
                },
                 new Dropdown_SelectValue()
                {
                    Value = 2,
                    Label = "Quyết toán theo hợp đồng"
                }
            };
        public List<string> arr = new List<string>()
            {
                "USD",
                "VND"
            };
        // GET: QLNH/QuyetToanNienDo
        public ActionResult Index()
        {
            QuyetToan_QuyetToanNienDoModel vm = new QuyetToan_QuyetToanNienDoModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListQuyetToanNienDo(ref vm._paging, null, null, null, null, 0);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<Dropdown_SelectValue> lstNamKeHoach = GetListNamKeHoach();
            lstNamKeHoach.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKeHoach = lstNamKeHoach;

            return View(vm);
        }
        [HttpPost]
        public ActionResult GetListTongHopQuyetToan(string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamKeHoach)
        {
            sSoDeNghi = HttpUtility.HtmlDecode(sSoDeNghi);
            var ListTongHopQuyetToan = _qlnhService.GetListTongHopQuyetToanNienDo(sSoDeNghi, dNgayDeNghi, iDonVi, iNamKeHoach);
            ViewBag.TabIndex = 1;

            return Json(new { data = ListTongHopQuyetToan }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(string id, bool edit)
        {
            NH_QT_QuyetToanNienDo_ChiTietView vm = new NH_QT_QuyetToanNienDo_ChiTietView();
           
            var quyetToanDonVi = getQuyetToanDonVi(id);
            var quyetToanNienDoDetail = _qlnhService.GetThongTinQuyetToanNienDoById(new Guid(id));
            var donVi = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).Where(x => x.iID_Ma == quyetToanNienDoDetail.iID_DonViID).FirstOrDefault();
            if (donVi != null)
            {
                quyetToanNienDoDetail.sTenDonVi = donVi.iID_MaDonVi + " - " + donVi.sTen;
            }
            vm.QuyetToanNienDoDetail = quyetToanNienDoDetail;
            var tiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTiet(vm.QuyetToanNienDoDetail.iID_TiGiaID, false).Where(x => arr.Contains(x.sMaTienTeQuyDoi)).FirstOrDefault();

            vm.QuyetToanNienDoDetail.fDonViTiGia = tiGiaChiTiet != null ? tiGiaChiTiet.fTiGia : 1;

            var tiGia = _qlnhService.GetNHDMTiGiaList(vm.QuyetToanNienDoDetail.iID_TiGiaID).FirstOrDefault();
            vm.QuyetToanNienDoDetail.sMaTienTeGoc = tiGia.sMaTienTeGoc;
            var listResult = getListDetailChiTiet(quyetToanDonVi, false, null, null);
            vm.ListDetailQuyetToanNienDo = listResult;

            ViewBag.IsAddOrUpdate = checkIsAddOrUpdate(quyetToanDonVi, null, null);


            ViewBag.IsHaveValue = listResult.Any();
            ViewBag.IsEdit = edit;
            ViewBag.IdQuyetToan = id;
            return View(vm);
        }

        [HttpPost]
        public JsonResult GetListDonvi()
        {
            var result = new List<dynamic>();
            var listModel = _qlnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.iID_Ma,
                        text = item.iID_MaDonVi + " - " + item.sTen
                    });
                }
            }
            return Json(new { status = true, data = result });
        }
        [HttpPost]
        public JsonResult GetListDropDownNamKeHoach()
        {
            var result = new List<dynamic>();
            var listModel = GetListNamKeHoach();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.Value,
                        text = item.Label
                    });
                }
            }
            return Json(new { status = true, data = result });
        }
        public List<NH_QT_QuyetToanNienDoByDonVi> getQuyetToanDonVi(string id)
        {
            var quyetToanNienDoDetail = _qlnhService.GetThongTinQuyetToanNienDoById(new Guid(id));
            List<NH_QT_QuyetToanNienDoByDonVi> qtndDonVi = new List<NH_QT_QuyetToanNienDoByDonVi>();
            if (quyetToanNienDoDetail.iID_DonViID != null)
            {
                var donVi = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).Where(x => x.iID_Ma == quyetToanNienDoDetail.iID_DonViID).FirstOrDefault();
                if (donVi != null)
                {
                    quyetToanNienDoDetail.sTenDonVi = donVi.iID_MaDonVi + " - " + donVi.sTen;
                }
                qtndDonVi.Add(new NH_QT_QuyetToanNienDoByDonVi()
                {
                    donVi = donVi,
                    quyetToanNienDo = quyetToanNienDoDetail
                });
            }
            else
            {
                foreach (var item in quyetToanNienDoDetail.sTongHop.Split(','))
                {
                    var qtndDetail = _qlnhService.GetThongTinQuyetToanNienDoById(new Guid(item));
                    var donVi = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).Where(x => x.iID_Ma == qtndDetail.iID_DonViID).FirstOrDefault();
                    if (donVi != null)
                    {
                        quyetToanNienDoDetail.sTenDonVi += donVi.iID_MaDonVi + " - " + donVi.sTen + ",";
                    }
                    qtndDonVi.Add(new NH_QT_QuyetToanNienDoByDonVi()
                    {
                        donVi = donVi,
                        quyetToanNienDo = qtndDetail
                    });
                }
            }
            return qtndDonVi;
        }
        public int checkIsAddOrUpdate(List<NH_QT_QuyetToanNienDoByDonVi> quyetToanNienDoDetail, int? donViUSD, int? donViVND)
        {
            var check = 0;
            foreach (var qtdv in quyetToanNienDoDetail)
            {
                var listDetail = _qlnhService.GetDetailQuyetToanNienDoDetail(qtdv.quyetToanNienDo.iNamKeHoach, qtdv.donVi.iID_Ma, qtdv.quyetToanNienDo.ID, donViUSD, donViVND).ToList();
                //var listTitle = new List<NH_QT_QuyetToanNienDo_ChiTietData>();

                if (listDetail.Any())
                {
                    check = 1;
                }
            }
            return check;
        }
        public List<NH_QT_QuyetToanNienDo_ChiTietData> getListDetailChiTiet(List<NH_QT_QuyetToanNienDoByDonVi> quyetToanNienDoDetail, bool isPrint, int? donViUSD, int? donViVND)
        {
            var listData = new List<NH_QT_QuyetToanNienDo_ChiTietData>();
            var listResult = new List<NH_QT_QuyetToanNienDo_ChiTietData>();

            foreach (var qtdv in quyetToanNienDoDetail)
            {
                var tiGia = _qlnhService.GetNHDMTiGiaChiTiet(qtdv.quyetToanNienDo.iID_TiGiaID, false).Where(x => arr.Contains(x.sMaTienTeQuyDoi)).FirstOrDefault().fTiGia;
                var maTiGia = _qlnhService.GetNHDMTiGiaList(qtdv.quyetToanNienDo.iID_TiGiaID).FirstOrDefault().sMaTienTeGoc;
                var listDetail = _qlnhService.GetDetailQuyetToanNienDoDetail(qtdv.quyetToanNienDo.iNamKeHoach, qtdv.donVi.iID_Ma, qtdv.quyetToanNienDo.ID, donViUSD, donViVND).ToList();
                //var listTitle = new List<NH_QT_QuyetToanNienDo_ChiTietData>();

                if (listDetail.Any())
                {
                    listData = listDetail;
                }
                else
                {
                    listData = _qlnhService.GetDetailQuyetToanNienDoCreate(qtdv.quyetToanNienDo.iNamKeHoach, qtdv.donVi.iID_Ma, donViUSD, donViVND).ToList();
                }
                var listTitle = listData.Where(x => x.iID_ParentID != null).ToList();
                if (quyetToanNienDoDetail.Count() > 1)
                {
                    var newObjDonVi = new NH_QT_QuyetToanNienDo_ChiTietData()
                    {
                        sTenNoiDungChi = qtdv.donVi.iID_MaDonVi + " - " + qtdv.donVi.sTen,
                        bIsTittle = true
                    };
                    listResult.Add(newObjDonVi);
                }
                var getAllChuongTrinh = listData.Where(x => x.iID_DonVi == qtdv.donVi.iID_Ma && x.iID_KHCTBQP_NhiemVuChiID != null && x.iID_ParentID == null).Select(x => new { x.sTenNhiemVuChi, x.iID_KHCTBQP_NhiemVuChiID, x.fKeHoach_TTCP_USD, x.fKeHoach_BQP_USD }).Distinct().ToList();

                var iCountChuongTrinh = 0;
                var iCurrentID = 0;

                foreach (var chuongTrinh in getAllChuongTrinh)
                {
                    iCountChuongTrinh++;
                    iCurrentID = listResult.Where(x => x.iCurrentId != 0).Count() + 1;
                    var newObj = new NH_QT_QuyetToanNienDo_ChiTietData()
                    {
                        STT = convertLetter(iCountChuongTrinh),
                        sTenNoiDungChi = chuongTrinh.sTenNhiemVuChi,
                        bIsTittle = true,
                        sLevel = "0",
                        iCurrentId = iCurrentID,
                        iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID,
                        fKeHoach_TTCP_USD = chuongTrinh.fKeHoach_TTCP_USD,
                        fKeHoach_BQP_USD = chuongTrinh.fKeHoach_BQP_USD,
                        iParentId = 0
                    };
                    listResult.Add(newObj);
                    var getListDuAn = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID != null && x.iID_ParentID == null).ToList();
                    var getListHopDong = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == null && x.iID_HopDongID != null && x.iID_ParentID == null).ToList();
                    var getListNone = qtdv.quyetToanNienDo.iLoaiQuyetToan == 1
                        ? listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == null && x.iID_HopDongID == null && x.iID_ParentID == null).ToList()
                        : new List<NH_QT_QuyetToanNienDo_ChiTietData>();
                    var iCountDuAn = 0;

                    if (getListDuAn.Any())
                    {
                        var getNameDuAn = getListDuAn.Select(x => new
                        {
                            x.sTenDuAn,
                            x.iID_DuAnID,
                            x.fHopDong_VND_DuAn,
                            x.fHopDong_USD_DuAn,
                            x.fLuyKeKinhPhiDuocCap_USD,
                            x.fLuyKeKinhPhiDuocCap_VND
                        }
                        )
                        .Distinct()
                        .ToList();
                        foreach (var hopDongDuAn in getNameDuAn)
                        {
                            iCountDuAn++;
                            iCurrentID = listResult.Where(x => x.iCurrentId != 0).Count() + 1;
                            var newObjHopDongDuAn = new NH_QT_QuyetToanNienDo_ChiTietData();
                            var findTittle = listTitle.Find(x => x.iID_ParentID == hopDongDuAn.iID_DuAnID && x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == hopDongDuAn.iID_DuAnID);
                            if (findTittle != null)
                            {
                                newObjHopDongDuAn.MapFrom(findTittle);

                            }

                            newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_USD = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamNay_USD : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD);
                            newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_VND = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamNay_VND : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND);
                            newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD = findTittle != null ? findTittle.fQTKinhPhiDuocCap_TongSo_USD : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD);
                            newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND = findTittle != null ? findTittle.fQTKinhPhiDuocCap_TongSo_VND : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND);
                            newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD);
                            newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = findTittle != null ? findTittle.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND : getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND);

                            newObjHopDongDuAn.fQTKinhPhiDuyetCacNamTruoc_USD = findTittle != null ? findTittle.fQTKinhPhiDuyetCacNamTruoc_USD : getListDuAn.Find(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).fQTKinhPhiDuyetCacNamTruoc_USD ;
                            newObjHopDongDuAn.fQTKinhPhiDuyetCacNamTruoc_VND = findTittle != null ? findTittle.fQTKinhPhiDuyetCacNamTruoc_VND : getListDuAn.Find(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).fQTKinhPhiDuyetCacNamTruoc_VND;

                            newObjHopDongDuAn.fLuyKeKinhPhiDuocCap_USD = findTittle != null ? findTittle.fLuyKeKinhPhiDuocCap_USD : (getListDuAn.Find(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).fLuyKeKinhPhiDuocCap_USD + 
                                (newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD==0
                                ?maTiGia.ToUpper()=="USD"
                                ? newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND / tiGia
                                : newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND * tiGia
                                : newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD
                                ));
                            newObjHopDongDuAn.fLuyKeKinhPhiDuocCap_VND = findTittle != null ? findTittle.fLuyKeKinhPhiDuocCap_VND : (getListDuAn.Find(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).fLuyKeKinhPhiDuocCap_VND + 
                                (
                                newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND==0
                                ? maTiGia.ToUpper() == "USD"
                                ? newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD * tiGia
                                : newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD / tiGia
                                : newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND
                                ));



                            newObjHopDongDuAn.STT = convertLaMa(Decimal.Parse(iCountDuAn.ToString()));
                            newObjHopDongDuAn.sTenNoiDungChi = hopDongDuAn.sTenDuAn;
                            newObjHopDongDuAn.bIsTittle = true;
                            newObjHopDongDuAn.bIsSaveTongHop = true;
                            newObjHopDongDuAn.fHopDong_VND = hopDongDuAn.fHopDong_VND_DuAn;
                            newObjHopDongDuAn.fHopDong_USD = hopDongDuAn.fHopDong_USD_DuAn;
                            newObjHopDongDuAn.bIsData = true;
                            newObjHopDongDuAn.sLevel = "1";
                            newObjHopDongDuAn.iCurrentId = iCurrentID;
                            if (isPrint)
                            {
                                newObjHopDongDuAn.fKeHoach_TTCP_USD = null;
                                newObjHopDongDuAn.fKeHoach_BQP_USD = null;
                            }

                            newObjHopDongDuAn.iParentId = newObj.iCurrentId;
                            newObjHopDongDuAn.iID_ParentID = hopDongDuAn.iID_DuAnID;
                            newObjHopDongDuAn.iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID;
                            newObjHopDongDuAn.iID_DuAnID = hopDongDuAn.iID_DuAnID;
                            newObjHopDongDuAn.iSum = 1;

                            listResult.Add(newObjHopDongDuAn);
                            listResult.AddRange(returnLoaiChi(isPrint, chuongTrinh.iID_KHCTBQP_NhiemVuChiID, hopDongDuAn.iID_DuAnID, iCurrentID, true, false, getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).ToList(), listTitle));
                        }
                    }
                    if (getListHopDong.Any())
                    {
                        iCountDuAn++;
                        iCurrentID = listResult.Where(x => x.iCurrentId != 0).Count() + 1;
                        var getThisList = listTitle.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == null && x.iID_HopDongID != null).ToList();

                        var newObjHopDong = new NH_QT_QuyetToanNienDo_ChiTietData()
                        {
                            STT = convertLaMa(decimal.Parse(iCountDuAn.ToString())),
                            sTenNoiDungChi = "Chi hợp đồng",
                            bIsTittle = true,
                            iCurrentId = iCurrentID,
                            iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID,
                            iParentId = newObj.iCurrentId,
                            iSum = 1,
                            fQTKinhPhiDuocCap_NamNay_USD = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD),
                            fQTKinhPhiDuocCap_NamNay_VND = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND),
                            fQTKinhPhiDuocCap_TongSo_USD = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD),
                            fQTKinhPhiDuocCap_TongSo_VND = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND),
                            fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD),
                            fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = getThisList.Any() ? getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND) : getListHopDong.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND),

                        };
                        if (isPrint)
                        {
                            newObjHopDong.fDeNghiQTNamNay_USD = getThisList.Sum(x => x.fDeNghiQTNamNay_USD);
                            newObjHopDong.fDeNghiQTNamNay_VND = getThisList.Sum(x => x.fDeNghiQTNamNay_VND);
                            newObjHopDong.fDeNghiChuyenNamSau_USD = getThisList.Sum(x => x.fDeNghiChuyenNamSau_USD);
                            newObjHopDong.fDeNghiChuyenNamSau_VND = getThisList.Sum(x => x.fDeNghiChuyenNamSau_VND);
                            newObjHopDong.fThuaNopNSNN_USD = getThisList.Sum(x => x.fThuaNopNSNN_USD);
                            newObjHopDong.fThuaNopNSNN_VND = getThisList.Sum(x => x.fThuaNopNSNN_VND);
                            newObjHopDong.fThuaThieuKinhPhiTrongNam_USD = getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD);
                            newObjHopDong.fThuaThieuKinhPhiTrongNam_VND = getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND);
                        }
                        listResult.Add(newObjHopDong);
                        listResult.AddRange(returnLoaiChi(isPrint, chuongTrinh.iID_KHCTBQP_NhiemVuChiID, null, iCurrentID, true, true, getListHopDong, listTitle));
                    }
                    if (getListNone.Any())
                    {
                        iCountDuAn++;
                        iCurrentID = listResult.Where(x => x.iCurrentId != 0).Count() + 1;
                        var newObjKhac = new NH_QT_QuyetToanNienDo_ChiTietData()
                        {
                            STT = convertLaMa(decimal.Parse(iCountDuAn.ToString())),
                            sTenNoiDungChi = "Chi khác",
                            bIsTittle = true,
                            iCurrentId = iCurrentID,
                            iID_KHCTBQP_NhiemVuChiID = chuongTrinh.iID_KHCTBQP_NhiemVuChiID,
                            iParentId = newObj.iCurrentId,
                            iSum = 1,
                            fQTKinhPhiDuocCap_NamNay_USD = getListNone.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD),
                            fQTKinhPhiDuocCap_NamNay_VND = getListNone.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND),
                            fQTKinhPhiDuocCap_TongSo_USD = getListNone.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD),
                            fQTKinhPhiDuocCap_TongSo_VND = getListNone.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND),
                            fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = getListNone.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD),
                            fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = getListNone.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND)

                        };
                        if (isPrint)
                        {
                            newObjKhac.fDeNghiQTNamNay_USD = getListNone.Sum(x => x.fDeNghiQTNamNay_USD);
                            newObjKhac.fDeNghiQTNamNay_VND = getListNone.Sum(x => x.fDeNghiQTNamNay_VND);
                            newObjKhac.fDeNghiChuyenNamSau_USD = getListNone.Sum(x => x.fDeNghiChuyenNamSau_USD);
                            newObjKhac.fDeNghiChuyenNamSau_VND = getListNone.Sum(x => x.fDeNghiChuyenNamSau_VND);
                            newObjKhac.fThuaNopNSNN_USD = getListNone.Sum(x => x.fThuaNopNSNN_USD);
                            newObjKhac.fThuaNopNSNN_VND = getListNone.Sum(x => x.fThuaNopNSNN_VND);
                            newObjKhac.fThuaThieuKinhPhiTrongNam_USD = getListNone.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD);
                            newObjKhac.fThuaThieuKinhPhiTrongNam_VND = getListNone.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND);
                        }
                        listResult.Add(newObjKhac);
                        listResult.AddRange(returnLoaiChi(isPrint, chuongTrinh.iID_KHCTBQP_NhiemVuChiID, null, iCurrentID, false, false, getListNone, listTitle));
                    }


                    var obj = listResult.FirstOrDefault(x => x.sLevel == "0" && x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID);
                    if (obj != null)
                    {
                        var getThisList = listResult.Where(x => x.iSum == 1 && x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).ToList();
                        if (isPrint)
                        {
                            obj.fDeNghiQTNamNay_USD = getThisList.Sum(x => x.fDeNghiQTNamNay_USD);
                            obj.fDeNghiQTNamNay_VND = getThisList.Sum(x => x.fDeNghiQTNamNay_VND);
                            obj.fDeNghiChuyenNamSau_USD = getThisList.Sum(x => x.fDeNghiChuyenNamSau_USD);
                            obj.fDeNghiChuyenNamSau_VND = getThisList.Sum(x => x.fDeNghiChuyenNamSau_VND);
                            obj.fThuaNopNSNN_USD = getThisList.Sum(x => x.fThuaNopNSNN_USD);
                            obj.fThuaNopNSNN_VND = getThisList.Sum(x => x.fThuaNopNSNN_VND);
                            obj.fThuaThieuKinhPhiTrongNam_USD = getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_USD);
                            obj.fThuaThieuKinhPhiTrongNam_VND = getThisList.Sum(x => x.fThuaThieuKinhPhiTrongNam_VND);
                        };

                        listResult.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).ToList().ForEach(x =>
                        {

                            if (maTiGia.ToUpper() == "USD")
                            {
                                x.fQTKinhPhiDuocCap_NamNay_USD = x.fQTKinhPhiDuocCap_NamNay_USD == null || x.fQTKinhPhiDuocCap_NamNay_USD == 0 ? x.fQTKinhPhiDuocCap_NamNay_VND / tiGia : x.fQTKinhPhiDuocCap_NamNay_USD;
                                x.fQTKinhPhiDuocCap_NamNay_VND = x.fQTKinhPhiDuocCap_NamNay_VND == null || x.fQTKinhPhiDuocCap_NamNay_VND == 0 ? x.fQTKinhPhiDuocCap_NamNay_USD * tiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_TongSo_USD = x.fQTKinhPhiDuocCap_TongSo_USD == null || x.fQTKinhPhiDuocCap_TongSo_USD == 0 ? x.fQTKinhPhiDuocCap_TongSo_VND / tiGia : x.fQTKinhPhiDuocCap_TongSo_USD;
                                x.fQTKinhPhiDuocCap_TongSo_VND = x.fQTKinhPhiDuocCap_TongSo_VND == null || x.fQTKinhPhiDuocCap_TongSo_VND == 0 ? x.fQTKinhPhiDuocCap_TongSo_USD * tiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND / tiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD * tiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND;
                            }
                            else
                            {
                                x.fQTKinhPhiDuocCap_NamNay_USD = x.fQTKinhPhiDuocCap_NamNay_USD == null || x.fQTKinhPhiDuocCap_NamNay_USD == 0 ? x.fQTKinhPhiDuocCap_NamNay_VND * tiGia : x.fQTKinhPhiDuocCap_NamNay_USD;
                                x.fQTKinhPhiDuocCap_NamNay_VND = x.fQTKinhPhiDuocCap_NamNay_VND == null || x.fQTKinhPhiDuocCap_NamNay_VND == 0 ? x.fQTKinhPhiDuocCap_NamNay_USD / tiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_TongSo_USD = x.fQTKinhPhiDuocCap_TongSo_USD == null || x.fQTKinhPhiDuocCap_TongSo_USD == 0 ? x.fQTKinhPhiDuocCap_TongSo_VND * tiGia : x.fQTKinhPhiDuocCap_TongSo_USD;
                                x.fQTKinhPhiDuocCap_TongSo_VND = x.fQTKinhPhiDuocCap_TongSo_VND == null || x.fQTKinhPhiDuocCap_TongSo_VND == 0 ? x.fQTKinhPhiDuocCap_TongSo_USD / tiGia : x.fQTKinhPhiDuocCap_NamNay_VND;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND * tiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD;
                                x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == null || x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND == 0 ? x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD / tiGia : x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND;
                            }
                        });

                        obj.fQTKinhPhiDuocCap_NamNay_USD = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD);
                        obj.fQTKinhPhiDuocCap_NamNay_VND = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND);
                        obj.fQTKinhPhiDuocCap_TongSo_USD = getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD);
                        obj.fQTKinhPhiDuocCap_TongSo_VND = getThisList.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND);
                        obj.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD);
                        obj.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = getThisList.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND);
                        obj.fKeHoachChuaGiaiNgan_USD = obj.fKeHoach_BQP_USD - obj.fQTKinhPhiDuocCap_TongSo_USD;

                    }
                }
            }
            return listResult;
        }
        [HttpPost]
        public JsonResult SaveDetail(List<NH_QT_QuyetToanNienDo_ChiTiet> data, int isAddOrUpdate, Guid idQuyetToan)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            var returnData = _qlnhService.SaveQuyetToanNienDoDetail(data, Username, isAddOrUpdate != 0, idQuyetToan);

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                _qlnhService.InsertNHTongHop_Tang(conn, trans, "QUYET_TOAN", isAddOrUpdate != 0 ? 2 : 1, idQuyetToan, null);
                _qlnhService.InsertNHTongHop_Giam(conn, trans, "QUYET_TOAN", isAddOrUpdate != 0 ? 2 : 1, idQuyetToan, null);
                _qlnhService.InsertNHTongHop_Giam(conn, trans, "QTND", isAddOrUpdate != 0 ? 2 : 1, idQuyetToan, null);
                trans.Commit();
            }


            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        public List<NH_QT_QuyetToanNienDo_ChiTietData> returnLoaiChi(bool isPrint, Guid? idChuongTrinh, Guid? idDuAn, int iCurrentID, bool isDuAn, bool haveDuAn, List<NH_QT_QuyetToanNienDo_ChiTietData> list, List<NH_QT_QuyetToanNienDo_ChiTietData> listTittle)
        {
            List<NH_QT_QuyetToanNienDo_ChiTietData> returnData = new List<NH_QT_QuyetToanNienDo_ChiTietData>();
            var listLoaiChiPhi = list.Select(x => new { x.iLoaiNoiDungChi }).Distinct().OrderBy(x => x.iLoaiNoiDungChi)
                  .ToList();
            var countLoaiChiPhi = 0;
            var currentParent = iCurrentID;
            foreach (var loaiChiPhi in listLoaiChiPhi)
            {
                countLoaiChiPhi++;
                var newObjLoaiChiPhi = new NH_QT_QuyetToanNienDo_ChiTietData()
                {
                    STT = countLoaiChiPhi.ToString(),
                    sTenNoiDungChi = loaiChiPhi.iLoaiNoiDungChi == 1 ? "Chi ngoại tệ" : "Chi trong nước",
                    bIsTittle = true
                };
                returnData.Add(newObjLoaiChiPhi);

                if (isDuAn)
                {
                    var listNameHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Select(x => new
                    {
                        x.sTenHopDong,
                        x.iID_HopDongID,
                        x.fHopDong_VND_HopDong,
                        x.fHopDong_USD_HopDong
                       
                    }
                    ).Distinct()
                    .ToList();
                    var countHopDong = 0;
                    foreach (var nameHopDong in listNameHopDong)
                    {
                        countHopDong++;
                        iCurrentID++;
                        var newObjHopDongDuAn = new NH_QT_QuyetToanNienDo_ChiTietData();
                        var findTittle = listTittle.Find(x => x.iID_HopDongID == nameHopDong.iID_HopDongID && x.iID_KHCTBQP_NhiemVuChiID == idChuongTrinh && x.iID_DuAnID == idDuAn && x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi);
                        if (findTittle != null)
                        {
                            newObjHopDongDuAn.MapFrom(findTittle);
                            newObjHopDongDuAn.sK = "";
                            newObjHopDongDuAn.sL = "";
                            newObjHopDongDuAn.sLNS = "";
                            newObjHopDongDuAn.sM = "";
                            newObjHopDongDuAn.sTM = "";
                            newObjHopDongDuAn.sTTM = "";

                        }
                        newObjHopDongDuAn.STT = countLoaiChiPhi.ToString() + "." + countHopDong.ToString();
                        newObjHopDongDuAn.sTenNoiDungChi = nameHopDong.iID_HopDongID != null ? nameHopDong.sTenHopDong : "Không thuộc hợp đồng";
                        newObjHopDongDuAn.fHopDong_VND = nameHopDong.fHopDong_VND_HopDong;
                        newObjHopDongDuAn.fHopDong_USD = nameHopDong.fHopDong_USD_HopDong;
                        newObjHopDongDuAn.bIsSaveTongHop = haveDuAn;

                        newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_USD = list.Sum(x => x.fQTKinhPhiDuocCap_TongSo_USD);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_TongSo_VND = list.Sum(x => x.fQTKinhPhiDuocCap_TongSo_VND);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_USD = list.Sum(x => x.fQTKinhPhiDuocCap_NamNay_USD);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamNay_VND = list.Sum(x => x.fQTKinhPhiDuocCap_NamNay_VND);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD = list.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_USD);
                        newObjHopDongDuAn.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND = list.Sum(x => x.fQTKinhPhiDuocCap_NamTruocChuyenSang_VND);
                       
                        newObjHopDongDuAn.bIsSaveTongHop = isDuAn;
                        newObjHopDongDuAn.bIsData = nameHopDong.iID_HopDongID != null;
                        newObjHopDongDuAn.bIsTittle = true;
                        newObjHopDongDuAn.sLevel = "2";
                        newObjHopDongDuAn.iCurrentId = iCurrentID;
                        if (isPrint)
                        {
                            newObjHopDongDuAn.fKeHoach_TTCP_USD = null;
                            newObjHopDongDuAn.fKeHoach_BQP_USD = null;
                        }
                        newObjHopDongDuAn.iParentId = currentParent;
                        newObjHopDongDuAn.iID_ParentID = nameHopDong.iID_HopDongID;
                        newObjHopDongDuAn.iID_KHCTBQP_NhiemVuChiID = idChuongTrinh;
                        newObjHopDongDuAn.iID_HopDongID = nameHopDong.iID_HopDongID;
                        newObjHopDongDuAn.iID_DuAnID = idDuAn;

                        var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi && x.iID_HopDongID == nameHopDong.iID_HopDongID).ToList();
                        newObjHopDongDuAn.iID_ThanhToan_ChiTietID = listHopDong.FirstOrDefault().iID_ThanhToan_ChiTietID;

                        returnData.Add(newObjHopDongDuAn);

                        listHopDong.ForEach(x =>
                        {
                            iCurrentID++;
                            x.bIsData = true;
                            x.sLevel = "3";
                            x.iCurrentId = iCurrentID;
                            x.iParentId = newObjHopDongDuAn.iCurrentId;
                            if (isPrint)
                            {
                                x.fKeHoachChuaGiaiNgan_USD = null;
                                x.fKeHoach_TTCP_USD = null;
                                x.fKeHoach_BQP_USD = null;
                            }

                            x.fQTKinhPhiDuyetCacNamTruoc_USD = null;
                            x.fQTKinhPhiDuyetCacNamTruoc_VND = null;
                            x.fLuyKeKinhPhiDuocCap_USD = null;
                            x.fLuyKeKinhPhiDuocCap_VND = null;
                        });
                        returnData.AddRange(listHopDong);
                    }
                }
                else
                {

                    var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).ToList();
                    listHopDong.ForEach(x =>
                    {
                        x.iParentId = currentParent;
                        iCurrentID++;
                        x.bIsData = true;
                        x.sLevel = "3";
                        x.iCurrentId = iCurrentID;
                        x.bIsSaveTongHop = true;
                        if (isPrint)
                        {
                            x.fKeHoachChuaGiaiNgan_USD = null;
                            x.fKeHoach_TTCP_USD = null;
                            x.fKeHoach_BQP_USD = null;
                        }
                    });
                    returnData.AddRange(listHopDong);
                }

            }
            return returnData;
        }
        public ActionResult ExportFile(string txtTieuDe1, string txtTieuDe2, string txtIDQuyetToan, int? slbDonViUSD, int? slbDonViVND, string ext = "xlsx", int to = 1)
        {
            var a = getQuyetToanDonVi(txtIDQuyetToan);
            string fileName = string.Format("{0}.{1}", "Quyet toan nien do nam" + a[0].quyetToanNienDo.iNamKeHoach, ext);
            ExcelFile xls = TaoFileExel(txtTieuDe1, txtTieuDe2, a, slbDonViUSD, slbDonViVND, to);
            return Print(xls, ext, fileName);
        }
        public ExcelFile TaoFileExel(string txtTieuDe1, string txtTieuDe2, List<NH_QT_QuyetToanNienDoByDonVi> quyetToanNienDoDetail, int? slbDonViUSD, int? slbDonViVND, int to = 1)
        {
            var donViVND = lstDonViVND.Find(x => x.Value == slbDonViVND);
            var donViUSD = lstDonViUSD.Find(x => x.Value == slbDonViUSD);
            List<NH_QT_QuyetToanNienDo_ChiTietData> data = new List<NH_QT_QuyetToanNienDo_ChiTietData>();
            data = getListDetailChiTiet(quyetToanNienDoDetail, true, donViUSD.Value, donViVND.Value);
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao));
            FlexCelReport fr = new FlexCelReport();

            fr.SetValue(new
            {
                To = to,
                txtTieuDe1 = txtTieuDe1,
                txtTieuDe2 = txtTieuDe2,
                donViUSD = donViUSD.Label,
                donViVND = donViVND.Label,

            });
            //fr.SetValue("iTongSoNgayDieuTri", iTongSoNgayDieuTri.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")));
            foreach (var item in data)
            {
                item.sTenNoiDungChi = (item.STT != null ? item.STT + "," : string.Empty) + item.sTenNoiDungChi;
            }
            fr.AddTable<NH_QT_QuyetToanNienDo_ChiTietData>("dt", data);

            fr.UseChuKy(Username)
              .UseChuKyForController(sControlName)
              .UseForm(this).Run(Result);

            return Result;
        }

        private string convertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }
        private string convertLaMa(decimal num)
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

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamKeHoach, int tabIndex)
        {
            sSoDeNghi = HttpUtility.HtmlDecode(sSoDeNghi);
            QuyetToan_QuyetToanNienDoModel vm = new QuyetToan_QuyetToanNienDoModel();
            vm._paging = _paging;
            vm.Items = _qlnhService.GetListQuyetToanNienDo(ref vm._paging, sSoDeNghi
                , (dNgayDeNghi == null ? null : dNgayDeNghi), (iDonVi == Guid.Empty ? null : iDonVi)
                , (iNamKeHoach == null ? null : iNamKeHoach), tabIndex);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<Dropdown_SelectValue> lstNamKeHoach = GetListNamKeHoach();
            lstNamKeHoach.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKeHoach = lstNamKeHoach;

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            NH_QT_QuyetToanNienDo data = new NH_QT_QuyetToanNienDo();

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<NH_DM_TiGia> lstTiGia = _qlnhService.GetTiGiaQuyetToan().ToList();
            lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỉ giá--" });
            ViewBag.ListTiGia = lstTiGia;

            List<Dropdown_SelectValue> lstNamKeHoach = GetListNamKeHoach();
            lstNamKeHoach.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamKeHoach = lstNamKeHoach;

            List<Dropdown_SelectValue> loaiQuyetToan = lstLoaiQuyetToan;
            loaiQuyetToan.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn loại quyết toán--" });
            ViewBag.ListLoaiQuyetToan = loaiQuyetToan;

            ViewBag.IsTongHop = false;
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinQuyetToanNienDoById(id.Value);
                ViewBag.IsTongHop = data.sTongHop != null ? true : false;

            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult GetModalInBaoCao(string[] listId)
        {
            lstDonViVND.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn đơn vị VND--" });
            ViewBag.ListDVVND = lstDonViVND;

            lstDonViUSD.Insert(0, new Dropdown_SelectValue { Value = 0, Label = "--Chọn đơn vị USD--" });
            ViewBag.ListDVUSD = lstDonViUSD;

            ViewBag.IDQuyetToan = listId[0];

            return PartialView("_modalInBaoCao");
        }
        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            NH_QT_QuyetToanNienDoData data = new NH_QT_QuyetToanNienDoData();
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinQuyetToanNienDoById(id.Value);
                var donvi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), data.iID_DonViID.ToString());
                data.sTenDonVi = donvi != null ? donvi.iID_MaDonVi + "-" + donvi.sTen : string.Empty;
                var tiGia = _qlnhService.GetTiGiaQuyetToan(data.iID_TiGiaID).FirstOrDefault();
                data.sTenTiGia = tiGia != null ? tiGia.sTenTiGia : string.Empty;

                data.sMaTienTeGoc = tiGia.sMaTienTeGoc;
            }

            return PartialView("_modalDetail", data);
        }
        [HttpPost]
        public JsonResult Save(NH_QT_QuyetToanNienDo data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sSoDeNghi = HttpUtility.HtmlDecode(data.sSoDeNghi);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);

            var returnData = _qlnhService.SaveQuyetToanNienDo(data, Username);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = returnData.errorMess }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true, dataID = returnData.QuyetToanNienDoData.ID }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveTongHop(NH_QT_QuyetToanNienDo data, string listId)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sSoDeNghi = HttpUtility.HtmlDecode(data.sSoDeNghi);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);

            if (!_qlnhService.SaveTongHop(data, Username, listId))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Xoa(string id)
        {
            if (!_qlnhService.DeleteQuyetToanNienDo(Guid.Parse(id)))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool LockQuyetToan(Guid id)
        {
            try
            {
                NH_QT_QuyetToanNienDo entity = _qlnhService.GetThongTinQuyetToanNienDoById(id);
                if (entity != null)
                {
                    bool isLockOrUnlock = entity.bIsKhoa;
                    return _qlnhService.LockOrUnLockQuyetToanNienDo(id, !isLockOrUnlock);
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
        public List<Dropdown_SelectValue> GetListNamKeHoach()
        {
            List<Dropdown_SelectValue> listNam = new List<Dropdown_SelectValue>();
            int namHienTai = DateTime.Now.Year + 1;
            for (int i = 20; i > 0; i--)
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

        [HttpPost]
        public ActionResult GetModalTongHop(string[] listId, PagingInfo _paging, int namKeHoach)
        {
            QuyetToan_QuyetToanNienDoModel vm = new QuyetToan_QuyetToanNienDoModel();
            vm._paging = _paging;
            List<NH_QT_QuyetToanNienDoData> list = new List<NH_QT_QuyetToanNienDoData>();
            var stringId = "";
            foreach (var id in listId)
            {
                NH_QT_QuyetToanNienDoData data = _qlnhService.GetThongTinQuyetToanNienDoById(new Guid(id));
                List<NS_DonVi> lstDonViQL = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).ToList();

                if (data.iID_DonViID != null)
                {
                    var donVi = lstDonViQL.Find(x => x.iID_Ma == data.iID_DonViID);
                    data.sTenDonVi += donVi.iID_MaDonVi + " - " + donVi.sTen;
                }
                stringId += id + ",";
                list.Add(data);
            }
            vm.Items = list;
            ViewBag.ListNamKeHoach = namKeHoach;
            ViewBag.ListId = stringId.Substring(0, stringId.Length - 1);
            return PartialView("_modelTongHop", vm);
        }
    }
}
