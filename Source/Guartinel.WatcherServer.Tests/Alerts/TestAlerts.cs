using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel;
using Guartinel.Kernel.Entities ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Instances ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Alerts {
   [TestFixture]
   public class TestAlerts : TestsBase {
      public class TestAlert1 : Alert {
         protected override string Caption => "TestAlert1" ;

         protected override void Fire1 (Instance instance,
                                    AlertInfo alertInfo) {
            Fired = true ;
         }

         public bool Fired {get ; set ;}
      }

      protected override void Setup1() {
         IoC.Use.Multi.Register<Alert, TestAlert1>() ;
      }

      protected override void TearDown1() {
         // Factory.Use.UnregisterCreators (typeof(TestAlert1)) ;
      }

      [Test]
      public void TestSimpleAlert() {
         TestAlert1 alert = new TestAlert1() ;
         string packageID = "111" ;
         alert.Configure ("Test1", packageID, 0) ;
         alert.Enabled = true ;
         Assert.IsFalse (alert.Fired) ;
         alert.Request (new Instance (Guid.NewGuid().ToString()),
                        new AlertInfo().Configure (new CheckResult().Configure ("Result1",
                                                                                CheckResultKind.Fail,
                                                                                new XSimpleString(alert.Name), null, null),
                                                   new XSimpleString (alert.Name),
                                                   null,
                                                   null,
                                                   packageID, "222", AlertKind.Alert, true),
                        null) ;
         new Kernel.Timeout (2000).WaitFor (() => alert.Fired) ;
         Assert.IsTrue (alert.Fired) ;
      }
   }
}