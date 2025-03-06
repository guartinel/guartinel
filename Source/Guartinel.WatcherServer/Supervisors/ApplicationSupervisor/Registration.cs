using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Supervisors.ApplicationSupervisor
{
   public static class Registration {
      public static void Register() {
         IoC.Use.Multi.Register<Package, ApplicationSupervisorPackage> (ApplicationSupervisorPackage.Constants.CREATOR_IDENTIFIERS) ;
         //Factory.Use.RegisterCreator (ApplicationInstanceDataChecker.GetCreator()) ;
      }

      public static void Unregister() {
         //Factory.Use.UnregisterCreators<ApplicationSupervisorPackage>() ;
         //Factory.Use.UnregisterCreators<ApplicationInstanceDataChecker>() ;
      }
   }
}
