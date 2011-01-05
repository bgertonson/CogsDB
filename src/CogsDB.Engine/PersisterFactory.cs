using System;
using System.Collections.Generic;
using System.Configuration;

namespace CogsDB.Engine
{
    public static class PersisterFactory
    {
        private static IDictionary<string, RegisteredPersister> _storage =
            new Dictionary<string, RegisteredPersister>(StringComparer.OrdinalIgnoreCase);

        private const string DefaultKey = "sql";

        static PersisterFactory()
        {
            _storage.Add("url", new RegisteredPersister(ExtractUrl, c => new RemotePersister(c)));
            _storage.Add("sql", new RegisteredPersister(s => s, c => new SqlPersister(c)));
        }

        public static ICogsPersister Create(string connectionName)
        {
            var connection = ConfigurationManager.ConnectionStrings[connectionName];
            //TODO: throw exception when no connection found with given name
            var connString = connection.ConnectionString;
            var connTypeKey = connString.TextBefore("=");

            var registration = _storage.ContainsKey(connTypeKey) ? _storage[connTypeKey] : _storage[DefaultKey];

            return registration.GetPersister(connectionName);
        }

        private static string ExtractUrl(string connectionName)
        {
            var connection = ConfigurationManager.ConnectionStrings[connectionName];
            //TODO: throw exception when no connection found with given name
            var connString = connection.ConnectionString;
            var url = connString.TextAfter("=");
            return url;
        }

        private class RegisteredPersister
        {
            public RegisteredPersister(Func<string, string> connectionStringResolver, Func<String, ICogsPersister> creator)
            {
                ConnectionStringResolver = connectionStringResolver;
                PersisterCreator = creator;
            }

            public Func<String, String> ConnectionStringResolver { get; set; }
            public Func<String, ICogsPersister> PersisterCreator { get; set; }

            public ICogsPersister GetPersister(string connectionName)
            {
                var connectionDetail = ConnectionStringResolver(connectionName);
                return PersisterCreator(connectionDetail);
            }
        }

    }
}