using System.Net;
using System.Web;
using Guartinel.Kernel;
using Guartinel.Website.Admin.Persistance;
using Guartinel.Website.Common.Configuration;
using Guartinel.Website.Common.Tools;
using Guartinel.Website.Common;
using Guartinel.Website.Common.Connection;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Website.Admin {
   public static class GuartinelApp  {
      private static GuartinelAdminWebSiteSettings _settings ;
      private static WebRequester _requester;
      private static string SETTINGS_PATH = HttpContext.Current.Server.MapPath ("~") + "AdminWebServerConfig.json" ;

      public static GuartinelAdminWebSiteSettings Settings {
         get {return _settings ;} }

      public static void OnStart() {
         _settings = (GuartinelAdminWebSiteSettings) Manager.Load (typeof (GuartinelAdminWebSiteSettings), SETTINGS_PATH) ;
         if (_settings == null) {
            Logger.Log("Settings are missing. Creating default one.");
            _settings = new GuartinelAdminWebSiteSettings() ;
            _settings.ResetDefaultValues() ;
            SaveSettings() ;
            return ;
         }

       _requester = new WebRequester (Settings);

         if (_settings.ManagementServer != null) {
            try {
               var loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester,Settings.ManagementServer,Settings.AdminAccount.Username, Settings.AdminAccount.PasswordHash) ;
               string token = loginRequest.Token ;
               _settings.ManagementServer.Token = token ;
               SaveSettings() ;
            } catch (CoreException e) {
               Logger.Error ($"Cannot login in MS to obtain token for admin communication{e.GetAllMessages()}") ;
            }
         }
      }
      public static WebRequester WebRequester {get {return _requester ;} }

      public static  void SaveSettings() {
         Manager.Save (Settings, SETTINGS_PATH) ;
      }
   }
}
