using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class DeletePackageRoute : PackageRoute {

      public DeletePackageRoute (PackageController packageController) : base (packageController) { }

      public override string Path => WatcherServerAPI.Packages.Delete.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {
         CheckToken (parameters) ;

         string packageID = parameters [WatcherServerAPI.Packages.Delete.Request.PACKAGE_ID] ;
         
         _packageController.DeletePackage (packageID, logger.Tags) ;

         results.Success() ;
      }
   }
}