using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Viettel.Data;
using VIETTEL.Helpers;

namespace Viettel.Services
{
    public interface IDuToanService
    {
        DataTable Get_ChiTieu_TheoDonVi(int namLamViec, string donvi, string phongBan = null);
        DataTable Get_DonViDT_TheoBql_ExistData_Negative(string nam, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataTable Get_DonViDT_TheoBql_ExistData(string nam, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataTable Get_DonViDT_ExistData(string nam, string donvis = null, string id_phongban = null, string id_phongbandich = null);
        DataRow GetCauHinhBia(string nam);
    }

    public class DuToanService : IDuToanService
    {
        private readonly IConnectionFactory _connectionFactory;
        public DuToanService(IConnectionFactory connectionFactory = null)
        {
            _connectionFactory = connectionFactory ?? ConnectionFactory.Default;
        }
        private static IDuToanService _default;

        public static IDuToanService Default
        {
            get { return _default ?? (_default = new DuToanService()); }
        }
        public DataTable Get_ChiTieu_TheoDonVi(
            int iNamLamViec, 
            string iID_MaDonVi, 
            string iID_MaPhongBanDich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                object phongban = string.IsNullOrWhiteSpace(iID_MaPhongBanDich) 
                    ? DBNull.Value 
                    : (object)iID_MaPhongBanDich;
                var dt = conn.ExecuteDataTable("NS_ChiTieu_TheoDonVi",
                    new
                    {
                        iNamLamViec,
                        iID_MaDonVi,
                        iID_MaPhongBanDich = phongban,
                    },
                    CommandType.StoredProcedure);

                return dt;
            }
        }
        public DataTable Get_DonViDT_ExistData(
            string nam,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var path_sql = "~/Areas/DuToan/Sql/donvidt.sql";
                var sql = FileHelpers.GetSqlQueryPath(path_sql);
                var dt = conn.ExecuteDataTable(sql,
                    new
                    {
                        nam,
                        donvis = donvis.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_phongbandich = id_phongbandich.ToParamString(),
                    },
                    CommandType.Text);

                return dt;
            }
        }

        public DataTable Get_DonViDT_TheoBql_ExistData(
            string nam,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = FileHelpers.GetSqlQuery("donvidt.sql");
                var dt = conn.ExecuteDataTable(sql,
                    new
                    {
                        nam,
                        donvis = donvis.ToParamString(),
                        id_phongban = id_phongban.ToParamString(),
                        id_phongbandich = id_phongbandich.ToParamString(),
                    },
                    CommandType.Text);

                return dt;
            }
        }

        public DataTable Get_DonViDT_TheoBql_ExistData_Negative(
            string nam,
            string donvis = null,
            string id_phongban = null,
            string id_phongbandich = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand("donvidt_neg", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_phongban", id_phongban.ToParamString());
                    cmd.Parameters.AddWithValue("@id_phongbandich", id_phongbandich.ToParamString());
                    cmd.Parameters.AddWithValue("@nam", nam);
                    cmd.Parameters.AddWithValue("@donvis", donvis);

                    var dt = cmd.GetDataset().Tables[0];
                    return dt;
                }
            }
        }

        public DataRow GetCauHinhBia(string nam)
        {
            String SQL = "SELECT * FROM DT_CauHinhBia WHERE iTrangThai=1 AND iNamLamViec=@iID_NamLamViec";
            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(SQL, conn))
                {
                    cmd.Parameters.AddWithValue("@iID_NamLamViec", nam);

                    var dt = cmd.GetDataset().Tables[0];
                    if (dt.Rows.Count > 0) return dt.Rows[0];
                    else
                    {
                        SQL = "SELECT TOP 1 * FROM DT_CauHinhBia WHERE iTrangThai=1 ORDER BY iNamLamViec DESC";
                        cmd.CommandText = SQL;
                        return cmd.GetDataset().Tables[0].Rows[0];
                    }
                }
            }

                
        }
    }
}
