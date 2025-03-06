using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Tests ;
using Guartinel.WatcherServer.Checkers.Network ;
using Guartinel.WatcherServer.CheckResults ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Checkers {

   [TestFixture]
   public class PingCheckerTests : CheckerTestsBase {
      [Test]
      public void CreateAndStartPingCheckerLocalhost() {
         // Good test
         PingChecker pingCheckerGood = new PingChecker().Configure ("Checker1", "packageID1", "127.0.0.1") ;
         Assert.IsNotNull (pingCheckerGood) ;

         var result = pingCheckerGood.Check(null)[0] ; 
         Assert.AreEqual (CheckResultKind.Success, result.CheckResultKind) ;

         // Test with wrong address
         PingChecker pingCheckerWrong = new PingChecker().Configure ("Checker1", "packageID1", "129.0.1.111") ;
         Assert.IsNotNull (pingCheckerWrong) ;

         var result2 = pingCheckerWrong.Check(null)[0] ;
         Assert.AreEqual (CheckResultKind.Fail, result2.CheckResultKind) ;
      }
   }
}