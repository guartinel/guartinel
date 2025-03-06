using System ;
using System.Collections.Generic;
using System.Diagnostics ;
using System.Globalization ;
using System.IO ;
using System.Linq;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Files {
   [TestFixture]
   public class FreeSpaceTests : FileTestsBase {
      [Test]
      public void GetFreeSpace_WriteBigFile_CheckRemainingSpace() {
         var driveInfo = new DriveInfo (new DirectoryInfo (_testFolder).Root.FullName) ;
         
         Logger.Info ($"Free info '{_testFolder}' on {driveInfo.TotalFreeSpace} of {driveInfo.TotalSize}") ;

         var result = RunCommand (_testFolder, (int) (driveInfo.TotalFreeSpace / 1024.0 / 1024.0 / 1024.0 / 10.0), 0)[0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (_testFolder, 0, (int) ((double) driveInfo.TotalFreeSpace / driveInfo.TotalSize * 100 / 2.0))[0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         var fileSize = (long) ( driveInfo.TotalFreeSpace / 100.0 * 2.0) ;

         WriteTestFile (string.Empty, "testfile.big", (int) fileSize) ;

         result = RunCommand(_testFolder, ((int) ((driveInfo.TotalFreeSpace - fileSize) / 1024.0 / 1024.0 / 1024.0 + 2)), 0)[0] ;
         Assert.IsFalse (result.Success, result.ToString());

         result = RunCommand (_testFolder, 0, (int) ((double) driveInfo.TotalFreeSpace / driveInfo.TotalSize * 100) + 2)[0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;
      }

      private List<CheckResult> RunCommand (string drive,
                                            int minFreeSpaceGBs,
                                            int minFreeSpacePercents) {
         return RunCommand ("checkFreeSpace", () => CreateArguments (drive, minFreeSpaceGBs, minFreeSpacePercents)) ;
      }

      private static List<string> CreateArguments (string folder,
                                                   int minFreeSpaceGBs,
                                                   int minFreeSpacePercents) {

         List<string> arguments = new List<string>() ;
         arguments.Add ("checkFreeSpace") ;
         arguments.Add ($"--drive={folder.WrapInDoubleQuotesIfContainsWhitespace()}") ;
         
         if (minFreeSpaceGBs > 0) {
            arguments.Add ($"--minSpaceGBs={minFreeSpaceGBs.ToString(CultureInfo.InvariantCulture)}") ;
         }
         
         if (minFreeSpacePercents > 0) {
            arguments.Add ($"--minSpacePercents={minFreeSpacePercents.ToString(CultureInfo.InvariantCulture)}") ;
         }
         return arguments ;
      }
   }
}