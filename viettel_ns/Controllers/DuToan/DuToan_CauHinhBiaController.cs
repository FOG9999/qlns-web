using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DomainModel.Controls;
using DomainModel;
using DomainModel.Abstract;
using System.Data.SqlClient;
using System.Collections.Specialized;
using VIETTEL.Models;


namespace VIETTEL.Controllers.DungChung
{
    public class DuToan_CauHinhBiaController : Controller
    {
        //
        // GET: /DuToan_CauHinhBia/
        public string sViewPath = "~/Views/DuToan/DuToan_CauHinhBia/";
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
                return View(sViewPath + "DuToan_CauHinhBia_Index.aspx");
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
        public ActionResult ThemNamTruoc()
        {
            String sGiaTriKhoa = DuToan_ChungTuModels.GetMaCauHinhBia(User.Identity.Name);
            if (String.IsNullOrEmpty(sGiaTriKhoa))
            {
                String iNamLamViec = ReportModels.LayNamLamViec(User.Identity.Name);
                String SQL = String.Format(@"INSERT INTO DT_CauHinhBia(sTyGia,sSoQuyetDinh,sSoCVKHNS,sThuCanDoi,sThuQuanLy,sThuNSNN,sNganSachBaoDam,sLuong,sNghiepVu,
                    sDoanhNghiep,sXDCB,sNganSachKhac,sNhaNuocGiao,sKinhPhiKhac,sID_MaNguoiDungTao,iNamLamViec
                    )
                    SELECT  sTyGia,sSoQuyetDinh,sSoCVKHNS,sThuCanDoi,sThuQuanLy,sThuNSNN,sNganSachBaoDam,sLuong,sNghiepVu,
                    sDoanhNghiep,sXDCB,sNganSachKhac,sNhaNuocGiao,sKinhPhiKhac,'{2}','{0}'
                    FROM    DT_CauHinhBia
                    WHERE   iNamLamViec={1}", iNamLamViec, Convert.ToInt32(iNamLamViec) - 1, User.Identity.Name);
                SqlCommand cmd = new SqlCommand(SQL);
                Connection.UpdateDatabase(cmd);
                cmd.Dispose();
            }
            return RedirectToAction("Index");
        }


    }
}
