using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Network ;
using Guartinel.WatcherServer.Supervisors.HostSupervisor ;
using Newtonsoft.Json.Linq ;
using SaveRequest = Guartinel.Communication.Supervisors.HostSupervisor.Strings.WatcherServerRoutes.Save.Request ;

namespace Guartinel.WatcherServer.Tests.Supervisors.HostSupervisor {
   public static class Configuration {
      public static void ConfigureChecker (HostChecker checker,
                                           string packageID,
                                           Host host,
                                           int? retryCount = 4,
                                           int? waitTimeSeconds = 5) {

         checker.Configure ("checker1", packageID, host, retryCount, waitTimeSeconds) ;
      }

      public static void CreatePackageConfiguration (JObject configuration,
                                                     List<Host> hosts,
                                                     int? retryCount = 4,
                                                     int? waitTimeSeconds = 5) {
         if (configuration == null) return ;

         // hosts[] helyett lett detailed_hosts:[{"address":"8.8.8.8","caption":"DNS"}]

         JArray hostsArray = new JArray() ;
         foreach (var host in hosts) {
            JObject hostJObject = new JObject();
            hostJObject [SaveRequest.DETAILED_HOST_ADDRESS] = host.Address ;
            hostJObject[SaveRequest.DETAILED_HOST_CAPTION] = host.Caption ;
            hostsArray.Add (hostJObject) ;
         }
         configuration [SaveRequest.DETAILED_HOSTS] = hostsArray ;         

         if (retryCount != null) {
            configuration [SaveRequest.RETRY_COUNT] = retryCount ;
         }

         if (waitTimeSeconds != null) {
            configuration [SaveRequest.WAIT_TIME_SECONDS] = waitTimeSeconds ;
         }         
      }
   }
}
