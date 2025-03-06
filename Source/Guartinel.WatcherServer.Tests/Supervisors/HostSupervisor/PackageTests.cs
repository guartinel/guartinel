using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Instances ;
using Guartinel.WatcherServer.Supervisors.HostSupervisor ;
using Guartinel.WatcherServer.Tests.Packages ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Supervisors.HostSupervisor {

   [TestFixture]
   public class PackageTests : HttpPackageTestsBase {
      protected string SavePackage (string token,
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

         return base.SavePackageX (token, Guartinel.Communication.Supervisors.HostSupervisor.Strings.Use.PackageType,
                                   modificationDate,
                                   configure,
                                   packageName, packageID, addMailAlerts,
                                   null,
                                   checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                                   checkDetails,
                                   package => {
                                      HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
                                      Assert.IsNotNull (testPackage) ;

                                      var checker = testPackage.CreateCheckers() [0].CastTo<HostChecker>() ;
                                      Assert.AreEqual (packageName, checker.Name) ;
                                   }, instanceStates) ;
      }

      [Test]
      public void CreateSetupAddPackageWithInvalidAddress_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var packageID = Guid.NewGuid().ToString() ;
         List<Host> hosts = new List<Host> {
                  new Host ("XEhuneX.com"),
                  new Host ("ehunex.comx")
         } ;
         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts, 2, 1), "packageName1", packageID, true,
                      10, 300, 1, false) ;
         CheckStatus (token, 1) ;

         // Wait a bit

         new Timeout (TimeSpan.FromSeconds (30)).WaitFor (() => (ManagementServer.DeviceAlerts.Count >= 4) &&
                                                                (ManagementServer.MailAlerts.Count >= 6)) ;
         // Alerts for all devices/mails for all hosts
         Assert.AreEqual (4, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (6, ManagementServer.MailAlerts.Count) ;

         // Check package IDs
         Assert.AreEqual (packageID, ManagementServer.DeviceAlerts [0].PackageID) ;
         Assert.AreEqual (packageID, ManagementServer.MailAlerts [0].PackageID) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            var checkResult1 = testPackage.CreateCheckers() [0].Check(null) [0] ;
            Assert.IsTrue (checkResult1.CheckResultKind == CheckResultKind.Fail) ;

            Assert.IsTrue (checkResult1.Message.ToJsonString().Contains ("HOST_SUPERVISOR.HostIsNotAvailableAlert"), checkResult1.Message.ToJsonString()) ;
            Assert.IsTrue (checkResult1.Message.ToJsonString().Contains ("XEhuneX.com"), checkResult1.Message.ToJsonString()) ;
         }) ;
      }

      [Test]
      public void CreateSetupAddPackageWithValidAddress_RunCheck() {
         StartServer() ;

         var token = Login() ;

         List<Host> hosts = new List<Host> {
                  new Host ("www.google.com")
         } ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts), "packageName1", packageID, true) ;
         CheckStatus (token, 1) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            var checker = testPackage.CreateCheckers() [0].CastTo<HostChecker>() ;
            Assert.AreEqual ("www.google.com", checker.Host.Address) ;
         }) ;

         // Wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds (10)).WaitFor (() => (ManagementServer.MeasuredDataList.Count > 0)) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var measurement = ManagementServer.MeasuredDataList.First().Data.Replace ("\n", "") ;
         Assert.IsTrue (measurement.Contains (@"""success"": true"), ManagementServer.MeasuredDataList.First().Data) ;
         Assert.IsTrue (measurement.Contains (@"www.google.com"), ManagementServer.MeasuredDataList.First().Data) ;
         Assert.IsTrue (measurement.Contains (@"""code"": ""HOST_SUPERVISOR.HostIsOKMessage"""), ManagementServer.MeasuredDataList.First().Data) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            var checkResult1 = testPackage.CreateCheckers() [0].Check(null) [0] ;
            Assert.IsTrue (checkResult1.CheckResultKind == CheckResultKind.Success) ;
         }) ;
      }

      [Test]
      public void CreateSetupAddPackageWithValidAddressAndCaption_RunCheck() {
         StartServer() ;

         var token = Login() ;

         List<Host> hosts = new List<Host> {
                  new Host ("www.google.com", "UncleGoogle")
         } ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts), "packageName1", packageID, true) ;
         CheckStatus (token, 1) ;

         // Wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds (10)).WaitFor (() => (ManagementServer.MeasuredDataList.Count > 0)) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var measurement = ManagementServer.MeasuredDataList.First().Data.Replace ("\n", "") ;
         Assert.IsTrue (measurement.Contains (@"""success"": true"), ManagementServer.MeasuredDataList.First().Data) ;
         Assert.IsTrue (measurement.Contains (@"UncleGoogle"), ManagementServer.MeasuredDataList.First().Data) ;
         Assert.IsTrue (measurement.Contains (@"""code"": ""HOST_SUPERVISOR.HostIsOKMessage"""), ManagementServer.MeasuredDataList.First().Data) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            var checkResult1 = testPackage.CreateCheckers() [0].Check(null) [0] ;
            Assert.IsTrue (checkResult1.CheckResultKind == CheckResultKind.Success) ;
         }) ;
      }

      [Test]
      public void CreateSetupAddPackageWithValidAddressNoRetryAndWaitSpecified_RunCheck() {
         StartServer() ;

         var token = Login() ;

         List<Host> hosts = new List<Host> {
                  new Host ("www.google.com")
         } ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts, null, null), "packageName1", packageID, true) ;
         CheckStatus (token, 1) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            var checker = testPackage.CreateCheckers() [0].CastTo<HostChecker>() ;
            Assert.AreEqual ("www.google.com", checker.Host.Address) ;
         }) ;

         // Wait a bit
         Kernel.Timeout waiter = new Kernel.Timeout (10000) ;
         waiter.WaitFor (() => (ManagementServer.MeasuredDataList.Count > 0)) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            var checkResult1 = testPackage.CreateCheckers() [0].Check(null) [0] ;
            Assert.IsTrue (checkResult1.CheckResultKind == CheckResultKind.Success) ;
         }) ;
      }

      [Test]
      public void CreateSetupAddManyPackages_RunChecks() {
         const int PACKAGE_COUNT = 100 ;

         StartServer() ;

         var token = Login() ;

         for (int packageIndex = 0; packageIndex < PACKAGE_COUNT; packageIndex++) {
            var elapsed = StopwatchEx.TimeIt (() => {
               List<Host> hosts = new List<Host> {
                        new Host ("www.google.com")
               } ;

               var packageID = Guid.NewGuid().ToString() ;
               SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts, 1, 3),
                            "packageName1", packageID, false,
                            1000, 1500, 1) ;
            }) ;

            Assert.LessOrEqual (elapsed.Seconds, 2,
                                $"Sending took {elapsed.Seconds} secs at {packageIndex + 1} of {PACKAGE_COUNT}.") ;
         }

         // Wait a bit
         new Timeout (5 * 100 * PACKAGE_COUNT).WaitFor (() => (ManagementServer.MeasuredDataList.Count >= PACKAGE_COUNT)) ;

         Assert.AreEqual (PACKAGE_COUNT, ManagementServer.MeasuredDataList.Count) ;
      }

      [Test]
      public void CreateSetupPackageWithPort_RunChecks() {
         StartServer() ;

         var token = Login() ;

         List<Host> hosts = new List<Host> {
                  new Host ("www.sysment.com:80")
         } ;

         var packageID = Guid.NewGuid().ToString() ;
         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts, null, 1),
                      "packageName1", packageID, false, 1000, 1500, 1) ;

         // Wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds (10)).WaitFor (() => (ManagementServer.MeasuredDataList.Count > 0)) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         _watcherServer.PackageController.UsePackage (packageID, package => {
            HostSupervisorPackage testPackage = package as HostSupervisorPackage ;
            Assert.IsNotNull (testPackage) ;

            var checkResult1 = testPackage.CreateCheckers() [0].Check(null) [0] ;
            Assert.IsTrue (checkResult1.CheckResultKind == CheckResultKind.Success) ;
         }) ;
      }

      [Test]
      public void CreateSetupPackageWithInstanceState_GetPackageState_Check() {
         StartServer() ;

         var token = Login() ;

         List<Host> hosts = new List<Host> {
                  new Host ("www.sysment.com")
         } ;

         var packageID = Guid.NewGuid().ToString() ;

         var instanceStateList = new InstanceStateList() ;
         var message = new XConstantString ("12345",
                                            new XConstantString.Parameter ("parameter1", "value1"),
                                            new XConstantString.Parameter ("parameter2", "lookupName2", "value2")) ;
         var details = new XConstantString ("6789",
                                            new XConstantString.Parameter ("detailsParameter1", "detailsValue1"),
                                            new XConstantString.Parameter ("detailsParameter2", "detailsLookupName2", "detailsValue2")) ;
         var extract = new XConstantString("512");
         instanceStateList.Add ("www.sysment.com", new InstanceState.Alerting (message, details, extract)) ;

         ManagementServer.PackageStates.Clear() ;

         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts, null, 1),
                      "packageName1", packageID, false, 1000, 1500, 1000, false, instanceStateList) ;

         // Wait a bit
         new Timeout (TimeSpan.FromSeconds (10)).WaitFor (() => (ManagementServer.PackageStates.Any (x => x.Message.ToString().Contains ("COMMON.AlertStatusMessage")))) ;

         Assert.Greater (ManagementServer.PackageStates.Count, 0) ;

         ManagementServerMock.PackageStateMock packageState = ManagementServer.PackageStates.Last() ;
         Assert.IsTrue (packageState.Message is XConstantString) ;
         message = packageState.Message as XConstantString ;
         Assert.IsNotNull (message) ;
         Assert.AreEqual ("COMMON.AlertStatusMessage", message?.Code) ;

         Assert.Greater (packageState.InstanceStates.Count, 0) ;
         var instanceState = packageState.InstanceStates ["www.sysment.com"] ;
         Assert.IsTrue (instanceState.Message is XConstantString) ;
         message = instanceState.Message as XConstantString ;
         Assert.AreEqual ("12345", message?.Code) ;

         Assert.AreEqual (2, message?.Parameters.Count) ;
         Assert.AreEqual ("parameter1", message?.Parameters [0].Name) ;
         Assert.AreEqual ("value1", (message?.Parameters [0].Value as XSimpleString)?.Value) ;
         Assert.AreEqual ("parameter2", message?.Parameters [1].Name) ;
         Assert.AreEqual ("lookupName2", message?.Parameters [1].LookupName) ;
         Assert.AreEqual ("value2", (message?.Parameters [1].Value as XSimpleString)?.Value) ;

         Assert.IsTrue (instanceState.Details is XConstantString) ;
         details = instanceState.Details as XConstantString ;
         Assert.AreEqual ("6789", details?.Code) ;

         Assert.AreEqual (2, details.Parameters.Count) ;
         Assert.AreEqual ("detailsParameter1", details.Parameters [0].Name) ;
         Assert.AreEqual ("detailsValue1", (details.Parameters [0].Value as XSimpleString)?.Value) ;
         Assert.AreEqual ("detailsParameter2", details.Parameters [1].Name) ;
         Assert.AreEqual ("detailsLookupName2", details.Parameters [1].LookupName) ;
         Assert.AreEqual ("detailsValue2", (details.Parameters [1].Value as XSimpleString)?.Value) ;

         Assert.IsTrue(instanceState.Extract is XConstantString);
         extract = instanceState.Extract as XConstantString;
         Assert.AreEqual("512", extract?.Code);
      }

      [Test]
      public void CreateSetupPackageWithAlertingInstanceState_WaitForCheck_ShouldNotAlertAlert() {
         StartServer() ;

         var token = Login() ;

         List<Host> hosts = new List<Host> {
                  new Host ("www.sysment.comX")
         } ;

         var packageID = Guid.NewGuid().ToString() ;

         var instanceStateList = new InstanceStateList();
         var message = new XConstantString("HOST_SUPERVISOR.HostIsNotAvailableAlertMessage",
                                           new XConstantString.Parameter("Host", "www.sysment.comX"));
         var details = new XConstantString ("6789",
                                            new XConstantString.Parameter ("detailsParameter1", "detailsValue1"),
                                            new XConstantString.Parameter ("detailsParameter2", "detailsLookupName2", "detailsValue2")) ;
         var extract = new XConstantString("512");
         instanceStateList.Add ("www.sysment.comX", new InstanceState.Alerting (message, details, extract)) ;

         ManagementServer.PackageStates.Clear() ;

         const int INITIAL_DELAY_SECONDS = 15 ;

         SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, hosts, waitTimeSeconds: 1, retryCount: 0),
                      "packageName1", packageID, false, 1000, 1500, INITIAL_DELAY_SECONDS, false, instanceStateList) ;

         // Wait a bit
         new Timeout (TimeSpan.FromSeconds (INITIAL_DELAY_SECONDS + ManagementServer.PackageStateStoreIntervalInSeconds + 2))
                  .WaitFor (() => (ManagementServer.PackageStates.Any (x => x.Message.ToString().Contains ("COMMON.AlertStatusMessage")))) ;

         Assert.Greater (ManagementServer.PackageStates.Count, 0) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MeasuredDataList.Count) ;

         ManagementServerMock.PackageStateMock packageState = ManagementServer.PackageStates.Last() ;
         Assert.IsTrue (packageState.Message is XConstantString) ;
         // Assert.AreEqual("X", packageState.InstanceStates ["www.sysment.comX"].Details.ToJsonString(), packageState.InstanceStates["www.sysment.comX"].Details.ToJsonString());
         message = packageState.Message as XConstantString ;
         Assert.IsNotNull (message) ;
         Assert.AreEqual ("COMMON.AlertStatusMessage", message?.Code) ;
         Assert.AreEqual ("alerting", packageState.State) ;

         new TimeoutSeconds (INITIAL_DELAY_SECONDS + 5).WaitFor (() => ManagementServer.MeasuredDataList.Count > 0) ;
         Assert.Greater (ManagementServer.MeasuredDataList.Count, 0) ;
         new TimeoutSeconds (5).WaitFor (() => ManagementServer.DeviceAlerts.Count > 0) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
      }
   }
}