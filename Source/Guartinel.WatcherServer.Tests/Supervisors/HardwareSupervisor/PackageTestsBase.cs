using System;
using System.Linq;
using Guartinel.Communication;
using Guartinel.Kernel.Utility;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor;
using Guartinel.WatcherServer.Tests.Packages;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.RegisterMeasurement.Request;
using Strings = Guartinel.Communication.Strings.Strings;
using Timeout = Guartinel.Kernel.Timeout;

namespace Guartinel.WatcherServer.Tests.Supervisors.HardwareSupervisor {
   public class PackageTestsBase : HttpPackageTestsBase {
      protected new static class Constants {
         public const int STARTUP_DELAY_SECONDS = 2 ;
         public const string INSTANCE_ID = "101" ;
         public const string INSTANCE_NAME = "testehune34" ;
         public const string NAMED_DETAILS = @"""code"":""HARDWARE_SUPERVISOR.NamedMeasurementAlertDetails""" ;
         public const string DETAILS = @"""code"":""HARDWARE_SUPERVISOR.MeasurementAlertDetails""" ;
         public const string SENSITIVE_DETAILS = @"""code"":""HARDWARE_SUPERVISOR.MeasurementSensitiveAlertDetails""" ;
      }

      protected string SavePackage (string token,
                                    DateTime modificationDate,
                                    Action<JObject> configure,
                                    string packageName,
                                    string packageID,
                                    bool addMailAlerts,
                                    Schedules schedules,                                    
                                    int checkIntervalSeconds = 15,
                                    int timeoutIntervalSeconds = 25,
                                    int startupDelaySeconds = 1,
                                    bool checkDetails = true) {

         return SavePackageX (token, Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Use.PackageType,
                             modificationDate,
                             configure,
                             packageName, packageID, addMailAlerts,
                             schedules,
                             checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,                             
                             checkDetails,
                             packageX => {
                                HardwareSupervisorPackage package = packageX as HardwareSupervisorPackage ;
                                Assert.IsNotNull (package) ;

                                var checker = package.CreateCheckers() [0].CastTo<HardwareInstanceDataChecker>() ;
                                Assert.AreEqual (packageName, checker.Name) ;
                             }) ;
      }

      protected string SavePackage (string token,
                                    JObject instance,                                    
                                    int checkIntervalSeconds = 15,
                                    int timeoutIntervalSeconds = 30,
                                    int startupDelaySeconds = Constants.STARTUP_DELAY_SECONDS) {
         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, instance),
                                      "packageName1", Guid.NewGuid().ToString(), true, null,
                                      checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                                      false) ;
         CheckStatus (token, 1) ;

