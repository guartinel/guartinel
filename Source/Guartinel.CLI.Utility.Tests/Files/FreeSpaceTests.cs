using System ;
using System.Collections.Generic;
using System.Diagnostics ;
using System.Globalization ;
using System.Linq;
using Fclp;
using Fclp.Internals.Extensions;
using Guartinel.CLI.Utility.Commands;
using Guartinel.Core ;
using NUnit.Framework;

namespace Guartinel.CLI.Utility.Tests.Files {
   [TestFixture]
   public class FreeSpaceTests : FileTestsBase {
      [Test]
      public void GetFreeSpace_WriteBigFile_CheckRemainingSpace() {
         ulong freeBytesAvailable ;
         ulong totalNumberOfBytes ;
         ulong totalNumberOfFreeBytes ;
         Assert.IsTrue (WinAPI.GetDiskFreeSpaceEx (_testFolder, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes)) ;

         Debug.WriteLine ($"Free info: {freeBytesAvailable}, {totalNumberOfBytes}, {totalNumberOfFreeBytes}") ;

         var result = RunCommand (_testFolder, totalNumberOfFreeBytes / 1024.0 / 1024.0 / 1024.0 / 2, 0) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (_testFolder, 0.0, (int) ((double) totalNumberOfFreeBytes / totalNumberOfBytes * 100 / 2.0)) ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         var fileSize = (ulong) (totalNumberOfFreeBytes / 100.0 * 2.0) ;

         WriteTestFile (string.Empty, "testfile.big", (int) fileSize) ;

         result = RunCommand(_testFolder, (totalNumberOfFreeBytes - fileSize) / 1024.0 / 1024.0 / 1024.0 + 0.1, 0) ;
         Assert.IsFalse (result.Success, result.ToString());

         result = RunCommand (_testFolder, 0.0, (int) ((double) totalNumberOfFreeBytes / totalNumberOfBytes * 100) + 1) ;
         Assert.IsFalse (result.Success, result.ToString()) ;
      }

      private CheckResult RunCommand (string folder,
                                      double minFreeSpaceGBs,
                                      int minFreeSpacePercents) {
         ICommand checkFolderSize = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "checkFreeSpace") ;
         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         checkFolderSize.Setup (parser) ;

         var arguments = CreateArguments (folder, Math.Round (minFreeSpaceGBs, 2), minFreeSpacePercents) ;
         parser.Parse (arguments.ToArray()) ;
         return checkFolderSize.Run()[0] ;
      }

      private static List<string> CreateArguments (string folder,
                                                   double minFreeSpaceGBs,
                                                   int minFreeSpacePercents) {

         List<string> arguments = new List<string>() ;
         arguments.Add ("--command:checkFreeSpace") ;
         arguments.Add ($"--folder:{folder.WrapInDoubleQuotesIfContainsWhitespace()}") ;
         
         if (minFreeSpaceGBs > 0) {
            arguments.Add ($"--minSpaceGBs:{minFreeSpaceGBs}") ;
         }
         
         if (minFreeSpacePercents > 0) {
            arguments.Add ($"--minSpacePercents:{minFreeSpacePercents.ToString(CultureInfo.InvariantCulture)}") ;
         }
         return arguments ;
      }
   }
}