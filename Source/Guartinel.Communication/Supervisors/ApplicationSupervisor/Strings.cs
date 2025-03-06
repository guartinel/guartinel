using System ;
using System.Collections.Generic ;
using Guartinel.Communication.Strings ;
using Guartinel.Communication.Supervisors.ApplicationSupervisor.Languages ;
using Newtonsoft.Json ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;

namespace Guartinel.Communication.Supervisors.ApplicationSupervisor {
   public class Strings : StringsBase {
      #region General

      public static Strings Use {get ;} = new Strings() ;

      public override string Prefix => PackageType ;
      public string PackageType => "APPLICATION_SUPERVISOR" ;

      private Strings() {
         _languages.Add (new English()) ;
      }
      #endregion

      #region Messages

      public interface IMessages {
         string InstanceNotAvailableAlertMessage {get ;}
         string InstanceNotAvailableAlertDetails {get ;}
         string InstanceNotAvailableAlertExtract { get; }
         string ApplicationMeasurementAlertMessage {get ;}
         string ApplicationMeasurementAlertExtract { get; }
         string ApplicationMeasurementWarningMessage { get; }
         string ApplicationMeasurementWarningExtract { get; }
         string ApplicationMeasurementCriticalMessage { get; }
         string ApplicationMeasurementCriticalExtract { get; }
         string ApplicationMeasurementAlertDetails {get ;}
         string ApplicationMeasurementOKMessage { get ;}
         string ApplicationMeasurementOKDetails {get ;}
         string ApplicationMeasurementOKExtract { get; }
         string PackageNotAvailableAlert {get ;}
         string PackageNotAvailableAlertDetails {get ;}
         string PackageNotAvailableAlertExtract { get; }         
         string PackageAvailableRecoveryMessage { get; }
         string PackageAvailableRecoveryDetails { get; }
         string PackageAvailableRecoveryExtract { get; }
      }

      public class Messages : IMessages {
         public static Messages Use {get ;} = new Messages() ;

         public string InstanceNotAvailableAlertMessage => nameof(InstanceNotAvailableAlertMessage) ;
         public string InstanceNotAvailableAlertDetails => nameof(InstanceNotAvailableAlertDetails) ;
         public string InstanceNotAvailableAlertExtract => nameof(InstanceNotAvailableAlertExtract) ;
         public string ApplicationMeasurementAlertMessage => nameof(ApplicationMeasurementAlertMessage) ;
         public string ApplicationMeasurementAlertExtract => nameof(ApplicationMeasurementAlertExtract);
         public string ApplicationMeasurementWarningMessage => nameof(ApplicationMeasurementWarningMessage) ;
         public string ApplicationMeasurementWarningExtract => nameof(ApplicationMeasurementWarningExtract);
         public string ApplicationMeasurementCriticalMessage => nameof(ApplicationMeasurementCriticalMessage) ;
         public string ApplicationMeasurementCriticalExtract => nameof(ApplicationMeasurementCriticalExtract) ;
         public string ApplicationMeasurementAlertDetails => nameof(ApplicationMeasurementAlertDetails) ;
         public string ApplicationMeasurementOKMessage => nameof(ApplicationMeasurementOKMessage) ;
         public string ApplicationMeasurementOKDetails => nameof(ApplicationMeasurementOKDetails) ;
         public string ApplicationMeasurementOKExtract => nameof(ApplicationMeasurementOKExtract);
         public string PackageNotAvailableAlert => nameof(PackageNotAvailableAlert) ;
         public string PackageNotAvailableAlertDetails => nameof(PackageNotAvailableAlertDetails) ;
         public string PackageNotAvailableAlertExtract => nameof(PackageNotAvailableAlertExtract);
         public string PackageAvailableRecoveryMessage => nameof(PackageAvailableRecoveryMessage) ;
         public string PackageAvailableRecoveryDetails => nameof(PackageAvailableRecoveryDetails) ;
         public string PackageAvailableRecoveryExtract => nameof(PackageAvailableRecoveryExtract);
      }
      #endregion

      #region Parameters, properties

