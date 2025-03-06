using System ;
using System.Linq ;
using System.Text;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Alerts ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests.Packages.Factories {
   [TestFixture]
   public class TestFactories : TestsBase {
      [Test]
      public void CheckRegistations() {
         Assert.IsTrue (IoC.Use.Multi.ImplementationExists<Alert>()) ;
         var alerts = IoC.Use.Multi.GetInstances<Alert>().ToList() ;

         Assert.Greater (alerts.Count(), 0) ;
         // Assert.AreEqual (1, alerts.Count (x => x.Caption == NoAlert.Constants.CAPTION)) ;
         Assert.AreEqual(1, alerts.Count(x => x.GetType().Name == nameof (NoAlert))) ;
      }
  }
}