using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel.Logging {
   public abstract class LoggerBase {
      protected LoggerBase (List<string> categories) {
         Categories = categories?.ToArray().ToList() ;
      }

      protected LoggerBase (string category) : this (new List<string> {category}) {}

      public List<string> Categories {get ;}

      protected string NormalizeLogLine (string logLine) {
         if (string.IsNullOrEmpty (logLine)) return string.Empty ;

         const string SEPARATOR = " >> " ;

         return logLine.Replace (Environment.NewLine, SEPARATOR)
                  // .Replace ("\\n", string.Empty)
                  .Replace ("\r", string.Empty)
                  .Replace ("\n", SEPARATOR) ;
      }

      public void Log (LogLevel level,
                       string message,
                       List<string> categories) {

         string normalizedMessageLine = NormalizeLogLine (message) ;
         string messageLine = $"{normalizedMessageLine}" ;

         DoLog (Logger.AsString (DateTime.UtcNow), level, messageLine) ;
      }

      protected abstract void DoLog (string timeStamp,
                                     LogLevel level,
                                     string message) ;

      protected string FormatLine (string timeStamp,
                                   LogLevel level,
                                   string message) {
         var levelString = $"{level.ToString().ToUpperInvariant()}:".PadRight (5) ;
         return $"{timeStamp}: {levelString} {message}" ;
      }
   }
}