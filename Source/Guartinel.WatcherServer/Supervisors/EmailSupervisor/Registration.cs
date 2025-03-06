using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Supervisors.EmailSupervisor {
   public static class Registration {
      public static void Register() {
         IoC.Use.Multi.Register<Package, EmailSupervisorPackage> (EmailSupervisorPackage.Constants.CREATOR_IDENTIFIERS) ;
         //Factory.Use.RegisterCreator(WebsiteChecker.GetCreator(Factory.Use.CreateInstance<IMeasuredDataStore>()));
      }

      //public static void Unregister() {
      //   Factory.Use.UnregisterCreators<WebsiteSupervisorPackage>();
      //   Factory.Use.UnregisterCreators<WebsiteChecker>();
      //}
   }
}
