using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class ConfirmDeviceAlertRoute : PackageRoute, IDisposable {

      public ConfirmDeviceAlertRoute (PackageController packageController) : base (packageController) {
      }

      public override string Path => WatcherServerAPI.Admin.ConfirmDeviceAlert.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {

         CheckToken (parameters) ;

         // Search for package
         var packageID = parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.PACKAGE_ID] ;
         var instanceID = parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.INSTANCE_ID] ;
         var alertID = parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.ALERT_ID] ;
         var deviceID = parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.DEVICE_ID] ;

         _packageController.UsePackage (packageID, package => {
            package.ConfirmDeviceAlert (instanceID, alertID, logger.Tags) ;
         }) ;

         results.Success() ;
      }

      public void Dispose() {         
      }
   }
}