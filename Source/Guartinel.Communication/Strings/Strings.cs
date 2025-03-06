using System.Collections.Generic ;
using Newtonsoft.Json ;

namespace Guartinel.Communication.Strings {
   public class Strings : StringsBase {
      private Strings() {
         _languages.Add (new Languages.English()) ;
      }

      public override string Prefix => "COMMON" ;

      public static Strings Use {get ;} = new Strings() ;

      public interface IMessages {
         // string Alert {get ;}
         // string RecoveryAlert { get; }

         // string InstanceNotAvailableAlertMessage { get; }
         string OKStatusMessage {get ;}
         string OKStatusDetails { get; }
         string OKStatusExtract { get; }

         string UnknownStatusMessage {get ;}
         string UnknownStatusDetails { get; }         
         string UnknownStatusExtract { get; }

         string AlertStatusMessage { get; }
         string AlertStatusDetails { get; }
         string AlertStatusExtract { get; }


         string WarningStatusMessage { get; }
         string WarningStatusDetails { get; }
         string WarningStatusExtract { get; }

         string CriticalStatusMessage { get; }
         string CriticalStatusDetails { get; }
         string CriticalStatusExtract { get; }

         string ErrorInCheckMessage { get; }
         string ErrorInCheckExtract { get; }         
      }

      public class Messages : IMessages {
         public static Messages Use {get ;} = new Messages() ;

         // public string Alert => nameof (Alert) ;
         // public string RecoveryAlert => nameof(RecoveryAlert);

         // public string InstanceNotAvailableAlertMessage => nameof(InstanceNotAvailableAlertMessage);
         public string OKStatusMessage => nameof(OKStatusMessage) ;
         public string OKStatusDetails => nameof(OKStatusDetails) ;
         public string OKStatusExtract => nameof(OKStatusExtract) ;

         public string UnknownStatusMessage => nameof(UnknownStatusMessage) ;
         public string UnknownStatusDetails => nameof(UnknownStatusDetails) ;
         public string UnknownStatusExtract => nameof(UnknownStatusExtract) ;
         public string AlertStatusMessage => nameof(AlertStatusMessage) ;
         public string AlertStatusDetails => nameof(AlertStatusDetails) ;
         public string AlertStatusExtract => nameof(AlertStatusExtract) ;

         public string WarningStatusMessage => nameof(WarningStatusMessage) ;
         public string WarningStatusDetails => nameof(WarningStatusDetails) ;
         public string WarningStatusExtract => nameof(WarningStatusExtract) ;

         public string CriticalStatusMessage => nameof(CriticalStatusMessage) ;
         public string CriticalStatusDetails => nameof(CriticalStatusDetails) ;
         public string CriticalStatusExtract => nameof(CriticalStatusExtract) ;

         public string ErrorInCheckMessage => nameof(ErrorInCheckMessage) ;
         public string ErrorInCheckExtract => nameof(ErrorInCheckExtract) ;
      }

      public static class Parameters {
         // public static string InstanceName => nameof (InstanceName) ;
         public static string PackageName => nameof(PackageName) ;
         public static string InstanceName => nameof(InstanceName) ;
         public static string CheckName => nameof(CheckName) ;
      }

      public static class Lookups {
         public static string PackageNameFromID => nameof(PackageNameFromID) ;
      }

      public override Dictionary<string, string> GetProperties() {
         return Helper.ObjectToDictionary (new Properties()) ;
      }

      public class Properties {
         [JsonProperty]
         public const string TIMEOUT_IN_SECONDS = "timeout_in_seconds" ;
      }

      public class CommonSerializationSummary {
         [JsonProperty]
         public AllParameters ALL_PARAMETERS => new AllParameters() ;

         [JsonProperty]
         public AllErrorValues ALL_ERROR_VALUES => new AllErrorValues() ;

         [JsonProperty]
         public AllSuccessValues ALL_SUCCESS_VALUES => new AllSuccessValues() ;

         [JsonProperty]
         public AllDeviceTypeValues ALL_DEVICE_TYPE_VALUES => new AllDeviceTypeValues() ;
      }

