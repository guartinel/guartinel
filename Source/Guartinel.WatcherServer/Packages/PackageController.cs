using System ;
using System.Collections.Concurrent ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using System.Threading.Tasks ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Entities ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.WatcherServer.Packages {
   public class PackageController : IDisposable {
      // public PackageController (int numberOfPackageRunners) {
      public PackageController() {
         _logger = new TagLogger(null) ;
         _stateLoggingTimer = new Timer (state => LogPackages(), null, TimeSpan.FromSeconds (30), TimeSpan.FromSeconds (30)) ;

         //if (numberOfPackageRunners == 0) {
         //   numberOfPackageRunners = 1 ;
         //}

         // Create Package runners
         //while (_packageRunners.Count < numberOfPackageRunners) {
         //   CreatePackageRunner ("PackageRunner" + _packageRunners.Count) ;
         //}
      }

      public void Dispose() {
         _stateLoggingTimer.Dispose() ;

         // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
         //foreach (var packageRunner in PackageRunners) {
         //   packageRunner.Stop();
         //}         
      }

      protected readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource() ;
      protected volatile bool _threadRuns = false ;

      protected readonly ConcurrentDictionary<string, Package> _packages = new ConcurrentDictionary<string, Package>() ;

      public class PackageCheck {
         public PackageCheck(Package package,
                             CancellationTokenSource cancellation,
                             // Thread checkingThread,
                             Task task,
                             DateTime startedAt) {
            Package = package ;
            Cancellation = cancellation ;
            // CheckingThread = checkingThread ;
            Task = task ;            
            StartedAt = startedAt ;
         }
         public Package Package ;
         public CancellationTokenSource Cancellation ;
         // public Thread CheckingThread ;
         public Task Task ;
         public DateTime StartedAt ;
      }

      protected readonly ConcurrentDictionary<string, PackageCheck> _packageChecks = new ConcurrentDictionary<string, PackageCheck>() ;

      // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
      // #region Package runners
      // Note: Package runners run in separate threads!
      // private readonly List<PackageRunner> _packageRunners = new List<PackageRunner>() ;

      //protected List<PackageRunner> PackageRunners {
      //   get {return _packageRunners ;}
      //}

      // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
      // get { return _packageRunners.Sum (x => x.PackageCount) ;}
      public int PackageCount => _packages.Count ;

      //private PackageRunner CreatePackageRunner (string name) {
      //   var packageRunner = new PackageRunner(name);

      //   _packageRunners.Add(packageRunner);

      //   return packageRunner;
      //}

      // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
      //protected PackageRunner ChooseLeastLoadedPackageRunner () {
      //   return _packageRunners.OrderBy(x => x.CalculateWorkload()).FirstOrDefault();
      //}

      // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
      // #endregion

      public void AddPackage (Package package) {
         // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
         // var packageRunner = ChooseLeastLoadedPackageRunner() ?? CreatePackageRunner(_packageRunners.Count.ToString()) ;

         // Logger.Log ($"Package {package.ID} is added to package runner {packageRunner.Name}.") ;

         // packageRunner.AddPackage (package) ;

         _packages.AddOrUpdate (package.ID, package, (packageID,
                                                      oldPackage) => package) ;
         _logger.Info ($"Package {package.ID} is added to controller.") ;
      }

      public void UsePackage (string packageID,
                              Action<Package> use) {

         if (string.IsNullOrEmpty (packageID)) return ;
         if (use == null) return ;

         // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
         //foreach (PackageRunner packageRunner in _packageRunners) {
         //   if (packageRunner.ExistsPackage(packageID)) {
         //      packageRunner.UsePackage(packageID, use);
         //   }
         //}

         if (!_packages.ContainsKey (packageID)) return ;
         var package = _packages [packageID] ;

         lock (package) {
            use (package) ;
         }
      }

      public bool ExistsPackage (string packageID) {
         if (string.IsNullOrEmpty (packageID)) return false ;

         //var exists = _packageRunners.Any (packageRunner => packageRunner.ExistsPackage (packageID)) ;

         //// Logger.Log ($"Package {packageID} exists: {exists}") ;

         //return exists ;
         // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
         return _packages.ContainsKey (packageID) ;
      }

      public void DeletePackage (string packageID,
                                 string[] tags) {
         // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
         //foreach (var packageRunner in _packageRunners) {
         //   if (packageRunner.ExistsPackage (packageID)) {
         //      packageRunner.RemovePackage (packageID, tags) ;
         //   }
         //}

         _packages.TryRemove (packageID, out _) ;
         _packageChecks.TryRemove (packageID, out _) ;
      }

      public IEnumerable<EntityTimestamp> GetPackageTimestamps() {
         List<EntityTimestamp> result = new List<EntityTimestamp>() ;

         // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
         //foreach (var packageRunner in PackageRunners) {
         //   result.AddRange (packageRunner.GetModificationTimestamps()) ;
         //}

         foreach (var packageItem in _packages) {
            result.Add (new EntityTimestamp (packageItem.Value.ID,
                                             packageItem.Value.ModificationTimestamp)) ;
         }

         return result ;
      }

      public void Start() {
         // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
         //foreach (var packageRunner in PackageRunners) {
         //   packageRunner.RunAsync() ;
         //}

         _logger.Info ($"Package controller started...") ;

         var token = _cancellationTokenSource.Token ;
         new Thread (() => {
            RunPackageChecks (token) ;
         }).Start() ;
      }

      private void RunPackageChecks (CancellationToken cancel) {
         _threadRuns = true ;
         try {
            while (!cancel.IsCancellationRequested) {
               foreach (var packageItem in _packages) {
                  var package = packageItem.Value ;
                  if (DateTime.Compare (package.GetNextCheckTime(), DateTime.UtcNow) <= 0) {
                     // Check if check already runs
                     PackageCheck packageCheck ;
                     if (_packageChecks.ContainsKey (package.ID)) {
                        _logger.Info ($"Previous check runs for package '{package.ID}', named '{package.Name}'.") ;
                        // Cancel previoud check
                        _packageChecks.TryRemove (package.ID, out packageCheck) ;
                        packageCheck?.Cancellation.Cancel() ;
                     }

                     _logger.Info ($"Run checks for package '{package.ID}', named '{package.Name}'.") ;

                     var cancellationSource = new CancellationTokenSource() ;
                     // Run check and when finished, remove from checks
                     // var thread = new Thread (() => {
                     // var task = new Task(() => {
                     var task = new Task(() => {
                        // @todo SzTZ: cancellation token is not used here
                        package.RunChecks (_logger.Tags) ;
                        _packageChecks.TryRemove (package.ID, out _) ;
                     }, cancellationSource.Token) ;

                     // packageCheck = new PackageCheck (package, new CancellationTokenSource(), thread, DateTime.UtcNow) ;
                     packageCheck = new PackageCheck(package, cancellationSource, task, DateTime.UtcNow);
                     _packageChecks.AddOrUpdate (package.ID, packageCheck, (packageID,
                                                                            packageCheckOld) => packageCheck) ;

                     // thread.Start() ;
                     task.Start();
                  }
               }
               new TimeoutSeconds (1).Wait() ;
            }
         } finally {
            _threadRuns = false ;
         }
      }

      public void Stop() {
         _logger.Info ($"Package controller exited.") ;

         // Cancel main thread, and try to wait for it
         _cancellationTokenSource.Cancel() ;
         new TimeoutSeconds (10).WaitFor(() => !_threadRuns) ;
      }

      //public void ClearPackages() {
      //   // 3406FE00-BDF9-4B14-8FD4-84F4D698C934
      //   //foreach (var packageRunner in PackageRunners) {
      //   //   packageRunner.ClearPackages() ;
      //   //}

      //   _packages.Clear() ;
      //}

      private class PackageNextCheckTime {
         public readonly Package Package ;
         public readonly DateTime NextCheckTime ;

         public PackageNextCheckTime (Package package,
                                      DateTime nextCheckTime) {
            Package = package ;
            NextCheckTime = nextCheckTime ;
         }
      }

      #region Logging

      protected readonly TagLogger _logger;
      protected readonly Timer _stateLoggingTimer;

      private void LogPackages() {
         // Log packages
         if (Logger.Settings.IsLogEnabled (LogLevel.Debug)) {
            var logger = new TagLogger (_logger.Tags) ;
            StringBuilder logText = new StringBuilder() ;
            logText.Append ($"Package statuses: ") ;
            foreach (var packageItem in _packages) {
               var nextCheckTime = new PackageNextCheckTime (packageItem.Value, packageItem.Value.GetNextCheckTime()) ;
               logText.Append ($"---> Package ID: {nextCheckTime.Package.ID}, next check time: {nextCheckTime.NextCheckTime.ToUniversalTime()}") ;
            }

            logger.Debug (logText.ToString()) ;
         }
      }
      #endregion
   }
}