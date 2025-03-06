using System;
using Guartinel.CLI.Files;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility;

namespace Guartinel.CLI.Tests.Files {
   [TestFixture]
   public class FileChangeCheckerTest : FileTestsBase {

      private const string TEST_RESULTS_PATH = @"C:\\Temp\Measurement\FileCheck" ;

      [Test]
      public void WriteFiles_CheckLatestAge() {
         if (Directory.Exists (TEST_RESULTS_PATH)) {
            Directory.Delete (TEST_RESULTS_PATH, true) ;
         }

         // create the file
         WriteTestFile (string.Empty, "latestTestfile.txt", 10, UnitsEx.ConvertTimeToSeconds (100, TimeUnit.Second)) ;

         //there should be no previus check so this is the first so this is success
         var result = RunCommand (true, 5, TimeUnit.Second, 101) [0] ;
         Assert.IsTrue (result.Success) ;

         //Write again 
         WriteTestFile (string.Empty, "latestTestfile.txt", 10, UnitsEx.ConvertTimeToSeconds (0, TimeUnit.Second)) ;

         //success should be error until the cooldown expires
         var result2 = RunCommand (true, 5, TimeUnit.Second, 101) [0] ;
         Assert.IsFalse (result2.Success) ;

         // wait to exceed cooldown
         System.Threading.Thread.Sleep (7000) ;

         // expect success = error because cooldown is exceeded
         var result3 = RunCommand (true, 5, TimeUnit.Second, 101) [0] ;
         Assert.AreEqual (result2.Data [FileChangeChecker.Constants.Results.LAST_MODIFICATION], result3.Data [FileChangeChecker.Constants.Results.LAST_MODIFICATION]) ;
         Assert.IsTrue (result3.Success) ;

         Directory.Delete (TEST_RESULTS_PATH, true) ;
      }

      private List<CheckResult> RunCommand (bool subFolders,
                                            int coolDown,
                                            TimeUnit coolDownTimeUnit,
                                            int instanceID,
                                            string pattern = null) {
         return RunCommand ("checkFileChange", () => CreateArguments (subFolders, coolDown, coolDownTimeUnit, instanceID, pattern)) ;
      }

      private List<string> CreateArguments (bool subFolders,
                                            int coolDown,
                                            TimeUnit coolDownTimeUnit,
                                            int instanceID,
                                            string pattern = null) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("checkFileChange") ;
         arguments.Add ($"--folder={_testFolder.WrapInDoubleQuotesIfContainsWhitespace()}") ;

         if (subFolders) {
            arguments.Add ($"--subFolders") ;
         }

         if (!string.IsNullOrEmpty (pattern)) {
            arguments.Add ($@"--pattern=""{pattern}""") ;
         }
         arguments.Add ($"--resultsFolder={TEST_RESULTS_PATH}") ;
         arguments.Add ($"--coolDown={coolDown}") ;
         arguments.Add ($"--coolDownUnit={coolDownTimeUnit.ToString().ToLowerInvariant()}") ;
         arguments.Add ($"--id={instanceID}") ;
         return arguments ;
      }
   }
}