using System ;
using System.Linq ;
using Guartinel.Communication.Supervisors.WebsiteSupervisor ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Communication.Routes ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Supervisors.WebsiteSupervisor {
   public class RegisterResultRoute : PackageRoute {
      public RegisterResultRoute (PackageController packageController,
                                  IMeasuredDataStore measuredDataStore) : base (packageController, measuredDataStore) { }

      public override string Path => Strings.WatcherServerRoutes.RegisterResult.FULL_URL ;

      protected void StoreMeasurement (string packageID,
                                       string instanceID,
                                       DateTime measurementTimestamp,
                                       string success,
                                       XString message,
                                       XString details) {
         //JObject measurement = new JObject();
         //measurement.Add(WSCheckResultsConstants.INSTANCE_ID, instanceID);
         //measurement.Add(WSCheckResultsConstants.CheckResult.SUCCESS, success);
         //measurement.Add(WSCheckResultsConstants.CheckResult.MESSAGE, message?.ToJObject());
         //measurement.Add(WSCheckResultsConstants.CheckResult.DETAILS, details?.ToJObject());

         //_measuredDataStore?.StoreData(packageID,
         //                                    WSCheckResultsConstants.CheckResult.TYPE_VALUE,
         //                                    measurementTimestamp,
         //                                    measurement);
      }

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {
         // System.IO.File.WriteAllText (@"c:\temp\RegisterResult.json", parameters.Data.ToString());

         CheckToken (parameters) ;

         if (results == null) return ;

         //string packageID = parameters[WSRegisterResultConstants.PACKAGE_ID];
         //ConfigurationData checkResult = parameters.GetChild(WSRegisterResultConstants.CHECK_RESULT) ?? new ConfigurationData();
         //var website = parameters[WSRegisterResultConstants.WEBSITE_ADDRESS] ;
         //var instanceName = parameters[WSRegisterResultConstants.INSTANCE_NAME] ;

         //_packageController.UsePackage(packageID, package => {
         //   if (!(package is ApplicationSupervisorPackage)) return;

         //   // Debug.WriteLine ($"checkResult: {checkResult}") ;
         //   if (!string.IsNullOrEmpty (instanceID)) package.CastTo<WebsiteSupervisorPackage>().RegisterWebsiteCheckResult (instanceID, instanceName, checkResult) ;
         //});

         //StoreData(packageID, instanceID,
         //                 parameters.AsDateTime(WSCheckResultsConstants.MEASUREMENT_TIMESTAMP),
         //                 checkResult[WSCheckResultsConstants.CheckResult.SUCCESS],
         //                 new XConstantString(checkResult[WSCheckResultsConstants.CheckResult.MESSAGE]),
         //                 new XConstantString(checkResult[WSCheckResultsConstants.CheckResult.DETAILS]));

         results.Success() ;
      }
   }
}