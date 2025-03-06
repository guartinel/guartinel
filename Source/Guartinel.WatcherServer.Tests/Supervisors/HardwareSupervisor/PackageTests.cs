using System ;
using System.Linq ;
using System.Threading ;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor ;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareCheckers ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;
using SaveConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.Save;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.RegisterMeasurement.Request ;
using HardwareConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares ;
using Timeout = Guartinel.Kernel.Timeout ;

namespace Guartinel.WatcherServer.Tests.Supervisors.HardwareSupervisor {
   [TestFixture]
   public class PackageTests : PackageTestsBase {      
      public static JObject CreateMeasuredDataCurrent (string token,
                                                       string packageID,
                                                       string instanceID,
                                                       string instanceName,
                                                       double current) {
         var measuredData = CreateMeasuredData (token, packageID, instanceID, instanceName, null) ;
         var data = measuredData [MeasuredDataConstants.MEASUREMENT] as JObject ;
         Assert.IsNotNull (data) ;
         data ["current_a"] = 3.1 ;
         measuredData [MeasuredDataConstants.MEASUREMENT] = data ;
         return measuredData ;
      }

      protected JObject Create230VConfiguration(double? thresholdMinVolts = 220.0,
                                                double? thresholdMaxVolts = 245.0,
                                                bool isSensitive = false) {
         JObject instance = new JObject() ;
         instance [SaveConstants.Request.Configuration.Instance.TYPE] = typeof(VoltageChecker230V).Name ;
         instance [SaveConstants.Request.Configuration.Instance.ID] = Constants.INSTANCE_ID ;
         instance [SaveConstants.Request.Configuration.Instance.NAME] = Constants.INSTANCE_NAME ;
         if (isSensitive) {
            instance[Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Properties.CHECK_MIN_MAX] = true ;
         }
         
         instance [Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Properties.MIN_THRESHOLD] = thresholdMinVolts ;
         instance [Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Properties.MAX_THRESHOLD] = thresholdMaxVolts ;

         return instance ;
      }

