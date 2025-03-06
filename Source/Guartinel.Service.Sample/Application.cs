using System;
using System.Linq.Expressions ;
using System.Text;
using Guartinel.Kernel;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.MessageQueues ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.Sample {
   public class Application : IDisposable {
      public static class Constants {
         public const string NAME = "Sample.1.0" ;
      }

      private readonly MessageConnection _queueConnection = new MessageConnection();
      private ServiceServer _server ;

      public Application() {         
      }

      public void Run() {
         Logger.RegisterLogger<SimpleConsoleLogger>() ;

         bool initialized ;
         do {
            try {
               _server = _queueConnection.CreateServiceServer (Constants.NAME, ProcessRequests) ;
               initialized = true ;
            } catch (Exception e) {
               // Log and try again
               Logger.Error (e.GetAllMessages()) ;
               initialized = false ;
            }
         } while (!initialized) ;
      }

      private JObject ProcessRequests (JObject request) {
         try {
            var website = new Website (request ["address"].ToString()) ;
            WebsiteDownloadResult result = new SiteDownloader().DownloadSite (website, SiteDownloadKind.Chrome) ;
            return result.ToJObject() ;
         } catch (Exception e) {
            var result = new JObject();
            result ["message"] = e.Message ;
            result ["message_details"] = e.GetAllMessages() ;
            return result ;
         }
      }

      public void Dispose() {
         _server.Dispose();
         _queueConnection.Dispose() ;
      }
   }
}