         return packageID ;
      }

      public static JObject CreateMeasuredData (string token,
                                                string packageID,
                                                string instanceID,
                                                string instanceName,
                                                double? a0,
                                                bool dummy,
                                                double? a0Min = null,
                                                double? a0Max=null,
                                                HardwareCheckBoolean? d1 = null,
                                                HardwareCheckBoolean? d1Min = null,
                                                HardwareCheckBoolean? d1Max = null,
                                                HardwareCheckBoolean? d2 = null,
                                                HardwareCheckBoolean? d2Min = null,
                                                HardwareCheckBoolean? d2Max = null,
                                                HardwareCheckBoolean? d3 = null,
                                                HardwareCheckBoolean? d3Min = null,
                                                HardwareCheckBoolean? d3Max = null) {
         JObject measuredData = new JObject() ;
         measuredData [MeasuredDataConstants.TOKEN] = token ;

         JArray packageIDs = new JArray();
         packageIDs.Add (packageID) ;
         measuredData.Add (MeasuredDataConstants.PACKAGE_IDS, packageIDs) ;
         measuredData.Add (MeasuredDataConstants.INSTANCE_ID, instanceID) ;
         measuredData.Add (MeasuredDataConstants.INSTANCE_NAME, instanceName) ;
         measuredData.Add (MeasuredDataConstants.MEASURED_DATA, Configuration.CreateMeasuredDataMinMax (a0, a0Min, a0Max,
                                                                                                        d1, d1Min, d1Max,
                                                                                                        d2, d2Min, d2Max,
                                                                                                        d3, d3Min, d3Max)) ;
         measuredData.Add (MeasuredDataConstants.MEASUREMENT_TIMESTAMP, DateTime.UtcNow) ;

         return measuredData ;
      }

      protected void SendMeasuredData (JObject measuredData) {
         var result = new Communication.Parameters (SendPostToServer ("hardwareSupervisor/registerMeasurement", measuredData)) ;

         Assert.AreEqual (Strings.AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS],
                          result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
      }

      protected void SendResultsRunCheck (JObject instance,
                                          JObject measuredData,
                                          bool isAlerting,
                                          string[] containingInMesssages = null,
                                          string[] containingInDetails = null) {

         SendMeasuredData (measuredData) ;
         var instanceID = measuredData [MeasuredDataConstants.INSTANCE_ID].ToString() ;
         string[] packageIDs = measuredData.AsStringArray (MeasuredDataConstants.PACKAGE_IDS) ;         
         Assert.Greater (packageIDs.Length, 0) ;

         const string ALERT = @"HARDWARE_SUPERVISOR.MeasurementAlertMessage";

         var message = $@"""code"":""{ALERT}""" ;

         // Wait a bit
         if (isAlerting) {
            // new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + 3)).WaitFor (() => (ManagementServer.DeviceAlerts.Count >= 2) &&
            new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + 3000)).WaitFor (() => (ManagementServer.DeviceAlerts.Count >= 2) &&
                                                                                                       (ManagementServer.MailAlerts.Count >= 3)) ;

            Assert.AreEqual (2, ManagementServer.DeviceAlerts.Count) ;
            Assert.AreEqual (3, ManagementServer.MailAlerts.Count) ;

            Assert.IsTrue (ManagementServer.DeviceAlerts.Exists (a => a.Message.Contains (ALERT))) ;
            Assert.IsTrue (ManagementServer.MailAlerts.Exists (a => a.Message.Contains (ALERT))) ;
         } else {
            var timeout = new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + 3)) ;
            timeout.WaitFor (() => (ManagementServer.MeasuredDataList.Count (x => packageIDs.Contains (x.PackageID)) > 0)) ;

            new Timeout (TimeSpan.FromSeconds (3)).Wait() ;
         }

         Assert.GreaterOrEqual (ManagementServer.MeasuredDataList.Count, 1) ;

         ManagementServer.MeasuredDataList.TryPeek (out var mockMeasuredData) ;
         JObject measuredDataReturned = JObject.Parse (mockMeasuredData.Data) ;

         Assert.AreEqual (instanceID, measuredDataReturned ["instance_id"].ToString()) ;

         _watcherServer.PackageController.UsePackage (packageIDs [0], package => {
            HardwareSupervisorPackage testPackage = package as HardwareSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            // checkResult = package.CreateCheckers() [0].Check() [0] ;
            //if (isAlerting) {
            //   Assert.AreEqual (CheckResultSuccess.Fail, checkResult.Success) ;
            //} else {
            //   Assert.AreEqual (CheckResultSuccess.Success, checkResult.Success) ;
            //}

            // Assert.IsTrue (checkResult1.Message.ToJsonString().Contains ("12345678")) ;
            // No package name anymore

            // The result should be an array
            // This has been changed to a constant string
            // Assert.IsAssignableFrom<XStrings> (checkResult1.Message) ;            
            //var messages = (XStrings) checkResult1.Message ;
            //Assert.AreEqual (2, messages.Values.Count) ;
         }) ;

         if (isAlerting) {
            var alert = ManagementServer.DeviceAlerts.Last() ;
            Assert.IsFalse (alert.Message.Contains ("packageName1")) ;

            Assert.IsTrue (alert.Message.Contains (message), alert.Message) ;

            Assert.IsTrue (alert.Details.Contains ("ReferenceValue"), alert.Details) ;
            Assert.IsTrue (alert.Details.Contains ("MeasuredValue"), alert.Details) ;
         }

         if (containingInMesssages != null || containingInDetails != null) {
            Assert.Greater (ManagementServer.DeviceAlerts.Count, 0) ;
            var alertMessage = ManagementServer.DeviceAlerts.Last().Message ;
            var alertDetails = ManagementServer.DeviceAlerts.Last().Details ;

            if (containingInMesssages != null) {
               foreach (var text in containingInMesssages) {
                  Assert.IsTrue (alertMessage.Contains (text), $"Missing '{text}' in: {alertMessage}") ;
               }
            }

            if (containingInDetails != null) {
               foreach (var text in containingInDetails) {
                  Assert.IsTrue (alertDetails.Contains (text), $"Missing '{text}' in: {alertDetails}") ;
               }
            }
         }
      }
   }
}