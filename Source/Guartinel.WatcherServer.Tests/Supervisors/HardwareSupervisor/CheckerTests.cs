using System ;
using System.Collections.Generic;
using System.Linq ;
using System.Text ;
using Guartinel.Communication.Supervisors.HardwareSupervisor ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor;
using Guartinel.WatcherServer.Tests.Checkers ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Supervisors.HardwareSupervisor {
   [TestFixture]
   public class CheckerTests : CheckerTestsBase {
      protected new static class Constants {
         public const string INSTANCE_ID = "101" ;
         public const string INSTANCE_NAME = "testehune34" ;
      }

      protected override void Setup1() {
         Registration.Register() ;
      }

      public void CreateSetupRunCheck (string type,
                                       string rangeString,
                                       int a0Value,
                                       bool d1Value,
                                       bool d2Value,
                                       bool d3Value,                                       
                                       bool success,
                                       string checkInDetails) {
         // Good test
         HardwareInstanceDataChecker checker = new HardwareInstanceDataChecker() ;
         Assert.IsNotNull (checker) ;

         string configuration = $@"{{""type"":""{type}"", ""name"": ""test1"",""id"":""12345"", {rangeString}}}" ;

         var d2 = d2Value ? 1 : 0 ;
         var d3 = d3Value ? 1 : 0 ;

         //string state = $@"{{""measurement"": {{""A0"":{a0Value}, ""D1"":{d1}, ""D2"":{d2}, ""D3"":{d3}}},
         //                 ""measurement_timestamp"":""2018-02-10 12:00:31"",""package_id"":""package1"", ""instance_id"":""12345""}}" ;
         string state = $@"{{""A0"":{a0Value}, ""D2"":{d2}, ""D3"":{d3}}}" ;

         Configuration.ConfigureChecker (checker,
                                         "packageID1",
                                         HardwareSensor.CreateInstance (new ConfigurationData (configuration)),
                                         new List<InstanceData.InstanceData> {new InstanceData.InstanceData ("001", "test1", new ConfigurationData (state))},
                                         Constants.INSTANCE_ID,
                                         Constants.INSTANCE_NAME) ;

         // new Timeout (TimeSpan.FromSeconds (10)).WaitFor() ;

         CheckResult checkResult = checker.Check(null) [0] ;

         Assert.IsNotNull (checkResult) ;
         Assert.AreEqual (success ? CheckResultKind.Success : CheckResultKind.Fail, checkResult.CheckResultKind) ;
         Assert.IsNotNull (checkResult.Message) ;
         if (success) {
            Assert.IsTrue(checkResult.Message.ToJsonString().ToLowerInvariant().Contains($"MeasurementOK".ToLowerInvariant()), checkResult.Message.ToJsonString());
         } else {
            Assert.IsTrue(checkResult.Message.ToJsonString().ToLowerInvariant().Contains($"MeasurementAlert".ToLowerInvariant()), checkResult.Message.ToJsonString());
         }

         if (!string.IsNullOrEmpty (checkInDetails)) {
            Assert.IsTrue(checkResult.Details.ToJsonString().ToLowerInvariant().Contains(checkInDetails.ToLowerInvariant()),
                          checkResult.Details.ToJsonString());
         }         
      }

      [Test]
      public void CreateSetupWithErrorValue_RunCheck() {
         // Bad tests
         CreateSetupRunCheck (Strings.Hardwares.VoltageLevel.Max230V.OneChannel.FULL_TYPE,
                              @"""max_threshold"":10", 134, false, false, false, false, "10") ;
         CreateSetupRunCheck (Strings.Hardwares.VoltageLevel.Max230V.OneChannel.FULL_TYPE,
                              @"""min_threshold"":40", 34, false, false, false, false, "40") ;

         CreateSetupRunCheck (Strings.Hardwares.VoltageLevel.Max230V.ThreeChannel.FULL_TYPE,
                              @"""channel_1"":{""min_threshold"":40}",
                              134, false, false, false, false, "40") ;

         // CreateSetupRunCheck(Strings.Hardwares.CurrentLevel.Max30A.FULL_TYPE, 40, 134, true, false) ;
      }

      [Test]
      public void CreateSetupWithSuccessValue_RunCheck() {
         // Good tests
         CreateSetupRunCheck (Strings.Hardwares.VoltageLevel.Max230V.OneChannel.FULL_TYPE,
                              @"""min_threshold"":10", 134, false, false, false, true, "34V");         
         
         CreateSetupRunCheck (Strings.Hardwares.VoltageLevel.Max230V.OneChannel.FULL_TYPE,
                              @"""max_threshold"":40", 134, false, false, false, true, "34V") ;

         CreateSetupRunCheck (Strings.Hardwares.VoltageLevel.Max230V.ThreeChannel.FULL_TYPE,
                              @"""channel_1"":{""max_threshold"":40}",
                              134, false, false, false, true, "34V") ;

         // CreateSetupRunCheck(Strings.Hardwares.CurrentLevel.Max30A.FULL_TYPE, 40, 134, false, true) ;
      }
   }
}