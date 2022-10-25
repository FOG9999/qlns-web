using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH.QuyetToan;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using static Viettel.Services.IQLNHService;

namespace VIETTEL.Areas.QLNH.Controllers.QuyetToan
{
    public class QuyetToanDuAnHoanThanhController : FlexcelReportController
    {
        // GET: QLNH/QuyetToanDuAnHoanThanh
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        private const string sFilePathBaoCao = "/Report_ExcelFrom/QLNH/rpt_QuyetToanDuAnHoanThanh.xlsx";
        private const string sControlName = "QuyetToanDuAnHoanThanh";
        public List<Dropdown_QuyetToanDAHT> lstDonViVND = new List<Dropdown_QuyetToanDAHT>()
            {
                new Dropdown_QuyetToanDAHT()
                {
                    Value = 1,
                    Label = "Đồng"
                },
                 new Dropdown_QuyetToanDAHT()
                {
                    Value = 1000,
                    Label = "Nghìn đồng"
                }, new Dropdown_QuyetToanDAHT()
                {
                    Value = 1000000000,
                    Label = "Tỉ đồng"
                }
            };
        public List<Dropdown_QuyetToanDAHT> lstDonViUSD = new List<Dropdown_QuyetToanDAHT>()
            {
                new Dropdown_QuyetToanDAHT()
                {
                    Value = 1,
                    Label = "USD"
                },
                 new Dropdown_QuyetToanDAHT()
                {
                    Value = 1000,
                    Label = "Nghìn USD"
                }, new Dropdown_QuyetToanDAHT()
                {
                    Value = 1000000000,
                    Label = "Tỉ USD"
                }
            };
        public ActionResult Index()
        {
            QuyetToan_QuyetToanDuAnModel vm = new QuyetToan_QuyetToanDuAnModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListQuyetToanDuAnHT(ref vm._paging, null, null, null, null, null, 0);
            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<Dropdown_QuyetToanDAHT> lstNamBaoCaoTu = GetListNamKeHoach();
            lstNamBaoCaoTu.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamBaoCaoTu = lstNamBaoCaoTu;

            List<Dropdown_QuyetToanDAHT> lstNamBaoCaoDen = GetListNamKeHoach();
            lstNamBaoCaoDen.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamBaoCaoDen = lstNamBaoCaoDen;
            return View(vm);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamBaoCaoTu, int? iNamBaoCaoDen, int tabIndex)
        {
            sSoDeNghi = HttpUtility.HtmlDecode(sSoDeNghi);
            QuyetToan_QuyetToanDuAnModel vm = new QuyetToan_QuyetToanDuAnModel();
            vm._paging = _paging;
            vm.Items = _qlnhService.GetListQuyetToanDuAnHT(ref vm._paging, sSoDeNghi
                , (dNgayDeNghi == null ? null : dNgayDeNghi), (iDonVi == Guid.Empty ? null : iDonVi)
                , (iNamBaoCaoTu == null ? null : iNamBaoCaoTu), (iNamBaoCaoDen == null ? null : iNamBaoCaoDen), tabIndex);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<Dropdown_QuyetToanDAHT> lstNamBaoCaoTu = GetListNamKeHoach();
            lstNamBaoCaoTu.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamBaoCaoTu = lstNamBaoCaoTu;

            List<Dropdown_QuyetToanDAHT> lstNamBaoCaoDen = GetListNamKeHoach();
            lstNamBaoCaoDen.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamBaoCaoDen = lstNamBaoCaoDen;

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            NH_QT_QuyetToanDAHT data = new NH_QT_QuyetToanDAHT();

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            List<NH_DM_TiGia> lstTiGia = _qlnhService.GetTiGiaQuyetToan().ToList();
            lstTiGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn đơn vị--" });
            ViewBag.ListTiGia = lstTiGia;

            List<Dropdown_QuyetToanDAHT> lstNamBaoCaoTu = GetListNamKeHoach();
            lstNamBaoCaoTu.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamBaoCaoTu = lstNamBaoCaoTu;

            List<Dropdown_QuyetToanDAHT> lstNamBaoCaoDen = GetListNamKeHoach();
            lstNamBaoCaoDen.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn năm--" });
            ViewBag.ListNamBaoCaoDen = lstNamBaoCaoDen;

            List<Dropdown_QuyetToanDAHT> lstTrangThai = new List<Dropdown_QuyetToanDAHT>();
            lstTrangThai.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn trạng thái--" });
            lstTrangThai.Insert(1, new Dropdown_QuyetToanDAHT { Value = 1, Label = "Chưa được duyệt" });
            lstTrangThai.Insert(2, new Dropdown_QuyetToanDAHT { Value = 2, Label = "Đã được duyệt" });
            ViewBag.ListTrangThai = lstTrangThai;

            ViewBag.IsTongHop = false;
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinQuyetToanDuAnHTById(id.Value);
                ViewBag.IsTongHop = data.sTongHop != null ? true : false;
            }
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public JsonResult Save(NH_QT_QuyetToanDAHT data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sSoDeNghi = HttpUtility.HtmlDecode(data.sSoDeNghi);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);

