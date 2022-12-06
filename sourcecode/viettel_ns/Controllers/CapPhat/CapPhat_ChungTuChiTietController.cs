using DomainModel;
using DomainModel.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.Mvc;
using VIETTEL.Models;


namespace VIETTEL.Controllers.CapPhat
{
    public class CapPhat_ChungTuChiTietController : Controller
    {
        public static readonly string VIEW_ROOTPATH = "~/Views/CapPhat/ChungTuChiTiet/";
        public static readonly string VIEW_CAPPHATCHITIET_INDEX_DONVI = "CapPhatChiTiet_Index_DonVi.aspx";
        public static readonly string VIEW_CAPPHATCHITIET_INDEX = "CapPhatChiTiet_Index.aspx";
        public static readonly string VIEW_CAPPHATCHITIET_DANHSACH_FRAME = "CapPhatChiTiet_Index_DanhSach_Frame.aspx";

        [Authorize]
        public ActionResult Index(string DonVi, string iID_MaCapPhat)
        {
            int opt = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["HienThiOpt"])))
                opt = Convert.ToInt32(Request.QueryString["HienThiOpt"]);
            if (string.IsNullOrEmpty(DonVi) == false)
            {
                return View(VIEW_ROOTPATH + VIEW_CAPPHATCHITIET_INDEX_DONVI);
            }
            return View(VIEW_ROOTPATH + VIEW_CAPPHATCHITIET_INDEX);
        }
        [Authorize]
        public ActionResult CapPhatChiTiet_Frame(string sLNS, string sL, string sK, string sM, string sTM, string sTTM, string sNG, string sTNG, string iID_MaDonVi, string iID_MaCapPhat, string f, string sMaLoai = "")
        {
            string MaLoai = Request.Form["CapPhat_MaLoai"];
            iID_MaCapPhat = Request.Form["CapPhat_iID_MaCapPhat"];
            if (string.IsNullOrEmpty(MaLoai))
            {
                ViewData["MaLoai"] = sMaLoai;
            }
            else
            {
                ViewData["MaLoai"] = MaLoai;
            }
            ViewData["iID_MaCapPhat"] = iID_MaCapPhat;
            ViewData["sLNS"] = sLNS;
            ViewData["sL"] = sL;
            ViewData["sK"] = sK;
            ViewData["sM"] = sM;
            ViewData["sTM"] = sTM;
            ViewData["sTTM"] = sTTM;
            ViewData["sNG"] = sNG;
            ViewData["sTNG"] = sTNG;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            return View(VIEW_ROOTPATH + VIEW_CAPPHATCHITIET_DANHSACH_FRAME);
        }
        /// <summary>
        /// Lưu chứng từ chi tiết
        /// </summary>
        /// <param name="ChiNganSach"></param>
        /// <param name="iID_MaCapPhat"></param>
        /// <param name="DonVi"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LuuChungTuChiTiet(string ChiNganSach, string iID_MaCapPhat, string DonVi)
        {
            NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);

            string MaND = User.Identity.Name;
            string TenBangChiTiet = "CP_CapPhatChiTiet";

            string idXauMaCacHang = Request.Form["idXauMaCacHang"];
            string idXauMaCacCot = Request.Form["idXauMaCacCot"];
            string idXauGiaTriChiTiet = Request.Form["idXauGiaTriChiTiet"];
            string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];
            string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
            string idXauLaHangCha = Request.Form["idXauLaHangCha"]; 
            int idNC_Fixed = Convert.ToInt32(Request.Form["idNC_Fixed"]);
            int idNC_Slide = Convert.ToInt32(Request.Form["idNC_Slide"]);

            String DSTruong = "iID_MaDonVi," + MucLucNganSachModels.strDSTruong;
            String[] arrDSTruong = DSTruong.Split(',');
            Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
            for (int i = 0; i < arrDSTruong.Length; i++)
            {
                arrGiaTriTimKiem.Add(arrDSTruong[i], Request.Form[arrDSTruong[i]]);
            }

            string[] arrMaHangCha = idXauLaHangCha.Split(',');
            string[] arrMaHang = idXauMaCacHang.Split(',');
            string[] arrHangDaXoa = idXauCacHangDaXoa.Split(',');
            string[] arrMaCot = idXauMaCacCot.Split(',');
            string[] arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] { CapPhat_BangDuLieu.DauCachHang }, StringSplitOptions.None);

            string iID_MaCapPhatChiTiet, iID_MaDonVi;

            //Luu cac hang sua
            string[] arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] { CapPhat_BangDuLieu.DauCachHang }, StringSplitOptions.None);
            for (int i = 0; i < arrMaHangCha.Length; i++)
            {
                if (arrMaHang[i] != "" && arrMaHangCha[i] == "0")
                {
                    iID_MaCapPhatChiTiet = arrMaHang[i].Split('_')[0];
                    iID_MaDonVi = arrMaHang[i].Split('_')[2];
                    if (arrHangDaXoa[i] == "1")
                    {
                        //Lưu các hàng đã xóa
                        if (iID_MaCapPhatChiTiet != "")
                        {
                            //Dữ liệu đã có
                            Bang bang = new Bang(TenBangChiTiet);
                            bang.DuLieuMoi = false;
                            bang.GiaTriKhoa = iID_MaCapPhatChiTiet;
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
                        Boolean okCoThayDoi = false;
                       
                        for (int j = idNC_Fixed; j < idNC_Fixed + idNC_Slide; j++)
                        {
                            if (arrThayDoi[j] == "1")
                            {
                                okCoThayDoi = true;
                            }
                            if (arrMaCot[j].StartsWith("iID_MaDonVi"))
                            {
                                if (arrGiaTri[j] == "" && iID_MaDonVi == "")
                                {
                                    if (iID_MaCapPhatChiTiet == "" && string.IsNullOrEmpty(data["iID_MaDonVi"]))
                                    {
                                        okCoThayDoi = false;
                                        break;
                                    }
                                    else
                                    {
                                        okCoThayDoi = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (okCoThayDoi)
                        {
                            Bang bang = new Bang(TenBangChiTiet);
                            if (iID_MaCapPhatChiTiet == "")
                            {
                                //Du Lieu Moi
                                bang.DuLieuMoi = true;
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);

                                //Them cac tham so tu bang CP_CapPhat
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBan", data["iID_MaPhongBan"]);
                                bang.CmdParams.Parameters.AddWithValue("@sTenPhongBan", data["sTenPhongBan"]);
                                bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", data["bChiNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", data["iID_MaTrangThaiDuyet"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaTinhChatCapThu", data["iID_MaTinhChatCapThu"]);
                                bang.CmdParams.Parameters.AddWithValue("@dNgayCapPhat", DateTime.Parse(data["dNgayCapPhat"]));
                                if (iID_MaDonVi != "" && string.IsNullOrEmpty(data["iID_MaDonVi"]))
                                {
                                    bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                                    bang.CmdParams.Parameters.AddWithValue("@sTenDonVi", DonViModels.Get_TenDonVi(iID_MaDonVi, MaND));
                                }
                                else if (!string.IsNullOrEmpty(data["iID_MaDonVi"]))
                                {
                                    bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", data["iID_MaDonVi"]);
                                    bang.CmdParams.Parameters.AddWithValue("@sTenDonVi", DonViModels.Get_TenDonVi(data["iID_MaDonVi"].ToString(), MaND));
                                }
                            }
                            else
                            {
                                //Du Lieu Da Co
                                bang.GiaTriKhoa = iID_MaCapPhatChiTiet;
                                bang.DuLieuMoi = false;
                            }
                            bang.MaNguoiDungSua = User.Identity.Name;
                            bang.IPSua = Request.UserHostAddress;

                            if (iID_MaCapPhatChiTiet == "")
                            {
                                //Xác định xâu mã nối
                                string sXauNoiMa = "";
                                for (int k = 0; k < MucLucNganSachModels.arrDSTruong.Length; k++)
                                {
                                    for (int j = 0; j < arrMaCot.Length; j++)
                                    {
                                        if (arrMaCot[j] == MucLucNganSachModels.arrDSTruong[k])
                                        {
                                            sXauNoiMa += string.Format("{0}-", arrGiaTri[j]);
                                            break;
                                        }
                                    }
                                }

                                string iID_MaMucLucNganSach = arrMaHang[i].Split('_')[1];

                                DataTable dtMucLuc = MucLucNganSachModels.dt_ChiTietMucLucNganSach(iID_MaMucLucNganSach);
                                //Dien thong tin cua Muc luc ngan sach
                                NganSach_HamChungModels.ThemThongTinCuaMucLucNganSach(dtMucLuc.Rows[0], bang.CmdParams.Parameters);
                                dtMucLuc.Dispose();
                            }

                            //Them tham so
                            for (int j = 0; j < arrMaCot.Length; j++)
                            {
                                if (arrThayDoi[j] == "1")
                                {
                                    if (arrMaCot[j].EndsWith("_ConLai") == false)
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
                                        else if (arrMaCot[j].StartsWith("r") || (arrMaCot[j].StartsWith("i") && arrMaCot[j].StartsWith("iID") == false))
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
                                            bang.CmdParams.Parameters.AddWithValue(Truong, arrGiaTri[j]);
                                        }
                                    }
                                }
                            }
                            bang.Save();
                        }
                    }
                }
            }
            string idAction = Request.Form["idAction"];
            string sLyDo = Request.Form["sLyDo"];
            if (idAction == "1")
            {
                return RedirectToAction("TuChoi", "CapPhat_ChungTu", new { ChiNganSach = ChiNganSach, iID_MaCapPhat = iID_MaCapPhat, DonVi = DonVi, sLyDo = sLyDo });
            }
            else if (idAction == "2")
            {
                return RedirectToAction("TrinhDuyet", "CapPhat_ChungTu", new { ChiNganSach = ChiNganSach, iID_MaCapPhat = iID_MaCapPhat, DonVi = DonVi, sLyDo = sLyDo });
            }
            return RedirectToAction("CapPhatChiTiet_Frame", new { sLNS = arrGiaTriTimKiem["sLNS"], sL = arrGiaTriTimKiem["sL"], sK = arrGiaTriTimKiem["sK"], sM = arrGiaTriTimKiem["sM"], sTM = arrGiaTriTimKiem["sTM"], sTTM = arrGiaTriTimKiem["sTTM"], sNG = arrGiaTriTimKiem["sNG"], sTNG = arrGiaTriTimKiem["sTNG"], iID_MaDonVi = arrGiaTriTimKiem["iID_MaDonVi"], iID_MaCapPhat = iID_MaCapPhat, DonVi = DonVi });
        }
        /// <summary>
        /// get_GiaTri
        /// </summary>
        /// <param name="Truong"></param>
        /// <param name="GiaTri"></param>
        /// <param name="DSGiaTri"></param>
        /// <returns></returns>
        #region Lấy 1 hang AJAX: rTongSoNamTruoc
        [Authorize]
        public JsonResult get_GiaTri(string Truong, string GiaTri, string DSGiaTri)
        {
            if (Truong == "PhanBo_DaCapPhat")
            {
                return LayGiaTriDaCapPhat(GiaTri, DSGiaTri);
            }
            return null;
        }
        /// <summary>
        /// Hàm lấy giá trị đã cấp phát của chứng từ chi tiết
        /// </summary>
        /// <param name="GiaTri"></param>
        /// <param name="DSGiaTri"></param>
        /// <returns></returns>
        private JsonResult LayGiaTriDaCapPhat(string GiaTri, string DSGiaTri)
        {
            string iID_MaCapPhat = GiaTri;
            string[] arrDSGiaTri = DSGiaTri.Split(',');
            string iID_MaDonVi = arrDSGiaTri[0];
            string iID_MaMucLucNganSach = arrDSGiaTri[1];

            NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            Object item = new
            {
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
