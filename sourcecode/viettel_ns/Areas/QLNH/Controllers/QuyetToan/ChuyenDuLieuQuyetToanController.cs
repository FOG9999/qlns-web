using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel.Controls;
using DomainModel;
using System.Collections.Specialized;
using VIETTEL.Models;
using Viettel.Services;
using VIETTEL.Controllers;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH.QuyetToan.ChuyenQuyetToan;
using Viettel.Models.QLNH.QuyetToan;

namespace VIETTEL.Areas.QLNH.Controllers.QuyetToan
{
    public class ChuyenDuLieuQuyetToanController : AppController
    {
        // GET: QLNH/ChuyenDuLieuChuyenHoa
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        public ActionResult Index()
        {
            QuyetToan_ChuyenQuyetToanModel vm = new QuyetToan_ChuyenQuyetToanModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qlnhService.GetListChuyenQuyetToan(ref vm._paging, null, null, null, null, null);

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            return View(vm);
        }

        [Authorize]
        public ActionResult Edit(string id, string sLNS)
        {
            string page = Request.QueryString["page"];
            int CurrentPage = 1;
            if (string.IsNullOrEmpty(page) == false)
            {
                CurrentPage = Convert.ToInt32(page);
            }
            DataTable dt = _qlnhService.Get_dtMucLucNganSach(CurrentPage, Globals.PageSize);
            double nums = _qlnhService.Get_MucLucNganSach_Count();
            int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
            ViewBag.totalPages = TotalPages;
            ViewBag.ListMucLuc = dt;
            ViewBag.idChuyenQuyetToan = id;

            return View();
        }

