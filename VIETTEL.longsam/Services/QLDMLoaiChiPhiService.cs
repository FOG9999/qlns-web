using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;
using VIETTEL.Helpers;

namespace Viettel.Services
{
    public interface IQLDMLoaiChiPhiService : IServiceBase
    {
        IEnumerable<VDT_DM_ChiPhi> GetAllVDT_DM_ChiPhi(string sMaChiPhi = "", string sTenChiPhi = "");

        IEnumerable<VDT_DM_ChiPhi> GetAllVDT_DM_ChiPhiPaging(ref PagingInfo _paging, string sMaChiPhi = "", string sTenChiPhi = "");

        bool InsertLoaiChiPhi(string sMaChiPhi, string sTenVietTat, string sTenChiPhi, string sMoTa, int iThuTu, string sUserLogin);
        bool UpdateLoaiChiPhi(Guid iId, string sMaChiPhi, string sTenVietTat, string sTenChiPhi, string sMoTa, int iThuTu, string sIPSua, string sUserLogin);
        VDT_DM_ChiPhi GetLoaiChiPhi(Guid id);

        bool deleteLoaiChiPhi(Guid iId);
    }

    public class QLDMLoaiChiPhiService: IQLDMLoaiChiPhiService
    {
        #region Private
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static IQLDMLoaiChiPhiService _default;
        #endregion

        public QLDMLoaiChiPhiService(
           IConnectionFactory connectionFactory = null,
           ILocalizationService languageService = null,
           ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
        }


        public static IQLDMLoaiChiPhiService Default
        {
            get { return _default ?? (_default = new QLDMLoaiChiPhiService()); }
        }

        public IEnumerable<VDT_DM_ChiPhi> GetAllVDT_DM_ChiPhi(string sMaChiPhi , string sTenChiPhi)
        {
            var sql = FileHelpers.GetSqlQuery("lcp_get_all_LoaiChiPhi.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_ChiPhi>(sql,
                    param: new
                    {
                        sMaChiPhi = sMaChiPhi,
                        sTenChiPhi = sTenChiPhi
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<VDT_DM_ChiPhi> GetAllVDT_DM_ChiPhiPaging(ref PagingInfo _paging, string sMaChiPhi, string sTenChiPhi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sMaChiPhi", sMaChiPhi);
                lstParam.Add("sTenChiPhi", sTenChiPhi);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_DM_ChiPhi>("proc_get_all_loaichiphi_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public void CreateLoaiChiPhi(VDT_DM_ChiPhi objDM)
        {
            objDM.iID_ChiPhi = Guid.NewGuid();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Insert(objDM);
            }
        }

        public bool InsertLoaiChiPhi(string sMaChiPhi, string sTenVietTat, string sTenChiPhi, string sMoTa, int iThuTu, string sUserLogin)
        {
            var sql = FileHelpers.GetSqlQuery("lcp_insert_LoaiChiPhi.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        sMaChiPhi,
                        sTenVietTat,
                        sTenChiPhi,
                        sMoTa,
                        iThuTu,
                        sUserLogin
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public bool UpdateLoaiChiPhi(Guid iId, string sMaChiPhi, string sTenVietTat, string sTenChiPhi, string sMoTa, int iThuTu, string sIPSua, string sUserLogin)
        {
            var sql = FileHelpers.GetSqlQuery("lcp_update_LoaiChiPhi.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId,
                        sMaChiPhi,
                        sTenVietTat,
                        sTenChiPhi,
                        sMoTa,
                        iThuTu,
                        sIPSua,
                        sUserLogin

                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public VDT_DM_ChiPhi GetLoaiChiPhi(Guid id)
        {
            var sql = FileHelpers.GetSqlQuery("lcp_get_by_id_LoaiChiPhi.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<VDT_DM_ChiPhi>(
                    sql,
                    param: new
                    {
                        iID_ChiPhi = id,
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public bool deleteLoaiChiPhi(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("lcp_delete_LoaiChiPhi.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

    }

}
