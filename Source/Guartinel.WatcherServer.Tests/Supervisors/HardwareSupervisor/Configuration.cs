using System ;
using System.Collections.Generic ;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor ;
using Newtonsoft.Json.Linq ;
using Guartinel.Kernel ;
using SaveConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.Save;

namespace Guartinel.WatcherServer.Tests.Supervisors.HardwareSupervisor {
   public class Configuration {
      public static void ConfigureChecker (HardwareInstanceDataChecker checker,
                                           string packageID,
                                           HardwareSensor instance,
                                           List<InstanceData.InstanceData> instanceDataList,
                                           string instanceID,
                                           string instanceName,
                                           int timeoutInSeconds = 30) {
         checker.Configure ("checker1",
                            packageID,
                            instanceID,
                            instanceName,
                            new Timeout (TimeSpan.FromSeconds (timeoutInSeconds)),
                            instance,
                            instanceDataList) ;
      }

      public static void CreatePackageConfiguration (JObject configuration,
                                                     List<JObject> instances) {
         if (configuration == null) return ;
         if (instances != null) {
            JArray instancesJ = new JArray() ;
            foreach (var instance in instances) {
               instancesJ.Add (instance) ;
            }

            configuration [SaveConstants.Request.Configuration.INSTANCES] = instancesJ ;
         }
      }

      public static void CreatePackageConfiguration (JObject configuration,
                                                     JObject instance) {
         CreatePackageConfiguration (configuration, instance == null ? null : new List<JObject> {instance}) ;
      }

      public static JObject CreateMeasuredData (double? a0,
                                                HardwareCheckBoolean? d1,
                                                HardwareCheckBoolean? d2,
                                                HardwareCheckBoolean? d3) {
         JObject result = new JObject() ;

         if (a0 != null) {
            result ["A0"] = a0 ;
         }

         if (d1 != null) {
            result ["D1"] = d1.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d2 != null) {
            result ["D2"] = d2.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d3 != null) {
            result ["D3"] = d3.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         return result ;
      }

      public static JObject CreateMeasuredDataMinMax (double? a0,
                                                      double? a0Min = null,
                                                      double? a0Max = null,
                                                      HardwareCheckBoolean? d1 = null,
                                                      HardwareCheckBoolean? d1Min = null,
                                                      HardwareCheckBoolean? d1Max = null,
                                                      HardwareCheckBoolean? d2 = null,
                                                      HardwareCheckBoolean? d2Min = null,
                                                      HardwareCheckBoolean? d2Max = null,
                                                      HardwareCheckBoolean? d3 = null,
                                                      HardwareCheckBoolean? d3Min = null,
                                                      HardwareCheckBoolean? d3Max = null) {
         JObject result = new JObject() ;

         if (a0 != null) {
            result ["A0"] = a0 ;
         }

         if (a0Min != null) {
            result ["A0_min"] = a0Min ;
         }

         if (a0Max != null) {
            result ["A0_max"] = a0Max ;
         }

         if (d1 != null) {
            result ["D1"] = d1.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d1Min != null) {
            result ["D1_min"] = d1Min.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d1Max != null) {
            result ["D1_max"] = d1Max.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d2 != null) {
            result ["D2"] = d2.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d2Min != null) {
            result ["D2_min"] = d2Min.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d2Max != null) {
            result ["D2_max"] = d2Max.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d3 != null) {
            result ["D3"] = d3.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d3Min != null) {
            result ["D3_min"] = d3Min.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         if (d3Max != null) {
            result ["D3_max"] = d3Max.Value == HardwareCheckBoolean.On ? 1 : 0 ;
         }

         return result ;
      }
   }
}