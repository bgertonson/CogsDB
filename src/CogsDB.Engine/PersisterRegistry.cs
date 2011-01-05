using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogsDB.Engine
{
    public class PersisterRegistry
    {
        private static readonly IList<RegistrationEntry> _entries = new List<RegistrationEntry>();

        public static void Register(String key, Func<String, ICogsPersister> func)
        {
            //TODO: throw exception on existing key... should not be able to replace existing
            _entries.Add(new RegistrationEntry(key, func));
        }

        public static ICogsPersister Get(string key, string connection)
        {
            var entry = _entries.FirstOrDefault(x => x.Key == key);
            if (entry == null) return null;
            return entry.InstantiationMethod(connection);
        }

        static PersisterRegistry()
        {
            Register("SQL", c => new SqlPersister(c));
            Register("REST", c => new RemotePersister(c));
        }

        public class RegistrationEntry
        {
            public RegistrationEntry(string key, Func<String, ICogsPersister> method)
            {
                Key = key;
                InstantiationMethod = method;
            }

            public string Key { get; private set; }
            public Func<String, ICogsPersister> InstantiationMethod { get; private set; }
        }
    }
}
