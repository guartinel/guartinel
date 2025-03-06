using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Guartinel.Kernel.Logging;
using System.IO;

namespace Guartinel.Website.User {
   public class MvcApplication : HttpApplication {
      protected void Application_Start() {
         AreaRegistration.RegisterAllAreas() ;
         GlobalConfiguration.Configure (WebApiConfig.Register) ;

         FilterConfig.RegisterGlobalFilters (GlobalFilters.Filters) ;
         RouteConfig.RegisterRoutes (RouteTable.Routes) ;
         BundleConfig.RegisterBundles (BundleTable.Bundles) ;

         GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear() ;

         bool isDebugLogEnabled = Properties.Config.Default.ENABLE_DEBUG_LOG ;
         string path = Path.Combine (HttpContext.Current.Server.MapPath ("~"), "Log") ;

         // Logger.Setup<SimpleFileLogger, WindowsEventLogger>("GuartinelUserWebsite", "Guartinel User Website",Properties.Config.Default.ENABLE_DEBUG_LOG);
         Logger.Setup<SimpleFileLogger> ("GuartinelUserWebsite", "Guartinel User Website", isDebugLogEnabled ? LogLevel.Debug : LogLevel.Info) ;
         Logger.SetSetting (FileLogger.Constants.SETTING_NAME_FOLDER, Properties.Config.Default.LOG_FOLDER) ;

         if (isDebugLogEnabled) {
            Logger.Log ("Debug log is enabled.") ;
         } else {
            Logger.Log ("Debug log is disabled.") ;
         }

         GuartinelApp.OnStart() ;
      }

      private void Application_BeginRequest (object sender,
                                             EventArgs e) {
         Response.Cache.SetCacheability (HttpCacheability.NoCache) ;
         Response.Cache.SetExpires (DateTime.UtcNow) ;
         Response.Cache.SetNoStore() ;
         Response.Cache.SetMaxAge (new TimeSpan (0, 0, 30)) ;
      }

      private void Application_End (object sender,
                                    EventArgs e) {
         Logger.Log ($"Application_End called because: {e.ToString()} ") ;

      }
   }
}