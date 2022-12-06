using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Services
{
    public interface IConnectionMenuFactory
    {
        string ConnectionString { get; }
        SqlConnection GetConnection();
    }

    public class ConnectionMenuFactory : IConnectionMenuFactory
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServicesMenu"].ConnectionString;

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        public SqlConnection GetConnection()
        {
            var sql = new SqlConnection(ConnectionString);
            return sql;
        }

        #region default

        private static IConnectionMenuFactory _default;

        public static IConnectionMenuFactory Default
        {
            get { return _default ?? (_default = new ConnectionMenuFactory()); }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ConnectionFactory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
