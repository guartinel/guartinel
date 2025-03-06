using System;
using System.Web;
using Guartinel.Website.Common.Configuration;
using Guartinel.Website.Common.Connection;
using Guartinel.Website.User.Persistance;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel;
using Guartinel.Kernel.Utility ;
using Guartinel.Website.User.License.Invoicing.SzamlazzDotHu;
using Guartinel.Website.User.License.PaymentMethod.MyPayPal;
using Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters;
using Guartinel.Website.User.License.PaymentMethod;
using Guartinel.Website.User.License.Invoicing;

namespace Guartinel.Website.User {
   public static class GuartinelApp {
      private static GuartinelUserWebSiteSettings _settings;
      private static WebRequester _requester;
      private static readonly string SETTINGS_PATH = HttpContext.Current.Server.MapPath("~") + "UserWebServerSettings.json";

      public static GuartinelUserWebSiteSettings Settings => _settings ;

      private static readonly PaymentManager _paymentManager = new PaymentManager(new PayPalRest(PayPalMode.LIVE));
      public static PaymentManager PaymentManager => _paymentManager ;

      private static readonly InvoiceManager _invoiceManager = new InvoiceManager( new SzamlaAgent("teszt@sysment.hu", "syYGsqHn72nqsdId"));
      public static InvoiceManager InvoiceManager => _invoiceManager ;

      public static void OnStart () {
         Logger.Log("Website started");
         _settings = (GuartinelUserWebSiteSettings) Manager.Load(typeof(GuartinelUserWebSiteSettings), SETTINGS_PATH);
         if ( _settings == null ) {
            _settings = new GuartinelUserWebSiteSettings();
            _settings.ResetDefaultValues();
            SaveSettings();
            return;
         }
         _requester = new WebRequester(Settings);

         if ( _settings.ManagementServer != null ) {
            try {
               var loginRequest = new Common.Connection.IManagementServer.Admin.Login(GuartinelApp.WebRequester, Settings.ManagementServer, Settings.AdminAccount.Username, Settings.AdminAccount.PasswordHash);
               string token = loginRequest.Token;
               _settings.ManagementServer.Token = token;
               SaveSettings();
            } catch ( Exception e ) {
               Logger.Error($"Cannot login in MS to obtain token for admin communication Error: {e.GetAllMessages()}");
            }
         }
      }

      public static WebRequester WebRequester => _requester ;

      public static void SaveSettings () {
         Manager.Save(Settings, SETTINGS_PATH);
      }
   }
}
