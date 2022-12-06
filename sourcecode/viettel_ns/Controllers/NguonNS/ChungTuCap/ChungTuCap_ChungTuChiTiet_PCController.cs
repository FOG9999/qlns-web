using DomainModel;
using DomainModel.Abstract;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Controllers.NguonNS
{
    public class ChungTuCap_ChungTuChiTiet_PCController : AppController
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;

        public string sViewPath = "~/Views/NguonNS/ChungTuCap/ChungTuChiTietPhanCap/";

        [Authorize]
        public ActionResult Index(string Id_CTCap, string Id_NguonBTC)
        {
            ViewData["Id_CTCap"] = Id_CTCap;
            ViewData["Id_NguonBTC"] = Id_NguonBTC;
            return View(sViewPath + "ChungTuChiTiet_Index.aspx");
        }
        [Authorize]
        public ActionResult ChungTuChiTiet_Frame(string Ma_Nguon)
        {
            string Id_CTCap = Request.Form["ChungTuCap_ChungTuChiTiet_Id_CTCap"];
            string Id_NguonBTC = Request.Form["ChungTuCap_ChungTuChiTiet_Id_NguonBTC"];

            ViewData["Id_CTCap"] = Id_CTCap;
            ViewData["Id_NguonBTC"] = Id_NguonBTC;
            ViewData["sL"] = Ma_Nguon;
            return View(sViewPath + "ChungTuChiTiet_Index_DanhSach_Frame.aspx");
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DetailSubmit(string Id_CTCap, string Id_NguonBTC)
        {
            var data = _nguonNSService.GetChungTu("Nguon_CTChi", Id_CTCap);

            string MaND = User.Identity.Name;
            string IPSua = Request.UserHostAddress;

            string TenBangChiTiet = "Nguon_CTChi_ChiTiet";
            string TenBangSLGOC = "Nguon_SLGOCChi";

            string idXauMaCacHang = Request.Form["idXauMaCacHang"];
            string idXauMaCacCot = Request.Form["idXauMaCacCot"];
            string idXauGiaTriChiTiet = Request.Form["idXauGiaTriChiTiet"];
            string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];
            string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
            string idMaMucLucNganSach = Request.Form["idMaMucLucNganSach"];
            int idNC_Fixed = Convert.ToInt32(Request.Form["idNC_Fixed"]);
            int idNC_Slide = Convert.ToInt32(Request.Form["idNC_Slide"]);
            string[] arrDSTruong = "Ma_Nguon".Split(',');
            Dictionary<string, string> arrGiaTriTimKiem = new Dictionary<string, string>();
            for (int i = 0; i < arrDSTruong.Length; i++)
            {
                arrGiaTriTimKiem.Add(arrDSTruong[i], Request.Form[arrDSTruong[i]]);
            }

            string[] arrMaHang = idXauMaCacHang.Split(',');
            string[] arrHangDaXoa = idXauCacHangDaXoa.Split(',');
            string[] arrMaCot = idXauMaCacCot.Split(',');
            string[] arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] { BangDuLieu.DauCachHang }, StringSplitOptions.None);

            string Id, Id_BQP, SoTien;

            //Luu cac hang sua
            string[] arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] { BangDuLieu.DauCachHang }, StringSplitOptions.None);
            for (int i = 0; i < arrMaHang.Length; i++)
            {
                if (arrMaHang[i] != "")
                {
                    Id = arrMaHang[i].Split('_')[0];
                    Id_BQP = arrMaHang[i].Split('_')[1];
                    SoTien = arrMaHang[i].Split('_')[2];
                    if (arrHangDaXoa[i] == "1" || SoTien == "0")
                    {
                        //Lưu các hàng đã xóa
                        if (Id != "")
                        {
                            //Dữ liệu đã có
                            Bang bang = new Bang(TenBangChiTiet);
                            bang.DuLieuMoi = false;
                            bang.GiaTriKhoa = Id;
                            bang.CmdParams.Parameters.AddWithValue("@TrangThai", 0);
                            bang.CmdParams.Parameters.AddWithValue("@NguoiSua", MaND);
                            bang.CmdParams.Parameters.AddWithValue("@IpSua", IPSua);
                            bang.Save();

                            _nguonNSService.Delete_ChungTuChiTiet("Nguon_SLGOCChi", "Id_CTChi_ChiTiet", Id, MaND, IPSua);
                        }
                    }
                    else
                    {
                        string[] arrGiaTri = arrHangGiaTri[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                        string[] arrThayDoi = arrHangThayDoi[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                        bool okCoThayDoi = false;
                        for (int j = idNC_Fixed + idNC_Slide - 1; j >= idNC_Fixed; j--)
                        {
                            if (arrThayDoi[j] == "1")
                            {
                                okCoThayDoi = true;
                            }
                        }                        
                        if (okCoThayDoi)
                        {
                            Bang bang = new Bang(TenBangChiTiet);
                            if (Id == "")
                            {
                                bang.DuLieuMoi = true;
                                bang.CmdParams.Parameters.AddWithValue("@Id_CTChi", Id_CTCap);
                                bang.CmdParams.Parameters.AddWithValue("@Id_NguonBTC", Id_NguonBTC);
                                bang.CmdParams.Parameters.AddWithValue("@Id_ChiBQP", Id_BQP);
                                bang.CmdParams.Parameters.AddWithValue("@Nam", data["Nam"]);
                                bang.CmdParams.Parameters.AddWithValue("@NguoiTao", data["NguoiTao"]);
                            }
                            else
                            {
                                //Du Lieu Da Co
                                bang.GiaTriKhoa = Id;
                                bang.DuLieuMoi = false;
                                bang.CmdParams.Parameters.AddWithValue("@NguoiSua", MaND);
                                bang.CmdParams.Parameters.AddWithValue("@IpSua", IPSua);
                            }

                            //Them tham so
                            for (int j = 0; j < arrMaCot.Length; j++)
                            {
                                if (arrThayDoi[j] == "1")
                                {
                                    string Truong = "@" + arrMaCot[j];
                                    if (arrMaCot[j] == "SoTien")
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
                                        bang.CmdParams.Parameters.AddWithValue(Truong, string.IsNullOrEmpty(arrGiaTri[j]) ? "" : arrGiaTri[j]);
                                    }

                                }
                            }
                                                        
                            if (bang.DuLieuMoi)
                            {
                                if (arrGiaTri[5].ToString() != "" && arrGiaTri[6].ToString() != "")
                                {
                                    _nguonNSService.Insert(TenBangSLGOC, TenBangChiTiet, Convert.ToString(bang.Save()));
                                }                                
                            }
                            else if (!bang.DuLieuMoi)
                            {
                                var check = true;
                                if (arrThayDoi[5] == "1")
                                {
                                    if (arrGiaTri[5].ToString() == "")
                                        check = false;
                                    else
                                        check = true;
                                } 
                                if (arrThayDoi[6] =="1")
                                {
                                    if (arrGiaTri[6].ToString() == "")
                                        check = false;
                                    else
                                        check = true;
                                }
                                if (arrThayDoi[3] == "1")
                                {
                                    if (arrGiaTri[3].ToString() == "0")
                                        check = false;
                                    else
                                        check = true;
                                }
                                if (check) { 
                                    bang.Save();
                                    _nguonNSService.Update(TenBangSLGOC, null, TenBangChiTiet, Id);
                                }
                            }
                        }
                    }
                }
            }

            _nguonNSService.UpdateSotienChungTu("Nguon_CTChi", Id_CTCap);

            return RedirectToAction("ChungTuChiTiet_Frame", new { Id_CTCap = Id_CTCap, Id_NguonBTC = Id_NguonBTC, Ma_Nguon = arrGiaTriTimKiem["Ma_Nguon"] });
        }
    }
}