      public class AllParameters {
         [JsonProperty]
         public const string ACCOUNT = "account" ;

         [JsonProperty]
         public const string ACCESS = "access" ;

         [JsonProperty]
         public const string ACTIVATION_CODE = "activation_code" ;

         [JsonProperty]
         public const string ADDRESS = "address" ;

         [JsonProperty]
         public const string AGENT_DEVICE_NAME = "agent_device_name" ;

         [JsonProperty]
         public const string AGENT_ID = "agent_id" ;

         [JsonProperty]
         public const string AGENT_DEVICE_ID = "agent_device_id" ;

         [JsonProperty]
         public const string AGENT_DEVICES = "agent_devices" ;

         [JsonProperty]
         public const string ALERT_DEVICES = "alert_devices" ;

         [JsonProperty]
         public const string AGENT_NAME = "agent_name" ;

         [JsonProperty]
         public const string ALERT_DEVICE_ID = "alert_device_id" ;

         [JsonProperty]
         public const string ALERT_DEVICE_IDS = "alert_device_ids" ;

         [JsonProperty]
         public const string ALERT_ID = "alert_id" ;

         [JsonProperty]
         public const string ALERT_TEXT = "alert_text" ;

         [JsonProperty]
         public const string ALERT_EMAIL = "alert_email" ;

         [JsonProperty]
         public const string ALERT_EMAILS = "alert_emails" ;

         [JsonProperty]
         public const string ALERT_MESSAGE = "alert_message" ;

         [JsonProperty]
         public const string ALERT_DETAILS = "alert_details" ;

         [JsonProperty]
         public const string ANDROID_DEVICE = "android_device" ;

         [JsonProperty]
         public const string AVARAGE_LATENCY = "average_latency" ;

         [JsonProperty]
         public const string BLACK_LIST_TOKEN = "black_list_token" ;

         [JsonProperty]
         public const string CATEGORIES = "categories" ;

         [JsonProperty]
         public const string CATEGORY = "category" ;

         [JsonProperty]
         public const string RUNNER_COUNT = "runner_count" ;

         [JsonProperty]
         public const string FAILED = "failed" ;

         [JsonProperty]
         public const string SENT = "sent" ;

         [JsonProperty]
         public const string CHECK_INTERVAL_SECONDS = "check_interval_seconds" ;

         [JsonProperty]
         public const string STARTUP_DELAY_SECONDS = "startup_delay" ;

         [JsonProperty]
         public const string CHECK_RESULT = "check_result" ;

         [JsonProperty]
         public const string CHECKSUM = "checksum";
         

         [JsonProperty]
         public const string CONFIGURATION = "configuration" ;

         [JsonProperty]
         public const string PACKAGE_PART_STATES = "states" ;

         [JsonProperty]
         public const string PACKAGE_PART_IDENTIFIER = "package_part_identifier" ;

         [JsonProperty]
         public const string PACKAGE_PART_STATE_NAME = "package_part_state" ;

         [JsonProperty]
         public const string PACKAGE_PART_STATE_MESSAGE = "package_part_message" ;

         [JsonProperty]
         public const string PACKAGE_PART_STATE_DETAILS = "package_part_details" ;

         [JsonProperty]
         public const string PACKAGE_PART_STATE_EXTRACT = "package_part_extract" ;

         [JsonProperty]
         public const string CONFIGURED = "configured" ;

         [JsonProperty]
         public const string CREATED_ON = "created_on" ;

         [JsonProperty]
         public const string DATABASE_URL = "database_url" ;

         [JsonProperty]
         public const string DB_VERSION = "db_version" ;

         [JsonProperty]
         public const string DESCRIPTION = "description" ;

         [JsonProperty]
         public const string DEVICE_CATEGORY = "device_category" ;

         [JsonProperty]
         public const string DEVICE_COUNT = "device_count" ;

         [JsonProperty]
         public const string DEVICE_NAME = "device_name" ;

         [JsonProperty]
         public const string DEVICE_TYPE = "device_type" ;

         [JsonProperty]
         public const string DEVICE_UUID = "device_uuid" ;

         [JsonProperty]
         public const string DEVICE_ID = "device_id" ;

