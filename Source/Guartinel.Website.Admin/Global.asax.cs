using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Guartinel.Kernel.Logging;
using System.IO;
using Guartinel.Kernel.Logging ;

namespace Guartinel.Website.Admin {
   public class MvcApplication : System.Web.HttpApplication {
     
      protected void Application_Start() {
         AreaRegistration.RegisterAllAreas() ;
         GlobalConfiguration.Configure (WebApiConfig.Register) ;

         FilterConfig.RegisterGlobalFilters (GlobalFilters.Filters) ;
         RouteConfig.RegisterRoutes (RouteTable.Routes) ;
         BundleConfig.RegisterBundles (BundleTable.Bundles) ;

         GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear() ;
         bool isDebugLogEnabled = Properties.Config.Default.ENABLE_DEBUG_LOG;
         string path = Path.Combine(HttpContext.Current.Server.MapPath("~"), "Log");

         // Logger.Setup<SimpleFileLogger, SimpleWindowsEventLogger>( "GuartinelAdminWebsite", "Guartinel Admin Website",true);
         Logger.Setup<SimpleFileLogger>("GuartinelAdminWebsite", "Guartinel Admin Website");
         Logger.SetSetting(FileLogger.Constants.SETTING_NAME_FOLDER, Properties.Config.Default.LOG_FOLDER);

         GuartinelApp.OnStart() ;
        // LogWrapper.SetDebugLogEnabled (Properties.Config.Default.ENABLE_DEBUG_LOG) ;
         //SettingsStore.Initialize() ;
      }

      private void Application_BeginRequest (object sender,
            EventArgs e) {
         Response.Cache.SetCacheability (HttpCacheability.NoCache) ;
         Response.Cache.SetExpires (DateTime.UtcNow) ;
         Response.Cache.SetNoStore() ;
         Response.Cache.SetMaxAge (new TimeSpan (0, 0, 30)) ;
      }
   }
}
