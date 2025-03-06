using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Packages ;
using Newtonsoft.Json.Linq ;
using SaveRequestConstants = Guartinel.Communication.Supervisors.HostSupervisor.Strings.WatcherServerRoutes.Save.Request ;

namespace Guartinel.WatcherServer.Supervisors.HostSupervisor {
   public class HostSupervisorPackage : Package {
      public new static class Constants {
         public const string CAPTION = "Host Supervisor Package" ;
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> {Guartinel.Communication.Supervisors.HostSupervisor.Strings.Use.PackageType} ;
      }

      public HostSupervisorPackage() {}

      //public new static Creator GetCreator() {
      //   return new Creator<Package, HostSupervisorPackage> (() => new HostSupervisorPackage(), Constants.CAPTION, Constants.CREATOR_IDENTIFIERS) ;
      //}

      private List<Host> _hosts = new List<Host>() ;
      private int? _retryCount ;
      private int? _waitTimeSeconds ;

      protected override void SpecificConfigure (ConfigurationData configuration) {
         // var hostJObjects = configuration.AsJObject [SaveRequestConstants.DETAILED_HOSTS].Select (x => (JObject) x) ;
         // _hosts = hostJObjects.Select (x => new Host (x.GetValue (SaveRequestConstants.DETAILED_HOST_ADDRESS, string.Empty),
         //                                              x.GetValue (SaveRequestConstants.DETAILED_HOST_CAPTION))).ToList() ;

         var hosts = configuration.AsArray (SaveRequestConstants.DETAILED_HOSTS) ;

         _hosts = hosts.Select (x => new Host (x.AsString (SaveRequestConstants.DETAILED_HOST_ADDRESS),
                                               x.AsString (SaveRequestConstants.DETAILED_HOST_CAPTION))).ToList() ;

         _retryCount = configuration.AsIntegerNull (SaveRequestConstants.RETRY_COUNT) ;
         _waitTimeSeconds = configuration.AsIntegerNull (SaveRequestConstants.WAIT_TIME_SECONDS) ;

         SetInstances (_hosts.Select (x => x.Address).ToList()) ;
      }

      protected override List<Checker> CreateCheckers1() {
         return _hosts.Select (host => (Checker) new HostChecker (
                                                                  IoC.Use.Single.GetInstance<IMeasuredDataStore>()).Configure (Name,
                                                                                                    ID,
                                                                                                    host,
                                                                                                    _retryCount,
                                                                                                    _waitTimeSeconds)).ToList() ;

      }
   }
}