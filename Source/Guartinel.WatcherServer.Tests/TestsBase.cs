using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Packages ;
using NUnit.Framework ;

namespace Guartinel.WatcherServer.Tests {
   public class TestsBase {
      public static class Constants {
         public const string LOGIN_USER_NAME = "guartadmin";
         // Production
         // public const string LOGIN_PASSWORD_HASH = "C632CE89A1D1DC4DFA1123157B040F4F7A1FAF38E77FAAC864C66FA79A1DF2E3F8ED4FF7D96BA23DBA10D254C6B6892A52C66AB1C39A3088B23B224A85570B2A" ;
         public const string LOGIN_PASSWORD_HASH = "6D4085805D20666B34D7219D20F57E96544971CB80D25912A04E2F7592E0B70E699A2A25E7C205415F5B1AAC6992BCB501F6A4726858196E2322028D9E5291BD" ;

         public const double MAX_DIFF = 0.01 ;
      }

      [SetUp]
      public void Setup() {
         IoC.Use.Clear() ;

         RegisterConfiguratonReader() ;         

         ApplicationSettings.Use.LogLevel = LogLevel.Debug ;
         RegisterLoggers();

         Setup1() ;
         IoC.Use.Multi.Register<Alert, NoAlert>() ;
         
         IoC.Use.Multi.Register<Package, TestPackage>(TestPackage.Constants.CREATOR_IDENTIFIERS) ;

         _watcherServer = new WatcherServerTest (false, false) ;

         //Factory.Use.UnregisterCreators (typeof (IMailAlertSender)) ;
         //Factory.Use.UnregisterCreators (typeof (IDeviceAlertSender)) ;
         //Factory.Use.UnregisterCreators (typeof (IMeasuredDataStore)) ;
         //Factory.Use.UnregisterCreators (typeof (IManagementServerAdmin)) ;
         //Factory.Use.UnregisterCreators (typeof (IManagementServerPackages)) ;
         //Factory.Use.UnregisterCreators (typeof (IManagementServer)) ;         

         // System.Diagnostics.Debug.WriteLine ($"Threadpool available before: {Kernel.Utility.General.AvailableWorkerThreadsInPool}") ;
      }

      protected virtual void RegisterConfiguratonReader() {
         IoC.Use.Single.Register<IApplicationSettingsReader>(() => new ApplicationSettingsTestReader());
      }

      [TearDown]
      public void TearDown() {
         TearDown1() ;

         _watcherServer?.Stop();         
         _watcherServer?.Dispose() ;

         // Factory.Use.Clear();
         IoC.Use.Clear() ;
         Logger.UnregisterLoggers() ;

         // System.Diagnostics.Debug.WriteLine($"Threadpool available after: {Kernel.Utility.General.AvailableWorkerThreadsInPool}");
      }

      protected virtual void RegisterLoggers() {
         Logger.Setup<NullLogger> ("GuartinelWatcherTest", "Guartinel Watcher Test") ;
         Logger.Setup<SimpleConsoleLogger> ("GuartinelWatcherTest", "Guartinel Watcher Test") ;

         // Logger.RegisterLogger (() => new ConsoleLogger (nameof (Timeout))) ;
         // Logger.Setup<ConsoleLogger>(new LoggerSettings ("GuartinelWatcherTest", "Guartinel Watcher Test"));
         // Logger.Setup (new LoggerSettings("GuartinelWatcherTest", "Guartinel Watcher Test"));
         // Logger.RegisterLogger (() => new FileLogger ("test1", "289E9A45-FFC8-43C6-9FA1-E47C586D4EEA")) ;         
      }

      public ManagementServerMock ManagementServer => _watcherServer.ManagementServer ;

      protected WatcherServerTest _watcherServer ;

      protected virtual void Setup1() {}

      protected virtual void TearDown1() {}
   }
}
