using System;
using System.Collections.Generic;
using CogsDB.Engine;
using CogsDB.Management.Core.Entities;

namespace CogsDB.Management.Core.Contracts
{
    public interface IDocumentStoreAccess
    {
        IEnumerable<String> ListDocumentTypes(string docStore);
        IEnumerable<DocumentSummary> ListDocuments(string docStore, string docType);
        Document GetDocument(string docStore, string docId);
    }
}
