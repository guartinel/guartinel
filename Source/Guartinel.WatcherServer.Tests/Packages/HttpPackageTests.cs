using System;
using Guartinel.Communication;
using Guartinel.Kernel.Utility;
using Guartinel.WatcherServer.Alerts;
using Guartinel.WatcherServer.Checkers;
using Guartinel.WatcherServer.Communication;
using Guartinel.WatcherServer.Instances ;
using Guartinel.WatcherServer.Packages;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;

namespace Guartinel.WatcherServer.Tests.Packages
{
   public class HttpPackageTestsBase : HttpServerTestsBase {
      protected string SavePackageX (string token,
                                    string packageType,
                                    DateTime modificationDate,
                                    Action<JObject> configure,
                                    string packageName,
                                    string packageID,
                                    bool addMailAlerts,
                                    Schedules disabledAlerts,
                                    int checkIntervalSeconds = 2, 
                                    int timeoutIntervalSeconds = 3,
                                    int startupDelaySeconds = 1,
                                    bool checkDetails = true,
                                    Action<Package> check = null,
                                    InstanceStateList instanceStates = null) {
         JObject parameters = new JObject() ;
         parameters [WatcherServerAPI.Packages.Save.Request.TOKEN] = token ;

         parameters [WatcherServerAPI.Packages.Save.Request.PACKAGE_ID] = packageID ;
         parameters [WatcherServerAPI.Packages.Save.Request.PACKAGE_TYPE] = packageType ;
         parameters [WatcherServerAPI.Packages.Save.Request.PACKAGE_NAME] = packageName ;         
         parameters [WatcherServerAPI.Packages.Save.Request.CHECK_INTERVAL_SECONDS] = checkIntervalSeconds ;
         parameters [WatcherServerAPI.Packages.Save.Request.TIMEOUT_INTERVAL_SECONDS] = timeoutIntervalSeconds ;
         parameters [WatcherServerAPI.Packages.Save.Request.STARTUP_DELAY_SECONDS] = startupDelaySeconds ;

         parameters [WatcherServerAPI.Packages.Save.Request.LAST_MODIFICATION_TIMESTAMP] = Kernel.Utility.Converter.DateTimeToStringJson (modificationDate) ;
         parameters [WatcherServerAPI.Packages.Save.Request.DISABLE_ALERTS] = disabledAlerts?.ToConfigurationData().AsJObject ;

         if (addMailAlerts) {
            JArray alertEmails = new JArray() ;
            alertEmails.Add ("email1@email.hu") ;
            alertEmails.Add ("email2@email.hu") ;
            alertEmails.Add ("email3@email.hu") ;
            parameters.Add ("alert_emails", alertEmails) ;
         }

         if (true) {
            JArray alertDeviceIDs = new JArray() ;
            alertDeviceIDs.Add ("alertDevice1") ;
            alertDeviceIDs.Add ("alertDevice2") ;
            parameters.Add ("alert_device_ids", alertDeviceIDs) ;
         }

         if (configure != null) {
            JObject configuration = new JObject() ;
            parameters.Add ("configuration", configuration) ;

            configure (configuration) ;
         }

         if (instanceStates != null) {
            JArray states = new JArray();
            foreach (var instanceState in instanceStates) {
               var packagePartStateObject = new JObject();
               packagePartStateObject ["package_part_identifier"] = instanceState.Key ;
               packagePartStateObject ["package_part_state"] = instanceState.Value.Name.ToLowerInvariant() ;
               packagePartStateObject ["package_part_message"] = instanceState.Value.Message?.ToJsonString() ;
               packagePartStateObject ["package_part_details"] = instanceState.Value.Details?.ToJsonString() ;
               packagePartStateObject ["package_part_extract"] = instanceState.Value.Extract?.ToJsonString() ;

               states.Add (packagePartStateObject) ;
            }

            var state  = new JObject() ;
            state ["states"] = states ;
            parameters.Add ("state", state) ;
         }

         Parameters result = new Parameters (SendPostToServer (WatcherServerAPI.Packages.Save.FULL_URL, parameters)) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS],
                          result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;

         if (checkDetails) {
            //Parameters result = new Parameters (SendPostToServer (WatcherServerAPI.Packages.Save.FULL_URL, parameters)) ;
            //Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS],
            //                 result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;

            _watcherServer.PackageController.UsePackage (packageID, package => {
               Assert.AreEqual (packageID, package.ID) ;
               Assert.AreEqual (packageName, package.Name) ;

               Assert.AreEqual (startupDelaySeconds, package.StartupDelaySeconds) ;
               Assert.AreEqual (checkIntervalSeconds, package.CheckIntervalSeconds) ;

               Assert.AreEqual (2, package.AlertsByType<DeviceAlert>().Count) ;
               Assert.AreEqual (addMailAlerts ? 3 : 0, package.AlertsByType<MailAlert>().Count) ;

               var checker = package.CreateCheckers() [0].CastTo<Checker>() ;
               Assert.AreEqual (packageName, checker.Name) ;

               if (addMailAlerts) {
                  var alertEmails = package.AlertsByType<MailAlert>() ;
                  // Assert.AreEqual (packageName, mailAlert.Name) ;
                  Assert.AreEqual (3, alertEmails.Count) ;

                  Assert.AreEqual ("email1@email.hu", alertEmails [0].ToAddress) ;
                  Assert.AreEqual ("email2@email.hu", alertEmails [1].ToAddress) ;
               }
               var alertDeviceAlerts = package.AlertsByType<DeviceAlert>() ;
               Assert.AreEqual (2, alertDeviceAlerts.Count) ;

               Assert.AreEqual ("alertDevice1", alertDeviceAlerts [0].AlertDeviceID) ;
               Assert.AreEqual ("alertDevice2", alertDeviceAlerts [1].AlertDeviceID) ;

               check?.Invoke (package) ;
            }) ;
         //} else {
         //   SendPostToServerAsync (WatcherServerAPI.Packages.Save.FULL_URL, parameters) ;
         }

