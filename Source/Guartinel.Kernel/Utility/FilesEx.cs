using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Logging;

namespace Guartinel.Kernel.Utility {
   public static class FilesEx {
      public static class Constants {
         public const int DEFAULT_RESULT = 0 ;
         public const string ALL_FILES = "*.*" ;
      }

      public static DateTime GetLatestFileModificationTimeStamp (string folder,
                                                                 bool subFolders,
                                                                 string includePattern = Constants.ALL_FILES) {
         try {
            // Get latest file
            var searchOption = subFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ;

            FileInfo[] files = new DirectoryInfo (folder).GetFiles (includePattern, searchOption) ;
            var latestFile = files.OrderByDescending (file => file.LastWriteTime).FirstOrDefault() ;
            if (latestFile == null) return DateTime.MinValue ;

            return latestFile.LastWriteTimeUtc ;

         } catch (Exception exception) {
            // Ignore error   

            Logger.Log (LogLevel.Error, $"Cannot determine latest file in {folder}. Message: {exception.GetAllMessages()}") ;
            return DateTime.MinValue ;
         }
      }

      public static int GetAgeOfLatestFile (string folder,
                                            bool subFolders,
                                            string includePattern = Constants.ALL_FILES) {
         DateTime result = GetLatestFileModificationTimeStamp (folder, subFolders, includePattern) ;
         if (result == DateTime.MinValue) {
            return Constants.DEFAULT_RESULT ;
         }

         TimeSpan age = DateTime.UtcNow - result ;
         return (int) age.TotalSeconds ;

      }

      public static string EnsureFileNameHasFullPath (string fileName) {
         if (string.IsNullOrEmpty (fileName)) return fileName ;

         var result = fileName ;
         string configurationFileFolder = Path.GetDirectoryName (fileName) ;
         if (string.IsNullOrEmpty (configurationFileFolder)) {
            result = Path.Combine (Directory.GetCurrentDirectory(), fileName) ;
         }

         return result ;
      }

   }
}