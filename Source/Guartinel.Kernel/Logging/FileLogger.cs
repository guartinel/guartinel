using System;
using System.Collections.Concurrent ;
using System.Collections.Generic ;
using System.Globalization ;
using System.IO ;
using System.Linq;
using System.Text;
using System.Threading ;
using System.Threading.Tasks ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Kernel.Logging {
   public class FileLogger : LoggerBase, ILogger {
      public static class Constants {
         public const string SETTING_NAME_FOLDER = "folder" ;

         // Max log file size: 20 MBs
         public const int MAX_FILE_SIZE = 20 * 1024 * 1024 ;

         public const int MAX_TASK_RUN_TIME_SECONDS = 5 ;
      }

      public FileLogger() : this (string.Empty, string.Empty, null) { }

      public FileLogger (string folderName) : this (folderName, string.Empty, null) { }

      public FileLogger (string folderName,
                         List<string> categories) : this (folderName, string.Empty, categories) { }

      public FileLogger (string folderName,
                         string fileNamePrefix,
                         List<string> categories) : base (categories) {

         _folderName = folderName ;
         _fileNamePrefix = fileNamePrefix ;
      }

      public FileLogger (string fileNamePrefix,
                         string category) : this (string.Empty, fileNamePrefix, new List<string> {category}) { }

      ~FileLogger() {
         ProcessQueue() ;
      }

      private readonly string _folderName ;

      private readonly string _fileNamePrefix ;
      private string FileNamePrefix => string.IsNullOrEmpty (_fileNamePrefix) ? string.Empty : $"{_fileNamePrefix}_" ;

      private int _fileIndex = 1 ;
      private string _fileDay = string.Empty ;
      private string _fileName = string.Empty;

      public string FileName {
         get {
            GenerateFileName() ;
            return _fileName ;
         }
      }

      private void GenerateFileName() {
         var fileDate = DateTime.UtcNow.ToString ("yyyy-MM-dd", CultureInfo.InvariantCulture) ;
         if (string.IsNullOrEmpty (_fileDay)) {
            _fileDay = fileDate ;
         }

         if (fileDate != _fileDay) {
            _fileIndex = 0 ;
            _fileDay = fileDate ;
         }

         var fileName = Path.Combine (GetFolderName(),
                                    $"{FileNamePrefix}{Logger.Settings.Name}_" +
                                    $"{fileDate}." +
                                    $"{_fileIndex.ToString().PadLeft (4, '0')}.log") ;
         if (!File.Exists (fileName)) {
            _fileName = fileName ;
            return ;
         }

         // Check if file is too big
         if (new FileInfo (fileName).Length > Constants.MAX_FILE_SIZE) {
            lock (_fileLock) {
               _fileIndex++ ;
            }
            GenerateFileName() ;
         }

         _fileName = fileName ;
      }

      private string GetFolderName() {
         // Use specified folder
         if (!string.IsNullOrEmpty (_folderName)) return _folderName ;

         // Try logger settings
         var folderName = Logger.GetSetting (Constants.SETTING_NAME_FOLDER) ;
         if (!string.IsNullOrEmpty (folderName)) return folderName ;

         return Path.Combine (AssemblyEx.GetAssemblyPath<FileLogger>(), "Logs") ;
      }

      private bool _directoryExists = false ;

      private void EnsureDirectory() {
         if (_directoryExists) return ;
         _directoryExists = Directory.Exists (GetFolderName()) ;
         if (_directoryExists) return ;
         Directory.CreateDirectory (GetFolderName()) ;
         _directoryExists = Directory.Exists (GetFolderName()) ;
      }

      private readonly object _fileLock = new object() ;
      private readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>() ;

      private readonly object _taskLock = new object() ;
      private Task _task = null ;

      private void ProcessQueue() {
         try {
            DateTime end = DateTime.UtcNow.AddSeconds (Constants.MAX_TASK_RUN_TIME_SECONDS) ;

            EnsureDirectory() ;
            GenerateFileName();
            List<string> collectedLines = new List<string>();

            // while (!_messages.IsEmpty) {
            while (DateTime.UtcNow < end) {
               //if (_messages.IsEmpty) {
               //   Thread.Sleep (500) ;
               //   continue ;
               //}

               // If no more messages, get out
               if (_messages.IsEmpty) break ;

               if (!_messages.TryDequeue (out string line)) break ;

               if (line == null) continue ;

               collectedLines.Add (line) ;

               //lock (_fileLock) {
               //   File.AppendAllText (fileName, $"{line}{Environment.NewLine}") ;
               //}
            }

            if (collectedLines.Any()) {
               lock (_fileLock) {
                  File.AppendAllText (_fileName, collectedLines.Concat (Environment.NewLine) + Environment.NewLine) ;
               }
            }
         } catch (Exception e) {
            // Ignore error, just show the reason on the standard output
            Console.WriteLine ($"File logging error. Filename: {_fileName}. Error: {NormalizeLogLine(e.GetAllMessages())}") ;
         }

         lock (_taskLock) {
            _task = null ;
         }
      }

      protected override void DoLog (string timeStamp,
                                     LogLevel level,
                                     string message) {

         _messages.Enqueue (FormatLine (timeStamp, level, message)) ;

         lock (_taskLock) {
            if (_task != null) return ;

            _task = new Task (ProcessQueue) ;
            _task.Start() ;
         }
      }
   }

   public class SimpleFileLogger<T> : FileLogger {
      public SimpleFileLogger() : base (AssemblyEx.GetAssemblyPath<T>()) { }
   }

   public class SimpleFileLogger : FileLogger {
      public SimpleFileLogger() { }
   }
}