         return packageID ;
      }

      protected string SavePackageX (string token,
                                    DateTime modificationDate,
                                    Action<JObject> configure,
                                    string packageName,
                                    string packageID,
                                    bool addMailAlerts,
                                    int checkIntervalSeconds = 2,
                                    int timeoutIntervalSeconds = 3,
                                    int startupDelaySeconds = 1,                                    
                                    bool checkDetails = true,
                                    InstanceStateList instanceStates = null) {
         return SavePackageX (token, TestPackage.Constants.PACKAGE_TYPE,
                             modificationDate,
                             configure,
                             packageName, packageID, addMailAlerts,
                             null,
                             checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                             checkDetails,
                             package => {
                                Assert.AreEqual (1, package.CreateCheckers().Count) ;

                                var checker = package.CreateCheckers() [0].CastTo<TestChecker>() ;
                                Assert.AreEqual (packageName, checker.Name) ;
                                Assert.AreEqual (10, checker.MinValue) ;
                                Assert.AreEqual (20, checker.MaxValue) ;
                             },
                             instanceStates) ;
      }
   }

   [TestFixture]
   public class HttpPackageTests : HttpPackageTestsBase {
      [Test]
      public void StartHttpServer_Login_AddPackage_CheckResult() {
         StartServer() ;

         var token = Login() ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackageX (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x), "packageName1", packageID, true) ;
         CheckStatus (token, 1) ;
      }

      [Test]
      public void StartHttpServer_Login_AddPackage_RenamePackage_CheckResult() {
         StartServer() ;

         var token = Login() ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackageX (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x), "packageName1", packageID, true) ;
         CheckStatus (token, 1) ;

         // Save with the same ID
         SavePackageX (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x), "packageName2", packageID, true) ;
         CheckStatus (token, 1) ;
      }

      [Test]
      public void StartHttpServer_Login_AddAndDeletePackage_CheckResult() {
         StartServer() ;
         var token = Login() ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackageX (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x), "packageName1", packageID, true) ;
         CheckStatus (token, 1) ;

         JObject parameters = new JObject() ;
         parameters [WatcherServerAPI.Packages.Delete.Request.TOKEN] = token ;
         parameters [WatcherServerAPI.Packages.Delete.Request.PACKAGE_ID] = packageID ;

         Parameters result = new Parameters (SendPostToServer (WatcherServerAPI.Packages.Delete.FULL_URL, parameters)) ;
         Assert.AreEqual (AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS], result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;

         CheckStatus (token, 0, true) ;
      }

      [Test]
      public void StartHttpServer_CreatePackage_CheckTimestamps() {
         StartServer() ;

         var token = Login() ;

         var packageID = Guid.NewGuid().ToString() ;
         var modificationDate = Kernel.Utility.Converter.DateTimeToJsonDateTime (DateTime.UtcNow) ;
         SavePackageX (token, modificationDate, x => Configuration.CreatePackageConfiguration (x), "packageName1", packageID, true) ;
         CheckStatus (token, 1) ;

         JObject parameters = new JObject() ;
         parameters [WatcherServerAPI.Packages.GetAllWithTimeStamp.Request.TOKEN] = token ;

         Parameters result = new Parameters (SendPostToServer ("packages/getAllWithTimeStamp", parameters)) ;
         string[] timeStamps = result.AsStringArray (WatcherServerAPI.Packages.GetAllWithTimeStamp.Response.TIMESTAMPS) ;

         Assert.AreEqual (1, timeStamps.Length) ;
         Parameters timeStamp = new Parameters (timeStamps [0]) ;

         Assert.AreEqual (packageID, timeStamp [WatcherServerAPI.Packages.GetAllWithTimeStamp.Response.Timestamp.PACKAGE_ID]) ;
         // Compare as date and as string
         Assert.AreEqual (modificationDate,
                          timeStamp.AsDateTime (WatcherServerAPI.Packages.GetAllWithTimeStamp.Response.Timestamp.MODIFICATION_TIMESTAMP)) ;
         Assert.AreEqual (Kernel.Utility.Converter.DateTimeToStringJson (modificationDate),
                          Kernel.Utility.Converter.DateTimeToStringJson (timeStamp.AsDateTime (WatcherServerAPI.Packages.GetAllWithTimeStamp.Response.Timestamp.MODIFICATION_TIMESTAMP))) ;
      }
   }
}
