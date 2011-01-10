using System;
using System.Collections.Generic;
using System.Web;

namespace CogsDB.Engine
{
    public class CogsStorage
    {
        private static readonly IDictionary<CogsSessionManagementStrategy, Func<ICogsSessionManager>> ManagerConfigurations;

        static CogsStorage()
        {
            ManagerConfigurations = new Dictionary<CogsSessionManagementStrategy, Func<ICogsSessionManager>>();
            ManagerConfigurations.Add(CogsSessionManagementStrategy.Static, () => new StaticSessionManager());
            ManagerConfigurations.Add(CogsSessionManagementStrategy.Web,
                                       () => new WebSessionManager(new HttpContextWrapper(HttpContext.Current)));
        }

        public void Initialize(ICogsConfiguration configuration)
        {
            if(String.IsNullOrEmpty(configuration.ConnectionName))
                throw new ArgumentException("A Connection Name must be provided");

            var persister = new SqlPersister(configuration.ConnectionName);

            SessionFactory.GetSession =
                () => new CogsSession(persister, new JsonSerializer());
            Func<ICogsSessionManager> factoryMethod;
            ManagerConfigurations.TryGetValue(configuration.SessionManagementStrategy, out factoryMethod);
            SessionManagerFactory.GetSessionManager = factoryMethod ??
                                                      ManagerConfigurations[CogsSessionManagementStrategy.Static];
        }

        public ICogsSession OpenSession()
        {
            return SessionManagerFactory.GetSessionManager().GetSession();
        }
    }
}
