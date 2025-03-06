using System ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Network ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Supervisors.HostSupervisor ;
using Guartinel.WatcherServer.Tests.Checkers ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Supervisors.HostSupervisor {

   [TestFixture]
   public class CheckerTests : CheckerTestsBase {
      [Test]
      public void SetupGoogleDotCom_CheckIfPinged() {
         // Good test
         HostChecker checker = new HostChecker (IoC.Use.Single.GetInstance<IMeasuredDataStore>());
         Assert.IsNotNull (checker) ;

         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(), new Host ("google.com")) ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Success, checkResult.CheckResultKind) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("HOST_SUPERVISOR.HostIsOK"), checkResult.Message.ToJsonString()) ;
      }

      [Test]
      public void SetupWrongAddress_CheckIfPinged() {
         // Good test
         HostChecker checker = new HostChecker (IoC.Use.Single.GetInstance<IMeasuredDataStore>());
         Assert.IsNotNull (checker) ;

         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(), new Host ("XehuneX.com"), 2, 1) ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsFalse (checkResult.Message.IsEmpty) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("HOST_SUPERVISOR.HostIsNotAvailableAlertMessage"), checkResult.Message.ToJsonString()) ;
         Assert.IsTrue(checkResult.Details.ToJsonString().Contains("HOST_SUPERVISOR.HostIsNotAvailableAlertDetails"), checkResult.Details.ToJsonString());
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("XehuneX.com"), checkResult.Message.ToJsonString()) ;
         Assert.IsFalse (checkResult.Details.ToJsonString().ToLowerInvariant().Contains ("tracing"), checkResult.Details.ToJsonString()) ;
         Assert.IsFalse(checkResult.Details.ToJsonString().ToLowerInvariant().Contains("trace error"), checkResult.Details.ToJsonString());
         Assert.IsFalse(checkResult.Details.ToJsonString().ToLowerInvariant().Contains("exception"), checkResult.Details.ToJsonString());
      }

      [Test]
      public void SetupRightAddressButNoPing_CheckIfPinged() {
         // Good test
         HostChecker checker = new HostChecker (IoC.Use.Single.GetInstance<IMeasuredDataStore>()) ;
         Assert.IsNotNull (checker) ;

         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(), new Host ("backend2.guartinel.com"), 2, 1) ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsFalse (checkResult.Message.IsEmpty) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("HOST_SUPERVISOR.HostIsNotAvailableAlert"), checkResult.Message.ToJsonString()) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("backend2.guartinel.com"), checkResult.Message.ToJsonString()) ;
         Debug.WriteLine (checkResult.Details.ToJsonString()) ;
         Assert.IsTrue (checkResult.Details.ToJsonString().ToLowerInvariant().Contains ("tracing"),
                        checkResult.Details.ToJsonString()) ;
      }

      [Test]
      public void SetupNoPingServer_CheckIfError() {
         // Good test
         HostChecker checker = new HostChecker (IoC.Use.Single.GetInstance<IMeasuredDataStore>());
         Assert.IsNotNull (checker) ;

         Configuration.ConfigureChecker (checker, Guid.NewGuid().ToString(), new Host("111.111.111.111"), 2, 1) ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsFalse (checkResult.Message.IsEmpty) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains ("HOST_SUPERVISOR.HostIsNotAvailable"), checkResult.Message.ToJsonString()) ;
         Assert.IsTrue (checkResult.Message.ToJsonString().Contains (@"111.111.111.111"), checkResult.Message.ToJsonString()) ;
      }
   }
}