using System ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using Guartinel.Kernel.Tests ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Checkers.Network ;
using Guartinel.WatcherServer.CheckResults ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Checkers {
   [TestFixture]
   public class CheckersTests : TestsBase {
      [Test]
      public void TestCheckResultClone() {
         var timeStamp = DateTime.UtcNow ;
         Thread.Sleep (100) ;

         CheckResult checkResult = new CheckResult().Configure ("Result1", CheckResultKind.Success, null, null, null, timeStamp) ;
         Assert.AreEqual (checkResult.Name, "Result1") ;
         Assert.AreEqual ("Success", EnumEx.GetName (typeof (CheckResultKind), checkResult.CheckResultKind)) ;
         AssertEx.AreEqualToMilliSeconds (timeStamp, checkResult.TimeStamp) ;

         // Check clone
         CheckResult clonedCheckResult = (CheckResult) checkResult.Duplicate() ;
         Assert.AreNotEqual (checkResult, clonedCheckResult) ;
         Assert.AreEqual (checkResult.Name, clonedCheckResult.Name) ;
         Assert.AreEqual (checkResult.CheckResultKind, clonedCheckResult.CheckResultKind) ;
         Assert.AreEqual (checkResult.TimeStamp, clonedCheckResult.TimeStamp) ;

         // Check if clone is independent
         checkResult.Name = "Result1a" ;
         Assert.AreEqual ("Result1", clonedCheckResult.Name) ;
      }

      [Test]
      public void TestCheckerClone() {
         PingChecker checker = new PingChecker().Configure ("Checker1", "PackageID1", "localhost") ;
         var clonedChecker = checker.Duplicate().CastTo<PingChecker>() ;
         clonedChecker.Configure ("Checker1Clone", "PackageID2", "remotehost") ;

         Assert.AreNotEqual (checker, clonedChecker) ;
         Assert.AreNotEqual (clonedChecker.Name, checker.Name) ;
         Assert.AreNotEqual (clonedChecker.PackageID, checker.PackageID) ;
         Assert.AreNotEqual (clonedChecker.InstanceID, checker.InstanceID) ;
         Assert.AreNotEqual (clonedChecker.Address, checker.Address) ;
      }
   }
}
