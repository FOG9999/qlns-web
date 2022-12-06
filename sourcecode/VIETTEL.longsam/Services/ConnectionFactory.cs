using System.Configuration;
using System.Data.SqlClient;

namespace Viettel.Services
{
    public interface IConnectionFactory
    {
        string ConnectionString { get; }
        SqlConnection GetConnection();
    }

    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //private readonly string connectionString2 = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;

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

        private static IConnectionFactory _default;

        public static IConnectionFactory Default
        {
            get { return _default ?? (_default = new ConnectionFactory()); }
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
