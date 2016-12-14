namespace Tailspin.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AppRoutes
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "OnBoarding",
                string.Empty,
                new { controller = "OnBoarding", action = "Index" });

            routes.MapRoute(
                "Management",
                "Management",
                new { controller = "Management", action = "Index" });

            routes.MapRoute(
                "Management-New",
                "Management/new",
                new { controller = "Management", action = "New" });

            routes.MapRoute(                
                "Management-Detail",
                "Management/{tenantId}",
                new { controller = "Management", action = "Detail" });

            routes.MapRoute(
               "JoinTenant",
               "Join",
               new { controller = "OnBoarding", action = "Join" });

            routes.MapRoute(
                "MyAccount",
                "{tenantId}/MyAccount",
                new { controller = "Account", action = "Index" });

            routes.MapRoute(
                "UploadLogo",
                "{tenantId}/MyAccount/UploadLogo",
                new { controller = "Account", action = "UploadLogo" });

            routes.MapRoute(
                "Authentication",
                "Account/{action}",
                new { controller = "Authentication" });
        }
    }
}
