using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DomainModel;
using DomainModel.Abstract;
using System.Data.SqlClient;
using VIETTEL.Models;


namespace VIETTEL.Controllers.DungChung
{
    public class DuToan_KhoaSoLieuController : Controller
    {
        //
        // GET: /DuToan_KhoaSoLieu/
        public string sViewPath = "~/Views/DuToan/DuToan_KhoaSoLieu/";
        public Bang bang = new Bang("DT_CauHinhBia");
        [Authorize]
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                String sGiaTriKhoa = DuToan_ChungTuModels.GetMaCauHinhBia(User.Identity.Name);
                bang.GiaTriKhoa = sGiaTriKhoa;
                Dictionary<string, object> dicData = bang.LayGoiDuLieu(null, true);
                ViewData["dicData"] = dicData;
                if (String.IsNullOrEmpty(sGiaTriKhoa))
                    ViewData["DuLieuMoi"] = "1";
                else
                    ViewData["DuLieuMoi"] = "0";
                return View(sViewPath + "DuToan_KhoaSoLieu_Index.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        [Authorize]
        public ActionResult Detail(String iID_MaPhongBan)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
                return View(sViewPath + "DuToan_KhoaSoLieu_Detail.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditSubmit(String ParentID)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                bang.MaNguoiDungSua = User.Identity.Name;
                bang.IPSua = Request.UserHostAddress;
                bang.TruyenGiaTri(ParentID, Request.Form);
                bang.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        [Authorize]
        public ActionResult Edit(String ParentID, String iID_MaPhongBan, String sMaNguoiDung, String iKhoa)
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                if (iKhoa == "0" || iKhoa == null || iKhoa == "") iKhoa = "1";
                else iKhoa = "0";

                //check co so lieu, neu ko co them moi
                String SQL = String.Format(@"SELECT COUNT(*) FROM DT_KhoaSoLieu WHERE iNamLamViec=@iNamLamViec AND iID_MaPhongBan=@iID_MaPhongBan AND sMaNguoiDung=@sMaNguoiDung");
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@sMaNguoiDung", sMaNguoiDung);
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(User.Identity.Name));
                int count = (int)Connection.GetValue(cmd, 0);
                //neu co so lieu
                if (count > 0)
                {
                    SQL = "UPDATE DT_KhoaSoLieu SET iKhoa=@iKhoa WHERE iNamLamViec=@iNamLamViec AND iID_MaPhongBan=@iID_MaPhongBan AND sMaNguoiDung=@sMaNguoiDung";
                }
                else
                {
                    SQL = "INSERT INTO DT_KhoaSoLieu (iID_MaPhongBan,sMaNguoiDung,iNamLamViec,iKhoa)  VALUES (@iID_MaPhongBan,@sMaNguoiDung,@iNamLamViec,@iKhoa)";
                }
                cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@iKhoa", iKhoa);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@sMaNguoiDung", sMaNguoiDung);
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(User.Identity.Name));
                Connection.UpdateDatabase(cmd);
                return RedirectToAction("detail", new { iID_MaPhongBan = iID_MaPhongBan });
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }



    }
}
