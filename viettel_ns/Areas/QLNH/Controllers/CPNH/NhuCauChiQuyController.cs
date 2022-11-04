using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Models.CPNH;
using Viettel.Services;
using DapperExtensions;
using VIETTEL.Helpers;
using System.Data;
using FlexCel.Core;
using FlexCel.Report;
using VIETTEL.Flexcel;
using FlexCel.XlsAdapter;
using System.Text;
using System.Net;

namespace VIETTEL.Areas.QLNH.Controllers.CPNH
{
    public class NhuCauChiQuyController : FlexcelReportController
    {
        private readonly ICPNHService _cpnhService = CPNHService.Default;
        private readonly IQLNguonNganSachService _nnsService = QLNguonNganSachService.Default;
        private static List<CPNHNhuCauChiQuy_Model> _lstTongHop = new List<CPNHNhuCauChiQuy_Model>();
        private const string sFilePathBaoCao2 = "/Report_ExcelFrom/QLNH/rpt_Nhucauchiquy_BaoCaoHD2.xlsx";
        private const string sFilePathBaoCao1 = "/Report_ExcelFrom/QLNH/rpt_Nhucauchiquy_BaoCao1.xlsx";
        private int _columnCountBC1 = 7;
        private const string sControlName = "NhuCauChiQuy";
        private readonly List<string> MaTienTeList = new List<string>() { "USD", "VND", "EUR" };

