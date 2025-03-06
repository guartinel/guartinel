using System ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Guartinel.Core ;
using Guartinel.Core.Logging ;
using NUnit.Framework ;

namespace Guartinel.CLI.Utility.Tests {
   [TestFixture]
   public class LoggingTests : TestsBase {
      protected override void RegisterLoggers() {
         Logger.Setup<SimpleFileLogger> ("Test", "Test") ;
      }

      [Test]
      public void CheckLogging() {
         Logger.Log (LogLevel.Info, "test") ;
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
         new Timeout (5000).WaitFor (() => Directory.Exists (testFolder)) ;

         Assert.IsTrue (Directory.Exists (testFolder)) ;
         // Wait to release the folder
         new Timeout(2000).Wait() ;
         Directory.Delete (testFolder, true) ;
         Assert.IsFalse (Directory.Exists (testFolder)) ;
      }
   }
}