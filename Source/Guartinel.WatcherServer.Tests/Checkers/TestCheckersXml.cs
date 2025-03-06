using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.WatcherServer.Tests.Checkers {
   //[TestFixture]
   //public class TestCheckersXml : TestXmls {
   //   [Test]
   //   public void TestCheckResultSaveLoad() {
   //      var node = CreateNode ("CheckResult") ;
   //      var timeStamp = DateTime.UtcNow ;

   //      CheckResult checkResult = new CheckResult().Configure ("Result1", CheckResultSuccess.Success, timeStamp) ;
   //      checkResult.SaveToNode (node) ;

   //      CheckResult loadedCheckResult = new CheckResult() ;
   //      loadedCheckResult.LoadFromNode (node) ;

   //      Assert.AreNotEqual (checkResult, loadedCheckResult) ;
   //      Assert.AreEqual (checkResult.Name, loadedCheckResult.Name) ;
   //      Assert.AreEqual (EnumEx.GetName (typeof (CheckResultSuccess), checkResult.Success),
   //                       EnumEx.GetName (typeof (CheckResultSuccess), loadedCheckResult.Success)) ;
   //      AssertEx.AreEqualToMilliSeconds (checkResult.TimeStamp, loadedCheckResult.TimeStamp) ;
   //   }

   //   [Test]
   //   public void TestCheckerSaveLoad() {
   //      var node = CreateNode ("Checker") ;

   //      Checker checker = new Checker().Configure ("Checker1", true) ;
   //      checker.SaveToNode (node) ;

   //      Checker loadedChecker = new Checker() ;
   //      loadedChecker.LoadFromNode (node) ;

   //      Assert.AreNotEqual (checker, loadedChecker) ;
   //      Assert.AreEqual (checker.ID, loadedChecker.ID) ;
   //      Assert.AreEqual (checker.Name, loadedChecker.Name) ;
   //      //Assert.AreEqual (checker.CheckInterval, loadedChecker.CheckInterval) ;
   //      //Assert.AreEqual (checker.StartupDelay, loadedChecker.StartupDelay) ;
   //   }
   //}
}