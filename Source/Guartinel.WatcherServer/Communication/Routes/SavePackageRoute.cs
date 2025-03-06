using System;
using System.Collections.Generic ;
using System.Linq;
using System.Text;
using Guartinel.Communication;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Instances ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class SavePackageRoute : PackageRoute {
      public SavePackageRoute (PackageController packageController) : base (packageController) { }

      public override string Path => WatcherServerAPI.Packages.Save.FULL_URL ;

      private static void ConfigurePackage (Package package,
                                            Parameters parameters) {

         // Read configuration from request
         string packageID = parameters [WatcherServerAPI.Packages.Save.Request.PACKAGE_ID] ;

         string packageName = parameters [WatcherServerAPI.Packages.Save.Request.PACKAGE_NAME] ;
         var checkIntervalSeconds = Kernel.Utility.Converter.StringToInt (parameters [WatcherServerAPI.Packages.Save.Request.CHECK_INTERVAL_SECONDS],
                                                                          Package.Constants.DEFAULT_CHECK_INTERVAL_SECONDS) ;
         var timeoutIntervalSeconds = Kernel.Utility.Converter.StringToInt (parameters [WatcherServerAPI.Packages.Save.Request.TIMEOUT_INTERVAL_SECONDS],
                                                                            Package.Constants.NO_TIMEOUT_INTERVAL_SPECIFIED) ;

         var startupDelaySeconds = Kernel.Utility.Converter.StringToInt (parameters [WatcherServerAPI.Packages.Save.Request.STARTUP_DELAY_SECONDS],
                                                                         Package.Constants.DEFAULT_STARTUP_DELAY_SECONDS) ;         
         bool forcedDeviceAlert = parameters.AsBooleanNull (WatcherServerAPI.Packages.Save.Request.FORCED_DEVICE_ALERT) ?? Package.Constants.DEFAULT_FORCED_DEVICE_ALERT ;

         var categories = new Categories (parameters.AsStringArray (WatcherServerAPI.Packages.Save.Request.CATEGORIES)) ;
         // License license = new License (parameters [WatcherServerAPI.Packages.Save.Request.LICENSE]) ;
         string lastModificationTimeStampRaw = parameters [WatcherServerAPI.Packages.Save.Request.LAST_MODIFICATION_TIMESTAMP] ;
         if (string.IsNullOrEmpty (lastModificationTimeStampRaw)) throw new ServerException ("No timestamp is specified for package.") ;
         DateTime lastModificationTimeStamp = DateTime.Parse (lastModificationTimeStampRaw, null, System.Globalization.DateTimeStyles.RoundtripKind) ;

         ConfigurationData specificConfiguration = new ConfigurationData (parameters [WatcherServerAPI.Packages.Save.Request.CONFIGURATION]) ;

         // Create mail alerts
         List<string> alertEmails = parameters.AsStringList (WatcherServerAPI.Packages.Save.Request.ALERT_EMAILS) ;

         // Create device alerts
         List<string> alertDeviceIDs = parameters.AsStringList (WatcherServerAPI.Packages.Save.Request.ALERT_DEVICE_IDS) ;

         // Extract state and instance states
         var state = new ConfigurationData (parameters [WatcherServerAPI.Packages.Save.Request.STATE]) ;
         InstanceStateList instanceStates = new InstanceStateList() ;
         foreach (ConfigurationData instanceState in state.GetChildren (WatcherServerAPI.Packages.Save.Request.State.STATES)) {
            instanceStates.Add (instanceState [WatcherServerAPI.Packages.Save.Request.State.States.IDENTIFIER],
                                InstanceState.Create (instanceState)) ;
         }

         var disabledAlerts = new Schedules().Configure (new ConfigurationData (parameters [WatcherServerAPI.Packages.Save.Request.DISABLE_ALERTS])) ;

         package.Configure (packageID, packageName, categories,
                            alertEmails,
                            alertDeviceIDs,
                            checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                            forcedDeviceAlert,
                            lastModificationTimeStamp, specificConfiguration,
                            instanceStates, disabledAlerts) ;
      }

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {

         // System.IO.File.WriteAllText (@"c:\temp\SaveRequest.json", parameters.Data.ToString());
         CheckToken (parameters) ;

         string packageID = parameters [WatcherServerAPI.Packages.Save.Request.PACKAGE_ID] ;

         // Delete package first!
         _packageController.DeletePackage (packageID, logger.Tags) ;

         string packageType = parameters [WatcherServerAPI.Packages.Save.Request.PACKAGE_TYPE] ;


         Package package = IoC.Use.Multi.GetInstance<Package> (packageType) ;

         ConfigurePackage (package, parameters) ;
         _packageController.AddPackage (package) ;

         results.Success() ;
      }
   }
}