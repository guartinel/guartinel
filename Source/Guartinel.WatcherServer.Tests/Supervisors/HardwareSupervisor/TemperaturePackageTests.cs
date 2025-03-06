using System;
using System.Collections.Generic ;
using System.Linq;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors ;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SaveConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.Save;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.RegisterMeasurement.Request;
using HardwareConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;
using Timeout = Guartinel.Kernel.Timeout;

namespace Guartinel.WatcherServer.Tests.Supervisors.HardwareSupervisor {
   [TestFixture]
   public class TemperaturePackageTests : PackageTestsBase {
      protected JObject CreateTemperatureConfiguration (bool includeHumidity) {
         JObject instance = new JObject() ;
         instance [SaveConstants.Request.Configuration.Instance.HARDWARE_TYPE] = "hardware_type_temperature_dht22"; // typeof(TemperatureSensorDht22).Name ;
         instance [SaveConstants.Request.Configuration.Instance.INSTANCE_ID] = Constants.INSTANCE_ID ;
         instance [SaveConstants.Request.Configuration.Instance.NAME] = Constants.INSTANCE_NAME ;
         JObject temperature = new JObject() ;
         temperature [HardwareConstants.Temperature.Instance.TemperatureCelsius.MIN_THRESHOLD] = 22.5 ;
         temperature [HardwareConstants.Temperature.Instance.TemperatureCelsius.MAX_THRESHOLD] = 29.5 ;
         instance [HardwareConstants.Temperature.Instance.TEMPERATURE_CELSIUS] = temperature ;

         if (includeHumidity) {
            JObject humidity = new JObject() ;
            humidity [HardwareConstants.Temperature.Instance.TemperatureCelsius.MAX_THRESHOLD] = 89 ;
            instance [HardwareConstants.Temperature.Instance.RELATIVE_HUMIDITY_PERCENT] = humidity ;
         }

         return instance ;
      }

      public static JObject CreateTemperatureAndHumidityMeasuredData (string token,
                                                                      string packageID,
                                                                      string instanceID,
                                                                      string instanceName,
                                                                      double? temperatureCelsius,
                                                                      double? relativeHumidityPercent) {
         JObject parameters = new JObject() ;
         parameters [MeasuredDataConstants.TOKEN] = token ;

         JArray packageIDs = new JArray (new string[] {packageID}) ;
         parameters [MeasuredDataConstants.PACKAGE_IDS] = packageIDs; 
         parameters.Add (MeasuredDataConstants.INSTANCE_ID, instanceID) ;
         parameters.Add (MeasuredDataConstants.INSTANCE_NAME, instanceName) ;
         JObject measuredData = new JObject() ;
         measuredData [HardwareConstants.Temperature.Measurement.TEMPERATURE_CELSIUS] = temperatureCelsius ;
         measuredData [HardwareConstants.Temperature.Measurement.RELATIVE_HUMIDITY_PERCENT] = relativeHumidityPercent ;
         parameters.Add (MeasuredDataConstants.MEASURED_DATA, measuredData) ;
         parameters.Add (MeasuredDataConstants.MEASUREMENT_TIMESTAMP, DateTime.UtcNow) ;

         return parameters ;
      }

      [Test]
      public void NoConfigurationOfInstance_SendData_CheckAlert() {
         const int CHECK_INTERVAL_SECONDS = 2 ;
         const int CHECK_TIMEOUT_SECONDS = 4 ;

         StartServer() ;

         var token = Login() ;

         // var instance = CreateTemperatureConfiguration(true);

         var packageID = SavePackage (token, null, CHECK_INTERVAL_SECONDS, CHECK_TIMEOUT_SECONDS, Constants.STARTUP_DELAY_SECONDS) ;

         ManagementServer.SetApplicationInstanceIDs (packageID, new List<string> {Constants.INSTANCE_NAME}) ;

         new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + CHECK_INTERVAL_SECONDS + 8)).WaitFor (() => (ManagementServer.DeviceAlerts.Any()) &&
                                                                                                                          (ManagementServer.MailAlerts.Any())) ;

         Assert.AreEqual (0, ManagementServer.DeviceAlerts.Count) ;
         Assert.AreEqual (0, ManagementServer.MailAlerts.Count) ;

         ManagementServer.PackageStates.Clear() ;