      [Test]
      public void AddPackage_230V_DoNotSendResults_CheckTimeout() {
         const int CHECK_INTERVAL_SECONDS = 2 ;
         const int CHECK_TIMEOUT_SECONDS = 4 ;

         StartServer() ;

         var token = Login() ;

         var instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance, 1, CHECK_INTERVAL_SECONDS, CHECK_TIMEOUT_SECONDS) ;

         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + CHECK_TIMEOUT_SECONDS + 13)).WaitFor (() => ManagementServer.MailAlerts.Count (x => x.PackageID == packageID) > 0) ;

         Assert.Greater (ManagementServer.MailAlerts.Count (x => x.PackageID == packageID), 1) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Details) ;
         // Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      [Test]
      public void AddPackage_230V_SendGoodResults_CheckNoTimeout() {
         const int CHECK_INTERVAL_SECONDS = 2 ;
         const int CHECK_TIMEOUT_SECONDS = 4 ;

         StartServer() ;

         var token = Login() ;

         var instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance, 1, CHECK_INTERVAL_SECONDS, CHECK_TIMEOUT_SECONDS) ;

         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Timeout runtime = new Timeout (TimeSpan.FromSeconds (CHECK_TIMEOUT_SECONDS * 6)) ;

         while (runtime.StillOK) {
            var measurementGood = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 980, HardwareCheckBoolean.On) ;
            SendResultsRunCheck (instance, measurementGood, false) ;
            Thread.Sleep (500) ;
         }

         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
      }

      [Test]
      public void AddPackage_230V_CheckTimeout_SendResults_CheckIfRecovered() {
         const int CHECK_INTERVAL_SECONDS = 2 ;
         const int CHECK_TIMEOUT_SECONDS = 4 ;

         StartServer() ;

         var token = Login() ;

         var instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance, 1, CHECK_INTERVAL_SECONDS, CHECK_TIMEOUT_SECONDS) ;

         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         new Timeout (TimeSpan.FromSeconds (CHECK_TIMEOUT_SECONDS + 3)).WaitFor (() => ManagementServer.MailAlerts.Count (x => x.PackageID == packageID) > 0) ;

         Assert.Greater (ManagementServer.MailAlerts.Count (x => x.PackageID == packageID), 1) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Details) ;
         // Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.DETAILS)), ManagementServer.MailAlerts [0].Details) ;

         var measurementGood = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 900, HardwareCheckBoolean.On) ;
         SendResultsRunCheck (instance, measurementGood, false) ;

         new Timeout (TimeSpan.FromSeconds (CHECK_TIMEOUT_SECONDS + 3)).WaitFor (() => ManagementServer.MailAlerts.Last (x => x.PackageID == packageID).IsRecovery) ;

         Assert.IsTrue (ManagementServer.MailAlerts.Last (x => x.PackageID == packageID).IsRecovery) ;
      }

      [Test]
      public void AddPackage_230V_SendAlertingResults_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance) ;

         // 300: too low
         var measuredDataAlerting = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 300, HardwareCheckBoolean.On) ;
         SendResultsRunCheck (instance, measuredDataAlerting, true) ;

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      [Test]
      public void AddPackage_230V_SendAlertingMinResult_RunCheck() {
         StartServer() ;

         var token = Login() ;

         JObject instance = Create230VConfiguration(220, null, true) ;
         var packageID = SavePackage (token, instance) ;

         var measuredDataAlerting = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 1000, HardwareCheckBoolean.On) ;
         var measuredData = measuredDataAlerting [MeasuredDataConstants.MEASUREMENT] as JObject ;
         Assert.IsNotNull (measuredData) ;
         measuredData ["A0_min"] = 800.0 ;
         measuredDataAlerting [MeasuredDataConstants.MEASUREMENT] = measuredData ;
         SendResultsRunCheck (instance, measuredDataAlerting, true) ;

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.SENSITIVE_DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      [Test]
      public void AddPackage_230V_SendAlertingMinResult_SendGoodResult_RunCheck() {
         StartServer() ;

         var token = Login() ;

         JObject instance = Create230VConfiguration (220, null, true) ;
         var packageID = SavePackage (token, instance) ;

         var measuredDataAlerting = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 1000, HardwareCheckBoolean.On) ;
         var measuredData = measuredDataAlerting [MeasuredDataConstants.MEASUREMENT] as JObject ;
         Assert.IsNotNull (measuredData) ;
         measuredData ["A0_min"] = 900.0 ;
         measuredDataAlerting [MeasuredDataConstants.MEASUREMENT] = measuredData ;
         SendMeasuredData (measuredDataAlerting) ;
         var measuredDataGood = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 1000, HardwareCheckBoolean.On) ;
         SendResultsRunCheck (instance, measuredDataGood, false) ;
      }

      [Test]
      public void AddPackage_230V_SendDifferentAlertingResults_ShouldGetOnlyOneAlert() {
         const int CHECK_INTERVAL_SECONDS = 2 ;

         StartServer() ;

         var token = Login() ;

         var instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance, 1, CHECK_INTERVAL_SECONDS) ;

         // 300: too low
         var measurement1 = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 300, HardwareCheckBoolean.On) ;
         SendResultsRunCheck (instance, measurement1, true) ;

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.DETAILS)), ManagementServer.MailAlerts [0].Details) ;

         int mailAlertCount = ManagementServer.MailAlerts.Count ;

         var measurement2 = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 312, HardwareCheckBoolean.On) ;
         SendMeasuredData (measurement2) ;
         new Timeout (TimeSpan.FromSeconds (CHECK_INTERVAL_SECONDS * 3)).WaitFor (() => ManagementServer.MailAlerts.Count > mailAlertCount) ;
         Assert.AreEqual (mailAlertCount, ManagementServer.MailAlerts.Count, ManagementServer.MailAlerts.Last().Details) ;
      }

      [Test]
      public void AddPackage_230V_SendGoodResults_RunCheck() {
         StartServer() ;

         var token = Login() ;
         var instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance) ;

         // 1020: OK!
         var measurementGood = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 1020, HardwareCheckBoolean.On) ;
         SendResultsRunCheck (instance, measurementGood, false) ;
      }

      protected JObject Create230V3ChannelConfiguration (int? a0MinThreshold = null,
                                                         int? a0MaxThreshold = null,
                                                         HardwareCheckBoolean? d2State = null,
                                                         HardwareCheckBoolean? d3State = null,
                                                         string ch1Name = "",
                                                         string ch2Name = "",
                                                         string ch3Name = "") {
         JObject instance = new JObject() ;
         instance [SaveConstants.Request.Configuration.Instance.TYPE] = typeof(VoltageChecker230V3Channel).Name ;
         instance [SaveConstants.Request.Configuration.Instance.ID] = Constants.INSTANCE_ID ;
         instance [SaveConstants.Request.Configuration.Instance.NAME] = Constants.INSTANCE_NAME ;

         JObject channel1 = new JObject() ;
         channel1 [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel1.MIN_THRESHOLD] = a0MinThreshold ;
         channel1[HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel1.MAX_THRESHOLD] = a0MaxThreshold;

         if (!string.IsNullOrEmpty (ch1Name)) {
            channel1 [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel1.NAME] = ch1Name ;
         }

         instance [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.CHANNEL_1] = channel1 ;

         if (d2State != null) {
            JObject channel2 = new JObject() ;
            channel2 [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel2.EXPECTED_STATE] = d2State.Value.ToString() ;
            if (!string.IsNullOrEmpty (ch2Name)) {
               channel2 [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel2.NAME] = ch2Name ;
            }

            instance [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.CHANNEL_2] = channel2 ;
         }

         if (d3State != null) {
            JObject channel3 = new JObject() ;
            channel3 [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel3.EXPECTED_STATE] = d3State.Value.ToString() ;
            if (!string.IsNullOrEmpty (ch3Name)) {
               channel3 [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel3.NAME] = ch3Name ;
            }

            instance [HardwareConstants.VoltageLevel.Max230V.ThreeChannel.Instance.CHANNEL_3] = channel3 ;
         }

         return instance ;
      }

      [Test]
      public void AddPackage_230V3Channel_SendAlertingResultsA0_RunCheck() {
         StartServer() ;

         var token = Login() ;
         JObject instance = Create230V3ChannelConfiguration (215, null, null, null, "ch1NameEhune") ;
         var packageID = SavePackage (token, instance) ;

         var measurementAlerting = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 300) ;
         SendResultsRunCheck (instance, measurementAlerting, true, null, new[] {"ch1NameEhune"}) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.NAMED_DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      [Test]
      public void AddPackage_230V3ChannelNoD2_SendAlertingResultD2On_RunCheck() {
         StartServer() ;

         var token = Login() ;
         JObject instance = Create230V3ChannelConfiguration (null, null, HardwareCheckBoolean.Off, null, "ch1NameEhune", "ch2NameEhune") ;
         var packageID = SavePackage (token, instance) ;

         var measurementAlerting = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, null, null, HardwareCheckBoolean.On) ;
         // SendResultsRunCheck (instance, measurementAlerting, false, null, new[] {"ch2NameEhune"}) ;
         SendResultsRunCheck(instance, measurementAlerting, false) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.NAMED_DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      [Test]
      public void AddPackage_230V3Channel_D1D2On_D3Off_SendGoodResuls_RunCheck() {
         StartServer() ;

         var token = Login() ;
         JObject instance = Create230V3ChannelConfiguration (150, null,
                                                             HardwareCheckBoolean.On,
                                                             HardwareCheckBoolean.Off,
                                                             "ch1NameEhune", "ch2NameEhune", "ch3NameEhune") ;
         var packageID = SavePackage (token, instance) ;

         var measurement = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 963,
                                               HardwareCheckBoolean.On,
                                               HardwareCheckBoolean.On,
                                               HardwareCheckBoolean.Off) ;
         SendResultsRunCheck (instance, measurement, false) ;
      }

      [Test]
      public void AddPackage_230V3Channel_SendAlertingResultsD3_RunCheck() {
         StartServer() ;

         var token = Login() ;
         JObject instance = Create230V3ChannelConfiguration (null, null, null, HardwareCheckBoolean.On) ;
         var packageID = SavePackage (token, instance) ;

         var measurementAlerting = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 1020,
                                                       HardwareCheckBoolean.On, HardwareCheckBoolean.On, HardwareCheckBoolean.Off) ;
         SendResultsRunCheck (instance, measurementAlerting, true) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.NAMED_DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      protected void SendResultsCheckIfPackageStateIsSentToMS (JObject instance,
                                                               JObject measurement,
                                                               bool isAlerting) {

         ManagementServer.DeviceAlerts.Clear() ;
         ManagementServer.MailAlerts.Clear() ;
         ManagementServer.PackageStates.Clear() ;

         // var instanceID = measurement[MeasurementConstants.INSTANCE_ID].ToString();
         var packageID = measurement [MeasuredDataConstants.PACKAGE_ID].ToString() ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.PackageStates.Count) ;

         ManagementServer.PackageStates.Clear() ;

         SendMeasuredData (measurement) ;

         new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + 2)).Wait() ;

         new Timeout (TimeSpan.FromSeconds (7)).WaitFor (() => ManagementServer.PackageStates.Count > 1) ;

         // Check package status
         var lastState = ManagementServer.PackageStates.LastOrDefault (packageState => packageState.PackageID == packageID) ;
         Assert.IsNotNull (lastState) ;
         string message ;
         if (isAlerting) {
            Assert.IsTrue (lastState.Message.ToJsonString().Contains ("AlertStatus"), lastState.Message.ToJsonString()) ;
            Assert.AreEqual ("alerting", lastState.InstanceStates [Constants.INSTANCE_ID].Name.ToLowerInvariant(), lastState.InstanceStates [Constants.INSTANCE_ID].Name.ToLowerInvariant()) ;

            message = @"""code"":""HARDWARE_SUPERVISOR.MeasurementAlert""" ;

         } else {
            Assert.IsTrue (lastState.Message.ToJsonString().Contains ("OK"), lastState.Message.ToJsonString()) ;
            Assert.AreEqual ("ok", lastState.InstanceStates [Constants.INSTANCE_ID].Name.ToLowerInvariant(), lastState.InstanceStates [Constants.INSTANCE_ID].Name.ToLowerInvariant()) ;

            message = @"""code"":""HARDWARE_SUPERVISOR.MeasurementOK""" ;
         }

         Assert.IsTrue (lastState.InstanceStates [Constants.INSTANCE_ID].Message.ToJsonString().Contains (message),
                        lastState.InstanceStates [Constants.INSTANCE_ID].Message.ToJsonString()) ;

         Assert.IsTrue (lastState.InstanceStates [Constants.INSTANCE_ID].Details.ToJsonString().Contains ("MeasuredValue"),
                        lastState.InstanceStates [Constants.INSTANCE_ID].Details.ToJsonString()) ;
      }

      [Test]
      public void SendAlertingResults_230V_CheckIfPackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;
         JObject instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance) ;

         JObject measurement = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 30) ;

         SendResultsCheckIfPackageStateIsSentToMS (instance, measurement, true) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      [Test]
      public void SendGoodResults_230V_CheckIfPackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;
         JObject instance = Create230VConfiguration() ;
         var packageID = SavePackage (token, instance) ;

         JObject measurement = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 980, HardwareCheckBoolean.On) ;

         SendResultsCheckIfPackageStateIsSentToMS (instance, measurement, false) ;
      }

      [Test]
      public void SendAlertingResults_230V3Channel_CheckIfPackageStateIsSentToMS() {
         StartServer() ;

         var token = Login() ;
         JObject instance = Create230V3ChannelConfiguration (220) ;
         var packageID = SavePackage (token, instance) ;

         JObject measurement = CreateMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 30) ;

         SendResultsCheckIfPackageStateIsSentToMS (instance, measurement, true) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.NAMED_DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      protected JObject Create30AConfiguration(double? minThreshold = 3.5,
                                               double? maxThreshold = 10) {
         JObject instance = new JObject() ;
         instance [SaveConstants.Request.Configuration.Instance.TYPE] = typeof(CurrentChecker30A).Name ;
         instance [SaveConstants.Request.Configuration.Instance.ID] = Constants.INSTANCE_ID ;
         instance [SaveConstants.Request.Configuration.Instance.NAME] = Constants.INSTANCE_NAME ;
         instance [HardwareConstants.CurrentLevel.Instance.MIN_THRESHOLD] = minThreshold ;
         instance [HardwareConstants.CurrentLevel.Instance.MAX_THRESHOLD] = maxThreshold ;

         return instance ;
      }

      [Test]
      public void AddPackage_30A_SendAlertingResults_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instance = Create30AConfiguration() ;
         var packageID = SavePackage (token, instance) ;

         // Not OK
         var measuredData = CreateMeasuredDataCurrent (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 3.1) ;
         SendResultsRunCheck (instance, measuredData, true) ;

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Details.Contains (Constants.DETAILS)), ManagementServer.MailAlerts [0].Details) ;
      }

      [Test]
      public void AddPackage_30A_SendGoodResults_RunCheck() {
         StartServer() ;

         var token = Login() ;
         var instance = Create30AConfiguration() ;
         var packageID = SavePackage (token, instance) ;

         // OK!
         var measurementGood = CreateMeasuredDataCurrent (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME, 3.7) ;
         SendResultsRunCheck (instance, measurementGood, false) ;
      }
   }
}