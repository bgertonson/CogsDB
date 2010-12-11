namespace CogsDB.Engine
{
    public interface ICogsSession
    {
        T Load<T>(string id) where T: class;
        T[] Load<T>(params string[] ids) where T: class;
        void Store<T>(T @object) where T: class;
        void Delete(string id);
        void SubmitChanges();
        T[] LoadAll<T>() where T : class;
    }
}
