using System;
using System.Collections.Generic;
using System.Linq;

namespace CogsDB.Engine
{
    public class CogsSession: ICogsSession
    {
        private readonly ICogsPersister _persister;
        private readonly IDocumentSerializer _serializer;
        private readonly IIdentityServer _identityServer;

        private readonly IDictionary<string, Document> _tracking =
            new Dictionary<string, Document>(StringComparer.CurrentCultureIgnoreCase);

        private readonly IList<Document> _queue = new List<Document>();

        public CogsSession(ICogsPersister persister, IDocumentSerializer serializer)
        {
            _persister = persister;
            _serializer = serializer;
            _identityServer = new IdentityServer(this);
        }

        public T Load<T>(string id) where T: class
        {
            var doc = _persister.Get(id);
            if (doc == null) return null;
            
            _tracking.Add(id, doc);
            var @object = _serializer.Deserialize<T>(doc.Content);
            return @object;
        }

        public T[] Load<T>(params string[] ids) where T: class
        {
            IList<T> objects = new List<T>();
            var docs = _persister.Get(ids);
            foreach (var doc in docs)
            {
                _tracking[doc.Id] = doc;
                objects.Add(_serializer.Deserialize<T>(doc.Content));
            }
            return objects.ToArray();
        }

        public T[] LoadAll<T>() where T : class
        {
            var docs = _persister.GetAll(typeof (T).Name);
            IList<T> objects = new List<T>();
            foreach (var doc in docs)
            {
                _tracking[doc.Id] = doc;
                objects.Add(_serializer.Deserialize<T>(doc.Content));
            }
            return objects.ToArray();
        }

        public void Store<T>(T @object) where T: class
        {
            Document document = BuildDocument(@object);

            _queue.Add(document);
        }

        internal void StoreImmediate<T>(T @object) where T: class
        {
            var document = BuildDocument(@object);
            _persister.Put(new[]{document});
        }

        private Document BuildDocument<T>(T @object) where T: class
        {
            var id = ExtractId(@object);
            var metadata = ExtractMetadata(@object);
            bool isNew = !IsTracked(id);
            var content = _serializer.Serialize(@object);
            var meta = _serializer.Serialize(metadata);
            return new Document
                       {
                           Id = id,
                           Type = typeof(T).Name,
                           Content = content,
                           Metadata = meta,
                           CreateDate = DateTime.Now,
                           ModifyDate = DateTime.Now,
                           IsNew = isNew
                       };
        }

        public void Delete(string id)
        {
            _persister.Delete(id);
        }

        public void SubmitChanges()
        {
            _persister.Put(_queue.ToArray());
            _queue.Clear();
            _tracking.Clear();
        }

        private bool IsTracked(string id)
        {
            return _tracking.ContainsKey(id);
        }

        private string ExtractId<T>(T document)
        {
            var type = typeof(T);
            var prop = type.GetProperty("Id");
            var value = prop.GetValue(document, null) as String;

            if (value != null) return value;

            value = _identityServer.GetNextIdentity<T>();
            prop.SetValue(document, value, null);

            return value;
        }

        private Metadata ExtractMetadata(object @object)
        {
            var type = @object.GetType();
            var metadata = new Metadata() {ClrType = type.FullName, AssemblyName = type.AssemblyQualifiedName};
            return metadata;
        }
    }
}