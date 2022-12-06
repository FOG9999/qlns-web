using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel;
using System.Collections.Specialized;
using VIETTEL.Models;
using VIETTEL.Models.ThuNop;

namespace VIETTEL.Controllers.ThuNop {
    public class ThuNop_LoaiHinhController : Controller
    {
        //
        // GET: /TN_DanhMucLoaiHinh/
        public string sViewPath = "~/Views/ThuNop/LoaiHinh/";
        [Authorize]
        public ActionResult Index()
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_DanhMucLoaiHinh", "List") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            return View(sViewPath + "ThuNop_LoaiHinh_Index.aspx");
        }
        /// <summary>
        /// Thêm mục con
        /// </summary>
        /// <param name="iID_MaMucLucQuanSo_Cha"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create(String iID_MaLoaiHinh_Cha)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_DanhMucLoaiHinh", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            return RedirectToAction("Edit", new { iID_MaLoaiHinh_Cha = iID_MaLoaiHinh_Cha });
        }
        /// <summary>
        /// Action sửa mục lục loại hình hoạt động có thu
        /// </summary>
        /// <param name="MaHangMau"></param>
        /// <param name="MaHangMauCha"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(String iID_MaLoaiHinh, String iID_MaLoaiHinh_Cha)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_DanhMucLoaiHinh", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            ViewData["DuLieuMoi"] = "0";
            if (String.IsNullOrEmpty(iID_MaLoaiHinh))
            {
                ViewData["DuLieuMoi"] = "1";
            }
            ViewData["iID_MaLoaiHinh"] = iID_MaLoaiHinh;
            ViewData["iID_MaLoaiHinh_Cha"] = iID_MaLoaiHinh_Cha;
            return View(sViewPath + "ThuNop_LoaiHinh_Edit.aspx");
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(String ParentID, String iID_MaLoaiHinh)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_DanhMucLoaiHinh", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            SqlCommand cmd;
            NameValueCollection arrLoi = new NameValueCollection();
            String sTen = Convert.ToString(Request.Form[ParentID + "_sTen"]);
            String iID_MaLoaiHinh_Cha = Convert.ToString(Request.Form[ParentID + "_iID_MaLoaiHinh_Cha"]);
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
                if (String.IsNullOrEmpty(iID_MaLoaiHinh))
                {
                    ViewData["DuLieuMoi"] = "1";
                }
                ViewData["iID_MaLoaiHinh"] = iID_MaLoaiHinh;
                ViewData["iID_MaLoaiHinh_Cha"] = iID_MaLoaiHinh_Cha;
                return View(sViewPath + "ThuNop_LoaiHinh_Edit.aspx");
            }
            else
            {
                Bang bang = new Bang("TN_DanhMucLoaiHinh");
                bang.MaNguoiDungSua = User.Identity.Name;
                bang.IPSua = Request.UserHostAddress;
                bang.TruyenGiaTri(ParentID, Request.Form);

                if (DuLieuMoi == "1")
                {
                    if (iID_MaLoaiHinh_Cha == null || iID_MaLoaiHinh_Cha == "")
                    {
                        int cs = bang.CmdParams.Parameters.IndexOf("@iID_MaLoaiHinh_Cha");
                        if (cs >= 0)
                        {
                            bang.CmdParams.Parameters.RemoveAt(cs);
                        }
                    }
                    string SQL = "SELECT  MAX(iSTT) AS  iSTT FROM TN_DanhMucLoaiHinh WHERE 1=1";
                    cmd = new SqlCommand();
                    if (iID_MaLoaiHinh_Cha != null && iID_MaLoaiHinh_Cha != "")
                    {
                        SQL += " AND iID_MaLoaiHinh_Cha=@iID_MaLoaiHinh_Cha";
                        cmd.Parameters.AddWithValue("@iID_MaLoaiHinh_Cha", iID_MaLoaiHinh_Cha);
                    }
                    cmd.CommandText = SQL;
                    int SoHangMauCon = Convert.ToInt32(Connection.GetValue(cmd, 0));
                    cmd.Dispose();
                    bang.CmdParams.Parameters.AddWithValue("@iSTT", SoHangMauCon);
                }
                if (DuLieuMoi == "0")
                {
                    if (iID_MaLoaiHinh_Cha == null || iID_MaLoaiHinh_Cha == "")
                    {
                        int cs = bang.CmdParams.Parameters.IndexOf("@iID_MaLoaiHinh_Cha");
                        if (cs >= 0)
                        {
                            bang.CmdParams.Parameters.RemoveAt(cs);
                        }
                    }
                    bang.GiaTriKhoa = iID_MaLoaiHinh;
                }
                bang.Save();
                return RedirectToAction("Index", new { iID_MaLoaiHinh_Cha = iID_MaLoaiHinh_Cha });
            }
        }
        /// <summary>
        /// Hiển thị form sắp xếp tài khoản
        /// </summary>
        /// <param name="iID_MaLoaiHinh_Cha"></param>
        /// <param name="iID_MaLoaiHinh"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Sort(String iID_MaLoaiHinh_Cha, String iID_MaLoaiHinh)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_DanhMucLoaiHinh", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            if (String.IsNullOrEmpty(iID_MaLoaiHinh_Cha))
            {
                iID_MaLoaiHinh_Cha = "";
            }
            ViewData["iID_MaLoaiHinh_Cha"] = iID_MaLoaiHinh_Cha;
            ViewData["iID_MaLoaiHinh"] = iID_MaLoaiHinh;
            return View(sViewPath + "ThuNop_LoaiHinh_Sort.aspx");
        }
        /// <summary>
        /// Sắp xếp tài khoản
        /// </summary>
        /// <param name="iID_MaLoaiHinh_Cha"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SortSubmit(String iID_MaLoaiHinh_Cha)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_DanhMucLoaiHinh", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            string strOrder = Request.Form["hiddenOrder"].ToString();
            String[] arrTG = strOrder.Split('$');
            int i;
            for (i = 0; i < arrTG.Length - 1; i++)
            {
                Bang bang = new Bang("TN_DanhMucLoaiHinh");
                bang.GiaTriKhoa = arrTG[i];
                bang.DuLieuMoi = false;
                bang.CmdParams.Parameters.AddWithValue("@iSTT", i);
                bang.Save();
            }
            return RedirectToAction("Index", new { iID_MaLoaiHinh_Cha = iID_MaLoaiHinh_Cha });
        }
        /// <summary>
        /// Lệnh xoá loại hình hoạt động có thu
        /// </summary>
        /// <param name="iID_MaLoaiHinh">Mã loại hình</param>
        /// <param name="iID_MaLoaiHinh_Cha">Mã loại hình cấp cha</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(String iID_MaLoaiHinh, String iID_MaLoaiHinh_Cha)
        {
            //kiểm tra quyền có được phép xóa
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "TN_DanhMucLoaiHinh", "Delete") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }

            Boolean bDelete = false;
            bDelete = ThuNop_LoaiHinhModels.Delete(iID_MaLoaiHinh);
            if (bDelete == true)
            {
                return RedirectToAction("Index", new { iID_MaLoaiHinh_Cha = iID_MaLoaiHinh_Cha });
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
            String sTenLoaiHinh = Request.Form[ParentID + "_sTenLoaiHinh"];
            String sKyHieu = Request.Form[ParentID + "_sKyHieu"];
            return RedirectToAction("Index", "ThuNop_LoaiHinh", new { ParentID = ParentID, sTenLoaiHinh = sTenLoaiHinh, KyHieu = sKyHieu });
        }

        public Boolean CheckMaTaiKhoan(String sKyHieu)
        {
            Boolean vR = false;
            int iSoDuLieu = ThuNop_LoaiHinhModels.Get_So_KyHieuLoaiHinh(sKyHieu);
            if (iSoDuLieu > 0)
            {
                vR = true;
            }
            return vR;
        }
    }
}