            var returnData = _qlnhService.SaveQuyetToanDuAnHT(data, Username);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true, dataID = returnData.QuyetToanDuAnHTData.ID }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Xoa(string id)
        {
            if (!_qlnhService.DeleteQuyetToanDuAnHT(Guid.Parse(id)))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public bool LockDuAn(Guid id)
        {
            try
            {
                NH_QT_QuyetToanDAHT entity = _qlnhService.GetThongTinQuyetToanDuAnHTById(id);
                if (entity != null)
                {
                    return _qlnhService.LockOrUnLockQuyetToanDuAnHT(id, !entity.bIsKhoa);
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
        [HttpPost]
        public ActionResult GetModalTongHop(string[] listId, PagingInfo _paging, string[] listNamBaoCao)
        {
            QuyetToan_QuyetToanDuAnModel vm = new QuyetToan_QuyetToanDuAnModel();
            vm._paging = _paging;
            List<NH_QT_QuyetToanDAHTData> list = new List<NH_QT_QuyetToanDAHTData>();
            var stringId = "";
            foreach (var id in listId)
            {
                NH_QT_QuyetToanDAHTData data = _qlnhService.GetThongTinQuyetToanDuAnHTById(new Guid(id));
                List<NS_DonVi> lstDonViQL = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).ToList();
                if (data.iID_DonViID != null)
                {
                    var donVi = lstDonViQL.Find(x => x.iID_Ma == data.iID_DonViID);
                    data.sTenDonVi += donVi.iID_MaDonVi + "-" + donVi.sTen;
                }
                stringId += id + ",";
                list.Add(data);
            }
            List<Dropdown_QuyetToanDAHT> lstTrangThai = new List<Dropdown_QuyetToanDAHT>();
            lstTrangThai.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn trạng thái--" });
            lstTrangThai.Insert(1, new Dropdown_QuyetToanDAHT { Value = 1, Label = "Chưa được duyệt" });
            lstTrangThai.Insert(2, new Dropdown_QuyetToanDAHT { Value = 2, Label = "Đã được duyệt" });
            ViewBag.ListTrangThai = lstTrangThai;
            vm.Items = list;
            ViewBag.ListNamBaoCaoTu = listNamBaoCao.OrderBy(x => x).First();
            ViewBag.ListNamBaoCaoDen = listNamBaoCao.OrderBy(x => x).Last();

            ViewBag.ListId = stringId.Substring(0, stringId.Length - 1);
            return PartialView("_modelTongHop", vm);
        }
        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            NH_QT_QuyetToanDAHTData data = new NH_QT_QuyetToanDAHTData();
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinQuyetToanDuAnHTById(id.Value);
                if (data.iID_DonViID == null)
                {
                    foreach (var idTongHop in data.sTongHop.Split(','))
                    {
                        var tongHop = _qlnhService.GetThongTinQuyetToanDuAnHTById(new Guid(idTongHop));
                        var donvi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), tongHop.iID_DonViID.ToString());
                        if (donvi != null)
                        {
                            data.sTenDonVi += (donvi != null ? donvi.iID_MaDonVi + " - " + donvi.sTen : string.Empty) + " , ";
                        }
                    }
                }
                else
                {
                    var donvi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), data.iID_DonViID.ToString());
                    if (donvi != null)
                    {
                        data.sTenDonVi = (donvi != null ? donvi.iID_MaDonVi + " - " + donvi.sTen : string.Empty) + " , ";
                    }
                }
                data.sTenDonVi = data.sTenDonVi.Remove(data.sTenDonVi.Length - 2);
                var tiGia = _qlnhService.GetNHDMTiGiaList(data.iID_TiGiaID).FirstOrDefault();
                data.sTenTiGia = tiGia != null ? tiGia.sTenTiGia : string.Empty;
            }



            return PartialView("_modalDetail", data);
        }

        public ActionResult ExportFile(string txtTieuDe1, string txtTieuDe2, string txtIDQuyetToan, int? slbDonViUSD, int? slbDonViVND, string ext = "xlsx", int to = 1)
        {
            var a = getQuyetToanDonVi(txtIDQuyetToan);
            string fileName = string.Format("{0}.{1}", "Quyet toan du an hoan thanh giai doan " + a[0].quyetToanDuAn.iNamBaoCaoTu + " - " + a[0].quyetToanDuAn.iNamBaoCaoDen, ext);
            ExcelFile xls = TaoFileExel(txtTieuDe1, txtTieuDe2, a, slbDonViUSD, slbDonViVND, to);
            return Print(xls, ext, fileName);
        }
        public ExcelFile TaoFileExel(string txtTieuDe1, string txtTieuDe2, List<NH_QT_QuyetToanDuAnByDonVi> quyetToanNienDoDetail, int? slbDonViUSD, int? slbDonViVND, int to = 1)
        {
            var donViVND = lstDonViVND.Find(x => x.Value == slbDonViVND);
            var donViUSD = lstDonViUSD.Find(x => x.Value == slbDonViUSD);
            List<NH_QT_QuyetToanDAHT_ChiTietData> data = new List<NH_QT_QuyetToanDAHT_ChiTietData>();
            data = getListDetailChiTiet(quyetToanNienDoDetail, true, donViUSD.Value, donViVND.Value);
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao));
            FlexCelReport fr = new FlexCelReport();
            var giaiDoans = data.Select(x => new
            {
                x.iNamBaoCaoTu,
                x.iNamBaoCaoDen
            }).Where(x => x.iNamBaoCaoTu != null && x.iNamBaoCaoDen != null).OrderBy(x => x.iNamBaoCaoTu).Distinct().ToList();
            foreach (var item in data)
            {
                item.listDataTTCP = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                item.listDataKPDC = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();
                item.listDataQTDD = new List<NH_QT_QuyetToanDuAnDataGiaiDoan>();


                foreach (var giaiDoan in giaiDoans)
                {
                    if (item.iNamBaoCaoTu == giaiDoan.iNamBaoCaoTu && item.iNamBaoCaoDen == giaiDoan.iNamBaoCaoDen)
                    {
                        item.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { value = item.fKeHoach_TTCP_USD });
                        item.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = item.fKinhPhiDuocCap_Tong_USD, valueVND = item.fKinhPhiDuocCap_Tong_VND });
                        item.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan() { valueUSD = item.fQuyetToanDuocDuyet_Tong_USD, valueVND = item.fQuyetToanDuocDuyet_Tong_VND });

                    }
                    else
                    {
                        item.listDataTTCP.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan());
                        item.listDataKPDC.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan());
                        item.listDataQTDD.Add(new NH_QT_QuyetToanDuAnDataGiaiDoan());
                    }
                }
            }
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
                item.sTenNoiDungChi = (item.STT != null ? item.STT + ", " : " - ") + item.sTenNoiDungChi;
            }
            List<NH_QT_QuyetToanDuAnGiaiDoan> lstGiaiDoan = new List<NH_QT_QuyetToanDuAnGiaiDoan>();
            foreach (var giaiDoan in giaiDoans)
            {
                lstGiaiDoan.Add(new NH_QT_QuyetToanDuAnGiaiDoan()
                {
                    giaiDoan = "Giai đoạn " + giaiDoan.iNamBaoCaoTu.ToString() + " - " + giaiDoan.iNamBaoCaoDen.ToString()
                });

            }

            fr.AddTable<NH_QT_QuyetToanDAHT_ChiTietData>("dt", data);
            fr.AddTable<NH_QT_QuyetToanDuAnGiaiDoan>("lstGiaiDoan", lstGiaiDoan);
            fr.AddTable<NH_QT_QuyetToanDuAnGiaiDoan>("lstGiaiDoan2", lstGiaiDoan);
            fr.AddTable<NH_QT_QuyetToanDuAnGiaiDoan>("lstGiaiDoan3", lstGiaiDoan);
            fr.UseChuKy(Username)
              .UseChuKyForController(sControlName)
              .UseForm(this).Run(Result);

            //count merge cột
            var col1 = 5 + lstGiaiDoan.Count();
            var col2 = col1 + ((lstGiaiDoan.Count() + 1) * 2);
            Result.MergeCells(9, 5, 9, col1);
            Result.MergeCells(9, col1 + 1, 9, col1 + ((lstGiaiDoan.Count() + 1) * 2));
            Result.MergeCells(9, col2 + 1, 9, col2 + ((lstGiaiDoan.Count() + 1) * 2));
            //tạo border format
            var bBorder = Result.GetDefaultFormat;
            bBorder.Borders.Left.Style = TFlxBorderStyle.Thin;
            bBorder.Borders.Right.Style = TFlxBorderStyle.Thin;
            bBorder.Borders.Top.Style = TFlxBorderStyle.Thin;
            bBorder.Borders.Bottom.Style = TFlxBorderStyle.Thin;
            var fBold = Result.GetDefaultFormat;
            fBold.Font.Style = TFlxFontStyles.Bold;
            fBold.Font.Name = "Times New Roman";
            fBold.Font.Size20 = 280;

            var fItalic = Result.GetDefaultFormat;
            fItalic.Font.Style = TFlxFontStyles.Italic;
            fItalic.Font.Name = "Times New Roman";
            fItalic.Font.Size20 = 280;

            var ApplyFormat = new TFlxApplyFormat();
            ApplyFormat.SetAllMembers(false);
            ApplyFormat.Borders.SetAllMembers(true);
            var ApplyFormatBold = new TFlxApplyFormat();
            ApplyFormatBold.SetAllMembers(false);
            ApplyFormatBold.Font.SetAllMembers(true);
            TCellAddress Cell = null;
            //tìm dòng cuối cùng của bảng
            Cell = Result.Find("Cộng", null, Cell, false, true, true, false);
            //set border cho bảng
            Result.SetCellFormat(9, 2, Cell.Row, 13 + lstGiaiDoan.Count() * 5, bBorder, ApplyFormat, false);
            Result.SetCellValue(Cell.Row + 5, 10 + lstGiaiDoan.Count() * 5, "Ngày … Tháng … Năm");

            Result.SetCellValue(Cell.Row + 6, 10 + lstGiaiDoan.Count() * 5, "THỦ TRƯỞNG ĐƠN VỊ");
            Result.SetCellFormat(Cell.Row + 6, 10 + lstGiaiDoan.Count() * 5, Cell.Row + 6, 10 + lstGiaiDoan.Count() * 5, fBold, ApplyFormatBold, false);
            
            Result.SetCellValue(3, 10 + lstGiaiDoan.Count() * 5, "Mẫu số 05/QT-QNH");
            Result.SetCellFormat(3, 10 + lstGiaiDoan.Count() * 5, 2, 10 + lstGiaiDoan.Count() * 5, fBold, ApplyFormatBold, false);

            Result.SetCellValue(8, 10 + lstGiaiDoan.Count() * 5, "Đơn vị tính: "+ donViUSD.Label + " / "+ donViVND.Label);
            Result.SetCellFormat(8, 10 + lstGiaiDoan.Count() * 5, 8, 10 + lstGiaiDoan.Count() * 5, fItalic, ApplyFormatBold, false);

            Result.SetCellValue(3, (13 + lstGiaiDoan.Count() * 5)/2, txtTieuDe1);
            Result.SetCellFormat(3, (13 + lstGiaiDoan.Count() * 5) / 2, 3, (13 + lstGiaiDoan.Count() * 5) / 2, fBold, ApplyFormatBold, false);

            Result.SetCellValue(4, (13 + lstGiaiDoan.Count() * 5) / 2, txtTieuDe2);
            Result.SetCellFormat(4, (13 + lstGiaiDoan.Count() * 5) / 2, 4, (13 + lstGiaiDoan.Count() * 5) / 2, fBold, ApplyFormatBold, false);

            //Result.InsertVPageBreak(15 + lstGiaiDoan.Count() * 5);
            //Result.InsertVPageBreak(Cell.Row + 17);

            return Result;
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
        public JsonResult GetListDropDownNamBaoCao()
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
        public ActionResult Detail(string id, bool edit)
        {
            NH_QT_QuyetToanDAHT_ChiTietView vm = new NH_QT_QuyetToanDAHT_ChiTietView();
            var thongTinQuyetToanDuAnHTById = _qlnhService.GetThongTinQuyetToanDuAnHTById(new Guid(id));
            var quyetToanDonVi = getQuyetToanDonVi(id);
            var donVi = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).Where(x => x.iID_Ma == thongTinQuyetToanDuAnHTById.iID_DonViID).FirstOrDefault();
            if (donVi != null)
            {
                thongTinQuyetToanDuAnHTById.sTenDonVi = donVi.iID_MaDonVi + " - " + donVi.sTen;
            }
            vm.QuyetToanDAHTDetail = thongTinQuyetToanDuAnHTById;
            var listResult = getListDetailChiTiet(quyetToanDonVi, false, null, null);
            vm.ListDetailQuyetToanDAHT = listResult;


            List<string> arr = new List<string>()
            {
                "USD",
                "VND"
            };
            var tiGiaChiTiet = _qlnhService.GetNHDMTiGiaChiTiet(vm.QuyetToanDAHTDetail.iID_TiGiaID, false).Where(x => arr.Contains(x.sMaTienTeQuyDoi)).FirstOrDefault();

            vm.QuyetToanDAHTDetail.fDonViTiGia = tiGiaChiTiet != null ? tiGiaChiTiet.fTiGia : 1;

            var tiGia = _qlnhService.GetNHDMTiGiaList(vm.QuyetToanDAHTDetail.iID_TiGiaID).FirstOrDefault();
            if (tiGia != null)
            {
                vm.QuyetToanDAHTDetail.sMaTienTeGoc = tiGia.sMaTienTeGoc;
            }
            ViewBag.IsEdit = edit;
            ViewBag.IdQuyetToan = id;

            return View(vm);
        }
        [HttpPost]
        public ActionResult GetListTongHopQuyetToan(string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamBaoCaoTu, int? iNamBaoCaoDen)
        {
            sSoDeNghi = HttpUtility.HtmlDecode(sSoDeNghi);
            var ListTongHopQuyetToan = _qlnhService.GetListTongHopQuyetToanDAHT(sSoDeNghi, dNgayDeNghi, iDonVi, iNamBaoCaoTu, iNamBaoCaoDen);
            ViewBag.TabIndex = 1;

            return Json(new { data = ListTongHopQuyetToan }, JsonRequestBehavior.AllowGet);
        }
        public List<NH_QT_QuyetToanDuAnByDonVi> getQuyetToanDonVi(string id)
        {
            var quyetToanDuAnDetail = _qlnhService.GetThongTinQuyetToanDuAnHTById(new Guid(id));
            List<NH_QT_QuyetToanDuAnByDonVi> qtdaDonVi = new List<NH_QT_QuyetToanDuAnByDonVi>();
            if (quyetToanDuAnDetail.iID_DonViID != null)
            {
                var donVi = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).Where(x => x.iID_Ma == quyetToanDuAnDetail.iID_DonViID).FirstOrDefault();
                quyetToanDuAnDetail.sTenDonVi = donVi.iID_MaDonVi + " - " + donVi.sTen;
                qtdaDonVi.Add(new NH_QT_QuyetToanDuAnByDonVi()
                {
                    donVi = donVi,
                    quyetToanDuAn = quyetToanDuAnDetail
                });
            }
            else
            {
                foreach (var item in quyetToanDuAnDetail.sTongHop.Split(','))
                {
                    var qtdaDetail = _qlnhService.GetThongTinQuyetToanDuAnHTById(new Guid(item));
                    var donVi = _qlnhService.GetDonviList(PhienLamViec.NamLamViec).Where(x => x.iID_Ma == qtdaDetail.iID_DonViID).FirstOrDefault();
                    if (donVi != null)
                    {
                        quyetToanDuAnDetail.sTenDonVi += donVi.iID_MaDonVi + " - " + donVi.sTen + ",";

                    }
                    qtdaDonVi.Add(new NH_QT_QuyetToanDuAnByDonVi()
                    {
                        donVi = donVi,
                        quyetToanDuAn = qtdaDetail
                    });
                }
            }
            return qtdaDonVi;
        }
        public List<NH_QT_QuyetToanDAHT_ChiTietData> getListDetailChiTiet(List<NH_QT_QuyetToanDuAnByDonVi> quyetToanDuAnDetail, bool isPrint, int? donViUSD, int? donViVND)
        {
            var listData = new List<NH_QT_QuyetToanDAHT_ChiTietData>();
            var listResult = new List<NH_QT_QuyetToanDAHT_ChiTietData>();

            foreach (var qtdv in quyetToanDuAnDetail)
            {
                var listQuyetToanDetail = _qlnhService.GetDetailQuyetToanDuAnDetail(qtdv.quyetToanDuAn.iNamBaoCaoTu, qtdv.quyetToanDuAn.iNamBaoCaoDen, qtdv.donVi.iID_Ma, qtdv.quyetToanDuAn.ID, donViUSD, donViVND).ToList();
                if (listQuyetToanDetail.Any())
                {
                    listData = listQuyetToanDetail;
                }
                else
                {
                    listData = _qlnhService.GetDetailQuyetToanDuAnCreate(qtdv.quyetToanDuAn.iNamBaoCaoTu, qtdv.quyetToanDuAn.iNamBaoCaoDen, qtdv.donVi.iID_Ma, donViUSD, donViVND).ToList();
                }

                var giaiDoans = listData.Select(x => new
                {
                    x.iNamBaoCaoTu,
                    x.iNamBaoCaoDen
                }).Distinct().ToList();

                var countColumn = 11 + giaiDoans.Count() + (giaiDoans.Count() * 4);
                var listColumn = new List<string>();
                for (var i = 1; i <= countColumn; i++)
                {
                    var startColumn1 = 3 + giaiDoans.Count() + 1;
                    var startColumn2 = startColumn1 + (giaiDoans.Count() * 2) + 2;
                    var startColumn3 = startColumn2 + (giaiDoans.Count() * 2) + 2;


                    if (i == startColumn1 || i == startColumn1 + 1 || i == startColumn2 || i == startColumn2 + 1)
                    {
                        var nowColumn = i.ToString() + " =";
                        for (var j = 1; j <= giaiDoans.Count(); j++)
                        {
                            nowColumn += (i + (j * 2)).ToString() + " +";
                        }
                        listColumn.Add(nowColumn.Remove(nowColumn.Length - 1));
                    }
                    else if (i == startColumn3)
                    {
                        var nowColumn = i.ToString() + " =" + startColumn1.ToString() + " -" + startColumn2.ToString();
                        listColumn.Add(nowColumn);
                    }
                    else if (i == startColumn3 + 1)
                    {
                        var nowColumn = i.ToString() + " =" + (startColumn1 + 1).ToString() + " -" + (startColumn2 + 1).ToString();
                        listColumn.Add(nowColumn);
                    }
                    else
                    {
                        listColumn.Add(i.ToString());
                    }
                }

                ViewBag.ListColumn = listColumn;
                ViewBag.ListGiaiDoan = giaiDoans.Select(x => new NH_QT_QuyetToanDuAnGiaiDoan
                {
                    giaiDoan = "Giai đoạn " + x.iNamBaoCaoTu.ToString() + " - " + x.iNamBaoCaoDen.ToString(),
                    iNamBaoCaoTu = x.iNamBaoCaoTu ?? 0,
                    iNamBaoCaoDen = x.iNamBaoCaoDen ?? 0
                }).OrderBy(x => x.iNamBaoCaoTu).ToList();
                ViewBag.CountGiaiDoan = giaiDoans.Count();

                if (quyetToanDuAnDetail.Count() > 1)
                {
                    var newObjDonVi = new NH_QT_QuyetToanDAHT_ChiTietData()
                    {
                        sTenNoiDungChi = qtdv.donVi.iID_MaDonVi + '-' + qtdv.donVi.sTen,
                        bIsTittle = true
                    };
                    listResult.Add(newObjDonVi);
                }
                var getAllChuongTrinh = listData.Where(x => x.iID_DonVi == qtdv.donVi.iID_Ma && x.iID_KHCTBQP_NhiemVuChiID != null).Select(x => new { x.sTenNhiemVuChi, x.iID_KHCTBQP_NhiemVuChiID, x.fKeHoach_TTCP_USD, x.iNamBaoCaoTu, x.iNamBaoCaoDen }).Distinct().ToList();
                var iCountChuongTrinh = 0;
                foreach (var chuongTrinh in getAllChuongTrinh)
                {
                    iCountChuongTrinh++;
                    var newObj = new NH_QT_QuyetToanDAHT_ChiTietData()
                    {
                        STT = convertLetter(iCountChuongTrinh),
                        sTenNoiDungChi = chuongTrinh.sTenNhiemVuChi,
                        sumTTCP = chuongTrinh.fKeHoach_TTCP_USD,
                        sumKPDCUSD = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                        sumKPDCVND = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                        sumQTDDUSD = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                        sumQTDDVND = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                        fKinhPhiDuocCap_Tong_USD = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                        fKinhPhiDuocCap_Tong_VND = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                        fQuyetToanDuocDuyet_Tong_USD = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                        fQuyetToanDuocDuyet_Tong_VND = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID).Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                        fKeHoach_TTCP_USD = chuongTrinh.fKeHoach_TTCP_USD,
                        iNamBaoCaoTu = chuongTrinh.iNamBaoCaoTu,
                        iNamBaoCaoDen = chuongTrinh.iNamBaoCaoDen,
                        bIsTittle = true
                    };
                    listResult.Add(newObj);
                    var getListDuAn = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID != null).ToList();
                    var getListHopDong = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == null && x.iID_HopDongID != null).ToList();
                    var getListNone = listData.Where(x => x.iID_KHCTBQP_NhiemVuChiID == chuongTrinh.iID_KHCTBQP_NhiemVuChiID && x.iID_DuAnID == null && x.iID_HopDongID == null).ToList();

                    var iCountDuAn = 0;

                    if (getListDuAn.Any())
                    {
                        var getNameDuAn = getListDuAn.Select(x => new { x.sTenDuAn, x.iID_DuAnID, x.fHopDong_VND_DuAn, x.fHopDong_USD_DuAn })
                        .Distinct()
                        .ToList();
                        foreach (var hopDongDuAn in getNameDuAn)
                        {
                            iCountDuAn++;
                            var newObjHopDongDuAn = new NH_QT_QuyetToanDAHT_ChiTietData()
                            {
                                STT = convertLaMa(Decimal.Parse(iCountDuAn.ToString())),
                                sTenNoiDungChi = hopDongDuAn.sTenDuAn,
                                bIsTittle = true,
                                fHopDong_VND = hopDongDuAn.fHopDong_VND_DuAn,
                                fHopDong_USD = hopDongDuAn.fHopDong_USD_DuAn,
                                sumKPDCUSD = getListDuAn.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                                sumKPDCVND = getListDuAn.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                                sumQTDDUSD = getListDuAn.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                                sumQTDDVND = getListDuAn.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                                fKinhPhiDuocCap_Tong_USD = getListDuAn.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                                fKinhPhiDuocCap_Tong_VND = getListDuAn.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                                fQuyetToanDuocDuyet_Tong_USD = getListDuAn.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                                fQuyetToanDuocDuyet_Tong_VND = getListDuAn.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                                iNamBaoCaoTu = chuongTrinh.iNamBaoCaoTu,
                                iNamBaoCaoDen = chuongTrinh.iNamBaoCaoDen,
                            };
                            listResult.Add(newObjHopDongDuAn);
                            listResult.AddRange(returnLoaiChi(isPrint, true, getListDuAn.Where(x => x.iID_DuAnID == hopDongDuAn.iID_DuAnID).ToList()));
                        }
                    }
                    if (getListHopDong.Any())
                    {
                        iCountDuAn++;
                        var newObjHopDong = new NH_QT_QuyetToanDAHT_ChiTietData()
                        {
                            STT = convertLaMa(Decimal.Parse(iCountDuAn.ToString())),
                            sTenNoiDungChi = "Chi hợp đồng",
                            sumKPDCUSD = getListHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                            sumKPDCVND = getListHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                            sumQTDDUSD = getListHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                            sumQTDDVND = getListHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                            fKinhPhiDuocCap_Tong_USD = getListHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                            fKinhPhiDuocCap_Tong_VND = getListHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                            fQuyetToanDuocDuyet_Tong_USD = getListHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                            fQuyetToanDuocDuyet_Tong_VND = getListHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                            iNamBaoCaoTu = chuongTrinh.iNamBaoCaoTu,
                            iNamBaoCaoDen = chuongTrinh.iNamBaoCaoDen,
                            bIsTittle = true,
                        };
                        listResult.Add(newObjHopDong);
                        listResult.AddRange(returnLoaiChi(isPrint, true, getListHopDong));
                        //
                    }
                    if (getListNone.Any())
                    {
                        iCountDuAn++;
                        var newObjKhac = new NH_QT_QuyetToanDAHT_ChiTietData()
                        {
                            STT = convertLaMa(Decimal.Parse(iCountDuAn.ToString())),
                            sTenNoiDungChi = "Chi khác",
                            bIsTittle = true,
                            sumKPDCUSD = getListNone.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                            sumKPDCVND = getListNone.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                            sumQTDDUSD = getListNone.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                            sumQTDDVND = getListNone.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                            fKinhPhiDuocCap_Tong_USD = getListNone.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                            fKinhPhiDuocCap_Tong_VND = getListNone.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                            fQuyetToanDuocDuyet_Tong_USD = getListNone.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                            fQuyetToanDuocDuyet_Tong_VND = getListNone.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                            iNamBaoCaoTu = chuongTrinh.iNamBaoCaoTu,
                            iNamBaoCaoDen = chuongTrinh.iNamBaoCaoDen,
                        };
                        listResult.Add(newObjKhac);
                        listResult.AddRange(returnLoaiChi(isPrint, false, getListNone));
                    }
                }
            }
            return listResult;
        }
        public List<NH_QT_QuyetToanDAHT_ChiTietData> returnLoaiChi( bool isPrint, bool idDuAn, List<NH_QT_QuyetToanDAHT_ChiTietData> list)
        {
            List<NH_QT_QuyetToanDAHT_ChiTietData> returnData = new List<NH_QT_QuyetToanDAHT_ChiTietData>();
            var listLoaiChiPhi = list.Select(x => new { x.iLoaiNoiDungChi }).Distinct().OrderBy(x => x.iLoaiNoiDungChi)
                  .ToList();
            var countLoaiChiPhi = 0;
            foreach (var loaiChiPhi in listLoaiChiPhi)
            {
                countLoaiChiPhi++;
                var newObjLoaiChiPhi = new NH_QT_QuyetToanDAHT_ChiTietData()
                {
                    STT = countLoaiChiPhi.ToString(),
                    sTenNoiDungChi = loaiChiPhi.iLoaiNoiDungChi == 1 ? "Chi ngoại tệ" : "Chi trong nước",
                    bIsTittle = true
                };
                returnData.Add(newObjLoaiChiPhi);

                if (idDuAn)
                {
                    var listNameHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).Select(x => new { x.sTenHopDong, x.iID_HopDongID, x.fHopDong_VND_HopDong, x.fHopDong_USD_HopDong, x.iNamBaoCaoTu, x.iNamBaoCaoDen }).Distinct()
                    .ToList();
                    var countHopDong = 0;
                    foreach (var nameHopDong in listNameHopDong)
                    {
                        countHopDong++;
                        var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi && x.iID_HopDongID == nameHopDong.iID_HopDongID)
                            .ToList();
                        var newObjHopDongDuAn = new NH_QT_QuyetToanDAHT_ChiTietData()
                        {
                            STT = countLoaiChiPhi.ToString() + "." + countHopDong.ToString(),
                            sTenNoiDungChi = nameHopDong.sTenHopDong,
                            bIsTittle = null,
                            fHopDong_VND = nameHopDong.fHopDong_VND_HopDong,
                            fHopDong_USD = nameHopDong.fHopDong_USD_HopDong,
                            sumKPDCUSD = listHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                            sumKPDCVND = listHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                            sumQTDDUSD = listHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                            sumQTDDVND = listHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                            fKinhPhiDuocCap_Tong_USD = listHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_USD),
                            fKinhPhiDuocCap_Tong_VND = listHopDong.Sum(x => x.fKinhPhiDuocCap_Tong_VND),
                            fQuyetToanDuocDuyet_Tong_USD = listHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_USD),
                            fQuyetToanDuocDuyet_Tong_VND = listHopDong.Sum(x => x.fQuyetToanDuocDuyet_Tong_VND),
                            iNamBaoCaoTu = nameHopDong.iNamBaoCaoTu,
                            iNamBaoCaoDen = nameHopDong.iNamBaoCaoDen,
                        };
                        returnData.Add(newObjHopDongDuAn);
                        listHopDong.ForEach(x =>
                        {
                            var a = listHopDong.GroupBy(z => z.iID_ThanhToan_ChiTietID).Select(y => new
                            {
                                sumKPDCUSD = y.Sum(k => k.fKinhPhiDuocCap_Tong_USD),
                                sumKPDCVND = y.Sum(k => k.fKinhPhiDuocCap_Tong_VND),
                                sumQTDDUSD = y.Sum(k => k.fQuyetToanDuocDuyet_Tong_USD),
                                sumQTDDVND = y.Sum(k => k.fQuyetToanDuocDuyet_Tong_VND),
                                iID_ThanhToan_ChiTietID = y.Key
                            }).Where(l => l.iID_ThanhToan_ChiTietID == x.iID_ThanhToan_ChiTietID).FirstOrDefault();
                            x.sumKPDCUSD = a.sumKPDCUSD;
                            x.sumKPDCVND = a.sumKPDCVND;
                            x.sumQTDDUSD = a.sumQTDDUSD;
                            x.sumQTDDVND = a.sumQTDDVND;
                            x.fSoSanhKinhPhi_USD = x.sumKPDCUSD - x.sumQTDDUSD;
                            x.fSoSanhKinhPhi_VND = x.sumKPDCVND - x.sumQTDDVND;
                            if (isPrint)
                            {
                                x.fKeHoach_TTCP_USD = null;
                            }
                        });

                        returnData.AddRange(listHopDong);
                    }
                }
                else
                {
                    var listHopDong = list.Where(x => x.iLoaiNoiDungChi == loaiChiPhi.iLoaiNoiDungChi).ToList();
                    listHopDong.ForEach(x =>
                    {
                        var a = listHopDong.GroupBy(z => z.iID_ThanhToan_ChiTietID).Select(y => new
                        {
                            sumKPDCUSD = y.Sum(k => k.fKinhPhiDuocCap_Tong_USD),
                            sumKPDCVND = y.Sum(k => k.fKinhPhiDuocCap_Tong_VND),
                            sumQTDDUSD = y.Sum(k => k.fQuyetToanDuocDuyet_Tong_USD),
                            sumQTDDVND = y.Sum(k => k.fQuyetToanDuocDuyet_Tong_VND),
                            iID_ThanhToan_ChiTietID = y.Key
                        }).Where(l => l.iID_ThanhToan_ChiTietID == x.iID_ThanhToan_ChiTietID).FirstOrDefault();
                        x.sumKPDCUSD = a.sumKPDCUSD;
                        x.sumKPDCVND = a.sumKPDCVND;
                        x.sumQTDDUSD = a.sumQTDDUSD;
                        x.sumQTDDVND = a.sumQTDDVND;
                        x.fSoSanhKinhPhi_USD = x.sumKPDCUSD - x.sumQTDDUSD;
                        x.fSoSanhKinhPhi_VND = x.sumKPDCVND - x.sumQTDDVND;
                        if (isPrint)
                        {
                            x.fKeHoach_TTCP_USD = null;
                        }
                    });
                    returnData.AddRange(listHopDong);
                }

            }
            return returnData;
        }
        [HttpPost]
        public JsonResult SaveDetail(List<NH_QT_QuyetToanDAHT_ChiTiet> data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            var returnData = _qlnhService.SaveQuyetToanDuAnDetail(data, Username);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModalInBaoCao(string[] listId)
        {
            lstDonViVND.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn đơn vị VND--" });
            ViewBag.ListDVVND = lstDonViVND;

            lstDonViUSD.Insert(0, new Dropdown_QuyetToanDAHT { Value = 0, Label = "--Chọn đơn vị USD--" });
            ViewBag.ListDVUSD = lstDonViUSD;

            ViewBag.IDQuyetToan = listId[0];
            return PartialView("_modalInBaoCao");
        }
        [HttpPost]
        public JsonResult SaveTongHop(NH_QT_QuyetToanDAHT data, string listId)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sSoDeNghi = HttpUtility.HtmlDecode(data.sSoDeNghi);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);

            if (!_qlnhService.SaveTongHopQuyetToanDAHT(data, Username, listId))
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
        public List<Dropdown_QuyetToanDAHT> GetListNamKeHoach()
        {
            List<Dropdown_QuyetToanDAHT> listNam = new List<Dropdown_QuyetToanDAHT>();
            int namHienTai = DateTime.Now.Year + 1;
            for (int i = 20; i > 0; i--)
            {
                namHienTai -= 1;
                Dropdown_QuyetToanDAHT namKeHoachOpt = new Dropdown_QuyetToanDAHT()
                {
                    Value = namHienTai,
                    Label = namHienTai.ToString()
                };
                listNam.Add(namKeHoachOpt);
            }
            return listNam;
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
    }
}