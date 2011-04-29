using System;
using System.Data.Common;

namespace CogsDB.Engine.Storage
{
    public class ConnectionWrapper : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _disposeInnerConnection;

        public ConnectionWrapper(DbConnection connection, bool disposeInnerConnection)
        {
            _connection = connection;
            _disposeInnerConnection = disposeInnerConnection;
        }

        public void Dispose()
        {
            if (_disposeInnerConnection)
            {
                _connection.Dispose();
            }
        }

        public DbConnection Connection
        {
            get { return _connection; }
        }
    }
}