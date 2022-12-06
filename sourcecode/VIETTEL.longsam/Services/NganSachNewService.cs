using Dapper;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;

namespace Viettel.Services
{
    public interface INganSachNewService : IServiceBase
    {
        #region NS_MucLucNganSach
        IEnumerable<NS_MucLucNganSach> GetDataDropdownMucLucNganSach(int iNamLamViec);
        IEnumerable<NS_MucLucNganSach> GetDataDropdownMucLucNganSachInVDT(int iNamLamViec);
        IEnumerable<NS_MucLucNganSach> GetDataDropdownMucLucNganSach(List<int> listiNamLamViec);
        DataTable GetDataDropdownLoaiAndKhoanByLoaiNganSach(string sLNS, int iNamKeHoach);
        DataTable GetDataDropdownLoaiAndKhoanByNganSach(string sLNS, int iNamKeHoach);
        IEnumerable<NS_MucLucNganSach> GetDataThongTinChiTietLoaiNganSach(string sLNS, string sLoai, string sKhoan, int iNamLamViec);
        IEnumerable<NS_MucLucNganSach> GetDataThongTinChiTietLoaiNganSach(string sLNS, int iNamLamViec);
        IEnumerable<NS_MucLucNganSach> GetDataDropdownLoaiAndKhoanByNganh(Guid iIdNganh, int iNamKeHoach);
        #endregion
    }

    public class NganSachNewService : INganSachNewService
    {
        #region Private
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static INganSachNewService _default;
        public NganSachNewService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
        }
        public static INganSachNewService Default
        {
            get { return _default ?? (_default = new NganSachNewService()); }
        }
        #endregion

        #region NS_MucLucNganSach
        public IEnumerable<NS_MucLucNganSach> GetDataDropdownMucLucNganSachInVDT(int iNamLamViec)
        {
            string sQuery = @"SELECT *
                              FROM NS_MucLucNganSach
                              WHERE (sM <> '' OR sTM <> '' OR sTTM <> '' OR sNG <> '') AND iTrangThai = 1 AND iNamLamViec = @iNamLamViec";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(sQuery,
                    param: new
                    {
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<NS_MucLucNganSach> GetDataDropdownMucLucNganSach(int iNamLamViec)
        {
            string sQuery = @"SELECT *
                              FROM NS_MucLucNganSach
                              WHERE sL = ''AND sK = '' AND sM = '' AND sTM = '' AND sTTM = '' AND sNG = '' AND iTrangThai = 1 AND iNamLamViec = @iNamLamViec";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(sQuery,
                    param: new
                    {
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<NS_MucLucNganSach> GetDataDropdownMucLucNganSach(List<int> listiNamLamViec)
        {
            string sQuery = @"SELECT *
                              FROM NS_MucLucNganSach
                              WHERE sL = ''AND sK = '' AND sM = '' AND sTM = '' AND sTTM = '' AND sNG = '' AND iTrangThai = 1 AND iNamLamViec in ("+string.Join(",", listiNamLamViec)+")";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(sQuery,
                   
                    commandType: CommandType.Text);

                return items;
            }
        }

        public DataTable GetDataDropdownLoaiAndKhoanByLoaiNganSach(string sLNS, int iNamKeHoach) {
            string sQuery = @"SELECT DISTINCT sL, sK FROM NS_MucLucNganSach 
                                WHERE iNamLamViec = @iNamKeHoach 
                                    AND sLNS = @sLNS 
                                    AND iTrangThai = 1
	                                AND bLaHangCha = 0 
                                    AND sNG <> '' 
                                    AND sTNG = ''";
            SqlCommand cmd = new SqlCommand(sQuery);
            cmd.Parameters.AddWithValue("@sLNS", sLNS);
            cmd.Parameters.AddWithValue("@iNamKeHoach", iNamKeHoach);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public DataTable GetDataDropdownLoaiAndKhoanByNganSach(string sLNS, int iNamKeHoach)
        {
            string sQuery = @"SELECT DISTINCT sL, sK FROM NS_MucLucNganSach 
                                WHERE iNamLamViec = @iNamKeHoach 
                                    AND sLNS = @sLNS 
                                    AND iTrangThai = 1
	                                AND bLaHangCha = 0 ";
            SqlCommand cmd = new SqlCommand(sQuery);
            cmd.Parameters.AddWithValue("@sLNS", sLNS);
            cmd.Parameters.AddWithValue("@iNamKeHoach", iNamKeHoach);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        
        public IEnumerable<NS_MucLucNganSach> GetDataDropdownLoaiAndKhoanByNganh(Guid iIdNganh, int iNamKeHoach)
        {
            string sQuery = @"DECLARE @sLNS nvarchar(100) 
                            DECLARE @sK nvarchar(100)
                            DECLARE @sL nvarchar(100)
                            SELECT @sLNS = sLNS, @sK = sK, @sL = sL FROM NS_MucLucNganSach WHERE iID_MaMucLucNganSach = @iIdNganh

                            SELECT DISTINCT * 
                            FROM NS_MucLucNganSach 
                            WHERE 
	                            iNamLamViec = @iNamKeHoach 
	                            AND sLNS = @sLNS 
	                            AND sl = @sl 
	                            AND sk = @sk 
	                            AND iTrangThai = 1
	                            AND bLaHangCha = 0
	                            AND sNG <> '' AND sTNG = ''";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(sQuery,
                    param: new
                    {
                        iIdNganh,
                        iNamKeHoach
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<NS_MucLucNganSach> GetDataThongTinChiTietLoaiNganSach(string sLNS, string sLoai, string sKhoan, int iNamLamViec)
        {
            string sQuery = @"SELECT * FROM NS_MucLucNganSach 
                                WHERE iNamLamViec = @iNamLamViec 
	                                AND sLNS = @sLNS 
	                                AND (@sl IS NULL OR sl = @sl) 
	                                AND (@sk IS NULL OR sk = @sk )
	                                AND iTrangThai = 1
	                                AND bLaHangCha = 0
	                                AND sNG <> '' AND sTNG = ''";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(sQuery,
                    param: new
                    {
                        iNamLamViec,
                        sLNS,
                        sl = sLoai,
                        sk = sKhoan
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<NS_MucLucNganSach> GetDataThongTinChiTietLoaiNganSach(string sLNS, int iNamLamViec)
        {
            string sQuery = @"SELECT * FROM NS_MucLucNganSach 
                                WHERE iNamLamViec = @iNamLamViec 
	                                AND sLNS = @sLNS 
	                                AND iTrangThai = 1
	                                AND bLaHangCha = 0
	                                AND sNG <> '' AND sTNG = ''";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(sQuery,
                    param: new
                    {
                        iNamLamViec,
                        sLNS
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }
        #endregion

    }
}
