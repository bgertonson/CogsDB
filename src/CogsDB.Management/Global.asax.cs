using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Mvc;

namespace CogsDB.Management
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Document", "{docStore}/{docType}/{docId}", new {controller = "Home", action = "ViewDocument"});

            routes.MapRoute(
                "Docs", "{docStore}/{docType}", new { controller = "Home", action = "DocumentList" });

            routes.MapRoute(
                "DocTypes", "{docStore}", new {controller = "Home", action = "DocumentTypeList"});

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected override void  OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }

        protected override IKernel CreateKernel()
        {
            return new StandardKernel(Bootstrapper.GetModules());
        }
    }
}