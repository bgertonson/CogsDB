using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CogsDB.Engine;
using Json = Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace CogsDB.Management.Controllers
{
    public class DocumentController : Controller
    {
        public DocumentController()
        {
        }

        public ActionResult Get(string docStore, string id)
        {
            var persister = GetPersister(docStore);
            var document = persister.Get(id);
            if (document == null) return Content(String.Empty);

            var data = GetBson(document);

            return File(data, "application/bson");
        }

        private byte[] GetBson(object @object)
        {
            var serializer = new Json.JsonSerializer();
            var ms = new MemoryStream();
            var writer = new BsonWriter(ms);
            serializer.Serialize(writer, @object);

            return ms.ToArray();
        }

        public ActionResult GetAll(string docStore, string[] ids)
        {
            var persister = GetPersister(docStore);
            var documents = persister.Get(ids).ToArray();
            var data = GetBson(documents);
            return File(data, "application/bson");
        }

        [HttpPost]
        public ActionResult Put(string docStore)
        {
            var persister = GetPersister(docStore);
            var serializer = new Json.JsonSerializer();
            var stream = Request.InputStream;
            var reader = new BsonReader(stream);
            var result = serializer.Deserialize<Document>(reader);
            if (result == null) return Content("FAIL");

            persister.Put(new[] {result});
            return Content("OK");
        }

        public ActionResult Delete(string docStore, string id)
        {
            var persister = GetPersister(docStore);
            persister.Delete(id);

            return Content("OK");
        }

        private ICogsPersister GetPersister(string docStore)
        {
            return new SqlPersister(docStore);
        }
    }
}
