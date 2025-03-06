using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Packages ;
using NUnit.Framework ;
using Timeout = Guartinel.Kernel.Timeout ;

namespace Guartinel.WatcherServer.Tests {
   [TestFixture]
   public class TestPackageController {
      [SetUp]
      public void Setup() {
         Logger.Setup ("Test", "Test");
      }

      [TearDown]
      public void TearDown() {
         IoC.Use.Clear();
      }

      public class TestChecker : Checker {
         public bool Success ;

         public TestChecker() {}

         protected override IList<CheckResult> Check1(string[] tags) {
            CheckResultKind result = CheckResultKind.Success ;
            if (!Success) {
               result = CheckResultKind.Fail ;
            }

            return new List<CheckResult> {new CheckResult().Configure ("TestCheck", result, null, null, null)} ;
         }
      }

      //public class TestAlert : Alert {
      //   private static readonly object _alertLock = new object();
      //   public static int AlertCount = 0 ;

      //   protected override Entity Create() {
      //      return new TestAlert();
      //   }

      //   protected override void Duplicate2 (Alert target) {
      //   }

      //   protected override void DoFire (AlertRequest alertRequest) {
      //      lock (_alertLock) {
      //         AlertCount++ ;
      //      }
      //   }
      //}

      public class PackageController1 : PackageController {
         // public PackageController1 (int numberOfPackageRunners) : base (numberOfPackageRunners) {}

         //public WatcherRunner GetLeastLoadedRunner() {
         //   return ChooseLeastLoadedWatcherRunner() ;
         //}

         // public List<PackageRunner> PackageRunners1 => PackageRunners ;
         public List<Package> Packages => _packages.Values.ToList() ;
      }

      private TestPackage CreatePackage (TestAlertCounter testAlertCounter,
                                         bool success = true,
                                         int checkIntervalSeconds = 30) {
         TestPackage result = new TestPackage().Configure (Guid.NewGuid().ToString(), success, testAlertCounter, null, null, checkIntervalSeconds) ;

         return result ;
      }

      [Test]
      public void CreatePackageController_CreatePackages_CheckLoad() {
         // var packageController = new PackageController1 (3) ;
         var packageController = new PackageController1();

         TestAlertCounter testAlertCounter = new TestAlertCounter() ;

         Assert.AreEqual (0, packageController.PackageCount) ;
         packageController.AddPackage (CreatePackage (testAlertCounter)) ;
         packageController.AddPackage (CreatePackage (testAlertCounter)) ;
         packageController.AddPackage (CreatePackage (testAlertCounter)) ;

         Assert.AreEqual (3, packageController.PackageCount) ;
         //Assert.AreEqual (1, packageController.PackageRunners1 [0].PackageCount) ;
         //Assert.AreEqual (1, packageController.PackageRunners1 [1].PackageCount) ;
         //Assert.AreEqual (1, packageController.PackageRunners1 [2].PackageCount) ;

         packageController.AddPackage (CreatePackage (testAlertCounter, false)) ;
         Assert.AreEqual(4, packageController.PackageCount);
         //Assert.AreEqual (1, packageController.PackageRunners1.Count (x => x.PackageCount == 2)) ;
         //Assert.AreEqual (2, packageController.PackageRunners1.Count (x => x.PackageCount == 1)) ;
      }

      [Test]
      public void CreatePackageController_CreatePackage_RunCheckers() {
         // PackageController1 packageController = new PackageController1 (3) ;
         PackageController1 packageController = new PackageController1();
         TestAlertCounter testAlertCounter = new TestAlertCounter() ;

         Assert.AreEqual (0, packageController.PackageCount) ;
         packageController.AddPackage (CreatePackage (testAlertCounter, false, 30)) ;
         packageController.AddPackage (CreatePackage (testAlertCounter, false, 30)) ;
         packageController.AddPackage (CreatePackage (testAlertCounter)) ;

         Assert.AreEqual (3, packageController.PackageCount) ;
         //Assert.AreEqual (1, packageController.PackageRunners1 [0].PackageCount) ;
         //Assert.AreEqual (1, packageController.PackageRunners1 [1].PackageCount) ;
         //Assert.AreEqual (1, packageController.PackageRunners1 [2].PackageCount) ;

         Package package = CreatePackage (testAlertCounter, false, 30) ;
         packageController.AddPackage (package) ;
         Assert.AreEqual(4, packageController.PackageCount);
         //Assert.AreEqual (1, packageController.PackageRunners1.Count (x => x.PackageCount == 2)) ;
         //Assert.AreEqual (2, packageController.PackageRunners1.Count (x => x.PackageCount == 1)) ;

         packageController.Start() ;
         new Timeout (10 * 1000).WaitFor (() => testAlertCounter.AlertCount == 3) ;
         new Timeout (3 * 1000).Wait() ;
         Assert.AreEqual (3, testAlertCounter.AlertCount) ;
      }

      [Test]
      public void CreatePackageController_CreatePackage_RunChecker() {
         const int CHECK_INTERVAL_SECONDS = 300 ;

         // PackageController1 packageController = new PackageController1 (1) ;
         PackageController1 packageController = new PackageController1();

         TestAlertCounter testAlertCounter = new TestAlertCounter() ;

         Assert.AreEqual (0, packageController.PackageCount) ;
         Assert.AreEqual (0, testAlertCounter.AlertCount) ;
         
         packageController.AddPackage (CreatePackage (testAlertCounter, false, CHECK_INTERVAL_SECONDS)) ;

         Assert.AreEqual (1, packageController.PackageCount) ;
         // Assert.AreEqual (1, packageController.PackageRunners1 [0].PackageCount) ;

         packageController.Start() ;
         new Timeout (10 * 1000).WaitFor (() => testAlertCounter.AlertCount >= 1) ;

         Assert.AreEqual (1, testAlertCounter.AlertCount) ;
      }
   }
}
