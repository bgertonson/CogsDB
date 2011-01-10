using System.Collections.Generic;

namespace CogsDB.Engine
{
    public interface ICogsPersister
    {
        void Put(IEnumerable<Document> documents);
        void Put(Document document);
        Document Get(string id);
        IEnumerable<Document> Get(string[] ids);
        IEnumerable<Document> GetAll(string type);
        void Delete(string id);
    }
}
