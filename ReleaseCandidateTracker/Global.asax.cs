using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReleaseCandidateTracker.Infrastructure;

namespace ReleaseCandidateTracker
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Environment",
                "Environment/{action}/{name}",
                new
                {
                    controller = "Environment",
                    action = "Index",
                    name = UrlParameter.Optional
                });

            routes.MapRoute(
                "ReleaseCandidate",
                "{controller}/{action}/{versionNumber}",
                new
                {
                    controller = "ReleaseCandidate",
                    action = "Index",
                    versionNumber = UrlParameter.Optional
                });            
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Database.Initialize();
        }
    }
}