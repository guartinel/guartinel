using System ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Guartinel.CLI.Tests.Files ;
using Guartinel.Kernel.Logging ;
using NUnit.Framework ;
using System.Collections.Generic;
using Guartinel.Kernel ;

namespace Guartinel.CLI.Tests {
   [TestFixture]
   public class LoggingTests : TestsBase {
      protected override void RegisterLoggers() {
         Logger.Setup<SimpleFileLogger<Program>> ("Test", "Test") ;
      }

      [Test]
      public void CheckLogging() {
         Logger.Log (LogLevel.Info, "test") ;
      }      
   }

   [TestFixture]
   public class LoggingWithTestingTests : FileTestsBase {
      protected override void RegisterLoggers() {
         Logger.Setup<SimpleFileLogger> ("Test", "Test") ;
      }

      [Test]
      public void CheckLoggingFolder() {

         // Test folder
         var testFolder = @"c:\temp\TestLogging" ;

         Logger.SetSetting (FileLogger.Constants.SETTING_NAME_FOLDER, testFolder) ;

         if (Directory.Exists (testFolder)) {
            Directory.Delete (testFolder, true) ;
         }

         Assert.IsFalse (Directory.Exists (testFolder)) ;

         Logger.Log ("Test!") ;
         // Need to wait a bit
         new Kernel.Timeout (TimeSpan.FromSeconds (5)).WaitFor (() => Directory.Exists (testFolder)) ;

         Assert.IsTrue (Directory.Exists (testFolder)) ;
         // Wait to release the folder
         new Kernel.Timeout (TimeSpan.FromSeconds (2)).Wait() ;
         Directory.Delete (testFolder, true) ;
         Assert.IsFalse (Directory.Exists (testFolder)) ;
      }

      [Test]
      public void CheckLog() {
         // First, get the appropriate command
         int fileSize = 100 ;

         const string LOG_FOLDER = @"C:\Temp\GuartinelCLILog" ;

         if (Directory.Exists (LOG_FOLDER)) {
            Directory.Delete (LOG_FOLDER, true) ;
         }

         WriteTestFile (null, "testfile.size", fileSize) ;

         var result = RunCommand (Path.Combine (_testFolder, "testfile.size"), fileSize + 1, LOG_FOLDER) [0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         Logger.Log ("Test!") ;
         new Timeout (TimeSpan.FromSeconds (12)).Wait() ;

         Assert.AreEqual (1, Directory.GetFiles (LOG_FOLDER).Length) ;

         if (Directory.Exists (LOG_FOLDER)) {
            Directory.Delete (LOG_FOLDER, true) ;
         }

         new Timeout (TimeSpan.FromSeconds (3)).Wait() ;

         Assert.IsFalse (Directory.Exists (LOG_FOLDER)) ;
      }

      private List<CheckResult> RunCommand (string pattern,
                                            int maxSize,
                                            string logFolder) {
         return RunCommand ("checkFileSize", () => CreateArguments (pattern, maxSize, logFolder)) ;
      }

      private List<string> CreateArguments (string pattern,
                                            int maxSize,
                                            string logFolder) {



         List<string> arguments = new List<string>() ;
         arguments.Add ("checkFileSize") ;
         arguments.Add ($"--pattern=\"{pattern}\"") ;
         arguments.Add ($"--maxSize={maxSize}") ;
         arguments.Add ($"--logFolder={logFolder}") ;

         return arguments ;
      }
   }
}