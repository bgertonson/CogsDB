namespace CogsDB.Engine
{
    public interface IIdentityServer
    {
        string GetNextIdentity<T>();
    }
}
