using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.WatcherServer.CheckResults ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Packages {
   [TestFixture]
   public class PackageTests : TestsBase {
      protected override void Setup1() {
         // Factory.Use.RegisterCreator (new Creator (typeof (TestChecker), () => new TestChecker(), typeof (Checker))) ;
         // Factory.Use.RegisterCreator (new Creator (typeof (TestAlert), () => new TestAlert(), typeof (Alert))) ;
      }

      protected override void TearDown1() {
         // Factory.Use.UnregisterCreators (typeof (TestChecker)) ;
         // Factory.Use.UnregisterCreators (typeof (TestAlert)) ;
      }

      protected static TestPackage CreateTestPackage (bool success,
                                                      int checkerCount = 1,
                                                      int checkIntervalSeconds = 10,
                                                      int timoutIntervalSeconds = 15,
                                                      int startupDelaySeconds = 0) {
         var package = new TestPackage() ;
         // Make setup
         package.Configure (Guid.NewGuid().ToString(), success, null, null, null, checkIntervalSeconds, timoutIntervalSeconds, startupDelaySeconds, checkerCount) ;

         //if (startChecker) {
         //   checker.Enabled = true ;
         //   checker.Start() ;
         //}

         //// WaitFor a bit
         //if (startChecker && waitForCheck) {
         //   checker.AfterCheck += delegate {
         //      checker.Stop() ;
         //   } ;

         //   while (checker.IsRunning) {
         //      Thread.Sleep (200) ;
         //   }
         //}

         return package ;
      }

      [Test]
      public void CreateTestPackage_CheckCheckerCount() {
         var package = new TestPackage() ;
         package.Configure (true) ;
         Assert.AreEqual (1, package.CreateCheckers().Count) ;

         package = new TestPackage() ;
         package.Configure (true, null, 5) ;

         Assert.AreEqual (5, package.CreateCheckers().Count) ;
      }

      [Test]
      public void CheckScheduleOfPackage() {
         // Check if initial time is between the right range
         for (int index = 0; index < 30; index++) {
            var startupDelay = new Random().Next (1, 4) ;
            var now = DateTime.UtcNow;
            var package = CreateTestPackage (true, 1, 5, 10, startupDelay) ;
            var initialCheckTime = package.GetNextCheckTime() ;
            Thread.Sleep (100) ;
            Assert.LessOrEqual (now.AddSeconds (startupDelay), initialCheckTime) ;
            Assert.LessOrEqual (now, initialCheckTime) ;
         }

         var now1 = DateTime.UtcNow ;
         var package2 = CreateTestPackage (true, 1, 5, 10, 3) ;
         // Sleep a bit more
         Thread.Sleep (4 * 1000) ;
         var initialCheckTime2 = package2.GetNextCheckTime() ;
         // The schedule is overdue
         var now2 = DateTime.UtcNow ;
         // Assert.AreEqual (initialCheckTime2.Ticks, now2.Ticks, TimeSpan.TicksPerSecond / 2) ;
         
         Assert.GreaterOrEqual (initialCheckTime2, now1) ;
         Assert.GreaterOrEqual (now2, initialCheckTime2) ;

         // After run, the next run should be around the check interval
         package2.RunChecks (null) ;
         var now3 = DateTime.UtcNow ;
         var nextCheckTime3 = package2.GetNextCheckTime() ;
         Assert.AreEqual (now3.AddSeconds (5).Ticks, nextCheckTime3.Ticks, TimeSpan.TicksPerSecond / 2,
                          $"{now3} <> {nextCheckTime3}") ;
      }

      [Test]
      public void RunChecksOfPackage_CheckIfExecutedAndAlerted() {
         var package = CreateTestPackage (true) ;
         
         // alert.Fire (new AlertRequest().Configure ("AlertReq1", new CheckResult("Result1", CheckResultSuccess.Success))) ;
         // alert.Fire (new AlertRequest().Configure (new CheckResult().Configure ("Result1", CheckResultSuccess.Success), "AlertReq1")) ;         
         Assert.AreEqual (1, package.CreateCheckers().Count) ;
         Assert.IsNull (package.TestAlertCounter.AlertInfo) ;

         package.RunChecks(null) ;
         // No alert!
         Assert.IsNull (package.TestAlertCounter.AlertInfo) ;

         package = CreateTestPackage (false) ;
         Assert.AreEqual (1, package.CreateCheckers().Count) ;
         Assert.IsNull (package.TestAlertCounter.AlertInfo) ;

         package.RunChecks (null) ;
         // Alert!
         Assert.IsNotNull (package.TestAlertCounter.AlertInfo) ;
         Assert.IsTrue (package.TestAlertCounter.AlertInfo.CheckResult.CheckResultKind == CheckResultKind.Fail) ;
      }

      [Test]
      public void RunChecksOfWatcherInThreads_CheckIfExecutedAndAlerted() {
         var package = CreateTestPackage (false) ;
         Assert.AreEqual (1, package.CreateCheckers().Count) ;
         Assert.IsNull (package.TestAlertCounter.AlertInfo) ;

         package.RunChecks (null) ;
         // Need to wait until the threads are done, should be quick
         Thread.Sleep (2000) ;

         // Alert!
         Assert.IsNotNull (package.TestAlertCounter.AlertInfo) ;
         Assert.IsTrue (package.TestAlertCounter.AlertInfo.CheckResult.CheckResultKind == CheckResultKind.Fail) ;
      }

      // private void SetupLotOfChecksRunAndCheck (bool stop) {
      private void SetupLotOfChecksRunAndCheck() {
         const int NUMBER_OF_PACKAGES = 1000 ;
         const int NUMBER_OF_CHECKERS = 10 ;
         const int NUMBER_OF_ALL_CHECKS = NUMBER_OF_PACKAGES * NUMBER_OF_CHECKERS ;

         TestAlertCounter testAlertCounter = new TestAlertCounter() ;

         List<TestPackage> packages = new List<TestPackage>() ;
         for (int packageIndex = 0; packageIndex < NUMBER_OF_PACKAGES; packageIndex++) {
            var package = new TestPackage().Configure (Guid.NewGuid().ToString(), false, testAlertCounter, null, null,
                                                       1000, 1500, 1, NUMBER_OF_CHECKERS) ;

            packages.Add (package) ;

            Assert.AreEqual (0, package.TestAlertCounter.AlertCount) ;
         }

         foreach (var package in packages) {
            package.RunChecks (null) ;
         }

         //if (stop) {
         //   Thread.Sleep (1500) ;
         //   // Stop all watchers!
         //   // Some of them will finish anyway
         //   foreach (var package in packages) {
         //      package.StopChecks() ;
         //   }
         //}

         // Need to wait until the threads are done, should be quick
         new Kernel.Timeout (NUMBER_OF_ALL_CHECKS * 10 + 5000).WaitFor (() => testAlertCounter.AlertCount >= NUMBER_OF_ALL_CHECKS) ;

         // Wait a bit, to check if too many alerts are executed
         Thread.Sleep (1500) ;

         //if (stop) {
         //   // Not all alerts should be fired
         //   Assert.Greater (testAlertCounter.AlertCount, 0) ;
         //   Assert.Less (testAlertCounter.AlertCount, NUMBER_OF_ALL_CHECKS) ;
         //} else {
         //   // Alerted how many times?
         //   Assert.AreEqual (NUMBER_OF_ALL_CHECKS, testAlertCounter.AlertCount) ;
         //}

         // Alerted how many times?
         Assert.AreEqual(NUMBER_OF_ALL_CHECKS, testAlertCounter.AlertCount);
      }

      [Test]
      public void RunLotOfChecksOfPackageInThreads_CheckIfExecutedAndAlerted() {
         // SetupLotOfChecksRunAndCheck (false) ;
         SetupLotOfChecksRunAndCheck() ;
      }

      // [Test]
      //public void RunLotOfChecksOfPackageInThreads_Stop_CheckIfStopped() {
      //   SetupLotOfChecksRunAndCheck (true) ;
      //}
   }
}