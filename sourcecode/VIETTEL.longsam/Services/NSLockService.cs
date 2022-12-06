using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Viettel.Domain.DomainModel;

namespace Viettel.Services
{
    public interface INSLockService
    {
        bool CheckLock(int nam, string id_phanhe, string id_phongban, string id_donvi = null);
    }

    public class NSLockService : INSLockService
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private static INSLockService _default;
        public static INSLockService Default => _default ?? (_default = new NSLockService());


        public bool CheckLock(int nam, string id_phanhe, string id_phongban, string id_donvi = null)
        {
            var sql = @"

select * from NS_Lock
where   NamLamViec=@NamLamViec
        and Id_PhongBan=@Id_PhongBan

";

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<NS_Lock>(
                    sql, new
                    {
                        NamLamViec = nam,
                        id_phongban,
                    });

                if (entity == null) return false;
                else if (entity.iLock) return true;
                else if (id_donvi.IsNotEmpty() && entity.Id_DonVi.ToList().Contains(id_donvi)) return true;


                return false;
            }
        }
    }
}
