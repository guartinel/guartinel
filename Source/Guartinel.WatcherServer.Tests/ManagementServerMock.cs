using System ;
using System.Collections.Concurrent ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Instances ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests {
   public class ManagementServerMock : ManagementServerBase {
      public class AlertMessage {
         public string PackageID ;
         public string InstanceID ;
         public string AlertID ;
         public string Message ;
         public string Details ;
         public bool IsRecovery ;
         public bool ForcedDeviceAlert ;
         public string PackageState ;
         public string Address ;
         public DateTime AlertTime ;

         public AlertMessage (string packageID,
                              string instanceID,
                              string alertID,
                              string message,
                              string details,
                              bool isRecovery,
                              bool forcedDeviceAlert,
                              string packageState,
                              string address) {
            PackageID = packageID ;
            InstanceID = instanceID ;
            AlertID = alertID ;
            Message = message ;
            Details = details;
            IsRecovery = isRecovery ;
            ForcedDeviceAlert = forcedDeviceAlert ;
            PackageState = packageState ;
            Address = address ;

            AlertTime = DateTime.UtcNow ;
         }
      }

      public class MeasuredData {
         public MeasuredData (string packageID,
                              DateTime measurementTime,
                              string data) {
            PackageID = packageID ;
            MeasurementTime = measurementTime ;
            Data = data ;
         }

         public string PackageID ;
         public DateTime MeasurementTime ;
         public string Data ;
      }

      public ManagementServerMock() {
      }

      protected override string UID => "0AE0278B-4A2B-4E00-9AF9-ED591562007A" ;

      public string AccessUID => UID ;

      protected override string RegistrationPassword {get ; set ;}
      // protected override string LoginPassword {get ; set ;}

      protected override string DoRegister (string password,
                                            string uid) {
         _loginCount++ ;

         // LoginPassword = Guid.NewGuid().ToString() ;
         RegistrationPassword = Guid.NewGuid().ToString() ;

         return Guid.NewGuid().ToString() ;
      }

      private volatile int _loginCount ;

      public int LoginCount => _loginCount ;

      protected override string ManagementServerLoginPassword => Kernel.Utility.Hashing.GenerateHash (UID, ApplicationSettings.Use.ServerID) ;

      protected override string DoLogin (string password) {
         _loginCount++ ;

         return Guid.NewGuid().ToString() ;
      }

      public List<AlertMessage> DeviceAlerts {get ;} = new List<AlertMessage>() ;

      protected override void DoSendAlertToDevice (string packageID,
                                                   string instanceID,
                                                   string alertID,
                                                   string alertDeviceID,
                                                   bool isRecovery,
                                                   bool forcedAlert,
                                                   string packageState,
                                                   XString message,
                                                   XString details) {
         CheckToken() ;

         Assert.IsFalse (string.IsNullOrEmpty (packageID)) ;
         Assert.IsFalse (string.IsNullOrEmpty (alertID)) ;
         Assert.IsFalse (string.IsNullOrEmpty (alertDeviceID)) ;

         lock (DeviceAlerts) {
            DeviceAlerts.Add (new AlertMessage (packageID, instanceID, alertID,
                               message?.ToJsonString(), details?.ToJsonString(),
                               isRecovery, forcedAlert, packageState, alertDeviceID)) ;
         }
      }

      protected override void DoRequestSynchronization() {
         CheckToken() ;

         // ExecuteAction (() => {
         //    CheckToken() ;
         // }, "Request Synchronization", String.Format ("Device: {0}, Message: {1}.", alertDeviceID, message)) ;
      }

      public class PackageStateMock {
         public PackageStateMock (string packageID,
                                  string state,
                                  XString message,
                                  XString details,
                                  InstanceStateList instanceStates) {
            PackageID = packageID ;
            State = state ;
            Message = message ;
            Details = details ;
            InstanceStates = instanceStates ;
         }

         public string PackageID ;
         public string State ;
         public XString Message ;
         public XString Details ;
         public readonly InstanceStateList InstanceStates ;
      }

      public readonly List<PackageStateMock> PackageStates = new List<PackageStateMock>() ;

      public ConcurrentDictionary<string, PackageStateForStore> PackageStatesForStore => _packageStatesForStore ;

      protected override void DoStorePackageState (string packageID,
                                                   string state,
                                                   XString message,
                                                   XString details,
                                                   InstanceStateList instanceStates) {
         CheckToken() ;

         // Store values
         PackageStates.Add (new PackageStateMock (packageID, state, message, details, instanceStates)) ;
      }

      protected readonly Dictionary<string, IList<string>> _agentIDs = new Dictionary<string, IList<string>>() ;

      public void SetAgentDeviceIDs (string packageID,
                                     IList<string> agentIDs) {
         _agentIDs.Add (packageID, agentIDs) ;
      }

      public bool GetAgentDeviceIDsThrowsError ;

      protected readonly Dictionary<string, IList<string>> _applicationInstanceIDs = new Dictionary<string, IList<string>>() ;

      public void SetApplicationInstanceIDs (string packageID,
                                             IList<string> applicationInstanceIDs) {
         _applicationInstanceIDs.Clear() ;
         _applicationInstanceIDs.Add (packageID, applicationInstanceIDs) ;
      }

      public void RegisterApplicationInstanceID (string packageID,
                                                 string applicationInstanceID) {
         if (!_applicationInstanceIDs.ContainsKey (packageID)) {
            _applicationInstanceIDs.Add (packageID, new List<string>()) ;
         }

         if (!_applicationInstanceIDs [packageID].Contains (applicationInstanceID)) {
            _applicationInstanceIDs[packageID].Add(applicationInstanceID);
         }         
      }

      public bool GetApplicationInstanceIDsThrowsError ;

      protected override IList<string> DoGetApplicationInstanceIDs (string packageID) {
         if (GetApplicationInstanceIDsThrowsError) {
            GetApplicationInstanceIDsThrowsError = false ;

            throw new Exception ("Error in Management Server Mock!") ;
         }

         CheckToken() ;

         if (string.IsNullOrEmpty (packageID)) return new List<string>() ;

         if (!_applicationInstanceIDs.ContainsKey (packageID)) return new List<string>() ;

         return _applicationInstanceIDs [packageID] ;
      }

      private readonly List<AlertMessage> _mailAlerts = new List<AlertMessage>() ;

      public List<AlertMessage> MailAlerts => _mailAlerts ;

      protected override string WatcherServerID {
         // DTAP we should store WatcherServerID to use it ManagementServerLoginPassword generation
         get => ApplicationSettings.Use.ServerID ;
         set => ApplicationSettings.Use.ServerID = value ;
      }

      protected int _packageStateStoreIntervalInSeconds = 5;

      public override int PackageStateStoreIntervalInSeconds => _packageStateStoreIntervalInSeconds ;

      public void SetPackageStateStoreIntervalInSeconds (int value) {
         _packageStateStoreIntervalInSeconds = value ;
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
         //ExecuteAction (() => {
         CheckToken() ;
         Assert.IsFalse (string.IsNullOrEmpty (packageID)) ;

         lock (_mailAlerts) {
            _mailAlerts.Add (new AlertMessage (packageID, instanceID, alertID,
                                               message?.ToJsonString(), details?.ToJsonString(),
                                               isRecovery, false, packageState, toAddress)) ;
         }

         //}, "Send Alert Email", String.Format ("From: {0}, To: {1}, Message: {2}.", fromAddress, toAddress, message)) ;
      }

      public readonly ConcurrentQueue<MeasuredData> MeasuredDataList = new ConcurrentQueue<MeasuredData>() ;

      protected override void DoStoreMeasurement (string packageID,
                                                  string measurementType,
                                                  DateTime measurementDateTime,
                                                  JObject measuredData) {
         CheckToken() ;

         // Check measurement data
         Assert.IsFalse (string.IsNullOrEmpty (packageID)) ;
         Assert.Greater (measurementDateTime, DateTime.UtcNow.AddYears (-1)) ;

         // Store measurement
         MeasuredDataList.Enqueue (new MeasuredData (packageID, measurementDateTime, measuredData.ToString())) ;
      }

      protected bool _tokenExpired = false ;

      public void ExpireToken() {
         _tokenExpired = true ;
      }

      protected void CheckToken() {
         if (_tokenExpired) {
            _tokenExpired = false ;

            throw new ExpiredTokenException() ;
         }
      }
   }
}
