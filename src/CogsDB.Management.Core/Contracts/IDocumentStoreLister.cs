using System;
using System.Collections.Generic;

namespace CogsDB.Management.Core.Contracts
{
    public interface IDocumentStoreLister
    {
        IEnumerable<string> AvailableDocumentStores();
    }
}
