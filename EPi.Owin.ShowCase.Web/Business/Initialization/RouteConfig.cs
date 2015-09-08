using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace EPi.Owin.ShowCase.Web.Business.Initialization
{
    [InitializableModule]
    public class RouteConfig : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
               name: "Account",
               url: "account/{action}",
               defaults: new { controller = "Account", action = "Index" });
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}