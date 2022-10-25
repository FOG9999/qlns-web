﻿using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel.Controls;
using DomainModel;
using System.Collections;
using System.Collections.Specialized;
using VIETTEL.Models;
using System.Text;

namespace VIETTEL.Controllers.SanPham
{
    public class SanPhamController : Controller
    {
        //
        // GET: /SanPham/
        public string sViewPath = "~/Views/SanPham/SanPham/";
        /// <summary>
        /// Điều hướng về form danh sách
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(String ParentID)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "DM_SanPham", "List") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            ViewData["sTen"] = Request.Form[ParentID + "_sTen"];
            ViewData["sMa"] = Request.Form[ParentID + "_sMa"];
            return View(sViewPath + "Index.aspx");
        }
        /// <summary>
        /// Action Thêm mới + Sửa
        /// </summary>
        /// <param name="MaHangMau"></param>
        /// <param name="MaHangMauCha"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(String iID_MaSanPham)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, "DM_SanPham", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            ViewData["iID_MaSanPham"] = iID_MaSanPham;
            return View(sViewPath + "Edit.aspx");
        }
        /// <summary>
        /// Lưu sản phẩm
        /// </summary>
        /// <param name="iID_MaMucLucQuanSo_Cha"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(String ParentID, String iID_MaSanPham)
        {
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, " DM_SanPham", "Edit") == false)
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
            NameValueCollection arrLoi = new NameValueCollection();
            String sMa = Convert.ToString(Request.Form[ParentID + "_sMa"]);
            String sTen = Convert.ToString(Request.Form[ParentID + "_sTen"]);
            String iID_MaSanPham_copy = Convert.ToString(Request.Form[ParentID + "_iID_MaSanPham_copy"]);
            if (String.IsNullOrEmpty(sTen))
            {
                arrLoi.Add("err_sTen", "Bạn chưa nhập tên sản phẩm!");
            }
            if (String.IsNullOrEmpty(sMa))
            {
                arrLoi.Add("err_sMa", "Bạn chưa nhập mã sản phẩm!");
            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT COUNT(iID_MaSanPham) AS value FROM DM_SanPham WHERE sMa=@sMa AND iTrangThai=1";
                cmd.Parameters.AddWithValue("@sMa", sMa);
                int Dem = Convert.ToInt32(Connection.GetValue(cmd, -1));
                if (Dem > 0) arrLoi.Add("err_sMa", "Mã sản phẩm đã tồn tại!");
            }
            if (arrLoi.Count > 0)
            {
                for (int i = 0; i <= arrLoi.Count - 1; i++)
                {
                    ModelState.AddModelError(ParentID + "_" + arrLoi.GetKey(i), arrLoi[i]);
                }
                ViewData["isError"] = "1";
                ViewData["sTen"] = Convert.ToString(Request.Form[ParentID + "_Ten"]);
                ViewData["sMa"] = Convert.ToString(Request.Form[ParentID + "_sMa"]);
                return View(sViewPath + "Edit.aspx");
            }
            Bang bang = new Bang("DM_SanPham");
            bang.MaNguoiDungSua = User.Identity.Name;
            bang.IPSua = Request.UserHostAddress;
            bang.TruyenGiaTri(ParentID, Request.Form);
            if (String.IsNullOrEmpty(iID_MaSanPham))
            {
                if (iID_MaSanPham_copy != Guid.Empty.ToString())
                {
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaSanPhamCopy", iID_MaSanPham_copy);
                }
                bang.DuLieuMoi = true;
            }
            else
            {
                bang.DuLieuMoi = false;
                bang.GiaTriKhoa = iID_MaSanPham;
            }
            String iID_MaSanPham_moi = Convert.ToString(bang.Save());
            if (String.IsNullOrEmpty(iID_MaSanPham))
            {// tao moi san pham thi lay het mau cau hinh danh muc sang
                for (int iID_MaLoaiHinh = 1; iID_MaLoaiHinh <= 3; iID_MaLoaiHinh++)
                {
                    int ThuTu = 0;
                    CopyDanhMucTheoMau(Convert.ToString(bang.GiaTriKhoa), Convert.ToString(iID_MaLoaiHinh), "", "0", User.Identity.Name, Request.UserHostAddress, 0, ref ThuTu);
                }
            }
            String[] arrCot = @"rSoLuong_DangThucHien,rDonGia_DangThucHien,rTien_DangThucHien,rSoLuong_DV_DeNghi,rDonGia_DV_DeNghi,rTien_DV_DeNghi,rSoLuong_DatHang_DeNghi,rDonGia_DatHang_DeNghi,rTien_DatHang_DeNghi,rSoLuong_CTC_DeNghi,rDonGia_CTC_DeNghi,rTien_CTC_DeNghi,rSoSanh_DV_DeNghi,rSoSanh_DatHang_DeNghi,rSoSanh_CTC_DeNghi,rSoSanh".Split(',');
            //copy dữ liệu
            if (iID_MaSanPham_copy != Guid.Empty.ToString())
            {
                String SQL = String.Format(@"SELECT * FROM
                                             DM_SanPham_DanhMucGia 
                                            WHERE iTrangThai=1 AND iID_MaSanPham=@iID_MaSanPham_copy AND iID_MaLoaiHinh=1 AND sKyHieu is null");
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@iID_MaSanPham_copy", iID_MaSanPham_copy);
                DataTable dtSanPhamCopy = Connection.GetDataTable(cmd);
                cmd.Dispose();
                for (int i = 0; i < dtSanPhamCopy.Rows.Count; i++)
                {
                    Bang bangCopy = new Bang("DM_SanPham_DanhMucGia");
                    bangCopy.CmdParams.Parameters.AddWithValue("@iID_MaDanhMucGia_Cha", dtSanPhamCopy.Rows[i]["iID_MaDanhMucGia_Cha"]);
                    bangCopy.CmdParams.Parameters.AddWithValue("@iID_MaSanPham", iID_MaSanPham_moi);
                    bangCopy.CmdParams.Parameters.AddWithValue("@iID_MaLoaiHinh", dtSanPhamCopy.Rows[i]["iID_MaLoaiHinh"]);
                    bangCopy.CmdParams.Parameters.AddWithValue("@iDM_MaDonViTinh", dtSanPhamCopy.Rows[i]["iDM_MaDonViTinh"]);
                    bangCopy.CmdParams.Parameters.AddWithValue("@sTen", dtSanPhamCopy.Rows[i]["sTen"]);
                    bangCopy.CmdParams.Parameters.AddWithValue("@iID_MaVatTu", dtSanPhamCopy.Rows[i]["iID_MaVatTu"]);
                    bangCopy.CmdParams.Parameters.AddWithValue("@sTen_DonVi", dtSanPhamCopy.Rows[i]["sTen_DonVi"]);
                    bangCopy.MaNguoiDungSua = User.Identity.Name;
                    bangCopy.IPSua = Request.UserHostAddress;
                    bangCopy.Save();
                }
                int MaKyHieu11 = SanPham_VatTuModels.GetMa(iID_MaSanPham_moi, "1.1");
                int MaKyHieu12 = SanPham_VatTuModels.GetMa(iID_MaSanPham_moi, "1.2");

                int MaKyHieuCha11 = SanPham_VatTuModels.GetMa(iID_MaSanPham_copy, "1.1");
                int MaKyHieuCha12 = SanPham_VatTuModels.GetMa(iID_MaSanPham_copy, "1.2");

                SQL = String.Format(@"UPDATE DM_SanPham_DanhMucGia SET iID_MaDanhMucGia_Cha=@MaKyHieu11 WHERE iID_MaDanhMucGia_Cha=@MaKyHieuCha11 AND iID_MaSanPham=@iID_MaSanPham");
                cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue(@"MaKyHieu11", MaKyHieu11);
                cmd.Parameters.AddWithValue(@"MaKyHieuCha11", MaKyHieuCha11);
                cmd.Parameters.AddWithValue(@"iID_MaSanPham", iID_MaSanPham_moi);
                Connection.UpdateDatabase(cmd);


                SQL = String.Format(@"UPDATE DM_SanPham_DanhMucGia SET iID_MaDanhMucGia_Cha=@MaKyHieu11 WHERE iID_MaDanhMucGia_Cha=@MaKyHieuCha11 AND iID_MaSanPham=@iID_MaSanPham");
                cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue(@"MaKyHieu11", MaKyHieu12);
                cmd.Parameters.AddWithValue(@"MaKyHieuCha11", MaKyHieuCha12);
                cmd.Parameters.AddWithValue(@"iID_MaSanPham", iID_MaSanPham_moi);
                Connection.UpdateDatabase(cmd);

                //for (int i = 0; i < dtSanPhamCopy.Rows.Count; i++)
                //{
                //    for (int j = 0; j < dtSanPhamMoi.Rows.Count; j++)
                //    {

                //        if (dtSanPhamCopy.Rows[i]["iID_MaLoaiHinh"].ToString() == dtSanPhamMoi.Rows[j]["iID_MaLoaiHinh"].ToString()
                //            && dtSanPhamCopy.Rows[i]["sKyHieu"].ToString() == dtSanPhamMoi.Rows[j]["sKyHieu"].ToString())
                //        {
                //            for (int c = 0; c < arrCot.Length; c++)
                //            {

                //                dtSanPhamMoi.Rows[j][arrCot[c]] = dtSanPhamCopy.Rows[i][arrCot[c]];
                //            }
                //        }
                //    }
                //}
                //update db
                //for (int i = 0; i < dtSanPhamMoi.Rows.Count; i++)
                //{
                //    bang = new Bang("DM_SanPham_DanhMucGia");
                //    bang.MaNguoiDungSua = User.Identity.Name;
                //    bang.IPSua = Request.UserHostAddress;
                //    bang.DuLieuMoi = false;
                //    bang.GiaTriKhoa = dtSanPhamMoi.Rows[i]["iID_MaDanhMucGia"];
                //    for (int c = 0; c < arrCot.Length; c++)
                //    {
                //        bang.CmdParams.Parameters.AddWithValue("@" + arrCot[c], dtSanPhamMoi.Rows[i][arrCot[c]]);
                //    }
                //   // bang.CmdParams.Parameters.AddWithValue("@iID_MaChiTietGia", iID_MaChiTiet);
                //    bang.Save();

                //}

            }
            return RedirectToAction("Index", new { iID_MaSanPham = bang.GiaTriKhoa });
        }
        public ActionResult Delete(String iID_MaSanPham)
        {
            Bang bang = new Bang("DM_SanPham");
            bang.GiaTriKhoa = iID_MaSanPham;
            bang.Delete();
            return RedirectToAction("Index", new { });
        }
        private void CopyDanhMucTheoMau(String iID_MaSanPham, String iID_MaLoaiHinh, String MaDanhMucCha, String iID_MaDanhMuc_Cha_Moi, String MaNguoiDungSua, String IPSua, int Cap, ref int ThuTu)
        {
            String SQL = "";
            ArrayList ListChiTiet = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            SQL = "SELECT * FROM DC_DanhMuc WHERE bHoatDong=1 AND iTrangThai=1";
            if (String.IsNullOrEmpty(MaDanhMucCha))
                SQL += " AND iID_MaDanhMucCha IS NULL ";
            else
                SQL += " AND iID_MaDanhMucCha = '" + MaDanhMucCha + "'";
            SQL += " AND iID_MaLoaiDanhMuc = (SELECT iID_MaLoaiDanhMuc FROM DC_LoaiDanhMuc WHERE sTenBang = 'MauDanhMucGiaSP')";
            SQL += " ORDER BY iSTT";
            cmd.CommandText = SQL;
            DataTable dt = Connection.GetDataTable(SQL);
            cmd.Dispose();
            if (dt.Rows.Count > 0)
            {
                int i, tgThuTu;
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    // int STT = i + 1;
                    ThuTu++;
                    tgThuTu = ThuTu;
                    DataRow Row = dt.Rows[i];
                    Bang bangDM = new Bang("DM_SanPham_DanhMucGia");
                    bangDM.MaNguoiDungSua = MaNguoiDungSua;
                    bangDM.IPSua = IPSua;
                    bangDM.DuLieuMoi = true;
                    bangDM.CmdParams.Parameters.AddWithValue("@iID_MaSanPham", iID_MaSanPham);
                    bangDM.CmdParams.Parameters.AddWithValue("@iID_MaLoaiHinh", iID_MaLoaiHinh);
                    bangDM.CmdParams.Parameters.AddWithValue("@iID_MaChiTietGia", 0);
                    bangDM.CmdParams.Parameters.AddWithValue("@iSTT", tgThuTu);
                    bangDM.CmdParams.Parameters.AddWithValue("@iID_MaDanhMucGia_Cha", iID_MaDanhMuc_Cha_Moi);
                    bangDM.CmdParams.Parameters.AddWithValue("@iID_MaVatTu", "dddddddd-dddd-dddd-dddd-dddddddddddd");
                    bangDM.CmdParams.Parameters.AddWithValue("@iDM_MaDonViTinh", "dddddddd-dddd-dddd-dddd-dddddddddddd");
                    bangDM.CmdParams.Parameters.AddWithValue("@bLaHangCha", 1);
                    bangDM.CmdParams.Parameters.AddWithValue("@bTheoMau", 1);
                    bangDM.CmdParams.Parameters.AddWithValue("@iTrangThai", 1);
                    bangDM.CmdParams.Parameters.AddWithValue("@sTen", Row["sTen"]);
                    bangDM.CmdParams.Parameters.AddWithValue("@sKyHieu", Row["sTenKhoa"]);
                    bangDM.Save();
                    CopyDanhMucTheoMau(iID_MaSanPham, iID_MaLoaiHinh, Convert.ToString(Row["iID_MaDanhMuc"]), SanPham_DanhMucGiaModels.Get_MaxId_DanhMucGia(), MaNguoiDungSua, IPSua, Cap + 1, ref ThuTu);
                }
            }
            dt.Dispose();
        }
    }
}
