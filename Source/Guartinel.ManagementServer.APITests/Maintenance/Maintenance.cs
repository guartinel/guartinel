using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Guartinel.ManagementServer.APITests.Maintenance {
   [TestFixture]
   public class Maintenance : TestsBase {

      [Test]
      [Category(TestsBase.Constants.EASY_TEST_PREPARE)]
      public void CreateTesterAccounts() {
         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            CreateAccount ($"tester_account{accountIndex.ToString().PadLeft (4, '0')}@sysment.hu", Constants.LOGIN_PASSWORD) ;
         }
      }

      [Test]
      [Category(TestsBase.Constants.EASY_TEST_CLEAN_UP)]
      public void DeleteAllTesterAccounts() {
         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            DeleteAccount ($"tester_account{accountIndex.ToString().PadLeft (4, '0')}@sysment.hu", Constants.LOGIN_PASSWORD) ;
         }
      }

      [Test]
      public void PrintAllPackages() {
         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            var token = GetToken() ;
            var packages = GetAllPackages (token) ;
            Debug.WriteLine ($"Account: {accountIndex}: {packages.Count} packages.") ;
            foreach (var package in packages) {
               Debug.WriteLine ($"Account: {accountIndex}, package {package.Name}, version {package.Version}.") ;
            }
         }
      }

      [Test]
      public void DisableAllPackages() {
         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            var token = GetToken (accountIndex) ;

            DisableAllPackages (token) ;

            Debug.WriteLine ($"Packages from account {accountIndex} are disabled.") ;
         }
      }

      [Test]
      public void EnableAllPackages() {
         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            var token = GetToken (accountIndex) ;

            EnableAllPackages (token) ;

            Debug.WriteLine ($"Packages from account {accountIndex} are enabled.") ;
         }
      }

      [Test]
      public void DeleteAllPackages() {
         for (int accountIndex = 0; accountIndex < Constants.TEST_ACCOUNT_COUNT; accountIndex++) {
            var token = GetToken (accountIndex) ;

            DeleteAllPackages (token) ;

            var packages = GetAllPackages (token) ;
            Debug.WriteLine ($"Packages from account {accountIndex} are deleted.") ;
            Assert.AreEqual (0, packages.Count) ;
         }
      }
   }
}