using System;
using System.Collections.Generic;
using Guartinel.Communication.Strings;
using Guartinel.Communication.Supervisors.HardwareSupervisor.Languages;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Communication.Supervisors.HardwareSupervisor {
   public class Strings : StringsBase {
      #region General

      public static Strings Use {get ;} = new Strings() ;

      public override string Prefix => PackageType ;
      public string PackageType => "HARDWARE_SUPERVISOR" ;

      private Strings() {
         _languages.Add (new English()) ;
      }
      #endregion

      #region Messages

      public interface IMessages {
         string InstanceNotAvailableAlert {get ;}
         string InstanceNotAvailableAlertDetails {get ;}
         string InstanceNotAvailableAlertExtract { get; }         
         string MeasurementAlertMessage { get ;}
         string MeasurementAlertDetails {get ;}
         string MeasurementAlertExtract { get; }
         string MeasurementWarningMessage {get ;}
         string MeasurementCriticalMessage {get ;}
         string MeasurementSensitiveAlertDetails {get ;}
         string NamedMeasurementAlertDetails {get ;}         
         string MeasurementOKMessage { get ;}         
         string MeasurementOKDetails {get ;}
         string MeasurementOKExtract { get; }
         string MeasurementExtractTemperature { get; }         
         string NamedMeasurementOKDetails {get ;}
         string NamedMeasurementOKExtract { get; }

         // Relative humidity
         string MeasurementOKDetailsRelativeHumidity {get ;}
         string MeasurementAlertDetailsRelativeHumidity {get ;}

         string MeasurementExtractRelativeHumidity { get; }
         string MeasurementDetailsLiquidSensorNo { get; }
         string MeasurementDetailsLiquidSensorYes { get; }
         string MeasurementExtractLiquidSensorNo { get; }
         string MeasurementExtractLiquidSensorYes { get; }

         string InstanceNotConfiguredAlert {get ;}
         string InstanceNotConfiguredAlertDetails {get ;}
         string InstanceNotConfiguredAlertExtract { get; }         

         string MeasurementErrorAlertDetails {get ;}
         string MeasurementErrorAlertExtract { get; }         
         // string InstanceNotConfiguredAlertDetailsTemperatureAndHumidity { get; }
      }

      public class Messages : IMessages {
         public static Messages Use {get ;} = new Messages() ;

         public string InstanceNotAvailableAlert => nameof(InstanceNotAvailableAlert) ;
         public string InstanceNotAvailableAlertDetails => nameof(InstanceNotAvailableAlertDetails) ;
         public string InstanceNotAvailableAlertExtract => nameof(InstanceNotAvailableAlertExtract) ;
         public string MeasurementAlertMessage => nameof(MeasurementAlertMessage) ;                  
         public string MeasurementAlertDetails => nameof(MeasurementAlertDetails) ;
         public string MeasurementAlertExtract => nameof(MeasurementAlertExtract);
         public string MeasurementWarningMessage => nameof(MeasurementWarningMessage) ;
         public string MeasurementCriticalMessage => nameof(MeasurementCriticalMessage) ;
         public string MeasurementSensitiveAlertDetails => nameof(MeasurementSensitiveAlertDetails) ;
         public string MeasurementSensitiveWarningDetails => nameof(MeasurementSensitiveWarningDetails) ;
         public string NamedMeasurementAlertDetails => nameof(NamedMeasurementAlertDetails) ;         
         public string MeasurementOKMessage => nameof(MeasurementOKMessage) ;
         public string MeasurementOKDetails => nameof(MeasurementOKDetails) ;
         public string MeasurementOKExtract => nameof(MeasurementOKExtract) ;
         public string NamedMeasurementOKDetails => nameof(NamedMeasurementOKDetails);
         public string NamedMeasurementOKExtract => nameof(NamedMeasurementOKExtract) ;         
         public string MeasurementExtractTemperature => nameof(MeasurementExtractTemperature);
         public string MeasurementOKDetailsRelativeHumidity => nameof(MeasurementOKDetailsRelativeHumidity);
         public string MeasurementAlertDetailsRelativeHumidity => nameof(MeasurementAlertDetailsRelativeHumidity);
         public string MeasurementExtractRelativeHumidity => nameof(MeasurementExtractRelativeHumidity);
         public string MeasurementDetailsLiquidSensorNo => nameof(MeasurementDetailsLiquidSensorNo);
         public string MeasurementDetailsLiquidSensorYes => nameof(MeasurementDetailsLiquidSensorYes);
         public string MeasurementExtractLiquidSensorNo => nameof(MeasurementExtractLiquidSensorNo);
         public string MeasurementExtractLiquidSensorYes => nameof(MeasurementExtractLiquidSensorYes);

         public string InstanceNotConfiguredAlert => nameof(InstanceNotConfiguredAlert) ;
         public string InstanceNotConfiguredAlertDetails => nameof(InstanceNotConfiguredAlertDetails) ;
         public string InstanceNotConfiguredAlertExtract => nameof(InstanceNotConfiguredAlertExtract);

         public string MeasurementErrorAlertDetails => nameof(MeasurementErrorAlertDetails) ;
         public string MeasurementErrorAlertExtract => nameof(MeasurementErrorAlertExtract);
      }
      #endregion

      #region Parameters, properties

      public static class Parameters {
         public static string InstanceName => nameof(InstanceName) ;
         public static string PackageName => nameof(PackageName) ;
         public static string Message => nameof(Message) ;
         public static string ReferenceValue => nameof(ReferenceValue) ;

         public static string MeasuredValue => nameof(MeasuredValue) ;

         // public static string MeasuredValue2 => nameof(MeasuredValue2);
         public static string DeviceName => nameof(DeviceName) ;
      }

      public static class Lookups { }

      public override Dictionary<string, string> GetProperties() {
         return Helper.ObjectToDictionary (new Properties()) ;
      }

      public class Properties {
         [JsonProperty]
         public const string ALIVE_VOLT = "alive_volt" ;

         [JsonProperty]
         public const string ID = "id" ;

         [JsonProperty]
         public const string NAME = "name" ;


         [JsonProperty]
         public const string INSTANCE_NAME = "instance_name" ;

         [JsonProperty]
         public const string CAPTION = "caption" ;

         [JsonProperty]
         public const string CHANNEL_1 = "channel_1" ;

         [JsonProperty]
         public const string CHANNEL_2 = "channel_2" ;

         [JsonProperty]
         public const string CHANNEL_3 = "channel_3" ;

         [JsonProperty]
         public const string CHECK_MIN_MAX = "check_min_max" ;

         [JsonProperty]
         [Obsolete ("Use HARDWARE_TYPE instead.")]
         public const string TYPE = "type" ;

         [JsonProperty]
         public const string HARDWARE_TYPE = "hardware_type" ;

         [JsonProperty]
         public const string THRESHOLD = "threshold" ;

         [JsonProperty]
         public const string THRESHOLD_TYPE = "threshold_type" ;

         [JsonProperty]
         public const string IS_ALIVE = "is_alive" ;

         [JsonProperty]
         public const string INSTANCES = "instances" ;

         [JsonProperty]
         public const string INSTANCE_ID = "instance_id" ;

         [JsonProperty]
         public const string INSTANCE_IDS = "instance_ids" ;

         [JsonProperty]
         public const string HARDWARE_TOKEN = "hardware_token" ;

         [JsonProperty]
         public const string MEASUREMENT = "measurement" ;

         [JsonProperty]
         public const string PACKAGE_ID = "package_id" ;

         [JsonProperty]
         public const string IS_HEARTBEAT = "is_heartbeat" ;

         [JsonProperty]
         public const string EXPECTED_STATE = "expected_state" ;

         [JsonProperty]
         public const string FIRMWARE = "firmware" ;

         [JsonProperty]
         public const string TEMPERATURE_CELSIUS = "temperature_celsius" ;

         [JsonProperty]
         public const string RELATIVE_HUMIDITY_PERCENT = "relative_humidity_percent" ;

         [JsonProperty]
         public const string CO2_PPM = "co2_ppm" ;

         [JsonProperty]
         public const string MEASUREMENT_TIMESTAMP = AllParameters.MEASUREMENT_TIMESTAMP ;

         [JsonProperty]
         public const string MIN_THRESHOLD = "min_threshold" ;

         [JsonProperty]
         public const string MAX_THRESHOLD = "max_threshold" ;

         [JsonProperty]
         public const string VALUE = "value" ;

         [JsonProperty]
         public const string WATER_PRESENCE = "water_presence" ;

         [JsonProperty]
         public const string HARDWARE_TYPE_CURRENT_LEVEL_MAX_30A = Hardwares.CurrentLevel.Max30A.FULL_TYPE ;

         [JsonProperty]
         public const string HARDWARE_TYPE_CURRENT_LEVEL_MAX_50A = Hardwares.CurrentLevel.Max50A.FULL_TYPE ;

         [JsonProperty]
         public const string HARDWARE_TYPE_CURRENT_LEVEL_MAX_100A = Hardwares.CurrentLevel.Max100A.FULL_TYPE ;

         [JsonProperty]
         public const string MIN_WARNING_THRESHOLD = "min_warning_threshold" ;

         [JsonProperty]
         public const string MAX_WARNING_THRESHOLD = "max_warning_threshold" ;

         [JsonProperty]
         public const string HARDWARE_TYPE_VOLTAGE_LEVEL_MAX_230V_ONE_CHANNEL = Hardwares.VoltageLevel.Max230V.OneChannel.FULL_TYPE ;

         [JsonProperty]
         public const string HARDWARE_TYPE_VOLTAGE_LEVEL_MAX_230V_THREE_CHANNEL = Hardwares.VoltageLevel.Max230V.ThreeChannel.FULL_TYPE ;

         [JsonProperty]
         public const string HARDWARE_TYPE_TEMPERATURE_DHT22 = Hardwares.Temperature.DHT22.FULL_TYPE ;

         [JsonProperty]
         public const string HARDWARE_TYPE_TEMPERATURE_DHT11 = Hardwares.Temperature.DHT11.FULL_TYPE ;

         [JsonProperty]
         public const string HARDWARE_TYPE_GAS_MQ135 = Hardwares.Gas.MQ135.FULL_TYPE ;

         [JsonProperty]
         public const string HARDWARE_TYPE_WATER_PRESENCE = Hardwares.Water.Presence.FULL_TYPE ;
      }

      public static class Hardwares {
         public const string PREFIX = "hardware_type" ;

         public static class GeneralMeasuredData {
            //D1,D2,D3 digital input 1=there is input on port, 0 = no input, A0 is analog input maximum value 1023 min value 0;
            public const string D1 = "D1" ;
            public const string D1_MIN = "D1_min" ;
            public const string D1_MAX = "D1_max" ;
            public const string D2 = "D2" ;
            public const string D2_MIN = "D1_min" ;
            public const string D2_MAX = "D1_max" ;
            public const string D3 = "D3" ;
            public const string D3_MIN = "D3_min" ;
            public const string D3_MAX = "D3_max" ;
            public const string A0 = "A0" ;
            public const string A0_MAX = "A0_max" ;
            public const string A0_MIN = "A0_min" ;
         }

         public static class Water {
            public const string TYPE = PREFIX + "_water" ;

            public static class Presence {
               public const string FULL_TYPE = TYPE + "_presence" ;

               public class Measurement {
                  public const string WATER_PRESENCE = Properties.WATER_PRESENCE ;
                  public const int VALUE_ON_PRESENCE = 1 ;
               }

               public static class Instance {
                  public const string NAME = Properties.NAME ;
                  public const string ID = Properties.ID ;

                  [Obsolete ("Use HARDWARE_TYPE instead.")]
                  public const string TYPE = Properties.TYPE ;

                  public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
                  public const string EXPECTED_STATE = Properties.EXPECTED_STATE ;
               }
            }
         }

         public static class Temperature {
            public const string TYPE = PREFIX + "_temperature" ;

            public class Measurement {
               public const string TEMPERATURE_CELSIUS = Properties.TEMPERATURE_CELSIUS ; // float value
               public const string RELATIVE_HUMIDITY_PERCENT = Properties.RELATIVE_HUMIDITY_PERCENT ; // float value
               public const int FAILED_READING_VALUE = -1000 ;
            }

            public static class Instance {
               public const string NAME = Properties.NAME ;
               public const string ID = Properties.ID ;

               [Obsolete ("Use HARDWARE_TYPE instead.")]
               public const string TYPE = Properties.TYPE ;

               public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
               public const string TEMPERATURE_CELSIUS = Properties.TEMPERATURE_CELSIUS ;
               public const string RELATIVE_HUMIDITY_PERCENT = Properties.RELATIVE_HUMIDITY_PERCENT ;

               public static class TemperatureCelsius {
                  public const string MIN_THRESHOLD = Properties.MIN_THRESHOLD ;
                  public const string MAX_THRESHOLD = Properties.MAX_THRESHOLD ;

                  public const string MIN_WARNING_THRESHOLD = Properties.MIN_WARNING_THRESHOLD ;
                  public const string MAX_WARNING_THRESHOLD = Properties.MAX_WARNING_THRESHOLD ;
               }

               public static class RelativeHumidityPercent {
                  public const string MIN_THRESHOLD = Properties.MIN_THRESHOLD ;
                  public const string MAX_THRESHOLD = Properties.MAX_THRESHOLD ;

                  public const string MIN_WARNING_THRESHOLD = Properties.MIN_WARNING_THRESHOLD ;
                  public const string MAX_WARNING_THRESHOLD = Properties.MAX_WARNING_THRESHOLD ;
               }
            }

            public static class DHT11 {
               public const string FULL_TYPE = TYPE + "_dht11" ;
            }

            public static class DHT22 {
               public const string FULL_TYPE = TYPE + "_dht22" ;
            }

            public static class DS18B20 {
               public const string FULL_TYPE = TYPE + "_ds18b20" ;
            }
         }

         public static class Gas {
            public const string TYPE = PREFIX + "_gas" ;

            public static class MQ135 {
               public const string FULL_TYPE = TYPE + "_mq135" ;

               public class Measurement {
                  public const string A0 = GeneralMeasuredData.A0 ;
                  public const string D1 = GeneralMeasuredData.D1 ;

                  public const int DIGITAL_VALUE_ON = 1 ;
                  public const int FAILED_READING_VALUE = -1000 ;
               }

               public static class Instance {
                  public const string NAME = Properties.NAME ;
                  public const string ID = Properties.ID ;

                  [Obsolete ("Use HARDWARE_TYPE instead.")]
                  public const string TYPE = Properties.TYPE ;

                  public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
                  public const string EXPECTED_STATE = Properties.EXPECTED_STATE ;
               }
            }

            public static class TemperatureCelsius {
               public const string MAX_THRESHOLD = Properties.MAX_THRESHOLD ;

               public static class MG811 {
                  public const string MIN_THRESHOLD = Properties.MIN_THRESHOLD ;
                  public const string FULL_TYPE = TYPE + "_mg811" ;
                  public const string MAX_WARNING_THRESHOLD = Properties.MAX_WARNING_THRESHOLD ;
                  public const string MIN_WARNING_THRESHOLD = Properties.MIN_WARNING_THRESHOLD ;
               }

               public class Measurement {
                  public const string CO2 = Properties.CO2_PPM ; // float value
                  public const int FAILED_READING_VALUE = -1000 ;
               }

               public static class Instance {
                  public const string NAME = Properties.NAME ;
                  public const string ID = Properties.ID ;

                  [Obsolete ("Use HARDWARE_TYPE instead.")]
                  public const string TYPE = Properties.TYPE ;

                  public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
                  public const string CO2_PPM = Properties.CO2_PPM ;

                  public static class RelativeHumidityPercent {
                     public const string MAX_THRESHOLD = Properties.MAX_THRESHOLD ;
                     public const string MIN_THRESHOLD = Properties.MIN_THRESHOLD ;

                     public const string MAX_WARNING_THRESHOLD = Properties.MAX_WARNING_THRESHOLD ;
                     public const string MIN_WARNING_THRESHOLD = Properties.MIN_WARNING_THRESHOLD ;
                  }

                  public static class Co2PPM {
                     public const string THRESHOLD = Properties.THRESHOLD ;
                     public const string THRESHOLD_TYPE = Properties.THRESHOLD_TYPE ;
                  }
               }
            }
         }

         public static class CurrentLevel {
            public const string BASE_TYPE = PREFIX + "_current_level" ;

            public class MeasuredDataProperties {
               public const string CURRENT_A = "current_a" ;
               public const string CURRENT_MIN_A = "current_min_a" ;
               public const string CURRENT_MAX_A = "current_max_a" ;
            }

            public static class InstanceProperties {
               public const string NAME = Properties.NAME ;
               public const string ID = Properties.ID ;

               [Obsolete ("Use HARDWARE_TYPE instead.")]
               public const string TYPE = Properties.TYPE ;

               public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
               public const string MAX_THRESHOLD = Properties.MAX_THRESHOLD ;
               public const string MIN_THRESHOLD = Properties.MIN_THRESHOLD ;
               public const string CHECK_MIN_MAX = Properties.CHECK_MIN_MAX ;
            }

            public static class Max30A {
               public const string FULL_TYPE = BASE_TYPE + "_max_30a" ;
            }

            public static class Max50A {
               public const string FULL_TYPE = BASE_TYPE + "_max_50a" ;

            }

            public static class Max100A {
               public const string FULL_TYPE = BASE_TYPE + "_max_100a" ;
            }
         }

         public static class VoltageLevel {
            public const string BASE_TYPE = PREFIX + "_voltage_level" ;

            public static class Max230V {
               public const string TYPE_PART = BASE_TYPE + "_max_230v" ;

               public static class OneChannel {
                  public const string FULL_TYPE = TYPE_PART + "_one_channel" ;

                  public static class MeasuredDataProperties {
                     public const string D1 = GeneralMeasuredData.D1 ;
                     public const string A0 = GeneralMeasuredData.A0 ;
                     public const string A0_MIN = GeneralMeasuredData.A0_MIN ;
                     public const string A0_MAX = GeneralMeasuredData.A0_MAX ;
                  }

                  public static class InstanceProperties {
                     public const string NAME = Properties.NAME ;
                     public const string ID = Properties.ID ;

                     [Obsolete ("Use HARDWARE_TYPE instead.")]
                     public const string TYPE = Properties.TYPE ;

                     public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
                     public const string CHECK_MIN_MAX = Properties.CHECK_MIN_MAX ;
                     public const string MAX_THRESHOLD = Properties.MAX_THRESHOLD ;
                     public const string MIN_THRESHOLD = Properties.MIN_THRESHOLD ;
                  }
               }

               public static class ThreeChannel {
                  public const string FULL_TYPE = TYPE_PART + "_three_channel" ;

                  public static class MeasuredDataProperties {
                     public const string A0 = GeneralMeasuredData.A0 ;
                     public const string A0_MIN = GeneralMeasuredData.A0_MIN ;
                     public const string A0_MAX = GeneralMeasuredData.A0_MAX ;
                     public const string D1 = GeneralMeasuredData.D1 ;
                     public const string D1_MIN = GeneralMeasuredData.A0_MIN ;
                     public const string D1_MAX = GeneralMeasuredData.D1_MAX ;

                     public const string D2 = GeneralMeasuredData.D2 ;
                     public const string D2_MIN = GeneralMeasuredData.D2_MIN ;
                     public const string D2_MAX = GeneralMeasuredData.D2_MAX ;

                     public const string D3 = GeneralMeasuredData.D3 ;
                     public const string D3_MIN = GeneralMeasuredData.D3_MIN ;
                     public const string D3_MAX = GeneralMeasuredData.D3_MAX ;

                     public const int DIGITAL_VALUE_ON = 1 ;
                  }

                  public static class Instance {
                     public const string NAME = Properties.NAME ;
                     public const string ID = Properties.ID ;

                     [Obsolete ("Use HARDWARE_TYPE instead.")]
                     public const string TYPE = Properties.TYPE ;

                     public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;

                     public const string CHANNEL_1 = Properties.CHANNEL_1 ;
                     public const string CHANNEL_2 = Properties.CHANNEL_2 ;
                     public const string CHANNEL_3 = Properties.CHANNEL_3 ;

                     public static class Channel1 {
                        public const string NAME = Properties.NAME ;
                        public const string MAX_THRESHOLD = Properties.MAX_THRESHOLD ;
                        public const string MIN_THRESHOLD = Properties.MIN_THRESHOLD ;
                        public const string CHECK_MIN_MAX = Properties.CHECK_MIN_MAX ;
                     }

                     public static class Channel2 {
                        public const string NAME = Properties.NAME ;
                        public const string EXPECTED_STATE = Properties.EXPECTED_STATE ;
                     }

                     public static class Channel3 {
                        public const string NAME = Properties.NAME ;
                        public const string EXPECTED_STATE = Properties.EXPECTED_STATE ;
                     }
                  }
               }
            }
         }
      }
      #endregion

      #region Management Server routes
      public static class ManagementServerRoutes {

         public const string URL_BASE = "/hardwareSupervisor/" ;

         public static class RegisterMeasuredData {
            public const string FULL_URL = URL_BASE + "registerMeasuredData" ;

            public static class Request {
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;
               public const string MEASUREMENT = "measurement" ;
               public const string MEASUREMENT_TIMESTAMP = Properties.MEASUREMENT_TIMESTAMP ;
            }
         }
            [Obsolete]
            public static class RegisterMeasurement
            {
                public const string FULL_URL = URL_BASE + "registerMeasurement";

                public static class Request
                {
                    public const string HARDWARE_TOKEN = Properties.HARDWARE_TOKEN;
                    public const string INSTANCE_ID = Properties.INSTANCE_ID;
                    public const string MEASUREMENT = "measurement";
                    public const string MEASUREMENT_TIMESTAMP = Properties.MEASUREMENT_TIMESTAMP;
                }
            }
            public static class RegisterHardware {
            public const string FULL_URL = URL_BASE + "registerHardware" ;

            public static class Request {
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;

               [Obsolete ("Use HARDWARE_TYPE instead.")]
               public const string TYPE = Properties.TYPE ;

               public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
               public const string INSTANCE_NAME = Properties.INSTANCE_NAME ;
            }
         }

         public static class Register {
            public const string FULL_URL = URL_BASE + "register" ;

            public static class Request {
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;

               [Obsolete ("Use HARDWARE_TYPE instead.")]
               public const string TYPE = Properties.TYPE ;

               public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
               public const string INSTANCE_NAME = Properties.INSTANCE_NAME ;
            }
         }

         public static class CheckForUpdate {
            public const string FULL_URL = URL_BASE + "checkForUpdate" ;

            public static class Request {
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;

               [Obsolete ("Use HARDWARE_TYPE instead.")]
               public const string TYPE = Properties.TYPE ;

               public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
               public const string INSTANCE_NAME = Properties.INSTANCE_NAME ;
               public const string VERSION = AllParameters.VERSION ;
            }
         }

         public static class RemoteLog {
            public const string FULL_URL = URL_BASE + "remoteLog" ;

            public static class Request {
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;
               public const string CAPTION = Properties.CAPTION ;
               public const string MESSAGE = AllParameters.MESSAGE ;
               public const string FIRMWARE = Properties.FIRMWARE ;
            }
         }

         public static class ValidateHardware {
            public const string FULL_URL = URL_BASE + "validateHardware" ;

            public static class Request {
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;

               [Obsolete ("Use HARDWARE_TYPE instead.")]
               public const string TYPE = Properties.TYPE ;

               public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;
            }
         }
      }
      #endregion

      #region Watcher Server routes
      public static class WatcherServerRoutes {
         public const string URL_BASE = "hardwareSupervisor/" ;

         public static class Save {
            public static class Request {
               public const string TIMEOUT_IN_SECONDS = AllParameters.TIMEOUT_INTERVAL_SECONDS ;
               public const string CONFIGURATION = Communication.Strings.Strings.AllParameters.CONFIGURATION ;

               public static class Configuration {
                  public const string INSTANCES = Properties.INSTANCES ;

                  public static class Instance {
                     [Obsolete("Use INSTANCE_ID instead.")]
                     public const string ID = Properties.ID ;
                     public const string INSTANCE_ID = Properties.INSTANCE_ID ;

                     public const string NAME = Properties.NAME ;

                     [Obsolete ("Use HARDWARE_TYPE instead.")]
                     public const string TYPE = Properties.TYPE ;

                     public const string HARDWARE_TYPE = Properties.HARDWARE_TYPE ;

                     //there are specific properties which are depending from the type. You can find them above
                  }
               }
            }
         }

         public static class RegisterMeasurement {
            private const string URL_PART = "registerMeasurement" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;

               [Obsolete ("Use PACKAGE_IDS instead.")]
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;

               public const string PACKAGE_IDS = AllParameters.PACKAGE_IDS ;
               public const string MEASURED_DATA = AllParameters.MEASUREMENT ;
               public const string MEASUREMENT_TIMESTAMP = AllParameters.MEASUREMENT_TIMESTAMP ;
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;
               public const string INSTANCE_NAME = Properties.INSTANCE_NAME ;

               public static class CheckResult {
                  public const string TYPE_VALUE = "HardwareStatusCheck" ;

                  //public const string SUCCESS = AllParameters.SUCCESS ;
                  //public const string MESSAGE = AllParameters.MESSAGE ;
                  //public const string DETAILS = AllParameters.DETAILS ;
                  public const string DATA = "data" ;
               }
            }
         }
      }
      #endregion
   }
}