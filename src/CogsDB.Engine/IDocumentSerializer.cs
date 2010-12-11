using System;

namespace CogsDB.Engine
{
    public interface IDocumentSerializer
    {
        T Deserialize<T>(string data) where T: class;
        String Serialize<T>(T @object) where T: class;
    }
}