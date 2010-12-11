using Newtonsoft.Json;

namespace CogsDB.Engine
{
    public class JsonSerializer: IDocumentSerializer
    {
        public T Deserialize<T>(string data) where T: class
        {
            var @object = JsonConvert.DeserializeObject<T>(data);
            return @object;
        }

        public string Serialize<T>(T @object) where T: class
        {
            var data = JsonConvert.SerializeObject(@object);
            return data;
        }
    }
}