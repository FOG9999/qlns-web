using System;
using System.Web.Mvc;
using DomainModel.Abstract;
using VIETTEL.Models;
using Viettel.Services;

namespace VIETTEL.Controllers.NguonNS
{
    public class Nguon_DMBQPController : Controller
    {        
        public string sViewPath = "~/Views/NguonNS/Nguon_DMBQP/";
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;

        [Authorize]
        public ActionResult Edit()
        {

            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                return View(sViewPath + "Nguon_DMBQP_Edit.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        public ActionResult Nguon_DMBQP_Index_Frame()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                return View(sViewPath + "Nguon_DMBQP_Index_Frame.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        [Authorize]
        public ActionResult Nguon_DMBQP_Edit_Frame(string phongban, string LuuThanhCong)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                return View(sViewPath + "Nguon_DMBQP_Edit_Frame.aspx", new { phongban = phongban, LuuThanhCong = LuuThanhCong });   
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DetailSubmit(string phongban)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                string TenBangChiTiet = "Nguon_DMBQP";
                string idXauMaCacHang = Request.Form["idXauMaCacHang"];
                string idXauMaCacCot = Request.Form["idXauMaCacCot"];
                string idXauGiaTriChiTiet = Request.Form["idXauGiaTriChiTiet"];
                string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];
                string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
                string[] arrMaHang = idXauMaCacHang.Split(',');
                string[] arrHangDaXoa = idXauCacHangDaXoa.Split(',');
                string[] arrMaCot = idXauMaCacCot.Split(',');
                string[] arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] {BangDuLieu.DauCachHang},
                                                                  StringSplitOptions.None);
                string Id, Id_Cha;
                //Luu cac hang sua
                string[] arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] {BangDuLieu.DauCachHang},
                                                                   StringSplitOptions.None);
                for (int i = 0; i < arrMaHang.Length; i++)
                {
                    Id = arrMaHang[i].Split('_')[0];
                    Id_Cha = arrMaHang[i].Split('_')[1];
                    if (arrHangDaXoa[i] == "1")
                    {
                        //Lưu các hàng đã xóa
                        if (Id != "")
                        {
                            //Dữ liệu đã có
                            Bang bang = new Bang(TenBangChiTiet);
                            bang.DuLieuMoi = false;
                            bang.GiaTriKhoa = Id;
                            bang.CmdParams.Parameters.AddWithValue("@TrangThai", 0);
                            bang.CmdParams.Parameters.AddWithValue("@NguoiSua", User.Identity.Name);
                            bang.CmdParams.Parameters.AddWithValue("@IpSua", Request.UserHostAddress);
                            if (!_nguonNSService.checkDuLieu(Id))
                            {
                                bang.Save();
                            }
                        }
                    }
                    else
                    {
                        string[] arrGiaTri = arrHangGiaTri[i].Split(new string[] {BangDuLieu.DauCachO},
                                                                    StringSplitOptions.None);
                        string[] arrThayDoi = arrHangThayDoi[i].Split(new string[] {BangDuLieu.DauCachO},
                                                                      StringSplitOptions.None);
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
                            Id = arrMaHang[i].Split('_')[0];
                            if (Id == "")
                            {
                                //Du Lieu Moi
                                bang.DuLieuMoi = true;
                                bang.CmdParams.Parameters.AddWithValue("@Nam", MucLucNganSachModels.LayNamLamViec(User.Identity.Name));
                                bang.CmdParams.Parameters.AddWithValue("@NguoiTao", User.Identity.Name);
                            }
                            else
                            {
                                //Du Lieu Da Co
                                bang.GiaTriKhoa = Id;
                                bang.CmdParams.Parameters.AddWithValue("@NguoiSua", User.Identity.Name);
                                bang.CmdParams.Parameters.AddWithValue("@IpSua", Request.UserHostAddress);
                                bang.DuLieuMoi = false;
                            }

                            for (int j = 0; j < arrMaCot.Length - 2; j++)
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
                                    else if (arrMaCot[j] == "Ma_NguonCha")
                                    {
                                        bang.CmdParams.Parameters.AddWithValue("@Id_Cha", _nguonNSService.getId("Nguon_DMBQP", arrGiaTri[j].ToString(), MucLucNganSachModels.LayNamLamViec(User.Identity.Name)));
                                    }
                                    else
                                    {
                                        //Nhap kieu xau
                                        bang.CmdParams.Parameters.AddWithValue(Truong, arrGiaTri[j]);
                                    }
                                }
                            }
                            bang.Save();
                            if (!bang.DuLieuMoi)
                            {
                                _nguonNSService.Update("Nguon_SLGOCChi", null, "Nguon_DMBQP", Id);
                            }
                        }
                    }
                }
                return RedirectToAction("Nguon_DMBQP_Edit_Frame", new { LuuThanhCong = "1" });

            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
    }
}
