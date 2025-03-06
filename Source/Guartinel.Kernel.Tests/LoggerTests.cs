using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Utility ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using Guartinel.Kernel.Logging ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.Kernel.Tests {

   public class Logger1 : LoggerBase, ILogger {
      public readonly List<string> Logs = new List<string>() ;

      public Logger1 (List<string> categories) : base (categories) { }

      protected override void DoLog (string timeStamp,
                                     LogLevel level,
                                     string message) {
         lock (Logs) {
            Logs.Add (message) ;
         }
      }
   }

   [TestFixture]
   public class LoggerTests {
      [Test]
      public void TestCategories() {
         const string CATEGORY1 = "category1" ;
         const string CATEGORY2 = "category2" ;

         Logger.Setup ("test", "test") ;
         var logger1 = new Logger1 (null) ;
         var logger2 = new Logger1 (new List<string> {CATEGORY1}) ;

         Logger.RegisterLogger (logger1) ;
         Logger.RegisterLogger (logger2) ;

         // Should go to first logger only
         Logger.Log ("test1a") ;
         // Should go to second logger only
         Logger.Log ("test2a", CATEGORY1) ;
         // Should go nowhere
         Logger.Log ("test3a", CATEGORY2) ;

         Assert.AreEqual (3, logger1.Logs.Count) ;
         Assert.AreEqual (1, logger1.Logs.Count (x => x.Equals ("test1a"))) ;

         Assert.AreEqual (1, logger2.Logs.Count) ;
         Assert.AreEqual (1, logger2.Logs.Count (x => x.Equals ("test2a"))) ;
      }

      [Test]
      public void TestCategoriesInFileLogger() {
         var testFolderName = AssemblyEx.AddToAssemblyPath<LoggerTests> ("Log") ;
         Directory.CreateDirectory (testFolderName) ;

         try {
            const string CATEGORY1 = "category1" ;
            const string CATEGORY2 = "category2" ;

            Logger.Setup ("test", "test") ;
            var logger1 = new FileLogger (testFolderName) ;
            var logger2 = new FileLogger (testFolderName, CATEGORY1, new List<string> {CATEGORY1}) ;

            Logger.RegisterLogger (logger1) ;
            Logger.RegisterLogger (logger2) ;

            // Should go to first logger only
            Logger.Log ("test1a") ;
            // Should go to second logger only
            Logger.Log ("test2a", CATEGORY1) ;
            // Should go nowhere
            Logger.Log ("test3a", CATEGORY2) ;

            // Wait a bit to finish the logging
            new Timeout (TimeSpan.FromSeconds (20)).Wait() ;

            var logFile1 = File.ReadAllLines (logger1.FileName).ToList() ;
            var logFile2 = File.ReadAllLines (logger2.FileName).ToList() ;

            Assert.AreEqual (3, logFile1.Count) ;
            Assert.AreEqual (1, logFile1.Count (x => x.Contains ("test1a"))) ;

            Assert.AreEqual (1, logFile2.Count) ;
            Assert.AreEqual (1, logFile2.Count (x => x.Contains ("test2a"))) ;

         } finally {
            Directory.Delete (testFolderName, true) ;
         }
      }

      [Test]
      public void TestLoggingPassword() {
         Logger.Setup ("test", "test") ;
         var logger1 = new Logger1 (null) ;

         Logger.RegisterLogger (logger1) ;

         // Create JObject with embedded password
         JObject jObject = new JObject() ;
         jObject ["password"] = "ytrewq" ;
         jObject ["child1"] = new JObject() ;
         jObject ["child1"] ["ehune1"] = "1234" ;
         jObject ["child1"] ["password"] = "qwerty" ;
         jObject ["child2"] = new JObject() ;
         jObject ["child2"] ["ehune2"] = "5678" ;
         Logger.Log (jObject.ConvertToLog()) ;

         Assert.AreEqual (1, logger1.Logs.Count) ;
         Assert.AreEqual (1, logger1.Logs.Count (x => x.Contains ("1234"))) ;
         Assert.AreEqual (0, logger1.Logs.Count (x => x.Contains ("qwerty"))) ;
         Assert.AreEqual (0, logger1.Logs.Count (x => x.Contains ("ytrewq"))) ;
         Assert.AreEqual (1, logger1.Logs.Count (x => x.Contains ("****"))) ;
         string[] split = logger1.Logs.First (x => x.Contains ("****")).Split ("****") ;
         Assert.AreEqual (3, split.Length) ;
         Assert.AreEqual (1, logger1.Logs.Count (x => x.Contains ("5678"))) ;
      }

      [Test]
      public void TestLoggingLevel() {
         Logger.Setup ("test", "test") ;
         var logger1 = new Logger1 (null) ;

         Logger.RegisterLogger (logger1) ;

         Assert.AreEqual (LogLevel.Info.ToString(), Logger.Settings.LogLevel.ToString()) ;
         Assert.AreEqual (0, logger1.Logs.Count) ;

         Logger.Info ("InfoLog1") ;
         Logger.Debug ("DebugLog1") ;
         Assert.AreEqual (1, logger1.Logs.Count) ;
         Assert.AreEqual (0, logger1.Logs.Count (x => x == "DebugLog1")) ;

         Logger.Settings.LogLevel = LogLevel.Debug ;
         Assert.AreEqual (LogLevel.Debug.ToString(), Logger.Settings.LogLevel.ToString()) ;

         Logger.Info ("InfoLog2") ;
         Logger.Debug ("DebugLog2") ;
         Assert.AreEqual (3, logger1.Logs.Count) ;
         Assert.AreEqual (1, logger1.Logs.Count (x => x == "InfoLog2")) ;
         Assert.AreEqual (1, logger1.Logs.Count (x => x == "DebugLog2")) ;
      }

      [Test]
      public void CheckTagLogger() {
         Logger.Setup ("test", "test") ;
         var logger1 = new Logger1 (null) ;
         Logger.RegisterLogger (logger1) ;

         TagLogger taglogger = new TagLogger ("test1", "test2") ;
         taglogger.Info ("Log message 11.") ;

         Assert.AreEqual (1, logger1.Logs.Count) ;
         Assert.AreEqual ("#test1 #test2 - Log message 11.", logger1.Logs [0]) ;
      }

      [Test]
      public void CheckTagLoggerMerging() {
         Logger.Setup ("test", "test") ;
         var logger1 = new Logger1 (null) ;
         Logger.RegisterLogger (logger1) ;

         TagLogger taglogger1 = new TagLogger ("test1") ;
         TagLogger taglogger2 = new TagLogger (taglogger1.Tags, "test2", "test1") ;
         taglogger2.Info ("Log message 12.") ;

         Assert.AreEqual (1, logger1.Logs.Count) ;
         Assert.AreEqual ("#test1 #test2 - Log message 12.", logger1.Logs [0]) ;

         TagLogger taglogger4 = new TagLogger (taglogger2.Tags, null) ;
         TagLogger taglogger5 = new TagLogger(null, taglogger2.Tags) ;
      }
   }
}