         // Start sending measurements - do not know if they are OK
         var data = CreateTemperatureAndHumidityMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME,
                                                             23, 55) ;
         SendMeasuredData (data) ;

         // Wait for alerts
         new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + CHECK_INTERVAL_SECONDS + 15)).WaitFor (() => (ManagementServer.DeviceAlerts.Count >= 2) &&
                                                                                                                           (ManagementServer.MailAlerts.Count >= 3)) ;

         new Timeout (TimeSpan.FromSeconds (15)).WaitFor (() => ManagementServer.PackageStates.Any()) ;

         Assert.Greater (ManagementServer.PackageStates.Count, 0) ;

         Assert.AreEqual ("alerting", ManagementServer.PackageStates.Last().State) ;

         Assert.GreaterOrEqual (ManagementServer.DeviceAlerts.Count, 2) ;
         Assert.GreaterOrEqual (ManagementServer.MailAlerts.Count, 3) ;

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.Any (x => x.Details.Contains ("HARDWARE_SUPERVISOR.InstanceNotConfiguredAlertDetails"))) ;
      }

      [Test]
      public void TemperatureHumidity_SendAlertingResults_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instance = CreateTemperatureConfiguration (true) ;
         var packageID = SavePackage (token, instance) ;

         // Not OK
         var measurementAlerting = CreateTemperatureAndHumidityMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME,
                                                                            21, 92) ;
         SendResultsRunCheck (instance, measurementAlerting, true) ;

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.Any (x => x.Details.Contains ("HARDWARE_SUPERVISOR.MeasurementAlertDetails"))) ;
         Assert.IsTrue (ManagementServer.MailAlerts.Any (x => x.Details.Contains ("HARDWARE_SUPERVISOR.MeasurementAlertDetailsRelativeHumidity"))) ;
      }

      [Test]
      public void TemperatureNoHumidity_SendAlertingResults_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instance = CreateTemperatureConfiguration (false) ;
         var packageID = SavePackage (token, instance) ;

         // Not OK
         var measurementAlerting = CreateTemperatureAndHumidityMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME,
                                                                            21, 55) ;

         ManagementServer.PackageStates.Clear() ;
         SendResultsRunCheck (instance, measurementAlerting, true) ;

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.Any (x => x.Details.Contains ("HARDWARE_SUPERVISOR.MeasurementAlertDetails"))) ;
         Assert.IsFalse (ManagementServer.MailAlerts.Any (x => x.Details.Contains ("HARDWARE_SUPERVISOR.MeasurementAlertDetailsRelativeHumidity"))) ;

         new Timeout (TimeSpan.FromSeconds (10)).WaitFor (() => ManagementServer.PackageStates.Count > 0) ;

         Assert.IsTrue (ManagementServer.PackageStates.Any()) ;
         Assert.AreEqual ("alerting", ManagementServer.PackageStates [0].State) ;
      }

      [Test]
      public void TemperatureNoHumidity_SendGoodResults_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instance = CreateTemperatureConfiguration (false) ;
         var packageID = SavePackage (token, instance) ;

         // OK
         var measurementOK = CreateTemperatureAndHumidityMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME,
                                                                      24, 55) ;
         ManagementServer.PackageStates.Clear() ;
         SendResultsRunCheck (instance, measurementOK, false) ;
         new Timeout (TimeSpan.FromSeconds (10)).WaitFor (() => ManagementServer.PackageStates.Count > 0) ;

         Assert.IsTrue (ManagementServer.PackageStates.Any()) ;
         Assert.AreEqual ("ok", ManagementServer.PackageStates [0].State) ;
      }

      [Test]
      public void Temperature_SendErrorValues_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instance = CreateTemperatureConfiguration (true) ;
         var packageID = SavePackage (token, instance) ;

         // Error
         var measurementError = CreateTemperatureAndHumidityMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME,
                                                                         -1000, 55) ;
         SendMeasuredData (measurementError) ;

         new Timeout (TimeSpan.FromSeconds (Constants.STARTUP_DELAY_SECONDS + 3)).WaitFor (() => (ManagementServer.DeviceAlerts.Count >= 2) &&
                                                                                                 (ManagementServer.MailAlerts.Count >= 3)) ;

         Assert.AreEqual(2, ManagementServer.DeviceAlerts.Count);
         Assert.AreEqual(3, ManagementServer.MailAlerts.Count);

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.Any (x => x.Details.Contains ("HARDWARE_SUPERVISOR.MeasurementErrorAlertDetails"))) ;
         // Assert.IsTrue(ManagementServer.MailAlerts.Any(x => x.Details.Contains("HARDWARE_SUPERVISOR.MeasurementErrorAlertDetailsRelativeHumidity")));
      }

      [Test]
      public void Humidity_SendErrorValues_RunCheck() {
         StartServer() ;

         var token = Login() ;

         var instance = CreateTemperatureConfiguration (true) ;
         var packageID = SavePackage (token, instance) ;

         // Error
         var measurementError = CreateTemperatureAndHumidityMeasuredData (token, packageID, Constants.INSTANCE_ID, Constants.INSTANCE_NAME,
                                                                         24, -1000) ;
         SendMeasuredData(measurementError);

         new Timeout(TimeSpan.FromSeconds(Constants.STARTUP_DELAY_SECONDS + 3)).WaitFor(() => (ManagementServer.DeviceAlerts.Count >= 2) &&
                                                                                              (ManagementServer.MailAlerts.Count >= 3));

         Assert.AreEqual(2, ManagementServer.DeviceAlerts.Count);
         Assert.AreEqual(3, ManagementServer.MailAlerts.Count);

         Assert.IsTrue (ManagementServer.MailAlerts.All (x => x.Message.Contains (Constants.INSTANCE_NAME)), ManagementServer.MailAlerts [0].Message) ;
         Assert.IsTrue (ManagementServer.MailAlerts.Any (x => x.Details.Contains ("HARDWARE_SUPERVISOR.MeasurementErrorAlertDetails"))) ;
      }
   }
}