using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Transactions;

namespace CogsDB.Engine.Storage
{
    public class Database
    {
        protected DbProviderFactory _factory;
        protected DbConnection _connection;
        protected string _connectionString;

        public Database(string connectionStringKey)
        {
            ConnectionStringSettings settings;

            try
            {
                settings = String.IsNullOrEmpty(connectionStringKey)
                    ? ConfigurationManager.ConnectionStrings[0]
                    : ConfigurationManager.ConnectionStrings[connectionStringKey];
                if (settings == null) throw new IndexOutOfRangeException("Could not find ConnectionString");
            }
            catch
            {
                throw new IndexOutOfRangeException("Could not find ConnectionString");
            }

            _connectionString = settings.ConnectionString;
            _factory = DbProviderFactories.GetFactory(settings.ProviderName);
        }

        public DbCommand GetStoredProcCommand(string procName)
        {
            return CreateCommand(CommandType.StoredProcedure, procName);
        }

        public DbCommand GetSqlStringCommand(string sql)
        {
            return CreateCommand(CommandType.Text, sql);

        }

        private DbCommand CreateCommand(CommandType commandType, string text)
        {
            DbCommand command = _factory.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = text;

            return command;
        }

        public void AddInParameter(DbCommand command, string parameterName, DbType type, object value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = BuildParameterName(parameterName);
            parameter.Direction = ParameterDirection.Input;
            parameter.DbType = type;
            parameter.Value = value;

            command.Parameters.Add(parameter);
        }

        public void AddOutParameter(DbCommand command, string parameterName, DbType type, int size)
        {
            DbParameter parameter = command.CreateParameter();

            parameter.Direction = ParameterDirection.Output;
            parameter.DbType = type;
            parameter.ParameterName = BuildParameterName(parameterName);
            parameter.Size = size;

            command.Parameters.Add(parameter);
        }

        public object GetParameterValue(DbCommand command, string parameterName)
        {
            object result;
            result = command.Parameters[BuildParameterName(parameterName)].Value;

            return result;
        }
        public IDataReader ExecuteReader(DbCommand command)
        {
            bool closeConnection = true;

            if (Transaction.Current != null)
                closeConnection = false;

            return ExecuteReader(command, closeConnection);
        }

        public IDataReader ExecuteReader(DbCommand command, bool closeConnection)
        {
            var conn = GetOpenConnection(closeConnection);

            PrepareCommand(command, conn);
            return DoExecuteReader(command, closeConnection);
        }

        private IDataReader DoExecuteReader(DbCommand command, bool closeConnection)
        {
            IDataReader reader = closeConnection
                          ? command.ExecuteReader(CommandBehavior.CloseConnection)
                          : command.ExecuteReader(CommandBehavior.Default);
            return reader;
        }

        public int ExecuteNonQuery(DbCommand command)
        {
            using (var conn = GetOpenConnection())
            {
                PrepareCommand(command, conn);
                return DoExecuteNonQuery(command);
            }
        }

        public int DoExecuteNonQuery(DbCommand command)
        {
            return command.ExecuteNonQuery();
        }

        public object ExecuteScalar(DbCommand command)
        {
            using (var conn = GetOpenConnection())
            {
                PrepareCommand(command, conn);
                return DoExecuteScalar(command);
            }
        }

        public object DoExecuteScalar(DbCommand command)
        {
            return command.ExecuteScalar();
        }

        private DbConnection GetOpenConnection()
        {
            return GetOpenConnection(true);
        }

        private DbConnection GetOpenConnection(bool disposeInnerConnection)
        {
            return GetNewOpenConnection();
        }

        internal DbConnection GetNewOpenConnection()
        {
            var connection = CreateConnection();

            connection.Open();

            return connection;
        }

        protected DbConnection CreateConnection()
        {
            var connection = _factory.CreateConnection();
            connection.ConnectionString = _connectionString;

            return connection;
        }

        protected static void PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (connection == null) throw new ArgumentException("connection");

            command.Connection = connection;
        }

        public virtual string BuildParameterName(string name)
        {
            return name;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    }

}
