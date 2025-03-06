using System ;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Packages ;
using Guartinel.WatcherServer.Supervisors.ApplicationSupervisor ;
using Guartinel.WatcherServer.Tests.Packages ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;
using CheckResultsRequestConstants = Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings.WatcherServerRoutes.RegisterResult.Request ;
using Strings = Guartinel.Communication.Strings.Strings ;

namespace Guartinel.WatcherServer.Tests.Supervisors.ApplicatonSupervisor {
   [TestFixture]
   public class PackageTests : HttpPackageTestsBase {
      protected string SavePackage (string token,
                                    DateTime modificationDate,
                                    Action<JObject> configure,
                                    string packageName,
                                    string packageID,
                                    List<string> applicationInstanceIds,
                                    bool addMailAlerts,
                                    Schedules schedules,                                    
                                    int checkIntervalSeconds = 2,
                                    int timeoutIntervalSeconds = 3,
                                    int startupDelaySeconds = 1,
                                    bool checkDetails = true) {
         ManagementServer.SetApplicationInstanceIDs (packageID, applicationInstanceIds) ;

         return SavePackageX (token, Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings.Use.PackageType,
                             modificationDate,
                             configure,
                             packageName, packageID, addMailAlerts,
                             schedules,                             
                             checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                             checkDetails,
                             packageX => {
                                ApplicationSupervisorPackage package = packageX as ApplicationSupervisorPackage ;
                                Assert.IsNotNull (package) ;
                             }) ;
      }

      protected string SavePackage (string token,
                                    DateTime modificationDate,
                                    Action<JObject> configure,
                                    string packageName,
                                    string packageID,
                                    List<string> applicationInstanceIds,
                                    Schedules schedules = null) {
         ManagementServer.SetApplicationInstanceIDs (packageID, applicationInstanceIds) ;

         return SavePackage (token,
                             modificationDate,
                             configure,
                             packageName, packageID,
                             applicationInstanceIds,
                             false, null) ;
      }

      public static JObject CreateCheckResults (CheckResultKind checkResultKind,
                                                string token,
                                                string packageID,
                                                bool isHeartbeat,
                                                string instanceID = "100",
                                                string instanceName = "instanceName100",
                                                string message = "",
                                                string details = "") {
         JObject parameters = new JObject() ;
         parameters [CheckResultsRequestConstants.TOKEN] = token ;

         parameters.Add (CheckResultsRequestConstants.PACKAGE_ID, packageID) ;
         parameters.Add (CheckResultsRequestConstants.INSTANCE_ID, instanceID) ;
         parameters.Add (CheckResultsRequestConstants.INSTANCE_NAME, instanceName) ;
         parameters.Add (CheckResultsRequestConstants.IS_HEARTBEAT, isHeartbeat) ;
         parameters.Add (CheckResultsRequestConstants.CHECK_RESULT, Configuration.CreateCheckResult (checkResultKind, message, details)) ;
         parameters.Add (CheckResultsRequestConstants.MEASUREMENT_TIMESTAMP, DateTime.UtcNow) ;

         return parameters ;
      }

      protected void SendCheckResults (CheckResultKind checkResultKind,
                                       string token,
                                       string packageID,
                                       bool isHeartbeat = true,
                                       string instanceID = "100",
                                       string instanceName = "instanceName100",
                                       string message = "",
                                       string details = "") {
         JObject parameters = CreateCheckResults (checkResultKind, token, packageID, isHeartbeat, instanceID, instanceName, message, details) ;
         ManagementServer.RegisterApplicationInstanceID (packageID, instanceID) ;

         var result = new Parameters (SendPostToServer ("applicationSupervisor/registerResult", parameters)) ;

         Assert.AreEqual (Strings.AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS],
                          result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
      }

      [Test]
      public void CreateSetup_AddPackage_SendAlertingResults_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1", Guid.NewGuid().ToString(),
                                      new List<string> {instanceID1}, true, null, 4, 6, 1) ;
         CheckStatus (token, 1) ;

