using System ;
using System.Collections.Generic ;
using System.Globalization ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Instances ;
using Newtonsoft.Json.Linq ;
using ResponseNames = Guartinel.Communication.ManagementServerAPI.GeneralResponse.Names ;
using ResponseErrors = Guartinel.Communication.ManagementServerAPI.GeneralResponse.ErrorValues ;
using SuccessValues = Guartinel.Communication.ManagementServerAPI.GeneralResponse.SuccessValues ;
using ApplicationSupervisorStrings = Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.WatcherServer.Communication.ManagementServer {
   public class InternalManagementServerException : ServerException {
      public InternalManagementServerException() : base (AllErrorValues.INTERNAL_SYSTEM_ERROR, "Internal Management Server Error.") { }
   }

   public class ManagementServer : ManagementServerBase {
      public static class Constants {
         public const string HTTP_PREFIX = "http" ;

         public const int REQUEST_TIMEOUT_SECONDS = 10 ;
         public const int REQUEST_TIMEOUT_MILLISECONDS = REQUEST_TIMEOUT_SECONDS * 1000 ;

         //public static class Requests {
         //   public const string LOGIN = ManagementServerAPI.Admin.Login.FULL_URL ;
         //   public const string LOGOUT = ManagementServerAPI.Admin.Logout.FULL_URL ;
         //   public const string REQUEST_SYNCHRONIZATION = ManagementServerAPI.Admin.WatcherServer.RequestSynchronization.FULL_URL ;
         //   public const string SEND_DEVICE_ALERT = ManagementServerAPI.Alert.SendGcm.FULL_URL ;
         //   public const string SEND_EMAIL_ALERT = ManagementServerAPI.Alert.SendEmail.FULL_URL ;
         //}
      }

      protected Parameters SendPostToServer (string path,
                                             JObject values) {
         // Send the post, take and check the result
         var result = new Parameters (SendPostToServerInt (path, values)) ;

         // Translate error
         if (result.IsError()) {
            if (result [ResponseNames.ERROR] == ResponseErrors.INTERNAL_SYSTEM_ERROR) {
               throw new InternalManagementServerException() ;
            }

            if (result [ResponseNames.ERROR] == ResponseErrors.INVALID_TOKEN) {
               throw new InvalidTokenException() ;
            }

            if (result [ResponseNames.ERROR] == ResponseErrors.TOKEN_EXPIRED) {
               throw new ExpiredTokenException() ;
            }

            throw new Exception ($"Error sending post to Management Server. {result.AsJObject}") ;
         }

         return result ;
      }

      protected readonly PostSender _postSender = new PostSender() ;

      protected JObject SendPostToServerInt (string path,
                                             JObject values) {
         const int WAIT_TIME_SECONDS = 5 ;
         try {
            string managementServerAddress = ApplicationSettings.Use.ManagementServerAddress ;
            path = StringEx.EnsureEnds (path, @"/") ;

            if (!managementServerAddress.StartsWith (Constants.HTTP_PREFIX, true, CultureInfo.InvariantCulture)) {
               managementServerAddress = $"http://{managementServerAddress}" ;
            }
            var postAddress = $"{managementServerAddress}{path}" ;
            Logger.Log ($@"Post sent to MS. Address: '{postAddress}'. Values: {values.ConvertToLog()}") ;
            JObject result = _postSender.Post (postAddress, values, WAIT_TIME_SECONDS) ;
            Logger.Log ($@"Post returned from MS. Address: '{postAddress}'. Result: {result.ConvertToLog()}") ;

            return result ;
         } catch (Exception e) {
            var result = new JObject() ;

            var uuid = Guid.NewGuid().ToString() ;
            result [ResponseNames.SUCCESS] = SuccessValues.ERROR ;
            result [ResponseNames.ERROR] = ResponseErrors.INTERNAL_SYSTEM_ERROR ;
            result [ResponseNames.ERROR_UUID] = uuid ;

            Logger.Error ($@"Error returned from MS. UUID: {uuid}. Message: {e.GetAllMessages()}") ;

            return result ;
         }
      }

      // "79902F50-3542-4E69-8506-7250A639D933"
      // protected override string UID => General.GetProcessorID() ;
      protected override string UID => "79902F50-3542-4E69-8506-7250A639D933" ;

      protected override string RegistrationPassword {
         get => ApplicationSettings.Use.ManagementServerRegistrationToken ;
         set => ApplicationSettings.Use.ManagementServerRegistrationToken = value ;
      }

      protected override string DoRegister (string password,
                                            string uid) {
         JObject parameters = new JObject() ;
         parameters [ManagementServerAPI.WatcherServer.Register.Request.PASSWORD] = password ;
         parameters [ManagementServerAPI.WatcherServer.Register.Request.UID] = uid ;

         Parameters result = SendPostToServer (ManagementServerAPI.WatcherServer.Register.FULL_URL, parameters) ;

         WatcherServerID = result [ManagementServerAPI.WatcherServer.Register.Response.WATCHER_SERVER_ID] ;
         //DTAP WatcherServerID must be saved to use it later when trying to login in MS
         RegistrationPassword = password ;
         return result [ManagementServerAPI.WatcherServer.Login.Response.TOKEN] ;
      }

      /*protected override string LoginPassword { // DTAP LoginPassword is not neccesary to be stored bacause we can generate it.And I think its not used properly because this is the same LoginPassword thats used in LoginRoute 
         get {return Configuration.LoginPasswordHash ;}
         set {Configuration.LoginPasswordHash = value ;               
            }
      }*/

      protected override string ManagementServerLoginPassword => Hashing.GenerateHash (UID, ApplicationSettings.Use.ServerID) ;

      protected override string WatcherServerID {
         // DTAP we should store WatcherServerID to use it ManagementServerLoginPassword generation
         get => ApplicationSettings.Use.ServerID ;
         set => ApplicationSettings.Use.ServerID = value ;
      }

      protected override string DoLogin (string password) {
         JObject parameters = new JObject() ;
         // parameters [ManagementServerConstants.Parameters.USER_NAME] = userName ;
         parameters [ManagementServerAPI.WatcherServer.Login.Request.PASSWORD] = password ;
         parameters [ManagementServerAPI.WatcherServer.Login.Request.WATCHER_SERVER_ID] = WatcherServerID ;

         Parameters result = SendPostToServer (ManagementServerAPI.WatcherServer.Login.FULL_URL,
                                               parameters) ;
         //LoginPassword = password ;// DTAP we dont store the login password 
         return result [ManagementServerAPI.WatcherServer.Login.Response.TOKEN] ;
      }

      //public void Logout() {
      //   JObject parameters = new JObject() ;
      //   parameters [ManagementServerAPI.WatcherServer.Login] = _token ;

      //   // Parameters result = new Parameters (SendPostToServer (Configuration.ManagementServerAddress,
      //   SendPostToServer (Constants.Requests.LOGOUT,
      //                     parameters) ;
      //}

      protected override void DoSendAlertToDevice (string packageID,
                                                   string instanceID,
                                                   string alertID,
                                                   string alertDeviceID,
                                                   bool isRecovery,
                                                   bool forcedAlert,
                                                   string packageState,
                                                   XString message,
                                                   XString details) {
         JObject parameters = new JObject() ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.TOKEN] = Token ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.PACKAGE_ID] = packageID ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.INSTANCE_ID] = instanceID ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.ALERT_ID] = alertID ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.ALERT_DEVICE_ID] = alertDeviceID ;
         // parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.ALERT_MESSAGE] = JObject.Parse (message?.ToJsonString()) ;
         parameters[ManagementServerAPI.Alert.SendDeviceAlert.Request.ALERT_MESSAGE] = message == null ? new JObject() : message.AsJObject() ;
         // parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.ALERT_DETAILS] = JObject.Parse (details?.ToJsonString()) ;
         parameters[ManagementServerAPI.Alert.SendDeviceAlert.Request.ALERT_DETAILS] = details == null ? new JObject() : details.AsJObject() ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.IS_RECOVERY] = isRecovery ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.FORCED_DEVICE_ALERT] = forcedAlert ;
         parameters [ManagementServerAPI.Alert.SendDeviceAlert.Request.PACKAGE_STATE] = packageState ;

         // Parameters result = new Parameters (SendPostToServer (Configuration.ManagementServerAddress,
         SendPostToServer (ManagementServerAPI.Alert.SendDeviceAlert.FULL_URL,
                           parameters) ;
         // return result ;
      }

      protected override void DoSendAlertMail (string packageID,
                                               string instanceID,
                                               string alertID,
                                               string fromAddress,
                                               string toAddress,
                                               bool isRecovery,
                                               string packageState,
                                               XString message,
                                               XString details) {

         // @todo SzTZ: Track sent mails: do not send mails exceeding the limit per package

         JObject parameters = new JObject() ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.TOKEN] = Token ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.PACKAGE_ID] = packageID ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.INSTANCE_ID] = instanceID ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.FROM_EMAIL] = fromAddress ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.ALERT_EMAIL] = toAddress ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.ALERT_ID] = alertID ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.IS_RECOVERY] = isRecovery ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.PACKAGE_STATE] = packageState ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.ALERT_MESSAGE] = message == null ? new JObject() : message.AsJObject() ;
         parameters [ManagementServerAPI.Alert.SendEmail.Request.ALERT_DETAILS] = details == null ? new JObject() : details.AsJObject() ;

         SendPostToServer (ManagementServerAPI.Alert.SendEmail.FULL_URL,
                           parameters) ;

         // return result ;
      }

      protected override void DoStoreMeasurement (string packageID,
                                                  string measurementType,
                                                  DateTime measurementDateTime,
                                                  JObject measuredData) {
         // Use separate thread
         new ExecuteBehavior.InTask().Execute ("Store measurement", () => {
            JObject parameters = new JObject() ;
            parameters [ManagementServerAPI.Package.StoreMeasurement.Request.TOKEN] = Token ;
            parameters [ManagementServerAPI.Package.StoreMeasurement.Request.PACKAGE_ID] = packageID ;
            parameters [ManagementServerAPI.Package.StoreMeasurement.Request.MEASUREMENT_TYPE] = measurementType ;
            parameters [ManagementServerAPI.Package.StoreMeasurement.Request.MEASUREMENT] = measuredData ;
            parameters [ManagementServerAPI.Package.StoreMeasurement.Request.MEASUREMENT_TIMESTAMP] = Kernel.Utility.Converter.DateTimeToStringJson (measurementDateTime) ;

            // Parameters result = new Parameters (SendPostToServer (Configuration.ManagementServerAddress,
            SendPostToServer (ManagementServerAPI.Package.StoreMeasurement.FULL_URL, parameters) ;
         }, Logger.Error) ;
      }

      protected override void DoRequestSynchronization() {
         JObject parameters = new JObject() ;
         parameters [ManagementServerAPI.WatcherServer.RequestSynchronization.Request.TOKEN] = Token ;

         SendPostToServer (ManagementServerAPI.WatcherServer.RequestSynchronization.FULL_URL,
                           parameters) ;
      }

      protected override void DoStorePackageState (string packageID,
                                                   string state,
                                                   XString message,
                                                   XString details,
                                                   InstanceStateList instanceStates) {
         SendPostToServer (ManagementServerAPI.Package.StoreState.FULL_URL,
                           CreatePackageStateParameters (packageID, state, message, details, instanceStates)) ;
      }

      protected override IList<string> DoGetApplicationInstanceIDs (string packageID) {
         JObject parameters = new JObject() ;
         parameters [ApplicationSupervisorStrings.ManagementServerRoutes.GetApplicationInstanceIDs.Request.TOKEN] = Token ;
         parameters [ApplicationSupervisorStrings.ManagementServerRoutes.GetApplicationInstanceIDs.Request.PACKAGE_ID] = packageID ;

         Parameters result = SendPostToServer (ApplicationSupervisorStrings.ManagementServerRoutes.GetApplicationInstanceIDs.FULL_URL,
                                               parameters) ;

         JArray applicationInstanceIds ;
         try {
            applicationInstanceIds = JArray.Parse (result [ApplicationSupervisorStrings.ManagementServerRoutes.GetApplicationInstanceIDs.Response.INSTANCE_IDS]) ;
         } catch (Exception) {
            throw new Exception ($"Invalid agent device IDs returned. {result.AsJObject}") ;
         }

         return applicationInstanceIds.Select (x => x.ToString()).ToList() ;
      }
   }
}