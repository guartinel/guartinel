using System;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading ;

namespace Guartinel.WatcherServer {
   static class Program {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      static void Main() {
         // Thread.Sleep (10000) ;
         ServiceBase[] servicesToRun = { 
            new WatcherService()
         } ;

         ServiceBase.Run (servicesToRun) ;
      }
   }
   }
