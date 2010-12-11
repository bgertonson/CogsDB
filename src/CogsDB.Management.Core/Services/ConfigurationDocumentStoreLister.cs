using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using CogsDB.Management.Core.Contracts;

namespace CogsDB.Management.Core.Services
{
    public class ConfigurationDocumentStoreLister: IDocumentStoreLister
    {
        public IEnumerable<String> AvailableDocumentStores()
        {
            var connections = ConfigurationManager.ConnectionStrings;
            var documentStores = connections.Cast<ConnectionStringSettings>().Select(c => c.Name);
            return documentStores;
        }
    }
}
