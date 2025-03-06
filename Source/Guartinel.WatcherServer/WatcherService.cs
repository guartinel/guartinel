using System;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.WatcherServer {
   public partial class WatcherService : ServiceBase {
      public WatcherService() {
         InitializeComponent() ;

         IoC.Use.Single.Register<IApplicationSettingsReader> (() => new ApplicationSettingsReaderGlobal(nameof (WatcherServer))) ;

         // this.EventLog.WriteEntry("Test from Watcher Service: InitializeComponent() done.");

         Logger.Setup<SimpleFileLogger, SimpleWindowsEventLogger> ("GuartinelWatcher", "Guartinel Watcher Server") ;
         // Logger.RegisterLogger (() => new FileLogger ("XTESTS", "XTESTS")) ;         

         // Logger.RegisterLogger (() => new FileLogger (nameof (Core.Timeout), nameof (Core.Timeout))) ;
      }

      protected WatcherServer _watcherServer ;

      protected void StartServer() {
         try {
            // Configuration.Reset (Core.Utility.Network.GetLocalFixIPv4Address()) ;
            // Configuration.Port = "80" ;

            _watcherServer.Start() ;
         } catch (Exception e) {
            // Write error to event log
            Logger.Error (e.GetAllMessages()) ;
         }
      }

      protected override void OnStart (string[] args) {
         try {
            // Stop, waiting for debugging
#if DEBUG && ATTACH_DEBUGGER
            Core.Timeout timeout2 = new Core.Timeout (30 * 1000) ;
            while ((!Debugger.IsAttached) && (!timeout2.RunnedOut)) {
               Thread.Sleep (500) ;
            }
#endif
            
            _watcherServer = new WatcherServer() ;
            var thread = new Thread (StartServer) ;
            Thread.Sleep (2000) ;
            thread.Start() ;

         } catch (Exception e) {
            // Write error to event log
            Logger.Error (e.GetAllMessages()) ;
         }
      }

      protected override void OnStop() {
         if (_watcherServer == null) return ;

         try {
            _watcherServer.Stop() ;
         } catch (Exception e) {
            // Write error to event log
            Logger.Error (e.GetAllMessages ()) ;
         }
      }
   }
}