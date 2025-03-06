using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Entities ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Packages {
   public class Package : Entity, IDisposable {
      #region Constants
      public static class Constants {
         public const int DEFAULT_CHECK_INTERVAL_SECONDS = 15 * 60 ;
         public const int NO_CHECK_INTERVAL_SPECIFIED = 0 ;
         public const double DEFAULT_TIMEOUT_INTERVAL_FACTOR = 1.5 ;
         public const int NO_TIMEOUT_INTERVAL_SPECIFIED = 0 ;
         public const int DEFAULT_STARTUP_DELAY_SECONDS = 10 ;
         public const bool DEFAULT_FORCED_DEVICE_ALERT = false ;
         public const string CAPTION = "Package" ;

         // Default workload
         public const int DEFAULT_WORKLOAD = 100 ;
      }
      #endregion

      #region Construction
      public Package() {
         Name = string.Empty ;
         AlertText = new XSimpleString() ;
         CheckIntervalSeconds = 0 ;
         StartupDelaySeconds = 0 ;
      }

      public void Dispose() {
         // Unsubscribe from messages
         if (ID != null) {
            IoC.Use.Single.GetInstance<IManagementServerPackages>().DeletePackageStates (ID) ;
            UnregisterMessages() ;
            // MessageBus.Use.Unregister<InstanceCheckResultMessage>(ID) ;
         }

         Dispose1() ;
      }

      protected virtual void Dispose1() { }

      //public static Creator GetCreator() {
      //   return new Creator (typeof (Package), () => new Package(), typeof (Package), Constants.CAPTION) ;
      //}

      #endregion

      #region Configuration
      public Package Configure (string id,
                                string name,
                                Categories categories,
                                // License license,
                                List<string> alertEmails,
                                List<string> alertDeviceIDs,
                                int checkIntervalSeconds,
                                int timeoutIntervalSeconds,
                                int startupDelaySeconds,
                                bool forcedDeviceAlert,
                                DateTime modificationTimestamp,                                
                                ConfigurationData specificConfiguration,
                                InstanceStateList instanceStates = null,
                                Schedules disabledAlerts = null) {
         ID = string.IsNullOrEmpty (id) ? Guid.NewGuid().ToString() : id ;
         _logger = new TagLogger (TagLogger.CreateTag (nameof(Package), ID)) ;
         _logger.Debug ("Configuring package.") ;

         // Unregister package
         if (!string.IsNullOrEmpty (ID)) {
            UnregisterMessages() ;
         }

         Name = name ;
         Categories = new Categories (categories) ;
         _silentSchedules = disabledAlerts ;
         // License = license ;

         CheckIntervalSeconds = Math.Max (checkIntervalSeconds, Package.Constants.NO_CHECK_INTERVAL_SPECIFIED) ;
         if (CheckIntervalSeconds == Package.Constants.NO_CHECK_INTERVAL_SPECIFIED) {
            CheckIntervalSeconds = Package.Constants.DEFAULT_CHECK_INTERVAL_SECONDS ;
         }

         TimeoutIntervalSeconds = Math.Max (timeoutIntervalSeconds, Package.Constants.NO_TIMEOUT_INTERVAL_SPECIFIED) ;
         if (TimeoutIntervalSeconds == Package.Constants.NO_TIMEOUT_INTERVAL_SPECIFIED) {
            TimeoutIntervalSeconds = (int) (checkIntervalSeconds * Package.Constants.DEFAULT_TIMEOUT_INTERVAL_FACTOR) ;
         }

         StartupDelaySeconds = Math.Max (startupDelaySeconds, 0) ;

         ModificationTimestamp = modificationTimestamp ;
         ForcedDeviceAlert = forcedDeviceAlert ;

         RegisterMessages() ;

         _alerts.Clear() ;

         // Create mail alerts
         if (alertEmails != null) {
            foreach (var alertEmail in alertEmails) {
               _alerts.Add (new MailAlert().Configure (Name, ID, alertEmail)) ;
            }
         }

         // Create device alerts
         if (alertDeviceIDs != null) {
            foreach (var alertDeviceID in alertDeviceIDs) {
               _alerts.Add (new DeviceAlert().Configure (Name, ID, alertDeviceID, 0)) ;
            }
         }

         // Clear instances
         lock (_instancesLock) {
            _instances.Clear() ;
         }

         // Descendant configuration
         if (specificConfiguration != null) {
            SpecificConfigure (specificConfiguration) ;
         }

         // Randomize the first time to avoid high traffic at once
         SetNextCheckTime() ;

         // Notification about package status change
         //if (Factory.Use.RegisteredCreatorExists<IManagementServerPackages>()) {
         //   Factory.Use.CreateInstance<IManagementServerPackages>().StorePackageState (ID,
         //                                                                              ManagementServerAPI.Package.StoreState.Request.State.StateNames.UNKNOWN,
         //                                                                              new XSimpleString (string.Empty),
         //                                                                              new XSimpleString (string.Empty),
         //                                                                              GetinstanceStates()) ;
         //}

         var filteredInstances = new InstanceStateList() ;
         lock (_instancesLock) {
            if (instanceStates != null && _instances != null) {
               foreach (var filteredInstance in instanceStates.Where (x => _instances.ContainsKey (x.Key))) {
                  filteredInstances.Add (filteredInstance.Key, filteredInstance.Value) ;
               }
            }
         }

         // Store instance states, set delivery notification for ALL alerts in package
         SetInstanceStates (filteredInstances, true, _alerts.Select (x => x.ID).ToList(), true) ;

         // Notification about package status change
         MessageBus.Use.Post (ID, new StateChangedMessage()) ;

         return this ;
      }

      public Package Configure (string id,
                                string name,
                                Categories categories,
                                // License license,
                                List<string> alertEmails,
                                List<string> alertDeviceIDs,
                                int checkIntervalSeconds,
                                int timeoutIntervalSeconds,
                                int startupDelaySeconds,
                                bool forcedDeviceAlert) {
         Configure (id, name, categories, // license,
                    alertEmails, alertDeviceIDs,
                    checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds, forcedDeviceAlert,
                    DateTime.UtcNow, new ConfigurationData()) ;

         return this ;
      }

      public Package Configure (string id,
                                List<string> alertEmails,
                                List<string> alertDeviceIDs,
                                int checkIntervalSeconds,
                                int timeoutIntervalSeconds,
                                int startupDelaySeconds,
                                bool forcedDeviceAlert) {
         return Configure (id, string.Empty, new Categories(),
                           // new License (string.Empty),
                           alertEmails, alertDeviceIDs,
                           checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds, forcedDeviceAlert,
                           DateTime.UtcNow, new ConfigurationData()) ;
      }

      public Package Configure (string id,
                                int checkIntervalSeconds,
                                int timeoutIntervalSeconds,
                                int startupDelaySeconds,
                                bool forcedDeviceAlert) {
         return Configure (id, string.Empty, new Categories(),
                           null, null,
                           checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds, forcedDeviceAlert,
                           DateTime.UtcNow, new ConfigurationData()) ;
      }

      /// <summary>
      /// Let descendants configure themselves.
      /// </summary>
      /// <param name="configuration"></param>
      protected virtual void SpecificConfigure (ConfigurationData configuration) { }

      protected TagLogger _logger = new TagLogger() ;

      #endregion

      #region Messages
      public class StateChangedMessage : MessageBus.Message { }

      public class InstanceCheckResultMessage : MessageBus.Message {
         public string InstanceID {get ;}
         public CheckResult CheckResult {get ;}

         public InstanceCheckResultMessage (string instanceID,
                                            CheckResult checkResult) {
            InstanceID = instanceID ;
            CheckResult = checkResult ;
         }
      }

      protected void RegisterMessages() {
         MessageBus.Use.Register<StateChangedMessage> (ID, StateChanged) ;
         // MessageBus.Use.Register<InstanceCheckResultMessage>(ID, RegisterInstanceResult) ;

         RegisterMessages1() ;
      }

      protected virtual void RegisterMessages1() { }

      public void UnregisterMessages() {
         MessageBus.Use.Unregister<StateChangedMessage> (ID) ;
         UnregisterMessages1() ;
      }

      protected virtual void UnregisterMessages1() { }

      #endregion

      #region Instances
      protected readonly InstanceList _instances = new InstanceList() ;
      protected readonly object _instancesLock = new object() ;

      protected void SetInstances (IList<string> identifiers) {
         lock (_instancesLock) {
            _instances.Synchronize (identifiers) ;
         }
      }

      #endregion

      #region State
      public PackageState State {
         get {
            lock (_instancesLock) {
               // No instances
               if (!_instances.Values.Any()) {
                  return new PackageState.Unknown() ;
               }

               // Something is alerted?
               if (_instances.Values.Any (x => x.StateIsAlerting)) {
                  return new PackageState.Alerting().AggregateAlertMessages (_instances.Values.ToList()) ;
               }

               // Something is unknown?
               if (_instances.Values.Any (x => x.StateIsUnknown)) {
                  return new PackageState.Unknown() ;
               }
            }

            return new PackageState.OK() ;
         }
      }

      public InstanceStateList GetInstanceStates() {
         var result = new InstanceStateList() ;

         lock (_instancesLock) {
            foreach (var instance in _instances) {
               result.Add (instance.Key, instance.Value.State) ;
            }
         }

         return result ;
      }

      public void SetInstanceStates (InstanceStateList instanceStates,
                                     bool setAlertConfirmations,
                                     IList<string> alertIDs,
                                     bool addInstances) {
         if (instanceStates == null) return ;

         lock (_instancesLock) {
            foreach (var instanceID in instanceStates.Keys) {
               // By default, do not add
               if (addInstances && !_instances.ContainsKey (instanceID)) {
                  _instances.Add (instanceID) ;
               }

               _instances [instanceID].State = instanceStates [instanceID] ;

               if (setAlertConfirmations) {
                  foreach (var alertID in alertIDs) {
                     _instances [instanceID].NotifyAlertDelivery (alertID, instanceStates [instanceID].Message) ;
                  }
               }
            }
         }
      }

      private void StateChanged (StateChangedMessage message) {
         if (!IoC.Use.Single.ImplementationExists<IManagementServerPackages>()) return ;
         if (message == null) return ;

         Logger.Info ($"Package {ID} state changed to {State.StateName}.") ;

         IoC.Use.Single.GetInstance<IManagementServerPackages>().StorePackageState (ID,
                                                                                    State.StateName,
                                                                                    State.Message,
                                                                                    State.Details,
                                                                                    GetInstanceStates()) ;
      }

      //private void RegisterInstanceResult (InstanceCheckResultMessage message) {         
      //   if (message == null) return ;

      //   Logger.Log($"InstanceCheckResultMessage message arrived to package '{ID}' instance '{message.InstanceID}', {message.CheckResult.Success.ToString().ToLowerInvariant()}.");

      //   if (!_instances.ContainsKey (message.InstanceID)) return ;

      //   _instances [message.InstanceID].RegisterCheckResult (message.CheckResult) ;
      //}
      #endregion

      #region License
      /// <summary>
      /// The license object is used to determine the available operations.
      /// </summary>
      // public License License {get ; set ;}
      #endregion

      #region Schedule
      protected Schedules _silentSchedules = new Schedules() ;

      protected DateTime? _nextCheckTime ;

      protected void SetNextCheckTime() {
         _nextCheckTime = DateTime.UtcNow.AddSeconds (_nextCheckTime == null ? Math.Max (StartupDelaySeconds, 0) : CheckIntervalSeconds) ;
         _logger.Debug ($"Set package next check time to {_nextCheckTime?.ToString()}.") ;
      }

      protected DateTime GetNextCheckTimeWithNoSchedule() {
         // If not run yet, then start with initial
         if (_nextCheckTime == null) return DateTime.UtcNow ;

         // If overdue, then run NOW!
         // if (DateTime.Compare (_nextCheckTime.Value, DateTime.UtcNow) < 0) {
         //    return DateTime.UtcNow ;
         // }

         return _nextCheckTime.Value ;
      }

      public DateTime GetNextCheckTime() {
         var result = GetNextCheckTimeWithNoSchedule() ;

         // Check schedule
         DateTime? nextNotSilencedDateTime = _silentSchedules?.FindScheduleEnd() ;         
         if (nextNotSilencedDateTime != null) {
            // Logger.Info ($"Silenced time end: {nextNotSilencedDateTime.Value.ToUniversalTime().ToString()}, now: {DateTime.UtcNow.ToString()}") ;
            if (result < nextNotSilencedDateTime) {
               // Causes HUGE log file!
               // Logger.Log($"Schedule is active, check postponed from {result} to {nextNotSilencedDateTime}.");
               result = nextNotSilencedDateTime.Value ;
            }
         }

         return result ;
      }

      public void RunChecks (string[] tags) {
         var logger = new TagLogger (_logger.Tags, tags) ;
         
         ExceptionEx.ExecuteWithLog (logger1 => {
            // Set next check time
            SetNextCheckTime() ;

            List<Checker> checkers = CreateCheckers() ;

            if (Logger.Settings.IsLogEnabled (LogLevel.Debug)) {
               foreach (var checker in checkers) {
                  logger1.Debug ($"Checker '{checker.Name}' ({checker.InstanceID}) created.") ;
               }
            }

            // Execute in synchronized mode
            // new ExecuteBehavior.Synchronized().Execute ("Check Executer", () => {
            CheckExecuter checkExecuter ;
            lock (_instancesLock) {
               checkExecuter = new CheckExecuter (ID,
                                                  () => State.StateName,
                                                  // _checkSemaphor.GetFlag,
                                                  checkers,
                                                  _instances,
                                                  _alerts,
                                                  ForcedDeviceAlert,
                                                  logger1.Tags) ;
            }

            checkExecuter.Run() ;
            // }, Logger.Log) ;
         }, logger) ;
      }

      /// <summary>
      /// Stop checks by disabling flag.
      /// </summary>
      //public void StopChecks() {
      //   _checkSemaphor.DisableAndAbandonFlag() ;
      //}
      #endregion

      #region Statistics

      // Caclulate the workload of the package
      public virtual int CalculateWorkload() {
         // return Checkers.Sum (x => x.CalculateWorkload()) ;
         return Constants.DEFAULT_WORKLOAD ;
      }
      #endregion

      #region Checkers

      public void CancelCheckers() {
         _checkCancellationSource.Cancel();
      }
      protected CancellationTokenSource _checkCancellationSource = new CancellationTokenSource();

      public List<Checker> CreateCheckers() {
         // Cancel current checkers
         _checkCancellationSource.Cancel() ;
         _checkCancellationSource = new CancellationTokenSource() ;

         return CreateCheckers1() ;
      }

      protected virtual List<Checker> CreateCheckers1() {
         return new List<Checker>() ;
      }
      #endregion

      #region Alerts
      protected List<Alert> _alerts = new List<Alert>() ;
      public List<Alert> Alerts => _alerts ;

      public List<T> AlertsByType<T>() where T : Alert {
         return _alerts.Where (alert => alert is T).Cast<T>().ToList() ;
      }

      public void ConfirmDeviceAlert (string instanceID,
                                      string alertID,
                                      string[] tags) {
         var logger = new TagLogger (tags, alertID, instanceID) ;
         List<Instance> instances ;
         lock (_instancesLock) {
            instances = _instances.Values.Where (x => x.Identifier == instanceID).ToList() ;
         }

         XString lastMessage = null ;
         foreach (Alert alert in _alerts) {
            if (alert.ID == alertID) {
               if (alert.LastAlertInfo != null) {
                  lastMessage = alert.LastAlertInfo?.Message ;
               }
            }
         }

         logger.Debug ("Alert delivery confirmation.") ;

         instances.ForEach (x => x.NotifyAlertDelivery (alertID, lastMessage)) ;
      }

      protected XString _alertText ;

      public XString AlertText {get => _alertText ; set => _alertText = value ;}

      protected virtual void BeforeCheckIfAlertNeeded() { }
      #endregion

      #region Properties
      public string Name {get ; set ;}

      protected Categories _categories = new Categories() ;

      public Categories Categories {get => _categories ; set => _categories = new Categories (value) ;}

      public int StartupDelaySeconds {get ; set ;}
      public int CheckIntervalSeconds {get ; set ;}

      public int TimeoutIntervalSeconds { get; set; }
      public bool ForcedDeviceAlert { get; set; }      

      //protected override void AddBasicSummary (List<string> result) {
      //   result.Add ($"{HTML.MakeLabel ("Name")}{Name}") ;
      //   result.Add ($"{HTML.MakeLabel ("Delay")}{StartupDelaySeconds}") ;
      //   result.Add ($"{HTML.MakeLabel ("Interval")}{CheckIntervalSeconds}") ;
      //}
      #endregion
   }
}