         SendCheckResults (CheckResultKind.Fail, token, packageID, true, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds (10)).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2) &&
                                                                 (ManagementServer.MailAlerts.Count == 3)) ;

         Assert.AreEqual (2, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (3, ManagementServer.MailAlerts.Count) ;

         Assert.IsTrue (ManagementServer.DeviceAlerts.Exists (a => a.Message.Contains ("APPLICATION_SUPERVISOR.ApplicationMeasurementAlertMessage"))) ;
         Assert.IsTrue (ManagementServer.MailAlerts.Exists (a => a.Message.Contains ("APPLICATION_SUPERVISOR.ApplicationMeasurementAlertMessage"))) ;

         Assert.GreaterOrEqual (ManagementServer.MeasuredDataList.Count, 1) ;

         ManagementServer.MeasuredDataList.TryPeek (out ManagementServerMock.MeasuredData mockMeasurement) ;
         JObject measurement = JObject.Parse (mockMeasurement.Data) ;

         Assert.AreEqual (JTokenType.String, (measurement [CheckResultsRequestConstants.CheckResult.RESULT]).Type) ;
      }

      [Test]
      public void CreateSetup_AddPackage_SendWarningAndCriticalResults_RunCheck () {
         StartServer();

         var token = Login();

         var instanceID1 = "101";
         var instanceName1 = "name101";
         var message = "Test Message 1";
         var details = "Test Details 1";

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1", Guid.NewGuid().ToString(),
                                      new List<string> {instanceID1}, true, null, 4, 15, 1) ;
         CheckStatus(token, 1);

         SendCheckResults (CheckResultKind.WarningFail, token, packageID, true, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Kernel.Timeout(TimeSpan.FromSeconds(10)).WaitFor(() => (ManagementServer.DeviceAlerts.Count == 2) &&
                                                              (ManagementServer.MailAlerts.Count == 3));

         Assert.AreEqual(2, ManagementServer.DeviceAlerts.Count);
         Assert.AreEqual(3, ManagementServer.MailAlerts.Count);

         Assert.IsTrue(ManagementServer.DeviceAlerts.Exists(a => a.Message.Contains("APPLICATION_SUPERVISOR.ApplicationMeasurementWarningMessage")));
         Assert.IsTrue(ManagementServer.MailAlerts.Exists(a => a.Message.Contains("APPLICATION_SUPERVISOR.ApplicationMeasurementWarningMessage")));

         Assert.GreaterOrEqual(ManagementServer.MeasuredDataList.Count, 1);

         ManagementServer.MeasuredDataList.TryPeek(out ManagementServerMock.MeasuredData mockMeasurement);
         JObject measurement = JObject.Parse(mockMeasurement.Data);

         Assert.AreEqual(JTokenType.String, (measurement[CheckResultsRequestConstants.CheckResult.RESULT]).Type);
      }

      [Test]
      public void CreateSetup_JustWait_CheckIfTimeoutAlertSent() {
         const int TIMEOUT_IN_SECONDS = 3 ;
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 1, TIMEOUT_IN_SECONDS, 1, false) ;
         CheckStatus (token, 1) ;

         Assert.AreEqual (0, ManagementServer.MeasuredDataList.Count) ;

         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;

         new Kernel.Timeout (TimeSpan.FromSeconds (3 * TIMEOUT_IN_SECONDS)).WaitFor (() => ManagementServer.MeasuredDataList.Count > 0) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.Unknown) ;
         }) ;

         // Waaaaait
         new Kernel.Timeout (TimeSpan.FromSeconds (5 * TIMEOUT_IN_SECONDS)).WaitFor (() => (ManagementServer.DeviceAlerts.Count > 0) &&
                                                                                           (ManagementServer.MailAlerts.Count > 0)) ;

         var deviceAlerts = ManagementServer.DeviceAlerts.Count ;
         var mailAlerts = ManagementServer.MailAlerts.Count ;

         Assert.Greater (deviceAlerts, 0) ;
         Assert.Greater (mailAlerts, 0) ;

         Assert.IsTrue (ManagementServer.DeviceAlerts [0].Message.Contains (packageID), ManagementServer.DeviceAlerts[0].Message) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.Alerting) ;
         }) ;

         // Waaaaait, should NOT arrive more mail alerts
         new Kernel.Timeout (TimeSpan.FromSeconds (3 * TIMEOUT_IN_SECONDS)).WaitFor (() => ManagementServer.MailAlerts.Count > mailAlerts) ;

         Assert.GreaterOrEqual (ManagementServer.DeviceAlerts.Count, deviceAlerts) ;
         Assert.IsFalse (ManagementServer.DeviceAlerts.Exists (x => x.IsRecovery)) ;
         Assert.AreEqual (mailAlerts, ManagementServer.MailAlerts.Count) ;

         ManagementServer.DeviceAlerts.Clear() ;
         ManagementServer.MailAlerts.Clear() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.Alerting) ;
         }) ;

         // Send not alerting results
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait, we should get recovery
         new Kernel.Timeout (TimeSpan.FromSeconds( 3 * TIMEOUT_IN_SECONDS)).WaitFor (() => ManagementServer.MailAlerts.Count > 0) ;
         Assert.Greater (ManagementServer.MailAlerts.Count, 0) ;
         Assert.IsFalse (ManagementServer.DeviceAlerts.Exists (x => !x.IsRecovery)) ;
         Assert.IsFalse (ManagementServer.MailAlerts.Exists (x => !x.IsRecovery)) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.OK) ;
         }) ;
      }

      [Test]
      public void CreateSetupOneHeartbeat_JustWait_CheckIfOneTimeoutAlertSent() {
         const int TIMEOUT_INTERVAL = 3 ;
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var instanceID1 = "101" ;
         var instanceID2 = "102" ;
         var instanceName1 = "name101" ;
         var instanceName2 = "name102" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1, instanceID2}, false, null, 2, TIMEOUT_INTERVAL, 2, false) ;
         CheckStatus (token, 1) ;

         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.Unknown) ;
         }) ;

         // Waaaaait
         new Timeout (TimeSpan.FromSeconds (3 * TIMEOUT_INTERVAL + 5)).WaitFor (() => (ManagementServer.DeviceAlerts.Count > 0)) ;
   
         // No alert for no heartbeat
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count (x => x.InstanceID == instanceID2)) ;
         Assert.Greater (ManagementServer.DeviceAlerts.Count (x => x.InstanceID == instanceID1), 0) ;
         Assert.IsTrue (ManagementServer.DeviceAlerts [0].Message.Contains (instanceName1)) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.Alerting) ;
            var instanceStates = package.GetInstanceStates() ;
            Assert.AreEqual (2, instanceStates.Count) ;
            Assert.AreEqual ("alerting", instanceStates [instanceID1].Name) ;
            Assert.AreEqual ("unknown", instanceStates [instanceID2].Name) ;
         }) ;

         ManagementServer.DeviceAlerts.Clear();

         // Send not alerting results
         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;

         // Wait, we should get recovery
         new Timeout (TimeSpan.FromSeconds (TIMEOUT_INTERVAL - 1)).WaitFor (() => (ManagementServer.DeviceAlerts.Count > 0)) ;

         // new Kernel.Timeout (TimeSpan.FromSeconds (TIMEOUT_INTERVAL / 2.0)).Wait() ;
         Assert.IsTrue (ManagementServer.DeviceAlerts.Exists (x => x.IsRecovery)) ;
         Assert.IsFalse (ManagementServer.DeviceAlerts.Exists (x => !x.IsRecovery)) ;
         Assert.IsTrue (ManagementServer.DeviceAlerts [0].Message.Contains (instanceName1)) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.OK, package.State.StateName) ;
         }) ;
      }

      [Test]
      public void CreateSetupNoHeartbeat_JustWait_CheckIfTimeoutAlertSent() {
         const int TIMEOUT_INTERVAL_SECONDS = 3 ;
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var instanceID1 = "101" ;
         var instanceID2 = "102" ;
         var instanceName1 = "name101" ;
         var instanceName2 = "name102" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1, instanceID2}, false, null, 1, TIMEOUT_INTERVAL_SECONDS, 1) ;
         CheckStatus (token, 1) ;

         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.Unknown) ;
         }) ;

         // Waaaaait
         new Kernel.Timeout (TimeSpan.FromSeconds (5 * TIMEOUT_INTERVAL_SECONDS)).WaitFor (() => (ManagementServer.DeviceAlerts.Count > 0)) ;

         // No alert for no heartbeat
         Assert.Greater (ManagementServer.DeviceAlerts.Count, 0) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count (x => x.InstanceID.Equals (instanceID1))) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count (x => x.InstanceID.Equals (instanceID2))) ;
         Assert.IsTrue (ManagementServer.DeviceAlerts [0].Message.Contains ("PackageNameFromID")) ;
         Assert.IsTrue (ManagementServer.DeviceAlerts [0].Message.Contains (packageID)) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsTrue (package.State is PackageState.Alerting) ;
            Assert.AreEqual ("COMMON.AlertStatusMessage", package.State.Message.ToString(), package.State.Message.ToString()) ;
            Assert.IsTrue (package.State.Details.ToJsonString().Contains ("APPLICATION_SUPERVISOR.PackageNotAvailableAlert"), package.State.Details.ToString()) ;


            var instanceStates = package.GetInstanceStates() ;
            Assert.AreEqual (3, instanceStates.Count) ;
            Assert.AreEqual ("unknown", instanceStates [instanceID1].Name.ToLowerInvariant()) ;
            Assert.AreEqual ("unknown", instanceStates [instanceID2].Name.ToLowerInvariant()) ;
            // Heartbeat alerted
            Assert.AreEqual ("alerting", instanceStates [Strings.HEARTBEAT_INSTANCE_ID].Name.ToLowerInvariant()) ;
         }) ;
      }

      [Test]
      public void CreateSetup_SendResultsMoreTimes_CheckIfTimeoutAlertSent() {
         const int TIMEOUT_INTERVAL = 3 ;

         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var instanceID1 = "101" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null,
                                      "packageName1", Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 1, TIMEOUT_INTERVAL, 1) ;
         CheckStatus (token, 1) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         for (int index = 0; index < 10; index++) {
            // Send not alerting results
            SendCheckResults (CheckResultKind.Success, token, packageID) ;
            // Wait
            new Kernel.Timeout (TIMEOUT_INTERVAL * 1000 / 2).Wait() ;
            Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
            Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         }

         new Kernel.Timeout (TIMEOUT_INTERVAL * 1000 * 3).WaitFor (() => (ManagementServer.DeviceAlerts.Count > 0) &&
                                                                         (ManagementServer.MailAlerts.Count > 0)) ;

         Assert.Greater (ManagementServer.DeviceAlerts.Count, 0) ;
         Assert.Greater (ManagementServer.MailAlerts.Count, 0) ;
      }

      [Test]
      public void CreateSetupDeviceAlerts_SendAlert_ConfirmDelivery() {
         StartServer() ;

         var token = Login() ;

         ManagementServer.SetPackageStateStoreIntervalInSeconds (1) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, false, null, 2, 150, 1) ;
         CheckStatus (token, 1) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         #region Send alerting results
         // Logger.Log ("Send alerting results") ;
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Timeout (TimeSpan.FromSeconds (8)).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2)) ;

         Assert.AreEqual (2, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         new Timeout (TimeSpan.FromSeconds (ManagementServer.PackageStateStoreIntervalInSeconds + 2)).WaitFor (() => ManagementServer.PackageStates.Count (x => x.State == "alerting") > 0) ;

         Assert.GreaterOrEqual (ManagementServer.PackageStates.Count, 1) ;
         Assert.AreEqual ("alerting", ManagementServer.PackageStates.Last().State) ;
         #endregion

         #region Send delivered
         // Make sure that one user got the alert, do not bother him
         JObject parameters = new JObject() ;
         parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.TOKEN] = token ;
         parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.PACKAGE_ID] = packageID ;
         var alertID = ManagementServer.DeviceAlerts [0].AlertID ;
         parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.ALERT_ID] = alertID ;
         var deviceID = ManagementServer.DeviceAlerts [0].Address ;
         parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.DEVICE_ID] = deviceID ;
         var instanceID = ManagementServer.DeviceAlerts [0].InstanceID ;
         parameters [WatcherServerAPI.Admin.ConfirmDeviceAlert.Request.INSTANCE_ID] = instanceID ;

         Parameters result = new Parameters (SendPostToServer ("admin/confirmDeviceAlert", parameters)) ;
         Assert.AreEqual (Strings.AllSuccessValues.SUCCESS, result [WatcherServerAPI.GeneralResponse.Names.SUCCESS],
                          result [WatcherServerAPI.GeneralResponse.Names.ERROR]) ;
         // Wait a bit
         new Timeout (TimeSpan.FromSeconds(3)).Wait() ;
         ManagementServer.DeviceAlerts.Clear() ;
         #endregion

         #region Send alerting results
         Logger.Log ("Send alerting results") ;
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Timeout (TimeSpan.FromSeconds (4)).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2)) ;

         // Not alerted the other one
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         #endregion

         #region Send good results, should reset delivered status
         ManagementServer.PackageStates.Clear() ;
         Logger.Log ("Send good results, should reset delivered status") ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds (4)).WaitFor (() => ManagementServer.DeviceAlerts.Count == 2) ;
         Assert.AreEqual (1, ManagementServer.DeviceAlerts.Count (x => (x.InstanceID == instanceID) && (x.AlertID == alertID))) ;
         Assert.AreEqual (2, ManagementServer.DeviceAlerts.Count (x => x.IsRecovery)) ;

         new Kernel.Timeout (TimeSpan.FromSeconds (4)).WaitFor (() => ManagementServer.PackageStates.Count > 0) ;
         Assert.GreaterOrEqual (ManagementServer.PackageStates.Count, 1) ;
         Assert.IsTrue (ManagementServer.PackageStates.Last().PackageID == packageID) ;
         Assert.IsTrue (ManagementServer.PackageStates.Last().State == "ok") ;
         ManagementServer.DeviceAlerts.Clear() ;
         #endregion

         #region Send alerting results again
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Kernel.Timeout (3000).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2)) ;

         Assert.AreEqual (1, ManagementServer.DeviceAlerts.Count (x => (x.InstanceID == instanceID) && (x.AlertID == alertID))) ;

         #endregion
      }

      [Test]
      public void CreateSetupMailAlerts_SendAlert_ConfirmDelivery() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 1, 3, 2) ;
         CheckStatus (token, 1) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         #region Send alerting results
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Kernel.Timeout (3000).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2)) ;

         Assert.AreEqual (2, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (3, ManagementServer.MailAlerts.Count) ;
         ManagementServer.DeviceAlerts.Clear() ;
         ManagementServer.MailAlerts.Clear() ;
         #endregion

         #region Send alerting results again
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds (3)).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2)) ;

         // Should not be new alerts!
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual(0, ManagementServer.DeviceAlerts.Count);
         ManagementServer.DeviceAlerts.Clear() ;
         ManagementServer.MailAlerts.Clear() ;
         #endregion

         #region Send good results, should reset delivered status
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit 
         new Kernel.Timeout (TimeSpan.FromSeconds(3)).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2)) ;

         Assert.GreaterOrEqual (ManagementServer.DeviceAlerts.Count, 2) ;
         Assert.GreaterOrEqual (ManagementServer.MailAlerts.Count, 3) ;

         Assert.AreEqual (2, ManagementServer.DeviceAlerts.Count (x => x.IsRecovery)) ;
         Assert.AreEqual (3, ManagementServer.MailAlerts.Count (x => x.IsRecovery)) ;
         ManagementServer.DeviceAlerts.Clear() ;
         ManagementServer.MailAlerts.Clear() ;
         #endregion

         #region Send alerting results again
         // Logger.Log ("Send alerting results again") ;
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds(3)).WaitFor (() => (ManagementServer.DeviceAlerts.Count == 2)) ;

         Assert.AreEqual (2, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (3, ManagementServer.MailAlerts.Count) ;
         #endregion
      }

      [Test]
      public void CreateSetup_ExpireToken_SendAlert_CheckIfNewLoginDone() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.LoginCount) ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null) ;
         CheckStatus (token, 1) ;

         // Send alert - should trigger a login to MS
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         new Kernel.Timeout (10000).WaitFor (() => ManagementServer.LoginCount == 1) ;
         Assert.AreEqual (1, ManagementServer.LoginCount) ;

         // Expire token
         ManagementServer.ExpireToken() ;

         // Send alert - should trigger a new login
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         new Kernel.Timeout (10000).WaitFor (() => ManagementServer.LoginCount == 2) ;
         Assert.AreEqual (2, ManagementServer.LoginCount) ;
      }

      [Test]
      public void CreateSetup_ErrorInMS() {
         StartServer() ;

         var token = Login() ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null) ;
         CheckStatus (token, 1) ;

         for (int sendIndex = 0; sendIndex < 20; sendIndex++) {
            ManagementServer.GetAgentDeviceIDsThrowsError = true ;
            // Send alert - should trigger a login to MS
            SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;
         }

         new Kernel.Timeout (10000).WaitFor (() => ManagementServer.DeviceAlerts.Count > 1) ;
         Assert.AreEqual (1, ManagementServer.LoginCount) ;
      }

      [Test]
      public void SendNoResults_CheckIfPackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         var instanceID1 = "101" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 1, 2, 2) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         ManagementServer.PackageStates.Clear() ;
         new TimeoutSeconds(7).WaitFor(() => ManagementServer.PackageStates.Count == 0);
         Assert.AreEqual(0, ManagementServer.PackageStates.Count);

         new TimeoutSeconds (7).WaitFor (() => ManagementServer.PackageStates.Count >= 1) ;

         // Check package status
         var lastState = ManagementServer.PackageStates.FirstOrDefault (packageState => packageState.PackageID == packageID) ;
         Assert.IsNotNull (lastState) ;
         // Logger.Log (lastState.Message.ToJsonString().Contains ());
         Assert.IsTrue (lastState.Message.ToJsonString().Contains ("COMMON.UnknownStatusMessage"), lastState.Message.ToJsonString()) ;
         Assert.AreEqual (0, lastState.InstanceStates.Count, lastState.InstanceStates.ToString()) ;
      }

      [Test]
      public void SendGoodResults_CheckIfPackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 100, 150, 2) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         ManagementServer.PackageStates.Clear() ;

         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;

         new Kernel.Timeout (TimeSpan.FromSeconds(7)).WaitFor (() => ManagementServer.PackageStates.Any(packageState => packageState.PackageID == packageID &&
                                                                                                                        packageState.Message.ToJsonString().Contains("COMMON.OKStatusMessage"))) ;

         // Check package status
         var lastState = ManagementServer.PackageStates.LastOrDefault (packageState => packageState.PackageID == packageID) ;
         Assert.IsNotNull (lastState) ;
         // Logger.Log (lastState.Message.ToJsonString().Contains ());
         Assert.IsTrue (lastState.Message.ToJsonString().Contains ("COMMON.OKStatusMessage")) ;
         Assert.AreEqual (lastState.InstanceStates [instanceID1].Name.ToLowerInvariant(), "ok") ;
         // Logger.Log (lastState.InstanceStates ["instance1"].Message.ToJsonString()) ;
         Assert.IsTrue (lastState.InstanceStates [instanceID1].Details.ToJsonString().Contains (message),
                        lastState.InstanceStates [instanceID1].Details.ToJsonString()) ;
      }

      [Test]
      public void SendFailAndGoodResults_CheckIfAlertContainsPackageState() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         var instanceID1 = "101" ;
         var instanceID2 = "102" ;
         var instanceName1 = "name101" ;
         var instanceName2 = "name102" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1, instanceID2}, false, null, 2, 2, 1) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         // Both items are alerting
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID2, instanceName2, message, details) ;

         new Kernel.Timeout (TimeSpan.FromSeconds (7)).WaitFor (() => ManagementServer.DeviceAlerts.Count > 1) ;

         // Check alert - alerting
         var lastAlert = ManagementServer.DeviceAlerts.LastOrDefault (packageState => packageState.PackageID == packageID) ;
         Assert.IsNotNull (lastAlert) ;
         Assert.AreEqual ("alerting", lastAlert.PackageState) ;
         Assert.AreEqual (false, lastAlert.IsRecovery) ;

         // Relax the first item
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;
         ManagementServer.DeviceAlerts.Clear() ;

         new Kernel.Timeout (TimeSpan.FromSeconds (7)).WaitFor (() => ManagementServer.DeviceAlerts.Count > 1) ;

         // Check alert - still alerting
         lastAlert = ManagementServer.DeviceAlerts.LastOrDefault (packageState => packageState.PackageID == packageID &&
                                                                                  packageState.InstanceID == instanceID1) ;
         Assert.IsNotNull (lastAlert) ;
         Assert.AreEqual ("alerting", lastAlert.PackageState) ;
         Assert.AreEqual (true, lastAlert.IsRecovery) ;
         Assert.IsTrue (lastAlert.Message.Contains ("APPLICATION_SUPERVISOR.ApplicationMeasurementOKMessage")) ;

         // Relax the second item
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;
         // Keep first item alive
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;

         ManagementServer.DeviceAlerts.Clear() ;
         new Kernel.Timeout (TimeSpan.FromSeconds (7)).WaitFor (() => ManagementServer.DeviceAlerts.Count > 1) ;

         // Check alert - package is ok now
         lastAlert = ManagementServer.DeviceAlerts.LastOrDefault (packageState => packageState.PackageID == packageID &&
                                                                                  packageState.InstanceID == instanceID2) ;
         Assert.IsNotNull (lastAlert) ;
         Assert.AreEqual ("ok", lastAlert.PackageState) ;
         Assert.AreEqual (true, lastAlert.IsRecovery) ;
      }

      [Test]
      public void SendGoodResultsFor2_LetItTimeout_SendRecoveryForOne_CheckIfRecoveryMessageIsEmpty() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         var instanceID1 = "101" ;
         var instanceID2 = "102" ;
         var instanceName1 = "name101" ;
         var instanceName2 = "name102" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1, instanceID2}, false, null, 2, 2, 1) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;

         // Both items are OK
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;

         new TimeoutSeconds (7).WaitFor (() => ManagementServer.DeviceAlerts.Count > 1) ;

         // Check alert - alerting
         var lastAlert = ManagementServer.DeviceAlerts.LastOrDefault (packageState => packageState.PackageID == packageID) ;
         Assert.IsNotNull (lastAlert) ;
         Assert.AreEqual ("alerting", lastAlert.PackageState) ;
         Assert.AreEqual (false, lastAlert.IsRecovery) ;
         Assert.IsTrue (lastAlert.Message?.Contains ("APPLICATION_SUPERVISOR.PackageNotAvailableAlert"), lastAlert.Message) ;

         // Relax the first item
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID1, instanceName1, message, details) ;
         ManagementServer.DeviceAlerts.Clear() ;

         new TimeoutSeconds (10).WaitFor (() => ManagementServer.DeviceAlerts.Count >= 1) ;

         // Check alert - still alerting
         lastAlert = ManagementServer.DeviceAlerts.LastOrDefault (packageState => packageState.PackageID == packageID &&
                                                                                  packageState.InstanceID == Strings.HEARTBEAT_INSTANCE_ID) ;
         Assert.IsNotNull (lastAlert) ;
         Assert.AreEqual ("unknown", lastAlert.PackageState) ;
         Assert.AreEqual (true, lastAlert.IsRecovery) ;
         Assert.IsTrue (lastAlert.Message?.Contains ("APPLICATION_SUPERVISOR.PackageAvailableRecoveryMessage"), lastAlert.Message) ;
      }

      [Test]
      public void SendMultipleResults_CheckIfOnlyOnePackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         var instanceID1 = "101" ;
         var instanceID2 = "102" ;
         var instanceName1 = "name101" ;
         var instanceName2 = "name102" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 100, 150, 1) ;

         ManagementServer.PackageStates.Clear() ;
         ManagementServer.MailAlerts.Clear() ;

         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         // Send an alert
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID2, instanceName2, message, details) ;

         new Kernel.Timeout (TimeSpan.FromSeconds (5)).WaitFor (() => ManagementServer.MailAlerts.Count > 1) ;
         new Kernel.Timeout (TimeSpan.FromSeconds (ManagementServer.PackageStateStoreIntervalInSeconds + 1)).Wait() ;

         // Check package status
         Assert.AreEqual (2, ManagementServer.PackageStates.Count) ;

         Assert.IsTrue (ManagementServer.PackageStates.Last().State.Equals ("alerting"), ManagementServer.PackageStates.Last().State) ;
      }

      [Test]
      public void SendMultipleResults_SavePackage_CheckIfStatesDeleted() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         var instanceID1 = "101" ;
         var instanceID2 = "102" ;
         var instanceName1 = "name101" ;
         var instanceName2 = "name102" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 100, 150, 1) ;

         ManagementServer.PackageStates.Clear() ;
         ManagementServer.MailAlerts.Clear() ;

         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;

         // Send an alert
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID2, instanceName2, message, details) ;

         // Check package state storage
         new Timeout(TimeSpan.FromSeconds(ManagementServer.PackageStateStoreIntervalInSeconds + 1)).WaitFor(() => ManagementServer.PackageStatesForStore.Count(x => x.Value.PackageID == packageID && x.Value.State == "alerting") > 0);
         Assert.Greater(ManagementServer.PackageStatesForStore.Count(x => x.Value.PackageID == packageID && x.Value.State == "alerting"), 0);

         // Save package again, should clear the states
         SavePackage (token, DateTime.UtcNow, null, "packageName1",
                      packageID, new List<string> {instanceID1}, true, null, 100, 150, 100) ;

         new Timeout(TimeSpan.FromSeconds(10)).WaitFor(() => ManagementServer.PackageStatesForStore.Count(x => x.Value.PackageID == packageID && x.Value.State == "alerting") == 0);

         // Check storage - package should disappear
         Assert.AreEqual (0, ManagementServer.PackageStatesForStore.Count (x => x.Value.PackageID == packageID && x.Value.State == "alerting")) ;

         // Check package states
         Assert.Greater(ManagementServer.PackageStatesForStore.Count (x => x.Key == packageID), 0);

         // Assert.IsTrue (ManagementServer.PackageStates [0].State.Equals ("unknown"), ManagementServer.PackageStates [0].State) ;
      }

      [Test]
      public void SavePackage_DeletePackage_CheckIfStatesDeleted() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         var instanceID1 = "101" ;
         var instanceID2 = "102" ;
         var instanceName1 = "name101" ;
         var instanceName2 = "name102" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, true, null, 100, 150, 1) ;

         ManagementServer.PackageStates.Clear() ;
         ManagementServer.MailAlerts.Clear() ;

         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, false, instanceID2, instanceName2, message, details) ;
         SendCheckResults (CheckResultKind.Success, token, packageID, true, instanceID1, instanceName1, message, details) ;

         // Send an alert
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID2, instanceName2, message, details) ;

         // Check package state storage
         new Timeout (TimeSpan.FromSeconds (ManagementServer.PackageStateStoreIntervalInSeconds + 1)).WaitFor (() => ManagementServer.PackageStatesForStore.Count (x => x.Value.PackageID == packageID) > 0) ;
         Assert.Greater (ManagementServer.PackageStatesForStore.Count (x => x.Value.PackageID == packageID), 0) ;

         // Check management server package states
         new Timeout (TimeSpan.FromSeconds (ManagementServer.PackageStateStoreIntervalInSeconds + 1)).WaitFor (() => ManagementServer.PackageStates.Count > 0) ;
         Assert.Greater (ManagementServer.PackageStates.Count, 0) ;

         // Delete package
         JObject parameters = new JObject() ;
         parameters [WatcherServerAPI.Packages.Delete.Request.TOKEN] = token ;
         parameters [WatcherServerAPI.Packages.Delete.Request.PACKAGE_ID] = packageID ;

         Parameters result = new Parameters (SendPostToServer (WatcherServerAPI.Packages.Delete.FULL_URL, parameters)) ;
         Assert.IsTrue (result.IsSuccess()) ;

         // Check storage - package should disappear
         Assert.AreEqual (0, ManagementServer.PackageStatesForStore.Count (x => x.Value.PackageID == packageID)) ;

         new Timeout (TimeSpan.FromSeconds (ManagementServer.PackageStateStoreIntervalInSeconds * 2 + 1)).Wait() ;

         // Check package states
         Assert.Greater (ManagementServer.PackageStates.Count (x => x.PackageID == packageID), 0) ;
      }

      public void CheckIfNotAlertedWithSchedule (Schedules schedules) {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var instanceID1 = "101" ;
         var instanceName1 = "name101" ;
         var message = "Test Message 1" ;
         var details = "Test Details 1" ;

         var packageID = SavePackage (token, DateTime.UtcNow, null, "packageName1",
                                      Guid.NewGuid().ToString(), new List<string> {instanceID1}, false, schedules) ;
         CheckStatus (token, 1) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;

         #region Send alerting results
         Logger.Log ("Send alerting results") ;
         SendCheckResults (CheckResultKind.Fail, token, packageID, false, instanceID1, instanceName1, message, details) ;

         // Wait a bit
         new TimeoutSeconds (10).WaitFor (() => ManagementServer.DeviceAlerts.Count > 0) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;

         #endregion
      }

      [Test]
      public void CreateOnceSchedule_SendAlert_CheckIfNotAlerted() {
         JObject schedulesConfiguration = new JObject() ;
         JArray schedulesArray = new JArray() ;
         ScheduleOnce once = new ScheduleOnce() ;
         once.Interval = TimeSpan.FromSeconds (100) ;
         once.Start = DateTime.UtcNow ;
         once.IsEnabled = true ;

         schedulesArray.Add (once.ToConfigurationData().AsJObject) ;
         schedulesConfiguration ["schedules"] = schedulesArray ;
         Schedules schedules = new Schedules() ;
         schedules.Configure (new ConfigurationData (schedulesConfiguration)) ;

         CheckIfNotAlertedWithSchedule (schedules) ;
      }

      [Test]
      public void CreateDailySchedule_SendAlert_CheckIfNotAlerted() {
         JObject schedulesConfiguration = new JObject() ;
         JArray schedulesArray = new JArray() ;
         ScheduleDaily daily = new ScheduleDaily() ;
         daily.Interval = TimeSpan.FromSeconds (100) ;
         daily.StartTime = DateTime.UtcNow.TimeOfDay ;
         daily.IsEnabled = true ;

         schedulesArray.Add (daily.ToConfigurationData().AsJObject) ;
         schedulesConfiguration ["schedules"] = schedulesArray ;
         Schedules schedules = new Schedules() ;
         schedules.Configure (new ConfigurationData (schedulesConfiguration)) ;

         CheckIfNotAlertedWithSchedule (schedules) ;
      }

      [Test]
      public void CreateWeeklySchedule_SendAlert_CheckIfNotAlerted() {
         JObject schedulesConfiguration = new JObject() ;
         JArray schedulesArray = new JArray() ;
         ScheduleWeekly weekly = new ScheduleWeekly() ;
         weekly.Interval = TimeSpan.FromSeconds (100) ;
         weekly.StartTime = DateTime.UtcNow.TimeOfDay;
         weekly.DaysOfWeek.Add (DateTime.UtcNow.DayOfWeek) ;
         weekly.IsEnabled = true ;

         schedulesArray.Add (weekly.ToConfigurationData().AsJObject) ;
         schedulesConfiguration ["schedules"] = schedulesArray ;
         Schedules schedules = new Schedules() ;
         schedules.Configure (new ConfigurationData (schedulesConfiguration)) ;

         CheckIfNotAlertedWithSchedule (schedules) ;
      }
   }
}