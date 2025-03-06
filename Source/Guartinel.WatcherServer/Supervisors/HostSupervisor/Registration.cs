using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Supervisors.HostSupervisor {
   public static class Registration {
      public static void Register() {
         IoC.Use.Multi.Register<Package, HostSupervisorPackage> (HostSupervisorPackage.Constants.CREATOR_IDENTIFIERS) ;
         //Factory.Use.RegisterCreator (HostChecker.GetCreator(Factory.Use.CreateInstance<IMeasuredDataStore>())) ;
      }

      public static void Unregister() {
         //Factory.Use.UnregisterCreators<HostSupervisorPackage>() ;
         //Factory.Use.UnregisterCreators<HostChecker>() ;
      }
   }
}
