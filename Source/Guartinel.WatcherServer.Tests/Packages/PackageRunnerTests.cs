using System ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Packages ;
using NUnit.Framework ;
using Timeout = Guartinel.Kernel.Timeout ;

namespace Guartinel.WatcherServer.Tests.Packages {
   [TestFixture]
   public class PackageRunnerTests : TestsBase {
      protected override void Setup1() {
         // Factory.Use.RegisterCreator (new Creator (typeof (TestChecker), () => new TestChecker(), typeof (Checker))) ;
         // Factory.Use.RegisterCreator (new Creator (typeof (TestAlert), () => new TestAlert(), typeof (Alert))) ;
      }

      protected override void TearDown1() {
         // Factory.Use.UnregisterCreators (typeof (TestChecker)) ;
         // Factory.Use.UnregisterCreators (typeof (TestAlert)) ;
      }

      protected override void RegisterLoggers() {
         // No loggers here - performance
      }

      private static PackageController CreateTestSetup (bool success,
                                                        TestAlertCounter testAlertCounter,
                                                        int numberOfPackages,
                                                        int numberOfCheckers,
                                                        int checkIntervalSeconds = 100,
                                                        int timeoutIntervalSeconds = 150,
                                                        int startupDelaySeconds = 0) {
         // Make setup
         PackageController packageController = new PackageController() ;
         // List<Alert> alerts = new List<Alert>() ;
         // alerts.Add (alert) ;

         for (var packageIndex = 0; packageIndex < numberOfPackages; packageIndex++) {
            var package = new TestPackage() ;

            package.Configure (Guid.NewGuid().ToString(), success, testAlertCounter, null, null,
                               checkIntervalSeconds, timeoutIntervalSeconds, startupDelaySeconds,
                               numberOfCheckers) ;
            packageController.AddPackage (package) ;
         }

         return packageController ;
      }

      [Test]
      public void CreatePackageController_Start_AddPackages_Check() {
         const int NUMBER_OF_PACKAGES = 10 ;
         const int NUMBER_OF_CHECKERS = 100 ;
         const int NUMBER_OF_ALL_CHECKS = NUMBER_OF_PACKAGES * NUMBER_OF_CHECKERS ;

         var testAlertCounter = new TestAlertCounter() ;
         var packageController = CreateTestSetup (false, testAlertCounter, NUMBER_OF_PACKAGES, NUMBER_OF_CHECKERS, 1000, 1500, 1) ;
         packageController.Start() ;
         try {
            // Need to wait until the threads are done, should be quick
            Timeout timeout = new Timeout (NUMBER_OF_ALL_CHECKS * 5 + 5000) ;

            timeout.WaitFor (() => testAlertCounter.AlertCount >= NUMBER_OF_ALL_CHECKS) ;

            // Sleep a bit more to find if too may alerts we have
            Thread.Sleep (1000) ;
            Assert.AreEqual (NUMBER_OF_ALL_CHECKS, testAlertCounter.AlertCount) ;
         } finally {
            packageController.Stop() ;
         }

         //Assert.AreEqual (1, watcherRunner.Checkers.Count) ;
         //TestChecker3 checker1 = (watcher.Checkers [0] as TestChecker3) ;
         //Assert.IsNotNull (checker1) ;
         //Assert.AreSame (checker, checker1) ;
      }
   }
}