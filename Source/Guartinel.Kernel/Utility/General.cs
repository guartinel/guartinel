using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Guartinel.Kernel.Utility {
   public static class General {
      #region Type name utilities
      public static string GetTypeName (Type type) {
         if (type == null) { return String.Empty ; }

         return type.Name ;
      }
      #endregion

      /// <summary>
      /// Check null argument and raise exception if necessary.
      /// </summary>
      /// <typeparam name="T">any type of class (not struct) nullable</typeparam>
      /// <param name="value">value to check</param>
      /// <param name="message">exception message if it is rather null</param>
      /// <returns></returns>
      public static T CheckNull<T> (this T value,
                                    string message) {
         return CheckNull (value, message, null) ;
      }

      /// <summary>
      /// Check null argument and raise exception if necessary.
      /// </summary>
      /// <typeparam name="T">any type of class (not struct) nullable</typeparam>
      /// <param name="value">value to check</param>
      /// <param name="message">exception message if it is rather null</param>
      /// <param name="messageArguments">Message arguments.</param>
      /// <returns></returns>
      public static T CheckNull<T> (this T value,
                                    string message,
                                    params object[] messageArguments) {
         if (value == null) {
            // Use arguments if possible
            if (messageArguments != null) { throw new ArgumentNullException(string.Format (message, messageArguments)) ; }

            throw new ArgumentNullException (message) ;
         }

         return value ;
      }

      /// <summary>
      /// Check null argument and raise exception if necessary.
      /// </summary>
      /// <typeparam name="T">any type of class (not struct) nullable</typeparam>
      /// <param name="value">value to check</param>
      /// <returns></returns>
      public static T CheckNull<T> (this T value) {
         return CheckNull (value, "Value or parameter cannot be null.") ;
      }

      public static T Go<T> (this T instance,
                             Action<T> action) where T : class {
         action?.Invoke (instance) ;

         return instance ;
      }

      public static T CastTo<T> (this object instance,
                                 string message = null) where T : class {
         if (instance == null) {
            throw new Exception (String.Format (string.IsNullOrEmpty (message) ? "Object expected to be '{0}' but it was null." : message, GetTypeName (typeof(T)))) ;
         }

         if (!(instance is T)) {
            throw new Exception (String.Format (string.IsNullOrEmpty (message) ? "Object expected to be '{0}' but it was '{1}'." : message, GetTypeName (typeof(T)), GetTypeName (instance.GetType()))) ;
         }

         return instance as T ;
      }

      public static bool IsProcessAlreadyRunning() {
         string processName = Process.GetCurrentProcess().ProcessName ;
         Process[] processes = Process.GetProcesses() ;

         foreach (Process process in processes) {
            if (process.ProcessName.Equals (processName)) { return true ; }
         }

         return false ;
      }

      //public static int AvailableWorkerThreadsInPool {
      //   get {
      //      int workerThreads;
      //      int completionPortThreads;
      //      ThreadPool.GetAvailableThreads (out workerThreads, out completionPortThreads) ;

      //      return workerThreads;
      //   }
      //}

      /// <summary>
      /// Compare two lists of the same type: the comparison is done by the specified expression.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="list1"></param>
      /// <param name="list2"></param>
      /// <param name="compareValues"></param>
      /// <returns></returns>
      // public static bool CompareLists<T> (IList<T> list1, IList<T> list2, CompareObjectValues<T> compareValues) {
      public static bool CompareLists<T> (IList<T> list1,
                                          IList<T> list2,
                                          Func<T, T, bool> compareValues) {
         // Compare number of items first
         if (list1.Count != list2.Count) { return false ; }

         // Loop on the first list
         foreach (var instance1 in list1) {
            // Need to find a pair for each items in the list
            bool foundPair = false ;
            // Track items already used in the second list
            IList<T> alreadyUsed = new List<T>() ;

            // Loop on the second list
            foreach (var instance2 in list2) {
               // Already used?
               if (!alreadyUsed.Contains (instance2)) {
                  // Compare!
                  if (compareValues (instance1, instance2)) {
                     foundPair = true ;
                     alreadyUsed.Add (instance2) ;
                     break ;
                  }
               }
            }

            // No pair, failed
            if (!foundPair) { return false ; }
         }

         return true ;
      }

      public static void SetProcessesPriority (string processName,
                                               ProcessPriorityClass priority,
                                               Action<string> logError) {
         try {
            var processes = Process.GetProcessesByName (processName) ;
            foreach (var process in processes) {
               try {
                  process.PriorityClass = priority ;
               } catch (Exception exception) {
                  logError?.Invoke ($"Cannot set process priority for '{processName}'. Error: {exception.GetAllMessages()}") ;
               }
            }
         } catch (Exception exception) {
            logError?.Invoke($"Cannot get processes for name '{processName}'. Error: {exception.GetAllMessages()}");
         }
      }
   }
}