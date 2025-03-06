using System ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Tests {
   public class TestAlert : Alert {
      public TestAlert (TestAlertCounter testAlertCounter) {
         TestAlertCounter = testAlertCounter ;
      }

      public readonly TestAlertCounter TestAlertCounter ;

      protected override string Caption => "TestAlert" ;

      protected override void Fire1 (Instance instance,
                                     AlertInfo alertInfo) {
         if (TestAlertCounter == null) return ;

         TestAlertCounter.AlertInfo = alertInfo ;
         TestAlertCounter.AlertCount++ ;
      }
   }
}