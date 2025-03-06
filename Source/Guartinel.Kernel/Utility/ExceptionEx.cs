using System ;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.Kernel.Utility {
   public static class ExceptionEx {
      public static string GetAllMessages (this System.Exception exception,
                                           bool addMessage = true,
                                           [System.Runtime.CompilerServices.CallerMemberName]
                                           string memberName = "",
                                           [System.Runtime.CompilerServices.CallerFilePath]
                                           string sourceFilePath = "",
                                           [System.Runtime.CompilerServices.CallerLineNumber]
                                           int sourceLineNumber = 0) {
         var startingPoint = addMessage ? exception : exception.InnerException ?? exception ;
         IEnumerable<string> messages = startingPoint.FromHierarchy (ex => ex.InnerException).Select (ex => ex.Message) ;

         string result = String.Join (Environment.NewLine, messages) ;
         result += "\n Exception details: caller member:" + memberName + " source file path: " + sourceFilePath + ":" + sourceLineNumber ;
         result += "\n Exception trace : " + exception.StackTrace ;
         return result ;
      }

      public static void ExecuteWithLog (Action action,
                                         Action<string> logError = null) {
         try {
            action?.Invoke() ;
         } catch (Exception e) {
            if (logError != null) {
               logError.Invoke (e.GetAllMessages()) ;
            } else {
               Logger.Error (e.GetAllMessages()) ;
            }
         }
      }

      public static void ExecuteWithLog (Action<TagLogger> action,
                                         TagLogger logger) {
         try {
            action?.Invoke (logger) ;
         } catch (Exception e) {
            if (logger != null) {
               logger.Error (e.GetAllMessages()) ;
            } else {
               Logger.Error (e.GetAllMessages()) ;
            }
         }
      }
   }
}