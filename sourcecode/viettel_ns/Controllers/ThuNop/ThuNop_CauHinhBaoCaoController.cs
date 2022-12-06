using System;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel;
using System.Collections.Specialized;
using VIETTEL.Models;
using VIETTEL.Models.ThuNop;

namespace VIETTEL.Controllers.ThuNop {
    public class ThuNop_CauHinhBaoCaoController : Controller
    {
        //
        // GET: /TN_CauHinhBaoCao/
        public string sViewPath = "~/Views/ThuNop/CauHinhBaoCao/";
        [Authorize]
        public ActionResult Index(String iLoai)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_CauHinhBaoCao", "List") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            ViewData["iLoai"] = iLoai;            
            return View(sViewPath + "ThuNop_CauHinhBaoCao_Index.aspx");
        }
        /// <summary>
        /// Thêm mục con
        /// </summary>
        /// <param name="iID_MaMucLucQuanSo_Cha"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create(String iID_MaBaoCao_Cha, String iLoai)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_CauHinhBaoCao", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            ViewData["iLoai"] = iLoai;
            return RedirectToAction("Edit", new { iID_MaBaoCao_Cha = iID_MaBaoCao_Cha });
        }
        /// <summary>
        /// Action sửa mục lục loại hình hoạt động có thu
        /// </summary>
        /// <param name="MaHangMau"></param>
        /// <param name="MaHangMauCha"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(String iID_MaBaoCao, String iID_MaBaoCao_Cha, String iLoai)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_CauHinhBaoCao", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }            
            ViewData["DuLieuMoi"] = "0";
            if (String.IsNullOrEmpty(iID_MaBaoCao))
            {
                ViewData["DuLieuMoi"] = "1";
            }
            ViewData["iLoai"] = iLoai;
            ViewData["iID_MaBaoCao"] = iID_MaBaoCao;
            ViewData["iID_MaBaoCao_Cha"] = iID_MaBaoCao_Cha;
            return View(sViewPath + "ThuNop_CauHinhBaoCao_Edit.aspx");
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(String ParentID, String iID_MaBaoCao, String iLoai)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_CauHinhBaoCao", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            
            NameValueCollection arrLoi = new NameValueCollection();
            String sTen = Convert.ToString(Request.Form[ParentID + "_sTen"]);
            String sTenVietTat = Convert.ToString(Request.Form[ParentID + "_sTenVietTat"]);
            String sLoaiNS = Convert.ToString(Request.Form["sLoaiNS"]);
            String sLoaiHinh = Convert.ToString(Request.Form["sLoaiHinh"]);            
            String iID_MaBaoCao_Cha = Convert.ToString(Request.Form[ParentID + "iID_MaBaoCao_Cha"]);
            String DuLieuMoi = Convert.ToString(Request.Form[ParentID + "_DuLieuMoi"]);
            if (sTen == string.Empty || sTen == "")
            {
                arrLoi.Add("err_sTen", MessageModels.sTen);
            }            
            if (arrLoi.Count > 0)
            {
                for (int i = 0; i <= arrLoi.Count - 1; i++)
                {
                    ModelState.AddModelError(ParentID + "_" + arrLoi.GetKey(i), arrLoi[i]);
                }
                ViewData["DuLieuMoi"] = "0";
                if (String.IsNullOrEmpty(iID_MaBaoCao))
                {
                    ViewData["DuLieuMoi"] = "1";
                }
                ViewData["iLoai"] = iLoai;
                ViewData["iID_MaBaoCao"] = iID_MaBaoCao;
                ViewData["iID_MaBaoCao_Cha"] = iID_MaBaoCao_Cha;
                return View(sViewPath + "ThuNop_CauHinhBaoCao_Edit.aspx");
            }
            else
            {
                Bang bang = new Bang("TN_CauHinhBaoCao");
                bang.MaNguoiDungSua = User.Identity.Name;
                bang.IPSua = Request.UserHostAddress;
                bang.TruyenGiaTri(ParentID, Request.Form);

                if (DuLieuMoi == "1")
                {
                    if (iID_MaBaoCao_Cha == null || iID_MaBaoCao_Cha == "")
                    {
                        int cs = bang.CmdParams.Parameters.IndexOf("@iID_MaBaoCao_Cha");
                        if (cs >= 0)
                        {
                            bang.CmdParams.Parameters.RemoveAt(cs);
                        }
                    }                    
                }
                if (DuLieuMoi == "0")
                {
                    if (iID_MaBaoCao_Cha == null || iID_MaBaoCao_Cha == "")
                    {
                        int cs = bang.CmdParams.Parameters.IndexOf("@iID_MaBaoCao_Cha");
                        if (cs >= 0)
                        {
                            bang.CmdParams.Parameters.RemoveAt(cs);
                        }
                    }
                    bang.GiaTriKhoa = iID_MaBaoCao;
                }
                bang.CmdParams.Parameters.AddWithValue("@sLoaiHinh", sLoaiHinh);
                bang.CmdParams.Parameters.AddWithValue("@sLoaiNS", sLoaiNS);
                bang.CmdParams.Parameters.AddWithValue("@iID_MaLoaiBaoCao", iLoai);
                bang.Save();
                return RedirectToAction("Index", new { iLoai = iLoai });
            }
        }
        /// <summary>
        /// Lệnh xoá loại hình hoạt động có thu
        /// </summary>
        /// <param name="iID_MaBaoCao">Mã loại hình</param>
        /// <param name="iID_MaBaoCao_Cha">Mã loại hình cấp cha</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(String iID_MaBaoCao, String iID_MaBaoCao_Cha)
        {
            //kiểm tra quyền có được phép xóa
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_CauHinhBaoCao", "Delete") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }

            Boolean bDelete = false;
            bDelete = ThuNop_CauHinhBaoCaoModels.Delete(iID_MaBaoCao);
            if (bDelete == true)
            {
                return RedirectToAction("Index", new { iID_MaBaoCao_Cha = iID_MaBaoCao_Cha });
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        /// <summary>
        /// Tìm kiếm loại tài khoản
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SearchSubmit(String ParentID)
        {
            String sTenBaoCao = Request.Form[ParentID + "_sTenBaoCao"];
            String sLoaiHinh = Request.Form[ParentID + "_sLoaiHinh"];
            return RedirectToAction("Index", "ThuNop_CauHinhBaoCao", new { ParentID = ParentID, sTenBaoCao = sTenBaoCao, KyHieu = sLoaiHinh });
        }

        public Boolean CheckMaTaiKhoan(String sLoaiHinh)
        {
            Boolean vR = false;
            int iSoDuLieu = ThuNop_CauHinhBaoCaoModels.Get_So_KyHieuBaoCao(sLoaiHinh);
            if (iSoDuLieu > 0)
            {
                vR = true;
            }
            return vR;
        }
    }
}
