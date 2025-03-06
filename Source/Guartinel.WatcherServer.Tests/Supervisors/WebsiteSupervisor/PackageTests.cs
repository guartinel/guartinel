using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Utility ;
using Guartinel.Service.WebsiteChecker ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Instances ;
using Guartinel.WatcherServer.Packages ;
using Guartinel.WatcherServer.Supervisors.WebsiteSupervisor ;
using Guartinel.WatcherServer.Tests.Packages ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Supervisors.WebsiteSupervisor {

   public class PackageTestsBase : HttpPackageTestsBase {
      public new static class Constants {
         public const int STARTUP_DELAY_SECONDS = 1 ;
         public const int CHECK_INTERVAL_SECONDS = 40 ;
         public const int TIMEOUT_SECONDS = 60 ;

         public const int WAIT_FOR_RESULTS = STARTUP_DELAY_SECONDS + CHECK_INTERVAL_SECONDS * 2 + 10 ;
      }

      protected override void Setup1 () {
         base.Setup1();
         // Clear cache
         WebsiteCheckCache.Clear();
      }

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

         return base.SavePackageX (token, Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.Use.PackageType,
                                   modificationDate,
                                   configure,
                                   packageName, packageID, addMailAlerts,
                                   null,
                                   checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                                   checkDetails,
                                   package => {
                                      var checker = package.CreateCheckers() [0].CastTo<WebsiteChecker>() ;
                                      Assert.AreEqual (packageName, checker.Name) ;
                                   },
                                   instanceStates) ;
      }

      protected void CheckManagementServerMockStatus (int? deviceAlertCount = 0,
                                                      int? mailAlertCount = 0,
                                                      int? measuredDataCount = 0) {
         if (mailAlertCount != null) {
            Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         }

         if (mailAlertCount != null) {
            Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         }

         if (measuredDataCount != null) {
            Assert.AreEqual (0, ManagementServer.MeasuredDataList.Count) ;
         }
      }

      protected ConfigurationData WaitForMeasuredData(int times = 1) {
         var timeout = new TimeoutSeconds (Constants.WAIT_FOR_RESULTS * times) ;
         timeout.WaitFor (() => ManagementServer.MeasuredDataList.Count >= times) ;
         Assert.GreaterOrEqual (ManagementServer.MeasuredDataList.Count, times) ;
         Assert.IsNotNull (ManagementServer.MeasuredDataList.LastOrDefault()) ;

         return new ConfigurationData (ManagementServer.MeasuredDataList.Last().Data) ;
      }

      protected ManagementServerMock.PackageStateMock WaitForPackageState (string packageID) {
         var timeout = new TimeoutSeconds (Constants.WAIT_FOR_RESULTS + ManagementServer.PackageStateStoreIntervalInSeconds + 5) ;
         timeout.WaitFor (() => ManagementServer.PackageStates.Count (packageState => packageState.PackageID == packageID &&
                                                                                      packageState.State != "unknown") > 0) ;
         var lastState = ManagementServer.PackageStates.LastOrDefault (packageState => packageState.PackageID == packageID &&
                                                                                       packageState.State != "unknown") ;
         Assert.IsNotNull (lastState) ;

         return lastState ;
      }

      protected void WaitForAlerts(int? deviceAlertCount = null,
                                   int? mailAlertCount = null) {
         ManagementServer.DeviceAlerts.Clear() ;
         ManagementServer.MailAlerts.Clear() ;
         var timeout = new TimeoutSeconds(Constants.WAIT_FOR_RESULTS * 2 + 5) ;
         timeout.WaitFor (() => (ManagementServer.DeviceAlerts.Count >= (deviceAlertCount ?? 0)) &&
                                (ManagementServer.MailAlerts.Count >= (mailAlertCount ?? 0))) ;

         Assert.IsTrue ((ManagementServer.DeviceAlerts.Count == (deviceAlertCount ?? 0)) &&
                        (ManagementServer.MailAlerts.Count == (mailAlertCount ?? 0))) ;
      }

      protected string TestSite (string site,
                               bool downloadSuccess,
                               bool checkSuccess,
                               string checkTextPattern = "") {

         const int LOAD_TIME_SECONDS = 20 ;

         ApplicationSettings.Use.WebsiteChecker = nameof(ChromeRemote) ;

         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, _watcherServer.PackageController.PackageCount) ;
         Assert.AreEqual (0, ManagementServer.MeasuredDataList.Count) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var websites = new List<Website> {new Website (site)} ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, websites, LOAD_TIME_SECONDS, checkTextPattern, 1),
                                      "packageName1", Guid.NewGuid().ToString(), true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS, false) ;
         CheckStatus (token, 1) ;

         // Wait a bit
         var measuredData = WaitForMeasuredData() ;
         Assert.AreEqual(downloadSuccess ? "true" : "false",
                         measuredData[Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.SUCCESS].ToLowerInvariant());

         if (downloadSuccess) {
            Assert.Greater (measuredData.AsDouble (Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.LOAD_TIME_SECONDS), 0) ;

            //var messageString = measuredData [Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.MESSAGE] ;
            //Assert.IsTrue (messageString.Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKMessage"), messageString) ;
            //var detailsString = measuredData [Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.DETAILS] ;
            //Assert.IsTrue (detailsString.Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKDetails"), detailsString) ;

            DateTime? certificateExpiry = measuredData.AsDateTimeNull(Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.CERTIFICATE_EXPIRY);
            if (certificateExpiry != null) {
               Assert.Greater(certificateExpiry, DateTime.UtcNow);
            }
         }

         var packageState = WaitForPackageState (packageID) ;

         Assert.IsTrue(packageState.Message.ToJsonString().Contains(checkSuccess ? "COMMON.OKStatusMessage" : "COMMON.AlertStatusMessage"), packageState.Message.ToJsonString());
         Assert.IsTrue(packageState.Details.ToJsonString().Contains(checkSuccess ? "COMMON.OKStatusDetails" : "COMMON.AlertStatusDetails"), packageState.Details.ToJsonString());

         if (checkSuccess) {
            // Check contents of the states
            Assert.AreEqual ("ok", packageState.InstanceStates [site].Name.ToLowerInvariant()) ;
            Assert.IsTrue (packageState.InstanceStates [site].Message.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKMessage"), packageState.InstanceStates [site].Message.ToJsonString()) ;
            Assert.IsTrue (packageState.InstanceStates [site].Details.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKDetails"), packageState.InstanceStates [site].Details.ToJsonString()) ;
            Assert.IsTrue (packageState.InstanceStates [site].Extract.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKExtract"), packageState.InstanceStates [site].Extract.ToJsonString()) ;
         }

         WaitForAlerts(checkSuccess ? 0 : 1) ;

         if (checkSuccess) {
            Assert.AreEqual(0, ManagementServer.DeviceAlerts.Count, string.Join(", ", ManagementServer.DeviceAlerts.Select(x => x.Message)));
            Assert.AreEqual(0, ManagementServer.MailAlerts.Count, string.Join(", ", ManagementServer.MailAlerts.Select(x => x.Message)));
            Assert.AreEqual("ok", packageState.State);
         } else {
            Assert.Greater(ManagementServer.DeviceAlerts.Count, 0);
            Assert.Greater(ManagementServer.MailAlerts.Count, 0);
            Assert.AreEqual("alerting", packageState.State);
         }

         return packageID ;
      }
   }

   [TestFixture]
   public class PackageTests : PackageTestsBase {

      [SetUp]
      public void SetUp() {
         //ApplicationSettings.Use.QueueServiceAddress = @"http://10.0.75.1:5672/";
         //ApplicationSettings.Use.QueueServiceUserName = "queue.test" ;
         //ApplicationSettings.Use.QueueServicePassword = "VLTWnQ3eW6T1MOJsmMwI" ;
      }

      protected void PackageWithInvalidAddressRunCheck (string downloadType) {
         ApplicationSettings.Use.WebsiteChecker = downloadType ;

         StartServer() ;

         var token = Login() ;
         CheckStatus (token, 0) ;

         CheckManagementServerMockStatus() ;

         // var websites = new List<Website> {new Website ("http://XEhuneX.com")} ;
         var websites = new List<Website> {new Website ("https:www.papapapa.fr")} ;
          var packageID = SavePackage (token, DateTime.UtcNow, x => Configuration.CreatePackageConfiguration (x, websites, 10, "", 1, 0), // , 1, 5),
                                      "packageName1", Guid.NewGuid().ToString(), true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS) ;
         CheckStatus (token, 1) ;

         WaitForMeasuredData() ;
         WaitForAlerts (2, 3) ;

         ConfigurationData measuredData = new ConfigurationData (ManagementServer.MeasuredDataList.Last().Data) ;

         Assert.AreEqual ("false", measuredData [Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.SUCCESS].ToLowerInvariant()) ;
         Assert.IsTrue (string.IsNullOrEmpty (measuredData [Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.LOAD_TIME_SECONDS])) ;

         var alert = ManagementServer.DeviceAlerts [0] ;
         Assert.IsTrue (alert.Message.Contains ("WEBSITE_SUPERVISOR.WebsiteCheckErrorMessage"),
                        alert.Message) ;
         Assert.IsTrue (alert.Message.ToLowerInvariant().Contains ("xehunex.com"),
                        alert.Message) ;
      }

      [Test]
      public void PackageWithInvalidAddressChrome_RunCheck() {
         PackageWithInvalidAddressRunCheck (nameof(ChromeRemote)) ;
      }

      [Test]
      public void PackageWithInvalidAddressWebRequest_RunCheck() {
         PackageWithInvalidAddressRunCheck (nameof(HttpRequest)) ;
      }

      [Test]
      public void PackageWithMultipleAddresses_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var websites = new List<Website> {
                  new Website (@"https://www.google.hu"),
                  new Website (@"https://www.xehunex.com"),
                  new Website (@"http://www.sysment.hu/"),
                  new Website (@"http://www.intensifight.com/", "Intenssssss")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, websites,
                                                                                     checkLoadTimeSeconds: 20),
                                      "packageName1", Guid.NewGuid().ToString(), true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS) ;
         CheckStatus (token, 1) ;

         WaitForMeasuredData (4) ;
         var measuredData = ManagementServer.MeasuredDataList.ToArray().ToList() ;
         Assert.AreEqual (4, measuredData.Count) ;

         Assert.AreEqual (true, JObject.Parse (measuredData [0].Data).GetBooleanValue ("success", false), measuredData [0].Data) ;
         Assert.AreEqual (false, JObject.Parse (measuredData [1].Data).GetBooleanValue ("success", true), measuredData [0].Data) ;
         Assert.AreEqual (true, JObject.Parse (measuredData [2].Data).GetBooleanValue ("success", false), measuredData [0].Data) ;
         Assert.AreEqual (true, JObject.Parse (measuredData [3].Data).GetBooleanValue ("success", false), measuredData [0].Data) ;
      }

      [Test]
      public void PackageWithMissingHttp_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var websites = new List<Website> {
                  new Website (@"www.sysment.hu"),
                  new Website (@"www.google.com")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, websites),
                                      "packageName1", Guid.NewGuid().ToString(), true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS) ;
         CheckStatus (token, 1) ;

         WaitForMeasuredData();

         var timeout = new TimeoutSeconds(Constants.WAIT_FOR_RESULTS) ;
         timeout.Wait();

         Assert.IsTrue (ManagementServer.DeviceAlerts.Count == 0 &&
                        ManagementServer.MailAlerts.Count == 0) ;
      }

      [Test]
      public void PackageWithRedirectedAddress_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var websites = new List<Website> {
                  new Website (@"http://manage.guartinel.com")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, websites),
                                      "packageName1", Guid.NewGuid().ToString(), true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS) ;
         CheckStatus (token, 1) ;

         CheckMeasuredData (WaitForMeasuredData(), true) ;
      }

      private void CheckMeasuredData (ConfigurationData measuredData,
                                          bool success) {
         Assert.AreEqual (success, measuredData.AsBoolean ("success")) ;
      }

      [Test]
      public void PackageWithValidAddress_RunCheck() {
         TestSite (@"http://www.sysment.hu", true, true) ;
      }

      [Test]
      public void PackageWithValidAddress_KeepRunning_CheckPackageStates() {
         var packageID = TestSite (@"www.index.hu", true, true) ;

         ManagementServer.PackageStates.Clear() ;

         // Keep running
         var timeout = new TimeoutSeconds (3 * (Constants.WAIT_FOR_RESULTS + ManagementServer.PackageStateStoreIntervalInSeconds + 5)) ;
         timeout.WaitFor (() => ManagementServer.PackageStates.Count (packageState => packageState.PackageID == packageID) >= 3) ;
         var states = ManagementServer.PackageStates.Where (packageState => packageState.PackageID == packageID).ToList() ;
         Assert.IsTrue (states.All (state => state.State == "ok")) ;
      }

      [Test]
      public void PackageWithValidAddressNoProtocol_RunCheck() {
         TestSite (@"www.sysment.hu", true, true) ;
      }

      [Test]
      public void PackageWithValidAddressWithNoEncoding_RunCheck() {
         TestSite (@"https://backend.guartinel.com:9090", true, true) ;
      }

      [Test]
      public void PackageWithValidAddress_ValidSubString_RunCheck() {
         TestSite (@"http://www.sysment.hu", true, true, "healthy") ;
      }

      [Test]
      public void PackageWithValidAddress_WithInvalidSubString_RunCheck() {
         TestSite (@"https://www.index.hu", true, false, "ehunex") ;
      }

      [Test]
      public void PackageWithValidAddress_WithValidHungarianSubString_RunCheck() {
         TestSite (@"http://www.index.hu", true, true, "tudomány") ;
      }

      [Test]
      public void PackageWithValidAddresses_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var webSites = new List<Website> {
                  new Website (@"http://www.sysment.hu"),
                  new Website (@"https://manage.guartinel.com"),
                  new Website (@"http://intensifight.com"),
                  new Website (@"http://www.nekedmihasznal.hu/hoerghurut")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, webSites,
                                                                                     checkLoadTimeSeconds: 20),
                                      "packageName1", Guid.NewGuid().ToString(),
                                      true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS) ;
         CheckStatus (token, 1) ;

         WaitForMeasuredData (webSites.Count) ;
         List<ConfigurationData> measuredDataItems = ManagementServer.MeasuredDataList.Select(x => new ConfigurationData(x.Data)).ToList();

         WaitForPackageState (packageID) ;
         var lastState = ManagementServer.PackageStates.LastOrDefault(packageState => packageState.PackageID == packageID);

         Assert.IsNotNull(lastState);

         Assert.IsTrue(lastState.Message.ToJsonString().Contains("COMMON.OKStatusMessage"), lastState.Message.ToJsonString());

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         
         Assert.GreaterOrEqual (measuredDataItems.Count, webSites.Count) ;
         measuredDataItems.ForEach (x => Debug.WriteLine ($"{x ["website"]}: {x ["load_time_milliseconds"]} ms")) ;

         Assert.IsTrue (measuredDataItems.All (x => x [Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.SUCCESS].ToLowerInvariant().Equals ("true"))) ;
         // Assert.IsTrue (measuredDataItems.All (x => x [Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.MESSAGE].Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKMessage"))) ;

         // Check contents of the states
         Assert.IsTrue (lastState.InstanceStates.All (x => x.Value.Name.ToLowerInvariant() == "ok")) ;
         Assert.IsTrue (lastState.InstanceStates.All (x => x.Value.Message.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKMessage"))) ;
      }

      [Test]
      public void Package_ConfigureAlerted_Check() {
         StartServer() ;

         var token = Login() ;

         var states = new InstanceStateList() ;
         states.Add ("http://www.INVALID.hu", InstanceState.Create (nameof(InstanceState.Alerting))) ;

         var webSites = new List<Website> {
                  new Website ("http://www.INVALID.hu"),
                  new Website ("http://www.intensifight.hu")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, webSites),
                                      "testPackage1", Guid.NewGuid().ToString(), false,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS,
                                      true, states) ;

         Assert.LessOrEqual (1, _watcherServer.PackageController.PackageCount) ;
         WaitForPackageState (packageID) ;

         // Play with state
         _watcherServer.PackageController.UsePackage (packageID, package => {
            Assert.IsInstanceOf<PackageState.Alerting> (package.State) ;
         }) ;
      }

      [Test]
      public void Package_ConfigureAlerted_ShouldNotSendAlert() {
         StartServer() ;

         var token = Login() ;

         var states = new InstanceStateList() ;
         states.Add ("http://www.INVALID.hu",
                     new InstanceState.Alerting (new XConstantString ("WEBSITE_SUPERVISOR.ErrorAccessingWebsiteMessage",
                                                                      new XConstantString.Parameter ("Website", "http://www.INVALID.hu")),
                                                 new XConstantString ("WEBSITE_SUPERVISOR.ErrorAccessingWebsiteDetails",
                                                                      new XConstantString.Parameter ("ErrorMessage", "The remote name could not be resolved: 'http://www.INVALID.hu'.")),
                                                 new XConstantString ("WEBSITE_SUPERVISOR.ErrorAccessingWebsiteExtract"))) ;

         states.Add ("http://www.intensifight.hu",
                     new InstanceState.Alerting (new XConstantString ("WEBSITE_SUPERVISOR.ErrorAccessingWebsiteMessage",
                                                                      new XConstantString.Parameter ("Website", "http://www.intensifight.hu")),
                                                 new XConstantString ("WEBSITE_SUPERVISOR.ErrorAccessingWebsiteDetails",
                                                                      new XConstantString.Parameter ("ErrorMessage", "The remote name could not be resolved: 'www.intensifight.hu'.")),
                                                 new XConstantString ("WEBSITE_SUPERVISOR.ErrorAccessingWebsiteExtract"))) ;

         var webSites = new List<Website> {
                  new Website ("http://www.INVALID.hu"),
                  new Website ("http://www.intensifight.hu")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, webSites, 10, String.Empty, 1),
                                      "testPackage1", Guid.NewGuid().ToString(), false,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS,
                                      true, states) ;

         Assert.LessOrEqual (1, _watcherServer.PackageController.PackageCount) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;

         // Wait a bit, no alert should be raised
         WaitForMeasuredData() ;
         WaitForAlerts (0) ;
      }

      [Test]
      public void SendGoodResults_CheckIfPackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;

         CheckManagementServerMockStatus() ;

         Assert.AreEqual (0, _watcherServer.PackageController.PackageCount) ;
         Assert.AreEqual (0, ManagementServer.MeasuredDataList.Count) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var webSites = new List<Website> {
                  new Website ("http://www.sysment.hu"),
                  new Website ("www.guartinelX.com")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, webSites,
                                                                                     checkLoadTimeSeconds: 25),
                                      "packageName1", Guid.NewGuid().ToString(), true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS,
                                      false) ;
         CheckStatus (token, 1) ;

         // Wait a bit
         WaitForMeasuredData() ;
         var packageState = WaitForPackageState (packageID) ;

         // Check package status
         Assert.IsTrue (packageState.Message.ToJsonString().Contains ("COMMON.AlertStatusMessage"), packageState.Message.ToJsonString()) ;

         // Check contents of the states
         Assert.AreEqual ("ok", packageState.InstanceStates ["http://www.sysment.hu"].Name.ToLowerInvariant()) ;
         Assert.AreEqual ("alerting", packageState.InstanceStates ["www.guartinelX.com"].Name.ToLowerInvariant()) ;
         var message1 = packageState.InstanceStates ["http://www.sysment.hu"].Message ;
         Assert.IsTrue (message1.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKMessage"), message1.ToJsonString()) ;
         Assert.IsTrue (message1.ToJsonString().Contains ("http://www.sysment.hu"), message1.ToJsonString()) ;
         var message2 = packageState.InstanceStates ["www.guartinelX.com"].Message ;
         Assert.IsTrue (message2.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteCheckErrorMessage"), message2.ToJsonString()) ;

         // Check order of the states         
         Assert.AreEqual (packageState.InstanceStates.Keys.ToArray() [0], "www.guartinelX.com") ;
         Assert.AreEqual (packageState.InstanceStates.Keys.ToArray() [1], "http://www.sysment.hu") ;
      }

      [Test]
      public void SendBadResults_CheckIfPackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, _watcherServer.PackageController.PackageCount) ;
         Assert.AreEqual (0, ManagementServer.MeasuredDataList.Count) ;
         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         var webSites = new List<Website> {
                  new Website ("www.guartinelX.com")
         } ;

         var packageID = SavePackage (token, DateTime.UtcNow,
                                      x => Configuration.CreatePackageConfiguration (x, webSites, Constants.CHECK_INTERVAL_SECONDS - 5, ""), // , 1, 5),
                                      "packageName1", Guid.NewGuid().ToString(), true,
                                      Constants.CHECK_INTERVAL_SECONDS,
                                      Constants.TIMEOUT_SECONDS,
                                      Constants.STARTUP_DELAY_SECONDS) ;
         CheckStatus (token, 1) ;

         WaitForMeasuredData() ;
         WaitForPackageState(packageID) ;

         // Check package status
         var lastState = ManagementServer.PackageStates.LastOrDefault (packageState => packageState.PackageID == packageID) ;
         Assert.IsNotNull (lastState) ;
         Assert.IsFalse (lastState.Message.ToJsonString().Contains ("COMMON.OKStatusMessage"), lastState.Message.ToJsonString()) ;
         Assert.IsTrue (lastState.Message.ToJsonString().Contains ("COMMON.AlertStatusMessage"), lastState.Message.ToJsonString()) ;
         Assert.IsTrue (lastState.Details.ToJsonString().ToLowerInvariant().Contains ("www.guartinelx.com"), lastState.Details.ToJsonString()) ;

         // Check contents of the states
         Assert.AreEqual (lastState.InstanceStates ["www.guartinelX.com"].Name.ToLowerInvariant(), "alerting") ;
         Assert.IsTrue (lastState.InstanceStates ["www.guartinelX.com"].Message.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteCheckErrorMessage"), lastState.InstanceStates ["www.guartinelX.com"].Message.ToJsonString()) ;
      }

      [Test]
      public void SetupWrongAddress_CheckCacheHandling() {
         const string URL = @"http://XEhuneX.com" ;

         // Success to cache
         WebsiteCheckCache.Register (URL, 3000, "ehune", null) ;

         TestSite (URL, true, true) ;
      }
   }
}