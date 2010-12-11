using CogsDB.Management.Core;
using CogsDB.Management.Core.Contracts;
using CogsDB.Management.Core.DataAccess;
using CogsDB.Management.Core.Services;
using Ninject.Modules;

namespace CogsDB.Management
{
    public class Bootstrapper
    {

        public static INinjectModule[] GetModules()
        {
            var modules = new[] {new ServicesModule() as NinjectModule, new DataAccessModule()};
            return modules;
        }

        public class ServicesModule: NinjectModule
        {
            public override void Load()
            {
                Bind<IDocumentStoreLister>().To<ConfigurationDocumentStoreLister>().InSingletonScope();
            }
        }

        public class DataAccessModule : NinjectModule
        {
            public override void Load()
            {
                Bind<IDocumentStoreAccess>().To<SqlDocumentStoreAccess>();
            }
        }
    }
}