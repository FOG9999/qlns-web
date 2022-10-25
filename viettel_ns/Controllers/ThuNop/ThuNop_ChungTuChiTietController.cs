using DomainModel;
using DomainModel.Abstract;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Web.Mvc;
using VIETTEL.Models;
using VIETTEL.Models.ThuNop;

namespace VIETTEL.Controllers.ThuNop
{
    public class ThuNop_ChungTuChiTietController : Controller
    {
        //
        // GET: /ThuNop_ChungTuChiTiet/
        public string sViewPath = "~/Views/ThuNop/ChungTuChiTiet/";
        [Authorize]
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                HamRiengModels.UserDefault(User.Identity.Name);
                return View(sViewPath + "ThuNop_ChungTuChiTiet_Index.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        [Authorize]
        public ActionResult ThuNopChiTiet_Frame(String iID_MaChungTu, String iLoai, String sLNS)
        {
            return View(sViewPath + "ThuNopChiTiet_Index_DanhSach_Frame.aspx", new { iID_MaChungTu = iID_MaChungTu, iLoai = iLoai, sLNS = sLNS });
        }

        /// <summary>
        /// Luu tru vao CSDL
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="iLoai"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DetailSubmit(String iID_MaChungTu, String iLoai, String sLNS)
        {
            NameValueCollection data = ThuNop_ChungTuModels.LayThongTin(iID_MaChungTu);
            String TenBangChiTiet = "TN_ChungTuChiTiet";

            String MaND = User.Identity.Name;
            DataTable dtPhongBan = NganSach_HamChungModels.DSBQLCuaNguoiDung(MaND);
            String strTenPhongBan = "", iID_MaPhongBan = "";
            if (dtPhongBan != null && dtPhongBan.Rows.Count > 0)
            {
                DataRow drPhongBan = dtPhongBan.Rows[0];
                iID_MaPhongBan = Convert.ToString(drPhongBan["sKyHieu"]);
                strTenPhongBan = Convert.ToString(drPhongBan["sTen"]);
                dtPhongBan.Dispose();
            }
            string idXauMaCacHang = Request.Form["idXauMaCacHang"];
            string idXauMaCacCot = Request.Form["idXauMaCacCot"];
            string idXauGiaTriChiTiet = Request.Form["idXauGiaTriChiTiet"];
            string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];
            string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
            string idMaMucLucNganSach = Request.Form["idMaMucLucNganSach"];

            String[] arrMaHang = idXauMaCacHang.Split(',');
            String[] arrMaCot = idXauMaCacCot.Split(',');
            String[] arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] { CapPhat_BangDuLieu.DauCachHang },
                                                               StringSplitOptions.None);
            String[] arrHangDaXoa = idXauCacHangDaXoa.Split(',');
            String[] arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] { BangDuLieu.DauCachHang },
                                                               StringSplitOptions.None);
            String[] arrMaMucLucNganSach = idMaMucLucNganSach.Split(',');
            String iID_MaCapPhatChiTiet, sKyHieu, sMoTa, iID_MaLoaiHinh, iID_MaLoaiHinh_Cha;

            //Luu cac hang sua

            for (int i = 0; i < arrMaHang.Length; i++)
            {
                if (arrMaHang[i] != "")
                {
                    iID_MaCapPhatChiTiet = arrMaHang[i].Split('_')[0];
                    sKyHieu = arrMaHang[i].Split('_')[1];
                    iID_MaLoaiHinh = arrMaHang[i].Split('_')[2];
                    iID_MaLoaiHinh_Cha = arrMaHang[i].Split('_')[3];
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
                        String[] arrGiaTri = arrHangGiaTri[i].Split(new string[] { BangDuLieu.DauCachO },
                                                                    StringSplitOptions.None);
                        String[] arrThayDoi = arrHangThayDoi[i].Split(
                            new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                        Boolean okCoThayDoi = false;
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

                            if (iID_MaCapPhatChiTiet == "")
                            {
                                //Du Lieu Moi
                                bang.DuLieuMoi = true;
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);

                                //Them cac tham so tu bang chung tu

                                bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                                bang.CmdParams.Parameters.AddWithValue("@sTenPhongBan", strTenPhongBan);
                                bang.CmdParams.Parameters.AddWithValue("@iThang_Quy", data["iThang_Quy"]);
                                bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaNguonNganSach",
                                                                       data["iID_MaNguonNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", data["bChiNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaTrangThaiDuyet",
                                                                       data["iID_MaTrangThaiDuyet"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", data["iID_MaDonVi"]);
                                bang.CmdParams.Parameters.AddWithValue("@sTenDonVi", DonViModels.Get_TenDonVi(data["iID_MaDonVi"], MaND));
                                //bang.CmdParams.Parameters.AddWithValue("@dNgayChungTu", DateTime.Parse(data["dNgayChungTu"]));
                                bang.CmdParams.Parameters.AddWithValue("@dNgayChungTu", DateTime.Parse(data["dNgayChungTu"]));

                                bang.CmdParams.Parameters.AddWithValue("@iLoai", iLoai);
                                bang.CmdParams.Parameters.AddWithValue("@sKyHieu", sKyHieu);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaLoaiHinh", iID_MaLoaiHinh);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaLoaiHinh_Cha", iID_MaLoaiHinh_Cha);
                                String SQL = String.Format(@"SELECT TOP 1  sTEN
FROM TN_DanhMucLoaiHinh
WHERE skyHieu={0} AND iTrangThai=1", sKyHieu); ;
                                sMoTa = Connection.GetValueString(SQL, "");
                                bang.CmdParams.Parameters.AddWithValue("@sMoTa", sMoTa);
                            }
                            else
                            {
                                //Du Lieu Da Co
                                bang.GiaTriKhoa = iID_MaCapPhatChiTiet;
                                bang.DuLieuMoi = false;
                            }
                            bang.MaNguoiDungSua = User.Identity.Name;
                            bang.IPSua = Request.UserHostAddress;

                            //Them tham so

                            for (int j = 0; j < arrMaCot.Length; j++)
                            {
                                if (arrThayDoi[j] == "1")
                                {

                                    String Truong = "@" + arrMaCot[j];

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
                                            bang.CmdParams.Parameters.AddWithValue(Truong,
                                                                                   Convert.ToDouble(arrGiaTri[j]));
                                        }
                                    }
                                    else
                                    {
                                        //Nhap kieu xau
                                        bang.CmdParams.Parameters.AddWithValue(Truong, arrGiaTri[j]);
                                    }

                                }
                            }
                            bang.Save();
                        }
                    }
                }
            }

            string idAction = Request.Form["idAction"];
            if (idAction == "1")
            {
                return RedirectToAction("TuChoi", "ThuNop_ChungTu", new { iID_MaChungTu = iID_MaChungTu, iLoai = iLoai, sLNS = sLNS });
            }
            else if (idAction == "2")
            {
                return RedirectToAction("TrinhDuyet", "ThuNop_ChungTu", new { iID_MaChungTu = iID_MaChungTu, iLoai = iLoai, sLNS = sLNS });
            }
            return RedirectToAction("ThuNopChiTiet_Frame", new { iID_MaChungTu = iID_MaChungTu, iLoai = iLoai, sLNS = sLNS });


        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SearchSubmit(String ParentID, String iID_MaChungTu)
        {

            String sKyHieu = Request.Form[ParentID + "_sKyHieu"];

            return RedirectToAction("Index", new { iID_MaChungTu = iID_MaChungTu, sKyHieu = sKyHieu });
        }
        [Authorize]
        public JsonResult get_GiaTri(String Truong, String GiaTri, String DSGiaTri, String iLoai)
        {
            if (iLoai == "1")
            {
                if (Truong == "PhanBo_DaCapPhat")
                {
                    return get_PhanBo_DaCapPhat(GiaTri, DSGiaTri);
                }
            }
            return null;
        }

        private JsonResult get_PhanBo_DaCapPhat(String GiaTri, String DSGiaTri)
        {
            String iID_MaChungTu = GiaTri;
            String[] arrDSGiaTri = DSGiaTri.Split(',');
            String iID_MaDonVi = arrDSGiaTri[0];
            String iID_MaMucLucNganSach = arrDSGiaTri[1];
            if (!String.IsNullOrEmpty(iID_MaChungTu))
            {


                NameValueCollection data = ThuNop_ChungTuModels.LayThongTin(iID_MaChungTu);
                int iNamLamViec = Convert.ToInt32(data["iNamLamViec"]);
                int iID_MaNguonNganSach = Convert.ToInt32(data["iID_MaNguonNganSach"]);
                int iID_MaNamNganSach = Convert.ToInt32(data["iID_MaNamNganSach"]);
                Object obNgayCapPhat = data["dNgayChungTu"];

                Object item = new
                {
                    data =
                        ThuNop_ChungTuChiTietModels.LayGiaTri_LoaiHinh_DaCap(iID_MaMucLucNganSach,
                                                                            iID_MaDonVi, iNamLamViec,
                                                                            iID_MaNguonNganSach,
                                                                            iID_MaNamNganSach)
                };

                return Json(item, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
