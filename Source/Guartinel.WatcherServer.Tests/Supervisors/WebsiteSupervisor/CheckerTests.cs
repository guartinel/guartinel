using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Service.WebsiteChecker ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Supervisors.WebsiteSupervisor ;
using Guartinel.WatcherServer.Tests.Checkers;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Supervisors.WebsiteSupervisor {

   [TestFixture]
   public class CheckerTests : CheckerTestsBase {
      [Test]
      public void SetupValidAddress_CheckIfLoaded() {
         // Good test
         WebsiteChecker checker = new WebsiteChecker() ;
         Assert.IsNotNull (checker) ;

         var website = new Website (@"http://index.hu") ;

         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(), website,
                                         checkLoadTimeSeconds:20,
                                         siteDownloadResults: new List<SiteDownloadResult>() {
                                                  new SiteDownloadResult (website, SiteDownloadResultSuccess.Success, 1200, "", "OK", "OK!", null)
                                         }) ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Success, checkResult.CheckResultKind) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteIsOKMessage"), checkResult.Message.ToJsonString()) ;
      }

      [Test]
      public void SetupWrongAddress_CheckIfNotLoaded() {
         // Good test
         WebsiteChecker checker = new WebsiteChecker();
         Assert.IsNotNull (checker) ;

         var website = new Website (@"http://XehuneX.com") ;
         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(), website,
                                         new List<SiteDownloadResult>() {
                                                  new SiteDownloadResult (website, SiteDownloadResultSuccess.Error, 1200, "", "NotOK", "NotOK!", null)
                                         }) ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsFalse (string.IsNullOrEmpty (checkResult.Message.ToJsonString())) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteCheckErrorMessage"), checkResult.Message.ToString()) ;
      }

      [Test]
      public void SetupInvalidServer_CheckIfNotLoaded() {
         // Good test
         WebsiteChecker checker = new WebsiteChecker();
         Assert.IsNotNull (checker) ;

         var website = new Website ("http://111.111.111.111") ;
         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(),
                                         website,
                                         new List<SiteDownloadResult>() {
                                                  new SiteDownloadResult (website, SiteDownloadResultSuccess.Error, 1200, "", "NotOK", "NotOK!", null)
                                         }) ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsFalse (string.IsNullOrEmpty (checkResult.Message.ToJsonString())) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("WEBSITE_SUPERVISOR.WebsiteCheckErrorMessage"), checkResult.Message.ToString()) ;
      }

      [Test]
      public void SetupUnparseableHost_CheckError() {
         // Good test
         WebsiteChecker checker = new WebsiteChecker() ;
         Assert.IsNotNull (checker) ;

         var website = new Website ("sima gyokerseg") ;
         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(), website,
                                         new List<SiteDownloadResult>() {
                                                  new SiteDownloadResult (website, SiteDownloadResultSuccess.Error, 1200, "", "NotOK sima gyokerseg", "NotOK!", null)
                                         },
                                         checkLoadTimeSeconds: 20) ;

         CheckResult checkResult = checker.Check (null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsFalse (string.IsNullOrEmpty (checkResult.Message?.ToJsonString())) ;
         Assert.IsTrue (checkResult.Message?.ToJsonString().Contains ("sima gyokerseg"), $"'sima gyokerseg' is missing from {checkResult.Message}") ;
      }
   }
}