         [JsonProperty]
         public const string DEVICES = "devices" ;

         [JsonProperty]
         public const string DISABLE_ALERTS = "disable_alerts" ;

         [JsonProperty]
         public const string EMAIL = "email" ;

         [JsonProperty]
         public const string FORCED_DEVICE_ALERT = "forced_device_alert" ;

         [JsonProperty]
         public const string FROM_EMAIL = "from_email" ;

         [JsonProperty]
         public const string FIRMWARE = "firmware" ;

         [JsonProperty]
         public const string EMAIL_PROVIDER = "email_provider" ;

         [JsonProperty]
         public const string EMAIL_USER_NAME = "email_user_name" ;

         [JsonProperty]
         public const string EMAIL_PASSWORD = "email_password" ;

         [JsonProperty]
         public const string ERROR = "error" ;

         [JsonProperty]
         public const string ERROR_DETAILS = "error_details" ;

         [JsonProperty]
         public const string ERROR_PARAMETERS = "error_parameters" ;

         [JsonProperty]
         public const string ERROR_UUID = "error_uuid" ;

         [JsonProperty]
         public const string EVENTS = "events" ;

         [JsonProperty]
         public const string EXPIRY_DATE = "expiry_date" ;

         [JsonProperty]
         public const string FIRST_NAME = "first_name" ;

         [JsonProperty]
         public const string GCM_ID = "gcm_id" ;

         [JsonProperty]
         public const string ID = "id" ;

         [JsonProperty]
         public const string IS_ACTIVATED = "is_activated" ;

         [JsonProperty]
         public const string IS_CONNECTED = "is_connected" ;

         [JsonProperty]
         public const string IS_DISCONNECTED = "is_disconnected" ;

         [JsonProperty]
         public const string IS_ENABLED = "is_enabled" ;

         [JsonProperty]
         public const string IS_RECOVERY = "is_recovery" ;

         [JsonProperty]
         public const string INVALID_REQUESTS = "invalid_requests" ;

         [JsonProperty]
         public const string JOB_NAME = "job_name" ;

         [JsonProperty]
         public const string LAST_NAME = "last_name" ;

         [JsonProperty]
         public const string LAST_MEASUREMENT = "last_measurement" ;

         [JsonProperty]
         public const string LAST_MEASUREMENT_TIMESTAMP = "last_measurement_timestamp" ;

         [JsonProperty]
         public const string LAST_MODIFICATION_TIMESTAMP = "last_modification_timestamp" ;

         [JsonProperty]
         public const string LICENSE = "license" ;

         [JsonProperty]
         public const string LICENSE_AGGREGATE = "license_aggregate" ;

         [JsonProperty]
         public const string LICENSE_ID = "license_id" ;

         [JsonProperty]
         public const string LICENSES = "licenses" ;

         [JsonProperty]
         public const string LICENSE_ORDER = "license_order" ;

         [JsonProperty]
         public const string LOG = "log" ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER = "management_server" ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_ADDRESS = "management_server_address" ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_ID = "management_server_id" ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_PORT = "management_server_port" ;

         [JsonProperty]
         public const string MEASUREMENT = "measurement" ;

         [JsonProperty]
         public const string MEASUREMENT_TYPE = "measurement_type" ;

         [JsonProperty]
         public const string MEASUREMENT_TIMESTAMP = "measurement_timestamp" ;

         [JsonProperty]
         public const string MESSAGE = "message" ;

         [JsonProperty]
         public const string DETAILS = "details" ;

         [JsonProperty]
         public const string NAME = "name" ;

         [JsonProperty]
         public const string NEW_EMAIL = "new_email" ;

         [JsonProperty]
         public const string NEW_PASSWORD = "new_password" ;

         [JsonProperty]
         public const string NEW_USER_NAME = "new_user_name" ;

         [JsonProperty]
         public const string PACKAGE_COUNT = "package_count" ;

         [JsonProperty]
         public const string PAYER_ID = "payer_id" ;

         [JsonProperty]
         public const string PAYPAL_TOKEN = "paypal_token" ;

