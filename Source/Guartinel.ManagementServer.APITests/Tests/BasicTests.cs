using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.ManagementServer.APITests.Tests {
   [TestFixture]
   public class BasicTests : TestsBase {
      [Test]
      [Category(TestsBase.Constants.EASY_TEST_CATEGORY)]
      public void GetToken_GetAllPackages() {
         var token = GetToken() ;
         var packages = GetAllPackages (token) ;
      }

      public void AddNewPackageAndCheck (Action<string, string> savePackage) {
         var token = GetToken() ;

         var originalPackages = GetAllPackages (token) ;

         var packageName = Guid.NewGuid().ToString() ;
         savePackage (token, packageName) ;
         var packages = GetAllPackages (token) ;

         Assert.AreEqual (originalPackages.Count + 1, packages.Count) ;
         Assert.AreEqual (packageName, packages [packages.Count - 1].Name) ;
      }
    
      [Test]
      public void AddNewWebsitePackage_CheckIfAdded() {
         AddNewPackageAndCheck ((token, packageName) => {
            SaveWebsiteSupervisorPackage (token, packageName, new List<string> {"www.sysment.hu"}, new List<string> {"z1@szabo-toth.hu"}, null) ;
         }) ;
      }

      [Test]
      public void AddNewHostPackage_CheckIfAdded() {
         AddNewPackageAndCheck ((token, packageName) => {
            SaveHostSupervisorPackage (token, packageName, new List<string> {"sysment.hu"}, new List<string> {"z1@szabo-toth.hu"}, null) ;
         }) ;
      }

      [Test]
      public void AddNewApplicationPackage_CheckIfAdded() {
         AddNewPackageAndCheck ((token, packageName) => {
            SaveApplicationSupervisorPackage (token, packageName, new List<string> {"z1@szabo-toth.hu"}, null) ;
         }) ;
      }
   
      [Test]
      public void CreatePackage_UpdatePackage_CheckIfVersionIncreased() {
         var token = GetToken() ;

         var packageName = Guid.NewGuid().ToString() ;
         SaveHostSupervisorPackage (token, packageName, new List<string> {"sysment.hu"}, new List<string> {"z1@szabo-toth.hu"}, null) ;
         var originalVersion = GetPackageVersion (token, packageName) ;

         SaveHostSupervisorPackage (token, packageName, new List<string> {"sysment.hu"}, new List<string> {"z1@szabo-toth.hu"}, null, 600, true) ;

         var newVersion = GetPackageVersion (token, packageName) ;

         Assert.AreEqual (originalVersion, 0) ;
         Assert.AreEqual (originalVersion + 1, newVersion) ;
         //Assert.AreEqual(packageName, packages[packages.Count - 1].Name);
      }

      [Test]
      public void CreatePackage_GetPackage_CheckIfSame() {
         var token = GetToken() ;

         var packageName = Guid.NewGuid().ToString() ;
         var hosts = new List<string> {"guartinel.com", "sysment.hu"} ;
         var alertMails = new List<string> {"z1@szabo-toth.hu", "z2@szabo-toth.hu"} ;
         SaveHostSupervisorPackage (token, packageName, hosts, alertMails, null, 500, true) ;

         var package = GetPackage (token, packageName) ;
         Assert.AreEqual (13, package.AsJObject.Count) ;
         Assert.AreEqual (packageName, package ["package_name"]) ;
         Assert.AreEqual (500, package.AsInteger ("check_interval_seconds")) ;
         Assert.AreEqual (true, package.AsBoolean ("is_enabled")) ;
         Assert.AreEqual (alertMails.Concat ("|"), package.AsStringArray ("alert_emails").Concat ("|")) ;

         var configuration = package.GetChild ("configuration") ;
         Assert.AreEqual (hosts.Concat ("|"), configuration.AsStringArray ("hosts").Concat ("|")) ;
         //Assert.AreEqual(packageName, packages[packages.Count - 1].Name);
      }


      [Test]
      public void CreatePackageEnabled_Disable_Check_Enable_Check() {
         var token = GetToken() ;
         var packageName = Guid.NewGuid().ToString() ;
         SaveHostSupervisorPackage (token, packageName, new List<string> {"sysment.hu"}, new List<string> {"zoltan.szabototh@sysment.hu"}, null, 300, true) ;
         var package = GetPackage (token, packageName) ;
         int version1 = package.AsInteger ("version") ;
         Assert.AreEqual ("true", package ["is_enabled"].ToLowerInvariant()) ;
         SetPackageEnabled (token, packageName, false) ;
         package = GetPackage (token, packageName) ;
         int version2 = package.AsInteger ("version") ;
         Assert.AreEqual (version1, version2) ;

         Assert.AreEqual ("false", package ["is_enabled"].ToLowerInvariant()) ;
         SetPackageEnabled (token, packageName, true) ;
         package = GetPackage (token, packageName) ;
         int version3 = package.AsInteger ("version") ;
         Assert.AreEqual (version1, version3) ;
         Assert.AreEqual ("true", package ["is_enabled"].ToLowerInvariant()) ;
         DeletePackage (token, packageName) ;
      }

      [Test]
      public void DeleteAllPackages_Check() {
         var token = GetToken() ;

         DeleteAllPackages (token) ;

         var packages = GetAllPackages (token) ;
         Assert.AreEqual (0, packages.Count) ;
      }
   }
}