using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel.Utility ;
using Guartinel.Service.WebsiteChecker ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Supervisors.WebsiteSupervisor {

   // [TestFixture]
   public class HeavyPackageTests : PackageTestsBase {
      // [Test]
      public void CreateSetup_AddLotOfPackages() {
         const int PACKAGE_COUNT = 100 ;

         StartServer() ;

         var token = Login() ;

         Assert.AreEqual (0, _watcherServer.PackageController.PackageCount) ;

         for (int packageIndex = 0; packageIndex < PACKAGE_COUNT; packageIndex++) {
            int packageIndexLocal = packageIndex ;

            var websites = new List<Website> {new Website ("http://www.sysment.hu")} ;

            var elapsed = StopwatchEx.TimeIt (() => SavePackage (token, DateTime.UtcNow,
                                                                 x => Configuration.CreatePackageConfiguration (x, websites),
                                                                 "packageName" + packageIndexLocal,
                                                                 Guid.NewGuid().ToString(),
                                                                 false,
                                                                 1000, 1500, 1000,
                                                                 false)) ;

            Assert.LessOrEqual (elapsed.Seconds, 2, $"Sending took {elapsed.TotalSeconds} secs at {packageIndex + 1} of {PACKAGE_COUNT}.") ;
         }

         // Login again to make sure that server is responsive
         // var elapsed1 = StopwatchEx.TimeIt (() => Login()) ;
         // Assert.LessOrEqual (elapsed1.Seconds, 1, $"Login took {elapsed1.Seconds} secs.") ;
         new Kernel.Timeout (20000 + 1000 * PACKAGE_COUNT).WaitFor (() => {
            Debug.WriteLine ($"Package count is {_watcherServer.PackageController.PackageCount} of {PACKAGE_COUNT}.") ;
            Thread.Sleep (1000) ;
            return _watcherServer.PackageController.PackageCount >= PACKAGE_COUNT ;
         }) ;

         Assert.GreaterOrEqual (_watcherServer.PackageController.PackageCount, PACKAGE_COUNT) ;
      }

      // [Test]
      public void CreateSetup_AddPackages_RunChecks() {
         const int PACKAGE_COUNT = 100 ;

         StartServer() ;

         var token = Login() ;

         for (int packageIndex = 0; packageIndex < PACKAGE_COUNT; packageIndex++) {
            int packageIndexLocal = packageIndex ;
            var websites = new List<Website> { new Website("http://www.sysment.hu") };
            var elapsed = StopwatchEx.TimeIt (() => SavePackage (token, DateTime.UtcNow,
                                                                 x => Configuration.CreatePackageConfiguration (x, websites),
                                                                 "packageName" + packageIndexLocal, Guid.NewGuid().ToString(), false,
                                                                 1000, 1500, 1, false)) ;

            Debug.WriteLine ($"Sending took {elapsed.Seconds} secs at {packageIndex + 1} of {PACKAGE_COUNT}.") ;
            Assert.LessOrEqual (elapsed.Seconds, 2, $"Sending took {elapsed.Seconds} secs at {packageIndex + 1} of {PACKAGE_COUNT}.") ;
         }

         new Kernel.Timeout (20000 + 1000 * PACKAGE_COUNT).WaitFor (() => _watcherServer.PackageController.PackageCount >= PACKAGE_COUNT) ;
         Assert.GreaterOrEqual (_watcherServer.PackageController.PackageCount, PACKAGE_COUNT) ;

         // Login again to make sure that server is responsive
         // var elapsed1 = StopwatchEx.TimeIt (() => Login()) ;
         // Assert.LessOrEqual (elapsed1.Seconds, 1, $"Login took {elapsed1.Seconds} secs.") ;

         // Wait a bit
         new Kernel.Timeout (20000 + 1000 * PACKAGE_COUNT).WaitFor (() => {
            Debug.WriteLine ($"Measurements: {ManagementServer.MeasuredDataList.Count}.") ;
            Thread.Sleep (1000) ;
            return (ManagementServer.MeasuredDataList.Count >= PACKAGE_COUNT) ;
         }) ;
         Assert.GreaterOrEqual (ManagementServer.MeasuredDataList.Count, PACKAGE_COUNT) ;

         // Login again to make sure that server is responsive
         // elapsed1 = StopwatchEx.TimeIt (() => Login()) ;
         // Assert.LessOrEqual (elapsed1.Seconds, 1, $"Login took {elapsed1.Seconds} secs.") ;
      }
   }
}