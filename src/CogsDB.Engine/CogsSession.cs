using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace CogsDB.Engine
{
    public class CogsSession: ICogsSession
    {
        private readonly ICogsPersister _persister;
        private readonly IDocumentSerializer _serializer;
        private readonly IIdentityServer _identityServer;

        private readonly IDictionary<string, Document> _tracked =
            new Dictionary<string, Document>(StringComparer.CurrentCultureIgnoreCase);

        private readonly IList<string> _deletes = new List<string>();

        private readonly IList<Document> _updates = new List<Document>();

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
            
            _tracked.Add(id, doc);
            var @object = _serializer.Deserialize<T>(doc.Content);
            return @object;
        }

        public T[] Load<T>(params string[] ids) where T: class
        {
            IList<T> objects = new List<T>();
            var docs = _persister.Get(ids);
            foreach (var doc in docs)
            {
                _tracked[doc.Id] = doc;
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
                _tracked[doc.Id] = doc;
                objects.Add(_serializer.Deserialize<T>(doc.Content));
            }
            return objects.ToArray();
        }

        public void Store<T>(T @object) where T: class
        {
            Document document = BuildDocument(@object);

            _updates.Add(document);
        }

        internal void StoreImmediate<T>(T @object) where T: class
        {
            var document = BuildDocument(@object);
            _persister.Put(new[]{document});
        }

        public void Delete(string id)
        {
            _deletes.Add(id);
        }

        public void SubmitChanges()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                _persister.Put(_updates.ToArray());
                foreach (var id in _deletes)
                {
                    _persister.Delete(id);
                }

                _updates.Clear();
                _deletes.Clear();
                _tracked.Clear();
                transaction.Complete();
            }
        }

        private bool IsTracked(string id)
        {
            return _tracked.ContainsKey(id);
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

        private Metadata ExtractMetadata(object @object)
        {
            var type = @object.GetType();
            var metadata = new Metadata() {ClrType = type.FullName, AssemblyName = type.AssemblyQualifiedName};
            return metadata;
        }
    }
}