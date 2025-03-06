using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.WatcherServer.Communication;
using Guartinel.WatcherServer.Communication.ManagementServer;
using Guartinel.WatcherServer.Communication.Routes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Communication.Supervisors.HardwareSupervisor ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.InstanceData ;
using Guartinel.WatcherServer.Packages ;
using WSCheckResultsConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.RegisterMeasurement.Request;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public class RegisterMeasuredDataRoute : PackageRoute {
      public RegisterMeasuredDataRoute (PackageController packageController,
                                        IMeasuredDataStore measuredDataStore) : base (packageController, measuredDataStore) { }

      public override string Path => Strings.WatcherServerRoutes.RegisterMeasurement.FULL_URL ;

      protected void StoreMeasuredData (string packageID,
                                        string instanceID,
                                        DateTime timestamp,
                                        ConfigurationData data,
                                        string[] tags) {
         var logger = new TagLogger(tags);

         JObject measuredData = new JObject() ;
         measuredData.Add (WSCheckResultsConstants.INSTANCE_ID, instanceID) ;
         measuredData.Add (WSCheckResultsConstants.CheckResult.DATA, data.AsJObject) ;

         logger.InfoWithDebug ($"Mesaured data is stored for package '{packageID}'.", measuredData.ConvertToLog()) ;

         _measuredDataStore?.StoreMeasuredData (packageID,
                                                WSCheckResultsConstants.CheckResult.TYPE_VALUE,
                                                timestamp,
                                                measuredData) ;
      }

      protected override void ProcessRequest (Parameters parameters,
                                           Parameters results,
                                           TagLogger logger) {
         // System.IO.File.WriteAllText (@"c:\temp\RegisterResult.json", parameters.Data.ToString());

         CheckToken (parameters) ;

         if (results == null) return ;

         List<string> packageIDs = parameters.AsStringArray (WSCheckResultsConstants.PACKAGE_IDS).ToList() ;
         // If package IDs are not used, then try package ID
         if (!packageIDs.Any()) {
            packageIDs = new List<string> {parameters [WSCheckResultsConstants.PACKAGE_ID]} ;
         }

         ConfigurationData data = parameters.GetChild (WSCheckResultsConstants.MEASURED_DATA) ?? new ConfigurationData() ;
         var instanceID = parameters [WSCheckResultsConstants.INSTANCE_ID] ;
         var instanceName = parameters [WSCheckResultsConstants.INSTANCE_NAME] ;

         foreach (var packageID in packageIDs) {
            MessageBus.Use.Post (packageID, new InstanceDataMessage (instanceID, instanceName, data)) ;

            StoreMeasuredData (packageID, instanceID,
                               parameters.AsDateTime (WSCheckResultsConstants.MEASUREMENT_TIMESTAMP),
                               data,
                               logger.Tags) ;
         }

         results.Success() ;
      }
   }
}