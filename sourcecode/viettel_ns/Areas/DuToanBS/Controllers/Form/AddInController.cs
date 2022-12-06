using DomainModel;
using DomainModel.Abstract;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Models;
using VIETTEL.Models.DuToanBS;

namespace VIETTEL.Areas.DuToanBS.Controllers
{
    public class AddInController : AppController
    {
        #region Hằng Số
        public string _viewPath = "~/Areas/DuToanBS/Views/Form/Addin/";

        private readonly INganSachService _ngansachService = NganSachService.Default;

        private string _username;
        #endregion

        #region Index
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaDotNganSach"></param>
        /// <param name="sLNS1">LNS 1</param>
        /// <param name="iLoai">Loại xử lý</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11")
            {
                var view = _viewPath + "AddIn_Index.cshtml";
                return View(view);               
            }
            else
            {
                return RedirectToAction("Index");
            }            
        }

        #endregion      

        #region Trình Duyệt

        /// <summary>
        /// Trình duyệt chứng từ
        /// </summary>
        /// <param name="maChungTu"></param>
        /// <param name="iLoai"></param>
        /// <param name="sLNS"></param>
        /// <param name="iKyThuat"></param>
        /// <param name="sLyDo"></param>
        /// <param name="maChungTuTLTH"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult CopyChungTuBQLToB2(string maChungTu, string username)
        {
            _username = string.IsNullOrEmpty(username) ? User.Identity.Name : username;
            DataTable dtChungTu = DuToanBSChungTuModels.LayChungTu(maChungTu);
            
            //tao DTBS_ChungTu
            string iID_MaChungTu_ChungTuNew = "";
            iID_MaChungTu_ChungTuNew = TaoChungTuPhanCapB2(maChungTu, dtChungTu.Rows[0]);
            if (!string.IsNullOrEmpty(iID_MaChungTu_ChungTuNew))
            {
                DataTable dtChungTuChiTiet = getChungTuChiTietCoPhanCap(maChungTu);

                string IID_MaChungTuChiTiet_New = "";
                for (int j = 0; j < dtChungTuChiTiet.Rows.Count; j++)
                {
                    IID_MaChungTuChiTiet_New = TaoChungTuChiTietPhanCapB2(iID_MaChungTu_ChungTuNew, dtChungTuChiTiet.Rows[j]);
                    if (!string.IsNullOrEmpty(IID_MaChungTuChiTiet_New))
                    {
                        DataTable dtChungTuChiTiet_PhanCap = getChungTuChiTiet_PhanCap(dtChungTuChiTiet.Rows[j]["iID_MaChungTuChiTiet"].ToString());

                        for (int z = 0; z < dtChungTuChiTiet_PhanCap.Rows.Count; z++)
                        {
                            var check = TaoChungTuChiTietPhanCap_PhanCapB2(IID_MaChungTuChiTiet_New, dtChungTuChiTiet_PhanCap.Rows[z]);
                        }
                    }
                }
                
                
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public DataTable getChungTuChiTietCoPhanCap(string iID_MaChungTu)
        {
            DataTable vR = null;
            string SQL;

            #region update lại quy trình bổ sung dự toán ngân sách
            SQL = string.Format(@"
            select  * from DTBS_ChungTuChiTiet
            where   iTrangThai=1 
                    and iID_MaChungTu=@iID_MaChungTu 
                    and (
                        (rTuChi + rHangNhap + rHangMua) <> 0
                        or iID_MaChungTuChiTiet IN (
                                select iID_MaChungTu 
                                from DTBS_ChungTuChiTiet_PhanCap
                                where iTrangThai=1 and rTuChi<>0) 
                        or  iID_MaChungTuChiTiet in (
                                select iID_MaChungTu from DTBS_ChungTuChiTiet 
                                where iTrangThai=1 and rTuChi<>0))


            ");
            #endregion

            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public DataTable getChungTuChiTiet_PhanCap(string iID_MaChungTu)
        {
            DataTable vR = null;
            string SQL;
            SQL =
                string.Format(@"select  * from DTBS_ChungTuChiTiet_PhanCap where iID_MaChungTu=@iID_MaChungTu AND iTrangThai=1");
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public string TaoChungTuPhanCapB2(string iID_MaChungTu, DataRow R)
        {
            string iID_MaChungTu_New = "";
            Bang bang = new Bang("DTBS_ChungTu");
            bang.DuLieuMoi = true;
            for (int i = 1; i < R.Table.Columns.Count; i++)
            {
                bang.CmdParams.Parameters.AddWithValue("@" + R.Table.Columns[i], R[R.Table.Columns[i]]);
            }
            bang.CmdParams.Parameters["@iCheck"].Value = "0";
            bang.CmdParams.Parameters["@iCheckInPL"].Value = "0";
            bang.CmdParams.Parameters["@dNgayTao"].Value = DateTime.Now;
            bang.CmdParams.Parameters["@dNgaySua"].Value = DateTime.Now;
            bang.CmdParams.Parameters["@iID_MaPhongBan"].Value = "02";
            bang.CmdParams.Parameters["@sTenPhongBan"].Value = "02 - B02";
            bang.CmdParams.Parameters["@iID_MaPhongBanDich"].Value = "02";
            bang.CmdParams.Parameters["@sID_MaNguoiDung_DuocGiao"].Value = _username;
            bang.CmdParams.Parameters["@sID_MaNguoiDungTao"].Value = _username;
            bang.CmdParams.Parameters["@sID_MaNguoiDungSua"].Value = _username;
            iID_MaChungTu_New = Convert.ToString(bang.Save());
            return iID_MaChungTu_New;
        }        

        public string TaoChungTuChiTietPhanCapB2(string iID_MaChungTu, DataRow R)
        {
            string iID_MaChungTu_New = "";
            Bang bang = new Bang("DTBS_ChungTuChiTiet");
            bang.DuLieuMoi = true;
            for (int i = 1; i < R.Table.Columns.Count; i++)
            {
                bang.CmdParams.Parameters.AddWithValue("@" + R.Table.Columns[i], R[R.Table.Columns[i]]);
            }
            bang.CmdParams.Parameters["@dNgayTao"].Value = DateTime.Now;
            bang.CmdParams.Parameters["@dNgaySua"].Value = DateTime.Now;
            bang.CmdParams.Parameters["@iID_MaChungTu"].Value = iID_MaChungTu;
            bang.CmdParams.Parameters["@iID_MaPhongBan"].Value = "02";
            bang.CmdParams.Parameters["@sTenPhongBan"].Value = "02 - B02";
            bang.CmdParams.Parameters["@sID_MaNguoiDung_DuocGiao"].Value = _username;
            bang.CmdParams.Parameters["@sID_MaNguoiDungTao"].Value = _username;
            bang.CmdParams.Parameters["@sID_MaNguoiDungSua"].Value = _username;
            iID_MaChungTu_New = Convert.ToString(bang.Save());
            return iID_MaChungTu_New;
        }

        public string TaoChungTuChiTietPhanCap_PhanCapB2(string iID_MaChungTu, DataRow R)
        {
            string iID_MaChungTu_New = "";
            Bang bang = new Bang("DTBS_ChungTuChiTiet_PhanCap");
            bang.DuLieuMoi = true;
            for (int i = 1; i < R.Table.Columns.Count; i++)
            {
                bang.CmdParams.Parameters.AddWithValue("@" + R.Table.Columns[i], R[R.Table.Columns[i]]);
            }
            bang.CmdParams.Parameters["@dNgayTao"].Value = DateTime.Now;
            bang.CmdParams.Parameters["@dNgaySua"].Value = DateTime.Now;
            bang.CmdParams.Parameters["@iID_MaChungTu"].Value = iID_MaChungTu;
            bang.CmdParams.Parameters["@iID_MaPhongBan"].Value = "02";
            bang.CmdParams.Parameters["@sTenPhongBan"].Value = "02 - B02";
            bang.CmdParams.Parameters["@sID_MaNguoiDung_DuocGiao"].Value = _username;
            bang.CmdParams.Parameters["@sID_MaNguoiDungTao"].Value = _username;
            bang.CmdParams.Parameters["@sID_MaNguoiDungSua"].Value = _username;
            iID_MaChungTu_New = Convert.ToString(bang.Save());
            return iID_MaChungTu_New;
        }
        #endregion
    }
}
