using System ;
using Guartinel.Communication.Supervisors.ApplicationSupervisor ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Communication.Routes ;
using Guartinel.WatcherServer.InstanceData ;
using Guartinel.WatcherServer.Packages ;
using Newtonsoft.Json.Linq ;
using WSCheckResultsConstants = Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings.WatcherServerRoutes.RegisterResult.Request ;

namespace Guartinel.WatcherServer.Supervisors.ApplicationSupervisor {
   public class RegisterResultRoute : PackageRoute {
      public RegisterResultRoute (PackageController packageController,
                                  IMeasuredDataStore measuredDataStore) : base (packageController, measuredDataStore) { }

      public override string Path => Strings.WatcherServerRoutes.RegisterResult.FULL_URL ;

      protected void StoreMeasurement (string packageID,
                                       string instanceID,
                                       DateTime measurementTimestamp,
                                       string success,
                                       XString message,
                                       XString details,
                                       string[] tags) {
         JObject measuredData = new JObject() ;
         measuredData.Add (WSCheckResultsConstants.INSTANCE_ID, instanceID) ;
         measuredData.Add (WSCheckResultsConstants.CheckResult.SUCCESS, success) ;
         measuredData.Add (WSCheckResultsConstants.CheckResult.RESULT, success) ;
         measuredData.Add (WSCheckResultsConstants.CheckResult.MESSAGE, message?.AsJObject()) ;
         measuredData.Add (WSCheckResultsConstants.CheckResult.DETAILS, details?.AsJObject()) ;

         var logger = new TagLogger(tags);

         logger.InfoWithDebug ($"Mesaured data is stored for package '{packageID}'.", measuredData.ConvertToLog()) ;

         _measuredDataStore?.StoreMeasuredData (packageID,
                                                WSCheckResultsConstants.CheckResult.TYPE_VALUE,
                                                measurementTimestamp,
                                                measuredData) ;
      }

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {
         // System.IO.File.WriteAllText (@"c:\temp\RegisterResult.json", parameters.Data.ToString());

         CheckToken (parameters) ;

         if (results == null) return ;

         string packageID = parameters [WSCheckResultsConstants.PACKAGE_ID] ;
         ConfigurationData checkResult = parameters.GetChild (WSCheckResultsConstants.CHECK_RESULT) ?? new ConfigurationData() ;
         var instanceID = parameters [WSCheckResultsConstants.INSTANCE_ID] ;
         var instanceName = parameters [WSCheckResultsConstants.INSTANCE_NAME] ;
         bool isHeartbeat = parameters.AsBoolean (WSCheckResultsConstants.IS_HEARTBEAT) ;

         if (isHeartbeat) {
            checkResult.AsJObject [WSCheckResultsConstants.IS_HEARTBEAT] = true ;
         }

         //_packageController.UsePackage (packageID, package => {
         //   if (!(package is ApplicationSupervisorPackage)) return ;

         //   // Debug.WriteLine ($"checkResult: {checkResult}") ;
         //   if (!string.IsNullOrEmpty (instanceID)) {
         //      package.CastTo<ApplicationSupervisorPackage>().RegisterInstanceData (instanceID, instanceName, checkResult, isHeartbeat) ;
         //   }
         //}) ;

#warning SzTZ: Messagebus: send/post, order?

         logger.InfoWithDebug ($"Instance data posted. ID: {instanceID}. Name: {instanceName}", checkResult.AsJObject.ConvertToLog()) ;

         MessageBus.Use.Post (packageID, new InstanceDataMessage (instanceID, instanceName, checkResult)) ;

         StoreMeasurement (packageID, instanceID,
                           parameters.AsDateTime (WSCheckResultsConstants.MEASUREMENT_TIMESTAMP),
                           !string.IsNullOrEmpty (checkResult [WSCheckResultsConstants.CheckResult.RESULT]) ?
                                    checkResult [WSCheckResultsConstants.CheckResult.RESULT] :
                                    checkResult [WSCheckResultsConstants.CheckResult.SUCCESS],
                           new XSimpleString (checkResult [WSCheckResultsConstants.CheckResult.MESSAGE]),
                           new XSimpleString (checkResult [WSCheckResultsConstants.CheckResult.DETAILS]),
                           logger.Tags) ;

         results.Success() ;
      }
   }
}