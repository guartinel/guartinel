using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.Packages ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Packages {
   public class TestPackageController : PackageController {
      // public TestPackageController (int numberOfPackageRunners) : base (numberOfPackageRunners) {}

      // public new List<PackageRunner> PackageRunners => base.PackageRunners ;
      public List<Package> Packages => _packages.Values.ToList() ;
   }

   [TestFixture]
   public class PackageControllerTests : TestsBase {
      protected override void Setup1() {}

      private static TestPackageController CreateTestSetup (TestAlertCounter testAlertCounter,
                                                            string packageID,
                                                            string instanceID,
                                                            bool success,
                                                            // int numberOfWatcherRunners,
                                                            int numberOfPackages,
                                                            int numberOfCheckersPerPackage,
                                                            int checkIntervalSeconds,
                                                            int timeoutIntervalSeconds,
                                                            int startupDelaySeconds) {
         // Make setup
         // TestPackageController packagerController = new TestPackageController (numberOfWatcherRunners) ;
         TestPackageController packagerController = new TestPackageController();

         for (var packageIndex = 0; packageIndex < numberOfPackages; packageIndex++) {
            List<Checker> checkers = new List<Checker>() ;
            var package = new TestPackage() ;
            for (var checkerIndex = 0; checkerIndex < numberOfCheckersPerPackage; checkerIndex++) {
               TestChecker checker = new TestChecker().Configure ("Checker3" + checkerIndex, packageID, instanceID, success) ;
               checkers.Add (checker) ;
            }

            package.Configure (Guid.NewGuid().ToString(), success, testAlertCounter, null, null, checkIntervalSeconds, startupDelaySeconds,
                               timeoutIntervalSeconds, numberOfCheckersPerPackage) ;
            packagerController.AddPackage (package) ;
         }

         return packagerController ;
      }

      [Test]
      public void CreatePackageController_AddPackages_CheckIfTheyAreEquallyDistributed () {
         // const int NUMBER_OF_PACKAGE_RUNNERS = 7 ;
         // const int NUMBER_OF_PACKAGES = NUMBER_OF_PACKAGE_RUNNERS * 753 ;
         const int NUMBER_OF_PACKAGES = 7 * 753;
         const int NUMBER_OF_CHECKERS_PER_PACKAGE = 11 ;
         // const Int64 NUMBER_OF_ALL_CHECKERS = NUMBER_OF_WATCHER_RUNNERS * NUMBER_OF_PACKAGES_PER_RUNNER * NUMBER_OF_CHECKERS_PER_WATCHER ;

         TestAlertCounter testAlertCounter = new TestAlertCounter() ;

         var packageController = CreateTestSetup (testAlertCounter, "packageID1", "instanceID1", false,
         // NUMBER_OF_PACKAGE_RUNNERS, NUMBER_OF_PACKAGES, NUMBER_OF_CHECKERS_PER_PACKAGE, 1000, 1500, 1000) ;
                                                  NUMBER_OF_PACKAGES, NUMBER_OF_CHECKERS_PER_PACKAGE, 1000, 1500, 1000);
         //foreach (var watcherRunner in packageController.PackageRunners) {
         //   Assert.AreEqual (NUMBER_OF_PACKAGES / NUMBER_OF_PACKAGE_RUNNERS, watcherRunner.PackageCount) ;
         //}
         Assert.AreEqual(NUMBER_OF_PACKAGES, packageController.PackageCount);
         // Assert.IsTrue (packageController.PackageRunners.TrueForAll (x => x.PackageCount == NUMBER_OF_PACKAGES / NUMBER_OF_PACKAGE_RUNNERS)) ;
         // Assert.IsTrue (packageController.PackageRunners.TrueForAll (x => (Int64) (x.CalculateWorkload()) == Package.Constants.DEFAULT_WORKLOAD * (NUMBER_OF_PACKAGES / NUMBER_OF_PACKAGE_RUNNERS) * NUMBER_OF_CHECKERS_PER_PACKAGE)) ;
      }

      private static void AddPackages (TestPackageController packageController,
                                       TestAlertCounter testAlertCounter,
                                       int packageCount,
                                       int checkerCountPerPackage,
                                       bool success) {
         for (var packageIndex = 0; packageIndex < packageCount; packageIndex++) {
            var package = new TestPackage().Configure (Guid.NewGuid().ToString(), success, testAlertCounter, null, null, 1000, 1500, 1000, checkerCountPerPackage) ;
            packageController.AddPackage (package) ;
         }
      }

      // [Test]
      //public void CreatePackageController_Add2TypesOfPackages_CheckIfTheyAreEquallyDistributed() {
      //   const int NUMBER_OF_PACKAGE_RUNNERS = 8 ;
      //   const int NUMBER_OF_CHECKERS_PER_PACKAGE = 7 ;
      //   const int NUMBER_OF_PACKAGES = 4 ;

      //   // const Int64 NUMBER_OF_ALL_CHECKERS = NUMBER_OF_WATCHER_RUNNERS * NUMBER_OF_PACKAGES_PER_RUNNER * NUMBER_OF_CHECKERS_PER_WATCHER ;

      //   TestAlertCounter testAlertCounter = new TestAlertCounter() ;

      //   // Make setup
      //   TestPackageController packageController = new TestPackageController (NUMBER_OF_PACKAGE_RUNNERS) ;

      //   // First, add 4 packages with double checkers
      //   AddPackages (packageController, testAlertCounter, NUMBER_OF_PACKAGES, 2 * NUMBER_OF_CHECKERS_PER_PACKAGE, true) ;
      //   // Second, add 4 packages with checkers
      //   AddPackages (packageController, testAlertCounter, NUMBER_OF_PACKAGES, NUMBER_OF_CHECKERS_PER_PACKAGE, false) ;
      //   // Finally add 4 packages with checkers
      //   AddPackages (packageController, testAlertCounter, NUMBER_OF_PACKAGES, NUMBER_OF_CHECKERS_PER_PACKAGE, true) ;

      //   // 4 should have 4 packages, 4 should have 2
      //   Assert.AreEqual (4, packageController.PackageRunners.Count (x => x.PackageCount == 1)) ;
      //   Assert.AreEqual (4, packageController.PackageRunners.Count (x => x.PackageCount == 2)) ;

      //   Assert.IsTrue (packageController.PackageRunners.TrueForAll (x => (Int64) (x.CalculateWorkload()) ==
      //                                                                    2 * NUMBER_OF_CHECKERS_PER_PACKAGE * Package.Constants.DEFAULT_WORKLOAD)) ;
      //}

      [Test]
      public void CreatePackageController_AddPackages_CheckTimestamps () {
         // const int NUMBER_OF_PACKAGE_RUNNERS = 4 ;
         // const int NUMBER_OF_PACKAGES = NUMBER_OF_PACKAGE_RUNNERS * 3 ;
         const int NUMBER_OF_PACKAGES = 4 * 3 ;
         const int NUMBER_OF_CHECKERS_PER_PACKAGE = 2 ;

         TestAlertCounter testAlertCounter = new TestAlertCounter();

         // var packageController = CreateTestSetup (testAlertCounter, "packageID1", "instanceID1", false, NUMBER_OF_PACKAGE_RUNNERS, NUMBER_OF_PACKAGES, NUMBER_OF_CHECKERS_PER_PACKAGE, 1000, 1500, 1000);
         var packageController = CreateTestSetup(testAlertCounter, "packageID1", "instanceID1", false, NUMBER_OF_PACKAGES, NUMBER_OF_CHECKERS_PER_PACKAGE, 1000, 1500, 1000);
         //foreach (var watcherRunner in packageController.PackageRunners) {
         //   Assert.AreEqual(NUMBER_OF_PACKAGES / NUMBER_OF_PACKAGE_RUNNERS, watcherRunner.PackageCount);
         //}
         Assert.AreEqual (12, packageController.GetPackageTimestamps().Count()) ;
         Assert.AreEqual (12, packageController.GetPackageTimestamps().Distinct().Count()) ;
      }
   }
}
