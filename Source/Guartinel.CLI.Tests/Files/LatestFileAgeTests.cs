using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.CLI.Files ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Files {
   [TestFixture]
   public class LatestFileAgeTests : FileTestsBase {
      public void WriteFilesCheckLatestAge (string subFolder,
                                            TimeUnit timeUnit) {
         int fileSize = 10 ;
         int fileCount = 5 ;
         int latestFileAge = 10 ;
         int otherFilesAgeMin = 20 ;
         int otherFilesAgeMax = 40 ;

         var random = new Random() ;
         for (int fileIndex = 0; fileIndex < fileCount; fileIndex++) {
            WriteTestFile (subFolder, $"testfile.{fileIndex}", fileSize, random.Next (otherFilesAgeMin, otherFilesAgeMax)) ;
         }

         WriteTestFile (subFolder, "latestTestfile.txt", fileSize, UnitsEx.ConvertTimeToSeconds (latestFileAge, timeUnit)) ;

         bool subFolders = !string.IsNullOrEmpty (subFolder) ;

         var result = RunCommand (latestFileAge + 10, timeUnit, subFolders) [0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (latestFileAge - 10, timeUnit, subFolders) [0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;

         result = RunCommand (latestFileAge + 10, timeUnit, subFolders, "latestTestfile.*") [0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (latestFileAge - 10, timeUnit, subFolders, "latestTestfile.*") [0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;
      }

      [Test]
      public void WriteFiles_CheckLatestAge() {
         WriteFilesCheckLatestAge (string.Empty, TimeUnit.Second) ;
         WriteFilesCheckLatestAge (string.Empty, TimeUnit.Minute) ;
         WriteFilesCheckLatestAge (string.Empty, TimeUnit.Hour) ;
         WriteFilesCheckLatestAge (string.Empty, TimeUnit.Day) ;

         WriteFilesCheckLatestAge ("subfolder1", TimeUnit.Second) ;
         WriteFilesCheckLatestAge ("subfolder1", TimeUnit.Minute) ;
         WriteFilesCheckLatestAge ("subfolder1", TimeUnit.Hour) ;
         WriteFilesCheckLatestAge ("subfolder1", TimeUnit.Day) ;
      }

      private List<CheckResult> RunCommand (int maxAge,
                                            TimeUnit timeUnit,
                                            bool subFolders,
                                            string pattern = null) {
         return RunCommand ("checkLatestFileInFolder", () => CreateArguments (maxAge, timeUnit, subFolders, pattern)) ;
      }

      private List<string> CreateArguments (int maxAge,
                                            TimeUnit timeUnit,
                                            bool subFolders,
                                            string pattern = null) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("checkLatestFileInFolder") ;
         arguments.Add ($"--folder={_testFolder.WrapInDoubleQuotesIfContainsWhitespace()}") ;
         
         if (subFolders) {
            arguments.Add ($"--subFolders") ;
         }

         if (!string.IsNullOrEmpty(pattern)) {
            arguments.Add($@"--pattern=""{pattern}""") ;
         }

         arguments.Add($"--maxAge={maxAge}");
         arguments.Add($"--maxAgeUnit={timeUnit.ToString().ToLowerInvariant()}");
         
         return arguments ;
      }
   }
}