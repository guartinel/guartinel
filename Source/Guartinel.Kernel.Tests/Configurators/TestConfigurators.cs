using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel.Tests.Configurators {
   //[TestFixture]
   //public class TestConfigurators : TestXmls {
   //   public static Controller CreateTestConfigurator() {
   //      Controller configurator = new Controller() ;
   //      var checker1 = new Checker().Configure ("Checker1", true) ;
   //      var checker2 = new Checker().Configure ("Checker2", true) ;
   //      var checker3 = new Checker().Configure ("Checker3", true) ;
   //      var alert1 = new Alert().Configure ("Alert1") ;
   //      var alert2 = new Alert().Configure ("Alert2") ;
   //      var alertTrigger1 = new Watcher().Configure ("test@test.hu", "Watcher1",
   //                                                   new List<Checker> {checker1, checker2},
   //                                                   new List<Alert> {alert1},
   //                                                   "AlertText1") ;
   //      var alertTrigger2 = new Watcher().Configure ("test@test.hu", "Watcher1",
   //                                                   new List<Checker> {checker3},
   //                                                   new List<Alert> {alert2},
   //                                                   "AlertText2") ;

   //      configurator.AddWatcher (alertTrigger1) ;
   //      configurator.AddWatcher (alertTrigger2) ;

   //      Assert.AreEqual (2, configurator.Watchers.Count) ;
   //      Assert.AreEqual (2, configurator.Watchers [0].Checkers.Count) ;
   //      Assert.AreEqual (1, configurator.Watchers [0].Alerts.Count) ;
   //      Assert.AreEqual (1, configurator.Watchers [1].Checkers.Count) ;
   //      Assert.AreEqual (1, configurator.Watchers [1].Alerts.Count) ;

   //      Assert.AreSame (checker1, configurator.Watchers [0].Checkers [0]) ;
   //      Assert.AreSame (checker2, configurator.Watchers [0].Checkers [1]) ;
   //      Assert.AreSame (checker3, configurator.Watchers [1].Checkers [0]) ;

   //      return configurator ;
   //   }

   //   [Test]
   //   public void TestSaveAndLoadXml() {
   //      var configurator = CreateTestConfigurator() ;
   //      var node = CreateNode ("Root") ;
   //      configurator.SaveToNode (node) ;

   //      //if (node.OwnerDocument != null) {
   //      //   node.OwnerDocument.Save (@"c:\temp\x.xml") ;
   //      //}         

   //      // Load back and check
   //      var loadedConfigurator = new Controller() ;
   //      loadedConfigurator.LoadFromNode (node) ;

   //      Assert.AreEqual (2, loadedConfigurator.Watchers.Count) ;
   //      Assert.AreEqual (2, loadedConfigurator.Watchers [0].Checkers.Count) ;
   //      Assert.AreEqual (1, loadedConfigurator.Watchers [0].Alerts.Count) ;
   //      Assert.AreEqual (1, loadedConfigurator.Watchers [1].Checkers.Count) ;
   //      Assert.AreEqual (1, loadedConfigurator.Watchers [1].Alerts.Count) ;

   //      Assert.AreEqual (configurator.Watchers [0].Checkers [0].ID, loadedConfigurator.Watchers [0].Checkers [0].ID) ;
   //      Assert.AreEqual (configurator.Watchers [0].Checkers [0].Name, loadedConfigurator.Watchers [0].Checkers [0].Name) ;
   //      Assert.AreEqual (configurator.Watchers [0].Checkers [1].ID, loadedConfigurator.Watchers [0].Checkers [1].ID) ;
   //      Assert.AreEqual (configurator.Watchers [0].Checkers [1].Name, loadedConfigurator.Watchers [0].Checkers [1].Name) ;
   //      Assert.AreEqual (configurator.Watchers [0].Alerts [0].ID, loadedConfigurator.Watchers [0].Alerts [0].ID) ;
   //      Assert.AreEqual (configurator.Watchers [1].Checkers [0].ID, loadedConfigurator.Watchers [1].Checkers [0].ID) ;
   //      Assert.AreEqual (configurator.Watchers [1].Checkers [0].Name, loadedConfigurator.Watchers [1].Checkers [0].Name) ;
   //      Assert.AreEqual (configurator.Watchers [1].Alerts [0].ID, loadedConfigurator.Watchers [1].Alerts [0].ID) ;
   //   }
   //}
}