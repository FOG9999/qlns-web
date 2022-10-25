using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Controllers.DanhMuc
{
    public class MucLucNganSachBDKTController : Controller
    {
        //
        // GET: /MucLucNganSach/
        public string sViewPath = "~/Views/DanhMuc/MucLucNganSachBDKT/";

        [Authorize]
        public ActionResult Edit(String MaMucLucNganSach)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                return View(sViewPath + "MucLucNganSachBDKT_Edit.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        [Authorize]
        public ActionResult MLNSBDKTChiTiet_Frame(String sLNS,String LuuThanhCong)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                return View(sViewPath + "MucLucNganSachBDKT_Edit_Frame.aspx", new { sLNS = sLNS, LuuThanhCong = LuuThanhCong });   
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }        

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DetailSubmit(String sLNS)
        {

            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                String TenBangChiTiet = "NS_MucLucNganSach";
                string idXauMaCacHang = Request.Form["idXauMaCacHang"];
                string idXauMaCacCot = Request.Form["idXauMaCacCot"];
                string idXauGiaTriChiTiet = Request.Form["idXauGiaTriChiTiet"];
                string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];
                string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
                String[] arrMaHang = idXauMaCacHang.Split(',');
                String[] arrHangDaXoa = idXauCacHangDaXoa.Split(',');
                String[] arrMaCot = idXauMaCacCot.Split(',');
                String[] arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] {BangDuLieu.DauCachHang},
                                                                  StringSplitOptions.None);

                String iID_MaChungTuChiTiet;
                //Luu cac hang sua
                String[] arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] {BangDuLieu.DauCachHang},
                                                                   StringSplitOptions.None);
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
                        String[] arrGiaTri = arrHangGiaTri[i].Split(new string[] {BangDuLieu.DauCachO},
                                                                    StringSplitOptions.None);
                        String[] arrThayDoi = arrHangThayDoi[i].Split(new string[] {BangDuLieu.DauCachO},
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
                            bang.CmdParams.Parameters.AddWithValue("@iNamLamViec",MucLucNganSachModels.LayNamLamViec(User.Identity.Name));
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
                                    if (arrMaCot[j].StartsWith("sMoTa"))
                                    {
                                        if (!String.IsNullOrEmpty(iID_MaChungTuChiTiet))
                                        {
                                            //update lai ten muc luc ngan sach
                                            String SQL = String.Format(@"UPDATE DT_ChungTuChiTiet SET sMoTa=@sMoTa WHERE 
                                            iID_MaMucLucNganSach =(SELECT TOP 1 iID_MaMucLucNganSach FROM NS_MucLucNganSach WHERE iTrangThai=1 AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach)

                                            UPDATE DT_ChungTuChiTiet_PhanCap SET sMoTa=@sMoTa WHERE 
                                            iID_MaMucLucNganSach =(SELECT TOP 1 iID_MaMucLucNganSach FROM NS_MucLucNganSach WHERE iTrangThai=1 AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach)

                                            UPDATE DTBS_ChungTuChiTiet SET sMoTa=@sMoTa WHERE 
                                            iID_MaMucLucNganSach =(SELECT TOP 1 iID_MaMucLucNganSach FROM NS_MucLucNganSach WHERE iTrangThai=1 AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach)

                                            UPDATE DTBS_ChungTuChiTiet_PhanCap SET sMoTa=@sMoTa WHERE 
                                            iID_MaMucLucNganSach =(SELECT TOP 1 iID_MaMucLucNganSach FROM NS_MucLucNganSach WHERE iTrangThai=1 AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach)

                                            UPDATE QTA_ChungTuChiTiet SET sMoTa=@sMoTa WHERE 
                                            iID_MaMucLucNganSach =(SELECT TOP 1 iID_MaMucLucNganSach FROM NS_MucLucNganSach WHERE iTrangThai=1 AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach)
");
                                            SqlCommand cmd = new SqlCommand(SQL);
                                            cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaChungTuChiTiet);
                                            cmd.Parameters.AddWithValue("@sMoTa", arrGiaTri[j]);
                                            Connection.UpdateDatabase(cmd);
                                        }
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

                return RedirectToAction("MLNSBDKTChiTiet_Frame", new { sLNS = sLNS, LuuThanhCong = "1" });

            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
    }
}
