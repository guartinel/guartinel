using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Fclp ;
using Fclp.Internals.Extensions ;
using Guartinel.CLI.Utility.Commands ;
using Guartinel.CLI.Utility.Files ;
using Guartinel.Core ;
using NUnit.Framework ;

namespace Guartinel.CLI.Utility.Tests.Files {
   [TestFixture]
   public class LatestFileAgeTests : FileTestsBase {
      public void WriteFilesCheckLatestAge (string subFolder,
                                            LatestFileAgeChecker.TimeUnit timeUnit) {
         int fileSize = 10 ;
         int fileCount = 5 ;
         int latestFileAge = 10 ;
         int otherFilesAgeMin = 20 ;
         int otherFilesAgeMax = 40 ;

         var random = new Random() ;
         for (int fileIndex = 0; fileIndex < fileCount; fileIndex++) {
            WriteTestFile (subFolder, $"testfile.{fileIndex}", fileSize, random.Next (otherFilesAgeMin, otherFilesAgeMax)) ;
         }

         WriteTestFile (subFolder, "latestTestfile.txt", fileSize, LatestFileAgeChecker.ConvertToSeconds (latestFileAge, timeUnit)) ;

         bool subFolders = !string.IsNullOrEmpty (subFolder) ;

         var result = RunCommand (latestFileAge + 10, timeUnit, subFolders) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (latestFileAge - 10, timeUnit, subFolders) ;
         Assert.IsFalse (result.Success, result.ToString()) ;

         result = RunCommand (latestFileAge + 10, timeUnit, subFolders, "latestTestfile.*") ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (latestFileAge - 10, timeUnit, subFolders, "latestTestfile.*") ;
         Assert.IsFalse (result.Success, result.ToString()) ;
      }

      [Test]
      public void WriteFiles_CheckLatestAge() {
         WriteFilesCheckLatestAge (string.Empty, LatestFileAgeChecker.TimeUnit.Second) ;
         WriteFilesCheckLatestAge (string.Empty, LatestFileAgeChecker.TimeUnit.Minute) ;
         WriteFilesCheckLatestAge (string.Empty, LatestFileAgeChecker.TimeUnit.Hour) ;
         WriteFilesCheckLatestAge (string.Empty, LatestFileAgeChecker.TimeUnit.Day) ;

         WriteFilesCheckLatestAge ("subfolder1", LatestFileAgeChecker.TimeUnit.Second) ;
         WriteFilesCheckLatestAge ("subfolder1", LatestFileAgeChecker.TimeUnit.Minute) ;
         WriteFilesCheckLatestAge ("subfolder1", LatestFileAgeChecker.TimeUnit.Hour) ;
         WriteFilesCheckLatestAge ("subfolder1", LatestFileAgeChecker.TimeUnit.Day) ;
      }

      private CheckResult RunCommand (int maxAge,
                                      LatestFileAgeChecker.TimeUnit timeUnit,
                                      bool subFolders,
                                      string pattern = null) {
         ICommand latestFileAgeChecker = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "checkLatestFileInFolder") ;
         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         latestFileAgeChecker.Setup (parser) ;

         var arguments = CreateArguments (maxAge, timeUnit, subFolders, pattern) ;
         parser.Parse (arguments.ToArray()) ;
         return latestFileAgeChecker.Run()[0] ;
      }

      private List<string> CreateArguments (int maxAge,
                                            LatestFileAgeChecker.TimeUnit timeUnit,
                                            bool subFolders,
                                            string pattern = null) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("--command:checkLatestFileInFolder") ;
         arguments.Add ($"--folder:{_testFolder.WrapInDoubleQuotesIfContainsWhitespace()}") ;
         
         if (subFolders) {
            arguments.Add ($"--subFolders") ;
         }

         if (!string.IsNullOrEmpty(pattern)) {
            arguments.Add($"--include:\"{pattern}\"");
         }

         arguments.Add($"--maxAge:{maxAge}");
         arguments.Add($"--maxAgeUnit:{timeUnit.ToString().ToLowerInvariant()}");
         
         return arguments ;
      }
   }
}