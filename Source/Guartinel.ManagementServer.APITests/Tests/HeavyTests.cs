using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.ManagementServer.APITests.Tests {
   [TestFixture]
   public class HeavyTests : TestsBase {
      protected void AddRandomPackage (int accountIndex,
                                       int packageCount) {
         var token = GetToken (accountIndex) ;
         var originalPackages = GetAllPackages (token) ;

         var packageNameBase = Guid.NewGuid().ToString() ;
         var random = new Random() ;

         for (var packageIndex = 0; packageIndex < packageCount; packageIndex++) {
            var packageType = random.Next (0, 3) ;
            var packageName = $"{packageNameBase}{packageIndex}" ;
            var elapsed = StopwatchEx.TimeIt (() => {
               switch (packageType) {
                  case 0: {
                     SaveHostSupervisorPackage (token, packageName, new List<string> {"sysment.hu"}, new List<string> {"z1@szabo-toth.hu"}, null, 60, true) ;
                     break ;
                  }

                  case 1: {
                     SaveApplicationSupervisorPackage (token, packageName, new List<string> {"z1@szabo-toth.hu"}, null) ;
                     break ;
                  }

                  case 2: {
                     SaveComputerSupervisorPackage (token, packageName, new List<string> {"z1@szabo-toth.hu"}, null, new ComputerSupervisorCheckThresholds().CPU (10).HDD ("C", 5)) ;
                     break ;
                  }

                  case 3: {
                     SaveWebsiteSupervisorPackage (token, packageName, new List<string> {"www.sysment.hu"}, new List<string> {"z1@szabo-toth.hu"}, null, 60, true) ;
                     break ;
                  }
               }
            }) ;

            var savedIn = (Math.Round (elapsed.TotalSeconds, 2)) ;
            Debug.WriteLine ($"Package {packageIndex} in account {accountIndex} saved ({savedIn} seconds)...") ;
         }

         // Check results
         var packages = GetAllPackages (token) ;

         Assert.AreEqual (originalPackages.Count + packageCount, packages.Count) ;

         for (var packageIndex = 0; packageIndex < packageCount; packageIndex++) {
            Assert.AreEqual (1, packages.Count (x => x.Name == $"{packageNameBase}{packageIndex}")) ;
         }
      }

      [Test]
      public void AddManyNewPackages_CheckIfAdded() {
         const int PACKAGE_COUNT_PER_ACCOUNT = 100 ;
         AddRandomPackage(1, PACKAGE_COUNT_PER_ACCOUNT);

         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            AddRandomPackage (accountIndex, PACKAGE_COUNT_PER_ACCOUNT) ;
         }
      }

      [Test]
      public void AddManyNewPackagesInMultipleThreads_CheckIfAdded() {
         const int PACKAGE_COUNT_PER_ACCOUNT = 100 ;
         // const int PACKAGE_COUNT_PER_ACCOUNT = 2 ;

         var tasks = new List<Task>() ;

         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            var accountIndexLocal = accountIndex ;
            var task = new Task (() => AddRandomPackage (accountIndexLocal, PACKAGE_COUNT_PER_ACCOUNT)) ;
            tasks.Add (task) ;
            task.Start() ;
         }
         Task.WaitAll (tasks.ToArray()) ;
      }

      [Test]
      public void DisableAndEnableAllPackages_Check() {
         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            var token = GetToken (accountIndex) ;
            var packages = GetAllPackages (token) ;

            foreach (var package in packages) {
               var elapsed = StopwatchEx.TimeIt (() => {
                  SetPackageEnabled (token, package.Name, false) ;
               }) ;

               Debug.WriteLine ($"Package {package.Name} in account {accountIndex} has been disabled ({elapsed.TotalSeconds} seconds).") ;
            }

            packages = GetAllPackages (token) ;
            foreach (var package in packages) {
               var fullPackage = GetPackage (token, package.Name) ;
               Assert.AreEqual ("false", fullPackage ["is_enabled"].ToLower()) ;

               var elapsed = StopwatchEx.TimeIt (() => {
                  SetPackageEnabled (token, package.Name, true) ;
               }) ;
               Debug.WriteLine ($"Package {package.Name} in account {accountIndex} has been enabled ({elapsed.TotalSeconds} seconds).") ;
            }

            packages = GetAllPackages (token) ;
            foreach (var package in packages) {
               var fullPackage = GetPackage (token, package.Name) ;
               Assert.AreEqual ("true", fullPackage ["is_enabled"].ToLower()) ;

               Debug.WriteLine ($"Package {package.Name} in account {accountIndex} is enabled.") ;
            }
         }
      }
   }
}