         [JsonProperty]
         public const string PACKAGE_IDS = "package_ids" ;

         [JsonProperty]
         public const string PACKAGE_ID = "package_id" ;

         [JsonProperty]
         public const string INSTANCE_ID = "instance_id" ;

         [JsonProperty]
         public const string PACKAGES = "packages" ;

         [JsonProperty]
         public const string PACKAGE = "package" ;

         [JsonProperty]
         public const string PACKAGE_NAME = "package_name" ;

         [JsonProperty]
         public const string PROPERTIES = "properties" ;

         [JsonProperty]
         public const string ENABLED = "enabled" ;

         [JsonProperty]
         public const string PACKAGE_TYPE = "package_type" ;

         [JsonProperty]
         public const string PASSWORD = "password" ;

         [JsonProperty]
         public const string PLACEHOLDERS = "placeholders" ;

         [JsonProperty]
         public const string PAYMENT = "payment" ;

         [JsonProperty]
         public const string PORT = "port" ;

         [JsonProperty]
         public const string SERVERS = "servers" ;

         //[JsonProperty]
         //public const string STATE = "state" ;

         [JsonProperty]
         public const string STATUS = "status" ;

         [JsonProperty]
         public const string START_DATE = "start_date" ;

         [JsonProperty]
         public const string CPU = "cpu" ;

         [JsonProperty]
         public const string MEMORY = "memory" ;

         [JsonProperty]
         public const string STORAGE = "storage" ;

         [JsonProperty]
         public const string STORAGE_SIZE = "storage_size" ;

         [JsonProperty]
         public const string STRESS_LEVEL = "stress_level" ;

         [JsonProperty]
         public const string SUCCESS = "success" ;

         [JsonProperty]
         public const string RESULT = "result" ;

         [JsonProperty]
         public const string PACKAGE_STATE = "package_state" ;

         [JsonProperty]
         public const string PACKAGE_STATE_OK = "ok" ;

         [JsonProperty]
         public const string PACKAGE_STATE_ALERTING = "alerting" ;

         [JsonProperty]
         public const string PACKAGE_STATE_UNKNOWN = "unknown" ;

         [JsonProperty]
         public const string SCHEDULES = "schedules" ;

         [JsonProperty]
         public const string PACKAGE_SCHEDULE_ONCE = "once" ;

         [JsonProperty]
         public const string PACKAGE_SCHEDULE_WEEKLY = "weekly" ;

         [JsonProperty]
         public const string PACKAGE_SCHEDULE_DAILY = "daily" ;

         [JsonProperty]
         public const string STATE = "state" ;

         [JsonProperty]
         public const string STATISTICS = "statistics" ;

         [JsonProperty]
         public const string STATE_NAME = "name" ;

         [JsonProperty]
         public const string STARTUP_TIME = "startup_time" ;

         [JsonProperty]
         public const string STATE_MESSAGE = "message" ;
         
         [JsonProperty]
         public const string STATE_DETAILS = "details";

         [JsonProperty]
         public const string REDIRECT_URL = "redirect_url" ;

         [JsonProperty]
         public const string REMOTE_DEBUG_URL = "remote_debug_url" ;

         // [JsonProperty]
         // public const string STATE_MESSAGE_DETAILS = "message_details" ;

         [JsonProperty]
         public const string TIME = "time" ;

         [JsonProperty]
         public const string TIMEOUT_INTERVAL_SECONDS = "timeout_interval_seconds" ;

         [JsonProperty]
         public const string ONE_TIME_REGISTRATION_TOKEN = "one_time_registration_token" ;

         [JsonProperty]
         public const string OVER_WRITE = "over_write" ;

         [JsonProperty]
         public const string OWNER = "owner" ;

         [JsonProperty]
         public const string USER_WEB_SERVER_ADDRESS = "user_web_server_address" ;

         [JsonProperty]
         public const string USER_NAME = "user_name" ;

         [JsonProperty]
         public const string USE_PLAIN_ALERT_EMAIL = "use_plain_alert_email" ;

         [JsonProperty]
         public const string UPDATE_SERVER_HOST = "update_server_host" ;

