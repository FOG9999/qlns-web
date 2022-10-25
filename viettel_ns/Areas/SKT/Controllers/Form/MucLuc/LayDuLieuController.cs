using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{

    public class LayDuLieuController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly ISharedService _sharedService = SharedService.Default;

        public ActionResult GetComData()
        {
            if ((_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11") && _sharedService.CheckLock(PhienLamViec.NamLamViec,"1","SKT_ComDatas") && _sharedService.CheckLock(PhienLamViec.NamLamViec, "1", "SKT_ComDatasDacThu") && _sharedService.CheckLock(PhienLamViec.NamLamViec, "1", "SKT_ComDatasSKTNT"))
            {
                using (var conn = _connectionFactory.GetConnection())
                using (var cmd = new SqlCommand("sp_ncskt_getcomdata", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParams(new
                    {
                        nam = PhienLamViec.NamLamViec,
                    });
                    conn.Open();
                    var result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public ActionResult GetMLDacThu()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11")
            {
                using (var conn = _connectionFactory.GetConnection())
                using (var cmd = new SqlCommand("sp_ncskt_getdacthu", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParams(new
                    {
                        nam = PhienLamViec.NamLamViec,
                    });
                    conn.Open();
                    var result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public ActionResult GetSoLieuCanCu()
        {
            if ((_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11") && _sharedService.CheckLock(PhienLamViec.NamLamViec, "1", "SKT_ComData_ChungTuChiTiet"))
            {
                using (var conn = _connectionFactory.GetConnection())
                using (var cmd = new SqlCommand("sp_ncskt_getcomdata_ctct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParams(new
                    {
                        nam = PhienLamViec.NamLamViec,
                    });
                    conn.Open();
                    var result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public ActionResult CopyMucLucNT()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11")
            {
                using (var conn = _connectionFactory.GetConnection())
                using (var cmd = new SqlCommand("sp_ncskt_copymlnc_mlskt", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParams(new
                    {
                        namd = PhienLamViec.NamLamViec,
                        nams = PhienLamViec.NamLamViec - 1,
                    });
                    conn.Open();
                    var result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
