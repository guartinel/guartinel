using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Packages {
   /// <summary>
   /// Execute check in a separate thread, and report back the result.
   /// It evaluates the result of check, and raises the alerts.
   /// </summary>
   public class CheckExecuter {
      public CheckExecuter (string packageID,
                            Func<string> getPackageState,
                            IEnumerable<Checker> checkers,
                            InstanceList instances,
                            IEnumerable<Alert> alerts,
                            bool forcedDeviceAlert,
                            string[] tags) {
         // _package = package ;
         _packageID = packageID ;
         _getPackageState = getPackageState ;

         _checkers = checkers ;
         _instances = instances ;
         _alerts = alerts ;
         _forcedDeviceAlert = forcedDeviceAlert ;

         _logger = new TagLogger (tags, TagLogger.CreateTag()) ;
         _logger.Debug ($"Check executer created for package '{_packageID}'.") ;
      }

      // private readonly Package _package ;
      private readonly string _packageID ;

      private readonly Func<string> _getPackageState ;
      private readonly IEnumerable<Checker> _checkers ;
      private readonly IEnumerable<Alert> _alerts ;
      private readonly bool _forcedDeviceAlert ;
      private readonly InstanceList _instances ;

      private readonly TagLogger _logger ;

      // private int _counter = 0 ;

      public void Run() {
         if (_checkers == null) return ;

         // if (checkBehavior == null) checkBehavior = new ExecuteBehavior.Synchronized() ;
         var instanceIDs = _instances.Keys.ToList() ;

         //foreach (Checker checker in _checkers) {
         //   // Check flag
         //   // if (!_runFlag.IsEnabled) return ;

         //   // Run check only for valid instances!
         //   // Allow package heartbeat
         //   if (!checker.AllowInstanceCheck (instanceIDs)) {
         //      _logger.Log($"Instance '{_guid}' check is not allowed.");
         //      continue;
         //   }

         //   var checkerLocal = checker ;
         //   // Use synchronized behavior - no tasks
         //   // SzTZ: 2018/06/21: try task pool to allow parallel checks
         //   // var behavior = new ExecuteBehavior.Synchronized() ;
         //   var behavior = new ExecuteBehavior.InTask() ;
         //   behavior.Execute ($"Check {_name}: {checkerLocal.Name}",
         //                     () => ExecuteCheckAndFireAlerts (checkerLocal, new ExecuteBehavior.Synchronized()),
         //                     _logger.Log) ;
         //}

         _logger.Info ($"Running {_checkers.Count()} checks.") ;

         // SzTZ: 2018/06/21: try task pool to allow parallel checks
         Parallel.ForEach (_checkers,
                           new ParallelOptions {MaxDegreeOfParallelism = ApplicationSettings.Use.CheckersPerPackage},
                           checker => {
                              var logger = new TagLogger(_logger.Tags, checker.InstanceID) ;

                              // Check flag
                              // if (!_runFlag.IsEnabled) return ;

                              // Run check only for valid instances!
                              // Allow package heartbeat
                              if (!checker.AllowInstanceCheck (instanceIDs)) {
                                 logger.Info ("Check is not allowed.") ;
                                 return ;
                              }

                              logger.Info ($"Running '{checker.GetType().Name}' checker for instance '{checker.InstanceID}'...") ;

                              var checkerLocal = checker ;
                              // Use synchronized behavior - no tasks
                              ExecuteCheckAndFireAlerts (checkerLocal) ;
                           }) ;
      }

      protected void ExecuteCheckAndFireAlerts (Checker checker) {
         try {
            if (checker == null) return ;
            
            var logger = new TagLogger(_logger.Tags, checker.InstanceID);

            // Check flag
            // if (!_runFlag.IsEnabled) return ;
            logger.Debug ($"Executing check '{checker.Name}' for package '{_packageID}', state is '{_getPackageState?.Invoke()}'...") ;
            var checkResults = checker.Check (_logger.Tags) ;
            logger.Debug ($"After executing check, package state is '{_getPackageState?.Invoke()}'...") ;

            foreach (var checkResult in checkResults) {
               logger.InfoWithDebug ($"Check result is {checkResult.CheckResultKind}.",
                                      $"Message: {checkResult.Message?.ToJsonString()} Details: {checkResult.Details?.ToJsonString()}...") ;

               // Check instance!            
               if (!_instances.ContainsKey (checker.InstanceID)) {
                  continue ;
               }

               Instance instance = _instances [checker.InstanceID] ;

               Dictionary<Alert, AlertInfo> alerts = new Dictionary<Alert, AlertInfo>() ;

               foreach (var alert in _alerts) {
                  AlertKind alertKind = instance.GetAlertKind (alert.ID, _packageID, checkResult.CheckResultKind, checkResult.Message) ;

                  if (alertKind == AlertKind.None) continue ;

                  logger.Debug ($"Preparing alert {checkResult.Message?.ToJsonString()} ...") ;

                  var alertInfo = new AlertInfo().Configure (checkResult,
                                                             checkResult.Message,
                                                             checkResult.Details,
                                                             checkResult.Extract,
                                                             _packageID, alert.ID,
                                                             alertKind,
                                                             _forcedDeviceAlert) ;

                  // This is where the instance can change the alert info (like change the message to recovery)
                  instance.AdjustAlertInfo (alertInfo) ;

                  alerts.Add (alert, alertInfo) ;
               }

               // Register new state of instance
               instance.RegisterCheckResult (checkResult, logger.Tags) ;

               var packageState = _getPackageState?.Invoke() ;
               logger.Debug ($"Package state is '{packageState}'...") ;

               // SzTZ: add parallelism to alerts               
               // foreach (var alert in _alerts) {
               Parallel.ForEach (_alerts,
                                 alert => {
                                    if (!alerts.ContainsKey (alert)) return ;

                                    var alertLocal = alert ;
                                    alerts [alertLocal].PackageState = packageState ;

                                    // Alert
                                    logger.Debug ($"Alerting for alert {alertLocal.Name}. Package state: '{packageState}. Message: {alerts [alertLocal].Message?.ToJsonString()} ...") ;
                                    lock (alertLocal) {
                                       // todo: SzTZ: what if Fire does not succeed?
                                       alertLocal.Request (instance, alerts [alertLocal], _logger.Tags) ;
                                       instance.AfterAlert (alerts [alertLocal]) ;
                                    }
                                 }) ;
            }

            MessageBus.Use.Post (checker.PackageID, new Package.StateChangedMessage()) ;
         } catch (Exception e) {
            // _counter++ ;
            _logger.Error ($"Error when executing check. Message: {e.GetAllMessages()}.") ;
            throw ;
         }
      }
   }
}