        public ActionResult Detail(string id, bool edit)
        {
            NH_QT_ChuyenQuyetToan_ChiTietView vm = new NH_QT_ChuyenQuyetToan_ChiTietView();
            var quyetToanDonVi = _qlnhService.GetThongTinChuyenQuyetToanById(new Guid(id));
            vm.ChuyenQuyetToanDetail = quyetToanDonVi;

            ViewBag.IsEdit = edit;
            ViewBag.IdQuyetToan = id;
            return View(vm);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid? id)
        {
            NH_QT_ChuyenQuyetToanData data = new NH_QT_ChuyenQuyetToanData();
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinChuyenQuyetToanById(id.Value);
                var donvi = _nganSachService.GetDonViById(PhienLamViec.NamLamViec.ToString(), data.iID_DonViID.ToString());
                data.sTenDonVi = donvi != null ? donvi.iID_MaDonVi + " - " + donvi.sTen : string.Empty;
                data.sLoaiThoiGian = data.iLoaiThoiGian == 1 ? "Tháng" : "Quý";
            }

            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult GetListDropDownThoiGian(int? iLoaiThoiGian)
        {
            var result = new List<dynamic>() {
                new
                    {
                        id = 0,
                        text = "-- Chọn --",
                    }
            };
            var listModel = new List<Dropdown_SelectValue>();
            switch (iLoaiThoiGian)
            {
                case 1:
                    listModel = GetListByMonth();
                    break;
                case 2:
                    listModel = GetListByQuy();
                    break;
                default:
                    listModel = new List<Dropdown_SelectValue>();
                    break;
            }
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

        public List<Dropdown_SelectValue> GetListByQuy()
        {
            var result = new List<Dropdown_SelectValue>();
            for (var i = 1; i <= 4; i++)
            {
                result.Add(new Dropdown_SelectValue()
                {
                    Value = i,
                    Label = "Quý " + i
                });
            }
            return result;
        }

        public List<Dropdown_SelectValue> GetListByMonth()
        {
            var result = new List<Dropdown_SelectValue>();
            for (var i = 1; i <= 12; i++)
            {
                result.Add(new Dropdown_SelectValue()
                {
                    Value = i,
                    Label = "Tháng " + i
                });
            }
            return result;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult MLNSChiTiet_Frame(string sLNS, string sK, string sL, string sM, string sMoTa, string sNG, string sTM, string sTNG, string sTTM, string fGiaTriUSD, string id)
        {
            ViewBag.idChuyenQuyetToan = id;
            return PartialView("ChuyenDuLieuQuyetToan_Edit_Frame", new { sLNS = sLNS, sK = sK, sL = sL, sM = sM, sMoTa = sMoTa, sNG = sNG, sTM = sTM, sTNG = sTNG, sTTM = sTTM, fGiaTriUSD = fGiaTriUSD });

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetModal(Guid? id)
        {
            NH_QT_ChuyenQuyetToan data = new NH_QT_ChuyenQuyetToan();

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;
            if (id.HasValue)
            {
                data = _qlnhService.GetThongTinChuyenQuyetToanById(id.Value);
            }
            return PartialView("_modalUpdate", data);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult DetailSubmit(string sLNS)
        {
            string iID_ChuyenQuyetToan = Request.QueryString["iID_ChuyenQuyetToan"];
            string TenBangChiTiet = "NH_QT_ChuyenQuyetToan_ChiTiet";
            string idXauMaCacHang = Request.Form["idXauMaCacHang"];
            string idXauMaCacCot = Request.Form["idXauMaCacCot"];
            string idXauGiaTriChiTiet = Request.Unvalidated.Form["idXauGiaTriChiTiet"];
            string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];
            string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
            string[] arrMaHang = idXauMaCacHang.Split(',');
            string[] arrHangDaXoa = idXauCacHangDaXoa.Split(',');
            string[] arrMaCot = idXauMaCacCot.Split(',');
            string[] arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] { BangDuLieu.DauCachHang }, StringSplitOptions.None);

            string iID_MaChungTuChiTiet;
            //Luu cac hang sua
            string[] arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] { BangDuLieu.DauCachHang }, StringSplitOptions.None);
            for (int i = 0; i < arrMaHang.Length; i++)
            {
                iID_MaChungTuChiTiet = arrMaHang[i];
                if (arrHangDaXoa[i] == "1")
                {
                    //Lưu các hàng đã xóa
                    if (iID_MaChungTuChiTiet != "")
                    {
                        //Dữ liệu đã có
                        Bang bang = new Bang(TenBangChiTiet);
                        bang.DuLieuMoi = false;
                        bang.GiaTriKhoa = iID_MaChungTuChiTiet;
                        bang.CmdParams.Parameters.AddWithValue("@iTrangThai", 0);
                        bang.MaNguoiDungSua = User.Identity.Name;
                        bang.IPSua = Request.UserHostAddress;
                        bang.Save();
                    }
                }
                else
                {
                    string[] arrGiaTri = arrHangGiaTri[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                    string[] arrThayDoi = arrHangThayDoi[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                    bool okCoThayDoi = false;
                    for (int j = 0; j < arrMaCot.Length; j++)
                    {
                        if (arrThayDoi[j] == "1")
                        {
                            okCoThayDoi = true;
                            break;
                        }
                    }
                    if (okCoThayDoi)
                    {
                        Bang bang = new Bang(TenBangChiTiet);
                        iID_MaChungTuChiTiet = arrMaHang[i];
                        if (iID_MaChungTuChiTiet == "")
                        {
                            //Du Lieu Moi
                            bang.DuLieuMoi = true;
                        }
                        else
                        {
                            //Du Lieu Da Co
                            bang.GiaTriKhoa = iID_MaChungTuChiTiet;
                            bang.DuLieuMoi = false;
                        }
                        bang.MaNguoiDungSua = User.Identity.Name;
                        bang.IPSua = Request.UserHostAddress;

                        bang.CmdParams.Parameters.AddWithValue("@iSTT", i);
                        bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", MucLucNganSachModels.LayNamLamViec(User.Identity.Name));
                        bang.CmdParams.Parameters.AddWithValue("@iID_ChuyenQuyetToanID", iID_ChuyenQuyetToan);
                        //Them tham so
                        for (int j = 0; j < arrMaCot.Length; j++)
                        {
                            if (arrThayDoi[j] == "1")
                            {
                                string Truong = "@" + arrMaCot[j];
                                if (arrMaCot[j].StartsWith("b"))
                                {
                                    //Nhap Kieu checkbox
                                    if (arrGiaTri[j] == "1")
                                    {
                                        bang.CmdParams.Parameters.AddWithValue(Truong, true);
                                    }
                                    else
                                    {
                                        bang.CmdParams.Parameters.AddWithValue(Truong, false);
                                    }
                                }
                                else if (arrMaCot[j].StartsWith("r") ||
                                         (arrMaCot[j].StartsWith("i") && arrMaCot[j].StartsWith("iID") == false))
                                {
                                    //Nhap Kieu so
                                    if (CommonFunction.IsNumeric(arrGiaTri[j]))
                                    {
                                        bang.CmdParams.Parameters.AddWithValue(Truong, Convert.ToDouble(arrGiaTri[j]));
                                    }
                                }
                                else
                                {
                                    //Nhap kieu xau
                                    bang.CmdParams.Parameters.AddWithValue(Truong, HttpUtility.HtmlDecode(arrGiaTri[j]));
                                }
                            }
                        }
                        if (iID_MaChungTuChiTiet == "")
                        {
                            bang.CmdParams.Parameters.RemoveAt(bang.CmdParams.Parameters.IndexOf("@iID_MaMucLucNganSach"));
                            bang.CmdParams.Parameters.RemoveAt(bang.CmdParams.Parameters.IndexOf("@iID_MaMucLucNganSach_Cha"));
                        }
                        bang.Save();
                    }
                }
            }

            MucLucNganSachModels.CapNhapLai(sLNS, User.Identity.Name);

            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Xoa(string id)
        {
            if (!_qlnhService.DeleteChuyenQuyetToan(Guid.Parse(id)))
            {
                return Json(new { bIsComplete = false, sMessError = "Không xóa được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocSubmit(String ParentID, string id)
        {
            String sLNS = Request.Form[ParentID + "_sLNS"];
            return Redirect("/QLNH/ChuyenDuLieuQuyetToan/Edit?id=" + id + "&sLNS=" + sLNS);
        }

        [HttpPost]
        public ActionResult TimKiem(PagingInfo _paging, string sSoChungTu, DateTime? dNgayChungTu, Guid? iDonVi, int? iLoaiThoiGian, int? iThoiGian)
        {
            QuyetToan_ChuyenQuyetToanModel vm = new QuyetToan_ChuyenQuyetToanModel();
            vm._paging = _paging;
            sSoChungTu = HttpUtility.HtmlDecode(sSoChungTu);
            vm.Items = _qlnhService.GetListChuyenQuyetToan(ref vm._paging, sSoChungTu
                , (dNgayChungTu == null ? null : dNgayChungTu),
                (iDonVi == Guid.Empty ? null : iDonVi),
                (iLoaiThoiGian == null ? null : iLoaiThoiGian),
                (iThoiGian == null ? null : iThoiGian)
            );

            List<NS_DonVi> lstDonViQL = _nganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec, false, false).ToList();
            lstDonViQL.Insert(0, new NS_DonVi { iID_Ma = Guid.Empty, sMoTa = "--Chọn đơn vị--" });
            ViewBag.ListDonVi = lstDonViQL;

            return PartialView("_list", vm);
        }

        [HttpPost]
        public JsonResult Save(NH_QT_ChuyenQuyetToan data)
        {
            if (data == null)
            {
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);
            }

            data.sSoChungTu = HttpUtility.HtmlDecode(data.sSoChungTu);
            data.sMoTa = HttpUtility.HtmlDecode(data.sMoTa);
            data.iID_MaDonVi = HttpUtility.HtmlDecode(data.iID_MaDonVi);

            var returnData = _qlnhService.SaveChuyenQuyetToan(data, Username, PhienLamViec.NamLamViec);
            if (!returnData.IsReturn)
            {
                return Json(new { bIsComplete = false, sMessError = returnData.errorMess }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { bIsComplete = true, dataID = returnData.ChuyenQuyetToanData.ID }, JsonRequestBehavior.AllowGet);
        }

    }
}