using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;

namespace VIETTEL.Controllers.CapPhat
{
    public class CapPhat_CapTheoCTGomController : Controller
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        #region Index
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_chungtucp">Id Chứng từ cấp phát</param>
        /// <param name="loai">loại cấp phát</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult Index(string id_chungtucp, string loai)
        {
            if (User.Identity.Name != "chuctc" && User.Identity.Name != "b10")
                return RedirectToAction("Index", "Home", new { area = "" });
            else
            {
                ViewData["id_chungtucp"] = id_chungtucp;
                ViewData["loai"] = loai;
                return View("~/Views/CapPhat/ChungTuChiTiet/CapPhat_CapTheoCTGom.aspx");
            }
        }

        #endregion

        #region Insert Data cấp phát from đợt gom dự toán bổ sung

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
        public ActionResult InsertData(string id_chungtucp, string loai)
        {
            string dsMaChungTu = Convert.ToString(Request.Form["iID_MaChungTu"]);
            string bolDel = Convert.ToString(Request.Form["bolDel"]);

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_cp_insertdata_fromdotgom", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParams(new
                {
                    idget = dsMaChungTu.ToParamString(),
                    idset = id_chungtucp.ToParamString(),
                    bolDel,
                });
                conn.Open();
                var result = cmd.ExecuteNonQuery();
                conn.Close();
            }

            return RedirectToAction("Index", "CapPhat_ChungTuChiTiet",
                new { Loai = loai, iID_MaCapPhat = id_chungtucp });
        }
        #endregion
    }
}