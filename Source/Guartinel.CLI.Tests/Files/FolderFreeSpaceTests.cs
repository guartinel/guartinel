using System ;
using System.Collections.Generic;
using System.Diagnostics ;
using System.Globalization ;
using System.IO ;
using System.Linq;
using System.Reflection ;
using Guartinel.CLI.Commands ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Files {
   [TestFixture]
  public class FolderFreeSpaceTests : FileTestsBase {
      [Test]
      public void TestNetworkShare() {
         const string SHARE_PATH = @"\\naska\root1" ;

         CheckResult result = RunCommand (SHARE_PATH, 1, 1) [0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (SHARE_PATH, 1800, 1) [0] ;
         Assert.IsFalse (result.Success, result.ToString());

         result = RunCommand (SHARE_PATH, 1, 80) [0] ;
         Assert.IsFalse (result.Success, result.ToString());
      }

      private List<CheckResult> RunCommand (string folder,
                                            int minFreeSpaceGBs,
                                            int minFreeSpacePercents) {
         return RunCommand ("checkFolderFreeSpace", () => CreateArguments (folder, minFreeSpaceGBs, minFreeSpacePercents)) ;
      }

      private static List<string> CreateArguments (string folder,
                                                   int minFreeSpaceGBs,
                                                   int minFreeSpacePercents) {

         List<string> arguments = new List<string>() ;
         arguments.Add ("checkFolderFreeSpace") ;
         arguments.Add ($"--folder={folder.WrapInDoubleQuotesIfContainsWhitespace()}") ;
         
         if (minFreeSpaceGBs > 0) {
            arguments.Add ($"--minSpaceGBs={minFreeSpaceGBs}") ;
         }
         
         if (minFreeSpacePercents > 0) {
            arguments.Add ($"--minSpacePercents={minFreeSpacePercents.ToString(CultureInfo.InvariantCulture)}") ;
         }
         return arguments ;
      }
   }
}