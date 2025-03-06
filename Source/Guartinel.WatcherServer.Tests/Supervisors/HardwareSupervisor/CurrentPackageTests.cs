using System ;
using System.Linq ;
using System.Threading ;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor ;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;
using SaveConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.Save;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.RegisterMeasurement.Request ;
using HardwareConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares ;
using Timeout = Guartinel.Kernel.Timeout ;

namespace Guartinel.WatcherServer.Tests.Supervisors.HardwareSupervisor {
   [TestFixture]
   public class CurrentPackageTests : PackageTestsBase {      
      public static JObject CreateMeasuredDataCurrent (string token,
                                                       string packageID,
                                                       string instanceID,
                                                       string instanceName,
                                                       double current) {
         var measuredData = CreateMeasuredData (token, packageID, instanceID, instanceName, null, false) ;
         var data = measuredData [MeasuredDataConstants.MEASURED_DATA] as JObject ;
         Assert.IsNotNull (data) ;
         data ["current_a"] = 3.1 ;
         measuredData [MeasuredDataConstants.MEASURED_DATA] = data ;
         return measuredData ;
      }

      protected JObject Create30AConfiguration(double? minThreshold = 3.5,
                                               double? maxThreshold = 10) {
         JObject instance = new JObject() ;
         instance [SaveConstants.Request.Configuration.Instance.HARDWARE_TYPE] = typeof(CurrentChecker30A).Name ;
         instance [SaveConstants.Request.Configuration.Instance.INSTANCE_ID] = Constants.INSTANCE_ID ;
         instance [SaveConstants.Request.Configuration.Instance.NAME] = Constants.INSTANCE_NAME ;
         instance [HardwareConstants.CurrentLevel.InstanceProperties.MIN_THRESHOLD] = minThreshold ;
         instance [HardwareConstants.CurrentLevel.InstanceProperties.MAX_THRESHOLD] = maxThreshold ;

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