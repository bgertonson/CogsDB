using System;

namespace CogsDB.Engine.Configuration
{
    public interface ICogsConfiguration
    {
        ICogsConfiguration Storage(IStorageConfiguration config);
    }

    public interface IStorageConfiguration
    {
        IStorageConfiguration ConnectionString(Func<IConnectionStringConfiguration> config);
    }

    public interface IConnectionStringConfiguration
    {
        IStorageConfiguration FromKey(Func<IConnectionStringConfiguration> config);
    }

    public class MsSql: MsSqlStorageBase
    {
        public static IStorageConfiguration CE4(Func<IStorageConfiguration> config)
        {
            return new MsSqlStorageBase();
        }
    }

    public class MsSqlStorageBase: IStorageConfiguration
    {
        public IStorageConfiguration ConnectionString(Func<IConnectionStringConfiguration> config)
        {
            throw new NotImplementedException();
        }
    }
}
