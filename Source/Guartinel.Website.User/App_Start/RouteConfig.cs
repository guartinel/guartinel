using System ;
using System.Linq ;
using System.Web.Mvc ;
using System.Web.Routing ;

namespace Guartinel.Website.User {
   public class RouteConfig {
      public static void RegisterRoutes (RouteCollection routes) {
         routes.IgnoreRoute ("{resource}.axd/{*pathInfo}") ;

         // route every URL_BASE request to HomeController, so Angular will be loaded everytime
         // an URL_BASE request is made to the server (first visit, page refresh)
         routes.MapRoute ("Default", "{*anything}", new {controller = "Home", action = "Index"}) ;
      }
   }
}
