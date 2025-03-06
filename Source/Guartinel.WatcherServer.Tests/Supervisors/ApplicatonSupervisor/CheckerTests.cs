using System ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Supervisors.ApplicationSupervisor ;
using Guartinel.WatcherServer.Tests.Checkers ;
using NUnit.Framework ;

using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.InstanceData ;

namespace Guartinel.WatcherServer.Tests.Supervisors.ApplicatonSupervisor {
[TestFixture]
   public class CheckerTests : CheckerTestsBase {
      [Test]
      public void CreateSetupWithAlertedSuccessFromApplication() {
         // Good test
         ApplicationInstanceDataChecker checker = new ApplicationInstanceDataChecker() ;
         Assert.IsNotNull (checker) ;

         var parameters = new Parameters (Configuration.CreateCheckResult (CheckResultKind.Fail, "There is a big problem!")) ;

         Configuration.ConfigureChecker (checker,
                                         "packageID1",
                                         new ApplicationInstanceDataLists (new InstanceDataLists ("101", "instance1", parameters)),
                                         "101",
                                         "instance1") ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsNotNull (checkResult.Message) ;
         Assert.IsTrue (checkResult.Details.ToJsonString().ToLowerInvariant().Contains ("big problem")) ;
      }

      [Test]
      public void CreateSetupWithSuccessfullSuccessFromApplication() {
         // Good test
         ApplicationInstanceDataChecker checker = new ApplicationInstanceDataChecker() ;
         Assert.IsNotNull (checker) ;

         var parameters = new Parameters (Configuration.CreateCheckResult (CheckResultKind.Success)) ;

         Configuration.ConfigureChecker (checker,
                                         "packageID1",
                                         new ApplicationInstanceDataLists (new InstanceDataLists ("101", "instance1", parameters)),
                                         "101",
                                         "instance1") ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (CheckResultKind.Success, checkResult.CheckResultKind) ;
         Assert.IsNotNull (checkResult.Message) ;
        
      }
   }
}