        // GET: QLVonDauTu/QLDMTyGia
        public ActionResult Index()
        {
            CPNHNhuCauChiQuy_ModelPaging vm = new CPNHNhuCauChiQuy_ModelPaging();
            vm._paging.CurrentPage = 1;
            vm.Items = _cpnhService.getListNhuCauChiQuyModels(ref vm._paging, null, null, null, null, 0, 0, 0);
            _lstTongHop = new List<CPNHNhuCauChiQuy_Model>();
            List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "--Tất cả quý--", iQuy = 0},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                };
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            List<NS_PhongBan> lstPhongBanQuanLy = _cpnhService.GetBQuanlyList().ToList();
            lstPhongBanQuanLy.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sMoTa = "--Tất cả phòng ban--" });
            ViewBag.ListBQuanLy = lstPhongBanQuanLy.ToSelectList("iID_MaPhongBan", "sMoTa");


            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả đơn vị--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            ViewBag.VoucherTabIndex = "checked";
            ViewBag.VoucherAggregateTabIndex = "";
            ViewBag.TabIndex = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult NhuCauChiQuySearch(PagingInfo _paging, NH_NhuCauChiQuy data, int tabIndex)
        {
            CPNHNhuCauChiQuy_ModelPaging vm = new CPNHNhuCauChiQuy_ModelPaging();
            CPNHNhuCauChiQuy_ModelPaging DataB2 = new CPNHNhuCauChiQuy_ModelPaging();
            CPNHNhuCauChiQuy_ModelPaging DataBQL = new CPNHNhuCauChiQuy_ModelPaging();
            vm._paging = _paging;
            ViewBag.VoucherTabIndex = "";
            vm.Items = _cpnhService.getListNhuCauChiQuyModels(ref vm._paging, HttpUtility.HtmlDecode(data.sSoDeNghi), data.dNgayDeNghi, data.iID_BQuanLyID, data.iID_DonViID, data.iQuy, data.iNamKeHoach, tabIndex);

            List <CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "--Tất cả quý--", iQuy = 0},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                };
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            List<NS_PhongBan> lstPhongBanQuanLy = _cpnhService.GetBQuanlyList().ToList();
            lstPhongBanQuanLy.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sMoTa = "--Tất cả phòng ban--" });
            ViewBag.ListBQuanLy = lstPhongBanQuanLy.ToSelectList("iID_MaPhongBan", "sMoTa");


            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Tất cả đơn vị--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");
            if (tabIndex == 1)
            {
                ViewBag.VoucherTabIndex = "";
                ViewBag.VoucherAggregateTabIndex = "checked";
                ViewBag.TabIndex = 1;
            }
            else if (tabIndex == 0)
            {
                ViewBag.VoucherTabIndex = "checked";
                ViewBag.VoucherAggregateTabIndex = "";
                ViewBag.TabIndex = 0;
            }

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetListNhuCauChiQuy()
        {
            var ListNhucauchiquy = _cpnhService.GetListNhuCauChiQuy("", null, Guid.Empty, Guid.Empty, 0, "");
            ViewBag.VoucherTabIndex = "";
            ViewBag.VoucherAggregateTabIndex = "checked";
            ViewBag.TabIndex = 1;

            return Json(new { data = ListNhucauchiquy }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetListNhuCauChiQuyByName(string sSodenghi, DateTime? dNgaydenghi, Guid? iBQuanly, Guid? iDonvi, int? iQuy, string iNam)
        {
            var lstData = _cpnhService.GetListNhuCauChiQuy(sSodenghi, dNgaydenghi, iBQuanly, iDonvi, iQuy, iNam).ToList();
            return Json(new { data = lstData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            CPNHNhuCauChiQuy_Model data = new CPNHNhuCauChiQuy_Model();
            if (id.HasValue)
            {
                data = _cpnhService.GetNhuCauChiQuyId(id.Value);
            }
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public ActionResult GetModalInBaoCao(string iloai)
        {
            CPNHNhuCauChiQuy_Model data = new CPNHNhuCauChiQuy_Model();
            List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
            {
                new CPNHNhuCauChiQuy_Model(){SQuyTypes = "--Chọn quý--", iQuy = 0},
                new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
            };
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            List<NS_PhongBan> lstPhongBanQuanLy = _cpnhService.GetBQuanlyList().ToList();
            lstPhongBanQuanLy.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sMoTa = "--Chọn phòng ban--" });
            ViewBag.ListBQuanLy = lstPhongBanQuanLy.ToSelectList("iID_MaPhongBan", "sMoTa");
            ViewBag.iloai = iloai;

            List<CPNHNhuCauChiQuy_Model> lstDVTinh = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){sDVTinh = "Không", iDVTinh = 0},
                    new CPNHNhuCauChiQuy_Model(){sDVTinh = "Nghìn", iDVTinh = 1},
                    new CPNHNhuCauChiQuy_Model(){sDVTinh = "Triệu", iDVTinh = 2},
                    new CPNHNhuCauChiQuy_Model(){sDVTinh = "Tỷ", iDVTinh = 3},
                };
            ViewBag.ListDVTinh = lstDVTinh.ToSelectList("iDVTinh", "sDVTinh");
            return PartialView("_modalGetpopupBaoCao", data);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id , bool dieuchinh)
        {
            CPNHNhuCauChiQuy_Model data = new CPNHNhuCauChiQuy_Model();
            if (id.HasValue)
            {
                data = _cpnhService.GetNhuCauChiQuyId(id.Value);
            }
            List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                {
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "--Chọn quý--", iQuy = 0},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                    new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                };
            ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
            List<NS_PhongBan> lstPhongBanQuanLy = _cpnhService.GetBQuanlyList().ToList();
            lstPhongBanQuanLy.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sMoTa = "--Chọn phòng ban--" });
            ViewBag.ListBQuanLy = lstPhongBanQuanLy.ToSelectList("iID_MaPhongBan", "sMoTa");

            List<NS_DonVi> lstDonViQuanLy = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            lstDonViQuanLy.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị quản lý--" });
            ViewBag.ListDonViQuanLy = lstDonViQuanLy.ToSelectList("iID_Ma", "sMoTa");

            List<NH_DM_TiGia> lstTyGia = _cpnhService.GetDonviListTyGia().ToList();
            lstTyGia.Insert(0, new NH_DM_TiGia { ID = Guid.Empty, sTenTiGia = "--Chọn tỷ giá--" });
            ViewBag.ListTyGia = lstTyGia;

            List<NH_DM_TiGia_ChiTiet> lstMangoaitekhac = _cpnhService.GetDonviListMangoaitekhac(null , false).ToList();
            lstMangoaitekhac.Insert(0, new NH_DM_TiGia_ChiTiet { ID = Guid.Empty, sMaTienTeQuyDoi = "--Chọn mã ngoại tệ khác--" });
            ViewBag.ListMaNgoaitekhac = lstMangoaitekhac;
            if (dieuchinh)
            {
                ViewBag.dieuchinh = "1";
            }
            else
            {
                ViewBag.dieuchinh = "0";
            }
            
            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public ActionResult NhuCauChiQuyPopupDelete(Guid? id)
        {
            CPNHNhuCauChiQuy_Model data = new CPNHNhuCauChiQuy_Model();
            if (id.HasValue)
            {
                data = _cpnhService.GetNhuCauChiQuyId(id.Value);
            }
            return PartialView("_modalDelete", data);
        }

        public ActionResult UpdateChitiet()
        {
            Guid IDNhuCauChiQuy = Guid.Parse(Request.QueryString["ID"].ToString());
            CPNHNhuCauChiQuy_Model data = new CPNHNhuCauChiQuy_Model();
            data = _cpnhService.GetNhuCauChiQuyId(IDNhuCauChiQuy);
            return PartialView("_modalUpdateChitiet", data);
        }
        public ActionResult XemChitiet()
        {
            Guid IDNhuCauChiQuy = Guid.Parse(Request.QueryString["ID"].ToString());
            CPNHNhuCauChiQuy_View_Model data = new CPNHNhuCauChiQuy_View_Model();
            data = _cpnhService.GetNhuCauChiQuyChitiet(IDNhuCauChiQuy);
            return PartialView("_modalXemChitiet", data);
        }
        public ActionResult SuaChitiet()
        {
            Guid IDNhuCauChiQuy = Guid.Empty;
            Guid IDUrl = Guid.Parse(Request.QueryString["ID"].ToString());
            string isDieuchinh = Request.QueryString["isDieuchinh"].ToString();
            if (isDieuchinh == "1")
            {
                CPNHNhuCauChiQuy_Model DataNhucauchiquy = _cpnhService.GetNhuCauChiQuyId(IDUrl);
                IDNhuCauChiQuy = Guid.Parse(DataNhucauchiquy.iID_ParentAdjustID.ToString());
            }
            else
            {
                IDNhuCauChiQuy = IDUrl;
            }
            CPNHNhuCauChiQuy_View_Model data = new CPNHNhuCauChiQuy_View_Model();
            data = _cpnhService.GetNhuCauChiQuyChitiet(IDNhuCauChiQuy);
            ViewBag.dieuchinh = isDieuchinh;
            ViewBag.IDUrl = IDUrl;
            return PartialView("_modalSuaChitiet", data);
        }


        [HttpPost]
        public JsonResult NhuCauChiQuyDelete(Guid id)
        {
            if (!_cpnhService.DeleteNhuCauChiQuy(id))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NhuCauChiQuySave(NH_NhuCauChiQuy data , string dieuchinh)
        {
            var iTrangthai = 1;
            if (data.ID == Guid.Empty)
            {
                iTrangthai = 0;
            }
            NH_NhuCauChiQuy datainsert = _cpnhService.NhuCauChiQuySave(data, dieuchinh , Username);
            if (datainsert.ID == null || datainsert.ID == Guid.Empty)
            {
                return Json(new { ID = Guid.Empty, iTrangthai, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ID = datainsert.ID, iTrangthai , dieuchinh }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult _modeGeneralSave(NH_NhuCauChiQuy data, List<CPNHNhuCauChiQuy_Model> lstItem)
        {
            var iTrangthai = 1;
            if (data.ID == Guid.Empty)
            {
                iTrangthai = 0;
            }
            
            NH_NhuCauChiQuy datainsert = _cpnhService.NhuCauChiQuyTongHopSave(data, lstItem, Username);
            if (datainsert.ID == null || datainsert.ID == Guid.Empty)
            {
                return Json(new { ID = Guid.Empty, iTrangthai, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            
            return Json(new { ID = datainsert.ID, iTrangthai }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NhuCauChiQuyXem(NH_NhuCauChiQuy data)
        {
            var iTrangthai = 1;
            if (data.ID == Guid.Empty)
            {
                iTrangthai = 0;
            }
            NH_NhuCauChiQuy datainsert = _cpnhService.NhuCauChiQuySave(data, "0",Username);
            if (datainsert.ID == null || datainsert.ID == Guid.Empty)
            {
                return Json(new { ID = Guid.Empty, iTrangthai, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ID = datainsert.ID, iTrangthai }, JsonRequestBehavior.AllowGet);
        }

        public bool NhuCauChiQuyLock(Guid id)
        {
            try
            {
                CPNHNhuCauChiQuy_Model entity = _cpnhService.GetNhuCauChiQuyId(id);
                bool isLockOrUnlock = false;

                if (entity != null)
                {
                    isLockOrUnlock = entity.bIsKhoa;
                    return _cpnhService.LockOrUnLockNhuCauChiQuy(id, !isLockOrUnlock);
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
        public JsonResult GetHopDongAll(Guid? iID_DonViID, Guid? iID_BQuanLyID)
        {
            var result = new List<dynamic>();
            var listModel = _cpnhService.GetListHopDong(iID_DonViID, iID_BQuanLyID).ToList();
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.ID,
                        text = item.sTenHopDong
                    });
                }
            }
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetListBQuanLy()
        {
            var result = new List<dynamic>();
            var listModel = _cpnhService.GetBQuanlyList().ToList();
            listModel.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sMoTa = "--Tất cả ban quản lý--" });
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.iID_MaPhongBan,
                        text = item.sMoTa
                    });
                }
            }
            return Json(new { status = true, data = result });
        }

        [HttpPost]
        public JsonResult GetListDonvi()
        {
            var result = new List<dynamic>();
            var listModel = _cpnhService.GetDonviListByYear(PhienLamViec.NamLamViec).ToList();
            listModel.Insert(0, new NS_DonVi { iID_MaPhongBan = Guid.Empty, sMoTa = "--Tất cả đơn vị--" });
            if (listModel != null && listModel.Any())
            {
                foreach (var item in listModel)
                {
                    result.Add(new
                    {
                        id = item.iID_Ma,
                        text = item.sMoTa
                    });
                }
            }
            return Json(new { status = true, data = result });
        }
        public JsonResult ChangeTiGia(Guid? idTiGia)
        {
            try
            {
                StringBuilder htmlMNTK = new StringBuilder();
                string htmlTienTe = string.Empty;

                if (idTiGia != null && idTiGia != Guid.Empty)
                {
                    List<NH_DM_TiGia_ChiTiet> lstMaNgoaiTeKhac = _cpnhService.GetNHDMTiGiaChiTietList(idTiGia).ToList();
                    if (lstMaNgoaiTeKhac != null && lstMaNgoaiTeKhac.Count > 0)
                    {
                        for (int i = 0; i < lstMaNgoaiTeKhac.Count; i++)
                        {
                            htmlMNTK.AppendFormat("<option value='{0}'>{1}</option>", lstMaNgoaiTeKhac[i].sMaTienTeQuyDoi, WebUtility.HtmlEncode(lstMaNgoaiTeKhac[i].sMaTienTeQuyDoi));
                        }
                    }
                }
                return Json(new
                {
                    bIsComplete = true,
                    htmlMNTK = htmlMNTK.ToString(),
                    htmlTienTe = htmlTienTe,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { bIsComplete = false, sMessError = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        private string GetHtmlTienteQuyDoi(List<NH_DM_TiGia_ChiTiet> tiGiaChiTietList, string maTienGoc)
        {
            StringBuilder htmlTienTe = new StringBuilder();
            var lstMaTienTe = new List<string>();
            lstMaTienTe.Add("VND");
            lstMaTienTe.Add("USD");

            foreach (var tgct in tiGiaChiTietList)
            {
                // Nếu mã tiền gốc là VND hoặc USD thì check trong tỉ giá chi tiết xem mã tiền tệ quy đổi có chứa VND or USD?
                if (tgct.fTiGia.HasValue && lstMaTienTe.Contains(maTienGoc.ToUpper().Trim()))
                {
                    if (lstMaTienTe.Contains(tgct.sMaTienTeQuyDoi.ToUpper().Trim()))
                    {
                        htmlTienTe.AppendFormat("1 {0} = {1} {2}; ", maTienGoc.Trim(), tgct.fTiGia.Value.ToString("0." + new string('#', 339)), tgct.sMaTienTeQuyDoi.Trim());
                    }
                }
                // Nếu mã tiền gốc không phải là VND or USD thì check thêm mà tiền tệ quy đổi có VND or USD?
                else if (tgct.fTiGia.HasValue && !lstMaTienTe.Contains(maTienGoc.ToUpper().Trim()) && lstMaTienTe.Contains(tgct.sMaTienTeQuyDoi.ToUpper().Trim()))
                {
                    htmlTienTe.AppendFormat("1 {0} = {1} {2}; ", maTienGoc.Trim(), tgct.fTiGia.Value.ToString("0." + new string('#', 339)), tgct.sMaTienTeQuyDoi.Trim());
                }
            }

            return htmlTienTe.ToString();
        }

        [HttpPost]
        public JsonResult LuuNhuCauChiQuyChiTiet(CPNHNhuCauChiQuy_Create_Model data , string dieuchinh , string IDUrl)
        {
            var IDNhuCauChiQuy = Guid.Empty;
            data.fTongChiNgoaiTeUSD = Convert.ToDouble(data.sTongChiNgoaiTeUSD.Replace(".", ","));
            data.fTongChiNgoaiTeVND = Convert.ToDouble(data.sTongChiNgoaiTeVND.Replace(".", ","));
            data.fTongChiTrongNuocVND = Convert.ToDouble(data.sTongChiTrongNuocVND.Replace(".", ","));
            data.fTongChiTrongNuocUSD = Convert.ToDouble(data.sTongChiTrongNuocUSD.Replace(".", ","));

            //data.ListNCCQChiTiet.fChiNgoaiTeUSD = Convert.ToDouble(data.ListNCCQChiTiet.sChiNgoaiTeUSD.Replace(".",","));
            //data.ListNCCQChiTiet.fChiNgoaiTeVND = Convert.ToDouble(data.ListNCCQChiTiet.sChiNgoaiTeVND.Replace(".",","));
            //data.ListNCCQChiTiet.fChiTrongNuocUSD = Convert.ToDouble(data.ListNCCQChiTiet.sChiTrongNuocUSD.Replace(".",","));
            //data.ListNCCQChiTiet.fChiTrongNuocVND = Convert.ToDouble(data.ListNCCQChiTiet.sChiTrongNuocVND.Replace(".",","));

            if (data.ListNCCQChiTiet == null)
                data.ListNCCQChiTiet = new List<CPNHNhuCauChiQuy_ChiTiet_Model>();

            foreach (var item in data.ListNCCQChiTiet)
            {
                if (item.iID_HopDongID == Guid.Empty && !item.isDelete)
                    return Json(new { status = false, message = "Chưa nhập tên nội dung chi !" });
            }

            //var anyDuplicate = data.ListNCCQChiTiet.Where(o => !o.isDelete).GroupBy(x => x.iID_HopDongID).Any(g => g.Count() > 1);
            //if (anyDuplicate)
            //{
            //    return Json(new { status = false, message = "Nội dung chi đã tồn tại. Vui lòng chọn lại!" });
            //}

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                //Lưu danh sách nguồn vốn, hạng mục
                List<CPNHNhuCauChiQuy_ChiTiet_Model> listAddChitietDieuChinh = data.ListNCCQChiTiet.Where(x => (dieuchinh == "1") && !x.isDelete).ToList();
                List<CPNHNhuCauChiQuy_ChiTiet_Model> listAddChitiet = data.ListNCCQChiTiet.Where(x => ((x.ID == null || x.ID == Guid.Empty) && dieuchinh != "1") && !x.isDelete).ToList();
                List<CPNHNhuCauChiQuy_ChiTiet_Model> listChitietEdit = data.ListNCCQChiTiet.Where(x => x.ID != null && x.iID_HopDongID != Guid.Empty && !x.isDelete && dieuchinh != "1").ToList();
                List<CPNHNhuCauChiQuy_ChiTiet_Model> listChitietDelete = data.ListNCCQChiTiet.Where(x => x.ID != null && x.ID != Guid.Empty && x.isDelete && dieuchinh != "1").ToList();

                if (listAddChitietDieuChinh.Count > 0)
                {
                    foreach (var item in listAddChitietDieuChinh)
                    {
                        var addChitiet = new NH_NhuCauChiQuy_ChiTiet();

                        addChitiet.iID_NhuCauChiQuyID =  Guid.Parse(IDUrl);
                        addChitiet.iID_HopDongID = item.iID_HopDongID;
                        addChitiet.sNoiDung = item.sNoiDung;
                        addChitiet.fChiNgoaiTeUSD = Convert.ToDouble(item.sChiNgoaiTeUSD.Replace(".", ","));
                        addChitiet.fChiNgoaiTeVND = Convert.ToDouble(item.sChiNgoaiTeVND.Replace(".", ","));
                        addChitiet.fChiTrongNuocUSD = Convert.ToDouble(item.sChiTrongNuocUSD.Replace(".", ","));
                        addChitiet.fChiTrongNuocVND = Convert.ToDouble(item.sChiTrongNuocVND.Replace(".", ","));
                        addChitiet.ID = Guid.NewGuid();
                        conn.Insert<NH_NhuCauChiQuy_ChiTiet>(addChitiet, trans);
                        IDNhuCauChiQuy = Guid.Parse(addChitiet.iID_NhuCauChiQuyID.ToString());
                    }
                }

                if (listAddChitiet.Count > 0)
                {
                    foreach (var item in listAddChitiet)
                    {
                        var addChitiet = new NH_NhuCauChiQuy_ChiTiet();
                        addChitiet.iID_NhuCauChiQuyID = item.iID_NhuCauChiQuyID;
                        addChitiet.iID_HopDongID = item.iID_HopDongID;
                        addChitiet.sNoiDung = item.sNoiDung;
                        addChitiet.fChiNgoaiTeUSD = Convert.ToDouble(item.sChiNgoaiTeUSD.Replace(".", ","));
                        addChitiet.fChiNgoaiTeVND = Convert.ToDouble(item.sChiNgoaiTeVND.Replace(".", ","));
                        addChitiet.fChiTrongNuocUSD = Convert.ToDouble(item.sChiTrongNuocUSD.Replace(".", ","));
                        addChitiet.fChiTrongNuocVND = Convert.ToDouble(item.sChiTrongNuocVND.Replace(".", ","));
                        addChitiet.ID = Guid.NewGuid();
                        conn.Insert<NH_NhuCauChiQuy_ChiTiet>(addChitiet, trans);
                        IDNhuCauChiQuy = Guid.Parse(addChitiet.iID_NhuCauChiQuyID.ToString());
                    }
                }
                if (listChitietEdit.Count > 0)
                {
                    foreach (var item in listChitietEdit)
                    {
                        NH_NhuCauChiQuy_ChiTiet EditChitiet = conn.Get<NH_NhuCauChiQuy_ChiTiet>(item.ID, trans);
                        if (EditChitiet != null)
                        {
                            EditChitiet.iID_HopDongID = item.iID_HopDongID;
                            EditChitiet.sNoiDung = item.sNoiDung;
                            EditChitiet.fChiNgoaiTeUSD = Convert.ToDouble(item.sChiNgoaiTeUSD.Replace(".", ","));
                            EditChitiet.fChiNgoaiTeVND = Convert.ToDouble(item.sChiNgoaiTeVND.Replace(".", ","));
                            EditChitiet.fChiTrongNuocUSD = Convert.ToDouble(item.sChiTrongNuocUSD.Replace(".", ","));
                            EditChitiet.fChiTrongNuocVND = Convert.ToDouble(item.sChiTrongNuocVND.Replace(".", ","));
                            conn.Update<NH_NhuCauChiQuy_ChiTiet>(EditChitiet, trans);
                            IDNhuCauChiQuy = Guid.Parse(EditChitiet.iID_NhuCauChiQuyID.ToString());
                        }
                    }
                }
                if (listChitietDelete.Count > 0)
                {
                    foreach (var item in listChitietDelete)
                    {
                        NH_NhuCauChiQuy_ChiTiet DeleteChitiet = conn.Get<NH_NhuCauChiQuy_ChiTiet>(item.ID, trans);
                        if (DeleteChitiet != null)
                        {
                            conn.Delete<NH_NhuCauChiQuy_ChiTiet>(DeleteChitiet, trans);
                        }
                    }
                }

                NH_NhuCauChiQuy EditNhuCauChiQuy = conn.Get<NH_NhuCauChiQuy>(IDNhuCauChiQuy, trans);
                EditNhuCauChiQuy.fTongChiNgoaiTeUSD = Convert.ToDouble(data.sTongChiNgoaiTeUSD.Replace(".", ","));
                EditNhuCauChiQuy.fTongChiNgoaiTeVND = Convert.ToDouble(data.sTongChiNgoaiTeVND.Replace(".", ","));
                EditNhuCauChiQuy.fTongChiTrongNuocUSD = Convert.ToDouble(data.sTongChiTrongNuocUSD.Replace(".", ","));
                EditNhuCauChiQuy.fTongChiTrongNuocVND = Convert.ToDouble(data.sTongChiTrongNuocVND.Replace(".", ","));
                conn.Update<NH_NhuCauChiQuy>(EditNhuCauChiQuy, trans);

                trans.Commit();
            }

            return Json(new { status = true, id = IDNhuCauChiQuy , dieuchinh = dieuchinh });
        }
        [HttpPost]
        public ActionResult UpdateGeneral(Guid? id, bool isAggregate, List<CPNHNhuCauChiQuy_Model> lstItem , bool ChiTiet)
        {
            CPNHNhuCauChiQuy_Model data = new CPNHNhuCauChiQuy_Model();
            try
            {
                _lstTongHop = new List<CPNHNhuCauChiQuy_Model>();

                ViewBag.IsAggregate = true;

                List<CPNHNhuCauChiQuy_Model> lstVoucherTypes = new List<CPNHNhuCauChiQuy_Model>()
                        {
                            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "--Tất cả--", iQuy = 0},
                            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 1", iQuy = 1},
                            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 2", iQuy = 2},
                            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 3", iQuy = 3},
                            new CPNHNhuCauChiQuy_Model(){SQuyTypes = "Quý 4", iQuy = 4}
                        };
                ViewBag.ListQuyTypes = lstVoucherTypes.ToSelectList("iQuy", "SQuyTypes");
                List<NS_PhongBan> lstPhongBanQuanLy = _cpnhService.GetBQuanlyList().ToList();
                lstPhongBanQuanLy.Insert(0, new NS_PhongBan { iID_MaPhongBan = Guid.Empty, sMoTa = "--Tất cả--" });
                ViewBag.ListBQuanLy = lstPhongBanQuanLy.ToSelectList("iID_MaPhongBan", "sMoTa");

                if (id.HasValue)
                {
                    lstItem = new List<CPNHNhuCauChiQuy_Model>();
                    data = _cpnhService.GetNhuCauChiQuyId(id.Value);
                    var arrayTongHop = data.sTongHop.Split(',');
                    int STT = 1;
                    foreach(var item in arrayTongHop)
                    {
                        CPNHNhuCauChiQuy_Model dataTongHop = _cpnhService.GetNhuCauChiQuyId(Guid.Parse(item));
                        dataTongHop.isChecked = true;
                        dataTongHop.STT = STT;
                        lstItem.Add(dataTongHop);
                        STT++;
                    }
                    ViewBag.LstTongHop = lstItem;
                }
                else if (isAggregate && !id.HasValue)
                {
                    
                    ViewBag.LstTongHop = _lstTongHop;

                    if (lstItem != null && lstItem.Count() > 0 && lstItem.FirstOrDefault() != null )
                    {
                        var lstValue = lstItem.GroupBy(x => x.ID).Select(grp => grp.LastOrDefault()).Where(x => x.isChecked).ToList();
                        var STT = 0;
                        foreach (var item in lstValue)
                        {
                            STT++;
                            var itemQuery = _cpnhService.GetNhuCauChiQuyId(item.ID);
                            itemQuery.STT = STT;
                            if (itemQuery != null)
                            {
                                _lstTongHop.Add(itemQuery);
                            }
                        }

                        _lstTongHop = _lstTongHop.GroupBy(item => item.ID).Select(grp => grp.FirstOrDefault()).ToList();

                        if (_lstTongHop.Count() == 0)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ để tổng hợp!" }, JsonRequestBehavior.AllowGet);
                        }

                        for (int i = 0; i < _lstTongHop.Count(); i++)
                        {
                            if (_lstTongHop[0].iNamKeHoach != _lstTongHop[i].iNamKeHoach)
                            {
                                return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có cùng năm !" }, JsonRequestBehavior.AllowGet);
                            }
                            if (_lstTongHop[0].iQuy != _lstTongHop[i].iQuy)
                            {
                                return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có cùng quý !" }, JsonRequestBehavior.AllowGet);
                            }
                            if (_lstTongHop[0].iID_BQuanLyID != _lstTongHop[i].iID_BQuanLyID)
                            {
                                return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ có cùng phòng ban !" }, JsonRequestBehavior.AllowGet);
                            }
                            if (_lstTongHop[i].iID_TongHopID != null && isAggregate == true)
                            {
                                if (_lstTongHop.Any(x => !string.IsNullOrEmpty(x.sTongHop)))
                                {
                                    return Json(new { bIsComplete = false, sMessError = "Chứng từ đã được tổng hợp (B2) !" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            if (_lstTongHop.Any(x => !string.IsNullOrEmpty(x.sTongHop)) && isAggregate == false)
                            {
                                return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ đã có tổng hợp (B QL)" }, JsonRequestBehavior.AllowGet);
                            }
                            if (_lstTongHop[i].iID_TongHopID != null && isAggregate == false)
                            {
                                return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ chưa có tổng hợp !" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        if (_lstTongHop.Any(x => x.bIsKhoa == false))
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn các chứng từ đã khóa !" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        ViewBag.LstTongHop = new List<CPNHNhuCauChiQuy_Model>();

                        if (_lstTongHop.Count() == 0)
                        {
                            return Json(new { bIsComplete = false, sMessError = "Vui lòng chọn chứng từ để tổng hợp!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            if (ChiTiet == true)
            {
                return PartialView("_modelDetailGeneral", data);
            }
            else
            {
                return PartialView("_modelUpdateGeneral", data);
            }
        }
        public ActionResult ExportExcelBaoCao(string sDonViCapTren, string sDonViCapDuoi, string ext = "pdf", int dvt = 1, int to = 1, Guid? iGui = null, string iloai = "" ,Guid? iDonvi = null, int iQuy = 0, int iNam = 0 , int iUSD = 0 , int iVND = 0)
        {
            string fileName = string.Format("{0}.{1}", "Bao_Cao_Nhu_Cau_Chi_Quy", ext);
            if (iloai == "1")
            {
                ExcelFile xls = TaoFileBaoCao1(sDonViCapTren, sDonViCapDuoi, dvt, to, iGui, iDonvi, iQuy, iNam);
                return Print(xls, ext, fileName);
            }
            else if (iloai == "2")
            {
                ExcelFile xls = TaoFileBaoCao2(sDonViCapTren, sDonViCapDuoi, dvt, to, iGui, iDonvi, iQuy, iNam , iUSD , iVND);
                return Print(xls, ext, fileName);
            }
            return null;
        }

        public ExcelFile TaoFileBaoCao1(string sDonViCapTren, string sDonViCapDuoi, int dvt = 1, int to = 1, Guid? iGui = null, Guid? iDonvi = null, int iQuy = 0, int iNam = 0)
        {
            sDonViCapTren = HttpUtility.UrlDecode(sDonViCapTren);
            sDonViCapDuoi = HttpUtility.UrlDecode(sDonViCapDuoi);

            var Gui = _cpnhService.GetPhongBanID((Guid)iGui);
            var Donvi = _cpnhService.GetPhongBanID((Guid)iDonvi);
            var sGui = "";
            if (Gui != null)
            {
                sGui = Gui.sTen + " - " + Gui.sMoTa;
            }
            var sDonvi = "";
            if (Donvi != null)
            {
                sDonvi = Donvi.sTen + " - " + Donvi.sMoTa;
            }
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao1));
            FlexCelReport fr = new FlexCelReport();

            int columnStart = _columnCountBC1 * (to - 1);

            List<CPNHNhuCauChiQuy_Model> dataNhucaichiquy = new List<CPNHNhuCauChiQuy_Model>();
            List<CPNHNhuCauChiQuy_ChiTiet_Model> ListData = new List<CPNHNhuCauChiQuy_ChiTiet_Model>();
            var listChitiet = _cpnhService.GetListNhucauchiquyChitiet(iDonvi , iQuy , iNam).ToList();
            int Stt = 0;
            int SttCha = 0;
            int SttChuongTrinh = 0;
            int Tong = 0;
            Guid? idcha = null;
            Guid? idchuongtrinh = null;
            foreach (var item in listChitiet)
            {
                Tong++;
                if (item.ID_NhiemVuChi != idchuongtrinh && item.ID_NhiemVuChi != null)
                {
                    SttChuongTrinh++;
                    SttCha = 0;
                    CPNHNhuCauChiQuy_ChiTiet_Model DataCha = new CPNHNhuCauChiQuy_ChiTiet_Model();
                    DataCha.Noidungchi = item.sTenNhiemVuChi;
                    DataCha.fChiNgoaiTeUSD = null;
                    DataCha.fChiTrongNuocVND = null;
                    DataCha.depth = convertLetter(SttChuongTrinh);
                    idchuongtrinh = item.ID_NhiemVuChi;
                    ListData.Add(DataCha);
                }
                if (item.iID_DonViID != idcha && item.iID_DonViID != null)
                {
                    SttCha++;
                    Stt = 0;
                    CPNHNhuCauChiQuy_ChiTiet_Model DataCha = new CPNHNhuCauChiQuy_ChiTiet_Model();
                    DataCha.Noidungchi = item.sTenDonvi;
                    DataCha.fChiNgoaiTeUSD = null;
                    DataCha.fChiTrongNuocVND = null;
                    DataCha.depth = _cpnhService.GetSTTLAMA(SttCha);
                    idcha = item.iID_DonViID;
                    ListData.Add(DataCha);
                }
                Stt++;
                item.depth = SttCha + "." + Stt;

                if (item.iID_HopDongID == null || item.iID_HopDongID == Guid.Empty)
                {
                    item.Noidungchi = item.sNoiDung;
                }
                else
                {
                    item.Noidungchi = item.sTenHopDong;
                }
                ListData.Add(item);

                if (Tong == listChitiet.Count)
                {
                    CPNHNhuCauChiQuy_ChiTiet_Model DataCha = new CPNHNhuCauChiQuy_ChiTiet_Model();
                    DataCha.Noidungchi = "Tổng Cộng: ";
                    DataCha.fChiNgoaiTeUSD = listChitiet.Sum(x => x.fChiNgoaiTeUSD);
                    DataCha.fChiTrongNuocVND = listChitiet.Sum(x => x.fChiTrongNuocVND); ;
                    DataCha.depth = null;
                    ListData.Add(DataCha);
                }
            }
            fr.AddTable<CPNHNhuCauChiQuy_ChiTiet_Model>("dt", ListData);
            fr.SetValue(new
            {
                dvt = dvt.ToStringDvt(),
                To = to,
                iNam = iNam,
                iQuy = iQuy,
                sKinhGui = sGui,
                sDonvi = sDonvi,
                sDonViCapTren = sDonViCapTren.ToUpper(),
                sDonViCapDuoi = sDonViCapDuoi.ToUpper()
            });
            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this).Run(Result);

            // merge
            Result.MergeH(5, 5, 13);
            Result.MergeH(6, 5, 13);

            Result.MergeC(5, 7, 5, 13);

            return Result;
        }

        public ExcelFile TaoFileBaoCao2(string sDonViCapTren, string sDonViCapDuoi, int dvt = 1, int to = 1, Guid? iGui = null, Guid? iDonvi = null, int iQuy = 0, int iNam = 0 , int iUSD = 0 , int iVND = 0)
        {
            sDonViCapTren = HttpUtility.UrlDecode(sDonViCapTren);
            sDonViCapDuoi = HttpUtility.UrlDecode(sDonViCapDuoi);

            var sDvTinhUSD = "";
            var sDvTinhVND = "";
            if (iUSD == 1) { sDvTinhUSD = "Nghìn "; }
            else if (iUSD == 2) { sDvTinhUSD = "Triệu "; }
            else if (iUSD == 3) { sDvTinhUSD = "Tỷ "; }
            else { sDvTinhUSD = ""; }

            if (iVND == 1) { sDvTinhVND = "Nghìn "; }
            else if (iVND == 2) { sDvTinhVND = "Triệu "; }
            else if (iVND == 3) { sDvTinhVND = "Tỷ "; }
            else { sDvTinhVND = ""; }
            var Gui = _cpnhService.GetPhongBanID((Guid)iGui);
            var Donvi = _cpnhService.GetPhongBanID((Guid)iDonvi);
            var sGui = "";
            if (Gui != null)
            {
                sGui = Gui.sTen + " - " + Gui.sMoTa;
            }
            var sDonvi = "";
            if (Donvi != null)
            {
                sDonvi = Donvi.sTen + " - " + Donvi.sMoTa;
            }
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePathBaoCao2));
            FlexCelReport fr = new FlexCelReport();
            int columnStart = _columnCountBC1 * (to - 1);
            List<CPNHNhuCauChiQuy_Model> dataNhucaichiquy = new List<CPNHNhuCauChiQuy_Model>();
            List<CPNHNhuCauChiQuy_ChiTiet_Model> ListData = new List<CPNHNhuCauChiQuy_ChiTiet_Model>();
            var listChitiet = _cpnhService.GetListNhucauchiquyChitietBaoCao2(iUSD , iVND , iDonvi, iQuy, iNam).ToList();
            int Stt = 0;
            int SttCha = 0;
            int SttChuongTrinh = 0;
            int Tong = 0;
            Guid? idcha = null;
            Guid? idchuongtrinh = null;
            foreach (var item in listChitiet)
            {
                Tong++;
                if (item.ID_NhiemVuChi != idchuongtrinh && item.ID_NhiemVuChi != null)
                {
                    SttChuongTrinh++;
                    SttCha = 0;
                    CPNHNhuCauChiQuy_ChiTiet_Model DataCha = new CPNHNhuCauChiQuy_ChiTiet_Model();
                    DataCha.Noidungchi = item.sTenNhiemVuChi;
                    DataCha.fChiNgoaiTeUSD = null;
                    DataCha.fChiTrongNuocVND = null;
                    DataCha.depth = convertLetter(SttChuongTrinh);
                    idchuongtrinh = item.ID_NhiemVuChi;
                    ListData.Add(DataCha);
                }
                if (item.iID_DonViID != idcha && item.iID_DonViID != null)
                {
                    SttCha++;
                    Stt = 0;
                    CPNHNhuCauChiQuy_ChiTiet_Model DataCha = new CPNHNhuCauChiQuy_ChiTiet_Model();
                    DataCha.Noidungchi = item.sTenDonvi;
                    DataCha.fChiNgoaiTeUSD = null;
                    DataCha.fChiTrongNuocVND = null;
                    DataCha.depth = _cpnhService.GetSTTLAMA(SttCha);
                    idcha = item.iID_DonViID;
                    ListData.Add(DataCha);
                }
                Stt++;
                item.depth = SttCha + "." + Stt;

                if (item.iID_HopDongID == null || item.iID_HopDongID == Guid.Empty)
                {
                    item.Noidungchi = item.sNoiDung;
                }
                else
                {
                    item.Noidungchi = item.sTenHopDong;
                }
                ListData.Add(item);

                if (Tong == listChitiet.Count)
                {
                    CPNHNhuCauChiQuy_ChiTiet_Model DataCha = new CPNHNhuCauChiQuy_ChiTiet_Model();
                    DataCha.Noidungchi = "Tổng Cộng: ";
                    DataCha.GiaTriHopDongUSD = listChitiet.Sum(x => x.GiaTriHopDongUSD); 
                    DataCha.GiaTriHopDongVND = listChitiet.Sum(x => x.GiaTriHopDongVND);
                    DataCha.GiaTriGaiDoanUSD = listChitiet.Sum(x => x.GiaTriGaiDoanUSD);
                    DataCha.GiaTriBQPUSD = listChitiet.Sum(x => x.GiaTriBQPUSD);
                    DataCha.KinhPhiUSD = listChitiet.Sum(x => x.KinhPhiUSD);
                    DataCha.KinhPhiVND = listChitiet.Sum(x => x.KinhPhiVND);
                    DataCha.KinhPhiDaChiUSD = listChitiet.Sum(x => x.KinhPhiDaChiUSD);
                    DataCha.KinhPhiDaChiVND = listChitiet.Sum(x => x.KinhPhiDaChiVND);
                    DataCha.KinhPhiToYUSD = listChitiet.Sum(x => x.KinhPhiToYUSD);
                    DataCha.KinhPhiToYVND = listChitiet.Sum(x => x.KinhPhiToYVND);
                    DataCha.KinhPhiDaChiToYUSD = listChitiet.Sum(x => x.KinhPhiDaChiToYUSD);
                    DataCha.KinhPhiDaChiToYVND = listChitiet.Sum(x => x.KinhPhiDaChiToYVND);
                    DataCha.fChiNgoaiTeUSDTyGia = listChitiet.Sum(x => x.fChiNgoaiTeUSDTyGia);
                    DataCha.fChiTrongNuocVNDTyGia = listChitiet.Sum(x => x.fChiTrongNuocVNDTyGia);
                    DataCha.depth = null;
                    ListData.Add(DataCha);
                }
            }
            fr.AddTable<CPNHNhuCauChiQuy_ChiTiet_Model>("dt", ListData);
            fr.SetValue(new
            {
                dvt = dvt.ToStringDvt(),
                To = to,
                iNam = iNam,
                iQuy = iQuy,
                sKinhGui = sGui,
                sDonvi = sDonvi,
                sDvTinhUSD = sDvTinhUSD,
                sDvTinhVND = sDvTinhVND,
                sDonViCapTren = sDonViCapTren.ToUpper(),
                sDonViCapDuoi = sDonViCapDuoi.ToUpper()
            });
            fr.UseChuKy(Username)
                .UseChuKyForController(sControlName)
                .UseForm(this).Run(Result);

            // merge
            Result.MergeH(5, 5, 13);
            Result.MergeH(6, 5, 13);

            Result.MergeC(5, 7, 5, 13);

            return Result;
        }
        private string convertLetter(int input)
        {
            StringBuilder res = new StringBuilder((input - 1).ToString());
            for (int j = 0; j < res.Length; j++)
                res[j] += (char)(17); // '0' is 48, 'A' is 65
            return res.ToString();
        }
    }
}