      public static class Parameters {
         public static string InstanceName => nameof(InstanceName) ;
         public static string PackageName => nameof(PackageName) ;
         public static string Message => nameof(Message) ;
         public static string Details => nameof(Details) ;
      }

      public static class Lookups {
         public static string InstanceNameFromID = nameof(InstanceNameFromID) ;
      }

      public override Dictionary<string, string> GetProperties() {
         return Helper.ObjectToDictionary (new Properties()) ;
      }

      public class Properties {
         [JsonProperty]
         public const string INSTANCE_NAME = "instance_name" ;

         [JsonProperty]
         public const string INSTANCES = "instances" ;

         [JsonProperty]
         public const string INSTANCE_ID = "instance_id" ;

         [JsonProperty]
         public const string INSTANCE_IDS = "instance_ids" ;

         [JsonProperty]
         public const string APPLICATION_TOKEN = "application_token" ;

         [JsonProperty]
         public const string MEASUREMENT = "measurement" ;

         [JsonProperty]
         public const string PACKAGE_ID = "package_id" ;

         [JsonProperty]
         public const string IS_HEARTBEAT = "is_heartbeat" ;

         [JsonProperty]
         public const string MEASUREMENT_TIMESTAMP = AllParameters.MEASUREMENT_TIMESTAMP ;
      }
      #endregion

      #region Management Server routes
      public static class ManagementServerRoutes {
         public const string URL_BASE = "/applicationSupervisor/" ;

         public static class RegisterMeasurement {
            public const string FULL_URL = URL_BASE + "registerMeasurement" ;

            public static class Request {
               public const string TYPE_VALUE = "ApplicationStatusCheck" ;
               public const string STARTUP_TIME = AllParameters.STARTUP_TIME ;
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;
               public const string MEASUREMENT = Properties.MEASUREMENT ;
               public const string PACKAGE_ID = Properties.PACKAGE_ID ;
               public const string IS_HEARTBEAT = Properties.IS_HEARTBEAT ;
               public const string MEASUREMENT_TIMESTAMP = Properties.MEASUREMENT_TIMESTAMP ;

               public static class Measurement {
                  public const string SUCCESS = AllParameters.SUCCESS ;
                  public const string MESSAGE = AllParameters.MESSAGE ;
               }
            }
         }

         public static class GetApplicationInstanceIDs {
            public const string FULL_URL = URL_BASE + "getApplicatonInstanceIDs" ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
            }

            public static class Response {
               public const string INSTANCE_IDS = Properties.INSTANCE_IDS ;
            }
         }
      }
      #endregion

      #region Watcher Server routes
      public static class WatcherServerRoutes {
         public const string URL_BASE = "applicationSupervisor/" ;

         public static class Save {
            public static class Request {
            }
         }

         public static class RegisterResult {
            private const string URL_PART = "registerResult" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string CHECK_RESULT = AllParameters.CHECK_RESULT ;
               public const string MEASUREMENT_TIMESTAMP = AllParameters.MEASUREMENT_TIMESTAMP ;
               public const string INSTANCE_ID = Properties.INSTANCE_ID ;
               public const string INSTANCE_NAME = Properties.INSTANCE_NAME ;
               public const string IS_HEARTBEAT = Properties.IS_HEARTBEAT ;

               public static class CheckResult {
                  public const string TYPE_VALUE = "ApplicationStatusCheck" ;
                  [Obsolete]
                  public const string SUCCESS = AllParameters.SUCCESS ;
                  
                  public const string RESULT = AllParameters.RESULT ;
                  public const string MESSAGE = AllParameters.MESSAGE ;
                  public const string DETAILS = AllParameters.DETAILS ;
                  public const string DATA = "data" ;

                  public static class CheckResultValue {
                     public const string SUCCESS = AllSuccessValues.SUCCESS ;
                     public const string FAIL = AllSuccessValues.FAIL ;
                     public const string WARNING = AllSuccessValues.WARNING ;
                     public const string CRITICAL = AllSuccessValues.CRITICAL ;
                  }
               }
            }
         }
      }
      #endregion
   }
}