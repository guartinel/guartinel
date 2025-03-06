using System ;
using System.Collections.Concurrent ;
using System.Collections.Generic ;
using System.Linq ;
using System.Threading ;
using System.Threading.Tasks ;
using Guartinel.Communication ;
using Guartinel.Kernel;
using Guartinel.Kernel.Utility ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Instances ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.WatcherServer.Communication.ManagementServer {
   public abstract class ManagementServerBase : IManagementServer {
      protected ManagementServerBase() {
         //private Task _packageStateStoreTask;
         //private volatile bool _packageStateStoreTaskCancelled ;      
         new Task (ScheduledStorePackageStates, _scheduledStorePackageStatesTokenSource.Token).Start() ;
      }

      ~ManagementServerBase() {
         _scheduledStorePackageStatesTokenSource.Cancel() ;
      }

      protected string Token {get ; set ;}

      protected abstract string UID {get ;}
      protected abstract string RegistrationPassword {get ; set ;}
      protected abstract string WatcherServerID {get ; set ;}
      protected abstract string ManagementServerLoginPassword {get ;}      

      protected object _statusCheckLock = new object() ;

      protected void CallIgnoreError (Action action) {
         try {
            action?.Invoke() ;
         } catch (System.Exception e) {
            Logger.Error ($"Error in call to Management Server. {e.GetAllMessages()}") ;
         }
      }

      protected void CallWithStatusCheck (Action action) {
         if (action == null) return ;

         CallIgnoreError (() => {
            lock (_statusCheckLock) {
               try {
                  if (string.IsNullOrEmpty (Token)) {
                     if (!string.IsNullOrEmpty (WatcherServerID)) {
                        //DTAP WatcherServerID is only not null after WS Registered to MS
                        Login (ManagementServerLoginPassword) ;
                     } else {
                        Register (RegistrationPassword, UID) ;
                     }
                  }
                  action() ;
               } catch (InvalidTokenException) {
                  // Login, and retry
                  Login (ManagementServerLoginPassword) ;

                  action() ;
               } catch (ExpiredTokenException) {
                  // Login, and retry
                  Login (ManagementServerLoginPassword) ;
               }
            }
         }) ;
      }

      protected abstract string DoRegister (string password,
                                            string uid) ;

      public void Register (string password,
                            string uid) {
         CallIgnoreError (() => {
            Token = DoRegister (password, uid) ;
         }) ;
      }

      protected abstract string DoLogin (string password) ;

      public void Login (string password) {
         CallIgnoreError (() => {
            Token = DoLogin (password) ;
         }) ;
      }

      protected abstract void DoSendAlertMail (string packageID,
                                               string instanceID,
                                               string alertID,
                                               string fromAddress,
                                               string toAddress,
                                               bool isRecovery,
                                               string packageState,
                                               XString message,
                                               XString details) ;

      public void SendAlertMail (string packageID,
                                 string instanceID,
                                 string alertID,
                                 string fromAddress,
                                 string toAddress,
                                 bool isRecovery,
                                 string packageState,
                                 XString message,
                                 XString details) {
         CallWithStatusCheck (() => DoSendAlertMail (packageID, instanceID, alertID, fromAddress, toAddress, isRecovery, packageState, message, details)) ;
      }

      protected abstract void DoSendAlertToDevice (string packageID,
                                                   string instanceID,
                                                   string alertID,
                                                   string alertDeviceID,
                                                   bool isRecovery,
                                                   bool forcedAlert,
                                                   string packageState,
                                                   XString message,
                                                   XString details) ;

      public void SendAlertToDevice (string packageID,
                                     string instanceID,
                                     string alertID,
                                     string alertDeviceID,
                                     bool isRecovery,
                                     bool forcedAlert,
                                     string packageState,
                                     XString message,
                                     XString details) {
         CallWithStatusCheck (() => DoSendAlertToDevice (packageID, instanceID, alertID, alertDeviceID, isRecovery, forcedAlert, packageState, message, details)) ;
      }

      public void StoreMeasuredData (string packageID,
                                    string measurementType,
                                    DateTime measurementDateTime,
                                    JObject measurement) {
         CallWithStatusCheck (() => DoStoreMeasurement (packageID, measurementType, measurementDateTime, measurement)) ;
      }

      protected abstract void DoStoreMeasurement (string packageID,
                                                  string measurementType,
                                                  DateTime measurementDateTime,
                                                  JObject measuredData) ;

      protected abstract void DoRequestSynchronization() ;

      public void RequestSynchronization() {
         CallWithStatusCheck (DoRequestSynchronization) ;
      }

      #region Store package state

      public virtual int PackageStateStoreIntervalInSeconds => 15 ;

      protected JObject CreatePackageStateParameters (string packageID,
                                                      string stateName,
                                                      XString message,
                                                      XString details,
                                                      InstanceStateList instanceStates) {
         JObject parameters = new JObject() ;
         parameters [ManagementServerAPI.Package.StoreState.Request.TOKEN] = Token ;
         parameters [ManagementServerAPI.Package.StoreState.Request.PACKAGE_ID] = packageID ;

         JObject state = new JObject() ;

         state [ManagementServerAPI.Package.StoreState.Request.State.NAME] = stateName ;
         state [ManagementServerAPI.Package.StoreState.Request.State.MESSAGE] = message?.AsJObject() ;
         state [ManagementServerAPI.Package.StoreState.Request.State.DETAILS] = details?.AsJObject();

         // Add detailed states
         if (instanceStates != null) {
            var instanceStatesArray = new JArray() ;
            foreach (KeyValuePair<string, InstanceState> instancePair in instanceStates) {               
               // Do not add extra heartbeat
               if (instancePair.Key == Guartinel.Communication.Strings.Strings.HEARTBEAT_INSTANCE_ID) continue ;
               if (instancePair.Value == null) continue ;

               var instanceState = new JObject() ;
               instanceState [ManagementServerAPI.Package.StoreState.Request.State.States.IDENTIFIER] = instancePair.Key ;
               instanceState [ManagementServerAPI.Package.StoreState.Request.State.States.STATE_NAME] = instancePair.Value.Name.ToLowerInvariant() ;
               // Add message and details as well
               // var combinedMessage = new XStrings (instancePair.Value.Message, instancePair.Value.Details) ;
               // instanceState [ManagementServerAPI.Package.StoreState.Request.State.States.STATE_MESSAGE] = combinedMessage.ToJObject() ;
               instanceState[ManagementServerAPI.Package.StoreState.Request.State.States.STATE_MESSAGE] = instancePair.Value.Message?.AsJObject();
               instanceState[ManagementServerAPI.Package.StoreState.Request.State.States.STATE_DETAILS] = instancePair.Value.Details?.AsJObject();
               instanceState[ManagementServerAPI.Package.StoreState.Request.State.States.STATE_EXTRACT] = instancePair.Value.Extract?.AsJObject();

               instanceStatesArray.Add (instanceState) ;
            }

            state [ManagementServerAPI.Package.StoreState.Request.State.STATES] = instanceStatesArray ;
         }

         parameters [ManagementServerAPI.Package.StoreState.Request.STATE] = state ;

         return parameters ;
      }

      protected abstract void DoStorePackageState (string packageID,
                                                   string state,
                                                   XString message,
                                                   XString details,
                                                   InstanceStateList instanceStates) ;

      protected int GetPriority (InstanceState instanceState) {
         if (instanceState == null) return 4 ;
         if (instanceState is InstanceState.OK) return 3 ;
         if (instanceState is InstanceState.Unknown) return 2 ;
         if (instanceState is InstanceState.Alerting) return 1 ;

         return 5 ;
      }

      public class PackageStateForStore {
         public string PackageID {get ; }
         public string State {get ; }
         public XString Message {get ; }
         public XString Details {get ; }
         public InstanceStateList InstanceStates {get ;}

         public PackageStateForStore (string packageID,
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
      }

      protected readonly ConcurrentDictionary<string, PackageStateForStore> _packageStatesForStore = new ConcurrentDictionary<string, PackageStateForStore>() ;

      public void StorePackageState (string packageID,
                                     string state,
                                     XString message,
                                     XString details,
                                     InstanceStateList instanceStates) {

         // Register package ID for state change store
         Logger.InfoWithDebug ($"Package state scheduled for package '{packageID}'. State: {state}.",
                               $"Message: {message.ToJsonString()}. Details: {details.ToJsonString()}") ;
         
         var newState = new PackageStateForStore (packageID, state, message, details, instanceStates) ;
         _packageStatesForStore.AddOrUpdate (packageID, newState, (key, oldState) => newState) ;
      }

      public void DeletePackageStates (string packageID) {
         // Unregister package ID for state change store
         if (_packageStatesForStore.ContainsKey (packageID)) {
            Logger.Log ($"Package state deleted for package {packageID}.") ;

            _packageStatesForStore.TryRemove (packageID, out _) ;
         }         
      }

      private readonly CancellationTokenSource _scheduledStorePackageStatesTokenSource = new CancellationTokenSource() ;

      private void ScheduledStorePackageStates() {
         while (true) {
            if (_scheduledStorePackageStatesTokenSource.IsCancellationRequested) return ;

            // Process package states waiting for store
            Dictionary<string, PackageStateForStore> packages = new Dictionary<string, PackageStateForStore>() ;

            while (!_packageStatesForStore.IsEmpty) {
               var packageID = _packageStatesForStore.Keys.First() ;
               if (_packageStatesForStore.TryRemove (packageID, out PackageStateForStore state)) {
                  if (!packages.ContainsKey (packageID)) {
                     Logger.InfoWithDebug ($"Add package {packageID} state '{state.State}'.",
                                           $"Message: {state.Message.ToJsonString()}. " +
                                           $"Details: {state.Details}. " + 
                                           $"Instance states: {state.InstanceStates.ToString()}") ;
                     packages.Add (packageID, state) ;
                  } else {
                     Logger.InfoWithDebug($"Overwrite package {packageID} state {state.State}.", $"Message: {state.Message.ToJsonString()}. Details: {state.Details}");
                     packages [packageID] = state ;
                  }
               } else {
                  Logger.Log ("Cannot dequeue package ID for store state.") ;
               }
            }

            foreach (var packageID in packages.Keys) {
               var packageState = packages [packageID] ;
               // Order states on alerting or OK
               InstanceStateList sortedInstanceStates = new InstanceStateList() ;
               if (packageState.InstanceStates != null) {
                  sortedInstanceStates = new InstanceStateList (packageState.InstanceStates.OrderBy (x => GetPriority (x.Value)).ToDictionary (pair => pair.Key, pair => pair.Value)) ;

                  sortedInstanceStates.Remove (Guartinel.Communication.Strings.Strings.HEARTBEAT_INSTANCE_ID) ;
               }

               Logger.Debug ($"Preparing package state for sending for package '{packageID}'. " +
                             $"State: {packageState.State}. " +
                             $"Message: {packageState.Message?.ToJsonString()}. " +
                             $"Details: {packageState.Details?.ToJsonString()}") ;

               foreach (var instanceState in sortedInstanceStates) {
                  Logger.Debug ($"Preparing instance state for sending for instance '{instanceState.Key}' state '{packageID}'. " +
                                $"State: {instanceState.Value.Name}. " +
                                $"Message: {instanceState.Value.Message?.ToJsonString()}. " +
                                $"Extract: {instanceState.Value.Extract?.ToJsonString()}. " +
                                $"Details: {instanceState.Value.Details?.ToJsonString()}") ;
               }

               CallWithStatusCheck(() => {
                  DoStorePackageState (packageID, packageState.State, packageState.Message, packageState.Details, sortedInstanceStates) ;
               }) ;
            }

#warning SzTZ: this must be rewritten to a event ManualResetEvent
            // http://dotnetpattern.com/threading-manualresetevent
            Thread.Sleep (TimeSpan.FromSeconds (PackageStateStoreIntervalInSeconds)) ;
         }
      }

      #endregion

      public IList<string> GetApplicationInstanceIDs (string packageID) {
         IList<string> result = new List<string>() ;
         // Use distinct list
         CallWithStatusCheck (() => result = DoGetApplicationInstanceIDs (packageID).Distinct().ToList());

         return result ;
      }

      protected abstract IList<string> DoGetApplicationInstanceIDs (string packageID) ;
   }
}