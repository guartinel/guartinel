using System;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace Guartinel.Website.Common.Tools {
   public class _LogWrapper {
      private static bool isDebugLogEnabled = true;

      protected class LogEntry {
         //  private string _programVersion = GuartinelApp.VERSION_NUMBER; //TODO
         private string _message;
         private Exception _exception;

         public LogEntry(string message,
               Exception exception = null) {
            _message = message;
            _exception = exception;
            }
         public new string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.Append(string.Format("V: {0} ", _programVersion));
            stringBuilder.Append(String.Format("MSG: {0} ", _message));
            if (_exception != null) {
               stringBuilder.Append(String.Format("EXCEPTION: {0} \n \tEXCEPTION_DATA: {1} \n \tEXCEPTION_STACK_TRACE: {2} \n \tINNER_EXCEPTION {3}", _exception.Message, _exception.Data, _exception.StackTrace, _exception.InnerException));
               }
            return stringBuilder.ToString();
            }
         }

      public static void SetDebugLogEnabled(bool enabled) {
         isDebugLogEnabled = enabled;
         }

      private static ILog _log;

      static _LogWrapper() {
         _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
         PatternLayout patternLayout = new PatternLayout();
         patternLayout.ConversionPattern = "%date [%thread] %-5level - %message%newline";
         patternLayout.ActivateOptions();

         RollingFileAppender roller = new RollingFileAppender();
         roller.AppendToFile = true;
         roller.File = @"logs\EventLog.txt";
         roller.Layout = patternLayout;
         roller.MaxSizeRollBackups = 10;
         roller.MaximumFileSize = "10MB";
         roller.RollingStyle = RollingFileAppender.RollingMode.Size;
         roller.StaticLogFileName = true;
         roller.LockingModel = new FileAppender.MinimalLock();
         roller.ActivateOptions();
         BasicConfigurator.Configure(roller);
         }

      public static void Debug(string message) {
         if (!isDebugLogEnabled) {
            return;
            }
         _log.Debug(new LogEntry(message).ToString());
         }

      public static void Error(string message) {
         _log.Error(new LogEntry(message).ToString());
         }

      public static void Error(Exception e,
            string message) {
         _log.Error(new LogEntry(message, e).ToString());
         }

      public static void Info(string message) {
         _log.Info(new LogEntry(message).ToString());
         }
      }

   /* public static class LogWrapper {

        private static ILog _log;

        static LogWrapper() {
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); ;
            log4net.Config.XmlConfigurator.Configure();
            }

        public static void Debug(String message) {
            System.Diagnostics.Debug.WriteLine("DEBUG: " + DateTime.UtcNow.ToString() + message);
            _log.Debug(message);
            }

        public static void Error(Exception e) {
            string message = "ERROR. Message: " + e.Message + "\nData: " + e.Data + "\n StackTrace: " + e.StackTrace + "\n Source:" + e.Source + "\n Inner exception:" + e.InnerException;

            _log.Error(message);
            }

        public static void Error(string text, Exception e) {
            string message = "ERROR.Text: " + text + " Message: " + e.Message + "\nData: " + e.Data + "\n StackTrace: " + e.StackTrace + "\n Source:" + e.Source + "\n Inner exception:" + e.InnerException;

            _log.Error(message);
            }

       public static void Error (string message) {
          
       }

        public static void Info(string message) {
            _log.Info(message);
            }
        }*/


   }
