using System;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Linq;
using System.Text;
using System.Threading.Tasks ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.WatcherServer {
   public class WindowsEventLogger : ILogger {
      public WindowsEventLogger() : this (null) {}

      public WindowsEventLogger (List<string> categories) {
         _categories = categories ;
      }

      private readonly List<string> _categories;
      public List<string> Categories => _categories;

      public void Log (LogLevel level,
                       string message,
                       List<string> categories) {
         // Log only the errors
         if (level != LogLevel.Error) return ;

         new Task (() => {
            try {
               WriteErrorEventLog ($"Error in {Logger.Settings.LongName}. Message: {message}") ;
            } catch {
               // Ignore error
            }
         }).Start() ;
      }

      private static void WriteErrorEventLog (string message) {
         if (!EventLog.SourceExists (Logger.Settings.LongName)) {
            //An event log source should not be created and immediately used.
            //There is a latency time to enable the source, it should be created
            //prior to executing the application that uses the source.
            //Execute this sample a second time to use the new source.
            EventLog.CreateEventSource (Logger.Settings.LongName, "Application") ;
            // The source is created.  Exit the application to allow it to be registered.
            new Kernel.Timeout (30 * 1000).WaitFor (() => EventLog.SourceExists (Logger.Settings.LongName)) ;
         }

         // Create an EventLog instance and assign its source.
         EventLog eventLog = new EventLog() ;
         eventLog.Source = Logger.Settings.LongName ;

         // Write an informational entry to the event log.    
         eventLog.WriteEntry (message, EventLogEntryType.Error) ;
      }
   }

   public class SimpleWindowsEventLogger : WindowsEventLogger {
      public SimpleWindowsEventLogger() { }
   }
}