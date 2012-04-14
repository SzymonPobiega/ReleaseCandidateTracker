using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReleaseCandidateTracker.Infrastructure;

namespace ReleaseCandidateTracker
{
    public class MvcApplication : HttpApplication
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
                "env/{action}/{name}",
                new
                {
                    controller = "Environment",
                    action = "Index",
                    name = UrlParameter.Optional
                });            

            routes.MapRoute(
                "EnvironmentApi",
                "env_api/{action}/{name}",
                new
                {
                    controller = "EnvironmentApi",
                    action = "Index",
                    name = UrlParameter.Optional
                });

            routes.MapRoute(
                "ReleaseCandidate",
                "rc/{action}/{versionNumber}",
                new
                {
                    controller = "ReleaseCandidate",
                    action = "Index",
                    versionNumber = UrlParameter.Optional
                });

            routes.MapRoute(
                "ReleaseCandidateApi",
                "rc_api/{action}/{versionNumber}",
                new
                {
                    controller = "ReleaseCandidateApi",
                    action = "Index",
                    versionNumber = UrlParameter.Optional
                });

            routes.MapRoute(
                "default",
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