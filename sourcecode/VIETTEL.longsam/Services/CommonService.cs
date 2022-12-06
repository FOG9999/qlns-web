using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;
using Viettel.Models;

namespace Viettel.Services
{
    public interface ICommonService : IServiceBase
    {
        #region Menu

        IEnumerable<MenuLeftViewModel> GetCurrentMenu(int iMenuID, string sMaNguoiDung);

        IEnumerable<MENU_MenuItem> GetBreadcrumbStringByMenuId(int iMenuID);

        IEnumerable<MENU_MenuItem> GetAllMenu();

        #endregion
    }
    public class CommonService : ICommonService
    {
        #region Private
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static ICommonService _default;
        private readonly IConnectionMenuFactory _connectionMenuFactory;
        #endregion

        public CommonService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null,
            IConnectionMenuFactory connectionMenuFactory = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
            _connectionMenuFactory = connectionMenuFactory ?? new ConnectionMenuFactory();
        }
        public static ICommonService Default
        {
            get { return _default ?? (_default = new CommonService()); }
        }

        public IEnumerable<MenuLeftViewModel> GetCurrentMenu(int iMenuID, string sMaNguoiDung)
        {
            using (var conn = _connectionMenuFactory.GetConnection())
            {
                var items = conn.Query<MenuLeftViewModel>("GET_MENU_LEFT",
                    param: new
                    {
                        menuID = iMenuID,
                        sMaNguoiDung
                    },
                    commandType: CommandType.StoredProcedure);

                return items;
            }
        }

        public IEnumerable<MENU_MenuItem> GetBreadcrumbStringByMenuId(int iMenuID)
        {
            using (var conn = _connectionMenuFactory.GetConnection())
            {
                var lstBreadcumb = conn.Query<MENU_MenuItem>("GET_BREADCRUMB_STRING_BY_MENUID",
                    param: new
                    {
                        iMenuID,
                    },
                    commandType: CommandType.StoredProcedure);

                return lstBreadcumb;
            }
        }

        public IEnumerable<MENU_MenuItem> GetAllMenu()
        {
            using(var conn = _connectionMenuFactory.GetConnection())
            {
                return conn.Query<MENU_MenuItem>("SELECT * FROM MENU_MenuItem");
            }
        }
    }
}