         [JsonProperty]
         public const string UPDATE_SERVER_PROTOCOL_PREFIX = "update_server_protocol_prefix" ;

         [JsonProperty]
         public const string UPDATE_SERVER_PORT = "update_server_port" ;

         [JsonProperty]
         public const string URL = "url" ;

         [JsonProperty]
         public const string TOKEN = "token" ;

         [JsonProperty]
         public const string TIMESTAMPS = "timestamps" ;

         [JsonProperty]
         public const string TIMESTAMP = "timestamp" ;

         [JsonProperty]
         public const string UID = "uid" ;

         [JsonProperty]
         public const string VALUE = "value" ;

         [JsonProperty]
         public const string VERIFICATION_CODE = "verification_code" ;

         [JsonProperty]
         public const string WATCHER_SERVER_ID = "watcher_server_id" ;

         [JsonProperty]
         public const string WEB_PAGE_URL = "web_page_url" ;

         [JsonProperty]
         public const string WEBSITE_ADDRESS = "website_address" ; // DTAP: it is used from the admin website.

         [JsonProperty]
         public const string DETAILED_WEBSITES = "detailed_websites" ;

         [JsonProperty]
         public const string DETAILED_WEBSITE_ADDRESS = "address" ;

         [JsonProperty]
         public const string DETAILED_WEBSITE_CAPTION = "caption" ;

         [JsonProperty]
         public const string WEBSITES = "websites" ;

         [JsonProperty]
         public const string VERSION = "version" ;

         [JsonProperty]
         public const string VERSION_CODE = "version_code";
      }

      public class AllErrorValues {
         [JsonProperty]
         public const string GENERAL_ERROR = "ERROR" ;

         [JsonProperty]
         public const string ACCOUNT_EXPIRED = "ACCOUNT_EXPIRED" ;

         [JsonProperty]
         public const string NO_BODY_IN_REQUEST = "NO_BODY_IN_REQUEST" ;

         [JsonProperty]
         public const string INVALID_ROUTE = "INVALID_ROUTE" ;

         [JsonProperty]
         public const string INVALID_ID = "INVALID_ID" ;

         [JsonProperty]
         public const string CANNOT_PARSE_REQUEST = "CANNOT_PARSE_REQUEST" ;

         [JsonProperty]
         public const string CANNOT_SEND_EMAIL = "CANNOT_SEND_EMAIL" ;

         [JsonProperty]
         public const string CANNOT_TEST_DEVICE = "CANNOT_TEST_DEVICE" ;

         [JsonProperty]
         public const string CANNOT_CONNECT_TO_REMOTE_HOST = "CANNOT_CONNECT_TO_REMOTE_HOST" ;

         [JsonProperty]
         public const string CONFIG_EXPIRED = "CONFIG_EXPIRED" ;

         [JsonProperty]
         public const string CONFIG_VALID = "CONFIG_VALID" ;

         [JsonProperty]
         public const string DISCONNECTED = "DISCONNECTED" ;

         [JsonProperty]
         public const string DEVICE_NAME_TAKEN = "DEVICE_NAME_TAKEN" ;

         [JsonProperty]
         public const string DEVICE_NOT_REGISTERED = "DEVICE_NOT_REGISTERED";

         [JsonProperty]
         public const string DEVICE_ALREADY_REGISTERED = "DEVICE_ALREADY_REGISTERED";
         

         [JsonProperty]
         public const string EMAIL_ALREADY_REGISTERED = "EMAIL_ALREADY_REGISTERED" ;

         [JsonProperty]
         public const string INVALID_DATABASE_URL = "INVALID_DATABASE_URL" ;

         [JsonProperty]
         public const string INVALID_VERIFICATION_CODE = "INVALID_VERIFICATION_CODE" ;

         [JsonProperty]
         public const string NO_DATABASE_CONNECTED = "NO_DATABASE_CONNECTED" ;

         [JsonProperty]
         public const string INTERNAL_SYSTEM_ERROR = "INTERNAL_SYSTEM_ERROR" ;

         [JsonProperty]
         public const string INVALID_PACKAGE_OBJECT = "INVALID_PACKAGE_OBJECT" ;

