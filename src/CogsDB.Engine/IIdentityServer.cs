namespace CogsDB.Engine
{
    public interface IIdentityServer
    {
        string GetNextIdentity<T>();
    }

    public interface IIdentityProvider
    {
        int GetNextBlock(string type);
    }
}
