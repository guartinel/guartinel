using System.Collections.Generic;
using System.Linq;
using Guartinel.CLI.Files ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests.Files {
   [TestFixture]
   public class FolderSizeTests : FileTestsBase {
      public void WriteFilesCheckSize (FileSizeUnit fileSizeUnit) {
         // First, get the appropriate command
         int fileSize = 5 ;
         int fileCount = 3 ;
         int folderCount = 2 ;

         for (int folderIndex = 0; folderIndex < folderCount; folderIndex++) {
            for (int fileIndex = 0; fileIndex < fileCount; fileIndex++) {
               WriteTestFile ($"sub{folderIndex}", $"testfile.{fileIndex}", (int) UnitsEx.ConvertSizeToBytes (fileSize, fileSizeUnit)) ;
            }
         }

         var result = RunCommand (folderCount * fileCount * fileSize + 1, fileSizeUnit)[0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (folderCount * fileCount * fileSize - 1, fileSizeUnit)[0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;

         result = RunCommand (folderCount * 1 * fileSize + 1, fileSizeUnit, "*.1")[0] ;
         Assert.IsTrue (result.Success, result.ToString()) ;

         result = RunCommand (folderCount * 1 * fileSize - 1, fileSizeUnit, "*.2")[0] ;
         Assert.IsFalse (result.Success, result.ToString()) ;
      }

      [Test]
      public void WriteFiles_CheckSize() {
         WriteFilesCheckSize (FileSizeUnit.Byte) ;
         WriteFilesCheckSize (FileSizeUnit.MB) ;
      }

      private List<CheckResult> RunCommand (double maxSize,
                                            FileSizeUnit fileSizeUnit,
                                            string pattern = null) {
         return RunCommand ("checkFolderSize", () => CreateArguments (maxSize, fileSizeUnit, pattern)) ;
      }

      private List<string> CreateArguments (double maxSize,
                                            FileSizeUnit fileSizeUnit,
                                            string pattern = null) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("checkFolderSize") ;
         arguments.Add ($"--folder={_testFolder.WrapInDoubleQuotesIfContainsWhitespace()}") ;

         if (!string.IsNullOrEmpty (pattern)) {
            arguments.Add ($"--pattern=\"{pattern}\"") ;
         }
         arguments.Add ($"--maxSize={maxSize}") ;
         arguments.Add ($"--maxSizeUnit={fileSizeUnit.ToString().ToLowerInvariant()}") ;

         return arguments ;
      }
   }
}