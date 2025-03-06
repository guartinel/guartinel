using System;
using System.Collections.Generic;
using System.IO ;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guartinel.Kernel.Utility {
   public static class AssemblyEx {
      public static string GetVersion() {
         return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString() ;
      }

      /// <summary>
      /// Get path of the executing assembly in normal path format (not uri).
      /// </summary>
      /// <returns></returns>
      public static string GetAssemblyPath<T> () {
         string assemblyPath = Path.GetDirectoryName(typeof(T).Assembly.Location);

         if (string.IsNullOrEmpty(assemblyPath)) { return String.Empty; }
         // if (executingAssemblyPath.StartsWith (@"file:/")) {
         // DTAP: On linux this will be trailing prefix of the path which cause exception in the URI constructor
         //    executingAssemblyPath = executingAssemblyPath.Remove (0, 6) ;
         // }
         Uri uri = null;
         try {
            uri = new Uri(assemblyPath, UriKind.Absolute);
         } catch {
            Console.WriteLine($"Invalid path for uri: {assemblyPath}");
            throw;
         }
         return uri.LocalPath;
      }

      /// <summary>
      /// Get path to the running assembly and add a relative path to it.
      /// </summary>
      /// <param name="relativePath"></param>
      /// <returns></returns>
      public static string AddToAssemblyPath<T> (string relativePath) {
         return Path.Combine(GetAssemblyPath<T>(), relativePath);
      }
   }
}