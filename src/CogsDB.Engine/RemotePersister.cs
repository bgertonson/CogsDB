using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Json = Newtonsoft.Json;
using Newtonsoft.Json.Bson;


namespace CogsDB.Engine
{
    public class RemotePersister: ICogsPersister
    {
        private string _url;
        private const string GET_URL = "get";
        private const string GET_ALL_URL = "get-all";
        private const string PUT_URL = "put";
        private const string DELETE_URL = "delete";

        public RemotePersister(string url)
        {
            _url = url;
        }

        public void Put(IEnumerable<Document> documents)
        {
            var serializer = new Json.JsonSerializer();
            //TODO: Make a single call
            foreach (var document in documents)
            {
                var ms = new MemoryStream();
                // serialize value to BSON
                var writer = new BsonWriter(ms);
                serializer.Serialize(writer, document);

                var data = ms.ToArray();

                var uri = new Uri(string.Format("{0}/{1}", _url, PUT_URL));
                using (var client = new WebClient())
                    client.UploadData(uri, data);
            }

        }

        public Document Get(string id)
        {
            using (var client = new WebClient())
            {
                var data = client.DownloadData(string.Format("{0}/{1}/{2}", _url, GET_URL, id));

                var serializer = new Json.JsonSerializer();
                using (var stream = new MemoryStream(data))
                {
                    var reader = new BsonReader(stream);
                    var result = serializer.Deserialize<Document>(reader);
                    return result;
                }
            }
        }

        public IEnumerable<Document> Get(string[] ids)
        {
            using (var client = new WebClient())
            {
                var data =
                    client.DownloadData(String.Format("{0}/{1}/?ids={2}", _url, GET_ALL_URL, String.Join(",", ids)));
                if (data.Length == 0) return null;

                var serializer = new Json.JsonSerializer();
                using (var stream = new MemoryStream(data))
                {
                    var reader = new BsonReader(stream);
                    var result = serializer.Deserialize<Document[]>(reader);
                    return result;
                }
            }
        }

        public IEnumerable<Document> GetAll(string type)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            using(var client = new WebClient())
            {
                client.OpenRead(String.Format("{0}/{1}/{2}", _url, DELETE_URL, id));
            }
        }
    }
}