         [JsonProperty]
         public const string INVALID_PACKAGE_NAME = "INVALID_PACKAGE_NAME" ;

         [JsonProperty]
         public const string INVALID_ACCOUNT_ID = "INVALID_ACCOUNT_ID" ;

         [JsonProperty]
         public const string INVALID_ACTIVATION_CODE = "INVALID_ACTIVATION_CODE" ;

         [JsonProperty]
         public const string INVALID_ADMINISTRATION_TOKEN = "INVALID_ADMINISTRATION_TOKEN" ;

         [JsonProperty]
         public const string INVALID_DEVICE_UUID = "INVALID_DEVICE_UUID" ;

         [JsonProperty]
         public const string INVALID_REQUEST_PARAMETERS = "INVALID_REQUEST_PARAMETERS" ;

         [JsonProperty]
         public const string INVALID_TOKEN = "INVALID_TOKEN" ;

         [JsonProperty]
         public const string LICENSE_ERROR = "LICENSE_ERROR" ;

         [JsonProperty]
         public const string TOKEN_EXPIRED = "TOKEN_EXPIRED" ;

         [JsonProperty]
         public const string INVALID_SERVER_LOGIN = "INVALID_SERVER_LOGIN" ;

         [JsonProperty]
         public const string INVALID_USER_NAME_OR_PASSWORD = "INVALID_USER_NAME_OR_PASSWORD" ;

         [JsonProperty]
         public const string MAXIMUM_DEVICE_COUNT_REACHED = "MAXIMUM_DEVICE_COUNT_REACHED" ;

         [JsonProperty]
         public const string MAXIMUM_PACKAGE_PART_COUNT_REACHED = "MAXIMUM_PACKAGE_PART_COUNT_REACHED" ;

         [JsonProperty]
         public const string MISSING_API_ACCESS_RIGHT = "MISSING_API_ACCESS_RIGHT" ;

         [JsonProperty]
         public const string MISSING_USER_WEB_SITE = "MISSING_USER_WEB_SITE" ;

         [JsonProperty]
         public const string MISSING_MANAGEMENT_SERVER = "MISSING_MANAGEMENT_SERVER" ;

         [JsonProperty]
         public const string NOT_USED_IN_PACKAGE = "NOT_USED_IN_PACKAGE" ;

         [JsonProperty]
         public const string UPDATE_NOW = "UPDATE_NOW";

         [JsonProperty]
         public const string PERMISSION_DENIED = "PERMISSION_DENIED" ;

         [JsonProperty]
         public const string TIMEOUT_IS_NOT_ELAPSED_SINCE_LAST_QUERY = "TIMEOUT_IS_NOT_ELAPSED_SINCE_LAST_QUERY" ;

         [JsonProperty]
         public const string ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND = "ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND" ;
      }

      public class AllSuccessValues {
         [JsonProperty]
         public const string SUCCESS = "SUCCESS" ;

         [JsonProperty]
         public const string ERROR = "ERROR" ;

         [JsonProperty]
         public const string FAIL = "ERROR" ;
         // @todo SzTZ: Should be FAIL!

         [JsonProperty]
         public const string WARNING = "WARNING" ;

         [JsonProperty]
         public const string CRITICAL = "CRITICAL" ;
      }

      public class AllDeviceTypeValues {
         [JsonProperty]
         public const string LINUX_AGENT_DEVICE = "linux_agent_device" ;

         [JsonProperty]
         public const string WINDOWS_AGENT_DEVICE = "windows_agent_device" ;

         [JsonProperty]
         public const string ANDROID_DEVICE = "android_device" ;

         [JsonProperty]
         public const string HARDWARE_SENSOR = "hardware_sensor" ;
      }

      public const string NULL = "NULL" ;
      public const string HEARTBEAT_INSTANCE_ID = "DAFB139B-D597-49CD-BAE2-6530D84FD534" ;
      public const string HEARTBEAT_INSTANCE_NAME = "Heartbeat" ;
      public const string IS_HEARTBEAT_PROPERTY_NAME = "is_heartbeat" ;
   }
}
